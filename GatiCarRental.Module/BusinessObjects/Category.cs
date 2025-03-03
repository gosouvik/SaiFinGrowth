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
    public class Category : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Category(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _CategoryID = (Session.Evaluate<Category>(CriteriaOperator.Parse("Max(CategoryID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Category>(CriteriaOperator.Parse("Max(CategoryID)"), CriteriaOperator.Parse("")))) + 1;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
        }
        [Persistent("CategoryID")] // this line for read-only columns mapping
        private int _CategoryID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_CategoryID")] // This line for read-only column mapping
        public int CategoryID
        {
            get { return _CategoryID; }
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

        //[Association]
        ////[VisibleInDetailView(false)]
        //public XPCollection<SubCategory> SubCategories
        //{
        //    get { return GetCollection<SubCategory>(nameof(SubCategories)); }
        //}

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<CallLogCustomer> CallLogCustomers
        {
            get { return GetCollection<CallLogCustomer>(nameof(CallLogCustomers)); }
        }


        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<Customer> Customers
        {
            get { return GetCollection<Customer>(nameof(Customers)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<CallLogEmployee> CallLogEmployees
        {
            get { return GetCollection<CallLogEmployee>(nameof(CallLogEmployees)); }
        }

        [Association]
        [VisibleInDetailView(false)]
        public XPCollection<CallLogLead> CallLogLeads
        {
            get { return GetCollection<CallLogLead>(nameof(CallLogLeads)); }
        }

        [Association]
        [VisibleInDetailView(false)]
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