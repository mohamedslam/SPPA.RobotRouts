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
using System.Collections;
using System.Globalization;
using System.Xml;
using System.Text;
using System.IO;
using System.Threading;
using Stimulsoft.Base;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Zip;
using Stimulsoft.Report.Components;
using System.Drawing;
using System.Drawing.Imaging;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the PowerPoint 2007 Export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourcePowerPoint.png")]
	public class StiPpt2007ExportService : StiExportService
    {
        #region StiExportService override
        /// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension => "pptx";

		public override StiExportFormat ExportFormat => StiExportFormat.Ppt2007;

		/// <summary>
		/// Gets a group of the export in the context menu.
		/// </summary>
		public override string GroupCategory => "Document";

		/// <summary>
		/// Gets a position of the export in the context menu.
		/// </summary>
		public override int Position => (int)StiExportPosition.Ppt2007;

		/// <summary>
		/// Gets an export name in the context menu.
		/// </summary>
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypePpt2007File");

	    /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportPowerPoint(report, stream, settings as StiPpt2007ExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }

		/// <summary>
		/// Exports a rendered report to the PowerPoint file.
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
                form["ExportFormat"] = StiExportFormat.Ppt2007;

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
		/// Returns a filter for the PowerPoint files.
		/// </summary>
        /// <returns>Returns a filter for the PowerPoint files.</returns>
		public override string GetFilter() => StiLocalization.Get("FileFilters", "Ppt2007Files");
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

                            var settings = new StiPpt2007ExportSettings
                            {
                                PageRange = form["PagesRange"] as StiPagesRange,
                                ImageResolution = (float)form["Resolution"],
                                ImageQuality = (float)form["ImageQuality"]
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
            else IsStopped = true;
        }
        #endregion

        #region Fields
        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        #endregion

        #region this
        private int imageListOffset = 0;
		private float imageResolution = 1;
		private float imageQuality = 0.75f;
		private StiImageCache imageCache = null;
        private int idCounter = 2;

        private ArrayList hyperlinkList = null;

		private int xmlIndentation = 1;

        private CultureInfo currentCulture = null;
        private CultureInfo newCulture = null;
        private string reportCulture = null;

        private string ReportCulture => reportCulture ?? "en-US";

        #region GetLineStyle
        private string GetLineStyle(StiPenStyle penStyle)
        {
            switch (penStyle)
            {
                case StiPenStyle.Solid:
                    return "solid";

                case StiPenStyle.Dot:
                    return "sysDot";

                case StiPenStyle.Dash:
                    return "sysDash";

                case StiPenStyle.DashDot:
                    return "sysDashDot";

                case StiPenStyle.DashDotDot:
                    return "sysDashDotDot";

                case StiPenStyle.Double:
                    return "solid";

                default:
                    return "solid";
            }
        }
        #endregion

        #region StringToUrl
        private string StringToUrl(string input)
        {
            var enc = new UTF8Encoding();
            var buf = enc.GetBytes(input);
            var output = new StringBuilder();
            foreach (byte byt in buf)
            {
                if ((byt < 0x20) || (byt > 0x7f) || (wrongUrlSymbols.IndexOf((char)byt) != -1))
                {
                    output.Append(string.Format("%{0:x2}", byt));
                }
                else
                {
                    output.Append((char)byt);
                }
            }
            return output.ToString();
        }
        //                                  space "   #   %   &   '   *   ,   :   ;   <   >   ?   [   ^   `   {   |   }   
        //private string wrongUrlSymbols = "\x20\x22\x23\x25\x26\x27\x2a\x2c\x3a\x3b\x3c\x3e\x3f\x5b\x5e\x60\x7b\x7c\x7d";
        private string wrongUrlSymbols = "\x20\x22\x27\x2a\x2c\x3b\x3c\x3e\x5b\x5e\x60\x7b\x7c\x7d";
        #endregion


        //conversion from hundredths of inch to twips
        private static double HiToTwips
        {
            get
            {
                //return 14.4 / 20 * 1.028;
                return 14.4 * 0.995;
            }
        }

        private int Convert(double x)
        {
            return (int)Math.Round((decimal)(x * HiToTwips));
        }

        // 1 inch = 914400 EMU
        private int ConvertTwipsToEmu(double x)
        {
            return (int)Math.Round((decimal)((x / HiToTwips) / 100 * 914400));
        }
        private int ConvertToEmu(double x)
        {
            return (int)Math.Round((decimal)(x / 100 * 914400));
        }


        #region WriteColor
        private void WriteColor(XmlWriter writer, Color color)
        {
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", string.Format("{0:X6}", color.ToArgb() & 0xFFFFFF));
            if (color.A != 0xFF)
            {
                int alphaValue = (int)(color.A / 256f * 100000);
                writer.WriteStartElement("a:alpha");
                writer.WriteAttributeString("val", string.Format("{0}", alphaValue));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        #endregion


        #region WriteContentTypes
        private MemoryStream WriteContentTypes(int pagesCount)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Types");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/content-types");

            for (int index = 0; index < 11; index++)
            {
                writer.WriteStartElement("Override");
                writer.WriteAttributeString("PartName", string.Format("/ppt/slideLayouts/slideLayout{0}.xml", index + 1));
                writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.presentationml.slideLayout+xml");
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/ppt/slideMasters/slideMaster1.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.presentationml.slideMaster+xml");
            writer.WriteEndElement();

            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/ppt/presProps.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.presentationml.presProps+xml");
            writer.WriteEndElement();

            for (int index = 0; index < pagesCount; index++)
            {
                writer.WriteStartElement("Override");
                writer.WriteAttributeString("PartName", string.Format("/ppt/slides/slide{0}.xml", index + 1));
                writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.presentationml.slide+xml");
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/ppt/theme/theme1.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.theme+xml");
            writer.WriteEndElement();

            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "jpeg");
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

            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/ppt/presentation.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.presentationml.presentation.main+xml");
            writer.WriteEndElement();

            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/docProps/app.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.extended-properties+xml");
            writer.WriteEndElement();

            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/ppt/tableStyles.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.presentationml.tableStyles+xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/ppt/viewProps.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.presentationml.viewProps+xml");
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
        #endregion

        #region WriteMainRels
        private MemoryStream WriteMainRels()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId1");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument");
            writer.WriteAttributeString("Target", "ppt/presentation.xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId2");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties");
            writer.WriteAttributeString("Target", "docProps/core.xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId3");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties");
            writer.WriteAttributeString("Target", "docProps/app.xml");
            writer.WriteEndElement();

            //<Relationship Id="rId4" Type="http://schemas.openxmlformats.org/package/2006/relationships/metadata/thumbnail" Target="docProps/thumbnail.jpeg"/>

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteDocPropsApp
        private MemoryStream WriteDocPropsApp(int pagesCount)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Properties");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties");
            writer.WriteAttributeString("xmlns:vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");

            writer.WriteElementString("Application", "Microsoft Office PowerPoint");
            writer.WriteElementString("Slides", string.Format("{0}", pagesCount));
            writer.WriteElementString("PresentationFormat", "On-screen Show (4:3)");
            writer.WriteElementString("TotalTime", "0");
            writer.WriteElementString("Words", "0");
            writer.WriteElementString("Paragraphs", "0");
            writer.WriteElementString("Notes", "0");
            writer.WriteElementString("HiddenSlides", "0");
            writer.WriteElementString("MMClips", "0");
            writer.WriteElementString("ScaleCrop", "false");

            writer.WriteStartElement("HeadingPairs");
            writer.WriteStartElement("vt:vector");
            writer.WriteAttributeString("size", "4");
            writer.WriteAttributeString("baseType", "variant");
            writer.WriteStartElement("vt:variant");
            writer.WriteElementString("vt:lpstr", "Theme");
            writer.WriteEndElement();
            writer.WriteStartElement("vt:variant");
            writer.WriteElementString("vt:i4", string.Format("{0}", pagesCount));
            writer.WriteEndElement();
            writer.WriteStartElement("vt:variant");
            writer.WriteElementString("vt:lpstr", "Slide Titles");
            writer.WriteEndElement();
            writer.WriteStartElement("vt:variant");
            writer.WriteElementString("vt:i4", string.Format("{0}", pagesCount));
            writer.WriteEndElement();	//vt:variant
            writer.WriteEndElement();	//vt:vector
            writer.WriteEndElement();	//HeadingPair

            writer.WriteStartElement("TitlesOfParts");
            writer.WriteStartElement("vt:vector");
            writer.WriteAttributeString("size", string.Format("{0}", 1 + pagesCount));
            writer.WriteAttributeString("baseType", "lpstr");
            writer.WriteElementString("vt:lpstr", "Office Theme");
            for (int index = 0; index < pagesCount; index++)
            {
                writer.WriteElementString("vt:lpstr", string.Format("Slide {0}", index + 1));
            }
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteElementString("Company", "Stimulsoft");
            writer.WriteElementString("LinksUpToDate", "false");
            writer.WriteElementString("SharedDoc", "false");
            writer.WriteElementString("HyperlinksChanged", "false");
            writer.WriteElementString("AppVersion", "12.0000");

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteDocPropsCore
        private MemoryStream WriteDocPropsCore()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("cp:coreProperties");
            writer.WriteAttributeString("xmlns:cp", "http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
            writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
            writer.WriteAttributeString("xmlns:dcterms", "http://purl.org/dc/terms/");
            writer.WriteAttributeString("xmlns:dcmitype", "http://purl.org/dc/dcmitype/");
            writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

            string dateTime = string.Format("{0}", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            writer.WriteElementString("dc:title", "");
            writer.WriteElementString("dc:subject", "");
            writer.WriteElementString("dc:creator", StiExportUtils.GetReportVersion(report));
            writer.WriteElementString("cp:keywords", "");
            writer.WriteElementString("dc:description", "");
            writer.WriteElementString("cp:lastModifiedBy", "Stimulsoft Reports");
            writer.WriteElementString("cp:revision", "1");
            writer.WriteStartElement("dcterms:created");
            writer.WriteAttributeString("xsi:type", "dcterms:W3CDTF");
            writer.WriteString(dateTime);
            writer.WriteEndElement();
            writer.WriteStartElement("dcterms:modified");
            writer.WriteAttributeString("xsi:type", "dcterms:W3CDTF");
            writer.WriteString(dateTime);
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteTableStyles
        private MemoryStream WriteTableStyles()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();

            writer.WriteStartElement("a:tblStyleLst");
            writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            writer.WriteAttributeString("def", "{5C22544A-7EE6-4342-B048-85BDC9FD1C3A}");
            writer.WriteEndElement();

            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WritePresProps
        private MemoryStream WritePresProps()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("p:presentationPr");
            writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:p", "http://schemas.openxmlformats.org/presentationml/2006/main");
            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteViewProps
        private MemoryStream WriteViewProps()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("p:viewPr");
            writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:p", "http://schemas.openxmlformats.org/presentationml/2006/main");

            writer.WriteStartElement("p:normalViewPr");
            writer.WriteAttributeString("showOutlineIcons", "0");
            writer.WriteStartElement("p:restoredLeft");
            writer.WriteAttributeString("sz", "15591");
            writer.WriteAttributeString("autoAdjust", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("p:restoredTop");
            writer.WriteAttributeString("sz", "94675");
            writer.WriteAttributeString("autoAdjust", "0");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("p:slideViewPr");
            writer.WriteStartElement("p:cSldViewPr");

            writer.WriteStartElement("p:cViewPr");
            writer.WriteAttributeString("varScale", "1");
            writer.WriteStartElement("p:scale");
            writer.WriteStartElement("a:sx");
            writer.WriteAttributeString("n", "107");
            writer.WriteAttributeString("d", "100");
            writer.WriteEndElement();
            writer.WriteStartElement("a:sy");
            writer.WriteAttributeString("n", "107");
            writer.WriteAttributeString("d", "100");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("p:origin");
            writer.WriteAttributeString("x", "-1098");
            writer.WriteAttributeString("y", "-84");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("p:guideLst");
            writer.WriteStartElement("p:guide");
            writer.WriteAttributeString("orient", "horz");
            writer.WriteAttributeString("pos", "2160");
            writer.WriteEndElement();
            writer.WriteStartElement("p:guide");
            writer.WriteAttributeString("pos", "2880");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndElement();

            //writer.WriteStartElement("p:outlineViewPr");
            //writer.WriteStartElement("p:cViewPr");
            //writer.WriteStartElement("p:scale");
            //writer.WriteStartElement("a:sx");
            //writer.WriteAttributeString("n", "33");
            //writer.WriteAttributeString("d", "100");
            //writer.WriteEndElement();
            //writer.WriteStartElement("a:sy");
            //writer.WriteAttributeString("n", "33");
            //writer.WriteAttributeString("d", "100");
            //writer.WriteEndElement();
            //writer.WriteEndElement();
            //writer.WriteStartElement("p:origin");
            //writer.WriteAttributeString("x", "0");
            //writer.WriteAttributeString("y", "0");
            //writer.WriteEndElement();
            //writer.WriteEndElement();
            //writer.WriteEndElement();

            writer.WriteStartElement("p:notesTextViewPr");
            writer.WriteStartElement("p:cViewPr");
            writer.WriteStartElement("p:scale");
            writer.WriteStartElement("a:sx");
            writer.WriteAttributeString("n", "100");
            writer.WriteAttributeString("d", "100");
            writer.WriteEndElement();
            writer.WriteStartElement("a:sy");
            writer.WriteAttributeString("n", "100");
            writer.WriteAttributeString("d", "100");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("p:origin");
            writer.WriteAttributeString("x", "0");
            writer.WriteAttributeString("y", "0");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("p:gridSpacing");
            writer.WriteAttributeString("cx", "73736200");
            writer.WriteAttributeString("cy", "73736200");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteTheme
        private MemoryStream WriteTheme()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("a:theme");
            writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            writer.WriteAttributeString("name", "Office Theme");

            writer.WriteStartElement("a:themeElements");

            #region a:clrScheme
            writer.WriteStartElement("a:clrScheme");
            writer.WriteAttributeString("name", "Office");
            writer.WriteStartElement("a:dk1");
            writer.WriteStartElement("a:sysClr");
            writer.WriteAttributeString("val", "windowText");
            writer.WriteAttributeString("lastClr", "000000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:lt1");
            writer.WriteStartElement("a:sysClr");
            writer.WriteAttributeString("val", "window");
            writer.WriteAttributeString("lastClr", "FFFFFF");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:dk2");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "1F497D");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:lt2");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "EEECE1");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:accent1");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "4F81BD");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:accent2");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "C0504D");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:accent3");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "9BBB59");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:accent4");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "8064A2");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:accent5");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "4BACC6");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:accent6");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "F79646");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:hlink");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "0000FF");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:folHlink");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "800080");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            #region a:fontScheme
            writer.WriteStartElement("a:fontScheme");
            writer.WriteAttributeString("name", "Office");

            #region a:majorFont
            writer.WriteStartElement("a:majorFont");
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "Calibri");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Jpan");
            writer.WriteAttributeString("typeface", "ＭＳ Ｐゴシック");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Hang");
            writer.WriteAttributeString("typeface", "맑은 고딕");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Hans");
            writer.WriteAttributeString("typeface", "宋体");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Hant");
            writer.WriteAttributeString("typeface", "新細明體");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Arab");
            writer.WriteAttributeString("typeface", "Times New Roman");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Hebr");
            writer.WriteAttributeString("typeface", "Times New Roman");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Thai");
            writer.WriteAttributeString("typeface", "Angsana New");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Ethi");
            writer.WriteAttributeString("typeface", "Nyala");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Beng");
            writer.WriteAttributeString("typeface", "Vrinda");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Gujr");
            writer.WriteAttributeString("typeface", "Shruti");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Khmr");
            writer.WriteAttributeString("typeface", "MoolBoran");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Knda");
            writer.WriteAttributeString("typeface", "Tunga");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Guru");
            writer.WriteAttributeString("typeface", "Raavi");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Cans");
            writer.WriteAttributeString("typeface", "Euphemia");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Cher");
            writer.WriteAttributeString("typeface", "Plantagenet Cherokee");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Yiii");
            writer.WriteAttributeString("typeface", "Microsoft Yi Baiti");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Tibt");
            writer.WriteAttributeString("typeface", "Microsoft Himalaya");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Thaa");
            writer.WriteAttributeString("typeface", "MV Boli");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Deva");
            writer.WriteAttributeString("typeface", "Mangal");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Telu");
            writer.WriteAttributeString("typeface", "Gautami");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Taml");
            writer.WriteAttributeString("typeface", "Latha");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Syrc");
            writer.WriteAttributeString("typeface", "Estrangelo Edessa");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Orya");
            writer.WriteAttributeString("typeface", "Kalinga");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Mlym");
            writer.WriteAttributeString("typeface", "Kartika");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Laoo");
            writer.WriteAttributeString("typeface", "DokChampa");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Sinh");
            writer.WriteAttributeString("typeface", "Iskoola Pota");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Mong");
            writer.WriteAttributeString("typeface", "Mongolian Baiti");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Viet");
            writer.WriteAttributeString("typeface", "Times New Roman");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Uigh");
            writer.WriteAttributeString("typeface", "Microsoft Uighur");
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            #region a:minorFont
            writer.WriteStartElement("a:minorFont");
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "Calibri");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Jpan");
            writer.WriteAttributeString("typeface", "ＭＳ Ｐゴシック");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Hang");
            writer.WriteAttributeString("typeface", "맑은 고딕");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Hans");
            writer.WriteAttributeString("typeface", "宋体");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Hant");
            writer.WriteAttributeString("typeface", "新細明體");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Arab");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Hebr");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Thai");
            writer.WriteAttributeString("typeface", "Cordia New");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Ethi");
            writer.WriteAttributeString("typeface", "Nyala");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Beng");
            writer.WriteAttributeString("typeface", "Vrinda");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Gujr");
            writer.WriteAttributeString("typeface", "Shruti");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Khmr");
            writer.WriteAttributeString("typeface", "DaunPenh");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Knda");
            writer.WriteAttributeString("typeface", "Tunga");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Guru");
            writer.WriteAttributeString("typeface", "Raavi");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Cans");
            writer.WriteAttributeString("typeface", "Euphemia");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Cher");
            writer.WriteAttributeString("typeface", "Plantagenet Cherokee");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Yiii");
            writer.WriteAttributeString("typeface", "Microsoft Yi Baiti");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Tibt");
            writer.WriteAttributeString("typeface", "Microsoft Himalaya");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Thaa");
            writer.WriteAttributeString("typeface", "MV Boli");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Deva");
            writer.WriteAttributeString("typeface", "Mangal");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Telu");
            writer.WriteAttributeString("typeface", "Gautami");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Taml");
            writer.WriteAttributeString("typeface", "Latha");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Syrc");
            writer.WriteAttributeString("typeface", "Estrangelo Edessa");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Orya");
            writer.WriteAttributeString("typeface", "Kalinga");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Mlym");
            writer.WriteAttributeString("typeface", "Kartika");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Laoo");
            writer.WriteAttributeString("typeface", "DokChampa");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Sinh");
            writer.WriteAttributeString("typeface", "Iskoola Pota");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Mong");
            writer.WriteAttributeString("typeface", "Mongolian Baiti");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Viet");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteEndElement();
            writer.WriteStartElement("a:font");
            writer.WriteAttributeString("script", "Uigh");
            writer.WriteAttributeString("typeface", "Microsoft Uighur");
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            writer.WriteEndElement();
            #endregion

            #region a:fmtScheme
            writer.WriteStartElement("a:fmtScheme");
            writer.WriteAttributeString("name", "Office");

            #region a:fillStyleLst
            writer.WriteStartElement("a:fillStyleLst");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:gradFill");
            writer.WriteAttributeString("rotWithShape", "1");
            writer.WriteStartElement("a:gsLst");
            writer.WriteStartElement("a:gs");
            writer.WriteAttributeString("pos", "0");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:tint");
            writer.WriteAttributeString("val", "50000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "300000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:gs");
            writer.WriteAttributeString("pos", "35000");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:tint");
            writer.WriteAttributeString("val", "37000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "300000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:gs");
            writer.WriteAttributeString("pos", "100000");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:tint");
            writer.WriteAttributeString("val", "15000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "350000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:lin");
            writer.WriteAttributeString("ang", "16200000");
            writer.WriteAttributeString("scaled", "1");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:gradFill");
            writer.WriteAttributeString("rotWithShape", "1");
            writer.WriteStartElement("a:gsLst");
            writer.WriteStartElement("a:gs");
            writer.WriteAttributeString("pos", "0");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:shade");
            writer.WriteAttributeString("val", "51000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "130000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:gs");
            writer.WriteAttributeString("pos", "80000");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:shade");
            writer.WriteAttributeString("val", "93000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "130000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:gs");
            writer.WriteAttributeString("pos", "100000");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:shade");
            writer.WriteAttributeString("val", "94000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "135000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:lin");
            writer.WriteAttributeString("ang", "16200000");
            writer.WriteAttributeString("scaled", "0");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            #region a:lnStyleLst
            writer.WriteStartElement("a:lnStyleLst");
            writer.WriteStartElement("a:ln");
            writer.WriteAttributeString("w", "9525");
            writer.WriteAttributeString("cap", "flat");
            writer.WriteAttributeString("cmpd", "sng");
            writer.WriteAttributeString("algn", "ctr");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:shade");
            writer.WriteAttributeString("val", "95000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "105000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:prstDash");
            writer.WriteAttributeString("val", "solid");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:ln");
            writer.WriteAttributeString("w", "25400");
            writer.WriteAttributeString("cap", "flat");
            writer.WriteAttributeString("cmpd", "sng");
            writer.WriteAttributeString("algn", "ctr");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:prstDash");
            writer.WriteAttributeString("val", "solid");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:ln");
            writer.WriteAttributeString("w", "38100");
            writer.WriteAttributeString("cap", "flat");
            writer.WriteAttributeString("cmpd", "sng");
            writer.WriteAttributeString("algn", "ctr");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:prstDash");
            writer.WriteAttributeString("val", "solid");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            #region a:effectStyleLst
            writer.WriteStartElement("a:effectStyleLst");
            writer.WriteStartElement("a:effectStyle");
            writer.WriteStartElement("a:effectLst");
            writer.WriteStartElement("a:outerShdw");
            writer.WriteAttributeString("blurRad", "40000");
            writer.WriteAttributeString("dist", "20000");
            writer.WriteAttributeString("dir", "5400000");
            writer.WriteAttributeString("rotWithShape", "0");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "000000");
            writer.WriteStartElement("a:alpha");
            writer.WriteAttributeString("val", "38000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:effectStyle");
            writer.WriteStartElement("a:effectLst");
            writer.WriteStartElement("a:outerShdw");
            writer.WriteAttributeString("blurRad", "40000");
            writer.WriteAttributeString("dist", "23000");
            writer.WriteAttributeString("dir", "5400000");
            writer.WriteAttributeString("rotWithShape", "0");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "000000");
            writer.WriteStartElement("a:alpha");
            writer.WriteAttributeString("val", "35000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:effectStyle");
            writer.WriteStartElement("a:effectLst");
            writer.WriteStartElement("a:outerShdw");
            writer.WriteAttributeString("blurRad", "40000");
            writer.WriteAttributeString("dist", "23000");
            writer.WriteAttributeString("dir", "5400000");
            writer.WriteAttributeString("rotWithShape", "0");
            writer.WriteStartElement("a:srgbClr");
            writer.WriteAttributeString("val", "000000");
            writer.WriteStartElement("a:alpha");
            writer.WriteAttributeString("val", "35000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:scene3d");
            writer.WriteStartElement("a:camera");
            writer.WriteAttributeString("prst", "orthographicFront");
            writer.WriteStartElement("a:rot");
            writer.WriteAttributeString("lat", "0");
            writer.WriteAttributeString("lon", "0");
            writer.WriteAttributeString("rev", "0");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:lightRig");
            writer.WriteAttributeString("rig", "threePt");
            writer.WriteAttributeString("dir", "t");
            writer.WriteStartElement("a:rot");
            writer.WriteAttributeString("lat", "0");
            writer.WriteAttributeString("lon", "0");
            writer.WriteAttributeString("rev", "1200000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:sp3d");
            writer.WriteStartElement("a:bevelT");
            writer.WriteAttributeString("w", "63500");
            writer.WriteAttributeString("h", "25400");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            #region a:bgFillStyleLst
            writer.WriteStartElement("a:bgFillStyleLst");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:gradFill");
            writer.WriteAttributeString("rotWithShape", "1");
            writer.WriteStartElement("a:gsLst");
            writer.WriteStartElement("a:gs");
            writer.WriteAttributeString("pos", "0");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:tint");
            writer.WriteAttributeString("val", "40000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "350000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:gs");
            writer.WriteAttributeString("pos", "40000");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:tint");
            writer.WriteAttributeString("val", "45000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:shade");
            writer.WriteAttributeString("val", "99000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "350000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:gs");
            writer.WriteAttributeString("pos", "100000");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:shade");
            writer.WriteAttributeString("val", "20000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "255000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:path");
            writer.WriteAttributeString("path", "circle");
            writer.WriteStartElement("a:fillToRect");
            writer.WriteAttributeString("l", "50000");
            writer.WriteAttributeString("t", "-80000");
            writer.WriteAttributeString("r", "50000");
            writer.WriteAttributeString("b", "180000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:gradFill");
            writer.WriteAttributeString("rotWithShape", "1");
            writer.WriteStartElement("a:gsLst");
            writer.WriteStartElement("a:gs");
            writer.WriteAttributeString("pos", "0");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:tint");
            writer.WriteAttributeString("val", "80000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "300000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:gs");
            writer.WriteAttributeString("pos", "100000");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "phClr");
            writer.WriteStartElement("a:shade");
            writer.WriteAttributeString("val", "30000");
            writer.WriteEndElement();
            writer.WriteStartElement("a:satMod");
            writer.WriteAttributeString("val", "200000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:path");
            writer.WriteAttributeString("path", "circle");
            writer.WriteStartElement("a:fillToRect");
            writer.WriteAttributeString("l", "50000");
            writer.WriteAttributeString("t", "50000");
            writer.WriteAttributeString("r", "50000");
            writer.WriteAttributeString("b", "50000");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            writer.WriteEndElement();
            #endregion

            writer.WriteEndElement();

            writer.WriteStartElement("a:objectDefaults");
            writer.WriteEndElement();
            writer.WriteStartElement("a:extraClrSchemeLst");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteSlideMasterRels
        private MemoryStream WriteSlideMasterRels()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            for (int index = 0; index < 11; index++)
            {
                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", string.Format("rId{0}", index + 1));
                writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideLayout");
                writer.WriteAttributeString("Target", string.Format("../slideLayouts/slideLayout{0}.xml", index + 1));
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId12");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme");
            writer.WriteAttributeString("Target", "../theme/theme1.xml");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteSlideMaster
        private MemoryStream WriteSlideMaster()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();

            writer.WriteStartElement("p:sldMaster");
            writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:p", "http://schemas.openxmlformats.org/presentationml/2006/main");

            #region p:cSld
            writer.WriteStartElement("p:cSld");
            writer.WriteStartElement("p:bg");
            writer.WriteStartElement("p:bgRef");
            writer.WriteAttributeString("idx", "1001");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "bg1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:spTree");
            writer.WriteStartElement("p:nvGrpSpPr");
            writer.WriteStartElement("p:cNvPr");
            writer.WriteAttributeString("id", "1");
            writer.WriteAttributeString("name", "");
            writer.WriteEndElement();
            writer.WriteStartElement("p:cNvGrpSpPr");
            writer.WriteEndElement();
            writer.WriteStartElement("p:nvPr");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:grpSpPr");
            writer.WriteStartElement("a:xfrm");
            writer.WriteStartElement("a:off");
            writer.WriteAttributeString("x", "0");
            writer.WriteAttributeString("y", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ext");
            writer.WriteAttributeString("cx", "0");
            writer.WriteAttributeString("cy", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:chOff");
            writer.WriteAttributeString("x", "0");
            writer.WriteAttributeString("y", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:chExt");
            writer.WriteAttributeString("cx", "0");
            writer.WriteAttributeString("cy", "0");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:sp");
            writer.WriteStartElement("p:nvSpPr");
            writer.WriteStartElement("p:cNvPr");
            writer.WriteAttributeString("id", "2");
            writer.WriteAttributeString("name", "Title Placeholder 1");
            writer.WriteEndElement();
            writer.WriteStartElement("p:cNvSpPr");
            writer.WriteStartElement("a:spLocks");
            writer.WriteAttributeString("noGrp", "1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:nvPr");
            writer.WriteStartElement("p:ph");
            writer.WriteAttributeString("type", "title");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:spPr");
            writer.WriteStartElement("a:xfrm");
            writer.WriteStartElement("a:off");
            writer.WriteAttributeString("x", "457200");
            writer.WriteAttributeString("y", "274638");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ext");
            writer.WriteAttributeString("cx", "8229600");
            writer.WriteAttributeString("cy", "1143000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:prstGeom");
            writer.WriteAttributeString("prst", "rect");
            writer.WriteStartElement("a:avLst");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:txBody");
            writer.WriteStartElement("a:bodyPr");
            writer.WriteAttributeString("vert", "horz");
            writer.WriteAttributeString("lIns", "91440");
            writer.WriteAttributeString("tIns", "45720");
            writer.WriteAttributeString("rIns", "91440");
            writer.WriteAttributeString("bIns", "45720");
            writer.WriteAttributeString("rtlCol", "0");
            writer.WriteAttributeString("anchor", "ctr");
            writer.WriteStartElement("a:normAutofit");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lstStyle");
            writer.WriteEndElement();
            writer.WriteStartElement("a:p");
            writer.WriteStartElement("a:r");
            writer.WriteStartElement("a:rPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteAttributeString("smtClean", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:t");
            writer.WriteString("Click to edit Master title style");
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:endParaRPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:sp");
            writer.WriteStartElement("p:nvSpPr");
            writer.WriteStartElement("p:cNvPr");
            writer.WriteAttributeString("id", "3");
            writer.WriteAttributeString("name", "Text Placeholder 2");
            writer.WriteEndElement();
            writer.WriteStartElement("p:cNvSpPr");
            writer.WriteStartElement("a:spLocks");
            writer.WriteAttributeString("noGrp", "1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:nvPr");
            writer.WriteStartElement("p:ph");
            writer.WriteAttributeString("type", "body");
            writer.WriteAttributeString("idx", "1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:spPr");
            writer.WriteStartElement("a:xfrm");
            writer.WriteStartElement("a:off");
            writer.WriteAttributeString("x", "457200");
            writer.WriteAttributeString("y", "1600200");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ext");
            writer.WriteAttributeString("cx", "8229600");
            writer.WriteAttributeString("cy", "4525963");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:prstGeom");
            writer.WriteAttributeString("prst", "rect");
            writer.WriteStartElement("a:avLst");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:txBody");
            writer.WriteStartElement("a:bodyPr");
            writer.WriteAttributeString("vert", "horz");
            writer.WriteAttributeString("lIns", "91440");
            writer.WriteAttributeString("tIns", "45720");
            writer.WriteAttributeString("rIns", "91440");
            writer.WriteAttributeString("bIns", "45720");
            writer.WriteAttributeString("rtlCol", "0");
            writer.WriteStartElement("a:normAutofit");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lstStyle");
            writer.WriteEndElement();
            writer.WriteStartElement("a:p");
            writer.WriteStartElement("a:pPr");
            writer.WriteAttributeString("lvl", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:r");
            writer.WriteStartElement("a:rPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteAttributeString("smtClean", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:t");
            writer.WriteString("Click to edit Master text styles");
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:p");
            writer.WriteStartElement("a:pPr");
            writer.WriteAttributeString("lvl", "1");
            writer.WriteEndElement();
            writer.WriteStartElement("a:r");
            writer.WriteStartElement("a:rPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteAttributeString("smtClean", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:t");
            writer.WriteString("Second level");
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:p");
            writer.WriteStartElement("a:pPr");
            writer.WriteAttributeString("lvl", "2");
            writer.WriteEndElement();
            writer.WriteStartElement("a:r");
            writer.WriteStartElement("a:rPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteAttributeString("smtClean", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:t");
            writer.WriteString("Third level");
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:p");
            writer.WriteStartElement("a:pPr");
            writer.WriteAttributeString("lvl", "3");
            writer.WriteEndElement();
            writer.WriteStartElement("a:r");
            writer.WriteStartElement("a:rPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteAttributeString("smtClean", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:t");
            writer.WriteString("Fourth level");
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:p");
            writer.WriteStartElement("a:pPr");
            writer.WriteAttributeString("lvl", "4");
            writer.WriteEndElement();
            writer.WriteStartElement("a:r");
            writer.WriteStartElement("a:rPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteAttributeString("smtClean", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:t");
            writer.WriteString("Fifth level");
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:endParaRPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:sp");
            writer.WriteStartElement("p:nvSpPr");
            writer.WriteStartElement("p:cNvPr");
            writer.WriteAttributeString("id", "4");
            writer.WriteAttributeString("name", "Date Placeholder 3");
            writer.WriteEndElement();
            writer.WriteStartElement("p:cNvSpPr");
            writer.WriteStartElement("a:spLocks");
            writer.WriteAttributeString("noGrp", "1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:nvPr");
            writer.WriteStartElement("p:ph");
            writer.WriteAttributeString("type", "dt");
            writer.WriteAttributeString("sz", "half");
            writer.WriteAttributeString("idx", "2");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:spPr");
            writer.WriteStartElement("a:xfrm");
            writer.WriteStartElement("a:off");
            writer.WriteAttributeString("x", "457200");
            writer.WriteAttributeString("y", "6356350");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ext");
            writer.WriteAttributeString("cx", "2133600");
            writer.WriteAttributeString("cy", "365125");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:prstGeom");
            writer.WriteAttributeString("prst", "rect");
            writer.WriteStartElement("a:avLst");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:txBody");
            writer.WriteStartElement("a:bodyPr");
            writer.WriteAttributeString("vert", "horz");
            writer.WriteAttributeString("lIns", "91440");
            writer.WriteAttributeString("tIns", "45720");
            writer.WriteAttributeString("rIns", "91440");
            writer.WriteAttributeString("bIns", "45720");
            writer.WriteAttributeString("rtlCol", "0");
            writer.WriteAttributeString("anchor", "ctr");
            writer.WriteEndElement();
            writer.WriteStartElement("a:lstStyle");
            writer.WriteStartElement("a:lvl1pPr");
            writer.WriteAttributeString("algn", "l");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteStartElement("a:tint");
            writer.WriteAttributeString("val", "75000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:p");
            writer.WriteStartElement("a:fld");
            writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
            writer.WriteAttributeString("type", "datetimeFigureOut");
            writer.WriteStartElement("a:rPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteAttributeString("smtClean", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:t");
            writer.WriteString("15.04.2009");
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:endParaRPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:sp");
            writer.WriteStartElement("p:nvSpPr");
            writer.WriteStartElement("p:cNvPr");
            writer.WriteAttributeString("id", "5");
            writer.WriteAttributeString("name", "Footer Placeholder 4");
            writer.WriteEndElement();
            writer.WriteStartElement("p:cNvSpPr");
            writer.WriteStartElement("a:spLocks");
            writer.WriteAttributeString("noGrp", "1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:nvPr");
            writer.WriteStartElement("p:ph");
            writer.WriteAttributeString("type", "ftr");
            writer.WriteAttributeString("sz", "quarter");
            writer.WriteAttributeString("idx", "3");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:spPr");
            writer.WriteStartElement("a:xfrm");
            writer.WriteStartElement("a:off");
            writer.WriteAttributeString("x", "3124200");
            writer.WriteAttributeString("y", "6356350");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ext");
            writer.WriteAttributeString("cx", "2895600");
            writer.WriteAttributeString("cy", "365125");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:prstGeom");
            writer.WriteAttributeString("prst", "rect");
            writer.WriteStartElement("a:avLst");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:txBody");
            writer.WriteStartElement("a:bodyPr");
            writer.WriteAttributeString("vert", "horz");
            writer.WriteAttributeString("lIns", "91440");
            writer.WriteAttributeString("tIns", "45720");
            writer.WriteAttributeString("rIns", "91440");
            writer.WriteAttributeString("bIns", "45720");
            writer.WriteAttributeString("rtlCol", "0");
            writer.WriteAttributeString("anchor", "ctr");
            writer.WriteEndElement();
            writer.WriteStartElement("a:lstStyle");
            writer.WriteStartElement("a:lvl1pPr");
            writer.WriteAttributeString("algn", "ctr");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteStartElement("a:tint");
            writer.WriteAttributeString("val", "75000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:p");
            writer.WriteStartElement("a:endParaRPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:sp");
            writer.WriteStartElement("p:nvSpPr");
            writer.WriteStartElement("p:cNvPr");
            writer.WriteAttributeString("id", "6");
            writer.WriteAttributeString("name", "Slide Number Placeholder 5");
            writer.WriteEndElement();
            writer.WriteStartElement("p:cNvSpPr");
            writer.WriteStartElement("a:spLocks");
            writer.WriteAttributeString("noGrp", "1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:nvPr");
            writer.WriteStartElement("p:ph");
            writer.WriteAttributeString("type", "sldNum");
            writer.WriteAttributeString("sz", "quarter");
            writer.WriteAttributeString("idx", "4");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:spPr");
            writer.WriteStartElement("a:xfrm");
            writer.WriteStartElement("a:off");
            writer.WriteAttributeString("x", "6553200");
            writer.WriteAttributeString("y", "6356350");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ext");
            writer.WriteAttributeString("cx", "2133600");
            writer.WriteAttributeString("cy", "365125");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:prstGeom");
            writer.WriteAttributeString("prst", "rect");
            writer.WriteStartElement("a:avLst");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:txBody");
            writer.WriteStartElement("a:bodyPr");
            writer.WriteAttributeString("vert", "horz");
            writer.WriteAttributeString("lIns", "91440");
            writer.WriteAttributeString("tIns", "45720");
            writer.WriteAttributeString("rIns", "91440");
            writer.WriteAttributeString("bIns", "45720");
            writer.WriteAttributeString("rtlCol", "0");
            writer.WriteAttributeString("anchor", "ctr");
            writer.WriteEndElement();
            writer.WriteStartElement("a:lstStyle");
            writer.WriteStartElement("a:lvl1pPr");
            writer.WriteAttributeString("algn", "r");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteStartElement("a:tint");
            writer.WriteAttributeString("val", "75000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:p");
            writer.WriteStartElement("a:fld");
            writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
            writer.WriteAttributeString("type", "slidenum");
            writer.WriteStartElement("a:rPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteAttributeString("smtClean", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:t");
            writer.WriteString("‹#›");
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:endParaRPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            #endregion

            #region p:clrMap
            writer.WriteStartElement("p:clrMap");
            writer.WriteAttributeString("bg1", "lt1");
            writer.WriteAttributeString("tx1", "dk1");
            writer.WriteAttributeString("bg2", "lt2");
            writer.WriteAttributeString("tx2", "dk2");
            writer.WriteAttributeString("accent1", "accent1");
            writer.WriteAttributeString("accent2", "accent2");
            writer.WriteAttributeString("accent3", "accent3");
            writer.WriteAttributeString("accent4", "accent4");
            writer.WriteAttributeString("accent5", "accent5");
            writer.WriteAttributeString("accent6", "accent6");
            writer.WriteAttributeString("hlink", "hlink");
            writer.WriteAttributeString("folHlink", "folHlink");
            writer.WriteEndElement();
            #endregion

            #region p:sldLayoutIdLst
            writer.WriteStartElement("p:sldLayoutIdLst");
            writer.WriteStartElement("p:sldLayoutId");
            writer.WriteAttributeString("id", "2147483649");
            writer.WriteAttributeString("r:id", "rId1");
            writer.WriteEndElement();
            writer.WriteStartElement("p:sldLayoutId");
            writer.WriteAttributeString("id", "2147483650");
            writer.WriteAttributeString("r:id", "rId2");
            writer.WriteEndElement();
            writer.WriteStartElement("p:sldLayoutId");
            writer.WriteAttributeString("id", "2147483651");
            writer.WriteAttributeString("r:id", "rId3");
            writer.WriteEndElement();
            writer.WriteStartElement("p:sldLayoutId");
            writer.WriteAttributeString("id", "2147483652");
            writer.WriteAttributeString("r:id", "rId4");
            writer.WriteEndElement();
            writer.WriteStartElement("p:sldLayoutId");
            writer.WriteAttributeString("id", "2147483653");
            writer.WriteAttributeString("r:id", "rId5");
            writer.WriteEndElement();
            writer.WriteStartElement("p:sldLayoutId");
            writer.WriteAttributeString("id", "2147483654");
            writer.WriteAttributeString("r:id", "rId6");
            writer.WriteEndElement();
            writer.WriteStartElement("p:sldLayoutId");
            writer.WriteAttributeString("id", "2147483655");
            writer.WriteAttributeString("r:id", "rId7");
            writer.WriteEndElement();
            writer.WriteStartElement("p:sldLayoutId");
            writer.WriteAttributeString("id", "2147483656");
            writer.WriteAttributeString("r:id", "rId8");
            writer.WriteEndElement();
            writer.WriteStartElement("p:sldLayoutId");
            writer.WriteAttributeString("id", "2147483657");
            writer.WriteAttributeString("r:id", "rId9");
            writer.WriteEndElement();
            writer.WriteStartElement("p:sldLayoutId");
            writer.WriteAttributeString("id", "2147483658");
            writer.WriteAttributeString("r:id", "rId10");
            writer.WriteEndElement();
            writer.WriteStartElement("p:sldLayoutId");
            writer.WriteAttributeString("id", "2147483659");
            writer.WriteAttributeString("r:id", "rId11");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            #endregion

            #region p:txStyles
            writer.WriteStartElement("p:txStyles");
            writer.WriteStartElement("p:titleStyle");
            writer.WriteStartElement("a:lvl1pPr");
            writer.WriteAttributeString("algn", "ctr");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:spcBef");
            writer.WriteStartElement("a:spcPct");
            writer.WriteAttributeString("val", "0");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:buNone");
            writer.WriteEndElement();
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "4400");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mj-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mj-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mj-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:bodyStyle");
            writer.WriteStartElement("a:lvl1pPr");
            writer.WriteAttributeString("marL", "342900");
            writer.WriteAttributeString("indent", "-342900");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:spcBef");
            writer.WriteStartElement("a:spcPct");
            writer.WriteAttributeString("val", "20000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:buFont");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteAttributeString("pitchFamily", "34");
            writer.WriteAttributeString("charset", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:buChar");
            writer.WriteAttributeString("char", "•");
            writer.WriteEndElement();
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "3200");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl2pPr");
            writer.WriteAttributeString("marL", "742950");
            writer.WriteAttributeString("indent", "-285750");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:spcBef");
            writer.WriteStartElement("a:spcPct");
            writer.WriteAttributeString("val", "20000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:buFont");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteAttributeString("pitchFamily", "34");
            writer.WriteAttributeString("charset", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:buChar");
            writer.WriteAttributeString("char", "–");
            writer.WriteEndElement();
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "2800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl3pPr");
            writer.WriteAttributeString("marL", "1143000");
            writer.WriteAttributeString("indent", "-228600");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:spcBef");
            writer.WriteStartElement("a:spcPct");
            writer.WriteAttributeString("val", "20000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:buFont");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteAttributeString("pitchFamily", "34");
            writer.WriteAttributeString("charset", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:buChar");
            writer.WriteAttributeString("char", "•");
            writer.WriteEndElement();
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "2400");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl4pPr");
            writer.WriteAttributeString("marL", "1600200");
            writer.WriteAttributeString("indent", "-228600");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:spcBef");
            writer.WriteStartElement("a:spcPct");
            writer.WriteAttributeString("val", "20000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:buFont");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteAttributeString("pitchFamily", "34");
            writer.WriteAttributeString("charset", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:buChar");
            writer.WriteAttributeString("char", "–");
            writer.WriteEndElement();
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "2000");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl5pPr");
            writer.WriteAttributeString("marL", "2057400");
            writer.WriteAttributeString("indent", "-228600");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:spcBef");
            writer.WriteStartElement("a:spcPct");
            writer.WriteAttributeString("val", "20000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:buFont");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteAttributeString("pitchFamily", "34");
            writer.WriteAttributeString("charset", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:buChar");
            writer.WriteAttributeString("char", "»");
            writer.WriteEndElement();
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "2000");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl6pPr");
            writer.WriteAttributeString("marL", "2514600");
            writer.WriteAttributeString("indent", "-228600");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:spcBef");
            writer.WriteStartElement("a:spcPct");
            writer.WriteAttributeString("val", "20000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:buFont");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteAttributeString("pitchFamily", "34");
            writer.WriteAttributeString("charset", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:buChar");
            writer.WriteAttributeString("char", "•");
            writer.WriteEndElement();
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "2000");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl7pPr");
            writer.WriteAttributeString("marL", "2971800");
            writer.WriteAttributeString("indent", "-228600");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:spcBef");
            writer.WriteStartElement("a:spcPct");
            writer.WriteAttributeString("val", "20000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:buFont");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteAttributeString("pitchFamily", "34");
            writer.WriteAttributeString("charset", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:buChar");
            writer.WriteAttributeString("char", "•");
            writer.WriteEndElement();
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "2000");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl8pPr");
            writer.WriteAttributeString("marL", "3429000");
            writer.WriteAttributeString("indent", "-228600");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:spcBef");
            writer.WriteStartElement("a:spcPct");
            writer.WriteAttributeString("val", "20000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:buFont");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteAttributeString("pitchFamily", "34");
            writer.WriteAttributeString("charset", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:buChar");
            writer.WriteAttributeString("char", "•");
            writer.WriteEndElement();
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "2000");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl9pPr");
            writer.WriteAttributeString("marL", "3886200");
            writer.WriteAttributeString("indent", "-228600");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:spcBef");
            writer.WriteStartElement("a:spcPct");
            writer.WriteAttributeString("val", "20000");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:buFont");
            writer.WriteAttributeString("typeface", "Arial");
            writer.WriteAttributeString("pitchFamily", "34");
            writer.WriteAttributeString("charset", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:buChar");
            writer.WriteAttributeString("char", "•");
            writer.WriteEndElement();
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "2000");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("p:otherStyle");
            writer.WriteStartElement("a:defPPr");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl1pPr");
            writer.WriteAttributeString("marL", "0");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl2pPr");
            writer.WriteAttributeString("marL", "457200");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl3pPr");
            writer.WriteAttributeString("marL", "914400");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl4pPr");
            writer.WriteAttributeString("marL", "1371600");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl5pPr");
            writer.WriteAttributeString("marL", "1828800");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl6pPr");
            writer.WriteAttributeString("marL", "2286000");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl7pPr");
            writer.WriteAttributeString("marL", "2743200");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl8pPr");
            writer.WriteAttributeString("marL", "3200400");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl9pPr");
            writer.WriteAttributeString("marL", "3657600");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            #endregion

            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteSlideLayoutRels
        private MemoryStream WriteSlideLayoutRels()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId1");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideMaster");
            writer.WriteAttributeString("Target", "../slideMasters/slideMaster1.xml");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteSlideLayout
        private MemoryStream WriteSlideLayout(int index, StiPage page)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("p:sldLayout");
            writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:p", "http://schemas.openxmlformats.org/presentationml/2006/main");

            if (index == 1)
            {
                #region Layout1
                writer.WriteAttributeString("type", "title");
                writer.WriteAttributeString("preserve", "1");

                writer.WriteStartElement("p:cSld");
                writer.WriteAttributeString("name", "Title Slide");
                writer.WriteStartElement("p:spTree");
                writer.WriteStartElement("p:nvGrpSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "1");
                writer.WriteAttributeString("name", "");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvGrpSpPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:grpSpPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chOff");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chExt");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "2");
                writer.WriteAttributeString("name", "Title 1");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ctrTitle");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "685800");
                writer.WriteAttributeString("y", "2130425");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "7772400");
                writer.WriteAttributeString("cy", "1470025");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master title style");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "3");
                writer.WriteAttributeString("name", "Subtitle 2");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "subTitle");
                writer.WriteAttributeString("idx", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "1371600");
                writer.WriteAttributeString("y", "3886200");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "6400800");
                writer.WriteAttributeString("cy", "1752600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteAttributeString("marL", "0");
                writer.WriteAttributeString("indent", "0");
                writer.WriteAttributeString("algn", "ctr");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteAttributeString("marL", "457200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteAttributeString("algn", "ctr");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteAttributeString("marL", "914400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteAttributeString("algn", "ctr");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteAttributeString("marL", "1371600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteAttributeString("algn", "ctr");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteAttributeString("marL", "1828800");
                writer.WriteAttributeString("indent", "0");
                writer.WriteAttributeString("algn", "ctr");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteAttributeString("marL", "2286000");
                writer.WriteAttributeString("indent", "0");
                writer.WriteAttributeString("algn", "ctr");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteAttributeString("marL", "2743200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteAttributeString("algn", "ctr");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteAttributeString("marL", "3200400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteAttributeString("algn", "ctr");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteAttributeString("marL", "3657600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteAttributeString("algn", "ctr");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master subtitle style");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "4");
                writer.WriteAttributeString("name", "Date Placeholder 3");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "dt");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "10");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
                writer.WriteAttributeString("type", "datetimeFigureOut");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("15.04.2009");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "5");
                writer.WriteAttributeString("name", "Footer Placeholder 4");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ftr");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "11");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "6");
                writer.WriteAttributeString("name", "Slide Number Placeholder 5");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "sldNum");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "12");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
                writer.WriteAttributeString("type", "slidenum");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("‹#›");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                #endregion
            }
            if (index == 2)
            {
                #region Layout2
                writer.WriteAttributeString("type", "obj");
                writer.WriteAttributeString("preserve", "1");

                writer.WriteStartElement("p:cSld");
                writer.WriteAttributeString("name", "Title and Content");
                writer.WriteStartElement("p:spTree");
                writer.WriteStartElement("p:nvGrpSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "1");
                writer.WriteAttributeString("name", "");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvGrpSpPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:grpSpPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chOff");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chExt");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "2");
                writer.WriteAttributeString("name", "Title 1");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "title");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master title style");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "3");
                writer.WriteAttributeString("name", "Content Placeholder 2");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("idx", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "1");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Second level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "2");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Third level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "3");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fourth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "4");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fifth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "4");
                writer.WriteAttributeString("name", "Date Placeholder 3");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "dt");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "10");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
                writer.WriteAttributeString("type", "datetimeFigureOut");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("15.04.2009");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "5");
                writer.WriteAttributeString("name", "Footer Placeholder 4");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ftr");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "11");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "6");
                writer.WriteAttributeString("name", "Slide Number Placeholder 5");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "sldNum");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "12");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
                writer.WriteAttributeString("type", "slidenum");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("‹#›");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                #endregion
            }
            if (index == 3)
            {
                #region Layout3
                writer.WriteAttributeString("type", "secHead");
                writer.WriteAttributeString("preserve", "1");

                writer.WriteStartElement("p:cSld");
                writer.WriteAttributeString("name", "Section Header");
                writer.WriteStartElement("p:spTree");
                writer.WriteStartElement("p:nvGrpSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "1");
                writer.WriteAttributeString("name", "");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvGrpSpPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:grpSpPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chOff");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chExt");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "2");
                writer.WriteAttributeString("name", "Title 1");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "title");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "722313");
                writer.WriteAttributeString("y", "4406900");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "7772400");
                writer.WriteAttributeString("cy", "1362075");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteAttributeString("anchor", "t");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteAttributeString("algn", "l");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "4000");
                writer.WriteAttributeString("b", "1");
                writer.WriteAttributeString("cap", "all");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master title style");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "3");
                writer.WriteAttributeString("name", "Text Placeholder 2");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "body");
                writer.WriteAttributeString("idx", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "722313");
                writer.WriteAttributeString("y", "2906713");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "7772400");
                writer.WriteAttributeString("cy", "1500187");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteAttributeString("anchor", "b");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteAttributeString("marL", "0");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteAttributeString("marL", "457200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteAttributeString("marL", "914400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteAttributeString("marL", "1371600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1400");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteAttributeString("marL", "1828800");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1400");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteAttributeString("marL", "2286000");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1400");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteAttributeString("marL", "2743200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1400");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteAttributeString("marL", "3200400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1400");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteAttributeString("marL", "3657600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1400");
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:schemeClr");
                writer.WriteAttributeString("val", "tx1");
                writer.WriteStartElement("a:tint");
                writer.WriteAttributeString("val", "75000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "4");
                writer.WriteAttributeString("name", "Date Placeholder 3");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "dt");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "10");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
                writer.WriteAttributeString("type", "datetimeFigureOut");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("15.04.2009");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "5");
                writer.WriteAttributeString("name", "Footer Placeholder 4");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ftr");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "11");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "6");
                writer.WriteAttributeString("name", "Slide Number Placeholder 5");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "sldNum");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "12");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
                writer.WriteAttributeString("type", "slidenum");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("‹#›");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                #endregion
            }
            if (index == 4)
            {
                #region Layout4
                writer.WriteAttributeString("type", "twoObj");
                writer.WriteAttributeString("preserve", "1");

                writer.WriteStartElement("p:cSld");
                writer.WriteAttributeString("name", "Two Content");
                writer.WriteStartElement("p:spTree");
                writer.WriteStartElement("p:nvGrpSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "1");
                writer.WriteAttributeString("name", "");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvGrpSpPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:grpSpPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chOff");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chExt");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "2");
                writer.WriteAttributeString("name", "Title 1");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "title");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master title style");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "3");
                writer.WriteAttributeString("name", "Content Placeholder 2");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "457200");
                writer.WriteAttributeString("y", "1600200");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "4038600");
                writer.WriteAttributeString("cy", "4525963");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2400");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "1");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Second level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "2");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Third level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "3");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fourth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "4");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fifth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "4");
                writer.WriteAttributeString("name", "Content Placeholder 3");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "2");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "4648200");
                writer.WriteAttributeString("y", "1600200");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "4038600");
                writer.WriteAttributeString("cy", "4525963");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2400");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "1");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Second level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "2");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Third level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "3");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fourth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "4");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fifth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "5");
                writer.WriteAttributeString("name", "Date Placeholder 4");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "dt");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "10");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
                writer.WriteAttributeString("type", "datetimeFigureOut");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("15.04.2009");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "6");
                writer.WriteAttributeString("name", "Footer Placeholder 5");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ftr");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "11");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "7");
                writer.WriteAttributeString("name", "Slide Number Placeholder 6");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "sldNum");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "12");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
                writer.WriteAttributeString("type", "slidenum");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("‹#›");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                #endregion
            }
            if (index == 5)
            {
                #region Layout5
                writer.WriteAttributeString("type", "twoTxTwoObj");
                writer.WriteAttributeString("preserve", "1");

                writer.WriteStartElement("p:cSld");
                writer.WriteAttributeString("name", "Comparison");
                writer.WriteStartElement("p:spTree");
                writer.WriteStartElement("p:nvGrpSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "1");
                writer.WriteAttributeString("name", "");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvGrpSpPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:grpSpPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chOff");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chExt");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "2");
                writer.WriteAttributeString("name", "Title 1");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "title");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master title style");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "3");
                writer.WriteAttributeString("name", "Text Placeholder 2");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "body");
                writer.WriteAttributeString("idx", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "457200");
                writer.WriteAttributeString("y", "1535113");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "4040188");
                writer.WriteAttributeString("cy", "639762");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteAttributeString("anchor", "b");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteAttributeString("marL", "0");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2400");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteAttributeString("marL", "457200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteAttributeString("marL", "914400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteAttributeString("marL", "1371600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteAttributeString("marL", "1828800");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteAttributeString("marL", "2286000");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteAttributeString("marL", "2743200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteAttributeString("marL", "3200400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteAttributeString("marL", "3657600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "4");
                writer.WriteAttributeString("name", "Content Placeholder 3");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "2");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "457200");
                writer.WriteAttributeString("y", "2174875");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "4040188");
                writer.WriteAttributeString("cy", "3951288");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2400");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "1");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Second level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "2");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Third level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "3");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fourth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "4");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fifth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "5");
                writer.WriteAttributeString("name", "Text Placeholder 4");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "body");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "3");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "4645025");
                writer.WriteAttributeString("y", "1535113");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "4041775");
                writer.WriteAttributeString("cy", "639762");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteAttributeString("anchor", "b");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteAttributeString("marL", "0");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2400");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteAttributeString("marL", "457200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteAttributeString("marL", "914400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteAttributeString("marL", "1371600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteAttributeString("marL", "1828800");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteAttributeString("marL", "2286000");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteAttributeString("marL", "2743200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteAttributeString("marL", "3200400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteAttributeString("marL", "3657600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "6");
                writer.WriteAttributeString("name", "Content Placeholder 5");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "4");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "4645025");
                writer.WriteAttributeString("y", "2174875");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "4041775");
                writer.WriteAttributeString("cy", "3951288");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2400");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1600");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "1");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Second level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "2");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Third level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "3");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fourth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "4");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fifth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "7");
                writer.WriteAttributeString("name", "Date Placeholder 6");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "dt");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "10");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
                writer.WriteAttributeString("type", "datetimeFigureOut");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("15.04.2009");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "8");
                writer.WriteAttributeString("name", "Footer Placeholder 7");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ftr");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "11");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "9");
                writer.WriteAttributeString("name", "Slide Number Placeholder 8");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "sldNum");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "12");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
                writer.WriteAttributeString("type", "slidenum");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("‹#›");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                #endregion
            }
            if (index == 6)
            {
                #region Layout6
                writer.WriteAttributeString("type", "titleOnly");
                writer.WriteAttributeString("preserve", "1");

                writer.WriteStartElement("p:cSld");
                writer.WriteAttributeString("name", "Title Only");
                writer.WriteStartElement("p:spTree");
                writer.WriteStartElement("p:nvGrpSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "1");
                writer.WriteAttributeString("name", "");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvGrpSpPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:grpSpPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chOff");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chExt");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "2");
                writer.WriteAttributeString("name", "Title 1");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "title");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master title style");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "3");
                writer.WriteAttributeString("name", "Date Placeholder 2");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "dt");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "10");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
                writer.WriteAttributeString("type", "datetimeFigureOut");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("15.04.2009");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "4");
                writer.WriteAttributeString("name", "Footer Placeholder 3");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ftr");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "11");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "5");
                writer.WriteAttributeString("name", "Slide Number Placeholder 4");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "sldNum");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "12");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
                writer.WriteAttributeString("type", "slidenum");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("‹#›");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                #endregion
            }
            if (index == 7)
            {
                #region Layout7
                writer.WriteAttributeString("type", "blank");
                writer.WriteAttributeString("preserve", "1");
                writer.WriteStartElement("p:cSld");
                writer.WriteAttributeString("name", "Blank");
                writer.WriteStartElement("p:spTree");
                writer.WriteStartElement("p:nvGrpSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "1");
                writer.WriteAttributeString("name", "");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvGrpSpPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:grpSpPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chOff");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chExt");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();

                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "2");
                writer.WriteAttributeString("name", "Date Placeholder 1");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "dt");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "10");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
                writer.WriteAttributeString("type", "datetimeFigureOut");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("15.04.2009");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();

                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "3");
                writer.WriteAttributeString("name", "Footer Placeholder 2");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ftr");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "11");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();

                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "4");
                writer.WriteAttributeString("name", "Slide Number Placeholder 3");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "sldNum");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "12");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
                writer.WriteAttributeString("type", "slidenum");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("‹#›");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();

                #region Trial
