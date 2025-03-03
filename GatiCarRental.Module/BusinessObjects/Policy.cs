using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using static GatiCarRental.Module.BusinessObjects.Customer;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Policy : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Policy(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _PolicyID = (Session.Evaluate<Policy>(CriteriaOperator.Parse("Max(PolicyID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Policy>(CriteriaOperator.Parse("Max(PolicyID)"), CriteriaOperator.Parse("")))) + 1;

            //LICPolicyStatus = PolicyStatus.Paid;
            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            fCreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            fCreatedOn = DateTime.Now;
            Department = Department.LICI;
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        [Persistent("PolicyID")] // this line for read-only columns mapping
        private int _PolicyID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_PolicyID")] // This line for read-only column mapping
        public int PolicyID
        {
            get { return _PolicyID; }
        }

        string fPolicyNumber;
        [Size(100)]
        [RuleUniqueValue] // Validation for unique value
        [RuleRequiredField("RuleRequiredField for Policy.PolicyNumber", DefaultContexts.Save, "A Policy Number must be specified")]
        public string PolicyNumber
        {
            get { return fPolicyNumber; }
            set { SetPropertyValue<string>("PolicyNumber", ref fPolicyNumber, value); }
        }

        Department fDepartment;
        //[Browsable(false)]
        public Department Department
        {
            get { return fDepartment; }
            set { SetPropertyValue<Department>("Department", ref fDepartment, value); }
        }

        private Customer fCustomer;
        [RuleRequiredField("RuleRequiredField for Policy.Customer", DefaultContexts.Save,
        "Customer must be specified")]
        [InverseProperty("Policies")]
        [Association("Policy-Customer")]
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

        private Employee fAgents;
        [RuleRequiredField("RuleRequiredField for Policy.Agents", DefaultContexts.Save,
        "Agents must be specified")]
        //[Appearance("AgentPolicyCond", Enabled = false, Criteria = "@this.Agents.TypeOfEmployee='Agent'", Context = "DetailView")]
        [Association]
        public Employee Agents
        {
            get { return fAgents; }
            set
            {
                SetPropertyValue(nameof(Agents), ref fAgents, value);
            }
        }
        
        [Association]
        [RuleRequiredField("RuleRequiredField for Policy.Plan", DefaultContexts.Save, "A Plan must be specified")]
        //[Appearance("PickedCarNumberCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public Plan Plan
        {
            get { return fPlan; }
            set
            {
                SetPropertyValue(nameof(Plan), ref fPlan, value);

            }
        }
        Plan fPlan;

        double fTerm;
        //[RuleRequiredField("", DefaultContexts.Save, SkipNullOrEmptyValues = false)]
        public double Term
        {
            get { return fTerm; }
            set { SetPropertyValue<double>("Term", ref fTerm, value); }
        }

        double fPPT;
        //[RuleRequiredField("", DefaultContexts.Save, SkipNullOrEmptyValues = false)]
        public double PPT
        {
            get { return fPPT; }
            set { SetPropertyValue<double>("PPT", ref fPPT, value); }
        }


        DateTime fStartDate;
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [RuleRequiredField("RuleRequiredField for Policy.StartDate", DefaultContexts.Save, "A Start Date must be specified")]
        public DateTime StartDate
        {
            get { return fStartDate; }
            set { SetPropertyValue<DateTime>("StartDate", ref fStartDate, value); }
        }

        DateTime fMaturityDate;
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [RuleRequiredField("RuleRequiredField for Policy.MaturityDate", DefaultContexts.Save, "A Start Date must be specified")]
        public DateTime MaturityDate
        {
            get { return fMaturityDate; }
            set { SetPropertyValue<DateTime>("MaturityDate", ref fMaturityDate, value); }
        }

        DateTime fFUPDate;
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        //[RuleRequiredField("RuleRequiredField for Policy.FUPDate", DefaultContexts.Save, "A FUP Date must be specified")]
        public DateTime FUPDate
        {
            get { return fFUPDate; }
            set { SetPropertyValue<DateTime>("FUPDate", ref fFUPDate, value); }
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

        [DataSourceProperty(nameof(AvailableNominees))]
        [Association("PolicyNominee-Customer")]
        [ImmediatePostData]
        //[RuleRequiredField("RuleRequiredField for Policy.Nominee", DefaultContexts.Save, "A Nominee must be specified")]
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

        RelationshipTypes fRelation;
        [Appearance("LICPolicyRelationCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Enabled = false, Criteria = "Nominee == null", Context = "DetailView")]
        public RelationshipTypes Relation
        {
            get { return fRelation; }
            set { SetPropertyValue<RelationshipTypes>("Relation", ref fRelation, value); }
        }


        PolicyStatus fLICPolicyStatus;
        public PolicyStatus LICPolicyStatus
        {
            get { return fLICPolicyStatus; }
            set { SetPropertyValue<PolicyStatus>("LICPolicyStatus", ref fLICPolicyStatus, value); }
        }

        PaymentFrequency fPaymentMode;
        public PaymentFrequency PaymentMode
        {
            get { return fPaymentMode; }
            set { SetPropertyValue<PaymentFrequency>("PaymentMode", ref fPaymentMode, value); }
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

        [DevExpress.Xpo.Aggregated, Association]
        public XPCollection<Receipt> PaymentReceipts
        {
            get { return GetCollection<Receipt>(nameof(PaymentReceipts)); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        public XPCollection<LicPolicyFileData> Attachments
        {
            get { return GetCollection<LicPolicyFileData>(nameof(Attachments)); }
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
                                    throw new UserFriendlyException($"{this.PolicyNumber.ToString()} Cannot be deleted. It is refrenced in: {mi.Name}");
                                else
                                    throw new UserFriendlyException($"{this.PolicyNumber.ToString()} Cannot be deleted. It is refrenced in: {mi.DisplayName}");
                            }

                        }
                    }
                }
            }


        }

    }

    public enum PolicyStatus
    {
        None = 0,
        Paid=5,
        Inforce = 10,
        RedPaidUp=15,
        Lapsed=20,
        Matured =25,
        Surrendered=30
    }

    public enum PaymentFrequency
    {
        None =0,
        Monthly = 5,
        Quarterly = 10,
        HalfYearly = 15,
        Yearly = 20,
        Single = 25
    }
}