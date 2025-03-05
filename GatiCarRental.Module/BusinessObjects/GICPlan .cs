using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections;
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
    public class GICPlan : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public GICPlan(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _GICPlanID = (Session.Evaluate<GICPlan>(CriteriaOperator.Parse("Max(GICPlanID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<GICPlan>(CriteriaOperator.Parse("Max(GICPlanID)"), CriteriaOperator.Parse("")))) + 1;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        [Persistent("GICPlanID")] // this line for read-only columns mapping
        private int _GICPlanID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_GICPlanID")] // This line for read-only column mapping
        public int GICPlanID
        {
            get { return _GICPlanID; }
        }

        string fName;
        [Size(50)]
        [RuleUniqueValue] // Validation for unique value
        [RuleRequiredField("RuleRequiredField for GICPlan.Name", DefaultContexts.Save, "A GICPlan Name must be specified")]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        string fTableName;
        [Size(50)]
        [RuleUniqueValue] // Validation for unique value
        [RuleRequiredField("RuleRequiredField for GICPlan.TableName", DefaultContexts.Save, "A GICPlan Name must be specified")]
        public string TableName
        {
            get { return fTableName; }
            set { SetPropertyValue<string>("TableName", ref fTableName, value); }
        }

        string fDescription;
        [Size(4000)]
        //[RuleRequiredField] // Validation for Required
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue<string>("Description", ref fDescription, value); }
        }

        

        Company fCompany;
        [Browsable(false)]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        //[Association]
        //[VisibleInDetailView(false)]
        //public XPCollection<Policy> Policies
        //{
        //    get { return GetCollection<Policy>(nameof(Policies)); }
        //}
        //[Association]
        //[VisibleInDetailView(false)]
        //public XPCollection<LICApplication> LICApplications
        //{
        //    get { return GetCollection<LICApplication>(nameof(LICApplications)); }
        //}

        private InsuranceCompany fInsuranceCompany;
        [RuleRequiredField("RuleRequiredField for GICPlan.InsuranceCompany", DefaultContexts.Save,
        "Insurance Company must be specified")]
        [Association]
        public InsuranceCompany InsuranceCompany
        {
            get { return fInsuranceCompany; }
            set
            {
                SetPropertyValue(nameof(InsuranceCompany), ref fInsuranceCompany, value);
            }
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