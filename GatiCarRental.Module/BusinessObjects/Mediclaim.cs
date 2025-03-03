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
    public class Mediclaim : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Mediclaim(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _MediclaimID = (Session.Evaluate<Mediclaim>(CriteriaOperator.Parse("Max(MediclaimID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Mediclaim>(CriteriaOperator.Parse("Max(MediclaimID)"), CriteriaOperator.Parse("")))) + 1;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            fCreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            fCreatedOn = DateTime.Now;
            fCoverType = CoverType.INDIVIDUAL;
            fBenefits = Benefits.ANNUAL_HEALTH_CHECK_UP;
            //CallResponse = CallResponseType.NotPickup;
            //DocumentNo = "TDS-" + (_TDSID).ToString();
            GetDocumentNumbering();
            DocumentDate = DateTime.Today;
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }


        private void GetDocumentNumbering()
        {

            XPCollection<DocumentNumbering> docnos = new XPCollection<DocumentNumbering>(Session);
            docnos.Criteria = CriteriaOperator.Parse("[DocumentType]=?", DocumentType.Mediclaim);
            if (docnos.Count == 1)
            {
                foreach (DocumentNumbering docno in docnos)
                {
                    DocumentNo = docno.Prefix + new String('X', docno.Body) + docno.Suffix;
                    DocSchemeOid = docno.Oid;
                    IsNew = true;
                }
            }

            Department = Department.Mediclaim;
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


        [Persistent("MediclaimID")] // this line for read-only columns mapping
        private int _MediclaimID;
        [RuleUniqueValue] // Validation for unique value
        //RuleRequiredField("RuleRequiredField for CallLogLead.CallLogLeadID", DefaultContexts.Save, "Call Log Lead ID must be specified")] // Validation for Required
        [Browsable(false)]
        [PersistentAlias("_MediclaimID")] // This line for read-only column mapping
        public int MediclaimID
        {
            get { return _MediclaimID; }
        }

        string fDocumentNo;
        [Appearance("MediclaimDocNoCond", Enabled = false, Criteria = "IsNew=1", Context = "Any")]
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
        //[RuleRequiredField("RuleRequiredField for Mediclaim.AssignTo", DefaultContexts.Save,
        //"Agents must be specified")]
        //[Appearance("AgentPolicyCond", Enabled = false, Criteria = "@this.Agents.TypeOfEmployee='Agent'", Context = "DetailView")]
        [Association("Mediclaim-Employee")]
        public Employee AssignTo
        {
            get { return fAssignTo; }
            set
            {
                SetPropertyValue(nameof(AssignTo), ref fAssignTo, value);


            }
        }

        private Customer fCustomer;
        [Association("Mediclaim-Customer")]
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




       

        //double fTerm;
        ////[RuleRequiredField("", DefaultContexts.Save, SkipNullOrEmptyValues = false)]
        //public double Term
        //{
        //    get { return fTerm; }
        //    set { SetPropertyValue<double>("PolicyTTerm", ref fTerm, value); }
        //}
        //[XafDisplayName("PolicyTerm"), ToolTip("Please enter Policy Term")]
        //[RuleRequiredField]
        //public Double PolicyTerm
        //{
        //    get { return policyTerm; }
        //    set { SetPropertyValue<Double>("PolicyTerm", ref policyTerm, value); }
        //}

        //string premiumPayingTerm;
        //[Size(100)]
        //[XafDisplayName("PremiumPayingTerm"), ToolTip("Please enter Premium PayingTerm")]
        //[RuleRequiredField]
        //public string PremiumPayingTerm
        //{
        //    get { return premiumPayingTerm; }
        //    set { SetPropertyValue<string>("PremiumPayingTerm", ref premiumPayingTerm, value); }
        //}        

        string fPhone;
        [Size(20)]
        [RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        public string Phone
        {
            get { return fPhone; }
            set { SetPropertyValue<string>("Phone", ref fPhone, value); }
        }
        string fEmail;
        [RuleUniqueValue]
        [Size(200)]
        public string Email
        {
            get { return fEmail; }
            set { SetPropertyValue<string>("Email", ref fEmail, value); }
        }
        string fPinCode;
        [Size(10)]
        [RuleRequiredField] // Validation for Required
        public string PinCode
        {
            get { return fPinCode; }
            set { SetPropertyValue<string>("PinCode", ref fPinCode, value); }
        }
       
        CoverType fCoverType;
        //[RuleRequiredField]
        public CoverType CoverTypes
        {
            get { return fCoverType; }
            set { SetPropertyValue<CoverType>("CoverType", ref fCoverType, value); }
        }

        int fTotalMembers;
        //[RuleRequiredField("", DefaultContexts.Save, SkipNullOrEmptyValues = false)]
        public int TotalMembers
        {
            get { return fTotalMembers; }
            set { SetPropertyValue<int>("Total Members", ref fTotalMembers, value); }
        }

        String fMembersAge;

        public String MembersAge

        {
            get { return fMembersAge; }
            set { SetPropertyValue<String>("Members Age", ref fMembersAge, value); }
        }

        Double fSumInsured;

        //[Appearance("PickedPricePerHourCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        //[Appearance("IsDryPricePerHour", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        //[RuleRange("", DefaultContexts.Save, 1, 999999, TargetCriteria = "IsDry=0")]
        [DefaultValue(0)]
        public Double SumInsured

        {
            get { return fSumInsured; }
            set { SetPropertyValue<Double>("Sum Insured", ref fSumInsured, value); }
        }

        int fTenure;
        //[RuleRequiredField("", DefaultContexts.Save, SkipNullOrEmptyValues = false)]
        public int Tenure
        {
            get { return fTenure; }
            set { SetPropertyValue<int>("Tenure", ref fTenure, value); }
        }

        Benefits fBenefits;
        //[RuleRequiredField]
        public Benefits Benefits
        {
            get { return fBenefits; }
            set { SetPropertyValue<Benefits>("Benefits", ref fBenefits, value); }
        }

        string fPanNo;
        [RuleUniqueValue]
        [Size(200)]
        public string PanNo
        {
            get { return fPanNo; }
            set { SetPropertyValue<string>("Pan Card No", ref fPanNo, value); }
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

        DateTime fGSTRegistrationDate ;
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        //[RuleRequiredField]
        public DateTime GSTRegistrationDate
        {
            get { return fGSTRegistrationDate; }
            set { SetPropertyValue<DateTime>("GST Registration Date", ref fGSTRegistrationDate, value); }
        }

        string fFullAddress;
        [Size(500)]
        //[RuleRequiredField] // Validation for Required
        public string FullAddress
        {
            get { return fFullAddress; }
            set { SetPropertyValue<string>("Full Address", ref fFullAddress, value); }
        }

        string fNominee;
        [Size(100)]
        public string Nominee
        {
            get { return fNominee; }
            set { SetPropertyValue<string>("Nominee Name", ref fNominee, value); }
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

        bool fHasAnyPED;
        //[Browsable(false)]
        [DefaultValue(false)]
        public bool HasAnyPED
        {
            get { return fHasAnyPED; }
            set { SetPropertyValue<bool>("PED?", ref fHasAnyPED, value); }
        }

        bool fClaimCurrentorprevInsurer;
        //[Browsable(false)]
        [DefaultValue(false)]
        public bool ClaimCurrentorprevInsurer
        {
            get { return fClaimCurrentorprevInsurer; }
            set { SetPropertyValue<bool>("HAS ANY OF THE NEW PERSON TO BE INSURED EVER FILLED A CLAIM WITH THEIR CURRENT/PREVIOUS INSURER?", ref fClaimCurrentorprevInsurer, value); }
        }


        bool fDeclinedCancelledOrChargedHigherPremium;
        //[Browsable(false)]
        [DefaultValue(false)]
        public bool DeclinedCancelledOrChargedHigherPremium
        {
            get { return fDeclinedCancelledOrChargedHigherPremium; }
            set { SetPropertyValue<bool>(" DECLINED CANCELLED OR CHARGED A HIGHER PREMIUM?", ref fDeclinedCancelledOrChargedHigherPremium, value); }
        }


        bool fOtherPolicy;
        ////[Browsable(false)]
        [DefaultValue(false)]
        public bool OtherPolicy
        {
            get { return fOtherPolicy; }
            set { SetPropertyValue<bool>("ANY OTHER HEALTH INSURANCE POLICY WITH THE COMPANY", ref fOtherPolicy, value); }
        }

        bool fWishToAddCrCard;
        //[Browsable(false)]
        [DefaultValue(false)]
        public bool WishToAddCrCard
        {
            get { return fWishToAddCrCard; }
            set { SetPropertyValue<bool>("WISH TO ADD CREDIT CARD FOR FUTURE RENEWAL PREMIUM", ref fWishToAddCrCard, value); }
        }

        PaymentFrequency fPaymentMode;
        public PaymentFrequency PaymentMode
        {
            get { return fPaymentMode; }
            set { SetPropertyValue<PaymentFrequency>("PaymentMode", ref fPaymentMode, value); }
        }

        private MediclaimStatus status;
        public virtual MediclaimStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                //if (isLoaded)
                //{
                if (value == MediclaimStatus.Closed)
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
        public XPCollection<MediclaimFileData> Attachments
        {
            get { return GetCollection<MediclaimFileData>(nameof(Attachments)); }
        }


        Company fCompany;
        //[Browsable(false)]
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "MediclaimID>0", Context = "Any")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }



        //[Association("MyMediclaim-MediclaimNotification")]
        //public XPCollection<MediclaimNotification> MyNotifications
        //{
        //    get
        //    {
        //        return GetCollection<MediclaimNotification>(nameof(MyNotifications));
        //    }
        //}

        [Action(ImageName = "State_Mediclaim_Completed")]
        //[Appearance("REmoveHoldCond", Enabled = false, Criteria = "Status != 0", Context = "Any")]
        public void RemoveHold()
        {
            Status = MediclaimStatus.PendingApproval;
        }

        [Action(ImageName = "State_Mediclaim_Approved")]
        public void Approved()
        {
            Status = MediclaimStatus.Approved;
        }
        [Action(ImageName = "State_Mediclaim_Submitted")]
        public void Submitted()
        {
            Status = MediclaimStatus.Submitted;
        }
        [Action(ImageName = "State_Mediclaim_Inprocess")]
        public void Inprocess()
        {
            Status = MediclaimStatus.Inprocess;
        }
        [Action(ImageName = "State_Mediclaim_Rejected")]
        public void Rejected()
        {
            Status = MediclaimStatus.Rejected;
        }
        [Action(ImageName = "State_Mediclaim_Closed")]
        public void Closed()
        {
            Status = MediclaimStatus.Closed;
        }

        [Action(ImageName = "State_Mediclaim_Closed")]
        public void ConvertToPolicy()
        {
            Status = MediclaimStatus.ConvertToPolicy;
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

    public enum MediclaimStatus
    {
        [ImageName("State_Mediclaim_NotStarted")]
        Hold,
        [ImageName("State_Mediclaim_PendingApproval")]
        PendingApproval,
        [ImageName("State_Mediclaim_Approved")]
        Approved,
        [ImageName("State_Mediclaim_Submitted")]
        Submitted,
        [ImageName("State_Mediclaim_Rejected")]
        Rejected,
        [ImageName("State_Mediclaim_Inprocess")]
        Inprocess,
        [ImageName("State_Mediclaim_Closed")]
        Closed,
        ConvertToPolicy
    }

    public enum CoverType
    {
        INDIVIDUAL = 1,
        FLOATER = 2,
        Others = 3,
    }

    public enum Benefits
    {
        CUMULATIVE_BONUS_SUPER = 1,
        WELLNESS_BENEFIT = 2,
        AIR_AMBULANCE_COVER = 3,
        INSTANT_COVER = 4,
        PED_WAIT_PERIOD_MODIFICATION = 5,
        ANNUAL_HEALTH_CHECK_UP = 6,
        BEFIT_BENEFIT = 7,
        CLAIM_SHIELD = 8,
        CARE_OPD = 9,
        Other = 9
    }
}