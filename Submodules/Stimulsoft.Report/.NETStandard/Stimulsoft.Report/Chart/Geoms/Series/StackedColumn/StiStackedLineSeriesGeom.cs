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
    public class StiStackedLineSeriesGeom : StiBaseLineSeriesGeom
    {
        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            for (int pointIndex = 0; pointIndex < (this.Points.Length - 1); pointIndex++)
            {
                PointF? point1 = this.Points[pointIndex];
                PointF? point2 = this.Points[pointIndex + 1];

                if (point1 == null || point2 == null) continue;

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
            var lineSeries = this.Series as IStiStackedBaseLineSeries;

            var chart = this.Series.Chart as StiChart;

            if (Points.Length == 0) return;

            var animation = GetAnimation();

            context.PushSmoothingModeToAntiAlias();

            var dashStyle = lineSeries.LineStyle;
            float scaledLineWidth = lineSeries.LineWidth * context.Options.Zoom;

            var pen = new StiPenGeom(lineSeries.LineColor, scaledLineWidth);
            pen.PenStyle = dashStyle;

            bool lighting = lineSeries.Lighting;

            #region ShowShadow
            if (lineSeries.ShowShadow)
            {
                var penShadow = new StiPenGeom(Color.FromArgb(50, 0, 0, 0), scaledLineWidth + 0.5f * context.Options.Zoom);
                penShadow.PenStyle = dashStyle;

                context.PushTranslateTransform(scaledLineWidth, scaledLineWidth);
                StiNullableDrawing.DrawLines(context, penShadow, Points, animation);
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
                    var pointRect = new RectangleF(point.Value.X - pointSize / 2 * zoom, point.Value.Y - pointSize / 2 * zoom, pointSize * zoom, pointSize * zoom);
                    context.FillEllipse(StiMouseOverHelper.GetLineMouseOverColor(lineSeries.LineColor), pointRect, null);
                }

                var penMouseOver = new StiPenGeom(StiMouseOverHelper.GetLineMouseOverColor(lineSeries.LineColor), (4 + lineWidth) * context.Options.Zoom)
                {
                    StartCap = StiPenLineCap.Round,
                    EndCap = StiPenLineCap.Round
                };

                StiNullableDrawing.DrawLines(context, penMouseOver, Points);
            }
            #endregion

            var coreLineColor = ((StiStackedLineSeries)this.Series).LineColor;

            if (((StiStackedLineSeries)(this.Series)).AllowApplyColorNegative)
            {

                var coreLineColorNegative = ((StiStackedLineSeries)this.Series).LineColorNegative;

                var penNegative = new StiPenGeom(coreLineColorNegative, scaledLineWidth)
                {
                    PenStyle = dashStyle
                };

                var axisArea = this.Series.Chart.Area as IStiAxisArea;
                float posY = axisArea.AxisCore.GetDividerY();

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
                                PointF? point0 = GetPointCross(point, Points[index + 1], posY);
                                pointsNegative.Add(point0);

                                if (pointNext.Value.Y == posY && ((index + 2) < Points.Length) && ((Points[index + 2].Value.Y) <= posY))
                                {
                                    pointsNegative.Add(Points[index + 2]);
                                }

                                pointsPositive.Add(point0);
                            }

                            StiNullableDrawing.DrawLines(context, penNegative, pointsNegative.ToArray(), animation);

                            #region Draw Light
                            if (scaledLineWidth >= 2 * context.Options.Zoom && lighting)
                            {
                                float step = 0.5f * context.Options.Zoom;
                                context.PushTranslateTransform(-step, -step);
                                var penLight = new StiPenGeom(StiColorUtils.Light(coreLineColorNegative, 70), scaledLineWidth)
                                {
                                    PenStyle = dashStyle
                                };

                                StiNullableDrawing.DrawLines(context, penLight, pointsNegative.ToArray(), animation);

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
                                var point0 = GetPointCross(point, Points[index + 1], posY);

                                pointsNegative.Add(point0);
                                pointsPositive.Add(point0);
                                pointsPositive.Add(pointNext);
                            }

                            StiNullableDrawing.DrawLines(context, pen, pointsPositive.ToArray(), animation);

                            #region Draw Light
                            if (scaledLineWidth >= 2 * context.Options.Zoom && lighting)
                            {
                                float step = 0.5f * context.Options.Zoom;
                                context.PushTranslateTransform(-step, -step);
                                var penLight = new StiPenGeom(StiColorUtils.Light(coreLineColor, 70), scaledLineWidth)
                                {
                                    PenStyle = dashStyle
                                };

                                StiNullableDrawing.DrawLines(context, penLight, pointsPositive.ToArray(), animation);

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
                StiNullableDrawing.DrawLines(context, pen, Points, animation);

                #region Draw Light
                if (scaledLineWidth >= 2 * context.Options.Zoom && lineSeries.Lighting)
                {
                    float step = 0.5f * context.Options.Zoom;
                    context.PushTranslateTransform(-step, -step);
                    var penLight = new StiPenGeom(StiColorUtils.Light(coreLineColor, 70), scaledLineWidth)
                    {
                        PenStyle = dashStyle
                    };
                    StiNullableDrawing.DrawLines(context, penLight, Points, animation);

                    context.PopTransform();
                }
                #endregion
            }

            context.PopSmoothingMode();
        }

        private PointF GetPointCross(PointF? point1, PointF? point2, float posY)
        {
            float y1 = point1.Value.Y;
            float x1 = point1.Value.X;

            float y2 = point2.Value.Y;
            float x2 = point2.Value.X;

            double x0 = (Math.Tan(Math.Atan((x2 - x1) / (y1 - y2)))) * (y1 - posY) + x1;
            return new PointF((float)x0, posY);
        }

        #endregion

        public StiStackedLineSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : base(areaGeom, pointsInfo, series)
        {
        }
    }
}
