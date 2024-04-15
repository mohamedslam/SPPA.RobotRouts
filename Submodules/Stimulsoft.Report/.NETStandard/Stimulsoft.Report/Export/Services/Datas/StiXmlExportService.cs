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

using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the XML export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceXml.png")]
	public class StiXmlExportService : StiExportService
    {
		#region Fields
		private StiReport report;
		private string fileName;
		private bool sendEMail;
		private StiGuiMode guiMode;
		#endregion

		#region StiExportService override
		/// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension => "xml";

		public override StiExportFormat ExportFormat => StiExportFormat.Xml;

		/// <summary>
        /// Gets a group of the export in the context menu.
		/// </summary>
		public override string GroupCategory => "Data";

		/// <summary>
        /// Gets a position of the export in the context menu.
		/// </summary>
		public override int Position => (int)StiExportPosition.Data;

        /// <summary>
        /// Gets a name of the export in the context menu.
        /// </summary>
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeXmlFile");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportXml(report, stream, settings as StiDataExportSettings);
			InvokeExporting(100, 100, 1, 1);
		}
        
        /// <summary>
        /// Exports a rendered report to the XML file. Also the file can be sent via e-mail.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
		{
            using (var form = StiGuiOptions.GetExportFormRunner("StiXmlExportSetupForm", guiMode, this.OwnerWindow))
			{
                form["OpenAfterExportEnabled"] = !sendEMail;

                this.report = report;
                this.fileName = fileName;
                this.sendEMail = sendEMail;
                this.guiMode = guiMode;

                form.Complete += Form_Complete;
                form.ShowDialog();
			}
		}

		/// <summary>
		/// Gets a value indicating a number of files in exported document as a result of export
		/// of one page of the rendered report.
		/// </summary>        
		public override bool MultipleFiles => false;

        /// <summary>
        /// Returns a filter for xml files.
        /// </summary>
        /// <returns>Returns a filter for xml files.</returns>
		public override string GetFilter() => StiLocalization.Get("FileFilters", "XmlFiles");
        #endregion

        #region Handlers
        private void Form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
        {
			if (e.DialogResult)
			{
				if (string.IsNullOrEmpty(fileName))
					fileName = base.GetFileName(report, sendEMail);

				if (fileName != null)
				{
					StiFileUtils.ProcessReadOnly(fileName);

					try
					{
						using (var stream = new FileStream(fileName, FileMode.Create))
						{
							StartProgress(guiMode);

							var settings = new StiDataExportSettings();

							base.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
						}
					}
					catch (Exception ex)
					{
						throw ex;
					}
				}
			}
			else IsStopped = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exports a rendered report to the XML file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public virtual void ExportXml(StiReport report, string fileName)
		{
			StiFileUtils.ProcessReadOnly(fileName);
			using (var stream = new FileStream(fileName, FileMode.Create))
			{
				ExportXml(report, stream);
				stream.Flush();
				stream.Close();
			}
		}

        /// <summary>
        /// Exports a rendered report to the XML file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        public virtual void ExportXml(StiReport report, Stream stream)
		{
            ExportXml(report, stream, new StiDataExportSettings());
        }
    
		/// <summary>
        /// Exports a rendered report to the XML file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="settings">A export settings.</param>
        public virtual void ExportXml(StiReport report, Stream stream, StiDataExportSettings settings)
		{
			StiLogService.Write(this.GetType(), "Export report to Xml format");

            #region Export Dashboard
            if (!report.IsDocument && report.GetCurrentPage() is IStiDashboard)
            {
                StiDashboardExport.Export(report, stream, settings);
                return;
            }
            #endregion

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

			//var pageRange = settings.PageRange;
			var mode = settings.DataExportMode;
            #endregion

			var separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            CurrentPassNumber = 0;
            MaximumPassNumber = 3;

			var pages = StiPagesRange.All.GetSelectedPages(report.RenderedPages);
			var matrix = new StiMatrix(pages, false, this);
			matrix.ScanComponentsPlacement(false);
			if (IsStopped) return;

			matrix.PrepareDocument(this, mode);

			#region Check fields names
			var hs = new Hashtable();
			for (int index = 0; index < matrix.Fields.Length; index ++)
			{
				var st = matrix.Fields[index].Name;
                if (StiOptions.Export.Xml.ConvertTagsToUpperCase)
                    st = st.ToUpper(CultureInfo.InvariantCulture);

				var stNum = string.Empty;
				var num = 0;
				while (true)
				{
					if (!hs.Contains(st + stNum)) break;
					num++;
					stNum = num.ToString();
				}

				st += stNum;
				matrix.Fields[index].Name = st;
				hs.Add(st, st);
			}
			#endregion

			var title = StiNameValidator.CorrectName(string.IsNullOrWhiteSpace(report.ReportAlias) ? report.ReportName : report.ReportAlias);
		    if (string.IsNullOrWhiteSpace(title))
                title = "Report";

            var dataSet = new DataSet(title)
            {
                EnforceConstraints = false
            };

            var dataTable = new DataTable();
            if (!string.IsNullOrWhiteSpace(settings.TableName))
                dataTable.TableName = settings.TableName;

            for (int columnIndex = 0; columnIndex < matrix.Fields.Length; columnIndex ++)
			{
				switch (matrix.Fields[columnIndex].Info[0])
				{
					case (int)StiExportDataType.String:
						dataTable.Columns.Add(matrix.Fields[columnIndex].Name, typeof(string));
						break;

					case (int)StiExportDataType.Int:
						dataTable.Columns.Add(matrix.Fields[columnIndex].Name, typeof(int));
						break;

					case (int)StiExportDataType.Long:
						dataTable.Columns.Add(matrix.Fields[columnIndex].Name, typeof(long));
						break;

					case (int)StiExportDataType.Float:
						dataTable.Columns.Add(matrix.Fields[columnIndex].Name, typeof(float));
						break;

					case (int)StiExportDataType.Double:
						dataTable.Columns.Add(matrix.Fields[columnIndex].Name, typeof(double));
						break;

					case (int)StiExportDataType.Date:
						dataTable.Columns.Add(matrix.Fields[columnIndex].Name, typeof(DateTime));
						break;
				}
			}
			dataSet.Tables.Add(dataTable);

			#region Write records
			StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
            CurrentPassNumber = 2;

            for (int rowIndex = 0; rowIndex < matrix.DataArrayLength; rowIndex++)
			{
				InvokeExporting(rowIndex, matrix.DataArrayLength, CurrentPassNumber, MaximumPassNumber);
				if (IsStopped) return;

				var row = dataTable.NewRow();
				for (int columnIndex = 0; columnIndex < matrix.Fields.Length; columnIndex ++)
				{
					string text = matrix.Fields[columnIndex].DataArray[rowIndex];

					#region convert text
					switch (matrix.Fields[columnIndex].Info[0])
					{
						case (int)StiExportDataType.String:
						{
							row[columnIndex] = text;
						}
						break;

						case (int)StiExportDataType.Int:
						{
							int val;
						    if (int.TryParse(text, out val))
						    {
                                row[columnIndex] = val;
						    }
						}
                        break;

						case (int)StiExportDataType.Long:
						{
							long val;
                            if (long.TryParse(text, out val))
						    {
                                row[columnIndex] = val;
						    }
						}
                        break;

						case (int)StiExportDataType.Float:
						{
							float val;
						    if (float.TryParse(text.Replace(".", ",").Replace(",", separator), out val))
						    {
						        row[columnIndex] = val;
						    }
						}
                        break;

						case (int)StiExportDataType.Double:
						{
							double val;
						    if (double.TryParse(text.Replace(".", ",").Replace(",", separator), out val))
						    {
						        row[columnIndex] = val;
						    }
						}
                        break;

						case (int)StiExportDataType.Date:
						{
							DateTime val;
                            if (DateTime.TryParse(text, out val))
						    {
                                row[columnIndex] = val;
						    }
						}
                        break;
					}
					#endregion
				}
				dataTable.Rows.Add(row);
			}
            #endregion

            WriteDataSetToStream(dataSet, stream, settings);

            if (matrix != null)
			{
				matrix.Clear();
				matrix = null;
			}
		}

        protected virtual void WriteDataSetToStream(DataSet dataSet, Stream stream, StiDataExportSettings settings)
        {
            var xmlWriter = new XmlTextWriter(stream, Encoding.UTF8)
            {
                Indentation = 1,
                Formatting = Formatting.Indented
            };
            if (StiOptions.Export.Xml.WriteXmlDocumentDeclaration)
                xmlWriter.WriteStartDocument(true);

            dataSet.WriteXml(xmlWriter);
            dataSet.Dispose();

            xmlWriter.Flush();
        }
		#endregion
	}
}