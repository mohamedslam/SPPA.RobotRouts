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
using Stimulsoft.Base.Zip;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Drawing.Imaging;
using Stimulsoft.Report.Helpers;

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the export in the MHT format.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceMht.png")]
	public class StiMhtExportService : StiExportService
    {
        #region StiExportService override
        /// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension => compressToArchive ? "zip" : "mht";

		public override StiExportFormat ExportFormat => StiExportFormat.Mht;

		/// <summary>
        /// Gets a group of the export in the context menu.
		/// </summary>
		public override string GroupCategory => "Web";

		/// <summary>
        /// Gets a position of the export in the context menu.
		/// </summary>
		public override int Position => (int)StiExportPosition.Mht;

        /// <summary>
        /// Gets a name of the export in the context menu.
        /// </summary>
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeMhtFile");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportMht(report, stream, settings as StiHtmlExportSettings);
			InvokeExporting(100, 100, 1, 1);
		}

        /// <summary>
        /// Exports a rendered report to the MHT format.
        /// Also file may be sent via e-mail.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">If true then the result of the exporting will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
		{
            using (var form = StiGuiOptions.GetExportFormRunner("StiHtmlExportSetupForm", guiMode, this.OwnerWindow))
            {
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;
                form["ExportFormat"] = StiExportFormat.Mht;

                this.report = report;
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
        /// Returns a filter for Mht files.
        /// </summary>
        /// <returns>Returns a filter for Mht files.</returns>
		public override string GetFilter()
		{
            if (compressToArchive)
                return StiLocalization.Get("FileFilters", "ZipArchives");
			return StiLocalization.Get("FileFilters", "MhtFiles");
		}
		#endregion

		#region Handlers
		private void Form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
		{
			if (e.DialogResult)
			{
				if (string.IsNullOrEmpty(documentFileName))
					documentFileName = base.GetFileName(report, sendEMail);

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
								Zoom = (float)form["Zoom"],
								ImageFormat = (ImageFormat)form["ImageFormat"],
								PageRange = form["PagesRange"] as StiPagesRange,
								ExportMode = (StiHtmlExportMode)form["ExportMode"],
								ExportQuality = StiHtmlExportQuality.High,
								AddPageBreaks = (bool)form["AddPageBreaks"],
								Encoding = Encoding.UTF8
							};

							this.fileName = documentFileName;

							base.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], documentFileName, guiMode);
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

		#region Fields
		internal string fileName = string.Empty;
        internal bool compressToArchive;
		private StiReport report;
		private string documentFileName;
		private bool sendEMail;
		private StiGuiMode guiMode;
		#endregion

		#region Methods
		/// <summary>
		/// Exports a rendered report to the MHT format.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="fileName">A name of the file for exporting a rendered report.</param>
		public void ExportMht(StiReport report, string fileName)
		{
			StiFileUtils.ProcessReadOnly(fileName);
			using (var stream = new FileStream(fileName, FileMode.Create))
			{
				ExportMht(report, stream);
				stream.Flush();
				stream.Close();
			}
		}

       
		/// <summary>
        /// Exports a rendered report to the MHT format.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        public void ExportMht(StiReport report, Stream stream)
		{
			ExportMht(report, stream, new StiHtmlExportSettings());
		}

       
		/// <summary>
        /// Exports a rendered report to the MHT format.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies a format of the images in the resulted Mht document.</param>
        public void ExportMht(StiReport report, Stream stream, double zoom, ImageFormat imageFormat)
		{
            var settings = new StiHtmlExportSettings
            {
                ImageFormat = imageFormat
            };

            ExportMht(report, stream, settings);
		}

       
		/// <summary>
        /// Exports a rendered report to the MHT format.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportMht(StiReport report, Stream stream, StiPagesRange pageRange)
		{
            var settings = new StiHtmlExportSettings
            {
                PageRange = pageRange
            };

            ExportMht(report, stream, settings);
		}

       
		/// <summary>
        /// Exports a rendered report to the MHT format.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies a format of the images in the resulted Mht document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportMht(StiReport report, Stream stream, double zoom, ImageFormat imageFormat, StiPagesRange pageRange)
		{
            var settings = new StiHtmlExportSettings
            {
                PageRange = pageRange,
                Zoom = zoom,
                ImageFormat = imageFormat
            };

            ExportMht(report, stream, settings);
		}

       
		/// <summary>
        /// Exports a rendered report to the MHT format.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies a format of the images in the resulted Mht document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="exportMode">Sets mode of the export (Span or Table).</param>
        /// <param name="encoding">A parameter that controls the character encoding of the resulted document.</param>
		public void ExportMht(StiReport report, Stream stream, double zoom, ImageFormat imageFormat, 
			StiPagesRange pageRange, StiHtmlExportMode exportMode, Encoding encoding)
		{
            var settings = new StiHtmlExportSettings
            {
                PageRange = pageRange,
                Zoom = zoom,
                ImageFormat = imageFormat,
                ExportMode = exportMode,
                Encoding = encoding
            };

            ExportMht(report, stream, settings);
		}


		/// <summary>
		/// Exports a rendered report to the MHT format.
		/// </summary>
        public void ExportMht(StiReport report, Stream stream, StiHtmlExportSettings settings)
		{
			StiLogService.Write(this.GetType(), "Export report to Mht format");

			#region Read settings
			if (settings == null)
				throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

			var pageRange = settings.PageRange;
			var encoding = settings.Encoding;
			this.compressToArchive = settings.CompressToArchive;
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

			var currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

				var htmlService = new StiHtmlExportService();
				htmlService.HtmlImageHost = new StiMhtImageHost(htmlService);

				#region Prepare main stream and images
				var memMain = new MemoryStream();
				var streamWriter = new StreamWriter(memMain, encoding);
				var writer = new StiHtmlTextWriter(streamWriter);
				var pages = pageRange.GetSelectedPages(report.RenderedPages);
				htmlService.fileName = fileName;
				var host = htmlService.HtmlImageHost as StiMhtImageHost;
//				host.imageList = new ArrayList();
				htmlService.Progress = this.Progress;

				var htmlSettings = new StiHtmlExportSettings
				{
				    AddPageBreaks = settings.AddPageBreaks,
				    Encoding = settings.Encoding,
				    ExportMode = settings.ExportMode,
				    ExportQuality = settings.ExportQuality,
				    ImageFormat = settings.ImageFormat,
				    PageRange = settings.PageRange,
				    Zoom = settings.Zoom,
				    ExportBookmarksMode = settings.ExportBookmarksMode,
				    BookmarksTreeWidth = settings.BookmarksTreeWidth,
				    ImageQuality = settings.ImageQuality,
				    ImageResolution = settings.ImageResolution
				};

			    htmlService.ExportHtml(report, writer, htmlSettings, settings.PageRange.GetSelectedPages(report.RenderedPages));
				streamWriter.Flush();
				memMain.WriteByte(0x0D);
				memMain.WriteByte(0x0A);
				#endregion

				var dt = DateTime.Now;
				string boundary = "----=_NextPart_000_0000_" +
					dt.Year.ToString("X4") +
					dt.Month.ToString("X2") +
					dt.Day.ToString("X2") + "." +
					dt.Hour.ToString("X2") +
					dt.Minute.ToString("X2") +
					dt.Second.ToString("X2");

				var baseDir = string.Empty;
				if (!string.IsNullOrEmpty(htmlService.fileName))
				{
					baseDir = Path.GetDirectoryName(htmlService.fileName);
				}
				if (baseDir.Length > 0 && baseDir[baseDir.Length - 1] != '\\') baseDir += '\\';
				string dir = Path.GetFileNameWithoutExtension(fileName) + ".files\\";
				string fileNameFull = "file:///" + baseDir + Path.GetFileNameWithoutExtension(fileName) + ".html";

				var sw = new StreamWriter(stream, Encoding.ASCII);

				sw.WriteLine("From: <Saved by Stimulsoft Reports>");
				sw.WriteLine("Subject: {0}", report.ReportName);
				sw.WriteLine("Date: {0}", dt.ToString("r"));
				sw.WriteLine("MIME-Version: 1.0");
				sw.WriteLine("Content-Type: multipart/related;");
				sw.WriteLine("	type=\"text/html\";");
				sw.WriteLine("	boundary=\"{0}\"", boundary);

				sw.WriteLine("");
				sw.WriteLine("--{0}", boundary);
				sw.WriteLine("Content-Type: text/html;");
				sw.WriteLine("	charset=\"{0}\"", encoding.WebName);
				sw.WriteLine("Content-Transfer-Encoding: quoted-printable");
				sw.WriteLine("Content-Location: {0}", StiExportUtils.StringToUrl(fileNameFull.Replace("\\", "/").Replace(" ", "_")));
				sw.WriteLine("");
				sw.Flush();

				#region write main stream; use quoted-printable encoding
				memMain.Seek(0, SeekOrigin.Begin);
				var sb = new StringBuilder();
				for (int index = 0; index < memMain.Length; index ++)
				{
					int sym = memMain.ReadByte();

					if (sym == 0x0A) continue;

					if ((sym != 0x0D) && (sb.Length > 72))
					{
						sb.Append("=");
						sb.Append((char)0x0D);
						sb.Append((char)0x0A);
						for (int tempIndex = 0; tempIndex < sb.Length; tempIndex ++)
						{
							stream.WriteByte((byte)sb[tempIndex]);
						}
						sb.Length = 0;
					}

					if ((sym >= 32) && (sym <= 126) && (sym != 61))
					{
						sb.Append((char)sym);
					}
					else
					{
						if (sym == 0x0D)
						{
							if ((sb.Length > 0) && (sb[sb.Length - 1] == ' '))
							{
								sb.Length --;
								sb.Append("=20");
							}
							sb.Append((char)0x0D);
							sb.Append((char)0x0A);
							for (int tempIndex = 0; tempIndex < sb.Length; tempIndex ++)
							{
								stream.WriteByte((byte)sb[tempIndex]);
							}
							sb.Length = 0;
						}
						else
						{
							sb.AppendFormat("=" + sym.ToString("X2"));
						}
					}
				}
				memMain.Close();
				stream.Flush();
				#endregion

				#region write images
				for (int index = 0; index < host.ImageCache.ImagePackedStore.Count; index ++)
				{
					var currImageFormat = (ImageFormat)host.ImageCache.ImageFormatStore[index];
					var imageType = (currImageFormat == ImageFormat.Jpeg) ? "jpeg" : "gif";
					if (currImageFormat == ImageFormat.Png) imageType = "png";

					var imageStr = "file:///" + baseDir + dir + Path.GetFileNameWithoutExtension(fileName)
						+ (index + 1).ToString() + "." + currImageFormat.ToString();
					imageStr = StiExportUtils.StringToUrl(imageStr.Replace("\\", "/").Replace(" ", "_"));

					sw.WriteLine("");
					sw.WriteLine("--{0}", boundary);
					sw.WriteLine("Content-Type: image/{0}", imageType);
					sw.WriteLine("Content-Transfer-Encoding: base64");
					sw.WriteLine("Content-Location: {0}", imageStr);
					sw.WriteLine("");
					sw.Flush();

					var buf = (byte[])host.ImageCache.ImagePackedStore[index];
					var st = Convert.ToBase64String(buf, 0, buf.Length);
					int counter = 0;					
					for (int tempIndex = 0; tempIndex < st.Length; tempIndex++)
					{
						stream.WriteByte((byte)st[tempIndex]);
						counter++;
						if (counter == 76)
						{
							stream.WriteByte(0x0D);
							stream.WriteByte(0x0A);
							counter = 0;
						}
					}
					stream.WriteByte(0x0D);
					stream.WriteByte(0x0A);
					stream.Flush();
				}
//				host.imageList.Clear();
//				host.imageList = null;
				host.ImageCache.Clear();
				#endregion

				sw.WriteLine("");
				sw.WriteLine("--{0}--", boundary);
				sw.Flush();

			}
			finally
			{
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }

            #region CompressToArchive, part2
            if (settings.CompressToArchive)
            {
                zip.AddFile(fileName + "." + "mht", ms);
                zip.End();
                zip = null;
            }
            #endregion
        }
		#endregion
	}	
}