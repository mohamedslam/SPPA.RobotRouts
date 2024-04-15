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

using System;
using System.Drawing;
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Report.Chart.Geoms.Series.Doughnut;

namespace Stimulsoft.Report.Chart
{
    public class StiDoughnutSeriesCoreXF : StiPieSeriesCoreXF
    {
        #region Methods
        private StiDoughnutSeriesElementGeom RenderDoughnutElement(StiContext context, PointF center, float radius, float radiusDt,
            Color borderColor, StiBrush brush, float start, float angle, double? value, int index,
            IStiDoughnutSeries currentSeries, bool shadow, StiAreaGeom areaGeom, TimeSpan beginTime)
        {
            if (radiusDt < 0) radiusDt = 1;

            if (angle == 0 || float.IsNaN(angle)) return null;
            
            var rectPie = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            var rectPieDt = new RectangleF(center.X - radiusDt, center.Y - radiusDt, radiusDt * 2, radiusDt * 2);

            if (rectPie.Width <= 0 && rectPie.Height <= 0) return null;

            #region Correct brush
            if (!shadow)
            {
                if (brush is StiGradientBrush)
                {
                    brush = brush.Clone() as StiGradientBrush;
                    ((StiGradientBrush)brush).Angle = -45f;
                }

                if (brush is StiGlareBrush)
                {
                    brush = brush.Clone() as StiGlareBrush;
                    ((StiGlareBrush)brush).Angle = -45f;
                }
            }
            #endregion

            var path = new List<StiSegmentGeom>();
            
            #region Init light
            StiBrush brLight = null;
            StiBrush brDark = null;
            List<StiSegmentGeom> pathLight = null;
            List<StiSegmentGeom> pathDark = null;
            float lightWidth = 0;

            if (currentSeries.Lighting && (!shadow))
            {
                pathLight = new List<StiSegmentGeom>();
                pathDark = new List<StiSegmentGeom>();

                lightWidth = radius * 0.02f;
                brLight = new StiGradientBrush(
                    Color.FromArgb(100, Color.White),
                    Color.FromArgb(50, Color.Black), 45);

                brDark = new StiGradientBrush(
                    Color.FromArgb(50, Color.Black),
                    Color.FromArgb(100, Color.White), 45);
            }
            #endregion

            var chart = this.Series.Chart as StiChart;

            #region Create path
            path.Add(new StiArcSegmentGeom(rectPie, start, angle));
            path.Add(new StiLineSegmentGeom(GetPoint(center, radius, start + angle), GetPoint(center, radiusDt, start + angle)));
            path.Add(new StiArcSegmentGeom(rectPieDt, start + angle, -angle));
            path.Add(new StiLineSegmentGeom(GetPoint(center, radiusDt, start), GetPoint(center, radius, start)));
            path.Add(new StiCloseFigureSegmentGeom());

            if (shadow)
            {
                if (chart.IsAnimation)
                {
                    var animation = new StiOpacityAnimation(TimeSpan.FromSeconds(1), beginTime);
                    context.FillDrawAnimationPath(brush, null, path, rectPie, null, animation, null);
                }
                else
                {
                    context.FillPath(brush, path, rectPie, null);
                }
            }
            #endregion

            #region Draw light
            if (brLight != null && (!shadow))
            {
                // Internal path
                pathLight.Add(new StiLineSegmentGeom(GetPoint(center, radius - lightWidth, start), GetPoint(center, radius, start)));
                pathLight.Add(new StiArcSegmentGeom(rectPie, start, angle));
                pathLight.Add(new StiLineSegmentGeom(GetPoint(center, radius, start + angle), GetPoint(center, radius - lightWidth, start + angle)));
                pathLight.Add(new StiArcSegmentGeom(new RectangleF(rectPie.X + lightWidth, rectPie.Y + lightWidth,
                    rectPie.Width - lightWidth * 2, rectPie.Height - lightWidth * 2), start + angle, -angle));

                // External path
                pathDark.Add(new StiLineSegmentGeom(GetPoint(center, radiusDt + lightWidth, start), GetPoint(center, radiusDt, start)));
                pathDark.Add(new StiArcSegmentGeom(rectPieDt, start, angle));
                pathDark.Add(new StiLineSegmentGeom(GetPoint(center, radiusDt, start + angle), GetPoint(center, radiusDt + lightWidth, start + angle)));
                pathDark.Add(new StiArcSegmentGeom(new RectangleF(rectPieDt.X - lightWidth, rectPieDt.Y - lightWidth,
                    rectPieDt.Width + lightWidth * 2, rectPieDt.Height + lightWidth * 2), start + angle, -angle));
            }
            #endregion

            if (!shadow)
            {
                var seriesGeom = new StiDoughnutSeriesElementGeom(areaGeom, value.GetValueOrDefault(), index, currentSeries, rectPie, rectPieDt, path, pathLight, pathDark,
                        borderColor, brush, brLight, brDark, start, start + angle, radius, radiusDt, beginTime);
                return seriesGeom;
            }

            return null;
        }

