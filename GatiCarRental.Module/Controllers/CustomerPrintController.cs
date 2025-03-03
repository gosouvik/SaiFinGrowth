using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Utils.CommonDialogs.Internal;
using DevExpress.XtraPivotGrid;
using GatiCarRental.Module.BusinessObjects;
using DialogResult = System.Windows.Forms.DialogResult;

namespace GatiCarRental.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class CustomerPrintController : ViewController
    {
        public CustomerPrintController()
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

        private void TDSRegister_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Customer record = (Customer)((DevExpress.ExpressApp.DetailView)this.ObjectSpace.Owner).CurrentObject;
            IObjectSpace objectSpace =
    ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(typeof(ReportDataV2));

            IReportDataV2 reportData =
                objectSpace.FindObject<ReportDataV2>(
                CriteriaOperator.Parse("[DisplayName] = 'TDS Register'"));
            string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
            //var __report = ReportDataProvider.ReportsStorage.LoadReport(reportData);
            //__report.FilterString = "[InvoiceNo]='" + record.InvoiceNo + "'";
            ReportServiceController controller = Frame.GetController<ReportServiceController>();


            string reportContainerHandle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
            CriteriaOperator criteria = new BinaryOperator("Customer", record.Oid); // Filter by Tags  
            //var dataSource = (DevExpress.Persistent.Base.ReportsV2.ISupportCriteria)__report.DataSource;
            //dataSource.Criteria = DevExpress.Data.Filtering.CriteriaOperator.Parse(
            //    "StartsWith(InvoiceNo, '" + record.InvoiceNo + "')");

            if (controller != null)
            {
                controller.ShowPreview(handle, criteria);
            }
        }

        private void ReceiptRegister_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

        }

        private void DiscountRegister_Execute_1(object sender, SimpleActionExecuteEventArgs e)
        {
            Customer record = (Customer)((DevExpress.ExpressApp.DetailView)this.ObjectSpace.Owner).CurrentObject;
            IObjectSpace objectSpace =
    ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(typeof(ReportDataV2));

            IReportDataV2 reportData =
                objectSpace.FindObject<ReportDataV2>(
                CriteriaOperator.Parse("[DisplayName] = 'Discount Register'"));
            string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
            //var __report = ReportDataProvider.ReportsStorage.LoadReport(reportData);
            //__report.FilterString = "[InvoiceNo]='" + record.InvoiceNo + "'";
            ReportServiceController controller = Frame.GetController<ReportServiceController>();


            string reportContainerHandle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
            CriteriaOperator criteria = new BinaryOperator("Customer", record.Oid); // Filter by Tags  
            //var dataSource = (DevExpress.Persistent.Base.ReportsV2.ISupportCriteria)__report.DataSource;
            //dataSource.Criteria = DevExpress.Data.Filtering.CriteriaOperator.Parse(
            //    "StartsWith(InvoiceNo, '" + record.InvoiceNo + "')");

            if (controller != null)
            {
                controller.ShowPreview(handle, criteria);
            }

        }

        private void CustomerPrintController_Activated(object sender, EventArgs e)
        {

        }

        private void ReceiptRegister_Execute_1(object sender, SimpleActionExecuteEventArgs e)
        {
            Customer record = (Customer)((DevExpress.ExpressApp.DetailView)this.ObjectSpace.Owner).CurrentObject;
            IObjectSpace objectSpace =
    ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(typeof(ReportDataV2));

            IReportDataV2 reportData =
                objectSpace.FindObject<ReportDataV2>(
                CriteriaOperator.Parse("[DisplayName] = 'Receipt Register'"));
            string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
            //var __report = ReportDataProvider.ReportsStorage.LoadReport(reportData);
            //__report.FilterString = "[InvoiceNo]='" + record.InvoiceNo + "'";
            ReportServiceController controller = Frame.GetController<ReportServiceController>();


            string reportContainerHandle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
            CriteriaOperator criteria = new BinaryOperator("Customer", record.Oid); // Filter by Tags  
            //var dataSource = (DevExpress.Persistent.Base.ReportsV2.ISupportCriteria)__report.DataSource;
            //dataSource.Criteria = DevExpress.Data.Filtering.CriteriaOperator.Parse(
            //    "StartsWith(InvoiceNo, '" + record.InvoiceNo + "')");

            if (controller != null)
            {
                controller.ShowPreview(handle, criteria);
            }
        }

        private void InvoiceRegister_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Customer record = (Customer)((DevExpress.ExpressApp.DetailView)this.ObjectSpace.Owner).CurrentObject;
            IObjectSpace objectSpace =
    ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(typeof(ReportDataV2));

            IReportDataV2 reportData =
                objectSpace.FindObject<ReportDataV2>(
                CriteriaOperator.Parse("[DisplayName] = 'GST Register'"));
            string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
            ReportServiceController controller = Frame.GetController<ReportServiceController>();


            string reportContainerHandle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
            CriteriaOperator criteria = new BinaryOperator("Name", record.Name); // Filter by Tags  

            if (controller != null)
            {
                controller.ShowPreview(handle, criteria);
            }
        }

        private void ImportCustomer_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Excel Files|*.xlsx";



            //if (ofd.ShowDialog() == DialogResult.OK)
            //{
            //    string filename = ofd.FileName;


            //    //ÏÂÃæ½âÎöExcelÎÄ¼þµÄ·½·¨£¬Çë¸ù¾ÝÄúµÄÐèÒª×ÔÐÐ±àÐ´£¬ÒÔÏÂ·½·¨½ö¹©²Î¿¼
            //    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();



            //    Microsoft.Office.Interop.Excel.Workbook workbook;

            //    Microsoft.Office.Interop.Excel.Worksheet worksheet;



            //    object oMissing = System.Reflection.Missing.Value;//Ïàµ±null



            //    workbook = excel.Workbooks.Open(filename, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);



            //    worksheet = (Worksheet)workbook.Worksheets[1];



            //    int rowCount = worksheet.UsedRange.Rows.Count;

            //    int colCount = worksheet.UsedRange.Columns.Count;

            //    for (int j = 2; j <= rowCount; j++)

            //    {
            //        //´´½¨¶ÔÏó£¬²¢Ìá½»µ½Êý¾Ý¿â
            //        Customer newEmp = ObjectSpace.CreateObject<Customer>();

            //        newEmp.Name = worksheet.Cells[j, 1].Value2.ToString();
            //        newEmp.Address1 = worksheet.Cells[j, 2].Value2.ToString();
            //        newEmp.Email = Convert.ToInt32(worksheet.Cells[j, 3].Value2.ToString());
            //        //how could I get the data of the  "Photo" column and set it to the Emp object ?


            //        newEmp.Save();
            //    }
            //    ObjectSpace.CommitChanges();
            //    View.ObjectSpace.Refresh();
            //}
        }
    }
}
