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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatiCarRental.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class LeadController : ViewController
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public LeadController()
        {
            InitializeComponent();
            TargetObjectType = typeof(Lead);
            TargetViewType = ViewType.ListView;

            PopupWindowShowAction assignLeadAction = new PopupWindowShowAction(this, "AssignLeadAction", PredefinedCategory.Edit)
            {
                Caption = "Assign Leads"
            };

            assignLeadAction.CustomizePopupWindowParams += AssignLeadAction_CustomizePopupWindowParams;
            // Target required Views (via the TargetXXX properties) and create their Actions.
            assignLeadAction.Execute += LeadAssignAction_Execute;
        }

        private void AssignLeadAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            //Create a List View for Note objects in the pop-up window.
            e.View = Application.CreateListView(typeof(Employee), true) ;
        }

        private void LeadAssignAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (this.View.SelectedObjects.Count >= 1 && this.View.Id == "Lead_ListView")
            {
                //IList<Lead> selectedLeads = this.View.SelectedObjects;
                 //Lead leads = (Lead)View.CurrentObject;
                foreach (Lead le in this.View.SelectedObjects)
                {
                    if (e.PopupWindowViewSelectedObjects.Count ==1 )
                    {
                        foreach (Employee note in e.PopupWindowViewSelectedObjects)
                        {
                            Employee employee = View.ObjectSpace.GetObject<Employee>(note);
                            employee.MyLeads.Add(le);
                        }
                    }
                    
                }
                
            }
                
            View.ObjectSpace.CommitChanges();
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
    }
}
