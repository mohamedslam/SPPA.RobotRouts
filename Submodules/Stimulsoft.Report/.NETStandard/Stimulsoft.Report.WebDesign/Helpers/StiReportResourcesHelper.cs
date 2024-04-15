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
using System.Collections;
using Stimulsoft.Base;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Components;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Dictionary;
using System.Data;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using Stimulsoft.Report.Maps;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiReportResourcesHelper
    {
        internal static string GetReportThumbnailParameters(StiReport report, double zoom)
        {
            if (!report.IsRendered)
            {
                try
                {
                    report.Render(false);
                }
                catch
                {
                    report = new StiReport();
                }
            }
            if (report.RenderedPages.Count == 0) return string.Empty;

            report.RenderedWith = StiRenderedWith.Net;

            StiOptions.Export.Html.UseComponentStyleName = false;
            StiHtmlExportService service = new StiHtmlExportService();
            service.RenderAsDocument = false;
            service.Styles = new ArrayList();
            service.ClearOnFinish = false;
            service.RenderStyles = false;

            StiHtmlExportSettings settings = new StiHtmlExportSettings();
            settings.PageRange = new StiPagesRange(0);
            settings.Zoom = zoom;
            settings.ImageFormat = ImageFormat.Png;
            settings.ExportQuality = StiHtmlExportQuality.High;
            settings.ExportBookmarksMode = StiHtmlExportBookmarksMode.ReportOnly;
            settings.RemoveEmptySpaceAtBottom = false;
            settings.UseWatermarkMargins = true;
            settings.AddPageBreaks = false;

            MemoryStream stream = new MemoryStream();
            service.ExportHtml(report, stream, settings);
            string htmlText = Encoding.UTF8.GetString(stream.ToArray()).Substring(1);
            stream.Close();

            // Add Styles
            StringWriter writer = new StringWriter();
            StiHtmlTextWriter htmlWriter = new StiHtmlTextWriter(writer);
            service.HtmlWriter = htmlWriter;
            if (service.TableRender != null) service.TableRender.RenderStylesTable(true, false, false);
            htmlWriter.Flush();
            writer.Flush();
            string htmlTextStyles = writer.GetStringBuilder().ToString();
            writer.Close();
            service.Clear();

            StiPage page = report.RenderedPages[0];

            string pageMargins = string.Format("{0}px {1}px {2}px {3}px",
                Math.Round(report.Unit.ConvertToHInches(page.Margins.Top) * zoom),
                Math.Round(report.Unit.ConvertToHInches(page.Margins.Right) * zoom),
                Math.Round(report.Unit.ConvertToHInches(page.Margins.Bottom) * zoom),
                Math.Round(report.Unit.ConvertToHInches(page.Margins.Left) * zoom));

            string pageBackground = GetHtmlColor(StiBrush.ToColor(page.Brush));

            return string.Format("<div style='display: inline-block; border: 1px solid #c6c6c6; background:{2}; padding:{3}'><style type='text/css'>{1}</style>{0}</div>",
                htmlText, htmlTextStyles, pageBackground, pageMargins);
        }

        internal static string GetHtmlColor(Color color)
        {
            return "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");
        }

        private static bool IsPackedFile(byte[] content)
        {
            if (content == null) return false;

            return (
                (content[0] == 0x1F && content[1] == 0x8B && content[2] == 0x08) ||
                (content[0] == 0x50 && content[1] == 0x4B && content[2] == 0x03)
            );
        }

        public static string GetStringContentForJSFromResourceContent(StiResourceType resourceType, byte[] content)
        {
            string resultContent = null;

            if (resourceType == StiResourceType.Image)
            {
                resultContent = StiReportEdit.ImageToBase64(content);
            }
            else if (resourceType == StiResourceType.Rtf)
            {
                string contentText = StiGZipHelper.ConvertByteArrayToString(content);
                RtfToHtmlConverter rtfConverter = new RtfToHtmlConverter();
                resultContent = StiEncodingHelper.Encode(rtfConverter.ConvertRtfToHtml(contentText));
            }
            else if (resourceType == StiResourceType.Txt)
            {
                string contentText = StiGZipHelper.ConvertByteArrayToString(content);
                resultContent = StiEncodingHelper.Encode(contentText);
            }
            else if (resourceType == StiResourceType.Report || resourceType == StiResourceType.ReportSnapshot)
            {
                StiReport report = new StiReport();

                if (IsPackedFile(content))
                {
                    if (resourceType == StiResourceType.Report)
                        report.LoadPackedReport(content);
                    else
                        report.LoadPackedDocument(content);
                }
                else
                {
                    if (resourceType == StiResourceType.Report)
                        report.Load(content);
                    else
                        report.LoadDocument(content);
                }

                resultContent = StiEncodingHelper.Encode(GetReportThumbnailParameters(report, 0.2));
            }
            else if (StiReportResourceHelper.IsFontResourceType(resourceType))
            {
                return StiReportResourceHelper.GetBase64DataFromFontResourceContent(resourceType, content);
            }

            return resultContent;
        }

        public static void GetResourceContent(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiResource resource = report.Dictionary.Resources[(string)param["resourceName"]];
            if (resource != null)
            {
                callbackResult["resourceType"] = resource.Type;
                callbackResult["resourceName"] = resource.Name;
                callbackResult["resourceSize"] = resource.Content != null ? resource.Content.Length : 0;
                callbackResult["haveContent"] = resource.Content != null;

                if (resource.Content != null)
                {
                    callbackResult["resourceContent"] = StiReportResourcesHelper.GetStringContentForJSFromResourceContent(resource.Type, resource.Content);
                }
            }
        }

        public static void GetResourceText(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiResource resource = report.Dictionary.Resources[(string)param["resourceName"]];
            if (resource != null && resource.Content != null)
            {
                if (resource.Type == StiResourceType.Rtf)
                {
                    RtfToHtmlConverter rtfConverter = new RtfToHtmlConverter();

                    string htmlText = rtfConverter.ConvertRtfToHtml(Encoding.UTF8.GetString(resource.Content));
                    htmlText = htmlText.Replace("  ", "&nbsp;&nbsp;");
                    htmlText = htmlText.Replace("<SPAN />", "<BR>");

                    callbackResult["resourceText"] = StiEncodingHelper.Encode(htmlText);
                }
                else
                {
                    callbackResult["resourceText"] = Convert.ToBase64String(resource.Content);
                }
            }
        }

        public static void SetResourceText(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiResource resource = report.Dictionary.Resources[(string)param["resourceName"]];
            if (resource != null && param["resourceText"] != null)
            {
                if (resource.Type == StiResourceType.Rtf)
                {
                    HtmlToRtfConverter htmlToRtfConverter = new HtmlToRtfConverter();
                    string richText = htmlToRtfConverter.ConvertHtmlToRtf(StiEncodingHelper.DecodeString(param["resourceText"] as string));
                    resource.Content = Encoding.UTF8.GetBytes(richText);

                    callbackResult["resourceContent"] = StiReportResourcesHelper.GetStringContentForJSFromResourceContent(resource.Type, resource.Content);
                }
                else
                {
                    resource.Content = Convert.FromBase64String(param["resourceText"] as string);
                }
                if (resource.Type == StiResourceType.Map)
                {
                    StiMapLoader.DeleteAllCustomMaps();
                }
            }
        }

        public static string GetRtfResourceContentFromHtmlText(Hashtable param)
        {
            if (param["resourceText"] != null)
            {
                HtmlToRtfConverter htmlToRtfConverter = new HtmlToRtfConverter();
                string richText = htmlToRtfConverter.ConvertHtmlToRtf(StiEncodingHelper.DecodeString(param["resourceText"] as string));

                return "data:application/msword;base64," + StiEncodingHelper.Encode(richText);
            }

            return string.Empty;
        }

        public static void GetResourceViewData(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiResource resource = report.Dictionary.Resources[(string)param["resourceName"]];
            string contentBase64Str = param["resourceContent"] as string;
            byte[] resourceContent = contentBase64Str != null ? Convert.FromBase64String(contentBase64Str.Substring(contentBase64Str.IndexOf("base64,") + 7)) : null;
            if ((resource != null && resource.Content != null) || resourceContent != null)
            {
                DataSet dataSet = StiResourceArrayToDataSet.Get((StiResourceType)Enum.Parse(typeof(StiResourceType), param["resourceType"] as string), resourceContent ?? resource.Content);
                Hashtable resultTables = new Hashtable();

                foreach (DataTable dataTable in dataSet.Tables)
                {
                    ArrayList resultData = new ArrayList();
                    List<StiDataColumn> dictionaryColumns = new List<StiDataColumn>();

                    ArrayList captions = new ArrayList();
                    for (int k = 0; k < dataTable.Columns.Count; k++)
                    {
                        StiDataColumn dictionaryColumn = new StiDataColumn(dataTable.Columns[k].Caption, dataTable.Columns[k].DataType);
                        captions.Add(dataTable.Columns[k].Caption);
                        dictionaryColumns.Add(dictionaryColumn);
                    }
                    resultData.Add(captions);

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        ArrayList rowArray = new ArrayList();
                        resultData.Add(rowArray);
                        for (int k = 0; k < dataTable.Columns.Count; k++)
                        {
                            rowArray.Add(StiDictionaryHelper.GetViewDataItemValue(dataTable.Rows[i][k], dictionaryColumns[k]));
                        }
                    }

                    resultTables[dataTable.TableName] = resultData;
                }

                callbackResult["dataTables"] = resultTables;
            }
        }
                
        public static string ConvertBase64MetaFileToBase64Png(string fileContent)
        {
            byte[] imageBytes = Convert.FromBase64String(fileContent.Substring(fileContent.IndexOf("base64,") + 7));
            return StiReportEdit.GetBase64PngFromMetaFileBytes(imageBytes);
        }

        public static StiResourceType GetResourceTypeByFileName(string fileName)
        {
            fileName = fileName.ToLower();

            if (fileName.EndsWith(".csv")) return StiResourceType.Csv;
            else if (fileName.EndsWith(".dbf")) return StiResourceType.Dbf;
            else if (fileName.EndsWith(".xls") || fileName.EndsWith(".xlsx")) return StiResourceType.Excel;
            else if (fileName.EndsWith(".json")) return StiResourceType.Json;
            else if (fileName.EndsWith(".xml")) return StiResourceType.Xml;
            else if (fileName.EndsWith(".xsd")) return StiResourceType.Xsd;
            else if (fileName.EndsWith(".ttf")) return StiResourceType.FontTtf;
            else if (fileName.EndsWith(".otf")) return StiResourceType.FontOtf;
            else if (fileName.EndsWith(".woff")) return StiResourceType.FontWoff;
            else if (fileName.EndsWith(".ttc")) return StiResourceType.FontTtc;
            else if (fileName.EndsWith(".eot")) return StiResourceType.FontEot;
            else if (fileName.EndsWith(".rtf")) return StiResourceType.Rtf;
            else if (fileName.EndsWith(".txt")) return StiResourceType.Txt;
            else if (fileName.EndsWith(".mrt") || fileName.EndsWith(".mrz")) return StiResourceType.Report;
            else if (fileName.EndsWith(".mdc") || fileName.EndsWith(".mdz")) return StiResourceType.ReportSnapshot;
            else if (fileName.EndsWith(".gif") || fileName.EndsWith(".png") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".jpg") || fileName.EndsWith(".wmf") ||
                fileName.EndsWith(".bmp") || fileName.EndsWith(".tiff") || fileName.EndsWith(".ico") || fileName.EndsWith(".emf") || fileName.EndsWith(".svg"))
                return StiResourceType.Image;

            return StiResourceType.Image;
        }

        public static bool IsDataResourceType(StiResourceType resourceType)
        {
            return (resourceType == StiResourceType.Json ||
                    resourceType == StiResourceType.Csv ||
                    resourceType == StiResourceType.Xml ||
                    resourceType == StiResourceType.Dbf ||
                    resourceType == StiResourceType.Excel);
        }

        public static StiWebActionResult DownloadResource(StiReport report, Hashtable param)
        {
            var resourceName = param["resourceName"] as string;
            var resource = report.Dictionary.Resources[resourceName];

            if (resource != null && resource.Content != null)
            {
                using (var stream = new MemoryStream(resource.Content))
                {
                    return new StiWebActionResult(stream, "application/octet-stream", $"{resource.Name}{GetResourceFileExt(resource)}");
                }
            }

            return null;
        }

        public static StiWebActionResult DownloadImageContent(StiReport report, Hashtable param)
        {
            var imageBytes = StiReportEdit.Base64ToImageByteArray(param["imageData"] as string);

            if (imageBytes != null)
            {
                using (var stream = new MemoryStream(imageBytes))
                {
                    return new StiWebActionResult(stream, "application/octet-stream", $"image{GetImageExt(imageBytes)}");
                }
            }

            return null;
        }

        public static void MoveImageToResource(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var imageBytes = StiReportEdit.Base64ToImageByteArray(param["imageData"] as string);

            if (imageBytes != null)
            {
                var resource = new StiResource();
                resource.Type = StiResourceType.Image;
                resource.Content = imageBytes;
                resource.Name = param["resourceName"] as string;

                report.Dictionary.Resources.Add(resource);

                callbackResult["itemObject"] = StiDictionaryHelper.ResourceItem(resource, report);
                callbackResult["resources"] = StiDictionaryHelper.GetResourcesTree(report);

                StiDictionaryHelper.GetImagesGallery(report, param, callbackResult);
            }
        }

        private static string GetResourceFileExt(StiResource resource)
        {
            switch (resource.Type)
            {
                case StiResourceType.Csv:
                    return ".csv";

                case StiResourceType.Dbf:
                    return "dbf";

                case StiResourceType.Excel:
                    return ".xlsx";

                case StiResourceType.Json:
                    return ".json";

                case StiResourceType.Xml:
                    return ".xml";

                case StiResourceType.Xsd:
                    return ".xsd";

                case StiResourceType.FontTtf:
                    return ".ttf";

                case StiResourceType.FontOtf:
                    return ".otf";

                case StiResourceType.FontWoff:
                    return ".woff";

                case StiResourceType.FontTtc:
                    return ".ttc";

                case StiResourceType.FontEot:
                    return ".eot";

                case StiResourceType.Map:
                    return ".map";

                case StiResourceType.Rtf:
                    return ".rtf";

                case StiResourceType.Txt:
                    return ".txt";

                case StiResourceType.Pdf:
                    return ".pdf";

                case StiResourceType.Word:
                    return ".docx";

                case StiResourceType.Report:
                    if (StiReport.IsPackedFile(resource.Content)) return ".mrz";
                    else if (StiReport.IsEncryptedFile(resource.Content)) return ".mrx";
                    else return ".mrt";

                case StiResourceType.ReportSnapshot:
                    if (StiReport.IsPackedFile(resource.Content)) return ".mdz";
                    else if (StiReport.IsEncryptedFile(resource.Content)) return ".mdx";
                    else return ".mdc";

                case StiResourceType.Image:
                    return GetImageExt(resource.Content);

                default:
                    return "";
            }
        }

        private static string GetImageExt(byte[] imageBytes)
        {
            if (Report.Helpers.StiImageHelper.IsBmp(imageBytes)) return ".bmp";
            else if (Report.Helpers.StiImageHelper.IsJpeg(imageBytes)) return ".jpg";
            else if (Report.Helpers.StiImageHelper.IsGif(imageBytes)) return ".gif";
            else if (Report.Helpers.StiImageHelper.IsEmf(imageBytes)) return ".emf";
            else if (Report.Helpers.StiImageHelper.IsWmf(imageBytes)) return ".wmf";
            else if (Report.Helpers.StiImageHelper.IsTiff(imageBytes)) return ".tiff";
            else if (Report.Helpers.StiImageHelper.IsSvg(imageBytes)) return ".svg";
            else return ".png";
        }
    }
}