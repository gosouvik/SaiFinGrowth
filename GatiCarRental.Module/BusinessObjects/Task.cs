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
    public class Task : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Task(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _TaskID = (Session.Evaluate<Task>(CriteriaOperator.Parse("Max(TaskID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Task>(CriteriaOperator.Parse("Max(TaskID)"), CriteriaOperator.Parse("")))) + 1;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            fCreatedBy = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            fCreatedOn = DateTime.Now;
            //CallResponse = CallResponseType.NotPickup;
            //DocumentNo = "TDS-" + (_TDSID).ToString();
            GetDocumentNumbering();
            DocumentDate = DateTime.Today;
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }


        private void GetDocumentNumbering()
        {

            XPCollection<DocumentNumbering> docnos = new XPCollection<DocumentNumbering>(Session);
            docnos.Criteria = CriteriaOperator.Parse("[DocumentType]=?", DocumentType.Task);
            if (docnos.Count == 1)
            {
                foreach (DocumentNumbering docno in docnos)
                {
                    DocumentNo = docno.Prefix + new String('X', docno.Body) + docno.Suffix;
                    DocSchemeOid = docno.Oid;
                    IsNew = true;
                }
            }


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


        [Persistent("TaskID")] // this line for read-only columns mapping
        private int _TaskID;
        [RuleUniqueValue] // Validation for unique value
        //RuleRequiredField("RuleRequiredField for CallLogLead.CallLogLeadID", DefaultContexts.Save, "Call Log Lead ID must be specified")] // Validation for Required
        [Browsable(false)]
        [PersistentAlias("_TaskID")] // This line for read-only column mapping
        public int TaskID
        {
            get { return _TaskID; }
        }

        string fDocumentNo;
        [Appearance("TaskDocNoCond", Enabled = false, Criteria = "IsNew=1", Context = "Any")]
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
        public Department Department
        {
            get { return fDepartment; }
            set { SetPropertyValue<Department>("Department", ref fDepartment, value); }
        }

        private TaskCategory fTaskCategory;
        [RuleRequiredField("RuleRequiredField for Task.TaskCategory", DefaultContexts.Save,
        "Task Category must be specified")]
        [ImmediatePostData]
        [Association]
        public TaskCategory TaskCategory
        {
            get { return fTaskCategory; } 
            set
            {
                
                SetPropertyValue(nameof(TaskCategory), ref fTaskCategory, value);
                RefreshAvailableTaskSubCategory();
            }
        }

        private XPCollection<TaskSubCategory> fAvailableTaskSubCategories;
        [Browsable(false)] // Prohibits showing the AvailableAccessories collection separately
        public XPCollection<TaskSubCategory> AvailableTaskSubCategories
        {
            get
            {
                if (fAvailableTaskSubCategories == null)
                {
                    // Retrieve all Sample objects
                    fAvailableTaskSubCategories = new XPCollection<TaskSubCategory>(Session);
                    // Filter the retrieved collection according to current conditions
                    RefreshAvailableTaskSubCategory();
                }
                // Return the filtered collection of Sample objects
                return fAvailableTaskSubCategories;
            }
        }

        private void RefreshAvailableTaskSubCategory()
        {
            if (fAvailableTaskSubCategories == null)
                return;
            // Process the situation when the Party is not specified (see the Scenario 3 above)
            if (TaskCategory == null)
            {
                // Show only Global Collection when the Party is not specified
                fAvailableTaskSubCategories.Criteria = CriteriaOperator.Parse("1=0");
            }
            else
            {
                // Leave only the current Party's Collection in the fAvailableSampleCollection collection
                fAvailableTaskSubCategories.Criteria = new BinaryOperator("TaskCategory", TaskCategory);

                //    // Add Global Collection
                //    XPCollection<SampleCollection> availableGlobalAccessories =
                //       new XPCollection<SampleCollection>(Session);
                //    availableGlobalAccessories.Criteria = CriteriaOperator.Parse("Party",Party);
                //fAvailableSampleCollection.AddRange(availableGlobalAccessories);
            }
            // Set null for the Collection property to allow an end-user 
            //to set a new value from the refreshed data source
            if (this.IsNew == true )
            TaskSubCategory = null;
        }

        [DataSourceProperty(nameof(AvailableTaskSubCategories))]
        [Association]
        [RuleRequiredField("RuleRequiredField for Task.TaskSubCategory", DefaultContexts.Save, "A Sub Category must be specified")]
        //[Appearance("PickedCarNumberCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public TaskSubCategory TaskSubCategory
        {
            get { return fTaskSubCategory; }
            set
            {
                SetPropertyValue(nameof(TaskSubCategory), ref fTaskSubCategory, value);

            }
        }
        TaskSubCategory fTaskSubCategory;

        string fDescription;
        [Size(1000)]
        //[Appearance("TaskDescriptionCond", Enabled = false, Criteria = "@this.CreatedBy.Oid=CurrentUserId()", Context = "DetailView")]
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue<string>("Description", ref fDescription, value); }
        }

        private Employee fAssignTo;
        //[RuleRequiredField("RuleRequiredField for Task.AssignTo", DefaultContexts.Save,
        //"Agents must be specified")]
        //[Appearance("AgentPolicyCond", Enabled = false, Criteria = "@this.Agents.TypeOfEmployee='Agent'", Context = "DetailView")]
        [Association("Task-Employee")]
        public Employee AssignTo
        {
            get { return fAssignTo; }
            set
            {
                SetPropertyValue(nameof(AssignTo), ref fAssignTo, value);
                

            }
        }

        private Customer fCustomer;
        [Association("Task-Customer")]
        [ImmediatePostData]
        public Customer Customer
        {
            get { return fCustomer; }
            set
            {
                SetPropertyValue(nameof(Customer), ref fCustomer, value);
            }
        }

        private TaskStatus status;
        public virtual TaskStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                //if (isLoaded)
                //{
                    if (value == TaskStatus.Completed)
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


        [Association("Collaborators-Task")]
        public XPCollection<Employee> Collaborators
        {
            get
            {
                return GetCollection<Employee>(nameof(Collaborators));
            }
        }

        [Association("Comments-Task")]
        public XPCollection<TaskComment> TaskComments
        {
            get
            {
                return GetCollection<TaskComment>(nameof(TaskComments));
            }
        }

        [DevExpress.Xpo.Aggregated, Association]
        public XPCollection<TaskFileData> Attachments
        {
            get { return GetCollection<TaskFileData>(nameof(Attachments)); }
        }

        [Association("Task-CallLogCustomer")]
        [VisibleInDetailView(false)]
        public XPCollection<CallLogCustomer> CallLogCustomers
        {
            get
            {
                return GetCollection<CallLogCustomer>(nameof(CallLogCustomers));
            }
        }

        [Association("Task-CallLogEmloyees")]
        [VisibleInDetailView(false)]
        public XPCollection<CallLogEmployee> CallLogEmployees
        {
            get
            {
                return GetCollection<CallLogEmployee>(nameof(CallLogEmployees));
            }
        }


        [Association("Task-CallLogLeads")]
        [VisibleInDetailView(false)]
        public XPCollection<CallLogLead> CallLogLeads
        {
            get
            {
                return GetCollection<CallLogLead>(nameof(CallLogLeads));
            }
        }

        Company fCompany;
        //[Browsable(false)]
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "TaskID>0", Context = "Any")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        [Action(ImageName = "State_Task_Completed")]
        public void MarkCompleted()
        {
            Status = TaskStatus.Completed;
        }

        [Association("MyTask-TaskNotification")]
        public XPCollection<TaskNotification> MyNotifications
        {
            get
            {
                return GetCollection<TaskNotification>(nameof(MyNotifications));
            }
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

    public enum TaskStatus
    {
        [ImageName("State_Task_NotStarted")]
        NotStarted,
        [ImageName("State_Task_InProgress")]
        InProgress,
        [ImageName("State_Task_WaitingForSomeoneElse")]
        WaitingForSomeoneElse,
        [ImageName("State_Task_Deferred")]
        Deferred,
        [ImageName("State_Task_Completed")]
        Completed
    }
}