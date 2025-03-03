namespace GatiCarRental.Win {
    partial class XafSplashScreen
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XafSplashScreen));
            this.progressBarControl = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.labelCopyright = new DevExpress.XtraEditors.LabelControl();
            this.labelStatus = new DevExpress.XtraEditors.LabelControl();
            this.peImage = new DevExpress.XtraEditors.PictureEdit();
            this.peLogo = new DevExpress.XtraEditors.PictureEdit();
            this.pcApplicationName = new DevExpress.XtraEditors.PanelControl();
            this.labelSubtitle = new DevExpress.XtraEditors.LabelControl();
            this.labelApplicationName = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peImage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peLogo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcApplicationName)).BeginInit();
            this.pcApplicationName.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBarControl
            // 
            this.progressBarControl.EditValue = 0;
            this.progressBarControl.Location = new System.Drawing.Point(148, 521);
            this.progressBarControl.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.progressBarControl.Name = "progressBarControl";
            this.progressBarControl.Properties.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(194)))), ((int)(((byte)(194)))));
            this.progressBarControl.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.progressBarControl.Properties.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(114)))), ((int)(((byte)(0)))));
            this.progressBarControl.Properties.LookAndFeel.SkinName = "Visual Studio 2013 Blue";
            this.progressBarControl.Properties.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
            this.progressBarControl.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.progressBarControl.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
            this.progressBarControl.Properties.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(144)))), ((int)(((byte)(0)))));
            this.progressBarControl.Size = new System.Drawing.Size(700, 31);
            this.progressBarControl.TabIndex = 5;
            // 
            // labelCopyright
            // 
            this.labelCopyright.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.labelCopyright.Location = new System.Drawing.Point(48, 622);
            this.labelCopyright.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(167, 25);
            this.labelCopyright.TabIndex = 6;
            this.labelCopyright.Text = "Copyright @ 2024";
            // 
            // labelStatus
            // 
            this.labelStatus.Location = new System.Drawing.Point(149, 486);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(92, 25);
            this.labelStatus.TabIndex = 7;
            this.labelStatus.Text = "Starting...";
            // 
            // peImage
            // 
            this.peImage.EditValue = ((object)(resources.GetObject("peImage.EditValue")));
            this.peImage.Location = new System.Drawing.Point(24, 22);
            this.peImage.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.peImage.Name = "peImage";
            this.peImage.Properties.AllowFocused = false;
            this.peImage.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.peImage.Properties.Appearance.Options.UseBackColor = true;
            this.peImage.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.peImage.Properties.ShowMenu = false;
            this.peImage.Size = new System.Drawing.Size(852, 346);
            this.peImage.TabIndex = 9;
            this.peImage.Visible = false;
            // 
            // peLogo
            // 
            this.peLogo.EditValue = ((object)(resources.GetObject("peLogo.EditValue")));
            this.peLogo.Location = new System.Drawing.Point(629, 597);
            this.peLogo.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.peLogo.Name = "peLogo";
            this.peLogo.Properties.AllowFocused = false;
            this.peLogo.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.peLogo.Properties.Appearance.Options.UseBackColor = true;
            this.peLogo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.peLogo.Properties.ShowMenu = false;
            this.peLogo.Size = new System.Drawing.Size(355, 106);
            this.peLogo.TabIndex = 8;
            // 
            // pcApplicationName
            // 
            this.pcApplicationName.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(114)))), ((int)(((byte)(0)))));
            this.pcApplicationName.Appearance.Options.UseBackColor = true;
            this.pcApplicationName.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pcApplicationName.Controls.Add(this.labelSubtitle);
            this.pcApplicationName.Controls.Add(this.labelApplicationName);
            this.pcApplicationName.Dock = System.Windows.Forms.DockStyle.Top;
            this.pcApplicationName.Location = new System.Drawing.Point(3, 2);
            this.pcApplicationName.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pcApplicationName.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.pcApplicationName.Name = "pcApplicationName";
            this.pcApplicationName.Size = new System.Drawing.Size(986, 422);
            this.pcApplicationName.TabIndex = 10;
            // 
            // labelSubtitle
            // 
            this.labelSubtitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSubtitle.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
            this.labelSubtitle.Appearance.Options.UseFont = true;
            this.labelSubtitle.Appearance.Options.UseForeColor = true;
            this.labelSubtitle.Location = new System.Drawing.Point(197, 239);
            this.labelSubtitle.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.labelSubtitle.Name = "labelSubtitle";
            this.labelSubtitle.Size = new System.Drawing.Size(678, 51);
            this.labelSubtitle.TabIndex = 1;
            this.labelSubtitle.Text = "A Complete solution of Financial Advisor";
            // 
            // labelApplicationName
            // 
            this.labelApplicationName.Appearance.Font = new System.Drawing.Font("Segoe UI", 26.25F);
            this.labelApplicationName.Appearance.ForeColor = System.Drawing.SystemColors.Window;
            this.labelApplicationName.Appearance.Options.UseFont = true;
            this.labelApplicationName.Appearance.Options.UseForeColor = true;
            this.labelApplicationName.Location = new System.Drawing.Point(245, 161);
            this.labelApplicationName.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.labelApplicationName.Name = "labelApplicationName";
            this.labelApplicationName.Size = new System.Drawing.Size(447, 93);
            this.labelApplicationName.TabIndex = 0;
            this.labelApplicationName.Text = "Sai Fin Growth";
            // 
            // XafSplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(992, 711);
            this.Controls.Add(this.pcApplicationName);
            this.Controls.Add(this.peImage);
            this.Controls.Add(this.peLogo);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.progressBarControl);
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "XafSplashScreen";
            this.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peImage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peLogo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcApplicationName)).EndInit();
            this.pcApplicationName.ResumeLayout(false);
            this.pcApplicationName.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.MarqueeProgressBarControl progressBarControl;
        private DevExpress.XtraEditors.LabelControl labelCopyright;
        private DevExpress.XtraEditors.LabelControl labelStatus;
        private DevExpress.XtraEditors.PictureEdit peLogo;
        private DevExpress.XtraEditors.PictureEdit peImage;
        private DevExpress.XtraEditors.PanelControl pcApplicationName;
        private DevExpress.XtraEditors.LabelControl labelSubtitle;
        private DevExpress.XtraEditors.LabelControl labelApplicationName;
    }
}
