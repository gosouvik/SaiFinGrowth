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
    public class Company : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Company(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        string fName;
        [Size(50)]
        //[RuleRequiredField]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        string fAllCapName;
        [Size(50)]
        //[RuleRequiredField]
        public string AllCapName
        {
            get { return fAllCapName; }
            set { SetPropertyValue<string>("AllCapName", ref fAllCapName, value); }
        } 

        string fAddress1;
        [Size(500)]
        //[RuleRequiredField] // Validation for Required
        public string Address1
        {
            get { return fAddress1; }
            set { SetPropertyValue<string>("Address1", ref fAddress1, value); }
        }
        string fCity;
        [Size(50)]
        //[RuleRequiredField] // Validation for Required
        public string City
        {
            get { return fCity; }
            set { SetPropertyValue<string>("City", ref fCity, value); }
        }
        string fState;
        [Size(50)]
        //[RuleRequiredField] // Validation for Required
        public string State
        {
            get { return fState; }
            set { SetPropertyValue<string>("State", ref fState, value); }
        }
        string fCountry;
        [Size(50)]
        //[RuleRequiredField] // Validation for Required
        public string Country
        {
            get { return fCountry; }
            set { SetPropertyValue<string>("Country", ref fCountry, value); }
        }
        string fPinCode;
        [Size(10)]
        //[RuleRequiredField] // Validation for Required
        public string PinCode
        {
            get { return fPinCode; }
            set { SetPropertyValue<string>("PinCode", ref fPinCode, value); }
        }
        string fPhone1;
        [Size(20)]
        //[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        public string Phone1
        {
            get { return fPhone1; }
            set { SetPropertyValue<string>("Phone1", ref fPhone1, value); }
        }
        
        string fEmail;
        [Size(200)]
        public string Email
        {
            get { return fEmail; }
            set { SetPropertyValue<string>("Email", ref fEmail, value); }
        }

        bool fIsGSTApplicable;
        public bool IsGSTApplicable
        {
            get { return fIsGSTApplicable; }
            set { SetPropertyValue<bool>("IsGSTApplicable", ref fIsGSTApplicable, value); }
        }

        string fGSTRegNo;
        [Size(15)]
        //[RuleRegularExpression("", DefaultContexts.Save, "[A-Z]{5}[0-9]{4}[A-Z]{1}")]
        public string GSTRegNo
        {
            get
            {
                if (fGSTRegNo == null || fGSTRegNo == "")
                    fGSTRegNo = "Unregistered";
                return fGSTRegNo;
            }
            set { SetPropertyValue<string>("GSTRegNo", ref fGSTRegNo, value); }
        }
        string fDBName;
        [Size(20)]
        //[RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        public string DBName
        {
            get { return fDBName; }
            set { SetPropertyValue<string>("DBName", ref fDBName, value); }
        }

        bool fActive;
        public bool Active
        {
            get { return fActive; }
            set { SetPropertyValue<bool>("Active", ref fActive, value); }
        }

        [Association("Company-Employees")]
        public XPCollection<Employee> Employees
        {
            get { return GetCollection<Employee>(nameof(Employees)); }
        }
    }
}