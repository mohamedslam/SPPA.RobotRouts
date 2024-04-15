#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base.Design;
using Stimulsoft.Base.Exceptions;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Stimulsoft.Base
{
    public partial class StiExceptionForm : Form, IComparer <AssemblyName>
    {
        public int Compare(AssemblyName x, AssemblyName y)
        {
            return x.Name.CompareTo(y.Name);
        }

        public StiExceptionForm(Exception exception)
        {
            InitializeComponent();

            var assembly = Assembly.GetAssembly(this.GetType());
            var version = assembly.GetName().Version;

            string message = exception.Message;

            #region Get additional info for ReflectionTypeLoadException
            var reflectionTypeLoad = exception as ReflectionTypeLoadException;
            if ((reflectionTypeLoad != null) && message.Contains("LoaderExceptions") && (reflectionTypeLoad.LoaderExceptions != null) && (reflectionTypeLoad.LoaderExceptions.Length > 0))
            {
                int pos = message.IndexOf(".", StringComparison.InvariantCulture);
                if (pos > 0)
                {
                    message = message.Substring(0, pos) + ":\r\n" + reflectionTypeLoad.LoaderExceptions[0].Message;
                }
            }
            #endregion

            var dt = DateTime.Now;
            var rand = new Random().Next(10000000, 99999999);
            this.textBoxNumber.Text = $"{dt.Year}{dt.Month}{dt.Day}{rand}";

            this.textBoxMessage.Text = message;
            this.textBoxApplication.Text = Application.ProductName;
            this.textBoxFramework.Text = RuntimeEnvironment.GetSystemVersion();
            this.textBoxVersion.Text = string.Format("Version: {0}.{1}.{2} from {3:D}",
                version.Major, version.Minor, version.Build, StiVersion.CreationDate);

            this.textBoxMessage2.Text = exception.Message;
            this.textBoxSource.Text = exception.Source;
            this.textBoxStackTrace.Text = exception.StackTrace;

            this.btSend.Visible = (StiDesignerAppStatus.IsRunning || !string.IsNullOrEmpty(StiExceptionProvider.ServerUrl));

            this.buttonClipboard.Image = StiExceptionsImages.Copy();
            this.buttonSaveToFile.Image = StiExceptionsImages.Save();

            var key = StiLicenseKey.Get(StiLicense.Key);
            if (key != null && !string.IsNullOrEmpty(key.UserName))
            {
                this.textBoxUserName.Text = key.UserName;
                this.textBoxUserName.ReadOnly = true;
            }
            else
            {
                this.btSend.Visible = false;
            }
            
            #region Create Assemblies
            listViewAssemblies.Clear();
            listViewAssemblies.Columns.Add(StiLocalization.Get("PropertyMain", "Name"), 320, HorizontalAlignment.Left);
            listViewAssemblies.Columns.Add(StiLocalization.Get("PropertyMain", "Version"), 150, HorizontalAlignment.Left);

            var assemblyEntry = Assembly.GetEntryAssembly();
            if (assemblyEntry == null) assemblyEntry = Assembly.GetExecutingAssembly();

            var assemblies = assemblyEntry.GetReferencedAssemblies();
            Array.Sort(assemblies, this);
            
            foreach (var assemblyName in assemblies)
            {
                var listViewItem = new ListViewItem();
                listViewItem.Text = assemblyName.Name;                
                listViewItem.SubItems.Add(assemblyName.Version.ToString());
                listViewAssemblies.Items.Add(listViewItem);
            }
            #endregion

            Localize();

            this.ShowIcon = false;
        }

        #region Methods.Localize
        private void Localize()
        {
            this.Text = StiLocalization.Get("ExceptionProvider", "ExceptionReport");

            this.toolTip1.SetToolTip(this.buttonSaveToFile, StiLocalization.Get("ExceptionProvider", "SaveToFile"));
            this.toolTip1.SetToolTip(this.buttonClipboard, StiLocalization.Get("HelpDesigner", "CopyToClipboard"));

            this.tabPageGeneral.Text = StiLocalization.Get("ExceptionProvider", "General");
            this.tabPageException.Text = StiLocalization.Get("ExceptionProvider", "Exception");
            this.tabPageAssemblies.Text = StiLocalization.Get("ExceptionProvider", "Assemblies");

            this.labelApplication.Text = StiLocalization.Get("PropertyEnum", "StiFontIconGroupWebApplicationIcons");
            this.labelVersion.Text = StiLocalization.Get("PropertyMain", "Version");
            this.labelFramework.Text = StiLocalization.Get("ExceptionProvider", "Framework");
            this.labelInformation.Text = StiLocalization.Get("ExceptionProvider", "PleaseEnterDetailedInformation");

            this.labelMessage1.Text = StiLocalization.Get("ExceptionProvider", "Message");
            this.labelSource.Text = StiLocalization.Get("ExceptionProvider", "Source");
            this.labelStackTrace.Text = StiLocalization.Get("ExceptionProvider", "StackTrace");

            this.btSend.Text = StiLocalization.Get("A_WebViewer", "ButtonSend");
            this.btCancel.Text = StiLocalization.Get("Buttons", "Close");

            this.labelNumber.Text = StiLocalization.Get("ExceptionProvider", "Number");
            this.labelUserName.Text = StiLocalization.Get("Cloud", "labelUserName").Replace(":", "");
        }
        #endregion

        #region Methods
        private string GetExceptionString()
        {            
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(textBoxInformation.Text))
            {
                sb.AppendLine("[Customer Explanation]")
                    .AppendLine(textBoxInformation.Text)
                    .AppendLine();
            }

            sb.AppendLine("[General Info]")
                .AppendLine("Application: " + this.textBoxApplication.Text)
                .AppendLine("Framework:   " + this.textBoxFramework.Text)
                .AppendLine("Version:     " + this.textBoxVersion.Text)
                .AppendLine("MachineName: " + Environment.MachineName)
                .AppendLine("OSVersion:   " + Environment.OSVersion.VersionString)
                .AppendLine("UserName:    " + Environment.UserName)
                .AppendLine();

            sb.AppendLine("[Exception Info]")
                .AppendLine("Message:     " + this.textBoxMessage.Text)
                .AppendLine()
                .AppendLine("Source:      " + this.textBoxSource.Text)
                .AppendLine()
                .AppendLine("StackTrace:").AppendLine(this.textBoxStackTrace.Text)
                .AppendLine();

            sb.AppendLine("[Assemblies]");

            var assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
            Array.Sort(assemblies, this);

            foreach (var assemblyName in assemblies)
            {
                sb.AppendLine(string.Format("{0}, Version = {1}", assemblyName.Name, assemblyName.Version));
            }

            return sb.ToString();
        }
        #endregion

        #region Handlers.This
        private void This_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                Close();
            }
        }
        #endregion

        #region Handlers
        private void ButtonSaveToFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
                    saveDialog.FilterIndex = 1;
                    saveDialog.FileName = "Exception.txt";
                    saveDialog.RestoreDirectory = true;

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (var writer = new StreamWriter(saveDialog.FileName))
                        {
                            writer.Write(GetExceptionString());
                            writer.Flush();
                            writer.Close();
                        }
                    }
                }
            }
            catch { }
        }

        private void ButtonClipboard_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(GetExceptionString());
            }
            catch { }
        }

        private async void ButtonSend_Click(object sender, EventArgs e)
        {
            bool state = await StiExceptionProviderHelper.SendAsync(textBoxUserName.Text, textBoxMessage.Text, GetExceptionString(), this.textBoxNumber.Text);

            if (state)
            {
                MessageBox.Show(this, StiLocalization.Get("ExceptionProvider", "SendErrorSuccess"), "", MessageBoxButtons.OK);
                this.Close();
            }
            else
            {
                MessageBox.Show(this, StiLocalization.Get("ExceptionProvider", "SendErrorFailed"), "", MessageBoxButtons.OK);
            }
        }
        #endregion
    }
}