        private bool IsNotNullValues(IStiSeries[]  seriesArray)
        {
            foreach (var series in seriesArray)
            {
                foreach (var value in series.Values)
                {
                    if (value != null)
                        return true;
                }
            }

            return false;
        }

        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesArray)
        {
            float radius = GetRadius(context, rect);
            var pointCenter = GetPointCenter(rect);

            if (seriesArray == null || seriesArray.Length == 0 || this.Series.Chart == null || (seriesArray != null && !IsNotNullValues(seriesArray)))
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(new StiDoughnutEmptySeriesElementGeom(new RectangleF(pointCenter.X - radius, pointCenter.Y - radius, 2 * radius, 2 * radius)));
                return;
            }

            var duration = StiChartHelper.GlobalDurationElement;
            var beginTime = StiChartHelper.GlobalBeginTimeElement;

            float step =  radius / (seriesArray.Length + 1);

            float sumStep = step;
            int colorCount = 0;
            foreach (IStiDoughnutSeries ser in seriesArray)
            {
                if (ser.Values != null)
                    colorCount += ser.Values.Length;

                sumStep += ser.Width != 0 ? ser.Width  : step;
            }

            radius = sumStep;

            if (colorCount == 0) return;
            
            var currentSeries = seriesArray[0] as IStiPieSeries;

            #region Measure Series Labels
            int colorIndex = 0;
            int index = 0;            

            var bounds = new RectangleF(10 * context.Options.Zoom, 10 * context.Options.Zoom,
                rect.Width - 20 * context.Options.Zoom, rect.Height - 20 * context.Options.Zoom);
            var rectPie2 = bounds;

            float angle = 0;

            float radiusDt = radius;
            foreach (IStiDoughnutSeries ser in seriesArray)
            {
                angle = ser.StartAngle;
                float gradPerValue = GetGradPerValue(ser);
                foreach (double? value in ser.Values)
                {
                    float percentPerValue = GetPercentPerValue(ser);
                    float arcWidth = (float)(gradPerValue * Math.Abs(value.GetValueOrDefault()));

                    if (this.Series.Chart != null && this.Series.Chart.SeriesLabels != null && this.Series.Chart.SeriesLabels.Visible)
                    {
                        IStiPieSeriesLabels labels = this.Series.Chart.SeriesLabels as IStiPieSeriesLabels;

                        if (labels != null && labels.Visible)
                        {
                            var radiusDt2 = ser.Width != 0 ? radiusDt - ser.Width : radiusDt - step;

                            RectangleF measureRect;
                            ((StiPieSeriesLabelsCoreXF)labels.Core).RenderLabel
                            (ser, context, pointCenter, radiusDt, radiusDt2,
                                angle + arcWidth / 2, index, Math.Abs(value.GetValueOrDefault()), value,
                                GetArgumentText(ser, index),
                                currentSeries.Core.GetTag(index), true,
                                colorIndex, colorCount, percentPerValue, out measureRect, false, 0);

                            if (!measureRect.IsEmpty)
                                bounds = RectangleF.Union(bounds, measureRect);
                        }
                    }
                    colorIndex++;

                    angle += arcWidth;
                    index++;
                }
                radiusDt -= ser.Width != 0 ? ser.Width + step * 0.2f : step * 1.2f;
            }

