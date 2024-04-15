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
using System.Text;
using System.IO;
using Stimulsoft.Report.Export;
using Stimulsoft.Base;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Collections;
using Stimulsoft.Base.Drawing;
using System.Reflection;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Export;
using Stimulsoft.Report.Components;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiExportsHelper
    {
        #region Methods

        public static string GetReportFileContentType(StiRequestParams requestParams)
        {
            switch (requestParams.ExportFormat)
            {
                case StiExportFormat.Document:
                    StiDocumentType type = GetExportDocumentType(requestParams);
                    switch (type)
                    {
                        case StiDocumentType.Mdc: return "text/xml";
                        case StiDocumentType.Mdz: return "application/zip";
                        case StiDocumentType.Mdx: return "application/octet-stream";
                    }
                    break;

                case StiExportFormat.Pdf: return "application/pdf";
                case StiExportFormat.Xps: return "application/vnd.ms-xpsdocument";
                case StiExportFormat.Ppt2007: return "application/vnd.ms-powerpoint";
                case StiExportFormat.Text: return "text/plain";
                case StiExportFormat.Rtf: return "application/rtf";
                case StiExportFormat.Word2007: return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case StiExportFormat.Odt: return "application/vnd.oasis.opendocument.text";
                case StiExportFormat.Ods: return "application/vnd.oasis.opendocument.spreadsheet";

                case StiExportFormat.Html:
                case StiExportFormat.Html5: return "text/html";
                case StiExportFormat.Mht: return "message/rfc822";

                case StiExportFormat.Excel:
                case StiExportFormat.ExcelXml: return "application/vnd.ms-excel";
                case StiExportFormat.Excel2007: return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                case StiExportFormat.Csv: return "text/csv";
                case StiExportFormat.Dbf: return "application/dbf";
                case StiExportFormat.Xml: return "application/xml";
                case StiExportFormat.Json: return "application/json";
                case StiExportFormat.Dif: return "video/x-dv";
                case StiExportFormat.Sylk: return "application/excel";

                case StiExportFormat.ImageBmp: return "image/bmp";
                case StiExportFormat.ImageGif: return "image/gif";
                case StiExportFormat.ImageJpeg: return "image/jpeg";
                case StiExportFormat.ImagePcx: return "image/x-pcx";
                case StiExportFormat.ImagePng: return "image/png";
                case StiExportFormat.ImageTiff: return "image/tiff";
                case StiExportFormat.ImageEmf: return "image/x-emf";
                case StiExportFormat.ImageSvg:
                case StiExportFormat.ImageSvgz: return "image/svg+xml";
            }

            if (requestParams.ExportFormat == StiExportFormat.Image && requestParams.ExportSettings != null && requestParams.ExportSettings.Contains("ImageType"))
            {
                switch (requestParams.ExportSettings["ImageType"].ToString())
                {
                    case "Bmp": return "image/bmp";
                    case "Gif": return "image/gif";
                    case "Jpeg": return "image/jpeg";
                    case "Pcx": return "image/x-pcx";
                    case "Png": return "image/png";
                    case "Tiff": return "image/tiff";
                    case "Emf": return "image/x-emf";
                    case "Svg":
                    case "Svgz": return "image/svg+xml";
                }
            }

            return "text/plain";
        }

        public static string GetReportFileName(StiRequestParams requestParams, StiReport report)
        {
            var fileName = requestParams.Viewer.ReportType == StiReportType.Dashboard
                ? string.IsNullOrEmpty(requestParams.Viewer.ElementName)
                    ? string.IsNullOrWhiteSpace(report.Pages[requestParams.Viewer.PageNumber].Alias)
                        ? report.Pages[requestParams.Viewer.PageNumber].Name
                        : report.Pages[requestParams.Viewer.PageNumber].Alias
                    : requestParams.Viewer.ElementName
                : StiReportHelper.GetReportFileName(report);

            fileName += ".";
            switch (requestParams.ExportFormat)
            {
                case StiExportFormat.Document:
                    return fileName + GetExportDocumentType(requestParams).ToString().ToLower();

                case StiExportFormat.Excel:
                    return fileName + "xls";

                case StiExportFormat.Excel2007:
                    return fileName + "xlsx";

                case StiExportFormat.ExcelXml:
                    return fileName + "xml.xls";

                case StiExportFormat.Ppt2007:
                    return fileName + "pptx";

                case StiExportFormat.Text:
                    return fileName + "txt";

                case StiExportFormat.Word2007:
                    return fileName + "docx";

                case StiExportFormat.Html5:
                    return fileName + "html";
            }

            var ext = requestParams.ExportFormat.ToString().ToLower().Replace("image", "");

            if (requestParams.ExportSettings != null && requestParams.ExportSettings.Contains("ImageType")) {
                if (String.IsNullOrEmpty(ext))
                    ext = requestParams.ExportSettings["ImageType"].ToString().ToLower();

                if (Convert.ToBoolean(requestParams.ExportSettings["CompressToArchive"]))
                    ext = "zip";
            }

            if (ext == "data" && requestParams.ExportSettings != null && requestParams.ExportSettings.Contains("DataType"))
            {
                ext = requestParams.ExportSettings["DataType"].ToString().ToLower();
            }

            return fileName + ext;
        }

        #endregion

        #region Export Settings

        private static StiDocumentType GetExportDocumentType(StiRequestParams requestParams)
        {
            string format = requestParams.ExportSettings["Format"] != null ? requestParams.ExportSettings["Format"].ToString().ToLower() : null;
            switch (format)
            {
                case "mdc": return StiDocumentType.Mdc;
                case "mdz": return StiDocumentType.Mdz;
                case "mdx": return StiDocumentType.Mdx;
            }

            return StiDocumentType.Mdc;
        }

        public static StiExportSettings GetExportSettings(StiRequestParams requestParams)
        {
            // Export settings for the report printing
            if (requestParams.Action == StiAction.PrintReport)
            {
                if (requestParams.Viewer.PrintAction == StiPrintAction.PrintPdf) return new StiPdfExportSettings();
                return new StiHtmlExportSettings();
            }

            switch (requestParams.ExportFormat)
            {
                case StiExportFormat.Pdf: return GetPdfExportSettings(requestParams.ExportSettings);
                case StiExportFormat.Xps: return GetXpsExportSettings(requestParams.ExportSettings);
                case StiExportFormat.Ppt2007: return GetPpt2007ExportSettings(requestParams.ExportSettings);
                case StiExportFormat.Text: return GetTextExportSettings(requestParams.ExportSettings);
                case StiExportFormat.Rtf: return GetRtfExportSettings(requestParams.ExportSettings);
                case StiExportFormat.Word2007: return GetWord2007ExportSettings(requestParams.ExportSettings);
                case StiExportFormat.Odt: return GetOdtExportSettings(requestParams.ExportSettings);
                case StiExportFormat.Ods: return GetOdsExportSettings(requestParams.ExportSettings);

                case StiExportFormat.Html:
                case StiExportFormat.Html5:
                case StiExportFormat.Mht: return GetHtmlExportSettings(requestParams.ExportSettings);

                case StiExportFormat.Excel:
                case StiExportFormat.Excel2007:
                case StiExportFormat.ExcelXml: return GetExcelExportSettings(requestParams.ExportSettings);

                case StiExportFormat.Csv:
                case StiExportFormat.Dbf:
                case StiExportFormat.Xml:
                case StiExportFormat.Json:
                case StiExportFormat.Dif:
                case StiExportFormat.Sylk: return GetDataExportSettings(requestParams.ExportSettings);

                case StiExportFormat.ImageBmp:
                case StiExportFormat.ImageGif:
                case StiExportFormat.ImageJpeg:
                case StiExportFormat.ImagePcx:
                case StiExportFormat.ImagePng:
                case StiExportFormat.ImageTiff:
                case StiExportFormat.ImageEmf:
                case StiExportFormat.ImageSvg:
                case StiExportFormat.ImageSvgz: return GetImageExportSettings(requestParams.ExportSettings);
            }

            return null;
        }

        public static IStiDashboardExportSettings GetDashboardExportSettings(StiRequestParams requestParams)
        {
            switch (requestParams.ExportFormat)
            {
                case StiExportFormat.Pdf: return GetPdfDashboardExportSettings(requestParams.ExportSettings);
                case StiExportFormat.Excel2007: return GetExcelDashboardExportSettings(requestParams.ExportSettings);
                case StiExportFormat.Data: return GetDataDashboardExportSettings(requestParams.ExportSettings);
                case StiExportFormat.Image: return GetImageDashboardExportSettings(requestParams.ExportSettings);
                case StiExportFormat.Html: return GetHtmlDashboardExportSettings(requestParams.ExportSettings);
            }

            return null;
        }

        private static Hashtable GetExportSettingsValues(object settings)
        {
            var values = new Hashtable();
            Type type = settings.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name != "CompanyString" && property.Name != "LastModifiedString")
                {
                    object value = property.GetValue(settings, null);
                    if (value is StiPagesRange) value =
                        ((StiPagesRange)value).RangeType == StiRangeType.CurrentPage
                            ? ((StiPagesRange)value).CurrentPage.ToString()
                            : ((StiPagesRange)value).RangeType == StiRangeType.Pages
                                ? ((StiPagesRange)value).PageRanges
                                : ((StiPagesRange)value).RangeType.ToString();
                    if (value is Encoding) value = ((Encoding)value).CodePage.ToString();
                    if (value is ImageFormat) value = ((ImageFormat)value).ToString();

                    values[property.Name] = value;
                }
            }

            return values;
        }

        public static Hashtable GetDefaultExportSettings(StiDefaultExportSettings exportSettings)
        {
            //Reports export settings
            StiExportSettings[] defaultSettingsObjects = {
                exportSettings.ExportToPdf,
                exportSettings.ExportToXps,
                exportSettings.ExportToPowerPoint,
                exportSettings.ExportToHtml,
                exportSettings.ExportToText,
                exportSettings.ExportToRtf,
                exportSettings.ExportToWord2007,
                exportSettings.ExportToOpenDocumentWriter,
                exportSettings.ExportToExcel,
                exportSettings.ExportToOpenDocumentCalc,
                exportSettings.ExportToData,
                exportSettings.ExportToImage
            };

            var defaultSettings = new Hashtable();
            foreach (StiExportSettings settings in defaultSettingsObjects)
            {
                defaultSettings[settings.GetType().Name] = GetExportSettingsValues(settings);
            }

            //Dashboards export settings
            if (StiDashboardExportAssembly.IsAssemblyLoaded)
            {
                var dashboardExportSettingFormats = new List<StiExportFormat> { StiExportFormat.Pdf, StiExportFormat.Excel2007, StiExportFormat.Data, StiExportFormat.Image, StiExportFormat.Html, StiExportFormat.Html5 };

                foreach (StiExportFormat format in dashboardExportSettingFormats)
                {
                    var settings = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard.Export", "Helpers.StiExportSettingsHelper",
                        "GetDashboardExportSettings", new object[] { format }, new Type[] { typeof(StiExportFormat) });

                    defaultSettings["Dashboard" + format.ToString()] = GetExportSettingsValues(settings);
                }
            }

            return defaultSettings;
        }

        private static Encoding GetEncoding(string value)
        {
            if (Char.IsDigit(value[0])) return Encoding.GetEncoding(Convert.ToInt32(value));
            return Encoding.GetEncoding(value);
        }

        #region Adobe PDF dashboard settings

        /// <summary>
        /// Get export settings for Adobe PDF format.
        /// </summary>
        private static IStiPdfDashboardExportSettings GetPdfDashboardExportSettings(Hashtable jsSettings)
        {
            var settings = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard.Export", "Helpers.StiExportSettingsHelper", "GetDashboardExportSettings",
                new object[] { StiExportFormat.Pdf }, new Type[] { typeof(StiExportFormat) }) as IStiPdfDashboardExportSettings;

            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PaperSize":
                        settings.PaperSize = (PaperKind)Enum.Parse(typeof(PaperKind), value);
                        break;

                    case "Orientation":
                        settings.Orientation = (StiPageOrientation)Enum.Parse(typeof(StiPageOrientation), value);
                        break;

                    case "ImageQuality":
                        int imageQuality = 200;
                        if (int.TryParse(value, out imageQuality))
                            settings.ImageQuality = imageQuality;
                        break;
                }
            }

            return settings;
        }

        #endregion

        #region Data dashboard settings

        /// <summary>
        /// Get export settings for Data format.
        /// </summary>
        private static IStiDataDashboardExportSettings GetDataDashboardExportSettings(Hashtable jsSettings)
        {
            var settings = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard.Export", "Helpers.StiExportSettingsHelper", "GetDashboardExportSettings",
                new object[] { StiExportFormat.Data }, new Type[] { typeof(StiExportFormat) }) as IStiDataDashboardExportSettings;

            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "DataType":
                        settings.DataType = (StiDataType)Enum.Parse(typeof(StiDataType), value);
                        break;
                }
            }

            return settings;
        }

        #endregion

        #region Excel dashboard settings

        /// <summary>
        /// Get export settings for Excel format.
        /// </summary>
        private static IStiExcelDashboardExportSettings GetExcelDashboardExportSettings(Hashtable jsSettings)
        {
            var settings = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard.Export", "Helpers.StiExportSettingsHelper", "GetDashboardExportSettings",
                new object[] { StiExportFormat.Excel2007 }, new Type[] { typeof(StiExportFormat) }) as IStiExcelDashboardExportSettings;

            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "ImageQuality":
                        int imageQuality = 200;
                        if (int.TryParse(value, out imageQuality))
                            settings.ImageQuality = imageQuality;
                        break;

                    case "ExportDataOnly":
                        settings.ExportDataOnly = Convert.ToBoolean(value);
                        break;
                }
            }

            return settings;
        }

        #endregion

        #region Image dashboard settings

        /// <summary>
        /// Get export settings for Image format.
        /// </summary>
        private static IStiImageDashboardExportSettings GetImageDashboardExportSettings(Hashtable jsSettings)
        {
            var settings = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard.Export", "Helpers.StiExportSettingsHelper", "GetDashboardExportSettings",
                new object[] { StiExportFormat.Image }, new Type[] { typeof(StiExportFormat) }) as IStiImageDashboardExportSettings;

            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "ImageType":
                        settings.ImageType = (StiImageType)Enum.Parse(typeof(StiImageType), value);
                        break;

                    case "Scale":
                        int scale = 100;
                        if (int.TryParse(value, out scale))
                            settings.Scale = scale;
                        break;
                }
            }

            return settings;
        }

        #endregion

        #region Html dashboard settings

        /// <summary>
        /// Get export settings for Html format.
        /// </summary>
        private static IStiHtmlDashboardExportSettings GetHtmlDashboardExportSettings(Hashtable jsSettings)
        {
            var settings = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard.Export", "Helpers.StiExportSettingsHelper", "GetDashboardExportSettings",
                new object[] { StiExportFormat.Html }, new Type[] { typeof(StiExportFormat) }) as IStiHtmlDashboardExportSettings;

            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "ImageQuality":
                        int imageQuality = 200;
                        if (int.TryParse(value, out imageQuality))
                            settings.ImageQuality = imageQuality;
                        break;

                    case "Scale":
                        int scale = 100;
                        if (int.TryParse(value, out scale))
                            settings.Scale = scale;
                        break;
                    
                    case "EnableAnimation":
                            settings.EnableAnimation = Convert.ToBoolean(value);
                        break;
                }
            }

            return settings;
        }

        #endregion

        #region Adobe PDF settings

        /// <summary>
        /// Get export settings for Adobe PDF format.
        /// </summary>
        private static StiPdfExportSettings GetPdfExportSettings(Hashtable jsSettings)
        {
            var settings = new StiPdfExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "ImageQuality": settings.ImageQuality = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "ImageResolution": settings.ImageResolution = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "EmbeddedFonts": settings.EmbeddedFonts = Convert.ToBoolean(value); break;
                    case "StandardPdfFonts": settings.StandardPdfFonts = Convert.ToBoolean(value); break;
                    case "Compressed": settings.Compressed = Convert.ToBoolean(value); break;
                    case "ExportRtfTextAsImage": settings.ExportRtfTextAsImage = Convert.ToBoolean(value); break;
                    case "PasswordInputUser": settings.PasswordInputUser = value; break;
                    case "PasswordInputOwner": settings.PasswordInputOwner = value; break;
                    case "UserAccessPrivileges": settings.UserAccessPrivileges = (StiUserAccessPrivileges)Enum.Parse(typeof(StiUserAccessPrivileges), value); break;
                    case "KeyLength": settings.KeyLength = (StiPdfEncryptionKeyLength)Enum.Parse(typeof(StiPdfEncryptionKeyLength), value); break;
                    case "UseUnicode": settings.UseUnicode = Convert.ToBoolean(value); break;
                    case "UseDigitalSignature": settings.UseDigitalSignature = Convert.ToBoolean(value); break;
                    case "GetCertificateFromCryptoUI": settings.GetCertificateFromCryptoUI = Convert.ToBoolean(value); break;
                    case "SubjectNameString": settings.SubjectNameString = value; break;
                    case "UseLocalMachineCertificates": settings.UseLocalMachineCertificates = Convert.ToBoolean(value); break;
                    case "CreatorString": settings.CreatorString = value; break;
                    case "KeywordsString": settings.KeywordsString = value; break;
                    case "ImageCompressionMethod": settings.ImageCompressionMethod = (StiPdfImageCompressionMethod)Enum.Parse(typeof(StiPdfImageCompressionMethod), value); break;
                    case "ImageFormat": settings.ImageFormat = (StiImageFormat)Enum.Parse(typeof(StiImageFormat), value); break;
                    case "DitheringType": settings.DitheringType = (StiMonochromeDitheringType)Enum.Parse(typeof(StiMonochromeDitheringType), value); break;
                    case "PdfACompliance": settings.PdfComplianceMode = Convert.ToBoolean(value) ? StiPdfComplianceMode.A1 : StiPdfComplianceMode.None; break;
                    case "AutoPrintMode": settings.AutoPrintMode = (StiPdfAutoPrintMode)Enum.Parse(typeof(StiPdfAutoPrintMode), value); break;
                    case "AllowEditable": settings.AllowEditable = (StiPdfAllowEditable)Enum.Parse(typeof(StiPdfAllowEditable), value); break;
                    case "CertificateThumbprint": settings.CertificateThumbprint = value; break;
                    case "ImageResolutionMode": settings.ImageResolutionMode = (StiImageResolutionMode)Enum.Parse(typeof(StiImageResolutionMode), value); break;
                }
            }

            return settings;
        }

        #endregion

        #region Microsoft XPS settings

        /// <summary>
        /// Get export settings for Microsoft XPS format.
        /// </summary>
        private static StiXpsExportSettings GetXpsExportSettings(Hashtable jsSettings)
        {
            var settings = new StiXpsExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "ImageResolution": settings.ImageResolution = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "ImageQuality": settings.ImageQuality = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "ExportRtfTextAsImage": settings.ExportRtfTextAsImage = Convert.ToBoolean(value); break;
                }
            }

            return settings;
        }

        #endregion

        #region Microsoft PowerPoint 2007 settings

        /// <summary>
        /// Get export settings for Microsoft PowerPoint 2007 format.
        /// </summary>
        private static StiPpt2007ExportSettings GetPpt2007ExportSettings(Hashtable jsSettings)
        {
            var settings = new StiPpt2007ExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "ImageResolution": settings.ImageResolution = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "ImageQuality": settings.ImageQuality = (float)StiObjectConverter.ConvertToDouble(value); break;
                }
            }

            return settings;
        }

        #endregion

        #region HTML settings

        /// <summary>
        /// Get export settings for HTML format.
        /// </summary>
        private static StiHtmlExportSettings GetHtmlExportSettings(Hashtable jsSettings)
        {
            var settings = new StiHtmlExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "HtmlType": settings.HtmlType = (StiHtmlType)Enum.Parse(typeof(StiHtmlType), value); break;
                    case "ImageQuality": settings.ImageQuality = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "ImageResolution": settings.ImageResolution = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "Encoding": settings.Encoding = GetEncoding(value); break;
                    case "Zoom": settings.Zoom = StiObjectConverter.ConvertToDouble(value); break;
                    case "ExportMode": settings.ExportMode = (StiHtmlExportMode)Enum.Parse(typeof(StiHtmlExportMode), value); break;
                    case "ExportQuality": settings.ExportQuality = (StiHtmlExportQuality)Enum.Parse(typeof(StiHtmlExportQuality), value); break;
                    case "AddPageBreaks": settings.AddPageBreaks = Convert.ToBoolean(value); break;
                    case "BookmarksTreeWidth": settings.BookmarksTreeWidth = Convert.ToInt32(value); break;
                    case "ExportBookmarksMode": settings.ExportBookmarksMode = (StiHtmlExportBookmarksMode)Enum.Parse(typeof(StiHtmlExportBookmarksMode), value); break;
                    case "UseStylesTable": settings.UseStylesTable = Convert.ToBoolean(value); break;
                    case "RemoveEmptySpaceAtBottom": settings.RemoveEmptySpaceAtBottom = Convert.ToBoolean(value); break;
                    case "PageHorAlignment": settings.PageHorAlignment = (StiHorAlignment)Enum.Parse(typeof(StiHorAlignment), value); break;
                    case "CompressToArchive": settings.CompressToArchive = Convert.ToBoolean(value); break;
                    case "UseEmbeddedImages": settings.UseEmbeddedImages = Convert.ToBoolean(value); break;
                    case "ContinuousPages": settings.ContinuousPages = Convert.ToBoolean(value); break;

                    case "ImageFormat":
                        switch (value)
                        {
                            case "Bmp": settings.ImageFormat = ImageFormat.Bmp; break;
                            case "Emf": settings.ImageFormat = ImageFormat.Emf; break;
                            case "Gif": settings.ImageFormat = ImageFormat.Gif; break;
                            case "Jpeg": settings.ImageFormat = ImageFormat.Jpeg; break;
                            case "Png": settings.ImageFormat = ImageFormat.Png; break;
                            case "Tiff": settings.ImageFormat = ImageFormat.Tiff; break;
                        }
                        break;
                }
            }

            return settings;
        }

        #endregion

        #region Text settings

        /// <summary>
        /// Get export settings for Text format.
        /// </summary>
        private static StiTxtExportSettings GetTextExportSettings(Hashtable jsSettings)
        {
            var settings = new StiTxtExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "Encoding": settings.Encoding = GetEncoding(value); break;
                    case "DrawBorder": settings.DrawBorder = Convert.ToBoolean(value); break;
                    case "BorderType": settings.BorderType = (StiTxtBorderType)Enum.Parse(typeof(StiTxtBorderType), value); break;
                    case "KillSpaceLines": settings.KillSpaceLines = Convert.ToBoolean(value); break;
                    case "KillSpaceGraphLines": settings.KillSpaceGraphLines = Convert.ToBoolean(value); break;
                    case "PutFeedPageCode": settings.PutFeedPageCode = Convert.ToBoolean(value); break;
                    case "CutLongLines": settings.CutLongLines = Convert.ToBoolean(value); break;
                    case "ZoomX": settings.ZoomX = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "ZoomY": settings.ZoomY = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "UseEscapeCodes": settings.UseEscapeCodes = Convert.ToBoolean(value); break;
                    case "EscapeCodesCollectionName": settings.EscapeCodesCollectionName = value; break;
                }
            }

            return settings;
        }

        #endregion

        #region Rich Text settings

        /// <summary>
        /// Get export settings for Rich Text format.
        /// </summary>
        private static StiRtfExportSettings GetRtfExportSettings(Hashtable jsSettings)
        {
            var settings = new StiRtfExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "CodePage": settings.CodePage = Convert.ToInt32(value); break;
                    case "ExportMode": settings.ExportMode = (StiRtfExportMode)Enum.Parse(typeof(StiRtfExportMode), value); break;
                    case "UsePageHeadersAndFooters": settings.UsePageHeadersAndFooters = Convert.ToBoolean(value); break;
                    case "ImageResolution": settings.ImageResolution = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "ImageQuality": settings.ImageQuality = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "RemoveEmptySpaceAtBottom": settings.RemoveEmptySpaceAtBottom = Convert.ToBoolean(value); break;
                    case "StoreImagesAsPng": settings.StoreImagesAsPng = Convert.ToBoolean(value); break;
                }
            }

            return settings;
        }

        #endregion

        #region Microsoft Word 2007 settings

        /// <summary>
        /// Get export settings for Microsoft Word 2007 format.
        /// </summary>
        private static StiWord2007ExportSettings GetWord2007ExportSettings(Hashtable jsSettings)
        {
            var settings = new StiWord2007ExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "UsePageHeadersAndFooters": settings.UsePageHeadersAndFooters = Convert.ToBoolean(value); break;
                    case "ImageQuality": settings.ImageQuality = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "ImageResolution": settings.ImageResolution = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "RemoveEmptySpaceAtBottom": settings.RemoveEmptySpaceAtBottom = Convert.ToBoolean(value); break;
                    case "CompanyString": settings.CompanyString = value; break;
                    case "LastModifiedString": settings.LastModifiedString = value; break;
                    case "RestrictEditing": settings.RestrictEditing = (StiWord2007RestrictEditing)Enum.Parse(typeof(StiWord2007RestrictEditing), value); break;
                }
            }

            return settings;
        }

        #endregion

        #region OpenDocument Writer settings

        /// <summary>
        /// Get export settings for OpenDocument Writer format.
        /// </summary>
        private static StiOdtExportSettings GetOdtExportSettings(Hashtable jsSettings)
        {
            var settings = new StiOdtExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "UsePageHeadersAndFooters": settings.UsePageHeadersAndFooters = Convert.ToBoolean(value); break;
                    case "ImageResolution": settings.ImageResolution = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "ImageQuality": settings.ImageQuality = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "RemoveEmptySpaceAtBottom": settings.RemoveEmptySpaceAtBottom = Convert.ToBoolean(value); break;
                }
            }

            return settings;
        }

        #endregion

        #region Microsoft Excel settings

        /// <summary>
        /// Get export settings for Microsoft Excel format.
        /// </summary>
        private static StiExcelExportSettings GetExcelExportSettings(Hashtable jsSettings)
        {
            var settings = new StiExcelExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "ExcelType": settings.ExcelType = (StiExcelType)Enum.Parse(typeof(StiExcelType), value); break;
                    case "UseOnePageHeaderAndFooter": settings.UseOnePageHeaderAndFooter = Convert.ToBoolean(value); break;
                    case "DataExportMode": settings.DataExportMode = (StiDataExportMode)Enum.Parse(typeof(StiDataExportMode), value); break;
                    case "ExportPageBreaks": settings.ExportPageBreaks = Convert.ToBoolean(value); break;
                    case "ExportObjectFormatting": settings.ExportObjectFormatting = Convert.ToBoolean(value); break;
                    case "ExportEachPageToSheet": settings.ExportEachPageToSheet = Convert.ToBoolean(value); break;
                    case "ImageQuality": settings.ImageQuality = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "ImageResolution": settings.ImageResolution = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "CompanyString": settings.CompanyString = value; break;
                    case "LastModifiedString": settings.LastModifiedString = value; break;
                }
            }

            return settings;
        }

        #endregion

        #region OpenDocument Calc settings

        /// <summary>
        /// Get export settings for OpenDocument Calc format.
        /// </summary>
        private static StiOdsExportSettings GetOdsExportSettings(Hashtable jsSettings)
        {
            var settings = new StiOdsExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "ImageResolution": settings.ImageResolution = (float)StiObjectConverter.ConvertToDouble(value); break;
                    case "ImageQuality": settings.ImageQuality = (float)StiObjectConverter.ConvertToDouble(value); break;
                }
            }

            return settings;
        }

        #endregion

        #region Data settings

        /// <summary>
        /// Get export settings for Data format.
        /// </summary>
        private static StiDataExportSettings GetDataExportSettings(Hashtable jsSettings)
        {
            var settings = new StiDataExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "DataType": settings.DataType = (StiDataType)Enum.Parse(typeof(StiDataType), value); break;
                    case "DataExportMode": settings.DataExportMode = (StiDataExportMode)Enum.Parse(typeof(StiDataExportMode), value); break;
                    case "Encoding": settings.Encoding = GetEncoding(value); break;
                    case "ExportDataOnly": settings.ExportDataOnly = Convert.ToBoolean(value); break;
                    case "CodePage": settings.CodePage = (StiDbfCodePages)Enum.Parse(typeof(StiDbfCodePages), value); break;
                    case "Separator": settings.Separator = value; break;
                    case "SkipColumnHeaders": settings.SkipColumnHeaders = Convert.ToBoolean(value); break;
                    case "UseDefaultSystemEncoding": settings.UseDefaultSystemEncoding = Convert.ToBoolean(value); break;
                }
            }

            return settings;
        }

        #endregion

        #region Image settings

        /// <summary>
        /// Get export settings for Images format.
        /// </summary>
        private static StiImageExportSettings GetImageExportSettings(Hashtable jsSettings)
        {
            var settings = new StiImageExportSettings();
            foreach (string key in jsSettings.Keys)
            {
                string value = jsSettings[key] != null ? jsSettings[key].ToString() : null;
                switch (key)
                {
                    case "PageRange":
                    case "PagesRange": settings.PageRange = new StiPagesRange(value); break;
                    case "ImageType": settings.ImageType = (StiImageType)Enum.Parse(typeof(StiImageType), value); break;
                    case "ImageZoom": settings.ImageZoom = StiObjectConverter.ConvertToDouble(value); break;
                    case "ImageResolution": settings.ImageResolution = Convert.ToInt32(value); break;
                    case "CutEdges": settings.CutEdges = Convert.ToBoolean(value); break;
                    case "ImageFormat": settings.ImageFormat = (StiImageFormat)Enum.Parse(typeof(StiImageFormat), value); break;
                    case "MultipleFiles": settings.MultipleFiles = Convert.ToBoolean(value); break;
                    case "DitheringType": settings.DitheringType = (StiMonochromeDitheringType)Enum.Parse(typeof(StiMonochromeDitheringType), value); break;
                    case "TiffCompressionScheme": settings.TiffCompressionScheme = (StiTiffCompressionScheme)Enum.Parse(typeof(StiTiffCompressionScheme), value); break;
                    case "CompressToArchive": settings.CompressToArchive = Convert.ToBoolean(value); break;
                }
            }

            return settings;
        }

        #endregion

        #endregion

        #region Export Report
        /// <summary>
        /// Exports dashboard page or element to the specified format
        /// </summary>
        public static Stream ExportDashboard(StiRequestParams requestParams, StiReport report, IStiDashboardExportSettings exportSettings)
        {
            // Set current report template page for export detection
            var page = report.Pages.ToList().FirstOrDefault(p => !(p is IStiDashboard)) ?? report.Pages[0];
            report.CurrentPage = requestParams.Viewer.PageNumber;
                        
            var dashboard = (IStiDashboard)report.Pages[requestParams.Viewer.PageNumber];

            if (!((StiPage)dashboard).Enabled)
                dashboard = report.Pages.ToList().FirstOrDefault(p => p.Enabled && p.IsDashboard) as IStiDashboard;

            var element = dashboard.GetElements(false).FirstOrDefault(e => e.Name == requestParams.Viewer.ElementName);

            if (exportSettings is IStiImageDashboardExportSettings)
            {
                ((IStiImageDashboardExportSettings)exportSettings).Width = requestParams.Viewer.DashboardWidth;
                ((IStiImageDashboardExportSettings)exportSettings).Height = requestParams.Viewer.DashboardHeight;
            }

            report.InvokeExporting(requestParams.ExportFormat);

            if (requestParams.ExportFormat == StiExportFormat.Document)
            {
                var reportClone = dashboard.Report.Clone() as StiReport;
                using (var stream = new MemoryStream())
                {
                    if (!string.IsNullOrEmpty(report.ReportGuid))
                        reportClone.ReportGuid = report.ReportGuid;

                    reportClone.SaveSnapshot(stream);

                    report.InvokeExported(requestParams.ExportFormat);

                    return stream;
                }
            }
            else
            {
                var stream = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard.Export", "StiDashboardExportTools", "ExportToStream",
                    new object[] { element != null ? element : dashboard, exportSettings },
                    new Type[] { typeof(IStiElement), StiInvokeMethodsHelper.GetType("Stimulsoft.Dashboard.Export", "Settings.StiDashboardExportSettings") });

                report.InvokeExported(requestParams.ExportFormat);

                return (Stream)stream;
            }
        }

        /// <summary>
        /// Exports report to the specified format
        /// </summary>
        public static Stream ExportReport(StiRequestParams requestParams, StiReport report, StiExportSettings settings)
        {
            // Set current report template page for export detection
            var page = report.Pages.ToList().FirstOrDefault(p => !(p is IStiDashboard)) ?? report.Pages[0];
            report.CurrentPage = report.Pages.IndexOf(page);

            StiEditableFieldsHelper.ApplyEditableFieldsToReport(report, requestParams.Interaction.Editable);

            StiExportService service = null;
            Stream stream = new MemoryStream();
            switch (requestParams.ExportFormat)
            {
                case StiExportFormat.Document:
                    StiDocumentType documentType = GetExportDocumentType(requestParams);
                    switch (documentType)
                    {
                        case StiDocumentType.Mdc:
                            report.SaveDocument(stream);
                            break;

                        case StiDocumentType.Mdz:
                            report.SavePackedDocument(stream);
                            break;

                        case StiDocumentType.Mdx:
                            string password = requestParams.ExportSettings["SaveReportPassword"] != null
                                ? (string)requestParams.ExportSettings["SaveReportPassword"] 
                                : string.Empty;
                            report.SaveEncryptedDocument(stream, password);
                            break;
                    }
                    break;

                case StiExportFormat.Pdf: service = new StiPdfExportService(); break;
                case StiExportFormat.Xps: service = new StiXpsExportService(); break;
                case StiExportFormat.Ppt2007: service = new StiPpt2007ExportService(); break;
                case StiExportFormat.Html: service = new StiHtmlExportService(); break;
                case StiExportFormat.Html5: service = new StiHtml5ExportService(); break;
                case StiExportFormat.Mht: service = new StiMhtExportService(); break;
                case StiExportFormat.Text: service = new StiTxtExportService(); break;
                case StiExportFormat.Rtf: service = new StiRtfExportService(); break;
                case StiExportFormat.Word2007: service = new StiWord2007ExportService(); break;
                case StiExportFormat.Odt: service = new StiOdtExportService(); break;
                case StiExportFormat.Excel: service = new StiExcelExportService(); break;
                case StiExportFormat.ExcelXml: service = new StiExcelXmlExportService(); break;
                case StiExportFormat.Excel2007: service = new StiExcel2007ExportService(); break;
                case StiExportFormat.Ods: service = new StiOdsExportService(); break;

                case StiExportFormat.Csv:
                case StiExportFormat.Dbf:
                case StiExportFormat.Xml:
                case StiExportFormat.Json:
                case StiExportFormat.Dif:
                case StiExportFormat.Sylk:
                    service = new StiDataExportService();
                    break;

                case StiExportFormat.ImageBmp:
                case StiExportFormat.ImageGif:
                case StiExportFormat.ImageJpeg:
                case StiExportFormat.ImagePcx:
                case StiExportFormat.ImagePng:
                case StiExportFormat.ImageTiff:
                case StiExportFormat.ImageEmf:
                case StiExportFormat.ImageSvg:
                case StiExportFormat.ImageSvgz:
                    service = new StiImageExportService();
                    break;
            }

            if (service != null)
            {
                if (report.CookieContainer == null || report.CookieContainer.Count == 0 || requestParams.Server.AllowAutoUpdateCookies)
                    report.CookieContainer = StiReportHelper.GetCookieContainer(requestParams.HttpContext.Request);

                report.InvokeExporting(requestParams.ExportFormat);

                service.ExportTo(report, stream, settings);

                report.InvokeExported(requestParams.ExportFormat);
            }           

            return stream;
        }

        #endregion

        #region Send Email

        public static Stream EmailReport(StiRequestParams requestParams, StiReport report, StiExportSettings settings, StiEmailOptions options)
        {
            var attachmentName = GetReportFileName(requestParams, report);
            var result = string.Empty;
            var stream = ExportReport(requestParams, report, settings);
            stream.Seek(0, SeekOrigin.Begin);

            try
            {
                var attachment = new Attachment(stream, attachmentName);
                var message = new MailMessage(options.AddressFrom, options.AddressTo);
                message.Attachments.Add(attachment);
                message.Subject = options.Subject;

                if (!string.IsNullOrEmpty(options.ReplyTo))
                {
                    var adresses = options.ReplyTo.Split(new string[] { ",", ";" }, StringSplitOptions.None);
                    foreach (var adress in adresses)
                    {
                        message.ReplyToList.Add(adress.Trim());
                    }
                }

                foreach (var email in options.CC)
                {
                    if (email is MailAddress) message.CC.Add((MailAddress)email);
                    else message.CC.Add((string)email);
                }
                foreach (var email in options.BCC)
                {
                    if (email is MailAddress) message.Bcc.Add((MailAddress)email);
                    else message.Bcc.Add((string)email);
                }
                if (!string.IsNullOrEmpty(options.Body)) message.Body = options.Body;
                else message.Body = $"This Email contains the '{attachmentName}' report file.";

                var client = new SmtpClient(options.Host, options.Port);
                client.EnableSsl = options.EnableSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(options.UserName, options.Password);

                result = "0";
                client.Send(message);
            }
            catch (Exception e)
            {
                result = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<SendEmail><ErrorCode>An error occurred while sending Email.</ErrorCode><ErrorDescription>{0}</ErrorDescription></SendEmail>",
                    e.InnerException != null ? e.InnerException.Message : e.Message);
            }
            finally
            {
                var byteArray = Encoding.UTF8.GetBytes(result);
                stream = new MemoryStream(byteArray);
            }

            return stream;
        }

        #endregion

        #region Result

        #region Print

        public static StiWebActionResult PrintReportResult(StiRequestParams requestParams, StiReport report, StiExportSettings settings)
        {
            if (report == null) return StiWebActionResult.EmptyReportResult();
            if (!report.IsRendered)
            {
                try
                {
                    report.Render(false);
                }
                catch (Exception e)
                {
                    var message = e.Message;
                    if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
                        message += " " + e.InnerException.Message;

                    return new StiWebActionResult("ServerError:" + message);
                }
            }

            report.InvokePrinting();

            StiEditableFieldsHelper.ApplyEditableFieldsToReport(report, requestParams.Interaction.Editable);
            var stream = new MemoryStream();

            if (requestParams.Viewer.PrintAction == StiPrintAction.PrintPdf)
            {
                var pdfSettings = (StiPdfExportSettings)settings;
                pdfSettings.AutoPrintMode = StiPdfAutoPrintMode.Dialog;

                var pdfService = new StiPdfExportService();
                report.IsPrinting = true;
                pdfService.ExportPdf(report, stream, pdfSettings);
                report.IsPrinting = false;

                report.InvokePrinted();

                return new StiWebActionResult(stream, "application/pdf");
            }

            var htmlSettings = (StiHtmlExportSettings)settings;
            htmlSettings.AddPageBreaks = true;
            htmlSettings.UseWatermarkMargins = false;
            htmlSettings.ExportQuality = StiHtmlExportQuality.High;
            htmlSettings.ChartType = (requestParams.Viewer.ChartRenderType == StiChartRenderType.Image) ? StiHtmlChartType.Image : StiHtmlChartType.Vector;
            if (requestParams.Viewer.BookmarksPrint) htmlSettings.ExportBookmarksMode = StiHtmlExportBookmarksMode.All;
            else htmlSettings.ExportBookmarksMode = StiHtmlExportBookmarksMode.ReportOnly;

            switch (requestParams.Viewer.ReportDisplayMode)
            {
                case StiReportDisplayMode.FromReport:
                    htmlSettings.ExportMode = report?.HtmlPreviewMode == StiHtmlPreviewMode.Div ? StiHtmlExportMode.Div : StiHtmlExportMode.Table;
                    break;

                case StiReportDisplayMode.Table:
                    htmlSettings.ExportMode = StiHtmlExportMode.Table;
                    break;

                case StiReportDisplayMode.Div:
                    htmlSettings.ExportMode = StiHtmlExportMode.Div;
                    break;

                case StiReportDisplayMode.Span:
                    htmlSettings.ExportMode = StiHtmlExportMode.Span;
                    break;
            }

            switch (requestParams.Viewer.ImagesQuality)
            {
                case StiImagesQuality.Low:
                    htmlSettings.ImageResolution = 50f;
                    break;

                case StiImagesQuality.Normal:
                    htmlSettings.ImageResolution = 100f;
                    break;

                case StiImagesQuality.High:
                    htmlSettings.ImageResolution = 200f;
                    break;
            }

            var htmlService = new StiHtmlExportService();
            report.IsPrinting = true;
            htmlService.ExportHtml(report, stream, htmlSettings);
            report.IsPrinting = false;
            string htmlText = Encoding.UTF8.GetString(stream.ToArray());
            stream.Close();

            // Fix for IE compatibility mode
            htmlText = htmlText.Insert(htmlText.IndexOf("></meta>") + 8, "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"></meta>");

            report.InvokePrinted();

            return new StiWebActionResult(htmlText, "text/html");
        }

        #endregion

        #region Export
        public static StiWebActionResult ExportDashboardResult(StiRequestParams requestParams, StiReport report, IStiDashboardExportSettings exportSettings)
        {
            if (report == null)
                return StiWebActionResult.EmptyReportResult();

            var exportResult = ExportDashboard(requestParams, report, exportSettings);

            if (requestParams.ExportFormat == StiExportFormat.Document)
            {
                return new StiWebActionResult(exportResult, "text/xml", $"{StiReportHelper.GetReportFileName(report)}.mrt");
            }
            else
            {   
                var contentType = GetReportFileContentType(requestParams);
                var fileName = GetReportFileName(requestParams, report);
                var openAfterExport = requestParams.ExportSettings.Contains("OpenAfterExport") ? (bool)requestParams.ExportSettings["OpenAfterExport"] : false;

                return new StiWebActionResult(exportResult, contentType, fileName, !openAfterExport);
            }
        }


        public static StiWebActionResult ExportReportResult(StiRequestParams requestParams, StiReport report, StiExportSettings settings)
        {
            if (report == null)
                return StiWebActionResult.EmptyReportResult();

            if (requestParams.Viewer.ReportType == StiReportType.Report && !report.IsRendered)
            {
                try
                {
                    report.Render(false);
                }
                catch (Exception e)
                {
                    var message = e.Message;
                    if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
                        message += " " + e.InnerException.Message;

                    return new StiWebActionResult("ServerError:" + message);
                }
            }

            var exportResult = ExportReport(requestParams, report, settings);
            var contentType = GetReportFileContentType(requestParams);
            var fileName = GetReportFileName(requestParams, report);
            var openAfterExport = requestParams.ExportSettings.Contains("OpenAfterExport") ? (bool)requestParams.ExportSettings["OpenAfterExport"] : false;

            return new StiWebActionResult(exportResult, contentType, fileName, !openAfterExport);
        }

        #endregion

        #region Email

        public static StiWebActionResult EmailReportResult(StiRequestParams requestParams, StiReport report, StiExportSettings settings, StiEmailOptions options)
        {
            if (options == null) return StiWebActionResult.ErrorResult(requestParams, "An error occurred while sending Email: StiEmailOptions object cannot be null.");

            if (report == null) return StiWebActionResult.EmptyReportResult();
            if (!report.IsRendered)
            {
                try
                {
                    report.Render(false);
                }
                catch (Exception e)
                {
                    var message = e.Message;
                    if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
                        message += " " + e.InnerException.Message;

                    return StiWebActionResult.ErrorResult(requestParams, message);
                }
            }

            Stream stream = EmailReport(requestParams, report, settings, options);
            return new StiWebActionResult(stream, "text/plain");
        }

        #endregion

        #endregion
    }
}
