namespace GatiCarRental.Module.Controllers
{
    partial class InvoicePrintController
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
            this.EmailInvoiceAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // PrintInvoiceAction
            // 
            this.PrintInvoiceAction.Caption = "Print Invoice";
            this.PrintInvoiceAction.Category = "Edit";
            this.PrintInvoiceAction.ConfirmationMessage = "Do you want to print?";
            this.PrintInvoiceAction.Id = "PrintInvoiceAction";
            this.PrintInvoiceAction.ImageName = "BO_Print";
            this.PrintInvoiceAction.ToolTip = null;
            this.PrintInvoiceAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PrintInvoiceAction_Execute);
            // 
            // EmailInvoiceAction
            // 
            this.EmailInvoiceAction.Caption = "Email Invoice";
            this.EmailInvoiceAction.Category = "Edit";
            this.EmailInvoiceAction.ConfirmationMessage = null;
            this.EmailInvoiceAction.Id = "EmailInvoiceAction";
            this.EmailInvoiceAction.ImageName = "BO_Print";
            this.EmailInvoiceAction.TargetObjectType = typeof(GatiCarRental.Module.BusinessObjects.Invoice);
            this.EmailInvoiceAction.ToolTip = null;
            this.EmailInvoiceAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.EmailInvoiceAction_Execute);
            // 
            // InvoicePrintController
            // 
            this.Actions.Add(this.PrintInvoiceAction);
            this.Actions.Add(this.EmailInvoiceAction);
            this.TargetObjectType = typeof(GatiCarRental.Module.BusinessObjects.Invoice);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction PrintInvoiceAction;
        private DevExpress.ExpressApp.Actions.SimpleAction EmailInvoiceAction;
    }
}