            float dist = 0;
            dist = Math.Min(dist, bounds.Left - rectPie2.Left);
            dist = Math.Min(dist, rectPie2.Right - bounds.Right);
            dist = Math.Min(dist, bounds.Top - rectPie2.Top);
            dist = Math.Min(dist, rectPie2.Bottom - bounds.Bottom);
            radius += dist;
            #endregion
            
            var chart = this.Series.Chart as StiChart;

            #region Render Shadow
            index = 0;
            if (currentSeries.ShowShadow && !chart.IsAnimation)
            {
                var rectPie = new RectangleF(
                    pointCenter.X - radius,
                    pointCenter.Y - radius,
                    radius * 2, radius * 2);

                var shadowContext = context.CreateShadowGraphics();

                var shadowBrush = new StiSolidBrush(Color.FromArgb(100, Color.Black));

                step = radius / (seriesArray.Length + 1);
                
                radiusDt = radius;
                foreach (IStiDoughnutSeries ser in seriesArray)
                {
                    angle = ser.StartAngle;
                    float gradPerValue = GetGradPerValue(ser);

                    foreach (double? value in ser.Values)
                    {
                        float arcWidth = (float)(gradPerValue * Math.Abs(value.GetValueOrDefault()));

                        var radiusDt2 = ser.Width != 0 ? radiusDt - ser.Width : radiusDt - step;

                        RenderDoughnutElement(shadowContext, new PointF(rectPie.Width / 2, rectPie.Height / 2),
                            radiusDt, radiusDt2, Color.Black, shadowBrush,
                            angle, arcWidth, Math.Abs(value.GetValueOrDefault()), index, ser, true, geom, new TimeSpan(beginTime.Ticks));

                        angle += arcWidth;
                        index++;
                    }
                    radiusDt -= ser.Width != 0 ? ser.Width + step * 0.2f : step * 1.2f;
                }

                var shadowGeom = new StiPieSeriesShadowElementGeom(currentSeries, rectPie, (radius * 0.01f + 2 * context.Options.Zoom), shadowContext, duration, beginTime);

                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(shadowGeom);
            }
            #endregion

            #region Render DoughnutElements
            step = radius / (seriesArray.Length + 1);
            colorIndex = 0;
            index = 0;
            radiusDt = radius;
            foreach (IStiDoughnutSeries ser in seriesArray)
            {
                var colorIndexCondition = 0;

                if (!ser.Chart.Area.ColorEach)
                    colorIndex = 0;

                angle = ser.StartAngle;
                float gradPerValue = GetGradPerValue(ser);

                #region Check for zeros
                int nonZeroValuesCount = 0;
                int firstNonZeroValueIndex = -1;
                IStiSeries firstNonZeroSeries = null;
                double? firstNonZeroValue = CheckNonZerovalue(seriesArray, out nonZeroValuesCount, out firstNonZeroValueIndex, out firstNonZeroSeries);
                #endregion

                if (nonZeroValuesCount == 0)
                {
                }

                #region Show one element
                else if (nonZeroValuesCount == 1)
                {
                    var seriesBrush = ser.Brush;
                    if (ser.AllowApplyBrush)
                    {
                        seriesBrush = ser.Core.GetSeriesBrush(firstNonZeroValueIndex, colorCount);
                        seriesBrush = ser.ProcessSeriesBrushes(firstNonZeroValueIndex, seriesBrush);
                    }

                    var borderColor = (Color)currentSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);
                    if (firstNonZeroValueIndex >= ser.Values.Length) continue;

                    var radiusDt2 = ser.Width != 0 ? radiusDt - ser.Width : radiusDt - step;

                    var doughnutElementGeom = RenderDoughnutElement(context, pointCenter, radiusDt, radiusDt2, borderColor, seriesBrush,
                                0, 360, ser.Values[firstNonZeroValueIndex], firstNonZeroValueIndex, ser, false, geom, new TimeSpan(beginTime.Ticks));

                    if (doughnutElementGeom != null)
                    {
                        if (ser.Core.Interaction != null)
                            doughnutElementGeom.Interaction = new StiSeriesInteractionData(geom.Area, ser, 0);

                        geom.CreateChildGeoms();
                        geom.ChildGeoms.Add(doughnutElementGeom);
                    }

                    radiusDt -= step * 1.2f;
                    colorIndexCondition++;
                    colorIndex++;
                    index++;
                    continue;
                }
                #endregion
                
