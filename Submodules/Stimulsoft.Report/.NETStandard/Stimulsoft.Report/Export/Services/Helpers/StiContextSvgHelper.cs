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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Base.Json.Linq;
using System.Linq;
using System.IO;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Painters.Context.Animation;
using System.Drawing;
using System.Drawing.Imaging;
using Stimulsoft.Report.Dashboard;
using System.Globalization;

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Export.Services.Helpers
{
    public static class StiContextSvgHelper
    {
        #region Methods
        public static void WriteGeoms(XmlTextWriter writer, StiContext context, bool needAnimation, bool correctionText = true)
        {
            var clip = String.Empty;
            double finishTime = 0;
            var needStaticShadow = true;

            foreach (var geom in context.Geoms)
            {
                var ag = geom as StiAnimationGeom;
                if (ag != null && ag.Animation != null && ag.Animation.Duration.TotalMilliseconds + ag.Animation.BeginTime.TotalMilliseconds > finishTime)
                {
                    finishTime = ag.Animation.Duration.TotalMilliseconds + ag.Animation.BeginTime.TotalMilliseconds;
                }
                //if (geom is StiShadowAnimationGeom)
                //needStaticShadow = false;
            }

            float dx = 0;
            float dy = 0;
            var listTransformGeom = new List<StiGeom>();

            foreach (var geom in context.Geoms)
            {
                if (geom is StiPushTranslateTransformGeom)
                {
                    listTransformGeom.Add(geom);

                    writer.WriteStartElement("g");

                    dx += (geom as StiPushTranslateTransformGeom).X;
                    dy += (geom as StiPushTranslateTransformGeom).Y;
                }

                else if (geom is StiPushRotateTransformGeom)
                {
                    listTransformGeom.Add(geom);

                    writer.WriteStartElement("g");

                    writer.WriteAttributeString("transform", string.Format("rotate({0} {1} {2})", DoubleToString((geom as StiPushRotateTransformGeom).Angle), DoubleToString(dx), DoubleToString(dy)));
                }

                else if (geom is StiPopTransformGeom)
                {
                    var lastTransformGeom = listTransformGeom[listTransformGeom.Count - 1];

                    var lastTranslateTransformGeom = lastTransformGeom as StiPushTranslateTransformGeom;

                    if (lastTranslateTransformGeom != null)
                    {
                        dx -= lastTranslateTransformGeom.X;
                        dy -= lastTranslateTransformGeom.Y;
                    }

                    listTransformGeom.Remove(lastTransformGeom);

                    writer.WriteEndElement();
                }

                else if (geom is StiClusteredBarSeriesAnimationGeom barAnimationGeom)
                {
                    var rect = RectToRectangleF(barAnimationGeom.ColumnRect);
                    var columnAnimation = barAnimationGeom.Animation as StiColumnAnimation;
                    var rectFrom = columnAnimation != null ? columnAnimation.RectFrom : RectangleF.Empty;

                    var style = new StringBuilder();
                    if (barAnimationGeom.Background != null)
                    {
                        style.Append(WriteFillBrush(writer, barAnimationGeom.Background, rect, dx, dy));
                    }
                    else
                    {
                        style.Append("fill:none;");
                    }

                    if (CheckPenGeom(barAnimationGeom.BorderPen))
                    {
                        var stroke = string.Format("{0}", WriteBorderStroke(writer, barAnimationGeom.BorderPen.Brush, rect));
                        style.Append(string.Format("{0};stroke-width:{1};", stroke, barAnimationGeom.BorderPen.Thickness));
                    }

                    if (barAnimationGeom.CornerRadius != null && !barAnimationGeom.CornerRadius.IsEmpty)
                    {
                        writer.WriteStartElement("path");
                        WriteClipPath(writer, clip);
                        WriteCicledRectPath(writer, rect, barAnimationGeom.CornerRadius, dx, dy);
                    }
                    else
                    {
                        writer.WriteStartElement("rect");
                        WriteClipPath(writer, clip);

                        if (needAnimation)
                        {
                            WriteRect(writer, rectFrom, dx, dy);

                            var action2 = $"[\"y\", {DoubleToString(rectFrom.Y + dy)}, {DoubleToString(rect.Y + dy)}, \"\"]";
                            action2 += $", [\"height\", {DoubleToString(rectFrom.Height)}, {DoubleToString(rect.Height)}, \"\"]";
                            action2 += $", [\"x\", {DoubleToString(rectFrom.X + dx)}, {DoubleToString(rect.X + dx)}, \"\"]";
                            action2 += $", [\"width\", {DoubleToString(rectFrom.Width)}, {DoubleToString(rect.Width)}, \"\"]";

                            AddAnimation(writer,
                                $"[{action2}]",
                                barAnimationGeom.Animation.BeginTime + barAnimationGeom.Animation.BeginTimeCorrect,
                                barAnimationGeom.Animation.Duration);
                        }
                        else
                        {
                            WriteRect(writer, rect, dx, dy);
                        }
                    }

                    writer.WriteAttributeString("shape-rendering", "geometricPrecision");

                    WriteInteraction(writer, barAnimationGeom.Interaction);
                    writer.WriteAttributeString("style", style.ToString());
                    WriteTooltip(writer, barAnimationGeom.ToolTip);
                    writer.WriteEndElement();
                }
                
                else if (geom is StiLabelAnimationGeom)
                {
                    var textGeom = geom as StiLabelAnimationGeom;
                    var labelAnimation = textGeom.Animation as StiLabelAnimation;
                    var animationPieLabel = textGeom.Animation as StiPieLabelAnimation;
                    var rect = RectToRectangleF(textGeom.Rectangle);
                    if (labelAnimation != null) rect = labelAnimation.LabelRect;
                    else if (animationPieLabel != null) rect = animationPieLabel.RectLabelFrom;

                    var sf = textGeom.StringFormat.IsGeneric ? StringFormat.GenericDefault.Clone() as StringFormat : new StringFormat();
                    sf.Alignment = textGeom.StringFormat.Alignment;
                    sf.FormatFlags = textGeom.StringFormat.FormatFlags;
                    sf.HotkeyPrefix = textGeom.StringFormat.HotkeyPrefix;
                    sf.LineAlignment = textGeom.StringFormat.LineAlignment;
                    sf.Trimming = textGeom.StringFormat.Trimming;

                    var pointF = new PointF();

                    if (textGeom.Angle == 0f)
                    {
                        pointF = new PointF(rect.X, rect.Y);

                        switch (sf.LineAlignment)
                        {
                            case StringAlignment.Near:
                                pointF = new PointF(rect.X, rect.Y);
                                break;

                            case StringAlignment.Center:
                                pointF = new PointF(rect.X, rect.Y + rect.Height / 2);
                                break;

                            case StringAlignment.Far:
                                pointF = new PointF(rect.X, rect.Y + rect.Height);
                                break;
                        }

                        textGeom.RotationMode = StiRotationMode.LeftCenter;
                    }
                    else
                    {
                        pointF = new PointF((float)(rect.X + rect.Width / 2), (float)(rect.Y + rect.Height / 2));
                    }

                    var style = new StringBuilder();

                    if (textGeom.LabelBrush != null)
                    {
                        style.Append(WriteFillBrush(writer, textGeom.LabelBrush, rect, dx, dy));
                    }
                    else
                    {
                        style.Append("fill:none;");
                    }

                    if (textGeom.DrawBorder && CheckPenGeom(textGeom.PenBorder))
                    {
                        var stroke = string.Format("{0}", WriteBorderStroke(writer, textGeom.PenBorder.Brush, rect));
                        style.Append(string.Format("{0};stroke-width:{1};", stroke, textGeom.PenBorder.Thickness));
                    }

                    writer.WriteStartElement("rect");

                    var centerX = 0f;
                    var centerY = 0f;

                    switch (textGeom.RotationMode)
                    {
                        case StiRotationMode.LeftTop:
                            break;

                        case StiRotationMode.LeftCenter:
                            centerY = rect.Height / 2;
                            break;

                        case StiRotationMode.LeftBottom:
                            centerY = rect.Height;
                            break;

                        case StiRotationMode.CenterTop:
                            centerX = rect.Width / 2;
                            break;

                        case StiRotationMode.CenterCenter:
                            centerX = rect.Width / 2;
                            centerY = rect.Height / 2;
                            break;

                        case StiRotationMode.CenterBottom:
                            centerX = rect.Width / 2;
                            centerY = rect.Height;
                            break;

                        case StiRotationMode.RightTop:
                            centerX = rect.Width;
                            break;

                        case StiRotationMode.RightCenter:
                            centerX = rect.Width;
                            centerY = rect.Height / 2;
                            break;

                        case StiRotationMode.RightBottom:
                            centerX = rect.Width;
                            centerY = rect.Height;
                            break;
                    }

                    writer.WriteAttributeString("transform", $"translate({DoubleToString(pointF.X + dx - centerX)}, {DoubleToString(pointF.Y + dy - centerY)}) rotate({DoubleToString(textGeom.Angle)} {DoubleToString(centerX)},{DoubleToString(centerY)})");
                    writer.WriteAttributeString("width", DoubleToString(rect.Width));
                    writer.WriteAttributeString("height", DoubleToString(rect.Height));
                    writer.WriteAttributeString("style", style.ToString());
                    writer.WriteAttributeString("shape-rendering", "geometricPrecision");
                    writer.WriteAttributeString("opacity", labelAnimation != null || animationPieLabel != null ? "1" : "0");
                    writer.WriteAttributeString("fill", "rgba(0,0,0,0)");

                    if (labelAnimation != null)
                    {
                        var deltaX = textGeom.Rectangle.X - labelAnimation.LabelRect.X;
                        var deltaY = textGeom.Rectangle.Y - labelAnimation.LabelRect.Y;

                        AddAnimation(writer, $"[[\"translate\",\"{DoubleToString(pointF.X + dx)}:{DoubleToString(pointF.Y + dy)}\",\"{DoubleToString(pointF.X + dx + deltaX)}:{DoubleToString(pointF.Y + dy + deltaY)}\",\" rotate({DoubleToString(textGeom.Angle)} {DoubleToString(rect.Width / 2)},{DoubleToString(rect.Height / 2)})\"]]",
                                labelAnimation.BeginTime, labelAnimation.Duration);
                    }

                    else if (animationPieLabel != null)
                    {
                        var deltaX = textGeom.Rectangle.X - animationPieLabel.RectLabel.X;
                        var deltaY = textGeom.Rectangle.Y - animationPieLabel.RectLabel.Y;

                        AddAnimation(writer, $"[[\"translate\",\"{DoubleToString(pointF.X + dx)}:{DoubleToString(pointF.Y + dy)}\",\"{DoubleToString(pointF.X + dx + deltaX)}:{DoubleToString(pointF.Y + dy + deltaY)}\",\" rotate({DoubleToString(textGeom.Angle)} {DoubleToString(rect.Width / 2)},{DoubleToString(rect.Height / 2)})\"]]",
                            animationPieLabel.BeginTime, animationPieLabel.Duration);
                    }

                    else
                    {
                        AddAnimation(writer, "[[\"opacity\", 0, 1, \"\"]]", TimeSpan.FromMilliseconds(finishTime), TimeSpan.FromMilliseconds(500));
                    }

                    writer.WriteEndElement();

                    var size = correctionText ? textGeom.Font.FontSize * 4 / 3 : textGeom.Font.FontSize;

                    writer.WriteStartElement("text");

                    switch (textGeom.RotationMode)
                    {
                        case StiRotationMode.LeftCenter:
                            writer.WriteAttributeString("dy", "0.5em");
                            break;

                        case StiRotationMode.LeftBottom:
                            break;

                        case StiRotationMode.CenterTop:
                            style.Append("text-anchor:middle;");
                            writer.WriteAttributeString("dy", "1em");
                            break;

                        case StiRotationMode.CenterCenter:
                            style.Append("text-anchor:middle;");
                            if (correctionText)
                                writer.WriteAttributeString("dy", "0.5em");
                            else
                                writer.WriteAttributeString("dy", "0.3em");
                            break;

                        case StiRotationMode.CenterBottom:
                            style.Append("text-anchor:middle;");
                            break;

                        case StiRotationMode.RightTop:
                            style.Append("text-anchor:end;");
                            writer.WriteAttributeString("dy", "1em");
                            break;

                        case StiRotationMode.RightCenter:
                            style.Append("text-anchor:end;");
                            writer.WriteAttributeString("dy", "0.5em");
                            break;

                        case StiRotationMode.RightBottom:
                            style.Append("text-anchor:end;");
                            break;

                        default:
                            writer.WriteAttributeString("dy", "1em");
                            break;
                    }

                    if (textGeom.Angle == 0 && (textGeom.Rectangle.Width == 0/* || textGeom.MaximalWidth == null*/))
                    {
                        WriteClipPath(writer, clip);
                        
                        writer.WriteAttributeString("x", DoubleToString(pointF.X + dx));
                        writer.WriteAttributeString("y", DoubleToString(pointF.Y + dy));
                    }
                    else
                    {
                        writer.WriteAttributeString("transform", string.Format("translate({0}, {1}) rotate({2} 0,0)",
                            DoubleToString(pointF.X + dx), DoubleToString(pointF.Y + dy), DoubleToString(textGeom.Angle)));
                    }


                    writer.WriteAttributeString("font-size", DoubleToString(size));
                    writer.WriteAttributeString("font-family", textGeom.Font.FontName);
                    if ((textGeom.Font.FontStyle & FontStyle.Bold) > 0)
                        writer.WriteAttributeString("font-weight", "bold");
                    if ((textGeom.Font.FontStyle & FontStyle.Italic) > 0)
                        writer.WriteAttributeString("font-style", "italic");

                    var valueDecoration = "";
                    if ((textGeom.Font.FontStyle & FontStyle.Underline) > 0)
                        valueDecoration += "underline";
                    if ((textGeom.Font.FontStyle & FontStyle.Strikeout) > 0)
                        valueDecoration += " line-through";
                    if (!String.IsNullOrEmpty(valueDecoration))
                        writer.WriteAttributeString("text-decoration", valueDecoration);

                    var textColor = textGeom.TextBrush is Color ? (Color)textGeom.TextBrush : StiBrush.ToColor(textGeom.TextBrush as StiBrush);
                    style.Append(string.Format("fill:#{0:X2}{1:X2}{2:X2};", textColor.R, textColor.G, textColor.B));
                    if (textColor.A != 0xFF)
                    {
                        style.Append(string.Format("fill-opacity:{0};", DoubleToString(Math.Round(textColor.A / 255f, 3))));
                    }

                    style.Append("pointer-events: none");

                    writer.WriteAttributeString("style", style.ToString());

                    /*if (textGeom.Rectangle.Width != 0 && (textGeom.StringFormat.FormatFlags & StringFormatFlags.NoWrap) == 0)
                    {
                        var length = textGeom.Text.Length;
                        var wrapCharCount = length;
                        var rectTemp = new SizeF(0, 0);

                        for (var index = 0; index < length; index++)
                        {

                            rectTemp = context.MeasureString(textGeom.Text.Substring(0, index), textGeom.Font);
                            if (rectTemp.Width > textGeom.Rectangle.Width && index != 0)
                            {
                                wrapCharCount = index - 1;
                                break;
                            }
                        }

                        var countRow = Math.Ceiling((double)length / wrapCharCount);
                        var startPointY = 0d;

                        switch (textGeom.RotationMode)
                        {
                            case StiRotationMode.LeftCenter:
                            case StiRotationMode.CenterCenter:
                            case StiRotationMode.RightCenter:
                                startPointY = -countRow * rectTemp.Height / 2 + rectTemp.Height / 2;//offset is taken into account 0.5em parent <text>
                                break;

                            default:
                                startPointY = 0;
                                break;
                        }

                        if (wrapCharCount > 0)
                        {
                            var startIndex = 0;
                            var index = 0;
                            while (startIndex < length)
                            {

                                writer.WriteStartElement("tspan");
                                writer.WriteAttributeString("x", "0");

                                if (index == 0)
                                    writer.WriteAttributeString("y", DoubleToString(startPointY));
                                else
                                    writer.WriteAttributeString("dy", DoubleToString(rectTemp.Height));

                                writer.WriteString(startIndex + wrapCharCount < textGeom.Text.Length
                                    ? textGeom.Text.Substring(startIndex, wrapCharCount)
                                    : textGeom.Text.Substring(startIndex));

                                writer.WriteEndElement();

                                startIndex += wrapCharCount;
                                index++;
                            }
                        }
                    }
                    else*/
                    {
                        writer.WriteAttributeString("opacity", labelAnimation != null || animationPieLabel != null ? "1" : "0");
                        if (labelAnimation != null)
                        {
                            AddAnimation(writer, String.Format("[[\"translate\",\"" + DoubleToString(labelAnimation.LabelRect.X + dx) + ":" + DoubleToString(labelAnimation.LabelRect.Y + dy) + "\",\"" +
                                                               DoubleToString(rect.X + dx) + ":" + DoubleToString(rect.Y + dy) + "\",\" rotate({0} 0, 0)\"]]", DoubleToString(textGeom.Angle)),
                                labelAnimation.BeginTime, labelAnimation.Duration);
                            if (labelAnimation.ValueFrom != null && labelAnimation.Value != null)
                            {
                                decimal val = (decimal)labelAnimation.ValueFrom.Value - (decimal)labelAnimation.Value.Value;
                                int decimals = BitConverter.GetBytes(decimal.GetBits(val)[3])[2];
                                AddAnimation(writer, "[[\"value\", " + DoubleToString(labelAnimation.ValueFrom.Value) + ", " + DoubleToString(labelAnimation.Value.Value) + ", \"" + decimals + "\"]]",
                                    labelAnimation.BeginTime, labelAnimation.Duration, "a1");
                            }
                        }
                        else if (animationPieLabel != null)
                        {
                            AddAnimation(writer, String.Format("[[\"translate\",\"" + DoubleToString(animationPieLabel.RectLabelFrom.X + dx) + ":" + DoubleToString(animationPieLabel.RectLabelFrom.Y + dy) + "\",\"" +
                                                               DoubleToString(rect.X + dx) + ":" + DoubleToString(rect.Y + dy) + "\",\" rotate({0} 0, 0)\"]]", DoubleToString(textGeom.Angle)),
                                animationPieLabel.BeginTime, animationPieLabel.Duration);
                            if (animationPieLabel.ValueFrom != null && animationPieLabel.Value != null)
                            {
                                decimal val = (decimal)animationPieLabel.ValueFrom.Value - (decimal)animationPieLabel.Value.Value;
                                int decimals = BitConverter.GetBytes(decimal.GetBits(val)[3])[2];
                                AddAnimation(writer, "[[\"value\", " + DoubleToString(animationPieLabel.ValueFrom.Value) + ", " + DoubleToString(animationPieLabel.Value.Value) + ", \"" + decimals + "\", \"" + textGeom.Text + "\" ]]",
                                    animationPieLabel.BeginTime, animationPieLabel.Duration, "a1");
                            }
                        }
                        else
                        {
                            AddAnimation(writer, "[[\"opacity\", 0, 1, \"\"]]", TimeSpan.FromMilliseconds(finishTime), TimeSpan.FromMilliseconds(500));
                        }

                        if (labelAnimation != null && labelAnimation.ValueFrom != null)
                        {
                            writer.WriteValue(DoubleToString(labelAnimation.ValueFrom.Value));
                        }
                        else if (animationPieLabel != null && animationPieLabel.ValueFrom != null)
                        {
                            writer.WriteValue(DoubleToString(animationPieLabel.ValueFrom.Value));
                        }
                        else
                        {
                            writer.WriteValue(textGeom.Text);
                        }
                    }
                    writer.WriteEndElement();
                }
                
                else if (geom is StiPushClipGeom)
                {
                    var clipGeom = geom as StiPushClipGeom;
                    var rect = RectToRectangleF(clipGeom.ClipRectangle);

                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        var guid = "s" + StiGuidUtils.NewGuid();
                        writer.WriteStartElement("defs");
                        writer.WriteStartElement("clipPath");
                        writer.WriteAttributeString("id", guid);
                        writer.WriteStartElement("rect");
                        writer.WriteAttributeString("x", DoubleToString(rect.X + dx));
                        writer.WriteAttributeString("y", DoubleToString(rect.Y + dy));
                        writer.WriteAttributeString("width", DoubleToString(rect.Width));
                        writer.WriteAttributeString("height", DoubleToString(rect.Height));
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        clip = guid;
                    }
                }

                else if (geom is StiPushClipPathGeom)
                {
                    var clipGeom = geom as StiPushClipPathGeom;

                    StringBuilder animatedPath;
                    TimeSpan? duration;
                    var pathData = GetPathData(clipGeom.Geoms, dx, dy, out animatedPath, out duration);

                    var guid = "s" + StiGuidUtils.NewGuid();
                    writer.WriteStartElement("defs");
                    writer.WriteStartElement("clipPath");
                    writer.WriteAttributeString("id", guid);
                    writer.WriteStartElement("path");
                    writer.WriteAttributeString("d", pathData);                    
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    clip = guid;
                }

                else if (geom is StiPopClipGeom)
                {
                    clip = String.Empty;
                }
                
                else if (geom is StiShadowAnimationGeom)
                {
                    var shadow = geom as StiShadowAnimationGeom;
                    var rect = RectToRectangleF(shadow.Rect);
                    var guid = "s" + StiGuidUtils.NewGuid();
                    writer.WriteStartElement("defs");
                    writer.WriteStartElement("filter");
                    writer.WriteAttributeString("id", guid);
                    writer.WriteAttributeString("x", "0");
                    writer.WriteAttributeString("y", "0");
                    writer.WriteAttributeString("width", "200%");
                    writer.WriteAttributeString("height", "200%");

                    writer.WriteStartElement("feOffset");
                    writer.WriteAttributeString("result", "offOut");
                    writer.WriteAttributeString("in", "SourceGraphic");
                    writer.WriteAttributeString("dx", "1.111111111111111");
                    writer.WriteAttributeString("dy", "1.111111111111111");
                    writer.WriteEndElement();
                    writer.WriteStartElement("feColorMatrix");
                    writer.WriteAttributeString("result", "matrixOut");
                    writer.WriteAttributeString("in", "offOut");
                    writer.WriteAttributeString("type", "matrix");
                    writer.WriteAttributeString("values", "0.58 0 0 0 0 0 0.58 0 0 0 0 0 0.58 0 0 0 0 0 1 0");
                    writer.WriteEndElement();
                    writer.WriteStartElement("feGaussianBlur");
                    writer.WriteAttributeString("result", "blurOut");
                    writer.WriteAttributeString("in", "matrixOut");
                    writer.WriteAttributeString("stdDeviation", "1.111111111111111");
                    writer.WriteEndElement();
                    writer.WriteStartElement("feBlend");
                    writer.WriteAttributeString("mode", "normal");
                    writer.WriteAttributeString("in", "SourceGraphic");
                    writer.WriteAttributeString("in2", "blurOut");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    if (shadow.CornerRadius != null && !shadow.CornerRadius.IsEmpty)
                    {
                        writer.WriteStartElement("path");
                        WriteClipPath(writer, clip);
                        WriteCicledRectPath(writer, rect, shadow.CornerRadius, dx, dy);
                    }

                    else
                    {
                        writer.WriteStartElement("rect");
                        WriteClipPath(writer, clip);
                        WriteRect(writer, rect, dx, dy);

                        writer.WriteAttributeString("rx", DoubleToString(shadow.RadiusX));
                        writer.WriteAttributeString("ry", DoubleToString(shadow.RadiusY));
                    }

                    writer.WriteAttributeString("fill", "rgb(150,150,150)");
                    writer.WriteAttributeString("filter", String.Format("url(#{0})", guid));                    

                    if (needAnimation)
                    {
                        writer.WriteAttributeString("opacity", "0");
                        AddAnimation(writer, "[[\"opacity\", 0, 1, \"\"]]", shadow.Animation.BeginTime, shadow.Animation.Duration);
                    }

                    writer.WriteEndElement();
                }
                
                else if (geom is StiBorderAnimationGeom)
                {
                    var border = geom as StiBorderAnimationGeom;
                    var rect = RectToRectangleF(border.Rect);

                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        var animation = border.Animation as StiOpacityAnimation;

                        var style = new StringBuilder();
                        if (border.Background != null)
                        {
                            style.Append(WriteFillBrush(writer, border.Background, rect, dx, dy));
                        }
                        else
                        {
                            style.Append("fill:none;");
                        }

                        if (CheckPenGeom(border.BorderPen))
                        {
                            var stroke = string.Format("{0}", WriteBorderStroke(writer, border.BorderPen.Brush, rect));
                            style.Append(string.Format("{0};stroke-width:{1};", stroke, border.BorderPen.Thickness));
                        }

                        if (string.IsNullOrEmpty(border.ToolTip) && border.Interaction == null)
                            style.Append("pointer-events: none");

                        if (border.CornerRadius != null && !border.CornerRadius.IsEmpty)
                        {
                            writer.WriteStartElement("path");
                            WriteClipPath(writer, clip);
                            WriteCicledRectPath(writer, rect, border.CornerRadius, dx, dy);
                        }
                        else
                        {
                            writer.WriteStartElement("rect");
                            WriteClipPath(writer, clip);
                            WriteRect(writer, rect, dx, dy);
                        }

                        writer.WriteAttributeString("style", style.ToString());
                        writer.WriteAttributeString("shape-rendering", "geometricPrecision");

                        if (animation != null)
                        {
                            writer.WriteAttributeString("opacity", "0");
                            AddAnimation(writer, "[[\"opacity\", 0 , 1,\"\"]]", animation.BeginTime, animation.Duration);
                        }

                        WriteInteraction(writer, border.Interaction);
                        WriteTooltip(writer, border.ToolTip);
                        writer.WriteEndElement();
                    }
                }
                
                else if (geom is StiClusteredColumnSeriesAnimationGeom columnAnimationGeom)
                {
                    var rect = RectToRectangleF(columnAnimationGeom.ColumnRect);
                    var columnAnimation = columnAnimationGeom.Animation as StiColumnAnimation;
                    var rectFrom = columnAnimation != null ? columnAnimation.RectFrom : RectangleF.Empty;
                    var style = new StringBuilder();
                    if (columnAnimationGeom.Background != null)
                    {
                        style.Append(WriteFillBrush(writer, columnAnimationGeom.Background, rect, dx, dy));
                    }
                    else
                    {
                        style.Append("fill:none;");
                    }

                    if (CheckPenGeom(columnAnimationGeom.BorderPen))
                    {
                        var stroke = string.Format("{0}", WriteBorderStroke(writer, columnAnimationGeom.BorderPen.Brush, rect));
                        style.Append(string.Format("{0};stroke-width:{1};", stroke, columnAnimationGeom.BorderPen.Thickness));
                    }

                    if (columnAnimationGeom.CornerRadius != null && !columnAnimationGeom.CornerRadius.IsEmpty)
                    {
                        writer.WriteStartElement("path");                        
                        WriteClipPath(writer, clip);
                        WriteCicledRectPath(writer, rect, columnAnimationGeom.CornerRadius, dx, dy);
                    }
                    else
                    {
                        writer.WriteStartElement("rect");
                        WriteClipPath(writer, clip);

                        if (needAnimation)
                        {
                            WriteRect(writer, rectFrom, dx, dy);

                            var action2 = $"[\"y\", {DoubleToString(rectFrom.Y + dy)}, {DoubleToString(rect.Y + dy)}, \"\"]";
                            action2 += $", [\"height\", {DoubleToString(rectFrom.Height)}, {DoubleToString(rect.Height)}, \"\"]";
                            action2 += $", [\"x\", {DoubleToString(rectFrom.X + dx)}, {DoubleToString(rect.X + dx)}, \"\"]";
                            action2 += $", [\"width\", {DoubleToString(rectFrom.Width)}, {DoubleToString(rect.Width)}, \"\"]";

                            AddAnimation(writer,
                                $"[{action2}]",
                                columnAnimationGeom.Animation.BeginTime + columnAnimationGeom.Animation.BeginTimeCorrect,
                                columnAnimationGeom.Animation.Duration);
                        }
                        else
                        {
                            WriteRect(writer, rect, dx, dy);
                        }
                    }

                    writer.WriteAttributeString("shape-rendering", "geometricPrecision");
                    WriteInteraction(writer, columnAnimationGeom.Interaction);

                    writer.WriteAttributeString("style", style.ToString());
                    WriteTooltip(writer, columnAnimationGeom.ToolTip);
                    writer.WriteEndElement();
                }
                
                else if (geom is StiLinesAnimationGeom)
                {
                    var lines = geom as StiLinesAnimationGeom;
                    if (CheckPenGeom(lines.Pen))
                    {
                        var guid = "g" + StiGuidUtils.NewGuid();
                        if (lines.Animation.Type == StiAnimationType.Translation)
                        {
                            var color = (Color)lines.Pen.Brush;
                            writer.WriteStartElement("g");
                            writer.WriteStartElement("defs");
                            writer.WriteStartElement("linearGradient");
                            writer.WriteAttributeString("id", guid);
                            writer.WriteAttributeString("x1", "0%");
                            writer.WriteAttributeString("y1", "0%");
                            writer.WriteAttributeString("x2", "100%");
                            writer.WriteAttributeString("y2", "0%");
                            writer.WriteStartElement("stop");
                            writer.WriteAttributeString("offset", "0%");
                            writer.WriteAttributeString("stop-color", string.Format("rgba({0},{1},{2},{3})", color.R, color.G, color.B,
                                DoubleToString(Math.Round(color.A / 255f, 3))));
                            writer.WriteAttributeString("stop-opacity", "1");
                            writer.WriteAttributeString("style", "x: 0px;");
                            AddAnimation(writer, "[[\"offset\", 0 , 100,\"%\"]]", lines.Animation.BeginTime, lines.Animation.Duration);
                            writer.WriteEndElement();
                            writer.WriteStartElement("stop");
                            writer.WriteAttributeString("offset", "0%");
                            writer.WriteAttributeString("stop-color", "transparent");
                            writer.WriteAttributeString("stop-opacity", "0");
                            writer.WriteAttributeString("style", "x: 00px;");
                            writer.WriteEndElement();

                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }

                        var animationPoints = lines.Animation as StiPointsAnimation;

                        writer.WriteStartElement("polyline");
                        WriteClipPath(writer, clip);
                        var sb = new StringBuilder();
                        var pointsTo = new StringBuilder();

                        for (var indexPoint = 0; lines.Points.Length > indexPoint; indexPoint++)
                        {
                            var point = lines.Points[indexPoint];
                            float deltaFix = 0;
                            if (indexPoint == lines.Points.Length - 1)
                                deltaFix = 0.0001f * indexPoint;

                            //fix: animation if line parallels XAxis
                            if (animationPoints == null)
                            {
                                sb.AppendFormat("{0},{1} ", DoubleToString(point.X + dx), DoubleToString(point.Y + dy + deltaFix));
                            }
                            else
                            {
                                sb.AppendFormat("{0},{1} ", DoubleToString(animationPoints.PointsFrom[indexPoint].X + dx), DoubleToString(animationPoints.PointsFrom[indexPoint].Y + dy + deltaFix));

                                pointsTo.AppendFormat("{0},{1} ", DoubleToString(point.X + dx), DoubleToString(point.Y + dy + deltaFix));
                            }
                        }

                        writer.WriteAttributeString("fill", "none");
                        writer.WriteAttributeString("points", sb.ToString());
                        if (lines.Animation.Type == StiAnimationType.Opacity || animationPoints != null)
                        {
                            var stroke = string.Format("{0}", WriteBorderStroke(writer, lines.Pen.Brush, new RectangleF()));
                            var style = new StringBuilder();
                            style.AppendFormat("{0};stroke-width:{1};", stroke, DoubleToString(lines.Pen.Thickness));
                            writer.WriteAttributeString("style", style.ToString());
                            if (animationPoints == null)
                            {
                                writer.WriteAttributeString("opacity", "0");
                                AddAnimation(writer, "[[\"opacity\", 0 , 1,\"\"]]", lines.Animation.BeginTime, lines.Animation.Duration);
                            }
                            else
                            {
                                AddAnimation(writer, "[[\"points\", \"" + sb.ToString() + "\", \"" + pointsTo.ToString() + "\",\"\"]]", lines.Animation.BeginTime, lines.Animation.Duration);
                            }
                        }
                        else if (lines.Animation.Type == StiAnimationType.Translation)
                        {
                            writer.WriteAttributeString("stroke-width", DoubleToString(lines.Pen.Thickness));
                            writer.WriteAttributeString("stroke", String.Format("url(#{0})", guid));
                        }
                        if (lines.Pen.PenStyle != StiPenStyle.Solid)
                        {
                            writer.WriteAttributeString("stroke-dasharray", StiSvgHelper.GetLineStyleDash(lines.Pen.PenStyle, lines.Pen.Thickness));
                        }

                        writer.WriteEndElement();
                    }
                }
                
                else if (geom is StiEllipseAnimationGeom)
                {
                    #region Draw Ellipse
                    var ellipse = geom as StiEllipseAnimationGeom;

                    var rect = RectToRectangleF(ellipse.Rect);

                    var style = string.Empty;
                    var fill = string.Empty;

                    if (ellipse.Background != null)
                    {
                        style = WriteFillBrush(writer, ellipse.Background, rect, dx, dy);
                    }
                    else
                    {
                        style = "fill:none;";
                    }

                    if (CheckPenGeom(ellipse.BorderPen))
                    {
                        var stroke = string.Format("{0}", WriteBorderStroke(writer, ellipse.BorderPen.Brush, rect));
                        style += string.Format("{0};stroke-width:{1};", stroke, DoubleToString(ellipse.BorderPen.Thickness));
                    }

                    writer.WriteStartElement("ellipse");

                    writer.WriteAttributeString("rx", DoubleToString(rect.Width / 2f));
                    writer.WriteAttributeString("ry", DoubleToString(rect.Height / 2f));
                    if (ellipse.Animation.Type != StiAnimationType.Scale)
                    {
                        writer.WriteAttributeString("cx", DoubleToString(rect.X + dx + rect.Width / 2f));
                        writer.WriteAttributeString("cy", DoubleToString(rect.Y + dy + rect.Height / 2f));
                    }

                    writer.WriteAttributeString("style", style);

                    if (ellipse.Animation.Type == StiAnimationType.Opacity)
                    {
                        writer.WriteAttributeString("opacity", "0");
                        AddAnimation(writer, "[[\"opacity\", 0 , 1,\"\"]]", ellipse.Animation.BeginTime, ellipse.Animation.Duration);
                    }
                    else if (ellipse.Animation.Type == StiAnimationType.Scale)
                    {
                        writer.WriteAttributeString("transform", "scale(0.1)");
                        AddAnimation(writer, String.Format("[[\"transform\", 0 , 1,\")\",\"translate({0},{1}) scale(\"]]",
                            DoubleToString(rect.X + dx + rect.Width / 2f),
                            DoubleToString(rect.Y + dy + rect.Height / 2f)),
                            ellipse.Animation.BeginTime,
                            ellipse.Animation.Duration);
                    }

                    WriteInteraction(writer, ellipse.Interaction);
                    WriteTooltip(writer, ellipse.ToolTip);
                    writer.WriteEndElement();
                    #endregion
                }
                
                else if (geom is StiPathElementAnimationGeom pathElementGeom)
                {
                    #region Draw path
                    var path = geom as StiPathElementAnimationGeom;

                    var rect = RectToRectangleF(path.Rect);

                    var style = string.Empty;
                    var fill = string.Empty;

                    if (path.Background != null)
                    {
                        style = WriteFillBrush(writer, path.Background, rect, dx, dy);
                    }
                    else
                    {
                        style = "fill:none;";
                    }

                    if (CheckPenGeom(path.BorderPen))
                    {
                        var stroke = string.Format("{0}", WriteBorderStroke(writer, path.BorderPen.Brush, rect));
                        style += string.Format("{0};stroke-width:{1};", stroke, DoubleToString(path.BorderPen.Thickness));
                    }

                    StringBuilder animatedPath;
                    TimeSpan? duration;
                    var pathData = GetPathData(path.PathGeoms, dx, dy, out animatedPath, out duration, pathElementGeom.Animation);

                    writer.WriteStartElement("path");
                    WriteClipPath(writer, clip);
                    writer.WriteAttributeString("d", pathData);
                    writer.WriteAttributeString("style", style);

                    if (animatedPath.Length == 0)
                    {
                        if (path.Animation != null)
                        {
                            writer.WriteAttributeString("opacity", "0");
                            AddAnimation(writer, "[[\"opacity\", 0 , 1,\"\"]]", path.Animation.BeginTime, path.Animation.Duration);
                        }
                    }

                    else
                    {
                        if (path.PathGeoms.Count == 1 && path.PathGeoms[0] is StiPieSegmentGeom)
                        {
                            AddAnimation(writer, "[[\"pie\", \"" + animatedPath + "\", 1,\"\"]]", TimeSpan.FromMilliseconds(0), duration.Value);
                        }
                        else if (path.PathGeoms.Count == 4 && path.Animation is StiPieSegmentAnimation)
                        {
                            AddAnimation(writer, "[[\"doughnut\", \"" + animatedPath + "\", 1,\"\"]]", TimeSpan.FromMilliseconds(0), duration.Value);
                        }
                        else
                        {
                            AddAnimation(writer, "[[\"path\", \"" + animatedPath + "\", 1,\"\"]]", TimeSpan.FromMilliseconds(0), duration.Value);
                        }
                    }

                    WriteInteraction(writer, path.Interaction);
                    WriteTooltip(writer, path.ToolTip);
                    writer.WriteEndElement();
                    #endregion
                }
                
                else if (geom is StiPathAnimationGeom)
                {
                    #region Draw path
                    var path = geom as StiPathAnimationGeom;

                    var rect = RectToRectangleF(path.Rect);

                    var style = string.Empty;
                    var fill = string.Empty;

                    if (path.Background != null)
                    {
                        style = WriteFillBrush(writer, path.Background, rect, dx, dy);
                    }
                    else
                    {
                        style = "fill:none;";
                    }

                    if (CheckPenGeom(path.Pen))
                    {
                        var stroke = string.Format("{0}", WriteBorderStroke(writer, path.Pen.Brush, rect));
                        style += string.Format("{0};stroke-width:{1};", stroke, DoubleToString(path.Pen.Thickness));
                    }

                    StringBuilder animatedPath;
                    TimeSpan? duration;
                    var pathData = GetPathData(path.Geoms, dx, dy, out animatedPath, out duration);

                    writer.WriteStartElement("path");
                    WriteClipPath(writer, clip);
                    writer.WriteAttributeString("d", pathData);
                    writer.WriteAttributeString("style", style);

                    if (path.Animation != null)
                    {
                        writer.WriteAttributeString("opacity", "0");
                        AddAnimation(writer, "[[\"opacity\", 0 , 1,\"\"]]", path.Animation.BeginTime, path.Animation.Duration);
                    }
                    else if (animatedPath.Length != 0)
                    {
                        AddAnimation(writer, "[[\"path\", \"" + animatedPath + "\", 1,\"\"]]", TimeSpan.FromMilliseconds(0), duration.Value);
                    }

                    WriteInteraction(writer, path.Interaction);

                    writer.WriteEndElement();
                    #endregion
                }
                
                else if (geom is StiCurveAnimationGeom)
                {
                    var curve = geom as StiCurveAnimationGeom;
                    var animationPoints = curve.Animation as StiPointsAnimation;
                    if (CheckPenGeom(curve.Pen))
                    {
                        var guid = "g" + StiGuidUtils.NewGuid();
                        var color = (Color)curve.Pen.Brush;
                        writer.WriteStartElement("g");
                        writer.WriteStartElement("defs");
                        writer.WriteStartElement("linearGradient");
                        writer.WriteAttributeString("id", guid);
                        writer.WriteAttributeString("x1", "0%");
                        writer.WriteAttributeString("y1", "0%");
                        writer.WriteAttributeString("x2", "100%");
                        writer.WriteAttributeString("y2", "0%");
                        writer.WriteStartElement("stop");
                        writer.WriteAttributeString("offset", "0%");
                        writer.WriteAttributeString("stop-color", string.Format("rgba({0},{1},{2},{3})", color.R, color.G, color.B,
                           DoubleToString(Math.Round(color.A / 255f, 3))));
                        writer.WriteAttributeString("stop-opacity", "1");
                        writer.WriteAttributeString("style", "x: 0px;");
                        if (animationPoints == null)
                        {
                            AddAnimation(writer, "[[\"offset\", 0 , 100,\"%\"]]", curve.Animation.BeginTime, curve.Animation.Duration);
                        }

                        writer.WriteEndElement();
                        if (animationPoints == null)
                        {
                            writer.WriteStartElement("stop");
                            writer.WriteAttributeString("offset", "0%");
                            writer.WriteAttributeString("stop-color", "transparent");
                            writer.WriteAttributeString("stop-opacity", "0");
                            writer.WriteAttributeString("style", "x: 0px;");
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();

                        writer.WriteStartElement("path");
                        WriteClipPath(writer, clip);
                        var pts = ConvertSplineToCubicBezier(curve.Points, curve.Tension);
                        if (animationPoints != null)
                        {
                            StringBuilder animatedPath = new StringBuilder();
                            var fromPts = ConvertSplineToCubicBezier(animationPoints.PointsFrom, curve.Tension);
                            animatedPath.AppendFormat("M{0}:{1},{2}:{3} C", DoubleToString(fromPts[0].X + dx), DoubleToString(pts[0].X + dx), DoubleToString(fromPts[0].Y + dy), DoubleToString(pts[0].Y + dy));
                            for (int index = 1; index < pts.Length; index++)
                            {
                                animatedPath.AppendFormat("{0}:{1},{2}:{3} ", DoubleToString(fromPts[index].X + dx), DoubleToString(pts[index].X + dx), DoubleToString(fromPts[index].Y + dy), DoubleToString(pts[index].Y + dy + index * 0.0001));
                            }

                            AddAnimation(writer, "[[\"path\", \"" + animatedPath + "\", 1,\"\"]]", TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(1));

                            StringBuilder stSpline = new StringBuilder();
                            stSpline.AppendFormat("M{0},{1} C", DoubleToString(fromPts[0].X + dx), DoubleToString(fromPts[0].Y + dy));
                            for (int index = 1; index < fromPts.Length; index++)
                            {
                                stSpline.AppendFormat("{0},{1} ", DoubleToString(fromPts[index].X + dx), DoubleToString(fromPts[index].Y + dy + index * 0.0001));
                            }

                            writer.WriteAttributeString("d", stSpline.ToString());
                        }
                        else
                        {
                            StringBuilder stSpline = new StringBuilder();
                            stSpline.AppendFormat("M{0},{1} C", DoubleToString(pts[0].X + dx), DoubleToString(pts[0].Y + dy));
                            for (int index = 1; index < pts.Length; index++)
                            {
                                stSpline.AppendFormat("{0},{1} ", DoubleToString(pts[index].X + dx), DoubleToString(pts[index].Y + dy + index * 0.0001));
                            }

                            writer.WriteAttributeString("d", stSpline.ToString());
                        }

                        writer.WriteAttributeString("fill", "none");
                        writer.WriteAttributeString("stroke-width", DoubleToString(curve.Pen.Thickness));
                        writer.WriteAttributeString("stroke", String.Format("url(#{0})", guid));
                        if (curve.Pen.PenStyle != StiPenStyle.Solid)
                        {
                            writer.WriteAttributeString("stroke-dasharray", StiSvgHelper.GetLineStyleDash(curve.Pen.PenStyle, curve.Pen.Thickness));
                        }

                        writer.WriteEndElement();
                    }
                }
                
                else if (geom is StiImageGeom)
                {
                    var imageGeom = geom as StiImageGeom;
                    if (imageGeom.Image != null)
                    {
                        using (var image = Stimulsoft.Report.Helpers.StiImageHelper.GetImageFromObject(imageGeom.Image))
                        {
                            #region Write image
                            MemoryStream ms = new MemoryStream();
                            image.Save(ms, ImageFormat.Jpeg);

                            byte[] buf = ms.ToArray();

                            writer.WriteStartElement("image");

                            writer.WriteAttributeString("x", DoubleToString(imageGeom.Rect.X));
                            writer.WriteAttributeString("y", DoubleToString(imageGeom.Rect.Y));
                            writer.WriteAttributeString("width", DoubleToString(imageGeom.Rect.Width));
                            writer.WriteAttributeString("height", DoubleToString(imageGeom.Rect.Height));

                            writer.WriteStartAttribute("href");
                            string imageFormatMime = "jpg";
                            writer.WriteString(string.Format("data:image/{0};base64,", imageFormatMime));
                            writer.WriteRaw("\r\n");
                            writer.WriteBase64(buf, 0, buf.Length);
                            writer.WriteEndAttribute();
                            writer.WriteEndElement();
                            #endregion
                        }
                    }
                }

                else if (geom is StiBorderGeom borderGeom)
                {
                    #region Draw Border
                    var rect = RectToRectangleF(borderGeom.Rect);
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        var style = new StringBuilder();

                        style.Append(WriteFillBrush(writer, borderGeom.Background, rect, dx, dy));

                        if (CheckPenGeom(borderGeom.BorderPen))
                        {
                            var stroke = string.Format("{0}", WriteBorderStroke(writer, borderGeom.BorderPen.Brush, rect));
                            style.Append(string.Format("{0};stroke-width:{1};", stroke, borderGeom.BorderPen.Thickness));
                        }

                        if (borderGeom.CornerRadius != null && !borderGeom.CornerRadius.IsEmpty)
                        {
                            writer.WriteStartElement("path");

                            if (borderGeom.BackgroundMouseOver != null)
                                WriteFillBrushMouserOver(writer, borderGeom.BackgroundMouseOver, rect, dx, dy);

                            WriteClipPath(writer, clip);
                            WriteCicledRectPath(writer, rect, borderGeom.CornerRadius, dx, dy);
                        }
                        else
                        {
                            writer.WriteStartElement("rect");

                            if (borderGeom.BackgroundMouseOver != null)
                                WriteFillBrushMouserOver(writer, borderGeom.BackgroundMouseOver, rect, dx, dy);

                            WriteClipPath(writer, clip);
                            WriteRect(writer, rect, dx, dy);
                        }

                        writer.WriteAttributeString("style", style.ToString());
                        writer.WriteAttributeString("shape-rendering", "geometricPrecision");

                        WriteInteraction(writer, borderGeom.Interaction);

                        writer.WriteEndElement();
                    }
                    #endregion
                }

                else if (geom is StiLineGeom)
                {
                    #region Draw Line
                    var line = geom as StiLineGeom;
                    if (CheckPenGeom(line.Pen))
                    {
                        writer.WriteStartElement("line");
                        WriteClipPath(writer, clip);

                        writer.WriteAttributeString("x1", DoubleToString(line.X1 + dx));
                        writer.WriteAttributeString("y1", DoubleToString(line.Y1 + dy));
                        writer.WriteAttributeString("x2", DoubleToString(line.X2 + dx));
                        writer.WriteAttributeString("y2", DoubleToString(line.Y2 + dy));

                        var stroke = string.Format("{0}", WriteBorderStroke(writer, line.Pen.Brush, new RectangleF()));
                        var style = string.Format("{0};stroke-width:{1};", stroke, DoubleToString(line.Pen.Thickness));

                        writer.WriteAttributeString("style", style);

                        if (line.Pen.PenStyle != StiPenStyle.Solid)
                        {
                            writer.WriteAttributeString("stroke-dasharray", StiSvgHelper.GetLineStyleDash(line.Pen.PenStyle, line.Pen.Thickness));
                        }

                        writer.WriteEndElement();
                    }
                    #endregion
                }

                else if (geom is StiLinesGeom)
                {
                    #region Draw Lines
                    var lines = geom as StiLinesGeom;
                    if (CheckPenGeom(lines.Pen))
                    {
                        writer.WriteStartElement("polyline");
                        WriteClipPath(writer, clip);

                        var points = string.Empty;

                        var sb = new StringBuilder();

                        for (int i = 0; i < lines.Points.Length; i++)
                        {
                            var point = lines.Points[i];
                            sb.AppendFormat("{0},{1} ", DoubleToString(point.X + dx), DoubleToString(point.Y + dy + i * 0.0001));
                        }

                        writer.WriteAttributeString("fill", "none");

                        writer.WriteAttributeString("points", sb.ToString());

                        var stroke = string.Format("{0}", WriteBorderStroke(writer, lines.Pen.Brush, new RectangleF()));

                        var style = new StringBuilder();
                        style.AppendFormat("{0};stroke-width:{1};", stroke, DoubleToString(lines.Pen.Thickness));

                        writer.WriteAttributeString("style", style.ToString());

                        if (lines.Pen.PenStyle != StiPenStyle.Solid)
                        {
                            writer.WriteAttributeString("stroke-dasharray", StiSvgHelper.GetLineStyleDash(lines.Pen.PenStyle, lines.Pen.Thickness));
                        }

                        writer.WriteEndElement();
                    }
                    #endregion
                }

                else if (geom is StiCurveGeom)
                {
                    #region Draw Curve
                    var curve = geom as StiCurveGeom;
                    if (CheckPenGeom(curve.Pen))
                    {
                        writer.WriteStartElement("path");
                        WriteClipPath(writer, clip);

                        var pts = ConvertSplineToCubicBezier(curve.Points, curve.Tension);

                        StringBuilder stSpline = new StringBuilder();
                        stSpline.AppendFormat("M{0},{1} C", DoubleToString(pts[0].X + dx), DoubleToString(pts[0].Y + dy));

                        for (int index = 1; index < pts.Length; index++)
                        {
                            stSpline.AppendFormat("{0},{1} ", DoubleToString(pts[index].X + dx), DoubleToString(pts[index].Y + dy + index * 0.0001));
                        }

                        writer.WriteAttributeString("d", stSpline.ToString());

                        writer.WriteAttributeString("fill", "none");

                        var stroke = string.Format("{0}", WriteBorderStroke(writer, curve.Pen.Brush, new RectangleF()));
                        var style = string.Format("{0};stroke-width:{1};", stroke, DoubleToString(curve.Pen.Thickness));

                        writer.WriteAttributeString("style", style);

                        if (curve.Pen.PenStyle != StiPenStyle.Solid)
                        {
                            writer.WriteAttributeString("stroke-dasharray", StiSvgHelper.GetLineStyleDash(curve.Pen.PenStyle, curve.Pen.Thickness));
                        }

                        writer.WriteEndElement();
                    }
                    #endregion
                }

                else if (geom is StiEllipseGeom)
                {
                    #region Draw Ellipse
                    var ellipse = geom as StiEllipseGeom;

                    var rect = RectToRectangleF(ellipse.Rect);

                    var style = string.Empty;
                    var fill = string.Empty;

                    if (ellipse.Background != null)
                    {
                        style = WriteFillBrush(writer, ellipse.Background, rect, dx, dy);
                    }
                    else
                    {
                        style = "fill:none;";
                    }

                    if (CheckPenGeom(ellipse.BorderPen))
                    {
                        var stroke = string.Format("{0}", WriteBorderStroke(writer, ellipse.BorderPen.Brush, rect));
                        style += string.Format("{0};stroke-width:{1};", stroke, DoubleToString(ellipse.BorderPen.Thickness));
                    }

                    writer.WriteStartElement("ellipse");
                    WriteClipPath(writer, clip);

                    writer.WriteAttributeString("cx", DoubleToString(rect.X + dx + rect.Width / 2f));
                    writer.WriteAttributeString("cy", DoubleToString(rect.Y + dy + rect.Height / 2f));
                    writer.WriteAttributeString("rx", DoubleToString(rect.Width / 2f));
                    writer.WriteAttributeString("ry", DoubleToString(rect.Height / 2f));
                    writer.WriteAttributeString("style", style);
                    writer.WriteAttributeString("elementindex", ellipse.ElementIndex.ToString());

                    WriteInteraction(writer, ellipse.Interaction, false);
                    WriteTooltip(writer, ellipse.ToolTip);

                    writer.WriteEndElement();
                    #endregion
                }

                else if (geom is StiCachedShadowGeom && needStaticShadow)
                {
                    var shadow = geom as StiCachedShadowGeom;
                    var rect = shadow.Rect;

                    #region ClipShadow
                    var clipTop = rect.Top - 10;
                    var clipRight = rect.Right + 10;
                    var clipBottom = rect.Bottom + 10;
                    var clipLeft = rect.Left - 10;
                    if ((shadow.Sides & StiShadowSides.Top) == 0) clipTop = shadow.ClipRect.Top;
                    if ((shadow.Sides & StiShadowSides.Right) == 0) clipRight = shadow.ClipRect.Right;
                    if ((shadow.Sides & StiShadowSides.Bottom) == 0) clipBottom = shadow.ClipRect.Bottom;
                    if ((shadow.Sides & StiShadowSides.Left) == 0) clipLeft = shadow.ClipRect.Left;
                    #endregion

                    rect.X += 2;
                    rect.Y += 2;
                    var guid = "s" + StiGuidUtils.NewGuid();
                    var clipGuid = "s" + StiGuidUtils.NewGuid();
                    writer.WriteStartElement("defs");
                    writer.WriteStartElement("filter");
                    writer.WriteAttributeString("id", guid);
                    writer.WriteStartElement("feGaussianBlur");
                    writer.WriteAttributeString("in", "SourceGraphic");
                    writer.WriteAttributeString("stdDeviation", "2");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteStartElement("clipPath");
                    writer.WriteAttributeString("id", clipGuid);
                    writer.WriteStartElement("rect");
                    writer.WriteAttributeString("x", DoubleToString(clipLeft + dx));
                    writer.WriteAttributeString("y", DoubleToString(clipTop + dy));
                    writer.WriteAttributeString("width", DoubleToString(clipRight - clipLeft));
                    writer.WriteAttributeString("height", DoubleToString(clipBottom - clipTop));
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteStartElement("rect");                    
                    WriteRect(writer, rect, dx, dy);
                    writer.WriteAttributeString("fill", "rgba(190,190,190,10)");
                    writer.WriteAttributeString("filter", "url(#" + guid + ")");
                    writer.WriteAttributeString("clip-path", String.Format("url(#{0})", clipGuid));
                    writer.WriteEndElement();
                }

                else if (geom is StiShadowGeom)
                {
                    var shadow = geom as StiShadowGeom;

                    var guid = "s" + StiGuidUtils.NewGuid();
                    writer.WriteStartElement("defs");
                    writer.WriteStartElement("filter");
                    writer.WriteAttributeString("id", guid);
                    writer.WriteAttributeString("x", "0");
                    writer.WriteAttributeString("y", "0");
                    writer.WriteAttributeString("width", "200%");
                    writer.WriteAttributeString("height", "200%");

                    writer.WriteStartElement("feOffset");
                    writer.WriteAttributeString("result", "offOut");
                    writer.WriteAttributeString("in", "SourceGraphic");
                    writer.WriteAttributeString("dx", "1.111111111111111");
                    writer.WriteAttributeString("dy", "1.111111111111111");
                    writer.WriteEndElement();
                    writer.WriteStartElement("feColorMatrix");
                    writer.WriteAttributeString("result", "matrixOut");
                    writer.WriteAttributeString("in", "offOut");
                    writer.WriteAttributeString("type", "matrix");
                    writer.WriteAttributeString("values", "0.58 0 0 0 0 0 0.58 0 0 0 0 0 0.58 0 0 0 0 0 1 0");
                    writer.WriteEndElement();
                    writer.WriteStartElement("feGaussianBlur");
                    writer.WriteAttributeString("result", "blurOut");
                    writer.WriteAttributeString("in", "matrixOut");
                    writer.WriteAttributeString("stdDeviation", "1.111111111111111");
                    writer.WriteEndElement();
                    writer.WriteStartElement("feBlend");
                    writer.WriteAttributeString("mode", "normal");
                    writer.WriteAttributeString("in", "SourceGraphic");
                    writer.WriteAttributeString("in2", "blurOut");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    foreach (StiGeom sgeom in shadow.ShadowContext.Geoms)
                    {
                        StringBuilder animatedPath;
                        TimeSpan? duration;
                        var pathGeom = sgeom as StiPathGeom;
                        if (pathGeom != null)
                        {
                            var pathData = GetPathData(pathGeom.Geoms, dx + shadow.Rect.X, dy + shadow.Rect.Y, out animatedPath, out duration);
                            writer.WriteStartElement("path");
                            WriteClipPath(writer, clip);
                            writer.WriteAttributeString("d", pathData);
                            writer.WriteAttributeString("fill", "rgb(150,150,150)");
                            writer.WriteAttributeString("filter", String.Format("url(#{0})", guid));
                            writer.WriteAttributeString("elementindex", pathGeom.ElementIndex.ToString());
                            writer.WriteEndElement();
                        }
                    }
                }

                else if (geom is StiTextGeom)
                {
                    #region Draw text
                    var textGeom = geom as StiTextGeom;
                    var sf = textGeom.StringFormat.IsGeneric ? StringFormat.GenericDefault.Clone() as StringFormat : new StringFormat();
                    sf.Alignment = textGeom.StringFormat.Alignment;
                    sf.FormatFlags = textGeom.StringFormat.FormatFlags;
                    sf.HotkeyPrefix = textGeom.StringFormat.HotkeyPrefix;
                    sf.LineAlignment = textGeom.StringFormat.LineAlignment;
                    sf.Trimming = textGeom.StringFormat.Trimming;

                    var pointF = new PointF();

                    if (textGeom.Angle == 0f && (!(textGeom.Location is PointF)) && !textGeom.IsRotatedText)
                    {
                        var rect = RectToRectangleF(textGeom.Location);
                        pointF = new PointF(rect.X, rect.Y);

                        switch (sf.LineAlignment)
                        {
                            case StringAlignment.Near:
                                pointF = new PointF(rect.X, rect.Y);
                                break;

                            case StringAlignment.Center:
                                pointF = new PointF(rect.X, rect.Y + rect.Height / 2);
                                break;

                            case StringAlignment.Far:
                                pointF = new PointF(rect.X, rect.Y + rect.Height);
                                break;
                        }

                        textGeom.RotationMode = StiRotationMode.LeftCenter;
                    }
                    else
                    {
                        if (!(textGeom.Location is PointF))
                        {
                            var rect = RectToRectangleF(textGeom.Location);
                            pointF = new PointF((float)(rect.X + rect.Width / 2), (float)(rect.Y + rect.Height / 2));
                        }
                        else
                        {
                            pointF = (PointF)textGeom.Location;
                        }
                    }

                    var style = new StringBuilder();

                    var size = correctionText ? textGeom.Font.FontSize * 4 / 3 : textGeom.Font.FontSize;

                    writer.WriteStartElement("text");

                    switch (textGeom.RotationMode)
                    {
                        case StiRotationMode.LeftCenter:
                            writer.WriteAttributeString("dy", "0.5em");
                            break;

                        case StiRotationMode.LeftBottom:
                            break;

                        case StiRotationMode.CenterTop:
                            style.Append("text-anchor:middle;");
                            writer.WriteAttributeString("dy", "1em");
                            break;

                        case StiRotationMode.CenterCenter:
                            style.Append("text-anchor:middle;");
                            if (correctionText && textGeom.Font.FontName != "Stimulsoft")
                                writer.WriteAttributeString("dy", "0.4em");
                            else
                                writer.WriteAttributeString("dy", "0.3em");
                            break;

                        case StiRotationMode.CenterBottom:
                            style.Append("text-anchor:middle;");
                            break;

                        case StiRotationMode.RightTop:
                            style.Append("text-anchor:end;");
                            writer.WriteAttributeString("dy", "1em");
                            break;

                        case StiRotationMode.RightCenter:
                            style.Append("text-anchor:end;");
                            writer.WriteAttributeString("dy", "0.5em");
                            break;

                        case StiRotationMode.RightBottom:
                            style.Append("text-anchor:end;");
                            break;

                        default:
                            writer.WriteAttributeString("dy", "1em");
                            break;
                    }

                    if (textGeom.Angle == 0 && (textGeom.MaximalWidth == 0 || textGeom.MaximalWidth == null))
                    {
                        WriteClipPath(writer, clip);

                        writer.WriteAttributeString("x", DoubleToString(pointF.X + dx));
                        writer.WriteAttributeString("y", DoubleToString(pointF.Y + dy));
                    }
                    else
                    {
                        writer.WriteAttributeString("transform", string.Format("translate({0}, {1}) rotate({2} 0,0)",
                            DoubleToString(pointF.X + dx), DoubleToString(pointF.Y + dy), DoubleToString(textGeom.Angle)));
                    }


                    writer.WriteAttributeString("font-size", DoubleToString(size));
                    writer.WriteAttributeString("font-family", textGeom.Font.FontName);
                    if ((textGeom.Font.FontStyle & FontStyle.Bold) > 0)
                        writer.WriteAttributeString("font-weight", "bold");
                    if ((textGeom.Font.FontStyle & FontStyle.Italic) > 0)
                        writer.WriteAttributeString("font-style", "italic");

                    var valueDecoration = "";
                    if ((textGeom.Font.FontStyle & FontStyle.Underline) > 0)
                        valueDecoration += "underline";
                    if ((textGeom.Font.FontStyle & FontStyle.Strikeout) > 0)
                        valueDecoration += " line-through";
                    if (!String.IsNullOrEmpty(valueDecoration))
                        writer.WriteAttributeString("text-decoration", valueDecoration);

                    var textColor = textGeom.Brush is Color ? (Color)textGeom.Brush : StiBrush.ToColor(textGeom.Brush as StiBrush);
                    style.Append(string.Format("fill:#{0:X2}{1:X2}{2:X2};", textColor.R, textColor.G, textColor.B));
                    if (textColor.A != 0xFF)
                    {
                        style.Append(string.Format("fill-opacity:{0};", DoubleToString(Math.Round(textColor.A / 255f, 3))));
                    }

                    if (string.IsNullOrEmpty(textGeom.ToolTip))
                        style.Append("pointer-events: none");

                    writer.WriteAttributeString("style", style.ToString());
                    writer.WriteAttributeString("elementindex", textGeom.ElementIndex.ToString());

                    if (!string.IsNullOrEmpty(textGeom.ToolTip))
                        writer.WriteAttributeString("interactiontooltip", textGeom.ToolTip);

                    writer.WriteAttributeString("opacity", textGeom.Animation == null ? "1" : "0");

                    if (textGeom.Animation != null)
                    {
                        AddAnimation(writer, "[[\"opacity\", 0, 1, \"\"]]", textGeom.Animation.BeginTime, textGeom.Animation.Duration);
                    }

                    if (textGeom.MaximalWidth != 0 && textGeom.MaximalWidth != null && (textGeom.StringFormat.FormatFlags & StringFormatFlags.NoWrap) == 0)
                    {
                        var length = textGeom.Text.Length;
                        var wrapCharCount = length;
                        var rectTemp = new SizeF(0, 0);

                        for (var index = 0; index < length; index++)
                        {

                            rectTemp = context.MeasureString(textGeom.Text.Substring(0, index), textGeom.Font);
                            if (rectTemp.Width > textGeom.MaximalWidth && index != 0)
                            {
                                wrapCharCount = index - 1;
                                break;
                            }
                        }

                        var countRow = Math.Ceiling((double)length / wrapCharCount);
                        var startPointY = 0d;

                        switch (textGeom.RotationMode)
                        {
                            case StiRotationMode.LeftCenter:
                            case StiRotationMode.CenterCenter:
                            case StiRotationMode.RightCenter:
                                startPointY = -countRow * rectTemp.Height / 2 + rectTemp.Height / 2;//offset is taken into account 0.5em parent <text>
                                break;

                            default:
                                startPointY = 0;
                                break;
                        }

                        if (wrapCharCount > 0)
                        {

                            var startIndex = 0;
                            var index = 0;
                            while (startIndex < length)
                            {

                                writer.WriteStartElement("tspan");
                                writer.WriteAttributeString("x", "0");

                                if (index == 0)
                                    writer.WriteAttributeString("y", DoubleToString(startPointY));
                                else
                                    writer.WriteAttributeString("dy", DoubleToString(rectTemp.Height));

                                writer.WriteString(startIndex + wrapCharCount < textGeom.Text.Length
                                    ? textGeom.Text.Substring(startIndex, wrapCharCount)
                                    : textGeom.Text.Substring(startIndex));

                                writer.WriteEndElement();

                                startIndex += wrapCharCount;
                                index++;
                            }
                        }

                        WriteTooltip(writer, textGeom.ToolTip);
                    }
                    else
                    {

                        WriteTooltip(writer, textGeom.ToolTip);
                        writer.WriteValue(textGeom.Text);
                    }

                    writer.WriteEndElement();
                    #endregion
                }

                else if (geom is StiPathGeom)
                {
                    #region Draw path
                    var path = geom as StiPathGeom;

                    var rect = RectToRectangleF(path.Rect);

                    var style = string.Empty;
                    var fill = string.Empty;

                    if (path.Background != null)
                    {
                        style = WriteFillBrush(writer, path.Background, rect, dx, dy);
                    }
                    else
                    {
                        style = "fill:none;";
                    }

                    if (CheckPenGeom(path.Pen))
                    {
                        var stroke = string.Format("{0}", WriteBorderStroke(writer, path.Pen.Brush, rect));
                        style += string.Format("{0};stroke-width:{1};", stroke, DoubleToString(path.Pen.Thickness));
                    }

                    StringBuilder animatedPath;
                    TimeSpan? duration;
                    var pathData = GetPathData(path.Geoms, dx, dy, out animatedPath, out duration);

                    writer.WriteStartElement("path");
                    WriteClipPath(writer, clip);
                    writer.WriteAttributeString("d", pathData);
                    writer.WriteAttributeString("style", style);
                    writer.WriteAttributeString("elementindex", path.ElementIndex.ToString());
                    WriteInteraction(writer, path.Interaction, false);
                    WriteTooltip(writer, path.ToolTip);

                    writer.WriteEndElement();
                    #endregion
                }
            }
        }

        private static void WriteClipPath(XmlTextWriter writer, string clip)
        {
            if (!String.IsNullOrEmpty(clip))
                writer.WriteAttributeString("clip-path", String.Format("url(#{0})", clip));
        }

        private static void WriteRect(XmlTextWriter writer, RectangleF rect, float dx, float dy)
        {
            writer.WriteAttributeString("x", DoubleToString(rect.X + dx));
            writer.WriteAttributeString("y", DoubleToString(rect.Y + dy));
            writer.WriteAttributeString("width", DoubleToString(rect.Width));
            writer.WriteAttributeString("height", DoubleToString(rect.Height));
        }

        private static void WriteCicledRectPath(XmlTextWriter writer, RectangleF rect, StiCornerRadius cornerRadius, float dx, float dy)
        {
            var sb = new StringBuilder();

            var rad = Math.Min(rect.Width / 2, rect.Height / 2);
            var radiusTopLeft = StiRoundedRectangleCreator.GetRadiusTopLeft(cornerRadius, 1, rad);
            var radiusTopRight = StiRoundedRectangleCreator.GetRadiusTopRight(cornerRadius, 1, rad);
            var radiusBottomLeft = StiRoundedRectangleCreator.GetRadiusBottomLeft(cornerRadius, 1, rad);
            var radiusBottomRight = StiRoundedRectangleCreator.GetRadiusBottomRight(cornerRadius, 1, rad);

            sb.Append($"M{DoubleToString(rect.X + dx)},{DoubleToString(rect.Y + radiusTopLeft + dy)}");

            if (radiusTopLeft != 0)
            {
                var cr = DoubleToString(radiusTopLeft);
                sb.Append($" q0,-{cr} {cr},-{cr}");
            }

            sb.Append($" h{DoubleToString(rect.Width - radiusTopLeft - radiusTopRight)}");

            if (radiusTopRight != 0)
            {
                var cr = DoubleToString(radiusTopRight);
                sb.Append($" q{cr},0 {cr},{cr}");
            }

            sb.Append($" v{DoubleToString(rect.Height - radiusTopRight - radiusBottomRight)}");

            if (radiusBottomRight != 0)
            {
                var cr = DoubleToString(radiusBottomRight);
                sb.Append($" q0,{cr} -{cr},{cr}");
            }

            sb.Append($" h{DoubleToString(-rect.Width + radiusBottomRight + radiusBottomLeft)}");

            if (radiusBottomLeft != 0)
            {
                var cr = DoubleToString(radiusBottomLeft);
                sb.Append($" q-{cr},0 -{cr},-{cr}");
            }

            sb.Append($" v{DoubleToString(-rect.Height + radiusBottomLeft + radiusTopLeft)}");

            writer.WriteAttributeString("d", sb.ToString());
        }

        private static void AddAnimation(XmlTextWriter writer, String actions, TimeSpan? begin, TimeSpan duration, String number = "")
        {
            var animation = String.Format("{{\"actions\":{0}, \"begin\":{1}, \"duration\":{2}}}", actions, DoubleToString(begin.HasValue ? begin.Value.TotalMilliseconds : 0), DoubleToString(duration.TotalMilliseconds));
            writer.WriteAttributeString("_animation" + number, animation);
        }

        private static RectangleF RectToRectangleF(object rect)
        {
            if (rect is RectangleF) return (RectangleF)rect;
            if (rect is Rectangle)
            {
                return (Rectangle)rect;
            }

            if (rect is RectangleD)
            {
                var rectangle = (RectangleD)rect;
                return new RectangleF((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
            }

            return new RectangleF();
        }

        private static void WriteInteraction(XmlTextWriter writer, StiInteractionDataGeom interaction, bool addElementIndex = true)
        {
            if (interaction != null)
            {
                if (!string.IsNullOrEmpty(interaction.ComponentName))
                    writer.WriteAttributeString("interaction", interaction.ComponentName);
                if (!string.IsNullOrEmpty(interaction.PageGuid))
                    writer.WriteAttributeString("pageguid", interaction.PageGuid);
                if (!string.IsNullOrEmpty(interaction.ComponentIndex))
                    writer.WriteAttributeString("compindex", interaction.ComponentIndex);
                if (!string.IsNullOrEmpty(interaction.PageIndex))
                    writer.WriteAttributeString("pageindex", interaction.PageIndex);

                if (addElementIndex)
                    writer.WriteAttributeString("elementindex", interaction.ElementIndex);

                if (!string.IsNullOrEmpty(interaction.InteractionHyperlink))
                    writer.WriteAttributeString("interactionhyperlink", interaction.InteractionHyperlink);

                if (!string.IsNullOrEmpty(interaction.InteractionToolTip))
                    writer.WriteAttributeString("interactiontooltip", interaction.InteractionToolTip);
                                
                if (interaction.InteractionData is StiSeriesInteractionData seriesInteractionData)
                {
                    var arg = seriesInteractionData.OriginalArgument ?? seriesInteractionData.Argument;
                    var argument = arg != null && ListExt.IsList(arg) ? ListExt.ToList(arg)?.FirstOrDefault() : arg;
                    writer.WriteAttributeString("elementargument", argument == null || argument == DBNull.Value ? "sti_IsNullValue" : argument.ToString());

                    writer.WriteAttributeString("elementvalue", seriesInteractionData.Value?.ToString());

                    writer.WriteAttributeString("elementendvalue", seriesInteractionData.EndValue?.ToString());

                    var series = seriesInteractionData.Series?.CoreTitle;
                    if (String.IsNullOrEmpty(series))
                        series = seriesInteractionData.Series?.TitleValue;
                    if (!String.IsNullOrEmpty(series))
                        writer.WriteAttributeString("elementseries", series);

                    if (!string.IsNullOrEmpty(seriesInteractionData.Hyperlink))
                        writer.WriteAttributeString("interactionhyperlink", seriesInteractionData.Hyperlink);

                    if (!string.IsNullOrEmpty(seriesInteractionData.Tooltip))
                        writer.WriteAttributeString("interactiontooltip", seriesInteractionData.Tooltip);
                }

                var indicatorInteractionData = interaction.InteractionData as StiIndicatorInteractionData;
                if (indicatorInteractionData != null)
                {
                    writer.WriteAttributeString("elementvalue", indicatorInteractionData.Value?.ToString());
                    writer.WriteAttributeString("elementseries", indicatorInteractionData.SeriesText);
                    writer.WriteAttributeString("elementtarget", indicatorInteractionData.Target?.ToString());

                    if (!string.IsNullOrEmpty(interaction.InteractionToolTip))
                        writer.WriteAttributeString("interactiontooltip", interaction.InteractionToolTip);

                    if (!string.IsNullOrEmpty(interaction.InteractionHyperlink))
                        writer.WriteAttributeString("interactionhyperlink", interaction.InteractionHyperlink);
                }
            }
        }

        private static string GetPathData(List<StiSegmentGeom> geoms, float dx, float dy, out StringBuilder animatedPath, out TimeSpan? duration, StiAnimation animation = null)
        {
            string path = string.Empty;
            animatedPath = new StringBuilder();
            duration = TimeSpan.FromMilliseconds(0);

            if (geoms.Count == 4 && geoms[0] is StiArcSegmentGeom arcGeom1 && geoms[2] is StiArcSegmentGeom arcGeom2 && animation is StiPieSegmentAnimation pieAnimation)
            {
                path += AddDoughnutPath(arcGeom1.Rect, arcGeom2.Rect, arcGeom1.StartAngle, arcGeom1.SweepAngle, pieAnimation, path, dx, dy, animatedPath, out duration);

                return path;
            }

            int geomIndex = 0;
            foreach (var geom in geoms)
            {
                if (geom is StiArcSegmentGeom)
                {
                    var arcSegment = geom as StiArcSegmentGeom;
                    path += AddArcPath(arcSegment, path, dx, dy);
                }
                else if (geom is StiCurveSegmentGeom)
                {
                    var curveSegment = geom as StiCurveSegmentGeom;
                    var pointsAnimation = curveSegment.Animation as StiPointsAnimation;
                    var points = StiCurveHelper.CardinalSpline(curveSegment.Points, false);
                    var pointsFrom = pointsAnimation != null ? StiCurveHelper.CardinalSpline(pointsAnimation.PointsFrom, false) : null;
                    var sb = new StringBuilder();

                    for (int index = 1; index < points.Length; index += 3)
                    {
                        if (pointsAnimation != null)
                        {
                            if (index == 1)
                                sb.AppendFormat("C{0},{1},{2},{3},{4},{5}",
                                    DoubleToString(pointsFrom[index].X + dx), DoubleToString(pointsFrom[index].Y + dy),
                                    DoubleToString(pointsFrom[index + 1].X + dx), DoubleToString(pointsFrom[index + 1].Y + dy),
                                    DoubleToString(pointsFrom[index + 2].X + dx), DoubleToString(pointsFrom[index + 2].Y + dy));
                            else
                                sb.AppendFormat(",{0},{1},{2},{3},{4},{5}",
                                    DoubleToString(pointsFrom[index].X + dx), (pointsFrom[index].Y + dy),
                                    DoubleToString(pointsFrom[index + 1].X + dx), (pointsFrom[index + 1].Y + dy),
                                    DoubleToString(pointsFrom[index + 2].X + dx), (pointsFrom[index + 2].Y + dy + index * 0.0001));

                            if (index == 1)
                                animatedPath.AppendFormat("C{0}:{1},{2}:{3},{4}:{5},{6}:{7},{8}:{9},{10}:{11}",
                                    DoubleToString(pointsFrom[index].X + dx), DoubleToString(points[index].X + dx),
                                    DoubleToString(pointsFrom[index].Y + dy), DoubleToString(points[index].Y + dy),
                                    DoubleToString(pointsFrom[index + 1].X + dx), DoubleToString(points[index + 1].X + dx),
                                    DoubleToString(pointsFrom[index + 1].Y + dy), DoubleToString(points[index + 1].Y + dy),
                                    DoubleToString(pointsFrom[index + 2].X + dx), DoubleToString(points[index + 2].X + dx),
                                    DoubleToString(pointsFrom[index + 2].Y + dy), DoubleToString(points[index + 2].Y + dy));
                            else
                                animatedPath.AppendFormat(",{0}:{1},{2}:{3},{4}:{5},{6}:{7},{8}:{9},{10}:{11}",
                                    DoubleToString(pointsFrom[index].X + dx), DoubleToString(points[index].X + dx),
                                    DoubleToString(pointsFrom[index].Y + dy), DoubleToString(points[index].Y + dy),
                                    DoubleToString(pointsFrom[index + 1].X + dx), DoubleToString(points[index + 1].X + dx),
                                    DoubleToString(pointsFrom[index + 1].Y + dy), DoubleToString(points[index + 1].Y + dy),
                                    DoubleToString(pointsFrom[index + 2].X + dx), DoubleToString(points[index + 2].X + dx),
                                    DoubleToString(pointsFrom[index + 2].Y + dy), DoubleToString(points[index + 2].Y + dy + index * 0.0001));
                            duration = TimeSpan.FromSeconds(1);
                        }
                        else
                        {
                            if (index == 1)
                                sb.AppendFormat("C{0},{1},{2},{3},{4},{5}",
                                    DoubleToString(points[index].X + dx), DoubleToString(points[index].Y + dy),
                                    DoubleToString(points[index + 1].X + dx), DoubleToString(points[index + 1].Y + dy),
                                    DoubleToString(points[index + 2].X + dx), DoubleToString(points[index + 2].Y + dy));
                            else
                                sb.AppendFormat(",{0},{1},{2},{3},{4},{5}",
                                    DoubleToString(points[index].X + dx), DoubleToString(points[index].Y + dy),
                                    DoubleToString(points[index + 1].X + dx), DoubleToString(points[index + 1].Y + dy),
                                    DoubleToString(points[index + 2].X + dx), DoubleToString(points[index + 2].Y + dy + index * 0.0001));
                        }
                    }

                    path += sb;
                }
                else if (geom is StiBezierSegmentGeom bezierSegmentGeom)
                {
                    var sb = new StringBuilder();

                    if (!path.StartsWith("M"))
                        sb.AppendFormat("M{0},{1}", DoubleToString(bezierSegmentGeom.Pt1.X + dx), DoubleToString(bezierSegmentGeom.Pt1.Y + dy));

                    else
                        sb.AppendFormat(" {0} {1}", DoubleToString(bezierSegmentGeom.Pt1.X + dx), DoubleToString(bezierSegmentGeom.Pt1.Y + dy));

                    sb.AppendFormat("C {0} {1}, {2} {3}, {4} {5}",
                        DoubleToString(bezierSegmentGeom.Pt2.X + dx), DoubleToString(bezierSegmentGeom.Pt2.Y + dy),
                        DoubleToString(bezierSegmentGeom.Pt3.X + dx), DoubleToString(bezierSegmentGeom.Pt3.Y + dy),
                        DoubleToString(bezierSegmentGeom.Pt4.X + dx), DoubleToString(bezierSegmentGeom.Pt4.Y + dy + 0.0001));

                    path += sb;
                }
                else if (geom is StiLineSegmentGeom)
                {
                    var lineSegment = geom as StiLineSegmentGeom;
                    var pointsAnimation = lineSegment.Animation as StiPointsAnimation;

                    var sb = new StringBuilder();

                    if (!path.StartsWith("M"))
                    {
                        if (pointsAnimation != null)
                        {
                            sb.AppendFormat("M{0},{1}", DoubleToString(pointsAnimation.PointsFrom[0].X + dx), DoubleToString(pointsAnimation.PointsFrom[0].Y + dy));
                            animatedPath.AppendFormat("M{0}:{1},{2}:{3}", DoubleToString(pointsAnimation.PointsFrom[0].X + dx), DoubleToString(lineSegment.X1 + dx),
                                DoubleToString(pointsAnimation.PointsFrom[0].Y + dy), DoubleToString(lineSegment.Y1 + dy));
                        }
                        else
                        {
                            sb.AppendFormat("M{0},{1}", DoubleToString(lineSegment.X1 + dx), DoubleToString(lineSegment.Y1 + dy));
                        }
                    }

                    if (pointsAnimation != null)
                    {
                        sb.AppendFormat("L{0},{1}", DoubleToString(pointsAnimation.PointsFrom[1].X + dx), DoubleToString(pointsAnimation.PointsFrom[1].Y + dy));
                        animatedPath.AppendFormat("L{0}:{1},{2}:{3}", DoubleToString(pointsAnimation.PointsFrom[1].X + dx), DoubleToString(lineSegment.X2 + dx),
                            DoubleToString(pointsAnimation.PointsFrom[1].Y + dy), DoubleToString(lineSegment.Y2 + dy + 0.0001));
                        duration = pointsAnimation.Duration;
                    }
                    else
                    {
                        sb.AppendFormat("L{0},{1}", DoubleToString(lineSegment.X2 + dx), DoubleToString(lineSegment.Y2 + dy + 0.0001));
                    }

                    path += sb;
                }
                else if (geom is StiLinesSegmentGeom)
                {
                    var linesSegment = geom as StiLinesSegmentGeom;
                    var pointsAnimation = linesSegment.Animation as StiPointsAnimation;

                    var sb = new StringBuilder();

                    if (!path.StartsWith("M"))
                    {
                        if (pointsAnimation != null)
                        {
                            sb.AppendFormat("M{0},{1}", DoubleToString(pointsAnimation.PointsFrom[0].X + dx), DoubleToString(pointsAnimation.PointsFrom[0].Y + dy));
                            animatedPath.AppendFormat("M{0}:{1},{2}:{3}", DoubleToString(pointsAnimation.PointsFrom[0].X + dx), DoubleToString(pointsAnimation.PointsFrom[1].X + dx),
                                DoubleToString(pointsAnimation.PointsFrom[0].Y + dy), DoubleToString(pointsAnimation.PointsFrom[1].Y + dy + 0.0001));
                        }
                        else
                        {
                            sb.AppendFormat("M{0},{1}", DoubleToString(linesSegment.Points[0].X + dx), DoubleToString(linesSegment.Points[0].Y + dy + 0.0001));
                        }
                    }

                    for (int index = 0; index < linesSegment.Points.Length; index++)
                    {
                        if (pointsAnimation != null)
                        {
                            sb.AppendFormat("{0}{1},{2}", index == 0 ? "L" : ",", DoubleToString(pointsAnimation.PointsFrom[index].X + dx), DoubleToString(pointsAnimation.PointsFrom[index].Y + dy));
                            animatedPath.AppendFormat("{0}{1}:{2},{3}:{4}", index == 0 ? "L" : ",", DoubleToString(pointsAnimation.PointsFrom[index].X + dx), DoubleToString(linesSegment.Points[index].X + dx),
                                DoubleToString(pointsAnimation.PointsFrom[index].Y + dy), DoubleToString(linesSegment.Points[index].Y + dy));
                            duration = pointsAnimation.Duration;
                        }
                        else
                        {
                            sb.AppendFormat("{0}{1},{2}", index == 0 ? "L" : ",", DoubleToString(linesSegment.Points[index].X + dx), DoubleToString(linesSegment.Points[index].Y + dy + index * 0.0001));
                        }
                    }

                    path += sb;
                }
                else if (geom is StiPieSegmentGeom)
                {
                    var pieSegment = geom as StiPieSegmentGeom;
                    pieSegment.Animation = animation;
                    path += AddPiePath(pieSegment, path, dx, dy, animatedPath, out duration);
                }
                else if (geom is StiCloseFigureSegmentGeom)
                {
                    //path.IsClosed = true;
                }

                geomIndex++;
            }

            return path;
        }

        private static string AddArcPath(float centerX, float centerY, float radius, float startAngle, float sweepAngle, float dx, float dy)
        {
            var sb = new StringBuilder();

            var step = (float)Round(Math.Abs(sweepAngle / 90));
            var stepAngle = sweepAngle / step;

            for (int indexStep = 0; indexStep < step; indexStep++)
            {
                var points = ConvertArcToCubicBezier(centerX - dx, centerY - dy, radius, startAngle, stepAngle);

                for (int index = 1; index < points.Length - 1; index += 3)
                {
                    if (index == 1)
                        sb.AppendFormat("C{0},{1},{2},{3},{4},{5}",
                            DoubleToString(points[1].X + dx), DoubleToString(points[1].Y + dy),
                            DoubleToString(points[2].X + dx), DoubleToString(points[2].Y + dy),
                            DoubleToString(points[3].X + dx), DoubleToString(points[3].Y + dy));
                    else
                        sb.AppendFormat(",{0},{1},{2},{3},{4},{5}",
                            DoubleToString(points[1].X + dx), DoubleToString(points[1].Y + dy),
                            DoubleToString(points[2].X + dx), DoubleToString(points[2].Y + dy),
                            DoubleToString(points[3].X + dx), DoubleToString(points[3].Y + dy));

                    startAngle += stepAngle;
                }
            }

            return sb.ToString();
        }

        private static string AddArcPathCrossElipse(float centerX, float centerY, float radiusX, float radiusY, float startAngle, float sweepAngle, float dx, float dy)
        {
            return SvgEllipseArc(centerX, centerY, radiusX, radiusY, startAngle, sweepAngle);
        }

        private static string SvgEllipseArc(float cx, float cy, float rx, float ry, float t1, float s, float rot = 0)
        {
            if (s == 360)//hack for #5626
                s = 359.99f;

            t1 = t1 * (float)Math.PI / 180;
            s = s * (float)Math.PI / 180;

            var rotMatrix = RotateMatrix(rot);
            var sXsY = VecAdd(MatricTimes(rotMatrix, new float[] {rx * (float)Math.Cos(t1), ry * (float)Math.Sin(t1) }), new float[] { cx, cy});
            var eXeY = VecAdd(MatricTimes(rotMatrix, new float[] { rx * (float)Math.Cos(t1 + s), ry * (float)Math.Sin(t1+s) }), new float[] { cx, cy });

            var fA = s > Math.PI ? 1 : 0;
            var fS = s > 0 ? 1 : 0;

            return $"M {DoubleToString(sXsY[0])} {DoubleToString(sXsY[1])} A {DoubleToString(rx)} {DoubleToString(ry)} {DoubleToString(rot / (2 * Math.PI) * 360)} {DoubleToString(fA)} {DoubleToString(fS)} {DoubleToString(eXeY[0])} {DoubleToString(eXeY[1])}";
        }

        private static float[] MatricTimes(float[,] abcd,  float[] xy)
        { 
            var a = abcd[0,0];
            var b = abcd[0,1];
            var c = abcd[1,0];
            var d = abcd[1,1];
            var x = xy[0];
            var y = xy[1];

            return new float[] { a * x + b * y, c * x +d * y };
        }

        private static float[,] RotateMatrix(float x)
        {
            return new float[,] { { (float)Math.Cos(x), -(float)Math.Sin(x) }, { (float)Math.Sin(x), (float)Math.Cos(x) } };
        }

        private static float[] VecAdd(float[] a, float[] b)
        {
            return new float[] { a[0] + b[0], a[1] + b[1] };
        }

        private static string AddArcPath(StiArcSegmentGeom arcSegment, string path, float dx, float dy)
        {
            var sb = new StringBuilder();

            var centerX = arcSegment.Rect.X + dx + arcSegment.Rect.Width / 2;
            var centerY = arcSegment.Rect.Y + dy + arcSegment.Rect.Height / 2;
            var radiusX = arcSegment.Rect.Width / 2;
            var radiusY = arcSegment.Rect.Height / 2;

            var startAngleFrom = arcSegment.RealStartAngle != null 
                ? arcSegment.RealStartAngle.GetValueOrDefault()
                : arcSegment.StartAngle;
            var sweepAngleFrom = arcSegment.RealStartAngle != null 
                ? arcSegment.RealSweepAngle.GetValueOrDefault()
                : arcSegment.SweepAngle;

            var startAngleRad = startAngleFrom * Math.PI / 180;

            var x1 = centerX + radiusX * Math.Cos(startAngleRad);
            var y1 = centerY + radiusY * Math.Sin(startAngleRad);

            if (!path.StartsWith("M") || sweepAngleFrom % 360 == 0)
                sb.AppendFormat("M{0},{1}", DoubleToString(x1), DoubleToString(y1));

            if (arcSegment.CrossElipseDraw)
                sb.Append(AddArcPathCrossElipse(centerX, centerY, radiusX, radiusY, startAngleFrom, sweepAngleFrom, dx, dy));
            else
                sb.Append(AddArcPath(centerX, centerY, radiusX, startAngleFrom, sweepAngleFrom, dx, dy));


            return sb.ToString();
        }

        private static string AddPiePath(StiPieSegmentGeom pieSegment, string path, float dx, float dy, StringBuilder animatedPath, out TimeSpan? duration)
        {
            var sb = new StringBuilder();
            var animation = pieSegment.Animation as StiPieSegmentAnimation;

            var centerX = pieSegment.Rect.X + dx + pieSegment.Rect.Width / 2;
            var centerY = pieSegment.Rect.Y + dy + pieSegment.Rect.Height / 2;
            var radiusX = animation == null ? pieSegment.Rect.Width / 2 : animation.RectFrom.Width / 2;
            var radiusY = animation == null ? pieSegment.Rect.Height / 2 : animation.RectFrom.Height / 2;
            var startAngleFrom = animation == null ? pieSegment.StartAngle : animation.StartAngleFrom;
            var sweepAngleFrom = animation == null ? pieSegment.SweepAngle : animation.EndAngleFrom - animation.StartAngleFrom;

            if (pieSegment.RealStartAngle != null)
                startAngleFrom = pieSegment.RealStartAngle.GetValueOrDefault();

            if (pieSegment.RealSweepAngle != null)
                sweepAngleFrom = pieSegment.RealSweepAngle.GetValueOrDefault();

            var startAngle = startAngleFrom * Math.PI / 180;

            var x1 = centerX + radiusX * Math.Cos(startAngle);
            var y1 = centerY + radiusY * Math.Sin(startAngle);

            sb.AppendFormat("M{0},{1}", DoubleToString(centerX), DoubleToString(centerY));
            sb.AppendFormat("L{0},{1}", DoubleToString(x1), DoubleToString(y1));

            if (pieSegment.CrossElipseDraw)
                sb.Append(AddArcPathCrossElipse(centerX, centerY, radiusX, radiusY,
                startAngleFrom,
                sweepAngleFrom,
                dx, dy));

            else
                sb.Append(AddArcPath(centerX, centerY, radiusX, startAngleFrom, sweepAngleFrom, dx, dy));

            sb.AppendFormat("L{0},{1}", DoubleToString(centerX), DoubleToString(centerY));
            if (animation != null)
            {
                duration = animation.Duration;
                animatedPath.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    $"{{\"startAngle\": {DoubleToString(pieSegment.StartAngle)}, " +
                    $"\"startAngleFrom\": {DoubleToString(animation.StartAngleFrom)}, " +
                    $"\"sweepAngle\": {DoubleToString(pieSegment.SweepAngle)}, " +
                    $"\"sweepAngleFrom\": {DoubleToString(sweepAngleFrom)}, " +
                    $"\"radiusFrom\": {DoubleToString(radiusX)}, " +
                    $"\"x\": {DoubleToString(pieSegment.Rect.X)}, " +
                    $"\"y\": {DoubleToString(pieSegment.Rect.Y)}, " +
                    $"\"width\": {DoubleToString(pieSegment.Rect.Width)}, " +
                    $"\"height\": {DoubleToString(pieSegment.Rect.Height)}, " +
                    $"\"dx\": {DoubleToString(dx)}, " +
                    $"\"dy\": {DoubleToString(dy)}}}")));
            }
            else
            {
                duration = TimeSpan.FromSeconds(1);
            }

            return sb.ToString();
        }

        private static string AddDoughnutPath(RectangleF rect, RectangleF rectDt, float startAngle, float sweepAngle, StiPieSegmentAnimation animation, string path, float dx, float dy, StringBuilder animatedPath, out TimeSpan? duration)
        {
            var sb = new StringBuilder();

            var centerX = rect.X + dx + rect.Width / 2;
            var centerY = rect.Y + dy + rect.Height / 2;
            var radius = animation == null ? rect.Width / 2 : animation.RectFrom.Width / 2;
            var radiusDt = animation == null ? rectDt.Width / 2 : animation.RectDtFrom.Width / 2;
            var startAngleFrom = animation == null ? startAngle : animation.StartAngleFrom;
            var sweepAngleFrom = animation == null ? sweepAngle : animation.EndAngleFrom - animation.StartAngleFrom;
            var endAngleFrom = animation == null ? startAngle + sweepAngle : animation.EndAngleFrom;
            var startAngleRad = startAngleFrom * Math.PI / 180;
            var endAngleRad = (startAngleFrom + sweepAngleFrom) * Math.PI / 180;

            var x1 = centerX + radius * Math.Cos(startAngleRad);
            var y1 = centerY + radius * Math.Sin(startAngleRad);

            var xDt1 = centerX + radiusDt * Math.Cos(startAngleRad);
            var yDt1 = centerY + radiusDt * Math.Sin(startAngleRad);

            var xDt2 = centerX + radiusDt * Math.Cos(endAngleRad);
            var yDt2 = centerY + radiusDt * Math.Sin(endAngleRad);

            sb.AppendFormat("M{0},{1}", DoubleToString(xDt1), DoubleToString(yDt1));
            sb.AppendFormat("L{0},{1}", DoubleToString(x1), DoubleToString(y1));

            sb.Append(AddArcPath(centerX, centerY, radius, startAngleFrom, sweepAngleFrom, dx, dy));

            sb.AppendFormat("L{0},{1}", DoubleToString(xDt2), DoubleToString(yDt2));

            sb.Append(AddArcPath(centerX, centerY, radiusDt, endAngleFrom, -sweepAngleFrom, dx, dy));

            if (animation != null)
            {
                duration = animation.Duration;
                animatedPath.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    $"{{\"startAngle\": {DoubleToString(startAngle)}, " +
                    $"\"startAngleFrom\": {DoubleToString(animation.StartAngleFrom)}, " +
                    $"\"sweepAngle\": {DoubleToString(sweepAngle)}, " +
                    $"\"sweepAngleFrom\": {DoubleToString(sweepAngleFrom)}, " +
                    $"\"radiusFrom\": {DoubleToString(radius)}, " +
                    $"\"radiusDtFrom\": {DoubleToString(radiusDt)}, " +
                    $"\"x\": {DoubleToString(rect.X)}, " +
                    $"\"y\": {DoubleToString(rect.Y)}, " +
                    $"\"width\": {DoubleToString(rect.Width)}, " +
                    $"\"widthDt\": {DoubleToString(rectDt.Width)}, " +
                    $"\"height\": {DoubleToString(rect.Height)}, " +
                    $"\"dx\": {DoubleToString(dx)}, " +
                    $"\"dy\": {DoubleToString(dy)}}}")));
            }
            else
            {
                duration = TimeSpan.FromSeconds(1);
            }

            return sb.ToString();
        }

        public static RectangleF CorrectRectLabel(StiRotationMode rotationMode, RectangleF textRect)
        {
            switch (rotationMode)
            {
                case StiRotationMode.LeftCenter:
                    return new RectangleF(textRect.X + textRect.Width / 2, textRect.Y, textRect.Width, textRect.Height);

                case StiRotationMode.LeftBottom:
                    return new RectangleF(textRect.X + textRect.Width / 2, textRect.Y - textRect.Height / 2, textRect.Width, textRect.Height);

                case StiRotationMode.LeftTop:
                    return new RectangleF(textRect.X + textRect.Width / 2, textRect.Y + textRect.Height / 2, textRect.Width, textRect.Height);

                case StiRotationMode.CenterTop:
                    return new RectangleF(textRect.X, textRect.Y + textRect.Height / 2, textRect.Width, textRect.Height);

                case StiRotationMode.CenterCenter:
                    return textRect;

                case StiRotationMode.CenterBottom:
                    return new RectangleF(textRect.X, textRect.Y - textRect.Height / 2, textRect.Width, textRect.Height);

                case StiRotationMode.RightTop:
                    return new RectangleF(textRect.X - textRect.Width / 2, textRect.Y + textRect.Height / 2, textRect.Width, textRect.Height);

                case StiRotationMode.RightCenter:
                    return new RectangleF(textRect.X - textRect.Width / 2, textRect.Y, textRect.Width, textRect.Height);

                case StiRotationMode.RightBottom:
                    return new RectangleF(textRect.X - textRect.Width / 2, textRect.Y - textRect.Height / 2, textRect.Width, textRect.Height);

                default:
                    return textRect;
            }
        }

        private static PointF[] ConvertArcToCubicBezier(float centerX, float centerY, float radius, float startAngle, float sweepAngle)
        {
            var startAngleRad = startAngle * Math.PI / 180;
            var sweepAngleRad = sweepAngle * Math.PI / 180;
            var endAngleRad = (startAngle + sweepAngle) * Math.PI / 180;

            var x1 = centerX + radius * Math.Cos(startAngleRad);
            var y1 = centerY + radius * Math.Sin(startAngleRad);

            var x2 = centerX + radius * Math.Cos(endAngleRad);
            var y2 = centerY + radius * Math.Sin(endAngleRad);

            var l = radius * 4 / 3 * Math.Tan(0.25 * sweepAngleRad);
            var aL = Math.Atan(l / radius);
            var radL = radius / Math.Cos(aL);

            aL += startAngleRad;
            var ax1 = centerX + radL * Math.Cos(aL);
            var ay1 = centerY + radL * Math.Sin(aL);

            aL = Math.Atan(-l / radius);
            aL += endAngleRad;
            var ax2 = centerX + radL * Math.Cos(aL);
            var ay2 = centerY + radL * Math.Sin(aL);

            return new PointF[4]
            {
                new PointF((float)x1, (float)y1), new PointF((float)ax1, (float)ay1), new PointF((float)ax2, (float)ay2), new PointF((float)x2, (float)y2)
            };
        }

        private static double Round(double value)
        {
            int value1 = (int)value;
            double rest = value - value1;

            return (rest > 0) ? (double)(value1 + 1) : (double)(value1);
        }

        private static PointF[] ConvertSplineToCubicBezier(PointF[] points, float tension)
        {
            var count = points.Length;
            int len_pt = count * 3 - 2;
            var pt = new PointF[len_pt];

            tension = tension * 0.3f;

            pt[0] = points[0];
            pt[1] = CalculateCurveBezierEndPoints(points[0], points[1], tension);

            for (int index = 0; index < count - 2; index++)
            {
                var temp = CalculateCurveBezier(points, index, tension);

                pt[3 * index + 2] = temp[0];
                pt[3 * index + 3] = points[index + 1];
                pt[3 * index + 4] = temp[1];
            }

            pt[len_pt - 2] = CalculateCurveBezierEndPoints(points[count - 1], points[count - 2], tension);
            pt[len_pt - 1] = points[count - 1];

            return pt;
        }

        private static PointF[] CalculateCurveBezier(PointF[] points, int index, float tension)
        {
            float xDiff = points[index + 2].X - points[index + 0].X;
            float yDiff = points[index + 2].Y - points[index + 0].Y;

            return new PointF[]
            {
                new PointF(points[index + 1].X - tension * xDiff, points[index + 1].Y - tension * yDiff),
                new PointF(points[index + 1].X + tension * xDiff, points[index + 1].Y + tension * yDiff)
            };
        }

        private static PointF CalculateCurveBezierEndPoints(PointF end, PointF adj, float tension)
        {
            return new PointF(tension * (adj.X - end.X) + end.X, tension * (adj.Y - end.Y) + end.Y);
        }

        private static bool CheckPenGeom(StiPenGeom penGeom)
        {
            return !((penGeom == null) || (penGeom.Brush == null) || (penGeom.PenStyle == StiPenStyle.None));
        }

        public static void WriteTooltip(XmlTextWriter writer, string tooltip)
        {
            if (string.IsNullOrEmpty(tooltip)) return;

            if (StiJsonChecker.IsValidJson(tooltip))
            {
                var jsonObject = JToken.Parse(tooltip);

                var color = jsonObject.Cast<KeyValuePair<string, JToken>>().FirstOrDefault(a => a.Key == "StiColor").Value;
                var value = jsonObject.Cast<KeyValuePair<string, JToken>>().FirstOrDefault(a => a.Key == "StiValue").Value;
                var argument = jsonObject.Cast<KeyValuePair<string, JToken>>().FirstOrDefault(a => a.Key == "StiArgument").Value;
                var weight = jsonObject.Cast<KeyValuePair<string, JToken>>().FirstOrDefault(a => a.Key == "StiWeight").Value;

                var ttBrush = jsonObject.Cast<KeyValuePair<string, JToken>>().FirstOrDefault(a => a.Key == "StiToolTipBrush").Value;
                var ttTextBrush = jsonObject.Cast<KeyValuePair<string, JToken>>().FirstOrDefault(a => a.Key == "StiToolTipTextBrush").Value;
                var ttCornerRadius = jsonObject.Cast<KeyValuePair<string, JToken>>().FirstOrDefault(a => a.Key == "StiToolTipCornerRadius").Value;
                var ttBorder = jsonObject.Cast<KeyValuePair<string, JToken>>().FirstOrDefault(a => a.Key == "StiToolTipBorder").Value;

                if (weight != null)
                {
                    writer.WriteAttributeString("_text1", value.ToString());
                    if (argument != null)
                        writer.WriteAttributeString("_text2", $"{ StiLocalization.Get("PropertyMain", "Argument")}: { argument}");
                    if (value != null)
                        writer.WriteAttributeString("_text3", $"{ StiLocalization.Get("PropertyMain", "Weight")}: { weight}");
                }
                else
                {

                    if (argument != null)
                        writer.WriteAttributeString("_text1", argument.ToString());

                    if (value != null)
                        writer.WriteAttributeString("_text2", value.ToString());
                }

                if (color != null)
                {
                    var colorHtml = ColorTranslator.FromHtml(color.ToString());
                    writer.WriteAttributeString("_color", $"#{colorHtml.R:X2}{colorHtml.G:X2}{colorHtml.B:X2}");
                }

                if (ttBrush != null)
                    writer.WriteAttributeString("_ttbrush", ttBrush.ToString());

                if (ttTextBrush != null)
                    writer.WriteAttributeString("_tttextbrush", ttTextBrush.ToString());

                if (ttCornerRadius != null)
                    writer.WriteAttributeString("_ttcornerradius", ttCornerRadius.ToString());

                if (ttBorder != null)
                    writer.WriteAttributeString("_ttborder", ttBorder.ToString());

                writer.WriteAttributeString("_ismap", "true");
            }
            else
            {
                writer.WriteStartElement("title");
                writer.WriteValue(tooltip);
                writer.WriteEndElement();
            }
        }

        public static void WriteFillBrushMouserOver(XmlTextWriter writer, object brushOver, RectangleF rect, float dx = 0, float dy = 0)
        {
            if (brushOver == null) return;

            var sRect = rect;
            sRect.X += dx;
            sRect.Y += dy;

            writer.WriteAttributeString("_brushover", $"{StiBrushSvgHelper.GetFillBrush(writer, brushOver, sRect)}");
        }

        public static string WriteFillBrush(XmlTextWriter writer, object brush, RectangleF rect, float dx = 0, float dy = 0)
        {
            var sRect = rect;
            sRect.X += dx;
            sRect.Y += dy;

            return $"fill:{StiBrushSvgHelper.GetFillBrush(writer, brush, sRect)};";
        }

        private static string WriteBorderStroke(XmlTextWriter writer, object brush, RectangleF rect)
        {
            if (brush is Color)
            {
                var color = (Color)brush;
                var result = string.Format("stroke:rgb({0},{1},{2});", color.R, color.G, color.B);

                var alfa = Math.Round(color.A / 255f, 3);
                if (alfa != 1)
                    result += string.Format("stroke-opacity:{0};", DoubleToString(alfa));

                return result;
            }
            else if (brush is StiGradientBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGradientBrush(writer, brush, rect);

                return string.Format("fill:url(#{0});", gradientId);
            }
            else if (brush is StiGlareBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGlareBrush(writer, brush, rect);

                return string.Format("fill:url(#{0});", gradientId);
            }
            else if (brush is StiGlassBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGlassBrush(writer, brush, rect);

                return string.Format("fill:url(#{0});", gradientId);
            }
            else if (brush is StiHatchBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteHatchBrush(writer, brush);

                return string.Format("fill:url(#{0});", gradientId);
            }
            else if (brush is StiBrush)
            {
                var color = StiBrush.ToColor(brush as StiBrush);
                var result = string.Format("stroke:rgb({0},{1},{2})", color.R, color.G, color.B);

                var alfa = Math.Round(color.A / 255f, 3);
                if (alfa != 1)
                    result += string.Format(";stroke-opacity:{0}", DoubleToString(alfa));
            }

            return "stroke-opacity:0";
        }

        internal static string DoubleToString(double value)
        {
            return value.ToString().Replace(",", ".");
        }
        #endregion
    }
}
