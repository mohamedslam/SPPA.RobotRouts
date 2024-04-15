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
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Zip;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using Stimulsoft.Report.Helpers;

#if CLOUD
using Stimulsoft.Base.Plans;
#endif

#if NETSTANDARD
using Stimulsoft.System.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the Microsoft XML Paper Specification.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceXps.png")]
    public class StiXpsExportService : StiExportService
    {
        #region struct StiXpsData
        /// <summary>
        /// Inner representation of the export objects.
        /// </summary>
        private struct StiXpsData
        {
            public double X;
            public double Y;
            public double Width;
            public double Height;
            public StiComponent Component;

            public double Right => X + Width;
            public double Bottom => Y + Height;
        }
        #endregion

        #region StiExportService override
        /// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension => "xps";

        public override StiExportFormat ExportFormat => StiExportFormat.Xps;

        /// <summary>
        /// Gets a group of the export in the context menu.
        /// </summary>
        public override string GroupCategory => "Document";

        /// <summary>
        /// Gets a position of the export in the context menu.
        /// </summary>
        public override int Position => (int)StiExportPosition.Xps;

        /// <summary>
        /// Gets an export name in the context menu.
        /// </summary>
        public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeXpsFile");

        /// <summary>
        /// Gets a value indicating a number of files in exported document as a result of export
        /// of one page of the rendered report.
        /// </summary>
        public override bool MultipleFiles => false;

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportXps(report, stream, settings as StiXpsExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }

        /// <summary>
        /// Exports a rendered report to the Xps file.
        /// Also exported document can be sent via e-mail.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
        {
            using (var form = StiGuiOptions.GetExportFormRunner("StiXpsExportSetupForm", guiMode, this.OwnerWindow))
            {
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;

                this.report = report;
                this.fileName = fileName;
                this.sendEMail = sendEMail;
                this.guiMode = guiMode;
                form.Complete += FormComplete;
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Returns a filter for the Excel files.
        /// </summary>
        /// <returns>Returns a filter for the Excel files.</returns>
        public override string GetFilter()
        {
            return StiLocalization.Get("FileFilters", "XpsFiles");
        }
        #endregion

        #region Consts
        private const double hiToDpi = 96 / 100f;
        #endregion

        #region Fields
        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        private StiImageCache imageCache;
        private Hashtable pageImages;
        private Hashtable pageFonts;
        private Hashtable pageBookmarks;
        private int indexPage;
        private int xmlIndentation = 1;
        private PdfFonts pdfFont;
        //private StiBidirectionalConvert bidi;
        private int precision_digits = 3;
        private bool reduceFontSize = true;
        private bool useImageComparer = true;
        private float imageQuality = 0.75f;
        private float imageResolution = 96; //dpi
        private double fontCorrectValue = 1.333333f * hiToDpi;
		private bool exportRtfTextAsImage = true;

        private Graphics graphicsForTextRenderer = null;
        private Image imageForGraphicsForTextRenderer = null;
        #endregion

        #region Methods
        private void FormComplete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
        {
            if (!e.DialogResult)
            {
                IsStopped = true;
                return;
            }

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

                        var settings = new StiXpsExportSettings
                        {
                            PageRange = form["PagesRange"] as StiPagesRange,
                            ImageQuality = (float)form["ImageQuality"],
                            ImageResolution = (float)form["Resolution"],
                            ExportRtfTextAsImage = (bool)form["ExportRtfTextAsImage"]
                        };

                        base.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private bool IsWordWrapSymbol(StringBuilder sb, int index)
        {
            var sym = ' ';
            if (index < sb.Length - 1)
                sym = sb[index + 1];

            return IsWordWrapSymbol(sb[index], sym);
        }

        private bool IsWordWrapSymbol(char sym1, char sym2)
        {
            var cat1 = char.GetUnicodeCategory(sym1);
            var cat2 = char.GetUnicodeCategory(sym2);

            return
                cat1 == UnicodeCategory.OpenPunctuation ||
                (cat1 == UnicodeCategory.ClosePunctuation && cat2 != UnicodeCategory.OtherPunctuation) ||
                cat1 == UnicodeCategory.DashPunctuation ||
                cat1 == UnicodeCategory.InitialQuotePunctuation ||
                cat1 == UnicodeCategory.FinalQuotePunctuation ||
                cat1 == UnicodeCategory.SpaceSeparator;
        }

        private string FloatToString(double number)
        {
            var digits = precision_digits;
            if (Math.Abs(number) < 1)
                digits = precision_digits + 2;

            var numValue = Math.Round((decimal)number, digits);
            return numValue.ToString("G");
        }

        private void PrepareData()
        {
            //bidi = new StiBidirectionalConvert(StiBidirectionalConvert.Mode.Xps);
            pdfFont = new PdfFonts
            {
                standardPdfFonts = false,
                MaxSymbols = 32767,
                fontList = new ArrayList()
            };

            imageCache = new StiImageCache(useImageComparer, ImageFormat.Jpeg, imageQuality);
        }

        private float GetTabsSize(IStiTextOptions textOp, double sizeInPt, double currentPosition)
        {
            if (textOp == null || textOp.TextOptions == null) return 0;

            var position = currentPosition;
            var spaceWidth = 750f / (float)sizeInPt;      //empiric; for CourierNew must be 726 ?
            var otherTab = spaceWidth * textOp.TextOptions.DistanceBetweenTabs;
            var firstTab = spaceWidth * textOp.TextOptions.FirstTabOffset + otherTab;

            if (currentPosition < firstTab)
                position = firstTab;

            else
            {
                if (textOp.TextOptions.DistanceBetweenTabs > 0)
                {
                    var kolTabs = (int) ((currentPosition - firstTab) / otherTab);
                    kolTabs++;
                    position = firstTab + kolTabs * otherTab;
                }
            }

            return (float)(position - currentPosition);

        }

        internal static string ConvertToEscapeSequence(string value)
        {
            var escapeChars = "" + (char)13 + (char)10 + (char)8 + (char)12;     //tabs processing in another place
            var replaceChars = "rnbf";
            bool flag;
            string st;

            if (value == null)
                return "";

            st = string.Empty;
            for (var index = 0; index < value.Length; index++)
            {
                flag = false;
                for (var index2 = 0; index2 < escapeChars.Length; index2++)
                {
                    if (value[index] == escapeChars[index2])
                    {
                        st += "\\" + replaceChars[index2];
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                    st += value[index];
            }

            return st;
        }

        private string GetLineStyleDash(StiPenStyle penStyle)
        {
            switch (penStyle)
            {
                case StiPenStyle.Dot:
                    return "1.1 1.1";

                case StiPenStyle.Dash:
                    return "3 1";

                case StiPenStyle.DashDot:
                    return "3 1 1 1";

                case StiPenStyle.DashDotDot:
                    return "3 1 1 1 1 1";

                default:
                    return string.Empty;
            }
        }

        private void CheckGraphicsForTextRenderer()
        {
            if (graphicsForTextRenderer == null)
            {
                if (StiOptions.Engine.FullTrust && StiOptions.Export.Pdf.AllowInvokeWindowsLibraries)
                {
                    graphicsForTextRenderer = Graphics.FromHwnd(IntPtr.Zero);
                }
                else
                {
                    imageForGraphicsForTextRenderer = new Bitmap(1, 1);
                    graphicsForTextRenderer = Graphics.FromImage(imageForGraphicsForTextRenderer);
                }
            }
        }

        private MemoryStream WriteContentTypes()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };
            writer.WriteStartDocument();
            writer.WriteStartElement("Types");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/content-types");

            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "jpg");
            writer.WriteAttributeString("ContentType", "image/jpeg");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "png");
            writer.WriteAttributeString("ContentType", "image/png");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "rels");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-package.relationships+xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "xml");
            writer.WriteAttributeString("ContentType", "application/xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "fdseq");
            writer.WriteAttributeString("ContentType", "application/vnd.ms-package.xps-fixeddocumentsequence+xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "fpage");
            writer.WriteAttributeString("ContentType", "application/vnd.ms-package.xps-fixedpage+xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "ttf");
            writer.WriteAttributeString("ContentType", "application/vnd.ms-opentype");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "fdoc");
            writer.WriteAttributeString("ContentType", "application/vnd.ms-package.xps-fixeddocument+xml");
            writer.WriteEndElement();

            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/docProps/core.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-package.core-properties+xml");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private MemoryStream WriteMainRels()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId1");
            writer.WriteAttributeString("Type", "http://schemas.microsoft.com/xps/2005/06/fixedrepresentation");
            writer.WriteAttributeString("Target", "FixedDocSeq.fdseq");
            writer.WriteEndElement();
            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId2");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties");
            writer.WriteAttributeString("Target", "docProps/core.xml");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private MemoryStream WriteDocPropsCore(StiReport report)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };
            writer.WriteStartDocument();
            writer.WriteStartElement("cp:coreProperties");
            writer.WriteAttributeString("xmlns:cp", "http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
            writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
            writer.WriteAttributeString("xmlns:dcterms", "http://purl.org/dc/terms/");
            writer.WriteAttributeString("xmlns:dcmitype", "http://purl.org/dc/dcmitype/");
            writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

            var dateTime = $"{DateTime.Now:yyyy-MM-ddTHH:mm:ssZ}";

            writer.WriteStartElement("dcterms:created");
            writer.WriteAttributeString("xsi:type", "dcterms:W3CDTF");
            writer.WriteString(dateTime);
            writer.WriteEndElement();
            writer.WriteStartElement("dcterms:modified");
            writer.WriteAttributeString("xsi:type", "dcterms:W3CDTF");
            writer.WriteString(dateTime);
            writer.WriteEndElement();

            if (!string.IsNullOrEmpty(report.ReportName))
                writer.WriteElementString("dc:title", report.ReportName);

            if (!string.IsNullOrEmpty(report.ReportAlias))
                writer.WriteElementString("dc:subject", report.ReportAlias);

            if (!string.IsNullOrEmpty(report.ReportDescription))
                writer.WriteElementString("dc:description", report.ReportDescription);

            writer.WriteElementString("dc:creator", StiExportUtils.GetReportVersion(report));

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private MemoryStream WriteFixedDocSeq()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.Unicode)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };
            writer.WriteStartElement("FixedDocumentSequence");
            writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/xps/2005/06");

            writer.WriteStartElement("DocumentReference");
            writer.WriteAttributeString("Source", "/Documents/1/FixedDoc.fdoc");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.Flush();
            return ms;
        }

        private MemoryStream WriteFixedDoc(StiPagesCollection pages)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.Unicode)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };
            writer.WriteStartElement("FixedDocument");
            writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/xps/2005/06");

            for (var index = 0; index < pages.Count; index++)
            {
                writer.WriteStartElement("PageContent");
                writer.WriteAttributeString("Source", $"Pages/{index + 1}.fpage");

                var bookmarks = new List<string>();
                foreach (DictionaryEntry de in pageBookmarks)
                {
                    if ((int) de.Value == index)
                        bookmarks.Add((string) de.Key);
                }

                if (bookmarks.Count > 0)
                {
                    writer.WriteStartElement("PageContent.LinkTargets");
                    foreach (var st in bookmarks)
                    {
                        writer.WriteStartElement("LinkTarget");
                        writer.WriteAttributeString("Name", st);
                        writer.WriteEndElement();
                    }
                    writer.WriteFullEndElement();
                }

                writer.WriteFullEndElement();
            }

            writer.WriteFullEndElement();
            writer.Flush();
            return ms;
        }

        private MemoryStream WritePage(StiPage page, StiReport report)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.Unicode)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };

            var pageHeight = hiToDpi * report.Unit.ConvertToHInches(page.PageHeight * page.SegmentPerHeight);
            var pageWidth = hiToDpi * report.Unit.ConvertToHInches(page.PageWidth * page.SegmentPerWidth);
            var mgLeft = hiToDpi * report.Unit.ConvertToHInches(page.Margins.Left);
            var mgRight = hiToDpi * report.Unit.ConvertToHInches(page.Margins.Right);
            var mgTop = hiToDpi * report.Unit.ConvertToHInches(page.Margins.Top);
            var mgBottom = hiToDpi * report.Unit.ConvertToHInches(page.Margins.Bottom);

            writer.WriteStartElement("FixedPage");
            writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/xps/2005/06");
            writer.WriteAttributeString("Width", $"{FloatToString(pageWidth)}");
            writer.WriteAttributeString("Height", $"{FloatToString(pageHeight)}");
            writer.WriteAttributeString("xml:lang", "en-EN");
            writer.WriteStartElement("Canvas");

            WriteHyperlinksData(writer, page, false);

            if (page.Brush != null)
            {
                var pp = new StiXpsData
                {
                    X = 0,
                    Y = 0,
                    Width = pageWidth,
                    Height = pageHeight,
                    Component = new StiContainer()
                };
                (pp.Component as StiContainer).Brush = page.Brush;
                (pp.Component as StiContainer).Border = null;
                WriteBorderFill(writer, pp);    //fullpage fill
            }

            WriteWatermark(writer, page.Watermark, true, pageWidth, pageHeight);

            #region  Process ExceedMargins
            foreach (StiComponent component in page.Components)
            {
                var txt = component as StiText;
                bool needPaint = component.Enabled && (component.Width > 0) && (component.Height > 0);
                if (needPaint && (txt != null) && (txt.ExceedMargins != StiExceedMargins.None) && !(txt.Brush is StiEmptyBrush))
                {
                    var x1 = hiToDpi * report.Unit.ConvertToHInches(component.Left);
                    var y1 = hiToDpi * report.Unit.ConvertToHInches(component.Top);
                    var x2 = hiToDpi * report.Unit.ConvertToHInches(component.Right);
                    var y2 = hiToDpi * report.Unit.ConvertToHInches(component.Bottom);

                    var pp = new StiXpsData
                    {
                        X = x1 + mgLeft,
                        Y = y1 + mgTop,
                        Width = x2 - x1,
                        Height = y2 - y1,
                        Component = component
                    };

                    if ((txt.ExceedMargins & StiExceedMargins.Left) > 0)
                    {
                        pp.Width = pp.Right;
                        pp.X = 0;
                    }
                    if ((txt.ExceedMargins & StiExceedMargins.Right) > 0)
                    {
                        pp.Width = pageWidth - pp.X;
                    }
                    if ((txt.ExceedMargins & StiExceedMargins.Top) > 0)
                    {
                        pp.Height = pp.Bottom;
                        pp.Y = 0;
                    }
                    if ((txt.ExceedMargins & StiExceedMargins.Bottom) > 0)
                    {
                        pp.Height = pageHeight - pp.Y;
                    }

                    WriteBorderFill(writer, pp);
                }
            }
            #endregion

            foreach (StiComponent component in page.Components)
            {
                bool needPaint = (component.Width > 0) && (component.Height > 0);
                if (component.Enabled && needPaint)
                {
                    var x1 = hiToDpi * report.Unit.ConvertToHInches(component.Left);
                    var y1 = hiToDpi * report.Unit.ConvertToHInches(component.Top);
                    var x2 = hiToDpi * report.Unit.ConvertToHInches(component.Right);
                    var y2 = hiToDpi * report.Unit.ConvertToHInches(component.Bottom);

                    var pp = new StiXpsData
                    {
                        X = x1,
                        Y = y1,
                        Width = x2 - x1,
                        Height = y2 - y1
                    };

                    pp.Y += mgTop;
                    pp.X += mgLeft;
                    pp.Component = component;

                    if (!(component is StiShape))
                        WriteBorderFill(writer, pp);

                    if (component is StiText && !component.IsExportAsImage(StiExportFormat.Xps))
                        WriteText(writer, pp);

                    if (component.IsExportAsImage(StiExportFormat.Xps))
                        WriteImage(writer, pp);
                    
                    WriteBorderStroke(writer, pp);
                }
            }

            if (page.Border != null)
            {
                var pp = new StiXpsData
                {
                    X = mgLeft,
                    Y = mgTop,
                    Width = pageWidth - mgLeft - mgRight,
                    Height = pageHeight - mgTop - mgBottom,
                    Component = new StiContainer()
                };
                (pp.Component as StiContainer).Border = page.Border;
                WriteBorderStroke(writer, pp);
            }

            WriteWatermark(writer, page.Watermark, false, pageWidth, pageHeight);

            #region Trial
