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
using GatiCarRental.Module.Controllers;
using System.Collections;
using DevExpress.Xpo.Metadata;
using DevExpress.ExpressApp.ConditionalAppearance;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class DryInvoice : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public DryInvoice(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            int MaxDryInvoiceID = 0;
            _InvoiceID = (Session.Evaluate<DryInvoice>(CriteriaOperator.Parse("Max(InvoiceID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<DryInvoice>(CriteriaOperator.Parse("Max(InvoiceID)"), CriteriaOperator.Parse("")))) + 1;
            MaxDryInvoiceID = (Session.Evaluate<Invoice>(CriteriaOperator.Parse("Max(InvoiceID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Invoice>(CriteriaOperator.Parse("Max(InvoiceID)"), CriteriaOperator.Parse("")))) + 1;
            if (MaxDryInvoiceID > _InvoiceID)
                _InvoiceID = MaxDryInvoiceID;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            //InvoiceNo = "G/" + (_InvoiceID).ToString().PadLeft(6, '0') + "/21-22";
            GetDocumentNumbering();

            InvoiceDate = DateTime.Today;
            fFuelChargesMileage = 12;
            fExtraKmRate = 5;
            fExtraHoursRate = 35;
            if (Company.IsGSTApplicable == true)
            {
                SGSTPercentage = 6;
                CGSTPercentage = 6;
            }
            else
            {
                SGSTPercentage = 0;
                CGSTPercentage = 0;
            }
            MobileMileage = 500;
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }


        private void GetDocumentNumbering()
        {

            XPCollection<DocumentNumbering> docnos = new XPCollection<DocumentNumbering>(Session);
            docnos.Criteria = CriteriaOperator.Parse("[DocumentType]=?", DocumentType.Invoice);
            if (docnos.Count == 1)
            {
                foreach (DocumentNumbering docno in docnos)
                {
                    InvoiceNo = docno.Prefix + new String('X', docno.Body) + docno.Suffix;
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

        [Persistent("InvoiceID")] // this line for read-only columns mapping
        private int _InvoiceID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_InvoiceID")] // This line for read-only column mapping
        public int InvoiceID
        {
            get { return _InvoiceID; }
        }

        string fInvoiceNo;
        [RuleUniqueValue]
        //[RuleRequiredField]
        [Size(50)]
        public string InvoiceNo
        {
            get { return fInvoiceNo; }
            set { SetPropertyValue<string>("InvoiceNo", ref fInvoiceNo, value); }
        }
        DateTime fInvoiceDate;
        //[RuleRequiredField]
        public DateTime InvoiceDate
        {
            get { return fInvoiceDate; }
            set { SetPropertyValue<DateTime>("InvoiceDate", ref fInvoiceDate, value); }
        }

        private Customer fCustomer;
        [ImmediatePostData(true)]
        //[RuleRequiredField]
        [Association]
        public Customer Customer
        {
            get { return fCustomer; }
            set
            {
                SetPropertyValue(nameof(Customer), ref fCustomer, value);
                PreviousBalance = CalculatePreviousBalance();
                // Refresh the Accessory Property data source
            }
        }


        DateTime fBillingFromDate;
        //[RuleRequiredField]
        public DateTime BillingFromDate
        {
            get { return fBillingFromDate; }
            set { SetPropertyValue<DateTime>("BillingFromDate", ref fBillingFromDate, value); }
        }

        DateTime fBillingToDate;
        //[RuleRequiredField]
        public DateTime BillingToDate
        {
            get { return fBillingToDate; }
            set { SetPropertyValue<DateTime>("BillingToDate", ref fBillingToDate, value); }
        }


        private CarType fCarType;
        //[RuleRequiredField]
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
                fAvailableCars.Criteria = CriteriaOperator.Parse("1=1");
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
        ////[RuleRequiredField]
        public Car CarNumber
        {
            get { return fCar; }
            set { SetPropertyValue(nameof(CarNumber), ref fCar, value); }
        }
        Car fCar;

        decimal fCarHiringCharges;
        ////[RuleRequiredField]
        public decimal CarHiringCharges
        {
            get
            {
                return fCarHiringCharges;
            }
            set
            {
                SetPropertyValue<decimal>("CarHiringCharges", ref fCarHiringCharges, value);
                CalculateTotal();
                //CalculateGrossAmount();
            }
        }

        string fRemarks;
        [Size(255)]
        public string Remarks
        {
            get { return fRemarks; }
            set { SetPropertyValue<string>("Remarks", ref fRemarks, value); }
        }

        decimal fFuelChargesKm;
        ////[RuleRequiredField]
        public decimal FuelChargesKm
        {
            get
            {
                return fFuelChargesKm;
            }
            set
            {
                SetPropertyValue<decimal>("FuelChargesKm", ref fFuelChargesKm, value);
                this.MobileKms = fFuelChargesKm;
                CalculateTotal();
                //CalculateFuelCharges();
            }
        }

        decimal fFuelChargesMileage;
        ////[RuleRequiredField]
        public decimal FuelChargesMileage
        {
            get
            {
                return fFuelChargesMileage;
            }
            set
            {
                SetPropertyValue<decimal>("FuelChargesMileage", ref fFuelChargesMileage, value);
                CalculateTotal();
                //CalculateFuelCharges();
            }
        }

        decimal fFuelChargesRatePerLitre;
        ////[RuleRequiredField]
        public decimal FuelChargesRatePerLitre
        {
            get
            {
                return fFuelChargesRatePerLitre;
            }
            set
            {
                SetPropertyValue<decimal>("FuelChargesRatePerLitre", ref fFuelChargesRatePerLitre, value);
                CalculateTotal();
                //CalculateFuelCharges();
            }
        }

        decimal fFuelCharges;
        ////[RuleRequiredField]
        public decimal FuelCharges
        {
            get
            {
                //fFuelCharges = CalculateFuelCharges();
                return fFuelCharges;
            }
            set { SetPropertyValue<decimal>("FuelCharges", ref fFuelCharges, value); }
        }

        decimal fExtraKm;
        //[RuleRequiredField]
        public decimal ExtraKm
        {
            get
            {
                return fExtraKm;
            }
            set
            {
                SetPropertyValue<decimal>("ExtraKm", ref fExtraKm, value);
                //CalculateExtraKmCharges();
                CalculateTotal();
            }
        }

        decimal fExtraKmRate;
        ////[RuleRequiredField]
        public decimal ExtraKmRate
        {
            get
            {
                return fExtraKmRate;
            }
            set
            {
                SetPropertyValue<decimal>("ExtraKmRate", ref fExtraKmRate, value);
                //CalculateExtraKmCharges();
                CalculateTotal();
            }
        }

        decimal fExtraKmCharges;
        //[RuleRequiredField]
        public decimal ExtraKmCharges
        {
            get
            {
                //fExtraKmCharges = CalculateExtraKmCharges();
                return fExtraKmCharges;
            }
            set { SetPropertyValue<decimal>("ExtraKmCharges", ref fExtraKmCharges, value); }
        }

        decimal fExtraHours;
        ////[RuleRequiredField]
        public decimal ExtraHours
        {
            get
            {
                return fExtraHours;
            }
            set
            {
                SetPropertyValue<decimal>("ExtraHours", ref fExtraHours, value);
                //CalculateExtraHourCharges();
                CalculateTotal();
            }
        }

        decimal fExtraHoursRate;
        //[RuleRequiredField]
        public decimal ExtraHoursRate
        {
            get
            {
                return fExtraHoursRate;
            }
            set
            {
                SetPropertyValue<decimal>("ExtraHoursRate", ref fExtraHoursRate, value);
                //CalculateExtraHourCharges();
                CalculateTotal();
            }
        }

        decimal fExtraHoursCharges;
        //[RuleRequiredField]
        public decimal ExtraHoursCharges
        {
            get
            {
                //fExtraHoursCharges = CalculateExtraHourCharges();
                return fExtraHoursCharges;
            }
            set { SetPropertyValue<decimal>("ExtraHoursCharges", ref fExtraHoursCharges, value); }
        }

        decimal fMobileKms;
        //[RuleRequiredField]
        public decimal MobileKms
        {
            get
            {
                return fMobileKms;
            }
            set
            {
                SetPropertyValue<decimal>("MobileKms", ref fMobileKms, value);
                //CalculateMobileCharges();
                CalculateTotal();
            }
        }

        decimal fMobileMileage;
        public decimal MobileMileage
        {
            get
            {
                return fMobileMileage;
            }
            set
            {
                SetPropertyValue<decimal>("MobileMileage", ref fMobileMileage, value);
                //CalculateMobileCharges();
                CalculateTotal();
            }
        }

        decimal fMobileRate;
        public decimal MobileRate
        {
            get
            {
                return fMobileRate;
            }
            set
            {
                SetPropertyValue<decimal>("MobileRate", ref fMobileRate, value);
                //CalculateMobileCharges();
                CalculateTotal();
            }
        }

        decimal fMobileCharges;
        public decimal MobileCharges
        {
            get
            {
                //fMobileCharges = CalculateMobileCharges();
                return fMobileCharges;
            }
            set { SetPropertyValue<decimal>("MobileCharges", ref fMobileCharges, value); }
        }

        decimal fNightRate;
        //[RuleRequiredField]
        public decimal NightRate
        {
            get
            {
                return fNightRate;
            }
            set
            {
                SetPropertyValue<decimal>("NightRate", ref fNightRate, value);
                //CalculateNightCharges();
                CalculateTotal();
            }
        }

        decimal fNoOfNight;
        public decimal NoOfNight
        {
            get
            {
                return fNoOfNight;
            }
            set
            {
                SetPropertyValue<decimal>("NoOfNight", ref fNoOfNight, value);
                CalculateTotal();
                //CalculateNightCharges();
            }
        }

        decimal fNightCharges;
        public decimal NightCharges
        {
            get
            {
                //fNightCharges = CalculateNightCharges();
                return fNightCharges;
            }
            set { SetPropertyValue<decimal>("NightCharges", ref fNightCharges, value); }
        }

        decimal fParkngCharges;
        public decimal ParkngCharges
        {
            get
            {
                return fParkngCharges;
            }
            set { SetPropertyValue<decimal>("ParkngCharges", ref fParkngCharges, value);
                CalculateTotal();
            }
        }

        decimal fGrossAmount;
        public decimal GrossAmount
        {
            get
            {
                //fGrossAmount = CalculateGrossAmount();
                return fGrossAmount;
            }
            set { SetPropertyValue<decimal>("GrossAmount", ref fGrossAmount, value); }
        }

        

        decimal fSGSTPercentage;
        [Appearance("IsGSTApplicableSGSTPerCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Company.IsGSTApplicable", Context = "DetailView")]
        public decimal SGSTPercentage
        {
            get
            {
                return fSGSTPercentage;
                
            }
            set
            {
                SetPropertyValue<decimal>("SGSTPercentage", ref fSGSTPercentage, value);

                //CalculateNetAmount();
            }
        }

        decimal fSGSTAmount;
        [Appearance("IsGSTApplicableSGSTAmtCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Company.IsGSTApplicable", Context = "DetailView")]
        public decimal SGSTAmount
        {
            get
            {
                //fSGSTAmount = Math.Round(((GrossAmount / 100) * fSGSTPercentage), 2);
                return fSGSTAmount;
            }
            set { SetPropertyValue<decimal>("SGSTAmount", ref fSGSTAmount, value); }
        }

        decimal fCGSTPercentage;
        [Appearance("IsGSTApplicableCGSTPerCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Company.IsGSTApplicable", Context = "DetailView")]
        public decimal CGSTPercentage
        {
            get
            {
                return fCGSTPercentage;
            }
            set
            {
                SetPropertyValue<decimal>("CGSTPercentage", ref fCGSTPercentage, value);
                //CalculateNetAmount();
            }
        }

        decimal fCGSTAmount;
        [Appearance("IsGSTApplicableCGSTAmtCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Company.IsGSTApplicable", Context = "DetailView")]
        public decimal CGSTAmount
        {
            get
            {

                //fCGSTAmount = Math.Round(((GrossAmount / 100) * fCGSTPercentage), 2);
                //CalculateNetAmount();
                return fCGSTAmount;
            }
            set { SetPropertyValue<decimal>("CGSTAmount", ref fCGSTAmount, value); }
        }

        decimal fRoundOff;
        public decimal RoundOff
        {
            get { return fRoundOff; }
            set
            {
                SetPropertyValue<decimal>("RoundOff", ref fRoundOff, value);
                //CalculateNetAmount();
            }
        }

        decimal fNetAmount;
        [ReadOnly(true)]
        public decimal NetAmount
        {
            get
            {
                //if (!IsLoading && !IsSaving && fNetAmount == null)
                //{
                //    //CalculateNetAmount();
                //}
                return fNetAmount;
            }
            set { SetPropertyValue<decimal>("NetAmount", ref fNetAmount, value); }
        }

        string fAmountInWords;
        public string AmountInWords
        {
            get
            {
                if (!IsLoading && !IsSaving && fAmountInWords == null)
                    UpdateAmountInWords(false);
                return fAmountInWords;
            }
            set { SetPropertyValue<string>("AmountInWords", ref fAmountInWords, value); }
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

        Company fCompany;
        //[Browsable(false)]
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "NetAmount>=0", Context = "Any")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }
        private decimal CalculateFuelCharges()
        {
            if (FuelChargesMileage == 0)
                return 0;

            decimal nFuelCharges = Math.Round(((FuelChargesKm / FuelChargesMileage) * FuelChargesRatePerLitre), 2);
            return nFuelCharges;
        }

        private decimal CalculateExtraKmCharges()
        {
            decimal nExtraKmCharges = ExtraKm * ExtraKmRate;
            return nExtraKmCharges;
        }
        private decimal CalculateExtraHourCharges()
        {
            decimal nExtraHoursCharges = ExtraHours * ExtraHoursRate;
            return nExtraHoursCharges;
        }

        private decimal CalculateMobileCharges()
        {
            if (MobileMileage == 0)
                return 0;

            decimal nMobileCharges = Math.Round(((MobileKms / MobileMileage) * MobileRate), 2);
            return nMobileCharges;
        }
        private decimal CalculateNightCharges()
        {
            decimal nNightCharges = NightRate * NoOfNight;
            return nNightCharges;
        }

        private decimal CalculateGrossAmount()
        {
            decimal nGrossAmount = CarHiringCharges + FuelCharges + ExtraKmCharges + ExtraHoursCharges + MobileCharges + NightCharges ;
            return nGrossAmount;
        }

        private void CalculateNetAmount()
        {

            decimal _roundOff = Math.Round(GrossAmount + SGSTAmount + fCGSTAmount+ ParkngCharges) - (GrossAmount + SGSTAmount + CGSTAmount + ParkngCharges);
            
            decimal nNetAmount = GrossAmount + SGSTAmount + CGSTAmount + ParkngCharges + _roundOff;
            string tempAmountInWords = "";
            tempAmountInWords = NumberToWords.ConvertAmount(Convert.ToDouble(nNetAmount));
            AmountInWords = tempAmountInWords;
            RoundOff = _roundOff;
            NetAmount= nNetAmount;
        }

        private void CalculateTotal()
        {
            FuelCharges = CalculateFuelCharges();
            ExtraKmCharges = CalculateExtraKmCharges();
            ExtraHoursCharges = CalculateExtraHourCharges();
            MobileCharges = CalculateMobileCharges();
            NightCharges = CalculateNightCharges();
            GrossAmount = CalculateGrossAmount();
            CGSTAmount = Math.Round(((GrossAmount / 100) * fCGSTPercentage), 2);
            SGSTAmount = Math.Round(((GrossAmount / 100) * fSGSTPercentage), 2);
            CalculateNetAmount();
        }

        private decimal CalculatePreviousBalance()
        {
            decimal InvoiceBalance = (Session.Evaluate<Invoice>(CriteriaOperator.Parse("Sum(NetAmount)"), CriteriaOperator.Parse("Customer=?", this.Customer)) == DBNull.Value ? 0 : Convert.ToDecimal(Session.Evaluate<Invoice>(CriteriaOperator.Parse("Sum(NetAmount)"), CriteriaOperator.Parse("Customer=? and Oid!=?", this.Customer, this.Oid))));
            decimal ReceiptBalance = (Session.Evaluate<Receipt>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer)) == DBNull.Value ? 0 : Convert.ToDecimal(Session.Evaluate<Receipt>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer))));
            decimal DryInvoiceBalance = (Session.Evaluate<DryInvoice>(CriteriaOperator.Parse("Sum(NetAmount)"), CriteriaOperator.Parse("Customer=?", this.Customer)) == DBNull.Value ? 0 : Convert.ToDecimal(Session.Evaluate<DryInvoice>(CriteriaOperator.Parse("Sum(NetAmount)"), CriteriaOperator.Parse("Customer=?", this.Customer))));
            decimal TDSBalance = (Session.Evaluate<TDS>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer)) == DBNull.Value ? 0 : Convert.ToDecimal(Session.Evaluate<TDS>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer))));
            decimal DiscountBalance = (Session.Evaluate<Discount>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer)) == DBNull.Value ? 0 : Convert.ToDecimal(Session.Evaluate<Discount>(CriteriaOperator.Parse("Sum(Amount)"), CriteriaOperator.Parse("Customer=?", this.Customer))));
            decimal xprevBal = ReceiptBalance - InvoiceBalance - DryInvoiceBalance +TDSBalance + DiscountBalance;
            return xprevBal;
        }

        public void UpdateAmountInWords(bool forceChangeEvents)
        {
            string oldAmountInWords = fAmountInWords;
            string tempAmountInWords = "";
            tempAmountInWords = NumberToWords.ConvertAmount(Convert.ToDouble(fNetAmount));

            fAmountInWords = tempAmountInWords;
            if (forceChangeEvents)
                OnChanged(nameof(AmountInWords), oldAmountInWords, fAmountInWords);
        }
        protected override void OnSaving()
        {
            //CheckDate();
            int newnum = 1;
            DocumentNumbering docnos = Session.GetObjectByKey<DocumentNumbering>(this.DocSchemeOid);
            if (!(this.Session is NestedUnitOfWork) && Session.IsNewObject(this))
            {
                if (this.IsNew == true)
                {
                    newnum = docnos.CurrentNo + 1;
                    this.InvoiceNo = docnos.Prefix + new string('0', docnos.Body - (newnum).ToString().Length) + (newnum).ToString() + docnos.Suffix;
                }
            }
            base.OnSaving();

            if (!(this.Session is NestedUnitOfWork) && this.IsNew == true)
            {
                docnos.CurrentNo = newnum;
                docnos.Save();

                this.IsNew = false;
                this.Save();
            }
        }
        protected override void OnDeleting()
        {
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
                                throw new InvalidOperationException("The booking order cannot be deleted. Duty Slip No " + obj.ToString() + " have references to it.");
                            }
                            //if (((XPBaseCollection)mi.GetValue(this)).BaseIndexOf(obj) < 0)
                            //{
                            //    throw new InvalidOperationException("The booking order cannot be deleted. Duty Slip have references to it.");
                            //}
                        }
                    }
                }
            }
            base.OnDeleting();
        }


    }
}