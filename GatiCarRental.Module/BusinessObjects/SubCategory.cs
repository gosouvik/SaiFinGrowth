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
using System.Runtime.Remoting.Contexts;
using DevExpress.ExpressApp.ConditionalAppearance;
using System.Collections;
using DevExpress.Xpo.Metadata;
using DevExpress.DataAccess.Native.Web;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty("CarNumber")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class SubCategory : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public SubCategory(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _SubCategoryID = (Session.Evaluate<SubCategory>(CriteriaOperator.Parse("Max(SubCategoryID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<SubCategory>(CriteriaOperator.Parse("Max(SubCategoryID)"), CriteriaOperator.Parse("")))) + 1;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
        }
        [Persistent("SubCategoryID")] // this line for read-only columns mapping
        private int _SubCategoryID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_SubCategoryID")] // This line for read-only column mapping
        public int SubCategoryID
        {
            get { return _SubCategoryID; }
        }

        string fName;
        //[RuleUniqueValue] // Validation for unique value
        //[RuleRequiredField] // Validation for Required
        [Size(50)]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        
        private Category fCategory;
        //[Association]
        //[VisibleInDetailView(false)]
        public Category Category
        {
            get { return fCategory; }
            set
            {
                SetPropertyValue(nameof(Category), ref fCategory, value);
            }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<CallLogCustomer> CallLogCustomers
        {
            get { return GetCollection<CallLogCustomer>(nameof(CallLogCustomers)); }
        }

                
        Company fCompany;
        [Browsable(false)]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        [Association]
        public XPCollection<CallLogEmployee> CallLogEmployees
        {
            get { return GetCollection<CallLogEmployee>(nameof(CallLogEmployees)); }
        }

        [Association]
        public XPCollection<CallLogLead> CallLogLeads
        {
            get { return GetCollection<CallLogLead>(nameof(CallLogLeads)); }
        }

        [Association]
        public XPCollection<Response> Responses
        {
            get { return GetCollection<Response>(nameof(Responses)); }
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