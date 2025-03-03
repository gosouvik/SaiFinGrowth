namespace GatiCarRental.Module.Controllers
{
    partial class LeadController
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
            this.AssignLead = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // AssignLead
            // 
            this.AssignLead.AcceptButtonCaption = null;
            this.AssignLead.CancelButtonCaption = null;
            this.AssignLead.Caption = "Assign Lead";
            this.AssignLead.ConfirmationMessage = null;
            this.AssignLead.Id = "AssignLead";
            this.AssignLead.ToolTip = null;
            // 
            // LeadController
            // 
            this.Actions.Add(this.AssignLead);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction AssignLead;
    }
}
