using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class TaskNotification : BaseObject, ISupportNotifications
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public TaskNotification(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        [Association("MyTask-TaskNotification")]
        public Task MyTask
        {
            get
            {
                return GetPropertyValue<Task>(nameof(MyTask));
            }
            set
            {
                SetPropertyValue(nameof(MyTask), value);
            }
        }
        [Association("AssignedTo-TaskNotifications")]
        public Employee AssignedTo
        {
            get
            {
                return GetPropertyValue<Employee>(nameof(AssignedTo));
            }
            set
            {
                SetPropertyValue(nameof(AssignedTo), value);
            }
        }
        #region ISupportNotifications members
        private DateTime? alarmTime;
        public DateTime? AlarmTime
        {
            get { return alarmTime; }
            set
            {
                if (value == null)
                {
                    IsPostponed = false;
                }
                SetPropertyValue(nameof(AlarmTime), ref alarmTime, value);
            }
        }

        [Browsable(false)]
        public bool IsPostponed
        {
            get
            {
                return GetPropertyValue<bool>(nameof(IsPostponed));
            }
            set
            {
                SetPropertyValue(nameof(IsPostponed), value);
            }
        }

        [Browsable(false)]
        public string NotificationMessage
        {
            get { return MyTask.Description; }
        }

        public object UniqueId
        {
            get
            {
                return Oid;
            }
        }
        #endregion ISupportNotifications members

    }
}