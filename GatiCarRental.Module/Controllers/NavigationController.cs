using System;
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
using DevExpress.ExpressApp.ReportsV2;
using GatiCarRental.Module.Reports;
using DevExpress.Persistent.BaseImpl;
using GatiCarRental.Module.BusinessObjects;
using DevExpress.ExpressApp.Xpo;

namespace GatiCarRental.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class NavigationController : WindowController
    {
        private ShowNavigationItemController showNavigationItemController;
        public NavigationController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem += new EventHandler<CustomShowNavigationItemEventArgs>(this.Report_CustomShowNavigationItem);
        }

        /// <summary>
        /// Called when [deactivated].
        /// </summary>
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem -= new EventHandler<CustomShowNavigationItemEventArgs>(this.Report_CustomShowNavigationItem);
        }

        /// <summary>
        /// Handles the CustomShowNavigationItem event of the SystemConfiguration control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CustomShowNavigationItemEventArgs"/> instance containing the event data.</param>
        private void Report_CustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e)
        {
            if (e.ActionArguments.SelectedChoiceActionItem.Id == "CustomerStatement")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                ReportDataV2 report = objectSpace.FindObject<ReportDataV2>(CriteriaOperator.Parse("DisplayName = 'Customer Statement'"));

                if (report != null)
                {
                    string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(report);
                    Frame.GetController<ReportServiceController>().ShowPreview(handle);
                    e.Handled = true;
                }
                else
                {
                    throw new UserFriendlyException("The report could not be found, please contact your system administrator.");
                }
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "CustomerBalance")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                ReportDataV2 report = objectSpace.FindObject<ReportDataV2>(CriteriaOperator.Parse("DisplayName = 'Customer Balance'"));

                if (report != null)
                {
                    string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(report);
                    Frame.GetController<ReportServiceController>().ShowPreview(handle);
                    e.Handled = true;
                }
                else
                {
                    throw new UserFriendlyException("The report could not be found, please contact your system administrator.");
                }
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "TDSRegister")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                ReportDataV2 report = objectSpace.FindObject<ReportDataV2>(CriteriaOperator.Parse("DisplayName = 'TDS Register'"));

                if (report != null)
                {
                    string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(report);
                    Frame.GetController<ReportServiceController>().ShowPreview(handle);
                    e.Handled = true;
                }
                else
                {
                    throw new UserFriendlyException("The report could not be found, please contact your system administrator.");
                }
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "DiscountRegister")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                ReportDataV2 report = objectSpace.FindObject<ReportDataV2>(CriteriaOperator.Parse("DisplayName = 'Discount Register'"));

                if (report != null)
                {
                    string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(report);
                    Frame.GetController<ReportServiceController>().ShowPreview(handle);
                    e.Handled = true;
                }
                else
                {
                    throw new UserFriendlyException("The report could not be found, please contact your system administrator.");
                }
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "ReceiptRegister")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                ReportDataV2 report = objectSpace.FindObject<ReportDataV2>(CriteriaOperator.Parse("DisplayName = 'Receipt Register'"));

                if (report != null)
                {
                    string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(report);
                    Frame.GetController<ReportServiceController>().ShowPreview(handle);
                    e.Handled = true;
                }
                else
                {
                    throw new UserFriendlyException("The report could not be found, please contact your system administrator.");
                }
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "GSTRegister")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                ReportDataV2 report = objectSpace.FindObject<ReportDataV2>(CriteriaOperator.Parse("DisplayName = 'GST Register'"));

                if (report != null)
                {
                    string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(report);
                    Frame.GetController<ReportServiceController>().ShowPreview(handle);
                    e.Handled = true;
                }
                else
                {
                    throw new UserFriendlyException("The report could not be found, please contact your system administrator.");
                }
            }
        }
    }
}
