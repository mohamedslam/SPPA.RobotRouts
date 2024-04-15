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

using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Export;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Web;
using System.IO;
using System.Text;
using System.Drawing.Imaging;

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Mvc
{
    public class StiNetCoreReportResponse
    {
#region Methods: Export

#region Document
        
        /// <summary>
        /// Exports report to MDC format and saves this document to the webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsDocument(StiReport report, string documentType, string password)
        {
            if (documentType != null)
            {
                switch (documentType)
                {
                    case "SaveReportMdc":
                    case "mdc":
                        return ResponseAsDocument(report, StiDocumentType.Mdc, password);

                    case "SaveReportMdz":
                    case "mdz":
                        return ResponseAsDocument(report, StiDocumentType.Mdz, password);

                    case "SaveReportMdx":
                    case "mdx":
                        return ResponseAsDocument(report, StiDocumentType.Mdx, password);
                }
            }

            return new StiNetCoreActionResult("Unknown document format.");
        }

        /// <summary>
        /// Exports report to MDC format and saves this document to the webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsDocument(StiReport report, StiDocumentType documentType, string password)
        {
            if (!report.IsRendered) report.Render(false);

            var stream = new MemoryStream();
            var fileType = ".mdc";
            var contentType = "text/xml";

            switch (documentType)
            {
                case StiDocumentType.Mdc:
                    report.SaveDocument(stream);
                    break;

                case StiDocumentType.Mdz:
                    report.SavePackedDocument(stream);
                    fileType = ".mdz";
                    contentType = "application/zip";
                    break;

                case StiDocumentType.Mdx:
                    report.SaveEncryptedDocument(stream, password);
                    fileType = ".mdz";
                    contentType = "application/octet-stream";
                    break;
            }

            return new StiNetCoreActionResult(stream, contentType, StiReportHelper.GetReportFileName(report) + fileType, true);
        }

#endregion

#region Adobe PDF File

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
		public static StiNetCoreActionResult ResponseAsPdf(StiReport report)
        {
            return ResponseAsPdf(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsPdf(StiReport report, bool saveFileDialog)
        {
            return ResponseAsPdf(report, StiPagesRange.All, 1, 100, true, false, saveFileDialog);
        }

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsPdf(StiReport report, StiPagesRange pageRange)
        {
            return ResponseAsPdf(report, pageRange, 1, 100, true, false);
        }

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsPdf(StiReport report, StiPagesRange pageRange, bool saveFileDialog)
        {
            return ResponseAsPdf(report, pageRange, 1, 100, true, false, saveFileDialog);
        }

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageQuality">A float value that sets the quality of exporting images. Default value is 1.</param>
        /// <param name="imageResolution">A float value that sets the resolution of exporting images. Default value is 100.</param>
        /// <param name="embeddedFonts">If embeddedFont is true then, when exporting, fonts of the report will be included in the resulting document.</param>
        /// <param name="standardPdfFonts">If standardPdfFont is true then, when exporting, non-standard fonts of the report will be replaced by the standard fonts in resulting document.</param>
        public static StiNetCoreActionResult ResponseAsPdf(StiReport report, StiPagesRange pageRange, float imageQuality, float imageResolution, bool embeddedFonts, bool standardPdfFonts)
        {
            return ResponseAsPdf(report, pageRange, imageQuality, imageResolution, embeddedFonts, standardPdfFonts, true);
        }

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageQuality">A float value that sets the quality of exporting images. Default value is 1.</param>
        /// <param name="imageResolution">A float value that sets the resolution of exporting images. Default value is 100.</param>
        /// <param name="embeddedFonts">If embeddedFont is true then, when exporting, fonts of the report will be included in the resulting document.</param>
        /// <param name="standardPdfFonts">If standardPdfFont is true then, when exporting, non-standard fonts of the report will be replaced by the standard fonts in resulting document.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
		public static StiNetCoreActionResult ResponseAsPdf(StiReport report, StiPagesRange pageRange, float imageQuality, float imageResolution, bool embeddedFonts, bool standardPdfFonts, bool saveFileDialog)
        {
            return ResponseAsPdf(report, pageRange, imageQuality, imageResolution, embeddedFonts, standardPdfFonts, false, "", "",
                StiUserAccessPrivileges.All, StiPdfEncryptionKeyLength.Bit40, true, saveFileDialog);
        }

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageQuality">A float value that sets the quality of exporting images. Default value is 1.</param>
        /// <param name="imageResolution">A float value that sets the resolution of exporting images. Default value is 100.</param>
        /// <param name="embeddedFonts">If embeddedFonts is true then, when exporting, fonts of the report will be included in the resulting document.</param>
        /// <param name="standardPdfFonts">If standardPdfFont is true then, when exporting, non-standard fonts of the report will be replaced by the standard fonts in resulting document.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        /// <param name="passwordUser">The user password that manage the exported document access.</param>
        /// <param name="passwordOwner">The owner password that manage ability to modify exported document.</param>
        /// <param name="userAccessPrivileges">Defines user access privileges to the document exported.</param>
        /// <param name="keyLength">Defines the encryption key length to protect the document access after export.</param>
        /// <param name="useUnicode">If useUnicode is true then in pdf files unicode is used.</param>
        public static StiNetCoreActionResult ResponseAsPdf(StiReport report, StiPagesRange pageRange, float imageQuality, float imageResolution,
            bool embeddedFonts, bool standardPdfFonts, bool exportRtfTextAsImage, string passwordUser, string passwordOwner,
            StiUserAccessPrivileges userAccessPrivileges, StiPdfEncryptionKeyLength keyLength, bool useUnicode, bool saveFileDialog)
        {
            var settings = new StiPdfExportSettings();
            settings.PageRange = pageRange;
            settings.ImageQuality = imageQuality;
            settings.ImageResolution = imageResolution;
            settings.EmbeddedFonts = embeddedFonts;
            settings.StandardPdfFonts = standardPdfFonts;
            settings.ExportRtfTextAsImage = exportRtfTextAsImage;
            settings.PasswordInputUser = passwordUser;
            settings.PasswordInputOwner = passwordOwner;
            settings.UserAccessPrivileges = userAccessPrivileges;
            settings.KeyLength = keyLength;
            settings.UseUnicode = useUnicode;

            return ResponseAsPdf(report, settings, saveFileDialog);
        }

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsPdf(StiReport report, StiPdfExportSettings settings)
        {
            return ResponseAsPdf(report, settings, true);
        }

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsPdf(StiReport report, StiPdfExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiPdfExportService();
            export.ExportPdf(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/pdf", StiReportHelper.GetReportFileName(report) + ".pdf", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsPdf(StiReport report, IStiPdfDashboardExportSettings settings)
        {
            return ResponseAsPdf(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsPdf(StiReport report, IStiPdfDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/pdf", StiReportHelper.GetReportFileName(report) + ".pdf", saveFileDialog);
        }
#endregion

#region Microsoft XPS File

        /// <summary>
        /// Exports report to XPS document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsXps(StiReport report)
        {
            return ResponseAsXps(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to XPS document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsXps(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiXpsExportSettings();
            settings.PageRange = pageRange;

            return ResponseAsXps(report, settings);
        }

        /// <summary>
        /// Exports report to XPS document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsXps(StiReport report, StiXpsExportSettings settings)
        {
            return ResponseAsXps(report, settings, true);
        }

        /// <summary>
        /// Exports report to XPS document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsXps(StiReport report, StiXpsExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiXpsExportService();
            export.ExportXps(report, stream, settings);
            
            return new StiNetCoreActionResult(stream, "application/vnd.ms-xpsdocument", StiReportHelper.GetReportFileName(report) + ".xps", saveFileDialog);
        }

#endregion

#region Microsoft PowerPoint 2007 File

        /// <summary>
        /// Exports report to PPTX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsPpt(StiReport report)
        {
            return ResponseAsPpt(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to PPTX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsPpt(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiPpt2007ExportSettings();
            settings.PageRange = pageRange;

            return ResponseAsPpt(report, settings, true);
        }

        /// <summary>
        /// Exports report to PPTX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsPpt(StiReport report, StiPpt2007ExportSettings settings)
        {
            return ResponseAsPpt(report, settings, true);
        }

        /// <summary>
        /// Exports report to PPTX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsPpt(StiReport report, StiPpt2007ExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiPpt2007ExportService();
            export.ExportPowerPoint(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/vnd.ms-powerpoint", StiReportHelper.GetReportFileName(report) + ".pptx", saveFileDialog);
        }

#endregion

#region HTML File

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report)
        {
            return ResponseAsHtml(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, StiPagesRange pageRange)
        {
            return ResponseAsHtml(report, pageRange, ImageFormat.Png);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat)
        {
            return ResponseAsHtml(report, pageRange, imageFormat, null);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        /// <param name="htmlImageHost">Class that controls placement of images when exporting.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, StiHtmlImageHost htmlImageHost)
        {
            return ResponseAsHtml(report, pageRange, imageFormat, htmlImageHost, true);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, bool saveFileDialog)
        {
            return ResponseAsHtml(report, StiPagesRange.All, ImageFormat.Png, null, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="htmlImageHost">Class that controls placement of images when exporting.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, StiHtmlImageHost htmlImageHost, bool saveFileDialog)
        {
            return ResponseAsHtml(report, StiPagesRange.All, ImageFormat.Png, htmlImageHost, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        /// <param name="htmlImageHost">Class that controls placement of images when exporting.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, StiHtmlImageHost htmlImageHost, bool saveFileDialog)
        {
            return ResponseAsHtml(report, pageRange, imageFormat, htmlImageHost, StiHtmlExportMode.Table, StiHtmlExportQuality.High, Encoding.UTF8, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        /// <param name="htmlImageHost">Class that controls placement of images during the export.</param>
        /// <param name="exportMode">Sets the mode of report export.</param>
        /// <param name="exportQuality">This parameter always is to be StiHtmlExportQuality.High.</param>
        /// <param name="encoding">Resulting Html page encoding.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, StiHtmlImageHost htmlImageHost, StiHtmlExportMode exportMode,
            StiHtmlExportQuality exportQuality, Encoding encoding, bool saveFileDialog)
        {
            var settings = new StiHtmlExportSettings();
            settings.PageRange = pageRange;
            settings.ImageFormat = imageFormat;
            settings.ExportMode = exportMode;
            settings.ExportQuality = exportQuality;
            settings.Encoding = encoding;

            return ResponseAsHtml(report, settings, htmlImageHost, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, StiHtmlExportSettings settings)
        {
            return ResponseAsHtml(report, settings, true);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, StiHtmlExportSettings settings, bool saveFileDialog)
        {
            return ResponseAsHtml(report, settings, null, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="htmlImageHost">Class that controls placement of images during the export.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, StiHtmlExportSettings settings, StiHtmlImageHost htmlImageHost, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiHtmlExportService();
            export.HtmlImageHost = htmlImageHost;
            export.ExportHtml(report, stream, settings);

            return new StiNetCoreActionResult(stream, "text/html", StiReportHelper.GetReportFileName(report) + ".html", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, IStiHtmlDashboardExportSettings settings)
        {
            return ResponseAsHtml(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsHtml(StiReport report, IStiHtmlDashboardExportSettings settings,  bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "text/html", StiReportHelper.GetReportFileName(report) + ".html", saveFileDialog);
        }
#endregion

#region HTML5 File

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsHtml5(StiReport report)
        {
            return ResponseAsHtml5(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsHtml5(StiReport report, StiPagesRange pageRange)
        {
            return ResponseAsHtml5(report, pageRange, ImageFormat.Png);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsHtml5(StiReport report, bool saveFileDialog)
        {
            return ResponseAsHtml5(report, StiPagesRange.All, ImageFormat.Png, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        public static StiNetCoreActionResult ResponseAsHtml5(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat)
        {
            return ResponseAsHtml5(report, pageRange, imageFormat, true);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsHtml5(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, bool saveFileDialog)
        {
            return ResponseAsHtml5(report, pageRange, imageFormat, StiHtmlExportMode.Table, StiHtmlExportQuality.High, Encoding.UTF8, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        /// <param name="exportMode">Sets the mode of report export.</param>
        /// <param name="exportQuality">This parameter always is to be StiHtmlExportQuality.High.</param>
        /// <param name="encoding">Resulting Html page encoding.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsHtml5(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, StiHtmlExportMode exportMode,
            StiHtmlExportQuality exportQuality, Encoding encoding, bool saveFileDialog)
        {
            var settings = new StiHtmlExportSettings(StiHtmlType.Html5);
            settings.PageRange = pageRange;
            settings.ImageFormat = imageFormat;
            settings.ExportMode = exportMode;
            settings.ExportQuality = exportQuality;
            settings.Encoding = encoding;

            return ResponseAsHtml5(report, settings, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsHtml5(StiReport report, StiHtmlExportSettings settings)
        {
            return ResponseAsHtml5(report, settings, true);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsHtml5(StiReport report, StiHtmlExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiHtml5ExportService();
            export.ExportHtml(report, stream, settings);

            return new StiNetCoreActionResult(stream, "text/html", StiReportHelper.GetReportFileName(report) + ".html", saveFileDialog);
        }

#endregion

#region MHT Web Archive

        /// <summary>
        /// Exports report to MHT format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsMht(StiReport report)
        {
            return ResponseAsMht(report, true);
        }

        /// <summary>
        /// Exports report to MHT format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsMht(StiReport report, bool saveFileDialog)
        {
            return ResponseAsMht(report, StiPagesRange.All, ImageFormat.Png, StiHtmlExportMode.Table, Encoding.UTF8, saveFileDialog);
        }

        /// <summary>
        /// Exports report to MHT format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        /// <param name="exportMode">This parameter always is to be StiHtmlExportQuality.High.</param>
        /// <param name="encoding">Sets the code page of the resulting MHT document.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsMht(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, StiHtmlExportMode exportMode, Encoding encoding, bool saveFileDialog)
        {
            var settings = new StiMhtExportSettings();
            settings.PageRange = pageRange;
            settings.ImageFormat = imageFormat;
            settings.ExportMode = exportMode;
            settings.Encoding = encoding;

            return ResponseAsMht(report, settings, saveFileDialog);
        }

        /// <summary>
        /// Exports report to MHT format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsMht(StiReport report, StiHtmlExportSettings settings)
        {
            return ResponseAsMht(report, settings, true);
        }

        /// <summary>
        /// Exports report to MHT format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsMht(StiReport report, StiHtmlExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiMhtExportService();
            export.ExportMht(report, stream, settings);

            return new StiNetCoreActionResult(stream, "message/rfc822", StiReportHelper.GetReportFileName(report) + ".mht", saveFileDialog);
        }

#endregion

#region Text File

        /// <summary>
        /// Exports report to Text format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsText(StiReport report)
        {
            return ResponseAsText(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to Text format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsText(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiTxtExportSettings();
            settings.PageRange = pageRange;

            return ResponseAsText(report, settings);
        }

        /// <summary>
        /// Exports report to Text format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsText(StiReport report, StiTxtExportSettings settings)
        {
            return ResponseAsText(report, settings, true);
        }

        /// <summary>
        /// Exports report to Text format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsText(StiReport report, StiTxtExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiTxtExportService();
            export.ExportTxt(report, stream, settings);

            return new StiNetCoreActionResult(stream, "text/plain", StiReportHelper.GetReportFileName(report) + ".txt", saveFileDialog);
        }

#endregion

#region Rich Text File

        /// <summary>
        /// Exports report to RTF format and saves this document to thecurrent  webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsRtf(StiReport report)
        {
            return ResponseAsRtf(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to RTF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsRtf(StiReport report, StiPagesRange pageRange)
        {
            return ResponseAsRtf(report, pageRange, StiRtfExportMode.Table, true);
        }

        /// <summary>
        /// Exports report to RTF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="exportMode">Sets the mode of report exporting.</param>
        public static StiNetCoreActionResult ResponseAsRtf(StiReport report, StiPagesRange pageRange, StiRtfExportMode exportMode)
        {
            return ResponseAsRtf(report, pageRange, exportMode, true);
        }

        /// <summary>
        /// Exports report to RTF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="exportMode">Sets the mode of report exporting.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsRtf(StiReport report, StiPagesRange pageRange, StiRtfExportMode exportMode, bool saveFileDialog)
        {
            var settings = new StiRtfExportSettings();
            settings.PageRange = pageRange;
            settings.ExportMode = exportMode;

            return ResponseAsRtf(report, settings, saveFileDialog);
        }

        /// <summary>
        /// Exports report to RTF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsRtf(StiReport report, StiRtfExportSettings settings)
        {
            return ResponseAsRtf(report, settings, true);
        }

        /// <summary>
        /// Exports report to RTF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsRtf(StiReport report, StiRtfExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiRtfExportService();
            export.ExportRtf(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/rtf", StiReportHelper.GetReportFileName(report) + ".rtf", saveFileDialog);
        }

#endregion

#region Microsoft Word 2007 File

        /// <summary>
        /// Exports report to Word 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsWord2007(StiReport report)
        {
            return ResponseAsWord2007(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to Word 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsWord2007(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiWord2007ExportSettings();
            settings.PageRange = pageRange;

            return ResponseAsWord2007(report, settings);
        }

        /// <summary>
        /// Exports report to Word 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsWord2007(StiReport report, StiWord2007ExportSettings settings)
        {
            return ResponseAsWord2007(report, settings, true);
        }

        /// <summary>
        /// Exports report to Word 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsWord2007(StiReport report, StiWord2007ExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiWord2007ExportService();
            export.ExportWord(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", StiReportHelper.GetReportFileName(report) + ".docx", saveFileDialog);
        }

#endregion

#region OpenDocument Writer File

        /// <summary>
        /// Exports report to OpenDocument Writer format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsOdt(StiReport report)
        {
            return ResponseAsOdt(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to OpenDocument Writer format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsOdt(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiOdtExportSettings();
            settings.PageRange = pageRange;

            return ResponseAsOdt(report, settings);
        }

        /// <summary>
        /// Exports report to OpenDocument Writer format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsOdt(StiReport report, StiOdtExportSettings settings)
        {
            return ResponseAsOdt(report, settings, true);
        }

        /// <summary>
        /// Exports report to OpenDocument Writer format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsOdt(StiReport report, StiOdtExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiOdtExportService();
            export.ExportOdt(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/vnd.oasis.opendocument.text", StiReportHelper.GetReportFileName(report) + ".odt", saveFileDialog);
        }

#endregion

#region Microsoft Excel File

        /// <summary>
        /// Exports report to Excel format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
		public static StiNetCoreActionResult ResponseAsXls(StiReport report)
        {
            return ResponseAsXls(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to Excel format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsXls(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiExcelExportSettings();
            settings.PageRange = pageRange;

            return ResponseAsXls(report, settings);
        }

        /// <summary>
        /// Exports report to Excel format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsXls(StiReport report, StiExcelExportSettings settings)
        {
            return ResponseAsXls(report, settings, true);
        }

        /// <summary>
        /// Exports report to Excel format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsXls(StiReport report, StiExcelExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiExcelExportService();
            export.ExportExcel(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/vnd.ms-excel", StiReportHelper.GetReportFileName(report) + ".xls", saveFileDialog);
        }

#endregion

#region Microsoft Excel Xml File

        /// <summary>
        /// Exports report to Excel XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsXlsXml(StiReport report)
        {
            return ResponseAsXlsXml(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to Excel XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsXlsXml(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiExcelXmlExportSettings();
            settings.PageRange = pageRange;

            return ResponseAsXlsXml(report, settings);
        }

        /// <summary>
        /// Exports report to Excel XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsXlsXml(StiReport report, StiExcelExportSettings settings)
        {
            return ResponseAsXlsXml(report, settings, true);
        }

        /// <summary>
        /// Exports report to Excel XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsXlsXml(StiReport report, StiExcelExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiExcelXmlExportService();
            export.ExportExcel(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/vnd.ms-excel", StiReportHelper.GetReportFileName(report) + ".xls", saveFileDialog);
        }

#endregion

#region Microsoft Excel 2007 File

        /// <summary>
        /// Exports report to Excel 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsExcel2007(StiReport report)
        {
            return ResponseAsExcel2007(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to Excel 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsExcel2007(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiExcel2007ExportSettings();
            settings.PageRange = pageRange;

            return ResponseAsExcel2007(report, settings);
        }

        /// <summary>
        /// Exports report to Excel 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsExcel2007(StiReport report, StiExcelExportSettings settings)
        {
            return ResponseAsExcel2007(report, settings, true);
        }

        /// <summary>
        /// Exports report to Excel 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsExcel2007(StiReport report, StiExcelExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiExcel2007ExportService();
            export.ExportExcel(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", StiReportHelper.GetReportFileName(report) + ".xlsx", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to Excel 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsExcel2007(StiReport report, IStiExcelDashboardExportSettings settings)
        {
            return ResponseAsExcel2007(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to Excel 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsExcel2007(StiReport report, IStiExcelDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", StiReportHelper.GetReportFileName(report) + ".xlsx", saveFileDialog);
        }
#endregion

#region OpenDocument Calc File

        /// <summary>
        /// Exports report to OpenDocument Calc format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsOds(StiReport report)
        {
            return ResponseAsOds(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to OpenDocument Calc format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsOds(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiOdsExportSettings();
            settings.PageRange = pageRange;

            return ResponseAsOds(report, settings);
        }

        /// <summary>
        /// Exports report to OpenDocument Calc format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsOds(StiReport report, StiOdsExportSettings settings)
        {
            return ResponseAsOds(report, settings, true);
        }

        /// <summary>
        /// Exports report to OpenDocument Calc format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsOds(StiReport report, StiOdsExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiOdsExportService();
            export.ExportOds(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/vnd.oasis.opendocument.spreadsheet", StiReportHelper.GetReportFileName(report) + ".ods", saveFileDialog);
        }

#endregion

#region CSV File

        /// <summary>
        /// Exports report to CSV format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsCsv(StiReport report)
        {
            return ResponseAsCsv(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to CSV format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
		public static StiNetCoreActionResult ResponseAsCsv(StiReport report, StiPagesRange pageRange)
        {
            return ResponseAsCsv(report, StiPagesRange.All, ";", Encoding.UTF8);
        }

        /// <summary>
        /// Exports report to CSV format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsCsv(StiReport report, StiPagesRange pageRange, string separator, Encoding encoding)
        {
            var settings = new StiCsvExportSettings();
            settings.Encoding = encoding;
            settings.PageRange = pageRange;
            settings.Separator = separator;

            return ResponseAsCsv(report, settings);
        }

        /// <summary>
        /// Exports report to CSV format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsCsv(StiReport report, StiDataExportSettings settings)
        {
            return ResponseAsCsv(report, settings, true);
        }

        /// <summary>
        /// Exports report to CSV format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsCsv(StiReport report, StiDataExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiCsvExportService();
            export.ExportCsv(report, stream, settings);

            return new StiNetCoreActionResult(stream, "text/csv", StiReportHelper.GetReportFileName(report) + ".csv", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to CSV format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsCsv(StiReport report, IStiDataDashboardExportSettings settings)
        {
            return ResponseAsCsv(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to CSV format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsCsv(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Csv;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "text/csv", StiReportHelper.GetReportFileName(report) + ".csv", saveFileDialog);
        }
#endregion

#region XML File

        /// <summary>
        /// Exports report to XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsXml(StiReport report)
        {
            return ResponseAsXml(report, true);
        }

        /// <summary>
        /// Exports report to XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsXml(StiReport report, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiXmlExportService();
            export.ExportXml(report, stream);

            return new StiNetCoreActionResult(stream, "application/xml", StiReportHelper.GetReportFileName(report) + ".xml", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsXml(StiReport report, IStiDataDashboardExportSettings settings)
        {
            return ResponseAsXml(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsXml(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Xml;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/xml", StiReportHelper.GetReportFileName(report) + ".xml", saveFileDialog);
        }
#endregion

#region JSON File

        /// <summary>
        /// Exports report to JSON format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsJson(StiReport report)
        {
            return ResponseAsJson(report, true);
        }

        /// <summary>
        /// Exports report to JSON format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsJson(StiReport report, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiJsonExportService();
            export.ExportJson(report, stream);

            return new StiNetCoreActionResult(stream, "application/json", StiReportHelper.GetReportFileName(report) + ".json", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to JSON format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsJson(StiReport report, IStiDataDashboardExportSettings settings)
        {
            return ResponseAsJson(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to JSON format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsJson(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Json;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/json", StiReportHelper.GetReportFileName(report) + ".json", saveFileDialog);
        }
#endregion

#region dBase DBF File

        /// <summary>
        /// Exports report to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsDbf(StiReport report)
        {
            return ResponseAsDbf(report, StiDbfCodePages.Default, true);
        }

        /// <summary>
        /// Exports report to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="codePage">Sets the code page of the exported document.</param>
        public static StiNetCoreActionResult ResponseAsDbf(StiReport report, StiDbfCodePages codePage)
        {
            return ResponseAsDbf(report, codePage, true);
        }

        /// <summary>
        /// Exports report to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="codePage">Sets the code page of the exported document.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsDbf(StiReport report, StiDbfCodePages codePage, bool saveFileDialog)
        {
            return ResponseAsDbf(report, StiPagesRange.All, codePage, saveFileDialog);
        }

        /// <summary>
        /// Exports report to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="codePage">Sets the code page of the exported document.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsDbf(StiReport report, StiPagesRange pageRange, StiDbfCodePages codePage, bool saveFileDialog)
        {
            var settings = new StiDbfExportSettings();
            settings.CodePage = codePage;
            settings.PageRange = pageRange;

            return ResponseAsDbf(report, settings, saveFileDialog);
        }

        /// <summary>
        /// Exports report to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsDbf(StiReport report, StiDataExportSettings settings)
        {
            return ResponseAsDbf(report, settings, true);
        }

        /// <summary>
        /// Exports report to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsDbf(StiReport report, StiDataExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiDbfExportService();
            export.ExportDbf(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/dbf", StiReportHelper.GetReportFileName(report) + ".dbf", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsDbf(StiReport report, IStiDataDashboardExportSettings settings)
        {
            return ResponseAsDbf(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsDbf(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Dbf;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/dbf", StiReportHelper.GetReportFileName(report) + ".dbf", saveFileDialog);
        }
#endregion

#region Data Interchange Format (DIF) File

        /// <summary>
        /// Exports report to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsDif(StiReport report)
        {
            return ResponseAsDif(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsDif(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiDifExportSettings();
            settings.PageRange = pageRange;

            return ResponseAsDif(report, settings, true);
        }

        /// <summary>
        /// Exports report to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsDif(StiReport report, StiDataExportSettings settings)
        {
            return ResponseAsDif(report, settings, true);
        }

        /// <summary>
        /// Exports report to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsDif(StiReport report, StiDataExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiDifExportService();
            export.ExportDif(report, stream, settings);

            return new StiNetCoreActionResult(stream, "video/x-dv", StiReportHelper.GetReportFileName(report) + ".dif", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsDif(StiReport report, IStiDataDashboardExportSettings settings)
        {
            return ResponseAsDif(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsDif(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Dif;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "video/x-dv", StiReportHelper.GetReportFileName(report) + ".dif", saveFileDialog);
        }
#endregion

#region Symbolic Link (SYLK) File

        /// <summary>
        /// Exports report to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsSylk(StiReport report)
        {
            return ResponseAsSylk(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsSylk(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiSylkExportSettings();
            settings.PageRange = pageRange;

            return ResponseAsSylk(report, settings, true);
        }

        /// <summary>
        /// Exports report to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsSylk(StiReport report, StiDataExportSettings settings)
        {
            return ResponseAsSylk(report, settings, true);
        }

        /// <summary>
        /// Exports report to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsSylk(StiReport report, StiDataExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiSylkExportService();
            export.ExportSylk(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/excel", StiReportHelper.GetReportFileName(report) + ".sylk", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsSylk(StiReport report, IStiDataDashboardExportSettings settings)
        {
            return ResponseAsSylk(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsSylk(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Sylk;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "application/excel", StiReportHelper.GetReportFileName(report) + ".sylk", saveFileDialog);
        }
#endregion

#region BMP Image

        /// <summary>
        /// Exports report to BMP format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsBmp(StiReport report)
        {
            return ResponseAsBmp(report, new StiImageExportSettings(StiImageType.Bmp));
        }

        /// <summary>
        /// Exports report to BMP format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsBmp(StiReport report, StiImageExportSettings settings)
        {
            return ResponseAsBmp(report, settings, true);
        }

        /// <summary>
        /// Exports report to BMP format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsBmp(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/bmp", StiReportHelper.GetReportFileName(report) + ".bmp", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to BMP format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsBmp(StiReport report, IStiImageDashboardExportSettings settings)
        {
            return ResponseAsBmp(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to BMP format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsBmp(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Bmp;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/bmp", StiReportHelper.GetReportFileName(report) + ".bmp", saveFileDialog);
        }
#endregion

#region GIF Image

        /// <summary>
        /// Exports report to GIF format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static StiNetCoreActionResult ResponseAsGif(StiReport report)
        {
            return ResponseAsGif(report, new StiImageExportSettings(StiImageType.Gif));
        }

        /// <summary>
        /// Exports report to GIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsGif(StiReport report, StiImageExportSettings settings)
        {
            return ResponseAsGif(report, settings, true);
        }

        /// <summary>
        /// Exports report to GIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsGif(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/gif", StiReportHelper.GetReportFileName(report) + ".gif", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to GIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsGif(StiReport report, IStiImageDashboardExportSettings settings)
        {
            return ResponseAsGif(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to GIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsGif(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Gif;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/gif", StiReportHelper.GetReportFileName(report) + ".gif", saveFileDialog);
        }
#endregion

#region JPEG Image

        /// <summary>
        /// Exports report to JPEG format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static StiNetCoreActionResult ResponseAsJpeg(StiReport report)
        {
            return ResponseAsJpeg(report, new StiImageExportSettings(StiImageType.Jpeg));
        }

        /// <summary>
        /// Exports report to JPEG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsJpeg(StiReport report, StiImageExportSettings settings)
        {
            return ResponseAsJpeg(report, settings, true);
        }

        /// <summary>
        /// Exports report to JPEG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsJpeg(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/jpeg", StiReportHelper.GetReportFileName(report) + ".jpg", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to JPEG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsJpeg(StiReport report, IStiImageDashboardExportSettings settings)
        {
            return ResponseAsJpeg(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to JPEG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsJpeg(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Jpeg;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/jpeg", StiReportHelper.GetReportFileName(report) + ".jpg", saveFileDialog);
        }
#endregion

#region PNG Image

        /// <summary>
        /// Exports report to PNG format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static StiNetCoreActionResult ResponseAsPng(StiReport report)
        {
            return ResponseAsPng(report, new StiImageExportSettings(StiImageType.Png));
        }

        /// <summary>
        /// Exports report to PNG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsPng(StiReport report, StiImageExportSettings settings)
        {
            return ResponseAsPng(report, settings, true);
        }

        /// <summary>
        /// Exports report to PNG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsPng(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/png", StiReportHelper.GetReportFileName(report) + ".png", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to PNG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsPng(StiReport report, IStiImageDashboardExportSettings settings)
        {
            return ResponseAsPng(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to PNG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsPng(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Png;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/png", StiReportHelper.GetReportFileName(report) + ".png", saveFileDialog);
        }
#endregion

#region PCX Image

        /// <summary>
        /// Exports report to PCX format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static StiNetCoreActionResult ResponseAsPcx(StiReport report)
        {
            return ResponseAsPcx(report, new StiImageExportSettings(StiImageType.Pcx));
        }

        /// <summary>
        /// Exports report to PCX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsPcx(StiReport report, StiImageExportSettings settings)
        {
            return ResponseAsPcx(report, settings, true);
        }

        /// <summary>
        /// Exports report to PCX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsPcx(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/x-pcx", StiReportHelper.GetReportFileName(report) + ".pcx", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to PCX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsPcx(StiReport report, IStiImageDashboardExportSettings settings)
        {
            return ResponseAsPcx(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to PCX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsPcx(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Pcx;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/x-pcx", StiReportHelper.GetReportFileName(report) + ".pcx", saveFileDialog);
        }
#endregion

#region TIFF Image

        /// <summary>
        /// Exports report to TIFF format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static StiNetCoreActionResult ResponseAsTiff(StiReport report)
        {
            return ResponseAsTiff(report, new StiImageExportSettings(StiImageType.Tiff));
        }

        /// <summary>
        /// Exports report to TIFF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsTiff(StiReport report, StiImageExportSettings settings)
        {
            return ResponseAsTiff(report, settings, true);
        }

        /// <summary>
        /// Exports report to TIFF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsTiff(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/tiff", StiReportHelper.GetReportFileName(report) + ".tiff", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to TIFF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsTiff(StiReport report, IStiImageDashboardExportSettings settings)
        {
            return ResponseAsTiff(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to TIFF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsTiff(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Tiff;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/tiff", StiReportHelper.GetReportFileName(report) + ".tiff", saveFileDialog);
        }
#endregion

#region Windows Metafile

        /// <summary>
		/// Exports report to EMF format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static StiNetCoreActionResult ResponseAsMetafile(StiReport report)
        {
            return ResponseAsMetafile(report, new StiImageExportSettings(StiImageType.Emf));
        }

        /// <summary>
        /// Exports report to EMF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsMetafile(StiReport report, StiImageExportSettings settings)
        {
            return ResponseAsMetafile(report, settings, true);
        }

        /// <summary>
        /// Exports report to EMF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsMetafile(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/x-emf", StiReportHelper.GetReportFileName(report) + ".emf", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to EMF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsMetafile(StiReport report, IStiImageDashboardExportSettings settings)
        {
            return ResponseAsMetafile(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to EMF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsMetafile(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Emf;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/x-emf", StiReportHelper.GetReportFileName(report) + ".emf", saveFileDialog);
        }
#endregion

#region Scalable Vector Graphics (SVG) File

        /// <summary>
        /// Exports report to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsSvg(StiReport report)
        {
            return ResponseAsSvg(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsSvg(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiImageExportSettings(StiImageType.Svg);
            settings.PageRange = pageRange;

            return ResponseAsSvg(report, settings, true);
        }

        /// <summary>
        /// Exports report to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsSvg(StiReport report, StiImageExportSettings settings)
        {
            return ResponseAsSvg(report, settings, true);
        }

        /// <summary>
        /// Exports report to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsSvg(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/svg+xml", StiReportHelper.GetReportFileName(report) + ".svg", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsSvg(StiReport report, IStiImageDashboardExportSettings settings)
        {
            return ResponseAsSvg(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsSvg(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Svg;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/svg+xml", StiReportHelper.GetReportFileName(report) + ".svg", saveFileDialog);
        }
#endregion

#region Compressed SVG (SVGZ) File

        /// <summary>
        /// Exports report to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static StiNetCoreActionResult ResponseAsSvgz(StiReport report)
        {
            return ResponseAsSvgz(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static StiNetCoreActionResult ResponseAsSvgz(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiImageExportSettings(StiImageType.Svgz);
            settings.PageRange = pageRange;

            return ResponseAsSvgz(report, settings, true);
        }

        /// <summary>
        /// Exports report to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsSvgz(StiReport report, StiImageExportSettings settings)
        {
            return ResponseAsSvgz(report, settings, true);
        }

        /// <summary>
        /// Exports report to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsSvgz(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/svg+xml", StiReportHelper.GetReportFileName(report) + ".svgz", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult ResponseAsSvgz(StiReport report, IStiImageDashboardExportSettings settings)
        {
            return ResponseAsSvgz(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static StiNetCoreActionResult ResponseAsSvgz(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Svgz;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            return new StiNetCoreActionResult(stream, "image/svg+xml", StiReportHelper.GetReportFileName(report) + ".svgz", saveFileDialog);
        }
#endregion

#endregion

#region Methods: Print

#region Adobe PDF

        /// <summary>
        /// Print report as PDF document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        public static StiNetCoreActionResult PrintAsPdf(StiReport report)
        {
            return PrintAsPdf(report, StiPagesRange.All);
        }

        /// <summary>
        /// Print report as PDF document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        /// <param name="pageRange">Describes pages range for the printing.</param>
        public static StiNetCoreActionResult PrintAsPdf(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiPdfExportSettings();
            settings.PageRange = pageRange;

            return PrintAsPdf(report, settings);
        }

        /// <summary>
        /// Print report as PDF document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult PrintAsPdf(StiReport report, StiPdfExportSettings settings)
        {
            settings.AutoPrintMode = StiPdfAutoPrintMode.Dialog;

            return ResponseAsPdf(report, settings, false);
        }

#endregion

#region HTML

        /// <summary>
        /// Print report as HTML document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        public static StiNetCoreActionResult PrintAsHtml(StiReport report)
        {
            return PrintAsHtml(report, StiPagesRange.All);
        }
        
        /// <summary>
        /// Print report as HTML document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        /// <param name="pageRange">Describes pages range for the printing.</param>
        public static StiNetCoreActionResult PrintAsHtml(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiHtmlExportSettings();
            settings.PageRange = pageRange;

            return PrintAsHtml(report, settings);
        }
        
        /// <summary>
        /// Print report as HTML document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static StiNetCoreActionResult PrintAsHtml(StiReport report, StiHtmlExportSettings settings)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) report.Render(false);

            settings.ChartType = StiHtmlChartType.Image;

            var stream = new MemoryStream();
            var export = new StiHtmlExportService();
            export.ExportHtml(report, stream, settings);

            stream.Position = 0;
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            {
                var html = sr.ReadToEnd();
                html = html.Replace("<body", "<body onload='window.print()'");
                stream = new MemoryStream(Encoding.UTF8.GetBytes(html));
            }
            
            return new StiNetCoreActionResult(stream, "text/html", null, false);
        }

#endregion

#endregion
    }
}
