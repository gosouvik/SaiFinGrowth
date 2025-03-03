using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.ReportsV2;
using GatiCarRental.Module.Reports;
using GatiCarRental.Module.BusinessObjects;
using DevExpress.ExpressApp.Notifications;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils.MVVM.Services;
using Task = GatiCarRental.Module.BusinessObjects.Task;
using DevExpress.ExpressApp.SystemModule;

namespace GatiCarRental.Module {
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
    public sealed partial class GatiCarRentalModule : ModuleBase {
        public GatiCarRentalModule() {
            InitializeComponent();
			BaseObject.OidInitializationMode = OidInitializationMode.AfterConstruction;
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Notifications.NotificationsModule));

        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);

            PredefinedReportsUpdater predefinedReportsUpdater =
                new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);
            predefinedReportsUpdater.AddPredefinedReport<XRInvoice>(
            "Invoice", typeof(InvoiceDetail));
            predefinedReportsUpdater.AddPredefinedReport<XRInvoice2>(
            "Invoice Email", typeof(InvoiceDetail));
            predefinedReportsUpdater.AddPredefinedReport<XRInvoiceNGST>(
            "Invoice NonGST", typeof(InvoiceDetail));
            predefinedReportsUpdater.AddPredefinedReport<XRDryInvoice>(
            "Dry Invoice", typeof(DryInvoice));
            predefinedReportsUpdater.AddPredefinedReport<XRCustomerStatement>(
            "Customer Statement", typeof(Customer));
            predefinedReportsUpdater.AddPredefinedReport<XRCustomerBalance>(
            "Customer Balance", typeof(Customer));
            predefinedReportsUpdater.AddPredefinedReport<XRTDSAll>(
            "TDS Register", typeof(TDS));
            predefinedReportsUpdater.AddPredefinedReport<XRDiscountRegister>(
            "Discount Register", typeof(Discount));
            predefinedReportsUpdater.AddPredefinedReport<XRReceiptRegister>(
            "Receipt Register", typeof(Receipt));
            predefinedReportsUpdater.AddPredefinedReport<XRReceipt>(
            "Booking Receipt", typeof(BookingOrder));
            predefinedReportsUpdater.AddPredefinedReport<XRMoneyReceipt>(
                        "Money Receipt", typeof(Receipt));
            predefinedReportsUpdater.AddPredefinedReport<XRGSTRegisterNeo>(
            "GST Register", typeof(Customer));
            return new ModuleUpdater[] { updater, predefinedReportsUpdater };
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.LoggedOn += application_LoggedOn;
            // Manage various aspects of the application UI and behavior at the module level.
        }
        void application_LoggedOn(object sender, LogonEventArgs e)
        {
            NotificationsModule notificationsModule = Application.Modules.FindModule<NotificationsModule>();
            DefaultNotificationsProvider notificationsProvider = notificationsModule.DefaultNotificationsProvider;
            notificationsProvider.CustomizeNotificationCollectionCriteria +=
            notificationsProvider_CustomizeNotificationCollectionCriteria;
        }

        void notificationsProvider_CustomizeNotificationCollectionCriteria(
    object sender, CustomizeCollectionCriteriaEventArgs e)
        {
            if (e.Type == typeof(TaskNotification))
            {
               // e.Criteria = CriteriaOperator.FromLambda<Task>(x => x.AssignTo == null || x.AssignTo.Oid == (Guid)CurrentUserIdOperator.CurrentUserId());
            }
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }
    }
}
