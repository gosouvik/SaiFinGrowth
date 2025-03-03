using DevExpress.Data.Filtering;
using DevExpress.DataProcessing.InMemoryDataProcessor;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatiCarRental.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class LICApplicationController : ViewController
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public LICApplicationController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            UpdateActions();
            ((SimpleAction)GetAction("RemoveHold", "LICApplication")).Execute += new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("Approved", "LICApplication")).Execute += new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("Submitted", "LICApplication")).Execute += new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("Inprocess", "LICApplication")).Execute += new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("Rejected", "LICApplication")).Execute += new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("Closed", "LICApplication")).Execute += new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("ConvertToPolicy", "LICApplication")).Execute += new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            //ConvertToPolicy
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
            ((SimpleAction)GetAction("RemoveHold", "LICApplication")).Execute -= new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("Approved", "LICApplication")).Execute -= new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("Submitted", "LICApplication")).Execute -= new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("Inprocess", "LICApplication")).Execute -= new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("Rejected", "LICApplication")).Execute -= new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("Closed", "LICApplication")).Execute -= new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
            ((SimpleAction)GetAction("ConvertToPolicy", "LICApplication")).Execute -= new SimpleActionExecuteEventHandler(LICApplicationController_Execute);
        }

        void LICApplicationController_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            UpdateActions();
        }

        private bool IsEnabled(ActionBase action)
        {
            if (action == null)
                return ((View as DetailView).CurrentObject as LICApplication).Status == LICApplicationStatus.Hold;
            if (action.Id == "LICApplication.RemoveHold")
                return ((View as DetailView).CurrentObject as LICApplication).Status == LICApplicationStatus.Hold;
            if (action.Id == "LICApplication.Approved")
                return ((View as DetailView).CurrentObject as LICApplication).Status == LICApplicationStatus.PendingApproval;
            if (action.Id == "LICApplication.Submitted")
                return ((View as DetailView).CurrentObject as LICApplication).Status == LICApplicationStatus.Approved;
            if (action.Id == "LICApplication.Inprocess")
                return ((View as DetailView).CurrentObject as LICApplication).Status == LICApplicationStatus.Submitted;
            if (action.Id == "LICApplication.Rejected")
                return ((View as DetailView).CurrentObject as LICApplication).Status == LICApplicationStatus.Hold;
            if (action.Id == "LICApplication.Closed")
                return ((View as DetailView).CurrentObject as LICApplication).Status == LICApplicationStatus.Inprocess;
            if (action.Id == "LICApplication.ConvertToPolicy")
                return ((View as DetailView).CurrentObject as LICApplication).Status == LICApplicationStatus.Closed;
            return false;
        }

        private ActionBase GetAction(string id, string typeName)
        {
            // search in controller of actions added in business object entities:  
            if (id.Contains("."))
                typeName = "";
            else if (!typeName.EndsWith("."))
                typeName += ".";

            ActionBase action = Frame.GetController<ObjectMethodActionsViewController>().Actions[typeName + id];
            if (action != null)
                return action;
            // search in this controller  
            action = this.Actions[id];
            if (action != null)
                return action;
            // action was not found, so search all controllers  
            foreach (Controller item in Frame.Controllers.Values)
            {
                foreach (ActionBase act in item.Actions)
                {
                    if (act.Id.Equals(id))
                        return act;
                }
            }
            return null;
        }

        private void UpdateActions()
        {
            try
            {
                List<ActionBase> list = new List<ActionBase>();
                list.Add(GetAction("RemoveHold", "LICApplication"));
                list.Add(GetAction("Approved", "LICApplication"));
                list.Add(GetAction("Submitted", "LICApplication"));
                list.Add(GetAction("Inprocess", "LICApplication"));
                list.Add(GetAction("Rejected", "LICApplication"));
                list.Add(GetAction("Closed", "LICApplication"));
                list.Add(GetAction("ConvertToPolicy", "LICApplication"));
                foreach (ActionBase action in list)
                {
                    bool enabled = IsEnabled(action);
                    action.Enabled.SetItemValue("Enabled", enabled);
                }
                //Frame.GetController<WebDetailViewController>().Actions //EditAction.Enabled.SetItemValue("Enabled", canEdit.Value);  
            }
            catch { }
        }
    }
}
