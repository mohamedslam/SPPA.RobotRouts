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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Zip;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters;
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Stimulsoft.Report.Helpers;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using Encoder = Stimulsoft.Drawing.Imaging.Encoder;
using EncoderParameter = Stimulsoft.Drawing.Imaging.EncoderParameter;
using EncoderParameters = Stimulsoft.Drawing.Imaging.EncoderParameters;
using ImageAttributes = Stimulsoft.Drawing.Imaging.ImageAttributes;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// Class for exporting reports in the image format.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceImage.png")]
	public class StiImageExportService : StiExportService
    {
        #region StiExportService override
        /// <summary>
        /// Gets or sets a default extension of export. 
        /// </summary>
        public override string DefaultExtension => (compressToArchive) ? "zip" : GetExtensionFromSettings();

        private string GetExtensionFromSettings()
        {
            if (imageSettings is StiBmpExportSettings || imageSettings.ImageType == StiImageType.Bmp) return "bmp";
            if (imageSettings is StiEmfExportSettings || imageSettings.ImageType == StiImageType.Emf) return "emf";
            if (imageSettings is StiGifExportSettings || imageSettings.ImageType == StiImageType.Gif) return "gif";
            if (imageSettings is StiJpegExportSettings || imageSettings.ImageType == StiImageType.Jpeg) return "jpg";
            if (imageSettings is StiPcxExportSettings || imageSettings.ImageType == StiImageType.Pcx) return "pcx";
            if (imageSettings is StiPngExportSettings || imageSettings.ImageType == StiImageType.Png) return "png";
            if (imageSettings is StiSvgExportSettings || imageSettings.ImageType == StiImageType.Svg) return "svg";
            if (imageSettings is StiSvgzExportSettings || imageSettings.ImageType == StiImageType.Svgz) return "svgz";
            if (imageSettings is StiTiffExportSettings || imageSettings.ImageType == StiImageType.Tiff) return "tiff";
            return "jpg";
        }

        public override StiExportFormat ExportFormat
        {
            get
            {
                if (imageSettings != null && imageSettings.GetType() == typeof(StiImageExportSettings)) return StiExportFormat.Image;

                if (imageSettings is StiBmpExportSettings || imageSettings.ImageType == StiImageType.Bmp) return StiExportFormat.ImageBmp;
                if (imageSettings is StiEmfExportSettings || imageSettings.ImageType == StiImageType.Emf) return StiExportFormat.ImageEmf;
                if (imageSettings is StiGifExportSettings || imageSettings.ImageType == StiImageType.Gif) return StiExportFormat.ImageGif;
                if (imageSettings is StiJpegExportSettings || imageSettings.ImageType == StiImageType.Jpeg) return StiExportFormat.ImageJpeg;
                if (imageSettings is StiPcxExportSettings || imageSettings.ImageType == StiImageType.Pcx) return StiExportFormat.ImagePcx;
                if (imageSettings is StiPngExportSettings || imageSettings.ImageType == StiImageType.Png) return StiExportFormat.ImagePng;
                if (imageSettings is StiSvgExportSettings || imageSettings.ImageType == StiImageType.Svg) return StiExportFormat.ImageSvg;
                if (imageSettings is StiSvgzExportSettings || imageSettings.ImageType == StiImageType.Svgz) return StiExportFormat.ImageSvgz;
                if (imageSettings is StiTiffExportSettings || imageSettings.ImageType == StiImageType.Tiff) return StiExportFormat.ImageTiff;

                return StiExportFormat.Image;
            }
        }

        /// <summary>
        /// Gets a group of the export in the context menu.
        /// </summary>
        public override string GroupCategory
        {
            get
            {
                if (this is StiEmfExportService || 
                    this is StiSvgExportService ||
                    this is StiSvgzExportService) 
                    return "Meta"; 
                else
                    return "Image";
            }
        }

        /// <summary>
        /// Gets a name of the export in the context menu.
        /// </summary>
        public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeImageFile");

        /// <summary>
        /// Gets a position of the export in the context menu.
        /// </summary>
        public override int Position => (int)StiExportPosition.Image;

        /// <summary>
        /// Returns a filter for files with bmp images.
        /// </summary>
        /// <returns>Returns a filter for files with bmp images.</returns>
        public override string GetFilter()
        {
            if (compressToArchive)
                return StiLocalization.Get("FileFilters", "ZipArchives");

            if (imageSettings is StiBmpExportSettings || imageSettings.ImageType == StiImageType.Bmp) return StiLocalization.Get("FileFilters", "BmpFiles");
            if (imageSettings is StiEmfExportSettings || imageSettings.ImageType == StiImageType.Emf) return StiLocalization.Get("FileFilters", "EmfFiles");
            if (imageSettings is StiGifExportSettings || imageSettings.ImageType == StiImageType.Gif) return StiLocalization.Get("FileFilters", "GifFiles");
            if (imageSettings is StiJpegExportSettings || imageSettings.ImageType == StiImageType.Jpeg) return StiLocalization.Get("FileFilters", "JpegFiles");
            if (imageSettings is StiPcxExportSettings || imageSettings.ImageType == StiImageType.Pcx) return StiLocalization.Get("FileFilters", "PcxFiles");
            if (imageSettings is StiPngExportSettings || imageSettings.ImageType == StiImageType.Png) return StiLocalization.Get("FileFilters", "PngFiles");
            if (imageSettings is StiSvgExportSettings || imageSettings.ImageType == StiImageType.Svg) return StiLocalization.Get("FileFilters", "SvgFiles");
            if (imageSettings is StiSvgzExportSettings || imageSettings.ImageType == StiImageType.Svgz) return StiLocalization.Get("FileFilters", "SvgzFiles");
            if (imageSettings is StiTiffExportSettings || imageSettings.ImageType == StiImageType.Tiff) return StiLocalization.Get("FileFilters", "TiffFiles");

            return StiLocalization.Get("FileFilters", "JpegFiles");
        }
        #endregion

        #region StiExportService override
        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportImage(report, stream, settings as StiImageExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }

        /// <summary>
        /// Exports a rendered document to the file as image.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
		public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
        {
            using (var form = StiGuiOptions.GetExportFormRunner("StiImageSetupForm", guiMode, this.OwnerWindow))
            {
                //var settings = GetSettings();
                //form["ImageType"] = settings.ImageType;
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;
				
                //form["MultipleFilesEnabled"] = this is StiTiffExportService;
                //form["TiffCompressionSchemeEnabled"] = this is StiTiffExportService;
                //form["MonochromeEnabled"] = !((this is StiEmfExportService) || (this is StiSvgExportService) || (this is StiSvgzExportService));

                this.report = report;
                this.fileName = fileName;
                this.sendEMail = sendEMail;
                this.guiMode = guiMode;

                form.Complete += Form_Complete;
                form.ShowDialog();
			}
		}
        #endregion

        #region Handlers
        private void Form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
        {
            if (e.DialogResult)
            {
                this.MultipleFiles = (bool)form["MultipleFiles"];
                compressToArchive = (bool)form["CompressToArchive"];

                #region Create Settings
                this.imageSettings = new StiImageExportSettings
                {
                    ImageType = (StiImageType)form["ImageType"],
                    ImageZoom = (double)form["ImageZoom"],
                    ImageResolution = (int)form["ImageResolution"],
                    CutEdges = (bool)form["CutEdges"],
                    ImageFormat = (StiImageFormat)form["ImageFormat"],
                    PageRange = form["PagesRange"] as StiPagesRange,
                    MultipleFiles = (bool)form["MultipleFiles"],
                    TiffCompressionScheme = (StiTiffCompressionScheme)form["TiffCompressionScheme"],
                    DitheringType = (StiMonochromeDitheringType)form["MonochromeDitheringType"],
                    CompressToArchive = (bool)form["CompressToArchive"]
                };
                #endregion

                if (string.IsNullOrEmpty(fileName))
                    fileName = this.GetFileName(report, sendEMail);

                if (!string.IsNullOrEmpty(fileName))
                {
                    StiFileUtils.ProcessReadOnly(fileName);
                    try
                    {
                        using (var stream = new FileStream(fileName, FileMode.Create))
                        {
                            StartProgress(guiMode);

                            this.StartExport(report, stream, imageSettings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
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
        internal StiImageExportSettings imageSettings;
        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        internal bool compressToArchive = false;
        #endregion

        #region Methods.Helpers
        private Image Draw(StiPage page, double imageZoomBase, int imageResolution, bool cutEdges, StiImageFormat imageFormat, Stream stream, bool emf)
		{
            var imageZoom = imageZoomBase * (imageResolution / 100f);

            var zoom = page.Report.Info.Zoom;
			page.Report.Info.Zoom = imageZoom;
			var unit = page.Unit;
            StiScale.Lock();

            double dWidth = unit.ConvertToHInches(page.DisplayRectangle.Width) * imageZoom;
            double dHeight = unit.ConvertToHInches(page.DisplayRectangle.Height) * imageZoom;

            int imageWidth = (int)dWidth;
			int imageHeight = (int)dHeight;

            if (imageWidth < dWidth)
                imageWidth++;

            if (imageHeight < dHeight)
                imageHeight++;

            double marginLeft = page.Margins.Left;
			double marginRight = page.Margins.Right;
			double marginTop = page.Margins.Top;
			double marginBottom = page.Margins.Bottom;

            if (cutEdges)
            {
                double mc = StiOptions.Export.ImageCutEdgesMode == StiImageCutEdgesMode.Full ? 1 : 0.8;
                imageWidth = (int)(unit.ConvertToHInches(page.DisplayRectangle.Width - marginLeft * mc - marginRight * mc) * imageZoom);
                imageHeight = (int)(unit.ConvertToHInches(page.DisplayRectangle.Height - marginTop * mc - marginBottom * mc) * imageZoom);
            }

            var bmp = emf && StiOptions.Engine.FullTrust 
                ? CreateMetafileInFullTrust(stream) 
                : new Bitmap(imageWidth, imageHeight);

            if (page.Report.RenderedWith == StiRenderedWith.Wpf)
            {
                try
                {
                    page.DenyDrawSegmentMode = true;

                    var painter = StiPainter.GetPainter(typeof(StiPage), StiGuiMode.Wpf);
                    float tempZoom = (float)imageZoom;
                    var tempImage = painter.GetImage(page, ref tempZoom, StiExportFormat.ImagePng);

                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        if (cutEdges)
                        {
                            double mc = StiOptions.Export.ImageCutEdgesMode == StiImageCutEdgesMode.Full ? 1 : 0.8;
                            int scaledMarginleft = (int)(unit.ConvertToHInches(marginLeft * mc) * imageZoom);
                            int scaledMarginTop = (int)(unit.ConvertToHInches(marginTop * mc) * imageZoom);

                            g.DrawImage(tempImage, 
                                new Rectangle(0, 0, imageWidth, imageHeight), 
                                new Rectangle(scaledMarginleft, scaledMarginTop, imageWidth, imageHeight), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(tempImage, new Rectangle(0, 0, imageWidth, imageHeight));
                        }
                    }
                }
                finally
                {
                    page.DenyDrawSegmentMode = false;
                }
            }
            else
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    if (cutEdges)
                    {
                        double mc = StiOptions.Export.ImageCutEdgesMode == StiImageCutEdgesMode.Full ? 1 : 0.8;
                        g.TranslateTransform(
                            (float)(unit.ConvertToHInches(-marginLeft * mc) * imageZoom),
                            (float)(unit.ConvertToHInches(-marginTop * mc) * imageZoom));
                    }

                    try
                    {
                        page.DenyDrawSegmentMode = true;

                        var painter = StiPainter.GetPainter(typeof(StiPage), StiGuiMode.Gdi) as StiPageGdiPainter;
                        painter.Paint(page, new StiPaintEventArgs(g, RectangleD.Empty));
                    }
                    finally
                    {
                        page.DenyDrawSegmentMode = false;
                    }
                }
            }
			page.Report.Info.Zoom = zoom;
            StiScale.Unlock();

            #region Gray colors
            if (imageFormat != StiImageFormat.Color)
            {
                var newBmp = new Bitmap(bmp.Width, bmp.Height);
                using (var g = Graphics.FromImage(newBmp))
                {
                    var matrix = new ColorMatrix(new float[][]
                    {
                        new float[]{0.3f, 0.3f, 0.3f, 0, 0},
                        new float[]{0.55f, 0.55f, 0.55f, 0, 0},
                        new float[]{0.15f, 0.15f, 0.15f, 0, 0},
                        new float[]{0, 0, 0, 1, 0, 0},
                        new float[]{0, 0, 0, 0, 1, 0},
                        new float[]{0, 0, 0, 0, 0, 1}
                    });

                    var attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix);
                    g.DrawImage(bmp,
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                    
                    bmp = newBmp;
                }                
            }
            #endregion

            return bmp;
		}

        private Image CreateMetafileInFullTrust(Stream stream)
        {
            Image bmp = null;
            using (var bmpTemp = new Bitmap(1, 1))
            using (var grfx = Graphics.FromImage(bmpTemp))
            {
                var ipHdc = grfx.GetHdc();
                bmp = new Metafile(stream, ipHdc);
                grfx.ReleaseHdc(ipHdc);
            }
            return bmp;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exports a rendered document to the file as image.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="imageFormat">Returns format of the image.</param>
		public void ExportImage(StiReport report, string fileName, StiImageFormat imageFormat)
		{
			StiFileUtils.ProcessReadOnly(fileName);
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                ExportImage(report, stream, imageFormat);
                stream.Flush();
                stream.Close();
            }
		}

		
		/// <summary>
        /// Exports a rendered document to the stream as image.
		/// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
		/// <param name="imageFormat">A parameter which sets format of the resulted image.</param>
		public virtual void ExportImage(StiReport report, Stream stream, StiImageFormat imageFormat)
		{
			ExportImage(report, stream, imageFormat, StiPagesRange.All);
		}

		
		/// <summary>
        /// Exports a rendered document to the stream as an image.
		/// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="imageFormat">A parameter which sets a format of the resulted image.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
		public virtual void ExportImage(StiReport report, Stream stream, StiImageFormat imageFormat, StiPagesRange pageRange)
		{
			ExportImage(report, stream, 1, false, imageFormat, pageRange);
		}

		
		/// <summary>
        /// Exports a rendered document to the stream as an image.
		/// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
		/// <param name="imageZoom">A parameter which sets zoom of the resulted image.</param>
		/// <param name="cutEdges">A parameter which cuts edges of the resulted image after the export.</param>
        /// <param name="imageFormat">A parameter which sets format of the resulted image.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
		public virtual void ExportImage(StiReport report, Stream stream, double imageZoom, bool cutEdges, StiImageFormat imageFormat, StiPagesRange pageRange)
		{
			if (this is StiEmfExportService)
			{
                var settings = new StiEmfExportSettings
                {
                    PageRange = pageRange
                };

                ExportImage(report, stream, settings);
			}
			else
			{
                var settings = new StiImageExportSettings
                {
                    PageRange = pageRange,
                    ImageZoom = imageZoom,
                    CutEdges = cutEdges,
                    ImageFormat = imageFormat
                };

                ExportImage(report, stream, settings);
			}
		}


		/// <summary>
		/// Exports a rendered document to the stream as an image.
		/// </summary>
        public virtual void ExportImage(StiReport report, Stream stream, StiImageExportSettings settings)
        {
            if (settings == null)
                settings = GetSettings();

            #region Export Dashboard
            if (!report.IsDocument && report.GetCurrentPage() is IStiDashboard)
            {
                StiDashboardExport.Export(report, stream, settings);
                return;
            }
            #endregion

            try
            {
                //StiExportUtils.DisableFontSmoothing();
                ExportImage1(report, stream, settings);
            }
            finally
            {
                StiExportUtils.EnableFontSmoothing(report);
            }
        }

        private StiImageExportSettings GetSettings()
        {
            if (this is StiBmpExportService) return new StiImageExportSettings(StiImageType.Bmp);
            if (this is StiEmfExportService) return new StiImageExportSettings(StiImageType.Emf);
            if (this is StiGifExportService) return new StiImageExportSettings(StiImageType.Gif);
            if (this is StiJpegExportService) return new StiImageExportSettings(StiImageType.Jpeg);
            if (this is StiPcxExportService) return new StiImageExportSettings(StiImageType.Pcx);
            if (this is StiPngExportService) return new StiImageExportSettings(StiImageType.Png);
            if (this is StiSvgExportService) return new StiImageExportSettings(StiImageType.Svg);
            if (this is StiSvgzExportService) return new StiImageExportSettings(StiImageType.Svgz);
            if (this is StiTiffExportService) return new StiImageExportSettings(StiImageType.Tiff);

            return new StiImageExportSettings(StiImageType.Jpeg);
        }

		/// <summary>
		/// Exports a rendered document to the stream as an image.
		/// </summary>
        private void ExportImage1(StiReport report, Stream baseStream, StiImageExportSettings settings)
        {
            StiLogService.Write(this.GetType(), string.Format("Export report to {0} format ", settings.ImageType));

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");
            else
                this.imageSettings = settings;

            var pageRange = settings.PageRange;
            var imageZoom = settings.ImageZoom;
            int imageResolution = settings.ImageResolution;
            var cutEdges = settings.CutEdges;
            var imageFormat = settings.ImageFormat;
            var ditheringType = settings.DitheringType;
            var tiffCompressionScheme = settings.TiffCompressionScheme;
            MultipleFiles = settings.MultipleFiles;
            compressToArchive = settings.CompressToArchive;
            #endregion

            var pages = pageRange.GetSelectedPages(report.RenderedPages);

            //for multipage
            Image baseImage = null;
            var encoder = Encoder.SaveFlag;
            var encoderParameters = new EncoderParameters(1);
            var imageCodecInfo = StiImageCodecInfo.GetImageCodec("image/tiff");

            var isGdi = true;
#if STIDRAWING
            isGdi = Graphics.GraphicsEngine == Stimulsoft.Drawing.GraphicsEngine.Gdi;
#endif

            if (tiffCompressionScheme != StiTiffCompressionScheme.Default)
            {
                if (MultipleFiles)
                {
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, (int)tiffCompressionScheme);
                }
                else
                {
                    encoderParameters = new EncoderParameters(2);
                    encoderParameters.Param[1] = new EncoderParameter(Encoder.Compression, (int)tiffCompressionScheme);
                }
            }

            var stream = baseStream;
            StiZipWriter20 zip = null;
            string name = string.Empty;

            if (compressToArchive)
            {
                var fileStream = stream as FileStream;
                if (fileStream != null) name = fileStream.Name;
                try
                {
                    if (!string.IsNullOrWhiteSpace(name)) name = Path.GetFileNameWithoutExtension(name);
                }
                catch { }
                if (string.IsNullOrWhiteSpace(name)) name = report.ReportName;
                if (string.IsNullOrWhiteSpace(name)) name = "report";
                if (pages.Count > 1)
                {
                    string firstOrder = GetOrderFileName(null, 1, report.RenderedPages.Count, null);
                    if (name.EndsWith(firstOrder))
                    {
                        name = name.Substring(0, name.Length - firstOrder.Length);
                    }
                }

                zip = new StiZipWriter20();
                zip.Begin(baseStream, true);
                stream = new MemoryStream();
            }
            else
            {
                if (baseStream is FileStream && pages.Count > 1)
                {
                    var file = baseStream as FileStream;
                    name = Path.ChangeExtension(file.Name, "");
                    name = name.Substring(0, name.Length - 1);

                    string firstOrder = GetOrderFileName(null, 1, report.RenderedPages.Count, null);
                    if (name.EndsWith(firstOrder))
                    {
                        name = name.Substring(0, name.Length - firstOrder.Length);
                    }
                }
            }

            StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");

            int index = 1;
            var format = settings.ImageType;
            if ((format == StiImageType.Emf) && !StiOptions.Engine.FullTrust) format = StiImageType.Png;

            foreach (StiPage page in pages)
            {
                pages.GetPage(page);

                InvokeExporting(page, pages, 0, 1);
                if (IsStopped) return;

                Image image = null;
                if (!(settings.ImageType == StiImageType.Svg || settings.ImageType == StiImageType.Svgz))
                {
                    image = Draw(page, imageZoom, imageResolution, cutEdges, imageFormat, stream, format == StiImageType.Emf);
                    if (tiffCompressionScheme == StiTiffCompressionScheme.CCITT3 ||
                        tiffCompressionScheme == StiTiffCompressionScheme.CCITT4 ||
                        tiffCompressionScheme == StiTiffCompressionScheme.Rle ||
                        imageFormat == StiImageFormat.Monochrome)
                    {
                        image = StiTiffHelper.MakeMonochromeImage(image, ditheringType, 128);
                    }
                    if (format != StiImageType.Emf)
                    {
                        (image as Bitmap).SetResolution(imageResolution, imageResolution);
                    }
                }

                if ((format == StiImageType.Tiff) && (!MultipleFiles) && isGdi)
                {
                    #region Tiff multipage
                    if (index == 1)
                    {
                        // Save the first page (frame).
                        baseImage = image;
                        encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.MultiFrame);
                        baseImage.Save(stream, imageCodecInfo, encoderParameters);
                    }
                    else
                    {
                        // Save other pages (frames).
                        encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.FrameDimensionPage);
                        baseImage.SaveAdd(image, encoderParameters);
                    }
                    if (index == pages.Count)
                    {
                        // Close the multiple-frame file.
                        encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.Flush);
                        baseImage.SaveAdd(encoderParameters);
                        if (compressToArchive)
                        {
                            stream.Flush();
                            zip.AddFile($"{name}.{GetExtensionFromSettings()}", stream as MemoryStream);
                            stream.Close();
                        }
                    }
                    if ((image != null) && (index != 1)) image.Dispose();
                    index++;
                    #endregion
                }
                else
                {
                    if (format != StiImageType.Emf)
                    {
                        if (format == StiImageType.Pcx)
                        {
                            StiPcxPaletteType paletteType = StiPcxPaletteType.Color;
                            if (imageFormat == StiImageFormat.Monochrome) paletteType = StiPcxPaletteType.Monochrome;
                            StiPcxHelper.SaveToStream(image, paletteType, ditheringType, stream);
                        }
                        else if ((format == StiImageType.Bmp) && (imageFormat == StiImageFormat.Monochrome))
                        {
                            StiBmpHelper.SaveToStreamMonochrome(image, ditheringType, stream);
                        }
                        else if (format == StiImageType.Svg || format == StiImageType.Svgz)
                        {
                            StiSvgHelper.SaveToStream(report, page, stream, format == StiImageType.Svgz);
                        }
                        else if (format == StiImageType.Tiff && tiffCompressionScheme != StiTiffCompressionScheme.Default)
                        {
                            image.Save(stream, imageCodecInfo, encoderParameters);
                        }
                        else
                        {
                            #region ImageFormat
                            var gdiFormat = ImageFormat.Bmp;
                            switch (format)
                            {
                                case StiImageType.Bmp:
                                    gdiFormat = ImageFormat.Bmp;
                                    break;

                                case StiImageType.Emf:
                                    gdiFormat = ImageFormat.Emf;
                                    break;

                                case StiImageType.Gif:
                                    gdiFormat = ImageFormat.Gif;
                                    break;

                                case StiImageType.Jpeg:
                                    gdiFormat = ImageFormat.Jpeg;
                                    break;

                                case StiImageType.Png:
                                    gdiFormat = ImageFormat.Png;
                                    break;

                                case StiImageType.Tiff:
                                    gdiFormat = ImageFormat.Tiff;
                                    break;
                            }
                            #endregion

                            image.Save(stream, gdiFormat);
                        }
                    }

                    if (compressToArchive)
                    {
                        stream.Flush();
                        var fileName = GetOrderFileName(name, index, report.RenderedPages.Count, GetExtensionFromSettings());
                        zip.AddFile(fileName, stream as MemoryStream);
                        stream.Close();

                        if (index < pages.Count)
                        {
                            stream = new MemoryStream();
                        }
                        index++;
                    }
                    else
                    {
                        #region Each page to file
                        if (stream is FileStream && pages.Count > 1)
                        {
                            if (index > 1)
                            {
                                stream.Flush();
                                stream.Close();
                            }
                            if (index < pages.Count)
                            {
                                var fileName = GetOrderFileName(name, index + 1, report.RenderedPages.Count, GetExtensionFromSettings());
                                StiFileUtils.ProcessReadOnly(fileName);
                                stream = new FileStream(fileName, FileMode.Create);
                            }
                            index++;
                        }
                        #endregion
                    }

                    if (image != null) image.Dispose();
                }
            }

            if (compressToArchive)
            {
                zip.End();
                zip = null;
            }

            if (baseImage != null) baseImage.Dispose();
        }
		#endregion

		public StiImageExportService()
		{
			this.MultipleFiles = true;
		}
	}
}