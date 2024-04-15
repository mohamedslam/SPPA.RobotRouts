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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the Json export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceJson.png")]
	public class StiJsonExportService : StiExportService
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
        public override string DefaultExtension => "json";

        public override StiExportFormat ExportFormat => StiExportFormat.Json;

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
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeJsonFile");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportJson(report, stream, settings as StiDataExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }
        
        /// <summary>
        /// Exports a rendered report to the Json file. Also the file can be sent via e-mail.
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
        /// Returns a filter for json files.
        /// </summary>
        /// <returns>Returns a filter for json files.</returns>
		public override string GetFilter() => StiLocalization.Get("FileFilters", "JsonFiles");
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
        /// Exports a rendered report to the Json file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportJson(StiReport report, string fileName)
		{
			StiFileUtils.ProcessReadOnly(fileName);
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                ExportJson(report, stream);
                stream.Flush();
                stream.Close();
            }
		}

        /// <summary>
        /// Exports a rendered report to the Json file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        public void ExportJson(StiReport report, Stream stream)
		{
            ExportJson(report, stream, new StiDataExportSettings());
        }
    
		/// <summary>
        /// Exports a rendered report to the Json file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="settings">A export settings.</param>
        public void ExportJson(StiReport report, Stream stream, StiDataExportSettings settings)
		{
			StiLogService.Write(this.GetType(), "Export report to Json format");

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

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
                var stNum = string.Empty;
				int num = 0;
				while (true)
				{
					if (!hs.Contains(st + stNum)) break;
					num ++;
					stNum = num.ToString();
				}

				st += stNum;
				matrix.Fields[index].Name = st;
				hs.Add(st, st);
			}
			#endregion

            var jarr = new JArray();

            #region Write records
            StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
            CurrentPassNumber = 2;

            for (int rowIndex = 0; rowIndex < matrix.DataArrayLength; rowIndex++)
			{
				InvokeExporting(rowIndex, matrix.DataArrayLength, CurrentPassNumber, MaximumPassNumber);
				if (IsStopped) return;

                var jobj = new JObject();
				for (int columnIndex = 0; columnIndex < matrix.Fields.Length; columnIndex ++)
				{
					string text = matrix.Fields[columnIndex].DataArray[rowIndex];

					#region convert text
					switch (matrix.Fields[columnIndex].Info[0])
					{
                        case (int)StiExportDataType.Long:
                        case (int)StiExportDataType.Int:
                            {
                                int val;
                                if (int.TryParse(text, out val))
                                {
                                    jobj.AddPropertyInt(matrix.Fields[columnIndex].Name, val);
                                    continue;
                                }
                            }
                            break;

                        case (int)StiExportDataType.Float:
                            {
                                float val;
                                if (float.TryParse(text.Replace(".", ",").Replace(",", separator), out val))
                                {
                                    jobj.AddPropertyFloat(matrix.Fields[columnIndex].Name, val, 0f);
                                    continue;
                                }
                            }
                            break;

                        case (int)StiExportDataType.Double:
                            {
                                double val;
                                if (double.TryParse(text.Replace(".", ",").Replace(",", separator), out val))
                                {
                                    jobj.AddPropertyDouble(matrix.Fields[columnIndex].Name, val);
                                    continue;
                                }
                            }
                            break;

                        case (int)StiExportDataType.Date:
                            {
                                DateTime val;
                                if (DateTime.TryParse(text, out val))
                                {
                                    jobj.AddPropertyDateTime(matrix.Fields[columnIndex].Name, val);
                                    continue;
                                }
                            }
                            break;
                    }

                     //case (int)StiExportDataType.String:
                     jobj.AddPropertyString(matrix.Fields[columnIndex].Name, text);
                    #endregion
                }
                jarr.Add(jobj);
			}
            #endregion

            var mainObj = new JObject();

            var tableName = StiNameValidator.CorrectName(string.IsNullOrWhiteSpace(report.ReportAlias) ? report.ReportName : report.ReportAlias);
            if (!string.IsNullOrWhiteSpace(settings.TableName)) tableName = settings.TableName;
            if (string.IsNullOrWhiteSpace(tableName)) tableName = "Table1";
            mainObj.Add(tableName, jarr);

            var jsonStr = mainObj.ToString();
            var buf = Encoding.UTF8.GetBytes(jsonStr);
            stream.Write(buf, 0, buf.Length);
            stream.Flush();

			if (matrix != null)
			{
				matrix.Clear();
				matrix = null;
			}
		}
		#endregion
	}
}