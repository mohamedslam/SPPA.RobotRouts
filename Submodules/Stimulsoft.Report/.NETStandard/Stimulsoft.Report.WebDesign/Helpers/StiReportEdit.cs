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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.ShapeTypes;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Maps;
using Stimulsoft.Report.Web.Helpers.Dashboards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Stimulsoft.Report.Dashboard.Styles;
using System.Xml;
using Stimulsoft.Report.SaveLoad;
using System.Threading;
using Stimulsoft.Report.Viewer;
using Stimulsoft.Data.Engine;
using System.Linq;
using StiDashboardHelper = Stimulsoft.Report.Web.Helpers.Dashboards.StiDashboardHelper;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Dashboard.Helpers;
using Stimulsoft.Base.Plans;
using Stimulsoft.Report.Import;
using System.ComponentModel;
using Stimulsoft.Report.Design.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;

#if NETSTANDARD
using Stimulsoft.System.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using FontFamily = Stimulsoft.Drawing.FontFamily;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiReportEdit
    {        
        #region Helpers methods
        public static void SetPropertyValue(StiReport report, string propertyName, object owner, object value)
        {
            if (string.IsNullOrEmpty(propertyName)) return;

            #region DBS
            if (owner is IStiTextElement && (propertyName == "Font" || propertyName == "HorAlignment" || propertyName == "ForeColor"))
            {
                var textElement = owner as IStiTextElement;
                var font = StiTextElementHelper.GetFontProperty(textElement);
                var horAlign = StiTextElementHelper.GetHorAlignmentProperty(textElement);
                var foreColors = StiTextElementHelper.GetForeColorsProperty(textElement);

                var fontAttrs = new Hashtable();
                fontAttrs["fontName"] = font.Name;
                fontAttrs["fontSize"] = font.Size;
                fontAttrs["fontBold"] = font.Bold;
                fontAttrs["fontItalic"] = font.Italic;
                fontAttrs["fontUnderline"] = font.Underline;
                fontAttrs["foreColors"] = foreColors;
                fontAttrs["horAlignment"] = horAlign.ToString();

                if (propertyName == "Font")
                {
                    font = StrToFont(value as string);
                    fontAttrs["fontName"] = font.Name;
                    fontAttrs["fontSize"] = font.Size;
                    fontAttrs["fontBold"] = font.Bold;
                    fontAttrs["fontItalic"] = font.Italic;
                    fontAttrs["fontUnderline"] = font.Underline;
                }

                if (propertyName == "HorAlignment")
                {
                    fontAttrs["horAlignment"] = value;
                }

                if (propertyName == "ForeColor")
                {
                    var color = StrToColor(value as string);
                    (textElement as IStiForeColor).ForeColor = color;

                    fontAttrs["foreColors"] = new ArrayList()
                    {
                        new Hashtable()
                        {
                            ["text"] = "",
                            ["color"] = color.ToHex()
                        }
                    };
                }

                StiTextElementHelper.SetFontProperties(textElement, fontAttrs);
                return;
            }

            if (owner is IStiSimpleBorder && propertyName == "Border")
            {
                (owner as IStiSimpleBorder).Border = StrToSimpleBorder((string)value);
                return;
            }

            if (owner is IStiDashboard && (propertyName == "Width" || propertyName == "Height"))
            {
                StiSettings.Load();
                if (StiSettings.GetBool("StiDesigner", "ScaleContent", true))
                {
                    var dashboard = owner as StiPage;
                    var prevWidth = dashboard.Width;
                    var prevHeight = dashboard.Height;
                    var val = StrToDouble((string)value);

                    if (propertyName == "Width")
                        dashboard.Width = val;
                    else
                        dashboard.Height = val;

                    StiDashboardScalingHelper.ApplyScalingToDashboard(dashboard, prevWidth, prevHeight);
                    return;
                }
            }
            #endregion

            PropertyInfo property = null;
            string[] propertyArray = propertyName.Split('.');
            var childPropertyName = propertyArray[propertyArray.Length - 1];
            object parentOwner = null;

            if (propertyArray.Length == 1)
                property = owner.GetType().GetProperty(propertyName);
            else
            {
                for (int i = 0; i < propertyArray.Length; i++)
                {
                    parentOwner = owner;
                    property = owner.GetType().GetProperty(propertyArray[i]);

                    if (property != null)
                        owner = property.GetValue(owner, null);
                    else
                        return;
                }
                owner = parentOwner;
            }

            if (property != null)
            {
                //Decode string from Base64
                if (value is string && ((string)value).StartsWith("Base64Code;"))
                {
                    value = StiEncodingHelper.DecodeString(((string)value).Replace("Base64Code;", ""));
                }

                #region string, bool
                if (property.PropertyType == typeof(string) || property.PropertyType == typeof(bool))
                {
                    property.SetValue(owner, value, null);
                }
                #endregion

                #region int
                else if (property.PropertyType == typeof(int))
                {
                    property.SetValue(owner, StrToInt((string)value), null);
                }
                #endregion

                #region double
                else if (property.PropertyType == typeof(double))
                {
                    property.SetValue(owner, StrToDouble((string)value), null);
                }
                #endregion

                #region float
                else if (property.PropertyType == typeof(float))
                {
                    property.SetValue(owner, (float)StrToDouble((string)value), null);
                }
                #endregion

                #region decimal
                else if (property.PropertyType == typeof(decimal))
                {
                    property.SetValue(owner, (decimal)StrToDouble((string)value), null);
                }
                #endregion

                #region StiPenStyle
                else if (property.PropertyType == typeof(StiPenStyle))
                {
                    property.SetValue(owner, (StiPenStyle)StrToInt((string)value), null);
                }
                #endregion                

                #region PaperKind
                else if (property.PropertyType == typeof(PaperKind))
                {
                    property.SetValue(owner, (PaperKind)StrToInt((string)value), null);
                }
                #endregion   

                #region StiGroupSortDirection
                else if (property.PropertyType == typeof(StiGroupSortDirection))
                {
                    property.SetValue(owner, (StiGroupSortDirection)StrToInt((string)value), null);
                }
                #endregion 

                #region StiGroupSummaryType
                else if (property.PropertyType == typeof(StiGroupSummaryType))
                {
                    property.SetValue(owner, (StiGroupSummaryType)StrToInt((string)value), null);
                }
                #endregion 

                #region Color
                else if (property.PropertyType == typeof(Color))
                {
                    property.SetValue(owner, StrToColor((string)value), null);
                }
                #endregion

                #region Colors
                else if (property.PropertyType == typeof(Color[]))
                {
                    SetColorsCollectionProperty(owner, childPropertyName, value as ArrayList);
                }
                #endregion

                #region StiBrush
                else if (property.PropertyType == typeof(StiBrush))
                {
                    property.SetValue(owner, StrToBrush((string)value), null);
                }
                #endregion

                #region StiSimpleBorder
                else if (property.PropertyType == typeof(StiSimpleBorder))
                {
                    property.SetValue(owner, StrToSimpleBorder((string)value), null);
                }
                #endregion

                #region StiBorder
                else if (property.PropertyType == typeof(StiBorder))
                {
                    property.SetValue(owner, StrToBorder((string)value), null);
                }
                #endregion                

                #region StiMargins
                else if (property.PropertyType == typeof(StiMargins))
                {
                    property.SetValue(owner, StrToMargins(value as string), null);
                }
                #endregion

                #region StiPadding
                else if (property.PropertyType == typeof(StiPadding))
                {
                    property.SetValue(owner, StrToPadding(value as string), null);
                }
                #endregion

                #region StiMargin
                else if (property.PropertyType == typeof(StiMargin))
                {
                    property.SetValue(owner, StrToMargin(value as string), null);
                }
                #endregion

                #region StiCornerRadius
                else if (property.PropertyType == typeof(StiCornerRadius))
                {
                    property.SetValue(owner, StrToCornerRadius((string)value), null);
                }
                #endregion

                #region Font
                else if (property.PropertyType == typeof(Font))
                {
                    property.SetValue(owner, StrToFont((string)value), null);
                }
                #endregion

                #region Size
                else if (property.PropertyType == typeof(Size))
                {
                    Size propertyValue = (Size)property.GetValue(owner, null);

                    string[] sizes = ((string)value).Split(';');
                    if (sizes.Length == 2)
                    {
                        propertyValue.Width = StrToInt(sizes[0]);
                        propertyValue.Height = StrToInt(sizes[1]);
                    }
                    property.SetValue(owner, propertyValue, null);
                }
                #endregion

                #region SizeD
                else if (property.PropertyType == typeof(SizeD))
                {
                    SizeD propertyValue = (SizeD)property.GetValue(owner, null);

                    string[] sizes = ((string)value).Split(';');
                    if (sizes.Length == 2)
                    {
                        propertyValue.Width = StrToDouble(sizes[0]);
                        propertyValue.Height = StrToDouble(sizes[1]);
                    }
                    property.SetValue(owner, propertyValue, null);
                }
                #endregion

                #region Point
                else if (property.PropertyType == typeof(Point))
                {
                    var coords = ((string)value).Split(';');
                    if (coords.Length == 2)
                    {
                        var point = new Point(StrToInt(coords[0]), (StrToInt(coords[1])));
                        property.SetValue(owner, point, null);
                    }
                }
                #endregion

                #region StiPage
                else if (property.PropertyType == typeof(StiPage))
                {   
                    StiPage page = report.Pages[(string)value];
                    property.SetValue(owner, page, null);
                }
                #endregion

                #region StiExpression
                else if (property.PropertyType.Name.StartsWith("Sti") && property.PropertyType.Name.EndsWith("Expression") && property.PropertyType.IsClass)
                {
                    var propertyExpression = property.GetValue(owner, null);
                    if (propertyExpression != null)
                    {
                        var propertyValue = propertyExpression.GetType().GetProperty("Value");
                        if (propertyValue != null)
                        {
                            propertyValue.SetValue(propertyExpression, (string)value, null);
                        }
                    }
                }
                #endregion

                #region Enum
                else if (property.PropertyType.IsEnum)
                {
                    property.SetValue(owner, Enum.Parse(property.PropertyType, (string)value), null);
                }
                #endregion

                #region StiDataRelaton
                else if (property.PropertyType == typeof(StiDataRelation))
                {
                    SetDataRelationProperty(owner, (string)value);
                }
                #endregion

                #region StiDataSource
                else if (property.PropertyType == typeof(StiDataSource))
                {
                    SetDataSourceProperty(owner, (string)value);
                }
                #endregion

                #region StiBusinessObject
                else if (property.PropertyType == typeof(StiBusinessObject))
                {
                    SetBusinessObjectProperty(owner, (string)value);
                }
                #endregion

                #region MasterComponent
                else if (property.PropertyType == typeof(StiComponent))
                {
                    SetMasterComponentProperty(owner, (string)value);
                }
                #endregion

                #region StiFiltersCollection
                else if (property.PropertyType == typeof(StiFiltersCollection))
                {
                    SetFiltersProperty(owner, (Hashtable)value);
                }
                #endregion

                #region StiChartFiltersCollection
                else if (owner is IStiSeries && propertyName == "Filters")
                {
                    StiChartHelper.SetFiltersValue((owner as IStiSeries).Filters, value as ArrayList);
                }
                #endregion

                #region StiChartConditionsCollection
                else if (owner is IStiSeries && propertyName == "Conditions")
                {
                    StiChartHelper.SetConditionsValue((owner as IStiSeries).Conditions, value as ArrayList);
                }
                #endregion

                #region StiTrendLinesCollection
                else if (owner is IStiSeries && propertyName == "TrendLines")
                {
                    StiChartHelper.SetTrendLinesValue((owner as IStiSeries).TrendLines, value as ArrayList);
                }
                #endregion

                #region Sort
                else if (property.PropertyType == typeof(string[]) && propertyName == "Sort")
                {
                    SetSortDataProperty((StiComponent)owner, (string)value);
                }
                #endregion

                #region Image
                if (property.PropertyType == typeof(Image))
                {
                    string propertyValue = value as string;
                    if (!string.IsNullOrEmpty(propertyValue) && propertyValue.StartsWith("data:image"))
                    {
                        property.SetValue(owner, Base64ToImage(propertyValue), null);
                    }
                    else
                    {
                        property.SetValue(owner, null, null);
                    }
                }
                #endregion

                #region ImageByteArray
                if (property.PropertyType == typeof(byte[]) && (propertyName == "CustomIcon" || propertyName == "Icon" || propertyName == "Image.Image"))
                {
                    property.SetValue(owner, !String.IsNullOrEmpty(value as string) ? Base64ToImageByteArray(value as string) : null, null);
                }
                #endregion

                #region Conditions
                else if (property.PropertyType == typeof(StiConditionsCollection))
                {
                    SetConditionsProperty(owner, value, report);
                }
                #endregion

                #region StyleConditions
                else if (property.PropertyType == typeof(StiStyleConditionsCollection) && owner is StiBaseStyle)
                {
                    StiStylesHelper.SetStyleConditionsProprty(owner as StiBaseStyle, value as ArrayList);
                }
                #endregion

                #region Interaction
                else if (property.PropertyType == typeof(StiInteraction))
                {
                    SetInteractionProperty(owner, value);
                }
                #endregion

                #region TextFormat
                else if (property.PropertyType == typeof(StiFormatService))
                {
                    SetTextFormatProperty(owner, value);
                }
                #endregion

                #region StiSeriesLabelsValueType?
                else if (property.PropertyType == typeof(StiSeriesLabelsValueType?))
                {
                    if ((string)value == "Auto")
                        property.SetValue(owner, null, null);
                    else
                        property.SetValue(owner, Enum.Parse(typeof(StiSeriesLabelsValueType), (string)value), null);
                }
                #endregion

                #region StiFontIcons?
                else if (property.PropertyType == typeof(StiFontIcons?))
                {
                    if (string.IsNullOrEmpty(value as string))
                        property.SetValue(owner, null, null);
                    else
                        property.SetValue(owner, Enum.Parse(typeof(StiFontIcons), (string)value), null);
                }
                #endregion

                #region Styles
                else if (property.PropertyType == typeof(StiStylesCollection))
                {
                    SetStylesProperty(owner, value);
                }
                #endregion     
            }
        }

        public static object GetPropertyValue(string propertyName, object owner, bool checkBrowsable = false)
        {
            if (owner == null) return null;
            PropertyInfo property = null;
            string[] propertyArray = propertyName.Split('.');
            object parentOwner = null;

            if (propertyArray.Length == 1)
                property = owner.GetType().GetProperty(propertyName);
            else
            {
                for (int i = 0; i < propertyArray.Length; i++)
                {
                    parentOwner = owner;
                    property = owner.GetType().GetProperty(propertyArray[i]);
                    if (property != null)
                    {
                        owner = property.GetValue(owner, null);
                    }
                    else
                        return null;
                }
                owner = parentOwner;
            }
                        
            if (property != null)
            {
                if (checkBrowsable && PropertyIsNotBrowsable(property))
                    return null;

                var value = property.GetValue(owner, null);
                if (value == null) return string.Empty;

                if (value is string) return "Base64Code;" + StiEncodingHelper.Encode((string)value);
                if (value is bool) return (bool)value;
                if (value is double || value is float) return value.ToString().Replace(",", ".");
                if (value is int) return value.ToString();
                if (value is StiPenStyle) return ((int)value).ToString();
                if (value is StiExpression)
                    return "Base64Code;" + StiEncodingHelper.Encode(((StiExpression)value).Value);
                if (value is StiArgumentExpression) return "Base64Code;" + StiEncodingHelper.Encode(((StiArgumentExpression)value).Value);
                if (value is StiListOfArgumentsExpression) return "Base64Code;" + StiEncodingHelper.Encode(((StiListOfArgumentsExpression)value).Value);
                if (value is Color) return GetStringFromColor((Color)value);
                if (value is Color[]) return GetColorsCollectionProperty((Color[])value);
                if (value is StiBrush) return BrushToStr((StiBrush)value);
                if (value is StiBorder) return BorderToStr((StiBorder)value);
                if (value is Font) return FontToStr((Font)value);
                if (value is Size) return ((Size)value).Width + ";" + ((Size)value).Height;
                if (value is StiDataRelation) return ((StiDataRelation)value).NameInSource;
                if (value is StiDataSource) return ((StiDataSource)value).Name;
                if (value is StiBusinessObject) return ((StiBusinessObject)value).GetFullName();
                if (value is StiFiltersCollection) return GetFiltersProperty(owner);
                if (value is string[] && propertyName == "Sort") return GetSortDataProperty((StiComponent)owner);
                if (value is Image) return (value != null) ? ImageToBase64((Image)value) : String.Empty;
                if (value is StiMargins) return MarginsToStr((StiMargins)value);
                if (value is StiMargin) return MarginToStr((StiMargin)value);
                if (value is StiPadding) return PaddingToStr((StiPadding)value);
                if (value is StiCornerRadius) return CornerRadiusToStr((StiCornerRadius)value);
                if (value is StiEvent) return ((StiEvent)value).Script;

                return value.ToString();
            }

            return null;
        }

        private static bool PropertyIsNotBrowsable(PropertyInfo property)
        {
            var browsableAttribute = property.GetCustomAttributes(typeof(BrowsableAttribute), false)?.FirstOrDefault() as BrowsableAttribute;
            if (browsableAttribute != null && !browsableAttribute.Browsable)
            {
                return true;
            }

            return false;
        }

        public static Hashtable GetFiltersProperty(object obj)
        {
            Hashtable properties = new Hashtable();
            properties["filterData"] = GetFilterDataProperty(obj);
            properties["filterOn"] = GetFilterOnProperty(obj);
            properties["filterMode"] = GetFilterModeProperty(obj);

            return properties;
        }

        public static void SetFiltersProperty(object obj, Hashtable value)
        {
            SetFilterDataProperty(obj, value["filterData"]);
            SetFilterOnProperty(obj, value["filterOn"]);
            SetFilterModeProperty(obj, value["filterMode"]);
        }

        public static string LowerFirstChar(string text)
        {
            return text[0].ToString().ToLower() + text.Substring(1);
        }

        public static string UpperFirstChar(string text)
        {
            string[] textArray = text.Split('.');
            if (textArray.Length == 1)
            {
                return text[0].ToString().ToUpper() + text.Substring(1);
            }
            else
            {
                string result = string.Empty;
                foreach (string word in textArray)
                {
                    if (result != string.Empty) result += ".";
                    result += word[0].ToString().ToUpper() + word.Substring(1);
                }
                return result;
            }
        }

        public static byte[] Base64ToImageByteArray(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;
            return Convert.FromBase64String(base64String.IndexOf("base64,") >= 0 ? base64String.Substring(base64String.IndexOf("base64,") + 7) : base64String);
        }

        public static Image Base64ToImage(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;
            return StiImageConverter.StringToImage(base64String.IndexOf("base64,") >= 0 ? base64String.Substring(base64String.IndexOf("base64,") + 7) : base64String);
        }

        public static string ImageToBase64(byte[] bytes)
        {
            if (OleUnit.IsOleHeader(bytes))
            {
                //remove ole-link from array
                var objHeader = new OleUnit.ObjectHeader(bytes);
                var tempData = new byte[bytes.Length - objHeader.HeaderLen];
                Array.Copy(bytes, objHeader.HeaderLen, tempData, 0, bytes.Length - objHeader.HeaderLen);
                bytes = tempData;
            }

            var mimeType = "data:image;base64,";

            if (Report.Helpers.StiImageHelper.IsPng(bytes))
            {
                mimeType = "data:image/png;base64,";
            }
            else if (Report.Helpers.StiImageHelper.IsMetafile(bytes))
            {
                mimeType = "data:image/x-wmf;base64,";
            }
            else if (Report.Helpers.StiImageHelper.IsBmp(bytes))
            {
                mimeType = "data:image/bmp;base64,";
            }
            else if (Report.Helpers.StiImageHelper.IsJpeg(bytes))
            {
                mimeType = "data:image/jpeg;base64,";
            }
            else if (Report.Helpers.StiImageHelper.IsGif(bytes))
            {
                mimeType = "data:image/gif;base64,";
            }
            else if (Base.Helpers.StiSvgHelper.IsSvg(bytes))
            {
                mimeType = "data:image/svg+xml;base64,";
            }

            return mimeType + Convert.ToBase64String(bytes);
        }

        public static string ImageToBase64(Image image)
        {
            return ImageToBase64(StiImageConverter.ImageToBytes(image));
        }

        public static string MarginToStr(StiMargin margin)
        {
            return $"{margin.Left};{margin.Top};{margin.Right};{margin.Bottom}";
        }

        public static string MarginsToStr(StiMargins margins)
        {
            return $"{margins.Left};{margins.Top};{margins.Right};{margins.Bottom}";
        }

        public static string PaddingToStr(StiPadding padding)
        {
            return $"{padding.Left};{padding.Top};{padding.Right};{padding.Bottom}";
        }

        public static string CornerRadiusToStr(StiCornerRadius cornerRadius)
        {
            return $"{cornerRadius.TopLeft};{cornerRadius.TopRight};{cornerRadius.BottomRight};{cornerRadius.BottomLeft}";
        }

        public static string FontToStr(Font font)
        {
            string fontStr = string.Empty;

            if (font != null)
            {
                //OriginalFontName - temporarily for fonts from resources
                string fontName = !string.IsNullOrEmpty(font.OriginalFontName) ? font.OriginalFontName : font.Name;

                fontStr = string.Format("{0}!{1}!{2}!{3}!{4}!{5}",
                    fontName,
                    font.Size.ToString(),
                    font.Bold ? "1" : "0",
                    font.Italic ? "1" : "0",
                    font.Underline ? "1" : "0",
                    font.Strikeout ? "1" : "0");

            }

            return fontStr;
        }

        public static string RectToStr(RectangleD rect)
        {
            return string.Format("{0}!{1}!{2}!{3}",
                DoubleToStr(rect.Left),
                DoubleToStr(rect.Top),
                DoubleToStr(rect.Width),
                DoubleToStr(rect.Height));
        }

        public static string BrushToStr(StiBrush brush)
        {
            string brushStr = "none";
            
            if (brush is StiStyleBrush)
            {
                return "isStyleBrush";
            }
            else if (brush is StiDefaultBrush)
            {
                return "isDefaultBrush";
            }
            else if (brush is StiEmptyBrush)
            {
                return "0";
            }
            else if (brush is StiSolidBrush)
            {
                var solid = brush as StiSolidBrush;
                return string.Format("1!{0}",
                    GetStringFromColor(solid.Color));
            }
            else if (brush is StiHatchBrush)
            {
                var hatch = brush as StiHatchBrush;
                return string.Format("2!{0}!{1}!{2}",
                    GetStringFromColor(hatch.ForeColor),
                    GetStringFromColor(hatch.BackColor),
                    ((int)hatch.Style).ToString());
            }
            else if (brush is StiGradientBrush)
            {
                var gradient = brush as StiGradientBrush;
                return string.Format("3!{0}!{1}!{2}",
                    GetStringFromColor(gradient.StartColor),
                    GetStringFromColor(gradient.EndColor),
                    gradient.Angle.ToString());
            }
            else if (brush is StiGlareBrush)
            {
                var glare = brush as StiGlareBrush;
                return string.Format("4!{0}!{1}!{2}!{3}!{4}",
                    GetStringFromColor(glare.StartColor),
                    GetStringFromColor(glare.EndColor),
                    glare.Angle.ToString(),
                    glare.Focus.ToString(),
                    glare.Scale.ToString());
            }
            else if (brush is StiGlassBrush)
            {
                var glass = brush as StiGlassBrush;
                return string.Format("5!{0}!{1}!{2}!{3}",
                    GetStringFromColor(glass.Color),
                    glass.Blend.ToString(),
                    glass.DrawHatch ? "1" : "0",
                    GetStringFromColor(StiColorUtils.Light(glass.Color, (byte)(64 * glass.Blend))));
            }

            return brushStr;
        }

        public static string BorderToStr(StiBorder border)
        {
            var borderStr = string.Empty;

            if (border != null)
            {
                borderStr = string.Format("{0},{1},{2},{3}!{4}!{5}!{6}!{7}!{8}!{9}!{10}",
                    border.IsLeftBorderSidePresent ? "1" : "0",
                    border.IsTopBorderSidePresent ? "1" : "0",
                    border.IsRightBorderSidePresent ? "1" : "0",
                    border.IsBottomBorderSidePresent ? "1" : "0",
                    border.Size,
                    GetStringFromColor(border.Color),
                    (int)border.Style,
                    border.DropShadow ? "1" : "0",
                    border.ShadowSize,
                    StiEncodingHelper.Encode(BrushToStr(border.ShadowBrush)),
                    border.Topmost ? "1" : "0"
                );

                if (border is StiAdvancedBorder)
                {
                    var advBorder = border as StiAdvancedBorder;
                    borderStr += string.Format("!{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11}",
                        advBorder.LeftSide.Size,
                        GetStringFromColor(advBorder.LeftSide.Color),
                        (int)advBorder.LeftSide.Style,
                        advBorder.TopSide.Size,
                        GetStringFromColor(advBorder.TopSide.Color),
                        (int)advBorder.TopSide.Style,
                        advBorder.RightSide.Size,
                        GetStringFromColor(advBorder.RightSide.Color),
                        (int)advBorder.RightSide.Style,
                        advBorder.BottomSide.Size,
                        GetStringFromColor(advBorder.BottomSide.Color),
                        (int)advBorder.BottomSide.Style);
                }
            }

            return borderStr;
        }

        public static string SimpleBorderToStr(StiSimpleBorder border)
        {
            var borderStr = string.Empty;

            if (border != null)
            {
                borderStr = string.Format("{0},{1},{2},{3}!{4}!{5}!{6}",
                border.IsLeftBorderSidePresent ? "1" : "0",
                border.IsTopBorderSidePresent ? "1" : "0",
                border.IsRightBorderSidePresent ? "1" : "0",
                border.IsBottomBorderSidePresent ? "1" : "0",
                border.Size,
                GetStringFromColor(border.Color),
                (int)border.Style);
            }

            return borderStr;
        }

        public static string GetStringFromColor(Color color)
        {
            if (color.A == 0)
                return "transparent";
            else
            {
                if (color.A == 255) return string.Format("{0},{1},{2}", color.R.ToString(), color.G.ToString(), color.B.ToString());
                else return string.Format("{0},{1},{2},{3}", color.A.ToString(), color.R.ToString(), color.G.ToString(), color.B.ToString());
            }
        }

        public static void AddComponentToPage(StiComponent component, StiPage currentPage)
        {
            currentPage.Components.Add(component);
            component.Select();
        }

        public static string GetParentName(StiComponent comp)
        {
            return (comp == null || comp.Parent == null) ? string.Empty : comp.Parent.Name;
        }

        public static int GetParentIndex(StiComponent comp)
        {
            int index = 0;

            if (comp is StiCrossLinePrimitive)
            {
                int index1 = GetParentIndex(((StiCrossLinePrimitive)comp).GetStartPoint());
                int index2 = GetParentIndex(((StiCrossLinePrimitive)comp).GetStartPoint());
                return Math.Max(index1, index2);
            }

            StiComponent parent = comp != null ? comp.Parent : null;
            while (true)
            {
                if (parent == null || parent is StiPage)
                    return index;

                parent = parent.Parent;
                index++;
            }
        }

        public static int GetComponentIndex(StiComponent component)
        {
            StiContainer parent = component.Parent as StiContainer;
            List<StiComponent> componentsList = parent.GetComponentsList();
            int index = 0;
            foreach (StiComponent comp in componentsList)
            {
                if (comp == component) return index;
                index++;
            }

            return index;
        }

        public static string GetAllChildComponents(StiComponent component)
        {
            string allChildComponents = string.Empty;
            if (component == null) return allChildComponents;
            StiPage currentPage = component.Page;
            if (currentPage == null) return allChildComponents;
            StiComponentsCollection components = currentPage.GetComponents();

            foreach (StiComponent otherComponent in components)
            {
                if (otherComponent is StiCrossLinePrimitive)
                {
                    StiStartPointPrimitive startPoint = ((StiCrossLinePrimitive)otherComponent).GetStartPoint();
                    StiEndPointPrimitive endPoint = ((StiCrossLinePrimitive)otherComponent).GetEndPoint();
                    if ((startPoint?.Parent != null && startPoint?.Parent == component) || (endPoint?.Parent != null && endPoint?.Parent == component))
                        allChildComponents += otherComponent.Name + ",";
                }
                else if(otherComponent.Parent != null && otherComponent.Parent == component)
                {
                    allChildComponents += otherComponent.Name + ",";
                }
            }
            if (allChildComponents != "")
                allChildComponents = allChildComponents.Substring(0, allChildComponents.Length - 1);

            return allChildComponents;
        }

        public static Hashtable GetPropsRebuildPage(StiReport report, StiPage currentPage)
        {
            Hashtable props = new Hashtable();
            StiComponentsCollection components = currentPage.GetComponents();

            foreach (StiComponent component in components)
            {
                Hashtable compProperties = new Hashtable();
                props[component.Name] = compProperties;
                compProperties["componentRect"] = GetComponentRect(component);
                compProperties["parentName"] = GetParentName(component);
                compProperties["parentIndex"] = GetParentIndex(component).ToString();
                compProperties["componentIndex"] = GetComponentIndex(component).ToString();
                compProperties["childs"] = GetAllChildComponents(component);
                compProperties["clientLeft"] = DoubleToStr(component.Left);
                compProperties["clientTop"] = DoubleToStr(component.Top);
            }

            return props;
        }

        public static Hashtable GetAllDbsElementsSvgContents(StiReport report)
        {
            Hashtable pages = new Hashtable();
            foreach (StiPage page in report.Pages)
            {
                if (!page.IsDashboard) continue;
                Hashtable svgContents = new Hashtable();
                pages[page.Name] = svgContents;

                foreach (StiComponent component in page.GetComponents())
                {
                    if (!(component is IStiElement)) continue;
                    svgContents[component.Name] = GetSvgContent(component);
                }   
            }

            return pages;
        }

        public static Hashtable GetPageIndexes(StiReport report)
        {
            Hashtable pageIndexes = new Hashtable();
            for (int index = 0; index < report.Pages.Count; index++)
                pageIndexes[report.Pages[index].Name] = index.ToString();

            return pageIndexes;
        }

        public static void SetComponentRectWithOffset(StiComponent comp, RectangleD newCompRect, string command, string resizeType, Hashtable compProps)
        {
            StiPage currentPage = comp.Page;

            RectangleD currCompRect = currentPage.Unit.ConvertFromHInches(comp.GetPaintRectangle());

            //set not selected all components            
            foreach (StiComponent component in currentPage.GetComponents())
            {
                component.IsSelected = false;
            }

            RectangleD rect = new RectangleD();

            if (!string.IsNullOrEmpty(resizeType) && resizeType.StartsWith("Multi"))
            {
                rect = new RectangleD(
                    currCompRect.Left - newCompRect.Left,
                    currCompRect.Top - newCompRect.Top,
                    newCompRect.Width - currCompRect.Width,
                    newCompRect.Height - currCompRect.Height
                ).AlignToGrid(currentPage.GridSize, currentPage.Report.Info.AlignToGrid);
            }
            else if (command == "MoveComponent")
            {
                rect = new RectangleD(
                    currCompRect.Left - newCompRect.Left,
                    currCompRect.Top - newCompRect.Top,
                    0, 0
                ).AlignToGrid(currentPage.GridSize, currentPage.Report.Info.AlignToGrid);
            }
            else if (command == "ResizeComponent")
            {
                bool invertWidth = compProps["invertWidth"] != null && (bool)compProps["invertWidth"];
                bool invertHeight = compProps["invertHeight"] != null && (bool)compProps["invertHeight"];

                switch (resizeType)
                {
                    case "Left":
                        {
                            rect = new RectangleD(
                                invertWidth ? -(currCompRect.Width + newCompRect.Width) : currCompRect.Left - newCompRect.Left, 0,
                                invertWidth ? -(currCompRect.Width + newCompRect.Width) : currCompRect.Left - newCompRect.Left, 0
                            ).AlignToGrid(currentPage.GridSize, currentPage.Report.Info.AlignToGrid);
                            break;
                        }
                    case "Right":
                    case "ResizeWidth":
                        {
                            rect = new RectangleD(
                                0, 0,
                                invertWidth ? -(currCompRect.Width + newCompRect.Width) : newCompRect.Width - currCompRect.Width, 0
                            ).AlignToGrid(currentPage.GridSize, currentPage.Report.Info.AlignToGrid);
                            break;
                        }
                    case "Top":
                        {
                            rect = new RectangleD(
                                0, invertHeight ? -(currCompRect.Height + newCompRect.Height) : currCompRect.Top - newCompRect.Top,
                                0, invertHeight ? -(currCompRect.Height + newCompRect.Height) : currCompRect.Top - newCompRect.Top
                            ).AlignToGrid(currentPage.GridSize, currentPage.Report.Info.AlignToGrid);
                            break;
                        }
                    case "Bottom":
                    case "ResizeHeight":
                        {
                            rect = new RectangleD(
                                0, 0, 0,
                                invertHeight ? -(currCompRect.Height + newCompRect.Height) : newCompRect.Height - currCompRect.Height
                            ).AlignToGrid(currentPage.GridSize, currentPage.Report.Info.AlignToGrid);
                            break;
                        }
                    case "LeftTop":
                        {
                            rect = new RectangleD(
                                invertWidth ? -(currCompRect.Width + newCompRect.Width) : currCompRect.Left - newCompRect.Left,
                                invertHeight ? -(currCompRect.Height + newCompRect.Height) : currCompRect.Top - newCompRect.Top,
                                invertWidth ? -(currCompRect.Width + newCompRect.Width) : currCompRect.Left - newCompRect.Left,
                                invertHeight ? -(currCompRect.Height + newCompRect.Height) : currCompRect.Top - newCompRect.Top
                            ).AlignToGrid(currentPage.GridSize, currentPage.Report.Info.AlignToGrid);
                            break;
                        }
                    case "LeftBottom":
                        {
                            rect = new RectangleD(
                                invertWidth ? -(currCompRect.Width + newCompRect.Width) : currCompRect.Left - newCompRect.Left, 0,
                                invertWidth ? -(currCompRect.Width + newCompRect.Width) : currCompRect.Left - newCompRect.Left,
                                invertHeight ? -(currCompRect.Height + newCompRect.Height) : newCompRect.Height - currCompRect.Height
                            ).AlignToGrid(currentPage.GridSize, currentPage.Report.Info.AlignToGrid);
                            break;
                        }
                    case "RightTop":
                        {
                            rect = new RectangleD(
                                0, invertHeight ? -(currCompRect.Height + newCompRect.Height) : currCompRect.Top - newCompRect.Top,
                                invertWidth ? -(currCompRect.Width + newCompRect.Width) : newCompRect.Width - currCompRect.Width,
                                invertHeight ? -(currCompRect.Height + newCompRect.Height) : currCompRect.Top - newCompRect.Top
                            ).AlignToGrid(currentPage.GridSize, currentPage.Report.Info.AlignToGrid);
                            break;
                        }
                    case "ResizeDiagonal":
                    case "RightBottom":
                        {
                            rect = new RectangleD(
                                0, 0,
                                invertWidth ? -(currCompRect.Width + newCompRect.Width) : newCompRect.Width - currCompRect.Width,
                                invertHeight ? -(currCompRect.Height + newCompRect.Height) : newCompRect.Height - currCompRect.Height
                            ).AlignToGrid(currentPage.GridSize, currentPage.Report.Info.AlignToGrid);
                            break;
                        }
                }
            }

            comp.Select();
            currentPage.ChangePosition(rect);

            if (comp is StiCrossLinePrimitive)
            {
                ChangeRectPrimitivePoints(comp, currentPage.Unit.ConvertFromHInches(comp.GetPaintRectangle()));
            }

            if (comp is StiCrossDataBand || comp is StiCrossHeaderBand || comp is StiCrossFooterBand
                || comp is StiCrossGroupHeaderBand || comp is StiCrossGroupFooterBand)
            {
                comp.GetContainer().Components.SortBandsByLeftPosition();
            }
            else if (comp is StiBand)
            {
                comp.GetContainer().Components.SortBandsByTopPosition();
            }

            comp.Page.Correct(command == "MoveComponent" && comp is StiBand);
            comp.Page.Normalize();
            comp.Invert();
        }

        public static void SetComponentRect(StiComponent component, RectangleD rect, bool alignToGrid = true, bool correctOnlySelect = false)
        {
            component.Select();

            if (alignToGrid)
            {
                rect = rect.AlignToGrid(component.Page.GridSize, component.Report.Info.AlignToGrid);
            }

            if (component is StiCrossLinePrimitive)
            {
                ChangeRectPrimitivePoints(component, rect);
            }

            component.SetPaintRectangle(rect);

            if (component is StiCrossDataBand || component is StiCrossHeaderBand || component is StiCrossFooterBand
                || component is StiCrossGroupHeaderBand || component is StiCrossGroupFooterBand)
            {
                component.GetContainer().Components.SortBandsByLeftPosition();
            }
            else if (component is StiBand)
            {
                component.GetContainer().Components.SortBandsByTopPosition();
            }

            component.Page.Correct(correctOnlySelect);
        }

        public static string GetComponentRect(StiComponent component)
        {
            return RectToStr(component.GetPaintRectangle(false, false));
        }

        public static string GetPageSize(StiPage page)
        {
            return string.Format("{0}!{1}",
                DoubleToStr(page is IStiDashboard ? page.Width : page.PageWidth),
                DoubleToStr(page is IStiDashboard ? page.Height : page.PageHeight));
        }

        public static string GetPageMargins(StiPage page)
        {
            return $"{DoubleToStr(page.Margins.Left)}!{DoubleToStr(page.Margins.Top)}!{DoubleToStr(page.Margins.Right)}!{DoubleToStr(page.Margins.Bottom)}";
        }

        public static void GetAllComponentsPositions(StiReport report, Hashtable callbackResult)
        {
            ArrayList compPositions = new ArrayList();
            ArrayList pagePositions = new ArrayList();

            foreach (StiPage page in report.Pages)
            {
                Hashtable pageObject = new Hashtable();
                pagePositions.Add(pageObject);
                pageObject["name"] = page.Name;
                pageObject["size"] = GetPageSize(page);
                pageObject["margins"] = GetPageMargins(page);
                pageObject["columnWidth"] = DoubleToStr(page.ColumnWidth);
                pageObject["columnGaps"] = DoubleToStr(page.ColumnGaps);

                StiComponentsCollection components = page.GetComponents();
                foreach (StiComponent component in components)
                {
                    Hashtable compObject = new Hashtable();
                    compPositions.Add(compObject);
                    compObject["pageName"] = page.Name;
                    compObject["name"] = component.Name;
                    compObject["componentRect"] = GetComponentRect(component);
                    compObject["compPos"] = DoubleToStr(component.Left) + "!" + DoubleToStr(component.Top);
                    if (component is StiPanel || component is StiDataBand || component is StiHierarchicalBand) {
                        double columnWidth = 0;
                        double columnGaps = 0;
                        PropertyInfo columnWidthProp = component.GetType().GetProperty("ColumnWidth");
                        if (columnWidthProp != null) columnWidth = (double)columnWidthProp.GetValue(component, null);
                        PropertyInfo columnGapsProp = component.GetType().GetProperty("ColumnGaps");
                        if (columnGapsProp != null) columnGaps = (double)columnGapsProp.GetValue(component, null);
                        compObject["columnWidth"] = DoubleToStr(columnWidth);
                        compObject["columnGaps"] = DoubleToStr(columnGaps);
                    }
                    if (component is StiCrossTab)
                    {
                        compObject["crossTabFields"] = GetCrossTabFieldsProperties(component as StiCrossTab);
                    }
                }
            }

            callbackResult["pagePositions"] = pagePositions;
            callbackResult["compPositions"] = compPositions;
        }

        private static Hashtable GetIconSetItemObject(StiIconSetItem iconSetItem)
        {
            if (iconSetItem == null) return null;

            Hashtable iconSetItemObject = new Hashtable();
            iconSetItemObject["Icon"] = iconSetItem.Icon;
            iconSetItemObject["Operation"] = iconSetItem.Operation;
            iconSetItemObject["ValueType"] = iconSetItem.ValueType;
            iconSetItemObject["Value"] = iconSetItem.Value;

            return iconSetItemObject;
        }

        private static StiIconSetItem GetIconSetItemFromObject(object iconSetItemObject)
        {
            if (iconSetItemObject == null) return null;
            Hashtable iconSetItem = iconSetItemObject as Hashtable;

            return new StiIconSetItem(
                iconSetItem["Icon"] != null ? (StiIcon)Enum.Parse(typeof(StiIcon), (string)iconSetItem["Icon"]) : StiIcon.None,
                iconSetItem["Operation"] != null ? (StiIconSetOperation)Enum.Parse(typeof(StiIconSetOperation), (string)iconSetItem["Operation"]) : StiIconSetOperation.MoreThanOrEqual,
                iconSetItem["ValueType"] != null ? (StiIconSetValueType)Enum.Parse(typeof(StiIconSetValueType), (string)iconSetItem["ValueType"]) : StiIconSetValueType.Percent,
                iconSetItem["Value"] != null ? (float)StrToDouble((string)iconSetItem["Value"]) : 0f
            );
        }

        public static Color StrToColor(string colorStr)
        {
            Color newColor = Color.Transparent;

            if (!string.IsNullOrEmpty(colorStr) && colorStr != "transparent")
            {
                string[] colors = colorStr.Split(',');

                if (colors.Length == 3)
                {
                    int r1 = StrToInt(colors[0]);
                    int g1 = StrToInt(colors[1]);
                    int b1 = StrToInt(colors[2]);
                    newColor = Color.FromArgb(255, ((r1 > 255) ? 255 : r1), ((g1 > 255) ? 255 : g1), ((b1 > 255) ? 255 : b1));

                }
                else
                {
                    int a = StrToInt(colors[0]);
                    int r2 = StrToInt(colors[1]);
                    int g2 = StrToInt(colors[2]);
                    int b2 = StrToInt(colors[3]);

                    if (a == 0 && r2 == 255 && g2 == 255 && b2 == 255)
                        newColor = Color.Transparent;
                    else 
                        newColor = Color.FromArgb(((a > 255) ? 255 : a), ((r2 > 255) ? 255 : r2), ((g2 > 255) ? 255 : g2), ((b2 > 255) ? 255 : b2));
                }
            }

            return newColor;
        }

        public static double StrToDouble(string value)
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            string numSep = currentCulture.NumberFormat.NumberDecimalSeparator;
            value = value.Replace(",", ".").Replace(".", numSep);

            double result = 0;
            double.TryParse(value, out result);

            return result;
        }

        public static string DoubleToStr(object value)
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            string numSep = currentCulture.NumberFormat.NumberDecimalSeparator;
            string str = value.ToString();
            str = str.Replace(numSep, ".").Replace(",", ".");

            return str;
        }

        public static int StrToInt(string value)
        {
            int result = 0;
            int.TryParse(value, out result);

            return result;
        }

        public static StiBrush StrToBrush(string value)
        {
            string[] brushArray = value.Split('!');

            switch (brushArray[0])
            {
                case "isDefaultBrush":
                    {
                        return new StiDefaultBrush();
                    }
                case "isStyleBrush":
                    {
                        return new StiStyleBrush();
                    }
                case "0":
                    {
                        return new StiEmptyBrush();
                    }
                case "1":
                    {
                        return new StiSolidBrush(StrToColor(brushArray[1]));
                    }
                case "2":
                    {
                        return new StiHatchBrush((HatchStyle)StrToInt(brushArray[3]), StrToColor(brushArray[1]), StrToColor(brushArray[2]));
                    }
                case "3":
                    {
                        return new StiGradientBrush(StrToColor(brushArray[1]), StrToColor(brushArray[2]), StrToDouble(brushArray[3]));
                    }
                case "4":
                    {
                        return new StiGlareBrush(StrToColor(brushArray[1]), StrToColor(brushArray[2]), StrToDouble(brushArray[3]), (float)StrToDouble(brushArray[4]), (float)StrToDouble(brushArray[5]));
                    }
                case "5":
                    {
                        return new StiGlassBrush(StrToColor(brushArray[1]), brushArray[3] == "1", (float)StrToDouble(brushArray[2])); ;
                    }
            }

            return new StiEmptyBrush();
        }

        public static StiBorder StrToBorder(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            StiBorder border = null;
            var borderArray = value.Split('!');
            var borderSides = borderArray[0].Split(',');

            StiBorderSides borderLeft = (borderSides[0] == "1") ? StiBorderSides.Left : StiBorderSides.None;
            StiBorderSides borderTop = (borderSides[1] == "1") ? StiBorderSides.Top : StiBorderSides.None;
            StiBorderSides borderRight = (borderSides[2] == "1") ? StiBorderSides.Right : StiBorderSides.None;
            StiBorderSides borderBottom = (borderSides[3] == "1") ? StiBorderSides.Bottom : StiBorderSides.None;

            if (borderArray.Length > 8)
            {
                //Advanced border
                var advBorders = borderArray[8].Split(';');
                var leftSide = new StiBorderSide(StrToColor(advBorders[1]), StrToDouble(advBorders[0]), (StiPenStyle)StrToInt(advBorders[2]));
                var topSide = new StiBorderSide(StrToColor(advBorders[4]), StrToDouble(advBorders[3]), (StiPenStyle)StrToInt(advBorders[5]));
                var rightSide = new StiBorderSide(StrToColor(advBorders[7]), StrToDouble(advBorders[6]), (StiPenStyle)StrToInt(advBorders[8]));
                var bottomSide = new StiBorderSide(StrToColor(advBorders[10]), StrToDouble(advBorders[9]), (StiPenStyle)StrToInt(advBorders[11]));
                border = new StiAdvancedBorder(topSide, bottomSide, leftSide, rightSide, borderArray[4] == "1", StrToDouble(borderArray[5]), StrToBrush(StiEncodingHelper.DecodeString(borderArray[6])), borderArray[7] == "1");
            }
            else
            {
                //Basic border
                border = new StiBorder(borderLeft | borderTop | borderRight | borderBottom, StrToColor(borderArray[2]), StrToDouble(borderArray[1]), (StiPenStyle)StrToInt(borderArray[3]));

                if (borderArray.Length > 4)
                {
                    border.DropShadow = borderArray[4] == "1";
                    border.ShadowSize = StrToDouble(borderArray[5]);
                    border.ShadowBrush = StrToBrush(StiEncodingHelper.DecodeString(borderArray[6]));
                    border.Topmost = borderArray[7] == "1";
                }
            }

            return border;
        }

        public static StiSimpleBorder StrToSimpleBorder(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var borderArray = value.Split('!');
            var borderSides = borderArray[0].Split(',');

            StiBorderSides borderLeft = (borderSides[0] == "1") ? StiBorderSides.Left : StiBorderSides.None;
            StiBorderSides borderTop = (borderSides[1] == "1") ? StiBorderSides.Top : StiBorderSides.None;
            StiBorderSides borderRight = (borderSides[2] == "1") ? StiBorderSides.Right : StiBorderSides.None;
            StiBorderSides borderBottom = (borderSides[3] == "1") ? StiBorderSides.Bottom : StiBorderSides.None;

            return new StiSimpleBorder(borderLeft | borderTop | borderRight | borderBottom, StrToColor(borderArray[2]), StrToDouble(borderArray[1]), (StiPenStyle)StrToInt(borderArray[3]));
        }

        public static RectangleD StrToRect(string value)
        {
            var compRectArray = value.Split('!');
            return new RectangleD(new PointD(StrToDouble(compRectArray[0]), StrToDouble(compRectArray[1])), new SizeD(StrToDouble(compRectArray[2]), StrToDouble(compRectArray[3])));
        }

        public static StiMargins StrToMargins(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var marginsArray = str.Split(';');
                return new StiMargins(StrToDouble(marginsArray[0]), StrToDouble(marginsArray[2]), StrToDouble(marginsArray[1]), StrToDouble(marginsArray[3]));
            }

            return new StiMargins();
        }

        public static StiMargin StrToMargin(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var marginArray = str.Split(';');
                return new StiMargin(StrToDouble(marginArray[0]), StrToDouble(marginArray[1]), StrToDouble(marginArray[2]), StrToDouble(marginArray[3]));
            }

            return new StiMargin();
        }

        public static StiPadding StrToPadding(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var paddingArray = str.Split(';');
                return new StiPadding(StrToDouble(paddingArray[0]), StrToDouble(paddingArray[1]), StrToDouble(paddingArray[2]), StrToDouble(paddingArray[3]));
            }

            return new StiPadding();
        }

        public static StiCornerRadius StrToCornerRadius(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var cornRadiusArray = str.Split(';');
                return new StiCornerRadius((float)StrToDouble(cornRadiusArray[0]), (float)StrToDouble(cornRadiusArray[1]), (float)StrToDouble(cornRadiusArray[2]), (float)StrToDouble(cornRadiusArray[3]));
            }

            return new StiCornerRadius();
        }

        public static Font StrToFont(string value)
        {
            if (string.IsNullOrEmpty(value)) 
                return null;
            
            string[] fontArray = value.Split('!');
            var fontName = fontArray[0];

            var fontStyle = (FontStyle)0;

            if (fontArray[2] == "1")
                fontStyle |= FontStyle.Bold;

            if (fontArray[3] == "1")
                fontStyle |= FontStyle.Italic;

            if (fontArray[4] == "1")
                fontStyle |= FontStyle.Underline;

            if (fontArray[5] == "1")
                fontStyle |= FontStyle.Strikeout;


            FontFamily fontFamily = StiFontCollection.IsCustomFont(fontName) ? StiFontCollection.GetFontFamily(fontName) : new FontFamily(fontName);
            try
            {
                return StiFontCollection.CreateFont(fontName, (float)StrToDouble(fontArray[1]), StiFontUtils.CorrectStyle(fontFamily.Name, fontStyle));
            }
            finally
            {
                if (!StiFontCollection.IsCustomFont(fontName)) fontFamily.Dispose();
            }
        }

        public static StiConditionBorderSides StrBordersToConditionBorderSidesObject(string borders)
        {
            int borderSides = 0;
            if (borders.IndexOf("All") >= 0 || borders.IndexOf("Top") >= 0) borderSides += 1;
            if (borders.IndexOf("All") >= 0 || borders.IndexOf("Left") >= 0) borderSides += 2;
            if (borders.IndexOf("All") >= 0 || borders.IndexOf("Right") >= 0) borderSides += 4;
            if (borders.IndexOf("All") >= 0 || borders.IndexOf("Bottom") >= 0) borderSides += 8;

            return (StiConditionBorderSides)borderSides;
        }

        public static StiConditionPermissions StrPermissionsToConditionPermissionsObject(string strPermissions)
        {
            int permissions = 0;
            if (strPermissions.IndexOf("All") >= 0 || strPermissions == ("Font") || strPermissions.IndexOf("Font,") == 0 ||
                strPermissions.IndexOf(", Font,") >= 0 || strPermissions.EndsWith("Font")) permissions += 1;
            if (strPermissions.IndexOf("All") >= 0 || strPermissions.IndexOf("FontSize") >= 0) permissions += 2;
            if (strPermissions.IndexOf("All") >= 0 || strPermissions.IndexOf("FontStyleBold") >= 0) permissions += 4;
            if (strPermissions.IndexOf("All") >= 0 || strPermissions.IndexOf("FontStyleItalic") >= 0) permissions += 8;
            if (strPermissions.IndexOf("All") >= 0 || strPermissions.IndexOf("FontStyleUnderline") >= 0) permissions += 16;
            if (strPermissions.IndexOf("All") >= 0 || strPermissions.IndexOf("FontStyleStrikeout") >= 0) permissions += 32;
            if (strPermissions.IndexOf("All") >= 0 || strPermissions.IndexOf("TextColor") >= 0) permissions += 64;
            if (strPermissions.IndexOf("All") >= 0 || strPermissions.IndexOf("BackColor") >= 0) permissions += 128;
            if (strPermissions.IndexOf("All") >= 0 || strPermissions.IndexOf("Borders") >= 0) permissions += 256;

            return (StiConditionPermissions)permissions;
        }

        public static string GetReportFileName(StiReport report)
        {
            string fileName = String.Empty;
            if (report.ReportFile != null && report.ReportFile.Length > 0)
            {
                string[] path = report.ReportFile.Replace("\\\\", "/").Replace("\\", "/").Split('/');
                fileName = path[path.Length - 1];
            }
            
            fileName = fileName.Replace("<", " ").Replace(">", " ").Replace("\\", "-").Replace("/", "-").Replace(":", "-");
            fileName = fileName.Replace("?", ".").Replace("\"", "'").Replace("|", " ").Replace("*", " ");

            return fileName;
        }

        public static StiComponent CreateInfographicComponent(string compAttributesString, StiReport report, Hashtable param)
        {
            string[] compAttributes = compAttributesString.Split(';');
            string componentType = compAttributes[1];

            switch (componentType)
            {
                case "StiChart":
                    {
                        StiChart chart = new StiChart();
                        string seriesType = compAttributes[2];

                        Assembly assembly = typeof(StiReport).Assembly;
                        var series = assembly.CreateInstance("Stimulsoft.Report.Chart." + seriesType) as StiSeries;
                        chart.Series.Add(series);
                        chart.EditorType = param.Contains("chartEditorType") ? (StiChartEditorType)Enum.Parse(typeof (StiChartEditorType), param["chartEditorType"] as string) : StiChartEditorType.Simple;

                        return chart;
                    }
                case "StiGauge":
                    {
                        StiGauge gauge = new StiGauge();
                        gauge.Type = (StiGaugeType)Enum.Parse(typeof(StiGaugeType), compAttributes[2] as string);
                        Gauge.Helpers.StiGaugeV2InitHelper.Init(gauge, gauge.Type, true, false);

                        return gauge;
                    }
                case "StiMap":
                    {
                        StiMap map = new StiMap();
                        map.MapIdent = compAttributes[2];

                        return map;
                    }
                case "StiRegionMapElement":
                    {
                        var mapElement = StiDashboardHelper.CreateDashboardElement(report, "StiRegionMapElement");
                        ((IStiRegionMapElement)mapElement).MapIdent = compAttributes[2];

                        return mapElement;
                    }
            }

            return null;
        }
        
        private static StiComponent CreateShapeComponent(string componentTypeArray)
        {
            StiShape comp = new StiShape();
            string[] compAttributes = componentTypeArray.Split(';');
            string shapeType = compAttributes[1];
            SetShapeTypeProperty(comp, shapeType);

            return comp;
        }

        private static StiComponent CreateBarCodeComponent(string componentTypeArray)
        {
            StiBarCode comp = new StiBarCode();
            comp.BackColor = Color.Transparent;
            string[] compAttributes = componentTypeArray.Split(';');
            SetBarCodeTypeProperty(comp, compAttributes[1]);
            comp.Code.Value = comp.BarCodeType.DefaultCodeValue;

            return comp;
        }

        private static void ApplyStyleCollection(StiComponent comp, StiStylesCollection stylesCollection)
        {
            if (comp is StiPage) return;
            foreach (StiBaseStyle style in stylesCollection)
            {
                if (!StiStyleConditionHelper.IsAllowStyle(comp, style)) continue;

                if (comp is StiDataBand)
                {
                    bool isOddStyleDataBand = false;
                    bool isEvenStyleDataBand = false;
                    foreach (StiStyleCondition condition in style.Conditions)
                    {
                        if ((condition.Placement & StiStyleComponentPlacement.DataOddStyle) > 0)
                        {
                            isOddStyleDataBand = true;
                            break;
                        }

                        if ((condition.Placement & StiStyleComponentPlacement.DataEvenStyle) > 0)
                        {
                            isEvenStyleDataBand = true;
                            break;
                        }
                    }
                    if (isOddStyleDataBand) ((StiDataBand)comp).OddStyle = style.Name;
                    else if (isEvenStyleDataBand) ((StiDataBand)comp).EvenStyle = style.Name;
                    else comp.ComponentStyle = style.Name;
                }
                else comp.ComponentStyle = style.Name;
            }

            ApplyStyles(comp, stylesCollection);
        }

        public static void ApplyStyles(StiComponent comp, StiStylesCollection stylesCollection)
        {
            if (!string.IsNullOrEmpty(comp.ComponentStyle))
            {
                #region Apply styles to component
                StiBaseStyle style = stylesCollection[comp.ComponentStyle];
                if (style != null)
                {
                    style.SetStyleToComponent(comp);
                }
                #endregion
            }
        }

        public static Hashtable GetComponentMainProperties(StiComponent component, double zoom)
        {
            Hashtable mainProps = new Hashtable();
            mainProps["name"] = component.Name;
            mainProps["typeComponent"] = component.GetType().Name;
            mainProps["componentRect"] = GetComponentRect(component);
            mainProps["parentName"] = GetParentName(component);
            mainProps["parentIndex"] = GetParentIndex(component).ToString();
            mainProps["componentIndex"] = GetComponentIndex(component).ToString();
            mainProps["childs"] = GetAllChildComponents(component);
            mainProps["svgContent"] = GetSvgContent(component, zoom);
            mainProps["pageName"] = component.Page.Name;
            mainProps["properties"] = GetAllProperties(component);

            return mainProps;
        }

        public static ArrayList GetTableCells(StiTable table, double zoom)
        {
            ArrayList components = new ArrayList();
            foreach (StiComponent component in table.Components)
            {
                components.Add(GetComponentMainProperties(component, zoom));
            }

            return components;
        }

        public static StiDataColumn GetColumnFromColumnPath(string columnPath, StiReport report)
        {
            string[] pathArray = columnPath.Split('.');
            if (pathArray.Length > 1)
            {
                var dataSource = report.Dictionary.DataSources[pathArray[pathArray.Length - 2]];
                if (dataSource == null && report.Dictionary.Relations[pathArray[pathArray.Length - 2]] != null)
                    dataSource = report.Dictionary.Relations[pathArray[pathArray.Length - 2]].ParentSource;

                if (dataSource != null)
                {
                    var column = dataSource.Columns[pathArray[pathArray.Length - 1]];
                    if (column != null && (column.Type == typeof(Image) || column.Type == typeof(byte[]) || column.Type == typeof(string)))
                    {
                        return column;
                    }
                }
            }

            return null;
        }

        public static string GetBase64PngFromMetaFileBytes(byte[] imageBytes, int width, int height)
        {
            if (imageBytes == null) return null;

            try
            {
                return ImageToBase64(StiMetafileConverter.MetafileToPngBytes(imageBytes, width, height));
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return null;
            }
        }

        public static string GetBase64PngFromMetaFileBytes(byte[] imageBytes)
        {
            return GetBase64PngFromMetaFileBytes(imageBytes, 500, 500);
        }

        public static string GetImageContentForPaint(StiImage imageComp, Hashtable requestParams = null)
        {
            if (imageComp.TakeImage() == null)
            {
                try
                {
                    if (requestParams != null && requestParams["command"] as string == "SendProperties")
                    {
                        //Keep previous content if not changes src properties 
                        var needToRepaintImage = false;
                        var components = requestParams["components"] as ArrayList;
                        if (components != null)
                        {
                            foreach (Hashtable component in components)
                            {
                                var properties = component["properties"] as ArrayList;
                                if (properties != null)
                                {
                                    foreach (Hashtable property in properties)
                                    {
                                        var propertyName = property["name"] as string;
                                        if (propertyName == "imageDataColumn" ||
                                            propertyName == "imageSrc" ||
                                            propertyName == "imageUrl" ||
                                            propertyName == "imageFile" ||
                                            propertyName == "imageData")
                                        {
                                            needToRepaintImage = true;
                                        }
                                    }
                                }
                            }
                        }
                        if (!needToRepaintImage) return "keepPrevContent";
                    }

                    if (imageComp.ImageURL != null && !String.IsNullOrEmpty(imageComp.ImageURL.Value))
                    {
                        if (StiHyperlinkProcessor.IsResourceHyperlink(imageComp.ImageURL.Value))
                        {
                            var resource = imageComp.Report.Dictionary.Resources[StiHyperlinkProcessor.GetResourceNameFromHyperlink(imageComp.ImageURL.Value)];
                            if (resource != null)
                            {
                                if (Stimulsoft.Report.Helpers.StiImageHelper.IsMetafile(resource.Content))
                                    return GetBase64PngFromMetaFileBytes(resource.Content);
                                else
                                    return ImageToBase64(resource.Content);
                            }
                        }
                        if (StiHyperlinkProcessor.IsVariableHyperlink(imageComp.ImageURL.Value))
                        {
                            var variable = imageComp.Report.Dictionary.Variables[StiHyperlinkProcessor.GetVariableNameFromHyperlink(imageComp.ImageURL.Value)];
                            if (variable != null)
                            {
                                return ImageToBase64(variable.ValueObject as Image);
                            }
                        }
                        else
                        {
                            if (imageComp.ImageURL.Value.StartsWith("{")) return null;
                            return imageComp.ImageURL.Value;
                        }
                    }
                    else if (!String.IsNullOrEmpty(imageComp.DataColumn) && StiOptions.Designer.PreloadImageFromDataColumn)
                    {
                        var column = GetColumnFromColumnPath(imageComp.DataColumn, imageComp.Report);
                        if (column != null)
                        {
                            imageComp.Report.Dictionary.Connect(true, new List<StiDataSource> { column.DataSource });
                            var image = StiGalleriesHelper.GetImageFromColumn(column, imageComp.Report);
                            if (image != null)
                            {
                                return ImageToBase64(image);
                            }
                        }
                    }
                    else if (imageComp.ImageData != null && !String.IsNullOrEmpty(imageComp.ImageData.Value))
                    {
                        string variableName = imageComp.ImageData.Value.Substring(1, imageComp.ImageData.Value.Length - 2);

                        var variable = imageComp.Report.Dictionary.Variables[variableName];
                        if (variable != null)
                        {
                            try
                            {
                                var image = variable.ValueObject as Image;
                                return image != null ? ImageToBase64(image) : null;
                            }
                            catch
                            {
                                return null;
                            }
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            else if (Stimulsoft.Report.Helpers.StiImageHelper.IsMetafile(imageComp.TakeImage()))
            {
                var imageRect = imageComp.GetPaintRectangle(true, false);
                return GetBase64PngFromMetaFileBytes(imageComp.TakeImage(), (int)imageRect.Width * 2, (int)imageRect.Height * 2);
            }

            return null;
        }

        public static string GetWatermarkImageContentForPaint(StiPage page, Hashtable pageProps)
        {
            if (page.Watermark != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(page.Watermark.ImageHyperlink))
                    {
                        if (StiHyperlinkProcessor.IsResourceHyperlink(page.Watermark.ImageHyperlink))
                        {
                            var resource = page.Report.Dictionary.Resources[StiHyperlinkProcessor.GetResourceNameFromHyperlink(page.Watermark.ImageHyperlink)];
                            if (resource != null)
                            {
                                using (var gdiImage = StiImageConverter.BytesToImage(resource.Content))
                                {
                                    pageProps["watermarkImageSize"] = String.Format("{0};{1}", gdiImage.Width, gdiImage.Height);

                                    if (Report.Helpers.StiImageHelper.IsMetafile(resource.Content))
                                        return GetBase64PngFromMetaFileBytes(resource.Content);
                                    else
                                        return ImageToBase64(resource.Content);
                                }
                            }
                        }
                        if (StiHyperlinkProcessor.IsVariableHyperlink(page.Watermark.ImageHyperlink))
                        {
                            var variable = page.Report.Dictionary.Variables[StiHyperlinkProcessor.GetVariableNameFromHyperlink(page.Watermark.ImageHyperlink)];
                            if (variable != null)
                            {
                                var image = variable.ValueObject as Image;
                                pageProps["watermarkImageSize"] = String.Format("{0};{1}", image.Width, image.Height);
                                return ImageToBase64(image);
                            }
                        }
                    }
                    else if (Stimulsoft.Report.Helpers.StiImageHelper.IsMetafile(page.Watermark.TakeImage()))
                    {
                        return GetBase64PngFromMetaFileBytes(page.Watermark.TakeImage());
                    }
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
                
        public static void ClearUndoArray(StiRequestParams requestParams)
        {
            requestParams.Cache.Helper.SaveObjectInternal(new ArrayList(), requestParams, StiCacheHelper.GUID_UndoArray);
        }

        public static void AddReportToUndoArray(StiRequestParams requestParams, StiReport report)
        {
            AddReportToUndoArray(requestParams, report, false);
        }

        public static void AddReportToUndoArray(StiRequestParams requestParams, StiReport report, bool withResources)
        {
            if (report == null) return;
            
            ArrayList undoArray = requestParams.Cache.Helper.GetObjectInternal(requestParams, StiCacheHelper.GUID_UndoArray) as ArrayList;
            if (undoArray == null || undoArray.Count == 0)
            {
                undoArray = new ArrayList();
                undoArray.Add(1);
                undoArray.Add(null);
            }

            StiReport cloneReport = StiReportCopier.CloneReport(report, withResources);
            int currentPos = Convert.ToInt32(undoArray[0]);
            undoArray.Insert(currentPos, new StiReportContainer(cloneReport, withResources, requestParams.Designer.Command));
            currentPos++;
            undoArray[0] = currentPos;

            if (undoArray.Count > currentPos + 1)
            {
                undoArray.RemoveRange(currentPos, undoArray.Count - currentPos);
                undoArray.Add(null);
            }

            if (undoArray.Count > requestParams.Designer.UndoMaxLevel + 2)
            {
                undoArray.RemoveAt(1);
                if (currentPos != 1) undoArray[0] = currentPos - 1;
            }

            requestParams.Cache.Helper.SaveObjectInternal(undoArray, requestParams, StiCacheHelper.GUID_UndoArray);
        }

        private static void AddPrimitivePoints(StiComponent addedComp, StiPage currentPage)
        {
            var primitive = addedComp as StiCrossLinePrimitive;
            if (primitive != null)
            {
                var rect = primitive.ClientRectangle;

                var startPoint = new StiStartPointPrimitive
                {
                    Left = rect.Left,
                    Top = rect.Top,
                    Width = 0,
                    Height = 0
                };
                currentPage.Components.Add(startPoint);

                var endPoint = new StiEndPointPrimitive();
                if (primitive is StiRectanglePrimitive)
                    endPoint.Left = rect.Right;
                else
                    endPoint.Left = rect.Left;

                endPoint.Top = rect.Bottom;
                endPoint.Width = 0;
                endPoint.Height = 0;

                currentPage.Components.Add(endPoint);

                startPoint.ReferenceToGuid = primitive.Guid;
                endPoint.ReferenceToGuid = primitive.Guid;

                currentPage.Correct();
            }
        }

        private static void RemovePrimitivePoints(StiComponent removiedComp)
        {
            StiCrossLinePrimitive primitive = removiedComp as StiCrossLinePrimitive;
            if (primitive != null)
            {
                StiStartPointPrimitive startPoint = primitive.GetStartPoint();
                if (startPoint != null && startPoint.Parent != null && startPoint.Parent.Components.Contains(startPoint))
                {
                    startPoint.ReferenceToGuid = null;
                    startPoint.Parent.Components.Remove(startPoint);
                }
                StiEndPointPrimitive endPoint = primitive.GetEndPoint();
                if (endPoint != null && endPoint.Parent != null && endPoint.Parent.Components.Contains(endPoint))
                {
                    endPoint.ReferenceToGuid = null;
                    endPoint.Parent.Components.Remove(endPoint);
                }
            }
        }

        private static void ChangeRectPrimitivePoints(StiComponent changedComp, RectangleD rect)
        {
            StiCrossLinePrimitive primitive = changedComp as StiCrossLinePrimitive;
            if (primitive != null)
            {
                StiStartPointPrimitive startPoint = primitive.GetStartPoint();
                if (startPoint != null)
                {
                    startPoint.SetPaintRectangle(new RectangleD(new PointD(rect.Left, rect.Top), new SizeD(0, 0)));
                }
                StiEndPointPrimitive endPoint = primitive.GetEndPoint();
                if (endPoint != null)
                {
                    endPoint.SetPaintRectangle(new RectangleD(new PointD(primitive is StiRectanglePrimitive ? rect.Right : rect.Left, rect.Bottom), new SizeD(0, 0)));
                }
            }
        }

        private static void CheckAllPrimitivePoints(StiPage page)
        {
            Hashtable primitiveElements = new Hashtable();

            //Find primitive componets
            foreach (StiComponent comp in page.GetComponents())
            {
                if (comp is StiPointPrimitive || comp is StiCrossLinePrimitive)
                {
                    string guid = comp is StiPointPrimitive ? ((StiPointPrimitive)comp).ReferenceToGuid : ((StiCrossLinePrimitive)comp).Guid;
                    if (!String.IsNullOrEmpty(guid))
                    {
                        if (primitiveElements[guid] == null) primitiveElements[guid] = new ArrayList();
                        ((ArrayList)primitiveElements[guid]).Add(comp);
                    }
                }
            }

            //Remove primitive components, which lost any elements
            foreach (string key in primitiveElements.Keys)
            {
                ArrayList primitiveComponents = primitiveElements[key] as ArrayList;
                if (primitiveComponents.Count < 3) {
                    foreach (StiComponent comp in primitiveComponents) {
                        if (comp != null && comp.Parent != null && comp.Parent.Components.Contains(comp)) {
                            comp.Parent.Components.Remove(comp);
                        }
                    }
                }
            }
        }

        private static bool IsAlignedByGrid(StiComponent component)
        {
            decimal gridSize = (decimal)component.Report.Info.GridSize;

            return (
                (decimal)component.Left % gridSize == 0 &&
                (decimal)component.Top % gridSize == 0 &&
                (decimal)component.Width % gridSize == 0 &&
                (decimal)component.Height % gridSize == 0
            );
        }

        private static void AddSubReportPage(StiSubReport subReport, Hashtable callbackResult, Hashtable param)
        {
            if (subReport != null)
            {   
                StiPage page = new StiPage(subReport.Report);
                page.Skip = true;
                subReport.Report.Pages.Add(page);
                subReport.SubReportPage = page;
                page.Name = StiNameCreation.CreateName(subReport.Report, StiNameCreation.GenerateName(subReport.Report, subReport.LocalizedName + '_', "subReport_"));

                #region LicenseKey
#if CLOUD
            var isValid = StiCloudPlan.IsReportsAvailable(subReport.Report.ReportGuid);
#elif SERVER
            var isValid = !StiVersionX.IsSvr;
#else
                var isValidKey = false;
                var licenseKey = StiLicenseKeyValidator.GetLicenseKey();
                if (licenseKey != null)
                {
                    try
                    {
                        using (var rsa = new RSACryptoServiceProvider(512))
                        using (var sha = new SHA1CryptoServiceProvider())
                        {
                            rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                            isValidKey = rsa.VerifyData(licenseKey.GetCheckBytes(), sha, licenseKey.GetSignatureBytes());
                        }
                    }
                    catch (Exception)
                    {
                        isValidKey = false;
                    }
                }

                var isValid = isValidKey & StiLicenseKeyValidator.IsValid(StiProductIdent.Web, licenseKey) && typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key);
#endif
                #endregion

                Hashtable subReportPage = new Hashtable();
                subReportPage["name"] = page.Name;
                subReportPage["pageIndex"] = subReport.Report.Pages.IndexOf(page).ToString();
                subReportPage["properties"] = GetAllProperties(page);
                subReportPage["pageIndexes"] = GetPageIndexes(subReport.Report);
                if (isValid) subReportPage["valid"] = true;

                callbackResult["newSubReportPage"] = subReportPage;
            }
        }

        public static ArrayList GetColorsCollectionProperty(Color[] colors)
        {            
            if (colors != null)
            {
                ArrayList result = new ArrayList();
                foreach (Color color in colors)
                    result.Add(GetStringFromColor(color));

                return result;
            }

            return null;
        }
        #endregion

        #region Get All Properties
        public static Hashtable GetAllProperties(StiComponent component, Hashtable requestParams = null)
        {
            Hashtable allProps = new Hashtable();

            #region Common properties for report components & DBS elements
            allProps["aliasName"] = StiEncodingHelper.Encode(component.Alias);
            allProps["pointerValue"] = !string.IsNullOrEmpty(component.Pointer?.Value) ? StiEncodingHelper.Encode(component.Pointer.Value) : string.Empty;
            allProps["events"] = GetEventsProperty(component);

            if (!(component is StiPage))
            {
                allProps["locked"] = component.Locked;
                allProps["linked"] = component.Linked;
            }
            #endregion

            if (component is IStiElement)
            {
                #region Common properties for all DBS elements
                allProps["enabled"] = (component as IStiElement).IsEnabled;
                allProps["name"] = (component as IStiElement).Name;
                allProps["expressions"] = GetExpressionsProperty(component);

                if (component is IStiDashboardElementStyle && !(component is IStiOnlineMapElement))
                {
                    allProps["elementStyle"] = (component as IStiDashboardElementStyle).Style;
                    allProps["customStyleName"] = (component as IStiDashboardElementStyle).CustomStyleName;
                    allProps["isDarkStyle"] = StiDashboardStyleHelper.IsDarkStyle(component as IStiElement);
                }

                if (component is IStiBackColor)
                    allProps["backColor"] = GetStringFromColor((component as IStiBackColor).BackColor);
                                
                if (!(component is IStiDashboard))
                {
                    allProps["isDashboardElement"] = true;
                    allProps["realBackColor"] = GetStringFromColor(StiDashboardStyleHelper.GetBackColor(component as IStiElement, null, true));
                    allProps["elementKey"] = (component as IStiElement)?.GetKey();
                    allProps["margin"] = MarginToStr((component as IStiMargin).Margin);
                    allProps["padding"] = PaddingToStr((component as IStiPadding).Padding);

                    if (component is IStiSimpleBorder)
                        allProps["border"] = SimpleBorderToStr((component as IStiSimpleBorder).Border);

                    if (component is IStiForeColor)
                        allProps["foreColor"] = GetStringFromColor((component as IStiForeColor).ForeColor);

                    if (component is IStiGroupElement)
                        allProps["group"] = (component as IStiGroupElement).Group;

                    if (component is IStiTextFormat && ((IStiTextFormat)component).TextFormat != null)
                        allProps["textFormat"] = StiTextFormatHelper.GetTextFormatItem(((IStiTextFormat)component).TextFormat);

                    if (component is IStiTransformFilters || component is IStiTransformSorts || component is IStiTransformActions)
                        allProps["dataTransformation"] = true;

                    if (component is IStiTitleElement)
                    {
                        allProps["titleText"] = StiEncodingHelper.Encode((component as IStiTitleElement).Title.Text);
                        allProps["titleBackColor"] = GetStringFromColor((component as IStiTitleElement).Title.BackColor);
                        allProps["titleForeColor"] = GetStringFromColor((component as IStiTitleElement).Title.ForeColor);
                        allProps["titleFont"] = FontToStr((component as IStiTitleElement).Title.Font);
                        allProps["titleHorAlignment"] = (component as IStiTitleElement).Title.HorAlignment;
                        allProps["titleVisible"] = (component as IStiTitleElement).Title.Visible;
                        allProps["titleSizeMode"] = (component as IStiTitleElement).Title.SizeMode;
                    }

                    var shadow = (component as IStiSimpleShadow)?.Shadow;
                    if (shadow != null)
                    {
                        allProps["shadowVisible"] = shadow.Visible;
                        allProps["shadowColor"] = GetStringFromColor(shadow.Color);
                        allProps["shadowLocation"] = $"{shadow.Location.X};{shadow.Location.Y}";
                        allProps["shadowSize"] = shadow.Size.ToString();
                    }

                    var cornerRadius = (component as IStiCornerRadius)?.CornerRadius;
                    if (cornerRadius != null)
                    {
                        allProps["cornerRadius"] = CornerRadiusToStr(cornerRadius);
                    }
                }
                #endregion
            }
            else
            {
                #region Common properties for all report components                
                allProps["componentStyle"] = !string.IsNullOrEmpty(component.ComponentStyle) ? component.ComponentStyle : "[None]";
                allProps["conditions"] = GetConditionsProperty(component);
                allProps["expressions"] = GetExpressionsProperty(component);

                if (!(component is StiPage))
                {
                    allProps["restrictions"] = component.Restrictions.ToString();

                    if (!(component is StiMap) && !(component is StiGauge))
                        allProps["useParentStyles"] = component.UseParentStyles;
                }

                if (component is StiBand)
                {
                    allProps["headerSize"] = GetComponentHeaderSize(component);
                }

                if (!(component is StiTableOfContents))
                {
                    allProps["interaction"] = GetInteractionProperty(component.Interaction);
                }
                #endregion
            }

            switch (component.GetType().Name)
            {
                #region StiPage
                case "StiPage":
                {
                        var page = component as StiPage;
                        allProps["pageKey"] = page.Guid;
                        allProps["border"] = BorderToStr(page.Border);
                        allProps["brush"] = BrushToStr(page.Brush);
                        allProps["orientation"] = page.Orientation.ToString();
                        allProps["unitWidth"] = DoubleToStr(page.PageWidth);
                        allProps["unitHeight"] = DoubleToStr(page.PageHeight);
                        allProps["unitMargins"] = GetPageMargins(page);
                        allProps["columns"] = page.Columns.ToString();
                        allProps["columnWidth"] = DoubleToStr(page.ColumnWidth);
                        allProps["columnGaps"] = DoubleToStr(page.ColumnGaps);
                        allProps["rightToLeft"] = page.RightToLeft;
                        allProps["paperSize"] = ((int)page.PaperSize).ToString();
                        allProps["waterMarkRatio"] = page.Watermark.AspectRatio;
                        allProps["waterMarkRightToLeft"] = page.Watermark.RightToLeft;
                        allProps["waterMarkEnabled"] = page.Watermark.Enabled;
                        allProps["waterMarkEnabledExpression"] = StiEncodingHelper.Encode(page.Watermark.EnabledExpression);
                        allProps["waterMarkAngle"] = DoubleToStr(page.Watermark.Angle);
                        allProps["waterMarkText"] = StiEncodingHelper.Encode(page.Watermark.Text);
                        allProps["waterMarkFont"] = FontToStr(page.Watermark.Font);
                        allProps["waterMarkTextBrush"] = BrushToStr(page.Watermark.TextBrush);
                        allProps["waterMarkTextBehind"] = page.Watermark.ShowBehind;
                        allProps["waterMarkImageBehind"] = page.Watermark.ShowImageBehind;
                        allProps["waterMarkImageAlign"] = page.Watermark.ImageAlignment.ToString();
                        allProps["waterMarkMultipleFactor"] = DoubleToStr(page.Watermark.ImageMultipleFactor);
                        allProps["waterMarkStretch"] = page.Watermark.ImageStretch;
                        allProps["waterMarkTiling"] = page.Watermark.ImageTiling;
                        allProps["waterMarkTransparency"] = page.Watermark.ImageTransparency.ToString();
                        allProps["watermarkImageSrc"] = page.Watermark.TakeImage() != null ? ImageToBase64(page.Watermark.TakeImage()) : String.Empty;
                        allProps["watermarkImageHyperlink"] = page.Watermark.ImageHyperlink;

                        if (page.Watermark.TakeGdiImage() != null)
                        {
                            using (var gdiImage = page.Watermark.TakeGdiImage())
                            {
                                allProps["watermarkImageSize"] = gdiImage.Width + ";" + gdiImage.Height;
                            }
                        }
                        else allProps["watermarkImageSize"] = "0;0";

                        allProps["watermarkImageContentForPaint"] = GetWatermarkImageContentForPaint(page, allProps);
                        allProps["stopBeforePrint"] = page.StopBeforePrint.ToString();
                        allProps["titleBeforeHeader"] = page.TitleBeforeHeader;
                        allProps["aliasName"] = StiEncodingHelper.Encode(page.Alias);
                        allProps["largeHeight"] = page.LargeHeight;
                        allProps["largeHeightFactor"] = page.LargeHeightFactor.ToString();
                        allProps["largeHeightAutoFactor"] = page.LargeHeightAutoFactor.ToString();
                        allProps["resetPageNumber"] = page.ResetPageNumber;
                        allProps["printOnPreviousPage"] = page.PrintOnPreviousPage;
                        allProps["printHeadersFootersFromPreviousPage"] = page.PrintHeadersFootersFromPreviousPage;
                        allProps["enabled"] = page.Enabled;
                        allProps["mirrorMargins"] = page.MirrorMargins;
                        allProps["segmentPerWidth"] = page.SegmentPerWidth.ToString();
                        allProps["segmentPerHeight"] = page.SegmentPerHeight.ToString();
                        allProps["unlimitedHeight"] = page.UnlimitedHeight;
                        allProps["unlimitedBreakable"] = page.UnlimitedBreakable;
                        allProps["numberOfCopies"] = page.NumberOfCopies.ToString();
                        allProps["excelSheet"] = StiEncodingHelper.Encode(page.ExcelSheet?.Value);
                        allProps["pageIcon"] = page.Icon != null ? ImageToBase64(page.Icon) : string.Empty; 
                        break;
                }
                #endregion

                #region StiText
                case "StiText":
                    {
                        allProps["border"] = BorderToStr(((StiText)component).Border);
                        allProps["brush"] = BrushToStr(((StiText)component).Brush);
                        allProps["text"] = StiEncodingHelper.Encode(((StiText)component).Text.Value);
                        allProps["textBrush"] = BrushToStr(((StiText)component).TextBrush);
                        allProps["font"] = FontToStr(((StiText)component).Font);
                        allProps["wordWrap"] = ((StiText)component).WordWrap;
                        allProps["hideZeros"] = ((StiText)component).HideZeros;
                        allProps["maxNumberOfLines"] = ((StiText)component).MaxNumberOfLines.ToString();
                        allProps["onlyText"] = ((StiText)component).OnlyText;
                        allProps["textMargins"] = MarginsToStr(((StiText)component).Margins);
                        allProps["allowHtmlTags"] = ((StiText)component).AllowHtmlTags;
                        allProps["editableText"] = ((StiText)component).Editable;
                        allProps["textAngle"] = DoubleToStr(((StiText)component).Angle);
                        allProps["horAlignment"] = ((StiText)component).HorAlignment.ToString();
                        allProps["vertAlignment"] = ((StiText)component).VertAlignment.ToString();
                        allProps["textFormat"] = StiTextFormatHelper.GetTextFormatItem(((StiText)component).TextFormat);
                        allProps["canBreak"] = ((StiText)component).CanBreak;
                        allProps["growToHeight"] = ((StiText)component).GrowToHeight;
                        allProps["autoWidth"] = ((StiText)component).AutoWidth;
                        allProps["printOn"] = ((StiText)component).PrintOn.ToString();
                        allProps["printable"] = ((StiText)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["trimming"] = ((StiText)component).TextOptions.Trimming.ToString();
                        allProps["textOptionsRightToLeft"] = ((StiText)component).TextOptions.RightToLeft;
                        allProps["processAt"] = ((StiText)component).ProcessAt.ToString();
                        allProps["processingDuplicates"] = ((StiText)component).ProcessingDuplicates.ToString();
                        allProps["shrinkFontToFit"] = ((StiText)component).ShrinkFontToFit;
                        allProps["shrinkFontToFitMinimumSize"] = ((StiText)component).ShrinkFontToFitMinimumSize.ToString();
                        allProps["textType"] = ((StiText)component).Type.ToString();
                        allProps["enabled"] = ((StiText)component).Enabled;
                        allProps["canGrow"] = ((StiText)component).CanGrow;
                        allProps["canShrink"] = ((StiText)component).CanShrink;
                        allProps["editable"] = ((StiText)component).Editable;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiText)component).DockStyle.ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["renderTo"] = ((StiText)component).RenderTo;
                        allProps["globalizedName"] = ((StiText)component).GlobalizedName;
                        allProps["excelValue"] = StiEncodingHelper.Encode(((StiText)component).ExcelValue.Value);
                        allProps["exportAsImage"] = ((StiText)component).ExportAsImage;
                        allProps["lineSpacing"] = DoubleToStr(((StiText)component).LineSpacing);
                        allProps["linesOfUnderline"] = ((int)((StiText)component).LinesOfUnderline).ToString();
                        allProps["nullValue"] = ((StiText)component).NullValue;
                        break;
                    }
                #endregion               

                #region StiTextInCells
                case "StiTextInCells":
                    {
                        allProps["border"] = BorderToStr(((StiTextInCells)component).Border);
                        allProps["brush"] = BrushToStr(((StiTextInCells)component).Brush);
                        allProps["text"] = StiEncodingHelper.Encode(((StiTextInCells)component).Text.Value);
                        allProps["textFormat"] = StiTextFormatHelper.GetTextFormatItem(((StiTextInCells)component).TextFormat);
                        allProps["textBrush"] = BrushToStr(((StiTextInCells)component).TextBrush);
                        allProps["font"] = FontToStr(((StiTextInCells)component).Font);
                        allProps["horAlignment"] = ((StiTextInCells)component).HorAlignment.ToString();
                        allProps["wordWrap"] = ((StiTextInCells)component).WordWrap;
                        allProps["textMargins"] = MarginsToStr(((StiTextInCells)component).Margins);
                        allProps["onlyText"] = ((StiTextInCells)component).OnlyText;
                        allProps["cellWidth"] = DoubleToStr(((StiTextInCells)component).CellWidth);
                        allProps["cellHeight"] = DoubleToStr(((StiTextInCells)component).CellHeight);
                        allProps["horizontalSpacing"] = DoubleToStr(((StiTextInCells)component).HorSpacing);
                        allProps["verticalSpacing"] = DoubleToStr(((StiTextInCells)component).VertSpacing);
                        allProps["rightToLeft"] = ((StiTextInCells)component).RightToLeft;
                        allProps["editableText"] = ((StiTextInCells)component).Editable;
                        allProps["canBreak"] = ((StiTextInCells)component).CanBreak;
                        allProps["growToHeight"] = ((StiTextInCells)component).GrowToHeight;
                        allProps["printOn"] = ((StiTextInCells)component).PrintOn.ToString();
                        allProps["printable"] = ((StiTextInCells)component).Printable;
                        allProps["hideZeros"] = ((StiTextInCells)component).HideZeros;
                        allProps["continuousText"] = ((StiTextInCells)component).ContinuousText;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["processAt"] = ((StiTextInCells)component).ProcessAt.ToString();
                        allProps["textType"] = ((StiTextInCells)component).Type.ToString();
                        allProps["enabled"] = ((StiTextInCells)component).Enabled;
                        allProps["canGrow"] = ((StiTextInCells)component).CanGrow;
                        allProps["canShrink"] = ((StiTextInCells)component).CanShrink;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiTextInCells)component).DockStyle.ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["globalizedName"] = ((StiTextInCells)component).GlobalizedName;
                        allProps["excelValue"] = StiEncodingHelper.Encode(((StiTextInCells)component).ExcelValue.Value);
                        allProps["exportAsImage"] = ((StiTextInCells)component).ExportAsImage;
                        allProps["lineSpacing"] = DoubleToStr(((StiTextInCells)component).LineSpacing);
                        break;
                    }
                #endregion               

                #region StiRichText
                case "StiRichText":
                    {
                        allProps["border"] = BorderToStr(((StiRichText)component).Border);
                        allProps["backColor"] = GetStringFromColor(((StiRichText)component).BackColor);
                        allProps["richText"] = GetRichTextProperty((StiRichText)component);
                        allProps["richTextDataColumn"] = ((StiRichText)component).DataColumn != null ? StiEncodingHelper.Encode(((StiRichText)component).DataColumn) : string.Empty;
                        allProps["richTextUrl"] = ((StiRichText)component).DataUrl != null ? StiEncodingHelper.Encode(((StiRichText)component).DataUrl.Value) : string.Empty;
                        allProps["editableText"] = ((StiRichText)component).Editable;
                        allProps["textMargins"] = MarginsToStr(((StiRichText)component).Margins);
                        allProps["onlyText"] = ((StiRichText)component).OnlyText;
                        allProps["wordWrap"] = ((StiRichText)component).WordWrap;
                        allProps["canBreak"] = ((StiRichText)component).CanBreak;
                        allProps["growToHeight"] = ((StiRichText)component).GrowToHeight;
                        allProps["printOn"] = ((StiRichText)component).PrintOn.ToString();
                        allProps["printable"] = ((StiRichText)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["processAt"] = ((StiRichText)component).ProcessAt.ToString();
                        allProps["enabled"] = ((StiRichText)component).Enabled;
                        allProps["canGrow"] = ((StiRichText)component).CanGrow;
                        allProps["canShrink"] = ((StiRichText)component).CanShrink;
                        allProps["editable"] = ((StiRichText)component).Editable;
                        allProps["detectUrls"] = ((StiRichText)component).DetectUrls;
                        allProps["rightToLeft"] = ((StiRichText)component).RightToLeft;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiRichText)component).DockStyle.ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        break;
                    }
                #endregion               

                #region StiImage
                case "StiImage":
                    {
                        allProps["border"] = BorderToStr(((StiImage)component).Border);
                        allProps["brush"] = BrushToStr(((StiImage)component).Brush);
                        allProps["horAlignment"] = ((StiImage)component).HorAlignment.ToString();
                        allProps["vertAlignment"] = ((StiImage)component).VertAlignment.ToString();
                        allProps["stretch"] = ((StiImage)component).Stretch;
                        allProps["ratio"] = ((StiImage)component).AspectRatio;
                        allProps["rotation"] = ((StiImage)component).ImageRotation.ToString();
                        allProps["imageMultipleFactor"] = DoubleToStr(((StiImage)component).MultipleFactor);
                        allProps["canBreak"] = ((StiImage)component).CanBreak;
                        allProps["growToHeight"] = ((StiImage)component).GrowToHeight;
                        allProps["printOn"] = ((StiImage)component).PrintOn.ToString();
                        allProps["printable"] = ((StiImage)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["imageDataColumn"] = ((StiImage)component).DataColumn != null ? StiEncodingHelper.Encode(((StiImage)component).DataColumn) : string.Empty;
                        allProps["imageSrc"] = ((StiImage)component).TakeImage() != null ? ImageToBase64(((StiImage)component).TakeImage()) : string.Empty;
                        allProps["imageUrl"] = ((StiImage)component).ImageURL != null ? StiEncodingHelper.Encode(((StiImage)component).ImageURL.Value) : string.Empty;
                        allProps["imageFile"] = ((StiImage)component).File != null ? StiEncodingHelper.Encode(((StiImage)component).File) : string.Empty;
                        allProps["imageData"] = ((StiImage)component).ImageData != null ? StiEncodingHelper.Encode(((StiImage)component).ImageData.Value) : string.Empty;
                        allProps["icon"] = ((StiImage)component).Icon;
                        allProps["iconColor"] = GetStringFromColor(((StiImage)component).IconColor);
                        allProps["imageContentForPaint"] = GetImageContentForPaint((StiImage)component, requestParams);
                        allProps["enabled"] = ((StiImage)component).Enabled;
                        allProps["canGrow"] = ((StiImage)component).CanGrow;
                        allProps["canShrink"] = ((StiImage)component).CanShrink;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiImage)component).DockStyle.ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["globalizedName"] = ((StiImage)component).GlobalizedName;
                        allProps["imageMargins"] = MarginsToStr(((StiImage)component).Margins);
                        allProps["imageSmoothing"] = ((StiImage)component).Smoothing;
                        allProps["imageProcessingDuplicates"] = ((StiImage)component).ProcessingDuplicates.ToString();
                        break;
                    }
                #endregion               

                #region StiBarCode
                case "StiBarCode":
                    {
                        allProps["border"] = BorderToStr(((StiBarCode)component).Border);
                        allProps["code"] = ((StiBarCode)component).Code != null ? StiEncodingHelper.Encode(((StiBarCode)component).Code.Value) : string.Empty;
                        allProps["codeType"] = ((StiBarCode)component).BarCodeType.GetType().Name;
                        allProps["horAlignment"] = ((StiBarCode)component).HorAlignment.ToString();
                        allProps["vertAlignment"] = ((StiBarCode)component).VertAlignment.ToString();
                        allProps["font"] = FontToStr(((StiBarCode)component).Font);
                        allProps["barCodeAngle"] = ((int)((StiBarCode)component).Angle).ToString();
                        allProps["autoScale"] = ((StiBarCode)component).AutoScale;
                        allProps["showLabelText"] = ((StiBarCode)component).ShowLabelText;
                        allProps["showQuietZones"] = ((StiBarCode)component).ShowQuietZones;
                        allProps["foreColor"] = GetStringFromColor(((StiBarCode)component).ForeColor);
                        allProps["backColor"] = GetStringFromColor(((StiBarCode)component).BackColor);
                        allProps["growToHeight"] = ((StiBarCode)component).GrowToHeight;
                        allProps["printOn"] = ((StiBarCode)component).PrintOn.ToString();
                        allProps["printable"] = ((StiBarCode)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["enabled"] = ((StiBarCode)component).Enabled;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiBarCode)component).DockStyle.ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        break;
                    }
                #endregion               

                #region StiShape
                case "StiShape":
                    {
                        allProps["brush"] = BrushToStr(((StiShape)component).Brush);
                        allProps["shapeType"] = GetShapeTypeProperty((StiShape)component);
                        allProps["shapeBorderStyle"] = ((int)((StiShape)component).Style).ToString();
                        allProps["size"] = DoubleToStr(((StiShape)component).Size);
                        allProps["shapeBorderColor"] = GetStringFromColor(((StiShape)component).BorderColor);
                        allProps["growToHeight"] = ((StiShape)component).GrowToHeight;
                        allProps["printOn"] = ((StiShape)component).PrintOn.ToString();
                        allProps["printable"] = ((StiShape)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["enabled"] = ((StiShape)component).Enabled;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiShape)component).DockStyle.ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["shapeText"] = ((StiShape)component).Text != null ? StiEncodingHelper.Encode(((StiShape)component).Text) : "";
                        allProps["foreColor"] = GetStringFromColor(((StiShape)component).ForeColor);
                        allProps["backgroundColor"] = GetStringFromColor(((StiShape)component).BackgroundColor);
                        allProps["font"] = FontToStr(((StiShape)component).Font);
                        allProps["horAlignment"] = ((StiShape)component).HorAlignment.ToString();
                        allProps["vertAlignment"] = ((StiShape)component).VertAlignment.ToString();
                        allProps["textMargins"] = MarginsToStr(((StiShape)component).Margins);
                        break;
                    }
                #endregion               

                #region StiPanel
                case "StiPanel":
                    {
                        allProps["brush"] = BrushToStr(((StiPanel)component).Brush);
                        allProps["border"] = BorderToStr(((StiPanel)component).Border);
                        allProps["columns"] = ((StiPanel)component).Columns.ToString();
                        allProps["columnWidth"] = DoubleToStr(((StiPanel)component).ColumnWidth);
                        allProps["columnGaps"] = DoubleToStr(((StiPanel)component).ColumnGaps);
                        allProps["rightToLeft"] = ((StiPanel)component).RightToLeft;
                        allProps["growToHeight"] = ((StiPanel)component).GrowToHeight;
                        allProps["printOn"] = ((StiPanel)component).PrintOn.ToString();
                        allProps["printable"] = ((StiPanel)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["enabled"] = ((StiPanel)component).Enabled;
                        allProps["canGrow"] = ((StiPanel)component).CanGrow;
                        allProps["canShrink"] = ((StiPanel)component).CanShrink;
                        allProps["canBreak"] = ((StiPanel)component).CanBreak;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiPanel)component).DockStyle.ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        break;
                    }
                #endregion 

                #region StiClone
                case "StiClone":
                    {
                        allProps["brush"] = BrushToStr(((StiClone)component).Brush);
                        allProps["border"] = BorderToStr(((StiClone)component).Border);
                        allProps["container"] = (((StiClone)component).Container != null) ? ((StiClone)component).Container.Name : "[Not Assigned]";
                        allProps["printOn"] = ((StiClone)component).PrintOn.ToString();
                        allProps["printable"] = ((StiClone)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["enabled"] = ((StiClone)component).Enabled;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiClone)component).DockStyle.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        break;
                    }
                #endregion 

                #region StiCheckBox
                case "StiCheckBox":
                    {
                        allProps["brush"] = BrushToStr(((StiCheckBox)component).Brush);
                        allProps["border"] = BorderToStr(((StiCheckBox)component).Border);
                        allProps["checked"] = StiEncodingHelper.Encode(((StiCheckBox)component).Checked.Value);
                        allProps["checkStyleForTrue"] = ((StiCheckBox)component).CheckStyleForTrue.ToString();
                        allProps["checkStyleForFalse"] = ((StiCheckBox)component).CheckStyleForFalse.ToString();
                        allProps["checkValues"] = ((StiCheckBox)component).Values;
                        allProps["editable"] = ((StiCheckBox)component).Editable;
                        allProps["textBrush"] = BrushToStr(((StiCheckBox)component).TextBrush);
                        allProps["size"] = DoubleToStr(((StiCheckBox)component).Size);
                        allProps["contourColor"] = GetStringFromColor(((StiCheckBox)component).ContourColor);
                        allProps["growToHeight"] = ((StiCheckBox)component).GrowToHeight;
                        allProps["printOn"] = ((StiCheckBox)component).PrintOn.ToString();
                        allProps["printable"] = ((StiCheckBox)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["enabled"] = ((StiCheckBox)component).Enabled;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiCheckBox)component).DockStyle.ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["excelValue"] = StiEncodingHelper.Encode(((StiCheckBox)component).ExcelValue.Value);
                        break;
                    }
                #endregion 

                #region StiSubReport
                case "StiSubReport":
                    {
                        allProps["brush"] = BrushToStr(((StiSubReport)component).Brush);
                        allProps["border"] = BorderToStr(((StiSubReport)component).Border);
                        allProps["subReportPage"] = (((StiSubReport)component).SubReportPage != null) ? ((StiSubReport)component).SubReportPage.Name : "[Not Assigned]";
                        allProps["subReportUrl"] = ((StiSubReport)component).SubReportUrl != null ? StiEncodingHelper.Encode(((StiSubReport)component).SubReportUrl) : string.Empty;
                        allProps["printOn"] = ((StiSubReport)component).PrintOn.ToString();
                        allProps["printable"] = ((StiSubReport)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["enabled"] = ((StiSubReport)component).Enabled;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiSubReport)component).DockStyle.ToString();
                        allProps["subReportParameters"] = GetSubReportParametersProperty((StiSubReport)component);
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["keepSubReportTogether"] = ((StiSubReport)component).KeepSubReportTogether;
                        break;
                    }
                #endregion 

                #region StiZipCode
                case "StiZipCode":
                    {
                        allProps["brush"] = BrushToStr(((StiZipCode)component).Brush);
                        allProps["border"] = BorderToStr(((StiZipCode)component).Border);
                        allProps["code"] = ((StiZipCode)component).Code != null ? StiEncodingHelper.Encode(((StiZipCode)component).Code.Value) : string.Empty;
                        allProps["size"] = DoubleToStr(((StiZipCode)component).Size);
                        allProps["foreColor"] = GetStringFromColor(((StiZipCode)component).ForeColor);
                        allProps["ratio"] = ((StiZipCode)component).Ratio;
                        allProps["growToHeight"] = ((StiZipCode)component).GrowToHeight;
                        allProps["printOn"] = ((StiZipCode)component).PrintOn.ToString();
                        allProps["printable"] = ((StiZipCode)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["enabled"] = ((StiZipCode)component).Enabled;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiZipCode)component).DockStyle.ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["spaceRatio"] = DoubleToStr(((StiZipCode)component).SpaceRatio);
                        allProps["upperMarks"] = ((StiZipCode)component).UpperMarks;
                        break;
                    }
                #endregion 

                #region StiChart
                case "StiChart":
                    {
                        allProps["brush"] = BrushToStr(((StiChart)component).Brush);
                        allProps["border"] = BorderToStr(((StiChart)component).Border);
                        allProps["growToHeight"] = ((StiChart)component).GrowToHeight;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["printOn"] = ((StiChart)component).PrintOn.ToString();
                        allProps["printable"] = ((StiChart)component).Printable;
                        allProps["sortData"] = GetSortDataProperty(component);
                        allProps["filterData"] = StiEncodingHelper.Encode(JSON.Encode(GetFiltersObject(((StiChart)component).Filters)));
                        allProps["filterMode"] = ((StiChart)component).FilterMode.ToString();
                        allProps["filterOn"] = ((StiChart)component).FilterOn;
                        allProps["dataSource"] = (((StiChart)component).DataSource != null) ? ((StiChart)component).DataSource.Name : "[Not Assigned]";
                        allProps["dataRelation"] = (((StiChart)component).DataRelation != null) ? ((StiChart)component).DataRelation.NameInSource : "[Not Assigned]";
                        allProps["masterComponent"] = (((StiChart)component).MasterComponent != null) ? ((StiChart)component).MasterComponent.Name : "[Not Assigned]";
                        allProps["countData"] = ((StiChart)component).CountData.ToString();
                        allProps["businessObject"] = ((StiChart)component).BusinessObject != null ? ((StiChart)component).BusinessObject.GetFullName() : "[Not Assigned]";
                        allProps["enabled"] = ((StiChart)component).Enabled;
                        allProps["chartStyle"] = StiChartHelper.GetStyle((StiChart)component);
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiChart)component).DockStyle.ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["editorType"] = ((StiChart)component).EditorType.ToString();

                        allProps["chartSeries"] = StiChartHelper.GetSeriesArray((StiChart)component);
                        allProps["chartStrips"] = StiChartHelper.GetStrips((StiChart)component);
                        allProps["chartConstantLines"] = StiChartHelper.GetConstantLines((StiChart)component);

                        allProps["allowApplyStyle"] = ((StiChart)component).AllowApplyStyle;
                        allProps["horizontalSpacing"] = ((StiChart)component).HorSpacing.ToString();
                        allProps["processAtEnd"] = ((StiChart)component).ProcessAtEnd;
                        allProps["chartRotation"] = ((StiChart)component).Rotation.ToString();
                        allProps["verticalSpacing"] = ((StiChart)component).VertSpacing.ToString();

                        allProps["areaAllowApplyStyle"] = GetPropertyValue("Area.AllowApplyStyle", component, true);
                        allProps["areaBrush"] = GetPropertyValue("Area.Brush", component, true);
                        allProps["areaBorderColor"] = GetPropertyValue("Area.BorderColor", component, true);
                        allProps["areaBorderThickness"] = GetPropertyValue("Area.BorderThickness", component, true);
                        allProps["areaColorEach"] = GetPropertyValue("Area.ColorEach", component, true);
                        allProps["areaReverseHor"] = GetPropertyValue("Area.ReverseHor", component, true);
                        allProps["areaReverseVert"] = GetPropertyValue("Area.ReverseVert", component, true);
                        allProps["areaSideBySide"] = GetPropertyValue("Area.SideBySide", component, true);

                        allProps["areaGridLinesHorAllowApplyStyle"] = GetPropertyValue("Area.GridLinesHor.AllowApplyStyle", component, true);
                        allProps["areaGridLinesHorColor"] = GetPropertyValue("Area.GridLinesHor.Color", component, true);
                        allProps["areaGridLinesHorMinorColor"] = GetPropertyValue("Area.GridLinesHor.MinorColor", component, true);
                        allProps["areaGridLinesHorMinorCount"] = GetPropertyValue("Area.GridLinesHor.MinorCount", component, true);
                        allProps["areaGridLinesHorMinorStyle"] = GetPropertyValue("Area.GridLinesHor.MinorStyle", component, true);
                        allProps["areaGridLinesHorMinorVisible"] = GetPropertyValue("Area.GridLinesHor.MinorVisible", component, true);
                        allProps["areaGridLinesHorStyle"] = GetPropertyValue("Area.GridLinesHor.Style", component, true);
                        allProps["areaGridLinesHorVisible"] = GetPropertyValue("Area.GridLinesHor.Visible", component, true);

                        allProps["areaGridLinesVertAllowApplyStyle"] = GetPropertyValue("Area.GridLinesVert.AllowApplyStyle", component, true);
                        allProps["areaGridLinesVertColor"] = GetPropertyValue("Area.GridLinesVert.Color", component, true);
                        allProps["areaGridLinesVertMinorColor"] = GetPropertyValue("Area.GridLinesVert.MinorColor", component, true);
                        allProps["areaGridLinesVertMinorCount"] = GetPropertyValue("Area.GridLinesVert.MinorCount", component, true);
                        allProps["areaGridLinesVertMinorStyle"] = GetPropertyValue("Area.GridLinesVert.MinorStyle", component, true);
                        allProps["areaGridLinesVertMinorVisible"] = GetPropertyValue("Area.GridLinesVert.MinorVisible", component, true);
                        allProps["areaGridLinesVertStyle"] = GetPropertyValue("Area.GridLinesVert.Style", component, true);
                        allProps["areaGridLinesVertVisible"] = GetPropertyValue("Area.GridLinesVert.Visible", component, true);

                        allProps["areaInterlacingHorAllowApplyStyle"] = GetPropertyValue("Area.InterlacingHor.AllowApplyStyle", component, true);
                        allProps["areaInterlacingHorColor"] = GetPropertyValue("Area.InterlacingHor.Color", component, true);
                        allProps["areaInterlacingHorVisible"] = GetPropertyValue("Area.InterlacingHor.Visible", component, true);
                        allProps["areaInterlacingHorInterlacedBrush"] = GetPropertyValue("Area.InterlacingHor.InterlacedBrush", component, true);

                        allProps["areaInterlacingVertAllowApplyStyle"] = GetPropertyValue("Area.InterlacingVert.AllowApplyStyle", component, true);
                        allProps["areaInterlacingVertColor"] = GetPropertyValue("Area.InterlacingVert.Color", component, true);
                        allProps["areaInterlacingVertVisible"] = GetPropertyValue("Area.InterlacingVert.Visible", component, true);
                        allProps["areaInterlacingVertInterlacedBrush"] = GetPropertyValue("Area.InterlacingHor.InterlacedBrush", component, true);

                        allProps["xAxisVisible"] = GetPropertyValue("Area.XAxis.Visible", component, true);
                        allProps["xAxisLabelsAngle"] = GetPropertyValue("Area.XAxis.Labels.Angle", component, true);
                        allProps["xAxisLabelsColor"] = GetPropertyValue("Area.XAxis.Labels.Color", component, true);
                        allProps["xAxisLabelsFont"] = GetPropertyValue("Area.XAxis.Labels.Font", component, true);
                        allProps["xAxisLabelsPlacement"] = GetPropertyValue("Area.XAxis.Labels.Placement", component, true);
                        allProps["xAxisLabelsTextAfter"] = GetPropertyValue("Area.XAxis.Labels.TextAfter", component, true);
                        allProps["xAxisLabelsTextAlignment"] = GetPropertyValue("Area.XAxis.Labels.TextAlignment", component, true);
                        allProps["xAxisLabelsTextBefore"] = GetPropertyValue("Area.XAxis.Labels.TextBefore", component, true);
                        allProps["xAxisLabelsStep"] = GetPropertyValue("Area.XAxis.Labels.Step", component, true);
                        allProps["xAxisTitleAlignment"] = GetPropertyValue("Area.XAxis.Title.Alignment", component, true);
                        allProps["xAxisTitleColor"] = GetPropertyValue("Area.XAxis.Title.Color", component, true);
                        allProps["xAxisTitleDirection"] = GetPropertyValue("Area.XAxis.Title.Direction", component, true);
                        allProps["xAxisTitleFont"] = GetPropertyValue("Area.XAxis.Title.Font", component, true);
                        allProps["xAxisTitlePosition"] = GetPropertyValue("Area.XAxis.Title.Position", component, true);
                        allProps["xAxisTitleText"] = GetPropertyValue("Area.XAxis.Title.Text", component, true);
                        allProps["xAxisTitleVisible"] = GetPropertyValue("Area.XAxis.Title.Visible", component, true);
                        allProps["xAxisRangeAuto"] = GetPropertyValue("Area.XAxis.Range.Auto", component, true);
                        allProps["xAxisRangeMinimum"] = GetPropertyValue("Area.XAxis.Range.Minimum", component, true);
                        allProps["xAxisRangeMaximum"] = GetPropertyValue("Area.XAxis.Range.Maximum", component, true);
                        allProps["xAxisShowEdgeValues"] = Convert.ToBoolean(GetPropertyValue("Area.XAxis.ShowEdgeValues", component, true)).ToString();
                        allProps["xAxisStartFromZero"] = Convert.ToBoolean(GetPropertyValue("Area.XAxis.StartFromZero", component, true)).ToString();

                        allProps["xTopAxisVisible"] = GetPropertyValue("Area.XTopAxis.Visible", component, true);
                        allProps["xTopAxisLabelsAngle"] = GetPropertyValue("Area.XTopAxis.Labels.Angle", component, true);
                        allProps["xTopAxisLabelsColor"] = GetPropertyValue("Area.XTopAxis.Labels.Color", component, true);
                        allProps["xTopAxisLabelsFont"] = GetPropertyValue("Area.XTopAxis.Labels.Font", component, true);
                        allProps["xTopAxisLabelsPlacement"] = GetPropertyValue("Area.XTopAxis.Labels.Placement", component, true);
                        allProps["xTopAxisLabelsTextAfter"] = GetPropertyValue("Area.XTopAxis.Labels.TextAfter", component, true);
                        allProps["xTopAxisLabelsTextAlignment"] = GetPropertyValue("Area.XTopAxis.Labels.TextAlignment", component, true);
                        allProps["xTopAxisLabelsTextBefore"] = GetPropertyValue("Area.XTopAxis.Labels.TextBefore", component, true);
                        allProps["xTopAxisLabelsStep"] = GetPropertyValue("Area.XTopAxis.Labels.Step", component, true);
                        allProps["xTopAxisTitleAlignment"] = GetPropertyValue("Area.XTopAxis.Title.Alignment", component, true);
                        allProps["xTopAxisTitleColor"] = GetPropertyValue("Area.XTopAxis.Title.Color", component, true);
                        allProps["xTopAxisTitleDirection"] = GetPropertyValue("Area.XTopAxis.Title.Direction", component, true);
                        allProps["xTopAxisTitleFont"] = GetPropertyValue("Area.XTopAxis.Title.Font", component, true);
                        allProps["xTopAxisTitlePosition"] = GetPropertyValue("Area.XTopAxis.Title.Position", component, true);
                        allProps["xTopAxisTitleText"] = GetPropertyValue("Area.XTopAxis.Title.Text", component, true);
                        allProps["xTopAxisTitleVisible"] = GetPropertyValue("Area.XTopAxis.Title.Visible", component, true);
                        allProps["xTopAxisShowEdgeValues"] = Convert.ToBoolean(GetPropertyValue("Area.XTopAxis.ShowEdgeValues", component, true)).ToString();

                        allProps["yAxisVisible"] = GetPropertyValue("Area.YAxis.Visible", component, true);
                        allProps["yAxisLabelsAngle"] = GetPropertyValue("Area.YAxis.Labels.Angle", component, true);
                        allProps["yAxisLabelsColor"] = GetPropertyValue("Area.YAxis.Labels.Color", component, true);
                        allProps["yAxisLabelsFont"] = GetPropertyValue("Area.YAxis.Labels.Font", component, true);
                        allProps["yAxisLabelsPlacement"] = GetPropertyValue("Area.YAxis.Labels.Placement", component, true);
                        allProps["yAxisLabelsTextAfter"] = GetPropertyValue("Area.YAxis.Labels.TextAfter", component, true);
                        allProps["yAxisLabelsTextAlignment"] = GetPropertyValue("Area.YAxis.Labels.TextAlignment", component, true);
                        allProps["yAxisLabelsTextBefore"] = GetPropertyValue("Area.YAxis.Labels.TextBefore", component, true);
                        allProps["yAxisTitleAlignment"] = GetPropertyValue("Area.YAxis.Title.Alignment", component, true);
                        allProps["yAxisTitleColor"] = GetPropertyValue("Area.YAxis.Title.Color", component, true);
                        allProps["yAxisTitleDirection"] = GetPropertyValue("Area.YAxis.Title.Direction", component, true);
                        allProps["yAxisTitleFont"] = GetPropertyValue("Area.YAxis.Title.Font", component, true);
                        allProps["yAxisTitlePosition"] = GetPropertyValue("Area.YAxis.Title.Position", component, true);
                        allProps["yAxisTitleText"] = GetPropertyValue("Area.YAxis.Title.Text", component, true);
                        allProps["yAxisTitleVisible"] = GetPropertyValue("Area.YAxis.Title.Visible", component, true);
                        allProps["yAxisRangeAuto"] = GetPropertyValue("Area.YAxis.Range.Auto", component, true);
                        allProps["yAxisRangeMinimum"] = GetPropertyValue("Area.YAxis.Range.Minimum", component, true);
                        allProps["yAxisRangeMaximum"] = GetPropertyValue("Area.YAxis.Range.Maximum", component, true);
                        allProps["yAxisStartFromZero"] = GetPropertyValue("Area.YAxis.StartFromZero", component, true);

                        allProps["yRightAxisVisible"] = GetPropertyValue("Area.YRightAxis.Visible", component, true);
                        allProps["yRightAxisLabelsAngle"] = GetPropertyValue("Area.YRightAxis.Labels.Angle", component, true);
                        allProps["yRightAxisLabelsColor"] = GetPropertyValue("Area.YRightAxis.Labels.Color", component, true);
                        allProps["yRightAxisLabelsFont"] = GetPropertyValue("Area.YRightAxis.Labels.Font", component, true);
                        allProps["yRightAxisLabelsPlacement"] = GetPropertyValue("Area.YRightAxis.Labels.Placement", component, true);
                        allProps["yRightAxisLabelsTextAfter"] = GetPropertyValue("Area.YRightAxis.Labels.TextAfter", component, true);
                        allProps["yRightAxisLabelsTextAlignment"] = GetPropertyValue("Area.YRightAxis.Labels.TextAlignment", component, true);
                        allProps["yRightAxisLabelsTextBefore"] = GetPropertyValue("Area.YRightAxis.Labels.TextBefore", component, true);
                        allProps["yRightAxisTitleAlignment"] = GetPropertyValue("Area.YRightAxis.Title.Alignment", component, true);
                        allProps["yRightAxisTitleColor"] = GetPropertyValue("Area.YRightAxis.Title.Color", component, true);
                        allProps["yRightAxisTitleDirection"] = GetPropertyValue("Area.YRightAxis.Title.Direction", component, true);
                        allProps["yRightAxisTitleFont"] = GetPropertyValue("Area.YRightAxis.Title.Font", component, true);
                        allProps["yRightAxisTitlePosition"] = GetPropertyValue("Area.YRightAxis.Title.Position", component, true);
                        allProps["yRightAxisTitleText"] = GetPropertyValue("Area.YRightAxis.Title.Text", component, true);
                        allProps["yRightAxisTitleVisible"] = GetPropertyValue("Area.YRightAxis.Title.Visible", component, true);
                        allProps["yRightAxisStartFromZero"] = GetPropertyValue("Area.YRightAxis.StartFromZero", component, true);


                        allProps["labelsLabelsType"] =  ((StiChart)component).Labels.GetType().Name;
                        allProps["labelsServiceName"] = ((StiChart)component).Labels.ToString();
                        allProps["labelsAllowApplyStyle"] = GetPropertyValue("Labels.AllowApplyStyle", component, true);
                        allProps["labelsAngle"] = GetPropertyValue("Labels.Angle", component, true);
                        allProps["labelsAntialiasing"] = GetPropertyValue("Labels.Antialiasing", component, true);
                        allProps["labelsAutoRotate"] = GetPropertyValue("Labels.AutoRotate", component, true);
                        allProps["labelsDrawBorder"] = GetPropertyValue("Labels.DrawBorder", component, true);
                        allProps["labelsBorderColor"] = GetPropertyValue("Labels.BorderColor", component, true);
                        allProps["labelsBrush"] = GetPropertyValue("Labels.Brush", component, true);
                        allProps["labelsFont"] = GetPropertyValue("Labels.Font", component, true);
                        allProps["labelsFormat"] = GetPropertyValue("Labels.Format", component, true);
                        allProps["labelsLabelColor"] = GetPropertyValue("Labels.LabelColor", component, true);
                        allProps["labelsLegendValueType"] = GetPropertyValue("Labels.LegendValueType", component, true);
                        allProps["labelsLineLength"] = GetPropertyValue("Labels.LineLength", component, true);
                        allProps["labelsMarkerAlignment"] = GetPropertyValue("Labels.MarkerAlignment", component, true);
                        allProps["labelsMarkerSize"] = GetPropertyValue("Labels.MarkerSize", component, true);
                        allProps["labelsMarkerVisible"] = GetPropertyValue("Labels.MarkerVisible", component, true);
                        allProps["labelsPreventIntersection"] = GetPropertyValue("Labels.PreventIntersection", component, true);
                        allProps["labelsShowInPercent"] = GetPropertyValue("Labels.ShowInPercent", component, true);
                        allProps["labelsShowNulls"] = GetPropertyValue("Labels.ShowNulls", component, true);
                        allProps["labelsShowZeros"] = GetPropertyValue("Labels.ShowZeros", component, true);
                        allProps["labelsStep"] = GetPropertyValue("Labels.Step", component, true);
                        allProps["labelsTextAfter"] = GetPropertyValue("Labels.TextAfter", component, true);
                        allProps["labelsTextBefore"] = GetPropertyValue("Labels.TextBefore", component, true);
                        allProps["labelsUseSeriesColor"] = GetPropertyValue("Labels.UseSeriesColor", component, true);
                        allProps["labelsValueType"] = GetPropertyValue("Labels.ValueType", component, true);
                        allProps["labelsValueTypeSeparator"] = GetPropertyValue("Labels.ValueTypeSeparator", component, true);
                        allProps["labelsVisible"] = GetPropertyValue("Labels.Visible", component, true);
                        allProps["labelsWidth"] = GetPropertyValue("Labels.Width", component, true);
                        allProps["labelsWordWrap"] = GetPropertyValue("Labels.WordWrap", component, true);

                        allProps["legendAllowApplyStyle"] = GetPropertyValue("Legend.AllowApplyStyle", component, true);
                        allProps["legendBorderColor"] = GetPropertyValue("Legend.BorderColor", component, true);
                        allProps["legendBrush"] = GetPropertyValue("Legend.Brush", component, true);
                        allProps["legendFont"] = GetPropertyValue("Legend.Font", component, true);
                        allProps["legendHorAlignment"] = GetPropertyValue("Legend.HorAlignment", component, true);
                        allProps["legendVertAlignment"] = GetPropertyValue("Legend.VertAlignment", component, true);
                        allProps["legendColumns"] = GetPropertyValue("Legend.Columns", component, true);
                        allProps["legendDirection"] = GetPropertyValue("Legend.Direction", component, true);
                        allProps["legendVisible"] = GetPropertyValue("Legend.Visible", component, true);
                        allProps["legendLabelsColor"] = GetPropertyValue("Legend.LabelsColor", component, true);
                        allProps["legendLabelsFont"] = GetPropertyValue("Legend.Font", component, true);
                        allProps["legendTitleColor"] = GetPropertyValue("Legend.TitleColor", component, true);
                        allProps["legendTitleFont"] = GetPropertyValue("Legend.TitleFont", component, true);
                        allProps["legendTitleText"] = GetPropertyValue("Legend.Title", component, true);

                        allProps["chartTitleText"] = GetPropertyValue("Title.Text", component, true);
                        allProps["chartTitleAllowApplyStyle"] = GetPropertyValue("Title.AllowApplyStyle", component, true);
                        allProps["chartTitleAlignment"] = GetPropertyValue("Title.Alignment", component, true);
                        allProps["chartTitleAntialiasing"] = GetPropertyValue("Title.Antialiasing", component, true);
                        allProps["chartTitleBrush"] = GetPropertyValue("Title.Brush", component, true);
                        allProps["chartTitleDock"] = GetPropertyValue("Title.Dock", component, true);
                        allProps["chartTitleFont"] = GetPropertyValue("Title.Font", component, true);
                        allProps["chartTitleSpacing"] = GetPropertyValue("Title.Spacing", component, true);
                        allProps["chartTitleVisible"] = GetPropertyValue("Title.Visible", component, true);

                        allProps["chartTableAllowApplyStyle"] = GetPropertyValue("Table.AllowApplyStyle", component, true);
                        allProps["chartTableGridLineColor"] = GetPropertyValue("Table.GridLineColor", component, true);
                        allProps["chartTableGridLinesHor"] = GetPropertyValue("Table.GridLinesHor", component, true);
                        allProps["chartTableGridLinesVert"] = GetPropertyValue("Table.GridLinesVert", component, true);
                        allProps["chartTableGridOutline"] = GetPropertyValue("Table.GridOutline", component, true);
                        allProps["chartTableMarkerVisible"] = GetPropertyValue("Table.MarkerVisible", component, true);
                        allProps["chartTableVisible"] = GetPropertyValue("Table.Visible", component, true);
                        allProps["chartTableDataCellsTextColor"] = GetPropertyValue("Table.DataCells.TextColor", component, true);
                        allProps["chartTableDataCellsShrinkFontToFit"] = GetPropertyValue("Table.DataCells.ShrinkFontToFit", component, true);
                        allProps["chartTableDataCellsShrinkFontToFitMinimumSize"] = GetPropertyValue("Table.DataCells.ShrinkFontToFitMinimumSize", component, true);
                        allProps["chartTableDataCellsFont"] = GetPropertyValue("Table.DataCells.Font", component, true);
                        allProps["chartTableHeaderTextAfter"] = GetPropertyValue("Table.Header.TextAfter", component, true);
                        allProps["chartTableHeaderTextColor"] = GetPropertyValue("Table.Header.TextColor", component, true);
                        allProps["chartTableHeaderWordWrap"] = GetPropertyValue("Table.Header.WordWrap", component, true);
                        allProps["chartTableHeaderBrush"] = GetPropertyValue("Table.Header.Brush", component, true);
                        allProps["chartTableHeaderFont"] = GetPropertyValue("Table.Header.Font", component, true);

                        allProps["isAxisAreaChart"] = StiChartHelper.IsAxisAreaChart((StiChart)component);
                        allProps["isAxisAreaChart3D"] = StiChartHelper.IsAxisAreaChart3D((StiChart)component);
                        allProps["isClusteredColumnChart3D"] = StiChartHelper.IsClusteredColumnChart3D((StiChart)component);                        
                        allProps["isPieChart"] = StiChartHelper.IsPieChart((StiChart)component);
                        allProps["isDoughnutChart"] = StiChartHelper.IsDoughnutChart((StiChart)component);
                        allProps["isFunnelChart"] = StiChartHelper.IsFunnelChart((StiChart)component);
                        allProps["isTreemapChart"] = StiChartHelper.IsTreemapChart((StiChart)component);
                        allProps["isSunburstChart"] = StiChartHelper.IsSunburstChart((StiChart)component);
                        allProps["isStackedChart"] = StiChartHelper.IsStackedChart((StiChart)component);
                        allProps["isWaterfallChart"] = StiChartHelper.IsWaterfallChart((StiChart)component);
                        allProps["isPictorialStackedChart"] = StiChartHelper.IsPictorialStackedChart((StiChart)component);                        
                        break;
                    }
                #endregion 

                #region StiGauge
                case "StiGauge":
                    {
                        allProps["brush"] = BrushToStr(((StiGauge)component).Brush);
                        allProps["border"] = BorderToStr(((StiGauge)component).Border);
                        allProps["growToHeight"] = ((StiGauge)component).GrowToHeight;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["printOn"] = ((StiGauge)component).PrintOn.ToString();
                        allProps["printable"] = ((StiGauge)component).Printable;
                        allProps["enabled"] = ((StiGauge)component).Enabled;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiGauge)component).DockStyle.ToString();
                        allProps["gaugeStyle"] = StiGaugeHelper.GetStyle((StiGauge)component);
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["allowApplyStyle"] = ((StiGauge)component).AllowApplyStyle;
                        allProps["shortValue"] = ((StiGauge)component).ShortValue;
                        break;
                    }
                #endregion 

                #region StiMap
                case "StiMap":
                    {
                        allProps["brush"] = BrushToStr(((StiMap)component).Brush);
                        allProps["border"] = BorderToStr(((StiMap)component).Border);
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["printOn"] = ((StiMap)component).PrintOn.ToString();
                        allProps["printable"] = ((StiMap)component).Printable;
                        allProps["enabled"] = ((StiMap)component).Enabled;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiMap)component).DockStyle.ToString();
                        allProps["mapStyle"] = StiMapHelper.GetStyle((StiMap)component);
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["colorEach"] = ((StiMap)component).ColorEach;
                        allProps["showValue"] = ((StiMap)component).ShowValue;
                        allProps["shortValue"] = ((StiMap)component).ShortValue;
                        allProps["showZeros"] = ((StiMap)component).ShowZeros;
                        allProps["stretch"] = ((StiMap)component).Stretch;
                        allProps["displayNameType"] = ((StiMap)component).DisplayNameType.ToString();
                        break;
                    }
                #endregion

                #region StiSparkline
                case "StiSparkline":
                    {
                        allProps["valueDataColumn"] = StiEncodingHelper.Encode(((StiSparkline)component).ValueDataColumn);
                        allProps["dataRelation"] = (((StiSparkline)component).DataRelation != null) ? ((StiSparkline)component).DataRelation.NameInSource : "[Not Assigned]";
                        allProps["brush"] = BrushToStr(((StiSparkline)component).Brush);
                        allProps["border"] = BorderToStr(((StiSparkline)component).Border);
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["printOn"] = ((StiSparkline)component).PrintOn.ToString();
                        allProps["printable"] = ((StiSparkline)component).Printable;
                        allProps["enabled"] = ((StiSparkline)component).Enabled;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiSparkline)component).DockStyle.ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["growToHeight"] = ((StiSparkline)component).GrowToHeight;
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        break;
                    }
                #endregion

                #region StiMathFormula
                case "StiMathFormula":
                    {
                        allProps["laTexExpression"] = StiEncodingHelper.Encode(((StiMathFormula)component).LaTexExpression);
                        allProps["border"] = BorderToStr(((StiMathFormula)component).Border);
                        allProps["brush"] = BrushToStr(((StiMathFormula)component).Brush);
                        allProps["textBrush"] = BrushToStr(((StiMathFormula)component).TextBrush);
                        allProps["font"] = FontToStr(((StiMathFormula)component).Font);
                        allProps["horAlignment"] = ((StiMathFormula)component).HorAlignment.ToString();
                        allProps["vertAlignment"] = ((StiMathFormula)component).VertAlignment.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["enabled"] = ((StiMathFormula)component).Enabled;
                        allProps["canGrow"] = ((StiMathFormula)component).CanGrow;
                        allProps["canShrink"] = ((StiMathFormula)component).CanShrink;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["dockStyle"] = ((StiMathFormula)component).DockStyle.ToString();
                        allProps["growToHeight"] = ((StiMathFormula)component).GrowToHeight;
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["printOn"] = ((StiMathFormula)component).PrintOn.ToString();
                        allProps["printable"] = ((StiMathFormula)component).Printable;
                        break;
                    }
                #endregion

                #region StiElectronicSignature
                case "StiElectronicSignature":
                    {
                        var signature = component as StiElectronicSignature;
                        allProps["border"] = BorderToStr(signature.Border);
                        allProps["brush"] = BrushToStr(signature.Brush);
                        allProps["minSize"] = DoubleToStr(signature.MinSize.Width) + ";" + DoubleToStr(signature.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(signature.MaxSize.Width) + ";" + DoubleToStr(signature.MaxSize.Height);
                        allProps["enabled"] = signature.Enabled;
                        allProps["canGrow"] = signature.CanGrow;
                        allProps["canShrink"] = signature.CanShrink;
                        allProps["shiftMode"] = signature.ShiftMode.ToString();
                        allProps["dockStyle"] = signature.DockStyle.ToString();
                        allProps["growToHeight"] = signature.GrowToHeight;
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["printOn"] = signature.PrintOn.ToString();
                        allProps["printable"] = signature.Printable;
                        allProps["signatureMode"] = signature.Mode.ToString();
                        allProps["typeFullName"] = signature.Type.FullName;
                        allProps["typeInitials"] = signature.Type.Initials;
                        allProps["typeStyle"] = signature.Type.Style;
                        allProps["drawText"] = StiEncodingHelper.Encode(signature.Text.Text);
                        allProps["drawTextFont"] = FontToStr(signature.Text.Font);
                        allProps["drawTextColor"] = GetStringFromColor(signature.Text.Color);
                        allProps["drawTextHorAlignment"] = signature.Text.HorAlignment.ToString();
                        allProps["drawImage"] = signature.Image.Image != null ? ImageToBase64(signature.Image.Image) : string.Empty;
                        allProps["drawImageHorAlignment"] = signature.Image.HorAlignment.ToString();
                        allProps["drawImageVertAlignment"] = signature.Image.VertAlignment.ToString();
                        allProps["drawImageStretch"] = signature.Image.Stretch;
                        allProps["drawImageAspectRatio"] = signature.Image.AspectRatio;

                        break;
                    }
                #endregion

                #region StiPdfDigitalSignature
                case "StiPdfDigitalSignature":
                    {
                        var signature = component as StiPdfDigitalSignature;
                        allProps["placeholder"] = StiEncodingHelper.Encode(signature.Placeholder);
                        allProps["border"] = BorderToStr(signature.Border);
                        allProps["brush"] = BrushToStr(signature.Brush);
                        allProps["minSize"] = DoubleToStr(signature.MinSize.Width) + ";" + DoubleToStr(signature.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(signature.MaxSize.Width) + ";" + DoubleToStr(signature.MaxSize.Height);
                        allProps["enabled"] = signature.Enabled;
                        allProps["canGrow"] = signature.CanGrow;
                        allProps["canShrink"] = signature.CanShrink;
                        allProps["shiftMode"] = signature.ShiftMode.ToString();
                        allProps["dockStyle"] = signature.DockStyle.ToString();
                        allProps["growToHeight"] = signature.GrowToHeight;
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["printOn"] = signature.PrintOn.ToString();
                        allProps["printable"] = signature.Printable;
                        break;
                    }
                #endregion

                #region StiTableCell
                case "StiTableCell":
                    {
                        allProps["cellType"] = ((StiTableCell)component).CellType.ToString();
                        allProps["cellDockStyle"] = ((StiTableCell)component).CellDockStyle.ToString();
                        allProps["fixedWidth"] = ((StiTableCell)component).FixedWidth;
                        allProps["border"] = BorderToStr(((StiTableCell)component).Border);
                        allProps["brush"] = BrushToStr(((StiTableCell)component).Brush);
                        allProps["text"] = StiEncodingHelper.Encode(((StiTableCell)component).Text.Value);
                        allProps["textBrush"] = BrushToStr(((StiTableCell)component).TextBrush);
                        allProps["font"] = FontToStr(((StiTableCell)component).Font);
                        allProps["wordWrap"] = ((StiTableCell)component).WordWrap;
                        allProps["hideZeros"] = ((StiTableCell)component).HideZeros;
                        allProps["maxNumberOfLines"] = ((StiTableCell)component).MaxNumberOfLines.ToString();
                        allProps["onlyText"] = ((StiTableCell)component).OnlyText;
                        allProps["textMargins"] = MarginsToStr(((StiTableCell)component).Margins);
                        allProps["allowHtmlTags"] = ((StiTableCell)component).AllowHtmlTags;
                        allProps["editableText"] = ((StiTableCell)component).Editable;
                        allProps["textAngle"] = DoubleToStr(((StiTableCell)component).Angle);
                        allProps["horAlignment"] = ((StiTableCell)component).HorAlignment.ToString();
                        allProps["vertAlignment"] = ((StiTableCell)component).VertAlignment.ToString();
                        allProps["textFormat"] = StiTextFormatHelper.GetTextFormatItem(((StiTableCell)component).TextFormat);
                        allProps["canBreak"] = ((StiTableCell)component).CanBreak;
                        allProps["printOn"] = ((StiTableCell)component).PrintOn.ToString();
                        allProps["printable"] = ((StiTableCell)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["trimming"] = ((StiTableCell)component).TextOptions.Trimming.ToString();
                        allProps["textOptionsRightToLeft"] = ((StiTableCell)component).TextOptions.RightToLeft;
                        allProps["processAt"] = ((StiTableCell)component).ProcessAt.ToString();
                        allProps["processingDuplicates"] = ((StiTableCell)component).ProcessingDuplicates.ToString();
                        allProps["shrinkFontToFit"] = ((StiTableCell)component).ShrinkFontToFit;
                        allProps["shrinkFontToFitMinimumSize"] = ((StiTableCell)component).ShrinkFontToFitMinimumSize.ToString();
                        allProps["textType"] = ((StiTableCell)component).Type.ToString();
                        allProps["enabled"] = ((StiTableCell)component).Enabled;
                        allProps["canGrow"] = ((StiTableCell)component).CanGrow;
                        allProps["canShrink"] = ((StiTableCell)component).CanShrink;
                        allProps["editable"] = ((StiTableCell)component).Editable;
                        allProps["excelValue"] = StiEncodingHelper.Encode(((StiTableCell)component).ExcelValue.Value);
                        allProps["exportAsImage"] = ((StiTableCell)component).ExportAsImage;
                        allProps["lineSpacing"] = DoubleToStr(((StiTableCell)component).LineSpacing);
                        allProps["merged"] = ((StiTableCell)component).Merged;
                        break;
                    }
                #endregion

                #region StiTableCellImage
                case "StiTableCellImage":
                    {
                        allProps["cellType"] = ((StiTableCellImage)component).CellType.ToString();
                        allProps["cellDockStyle"] = ((StiTableCellImage)component).CellDockStyle.ToString();
                        allProps["fixedWidth"] = ((StiTableCellImage)component).FixedWidth;
                        allProps["border"] = BorderToStr(((StiTableCellImage)component).Border);
                        allProps["brush"] = BrushToStr(((StiTableCellImage)component).Brush);
                        allProps["horAlignment"] = ((StiTableCellImage)component).HorAlignment.ToString();
                        allProps["vertAlignment"] = ((StiTableCellImage)component).VertAlignment.ToString();
                        allProps["stretch"] = ((StiTableCellImage)component).Stretch;
                        allProps["ratio"] = ((StiTableCellImage)component).AspectRatio;
                        allProps["rotation"] = ((StiTableCellImage)component).ImageRotation.ToString();
                        allProps["imageMultipleFactor"] = DoubleToStr(((StiTableCellImage)component).MultipleFactor);
                        allProps["canBreak"] = ((StiTableCellImage)component).CanBreak;
                        allProps["printOn"] = ((StiTableCellImage)component).PrintOn.ToString();
                        allProps["printable"] = ((StiTableCellImage)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["imageDataColumn"] = ((StiTableCellImage)component).DataColumn != null ? StiEncodingHelper.Encode(((StiTableCellImage)component).DataColumn) : string.Empty;
                        allProps["imageSrc"] = ((StiTableCellImage)component).TakeImage() != null ? ImageToBase64(((StiTableCellImage)component).TakeImage()) : string.Empty;
                        allProps["imageUrl"] = ((StiTableCellImage)component).ImageURL != null ? StiEncodingHelper.Encode(((StiTableCellImage)component).ImageURL.Value) : string.Empty;
                        allProps["imageFile"] = ((StiTableCellImage)component).File != null ? StiEncodingHelper.Encode(((StiTableCellImage)component).File) : string.Empty;
                        allProps["imageData"] = ((StiTableCellImage)component).ImageData != null ? StiEncodingHelper.Encode(((StiTableCellImage)component).ImageData.Value) : string.Empty;
                        allProps["icon"] = ((StiTableCellImage)component).Icon;
                        allProps["iconColor"] = GetStringFromColor(((StiTableCellImage)component).IconColor);
                        allProps["imageContentForPaint"] = GetImageContentForPaint((StiImage)component);
                        allProps["enabled"] = ((StiTableCellImage)component).Enabled;
                        allProps["canGrow"] = ((StiTableCellImage)component).CanGrow;
                        allProps["canShrink"] = ((StiTableCellImage)component).CanShrink;
                        allProps["merged"] = ((StiTableCellImage)component).Merged;
                        allProps["imageMargins"] = MarginsToStr(((StiTableCellImage)component).Margins);
                        break;
                    }
                #endregion

                #region StiTableCellCheckBox
                case "StiTableCellCheckBox":
                    {
                        allProps["cellType"] = ((StiTableCellCheckBox)component).CellType.ToString();
                        allProps["cellDockStyle"] = ((StiTableCellCheckBox)component).CellDockStyle.ToString();
                        allProps["fixedWidth"] = ((StiTableCellCheckBox)component).FixedWidth;
                        allProps["brush"] = BrushToStr(((StiTableCellCheckBox)component).Brush);
                        allProps["border"] = BorderToStr(((StiTableCellCheckBox)component).Border);
                        allProps["checked"] = StiEncodingHelper.Encode(((StiTableCellCheckBox)component).Checked.Value);
                        allProps["checkStyleForTrue"] = ((StiTableCellCheckBox)component).CheckStyleForTrue.ToString();
                        allProps["checkStyleForFalse"] = ((StiTableCellCheckBox)component).CheckStyleForFalse.ToString();
                        allProps["checkValues"] = ((StiTableCellCheckBox)component).Values;
                        allProps["editable"] = ((StiTableCellCheckBox)component).Editable;
                        allProps["textBrush"] = BrushToStr(((StiTableCellCheckBox)component).TextBrush);
                        allProps["size"] = DoubleToStr(((StiTableCellCheckBox)component).Size);
                        allProps["contourColor"] = GetStringFromColor(((StiTableCellCheckBox)component).ContourColor);
                        allProps["printOn"] = ((StiTableCellCheckBox)component).PrintOn.ToString();
                        allProps["printable"] = ((StiTableCellCheckBox)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["enabled"] = ((StiTableCellCheckBox)component).Enabled;
                        allProps["excelValue"] = StiEncodingHelper.Encode(((StiTableCellCheckBox)component).ExcelValue.Value);
                        allProps["merged"] = ((StiTableCellCheckBox)component).Merged;
                        break;
                    }
                #endregion

                #region StiTableCellRichText
                case "StiTableCellRichText":
                    {
                        allProps["cellType"] = ((StiTableCellRichText)component).CellType.ToString();
                        allProps["cellDockStyle"] = ((StiTableCellRichText)component).CellDockStyle.ToString();
                        allProps["fixedWidth"] = ((StiTableCellRichText)component).FixedWidth;
                        allProps["border"] = BorderToStr(((StiTableCellRichText)component).Border);
                        allProps["richText"] = GetRichTextProperty((StiTableCellRichText)component);
                        allProps["richTextDataColumn"] = ((StiTableCellRichText)component).DataColumn != null ? StiEncodingHelper.Encode(((StiTableCellRichText)component).DataColumn) : string.Empty;
                        allProps["richTextUrl"] = ((StiTableCellRichText)component).DataUrl != null ? StiEncodingHelper.Encode(((StiTableCellRichText)component).DataUrl.Value) : string.Empty;
                        allProps["editableText"] = ((StiTableCellRichText)component).Editable;
                        allProps["textMargins"] = MarginsToStr(((StiTableCellRichText)component).Margins);
                        allProps["onlyText"] = ((StiTableCellRichText)component).OnlyText;
                        allProps["wordWrap"] = ((StiTableCellRichText)component).WordWrap;
                        allProps["canBreak"] = ((StiTableCellRichText)component).CanBreak;
                        allProps["printOn"] = ((StiTableCellRichText)component).PrintOn.ToString();
                        allProps["printable"] = ((StiTableCellRichText)component).Printable;
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["processAt"] = ((StiTableCellRichText)component).ProcessAt.ToString();
                        allProps["enabled"] = ((StiTableCellRichText)component).Enabled;
                        allProps["canGrow"] = ((StiTableCellRichText)component).CanGrow;
                        allProps["canShrink"] = ((StiTableCellRichText)component).CanShrink;
                        allProps["editable"] = ((StiTableCellRichText)component).Editable;
                        allProps["merged"] = ((StiTableCellRichText)component).Merged;
                        break;
                    }
                #endregion


                #region StiHorizontalLinePrimitive
                case "StiHorizontalLinePrimitive":
                    {
                        allProps["color"] = GetStringFromColor(((StiHorizontalLinePrimitive)component).Color);
                        allProps["size"] = DoubleToStr(((StiHorizontalLinePrimitive)component).Size);
                        allProps["style"] = ((int)((StiHorizontalLinePrimitive)component).Style).ToString();
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["enabled"] = component.Enabled;
                        allProps["printOn"] = component.PrintOn.ToString();
                        allProps["printable"] = component.Printable;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["isPrimitiveComponent"] = true;
                        break;
                    }
                #endregion

                #region StiVerticalLinePrimitive
                case "StiVerticalLinePrimitive":
                    {
                        allProps["color"] = GetStringFromColor(((StiVerticalLinePrimitive)component).Color);
                        allProps["size"] = DoubleToStr(((StiVerticalLinePrimitive)component).Size);
                        allProps["style"] = ((int)((StiVerticalLinePrimitive)component).Style).ToString();
                        allProps["enabled"] = component.Enabled;
                        allProps["printOn"] = component.PrintOn.ToString();
                        allProps["printable"] = component.Printable;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["isPrimitiveComponent"] = true;
                        break;
                    }
                #endregion

                #region StiRectanglePrimitive
                case "StiRectanglePrimitive":
                    {
                        allProps["color"] = GetStringFromColor(((StiRectanglePrimitive)component).Color);
                        allProps["size"] = DoubleToStr(((StiRectanglePrimitive)component).Size);
                        allProps["style"] = ((int)((StiRectanglePrimitive)component).Style).ToString();
                        allProps["leftSide"] = ((StiRectanglePrimitive)component).LeftSide;
                        allProps["rightSide"] = ((StiRectanglePrimitive)component).RightSide;
                        allProps["topSide"] = ((StiRectanglePrimitive)component).TopSide;
                        allProps["bottomSide"] = ((StiRectanglePrimitive)component).BottomSide;
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["enabled"] = component.Enabled;
                        allProps["printOn"] = component.PrintOn.ToString();
                        allProps["printable"] = component.Printable;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["isPrimitiveComponent"] = true;
                        break;
                    }
                #endregion

                #region StiRoundedRectanglePrimitive
                case "StiRoundedRectanglePrimitive":
                    {
                        allProps["color"] = GetStringFromColor(((StiRoundedRectanglePrimitive)component).Color);
                        allProps["size"] = DoubleToStr(((StiRoundedRectanglePrimitive)component).Size);
                        allProps["style"] = ((int)((StiRoundedRectanglePrimitive)component).Style).ToString();
                        allProps["round"] = DoubleToStr(((StiRoundedRectanglePrimitive)component).Round);
                        allProps["anchor"] = component.Anchor.ToString();
                        allProps["enabled"] = ((StiRoundedRectanglePrimitive)component).Enabled;
                        allProps["printOn"] = ((StiRoundedRectanglePrimitive)component).PrintOn.ToString();
                        allProps["printable"] = ((StiRoundedRectanglePrimitive)component).Printable;
                        allProps["shiftMode"] = component.ShiftMode.ToString();
                        allProps["isPrimitiveComponent"] = true;
                        break;
                    }
                #endregion 


                #region StiPageHeaderBand
                case "StiPageHeaderBand":
                    {
                        allProps["brush"] = BrushToStr(((StiPageHeaderBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiPageHeaderBand)component).Border);
                        allProps["printOn"] = ((StiPageHeaderBand)component).PrintOn.ToString();
                        allProps["resetPageNumber"] = ((StiPageHeaderBand)component).ResetPageNumber;
                        allProps["enabled"] = ((StiPageHeaderBand)component).Enabled;
                        allProps["canGrow"] = ((StiPageHeaderBand)component).CanGrow;
                        allProps["canShrink"] = ((StiPageHeaderBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiPageHeaderBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiPageHeaderBand)component).MaxHeight);
                        allProps["printOnEvenOddPages"] = ((StiPageHeaderBand)component).PrintOnEvenOddPages;
                        break;
                    }
                #endregion

                #region StiPageFooterBand
                case "StiPageFooterBand":
                    {
                        allProps["brush"] = BrushToStr(((StiPageFooterBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiPageFooterBand)component).Border);
                        allProps["printOn"] = ((StiPageFooterBand)component).PrintOn.ToString();
                        allProps["resetPageNumber"] = ((StiPageFooterBand)component).ResetPageNumber;
                        allProps["enabled"] = ((StiPageFooterBand)component).Enabled;
                        allProps["canGrow"] = ((StiPageFooterBand)component).CanGrow;
                        allProps["canShrink"] = ((StiPageFooterBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiPageFooterBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiPageFooterBand)component).MaxHeight);
                        allProps["printOnEvenOddPages"] = ((StiPageFooterBand)component).PrintOnEvenOddPages;
                        break;
                    }
                #endregion

                #region StiReportTitleBand
                case "StiReportTitleBand":
                    {
                        allProps["brush"] = BrushToStr(((StiReportTitleBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiReportTitleBand)component).Border);
                        allProps["printIfEmpty"] = ((StiReportTitleBand)component).PrintIfEmpty;
                        allProps["resetPageNumber"] = ((StiReportTitleBand)component).ResetPageNumber;
                        allProps["enabled"] = ((StiReportTitleBand)component).Enabled;
                        allProps["canGrow"] = ((StiReportTitleBand)component).CanGrow;
                        allProps["canShrink"] = ((StiReportTitleBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiReportTitleBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiReportTitleBand)component).MaxHeight);
                        break;
                    }
                #endregion

                #region StiGroupHeaderBand
                case "StiGroupHeaderBand":
                    {
                        allProps["brush"] = BrushToStr(((StiGroupHeaderBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiGroupHeaderBand)component).Border);
                        allProps["canBreak"] = ((StiGroupHeaderBand)component).CanBreak;
                        allProps["printOn"] = ((StiGroupHeaderBand)component).PrintOn.ToString();
                        allProps["newPageBefore"] = ((StiGroupHeaderBand)component).NewPageBefore;
                        allProps["newPageAfter"] = ((StiGroupHeaderBand)component).NewPageAfter;
                        allProps["newColumnBefore"] = ((StiGroupHeaderBand)component).NewColumnBefore;
                        allProps["newColumnAfter"] = ((StiGroupHeaderBand)component).NewColumnAfter;
                        allProps["breakIfLessThan"] = DoubleToStr(((StiGroupHeaderBand)component).BreakIfLessThan);
                        allProps["skipFirst"] = ((StiGroupHeaderBand)component).SkipFirst;
                        allProps["condition"] = (((StiGroupHeaderBand)component).Condition != null) ? StiEncodingHelper.Encode(((StiGroupHeaderBand)component).Condition.Value) : string.Empty;
                        allProps["sortDirection"] = ((int)((StiGroupHeaderBand)component).SortDirection).ToString();
                        allProps["summarySortDirection"] = ((int)((StiGroupHeaderBand)component).SummarySortDirection).ToString();
                        allProps["summaryExpression"] = StiEncodingHelper.Encode(((StiGroupHeaderBand)component).SummaryExpression.ToString());
                        allProps["summaryType"] = ((int)((StiGroupHeaderBand)component).SummaryType).ToString();
                        allProps["printOnAllPages"] = ((StiGroupHeaderBand)component).PrintOnAllPages;
                        allProps["resetPageNumber"] = ((StiGroupHeaderBand)component).ResetPageNumber;
                        allProps["printAtBottom"] = ((StiGroupHeaderBand)component).PrintAtBottom;
                        allProps["keepGroupTogether"] = ((StiGroupHeaderBand)component).KeepGroupTogether;
                        allProps["keepGroupHeaderTogether"] = ((StiGroupHeaderBand)component).KeepGroupHeaderTogether;
                        allProps["enabled"] = ((StiGroupHeaderBand)component).Enabled;
                        allProps["canGrow"] = ((StiGroupHeaderBand)component).CanGrow;
                        allProps["canShrink"] = ((StiGroupHeaderBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiGroupHeaderBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiGroupHeaderBand)component).MaxHeight);
                        break;
                    }
                #endregion

                #region StiGroupFooterBand
                case "StiGroupFooterBand":
                    {
                        allProps["brush"] = BrushToStr(((StiGroupFooterBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiGroupFooterBand)component).Border);
                        allProps["canBreak"] = ((StiGroupFooterBand)component).CanBreak;
                        allProps["printOn"] = ((StiGroupFooterBand)component).PrintOn.ToString();
                        allProps["newPageBefore"] = ((StiGroupFooterBand)component).NewPageBefore;
                        allProps["newPageAfter"] = ((StiGroupFooterBand)component).NewPageAfter;
                        allProps["newColumnBefore"] = ((StiGroupFooterBand)component).NewColumnBefore;
                        allProps["newColumnAfter"] = ((StiGroupFooterBand)component).NewColumnAfter;
                        allProps["breakIfLessThan"] = DoubleToStr(((StiGroupFooterBand)component).BreakIfLessThan);
                        allProps["skipFirst"] = ((StiGroupFooterBand)component).SkipFirst;
                        allProps["resetPageNumber"] = ((StiGroupFooterBand)component).ResetPageNumber;
                        allProps["printAtBottom"] = ((StiGroupFooterBand)component).PrintAtBottom;
                        allProps["enabled"] = ((StiGroupFooterBand)component).Enabled;
                        allProps["canGrow"] = ((StiGroupFooterBand)component).CanGrow;
                        allProps["canShrink"] = ((StiGroupFooterBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiGroupFooterBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiGroupFooterBand)component).MaxHeight);
                        allProps["keepGroupFooterTogether"] = ((StiGroupFooterBand)component).KeepGroupFooterTogether;
                        break;
                    }
                #endregion

                #region StiHeaderBand
                case "StiHeaderBand":
                    {
                        allProps["brush"] = BrushToStr(((StiHeaderBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiHeaderBand)component).Border);
                        allProps["canBreak"] = ((StiHeaderBand)component).CanBreak;
                        allProps["printOn"] = ((StiHeaderBand)component).PrintOn.ToString();
                        allProps["newPageBefore"] = ((StiHeaderBand)component).NewPageBefore;
                        allProps["newPageAfter"] = ((StiHeaderBand)component).NewPageAfter;
                        allProps["newColumnBefore"] = ((StiHeaderBand)component).NewColumnBefore;
                        allProps["newColumnAfter"] = ((StiHeaderBand)component).NewColumnAfter;
                        allProps["breakIfLessThan"] = DoubleToStr(((StiHeaderBand)component).BreakIfLessThan);
                        allProps["skipFirst"] = ((StiHeaderBand)component).SkipFirst;
                        allProps["printIfEmpty"] = ((StiHeaderBand)component).PrintIfEmpty;
                        allProps["printOnAllPages"] = ((StiHeaderBand)component).PrintOnAllPages;
                        allProps["resetPageNumber"] = ((StiHeaderBand)component).ResetPageNumber;
                        allProps["printAtBottom"] = ((StiHeaderBand)component).PrintAtBottom;
                        allProps["enabled"] = ((StiHeaderBand)component).Enabled;
                        allProps["canGrow"] = ((StiHeaderBand)component).CanGrow;
                        allProps["canShrink"] = ((StiHeaderBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiHeaderBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiHeaderBand)component).MaxHeight);
                        allProps["keepHeaderTogether"] = ((StiHeaderBand)component).KeepHeaderTogether;
                        allProps["printOnEvenOddPages"] = ((StiHeaderBand)component).PrintOnEvenOddPages;
                        break;
                    }
                #endregion

                #region StiFooterBand
                case "StiFooterBand":
                    {
                        allProps["brush"] = BrushToStr(((StiFooterBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiFooterBand)component).Border);
                        allProps["canBreak"] = ((StiFooterBand)component).CanBreak;
                        allProps["printOn"] = ((StiFooterBand)component).PrintOn.ToString();
                        allProps["newPageBefore"] = ((StiFooterBand)component).NewPageBefore;
                        allProps["newPageAfter"] = ((StiFooterBand)component).NewPageAfter;
                        allProps["newColumnBefore"] = ((StiFooterBand)component).NewColumnBefore;
                        allProps["newColumnAfter"] = ((StiFooterBand)component).NewColumnAfter;
                        allProps["breakIfLessThan"] = DoubleToStr(((StiFooterBand)component).BreakIfLessThan);
                        allProps["skipFirst"] = ((StiFooterBand)component).SkipFirst;
                        allProps["printIfEmpty"] = ((StiFooterBand)component).PrintIfEmpty;
                        allProps["printOnAllPages"] = ((StiFooterBand)component).PrintOnAllPages;
                        allProps["resetPageNumber"] = ((StiFooterBand)component).ResetPageNumber;
                        allProps["printAtBottom"] = ((StiFooterBand)component).PrintAtBottom;
                        allProps["enabled"] = ((StiFooterBand)component).Enabled;
                        allProps["canGrow"] = ((StiFooterBand)component).CanGrow;
                        allProps["canShrink"] = ((StiFooterBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiFooterBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiFooterBand)component).MaxHeight);
                        allProps["keepFooterTogether"] = ((StiFooterBand)component).KeepFooterTogether;
                        allProps["printOnEvenOddPages"] = ((StiFooterBand)component).PrintOnEvenOddPages;
                        break;
                    }
                #endregion

                #region StiColumnHeaderBand
                case "StiColumnHeaderBand":
                    {
                        allProps["brush"] = BrushToStr(((StiColumnHeaderBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiColumnHeaderBand)component).Border);
                        allProps["canBreak"] = ((StiColumnHeaderBand)component).CanBreak;
                        allProps["printOn"] = ((StiColumnHeaderBand)component).PrintOn.ToString();
                        allProps["newPageBefore"] = ((StiColumnHeaderBand)component).NewPageBefore;
                        allProps["newPageAfter"] = ((StiColumnHeaderBand)component).NewPageAfter;
                        allProps["newColumnBefore"] = ((StiColumnHeaderBand)component).NewColumnBefore;
                        allProps["newColumnAfter"] = ((StiColumnHeaderBand)component).NewColumnAfter;
                        allProps["breakIfLessThan"] = DoubleToStr(((StiColumnHeaderBand)component).BreakIfLessThan);
                        allProps["skipFirst"] = ((StiColumnHeaderBand)component).SkipFirst;
                        allProps["printIfEmpty"] = ((StiColumnHeaderBand)component).PrintIfEmpty;
                        allProps["printOnAllPages"] = ((StiColumnHeaderBand)component).PrintOnAllPages;
                        allProps["resetPageNumber"] = ((StiColumnHeaderBand)component).ResetPageNumber;
                        allProps["printAtBottom"] = ((StiColumnHeaderBand)component).PrintAtBottom;
                        allProps["enabled"] = ((StiColumnHeaderBand)component).Enabled;
                        allProps["canGrow"] = ((StiColumnHeaderBand)component).CanGrow;
                        allProps["canShrink"] = ((StiColumnHeaderBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiColumnHeaderBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiColumnHeaderBand)component).MaxHeight);
                        allProps["keepHeaderTogether"] = ((StiColumnHeaderBand)component).KeepHeaderTogether;
                        break;
                    }
                #endregion

                #region StiColumnFooterBand
                case "StiColumnFooterBand":
                    {
                        allProps["brush"] = BrushToStr(((StiColumnFooterBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiColumnFooterBand)component).Border);
                        allProps["canBreak"] = ((StiColumnFooterBand)component).CanBreak;
                        allProps["printOn"] = ((StiColumnFooterBand)component).PrintOn.ToString();
                        allProps["newPageBefore"] = ((StiColumnFooterBand)component).NewPageBefore;
                        allProps["newPageAfter"] = ((StiColumnFooterBand)component).NewPageAfter;
                        allProps["newColumnBefore"] = ((StiColumnFooterBand)component).NewColumnBefore;
                        allProps["newColumnAfter"] = ((StiColumnFooterBand)component).NewColumnAfter;
                        allProps["breakIfLessThan"] = DoubleToStr(((StiColumnFooterBand)component).BreakIfLessThan);
                        allProps["skipFirst"] = ((StiColumnFooterBand)component).SkipFirst;
                        allProps["printIfEmpty"] = ((StiColumnFooterBand)component).PrintIfEmpty;
                        allProps["printOnAllPages"] = ((StiColumnFooterBand)component).PrintOnAllPages;
                        allProps["resetPageNumber"] = ((StiColumnFooterBand)component).ResetPageNumber;
                        allProps["printAtBottom"] = ((StiColumnFooterBand)component).PrintAtBottom;
                        allProps["enabled"] = ((StiColumnFooterBand)component).Enabled;
                        allProps["canGrow"] = ((StiColumnFooterBand)component).CanGrow;
                        allProps["canShrink"] = ((StiColumnFooterBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiColumnFooterBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiColumnFooterBand)component).MaxHeight);
                        allProps["keepFooterTogether"] = ((StiColumnFooterBand)component).KeepFooterTogether;
                        break;
                    }
                #endregion

                #region StiDataBand
                case "StiDataBand":
                    {
                        allProps["brush"] = BrushToStr(((StiDataBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiDataBand)component).Border);
                        allProps["canBreak"] = ((StiDataBand)component).CanBreak;
                        allProps["printOn"] = ((StiDataBand)component).PrintOn.ToString();
                        allProps["newPageBefore"] = ((StiDataBand)component).NewPageBefore;
                        allProps["newPageAfter"] = ((StiDataBand)component).NewPageAfter;
                        allProps["newColumnBefore"] = ((StiDataBand)component).NewColumnBefore;
                        allProps["newColumnAfter"] = ((StiDataBand)component).NewColumnAfter;
                        allProps["breakIfLessThan"] = DoubleToStr(((StiDataBand)component).BreakIfLessThan);
                        allProps["skipFirst"] = ((StiDataBand)component).SkipFirst;
                        allProps["columns"] = ((StiDataBand)component).Columns.ToString();
                        allProps["columnWidth"] = DoubleToStr(((StiDataBand)component).ColumnWidth);
                        allProps["columnGaps"] = DoubleToStr(((StiDataBand)component).ColumnGaps);
                        allProps["rightToLeft"] = ((StiDataBand)component).RightToLeft;
                        allProps["columnDirection"] = ((StiDataBand)component).ColumnDirection.ToString();
                        allProps["minRowsInColumn"] = ((StiDataBand)component).MinRowsInColumn.ToString();
                        allProps["sortData"] = GetSortDataProperty(component);
                        allProps["filterData"] = StiEncodingHelper.Encode(JSON.Encode(GetFiltersObject(((StiDataBand)component).Filters)));
                        allProps["filterMode"] = ((StiDataBand)component).FilterMode.ToString();
                        allProps["filterEngine"] = ((StiDataBand)component).FilterEngine.ToString();
                        allProps["filterOn"] = ((StiDataBand)component).FilterOn;
                        allProps["dataSource"] = (((StiDataBand)component).DataSource != null) ? ((StiDataBand)component).DataSource.Name : "[Not Assigned]";
                        allProps["dataRelation"] = (((StiDataBand)component).DataRelation != null) ? ((StiDataBand)component).DataRelation.NameInSource : "[Not Assigned]";
                        allProps["masterComponent"] = (((StiDataBand)component).MasterComponent != null) ? ((StiDataBand)component).MasterComponent.Name : "[Not Assigned]";
                        allProps["countData"] = ((StiDataBand)component).CountData.ToString();
                        allProps["oddStyle"] = !string.IsNullOrEmpty(((StiDataBand)component).OddStyle) ? ((StiDataBand)component).OddStyle : "[None]";
                        allProps["evenStyle"] = !string.IsNullOrEmpty(((StiDataBand)component).EvenStyle) ? ((StiDataBand)component).EvenStyle : "[None]";
                        allProps["businessObject"] = ((StiDataBand)component).BusinessObject != null ? ((StiDataBand)component).BusinessObject.GetFullName() : "[Not Assigned]";
                        allProps["calcInvisible"] = ((StiDataBand)component).CalcInvisible;
                        allProps["printOnAllPages"] = ((StiDataBand)component).PrintOnAllPages;
                        allProps["resetPageNumber"] = ((StiDataBand)component).ResetPageNumber;
                        allProps["printAtBottom"] = ((StiDataBand)component).PrintAtBottom;
                        allProps["printIfDetailEmpty"] = ((StiDataBand)component).PrintIfDetailEmpty;
                        allProps["keepDetails"] = ((StiDataBand)component).KeepDetails;
                        allProps["enabled"] = ((StiDataBand)component).Enabled;
                        allProps["canGrow"] = ((StiDataBand)component).CanGrow;
                        allProps["canShrink"] = ((StiDataBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiDataBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiDataBand)component).MaxHeight);
                        allProps["limitRows"] = StiEncodingHelper.Encode(((StiDataBand)component).LimitRows);
                        allProps["multipleInitialization"] = ((StiDataBand)component).MultipleInitialization;
                        break;
                    }
                #endregion

                #region StiTable
                case "StiTable":
                    {
                        allProps["brush"] = BrushToStr(((StiTable)component).Brush);
                        allProps["border"] = BorderToStr(((StiTable)component).Border);
                        allProps["canBreak"] = ((StiTable)component).CanBreak;
                        allProps["printOn"] = ((StiTable)component).PrintOn.ToString();
                        allProps["newPageBefore"] = ((StiTable)component).NewPageBefore;
                        allProps["newPageAfter"] = ((StiTable)component).NewPageAfter;
                        allProps["newColumnBefore"] = ((StiTable)component).NewColumnBefore;
                        allProps["newColumnAfter"] = ((StiTable)component).NewColumnAfter;
                        allProps["breakIfLessThan"] = DoubleToStr(((StiTable)component).BreakIfLessThan);
                        allProps["skipFirst"] = ((StiTable)component).SkipFirst;
                        allProps["sortData"] = GetSortDataProperty(component);
                        allProps["filterData"] = StiEncodingHelper.Encode(JSON.Encode(GetFiltersObject(((StiTable)component).Filters)));
                        allProps["filterEngine"] = ((StiTable)component).FilterEngine.ToString();
                        allProps["filterMode"] = ((StiTable)component).FilterMode.ToString();
                        allProps["filterOn"] = ((StiTable)component).FilterOn;
                        allProps["dataSource"] = (((StiTable)component).DataSource != null) ? ((StiTable)component).DataSource.Name : "[Not Assigned]";
                        allProps["dataRelation"] = (((StiTable)component).DataRelation != null) ? ((StiTable)component).DataRelation.NameInSource : "[Not Assigned]";
                        allProps["masterComponent"] = (((StiTable)component).MasterComponent != null) ? ((StiTable)component).MasterComponent.Name : "[Not Assigned]";
                        allProps["countData"] = ((StiTable)component).CountData.ToString();
                        allProps["oddStyle"] = !string.IsNullOrEmpty(((StiTable)component).OddStyle) ? ((StiTable)component).OddStyle : "[None]";
                        allProps["evenStyle"] = !string.IsNullOrEmpty(((StiTable)component).EvenStyle) ? ((StiTable)component).EvenStyle : "[None]";
                        allProps["businessObject"] = ((StiTable)component).BusinessObject != null ? ((StiTable)component).BusinessObject.GetFullName() : "[Not Assigned]";
                        allProps["calcInvisible"] = ((StiTable)component).CalcInvisible;
                        allProps["printOnAllPages"] = ((StiTable)component).PrintOnAllPages;
                        allProps["resetPageNumber"] = ((StiTable)component).ResetPageNumber;
                        allProps["printAtBottom"] = ((StiTable)component).PrintAtBottom;
                        allProps["printIfDetailEmpty"] = ((StiTable)component).PrintIfDetailEmpty;
                        allProps["enabled"] = ((StiTable)component).Enabled;
                        allProps["canGrow"] = ((StiTable)component).CanGrow;
                        allProps["canShrink"] = ((StiTable)component).CanShrink;
                        allProps["keepDetails"] = ((StiTable)component).KeepDetails;
                        allProps["tableAutoWidth"] = ((StiTable)component).AutoWidth.ToString();
                        allProps["autoWidthType"] = ((StiTable)component).AutoWidthType.ToString();
                        allProps["columnCount"] = ((StiTable)component).ColumnCount.ToString();
                        allProps["rowCount"] = ((StiTable)component).RowCount.ToString();
                        allProps["headerRowsCount"] = ((StiTable)component).HeaderRowsCount.ToString();
                        allProps["footerRowsCount"] = ((StiTable)component).FooterRowsCount.ToString();
                        allProps["tableRightToLeft"] = ((StiTable)component).RightToLeft;
                        allProps["dockableTable"] = ((StiTable)component).DockableTable;
                        allProps["headerPrintOn"] = ((StiTable)component).HeaderPrintOn.ToString();
                        allProps["headerCanGrow"] = ((StiTable)component).HeaderCanGrow;
                        allProps["headerCanShrink"] = ((StiTable)component).HeaderCanShrink;
                        allProps["headerCanBreak"] = ((StiTable)component).HeaderCanBreak;
                        allProps["headerPrintAtBottom"] = ((StiTable)component).HeaderPrintAtBottom;
                        allProps["headerPrintIfEmpty"] = ((StiTable)component).HeaderPrintIfEmpty;
                        allProps["headerPrintOnAllPages"] = ((StiTable)component).HeaderPrintOnAllPages;
                        allProps["headerPrintOnEvenOddPages"] = ((StiTable)component).HeaderPrintOnEvenOddPages;
                        allProps["footerPrintOn"] = ((StiTable)component).FooterPrintOn.ToString();
                        allProps["footerCanGrow"] = ((StiTable)component).FooterCanGrow;
                        allProps["footerCanShrink"] = ((StiTable)component).FooterCanShrink;
                        allProps["footerCanBreak"] = ((StiTable)component).FooterCanBreak;
                        allProps["footerPrintAtBottom"] = ((StiTable)component).FooterPrintAtBottom;
                        allProps["footerPrintIfEmpty"] = ((StiTable)component).FooterPrintIfEmpty;
                        allProps["footerPrintOnAllPages"] = ((StiTable)component).FooterPrintOnAllPages;
                        allProps["footerPrintOnEvenOddPages"] = ((StiTable)component).FooterPrintOnEvenOddPages;
                        allProps["styleId"] = ((StiTable)component).TableStyleFX != null ? ((StiTable)component).TableStyleFX.StyleId.ToString() : "";
                        break;
                    }
                #endregion

                #region StiHierarchicalBand
                case "StiHierarchicalBand":
                    {
                        allProps["brush"] = BrushToStr(((StiHierarchicalBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiHierarchicalBand)component).Border);
                        allProps["canBreak"] = ((StiHierarchicalBand)component).CanBreak;
                        allProps["printOn"] = ((StiHierarchicalBand)component).PrintOn.ToString();
                        allProps["newPageBefore"] = ((StiHierarchicalBand)component).NewPageBefore;
                        allProps["newPageAfter"] = ((StiHierarchicalBand)component).NewPageAfter;
                        allProps["newColumnBefore"] = ((StiHierarchicalBand)component).NewColumnBefore;
                        allProps["newColumnAfter"] = ((StiHierarchicalBand)component).NewColumnAfter;
                        allProps["breakIfLessThan"] = DoubleToStr(((StiHierarchicalBand)component).BreakIfLessThan);
                        allProps["skipFirst"] = ((StiHierarchicalBand)component).SkipFirst;
                        allProps["columns"] = ((StiHierarchicalBand)component).Columns.ToString();
                        allProps["columnWidth"] = DoubleToStr(((StiHierarchicalBand)component).ColumnWidth);
                        allProps["columnGaps"] = DoubleToStr(((StiHierarchicalBand)component).ColumnGaps);
                        allProps["rightToLeft"] = ((StiHierarchicalBand)component).RightToLeft;
                        allProps["columnDirection"] = ((StiHierarchicalBand)component).ColumnDirection.ToString();
                        allProps["minRowsInColumn"] = ((StiHierarchicalBand)component).MinRowsInColumn.ToString();
                        allProps["sortData"] = GetSortDataProperty(component);
                        allProps["filterData"] = StiEncodingHelper.Encode(JSON.Encode(GetFiltersObject(((StiHierarchicalBand)component).Filters)));
                        allProps["filterEngine"] = ((StiHierarchicalBand)component).FilterEngine.ToString();
                        allProps["filterMode"] = ((StiHierarchicalBand)component).FilterMode.ToString();
                        allProps["filterOn"] = ((StiHierarchicalBand)component).FilterOn;
                        allProps["dataSource"] = (((StiHierarchicalBand)component).DataSource != null) ? ((StiHierarchicalBand)component).DataSource.Name : "[Not Assigned]";
                        allProps["dataRelation"] = (((StiHierarchicalBand)component).DataRelation != null) ? ((StiHierarchicalBand)component).DataRelation.NameInSource : "[Not Assigned]";
                        allProps["masterComponent"] = (((StiHierarchicalBand)component).MasterComponent != null) ? ((StiHierarchicalBand)component).MasterComponent.Name : "[Not Assigned]";
                        allProps["countData"] = ((StiHierarchicalBand)component).CountData.ToString();
                        allProps["keyDataColumn"] = ((StiHierarchicalBand)component).KeyDataColumn;
                        allProps["masterKeyDataColumn"] = ((StiHierarchicalBand)component).MasterKeyDataColumn;
                        allProps["parentValue"] = StiEncodingHelper.Encode(((StiHierarchicalBand)component).ParentValue);
                        allProps["indent"] = DoubleToStr(((StiHierarchicalBand)component).Indent);
                        allProps["headers"] = StiEncodingHelper.Encode(((StiHierarchicalBand)component).Headers);
                        allProps["footers"] = StiEncodingHelper.Encode(((StiHierarchicalBand)component).Footers);
                        allProps["oddStyle"] = !string.IsNullOrEmpty(((StiHierarchicalBand)component).OddStyle) ? ((StiHierarchicalBand)component).OddStyle : "[None]";
                        allProps["evenStyle"] = !string.IsNullOrEmpty(((StiHierarchicalBand)component).EvenStyle) ? ((StiHierarchicalBand)component).EvenStyle : "[None]";
                        allProps["businessObject"] = ((StiHierarchicalBand)component).BusinessObject != null ? ((StiHierarchicalBand)component).BusinessObject.GetFullName() : "[Not Assigned]";
                        allProps["calcInvisible"] = ((StiHierarchicalBand)component).CalcInvisible;
                        allProps["printOnAllPages"] = ((StiHierarchicalBand)component).PrintOnAllPages;
                        allProps["resetPageNumber"] = ((StiHierarchicalBand)component).ResetPageNumber;
                        allProps["printAtBottom"] = ((StiHierarchicalBand)component).PrintAtBottom;
                        allProps["printIfDetailEmpty"] = ((StiHierarchicalBand)component).PrintIfDetailEmpty;
                        allProps["keepDetails"] = ((StiHierarchicalBand)component).KeepDetails;
                        allProps["enabled"] = ((StiHierarchicalBand)component).Enabled;
                        allProps["canGrow"] = ((StiHierarchicalBand)component).CanGrow;
                        allProps["canShrink"] = ((StiHierarchicalBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiHierarchicalBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiHierarchicalBand)component).MaxHeight);
                        allProps["limitRows"] = StiEncodingHelper.Encode(((StiHierarchicalBand)component).LimitRows);
                        break;
                    }
                #endregion

                #region StiChildBand
                case "StiChildBand":
                    {
                        allProps["brush"] = BrushToStr(((StiChildBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiChildBand)component).Border);
                        allProps["canBreak"] = ((StiChildBand)component).CanBreak;
                        allProps["printOn"] = ((StiChildBand)component).PrintOn.ToString();
                        allProps["newPageBefore"] = ((StiChildBand)component).NewPageBefore;
                        allProps["newPageAfter"] = ((StiChildBand)component).NewPageAfter;
                        allProps["newColumnBefore"] = ((StiChildBand)component).NewColumnBefore;
                        allProps["newColumnAfter"] = ((StiChildBand)component).NewColumnAfter;
                        allProps["breakIfLessThan"] = DoubleToStr(((StiChildBand)component).BreakIfLessThan);
                        allProps["skipFirst"] = ((StiChildBand)component).SkipFirst;
                        allProps["resetPageNumber"] = ((StiChildBand)component).ResetPageNumber;
                        allProps["printAtBottom"] = ((StiChildBand)component).PrintAtBottom;
                        allProps["enabled"] = ((StiChildBand)component).Enabled;
                        allProps["canGrow"] = ((StiChildBand)component).CanGrow;
                        allProps["canShrink"] = ((StiChildBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiChildBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiChildBand)component).MaxHeight);
                        break;
                    }
                #endregion

                #region StiEmptyBand
                case "StiEmptyBand":
                    {
                        allProps["brush"] = BrushToStr(((StiEmptyBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiEmptyBand)component).Border);
                        allProps["printOn"] = ((StiEmptyBand)component).PrintOn.ToString();
                        allProps["oddStyle"] = !string.IsNullOrEmpty(((StiEmptyBand)component).OddStyle) ? ((StiEmptyBand)component).OddStyle : "[None]";
                        allProps["evenStyle"] = !string.IsNullOrEmpty(((StiEmptyBand)component).EvenStyle) ? ((StiEmptyBand)component).EvenStyle : "[None]";
                        allProps["resetPageNumber"] = ((StiEmptyBand)component).ResetPageNumber;
                        allProps["enabled"] = ((StiEmptyBand)component).Enabled;
                        allProps["canGrow"] = ((StiEmptyBand)component).CanGrow;
                        allProps["canShrink"] = ((StiEmptyBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiEmptyBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiEmptyBand)component).MaxHeight);
                        allProps["sizeMode"] = ((StiEmptyBand)component).SizeMode;
                        break;
                    }
                #endregion

                #region StiReportSummaryBand
                case "StiReportSummaryBand":
                    {
                        allProps["brush"] = BrushToStr(((StiReportSummaryBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiReportSummaryBand)component).Border);
                        allProps["canBreak"] = ((StiReportSummaryBand)component).CanBreak;
                        allProps["gGrowToHeight"] = ((StiReportSummaryBand)component).GrowToHeight;
                        allProps["newPageBefore"] = ((StiReportSummaryBand)component).NewPageBefore;
                        allProps["newPageAfter"] = ((StiReportSummaryBand)component).NewPageAfter;
                        allProps["newColumnBefore"] = ((StiReportSummaryBand)component).NewColumnBefore;
                        allProps["newColumnAfter"] = ((StiReportSummaryBand)component).NewColumnAfter;
                        allProps["breakIfLessThan"] = DoubleToStr(((StiReportSummaryBand)component).BreakIfLessThan);
                        allProps["skipFirst"] = ((StiReportSummaryBand)component).SkipFirst;
                        allProps["printIfEmpty"] = ((StiReportSummaryBand)component).PrintIfEmpty;
                        allProps["resetPageNumber"] = ((StiReportSummaryBand)component).ResetPageNumber;
                        allProps["printAtBottom"] = ((StiReportSummaryBand)component).PrintAtBottom;
                        allProps["enabled"] = ((StiReportSummaryBand)component).Enabled;
                        allProps["canGrow"] = ((StiReportSummaryBand)component).CanGrow;
                        allProps["canShrink"] = ((StiReportSummaryBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiReportSummaryBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiReportSummaryBand)component).MaxHeight);
                        allProps["keepReportSummaryTogether"] = ((StiReportSummaryBand)component).KeepReportSummaryTogether;
                        break;
                    }
                #endregion

                #region StiOverlayBand
                case "StiOverlayBand":
                    {
                        allProps["brush"] = BrushToStr(((StiOverlayBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiOverlayBand)component).Border);
                        allProps["printOn"] = ((StiOverlayBand)component).PrintOn.ToString();
                        allProps["resetPageNumber"] = ((StiOverlayBand)component).ResetPageNumber;
                        allProps["enabled"] = ((StiOverlayBand)component).Enabled;
                        allProps["canGrow"] = ((StiOverlayBand)component).CanGrow;
                        allProps["canShrink"] = ((StiOverlayBand)component).CanShrink;
                        allProps["minHeight"] = DoubleToStr(((StiOverlayBand)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiOverlayBand)component).MaxHeight);
                        allProps["vertAlignment"] = ((StiOverlayBand)component).VertAlignment.ToString();
                        break;
                    }
                #endregion

                #region StiTableOfContents
                case "StiTableOfContents":
                    {   
                        allProps["brush"] = BrushToStr(((StiTableOfContents)component).Brush);
                        allProps["border"] = BorderToStr(((StiTableOfContents)component).Border);
                        allProps["tableOfContentsIndent"] = ((StiTableOfContents)component).Indent.ToString();
                        allProps["tableOfContentsNewPageBefore"] = ((StiTableOfContents)component).NewPageBefore;
                        allProps["tableOfContentsNewPageAfter"] = ((StiTableOfContents)component).NewPageAfter;
                        allProps["tableOfContentsRightToLeft"] = ((StiTableOfContents)component).RightToLeft;
                        allProps["enabled"] = ((StiTableOfContents)component).Enabled;
                        allProps["minHeight"] = DoubleToStr(((StiTableOfContents)component).MinHeight);
                        allProps["maxHeight"] = DoubleToStr(((StiTableOfContents)component).MaxHeight);
                        allProps["tableOfContentsStyles"] = GetStylesProperty(component);
                        allProps["tableOfContentsMargins"] = MarginsToStr(((StiTableOfContents)component).Margins);
                        allProps["reportPointer"] = !string.IsNullOrEmpty(((StiTableOfContents)component).ReportPointer) ? StiEncodingHelper.Encode(((StiTableOfContents)component).ReportPointer) : string.Empty;
                        break;
                    }
                #endregion


                #region StiCrossTab
                case "StiCrossTab":
                    {
                        allProps["brush"] = BrushToStr(((StiCrossTab)component).Brush);
                        allProps["border"] = BorderToStr(((StiCrossTab)component).Border);
                        allProps["printOn"] = ((StiCrossTab)component).PrintOn.ToString();
                        allProps["clientLeft"] = DoubleToStr(component.Left);
                        allProps["clientTop"] = DoubleToStr(component.Top);
                        allProps["sortData"] = GetSortDataProperty(component);
                        allProps["filterData"] = StiEncodingHelper.Encode(JSON.Encode(GetFiltersObject(((StiCrossTab)component).Filters)));
                        allProps["filterMode"] = ((StiCrossTab)component).FilterMode.ToString();
                        allProps["filterOn"] = ((StiCrossTab)component).FilterOn;
                        allProps["filterEngine"] = ((StiCrossTab)component).FilterEngine.ToString();
                        allProps["dataSource"] = (((StiCrossTab)component).DataSource != null) ? ((StiCrossTab)component).DataSource.Name : "[Not Assigned]";
                        allProps["dataRelation"] = (((StiCrossTab)component).DataRelation != null) ? ((StiCrossTab)component).DataRelation.NameInSource : "[Not Assigned]";
                        allProps["crossTabEmptyValue"] = StiEncodingHelper.Encode(((StiCrossTab)component).EmptyValue);
                        allProps["crossTabHorAlign"] = ((StiCrossTab)component).HorAlignment.ToString();
                        allProps["rightToLeft"] = ((StiCrossTab)component).RightToLeft;
                        allProps["crossTabWrap"] = ((StiCrossTab)component).Wrap;
                        allProps["crossTabWrapGap"] = DoubleToStr(((StiCrossTab)component).WrapGap);
                        allProps["enabled"] = ((StiCrossTab)component).Enabled;
                        allProps["crossTabFields"] = GetCrossTabFieldsProperties(component as StiCrossTab);
                        allProps["dockStyle"] = ((StiCrossTab)component).DockStyle.ToString();
                        allProps["summaryDirection"] = ((StiCrossTab)component).SummaryDirection.ToString();
                        allProps["minSize"] = DoubleToStr(component.MinSize.Width) + ";" + DoubleToStr(component.MinSize.Height);
                        allProps["maxSize"] = DoubleToStr(component.MaxSize.Width) + ";" + DoubleToStr(component.MaxSize.Height);
                        allProps["keepCrossTabTogether"] = ((StiCrossTab)component).KeepCrossTabTogether;
                        allProps["dataSource"] = (((StiCrossTab)component).DataSource != null) ? ((StiCrossTab)component).DataSource.Name : "[Not Assigned]";
                        allProps["dataRelation"] = (((StiCrossTab)component).DataRelation != null) ? ((StiCrossTab)component).DataRelation.NameInSource : "[Not Assigned]";
                        allProps["businessObject"] = ((StiCrossTab)component).BusinessObject != null ? ((StiCrossTab)component).BusinessObject.GetFullName() : "[Not Assigned]";
                        break;
                    }
                #endregion

                #region StiCrossGroupHeaderBand
                case "StiCrossGroupHeaderBand":
                    {
                        allProps["brush"] = BrushToStr(((StiCrossGroupHeaderBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiCrossGroupHeaderBand)component).Border);
                        allProps["printOn"] = ((StiCrossGroupHeaderBand)component).PrintOn.ToString();
                        allProps["condition"] = (((StiCrossGroupHeaderBand)component).Condition != null) ? StiEncodingHelper.Encode(((StiCrossGroupHeaderBand)component).Condition.Value) : string.Empty;
                        allProps["sortDirection"] = ((int)((StiCrossGroupHeaderBand)component).SortDirection).ToString();
                        allProps["summarySortDirection"] = ((int)((StiCrossGroupHeaderBand)component).SummarySortDirection).ToString();
                        allProps["summaryExpression"] = StiEncodingHelper.Encode(((StiCrossGroupHeaderBand)component).SummaryExpression.ToString());
                        allProps["summaryType"] = ((int)((StiCrossGroupHeaderBand)component).SummaryType).ToString();
                        allProps["keepGroupTogether"] = ((StiCrossGroupHeaderBand)component).KeepGroupTogether;
                        allProps["keepGroupHeaderTogether"] = ((StiCrossGroupHeaderBand)component).KeepGroupHeaderTogether;
                        allProps["enabled"] = ((StiCrossGroupHeaderBand)component).Enabled;
                        allProps["canGrow"] = ((StiCrossGroupHeaderBand)component).CanGrow;
                        allProps["canShrink"] = ((StiCrossGroupHeaderBand)component).CanShrink;
                        allProps["canBreak"] = ((StiCrossGroupHeaderBand)component).CanBreak;
                        allProps["minWidth"] = DoubleToStr(((StiCrossGroupHeaderBand)component).MinWidth);
                        allProps["maxWidth"] = DoubleToStr(((StiCrossGroupHeaderBand)component).MaxWidth);
                        break;
                    }
                #endregion

                #region StiCrossGroupFooterBand
                case "StiCrossGroupFooterBand":
                    {
                        allProps["brush"] = BrushToStr(((StiCrossGroupFooterBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiCrossGroupFooterBand)component).Border);
                        allProps["printOn"] = ((StiCrossGroupFooterBand)component).PrintOn.ToString();
                        allProps["enabled"] = ((StiCrossGroupFooterBand)component).Enabled;
                        allProps["canGrow"] = ((StiCrossGroupFooterBand)component).CanGrow;
                        allProps["canShrink"] = ((StiCrossGroupFooterBand)component).CanShrink;
                        allProps["canBreak"] = ((StiCrossGroupFooterBand)component).CanBreak;
                        allProps["keepGroupFooterTogether"] = ((StiCrossGroupFooterBand)component).KeepGroupFooterTogether;
                        allProps["minWidth"] = DoubleToStr(((StiCrossGroupFooterBand)component).MinWidth);
                        allProps["maxWidth"] = DoubleToStr(((StiCrossGroupFooterBand)component).MaxWidth);
                        break;
                    }
                #endregion

                #region StiCrossHeaderBand
                case "StiCrossHeaderBand":
                    {
                        allProps["brush"] = BrushToStr(((StiCrossHeaderBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiCrossHeaderBand)component).Border);
                        allProps["enabled"] = ((StiCrossHeaderBand)component).Enabled;
                        allProps["canGrow"] = ((StiCrossHeaderBand)component).CanGrow;
                        allProps["canShrink"] = ((StiCrossHeaderBand)component).CanShrink;
                        allProps["canBreak"] = ((StiCrossHeaderBand)component).CanBreak;
                        allProps["keepHeaderTogether"] = ((StiCrossHeaderBand)component).KeepHeaderTogether;
                        allProps["minWidth"] = DoubleToStr(((StiCrossHeaderBand)component).MinWidth);
                        allProps["maxWidth"] = DoubleToStr(((StiCrossHeaderBand)component).MaxWidth);
                        break;
                    }
                #endregion

                #region StiCrossFooterBand
                case "StiCrossFooterBand":
                    {
                        allProps["brush"] = BrushToStr(((StiCrossFooterBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiCrossFooterBand)component).Border);
                        allProps["enabled"] = ((StiCrossFooterBand)component).Enabled;
                        allProps["canGrow"] = ((StiCrossFooterBand)component).CanGrow;
                        allProps["canShrink"] = ((StiCrossFooterBand)component).CanShrink;
                        allProps["canBreak"] = ((StiCrossFooterBand)component).CanBreak;
                        allProps["keepFooterTogether"] = ((StiCrossFooterBand)component).KeepFooterTogether;
                        allProps["minWidth"] = DoubleToStr(((StiCrossFooterBand)component).MinWidth);
                        allProps["maxWidth"] = DoubleToStr(((StiCrossFooterBand)component).MaxWidth);
                        break;
                    }
                #endregion

                #region StiCrossDataBand
                case "StiCrossDataBand":
                    {
                        allProps["brush"] = BrushToStr(((StiCrossDataBand)component).Brush);
                        allProps["border"] = BorderToStr(((StiCrossDataBand)component).Border);
                        allProps["sortData"] = GetSortDataProperty(component);
                        allProps["filterData"] = StiEncodingHelper.Encode(JSON.Encode(GetFiltersObject(((StiCrossDataBand)component).Filters)));
                        allProps["filterEngine"] = ((StiCrossDataBand)component).FilterEngine.ToString();
                        allProps["filterMode"] = ((StiCrossDataBand)component).FilterMode.ToString();
                        allProps["filterOn"] = ((StiCrossDataBand)component).FilterOn;
                        allProps["dataSource"] = (((StiCrossDataBand)component).DataSource != null) ? ((StiCrossDataBand)component).DataSource.Name : "[Not Assigned]";
                        allProps["dataRelation"] = (((StiCrossDataBand)component).DataRelation != null) ? ((StiCrossDataBand)component).DataRelation.NameInSource : "[Not Assigned]";
                        allProps["masterComponent"] = (((StiCrossDataBand)component).MasterComponent != null) ? ((StiCrossDataBand)component).MasterComponent.Name : "[Not Assigned]";
                        allProps["countData"] = ((StiCrossDataBand)component).CountData.ToString();
                        allProps["oddStyle"] = !string.IsNullOrEmpty(((StiCrossDataBand)component).OddStyle) ? ((StiCrossDataBand)component).OddStyle : "[None]";
                        allProps["evenStyle"] = !string.IsNullOrEmpty(((StiCrossDataBand)component).EvenStyle) ? ((StiCrossDataBand)component).EvenStyle : "[None]";
                        allProps["businessObject"] = ((StiCrossDataBand)component).BusinessObject != null ? ((StiCrossDataBand)component).BusinessObject.GetFullName() : "[Not Assigned]";
                        allProps["calcInvisible"] = ((StiCrossDataBand)component).CalcInvisible;
                        allProps["printIfDetailEmpty"] = ((StiCrossDataBand)component).PrintIfDetailEmpty;
                        allProps["keepDetails"] = ((StiCrossDataBand)component).KeepDetails;
                        allProps["enabled"] = ((StiCrossDataBand)component).Enabled;
                        allProps["canGrow"] = ((StiCrossDataBand)component).CanGrow;
                        allProps["canShrink"] = ((StiCrossDataBand)component).CanShrink;
                        allProps["canBreak"] = ((StiCrossDataBand)component).CanBreak;
                        allProps["minWidth"] = DoubleToStr(((StiCrossDataBand)component).MinWidth);
                        allProps["maxWidth"] = DoubleToStr(((StiCrossDataBand)component).MaxWidth);
                        allProps["multipleInitialization"] = ((StiCrossDataBand)component).MultipleInitialization;
                        break;
                    }
                #endregion

                #region StiDashboard
                case "StiDashboard":
                    {
                        var page = component as StiPage;
                        allProps["pageKey"] = page.Guid;
                        allProps["isDashboard"] = true;
                        allProps["realBackColor"] = GetStringFromColor(StiDashboardHelper.GetDashboardBackColor(page));
                        allProps["gridLinesColor"] = GetStringFromColor(StiDashboardHelper.GetDashboardGridLinesColor(page));
                        allProps["gridDotsColor"] = GetStringFromColor(StiDashboardHelper.GetDashboardGridDotsColor(page));
                        allProps["selectionBorderColor"] = GetStringFromColor(StiDashboardHelper.GetSelectionBorderColor(page));
                        allProps["selectionCornerColor"] = GetStringFromColor(StiDashboardHelper.GetSelectionCornerColor(page));
                        allProps["gridSize"] = DoubleToStr(page.GridSize);
                        allProps["orientation"] = page.Orientation.ToString();
                        allProps["unitWidth"] = DoubleToStr(page.Width);
                        allProps["unitHeight"] = DoubleToStr(page.Height);
                        allProps["unitMargins"] = GetPageMargins(page);
                        allProps["dashboardViewMode"] = ((IStiDashboard)page).IsDesktopSurfaceSelected ? StiDashboardViewMode.Desktop : StiDashboardViewMode.Mobile;
                        allProps["mobileViewModePresent"] = ((IStiDashboard)page).IsMobileSurfaceSelected || ((IStiDashboard)page).IsMobileSurfacePresent;
                        allProps["deviceWidth"] = ((IStiDashboard)page).DeviceWidth.ToString();
                        allProps["contentAlignment"] = ((IStiDashboard)page).ContentAlignment;
                        allProps["pageIcon"] = page.Icon != null ? ImageToBase64(page.Icon) : string.Empty;
                        allProps["dashboardWatermark"] = GetDashboardWatermarkProperty(page);
                        break;
                    }
                #endregion

                #region StiTableElement
                case "StiTableElement":
                    {
                        allProps["font"] = FontToStr((component as IStiFont).Font);
                        allProps["foreColor"] = GetStringFromColor((component as IStiTableElement).ForeColor);
                        allProps["headerFont"] = FontToStr((component as IStiTableElement).HeaderFont);
                        allProps["headerForeColor"] = GetStringFromColor((component as IStiTableElement).HeaderForeColor);
                        allProps["footerFont"] = FontToStr((component as IStiTableElement).FooterFont);
                        allProps["footerForeColor"] = GetStringFromColor((component as IStiTableElement).FooterForeColor);
                        allProps["sizeMode"] = (component as IStiTableElement).SizeMode;
                        allProps["dashboardInteraction"] = GetDashboardInteractionProperty((component as IStiElementInteraction)?.DashboardInteraction);
                        allProps["tableConditions"] = GetTableConditionsProperty(component as IStiTableElement);
                        allProps["valueMeters"] = GetTableMeters(component as IStiTableElement);
                        allProps["crossFiltering"] = (component as IStiTableElement).CrossFiltering;
                        allProps["frozenColumns"] = (component as IStiTableElement).FrozenColumns.ToString();
                        break;
                    }
                #endregion

                #region StiChartElement
                case "StiChartElement":
                    {
                        allProps["crossFiltering"] = (component as IStiChartElement).CrossFiltering;
                        allProps["valueFormat"] = StiTextFormatHelper.GetTextFormatItem((component as IStiChartElement).ValueFormat);
                        allProps["argumentFormat"] = StiTextFormatHelper.GetTextFormatItem((component as IStiChartElement).ArgumentFormat);
                        allProps["seriesColors"] =  GetColorsCollectionProperty((component as IStiSeriesColors)?.SeriesColors);
                        allProps["negativeSeriesColors"] = GetColorsCollectionProperty((component as IStiNegativeSeriesColors)?.NegativeSeriesColors);
                        allProps["topN"] = GetTopNProperty((component as IStiChartElement).TopN);
                        allProps["allowUserSorting"] = StiSortMenuHelper.IsAllowUserSorting(component as IStiChartElement);
                        allProps["icon"] = (component as IStiChartElement).Icon;
                        allProps["roundValues"] = (component as IStiChartElement).RoundValues;
                        allProps["dashboardInteraction"] = GetDashboardInteractionProperty((component as IStiElementInteraction)?.DashboardInteraction);
                        allProps["chartConstantLines"] = StiChartElementHelper.GetConstantLines(component as IStiChartElement);
                        allProps["chartConditions"] = GetChartConditionsProperty(component as IStiChartElement);
                        allProps["chartTrendLines"] = GetChartTrendLinesProperty(component as IStiChartElement);
                        allProps["chartTitleText"] = GetPropertyValue("Title.Text", component);
                        allProps["userViewStates"] = StiChartElementHelper.GetUserViewStates(component as IStiChartElement);
                        allProps["selectedViewStateKey"] = (component as IStiUserViewStates)?.SelectedViewStateKey;
                        allProps["isDarkStyle"] = StiDashboardStyleHelper.IsDarkStyle(component as IStiElement);
                        allProps["dataMode"] = (component as IStiChartElement).DataMode;
                        allProps["columnShape"] = (component as IStiChartElement).ColumnShape;

                        allProps["areaColorEach"] = GetPropertyValue("Area.ColorEach", component, true);
                        allProps["areaReverseHor"] = GetPropertyValue("Area.ReverseHor", component, true);
                        allProps["areaReverseVert"] = GetPropertyValue("Area.ReverseVert", component, true);
                        allProps["areaSideBySide"] = GetPropertyValue("Area.SideBySide", component, true);
                        allProps["areaGridLinesHorColor"] = GetPropertyValue("Area.GridLinesHor.Color", component, true);
                        allProps["areaGridLinesHorVisible"] = GetPropertyValue("Area.GridLinesHor.Visible", component, true);
                        allProps["areaGridLinesVertColor"] = GetPropertyValue("Area.GridLinesVert.Color", component, true);
                        allProps["areaGridLinesVertVisible"] = GetPropertyValue("Area.GridLinesVert.Visible", component, true);
                        allProps["areaInterlacingHorColor"] = GetPropertyValue("Area.InterlacingHor.Color", component, true);
                        allProps["areaInterlacingHorVisible"] = GetPropertyValue("Area.InterlacingHor.Visible", component, true);
                        allProps["areaInterlacingVertColor"] = GetPropertyValue("Area.InterlacingVert.Color", component, true);
                        allProps["areaInterlacingVertVisible"] = GetPropertyValue("Area.InterlacingVert.Visible", component, true);

                        allProps["xAxisVisible"] = GetPropertyValue("Area.XAxis.Visible", component, true);
                        allProps["xAxisLabelsAngle"] = GetPropertyValue("Area.XAxis.Labels.Angle", component, true);
                        allProps["xAxisLabelsColor"] = GetPropertyValue("Area.XAxis.Labels.Color", component, true);
                        allProps["xAxisLabelsFont"] = GetPropertyValue("Area.XAxis.Labels.Font", component, true);
                        allProps["xAxisLabelsPlacement"] = GetPropertyValue("Area.XAxis.Labels.Placement", component, true);
                        allProps["xAxisLabelsTextAfter"] = GetPropertyValue("Area.XAxis.Labels.TextAfter", component, true);
                        allProps["xAxisLabelsTextAlignment"] = GetPropertyValue("Area.XAxis.Labels.TextAlignment", component, true);
                        allProps["xAxisLabelsTextBefore"] = GetPropertyValue("Area.XAxis.Labels.TextBefore", component, true);
                        allProps["xAxisLabelsStep"] = GetPropertyValue("Area.XAxis.Labels.Step", component, true);
                        allProps["xAxisTitleAlignment"] = GetPropertyValue("Area.XAxis.Title.Alignment", component, true);
                        allProps["xAxisTitleColor"] = GetPropertyValue("Area.XAxis.Title.Color", component, true);
                        allProps["xAxisTitleDirection"] = GetPropertyValue("Area.XAxis.Title.Direction", component, true);
                        allProps["xAxisTitleFont"] = GetPropertyValue("Area.XAxis.Title.Font", component, true);
                        allProps["xAxisTitlePosition"] = GetPropertyValue("Area.XAxis.Title.Position", component, true);
                        allProps["xAxisTitleText"] = GetPropertyValue("Area.XAxis.Title.Text", component, true);
                        allProps["xAxisTitleVisible"] = GetPropertyValue("Area.XAxis.Title.Visible", component, true);
                        allProps["xAxisRangeAuto"] = GetPropertyValue("Area.XAxis.Range.Auto", component, true);
                        allProps["xAxisRangeMinimum"] = GetPropertyValue("Area.XAxis.Range.Minimum", component, true);
                        allProps["xAxisRangeMaximum"] = GetPropertyValue("Area.XAxis.Range.Maximum", component, true);
                        allProps["xAxisShowEdgeValues"] = GetPropertyValue("Area.XAxis.ShowEdgeValues", component, true);
                        allProps["xAxisStartFromZero"] = GetPropertyValue("Area.XAxis.StartFromZero", component, true);

                        allProps["xTopAxisVisible"] = GetPropertyValue("Area.XTopAxis.Visible", component, true);
                        allProps["xTopAxisLabelsAngle"] = GetPropertyValue("Area.XTopAxis.Labels.Angle", component, true);
                        allProps["xTopAxisLabelsColor"] = GetPropertyValue("Area.XTopAxis.Labels.Color", component, true);
                        allProps["xTopAxisLabelsFont"] = GetPropertyValue("Area.XTopAxis.Labels.Font", component, true);
                        allProps["xTopAxisLabelsPlacement"] = GetPropertyValue("Area.XTopAxis.Labels.Placement", component, true);
                        allProps["xTopAxisLabelsTextAfter"] = GetPropertyValue("Area.XTopAxis.Labels.TextAfter", component, true);
                        allProps["xTopAxisLabelsTextAlignment"] = GetPropertyValue("Area.XTopAxis.Labels.TextAlignment", component, true);
                        allProps["xTopAxisLabelsTextBefore"] = GetPropertyValue("Area.XTopAxis.Labels.TextBefore", component, true);
                        allProps["xTopAxisLabelsStep"] = GetPropertyValue("Area.XTopAxis.Labels.Step", component, true);
                        allProps["xTopAxisTitleAlignment"] = GetPropertyValue("Area.XTopAxis.Title.Alignment", component, true);
                        allProps["xTopAxisTitleColor"] = GetPropertyValue("Area.XTopAxis.Title.Color", component, true);
                        allProps["xTopAxisTitleDirection"] = GetPropertyValue("Area.XTopAxis.Title.Direction", component, true);
                        allProps["xTopAxisTitleFont"] = GetPropertyValue("Area.XTopAxis.Title.Font", component, true);
                        allProps["xTopAxisTitlePosition"] = GetPropertyValue("Area.XTopAxis.Title.Position", component, true);
                        allProps["xTopAxisTitleText"] = GetPropertyValue("Area.XTopAxis.Title.Text", component, true);
                        allProps["xTopAxisTitleVisible"] = GetPropertyValue("Area.XTopAxis.Title.Visible", component, true);
                        allProps["xTopAxisShowEdgeValues"] = GetPropertyValue("Area.XTopAxis.ShowEdgeValues", component, true);

                        allProps["yAxisVisible"] = GetPropertyValue("Area.YAxis.Visible", component, true);
                        allProps["yAxisLabelsAngle"] = GetPropertyValue("Area.YAxis.Labels.Angle", component, true);
                        allProps["yAxisLabelsColor"] = GetPropertyValue("Area.YAxis.Labels.Color", component, true);
                        allProps["yAxisLabelsFont"] = GetPropertyValue("Area.YAxis.Labels.Font", component, true);
                        allProps["yAxisLabelsPlacement"] = GetPropertyValue("Area.YAxis.Labels.Placement", component, true);
                        allProps["yAxisLabelsTextAfter"] = GetPropertyValue("Area.YAxis.Labels.TextAfter", component, true);
                        allProps["yAxisLabelsTextAlignment"] = GetPropertyValue("Area.YAxis.Labels.TextAlignment", component, true);
                        allProps["yAxisLabelsTextBefore"] = GetPropertyValue("Area.YAxis.Labels.TextBefore", component, true);
                        allProps["yAxisLabelsStep"] = GetPropertyValue("Area.YAxis.Labels.Step", component, true);
                        allProps["yAxisTitleAlignment"] = GetPropertyValue("Area.YAxis.Title.Alignment", component, true);
                        allProps["yAxisTitleColor"] = GetPropertyValue("Area.YAxis.Title.Color", component, true);
                        allProps["yAxisTitleDirection"] = GetPropertyValue("Area.YAxis.Title.Direction", component, true);
                        allProps["yAxisTitleFont"] = GetPropertyValue("Area.YAxis.Title.Font", component, true);
                        allProps["yAxisTitlePosition"] = GetPropertyValue("Area.YAxis.Title.Position", component, true);
                        allProps["yAxisTitleText"] = GetPropertyValue("Area.YAxis.Title.Text", component, true);
                        allProps["yAxisTitleVisible"] = GetPropertyValue("Area.YAxis.Title.Visible", component, true);
                        allProps["yAxisRangeAuto"] = GetPropertyValue("Area.YAxis.Range.Auto", component, true);
                        allProps["yAxisRangeMinimum"] = GetPropertyValue("Area.YAxis.Range.Minimum", component, true);
                        allProps["yAxisRangeMaximum"] = GetPropertyValue("Area.YAxis.Range.Maximum", component, true);
                        allProps["yAxisStartFromZero"] = GetPropertyValue("Area.YAxis.StartFromZero", component, true);

                        allProps["yRightAxisVisible"] = GetPropertyValue("Area.YRightAxis.Visible", component, true);
                        allProps["yRightAxisLabelsAngle"] = GetPropertyValue("Area.YRightAxis.Labels.Angle", component, true);
                        allProps["yRightAxisLabelsColor"] = GetPropertyValue("Area.YRightAxis.Labels.Color", component, true);
                        allProps["yRightAxisLabelsFont"] = GetPropertyValue("Area.YRightAxis.Labels.Font", component, true);
                        allProps["yRightAxisLabelsPlacement"] = GetPropertyValue("Area.YRightAxis.Labels.Placement", component, true);
                        allProps["yRightAxisLabelsTextAfter"] = GetPropertyValue("Area.YRightAxis.Labels.TextAfter", component, true);
                        allProps["yRightAxisLabelsTextAlignment"] = GetPropertyValue("Area.YRightAxis.Labels.TextAlignment", component, true);
                        allProps["yRightAxisLabelsTextBefore"] = GetPropertyValue("Area.YRightAxis.Labels.TextBefore", component, true);
                        allProps["yRightAxisTitleAlignment"] = GetPropertyValue("Area.YRightAxis.Title.Alignment", component, true);
                        allProps["yRightAxisTitleColor"] = GetPropertyValue("Area.YRightAxis.Title.Color", component, true);
                        allProps["yRightAxisTitleDirection"] = GetPropertyValue("Area.YRightAxis.Title.Direction", component, true);
                        allProps["yRightAxisTitleFont"] = GetPropertyValue("Area.YRightAxis.Title.Font", component, true);
                        allProps["yRightAxisTitlePosition"] = GetPropertyValue("Area.YRightAxis.Title.Position", component, true);
                        allProps["yRightAxisTitleText"] = GetPropertyValue("Area.YRightAxis.Title.Text", component, true);
                        allProps["yRightAxisTitleVisible"] = GetPropertyValue("Area.YRightAxis.Title.Visible", component, true);
                        allProps["yRightAxisStartFromZero"] = GetPropertyValue("Area.YRightAxis.StartFromZero", component, true);

                        allProps["labelsFont"] = GetPropertyValue("Labels.Font", component, true);
                        allProps["labelsForeColor"] = GetPropertyValue("Labels.ForeColor", component, true);
                        allProps["labelsAutoRotate"] = GetPropertyValue("Labels.AutoRotate", component, true);
                        allProps["labelsPosition"] = GetPropertyValue("Labels.Position", component, true);
                        allProps["labelsStyle"] = GetPropertyValue("Labels.Style", component, true);
                        allProps["labelsTextAfter"] = GetPropertyValue("Labels.TextAfter", component, true);
                        allProps["labelsTextBefore"] = GetPropertyValue("Labels.TextBefore", component, true);

                        allProps["legendHorAlignment"] = GetPropertyValue("Legend.HorAlignment", component, true);
                        allProps["legendVertAlignment"] = GetPropertyValue("Legend.VertAlignment", component, true);
                        allProps["legendColumns"] = GetPropertyValue("Legend.Columns", component, true);
                        allProps["legendDirection"] = GetPropertyValue("Legend.Direction", component, true);
                        allProps["legendVisible"] = GetPropertyValue("Legend.Visible", component, true);
                        allProps["legendVisibility"] = GetPropertyValue("Legend.Visibility", component, true);
                        allProps["legendLabelsColor"] = GetPropertyValue("Legend.Labels.Color", component, true);
                        allProps["legendLabelsFont"] = GetPropertyValue("Legend.Labels.Font", component, true);                        
                        allProps["legendTitleColor"] = GetPropertyValue("Legend.Title.Color", component, true);
                        allProps["legendTitleFont"] = GetPropertyValue("Legend.Title.Font", component, true);
                        allProps["legendTitleText"] = GetPropertyValue("Legend.Title.Text", component, true);

                        allProps["markerAngle"] = GetPropertyValue("Marker.Angle", component, true);
                        allProps["markerSize"] = GetPropertyValue("Marker.Size", component, true);
                        allProps["markerType"] = GetPropertyValue("Marker.Type", component, true);
                        allProps["markerVisible"] = GetPropertyValue("Marker.Visible", component, true);

                        allProps["options3DDistance"] = GetPropertyValue("Options3D.Distance", component, true);
                        allProps["options3DHeight"] = GetPropertyValue("Options3D.Height", component, true);
                        allProps["options3DLighting"] = GetPropertyValue("Options3D.Lighting", component, true);
                        allProps["options3DOpacity"] = GetPropertyValue("Options3D.Opacity", component, true);

                        allProps["isAxisAreaChart"] = (component as IStiChartElement).IsAxisAreaChart;
                        allProps["isAxisAreaChart3D"] = (component as IStiChartElement).IsAxisAreaChart3D;
                        allProps["isClusteredColumnChart3D"] = (component as IStiChartElement).IsClusteredColumnChart3D;                        
                        allProps["isPieChart"] = (component as IStiChartElement).IsPieChart;
                        allProps["isPie3dChart"] = (component as IStiChartElement).IsPie3dChart;
                        allProps["isDoughnutChart"] = (component as IStiChartElement).IsDoughnutChart;
                        allProps["isFunnelChart"] = (component as IStiChartElement).IsFunnelChart;
                        allProps["isPictorialStackedChart"] = (component as IStiChartElement).IsPictorialStackedChart;
                        allProps["isTreemapChart"] = (component as IStiChartElement).IsTreemapChart;
                        allProps["isRadarChart"] = (component as IStiChartElement).IsRadarChart;
                        allProps["isSunburstChart"] = (component as IStiChartElement).IsSunburstChart;
                        allProps["isStackedChart"] = (component as IStiChartElement).IsStackedChart;
                        allProps["isWaterfallChart"] = (component as IStiChartElement).IsWaterfallChart;
                        allProps["isScatterChart"] = (component as IStiChartElement).IsScatterChart;
                        allProps["isRange"] = (component as IStiChartElement).IsRange;

                        allProps["valueMeters"] = StiChartElementHelper.GetValueMeterItems(component as IStiChartElement);
                        allProps["argumentMeters"] = StiChartElementHelper.GetArgumentMeterItems(component as IStiChartElement);
                        allProps["seriesMeters"] = StiChartElementHelper.GetSeriesMeterItems(component as IStiChartElement);

                        if ((component as IStiChartElement).IsPieChart || (component as IStiChartElement).IsPie3dChart || (component as IStiChartElement).IsDoughnutChart)
                        {
                            var valueType = GetPropertyValue("Legend.Labels.ValueType", component) as string;
                            allProps["legendLabelsValueType"] = !String.IsNullOrEmpty(valueType) ? valueType : "Auto";
                        }
                        if ((component as IStiChartElement).IsParetoChart)
                        {
                            allProps["paretoSeriesColors"] = GetColorsCollectionProperty((component as IStiParetoSeriesColors)?.ParetoSeriesColors);
                        }
                        break;
                    }
                #endregion

                #region StiGaugeElement
                case "StiGaugeElement":
                    {
                        allProps["valueFormat"] = StiTextFormatHelper.GetTextFormatItem((component as IStiGaugeElement).ValueFormat);
                        allProps["type"] = (component as IStiGaugeElement).Type;
                        allProps["calculationMode"] = (component as IStiGaugeElement).CalculationMode;
                        allProps["minimum"] = DoubleToStr((component as IStiGaugeElement).Minimum);
                        allProps["maximum"] = DoubleToStr((component as IStiGaugeElement).Maximum);
                        allProps["rangeType"] = (component as IStiGaugeElement).RangeType;
                        allProps["rangeMode"] = (component as IStiGaugeElement).RangeMode;
                        allProps["shortValue"] = (component as IStiGaugeElement).ShortValue;
                        allProps["dashboardInteraction"] = GetDashboardInteractionProperty((component as IStiElementInteraction)?.DashboardInteraction);
                        allProps["labelsVisible"] = GetPropertyValue("Labels.Visible", component);
                        allProps["labelsPlacement"] = GetPropertyValue("Labels.Placement", component);
                        allProps["crossFiltering"] = (component as IStiGaugeElement).CrossFiltering;
                        allProps["dataMode"] = (component as IStiGaugeElement).DataMode;
                        allProps["targetSettingsShowLabel"] = GetPropertyValue("TargetSettings.ShowLabel", component);
                        allProps["targetSettingsPlacement"] = GetPropertyValue("TargetSettings.Placement", component);
                        break;
                    }
                #endregion

                #region StiPivotTableElement
                case "StiPivotTableElement":
                    {
                        allProps["pivotTableConditions"] = GetPivotTableConditionsProperty(component as IStiPivotTableElement);
                        allProps["valueMeters"] = GetPivotTableMeters(component as IStiPivotTableElement);
                        allProps["dashboardInteraction"] = GetDashboardInteractionProperty((component as IStiElementInteraction)?.DashboardInteraction);
                        allProps["summaryDirection"] = (component as IStiPivotTableElement).SummaryDirection;
                        allProps["crossFiltering"] = (component as IStiPivotTableElement).CrossFiltering;
                        break;
                    }
                #endregion

                #region StiIndicatorElement
                case "StiIndicatorElement":
                    {
                        allProps["dashboardInteraction"] = GetDashboardInteractionProperty((component as IStiElementInteraction)?.DashboardInteraction);
                        allProps["glyphColor"] = GetStringFromColor((component as IStiIndicatorElement).GlyphColor);
                        allProps["realGlyphColor"] = GetStringFromColor(StiDashboardStyleHelper.GetGlyphColor(component as IStiIndicatorElement));
                        allProps["icon"] = (component as IStiIndicatorElement).Icon;
                        allProps["iconSet"] = (component as IStiIndicatorElement).IconSet;
                        allProps["iconAlignment"] = (component as IStiIndicatorElement).IconAlignment;
                        allProps["topN"] = GetTopNProperty((component as IStiIndicatorElement).TopN);
                        allProps["allowUserSorting"] = StiSortMenuHelper.IsAllowUserSorting(component as IStiIndicatorElement);
                        allProps["customIcon"] = (component as IStiIndicatorElement).CustomIcon != null ? ImageToBase64((component as IStiIndicatorElement).CustomIcon) : string.Empty;
                        allProps["indicatorConditions"] = GetIndicatorConditionsProperty(component as IStiIndicatorElement);
                        allProps["dashboardInteraction"] = GetDashboardInteractionProperty((component as IStiElementInteraction)?.DashboardInteraction);
                        allProps["targetMode"] = (component as IStiIndicatorElement).TargetMode;
                        allProps["isSeriesPresent"] = StiIndicatorElementHelper.IsSeriesPresent(component as IStiIndicatorElement);
                        allProps["isTargetPresent"] = StiIndicatorElementHelper.IsTargetPresent(component as IStiIndicatorElement);
                        allProps["fontSizeMode"] = (component as IStiIndicatorElement).FontSizeMode;
                        allProps["font"] = FontToStr((component as IStiIndicatorElement).Font);
                        allProps["crossFiltering"] = (component as IStiIndicatorElement).CrossFiltering;
                        allProps["iconMode"] = (component as IStiIndicatorElement).IconMode;
                        allProps["iconRangeMode"] = (component as IStiIndicatorElement).IconRangeMode;
                        allProps["dataMode"] = (component as IStiIndicatorElement).DataMode;
                        break;
                    }
                #endregion

                #region StiProgressElement
                case "StiProgressElement":
                    {
                        allProps["mode"] = (component as IStiProgressElement).Mode;
                        allProps["font"] = FontToStr((component as IStiProgressElement).Font);
                        allProps["seriesColors"] = GetColorsCollectionProperty((component as IStiSeriesColors)?.SeriesColors);
                        allProps["topN"] = GetTopNProperty((component as IStiProgressElement).TopN);
                        allProps["allowUserSorting"] = StiSortMenuHelper.IsAllowUserSorting(component as IStiProgressElement);
                        allProps["colorEach"] = (component as IStiProgressElement).ColorEach;
                        allProps["dashboardInteraction"] = GetDashboardInteractionProperty((component as IStiElementInteraction)?.DashboardInteraction);
                        allProps["isSeriesPresent"] = StiProgressElementHelper.IsSeriesPresent(component as IStiProgressElement);
                        allProps["progressConditions"] = GetProgressConditionsProperty(component as IStiProgressElement);
                        allProps["crossFiltering"] = (component as IStiProgressElement).CrossFiltering;
                        allProps["dataMode"] = (component as IStiProgressElement).DataMode;
                        break;
                    }
                #endregion

                #region StiRegionMapElement
                case "StiRegionMapElement":
                    {
                        allProps["mapType"] = (component as IStiRegionMapElement).MapType;
                        allProps["mapID"] = (component as IStiRegionMapElement).MapIdent;
                        allProps["showValue"] = (component as IStiRegionMapElement).ShowValue;
                        allProps["showBubble"] = (component as IStiRegionMapElement).ShowBubble;
                        allProps["colorEach"] = (component as IStiRegionMapElement).ColorEach;
                        allProps["shortValue"] = (component as IStiRegionMapElement).ShortValue;
                        allProps["showZeros"] = (component as IStiRegionMapElement).ShowZeros;
                        allProps["displayNameType"] = (component as IStiRegionMapElement).ShowName;
                        allProps["dataFrom"] = (component as IStiRegionMapElement).DataFrom;
                        allProps["dashboardInteraction"] = GetDashboardInteractionProperty((component as IStiElementInteraction)?.DashboardInteraction);
                        allProps["crossFiltering"] = (component as IStiRegionMapElement).CrossFiltering;
                        break;
                    }
                #endregion

                #region StiOnlineMapElement
                case "StiOnlineMapElement":
                    {
                        var map = component as IStiOnlineMapElement;
                        allProps["locationType"] = map.LocationType;
                        allProps["culture"] = map.Culture;
                        allProps["iframeContent"] = GetOnlineMapContent(component as StiComponent);
                        allProps["cultures"] = GetCultures();
                        allProps["locationColor"] = GetStringFromColor(map.LocationColor);
                        allProps["locationColorType"] = map.LocationColorType;
                        allProps["valueViewMode"] = map.ValueViewMode;
                        allProps["icon"] = map.Icon.ToString();
                        allProps["iconColor"] = GetStringFromColor(map.IconColor);
                        allProps["customIcon"] = map.CustomIcon != null ? ImageToBase64(map.CustomIcon) : string.Empty;
                        allProps["dashboardInteraction"] = GetDashboardInteractionProperty((component as IStiElementInteraction)?.DashboardInteraction);
                        allProps["crossFiltering"] = (component as IStiOnlineMapElement).CrossFiltering;
                        break;
                    }
                #endregion

                #region StiImageElement
                case "StiImageElement":
                    {
                        allProps["imageSrc"] = ((IStiImageElement)component).Image != null ? ImageToBase64(((IStiImageElement)component).Image) : string.Empty;
                        allProps["imageUrl"] = ((IStiImageElement)component).ImageHyperlink != null ? StiEncodingHelper.Encode(((IStiImageElement)component).ImageHyperlink) : string.Empty;
                        allProps["icon"] = ((IStiImageElement)component).Icon;
                        allProps["iconColor"] = GetStringFromColor(((IStiImageElement)component).IconColor);
                        allProps["ratio"] = ((IStiImageElement)component).AspectRatio;
                        allProps["horAlignment"] = ((IStiHorAlignment)component).HorAlignment;
                        allProps["vertAlignment"] = ((IStiVertAlignment)component).VertAlignment;
                        allProps["stretch"] = true;
                        allProps["imageMultipleFactor"] = 1;
                        allProps["dashboardInteraction"] = GetDashboardInteractionProperty((component as IStiElementInteraction)?.DashboardInteraction);
                        allProps["crossFiltering"] = (component as IStiImageElement).CrossFiltering;
                        break;
                    }
                #endregion

                #region StiTextElement
                case "StiTextElement":
                    {
                        allProps["text"] = StiEncodingHelper.Encode(((IStiTextElement)component).Text);
                        allProps["vertAlignment"] = ((IStiVertAlignment)component).VertAlignment.ToString();
                        allProps["foreColor"] = GetStringFromColor(((IStiForeColor)component).ForeColor);
                        allProps["realForeColor"] = GetStringFromColor(StiDashboardStyleHelper.GetForeColor((IStiTextElement)component));
                        allProps["dashboardInteraction"] = GetDashboardInteractionProperty((component as IStiElementInteraction)?.DashboardInteraction);
                        allProps["font"] = FontToStr(StiTextElementHelper.GetFontProperty((IStiTextElement)component));
                        allProps["horAlignment"] = StiTextElementHelper.GetHorAlignmentProperty((IStiTextElement)component).ToString();
                        allProps["crossFiltering"] = (component as IStiTextElement).CrossFiltering;
                        allProps["textSizeMode"] = (component as IStiTextElement).SizeMode;
                        break;
                    }
                #endregion

                #region StiPanelElement
                case "StiPanelElement":
                    {
                        allProps["dashboardWatermark"] = GetDashboardWatermarkProperty(component);
                        break;
                    }
                #endregion

                #region StiShapeElement
                case "StiShapeElement":
                    {
                        allProps["shapeType"] = StiShapeElementHelper.GetShapeTypeProperty(component as IStiShapeElement);
                        allProps["size"] = StiReportEdit.DoubleToStr((component as IStiShapeElement).Size);
                        allProps["stroke"] = StiReportEdit.GetStringFromColor((component as IStiShapeElement).Stroke);
                        allProps["fill"] = StiReportEdit.BrushToStr((component as IStiShapeElement).Fill);
                        break;
                    }
                #endregion

                #region StiListBoxElement
                case "StiListBoxElement":
                    {
                        allProps["selectionMode"] = (component as IStiListBoxElement).SelectionMode;
                        allProps["showAllValue"] = (component as IStiListBoxElement).ShowAllValue;
                        allProps["showBlanks"] = (component as IStiListBoxElement).ShowBlanks;
                        allProps["font"] = FontToStr((component as IStiFont).Font);
                        allProps["parentKey"] = (component as IStiListBoxElement).GetParentKey();
                        allProps["orientation"] = (component as IStiListBoxElement).Orientation;
                        break;
                    }
                #endregion

                #region StiComboBoxElement
                case "StiComboBoxElement":
                    {
                        allProps["selectionMode"] = (component as IStiComboBoxElement).SelectionMode;
                        allProps["showAllValue"] = (component as IStiComboBoxElement).ShowAllValue;
                        allProps["showBlanks"] = (component as IStiComboBoxElement).ShowBlanks;
                        allProps["font"] = FontToStr((component as IStiFont).Font);
                        allProps["parentKey"] = (component as IStiComboBoxElement).GetParentKey();
                        break;
                    }
                #endregion

                #region StiTreeViewElement
                case "StiTreeViewElement":
                    {
                        allProps["selectionMode"] = (component as IStiTreeViewElement).SelectionMode;
                        allProps["showAllValue"] = (component as IStiTreeViewElement).ShowAllValue;
                        allProps["showBlanks"] = (component as IStiTreeViewElement).ShowBlanks;
                        allProps["font"] = FontToStr((component as IStiFont).Font);
                        allProps["parentKey"] = (component as IStiTreeViewElement).GetParentKey();
                        break;
                    }
                #endregion

                #region StiTreeViewBoxElement
                case "StiTreeViewBoxElement":
                    {
                        allProps["selectionMode"] = (component as IStiTreeViewBoxElement).SelectionMode;
                        allProps["showAllValue"] = (component as IStiTreeViewBoxElement).ShowAllValue;
                        allProps["showBlanks"] = (component as IStiTreeViewBoxElement).ShowBlanks;
                        allProps["font"] = FontToStr((component as IStiFont).Font);
                        allProps["parentKey"] = (component as IStiTreeViewBoxElement).GetParentKey();
                        break;
                    }
                #endregion

                #region StiDatePickerElement
                case "StiDatePickerElement":
                    {
                        allProps["selectionMode"] = (component as IStiDatePickerElement).SelectionMode;
                        allProps["conditionDatePicker"] = (component as IStiDatePickerElement).Condition;
                        allProps["initialRangeSelection"] = (component as IStiDatePickerElement).InitialRangeSelection;
                        allProps["initialRangeSelectionSource"] = (component as IStiDatePickerElement).InitialRangeSelectionSource;
                        allProps["font"] = FontToStr(((IStiFont)component).Font);
                        allProps["isVariablePresent"] = StiDatePickerElementHelper.IsVariablePresent(component as IStiDatePickerElement);
                        allProps["isRangeVariablePresent"] = StiDatePickerElementHelper.IsRangeVariablePresent(component as IStiDatePickerElement);
                        break;
                    }
                #endregion

                #region StiCardsElement
                case "StiCardsElement":
                    {
                        allProps["backColor"] = GetStringFromColor((component as IStiCardsElement).BackColor);
                        allProps["columnCount"] = (component as IStiCardsElement).ColumnCount.ToString();
                        allProps["orientation"] = (component as IStiCardsElement).Orientation;
                        allProps["cardsColorEach"] = (component as IStiCardsElement).GetCards().ColorEach;
                        allProps["cardsCornerRadius"] = CornerRadiusToStr((component as IStiCardsElement).GetCards().CornerRadius);
                        allProps["cardsMargin"] = MarginToStr((component as IStiCardsElement).GetCards().Margin);
                        allProps["cardsPadding"] = PaddingToStr((component as IStiCardsElement).GetCards().Padding);
                        allProps["seriesColors"] = GetColorsCollectionProperty((component as IStiSeriesColors)?.SeriesColors);
                        allProps["crossFiltering"] = (component as IStiCardsElement).CrossFiltering;
                        break;
                    }
                #endregion

                #region StiButtonElement
                case "StiButtonElement":
                    {
                        var buttonElement = component as IStiButtonElement;
                        allProps["buttonText"] = !string.IsNullOrEmpty(buttonElement.Text) ? StiEncodingHelper.Encode(buttonElement.Text) : "";
                        allProps["buttonChecked"] = buttonElement.Checked;
                        allProps["buttonGroup"] = buttonElement.Group;
                        allProps["buttonType"] = buttonElement.Type;
                        allProps["iconAlignment"] = buttonElement.IconAlignment;
                        allProps["horAlignment"] = (component as IStiTextHorAlignment).HorAlignment;
                        allProps["vertAlignment"] = (component as IStiVertAlignment).VertAlignment;
                        allProps["wordWrap"] = (component as IStiWordWrap).WordWrap;
                        allProps["buttonShapeType"] = buttonElement.ShapeType;
                        allProps["buttonStretch"] = buttonElement.Stretch;
                        allProps["brush"] = BrushToStr((component as IStiBrush).Brush);
                        allProps["textBrush"] = BrushToStr((component as IStiTextBrush).TextBrush);
                        allProps["iconBrush"] = BrushToStr(buttonElement.IconBrush);
                        allProps["font"] = FontToStr(buttonElement.Font);
                        allProps["buttonIconSet"] = GetButtonIconSetProperty(buttonElement.GetIconSet());
                        allProps["buttonVisualStates"] = GetButtonVisualStatesProperty(buttonElement.GetVisualStates());
                        allProps["styleColors"] = GetButtonStyleColors(buttonElement);
                        break;
                    }
                #endregion

                #region StiFormContainer
                case "StiFormContainer":
                    {
                        var form = component as StiFormContainer;
                        allProps["pageKey"] = form.Guid;
                        allProps["border"] = BorderToStr(form.Border);
                        allProps["brush"] = BrushToStr(form.Brush);
                        allProps["orientation"] = form.Orientation.ToString();
                        allProps["unitWidth"] = DoubleToStr(form.PageWidth);
                        allProps["unitHeight"] = DoubleToStr(form.PageHeight);
                        allProps["unitMargins"] = GetPageMargins(form);
                        allProps["columns"] = form.Columns.ToString();
                        allProps["columnWidth"] = DoubleToStr(form.ColumnWidth);
                        allProps["columnGaps"] = DoubleToStr(form.ColumnGaps);
                        allProps["rightToLeft"] = form.RightToLeft;
                        allProps["paperSize"] = ((int)form.PaperSize).ToString();
                        allProps["waterMarkRatio"] = form.Watermark.AspectRatio;
                        allProps["waterMarkRightToLeft"] = form.Watermark.RightToLeft;
                        allProps["waterMarkEnabled"] = form.Watermark.Enabled;
                        allProps["waterMarkEnabledExpression"] = StiEncodingHelper.Encode(form.Watermark.EnabledExpression);
                        allProps["waterMarkAngle"] = DoubleToStr(form.Watermark.Angle);
                        allProps["waterMarkText"] = StiEncodingHelper.Encode(form.Watermark.Text);
                        allProps["waterMarkFont"] = FontToStr(form.Watermark.Font);
                        allProps["waterMarkTextBrush"] = BrushToStr(form.Watermark.TextBrush);
                        allProps["waterMarkTextBehind"] = form.Watermark.ShowBehind;
                        allProps["waterMarkImageBehind"] = form.Watermark.ShowImageBehind;
                        allProps["waterMarkImageAlign"] = form.Watermark.ImageAlignment.ToString();
                        allProps["waterMarkMultipleFactor"] = DoubleToStr(form.Watermark.ImageMultipleFactor);
                        allProps["waterMarkStretch"] = form.Watermark.ImageStretch;
                        allProps["waterMarkTiling"] = form.Watermark.ImageTiling;
                        allProps["waterMarkTransparency"] = form.Watermark.ImageTransparency.ToString();
                        allProps["watermarkImageSrc"] = form.Watermark.TakeImage() != null ? ImageToBase64(form.Watermark.TakeImage()) : String.Empty;
                        allProps["watermarkImageHyperlink"] = form.Watermark.ImageHyperlink;

                        if (form.Watermark.TakeGdiImage() != null)
                        {
                            using (var gdiImage = form.Watermark.TakeGdiImage())
                            {
                                allProps["watermarkImageSize"] = gdiImage.Width + ";" + gdiImage.Height;
                            }
                        }
                        else allProps["watermarkImageSize"] = "0;0";

                        allProps["watermarkImageContentForPaint"] = GetWatermarkImageContentForPaint(form, allProps);
                        allProps["stopBeforePrint"] = form.StopBeforePrint.ToString();
                        allProps["titleBeforeHeader"] = form.TitleBeforeHeader;
                        allProps["aliasName"] = StiEncodingHelper.Encode(form.Alias);
                        allProps["largeHeight"] = form.LargeHeight;
                        allProps["largeHeightFactor"] = form.LargeHeightFactor.ToString();
                        allProps["largeHeightAutoFactor"] = form.LargeHeightAutoFactor.ToString();
                        allProps["resetPageNumber"] = form.ResetPageNumber;
                        allProps["printOnPreviousPage"] = form.PrintOnPreviousPage;
                        allProps["printHeadersFootersFromPreviousPage"] = form.PrintHeadersFootersFromPreviousPage;
                        allProps["enabled"] = form.Enabled;
                        allProps["mirrorMargins"] = form.MirrorMargins;
                        allProps["segmentPerWidth"] = form.SegmentPerWidth.ToString();
                        allProps["segmentPerHeight"] = form.SegmentPerHeight.ToString();
                        allProps["unlimitedHeight"] = form.UnlimitedHeight;
                        allProps["unlimitedBreakable"] = form.UnlimitedBreakable;
                        allProps["numberOfCopies"] = form.NumberOfCopies.ToString();
                        allProps["excelSheet"] = StiEncodingHelper.Encode(form.ExcelSheet?.Value);
                        allProps["pageIcon"] = form.Icon != null ? ImageToBase64(form.Icon) : string.Empty;
                        break;
                    }
                    #endregion
            }

            return allProps;
        }
        #endregion

        #region Get Property
        #region RichText
        public static string GetRichTextProperty(StiRichText component)
        {
            StiExpression text = component.Text;
            if (StiHyperlinkProcessor.IsServerHyperlink(text != null ? text.Value : null))
            {
                return StiEncodingHelper.Encode(text.Value);
            }
            RtfToHtmlConverter rtfConverter = new RtfToHtmlConverter();

            string htmlText = rtfConverter.ConvertRtfToHtml(component.RtfText);
            htmlText = htmlText.Replace("  ", "&nbsp;&nbsp;"); //Fixed bug with multispaces
            htmlText = htmlText.Replace("<SPAN />", "<BR>");

            return StiEncodingHelper.Encode(htmlText);
        }
        #endregion

        #region Sort
        public static string GetSortDataProperty(object object_)
        {
            ArrayList sorts = new ArrayList();

            string[] sort = null;
            PropertyInfo pi2 = object_.GetType().GetProperty("Sort");
            if (pi2 != null) sort = (string[])pi2.GetValue(object_, null);

            if (sort != null)
            {
                ArrayList singleSort = null;
                for (int i = 0; i < sort.Length; i++)
                {
                    if (sort[i] == "ASC" || sort[i] == "DESC" || i == sort.Length - 1)
                    {
                        if (i == sort.Length - 1) singleSort.Add(sort[i]);
                        if (singleSort != null) sorts.Add(GetSingleSort(object_, singleSort));
                        singleSort = new ArrayList();
                    }
                    singleSort.Add(sort[i]);
                }
            }

            return sorts.Count != 0 ? StiEncodingHelper.Encode(JSON.Encode(sorts)) : string.Empty;
        }

        private static string GetRelationNameByNameInSource(object object_, string nameInSource, ArrayList sortArray)
        {
            StiDictionary dictionary = null;

            if (object_ is StiComponent) dictionary = ((StiComponent)object_).Report.Dictionary;
            else if (object_ is StiDataSource) dictionary = ((StiDataSource)object_).Dictionary;

            if (dictionary != null)
            {
                //Try to get relation
                StiDataRelationsCollection relations = dictionary.Relations;

                foreach (StiDataRelation relation in relations)
                {
                    if (relation.NameInSource == nameInSource)
                        return relation.Name;
                }
            }

            return string.Empty;
        }

        private static Hashtable GetSingleSort(object object_, ArrayList sortArray_)
        {
            Hashtable oneSort = new Hashtable();
            Array sortArray = sortArray_.ToArray();
            oneSort["direction"] = (string)sortArray.GetValue(0);

            oneSort["column"] = string.Empty;
            for (int k = 1; k < sortArray.Length - 1; k++)
            {
                var relationName = GetRelationNameByNameInSource(object_, (string) sortArray.GetValue(k), sortArray_);
                if (string.IsNullOrEmpty(relationName)) relationName = sortArray_[k] as string;
                oneSort["column"] += relationName + ".";
            }

            oneSort["column"] += (string)sortArray.GetValue(sortArray.Length - 1);

            return oneSort;
        }
        #endregion

        #region Filter
        public static ArrayList GetFiltersObject(StiFiltersCollection filters)
        {
            ArrayList result = new ArrayList();
            foreach (StiFilter filter in filters)
            {
                Hashtable singleFilter = new Hashtable();
                result.Add(singleFilter);

                singleFilter["fieldIs"] = filter.Item.ToString();
                singleFilter["dataType"] = filter.DataType.ToString();
                singleFilter["column"] = filter.Column;
                singleFilter["condition"] = filter.Condition.ToString();
                singleFilter["value1"] = filter.Value1.ToString();
                singleFilter["value2"] = filter.Value2.ToString();

                string expression = filter.Expression.Value;
                if (!String.IsNullOrEmpty(expression) && expression.StartsWith("{") && expression.EndsWith("}"))
                    expression = expression.Substring(1, expression.Length - 2);

                singleFilter["expression"] = expression;
            }

            return result;
        }

        public static string GetFilterDataProperty(object component)
        {
            StiFiltersCollection filters = null;
            ArrayList result = new ArrayList();

            PropertyInfo pi = component.GetType().GetProperty("Filters");
            if (pi != null) filters = (StiFiltersCollection)pi.GetValue(component, null);
            if (filters != null) result = GetFiltersObject(filters);

            return StiEncodingHelper.Encode(JSON.Encode(result));
        }

        public static bool GetFilterOnProperty(object component)
        {
            bool filterOn = true;

            PropertyInfo pi = component.GetType().GetProperty("FilterOn");
            if (pi != null) filterOn = (bool)pi.GetValue(component, null);

            return filterOn;
        }

        public static string GetFilterModeProperty(object component)
        {
            StiFilterMode filterMode = StiFilterMode.And;

            PropertyInfo pi = component.GetType().GetProperty("FilterMode");
            if (pi != null) filterMode = (StiFilterMode)pi.GetValue(component, null);

            return filterMode.ToString();
        }
        #endregion        

        #region SvgContent
        public static string GetSvgContent(StiComponent component, double zoom = 1)
        {
            string svgContent = string.Empty;

            if (component == null || component.Width == 0 || component.Height == 0 || (component is StiImage && ((StiImage)component).Icon == null))
            {
                return svgContent;
            }

            if (component is IStiElement)
            {
                if (!(component is IStiPanel) && !(component is IStiButtonElement))
                {
                    svgContent = StiDashboardsSvgHelper.SaveElementToString(component as IStiElement, 1, 1, true);
                }
            }
            else
            {
                svgContent = StiSvgHelper.SaveComponentToString(component, ImageFormat.Png, 0.75f, (float)(zoom * 100));
            }

            return StiEncodingHelper.Encode(svgContent);
        }
        #endregion

        #region Conditions
        public static string GetConditionsProperty(object component)
        {
            ArrayList conditionsArray = new ArrayList();
            StiConditionsCollection conditions = null;
            PropertyInfo pi = component.GetType().GetProperty("Conditions");
            if (pi != null) conditions = (StiConditionsCollection)pi.GetValue(component, null);

            foreach (StiBaseCondition condition in conditions)
            {
                if (condition is StiDataBarCondition)
                {
                    conditionsArray.Add(GetDataBarConditionObject(condition as StiDataBarCondition));
                }
                else if (condition is StiIconSetCondition)
                {
                    conditionsArray.Add(GetIconSetConditionObject(condition as StiIconSetCondition));
                }
                else if (condition is StiColorScaleCondition)
                {
                    conditionsArray.Add(GetColorScaleConditionObject(condition as StiColorScaleCondition));
                }
                else if (condition is StiCondition)
                {
                    conditionsArray.Add(GetHighlightConditionObject(condition as StiCondition));
                }

            }
            return conditionsArray.Count > 0 ? StiEncodingHelper.Encode(JSON.Encode(conditionsArray)) : "";
        }

        public static Hashtable GetDataBarConditionObject(StiDataBarCondition condition)
        {
            Hashtable conditionObject = new Hashtable();
            conditionObject["ConditionType"] = "StiDataBarCondition";
            conditionObject["Column"] = condition.Column;
            conditionObject["MaximumType"] = condition.MaximumType;
            conditionObject["MaximumValue"] = DoubleToStr(condition.MaximumValue);
            conditionObject["MinimumType"] = condition.MinimumType;
            conditionObject["MinimumValue"] = DoubleToStr(condition.MinimumValue);
            conditionObject["Direction"] = condition.Direction;
            conditionObject["BrushType"] = condition.BrushType;
            conditionObject["PositiveColor"] = GetStringFromColor(condition.PositiveColor);
            conditionObject["NegativeColor"] = GetStringFromColor(condition.NegativeColor);
            conditionObject["ShowBorder"] = condition.ShowBorder;
            conditionObject["PositiveBorderColor"] = GetStringFromColor(condition.PositiveBorderColor);
            conditionObject["NegativeBorderColor"] = GetStringFromColor(condition.NegativeBorderColor);

            return conditionObject;
        }

        public static Hashtable GetIconSetConditionObject(StiIconSetCondition condition)
        {
            Hashtable conditionObject = new Hashtable();
            conditionObject["ConditionType"] = "StiIconSetCondition";
            conditionObject["Column"] = condition.Column;
            conditionObject["ContentAlignment"] = condition.ContentAlignment;
            conditionObject["IconSet"] = condition.IconSet;
            conditionObject["IconSetItem1"] = GetIconSetItemObject(condition.IconSetItem1);
            conditionObject["IconSetItem2"] = GetIconSetItemObject(condition.IconSetItem2);
            conditionObject["IconSetItem3"] = GetIconSetItemObject(condition.IconSetItem3);
            conditionObject["IconSetItem4"] = GetIconSetItemObject(condition.IconSetItem4);
            conditionObject["IconSetItem5"] = GetIconSetItemObject(condition.IconSetItem5);

            return conditionObject;
        }

        public static Hashtable GetColorScaleConditionObject(StiColorScaleCondition condition)
        {
            Hashtable conditionObject = new Hashtable();
            conditionObject["ConditionType"] = "StiColorScaleCondition";
            conditionObject["Column"] = condition.Column;
            conditionObject["ScaleType"] = condition.ScaleType;
            conditionObject["MaximumType"] = condition.MaximumType;
            conditionObject["MaximumValue"] = DoubleToStr(condition.MaximumValue);
            conditionObject["MaximumColor"] = GetStringFromColor(condition.MaximumColor);
            conditionObject["MinimumType"] = condition.MinimumType;
            conditionObject["MinimumValue"] = DoubleToStr(condition.MinimumValue);
            conditionObject["MinimumColor"] = GetStringFromColor(condition.MinimumColor);
            conditionObject["MidType"] = condition.MidType;
            conditionObject["MidValue"] = DoubleToStr(condition.MidValue);
            conditionObject["MidColor"] = GetStringFromColor(condition.MidColor);

            return conditionObject;
        }

        public static Hashtable GetHighlightConditionObject(StiCondition condition)
        {
            Hashtable conditionObject = new Hashtable();
            conditionObject["ConditionType"] = "StiHighlightCondition";
            conditionObject["BorderSides"] = condition.BorderSides.ToString();
            conditionObject["Permissions"] = condition.Permissions.ToString();
            conditionObject["Style"] = condition.Style.ToString();
            conditionObject["Font"] = FontToStr(condition.Font);
            conditionObject["BackColor"] = GetStringFromColor(condition.BackColor);
            conditionObject["TextColor"] = GetStringFromColor(condition.TextColor);
            conditionObject["Enabled"] = condition.Enabled;
            conditionObject["AssignExpression"] = StiEncodingHelper.Encode(condition.AssignExpression);
            conditionObject["CanAssignExpression"] = condition.CanAssignExpression;
            conditionObject["BreakIfTrue"] = condition.BreakIfTrue;

            if (condition is StiMultiCondition)
            {
                conditionObject["Filters"] = GetFilterDataProperty((StiMultiCondition)condition);
                conditionObject["FilterMode"] = GetFilterModeProperty((StiMultiCondition)condition);
            }
            else
            {
                ArrayList filters = new ArrayList();
                Hashtable filter = new Hashtable();
                filters.Add(filter);

                filter["fieldIs"] = condition.Item.ToString();
                filter["dataType"] = condition.DataType.ToString();
                filter["column"] = condition.Column;
                filter["condition"] = condition.Condition.ToString();
                filter["value1"] = condition.Value1.ToString();
                filter["value2"] = condition.Value2.ToString();

                string expression = condition.Expression.Value;
                if (!String.IsNullOrEmpty(expression) && expression.StartsWith("{") && expression.EndsWith("}"))
                    expression = expression.Substring(1, expression.Length - 2);

                filter["expression"] = expression;

                conditionObject["Filters"] = StiEncodingHelper.Encode(JSON.Encode(filters));
            }

            return conditionObject;
        }
        #endregion

        #region HeaderSize
        public static string GetComponentHeaderSize(object component)
        {
            object headerSize = GetPropertyValue("HeaderSize", component);
            return headerSize != null ? (string)headerSize : "0";
        }
        #endregion

        #region Interaction
        public static Hashtable GetInteractionProperty(StiInteraction interaction)
        {
            Hashtable interactionObject = new Hashtable();

            if (interaction is StiBandInteraction)
            {
                var bandInteraction = interaction as StiBandInteraction;
                interactionObject["isBandInteraction"] = true;
                interactionObject["collapsingEnabled"] = bandInteraction.CollapsingEnabled;
                interactionObject["collapseGroupFooter"] = bandInteraction.CollapseGroupFooter;
                interactionObject["collapsedValue"] = StiEncodingHelper.Encode(bandInteraction.Collapsed.Value);
                interactionObject["selectionEnabled"] = bandInteraction.SelectionEnabled;
            }

            if (interaction is StiCrossHeaderInteraction) {
                var crossHeaderInteraction = interaction as StiCrossHeaderInteraction;
                interactionObject["isCrossHeaderInteraction"] = true;
                interactionObject["crossHeaderCollapsingEnabled"] = crossHeaderInteraction.CollapsingEnabled;
            }

            interactionObject["drillDownEnabled"] = interaction.DrillDownEnabled;
            interactionObject["drillDownReport"] = interaction.DrillDownReport;
            interactionObject["drillDownMode"] = interaction.DrillDownMode.ToString();
            interactionObject["drillDownPage"] = interaction.DrillDownPage != null ? interaction.DrillDownPage.Name : "";

            interactionObject["drillDownParameter1Name"] = interaction.DrillDownParameter1.Name;
            interactionObject["drillDownParameter1Expression"] = StiEncodingHelper.Encode(interaction.DrillDownParameter1.Expression.Value);
            interactionObject["drillDownParameter2Name"] = interaction.DrillDownParameter2.Name;
            interactionObject["drillDownParameter2Expression"] = StiEncodingHelper.Encode(interaction.DrillDownParameter2.Expression.Value);
            interactionObject["drillDownParameter3Name"] = interaction.DrillDownParameter3.Name;
            interactionObject["drillDownParameter3Expression"] = StiEncodingHelper.Encode(interaction.DrillDownParameter3.Expression.Value);
            interactionObject["drillDownParameter4Name"] = interaction.DrillDownParameter4.Name;
            interactionObject["drillDownParameter4Expression"] = StiEncodingHelper.Encode(interaction.DrillDownParameter4.Expression.Value);
            interactionObject["drillDownParameter5Name"] = interaction.DrillDownParameter5.Name;
            interactionObject["drillDownParameter5Expression"] = StiEncodingHelper.Encode(interaction.DrillDownParameter5.Expression.Value);
            interactionObject["drillDownParameter6Name"] = interaction.DrillDownParameter6.Name;
            interactionObject["drillDownParameter6Expression"] = StiEncodingHelper.Encode(interaction.DrillDownParameter6.Expression.Value);
            interactionObject["drillDownParameter7Name"] = interaction.DrillDownParameter7.Name;
            interactionObject["drillDownParameter7Expression"] = StiEncodingHelper.Encode(interaction.DrillDownParameter7.Expression.Value);
            interactionObject["drillDownParameter8Name"] = interaction.DrillDownParameter8.Name;
            interactionObject["drillDownParameter8Expression"] = StiEncodingHelper.Encode(interaction.DrillDownParameter8.Expression.Value);
            interactionObject["drillDownParameter9Name"] = interaction.DrillDownParameter9.Name;
            interactionObject["drillDownParameter9Expression"] = StiEncodingHelper.Encode(interaction.DrillDownParameter9.Expression.Value);
            interactionObject["drillDownParameter10Name"] = interaction.DrillDownParameter10.Name;
            interactionObject["drillDownParameter10Expression"] = StiEncodingHelper.Encode(interaction.DrillDownParameter10.Expression.Value);

            interactionObject["sortingEnabled"] = interaction.SortingEnabled;

            if (!string.IsNullOrEmpty(interaction.SortingColumn))
            {
                string column = interaction.SortingColumn;
                if (column.StartsWith("{"))
                    column = column.Remove(0, 1);
                if (column.EndsWith("}"))
                    column = column.Remove(column.Length - 1, 1);
                interactionObject["sortingColumn"] = column;
            }
            else
                interactionObject["sortingColumn"] = string.Empty;

            interactionObject["bookmark"] = StiEncodingHelper.Encode(interaction.Bookmark.Value);

            string hyperlinkText = interaction.Hyperlink.Value;
            if (hyperlinkText.StartsWith("##"))
            {
                interactionObject["hyperlinkType"] = "HyperlinkUsingInteractionTag";
                interactionObject["hyperlink"] = StiEncodingHelper.Encode(hyperlinkText.Remove(0, 2));
            }
            else if (hyperlinkText.StartsWith("#"))
            {
                interactionObject["hyperlinkType"] = "HyperlinkUsingInteractionBookmark";
                interactionObject["hyperlink"] = StiEncodingHelper.Encode(hyperlinkText.Remove(0, 1));
            }
            else
            {
                interactionObject["hyperlinkType"] = "HyperlinkExternalDocuments";
                interactionObject["hyperlink"] = StiEncodingHelper.Encode(hyperlinkText);
            }

            interactionObject["tag"] = StiEncodingHelper.Encode(interaction.Tag.Value);
            interactionObject["toolTip"] = StiEncodingHelper.Encode(interaction.ToolTip.Value);

            return interactionObject;
        }
        #endregion

        #region CrossTabComponents
        public static Hashtable GetCrossTabFieldsProperties(StiCrossTab crossTab)
        {
            StiCrossTabHelper crossTabHelper = new StiCrossTabHelper(crossTab);
            Hashtable properties = crossTabHelper.GetFieldsPropertiesForJS();
            crossTabHelper.RestorePositions();
            crossTabHelper = null;

            return properties;
        }
        #endregion

        #region Events
        public static Hashtable GetEventsProperty(object element)
        {
            var events = new Hashtable();

            var eventNames = new string[] { "GetExcelValueEvent", "GetCollapsedEvent", "GetTagEvent", "GetToolTipEvent", "GetValueEvent", "ClickEvent", "DoubleClickEvent",
                "GetDrillDownReportEvent", "MouseEnterEvent", "MouseLeaveEvent", "GetBookmarkEvent", "GetHyperlinkEvent", "AfterPrintEvent", "BeforePrintEvent",
                "BeginRenderEvent", "ColumnBeginRenderEvent", "ColumnEndRenderEvent", "EndRenderEvent", "RenderingEvent", "ExportedEvent", "ExportingEvent",
                "PrintedEvent", "PrintingEvent", "GetDataUrlEvent", "GetImageDataEvent", "GetImageURLEvent", "GetBarCodeEvent", "GetCheckedEvent", "FillParametersEvent",
                "GetZipCodeEvent", "GetSummaryExpressionEvent", "RefreshingEvent", "ProcessChartEvent" };

            if (element is IStiElement)
            {
                eventNames = new string[] { };

                if (element is IStiButtonElement)
                    eventNames = new string[] { "ClickEvent", "CheckedChangedEvent" };
            }

            foreach (string eventName in eventNames)
            {
                var value = GetPropertyValue(eventName, element);
                if (value != null) events[eventName] = StiEncodingHelper.Encode(value as string);
            }

            return events;
        }
        #endregion

        #region SubReportParameters
        public static ArrayList GetSubReportParametersProperty(StiSubReport subReport)
        {
            ArrayList parameters = new ArrayList();

            foreach (StiParameter parameter in subReport.Parameters)
            {
                Hashtable parameterObject = new Hashtable();
                parameterObject["name"] = parameter.Name;
                parameterObject["expression"] = StiEncodingHelper.Encode(parameter.Expression);
                parameters.Add(parameterObject);
            }

            return parameters;
        }
        #endregion                

        #region ShapeType
        public static string GetShapeTypeProperty(StiShape component)
        {
            
            if (component.ShapeType is StiArrowShapeType)
            {
                StiArrowShapeType arrowShapeType = new StiArrowShapeType();
                switch (((StiArrowShapeType)component.ShapeType).Direction)
                {
                    case StiShapeDirection.Up: return "StiArrowShapeTypeUp";
                    case StiShapeDirection.Down: return "StiArrowShapeTypeDown";
                    case StiShapeDirection.Right: return "StiArrowShapeTypeRight";
                    case StiShapeDirection.Left: return "StiArrowShapeTypeLeft";
                }
            }

            return component.ShapeType.GetType().Name;
        }
        #endregion

        #region PreviewSettings
        public static Hashtable GetPreviewSettingsProperty(StiReport report)
        {
            Hashtable settingsJsObject = new Hashtable();

            var repSettings = (StiPreviewSettings)report.PreviewSettings;
            settingsJsObject["reportPrint"] = (repSettings & StiPreviewSettings.Print) > 0;
            settingsJsObject["reportOpen"] = (repSettings & StiPreviewSettings.Open) > 0;
            settingsJsObject["reportSave"] = (repSettings & StiPreviewSettings.Save) > 0;
            settingsJsObject["reportSendEMail"] = (repSettings & StiPreviewSettings.SendEMail) > 0;
            settingsJsObject["reportPageControl"] = (repSettings & StiPreviewSettings.PageControl) > 0;
            settingsJsObject["reportEditor"] = (repSettings & StiPreviewSettings.Editor) > 0;
            settingsJsObject["reportFind"] = (repSettings & StiPreviewSettings.Find) > 0;
            settingsJsObject["reportSignature"] = (repSettings & StiPreviewSettings.Signature) > 0;
            settingsJsObject["reportPageViewMode"] = (repSettings & StiPreviewSettings.PageViewMode) > 0;
            settingsJsObject["reportZoom"] = (repSettings & StiPreviewSettings.Zoom) > 0;
            settingsJsObject["reportBookmarks"] = (repSettings & StiPreviewSettings.Bookmarks) > 0;
            settingsJsObject["reportParameters"] = (repSettings & StiPreviewSettings.Parameters) > 0;
            settingsJsObject["reportResources"] = (repSettings & StiPreviewSettings.Resources) > 0;
            settingsJsObject["reportStatusBar"] = (repSettings & StiPreviewSettings.StatusBar) > 0;
            settingsJsObject["reportToolbar"] = (repSettings & StiPreviewSettings.Toolbar) > 0;

            var dbsSettings = report.DashboardViewerSettings;
            settingsJsObject["dashboardToolBar"] = (dbsSettings & StiDashboardViewerSettings.ShowToolBar) > 0;
            settingsJsObject["dashboardRefreshButton"] = (dbsSettings & StiDashboardViewerSettings.ShowRefreshButton) > 0;
            settingsJsObject["dashboardOpenButton"] = (dbsSettings & StiDashboardViewerSettings.ShowOpenButton) > 0;
            settingsJsObject["dashboardEditButton"] = (dbsSettings & StiDashboardViewerSettings.ShowEditButton) > 0;
            settingsJsObject["dashboardFullScreenButton"] = (dbsSettings & StiDashboardViewerSettings.ShowFullScreenButton) > 0;
            settingsJsObject["dashboardMenuButton"] = (dbsSettings & StiDashboardViewerSettings.ShowMenuButton) > 0;
            settingsJsObject["dashboardResetAllFiltersButton"] = (dbsSettings & StiDashboardViewerSettings.ShowResetAllFilters) > 0;
            settingsJsObject["dashboardParametersButton"] = (dbsSettings & StiDashboardViewerSettings.ShowParametersButton) > 0;
            settingsJsObject["dashboardShowReportSnapshots"] = (dbsSettings & StiDashboardViewerSettings.ShowReportSnapshots) > 0;
            settingsJsObject["dashboardShowExports"] = (dbsSettings & StiDashboardViewerSettings.ShowExports) > 0;

            settingsJsObject["htmlPreviewMode"] = report.HtmlPreviewMode;
            settingsJsObject["reportToolbarHorAlignment"] = report.PreviewToolBarOptions.ReportToolbarHorAlignment;
            settingsJsObject["reportToolbarReverse"] = report.PreviewToolBarOptions.ReportToolbarReverse;
            settingsJsObject["dashboardToolbarHorAlignment"] = report.PreviewToolBarOptions.DashboardToolbarHorAlignment;
            settingsJsObject["dashboardToolbarReverse"] = report.PreviewToolBarOptions.DashboardToolbarReverse;

            return settingsJsObject;
        }
        #endregion

        #region TopN
        public static Hashtable GetTopNProperty(StiDataTopN topN)
        {
            Hashtable topNObject = new Hashtable();

            topNObject["mode"] = topN.Mode.ToString();
            topNObject["count"] = topN.Count;
            topNObject["showOthers"] = topN.ShowOthers;
            topNObject["othersText"] = topN.OthersText;
            topNObject["measureField"] = topN.MeasureField;
            topNObject["stringContent"] = topN.ToString();

            return topNObject;
        }
        #endregion

        #region OnlineMapContent
        public static string GetOnlineMapContent(StiComponent component)
        {
            var result = StiDashboardOnlimeMapHelper.GetBingMapScript(component as IStiElement, false);
            return StiEncodingHelper.Encode(result);
        }

        public static ArrayList GetCultures()
        {
            ArrayList cultures = new ArrayList();
            foreach (StiOnlineMapCulture culture in (StiOnlineMapCulture[])Enum.GetValues(typeof(StiOnlineMapCulture)))
                cultures.Add(culture.ToString());
            return cultures;
        }
        #endregion

        #region Dashboard Interaction
        public static Hashtable GetDashboardInteractionProperty(IStiDashboardInteraction dashboardInteraction)
        {
            Hashtable interactionObject = new Hashtable();
            if (dashboardInteraction != null)
            {
                interactionObject["ident"] = dashboardInteraction.Ident;
                interactionObject["onHover"] = dashboardInteraction.OnHover;
                interactionObject["onClick"] = dashboardInteraction.OnClick;
                interactionObject["hyperlinkDestination"] = dashboardInteraction.HyperlinkDestination;
                interactionObject["toolTip"] = StiEncodingHelper.Encode(dashboardInteraction.ToolTip);
                interactionObject["hyperlink"] = StiEncodingHelper.Encode(dashboardInteraction.Hyperlink);
                interactionObject["drillDownPageKey"] = dashboardInteraction.DrillDownPageKey;
                interactionObject["isDefault"] = dashboardInteraction.IsDefault;

                var drillDownParameters = dashboardInteraction.GetDrillDownParameters();
                if (drillDownParameters != null) {
                    interactionObject["drillDownParameters"] = new ArrayList();
                    drillDownParameters.ForEach(p =>
                    {
                        var drillDownParameter = new Hashtable();
                        drillDownParameter["name"] = p.Name;
                        drillDownParameter["expression"] = p.Expression;
                        ((ArrayList)interactionObject["drillDownParameters"]).Add(drillDownParameter);
                    });
                }

                if (dashboardInteraction is IStiAllowUserColumnSelectionDashboardInteraction)
                {
                    interactionObject["allowUserColumnSelection"] = ((IStiAllowUserColumnSelectionDashboardInteraction)dashboardInteraction).AllowUserColumnSelection;
                }

                if (dashboardInteraction is IStiAllowUserSortingDashboardInteraction)
                {
                    interactionObject["allowUserSorting"] = ((IStiAllowUserSortingDashboardInteraction)dashboardInteraction).AllowUserSorting;
                }

                if (dashboardInteraction is IStiAllowUserFilteringDashboardInteraction)
                {
                    interactionObject["allowUserFiltering"] = ((IStiAllowUserFilteringDashboardInteraction)dashboardInteraction).AllowUserFiltering;
                }

                if (dashboardInteraction is IStiAllowUserDrillDownDashboardInteraction)
                {
                    interactionObject["allowUserDrillDown"] = ((IStiAllowUserDrillDownDashboardInteraction)dashboardInteraction).AllowUserDrillDown;
                }

                if (dashboardInteraction is IStiInteractionLayout)
                {
                    interactionObject["showFullScreenButton"] = ((IStiInteractionLayout)dashboardInteraction).ShowFullScreenButton;
                    interactionObject["showSaveButton"] = ((IStiInteractionLayout)dashboardInteraction).ShowSaveButton;
                    interactionObject["showViewDataButton"] = ((IStiInteractionLayout)dashboardInteraction).ShowViewDataButton;
                }

                if (dashboardInteraction is IStiTableDashboardInteraction)
                {   
                    interactionObject["drillDownFiltered"] = ((IStiTableDashboardInteraction)dashboardInteraction).DrillDownFiltered;
                    interactionObject["fullRowSelect"] = ((IStiTableDashboardInteraction)dashboardInteraction).FullRowSelect;
                }

                if (dashboardInteraction is IStiChartDashboardInteraction)
                {
                    interactionObject["viewsState"] = ((IStiChartDashboardInteraction)dashboardInteraction).ViewsState;
                }
            }

            return interactionObject;
        }
        #endregion
                
        #region Chart Conditions
        public static ArrayList GetChartConditionsProperty(IStiChartElement chartElement)
        {
            ArrayList chartConditionsArray = new ArrayList();
            var chartConditions = chartElement.FetchChartConditions();
            if (chartConditions != null)
            {
                chartConditions.ForEach(c =>
                {
                    Hashtable chartConditionObject = new Hashtable();
                    chartConditionObject["keyValueMeter"] = c.KeyValueMeter;
                    chartConditionObject["dataType"] = c.DataType.ToString();
                    chartConditionObject["condition"] = c.Condition.ToString();
                    chartConditionObject["value"] = !string.IsNullOrEmpty(c.Value) ? StiEncodingHelper.Encode(c.Value) : string.Empty;
                    chartConditionObject["color"] = GetStringFromColor(c.Color);
                    chartConditionObject["markerType"] = c.MarkerType.ToString();
                    chartConditionObject["markerAngle"] = DoubleToStr(c.MarkerAngle);
                    chartConditionObject["isExpression"] = c.IsExpression;
                    chartConditionsArray.Add(chartConditionObject);
                });
            }

            return chartConditionsArray;
        }
        #endregion

        #region Trend Lines
        public static ArrayList GetChartTrendLinesProperty(IStiChartElement chartElement)
        {
            ArrayList trendLinesArray = new ArrayList();
            var trendLines = chartElement.FetchTrendLines();
            if (trendLines != null)
            {
                trendLines.ForEach(tLine =>
                {
                    Hashtable trendLinesObject = new Hashtable();
                    trendLinesObject["keyValueMeter"] = tLine.KeyValueMeter;
                    trendLinesObject["type"] = tLine.Type.ToString();
                    trendLinesObject["lineColor"] = GetStringFromColor(tLine.LineColor);
                    trendLinesObject["lineStyle"] = ((int)tLine.LineStyle).ToString();
                    trendLinesObject["lineWidth"] = DoubleToStr(tLine.LineWidth);
                    trendLinesArray.Add(trendLinesObject);
                });
            }

            return trendLinesArray;
        }
        #endregion

        #region Indicator Conditions
        public static ArrayList GetIndicatorConditionsProperty(IStiIndicatorElement indicatorElement)
        {
            ArrayList indicatorConditionsArray = new ArrayList();
            var indicatorConditions = indicatorElement.FetchIndicatorConditions();
            if (indicatorConditions != null)
            {
                indicatorConditions.ForEach(c =>
                {
                    Hashtable conditionObject = new Hashtable();
                    conditionObject["field"] = c.Field;
                    conditionObject["condition"] = c.Condition;
                    conditionObject["value"] = !string.IsNullOrEmpty(c.Value) ? StiEncodingHelper.Encode(c.Value) : string.Empty;
                    conditionObject["font"] = FontToStr(c.Font);
                    conditionObject["textColor"] = GetStringFromColor(c.TextColor);
                    conditionObject["backColor"] = GetStringFromColor(c.BackColor);
                    conditionObject["icon"] = c.Icon;
                    conditionObject["customIcon"] = c.CustomIcon != null ? ImageToBase64(c.CustomIcon) : string.Empty;
                    conditionObject["iconAlignment"] = c.IconAlignment;
                    conditionObject["iconColor"] = GetStringFromColor(c.IconColor);
                    conditionObject["targetIcon"] = c.TargetIcon;
                    conditionObject["targetIconAlignment"] = c.TargetIconAlignment;
                    conditionObject["targetIconColor"] = GetStringFromColor(c.TargetIconColor);
                    conditionObject["permissions"] = c.Permissions.ToString();
                    indicatorConditionsArray.Add(conditionObject);
                });
            }

            return indicatorConditionsArray;
        }
        #endregion

        #region Progress Conditions
        public static ArrayList GetProgressConditionsProperty(IStiProgressElement progressElement)
        {
            ArrayList progressConditionsArray = new ArrayList();
            var progressConditions = progressElement.FetchProgressConditions();
            if (progressConditions != null)
            {
                progressConditions.ForEach(c =>
                {
                    Hashtable conditionObject = new Hashtable();
                    conditionObject["field"] = c.Field;
                    conditionObject["condition"] = c.Condition;
                    conditionObject["value"] = !string.IsNullOrEmpty(c.Value) ? StiEncodingHelper.Encode(c.Value) : string.Empty;
                    conditionObject["font"] = FontToStr(c.Font);
                    conditionObject["textColor"] = GetStringFromColor(c.TextColor);
                    conditionObject["color"] = GetStringFromColor(c.Color);
                    conditionObject["trackColor"] = GetStringFromColor(c.TrackColor);
                    conditionObject["permissions"] = c.Permissions.ToString();
                    progressConditionsArray.Add(conditionObject);
                });
            }

            return progressConditionsArray;
        }
        #endregion

        #region Progress Conditions
        public static ArrayList GetTableConditionsProperty(IStiTableElement tableElement)
        {
            ArrayList conditionsArray = new ArrayList();
            if (tableElement.TableConditions != null)
            {
                tableElement.TableConditions.ForEach(c =>
                {
                    var conditionObject = new Hashtable
                    {
                        ["keyDataFieldMeters"] = new ArrayList(c.KeyDataFieldMeters),
                        ["keyDestinationMeters"] = new ArrayList(c.KeyDestinationMeters),
                        ["dataType"] = c.DataType.ToString(),
                        ["condition"] = c.Condition.ToString(),
                        ["value"] = !string.IsNullOrEmpty(c.Value) ? StiEncodingHelper.Encode(c.Value) : string.Empty,
                        ["font"] = FontToStr(c.Font),
                        ["foreColor"] = GetStringFromColor(c.ForeColor),
                        ["backColor"] = GetStringFromColor(c.BackColor),
                        ["isExpression"] = c.IsExpression,
                        ["permissions"] = c.Permissions.ToString()
                    };
                    conditionsArray.Add(conditionObject);
                });
            }

            return conditionsArray;
        }

        private static ArrayList GetTableMeters(IStiTableElement tableElement)
        {
            var items = new ArrayList();
            tableElement.FetchAllMeters().ForEach(m =>
            {
                items.Add(new Hashtable
                {
                    ["label"] = StiTableElementHelper.GetMeterLabel(m),
                    ["key"] = m.Key
                });
            });
            return items;
        }
        #endregion

        #region PivotTable Conditions
        public static ArrayList GetPivotTableConditionsProperty(IStiPivotTableElement pivotTableElement)
        {
            ArrayList pivotTableConditionsArray = new ArrayList();
            var pivotTableConditions = pivotTableElement.PivotTableConditions;
            if (pivotTableConditions != null)
            {
                pivotTableConditions.ForEach(c =>
                {
                    Hashtable pivotTableConditionObject = new Hashtable
                    {
                        ["keyValueMeter"] = c.KeyValueMeter,
                        ["dataType"] = c.DataType.ToString(),
                        ["condition"] = c.Condition.ToString(),
                        ["value"] = !string.IsNullOrEmpty(c.Value) ? StiEncodingHelper.Encode(c.Value) : string.Empty,
                        ["backColor"] = GetStringFromColor(c.BackColor),
                        ["customIcon"] = c.CustomIcon != null ? ImageToBase64(c.CustomIcon) : string.Empty,
                        ["font"] = FontToStr(c.Font),
                        ["icon"] = c.Icon.ToString(),
                        ["iconAlignment"] = c.IconAlignment.ToString(),
                        ["iconColor"] = GetStringFromColor(c.IconColor),
                        ["permissions"] = c.Permissions.ToString(),
                        ["textColor"] = GetStringFromColor(c.TextColor)
                    };
                    pivotTableConditionsArray.Add(pivotTableConditionObject);
                });
            }

            return pivotTableConditionsArray;
        }

        private static Hashtable GetPivotTableMeters(IStiPivotTableElement pivotTableElement)
        {
            var result = new Hashtable();
            foreach (var pivotTableMeter in pivotTableElement.GetAllMeters())
            {
                var label = StiLabelHelper.GetLabel(pivotTableMeter);
                if (!result.Contains(pivotTableMeter.Key))
                    result.Add(pivotTableMeter.Key, label);
            }
            return result;
        }
        #endregion

        #region Expressions
        public static Hashtable GetExpressionsProperty(StiComponent component)
        {
            Hashtable expressions = new Hashtable();
            var appExpessions = component as IStiAppExpressionCollection;

            if (appExpessions != null)
            {   
                foreach (var expression in appExpessions.Expressions.ToList().Where(e => !e.IsEmpty))
                {
                    expressions[LowerFirstChar(expression.Name)] = StiEncodingHelper.Encode(expression.Expression);
                }
            }

            return expressions;
        }
        #endregion

        #region Styles
        public static ArrayList GetStylesProperty(object component)
        {
            var items = new ArrayList();
            PropertyInfo pi = component.GetType().GetProperty("Styles");
            if (pi != null)
            {
                var styles = pi.GetValue(component, null) as StiStylesCollection;
                if (styles != null) 
                    styles.ToList().ForEach(s => items.Add(StiStylesHelper.StyleItem(s)));
            }
            return items;
        }
        #endregion

        #region DashboardWatermark
        private static Hashtable GetDashboardWatermarkProperty(StiComponent component) {
            var watermark = (component as IStiDashboardWatermark)?.DashboardWatermark;
            if (watermark != null)
            {
                var jsWatermark = new Hashtable()
                {
                    ["textEnabled"] = watermark.TextEnabled,
                    ["text"] = StiEncodingHelper.Encode(watermark.Text),
                    ["textFont"] = FontToStr(watermark.TextFont),
                    ["textColor"] = GetStringFromColor(watermark.TextColor),
                    ["textAngle"] = watermark.TextAngle,

                    ["imageEnabled"] = watermark.ImageEnabled,
                    ["image"] = watermark.Image != null ? ImageToBase64(watermark.Image) : string.Empty,
                    ["imageSize"] = watermark.Image != null ? $"{watermark.Image.Width};{watermark.Image.Height}" : "0;0",
                    ["imageAlignment"] = watermark.ImageAlignment,
                    ["imageTransparency"] = watermark.ImageTransparency,
                    ["imageMultipleFactor"] = DoubleToStr(watermark.ImageMultipleFactor),
                    ["imageAspectRatio"] = watermark.ImageAspectRatio,
                    ["imageStretch"] = watermark.ImageStretch,
                    ["imageTiling"] = watermark.ImageTiling,

                    ["weaveEnabled"] = watermark.WeaveEnabled,
                    ["weaveMajorIcon"] = watermark.WeaveMajorIcon,
                    ["weaveMajorColor"] = GetStringFromColor(watermark.WeaveMajorColor),
                    ["weaveMajorSize"] = watermark.WeaveMajorSize,
                    ["weaveMinorIcon"] = watermark.WeaveMinorIcon,
                    ["weaveMinorColor"] = GetStringFromColor(watermark.WeaveMinorColor),
                    ["weaveMinorSize"] = watermark.WeaveMinorSize,
                    ["weaveDistance"] = watermark.WeaveDistance,
                    ["weaveAngle"] = watermark.WeaveAngle
                };

                if (watermark.WeaveEnabled && (watermark.WeaveMajorIcon != null || watermark.WeaveMinorIcon != null)) {
                    GetWeaveWatermarkImages(watermark, jsWatermark);
                }

                return jsWatermark;
            }
            return null;
        }

        private static void GetWeaveWatermarkImages(StiAdvancedWatermark watermark, Hashtable jsWatermark)
        {
            var majorSize = 5 * watermark.WeaveMajorSize;
            var minorSize = 5 * watermark.WeaveMinorSize;

            if (watermark.WeaveMajorIcon != null)
            {
                var weaveMajorImage = StiFontIconsHelper.ConvertFontIconToImage(watermark.WeaveMajorIcon.Value, watermark.WeaveMajorColor, majorSize, majorSize);
                jsWatermark["weaveMajorImage"] = new Hashtable()
                {
                    ["width"] = weaveMajorImage.Width,
                    ["height"] = weaveMajorImage.Height,
                    ["text"] = StiFontIconsHelper.GetContent(watermark.WeaveMajorIcon),
                    ["size"] = watermark.WeaveMajorSize,
                    ["color"] = GetStringFromColor(watermark.WeaveMajorColor)
                };
            }

            if (watermark.WeaveMinorIcon != null)
            {
                var weaveMinorImage = StiFontIconsHelper.ConvertFontIconToImage(watermark.WeaveMinorIcon.Value, watermark.WeaveMinorColor, minorSize, minorSize);
                jsWatermark["weaveMinorImage"] = new Hashtable()
                {
                    ["width"] = weaveMinorImage.Width,
                    ["height"] = weaveMinorImage.Height,
                    ["text"] = StiFontIconsHelper.GetContent(watermark.WeaveMinorIcon),
                    ["size"] = watermark.WeaveMinorSize,
                    ["color"] = GetStringFromColor(watermark.WeaveMinorColor)
                };
            }
        }
        #endregion

        #region Button IconSet
        private static Hashtable GetButtonIconSetProperty(IStiButtonElementIconSet iconSet)
        {
            if (iconSet != null)
            {
                return new Hashtable()
                {
                    ["icon"] = iconSet.Icon,
                    ["iconText"] = iconSet.Icon != null ? StiFontIconsHelper.GetContent(iconSet.Icon) : "",
                    ["checkedIcon"] = iconSet.CheckedIcon,
                    ["checkedIconText"] = iconSet.CheckedIcon != null ? StiFontIconsHelper.GetContent(iconSet.CheckedIcon) : "",
                    ["uncheckedIcon"] = iconSet.UncheckedIcon,
                    ["uncheckedIconText"] = iconSet.UncheckedIcon != null ? StiFontIconsHelper.GetContent(iconSet.UncheckedIcon) : "",
                };
            }
            return null;
        }
        #endregion

        #region Button VisualStates
        private static Hashtable GetButtonVisualStatesProperty(IStiButtonVisualStates visualStates)
        {
            if (visualStates != null)
            {
                var result = new Hashtable();
                var states = new Hashtable()
                {
                    ["hover"] = visualStates.GetHoverState(),
                    ["pressed"] = visualStates.GetPressedState(),
                    ["check"] = visualStates.GetCheckedState()
                };

                foreach (DictionaryEntry state in states)
                {
                    var visualState = state.Value as IStiButtonVisualState;

                    result[state.Key] = new Hashtable()
                    {
                        ["border"] = SimpleBorderToStr(visualState.Border),
                        ["brush"] = BrushToStr(visualState.Brush),
                        ["font"] = FontToStr(visualState.Font),
                        ["iconBrush"] = BrushToStr(visualState.IconBrush),
                        ["textBrush"] = BrushToStr(visualState.TextBrush),
                        ["iconSet"] = GetButtonIconSetProperty(visualState.GetIconSet())
                    };
                };

                return result;
            }
            return null;
        }
        #endregion

        #region Button Style Colors
        private static Hashtable GetButtonStyleColors(IStiButtonElement buttonElement)
        {
            var styleColors = new Hashtable();
            var controlElementStyle = StiDashboardStyleHelper.GetControlStyle(buttonElement);
            if (controlElementStyle != null)
            {
                styleColors["backColor"] = GetStringFromColor(controlElementStyle.BackColor);
                styleColors["textColor"] = GetStringFromColor(controlElementStyle.ForeColor);
                styleColors["iconColor"] = GetStringFromColor(controlElementStyle.GlyphColor);

                styleColors["hoverBackColor"] = GetStringFromColor(controlElementStyle.HotBackColor);
                styleColors["hoverTextColor"] = GetStringFromColor(controlElementStyle.HotForeColor);
                styleColors["hoverIconColor"] = GetStringFromColor(controlElementStyle.HotGlyphColor);
                
                styleColors["selectedBackColor"] = GetStringFromColor(controlElementStyle.SelectedBackColor);
                styleColors["selectedTextColor"] = GetStringFromColor(controlElementStyle.SelectedForeColor);
                styleColors["selectedIconColor"] = GetStringFromColor(controlElementStyle.SelectedGlyphColor);
            }
            return styleColors;
        }
        #endregion

        #endregion

        #region Set All Properties
        public static void SetAllProperties(StiComponent component, ArrayList props)
        {
            StiReport currReport = component.Report;

            foreach (Hashtable prop in props)
            {
                string propertyName = (string)prop["name"];
                object propertyValue = prop["value"];

                if (propertyName == "ratio") SetPropertyValue(currReport, component is StiZipCode ? "Ratio" : "AspectRatio", component, propertyValue);
                else if (propertyName == "rotation") SetPropertyValue(currReport, "ImageRotation", component, propertyValue);
                else if (propertyName == "imageMultipleFactor") SetPropertyValue(currReport, "MultipleFactor", component, propertyValue);
                else if (propertyName == "subReportPage") SetSubReportPageProperty(component, propertyValue);
                else if (propertyName == "subReportUrl") SetPropertyValue(currReport, "SubReportUrl", component, propertyValue as string != "" ? StiEncodingHelper.DecodeString(propertyValue as string) : null);
                else if (propertyName == "contourColor") SetPropertyValue(currReport, "ContourColor", component, propertyValue);
                else if (propertyName == "size") SetPropertyValue(currReport, "Size", component, propertyValue);
                else if (propertyName == "checkValues") SetPropertyValue(currReport, "Values", component, propertyValue);
                else if (propertyName == "editable") SetPropertyValue(currReport, "Editable", component, propertyValue);
                else if (propertyName == "checkStyleForFalse") SetPropertyValue(currReport, "CheckStyleForFalse", component, propertyValue);
                else if (propertyName == "checkStyleForTrue") SetPropertyValue(currReport, "CheckStyleForTrue", component, propertyValue);
                else if (propertyName == "checked") SetPropertyValue(currReport, "Checked.Value", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "container") SetContainerProperty(component, propertyValue);
                else if (propertyName == "columnWidth") SetPropertyValue(currReport, "ColumnWidth", component, propertyValue);
                else if (propertyName == "columnGaps") SetPropertyValue(currReport, "ColumnGaps", component, propertyValue);
                else if (propertyName == "columns") SetPropertyValue(currReport, "Columns", component, propertyValue);
                else if (propertyName == "columnDirection") SetPropertyValue(currReport, "ColumnDirection", component, propertyValue);
                else if (propertyName == "minRowsInColumn") SetPropertyValue(currReport, "MinRowsInColumn", component, propertyValue);
                else if (propertyName == "shapeBorderColor") SetPropertyValue(currReport, "BorderColor", component, propertyValue);
                else if (propertyName == "shapeBorderStyle") SetPropertyValue(currReport, "Style", component, propertyValue);
                else if (propertyName == "shapeType") SetShapeTypeProperty(component, propertyValue);
                else if (propertyName == "backColor") SetPropertyValue(currReport, "BackColor", component, propertyValue);
                else if (propertyName == "foreColor") SetPropertyValue(currReport, "ForeColor", component, propertyValue);
                else if (propertyName == "autoScale") SetPropertyValue(currReport, "AutoScale", component, propertyValue);
                else if (propertyName == "rightToLeft") SetPropertyValue(currReport, "RightToLeft", component, propertyValue);
                else if (propertyName == "showLabelText") SetPropertyValue(currReport, "ShowLabelText", component, propertyValue);
                else if (propertyName == "showQuietZones") SetPropertyValue(currReport, "ShowQuietZones", component, propertyValue);
                else if (propertyName == "barCodeAngle") SetPropertyValue(currReport, "Angle", component, propertyValue);
                else if (propertyName == "codeType") SetBarCodeTypeProperty(component, propertyValue);
                else if (propertyName == "code") SetPropertyValue(currReport, "Code.Value", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "stretch") SetPropertyValue(currReport, "Stretch", component, propertyValue);
                else if (propertyName == "componentStyle") component.ComponentStyle = propertyValue as string != "[None]" ? propertyValue as string : string.Empty;
                else if (propertyName == "unitWidth") SetPropertyValue(currReport, component is IStiDashboard ? "Width" : "PageWidth", component, propertyValue);
                else if (propertyName == "unitHeight") SetPropertyValue(currReport, component is IStiDashboard ? "Height" : "PageHeight", component, propertyValue);
                else if (propertyName == "orientation") SetPropertyValue(currReport, "Orientation", component, propertyValue);
                else if (propertyName == "unitMargins") SetMarginsProperty(component, propertyValue);
                else if (propertyName == "vertAlignment") SetPropertyValue(currReport, "VertAlignment", component, propertyValue);
                else if (propertyName == "horAlignment") SetPropertyValue(currReport, "HorAlignment", component, propertyValue);
                else if (propertyName == "textAngle") SetPropertyValue(currReport, "Angle", component, propertyValue);
                else if (propertyName == "textMargins") SetPropertyValue(currReport, "Margins", component, propertyValue);
                else if (propertyName == "maxNumberOfLines") SetPropertyValue(currReport, "MaxNumberOfLines", component, propertyValue);
                else if (propertyName == "cellWidth") SetPropertyValue(currReport, "CellWidth", component, propertyValue);
                else if (propertyName == "cellHeight") SetPropertyValue(currReport, "CellHeight", component, propertyValue);
                else if (propertyName == "horizontalSpacing") SetPropertyValue(currReport, "HorSpacing", component, propertyValue);
                else if (propertyName == "verticalSpacing") SetPropertyValue(currReport, "VertSpacing", component, propertyValue);
                else if (propertyName == "wordWrap") SetPropertyValue(currReport, "WordWrap", component, propertyValue);
                else if (propertyName == "font") SetPropertyValue(currReport, "Font", component, propertyValue);
                else if (propertyName == "textBrush") SetPropertyValue(currReport, "TextBrush", component, propertyValue);
                else if (propertyName == "text") SetTextProperty(component, propertyValue);
                else if (propertyName == "textFormat") SetTextFormatProperty(component, propertyValue);
                else if (propertyName == "editableText") SetPropertyValue(currReport, "Editable", component, propertyValue);
                else if (propertyName == "hideZeros") SetPropertyValue(currReport, "HideZeros", component, propertyValue);
                else if (propertyName == "continuousText") SetPropertyValue(currReport, "ContinuousText", component, propertyValue);
                else if (propertyName == "onlyText") SetPropertyValue(currReport, "OnlyText", component, propertyValue);
                else if (propertyName == "allowHtmlTags") SetPropertyValue(currReport, "AllowHtmlTags", component, propertyValue);
                else if (propertyName == "border") SetPropertyValue(currReport, "Border", component, propertyValue);
                else if (propertyName == "brush") SetPropertyValue(currReport, "Brush", component, propertyValue);
                else if (propertyName == "canGrow") SetPropertyValue(currReport, "CanGrow", component, propertyValue);
                else if (propertyName == "canShrink") SetPropertyValue(currReport, "CanShrink", component, propertyValue);
                else if (propertyName == "canBreak") SetPropertyValue(currReport, "CanBreak", component, propertyValue);
                else if (propertyName == "autoWidth") SetPropertyValue(currReport, "AutoWidth", component, propertyValue);
                else if (propertyName == "growToHeight") SetPropertyValue(currReport, "GrowToHeight", component, propertyValue);
                else if (propertyName == "enabled") SetPropertyValue(currReport, "Enabled", component, propertyValue);
                else if (propertyName == "printable") SetPropertyValue(currReport, "Printable", component, propertyValue);
                else if (propertyName == "printOn") SetPropertyValue(currReport, "PrintOn", component, propertyValue);
                else if (propertyName == "paperSize") SetPropertyValue(currReport, "PaperSize", component, propertyValue);
                else if (propertyName == "waterMarkRatio") SetPropertyValue(currReport, "Watermark.AspectRatio", component, propertyValue);
                else if (propertyName == "waterMarkRightToLeft") SetPropertyValue(currReport, "Watermark.RightToLeft", component, propertyValue);
                else if (propertyName == "waterMarkEnabled") SetPropertyValue(currReport, "Watermark.Enabled", component, propertyValue);
                else if (propertyName == "waterMarkEnabledExpression") SetPropertyValue(currReport, "Watermark.EnabledExpression", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "waterMarkAngle") SetPropertyValue(currReport, "Watermark.Angle", component, propertyValue);
                else if (propertyName == "waterMarkText") SetPropertyValue(currReport, "Watermark.Text", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "waterMarkFont") SetPropertyValue(currReport, "Watermark.Font", component, propertyValue);
                else if (propertyName == "waterMarkTextBrush") SetPropertyValue(currReport, "Watermark.TextBrush", component, propertyValue);
                else if (propertyName == "waterMarkTextBehind") SetPropertyValue(currReport, "Watermark.ShowBehind", component, propertyValue);
                else if (propertyName == "waterMarkImageBehind") SetPropertyValue(currReport, "Watermark.ShowImageBehind", component, propertyValue);
                else if (propertyName == "waterMarkImageAlign") SetPropertyValue(currReport, "Watermark.ImageAlignment", component, propertyValue);
                else if (propertyName == "waterMarkMultipleFactor") SetPropertyValue(currReport, "Watermark.ImageMultipleFactor", component, propertyValue);
                else if (propertyName == "waterMarkStretch") SetPropertyValue(currReport, "Watermark.ImageStretch", component, propertyValue);
                else if (propertyName == "waterMarkTiling") SetPropertyValue(currReport, "Watermark.ImageTiling", component, propertyValue);
                else if (propertyName == "waterMarkTransparency") SetPropertyValue(currReport, "Watermark.ImageTransparency", component, propertyValue);
                else if (propertyName == "newPageBefore") SetPropertyValue(currReport, "NewPageBefore", component, propertyValue);
                else if (propertyName == "newPageAfter") SetPropertyValue(currReport, "NewPageAfter", component, propertyValue);
                else if (propertyName == "newColumnBefore") SetPropertyValue(currReport, "NewColumnBefore", component, propertyValue);
                else if (propertyName == "newColumnAfter") SetPropertyValue(currReport, "NewColumnAfter", component, propertyValue);
                else if (propertyName == "skipFirst") SetPropertyValue(currReport, "SkipFirst", component, propertyValue);
                else if (propertyName == "condition") SetConditionProperty(component, propertyValue);
                else if (propertyName == "sortDirection") SetPropertyValue(currReport, "SortDirection", component, propertyValue);
                else if (propertyName == "summarySortDirection") SetPropertyValue(currReport, "SummarySortDirection", component, propertyValue);
                else if (propertyName == "summaryExpression") SetPropertyValue(currReport, "SummaryExpression.Value", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "summaryType") SetPropertyValue(currReport, "SummaryType", component, propertyValue);
                else if (propertyName == "sortData") SetSortDataProperty(component, propertyValue);
                else if (propertyName == "dataSource") SetDataSourceProperty(component, propertyValue);
                else if (propertyName == "dataRelation") SetDataRelationProperty(component, propertyValue);
                else if (propertyName == "masterComponent") SetMasterComponentProperty(component, propertyValue);
                else if (propertyName == "businessObject") SetBusinessObjectProperty(component, propertyValue);
                else if (propertyName == "countData") SetPropertyValue(currReport, "CountData", component, propertyValue);
                else if (propertyName == "filterEngine") SetPropertyValue(currReport, "FilterEngine", component, propertyValue);
                else if (propertyName == "filterData") SetFilterDataProperty(component, propertyValue);
                else if (propertyName == "filterOn") SetFilterOnProperty(component, propertyValue);
                else if (propertyName == "filterMode") SetFilterModeProperty(component, propertyValue);
                else if (propertyName == "imageDataColumn") SetPropertyValue(currReport, "DataColumn", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "imageSrc") SetImageProperty(component, propertyValue as string);
                else if (propertyName == "imageUrl") SetPropertyValue(currReport, "ImageURL.Value", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "imageFile") SetPropertyValue(currReport, "File", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "imageData") SetPropertyValue(currReport, "ImageData.Value", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "watermarkImageSrc") SetWatermarkImageProperty(component as StiPage, propertyValue as string);
                else if (propertyName == "watermarkImageHyperlink") SetPropertyValue(currReport, "Watermark.ImageHyperlink", component, propertyValue);
                else if (propertyName == "stopBeforePrint") SetPropertyValue(currReport, "StopBeforePrint", component, propertyValue);
                else if (propertyName == "titleBeforeHeader") SetPropertyValue(currReport, "TitleBeforeHeader", component, propertyValue);
                else if (propertyName == "crossTabEmptyValue") SetPropertyValue(currReport, "EmptyValue", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "crossTabHorAlign") SetPropertyValue(currReport, "HorAlignment", component, propertyValue);
                else if (propertyName == "printIfEmpty") SetPropertyValue(currReport, "PrintIfEmpty", component, propertyValue);
                else if (propertyName == "printOnAllPages") SetPropertyValue(currReport, "PrintOnAllPages", component, propertyValue);
                else if (propertyName == "crossTabWrap") SetPropertyValue(currReport, "Wrap", component, propertyValue);
                else if (propertyName == "crossTabWrapGap") SetPropertyValue(currReport, "WrapGap", component, propertyValue);
                else if (propertyName == "crossTabPrintTitle") SetPropertyValue(currReport, "PrintTitleOnAllPages", component, propertyValue);
                else if (propertyName == "keyDataColumn") SetPropertyValue(currReport, "KeyDataColumn", component, propertyValue);
                else if (propertyName == "masterKeyDataColumn") SetPropertyValue(currReport, "MasterKeyDataColumn", component, propertyValue);
                else if (propertyName == "parentValue") SetPropertyValue(currReport, "ParentValue", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "indent") SetPropertyValue(currReport, "Indent", component, propertyValue);
                else if (propertyName == "headers") SetPropertyValue(currReport, "Headers", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "footers") SetPropertyValue(currReport, "Footers", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "aliasName") component.Alias = StiEncodingHelper.DecodeString(propertyValue as string);
                else if (propertyName == "oddStyle") SetPropertyValue(currReport, "OddStyle", component, (propertyValue as string != "[None]") ? propertyValue as string : string.Empty);
                else if (propertyName == "evenStyle") SetPropertyValue(currReport, "EvenStyle", component, (propertyValue as string != "[None]") ? propertyValue as string : string.Empty);
                else if (propertyName == "largeHeight") SetPropertyValue(currReport, "LargeHeight", component, propertyValue);
                else if (propertyName == "largeHeightFactor") SetPropertyValue(currReport, "LargeHeightFactor", component, propertyValue);
                else if (propertyName == "calcInvisible") SetPropertyValue(currReport, "CalcInvisible", component, propertyValue);
                else if (propertyName == "richText") SetRichTextProperty(component, propertyValue);
                else if (propertyName == "richTextUrl") SetPropertyValue(currReport, "DataUrl.Value", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "richTextDataColumn") SetPropertyValue(currReport, "DataColumn", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "restrictions") SetRestrictionsProperty(component, propertyValue);
                else if (propertyName == "conditions") SetConditionsProperty(component, propertyValue, currReport);
                else if (propertyName == "trimming") SetPropertyValue(currReport, "TextOptions.Trimming", component, propertyValue);
                else if (propertyName == "textOptionsRightToLeft") SetPropertyValue(currReport, "TextOptions.RightToLeft", component, propertyValue);
                else if (propertyName == "processAt") SetPropertyValue(currReport, "ProcessAt", component, propertyValue);
                else if (propertyName == "processingDuplicates") SetPropertyValue(currReport, "ProcessingDuplicates", component, propertyValue);
                else if (propertyName == "shrinkFontToFit") SetPropertyValue(currReport, "ShrinkFontToFit", component, propertyValue);
                else if (propertyName == "shrinkFontToFitMinimumSize") SetPropertyValue(currReport, "ShrinkFontToFitMinimumSize", component, propertyValue);
                else if (propertyName == "textType") SetPropertyValue(currReport, "Type", component, propertyValue);
                else if (propertyName == "resetPageNumber") SetPropertyValue(currReport, "ResetPageNumber", component, propertyValue);
                else if (propertyName == "printOnPreviousPage") SetPropertyValue(currReport, "PrintOnPreviousPage", component, propertyValue);
                else if (propertyName == "printHeadersFootersFromPreviousPage") SetPropertyValue(currReport, "PrintHeadersFootersFromPreviousPage", component, propertyValue);
                else if (propertyName == "printAtBottom") SetPropertyValue(currReport, "PrintAtBottom", component, propertyValue);
                else if (propertyName == "printIfDetailEmpty") SetPropertyValue(currReport, "PrintIfDetailEmpty", component, propertyValue);
                else if (propertyName == "keepGroupTogether") SetPropertyValue(currReport, "KeepGroupTogether", component, propertyValue);
                else if (propertyName == "keepGroupHeaderTogether") SetPropertyValue(currReport, "KeepGroupHeaderTogether", component, propertyValue);
                else if (propertyName == "keepGroupFooterTogether") SetPropertyValue(currReport, "KeepGroupFooterTogether", component, propertyValue);
                else if (propertyName == "keepHeaderTogether") SetPropertyValue(currReport, "KeepHeaderTogether", component, propertyValue);
                else if (propertyName == "keepFooterTogether") SetPropertyValue(currReport, "KeepFooterTogether", component, propertyValue);
                else if (propertyName == "keepDetailsTogether") SetPropertyValue(currReport, "KeepDetailsTogether", component, propertyValue);
                else if (propertyName == "keepDetails") SetPropertyValue(currReport, "KeepDetails", component, propertyValue);
                else if (propertyName == "keepReportSummaryTogether") SetPropertyValue(currReport, "KeepReportSummaryTogether", component, propertyValue);
                else if (propertyName == "keepSubReportTogether") SetPropertyValue(currReport, "KeepSubReportTogether", component, propertyValue);
                else if (propertyName == "keepCrossTabTogether") SetPropertyValue(currReport, "KeepCrossTabTogether", component, propertyValue);
                else if (propertyName == "interaction") SetInteractionProperty(component, propertyValue);
                else if (propertyName == "chartStyle") SetChartStyleProperty(component, propertyValue);
                else if (propertyName == "gaugeStyle") SetGaugeStyleProperty(component, propertyValue);
                else if (propertyName == "mapStyle") SetMapStyleProperty(component, propertyValue);
                else if (propertyName == "crossTabStyle") SetCrossTabStyleProperty(component, propertyValue);
                else if (propertyName == "shiftMode") SetShiftModeProperty(component, propertyValue);
                else if (propertyName == "cellDockStyle") SetPropertyValue(currReport, "CellDockStyle", component, propertyValue);
                else if (propertyName == "fixedWidth") SetPropertyValue(currReport, "FixedWidth", component, propertyValue);
                else if (propertyName == "tableAutoWidth") SetPropertyValue(currReport, "AutoWidth", component, propertyValue);
                else if (propertyName == "autoWidthType") SetPropertyValue(currReport, "AutoWidthType", component, propertyValue);
                else if (propertyName == "headerRowsCount") SetPropertyValue(currReport, "HeaderRowsCount", component, propertyValue);
                else if (propertyName == "footerRowsCount") SetPropertyValue(currReport, "FooterRowsCount", component, propertyValue);
                else if (propertyName == "tableRightToLeft") SetPropertyValue(currReport, "RightToLeft", component, propertyValue);
                else if (propertyName == "dockableTable") SetPropertyValue(currReport, "DockableTable", component, propertyValue);
                else if (propertyName == "headerPrintOn") SetPropertyValue(currReport, "HeaderPrintOn", component, propertyValue);
                else if (propertyName == "headerCanGrow") SetPropertyValue(currReport, "HeaderCanGrow", component, propertyValue);
                else if (propertyName == "headerCanShrink") SetPropertyValue(currReport, "HeaderCanShrink", component, propertyValue);
                else if (propertyName == "headerCanBreak") SetPropertyValue(currReport, "HeaderCanBreak", component, propertyValue);
                else if (propertyName == "headerPrintAtBottom") SetPropertyValue(currReport, "HeaderPrintAtBottom", component, propertyValue);
                else if (propertyName == "headerPrintIfEmpty") SetPropertyValue(currReport, "HeaderPrintIfEmpty", component, propertyValue);
                else if (propertyName == "headerPrintOnAllPages") SetPropertyValue(currReport, "HeaderPrintOnAllPages", component, propertyValue);
                else if (propertyName == "headerPrintOnEvenOddPages") SetPropertyValue(currReport, "HeaderPrintOnEvenOddPages", component, propertyValue);
                else if (propertyName == "footerPrintOn") SetPropertyValue(currReport, "FooterPrintOn", component, propertyValue);
                else if (propertyName == "footerCanGrow") SetPropertyValue(currReport, "FooterCanGrow", component, propertyValue);
                else if (propertyName == "footerCanShrink") SetPropertyValue(currReport, "FooterCanShrink", component, propertyValue);
                else if (propertyName == "footerCanBreak") SetPropertyValue(currReport, "FooterCanBreak", component, propertyValue);
                else if (propertyName == "footerPrintAtBottom") SetPropertyValue(currReport, "FooterPrintAtBottom", component, propertyValue);
                else if (propertyName == "footerPrintIfEmpty") SetPropertyValue(currReport, "FooterPrintIfEmpty", component, propertyValue);
                else if (propertyName == "footerPrintOnAllPages") SetPropertyValue(currReport, "FooterPrintOnAllPages", component, propertyValue);
                else if (propertyName == "footerPrintOnEvenOddPages") SetPropertyValue(currReport, "FooterPrintOnEvenOddPages", component, propertyValue);
                else if (propertyName == "locked") SetPropertyValue(currReport, "Locked", component, propertyValue);
                else if (propertyName == "linked") SetPropertyValue(currReport, "Linked", component, propertyValue);
                else if (propertyName == "dockStyle") SetPropertyValue(currReport, "DockStyle", component, propertyValue);
                else if (propertyName == "mirrorMargins") SetPropertyValue(currReport, "MirrorMargins", component, propertyValue);
                else if (propertyName == "subReportParameters") SetSubReportParametersProperty(component, propertyValue);
                else if (propertyName == "segmentPerWidth") SetPropertyValue(currReport, "SegmentPerWidth", component, propertyValue);
                else if (propertyName == "segmentPerHeight") SetPropertyValue(currReport, "SegmentPerHeight", component, propertyValue);
                else if (propertyName == "anchor") SetAnchorProperty(component, propertyValue);
                else if (propertyName == "minSize") SetPropertyValue(currReport, "MinSize", component, propertyValue);
                else if (propertyName == "maxSize") SetPropertyValue(currReport, "MaxSize", component, propertyValue);
                else if (propertyName == "minHeight") SetPropertyValue(currReport, "MinHeight", component, propertyValue);
                else if (propertyName == "maxHeight") SetPropertyValue(currReport, "MaxHeight", component, propertyValue);
                else if (propertyName == "minWidth") SetPropertyValue(currReport, "MinWidth", component, propertyValue);
                else if (propertyName == "maxWidth") SetPropertyValue(currReport, "MaxWidth", component, propertyValue);
                else if (propertyName == "color") SetPropertyValue(currReport, "Color", component, propertyValue);
                else if (propertyName == "style") SetPropertyValue(currReport, "Style", component, propertyValue);
                else if (propertyName == "round") SetPropertyValue(currReport, "Round", component, propertyValue);
                else if (propertyName == "leftSide") SetPropertyValue(currReport, "LeftSide", component, propertyValue);
                else if (propertyName == "rightSide") SetPropertyValue(currReport, "RightSide", component, propertyValue);
                else if (propertyName == "topSide") SetPropertyValue(currReport, "TopSide", component, propertyValue);
                else if (propertyName == "bottomSide") SetPropertyValue(currReport, "BottomSide", component, propertyValue);
                else if (propertyName == "startCapColor") SetPropertyValue(currReport, "StartCap.Color", component, propertyValue);
                else if (propertyName == "startCapFill") SetPropertyValue(currReport, "StartCap.Fill", component, propertyValue);
                else if (propertyName == "startCapWidth") SetPropertyValue(currReport, "StartCap.Width", component, propertyValue);
                else if (propertyName == "startCapHeight") SetPropertyValue(currReport, "StartCap.Height", component, propertyValue);
                else if (propertyName == "startCapStyle") SetPropertyValue(currReport, "StartCap.Style", component, propertyValue);
                else if (propertyName == "useParentStyles") SetPropertyValue(currReport, "UseParentStyles", component, propertyValue);
                else if (propertyName == "unlimitedHeight") SetPropertyValue(currReport, "UnlimitedHeight", component, propertyValue);
                else if (propertyName == "unlimitedBreakable") SetPropertyValue(currReport, "UnlimitedBreakable", component, propertyValue);
                else if (propertyName == "numberOfCopies") SetPropertyValue(currReport, "NumberOfCopies", component, propertyValue);
                else if (propertyName == "breakIfLessThan") SetPropertyValue(currReport, "BreakIfLessThan", component, propertyValue);
                else if (propertyName == "renderTo") SetPropertyValue(currReport, "RenderTo", component, propertyValue);
                else if (propertyName == "globalizedName") SetPropertyValue(currReport, "GlobalizedName", component, propertyValue);
                else if (propertyName == "excelValue") SetExcelValueProperty(component, propertyValue);
                else if (propertyName == "exportAsImage") SetPropertyValue(currReport, "ExportAsImage", component, propertyValue);
                else if (propertyName == "lineSpacing") SetPropertyValue(currReport, "LineSpacing", component, propertyValue);
                else if (propertyName == "elementStyle") SetPropertyValue(currReport, "Style", component, propertyValue);
                else if (propertyName == "customStyleName") SetPropertyValue(currReport, "CustomStyleName", component, propertyValue);
                else if (propertyName == "detectUrls") SetPropertyValue(currReport, "DetectUrls", component, propertyValue);
                else if (propertyName == "margin") SetPropertyValue(currReport, "Margin", component, propertyValue);
                else if (propertyName == "padding") SetPropertyValue(currReport, "Padding", component, propertyValue);
                else if (propertyName == "titleText") SetPropertyValue(currReport, "Title.Text", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "titleBackColor") SetPropertyValue(currReport, "Title.BackColor", component, propertyValue);
                else if (propertyName == "titleForeColor") SetPropertyValue(currReport, "Title.ForeColor", component, propertyValue);
                else if (propertyName == "titleFont") SetPropertyValue(currReport, "Title.Font", component, propertyValue);
                else if (propertyName == "titleHorAlignment") SetPropertyValue(currReport, "Title.HorAlignment", component, propertyValue);
                else if (propertyName == "titleVisible") SetPropertyValue(currReport, "Title.Visible", component, propertyValue);
                else if (propertyName == "titleSizeMode") SetPropertyValue(currReport, "Title.SizeMode", component, propertyValue);
                else if (propertyName == "sizeMode") SetPropertyValue(currReport, "SizeMode", component, propertyValue);
                else if (propertyName == "headerForeColor") SetPropertyValue(currReport, "HeaderForeColor", component, propertyValue);
                else if (propertyName == "headerFont") SetPropertyValue(currReport, "HeaderFont", component, propertyValue);
                else if (propertyName == "footerForeColor") SetPropertyValue(currReport, "FooterForeColor", component, propertyValue);
                else if (propertyName == "footerFont") SetPropertyValue(currReport, "FooterFont", component, propertyValue);
                else if (propertyName == "excelSheet") SetExcelSheetProperty(component, propertyValue);
                else if (propertyName == "valueFormat") SetTextFormatProperty(component, propertyValue, "ValueFormat");
                else if (propertyName == "argumentFormat") SetTextFormatProperty(component, propertyValue, "ArgumentFormat");
                else if (propertyName == "colorEach") SetPropertyValue(currReport, "ColorEach", component, propertyValue);
                else if (propertyName == "showValue") SetPropertyValue(currReport, "ShowValue", component, propertyValue);
                else if (propertyName == "shortValue") SetPropertyValue(currReport, "ShortValue", component, propertyValue);
                else if (propertyName == "showZeros") SetPropertyValue(currReport, "ShowZeros", component, propertyValue);
                else if (propertyName == "displayNameType") SetPropertyValue(currReport, "DisplayNameType", component, propertyValue);
                else if (propertyName == "group") SetPropertyValue(currReport, "Group", component, propertyValue);
                else if (propertyName == "glyphColor") SetPropertyValue(currReport, "GlyphColor", component, propertyValue);
                else if (propertyName == "mode") SetPropertyValue(currReport, "Mode", component, propertyValue);
                else if (propertyName == "icon") SetPropertyValue(currReport, "Icon", component, propertyValue);
                else if (propertyName == "iconSet") SetPropertyValue(currReport, "IconSet", component, propertyValue);
                else if (propertyName == "iconColor") SetPropertyValue(currReport, "IconColor", component, propertyValue);
                else if (propertyName == "type") SetPropertyValue(currReport, "Type", component, propertyValue);
                else if (propertyName == "calculationMode") SetPropertyValue(currReport, "CalculationMode", component, propertyValue);
                else if (propertyName == "minimum") SetPropertyValue(currReport, "Minimum", component, propertyValue);
                else if (propertyName == "maximum") SetPropertyValue(currReport, "Maximum", component, propertyValue);
                else if (propertyName == "rangeType") SetPropertyValue(currReport, "RangeType", component, propertyValue);
                else if (propertyName == "rangeMode") SetPropertyValue(currReport, "RangeMode", component, propertyValue);
                else if (propertyName == "selectionMode") SetPropertyValue(currReport, "SelectionMode", component, propertyValue);
                else if (propertyName == "initialRangeSelection") SetPropertyValue(currReport, "InitialRangeSelection", component, propertyValue);
                else if (propertyName == "showAllValue") SetPropertyValue(currReport, "ShowAllValue", component, propertyValue);
                else if (propertyName == "conditionDatePicker") SetPropertyValue(currReport, "Condition", component, propertyValue);
                else if (propertyName == "seriesColors") SetPropertyValue(currReport, "SeriesColors", component, propertyValue);
                else if (propertyName == "negativeSeriesColors") SetPropertyValue(currReport, "NegativeSeriesColors", component, propertyValue);
                else if (propertyName == "paretoSeriesColors") SetPropertyValue(currReport, "ParetoSeriesColors", component, propertyValue);
                else if (propertyName == "areaAllowApplyStyle") SetPropertyValue(currReport, "Area.AllowApplyStyle", component, propertyValue);
                else if (propertyName == "areaColorEach") SetPropertyValue(currReport, "Area.ColorEach", component, propertyValue);
                else if (propertyName == "areaBrush") SetPropertyValue(currReport, "Area.Brush", component, propertyValue);
                else if (propertyName == "areaBorderColor") SetPropertyValue(currReport, "Area.BorderColor", component, propertyValue);
                else if (propertyName == "areaBorderThickness") SetPropertyValue(currReport, "Area.BorderThickness", component, propertyValue);
                else if (propertyName == "areaReverseHor") SetPropertyValue(currReport, "Area.ReverseHor", component, propertyValue);
                else if (propertyName == "areaReverseVert") SetPropertyValue(currReport, "Area.ReverseVert", component, propertyValue);
                else if (propertyName == "areaSideBySide") SetPropertyValue(currReport, "Area.SideBySide", component, propertyValue);
                else if (propertyName == "areaGridLinesHorAllowApplyStyle") SetPropertyValue(currReport, "Area.GridLinesHor.AllowApplyStyle", component, propertyValue);
                else if (propertyName == "areaGridLinesHorColor") SetPropertyValue(currReport, "Area.GridLinesHor.Color", component, propertyValue);
                else if (propertyName == "areaGridLinesHorMinorColor") SetPropertyValue(currReport, "Area.GridLinesHor.MinorColor", component, propertyValue);
                else if (propertyName == "areaGridLinesHorMinorCount") SetPropertyValue(currReport, "Area.GridLinesHor.MinorCount", component, propertyValue);
                else if (propertyName == "areaGridLinesHorMinorStyle") SetPropertyValue(currReport, "Area.GridLinesHor.MinorStyle", component, propertyValue);
                else if (propertyName == "areaGridLinesHorMinorVisible") SetPropertyValue(currReport, "Area.GridLinesHor.MinorVisible", component, propertyValue);
                else if (propertyName == "areaGridLinesHorStyle") SetPropertyValue(currReport, "Area.GridLinesHor.Style", component, propertyValue);
                else if (propertyName == "areaGridLinesHorVisible") SetPropertyValue(currReport, "Area.GridLinesHor.Visible", component, propertyValue);
                else if (propertyName == "areaGridLinesVertAllowApplyStyle") SetPropertyValue(currReport, "Area.GridLinesVert.AllowApplyStyle", component, propertyValue);
                else if (propertyName == "areaGridLinesVertColor") SetPropertyValue(currReport, "Area.GridLinesVert.Color", component, propertyValue);
                else if (propertyName == "areaGridLinesVertMinorColor") SetPropertyValue(currReport, "Area.GridLinesVert.MinorColor", component, propertyValue);
                else if (propertyName == "areaGridLinesVertMinorCount") SetPropertyValue(currReport, "Area.GridLinesVert.MinorCount", component, propertyValue);
                else if (propertyName == "areaGridLinesVertMinorStyle") SetPropertyValue(currReport, "Area.GridLinesVert.MinorStyle", component, propertyValue);
                else if (propertyName == "areaGridLinesVertMinorVisible") SetPropertyValue(currReport, "Area.GridLinesVert.MinorVisible", component, propertyValue);
                else if (propertyName == "areaGridLinesVertStyle") SetPropertyValue(currReport, "Area.GridLinesVert.Style", component, propertyValue);
                else if (propertyName == "areaGridLinesVertVisible") SetPropertyValue(currReport, "Area.GridLinesVert.Visible", component, propertyValue);
                else if (propertyName == "areaInterlacingHorAllowApplyStyle") SetPropertyValue(currReport, "Area.InterlacingHor.AllowApplyStyle", component, propertyValue);
                else if (propertyName == "areaInterlacingHorColor") SetPropertyValue(currReport, "Area.InterlacingHor.Color", component, propertyValue);
                else if (propertyName == "areaInterlacingHorInterlacedBrush") SetPropertyValue(currReport, "Area.InterlacingHor.InterlacedBrush", component, propertyValue);
                else if (propertyName == "areaInterlacingHorVisible") SetPropertyValue(currReport, "Area.InterlacingHor.Visible", component, propertyValue);
                else if (propertyName == "areaInterlacingVertAllowApplyStyle") SetPropertyValue(currReport, "Area.InterlacingVert.AllowApplyStyle", component, propertyValue);
                else if (propertyName == "areaInterlacingVertColor") SetPropertyValue(currReport, "Area.InterlacingVert.Color", component, propertyValue);
                else if (propertyName == "areaInterlacingVertInterlacedBrush") SetPropertyValue(currReport, "Area.InterlacingVert.InterlacedBrush", component, propertyValue);
                else if (propertyName == "areaInterlacingVertVisible") SetPropertyValue(currReport, "Area.InterlacingVert.Visible", component, propertyValue);
                else if (propertyName == "labelsLabelsType") StiChartHelper.SetLabelsType(component, propertyValue);
                else if (propertyName == "labelsAllowApplyStyle") SetPropertyValue(currReport, "Labels.AllowApplyStyle", component, propertyValue);
                else if (propertyName == "labelsAngle") SetPropertyValue(currReport, "Labels.Angle", component, propertyValue);
                else if (propertyName == "labelsAntialiasing") SetPropertyValue(currReport, "Labels.Antialiasing", component, propertyValue);
                else if (propertyName == "labelsDrawBorder") SetPropertyValue(currReport, "Labels.DrawBorder", component, propertyValue);
                else if (propertyName == "labelsBorderColor") SetPropertyValue(currReport, "Labels.BorderColor", component, propertyValue);
                else if (propertyName == "labelsFormat") SetPropertyValue(currReport, "Labels.Format", component, propertyValue);
                else if (propertyName == "labelsLegendValueType") SetPropertyValue(currReport, "Labels.LegendValueType", component, propertyValue);
                else if (propertyName == "labelsLineLength") SetPropertyValue(currReport, "Labels.LineLength", component, propertyValue);
                else if (propertyName == "labelsMarkerAlignment") SetPropertyValue(currReport, "Labels.MarkerAlignment", component, propertyValue);
                else if (propertyName == "labelsMarkerSize") SetPropertyValue(currReport, "Labels.MarkerSize", component, propertyValue);
                else if (propertyName == "labelsMarkerVisible") SetPropertyValue(currReport, "Labels.MarkerVisible", component, propertyValue);
                else if (propertyName == "labelsPreventIntersection") SetPropertyValue(currReport, "Labels.PreventIntersection", component, propertyValue);
                else if (propertyName == "labelsShowInPercent") SetPropertyValue(currReport, "Labels.ShowInPercent", component, propertyValue);
                else if (propertyName == "labelsShowNulls") SetPropertyValue(currReport, "Labels.ShowNulls", component, propertyValue);
                else if (propertyName == "labelsShowZeros") SetPropertyValue(currReport, "Labels.ShowZeros", component, propertyValue);
                else if (propertyName == "labelsStep") SetPropertyValue(currReport, "Labels.Step", component, propertyValue);
                else if (propertyName == "labelsUseSeriesColor") SetPropertyValue(currReport, "Labels.UseSeriesColor", component, propertyValue);
                else if (propertyName == "labelsValueType") SetPropertyValue(currReport, "Labels.ValueType", component, propertyValue);
                else if (propertyName == "labelsValueTypeSeparator") SetPropertyValue(currReport, "Labels.ValueTypeSeparator", component, propertyValue);
                else if (propertyName == "labelsVisible") SetPropertyValue(currReport, "Labels.Visible", component, propertyValue);
                else if (propertyName == "labelsWidth") SetPropertyValue(currReport, "Labels.Width", component, propertyValue);
                else if (propertyName == "labelsWordWrap") SetPropertyValue(currReport, "Labels.WordWrap", component, propertyValue);
                else if (propertyName == "labelsFont") SetPropertyValue(currReport, "Labels.Font", component, propertyValue);
                else if (propertyName == "labelsBrush") SetPropertyValue(currReport, "Labels.Brush", component, propertyValue);
                else if (propertyName == "labelsLabelColor") SetPropertyValue(currReport, "Labels.LabelColor", component, propertyValue);
                else if (propertyName == "labelsBorderColor") SetPropertyValue(currReport, "Labels.BorderColor", component, propertyValue);
                else if (propertyName == "labelsForeColor") SetPropertyValue(currReport, "Labels.ForeColor", component, propertyValue);
                else if (propertyName == "labelsAutoRotate") SetPropertyValue(currReport, "Labels.AutoRotate", component, propertyValue);
                else if (propertyName == "labelsPosition") SetPropertyValue(currReport, "Labels.Position", component, propertyValue);
                else if (propertyName == "labelsStyle") SetPropertyValue(currReport, "Labels.Style", component, propertyValue);
                else if (propertyName == "labelsTextAfter") SetPropertyValue(currReport, "Labels.TextAfter", component, propertyValue);
                else if (propertyName == "labelsTextBefore") SetPropertyValue(currReport, "Labels.TextBefore", component, propertyValue);
                else if (propertyName == "legendAllowApplyStyle") SetPropertyValue(currReport, "Legend.AllowApplyStyle", component, propertyValue);
                else if (propertyName == "legendBorderColor") SetPropertyValue(currReport, "Legend.BorderColor", component, propertyValue);
                else if (propertyName == "legendBrush") SetPropertyValue(currReport, "Legend.Brush", component, propertyValue);
                else if (propertyName == "legendFont") SetPropertyValue(currReport, "Legend.Font", component, propertyValue);
                else if (propertyName == "legendHorAlignment") SetPropertyValue(currReport, "Legend.HorAlignment", component, propertyValue);
                else if (propertyName == "legendVertAlignment") SetPropertyValue(currReport, "Legend.VertAlignment", component, propertyValue);
                else if (propertyName == "legendColumns") SetPropertyValue(currReport, "Legend.Columns", component, propertyValue);
                else if (propertyName == "legendDirection") SetPropertyValue(currReport, "Legend.Direction", component, propertyValue);
                else if (propertyName == "legendVisible") SetPropertyValue(currReport, "Legend.Visible", component, propertyValue);
                else if (propertyName == "legendVisibility") SetPropertyValue(currReport, "Legend.Visibility", component, propertyValue);
                else if (propertyName == "legendLabelsValueType") SetPropertyValue(currReport, "Legend.Labels.ValueType", component, propertyValue);
                else if (propertyName == "legendLabelsColor") SetPropertyValue(currReport, component is StiChart ? "Legend.LabelsColor" : "Legend.Labels.Color", component, propertyValue);
                else if (propertyName == "legendLabelsFont") SetPropertyValue(currReport, component is StiChart ? "Legend.Font" : "Legend.Labels.Font", component, propertyValue);
                else if (propertyName == "legendTitleColor") SetPropertyValue(currReport, component is StiChart ? "Legend.TitleColor" : "Legend.Title.Color", component, propertyValue);
                else if (propertyName == "legendTitleFont") SetPropertyValue(currReport, component is StiChart ? "Legend.TitleFont" : "Legend.Title.Font", component, propertyValue);
                else if (propertyName == "legendTitleText") SetPropertyValue(currReport, component is StiChart ? "Legend.Title" : "Legend.Title.Text", component, propertyValue);
                else if (propertyName == "xAxisVisible") SetPropertyValue(currReport, "Area.XAxis.Visible", component, propertyValue);
                else if (propertyName == "xAxisLabelsAngle") SetPropertyValue(currReport, "Area.XAxis.Labels.Angle", component, propertyValue);
                else if (propertyName == "xAxisLabelsColor") SetPropertyValue(currReport, "Area.XAxis.Labels.Color", component, propertyValue);
                else if (propertyName == "xAxisLabelsFont") SetPropertyValue(currReport, "Area.XAxis.Labels.Font", component, propertyValue);
                else if (propertyName == "xAxisLabelsPlacement") SetPropertyValue(currReport, "Area.XAxis.Labels.Placement", component, propertyValue);
                else if (propertyName == "xAxisLabelsTextAfter") SetPropertyValue(currReport, "Area.XAxis.Labels.TextAfter", component, propertyValue);
                else if (propertyName == "xAxisLabelsTextAlignment") SetPropertyValue(currReport, "Area.XAxis.Labels.TextAlignment", component, propertyValue);
                else if (propertyName == "xAxisLabelsTextBefore") SetPropertyValue(currReport, "Area.XAxis.Labels.TextBefore", component, propertyValue);
                else if (propertyName == "xAxisLabelsStep") SetPropertyValue(currReport, "Area.XAxis.Labels.Step", component, propertyValue);
                else if (propertyName == "xAxisTitleAlignment") SetPropertyValue(currReport, "Area.XAxis.Title.Alignment", component, propertyValue);
                else if (propertyName == "xAxisTitleColor") SetPropertyValue(currReport, "Area.XAxis.Title.Color", component, propertyValue);
                else if (propertyName == "xAxisTitleDirection") SetPropertyValue(currReport, "Area.XAxis.Title.Direction", component, propertyValue);
                else if (propertyName == "xAxisTitleFont") SetPropertyValue(currReport, "Area.XAxis.Title.Font", component, propertyValue);
                else if (propertyName == "xAxisTitlePosition") SetPropertyValue(currReport, "Area.XAxis.Title.Position", component, propertyValue);
                else if (propertyName == "xAxisTitleText") SetPropertyValue(currReport, "Area.XAxis.Title.Text", component, propertyValue);
                else if (propertyName == "xAxisTitleVisible") SetPropertyValue(currReport, "Area.XAxis.Title.Visible", component, propertyValue);
                else if (propertyName == "xAxisRangeAuto") SetPropertyValue(currReport, "Area.XAxis.Range.Auto", component, propertyValue);
                else if (propertyName == "xAxisRangeMinimum") SetPropertyValue(currReport, "Area.XAxis.Range.Minimum", component, propertyValue);
                else if (propertyName == "xAxisRangeMaximum") SetPropertyValue(currReport, "Area.XAxis.Range.Maximum", component, propertyValue);
                else if (propertyName == "xAxisShowEdgeValues") SetPropertyValue(currReport, "Area.XAxis.ShowEdgeValues", component, component is IStiChartElement ? propertyValue : propertyValue?.ToString() == "True");
                else if (propertyName == "xAxisStartFromZero") SetPropertyValue(currReport, "Area.XAxis.StartFromZero", component, component is IStiChartElement ? propertyValue : propertyValue?.ToString() == "True");
                else if (propertyName == "xTopAxisVisible") SetPropertyValue(currReport, "Area.XTopAxis.Visible", component, propertyValue);
                else if (propertyName == "xTopAxisLabelsAngle") SetPropertyValue(currReport, "Area.XTopAxis.Labels.Angle", component, propertyValue);
                else if (propertyName == "xTopAxisLabelsColor") SetPropertyValue(currReport, "Area.XTopAxis.Labels.Color", component, propertyValue);
                else if (propertyName == "xTopAxisLabelsFont") SetPropertyValue(currReport, "Area.XTopAxis.Labels.Font", component, propertyValue);
                else if (propertyName == "xTopAxisLabelsPlacement") SetPropertyValue(currReport, "Area.XTopAxis.Labels.Placement", component, propertyValue);
                else if (propertyName == "xTopAxisLabelsTextAfter") SetPropertyValue(currReport, "Area.XTopAxis.Labels.TextAfter", component, propertyValue);
                else if (propertyName == "xTopAxisLabelsTextAlignment") SetPropertyValue(currReport, "Area.XTopAxis.Labels.TextAlignment", component, propertyValue);
                else if (propertyName == "xTopAxisLabelsTextBefore") SetPropertyValue(currReport, "Area.XTopAxis.Labels.TextBefore", component, propertyValue);
                else if (propertyName == "xTopAxisLabelsStep") SetPropertyValue(currReport, "Area.XTopAxis.Labels.Step", component, propertyValue);
                else if (propertyName == "xTopAxisTitleAlignment") SetPropertyValue(currReport, "Area.XTopAxis.Title.Alignment", component, propertyValue);
                else if (propertyName == "xTopAxisTitleColor") SetPropertyValue(currReport, "Area.XTopAxis.Title.Color", component, propertyValue);
                else if (propertyName == "xTopAxisTitleDirection") SetPropertyValue(currReport, "Area.XTopAxis.Title.Direction", component, propertyValue);
                else if (propertyName == "xTopAxisTitleFont") SetPropertyValue(currReport, "Area.XTopAxis.Title.Font", component, propertyValue);
                else if (propertyName == "xTopAxisTitlePosition") SetPropertyValue(currReport, "Area.XTopAxis.Title.Position", component, propertyValue);
                else if (propertyName == "xTopAxisTitleText") SetPropertyValue(currReport, "Area.XTopAxis.Title.Text", component, propertyValue);
                else if (propertyName == "xTopAxisTitleVisible") SetPropertyValue(currReport, "Area.XTopAxis.Title.Visible", component, propertyValue);
                else if (propertyName == "xTopAxisShowEdgeValues") SetPropertyValue(currReport, "Area.XTopAxis.ShowEdgeValues", component, component is IStiChartElement ? propertyValue : propertyValue?.ToString() == "True");
                else if (propertyName == "yAxisVisible") SetPropertyValue(currReport, "Area.YAxis.Visible", component, propertyValue);
                else if (propertyName == "yAxisLabelsAngle") SetPropertyValue(currReport, "Area.YAxis.Labels.Angle", component, propertyValue);
                else if (propertyName == "yAxisLabelsColor") SetPropertyValue(currReport, "Area.YAxis.Labels.Color", component, propertyValue);
                else if (propertyName == "yAxisLabelsFont") SetPropertyValue(currReport, "Area.YAxis.Labels.Font", component, propertyValue);
                else if (propertyName == "yAxisLabelsPlacement") SetPropertyValue(currReport, "Area.YAxis.Labels.Placement", component, propertyValue);
                else if (propertyName == "yAxisLabelsTextAfter") SetPropertyValue(currReport, "Area.YAxis.Labels.TextAfter", component, propertyValue);
                else if (propertyName == "yAxisLabelsTextAlignment") SetPropertyValue(currReport, "Area.YAxis.Labels.TextAlignment", component, propertyValue);
                else if (propertyName == "yAxisLabelsTextBefore") SetPropertyValue(currReport, "Area.YAxis.Labels.TextBefore", component, propertyValue);
                else if (propertyName == "yAxisLabelsStep") SetPropertyValue(currReport, "Area.YAxis.Labels.Step", component, propertyValue);
                else if (propertyName == "yAxisTitleAlignment") SetPropertyValue(currReport, "Area.YAxis.Title.Alignment", component, propertyValue);
                else if (propertyName == "yAxisTitleColor") SetPropertyValue(currReport, "Area.YAxis.Title.Color", component, propertyValue);
                else if (propertyName == "yAxisTitleDirection") SetPropertyValue(currReport, "Area.YAxis.Title.Direction", component, propertyValue);
                else if (propertyName == "yAxisTitleFont") SetPropertyValue(currReport, "Area.YAxis.Title.Font", component, propertyValue);
                else if (propertyName == "yAxisTitlePosition") SetPropertyValue(currReport, "Area.YAxis.Title.Position", component, propertyValue);
                else if (propertyName == "yAxisTitleText") SetPropertyValue(currReport, "Area.YAxis.Title.Text", component, propertyValue);
                else if (propertyName == "yAxisTitleVisible") SetPropertyValue(currReport, "Area.YAxis.Title.Visible", component, propertyValue);
                else if (propertyName == "yAxisRangeAuto") SetPropertyValue(currReport, "Area.YAxis.Range.Auto", component, propertyValue);
                else if (propertyName == "yAxisRangeMinimum") SetPropertyValue(currReport, "Area.YAxis.Range.Minimum", component, propertyValue);
                else if (propertyName == "yAxisRangeMaximum") SetPropertyValue(currReport, "Area.YAxis.Range.Maximum", component, propertyValue);
                else if (propertyName == "yAxisStartFromZero") SetPropertyValue(currReport, "Area.YAxis.StartFromZero", component, propertyValue);
                else if (propertyName == "yRightAxisVisible") SetPropertyValue(currReport, "Area.YRightAxis.Visible", component, propertyValue);
                else if (propertyName == "yRightAxisLabelsAngle") SetPropertyValue(currReport, "Area.YRightAxis.Labels.Angle", component, propertyValue);
                else if (propertyName == "yRightAxisLabelsColor") SetPropertyValue(currReport, "Area.YRightAxis.Labels.Color", component, propertyValue);
                else if (propertyName == "yRightAxisLabelsFont") SetPropertyValue(currReport, "Area.YRightAxis.Labels.Font", component, propertyValue);
                else if (propertyName == "yRightAxisLabelsPlacement") SetPropertyValue(currReport, "Area.YRightAxis.Labels.Placement", component, propertyValue);
                else if (propertyName == "yRightAxisLabelsTextAfter") SetPropertyValue(currReport, "Area.YRightAxis.Labels.TextAfter", component, propertyValue);
                else if (propertyName == "yRightAxisLabelsTextAlignment") SetPropertyValue(currReport, "Area.YRightAxis.Labels.TextAlignment", component, propertyValue);
                else if (propertyName == "yRightAxisLabelsTextBefore") SetPropertyValue(currReport, "Area.YRightAxis.Labels.TextBefore", component, propertyValue);
                else if (propertyName == "yRightAxisTitleAlignment") SetPropertyValue(currReport, "Area.YRightAxis.Title.Alignment", component, propertyValue);
                else if (propertyName == "yRightAxisTitleColor") SetPropertyValue(currReport, "Area.YRightAxis.Title.Color", component, propertyValue);
                else if (propertyName == "yRightAxisTitleDirection") SetPropertyValue(currReport, "Area.YRightAxis.Title.Direction", component, propertyValue);
                else if (propertyName == "yRightAxisTitleFont") SetPropertyValue(currReport, "Area.YRightAxis.Title.Font", component, propertyValue);
                else if (propertyName == "yRightAxisTitlePosition") SetPropertyValue(currReport, "Area.YRightAxis.Title.Position", component, propertyValue);
                else if (propertyName == "yRightAxisTitleText") SetPropertyValue(currReport, "Area.YRightAxis.Title.Text", component, propertyValue);
                else if (propertyName == "yRightAxisTitleVisible") SetPropertyValue(currReport, "Area.YRightAxis.Title.Visible", component, propertyValue);
                else if (propertyName == "yRightAxisStartFromZero") SetPropertyValue(currReport, "Area.YRightAxis.StartFromZero", component, propertyValue);
                else if (propertyName == "markerAngle") SetPropertyValue(currReport, "Marker.Angle", component, propertyValue);
                else if (propertyName == "markerSize") SetPropertyValue(currReport, "Marker.Size", component, propertyValue);
                else if (propertyName == "markerType") SetPropertyValue(currReport, "Marker.Type", component, propertyValue);
                else if (propertyName == "markerVisible") SetPropertyValue(currReport, "Marker.Visible", component, propertyValue);
                else if (propertyName == "options3DDistance") SetPropertyValue(currReport, "Options3D.Distance", component, propertyValue);
                else if (propertyName == "options3DHeight") SetPropertyValue(currReport, "Options3D.Height", component, propertyValue);
                else if (propertyName == "options3DLighting") SetPropertyValue(currReport, "Options3D.Lighting", component, propertyValue);
                else if (propertyName == "options3DOpacity") SetPropertyValue(currReport, "Options3D.Opacity", component, propertyValue);
                else if (propertyName == "chartTitleAllowApplyStyle") SetPropertyValue(currReport, "Title.AllowApplyStyle", component, propertyValue);
                else if (propertyName == "chartTitleAlignment") SetPropertyValue(currReport, "Title.Alignment", component, propertyValue);
                else if (propertyName == "chartTitleAntialiasing") SetPropertyValue(currReport, "Title.Antialiasing", component, propertyValue);
                else if (propertyName == "chartTitleBrush") SetPropertyValue(currReport, "Title.Brush", component, propertyValue);
                else if (propertyName == "chartTitleDock") SetPropertyValue(currReport, "Title.Dock", component, propertyValue);
                else if (propertyName == "chartTitleFont") SetPropertyValue(currReport, "Title.Font", component, propertyValue);
                else if (propertyName == "chartTitleSpacing") SetPropertyValue(currReport, "Title.Spacing", component, propertyValue);
                else if (propertyName == "chartTitleText") SetPropertyValue(currReport, "Title.Text", component, propertyValue);
                else if (propertyName == "chartTitleVisible") SetPropertyValue(currReport, "Title.Visible", component, propertyValue);
                else if (propertyName == "chartTableAllowApplyStyle") SetPropertyValue(currReport, "Table.AllowApplyStyle", component, propertyValue);
                else if (propertyName == "chartTableGridLineColor") SetPropertyValue(currReport, "Table.GridLineColor", component, propertyValue);
                else if (propertyName == "chartTableGridLinesHor") SetPropertyValue(currReport, "Table.GridLinesHor", component, propertyValue);
                else if (propertyName == "chartTableGridLinesVert") SetPropertyValue(currReport, "Table.GridLinesVert", component, propertyValue);
                else if (propertyName == "chartTableGridOutline") SetPropertyValue(currReport, "Table.GridOutline", component, propertyValue);
                else if (propertyName == "chartTableMarkerVisible") SetPropertyValue(currReport, "Table.MarkerVisible", component, propertyValue);
                else if (propertyName == "chartTableVisible") SetPropertyValue(currReport, "Table.Visible", component, propertyValue);
                else if (propertyName == "chartTableDataCellsTextColor") SetPropertyValue(currReport, "Table.DataCells.TextColor", component, propertyValue);
                else if (propertyName == "chartTableDataCellsFont") SetPropertyValue(currReport, "Table.DataCells.Font", component, propertyValue);
                else if (propertyName == "chartTableDataCellsShrinkFontToFit") SetPropertyValue(currReport, "Table.DataCells.ShrinkFontToFit", component, propertyValue);
                else if (propertyName == "chartTableDataCellsShrinkFontToFitMinimumSize") SetPropertyValue(currReport, "Table.DataCells.ShrinkFontToFitMinimumSize", component, propertyValue);
                else if (propertyName == "chartTableHeaderTextAfter") SetPropertyValue(currReport, "Table.Header.TextAfter", component, propertyValue);
                else if (propertyName == "chartTableHeaderTextColor") SetPropertyValue(currReport, "Table.Header.TextColor", component, propertyValue);
                else if (propertyName == "chartTableHeaderWordWrap") SetPropertyValue(currReport, "Table.Header.WordWrap", component, propertyValue);
                else if (propertyName == "chartTableHeaderBrush") SetPropertyValue(currReport, "Table.Header.Brush", component, propertyValue);
                else if (propertyName == "chartTableHeaderFont") SetPropertyValue(currReport, "Table.Header.Font", component, propertyValue);
                else if (propertyName == "topN") SetTopNProperty(component, propertyValue);
                else if (propertyName == "dashboardInteraction") SetDashboardInteractionProperty((component as IStiElementInteraction).DashboardInteraction, propertyValue);
                else if (propertyName == "customIcon") SetPropertyValue(currReport, "CustomIcon", component, propertyValue);
                else if (propertyName == "chartConditions") SetChartConditionsProperty(component, propertyValue);
                else if (propertyName == "chartTrendLines") SetChartTrendLinesProperty(component, propertyValue);
                else if (propertyName == "pivotTableConditions") SetPivotTableConditionsProperty(component, propertyValue);
                else if (propertyName == "limitRows") SetPropertyValue(currReport, "LimitRows", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "iconAlignment") SetPropertyValue(currReport, "IconAlignment", component, propertyValue);
                else if (propertyName == "indicatorConditions") SetIndicatorConditionsProperty(component, propertyValue);
                else if (propertyName == "progressConditions") SetProgressConditionsProperty(component, propertyValue);
                else if (propertyName == "tableConditions") SetTableConditionsProperty(component, propertyValue);
                else if (propertyName == "deviceWidth") SetPropertyValue(currReport, "DeviceWidth", component, propertyValue);
                else if (propertyName == "targetMode") SetPropertyValue(currReport, "TargetMode", component, propertyValue);
                else if (propertyName == "fontSizeMode") SetPropertyValue(currReport, "FontSizeMode", component, propertyValue);
                else if (propertyName == "editorType") SetPropertyValue(currReport, "EditorType", component, propertyValue);
                else if (propertyName == "processAtEnd") SetPropertyValue(currReport, "ProcessAtEnd", component, propertyValue);
                else if (propertyName == "chartRotation") SetPropertyValue(currReport, "Rotation", component, propertyValue);
                else if (propertyName == "allowApplyStyle") SetPropertyValue(currReport, "AllowApplyStyle", component, propertyValue);
                else if (propertyName == "valueDataColumn") SetPropertyValue(currReport, "ValueDataColumn", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "labelsVisible") SetPropertyValue(currReport, "Labels.Visible", component, propertyValue);
                else if (propertyName == "labelsPlacement") SetPropertyValue(currReport, "Labels.Placement", component, propertyValue);
                else if (propertyName == "expressions") SetExpressionsProperty(component, propertyValue);
                else if (propertyName == "spaceRatio") SetPropertyValue(currReport, "SpaceRatio", component, propertyValue);
                else if (propertyName == "upperMarks") SetPropertyValue(currReport, "UpperMarks", component, propertyValue);
                else if (propertyName == "linesOfUnderline") SetPropertyValue(currReport, "LinesOfUnderline", component, propertyValue);
                else if (propertyName == "imageMargins") SetPropertyValue(currReport, "Margins", component, propertyValue);
                else if (propertyName == "imageSmoothing") SetPropertyValue(currReport, "Smoothing", component, propertyValue);
                else if (propertyName == "imageProcessingDuplicates") SetPropertyValue(currReport, "ProcessingDuplicates", component, propertyValue);
                else if (propertyName == "nullValue") SetPropertyValue(currReport, "NullValue", component, propertyValue);
                else if (propertyName == "crossFiltering") SetPropertyValue(currReport, "CrossFiltering", component, propertyValue);
                else if (propertyName == "tableOfContentsIndent") SetPropertyValue(currReport, "Indent", component, propertyValue);
                else if (propertyName == "tableOfContentsMargins") SetPropertyValue(currReport, "Margins", component, propertyValue);
                else if (propertyName == "tableOfContentsNewPageBefore") SetPropertyValue(currReport, "NewPageBefore", component, propertyValue);
                else if (propertyName == "tableOfContentsNewPageAfter") SetPropertyValue(currReport, "NewPageAfter", component, propertyValue);
                else if (propertyName == "tableOfContentsRightToLeft") SetPropertyValue(currReport, "RightToLeft", component, propertyValue);
                else if (propertyName == "tableOfContentsStyles") SetPropertyValue(currReport, "Styles", component, propertyValue);
                else if (propertyName == "printOnEvenOddPages") SetPropertyValue(currReport, "PrintOnEvenOddPages", component, propertyValue);
                else if (propertyName == "laTexExpression") SetPropertyValue(currReport, "LaTexExpression", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "frozenColumns") SetPropertyValue(currReport, "FrozenColumns", component, propertyValue);
                else if (propertyName == "pageIcon") SetPropertyValue(currReport, "Icon", component, propertyValue);
                else if (propertyName == "shapeText") SetPropertyValue(currReport, "Text", component, !string.IsNullOrEmpty(propertyValue as string) ? StiEncodingHelper.DecodeString(propertyValue as string) : null);
                else if (propertyName == "backgroundColor") SetPropertyValue(currReport, "BackgroundColor", component, propertyValue);
                else if (propertyName == "shadowVisible") SetPropertyValue(currReport, "Shadow.Visible", component, propertyValue);
                else if (propertyName == "shadowColor") SetPropertyValue(currReport, "Shadow.Color", component, propertyValue);
                else if (propertyName == "shadowLocation") SetPropertyValue(currReport, "Shadow.Location", component, propertyValue);
                else if (propertyName == "shadowSize") SetPropertyValue(currReport, "Shadow.Size", component, propertyValue);
                else if (propertyName == "cornerRadius") SetPropertyValue(currReport, "CornerRadius", component, propertyValue);
                else if (propertyName == "dashboardWatermark") SetDashboardWatermarkProperty(component, propertyValue);
                else if (propertyName == "iconBrush") SetPropertyValue(currReport, "IconBrush", component, propertyValue);
                else if (propertyName == "buttonIconSetIcon") SetPropertyValue(currReport, "IconSet.Icon", component, propertyValue);
                else if (propertyName == "buttonIconSetCheckedIcon") SetPropertyValue(currReport, "IconSet.CheckedIcon", component, propertyValue);
                else if (propertyName == "buttonIconSetUncheckedIcon") SetPropertyValue(currReport, "IconSet.UncheckedIcon", component, propertyValue);
                else if (propertyName == "buttonText") SetPropertyValue(currReport, "Text", component, !string.IsNullOrEmpty(propertyValue as string) ? StiEncodingHelper.DecodeString(propertyValue as string) : "");
                else if (propertyName == "buttonType") SetPropertyValue(currReport, "Type", component, propertyValue);
                else if (propertyName == "buttonChecked") SetPropertyValue(currReport, "Checked", component, propertyValue);
                else if (propertyName == "buttonGroup") SetPropertyValue(currReport, "Group", component, propertyValue);
                else if (propertyName == "buttonShapeType") SetPropertyValue(currReport, "ShapeType", component, propertyValue);
                else if (propertyName == "buttonStretch") SetPropertyValue(currReport, "Stretch", component, propertyValue);
                else if (propertyName == "visualStatesCheckBorder") SetPropertyValue(currReport, "VisualStates.Checked.Border", component, propertyValue);
                else if (propertyName == "visualStatesCheckBrush") SetPropertyValue(currReport, "VisualStates.Checked.Brush", component, propertyValue);
                else if (propertyName == "visualStatesCheckFont") SetPropertyValue(currReport, "VisualStates.Checked.Font", component, propertyValue);
                else if (propertyName == "visualStatesCheckIconBrush") SetPropertyValue(currReport, "VisualStates.Checked.IconBrush", component, propertyValue);
                else if (propertyName == "visualStatesCheckTextBrush") SetPropertyValue(currReport, "VisualStates.Checked.TextBrush", component, propertyValue);
                else if (propertyName == "visualStatesCheckIconSetIcon") SetPropertyValue(currReport, "VisualStates.Checked.IconSet.Icon", component, propertyValue);
                else if (propertyName == "visualStatesCheckIconSetCheckedIcon") SetPropertyValue(currReport, "VisualStates.Checked.IconSet.CheckedIcon", component, propertyValue);
                else if (propertyName == "visualStatesCheckIconSetUncheckedIcon") SetPropertyValue(currReport, "VisualStates.Checked.IconSet.UncheckedIcon", component, propertyValue);
                else if (propertyName == "visualStatesHoverBorder") SetPropertyValue(currReport, "VisualStates.Hover.Border", component, propertyValue);
                else if (propertyName == "visualStatesHoverBrush") SetPropertyValue(currReport, "VisualStates.Hover.Brush", component, propertyValue);
                else if (propertyName == "visualStatesHoverFont") SetPropertyValue(currReport, "VisualStates.Hover.Font", component, propertyValue);
                else if (propertyName == "visualStatesHoverIconBrush") SetPropertyValue(currReport, "VisualStates.Hover.IconBrush", component, propertyValue);
                else if (propertyName == "visualStatesHoverTextBrush") SetPropertyValue(currReport, "VisualStates.Hover.TextBrush", component, propertyValue);
                else if (propertyName == "visualStatesHoverIconSetIcon") SetPropertyValue(currReport, "VisualStates.Hover.IconSet.Icon", component, propertyValue);
                else if (propertyName == "visualStatesHoverIconSetCheckedIcon") SetPropertyValue(currReport, "VisualStates.Hover.IconSet.CheckedIcon", component, propertyValue);
                else if (propertyName == "visualStatesHoverIconSetUncheckedIcon") SetPropertyValue(currReport, "VisualStates.Hover.IconSet.UncheckedIcon", component, propertyValue);
                else if (propertyName == "visualStatesPressedBorder") SetPropertyValue(currReport, "VisualStates.Pressed.Border", component, propertyValue);
                else if (propertyName == "visualStatesPressedBrush") SetPropertyValue(currReport, "VisualStates.Pressed.Brush", component, propertyValue);
                else if (propertyName == "visualStatesPressedFont") SetPropertyValue(currReport, "VisualStates.Pressed.Font", component, propertyValue);
                else if (propertyName == "visualStatesPressedIconBrush") SetPropertyValue(currReport, "VisualStates.Pressed.IconBrush", component, propertyValue);
                else if (propertyName == "visualStatesPressedTextBrush") SetPropertyValue(currReport, "VisualStates.Pressed.TextBrush", component, propertyValue);
                else if (propertyName == "visualStatesPressedIconSetIcon") SetPropertyValue(currReport, "VisualStates.Pressed.IconSet.Icon", component, propertyValue);
                else if (propertyName == "visualStatesPressedIconSetCheckedIcon") SetPropertyValue(currReport, "VisualStates.Pressed.IconSet.CheckedIcon", component, propertyValue);
                else if (propertyName == "visualStatesPressedIconSetUncheckedIcon") SetPropertyValue(currReport, "VisualStates.Pressed.IconSet.UncheckedIcon", component, propertyValue);
                else if (propertyName == "cardsCornerRadius") SetPropertyValue(currReport, "Cards.CornerRadius", component, propertyValue);
                else if (propertyName == "cardsMargin") SetPropertyValue(currReport, "Cards.Margin", component, propertyValue);
                else if (propertyName == "cardsPadding") SetPropertyValue(currReport, "Cards.Padding", component, propertyValue);
                else if (propertyName == "cardsColorEach") SetPropertyValue(currReport, "Cards.ColorEach", component, propertyValue);
                else if (propertyName == "columnCount") SetPropertyValue(currReport, "ColumnCount", component, propertyValue);
                else if (propertyName == "textSizeMode") SetPropertyValue(currReport, "SizeMode", component, propertyValue);
                else if (propertyName == "targetSettingsShowLabel") SetPropertyValue(currReport, "TargetSettings.ShowLabel", component, propertyValue);
                else if (propertyName == "targetSettingsPlacement") SetPropertyValue(currReport, "TargetSettings.Placement", component, propertyValue);
                else if (propertyName == "signatureMode") SetPropertyValue(currReport, "Mode", component, propertyValue);
                else if (propertyName == "typeFullName") SetPropertyValue(currReport, "Type.FullName", component, propertyValue);
                else if (propertyName == "typeInitials") SetPropertyValue(currReport, "Type.Initials", component, propertyValue);
                else if (propertyName == "typeStyle") SetPropertyValue(currReport, "Type.Style", component, propertyValue);
                else if (propertyName == "drawImage") SetPropertyValue(currReport, "Image.Image", component, propertyValue);
                else if (propertyName == "drawImageHorAlignment") SetPropertyValue(currReport, "Image.HorAlignment", component, propertyValue);
                else if (propertyName == "drawImageVertAlignment") SetPropertyValue(currReport, "Image.VertAlignment", component, propertyValue);
                else if (propertyName == "drawImageStretch") SetPropertyValue(currReport, "Image.Stretch", component, propertyValue);
                else if (propertyName == "drawImageAspectRatio") SetPropertyValue(currReport, "Image.AspectRatio", component, propertyValue);
                else if (propertyName == "drawText") SetPropertyValue(currReport, "Text.Text", component, !string.IsNullOrEmpty(propertyValue as string) ? StiEncodingHelper.DecodeString(propertyValue as string) : "");
                else if (propertyName == "drawTextFont") SetPropertyValue(currReport, "Text.Font", component, propertyValue);
                else if (propertyName == "drawTextColor") SetPropertyValue(currReport, "Text.Color", component, propertyValue);
                else if (propertyName == "drawTextHorAlignment") SetPropertyValue(currReport, "Text.HorAlignment", component, propertyValue);
                else if (propertyName == "placeholder") SetPropertyValue(currReport, "Placeholder", component, StiEncodingHelper.DecodeString(propertyValue as string));
                else if (propertyName == "multipleInitialization") SetPropertyValue(currReport, "MultipleInitialization", component, propertyValue);
            }
        }
        #endregion

        #region Set Property 
        #region SubReportPage
        public static void SetSubReportPageProperty(object component, object propertyValue)
        {
            StiSubReport comp = (StiSubReport)component;
            StiReport report = comp.Report;
            if (propertyValue as string != "[Not Assigned]")
                comp.SubReportPage = report.Pages[propertyValue as string];
            else
                comp.SubReportPage = null;
        }
        #endregion

        #region Container
        public static void SetContainerProperty(object component, object propertyValue)
        {
            StiClone cloneComp = (StiClone)component;
            cloneComp.Container = propertyValue as string != "[Not Assigned]" ? (StiContainer)cloneComp.Report.Pages.GetComponentByName(propertyValue as string) : null;
        }
        #endregion       

        #region ShapeType
        public static void SetShapeTypeProperty(object component, object propValue)
        {
            var comp = component as StiShape;
            if (comp == null) return;
            string shapeType = propValue as string;

            if (shapeType.StartsWith("StiArrowShapeType"))
            {
                StiArrowShapeType arrowShapeType = new StiArrowShapeType();
                switch (shapeType)
                {
                    case "StiArrowShapeTypeUp": arrowShapeType.Direction = StiShapeDirection.Up; break;
                    case "StiArrowShapeTypeDown": arrowShapeType.Direction = StiShapeDirection.Down; break;
                    case "StiArrowShapeTypeRight": arrowShapeType.Direction = StiShapeDirection.Right; break;
                    case "StiArrowShapeTypeLeft": arrowShapeType.Direction = StiShapeDirection.Left; break;
                }
                comp.ShapeType = arrowShapeType;
            }
            else if (shapeType == "StiDiagonalDownLineShapeType") comp.ShapeType = new StiDiagonalDownLineShapeType();
            else if (shapeType == "StiDiagonalUpLineShapeType") comp.ShapeType = new StiDiagonalUpLineShapeType();
            else if (shapeType == "StiHorizontalLineShapeType") comp.ShapeType = new StiHorizontalLineShapeType();
            else if (shapeType == "StiLeftAndRightLineShapeType") comp.ShapeType = new StiLeftAndRightLineShapeType();
            else if (shapeType == "StiOvalShapeType") comp.ShapeType = new StiOvalShapeType();
            else if (shapeType == "StiRectangleShapeType") comp.ShapeType = new StiRectangleShapeType();
            else if (shapeType == "StiRoundedRectangleShapeType") comp.ShapeType = new StiRoundedRectangleShapeType();
            else if (shapeType == "StiTopAndBottomLineShapeType") comp.ShapeType = new StiTopAndBottomLineShapeType();
            else if (shapeType == "StiTriangleShapeType") comp.ShapeType = new StiTriangleShapeType();
            else if (shapeType == "StiVerticalLineShapeType") comp.ShapeType = new StiVerticalLineShapeType();
            else if (shapeType == "StiComplexArrowShapeType") comp.ShapeType = new StiComplexArrowShapeType();
            else if (shapeType == "StiBentArrowShapeType") comp.ShapeType = new StiBentArrowShapeType();
            else if (shapeType == "StiChevronShapeType") comp.ShapeType = new StiChevronShapeType();
            else if (shapeType == "StiDivisionShapeType") comp.ShapeType = new StiDivisionShapeType();
            else if (shapeType == "StiEqualShapeType") comp.ShapeType = new StiEqualShapeType();
            else if (shapeType == "StiFlowchartCardShapeType") comp.ShapeType = new StiFlowchartCardShapeType();
            else if (shapeType == "StiFlowchartCollateShapeType") comp.ShapeType = new StiFlowchartCollateShapeType();
            else if (shapeType == "StiFlowchartDecisionShapeType") comp.ShapeType = new StiFlowchartDecisionShapeType();
            else if (shapeType == "StiFlowchartManualInputShapeType") comp.ShapeType = new StiFlowchartManualInputShapeType();
            else if (shapeType == "StiFlowchartOffPageConnectorShapeType") comp.ShapeType = new StiFlowchartOffPageConnectorShapeType();
            else if (shapeType == "StiFlowchartPreparationShapeType") comp.ShapeType = new StiFlowchartPreparationShapeType();
            else if (shapeType == "StiFlowchartSortShapeType") comp.ShapeType = new StiFlowchartSortShapeType();
            else if (shapeType == "StiFrameShapeType") comp.ShapeType = new StiFrameShapeType();
            else if (shapeType == "StiMinusShapeType") comp.ShapeType = new StiMinusShapeType();
            else if (shapeType == "StiMultiplyShapeType") comp.ShapeType = new StiMultiplyShapeType();
            else if (shapeType == "StiParallelogramShapeType") comp.ShapeType = new StiParallelogramShapeType();
            else if (shapeType == "StiPlusShapeType") comp.ShapeType = new StiPlusShapeType();
            else if (shapeType == "StiRegularPentagonShapeType") comp.ShapeType = new StiRegularPentagonShapeType();
            else if (shapeType == "StiTrapezoidShapeType") comp.ShapeType = new StiTrapezoidShapeType();
            else if (shapeType == "StiOctagonShapeType") comp.ShapeType = new StiOctagonShapeType();
            else if (shapeType == "StiSnipSameSideCornerRectangleShapeType") comp.ShapeType = new StiSnipSameSideCornerRectangleShapeType();
            else if (shapeType == "StiSnipDiagonalSideCornerRectangleShapeType") comp.ShapeType = new StiSnipDiagonalSideCornerRectangleShapeType();
        }
        #endregion

        #region BarCodeType
        public static void SetBarCodeTypeProperty(object component, object propValue)
        {
            StiBarCode comp = (StiBarCode)component;
            string barCodeType = propValue as string;
            if (barCodeType == "StiAustraliaPost4StateBarCodeType") comp.BarCodeType = new StiAustraliaPost4StateBarCodeType();
            else if (barCodeType == "StiCode11BarCodeType") comp.BarCodeType = new StiCode11BarCodeType();
            else if (barCodeType == "StiCode128aBarCodeType") comp.BarCodeType = new StiCode128aBarCodeType();
            else if (barCodeType == "StiCode128bBarCodeType") comp.BarCodeType = new StiCode128bBarCodeType();
            else if (barCodeType == "StiCode128cBarCodeType") comp.BarCodeType = new StiCode128cBarCodeType();
            else if (barCodeType == "StiCode128AutoBarCodeType") comp.BarCodeType = new StiCode128AutoBarCodeType();
            else if (barCodeType == "StiCode39BarCodeType") comp.BarCodeType = new StiCode39BarCodeType();
            else if (barCodeType == "StiCode39ExtBarCodeType") comp.BarCodeType = new StiCode39ExtBarCodeType();
            else if (barCodeType == "StiCode93BarCodeType") comp.BarCodeType = new StiCode93BarCodeType();
            else if (barCodeType == "StiCode93ExtBarCodeType") comp.BarCodeType = new StiCode93ExtBarCodeType();
            else if (barCodeType == "StiCodabarBarCodeType") comp.BarCodeType = new StiCodabarBarCodeType();
            else if (barCodeType == "StiDataMatrixBarCodeType") comp.BarCodeType = new StiDataMatrixBarCodeType();
            else if (barCodeType == "StiEAN128aBarCodeType") comp.BarCodeType = new StiEAN128aBarCodeType();
            else if (barCodeType == "StiEAN128bBarCodeType") comp.BarCodeType = new StiEAN128bBarCodeType();
            else if (barCodeType == "StiEAN128cBarCodeType") comp.BarCodeType = new StiEAN128cBarCodeType();
            else if (barCodeType == "StiEAN128AutoBarCodeType") comp.BarCodeType = new StiEAN128AutoBarCodeType();
            else if (barCodeType == "StiEAN13BarCodeType") comp.BarCodeType = new StiEAN13BarCodeType();
            else if (barCodeType == "StiEAN8BarCodeType") comp.BarCodeType = new StiEAN8BarCodeType();
            else if (barCodeType == "StiGS1_128BarCodeType") comp.BarCodeType = new StiGS1_128BarCodeType();
            else if (barCodeType == "StiGS1DataMatrixBarCodeType") comp.BarCodeType = new StiGS1DataMatrixBarCodeType();
            else if (barCodeType == "StiGS1QRCodeBarCodeType") comp.BarCodeType = new StiGS1QRCodeBarCodeType();
            else if (barCodeType == "StiFIMBarCodeType") comp.BarCodeType = new StiFIMBarCodeType();
            else if (barCodeType == "StiIsbn10BarCodeType") comp.BarCodeType = new StiIsbn10BarCodeType();
            else if (barCodeType == "StiIsbn13BarCodeType") comp.BarCodeType = new StiIsbn13BarCodeType();
            else if (barCodeType == "StiITF14BarCodeType") comp.BarCodeType = new StiITF14BarCodeType();
            else if (barCodeType == "StiJan13BarCodeType") comp.BarCodeType = new StiJan13BarCodeType();
            else if (barCodeType == "StiJan8BarCodeType") comp.BarCodeType = new StiJan8BarCodeType();
            else if (barCodeType == "StiMsiBarCodeType") comp.BarCodeType = new StiMsiBarCodeType();
            else if (barCodeType == "StiPdf417BarCodeType") comp.BarCodeType = new StiPdf417BarCodeType();
            else if (barCodeType == "StiPharmacodeBarCodeType") comp.BarCodeType = new StiPharmacodeBarCodeType();
            else if (barCodeType == "StiMaxicodeBarCodeType") comp.BarCodeType = new StiMaxicodeBarCodeType();
            else if (barCodeType == "StiPlesseyBarCodeType") comp.BarCodeType = new StiPlesseyBarCodeType();
            else if (barCodeType == "StiPostnetBarCodeType") comp.BarCodeType = new StiPostnetBarCodeType();
            else if (barCodeType == "StiQRCodeBarCodeType") comp.BarCodeType = new StiQRCodeBarCodeType();
            else if (barCodeType == "StiDutchKIXBarCodeType") comp.BarCodeType = new StiDutchKIXBarCodeType();
            else if (barCodeType == "StiRoyalMail4StateBarCodeType") comp.BarCodeType = new StiRoyalMail4StateBarCodeType();
            else if (barCodeType == "StiSSCC18BarCodeType") comp.BarCodeType = new StiSSCC18BarCodeType();
            else if (barCodeType == "StiUpcABarCodeType") comp.BarCodeType = new StiUpcABarCodeType();
            else if (barCodeType == "StiUpcEBarCodeType") comp.BarCodeType = new StiUpcEBarCodeType();
            else if (barCodeType == "StiUpcSup2BarCodeType") comp.BarCodeType = new StiUpcSup2BarCodeType();
            else if (barCodeType == "StiUpcSup5BarCodeType") comp.BarCodeType = new StiUpcSup5BarCodeType();
            else if (barCodeType == "StiInterleaved2of5BarCodeType") comp.BarCodeType = new StiInterleaved2of5BarCodeType();
            else if (barCodeType == "StiStandard2of5BarCodeType") comp.BarCodeType = new StiStandard2of5BarCodeType();
            else if (barCodeType == "StiAztecBarCodeType") comp.BarCodeType = new StiAztecBarCodeType();
            else if (barCodeType == "StiIntelligentMail4StateBarCodeType") comp.BarCodeType = new StiIntelligentMail4StateBarCodeType();
        }
        #endregion      

        #region PageMargins
        public static void SetMarginsProperty(object component, object propertyValue)
        {
            StiPage comp = (StiPage)component;
            string[] margins = ((string)propertyValue).Split('!');

            comp.Margins.Left = StrToDouble(margins[0]);
            comp.Margins.Top = StrToDouble(margins[1]);
            comp.Margins.Right = StrToDouble(margins[2]);
            comp.Margins.Bottom = StrToDouble(margins[3]);
        }
        #endregion

        #region Text
        public static void SetTextProperty(object component, object propertyValue)
        {
            propertyValue = StiEncodingHelper.DecodeString(propertyValue as string).Replace("\n", "\r\n");
            StiExpression text = new StiExpression(propertyValue as string);
            PropertyInfo pi = component.GetType().GetProperty("Text");

            if (pi != null) pi.SetValue(component, text, null);
        }
        #endregion

        #region RichText
        public static void SetRichTextProperty(object component, object propValue)
        {
            string propertyValue = StiEncodingHelper.DecodeString(propValue as string);
            PropertyInfo pi = component.GetType().GetProperty("RtfText");
            if (pi != null)
            {
                if (!StiHyperlinkProcessor.IsServerHyperlink(propertyValue))
                {
                    HtmlToRtfConverter htmlToRtfConverter = new HtmlToRtfConverter();
                    string richText = htmlToRtfConverter.ConvertHtmlToRtf(propertyValue);
                    pi.SetValue(component, richText, null);
                }
                else
                {
                    pi.SetValue(component, string.Empty, null);
                    StiExpression text = new StiExpression(propertyValue);
                    PropertyInfo pi2 = component.GetType().GetProperty("Text");
                    if (pi2 != null) pi2.SetValue(component, text, null);
                }
            }
        }
        #endregion

        #region DataURL
        public static void SetRichTextDataUrlProperty(object component, string propertyValue)
        {
            StiRichText comp = (StiRichText)component;
            string dataUrl = StiEncodingHelper.DecodeString(propertyValue);
            StiDataUrlExpression url = new StiDataUrlExpression();
            url.Value = dataUrl;
            comp.DataUrl = url;
        }
        #endregion

        #region TextFormat
        public static void SetTextFormatProperty(object component, object propertyValue, string propertyName = null)
        {
            PropertyInfo pi = component.GetType().GetProperty(propertyName != null ? propertyName : "TextFormat");
            if (pi != null) pi.SetValue(component, StiTextFormatHelper.GetFormatService((Hashtable)propertyValue), null);
        }
        #endregion         

        #region Condition
        public static void SetConditionProperty(object component, object propValue)
        {
            string propertyValue = StiEncodingHelper.DecodeString(propValue as string);
            StiGroupHeaderBand comp = (StiGroupHeaderBand)component;
            StiGroupConditionExpression condExpr = new StiGroupConditionExpression(propertyValue);
            comp.Condition = condExpr;
        }
        private static string GetCorrectName(string value)
        {
            string result = string.Empty;
            if (value == "") return "";

            if (value.StartsWith("{") && value.EndsWith("}"))
            {
                value = value.Substring(1, value.Length - 2);

                string[] valueArray = value.Split('.');
                for (int i = 0; i < valueArray.Length; i++)
                {
                    if (result != "") result += ".";
                    result += StiNameValidator.CorrectName(valueArray[i]);
                }

                return "{" + result + "}";
            }
            return value;
        }
        #endregion       

        #region DataSource
        public static void SetDataSourceProperty(object component, object propValue)
        {
            StiComponent comp = component as StiComponent;
            string propertyValue = propValue as string == "[Not Assigned]" ? string.Empty : propValue as string;
            PropertyInfo pi = component.GetType().GetProperty("DataSourceName");
            if (pi != null)
            {
                string oldDataSourceName = (string)pi.GetValue(component, null);
                if (oldDataSourceName != propertyValue)
                    SetSortDataProperty(component, string.Empty);
                pi.SetValue(component, propertyValue, null);
            }
        }
        #endregion

        #region DataRelation
        public static void SetDataRelationProperty(object component, object propValue)
        {
            string propertyValue = propValue as string == "[Not Assigned]" ? null : propValue as string;
            PropertyInfo pi = component.GetType().GetProperty("DataRelationName");
            if (pi != null) pi.SetValue(component, propertyValue, null);
        }
        #endregion

        #region MasterComponent
        public static void SetMasterComponentProperty(object component, object propertyValue)
        {
            StiComponent comp = (StiComponent)component;
            StiComponent masterComponent = (propertyValue as string != "[Not Assigned]") ? comp.Report.Pages.GetComponentByName(propertyValue as string) : null;

            PropertyInfo pi = component.GetType().GetProperty("MasterComponent");
            if (pi != null) pi.SetValue(component, masterComponent, null);
        }
        #endregion

        #region BusinessObject
        public static void SetBusinessObjectProperty(object component, object propertyValue)
        {
            StiComponent comp = (StiComponent)component;
            ArrayList nameArray = new ArrayList();
            nameArray.AddRange(((string)propertyValue).Split('.'));
            nameArray.Reverse();
            StiBusinessObject businessObject = ((string)propertyValue != "[Not Assigned]")
                ? StiDictionaryHelper.GetBusinessObjectByFullName(comp.Report, nameArray) : null;
            PropertyInfo pi = component.GetType().GetProperty("BusinessObjectGuid");
            if (pi != null) pi.SetValue(component, businessObject != null ? businessObject.Guid : string.Empty, null);
        }
        #endregion 

        #region Sort
        public static void SetSortDataProperty(object object_, ArrayList sortArray)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < sortArray.Count; i++)
            {
                Hashtable oneSort = (Hashtable)sortArray[i];
                result = GetSortArray(object_, oneSort, result);
            }

            PropertyInfo pi = object_.GetType().GetProperty("Sort");
            if (pi != null) pi.SetValue(object_, result.ToArray(), null);
        }

        public static void SetSortDataProperty(object object_, object propertyValue)
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(propertyValue as string))
            {
                ArrayList sortArray = (ArrayList)JSON.Decode(StiEncodingHelper.DecodeString(propertyValue as string));

                for (int i = 0; i < sortArray.Count; i++)
                {
                    Hashtable oneSort = (Hashtable)sortArray[i];
                    result = GetSortArray(object_, oneSort, result);
                }
            }

            PropertyInfo pi = object_.GetType().GetProperty("Sort");
            if (pi != null) pi.SetValue(object_, result.ToArray(), null);
        }

        private static List<string> GetSortArray(object object_, Hashtable sort, List<string> result)
        {
            StiDataSource dataSource = null;
            StiBusinessObject businessObject = null;

            if (object_ is StiVirtualSource)
            {
                dataSource = ((StiVirtualSource)object_).Dictionary.DataSources[((StiVirtualSource)object_).NameInSource];
            }
            else
            {
                PropertyInfo pDataSource = object_.GetType().GetProperty("DataSource");
                if (pDataSource != null) dataSource = (StiDataSource)pDataSource.GetValue(object_, null);
                if (dataSource == null)
                {
                    PropertyInfo pBusinessObject = object_.GetType().GetProperty("BusinessObject");
                    businessObject = pBusinessObject.GetValue(object_, null) as StiBusinessObject;
                }
            }

            if (dataSource != null || businessObject != null)
            {
                result.Add((string)sort["direction"]);
                string column = (string)sort["column"];
                if (column.StartsWith("{") && column.EndsWith("}"))
                {
                    result.Add(column);
                }
                else
                {
                    string[] columnPath = column.Split('.');
                    object dataObject = null;
                    if (dataSource != null) dataObject = dataSource;
                    if (businessObject != null) dataObject = businessObject;
                    result = GetColumnPathArray(dataObject, columnPath, result);
                }
            }

            return result;
        }

        private static List<string> GetColumnPathArray(object dataObject, string[] columnPath, List<string> result)
        {
            if (dataObject is StiDataSource)
            {
                StiDataSource dataSource = dataObject as StiDataSource;
                if (columnPath.Length == 1)
                {
                    result.Add(columnPath[0]);
                }
                else
                {
                    StiDataRelationsCollection relations = dataSource.GetParentRelations();

                    for (int level = 0; level < columnPath.Length - 1; level++)
                    {
                        StiDataRelation relation = GetChildRelation(relations, columnPath[level]);
                        if (relation != null)
                        {
                            result.Add(relation.NameInSource);
                            relations = relation.ParentSource.GetParentRelations();
                        }
                    }

                    result.Add(columnPath[columnPath.Length - 1]);
                }
            }
            else if (dataObject is StiBusinessObject)
            {
                if (columnPath.Length > 0)
                {
                    for (int level = 0; level < columnPath.Length; level++)
                    {
                        result.Add(columnPath[level]);
                    }
                }
                else
                {
                    result.Add(string.Empty);
                }
            }

            return result;
        }

        private static StiDataRelation GetChildRelation(StiDataRelationsCollection relations, string relationName)
        {
            foreach (StiDataRelation relation in relations)
                if (relation.Name == relationName)
                    return relation;

            return null;
        }
        #endregion

        #region Filter
        public static void SetFilterDataProperty(object component, ArrayList filters)
        {
            StiFiltersCollection filtersCollection = new StiFiltersCollection();

            for (var i = 0; i < filters.Count; i++)
            {
                Hashtable filter = (Hashtable)filters[i];
                filtersCollection.Add(FilterFromObject(filter));
            }

            PropertyInfo pi = component.GetType().GetProperty("Filters");
            if (pi != null) pi.SetValue(component, filtersCollection, null);
        }

        public static void SetFilterDataProperty(object component, object propertyValue)
        {
            ArrayList filtersObject = (ArrayList)JSON.Decode(StiEncodingHelper.DecodeString(propertyValue as string));

            StiFiltersCollection filters = new StiFiltersCollection();

            if (!string.IsNullOrEmpty(propertyValue as string))
            {
                for (int i = 0; i < filtersObject.Count; i++)
                {
                    Hashtable filter = (Hashtable)filtersObject[i];
                    filters.Add(FilterFromObject(filter));
                }
            }

            PropertyInfo pi = component.GetType().GetProperty("Filters");
            if (pi != null) pi.SetValue(component, filters, null);
        }

        private static StiFilter FilterFromObject(Hashtable filterObject)
        {
            StiFilter filter = new StiFilter();
            filter.Item = (((string)filterObject["fieldIs"]) == "Value") ? StiFilterItem.Value : StiFilterItem.Expression;
            filter.DataType = StrToFilterDataType((string)filterObject["dataType"]);
            filter.Column = (string)filterObject["column"];
            filter.Condition = StrToFilterCondition((string)filterObject["condition"]);
            filter.Value1 = (string)filterObject["value1"];
            filter.Value2 = (string)filterObject["value2"];            
            filter.Expression.Value = (string)filterObject["expression"] != string.Empty ? "{" + (string)filterObject["expression"] + "}" : string.Empty;

            return filter;
        }

        private static StiFilterDataType StrToFilterDataType(string dataType)
        {
            return (StiFilterDataType)Enum.Parse(typeof(StiFilterDataType), dataType);
        }

        private static StiFilterCondition StrToFilterCondition(string condition)
        {
            return (StiFilterCondition)Enum.Parse(typeof(StiFilterCondition), condition);
        }

        public static void SetFilterOnProperty(object component, object propertyValue)
        {
            PropertyInfo pi = component.GetType().GetProperty("FilterOn");
            if (pi != null) pi.SetValue(component, propertyValue, null);
        }

        public static void SetFilterModeProperty(object component, object propertyValue)
        {
            PropertyInfo pi = component.GetType().GetProperty("FilterMode");
            if (pi != null) pi.SetValue(component, propertyValue as string == "And" ? StiFilterMode.And : StiFilterMode.Or, null);
        }
        #endregion        

        #region ShiftMode
        public static void SetShiftModeProperty(object component, object propValue)
        {
            StiComponent comp = (StiComponent)component;
            string propertyValue = propValue as string;

            int value = 0;
            if (propertyValue.IndexOf("IncreasingSize") >= 0) value += 1;
            if (propertyValue.IndexOf("DecreasingSize") >= 0) value += 2;
            if (propertyValue.IndexOf("OnlyInWidthOfComponent") >= 0) value += 4;
            if (propertyValue.IndexOf("All") >= 0) value = 7;

            comp.ShiftMode = (StiShiftMode)value;
        }
        #endregion

        #region Restrictions
        public static void SetRestrictionsProperty(object component, object propValue)
        {
            StiComponent comp = (StiComponent)component;
            string propertyValue = propValue as string;

            int value = 0;
            if (propertyValue.IndexOf("AllowMove") >= 0) value += 1;
            if (propertyValue.IndexOf("AllowResize") >= 0) value += 2;
            if (propertyValue.IndexOf("AllowSelect") >= 0) value += 4;
            if (propertyValue.IndexOf("AllowChange") >= 0) value += 8;
            if (propertyValue.IndexOf("AllowDelete") >= 0) value += 16;
            if (propertyValue == "All") value = 31;

            comp.Restrictions = (Stimulsoft.Report.Components.StiRestrictions)value;
        }
        #endregion

        #region IndicatorConditionsPermissions
        public static StiIndicatorConditionPermissions StrToIndicatorConditionsPermissions(string propertyValue)
        {
            if (propertyValue == "All") return StiIndicatorConditionPermissions.All;

            int value = 0;
            if (propertyValue == "Font" || propertyValue.IndexOf("Font,") == 0 || propertyValue.EndsWith(" Font") || propertyValue.IndexOf(" Font,") >= 0) value += 1;
            if (propertyValue.IndexOf("FontSize") >= 0) value += 2;
            if (propertyValue.IndexOf("FontStyleBold") >= 0) value += 4;
            if (propertyValue.IndexOf("FontStyleItalic") >= 0) value += 8;
            if (propertyValue.IndexOf("FontStyleUnderline") >= 0) value += 16;
            if (propertyValue.IndexOf("FontStyleStrikeout") >= 0) value += 32;
            if (propertyValue.IndexOf("TextColor") >= 0) value += 64;
            if (propertyValue.IndexOf("BackColor") >= 0) value += 128;
            if (propertyValue.IndexOf("Borders,") >= 0) value += 256;
            if (propertyValue == "Icon" || propertyValue.IndexOf("Icon,") == 0 || propertyValue.EndsWith(" Icon") || propertyValue.IndexOf(" Icon,") >= 0) value += 512;
            if (propertyValue.IndexOf("TargetIcon") >= 0) value += 1024;

            return (StiIndicatorConditionPermissions)value;
        }
        #endregion

        #region ProgressConditionsPermissions
        public static StiProgressConditionPermissions StrToProgressConditionsPermissions(string propertyValue)
        {
            if (propertyValue == "All") return StiProgressConditionPermissions.All;

            int value = 0;
            if (propertyValue == "Font" || propertyValue.IndexOf("Font,") == 0 || propertyValue.EndsWith(" Font") || propertyValue.IndexOf(" Font,") >= 0) value += 1;
            if (propertyValue.IndexOf("FontSize") >= 0) value += 2;
            if (propertyValue.IndexOf("FontStyleBold") >= 0) value += 4;
            if (propertyValue.IndexOf("FontStyleItalic") >= 0) value += 8;
            if (propertyValue.IndexOf("FontStyleUnderline") >= 0) value += 16;
            if (propertyValue.IndexOf("FontStyleStrikeout") >= 0) value += 32;
            if (propertyValue.IndexOf("TextColor") >= 0) value += 64;
            if (propertyValue == "Color" || propertyValue.IndexOf("Color,") == 0 || propertyValue.EndsWith(" Color") || propertyValue.IndexOf(" Color,") >= 0) value += 128;
            if (propertyValue.IndexOf("TrackColor") >= 0) value += 256;

            return (StiProgressConditionPermissions)value;
        }
        #endregion

        #region ProgressConditionsPermissions
        public static StiTableConditionPermissions StrToTableConditionsPermissions(string propertyValue)
        {
            if (propertyValue == "All") return StiTableConditionPermissions.All;

            int value = 0;
            if (propertyValue == "Font" || propertyValue.IndexOf("Font,") == 0 || propertyValue.EndsWith(" Font") || propertyValue.IndexOf(" Font,") >= 0) value += 1;
            if (propertyValue.IndexOf("FontSize") >= 0) value += 2;
            if (propertyValue.IndexOf("FontStyleBold") >= 0) value += 4;
            if (propertyValue.IndexOf("FontStyleItalic") >= 0) value += 8;
            if (propertyValue.IndexOf("FontStyleUnderline") >= 0) value += 16;
            if (propertyValue.IndexOf("FontStyleStrikeout") >= 0) value += 32;
            if (propertyValue.IndexOf("ForeColor") >= 0) value += 64;
            if (propertyValue.IndexOf("BackColor") >= 0) value += 128;

            return (StiTableConditionPermissions)value;
        }
        #endregion

        #region Conditions
        public static void SetConditionsProperty(object component, object propertyValue, StiReport report)
        {
            StiConditionsCollection conditions = new StiConditionsCollection();

            if (propertyValue as string != "")
            {
                ArrayList conditionsArray = JSON.Decode(StiEncodingHelper.DecodeString(propertyValue as string)) as ArrayList;

                foreach (Hashtable conditionObject in conditionsArray)
                {
                    string conditionType = (string)conditionObject["ConditionType"];

                    if (conditionType == "StiDataBarCondition")
                    {
                        conditions.Add(CreateDataBarCondition(conditionObject));
                    }
                    else if (conditionType == "StiIconSetCondition")
                    {
                        conditions.Add(CreateIconSetCondition(conditionObject));
                    }
                    else if (conditionType == "StiColorScaleCondition")
                    {
                        conditions.Add(CreateColorScaleCondition(conditionObject));
                    }
                    else if (conditionType == "StiHighlightCondition")
                    {
                        StiBaseCondition highlightCondition = CreateHighlightCondition(conditionObject, report);
                        if (highlightCondition != null) conditions.Add(highlightCondition);
                    }
                }
            }

            PropertyInfo pi = component.GetType().GetProperty("Conditions");
            if (pi != null) pi.SetValue(component, conditions, null);
        }

        public static StiBaseCondition CreateHighlightCondition(Hashtable conditionObject, StiReport report)
        {
            ArrayList filters = JSON.Decode(StiEncodingHelper.DecodeString((string)conditionObject["Filters"])) as ArrayList;

            if (filters.Count > 1)
            {
                StiMultiCondition condition = new StiMultiCondition();
                SetFilterDataProperty(condition, filters);
                if (conditionObject["FilterMode"] != null) SetFilterModeProperty(condition, (string)conditionObject["FilterMode"]);
                condition.TextColor = StrToColor((string)conditionObject["TextColor"]);
                condition.BackColor = StrToColor((string)conditionObject["BackColor"]);
                condition.Font = StrToFont((string)conditionObject["Font"]);
                condition.Enabled = (bool)conditionObject["Enabled"];
                condition.CanAssignExpression = (bool)conditionObject["CanAssignExpression"];
                condition.AssignExpression = StiEncodingHelper.DecodeString((string)conditionObject["AssignExpression"]);
                condition.BorderSides = StrBordersToConditionBorderSidesObject((string)conditionObject["BorderSides"]);
                condition.Permissions = StrPermissionsToConditionPermissionsObject((string)conditionObject["Permissions"]);
                condition.Style = (string)conditionObject["Style"];
                condition.BreakIfTrue = (bool)conditionObject["BreakIfTrue"];

                return condition;
            }
            else if (filters.Count == 1)
            {
                StiCondition condition = new StiCondition();
                StiFilter filter = FilterFromObject((Hashtable)filters[0]);
                condition.Item = filter.Item;
                condition.DataType = filter.DataType;
                condition.Column = filter.Column;
                condition.Condition = filter.Condition;
                condition.Value1 = filter.Value1;
                condition.Value2 = filter.Value2;
                condition.Expression = filter.Expression;

                condition.BreakIfTrue = (bool)conditionObject["BreakIfTrue"];
                condition.TextColor = StrToColor((string)conditionObject["TextColor"]);
                condition.BackColor = StrToColor((string)conditionObject["BackColor"]);
                condition.Font = StrToFont((string)conditionObject["Font"]);
                condition.Enabled = (bool)conditionObject["Enabled"];
                condition.CanAssignExpression = (bool)conditionObject["CanAssignExpression"];
                condition.AssignExpression = StiEncodingHelper.DecodeString((string)conditionObject["AssignExpression"]);
                condition.BorderSides = StrBordersToConditionBorderSidesObject((string)conditionObject["BorderSides"]);
                condition.Permissions = StrPermissionsToConditionPermissionsObject((string)conditionObject["Permissions"]);
                condition.Style = (string)conditionObject["Style"];

                return condition;
            }

            return null;
        }

        public static StiBaseCondition CreateDataBarCondition(Hashtable conditionObject)
        {
            StiDataBarCondition condition = new StiDataBarCondition(
                (string)conditionObject["Column"],
                (Components.StiBrushType)Enum.Parse(typeof(Components.StiBrushType), (string)conditionObject["BrushType"]),
                StrToColor((string)conditionObject["PositiveColor"]),
                StrToColor((string)conditionObject["NegativeColor"]),
                (bool)conditionObject["ShowBorder"],
                StrToColor((string)conditionObject["PositiveBorderColor"]),
                StrToColor((string)conditionObject["NegativeBorderColor"]),
                (StiDataBarDirection)Enum.Parse(typeof(StiDataBarDirection), (string)conditionObject["Direction"]),
                (StiMinimumType)Enum.Parse(typeof(StiMinimumType), (string)conditionObject["MinimumType"]),
                (float)StrToDouble((string)conditionObject["MinimumValue"]),
                (StiMaximumType)Enum.Parse(typeof(StiMaximumType), (string)conditionObject["MaximumType"]),
                (float)StrToDouble((string)conditionObject["MaximumValue"])
            );

            return condition;
        }

        public static StiBaseCondition CreateColorScaleCondition(Hashtable conditionObject)
        {
            StiColorScaleCondition condition = new StiColorScaleCondition(
                (string)conditionObject["Column"],
                (StiColorScaleType)Enum.Parse(typeof(StiColorScaleType), (string)conditionObject["ScaleType"]),
                StrToColor((string)conditionObject["MinimumColor"]),
                StrToColor((string)conditionObject["MidColor"]),
                StrToColor((string)conditionObject["MaximumColor"]),
                (StiMinimumType)Enum.Parse(typeof(StiMinimumType), (string)conditionObject["MinimumType"]),
                (float)StrToDouble((string)conditionObject["MinimumValue"]),
                (StiMidType)Enum.Parse(typeof(StiMidType), (string)conditionObject["MidType"]),
                (float)StrToDouble((string)conditionObject["MidValue"]),
                (StiMaximumType)Enum.Parse(typeof(StiMaximumType), (string)conditionObject["MaximumType"]),
                (float)StrToDouble((string)conditionObject["MaximumValue"])
            );

            return condition;
        }

        public static StiBaseCondition CreateIconSetCondition(Hashtable conditionObject)
        {
            StiIconSetCondition condition = new StiIconSetCondition(
                (string)conditionObject["Column"],
                (StiIconSet)Enum.Parse(typeof(StiIconSet), (string)conditionObject["IconSet"]),
                (ContentAlignment)Enum.Parse(typeof(ContentAlignment), (string)conditionObject["ContentAlignment"]),
                GetIconSetItemFromObject(conditionObject["IconSetItem1"]),
                GetIconSetItemFromObject(conditionObject["IconSetItem2"]),
                GetIconSetItemFromObject(conditionObject["IconSetItem3"]),
                GetIconSetItemFromObject(conditionObject["IconSetItem4"]),
                GetIconSetItemFromObject(conditionObject["IconSetItem5"])
            );

            return condition;
        }
        #endregion

        #region Interaction
        public static void SetInteractionProperty(object component, object propertyValue)
        {
            Hashtable interactionObject = (Hashtable)propertyValue;
            StiInteraction interaction = ((StiComponent)component).Interaction;

            if (interaction is StiBandInteraction)
            {
                ((StiBandInteraction)interaction).CollapsingEnabled = (bool)interactionObject["collapsingEnabled"];
                ((StiBandInteraction)interaction).CollapseGroupFooter = (bool)interactionObject["collapseGroupFooter"];
                ((StiBandInteraction)interaction).Collapsed.Value = StiEncodingHelper.DecodeString((string)interactionObject["collapsedValue"]);
                ((StiBandInteraction)interaction).SelectionEnabled = (bool)interactionObject["selectionEnabled"];
            }

            if (interaction is StiCrossHeaderInteraction)
            {
                ((StiCrossHeaderInteraction)interaction).CollapsingEnabled = (bool)interactionObject["crossHeaderCollapsingEnabled"];
            }

            interaction.DrillDownEnabled = (bool)interactionObject["drillDownEnabled"];
            interaction.DrillDownReport = (string)interactionObject["drillDownReport"];
            interaction.DrillDownMode = (StiDrillDownMode)Enum.Parse(typeof(StiDrillDownMode), (string)interactionObject["drillDownMode"]);
            interaction.DrillDownPage = ((StiComponent)component).Report.Pages[(string)interactionObject["drillDownPage"]];
            interaction.DrillDownParameter1.Name = (string)interactionObject["drillDownParameter1Name"];
            interaction.DrillDownParameter1.Expression.Value = StiEncodingHelper.DecodeString((string)interactionObject["drillDownParameter1Expression"]);
            interaction.DrillDownParameter2.Name = (string)interactionObject["drillDownParameter2Name"];
            interaction.DrillDownParameter2.Expression.Value = StiEncodingHelper.DecodeString((string)interactionObject["drillDownParameter2Expression"]);
            interaction.DrillDownParameter3.Name = (string)interactionObject["drillDownParameter3Name"];
            interaction.DrillDownParameter3.Expression.Value = StiEncodingHelper.DecodeString((string)interactionObject["drillDownParameter3Expression"]);
            interaction.DrillDownParameter4.Name = (string)interactionObject["drillDownParameter4Name"];
            interaction.DrillDownParameter4.Expression.Value = StiEncodingHelper.DecodeString((string)interactionObject["drillDownParameter4Expression"]);
            interaction.DrillDownParameter5.Name = (string)interactionObject["drillDownParameter5Name"];
            interaction.DrillDownParameter5.Expression.Value = StiEncodingHelper.DecodeString((string)interactionObject["drillDownParameter5Expression"]);
            interaction.DrillDownParameter6.Name = (string)interactionObject["drillDownParameter6Name"];
            interaction.DrillDownParameter6.Expression.Value = StiEncodingHelper.DecodeString((string)interactionObject["drillDownParameter6Expression"]);
            interaction.DrillDownParameter7.Name = (string)interactionObject["drillDownParameter7Name"];
            interaction.DrillDownParameter7.Expression.Value = StiEncodingHelper.DecodeString((string)interactionObject["drillDownParameter7Expression"]);
            interaction.DrillDownParameter8.Name = (string)interactionObject["drillDownParameter8Name"];
            interaction.DrillDownParameter8.Expression.Value = StiEncodingHelper.DecodeString((string)interactionObject["drillDownParameter8Expression"]);
            interaction.DrillDownParameter9.Name = (string)interactionObject["drillDownParameter9Name"];
            interaction.DrillDownParameter9.Expression.Value = StiEncodingHelper.DecodeString((string)interactionObject["drillDownParameter9Expression"]);
            interaction.DrillDownParameter10.Name = (string)interactionObject["drillDownParameter10Name"];
            interaction.DrillDownParameter10.Expression.Value = StiEncodingHelper.DecodeString((string)interactionObject["drillDownParameter10Expression"]);

            interaction.SortingEnabled = (bool)interactionObject["sortingEnabled"];
            interaction.SortingColumn = string.IsNullOrEmpty(interactionObject["sortingColumn"] as string) ? string.Empty : (string)interactionObject["sortingColumn"];

            interaction.Bookmark.Value = StiEncodingHelper.DecodeString((string)interactionObject["bookmark"]);
            interaction.Tag.Value = StiEncodingHelper.DecodeString((string)interactionObject["tag"]);
            interaction.ToolTip.Value = StiEncodingHelper.DecodeString((string)interactionObject["toolTip"]);

            switch ((string)interactionObject["hyperlinkType"])
            {
                case "HyperlinkUsingInteractionBookmark":
                    interaction.Hyperlink.Value = "#" + StiEncodingHelper.DecodeString((string)interactionObject["hyperlink"]);
                    break;

                case "HyperlinkUsingInteractionTag":
                    interaction.Hyperlink.Value = "##" + StiEncodingHelper.DecodeString((string)interactionObject["hyperlink"]);
                    break;

                case "HyperlinkExternalDocuments":
                    interaction.Hyperlink.Value = StiEncodingHelper.DecodeString((string)interactionObject["hyperlink"]);
                    break;
            }
        }
        #endregion

        #region ChartStyle
        public static void SetChartStyleProperty(object component, object propertyValue)
        {
            Hashtable param = new Hashtable();
            param["componentName"] = ((StiComponent)component).Name;
            param["styleType"] = ((Hashtable)propertyValue)["type"];
            param["styleName"] = ((Hashtable)propertyValue)["name"];

            StiChartHelper.SetChartStyle(((StiComponent)component).Report, param, new Hashtable());
        }
        #endregion

        #region GaugeStyle
        public static void SetGaugeStyleProperty(object component, object propertyValue)
        {
            Hashtable param = new Hashtable();
            param["componentName"] = ((StiComponent)component).Name;
            param["styleType"] = ((Hashtable)propertyValue)["type"];
            param["styleName"] = ((Hashtable)propertyValue)["name"];

            StiGaugeHelper.SetGaugeStyle(((StiComponent)component).Report, param, new Hashtable());
        }
        #endregion

        #region MapStyle
        public static void SetMapStyleProperty(object component, object propertyValue)
        {
            Hashtable param = new Hashtable();
            param["componentName"] = ((StiComponent)component).Name;
            param["styleType"] = ((Hashtable)propertyValue)["type"];
            param["styleName"] = ((Hashtable)propertyValue)["name"];

            StiMapHelper.SetMapStyle(((StiComponent)component).Report, param, new Hashtable());
        }
        #endregion

        #region CrossTabStyle
        public static void SetCrossTabStyleProperty(object component, object propertyValue)
        {
            Hashtable parameters = (Hashtable)propertyValue;
            StiCrossTab crossTab = (StiCrossTab)component;

            if (parameters["crossTabStyleIndex"] != null)
            {
                crossTab.CrossTabStyle = string.Empty;
                crossTab.CrossTabStyleIndex = Convert.ToInt32(parameters["crossTabStyleIndex"]);
            }
            else
            {
                crossTab.CrossTabStyleIndex = -1;
                crossTab.CrossTabStyle = (string)parameters["crossTabStyle"];
                crossTab.UpdateStyles();
            }

            crossTab.UpdateStyles();
        }
        #endregion

        #region SubReportParameters
        public static void SetSubReportParametersProperty(object component, object propertyValue)
        {
            StiSubReport subReport = component as StiSubReport;
            ArrayList parameters = propertyValue as ArrayList;
            subReport.Parameters.Clear();

            foreach (Hashtable parameterObject in parameters)
            {
                subReport.Parameters.Add(new StiParameter()
                {
                    Name = parameterObject["name"] as string,
                    Expression = StiEncodingHelper.DecodeString(parameterObject["expression"] as string)
                });
            }
        }

        #endregion

        #region Anchor
        public static void SetAnchorProperty(object component, object propValue)
        {
            StiComponent comp = (StiComponent)component;
            string propertyValue = propValue as string;

            int value = 0;
            if (propertyValue.IndexOf("Top") >= 0) value += 1;
            if (propertyValue.IndexOf("Bottom") >= 0) value += 2;
            if (propertyValue.IndexOf("Left") >= 0) value += 4;
            if (propertyValue.IndexOf("Right") >= 0) value += 8;

            comp.Anchor = (StiAnchorMode)value;
        }
        #endregion

        #region Image
        public static void SetImageProperty(StiComponent component, string propValue)
        {
            if (string.IsNullOrEmpty(propValue))
                ((StiImage)component).ResetImage();
            else
                ((StiImage)component).PutImage(Base64ToImage(propValue));
        }
        #endregion

        #region WatermarkImage
        public static void SetWatermarkImageProperty(StiPage page, string propValue)
        {
            if (string.IsNullOrEmpty(propValue))
                page.Watermark.ResetImage();
            else
                page.Watermark.PutImage(Base64ToImage(propValue));
        }
        #endregion

        #region ExcelValue
        public static void SetExcelValueProperty(object component, object propertyValue)
        {
            propertyValue = StiEncodingHelper.DecodeString(propertyValue as string).Replace("\n", "\r\n");
            PropertyInfo pi = component.GetType().GetProperty("ExcelValue");
            if (pi != null) pi.SetValue(component, new StiExcelValueExpression(propertyValue as string), null);
        }
        #endregion

        #region ExcelSheet
        public static void SetExcelSheetProperty(object component, object propertyValue)
        {
            propertyValue = StiEncodingHelper.DecodeString(propertyValue as string).Replace("\n", "\r\n");
            PropertyInfo pi = component.GetType().GetProperty("ExcelSheet");
            if (pi != null) pi.SetValue(component, new StiExcelSheetExpression(propertyValue as string), null);
        }
        #endregion

        #region PreviewSettings
        public static void SetPreviewSettingsProperty(StiReport report, Hashtable previewSettings, Hashtable callbackResult)
        {
            report.PreviewSettings = 0;

            if (Convert.ToBoolean(previewSettings["reportPrint"])) report.PreviewSettings |= (int)StiPreviewSettings.Print;
            if (Convert.ToBoolean(previewSettings["reportOpen"])) report.PreviewSettings |= (int)StiPreviewSettings.Open;
            if (Convert.ToBoolean(previewSettings["reportSave"])) report.PreviewSettings |= (int)StiPreviewSettings.Save;
            if (Convert.ToBoolean(previewSettings["reportSendEMail"])) report.PreviewSettings |= (int)StiPreviewSettings.SendEMail;
            if (Convert.ToBoolean(previewSettings["reportPageControl"])) report.PreviewSettings |= (int)StiPreviewSettings.PageControl;
            if (Convert.ToBoolean(previewSettings["reportEditor"])) report.PreviewSettings |= (int)StiPreviewSettings.Editor;
            if (Convert.ToBoolean(previewSettings["reportFind"])) report.PreviewSettings |= (int)StiPreviewSettings.Find;
            if (Convert.ToBoolean(previewSettings["reportSignature"])) report.PreviewSettings |= (int)StiPreviewSettings.Signature;
            if (Convert.ToBoolean(previewSettings["reportPageViewMode"])) report.PreviewSettings |= (int)StiPreviewSettings.PageViewMode;
            if (Convert.ToBoolean(previewSettings["reportZoom"])) report.PreviewSettings |= (int)StiPreviewSettings.Zoom;
            if (Convert.ToBoolean(previewSettings["reportBookmarks"])) report.PreviewSettings |= (int)StiPreviewSettings.Bookmarks;
            if (Convert.ToBoolean(previewSettings["reportParameters"])) report.PreviewSettings |= (int)StiPreviewSettings.Parameters;
            if (Convert.ToBoolean(previewSettings["reportResources"])) report.PreviewSettings |= (int)StiPreviewSettings.Resources;
            if (Convert.ToBoolean(previewSettings["reportStatusBar"])) report.PreviewSettings |= (int)StiPreviewSettings.StatusBar;
            if (Convert.ToBoolean(previewSettings["reportToolbar"])) report.PreviewSettings |= (int)StiPreviewSettings.Toolbar;

            report.DashboardViewerSettings = 0;

            if (Convert.ToBoolean(previewSettings["dashboardToolBar"])) report.DashboardViewerSettings |= StiDashboardViewerSettings.ShowToolBar;
            if (Convert.ToBoolean(previewSettings["dashboardRefreshButton"])) report.DashboardViewerSettings |= StiDashboardViewerSettings.ShowRefreshButton;
            if (Convert.ToBoolean(previewSettings["dashboardOpenButton"])) report.DashboardViewerSettings |= StiDashboardViewerSettings.ShowOpenButton;
            if (Convert.ToBoolean(previewSettings["dashboardEditButton"])) report.DashboardViewerSettings |= StiDashboardViewerSettings.ShowEditButton;
            if (Convert.ToBoolean(previewSettings["dashboardFullScreenButton"])) report.DashboardViewerSettings |= StiDashboardViewerSettings.ShowFullScreenButton;
            if (Convert.ToBoolean(previewSettings["dashboardMenuButton"])) report.DashboardViewerSettings |= StiDashboardViewerSettings.ShowMenuButton;
            if (Convert.ToBoolean(previewSettings["dashboardResetAllFiltersButton"])) report.DashboardViewerSettings |= StiDashboardViewerSettings.ShowResetAllFilters;
            if (Convert.ToBoolean(previewSettings["dashboardParametersButton"])) report.DashboardViewerSettings |= StiDashboardViewerSettings.ShowParametersButton;
            if (Convert.ToBoolean(previewSettings["dashboardShowReportSnapshots"])) report.DashboardViewerSettings |= StiDashboardViewerSettings.ShowReportSnapshots;
            if (Convert.ToBoolean(previewSettings["dashboardShowExports"])) report.DashboardViewerSettings |= StiDashboardViewerSettings.ShowExports;

            report.HtmlPreviewMode = (StiHtmlPreviewMode)Enum.Parse(typeof(StiHtmlPreviewMode), previewSettings["htmlPreviewMode"] as string);
            report.PreviewToolBarOptions.ReportToolbarHorAlignment = (StiHorAlignment)Enum.Parse(typeof(StiHorAlignment), previewSettings["reportToolbarHorAlignment"] as string);
            report.PreviewToolBarOptions.ReportToolbarReverse = Convert.ToBoolean(previewSettings["reportToolbarReverse"]);
            report.PreviewToolBarOptions.DashboardToolbarHorAlignment = (StiHorAlignment)Enum.Parse(typeof(StiHorAlignment), previewSettings["dashboardToolbarHorAlignment"] as string);
            report.PreviewToolBarOptions.DashboardToolbarReverse = Convert.ToBoolean(previewSettings["dashboardToolbarReverse"]);

            callbackResult["resultPreviewSettings"] = GetPreviewSettingsProperty(report);
        }
        #endregion

        #region ColorsCollection
        public static void SetColorsCollectionProperty(object object_, string propertyName, ArrayList propertyValue)
        {
            PropertyInfo pi = object_.GetType().GetProperty(propertyName);
            if (pi != null && propertyValue != null)
            {
                var colors = new List<Color>();
                foreach (string colorString in propertyValue)
                {
                    colors.Add(StrToColor(colorString));
                }
                pi.SetValue(object_, colors.ToArray(), null);
            }
        }
        #endregion

        #region TopN
        public static void SetTopNProperty(object component, object propertyValue)
        {
            if (propertyValue != null) {
                var topN = new StiDataTopN()
                {
                    Mode = (StiDataTopNMode)Enum.Parse(typeof(StiDataTopNMode), ((Hashtable)propertyValue)["mode"] as string),
                    Count = Convert.ToInt32(((Hashtable)propertyValue)["count"]),
                    ShowOthers = Convert.ToBoolean(((Hashtable)propertyValue)["showOthers"]),
                    OthersText = ((Hashtable)propertyValue)["othersText"] as string,
                    MeasureField = ((Hashtable)propertyValue)["measureField"] as string
                };

                PropertyInfo pi = component.GetType().GetProperty("TopN");
                if (pi != null) pi.SetValue(component, topN, null);
            }
        }
        #endregion

        #region DashboardInteraction
        public static void SetDashboardInteractionProperty(IStiDashboardInteraction dashboardInteraction, object propertyValue)
        {
            if (propertyValue != null && dashboardInteraction != null)
            {
                var interaction = propertyValue as Hashtable;

                dashboardInteraction.OnHover = (StiInteractionOnHover)Enum.Parse(typeof(StiInteractionOnHover), interaction["onHover"] as string);
                dashboardInteraction.ToolTip = StiEncodingHelper.DecodeString(interaction["toolTip"] as string);
                dashboardInteraction.OnClick = (StiInteractionOnClick)Enum.Parse(typeof(StiInteractionOnClick), interaction["onClick"] as string);
                dashboardInteraction.Hyperlink = StiEncodingHelper.DecodeString(interaction["hyperlink"] as string);
                dashboardInteraction.HyperlinkDestination = (StiInteractionOpenHyperlinkDestination)Enum.Parse(typeof(StiInteractionOpenHyperlinkDestination), interaction["hyperlinkDestination"] as string);
                dashboardInteraction.DrillDownPageKey = interaction["drillDownPageKey"] as string;
                dashboardInteraction.SetDrillDownParameters(interaction["drillDownParameters"] as ArrayList);

                if (dashboardInteraction is IStiAllowUserColumnSelectionDashboardInteraction)
                {
                    ((IStiAllowUserColumnSelectionDashboardInteraction)dashboardInteraction).AllowUserColumnSelection = Convert.ToBoolean(((Hashtable)propertyValue)["allowUserColumnSelection"]);
                }

                if (dashboardInteraction is IStiAllowUserSortingDashboardInteraction)
                {
                    ((IStiAllowUserSortingDashboardInteraction)dashboardInteraction).AllowUserSorting = Convert.ToBoolean(((Hashtable)propertyValue)["allowUserSorting"]);
                }

                if (dashboardInteraction is IStiAllowUserFilteringDashboardInteraction)
                {
                    ((IStiAllowUserFilteringDashboardInteraction)dashboardInteraction).AllowUserFiltering = Convert.ToBoolean(((Hashtable)propertyValue)["allowUserFiltering"]);
                }

                if (dashboardInteraction is IStiAllowUserDrillDownDashboardInteraction)
                {
                    ((IStiAllowUserDrillDownDashboardInteraction)dashboardInteraction).AllowUserDrillDown = Convert.ToBoolean(((Hashtable)propertyValue)["allowUserDrillDown"]);
                }

                if (dashboardInteraction is IStiTableDashboardInteraction)
                {   
                    ((IStiTableDashboardInteraction)dashboardInteraction).DrillDownFiltered = Convert.ToBoolean(((Hashtable)propertyValue)["drillDownFiltered"]);
                    ((IStiTableDashboardInteraction)dashboardInteraction).FullRowSelect = Convert.ToBoolean(((Hashtable)propertyValue)["fullRowSelect"]);
                }

                if (dashboardInteraction is IStiChartDashboardInteraction)
                {
                    ((IStiChartDashboardInteraction)dashboardInteraction).ViewsState = (StiInteractionViewsState)Enum.Parse(typeof(StiInteractionViewsState), interaction["viewsState"] as string);
                }

                if (dashboardInteraction is IStiInteractionLayout)
                {
                    ((IStiInteractionLayout)dashboardInteraction).ShowFullScreenButton = Convert.ToBoolean(((Hashtable)propertyValue)["showFullScreenButton"]);
                    ((IStiInteractionLayout)dashboardInteraction).ShowSaveButton = Convert.ToBoolean(((Hashtable)propertyValue)["showSaveButton"]);
                    ((IStiInteractionLayout)dashboardInteraction).ShowViewDataButton = Convert.ToBoolean(((Hashtable)propertyValue)["showViewDataButton"]);
                }
            }
        }
        #endregion

        #region ChartConditions
        public static void SetChartConditionsProperty(object component, object propertyValue)
        {
            var chartElement = component as IStiChartElement;
            if (chartElement != null && propertyValue != null)
            {
                chartElement.ClearChartConditions();

                var valueMeters = chartElement.FetchAllValues();
                var argumentMeters = chartElement.FetchAllArguments();
                var seriesMeter = chartElement.GetSeries();
                var chartConditionsArray = propertyValue as ArrayList;

                foreach (Hashtable chartConditionObject in chartConditionsArray)
                {
                    var keyValueMeter = chartConditionObject["keyValueMeter"] as string;
                    var fieldType = StiChartConditionalField.Value;
                    if (valueMeters.Any(m => m.Key == keyValueMeter)) fieldType = StiChartConditionalField.Value;
                    if (argumentMeters.Any(m => m.Key == keyValueMeter)) fieldType = StiChartConditionalField.Argument;
                    if (seriesMeter != null && seriesMeter.Key == keyValueMeter) fieldType = StiChartConditionalField.Series;

                    chartElement.AddChartCondition(
                        keyValueMeter,
                        (StiFilterDataType)Enum.Parse(typeof(StiFilterDataType), chartConditionObject["dataType"] as string),
                        (StiFilterCondition)Enum.Parse(typeof(StiFilterCondition), chartConditionObject["condition"] as string),
                        StiEncodingHelper.DecodeString(chartConditionObject["value"] as string),
                        StrToColor(chartConditionObject["color"] as string),
                        (StiMarkerType)Enum.Parse(typeof(StiMarkerType), chartConditionObject["markerType"] as string),
                        (float)StrToDouble(chartConditionObject["markerAngle"] as string),
                        fieldType,
                        Convert.ToBoolean(chartConditionObject["isExpression"])
                    );
                }
            }
        }
        #endregion

        #region TrendLines
        public static void SetChartTrendLinesProperty(object component, object propertyValue)
        {
            var chartElement = component as IStiChartElement;
            if (chartElement != null && propertyValue != null)
            {
                chartElement.ClearTrendLines();
                var trendLinesArray = propertyValue as ArrayList;
                foreach (Hashtable trendLineObject in trendLinesArray)
                {
                    chartElement.AddTrendLines(
                        trendLineObject["keyValueMeter"] as string,
                        (StiChartTrendLineType)Enum.Parse(typeof(StiChartTrendLineType), trendLineObject["type"] as string),
                        StrToColor(trendLineObject["lineColor"] as string),
                        (StiPenStyle)StrToInt(trendLineObject["lineStyle"] as string),
                        (float)StrToDouble(trendLineObject["lineWidth"] as string)
                    );
                }
            }
        }
        #endregion

        #region IndicatorConditions
        public static void SetIndicatorConditionsProperty(object component, object propertyValue)
        {
            var indicatorElement = component as IStiIndicatorElement;
            if (indicatorElement != null && propertyValue != null)
            {
                indicatorElement.ClearIndicatorConditions();
                var indicatorConditionsArray = propertyValue as ArrayList;
                foreach (Hashtable conditionObject in indicatorConditionsArray)
                {
                    indicatorElement.AddIndicatorCondition(
                        (StiIndicatorFieldCondition)Enum.Parse(typeof(StiIndicatorFieldCondition), conditionObject["field"] as string),
                        (StiFilterCondition)Enum.Parse(typeof(StiFilterCondition), conditionObject["condition"] as string),
                        StiEncodingHelper.DecodeString(conditionObject["value"] as string),
                        (StiFontIcons)Enum.Parse(typeof(StiFontIcons), conditionObject["icon"] as string),
                        StrToColor(conditionObject["iconColor"] as string),
                        (StiFontIcons)Enum.Parse(typeof(StiFontIcons), conditionObject["targetIcon"] as string),
                        StrToColor(conditionObject["targetIconColor"] as string),
                        !String.IsNullOrEmpty(conditionObject["customIcon"] as string) ? Base64ToImageByteArray((string)conditionObject["customIcon"]) : null,
                        (StiIconAlignment)Enum.Parse(typeof(StiIconAlignment), conditionObject["iconAlignment"] as string),
                        (StiIconAlignment)Enum.Parse(typeof(StiIconAlignment), conditionObject["targetIconAlignment"] as string),
                        StrToIndicatorConditionsPermissions(conditionObject["permissions"] as string),
                        StrToFont(conditionObject["font"] as string),
                        StrToColor(conditionObject["textColor"] as string),
                        StrToColor(conditionObject["backColor"] as string)
                    );
                }
            }
        }
        #endregion

        #region ProgressConditions
        public static void SetProgressConditionsProperty(object component, object propertyValue)
        {
            var progressElement = component as IStiProgressElement;
            if (progressElement != null && propertyValue != null)
            {
                progressElement.ClearProgressConditions();
                var progressConditionsArray = propertyValue as ArrayList;
                foreach (Hashtable conditionObject in progressConditionsArray)
                {
                    progressElement.AddProgressCondition(
                        (StiProgressFieldCondition)Enum.Parse(typeof(StiProgressFieldCondition), conditionObject["field"] as string),
                        (StiFilterCondition)Enum.Parse(typeof(StiFilterCondition), conditionObject["condition"] as string),
                        StiEncodingHelper.DecodeString(conditionObject["value"] as string),
                        StrToProgressConditionsPermissions(conditionObject["permissions"] as string),
                        StrToFont(conditionObject["font"] as string),
                        StrToColor(conditionObject["textColor"] as string),
                        StrToColor(conditionObject["color"] as string),
                        StrToColor(conditionObject["trackColor"] as string)
                    );
                }
            }
        }
        #endregion

        #region ProgressConditions
        public static void SetTableConditionsProperty(object component, object propertyValue)
        {
            var tableElement = component as IStiTableElement;
            if (tableElement != null && propertyValue != null)
            {
                tableElement.TableConditions.Clear();

                var tableConditionsArray = propertyValue as ArrayList;
                foreach (Hashtable conditionObject in tableConditionsArray)
                {
                    tableElement.AddTableCondition(
                        (conditionObject["keyDataFieldMeters"] as ArrayList).ToArray(typeof(string)) as string[],
                        (conditionObject["keyDestinationMeters"] as ArrayList).ToArray(typeof(string)) as string[],
                        (StiFilterDataType)Enum.Parse(typeof(StiFilterDataType), conditionObject["dataType"] as string),
                        (StiFilterCondition)Enum.Parse(typeof(StiFilterCondition), conditionObject["condition"] as string),
                        StiEncodingHelper.DecodeString(conditionObject["value"] as string),
                        StrToTableConditionsPermissions(conditionObject["permissions"] as string),
                        StrToFont(conditionObject["font"] as string),
                        StrToColor(conditionObject["foreColor"] as string),
                        StrToColor(conditionObject["backColor"] as string),
                        Convert.ToBoolean(conditionObject["isExpression"])
                    );
                }
            }
        }
        #endregion

        #region PivotTableConditions
        public static void SetPivotTableConditionsProperty(object component, object propertyValue)
        {
            var pivotTableElement = component as IStiPivotTableElement;
            if (pivotTableElement != null && propertyValue != null)
            {
                pivotTableElement.PivotTableConditions.Clear();
                var pivotTableConditionsArray = propertyValue as ArrayList;
                foreach (Hashtable pivotTableConditionObject in pivotTableConditionsArray)
                {
                    var customIcon = pivotTableConditionObject["customIcon"] as string;
                    byte[] bytes = null;
                    if (!String.IsNullOrEmpty(customIcon))
                    {
                        try
                        {
                            bytes = Base64ToImageByteArray(customIcon);
                            double maxIconSize = 50;
                            if (!Base.Helpers.StiSvgHelper.IsSvg(bytes))
                            {
                                using (var stream = new MemoryStream(bytes))
                                using (var image = Image.FromStream(stream))
                                {
                                    var factor = Math.Min(1d, maxIconSize / Math.Max(image.Width, image.Height));
                                    if (factor < 1)
                                    {
                                        using (var newImage = StiImageUtils.ResizeImage(image, (int)Math.Round(image.Width * factor), (int)Math.Round(image.Height * factor)))
                                        {
                                            bytes = StiImageConverter.ImageToBytes(newImage);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var document = Base.Helpers.StiSvgHelper.OpenSvg(bytes);
                                var svgSize = Base.Helpers.StiSvgHelper.GetSvgSize(document);
                                var factor = Math.Min(1d, maxIconSize / Math.Max(svgSize.Width, svgSize.Height));
                                var imageWidth = (int)Math.Round(svgSize.Width * factor);
                                var imageHeight = (int)Math.Round(svgSize.Height * factor);
                                using (var image = Base.Helpers.StiSvgHelper.ConvertSvgToImage(bytes, imageWidth, imageHeight, true, true))
                                {
                                    bytes = StiImageConverter.ImageToBytes(image);
                                }
                            }
                        }
                        catch (Exception) { }
                    }

                    pivotTableElement.AddPivotTableCondition(
                        pivotTableConditionObject["keyValueMeter"] as string,
                        (StiFilterDataType)Enum.Parse(typeof(StiFilterDataType), pivotTableConditionObject["dataType"] as string),
                        (StiFilterCondition)Enum.Parse(typeof(StiFilterCondition), pivotTableConditionObject["condition"] as string),
                        StiEncodingHelper.DecodeString(pivotTableConditionObject["value"] as string),
                        StrToFont(pivotTableConditionObject["font"] as string),
                        StrToColor(pivotTableConditionObject["textColor"] as string),
                        StrToColor(pivotTableConditionObject["backColor"] as string),
                        StrPermissionsToConditionPermissionsObject(pivotTableConditionObject["permissions"] as string),
                        (StiFontIcons)Enum.Parse(typeof(StiFontIcons), pivotTableConditionObject["icon"] as string),
                        (StiIconAlignment)Enum.Parse(typeof(StiIconAlignment), pivotTableConditionObject["iconAlignment"] as string),
                        bytes,
                        StrToColor(pivotTableConditionObject["iconColor"] as string));
                }
            }
        }
        #endregion

        #region Expressions
        public static void SetExpressionsProperty(StiComponent component, object propertyValue)
        {
            var expressionsJS = propertyValue as Hashtable;
            var appExpessions = component as IStiAppExpressionCollection;

            if (appExpessions != null && expressionsJS != null)
            {
                appExpessions.Expressions.Clear();
                foreach (DictionaryEntry expressionJS in expressionsJS)
                {
                    StiAppExpressionHelper.SetExpression(component, UpperFirstChar(expressionJS.Key as string), StiEncodingHelper.DecodeString(expressionJS.Value as string));
                }
                StiAppExpressionParser.ProcessExpressions(component, false);
            }
        }
        #endregion

        #region Styles
        public static void SetStylesProperty(object component, object propertyValue)
        {
            var stylesJS = propertyValue as ArrayList;
            var stylesProperty = component.GetType().GetProperty("Styles")?.GetValue(component, null) as StiStylesCollection;

            if (stylesJS != null && stylesProperty != null)
            {
                stylesProperty.Clear();

                foreach (Hashtable styleObject in stylesJS)
                {
                    Assembly assembly = typeof(StiReport).Assembly;
                    StiBaseStyle style = assembly.CreateInstance("Stimulsoft.Report." + (string)styleObject["type"]) as StiBaseStyle;

                    if (style != null)
                    {
                        var properties = styleObject["properties"] as Hashtable;

                        if (properties != null && !String.IsNullOrEmpty(properties["name"] as string))
                            style.Name = properties["name"] as string;
                        else
                            StiStylesHelper.GenerateNewName(stylesProperty, style);
                        
                        StiStylesHelper.ApplyStyleProperties(style, properties);
                        stylesProperty.Add(style);
                    }
                }
            }
        }
        #endregion

        #region DashboardWatermark
        private static void SetDashboardWatermarkProperty(object component, object propertyValue)
        {
            var watermark = (component as IStiDashboardWatermark)?.DashboardWatermark;
            if (watermark != null)
            {
                var watermarkJS = propertyValue as Hashtable;
                watermark.TextEnabled = Convert.ToBoolean(watermarkJS["textEnabled"]);
                watermark.Text = StiEncodingHelper.DecodeString(watermarkJS["text"] as string);
                watermark.TextFont = StrToFont(watermarkJS["textFont"] as string);
                watermark.TextColor = StrToColor(watermarkJS["textColor"] as string);
                watermark.TextAngle = Convert.ToInt32(watermarkJS["textAngle"]);

                watermark.ImageEnabled = Convert.ToBoolean(watermarkJS["imageEnabled"]);
                watermark.Image = Base64ToImage(watermarkJS["image"] as string);
                watermark.ImageAlignment = (ContentAlignment)Enum.Parse(typeof(ContentAlignment), watermarkJS["imageAlignment"] as string);
                watermark.ImageTransparency = Convert.ToInt32(watermarkJS["imageTransparency"]);
                watermark.ImageMultipleFactor = StrToDouble(watermarkJS["imageMultipleFactor"] as string);
                watermark.ImageAspectRatio = Convert.ToBoolean(watermarkJS["imageAspectRatio"]);
                watermark.ImageStretch = Convert.ToBoolean(watermarkJS["imageStretch"]);
                watermark.ImageTiling = Convert.ToBoolean(watermarkJS["imageTiling"]);

                watermark.WeaveEnabled = Convert.ToBoolean(watermarkJS["weaveEnabled"]);
                watermark.WeaveMajorIcon = !string.IsNullOrEmpty(watermarkJS["weaveMajorIcon"] as string) ? (StiFontIcons?)Enum.Parse(typeof(StiFontIcons), watermarkJS["weaveMajorIcon"] as string) : null;
                watermark.WeaveMajorColor = StrToColor(watermarkJS["weaveMajorColor"] as string);
                watermark.WeaveMajorSize = Convert.ToInt32(watermarkJS["weaveMajorSize"]);
                watermark.WeaveMinorIcon = !string.IsNullOrEmpty(watermarkJS["weaveMinorIcon"] as string) ? (StiFontIcons?)Enum.Parse(typeof(StiFontIcons), watermarkJS["weaveMinorIcon"] as string) : null;
                watermark.WeaveMinorColor = StrToColor(watermarkJS["weaveMinorColor"] as string);
                watermark.WeaveMinorSize = Convert.ToInt32(watermarkJS["weaveMinorSize"]);
                watermark.WeaveDistance = Convert.ToInt32(watermarkJS["weaveDistance"]);
                watermark.WeaveAngle = Convert.ToInt32(watermarkJS["weaveAngle"]);
            }
        }
        #endregion
        #endregion

        #region CallBack methods
        public static string WriteReportToJsObject(StiReport report, double zoom = 0)
        {
            if (zoom != 0) report.Info.Zoom = zoom;
            StiDesignReportHelper reportHelper = new StiDesignReportHelper(report);
            string jsonReport = JSON.Encode(reportHelper.GetReportJsObject());
            report.Info.Zoom = 1;

            return jsonReport;
        }

        public static string WriteReportToJsObject(StiReport report, Hashtable attachedItems, double zoom = 0)
        {
            if (zoom != 0) report.Info.Zoom = zoom;
            StiDesignReportHelper reportHelper = new StiDesignReportHelper(report);
            Hashtable reportObject = reportHelper.GetReportJsObject();
            if (attachedItems != null && reportObject["dictionary"] != null) ((Hashtable)reportObject["dictionary"])["attachedItems"] = attachedItems;
            string jsonReport = JSON.Encode(reportObject);
            report.Info.Zoom = 1;

            return jsonReport;
        }

        public static void CreateComponent(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            var currentPageName = (string)param["pageName"];
            var typeComponent = (string)param["typeComponent"];
            var componentRect = ((string)param["componentRect"]).Split('!');
            var zoom = StrToDouble((string)param["zoom"]);
            var additionalParams = param["additionalParams"] as Hashtable;

            StiComponent newComponent = null;
            switch (typeComponent)
            {
                //Components
                case "StiText": { newComponent = new StiText(); break; }
                case "StiTextInCells": { newComponent = new StiTextInCells(); break; }
                case "StiRichText": { newComponent = new StiRichText(); break; }
                case "StiImage": { newComponent = new StiImage(); break; }
                case "StiBarCode": { newComponent = new StiBarCode(); break; }
                case "StiShape": { newComponent = new StiShape(); break; }
                case "StiPanel": { newComponent = new StiPanel(); break; }
                case "StiClone": { newComponent = new StiClone(); break; }
                case "StiCheckBox": { newComponent = new StiCheckBox(); break; }
                case "StiSubReport": { newComponent = new StiSubReport(); break; }
                case "StiZipCode": { newComponent = new StiZipCode(); break; }
                case "StiChart": { newComponent = new StiChart(); break; }
                case "StiSparkline": { newComponent = new StiSparkline(); break; }
                case "StiMathFormula": { newComponent = new StiMathFormula(); break; }
                case "StiElectronicSignature": { newComponent = new StiElectronicSignature(); break; }
                case "StiPdfDigitalSignature": { newComponent = new StiPdfDigitalSignature(); break; }
                case "StiMap": { newComponent = new StiMap(); break; }
                case "StiGauge":
                    {
                        newComponent = new StiGauge();
                        Gauge.Helpers.StiGaugeV2InitHelper.Init((StiGauge)newComponent, StiGaugeType.FullCircular, true, false);
                        break;
                    }
                case "StiTableCell": { newComponent = new StiTableCell(); break; }
                case "StiTableCellImage": { newComponent = new StiTableCellImage(); break; }
                case "StiTableCellCheckBox": { newComponent = new StiTableCellCheckBox(); break; }
                case "StiTableCellRichText": { newComponent = new StiTableCellRichText(); break; }

                //Primitives
                case "StiRectanglePrimitive": { newComponent = new StiRectanglePrimitive(); break; }
                case "StiRoundedRectanglePrimitive": { newComponent = new StiRoundedRectanglePrimitive(); break; }
                case "StiHorizontalLinePrimitive": { newComponent = new StiHorizontalLinePrimitive(); break; }
                case "StiVerticalLinePrimitive": { newComponent = new StiVerticalLinePrimitive(); break; }

                //Bands
                case "StiReportTitleBand": { newComponent = new StiReportTitleBand(); break; }
                case "StiReportSummaryBand": { newComponent = new StiReportSummaryBand(); break; }
                case "StiPageHeaderBand": { newComponent = new StiPageHeaderBand(); break; }
                case "StiPageFooterBand": { newComponent = new StiPageFooterBand(); break; }
                case "StiGroupHeaderBand": { newComponent = new StiGroupHeaderBand(); break; }
                case "StiGroupFooterBand": { newComponent = new StiGroupFooterBand(); break; }
                case "StiHeaderBand": { newComponent = new StiHeaderBand(); break; }
                case "StiFooterBand": { newComponent = new StiFooterBand(); break; }
                case "StiColumnHeaderBand": { newComponent = new StiColumnHeaderBand(); break; }
                case "StiColumnFooterBand": { newComponent = new StiColumnFooterBand(); break; }
                case "StiDataBand": { newComponent = new StiDataBand(); break; }
                case "StiTable":
                    {
                        newComponent = new StiTable();
                        if (additionalParams != null)
                        {
                            (newComponent as StiTable).RowCount = Convert.ToInt32(additionalParams["rowCount"]);
                            (newComponent as StiTable).ColumnCount = Convert.ToInt32(additionalParams["columnCount"]);
                        }
                        break;
                    }
                case "StiHierarchicalBand": { newComponent = new StiHierarchicalBand(); break; }
                case "StiChildBand": { newComponent = new StiChildBand(); break; }
                case "StiEmptyBand": { newComponent = new StiEmptyBand(); break; }
                case "StiOverlayBand": { newComponent = new StiOverlayBand(); break; }
                case "StiTableOfContents": { newComponent = new StiTableOfContents(); break; }

                //Cross Bands
                case "StiCrossTab": { newComponent = new StiCrossTab(); break; }
                case "StiCrossGroupHeaderBand": { newComponent = new StiCrossGroupHeaderBand(); break; }
                case "StiCrossGroupFooterBand": { newComponent = new StiCrossGroupFooterBand(); break; }
                case "StiCrossHeaderBand": { newComponent = new StiCrossHeaderBand(); break; }
                case "StiCrossFooterBand": { newComponent = new StiCrossFooterBand(); break; }
                case "StiCrossDataBand": { newComponent = new StiCrossDataBand(); break; }

                //Dashboards elements
                case "StiTableElement":
                case "StiChartElement":
                case "StiGaugeElement":
                case "StiPivotTableElement":
                case "StiIndicatorElement":
                case "StiProgressElement":
                case "StiRegionMapElement":
                case "StiOnlineMapElement":
                case "StiImageElement":
                case "StiTextElement":
                case "StiPanelElement":
                case "StiListBoxElement":
                case "StiComboBoxElement":
                case "StiTreeViewElement":
                case "StiTreeViewBoxElement":
                case "StiDatePickerElement":
                case "StiCardsElement":
                case "StiButtonElement":
                    {
                        newComponent = StiDashboardHelper.CreateDashboardElement(report, typeComponent);
                        break;
                    }
            }

            if (typeComponent.StartsWith("Infographic"))
            {
                newComponent = CreateInfographicComponent(typeComponent, report, param);
                if (newComponent != null) typeComponent = newComponent.GetType().Name;
            }
            else if (typeComponent.StartsWith("StiShape;"))
            {
                newComponent = CreateShapeComponent(typeComponent);
                if (newComponent != null) typeComponent = newComponent.GetType().Name;
            }            
            else if (typeComponent.StartsWith("StiBarCode;"))
            {
                newComponent = CreateBarCodeComponent(typeComponent);
                if (newComponent != null) typeComponent = newComponent.GetType().Name;
            }
            //Dashboards shape element
            else if (typeComponent.StartsWith("StiShapeElement;"))
            {                
                newComponent = StiShapeElementHelper.CreateShapeElement(typeComponent);
                if (newComponent != null) typeComponent = newComponent.GetType().Name;
            }

            if (newComponent == null) return;

            StiPage currentPage = report.Pages[currentPageName];
            AddComponentToPage(newComponent, currentPage);

            double compWidth = StrToDouble(componentRect[2]);
            double compHeight = StrToDouble(componentRect[3]);

            if (newComponent is IStiElement && compWidth < 10 && compHeight < 10)
            {
                compWidth = newComponent.DefaultClientRectangle.Size.Width;
                compHeight = newComponent.DefaultClientRectangle.Size.Height;
            }
            else if (report.Unit.ConvertToHInches(compWidth) < 10 && report.Unit.ConvertToHInches(compHeight) < 10)
            {
                compWidth = report.Unit.ConvertFromHInches(newComponent.DefaultClientRectangle.Size.Width);
                compHeight = report.Unit.ConvertFromHInches(newComponent.DefaultClientRectangle.Size.Height);
            }

            var compRect = new RectangleD(new PointD(StrToDouble(componentRect[0]), StrToDouble(componentRect[1])), new SizeD(compWidth, compHeight));

            if (additionalParams != null && Convert.ToBoolean(additionalParams["createdByDblClick"]) && !(newComponent is StiBand))
            {
                compRect = new RectangleD(GetFreePointOnPage(currentPage, compWidth, compHeight), new SizeD(compWidth, compHeight));

                if (additionalParams["currentComponent"] != null)
                {
                    var selectedComp = currentPage.Report.GetComponentByName(additionalParams["currentComponent"] as string) as StiBand;
                    if (selectedComp != null)
                    {
                        if (IsCrossBand(selectedComp))
                            compRect = GetComponentRectForInsertingToCrossBand(selectedComp);
                        else
                            compRect = GetComponentRectForInsertingToBand(selectedComp);
                    }
                }
            }

            SetComponentRect(newComponent, compRect);

            if (newComponent is StiCrossLinePrimitive)
                AddPrimitivePoints(newComponent, currentPage);

            if (newComponent is StiSubReport)
                AddSubReportPage(newComponent as StiSubReport, callbackResult, param);

            if (param["lastStyleProperties"] != null && !(newComponent is IStiElement) && !(newComponent is StiMap) && !(newComponent is StiChart))
                SetAllProperties(newComponent, param["lastStyleProperties"] as ArrayList);

            callbackResult["name"] = newComponent.Name;
            callbackResult["typeComponent"] = typeComponent;
            callbackResult["componentRect"] = GetComponentRect(newComponent);
            callbackResult["parentName"] = GetParentName(newComponent);
            callbackResult["parentIndex"] = GetParentIndex(newComponent).ToString();
            callbackResult["componentIndex"] = GetComponentIndex(newComponent).ToString();
            callbackResult["childs"] = GetAllChildComponents(newComponent);
            callbackResult["svgContent"] = GetSvgContent(newComponent, zoom);
            callbackResult["pageName"] = currentPage.Name;
            callbackResult["properties"] = GetAllProperties(newComponent);
            callbackResult["rebuildProps"] = GetPropsRebuildPage(report, currentPage);
            callbackResult["largeHeightAutoFactor"] = currentPage.LargeHeightAutoFactor.ToString();
            if (newComponent is StiTable) callbackResult["tableCells"] = GetTableCells(newComponent as StiTable, zoom);
            if (additionalParams != null) callbackResult["additionalParams"] = additionalParams;
        }

        private static RectangleD GetComponentRectForInsertingToBand(StiBand band)
        {
            var posX = 0d;
            var posY = band.Top;
            var parent = band.Parent;

            band.Components.ToList().ForEach(comp =>
            {
                if (posX < comp.Right)
                    posX = comp.Right;
            });

            posX += band.Left;

            while (parent != null && !(parent is StiPage)) 
            {
                posX += parent.Left;
                posY += parent.Top;
                parent = parent.Parent;
            }

            return new RectangleD(posX, posY, StiAlignValue.AlignToMaxGrid(band.Width / 8, band.Page.GridSize, true), band.Height);
        }

        private static RectangleD GetComponentRectForInsertingToCrossBand(StiBand band)
        {
            var posY = 0d;
            var posX = band.Left;
            var parent = band.Parent;

            band.Components.ToList().ForEach(comp =>
            {
                if (posY < comp.Bottom)
                    posY = comp.Bottom;
            });

            posY += band.Top;

            while (parent != null && !(parent is StiPage))
            {
                posX += parent.Left;
                posY += parent.Top;
                parent = parent.Parent;
            }

            return new RectangleD(posX, posY, band.Width, StiAlignValue.AlignToMaxGrid(band.Height / 8, band.Page.GridSize, true));
        }

        private static bool IsCrossBand(StiComponent comp)
        {
            return comp is StiCrossDataBand || comp is StiCrossHeaderBand || comp is StiCrossFooterBand || comp is StiCrossGroupHeaderBand || comp is StiCrossGroupFooterBand;
        }

        private static PointD GetFreePointOnPage(StiPage page, double compWidth, double compHeight)
        {
            var point = new PointD();

            page.Components.ToList().ForEach(comp =>
            {
                if (!(comp is StiPageFooterBand))
                {
                    if (!(comp is StiBand) || IsCrossBand(comp))
                        point.X = Math.Max(point.X, comp.Left + comp.Width);

                    if (!IsCrossBand(comp))
                        point.Y = Math.Max(point.Y, comp.Top + comp.Height);
                }
            });

            point.X += page.GridSize;
            point.Y += page.GridSize;

            if (point.X + compWidth > page.Width) 
                point.X = page.Width - compWidth - page.GridSize;

            if (point.Y + compHeight > page.Height) 
                point.Y = page.Height - compHeight - page.GridSize;

            return point;
        }
        public static void RemoveComponent(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            ArrayList componentNames = (ArrayList)param["components"];

            report.IsModified = true;
            StiPage currPage = null;
            for (int i = 0; i < componentNames.Count; i++)
            {
                StiComponent currentComponent = report.Pages.GetComponentByName((string)componentNames[i]);
                if (currentComponent != null)
                {
                    if (currentComponent is StiCrossLinePrimitive)
                    {
                        RemovePrimitivePoints(currentComponent);
                    }

                    currentComponent.Parent.Components.Remove(currentComponent);
                    currPage = currentComponent.Page;
                    CheckAllPrimitivePoints(currPage);
                }
            }
            callbackResult["rebuildProps"] = GetPropsRebuildPage(report, currPage);
            callbackResult["pageName"] = currPage.Name;
            callbackResult["largeHeightAutoFactor"] = currPage.LargeHeightAutoFactor.ToString();
        }

        public static void ChangeRectComponent(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            StiPage currentPage = report.Pages[(string)param["pageName"]];
            double zoom = StrToDouble((string)param["zoom"]);
            ArrayList components = (ArrayList)param["components"];
            ArrayList componentsResultArray = new ArrayList();
            ArrayList changedComponents = new ArrayList();
            var command = param["command"] as string;

            #region Set Sequence
            //If all copied components are bands then place all bands with small steps prior to the first band.
            //In that case all inserted bands will be inserted in one sequence before the next band.
            var bands = new StiComponentsCollection();
            var pos = 0d;
            var firstBandHeight = 0d;
            var isFirstBand = true;
            for (int i = 0; i < components.Count; i++)
            {
                var compProps = (Hashtable)components[i];
                var comp = report.Pages.GetComponentByName((string)compProps["componentName"]);
                if (comp != null && comp is StiBand)
                {
                    var compRect = StrToRect((string)compProps["componentRect"]);
                    if (isFirstBand)
                    {
                        pos = compRect.Top;
                        firstBandHeight = compRect.Height;
                    }
                    isFirstBand = false;
                    bands.Add(comp);
                }
            }
            if (bands.Count > 1)
            {
                bands.ToList().Skip(1).ToList().ForEach(c =>
                {
                    for (int i = 0; i < components.Count; i++)
                    {
                        var compProps = (Hashtable)components[i];
                        if ((string)compProps["componentName"] == c.Name)
                        {
                            var compRect = StrToRect((string)compProps["componentRect"]);
                            var newRect = new RectangleD(new PointD(compRect.Left, pos), new SizeD(compRect.Width, compRect.Height));
                            compProps["componentRect"] = RectToStr(newRect);
                        }
                    }
                    pos += firstBandHeight / 10;
                });
            }
            #endregion

            for (int i = 0; i < components.Count; i++)
            {
                var compProps = (Hashtable)components[i];
                var currentComponent = report.Pages.GetComponentByName((string)compProps["componentName"]);
                changedComponents.Add(currentComponent);
                var currentComponentRect = ((string)compProps["componentRect"]).Split('!');

                var compRect = new RectangleD(
                    new PointD(StrToDouble(currentComponentRect[0]), StrToDouble(currentComponentRect[1])),
                    new SizeD(StrToDouble(currentComponentRect[2]), StrToDouble(currentComponentRect[3]))
                );

                if (currentComponent is StiTableCell || !report.Info.AlignToGrid || param["runFromProperty"] != null)
                    SetComponentRect(currentComponent, compRect, param["isUnplacedComponent"] != null ? report.Info.AlignToGrid : false, command == "MoveComponent" && currentComponent is StiBand);
                else
                    SetComponentRectWithOffset(currentComponent, compRect, param["command"] as string, param["resizeType"] as string, compProps);

                var resultProps = new Hashtable();
                resultProps["componentName"] = currentComponent.Name;
                if (((string)param["command"] == "ResizeComponent") || currentComponent.DockStyle != StiDockStyle.None)
                {
                    resultProps["svgContent"] = GetSvgContent(currentComponent, zoom);
                }

                componentsResultArray.Add(resultProps);

                if (command == "ResizeComponent" && currentComponent is StiTable)
                {
                    ((StiTable)currentComponent).DistributeRows();
                    callbackResult["cells"] = StiTableHelper.GetTableCellsProperties(currentComponent as StiTable, zoom);
                }
            }

            if (param["moveAfterCopyPaste"] != null)
            {
                foreach (StiComponent component in changedComponents)
                    component.Dockable = true;
            }

            callbackResult["components"] = componentsResultArray;
            callbackResult["pageName"] = currentPage.Name;
            callbackResult["rebuildProps"] = GetPropsRebuildPage(report, currentPage);
            callbackResult["largeHeightAutoFactor"] = currentPage.LargeHeightAutoFactor.ToString();
            callbackResult["isMultiResize"] = param["resizeType"] != null && ((string)param["resizeType"]).StartsWith("Multi");
            callbackResult["isUnplacedComponent"] = param["isUnplacedComponent"];
        }

        public static void AddPage(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            StiPage newPage = new StiPage(report);
            int currentPageIndex = int.Parse((string)param["pageIndex"]);

            if (currentPageIndex + 1 == report.Pages.Count)
                report.Pages.Add(newPage);
            else
            {
                report.Pages.Insert(currentPageIndex + 1, newPage);
                newPage.Name = StiNameCreation.CreateName(report, StiNameCreation.GenerateName(newPage));
            }

            #region LicenseKey
#if CLOUD
            var isValid = StiCloudPlan.IsReportsAvailable(report.ReportGuid);
#elif SERVER
            var isValid = !StiVersionX.IsSvr;
#else
            var isValidKey = false;
            var licenseKey = StiLicenseKeyValidator.GetLicenseKey();
            if (licenseKey != null)
            {
                try
                {
                    using (var rsa = new RSACryptoServiceProvider(512))
                    using (var sha = new SHA1CryptoServiceProvider())
                    {
                        rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                        isValidKey = rsa.VerifyData(licenseKey.GetCheckBytes(), sha, licenseKey.GetSignatureBytes());
                    }
                }
                catch (Exception)
                {
                    isValidKey = false;
                }
            }

            var isValid = isValidKey & StiLicenseKeyValidator.IsValid(StiProductIdent.Web, licenseKey) && typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key);
#endif
            #endregion

            callbackResult["name"] = newPage.Name;
            callbackResult["pageIndex"] = report.Pages.IndexOf(newPage).ToString();
            callbackResult["properties"] = GetAllProperties(newPage);
            callbackResult["pageIndexes"] = GetPageIndexes(report);

            if (isValid)
                callbackResult["valid"] = true;
        }

        public static void RemovePage(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            StiPage currentPage = report.Pages[(string)param["pageName"]];
            report.Pages.Remove(currentPage);

            callbackResult["pageName"] = currentPage.Name;
            callbackResult["pageIndexes"] = GetPageIndexes(report);
        }

        public static void ReadAllPropertiesFromString(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            bool updateAllControls = (bool)param["updateAllControls"];
            double zoom = StrToDouble((string)param["zoom"]);
            StiPage page = null;
            ArrayList components = (ArrayList)param["components"];
            ArrayList resultComponents = new ArrayList();
            Hashtable resultProperties = new Hashtable();

            for (int i = 0; i < components.Count; i++)
            {
                Hashtable componentProps = (Hashtable)components[i];
                string componentType = (string)componentProps["typeComponent"];
                string componentName = (string)componentProps["componentName"];
                ArrayList allProperties = (ArrayList)componentProps["properties"];
                Hashtable resultCompProps = new Hashtable();
                resultComponents.Add(resultCompProps);

                if (componentType == "StiPage")
                {
                    page = report.Pages[componentName];
                    if (componentProps["cannotChange"] == null) SetAllProperties(page, allProperties);
                    resultProperties = GetAllProperties(page);
                    callbackResult["rebuildProps"] = GetPropsRebuildPage(report, page);
                }
                else
                {
                    StiComponent component = report.Pages.GetComponentByName(componentName);
                    if (componentProps["cannotChange"] == null) SetAllProperties(component, allProperties);
                    if (page == null) page = component.Page;

                    foreach (Hashtable property in allProperties)
                    {
                        if ((string)property["name"] == "dockStyle")
                        {
                            callbackResult["rebuildProps"] = GetPropsRebuildPage(report, page);
                            break;
                        }
                    }

                    resultProperties = GetAllProperties(component, param);
                    resultCompProps["svgContent"] = GetSvgContent(component, zoom);
                }

                resultCompProps["typeComponent"] = componentType;
                resultCompProps["componentName"] = componentName;
                resultCompProps["properties"] = resultProperties;
            }

            callbackResult["components"] = resultComponents;
            callbackResult["largeHeightAutoFactor"] = page.LargeHeightAutoFactor.ToString();
            callbackResult["pageName"] = page.Name;
            callbackResult["updateAllControls"] = updateAllControls;
        }

        public static void ChangeUnit(StiReport report, string unitName)
        {
            report.IsModified = true;
            if (unitName == "cm") report.ReportUnit = StiReportUnitType.Centimeters;
            if (unitName == "hi") report.ReportUnit = StiReportUnitType.HundredthsOfInch;
            if (unitName == "in") report.ReportUnit = StiReportUnitType.Inches;
            if (unitName == "mm") report.ReportUnit = StiReportUnitType.Millimeters;
        }

        public static void SetToClipboard(StiRequestParams requestParams, StiReport report, Hashtable param, Hashtable callbackResult)
        {
            ArrayList componentNames = (ArrayList)param["components"];
            StiPage currentPage = report.Pages[((string)param["pageName"])];

            foreach (StiComponent component in report.GetComponents())
            {
                component.IsSelected = componentNames.Contains(component.Name) ? true : false;
            }

            var group = StiGroup.GetGroupFromPage(currentPage);
            var clipboardResult = group.ToString("StiReport Clipboard");
            requestParams.Cache.Helper.SaveObjectInternal(clipboardResult, requestParams, StiCacheHelper.GUID_Clipboard);

            callbackResult["clipboardResult"] = "stiClipboard_" + StiEncodingHelper.Encode(clipboardResult);

            foreach (StiComponent component in report.GetComponents())
                component.IsSelected = false;
        }

        public static void GetFromClipboard(StiRequestParams requestParams, StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var components = new ArrayList();
            var currentPage = report.Pages[((string)param["pageName"])];
            var zoom = StrToDouble((string)param["zoom"]);
            var clipboardResult = param["clipboardResult"] as string;
            
            string text = !string.IsNullOrEmpty(clipboardResult) && clipboardResult.StartsWith("stiClipboard_") 
                ? StiEncodingHelper.DecodeString(clipboardResult.Replace("stiClipboard_", ""))
                : requestParams.Cache.Helper.GetObjectInternal(requestParams, StiCacheHelper.GUID_Clipboard) as string;

            if (!string.IsNullOrEmpty(text))
            {
                var group = StiGroup.CreateFromString(text, "StiReport Clipboard");

                //check components
                group.Components.ToList().ForEach(c =>
                {
                    if ((c is IStiElement && !currentPage.IsDashboard) || (!(c is IStiElement) && currentPage.IsDashboard))
                        group.Components.Remove(c);
                });

                StiInsertionComponents.InsertGroups(currentPage, group);

                foreach (StiComponent component in group.GetComponents())
                {
                    if (component is StiCrossLinePrimitive)
                        AddPrimitivePoints(component, currentPage);

                    Hashtable attributes = new Hashtable();
                    attributes["name"] = component.Name;
                    attributes["typeComponent"] = component.GetType().Name;
                    attributes["componentRect"] = GetComponentRect(component);
                    attributes["parentName"] = GetParentName(component);
                    attributes["parentIndex"] = GetParentIndex(component).ToString();
                    attributes["componentIndex"] = GetComponentIndex(component).ToString();
                    attributes["childs"] = GetAllChildComponents(component);
                    attributes["svgContent"] = GetSvgContent(component, zoom);
                    attributes["pageName"] = currentPage.Name;
                    attributes["properties"] = GetAllProperties(component);
                    attributes["largeHeightAutoFactor"] = currentPage.LargeHeightAutoFactor.ToString();

                    components.Add(attributes);
                }
            }

            callbackResult["rebuildProps"] = GetPropsRebuildPage(report, currentPage);
            callbackResult["components"] = components;
        }

        public static StiReport GetUndoStep(StiRequestParams requestParams, StiReport currentReport, Hashtable param, Hashtable callbackResult)
        {
            ArrayList undoArray = requestParams.Cache.Helper.GetObjectInternal(requestParams, StiCacheHelper.GUID_UndoArray) as ArrayList;
            if (undoArray != null)
            {
                int currentPos = Convert.ToInt32(undoArray[0]);
                if (currentPos == undoArray.Count - 1) undoArray[currentPos] = new StiReportContainer(StiReportCopier.CloneReport(currentReport, true), true, StiDesignerCommand.Undo);
                if (currentPos > 1) currentPos--;

                StiReportContainer reportContainer = (StiReportContainer)undoArray[currentPos];
                StiReport report = reportContainer.report;
                report.Info.Zoom = 1;
                report.Info.ForceDesigningMode = true;

                if (param["reportFile"] != null) report.ReportFile = (string)param["reportFile"];

                if (!reportContainer.resourcesIncluded)
                    StiReportResourceHelper.LoadResourcesToReport(report, currentReport.Dictionary.Resources);

                currentReport = report;
                StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(StiDesignerOptionsHelper.GetDesignerOptions(requestParams), report);
                callbackResult["reportObject"] = WriteReportToJsObject(report, StrToDouble((string)param["zoom"]));
                callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
                callbackResult["enabledUndoButton"] = currentPos > 1;
                undoArray[0] = currentPos;
            }
            callbackResult["selectedObjectName"] = param["selectedObjectName"];
            requestParams.Cache.Helper.SaveObjectInternal(undoArray, requestParams, StiCacheHelper.GUID_UndoArray);

            return currentReport;
        }

        public static StiReport GetRedoStep(StiRequestParams requestParams, StiReport currentReport, Hashtable param, Hashtable callbackResult)
        {
            ArrayList undoArray = requestParams.Cache.Helper.GetObjectInternal(requestParams, StiCacheHelper.GUID_UndoArray) as ArrayList;
            if (undoArray != null)
            {
                int currentPos = Convert.ToInt32(undoArray[0]);
                if (currentPos + 1 < undoArray.Count) currentPos++;

                StiReportContainer reportContainer = (StiReportContainer)undoArray[currentPos];
                StiReport report = reportContainer.report;
                report.Info.Zoom = 1;
                report.Info.ForceDesigningMode = true;

                if (param["reportFile"] != null) report.ReportFile = (string)param["reportFile"];

                if (!reportContainer.resourcesIncluded)
                    StiReportResourceHelper.LoadResourcesToReport(report, currentReport.Dictionary.Resources);

                StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(StiDesignerOptionsHelper.GetDesignerOptions(requestParams), report);
                callbackResult["reportObject"] = WriteReportToJsObject(report, StrToDouble((string)param["zoom"]));
                callbackResult["enabledRedoButton"] = currentPos + 1 < undoArray.Count;
                currentReport = report;
                callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
                undoArray[0] = currentPos;
            }
            callbackResult["selectedObjectName"] = param["selectedObjectName"];
            requestParams.Cache.Helper.SaveObjectInternal(undoArray, requestParams, StiCacheHelper.GUID_UndoArray);

            return currentReport;
        }

        public static void RenameComponent(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            string typeComponent = (string)param["typeComponent"];
            string oldName = (string)param["oldName"];
            string newName = (string)param["newName"];

            if (typeComponent == "StiPage")
            {
                StiPage page = report.Pages[oldName];
                if (report.Pages[newName] == null)
                {
                    page.Name = newName;
                    callbackResult["rebuildProps"] = GetPropsRebuildPage(report, page);
                    newName = page.Name;
                }
                else newName = oldName;
            }
            else
            {
                StiComponent component = report.Pages.GetComponentByName(oldName);
                if (report.Pages.GetComponentByName(newName) == null)
                {
                    component.Name = newName;
                    callbackResult["rebuildProps"] = GetPropsRebuildPage(report, component.Page);
                    newName = component.Name;
                }
                else newName = oldName;
            }

            callbackResult["typeComponent"] = typeComponent;
            callbackResult["oldName"] = oldName;
            callbackResult["newName"] = newName;

        }

        public static void SaveComponentClone(StiRequestParams requestParams, StiComponent component)
        {
            //clone component
            StiComponent cloneComponent = component.Clone() as StiComponent;

            //Reset all parents
            cloneComponent.Report = null;
            cloneComponent.Parent = null;
            cloneComponent.Page = null;

            if (cloneComponent is StiCrossTab)
            {
                StiCrossTab crossTab = cloneComponent as StiCrossTab;
                foreach (StiComponent field in crossTab.Components)
                {
                    field.Report = null;
                    field.Page = null;
                    field.Parent = null;
                }
            }

            //Save clone component as string to cache
            var sr = new StiSerializing(new StiReportObjectStringConverter());
            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb))
            {
                sr.Serialize(cloneComponent, stringWriter, "CloneComponent", StiSerializeTypes.SerializeToAll);
                stringWriter.Close();
                requestParams.Cache.Helper.SaveObjectInternal(sb.ToString(), requestParams, StiCacheHelper.GUID_ComponentClone);
            }
        }

        public static void CanceledEditComponent(StiRequestParams requestParams, StiReport currentReport, Hashtable param, Hashtable callbackResult)
        {
            var currentComponent = currentReport.Pages.GetComponentByName((string)param["componentName"]);
            if (currentComponent == null) return;
            StiComponent cloneComponent = null;

            if (currentComponent is StiChart) cloneComponent = new StiChart();
            else if (currentComponent is IStiChartElement) cloneComponent = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Chart.StiChartElement") as StiComponent;
            else if (currentComponent is StiCrossTab) cloneComponent = new StiCrossTab();
            else if (currentComponent is StiGauge) cloneComponent = new StiGauge();
            else if (currentComponent is StiMap) cloneComponent = new StiMap();
            else if (currentComponent is StiSparkline) cloneComponent = new StiSparkline();
            else if (currentComponent is StiBarCode) cloneComponent = new StiBarCode();
            else if (currentComponent is StiShape) cloneComponent = new StiShape();
            if (cloneComponent == null) return;

            //Get component from cahce
            string componentClone = requestParams.Cache.Helper.GetObjectInternal(requestParams, StiCacheHelper.GUID_ComponentClone) as string;
            using (var stringReader = new StringReader(componentClone))
            {
                var sr = new StiSerializing(new StiReportObjectStringConverter());
                sr.Deserialize(cloneComponent, stringReader, "CloneComponent");

                stringReader.Close();

                //Restore all parents
                cloneComponent.Report = currentReport;
                cloneComponent.Parent = currentComponent.Parent;
                cloneComponent.Page = currentComponent.Page;

                if (cloneComponent is StiCrossTab)
                {
                    StiCrossTab crossTab = cloneComponent as StiCrossTab;
                    crossTab.Components.SetParent(crossTab);

                    foreach (StiComponent field in crossTab.Components)
                    {
                        field.Report = currentReport;
                        field.Page = currentComponent.Page;
                    }
                }

                //Replace current component to component from Cache
                var parent = currentComponent.Parent;
                currentComponent.Parent.Components.Remove(currentComponent);
                parent.Components.Add(cloneComponent);
            }
        }

        public static void CreateTextComponentFromDictionary(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            string currentPageName = (string)param["pageName"];
            double zoom = StrToDouble((string)param["zoom"]);
            Hashtable point = (Hashtable)param["point"];
            Hashtable itemObject = (Hashtable)param["itemObject"];
            ArrayList newComponents = new ArrayList();
            bool isParameter = (string)itemObject["typeItem"] == "Parameter";
            bool createLabel = Convert.ToBoolean(param["createLabel"]);
            bool useAliases = Convert.ToBoolean(param["useAliases"]);

            StiPage currentPage = report.Pages[currentPageName];

            StiText sampleComp = new StiText();
            double compWidth = report.Unit.ConvertFromHInches(sampleComp.DefaultClientRectangle.Size.Width);
            double compHeight = report.Unit.ConvertFromHInches(sampleComp.DefaultClientRectangle.Size.Height);
            double comp1XPos = StrToDouble((string)point["x"]);
            double comp2XPos = comp1XPos + compWidth * 2;
            double compYPos = StrToDouble((string)point["y"]);
            var countComponents = (isParameter || !createLabel) ? 1 : 2;

            if (countComponents == 1)
            {
                if (comp1XPos + currentPage.Margins.Left + compWidth * 2 > currentPage.PageWidth)
                {
                    comp1XPos = currentPage.PageWidth - currentPage.Margins.Left - compWidth * 2;
                }
            }
            else if (countComponents == 2)
            {
                if (comp2XPos + currentPage.Margins.Left + compWidth > currentPage.PageWidth)
                {
                    comp2XPos = currentPage.PageWidth - currentPage.Margins.Left - compWidth;
                    comp1XPos = comp2XPos - compWidth * 2;
                }
            }

            for (int i = 0; i < countComponents; i++)
            {
                StiText newComponent = new StiText();
                AddComponentToPage(newComponent, currentPage);

                RectangleD compRect = new RectangleD(new PointD(i == 0 ? comp1XPos : comp2XPos, compYPos), new SizeD(i == 0 ? compWidth * 2 : compWidth, compHeight));
                SetComponentRect(newComponent, compRect);

                newComponent.Text = isParameter || !createLabel || (createLabel && i != 0) ? StiEncodingHelper.DecodeString((string)itemObject["fullName"]) : (string)itemObject["name"];

                if (useAliases && createLabel && i == 0 && !string.IsNullOrEmpty(itemObject["alias"] as string))
                    newComponent.Text = itemObject["alias"] as string;

                if (param["lastStyleProperties"] != null) SetAllProperties(newComponent, param["lastStyleProperties"] as ArrayList);

                Hashtable componentProps = new Hashtable();
                componentProps["name"] = newComponent.Name;
                componentProps["typeComponent"] = "StiText";
                componentProps["componentRect"] = GetComponentRect(newComponent);
                componentProps["parentName"] = GetParentName(newComponent);
                componentProps["parentIndex"] = GetParentIndex(newComponent).ToString();
                componentProps["componentIndex"] = GetComponentIndex(newComponent).ToString();
                componentProps["childs"] = GetAllChildComponents(newComponent);
                componentProps["svgContent"] = GetSvgContent(newComponent, zoom);
                componentProps["pageName"] = currentPage.Name;
                componentProps["properties"] = GetAllProperties(newComponent);
                componentProps["rebuildProps"] = GetPropsRebuildPage(report, currentPage);

                newComponents.Add(componentProps);
            }
            callbackResult["newComponents"] = newComponents;
        }

        public static void CreateComponentFromResource(StiReport report, Hashtable param, Hashtable callbackResult)
        {            
            report.IsModified = true;
            string currentPageName = (string)param["pageName"];
            double zoom = StrToDouble((string)param["zoom"]);
            Hashtable point = (Hashtable)param["point"];
            Hashtable itemObject = (Hashtable)param["itemObject"];
            StiPage currentPage = report.Pages[currentPageName];
            StiComponent newComponent = null;
            string typeItem = itemObject["typeItem"] as string;
            string typeResource = itemObject["type"] as string;
            
            if (typeItem == "Resource" && (typeResource == "Report" || typeResource == "ReportSnapshot"))
            {
                newComponent = new StiSubReport()
                {
                    SubReportUrl = new StiHyperlinkExpression(StiHyperlinkProcessor.CreateResourceName(itemObject["name"] as string))
                };
            }
            else if (typeItem == "Resource" && (typeResource == "Rtf"))
            {
                newComponent = new StiRichText()
                {
                    DataUrl = new StiDataUrlExpression(StiHyperlinkProcessor.CreateResourceName(itemObject["name"] as string))
                };
            }
            else if (typeItem == "Resource" && typeResource == "Map")
            {
                newComponent = new StiMap();
                (newComponent as StiMap).MapIdent = itemObject["name"] as string;
            }
            else
            {
                newComponent = new StiImage();

                if (typeItem == "Variable" && itemObject["name"] != null)
                    (newComponent as StiImage).ImageData = new StiImageDataExpression("{" + itemObject["name"] as string + "}");
                else if (typeItem == "Column" && itemObject["fullName"] != null)
                {
                    string dataColumn = StiEncodingHelper.DecodeString((string)itemObject["fullName"]);
                    if (dataColumn.StartsWith("{") && dataColumn.EndsWith("}")) dataColumn = dataColumn.Substring(1, dataColumn.Length - 2);
                    (newComponent as StiImage).DataColumn = dataColumn;
                }
                else
                    (newComponent as StiImage).ImageURL = new StiImageURLExpression(StiHyperlinkProcessor.CreateResourceName(itemObject["name"] as string));
            }

            double compWidth = report.Unit.ConvertFromHInches(newComponent.DefaultClientRectangle.Size.Width);
            double compHeight = report.Unit.ConvertFromHInches(newComponent.DefaultClientRectangle.Size.Height);
            double compXPos = StrToDouble((string)point["x"]);
            double compYPos = StrToDouble((string)point["y"]);

            if (compXPos + currentPage.Margins.Left + compWidth * 2 > currentPage.PageWidth)
            {
                compXPos = currentPage.PageWidth - currentPage.Margins.Left - compWidth * 2;
            }

            AddComponentToPage(newComponent, currentPage);

            RectangleD compRect = new RectangleD(new PointD(compXPos, compYPos), new SizeD(compWidth, compHeight));
            SetComponentRect(newComponent, compRect);

            if (param["lastStyleProperties"] != null) SetAllProperties(newComponent, param["lastStyleProperties"] as ArrayList);

            Hashtable componentProps = new Hashtable();
            componentProps["name"] = newComponent.Name;
            componentProps["typeComponent"] = newComponent.GetType().Name;
            componentProps["componentRect"] = GetComponentRect(newComponent);
            componentProps["parentName"] = GetParentName(newComponent);
            componentProps["parentIndex"] = GetParentIndex(newComponent).ToString();
            componentProps["componentIndex"] = GetComponentIndex(newComponent).ToString();
            componentProps["childs"] = GetAllChildComponents(newComponent);
            componentProps["svgContent"] = GetSvgContent(newComponent, zoom);
            componentProps["pageName"] = currentPage.Name;
            componentProps["properties"] = GetAllProperties(newComponent);
            componentProps["rebuildProps"] = GetPropsRebuildPage(report, currentPage);

            callbackResult["newComponent"] = componentProps;
        }

        public static void CreateElementFromResource(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            string currentPageName = (string)param["pageName"];
            Hashtable point = (Hashtable)param["point"];
            Hashtable itemObject = (Hashtable)param["itemObject"];
            StiPage currentPage = report.Pages[currentPageName];
            string typeItem = itemObject["typeItem"] as string;
            string typeResource = itemObject["type"] as string;
            StiComponent newComponent = null;

            if (typeItem == "Resource" && typeResource == "Map")
            {
                newComponent = StiDashboardHelper.CreateDashboardElement(report, "StiRegionMapElement");
                (newComponent as IStiRegionMapElement).MapIdent = itemObject["name"] as string;
            }
            else if ((typeItem == "Resource" && typeResource == "Image") || (typeItem == "Variable" && typeResource == "image"))
            {
                newComponent = StiDashboardHelper.CreateDashboardElement(report, "StiImageElement");
                (newComponent as IStiImageElement).ImageHyperlink = StiHyperlinkProcessor.CreateResourceName(itemObject["name"] as string);
            }
            else if (typeItem == "Column" && (typeResource == "image" || typeResource == "byte[]"))
            {
                newComponent = StiDashboardHelper.CreateDashboardElement(report, "StiImageElement");
                var expression = StiEncodingHelper.DecodeString(itemObject["fullName"] as string);
                if (expression.StartsWith("{") && expression.EndsWith("}")) expression = expression.Substring(1, expression.Length - 2);
                (newComponent as IStiImageElement).ImageHyperlink = $"datacolumn://{expression}";
            }
            else
            {
                newComponent = StiDashboardHelper.CreateDashboardElement(report, "StiTextElement");
            }

            var compWidth = newComponent.DefaultClientRectangle.Size.Width;
            var compHeight = newComponent.DefaultClientRectangle.Size.Height;
            var compXPos = StiReportEdit.StrToDouble((string)point["x"]);
            var compYPos = StiReportEdit.StrToDouble((string)point["y"]);

            if (compXPos + currentPage.Margins.Left + compWidth * 2 > currentPage.Width)
            {
                compXPos = currentPage.Width - currentPage.Margins.Left - compWidth * 2;
            }

            AddComponentToPage(newComponent, currentPage);

            var compRect = new RectangleD(new PointD(compXPos, compYPos), new SizeD(compWidth, compHeight));
            StiReportEdit.SetComponentRect(newComponent, compRect);

            Hashtable componentProps = new Hashtable();
            componentProps["name"] = newComponent.Name;
            componentProps["typeComponent"] = newComponent.GetType().Name;
            componentProps["componentRect"] = StiReportEdit.GetComponentRect(newComponent);
            componentProps["parentName"] = StiReportEdit.GetParentName(newComponent);
            componentProps["parentIndex"] = StiReportEdit.GetParentIndex(newComponent).ToString();
            componentProps["componentIndex"] = StiReportEdit.GetComponentIndex(newComponent).ToString();
            componentProps["childs"] = StiReportEdit.GetAllChildComponents(newComponent);
            componentProps["svgContent"] = StiReportEdit.GetSvgContent(newComponent);
            componentProps["pageName"] = currentPage.Name;
            componentProps["properties"] = StiReportEdit.GetAllProperties(newComponent);
            componentProps["rebuildProps"] = StiReportEdit.GetPropsRebuildPage(report, currentPage);

            callbackResult["newComponent"] = componentProps;
        }

        private static double AlignToMaxGrid(StiPage page, double value, bool converted)
        {
            if (converted) value = page.Unit.ConvertFromHInches(value);
            return StiAlignValue.AlignToMaxGrid(value,
                page.GridSize, page.Report.Info.AlignToGrid);
        }

        private static double AlignToGrid(StiPage page, double value, bool converted)
        {
            if (converted) value = page.Unit.ConvertFromHInches(value);
            return StiAlignValue.AlignToGrid(value,
                page.GridSize, page.Report.Info.AlignToGrid);
        }

        public static void CreateDataComponentFromDictionary(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;

            var currentPageName = (string)param["pageName"];
            var point = (Hashtable)param["point"];
            var dataSource = (Hashtable)param["dataSource"];
            var settings = (Hashtable)param["settings"];
            var columns = (ArrayList)param["columns"];

            var currentPage = report.Pages[currentPageName];
            var cursorPoint = new PointD(StrToDouble((string)point["x"]), StrToDouble((string)point["y"]));
            var allComps = new List<StiComponent>();

            if ((bool)settings["data"] || columns.Count == 0)
            {
                #region DataBand

                StiDataBand dataBand = new StiDataBand();
                string dataSourceName = (string)dataSource["name"];
                dataBand.Name = StiNameValidator.CorrectName("Data" + dataSourceName);
                dataBand.Name = StiNameCreation.CreateName(report, dataBand.Name, false, false, true);
                allComps.Add(dataBand);

                AddComponentToPage(dataBand, currentPage);

                double compWidth = report.Unit.ConvertFromHInches(dataBand.DefaultClientRectangle.Size.Width);
                double compHeight = report.Unit.ConvertFromHInches(dataBand.DefaultClientRectangle.Size.Height);
                RectangleD compRect = new RectangleD(cursorPoint, new SizeD(compWidth, compHeight));
                SetComponentRect(dataBand, compRect);

                if ((string)dataSource["typeItem"] == "DataSource")
                    SetDataSourceProperty(dataBand, dataSourceName);
                else
                    SetBusinessObjectProperty(dataBand, (string)dataSource["fullName"]);

                double width = dataBand.Parent is StiPage ? dataBand.Page.GetColumnWidth() : dataBand.Parent.Width;

                #region Create Header
                StiHeaderBand headerBand = null;
                if ((bool)settings["header"])
                {
                    headerBand = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.Bands.HeaderBand) as StiHeaderBand;
                    string baseName = dataBand.Report.Info.GenerateLocalizedName ? headerBand.LocalizedName + dataSourceName : "Header" + dataSourceName;
                    headerBand.Name = StiNameCreation.CreateName(dataBand.Report, StiNameValidator.CorrectName(baseName), false, false, true);
                    headerBand.Height = AlignToMaxGrid(dataBand.Page, 30, true);
                    int index = dataBand.Parent.Components.IndexOf(dataBand);
                    dataBand.Parent.Components.Insert(index, headerBand);
                    allComps.Add(headerBand);
                }
                #endregion

                #region Create Footer
                StiFooterBand footerBand = null;
                if ((bool)settings["footer"])
                {
                    footerBand = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.Bands.FooterBand) as StiFooterBand;
                    string baseName = dataBand.Report.Info.GenerateLocalizedName ? footerBand.LocalizedName + dataSourceName : "Footer" + dataSourceName;
                    footerBand.Name = StiNameCreation.CreateName(dataBand.Report, StiNameValidator.CorrectName(baseName), false, false, true);
                    footerBand.Height = AlignToMaxGrid(dataBand.Page, 30, true);
                    int index = dataBand.Parent.Components.IndexOf(dataBand);
                    dataBand.Parent.Components.Insert(index + 1, footerBand);
                    allComps.Add(footerBand);
                }
                #endregion

                #region Create columns
                double columnWidth = AlignToGrid(dataBand.Page, width / columns.Count, false);
                if (!(dataBand.Parent is StiPage))
                {
                    columnWidth = AlignToGrid(dataBand.Page, width / columns.Count, false);
                }
                double pos = 0;

                int indexNode = 1;

                foreach (Hashtable columnObject in columns)
                {
                    string columnName = (string)columnObject["fullName"];

                    if (indexNode == columns.Count)
                    {
                        columnWidth = AlignToGrid(dataBand.Page, width - pos, false);
                        if (columnWidth <= 0) columnWidth = dataBand.Page.GridSize;
                    }

                    #region HeaderText
                    if ((bool)settings["header"])
                    {
                        var headerText = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.Text) as StiText;
                        headerText.VertAlignment = StiVertAlignment.Center;
                        headerText.Font = new Font("Arial", 10, FontStyle.Bold);
                        string baseName = dataBand.Report.Info.GenerateLocalizedName ? headerBand.LocalizedName + columnName : "Header" + columnName;
                        headerText.Name = StiNameCreation.CreateName(dataBand.Report, StiNameValidator.CorrectName(baseName), false, false, true);
                        headerText.Top = 0;
                        headerText.Left = pos;
                        headerText.Width = columnWidth;
                        headerText.Height = headerBand.Height;
                        headerText.WordWrap = true;

                        int index = (columnName).LastIndexOf('.');

                        var column = StiDataPathFinder.GetColumnFromPath(columnName, dataBand.Report.Dictionary);

                        if (column != null)
                        {
                            headerText.Text = column.Alias;
                        }
                        else
                        {
                            if (index == -1) headerText.Text = columnName;
                            else headerText.Text = columnName.Substring(index + 1);
                        }

                        headerBand.Components.Add(headerText);
                        allComps.Add(headerText);
                    }
                    #endregion

                    #region Data
                    var dataColumn = StiDataPathFinder.GetColumnFromPath(columnName, dataBand.Report.Dictionary);
                    #region Image
                    if (dataColumn != null && (dataColumn.Type == typeof(Image) || dataColumn.Type == typeof(byte[])))
                    {
                        var dataImage = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.Image) as StiImage;
                        string baseName2 = dataBand.Report.Info.GenerateLocalizedName ? dataBand.LocalizedName + columnName : "Data" + columnName;
                        dataImage.Name = StiNameCreation.CreateName(dataBand.Report, StiNameValidator.CorrectName(baseName2), false, false, true);
                        dataImage.Top = 0;
                        dataImage.Left = pos;
                        dataImage.Width = columnWidth;
                        dataImage.Height = dataBand.Height;
                        dataImage.DataColumn = columnObject["imageColumnFullName"] != null ? (string)columnObject["imageColumnFullName"] : columnName;
                        dataImage.CanGrow = true;
                        dataBand.Components.Add(dataImage);
                        allComps.Add(dataImage);
                    }
                    #endregion

                    #region CheckBox
                    else if (dataColumn != null && dataColumn.Type == typeof(bool))
                    {
                        var dataCheck = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.CheckBox) as Stimulsoft.Report.Components.StiCheckBox;
                        string baseName2 = dataBand.Report.Info.GenerateLocalizedName ? dataBand.LocalizedName + columnName : "Data" + columnName;
                        dataCheck.Name = StiNameCreation.CreateName(dataBand.Report, StiNameValidator.CorrectName(baseName2), false, false, true);
                        dataCheck.Top = 0;
                        dataCheck.Left = pos;
                        dataCheck.Width = columnWidth;
                        dataCheck.Height = dataBand.Height;
                        dataCheck.Checked.Value = "{" + columnName + "}";
                        dataBand.Components.Add(dataCheck);
                        allComps.Add(dataCheck);
                    }
                    #endregion

                    #region Text
                    else
                    {
                        var dataText = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.Text) as StiText;
                        string baseName2 = dataBand.Report.Info.GenerateLocalizedName ? dataBand.LocalizedName + columnName : "Data" + columnName;
                        dataText.Name = StiNameCreation.CreateName(dataBand.Report, StiNameValidator.CorrectName(baseName2), false, false, true);
                        dataText.VertAlignment = StiVertAlignment.Center;
                        dataText.Top = 0;
                        dataText.Left = pos;
                        dataText.Width = columnWidth;
                        dataText.Height = dataBand.Height;
                        dataText.Text = "{" + columnName + "}";
                        dataText.CanGrow = true;
                        dataText.WordWrap = true;
                        dataBand.Components.Add(dataText);
                        allComps.Add(dataText);
                    }
                    #endregion
                    #endregion

                    #region FooterText
                    if ((bool)settings["footer"])
                    {
                        var footerText = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.Text) as StiText;
                        footerText.VertAlignment = StiVertAlignment.Center;
                        footerText.HorAlignment = StiTextHorAlignment.Right;
                        footerText.Font = new Font("Arial", 10, FontStyle.Bold);
                        string baseName = dataBand.Report.Info.GenerateLocalizedName ? footerBand.LocalizedName + columnName : "Footer" + columnName;
                        footerText.Name = StiNameCreation.CreateName(dataBand.Report, StiNameValidator.CorrectName("Footer" + columnName), false, false, true);
                        footerText.Top = 0;
                        footerText.Left = pos;
                        footerText.Width = columnWidth;
                        footerText.Height = footerBand.Height;
                        footerText.WordWrap = true;

                        footerBand.Components.Add(footerText);
                        allComps.Add(footerText);
                    }
                    #endregion

                    pos += columnWidth;

                    indexNode++;
                }
                #endregion

                #region Theme
                if (param["theme"] != null)
                {
                    Hashtable themeProps = (Hashtable)param["theme"];
                    var themeName = themeProps["name"] as string;                    
                    var stylesCollection = new StiStylesCollection();

                    switch ((string)themeProps["type"])
                    {
                        case "Default":
                            {
                                stylesCollection = StiStylesHelper.CreateStylesCollectionFromBaseColor(StrToColor(themeProps["color"] as string), themeName);
                                
                                foreach (StiBaseStyle style in stylesCollection)
                                {
                                    report.Styles[style.Name] = style;
                                }
                                break;
                            }
                        case "User":
                            {
                                foreach (StiBaseStyle style in report.Styles)
                                {
                                    if (style.CollectionName == themeName)
                                        stylesCollection.Add(style);
                                }
                                break;
                            }
                    }

                    foreach (StiComponent component in allComps)
                    {
                        ApplyStyleCollection(component, stylesCollection);
                    }
                }
                #endregion

                #endregion
            }
            else
            {
                #region Table
                var dataSourceName = (string)dataSource["name"];
                var table = new StiTable();

                AddComponentToPage(table, currentPage);

                if ((string)dataSource["typeItem"] == "DataSource")
                    SetDataSourceProperty(table, dataSourceName);
                else
                    SetBusinessObjectProperty(table, (string)dataSource["fullName"]);

                string baseName = table.Report.Info.GenerateLocalizedName ? table.LocalizedName + dataSourceName : "Table" + dataSourceName;
                table.Name = StiNameCreation.CreateName(table.Report, StiNameValidator.CorrectName(baseName), false, false, true);
                double width = table.Parent is StiPage ? table.Page.GetColumnWidth() : table.Parent.Width;

                int countRows = 1;
                if ((bool)settings["header"]) countRows++;
                if ((bool)settings["footer"]) countRows++;
                table.RowCount = countRows;
                table.ColumnCount = columns.Count;
                table.HeaderRowsCount = ((bool)settings["header"]) ? 1 : 0;
                table.FooterRowsCount = ((bool)settings["footer"]) ? 1 : 0;
                table.Height = (table.Page.GridSize * 4) * countRows;
                table.Width = width;

                int indexHeader = 0;
                int indexData = ((bool)settings["header"]) ? columns.Count : 0;

                foreach (Hashtable columnObject in columns)
                {
                    string columnName = (string)columnObject["fullName"];

                    #region Create Header
                    if ((bool)settings["header"])
                    {
                        var headerCell = (StiTableCell)table.Components[indexHeader];

                        int index = columnName.LastIndexOf('.');
                        var column = StiDataPathFinder.GetColumnFromPath(columnName, table.Report.Dictionary);

                        if (column != null)
                        {
                            headerCell.Text = column.Alias;
                        }
                        else
                        {
                            if (index == -1) headerCell.Text = columnName;
                            else headerCell.Text = columnName.Substring(index + 1);
                        }
                        indexHeader++;
                    }
                    #endregion

                    #region Create Data
                    var dataColumn = StiDataPathFinder.GetColumnFromPath(columnName, table.Report.Dictionary);
                    #region Image
                    if (dataColumn != null && (dataColumn.Type == typeof(Image) || dataColumn.Type == typeof(byte[])))
                    {
                        ((IStiTableCell)table.Components[indexData]).CellType = StiTablceCellType.Image;
                        var dataCell = (StiTableCellImage)table.Components[indexData];
                        dataCell.DataColumn = columnObject["imageColumnFullName"] != null ? (string)columnObject["imageColumnFullName"] : columnName;
                        indexData++;
                    }
                    #endregion

                    #region CheckBox
                    else if (dataColumn != null && dataColumn.Type == typeof(bool))
                    {
                        ((IStiTableCell)table.Components[indexData]).CellType = StiTablceCellType.CheckBox;
                        var dataCell = (StiTableCellCheckBox)table.Components[indexData];
                        dataCell.Checked.Value = "{" + columnName as string + "}";
                        indexData++;
                    }
                    #endregion

                    #region Text
                    else
                    {
                        var dataCell = (StiTableCell)table.Components[indexData];
                        dataCell.Text = "{" + columnName as string + "}";
                        indexData++;
                    }
                    #endregion
                    #endregion
                }

                #region Apply Styles
                if (param["theme"] != null)
                {
                    Hashtable themeProps = (Hashtable)param["theme"];
                    var themeName = themeProps["name"] as string;
                    var stylesCollection = new StiStylesCollection();

                    switch ((string)themeProps["type"])
                    {
                        case "Default":
                            {
                                stylesCollection = StiStylesHelper.CreateStylesCollectionFromBaseColor(StrToColor(themeProps["color"] as string), themeName);

                                foreach (StiBaseStyle style in stylesCollection)
                                {
                                    report.Styles[style.Name] = style;
                                }
                                break;
                            }
                        case "User":
                            {
                                foreach (StiBaseStyle style in report.Styles)
                                {
                                    if (style.CollectionName == themeName)
                                        stylesCollection.Add(style);
                                }
                                break;
                            }
                    }

                    table.ComponentStyle = null;

                    if (stylesCollection.Count > 0)
                    {
                        var tableStyle = new StiTableStyle();

                        var oddStyle = stylesCollection.ToList().FirstOrDefault(s => s.Name.EndsWith(Loc.GetMain("OddStyle"))) as StiStyle;
                        tableStyle.DataColor = StiBrush.ToColor(oddStyle?.Brush);
                        tableStyle.DataForeground = StiBrush.ToColor(oddStyle?.TextBrush);

                        var evenStyle = stylesCollection.ToList().FirstOrDefault(s => s.Name.EndsWith(Loc.GetMain("EvenStyle"))) as StiStyle;
                        tableStyle.AlternatingDataColor = StiBrush.ToColor(evenStyle?.Brush);
                        tableStyle.AlternatingDataForeground = StiBrush.ToColor(evenStyle?.TextBrush);

                        var groupFooterName = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiGroupFooterBand")).Replace(" ", "_");
                        var footerName = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiFooterBand")).Replace(" ", "_");
                        var footerStyle = stylesCollection.ToList().FirstOrDefault(s => !s.Name.EndsWith($"{groupFooterName}1") && s.Name.EndsWith($"{footerName}1")) as StiStyle;
                        tableStyle.FooterColor = StiBrush.ToColor(footerStyle?.Brush);
                        tableStyle.FooterForeground = StiBrush.ToColor(footerStyle?.TextBrush);

                        var groupHeaderName = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiGroupHeaderBand")).Replace(" ", "_");
                        var headerName = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiHeaderBand")).Replace(" ", "_");
                        var headerStyle = stylesCollection.ToList().FirstOrDefault(s => !s.Name.EndsWith($"{groupHeaderName}1") && s.Name.EndsWith($"{headerName}1")) as StiStyle;
                        tableStyle.HotHeaderColor = StiBrush.ToColor(headerStyle?.Brush);
                        tableStyle.HeaderColor = StiBrush.ToColor(headerStyle?.Brush);
                        tableStyle.HeaderForeground = StiBrush.ToColor(headerStyle?.TextBrush);

                        tableStyle.Name = table.Name + (stylesCollection.ToList().FirstOrDefault() as StiStyle).CollectionName;
                        report.Styles.Add(tableStyle);
                        table.ComponentStyle = tableStyle.Name;
                        table.RefreshTableStyle();
                    }
                }
                #endregion
                #endregion
            }

            StiDesignReportHelper reportHelper = new StiDesignReportHelper(report);
            callbackResult["pageComponents"] = reportHelper.GetPage(report.Pages.IndexOf(currentPage));
            callbackResult["pageName"] = currentPageName;
            callbackResult["rebuildProps"] = GetPropsRebuildPage(report, currentPage);
            callbackResult["stylesCollection"] = StiStylesHelper.GetStyles(report);
        }

        public static void SetReportProperties(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable allProperties = (Hashtable)param["properties"];
            foreach (DictionaryEntry property in allProperties)
            {
                string propertyName = property.Key as string;
                object propertyValue = property.Value;

                if (propertyName == "reportUnit")
                {
                    ChangeUnit(report, propertyName);
                }
                else if (propertyName == "scaleContent")
                {
                    StiSettings.Load();
                    StiSettings.Set("StiDesigner", "ScaleContent", Convert.ToBoolean(propertyValue));
                    StiSettings.Save();
                }
                else
                {
                    SetPropertyValue(report, UpperFirstChar(propertyName), report, propertyValue);
                }
            }
            callbackResult["properties"] = GetReportProperties(report);
        }

        public static Hashtable GetReportProperties(StiReport report)
        {            
            Hashtable properties = new Hashtable();
            properties["reportUnit"] = report.Unit.ShortName;
            properties["reportFile"] = GetReportFileName(report);
            properties["events"] = GetEventsProperty(report);
            properties["previewSettings"] = GetPreviewSettingsProperty(report);
            properties["reportCreated"] = report.ReportCreated.ToString();
            properties["reportChanged"] = report.ReportChanged.ToString();

            StiSettings.Load();
            properties["scaleContent"] = StiSettings.GetBool("StiDesigner", "ScaleContent", true);

            string[] propertyNames = { "ReportName", "ReportAlias", "ReportAuthor", "ReportDescription", "ReportImage", "AutoLocalizeReportOnRun", "CacheAllData", "CacheTotals",
                "CalculationMode", "ConvertNulls", "Collate", "Culture", "EngineVersion", "NumberOfPass", "PreviewMode", "ReportCacheMode", "ParametersOrientation",
                "RequestParameters", "ScriptLanguage", "StopBeforePage", "StoreImagesInResources", "RetrieveOnlyUsedData", "RefreshTime", "ParameterWidth"};

            foreach (string propertyName in propertyNames)
            {
                var value = GetPropertyValue(propertyName, report);
                if (value != null) { properties[LowerFirstChar(propertyName)] = value; }
            }

#if CLOUD || SERVER
            properties["parametersDateFormat"] = report.ParametersDateFormat;
#endif

            return properties;
        }

        public static void PageMove(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            int pageIndex = StrToInt((string)param["pageIndex"]);
            string direction = (string)param["direction"];

            if (direction == "Left" && pageIndex > 0)
            {
                var page = report.Pages[pageIndex - 1];
                report.Pages.RemoveAt(pageIndex - 1);
                report.Pages.Insert(pageIndex, page);
            }
            else if (direction == "Right" && pageIndex < report.Pages.Count - 1)
            {
                var page = report.Pages[pageIndex];
                report.Pages.RemoveAt(pageIndex);
                report.Pages.Insert(pageIndex + 1, page);
            }

            callbackResult["pageIndexes"] = GetPageIndexes(report);
        }

        public static void AlignToGridComponents(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            StiPage currentPage = report.Pages[(string)param["pageName"]];
            ArrayList componentNames = (ArrayList)param["components"];
            for (int i = 0; i < componentNames.Count; i++)
            {
                StiComponent component = report.Pages.GetComponentByName((string)componentNames[i]);
                RectangleD rect = component.GetPaintRectangle(false, false);
                rect = rect.AlignToGrid(component.Page.GridSize, true);
                SetComponentRect(component, rect);
            }
            callbackResult["pageName"] = currentPage.Name;
            callbackResult["rebuildProps"] = GetPropsRebuildPage(report, currentPage);
        }

        public static void ChangeArrangeComponents(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            StiPage currentPage = report.Pages[(string)param["pageName"]];
            ArrayList componentNames = (ArrayList)param["components"];

            //set not selected all components            
            foreach (StiComponent component in currentPage.GetComponents())
            {
                component.IsSelected = false;
            }

            //set selected current components
            foreach (string componentName in componentNames)
            {
                StiComponent component = report.Pages.GetComponentByName(componentName);
                if (component != null) component.Select();
            }

            switch ((string)param["arrangeCommand"])
            {
                case "BringToFront": currentPage.BringToFront(); break;
                case "SendToBack": currentPage.SendToBack(); break;
                case "MoveForward": currentPage.MoveForward(); break;
                case "MoveBackward": currentPage.MoveBackward(); break;
                case "AlignLeft": currentPage.AlignTo(StiAligning.Left); break;
                case "AlignCenter": currentPage.AlignTo(StiAligning.Center); break;
                case "AlignRight": currentPage.AlignTo(StiAligning.Right); break;
                case "AlignTop": currentPage.AlignTo(StiAligning.Top); break;
                case "AlignMiddle": currentPage.AlignTo(StiAligning.Middle); break;
                case "AlignBottom": currentPage.AlignTo(StiAligning.Bottom); break;
                case "MakeHorizontalSpacingEqual": currentPage.MakeHorizontalSpacingEqual(); break;
                case "MakeVerticalSpacingEqual": currentPage.MakeVerticalSpacingEqual(); break;
                case "CenterHorizontally": currentPage.SetCenterHorizontally(); break;
                case "CenterVertically": currentPage.SetCenterVertically(); break;
            }

            currentPage.Correct();

            callbackResult["pageName"] = currentPage.Name;
            callbackResult["rebuildProps"] = GetPropsRebuildPage(report, currentPage);
        }

        public static void DuplicatePage(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            int pageIndex = Convert.ToInt32((string)param["pageIndex"]);
            StiPage page = report.Pages[pageIndex];
            StiPage newPage = null;

            using (var stream = new MemoryStream())
            {
                page.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                
                if (page is IStiDashboard)
                    newPage = StiDashboardCreator.CreateDashboard(report) as StiPage;
                else
                    newPage = new StiPage();

                newPage.Report = report;
                newPage.Name = "TempName";
                newPage.Load(stream);
                newPage.Name = StiNameCreation.CreateName(report, StiNameCreation.GenerateName(report, page.LocalizedName, page.GetType()));
                newPage.NewGuid();
                newPage.NewCacheGuid();
            }

            report.Pages.Insert(pageIndex + 1, newPage);

            StiComponentsCollection comps = newPage.GetComponents();
            foreach (StiComponent comp in comps)
            {
                comp.Name = StiNameCreation.CreateName(report, StiNameCreation.GenerateName(report, comp.LocalizedName, comp.GetType()));
            }

            if (param["reportFile"] != null) report.ReportFile = (string)param["reportFile"];
            callbackResult["reportGuid"] = param["reportGuid"];
            callbackResult["reportObject"] = WriteReportToJsObject(report);
            callbackResult["selectedPageIndex"] = pageIndex + 1;
        }
        
        public static void SetEventValue(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            ArrayList components = (ArrayList)param["components"];
            foreach (Hashtable componentParams in components)
            {
                object obj = null;

                if ((string)componentParams["typeComponent"] == "StiReport") 
                    obj = report;
                else if ((string)componentParams["typeComponent"] == "StiPage") 
                    obj = report.Pages[(string)componentParams["name"]];
                else 
                    obj = report.Pages.GetComponentByName((string)componentParams["name"]);

                if (obj != null)
                    SetPropertyValue(report, (string)param["eventName"] + ".Script", obj, StiEncodingHelper.DecodeString((string)param["eventValue"]));
            }
        }

        public static void ChangeSizeComponents(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            StiPage currentPage = report.Pages[(string)param["pageName"]];
            ArrayList componentNames = (ArrayList)param["components"];
            double zoom = StrToDouble((string)param["zoom"]);
            StiComponentsCollection currentComponents = new StiComponentsCollection();

            //set not selected all components            
            foreach (StiComponent component in currentPage.GetComponents())
            {
                component.IsSelected = false;
            }

            //set selected current components
            foreach (string componentName in componentNames)
            {
                StiComponent component = report.Pages.GetComponentByName(componentName);
                if (component != null)
                {
                    component.Select();
                    currentComponents.Add(component);
                }
            }

            if (currentComponents.Count > 0)
            {
                switch ((string)param["actionName"])
                {
                    case "MakeSameSize": currentPage.MakeSameSize(new SizeD(currentComponents[0].Width, currentComponents[0].Height)); break;
                    case "MakeSameWidth": currentPage.MakeSameWidth(currentComponents[0].Width); break;
                    case "MakeSameHeight": currentPage.MakeSameHeight(currentComponents[0].Height); break;
                }
                currentPage.Correct();
            }

            callbackResult["pageName"] = currentPage.Name;
            Hashtable rebuildProps = GetPropsRebuildPage(report, currentPage);

            //repaint svg content
            foreach (StiComponent component in currentComponents)
            {
                if (rebuildProps[component.Name] != null)
                {
                    (rebuildProps[component.Name] as Hashtable)["svgContent"] = GetSvgContent(component, zoom);
                }
            }
            callbackResult["rebuildProps"] = rebuildProps;
        }
                
        public static void CreateMovingCopyComponent(StiRequestParams requestParams, StiReport report, Hashtable param, Hashtable callbackResult)
        {
            SetToClipboard(requestParams, report, param, callbackResult);

            string text = requestParams.Cache.Helper.GetObjectInternal(requestParams, StiCacheHelper.GUID_Clipboard) as string;
            var group = StiGroup.CreateFromString(text, "StiReport Clipboard");

            StiPage currentPage = report.Pages[((string)param["pageName"])];
            double zoom = StrToDouble((string)param["zoom"]);
            StiInsertionComponents.InsertGroups(currentPage, group);

            ArrayList components = new ArrayList();
            StiComponentsCollection groupComponents = group.GetComponents();
            if (groupComponents.Count > 0)
            {
                string[] componentRect = ((string)param["componentRect"]).Split('!');
                RectangleD compRect = new RectangleD(
                    new PointD(StrToDouble(componentRect[0]), StrToDouble(componentRect[1])),
                    new SizeD(StrToDouble(componentRect[2]), StrToDouble(componentRect[3]))
                );

                bool ignoreOffset = false;
                ArrayList componentNames = (ArrayList)param["components"];
                callbackResult["oldComponentNames"] = componentNames;

                if (componentNames.Count > 0)
                {
                    StiComponent compSource = report.Pages.GetComponentByName(componentNames[0] as string);
                    if (compSource != null && IsAlignedByGrid(compSource))
                        ignoreOffset = true;
                }

                if (ignoreOffset)
                    SetComponentRect(groupComponents[0], compRect);
                else
                {
                    SetComponentRect(groupComponents[0], compRect, false);
                    SetComponentRectWithOffset(groupComponents[0], compRect, "MoveComponent", null, null);
                }
            }

            foreach (StiComponent component in group.GetComponents())
            {
                if (component is StiCrossLinePrimitive)
                    AddPrimitivePoints(component, currentPage);
                
                Hashtable attributes = new Hashtable();
                attributes["name"] = component.Name;
                attributes["typeComponent"] = component.GetType().Name;
                attributes["componentRect"] = GetComponentRect(component);
                attributes["parentName"] = GetParentName(component);
                attributes["parentIndex"] = GetParentIndex(component).ToString();
                attributes["componentIndex"] = GetComponentIndex(component).ToString();
                attributes["childs"] = GetAllChildComponents(component);
                attributes["svgContent"] = GetSvgContent(component, zoom);
                attributes["pageName"] = currentPage.Name;
                attributes["properties"] = GetAllProperties(component);
                attributes["largeHeightAutoFactor"] = currentPage.LargeHeightAutoFactor.ToString();
                components.Add(attributes);
                component.Dockable = true;
            }

            callbackResult["components"] = components;
            callbackResult["rebuildProps"] = GetPropsRebuildPage(report, currentPage);
            callbackResult["isLastCommand"] = param["isLastCommand"];
        }

        public static void UpdateReportAliases(StiRequestParams requestParams, StiReport report, Hashtable param, Hashtable callbackResult)
        {
            if (report != null)
            {
                var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
                callbackResult["reportObject"] = WriteReportToJsObject(report, zoom);
                callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
                callbackResult["selectedObjectName"] = param["selectedObjectName"];
            }
        }

        private static string GetPageTypeFromStream(byte[] data)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;

            using (var stream = new MemoryStream(data))
            {
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                    if (StiReport.IsJsonFile(stream)) return "StiPage";

                    var tr = new XmlTextReader(stream);
					tr.DtdProcessing = DtdProcessing.Ignore;
					
                    tr.Read();

                    tr.Read();

                    if (tr.IsStartElement())
                        return tr.Name == "StiSerializer" ? tr.GetAttribute("application") : "StiPage";
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    stream.Close();
                }
            }

            return "StiPage";
        }

        public static void OpenPage(StiRequestParams requestParams, StiReport report, Hashtable callbackResult)
        {
            using (var stream = new MemoryStream(requestParams.Data))
            {
                report.IsModified = true;
                StiPage page = null;
                string typePage = GetPageTypeFromStream(requestParams.Data);
                string pageName = null;

                if (typePage == "StiDashboard")
                {
                    page = StiDashboardCreator.CreateDashboard(report) as StiPage;
                    pageName = "Dashboard";
                }
                else
                {
                    page = new StiPage();
                    pageName = "Page";
                }

                page.Report = report;
                report.Pages.Add(page);

                StiPageSLService service;
                if (StiReport.IsJsonFile(stream))
                    service = new StiJsonPageSLService();
                else
                    service = new StiXmlPageSLService();

                page.Load(service, stream);

                page.Components.SortByPriority();

                #region Create names for components
                report.Pages.Remove(page);

                var tempPage = new StiPage(report);
                tempPage.Name = "#%#TempPageForStoreComponents#%#";
                report.Pages.Add(tempPage);

                var compcoll = new StiComponentsCollection();
                tempPage.Components = compcoll;

                page.Name = StiNameCreation.CreateName(report, pageName, true, false, true);
                var comps = page.GetComponents();

                foreach (StiComponent comp in comps)
                {
                    if (!StiNameCreation.IsValidName(report, comp.Name))
                        comp.Name = StiNameCreation.CreateName(report, StiNameCreation.GenerateName(comp));

                    tempPage.Components.Add(comp);
                }

                report.Pages.Remove(tempPage);
                report.Pages.Add(page);
                #endregion

                if (page.IsPage && page.ReportUnit != null)
                {
                    page.Convert(page.ReportUnit, page.Report.Unit);
                    page.ReportUnit = null;
                }

                StiDesignReportHelper reportHelper = new StiDesignReportHelper(report);
                var pageProps = reportHelper.GetPage(report.Pages.IndexOf(page));
                pageProps["pageIndexes"] = GetPageIndexes(report);

                callbackResult["pageProps"] = pageProps;
            }
        }
        #endregion
    }
}