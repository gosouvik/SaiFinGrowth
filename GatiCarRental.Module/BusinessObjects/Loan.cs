using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty("DocumentNo")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Loan : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Loan(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _LoanID = (Session.Evaluate<Loan>(CriteriaOperator.Parse("Max(LoanID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Loan>(CriteriaOperator.Parse("Max(LoanID)"), CriteriaOperator.Parse("")))) + 1;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            fCreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            fCreatedOn = DateTime.Now;
            fMaritalStatus = MaritalStatusList.Married;
            fCoMaritalStatus = CoMaritalStatusList.Married;
            fOCCUPATION = OCCUPATION.BUSINESS;
            fCoOCCUPATION = CoOCCUPATION.BUSINESS;
            fLoanCategory = LoanCategory.Other;
            fBTREQUIRED = BTREQUIRED.HOAM_LOAN_BT;
            //CallResponse = CallResponseType.NotPickup;
            //DocumentNo = "TDS-" + (_TDSID).ToString();
            GetDocumentNumbering();
            DocumentDate = DateTime.Today;
            Department = Department.Loan;
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }


        private void GetDocumentNumbering()
        {

            XPCollection<DocumentNumbering> docnos = new XPCollection<DocumentNumbering>(Session);
            docnos.Criteria = CriteriaOperator.Parse("[DocumentType]=?", DocumentType.Loan);
            if (docnos.Count == 1)
            {
                foreach (DocumentNumbering docno in docnos)
                {
                    DocumentNo = docno.Prefix + new String('X', docno.Body) + docno.Suffix;
                    DocSchemeOid = docno.Oid;
                    IsNew = true;
                }
            }
            Department = Department.Loan;

        }

        Guid fDocSchemeOid;
        [Browsable(false)]
        public Guid DocSchemeOid
        {
            get { return fDocSchemeOid; }
            set { SetPropertyValue<Guid>("DocSchemeOid", ref fDocSchemeOid, value); }
        }

        bool fIsNew;
        [Browsable(false)]
        [DefaultValue(false)]
        public bool IsNew
        {
            get { return fIsNew; }
            set { SetPropertyValue<bool>("IsNew", ref fIsNew, value); }
        }
       

        [Persistent("LoanID")] // this line for read-only columns mapping
        private int _LoanID;
        [RuleUniqueValue] // Validation for unique value
        //RuleRequiredField("RuleRequiredField for CallLogLead.CallLogLeadID", DefaultContexts.Save, "Call Log Lead ID must be specified")] // Validation for Required
        [Browsable(false)]
        [PersistentAlias("_LoanID")] // This line for read-only column mapping
        public int LoanID
        {
            get { return _LoanID; }
        }

        string fDocumentNo;
        [Appearance("LoanDocNoCond", Enabled = false, Criteria = "IsNew=1", Context = "Any")]
        [RuleUniqueValue]
        [Size(50)]
        public string DocumentNo
        {
            get { return fDocumentNo; }
            set { SetPropertyValue<string>("DocumentNo", ref fDocumentNo, value); }
        }

        DateTime fDocumentDate;
        public DateTime DocumentDate
        {
            get { return fDocumentDate; }
            set { SetPropertyValue<DateTime>("DocumentDate", ref fDocumentDate, value); }
        }

        Department fDepartment;
        //[Browsable(false)]
        public Department Department
        {
            get { return fDepartment; }
            set { SetPropertyValue<Department>("Department", ref fDepartment, value); }
        }

        //private Employee fAssignTo;
        ////[RuleRequiredField("RuleRequiredField for Loan.AssignTo", DefaultContexts.Save,
        ////"Agents must be specified")]
        ////[Appearance("AgentPolicyCond", Enabled = false, Criteria = "@this.Agents.TypeOfEmployee='Agent'", Context = "DetailView")]
        //[Association("Loan-Employee")]
        //public Employee AssignTo
        //{
        //    get { return fAssignTo; }
        //    set
        //    {
        //        SetPropertyValue(nameof(AssignTo), ref fAssignTo, value);


        //    }
        //}

        private Customer fCustomer;
        [Association("Loan-Customer")]
        [ImmediatePostData]
        public Customer Customer
        {
            get { return fCustomer; }
            set
            {
                SetPropertyValue(nameof(Customer), ref fCustomer, value);
            }
        }


        ///// <Swati>


        string ApplicantName;
        [Size(200)]
        [XafDisplayName("Applicant's Name"), ToolTip("Please enter Applicant Name")]
        //[RuleRequiredField]
        public string ApplicantNames
        {
            get { return ApplicantName; }
            set { SetPropertyValue<string>("Applicant's Name", ref ApplicantName, value); }
        }

       
        string CoApplicantName;
        [Size(200)]
        [XafDisplayName("Co Applicant's Name"), ToolTip("Please enter Applicant Name")]
        //[RuleRequiredField]
        public string CoApplicantNames
        {
            get { return CoApplicantName; }
            set { SetPropertyValue<string>("Co Applicant's Name", ref CoApplicantName, value); }
        }

        DateTime fDateOfBirth;
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        //[RuleRequiredField]
        public DateTime DateOfBirth
        {
            get { return fDateOfBirth; }
            set { SetPropertyValue<DateTime>("Applicant's DateOfBirth", ref fDateOfBirth, value); }
        }

        DateTime fCoDateOfBirth;
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        //[RuleRequiredField]
        public DateTime CoDateOfBirth
        {
            get { return fCoDateOfBirth; }
            set { SetPropertyValue<DateTime>("Co Applicant's DateOfBirth", ref fCoDateOfBirth, value); }
        }

        MaritalStatusList fMaritalStatus;
        //[RuleRequiredField]
        public MaritalStatusList MaritalStatu
        {
            get { return fMaritalStatus; }
            set { SetPropertyValue<MaritalStatusList>("Marital Status", ref fMaritalStatus, value); }
        }

        CoMaritalStatusList fCoMaritalStatus;
        //[RuleRequiredField]
        public CoMaritalStatusList CoMaritalStatus
        {
            get { return fCoMaritalStatus; }
            set { SetPropertyValue<CoMaritalStatusList>("Co Marital Status", ref fCoMaritalStatus, value); }
        }


        string fPhone;
        [Size(20)]
        [RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        public string Phone
        {
            get { return fPhone; }
            set { SetPropertyValue<string>("Applicant's Phone", ref fPhone, value); }
        }

        string fCoPhone;
        [Size(20)]
        [RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        public string CoPhone
        {
            get { return fCoPhone; }
            set { SetPropertyValue<string>("Co Applicant's Phone", ref fCoPhone, value); }
        }
        string fEmail;
        [RuleUniqueValue]
        [Size(200)]
        public string Email
        {
            get { return fEmail; }
            set { SetPropertyValue<string>("Applicant's Email", ref fEmail, value); }
        }
        string fCoEmail;
        [RuleUniqueValue]
        [Size(200)]
        public string CoEmail
        {
            get { return fCoEmail; }
            set { SetPropertyValue<string>("CoApplicant's Email", ref fCoEmail, value); }
        }

        string fFullAddress;
        [Size(500)]
        //[RuleRequiredField] // Validation for Required
        public string FullAddress
        {
            get { return fFullAddress; }
            set { SetPropertyValue<string>("Full Address", ref fFullAddress, value); }
        }

        string fApplicantRelation;
        [Size(100)]
        public string ApplicantRelation
        {
            get { return fApplicantRelation; }
            set { SetPropertyValue<string>("Relation With Co Applicant's", ref fApplicantRelation, value); }
        }
        OCCUPATION fOCCUPATION;
        //[RuleRequiredField]
        public OCCUPATION OCCUPATIONs
        {
            get { return fOCCUPATION; }
            set { SetPropertyValue<OCCUPATION>("Appicant's Occupation", ref fOCCUPATION, value); }
        }

        CoOCCUPATION fCoOCCUPATION;
        //[RuleRequiredField]
        public CoOCCUPATION CoOCCUPATION
        {
            get { return fCoOCCUPATION; }
            set { SetPropertyValue<CoOCCUPATION>("Co Appicant's Occupation", ref fCoOCCUPATION, value); }
        }
        Double fYearlyIncome;

        //[Appearance("PickedPricePerHourCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        //[Appearance("IsDryPricePerHour", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        //[RuleRange("", DefaultContexts.Save, 1, 999999, TargetCriteria = "IsDry=0")]
        [DefaultValue(0)]
        public Double YearlyIncome

        {
            get { return fYearlyIncome; }
            set { SetPropertyValue<Double>("Applicant's YearlyIncome", ref fYearlyIncome, value); }
        }

        Double fCoYearlyIncome;

        //[Appearance("PickedPricePerHourCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        //[Appearance("IsDryPricePerHour", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        //[RuleRange("", DefaultContexts.Save, 1, 999999, TargetCriteria = "IsDry=0")]
        [DefaultValue(0)]
        public Double CoYearlyIncome

        {
            get { return fCoYearlyIncome; }
            set { SetPropertyValue<Double>("Co Applicants YearlyIncome", ref fCoYearlyIncome, value); }
        }

        bool fPrevInvestment;
        //[Browsable(false)]
        [DefaultValue(false)]
        public bool PrevInvestment
        {
            get { return fPrevInvestment; }
            set { SetPropertyValue<bool>("Any Previous Investment", ref fPrevInvestment, value); }
        }

        string fPanNo;
        [RuleUniqueValue]
        [Size(200)]
        public string PanNo
        {
            get { return fPanNo; }
            set { SetPropertyValue<string>("Applicant's Pan No", ref fPanNo, value); }
        }
        string fCoPanNo;
        [RuleUniqueValue]
        [Size(200)]
        public string CoPanNo
        {
            get { return fCoPanNo; }
            set { SetPropertyValue<string>("CoApplicant's Pan No", ref fCoPanNo, value); }
        }

        LoanCategory fLoanCategory;
        //[RuleRequiredField]
        public LoanCategory LoanCategory
        {
            get { return fLoanCategory; }
            set { SetPropertyValue<LoanCategory>("Loan Category", ref fLoanCategory, value); }
        }

        BTREQUIRED fBTREQUIRED;
        //[RuleRequiredField]
        public BTREQUIRED BTREQUIRED
        {
            get { return fBTREQUIRED; }
            set { SetPropertyValue<BTREQUIRED>("BT REQUIRED", ref fBTREQUIRED, value); }
        }

        Double fExpectedLoanAmt;

        //[Appearance("PickedPricePerHourCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        //[Appearance("IsDryPricePerHour", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        //[RuleRange("", DefaultContexts.Save, 1, 999999, TargetCriteria = "IsDry=0")]
        [DefaultValue(0)]
        public Double ExpectedLoanAmt

        {
            get { return fExpectedLoanAmt; }
            set { SetPropertyValue<Double>("Expected LoanAmt", ref fExpectedLoanAmt, value); }
        }

        
        private LoanStatus status;
        public virtual LoanStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                //if (isLoaded)
                //{
                    if (value == LoanStatus.Closed)
                    {
                        DateCompleted = DateTime.Now;
                    }
                    else
                    {
                        DateCompleted = Convert.ToDateTime("1900-01-01");
                    }
                //}
            }
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

        private Employee fCreatedByName;
        public Employee CreatedByName
        {
            get
                { return fCreatedByName; }
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

        DateTime fDateCompleted;
        [ModelDefault(nameof(IModelCommonMemberViewItem.AllowEdit), "False")]
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy hh:mm tt}")]
        [ModelDefault("EditMask", "dd/MM/yyyy hh:mm tt")]
        public DateTime DateCompleted
        {
            get { return fDateCompleted; }
            set { SetPropertyValue<DateTime>("DateCompleted", ref fDateCompleted, value); }
        }


       

        [DevExpress.Xpo.Aggregated, Association]
        public XPCollection<LoanFileData> Attachments
        {
            get { return GetCollection<LoanFileData>(nameof(Attachments)); }
        }     
               

        Company fCompany;
        //[Browsable(false)]
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "LoanID>0", Context = "Any")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }



        //[Association("MyLoan-LoanNotification")]
        //public XPCollection<LoanNotification> MyNotifications
        //{
        //    get
        //    {
        //        return GetCollection<LoanNotification>(nameof(MyNotifications));
        //    }
        //}

        [Action(ImageName = "State_Loan_Completed")]
        //[Appearance("REmoveHoldCond", Enabled = false, Criteria = "Status != 0", Context = "Any")]
        public void RemoveHold()
        {
            Status = LoanStatus.PendingApproval;
        }
        
        [Action(ImageName = "State_Loan_Approved")]
        public void Approved()
        {
            Status = LoanStatus.Approved;
        }
        [Action(ImageName = "State_Loan_Submitted")]
        public void Submitted()
        {
            Status = LoanStatus.Submitted;
        }
        [Action(ImageName = "State_Loan_Inprocess")]
        public void Inprocess()
        {
            Status = LoanStatus.Inprocess;
        }
        [Action(ImageName = "State_Loan_Rejected")]
        public void Rejected()
        {
            Status = LoanStatus.Rejected;
        }
        [Action(ImageName = "State_Loan_Closed")]
        public void Closed()
        {
            Status = LoanStatus.Closed;
        }

        [Action(ImageName = "State_Loan_Closed")]
        public void ConvertToPolicy()
        {
            Status = LoanStatus.ConvertToPolicy;
        }
        protected override void OnSaving()
        {
            int newnum = 1;
            DocumentNumbering docnos = Session.GetObjectByKey<DocumentNumbering>(this.DocSchemeOid);
            if (!(this.Session is NestedUnitOfWork) && Session.IsNewObject(this))
            {
                if (this.IsNew == true)
                {
                    newnum = docnos.CurrentNo + 1;
                    this.DocumentNo = docnos.Prefix + new string('0', docnos.Body - (newnum).ToString().Length) + (newnum).ToString() + docnos.Suffix;
                }
            }
            base.OnSaving();

            if (!(this.Session is NestedUnitOfWork) && this.IsNew == true)
            {
                docnos.CurrentNo = newnum;
                docnos.Save();
                this.CreatedOn = DateTime.Now;
                this.CreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
                this.IsNew = false;
                this.Save();
            }
        }

        private XPCollection<AuditDataItemPersistent> auditTrail;
        [CollectionOperationSet(AllowAdd = false, AllowRemove = false)]
        public XPCollection<AuditDataItemPersistent> Activities
        {
            get
            {
                if (auditTrail == null)
                {
                    auditTrail = AuditedObjectWeakReference.GetAuditTrail(Session, this);
                }
                return auditTrail;
            }
        }
        //private string _PersistentProperty;
        //[XafDisplayName("My display name"), ToolTip("My hint message")]
        //[ModelDefault("EditMask", "(000)-00"), Index(0), VisibleInListView(false)]
        //[Persistent("DatabaseColumnName"), RuleRequiredField(DefaultContexts.Save)]
        //public string PersistentProperty {
        //    get { return _PersistentProperty; }
        //    set { SetPropertyValue(nameof(PersistentProperty), ref _PersistentProperty, value); }
        //}

        //[Action(Caption = "My UI Action", ConfirmationMessage = "Are you sure?", ImageName = "Attention", AutoCommit = true)]
        //public void ActionMethod() {
        //    // Trigger a custom business logic for the current record in the UI (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112619.aspx).
        //    this.PersistentProperty = "Paid";
        //}
    }

    public enum LoanStatus
    {
        [ImageName("State_Loan_NotStarted")]
        Hold,
        [ImageName("State_Loan_PendingApproval")]
        PendingApproval,
        [ImageName("State_Loan_Approved")]
        Approved,
        [ImageName("State_Loan_Submitted")]
        Submitted,
        [ImageName("State_Loan_Rejected")]
        Rejected,
        [ImageName("State_Loan_Inprocess")]
        Inprocess,
        [ImageName("State_Loan_Closed")]
        Closed,
        ConvertToPolicy
    }

    public enum CoMaritalStatusList
    {
        Married = 1,
        UnMarried = 2,
        Others = 3,
    }
    public enum OCCUPATION

    {
        SERVICE = 1,
        BUSINESS = 2,
        SELFEMPLOYED = 3
    }
    public enum CoOCCUPATION

    {
        SERVICE = 1,
        BUSINESS = 2,
        SELFEMPLOYED = 3
    }
    public enum LoanCategory
    {
        HOAM_LOAN = 1,
        MORTGAGE_LOAN = 2,
        COMMERCIAL_LOAN = 3,
        LAND_PURCHASE_LOAN = 4,
        SELF_CONSTRUCTION_LOAN = 5,
        BALANCE_TRANSFER_LOAN = 6,       
        Other = 9
    }
    public enum BTREQUIRED
    {
        HOAM_LOAN_BT = 1,
        MORTGAGE_LOAN_BT = 2,
        COMMERCIAL_LOAN_BT = 3,
        LAND_PURCHASE_LOAN_BT = 4,
        SELF_CONSTRUCTION_LOAN_BT = 5,
        
    }
}