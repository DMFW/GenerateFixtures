namespace FixtureGenerator
{
    partial class frmGenerate
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
            this.btnGenerate = new System.Windows.Forms.Button();
            this.stsStrip = new System.Windows.Forms.StatusStrip();
            this.tssLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnApportion = new System.Windows.Forms.Button();
            this.btnCopyToClipboard = new System.Windows.Forms.Button();
            this.txtMasterSpreadsheet = new System.Windows.Forms.TextBox();
            this.btnSpreadsheet = new System.Windows.Forms.Button();
            this.lblMasterSpreadsheet = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.ofdSpreadsheet = new System.Windows.Forms.OpenFileDialog();
            this.lstImportValidation = new System.Windows.Forms.ListBox();
            this.stsStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.Enabled = false;
            this.btnGenerate.Location = new System.Drawing.Point(33, 532);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(7);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(226, 56);
            this.btnGenerate.TabIndex = 0;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // stsStrip
            // 
            this.stsStrip.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.stsStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssLabel});
            this.stsStrip.Location = new System.Drawing.Point(0, 735);
            this.stsStrip.Name = "stsStrip";
            this.stsStrip.Padding = new System.Windows.Forms.Padding(2, 0, 33, 0);
            this.stsStrip.Size = new System.Drawing.Size(721, 42);
            this.stsStrip.TabIndex = 6;
            // 
            // tssLabel
            // 
            this.tssLabel.Name = "tssLabel";
            this.tssLabel.Size = new System.Drawing.Size(88, 37);
            this.tssLabel.Text = "Status";
            // 
            // btnApportion
            // 
            this.btnApportion.Enabled = false;
            this.btnApportion.Location = new System.Drawing.Point(33, 639);
            this.btnApportion.Margin = new System.Windows.Forms.Padding(7);
            this.btnApportion.Name = "btnApportion";
            this.btnApportion.Size = new System.Drawing.Size(226, 54);
            this.btnApportion.TabIndex = 9;
            this.btnApportion.Text = "Apportion Lanes";
            this.btnApportion.UseVisualStyleBackColor = true;
            this.btnApportion.Click += new System.EventHandler(this.btnApportion_Click);
            // 
            // btnCopyToClipboard
            // 
            this.btnCopyToClipboard.Enabled = false;
            this.btnCopyToClipboard.Location = new System.Drawing.Point(481, 601);
            this.btnCopyToClipboard.Margin = new System.Windows.Forms.Padding(7);
            this.btnCopyToClipboard.Name = "btnCopyToClipboard";
            this.btnCopyToClipboard.Size = new System.Drawing.Size(175, 91);
            this.btnCopyToClipboard.TabIndex = 10;
            this.btnCopyToClipboard.Text = "Copy to Clipboard";
            this.btnCopyToClipboard.UseVisualStyleBackColor = true;
            this.btnCopyToClipboard.Click += new System.EventHandler(this.btnCopyToClipboard_Click);
            // 
            // txtMasterSpreadsheet
            // 
            this.txtMasterSpreadsheet.Location = new System.Drawing.Point(35, 73);
            this.txtMasterSpreadsheet.Multiline = true;
            this.txtMasterSpreadsheet.Name = "txtMasterSpreadsheet";
            this.txtMasterSpreadsheet.Size = new System.Drawing.Size(646, 81);
            this.txtMasterSpreadsheet.TabIndex = 11;
            this.txtMasterSpreadsheet.TextChanged += new System.EventHandler(this.txtMasterSpreadsheet_TextChanged);
            // 
            // btnSpreadsheet
            // 
            this.btnSpreadsheet.Location = new System.Drawing.Point(532, 25);
            this.btnSpreadsheet.Name = "btnSpreadsheet";
            this.btnSpreadsheet.Size = new System.Drawing.Size(149, 42);
            this.btnSpreadsheet.TabIndex = 12;
            this.btnSpreadsheet.Text = "Browse";
            this.btnSpreadsheet.UseVisualStyleBackColor = true;
            this.btnSpreadsheet.Click += new System.EventHandler(this.btnSpreadsheet_Click);
            // 
            // lblMasterSpreadsheet
            // 
            this.lblMasterSpreadsheet.AutoSize = true;
            this.lblMasterSpreadsheet.Location = new System.Drawing.Point(28, 32);
            this.lblMasterSpreadsheet.Name = "lblMasterSpreadsheet";
            this.lblMasterSpreadsheet.Size = new System.Drawing.Size(222, 29);
            this.lblMasterSpreadsheet.TabIndex = 13;
            this.lblMasterSpreadsheet.Text = "Master Speadsheet";
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(40, 172);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(271, 44);
            this.btnImport.TabIndex = 14;
            this.btnImport.Text = "Import from Sheet";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // ofdSpreadsheet
            // 
            this.ofdSpreadsheet.Filter = "Spreadsheets (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            // 
            // lstImportValidation
            // 
            this.lstImportValidation.FormattingEnabled = true;
            this.lstImportValidation.ItemHeight = 29;
            this.lstImportValidation.Location = new System.Drawing.Point(35, 247);
            this.lstImportValidation.Name = "lstImportValidation";
            this.lstImportValidation.Size = new System.Drawing.Size(646, 236);
            this.lstImportValidation.TabIndex = 15;
            // 
            // frmGenerate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 777);
            this.Controls.Add(this.lstImportValidation);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.lblMasterSpreadsheet);
            this.Controls.Add(this.btnSpreadsheet);
            this.Controls.Add(this.txtMasterSpreadsheet);
            this.Controls.Add(this.btnCopyToClipboard);
            this.Controls.Add(this.btnApportion);
            this.Controls.Add(this.stsStrip);
            this.Controls.Add(this.btnGenerate);
            this.Margin = new System.Windows.Forms.Padding(7);
            this.Name = "frmGenerate";
            this.Text = "Fixture Generator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmGenerate_FormClosed);
            this.Load += new System.EventHandler(this.frmGenerate_Load);
            this.stsStrip.ResumeLayout(false);
            this.stsStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.StatusStrip stsStrip;
        private System.Windows.Forms.ToolStripStatusLabel tssLabel;
        private System.Windows.Forms.Button btnApportion;
        private System.Windows.Forms.Button btnCopyToClipboard;
        private System.Windows.Forms.TextBox txtMasterSpreadsheet;
        private System.Windows.Forms.Button btnSpreadsheet;
        private System.Windows.Forms.Label lblMasterSpreadsheet;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.OpenFileDialog ofdSpreadsheet;
        private System.Windows.Forms.ListBox lstImportValidation;
    }
}

