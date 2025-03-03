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

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly,D:\Projects\GatiCarRental_110921\GatiCarRental\GatiCarRental.Module\BusinessObjects\InvoiceDetail.cs false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class InvoiceDetail : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public InvoiceDetail(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }

            if (Company.IsGSTApplicable == true)
            {
                fCGSTRate = Convert.ToDecimal("6.00");
                fSGSTRate = Convert.ToDecimal("6.00");
                fIGSTRate = Convert.ToDecimal("0.00");
            }
            else
            {
                fCGSTRate = Convert.ToDecimal("0.00");
                fSGSTRate = Convert.ToDecimal("0.00");
                fIGSTRate = Convert.ToDecimal("0.00");
            }
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private DutySlip fDutySlip;
        [Association]
        [RuleUniqueValue("RuleCollectionValidation", DefaultContexts.Save)]
        //[DataSourceCriteria("Customer.Oid == '@This.Invoice.Customer.Oid' && DutyDate<='@This.Invoice.InvoiceDate' && Picked=0 && Oid In('@This.DutySlip')")]
        [DataSourceProperty(nameof(AvailableDutySlips))]
        public DutySlip DutySlip
        {
            get { return fDutySlip; }
            set
            {
                SetPropertyValue<DutySlip>("DutySlip", ref fDutySlip, value);
                FetchDutySlip();
            }
        }

        private XPCollection<DutySlip> fAvailableDutySlips;
        [Browsable(false)] // Prohibits showing the AvailableAccessories collection separately
        public XPCollection<DutySlip> AvailableDutySlips
        {
            get
            {
                if (fAvailableDutySlips == null)
                {
                    // Retrieve all Accessory objects

                    fAvailableDutySlips = new XPCollection<DutySlip>(Session);
                    RefreshDutySlips();
                }
                return fAvailableDutySlips;
            }
        }

        private void RefreshDutySlips()
        {
            if (fAvailableDutySlips == null)
                return;
            if (this.Invoice.Customer == null)
            {
                fAvailableDutySlips.Criteria = CriteriaOperator.Parse("1=0");
                return;
            }
            fAvailableDutySlips.Criteria = CriteriaOperator.Parse("Picked=0 && Customer=? && DutyDate<='" + this.Invoice.InvoiceDate.ToString("yyyyMMdd") + "'", this.Invoice.Customer);

            BindingList<DutySlip> existsDutySlips = new BindingList<DutySlip>();
            foreach (InvoiceDetail InvD in this.Invoice.InvoiceDetails)
            {
                existsDutySlips.Add(InvD.DutySlip);
            }

            BindingList<DutySlip> deleteDutySlips = new BindingList<DutySlip>();
            foreach (DutySlip Ds in fAvailableDutySlips)
            {
                if (existsDutySlips.Contains(Ds))
                    deleteDutySlips.Add(Ds);
               
            }

            foreach(DutySlip Ds in deleteDutySlips)
            {
                fAvailableDutySlips.Remove(Ds);
            }
            
            DutySlip = null;
        }

        decimal fGrossAmount;
        public decimal GrossAmount
        {
            get { return fGrossAmount; }
            set
            {
                bool modified = SetPropertyValue(nameof(GrossAmount), ref fGrossAmount, value);
                if (!IsLoading && !IsSaving && Invoice != null && modified)
                {
                    Invoice.UpdateGrossAmount(true);
                    CalculateTax();
                }

            }
        }

        decimal fNightCharge;
        public decimal NightCharge
        {
            get { return fNightCharge; }
            set
            {
                bool modified = SetPropertyValue(nameof(NightCharge), ref fNightCharge, value);
                if (!IsLoading && !IsSaving && Invoice != null && modified)
                {
                    CalculateTax();
                    Invoice.UpdateNightCharge(true);
                }
            }
        }

        decimal fTaxableAmount;
        [Appearance("IsGSTApplicableTaxableAmtCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Invoice.Company.IsGSTApplicable", Context = "Any")]
        public decimal TaxableAmount
        {
            get { return fTaxableAmount; }
            set
            {
                bool modified = SetPropertyValue(nameof(TaxableAmount), ref fTaxableAmount, value);
                if (!IsLoading && !IsSaving && Invoice != null && modified)
                {
                    Invoice.UpdateTaxableAmount(true);
                }
            }
        }

        decimal fIGSTRate;
        decimal fIGSTAmount;
        [Appearance("IsGSTApplicableIGSTAmtCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Invoice.Company.IsGSTApplicable", Context = "Any")]
        public decimal IGSTAmount
        {
            get { return fIGSTAmount; }
            set
            {
                bool modified = SetPropertyValue(nameof(IGSTAmount), ref fIGSTAmount, value);
                if (!IsLoading && !IsSaving && Invoice != null && modified)
                {
                    Invoice.UpdateTaxAmount(true);
                }
            }
        }

        decimal fSGSTRate;
        decimal fSGSTAmount;
        [Appearance("IsGSTApplicableSGSTAmtCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Invoice.Company.IsGSTApplicable", Context = "Any")]
        public decimal SGSTAmount
        {
            get { return fSGSTAmount; }
            set
            {
                bool modified = SetPropertyValue(nameof(SGSTAmount), ref fSGSTAmount, value);
                if (!IsLoading && !IsSaving && Invoice != null && modified)
                {
                    Invoice.UpdateTaxAmount(true);
                }
            }
        }

        decimal fCGSTRate;
        decimal fCGSTAmount;
        [Appearance("IsGSTApplicableCGSTAmtCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "!Invoice.Company.IsGSTApplicable", Context = "Any")]
        public decimal CGSTAmount
        {
            get { return fCGSTAmount; }
            set
            {
                bool modified = SetPropertyValue(nameof(CGSTAmount), ref fCGSTAmount, value);
                if (!IsLoading && !IsSaving && Invoice != null && modified)
                {
                    Invoice.UpdateTaxAmount(true);
                }
            }
        }

        decimal fParkngCharges;
        public decimal ParkngCharges
        {
            get { return fParkngCharges; }
            set
            {
                bool modified = SetPropertyValue(nameof(ParkngCharges), ref fParkngCharges, value);
                if (!IsLoading && !IsSaving && Invoice != null && modified)
                {
                    Invoice.UpdateParkngCharges(true);
                }
            }
        }

        decimal fDecorationCharge;
        public decimal DecorationCharges
        {
            get { return fDecorationCharge; }
            set
            {
                bool modified = SetPropertyValue(nameof(DecorationCharges), ref fDecorationCharge, value);
                if (!IsLoading && !IsSaving && Invoice != null && modified)
                {
                    CalculateTax();
                    Invoice.UpdateDecorationCharge(true);
                }
            }
        }

        decimal fTotalAmount;
        public decimal TotalAmount
        {
            get { return fTotalAmount; }
            set
            {
                bool modified = SetPropertyValue(nameof(TotalAmount), ref fTotalAmount, value);
                if (!IsLoading && !IsSaving && Invoice != null && modified)
                {
                    Invoice.UpdateNetAmount(true);
                    Invoice.UpdateAmountInWords(true);
                }
            }
        }

        Company fCompany;
        [Browsable(false)]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        private Invoice _Invoice;
        [Association]
        public Invoice Invoice
        {
            get { return _Invoice; }
            set { SetPropertyValue<Invoice>(nameof(Invoice), ref _Invoice, value); }
        }

        private void FetchDutySlip()
        {
            try
            {
                if (DutySlip != null)
                {
                    if (DutySlip.DocumentNo != null)
                    {
                        if (DutySlip.DocumentNo.ToString().Trim() != "")
                        {
                            XPCollection<DutySlip> getRecords = new XPCollection<DutySlip>(Session);
                            getRecords.Criteria = CriteriaOperator.Parse("DocumentNo='" + DutySlip.DocumentNo.ToString() + "'");
                            foreach (var item in getRecords)
                            {
                                GrossAmount = item.GrossAmount;
                                NightCharge = item.NightCharge;
                                ParkngCharges = item.ParkngCharges;
                                DecorationCharges = item.DecorationCharges;
                            }
                            CalculateTax();
                        }
                    }
                }
            }
            catch { }
        }

        private void CalculateTax()
        {
            try
            {
                TaxableAmount = GrossAmount + NightCharge + DecorationCharges;
                if (Company.IsGSTApplicable == true)
                {
                    //if (this.Invoice.Customer.GSTRegNo != null)
                    //{
                    //    if (this.Invoice.Customer.GSTRegNo.Substring(0, 2) == "19" || this.Invoice.Customer.GSTRegNo == "Unregistered")
                    //    {
                    //        SGSTAmount = Math.Round(((TaxableAmount / 100) * fSGSTRate), 2);
                    //        CGSTAmount = Math.Round(((TaxableAmount / 100) * fSGSTRate), 2);
                    //    }
                    //    else
                    //    {
                    //        fIGSTRate = 12;
                    //        IGSTAmount = Math.Round(((TaxableAmount / 100) * fIGSTRate), 2);
                    //    }
                    //}
                    //else
                    //{
                    //    SGSTAmount = Math.Round(((TaxableAmount / 100) * fSGSTRate), 2);
                    //    CGSTAmount = Math.Round(((TaxableAmount / 100) * fSGSTRate), 2);
                    //}
                }
                else
                {
                    SGSTAmount = 0;
                    CGSTAmount = 0;
                    IGSTAmount = 0;
                }
                TotalAmount = TaxableAmount + SGSTAmount + CGSTAmount + IGSTAmount + ParkngCharges;
            }
            catch { }
        }
    }
}