                else
                {
                    var currentIndex = 0;
                    foreach (double? value in ser.Values)
                    {

                        float arcWidth = (float)(gradPerValue * Math.Abs(value.GetValueOrDefault()));

                        if (value != 0)
                        {
                            var seriesBrush = ser.Brush;
                            if (ser.AllowApplyBrush)
                            {
                                seriesBrush = ser.Core.GetSeriesBrush(colorIndex, colorCount);
                                seriesBrush = ser.ProcessSeriesBrushes(colorIndexCondition, seriesBrush);
                            }

                            var borderColor = ser.BorderColor;
                            if (ser.AllowApplyBorderColor)
                            {
                                borderColor = (Color)ser.Core.GetSeriesBorderColor(colorIndex, colorCount);
                            }

                            var radiusDt2 = ser.Width != 0 ? radiusDt - ser.Width: radiusDt - step;

                            var doughnutElementGeom = RenderDoughnutElement(context, pointCenter, radiusDt, radiusDt2, borderColor, seriesBrush,
                                angle, arcWidth, Math.Abs(value.GetValueOrDefault()), currentIndex, ser, false, geom, new TimeSpan(beginTime.Ticks / ser.Values.Length * colorIndex));

                            if (doughnutElementGeom != null)
                            {
                                if (ser.Core.Interaction != null)
                                    doughnutElementGeom.Interaction = new StiSeriesInteractionData(geom.Area, ser, currentIndex);

                                geom.CreateChildGeoms();
                                geom.ChildGeoms.Add(doughnutElementGeom);
                            }

                            angle += arcWidth;
                        }
                        colorIndexCondition++;
                        colorIndex++;
                        index++;
                        currentIndex++;
                    }
                }
                radiusDt -= ser.Width != 0 ? ser.Width + step * 0.2f : step * 1.2f;
            }
            #endregion

            if (index == 0)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(new StiDoughnutEmptySeriesElementGeom(new RectangleF(pointCenter.X - radius, pointCenter.Y - radius, 2 * radius, 2 * radius)));
                return;
            }

            #region Draw Series Labels
            colorIndex = 0;
            radiusDt = radius;
            foreach (IStiDoughnutSeries ser in seriesArray)
            {
                index = 0;
                angle = ser.StartAngle;
                float gradPerValue = GetGradPerValue(ser);
                foreach (double? value in ser.Values)
                {
                    float percentPerValue = GetPercentPerValue(ser);
                    float arcWidth = (float)(gradPerValue * Math.Abs(value.GetValueOrDefault()));

                    IStiSeriesLabels seriesLabels = null;
                    if (ser.ShowSeriesLabels == StiShowSeriesLabels.FromChart)
                        seriesLabels = this.Series.Chart.SeriesLabels;
                    if (ser.ShowSeriesLabels == StiShowSeriesLabels.FromSeries)
                        seriesLabels = currentSeries.SeriesLabels;

                    if (this.Series.Chart != null && seriesLabels != null && seriesLabels.Visible)
                    {
                        var labels = seriesLabels as IStiPieSeriesLabels;

                        if (labels != null && labels.Visible && (labels.Step == 0 || (index % labels.Step == 0)))
                        {
                            RectangleF measureRect;
                            var radiusDt2 = ser.Width != 0 ? radiusDt - ser.Width : radiusDt - step;

                            var seriesLabelsGeom =
                                ((StiPieSeriesLabelsCoreXF)labels.Core).RenderLabel(ser, context, pointCenter, radiusDt, radiusDt2,
                                    angle + arcWidth / 2,
                                    index, Math.Abs(value.GetValueOrDefault()), value,
                                    GetArgumentText(ser, index), 
                                    GetTag(index), false,
                                    colorIndex, colorCount, percentPerValue, out measureRect, false, 0);

                            if (seriesLabelsGeom != null)
                            {
                                seriesLabelsGeom.Duration = duration;
                                seriesLabelsGeom.BeginTime = new TimeSpan(beginTime.Ticks / ser.Values.Length * colorIndex);

                                geom.CreateChildGeoms();
                                geom.ChildGeoms.Add(seriesLabelsGeom);
                            }
                        }
                    }
                    colorIndex++;

                    angle += arcWidth;
                    index++;
                }
                radiusDt -= ser.Width != 0 ? ser.Width + step * 0.2f : step * 1.2f;
            }
            #endregion

            var seriesLabel = GetPieSeriesLabels();

            if (seriesLabel != null && seriesLabel.PreventIntersection)
            {
                CheckIntersectionLabels(geom);
            }
        }

        private double? CheckNonZerovalue(IStiSeries[] seriesArray, out int nonZeroValuesCount, out int firstNonZeroValueIndex, out IStiSeries firstNonZeroSeries, bool isForValueFrom = false)
        {
            int nonZeroValuesCountTemp = 0;
            int firstNonZeroValueIndexTemp = 0;
            int firstNonZeroValueIndexTemp1 = 0;
            double? firstNonZeroValue = 0;
            IStiSeries firstNonZeroSeriesTemp = null;

            foreach (IStiSeries ser in seriesArray)
            {
                var values = isForValueFrom ? ser.ValuesStart : ser.Values;

                foreach (double? value in values)
                {
                    if (!(value == 0 || value == null || double.IsNaN(value.Value)))
                    {
                        nonZeroValuesCountTemp++;
                        firstNonZeroValue = value;
                        firstNonZeroSeriesTemp = ser;
                        if (nonZeroValuesCountTemp == 1)
                            firstNonZeroValueIndexTemp1 = firstNonZeroValueIndexTemp;
                    }
                    firstNonZeroValueIndexTemp++;
                }
            }

            firstNonZeroValueIndex = firstNonZeroValueIndexTemp1;
            nonZeroValuesCount = nonZeroValuesCountTemp;
            firstNonZeroSeries = firstNonZeroSeriesTemp;

            return firstNonZeroValue;
        }

        private float GetGradPerValue(IStiSeries series)
        {
            double totals = 0;
            foreach (double? value in series.Values)
            {
                totals += Math.Abs(value.GetValueOrDefault());
            }
            return (float)(360 / totals);
        }

        private float GetPercentPerValue(IStiSeries series)
        {
            double totals = 0;
            foreach (double? value in series.Values)
            {
                totals += Math.Abs(value.GetValueOrDefault());
            }
            return (float)(1 / totals * 100);
        }

        protected override string GetArgumentText(IStiSeries series, int index)
        {
            if (series.Arguments.Length > index)
            {
                return series.Arguments[index].ToString();
            }
            return string.Empty;
        }
        #endregion        

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "Doughnut");
            }
        }
        #endregion

        public StiDoughnutSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
	}
}
