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

using System;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Web;
using Stimulsoft.Report.Export;
using System.Threading;
using Stimulsoft.Report.Dashboard.Export;
using Stimulsoft.Report.Dashboard;
using System.Drawing.Imaging;

#if NETSTANDARD
using Stimulsoft.System.Web;
using Stimulsoft.System.Web.UI;
#else
using System.Web.UI;
#endif

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Web
{
	public sealed class StiReportResponse
    {
        #region Methods: Export

        #region Document

        /// <summary>
        /// Exports report to MDC format and saves this document to the webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsDocument(StiReport report, string documentType, string password)
        {
            ResponseAsDocument(null, report, documentType, password);
        }

        /// <summary>
        /// Exports report to MDC format and saves this document to the webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsDocument(StiReport report, StiDocumentType documentType, string password)
        {
            ResponseAsDocument(null, report, documentType, password);
        }

        /// <summary>
        /// Exports report to MDC format and saves this document to the webpage response.
        /// </summary>
        /// <param name="page">Webpage to save exported report.</param>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsDocument(Page page, StiReport report, string documentType, string password)
        {
            if (documentType != null)
            {
                switch (documentType)
                {
                    case "SaveReportMdc":
                    case "mdc":
                        ResponseAsDocument(page, report, StiDocumentType.Mdc, password);
                        break;

                    case "SaveReportMdz":
                    case "mdz":
                        ResponseAsDocument(page, report, StiDocumentType.Mdz, password);
                        break;

                    case "SaveReportMdx":
                    case "mdx":
                        ResponseAsDocument(page, report, StiDocumentType.Mdx, password);
                        break;
                }
            }
        }

        /// <summary>
        /// Exports report to MDC format and saves this document to the webpage response.
        /// </summary>
        /// <param name="page">Webpage to save exported report.</param>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsDocument(Page page, StiReport report, StiDocumentType documentType, string password)
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

            ResponseStream(stream, contentType, false, StiReportHelper.GetReportFileName(report) + fileType, true);
        }

        #endregion

        #region Adobe PDF File

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
		public static void ResponseAsPdf(StiReport report)
		{
			ResponseAsPdf(report, StiPagesRange.All);
		}

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsPdf(StiReport report, bool saveFileDialog)
        {
            ResponseAsPdf(report, StiPagesRange.All, 1, 100, true, false, saveFileDialog);
        }

		/// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
		public static void ResponseAsPdf(StiReport report, StiPagesRange pageRange)
		{
			ResponseAsPdf(report, pageRange, 1, 100, true, false);
		}

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsPdf(StiReport report, StiPagesRange pageRange, bool saveFileDialog)
        {
            ResponseAsPdf(report, pageRange, 1, 100, true, false, saveFileDialog);
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
		public static void ResponseAsPdf(StiReport report, StiPagesRange pageRange, float imageQuality, float imageResolution, bool embeddedFonts, bool standardPdfFonts)
		{
			ResponseAsPdf(report, pageRange, imageQuality, imageResolution, embeddedFonts, standardPdfFonts, true);
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
		public static void ResponseAsPdf(StiReport report, StiPagesRange pageRange, float imageQuality, float imageResolution, bool embeddedFonts, bool standardPdfFonts, bool saveFileDialog)
		{
            ResponseAsPdf(report, pageRange, imageQuality, imageResolution, embeddedFonts, standardPdfFonts, false, "", "", 
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
		public static void ResponseAsPdf(StiReport report, StiPagesRange pageRange, float imageQuality, float imageResolution, 
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

            ResponseAsPdf(report, settings, saveFileDialog);
		}

        /// <summary>
        /// Exports report to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsPdf(StiReport report, StiPdfExportSettings settings)
        {
            ResponseAsPdf(report, settings, true);
        }

		/// <summary>
		/// Exports report to PDF document and saves this document to the current webpage response.
		/// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsPdf(StiReport report, StiPdfExportSettings settings, bool saveFileDialog)
		{
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

			if (!report.IsRendered) 
                report.Render(false);

			var stream = new MemoryStream();
			var export = new StiPdfExportService();
			export.ExportPdf(report, stream, settings);

            ResponseStream(stream, "application/pdf", false, StiReportHelper.GetReportFileName(report) + ".pdf", saveFileDialog);
		}

        /// <summary>
        /// Exports dashboard to PDF document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsPdf(StiReport report, IStiPdfDashboardExportSettings settings)
        {
            ResponseAsPdf(report, settings, true);
        }

        /// <summary>
		/// Exports dashboard to PDF document and saves this document to the current webpage response.
		/// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsPdf(StiReport report, IStiPdfDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "application/pdf", false, StiReportHelper.GetReportFileName(report) + ".pdf", saveFileDialog);
        }
        #endregion

        #region Microsoft XPS File

        /// <summary>
        /// Exports report to XPS document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsXps(StiReport report)
		{
			ResponseAsXps(report, StiPagesRange.All);
		}

		/// <summary>
        /// Exports report to XPS document and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		/// <param name="pageRange">Describes pages range for the export.</param>
		public static void ResponseAsXps(StiReport report, StiPagesRange pageRange)
		{
			var settings = new StiXpsExportSettings();
			settings.PageRange = pageRange;

            ResponseAsXps(report, settings);
		}

        /// <summary>
        /// Exports report to XPS document and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsXps(StiReport report, StiXpsExportSettings settings)
        {
            ResponseAsXps(report, settings, true);
        }

		/// <summary>
        /// Exports report to XPS document and saves this document to the current webpage response.
		/// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsXps(StiReport report, StiXpsExportSettings settings, bool saveFileDialog)
		{
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

			if (!report.IsRendered) 
                report.Render(false);

			var stream = new MemoryStream();
			var export = new StiXpsExportService();
			export.ExportXps(report, stream, settings);

            ResponseStream(stream, "application/vnd.ms-xpsdocument", false, StiReportHelper.GetReportFileName(report) + ".xps", saveFileDialog);
		}

        #endregion

        #region Microsoft PowerPoint 2007 File

        /// <summary>
        /// Exports report to PPTX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsPpt(StiReport report)
        {
            ResponseAsPpt(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to PPTX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsPpt(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiPpt2007ExportSettings();
            settings.PageRange = pageRange;

            ResponseAsPpt(report, settings, true);
        }

        /// <summary>
        /// Exports report to PPTX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsPpt(StiReport report, StiPpt2007ExportSettings settings)
        {
            ResponseAsPpt(report, settings, true);
        }

        /// <summary>
        /// Exports report to PPTX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsPpt(StiReport report, StiPpt2007ExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiPpt2007ExportService();
            export.ExportPowerPoint(report, stream, settings);

            ResponseStream(stream, "application/vnd.ms-powerpoint", false, StiReportHelper.GetReportFileName(report) + ".pptx", saveFileDialog);
        }

        #endregion

        #region HTML File

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsHtml(StiReport report)
        {
            ResponseAsHtml(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsHtml(StiReport report, StiPagesRange pageRange)
        {
            ResponseAsHtml(report, pageRange, ImageFormat.Png);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        public static void ResponseAsHtml(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat)
        {
            ResponseAsHtml(report, pageRange, imageFormat, null);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        /// <param name="htmlImageHost">Class that controls placement of images when exporting.</param>
        public static void ResponseAsHtml(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, StiHtmlImageHost htmlImageHost)
        {
            ResponseAsHtml(report, pageRange, imageFormat, htmlImageHost, true);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsHtml(StiReport report, bool saveFileDialog)
        {
            ResponseAsHtml(report, StiPagesRange.All, ImageFormat.Png, null, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="htmlImageHost">Class that controls placement of images when exporting.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsHtml(StiReport report, StiHtmlImageHost htmlImageHost, bool saveFileDialog)
        {
            ResponseAsHtml(report, StiPagesRange.All, ImageFormat.Png, htmlImageHost, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        /// <param name="htmlImageHost">Class that controls placement of images when exporting.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsHtml(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, StiHtmlImageHost htmlImageHost, bool saveFileDialog)
        {
            ResponseAsHtml(report, pageRange, imageFormat, htmlImageHost, StiHtmlExportMode.Table, StiHtmlExportQuality.High, Encoding.UTF8, saveFileDialog);
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
        public static void ResponseAsHtml(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, StiHtmlImageHost htmlImageHost, StiHtmlExportMode exportMode,
            StiHtmlExportQuality exportQuality, Encoding encoding, bool saveFileDialog)
        {
            var settings = new StiHtmlExportSettings();
            settings.PageRange = pageRange;
            settings.ImageFormat = imageFormat;
            settings.ExportMode = exportMode;
            settings.ExportQuality = exportQuality;
            settings.Encoding = encoding;

            ResponseAsHtml(report, settings, htmlImageHost, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsHtml(StiReport report, StiHtmlExportSettings settings)
        {
            ResponseAsHtml(report, settings, true);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsHtml(StiReport report, StiHtmlExportSettings settings, bool saveFileDialog)
        {
            ResponseAsHtml(report, settings, null, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="htmlImageHost">Class that controls placement of images during the export.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsHtml(StiReport report, StiHtmlExportSettings settings, StiHtmlImageHost htmlImageHost, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiHtmlExportService();
            export.HtmlImageHost = htmlImageHost;
            export.ExportHtml(report, stream, settings);

            ResponseStream(stream, "text/html", false, StiReportHelper.GetReportFileName(report) + ".html", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsHtml(StiReport report, IStiHtmlDashboardExportSettings settings)
        {
            ResponseAsHtml(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to HTML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsHtml(StiReport report, IStiHtmlDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "text/html", false, StiReportHelper.GetReportFileName(report) + ".html", saveFileDialog);
        }
        #endregion		

        #region HTML5 File

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsHtml5(StiReport report)
        {
            ResponseAsHtml5(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsHtml5(StiReport report, StiPagesRange pageRange)
        {
            ResponseAsHtml5(report, pageRange, ImageFormat.Png);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsHtml5(StiReport report, bool saveFileDialog)
        {
            ResponseAsHtml5(report, StiPagesRange.All, ImageFormat.Png, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        public static void ResponseAsHtml5(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat)
        {
            ResponseAsHtml5(report, pageRange, imageFormat, true);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="imageFormat">Format of images for the export.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsHtml5(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, bool saveFileDialog)
        {
            ResponseAsHtml5(report, pageRange, imageFormat, StiHtmlExportMode.Table, StiHtmlExportQuality.High, Encoding.UTF8, saveFileDialog);
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
        public static void ResponseAsHtml5(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, StiHtmlExportMode exportMode,
            StiHtmlExportQuality exportQuality, Encoding encoding, bool saveFileDialog)
        {
            var settings = new StiHtmlExportSettings(StiHtmlType.Html5);
            settings.PageRange = pageRange;
            settings.ImageFormat = imageFormat;
            settings.ExportMode = exportMode;
            settings.ExportQuality = exportQuality;
            settings.Encoding = encoding;

            ResponseAsHtml5(report, settings, saveFileDialog);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsHtml5(StiReport report, StiHtmlExportSettings settings)
        {
            ResponseAsHtml5(report, settings, true);
        }

        /// <summary>
        /// Exports report to HTML5 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsHtml5(StiReport report, StiHtmlExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiHtml5ExportService();
            export.ExportHtml(report, stream, settings);

            ResponseStream(stream, "text/html", false, StiReportHelper.GetReportFileName(report) + ".html", saveFileDialog);
        }

        #endregion		

        #region MHT Web Archive

        /// <summary>
        /// Exports report to MHT format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsMht(StiReport report, bool saveFileDialog)
		{
            ResponseAsMht(report, StiPagesRange.All, ImageFormat.Png, StiHtmlExportMode.Table, Encoding.UTF8, saveFileDialog);
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
        public static void ResponseAsMht(StiReport report, StiPagesRange pageRange, ImageFormat imageFormat, StiHtmlExportMode exportMode, Encoding encoding, bool saveFileDialog)
        {
            var settings = new StiMhtExportSettings();
            settings.PageRange = pageRange;
            settings.ImageFormat = imageFormat;
            settings.ExportMode = exportMode;
            settings.Encoding = encoding;

            ResponseAsMht(report, settings, saveFileDialog);
        }

        /// <summary>
        /// Exports report to MHT format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsMht(StiReport report, StiHtmlExportSettings settings)
        {
            ResponseAsMht(report, settings, true);
        }

		/// <summary>
        /// Exports report to MHT format and saves this document to the current webpage response.
		/// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsMht(StiReport report, StiHtmlExportSettings settings, bool saveFileDialog)
		{
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

			if (!report.IsRendered)
                report.Render(false);

			var stream = new MemoryStream();
			var export = new StiMhtExportService();
			export.ExportMht(report, stream, settings);

            ResponseStream(stream, "message/rfc822", false, StiReportHelper.GetReportFileName(report) + ".mht", saveFileDialog);
		}

		#endregion		

        #region Text File

        /// <summary>
        /// Exports report to Text format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsText(StiReport report)
        {
            ResponseAsText(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to Text format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsText(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiTxtExportSettings();
            settings.PageRange = pageRange;

            ResponseAsText(report, settings);
        }

        /// <summary>
        /// Exports report to Text format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsText(StiReport report, StiTxtExportSettings settings)
        {
            ResponseAsText(report, settings, true);
        }

        /// <summary>
        /// Exports report to Text format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsText(StiReport report, StiTxtExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiTxtExportService();
            export.ExportTxt(report, stream, settings);

            ResponseStream(stream, "text/plain", false, StiReportHelper.GetReportFileName(report) + ".txt", saveFileDialog);
        }

        #endregion

        #region Rich Text File

        /// <summary>
        /// Exports report to RTF format and saves this document to thecurrent  webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsRtf(StiReport report)
        {
            ResponseAsRtf(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to RTF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsRtf(StiReport report, StiPagesRange pageRange)
        {
            ResponseAsRtf(report, pageRange, StiRtfExportMode.Table, true);
        }

        /// <summary>
        /// Exports report to RTF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="exportMode">Sets the mode of report exporting.</param>
        public static void ResponseAsRtf(StiReport report, StiPagesRange pageRange, StiRtfExportMode exportMode)
        {
            ResponseAsRtf(report, pageRange, exportMode, true);
        }

        /// <summary>
        /// Exports report to RTF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="exportMode">Sets the mode of report exporting.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsRtf(StiReport report, StiPagesRange pageRange, StiRtfExportMode exportMode, bool saveFileDialog)
        {
            var settings = new StiRtfExportSettings();
            settings.PageRange = pageRange;
            settings.ExportMode = exportMode;

            ResponseAsRtf(report, settings, saveFileDialog);
        }

        /// <summary>
        /// Exports report to RTF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsRtf(StiReport report, StiRtfExportSettings settings)
        {
            ResponseAsRtf(report, settings, true);
        }

        /// <summary>
        /// Exports report to RTF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsRtf(StiReport report, StiRtfExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiRtfExportService();
            export.ExportRtf(report, stream, settings);

            ResponseStream(stream, "application/rtf", false, StiReportHelper.GetReportFileName(report) + ".rtf", saveFileDialog);
        }

        #endregion

        #region Microsoft Word 2007 File

        /// <summary>
        /// Exports report to Word 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsWord2007(StiReport report)
        {
            ResponseAsWord2007(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to Word 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsWord2007(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiWord2007ExportSettings();
            settings.PageRange = pageRange;

            ResponseAsWord2007(report, settings);
        }

        /// <summary>
        /// Exports report to Word 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsWord2007(StiReport report, StiWord2007ExportSettings settings)
        {
            ResponseAsWord2007(report, settings, true);
        }

        /// <summary>
        /// Exports report to Word 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsWord2007(StiReport report, StiWord2007ExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiWord2007ExportService();
            export.ExportWord(report, stream, settings);

            ResponseStream(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", false, StiReportHelper.GetReportFileName(report) + ".docx", saveFileDialog);
        }

        #endregion		

        #region OpenDocument Writer File

        /// <summary>
        /// Exports report to OpenDocument Writer format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsOdt(StiReport report)
        {
            ResponseAsOdt(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to OpenDocument Writer format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsOdt(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiOdtExportSettings();
            settings.PageRange = pageRange;

            ResponseAsOdt(report, settings);
        }

        /// <summary>
        /// Exports report to OpenDocument Writer format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsOdt(StiReport report, StiOdtExportSettings settings)
        {
            ResponseAsOdt(report, settings, true);
        }

        /// <summary>
        /// Exports report to OpenDocument Writer format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsOdt(StiReport report, StiOdtExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiOdtExportService();
            export.ExportOdt(report, stream, settings);

            ResponseStream(stream, "application/vnd.oasis.opendocument.text", false, StiReportHelper.GetReportFileName(report) + ".odt", saveFileDialog);
        }

        #endregion

        #region Microsoft Excel File

        /// <summary>
        /// Exports report to Excel format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
		public static void ResponseAsXls(StiReport report)
		{
			ResponseAsXls(report, StiPagesRange.All);
		}

		/// <summary>
        /// Exports report to Excel format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		/// <param name="pageRange">Describes pages range for the export.</param>
		public static void ResponseAsXls(StiReport report, StiPagesRange pageRange)
		{
			var settings = new StiExcelExportSettings();
			settings.PageRange = pageRange;

			ResponseAsXls(report, settings);
		}

		/// <summary>
        /// Exports report to Excel format and saves this document to the current webpage response.
		/// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
		public static void ResponseAsXls(StiReport report, StiExcelExportSettings settings)
		{
            ResponseAsXls(report, settings, true);
		}

        /// <summary>
        /// Exports report to Excel format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsXls(StiReport report, StiExcelExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiExcelExportService();
            export.ExportExcel(report, stream, settings);

            ResponseStream(stream, "application/vnd.ms-excel", false, StiReportHelper.GetReportFileName(report) + ".xls", saveFileDialog);
        }

		#endregion

        #region Microsoft Excel Xml File

        /// <summary>
        /// Exports report to Excel XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsXlsXml(StiReport report)
        {
            ResponseAsXlsXml(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to Excel XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsXlsXml(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiExcelXmlExportSettings();
            settings.PageRange = pageRange;

            ResponseAsXlsXml(report, settings);
        }

        /// <summary>
        /// Exports report to Excel XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsXlsXml(StiReport report, StiExcelExportSettings settings)
        {
            ResponseAsXlsXml(report, settings, true);
        }

        /// <summary>
        /// Exports report to Excel XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsXlsXml(StiReport report, StiExcelExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiExcelXmlExportService();
            export.ExportExcel(report, stream, settings);

            ResponseStream(stream, "application/vnd.ms-excel", false, StiReportHelper.GetReportFileName(report) + ".xls", saveFileDialog);
        }

        #endregion

        #region Microsoft Excel 2007 File
        
        /// <summary>
		/// Exports report to Excel 2007 format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static void ResponseAsExcel2007(StiReport report)
		{
			ResponseAsExcel2007(report, StiPagesRange.All);
		}

		/// <summary>
        /// Exports report to Excel 2007 format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		/// <param name="pageRange">Describes pages range for the export.</param>
		public static void ResponseAsExcel2007(StiReport report, StiPagesRange pageRange)
		{
			var settings = new StiExcel2007ExportSettings();
			settings.PageRange = pageRange;

			ResponseAsExcel2007(report, settings);
		}

		/// <summary>
        /// Exports report to Excel 2007 format and saves this document to the current webpage response.
		/// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsExcel2007(StiReport report, StiExcelExportSettings settings)
		{
            ResponseAsExcel2007(report, settings, true);
		}

        /// <summary>
        /// Exports report to Excel 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsExcel2007(StiReport report, StiExcelExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiExcel2007ExportService();
            export.ExportExcel(report, stream, settings);

            ResponseStream(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", false, StiReportHelper.GetReportFileName(report) + ".xlsx", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to Excel 2007 format and saves this document to the current webpage response.
		/// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsExcel2007(StiReport report, IStiExcelDashboardExportSettings settings)
        {
            ResponseAsExcel2007(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to Excel 2007 format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsExcel2007(StiReport report, IStiExcelDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", false, StiReportHelper.GetReportFileName(report) + ".xlsx", saveFileDialog);
        }
        #endregion

        #region OpenDocument Calc File

        /// <summary>
        /// Exports report to OpenDocument Calc format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsOds(StiReport report)
        {
            ResponseAsOds(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to OpenDocument Calc format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsOds(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiOdsExportSettings();
            settings.PageRange = pageRange;

            ResponseAsOds(report, settings);
        }

        /// <summary>
        /// Exports report to OpenDocument Calc format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsOds(StiReport report, StiOdsExportSettings settings)
        {
            ResponseAsOds(report, settings, true);
        }

        /// <summary>
        /// Exports report to OpenDocument Calc format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsOds(StiReport report, StiOdsExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiOdsExportService();
            export.ExportOds(report, stream, settings);

            ResponseStream(stream, "application/vnd.oasis.opendocument.spreadsheet", false, StiReportHelper.GetReportFileName(report) + ".ods", saveFileDialog);
        }

        #endregion

        #region CSV File

        /// <summary>
        /// Exports report to CSV format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsCsv(StiReport report)
        {
            ResponseAsCsv(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to CSV format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
		public static void ResponseAsCsv(StiReport report, StiPagesRange pageRange)
		{
			ResponseAsCsv(report, StiPagesRange.All, ";", Encoding.UTF8);
		}

		/// <summary>
        /// Exports report to CSV format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		/// <param name="pageRange">Describes pages range for the export.</param>
		public static void ResponseAsCsv(StiReport report, StiPagesRange pageRange, string separator, Encoding encoding)
		{			
			var settings = new StiCsvExportSettings();
			settings.Encoding = encoding;
			settings.PageRange = pageRange;
			settings.Separator = separator;

			ResponseAsCsv(report, settings);
		}

		/// <summary>
        /// Exports report to CSV format and saves this document to the current webpage response.
		/// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsCsv(StiReport report, StiDataExportSettings settings)
		{
            ResponseAsCsv(report, settings, true);
		}

        /// <summary>
        /// Exports report to CSV format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsCsv(StiReport report, StiDataExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiCsvExportService();
            export.ExportCsv(report, stream, settings);

            ResponseStream(stream, "text/csv", false, StiReportHelper.GetReportFileName(report) + ".csv", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to CSV format and saves this document to the current webpage response.
		/// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsCsv(StiReport report, IStiDataDashboardExportSettings settings)
        {
            ResponseAsCsv(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to CSV format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsCsv(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Csv;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "text/csv", false, StiReportHelper.GetReportFileName(report) + ".csv", saveFileDialog);
        }
        #endregion

        #region XML File

        /// <summary>
        /// Exports report to XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsXml(StiReport report)
		{
            ResponseAsXml(report, true);
		}

        /// <summary>
        /// Exports report to XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsXml(StiReport report, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiXmlExportService();
            export.ExportXml(report, stream);

            ResponseStream(stream, "application/xml", false, StiReportHelper.GetReportFileName(report) + ".xml", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsXml(StiReport report, IStiDataDashboardExportSettings settings)
        {
            ResponseAsXml(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to XML format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsXml(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Xml;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "application/xml", false, StiReportHelper.GetReportFileName(report) + ".xml", saveFileDialog);
        }
        #endregion

        #region JSON File

        /// <summary>
        /// Exports report to JSON format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsJson(StiReport report)
        {
            ResponseAsJson(report, true);
        }

        /// <summary>
        /// Exports report to JSON format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsJson(StiReport report, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiJsonExportService();
            export.ExportJson(report, stream);

            ResponseStream(stream, "application/json", false, StiReportHelper.GetReportFileName(report) + ".json", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to JSON format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsJson(StiReport report, IStiDataDashboardExportSettings settings)
        {
            ResponseAsJson(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to JSON format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsJson(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Json;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "application/json", false, StiReportHelper.GetReportFileName(report) + ".json", saveFileDialog);
        }
        #endregion

        #region dBase DBF File

        /// <summary>
        /// Exports report to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="codePage">Sets the code page of the exported document.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsDbf(StiReport report, StiDbfCodePages codePage, bool saveFileDialog)
        {
            ResponseAsDbf(report, StiPagesRange.All, codePage, saveFileDialog);
        }

        /// <summary>
        /// Exports report to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        /// <param name="codePage">Sets the code page of the exported document.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsDbf(StiReport report, StiPagesRange pageRange, StiDbfCodePages codePage, bool saveFileDialog)
        {
            var settings = new StiDbfExportSettings();
            settings.CodePage = codePage;
            settings.PageRange = pageRange;

            ResponseAsDbf(report, settings, saveFileDialog);
        }

        /// <summary>
        /// Exports report to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsDbf(StiReport report, StiDataExportSettings settings)
        {
            ResponseAsDbf(report, settings, true);
        }

        /// <summary>
        /// Exports report to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsDbf(StiReport report, StiDataExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiDbfExportService();
            export.ExportDbf(report, stream, settings);

            ResponseStream(stream, "application/dbf", false, StiReportHelper.GetReportFileName(report) + ".dbf", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsDbf(StiReport report, IStiDataDashboardExportSettings settings)
        {
            ResponseAsDbf(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to Dbase format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsDbf(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Dbf;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "application/dbf", false, StiReportHelper.GetReportFileName(report) + ".dbf", saveFileDialog);
        }
        #endregion

        #region Data Interchange Format (DIF) File

        /// <summary>
        /// Exports report to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsDif(StiReport report)
        {
            ResponseAsDif(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsDif(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiDifExportSettings();
            settings.PageRange = pageRange;

            ResponseAsDif(report, settings, true);
        }

        /// <summary>
        /// Exports report to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsDif(StiReport report, StiDataExportSettings settings)
        {
            ResponseAsDif(report, settings, true);
        }

        /// <summary>
        /// Exports report to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsDif(StiReport report, StiDataExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiDifExportService();
            export.ExportDif(report, stream, settings);

            ResponseStream(stream, "video/x-dv", false, StiReportHelper.GetReportFileName(report) + ".dif", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsDif(StiReport report, IStiDataDashboardExportSettings settings)
        {
            ResponseAsDif(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to DIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsDif(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Dif;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "video/x-dv", false, StiReportHelper.GetReportFileName(report) + ".dif", saveFileDialog);
        }
        #endregion

        #region Symbolic Link (SYLK) File

        /// <summary>
        /// Exports report to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsSylk(StiReport report)
        {
            ResponseAsSylk(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsSylk(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiSylkExportSettings();
            settings.PageRange = pageRange;

            ResponseAsSylk(report, settings, true);
        }

        /// <summary>
        /// Exports report to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsSylk(StiReport report, StiDataExportSettings settings)
        {
            ResponseAsSylk(report, settings, true);
        }

        /// <summary>
        /// Exports report to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsSylk(StiReport report, StiDataExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiSylkExportService();
            export.ExportSylk(report, stream, settings);

            ResponseStream(stream, "application/excel", false, StiReportHelper.GetReportFileName(report) + ".sylk", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsSylk(StiReport report, IStiDataDashboardExportSettings settings)
        {
            ResponseAsSylk(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to SYLK format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsSylk(StiReport report, IStiDataDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.DataType = StiDataType.Sylk;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "application/excel", false, StiReportHelper.GetReportFileName(report) + ".sylk", saveFileDialog);
        }
        #endregion

        #region BMP Image

        /// <summary>
        /// Exports report to BMP format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsBmp(StiReport report)
        {
            ResponseAsBmp(report, new StiImageExportSettings(StiImageType.Bmp));
        }

		/// <summary>
        /// Exports report to BMP format and saves this document to the current webpage response.
		/// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsBmp(StiReport report, StiImageExportSettings settings)
		{
            ResponseAsBmp(report, settings, true);
		}

        /// <summary>
        /// Exports report to BMP format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsBmp(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            ResponseStream(stream, "image/bmp", false, StiReportHelper.GetReportFileName(report) + ".bmp", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to BMP format and saves this document to the current webpage response.
		/// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsBmp(StiReport report, IStiImageDashboardExportSettings settings)
        {
            ResponseAsBmp(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to BMP format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsBmp(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Bmp;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "image/bmp", false, StiReportHelper.GetReportFileName(report) + ".bmp", saveFileDialog);
        }
        #endregion

        #region GIF Image

        /// <summary>
        /// Exports report to GIF format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static void ResponseAsGif(StiReport report)
		{
            ResponseAsGif(report, new StiImageExportSettings(StiImageType.Gif));
		}

        /// <summary>
        /// Exports report to GIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsGif(StiReport report, StiImageExportSettings settings)
        {
            ResponseAsGif(report, settings, true);
        }

        /// <summary>
        /// Exports report to GIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsGif(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            ResponseStream(stream, "image/gif", false, StiReportHelper.GetReportFileName(report) + ".gif", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to GIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsGif(StiReport report, IStiImageDashboardExportSettings settings)
        {
            ResponseAsGif(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to GIF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsGif(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Gif;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "image/gif", false, StiReportHelper.GetReportFileName(report) + ".gif", saveFileDialog);
        }
        #endregion

        #region JPEG Image

        /// <summary>
        /// Exports report to JPEG format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static void ResponseAsJpeg(StiReport report)
		{
            ResponseAsJpeg(report, new StiImageExportSettings(StiImageType.Jpeg));
		}

        /// <summary>
        /// Exports report to JPEG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsJpeg(StiReport report, StiImageExportSettings settings)
        {
            ResponseAsJpeg(report, settings, true);
        }

        /// <summary>
        /// Exports report to JPEG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsJpeg(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered)
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            ResponseStream(stream, "image/jpeg", false, StiReportHelper.GetReportFileName(report) + ".jpg", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to JPEG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsJpeg(StiReport report, IStiImageDashboardExportSettings settings)
        {
            ResponseAsJpeg(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to JPEG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsJpeg(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Jpeg;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "image/jpeg", false, StiReportHelper.GetReportFileName(report) + ".jpg", saveFileDialog);
        }
        #endregion

        #region PNG Image

        /// <summary>
        /// Exports report to PNG format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static void ResponseAsPng(StiReport report)
		{
            ResponseAsPng(report, new StiImageExportSettings(StiImageType.Png));
		}

        /// <summary>
        /// Exports report to PNG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsPng(StiReport report, StiImageExportSettings settings)
        {
            ResponseAsPng(report, settings, true);
        }

        /// <summary>
        /// Exports report to PNG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsPng(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            ResponseStream(stream, "image/png", false, StiReportHelper.GetReportFileName(report) + ".png", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to PNG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsPng(StiReport report, IStiImageDashboardExportSettings settings)
        {
            ResponseAsPng(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to PNG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsPng(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Png;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "image/png", false, StiReportHelper.GetReportFileName(report) + ".png", saveFileDialog);
        }
        #endregion

        #region PCX Image

        /// <summary>
        /// Exports report to PCX format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static void ResponseAsPcx(StiReport report)
		{
            ResponseAsPcx(report, new StiImageExportSettings(StiImageType.Pcx));
		}

        /// <summary>
        /// Exports report to PCX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsPcx(StiReport report, StiImageExportSettings settings)
        {
            ResponseAsPcx(report, settings, true);
        }

        /// <summary>
        /// Exports report to PCX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsPcx(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            ResponseStream(stream, "image/x-pcx", false, StiReportHelper.GetReportFileName(report) + ".pcx", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to PCX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsPcx(StiReport report, IStiImageDashboardExportSettings settings)
        {
            ResponseAsPcx(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to PCX format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsPcx(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Pcx;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "image/x-pcx", false, StiReportHelper.GetReportFileName(report) + ".pcx", saveFileDialog);
        }
        #endregion

        #region TIFF Image

        /// <summary>
        /// Exports report to TIFF format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static void ResponseAsTiff(StiReport report)
		{
            ResponseAsTiff(report, new StiImageExportSettings(StiImageType.Tiff));
		}

        /// <summary>
        /// Exports report to TIFF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsTiff(StiReport report, StiImageExportSettings settings)
        {
            ResponseAsTiff(report, settings, true);
        }

        /// <summary>
        /// Exports report to TIFF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsTiff(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            ResponseStream(stream, "image/tiff", false, StiReportHelper.GetReportFileName(report) + ".tiff", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to TIFF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsTiff(StiReport report, IStiImageDashboardExportSettings settings)
        {
            ResponseAsTiff(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to TIFF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsTiff(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Tiff;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "image/tiff", false, StiReportHelper.GetReportFileName(report) + ".tiff", saveFileDialog);
        }
        #endregion

        #region Windows Metafile

        /// <summary>
		/// Exports report to EMF format and saves this document to the current webpage response.
		/// </summary>
		/// <param name="report">Report, which is to be exported.</param>
		public static void ResponseAsMetafile(StiReport report)
		{
            ResponseAsMetafile(report, new StiImageExportSettings(StiImageType.Emf));
		}

        /// <summary>
        /// Exports report to EMF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsMetafile(StiReport report, StiImageExportSettings settings)
        {
            ResponseAsMetafile(report, settings, true);
        }

        /// <summary>
        /// Exports report to EMF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsMetafile(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            ResponseStream(stream, "image/x-emf", false, StiReportHelper.GetReportFileName(report) + ".emf", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to EMF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsMetafile(StiReport report, IStiImageDashboardExportSettings settings)
        {
            ResponseAsMetafile(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to EMF format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsMetafile(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Emf;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "image/x-emf", false, StiReportHelper.GetReportFileName(report) + ".emf", saveFileDialog);
        }
        #endregion

        #region Scalable Vector Graphics (SVG) File

        /// <summary>
        /// Exports report to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsSvg(StiReport report)
        {
            ResponseAsSvg(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsSvg(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiImageExportSettings(StiImageType.Svg);
            settings.PageRange = pageRange;

            ResponseAsSvg(report, settings, true);
        }

        /// <summary>
        /// Exports report to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsSvg(StiReport report, StiImageExportSettings settings)
        {
            ResponseAsSvg(report, settings, true);
        }

        /// <summary>
        /// Exports report to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsSvg(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            ResponseStream(stream, "image/svg+xml", false, StiReportHelper.GetReportFileName(report) + ".svg", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsSvg(StiReport report, IStiImageDashboardExportSettings settings)
        {
            ResponseAsSvg(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to SVG format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsSvg(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Emf;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "image/svg+xml", false, StiReportHelper.GetReportFileName(report) + ".svg", saveFileDialog);
        }
        #endregion

        #region Compressed SVG (SVGZ) File

        /// <summary>
        /// Exports report to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        public static void ResponseAsSvgz(StiReport report)
        {
            ResponseAsSvgz(report, StiPagesRange.All);
        }

        /// <summary>
        /// Exports report to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="pageRange">Describes pages range for the export.</param>
        public static void ResponseAsSvgz(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiImageExportSettings(StiImageType.Svgz);
            settings.PageRange = pageRange;

            ResponseAsSvgz(report, settings, true);
        }

        /// <summary>
        /// Exports report to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsSvgz(StiReport report, StiImageExportSettings settings)
        {
            ResponseAsSvgz(report, settings, true);
        }

        /// <summary>
        /// Exports report to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Report, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsSvgz(StiReport report, StiImageExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

            var stream = new MemoryStream();
            var export = new StiImageExportService();
            export.ExportImage(report, stream, settings);

            ResponseStream(stream, "image/svg+xml", false, StiReportHelper.GetReportFileName(report) + ".svgz", saveFileDialog);
        }

        /// <summary>
        /// Exports dashboard to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void ResponseAsSvgz(StiReport report, IStiImageDashboardExportSettings settings)
        {
            ResponseAsSvgz(report, settings, true);
        }

        /// <summary>
        /// Exports dashboard to SVGZ format and saves this document to the current webpage response.
        /// </summary>
        /// <param name="report">Dashboard, which is to be exported.</param>
        /// <param name="settings">All available settings for this export type.</param>
        /// <param name="saveFileDialog">If saveFileDialog is true then the browser Save Dialog Box will be displayed.</param>
        public static void ResponseAsSvgz(StiReport report, IStiImageDashboardExportSettings settings, bool saveFileDialog)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            settings.ImageType = StiImageType.Svgz;

            var stream = new MemoryStream();
            StiDashboardExport.Export(report, stream, settings);

            ResponseStream(stream, "image/svg+xml", false, StiReportHelper.GetReportFileName(report) + ".svgz", saveFileDialog);
        }
        #endregion

        #endregion

        #region Methods: Print

        #region Adobe PDF

        /// <summary>
        /// Print report as PDF document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        public static void PrintAsPdf(StiReport report)
        {
            PrintAsPdf(report, StiPagesRange.All);
        }

        /// <summary>
        /// Print report as PDF document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        /// <param name="pageRange">Describes pages range for the printing.</param>
        public static void PrintAsPdf(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiPdfExportSettings();
            settings.PageRange = pageRange;

            PrintAsPdf(report, settings);
        }

        /// <summary>
        /// Print report as PDF document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void PrintAsPdf(StiReport report, StiPdfExportSettings settings)
        {
            settings.AutoPrintMode = StiPdfAutoPrintMode.Dialog;

            ResponseAsPdf(report, settings, false);
        }

        #endregion

        #region HTML

        /// <summary>
        /// Print report as HTML document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        public static void PrintAsHtml(StiReport report)
        {
            PrintAsHtml(report, StiPagesRange.All);
        }

        /// <summary>
        /// Print report as HTML document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        /// <param name="pageRange">Describes pages range for the printing.</param>
        public static void PrintAsHtml(StiReport report, StiPagesRange pageRange)
        {
            var settings = new StiHtmlExportSettings();
            settings.PageRange = pageRange;

            PrintAsHtml(report, settings);
        }

        /// <summary>
        /// Print report as HTML document.
        /// </summary>
        /// <param name="report">Report, which is to be printed.</param>
        /// <param name="settings">All available settings for this export type.</param>
        public static void PrintAsHtml(StiReport report, StiHtmlExportSettings settings)
        {
            if (report.CookieContainer == null || report.CookieContainer.Count == 0)
                report.CookieContainer = StiReportHelper.GetCookieContainer();

            if (!report.IsRendered) 
                report.Render(false);

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

            ResponseStream(stream, "text/html", false);
        }
        #endregion

        #endregion

        #region Methods: Response
        public static void ResponseString(string data, string contentType, bool browserCache, string fileName = null, bool showSaveFileDialog = true)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            ResponseBuffer(buffer, contentType, browserCache, fileName, showSaveFileDialog);
        }

        public static void ResponseStream(Stream stream, string contentType, bool browserCache, string fileName = null, bool showSaveFileDialog = true)
        {
            ResponseBuffer(((MemoryStream)stream).ToArray(), contentType, browserCache, fileName, showSaveFileDialog);
        }

        public static void ResponseBuffer(byte[] buffer, string contentType, bool browserCache, string fileName = null, bool showSaveFileDialog = true)
		{
            HttpResponse response = null;

            try
            {
                response = HttpContext.Current.Response;
                response.StatusCode = 200;
            }
            catch
            {
                return;
            }

            if (StiOptions.Web.ClearResponseHeaders) response.ClearHeaders();

            response.Buffer = true;
            response.ClearContent();

            if (browserCache)
            {
                response.Cache.SetExpires(DateTime.Now.AddYears(1));
                response.Cache.SetCacheability(HttpCacheability.Public);
            }
            else if (!string.IsNullOrEmpty(fileName) && StiOptions.Web.ResponseCacheTimeout > 0)
            {
                response.Cache.SetExpires(DateTime.Now.AddSeconds(StiOptions.Web.ResponseCacheTimeout));
                response.Cache.SetCacheability(HttpCacheability.Public);
            }
            else
            {
                response.Cache.SetExpires(DateTime.MinValue);
                response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
            
            response.ContentType = contentType;
            response.ContentEncoding = Encoding.UTF8;
            response.AppendHeader("Content-Length", buffer.Length.ToString());
            if (!string.IsNullOrEmpty(fileName))
            {
                var type = showSaveFileDialog ? "attachment" : "inline";
                var value = string.Format("{0};filename=\"{1}\";filename*=UTF-8''{2}", type, fileName, HttpUtility.UrlEncode(fileName).Replace("+", "%20"));
                response.AppendHeader("Content-Disposition", value);
            }

            response.BinaryWrite(buffer);

            if (StiOptions.Web.AllowGCCollect)
            {
                GC.Collect();
                GC.Collect();
            }

            try
            {
                if (StiOptions.Web.AllowUseResponseFlush) response.Flush();
                if (StiOptions.Web.AllowUseResponseEnd) response.End();
                else
                {
                    HttpContext.Current.Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch
            {
            }
        }

        #endregion
    }
}
