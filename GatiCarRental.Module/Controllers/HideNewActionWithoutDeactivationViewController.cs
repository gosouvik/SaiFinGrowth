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
using System.ComponentModel;
using DevExpress.ExpressApp.Templates.ActionControls;
using GatiCarRental.Module.BusinessObjects;

namespace GatiCarRental.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class HideNewActionWithoutDeactivationViewController : ViewController
    {
        private ActionControlsSiteController actionControlsSiteController = null;
        private FillActionContainersController fillActionsController = null;

        public HideNewActionWithoutDeactivationViewController()
        {
            InitializeComponent();
            TargetViewId = "BookingOrder_DutySlips_ListView";
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            actionControlsSiteController = Frame.GetController<ActionControlsSiteController>();
            if (actionControlsSiteController != null)
            {
                actionControlsSiteController.CustomAddActionControlToContainer += ActionControlsSiteController_CustomAddActionControlToContainer;
            }
            fillActionsController = Frame.GetController<FillActionContainersController>();
            if (fillActionsController != null)
            {
                fillActionsController.CustomRegisterActionInContainer += FillActionsController_CustomRegisterActionInContainer;
            }

            if (View.Id == TargetViewId)
            {
                ((CompositeView)View).ItemsChanged += BookingOrderPickedController_ItemsChanged;
            }
        }

        private void BookingOrderPickedController_ItemsChanged(object sender, ViewItemsChangedEventArgs e)
        {
            if (e.ChangedType == ViewItemsChangedType.Added && e.Item.Id == "Picked")
            {
                DisableActionControlCreation(new HandledEventArgs(), e.Item.Id);
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        private void DisableActionControlCreation(HandledEventArgs args, string actionId)
        {

            if (View.Id == TargetViewId)
            {
                if (actionId == NewObjectViewController.NewActionId)
                {
                    args.Handled = true;
                    BookingOrder record = (BookingOrder)((DevExpress.ExpressApp.DetailView)this.ObjectSpace.Owner).CurrentObject;
                    if (record.PickupDateTime.Year > 2000)
                    {
                        if (record.DutySlips.Count == 0 && record.CarNumber != null && record.PickupDateTime <= DateTime.Today)
                        {
                            args.Handled = false;
                        }
                    }
                }
            }

        }
        private void FillActionsController_CustomRegisterActionInContainer(object sender, CustomRegisterActionInContainerEventArgs e)
        {
            DisableActionControlCreation(e, e.Action.Id);
        }
        private void ActionControlsSiteController_CustomAddActionControlToContainer(object sender, CustomAddActionControlEventArgs e)
        {
            DisableActionControlCreation(e, e.Action.Id);
        }
        protected override void OnDeactivated()
        {
            if (actionControlsSiteController != null)
            {
                actionControlsSiteController.CustomAddActionControlToContainer -= ActionControlsSiteController_CustomAddActionControlToContainer;
            }
            if (fillActionsController != null)
            {
                fillActionsController.CustomRegisterActionInContainer -= FillActionsController_CustomRegisterActionInContainer;
            }
            base.OnDeactivated();
        }
    }
}
