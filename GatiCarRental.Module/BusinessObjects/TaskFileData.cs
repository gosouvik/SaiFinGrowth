﻿using DevExpress.CodeParser;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
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
    [ImageName("BO_FileAttachment")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class TaskFileData : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public TaskFileData(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            //DocumentType = FileType.Unknown;
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        [RuleRequiredField("TaskFileDataRule", "Save", "File should be assigned")]
        [FileTypeFilter("DocumentFiles", 1, "*.txt", "*.doc")]
        [FileTypeFilter("AllFiles", 2, "*.*")]
        public FileData File { get; set; }

        [Association]
        public Task Task
        {
            get { return fTask; }
            set
            {
                SetPropertyValue(nameof(Task), ref fTask, value);
            }
        }
        Task fTask;

        [Association]
        public FileType FileType
        {
            get { return fFileType; }
            set
            {
                SetPropertyValue(nameof(FileType), ref fFileType, value);
            }
        }
        FileType fFileType;

    }
}