#if CLOUD
            var isTrial = StiCloudPlan.IsTrial(report.ReportGuid);
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
            {
                var tempX = pageWidth / 794d * 1.75d;
                var tempY = pageHeight / 1123d * 1.75d;
                if (tempX > tempY) tempX = tempY; else tempY = tempX;

                writer.WriteStartElement("Canvas");
                writer.WriteAttributeString("RenderTransform", string.Format("{0},0,0,{1},{2},{3}",
                    FloatToString(tempX),
                    FloatToString(tempY),
                    FloatToString(pageWidth / 2),
                    FloatToString(pageHeight / 2)));
                writer.WriteStartElement("Canvas");
                writer.WriteAttributeString("RenderTransform", "0.707,-0.707,0.707,0.707,0,0");

                writer.WriteStartElement("Path");
                writer.WriteAttributeString("Stroke", $"#{0x40646464:X8}");
                writer.WriteAttributeString("StrokeThickness", $"{FloatToString(15)}");
                writer.WriteAttributeString("StrokeStartLineCap", "Round");
                writer.WriteAttributeString("StrokeEndLineCap", "Round");
                writer.WriteAttributeString("StrokeLineJoin", "Round");

                var path = new StringBuilder();
                path.Append("M 40,0 L 40,100 M 0,100 L 80,100 ");
                path.Append("M 100,0 L 100,70 M 100,45 L 120,65 L 130,72 L 140,68 ");
                path.Append("M 170,0 L 170,70 M 169,100 L 171,100 ");
                path.Append("M 215,60 L 222,69 L 232,71 L 255,70 L 265,60 L 265,5 L 270,0 M 265,44 L 220,31 L 212,20 L 212,10 L 225,0 L 235,0 L 250,5 L 265,18 ");
                path.Append("M 310,0 L 310,100 ");
                writer.WriteAttributeString("Data", path.ToString());
                writer.WriteAttributeString("RenderTransform", "1,0,0,-1,-155,50");
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
            }
            #endregion

            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.Flush();
            return ms;
        }

        private void WriteBorderFill(XmlTextWriter writer, StiXpsData data)
        {
            //fill band
            var tempColor = Color.Transparent;
            var mBrush = data.Component as IStiBrush;
            if (mBrush != null && mBrush.Brush != null)
                tempColor = StiBrush.ToColor(mBrush.Brush);

            var mRich = data.Component as StiRichText;
            if (mRich != null)
                tempColor = mRich.BackColor;

            if (tempColor.A != 0)
            {
                writer.WriteStartElement("Path");
                writer.WriteAttributeString("Data", string.Format("M {0},{1} L {0},{3} L {2},{3} L {2},{1} Z ",
                    FloatToString(data.X),		//x0
                    FloatToString(data.Y),		//y0	
                    FloatToString(data.X + data.Width),		//x1
                    FloatToString(data.Y + data.Height)));	//y1

                if (!(data.Component is StiText || data.Component is StiImage))
                    WriteHyperlinksData(writer, data.Component, false);

                writer.WriteStartElement("Path.Fill");

                if ((mBrush != null) && (mBrush.Brush != null) && (mBrush.Brush is StiGradientBrush))
                {
                    #region GradientColor
                    var brush = mBrush.Brush as StiGradientBrush;

                    #region calculate coordinates
                    double xs = 1;
                    double ys = 1;
                    var angle = brush.Angle;
                    if (angle < 0) angle += 360;
                    if ((angle >= 270) && (angle < 360))
                    {
                        angle = 360 - angle;
                        ys = -1;
                    };
                    if ((angle >= 180) && (angle < 270))
                    {
                        angle = angle - 180;
                        ys = -1;
                        xs = -1;
                    };
                    if ((angle >= 90) && (angle < 180))
                    {
                        angle = 180 - angle;
                        xs = -1;
                    };
                    angle = angle * Math.PI / 180f;

                    var x0 = data.X + data.Width / 2f;
                    var y0 = data.Y + data.Height / 2f;
                    var r = Math.Sqrt(data.Width * data.Width + data.Height * data.Height) / 2f;
                    var a2 = Math.Atan2(data.Height, data.Width);
                    var st = Math.PI / 2f - angle + a2;
                    var b = r * Math.Sin(st);
                    var xr = b * Math.Cos(angle) * xs;
                    var yr = b * Math.Sin(angle) * ys;

                    var x1 = x0 - xr;
                    var x2 = x0 + xr;
                    var y1 = y0 - yr;
                    var y2 = y0 + yr;
                    #endregion

                    writer.WriteStartElement("LinearGradientBrush");
                    writer.WriteAttributeString("MappingMode", "Absolute");
                    writer.WriteAttributeString("StartPoint", $"{FloatToString(x1)},{FloatToString(y1)}");
                    writer.WriteAttributeString("EndPoint", $"{FloatToString(x2)},{FloatToString(y2)}");
                    writer.WriteStartElement("LinearGradientBrush.GradientStops");
                    writer.WriteStartElement("GradientStop");
                    writer.WriteAttributeString("Color", $"#{brush.StartColor.ToArgb():X8}");
                    writer.WriteAttributeString("Offset", "0");
                    writer.WriteEndElement();
                    writer.WriteStartElement("GradientStop");
                    writer.WriteAttributeString("Color", $"#{brush.EndColor.ToArgb():X8}");
                    writer.WriteAttributeString("Offset", "1");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    #endregion
                }
                else
                {
                    #region SolidColor
                    writer.WriteStartElement("SolidColorBrush");
                    writer.WriteAttributeString("Color", $"#{tempColor.ToArgb():X8}");
                    writer.WriteEndElement();
                    #endregion
                }

                writer.WriteEndElement();
                writer.WriteFullEndElement();
            }
            else
            {
                if (WriteHyperlinksData(writer, data.Component, true) && !(data.Component is StiText || data.Component is StiImage))
                {
                    writer.WriteStartElement("Path");
                    writer.WriteAttributeString("Data", string.Format("M {0},{1} L {0},{3} L {2},{3} L {2},{1} Z ",
                        FloatToString(data.X),		//x0
                        FloatToString(data.Y),		//y0	
                        FloatToString(data.X + data.Width),		//x1
                        FloatToString(data.Y + data.Height)));	//y1

                    WriteHyperlinksData(writer, data.Component, false);

                    writer.WriteEndElement();
                }
            }
        }

        private void WriteBorderStroke(XmlTextWriter writer, StiXpsData data)
        {
            var mBorder = data.Component as IStiBorder;
            if (mBorder == null) return;

            #region draw shadow
            if (mBorder.Border.DropShadow && mBorder.Border.ShadowBrush != null)
            {
                var tempColor = StiBrush.ToColor(mBorder.Border.ShadowBrush);
                if (tempColor.A != 0)
                {
                    var shadowSize = mBorder.Border.ShadowSize;
                    writer.WriteStartElement("Path");
                    writer.WriteAttributeString("Data", string.Format("M {0},{4} L {1},{4} L {1},{3} L {2},{3} L {2},{5} L {0},{5} Z ",
                        FloatToString(data.X + shadowSize),                 //0 - x0
                        FloatToString(data.X + data.Width),                 //1 - x1	
                        FloatToString(data.X + data.Width + shadowSize),    //2 - x2
                        FloatToString(data.Y + shadowSize),                 //3 - y0	
                        FloatToString(data.Y + data.Height),                //4 - y1
                        FloatToString(data.Y + data.Height + shadowSize))); //5 - y2
                    writer.WriteStartElement("Path.Fill");
                    writer.WriteStartElement("SolidColorBrush");
                    writer.WriteAttributeString("Color", $"#{tempColor.ToArgb():X8}");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteFullEndElement();
                }
            }
            #endregion

            var advBorder = mBorder.Border as StiAdvancedBorder;
            var useAdvBorder = advBorder != null;

            var primitive = new StiRectanglePrimitive();  //by default all sides are enabled
            if (data.Component is StiRectanglePrimitive)
                primitive = data.Component as StiRectanglePrimitive;

            var needBorderLeft = mBorder.Border.IsLeftBorderSidePresent && primitive.LeftSide;
            var needBorderRight = mBorder.Border.IsRightBorderSidePresent && primitive.RightSide;
            var needBorderTop = mBorder.Border.IsTopBorderSidePresent && primitive.TopSide;
            var needBorderBottom = mBorder.Border.IsBottomBorderSidePresent && primitive.BottomSide;
            var needDraw = (mBorder.Border.Side != StiBorderSides.None) && (mBorder.Border.Style != StiPenStyle.None);

            if (useAdvBorder)
                needDraw = advBorder.Side != StiBorderSides.None;

            const float doubleStyleOffset = 0.9f;

            var border = new StiBorderSide(mBorder.Border.Color, mBorder.Border.Size, mBorder.Border.Style);
            double offset = 0;

            if (border.Style == StiPenStyle.Double)
                offset = doubleStyleOffset;

            if (needDraw)
            {
                #region draw border
                var path = new StringBuilder();
                if (needBorderLeft)
                {
                    if (useAdvBorder)
                    {
                        border = advBorder.LeftSide;
                        offset = (border.Style == StiPenStyle.Double ? doubleStyleOffset : 0);
                    }

                    var sidePath = string.Format("M {0},{1} L {2},{3} ",
                        FloatToString(data.X - offset),
                        FloatToString(data.Y - offset),
                        FloatToString(data.X - offset),
                        FloatToString(data.Y + offset + data.Height));

                    if (border.Style == StiPenStyle.Double)
                    {
                        sidePath += string.Format("M {0},{1} L {2},{3} ",
                            FloatToString(data.X + offset),
                            FloatToString(data.Y + (needBorderTop ? offset : -offset)),
                            FloatToString(data.X + offset),
                            FloatToString(data.Y + (needBorderBottom ? -offset : offset) + data.Height));
                    }

                    if (useAdvBorder)
                        WriteBorderSide(writer, border, sidePath);

                    else
                        path.Append(sidePath);
                }

                if (needBorderRight)
                {
                    if (useAdvBorder)
                    {
                        border = advBorder.RightSide;
                        offset = (border.Style == StiPenStyle.Double ? doubleStyleOffset : 0);
                    }

                    var sidePath = string.Format("M {0},{1} L {2},{3} ",
                        FloatToString(data.X + offset + data.Width),
                        FloatToString(data.Y - offset),
                        FloatToString(data.X + offset + data.Width),
                        FloatToString(data.Y + offset + data.Height));

                    if (border.Style == StiPenStyle.Double)
                    {
                        sidePath += string.Format("M {0},{1} L {2},{3} ",
                            FloatToString(data.X - offset + data.Width),
                            FloatToString(data.Y + (needBorderTop ? offset : -offset)),
                            FloatToString(data.X - offset + data.Width),
                            FloatToString(data.Y + (needBorderBottom ? -offset : offset) + data.Height));
                    }

                    if (useAdvBorder)
                        WriteBorderSide(writer, border, sidePath);

                    else
                        path.Append(sidePath);
                }

                if (needBorderTop)
                {
                    if (useAdvBorder)
                    {
                        border = advBorder.TopSide;
                        offset = (border.Style == StiPenStyle.Double ? doubleStyleOffset : 0);
                    }

                    var sidePath = string.Format("M {0},{1} L {2},{3} ",
                        FloatToString(data.X - offset),
                        FloatToString(data.Y - offset),
                        FloatToString(data.X + offset + data.Width),
                        FloatToString(data.Y - offset));

                    if (border.Style == StiPenStyle.Double)
                    {
                        sidePath += string.Format("M {0},{1} L {2},{3} ",
                            FloatToString(data.X + (needBorderLeft ? offset : -offset)),
                            FloatToString(data.Y + offset),
                            FloatToString(data.X + (needBorderRight ? -offset : offset) + data.Width),
                            FloatToString(data.Y + offset));
                    }

                    if (useAdvBorder)
                        WriteBorderSide(writer, border, sidePath);

                    else
                        path.Append(sidePath);
                }

                if (needBorderBottom)
                {
                    if (useAdvBorder)
                    {
                        border = advBorder.BottomSide;
                        offset = (border.Style == StiPenStyle.Double ? doubleStyleOffset : 0);
                    }

                    var sidePath = string.Format("M {0},{1} L {2},{3} ",
                        FloatToString(data.X - offset),
                        FloatToString(data.Y + offset + data.Height),
                        FloatToString(data.X + offset + data.Width),
                        FloatToString(data.Y + offset + data.Height));

                    if (border.Style == StiPenStyle.Double)
                    {
                        sidePath += string.Format("M {0},{1} L {2},{3} ",
                            FloatToString(data.X + (needBorderLeft ? offset : -offset)),
                            FloatToString(data.Y - offset + data.Height),
                            FloatToString(data.X + (needBorderRight ? -offset : offset) + data.Width),
                            FloatToString(data.Y - offset + data.Height));
                    }

                    if (useAdvBorder)
                        WriteBorderSide(writer, border, sidePath);

                    else
                        path.Append(sidePath);
                }

                if (!useAdvBorder)
                    WriteBorderSide(writer, new StiBorderSide(mBorder.Border.Color, mBorder.Border.Size, mBorder.Border.Style), path.ToString());
                #endregion
            }
        }

        private void WriteBorderSide(XmlTextWriter writer, StiBorderSide border, string path)
        {
            var borderSizeHi = border.Size;
            if (border.Style == StiPenStyle.Double)
                borderSizeHi = 1;

            var borderSize = borderSizeHi * 0.9;
            var tempColor = border.Color;
            var dashArray = GetLineStyleDash(border.Style);

            writer.WriteStartElement("Path");
            writer.WriteAttributeString("Stroke", $"#{tempColor.ToArgb():X8}");
            writer.WriteAttributeString("StrokeThickness", $"{borderSize}");
            writer.WriteAttributeString("StrokeStartLineCap", "Square");
            writer.WriteAttributeString("StrokeEndLineCap", "Square");

            if (dashArray != string.Empty)
            {
                writer.WriteAttributeString("StrokeDashArray", dashArray);
                writer.WriteAttributeString("StrokeDashCap", "Flat");
            }

            writer.WriteAttributeString("Data", path);
            writer.WriteFullEndElement();
        }

        private bool WriteHyperlinksData(XmlTextWriter writer, StiComponent component, bool check)
        {
            var flag = false;
            if (component.BookmarkValue != null && (string)component.BookmarkValue != "")
            {
                var bookmark = StiNameValidator.CorrectName(component.BookmarkValue.ToString(), report);
                if (!check)
                {
                    if (!pageBookmarks.ContainsKey(bookmark))
                    {
                        writer.WriteAttributeString("Name", bookmark);
                        pageBookmarks[bookmark] = indexPage;
                    }
                }
                flag = true;
            }
            if (component.HyperlinkValue != null && component.HyperlinkValue.ToString().Trim().Length > 0 && !component.HyperlinkValue.ToString().Trim().StartsWith("javascript:"))
            {
                var hyperlink = component.HyperlinkValue.ToString().Trim();
                if (!check)
                {
                    if (hyperlink.StartsWith("#"))
                    {
                        hyperlink = StiNameValidator.CorrectName(hyperlink.Substring(1), report);
                        writer.WriteAttributeString("FixedPage.NavigateUri", $"../FixedDoc.fdoc#{hyperlink}");
                    }
                    else
                        writer.WriteAttributeString("FixedPage.NavigateUri", hyperlink);
                }
                flag = true;
            }
            return flag;
        }

        private void WriteText(XmlTextWriter writer, StiXpsData pp)
        {
            var text = pp.Component as IStiText;
            var stiText = pp.Component as StiText;
            var mFont = pp.Component as IStiFont;
            var mTextBrush = pp.Component as IStiTextBrush;
            var textOpt = pp.Component as IStiTextOptions;
            var mTextHorAlign = pp.Component as IStiTextHorAlignment;
            var mVertAlign = pp.Component as IStiVertAlignment;

            var wordWrap = textOpt != null && textOpt.TextOptions.WordWrap;
            var needWidthAlign = mTextHorAlign != null && mTextHorAlign.HorAlignment == StiTextHorAlignment.Width;
            var useRightToLeft = textOpt != null && textOpt.TextOptions != null && textOpt.TextOptions.RightToLeft;
            var linesCount = 0;
            float textAngle = 0;

            var fontNumber = 0;
            double fontSize = 0;
            if (mFont != null)
            {
                fontNumber = pdfFont.GetFontNumber(mFont.Font);
                fontSize = mFont.Font.SizeInPoints * fontCorrectValue;
                pageFonts[fontNumber] = fontNumber;
            }

            if (text != null)
            {
                var tempColor = Color.Transparent;
                if (mTextBrush != null)
                    tempColor = StiBrush.ToColor(mTextBrush.TextBrush);

                #region calculate font parameters
                double sizeInPt = 1;
                if (mFont != null)
                    sizeInPt = mFont.Font.SizeInPoints;

                double cf = StiDpiHelper.GraphicsScale;

                var fonttmASC = sizeInPt * fontCorrectValue * pdfFont.tmASC / 1000 * cf;
                var fonttmDESC = sizeInPt * fontCorrectValue * pdfFont.tmDESC / 1000 * (-1) * cf;
                var fonttmExternal = sizeInPt * fontCorrectValue * pdfFont.tmExternal / 1000 * cf;
                var fontUnderscoreSize = sizeInPt * fontCorrectValue * pdfFont.UnderscoreSize / 1000 * cf;
                var fontUnderscorePosition = sizeInPt * fontCorrectValue * pdfFont.UnderscorePosition / 1000 * cf;
                var fontStrikeoutSize = sizeInPt * fontCorrectValue * pdfFont.StrikeoutSize / 1000 * cf;
                var fontStrikeoutPosition = sizeInPt * fontCorrectValue * pdfFont.StrikeoutPosition / 1000 * cf;

                var fontLineHeight = (fonttmASC - fonttmDESC + fonttmExternal) * stiText.LineSpacing;
                var fontAscF = fonttmASC;
                var fontDescF = fonttmDESC;
                #endregion

                #region calculate text coordinates
                var marginL = stiText.Margins.Left;
                var marginR = stiText.Margins.Right;
                var marginT = stiText.Margins.Top;
                var marginB = stiText.Margins.Bottom;
                var textX = pp.X + marginL;
                var textY = pp.Y + marginB;
                var textW = pp.Width - marginL - marginR;
                var textH = pp.Height - marginT - marginB;

                //correction of coordinates for border; temporarly
                var correctX = 2f;
                var correctY = 1f;
                textX += correctX;
                textW -= correctX * 2f;
                textY += correctY;
                textH -= correctY * 2f;
                #endregion

                var sb = new StringBuilder(text.Text);
                sb.Replace("\r", "");

                if (textW > 0 && sb.Length > 0)
                {
                    writer.WriteStartElement("Canvas");
                    writer.WriteAttributeString("Clip", string.Format("M {0},{1} L {0},{3} L {2},{3} L {2},{1} Z ",
                        FloatToString(pp.X),        //x0
                        FloatToString(pp.Y),        //y0	
                        FloatToString(pp.X + pp.Width),     //x1
                        FloatToString(pp.Y + pp.Height)));	//y1

                    WriteHyperlinksData(writer, pp.Component, false);

                    List<string> stringList = null;
                    if (needWidthAlign)
                    {
                        RectangleD rect = stiText.Page.Unit.ConvertToHInches(pp.Component.ComponentToPage(pp.Component.ClientRectangle));
                        rect = stiText.ConvertTextMargins(rect, false);
                        rect = stiText.ConvertTextBorders(rect, false);
                        rect.Width /= StiDpiHelper.DeviceCapsScale;
                        rect.Height /= StiDpiHelper.DeviceCapsScale;
                        CheckGraphicsForTextRenderer();
                        stringList = StiTextDrawing.SplitTextWordwrapWidth(sb.ToString(), graphicsForTextRenderer, stiText.Font, rect);
                        wordWrap = false;
                    }
                    else
                    {
                        stringList = StiExportUtils.SplitString(sb.ToString(), true);
                    }
                    linesCount = stringList.Count;

                    #region calculate rotating angle
                    var normTextX = textX;
                    var normTextY = textY;
                    var normTextW = textW;
                    var normTextH = textH;

                    if (textOpt != null)
                    {
                        textAngle = textOpt.TextOptions.Angle;
                        if (textAngle != 0)
                        {
                            if ((textAngle > 45 && textAngle < 135) || (textAngle > 225 && textAngle < 315))
                            {
                                var tempValue = textW;
                                textW = textH;
                                textH = tempValue;
                            }
                            textX = -textW / 2f;
                            textY = -textH / 2f;
                        }
                    }
                    #endregion

                    writer.WriteStartElement("Canvas");
                    if (textAngle != 0)
                    {
                        var AngleInRadians = -textAngle * Math.PI / 180f;
                        writer.WriteAttributeString("RenderTransform", string.Format("{0},{1},{2},{3},{4},{5}",
                            FloatToString(Math.Cos(AngleInRadians)),
                            FloatToString(Math.Sin(AngleInRadians)),
                            FloatToString(-Math.Sin(AngleInRadians)),
                            FloatToString(Math.Cos(AngleInRadians)),
                            FloatToString(normTextX + normTextW / 2),
                            FloatToString(normTextY + normTextH / 2)));
                    }

                    #region check wordwrap
                    if (wordWrap)
                    {
                        for (var indexLine = 0; indexLine < stringList.Count; indexLine++)
                        {
                            var stt = stringList[indexLine];
                            var sbt = new StringBuilder();
                            var indexChar = 0;
                            while (indexChar < stt.Length)
                            {
                                if (char.IsWhiteSpace(stt[indexChar]))
                                {
                                    sbt.Append(stt[indexChar]);
                                    indexChar++;
                                }
                                else
                                {
                                    var sbtWord = new StringBuilder();
                                    while ((indexChar < stt.Length) && (!char.IsWhiteSpace(stt[indexChar])))
                                    {
                                        sbtWord.Append(stt[indexChar]);
                                        indexChar++;
                                    }
                                    //var sbtWordBidi = bidi.Convert(sbtWord, false);   //need add parameter - bool reverseString
                                    var sbtWordBidi = StiBidirectionalConvert2.ConvertStringBuilder(sbtWord, false);
                                    sbt.Append(sbtWordBidi);
                                    var lenWord = sbtWord.Length - sbtWordBidi.Length;

                                    if (lenWord > 0)
                                        sbt.Append((char) 0x00, lenWord);
                                }
                            }

                            var charr = new int[stt.Length];
                            for (var tempIndex = 0; tempIndex < stt.Length; tempIndex++)
                            {
                                charr[tempIndex] = pdfFont.UnicodeMap[(int)sbt[tempIndex]];
                            }

                            var summarr = new int[stt.Length];
                            var wordarr = new int[stt.Length];
                            var wordCounter = 0;
                            float summ = 0;
                            for (var index = 0; index < charr.Length; index++)
                            {
                                if (charr[index] >= 32)
                                    summ += pdfFont.Widths[charr[index] - 32];

                                if (charr[index] == '\t')
                                    summ += GetTabsSize(textOpt, sizeInPt, summ);

                                summarr[index] = (int)summ;
                                
                                if (IsWordWrapSymbol(sbt, index) && index > 0)
                                    wordCounter++;

                                wordarr[index] = wordCounter;
                            }

                            var summf = summ * (sizeInPt * fontCorrectValue) / 1000;
                            //line is too long and have more than one word
                            //if ((summf > textW) && (wordarr[stt.Length-1]>0))
                            if (summf > textW)
                            {
                                var index = stt.Length - 1;
                                var textWint = (int)(textW * 1000 / (sizeInPt * fontCorrectValue));
                                while ((summarr[index] > textWint) && (index > 0)) index--;
                                //check in which word; if not first - find the beginnning of the word, otherwise - the end of the word
                                var index2 = index;
                                if (wordarr[index] > 0)
                                {
                                    if (wordarr[index] != wordarr[index + 1])
                                    {
                                        index2 = index++;
                                        while (char.IsWhiteSpace(sbt[index])) index++;  //TrimEnd => no check index++
                                    }
                                    else
                                    {
                                        while (!IsWordWrapSymbol(sbt, index)) index--;
                                        index2 = index - 1;
                                        while ((char.IsWhiteSpace(sbt[index2])) && (index2 > 0)) index2--;
                                        while (char.IsWhiteSpace(sbt[index])) index++;
                                    }
                                }
                                else
                                {
                                    index++;
                                }

                                //this block must be optimized - on long text may be very slow (many pass on long line)
                                if (needWidthAlign)
                                    stringList[indexLine] = stt.Substring(0, index2 + 1) + '\a';

                                else
                                    stringList[indexLine] = stt.Substring(0, index2 + 1);

                                stringList.Insert(indexLine + 1, stt.Substring(index, stt.Length - index).TrimStart(' '));
                            }
                        }
                    }
                    #endregion

                    #region mapping Unicode symbols and BidiConvert
                    for (var indexLine = 0; indexLine < stringList.Count; indexLine++)
                    {
                        var stt = stringList[indexLine];
                        //var sbTemp = bidi.Convert(new StringBuilder(stt), useRightToLeft);
                        var sbTemp = StiBidirectionalConvert2.ConvertStringBuilder(new StringBuilder(stt), useRightToLeft);
                        stringList[indexLine] = sbTemp.ToString();
                    }
                    #endregion

                    #region VertAlign
                    if (mVertAlign != null)
                    {
                        var lineHeight = fontLineHeight;
                        var textHeight = stringList.Count * lineHeight;
                        var vertAlignment = mVertAlign.VertAlignment;

                        if (textAngle != 0 && textAngle != 90 && textAngle != 180 && textAngle != 270)
                            vertAlignment = StiVertAlignment.Center;

                        if (vertAlignment == StiVertAlignment.Top)
                            textY += fontAscF;

                        if (vertAlignment == StiVertAlignment.Center)
                            textY += (textH - textHeight) / 2 + fontAscF;

                        if (vertAlignment == StiVertAlignment.Bottom)
                            textY += textH - textHeight + fontAscF;
                    }
                    #endregion

                    #region write text
                    var underlineArray = new double[stringList.Count, 3];
                    for (var indexLine = 0; indexLine < stringList.Count; indexLine++)
                    {
                        var stt = stringList[indexLine];
                        if (stt.Length > 0)
                        {

                            #region calculate text line length in pt
                            var sttWidths = new double[stt.Length];
                            double summ = 0;

                            for (var index = 0; index < stt.Length; index++)
                            {
                                var charrSym = (int)stt[index];
                                if (charrSym >= 32)
                                    summ += pdfFont.Widths[pdfFont.UnicodeMap[charrSym] - 32];

                                if (charrSym == '\t')
                                {
                                    var tabSize = GetTabsSize(textOpt, sizeInPt, (float)summ);
                                    summ += tabSize;
                                    sttWidths[index] = tabSize / 10;
                                }
                            }
                            summ = summ * (sizeInPt * fontCorrectValue) / 1000;
                            #endregion

                            stt = ConvertToEscapeSequence(stt);
                            if (stt[0] == '{') //specific behavior of xml-parser
                                stt = "{}" + stt;

                            var textLineX = textX;
                            var textLineY = textY + fontLineHeight * indexLine;

                            #region HorAlign
                            if (mTextHorAlign != null)
                            {
                                var horAlign = mTextHorAlign.HorAlignment;

                                if (needWidthAlign)
                                {
                                    if (stt.Length > 0 && (stt[stt.Length - 1] == '\a'))
                                        stt = stt.Substring(0, stt.Length - 1);

                                    else
                                        horAlign = StiTextHorAlignment.Left;
                                }

                                if (textOpt != null && textOpt.TextOptions != null && textOpt.TextOptions.RightToLeft)
                                {
                                    if (horAlign == StiTextHorAlignment.Left)
                                        horAlign = StiTextHorAlignment.Right;

                                    else if (horAlign == StiTextHorAlignment.Right)
                                        horAlign = StiTextHorAlignment.Left;
                                }

                                // left justify - not need any offset
                                if (horAlign == StiTextHorAlignment.Center)
                                    textLineX += (textW - summ) / 2;

                                if (horAlign == StiTextHorAlignment.Right)
                                    textLineX += textW - summ;

                                if (horAlign == StiTextHorAlignment.Width)
                                {
                                    #region calculate width align
                                    var numSpaces = 0;
                                    for (var spaceIndex = 0; spaceIndex < stt.Length; spaceIndex++)
                                    {
                                        if (stt[spaceIndex] == ' ')
                                            numSpaces++;
                                    }

                                    if (numSpaces > 0 && (indexLine != stringList.Count - 1 || needWidthAlign))
                                    {
                                        var spaceOffset = ((textW - summ) / numSpaces * 100) / (sizeInPt * fontCorrectValue);
                                        if (spaceOffset > 0)
                                        {
                                            spaceOffset += pdfFont.Widths[0] / 10;
                                            for (var charIndex = 0; charIndex < stt.Length; charIndex++)
                                            {
                                                if (stt[charIndex] == ' ')
                                                {
                                                    sttWidths[charIndex] = spaceOffset;
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion

                            underlineArray[indexLine, 0] = textLineX;
                            underlineArray[indexLine, 1] = textLineY;
                            underlineArray[indexLine, 2] = summ;

                            var tempInfo = (PdfFonts.pfontInfo)pdfFont.fontList[fontNumber];

                            string fontName = tempInfo.Name;
                            if (StiOptions.Export.UseAlternativeFontNames && StiOptions.Export.AlternativeFontNames.ContainsKey(fontName))
                            {
                                fontName = (string)StiOptions.Export.AlternativeFontNames[fontName];
                            }

                            var tempSb = new StringBuilder(fontName);
                            tempSb.Replace(" ", "_");

                            if (tempInfo.Bold || tempInfo.Italic)
                            {
                                if (tempInfo.Bold)
                                    tempSb.Append("_Bold");

                                if (tempInfo.Italic)
                                    tempSb.Append("_Italic");
                            }

                            var useIndices = false;
                            var indices = new StringBuilder();
                            for (var index = 0; index < sttWidths.Length - 1; index++)
                            {
                                if (sttWidths[index] != 0)
                                {
                                    indices.Append(",");
                                    indices.Append(FloatToString(sttWidths[index]));
                                    useIndices = true;
                                }

                                if (index != sttWidths.Length - 1 - 1)
                                    indices.Append(";");
                            }

                            writer.WriteStartElement("Glyphs");
                            writer.WriteAttributeString("BidiLevel", "0");
                            writer.WriteAttributeString("Fill", string.Format("#{0:X8}", tempColor.ToArgb()));
                            writer.WriteAttributeString("FontUri", string.Format("/Resources/{0}.ttf", tempSb.ToString()));
                            writer.WriteAttributeString("FontRenderingEmSize", FloatToString(fontSize));
                            writer.WriteAttributeString("StyleSimulations", "None");
                            writer.WriteAttributeString("OriginX", FloatToString(textLineX));
                            writer.WriteAttributeString("OriginY", FloatToString(textLineY));
                            writer.WriteAttributeString("UnicodeString", stt);
                            writer.WriteAttributeString("Indices", (useIndices ? indices.ToString() : string.Empty));
                            writer.WriteFullEndElement();
                        }
                    }
                    #endregion

                    #region underline text
                    if (mFont != null && mFont.Font.Underline)
                    {
                        var underscoreSize = sizeInPt * 0.09;
                        var underscorePosition = -sizeInPt * 0.115;

                        if (pdfFont.UnderscoreSize != 0)
                        {
                            underscoreSize = fontUnderscoreSize;
                            underscorePosition = fontUnderscorePosition;
                        }

                        if (underscoreSize < 0.1f)
                            underscoreSize = 0.1f;

                        for (var tempIndex = 0; tempIndex < stringList.Count; tempIndex++)
                        {
                            if (underlineArray[tempIndex, 2] != 0)
                            {
                                writer.WriteStartElement("Path");
                                writer.WriteAttributeString("Stroke", string.Format("#{0:X8}", tempColor.ToArgb()));
                                writer.WriteAttributeString("StrokeThickness", string.Format("{0}", FloatToString(underscoreSize)));
                                writer.WriteAttributeString("Data", string.Format("M {0},{2} L {1},{2}",
                                    FloatToString(underlineArray[tempIndex, 0]),
                                    FloatToString(underlineArray[tempIndex, 0] + underlineArray[tempIndex, 2]),
                                    FloatToString(underlineArray[tempIndex, 1] - underscorePosition)));
                                writer.WriteFullEndElement();
                            }
                        }
                    }
                    #endregion

                    #region strikeout text
                    if (mFont != null && mFont.Font.Strikeout)
                    {
                        var strikeoutSize = sizeInPt * 0.09;
                        var strikeoutPosition = sizeInPt * 0.4;

                        if (pdfFont.StrikeoutSize != 0)
                        {
                            strikeoutSize = fontStrikeoutSize;
                            strikeoutPosition = fontStrikeoutPosition;
                        }

                        if (strikeoutSize < 0.1f)
                            strikeoutSize = 0.1f;

                        for (var tempIndex = 0; tempIndex < stringList.Count; tempIndex++)
                        {
                            if (underlineArray[tempIndex, 2] != 0)
                            {
                                writer.WriteStartElement("Path");
                                writer.WriteAttributeString("Stroke", string.Format("#{0:X8}", tempColor.ToArgb()));
                                writer.WriteAttributeString("StrokeThickness", string.Format("{0}", FloatToString(strikeoutSize)));
                                writer.WriteAttributeString("Data", string.Format("M {0},{2} L {1},{2}",
                                    FloatToString(underlineArray[tempIndex, 0]),
                                    FloatToString(underlineArray[tempIndex, 0] + underlineArray[tempIndex, 2]),
                                    FloatToString(underlineArray[tempIndex, 1] - strikeoutPosition)));
                                writer.WriteFullEndElement();
                            }
                        }
                    }
                    #endregion

                    writer.WriteFullEndElement();   //canvas
                    writer.WriteFullEndElement();   //canvas
                }

                #region Lines of underline
                if (text.LinesOfUnderline != StiPenStyle.None)
                {
                    #region calculate coordinate
                    var coordTextY = pp.Y + marginB + correctY;
                    var needLineUp = true;
                    var needLineDown = true;
                    if (mVertAlign != null)
                    {
                        if (linesCount == 0) linesCount = 1;
                        var textHeight = fontLineHeight * linesCount;
                        var vertAlignment = mVertAlign.VertAlignment;

                        if (textAngle != 0 && textAngle != 90 && textAngle != 180 && textAngle != 270)
                        {
                            vertAlignment = StiVertAlignment.Center;
                        }

                        if (vertAlignment == StiVertAlignment.Top)
                        {
                            coordTextY += fontAscF;
                            needLineUp = false;
                        }

                        if (vertAlignment == StiVertAlignment.Center)
                        {
                            coordTextY += (textH - textHeight) / 2 + fontAscF;
                        }

                        if (vertAlignment == StiVertAlignment.Bottom)
                        {
                            coordTextY += textH - fontLineHeight + fontAscF;
                            needLineDown = false;
                        }
                    }
                    coordTextY += -fontDescF;
                    #endregion

                    var mBorder = pp.Component as IStiBorder;
                    if (mBorder != null)
                    {
                        #region style
                        var borderSizeHi = mBorder.Border.Size;
                        var borderSize = borderSizeHi * 0.9;
                        var tempColor2 = mBorder.Border.Color;
                        var dashArray = GetLineStyleDash(text.LinesOfUnderline);
                        #endregion

                        #region draw lines
                        var path = new StringBuilder();
                        var lineY = coordTextY;
                        if (needLineUp)
                        {
                            while (lineY - fontLineHeight > pp.Y + marginT)
                            {
                                lineY -= fontLineHeight;
                            }
                        }

                        while (lineY < pp.Y + pp.Height - marginB - (needLineDown ? 0 : fontLineHeight))
                        {
                            path.Append(string.Format("M {0},{1} L {2},{3} ",
                                FloatToString(pp.X),
                                FloatToString(lineY),
                                FloatToString(pp.X + pp.Width),
                                FloatToString(lineY)));
                            lineY += fontLineHeight;
                        }
                        writer.WriteStartElement("Path");
                        writer.WriteAttributeString("Stroke", $"#{tempColor2.ToArgb():X8}");
                        writer.WriteAttributeString("StrokeThickness", $"{borderSize}");
                        writer.WriteAttributeString("StrokeStartLineCap", "Square");
                        writer.WriteAttributeString("StrokeEndLineCap", "Square");

                        if (dashArray != string.Empty)
                        {
                            writer.WriteAttributeString("StrokeDashArray", dashArray);
                            writer.WriteAttributeString("StrokeDashCap", "Flat");
                        }

                        writer.WriteAttributeString("Data", path.ToString());
                        writer.WriteFullEndElement();
                        #endregion
                    }
                }
                #endregion

            }
        }

        private void WriteImage(XmlTextWriter writer, StiXpsData data)
        {
            var exportImage = data.Component as IStiExportImageExtended;
            if (exportImage == null || !data.Component.IsExportAsImage(StiExportFormat.Xps)) return;

            var rsImageResolution = imageResolution / 100f;
            using (var image = exportImage.GetImage(ref rsImageResolution, StiExportFormat.Xps))
            {
                if (image == null) return;

                if (image is Bitmap)
                    (image as Bitmap).SetResolution(96, 96);

                var imageIndex = imageCache.AddImageInt(image);
                var maskIndex = -1;

                if (StiOptions.Export.Xps.AllowImageTransparency && image is Bitmap)
                {
                    #region get mask index
                    var bitmap = image as Bitmap;

                    // Lock the bitmap's bits
                    var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    var stride = Math.Abs(bitmapData.Stride);

                    // Declare an array to hold the bytes of the bitmap.
                    var pixelsDataCount1 = stride * bitmapData.Height;
                    var pixelsData1 = new byte[pixelsDataCount1];

                    // Copy the bitmap pixel values into the array.
                    Marshal.Copy(bitmapData.Scan0, pixelsData1, 0, pixelsDataCount1);

                    // Unlock the bits.
                    bitmap.UnlockBits(bitmapData);

                    //check data for mask
                    var needMask = false;
                    for (var y = 0; y < bitmap.Height; y++)
                    {
                        var offs1 = y * stride;
                        var x = bitmap.Width;
                        while (x > 0)
                        {
                            pixelsData1[offs1++] = 0;
                            pixelsData1[offs1++] = 0;
                            pixelsData1[offs1++] = 0;
                            if (pixelsData1[offs1++] != 255)
                                needMask = true;

                            x--;
                        }
                    }

                    if (needMask)
                    {
                        bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                        Marshal.Copy(pixelsData1, 0, bitmapData.Scan0, pixelsDataCount1);
                        bitmap.UnlockBits(bitmapData);

                        maskIndex = imageCache.AddImageInt(image, ImageFormat.Png);
                    }
                    #endregion
                }

                pageImages[imageIndex] = imageIndex;
                if (maskIndex != -1) pageImages[maskIndex] = maskIndex;

                #region write image info
                writer.WriteStartElement("Path");

                WriteHyperlinksData(writer, data.Component, false);

                writer.WriteAttributeString("Data", string.Format("M {0},{1} L {0},{3} L {2},{3} L {2},{1} Z ",
                    FloatToString(data.X),      //x0
                    FloatToString(data.Y),      //y0	
                    FloatToString(data.X + data.Width),     //x1
                    FloatToString(data.Y + data.Height)));  //y1

                if (maskIndex != -1)
                {
                    writer.WriteStartElement("Path.OpacityMask");
                    writer.WriteStartElement("ImageBrush");
                    writer.WriteAttributeString("ImageSource", string.Format("/Resources/Images/image_{0}.png", maskIndex));
                    writer.WriteAttributeString("Viewbox", string.Format("{0},{1},{2},{3}",
                        0,
                        0,
                        image.Width,
                        image.Height));
                    writer.WriteAttributeString("TileMode", "None");
                    writer.WriteAttributeString("ViewboxUnits", "Absolute");
                    writer.WriteAttributeString("ViewportUnits", "Absolute");
                    //writer.WriteAttributeString("Viewport", string.Format("0,0,{0},{0}", rsImageResolution));
                    writer.WriteAttributeString("Viewport", string.Format("0,0,{0},{0}", 1));
                    writer.WriteStartElement("ImageBrush.Transform");
                    writer.WriteStartElement("MatrixTransform");
                    writer.WriteAttributeString("Matrix", string.Format("{0},{1},{2},{3},{4},{5}",
                        FloatToString(data.Width),
                        0,
                        0,
                        FloatToString(data.Height),
                        FloatToString(data.X),      //
                        FloatToString(data.Y)));    //
                    writer.WriteEndElement();   //MatrixTransform
                    writer.WriteEndElement();   //ImageBrush.Transform
                    writer.WriteEndElement();   //ImageBrush
                    writer.WriteEndElement();   //Path.OpacityMask
                }

                writer.WriteStartElement("Path.Fill");
                writer.WriteStartElement("ImageBrush");
                writer.WriteAttributeString("ImageSource", string.Format("/Resources/Images/image_{0}.jpg", imageIndex));
                writer.WriteAttributeString("Viewbox", string.Format("{0},{1},{2},{3}",
                    0,
                    0,
                    image.Width,
                    image.Height));
                writer.WriteAttributeString("TileMode", "None");
                writer.WriteAttributeString("ViewboxUnits", "Absolute");
                writer.WriteAttributeString("ViewportUnits", "Absolute");
                //writer.WriteAttributeString("Viewport", string.Format("0,0,{0},{0}", rsImageResolution));
                writer.WriteAttributeString("Viewport", string.Format("0,0,{0},{0}", 1));
                writer.WriteStartElement("ImageBrush.Transform");
                writer.WriteStartElement("MatrixTransform");
                writer.WriteAttributeString("Matrix", string.Format("{0},{1},{2},{3},{4},{5}",
                    FloatToString(data.Width),
                    0,
                    0,
                    FloatToString(data.Height),
                    FloatToString(data.X),      //
                    FloatToString(data.Y)));    //
                writer.WriteEndElement();   //MatrixTransform
                writer.WriteEndElement();   //ImageBrush.Transform
                writer.WriteEndElement();   //ImageBrush
                writer.WriteEndElement();   //Path.Fill

                writer.WriteFullEndElement();
                #endregion
            }
        }

        private void WriteWatermark(XmlTextWriter writer, StiWatermark watermark, bool behind, double pageWidth, double pageHeight)
        {
            if (watermark == null || string.IsNullOrEmpty(watermark.Text) || watermark.ShowBehind != behind) return;

            var pp = new StiXpsData
            {
                X = 0,
                Y = 0,
                Width = pageWidth,
                Height = pageHeight
            };

            pp.Component = new StiText(new RectangleD(pp.X, pp.Y, pp.Width, pp.Height))
            {
                Text = watermark.Text,
                TextBrush = watermark.TextBrush,
                Font = watermark.Font,
                TextOptions = new StiTextOptions
                {
                    Angle = watermark.Angle
                },
                HorAlignment = StiTextHorAlignment.Center,
                VertAlignment = StiVertAlignment.Center
            };
            WriteText(writer, pp);
        }

        private MemoryStream WritePageRels()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            var fonts = new int[pageFonts.Count];
            pageFonts.Values.CopyTo(fonts, 0);
            for (var indexFont = 0; indexFont < fonts.Length; indexFont++)
            {
                var tempInfo = (PdfFonts.pfontInfo)pdfFont.fontList[fonts[indexFont]];

                string fontName = tempInfo.Name;
                if (StiOptions.Export.UseAlternativeFontNames && StiOptions.Export.AlternativeFontNames.ContainsKey(fontName))
                {
                    fontName = (string)StiOptions.Export.AlternativeFontNames[fontName];
                }

                var tempSb = new StringBuilder(fontName);
                tempSb.Replace(" ", "_");

                if (tempInfo.Bold || tempInfo.Italic)
                {
                    if (tempInfo.Bold)
                        tempSb.Append("_Bold");

                    if (tempInfo.Italic)
                        tempSb.Append("_Italic");
                }

                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", $"rId{indexFont + 1}");
                writer.WriteAttributeString("Type", "http://schemas.microsoft.com/xps/2005/06/required-resource");
                writer.WriteAttributeString("Target", $"../../../Resources/{tempSb}.ttf");
                writer.WriteEndElement();
            }

            var images = new int[pageImages.Count];
            pageImages.Values.CopyTo(images, 0);

            for (var indexImage = 0; indexImage < images.Length; indexImage++)
            {
                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", $"rId{pageFonts.Count + indexImage + 1}");
                writer.WriteAttributeString("Type", "http://schemas.microsoft.com/xps/2005/06/required-resource");

                var fileExtension = "jpg";
                if (imageCache.ImageFormatStore[images[indexImage]] == ImageFormat.Png)
                    fileExtension = "png";

                writer.WriteAttributeString("Target", $"../../../Resources/Images/image_{images[indexImage]}.{fileExtension}");
                writer.WriteEndElement();
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private MemoryStream WriteResourceImage(int index)
        {
            var stream = new MemoryStream();

            var buf = (byte[])imageCache.ImagePackedStore[index];
            stream.Write(buf, 0, buf.Length);

            return stream;
        }

        private MemoryStream WriteRecourceFont(int index)
        {
            var ms = new MemoryStream();

            pdfFont.CurrentFont = index;
            var tempInfo = (PdfFonts.pfontInfo)pdfFont.fontList[index];
            byte[] buff;
            pdfFont.GetFontDataBuf(tempInfo.Font, out buff, true);

            if (buff.Length > 0 && reduceFontSize)
                pdfFont.OpenTypeHelper.ReduceFontSize(ref buff, tempInfo.Name, true, pdfFont.GlyphList, pdfFont.GlyphRtfList);

            ms.Write(buff, 0, buff.Length);
            return ms;
        }

        /// <summary>
        /// Exports rendered report to an Xps file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportXps(StiReport report, string fileName)
        {
            FileStream stream = null;

            try
            {
                StiFileUtils.ProcessReadOnly(fileName);
                stream = new FileStream(fileName, FileMode.Create);
                ExportXps(report, stream);
            }
            finally
            {
                stream.Flush();
                stream.Close();
            }
        }

        /// <summary>
        /// Exports rendered report to an Excel file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        public void ExportXps(StiReport report, Stream stream)
        {
            ExportXps(report, stream, new StiXpsExportSettings());
        }

        /// <summary>
        /// Exports rendered report to an Xps file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportXps(StiReport report, Stream stream, StiPagesRange pageRange)
        {
            ExportXps(report, stream, new StiXpsExportSettings
            {
                PageRange = pageRange
            });
        }

        public void ExportXps(StiReport report, Stream stream, StiXpsExportSettings settings)
        {
            StiLogService.Write(this.GetType(), "Export report to Microsoft XML Paper Specification format");

#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var pageRange = settings.PageRange;
            this.imageResolution = settings.ImageResolution;
            this.imageQuality = settings.ImageQuality;
            this.exportRtfTextAsImage = settings.ExportRtfTextAsImage;
            #endregion

            if (imageQuality < 0)
                imageQuality = 0;

            if (imageQuality > 1)
                imageQuality = 1;

            if (imageResolution < 10)
                imageResolution = 10;

            xmlIndentation = 1;

            this.useImageComparer = StiOptions.Export.Xps.AllowImageComparer;
            this.reduceFontSize = StiOptions.Export.Xps.ReduceFontFileSize;

            var currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";

                var pages = pageRange.GetSelectedPages(report.RenderedPages);
                if (IsStopped) return;
                StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");

                PrepareData();

                #region Scan for assemble data
                foreach (StiPage page in pages)
                {
                    pages.GetPage(page);
                    if (pages.CacheMode) InvokeExporting(page, pages, 0, 2);

                    if (page.Watermark != null)
                    {
                        if (!string.IsNullOrEmpty(page.Watermark.Text))
                        {
                            if (page.Watermark.Font != null)
                                pdfFont.GetFontNumber(page.Watermark.Font);

                            //var sb = new StringBuilder(page.Watermark.Text);
                            //sb = bidi.Convert(sb, false);
                            var sb = new StringBuilder(StiBidirectionalConvert2.ConvertString(page.Watermark.Text, false));
                            pdfFont.StoreUnicodeSymbolsInMap(sb);
                        }
                    }

                    foreach (StiComponent component in page.Components)
                    {
                        if (component.Enabled)
                        {
                            var mFont = component as IStiFont;
                            if (mFont != null)
                                pdfFont.GetFontNumber(mFont.Font);

                            //make map of the unicode symbols
                            var textOpt = component as IStiTextOptions;
                            if (component is StiText && (!component.IsExportAsImage(StiExportFormat.Xps)))
                            {
                                var text = component as IStiText;
                                var useRightToLeft = textOpt.TextOptions != null && textOpt.TextOptions.RightToLeft;
                                //var sb = new StringBuilder(text.Text);
                                //sb = bidi.Convert(sb, useRightToLeft);
                                var sb = new StringBuilder(StiBidirectionalConvert2.ConvertString(text.Text, useRightToLeft));
                                pdfFont.StoreUnicodeSymbolsInMap(sb);
                            }
                        }
                    }
                }
                pdfFont.InitFontsData(false);
                #endregion

                var zip = new StiZipWriter20();
                zip.Begin(stream, true);

                zip.AddFile("[Content_Types].xml", WriteContentTypes());
                zip.AddFile("_rels/.rels", WriteMainRels());
                zip.AddFile("docProps/core.xml", WriteDocPropsCore(report));
                zip.AddFile("FixedDocSeq.fdseq", WriteFixedDocSeq());

                pageFonts = new Hashtable();
                pageImages = new Hashtable();
                pageBookmarks = new Hashtable();
                indexPage = 0;
                foreach (StiPage page in pages)
                {
                    pages.GetPage(page);
                    InvokeExporting(page, pages, 1, 2);
                    if (IsStopped) return;

                    zip.AddFile($"Documents/1/Pages/{indexPage + 1}.fpage", WritePage(page, report));

                    if (pageFonts.Count > 0 || pageImages.Count > 0)
                        zip.AddFile($"Documents/1/Pages/_rels/{indexPage + 1}.fpage.rels", WritePageRels());

                    pageFonts.Clear();
                    pageImages.Clear();
                    if (IsStopped) return;
                    indexPage++;
                }

                zip.AddFile("Documents/1/FixedDoc.fdoc", WriteFixedDoc(pages));

                if (pdfFont.fontList.Count > 0)
                {
                    for (var index = 0; index < pdfFont.fontList.Count; index++)
                    {
                        var tempInfo = (PdfFonts.pfontInfo)pdfFont.fontList[index];

                        string fontName = tempInfo.Name;
                        if (StiOptions.Export.UseAlternativeFontNames && StiOptions.Export.AlternativeFontNames.ContainsKey(fontName))
                        {
                            fontName = (string)StiOptions.Export.AlternativeFontNames[fontName];
                        }

                        var tempSb = new StringBuilder(fontName);
                        tempSb.Replace(" ", "_");

                        if (tempInfo.Bold || tempInfo.Italic)
                        {
                            if (tempInfo.Bold)
                                tempSb.Append("_Bold");

                            if (tempInfo.Italic)
                                tempSb.Append("_Italic");
                        }
                        zip.AddFile($"Resources/{tempSb}.ttf", WriteRecourceFont(index));
                    }
                }

                if (imageCache.ImagePackedStore.Count > 0)
                {
                    for (var index = 0; index < imageCache.ImagePackedStore.Count; index++)
                    {
                        var fileExtension = "jpg";
                        if (imageCache.ImageFormatStore[index] == ImageFormat.Png)
                            fileExtension = "png";

                        zip.AddFile($"Resources/Images/image_{index}.{fileExtension}", WriteResourceImage(index));
                    }
                }

                zip.End();
            }
            finally
            {
                StiExportUtils.EnableFontSmoothing(report);
                Thread.CurrentThread.CurrentCulture = currentCulture;
                pdfFont.Clear();
                pdfFont = null;
                //bidi.Clear();
                //bidi = null;
                pageFonts = null;
                pageImages = null;
                imageCache.Clear();
                pageBookmarks.Clear();
                pageBookmarks = null;
            }
        }
        #endregion
    }
}
