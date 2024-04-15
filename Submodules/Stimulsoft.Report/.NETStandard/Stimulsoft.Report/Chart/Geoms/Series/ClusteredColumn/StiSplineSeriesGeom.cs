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
    public class StiSplineSeriesGeom : StiBaseLineSeriesGeom
    {
        #region Properties
        public PointF?[] PointsZeroConnect { get; private set; }

        public PointF?[] PointsNullConnect { get; private set; }
        #endregion

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
            var coreLineStyle = StiPenStyle.Solid; ;
            var lineWidth = 1f;
            var coreLineColor = Color.Black;
            var showShadow = true;
            var lighting = true;
            var tension = 1f;

            var animation = GetAnimation();

            #region IStiSplineSeriesXF
            var splineSeries = this.Series as IStiSplineSeries;
            if (splineSeries != null)
            {
                coreLineStyle = splineSeries.LineStyle;
                lineWidth = splineSeries.LineWidth;
                coreLineColor = splineSeries.LineColor;
                showShadow = splineSeries.ShowShadow;
                lighting = splineSeries.Lighting;
                tension = splineSeries.Tension;
            }
            #endregion

            #region IStiRadarLineSeriesXF
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

            #region IStiRadarAreaSeriesXF
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

            StiPenStyle dashStyle = coreLineStyle;
            float scaledLineWidth = lineWidth * context.Options.Zoom;

            var pen = new StiPenGeom(coreLineColor, scaledLineWidth);
            pen.PenStyle = dashStyle;

            var chart = this.Series.Chart as StiChart;

            #region showShadow
            if (showShadow)
            {
                var penShadow = new StiPenGeom(Color.FromArgb(50, 0, 0, 0), scaledLineWidth + 0.5f * context.Options.Zoom);
                penShadow.PenStyle = dashStyle;

                context.PushTranslateTransform(scaledLineWidth, scaledLineWidth);
                StiNullableDrawing.DrawCurve(context, penShadow, Points, tension, animation);
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
                StiNullableDrawing.DrawCurve(context, penMouseOver, Points, tension, animation);
            }
            #endregion

            if (PointsZeroConnect != null)
            {
                var penZeroConnect = new StiPenGeom(coreLineColor, scaledLineWidth);
                penZeroConnect.PenStyle = StiPenStyle.Dash;

                var zeroAnimation = GetAnimationConnect(PointsZeroConnect);
                StiNullableDrawing.DrawCurve(context, penZeroConnect, null, PointsZeroConnect, tension, zeroAnimation);
            }

            if (PointsNullConnect != null)
            {
                var penNullConnect = new StiPenGeom(coreLineColor, scaledLineWidth);
                penNullConnect.PenStyle = StiPenStyle.Dash;

                var nullAnimation = GetAnimationConnect(PointsNullConnect);
                StiNullableDrawing.DrawCurve(context, penNullConnect, null, PointsNullConnect, tension, nullAnimation);
            }

            StiNullableDrawing.DrawCurve(context, pen, PointsFrom, Points, tension, animation);

            #region Draw Light
            if (scaledLineWidth >= 2 && lighting)
            {
                float step = 0.5f * context.Options.Zoom;

                context.PushTranslateTransform(-step, -step);
                var penLight = new StiPenGeom(StiColorUtils.Light(coreLineColor, 70), scaledLineWidth);
                penLight.PenStyle = dashStyle;
                StiNullableDrawing.DrawCurve(context, penLight, Points, tension, animation);
                context.PopTransform();
            }
            #endregion

            if (splineSeries != null && splineSeries.AllowApplyColorNegative)
            {
                var penNegative = new StiPenGeom(splineSeries.LineColorNegative, scaledLineWidth);
                penNegative.PenStyle = dashStyle;

                var axisArea = this.Series.Chart.Area as IStiAxisArea;
                float posY = axisArea.AxisCore.GetDividerY();

                float width = (float)(axisArea.AxisCore.ScrollRangeX * axisArea.AxisCore.ScrollDpiX);
                float height = (float)(axisArea.AxisCore.ScrollRangeY * axisArea.AxisCore.ScrollDpiY - posY);

                var clipRect = new RectangleF(0, posY, width, height);
                context.PushClip(clipRect);

                StiNullableDrawing.DrawCurve(context, penNegative, Points, tension, animation);

                #region Draw Light
                if (scaledLineWidth >= 2 && lighting)
                {
                    float step = 0.5f * context.Options.Zoom;

                    context.PushTranslateTransform(-step, -step);
                    var penLight = new StiPenGeom(StiColorUtils.Light(splineSeries.LineColorNegative, 70), scaledLineWidth);
                    penLight.PenStyle = dashStyle;
                    StiNullableDrawing.DrawCurve(context, penLight, Points, tension, animation);
                    context.PopTransform();
                }
                #endregion

                context.PopClip();
            }

            context.PopSmoothingMode();
        }
        #endregion

        public StiSplineSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : base(areaGeom, pointsInfo, series)
        {
            this.PointsZeroConnect = pointsInfo.PointsZeroConnect;
            this.PointsNullConnect = pointsInfo.PointsNullConnect;
        }
    }
}
