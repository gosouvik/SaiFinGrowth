using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.Xpo;
using GatiCarRental.Module.BusinessObjects;
using DevExpress.Data.Filtering;

namespace GatiCarRental.Module.Reports
{
    public partial class XRCustomerBalance : DevExpress.XtraReports.UI.XtraReport
    {
        public XRCustomerBalance()
        {
            InitializeComponent();

            //DevExpress.ExpressApp.IObjectSpace objectSpace = DevExpress.Persistent.Base.ReportsV2.DataSourceBase.CreateObjectSpace(typeof(Company), (DevExpress.XtraReports.UI.XtraReport)this.Report);

            //Company comp = Session.DefaultSession.FindObject<Company>(new OperandProperty("Active") == 1) as Company;
            //if (comp.DBName == "Carrental")
            //{
            //    sqlDataSource2.ConnectionName = "ConnectionString";
            //}
            //else if (comp.DBName == "Carrental_NonGST")
            //{
            //    sqlDataSource2.ConnectionName = "ConnectionString_NonGST";
            //}

        }

       
    }
}
