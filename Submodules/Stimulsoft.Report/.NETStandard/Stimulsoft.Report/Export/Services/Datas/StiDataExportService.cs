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
using Stimulsoft.Report.Helpers;
using System;
using System.IO;
using System.Text;

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the Csv export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceData.png")]
	public class StiDataExportService : StiExportService
    {
        #region Fields
        private StiExportSettings exportSettings;
        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        #endregion

        #region StiExportService override
        /// <summary>
        /// Gets or sets a default extension of export. 
        /// </summary>
        public override string DefaultExtension
		{
			get
			{
                if (exportSettings is StiDbfExportSettings) return "dbf";
                if (exportSettings is StiDifExportSettings) return "dif";
                if (exportSettings is StiSylkExportSettings) return "slk";
                if (exportSettings is StiXmlExportSettings) return "xml";
                if (exportSettings is StiJsonExportSettings) return "json";
                if (exportSettings is StiDataExportSettings)
                {
                    var dataType = (exportSettings as StiDataExportSettings).DataType;
                    if (dataType == StiDataType.Dbf) return "dbf";
                    if (dataType == StiDataType.Dif) return "dif";
                    if (dataType == StiDataType.Sylk) return "slk";
                    if (dataType == StiDataType.Xml) return "xml";
                    if (dataType == StiDataType.Json) return "json";
                }

                return "csv";
			}
		}


		public override StiExportFormat ExportFormat
		{
            get
            {
                if (exportSettings is StiCsvExportSettings) return StiExportFormat.Csv;
                if (exportSettings is StiDbfExportSettings) return StiExportFormat.Dbf;
                if (exportSettings is StiDifExportSettings) return StiExportFormat.Dif;
                if (exportSettings is StiSylkExportSettings) return StiExportFormat.Sylk;
                if (exportSettings is StiXmlExportSettings) return StiExportFormat.Xml;
                if (exportSettings is StiJsonExportSettings) return StiExportFormat.Json;
                if (exportSettings is StiDataExportSettings)
                {
                    var dataType = (exportSettings as StiDataExportSettings).DataType;
                    if (dataType == StiDataType.Csv) return StiExportFormat.Csv;
                    if (dataType == StiDataType.Dbf) return StiExportFormat.Dbf;
                    if (dataType == StiDataType.Dif) return StiExportFormat.Dif;
                    if (dataType == StiDataType.Sylk) return StiExportFormat.Sylk;
                    if (dataType == StiDataType.Xml) return StiExportFormat.Xml;
                    if (dataType == StiDataType.Json) return StiExportFormat.Json;
                }

                return StiExportFormat.Data;
			}
		}

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
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeDataFile");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportData(report, stream, settings as StiDataExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }

		/// <summary>
		/// Exports a rendered report to a csv file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="fileName">A name of the file for exporting a rendered report.</param>
		/// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
		{
            using (var form = StiGuiOptions.GetExportFormRunner("StiDataSetupForm", guiMode, this.OwnerWindow))
			{
                form["CurrentPage"] = report.CurrentPrintPage;
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
		/// Returns the filter of all available services which serves for saving, loading a document.
		/// </summary>
		/// <returns>Filter.</returns>
		public override string GetFilter() => StiLocalization.Get("FileFilters", "CsvFiles");
        #endregion

        #region Handlers
        private void Form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
        {
            if (e.DialogResult)
            {
                var exportFormat = (StiExportFormat)form["ExportFormat"];

                #region Dbf

                if (exportFormat == StiExportFormat.Dbf)
                {
                    var dbfExportService = new StiDbfExportService();

                    if (string.IsNullOrEmpty(fileName))
                        fileName = dbfExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        var codePage = (StiDbfCodePages)Enum.Parse(typeof(StiDbfCodePages),
                            StiDbfExportService.codePageCodes[(int)form["EncodingSelectedIndex"], 1].ToString());

                        StiFileUtils.ProcessReadOnly(fileName);
                        var stream = new FileStream(fileName, FileMode.Create);

                        dbfExportService.StartProgress(guiMode);

                        var settings = new StiDbfExportSettings
                        {
                            PageRange = form["PagesRange"] as StiPagesRange,
                            CodePage = codePage
                        };

                        exportSettings = settings;

                        dbfExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                    }
                }

                #endregion

                #region Xml
                else if (exportFormat == StiExportFormat.Xml)
                {
                    var xmlExportService = new StiXmlExportService();

                    if (string.IsNullOrEmpty(fileName))
                        fileName = xmlExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);
                        var stream = new FileStream(fileName, FileMode.Create);

                        xmlExportService.StartProgress(guiMode);

                        var settings = new StiXmlExportSettings();

                        int mode = (int)form["DataExportMode"];
                        if (mode == 1) settings.DataExportMode = StiDataExportMode.DataAndHeadersFooters;
                        if (mode == 2) settings.DataExportMode = StiDataExportMode.AllBands;

                        exportSettings = settings;

                        xmlExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                    }
                }
                #endregion

                #region Json
                else if (exportFormat == StiExportFormat.Json)
                {
                    var jsonExportService = new StiJsonExportService();

                    if (string.IsNullOrEmpty(fileName))
                        fileName = jsonExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);
                        var stream = new FileStream(fileName, FileMode.Create);

                        jsonExportService.StartProgress(guiMode);

                        var settings = new StiJsonExportSettings();

                        int mode = (int)form["DataExportMode"];
                        if (mode == 1) settings.DataExportMode = StiDataExportMode.DataAndHeadersFooters;
                        if (mode == 2) settings.DataExportMode = StiDataExportMode.AllBands;

                        exportSettings = settings;

                        jsonExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                    }
                }
                #endregion

                #region Dif

                else if (exportFormat == StiExportFormat.Dif)
                {
                    var difExportService = new StiDifExportService();

                    if (string.IsNullOrEmpty(fileName))
                        fileName = difExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);
                        var stream = new FileStream(fileName, FileMode.Create);

                        difExportService.StartProgress(guiMode);

                        var settings = new StiDifExportSettings
                        {
                            PageRange = form["PagesRange"] as StiPagesRange,
                            ExportDataOnly = (bool)form["ExportDataOnly"],
                            Encoding = form["Encoding"] as Encoding,
                            UseDefaultSystemEncoding = (bool)form["UseDefaultSystemEncoding"]
                        };

                        exportSettings = settings;

                        difExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                    }
                }

                #endregion

                #region Sylk

                else if (exportFormat == StiExportFormat.Sylk)
                {
                    var sylkExportService = new StiSylkExportService();

                    if (string.IsNullOrEmpty(fileName))
                        fileName = sylkExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);
                        var stream = new FileStream(fileName, FileMode.Create);

                        sylkExportService.StartProgress(guiMode);

                        var settings = new StiSylkExportSettings
                        {
                            PageRange = form["PagesRange"] as StiPagesRange,
                            ExportDataOnly = (bool)form["ExportDataOnly"],
                            Encoding = form["Encoding"] as Encoding,
                            UseDefaultSystemEncoding = (bool)form["UseDefaultSystemEncoding"]
                        };

                        exportSettings = settings;

                        sylkExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                    }
                }

                #endregion

                #region Csv

                else
                {
                    var csvExportService = new StiCsvExportService();

                    if (string.IsNullOrEmpty(fileName))
                        fileName = csvExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);
                        var stream = new FileStream(fileName, FileMode.Create);

                        csvExportService.StartProgress(guiMode);

                        var settings = new StiCsvExportSettings
                        {
                            PageRange = form["PagesRange"] as StiPagesRange,
                            Separator = form["Separator"] as string,
                            Encoding = form["Encoding"] as Encoding,
                            SkipColumnHeaders = (bool)form["SkipColumnHeaders"]
                        };

                        int mode = (int)form["DataExportMode"];
                        if (mode == 1) settings.DataExportMode = StiDataExportMode.DataAndHeadersFooters;
                        if (mode == 2) settings.DataExportMode = StiDataExportMode.AllBands;

                        exportSettings = settings;

                        csvExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                    }
                }

                #endregion
            }
            else IsStopped = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exports a rendered report to a csv file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        public void ExportData(StiReport report, Stream stream, StiDataExportSettings settings)
		{
            exportSettings = settings;

            var dataType = settings != null ? settings.DataType : StiDataType.Csv;

            if (dataType == StiDataType.Dbf || settings is StiDbfExportSettings)
            {
                var dbfExportService = new StiDbfExportService();
                dbfExportService.ExportDbf(report, stream, settings);
            }
            else if (dataType == StiDataType.Dif || settings is StiDifExportSettings)
            {
                var difExportService = new StiDifExportService();
                difExportService.ExportDif(report, stream, settings);
            }
            else if (dataType == StiDataType.Sylk || settings is StiSylkExportSettings)
            {
                var sylkExportService = new StiSylkExportService();
                sylkExportService.ExportSylk(report, stream, settings);
            }
            else if (dataType == StiDataType.Xml || settings is StiXmlExportSettings)
            {
                var xmlExportService = new StiXmlExportService();
                xmlExportService.ExportXml(report, stream, settings);
            }
            else if (dataType == StiDataType.Json)
            {
                var jsonExportService = new StiJsonExportService();
                jsonExportService.ExportJson(report, stream, settings);
            }
            else
            {
                var csvExportService = new StiCsvExportService();
                csvExportService.ExportCsv(report, stream, settings);
            }
		}
		#endregion
	}
}