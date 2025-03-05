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
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.XtraCharts;
using static GatiCarRental.Module.BusinessObjects.Customer;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Customer : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Customer(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _CustomerID = (Session.Evaluate<Customer>(CriteriaOperator.Parse("Max(CustomerID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Customer>(CriteriaOperator.Parse("Max(CustomerID)"), CriteriaOperator.Parse("")))) + 1;
            if (Country == null)
                Country = "India";
            if (City == null)
                City = "Kolkata";
            if (State == null)
                State = "West Bengal";

            Relation = RelationshipTypes.Self;
            Gender = GenderList.Male;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            fCreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            fCreatedOn = DateTime.Now;
        }

        [Persistent("CustomerID")] // this line for read-only columns mapping
        private int _CustomerID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_CustomerID")] // This line for read-only column mapping
        public int CustomerID
        {
            get { return _CustomerID; }
        }

        string fCustomerCode;
        [Size(20)]
        [RuleUniqueValue]
        [XafDisplayName("UIN"), ToolTip("Unique Identification Number")]
        //[RuleRequiredField]
        public string CustomerCode
        {
            get { return fCustomerCode; }
            set { SetPropertyValue<string>("CustomerCode", ref fCustomerCode, value); }
        }

        string fName;
        [Size(50)]
        //[RuleRequiredField]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        DateTime fDateOfBirth;
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        //[RuleRequiredField]
        public DateTime DateOfBirth
        {
            get { return fDateOfBirth; }
            set { SetPropertyValue<DateTime>("DateOfBirth", ref fDateOfBirth, value); }
        }

        GenderList fGender;
        //[RuleRequiredField]
        public GenderList Gender
        {
            get { return fGender; }
            set { SetPropertyValue<GenderList>("Gender", ref fGender, value); }
        }

        Department fDepartment;
        public Department Department
        {
            get { return fDepartment; }
            set { SetPropertyValue<Department>("Department", ref fDepartment, value); }
        }

        string fHeight;
        [Size(100)]
        public string Height
        {
            get { return fHeight; }
            set { SetPropertyValue<string>("Height", ref fHeight, value); }
        }

        string fWeight;
        [Size(100)]
        public string Weight
        {
            get { return fWeight; }
            set { SetPropertyValue<string>("Weight", ref fWeight, value); }
        }


        string fFathersName;
        [Size(100)]
        public string FathersName
        {
            get { return fFathersName; }
            set { SetPropertyValue<string>("FathersName", ref fFathersName, value); }
        }

        string fMothersName;
        [Size(100)]
        public string MothersName
        {
            get { return fMothersName; }
            set { SetPropertyValue<string>("MothersName", ref fMothersName, value); }
        }

        string fSpouseName;
        [Size(100)]
        public string SpouseName
        {
            get { return fSpouseName; }
            set { SetPropertyValue<string>("SpouseName", ref fSpouseName, value); }
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
        //[RuleRequiredField] // Validation for Required
        //[RuleUniqueValue] // Validation for unique value
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
        [RuleUniqueValue]
        [Size(200)]
        public string Email
        {
            get { return fEmail; }
            set { SetPropertyValue<string>("Email", ref fEmail, value); }
        }

        string fWebsite;
        [Size(200)]
        //[RuleRequiredField]
        public string Website
        {
            get { return fWebsite; }
            set { SetPropertyValue<string>("Website", ref fWebsite, value); }
        }

        string fPlaceOfBirth;
        [Size(200)]
        public string PlaceOfBirth
        {
            get { return fPlaceOfBirth; }
            set { SetPropertyValue<string>("PlaceOfBirth", ref fPlaceOfBirth, value); }
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
        [RuleUniqueValue]
        [Size(200)]
        public string PanNo
        {
            get { return fPanNo; }
            set { SetPropertyValue<string>("PanNo", ref fPanNo, value); }
        }

        string fAadharNo;
        [RuleUniqueValue]
        [Size(100)]
        public string AadharNo
        {
            get { return fAadharNo; }
            set { SetPropertyValue<string>("AadharNo", ref fAadharNo, value); }
        }

        string fBankName;
        [Size(100)]
        public string BankName
        {
            get { return fBankName; }
            set { SetPropertyValue<string>("BankName", ref fBankName, value); }
        }

        string fBranchName;
        [Size(100)]
        public string BranchName
        {
            get { return fBranchName; }
            set { SetPropertyValue<string>("BranchName", ref fBranchName, value); }
        }

        string fAccountNo;
        [Size(100)]
        public string AccountNo
        {
            get { return fAccountNo; }
            set { SetPropertyValue<string>("AccountNo", ref fAccountNo, value); }
        }

        string fIFSCCode;
        [Size(100)]
        public string IFSCCode
        {
            get { return fIFSCCode; }
            set { SetPropertyValue<string>("IFSCCode", ref fIFSCCode, value); }
        }

        string fPrevInvestment;
        [Size(200)]
        public string PrevInvestment
        {
            get { return fPrevInvestment; }
            set { SetPropertyValue<string>("PrevInvestment", ref fPrevInvestment, value); }
        }
        //=============================================================================


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

        //[RuleRequiredField("RuleRequiredField for Customer.Category", DefaultContexts.Save,
        //"Category must be specified")]
        [Association]
        public XPCollection<Category> Categories
        {
            get
            {
                return GetCollection<Category>(nameof(Categories));
            }
        }

        bool fHeadOfFamily;
        //[RuleRequiredField]
        public bool HeadOfFamily
        {
            get { return fHeadOfFamily; }
            set { SetPropertyValue<bool>("HeadOfFamily", ref fHeadOfFamily, value); }
        }

        Customer fHeadOfTheFamily;
        [DataSourceCriteria("HeadOfFamily == true")]
        [Association]
        public Customer HeadOfTheFamily
        {
            get { return fHeadOfTheFamily; }
            set { SetPropertyValue<Customer>("HeadOfTheFamily", ref fHeadOfTheFamily, value); }
        }

        RelationshipTypes fRelation;
        [Appearance("CustomerRelationCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Enabled = false, Criteria = "HeadOfTheFamily == null", Context = "DetailView")]
        public RelationshipTypes Relation
        {
            get { return fRelation; }
            set { SetPropertyValue<RelationshipTypes>("Relation", ref fRelation, value); }
        }

        Company fCompany;
        //[Browsable(false)]
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "CustomerID>0", Context = "DetailView")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        [Association]
        public XPCollection<Customer> FamilyMembers
        {
            get { return GetCollection<Customer>(nameof(FamilyMembers)); }
        }

        

        [Association]
        //[VisibleInDetailView(false)]
        public XPCollection<CallLogCustomer> CallLogs
        {
            get { return GetCollection<CallLogCustomer>(nameof(CallLogs)); }
        }

        [Association("Policy-Customer")]
        //[VisibleInDetailView(false)]
        [InverseProperty("Customer")]
        public XPCollection<Policy> Policies
        {
            get { return GetCollection<Policy>(nameof(Policies)); }
        }

        [Association("PolicyNominee-Customer")]
        [VisibleInDetailView(false)]
        public XPCollection<Policy> NomineePolicies
        {
            get { return GetCollection<Policy>(nameof(NomineePolicies)); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        public XPCollection<CustomerFileData> Attachments
        {
            get { return GetCollection<CustomerFileData>(nameof(Attachments)); }
        }

        [Association("MediclaimPolicy-Customer")]
        //[VisibleInDetailView(false)]
        [InverseProperty("Customer")]
        public XPCollection<MediclaimPolicy> MediclaimPolicyProposer
        {
            get { return GetCollection<MediclaimPolicy>(nameof(MediclaimPolicyProposer)); }
        }

        [Association("PolicyHolder-Customer")]
        public XPCollection<MediclaimPolicy> MediclaimPolicies
        {
            get { return GetCollection<MediclaimPolicy>(nameof(MediclaimPolicies)); }
        }

        [Association("MediclaimPolicyNominee-Customer")]
        [VisibleInDetailView(false)]
        public XPCollection<MediclaimPolicy> MediclaimNomineePolicies
        {
            get { return GetCollection<MediclaimPolicy>(nameof(MediclaimNomineePolicies)); }
        }

        [Association("Task-Customer")]
        //[VisibleInDetailView(false)]
        public XPCollection<Task> Services
        {
            get { return GetCollection<Task>(nameof(Services)); }
        }

        [Association("LICApplication-Customer")]
        //[VisibleInDetailView(false)]
        public XPCollection<LICApplication> LICApplications
        {
            get { return GetCollection<LICApplication>(nameof(LICApplications)); }
        }
        [Association("GICApplication-Customer")]
        //[VisibleInDetailView(false)]
        public XPCollection<GICApplication> GICApplications
        {
            get { return GetCollection<GICApplication>(nameof(GICApplications)); }
        }

        [Association("Mediclaim-Customer")]
        //[VisibleInDetailView(false)]
        public XPCollection<Mediclaim> Mediclaims
        {
            get { return GetCollection<Mediclaim>(nameof(Mediclaims)); }
        }

        [Association("Loan-Customer")]
        //[VisibleInDetailView(false)]
        public XPCollection<Loan> Loans
        {
            get { return GetCollection<Loan>(nameof(Loans)); }
        }
        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<BookingOrder> BookingOrders
        {
            get { return GetCollection<BookingOrder>(nameof(BookingOrders)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<Receipt> MoneyReceipts
        {
            get { return GetCollection<Receipt>(nameof(MoneyReceipts)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<MediclaimReceipt> MediclaimMoneyReceipts
        {
            get { return GetCollection<MediclaimReceipt>(nameof(MediclaimMoneyReceipts)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<Discount> Discounts
        {
            get { return GetCollection<Discount>(nameof(Discounts)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<TDS> TDSs
        {
            get { return GetCollection<TDS>(nameof(TDSs)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<DryInvoice> DryInvoices
        {
            get { return GetCollection<DryInvoice>(nameof(DryInvoices)); }
        }

        protected override void OnSaving()
        {

            base.OnSaving();

            if (!(this.Session is NestedUnitOfWork) && this.Oid == null)
            {
                this.CreatedOn = DateTime.Now;
                this.CreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
                //this.IsNew = false;
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
                        if (mi.Name != "Categories" && mi.Name != "FamilyMembers")
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

        public enum RelationshipTypes
            {
            None = 0,
            Self = 1,
            Husband =5,
            Father = 10,
            Mother =15,
            Wife = 20,
            Daughter = 25,
            Son = 30,
            SonInLaw =35,
            DaughterInLaw =40,
            FatherInLaw = 45,
            MotherInLaw = 50,
            Friend = 80,
            Other = 90,
            }

        public enum GenderList
        {
            Male = 1,
            Female = 2,
            Others = 3,
        }
    }
}