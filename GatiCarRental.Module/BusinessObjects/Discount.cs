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
using DevExpress.ExpressApp.Editors;
using GatiCarRental.Module.Controllers;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty("DocumentNo")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Discount : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Discount(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _DiscountID = (Session.Evaluate<Discount>(CriteriaOperator.Parse("Max(DiscountID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Discount>(CriteriaOperator.Parse("Max(DiscountID)"), CriteriaOperator.Parse("")))) + 1;
            //BookingOrder bookOrdObj = Session.FindObject<BookingOrder>(CriteriaOperator.Parse("BookingOrderID=[<BookingOrder>].Max(BookingOrderID)"));
            //if (bookOrdObj != null)
            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }

            //DocumentNo = "DC-" + (_DiscountID).ToString();
            GetDocumentNumbering();
            DocumentDate = DateTime.Today;
        }

        private void GetDocumentNumbering()
        {

            XPCollection<DocumentNumbering> docnos = new XPCollection<DocumentNumbering>(Session);
            docnos.Criteria = CriteriaOperator.Parse("[DocumentType]=?", DocumentType.Discount);
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

        [Persistent("DiscountID")] // this line for read-only columns mapping
        private int _DiscountID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_DiscountID")] // This line for read-only column mapping
        public int DiscountID
        {
            get { return _DiscountID; }
        }

        string fDocumentNo;
        [RuleUniqueValue]
        [Size(50)]
        public string DocumentNo
        {
            get { return fDocumentNo; }
            set { SetPropertyValue<string>("DocumentNo", ref fDocumentNo, value); }
        }
        DateTime fDocumentDate;
        public DateTime DocumentDate
        {
            get { return fDocumentDate; }
            set { SetPropertyValue<DateTime>("DocumentDate", ref fDocumentDate, value); }
        }

        private Customer fCustomer;
        [Association]
        public Customer Customer
        {
            get { return fCustomer; }
            set
            {
                SetPropertyValue(nameof(Customer), ref fCustomer, value);
            }
        }

        decimal fAmount;
        public decimal Amount
        {
            get { return fAmount; }
            set { SetPropertyValue<decimal>("Amount", ref fAmount, value); }
        }

        string fAmountInWords;
        [ReadOnly(true)]
        public string AmountInWords
        {
            get
            {
                if (!IsLoading && !IsSaving)
                    UpdateAmountInWords(false);
                return fAmountInWords;
            }
            set { SetPropertyValue<string>("AmountInWords", ref fAmountInWords, value); }
        }

        string fRemarks;
        [Size(500)]
        public string Remarks
        {
            get
            {
                return fRemarks;
            }
            set { SetPropertyValue<string>("Remarks", ref fRemarks, value); }
        }

        Company fCompany;
        //[Browsable(false)]
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "Amount>=0", Context = "Any")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }
        public void UpdateAmountInWords(bool forceChangeEvents)
        {
            string oldAmountInWords = fAmountInWords;
            string tempAmountInWords = "";
            tempAmountInWords = NumberToWords.ConvertAmount(Convert.ToDouble(Amount));

            fAmountInWords = tempAmountInWords;
            if (forceChangeEvents)
                OnChanged(nameof(AmountInWords), oldAmountInWords, fAmountInWords);
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
                    this.DocumentNo = docnos.Prefix + new string('0', docnos.Body - (newnum).ToString().Length) + (newnum).ToString() + docnos.Suffix;
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
    }
}