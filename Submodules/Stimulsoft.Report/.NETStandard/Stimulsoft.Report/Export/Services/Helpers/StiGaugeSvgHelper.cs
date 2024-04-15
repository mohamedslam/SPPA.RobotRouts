/*
 * Copyright (C) 2003-2022 Stimulsoft
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Stimulsoft.Base;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Gauge.Painters;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Gauge.Helpers;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Export
{
    public static class StiGaugeSvgHelper
    {
        #region Consts
        private const double PiDiv180 = Math.PI / 180;
        private const double FourDivThree = 4d / 3d;
        #endregion

        #region Methods        
        public static void WriteGauge(XmlTextWriter writer, StiSvgData svgData, float zoom, bool needAnimation, bool isStyleSample = false)
        {
            var gauge = svgData.Component as StiGauge;
            var storeIsAnimation = gauge.IsAnimation;
            gauge.IsAnimation = needAnimation;

            using (var img = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(img))
                {
                    if (gauge.IsDesigning)
                    {
                        StiGaugeV2InitHelper.Prepare(gauge);
                    }

                    var painter = new StiGdiGaugeContextPainter(g, gauge, new RectangleF(0, 0, (float)svgData.Width, (float)svgData.Height), zoom)
                    {
                        Gauge = gauge,
                        Rect = new RectangleF((float)svgData.X, (float)svgData.Y, (float)svgData.Width, (float)svgData.Height),
                        Zoom = zoom
                    };

                    painter.Geoms.Clear();
                    gauge.DrawGauge(painter);
                    gauge.PreviousAnimations = painter.Animations;

                    //Internal use only, remove text and scale elements from style miniatures
                    if (isStyleSample)
                    {
                        painter.Geoms.RemoveAll(geom => (geom is StiTextGaugeGeom || geom is StiRectangleGaugeGeom));
                    }

                    gauge.IsAnimation = storeIsAnimation;

                    double finishTime = 0;
                    foreach (var ag in painter.Geoms)
                    {
                        if (ag == null || ag.Animation == null ||
                            !(ag.Animation.Duration.TotalMilliseconds + ag.Animation.BeginTime.TotalMilliseconds > finishTime)) continue;

                        finishTime = ag.Animation.Duration.TotalMilliseconds + ag.Animation.BeginTime.TotalMilliseconds;
                    }

                    writer.WriteStartElement("g");
                    writer.WriteAttributeString("transform", $"translate({(svgData.X + 0.5).ToString().Replace(",", ".")},{(svgData.Y + 0.5).ToString().Replace(",", ".")})");

                    foreach (var rGeom in painter.Geoms)
                    {
                        if (rGeom is StiPushMatrixGaugeGeom)
                        {
                            var tr = rGeom as StiPushMatrixGaugeGeom;
                            writer.WriteStartElement("g");

                            writer.WriteAttributeString("transform", string.Format("rotate({2}, {0}, {1})",
                                tr.CenterPoint.X.ToString().Replace(",", "."), tr.CenterPoint.Y.ToString().Replace(",", "."),
                                tr.Angle.ToString().Replace(",", ".")));
                        }
                        else if (rGeom is StiPopTranformGaugeGeom)
                        {
                            writer.WriteEndElement();
                        }
                        else if (rGeom.Type == StiGaugeGeomType.RoundedRectangle)
                        {
                            var geom = rGeom as StiRoundedRectangleGaugeGeom;
                            var rect = RectToRectangleF(geom.Rect);
                            var style = new StringBuilder();

                            if (geom.Background != null)
                                style.Append(WriteFillBrush(writer, geom.Background, rect));

                            else
                                style.Append("fill-opacity:0;");

                            var stroke = $"{WriteBorderStroke(writer, geom.BorderBrush, rect)}";
                            style.Append($"{stroke};stroke-width:{geom.BorderWidth};");
                            writer.WriteStartElement("rect");
                            writer.WriteAttributeString("x", rect.X.ToString().Replace(",", "."));
                            writer.WriteAttributeString("y", rect.Y.ToString().Replace(",", "."));
                            writer.WriteAttributeString("ry", geom.LeftTop.ToString().Replace(",", ".")); //TODO
                            writer.WriteAttributeString("rx", geom.LeftTop.ToString().Replace(",", ".")); //TODO
                            writer.WriteAttributeString("height", rect.Height.ToString().Replace(",", "."));
                            writer.WriteAttributeString("width", rect.Width.ToString().Replace(",", "."));
                            writer.WriteAttributeString("shape-rendering", "crispEdges");
                            writer.WriteAttributeString("style", style.ToString());
                            writer.WriteEndElement();
                        }
                        else if (rGeom.Type == StiGaugeGeomType.Rectangle)
                        {
                            var geom = rGeom as StiRectangleGaugeGeom;
                            var rect = RectToRectangleF(geom.Rect);
                            var style = new StringBuilder();

                            if (geom.Background != null)
                                style.Append(WriteFillBrush(writer, geom.Background, rect));

                            else
                                style.Append("fill-opacity:0;");

                            var stroke = $"{WriteBorderStroke(writer, geom.BorderBrush, rect)}";
                            style.Append($"{stroke};stroke-width:{geom.BorderWidth};");

                            writer.WriteStartElement("rect");
                            writer.WriteAttributeString("x", rect.X.ToString().Replace(",", "."));
                            writer.WriteAttributeString("y", rect.Y.ToString().Replace(",", "."));
                            writer.WriteAttributeString("height", (rect.Height + 1).ToString().Replace(",", "."));
                            writer.WriteAttributeString("width", (rect.Width + 1).ToString().Replace(",", "."));
                            writer.WriteAttributeString("shape-rendering", "crispEdges");
                            writer.WriteAttributeString("style", style.ToString());
                            writer.WriteEndElement();
                        }
                        else if (rGeom.Type == StiGaugeGeomType.GraphicsPath)
                        {
                            var geom = rGeom as StiGraphicsPathGaugeGeom;
                            var rect = RectToRectangleF(geom.Rect);

                            var style = geom.Background != null
                                ? WriteFillBrush(writer, geom.Background, rect)
                                : "fill-opacity:0;";

                            var stroke = $"{WriteBorderStroke(writer, geom.BorderBrush, rect)}";
                            style += $"{stroke};stroke-width:{geom.BorderWidth.ToString().Replace(",", ".")};";

                            var pathData = GetPathData(geom.Geoms, geom.StartPoint);
                            writer.WriteStartElement("path");
                            writer.WriteAttributeString("d", pathData);
                            writer.WriteAttributeString("style", style);

                            if (geom.Animation != null)
                            {
                                if (geom.Animation is StiTranslationAnimation)
                                {
                                    var translationAnimation = geom.Animation as StiTranslationAnimation;
                                    var dx = (translationAnimation.StartPoint.X - translationAnimation.EndPoint.X).ToString().Replace(",", ".");
                                    var dy = (translationAnimation.StartPoint.Y - translationAnimation.EndPoint.Y).ToString().Replace(",", ".");

                                    writer.WriteStartElement("animateTransform");
                                    writer.WriteAttributeString("attributeType", "xml");
                                    writer.WriteAttributeString("attributeName", "transform");
                                    writer.WriteAttributeString("type", "translate");
                                    writer.WriteAttributeString("from", $"{dx} {dy}");
                                    writer.WriteAttributeString("to", "0 0");
                                    writer.WriteAttributeString("dur", $"{translationAnimation.Duration}");
                                    writer.WriteEndElement();
                                }
                                else if (geom.Animation is StiRotationAnimation)
                                {
                                    var rotationAnimation = geom.Animation as StiRotationAnimation;

                                    writer.WriteStartElement("animateTransform");
                                    writer.WriteAttributeString("attributeType", "xml");
                                    writer.WriteAttributeString("attributeName", "transform");
                                    writer.WriteAttributeString("type", "rotate");
                                    writer.WriteAttributeString("from", $"{(rotationAnimation.StartAngle - rotationAnimation.EndAngle).ToString().Replace(",", ".")} {rotationAnimation.CenterPoint.X.ToString().Replace(",", ".")} {rotationAnimation.CenterPoint.Y.ToString().Replace(",", ".")}");
                                    writer.WriteAttributeString("to", $"0 {rotationAnimation.CenterPoint.X.ToString().Replace(",", ".")} {rotationAnimation.CenterPoint.Y.ToString().Replace(",", ".")}");
                                    writer.WriteAttributeString("dur", $"{rotationAnimation.Duration}");
                                    writer.WriteEndElement();
                                }
                                else if (geom.Animation is StiScaleAnimation)
                                {
                                    var scaleAnimation = geom.Animation as StiScaleAnimation;
                                    var dx = ((-scaleAnimation.StartScaleX + scaleAnimation.EndScaleX) * scaleAnimation.CenterX).ToString().Replace(",", ".");
                                    var dy = ((-scaleAnimation.StartScaleY + scaleAnimation.EndScaleY) * scaleAnimation.CenterY).ToString().Replace(",", ".");

                                    writer.WriteStartElement("animateTransform");
                                    writer.WriteAttributeString("attributeType", "xml");
                                    writer.WriteAttributeString("attributeName", "transform");
                                    writer.WriteAttributeString("type", "translate");
                                    writer.WriteAttributeString("from", $"{dx} {dy}");
                                    writer.WriteAttributeString("to", "0 0");
                                    writer.WriteAttributeString("dur", $"{scaleAnimation.Duration}");
                                    writer.WriteEndElement();

                                    writer.WriteStartElement("animateTransform");
                                    writer.WriteAttributeString("attributeType", "xml");
                                    writer.WriteAttributeString("attributeName", "transform");
                                    writer.WriteAttributeString("type", "scale");
                                    writer.WriteAttributeString("from", $"{scaleAnimation.StartScaleX} {scaleAnimation.StartScaleY}");
                                    writer.WriteAttributeString("to", $"{scaleAnimation.EndScaleX} {scaleAnimation.EndScaleY}");
                                    writer.WriteAttributeString("dur", $"{scaleAnimation.Duration}");
                                    writer.WriteAttributeString("additive", "sum");
                                    writer.WriteEndElement();
                                }
                            }

                            writer.WriteEndElement();
                        }
                        else if (rGeom.Type == StiGaugeGeomType.Pie)
                        {
                            var geom = rGeom as StiPieGaugeGeom;
                            var rect = RectToRectangleF(geom.Rect);

                            var style = geom.Background != null
                                ? WriteFillBrush(writer, geom.Background, rect)
                                : "fill-opacity:0;";

                            var stroke = $"{WriteBorderStroke(writer, geom.BorderBrush, rect)}";
                            style += $"{stroke};stroke-width:{geom.BorderWidth.ToString().Replace(",", ".")};";

                            var pathData = AddPiePath(geom, "");
                            writer.WriteStartElement("path");
                            writer.WriteAttributeString("d", pathData);
                            writer.WriteAttributeString("style", style);
                            writer.WriteEndElement();
                        }
                        else if (rGeom.Type == StiGaugeGeomType.Ellipse)
                        {
                            var ellipse = rGeom as StiEllipseGaugeGeom;
                            var rect = RectToRectangleF(ellipse.Rect);

                            var style = ellipse.Background != null
                                ? WriteFillBrush(writer, ellipse.Background, rect)
                                : "fill-opacity:0;";

                            var stroke = $"{WriteBorderStroke(writer, ellipse.BorderBrush, rect)}";
                            style += $"{stroke};stroke-width:{ellipse.BorderWidth.ToString().Replace(",", ".")};";

                            writer.WriteStartElement("ellipse");
                            writer.WriteAttributeString("cx", (rect.X + rect.Width / 2f).ToString().Replace(",", "."));
                            writer.WriteAttributeString("cy", (rect.Y + rect.Height / 2f).ToString().Replace(",", "."));
                            writer.WriteAttributeString("rx", (rect.Width / 2f).ToString().Replace(",", "."));
                            writer.WriteAttributeString("ry", (rect.Height / 2f).ToString().Replace(",", "."));
                            writer.WriteAttributeString("style", style);

                            writer.WriteEndElement();
                        }
                        else if (rGeom.Type == StiGaugeGeomType.GraphicsArcGeometry)
                        {
                            var geom = rGeom as StiGraphicsArcGeometryGaugeGeom;
                            var rect = RectToRectangleF(geom.Rect);
                            var style = geom.Background != null
                                ? WriteFillBrush(writer, geom.Background, rect)
                                : "fill-opacity:0;";

                            var stroke = $"{WriteBorderStroke(writer, geom.BorderBrush, rect)}";
                            style += $"{stroke};stroke-width:{geom.BorderWidth.ToString().Replace(",", ".")};";
                            var pathData = AddArcPath(geom, "");
                            writer.WriteStartElement("path");
                            writer.WriteAttributeString("d", pathData);
                            writer.WriteAttributeString("style", style);
                            writer.WriteEndElement();
                        }
                        else if (rGeom.Type == StiGaugeGeomType.Text)
                        {
                            var textGeom = rGeom as StiTextGaugeGeom;
                            var font = new Font(StiFontCollection.GetFontFamily(textGeom.Font.Name), textGeom.Font.Size, textGeom.Font.Style, textGeom.Font.Unit, textGeom.Font.GdiCharSet, textGeom.Font.GdiVerticalFont);
                            var rect = RectToRectangleF(textGeom.Rect);
                            var pointF = new PointF(rect.X, rect.Y);

                            var size = font.Size * 4 / 3;

                            WriteText(writer, textGeom.Text, font, textGeom.Foreground, pointF, size);
                        }
                        else if (rGeom.Type == StiGaugeGeomType.RadialRange)
                        {
                            var geom = rGeom as StiRadialRangeGaugeGeom;
                            var currentStartAngle = geom.StartAngle * PiDiv180;
                            var path = new StringBuilder();
                            var centerPoint = geom.CenterPoint;

                            var x1 = centerPoint.X + geom.Radius1 * Math.Cos(currentStartAngle);
                            var y1 = centerPoint.Y + geom.Radius1 * Math.Sin(currentStartAngle);
                            var steps = Math.Round(Math.Abs(geom.SweepAngle / 10));
                            var stepAngle = geom.SweepAngle / steps;

                            #region First Arc
                            double restRadius = geom.Radius1 - geom.Radius2;
                            var offsetStep = 1 / (steps);
                            double offset = 0;

                            path.AppendFormat("M{0},{1}", x1.ToString().Replace(",", "."), y1.ToString().Replace(",", "."));

                            currentStartAngle = geom.StartAngle;
                            var indexStep = -1;
                            while (++indexStep < steps)
                            {
                                var startRadius = geom.Radius1 - restRadius * offset;
                                var endRadius = geom.Radius1 - restRadius * (offset + offsetStep);
                                var points = ConvertArcToCubicBezier(new PointF(centerPoint.X, centerPoint.Y), startRadius, endRadius, currentStartAngle, stepAngle);

                                if (indexStep == 0)
                                {
                                    path.AppendFormat(" C{0},{1},{2},{3},{4},{5}",
                                        points[1].X.ToString().Replace(",", "."), points[1].Y.ToString().Replace(",", "."),
                                        points[2].X.ToString().Replace(",", "."), points[2].Y.ToString().Replace(",", "."),
                                        points[3].X.ToString().Replace(",", "."), points[3].Y.ToString().Replace(",", "."));
                                }
                                else
                                {
                                    path.AppendFormat(",{0},{1},{2},{3},{4},{5}",
                                        points[1].X.ToString().Replace(",", "."), points[1].Y.ToString().Replace(",", "."),
                                        points[2].X.ToString().Replace(",", "."), points[2].Y.ToString().Replace(",", "."),
                                        points[3].X.ToString().Replace(",", "."), points[3].Y.ToString().Replace(",", "."));
                                }


                                currentStartAngle += stepAngle;
                                offset += offsetStep;
                            }
                            #endregion

                            #region Second Arc
                            stepAngle = geom.SweepAngle / steps;
                            restRadius = geom.Radius3 - geom.Radius4;
                            offsetStep = 1 / steps;
                            offset = steps;
                            currentStartAngle = geom.StartAngle + geom.SweepAngle;

                            for (indexStep = 0; indexStep < steps; indexStep++)
                            {
                                var startRadius = geom.Radius3 - (restRadius * offset);
                                var endRadius = geom.Radius3 - (restRadius * (offset + offsetStep));
                                var points = ConvertArcToCubicBezier(centerPoint, startRadius, endRadius, currentStartAngle, -stepAngle);

                                if (indexStep == 0)
                                {
                                    path.AppendFormat(" L{0},{1}", points[0].X.ToString().Replace(",", "."), points[0].Y.ToString().Replace(",", "."));

                                    path.AppendFormat(" C{0},{1},{2},{3},{4},{5}",
                                        points[1].X.ToString().Replace(",", "."), points[1].Y.ToString().Replace(",", "."),
                                        points[2].X.ToString().Replace(",", "."), points[2].Y.ToString().Replace(",", "."),
                                        points[3].X.ToString().Replace(",", "."), points[3].Y.ToString().Replace(",", "."));
                                }
                                else
                                {
                                    path.AppendFormat(",{0},{1},{2},{3},{4},{5}",
                                        points[1].X.ToString().Replace(",", "."), points[1].Y.ToString().Replace(",", "."),
                                        points[2].X.ToString().Replace(",", "."), points[2].Y.ToString().Replace(",", "."),
                                        points[3].X.ToString().Replace(",", "."), points[3].Y.ToString().Replace(",", "."));
                                }

                                currentStartAngle -= stepAngle;
                                offset -= offsetStep;
                            }

                            path.AppendFormat("z");
                            #endregion

                            var rect = RectToRectangleF(geom.Rect);
                            var style = geom.Background != null ? WriteFillBrush(writer, geom.Background, rect) : "fill-opacity:0;";
                            var stroke = $"{WriteBorderStroke(writer, geom.BorderBrush, rect)}";
                            style += $"{stroke};stroke-width:{geom.BorderWidth.ToString().Replace(",", ".")};";
                            writer.WriteStartElement("path");
                            writer.WriteAttributeString("d", path.ToString());
                            writer.WriteAttributeString("style", style);
                            writer.WriteEndElement();
                        }
                    }
                }
            }

            writer.WriteEndElement();
        }

        public static void WriteGauge(XmlTextWriter writer, StiSvgData svgData, bool needAnimation, bool isStyleSample = false)
        {
            WriteGauge(writer, svgData, 1f, needAnimation, isStyleSample);
        }

        private static string GetPathData(List<StiGaugeGeom> geoms, PointF startPoint)
        {
            var path = $"M{startPoint.X.ToString().Replace(",", ".")},{startPoint.Y.ToString().Replace(",", ".")}";

            var geomIndex = 0;
            foreach (var gm in geoms)
            {
                if (gm.Type == StiGaugeGeomType.GraphicsPathArc)
                {
                    var arcGeom = gm as StiGraphicsPathArcGaugeGeom;
                    path += GetArcPath(new RectangleF(arcGeom.X, arcGeom.Y, arcGeom.Width, arcGeom.Height), path, arcGeom.StartAngle, arcGeom.SweepAngle, geomIndex == 0);
                }
                else if (gm.Type == StiGaugeGeomType.GraphicsPathLine)
                {
                    var lineGeom = gm as StiGraphicsPathLineGaugeGeom;

                    var sb = new StringBuilder();

                    if (!path.StartsWith("M"))
                        sb.AppendFormat("M{0},{1}", lineGeom.P2.X.ToString().Replace(",", "."), lineGeom.P2.Y.ToString().Replace(",", "."));

                    sb.AppendFormat("L{0},{1}", lineGeom.P2.X.ToString().Replace(",", "."), lineGeom.P2.Y.ToString().Replace(",", "."));

                    path += sb;
                }
                else if (gm.Type == StiGaugeGeomType.GraphicsPathLines)
                {
                    var linesSegment = gm as StiGraphicsPathLinesGaugeGeom;

                    var sb = new StringBuilder();

                    if (!path.StartsWith("M"))
                        sb.AppendFormat("M{0},{1}", linesSegment.Points[0].X.ToString().Replace(",", "."), linesSegment.Points[0].Y.ToString().Replace(",", "."));

                    for (var index = 0; index < linesSegment.Points.Length; index++)
                    {
                        if (index == 0)
                            sb.AppendFormat("L{0},{1}", linesSegment.Points[index].X.ToString().Replace(",", "."), linesSegment.Points[index].Y.ToString().Replace(",", "."));
                        else
                            sb.AppendFormat(",{0},{1}", linesSegment.Points[index].X.ToString().Replace(",", "."), linesSegment.Points[index].Y.ToString().Replace(",", "."));
                    }

                    path += sb;
                }
                else if (gm.Type == StiGaugeGeomType.GraphicsPathCloseFigure)
                {
                    path += "z";
                }

                geomIndex++;
            }

            return path;
        }

        public static string GetArcPath(RectangleF rect, string path_, float startAngle, double sweepAngle, bool isSetStartPoint)
        {
            var isDraw = false;
            var path = new StringBuilder(path_);

            var centerPoint = new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);

            var leftTopPoint = new PointF(centerPoint.X - rect.Width / 2, centerPoint.Y - rect.Height / 2);
            var centerTopPoint = new PointF(centerPoint.X, centerPoint.Y - rect.Height / 2);
            var rightTopPoint = new PointF(centerPoint.X + rect.Width / 2, centerPoint.Y - rect.Height / 2);

            var rightMiddlePoint = new PointF(centerPoint.X + rect.Width / 2, centerPoint.Y);

            var rightBottomPoint = new PointF(centerPoint.X + rect.Width / 2, centerPoint.Y + rect.Height / 2);
            var centerBottomPoint = new PointF(centerPoint.X, centerPoint.Y + rect.Height / 2);
            var leftBottomPoint = new PointF(centerPoint.X - rect.Width / 2, centerPoint.Y + rect.Height / 2);

            var leftMiddlePoint = new PointF(centerPoint.X - rect.Width / 2, centerPoint.Y);

            if (startAngle == 0)
            {
                if (sweepAngle == 90)
                {
                    if (isSetStartPoint)
                        path.AppendFormat("M{0},{1}", rightMiddlePoint.X.ToString().Replace(",", "."), rightMiddlePoint.Y.ToString().Replace(",", "."));

                    path.AppendFormat("Q{0},{1},{2},{3}", rightBottomPoint.X.ToString().Replace(",", "."), rightBottomPoint.Y.ToString().Replace(",", "."),
                        centerBottomPoint.X.ToString().Replace(",", "."), centerBottomPoint.Y.ToString().Replace(",", "."));

                    isDraw = true;
                }
                else if (sweepAngle == 180)
                {
                    if (isSetStartPoint)
                        path.AppendFormat("M{0},{1}", rightMiddlePoint.X.ToString().Replace(",", "."), rightMiddlePoint.Y.ToString().Replace(",", "."));

                    path.AppendFormat("A{0},{1},90,1,1,{2},{3}", (rect.Height / 2).ToString().Replace(",", "."), (rect.Width / 2).ToString().Replace(",", "."),
                        leftMiddlePoint.X.ToString().Replace(",", "."), leftMiddlePoint.Y.ToString().Replace(",", "."));

                    isDraw = true;
                }
            }
            else if (startAngle == 90)
            {
                if (sweepAngle == 90)
                {
                    if (isSetStartPoint)
                        path.AppendFormat("M{0},{1}", centerBottomPoint.X.ToString().Replace(",", "."), centerBottomPoint.Y.ToString().Replace(",", "."));

                    path.AppendFormat("Q{0},{1},{2},{3}", leftBottomPoint.X.ToString().Replace(",", "."), leftBottomPoint.Y.ToString().Replace(",", "."),
                        leftMiddlePoint.X.ToString().Replace(",", "."), leftMiddlePoint.Y.ToString().Replace(",", "."));

                    isDraw = true;
                }
                else if (sweepAngle == 180)
                {
                    if (isSetStartPoint)
                        path.AppendFormat("M{0}, {1}", centerBottomPoint.X.ToString().Replace(",", "."), centerBottomPoint.Y.ToString().Replace(",", "."));

                    path.AppendFormat("A{0},{1},90,1,1,{2},{3}", (rect.Height / 2).ToString().Replace(",", "."), (rect.Width / 2).ToString().Replace(",", "."),
                        centerTopPoint.X.ToString().Replace(",", "."), centerTopPoint.Y.ToString().Replace(",", "."));

                    isDraw = true;
                }
            }
            else if (startAngle == 180)
            {
                if (sweepAngle == 90)
                {
                    if (isSetStartPoint)
                        path.AppendFormat("M{0},{1}", leftMiddlePoint.X.ToString().Replace(",", "."), leftMiddlePoint.Y.ToString().Replace(",", "."));

                    path.AppendFormat("Q{0},{1},{2},{3}", leftTopPoint.X.ToString().Replace(",", "."), leftTopPoint.Y.ToString().Replace(",", "."),
                        centerTopPoint.X.ToString().Replace(",", "."), centerTopPoint.Y.ToString().Replace(",", "."));

                    isDraw = true;
                }
                else if (sweepAngle == 180)
                {
                    if (isSetStartPoint)
                        path.AppendFormat("M{0},{1}", leftMiddlePoint.X.ToString().Replace(",", "."), leftMiddlePoint.Y.ToString().Replace(",", "."));

                    path.AppendFormat("A{0},{1},90,1,1,{2},{3}", (rect.Height / 2).ToString().Replace(",", "."), (rect.Width / 2).ToString().Replace(",", "."),
                        rightMiddlePoint.X.ToString().Replace(",", "."), rightMiddlePoint.Y.ToString().Replace(",", "."));

                    isDraw = true;
                }
            }
            else if (startAngle == 270)
            {
                if (sweepAngle == 90)
                {
                    path.AppendFormat("M{0},{1}", centerTopPoint.X.ToString().Replace(",", "."), centerTopPoint.Y.ToString().Replace(",", "."));
                    path.AppendFormat("Q{0},{1},{2},{3}", rightTopPoint.X.ToString().Replace(",", "."), rightTopPoint.Y.ToString().Replace(",", "."),
                        rightMiddlePoint.X.ToString().Replace(",", "."), rightMiddlePoint.Y.ToString().Replace(",", "."));
                    isDraw = true;
                }
                else if (sweepAngle == 180)
                {
                    if (isSetStartPoint)
                        path.AppendFormat("M{0},{1}", centerTopPoint.X.ToString().Replace(",", "."), centerTopPoint.Y.ToString().Replace(",", "."));

                    path.AppendFormat("A{0},{1},90,1,1,{2},{3}", (rect.Height / 2).ToString().Replace(",", "."), (rect.Width / 2).ToString().Replace(",", "."),
                        centerBottomPoint.X.ToString().Replace(",", "."), centerBottomPoint.Y.ToString().Replace(",", "."));

                    isDraw = true;
                }
            }

            if (!isDraw)
            {
                var radius = Math.Min(rect.Width / 2, rect.Height / 2);
                var currentStartAngle = startAngle * PiDiv180;

                var x1 = (float)(centerPoint.X + radius * Math.Cos(currentStartAngle));
                var y1 = (float)(centerPoint.Y + radius * Math.Sin(currentStartAngle));

                var lastPoint = new PointF(x1, y1);

                var addLine = false;
                if (path.Length == 0)
                    path.AppendFormat("M{0},{1}", lastPoint.X.ToString().Replace(",", "."), lastPoint.Y.ToString().Replace(",", "."));
                else
                    addLine = true;

                var steps = Round(Math.Abs(sweepAngle / 90));
                var stepAngle = sweepAngle / steps;
                currentStartAngle = startAngle;

                #region First Arc
                var indexStep = -1;
                while (++indexStep < steps)
                {
                    var points = ConvertArcToCubicBezier(centerPoint, radius, currentStartAngle, stepAngle);

                    if (addLine)
                    {
                        path.AppendFormat("M{0},{1}", points[0].X.ToString().Replace(",", "."), points[0].Y.ToString().Replace(",", "."));
                        addLine = false;
                    }

                    path.AppendFormat(" C{0},{1},{2},{3},{4},{5}",
                        points[1].X.ToString().Replace(",", "."), points[1].Y.ToString().Replace(",", "."),
                        points[2].X.ToString().Replace(",", "."), points[2].Y.ToString().Replace(",", "."),
                        points[3].X.ToString().Replace(",", "."), points[3].Y.ToString().Replace(",", "."));

                    currentStartAngle += stepAngle;
                }
                #endregion
            }

            return path.ToString();
        }

        private static PointF[] ConvertArcToCubicBezier(PointF centerPoint, double radius, double startAngle, double sweepAngle)
        {
            var startAngle1 = startAngle * PiDiv180;
            var sweepAngle1 = sweepAngle * PiDiv180;
            var endAngle1 = startAngle1 + sweepAngle1;

            var x1 = centerPoint.X + radius * Math.Cos(startAngle1);
            var y1 = centerPoint.Y + radius * Math.Sin(startAngle1);

            var x2 = centerPoint.X + radius * Math.Cos(endAngle1);
            var y2 = centerPoint.Y + radius * Math.Sin(endAngle1);

            var l = radius * FourDivThree * Math.Tan(0.25 * sweepAngle1);
            var aL = Math.Atan(l / radius);
            var radL = radius / Math.Cos(aL);

            aL += startAngle1;
            var ax1 = centerPoint.X + radL * Math.Cos(aL);
            var ay1 = centerPoint.Y + radL * Math.Sin(aL);

            aL = Math.Atan(-l / radius);
            aL += endAngle1;
            var ax2 = centerPoint.X + radL * Math.Cos(aL);
            var ay2 = centerPoint.Y + radL * Math.Sin(aL);

            return new[]
            {
                new PointF((float) x1, (float) y1),
                new PointF((float) ax1, (float) ay1),
                new PointF((float) ax2, (float) ay2),
                new PointF((float) x2, (float) y2)
            };
        }

        private static string AddArcPath(object arcSegment, string path)
        {
            var x = arcSegment is StiGraphicsPathArcGaugeGeom ? (arcSegment as StiGraphicsPathArcGaugeGeom).X : (arcSegment as StiGraphicsArcGeometryGaugeGeom).Rect.X;
            var y = arcSegment is StiGraphicsPathArcGaugeGeom ? (arcSegment as StiGraphicsPathArcGaugeGeom).Y : (arcSegment as StiGraphicsArcGeometryGaugeGeom).Rect.Y;
            var width = arcSegment is StiGraphicsPathArcGaugeGeom ? (arcSegment as StiGraphicsPathArcGaugeGeom).Width : (arcSegment as StiGraphicsArcGeometryGaugeGeom).Rect.Width;
            var height = arcSegment is StiGraphicsPathArcGaugeGeom ? (arcSegment as StiGraphicsPathArcGaugeGeom).Height : (arcSegment as StiGraphicsArcGeometryGaugeGeom).Rect.Height;
            var startAngle_ = arcSegment is StiGraphicsPathArcGaugeGeom ? (arcSegment as StiGraphicsPathArcGaugeGeom).StartAngle : (arcSegment as StiGraphicsArcGeometryGaugeGeom).StartAngle;
            var sweepAngle = arcSegment is StiGraphicsPathArcGaugeGeom ? (arcSegment as StiGraphicsPathArcGaugeGeom).SweepAngle : (arcSegment as StiGraphicsArcGeometryGaugeGeom).SweepAngle;

            var sb = new StringBuilder();

            double centerX = x + width / 2;
            double centerY = y + height / 2;
            double radius = width / 2;
            var startAngle = startAngle_ * Math.PI / 180;

            var x1 = centerX + radius * Math.Cos(startAngle);
            var y1 = centerY + radius * Math.Sin(startAngle);

            if (!path.StartsWith("M"))
                sb.AppendFormat("M{0},{1}", x1.ToString().Replace(",", "."), y1.ToString().Replace(",", "."));

            var step = Round(Math.Abs(sweepAngle / 10));
            var stepAngle = sweepAngle / step;
            startAngle = startAngle_;

            for (var indexStep = 0; indexStep < step; indexStep++)
            {
                var points = ConvertArcToCubicBezier(new RectangleF(x, y, width, height), startAngle, stepAngle);

                if (indexStep == 0)
                {
                    sb.AppendFormat(" C{0},{1},{2},{3},{4},{5}",
                        points[1].X.ToString().Replace(",", "."), points[1].Y.ToString().Replace(",", "."),
                        points[2].X.ToString().Replace(",", "."), points[2].Y.ToString().Replace(",", "."),
                        points[3].X.ToString().Replace(",", "."), points[3].Y.ToString().Replace(",", "."));
                }
                else
                {
                    sb.AppendFormat(",{0},{1},{2},{3},{4},{5}",
                        points[1].X.ToString().Replace(",", "."), points[1].Y.ToString().Replace(",", "."),
                        points[2].X.ToString().Replace(",", "."), points[2].Y.ToString().Replace(",", "."),
                        points[3].X.ToString().Replace(",", "."), points[3].Y.ToString().Replace(",", "."));
                }

                startAngle += stepAngle;
            }

            if (arcSegment is StiGraphicsArcGeometryGaugeGeom)
            {
                #region Second Arc
                var geom = arcSegment as StiGraphicsArcGeometryGaugeGeom;
                step = Round(Math.Abs(sweepAngle / 10));
                stepAngle = sweepAngle / step;
                var secondStartRadius = radius - (width * geom.StartWidth);
                var secondEndRadius = radius - (width * geom.EndWidth);

                if (secondStartRadius <= 0 || secondEndRadius <= 0) return null;

                var offsetSecondRadius = secondStartRadius - secondEndRadius;

                var offsetStep = 1 / (step);
                var offset = step;
                startAngle = startAngle_ + sweepAngle;
                for (var indexStep = 0; indexStep < step; indexStep++)
                {
                    var startRadius = secondStartRadius - (offsetSecondRadius * offset);
                    var endRadius = secondStartRadius - (offsetSecondRadius * (offset + offsetStep));
                    var points = ConvertArcToCubicBezier(new PointF((float)centerX, (float)centerY), startRadius, endRadius, startAngle, -stepAngle);

                    if (indexStep == 0)
                    {
                        sb.AppendFormat(" L{0},{1}", points[0].X.ToString().Replace(",", "."), points[0].Y.ToString().Replace(",", "."));

                        sb.AppendFormat(" C{0},{1},{2},{3},{4},{5}",
                            points[1].X.ToString().Replace(",", "."), points[1].Y.ToString().Replace(",", "."),
                            points[2].X.ToString().Replace(",", "."), points[2].Y.ToString().Replace(",", "."),
                            points[3].X.ToString().Replace(",", "."), points[3].Y.ToString().Replace(",", "."));
                    }
                    else
                    {
                        sb.AppendFormat(",{0},{1},{2},{3},{4},{5}",
                            points[1].X.ToString().Replace(",", "."), points[1].Y.ToString().Replace(",", "."),
                            points[2].X.ToString().Replace(",", "."), points[2].Y.ToString().Replace(",", "."),
                            points[3].X.ToString().Replace(",", "."), points[3].Y.ToString().Replace(",", "."));
                    }

                    startAngle -= stepAngle;
                    offset -= offsetStep;
                }

                sb.AppendFormat("z");
                #endregion
            }

            return sb.ToString();
        }

        private static string AddPiePath(StiPieGaugeGeom pieSegment, string path)
        {
            var sb = new StringBuilder();

            double centerX = pieSegment.Rect.X + pieSegment.Rect.Width / 2;
            double centerY = pieSegment.Rect.Y + pieSegment.Rect.Height / 2;
            double radius = pieSegment.Rect.Width / 2;
            var startAngle = pieSegment.StartAngle * Math.PI / 180;

            var x1 = centerX + radius * Math.Cos(startAngle);
            var y1 = centerY + radius * Math.Sin(startAngle);

            sb.AppendFormat("M{0},{1}", centerX.ToString().Replace(",", "."), centerY.ToString().Replace(",", "."));
            sb.AppendFormat("L{0},{1}", x1.ToString().Replace(",", "."), y1.ToString().Replace(",", "."));

            var step = Round(Math.Abs(pieSegment.SweepAngle / 90));
            var stepAngle = pieSegment.SweepAngle / step;
            startAngle = pieSegment.StartAngle;

            for (var indexStep = 0; indexStep < step; indexStep++)
            {
                var points = ConvertArcToCubicBezier(pieSegment.Rect, startAngle, stepAngle);

                for (var index = 1; index < points.Length - 1; index += 3)
                {
                    if (index == 1)
                    {
                        sb.AppendFormat("C{0},{1},{2},{3},{4},{5}",
                            points[index].X.ToString().Replace(",", "."), points[index].Y.ToString().Replace(",", "."),
                            points[index + 1].X.ToString().Replace(",", "."), points[index + 1].Y.ToString().Replace(",", "."),
                            points[index + 2].X.ToString().Replace(",", "."), points[index + 2].Y.ToString().Replace(",", "."));
                    }
                    else
                    {
                        sb.AppendFormat(",{0},{1},{2},{3},{4},{5}",
                            points[index].X.ToString().Replace(",", "."), points[index].Y.ToString().Replace(",", "."),
                            points[index + 1].X.ToString().Replace(",", "."), points[index + 1].Y.ToString().Replace(",", "."),
                            points[index + 2].X.ToString().Replace(",", "."), points[index + 2].Y.ToString().Replace(",", "."));
                    }
                }

                startAngle += stepAngle;
            }

            sb.AppendFormat("L{0},{1}", centerX.ToString().Replace(",", "."), centerY.ToString().Replace(",", "."));

            return sb.ToString();
        }

        private static PointF[] ConvertArcToCubicBezier(RectangleF rect, double startAngle1, double sweepAngle1)
        {
            var centerX = rect.X + rect.Width / 2;
            var centerY = rect.Y + rect.Height / 2;

            var radius = Math.Min(rect.Width / 2, rect.Height / 2);

            var startAngle = startAngle1 * Math.PI / 180;
            var sweepAngle = sweepAngle1 * Math.PI / 180;
            var endAngle = (startAngle1 + sweepAngle1) * Math.PI / 180;

            var x1 = (float)(centerX + radius * Math.Cos(startAngle));
            var y1 = (float)(centerY + radius * Math.Sin(startAngle));

            var x2 = (float)(centerX + radius * Math.Cos(endAngle));
            var y2 = (float)(centerY + radius * Math.Sin(endAngle));

            var l = radius * 4 / 3 * Math.Tan(0.25 * sweepAngle);
            var aL = Math.Atan(l / radius);
            var radL = radius / Math.Cos(aL);

            aL += startAngle;
            var ax1 = (float)(centerX + radL * Math.Cos(aL));
            var ay1 = (float)(centerY + radL * Math.Sin(aL));

            aL = Math.Atan(-l / radius);
            aL += endAngle;
            var ax2 = (float)(centerX + radL * Math.Cos(aL));
            var ay2 = (float)(centerY + radL * Math.Sin(aL));

            return new[]
            {
                new PointF(x1, y1),
                new PointF(ax1, ay1),
                new PointF(ax2, ay2),
                new PointF(x2, y2)
            };
        }

        private static PointF[] ConvertArcToCubicBezier(PointF centerPoint, double radius1, double radius2, double startAngle, double sweepAngle)
        {
            var startAngle1 = startAngle * PiDiv180;
            var sweepAngle1 = sweepAngle * PiDiv180;
            var endAngle = startAngle1 + sweepAngle1;

            var x1 = centerPoint.X + radius1 * Math.Cos(startAngle1);
            var y1 = centerPoint.Y + radius1 * Math.Sin(startAngle1);

            var x2 = centerPoint.X + radius2 * Math.Cos(endAngle);
            var y2 = centerPoint.Y + radius2 * Math.Sin(endAngle);

            var rest = (radius1 - radius2) / 3;
            radius1 -= rest;

            var l = radius1 * FourDivThree * Math.Tan(0.25 * sweepAngle1);
            var aL = Math.Atan(l / radius1);
            var radL = radius1 / Math.Cos(aL);

            aL += startAngle1;
            var ax1 = centerPoint.X + radL * Math.Cos(aL);
            var ay1 = centerPoint.Y + radL * Math.Sin(aL);

            aL = Math.Atan(-l / radius1);
            aL += endAngle;
            var ax2 = centerPoint.X + radL * Math.Cos(aL);
            var ay2 = centerPoint.Y + radL * Math.Sin(aL);

            return new[]
            {
                new PointF((float) x1, (float) y1),
                new PointF((float) ax1, (float) ay1),
                new PointF((float) ax2, (float) ay2),
                new PointF((float) x2, (float) y2)
            };
        }

        private static double Round(double value)
        {
            var value1 = (int)value;
            var rest = value - value1;

            return rest > 0 ? value1 + 1 : value1;
        }

        public static void WriteText(XmlTextWriter writer, string text, Font font, StiBrush foreground, PointF pointF, float size)
        {
            var style = new StringBuilder();
            writer.WriteStartElement("text");
            style.AppendFormat("text-anchor:{0};", "middle");
            writer.WriteAttributeString("dy", "0.9em");
            writer.WriteAttributeString("dx", $"{(0.3 * text.Length).ToString().Replace(",", ".")}em");
            writer.WriteAttributeString("transform", $"translate({pointF.X.ToString().Replace(",", ".")}, {pointF.Y.ToString().Replace(",", ".")}) ");
            writer.WriteAttributeString("font-size", size.ToString().Replace(",", "."));
            writer.WriteAttributeString("font-family", font.FontFamily.Name);
            var textColor = StiBrush.ToColor(foreground);
            style.Append($"fill:#{textColor.R:X2}{textColor.G:X2}{textColor.B:X2};");

            if (textColor.A != 0xFF)
                style.Append($"fill-opacity:{Math.Round(textColor.A / 255f, 3).ToString().Replace(",", ".")};");

            writer.WriteAttributeString("style", style.ToString());
            writer.WriteValue(text);
            writer.WriteEndElement();
        }

        public static string WriteFillBrush(XmlTextWriter writer, object brush, RectangleF rect)
        {
            if (brush is Color)
            {
                var color = (Color)brush;
                return $"fill:rgb({color.R},{color.G},{color.B});fill-opacity:{Math.Round(color.A / 255f, 3).ToString().Replace(",", ".")};";
            }

            if (brush is StiGradientBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGradientBrush(writer, brush, rect);

                return $"fill:url(#{gradientId});";
            }

            if (brush is StiGlareBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGlareBrush(writer, brush, rect);

                return $"fill:url(#{gradientId});";
            }

            if (brush is StiGlassBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGlassBrush(writer, brush, rect);

                return $"fill:url(#{gradientId});";
            }

            if (brush is StiHatchBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteHatchBrush(writer, brush);

                return $"fill:url(#{gradientId});";
            }

            if (brush is StiBrush)
            {
                var color = StiBrush.ToColor(brush as StiBrush);
                return $"fill:rgb({color.R},{color.G},{color.B});fill-opacity:{Math.Round(color.A / 255f, 3).ToString().Replace(",", ".")};";
            }

            return "fill-opacity:0;";
        }

        private static string WriteBorderStroke(XmlTextWriter writer, object brush, RectangleF rect)
        {
            if (brush is Color)
            {
                var color = (Color)brush;
                var result = $"stroke:rgb({color.R},{color.G},{color.B});";

                var alfa = Math.Round(color.A / 255f, 3);
                if (alfa != 1)
                    result += $"stroke-opacity:{alfa.ToString().Replace(",", ".")};";

                return result;
            }

            if (brush is StiSolidBrush)
            {
                var solid = (StiSolidBrush)brush;
                var result = $"stroke:rgb({solid.Color.R},{solid.Color.G},{solid.Color.B});";

                var alfa = Math.Round(solid.Color.A / 255f, 3);
                if (alfa != 1)
                    result += $"stroke-opacity:{alfa.ToString().Replace(",", ".")};";

                return result;
            }

            if (brush is StiGradientBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGradientBrush(writer, brush, rect);

                return $"fill:url(#{gradientId});";
            }

            if (brush is StiGlareBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGlareBrush(writer, brush, rect);

                return $"fill:url(#{gradientId});";
            }

            if (brush is StiGlassBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGlassBrush(writer, brush, rect);

                return $"fill:url(#{gradientId});";
            }

            if (brush is StiHatchBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteHatchBrush(writer, brush);

                return $"fill:url(#{gradientId});";
            }

            if (brush is StiBrush)
            {
                var color = StiBrush.ToColor(brush as StiBrush);
                var result = $"stroke:rgb({color.R},{color.G},{color.B})";

                var alfa = Math.Round(color.A / 255f, 3);
                if (alfa != 1)
                    result += $";stroke-opacity:{alfa.ToString().Replace(",", ".")}";
            }

            return "stroke-opacity:0";
        }

        private static RectangleF RectToRectangleF(object rect)
        {
            if (rect is RectangleF)
                return (RectangleF)rect;

            if (rect is Rectangle)
                return (Rectangle)rect;

            if (rect is RectangleD)
            {
                var rectangle = (RectangleD)rect;
                return new RectangleF((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
            }

            return new RectangleF();
        }
        #endregion
    }
}