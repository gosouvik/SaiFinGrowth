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
using DevExpress.ExpressApp.ConditionalAppearance;
using System.Collections;
using DevExpress.Xpo.Metadata;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Lead : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Lead(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _LeadID = (Session.Evaluate<Lead>(CriteriaOperator.Parse("Max(LeadID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Lead>(CriteriaOperator.Parse("Max(LeadID)"), CriteriaOperator.Parse("")))) + 1;
            if (Country == null)
                Country = "India";
            if (City == null)
                City = "Kolkata";
            if (State == null)
                State = "West Bengal";

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
        }

        [Persistent("LeadID")] // this line for read-only columns mapping
        private int _LeadID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_LeadID")] // This line for read-only column mapping
        public int LeadID
        {
            get { return _LeadID; }
        }

        string fName;
        [Size(50)]
        [RuleRequiredField("RuleRequiredField for Lead.Name", DefaultContexts.Save,
      "A Name must be specified")]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }


        Department fDepartment;
        public Department Department
        {
            get { return fDepartment; }
            set { SetPropertyValue<Department>("Department", ref fDepartment, value); }
        }

        string fAddress1;
        [Size(500)]
        //[RuleRequiredField] // Validation for Required
        public string Address1
        {
            get { return fAddress1; }
            set { SetPropertyValue<string>("Address1", ref fAddress1, value); }
        }

        string fAddress2;
        [Size(500)]
        //[RuleRequiredField] // Validation for Required
        public string Address2
        {
            get { return fAddress2; }
            set { SetPropertyValue<string>("Address2", ref fAddress2, value); }
        }

        string fCity;
        [Size(50)]
        //[RuleRequiredField] // Validation for Required
        public string City
        {
            get { return fCity; }
            set { SetPropertyValue<string>("City", ref fCity, value); }
        }
        string fState;
        [Size(50)]
        //[RuleRequiredField] // Validation for Required
        public string State
        {
            get { return fState; }
            set { SetPropertyValue<string>("State", ref fState, value); }
        }
        string fCountry;
        [Size(50)]
        //[RuleRequiredField] // Validation for Required
        public string Country
        {
            get { return fCountry; }
            set { SetPropertyValue<string>("Country", ref fCountry, value); }
        }
        string fPinCode;
        [Size(10)]
        //[RuleRequiredField] // Validation for Required
        public string PinCode
        {
            get { return fPinCode; }
            set { SetPropertyValue<string>("PinCode", ref fPinCode, value); }
        }
        string fPhone1;
        [Size(20)]
        [RuleRequiredField("RuleRequiredField for Lead.Phone1", DefaultContexts.Save,
      "Phone must be specified")] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        public string Phone1
        {
            get { return fPhone1; }
            set { SetPropertyValue<string>("Phone1", ref fPhone1, value); }
        }
        string fPhone2;
        [Size(20)]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.NotEquals, "Phone1", ParametersMode.Expression)]
        public string Phone2
        {
            get { return fPhone2; }
            set { SetPropertyValue<string>("Phone2", ref fPhone2, value); }
        }
        string fEmail;
      //  [RuleRequiredField("RuleRequiredField for Lead.Email", DefaultContexts.Save,
      //"A Email must be specified")]
        [RuleRegularExpression("", DefaultContexts.Save,
    @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")]
        [Size(200)]
        public string Email
        {
            get { return fEmail; }
            set { SetPropertyValue<string>("Email", ref fEmail, value); }
        }

        decimal fMonthlyIncome;
        public decimal MonthlyIncome
        {
            get { return fMonthlyIncome; }
            set { SetPropertyValue<decimal>("MonthlyIncome", ref fMonthlyIncome, value); }
        }

        string fOccupation;
        [Size(200)]
        public string Occupation
        {
            get { return fOccupation; }
            set { SetPropertyValue<string>("Occupation", ref fOccupation, value); }
        }

        string fQualification;
        [Size(200)]
        public string Qualification
        {
            get { return fQualification; }
            set { SetPropertyValue<string>("Qualification", ref fQualification, value); }
        }

        string fPanNo;
        [Size(200)]
        public string PanNo
        {
            get { return fPanNo; }
            set { SetPropertyValue<string>("PanNo", ref fPanNo, value); }
        }

        string fAadharNo;
        [Size(100)]
        public string AadharNo
        {
            get { return fAadharNo; }
            set { SetPropertyValue<string>("AadharNo", ref fAadharNo, value); }
        }

        private Employee fAssignTo;
        [RuleRequiredField("RuleRequiredField for Lead.AssignTo", DefaultContexts.Save,
        "Assign To must be specified")]
        //[Appearance("AgentPolicyCond", Enabled = false, Criteria = "@this.Agents.TypeOfEmployee='Agent'", Context = "DetailView")]
        [Association("Lead-Employee")]
        public Employee AssignTo
        {
            get { return fAssignTo; }
            set
            {
                SetPropertyValue(nameof(AssignTo), ref fAssignTo, value);
            }
        }

        string fReasonOfCreateLead;
        [Size(500)]
        public string ReasonOfCreateLead
        {
            get { return fReasonOfCreateLead; }
            set { SetPropertyValue<string>("REASON OF CREATE LEAD ", ref fReasonOfCreateLead, value); }
        }
        string fQUERYOFLead;
        [Size(500)]
        public string QueryLead
        {
            get { return fQUERYOFLead; }
            set { SetPropertyValue<string>("QUERY OF LEAD ", ref fQUERYOFLead, value); }
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
        //[Browsable(false)]
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "LeadID>0", Context = "DetailView")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        [Association]
        //[VisibleInDetailView(false)]
        public XPCollection<CallLogLead> CallLogs
        {
            get { return GetCollection<CallLogLead>(nameof(CallLogs)); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        //[VisibleInDetailView(false)]
        public XPCollection<LeadFileData> Attachments
        {
            get { return GetCollection<LeadFileData>(nameof(Attachments)); }
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
                                if (((DevExpress.Xpo.XPBaseCollection)mi.GetValue(this)).BaseIndexOf(obj) >= 0)
                                {
                                    if (string.IsNullOrEmpty(mi.DisplayName))
                                        throw new UserFriendlyException($"{this.Name.ToString()} Cannot be deleted. It is refrenced in: {mi.Name}");
                                    else
                                        throw new UserFriendlyException($"{this.Name.ToString()} Cannot be deleted. It is refrenced in: {mi.DisplayName}");
                                }
                            }
                            
                        }
                    }
                }
            }

            
        }
    }
}