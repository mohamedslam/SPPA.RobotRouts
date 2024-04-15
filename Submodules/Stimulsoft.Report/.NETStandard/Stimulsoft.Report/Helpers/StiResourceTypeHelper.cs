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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dictionary;
using System.Drawing;
using System.Linq;
using System.IO;
using System;
using static Stimulsoft.Report.Func;

namespace Stimulsoft.Report.Helpers
{
    public static class StiResourceTypeHelper
    {
        public static StiResourceType? GetTypeFromExtension(string extension)
        {
            extension = extension.ToLowerInvariant();
            if (IsImageType(extension))return StiResourceType.Image;

            switch (extension)
            {
                case ".csv":
                    return StiResourceType.Csv;

                case ".dbf":
                    return StiResourceType.Dbf;

                case ".xls":
                case ".xlsx":
                    return StiResourceType.Excel;

                case ".json":
                    return StiResourceType.Json;

                case ".xml":
                    return StiResourceType.Xml;

                case ".xsd":
                    return StiResourceType.Xsd;

                case ".ttf":
                    return StiResourceType.FontTtf;

                case ".otf":
                    return StiResourceType.FontOtf;

                case ".woff":
                    return StiResourceType.FontWoff;

                case ".ttc":
                    return StiResourceType.FontTtc;

                case ".eot":
                    return StiResourceType.FontEot;

                case ".rtf":
                    return StiResourceType.Rtf;

                case ".txt":
                    return StiResourceType.Txt;

                case ".mrt":
                case ".mrz":
                    return StiResourceType.Report;

                case ".mdc":
                case ".mdz":
                    return StiResourceType.ReportSnapshot;

                case ".pdf":
                    return StiResourceType.Pdf;

                case ".doc":
                case ".docx":
                    return StiResourceType.Word;

                case ".map":
                    return StiResourceType.Map;

                case ".wkt":
                case ".geojson":
                    return StiResourceType.Gis;

                default:
                    return null;

            }
        }

        public static Bitmap GetResourceImage(StiResource resource)
        {
            return StiImageUtils.GetImage("Stimulsoft.Controls", $"Stimulsoft.Controls.Bmp.Resources.{resource.Type}{StiScaleUI.StepName}.png");
        }

        public static string GetResourceFileName(StiResource resource)
        {
            var name = string.IsNullOrWhiteSpace(resource.Alias) ? resource.Name : resource.Alias;
            var fileExt = GetExtension(resource.Type);
            var invalids = Path.GetInvalidFileNameChars();
            var fileName = string.Join("_", name.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            fileName = string.IsNullOrEmpty(fileExt) ? fileName : $"{fileName}.{fileExt}";
            return fileName;
        }

        public static string GetExtension(StiResourceType type)
        {
            switch (type)
            {
                case StiResourceType.Excel:
                    // todo: detect xls/xslx
                    return "xls";

                case StiResourceType.Image:
                    // todo: detect image type
                    return "png";

                case StiResourceType.Report:
                    return "mrt";

                case StiResourceType.ReportSnapshot:
                    return "mdc";

                case StiResourceType.Word:
                    // todo: detect doc/docx
                    return "doc";

                default:
                    return type.ToString().ToLower().Replace("font", "");
            }
        }

        public static string GetDataSourceIconName(StiResourceType type)
        {
            switch (type)
            {
                case StiResourceType.Csv:
                    return "Csv";

                case StiResourceType.Dbf:
                    return "Dbf";

                case StiResourceType.Excel:
                    return "Excel";

                case StiResourceType.Json:
                    return "Json";

                case StiResourceType.Xml:
                case StiResourceType.Xsd:
                    return "Xml";

                default:
                    return null;
            }
        }

        public static bool IsPossibleToConvertToDataSource(StiResourceType type)
        {
            return 
                type == StiResourceType.Csv ||
                type == StiResourceType.Dbf ||
                type == StiResourceType.Excel ||
                type == StiResourceType.Json ||
                type == StiResourceType.Gis ||
                type == StiResourceType.Xml ||
                type == StiResourceType.Xsd;
        }

        public static string GetFilter(StiResourceType type)
        {
            switch (type)
            {
                case StiResourceType.Csv:
                    return StiLocalization.Get("FileFilters", "CsvFiles");

                case StiResourceType.Dbf:
                    return StiLocalization.Get("FileFilters", "DbfFiles");

                case StiResourceType.Excel:
                    return StiLocalization.Get("FileFilters", "ExcelAllFiles");

                case StiResourceType.Json:
                    return StiLocalization.Get("FileFilters", "JsonFiles");

                case StiResourceType.Xml:
                    return StiLocalization.Get("FileFilters", "XmlFiles");

                case StiResourceType.Xsd:
                    return StiLocalization.Get("FileFilters", "DataSetXmlSchema");

                case StiResourceType.FontTtf:
                    return "(.ttf)|*.ttf";

                case StiResourceType.FontOtf:
                    return "(.otf)|*.otf";

                case StiResourceType.FontWoff:
                    return "(.woff)|*.woff";

                case StiResourceType.FontTtc:
                    return "(.ttc)|*.ttc";

                case StiResourceType.FontEot:
                    return "(.eot)|*.eot";

                case StiResourceType.Map:
                    return "(.map)|*.map";

                case StiResourceType.Rtf:
                    return StiLocalization.Get("FileFilters", "RtfFiles");

                case StiResourceType.Txt:
                    return StiLocalization.Get("FileFilters", "TxtFiles");

                case StiResourceType.Pdf:
                    return StiLocalization.Get("FileFilters", "PdfFiles");

                case StiResourceType.Word:
                    return 
                        $"{StiLocalization.Get("FileFilters", "Word2007Files")}|" +
                        $"{StiLocalization.Get("FileFilters", "WordFiles")}";

                case StiResourceType.Report:
                    return 
                        $"{StiLocalization.Get("FileFilters", "ReportFiles")}|" +
                        $"{StiLocalization.Get("FileFilters", "PackedReportFiles")}";

                case StiResourceType.ReportSnapshot:
                    return 
                        $"{StiLocalization.Get("FileFilters", "DocumentFiles")}|" +
                        $"{StiLocalization.Get("FileFilters", "PackedDocumentFiles")}";

                case StiResourceType.Image:
                    return StiFileDialogHelper.GetImageSaveFilter();

                default:
                    return null;
            }
        }

        public static bool IsImageType(string ext)
        {
            return IsExtensionType(StiFileDialogHelper.ImageExts, ext);
        }

        public static bool IsTextType(string ext)
        {
            return IsExtensionType(StiFileDialogHelper.TextExts, ext);
        }

        private static bool IsExtensionType(string[] exts, string ext)
        {
            ext = ext.StartsWith(".") ? ext.Substring(1) : ext;
            ext = ext.ToLowerInvariant();

            return exts.Any(e => e == ext);
        }
    }
}