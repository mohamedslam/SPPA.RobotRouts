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
using Stimulsoft.Base.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Stimulsoft.Base.Json;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base
{
    public static class StiJsonReportObjectHelper
    {
        #region Extensions
        public static StiSimpleShadow DeserializeSimpleShadow(this JProperty property)
        {
            return StiJsonReportObjectHelper.Deserialize.SimpleShadow(property);
        }

        public static StiBrush DeserializeBrush(this JProperty property)
        {
            return Deserialize.Brush(property);
        }

        public static StiCornerRadius DeserializeCornerRadius(this JProperty property)
        {
            return StiCornerRadius.TryParse(property.DeserializeString());
        }

        public static StiBorder DeserializeBorder(this JProperty property)
        {
            return Deserialize.Border(property);
        }

        public static StiSimpleBorder DeserializeSimpleBorder(this JProperty property)
        {
            return Deserialize.SimpleBorder(property);
        }

        public static Color DeserializeColor(this JProperty property)
        {
            return Deserialize.Color(property.Value.ToObject<string>());
        }

        public static Color[] DeserializeArrayColor(this JProperty property)
        {
            return Deserialize.ColorArray((JObject)property.Value);
        }

        public static double DeserializeDouble(this JProperty property)
        {
            return property.Value.ToObject<double>();
        }

        public static float DeserializeFloat(this JProperty property)
        {
            return property.Value.ToObject<float>();
        }

        public static decimal DeserializeDecimal(this JProperty property)
        {
            return property.Value.ToObject<decimal>();
        }

        public static float? DeserializeFloatNullable(this JProperty property)
        {
            return property.Value.ToObject<float?>();
        }

        public static DateTime DeserializeDateTime(this JProperty property)
        {
            return Deserialize.DateTime(property);
        }

        public static TimeSpan DeserializeTimeSpan(this JProperty property)
        {
            return new TimeSpan(property.Value.ToObject<long>());
        }

        public static int DeserializeInt(this JProperty property)
        {
            return property.Value.ToObject<int>();
        }

        public static string DeserializeString(this JProperty property)
        {
            return property.Value.ToObject<string>();
        }

        public static string[] DeserializeStringArray(this JProperty property)
        {
            return Deserialize.StringArray((JObject)property.Value);
        }

        public static bool[] DeserializeBoolArray(this JProperty property)
        {
            return Deserialize.BoolArray((JObject)property.Value);
        }

        public static List<string> DeserializeListString(this JProperty property)
        {
            return Deserialize.ListString((JObject)property.Value);
        }

        public static bool DeserializeBool(this JProperty property)
        {
            return property.Value.ToObject<bool>();
        }

        public static Point DeserializePoint(this JProperty property)
        {
            return Deserialize.Point((JObject)property.Value);
        }

        public static PointF DeserializePointF(this JProperty property)
        {
            return Deserialize.PointF((JObject)property.Value);
        }

        public static Font DeserializeFont(this JProperty property, Font defaultFont)
        {
            return Deserialize.Font(property, defaultFont);
        }

        public static StiCap DeserializeCap(this JProperty property)
        {
            return Deserialize.JCap(property.Value.ToObject<string>());
        }

        public static int[] DeserializeIntArray(this JProperty property)
        {
            return Deserialize.IntArray((JObject)property.Value);
        }

        public static byte[] DeserializeByteArray(this JProperty property)
        {
            return StiImageConverter.StringToByteArray(property.Value.ToObject<string>());
        }

        public static Image DeserializeImage(this JProperty property)
        {
            return StiImageConverter.StringToImage(property.Value.ToObject<string>());
        }

        public static SizeD DeserializeSizeD(this JProperty property)
        {
            return Deserialize.SizeD(property);
        }

        public static Size DeserializeSize(this JProperty property)
        {
            return Deserialize.Size(property);
        }

        public static RectangleD DeserializeRectangleD(this JProperty property)
        {
            return Deserialize.RectangleD(property);
        }

        public static T DeserializeEnum<T>(this JProperty property)
        {
            return (T)Enum.Parse(typeof(T), property.Value.ToObject<string>());
        }
        #endregion

        #region Serialize
        public static class Serialize
        {
            public static string FontTahoma12Bold(Font font)
            {
                return Font(font, "Tahoma", 12, FontStyle.Bold, GraphicsUnit.Point);
            }

            public static string FontArial8(Font font)
            {
                return Font(font, "Arial", 8, FontStyle.Regular, GraphicsUnit.Point);
            }
            
            public static string FontMicrosoftSansSerif8(Font font)
            {
                return Font(font, "Microsoft Sans Serif", 8, FontStyle.Regular, GraphicsUnit.Point);
            }
            
            public static string FontTahoma8(Font font)
            {
                return Font(font, "Tahoma", 8, FontStyle.Regular, GraphicsUnit.Point);
            }

            public static string FontArial8BoldPixel(Font font)
            {
                return Font(font, "Arial", 8, FontStyle.Bold, GraphicsUnit.Pixel);
            }

            public static string FontArial7(Font font)
            {
                return Font(font, "Arial", 7, FontStyle.Regular, GraphicsUnit.Point);
            }

            public static string FontArial10(Font font)
            {
                return Font(font, "Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            }

            public static string FontArial13(Font font)
            {
                return Font(font, "Arial", 13, FontStyle.Regular, GraphicsUnit.Point);
            }

            public static string FontArial11(Font font)
            {
                return Font(font, "Arial", 11, FontStyle.Regular, GraphicsUnit.Point);
            }

            public static string FontArial12Bold(Font font)
            {
                return Font(font, "Arial", 12, FontStyle.Bold);
            }

            public static string FontSegoeUI12Bold(Font font)
            {
                return Font(font, "Segoe UI", 12, FontStyle.Bold);
            }

            public static string FontArial14Bold(Font font)
            {
                return Font(font, "Arial", 14, FontStyle.Bold);
            }

            public static string Font(Font font, string defaultFamily, float defaultEmSize)
            {
                return Font(font, defaultFamily, defaultEmSize, FontStyle.Regular, GraphicsUnit.Point);
            }

            public static string Font(Font font, string defaultFamily, float defaultEmSize, FontStyle fontStyle)
            {
                return Font(font, defaultFamily, defaultEmSize, fontStyle, GraphicsUnit.Point);
            }

            public static string Font(Font font, string defaultFamily, float defaultEmSize, FontStyle defaultStyle, GraphicsUnit defaultUnit)
            {
                if (font == null)
                    return string.Empty;

                string fontFamily = null;
                string size = null;
                string style = null;
                string unit = null;

                int count = 0;
                if (font.FontFamily.Name != defaultFamily)
                {
                    count++;
                    fontFamily = font.FontFamily.Name;
                }
                if ((font.Size != defaultEmSize))
                {
                    count++;
                    size = font.Size.ToString();
                }
                if (font.Style != defaultStyle)
                {
                    count++;
                    style = font.Style.ToString();
                }
                if (font.Unit != defaultUnit)
                {
                    count++;
                    unit = font.Unit.ToString();
                }

                if (count == 0)
                    return null;

                return fontFamily + ";" + size + ";" + style + ";" + unit;
            }

            public static string RectangleD(RectangleD rect)
            {
                return rect.X + "," + rect.Y + "," + rect.Width + "," + rect.Height;
            }

            public static string SizeD(SizeD size)
            {
                return size.Width + "," + size.Height;
            }

            public static string JColor(Color color, Color defColor)
            {
                if (color == defColor)
                    return null;

                return JColor(color);
            }

            public static string JColor(Color color)
            {
                if (color.IsNamedColor)
                {
                    return color.Name;
                }
                else
                {
                    if (color.A == 255)
                        return
                        color.R.ToString() + "," +
                        color.G.ToString() + "," +
                        color.B.ToString();
                    else
                        return
                        color.A.ToString() + "," +
                        color.R.ToString() + "," +
                        color.G.ToString() + "," +
                        color.B.ToString();
                }
            }

            public static JObject StringArray(string[] array)
            {
                if (array == null || array.Length == 0)
                    return null;

                var jObject = new JObject();

                for (int index = 0; index < array.Length; index++ )
                {
                    jObject.AddPropertyString(index.ToString(), array[index]);
                }

                return jObject;
            }

            public static JObject ListString(List<string> array)
            {
                if (array == null || array.Count == 0)
                    return null;

                var jObject = new JObject();

                for (int index = 0; index < array.Count; index++)
                {
                    jObject.AddPropertyString(index.ToString(), array[index]);
                }

                return jObject;
            }

            public static JObject BoolArray(bool[] array, bool ignoreDefaultValues = false)
            {
                if (array == null || array.Length == 0)
                    return null;

                var jObject = new JObject();

                for (int index = 0; index < array.Length; index++)
                {
                    jObject.AddPropertyBool(index.ToString(), array[index], false, ignoreDefaultValues);
                }

                return jObject;
            }

            public static JObject IntArray(int[] array)
            {
                if (array == null || array.Length == 0)
                    return null;

                var jObject = new JObject();

                for (int index = 0; index < array.Length; index++)
                {
                    jObject.AddPropertyStringNullOrEmpty(index.ToString(), array[index].ToString());
                }

                return jObject;
            }

            public static JObject JObjectCollection(IEnumerable<IStiJsonReportObject> list, StiJsonSaveMode mode)
            {
                if (list == null || list.Count() == 0)
                    return null;

                var jObject = new JObject();

                var index = 0;
                foreach(var item in list)
                {
                    jObject.AddPropertyJObject(index.ToString(), item.SaveToJsonObject(mode));
                    index++;
                }

                return jObject;
            }

            public static JObject Size(Size size)
            {
                var jObject = new JObject();

                jObject.AddPropertyInt("Width", size.Width);
                jObject.AddPropertyInt("Height", size.Height);

                return jObject;
            }

            public static JObject Point(Point pos)
            {
                var jObject = new JObject();

                jObject.AddPropertyStringNullOrEmpty("X", pos.X.ToString());
                jObject.AddPropertyStringNullOrEmpty("Y", pos.Y.ToString());

                return jObject;
            }

            public static JObject PointF(PointF pos)
            {
                var jObject = new JObject();

                jObject.AddPropertyStringNullOrEmpty("X", pos.X.ToString());
                jObject.AddPropertyStringNullOrEmpty("Y", pos.Y.ToString());

                return jObject;
            }

            public static string JCap(StiCap cap)
            {
                var builder = new StringBuilder();

                // Width
                if (cap.Width != 10)
                    builder.Append(cap.Width);
                builder.Append(";");

                // Height
                if (cap.Height != 10)
                    builder.Append(cap.Height);
                builder.Append(";");

                // Style
                if (cap.Style != StiCapStyle.None)
                    builder.Append(cap.Style.ToString());
                builder.Append(";");

                // Color
                builder.Append(JColor(cap.Color, Color.Black));

                return builder.ToString();
            }

            public static string JBrush(StiBrush brush, StiBrush defaultBrush = null)
            {
                var builder = new StringBuilder();

                #region StiStyleBrush
                if (brush is StiStyleBrush)
                {
                    builder.Append("style");
                }
                #endregion

                #region StiDefaultBrush
                if (brush is StiDefaultBrush)
                {
                    builder.Append("default");
                }
                #endregion

                #region StiSolidBrush
                else if (brush is StiSolidBrush)
                {
                    var solid = (StiSolidBrush)brush;

                    builder.Append("solid:");

                    // Color
                    builder.Append(JColor(solid.Color, Color.Transparent));
                }
                #endregion

                #region StiEmptyBrush
                else if (brush is StiEmptyBrush)
                {
                    builder.Append("empty");
                }
                #endregion

                #region StiGlareBrush
                else if (brush is StiGlareBrush)
                {
                    var glare = (StiGlareBrush)brush;

                    builder.Append("glare:");

                    // StartColor
                    builder.Append(JColor(glare.StartColor, Color.Black));
                    builder.Append(":");

                    // EndColor
                    builder.Append(JColor(glare.EndColor, Color.White));
                    builder.Append(":");

                    // Angle
                    if (glare.Angle != 0.0d)
                        builder.Append(glare.Angle);
                    builder.Append(":");

                    // Focus
                    if (glare.Focus != 0.5f)
                        builder.Append(glare.Focus);
                    builder.Append(":");

                    // Scale
                    if (glare.Scale != 1.0f)
                        builder.Append(glare.Scale);
                }
                #endregion

                #region StiGlassBrush
                else if (brush is StiGlassBrush)
                {
                    var glass = (StiGlassBrush)brush;

                    builder.Append("glass:");

                    // Color
                    builder.Append(JColor(glass.Color, Color.Silver));
                    builder.Append(":");

                    // Angle
                    if (glass.DrawHatch)
                        builder.Append(glass.DrawHatch);
                    builder.Append(":");

                    // Focus
                    if (glass.Blend != 0.2f)
                        builder.Append(glass.Blend);
                }
                #endregion

                #region StiGradientBrush
                else if (brush is StiGradientBrush)
                {
                    var gradient = (StiGradientBrush)brush;

                    builder.Append("gradient:");

                    // StartColor
                    builder.Append(JColor(gradient.StartColor, Color.Black));
                    builder.Append(":");

                    // EndColor
                    builder.Append(JColor(gradient.EndColor, Color.White));
                    builder.Append(":");

                    // Angle
                    if (gradient.Angle != 0.0d)
                        builder.Append(gradient.Angle);
                }
                #endregion

                #region StiHatchBrush
                else if (brush is StiHatchBrush)
                {
                    var hatch = (StiHatchBrush)brush;

                    builder.Append("hatch:");

                    // BackColor
                    builder.Append(JColor(hatch.BackColor, Color.Black));
                    builder.Append(":");

                    // ForeColor
                    builder.Append(JColor(hatch.ForeColor, Color.White));
                    builder.Append(":");

                    // BackwardDiagonal
                    if (hatch.Style != HatchStyle.BackwardDiagonal)
                        builder.Append(hatch.Style);
                }
                #endregion

                return builder.ToString();
            }

            public static string JBorderSide(StiBorderSide side)
            {
                string color = JColor(side.Color, Color.Black);

                string size = string.Empty;
                if (side.Size != 1d)
                    size = side.Size.ToString();

                string style = string.Empty;
                if (side.Style != StiPenStyle.None)
                    style = side.Style.ToString();

                return color + ":" + size + ":" + style;
            }

            public static string JBorder(StiBorder border)
            {
                var builder = new StringBuilder();

                var advancedBorder = border as StiAdvancedBorder;
                if (advancedBorder != null)
                {
                    // TopSide
                    builder.Append(JBorderSide(advancedBorder.TopSide));
                    builder.Append(";");

                    // BottomSide
                    builder.Append(JBorderSide(advancedBorder.BottomSide));
                    builder.Append(";");

                    // LeftSide
                    builder.Append(JBorderSide(advancedBorder.LeftSide));
                    builder.Append(";");

                    // RightSide
                    builder.Append(JBorderSide(advancedBorder.RightSide));
                    builder.Append(";");

                    // DropShadow
                    if (border.DropShadow)
                        builder.Append(border.DropShadow);
                    builder.Append(";");

                    // ShadowSize
                    if (border.ShadowSize != 4d)
                        builder.Append(border.ShadowSize);
                    builder.Append(";");

                    // ShadowBrush
                    builder.Append(JBrush(border.ShadowBrush, new StiSolidBrush(Color.Black)));

                }
                else
                {
                    // Side
                    if (border.Side != StiBorderSides.None)
                        builder.Append(border.Side);
                    builder.Append(";");

                    // Color
                    if (border.Color != Color.Black)
                        builder.Append(JColor(border.Color));
                    builder.Append(";");

                    // Size
                    if (border.Size != 1d)
                        builder.Append(border.Size);
                    builder.Append(";");

                    // Style
                    if (border.Style != StiPenStyle.Solid)
                        builder.Append(border.Style);
                    builder.Append(";");

                    // ShadowSize
                    if (border.ShadowSize != 4d)
                        builder.Append(border.ShadowSize);
                    builder.Append(";");

                    // DropShadow
                    if (border.DropShadow)
                        builder.Append(border.DropShadow);
                    builder.Append(";");

                    // Topmost
                    if (border.Topmost)
                        builder.Append(border.Topmost);
                    builder.Append(";");

                    // ShadowBrush
                    builder.Append(JBrush(border.ShadowBrush, new StiSolidBrush(Color.Black)));
                }

                return builder.ToString();
            }

            public static string JBorder(StiSimpleBorder border)
            {
                var builder = new StringBuilder();
                
                // Side
                if (border.Side != StiBorderSides.None)
                    builder.Append(border.Side);

                builder.Append(";");

                // Color
                if (border.Color != Color.Gray)
                    builder.Append(JColor(border.Color));

                builder.Append(";");

                // Size
                if (border.Size != 1d)
                    builder.Append(border.Size);

                builder.Append(";");

                // Style
                if (border.Style != StiPenStyle.Solid)
                    builder.Append(border.Style);

                builder.Append(";");

                return builder.ToString();
            }

            public static string JShadow(StiSimpleShadow shadow)
            {
                var builder = new StringBuilder();

                // Color
                if (shadow.Color != StiColor.Get("#44222222"))
                    builder.Append(JColor(shadow.Color));

                builder.Append(";");

                // Location
                if (!(shadow.Location.X == 2 && shadow.Location.Y == 2))
                    builder.Append(shadow.Location.X + "," + shadow.Location.Y);

                builder.Append(";");

                // Size
                if (shadow.Size != 5)
                    builder.Append(shadow.Size);

                builder.Append(";");

                // Visible
                if (shadow.Visible)
                    builder.Append(shadow.Visible);

                return builder.ToString();
            }
        }
        #endregion

        #region Deserialize
        public static class Deserialize
        {
            public static List<string> ListString(JObject jObject)
            {
                var result = new List<string>();

                int index = 0;
                foreach (var prop in jObject.Properties())
                {
                    result.Add(prop.Value.ToObject<string>());
                    index++;
                }

                return result;
            }

            public static string[] StringArray(JObject jObject)
            {
                var result = new string[jObject.Count];
                
                int index = 0;
                foreach (var prop in jObject.Properties())
                {
                    result[index] = prop.Value.ToObject<string>();
                    index++;
                }

                return result;
            }

            public static bool[] BoolArray(JObject jObject)
            {
                var result = new bool[jObject.Count];

                int index = 0;
                foreach (var prop in jObject.Properties())
                {
                    result[index] = prop.Value.ToObject<bool>();
                    index++;
                }

                return result;
            }

            public static int[] IntArray(JObject jObject)
            {
                var result = new int[jObject.Count];

                int index = 0;
                foreach (var prop in jObject.Properties())
                {
                    result[index] = prop.Value.ToObject<int>();
                    index++;
                }

                return result;
            }

            public static Font Font(JProperty prop, Font defaultFont)
            {
                if (prop.Value is JValue)
                    return Font((string)((JValue)prop.Value).Value, defaultFont);
                else
                    return Font((JObject)prop.Value, defaultFont);
            }

            private static Font Font(string text, Font defaultFont)
            {
                var values = text.Split(';');
                if (values.Length != 4)
                    throw new Exception("Parsing Error");

                string defaultFamily = defaultFont.FontFamily.Name;
                float defaultEmSize = defaultFont.Size;
                FontStyle defaultStyle = defaultFont.Style;
                GraphicsUnit defaultUnit = defaultFont.Unit;

                if (!string.IsNullOrEmpty(values[0]))
                    defaultFamily = values[0];
                if (!string.IsNullOrEmpty(values[1]))
                    defaultEmSize = float.Parse(values[1]);
                if (!string.IsNullOrEmpty(values[2]))
                    defaultStyle = (FontStyle)Enum.Parse(typeof(FontStyle), values[2]);
                if (!string.IsNullOrEmpty(values[3]))
                    defaultUnit = (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), values[3]);

                var fontFamily = StiFontCollection.GetFontFamily(defaultFamily, defaultStyle, true);
                if (fontFamily != null)
                    return new Font(fontFamily, defaultEmSize, defaultStyle, defaultUnit);
                return new Font(defaultFamily, defaultEmSize, defaultStyle, defaultUnit);
            }

            private static Font Font(JObject jObject, Font defaultFont)
            {
                string familyName = defaultFont.FontFamily.Name;
                float emSize = defaultFont.Size;
                FontStyle style = defaultFont.Style;
                GraphicsUnit unit = defaultFont.Unit;

                foreach (var property in jObject.Properties())
                {
                    switch (property.Name)
                    {
                        case "FamilyName":
                            familyName = property.Value.ToObject<string>();
                            break;

                        case "Size":
                            emSize = property.Value.ToObject<float>();
                            break;

                        case "Style":
                            style = (FontStyle)Enum.Parse(typeof(FontStyle), property.Value.ToObject<string>());
                            break;

                        case "Unit":
                            unit = (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), property.Value.ToObject<string>());
                            break;
                    }
                }

                return new Font(familyName, emSize, style, unit);
            }

            public static StiBorderSide JBorderSide(string text)
            {
                var values = text.Split(':');
                var side = new StiBorderSide();

                if (!string.IsNullOrEmpty(values[0]))
                    side.Color = Color(values[0]);

                if (!string.IsNullOrEmpty(values[1]))
                    side.Size = double.Parse(values[1]);

                if (!string.IsNullOrEmpty(values[2]))
                    side.Style = (StiPenStyle)Enum.Parse(typeof(StiPenStyle), values[2]);

                return side;
            }

            public static StiCap JCap(string text)
            {
                var values = text.Split(';');
                var cap = new StiCap();

                if (values.Length != 4)
                    throw new Exception("Parsing Error");

                if (!string.IsNullOrEmpty(values[0]))
                    cap.Width = int.Parse(values[0]);

                if (!string.IsNullOrEmpty(values[1]))
                    cap.Height = int.Parse(values[1]);

                if (!string.IsNullOrEmpty(values[2]))
                    cap.Style = (StiCapStyle)Enum.Parse(typeof(StiCapStyle), values[2]);

                if (!string.IsNullOrEmpty(values[3]))
                    cap.Color = Color(values[3]);

                return cap;
            }

            public static StiBorder Border(JProperty prop)
            {
                if (prop.Value is JValue)
                {
                    return Border((string)((JValue)prop.Value).Value);
                }
                else
                {
                    var border = new StiBorder();
                    border.LoadFromJson((JObject)prop.Value);
                    return border;
                }
            }

            private static StiBorder Border(string text)
            {
                var values = text.Split(';');
                if (values.Length == 7)
                {
                    bool dropShadow = false;
                    double shadowSize = 4d;
                    var shadowBrush = new StiSolidBrush(global::System.Drawing.Color.Black);

                    return new StiAdvancedBorder(JBorderSide(values[0]), JBorderSide(values[1]),
                        JBorderSide(values[2]), JBorderSide(values[3]), dropShadow, shadowSize, shadowBrush);
                }
                else
                {
                    var border = new StiBorder();

                    // Side
                    if (!string.IsNullOrEmpty(values[0]))
                        border.Side = (StiBorderSides)Enum.Parse(typeof(StiBorderSides), values[0]);

                    // Color
                    if (!string.IsNullOrEmpty(values[1]))
                        border.Color = Color(values[1]);

                    // Size
                    if (!string.IsNullOrEmpty(values[2]))
                        border.Size = double.Parse(values[2]);

                    // Style
                    if (!string.IsNullOrEmpty(values[3]))
                        border.Style = (StiPenStyle)Enum.Parse(typeof(StiPenStyle), values[3]);

                    // ShadowSize
                    if (values.Length > 4 && !string.IsNullOrEmpty(values[4]))
                        border.ShadowSize = double.Parse(values[4]);

                    // DropShadow
                    if (values.Length > 5 && !string.IsNullOrEmpty(values[5]))
                        border.DropShadow = true;

                    // Topmost
                    if (values.Length > 6 && !string.IsNullOrEmpty(values[6]))
                        border.Topmost = true;

                    // ShadowBrush
                    if (values.Length > 7 && !string.IsNullOrEmpty(values[7]))
                        border.ShadowBrush = Brush(values[7]);

                    return border;
                }
            }

            public static StiSimpleBorder SimpleBorder(JProperty prop)
            {
                var text = (string) ((JValue) prop.Value).Value;
                var values = text.Split(';');
                var border = new StiSimpleBorder();

                // Side
                if (!string.IsNullOrEmpty(values[0]))
                    border.Side = (StiBorderSides) Enum.Parse(typeof(StiBorderSides), values[0]);

                // Color
                if (!string.IsNullOrEmpty(values[1]))
                    border.Color = Color(values[1]);

                // Size
                if (!string.IsNullOrEmpty(values[2]))
                    border.Size = double.Parse(values[2]);

                // Style
                if (!string.IsNullOrEmpty(values[3]))
                    border.Style = (StiPenStyle) Enum.Parse(typeof(StiPenStyle), values[3]);

                return border;
            }

            public static StiSimpleShadow SimpleShadow(JProperty prop)
            {
                var text = (string)((JValue)prop.Value).Value;
                var values = text.Split(';');
                var shadow = new StiSimpleShadow();

                // Color
                if (!string.IsNullOrEmpty(values[0]))
                    shadow.Color = Color(values[0]);

                // Location
                if (!string.IsNullOrEmpty(values[1]))
                {
                    var strs = values[1].Split(',');
                    shadow.Location = new Point(int.Parse(strs[0]), int.Parse(strs[1]));
                }

                // Size
                if (!string.IsNullOrEmpty(values[2]))
                    shadow.Size = int.Parse(values[2]);

                // Visible
                if (!string.IsNullOrEmpty(values[3]))
                    shadow.Visible = true;

                return shadow;
            }

            public static Color Color(string value)
            {
                if (value.IndexOf(",", StringComparison.InvariantCulture) != -1)
                {
                    string[] strs = value.Split(new char[] { ',' });
                    if (strs.Length == 4) return global::System.Drawing.Color.FromArgb(
                                              int.Parse(strs[0].Trim()),
                                              int.Parse(strs[1].Trim()),
                                              int.Parse(strs[2].Trim()),
                                              int.Parse(strs[3].Trim()));

                    return global::System.Drawing.Color.FromArgb(
                        int.Parse(strs[0].Trim()),
                        int.Parse(strs[1].Trim()),
                        int.Parse(strs[2].Trim()));
                }
                else if (value.StartsWith("#"))
                {
                    return ColorTranslator.FromHtml(value);
                }
                else
                {
                    return global::System.Drawing.Color.FromName(value);
                }
            }

            public static StiBrush Brush(JProperty prop)
            {
                if (prop.Value is JValue)
                {
                    return Brush((string)((JValue)prop.Value).Value);
                }
                else
                {
                    return StiBrush.LoadFromJson((JObject)prop.Value);
                }
            }

            private static StiBrush Brush(string text)
            {
                var values = text.Split(':');

                switch (values[0])
                {
                    #region StiStyleBrush
                    case "style":
                        return new StiStyleBrush();
                    #endregion

                    #region StiDefaultBrush
                    case "default":
                        return new StiDefaultBrush();
                    #endregion

                    #region StiEmptyBrush
                    case "empty":
                        return new StiEmptyBrush();
                    #endregion

                    #region StiGlassBrush
                    case "glass":
                        {
                            var glass = new StiGlassBrush();

                            if (!string.IsNullOrEmpty(values[1]))
                                glass.Color = Color(values[1]);

                            if (!string.IsNullOrEmpty(values[2]))
                                glass.DrawHatch = true;

                            if (!string.IsNullOrEmpty(values[3]))
                                glass.Blend = float.Parse(values[3]);

                            return glass;
                        }
                    #endregion

                    #region StiGlareBrush
                    case "glare":
                        {
                            var glare = new StiGlareBrush();

                            if (!string.IsNullOrEmpty(values[1]))
                                glare.StartColor = Color(values[1]);

                            if (!string.IsNullOrEmpty(values[2]))
                                glare.EndColor = Color(values[2]);

                            if (!string.IsNullOrEmpty(values[3]))
                                glare.Angle = double.Parse(values[3]);

                            if (!string.IsNullOrEmpty(values[4]))
                                glare.Focus = float.Parse(values[4]);

                            if (!string.IsNullOrEmpty(values[5]))
                                glare.Scale = float.Parse(values[5]);

                            return glare;
                        }
                    #endregion

                    #region StiHatchBrush
                    case "hatch":
                        {
                            var hatch = new StiHatchBrush();

                            if (!string.IsNullOrEmpty(values[1]))
                                hatch.BackColor = Color(values[1]);

                            if (!string.IsNullOrEmpty(values[2]))
                                hatch.ForeColor = Color(values[2]);

                            if (!string.IsNullOrEmpty(values[3]))
                                hatch.Style = (HatchStyle)Enum.Parse(typeof(HatchStyle), values[3]);

                            return hatch;
                        }
                    #endregion

                    #region StiGradientBrush
                    case "gradient":
                        {
                            var gradient = new StiGradientBrush();

                            if (!string.IsNullOrEmpty(values[1]))
                                gradient.StartColor = Color(values[1]);

                            if (!string.IsNullOrEmpty(values[2]))
                                gradient.EndColor = Color(values[2]);

                            if (!string.IsNullOrEmpty(values[3]))
                                gradient.Angle = double.Parse(values[3]);

                            return gradient;
                        }
                    #endregion

                    #region StiSolidBrush
                    case "solid":
                        {
                            var solid = new StiSolidBrush();

                            if (!string.IsNullOrEmpty(values[1]))
                                solid.Color = Color(values[1]);

                            return solid;
                        }
                    #endregion
                }

                return null;
            }

            public static Color[] ColorArray(JObject jObject)
            {
                var result = new Color[jObject.Count];

                int index = 0;
                foreach (var prop in jObject.Properties())
                {
                    var value = prop.Value.ToObject<string>();

                    if (value.IndexOf(",", StringComparison.InvariantCulture) != -1)
                    {
                        string[] strs = value.Split(new char[] { ',' });
                        if (strs.Length == 4)
                        {
                            result[index] = global::System.Drawing.Color.FromArgb(
                                                  int.Parse(strs[0].Trim()),
                                                  int.Parse(strs[1].Trim()),
                                                  int.Parse(strs[2].Trim()),
                                                  int.Parse(strs[3].Trim()));
                        }
                        else
                        {
                            result[index] = global::System.Drawing.Color.FromArgb(
                                int.Parse(strs[0].Trim()),
                                int.Parse(strs[1].Trim()),
                                int.Parse(strs[2].Trim()));
                        }
                    }
                    else
                    {
                        result[index] = global::System.Drawing.Color.FromName(value);
                    }

                    index++;
                }

                return result;
            }

            public static Size Size(JObject jObject)
            {
                Size size = new Size();

                foreach (var prop in jObject.Properties())
                {
                    switch(prop.Name)
                    {
                        case "Width":
                            size.Width = prop.Value.ToObject<int>();
                            break;

                        case "Height":
                            size.Height = prop.Value.ToObject<int>();
                            break;
                    }
                }

                return size;
            }

            public static DateTime DateTime(JProperty prop)
            {
                var obj = ((JValue)prop.Value).Value;

                if (obj is DateTime)
                    return prop.Value.ToObject<DateTime>();
                else if (obj is long)
                    return new DateTime(prop.Value.ToObject<long>());

                return global::System.DateTime.Now;
            }

            public static RectangleD RectangleD(JProperty prop)
            {
                if (prop.Value is JValue)
                {
                    string text = (string)((JValue)prop.Value).Value;
                    var values = text.Split(',');
                    if (values.Length != 4)
                        throw new Exception("Parsing Error");

                    return new RectangleD(double.Parse(values[0]), double.Parse(values[1]),
                        double.Parse(values[2]), double.Parse(values[3]));
                }
                else
                {
                    var rect = new RectangleD();
                    rect.LoadFromJson((JObject)prop.Value);
                    return rect;
                }
            }

            public static SizeD SizeD(JProperty prop)
            {
                if (prop.Value is JValue)
                {
                    string text = (string)((JValue)prop.Value).Value;
                    var values = text.Split(',');
                    if (values.Length != 2)
                        throw new Exception("Parsing Error");

                    return new SizeD(double.Parse(values[0]), double.Parse(values[1]));
                }
                else
                {
                    var size = new SizeD();
                    foreach (var pr in ((JObject)prop.Value).Properties())
                    {
                        switch (prop.Name)
                        {
                            case "Width":
                                size.Width = pr.Value.ToObject<int>();
                                break;

                            case "Height":
                                size.Height = pr.Value.ToObject<int>();
                                break;
                        }
                    }
                    return size;
                }
            }

            public static Size Size(JProperty prop)
            {
                if (prop.Value is JValue)
                {
                    var text = (string)((JValue)prop.Value).Value;
                    var values = text.Split(',');
                    if (values.Length != 2)
                        throw new Exception("Parsing Error");

                    return new Size(int.Parse(values[0]), int.Parse(values[1]));
                }
                else
                {
                    var size = new Size();
                    foreach (var pr in ((JObject)prop.Value).Properties())
                    {
                        switch (pr.Name)
                        {
                            case "Width":
                                size.Width = pr.Value.ToObject<int>();
                                break;

                            case "Height":
                                size.Height = pr.Value.ToObject<int>();
                                break;
                        }
                    }
                    return size;
                }
            }

            public static Point Point(JObject jObject)
            {
                var point = new Point();

                foreach (var property in jObject.Properties())
                {
                    switch (property.Name)
                    {
                        case "X":
                            point.X = property.Value.ToObject<int>();
                            break;

                        case "Y":
                            point.Y = property.Value.ToObject<int>();
                            break;
                    }
                }

                return point;
            }

            public static PointF PointF(JObject jObject)
            {
                var point = new PointF();

                foreach (var property in jObject.Properties())
                {
                    switch (property.Name)
                    {
                        case "X":
                            point.X = property.Value.ToObject<float>();
                            break;

                        case "Y":
                            point.Y = property.Value.ToObject<float>();
                            break;
                    }
                }

                return point;
            }
        }
        #endregion

        public static void SplitReportJson(Stream stream, out string reportString, out string[] pagesString)
        {
            using (var streamReader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var startLine = 0;
                var startLinePos = 0;
                var endLine = 0;
                var endLinePos = 0;
                var pagesPositions = new List<int>();
                var pages = new List<string>();

                var depth = 0;
                var findPages = false;
                while (jsonTextReader.Read())
                {
                    if (findPages)
                    {
                        if (jsonTextReader.TokenType == JsonToken.StartObject)
                        {
                            depth++;

                            if (depth == 2)
                            {
                                pagesPositions.Add(jsonTextReader.LineNumber - 1);
                                pagesPositions.Add(jsonTextReader.LinePosition - 1);
                            }
                        }
                        if (jsonTextReader.TokenType == JsonToken.EndObject)
                        {
                            depth--;

                            if (depth == 1)
                            {
                                pagesPositions.Add(jsonTextReader.LineNumber - 1);
                                pagesPositions.Add(jsonTextReader.LinePosition - 1);
                            }
                        }

                        if (depth == 0)
                        {
                            findPages = false;
                            endLine = jsonTextReader.LineNumber - 1;
                            endLinePos = jsonTextReader.LinePosition - 1;
                        }
                    }

                    if (jsonTextReader.TokenType == JsonToken.PropertyName && (String)jsonTextReader.Value == "RenderedPages")
                    {
                        findPages = true;
                        startLine = jsonTextReader.LineNumber - 1;
                        startLinePos = jsonTextReader.LinePosition - 1;
                    }
                }

                streamReader.BaseStream.Position = 0;
                var reportStringBuilder = new StringBuilder();
                var lineIndex = 0;
                var pageIndex = 0;
                var pageStr = new StringBuilder();
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    if (lineIndex < startLine)
                    {
                        reportStringBuilder.AppendLine(line);
                    }
                    if (lineIndex == startLine)
                    {
                        reportStringBuilder.Append(line.Substring(0, startLinePos));
                        if (pagesPositions.Count > 0) reportStringBuilder.Append("{}");
                    }
                    if (lineIndex == endLine)
                    {
                        reportStringBuilder.AppendLine(line.Substring(endLinePos));
                    }
                    if (lineIndex > endLine)
                    {
                        reportStringBuilder.AppendLine(line);
                    }

                    if (lineIndex >= startLine && pageIndex <= pagesPositions.Count - 4)
                    {
                        if (lineIndex == pagesPositions[pageIndex])
                        {
                            pageStr.AppendLine("{");
                            pageStr.AppendLine(line.Substring(pagesPositions[pageIndex + 1]));
                        }
                        if (lineIndex > pagesPositions[pageIndex] && lineIndex < pagesPositions[pageIndex + 2])
                        {
                            pageStr.AppendLine(line);
                        }
                        if (lineIndex == pagesPositions[pageIndex + 2])
                        {
                            pageStr.AppendLine(line.Substring(0, pagesPositions[pageIndex + 3]));

                            pages.Add(pageStr.ToString());
                            pageStr.Clear();
                            pageIndex += 4;
                        }
                    }
                    lineIndex++;
                }

                if (reportStringBuilder[0] == 0xFEFF)   //BOM
                    reportStringBuilder.Remove(0, 1);

                reportString = reportStringBuilder.ToString();
                pagesString = pages.ToArray();
            }
        }
    }
}