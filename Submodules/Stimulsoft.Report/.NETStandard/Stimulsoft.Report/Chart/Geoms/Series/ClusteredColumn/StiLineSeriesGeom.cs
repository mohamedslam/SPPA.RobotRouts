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
using System.Drawing;
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiLineSeriesGeom : StiBaseLineSeriesGeom
    {
        #region Properties
        public PointF?[] PointsZeroConnect { get; }

        public PointF?[] PointsNullConnect { get; }
        #endregion

        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            for (int pointIndex = 0; pointIndex < (this.Points.Length - 1); pointIndex++)
            {
                var point1 = this.Points[pointIndex];
                var point2 = this.Points[pointIndex + 1];

                if (point1 == null || point2 == null)
                    continue;

                bool result = StiPointHelper.IsLineContainsPoint(point1.Value, point2.Value, StiMouseOverHelper.MouseOverLineDistance, new PointF(x, y));
                if (result)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var coreLineStyle = StiPenStyle.Solid;
            float lineWidth = 1f;
            var coreLineColor = Color.Black;
            bool showShadow = true;
            IStiLineMarker lineMarker = null;
            bool lighting = true;

            var animation = GetAnimation();

            #region IStiBaseLineSeries
            var lineSeries = this.Series as IStiBaseLineSeries;
            if (lineSeries != null)
            {
                coreLineStyle = lineSeries.LineStyle;
                lineWidth = lineSeries.LineWidth;
                coreLineColor = lineSeries.LineColor;
                showShadow = lineSeries.ShowShadow;
                lineMarker = lineSeries.LineMarker;
                lighting = lineSeries.Lighting;
            }
            #endregion

            #region IStiRadarLineSeries
            var radarLineSeries = this.Series as IStiRadarLineSeries;
            if (radarLineSeries != null)
            {
                coreLineStyle = radarLineSeries.LineStyle;
                lineWidth = radarLineSeries.LineWidth;
                coreLineColor = radarLineSeries.LineColor;
                showShadow = radarLineSeries.ShowShadow;
                lighting = radarLineSeries.Lighting;
            }
            #endregion

            #region IStiRadarAreaSeries
            var radarAreaSeries = this.Series as IStiRadarAreaSeries;
            if (radarAreaSeries != null)
            {
                coreLineStyle = radarAreaSeries.LineStyle;
                lineWidth = radarAreaSeries.LineWidth;
                coreLineColor = radarAreaSeries.LineColor;
                showShadow = radarAreaSeries.ShowShadow;
                lighting = radarAreaSeries.Lighting;
            }
            #endregion

            context.PushSmoothingModeToAntiAlias();

            var dashStyle = coreLineStyle;
            var scaledLineWidth = lineWidth * context.Options.Zoom;

            var pen = new StiPenGeom(coreLineColor, scaledLineWidth);
            pen.PenStyle = dashStyle;

            var chart = this.Series.Chart as StiChart;

            #region showShadow
            if (showShadow)
            {
                var penShadow = new StiPenGeom(Color.FromArgb(50, 0, 0, 0), scaledLineWidth + 0.5f * context.Options.Zoom);
                penShadow.PenStyle = dashStyle;

                context.PushTranslateTransform(scaledLineWidth, scaledLineWidth);
                StiNullableDrawing.DrawLines(context, penShadow, PointsFrom, Points, animation);

                if (lineMarker != null && lineMarker.Visible)
                {
                    var brushShadow = new StiSolidBrush(Color.FromArgb(50, 0, 0, 0));

                    lineMarker.Core.DrawLines(context, Points, (float)context.Options.Zoom,
                        brushShadow, null, lineMarker.Type,
                        (float)lineMarker.Step, (float)lineMarker.Size, lineMarker.Angle);

                }
                context.PopTransform();
            }
            #endregion

            #region IsMouseOver
            if (Series.Core.IsMouseOver)
            {
                var zoom = context.Options.Zoom;
                var pointSize = 11 + lineWidth;
                foreach (var point in Points)
                {
                    if (point == null) continue;
                    var pointRect = new RectangleF(point.Value.X - pointSize / 2 * zoom, point.Value.Y - pointSize / 2 * zoom, pointSize * zoom, pointSize * zoom);
                    context.FillEllipse(StiMouseOverHelper.GetLineMouseOverColor(coreLineColor), pointRect, null);
                }

                var penMouseOver = new StiPenGeom(StiMouseOverHelper.GetLineMouseOverColor(coreLineColor), (4 + lineWidth) * context.Options.Zoom);
                penMouseOver.StartCap = StiPenLineCap.Round;
                penMouseOver.EndCap = StiPenLineCap.Round;
                StiNullableDrawing.DrawLines(context, penMouseOver, PointsFrom, Points, animation);
            }
            #endregion

            if (PointsZeroConnect != null)
            {
                var penZeroConnect = new StiPenGeom(coreLineColor, scaledLineWidth);
                penZeroConnect.PenStyle = StiPenStyle.Dash;

                var zeroAnimation = GetAnimationConnect(PointsZeroConnect);
                StiNullableDrawing.DrawLines(context, penZeroConnect, null, PointsZeroConnect, zeroAnimation);
            }

            if (PointsNullConnect != null)
            {
                var penNullConnect = new StiPenGeom(coreLineColor, scaledLineWidth);
                penNullConnect.PenStyle = StiPenStyle.Dash;

                var nullAnimation = GetAnimationConnect(PointsNullConnect);
                StiNullableDrawing.DrawLines(context, penNullConnect, null, PointsNullConnect, nullAnimation);
            }

            if ((this.Series is StiLineSeries) && ((StiLineSeries)(this.Series)).AllowApplyColorNegative)
            {
                var coreLineColorNegative = ((StiLineSeries)this.Series).LineColorNegative;

                var penNegative = new StiPenGeom(coreLineColorNegative, scaledLineWidth);
                penNegative.PenStyle = dashStyle;

                var axisArea = this.Series.Chart.Area as IStiAxisArea;
                var posY = axisArea.AxisCore.GetDividerY();

                var pointsNegative = new List<PointF?>();
                var pointsPositive = new List<PointF?>();

                for (int index = 0; index < Points.Length; index++)
                {
                    var point = Points[index];
                    var pointNext = (index != (Points.Length - 1)) ? Points[index + 1] : null;

                    if (point.Value.Y > posY)
                    {
                        #region Negative Line
                        pointsNegative.Add(point);
                        if (pointNext == null ||
                            pointNext.Value.Y < posY ||
                            pointNext.Value.Y == posY && ((index + 2) < Points.Length) && ((Points[index + 2].Value.Y) <= posY))
                        {
                            if (pointNext != null)
                            {
                                var point0 = GetPointCross(point, Points[index + 1], posY);
                                pointsNegative.Add(point0);

                                if (pointNext.Value.Y == posY && ((index + 2) < Points.Length) && ((Points[index + 2].Value.Y) <= posY))
                                {
                                    pointsNegative.Add(Points[index + 2]);
                                }

                                pointsPositive.Add(point0);
                            }

                            StiNullableDrawing.DrawLines(context, penNegative, null, pointsNegative.ToArray(), animation);

                            #region Draw Light
                            if (scaledLineWidth >= 2 * context.Options.Zoom && lighting)
                            {
                                float step = 0.5f * context.Options.Zoom;
                                context.PushTranslateTransform(-step, -step);
                                StiPenGeom penLight = new StiPenGeom(StiColorUtils.Light(coreLineColorNegative, 70), scaledLineWidth);
                                penLight.PenStyle = dashStyle;

                                StiNullableDrawing.DrawLines(context, penLight, null, pointsNegative.ToArray(), animation);

                                context.PopTransform();
                            }
                            #endregion

                            pointsNegative.Clear();
                        }
                        #endregion
                    }
                    else
                    {
                        #region Positive Line
                        pointsPositive.Add(point);
                        if (pointNext == null || pointNext.Value.Y > posY)
                        {
                            if (pointNext != null)
                            {
                                PointF? point0 = GetPointCross(point, Points[index + 1], posY);

                                pointsNegative.Add(point0);
                                pointsPositive.Add(point0);
                                pointsPositive.Add(pointNext);
                            }

                            StiNullableDrawing.DrawLines(context, pen, null, pointsPositive.ToArray(), animation);

                            #region Draw Light
                            if (scaledLineWidth >= 2 * context.Options.Zoom && lighting)
                            {
                                float step = 0.5f * context.Options.Zoom;
                                context.PushTranslateTransform(-step, -step);
                                StiPenGeom penLight = new StiPenGeom(StiColorUtils.Light(coreLineColor, 70), scaledLineWidth);
                                penLight.PenStyle = dashStyle;

                                StiNullableDrawing.DrawLines(context, penLight, null, pointsPositive.ToArray(), animation);

                                context.PopTransform();
                            }
                            #endregion

                            pointsPositive.Clear();
                        }
                        #endregion
                    }
                }
            }
            else
            {
                StiNullableDrawing.DrawLines(context, pen, PointsFrom, Points, animation);

                #region Draw Light
                if (scaledLineWidth >= 2 * context.Options.Zoom && lighting)
                {
                    var step = 0.5f * context.Options.Zoom;
                    context.PushTranslateTransform(-step, -step);
                    var penLight = new StiPenGeom(StiColorUtils.Light(coreLineColor, 70), scaledLineWidth);
                    penLight.PenStyle = dashStyle;
                    StiNullableDrawing.DrawLines(context, penLight, PointsFrom, Points, animation);

                    context.PopTransform();
                }
                #endregion
            }

            if (lineMarker != null && lineMarker.Visible)
            {
                var borderPen = new StiPenGeom(lineMarker.BorderColor);
                lineMarker.Core.DrawLines(context, Points, context.Options.Zoom, lineMarker.Brush, borderPen, lineMarker.Type,
                    (float)lineMarker.Step, lineMarker.Size, lineMarker.Angle);
            }

            context.PopSmoothingMode();
        }

        private PointF GetPointCross(PointF? point1, PointF? point2, float posY)
        {
            var y1 = point1.Value.Y;
            var x1 = point1.Value.X;

            var y2 = point2.Value.Y;
            var x2 = point2.Value.X;

            var x0 = (Math.Tan(Math.Atan((x2 - x1) / (y1 - y2)))) * (y1 - posY) + x1;
            return new PointF((float)x0, posY);
        }
        #endregion

        public StiLineSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : base(areaGeom, pointsInfo, series)
        {
            this.PointsZeroConnect = pointsInfo.PointsZeroConnect;
            this.PointsNullConnect = pointsInfo.PointsNullConnect;
        }
    }
}
