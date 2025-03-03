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
    public class Car : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Car(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _CarID = (Session.Evaluate<Car>(CriteriaOperator.Parse("Max(CarID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Car>(CriteriaOperator.Parse("Max(CarID)"), CriteriaOperator.Parse("")))) + 1;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
        }
        [Persistent("CarID")] // this line for read-only columns mapping
        private int _CarID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_CarID")] // This line for read-only column mapping
        public int CarID
        {
            get { return _CarID; }
        }

        string fCarNumber;
        [RuleUniqueValue] // Validation for unique value
        //[RuleRequiredField] // Validation for Required
        [Size(50)]
        public string CarNumber
        {
            get { return fCarNumber; }
            set { SetPropertyValue<string>("CarNumber", ref fCarNumber, value); }
        }

        
        private CarType fCarType;
        [Association]
        //[RuleRequiredField] // Validation for Required
        public CarType CarType
        {
            get { return fCarType; }
            set
            {
                SetPropertyValue(nameof(CarType), ref fCarType, value);
            }
        }

        string fHSNSAC;
        [Appearance("IsGSTApplicableGSTRegNoCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Company.IsGSTApplicable", Context = "Any")]
        public string HSNSAC
        {
            get { return fHSNSAC; }
            set { SetPropertyValue<string>("HSNSAC", ref fHSNSAC, value); }
        }

        string fDriverName;
        //[RuleRequiredField] // Validation for Required
        public string DriverName
        {
            get { return fDriverName; }
            set { SetPropertyValue<string>("DriverName", ref fDriverName, value); }
        }

        string fDriverMobile;
        //[RuleRequiredField] // Validation for Required
        public string DriverMobile
        {
            get { return fDriverMobile; }
            set { SetPropertyValue<string>("DriverMobile", ref fDriverMobile, value); }
        }

        Company fCompany;
        [Browsable(false)]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        [Association]
        public XPCollection<BookingOrder> BookingOrders
        {
            get { return GetCollection<BookingOrder>(nameof(BookingOrders)); }
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
                                    throw new UserFriendlyException($"{this.CarNumber.ToString()} Cannot be deleted. It is refrenced in: {mi.Name}");
                                else
                                    throw new UserFriendlyException($"{this.CarNumber.ToString()} Cannot be deleted. It is refrenced in: {mi.DisplayName}");
                            }

                        }
                    }
                }
            }


        }

    }
}