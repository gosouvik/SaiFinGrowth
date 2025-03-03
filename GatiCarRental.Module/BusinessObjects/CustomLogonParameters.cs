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
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo.DB.Helpers;

namespace GatiCarRental.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public interface ICompanyNameParameter
    {
        string CompanyName { get; set; }
    }
    [DomainComponent]
    public class CustomLogonParametersForStandardAuthentication : AuthenticationStandardLogonParameters, ICompanyNameParameter
    {
        private string companyName = MSSqlServerChangeDatabaseHelper.Databases.Split(';')[0];
        [ModelDefault("PredefinedValues", MSSqlServerChangeDatabaseHelper.Databases)]
        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }
    }
    [DomainComponent]
    public class CustomLogonParametersForActiveDirectoryAuthentication : ICompanyNameParameter
    {
        private string companyName = MSSqlServerChangeDatabaseHelper.Databases.Split(';')[0];

        [ModelDefault("PredefinedValues", MSSqlServerChangeDatabaseHelper.Databases)]
        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }
    }

    public class MSSqlServerChangeDatabaseHelper
    {
        //public const string Databases = "CarRental;Carrental_NonGST";
        public const string Databases = "SaiFinGrwoth;Avinash";


        public static string PatchConnectionString(string databaseName, string connectionString)
        {
            ConnectionStringParser helper = new ConnectionStringParser(connectionString);
            helper.RemovePartByName("Initial Catalog");
            return string.Format("Initial Catalog={0};{1}", databaseName, helper.GetConnectionString());
        }
    }
}