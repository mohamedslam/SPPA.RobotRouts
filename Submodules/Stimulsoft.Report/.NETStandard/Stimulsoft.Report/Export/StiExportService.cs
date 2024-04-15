#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
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

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using System.Diagnostics;
using Stimulsoft.Report.Chart;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Export
{

	/// <summary>
	///  Describes an abstract class that allows to save and load documents.
	/// </summary>
	[StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Bmp.Export.ExportService.bmp")]
	[StiServiceCategoryBitmap(typeof(StiExportService), "Stimulsoft.Report.Bmp.Export.ExportService.bmp")]
	public abstract class StiExportService : StiService
	{
		#region StiService override
        /// <summary>
        /// Gets a service type.
        /// </summary>
        public sealed override Type ServiceType => typeof(StiExportService);

        /// <summary>
        /// Gets a service category.
        /// </summary>
        public sealed override string ServiceCategory => StiLocalization.Get("Services", "categoryExport");
        #endregion

        #region Events
        /// <summary>
        /// An event which fires when report is being exported.
        /// </summary>
        public event StiExportingEventHandler Exporting;

        public void InvokeExporting(StiPage page, StiPagesCollection pages, int currentPass, int maximumPass)
		{
            InvokeExporting(pages.IndexOf(page), pages.Count, currentPass, maximumPass); 
		}

        public void InvokeExporting(int value, int maximum, int currentPass, int maximumPass)
		{
            InvokeExporting(new StiExportingEventArgs(value, maximum, currentPass, maximumPass)); 
		}

		public void InvokeExporting(StiExportingEventArgs e)
		{
			if (Progress != null)
			    Progress.SetProgressBar(e.Value, e.Maximum);

			OnExporting(e);
		}

		public void OnExporting(StiExportingEventArgs e)
		{
            Exporting?.Invoke(this, e);
        }
		#endregion

        #region Methods
        internal void StartExport(StiReport report, Stream stream, StiExportSettings settings, bool sendEMail, 
            bool openAfterExport, string fileName, StiGuiMode guiMode)
        {
            var info = new StiExportInfo(report, settings, stream, sendEMail, openAfterExport, fileName);

            if (StiOptions.Engine.AllowProgressInThread)
            {
                ShowProgress();

                using (var worker = new BackgroundWorker())
                {
                    worker.WorkerSupportsCancellation = true;
                    worker.DoWork += Worker_DoWork;
                    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                    worker.RunWorkerAsync(info);
                }
            }
            else
            {
                ShowProgress();

                try
                {
                    DoExport(info);
                }
                finally
                {
                    CloseProgress();
                }

                ProcessFile(info.SendEMail, info.OpenAfterExport, info.FileName, info.Report);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var info = e.Result as StiExportInfo;

            CloseProgress();

            ProcessFile(info.SendEMail, info.OpenAfterExport, info.FileName, info.Report);
        }        

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var info = e.Argument as StiExportInfo;
            e.Result = info;
            DoExport(info);       
        }

        private void DoExport(StiExportInfo info)
        {
            try
            {
                #region Clear chart animation before export
                foreach (StiPage page in info.Report.RenderedPages)
                {
					foreach(var component in page.GetComponents())
                    {
						var chart = component as IStiChart;
						if (chart != null)
							chart.PreviousAnimations.Clear();
                    }
                }
                #endregion

                info.Report.ExportDocument(this.ExportFormat, this, info.Stream, info.Settings);
                InvokeExporting(100, 100, 1, 1);
            }
            catch (StopProgressException)
            {
            }
            catch (Exception ee)
            {
                StiLogService.Write(this.GetType(), "Save document...ERROR");
                StiLogService.Write(this.GetType(), ee);
                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                info.Stream.Flush();
                info.Stream.Close();
            }
        }

		internal void StartProgress(StiGuiMode guiMode)
		{
			isStopped = false;
            Progress = StiGuiOptions.GetProgressInformation(this.OwnerWindow, guiMode);
		}

        protected void ShowProgress()
        {
            Progress.Start(StiLocalization.Get("Export", "ExportingReport"));
        }

		protected void CloseProgress()
		{
			isStopped = Progress.IsBreaked;
			Progress.Close();
			Progress = null;
		}

        /// <summary>
        /// Exports a document to the stream.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="file">A file for the export of a document.</param>
        public void Export(StiReport report, string file)
        {
            Export(report, file, false, StiGuiMode.Gdi);
        }

		/// <summary>
		/// Exports a document to the stream.
		/// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="file">A file for the export of a document.</param>
        public void Export(StiReport report, string file, StiGuiMode guiMode)
		{
			Export(report, file, false, guiMode);
		}

        /// <summary>
        /// Exports a document to the stream.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="file">A file for the export of a document.</param>
        /// <param name="sendEMail">A parameter indicating whether is exported report will be sended via e-mail.</param>
        public void Export(StiReport report, string file, bool sendEMail)
        {
            Export(report, file, sendEMail, StiGuiMode.Gdi);
        }

		/// <summary>
		/// Exports a document to the stream.
		/// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="file">A file for the export of a document.</param>
        /// <param name="sendEMail">A parameter indicating whether is exported report will be sended via e-mail.</param>
		public abstract void Export(StiReport report, string file, bool sendEMail, StiGuiMode guiMode);

		/// <summary>
		/// Exports a document to the stream with dialog of saving file.
		/// </summary>
        /// <param name="report">A report which is to be exported.</param>
		public void Export(StiReport report)
		{
			Export(report, null, false);
		}

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public abstract void ExportTo(StiReport report, Stream stream, StiExportSettings settings);

		protected void ProcessFile(bool sendEMail, bool openAfterExport, string fileName, StiReport report)
		{
		    if (IsStopped)
		        File.Delete(fileName);

		    else
		    {
		        if (sendEMail)
		        {
		            var a = new Stimulsoft.Report.Events.StiSendEMailEventArgs(report, string.Empty, 
		                report.ReportAlias != null && report.ReportAlias.Trim().Length > 0 ? report.ReportAlias : report.ReportName,
		                report.ReportDescription, fileName);

		            StiOptions.Engine.GlobalEvents.InvokeSendEMailProcess(this, a);

		            if (a.CallStandardHandler)
		                SendEMail(a.Recipient, a.Subject, a.Body, a.FilePath);
		        }
		        else
		        {
		            if (openAfterExport)
		                OpenFile(fileName);
		        }
		    }
		}

        protected void SendEMail(string subject, string body, string filePath)
        {
            SendEMail(string.Empty, subject, body, filePath);
        }

		protected void SendEMail(string recipient, string subject, string body, string filePath)
		{
            var str = Directory.GetCurrentDirectory();

			try
			{                
                MAPI.SendEMail(recipient, subject, body, filePath);
			}
			catch (Exception e)
			{
				StiLogService.Write(this.GetType(), "Send E-mail");
				StiLogService.Write(this.GetType(), e);

                StiExceptionProvider.Show(e);
			}
            finally
            {
                if (StiOptions.Engine.AllowSetCurrentDirectory)
                {
                    Directory.SetCurrentDirectory(str);
                }
            }
		}

		protected void OpenFile(string fileName)
		{
			try
			{
				StiProcess.Start($"\"{fileName}\"");
			}
			catch (Exception e)
			{
				StiLogService.Write(this.GetType(), "Open file");
				StiLogService.Write(this.GetType(), e);

                var exception = e as Win32Exception;

			    if (exception != null && exception.NativeErrorCode == 0x00000483)
			        MessageBox.Show(e.Message, StiLocalization.Get("Export", "OpenAfterExport"));

			    else
			        StiExceptionProvider.Show(e);
			}
		}

		internal string CorrectFileName(string str)
		{
			var error = "/\\:*?\"<>|";
			var chars = str.ToCharArray();

			for (var index = 0; index < chars.Length; index ++)
			{
                var i = error.IndexOf(chars[index]);
				if (i != -1)
				    chars[index] = '_';
			}
			return new string(chars);
		}

        internal string GetFileName(StiReport report, bool sendEMail)
		{
            if (sendEMail && StiOptions.Export.AutoGenerateFileNameInSendEMail)
            {
                var fileName = Path.GetTempFileName(); 
                fileName = Path.ChangeExtension(fileName, this.DefaultExtension);

                return Path.Combine(Path.GetTempPath(), fileName);
            }
			using (var sf = new SaveFileDialog())
			{
				#region Get ExportSaveLoadPath
				StiSettings.Load();
				var path = StiOptions.Viewer.Windows.ExportSaveLoadPath;
				if (string.IsNullOrEmpty(path))
					path = StiSettings.GetStr("Viewer", "ExportSaveLoadPath", string.Empty);

			    if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
			        sf.InitialDirectory = path;
			    #endregion

				sf.Filter = GetFilter();
				sf.RestoreDirectory = true;
				sf.Title = StiHint.CreateHint(ExportNameInMenu);
				sf.FileName = CorrectFileName(report.ReportAlias);
				sf.DefaultExt = this.DefaultExtension;
						
				if (sf.ShowDialog() == DialogResult.OK && sf.FileName.Length > 0)
				{
                    var temp = sf.FileName;
				    if (sf.FileName.LastIndexOf("." + sf.DefaultExt, StringComparison.InvariantCulture) == -1)
				        temp += "." + sf.DefaultExt;

				    #region Set ExportSaveLoadPath
					path = StiOptions.Viewer.Windows.ExportSaveLoadPath;
					if (string.IsNullOrEmpty(path))
					{
						StiSettings.Load();
                        StiSettings.Set("Viewer", "ExportSaveLoadPath", Path.GetDirectoryName(temp));
						StiSettings.Save();
					}
					#endregion

					#region FileName
					var fileName = temp;
                    if (MultipleFiles)
                    {
                        var ext = Path.GetExtension(fileName).Substring(1);
                        fileName = GetOrderFileName(fileName.Substring(0, fileName.Length - ext.Length - 1), 1, report.RenderedPages.Count, ext);
                    }
					#endregion

					return fileName;
				}
			}
			return null;
		}

        public static string GetOrderFileName(string baseName, int index, int totalPagesCount, string extension)
        {
            var order = string.Empty;
            if (totalPagesCount > 1)
            {
                var power = 1;
                while (totalPagesCount >= 10)
                {
                    power++;
                    totalPagesCount /= 10;
                }
                order = index.ToString().PadLeft(power, '0');
            }

            return string.IsNullOrWhiteSpace(extension) ? order : $"{baseName}{order}.{extension}";
        }

		/// <summary>
		/// Returns the filter of all available services which serves for saving, loading a document.
		/// </summary>
		/// <returns>A Filter.</returns>
		public static string GetFilters()
		{
			var filter = "";
			var first = true;

            foreach (var service in StiOptions.Services.Exports.Where(s => s.ServiceEnabled))
			{
				if (!first)filter += '|';
				filter += service.GetFilter();
				first = false;
			}
			return filter;
		}

		/// <summary>
		/// Returns a filter for files of the export format.
		/// </summary>
		/// <returns>String with filter.</returns>
		public abstract string GetFilter();
		#endregion

		#region Properties
	    public object OwnerWindow { get; set; }

	    public IStiProgressInformation Progress { get; set; }

	    private string statusString = "";
        /// <summary>
        /// Gets or sets a status string for the progress information. 
        /// </summary>
		public string StatusString
		{
			get
			{
				return statusString;
			}
			set
			{
			    if (statusString == value) return;

			    statusString = value;
			    if (Progress != null)
			        Progress.Update(statusString);
			}
		}

        private bool isStopped;
        public bool IsStopped
        {
            get
            {
                return Progress != null && Progress.IsBreaked || isStopped;
            }
            set
            {
                isStopped = value;
            }
        }

        [Obsolete("IsStoped property is obsolete. Please use IsStopped property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsStoped
		{
			get
			{
                return IsStopped;

            }
            set
            {
                IsStopped = value;
            }
		}

		public abstract StiExportFormat ExportFormat { get; }

		/// <summary>
        /// Gets an export name in the context menu.
		/// </summary>
		public abstract string ExportNameInMenu { get; }

		/// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public abstract string DefaultExtension { get; }

	    /// <summary>
		/// Gets a value indicating a number of files in exported document as a result of export
		/// of one page of the rendered report.
		/// </summary>
		public virtual bool MultipleFiles { get; set; }

	    /// <summary>
        /// Gets a position of the export in the context menu.
		/// 0-PDF, 1-HTML, 2-RTF, 3-XML, 4-EXCEL, 5-EXCELXML, 6-TEXT
		/// 10-BMP, 11-GIF, 12-JPEG, 13-PNG, 14-TIFF, 20-EMF
		/// </summary>
		public abstract int Position { get; }

	    /// <summary>
	    /// Gets a group of the export in the context menu.
	    /// </summary>
	    public abstract string GroupCategory { get; }

	    protected int RenderedPagesCount { get; set; }

	    internal int CurrentPassNumber { get; set; }

	    internal int MaximumPassNumber { get; set; }
        #endregion
    }
}
