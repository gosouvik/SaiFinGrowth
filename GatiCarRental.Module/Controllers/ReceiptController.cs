﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using GatiCarRental.Module.BusinessObjects;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.ReportsV2;

namespace GatiCarRental.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ReceiptController : ViewController
    {
        public ReceiptController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void PrintReceipt_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Receipt record = (Receipt)((DevExpress.ExpressApp.DetailView)this.ObjectSpace.Owner).CurrentObject;
            IObjectSpace objectSpace =
    ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(typeof(ReportDataV2));

            IReportDataV2 reportData =
                objectSpace.FindObject<ReportDataV2>(
                CriteriaOperator.Parse("[DisplayName] = 'Money Receipt'"));
            string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
            //var __report = ReportDataProvider.ReportsStorage.LoadReport(reportData);
            //__report.FilterString = "[InvoiceNo]='" + record.InvoiceNo + "'";
            ReportServiceController controller = Frame.GetController<ReportServiceController>();


            string reportContainerHandle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
            CriteriaOperator criteria = new BinaryOperator("DocumentNo", record.DocumentNo); // Filter by Tags  
            //var dataSource = (DevExpress.Persistent.Base.ReportsV2.ISupportCriteria)__report.DataSource;
            //dataSource.Criteria = DevExpress.Data.Filtering.CriteriaOperator.Parse(
            //    "StartsWith(InvoiceNo, '" + record.InvoiceNo + "')");

            if (controller != null)
            {
                controller.ShowPreview(handle, criteria);
            }
        }
    }
}
