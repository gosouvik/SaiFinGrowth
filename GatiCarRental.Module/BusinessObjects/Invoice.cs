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
    [DefaultProperty("InvoiceNo")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Invoice : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Invoice(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            int MaxDryInvoiceID = 0;
            _InvoiceID = (Session.Evaluate<Invoice>(CriteriaOperator.Parse("Max(InvoiceID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Invoice>(CriteriaOperator.Parse("Max(InvoiceID)"), CriteriaOperator.Parse("")))) + 1;
            MaxDryInvoiceID = (Session.Evaluate<DryInvoice>(CriteriaOperator.Parse("Max(InvoiceID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<DryInvoice>(CriteriaOperator.Parse("Max(InvoiceID)"), CriteriaOperator.Parse("")))) + 1;
            if (MaxDryInvoiceID > _InvoiceID)
                _InvoiceID = MaxDryInvoiceID;

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }

            //709 last invoice
            //InvoiceNo = "G/" + (_InvoiceID).ToString().PadLeft(6,'0') + "/21-22";
            GetDocumentNumbering();

            InvoiceDate = DateTime.Today;
            if (CarUsedAt == null)
                CarUsedAt = "Kolkata";
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
        public Customer Customer
        {
            get { return fCustomer; }
            set
            {
                SetPropertyValue(nameof(Customer), ref fCustomer, value);
                // Refresh the Accessory Property data source
                ClearInvoiceDetail();
                PreviousBalance = CalculatePreviousBalance();
            }
        }
        decimal fGrossAmount;
        [ReadOnly(true)]
        public decimal GrossAmount
        {
            get
            {
                if (!IsLoading && !IsSaving && fGrossAmount == null)
                    UpdateGrossAmount(false);
                return fGrossAmount;
            }
            set { SetPropertyValue<decimal>("GrossAmount", ref fGrossAmount, value); }
        }

        decimal fNightCharge;
        [ReadOnly(true)]
        public decimal NightCharge
        {
            get
            {
                if (!IsLoading && !IsSaving && fNightCharge == null)
                    UpdateNightCharge(false);
                return fNightCharge;
            }
            set { SetPropertyValue<decimal>("NightCharge", ref fNightCharge, value); }
        }

        decimal fDecorationCharge;
        [ReadOnly(true)]
        public decimal DecorationCharge
        {
            get
            {
                if (!IsLoading && !IsSaving && fDecorationCharge == null)
                    UpdateDecorationCharge(false);
                return fDecorationCharge;
            }
            set { SetPropertyValue<decimal>("DecorationCharge", ref fDecorationCharge, value); }
        }

        decimal fTaxableAmount;
        [ReadOnly(true)]
        [Appearance("IsGSTApplicableTaxableAmtCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Company.IsGSTApplicable", Context = "Any")]
        public decimal TaxableAmount
        {
            get
            {
                if (!IsLoading && !IsSaving && fTaxableAmount == null)
                    UpdateTaxableAmount(false);
                return fTaxableAmount;
            }
            set { SetPropertyValue<decimal>("TaxableAmount", ref fTaxableAmount, value); }
        }

        decimal fTaxAmount;
        [ReadOnly(true)]
        [Appearance("IsGSTApplicableTaxAmtCondDV", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Company.IsGSTApplicable", Context = "Any")]
        //[Appearance("IsGSTApplicableTaxAmtCondLV", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Company.IsGSTApplicable", Context = "ListView", AppearanceItemType = "ViewItem")]
        public decimal TaxAmount
        {
            get
            {
                if (!IsLoading && !IsSaving && fTaxableAmount == null)
                    UpdateTaxAmount(false);
                return fTaxAmount;
            }
            set { SetPropertyValue<decimal>("TaxAmount", ref fTaxAmount, value); }
        }


        decimal fParkngCharges;
        public decimal ParkngCharges
        {
            get
            {
                if (!IsLoading && !IsSaving && fParkngCharges == null)
                    UpdateParkngCharges(false);
                return fParkngCharges;
            }
            set { SetPropertyValue<decimal>("ParkngCharges", ref fParkngCharges, value); }
        }


        decimal fRoundOff;
        [ReadOnly(true)]
        public decimal RoundOff
        {
            get { return fRoundOff; }
            set
            {
                SetPropertyValue<decimal>("RoundOff", ref fRoundOff, value);
                CalculateNetAmount();
            }
        }

        decimal fNetAmount;
        [ReadOnly(true)]
        public decimal NetAmount
        {
            get
            {
                if (!IsLoading && !IsSaving && fNetAmount == null)
                    UpdateNetAmount(false);
                return fNetAmount;
            }
            set { SetPropertyValue<decimal>("NetAmount", ref fNetAmount, value); }
        }

        string fAmountInWords;
        [ReadOnly(true)]
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

        string fCarUsedAt;
        //[RuleRequiredField]
        public string CarUsedAt
        {
            get
            {

                return fCarUsedAt;
            }
            set { SetPropertyValue<string>("CarUsedAt", ref fCarUsedAt, value); }
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

        DocumentStatus fDocumentStatus;
        public DocumentStatus DocumentStatus
        {
            get { return fDocumentStatus; }
            set { SetPropertyValue<DocumentStatus>("DocumentStatus", ref fDocumentStatus, value); }
        }

        Company fCompany;
        //[Browsable(false)]
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "NetAmount>=0", Context = "Any")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        private void ClearInvoiceDetail()
        {

            //this.InvoiceDetails.
        }


        [DevExpress.Xpo.Aggregated, Association]
        //[RuleRequiredField]
        public XPCollection<InvoiceDetail> InvoiceDetails
        {
            get { return GetCollection<InvoiceDetail>(nameof(InvoiceDetails)); }
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
        public void UpdateGrossAmount(bool forceChangeEvents)
        {
            decimal? oldGrossAmount = fGrossAmount;
            decimal tempGrossAmount = 0m;
            foreach (InvoiceDetail detail in InvoiceDetails)
                tempGrossAmount += detail.GrossAmount;
            fGrossAmount = tempGrossAmount;
            if (forceChangeEvents)
                OnChanged(nameof(GrossAmount), oldGrossAmount, fGrossAmount);
        }

        public void UpdateNightCharge(bool forceChangeEvents)
        {
            decimal? oldNightCharge = fNightCharge;
            decimal tempNightCharge = 0m;
            foreach (InvoiceDetail detail in InvoiceDetails)
                tempNightCharge += detail.NightCharge;
            fNightCharge = tempNightCharge;
            if (forceChangeEvents)
                OnChanged(nameof(NightCharge), oldNightCharge, fNightCharge);
        }

        public void UpdateParkngCharges(bool forceChangeEvents)
        {
            decimal? oldParkngCharges = fParkngCharges;
            decimal tempParkngCharges = 0m;
            foreach (InvoiceDetail detail in InvoiceDetails)
                tempParkngCharges += detail.ParkngCharges;
            fParkngCharges = tempParkngCharges;
            if (forceChangeEvents)
                OnChanged(nameof(ParkngCharges), oldParkngCharges, fParkngCharges);
        }

        public void UpdateDecorationCharge(bool forceChangeEvents)
        {
            decimal? oldDecorationCharge = fDecorationCharge;
            decimal tempDecorationCharge = 0m;
            foreach (InvoiceDetail detail in InvoiceDetails)
                tempDecorationCharge += detail.DecorationCharges;
            fDecorationCharge = tempDecorationCharge;
            if (forceChangeEvents)
                OnChanged(nameof(DecorationCharge), oldDecorationCharge, fDecorationCharge);
        }

        public void UpdateTaxableAmount(bool forceChangeEvents)
        {
            decimal? oldTaxableAmount = fTaxableAmount;
            decimal tempTaxableAmount = 0m;
            foreach (InvoiceDetail detail in InvoiceDetails)
                tempTaxableAmount += detail.TaxableAmount;
            fTaxableAmount = tempTaxableAmount;
            if (forceChangeEvents)
                OnChanged(nameof(TaxableAmount), oldTaxableAmount, fTaxableAmount);
        }

        public void UpdateTaxAmount(bool forceChangeEvents)
        {
            decimal? oldTaxAmount = fTaxAmount;
            decimal tempTaxAmount = 0m;
            foreach (InvoiceDetail detail in InvoiceDetails)
                tempTaxAmount += detail.SGSTAmount + detail.CGSTAmount + detail.IGSTAmount;
            fTaxAmount = tempTaxAmount;
            if (forceChangeEvents)
                OnChanged(nameof(TaxAmount), oldTaxAmount, fTaxAmount);
        }

        public void UpdateNetAmount(bool forceChangeEvents)
        {
            decimal? oldNetAmount = fNetAmount;
            decimal tempNetAmount = 0m;
            foreach (InvoiceDetail detail in InvoiceDetails)
                tempNetAmount += detail.TotalAmount;//+ RoundOff;
            fNetAmount = tempNetAmount;
            // Calculate Round Off
            decimal _roundOff = Math.Round(fNetAmount) - fNetAmount;
            RoundOff = _roundOff;
            if (forceChangeEvents)
                OnChanged(nameof(NetAmount), oldNetAmount, fNetAmount);
        }

        public void UpdateAmountInWords(bool forceChangeEvents)
        {
            string oldAmountInWords = fAmountInWords;
            string tempAmountInWords = "";
            tempAmountInWords = NumberToWords.ConvertAmount(Convert.ToDouble(NetAmount));

            fAmountInWords = tempAmountInWords;
            if (forceChangeEvents)
                OnChanged(nameof(AmountInWords), oldAmountInWords, fAmountInWords);
        }
        protected void CalculateNetAmount()
        {
            NetAmount = TaxableAmount + TaxAmount + ParkngCharges + RoundOff;
        }

       
        protected override void OnSaving()
        {
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
            if (!IsDeleted)
            {
                if (!(this.Session is NestedUnitOfWork) && Session.IsNewObject(this))
                {
                    foreach (InvoiceDetail detail in InvoiceDetails)
                    {
                        detail.DutySlip.Picked = true;
                        detail.Save();
                    }


                }
            }
            else
            {

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
                    if ((mi.IsAssociation) && (mi.IsAggregated == false))
                    {
                        foreach (IXPObject obj in objs)
                        {
                            if (obj != null)
                            {

                                throw new InvalidOperationException("The Invoice cannot be deleted. Duty Slip No " + obj.ToString() + " have references to it.");
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

            if (!IsDeleted)
            {
                foreach (InvoiceDetail invd in this.InvoiceDetails)
                {
                    invd.DutySlip.Picked = false;
                    invd.DutySlip.Save();
                }
            }
        }
    }

    public enum DocumentStatus
    {
        Draft = 0,
        Approved = 1,
        Declined = 2,
        Cancelled = 3
    }
    class NumberToWords
    {
        private static String[] units = { "Zero", "One", "Two", "Three",
    "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",
    "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
    "Seventeen", "Eighteen", "Nineteen" };
        private static String[] tens = { "", "", "Twenty", "Thirty", "Forty",
    "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        public static String ConvertAmount(double amount)
        {
            try
            {
                Int64 amount_int = (Int64)amount;
                Int64 amount_dec = (Int64)Math.Round((amount - (double)(amount_int)) * 100);
                if (amount_dec == 0)
                {
                    return "Rupees " + Convert(amount_int) + " Only.";
                }
                else
                {
                    return "Rupees " + Convert(amount_int) + " and " + Convert(amount_dec) + " Paisa Only.";
                }
            }
            catch (Exception e)
            {
                // TODO: handle exception  
            }
            return "";
        }

        public static String Convert(Int64 i)
        {
            if (i < 20)
            {
                return units[i];
            }
            if (i < 100)
            {
                return tens[i / 10] + ((i % 10 > 0) ? " " + Convert(i % 10) : "");
            }
            if (i < 1000)
            {
                return units[i / 100] + " Hundred"
                        + ((i % 100 > 0) ? " And " + Convert(i % 100) : "");
            }
            if (i < 100000)
            {
                return Convert(i / 1000) + " Thousand "
                + ((i % 1000 > 0) ? " " + Convert(i % 1000) : "");
            }
            if (i < 10000000)
            {
                return Convert(i / 100000) + " Lakh "
                        + ((i % 100000 > 0) ? " " + Convert(i % 100000) : "");
            }
            if (i < 1000000000)
            {
                return Convert(i / 10000000) + " Crore "
                        + ((i % 10000000 > 0) ? " " + Convert(i % 10000000) : "");
            }
            return Convert(i / 1000000000) + " Arab "
                    + ((i % 1000000000 > 0) ? " " + Convert(i % 1000000000) : "");
        }


    }
}