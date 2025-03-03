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
    public class LICApplication : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public LICApplication(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _LICApplicationID = (Session.Evaluate<LICApplication>(CriteriaOperator.Parse("Max(LICApplicationID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<LICApplication>(CriteriaOperator.Parse("Max(LICApplicationID)"), CriteriaOperator.Parse("")))) + 1;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            fCreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            fCreatedOn = DateTime.Now;
            MaritalStatu = MaritalStatusList.Married;
            //CallResponse = CallResponseType.NotPickup;
            //DocumentNo = "TDS-" + (_TDSID).ToString();
            GetDocumentNumbering();
            DocumentDate = DateTime.Today;
            Department = Department.LICI;
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }


        private void GetDocumentNumbering()
        {

            XPCollection<DocumentNumbering> docnos = new XPCollection<DocumentNumbering>(Session);
            docnos.Criteria = CriteriaOperator.Parse("[DocumentType]=?", DocumentType.LICApplication);
            if (docnos.Count == 1)
            {
                foreach (DocumentNumbering docno in docnos)
                {
                    DocumentNo = docno.Prefix + new String('X', docno.Body) + docno.Suffix;
                    DocSchemeOid = docno.Oid;
                    IsNew = true;
                }
            }
            Department = Department.LICI;

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
       

        [Persistent("LICApplicationID")] // this line for read-only columns mapping
        private int _LICApplicationID;
        [RuleUniqueValue] // Validation for unique value
        //RuleRequiredField("RuleRequiredField for CallLogLead.CallLogLeadID", DefaultContexts.Save, "Call Log Lead ID must be specified")] // Validation for Required
        [Browsable(false)]
        [PersistentAlias("_LICApplicationID")] // This line for read-only column mapping
        public int LICApplicationID
        {
            get { return _LICApplicationID; }
        }

        string fDocumentNo;
        [Appearance("LICApplicationDocNoCond", Enabled = false, Criteria = "IsNew=1", Context = "Any")]
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

        private Employee fAssignTo;
        //[RuleRequiredField("RuleRequiredField for LICApplication.AssignTo", DefaultContexts.Save,
        //"Agents must be specified")]
        //[Appearance("AgentPolicyCond", Enabled = false, Criteria = "@this.Agents.TypeOfEmployee='Agent'", Context = "DetailView")]
        [Association("LICApplication-Employee")]
        public Employee AssignTo
        {
            get { return fAssignTo; }
            set
            {
                SetPropertyValue(nameof(AssignTo), ref fAssignTo, value);
                

            }
        }

        private Customer fCustomer;
        [Association("LICApplication-Customer")]
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


        string residentialStatus;
        [Size(100)]
        [XafDisplayName("ResidentialStatus"), ToolTip("Please enter Residential Status")]
        //[RuleRequiredField]
        public string ResidentialStatus
        {
            get { return residentialStatus; }
            set { SetPropertyValue<string>("ResidentialStatus", ref residentialStatus, value); }
        }

        [Association]
        [RuleRequiredField("RuleRequiredField for LICApplication.Plan", DefaultContexts.Save, "A Plan must be specified")]
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
        //string planName;
        //[Size(200)]
        //[XafDisplayName("PlanName"), ToolTip("Please enter Plan Name")]
        //[RuleRequiredField]
        //public string PlanName
        //{
        //    get { return planName; }
        //    set { SetPropertyValue<string>("PlanName", ref planName, value); }
        //}

        Double fBasicSumAssured;

        //[Appearance("PickedPricePerHourCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        //[Appearance("IsDryPricePerHour", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        //[RuleRange("", DefaultContexts.Save, 1, 999999, TargetCriteria = "IsDry=0")]
        [DefaultValue(0)]
        public Double BasicSumAssured

        {
            get { return fBasicSumAssured; }
            set { SetPropertyValue<Double>("BasicSumAssured", ref fBasicSumAssured, value); }
        }

               
        double fTerm;
        //[RuleRequiredField("", DefaultContexts.Save, SkipNullOrEmptyValues = false)]
        public double Term
        {
            get { return fTerm; }
            set { SetPropertyValue<double>("PolicyTTerm", ref fTerm, value); }
        }
        //[XafDisplayName("PolicyTerm"), ToolTip("Please enter Policy Term")]
        //[RuleRequiredField]
        //public Double PolicyTerm
        //{
        //    get { return policyTerm; }
        //    set { SetPropertyValue<Double>("PolicyTerm", ref policyTerm, value); }
        //}

        string premiumPayingTerm;
        [Size(100)]
        [XafDisplayName("PremiumPayingTerm"), ToolTip("Please enter Premium PayingTerm")]
        [RuleRequiredField]
        public string PremiumPayingTerm
        {
            get { return premiumPayingTerm; }
            set { SetPropertyValue<string>("PremiumPayingTerm", ref premiumPayingTerm, value); }
        }

        bool fUnderNACH;
       // [Browsable(false)]
        [DefaultValue(false)]
        public bool UnderNACH
        {
            get { return fUnderNACH; }
            set { SetPropertyValue<bool>("UnderNACH", ref fUnderNACH, value); }
        }

        string cKYCNumber;
        [Size(100)]
        [XafDisplayName("CKYCNumber"), ToolTip("Please enter CKYC Number")]
        //[RuleRequiredField]
        public string CKYCNumber
        {
            get { return cKYCNumber; }
            set { SetPropertyValue<string>("CKYC Number", ref cKYCNumber, value); }
        }


        bool fITAssesseeorNot;
        //[Browsable(false)]
        [DefaultValue(false)]
        public bool ITAssesseeorNot
        {
            get { return fITAssesseeorNot; }
            set { SetPropertyValue<bool>("ITAssesseeorNot", ref fITAssesseeorNot, value); }
        }

        bool fRegisterUnderGSTAct;
        //[Browsable(false)]
        [DefaultValue(false)]
        public bool RegisterUnderGSTAct
        {
            get { return fRegisterUnderGSTAct; }
            set { SetPropertyValue<bool>("RegisterUnderGSTAct", ref fRegisterUnderGSTAct, value); }
        }

        string gSTINNo;
        [Size(100)]
        [XafDisplayName("GSTINNo"), ToolTip("Please enter GSTINNo")]
        //[RuleRequiredField]
        public string GSTINNo
        {
            get { return gSTINNo; }
            set { SetPropertyValue<string>("GSTINNo", ref gSTINNo, value); }
        }

        Double fYearlyIncome;

        //[Appearance("PickedPricePerHourCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        //[Appearance("IsDryPricePerHour", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        //[RuleRange("", DefaultContexts.Save, 1, 999999, TargetCriteria = "IsDry=0")]
        [DefaultValue(0)]
        public Double YearlyIncome

        {
            get { return fYearlyIncome; }
            set { SetPropertyValue<Double>("YearlyIncome", ref fYearlyIncome, value); }
        }

        string presentOccupation;
        [Size(200)]
        [XafDisplayName("Present Occupation"), ToolTip("Please enter PresentOccupation")]
        //[RuleRequiredField]
        public string PresentOccupation
        {
            get { return presentOccupation; }
            set { SetPropertyValue<string>("Present Occupation", ref presentOccupation, value); }
        }

        string companyName;
        [Size(250)]
        [XafDisplayName("CompanyName"), ToolTip("Please enter CompanyName")]
        //[RuleRequiredField]
        public string CompanyName
        {
            get { return companyName; }
            set { SetPropertyValue<string>("CompanyName", ref companyName, value); }
        }

        string exactNatureOfDuty;
        [Size(100)]
        [XafDisplayName("ExactNatureOfDuty"), ToolTip("Please enter Exact Nature Of Duty")]
        //[RuleRequiredField]
        public string ExactNatureOfDuty
        {
            get { return exactNatureOfDuty; }
            set { SetPropertyValue<string>("ExactNatureOfDuty", ref exactNatureOfDuty, value); }
        }

        string sourceOfIncome;
        [Size(100)]
        [XafDisplayName("SourceOfIncome"), ToolTip("Please enter Source Of Income")]
        //[RuleRequiredField]
        public string SourceOfIncome
        {
            get { return sourceOfIncome; }
            set { SetPropertyValue<string>("SourceOfIncome", ref sourceOfIncome, value); }
        }

        string educationalQualification;
        [Size(100)]
        [XafDisplayName("EducationalQualification"), ToolTip("Please enter EducationalQualification")]
        //[RuleRequiredField]
        public string EducationalQualification
        {
            get { return educationalQualification; }
            set { SetPropertyValue<string>("EducationalQualification", ref educationalQualification, value); }
        }

        //Length Of service

        Double fLastYearIncome;
        [DefaultValue(0)]
        public Double LastYearIncome

        {
            get { return fLastYearIncome; }
            set { SetPropertyValue<Double>("LastYearIncome", ref fLastYearIncome, value); }
        }

        Double f2ndLastYearIncome;
        [DefaultValue(0)]
        public Double Last2ndYearIncome

        {
            get { return f2ndLastYearIncome; }
            set { SetPropertyValue<Double>("LastYearIncome", ref f2ndLastYearIncome, value); }
        }
        string prevPolicyDetails1;
        [Size(200)]
        [XafDisplayName("PrevPolicyDetails1"), ToolTip("Please enter PrevPolicyDetails1")]
        //[RuleRequiredField]
        public string PrevPolicyDetails1
        {
            get { return prevPolicyDetails1; }
            set { SetPropertyValue<string>("PrevPolicyDetails1", ref prevPolicyDetails1, value); }
        }

        string prevPolicyDetails2;
        [Size(200)]
        [XafDisplayName("PrevPolicyDetails2"), ToolTip("Please enter PrevPolicyDetails2")]
        //[RuleRequiredField]
        public string PrevPolicyDetails2
        {
            get { return prevPolicyDetails2; }
            set { SetPropertyValue<string>("PrevPolicyDetails2", ref prevPolicyDetails2, value); }
        }

        string prevPolicyDetails3;
        [Size(200)]
        [XafDisplayName("PrevPolicyDetails3"), ToolTip("Please enter PrevPolicyDetails3")]
        //[RuleRequiredField]
        public string PrevPolicyDetails3
        {
            get { return prevPolicyDetails3; }
            set { SetPropertyValue<string>("PrevPolicyDetails3", ref prevPolicyDetails3, value); }
        }

        string prevPolicyDetails4;
        [Size(200)]
        [XafDisplayName("PrevPolicyDetails4"), ToolTip("Please enter PrevPolicyDetails4")]
        //[RuleRequiredField]
        public string PrevPolicyDetails4
        {
            get { return prevPolicyDetails4; }
            set { SetPropertyValue<string>("PrevPolicyDetails4", ref prevPolicyDetails4, value); }
        }

        string prevPolicyDetails5;
        [Size(200)]
        [XafDisplayName("PrevPolicyDetails5"), ToolTip("Please enter PrevPolicyDetails5")]
        //[RuleRequiredField]
        public string PrevPolicyDetails5
        {
            get { return prevPolicyDetails5; }
            set { SetPropertyValue<string>("PrevPolicyDetails5", ref prevPolicyDetails5, value); }
        }

        string prevPolicyDetails6;
        [Size(200)]
        [XafDisplayName("PrevPolicyDetails6"), ToolTip("Please enter PrevPolicyDetails6")]
        //[RuleRequiredField]
        public string PrevPolicyDetails6
        {
            get { return prevPolicyDetails6; }
            set { SetPropertyValue<string>("PrevPolicyDetails6", ref prevPolicyDetails6, value); }
        }

        string prevPolicyDetails7;
        [Size(200)]
        [XafDisplayName("PrevPolicyDetails7"), ToolTip("Please enter PrevPolicyDetails7")]
        //[RuleRequiredField]
        public string PrevPolicyDetails7
        {
            get { return prevPolicyDetails7; }
            set { SetPropertyValue<string>("PrevPolicyDetails7", ref prevPolicyDetails7, value); }
        }

        string prevPolicyDetails8;
        [Size(200)]
        [XafDisplayName("PrevPolicyDetails8"), ToolTip("Please enter PrevPolicyDetails8")]
        //[RuleRequiredField]
        public string PrevPolicyDetails8
        {
            get { return prevPolicyDetails8; }
            set { SetPropertyValue<string>("PrevPolicyDetails8", ref prevPolicyDetails8, value); }
        }

        string prevPolicyDetails9;
        [Size(200)]
        [XafDisplayName("PrevPolicyDetails9"), ToolTip("Please enter PrevPolicyDetails9")]
        //[RuleRequiredField]
        public string PrevPolicyDetails9
        {
            get { return prevPolicyDetails9; }
            set { SetPropertyValue<string>("PrevPolicyDetails9", ref prevPolicyDetails9, value); }
        }

        string prevPolicyDetails10;
        [Size(200)]
        [XafDisplayName("PrevPolicyDetails10"), ToolTip("Please enter PrevPolicyDetails10")]
        //[RuleRequiredField]
        public string PrevPolicyDetails10
        {
            get { return prevPolicyDetails10; }
            set { SetPropertyValue<string>("PrevPolicyDetails10", ref prevPolicyDetails10, value); }
        }


        MaritalStatusList fMaritalStatus;
        //[RuleRequiredField]
        public MaritalStatusList MaritalStatu
        {
            get { return fMaritalStatus; }
            set { SetPropertyValue<MaritalStatusList>("MaritalStatus", ref fMaritalStatus, value); }
        }


        String fFathersAge;

        public String FathersAge

        {
            get { return fFathersAge; }
            set { SetPropertyValue<String>("FathersAge", ref fFathersAge, value); }
        }


        String fMothersAge;

        public String MothersAge

        {
            get { return fMothersAge; }
            set { SetPropertyValue<String>("MothersAge", ref fMothersAge, value); }
        }

        String fAllBrothersAge;
       
        public String AllBrothersAge

        {
            get { return fAllBrothersAge; }
            set { SetPropertyValue<String>("AllBrothersAge", ref fAllBrothersAge, value); }
        }

        String fAllSistersAge;
       
        public String AllSistersAge

        {
            get { return fAllSistersAge; }
            set { SetPropertyValue<String>("AllSistersAge", ref fAllSistersAge, value); }
        }

        String fSpouseAge;
        
        public String SpouseAge

        {
            get { return fSpouseAge; }
            set { SetPropertyValue<String>("SpouseAge", ref fSpouseAge, value); }
        }

        String fAllDaughtersAge;
        
        public String AllDaughtersAge

        {
            get { return fAllDaughtersAge; }
            set { SetPropertyValue<String>("AllSisterssAge", ref fAllDaughtersAge, value); }
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

        string fNominee;
        [Size(100)]
        public string Nominee
        {
            get { return fNominee; }
            set { SetPropertyValue<string>("Nominee", ref fNominee, value); }
        }

        //String fNomineeAge;

        //public String NomineeAge

        //{
        //    get { return fNomineeAge; }
        //    set { SetPropertyValue<String>("NomineeAge", ref fNomineeAge, value); }
        //}

        DateTime fNomineeDateOfBirth;
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        //[RuleRequiredField]
        public DateTime NomineeDateOfBirth
        {
            get { return fNomineeDateOfBirth; }
            set { SetPropertyValue<DateTime>("Nominee Date Of Birth", ref fNomineeDateOfBirth, value); }
        }

        string fNomineeRelation;
        [Size(100)]
        public string NomineeRelation
        {
            get { return fNomineeRelation; }
            set { SetPropertyValue<string>("Nominee Relation", ref fNomineeRelation, value); }
        }

        string fBankDetails;
        [Size(100)]
        public string BankDetails
        {
            get { return fBankDetails; }
            set { SetPropertyValue<string>("Bank Details", ref fBankDetails, value); }
        }
        /// <End>

        private LICApplicationStatus status;
        public virtual LICApplicationStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                //if (isLoaded)
                //{
                    if (value == LICApplicationStatus.Closed)
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
        public XPCollection<LICApplicationFileData> Attachments
        {
            get { return GetCollection<LICApplicationFileData>(nameof(Attachments)); }
        }     
               

        Company fCompany;
        //[Browsable(false)]
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "LICApplicationID>0", Context = "Any")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }



        //[Association("MyLICApplication-LICApplicationNotification")]
        //public XPCollection<LICApplicationNotification> MyNotifications
        //{
        //    get
        //    {
        //        return GetCollection<LICApplicationNotification>(nameof(MyNotifications));
        //    }
        //}

        [Action(ImageName = "State_LicApplication_Completed")]
        //[Appearance("REmoveHoldCond", Enabled = false, Criteria = "Status != 0", Context = "Any")]
        public void RemoveHold()
        {
            Status = LICApplicationStatus.PendingApproval;
        }
        
        [Action(ImageName = "State_LICApplication_Approved")]
        public void Approved()
        {
            Status = LICApplicationStatus.Approved;
        }
        [Action(ImageName = "State_LICApplication_Submitted")]
        public void Submitted()
        {
            Status = LICApplicationStatus.Submitted;
        }
        [Action(ImageName = "State_LICApplication_Inprocess")]
        public void Inprocess()
        {
            Status = LICApplicationStatus.Inprocess;
        }
        [Action(ImageName = "State_LICApplication_Rejected")]
        public void Rejected()
        {
            Status = LICApplicationStatus.Rejected;
        }
        [Action(ImageName = "State_LICApplication_Closed")]
        public void Closed()
        {
            Status = LICApplicationStatus.Closed;
        }

        [Action(ImageName = "State_LICApplication_Closed")]
        public void ConvertToPolicy()
        {
            Status = LICApplicationStatus.ConvertToPolicy;
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

    public enum LICApplicationStatus
    {
        [ImageName("State_LICApplication_NotStarted")]
        Hold,
        [ImageName("State_LICApplication_PendingApproval")]
        PendingApproval,
        [ImageName("State_LICApplication_Approved")]
        Approved,
        [ImageName("State_LICApplication_Submitted")]
        Submitted,
        [ImageName("State_LICApplication_Rejected")]
        Rejected,
        [ImageName("State_LICApplication_Inprocess")]
        Inprocess,
        [ImageName("State_LICApplication_Closed")]
        Closed,
        ConvertToPolicy
    }

    public enum MaritalStatusList
    {
        Married = 1,
        UnMarried = 2,
        Others = 3,
    }
}