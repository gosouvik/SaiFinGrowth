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
using Microsoft.VisualBasic;
using DevExpress.ExpressApp.ConditionalAppearance;
using System.Collections;
using DevExpress.Xpo.Metadata;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty("ManualDutySlipNo")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class DutySlip : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public DutySlip(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _DutySlipID = (Session.Evaluate<DutySlip>(CriteriaOperator.Parse("Max(DutySlipID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<DutySlip>(CriteriaOperator.Parse("Max(DutySlipID)"), CriteriaOperator.Parse("")))) + 1;
            //BookingOrder bookOrdObj = Session.FindObject<BookingOrder>(CriteriaOperator.Parse("BookingOrderID=[<BookingOrder>].Max(BookingOrderID)"));
            //if (bookOrdObj != null)
            //DocumentNo = "DS-" + (_DutySlipID).ToString();

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }

            GetDocumentNumbering();
            DutyDate = DateTime.Today;

            try
            {
                //Customer = this.BookingOrder.Customer;
            }
            catch (Exception e) { }
        }

        private void GetDocumentNumbering()
        {

            XPCollection<DocumentNumbering> docnos = new XPCollection<DocumentNumbering>(Session);
            docnos.Criteria = CriteriaOperator.Parse("[DocumentType]=?", DocumentType.DutySlip);
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

        [Persistent("DutySlipID")] // this line for read-only columns mapping
        private int _DutySlipID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_DutySlipID")] // This line for read-only column mapping
        public int DutySlipID
        {
            get { return _DutySlipID; }
        }

        string fDocumentNo;
        //[RuleRequiredField]
        [RuleUniqueValue]
        [Size(50)]
        public string DocumentNo
        {
            get { return fDocumentNo; }
            set { SetPropertyValue<string>("DocumentNo", ref fDocumentNo, value); }
        }
        DateTime fDutyDate;
        [Appearance("PickedDutyDateCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        //[RuleRequiredField]
        public DateTime DutyDate
        {
            get { return fDutyDate; }
            set
            {
                SetPropertyValue<DateTime>("DutyDate", ref fDutyDate, value);
                //RefreshAvailableBookingOrders();
            }
        }

        private Customer fCustomer;
        [Appearance("PickedCustomerCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [Appearance("BookingNumberCustomerCond", Enabled = false, Criteria = "BookingOrder != null", Context = "DetailView")]
        //[RuleRequiredField]
        [VisibleInLookupListView(true)]
        [Custom("LookupProperty", "CustomerLookup")]
        [Custom("IsVisibleInLookupListView", "True")]
        [ReadOnly(true)]
        //[DataSourceCriteria("Oid == '@this.BookingOrder.Customer.Oid'")]
        public Customer Customer
        {
            get { return fCustomer; }
            set
            {
                SetPropertyValue(nameof(Customer), ref fCustomer, value);
                // Refresh the Accessory Property data source
                //RefreshAvailableBookingOrders();
            }
        }


        [Association]
        [Appearance("PickedBookingOrderCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        //[RuleRequiredField]
        public BookingOrder BookingOrder
        {
            get { return fBookingOrder; }
            set
            {
                SetPropertyValue(nameof(BookingOrder), ref fBookingOrder, value);
                if (fBookingOrder != null)
                {
                    if (this.BookingOrder.Customer != null)
                    {
                        this.Customer = this.BookingOrder.Customer;
                        this.DutyDate = this.BookingOrder.PickupDateTime;
                        this.GarageOutDate = this.BookingOrder.PickupDateTime;
                        this.GarageOutTime = this.BookingOrder.ReportingTime;
                        this.GarageInDate = this.BookingOrder.DropOffDateTime;
                        this.CarNumber = this.BookingOrder.CarNumber;
                    }
                    CalculateTotal();

                }
            }
        }
        BookingOrder fBookingOrder;

        string fManualDutySlipNo;
        [Size(50)]
        [VisibleInLookupListView(true)]
        [Custom("LookupProperty", "ManualDutySlipNoLookup")]
        [Custom("IsVisibleInLookupListView", "True")]
        //[RuleRequiredField]
        [RuleUniqueValue]
        [Appearance("PickedManualDutySlipNoCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public string ManualDutySlipNo
        {
            get { return fManualDutySlipNo; }
            set { SetPropertyValue<string>("ManualDutySlipNo", ref fManualDutySlipNo, value); }
        }

        Car fCar;
        [ReadOnly(true)]
        [Appearance("PickedCarNumberCond", Enabled = false, Criteria = "BookingOrder != null", Context = "DetailView")]
        public Car CarNumber
        {
            get { return fCar; }
            set { SetPropertyValue(nameof(CarNumber), ref fCar, value); }
        }


        DateTime fGarageOutDate;
        //[RuleRequiredField]
        [ReadOnly(true)]
        [Appearance("PickedGarageOutDateCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [Appearance("BookingNumberGarageOutDateCond", Enabled = false, Criteria = "BookingOrder != null", Context = "DetailView")]
        public DateTime GarageOutDate
        {
            get { return fGarageOutDate; }
            set
            {
                SetPropertyValue<DateTime>("GarageOutDate", ref fGarageOutDate, value);
                CalculateTotal();
            }

        }

        TimeSpan fGarageOutTime;
        ////[RuleRequiredField]
        [Appearance("PickedGarageOutTimeCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public TimeSpan GarageOutTime
        {
            get { return fGarageOutTime; }
            set
            {
                SetPropertyValue<TimeSpan>("GarageOutTime", ref fGarageOutTime, value);
                CalculateTotal();
            }
        }

        decimal fGarageOutKm;
        [Appearance("PickedGarageOutKmCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        ////[RuleRequiredField]
        public decimal GarageOutKm
        {
            get { return fGarageOutKm; }
            set
            {
                SetPropertyValue<decimal>("GarageOutKm", ref fGarageOutKm, value);
                CalculateTotal();
            }
        }

        DateTime fGarageInDate;
        //[RuleRequiredField]
        [ReadOnly(true)]
        [Appearance("PickedGarageInDateCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [Appearance("BookingNumberGarageInDateCond", Enabled = false, Criteria = "BookingOrder != null", Context = "DetailView")]
        //[RuleRange("", DefaultContexts.Save, fGarageOutDate, fGarageOutDate.AddYears(5))]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual,
    "GarageOutDate", ParametersMode.Expression)]
        public DateTime GarageInDate
        {
            get { return fGarageInDate; }
            set
            {
                SetPropertyValue<DateTime>("GarageInDate", ref fGarageInDate, value);
                CalculateTotal();
            }
        }

        TimeSpan fGarageInTime;
        [Appearance("PickedGarageInTimeCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        ////[RuleRequiredField]
        public TimeSpan GarageInTime
        {
            get { return fGarageInTime; }
            set
            {
                SetPropertyValue<TimeSpan>("GarageInTime", ref fGarageInTime, value);
                CalculateTotal();
            }
        }


        decimal fGarageInKm;
        ////[RuleRequiredField]
        [Appearance("PickedGarageInKmCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThan,
    "GarageOutKm", ParametersMode.Expression)]
        public decimal GarageInKm
        {
            get { return fGarageInKm; }
            set
            {
                SetPropertyValue<decimal>("GarageInKm", ref fGarageInKm, value);
                CalculateTotal();
            }
        }

        decimal fTotalKm;
        [Appearance("PickedTotalKmCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public decimal TotalKm
        {
            get { return fTotalKm; }
            set { SetPropertyValue<decimal>("TotalKm", ref fTotalKm, value); }
        }

        decimal fTotalHours;
        [Appearance("PickedTotalHoursCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public decimal TotalHours
        {
            get { return fTotalHours; }
            set { SetPropertyValue<decimal>("TotalHours", ref fTotalHours, value); }
        }

        decimal fNightCharge;
        [Appearance("PickedNightChargeCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public decimal NightCharge
        {
            get { return fNightCharge; }
            set
            {
                SetPropertyValue<decimal>("NightCharge", ref fNightCharge, value);
                CalculateTotal();
            }
        }


        decimal fParkngCharges;
        [Appearance("PickedParkingChargesCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public decimal ParkngCharges
        {
            get { return fParkngCharges; }
            set
            {
                SetPropertyValue<decimal>("ParkngCharges", ref fParkngCharges, value);
                CalculateTotal();
            }
        }

        decimal fDecorationCharge;
        [Appearance("PickedDecorationChargeCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public decimal DecorationCharges
        {
            get { return fDecorationCharge; }
            set
            {
                SetPropertyValue<decimal>("DecorationCharges", ref fDecorationCharge, value);
                CalculateTotal();
            }
        }

        decimal fGrossAmount;
        [Appearance("PickedGrossAmountCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public decimal GrossAmount
        {
            get { return fGrossAmount; }
            set { SetPropertyValue<decimal>("GrossAmount", ref fGrossAmount, value); }
        }

        decimal fNetAmount;
        [Appearance("PickedNetAmountCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public decimal NetAmount
        {
            get { return fNetAmount; }
            set { SetPropertyValue<decimal>("NetAmount", ref fNetAmount, value); }
        }

        bool fChargedOnHours;
        [Browsable(false)]
        public bool ChargedOnHours
        {
            get { return fChargedOnHours; }
            set { SetPropertyValue<bool>("ChargedOnHours", ref fChargedOnHours, value); }
        }

        string fHiringTimeKm;
        [Appearance("PickedHiringTimeKmCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public string HiringTimeKm
        {
            get { return fHiringTimeKm; }
            set { SetPropertyValue<string>("HiringTimeKm", ref fHiringTimeKm, value); }
        }

        decimal fRatePerKmHour;
        [Appearance("PickedRatePerKmHourCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public decimal RatePerKmHour
        {
            get { return fRatePerKmHour; }
            set { SetPropertyValue<decimal>("RatePerKmHour", ref fRatePerKmHour, value); }
        }

        bool fPicked;
        public bool Picked
        {
            get { return fPicked; }
            set { SetPropertyValue<bool>("Picked", ref fPicked, value); }
        }

        Company fCompany;
        [Browsable(false)]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        [NonPersistent]
        [Browsable(false)]
        [RuleFromBoolProperty("EventIntervalValid", DefaultContexts.Save, "The garage out date must be less than the garage in date", SkipNullOrEmptyValues = false, UsedProperties = "GarageOutDate, GarageInDate")]
        public bool IsIntervalValid { get { return GarageOutDate <= GarageInDate; } }

        [Association]
        [Appearance("PickedInvoiceDetailsCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        [VisibleInDetailView(false)]
        public XPCollection<InvoiceDetail> InvoiceDetails
        {
            get { return GetCollection<InvoiceDetail>(nameof(InvoiceDetails)); }
        }
        private void CalculateTotal()
        {
            try
            {
                decimal _totalKm = 0;
                decimal _startKm = GarageOutKm;
                decimal _endKm = GarageInKm;

                _totalKm = _endKm - _startKm;

                DateTime _startDtTime = GarageOutDate + GarageOutTime;
                DateTime _endTime = GarageInDate + GarageInTime;

                double _totalHours = ((TimeSpan)(_endTime - _startDtTime)).TotalHours;

                TotalHours = (decimal)_totalHours;
                TotalKm = _totalKm;



                decimal _rateKm = 0;
                decimal _rateHr = 0;
                decimal _minkm = 0;
                decimal _minHr = 0;
                bool isMin = false;

                if (BookingOrder.DocumentNo != null)
                {
                    XPCollection<BookingOrder> getRecords = new XPCollection<BookingOrder>(Session);
                    getRecords.Criteria = CriteriaOperator.Parse("DocumentNo='" + BookingOrder.DocumentNo.ToString() + "'");
                    foreach (var item in getRecords)
                    {
                        //do something with code
                        _rateHr = item.PricePerHour;
                        _rateKm = item.PricePerKm;
                        _minHr = item.MinHour;
                        _minkm = item.MinKm;




                    }

                    if ((TotalHours < _minHr) && (TotalKm < _minkm))
                    {
                        _totalHours = (double)_minHr;
                        _totalKm = _minkm;
                        isMin = true;
                    }


                    decimal _totalHrAmt = (decimal)_totalHours * _rateHr;
                    decimal _totalKmAmt = _totalKm * _rateKm;
                    decimal _minAmount = _minHr * _rateHr;

                    if (_totalHrAmt >= _totalKmAmt)
                    {
                        GrossAmount = _totalHrAmt;
                        ChargedOnHours = true;


                        //double totalInHrs = _totalHours / 24;


                        var timeSpan = TimeSpan.FromHours(_totalHours);
                        int hh = timeSpan.Hours;
                        int mm = timeSpan.Minutes;
                        int ss = timeSpan.Seconds;
                        int days = timeSpan.Days;

                        hh = (days * 24) + hh;

                        if (mm == 0)
                            HiringTimeKm = hh.ToString() + " Hrs ";
                        else
                            HiringTimeKm = hh.ToString() + " Hrs " + mm + " Min";



                        RatePerKmHour = _rateHr;
                    }
                    else
                    {
                        GrossAmount = _totalKmAmt;
                        ChargedOnHours = false;


                        HiringTimeKm = Math.Round(_totalKm, 1).ToString() + " Kms";
                        RatePerKmHour = _rateKm;
                    }

                    if (GrossAmount < _minAmount)
                        GrossAmount = _minAmount;

                    if (isMin)
                        HiringTimeKm += " (Min)";

                    NetAmount = GrossAmount + ParkngCharges + NightCharge + DecorationCharges;
                }
            }
            catch { }
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

            if (!IsDeleted)
            {
                BookingOrder.Picked = false;
                BookingOrder.Save();
            }

        }


        protected override void OnSaving()
        {
            if (!IsDeleted)
            {
                DateTime reportingDateTime, garageInDateTime, actReportingDateTime, actGarageInDateTime;

                reportingDateTime = this.BookingOrder.PickupDateTime + this.BookingOrder.ReportingTime;
                garageInDateTime = this.BookingOrder.DropOffDateTime.AddHours(23).AddMinutes(59);

                actReportingDateTime = this.GarageOutDate + this.GarageOutTime;
                actGarageInDateTime = this.GarageInDate + this.GarageInTime;

                if (actReportingDateTime > reportingDateTime)
                    throw new InvalidOperationException("Duty Slip Reporting Date & Time must be greater than Booking Order Reporting Date & Time");

                if (actGarageInDateTime > garageInDateTime)
                    throw new InvalidOperationException("Duty Slip Garage In Date & Time must be greater than Booking Order Garage In Date & Time");
            
                BookingOrder.Picked = true;
                BookingOrder.Save();
            }
            else
            {
                //BookingOrder.Picked = false;
                //BookingOrder.Save();
            }

            int newnum = 1;
            DocumentNumbering docnos = Session.GetObjectByKey<DocumentNumbering>(this.DocSchemeOid);
            if (Session.IsNewObject(this))
            {
                if (this.IsNew == true)
                {
                    newnum = docnos.CurrentNo + 1;
                    this.DocumentNo = docnos.Prefix + new string('0', docnos.Body - (newnum).ToString().Length) + (newnum).ToString() + docnos.Suffix;
                }
            }
            base.OnSaving();

            if (this.IsNew == true)
            {
                docnos.CurrentNo = newnum;
                docnos.Save();

                this.IsNew = false;
                this.Save();
            }
        }
    }
}