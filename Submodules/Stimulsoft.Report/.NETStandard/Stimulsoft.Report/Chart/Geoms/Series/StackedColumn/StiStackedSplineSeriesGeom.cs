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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiStackedSplineSeriesGeom : StiBaseLineSeriesGeom
    {
        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            for (int pointIndex = 0; pointIndex < (this.Points.Length - 1); pointIndex++)
            {
                PointF? point1 = this.Points[pointIndex];
                PointF? point2 = this.Points[pointIndex + 1];

                if (point1 == null || point2 == null)
                    continue;

                bool result = StiPointHelper.IsLineContainsPoint(point1.Value, point2.Value, StiMouseOverHelper.MouseOverSplineDistance, new PointF(x, y));
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
            if (Points.Length == 0) return;

            var chart = this.Series.Chart as StiChart;

            var lineSeries = this.Series as IStiStackedSplineSeries;

            var animation = GetAnimation();

            context.PushSmoothingModeToAntiAlias();

            var dashStyle = lineSeries.LineStyle;
            float scaledLineWidth = lineSeries.LineWidth * context.Options.Zoom;

            var pen = new StiPenGeom(lineSeries.LineColor, scaledLineWidth)
            {
                PenStyle = dashStyle
            };

            if (Points != null && Points.Length > 1)
            {
                #region ShowShadow
                if (lineSeries.ShowShadow)
                {
                    var penShadow = new StiPenGeom(Color.FromArgb(50, 0, 0, 0), scaledLineWidth + 0.5f * context.Options.Zoom);
                    penShadow.PenStyle = dashStyle;
                    context.PushTranslateTransform(scaledLineWidth, scaledLineWidth);
                    StiNullableDrawing.DrawCurve(context, penShadow, Points, lineSeries.Tension, animation);
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
                    StiNullableDrawing.DrawCurve(context, penMouseOver, Points, lineSeries.Tension);
                }
                #endregion

                StiNullableDrawing.DrawCurve(context, pen, Points, lineSeries.Tension, animation);

                #region Draw Light
                if (scaledLineWidth >= 2 && lineSeries.Lighting)
                {
                    float step = 0.5f * context.Options.Zoom;
                    context.PushTranslateTransform(-step, -step);
                    var penLight = new StiPenGeom(StiColorUtils.Light(lineSeries.LineColor, 70), scaledLineWidth);
                    penLight.PenStyle = dashStyle;
                    StiNullableDrawing.DrawCurve(context, penLight, Points, lineSeries.Tension, animation);
                    context.PopTransform();
                }
                #endregion

                if (lineSeries.AllowApplyColorNegative)
                {
                    StiPenGeom penNegative = new StiPenGeom(lineSeries.LineColorNegative, scaledLineWidth)
                    {
                        PenStyle = dashStyle
                    };

                    var axisArea = this.Series.Chart.Area as IStiAxisArea;
                    float posY = axisArea.AxisCore.GetDividerY();

                    float width = (float)(axisArea.AxisCore.ScrollRangeX * axisArea.AxisCore.ScrollDpiX);
                    float height = (float)(axisArea.AxisCore.ScrollRangeY * axisArea.AxisCore.ScrollDpiY - posY);

                    var clipRect = new RectangleF(0, posY, width, height);
                    context.PushClip(clipRect);

                    StiNullableDrawing.DrawCurve(context, penNegative, Points, lineSeries.Tension, animation);

                    #region Draw Light
                    if (scaledLineWidth >= 2 && lineSeries.Lighting)
                    {
                        float step = 0.5f * context.Options.Zoom;

                        context.PushTranslateTransform(-step, -step);
                        var penLight = new StiPenGeom(StiColorUtils.Light(lineSeries.LineColorNegative, 70), scaledLineWidth)
                        {
                            PenStyle = dashStyle
                        };
                        StiNullableDrawing.DrawCurve(context, penLight, Points, lineSeries.Tension, animation);
                        context.PopTransform();
                    }
                    #endregion

                    context.PopClip();
                }
            }

            context.PopSmoothingMode();
        }
        #endregion

        public StiStackedSplineSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : base(areaGeom, pointsInfo, series)
        {
        }
    }
}
