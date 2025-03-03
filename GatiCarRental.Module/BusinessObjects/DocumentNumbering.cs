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
    public class DocumentNumbering : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public DocumentNumbering(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);

            DocumentType = DocumentType.CallLogEmployee;
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private DocumentType fDocumentType;
        ////[RuleRequiredField]
        public DocumentType DocumentType
        {
            get { return fDocumentType; }
            set
            {
                SetPropertyValue(nameof(DocumentType), ref fDocumentType, value);
            }
        }

        private string fPrefix;
        public string Prefix
        {
            get { return fPrefix; }
            set
            {
                SetPropertyValue(nameof(String), ref fPrefix, value);
            }
        }

        private string fSuffix;
        public string Suffix
        {
            get { return fSuffix; }
            set
            {
                SetPropertyValue(nameof(String), ref fSuffix, value);
            }
        }

        private int fBody;
        public int Body
        {
            get { return fBody; }
            set
            {
                SetPropertyValue(nameof(Int32), ref fBody, value);
            }
        }

        private int fCurrentNo;
        public int CurrentNo
        {
            get { return fCurrentNo; }
            set
            {
                SetPropertyValue(nameof(Int32), ref fCurrentNo, value);
            }
        }

        private DateTime fStartDate;
        public DateTime StartDate
        {
            get { return fStartDate; }
            set
            {
                SetPropertyValue(nameof(DateTime), ref fStartDate, value);
            }
        }

        private DateTime fEndDate;
        public DateTime EndDate
        {
            get { return fEndDate; }
            set
            {
                SetPropertyValue(nameof(DateTime), ref fEndDate, value);
            }
        }

        Company fCompany;
        [Browsable(false)]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }
    }

    public enum DocumentType
    {
        CallLogLead = 10,
        CallLogCustomer=15,
        CallLogEmployee = 20,
        BookingOrder = 1,
        DutySlip = 2,
        Invoice = 3,
        MoneyReceipt = 4,
        Discount = 5,
        TDS = 6,
        Task = 25,
        MoneyMediclaimReceipt = 30,
        LICApplication = 35,
        Mediclaim=40 ,     
        Loan=45  

    }


}