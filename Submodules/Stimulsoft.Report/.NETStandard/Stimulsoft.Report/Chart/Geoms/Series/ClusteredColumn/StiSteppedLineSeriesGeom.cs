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

using System.Drawing;
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using System.Linq;
using System;

namespace Stimulsoft.Report.Chart
{
    public class StiSteppedLineSeriesGeom : StiBaseLineSeriesGeom
    {
        #region Methods
        protected PointF?[] GetSteppedPoints(PointF?[] points)
        {
            var lineSeries = this.Series as IStiSteppedLineSeries;
            var list = new List<PointF?>();

            float distX = (float)((IStiAxisArea)lineSeries.Chart.Area).XAxis.Info.Dpi / 2;

            for (int index = 0; index < points.Length - 1; index++)
            {
                PointF? point = points[index];
                PointF? nextPoint = points[index + 1];

                if (point != null && nextPoint != null)
                {
                    if (lineSeries.PointAtCenter)
                    {
                        point = new PointF(point.Value.X - distX, point.Value.Y);
                        nextPoint = new PointF(nextPoint.Value.X - distX, nextPoint.Value.Y);
                    }

                    list.Add(point);
                    list.Add(new PointF(nextPoint.Value.X, point.Value.Y));
                }
                else
                {
                    list.Add(null);
                    list.Add(null);
                }
            }

            PointF? pos = points[points.Length - 1];

            if (pos != null)
            {
                if (lineSeries.PointAtCenter)
                {
                    pos = new PointF(pos.Value.X - distX, pos.Value.Y);

                    list.Add(pos);

                    pos = new PointF(pos.Value.X + distX * 2, pos.Value.Y);
                    list.Add(pos);
                }
                else list.Add(pos);
            }
            else
            {
                list.Add(null);
            }

            return list.ToArray();
        }

        protected string[] GetSteppedPointsIds(string[] ids)
        {
            var lineSeries = this.Series as IStiSteppedLineSeries;
            var list = new List<string>();

            if (lineSeries.PointAtCenter && ids.Length > 0)
                list.Add(ids[0] + "_h");

            for (int index = 0; index < ids.Length - 1; index++)
            {
                var id = ids[index] + "_l";
                var nextId = ids[index + 1] + "_h";

                list.Add(id);
                list.Add(nextId);
            }

            var lastId = ids[ids.Length - 1] + "_l";

            list.Add(lastId);

            return list.ToArray();
        }

        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            PointF?[] newPoints = GetSteppedPoints(this.Points);

            int pointIndex = 0;
            foreach (PointF? point in newPoints)
            {
                if (Points.Length <= pointIndex + 1) continue;

                PointF? nextPoint = newPoints[pointIndex + 1];

                if (point == null || nextPoint == null) continue;

                RectangleF pointRect = RectangleF.Empty;
                if (nextPoint.Value.X > point.Value.X)
                {
                    pointRect = new RectangleF(point.Value.X, point.Value.Y, nextPoint.Value.X - point.Value.X, 1);
                    pointRect.Inflate(0, 5);
                }
                else if (nextPoint.Value.Y > point.Value.Y)
                {
                    pointRect = new RectangleF(point.Value.X, point.Value.Y, 1, nextPoint.Value.Y - point.Value.Y);
                    pointRect.Inflate(5, 0);
                }
                else
                {
                    pointRect = new RectangleF(nextPoint.Value.X, nextPoint.Value.Y, 1, point.Value.Y - nextPoint.Value.Y);
                    pointRect.Inflate(5, 0);
                }

                if (pointRect.Contains(x, y))
                    return true;

                pointIndex++;
                if (pointIndex == newPoints.Length - 1)
                    break;
            }
            return false;
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var lineSeries = this.Series as IStiSteppedLineSeries;

            context.PushSmoothingModeToAntiAlias();

            var dashStyle = lineSeries.LineStyle;
            float scaledLineWidth = lineSeries.LineWidth * context.Options.Zoom;

            var pen = new StiPenGeom(lineSeries.LineColor, scaledLineWidth);
            pen.PenStyle = dashStyle;

            var list = StiNullableDrawing.GetNullablePointsList(this.Points);

            var chart = this.Series.Chart as StiChart;

