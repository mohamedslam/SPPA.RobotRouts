namespace Stimulsoft.Base
{
    partial class StiExceptionForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StiExceptionForm));
            this.btCancel = new System.Windows.Forms.Button();
            this.btSend = new System.Windows.Forms.Button();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.tabControlGeneral = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.labelInformation = new System.Windows.Forms.Label();
            this.labelFramework = new System.Windows.Forms.Label();
            this.labelUserName = new System.Windows.Forms.Label();
            this.labelNumber = new System.Windows.Forms.Label();
            this.textBoxInformation = new System.Windows.Forms.TextBox();
            this.textBoxFramework = new System.Windows.Forms.TextBox();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.textBoxNumber = new System.Windows.Forms.TextBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.textBoxVersion = new System.Windows.Forms.TextBox();
            this.labelApplication = new System.Windows.Forms.Label();
            this.textBoxApplication = new System.Windows.Forms.TextBox();
            this.picGeneral = new System.Windows.Forms.PictureBox();
            this.tabPageException = new System.Windows.Forms.TabPage();
            this.labelStackTrace = new System.Windows.Forms.Label();
            this.labelSource = new System.Windows.Forms.Label();
            this.labelMessage1 = new System.Windows.Forms.Label();
            this.textBoxStackTrace = new System.Windows.Forms.TextBox();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.textBoxMessage2 = new System.Windows.Forms.TextBox();
            this.tabPageAssemblies = new System.Windows.Forms.TabPage();
            this.listViewAssemblies = new System.Windows.Forms.ListView();
            this.buttonClipboard = new System.Windows.Forms.Button();
            this.buttonSaveToFile = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControlGeneral.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGeneral)).BeginInit();
            this.tabPageException.SuspendLayout();
            this.tabPageAssemblies.SuspendLayout();
            this.SuspendLayout();
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(1006, 833);
            this.btCancel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(152, 46);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "Close";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btSend
            // 
            this.btSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btSend.Location = new System.Drawing.Point(842, 833);
            this.btSend.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btSend.Name = "btSend";
            this.btSend.Size = new System.Drawing.Size(152, 46);
            this.btSend.TabIndex = 1;
            this.btSend.Text = "Send";
            this.btSend.UseVisualStyleBackColor = true;
            this.btSend.Click += new System.EventHandler(this.ButtonSend_Click);            
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMessage.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxMessage.Location = new System.Drawing.Point(176, 23);
            this.textBoxMessage.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ReadOnly = true;
            this.textBoxMessage.Size = new System.Drawing.Size(916, 119);
            this.textBoxMessage.TabIndex = 0;
            // 
            // tabControlGeneral
            // 
            this.tabControlGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlGeneral.Controls.Add(this.tabPageGeneral);
            this.tabControlGeneral.Controls.Add(this.tabPageException);
            this.tabControlGeneral.Controls.Add(this.tabPageAssemblies);
            this.tabControlGeneral.Location = new System.Drawing.Point(24, 23);
            this.tabControlGeneral.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabControlGeneral.Name = "tabControlGeneral";
            this.tabControlGeneral.SelectedIndex = 0;
            this.tabControlGeneral.Size = new System.Drawing.Size(1136, 785);
            this.tabControlGeneral.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.BackColor = System.Drawing.Color.White;
            this.tabPageGeneral.Controls.Add(this.labelInformation);
            this.tabPageGeneral.Controls.Add(this.labelFramework);
            this.tabPageGeneral.Controls.Add(this.labelUserName);
            this.tabPageGeneral.Controls.Add(this.labelNumber);
            this.tabPageGeneral.Controls.Add(this.textBoxInformation);
            this.tabPageGeneral.Controls.Add(this.textBoxFramework);
            this.tabPageGeneral.Controls.Add(this.textBoxUserName);
            this.tabPageGeneral.Controls.Add(this.textBoxNumber);
            this.tabPageGeneral.Controls.Add(this.labelVersion);
            this.tabPageGeneral.Controls.Add(this.textBoxVersion);
            this.tabPageGeneral.Controls.Add(this.labelApplication);
            this.tabPageGeneral.Controls.Add(this.textBoxApplication);
            this.tabPageGeneral.Controls.Add(this.picGeneral);
            this.tabPageGeneral.Controls.Add(this.textBoxMessage);
            this.tabPageGeneral.Location = new System.Drawing.Point(8, 39);
            this.tabPageGeneral.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPageGeneral.Size = new System.Drawing.Size(1120, 738);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            // 
            // labelInformation
            // 
            this.labelInformation.ForeColor = System.Drawing.Color.DimGray;
            this.labelInformation.Location = new System.Drawing.Point(24, 475);
            this.labelInformation.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelInformation.Name = "labelInformation";
            this.labelInformation.Size = new System.Drawing.Size(1066, 38);
            this.labelInformation.TabIndex = 40;
            this.labelInformation.Text = "Please enter detailed information about events which cause this exception. ";
            this.labelInformation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFramework
            // 
            this.labelFramework.ForeColor = System.Drawing.Color.DimGray;
            this.labelFramework.Location = new System.Drawing.Point(24, 292);
            this.labelFramework.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelFramework.Name = "labelFramework";
            this.labelFramework.Size = new System.Drawing.Size(140, 38);
            this.labelFramework.TabIndex = 40;
            this.labelFramework.Text = "Framework";
            this.labelFramework.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelUserName
            // 
            this.labelUserName.ForeColor = System.Drawing.Color.DimGray;
            this.labelUserName.Location = new System.Drawing.Point(24, 358);
            this.labelUserName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(140, 38);
            this.labelUserName.TabIndex = 40;
            this.labelUserName.Text = "User Name";
            this.labelUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelNumber
            // 
            this.labelNumber.ForeColor = System.Drawing.Color.DimGray;
            this.labelNumber.Location = new System.Drawing.Point(24, 417);
            this.labelNumber.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelNumber.Name = "labelNumber";
            this.labelNumber.Size = new System.Drawing.Size(140, 38);
            this.labelNumber.TabIndex = 40;
            this.labelNumber.Text = "Number";
            this.labelNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxInformation
            // 
            this.textBoxInformation.AcceptsReturn = true;
            this.textBoxInformation.AcceptsTab = true;
            this.textBoxInformation.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxInformation.Location = new System.Drawing.Point(24, 519);
            this.textBoxInformation.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxInformation.Multiline = true;
            this.textBoxInformation.Name = "textBoxInformation";
            this.textBoxInformation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxInformation.Size = new System.Drawing.Size(1068, 185);
            this.textBoxInformation.TabIndex = 8;
            // 
            // textBoxFramework
            // 
            this.textBoxFramework.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFramework.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxFramework.Location = new System.Drawing.Point(176, 292);
            this.textBoxFramework.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxFramework.Name = "textBoxFramework";
            this.textBoxFramework.ReadOnly = true;
            this.textBoxFramework.Size = new System.Drawing.Size(916, 31);
            this.textBoxFramework.TabIndex = 4;
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUserName.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxUserName.Location = new System.Drawing.Point(176, 354);
            this.textBoxUserName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(916, 31);
            this.textBoxUserName.TabIndex = 4;
            // 
            // textBoxNumber
            // 
            this.textBoxNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNumber.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxNumber.Location = new System.Drawing.Point(176, 413);
            this.textBoxNumber.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxNumber.Name = "textBoxNumber";
            this.textBoxNumber.ReadOnly = true;
            this.textBoxNumber.Size = new System.Drawing.Size(916, 31);
            this.textBoxNumber.TabIndex = 4;
            // 
            // labelVersion
            // 
            this.labelVersion.ForeColor = System.Drawing.Color.DimGray;
            this.labelVersion.Location = new System.Drawing.Point(24, 231);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(128, 38);
            this.labelVersion.TabIndex = 30;
            this.labelVersion.Text = "Version";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxVersion
            // 
            this.textBoxVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxVersion.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxVersion.Location = new System.Drawing.Point(176, 231);
            this.textBoxVersion.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxVersion.Name = "textBoxVersion";
            this.textBoxVersion.ReadOnly = true;
            this.textBoxVersion.Size = new System.Drawing.Size(916, 31);
            this.textBoxVersion.TabIndex = 2;
            // 
            // labelApplication
            // 
            this.labelApplication.ForeColor = System.Drawing.Color.DimGray;
            this.labelApplication.Location = new System.Drawing.Point(24, 171);
            this.labelApplication.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelApplication.Name = "labelApplication";
            this.labelApplication.Size = new System.Drawing.Size(128, 38);
            this.labelApplication.TabIndex = 28;
            this.labelApplication.Text = "Application";
            this.labelApplication.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxApplication
            // 
            this.textBoxApplication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxApplication.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxApplication.Location = new System.Drawing.Point(176, 169);
            this.textBoxApplication.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxApplication.Name = "textBoxApplication";
            this.textBoxApplication.ReadOnly = true;
            this.textBoxApplication.Size = new System.Drawing.Size(916, 31);
            this.textBoxApplication.TabIndex = 1;
            // 
            // picGeneral
            // 
            this.picGeneral.Image = ((System.Drawing.Image)(resources.GetObject("picGeneral.Image")));
            this.picGeneral.Location = new System.Drawing.Point(24, 23);
            this.picGeneral.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.picGeneral.Name = "picGeneral";
            this.picGeneral.Size = new System.Drawing.Size(128, 123);
            this.picGeneral.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picGeneral.TabIndex = 26;
            this.picGeneral.TabStop = false;
            // 
            // tabPageException
            // 
            this.tabPageException.BackColor = System.Drawing.Color.White;
            this.tabPageException.Controls.Add(this.labelStackTrace);
            this.tabPageException.Controls.Add(this.labelSource);
            this.tabPageException.Controls.Add(this.labelMessage1);
            this.tabPageException.Controls.Add(this.textBoxStackTrace);
            this.tabPageException.Controls.Add(this.textBoxSource);
            this.tabPageException.Controls.Add(this.textBoxMessage2);
            this.tabPageException.Location = new System.Drawing.Point(8, 39);
            this.tabPageException.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPageException.Name = "tabPageException";
            this.tabPageException.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPageException.Size = new System.Drawing.Size(1120, 738);
            this.tabPageException.TabIndex = 1;
            this.tabPageException.Text = "Exception";
            // 
            // labelStackTrace
            // 
            this.labelStackTrace.ForeColor = System.Drawing.Color.DimGray;
            this.labelStackTrace.Location = new System.Drawing.Point(24, 323);
            this.labelStackTrace.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelStackTrace.Name = "labelStackTrace";
            this.labelStackTrace.Size = new System.Drawing.Size(1064, 38);
            this.labelStackTrace.TabIndex = 41;
            this.labelStackTrace.Text = "Stack Trace";
            this.labelStackTrace.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSource
            // 
            this.labelSource.ForeColor = System.Drawing.Color.DimGray;
            this.labelSource.Location = new System.Drawing.Point(24, 210);
            this.labelSource.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(1070, 38);
            this.labelSource.TabIndex = 41;
            this.labelSource.Text = "Source";
            this.labelSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelMessage1
            // 
            this.labelMessage1.ForeColor = System.Drawing.Color.DimGray;
            this.labelMessage1.Location = new System.Drawing.Point(24, 23);
            this.labelMessage1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelMessage1.Name = "labelMessage1";
            this.labelMessage1.Size = new System.Drawing.Size(1070, 38);
            this.labelMessage1.TabIndex = 41;
            this.labelMessage1.Text = "Message";
            this.labelMessage1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxStackTrace
            // 
            this.textBoxStackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStackTrace.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxStackTrace.Location = new System.Drawing.Point(24, 369);
            this.textBoxStackTrace.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxStackTrace.Multiline = true;
            this.textBoxStackTrace.Name = "textBoxStackTrace";
            this.textBoxStackTrace.ReadOnly = true;
            this.textBoxStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxStackTrace.Size = new System.Drawing.Size(1066, 214);
            this.textBoxStackTrace.TabIndex = 2;
            // 
            // textBoxSource
            // 
            this.textBoxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSource.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxSource.Location = new System.Drawing.Point(24, 256);
            this.textBoxSource.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxSource.Multiline = true;
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.ReadOnly = true;
            this.textBoxSource.Size = new System.Drawing.Size(1066, 41);
            this.textBoxSource.TabIndex = 1;
            // 
            // textBoxMessage2
            // 
            this.textBoxMessage2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMessage2.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxMessage2.Location = new System.Drawing.Point(24, 67);
            this.textBoxMessage2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxMessage2.Multiline = true;
            this.textBoxMessage2.Name = "textBoxMessage2";
            this.textBoxMessage2.ReadOnly = true;
            this.textBoxMessage2.Size = new System.Drawing.Size(1066, 119);
            this.textBoxMessage2.TabIndex = 0;
            // 
            // tabPageAssemblies
            // 
            this.tabPageAssemblies.BackColor = System.Drawing.Color.White;
            this.tabPageAssemblies.Controls.Add(this.listViewAssemblies);
            this.tabPageAssemblies.Location = new System.Drawing.Point(8, 39);
            this.tabPageAssemblies.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPageAssemblies.Name = "tabPageAssemblies";
            this.tabPageAssemblies.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPageAssemblies.Size = new System.Drawing.Size(1120, 738);
            this.tabPageAssemblies.TabIndex = 2;
            this.tabPageAssemblies.Text = "Assemblies";
            // 
            // listViewAssemblies
            // 
            this.listViewAssemblies.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewAssemblies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewAssemblies.FullRowSelect = true;
            this.listViewAssemblies.HideSelection = false;
            this.listViewAssemblies.HotTracking = true;
            this.listViewAssemblies.HoverSelection = true;
            this.listViewAssemblies.Location = new System.Drawing.Point(24, 23);
            this.listViewAssemblies.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.listViewAssemblies.Name = "listViewAssemblies";
            this.listViewAssemblies.Size = new System.Drawing.Size(1066, 687);
            this.listViewAssemblies.TabIndex = 22;
            this.listViewAssemblies.UseCompatibleStateImageBehavior = false;
            this.listViewAssemblies.View = System.Windows.Forms.View.Details;
            // 
            // buttonClipboard
            // 
            this.buttonClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClipboard.Location = new System.Drawing.Point(84, 833);
            this.buttonClipboard.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonClipboard.Name = "buttonClipboard";
            this.buttonClipboard.Size = new System.Drawing.Size(48, 46);
            this.buttonClipboard.TabIndex = 1;
            this.buttonClipboard.UseVisualStyleBackColor = true;
            this.buttonClipboard.Click += new System.EventHandler(this.ButtonClipboard_Click);
            // 
            // buttonSaveToFile
            // 
            this.buttonSaveToFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveToFile.Location = new System.Drawing.Point(24, 833);
            this.buttonSaveToFile.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonSaveToFile.Name = "buttonSaveToFile";
            this.buttonSaveToFile.Size = new System.Drawing.Size(48, 46);
            this.buttonSaveToFile.TabIndex = 1;
            this.buttonSaveToFile.UseVisualStyleBackColor = true;
            this.buttonSaveToFile.Click += new System.EventHandler(this.ButtonSaveToFile_Click);
            // 
            // StiExceptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1184, 900);
            this.Controls.Add(this.tabControlGeneral);
            this.Controls.Add(this.buttonSaveToFile);
            this.Controls.Add(this.buttonClipboard);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btSend);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StiExceptionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Exception Report";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.This_KeyDown);
            this.tabControlGeneral.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGeneral)).EndInit();
            this.tabPageException.ResumeLayout(false);
            this.tabPageException.PerformLayout();
            this.tabPageAssemblies.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btSend;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.TabControl tabControlGeneral;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TabPage tabPageException;
        private System.Windows.Forms.PictureBox picGeneral;
        private System.Windows.Forms.Label labelInformation;
        private System.Windows.Forms.Label labelFramework;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.Label labelNumber;
        private System.Windows.Forms.TextBox textBoxInformation;
        private System.Windows.Forms.TextBox textBoxFramework;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.TextBox textBoxNumber;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TextBox textBoxVersion;
        private System.Windows.Forms.Label labelApplication;
        private System.Windows.Forms.TextBox textBoxApplication;
        private System.Windows.Forms.TextBox textBoxStackTrace;
        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.TextBox textBoxMessage2;
        private System.Windows.Forms.Label labelMessage1;
        private System.Windows.Forms.Label labelSource;
        private System.Windows.Forms.Label labelStackTrace;
        private System.Windows.Forms.TabPage tabPageAssemblies;
        private System.Windows.Forms.ListView listViewAssemblies;
        private System.Windows.Forms.Button buttonClipboard;
        private System.Windows.Forms.Button buttonSaveToFile;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}