#if SERVER
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
                    double pageWidth = page.Unit.ConvertToHInches(page.PageWidth - page.Margins.Left - page.Margins.Right);
                    double pageHeight = page.Unit.ConvertToHInches(page.PageHeight - page.Margins.Top - page.Margins.Bottom);
                    double mgLeft = page.Unit.ConvertToHInches(page.Margins.Left);
                    double mgTop = page.Unit.ConvertToHInches(page.Margins.Top);

                    writer.WriteRaw("<p:sp><p:nvSpPr><p:cNvPr id=\"67\" name=\"TextBox Additional\"/><p:cNvSpPr txBox=\"1\"/><p:nvPr userDrawn=\"1\"/></p:nvSpPr>" +
                        "<p:spPr><a:xfrm rot=\"-2700000\"><a:off x=\"" + ConvertToEmu(mgLeft) + "\" y=\"" + ConvertToEmu(mgTop + pageHeight * 0.45) + "\"/><a:ext cx=\"" + ConvertToEmu(pageWidth) + "\" cy=\"" + ConvertToEmu(pageHeight * 0.1) + "\"/></a:xfrm>" +
                        "<a:prstGeom prst=\"rect\"><a:avLst/></a:prstGeom><a:noFill/></p:spPr>" +
                        "<p:txBody><a:bodyPr wrap=\"square\" rtlCol=\"0\" anchor=\"ctr\" anchorCtr=\"0\"><a:spAutoFit/></a:bodyPr><a:lstStyle/><a:p><a:pPr algn=\"ctr\"/><a:r><a:rPr lang=\"en-US\" sz=\"9600\" b=\"1\" dirty=\"0\" smtClean=\"0\">" +
                        "<a:solidFill><a:schemeClr val=\"tx1\"><a:alpha val=\"12000\"/></a:schemeClr></a:solidFill>" +
                        "<a:latin typeface=\"Arial\" panose=\"020B0604020202020204\" pitchFamily=\"34\" charset=\"0\"/><a:cs typeface=\"Arial\" panose=\"020B0604020202020204\" pitchFamily=\"34\" charset=\"0\"/>" +
                        "</a:rPr><a:t>Trial</a:t></a:r></a:p></p:txBody></p:sp>");
                }
                #endregion

                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                #endregion
            }
            if (index == 8)
            {
                #region Layout8
                writer.WriteAttributeString("type", "objTx");
                writer.WriteAttributeString("preserve", "1");

                writer.WriteStartElement("p:cSld");
                writer.WriteAttributeString("name", "Content with Caption");
                writer.WriteStartElement("p:spTree");
                writer.WriteStartElement("p:nvGrpSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "1");
                writer.WriteAttributeString("name", "");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvGrpSpPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:grpSpPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chOff");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chExt");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "2");
                writer.WriteAttributeString("name", "Title 1");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "title");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "457200");
                writer.WriteAttributeString("y", "273050");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "3008313");
                writer.WriteAttributeString("cy", "1162050");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteAttributeString("anchor", "b");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteAttributeString("algn", "l");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master title style");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "3");
                writer.WriteAttributeString("name", "Content Placeholder 2");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("idx", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "3575050");
                writer.WriteAttributeString("y", "273050");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "5111750");
                writer.WriteAttributeString("cy", "5853113");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "3200");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2400");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "1");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Second level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "2");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Third level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "3");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fourth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "4");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fifth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "4");
                writer.WriteAttributeString("name", "Text Placeholder 3");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "body");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "2");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "457200");
                writer.WriteAttributeString("y", "1435100");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "3008313");
                writer.WriteAttributeString("cy", "4691063");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteAttributeString("marL", "0");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1400");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteAttributeString("marL", "457200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1200");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteAttributeString("marL", "914400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteAttributeString("marL", "1371600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteAttributeString("marL", "1828800");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteAttributeString("marL", "2286000");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteAttributeString("marL", "2743200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteAttributeString("marL", "3200400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteAttributeString("marL", "3657600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "5");
                writer.WriteAttributeString("name", "Date Placeholder 4");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "dt");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "10");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
                writer.WriteAttributeString("type", "datetimeFigureOut");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("15.04.2009");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "6");
                writer.WriteAttributeString("name", "Footer Placeholder 5");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ftr");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "11");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "7");
                writer.WriteAttributeString("name", "Slide Number Placeholder 6");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "sldNum");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "12");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
                writer.WriteAttributeString("type", "slidenum");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("‹#›");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                #endregion
            }
            if (index == 9)
            {
                #region Layout9
                writer.WriteAttributeString("type", "picTx");
                writer.WriteAttributeString("preserve", "1");

                writer.WriteStartElement("p:cSld");
                writer.WriteAttributeString("name", "Picture with Caption");
                writer.WriteStartElement("p:spTree");
                writer.WriteStartElement("p:nvGrpSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "1");
                writer.WriteAttributeString("name", "");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvGrpSpPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:grpSpPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chOff");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chExt");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "2");
                writer.WriteAttributeString("name", "Title 1");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "title");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "1792288");
                writer.WriteAttributeString("y", "4800600");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "5486400");
                writer.WriteAttributeString("cy", "566738");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteAttributeString("anchor", "b");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteAttributeString("algn", "l");
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteAttributeString("b", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master title style");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "3");
                writer.WriteAttributeString("name", "Picture Placeholder 2");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "pic");
                writer.WriteAttributeString("idx", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "1792288");
                writer.WriteAttributeString("y", "612775");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "5486400");
                writer.WriteAttributeString("cy", "4114800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteAttributeString("marL", "0");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "3200");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteAttributeString("marL", "457200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2800");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteAttributeString("marL", "914400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2400");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteAttributeString("marL", "1371600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteAttributeString("marL", "1828800");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteAttributeString("marL", "2286000");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteAttributeString("marL", "2743200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteAttributeString("marL", "3200400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteAttributeString("marL", "3657600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "2000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "4");
                writer.WriteAttributeString("name", "Text Placeholder 3");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "body");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "2");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "1792288");
                writer.WriteAttributeString("y", "5367338");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "5486400");
                writer.WriteAttributeString("cy", "804862");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteStartElement("a:lvl1pPr");
                writer.WriteAttributeString("marL", "0");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1400");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl2pPr");
                writer.WriteAttributeString("marL", "457200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1200");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl3pPr");
                writer.WriteAttributeString("marL", "914400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "1000");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl4pPr");
                writer.WriteAttributeString("marL", "1371600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl5pPr");
                writer.WriteAttributeString("marL", "1828800");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl6pPr");
                writer.WriteAttributeString("marL", "2286000");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl7pPr");
                writer.WriteAttributeString("marL", "2743200");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl8pPr");
                writer.WriteAttributeString("marL", "3200400");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:lvl9pPr");
                writer.WriteAttributeString("marL", "3657600");
                writer.WriteAttributeString("indent", "0");
                writer.WriteStartElement("a:buNone");
                writer.WriteEndElement();
                writer.WriteStartElement("a:defRPr");
                writer.WriteAttributeString("sz", "900");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "5");
                writer.WriteAttributeString("name", "Date Placeholder 4");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "dt");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "10");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
                writer.WriteAttributeString("type", "datetimeFigureOut");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("15.04.2009");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "6");
                writer.WriteAttributeString("name", "Footer Placeholder 5");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ftr");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "11");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "7");
                writer.WriteAttributeString("name", "Slide Number Placeholder 6");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "sldNum");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "12");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
                writer.WriteAttributeString("type", "slidenum");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("‹#›");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                #endregion
            }
            if (index == 10)
            {
                #region Layout10
                writer.WriteAttributeString("type", "vertTx");
                writer.WriteAttributeString("preserve", "1");

                writer.WriteStartElement("p:cSld");
                writer.WriteAttributeString("name", "Title and Vertical Text");
                writer.WriteStartElement("p:spTree");
                writer.WriteStartElement("p:nvGrpSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "1");
                writer.WriteAttributeString("name", "");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvGrpSpPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:grpSpPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chOff");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chExt");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "2");
                writer.WriteAttributeString("name", "Title 1");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "title");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master title style");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "3");
                writer.WriteAttributeString("name", "Vertical Text Placeholder 2");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "body");
                writer.WriteAttributeString("orient", "vert");
                writer.WriteAttributeString("idx", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteAttributeString("vert", "eaVert");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "1");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Second level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "2");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Third level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "3");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fourth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "4");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fifth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "4");
                writer.WriteAttributeString("name", "Date Placeholder 3");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "dt");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "10");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
                writer.WriteAttributeString("type", "datetimeFigureOut");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("15.04.2009");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "5");
                writer.WriteAttributeString("name", "Footer Placeholder 4");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ftr");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "11");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "6");
                writer.WriteAttributeString("name", "Slide Number Placeholder 5");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "sldNum");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "12");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
                writer.WriteAttributeString("type", "slidenum");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("‹#›");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                #endregion
            }
            if (index == 11)
            {
                #region Layout11
                writer.WriteAttributeString("type", "vertTitleAndTx");
                writer.WriteAttributeString("preserve", "1");

                writer.WriteStartElement("p:cSld");
                writer.WriteAttributeString("name", "Vertical Title and Text");
                writer.WriteStartElement("p:spTree");
                writer.WriteStartElement("p:nvGrpSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "1");
                writer.WriteAttributeString("name", "");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvGrpSpPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:grpSpPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chOff");
                writer.WriteAttributeString("x", "0");
                writer.WriteAttributeString("y", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:chExt");
                writer.WriteAttributeString("cx", "0");
                writer.WriteAttributeString("cy", "0");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "2");
                writer.WriteAttributeString("name", "Vertical Title 1");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "title");
                writer.WriteAttributeString("orient", "vert");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "6629400");
                writer.WriteAttributeString("y", "274638");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "2057400");
                writer.WriteAttributeString("cy", "5851525");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteAttributeString("vert", "eaVert");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master title style");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "3");
                writer.WriteAttributeString("name", "Vertical Text Placeholder 2");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "body");
                writer.WriteAttributeString("orient", "vert");
                writer.WriteAttributeString("idx", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "457200");
                writer.WriteAttributeString("y", "274638");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "6019800");
                writer.WriteAttributeString("cy", "5851525");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteAttributeString("vert", "eaVert");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Click to edit Master text styles");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "1");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Second level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "2");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Third level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "3");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fourth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:pPr");
                writer.WriteAttributeString("lvl", "4");
                writer.WriteEndElement();
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("Fifth level");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "4");
                writer.WriteAttributeString("name", "Date Placeholder 3");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "dt");
                writer.WriteAttributeString("sz", "half");
                writer.WriteAttributeString("idx", "10");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{BBCD0B08-7874-4091-A11F-B0CCB138CA98}");
                writer.WriteAttributeString("type", "datetimeFigureOut");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("15.04.2009");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "5");
                writer.WriteAttributeString("name", "Footer Placeholder 4");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "ftr");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "11");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:sp");
                writer.WriteStartElement("p:nvSpPr");
                writer.WriteStartElement("p:cNvPr");
                writer.WriteAttributeString("id", "6");
                writer.WriteAttributeString("name", "Slide Number Placeholder 5");
                writer.WriteEndElement();
                writer.WriteStartElement("p:cNvSpPr");
                writer.WriteStartElement("a:spLocks");
                writer.WriteAttributeString("noGrp", "1");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:nvPr");
                writer.WriteStartElement("p:ph");
                writer.WriteAttributeString("type", "sldNum");
                writer.WriteAttributeString("sz", "quarter");
                writer.WriteAttributeString("idx", "12");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("p:spPr");
                writer.WriteEndElement();
                writer.WriteStartElement("p:txBody");
                writer.WriteStartElement("a:bodyPr");
                writer.WriteEndElement();
                writer.WriteStartElement("a:lstStyle");
                writer.WriteEndElement();
                writer.WriteStartElement("a:p");
                writer.WriteStartElement("a:fld");
                writer.WriteAttributeString("id", "{F548980B-80A5-4AC9-8F15-D7D0FD0AEED9}");
                writer.WriteAttributeString("type", "slidenum");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteAttributeString("smtClean", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:t");
                writer.WriteString("‹#›");
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:endParaRPr");
                writer.WriteAttributeString("lang", ReportCulture);
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
                #endregion
            }

            writer.WriteStartElement("p:clrMapOvr");
            writer.WriteStartElement("a:masterClrMapping");
            writer.WriteEndElement();
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WritePresentationRels
        private MemoryStream WritePresentationRels(StiPagesCollection allPages)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId1");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/tableStyles");
            writer.WriteAttributeString("Target", "tableStyles.xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId2");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/viewProps");
            writer.WriteAttributeString("Target", "viewProps.xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId3");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/presProps");
            writer.WriteAttributeString("Target", "presProps.xml");
            writer.WriteEndElement();

            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId4");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme");
            writer.WriteAttributeString("Target", "theme/theme1.xml");
            writer.WriteEndElement();

            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId5");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideMaster");
            writer.WriteAttributeString("Target", "slideMasters/slideMaster1.xml");
            writer.WriteEndElement();

            for (int index = 0; index < allPages.Count; index++)
            {
                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", string.Format("rId{0}", 6 + index));
                writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide");
                writer.WriteAttributeString("Target", string.Format("slides/slide{0}.xml", index + 1));
                writer.WriteEndElement();
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WritePresentation
        private MemoryStream WritePresentation(StiPagesCollection allPages)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("p:presentation");
            writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:p", "http://schemas.openxmlformats.org/presentationml/2006/main");
            writer.WriteAttributeString("saveSubsetFonts", "1");

            writer.WriteStartElement("p:sldMasterIdLst");
            writer.WriteStartElement("p:sldMasterId");
            writer.WriteAttributeString("id", "2147483648");
            writer.WriteAttributeString("r:id", "rId5");
            writer.WriteEndElement();
            writer.WriteFullEndElement();

            writer.WriteStartElement("p:sldIdLst");
            for (int index = 0; index < allPages.Count; index++)
            {
                writer.WriteStartElement("p:sldId");
                writer.WriteAttributeString("id", string.Format("{0}", 256 + index));
                writer.WriteAttributeString("r:id", string.Format("rId{0}", 6 + index));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            var page = allPages[0];
            writer.WriteStartElement("p:sldSz");
            writer.WriteAttributeString("cx", ConvertToEmu(page.Unit.ConvertToHInches(page.Width + page.Margins.Left + page.Margins.Right)).ToString());
            writer.WriteAttributeString("cy", ConvertToEmu(page.Unit.ConvertToHInches(page.Height + page.Margins.Top + page.Margins.Bottom)).ToString());
            //writer.WriteAttributeString("type", "screen4x3");     //if not written - custom size is selected
            writer.WriteEndElement();

            writer.WriteStartElement("p:notesSz");
            writer.WriteAttributeString("cx", "6858000");
            writer.WriteAttributeString("cy", "9144000");
            writer.WriteEndElement();

            #region p:defaultTextStyle
            writer.WriteStartElement("p:defaultTextStyle");

            writer.WriteStartElement("a:defPPr");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("lang", ReportCulture);
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl1pPr");
            writer.WriteAttributeString("marL", "0");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl2pPr");
            writer.WriteAttributeString("marL", "457200");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl3pPr");
            writer.WriteAttributeString("marL", "914400");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl4pPr");
            writer.WriteAttributeString("marL", "1371600");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl5pPr");
            writer.WriteAttributeString("marL", "1828800");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl6pPr");
            writer.WriteAttributeString("marL", "2286000");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl7pPr");
            writer.WriteAttributeString("marL", "2743200");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl8pPr");
            writer.WriteAttributeString("marL", "3200400");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:lvl9pPr");
            writer.WriteAttributeString("marL", "3657600");
            writer.WriteAttributeString("algn", "l");
            writer.WriteAttributeString("defTabSz", "914400");
            writer.WriteAttributeString("rtl", "0");
            writer.WriteAttributeString("eaLnBrk", "1");
            writer.WriteAttributeString("latinLnBrk", "0");
            writer.WriteAttributeString("hangingPunct", "1");
            writer.WriteStartElement("a:defRPr");
            writer.WriteAttributeString("sz", "1800");
            writer.WriteAttributeString("kern", "1200");
            writer.WriteStartElement("a:solidFill");
            writer.WriteStartElement("a:schemeClr");
            writer.WriteAttributeString("val", "tx1");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:latin");
            writer.WriteAttributeString("typeface", "+mn-lt");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ea");
            writer.WriteAttributeString("typeface", "+mn-ea");
            writer.WriteEndElement();
            writer.WriteStartElement("a:cs");
            writer.WriteAttributeString("typeface", "+mn-cs");
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();

            writer.WriteEndElement();
            #endregion

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteSlideRels
        private MemoryStream WriteSlideRels(int indexSheet)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId1");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideLayout");
            writer.WriteAttributeString("Target", "../slideLayouts/slideLayout7.xml");
            writer.WriteEndElement();

            if (imageCache.ImageIndex.Count > imageListOffset)
            {
                for (int index = 0; index < imageCache.ImageIndex.Count - imageListOffset; index++)
                {
                    writer.WriteStartElement("Relationship");
                    writer.WriteAttributeString("Id", string.Format("rId{0}", 2 + index));
                    writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image");
                    writer.WriteAttributeString("Target", string.Format("../media/image{0:D5}.{1}", (int)imageCache.ImageIndex[imageListOffset + index] + 1,
                        StiOptions.Export.PowerPoint.StoreImagesAsPng ? "png" : "jpeg"));
                    writer.WriteEndElement();
                }
            }

            if (hyperlinkList.Count > 0)
            {
                for (int index = 0; index < hyperlinkList.Count; index++)
                {
                    writer.WriteStartElement("Relationship");
                    writer.WriteAttributeString("Id", string.Format("hId{0}", index + 1));
                    object link = hyperlinkList[index];
                    if (link is string)
                    {
                        writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink");
                        writer.WriteAttributeString("Target", (string)link);
                        writer.WriteAttributeString("TargetMode", "External");
                    }
                    if (link is int)
                    {
                        writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide");
                        writer.WriteAttributeString("Target", string.Format("slide{0}.xml", (int)link));
                    }
                    writer.WriteEndElement();
                }
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteSlide
        private MemoryStream WriteSlide(int indexPage, StiPagesCollection allPages)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("p:sld");
            writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:p", "http://schemas.openxmlformats.org/presentationml/2006/main");
            writer.WriteStartElement("p:cSld");

            var page = allPages[indexPage];
            allPages.GetPage(page);

            Color color = StiBrush.ToColor(page.Brush);
            if (StiOptions.Export.PowerPoint.ReplaceTransparentPageBackground && color.Equals(Color.Transparent))
            {
                color = Color.White;
            }
            if (!color.Equals(Color.White))
            {
                writer.WriteStartElement("p:bg");
                writer.WriteStartElement("p:bgPr");
                writer.WriteStartElement("a:solidFill");
                WriteColor(writer, color);
                writer.WriteFullEndElement();
                writer.WriteStartElement("a:effectLst");
                writer.WriteEndElement();
                writer.WriteFullEndElement();
                writer.WriteFullEndElement();
            }

            writer.WriteStartElement("p:spTree");

            #region p:nvGrpSpPr
            writer.WriteStartElement("p:nvGrpSpPr");
            writer.WriteStartElement("p:cNvPr");
            writer.WriteAttributeString("id", "1");
            writer.WriteAttributeString("name", "");
            writer.WriteEndElement();
            writer.WriteStartElement("p:cNvGrpSpPr");
            writer.WriteEndElement();
            writer.WriteStartElement("p:nvPr");
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            #region p:grpSpPr
            writer.WriteStartElement("p:grpSpPr");
            writer.WriteStartElement("a:xfrm");
            writer.WriteStartElement("a:off");
            writer.WriteAttributeString("x", "0");
            writer.WriteAttributeString("y", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:ext");
            writer.WriteAttributeString("cx", "0");
            writer.WriteAttributeString("cy", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:chOff");
            writer.WriteAttributeString("x", "0");
            writer.WriteAttributeString("y", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("a:chExt");
            writer.WriteAttributeString("cx", "0");
            writer.WriteAttributeString("cy", "0");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            idCounter = 2;

            WriteWatermark(writer, page, true);

            #region Process ExceedMargins
            foreach (StiComponent comp in page.Components)
            {
                if (comp.Enabled && comp.Width > 0 && comp.Height > 0)
                {
                    var textBox = comp as StiText;
                    if ((textBox != null) && (textBox.ExceedMargins != StiExceedMargins.None))
                    {
                        WriteExceedMargins(writer, textBox);
                    }
                }
            }
            #endregion

            foreach (StiComponent comp in page.Components)
            {
                if (comp.Enabled && comp.Width > 0 && comp.Height > 0)
                {
                    var textBox = comp as StiText;
                    if (textBox != null && !textBox.CheckAllowHtmlTags())
                    {
                        //textbox
                        float angle = textBox.Angle % 360;
                        if (angle < 0) angle = 360 + angle;

                        if (angle == 0 || angle == 90 || angle == 270)
                        {
                            WriteStiTextbox(writer, comp);
                        }
                        else
                        {
                            var tempText = textBox.Clone() as StiText;
                            tempText.Text = null;
                            tempText.Angle = 0;
                            WriteStiTextbox(writer, tempText);

                            tempText = textBox.Clone() as StiText;
                            tempText.Border = null;
                            tempText.Brush = null;
                            tempText.HorAlignment = StiTextHorAlignment.Center;
                            tempText.VertAlignment = StiVertAlignment.Center;
                            WriteStiTextbox(writer, tempText);
                        }
                    }
                    else
                    {
                        if (comp is StiContainer)
                        {
                            var newText = new StiText(comp.ClientRectangle);
                            newText.Page = comp.Page;
                            newText.Border = (comp as StiContainer).Border;
                            newText.Brush = (comp as StiContainer).Brush;
                            newText.HyperlinkValue = comp.HyperlinkValue;
                            WriteStiTextbox(writer, newText);
                        }
                        else
                        {
                            if (comp is StiRectanglePrimitive)
                            {
                                WriteRoundRectangle(writer, comp);
                            }
                            else
                            {
                                //image
                                WriteStiImage(writer, comp);
                            }
                        }
                    }

                    idCounter++;
                }
            }

            WriteWatermark(writer, page, false);

            #region Trial
#if SERVER
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
                double pageWidth = page.Unit.ConvertToHInches(page.PageWidth - page.Margins.Left - page.Margins.Right);
                double pageHeight = page.Unit.ConvertToHInches(page.PageHeight - page.Margins.Top - page.Margins.Bottom);
                double mgLeft = page.Unit.ConvertToHInches(page.Margins.Left);
                double mgTop = page.Unit.ConvertToHInches(page.Margins.Top);

                writer.WriteRaw("<p:sp><p:nvSpPr><p:cNvPr id=\"67\" name=\"TextBox Additional\"/><p:cNvSpPr txBox=\"1\"/><p:nvPr userDrawn=\"1\"/></p:nvSpPr>" +
                    "<p:spPr><a:xfrm rot=\"-2700000\"><a:off x=\"" + ConvertToEmu(mgLeft) + "\" y=\"" + ConvertToEmu(mgTop + pageHeight * 0.45) + "\"/><a:ext cx=\"" + ConvertToEmu(pageWidth) + "\" cy=\"" + ConvertToEmu(pageHeight * 0.1) + "\"/></a:xfrm>" +
                    "<a:prstGeom prst=\"rect\"><a:avLst/></a:prstGeom><a:noFill/></p:spPr>" +
                    "<p:txBody><a:bodyPr wrap=\"square\" rtlCol=\"0\" anchor=\"ctr\" anchorCtr=\"0\"><a:spAutoFit/></a:bodyPr><a:lstStyle/><a:p><a:pPr algn=\"ctr\"/><a:r><a:rPr lang=\"en-US\" sz=\"9600\" b=\"1\" dirty=\"0\" smtClean=\"0\">" +
                    "<a:solidFill><a:schemeClr val=\"tx1\"><a:alpha val=\"12000\"/></a:schemeClr></a:solidFill>" +
                    "<a:latin typeface=\"Arial\" panose=\"020B0604020202020204\" pitchFamily=\"34\" charset=\"0\"/><a:cs typeface=\"Arial\" panose=\"020B0604020202020204\" pitchFamily=\"34\" charset=\"0\"/>" +
                    "</a:rPr><a:t>Trial</a:t></a:r></a:p></p:txBody></p:sp>");
            }
            #endregion

            writer.WriteFullEndElement();   //p:spTree
            writer.WriteEndElement();   //p:cSld

            writer.WriteStartElement("p:clrMapOvr");
            writer.WriteStartElement("a:masterClrMapping");
            writer.WriteEndElement();
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();   //p:sld
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private void WriteExceedMargins(XmlWriter writer, StiText text)
        {
            if ((text == null) || (text.ExceedMargins == StiExceedMargins.None) || (text.Brush is StiEmptyBrush)) return;
            var color = StiBrush.ToColor(text.Brush);
            if (color.Equals(Color.Transparent)) return;

            var rect = text.ClientRectangle;
            if ((text.ExceedMargins & StiExceedMargins.Left) > 0)
            {
                rect.Width += rect.Left + text.Page.Margins.Left;
                rect.X = -text.Page.Margins.Left;
            }
            if ((text.ExceedMargins & StiExceedMargins.Top) > 0)
            {
                rect.Height += rect.Top + text.Page.Margins.Top;
                rect.Y = -text.Page.Margins.Top;
            }
            if ((text.ExceedMargins & StiExceedMargins.Right) > 0)
            {
                rect.Width += text.Page.Width + text.Page.Margins.Right - rect.Right;
            }
            if ((text.ExceedMargins & StiExceedMargins.Bottom) > 0)
            {
                rect.Height += text.Page.Height + text.Page.Margins.Bottom - rect.Bottom;
            }

            #region Write rectangle primitive
            writer.WriteStartElement("p:sp");

            writer.WriteStartElement("p:nvSpPr");
            writer.WriteStartElement("p:cNvPr");
            writer.WriteAttributeString("id", string.Format("{0}", idCounter));
            writer.WriteAttributeString("name", string.Format("Rectangle {0}", idCounter));
            writer.WriteEndElement();
            writer.WriteStartElement("p:cNvSpPr");
            writer.WriteEndElement();
            writer.WriteStartElement("p:nvPr");
            writer.WriteEndElement();
            writer.WriteEndElement();

            int x1 = ConvertToEmu(text.Report.Unit.ConvertToHInches(rect.Left + text.Page.Margins.Left));
            int y1 = ConvertToEmu(text.Report.Unit.ConvertToHInches(rect.Top + text.Page.Margins.Top));
            int x2 = ConvertToEmu(text.Report.Unit.ConvertToHInches(rect.Right + text.Page.Margins.Left));
            int y2 = ConvertToEmu(text.Report.Unit.ConvertToHInches(rect.Bottom + text.Page.Margins.Top));

            writer.WriteStartElement("p:spPr");
            writer.WriteStartElement("a:xfrm");
            writer.WriteStartElement("a:off");
            writer.WriteAttributeString("x", string.Format("{0}", x1));
            writer.WriteAttributeString("y", string.Format("{0}", y1));
            writer.WriteEndElement();
            writer.WriteStartElement("a:ext");
            writer.WriteAttributeString("cx", string.Format("{0}", x2 - x1));
            writer.WriteAttributeString("cy", string.Format("{0}", y2 - y1));
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:prstGeom");
            writer.WriteAttributeString("prst", "rect");
            writer.WriteStartElement("a:avLst");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("a:solidFill");
            WriteColor(writer, color);
            writer.WriteFullEndElement();

            writer.WriteEndElement();   //p:spPr
            writer.WriteEndElement();   //p:sp
            #endregion
        }

        private void WriteStiTextbox(XmlWriter writer, StiComponent comp)
        {
            #region Write textbox
            var stiText = comp as StiText;

            writer.WriteStartElement("p:sp");

            writer.WriteStartElement("p:nvSpPr");
            writer.WriteStartElement("p:cNvPr");
            writer.WriteAttributeString("id", string.Format("{0}", idCounter));
            writer.WriteAttributeString("name", string.Format("TextBox {0}", idCounter));
            WriteHyperlinkInfo(writer, comp);
            writer.WriteEndElement();
            writer.WriteStartElement("p:cNvSpPr");
            writer.WriteAttributeString("txBox", "1");
            writer.WriteEndElement();
            writer.WriteStartElement("p:nvPr");
            writer.WriteEndElement();
            writer.WriteEndElement();

            bool topmost = WriteSpPr(writer, comp);

            writer.WriteStartElement("p:txBody");
            writer.WriteStartElement("a:bodyPr");

            bool textWordWrap = false;
            var stTextAngle = string.Empty;
            if (stiText.TextOptions != null)
            {
                textWordWrap = stiText.TextOptions.WordWrap;
                float textAngle = stiText.TextOptions.Angle % 360;
                if (textAngle < 0) textAngle = 360 + textAngle;
                if (textAngle == 90) stTextAngle = "vert270";
                if (textAngle == 270) stTextAngle = "vert";   //in early versions - "vert90"
            }
            if (stTextAngle != string.Empty)
            {
                writer.WriteAttributeString("vert", stTextAngle);
            }

            writer.WriteAttributeString("horzOverflow", "clip");
            writer.WriteAttributeString("vertOverflow", "clip");

            //margins
            int mLeft = ConvertToEmu(stiText.Margins.Left);
            int mRight = ConvertToEmu(stiText.Margins.Right);
            int mTop = ConvertToEmu(stiText.Margins.Top);
            int mBottom = ConvertToEmu(stiText.Margins.Bottom);
            writer.WriteAttributeString("lIns", string.Format("{0}", mLeft));
            writer.WriteAttributeString("tIns", string.Format("{0}", mTop));
            writer.WriteAttributeString("rIns", string.Format("{0}", mRight));
            writer.WriteAttributeString("bIns", string.Format("{0}", mBottom));

            writer.WriteAttributeString("wrap", (textWordWrap ? "square" : "none"));
            writer.WriteAttributeString("rtlCol", "0");

            //vertical justify
            string vertAlign = "t";
            if (stiText.VertAlignment == StiVertAlignment.Center) vertAlign = "ctr";
            if (stiText.VertAlignment == StiVertAlignment.Bottom) vertAlign = "b";
            writer.WriteAttributeString("anchor", vertAlign);

            writer.WriteAttributeString("anchorCtr", "0");
            writer.WriteStartElement("a:noAutofit");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:lstStyle");
            writer.WriteEndElement();
            writer.WriteStartElement("a:p");

            bool rightToLeft = false;
            if (stiText.TextOptions != null) rightToLeft = stiText.TextOptions.RightToLeft;

            writer.WriteStartElement("a:pPr");
            //horizontal justify
            string horAlign = string.Empty;
            if (stiText.HorAlignment == StiTextHorAlignment.Center) horAlign = "ctr";
            if ((stiText.HorAlignment == StiTextHorAlignment.Right && !rightToLeft) ||
                (stiText.HorAlignment == StiTextHorAlignment.Left && rightToLeft)) horAlign = "r";
            if (stiText.HorAlignment == StiTextHorAlignment.Width) horAlign = "just";
            if (horAlign != string.Empty)
            {
                writer.WriteAttributeString("algn", horAlign);
            }
            if (rightToLeft)
            {
                writer.WriteAttributeString("rtl", "1");
            }
            writer.WriteStartElement("a:lnSpc");
            writer.WriteStartElement("a:spcPct");
            writer.WriteAttributeString("val", Math.Round(stiText.LineSpacing * 0.94 * 100000, 0).ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            string st = string.Empty;
            if (stiText.Text != null) st = stiText.Text.ToString();
            if ((stiText.TextQuality == StiTextQuality.Wysiwyg || stiText.HorAlignment == StiTextHorAlignment.Width) && !string.IsNullOrEmpty(st) && st.EndsWith(StiTextRenderer.StiForceWidthAlignTag))
            {
                st = st.Substring(0, st.Length - StiTextRenderer.StiForceWidthAlignTag.Length);
            }

            if (!string.IsNullOrEmpty(st))
            {
                writer.WriteStartElement("a:r");
                writer.WriteStartElement("a:rPr");
                writer.WriteAttributeString("lang", ReportCulture);
                if (reportCulture == null) writer.WriteAttributeString("noProof", "1");

                int fSize = (int)(stiText.Font.SizeInPoints * 100 * 0.984);
                if (fSize < 100) fSize = 100;
                writer.WriteAttributeString("sz", fSize.ToString());

                if (stiText.Font.Bold) writer.WriteAttributeString("b", "1");
                if (stiText.Font.Italic) writer.WriteAttributeString("i", "1");
                if (stiText.Font.Underline) writer.WriteAttributeString("u", "sng");
                if (stiText.Font.Strikeout) writer.WriteAttributeString("strike", "sngStrike");

                writer.WriteAttributeString("dirty", "0");
                writer.WriteAttributeString("smtClean", "0");

                writer.WriteStartElement("a:solidFill");
                WriteColor(writer, StiBrush.ToColor(stiText.TextBrush));
                writer.WriteFullEndElement();

                writer.WriteStartElement("a:latin");
                writer.WriteAttributeString("typeface", stiText.Font.Name);
                writer.WriteAttributeString("pitchFamily", "18");
                writer.WriteAttributeString("charset", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("a:cs");
                writer.WriteAttributeString("typeface", stiText.Font.Name);
                writer.WriteAttributeString("pitchFamily", "18");
                writer.WriteAttributeString("charset", "0");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteElementString("a:t", st);
                writer.WriteEndElement();   //a:r
            }
            else
            {
                writer.WriteStartElement("a:endParaRPr");

                int fSize = (int)(stiText.Font.SizeInPoints * 100 * 0.984);
                if (fSize < 100) fSize = 100;
                writer.WriteAttributeString("sz", fSize.ToString());
                writer.WriteAttributeString("dirty", "0");

                writer.WriteStartElement("a:latin");
                writer.WriteAttributeString("typeface", stiText.Font.Name);
                writer.WriteAttributeString("pitchFamily", "18");
                writer.WriteAttributeString("charset", "0");
                writer.WriteEndElement();

                writer.WriteEndElement();   //a:endParaRPr
            }

            writer.WriteEndElement();   //a:p
            writer.WriteEndElement();   //p:txBody

            writer.WriteEndElement();   //p:sp
            #endregion

            if (topmost) WriteBorder(writer, comp);
        }

        private void WriteStiImage(XmlWriter writer, StiComponent comp)
        {
            #region Write image
            bool topmost = false;

            var exportImage = comp as IStiExportImage;
            if (exportImage != null)
            {
                var exportImageExtended = exportImage as IStiExportImageExtended;
                float zoom = imageResolution;

                Image image = null;
                if (comp.IsExportAsImage(StiExportFormat.Ppt2007))
                {
                    try
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture;
 
                        if (exportImageExtended != null && exportImageExtended.IsExportAsImage(StiExportFormat.Ppt2007))
                            image = exportImageExtended.GetImage(ref zoom, StiExportFormat.ImagePng);
                        else image = exportImage.GetImage(ref zoom);
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = newCulture;
                    }
                }

                if (image != null)
                {
                    int imageIndex = imageCache.AddImageInt(image);
                    image.Dispose();

                    #region Write image info
                    writer.WriteStartElement("p:pic");
                    writer.WriteStartElement("p:nvPicPr");
                    writer.WriteStartElement("p:cNvPr");
                    writer.WriteAttributeString("id", string.Format("{0}", idCounter));
                    writer.WriteAttributeString("name", string.Format("Picture {0}", idCounter + 1));
                    writer.WriteAttributeString("descr", string.Format("Picture {0} description", idCounter + 1));
                    WriteHyperlinkInfo(writer, comp);
                    writer.WriteEndElement();
                    writer.WriteStartElement("p:cNvPicPr");
                    writer.WriteStartElement("a:picLocks");
                    writer.WriteAttributeString("noChangeAspect", "1");
                    writer.WriteEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteStartElement("p:nvPr");
                    writer.WriteEndElement();
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("p:blipFill");
                    writer.WriteStartElement("a:blip");
                    writer.WriteAttributeString("r:embed", string.Format("rId{0}", 1 + imageCache.ImageIndex.Count - imageListOffset));
                    writer.WriteEndElement();
                    writer.WriteStartElement("a:stretch");
                    writer.WriteStartElement("a:fillRect");
                    writer.WriteEndElement();
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();

                    topmost = WriteSpPr(writer, comp);

                    writer.WriteFullEndElement();
                    #endregion
                }
            }

            #endregion

            if (topmost || (comp is StiLinePrimitive)) WriteBorder(writer, comp);
        }

        private bool WriteSpPr(XmlWriter writer, StiComponent comp)
        {
			int x1 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(comp.Left + comp.Page.Margins.Left));
            int y1 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(comp.Top + comp.Page.Margins.Top));
            int x2 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(comp.Right + comp.Page.Margins.Left));
            int y2 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(comp.Bottom + comp.Page.Margins.Top));

            float angle = 0;
            var txtOpt = comp as IStiTextOptions;
            if (txtOpt != null && txtOpt.TextOptions != null)
            {
                angle = txtOpt.TextOptions.Angle % 360;
                if (angle == 90 || angle == 270 || angle == -90 || angle == -270) angle = 0;
            }

            writer.WriteStartElement("p:spPr");
            writer.WriteStartElement("a:xfrm");
            if (angle != 0)
            {
                writer.WriteAttributeString("rot", string.Format("{0}", (int)Math.Round((decimal)(-angle * 60000))));
            }
            writer.WriteStartElement("a:off");
            writer.WriteAttributeString("x", string.Format("{0}", x1));
            writer.WriteAttributeString("y", string.Format("{0}", y1));
            writer.WriteEndElement();
            writer.WriteStartElement("a:ext");
            writer.WriteAttributeString("cx", string.Format("{0}", x2 - x1));
            writer.WriteAttributeString("cy", string.Format("{0}", y2 - y1));
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:prstGeom");
            writer.WriteAttributeString("prst", "rect");
            writer.WriteStartElement("a:avLst");
            writer.WriteEndElement();
            writer.WriteEndElement();

            bool noNeedFill = (comp is StiShape) || (comp is StiImage);

            var color = Color.Transparent;
            if (comp is IStiBrush)
            {
                color = StiBrush.ToColor((comp as IStiBrush).Brush);
            }
            if ((color == Color.Transparent) || noNeedFill)
            {
                writer.WriteStartElement("a:noFill");
                writer.WriteEndElement();
            }
            else
            {
                writer.WriteStartElement("a:solidFill");
                WriteColor(writer, color);
                writer.WriteFullEndElement();
            }

            bool topmost = false;
            StiBorder border = null;
            if (comp is IStiBorder)
            {
                border = (comp as IStiBorder).Border;
            }
            if ((border != null) && (border.Side != StiBorderSides.None) && (border.Style != StiPenStyle.None))
            {
                if (border.Side != StiBorderSides.All || border is StiAdvancedBorder)
                {
                    topmost = true;
                }
                else
                {
                    writer.WriteStartElement("a:ln");
                    writer.WriteAttributeString("w", string.Format("{0}", ConvertToEmu(border.Size)));
                    if (border.Style == StiPenStyle.Double)
                    {
                        writer.WriteAttributeString("cmpd", "dbl");
                    }
                    writer.WriteStartElement("a:solidFill");
                    WriteColor(writer, border.Color);
                    writer.WriteFullEndElement();
                    writer.WriteStartElement("a:prstDash");     //page 3755 in reference
                    writer.WriteAttributeString("val", GetLineStyle(border.Style));
                    writer.WriteEndElement();
                    writer.WriteStartElement("a:miter");
                    writer.WriteAttributeString("lim", "800000");
                    writer.WriteEndElement();
                    writer.WriteFullEndElement();
                }
            }
            writer.WriteEndElement();

            return topmost;
        }

        private void WriteBorder(XmlWriter writer, StiComponent comp)
        {
            var border = (comp as IStiBorder).Border;
            if (border is StiAdvancedBorder)
            {
                var adv = border as StiAdvancedBorder;
                if (adv.IsLeftBorderSidePresent) 
                    WriteLine(writer, comp, new RectangleD(comp.Left, comp.Top, 0, comp.Height), adv.LeftSide.Size, adv.LeftSide.Style, adv.LeftSide.Color);
                if (adv.IsRightBorderSidePresent)
                    WriteLine(writer, comp, new RectangleD(comp.Right, comp.Top, 0, comp.Height), adv.RightSide.Size, adv.RightSide.Style, adv.RightSide.Color);
                if (adv.IsTopBorderSidePresent)
                    WriteLine(writer, comp, new RectangleD(comp.Left, comp.Top, comp.Width, 0), adv.TopSide.Size, adv.TopSide.Style, adv.TopSide.Color);
                if (adv.IsBottomBorderSidePresent)
                    WriteLine(writer, comp, new RectangleD(comp.Left, comp.Bottom, comp.Width, 0), adv.BottomSide.Size, adv.BottomSide.Style, adv.BottomSide.Color);
            }
            else
            {
                if (border.IsLeftBorderSidePresent)
                    WriteLine(writer, comp, new RectangleD(comp.Left, comp.Top, 0, comp.Height), border.Size, border.Style, border.Color);
                if (border.IsRightBorderSidePresent)
                    WriteLine(writer, comp, new RectangleD(comp.Right, comp.Top, 0, comp.Height), border.Size, border.Style, border.Color);
                if (border.IsTopBorderSidePresent)
                    WriteLine(writer, comp, new RectangleD(comp.Left, comp.Top, comp.Width, 0), border.Size, border.Style, border.Color);
                if (border.IsBottomBorderSidePresent)
                    WriteLine(writer, comp, new RectangleD(comp.Left, comp.Bottom, comp.Width, 0), border.Size, border.Style, border.Color);
            }
        }

        private void WriteLine(XmlWriter writer, StiComponent comp, RectangleD rect, double size, StiPenStyle style, Color color)
        {
            if (style == StiPenStyle.None || color == Color.Transparent) return;

            #region Write line
            writer.WriteStartElement("p:cxnSp");

            writer.WriteStartElement("p:nvCxnSpPr");
            writer.WriteStartElement("p:cNvPr");
            writer.WriteAttributeString("id", string.Format("{0}", idCounter));
            writer.WriteAttributeString("name", string.Format("Line {0}", idCounter));
            writer.WriteEndElement();
            writer.WriteStartElement("p:cNvCxnSpPr");
            writer.WriteEndElement();
            writer.WriteStartElement("p:nvPr");
            writer.WriteEndElement();
            writer.WriteEndElement();

            int x1 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(rect.Left + comp.Page.Margins.Left));
            int y1 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(rect.Top + comp.Page.Margins.Top));
            int x2 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(rect.Right + comp.Page.Margins.Left));
            int y2 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(rect.Bottom + comp.Page.Margins.Top));

            writer.WriteStartElement("p:spPr");
            writer.WriteStartElement("a:xfrm");
            writer.WriteStartElement("a:off");
            writer.WriteAttributeString("x", string.Format("{0}", x1));
            writer.WriteAttributeString("y", string.Format("{0}", y1));
            writer.WriteEndElement();
            writer.WriteStartElement("a:ext");
            writer.WriteAttributeString("cx", string.Format("{0}", x2 - x1));
            writer.WriteAttributeString("cy", string.Format("{0}", y2 - y1));
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:prstGeom");
            writer.WriteAttributeString("prst", "line");
            writer.WriteStartElement("a:avLst");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("a:ln");
            writer.WriteAttributeString("w", string.Format("{0}", ConvertToEmu(size)));
            if (style == StiPenStyle.Double)
            {
                writer.WriteAttributeString("cmpd", "dbl");
            }
            writer.WriteStartElement("a:solidFill");
            WriteColor(writer, color);
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:prstDash");     //page 3755 in reference
            writer.WriteAttributeString("val", GetLineStyle(style));
            writer.WriteEndElement();
            //writer.WriteStartElement("a:miter");
            //writer.WriteAttributeString("lim", "800000");
            //writer.WriteEndElement();

            #region Caps
            var linePrimitive = comp as StiLinePrimitive;
            if (linePrimitive != null)
            {
                StiCap cap1 = null;
                StiCap cap2 = null;
                if (linePrimitive is StiHorizontalLinePrimitive)
                {
                    cap1 = (linePrimitive as StiHorizontalLinePrimitive).StartCap;
                    cap2 = (linePrimitive as StiHorizontalLinePrimitive).EndCap;
                }
                if (linePrimitive is StiVerticalLinePrimitive)
                {
                    cap1 = (linePrimitive as StiVerticalLinePrimitive).StartCap;
                    cap2 = (linePrimitive as StiVerticalLinePrimitive).EndCap;
                }
                if (cap1 != null)
                {
                    var capStyle = CapStyleToPptStyle(cap1.Style);
                    if (!string.IsNullOrEmpty(capStyle))
                    {
                        writer.WriteStartElement("a:headEnd");
                        writer.WriteAttributeString("type", capStyle);
                        writer.WriteEndElement();
                    }
                }
                if (cap2 != null)
                {
                    var capStyle = CapStyleToPptStyle(cap2.Style);
                    if (!string.IsNullOrEmpty(capStyle))
                    {
                        writer.WriteStartElement("a:tailEnd");
                        writer.WriteAttributeString("type", capStyle);
                        writer.WriteEndElement();
                    }
                }
            }
            #endregion

            writer.WriteFullEndElement();   //a:ln

            writer.WriteEndElement();   //p:spPr

            writer.WriteEndElement();   //p:cxnSp
            #endregion
        }

        private void WriteRoundRectangle(XmlWriter writer, StiComponent comp)
        {
            StiRectanglePrimitive prim = comp as StiRectanglePrimitive;
            if (prim == null) return;
            if (prim.Style == StiPenStyle.None || prim.Color.Equals(Color.Transparent)) return;

            StiRoundedRectanglePrimitive primRound = comp as StiRoundedRectanglePrimitive;
            var rect = comp.ClientRectangle;

            #region Write rectangle primitive
            writer.WriteStartElement("p:sp");

            writer.WriteStartElement("p:nvSpPr");
            writer.WriteStartElement("p:cNvPr");
            writer.WriteAttributeString("id", string.Format("{0}", idCounter));
            writer.WriteAttributeString("name", string.Format("{0}Rectangle {1}", (primRound != null ? "Round" : ""), idCounter));
            writer.WriteEndElement();
            writer.WriteStartElement("p:cNvSpPr");
            writer.WriteEndElement();
            writer.WriteStartElement("p:nvPr");
            writer.WriteEndElement();
            writer.WriteEndElement();

            int x1 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(rect.Left + comp.Page.Margins.Left));
            int y1 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(rect.Top + comp.Page.Margins.Top));
            int x2 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(rect.Right + comp.Page.Margins.Left));
            int y2 = ConvertToEmu(comp.Report.Unit.ConvertToHInches(rect.Bottom + comp.Page.Margins.Top));

            writer.WriteStartElement("p:spPr");
            writer.WriteStartElement("a:xfrm");
            writer.WriteStartElement("a:off");
            writer.WriteAttributeString("x", string.Format("{0}", x1));
            writer.WriteAttributeString("y", string.Format("{0}", y1));
            writer.WriteEndElement();
            writer.WriteStartElement("a:ext");
            writer.WriteAttributeString("cx", string.Format("{0}", x2 - x1));
            writer.WriteAttributeString("cy", string.Format("{0}", y2 - y1));
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("a:prstGeom");
            writer.WriteAttributeString("prst", primRound != null ? "roundRect" : "rect");
            writer.WriteStartElement("a:avLst");
            if (primRound != null)
            {
                double nWidth = comp.Report.Unit.ConvertToHInches(rect.Width);
                double nHeight = comp.Report.Unit.ConvertToHInches(rect.Height);
                double minValue = Math.Max(nHeight < nWidth ? nHeight : nWidth, 1);
                double scaleSpace = Math.Min(1, 100 / minValue);

                writer.WriteStartElement("a:gd");
                writer.WriteAttributeString("name", "adj");
                writer.WriteAttributeString("fmla", string.Format("val {0}", (int)(primRound.Round * scaleSpace * 100000)));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("a:noFill");
            writer.WriteEndElement();

            writer.WriteStartElement("a:ln");
            writer.WriteAttributeString("w", string.Format("{0}", ConvertToEmu(prim.Size)));
            if (prim.Style == StiPenStyle.Double)
            {
                writer.WriteAttributeString("cmpd", "dbl");
            }
            writer.WriteStartElement("a:solidFill");
            WriteColor(writer, prim.Color);
            writer.WriteFullEndElement();
            writer.WriteStartElement("a:prstDash");     //page 3755 in reference
            writer.WriteAttributeString("val", GetLineStyle(prim.Style));
            writer.WriteEndElement();
            writer.WriteFullEndElement();   //a:ln

            writer.WriteEndElement();   //p:spPr
            writer.WriteEndElement();   //p:sp
            #endregion
        }

        private string CapStyleToPptStyle(StiCapStyle capStyle)
        {
            switch (capStyle)
            {
                case StiCapStyle.Arrow:     return "triangle";
                case StiCapStyle.Diamond:   return "diamond";
                case StiCapStyle.Square:    return "diamond";
                case StiCapStyle.Open:      return "arrow";
                case StiCapStyle.Oval:      return "oval";
                case StiCapStyle.Stealth:   return "stealth";
            }
            return null;
        }

        private void WriteWatermark(XmlWriter writer, StiPage page, bool showBehind)
        {
            if (page.Watermark != null && (page.Watermark.ExistImage() || !string.IsNullOrWhiteSpace(page.Watermark.ImageHyperlink) ) && page.Watermark.ShowImageBehind == showBehind)
            {
                var image = new StiImage();
                image.Page = page;
                image.PutImage(page.Watermark.GetImage(page.Report));
                image.Left = - page.Margins.Left;
                image.Top = - page.Margins.Top;
                image.Width = page.PageWidth;
                image.Height = page.PageHeight;
                if (page.Watermark.ImageStretch) image.Stretch = true;
                image.VertAlignment = StiVertAlignment.Center;
                image.HorAlignment = StiHorAlignment.Center;

                WriteStiImage(writer, image);
            }
        }

        private void WriteHyperlinkInfo(XmlWriter writer, StiComponent comp)
        {
            if (comp.HyperlinkValue != null)
            {
                string hyperlink = comp.HyperlinkValue.ToString().Trim();
                int hypRefId;
                if (hyperlink.Length > 0 && !hyperlink.StartsWith("javascript:"))
                {
                    if (hyperlink.StartsWith("#", StringComparison.InvariantCulture))
                    {
                        //
                        //todo in next release or by request
                        //
                        //hypRefText = ConvertStringToBookmark(hyperlink.Substring(1));
                        //writer.WriteStartElement("a:hlinkClick");
                        //writer.WriteAttributeString("r:id", string.Format("hId{0}", hypRefId));
                        //writer.WriteAttributeString("action", "ppaction://hlinksldjump");
                        //writer.WriteEndElement();
                    }
                    else
                    {
                        string hypRefText = StringToUrl(hyperlink);
                        hypRefId = hyperlinkList.IndexOf(hypRefText);
                        if (hypRefId == -1)
                        {
                            hyperlinkList.Add(hypRefText);
                            hypRefId = hyperlinkList.Count;
                        }

                        writer.WriteStartElement("a:hlinkClick");
                        writer.WriteAttributeString("r:id", string.Format("hId{0}", hypRefId));
                        writer.WriteEndElement();
                    }
                }
            }
        }
        #endregion

        #region WriteImage
        private MemoryStream WriteImage(int number)
        {
            var ms = new MemoryStream();
            var buf = (byte[])imageCache.ImagePackedStore[number];
            ms.Write(buf, 0, buf.Length);
            return ms;
        }
        #endregion

		/// <summary>
        /// Exports rendered report to an PowerPoint file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportPowerPoint(StiReport report, string fileName)
		{
			FileStream stream = null;
			try
			{
				StiFileUtils.ProcessReadOnly(fileName);
				stream = new FileStream(fileName, FileMode.Create);
                ExportPowerPoint(report, stream);
			}
			finally
			{
				stream.Flush();
				stream.Close();
			}			
		}

        
		/// <summary>
        /// Exports rendered report to an PowerPoint file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
        public void ExportPowerPoint(StiReport report, Stream stream)
		{
            ExportPowerPoint(report, stream, new StiPpt2007ExportSettings());
		}

        
		/// <summary>
        /// Exports rendered report to an PowerPoint file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
		/// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportPowerPoint(StiReport report, Stream stream, StiPagesRange pageRange)
		{
            var settings = new StiPpt2007ExportSettings
            {
                PageRange = pageRange
            };

            ExportPowerPoint(report, stream, settings);
		}



        public void ExportPowerPoint(StiReport report, Stream stream, StiPpt2007ExportSettings settings)
		{
			StiLogService.Write(this.GetType(), "Export report to PowerPoint 2007 format");

#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            #region Read settings
            if (settings == null)
				throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            var pageRange = settings.PageRange;
            this.imageResolution = settings.ImageResolution;
            this.imageQuality = settings.ImageQuality;
			#endregion

			xmlIndentation = -1;
            if (imageResolution < 10) imageResolution = 10;
			imageResolution = imageResolution / 100;

			currentCulture = Thread.CurrentThread.CurrentCulture;
            newCulture = new CultureInfo("en-US", false);
			try
			{
                //StiExportUtils.DisableFontSmoothing();
                Thread.CurrentThread.CurrentCulture = newCulture;
                var culture = report.GetParsedCulture();
                reportCulture = !string.IsNullOrWhiteSpace(culture) ? culture : null;

                var allPages = pageRange.GetSelectedPages(report.RenderedPages);
				if (IsStopped) return;

				StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");


				//PrepareData
                imageCache = new StiImageCache(StiOptions.Export.PowerPoint.AllowImageComparer,
                    StiOptions.Export.PowerPoint.StoreImagesAsPng ? ImageFormat.Png : ImageFormat.Jpeg, imageQuality);


                var zip = new StiZipWriter20();
				zip.Begin(stream, true);

                zip.AddFile("[Content_Types].xml", WriteContentTypes(allPages.Count));
                zip.AddFile("_rels/.rels", WriteMainRels());
                zip.AddFile("docProps/app.xml", WriteDocPropsApp(allPages.Count));
                zip.AddFile("docProps/core.xml", WriteDocPropsCore());
                zip.AddFile("ppt/tableStyles.xml", WriteTableStyles());
                zip.AddFile("ppt/presProps.xml", WritePresProps());
                zip.AddFile("ppt/viewProps.xml", WriteViewProps());
                zip.AddFile("ppt/theme/theme1.xml", WriteTheme());
                zip.AddFile("ppt/slideMasters/slideMaster1.xml", WriteSlideMaster());
                zip.AddFile("ppt/slideMasters/_rels/slideMaster1.xml.rels", WriteSlideMasterRels());
                for (int index = 0; index < 11; index++)
                {
                    zip.AddFile(string.Format("ppt/slideLayouts/slideLayout{0}.xml", index + 1), WriteSlideLayout(index + 1, allPages[0]));
                    zip.AddFile(string.Format("ppt/slideLayouts/_rels/slideLayout{0}.xml.rels", index + 1), WriteSlideLayoutRels());
                }

                zip.AddFile("ppt/presentation.xml", WritePresentation(allPages));
                zip.AddFile("ppt/_rels/presentation.xml.rels", WritePresentationRels(allPages));

                imageListOffset = 0;
                hyperlinkList = new ArrayList();
                for (int indexPage = 0; indexPage < allPages.Count; indexPage++)
                {
                    InvokeExporting(indexPage, allPages.Count, 0, 1);
                    if (IsStopped) return;

                    var page = allPages[indexPage];
                    allPages.GetPage(page);

                    zip.AddFile(string.Format("ppt/slides/slide{0}.xml", indexPage + 1), WriteSlide(indexPage, allPages));
                    zip.AddFile(string.Format("ppt/slides/_rels/slide{0}.xml.rels", indexPage + 1), WriteSlideRels(indexPage));

                    imageListOffset = imageCache.ImageIndex.Count;
                    hyperlinkList.Clear();
                }

                if (imageCache.ImagePackedStore.Count > 0)
                {
                    for (int index = 0; index < imageCache.ImagePackedStore.Count; index++)
                    {
                        zip.AddFile(string.Format("ppt/media/image{0:D5}.{1}", index + 1, StiOptions.Export.PowerPoint.StoreImagesAsPng ? "png" : "jpeg"), WriteImage(index));
                    }
                }

                zip.End();
			}
			finally
			{
                StiExportUtils.EnableFontSmoothing(report);
                Thread.CurrentThread.CurrentCulture = currentCulture;
				imageCache.Clear();
                hyperlinkList = null;
			}
		}
		#endregion
	}
}