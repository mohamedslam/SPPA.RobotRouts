namespace Stimulsoft.Base
{
    partial class StiExceptionPreviewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StiExceptionPreviewForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.imageMessage = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBlockTitle = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.buttonContinue = new Stimulsoft.Base.StiExceptionPreviewForm.ExceptionButton();
            this.buttonReport = new Stimulsoft.Base.StiExceptionPreviewForm.ExceptionButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageMessage)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.imageMessage);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(687, 190);
            this.panel1.TabIndex = 0;
            // 
            // imageMessage
            // 
            this.imageMessage.Image = ((System.Drawing.Image)(resources.GetObject("imageMessage.Image")));
            this.imageMessage.Location = new System.Drawing.Point(244, 50);
            this.imageMessage.Name = "imageMessage";
            this.imageMessage.Size = new System.Drawing.Size(112, 112);
            this.imageMessage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imageMessage.TabIndex = 0;
            this.imageMessage.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBlockTitle);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 200);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(687, 80);
            this.panel2.TabIndex = 1;
            // 
            // textBlockTitle
            // 
            this.textBlockTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBlockTitle.Font = new System.Drawing.Font("Arial", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBlockTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(109)))), ((int)(((byte)(185)))));
            this.textBlockTitle.Location = new System.Drawing.Point(0, 0);
            this.textBlockTitle.Name = "textBlockTitle";
            this.textBlockTitle.Size = new System.Drawing.Size(687, 35);
            this.textBlockTitle.TabIndex = 0;
            this.textBlockTitle.Text = "message";
            this.textBlockTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.textBlockTitle.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.buttonContinue);
            this.panel3.Controls.Add(this.buttonReport);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 235);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(687, 65);
            this.panel3.TabIndex = 2;
            // 
            // buttonContinue
            // 
            this.buttonContinue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonContinue.ForeColor = System.Drawing.Color.White;
            this.buttonContinue.IsContinueButton = true;
            this.buttonContinue.Location = new System.Drawing.Point(306, 15);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(140, 30);
            this.buttonContinue.TabIndex = 1;
            this.buttonContinue.Text = "Continue";
            this.buttonContinue.UseVisualStyleBackColor = true;
            this.buttonContinue.Click += new System.EventHandler(this.ButtonContinue_Click);
            // 
            // buttonReport
            // 
            this.buttonReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonReport.IsContinueButton = false;
            this.buttonReport.Location = new System.Drawing.Point(154, 15);
            this.buttonReport.Name = "buttonReport";
            this.buttonReport.Size = new System.Drawing.Size(140, 30);
            this.buttonReport.TabIndex = 0;
            this.buttonReport.Text = "Report";
            this.buttonReport.UseVisualStyleBackColor = true;
            this.buttonReport.Click += new System.EventHandler(this.ButtonReport_Click);
            // 
            // StiExceptionPreviewForm
            // 
            this.AcceptButton = this.buttonContinue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(600, 370);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StiExceptionPreviewForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Exception Report";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.This_KeyDown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageMessage)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox imageMessage;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label textBlockTitle;
        private System.Windows.Forms.Panel panel3;
        private Stimulsoft.Base.StiExceptionPreviewForm.ExceptionButton buttonReport;
        private Stimulsoft.Base.StiExceptionPreviewForm.ExceptionButton buttonContinue;
    }
}