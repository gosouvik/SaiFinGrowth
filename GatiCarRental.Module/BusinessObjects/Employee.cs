using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using static GatiCarRental.Module.BusinessObjects.Customer;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions, DefaultProperty(nameof(firstName))]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Employee : PermissionPolicyUser
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Employee(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            Gender = GenderList.Male;
            TypeOfEmployee = EmployeeType.Associates;
            //PermissionPolicyUser puser;
            //puser = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            //Guid employeeid = puser.Oid;
            
            //XPCollection<Employee> employees = new XPCollection<Employee>(Session, new BinaryOperator("Oid", employeeid));
            //foreach (Employee employee in employees)
            //    fCreatedBy = employee;
            fCreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            fCreatedOn = DateTime.Now;
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private Company fCompany;
        [Association("Company-Employees")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue(nameof(Company), ref fCompany, value); }
        }

        string fUINCode;
        [Size(20)]
        [RuleUniqueValue]
        [XafDisplayName("UIN"), ToolTip("Unique Identification Number")]
        //[RuleRequiredField]
        public string UINCode
        {
            get { return fUINCode; }
            set { SetPropertyValue<string>("UINCode", ref fUINCode, value); }
        }

        string firstName;
        [Size(100)]
        [XafDisplayName("Full Name"), ToolTip("Please enter your Full Name")]
        //[RuleRequiredField]
        public string FirstName
        {
            get { return firstName; }
            set { SetPropertyValue<string>("FirstName", ref firstName, value); }
        }

        
        EmployeeType fTypeOfEmployee;
        public EmployeeType TypeOfEmployee
        {
            get { return fTypeOfEmployee; }
            set { SetPropertyValue<EmployeeType>("TypeOfEmployee", ref fTypeOfEmployee, value); }
        }

        Department fDepartment;
        public Department Department
        {
            get { return fDepartment; }
            set { SetPropertyValue<Department>("Department", ref fDepartment, value); }
        }


        string agencyCode;
        [Size(50)]
        //[RuleRequiredField]
        [XafDisplayName("LIC Agency Code"), ToolTip("Please enter your LIC Agency Code")]
        public string AgencyCode
        {
            get { return agencyCode; }
            set { SetPropertyValue<string>("AgencyCode", ref agencyCode, value); }
        }

        string rmCode;
        [Size(50)]
        //[RuleRequiredField]
        [XafDisplayName("RM Code"), ToolTip("Please enter your RM Code")]
        public string RMCode
        {
            get { return rmCode; }
            set { SetPropertyValue<string>("RMCode", ref rmCode, value); }
        }

        string mediclaimAgencyCode;
        [Size(50)]
        //[RuleRequiredField]
        [XafDisplayName("Mediclaim Agency Code"), ToolTip("Please enter your Mediclaim Agency Code")]
        public string MediclaimAgencyCode
        {
            get { return mediclaimAgencyCode; }
            set { SetPropertyValue<string>("MediclaimAgencyCode", ref mediclaimAgencyCode, value); }
        }

        string mutualFundCode;
        [Size(50)]
        [XafDisplayName("Mutual Fund Distributor Code"), ToolTip("Please enter your Mutual Fund Distributor Code")]
        public string MutualFundCode
        {
            get { return mutualFundCode; }
            set { SetPropertyValue<string>("MutualFundCode", ref mutualFundCode, value); }
        }

        string loanCode;
        [Size(50)]
        [XafDisplayName("Loan Councillor Code"), ToolTip("Please enter your Loan Councillor Code")]
        public string LoanCode
        {
            get { return loanCode; }
            set { SetPropertyValue<string>("LoanCode", ref loanCode, value); }
        }

        DateTime fDateOfAppointment;
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        //[RuleRequiredField]
        public DateTime DateOfAppointment
        {
            get { return fDateOfAppointment; }
            set { SetPropertyValue<DateTime>("DateOfAppointment", ref fDateOfAppointment, value); }
        }

        DateTime fDateOfBirth;
        //[RuleRequiredField]
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
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

        DateTime fSpouseDateOfBirth;
        //[RuleRequiredField]
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        public DateTime SpouseDateOfBirth
        {
            get { return fSpouseDateOfBirth; }
            set { SetPropertyValue<DateTime>("SpouseDateOfBirth", ref fSpouseDateOfBirth, value); }
        }

        DateTime fAnniversaryDate;
        //[RuleRequiredField]
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        public DateTime AnniversaryDate
        {
            get { return fAnniversaryDate; }
            set { SetPropertyValue<DateTime>("AnniversaryDate", ref fAnniversaryDate, value); }
        }


        string fChildName1;
        [Size(100)]
        public string ChildName1
        {
            get { return fChildName1; }
            set { SetPropertyValue<string>("ChildName1", ref fChildName1, value); }
        }

        DateTime fChild1DateOfBirth;
        //[RuleRequiredField]
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        public DateTime Child1DateOfBirth
        {
            get { return fChild1DateOfBirth; }
            set { SetPropertyValue<DateTime>("Child1DateOfBirth", ref fChild1DateOfBirth, value); }
        }

        string fChildName2;
        [Size(100)]
        public string ChildName2
        {
            get { return fChildName2; }
            set { SetPropertyValue<string>("ChildName2", ref fChildName2, value); }
        }

        DateTime fChild2DateOfBirth;
        //[RuleRequiredField]
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        public DateTime Child2DateOfBirth
        {
            get { return fChild2DateOfBirth; }
            set { SetPropertyValue<DateTime>("Child2DateOfBirth", ref fChild2DateOfBirth, value); }
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

        string fOccupation;
        [Size(200)]
        public string Occupation
        {
            get { return fOccupation; }
            set { SetPropertyValue<string>("Occupation", ref fOccupation, value); }
        }

        string fSalesExperience;
        [Size(255)]
        //[RuleRequiredField]
        public string SalesExperience
        {
            get { return fSalesExperience; }
            set { SetPropertyValue<string>("SalesExperience", ref fSalesExperience, value); }
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

        [Association]
        //[VisibleInDetailView(false)]
        public XPCollection<CallLogEmployee> CallLogs
        {
            get { return GetCollection<CallLogEmployee>(nameof(CallLogs)); }
        }

        [Association]
        //[VisibleInDetailView(false)]
        public XPCollection<Policy> Policies
        {
            get { return GetCollection<Policy>(nameof(Policies)); }
        }

        [Association("Mediclaim-Agents")]
        //[VisibleInDetailView(false)]
        public XPCollection<MediclaimPolicy> MediclaimPolicies
        {
            get { return GetCollection<MediclaimPolicy>(nameof(MediclaimPolicies)); }
        }

        [Association("Task-Employee")]
        //[VisibleInDetailView(false)]
        public XPCollection<Task> MyTask
        {
            get { return GetCollection<Task>(nameof(MyTask)); }
        }

        [Association("LICApplication-Employee")]
        //[VisibleInDetailView(false)]
        public XPCollection<LICApplication> MyLICApplication
        {
            get { return GetCollection<LICApplication>(nameof(MyLICApplication)); }
        }

        [Association("Mediclaim-Employee")]
        //[VisibleInDetailView(false)]
        public XPCollection<Mediclaim> MyMediclaim
        {
            get { return GetCollection<Mediclaim>(nameof(MyMediclaim)); }
        }

        //[Association("Loan-Employee")]
        ////[VisibleInDetailView(false)]
        //public XPCollection<Loan> loans
        //{
        //    get { return GetCollection<Loan>(nameof(loans)); }
        //}
        [Association("Lead-Employee")]
        //[VisibleInDetailView(false)]
        public XPCollection<Lead> MyLeads
        {
            get { return GetCollection<Lead>(nameof(MyLeads)); }
        }

        [Association("Collaborators-Task")]
        [VisibleInDetailView(false)]
        public XPCollection<Task> CollaboratorTasks
        {
            get
            {
                return GetCollection<Task>(nameof(CollaboratorTasks));
            }
        }

        [Association("AssignedTo-TaskNotifications")]
        [VisibleInDetailView(false)]
        public XPCollection<TaskNotification> TaskNotifications
        {
            get
            {
                return GetCollection<TaskNotification>(nameof(TaskNotifications));
            }
        }

        [DevExpress.Xpo.Aggregated, Association]
        public XPCollection<EmployeeFileData> Attachments
        {
            get { return GetCollection<EmployeeFileData>(nameof(Attachments)); }
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



    }

    public enum EmployeeType
    {
        Employee = 5,
        Associates = 10,
        Partner = 15,
        ChannelPartner=20,
        Distributor=25,
        Others=30
    }

    public enum Department
    {
        LICI = 0,
        GIC = 5,
        Mediclaim = 10,
        Loan = 15,
        MutualFund = 20,
        Others =30
    }
}