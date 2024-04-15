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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Units;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#if NETSTANDARD
using Stimulsoft.System.Drawing;
using ColorConverter = Stimulsoft.System.Drawing.ColorConverter;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Describes the class that contains methods which serves for conversion of an object into a string and a string into an object.
    /// </summary>
    public class StiReportObjectStringConverter : StiObjectStringConverter
    {
        #region Fields
        private Hashtable hashObjectToString = new Hashtable();
        private Hashtable hashObjects = new Hashtable();
        private Hashtable hashStringToObject = new Hashtable();
        private Hashtable hashStrings = new Hashtable();
        #endregion

        #region Properties
        internal bool AllowLoadOptimization { get; set; }
        #endregion

        #region Methods
        public void Clear()
        {
            hashObjectToString.Clear();
            hashObjectToString = null;
            hashObjects.Clear();
            hashObjects = null;
            hashStringToObject.Clear();
            hashStringToObject = null;
            hashStrings.Clear();
            hashStrings = null;
        }

        public override void SetProperty(PropertyInfo p, object parentObject, object obj)
        {
            if (p.Name != "HighlightCondition") return;

            var highlightCondition = obj as StiHighlightCondition;
            var text = parentObject as StiSimpleText;

            if (highlightCondition.Condition.Value.Trim().Length == 0)return;

            text.Conditions.Add(new StiCondition
            {
                BackColor = StiBrush.ToColor(highlightCondition.Brush),
                TextColor = StiBrush.ToColor(highlightCondition.TextBrush),
                Font = highlightCondition.Font,
                Expression = highlightCondition.Condition,
                Item = StiFilterItem.Expression
            });
        }

        /// <summary>
        /// Converts object into string.
        /// </summary>
        /// <param name="obj">Object for converting.</param>
        /// <returns>String that contains object.</returns>
        public override string ObjectToString(object obj)
        {
            if (!StiOptions.Engine.SerializationOptimization)
                return ObjectToString2(obj);

            if (obj is Font || obj is StiBorder || obj is StiSimpleBorder || obj is StiBrush || obj is StiTextOptions || obj is StiSimpleShadow)
            {
                var hash = obj.GetHashCode();
                var res = hashObjectToString[hash];
                if (res != null)
                {
                    var objRes = hashObjects[hash];
                    if (obj.Equals(objRes))
                    {
                        if (res is bool) 
                            return null;

                        return (string)res;
                    }
                    return ObjectToString2(obj);
                }

                var st = ObjectToString2(obj);
                if (st == null)
                    hashObjectToString[hash] = false;
                else
                    hashObjectToString[hash] = st;

                hashObjects[hash] = obj;
                return st;
            }

            return ObjectToString2(obj);
        }

        public string ObjectToString2(object obj)
        {
            if (obj is Font)
                return ConvertFontToString((Font)obj);

            if (obj is PointF)
                return ConvertPointFToString((PointF)obj);

            if (obj is RectangleD)
                return ConvertRectangleDToString((RectangleD)obj);

            if (obj is StiTextOptions)
                return ConvertTextOptionsToString(obj as StiTextOptions);

            if (obj is StiBrush)
                return ConvertBrushToString(obj as StiBrush);

            if (obj is StiBorder)
                return ConvertBorderToString(obj as StiBorder);

            if (obj is StiSimpleBorder)
                return ConvertSimpleBorderToString(obj as StiSimpleBorder);

            if (obj is StiSimpleShadow)
                return ConvertSimpleShadowToString(obj as StiSimpleShadow);

            if (obj is StiVariable)
                return ConvertVariableToString(obj as StiVariable);

            if (obj is StiExpression)
                return ConvertExpressionToString(obj as StiExpression);

            if (obj is StiEvent)
                return ConvertEventToString(obj as StiEvent);

            if (obj is DateTime)
                return ((DateTime)obj).ToString(new CultureInfo("en-US", false));

            if (obj is Color[])
                return ConvertColorArrayToString(obj as Color[]);

            return base.ObjectToString(obj);
        }

        /// <summary>
        /// Converts string into object.
        /// </summary>
        /// <param name="str">String that contains object.</param>
        /// <param name="type">Object type.</param>
        /// <returns>Object.</returns>
        public override object StringToObject(string str, Type type)
        {
            if (!StiOptions.Engine.SerializationOptimization || !AllowLoadOptimization)
                return StringToObject2(str, type);

            if (type == typeof(Font) || type == typeof(StiBorder) || type == typeof(StiSimpleBorder) ||
                type == typeof(StiBrush) || type == typeof(StiTextOptions) || type == typeof(StiSimpleShadow))
            {
                var hash = str.GetHashCode();
                var res = hashStringToObject[hash];
                if (res != null)
                {
                    var strRes = (string)hashStrings[hash];
                    if (str.Equals(strRes))
                        return res;

                    return StringToObject2(str, type);
                }
                res = StringToObject2(str, type);
                hashStringToObject[hash] = res;
                hashStrings[hash] = str;
                return res;
            }

            return StringToObject2(str, type);
        }

        public object StringToObject2(string str, Type type)
        {
            if (type == typeof(Font))
                return ConvertStringToFont(str);

            if (type == typeof(RectangleD))
                return ConvertStringToRectangleD(str);

            if (type == typeof(PointF))
                return ConvertStringToPointF(str);

            if (type == typeof(StiTextOptions))
                return ConvertStringToTextOptions(str);

            if (type == typeof(StiBrush))
                return ConvertStringToBrush(str);

            if (type == typeof(StiBorder))
                return ConvertStringToBorder(str);

            if (type == typeof(StiSimpleBorder))
                return ConvertStringToSimpleBorder(str);

            if (type == typeof(StiSimpleShadow))
                return ConvertStringToSimpleShadow(str);

            if (type == typeof(StiVariable))
                return ConvertStringToVariable(type, str);

            if (StiTypeFinder.FindType(type, typeof(StiDataParameter)))
                return base.StringToObject(str, type);

            if (type == typeof(StiExpression))
                return ConvertStringToExpression(type, str);

            if (type == typeof(Image))
                return StiImageConverter.StringToImage(str);

            if (type == typeof(Metafile))
                return StiMetafileConverter.StringToMetafile(str);

            if (type == typeof(DateTime))
            {
                if (str == null || str.Trim().Length == 0)
                    return DateTime.Now;

                DateTime result;
                if (!DateTime.TryParse(str, new CultureInfo("en-US", false), DateTimeStyles.None, out result))
                    return DateTime.Today;

                return result;
            }

            if (type == typeof(StiUnit))
            {
                if (str == "Centimeters")
                    return StiUnit.Centimeters;

                if (str == "Millimeters")
                    return StiUnit.Millimeters;

                if (str == "HundredthsOfInch")
                    return StiUnit.HundredthsOfInch;

                if (str == "Inches")
                    return StiUnit.Inches;
            }

            if (type == typeof(Color[]))
                return ConvertStringToColorArray(str);

            return base.StringToObject(str, type);
        }
        #endregion

        #region Methods.Event
        /// <summary>
        /// Converts event into string.
        /// </summary>
        /// <param name="ev">Event for conversion.</param>
        /// <returns>Converted string.</returns>
        public string ConvertEventToString(StiEvent ev)
        {
            if (string.IsNullOrEmpty(ev.Script)) 
                return null;

            return ev.Script;
        }
        #endregion

        #region Methods.Expression
        /// <summary>
        /// Convets expression into string.
        /// </summary>
        /// <param name="expr">Expression for conversion.</param>
        /// <returns>String got as result of conversion.</returns>
        public string ConvertExpressionToString(StiExpression expr)
        {
            if (!(expr is StiDataParameter) && expr.Value == string.Empty)
                return null;

            return TypeDescriptor.GetConverter(expr.GetType()).ConvertToString(expr);
        }

        /// <summary>
        /// Converts string into expression.
        /// </summary>
        /// <param name="type">Expression type.</param>
        /// <param name="str">String that shows expression.</param>
        /// <returns>Expression got as result of conversion.</returns>
        public StiExpression ConvertStringToExpression(Type type, string str)
        {
            return TypeDescriptor.GetConverter(type).ConvertFromString(str) as StiExpression;
        }
        #endregion

        #region Methods.Variable
        /// <summary>
        /// Converts variable into string.
        /// </summary>
        /// <param name="variable">Variable for conversion.</param>
        /// <returns>String got as result of conversion.</returns>
        public string ConvertVariableToString(StiVariable variable)
        {
            return TypeDescriptor.GetConverter(variable.GetType()).ConvertToString(variable);
        }

        /// <summary>
        /// Converts string into variable.
        /// </summary>
        /// <param name="type">Type of variable.</param>
        /// <param name="str">String that shows variable.</param>
        /// <returns>Variable got as result of conversion.</returns>
        public StiVariable ConvertStringToVariable(Type type, string str)
        {
            return TypeDescriptor.GetConverter(type).ConvertFromString(str) as StiVariable;
        }
        #endregion

        #region Methods.Font
        private static string GetOriginalFontStr(Font font)
        {
            return string.IsNullOrEmpty(font.OriginalFontName) || font.OriginalFontName == font.Name
                ? $"{font.FontFamily.Name},{font.Size}"
                : $"{font.OriginalFontName},{font.Size}";
        }

        /// <summary>
        /// Converts font into string.
        /// </summary>
        /// <param name="font">Font for conversion.</param>
        /// <returns>String got as result of conversion.</returns>
        public static string ConvertFontToString(Font font)
        {
            var sb = new StringBuilder();
            sb = StiOptions.Engine.AllowUseOriginalFontName
                ? sb.Append(GetOriginalFontStr(font))
                : sb.AppendFormat("{0},{1}", font.FontFamily.Name, font.Size);

            var count = 2;

            if (font.GdiCharSet != 1)
                count = 6;

            else if (font.GdiVerticalFont)
                count = 5;

            else if (font.Unit != GraphicsUnit.Point)
                count = 4;

            else if (font.Style != FontStyle.Regular)
                count = 3;

            if (count > 2)
            {
                var fontStyleConverter = new EnumConverter(typeof(FontStyle));
                sb = sb.AppendFormat(",{0}", fontStyleConverter.ConvertToString(font.Style).Replace(',', '|'));

                if (count > 3)
                {
                    var unitConverter = new EnumConverter(typeof(GraphicsUnit));
                    sb = sb.AppendFormat(",{0}", unitConverter.ConvertToString(font.Unit));

                    if (count > 4)
                    {
                        sb = sb.AppendFormat(",{0}", font.GdiVerticalFont);

                        if (count > 5)
                            sb = sb.AppendFormat(",{0}", font.GdiCharSet);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
		/// Converts string into font.
		/// </summary>
		/// <param name="str">String that shows font.</param>
		/// <returns>Font got as result of conversion.</returns>
		public static Font ConvertStringToFont(string str)
        {
            var words = str.Split(',');
            var fontFamilyStr = words[0];
            var fontFamily = StiFontCollection.GetFontFamily(fontFamilyStr, FontStyle.Regular, true);
            var size = float.Parse(words[1]);

            if (words.Length == 2)
            {
                var font = fontFamily != null 
                    ? new Font(fontFamily, size) 
                    : new Font(fontFamilyStr, size);

                if (StiOptions.Engine.ThrowErrorIfFontIsNotInstalledInSystem && font.FontFamily.Name != fontFamilyStr)
                    throw new Exception($"Font '{fontFamily}' is not installed on system!");

                return font;
            }

            if (words.Length > 2)
            {
                var fontStyleConverter = new EnumConverter(typeof(FontStyle));
                var style = (FontStyle)fontStyleConverter.ConvertFromString(words[2].Replace('|', ','));
                fontFamily = StiFontCollection.GetFontFamily(fontFamilyStr, style, true);
                if (words.Length == 3)
                {
                    try
                    {
                        var font = fontFamily != null
                            ? new Font(fontFamily, size, style)
                            : new Font(fontFamilyStr, size, style);

                        if (StiOptions.Engine.ThrowErrorIfFontIsNotInstalledInSystem && font.FontFamily.Name != fontFamilyStr)
                            throw new Exception($"Font '{fontFamily}' is not installed on system!");

                        return font;
                    }
                    catch (ArgumentException)
                    {
                        return new Font("Arial", size, style);
                    }
                }

                var unitConverter = new EnumConverter(typeof(GraphicsUnit));
                var unit = (GraphicsUnit)unitConverter.ConvertFromString(words[3]);
                if (words.Length == 4)
                {
                    try
                    {
                        var font = fontFamily != null
                            ? new Font(fontFamily, size, style, unit)
                            : new Font(fontFamilyStr, size, style, unit);

                        if (StiOptions.Engine.ThrowErrorIfFontIsNotInstalledInSystem && font.FontFamily.Name != fontFamilyStr)
                            throw new Exception($"Font '{fontFamily}' is not installed on system!");

                        return font;
                    }
                    catch (ArgumentException)
                    {
                        return new Font("Arial", size, style, unit);
                    }
                }

                var gdiVerticalFont = words[4] == "True";
                if (words.Length == 5)
                {
                    try
                    {
                        var font = fontFamily != null
                            ? new Font(fontFamily, size, style, unit, 1, gdiVerticalFont)
                            : new Font(fontFamilyStr, size, style, unit, 1, gdiVerticalFont);

                        if (StiOptions.Engine.ThrowErrorIfFontIsNotInstalledInSystem && font.FontFamily.Name != fontFamilyStr)
                            throw new Exception($"Font '{fontFamily}' is not installed on system!");

                        return font;
                    }
                    catch (ArgumentException)
                    {
                        return new Font("Arial", size, style, unit, 1, gdiVerticalFont);
                    }
                }

                var gdiCharSet = byte.Parse(words[5]);
                if (words.Length == 6)
                {
                    try
                    {
                        var font = fontFamily != null
                            ? new Font(fontFamily, size, style, unit, gdiCharSet, gdiVerticalFont)
                            : new Font(fontFamilyStr, size, style, unit, gdiCharSet, gdiVerticalFont);

                        if (StiOptions.Engine.ThrowErrorIfFontIsNotInstalledInSystem && font.FontFamily.Name != fontFamilyStr)
                            throw new Exception($"Font '{fontFamily}' is not installed on system!");

                        return font;
                    }
                    catch (ArgumentException)
                    {
                        return new Font("Arial", size, style, unit, gdiCharSet, gdiVerticalFont);
                    }
                }
            }
            return null;
        }
        #endregion

        #region Methods.StiTextOptions
        /// <summary>
        /// Converts StiTextOptions into string.
        /// </summary>
        /// <param name="textOptions">StiTextOptions for conversion.</param>
        /// <returns>String got as result of conversion.</returns>
        public string ConvertTextOptionsToString(StiTextOptions textOptions)
        {
            return textOptions.IsDefault ? null : StiTextOptionsHelper.ConvertTextOptionsToString(textOptions, ',');
        }

        /// <summary>
        /// Converts string into StiTextOptions.
        /// </summary>
        /// <param name="str">String that shows StiTextOptions.</param>
        /// <returns>StiTextOptions got as result of conversion.</returns>
        public StiTextOptions ConvertStringToTextOptions(string str)
        {
            return StiTextOptionsHelper.ConvertStringToTextOptions(str, ',');
        }
        #endregion

        #region Methods.RectangleD
        /// <summary>
        /// Converts rectangle into string.
        /// </summary>
        /// <param name="rectangle">Rectangle for conversion.</param>
        /// <returns>String got as result of conversion.</returns>
        public string ConvertRectangleDToString(RectangleD rectangle)
        {
            return RectangleDHelper.ConvertRectangleDToString(rectangle);
        }

        /// <summary>
        /// Converts string into rectangle.
        /// </summary>
        /// <param name="str">String that shows rectangle.</param>
        /// <returns>Rectangle got as result of conversion.</returns>
        public RectangleD ConvertStringToRectangleD(string str)
        {
            return RectangleDHelper.ConvertStringToRectangleD(str, ',');
        }
        #endregion

        #region Methods.PointF
        /// <summary>
        /// Converts PointF into string.
        /// </summary>
        /// <returns>String got as result of conversion.</returns>
        public static string ConvertPointFToString(PointF point)
        {
            return $"{point.X},{point.Y}";
        }

        /// <summary>
        /// Converts a string into a PointF.
        /// </summary>
        /// <param name="str">String that shows color.</param>
        /// <returns>PointF got as result of conversion.</returns>
        public static PointF ConvertStringToPointF(string str)
        {
            var strs = str.Split(',');

            var x = float.Parse(strs[0]);
            var y = float.Parse(strs[1]);

            return new PointF(x, y);
        }
        #endregion

        #region Methods.StiBrush
        /// <summary>
		/// Converts brush into string.
		/// </summary>
		/// <param name="brush">Brush for conversion.</param>
		/// <returns>String got as result of conversion.</returns>
		public string ConvertBrushToString(StiBrush brush)
        {
            if (brush is StiEmptyBrush)
                return "EmptyBrush";

            if (brush is StiSolidBrush)
            {
                var solidBrush = brush as StiSolidBrush;
                return ConvertColorToString(solidBrush.Color);
            }

            if (brush is StiHatchBrush)
            {
                var hatchBrush = brush as StiHatchBrush;
                return string.Format("HatchBrush,{0},{1},{2}",
                    hatchBrush.Style,
                    ConvertColorToString(hatchBrush.ForeColor),
                    ConvertColorToString(hatchBrush.BackColor));
            }

            if (brush is StiGradientBrush)
            {
                var gradientBrush = brush as StiGradientBrush;
                return string.Format("GradientBrush,{0},{1},{2}",
                    ConvertColorToString(gradientBrush.StartColor),
                    ConvertColorToString(gradientBrush.EndColor),
                    gradientBrush.Angle);
            }

            if (brush is StiGlassBrush)
            {
                var glassBrush = brush as StiGlassBrush;
                return string.Format("GlassBrush,{0},{1},{2}",
                    ConvertColorToString(glassBrush.Color),
                    glassBrush.DrawHatch,
                    glassBrush.Blend);
            }

            if (brush is StiGlareBrush)
            {
                var glareBrush = brush as StiGlareBrush;
                return string.Format("GlareBrush,{0},{1},{2},{3},{4}",
                    ConvertColorToString(glareBrush.StartColor),
                    ConvertColorToString(glareBrush.EndColor),
                    glareBrush.Angle,
                    glareBrush.Focus,
                    glareBrush.Scale);
            }

            if (brush is StiDefaultBrush)
                return "DefaultBrush";

            if (brush is StiStyleBrush)
                return "StyleBrush";

            return string.Empty;
        }

        /// <summary>
        /// Converts a string into a brush.
        /// </summary>
        /// <param name="str">String that shows brush.</param>
        /// <returns>Brush got as a result of conversion.</returns>
        public StiBrush ConvertStringToBrush(string str)
        {
            var words = str.Split(',');
            if (words[0] == "EmptyBrush")
                return new StiEmptyBrush();

            if (words[0] == "HatchBrush")
            {
                var style = (HatchStyle)Enum.Parse(typeof(HatchStyle), words[1], false);

                return new StiHatchBrush(
                    style,
                    ConvertStringToColor(words[2]),
                    ConvertStringToColor(words[3]));
            }

            if (words[0] == "GradientBrush")
            {
                return new StiGradientBrush(
                    ConvertStringToColor(words[1]),
                    ConvertStringToColor(words[2]),
                    double.Parse(words[3]));
            }

            if (words[0] == "GlareBrush")
            {
                return new StiGlareBrush(
                    ConvertStringToColor(words[1]),
                    ConvertStringToColor(words[2]),
                    double.Parse(words[3]),
                    float.Parse(words[4]),
                    float.Parse(words[5]));
            }

            if (words[0] == "GlassBrush")
            {
                return new StiGlassBrush(
                    ConvertStringToColor(words[1]),
                    words[2].ToLowerInvariant() == "true",
                    float.Parse(words[3]));
            }

            if (words[0] == "DefaultBrush")
                return new StiDefaultBrush();

            if (words[0] == "StyleBrush")
                return new StiStyleBrush();

            return new StiSolidBrush(ConvertStringToColor(words[0]));
        }
        #endregion

        #region Methods.Color
        /// <summary>
        /// Converts a color into a string.
        /// </summary>
        /// <param name="color">Color for conversion.</param>
        /// <returns>String got as result of conversion.</returns>
        public static string ConvertColorToString(Color color)
        {
            if (!color.IsNamedColor && !color.IsSystemColor)
            {
                return color.A == 255
                    ? $"[{color.R}:{color.G}:{color.B}]"
                    : $"[{color.A}:{color.R}:{color.G}:{color.B}]";
            }

            var converter = new ColorConverter();
            return converter.ConvertToString(color);
        }

        /// <summary>
        /// Converts a string into a color.
        /// </summary>
        /// <param name="str">String that shows color.</param>
        /// <returns>Color got as result of conversion.</returns>
        public static Color ConvertStringToColor(string str)
        {
            if (str.IndexOf(';') != -1)
            {
                str = str.Trim();
                var colorPar = str.Split(';');

                try
                {
                    if (colorPar.Length == 4)
                    {
                        return Color.FromArgb(
                            int.Parse(colorPar[0]),
                            int.Parse(colorPar[1]),
                            int.Parse(colorPar[2]),
                            int.Parse(colorPar[3]));
                    }

                    if (colorPar.Length == 3)
                    {
                        return Color.FromArgb(
                            int.Parse(colorPar[0]),
                            int.Parse(colorPar[1]),
                            int.Parse(colorPar[2]));
                    }

                    return Color.Empty;
                }
                catch
                {
                    return Color.Empty;
                }
            }

            if (str.IndexOf('[') != -1)
            {
                str = str.Trim().Substring(1, str.Length - 2);
                var colorPar = str.Split(':');

                try
                {
                    if (colorPar.Length == 4)
                    {
                        return Color.FromArgb(
                            int.Parse(colorPar[0]),
                            int.Parse(colorPar[1]),
                            int.Parse(colorPar[2]),
                            int.Parse(colorPar[3]));
                    }

                    if (colorPar.Length == 3)
                    {
                        return Color.FromArgb(
                            int.Parse(colorPar[0]),
                            int.Parse(colorPar[1]),
                            int.Parse(colorPar[2]));
                    }

                    return Color.Empty;
                }
                catch
                {
                    return Color.Empty;
                }
            }

            var converter = new ColorConverter();
            return (Color)converter.ConvertFromString(str);
        }
        #endregion

        #region Methods.ColorArray
        /// <summary>
        /// Converts a color into a string.
        /// </summary>
        /// <param name="colors">Color for conversion.</param>
        /// <returns>String got as result of conversion.</returns>
        public static string ConvertColorArrayToString(Color[] colors)
        {
            var sb = new StringBuilder();
            var index = 0;
            foreach (var color in colors)
            {
                if (index != 0)
                    sb = sb.Append(", ");

                sb = sb.Append(ConvertColorToString(color));
                index++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a string into a color.
        /// </summary>
        /// <param name="str">String that shows color.</param>
        /// <returns>Color got as result of conversion.</returns>
        public static Color[] ConvertStringToColorArray(string str)
        {
            if (str.IndexOfInvariant(",") == -1)
                return new[] { ConvertStringToColor(str) };

            var strs = str.Split(',');
            var colors = new Color[strs.Length];
            var index = 0;

            foreach (var st in strs)
            {
                colors[index] = ConvertStringToColor(st.Trim());
                index++;
            }

            return colors;
        }
        #endregion

        #region Methods.StiBorder
        /// <summary>
        /// Converts a border into a string.
        /// </summary>
        /// <param name="border">Border for conversion.</param>
        /// <returns>String got as result of conversion.</returns>
        public string ConvertBorderToString(StiBorder border)
        {
            if (border.IsDefault) 
                return null;

            if (border is StiAdvancedBorder)
            {
                var aBorder = border as StiAdvancedBorder;

                var sb = new StringBuilder();
                sb = sb.AppendFormat("Adv{0};", ConvertColorToString(aBorder.TopSide.Color));
                sb = sb.AppendFormat("{0};", aBorder.TopSide.Size);
                sb = sb.AppendFormat("{0};", aBorder.TopSide.Style);

                sb = sb.AppendFormat("{0};", ConvertColorToString(aBorder.BottomSide.Color));
                sb = sb.AppendFormat("{0};", aBorder.BottomSide.Size);
                sb = sb.AppendFormat("{0};", aBorder.BottomSide.Style);

                sb = sb.AppendFormat("{0};", ConvertColorToString(aBorder.LeftSide.Color));
                sb = sb.AppendFormat("{0};", aBorder.LeftSide.Size);
                sb = sb.AppendFormat("{0};", aBorder.LeftSide.Style);

                sb = sb.AppendFormat("{0};", ConvertColorToString(aBorder.RightSide.Color));
                sb = sb.AppendFormat("{0};", aBorder.RightSide.Size);
                sb = sb.AppendFormat("{0};", aBorder.RightSide.Style);

                sb = sb.AppendFormat("{0};", border.DropShadow);
                sb = sb.AppendFormat("{0};", border.ShadowSize);
                sb = sb.Append(ConvertBrushToString(border.ShadowBrush));

                if (border.Topmost)
                    sb = sb.AppendFormat(";{0}", border.Topmost);

                return sb.ToString();
            }
            else
            {
                var converter = new EnumConverter(typeof(StiBorderSides));

                var sb = new StringBuilder();
                sb = sb.AppendFormat("{0};", converter.ConvertToString(border.Side));
                sb = sb.AppendFormat("{0};", ConvertColorToString(border.Color));
                sb = sb.AppendFormat("{0};", border.Size);
                sb = sb.AppendFormat("{0};", border.Style);
                sb = sb.AppendFormat("{0};", border.DropShadow);
                sb = sb.AppendFormat("{0};", border.ShadowSize);
                sb = sb.Append(ConvertBrushToString(border.ShadowBrush));

                if (border.Topmost)
                    sb = sb.AppendFormat(";{0}", border.Topmost);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Converts a string into a border.
        /// </summary>
        /// <param name="str">String that shows border.</param>
        /// <returns>Border got as result of conversion.</returns>
        public StiBorder ConvertStringToBorder(string str)
        {
            if (str.StartsWithInvariant("Adv"))
            {
                str = str.Substring(3);

                var words = str.Split(';');
                var penStyleConverter = new EnumConverter(typeof(StiPenStyle));

                var topSide = new StiBorderSide();
                var bottomSide = new StiBorderSide();
                var leftSide = new StiBorderSide();
                var rightSide = new StiBorderSide();

                topSide.Color = ConvertStringToColor(words[0]);
                topSide.Size = double.Parse(words[1]);
                topSide.Style = (StiPenStyle)penStyleConverter.ConvertFromString(words[2]);

                bottomSide.Color = ConvertStringToColor(words[3]);
                bottomSide.Size = double.Parse(words[4]);
                bottomSide.Style = (StiPenStyle)penStyleConverter.ConvertFromString(words[5]);

                leftSide.Color = ConvertStringToColor(words[6]);
                leftSide.Size = double.Parse(words[7]);
                leftSide.Style = (StiPenStyle)penStyleConverter.ConvertFromString(words[8]);

                rightSide.Color = ConvertStringToColor(words[9]);
                rightSide.Size = double.Parse(words[10]);
                rightSide.Style = (StiPenStyle)penStyleConverter.ConvertFromString(words[11]);

                if (words.Length == 15)
                {
                    return new StiAdvancedBorder(topSide, bottomSide, leftSide, rightSide,
                        words[12].ToLowerInvariant() == "true",
                        float.Parse(words[13]),
                        ConvertStringToBrush(words[14]));
                }

                return new StiAdvancedBorder(topSide, bottomSide, leftSide, rightSide,
                    words[12].ToLowerInvariant() == "true",
                    float.Parse(words[13]),
                    ConvertStringToBrush(words[14]),
                    words[15].ToLowerInvariant() == "true");

            }
            else
            {
                var words = str.Split(';');
                var sidesConverter = new EnumConverter(typeof(StiBorderSides));
                var penStyleConverter = new EnumConverter(typeof(StiPenStyle));

                Color color;

                int start;
                if (words.Length == 10)
                {
                    start = 5;
                    color = Color.FromArgb(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4]));
                }

                else if (words.Length == 9)
                {
                    start = 4;
                    color = Color.FromArgb(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]));
                }

                else
                {
                    start = 2;
                    color = ConvertStringToColor(words[1]);
                }

                if (words.Length - 1 == start + 4)
                {
                    return new StiBorder(
                        (StiBorderSides)sidesConverter.ConvertFromString(words[0]),
                        color,
                        float.Parse(words[start]),
                        (StiPenStyle)penStyleConverter.ConvertFromString(words[start + 1]),
                        words[start + 2].ToLowerInvariant() == "true",
                        float.Parse(words[start + 3]),
                        ConvertStringToBrush(words[start + 4]));
                }

                return new StiBorder(
                    (StiBorderSides)sidesConverter.ConvertFromString(words[0]),
                    color,
                    float.Parse(words[start]),
                    (StiPenStyle)penStyleConverter.ConvertFromString(words[start + 1]),
                    words[start + 2].ToLowerInvariant() == "true",
                    float.Parse(words[start + 3]),
                    ConvertStringToBrush(words[start + 4]),
                    words[start + 5].ToLowerInvariant() == "true");

            }
        }
        #endregion

        #region Methods.StiSimpleBorder
        /// <summary>
        /// Converts a simple border into a string.
        /// </summary>
        /// <param name="border">Border for conversion.</param>
        /// <returns>String got as result of conversion.</returns>
        public string ConvertSimpleBorderToString(StiSimpleBorder border, bool ignoreDefault = false)
        {
            if (border.IsDefault && !ignoreDefault) 
                return null;

            var converter = new EnumConverter(typeof(StiBorderSides));
            var sb = new StringBuilder();

            sb = sb.AppendFormat("{0};", converter.ConvertToString(border.Side));
            sb = sb.AppendFormat("{0};", ConvertColorToString(border.Color));
            sb = sb.AppendFormat("{0};", border.Size);
            sb = sb.AppendFormat("{0}", border.Style);

            return sb.ToString();
        }

        /// <summary>
        /// Converts a string into a simple border.
        /// </summary>
        /// <param name="str">String that shows border.</param>
        /// <returns>Border got as result of conversion.</returns>
        public StiSimpleBorder ConvertStringToSimpleBorder(string str)
        {
            var words = str.Split(';');
            var sidesConverter = new EnumConverter(typeof(StiBorderSides));
            var penStyleConverter = new EnumConverter(typeof(StiPenStyle));

            return new StiSimpleBorder(
                (StiBorderSides) sidesConverter.ConvertFromString(words[0]),
                ConvertStringToColor(words[1]),
                float.Parse(words[2]),
                (StiPenStyle) penStyleConverter.ConvertFromString(words[3]));
        }
        #endregion

        #region Methods.StiSimpleShadow
        /// <summary>
        /// Converts a simple border into a string.
        /// </summary>
        /// <param name="shadow">Border for conversion.</param>
        /// <returns>String got as result of conversion.</returns>
        public string ConvertSimpleShadowToString(StiSimpleShadow shadow, bool ignoreDefault = false)
        {
            if (shadow.IsDefault && !ignoreDefault)
                return null;

            var sb = new StringBuilder();

            sb = sb.AppendFormat($"{ConvertColorToString(shadow.Color)};");
            sb = sb.AppendFormat($"{shadow.Location.X}, {shadow.Location.Y};");
            sb = sb.AppendFormat($"{shadow.Size};");
            sb = sb.AppendFormat($"{shadow.Visible}");
                        
            return sb.ToString();
        }

        /// <summary>
        /// Converts a string into a simple shadow.
        /// </summary>
        /// <param name="str">String that shows border.</param>
        /// <returns>Border got as result of conversion.</returns>
        public StiSimpleShadow ConvertStringToSimpleShadow(string str)
        {
            var words = str.Split(';');
            var pointCoords = words[1].Split(',');

            return new StiSimpleShadow(
                ConvertStringToColor(words[0]),
                new Point(int.Parse(pointCoords[0]), int.Parse(pointCoords[1])),
                int.Parse(words[2]),
                words[3].ToLowerInvariant() == "true");
        }
        #endregion

        static StiReportObjectStringConverter()
        {
            StiSerializing.AddStringType(typeof(StiPage), "Page");
            StiSerializing.AddStringType(typeof(StiReportTitleBand), "ReportTitleBand");
            StiSerializing.AddStringType(typeof(StiReportSummaryBand), "ReportSummaryBand");
            StiSerializing.AddStringType(typeof(StiPageHeaderBand), "PageHeaderBand");
            StiSerializing.AddStringType(typeof(StiPageFooterBand), "PageFooterBand");
            StiSerializing.AddStringType(typeof(StiGroupHeaderBand), "GroupHeaderBand");
            StiSerializing.AddStringType(typeof(StiGroupFooterBand), "GroupFooterBand");
            StiSerializing.AddStringType(typeof(StiHeaderBand), "HeaderBand");
            StiSerializing.AddStringType(typeof(StiFooterBand), "FooterBand");
            StiSerializing.AddStringType(typeof(StiDataBand), "DataBand");
            StiSerializing.AddStringType(typeof(StiChildBand), "ChildBand");

            StiSerializing.AddStringType(typeof(StiCrossGroupHeaderBand), "CrossGroupHeaderBand");
            StiSerializing.AddStringType(typeof(StiCrossGroupFooterBand), "CrossGroupFooterBand");
            StiSerializing.AddStringType(typeof(StiCrossDataBand), "CrossDataBand");
            StiSerializing.AddStringType(typeof(StiCheckBox), "CheckBox");
            StiSerializing.AddStringType(typeof(StiContourText), "ContourText");
            StiSerializing.AddStringType(typeof(StiSubReport), "SubReport");
            StiSerializing.AddStringType(typeof(StiShape), "Shape");
            StiSerializing.AddStringType(typeof(StiSystemText), "SystemText");
            StiSerializing.AddStringType(typeof(StiImage), "Image");

            StiSerializing.AddStringType(typeof(StiContainer), "Container");
            StiSerializing.AddStringType(typeof(StiText), "Text");
            StiSerializing.AddStringType(typeof(StiClone), "Clone");

            StiSerializing.AddStringType(typeof(StiCap), "Cap");
            StiSerializing.AddStringType(typeof(StiVerticalLinePrimitive), "VerticalLinePrimitive");
            StiSerializing.AddStringType(typeof(StiHorizontalLinePrimitive), "HorizontalLinePrimitive");
            StiSerializing.AddStringType(typeof(StiRectanglePrimitive), "RectanglePrimitive");

            StiSerializing.AddStringType(typeof(StiCrossTitle), "CrossTitle");
            StiSerializing.AddStringType(typeof(StiCrossColumn), "CrossColumn");
            StiSerializing.AddStringType(typeof(StiCrossRow), "CrossRow");
            StiSerializing.AddStringType(typeof(StiCrossColumnTotal), "CrossColumnTotal");
            StiSerializing.AddStringType(typeof(StiCrossRowTotal), "CrossRowTotal");
            StiSerializing.AddStringType(typeof(StiCrossSummary), "CrossSummary");

            StiSerializing.AddStringType(typeof(StiGeneralFormatService), "GeneralFormat");
            StiSerializing.AddStringType(typeof(StiNumberFormatService), "NumberFormat");
            StiSerializing.AddStringType(typeof(StiCurrencyFormatService), "CurrencyFormat");
            StiSerializing.AddStringType(typeof(StiPercentageFormatService), "PercentageFormat");
            StiSerializing.AddStringType(typeof(StiDateFormatService), "DateFormat");
            StiSerializing.AddStringType(typeof(StiTimeFormatService), "TimeFormat");
            StiSerializing.AddStringType(typeof(StiBooleanFormatService), "BooleanFormat");
            StiSerializing.AddStringType(typeof(StiCustomFormatService), "CustomFormat");

            StiSerializing.AddStringType(typeof(StiCentimetersUnit), "cm");
            StiSerializing.AddStringType(typeof(StiHundredthsOfInchUnit), "hi");
            StiSerializing.AddStringType(typeof(StiInchesUnit), "in");
            StiSerializing.AddStringType(typeof(StiMillimetersUnit), "mm");

            StiSerializing.AddStringType(typeof(StiDictionary), "Dictionary");
            StiSerializing.AddStringType(typeof(StiDataTableSource), "DataTableSource");
            StiSerializing.AddStringType(typeof(StiDataRelation), "DataRelation");
            StiSerializing.AddStringType(typeof(StiReport), "Report");
            StiSerializing.AddStringType(typeof(StiCSharpLanguage), "CSharp");
            StiSerializing.AddStringType(typeof(StiVBLanguage), "VB");
            StiSerializing.AddStringType(typeof(StiRenderProviderV1), "RenderProvider");//Back compability

            StiSerializing.AddStringType(typeof(StiHighlightCondition), "HighlightCondition");
            StiSerializing.AddStringType(typeof(StiConditionsCollection), "Conditions");
            StiSerializing.AddStringType(typeof(StiBookmark), "Bookmark");

            StiSerializing.AddStringType(typeof(StiPanel), "Panel");
            StiSerializing.AddStringType(typeof(StiTableCell), "TableCell");

            #region Dialogs
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiForm", "Form");

            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiButtonControl", "Stimulsoft.Report.ReportControls.StiButtonControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiCheckBoxControl", "Stimulsoft.Report.ReportControls.StiCheckBoxControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiCheckedListBoxControl", "Stimulsoft.Report.ReportControls.StiCheckedListBoxControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiComboBoxControl", "Stimulsoft.Report.ReportControls.StiComboBoxControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiDateTimePickerControl", "Stimulsoft.Report.ReportControls.StiDateTimePickerControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiForm", "Stimulsoft.Report.ReportControls.StiForm");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiGridControl", "Stimulsoft.Report.ReportControls.StiGridControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiGroupBoxControl", "Stimulsoft.Report.ReportControls.StiGroupBoxControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiLabelControl", "Stimulsoft.Report.ReportControls.StiLabelControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiListBoxControl", "Stimulsoft.Report.ReportControls.StiListBoxControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiLookUpBoxControl", "Stimulsoft.Report.ReportControls.StiLookUpBoxControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiNumericUpDownControl", "Stimulsoft.Report.ReportControls.StiNumericUpDownControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiPanelControl", "Stimulsoft.Report.ReportControls.StiPanelControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiPictureBoxControl", "Stimulsoft.Report.ReportControls.StiPictureBoxControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiRadioButtonControl", "Stimulsoft.Report.ReportControls.StiRadioButtonControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiTextBoxControl", "Stimulsoft.Report.ReportControls.StiTextBoxControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiRichTextBoxControl", "Stimulsoft.Report.ReportControls.StiRichTextBoxControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiTreeViewControl", "Stimulsoft.Report.ReportControls.StiTreeViewControl");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.Dialogs.StiListViewControl", "Stimulsoft.Report.ReportControls.StiListViewControl");
            #endregion

            #region BarCodes
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiBarCode", "Stimulsoft.Report.Components.StiBarCode");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiCodabarBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiCodabarBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiCode128aBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiCode128aBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiCode128bBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiCode128bBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiCode128cBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiCode128cBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiCode39BarCodeType", "Stimulsoft.Report.Components.BarCodes.StiCode39BarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiCode39ExtBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiCode39ExtBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiCode93BarCodeType", "Stimulsoft.Report.Components.BarCodes.StiCode93BarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiCode93ExtBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiCode93ExtBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiEAN128aBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiEAN128aBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiEAN128bBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiEAN128bBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiEAN128cBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiEAN128cBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiEAN13BarCodeType", "Stimulsoft.Report.Components.BarCodes.StiEAN13BarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiEAN8BarCodeType", "Stimulsoft.Report.Components.BarCodes.StiEAN8BarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiInterleaved2of5BarCodeType", "Stimulsoft.Report.Components.BarCodes.StiInterleaved2of5BarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiMsiBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiMsiBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiPlesseyBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiPlesseyBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiPostnetBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiPostnetBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiStandard2of5BarCodeType", "Stimulsoft.Report.Components.BarCodes.StiStandard2of5BarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiUpcABarCodeType", "Stimulsoft.Report.Components.BarCodes.StiUpcABarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiUpcEBarCodeType", "Stimulsoft.Report.Components.BarCodes.StiUpcEBarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiUpcSup2BarCodeType", "Stimulsoft.Report.Components.BarCodes.StiUpcSup2BarCodeType");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Report.BarCodes.StiUpcSup5BarCodeType", "Stimulsoft.Report.Components.BarCodes.StiUpcSup5BarCodeType");
            #endregion

            #region Viewer
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Viewer.StiPageViewMode", "Stimulsoft.Report.Render.StiPageViewMode");
            StiSerializing.AddSourceTypeToDestinationType("Stimulsoft.Viewer.StiPreviewSettings", "Stimulsoft.Report.Render.StiPreviewSettings");
            #endregion
        }

        public StiReportObjectStringConverter() : this(false)
        {
        }

        public StiReportObjectStringConverter(bool allowLoadOptimization)
        {
            this.AllowLoadOptimization = allowLoadOptimization;
        }
    }
}
