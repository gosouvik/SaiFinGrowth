namespace GatiCarRental.Module.Controllers
{
    partial class CustomerPrintController
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
            this.TDSRegister = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.DiscountRegister = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.ReceiptRegister = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.InvoiceRegister = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.customerStatementAction = new DevExpress.ExpressApp.Actions.ParametrizedAction(this.components);
            this.ImportCustomer = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // TDSRegister
            // 
            this.TDSRegister.Caption = "TDS Register";
            this.TDSRegister.ConfirmationMessage = null;
            this.TDSRegister.Id = "TDSRegisterController";
            this.TDSRegister.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.TDSRegister.ToolTip = null;
            this.TDSRegister.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.TDSRegister.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.TDSRegister_Execute);
            // 
            // DiscountRegister
            // 
            this.DiscountRegister.Caption = "Discount Register";
            this.DiscountRegister.ConfirmationMessage = null;
            this.DiscountRegister.Id = "DiscountRegisterController";
            this.DiscountRegister.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.DiscountRegister.ToolTip = null;
            this.DiscountRegister.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.DiscountRegister.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DiscountRegister_Execute_1);
            // 
            // ReceiptRegister
            // 
            this.ReceiptRegister.Caption = "Receipt Register";
            this.ReceiptRegister.ConfirmationMessage = null;
            this.ReceiptRegister.Id = "ReceiptRegisterController";
            this.ReceiptRegister.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.ReceiptRegister.ToolTip = null;
            this.ReceiptRegister.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.ReceiptRegister.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ReceiptRegister_Execute_1);
            // 
            // InvoiceRegister
            // 
            this.InvoiceRegister.Caption = "Invoice Register";
            this.InvoiceRegister.ConfirmationMessage = null;
            this.InvoiceRegister.Id = "InvoiceRegisterController";
            this.InvoiceRegister.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.InvoiceRegister.ToolTip = null;
            this.InvoiceRegister.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.InvoiceRegister.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.InvoiceRegister_Execute);
            // 
            // customerStatementAction
            // 
            this.customerStatementAction.Caption = "Customer Statement";
            this.customerStatementAction.ConfirmationMessage = null;
            this.customerStatementAction.Id = "CustomerStatementAction";
            this.customerStatementAction.NullValuePrompt = null;
            this.customerStatementAction.ShortCaption = null;
            this.customerStatementAction.TargetObjectType = typeof(GatiCarRental.Module.BusinessObjects.Customer);
            this.customerStatementAction.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.customerStatementAction.ToolTip = null;
            this.customerStatementAction.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            // 
            // ImportCustomer
            // 
            this.ImportCustomer.Caption = "Import Customer Action";
            this.ImportCustomer.ConfirmationMessage = null;
            this.ImportCustomer.Id = "ImportCustomerAction";
            this.ImportCustomer.ToolTip = null;
            this.ImportCustomer.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ImportCustomer_Execute);
            // 
            // CustomerPrintController
            // 
            this.Actions.Add(this.TDSRegister);
            this.Actions.Add(this.DiscountRegister);
            this.Actions.Add(this.ReceiptRegister);
            this.Actions.Add(this.InvoiceRegister);
            this.Actions.Add(this.customerStatementAction);
            this.Actions.Add(this.ImportCustomer);
            this.TargetObjectType = typeof(GatiCarRental.Module.BusinessObjects.Customer);
            this.Activated += new System.EventHandler(this.CustomerPrintController_Activated);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction TDSRegister;
        private DevExpress.ExpressApp.Actions.SimpleAction DiscountRegister;
        private DevExpress.ExpressApp.Actions.SimpleAction ReceiptRegister;
        private DevExpress.ExpressApp.Actions.SimpleAction InvoiceRegister;
        private DevExpress.ExpressApp.Actions.ParametrizedAction customerStatementAction;
        private DevExpress.ExpressApp.Actions.SimpleAction ImportCustomer;
    }
}
