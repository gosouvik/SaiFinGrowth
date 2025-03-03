namespace GatiCarRental.Module.Controllers
{
    partial class BookingOrderController
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
            this.PrintAdvanceSlip = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // PrintAdvanceSlip
            // 
            this.PrintAdvanceSlip.Caption = "Print Advance Slip";
            this.PrintAdvanceSlip.ConfirmationMessage = null;
            this.PrintAdvanceSlip.Id = "PrintAdvanceSlip";
            this.PrintAdvanceSlip.TargetObjectType = typeof(GatiCarRental.Module.BusinessObjects.BookingOrder);
            this.PrintAdvanceSlip.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.PrintAdvanceSlip.ToolTip = null;
            this.PrintAdvanceSlip.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.PrintAdvanceSlip.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PrintAdvanceSlip_Execute);
            // 
            // BookingOrderController
            // 
            this.Actions.Add(this.PrintAdvanceSlip);
            this.TargetObjectType = typeof(GatiCarRental.Module.BusinessObjects.BookingOrder);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction PrintAdvanceSlip;
    }
}
