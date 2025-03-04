﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace GatiCarRental.Module.Reports
{
    public partial class XRCustomerStatement : DevExpress.XtraReports.UI.XtraReport
    {
        public XRCustomerStatement()
        {
            InitializeComponent();
        }

        private void xrLabel38_BeforePrint(object sender, CancelEventArgs e)
        {
            //DevExpress.ExpressApp.IObjectSpace objectSpace = DevExpress.Persistent.Base.ReportsV2.DataSourceBase.CreateObjectSpace(type, (DevExpress.XtraReports.UI.XtraReport)sender);
            if (GetCurrentColumnValue("Debit") != null)
            {
                if (GetCurrentColumnValue("Debit") != System.DBNull.Value)
                {
                    if (Convert.ToDecimal(GetCurrentColumnValue("Debit")) == 0)
                    {
                        xrLabel38.Text = "";
                    }
                }
            }
        }

        
        private void xrLabel39_BeforePrint_1(object sender, CancelEventArgs e)
        {
            if (GetCurrentColumnValue("Credit") != null)
            {
                if (GetCurrentColumnValue("Credit") != System.DBNull.Value)
                {
                    if (Convert.ToDecimal(GetCurrentColumnValue("Credit")) == 0)
                    {
                        xrLabel39.Text = "";
                    }
                }
            }
        }

    }
}
