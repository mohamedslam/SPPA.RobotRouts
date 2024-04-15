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
using System.IO;
using System.Threading.Tasks;
using Stimulsoft.Base;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Export;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Export;

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        #region Methods
        /// <summary>
        /// Exports a rendered report into a file with dialog using the provider.
        /// </summary>
        /// <param name="service">A provider that exports a rendered report.</param>
        /// <param name="path">A file to export the rendered report.</param>
        public async Task<StiReport> ExportDocumentAsync(StiExportService service, string path)
        {
            return await Task.Run(() => ExportDocument(service, path));
        }

        /// <summary>
        /// Exports a rendered report into a file with dialog using the provider.
        /// </summary>
        /// <param name="service">A provider that exports a rendered report.</param>
        /// <param name="path">A file to export the rendered report.</param>
        public StiReport ExportDocument(StiExportService service, string path)
        {
            return ExportDocument(service, path, false);
        }

        /// <summary>
        /// Exports a rendered report into a file with dialog using the provider.
        /// </summary>
        /// <param name="service">A provider that exports a rendered report.</param>
        /// <param name="path">A file to export the rendered report.</param>
        /// <param name="sendEMail">If this parameter is true then the exported report will be sent via email.</param>
        public async Task<StiReport> ExportDocumentAsync(StiExportService service, string path, bool sendEMail)
        {
            return await Task.Run(() => ExportDocument(service, path, sendEMail));
        }

        /// <summary>
        /// Exports a rendered report into a file with dialog using the provider.
        /// </summary>
        /// <param name="service">A provider that exports a rendered report.</param>
        /// <param name="path">A file to export the rendered report.</param>
        /// <param name="sendEMail">If this parameter is true then the exported report will be sent via email.</param>
        public StiReport ExportDocument(StiExportService service, string path, bool sendEMail)
        {
            try
            {
                StiLogService.Write(typeof(StiReport), "Exporting rendered report");
                service.Export(this, path, sendEMail);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Exporting rendered report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }

            return this;
        }

        /// <summary>
        /// Exports a rendered report into the file without dialog using the provider.
        /// </summary>
        /// <param name="exportFormat">A format for the document which will be exported to.</param>
        /// <param name="file">A file to export the rendered report.</param>
        public async Task<StiReport> ExportDocumentAsync(StiExportFormat exportFormat, string file)
        {
            return await Task.Run(() => ExportDocument(exportFormat, file));
        }

        /// <summary>
        /// Exports a rendered report into the file without dialog using the provider.
        /// </summary>
        /// <param name="exportFormat">A format for the document which will be exported to.</param>
        /// <param name="file">A file to export the rendered report.</param>
        public StiReport ExportDocument(StiExportFormat exportFormat, string file)
        {
            return ExportDocument(exportFormat, file, null);
        }

        /// <summary>
        /// Exports a rendered report into the file without dialog using the provider.
        /// </summary>
        /// <param name="exportFormat">A format for the document which will be exported to.</param>
        /// <param name="file">A file to export the rendered report.</param>
        public async Task<StiReport> ExportDocumentAsync(StiExportFormat exportFormat, string file, StiExportSettings settings)
        {
            return await Task.Run(() => ExportDocument(exportFormat, file, settings));
        }

        /// <summary>
        /// Exports a rendered report into the file without dialog using the provider.
        /// </summary>
        /// <param name="exportFormat">A format for the document which will be exported to.</param>
        /// <param name="file">A file to export the rendered report.</param>
        public StiReport ExportDocument(StiExportFormat exportFormat, string file, StiExportSettings settings)
        {
            #region Check file for MultipleFiles
            if ((RenderedPages.Count > 1) &&
                (exportFormat == StiExportFormat.ImageBmp ||
                exportFormat == StiExportFormat.ImageEmf ||
                exportFormat == StiExportFormat.ImageGif ||
                exportFormat == StiExportFormat.ImageJpeg ||
                exportFormat == StiExportFormat.ImagePcx ||
                exportFormat == StiExportFormat.ImagePng ||
                exportFormat == StiExportFormat.ImageSvg ||
                exportFormat == StiExportFormat.ImageSvgz ||
                exportFormat == StiExportFormat.ImageTiff))
            {
                var name = file;
                var extension = string.Empty;
                if (Path.HasExtension(file))
                {
                    extension = Path.GetExtension(file).Substring(1);
                    name = Path.ChangeExtension(file, "");
                    name = name.Substring(0, name.Length - 1);
                }

                var firstOrder = StiExportService.GetOrderFileName(null, 1, RenderedPages.Count, null);
                if (!name.EndsWith(firstOrder))
                {
                    file = $"{name}{firstOrder}.{extension}";
                }
            }
            #endregion

            StiFileUtils.ProcessReadOnly(file);
            var stream = new FileStream(file, FileMode.Create, FileAccess.ReadWrite);
            ExportDocument(exportFormat, stream, settings);
            stream.Close();

            return this;
        }
        
        /// <summary>
        /// Exports a rendered report into the stream without dialog using the provider. 
        /// </summary>
        /// <param name="exportFormat">A format for the document which will be exported to.</param>
        /// <param name="stream">A file to export the rendered report.</param>
        public async Task<StiReport> ExportDocumentAsync(StiExportFormat exportFormat, Stream stream)
        {
            return await Task.Run(() => ExportDocument(exportFormat, stream));
        }

        /// <summary>
        /// Exports a rendered report into the stream without dialog using the provider. 
        /// </summary>
        /// <param name="exportFormat">A format for the document which will be exported to.</param>
        /// <param name="stream">A file to export the rendered report.</param>
        public StiReport ExportDocument(StiExportFormat exportFormat, Stream stream)
        {
            return ExportDocument(exportFormat, stream, null);
        }
        
        /// <summary>
        /// Exports a rendered report into the stream without dialog using the provider. 
        /// </summary>
        /// <param name="exportFormat">A format for the document which will be exported to.</param>
        /// <param name="stream">A file to export the rendered report.</param>
        public async Task<StiReport> ExportDocumentAsync(StiExportFormat exportFormat, Stream stream, StiExportSettings settings)
        {
            return await Task.Run(() => ExportDocument(exportFormat, stream, settings));
        }

        /// <summary>
        /// Exports a rendered report into the stream without dialog using the provider. 
        /// </summary>
        /// <param name="exportFormat">A format for the document which will be exported to.</param>
        /// <param name="stream">A file to export the rendered report.</param>
        public StiReport ExportDocument(StiExportFormat exportFormat, Stream stream, StiExportSettings settings)
        {
            return ExportDocument(exportFormat, null, stream, settings);
        }
        
        /// <summary>
        /// Internal use only.
        /// </summary>
        public StiReport ExportDocument(StiExportFormat exportFormat, StiExportService exportService, Stream stream, StiExportSettings settings)
        {
            InvokeExporting(exportFormat);

            var stateDesigner = this.Designer;
            var stateForceDesigningMode = this.Info.ForceDesigningMode;
            var stateIsPageDesigner = this.IsPageDesigner;

            this.Designer = null;
            this.Info.ForceDesigningMode = false;
            this.IsPageDesigner = false;

            try
            {
                StiLogService.Write(typeof(StiReport), "Exporting rendered report");

                var processExportEventArgs = new StiProcessExportEventArgs(exportFormat, exportService, stream, settings);
                StiOptions.Engine.GlobalEvents.InvokeProcessExport(this, processExportEventArgs);

                if (!processExportEventArgs.Processed)
                {
                    #region Dashboard export
                    var dashboardSettings = settings as IStiDashboardExportSettings;
                    if (dashboardSettings != null)
                    {
                        var imageDashboardSettings = settings as IStiImageDashboardExportSettings;
                        if (imageDashboardSettings != null)
                        {
                            #region Set image type
                            switch (exportFormat)
                            {
                                case StiExportFormat.ImageBmp:
                                    imageDashboardSettings.ImageType = StiImageType.Bmp;
                                    break;

                                case StiExportFormat.ImageEmf:
                                    imageDashboardSettings.ImageType = StiImageType.Emf;
                                    break;

                                case StiExportFormat.ImageGif:
                                    imageDashboardSettings.ImageType = StiImageType.Gif;
                                    break;

                                case StiExportFormat.ImageJpeg:
                                    imageDashboardSettings.ImageType = StiImageType.Jpeg;
                                    break;

                                case StiExportFormat.ImagePcx:
                                    imageDashboardSettings.ImageType = StiImageType.Pcx;
                                    break;

                                case StiExportFormat.ImagePng:
                                    imageDashboardSettings.ImageType = StiImageType.Png;
                                    break;

                                case StiExportFormat.ImageSvg:
                                    imageDashboardSettings.ImageType = StiImageType.Svg;
                                    break;

                                case StiExportFormat.ImageSvgz:
                                    imageDashboardSettings.ImageType = StiImageType.Svgz;
                                    break;

                                case StiExportFormat.ImageTiff:
                                    imageDashboardSettings.ImageType = StiImageType.Tiff;
                                    break;
                            }
                            #endregion
                        }

                        StiDashboardExport.Export(this, stream, dashboardSettings);
                    }
                    #endregion
                    else
                    {
                        switch (exportFormat)
                        {
                            #region Csv
                            case StiExportFormat.Csv:
                                if (exportService != null && (!(exportService is StiCsvExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var csvService = exportService as StiCsvExportService;
                                if (csvService == null) csvService = new StiCsvExportService();

                                if (settings == null) settings = new StiCsvExportSettings();
                                bool isCorrectSettingsCsv = (settings is StiCsvExportSettings) || ((settings is StiDataExportSettings) && (settings as StiDataExportSettings).DataType == StiDataType.Csv);
                                if (!isCorrectSettingsCsv)
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiCsvExportSettings is need to be used.");

                                csvService.ExportCsv(this, stream, settings as StiDataExportSettings);
                                break;
                            #endregion

                            #region Data
                            case StiExportFormat.Data:
                                if (exportService != null && (!(exportService is StiDataExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var dataExportService = exportService as StiDataExportService;
                                if (dataExportService == null) dataExportService = new StiDataExportService();

                                if (settings == null) settings = new StiDataExportSettings();
                                if (!(settings is StiDataExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiCsvExportSettings is need to be used.");

                                dataExportService.ExportTo(this, stream, settings as StiDataExportSettings);
                                break;
                            #endregion

                            #region Dif
                            case StiExportFormat.Dif:
                                if (exportService != null && (!(exportService is StiDifExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var difService = exportService as StiDifExportService;
                                if (difService == null) difService = new StiDifExportService();

                                if (settings == null) settings = new StiDifExportSettings();
                                bool isCorrectSettingsDif = (settings is StiDifExportSettings) || ((settings is StiDataExportSettings) && (settings as StiDataExportSettings).DataType == StiDataType.Dif);
                                if (!isCorrectSettingsDif)
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiDifExportSettings is need to be used.");

                                difService.ExportDif(this, stream, settings as StiDataExportSettings);
                                break;
                            #endregion

                            #region Sylk
                            case StiExportFormat.Sylk:
                                if (exportService != null && (!(exportService is StiSylkExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var sylkService = exportService as StiSylkExportService;
                                if (sylkService == null) sylkService = new StiSylkExportService();

                                if (settings == null) settings = new StiSylkExportSettings();
                                bool isCorrectSettingsSylk = (settings is StiSylkExportSettings) || ((settings is StiDataExportSettings) && (settings as StiDataExportSettings).DataType == StiDataType.Sylk);
                                if (!isCorrectSettingsSylk)
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiSylkExportSettings is need to be used.");

                                sylkService.ExportSylk(this, stream, settings as StiDataExportSettings);
                                break;
                            #endregion

                            #region Excel
                            case StiExportFormat.Excel:
                                if (exportService != null && (!(exportService is StiExcelExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var excelService = exportService as StiExcelExportService;
                                if (excelService == null) excelService = new StiExcelExportService();

                                if (settings == null) settings = new StiExcelExportSettings();
                                if (!(settings is StiExcelExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiExcelExportSettings is need to be used.");

                                excelService.ExportExcel(this, stream, settings as StiExcelExportSettings);
                                break;
                            #endregion

                            #region ExcelXml
                            case StiExportFormat.ExcelXml:
                                if (exportService != null && (!(exportService is StiExcelXmlExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var excelXmlService = exportService as StiExcelXmlExportService;
                                if (excelXmlService == null) excelXmlService = new StiExcelXmlExportService();

                                if (settings == null) settings = new StiExcelXmlExportSettings();
                                if (!((settings is StiExcelXmlExportSettings) || ((settings is StiExcelExportSettings) && ((settings as StiExcelExportSettings).ExcelType == StiExcelType.ExcelXml))))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiExcelXmlExportSettings is need to be used.");

                                excelXmlService.ExportExcel(this, stream, settings as StiExcelExportSettings);
                                break;
                            #endregion

                            #region Excel2007
                            case StiExportFormat.Excel2007:
                                if (exportService != null && (!(exportService is StiExcel2007ExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var excelService2 = exportService as StiExcel2007ExportService;
                                if (excelService2 == null) excelService2 = new StiExcel2007ExportService();

                                if (settings == null) settings = new StiExcel2007ExportSettings();
                                if (!((settings is StiExcel2007ExportSettings) || ((settings is StiExcelExportSettings) && ((settings as StiExcelExportSettings).ExcelType == StiExcelType.Excel2007))))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiExcel2007ExportSettings is need to be used.");

                                excelService2.ExportExcel(this, stream, settings as StiExcelExportSettings);
                                break;
                            #endregion

                            #region Word2007
                            case StiExportFormat.Word2007:
                                if (exportService != null && (!(exportService is StiWord2007ExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var wordService = exportService as StiWord2007ExportService;
                                if (wordService == null) wordService = new StiWord2007ExportService();

                                if (settings == null) settings = new StiWord2007ExportSettings();
                                if (!(settings is StiWord2007ExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiWord2007ExportSettings is need to be used.");

                                wordService.ExportWord(this, stream, settings as StiWord2007ExportSettings);
                                break;
                            #endregion

                            #region Dbf
                            case StiExportFormat.Dbf:
                                if (exportService != null && (!(exportService is StiDbfExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var dbfService = exportService as StiDbfExportService;
                                if (dbfService == null) dbfService = new StiDbfExportService();

                                if (settings == null) settings = new StiDbfExportSettings();
                                bool isCorrectSettingsDbf = (settings is StiDbfExportSettings) || ((settings is StiDataExportSettings) && (settings as StiDataExportSettings).DataType == StiDataType.Dbf);
                                if (!isCorrectSettingsDbf)
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiDbfExportSettings is need to be used.");

                                dbfService.ExportDbf(this, stream, settings as StiDbfExportSettings);
                                break;
                            #endregion

                            #region Mht
                            case StiExportFormat.Mht:
                                if (exportService != null && (!(exportService is StiMhtExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var mhtService = exportService as StiMhtExportService;
                                if (mhtService == null) mhtService = new StiMhtExportService();

                                if (settings == null) settings = new StiMhtExportSettings();
                                if (!(settings is StiMhtExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiMhtExportSettings is need to be used.");

                                mhtService.ExportMht(this, stream, settings as StiMhtExportSettings);
                                break;
                            #endregion

                            #region HtmlSpan
                            case StiExportFormat.HtmlSpan:
                                if (exportService != null && (!(exportService is StiHtmlExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var htmlSpanService = exportService as StiHtmlExportService;
                                if (htmlSpanService == null) htmlSpanService = new StiHtmlExportService();

                                if (settings == null) settings = new StiHtmlExportSettings();
                                if (!(settings is StiHtmlExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiHtmlExportSettings is need to be used.");

                                ((StiHtmlExportSettings)settings).ExportMode = StiHtmlExportMode.Span;

                                htmlSpanService.ExportHtml(this, stream, settings as StiHtmlExportSettings);
                                break;
                            #endregion

                            #region HtmlDiv
                            case StiExportFormat.HtmlDiv:
                                if (exportService != null && (!(exportService is StiHtmlExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var htmlDivService = exportService as StiHtmlExportService;
                                if (htmlDivService == null) htmlDivService = new StiHtmlExportService();

                                if (settings == null) settings = new StiHtmlExportSettings();
                                if (!(settings is StiHtmlExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiHtmlExportSettings is need to be used.");

                                ((StiHtmlExportSettings)settings).ExportMode = StiHtmlExportMode.Div;

                                htmlDivService.ExportHtml(this, stream, settings as StiHtmlExportSettings);
                                break;
                            #endregion

                            #region Html, HtmlTable
                            case StiExportFormat.Html:
                            case StiExportFormat.HtmlTable:
                                if (exportService != null && (!(exportService is StiHtmlExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var htmlTableService = exportService as StiHtmlExportService;
                                if (htmlTableService == null) htmlTableService = new StiHtmlExportService();

                                if (settings == null) settings = new StiHtmlExportSettings();
                                if (!(settings is StiHtmlExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiHtmlExportSettings is need to be used.");

                                if (exportFormat != StiExportFormat.Html)
                                {
                                    ((StiHtmlExportSettings)settings).ExportMode = StiHtmlExportMode.Table;
                                }

                                htmlTableService.ExportHtml(this, stream, settings as StiHtmlExportSettings);
                                break;
                            #endregion

                            #region Html5
                            case StiExportFormat.Html5:
                                if (exportService != null && (!(exportService is StiHtml5ExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var html5Service = exportService as StiHtml5ExportService;
                                if (html5Service == null) html5Service = new StiHtml5ExportService();

                                if (settings == null) settings = new StiHtml5ExportSettings();
                                if (!(settings is StiHtml5ExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiHtml5ExportSettings is need to be used.");

                                html5Service.ExportHtml(this, stream, settings as StiHtml5ExportSettings);
                                break;
                            #endregion

                            #region Image
                            case StiExportFormat.Image:
                                if (exportService != null && (!(exportService is StiImageExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var imageService = exportService as StiImageExportService;
                                if (imageService == null) imageService = new StiImageExportService();

                                if (settings == null) settings = new StiImageExportSettings();
                                if (!(settings is StiImageExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiImageExportSettings is need to be used.");

                                imageService.ExportTo(this, stream, settings as StiImageExportSettings);
                                break;
                            #endregion

                            #region ImageBmp
                            case StiExportFormat.ImageBmp:
                                if (exportService != null && (!(exportService is StiBmpExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var bmpService = exportService as StiBmpExportService;
                                if (bmpService == null) bmpService = new StiBmpExportService();

                                if (settings == null) settings = new StiBmpExportSettings();
                                if (!(settings is StiImageExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiBmpExportSettings is need to be used.");

                                bmpService.ExportImage(this, stream, settings as StiImageExportSettings);
                                break;
                            #endregion

                            #region ImageEmf
                            case StiExportFormat.ImageEmf:
                                if (exportService != null && (!(exportService is StiEmfExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var emfService = exportService as StiEmfExportService;
                                if (emfService == null) emfService = new StiEmfExportService();

                                if (settings == null) settings = new StiEmfExportSettings();
                                if (!(settings is StiImageExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiEmfExportSettings is need to be used.");

                                emfService.ExportImage(this, stream, settings as StiImageExportSettings);
                                break;
                            #endregion

                            #region ImageSvg
                            case StiExportFormat.ImageSvg:
                                if (exportService != null && (!(exportService is StiSvgExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var svgService = exportService as StiSvgExportService;
                                if (svgService == null) svgService = new StiSvgExportService();

                                if (settings == null) settings = new StiSvgExportSettings();
                                if (!(settings is StiImageExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiSvgExportSettings is need to be used.");

                                svgService.ExportImage(this, stream, settings as StiImageExportSettings);
                                break;
                            #endregion

                            #region ImageSvgz
                            case StiExportFormat.ImageSvgz:
                                if (exportService != null && (!(exportService is StiSvgzExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var svgzService = exportService as StiSvgzExportService;
                                if (svgzService == null) svgzService = new StiSvgzExportService();

                                if (settings == null) settings = new StiSvgzExportSettings();
                                if (!(settings is StiImageExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiSvgzExportSettings is need to be used.");

                                svgzService.ExportImage(this, stream, settings as StiImageExportSettings);
                                break;
                            #endregion

                            #region ImageGif
                            case StiExportFormat.ImageGif:
                                if (exportService != null && (!(exportService is StiGifExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var gifService = exportService as StiGifExportService;
                                if (gifService == null) gifService = new StiGifExportService();

                                if (settings == null) settings = new StiGifExportSettings();
                                if (!(settings is StiImageExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiGifExportSettings is need to be used.");

                                gifService.ExportImage(this, stream, settings as StiImageExportSettings);
                                break;
                            #endregion

                            #region ImageJpeg
                            case StiExportFormat.ImageJpeg:
                                if (exportService != null && (!(exportService is StiJpegExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var jpegService = exportService as StiJpegExportService;
                                if (jpegService == null) jpegService = new StiJpegExportService();

                                if (settings == null) settings = new StiJpegExportSettings();
                                if (!(settings is StiImageExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiJpegExportSettings is need to be used.");

                                jpegService.ExportImage(this, stream, settings as StiImageExportSettings);
                                break;
                            #endregion

                            #region ImagePng
                            case StiExportFormat.ImagePng:
                                if (exportService != null && (!(exportService is StiPngExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var pngService = exportService as StiPngExportService;
                                if (pngService == null) pngService = new StiPngExportService();

                                if (settings == null) settings = new StiPngExportSettings();
                                if (!(settings is StiImageExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiPngExportSettings is need to be used.");

                                pngService.ExportImage(this, stream, settings as StiImageExportSettings);
                                break;
                            #endregion

                            #region ImageTiff
                            case StiExportFormat.ImageTiff:
                                if (exportService != null && (!(exportService is StiTiffExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var tiffService = exportService as StiTiffExportService;
                                if (tiffService == null) tiffService = new StiTiffExportService();

                                if (settings == null) settings = new StiTiffExportSettings();
                                if (!(settings is StiImageExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiTiffExportSettings is need to be used.");

                                tiffService.ExportImage(this, stream, settings as StiImageExportSettings);
                                break;
                            #endregion

                            #region ImagePcx
                            case StiExportFormat.ImagePcx:
                                if (exportService != null && (!(exportService is StiPcxExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var pcxService = exportService as StiPcxExportService;
                                if (pcxService == null) pcxService = new StiPcxExportService();

                                if (settings == null) settings = new StiPcxExportSettings();
                                if (!(settings is StiImageExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiPcxExportSettings is need to be used.");

                                pcxService.ExportImage(this, stream, settings as StiImageExportSettings);
                                break;
                            #endregion

                            #region Pdf
                            case StiExportFormat.Pdf:
                                if (exportService != null && (!(exportService is StiPdfExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var pdfService = exportService as StiPdfExportService;
                                if (pdfService == null) pdfService = new StiPdfExportService();

                                if (settings == null) settings = new StiPdfExportSettings();
                                if (!(settings is StiPdfExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiPdfExportSettings is need to be used.");

                                pdfService.ExportPdf(this, stream, settings as StiPdfExportSettings);
                                break;
                            #endregion

                            #region Xps
                            case StiExportFormat.Xps:
                                if (exportService != null && (!(exportService is StiXpsExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var xpsService = exportService as StiXpsExportService;
                                if (xpsService == null) xpsService = new StiXpsExportService();

                                if (settings == null) settings = new StiXpsExportSettings();
                                if (!(settings is StiXpsExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiXpsExportSettings is need to be used.");

                                xpsService.ExportXps(this, stream, settings as StiXpsExportSettings);
                                break;
                            #endregion

                            #region Rtf, RtfTable
                            case StiExportFormat.Rtf:
                            case StiExportFormat.RtfTable:
                                if (exportService != null && (!(exportService is StiRtfExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var rtf1service = exportService as StiRtfExportService;
                                if (rtf1service == null) rtf1service = new StiRtfExportService();

                                if (settings == null) settings = new StiRtfExportSettings();
                                if (!(settings is StiRtfExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiRtfExportSettings is need to be used.");

                                if (exportFormat != StiExportFormat.Rtf)
                                {
                                    ((StiRtfExportSettings)settings).ExportMode = StiRtfExportMode.Table;
                                }

                                rtf1service.ExportRtf(this, stream, settings as StiRtfExportSettings);
                                break;
                            #endregion

                            #region RtfFrame
                            case StiExportFormat.RtfFrame:
                                if (exportService != null && (!(exportService is StiRtfExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var rtf2service = exportService as StiRtfExportService;
                                if (rtf2service == null) rtf2service = new StiRtfExportService();

                                if (settings == null) settings = new StiRtfExportSettings();
                                if (!(settings is StiRtfExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiRtfExportSettings is need to be used.");

                                ((StiRtfExportSettings)settings).ExportMode = StiRtfExportMode.Frame;
                                rtf2service.ExportRtf(this, stream, settings as StiRtfExportSettings);
                                break;
                            #endregion

                            #region RtfWinWord
                            case StiExportFormat.RtfWinWord:
                                if (exportService != null && (!(exportService is StiRtfExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var rtf3service = exportService as StiRtfExportService;
                                if (rtf3service == null) rtf3service = new StiRtfExportService();

                                if (settings == null) settings = new StiRtfExportSettings();
                                if (!(settings is StiRtfExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiRtfExportSettings is need to be used.");

                                ((StiRtfExportSettings)settings).ExportMode = StiRtfExportMode.WinWord;
                                rtf3service.ExportRtf(this, stream, settings as StiRtfExportSettings);
                                break;
                            #endregion

                            #region RtfTabbedText
                            case StiExportFormat.RtfTabbedText:
                                if (exportService != null && (!(exportService is StiRtfExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var rtf4service = exportService as StiRtfExportService;
                                if (rtf4service == null) rtf4service = new StiRtfExportService();

                                if (settings == null) settings = new StiRtfExportSettings();
                                if (!(settings is StiRtfExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiRtfExportSettings is need to be used.");

                                ((StiRtfExportSettings)settings).ExportMode = StiRtfExportMode.TabbedText;
                                rtf4service.ExportRtf(this, stream, settings as StiRtfExportSettings);
                                break;
                            #endregion

                            #region Text
                            case StiExportFormat.Text:
                                if (exportService != null && (!(exportService is StiTxtExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var txtService = exportService as StiTxtExportService;
                                if (txtService == null) txtService = new StiTxtExportService();

                                if (settings == null) settings = new StiTxtExportSettings();
                                if (!(settings is StiTxtExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiTxtExportSettings is need to be used.");

                                txtService.ExportTxt(this, stream, settings as StiTxtExportSettings);
                                break;
                            #endregion

                            #region Xml
                            case StiExportFormat.Xml:
                                if (exportService != null && (!(exportService is StiXmlExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var xmlService = exportService as StiXmlExportService;
                                if (xmlService == null) xmlService = new StiXmlExportService();

                                if (settings == null) settings = new StiXmlExportSettings();
                                bool isCorrectSettingsXml = (settings is StiXmlExportSettings) || ((settings is StiDataExportSettings) && (settings as StiDataExportSettings).DataType == StiDataType.Xml);
                                if (!isCorrectSettingsXml)
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiXmlExportSettings is need to be used.");

                                xmlService.ExportXml(this, stream, settings as StiDataExportSettings);
                                break;
                            #endregion

                            #region JSON
                            case StiExportFormat.Json:
                                if (exportService != null && (!(exportService is StiJsonExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var jsonService = exportService as StiJsonExportService;
                                if (jsonService == null) jsonService = new StiJsonExportService();

                                if (settings == null) settings = new StiJsonExportSettings();
                                bool isCorrectSettingsJson = (settings is StiJsonExportSettings) || ((settings is StiDataExportSettings) && (settings as StiDataExportSettings).DataType == StiDataType.Json);
                                if (!isCorrectSettingsJson)
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiJsonExportSettings is need to be used.");

                                jsonService.ExportJson(this, stream, settings as StiDataExportSettings);
                                break;
                            #endregion

                            #region Ods
                            case StiExportFormat.Ods:
                                if (exportService != null && (!(exportService is StiOdsExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var odsService = exportService as StiOdsExportService;
                                if (odsService == null) odsService = new StiOdsExportService();

                                if (settings == null) settings = new StiOdsExportSettings();
                                if (!(settings is StiOdsExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiOdsExportSettings is need to be used.");

                                odsService.ExportOds(this, stream, settings as StiOdsExportSettings);
                                break;
                            #endregion

                            #region Odt
                            case StiExportFormat.Odt:
                                if (exportService != null && (!(exportService is StiOdtExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var odtService = exportService as StiOdtExportService;
                                if (odtService == null) odtService = new StiOdtExportService();

                                if (settings == null) settings = new StiOdtExportSettings();
                                if (!(settings is StiOdtExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiOdtExportSettings is need to be used.");

                                odtService.ExportOdt(this, stream, settings as StiOdtExportSettings);
                                break;
                            #endregion

                            #region Ppt2007
                            case StiExportFormat.Ppt2007:
                                if (exportService != null && (!(exportService is StiPpt2007ExportService)))
                                    throw new ArgumentException("The incorrect type of the 'exportService' argument is used. The StiExportService must have same type of export as declared in exportFormat argument.");

                                var pptService2 = exportService as StiPpt2007ExportService;
                                if (pptService2 == null) pptService2 = new StiPpt2007ExportService();

                                if (settings == null) settings = new StiPpt2007ExportSettings();
                                if (!(settings is StiPpt2007ExportSettings))
                                    throw new ArgumentException("The incorrect type of the 'settings' argument is used. The StiPpt2007ExportSettings is need to be used.");

                                pptService2.ExportPowerPoint(this, stream, settings as StiPpt2007ExportSettings);
                                break;
                            #endregion

                            #region Document
                            case StiExportFormat.Document:
                                SaveDocument(stream);
                                break;
                                #endregion
                        }
                    }
                }
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Exporting rendered report...ERROR");
                StiLogService.Write(this.GetType(), e);

                //this is only for catch to avoid DisableFontSmoothing
                this.IsExporting = false;
                if (this.CompiledReport != null) CompiledReport.IsExporting = false;

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                this.Designer = stateDesigner;
                this.Info.ForceDesigningMode = stateForceDesigningMode;
                this.IsPageDesigner = stateIsPageDesigner;
            }

            InvokeExported(exportFormat);

            return this;
        }
        #endregion
    }
}
