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

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty("CarNumber")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Response : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Response(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _ResponseID = (Session.Evaluate<Response>(CriteriaOperator.Parse("Max(ResponseID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Response>(CriteriaOperator.Parse("Max(ResponseID)"), CriteriaOperator.Parse("")))) + 1;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
        }
        [Persistent("ResponseID")] // this line for read-only columns mapping
        private int _ResponseID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_ResponseID")] // This line for read-only column mapping
        public int ResponseID
        {
            get { return _ResponseID; }
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
        [RuleRequiredField("RuleRequiredField for Response.Category", DefaultContexts.Save,
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
        //[Appearance("PickedCarNumberCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public SubCategory SubCategory
        {
            get { return fSubCategory; }
            set { SetPropertyValue(nameof(SubCategory), ref fSubCategory, value); }
        }
        SubCategory fSubCategory;

        bool fCallLogLead;
        public bool CallLogLead
        {
            get { return fCallLogLead; }
            set { SetPropertyValue<bool>("CallLogLead", ref fCallLogLead, value); }
        }

        bool fCallLogCustomer;
        public bool CallLogCustomer
        {
            get { return fCallLogCustomer; }
            set { SetPropertyValue<bool>("CallLogCustomer", ref fCallLogCustomer, value); }
        }

        bool fCallLogEmployee;
        public bool CallLogEmployee
        {
            get { return fCallLogEmployee; }
            set { SetPropertyValue<bool>("CallLogEmployee", ref fCallLogEmployee, value); }
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
        public XPCollection<CallLogCustomer> CallLogCustomers
        {
            get { return GetCollection<CallLogCustomer>(nameof(CallLogCustomers)); }
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