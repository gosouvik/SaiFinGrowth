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
using System.Collections;
using DevExpress.Xpo.Metadata;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class FileType : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public FileType(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _FileTypeID = (Session.Evaluate<FileType>(CriteriaOperator.Parse("Max(FileTypeID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<FileType>(CriteriaOperator.Parse("Max(FileTypeID)"), CriteriaOperator.Parse("")))) + 1;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
        }
        [Persistent("FileTypeID")] // this line for read-only columns mapping
        private int _FileTypeID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_FileTypeID")] // This line for read-only column mapping
        public int FileTypeID
        {
            get { return _FileTypeID; }
        }

        string fName;
        [Size(50)]
        [RuleUniqueValue] // Validation for unique value
        //[RuleRequiredField] // Validation for Required
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        Company fCompany;
        [Browsable(false)]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<TaskFileData> Tasks
        {
            get { return GetCollection<TaskFileData>(nameof(Tasks)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<CustomerFileData> Customers
        {
            get { return GetCollection<CustomerFileData>(nameof(Customers)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<LicPolicyFileData> LICPolicies
        {
            get { return GetCollection<LicPolicyFileData>(nameof(LICPolicies)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<MediclaimPolicyFileData> MediclaimPolicies
        {
            get { return GetCollection<MediclaimPolicyFileData>(nameof(MediclaimPolicies)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<MediclaimReceiptFileData> MediclaimReceiptPolicies
        {
            get { return GetCollection<MediclaimReceiptFileData>(nameof(MediclaimReceiptPolicies)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<ReceiptFileData> LICPolicyReceipts
        {
            get { return GetCollection<ReceiptFileData>(nameof(LICPolicyReceipts)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<LeadFileData> Leads
        {
            get { return GetCollection<LeadFileData>(nameof(Leads)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<EmployeeFileData> Employees
        {
            get { return GetCollection<EmployeeFileData>(nameof(Employees)); }
        }


        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<LICApplicationFileData> LICApplications
        {
            get { return GetCollection<LICApplicationFileData>(nameof(LICApplications)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<MediclaimFileData> Mediclaims
        {
            get { return GetCollection<MediclaimFileData>(nameof(Mediclaims)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<LoanFileData> Loans
        {
            get { return GetCollection<LoanFileData>(nameof(Loans)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<GICApplicationFileData> GICApplications
        {
            get { return GetCollection<GICApplicationFileData>(nameof(GICApplications)); }
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