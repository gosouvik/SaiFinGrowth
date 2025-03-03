namespace GatiCarRental.Module.Controllers
{
    partial class DryInvoiceController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.PrintInvoiceAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // PrintInvoiceAction
            // 
            this.PrintInvoiceAction.Caption = "Print Invoice";
            this.PrintInvoiceAction.Category = "Edit";
            this.PrintInvoiceAction.ConfirmationMessage = "Do you want to Print";
            this.PrintInvoiceAction.Id = "DryInvoicePrintAction";
            this.PrintInvoiceAction.ImageName = "BO_Print";
            this.PrintInvoiceAction.ToolTip = null;
            this.PrintInvoiceAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PrintInvoiceAction_Execute);
            // 
            // DryInvoiceController
            // 
            this.Actions.Add(this.PrintInvoiceAction);
            this.TargetObjectType = typeof(GatiCarRental.Module.BusinessObjects.DryInvoice);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction PrintInvoiceAction;
    }
}
