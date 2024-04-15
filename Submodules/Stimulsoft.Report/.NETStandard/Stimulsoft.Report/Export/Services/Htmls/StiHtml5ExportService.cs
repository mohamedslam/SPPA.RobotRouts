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
using System.Globalization;
using System.IO;
using System.Text;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Zip;
using System.Threading;
using Stimulsoft.Base.Plans;
using System.Drawing;
using System.Drawing.Imaging;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the HTML5 export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceHtml.png")]
    public class StiHtml5ExportService : StiExportService
    {
        #region Fields
        private StiReport reporTmp;
        private string documentFileName;
        private bool sendEMail;
        private StiGuiMode guiMode;

        internal StiReport report;
        internal string fileName = string.Empty;
        internal ImageFormat imageFormat;
        private NumberFormatInfo numberFormat = new NumberFormatInfo();
        public StiHtmlTextWriter HtmlWriter = null;
        private Stream baseStream = null;
        private StreamWriter streamWriter = null;
        internal float imageQuality = 0.75f;
        internal float imageResolution = 96;
        internal bool compressToArchive = false;

        private CultureInfo currentCulture = null;
        #endregion

        #region StiExportService override
        /// <summary>
        /// Gets or sets a default extension of export. 
        /// </summary>
        public override string DefaultExtension => compressToArchive ? "zip" : "html";

        public override StiExportFormat ExportFormat => StiExportFormat.Html5;

        /// <summary>
        /// Gets a group of the export in the context menu.
        /// </summary>
        public override string GroupCategory => "Web";

        /// <summary>
        /// Gets a position of the export in the context menu.
        /// </summary>
        public override int Position => (int)StiExportPosition.Html5;

        /// <summary>
        /// Gets a export name in the context menu.
        /// </summary>
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeHtml5File");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportHtml(report, stream, settings as StiHtmlExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }

        /// <summary>
        /// Exports a rendered report to the HTML5 file.
        /// Also rendered report can be sent via e-mail.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
        {
            using (var form = StiGuiOptions.GetExportFormRunner("StiHtml5ExportSetupForm", guiMode, this.OwnerWindow))
            {
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;

                this.reporTmp = report;
                this.documentFileName = fileName;
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
        /// Returns a filter for Html files.
        /// </summary>
        /// <returns>Returns a filter for Html files.</returns>
        public override string GetFilter()
        {
            if (compressToArchive)
                return StiLocalization.Get("FileFilters", "ZipArchives");
            return StiLocalization.Get("FileFilters", "HtmlFiles");
        }
        #endregion

        #region Handlers
        private void Form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
        {
            if (e.DialogResult)
            {
                if (string.IsNullOrEmpty(documentFileName))
                    documentFileName = base.GetFileName(reporTmp, sendEMail);

                if (documentFileName != null)
                {
                    StiFileUtils.ProcessReadOnly(documentFileName);
                    try
                    {
                        using (var stream = new FileStream(documentFileName, FileMode.Create))
                        {
                            StartProgress(guiMode);

                            var settings = new StiHtmlExportSettings
                            {
                                ImageFormat = (ImageFormat)form["ImageFormat"],
                                PageRange = form["PagesRange"] as StiPagesRange,
                                ImageResolution = (float)form["Resolution"],
                                ImageQuality = (float)form["ImageQuality"],
                                ContinuousPages = (bool)form["ContinuousPages"]
                            };

                            base.StartExport(reporTmp, stream, settings, sendEMail, (bool)form["OpenAfterExport"], documentFileName, guiMode);
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
        private void RenderPage(StiReport report, StiPage page, ref int clipCounter)
        {
            var ms = new MemoryStream();
            StiSvgHelper.SaveToStream(report, page, ms, false, false, ref clipCounter, imageFormat, imageQuality, imageResolution);

            HtmlWriter.Flush();
            streamWriter.Flush();
            baseStream.Flush();

            ms.Seek(0, SeekOrigin.Begin);
            byte[] buf = new byte[ms.Length];
            ms.Read(buf, 0, buf.Length);
            baseStream.Write(buf, 3, buf.Length - 3);
            baseStream.Flush();
        }

        private void RenderStartDoc(StiPagesCollection pages, Encoding encoding)
        {
            HtmlWriter.Write("<!DOCTYPE html>");

            HtmlWriter.WriteLine();
            HtmlWriter.Indent++;

            HtmlWriter.WriteFullBeginTag("head");
            HtmlWriter.WriteLine();
            HtmlWriter.Indent++;

            HtmlWriter.WriteFullBeginTag("title");
            HtmlWriter.Write(report.ReportAlias);
            HtmlWriter.WriteEndTag("title");
            HtmlWriter.WriteLine();

            HtmlWriter.WriteBeginTag("meta");
            //HtmlWriter.WriteAttribute("http-equiv", "Content-Type");
            //HtmlWriter.WriteAttribute("content", string.Format("text/html; charset={0}", encoding.WebName));
            HtmlWriter.WriteAttribute("charset", encoding.WebName);
            HtmlWriter.WriteEndTag("meta");
            HtmlWriter.WriteLine();

            HtmlWriter.Indent--;
            //HtmlWriter.WriteLine();

            HtmlWriter.WriteEndTag("head");
            HtmlWriter.WriteLine();

            HtmlWriter.WriteBeginTag("body");
            //if ((pages != null) && (pages.Count > 0) && (pages[0].Brush != null))
            //{
            //    Color backColor = StiBrush.ToColor(pages[0].Brush);
            //    if (backColor != Color.Transparent)
            //    {
            //        HtmlWriter.WriteAttribute("bgcolor", ColorTranslator.ToHtml(backColor));
            //    }
            //}
            HtmlWriter.WriteAttribute("bgcolor", ColorTranslator.ToHtml(Color.LightBlue));
            HtmlWriter.WriteAttribute("style", "text-align:center;");
            HtmlWriter.Write(">");
            HtmlWriter.Indent++;
            HtmlWriter.WriteLine();
            //            HtmlWriter.Indent++;
        }

        private void RenderEndDoc()
        {
            HtmlWriter.Indent--;
            HtmlWriter.WriteEndTag("body");
            HtmlWriter.Indent--;
            HtmlWriter.WriteLine();
            HtmlWriter.WriteEndTag("html");
        }

        /// <summary>
        /// Exports a document to the HTML5.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        public void ExportHtml(StiReport report, Stream stream, StiHtmlExportSettings settings)
        {
            StiLogService.Write(this.GetType(), "Export report to Html format");

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            var pageRange = settings.PageRange;
            this.imageFormat = settings.ImageFormat;
            //bool useBookmarks =				settings.ExportBookmarksMode != StiHtmlExportBookmarksMode.ReportOnly;
            //int bookmarksWidth =			settings.BookmarksTreeWidth;
            //bool exportBookmarksOnly =		settings.ExportBookmarksMode == StiHtmlExportBookmarksMode.BookmarksOnly;
            //this.useStylesTable =           settings.UseStylesTable;
            this.imageResolution = settings.ImageResolution;
            this.imageQuality = settings.ImageQuality;
            this.compressToArchive = settings.CompressToArchive;
            bool continuousPages = settings.ContinuousPages;
            #endregion

            #region CompressToArchive, part1
            StiZipWriter20 zip = null;
            fileName = string.Empty;
            MemoryStream ms = null;
            if (settings.CompressToArchive)
            {
                var fileStream = stream as FileStream;
                if (fileStream != null) fileName = fileStream.Name;
                try
                {
                    if (!string.IsNullOrEmpty(fileName)) fileName = Path.GetFileNameWithoutExtension(fileName);
                }
                catch
                {
                }
                if (string.IsNullOrEmpty(fileName)) fileName = report.ReportName;
                if (string.IsNullOrEmpty(fileName)) fileName = "report";

                zip = new StiZipWriter20();
                zip.Begin(stream, true);
                ms = new MemoryStream();
                stream = ms;
            }
            #endregion

            baseStream = stream;
            streamWriter = new StreamWriter(stream, settings.Encoding);
            HtmlWriter = new StiHtmlTextWriter(streamWriter);
            var pages = settings.PageRange.GetSelectedPages(report.RenderedPages);

            //useBookmarks &= (report.Bookmark != null) && (report.Bookmark.Bookmarks.Count != 0);

            currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                //StiExportUtils.DisableFontSmoothing();

                var reportCulture = currentCulture;
                if (!string.IsNullOrEmpty(report.Culture))
                {
                    try
                    {
                        reportCulture = new CultureInfo(report.Culture);
                    }
                    catch
                    {
                    }
                }

                Thread.CurrentThread.CurrentCulture = reportCulture;

                this.report = report;

                RenderStartDoc(pages, settings.Encoding);

                #region Trial
#if CLOUD
                var isTrial = !StiCloudPlan.IsReportsAvailable(report.ReportGuid);
#elif SERVER
                var isTrial = StiVersionX.IsSvr;
#else
                var key = StiLicenseKeyValidator.GetLicenseKey();

                var isValidInDesigner = StiLicenseKeyValidator.IsValidInReportsDesignerOrOnPlatform(StiProductIdent.Net, key);
                var isTrial = !(isValidInDesigner && Base.Design.StiDesignerAppStatus.IsRunning || StiLicenseKeyValidator.IsValidOnNetFramework(key));

                if (!typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key))
                    isTrial = true;

                #region IsValidLicenseKey
                if (!isTrial)
                {
                    try
                    {
                        using (var rsa = new RSACryptoServiceProvider(512))
                        using (var sha = new SHA1CryptoServiceProvider())
                        {
                            rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                            isTrial = !rsa.VerifyData(key.GetCheckBytes(), sha, key.GetSignatureBytes());
                        }
                    }
                    catch (Exception)
                    {
                        isTrial = true;
                    }
                }
                #endregion
#endif
                if (isTrial)
                    HtmlWriter.Write("<div style=\"FONT-SIZE: 10px; COLOR: red; FONT-FAMILY: Arial\">Stimulsoft Reports - Trial</div>");
                #endregion

                StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");

                HtmlWriter.Indent = 0;

                int clipCounter = 1;
                foreach (StiPage page in pages)
                {
                    pages.GetPage(page);

                    this.InvokeExporting(page, pages, 0, 1);
                    if (IsStopped) return;

                    RenderPage(report, page, ref clipCounter);

                    if (continuousPages)
                    {
                        HtmlWriter.Write("<br/>");
                    }
                    HtmlWriter.WriteLine();
                }

                RenderEndDoc();

                HtmlWriter.Flush();
                streamWriter.Flush();
            }
            finally
            {
                StiExportUtils.EnableFontSmoothing(report);
                Thread.CurrentThread.CurrentCulture = currentCulture;

                report = null;
            }

            #region CompressToArchive, part2
            if (settings.CompressToArchive)
            {
                zip.AddFile(fileName + "." + "html", ms);
                zip.End();
                zip = null;
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// Creates an instance of the class for the HTML export.
        /// </summary>
        public StiHtml5ExportService()
        {
            numberFormat.NumberDecimalSeparator = ".";
        }
    }
}