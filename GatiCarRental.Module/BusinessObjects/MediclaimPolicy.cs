using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using System.Collections;
using DevExpress.Xpo.Metadata;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System.ComponentModel.DataAnnotations.Schema;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class MediclaimPolicy : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public MediclaimPolicy(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _MediclaimPolicyID = (Session.Evaluate<MediclaimPolicy>(CriteriaOperator.Parse("Max(MediclaimPolicyID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<MediclaimPolicy>(CriteriaOperator.Parse("Max(MediclaimPolicyID)"), CriteriaOperator.Parse("")))) + 1;

            fCreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            fCreatedOn = DateTime.Now;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            Department = Department.Mediclaim;
        }
        [Persistent("MediclaimPolicyID")] // this line for read-only columns mapping
        private int _MediclaimPolicyID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_MediclaimPolicyID")] // This line for read-only column mapping
        public int MediclaimPolicyID
        {
            get { return _MediclaimPolicyID; }
        }

        string fPolicyNo;
        [Size(100)]
        [RuleUniqueValue]
        //[RuleRequiredField] // Validation for Required
        public string PolicyNo
        {
            get { return fPolicyNo; }
            set { SetPropertyValue<string>("PolicyNo", ref fPolicyNo, value); }
        }

        Department fDepartment;
        //[Browsable(false)]
        public Department Department
        {
            get { return fDepartment; }
            set { SetPropertyValue<Department>("Department", ref fDepartment, value); }
        }

        DateTime fStartDate;
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [RuleRequiredField("RuleRequiredField for MediclaimPolicy.StartDate", DefaultContexts.Save, "A Start Date must be specified")]
        public DateTime StartDate
        {
            get { return fStartDate; }
            set { SetPropertyValue<DateTime>("StartDate", ref fStartDate, value); }
        }

        DateTime fEndDate;
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [RuleRequiredField("RuleRequiredField for MediclaimPolicy.EndDate", DefaultContexts.Save, "A End Date must be specified")]
        public DateTime EndDate
        {
            get { return fEndDate; }
            set { SetPropertyValue<DateTime>("EndDate", ref fEndDate, value); }
        }

        private Customer fCustomer;
        [RuleRequiredField("RuleRequiredField for MediclaimPolicy.Customer", DefaultContexts.Save,
        "Customer must be specified")]
        [Association("MediclaimPolicy-Customer")]
        [ImmediatePostData]
        public Customer Customer
        {
            get { return fCustomer; }
            set
            {
                SetPropertyValue(nameof(Customer), ref fCustomer, value);
                RefreshAvailableNominees();
            }
        }

        private XPCollection<Customer> fAvailableNominees;
        [Browsable(false)] // Prohibits showing the AvailableAccessories collection separately
        public XPCollection<Customer> AvailableNominees
        {
            get
            {
                if (fAvailableNominees == null)
                {
                    // Retrieve all Sample objects
                    fAvailableNominees = new XPCollection<Customer>(Session);
                    // Filter the retrieved collection according to current conditions
                    RefreshAvailableNominees();
                }
                // Return the filtered collection of Sample objects
                return fAvailableNominees;
            }
        }

        private void RefreshAvailableNominees()
        {
            if (fAvailableNominees == null)
                return;
            // Process the situation when the Party is not specified (see the Scenario 3 above)
            if (Customer == null)
            {
                // Show only Global Collection when the Party is not specified
                fAvailableNominees.Criteria = CriteriaOperator.Parse("1=0");
            }
            else
            {
                // Leave only the current Party's Collection in the fAvailableSampleCollection collection
                //fAvailableNominees.Criteria = new BinaryOperator("HeadOfTheFamily", Customer);

                //    // Add Global Collection
                //    XPCollection<SampleCollection> availableGlobalAccessories =
                //       new XPCollection<SampleCollection>(Session);
                //    availableGlobalAccessories.Criteria = CriteriaOperator.Parse("Party",Party);
                //fAvailableSampleCollection.AddRange(availableGlobalAccessories);
            }
            // Set null for the Collection property to allow an end-user 
            //to set a new value from the refreshed data source
            Nominee = null;
        }

        [DataSourceProperty(nameof(AvailableNominees))]
        [Association("MediclaimPolicyNominee-Customer")]
        [RuleRequiredField("RuleRequiredField for MediclaimPolicy.Nominee", DefaultContexts.Save, "A Nominee must be specified")]
        //[Appearance("PickedCarNumberCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public Customer Nominee
        {
            get { return fNominee; }
            set
            {
                SetPropertyValue(nameof(Nominee), ref fNominee, value);
            }
        }
        Customer fNominee;

        InsuranceCompany fInsuranceCompany;
        public InsuranceCompany InsuranceCompany
        {
            get { return fInsuranceCompany; }
            set { SetPropertyValue<InsuranceCompany>("InsuranceCompany", ref fInsuranceCompany, value); }
        }

        MediclaimPlan fMediclaimPlan;
        public MediclaimPlan MediclaimPlan
        {
            get { return fMediclaimPlan; }
            set { SetPropertyValue<MediclaimPlan>("MediclaimPlan", ref fMediclaimPlan, value); }
        }

        PolicyType fPolicyType;
        public PolicyType PolicyType
        {
            get { return fPolicyType; }
            set { SetPropertyValue<PolicyType>("PolicyType", ref fPolicyType, value); }
        }

        bool fMedicineTaken;
        public bool MedicineTaken
        {
            get { return fMedicineTaken; }
            set { SetPropertyValue<bool>("MedicineTaken", ref fMedicineTaken, value); }
        }

        double fSumAssured;
        //[RuleRequiredField("", DefaultContexts.Save, SkipNullOrEmptyValues = false)]
        public double SumAssured
        {
            get { return fSumAssured; }
            set { SetPropertyValue<double>("SumAssured", ref fSumAssured, value); }
        }

        double fPremiumAmount;
        //[RuleRequiredField("", DefaultContexts.Save, SkipNullOrEmptyValues = false)]
        public double PremiumAmount
        {
            get { return fPremiumAmount; }
            set { SetPropertyValue<double>("PremiumAmount", ref fPremiumAmount, value); }
        }

        private Employee fAgents;
        [RuleRequiredField("RuleRequiredField for MediclaimPolicy.Agents", DefaultContexts.Save,
        "Agents must be specified")]
        //[Appearance("AgentPolicyCond", Enabled = false, Criteria = "@this.Agents.TypeOfEmployee='Agent'", Context = "DetailView")]
        [Association("Mediclaim-Agents")]
        public Employee Agents
        {
            get { return fAgents; }
            set
            {
                SetPropertyValue(nameof(Agents), ref fAgents, value);
            }
        }

        PolicyStatus fMediclaimPolicyStatus;
        public PolicyStatus MediclaimPolicyStatus
        {
            get { return fMediclaimPolicyStatus; }
            set { SetPropertyValue<PolicyStatus>("MediclaimPolicyStatus", ref fMediclaimPolicyStatus, value); }
        }

        string fPreviousProposalNo;
        [Size(100)]
        //[RuleRequiredField] // Validation for Required
        public string PreviousProposalNo
        {
            get { return fPreviousProposalNo; }
            set { SetPropertyValue<string>("PreviousProposalNo", ref fPreviousProposalNo, value); }
        }

        MediclaimPolicy fPreviousPolicy;
        [Association]
        //[RuleRequiredField] // Validation for Required
        public MediclaimPolicy PreviousPolicy
        {
            get { return fPreviousPolicy; }
            set { SetPropertyValue<MediclaimPolicy>("PreviousPolicy", ref fPreviousPolicy, value); }
        }

        string fCurrentProposalNo;
        [Size(100)]
        //[RuleRequiredField] // Validation for Required
        public string CurrentProposalNo
        {
            get { return fCurrentProposalNo; }
            set { SetPropertyValue<string>("CurrentProposalNo", ref fCurrentProposalNo, value); }
        }

        private PermissionPolicyUser fCreatedBy;
        //[Appearance("AgentPolicyCond", Enabled = false, Criteria = "@this.Agents.TypeOfEmployee='Agent'", Context = "DetailView")]
        [ModelDefault(nameof(IModelCommonMemberViewItem.AllowEdit), "False")]
        public PermissionPolicyUser CreatedBy
        {
            get { return fCreatedBy; }
            set
            {
                SetPropertyValue(nameof(CreatedBy), ref fCreatedBy, value);
            }
        }

        DateTime fCreatedOn;
        [ModelDefault(nameof(IModelCommonMemberViewItem.AllowEdit), "False")]
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy hh:mm tt}")]
        [ModelDefault("EditMask", "dd/MM/yyyy hh:mm tt")]
        public DateTime CreatedOn
        {
            get { return fCreatedOn; }
            set { SetPropertyValue<DateTime>("CreatedOn", ref fCreatedOn, value); }
        }

        Company fCompany;
        [Browsable(false)]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        [Association("PolicyHolder-Customer")]
        public XPCollection<Customer> PolicyHolders
        {
            get
            {
                XPCollection<Customer> collection = GetCollection<Customer>(nameof(PolicyHolders));
                collection.Criteria = new BinaryOperator(nameof(Customer.HeadOfTheFamily), this.Customer);
                return collection;
            }
        }

        [DevExpress.Xpo.Aggregated, Association]
        public XPCollection<MediclaimPolicyFileData> Attachments
        {
            get { return GetCollection<MediclaimPolicyFileData>(nameof(Attachments)); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        public XPCollection<MediclaimReceipt> PaymentReceipts
        {
            get { return GetCollection<MediclaimReceipt>(nameof(PaymentReceipts)); }
        }
        [Association]
        public XPCollection<MediclaimPolicy> PreviousPolicies
        {
            get { return GetCollection<MediclaimPolicy>(nameof(PreviousPolicies)); }
        }
        protected override void OnSaving()
        {
            base.OnSaving();

            if (!(this.Session is NestedUnitOfWork))
            {
                this.CreatedOn = DateTime.Now;
                this.CreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
                this.Save();
            }
        }

        protected override void OnDeleting()
        {
            base.OnDeleting();


            ICollection objs = Session.CollectReferencingObjects(this);
            if (objs.Count > 0)
            {
                foreach (XPMemberInfo mi in ClassInfo.CollectionProperties)
                {
                    //if (mi.IsAggregated && mi.IsCollection && mi.IsAssociation)
                    if (mi.IsAssociation)
                    {
                        foreach (IXPObject obj in objs)
                        {
                            if (obj != null)
                            {

                                if (string.IsNullOrEmpty(mi.DisplayName))
                                    throw new UserFriendlyException($"{this.PolicyNo.ToString()} Cannot be deleted. It is refrenced in: {mi.Name}");
                                else
                                    throw new UserFriendlyException($"{this.PolicyNo.ToString()} Cannot be deleted. It is refrenced in: {mi.DisplayName}");
                            }

                        }
                    }
                }
            }


        }
    }

    public enum PolicyType
    {
        FamilyFloater=0,
        Individual = 1,
    }
}