namespace GatiCarRental.Module.Controllers
{
    partial class ReceiptController
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
            this.PrintReceipt = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // PrintReceipt
            // 
            this.PrintReceipt.Caption = "Print Receipt";
            this.PrintReceipt.ConfirmationMessage = null;
            this.PrintReceipt.Id = "PrintReceipt";
            this.PrintReceipt.TargetObjectType = typeof(GatiCarRental.Module.BusinessObjects.Receipt);
            this.PrintReceipt.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.PrintReceipt.ToolTip = null;
            this.PrintReceipt.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.PrintReceipt.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PrintReceipt_Execute);
            // 
            // ReceiptController
            // 
            this.Actions.Add(this.PrintReceipt);
            this.TargetObjectType = typeof(GatiCarRental.Module.BusinessObjects.Receipt);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction PrintReceipt;
    }
}