            foreach (PointF?[] newPoints2 in list)
            {
                var newPoints = GetSteppedPoints(newPoints2);
                var newPointsFrom = GetSteppedPoints(PointsFrom);
                var newPointsIds = GetSteppedPointsIds(PointsIds);

                var animation = GetAnimation(newPointsFrom, newPoints, newPointsIds);

                #region Draw Shadow
                if (lineSeries.ShowShadow)
                {
                    var penShadow = new StiPenGeom(Color.FromArgb(50, 0, 0, 0), scaledLineWidth + 0.5f * context.Options.Zoom);
                    penShadow.PenStyle = dashStyle;

                    context.PushTranslateTransform(scaledLineWidth, scaledLineWidth);
                    StiNullableDrawing.DrawLines(context, penShadow, newPoints, animation);

                    if (lineSeries.LineMarker.Visible)
                    {
                        var brushShadow = new StiSolidBrush(Color.FromArgb(50, 0, 0, 0));

                        lineSeries.LineMarker.Core.DrawLines(context, newPoints, context.Options.Zoom,
                            brushShadow, null, lineSeries.LineMarker.Type, (float)lineSeries.LineMarker.Step,
                            lineSeries.LineMarker.Size, lineSeries.LineMarker.Angle);

                    }
                    context.PopTransform();
                }
                #endregion

                #region IsMouseOver
                if (Series.Core.IsMouseOver)
                {
                    float lineWidth = lineSeries.LineWidth;
                    float zoom = context.Options.Zoom;
                    float pointSize = 11 + lineWidth;
                    foreach (PointF? point in Points)
                    {
                        if (point == null) continue;
                        RectangleF pointRect = new RectangleF(point.Value.X - pointSize / 2 * zoom, point.Value.Y - pointSize / 2 * zoom, pointSize * zoom, pointSize * zoom);
                        context.FillEllipse(StiMouseOverHelper.GetLineMouseOverColor(lineSeries.LineColor), pointRect, null);
                    }

                    StiPenGeom penMouseOver = new StiPenGeom(StiMouseOverHelper.GetLineMouseOverColor(lineSeries.LineColor), (4 + lineWidth) * context.Options.Zoom);
                    penMouseOver.StartCap = StiPenLineCap.Round;
                    penMouseOver.EndCap = StiPenLineCap.Round;
                    StiNullableDrawing.DrawLines(context, penMouseOver, newPoints, animation);
                }
                #endregion

                var axisArea = this.Series.Chart.Area as IStiAxisArea;
                float posY = axisArea.AxisCore.GetDividerY();

                if (lineSeries.AllowApplyColorNegative)
                {
                    #region AllowApplyColorNegative
                    StiPenGeom penNegative = new StiPenGeom(lineSeries.LineColorNegative, scaledLineWidth);
                    penNegative.PenStyle = dashStyle;


                    List<PointF?> pointsNegative = new List<PointF?>();
                    List<PointF?> pointsPositive = new List<PointF?>();

                    for (int index = 0; index < newPoints.Length; index++)
                    {
                        PointF? point = newPoints[index];
                        PointF? pointNext = (index != (newPoints.Length - 1)) ? newPoints[index + 1] : null;

                        if (point.Value.Y > posY)
                        {
                            pointsNegative.Add(point);

                            if (pointNext == null ||
                                pointNext.Value.Y <= posY)
                            {
                                IntersectionAxis(point, pointNext, pointsNegative, pointsPositive, posY);

                                PointF?[] points = new PointF?[pointsNegative.Count];
                                pointsNegative.CopyTo(points);

                                StiNullableDrawing.DrawLines(context, penNegative, points, animation);

                                #region Draw Light
                                if (scaledLineWidth >= 2 && lineSeries.Lighting)
                                {
                                    float step = 0.5f * context.Options.Zoom;
                                    context.PushTranslateTransform(-step, -step);
                                    StiPenGeom penLight = new StiPenGeom(StiColorUtils.Light(lineSeries.LineColorNegative, 70), scaledLineWidth);

                                    penLight.PenStyle = dashStyle;
                                    StiNullableDrawing.DrawLines(context, penLight, points, animation);

                                    context.PopTransform();
                                }
                                #endregion

                                pointsNegative.Clear();
                            }
                        }
                        else
                        {
                            pointsPositive.Add(point);

                            if (pointNext == null ||
                                pointNext.Value.Y > posY)
                            {
                                IntersectionAxis(point, pointNext, pointsPositive, pointsNegative, posY);

                                PointF?[] points = new PointF?[pointsPositive.Count];
                                pointsPositive.CopyTo(points);

                                StiNullableDrawing.DrawLines(context, pen, points, animation);

                                #region Draw Light
                                if (scaledLineWidth >= 2 && lineSeries.Lighting)
                                {
                                    float step = 0.5f * context.Options.Zoom;
                                    context.PushTranslateTransform(-step, -step);
                                    StiPenGeom penLight = new StiPenGeom(StiColorUtils.Light(lineSeries.LineColor, 70), scaledLineWidth);

                                    penLight.PenStyle = dashStyle;
                                    StiNullableDrawing.DrawLines(context, penLight, points, animation);

                                    context.PopTransform();
                                }
                                #endregion

                                pointsPositive.Clear();
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    if (list.Count == 1)
                    {
                        var newPointsStart = this.PointsFrom != null ? GetSteppedPoints(this.PointsFrom) : null;
                        StiNullableDrawing.DrawLines(context, pen, newPointsStart, newPoints, animation);
                    }
                    else
                    {
                        StiNullableDrawing.DrawLines(context, pen, newPoints, animation);
                    }

                    #region Draw Light
                    if (scaledLineWidth >= 2 && lineSeries.Lighting)
                    {
                        float step = 0.5f * context.Options.Zoom;
                        context.PushTranslateTransform(-step, -step);
                        StiPenGeom penLight = new StiPenGeom(StiColorUtils.Light(lineSeries.LineColor, 70), 1);

                        penLight.PenStyle = dashStyle;
                        StiNullableDrawing.DrawLines(context, penLight, newPoints, animation);

                        context.PopTransform();
                    }
                    #endregion
                }

                #region Draw Marker
                if (lineSeries.LineMarker.Visible)
                {
                    var borderPen = new StiPenGeom(lineSeries.LineMarker.BorderColor);

                    lineSeries.LineMarker.Core.DrawLines(context, newPoints, context.Options.Zoom, lineSeries.LineMarker.Brush, borderPen, lineSeries.LineMarker.Type,
                        (float)lineSeries.LineMarker.Step, lineSeries.LineMarker.Size, lineSeries.LineMarker.Angle);
                }
                #endregion

            }

            context.PopSmoothingMode();
        }

        private void IntersectionAxis(PointF? point, PointF? pointNext, List<PointF?> points, List<PointF?> pointsNext, float posY)
        {
            if (pointNext != null)
            {
                PointF? pointCross = new PointF(point.Value.X, posY);
                points.Add(pointCross);
                pointsNext.Add(pointCross);
            }
        }
        #endregion

        public StiSteppedLineSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : base(areaGeom, pointsInfo, series)
        {
        }
    }
}
