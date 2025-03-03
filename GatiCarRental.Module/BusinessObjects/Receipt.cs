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
    public class Receipt : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Receipt(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _ReceiptID = (Session.Evaluate<Receipt>(CriteriaOperator.Parse("Max(ReceiptID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Receipt>(CriteriaOperator.Parse("Max(ReceiptID)"), CriteriaOperator.Parse("")))) + 1;
            XPCollection<Company> comps = new XPCollection<Company>(Session);
            comps.Criteria = CriteriaOperator.Parse("Active=?", true);
            foreach (Company comp in comps)
            {
                Company = comp;
            }
            //BookingOrder bookOrdObj = Session.FindObject<BookingOrder>(CriteriaOperator.Parse("BookingOrderID=[<BookingOrder>].Max(BookingOrderID)"));
            //if (bookOrdObj != null)
            //DocumentNo = "RC-" + (_ReceiptID).ToString();
            GetDocumentNumbering();
            DocumentDate = DateTime.Today;
        }

        private void GetDocumentNumbering()
        {

            XPCollection<DocumentNumbering> docnos = new XPCollection<DocumentNumbering>(Session);
            docnos.Criteria = CriteriaOperator.Parse("[DocumentType]=?", DocumentType.MoneyReceipt);
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

        [Persistent("ReceiptID")] // this line for read-only columns mapping
        private int _ReceiptID;
        ////[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]
        [PersistentAlias("_ReceiptID")] // This line for read-only column mapping
        public int ReceiptID
        {
            get { return _ReceiptID; }
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
        [ImmediatePostData]
        [Association]
        public Customer Customer
        {
            get { return fCustomer; }
            set
            {
                SetPropertyValue(nameof(Customer), ref fCustomer, value);
                RefreshAvailableLICPolicies();
            }
        }

        private XPCollection<Policy> fAvailableLICPolies;
        [Browsable(false)] // Prohibits showing the AvailableAccessories collection separately
        public XPCollection<Policy> AvailableLICPolies
        {
            get
            {
                if (fAvailableLICPolies == null)
                {
                    // Retrieve all Sample objects
                    fAvailableLICPolies = new XPCollection<Policy>(Session);
                    // Filter the retrieved collection according to current conditions
                    RefreshAvailableLICPolicies();
                }
                // Return the filtered collection of Sample objects
                return fAvailableLICPolies;
            }
        }

        private void RefreshAvailableLICPolicies()
        {
            if (fAvailableLICPolies == null)
                return;
            // Process the situation when the Party is not specified (see the Scenario 3 above)
            if (Customer == null)
            {
                // Show only Global Collection when the Party is not specified
                fAvailableLICPolies.Criteria = CriteriaOperator.Parse("1=0");
            }
            else
            {
                // Leave only the current Party's Collection in the fAvailableSampleCollection collection
                fAvailableLICPolies.Criteria = new BinaryOperator("Customer", Customer);

                //    // Add Global Collection
                //    XPCollection<SampleCollection> availableGlobalAccessories =
                //       new XPCollection<SampleCollection>(Session);
                //    availableGlobalAccessories.Criteria = CriteriaOperator.Parse("Party",Party);
                //fAvailableSampleCollection.AddRange(availableGlobalAccessories);
            }
            // Set null for the Collection property to allow an end-user 
            //to set a new value from the refreshed data source
            LicPolicies = null;
        }

        [DataSourceProperty(nameof(AvailableLICPolies))]
        [Association]
        [ImmediatePostData]
        //[Appearance("PickedCarNumberCond", Enabled = false, Criteria = "Picked", Context = "DetailView")]
        public Policy LicPolicies
        {
            get { return fLicPolicies; }
            set
            {
                SetPropertyValue(nameof(Policy), ref fLicPolicies, value);
                if (fLicPolicies != null)
                {
                    if (this.fLicPolicies.Customer != null)
                    {
                        this.Customer = this.fLicPolicies.Customer;
                    }
                }
            }
        }
        Policy fLicPolicies;


        private CashBank fCashBank;
        //[RuleRequiredField]
        

        public CashBank CashBank
        {
            get { return fCashBank; }
            set
            {
                SetPropertyValue(nameof(CashBank), ref fCashBank, value);
                if (fCashBank != null)
                {
                    IsCash = fCashBank.IsCash;
                    RefreshAvailableInstrumentTypes();
                }
            }
        }

        private XPCollection<InstrumentType> fAvailableInstrumentTypes;
        [Browsable(false)] // Prohibits showing the AvailableAccessories collection separately
        public XPCollection<InstrumentType> AvailableInstrumentTypes
        {
            get
            {
                if (fAvailableInstrumentTypes == null)
                {
                    // Retrieve all Sample objects
                    fAvailableInstrumentTypes = new XPCollection<InstrumentType>(Session);
                    // Filter the retrieved collection according to current conditions
                    RefreshAvailableInstrumentTypes();
                }
                // Return the filtered collection of Sample objects
                return fAvailableInstrumentTypes;
            }
        }
        private void RefreshAvailableInstrumentTypes()
        {
            if (fAvailableInstrumentTypes == null)
                return;
            // Process the situation when the Party is not specified (see the Scenario 3 above)
            if (CashBank == null)
            {
                // Show only Global Collection when the Party is not specified
                fAvailableInstrumentTypes.Criteria = CriteriaOperator.Parse("1=1");
            }
            else
            {
                // Leave only the current Party's Collection in the fAvailableSampleCollection collection
                fAvailableInstrumentTypes.Criteria = new ContainsOperator(nameof(InstrumentType.CashBanks), new BinaryOperator("This", CashBank));
                
                //    // Add Global Collection
                //    XPCollection<SampleCollection> availableGlobalAccessories =
                //       new XPCollection<SampleCollection>(Session);
                //    availableGlobalAccessories.Criteria = CriteriaOperator.Parse("Party",Party);
                //fAvailableSampleCollection.AddRange(availableGlobalAccessories);
            }
            // Set null for the Collection property to allow an end-user 
            //to set a new value from the refreshed data source
            InstrumentType = null;


        }

        bool fIsCash;
        [ImmediatePostData]
        [Browsable(false)]
        public bool IsCash
        {
            get { return fIsCash; }
            set { SetPropertyValue<bool>("IsCash", ref fIsCash, value); }
        }
        
        

        private InstrumentType fInstrumentType;
        //[RuleRequiredField]
        [DataSourceProperty(nameof(AvailableInstrumentTypes))]
        public InstrumentType InstrumentType
        {
            get { return fInstrumentType; }
            set
            {
                SetPropertyValue(nameof(InstrumentType), ref fInstrumentType, value);
            }
        }

        string fInstrumentNumber;
        [Size(100)]
        [Appearance("IsCashTrueInstrumentNumber", Visibility = ViewItemVisibility.Hide, Criteria = "IsCash", Context = "DetailView")]
        public string InstrumentNumber
        {
            get { return fInstrumentNumber; }
            set { SetPropertyValue<string>("InstrumentNumber", ref fInstrumentNumber, value); }
        }

        DateTime fInstrumentDate;
        [Appearance("IsCashTrueInstrumentDate", Visibility = ViewItemVisibility.Hide, Criteria = "IsCash", Context = "DetailView")]
        public DateTime InstrumentDate
        {
            get { return fInstrumentDate; }
            set { SetPropertyValue<DateTime>("InstrumentDate", ref fInstrumentDate, value); }
        }

        private CashBank fCompanyCashBank;
        //[RuleRequiredField]
        [DataSourceCriteria("IsDual == true")]

        public CashBank CompanyCashBank
        {
            get { return fCompanyCashBank; }
            set
            {
                SetPropertyValue(nameof(CompanyCashBank), ref fCompanyCashBank, value);
            }
        }


        decimal fAmount;
        [ImmediatePostData(true)]
        public decimal Amount
        {
            get { return fAmount; }
            set { SetPropertyValue<decimal>("Amount", ref fAmount, value);
                
            }
        }

        
        string fAmountInWords;
        [ReadOnly(true)]
        public string AmountInWords
        {
            get
            {
                if (!IsLoading && !IsSaving )
                        UpdateAmountInWords(false);
                    return fAmountInWords;
            }
            set { SetPropertyValue<string>("AmountInWords", ref fAmountInWords, value); }
        }

        Company fCompany;
        //[Browsable(false)]
        [Appearance("CompanyCond", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "Amount>=0", Context = "Any")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>("Company", ref fCompany, value); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        public XPCollection<ReceiptFileData> Attachments
        {
            get { return GetCollection<ReceiptFileData>(nameof(Attachments)); }
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
            //CheckDate();
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