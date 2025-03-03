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
using DevExpress.ExpressApp.Dashboards.Win;
using DevExpress.DashboardCommon;

namespace GatiCarRental.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class DashboardDesignerManagerEx : DashboardDesignerManager
    {
        public DashboardDesignerManagerEx(XafApplication application) : base(application)
        {
        }
        protected override void ProcessDashboardBeforeSaving(Dashboard dashboard)
        {
            //base.ProcessDashboardBeforeSaving(dashboard);  
        }
    }

    public class DashboardDesignerController : ObjectViewController<ObjectView, IDashboardData>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            WinShowDashboardDesignerController showDashboardDesignerController = Frame.GetController<WinShowDashboardDesignerController>();
            if (showDashboardDesignerController != null)
            {
                showDashboardDesignerController.DashboardDesignerManager = new DashboardDesignerManagerEx(Application);
                showDashboardDesignerController.DashboardDesignerManager.DashboardDesignerCreated += DashboardDesignerManager_DashboardDesignerCreated;
            }
        }
        private void DashboardDesignerManager_DashboardDesignerCreated(object sender, DashboardDesignerShownEventArgs e)
        {
            //this part...  
            e.DashboardDesigner.DataSourceWizard.ShowConnectionsFromAppConfig = false;
            e.DashboardDesigner.DataSourceWizard.SqlWizardSettings.DatabaseCredentialsSavingBehavior = DevExpress.DataAccess.Wizard.SensitiveInfoSavingBehavior.Always;
        }
        protected override void OnDeactivated()
        {
            WinShowDashboardDesignerController showDashboardDesignerController = Frame.GetController<WinShowDashboardDesignerController>();
            if (showDashboardDesignerController != null)
                showDashboardDesignerController.DashboardDesignerManager.DashboardDesignerCreated -= DashboardDesignerManager_DashboardDesignerCreated;
            base.OnDeactivated();
        }
    }
}
