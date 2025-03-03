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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class CallLogEmployee : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public CallLogEmployee(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _CallLogEmployeeID = (Session.Evaluate<CallLogEmployee>(CriteriaOperator.Parse("Max(CallLogEmployeeID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<CallLogEmployee>(CriteriaOperator.Parse("Max(CallLogEmployeeID)"), CriteriaOperator.Parse("")))) + 1;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            fCreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            fCreatedOn = DateTime.Now;
            //CallResponse = CallResponseType.NotInterested;
            //DocumentNo = "TDS-" + (_TDSID).ToString();
            GetDocumentNumbering();
            DocumentDate = DateTime.Today;
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
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


        private void GetDocumentNumbering()
        {

            XPCollection<DocumentNumbering> docnos = new XPCollection<DocumentNumbering>(Session);
            docnos.Criteria = CriteriaOperator.Parse("[DocumentType]=?", DocumentType.CallLogEmployee);
            if (docnos.Count == 1)
            {
                foreach (DocumentNumbering docno in docnos)
                {
                    DocumentNo = docno.Prefix + new String('X', docno.Body) + docno.Suffix;
                    DocSchemeOid = docno.Oid;
                    IsNew = true;
                }
            }

            fCallStatus = CallStatusType.NotAnswered;

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


        [Persistent("CallLogEmployeeID")] // this line for read-only columns mapping
        private int _CallLogEmployeeID;
        [RuleUniqueValue] // Validation for unique value
                          //[RuleRequiredField("RuleRequiredField for CallLogLead.CallLogEmployeeID", DefaultContexts.Save,
                          //"Call Log Lead ID must be specified")] // Validation for Required
        [Browsable(false)]
        [PersistentAlias("_CallLogEmployeeID")] // This line for read-only column mapping
        public int CallLogEmployeeID
        {
            get { return _CallLogEmployeeID; }
        }

        string fDocumentNo;
        [Appearance("CallLogEmpDocNoCond", Enabled = false, Criteria = "IsNew=1", Context = "Any")]
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

        private Employee fEmployee;
        [RuleRequiredField("RuleRequiredField for CallLogEmployee.Employees", DefaultContexts.Save,
        "Lead must be specified")]
        [Association]
        public Employee Employees
        {
            get { return fEmployee; }
            set
            {
                SetPropertyValue(nameof(Employees), ref fEmployee, value);
            }
        }

        private Category fCategory;
        [RuleRequiredField("RuleRequiredField for CallLogEmployee.Category", DefaultContexts.Save,
        "Category must be specified")]
        [ImmediatePostData]
        [Association]
        public Category Category
        {
            get { return fCategory; }
            set
            {
                SetPropertyValue(nameof(Category), ref fCategory, value);
                RefreshAvailableSubCategory();
            }
        }

        private XPCollection<SubCategory> fAvailableSubCategories;
        [Browsable(false)] // Prohibits showing the AvailableAccessories collection separately
        public XPCollection<SubCategory> AvailableSubCategories
        {
            get
            {
                if (fAvailableSubCategories == null)
                {
                    // Retrieve all Sample objects
                    fAvailableSubCategories = new XPCollection<SubCategory>(Session);
                    // Filter the retrieved collection according to current conditions
                    RefreshAvailableSubCategory();
                }
                // Return the filtered collection of Sample objects
                return fAvailableSubCategories;
            }
        }

        private void RefreshAvailableSubCategory()
        {
            if (fAvailableSubCategories == null)
                return;
            // Process the situation when the Party is not specified (see the Scenario 3 above)
            if (Category == null)
            {
                // Show only Global Collection when the Party is not specified
                fAvailableSubCategories.Criteria = CriteriaOperator.Parse("1=0");
            }
            else
            {
                // Leave only the current Party's Collection in the fAvailableSampleCollection collection
                fAvailableSubCategories.Criteria = new BinaryOperator("Category", Category);

                //    // Add Global Collection
                //    XPCollection<SampleCollection> availableGlobalAccessories =
                //       new XPCollection<SampleCollection>(Session);
                //    availableGlobalAccessories.Criteria = CriteriaOperator.Parse("Party",Party);
                //fAvailableSampleCollection.AddRange(availableGlobalAccessories);
            }
            // Set null for the Collection property to allow an end-user 
            //to set a new value from the refreshed data source
            SubCategory = null;
        }

        [DataSourceProperty(nameof(AvailableSubCategories))]
        [Association]
        [ImmediatePostData]
        //[Appearance("PickedCarNumberCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public SubCategory SubCategory
        {
            get { return fSubCategory; }
            set
            {
                SetPropertyValue(nameof(SubCategory), ref fSubCategory, value);
                RefreshAvailableResponses();
            }
        }
        SubCategory fSubCategory;

        private XPCollection<Response> fAvailableResponses;
        [Browsable(false)] // Prohibits showing the AvailableAccessories collection separately
        public XPCollection<Response> AvailableResponses
        {
            get
            {
                if (fAvailableResponses == null)
                {
                    // Retrieve all Sample objects
                    fAvailableResponses = new XPCollection<Response>(Session);
                    // Filter the retrieved collection according to current conditions
                    RefreshAvailableResponses();
                }
                // Return the filtered collection of Sample objects
                return fAvailableResponses;
            }
        }

        private void RefreshAvailableResponses()
        {
            if (fAvailableResponses == null)
                return;
            // Process the situation when the Party is not specified (see the Scenario 3 above)
            if (Category == null)
            {
                // Show only Global Collection when the Party is not specified
                fAvailableResponses.Criteria = CriteriaOperator.Parse("1=0");
            }
            else
            {
                // Leave only the current Party's Collection in the fAvailableSampleCollection collection
                fAvailableResponses.Criteria = CriteriaOperator.Parse("CallLogEmployee=1 and SubCategory=?", SubCategory);

                //    // Add Global Collection
                //    XPCollection<SampleCollection> availableGlobalAccessories =
                //       new XPCollection<SampleCollection>(Session);
                //    availableGlobalAccessories.Criteria = CriteriaOperator.Parse("Party",Party);
                //fAvailableSampleCollection.AddRange(availableGlobalAccessories);
            }
            // Set null for the Collection property to allow an end-user 
            //to set a new value from the refreshed data source
            Responses = null;
        }


        [DataSourceProperty(nameof(AvailableResponses))]
        [Appearance("CallLogEmployeeResponseCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Enabled = false, Criteria = "CallStatus != 10", Context = "DetailView")]
        [Association]
        //[Appearance("PickedCarNumberCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public Response Responses
        {
            get { return fResponses; }
            set
            {
                SetPropertyValue(nameof(Responses), ref fResponses, value);
            }
        }
        Response fResponses;

        CallStatusType fCallStatus;
        [ImmediatePostData]
        public CallStatusType CallStatus
        {
            get { return fCallStatus; }
            set
            {
                SetPropertyValue<CallStatusType>("CallStatus", ref fCallStatus, value);
                if (value != CallStatusType.Answered)
                {
                    //Response = CallResponseType.NotInterested;
                    //CallDuration = 0;
                }
            }
        }

        //Decimal fCallDuration;
        //[Appearance("CallLogleadCallDurationCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Enabled = false, Criteria = "CallStatus != 10", Context = "DetailView")]
        //public Decimal CallDuration
        //{
        //    get { return fCallDuration; }
        //    set { SetPropertyValue<Decimal>("CallDuration", ref fCallDuration, value); }
        //}

        //CallResponseType fCallResponse;
        //[ImmediatePostData]
        //[Appearance("CallLogleadCallResponseCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Enabled = false, Criteria = "CallStatus != 10", Context = "DetailView")]

        //public CallResponseType CallResponse
        //{
        //    get { return fCallResponse; }
        //    set { SetPropertyValue<CallResponseType>("CallResponse", ref fCallResponse, value); }
        //}

        DateTime fFollowUpDateTime;
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy hh:mm tt}")]
        [ModelDefault("EditMask", "dd/MM/yyyy hh:mm tt")]
        [Appearance("CallLogEmployeeFollowupCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Enabled = false, Criteria = "CallStatus != 10", Context = "DetailView")]
        //[Appearance("CallLogleadFollowUpDateTimeCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Enabled = false, Criteria = "Response == 'Follow Up'", Context = "DetailView")]
        public DateTime FollowUpDateTime
        {
            get { return fFollowUpDateTime; }
            set { SetPropertyValue<DateTime>("FollowUpDateTime", ref fFollowUpDateTime, value); }
        }

        string fRemarks;
        [Size(1000)]
        public string Remarks
        {
            get { return fRemarks; }
            set { SetPropertyValue<string>("Remarks", ref fRemarks, value); }
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
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "CallLogEmployeeID>0", Context = "Any")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        [DevExpress.Xpo.Aggregated, Association("Task-CallLogEmloyees")]
        //[VisibleInDetailView(false)]
        public XPCollection<Task> Tasks
        {
            get { return GetCollection<Task>(nameof(Tasks)); }
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

        public enum CallStatusType
        {
            NotAnswered = 0,
            Answered = 10,
            SwitchOff = 15,
            Busy = 20,
            CallWaiting = 25,
            CallCancelled = 30,
            CallFailed = 33,
            CallDiverted = 35
            //NotPickup = 15

        }

        public enum CallResponseType
        {
            FollowUp = 5,
            NotInterested = 10
            //NotPickup = 15

        }
    }
}