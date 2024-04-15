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

using System.Linq;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Helpers
{
    public static class StiFileDialogHelper
    {
        #region Fields
        public static string[] DataExts = { "xlsx", "xls", "csv", "dbf", "json", "xml", "xsd", "geojson" };
        public static string[] ImageExts = { "gif", "png", "jpeg", "jpg", "bmp", "tiff", "ico", "emf", "wmf", "svg" };
        public static string[] ReportExts = { "mrt", "mrz", "mdc", "mdz" };
        public static string[] TextExts = { "rtf", "txt" };
        public static string[] DocumentExts = { "pdf", "doc", "docx" };
        public static string[] FontExts = { "ttf", "otf", "ttc" };
        public static string[] CustomMapExts = { "map" };
        public static string[] BlocklyExts = { "blockly" };
        #endregion

        #region Methods
        private static string GetFilterString(string[] extensions)
        {
            return string.Join(";", extensions.Select(s => "*." + s));
        }

        public static string[] GetAllResourceExtensions()
        {
            var list = DataExts.ToList();

            list.AddRange(ImageExts);
            list.AddRange(ReportExts);
            list.AddRange(TextExts);
            list.AddRange(DocumentExts);
            list.AddRange(FontExts);
            list.AddRange(CustomMapExts);

            return list.ToArray();
        }

        public static string GetImageFilter()
        {
            return $"{StiLocalization.Get("FileFilters", "AllImageFiles")}|{GetFilterString(ImageExts)}";
        }

        public static string GetImageSaveFilter()
        {
            return
                $"{StiLocalization.Get("FileFilters", "PngFiles")}|" +
                $"{StiLocalization.Get("FileFilters", "GifFiles")}|" +                
                $"{StiLocalization.Get("FileFilters", "JpegFiles")}|" +
                $"{StiLocalization.Get("FileFilters", "BmpFiles")}|" +
                $"{StiLocalization.Get("FileFilters", "TiffFiles")}|" +
                $"{"ICO image (.ico)|*.ico"}|" +
                $"{StiLocalization.Get("FileFilters", "EmfFiles")}|" +
                $"{"Windows Metafile Format (.wmf)|*.wmf"}";
        }

        public static string GetSvgImageSaveFilter()
        {
            return $"{StiLocalization.Get("FileFilters", "SvgFiles")}";
        }

        public static string GetAllResourceFilter()
        {
            return
                $"{StiLocalization.Get("FileFilters", "AllFiles")}|" +
                $"{GetFilterString(DataExts)};{GetFilterString(ImageExts)};{GetFilterString(ReportExts)};" +
                $"{GetFilterString(TextExts)};{GetFilterString(DocumentExts)};{GetFilterString(FontExts)};" +
                $"{GetFilterString(CustomMapExts)}";
        }

        public static string GetBlocklyFilter()
        {
            return $"Blockly|{GetFilterString(BlocklyExts)}";
        }
        #endregion
    }
}
