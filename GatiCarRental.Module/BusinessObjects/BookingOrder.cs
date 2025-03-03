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
using DevExpress.ExpressApp.ConditionalAppearance;
using System.Collections;
using DevExpress.Xpo.Metadata;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    [DefaultProperty("DocumentNo")]
    [DeferredDeletion(false)]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    [RuleCriteria("ValidRangeDates", DefaultContexts.Save, "DropOffDateTime >= PickupDateTime", "Pickup Date must be less than Drop Off Date")]
    public class BookingOrder : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public BookingOrder(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _BookingOrderID = (Session.Evaluate<BookingOrder>(CriteriaOperator.Parse("Max(BookingOrderID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<BookingOrder>(CriteriaOperator.Parse("Max(BookingOrderID)"), CriteriaOperator.Parse("")))) + 1;
            //BookingOrder bookOrdObj = Session.FindObject<BookingOrder>(CriteriaOperator.Parse("BookingOrderID=[<BookingOrder>].Max(BookingOrderID)"));
            //if (bookOrdObj != null)

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }

            //DocumentNo = "BO-" + (_BookingOrderID).ToString();
            GetDocumentNumbering();
            DocumentDate = DateTime.Today;
            ReportingAddress = "West Bengal, India";
            if (ReportingPersonMobileNo == null)
                ReportingPersonMobileNo = "";
        }

        private void GetDocumentNumbering()
        {

            XPCollection<DocumentNumbering> docnos = new XPCollection<DocumentNumbering>(Session);
            docnos.Criteria = CriteriaOperator.Parse("[DocumentType]=?", DocumentType.BookingOrder);
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

        [Persistent("BookingOrderID")] // this line for read-only columns mapping
        private int _BookingOrderID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_BookingOrderID")] // This line for read-only column mapping
        public int BookingOrderID
        {
            get { return _BookingOrderID; }
        }

        string fDocumentNo;
        [RuleUniqueValue]
        //[RuleRequiredField]
        [ReadOnly(true)]
        [Size(50)]
        public string DocumentNo
        {
            get { return fDocumentNo; }
            set { SetPropertyValue<string>("DocumentNo", ref fDocumentNo, value); }
        }
        DateTime fDocumentDate;
        [ImmediatePostData(true)]
        //[RuleRequiredField]
        [Appearance("PickedDocumentDateCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public DateTime DocumentDate
        {
            get { return fDocumentDate; }
            set
            {
                SetPropertyValue<DateTime>("DocumentDate", ref fDocumentDate, value);
                CheckDate();
            }
        }

        private Customer fCustomer;
        //[RuleRequiredField]
        [Association]
        [Appearance("PickedCustomerCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public Customer Customer
        {
            get { return fCustomer; }
            set
            {
                SetPropertyValue(nameof(Customer), ref fCustomer, value);
                PreviousBalance = CalculatePreviousBalance();
            }
        }

        private bool fSameAsCustomer;
        [ImmediatePostData(true)]
        [Appearance("PickedSameAsCustomerCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public bool SameAsCustomer
        {
            get { return fSameAsCustomer; }
            set
            {
                SetPropertyValue(nameof(SameAsCustomer), ref fSameAsCustomer, value);
                SetReportingPerson();
            }
        }

        private void SetReportingPerson()
        {
            if (SameAsCustomer == true)
            {
                ReportingPersonName = Customer.Name;
                ReportingPersonMobileNo = Customer.Phone1;
                ReportingPersonMobileNo2 = Customer.Phone2;
                ReportingAddress = Customer.Address1 + ", " + Customer.City + " - " + Customer.PinCode + ", " + Customer.State + ", " + Customer.Country ;
            }
            else
            {
                ReportingPersonName = string.Empty;
                ReportingPersonMobileNo = string.Empty;
                ReportingPersonMobileNo2 = string.Empty;
                ReportingAddress = "West Bengal, India";
            }
        }

        string fReportingPersonName;
        [Size(50)]
        [Appearance("PickedReportingPersonCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [Appearance("SameAsCustomerReportingPersonCond", Enabled = false, Criteria = "SameAsCustomer", Context = "DetailView")]
        public string ReportingPersonName
        {
            get { return fReportingPersonName; }
            set { SetPropertyValue<string>("ReportingPersonName", ref fReportingPersonName, value); }
        }
        string fReportingPersonMobileNo;
        [RuleRegularExpression("", DefaultContexts.Save, "\\A[0-9]{10}\\z", "Mobile Number 1 must be 10 digits")]
        [Appearance("PickedMobile1Cond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [Appearance("SameAsCustomerReportingPersonMobileNoCond", Enabled = false, Criteria = "SameAsCustomer", Context = "DetailView")]
        //[RuleRequiredField]
        [Size(10)]
        public string ReportingPersonMobileNo
        {
            get { return fReportingPersonMobileNo; }
            set { SetPropertyValue<string>("ReportingPersonMobileNo", ref fReportingPersonMobileNo, value); }
        }

        string fReportingPersonMobileNo2;
        //[RuleRegularExpression("", DefaultContexts.Save, "\\A[0-9]{20}\\z", "Mobile Number 2 must be 20 digits")]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.NotEquals, "ReportingPersonMobileNo",ParametersMode.Expression)]
        [Appearance("PickedMobile2Cond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [Appearance("SameAsCustomerReportingPersonMobileNo2Cond", Enabled = false, Criteria = "SameAsCustomer", Context = "DetailView")]
        [Size(20)]
        public string ReportingPersonMobileNo2
        {
            get { return fReportingPersonMobileNo2; }
            set { SetPropertyValue<string>("ReportingPersonMobileNo2", ref fReportingPersonMobileNo2, value); }
        }
        string fReportingAddress;
        [Size(1000)]
        [Appearance("PickedAddressCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        //[Appearance("SameAsCustomerReportingAddressCond", Enabled = false, Criteria = "SameAsCustomer", Context = "DetailView")]
        public string ReportingAddress
        {
            get { return fReportingAddress; }
            set { SetPropertyValue<string>("ReportingAddress", ref fReportingAddress, value); }
        }

        private CarType fBookingCarType;
        [Appearance("PickedBookingCarTypeCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        //[RuleRequiredField]
        public CarType BookingCarType
        {
            get { return fBookingCarType; }
            set
            {
                SetPropertyValue(nameof(BookingCarType), ref fBookingCarType, value);
                // Refresh the Accessory Property data source
                //RefreshAvailableCars();
            }
        }

        private CarType fCarType;
        [Appearance("PickedCarTypeCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public CarType CarType
        {
            get { return fCarType; }
            set
            {
                SetPropertyValue(nameof(CarType), ref fCarType, value);
                // Refresh the Accessory Property data source
                RefreshAvailableCars();
            }
        }

        private XPCollection<Car> fAvailableCars;
        [Browsable(false)] // Prohibits showing the AvailableAccessories collection separately
        public XPCollection<Car> AvailableCars
        {
            get
            {
                if (fAvailableCars == null)
                {
                    // Retrieve all Sample objects
                    fAvailableCars = new XPCollection<Car>(Session);
                    // Filter the retrieved collection according to current conditions
                    RefreshAvailableCars();
                }
                // Return the filtered collection of Sample objects
                return fAvailableCars;
            }
        }
        private void RefreshAvailableCars()
        {
            if (fAvailableCars == null)
                return;
            // Process the situation when the Party is not specified (see the Scenario 3 above)
            if (CarType == null)
            {
                // Show only Global Collection when the Party is not specified
                fAvailableCars.Criteria = CriteriaOperator.Parse("1=0");
            }
            else
            {
                // Leave only the current Party's Collection in the fAvailableSampleCollection collection
                fAvailableCars.Criteria = new BinaryOperator("CarType", CarType);

                //    // Add Global Collection
                //    XPCollection<SampleCollection> availableGlobalAccessories =
                //       new XPCollection<SampleCollection>(Session);
                //    availableGlobalAccessories.Criteria = CriteriaOperator.Parse("Party",Party);
                //fAvailableSampleCollection.AddRange(availableGlobalAccessories);
            }
            // Set null for the Collection property to allow an end-user 
            //to set a new value from the refreshed data source
            CarNumber = null;
        }

        [DataSourceProperty(nameof(AvailableCars))]
        [Association]
        [Appearance("PickedCarNumberCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public Car CarNumber
        {
            get { return fCar; }
            set { SetPropertyValue(nameof(CarNumber), ref fCar, value); }
        }
        Car fCar;


        DateTime fPickupDateTime;
        //[RuleRequiredField]
        [Appearance("PickedPickupDateCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public DateTime PickupDateTime
        {
            get { return fPickupDateTime; }
            set { SetPropertyValue<DateTime>("PickupDateTime", ref fPickupDateTime, value); }
        }

        TimeSpan fReportingTime;
        [Appearance("PickedReportingTimeCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        ////[RuleRequiredField]
        public TimeSpan ReportingTime
        {
            get { return fReportingTime; }
            set { SetPropertyValue<TimeSpan>("ReportingTime", ref fReportingTime, value); }
        }

        DateTime fDropOffDateTime;
        //[RuleRequiredField]
        [Appearance("PickedDropOffDateCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual,
    "PickupDateTime", ParametersMode.Expression)]
        public DateTime DropOffDateTime
        {
            get { return fDropOffDateTime; }
            set { SetPropertyValue<DateTime>("DropOffDateTime", ref fDropOffDateTime, value); }
        }

        bool fIsDry;
        [ImmediatePostData]
        [Appearance("PickedIsDryCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [Appearance("IsGSTApplicableIsDryCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Company.IsGSTApplicable", Context = "Any")]
        public bool IsDry
        {
            get { return fIsDry; }
            set { SetPropertyValue<bool>("IsDry", ref fIsDry, value); }
        }

        decimal fPricePerHour;
        
        [Appearance("PickedPricePerHourCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [Appearance("IsDryPricePerHour", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        [RuleRange("", DefaultContexts.Save, 1, 999999, TargetCriteria = "IsDry=0")]
        
        public decimal PricePerHour
        {
            get { return fPricePerHour; }
            set { SetPropertyValue<decimal>("PricePerHour", ref fPricePerHour, value); }
        }
        decimal fPricePerKm;
        //[RuleRequiredField]
        [Appearance("IsDryPricePerKm", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        [Appearance("PickedPricePerKmCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [RuleRange("", DefaultContexts.Save, 1, 999999, TargetCriteria = "IsDry=0")]
        public decimal PricePerKm
        {
            get { return fPricePerKm; }
            set { SetPropertyValue<decimal>("PricePerKm", ref fPricePerKm, value); }
        }


        decimal fMinHour;
        //[RuleRequiredField]
        [Appearance("IsDryMinHour", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        [Appearance("PickedMinHourCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [RuleRange("", DefaultContexts.Save, 1, 999999, TargetCriteria = "IsDry=0")]
        public decimal MinHour
        {
            get { return fMinHour; }
            set { SetPropertyValue<decimal>("MinHour", ref fMinHour, value); }
        }
        decimal fMinKm;
        //[RuleRequiredField]
        [Appearance("IsDryMinKm", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        [Appearance("PickedMinKmCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [RuleRange("", DefaultContexts.Save, 1, 999999, TargetCriteria = "IsDry=0")]
        public decimal MinKm
        {
            get { return fMinKm; }
            set { SetPropertyValue<decimal>("MinKm", ref fMinKm, value); }
        }

        decimal fNightCharge;
        //[RuleRequiredField]
        [Appearance("IsDryNightCharge", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        [Appearance("PickedNightChargeCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public decimal NightCharge
        {
            get { return fNightCharge; }
            set { SetPropertyValue<decimal>("NightCharge", ref fNightCharge, value); }
        }

        decimal fDecorationCharge;
        [Appearance("IsDryDecorationCharges", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        [Appearance("PickedDecorationChargesCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public decimal DecorationCharges
        {
            get { return fDecorationCharge; }
            set { SetPropertyValue<decimal>("DecorationCharges", ref fDecorationCharge, value); }
        }

        private Receipt fReceipt;
        [Appearance("IsDryReceipt", Enabled = false, Criteria = "IsDry", Context = "DetailView")]
        [Appearance("PickedReceiptCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [DataSourceCriteria("Customer.Oid == '@this.Customer.Oid' && DocumentNo='NewDocumentOnly'")]
        public Receipt Receipt
        {
            get { return fReceipt; }
            set
            {
                SetPropertyValue(nameof(Receipt), ref fReceipt, value);
                fetchReceiptAmount();
            }
        }

        decimal fAdvanceAmount;
        [Appearance("PickedAdvanceAmountCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public decimal AdvanceAmount
        {
            get { return fAdvanceAmount; }
            set { SetPropertyValue<decimal>("AdvanceAmount", ref fAdvanceAmount, value); }
        }

        bool fPicked;
        public bool Picked
        {
            get { return fPicked; }
            set { SetPropertyValue<bool>("Picked", ref fPicked, value); }
        }

        decimal fPreviousBalance;
        [ReadOnly(true)]
        public decimal PreviousBalance
        {
            get
            {
                //fPreviousBalance = CalculatePreviousBalance();
                return fPreviousBalance;
            }
            set { SetPropertyValue<decimal>("PreviousBalance", ref fPreviousBalance, value); }
        }

        private decimal CalculatePreviousBalance()
        {
            decimal InvoiceBalance = (Session.Evaluate<Invoice>(CriteriaOperator.Parse("Sum(NetAmount)"), CriteriaOperator.Parse("Customer=?", this.Customer)) == DBNull.Value ? 0 : Convert.ToDecimal(Session.Evaluate<Invoice>(CriteriaOperator.Parse("Sum(NetAmount)"), CriteriaOperator.Parse("Customer=? and Oid!=?", this.Customer, this.Oid))));
            decimal ReceiptBalance = (Session.Evaluate<Receipt>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer)) == DBNull.Value ? 0 : Convert.ToDecimal(Session.Evaluate<Receipt>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer))));
            decimal DryInvoiceBalance = (Session.Evaluate<DryInvoice>(CriteriaOperator.Parse("Sum(NetAmount)"), CriteriaOperator.Parse("Customer=?", this.Customer)) == DBNull.Value ? 0 : Convert.ToDecimal(Session.Evaluate<DryInvoice>(CriteriaOperator.Parse("Sum(NetAmount)"), CriteriaOperator.Parse("Customer=?", this.Customer))));
            decimal TDSBalance = (Session.Evaluate<TDS>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer)) == DBNull.Value ? 0 : Convert.ToDecimal(Session.Evaluate<TDS>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer))));
            decimal DiscountBalance = (Session.Evaluate<Discount>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer)) == DBNull.Value ? 0 : Convert.ToDecimal(Session.Evaluate<Discount>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer))));
            decimal xprevBal = ReceiptBalance - InvoiceBalance - DryInvoiceBalance + TDSBalance + DiscountBalance;
            return xprevBal;
        }

        Company fCompany;
        //[Browsable(false)]
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "AdvanceAmount>=0", Context = "DetailView")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        [Appearance("PickedDutySlipsCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public XPCollection<DutySlip> DutySlips
        {
            get { return GetCollection<DutySlip>(nameof(DutySlips)); }
        }

        private void CheckDate()
        {
            if (DocumentDate > DateTime.Today)
                throw new UserFriendlyException("Invalid Date");
        }

        private void fetchReceiptAmount()
        {
            try
            {
                if (Receipt != null)
                {
                    if (Receipt.DocumentNo != null)
                    {
                        if (Receipt.DocumentNo.ToString().Trim() != "")
                        {
                            XPCollection<Receipt> getRecords = new XPCollection<Receipt>(Session);
                            getRecords.Criteria = CriteriaOperator.Parse("DocumentNo='" + Receipt.DocumentNo.ToString() + "'");
                            foreach (var item in getRecords)
                            {
                                AdvanceAmount = item.Amount;
                            }
                        }
                    }
                }
            }
            catch { }
        }

        protected override void OnSaving()
        {
            CheckDate();
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

            if (!(this.Session is NestedUnitOfWork) &&  this.IsNew == true)
            {
                docnos.CurrentNo = newnum;
                docnos.Save();

                this.IsNew = false;
                this.Save();
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
                                    throw new UserFriendlyException($"{this.DocumentNo.ToString()} Cannot be deleted. It is refrenced in: {mi.Name}");
                                else
                                    throw new UserFriendlyException($"{this.DocumentNo.ToString()} Cannot be deleted. It is refrenced in: {mi.DisplayName}");
                            }

                        }
                    }
                }
            }


        }
    }
}