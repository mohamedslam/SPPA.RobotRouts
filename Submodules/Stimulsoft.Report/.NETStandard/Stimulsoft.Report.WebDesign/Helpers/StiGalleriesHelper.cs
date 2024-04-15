#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helpers;
using System.Text;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiGalleriesHelper
    {
        public static List<StiDataColumn> GetImageColumns(StiReport report)
        {
            var columns = report.Dictionary.DataSources.ToList().SelectMany(d => d.Columns.ToList());
            return columns.Where(c => c.Type == typeof(Image) || c.Type == typeof(byte[]) || c.Type == typeof(string)).ToList();
        }

        public static List<StiVariable> GetImageVariables(StiReport report)
        {
            return report.Dictionary.Variables.ToList().Where(v => v.ValueObject is Image).ToList();
        }

        public static List<StiResource> GetImageResources(StiReport report)
        {
            return report.Dictionary.Resources.ToList().Where(v => v.Type == StiResourceType.Image && v.Content != null).ToList();
        }

        public static List<StiDataColumn> GetRichTextColumns(StiReport report)
        {
            var columns = report.Dictionary.DataSources.ToList().SelectMany(d => d.Columns.ToList());
            return columns.Where(c => c.Type == typeof(byte[]) || c.Type == typeof(string)).ToList();
        }

        public static List<StiVariable> GetRichTextVariables(StiReport report)
        {
            return report.Dictionary.Variables.ToList().Where(v => v.ValueObject is string).ToList();
        }

        public static List<StiResource> GetRichTextResources(StiReport report)
        {
            return report.Dictionary.Resources.ToList().Where(v => (v.Type == StiResourceType.Rtf || v.Type == StiResourceType.Txt) && v.Content != null).ToList();
        }

        public static Image GetImageFromColumn(StiDataColumn column, StiReport report)
        {
            var columnPath = column.GetColumnPath();
            var datas = StiDataColumn.GetDataListFromDataColumn(report.Dictionary, columnPath, 3);

            foreach (var data in datas)
            {
                if (data == null) continue;

                var isSvg = false;

                if (data is byte[])
                    isSvg = StiImageHelper.IsSvg(data as byte[]);
                else if (data is string)
                {
                    var strContent = data as string;
                    if (StringExt.IsBase64String(strContent)) strContent = StiEncodingHelper.DecodeString(strContent);
                    isSvg = StiImageHelper.IsSvg(Encoding.UTF8.GetBytes(strContent));
                }

                if (!StiImageHelper.IsImage(data)) continue;

                try
                {
                    var image = StiImageHelper.GetImageFromObject(data, 200, 200, isSvg ? false : true, false);
                    if (image != null) return image;
                }
                catch
                {
                }
            }
            return null;
        }

        public static bool IsRtfColumn(StiDataColumn column, StiReport report)
        {
            var columnPath = column.GetColumnPath();
            var datas = StiDataColumn.GetDataListFromDataColumn(report.Dictionary, columnPath, 3);

            foreach (var data in datas)
            {
                if (data == null) continue;
                if (data is byte[]) return StiRtfHelper.IsRtfBytes(data as byte[]);
                if (data is string) return StiRtfHelper.IsRtfText(data as string);
            }
            return false;
        }

        public static string GetRichTextAsHtmlFromColumn(StiDataColumn column, StiReport report)
        {
            var columnPath = column.GetColumnPath();
            var datas = StiDataColumn.GetDataListFromDataColumn(report.Dictionary, columnPath, 3);

            foreach (var data in datas)
            {
                if (data == null) continue;
                try
                {
                    if (data is byte[] && StiRtfHelper.IsRtfBytes(data as byte[]))
                    {
                        return new RtfToHtmlConverter().ConvertRtfToHtml(StiBytesToStringConverter.ConvertBytesToString(data as byte[]));
                    }
                    if (data is string && StiRtfHelper.IsRtfText(data as string))
                    {
                        return new RtfToHtmlConverter().ConvertRtfToHtml(data as string);
                    }
                }
                catch
                {
                }
            }

            return String.Empty;
        }

        public static string GetHtmlTextFromText(string text)
        {
            return text.Replace("\r\n", "<br>").Replace("\n", "<br>");
        }

        public static string GetHtmlStringFromRichTextItem(StiReport report, Hashtable itemObject)
        {
            if (itemObject == null) return string.Empty;

            try
            {
                string type = itemObject["type"] as string;
                string name = itemObject["name"] as string;

                if (type == "StiResource")
                {
                    var resource = GetResource(report, name);
                    if (resource != null && resource.Content != null)
                    {
                        var text = StiBytesToStringConverter.ConvertBytesToString(resource.Content);
                        if (StiRtfHelper.IsRtfText(text))
                        {
                            text = new RtfToHtmlConverter().ConvertRtfToHtml(text);
                        }
                        else
                        {
                            text = GetHtmlTextFromText(text);
                        }

                        return StiEncodingHelper.Encode(text);
                    }
                }
                else if (type == "StiVariable")
                {
                    var variable = GetVariable(report, name);
                    if (variable != null && variable.ValueObject != null)
                    {
                        var text = variable.ValueObject as string;
                        if (StiRtfHelper.IsRtfText(text))
                        {
                            text = new RtfToHtmlConverter().ConvertRtfToHtml(text);
                        }
                        else
                        {
                            text = GetHtmlTextFromText(text);
                        }

                        return StiEncodingHelper.Encode(text);
                    }
                }
                else if (type == "StiDataColumn")
                {
                    var column = StiReportEdit.GetColumnFromColumnPath(name, report);
                    if (column != null)
                    {
                        report.Dictionary.Connect(true, new List<StiDataSource> { column.DataSource });
                        return StiEncodingHelper.Encode(GetRichTextAsHtmlFromColumn(column, report));
                    }
                }
                else if (itemObject["url"] != null)
                {
                    return StiEncodingHelper.Encode(StiBytesToStringConverter.ConvertBytesToString(StiBytesFromURL.Load(itemObject["url"] as string)));

                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }

        private static StiResource GetResource(StiReport report, string resourceName)
        {
            if (report == null || string.IsNullOrWhiteSpace(resourceName)) return null;

            resourceName = resourceName.ToLowerInvariant().Trim();

            return report.Dictionary.Resources.ToList()
                .FirstOrDefault(r => r.Name != null && r.Name.ToLowerInvariant().Trim() == resourceName);
        }

        private static StiVariable GetVariable(StiReport report, string variableName)
        {
            if (report == null || string.IsNullOrWhiteSpace(variableName)) return null;

            variableName = variableName.ToLowerInvariant().Trim();

            return report.Dictionary.Variables.ToList()
                .FirstOrDefault(v => v.Name != null && v.Name.ToLowerInvariant().Trim() == variableName);
        }
    }
}