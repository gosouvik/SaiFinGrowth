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

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class InvoiceRegister : XPLiteObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public InvoiceRegister(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private Guid fInvoiceId;
        [Key(AutoGenerate =false)]
        public Guid InvoiceId
        {
            get { return fInvoiceId; }
            set
            {
                SetPropertyValue(nameof(Guid), ref fInvoiceId, value);
            }
        }

        private int fSerialNo;
        public int SerialNo
        {
            get { return fSerialNo; }
            set
            {
                SetPropertyValue(nameof(Int32), ref fSerialNo, value);
            }
        }

        private String fInvoiceNo;
        public String InvoiceNo
        {
            get { return fInvoiceNo; }
            set
            {
                SetPropertyValue(nameof(String), ref fInvoiceNo, value);
            }
        }

        private DateTime fInvoiceDate;
        public DateTime InvoiceDate
        {
            get { return fInvoiceDate; }
            set
            {
                SetPropertyValue(nameof(DateTime), ref fInvoiceDate, value);
            }
        }

        private Customer fCustomerID;
        public Customer CustomerID
        {
            get { return fCustomerID; }
            set
            {
                SetPropertyValue(nameof(Customer), ref fCustomerID, value);
            }
        }

        private Decimal fTaxableAmount;
        public Decimal TaxableAmount
        {
            get { return fTaxableAmount; }
            set
            {
                SetPropertyValue(nameof(Decimal), ref fTaxableAmount, value);
            }
        }

        private Decimal fIGSTAmount;
        public Decimal IGSTAmount
        {
            get { return fIGSTAmount; }
            set
            {
                SetPropertyValue(nameof(Decimal), ref fIGSTAmount, value);
            }
        }

        private Decimal fCGSTAmount;
        public Decimal CGSTAmount
        {
            get { return fCGSTAmount; }
            set
            {
                SetPropertyValue(nameof(Decimal), ref fCGSTAmount, value);
            }
        }

        private Decimal fSGSTAmount;
        public Decimal SGSTAmount
        {
            get { return fSGSTAmount; }
            set
            {
                SetPropertyValue(nameof(Decimal), ref fSGSTAmount, value);
            }
        }

        private Decimal fNeTAmount;
        public Decimal NeTAmount
        {
            get { return fNeTAmount; }
            set
            {
                SetPropertyValue(nameof(Decimal), ref fNeTAmount, value);
            }
        }
    }

}
