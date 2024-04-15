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
using System.Linq;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Report.Chart.Geoms.Series.Pie;

namespace Stimulsoft.Report.Chart
{
    public class StiPieSeriesCoreXF : StiSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            var pieSeries = this.Series as IStiPieSeries;

            if (pieSeries.AllowApplyStyle)
            {
                pieSeries.Lighting = style.Core.SeriesLighting;
                pieSeries.BorderThickness = style.Core.SeriesBorderThickness;

                if (pieSeries.AllowApplyBrush)
                    pieSeries.Brush = style.Core.GetColumnBrush(color);

                if (pieSeries.AllowApplyBorderColor)
                    pieSeries.BorderColor = style.Core.GetColumnBorder(color);
            }
        }
        #endregion

        #region Methods
        private StiBrush CorrectBrush(StiBrush brush)
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
            return brush;
        }

        private StiPieSeriesElementGeom RenderPieElement(StiContext context, PointF center, float radius,
            Color borderColor, StiBrush brush, float start, float angle, double? value, int index,
            IStiPieSeries currentSeries, float distance, StiAreaGeom geom, TimeSpan beginTime)
        {
            List<StiSegmentGeom> path;
            List<StiSegmentGeom> pathLight;
            var rectPie = RectangleF.Empty;

            brush = CorrectBrush(brush);

            var clientRectangle = MeasurePieElementCore(context, center, radius, start, angle,
                currentSeries, distance, out path, out pathLight, out rectPie);

            if (clientRectangle.IsEmpty) return null;

            return new StiPieSeriesElementGeom(geom, value.GetValueOrDefault(), index, currentSeries, rectPie, path, pathLight, borderColor, brush,
                start, start + angle, radius);
        }

        private void RenderPieElementShadow(StiContext context, PointF center, float radius,
            StiBrush brush, float start, float angle, IStiPieSeries currentSeries, float distance)
        {
            if (((StiChart)this.Series.Chart).IsAnimation) return;

            List<StiSegmentGeom> path;
            List<StiSegmentGeom> pathLight;
            var rectPie = RectangleF.Empty;

            brush = CorrectBrush(brush);

            var clientRectangle = MeasurePieElementCore(context, center, radius, start, angle,
                currentSeries, distance, out path, out pathLight, out rectPie);

            if (clientRectangle.IsEmpty) return;

            context.PushSmoothingModeToAntiAlias();

            #region Render Pie Segment
            context.FillPath(brush, path, rectPie, null);
            #endregion

            #region Render lights
            if (pathLight != null)
            {
                var brLight = new StiGradientBrush(Color.FromArgb(100, Color.White), Color.FromArgb(50, Color.Black), 45);
                context.FillPath(brLight, pathLight, rectPie, null);
            }
            #endregion

            context.PopSmoothingMode();
        }


        private RectangleF MeasurePieElement(StiContext context, PointF center, float radius,
            float start, float angle,
            IStiPieSeries currentSeries, float distance)
        {
            List<StiSegmentGeom> path;
            List<StiSegmentGeom> pathLight;
            var rectPie = RectangleF.Empty;

            return MeasurePieElementCore(context, center, radius,
                start, angle,
                currentSeries, distance,
                out path, out pathLight, out rectPie);
        }

        private RectangleF MeasurePieElementCore(StiContext context, PointF center, float radius,
            float start, float angle, IStiPieSeries currentSeries, float distance,
            out List<StiSegmentGeom> path, out List<StiSegmentGeom> pathLight, out RectangleF rectPie)
        {
            path = new List<StiSegmentGeom>();
            pathLight = null;
            rectPie = RectangleF.Empty;

            if (angle == 0 || float.IsNaN(angle)) return RectangleF.Empty;
            if (distance > 0) center = GetPoint(center, distance * context.Options.Zoom, start + angle / 2);

            rectPie = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            if (rectPie.Width <= 0 && rectPie.Height <= 0) return RectangleF.Empty;

            path.Add(new StiPieSegmentGeom(new RectangleF(rectPie.X, rectPie.Y, rectPie.Width, rectPie.Height), start, angle, null));

            #region Render light
            if (currentSeries.Lighting && !Color.Transparent.Equals(currentSeries.BorderColor))
            {
                float lightWidth = 0;
                pathLight = new List<StiSegmentGeom>();
                lightWidth = radius * 0.02f;

                pathLight.Add(new StiArcSegmentGeom(rectPie, start, angle));
                pathLight.Add(new StiLineSegmentGeom(GetPoint(center, radius - lightWidth, start + angle), GetPoint(center, radius - lightWidth, start + angle)));
                pathLight.Add(new StiArcSegmentGeom(new RectangleF(rectPie.X + lightWidth, rectPie.Y + lightWidth,
                    rectPie.Width - lightWidth * 2, rectPie.Height - lightWidth * 2), start + angle, -angle));
                pathLight.Add(new StiLineSegmentGeom(GetPoint(center, radius - lightWidth, start), GetPoint(center, radius - lightWidth, start)));
            }
            #endregion

            return context.GetPathBounds(path);
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

        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesArray)
        {

            float radius = GetRadius(context, rect);
            var pointCenter = GetPointCenter(rect);

            if (seriesArray == null || seriesArray.Length == 0 || this.Series.Chart == null)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(new StiPieEmptySeriesElementGeom(new RectangleF(pointCenter.X - radius, pointCenter.Y - radius, 2 * radius, 2 * radius)));
                return;
            }

            var duration = StiChartHelper.GlobalDurationElement;
            var beginTime = StiChartHelper.GlobalBeginTimeElement;

            var chart = this.Series.Chart as StiChart;

            int colorCount = GetColorCount(seriesArray);

            #region Check for zeros
            int nonZeroValuesCount = 0;
            int firstNonZeroValueIndex = -1;
            IStiSeries firstNonZeroSeries = null;

            CheckNonZerovalue(seriesArray, out nonZeroValuesCount, out firstNonZeroValueIndex, out firstNonZeroSeries);
            #endregion

            float gradPerValue = GetGradPerValue(seriesArray);
            float percentPerValue = GetPercentPerValue(seriesArray);
            var currentSeries = seriesArray[0] as IStiPieSeries;

            var seriesLabels = ((StiPieSeriesCoreXF)currentSeries.Core).GetPieSeriesLabels();
            var isAnimationChangingValues = chart.IsAnimationChangingValues;// && (seriesLabels?.FormatService == null || seriesLabels?.FormatService is StiGeneralFormatService);

            #region Measure Pie
            var angle = currentSeries.StartAngle;
            var bounds = new RectangleF(0, 0, rect.Width, rect.Height);

            foreach (IStiSeries ser in seriesArray)
            {
                if (nonZeroValuesCount > 1)
                {
                    var pieSeries = ser as IStiPieSeries;
                    int index = 0;

                    foreach (double? value in ser.Values)
                    {
                        var arcWidth = (float)(gradPerValue * Math.Abs(value.GetValueOrDefault()));

                        var measureRect = MeasurePieElement(context, pointCenter, radius,
                            angle, arcWidth, pieSeries, GetPieDistance(pieSeries, index));

                        if (value != 0) bounds = RectangleF.Union(bounds, measureRect);
                        angle += arcWidth;
                        index++;
                    }
                }
            }

            var maximumDeltaX = Math.Max(-bounds.Left, bounds.Right - rect.Width);
            var maximumDeltaY = Math.Max(-bounds.Top, bounds.Bottom - rect.Height);
            var maximumDelta = Math.Max(maximumDeltaX, maximumDeltaY);

            radius -= maximumDelta * 1.2f;
            #endregion

            #region Measure Series Labels
            var labelsRect = new RectangleF[colorCount];
            var deltaY = new float[colorCount];
            var quarterCounts = new int[4];
            Array.Clear(quarterCounts, 0, quarterCounts.Length);

            angle = currentSeries.StartAngle;
            bounds = new RectangleF(10 * context.Options.Zoom, 10 * context.Options.Zoom,
                rect.Width - 20 * context.Options.Zoom, rect.Height - 20 * context.Options.Zoom);
            var rectPie2 = bounds;

            #region Measure Labels - First Pass
            int labelIndex = 0;
            foreach (IStiSeries ser in seriesArray)
            {
                IStiPieSeries pieSer = ser as IStiPieSeries;
                int index = 0;

                foreach (double? value in ser.Values)
                {
                    var arcWidth = (float)(gradPerValue * Math.Abs(value.GetValueOrDefault()));
                    var labels = ((StiPieSeriesCoreXF)pieSer.Core).GetPieSeriesLabels();

                    if (labels != null && labels.Visible)
                    {
                        float labelRadius = radius;
                        if (GetPieDistance(pieSer, index) > 0) labelRadius += pieSer.Distance * context.Options.Zoom;
                        float currAngle = angle + arcWidth / 2;

                        RectangleF measureRect;

                        ((StiPieSeriesLabelsCoreXF)labels.Core).RenderLabel(pieSer, context, pointCenter, labelRadius, 0, currAngle,
                            index, Math.Abs(value.GetValueOrDefault()), value, GetArgumentText(ser, index),
                            pieSer.Core.GetTag(index), true, index, colorCount, percentPerValue, out measureRect, false, 0);

                        if (!measureRect.IsEmpty) bounds = RectangleF.Union(bounds, measureRect);

                        if (value != 0 || labels.ShowZeros)
                        {
                            labelsRect[labelIndex] = measureRect;
                            deltaY[labelIndex] = measureRect.Y;

                            #region Calculate Quarters Count
                            if (currAngle >= 0 && currAngle <= 90) quarterCounts[0]++;
                            if (currAngle > 90 && currAngle <= 180) quarterCounts[1]++;
                            if (currAngle > 180 && currAngle <= 270) quarterCounts[2]++;
                            if (currAngle > 270 && currAngle <= 360) quarterCounts[3]++;
                            #endregion
                        }
                    }

                    angle += arcWidth;
                    index++;
                    if (value != 0 || this.Series.Chart.SeriesLabels.ShowZeros) labelIndex++;
                }
            }
            #endregion

            #region Recalc Radius
            if (currentSeries.Diameter == 0)
            {
                float dist = 0;
                dist = Math.Min(dist, bounds.Left - rectPie2.Left);
                dist = Math.Min(dist, rectPie2.Right - bounds.Right);
                dist = Math.Min(dist, bounds.Top - rectPie2.Top);
                dist = Math.Min(dist, rectPie2.Bottom - bounds.Bottom);

                radius += dist;
            }
            #endregion

            #region Measure Labels - Second Pass (For New Radius)
            labelIndex = 0;
            foreach (IStiSeries ser in seriesArray)
            {
                IStiPieSeries pieSer = ser as IStiPieSeries;
                int index = 0;

                foreach (double? value in ser.Values)
                {
                    var arcWidth = (float)(gradPerValue * value.GetValueOrDefault());
                    var labels = ((StiPieSeriesCoreXF)pieSer.Core).GetPieSeriesLabels();

                    if (labels != null && labels.Visible)
                    {
                        float labelRadius = radius;
                        if (GetPieDistance(pieSer, index) > 0) labelRadius += pieSer.Distance * context.Options.Zoom;
                        float currAngle = angle + arcWidth / 2;

                        RectangleF measureRect;
                        ((StiPieSeriesLabelsCoreXF)labels.Core).RenderLabel(this.Series, context, pointCenter, labelRadius, 0, currAngle,
                            index, value, value, GetArgumentText(ser, index),
                            pieSer.Core.GetTag(index), true, index, colorCount, percentPerValue, out measureRect, false, 0);

                        if (value != 0 || labels.ShowZeros)
                        {
                            labelsRect[labelIndex] = measureRect;
                            deltaY[labelIndex] = measureRect.Y;
                        }
                    }

                    angle += arcWidth;
                    index++;
                    if (value != 0 || this.Series.Chart.SeriesLabels.ShowZeros) labelIndex++;
                }
            }
            #endregion

            #region Sort Labels Position
            if (this.Series.Chart != null && this.Series.Chart.SeriesLabels != null)
            {
                var twoColumnsLabels = this.Series.Chart.SeriesLabels as IStiTwoColumnsPieLabels;

                if (twoColumnsLabels != null && twoColumnsLabels.PreventIntersection)
                {
                    float calcedHeight = 0;

                    #region 3...6 Hours Quarter
                    if (quarterCounts[0] > 0)
                    {
                        var destinationArray = new RectangleF[quarterCounts[0]];
                        Array.Copy(labelsRect, 0, destinationArray, 0, quarterCounts[0]);
                        if (IsIntersectionLabels(destinationArray))
                        {
                            calcedHeight = bounds.Height / 2 / quarterCounts[0];
                            labelsRect[0].Y = bounds.Y + bounds.Height / 2 + calcedHeight / 2 - labelsRect[0].Height / 2;
                            for (int i = 1; i < quarterCounts[0]; i++)
                            {
                                labelsRect[i].Y = labelsRect[i - 1].Y + calcedHeight;
                            }
                        }
                    }
                    #endregion

                    #region 6...9 Hours Quarter
                    int startIndex = quarterCounts[0];

                    if (quarterCounts[1] > 0)
                    {
                        var destinationArray = new RectangleF[quarterCounts[1]];
                        Array.Copy(labelsRect, startIndex, destinationArray, 0, quarterCounts[1]);
                        if (IsIntersectionLabels(destinationArray))
                        {
                            calcedHeight = bounds.Height / 2 / quarterCounts[1];
                            labelsRect[startIndex].Y = bounds.Y + bounds.Height - calcedHeight / 2 - labelsRect[startIndex].Height / 2;
                            for (int i = startIndex + 1; i < startIndex + quarterCounts[1]; i++)
                            {
                                labelsRect[i].Y = labelsRect[i - 1].Y - calcedHeight;
                            }
                        }
                    }
                    #endregion

                    #region 9..12 Hours Quarter
                    startIndex += quarterCounts[1];

                    if (quarterCounts[2] > 0)
                    {
                        var destinationArray = new RectangleF[quarterCounts[2]];
                        Array.Copy(labelsRect, startIndex, destinationArray, 0, quarterCounts[2]);
                        if (IsIntersectionLabels(destinationArray))
                        {
                            calcedHeight = bounds.Height / 2 / quarterCounts[2];
                            labelsRect[startIndex].Y = bounds.Y + bounds.Height / 2 - calcedHeight / 2 - labelsRect[startIndex].Height / 2;
                            for (int i = startIndex + 1; i < startIndex + quarterCounts[2]; i++)
                            {
                                labelsRect[i].Y = labelsRect[i - 1].Y - calcedHeight;
                            }
                        }
                    }
                    #endregion

                    #region 0..3 Hours Quarter
                    startIndex += quarterCounts[2];

                    if (quarterCounts[3] > 0)
                    {
                        var destinationArray = new RectangleF[quarterCounts[3]];
                        Array.Copy(labelsRect, startIndex, destinationArray, 0, quarterCounts[3]);
                        if (IsIntersectionLabels(destinationArray))
                        {
                            calcedHeight = bounds.Height / 2 / quarterCounts[3];
                            labelsRect[startIndex].Y = bounds.Y + calcedHeight / 2 - labelsRect[startIndex].Height / 2;
                            for (int i = startIndex + 1; i < startIndex + quarterCounts[3]; i++)
                            {
                                labelsRect[i].Y = labelsRect[i - 1].Y + calcedHeight;
                            }
                        }
                    }
                    #endregion

                    // Recalc Labels Delta
                    for (int i = 0; i < labelIndex; i++)
                    {
                        deltaY[i] = labelsRect[i].Y - deltaY[i];
                    }
                }
            }
            #endregion
            #endregion

            if (radius <= 5) return;

            #region Render Shadow
            if (currentSeries.ShowShadow)
            {
                angle = currentSeries.StartAngle;
                foreach (IStiSeries ser in seriesArray)
                {
                    if (nonZeroValuesCount == 1)
                    {
                        var rectPie = new RectangleF(pointCenter.X - radius, pointCenter.Y - radius, radius * 2, radius * 2);

                        var shadowRect = new RectangleF(0, 0, radius * 2, radius * 2);

                        var shadowContext = context.CreateShadowGraphics();
                        if (shadowContext != null)
                            shadowContext.FillEllipse(Color.FromArgb(100, Color.Black), shadowRect, null);

                        var shadowGeom = new StiPieSeriesShadowElementGeom(currentSeries, rectPie, radius * 0.01f + 2 * context.Options.Zoom, shadowContext, duration, beginTime);

                        geom.CreateChildGeoms();
                        geom.ChildGeoms.Add(shadowGeom);
                        break;
                    }
                    else
                    {
                        IStiPieSeries pieSeries = ser as IStiPieSeries;
                        int index = 0;

                        foreach (double? value in ser.Values)
                        {
                            var arcWidth = (float)(gradPerValue * Math.Abs(value.GetValueOrDefault()));

                            var shadowPointCenter = pointCenter;
                            var shadowBrush = new StiSolidBrush(Color.FromArgb(100, Color.Black));

                            var shadowContext = context.CreateShadowGraphics();

                            var shadowRect = rect;
                            shadowRect.X = 0;
                            shadowRect.Y = 0;

                            if (chart.IsAnimation)
                            {
                                shadowRect.X = pointCenter.X - radius;
                                shadowRect.Y = pointCenter.Y - radius;

                                shadowRect.Height = radius * 2;
                                shadowRect.Width = radius * 2;
                            }

                            RenderPieElementShadow(shadowContext, shadowPointCenter, radius, shadowBrush, angle, arcWidth, pieSeries, GetPieDistance(pieSeries, index));

                            var shadowGeom = new StiPieSeriesShadowElementGeom(currentSeries, shadowRect, radius * 0.01f + 2 * context.Options.Zoom, shadowContext, duration, beginTime);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(shadowGeom);

                            angle += arcWidth;
                            index++;
                        }
                    }
                }
            }
            #endregion

            #region Render PieElements
            var listPieElementsGeom = GetPieElementGeoms(context, geom, radius, rect, seriesArray);
            if (listPieElementsGeom.Count < 1)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(new StiPieEmptySeriesElementGeom(new RectangleF(pointCenter.X - radius, pointCenter.Y - radius, 2 * radius, 2 * radius)));
                return;
            }

            var listPieElementsFromGeom = GetPieElementGeoms(context, geom, radius, rect, seriesArray, isAnimationChangingValues);

            for (int indexGeom = 0; indexGeom < listPieElementsGeom.Count; indexGeom++)
            {
                var elementGeom = listPieElementsGeom[indexGeom];

                if (elementGeom is StiPieSeriesFullElementGeom pieSeriesFullElementGeom)
                {
                    if (pieSeriesFullElementGeom.Series.Core.Interaction != null)
                        pieSeriesFullElementGeom.Interaction = new StiSeriesInteractionData(geom.Area, pieSeriesFullElementGeom.Series, pieSeriesFullElementGeom.Index);

                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(pieSeriesFullElementGeom);
                }
                else if (elementGeom is StiPieSeriesElementGeom pieElementGeom)
                {
                    pieElementGeom.Count = listPieElementsGeom.Count;

                    if (pieElementGeom.Series.Core.Interaction != null)
                        pieElementGeom.Interaction = new StiSeriesInteractionData(geom.Area, pieElementGeom.Series, pieElementGeom.Index);

                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(pieElementGeom);
                }


            }
            #endregion

            #region Render Series Labels
            angle = currentSeries.StartAngle;

            var listPieLabelsGeom = GetPieLabelGeoms(context, radius, rect, seriesArray, deltaY);

            if (isAnimationChangingValues && seriesLabels != null && seriesLabels.ValueType == StiSeriesLabelsValueType.Value)
            {
                var listPieLabelsFromGeom = GetPieLabelGeoms(context, radius, rect, seriesArray, deltaY, isAnimationChangingValues);

                var labelRadius = radius / 1.75f;

                var labelClientRect = new RectangleF()
                {
                    X = pointCenter.X - labelRadius,
                    Y = pointCenter.Y - labelRadius,
                    Width = 2 * labelRadius,
                    Height = 2 * labelRadius
                };
                for (int indexGeom = 0; indexGeom < listPieElementsGeom.Count; indexGeom++)
                {
                    /*var labelGeom = listPieLabelsGeom.Count > indexGeom
                        ? listPieLabelsGeom[indexGeom] as StiCenterPieLabelsGeom
                        : null;

                    var labelGeomFrom = listPieLabelsFromGeom.Count > indexGeom
                        ? listPieLabelsFromGeom[indexGeom] as StiCenterPieLabelsGeom
                        : null;

                    if (labelGeom != null && labelGeomFrom != null)
                    {
                        var pointIndex = labelGeom.Index;

                        float angleLabelFrom = 0;
                        float angleLabel = 0;

                        var pieElementFrom = listPieElementsFromGeom.Count > pointIndex ? listPieElementsFromGeom[pointIndex] as StiPieSeriesElementGeom : null;
                        var pieElement = listPieElementsGeom.Count > pointIndex ? listPieElementsGeom[pointIndex] as StiPieSeriesElementGeom : null;

                        if (pieElement != null && pieElementFrom != null)
                        {
                            angleLabelFrom = (pieElementFrom.StartAngle + pieElementFrom.EndAngle) / 2;
                            angleLabel = (pieElement.StartAngle + pieElement.EndAngle) / 2;

                            var labelAnimation = new StiPieLabelAnimation(labelGeomFrom.Value, labelGeom.Value, angleLabelFrom, angleLabel, labelClientRect, labelGeomFrom.LabelRect, labelGeom.LabelRect, StiChartHelper.GlobalBeginTimeElement, TimeSpan.Zero);
                            labelGeom.Animation = labelAnimation;
                        }
                    }*/

                    var cellGeom = listPieLabelsGeom.Count > indexGeom
                        ? listPieLabelsGeom[indexGeom] as StiCellGeom
                        : null;

                    if (cellGeom != null)
                    {
                        geom.CreateChildGeoms();
                        geom.ChildGeoms.Add(cellGeom);
                    }
                }
            }
            else
            {
                foreach (StiCellGeom labelGeom in listPieLabelsGeom)
                {
                    if (isAnimationChangingValues && labelGeom is StiCenterPieLabelsGeom)
                        ((StiCenterPieLabelsGeom)labelGeom).Animation = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, StiChartHelper.GlobalBeginTimeElement);

                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(labelGeom);
                }
            }
            #endregion

            var seriesLabel = GetPieSeriesLabels();

            if (seriesLabel != null && seriesLabel.PreventIntersection)
            {
                if (seriesLabel is IStiTwoColumnsPieLabels)
                    CheckIntersectionTwoColumnsLabels(geom, rectPie2);
                else if (seriesLabel is IStiOutsidePieLabels)
                    this.CheckIntersectionOutLabels(geom);
                else
                    CheckIntersectionLabels(geom);
            }
        }

        protected int GetColorCount(IStiSeries[] seriesArray, bool isForValueFrom = false)
        {
            int colorCount = 0;
            foreach (IStiSeries ser in seriesArray)
            {
                if (isForValueFrom && ser.ValuesStart != null) colorCount += ser.ValuesStart.Length;

                if (!isForValueFrom && ser.Values != null) colorCount += ser.Values.Length;
            }
            return colorCount;
        }

        private bool IsIntersectionLabels(RectangleF[] labels)
        {
            for (int index = 0; index < labels.Length; index++)
            {
                for (int indexSecond = 0; indexSecond < labels.Length; indexSecond++)
                {
                    if (index == indexSecond) continue;

                    if (labels[index].IntersectsWith(labels[indexSecond]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private List<StiGeom> GetPieLabelGeoms(StiContext context, float radius, RectangleF rect, IStiSeries[] seriesArray, float[] deltaY, bool isForValueFrom = false)
        {
            var resault = new List<StiGeom>();

            int colorIndex = 0;
            int labelIndex = 0;

            var currentSeries = seriesArray[0] as IStiPieSeries;

            float gradPerValue = GetGradPerValue(seriesArray, isForValueFrom);
            float percentPerValue = GetPercentPerValue(seriesArray, isForValueFrom);

            var pointCenter = GetPointCenter(rect);
            var angle = currentSeries.StartAngle;
            var colorCount = GetColorCount(seriesArray, isForValueFrom);

            foreach (IStiSeries ser in seriesArray)
            {
                var pieSeries = ser as IStiPieSeries;
                int index = 0;

                var values = isForValueFrom ? ser.ValuesStart : ser.Values;

                foreach (double? value in values)
                {
                    float arcWidth = (float)(gradPerValue * Math.Abs(value.GetValueOrDefault()));

                    var seriesLabels = ((StiPieSeriesCoreXF)pieSeries.Core).GetPieSeriesLabels();

                    if (seriesLabels != null && seriesLabels.Visible)
                    {
                        var labels = seriesLabels as IStiPieSeriesLabels;
                        var twoColumnsLabels = seriesLabels as IStiTwoColumnsPieLabels;

                        if (labels != null)
                        {
                            float labelRadius = radius;
                            if (GetPieDistance(pieSeries, index) > 0) labelRadius += pieSeries.Distance * context.Options.Zoom;
                            float currAngle = angle + arcWidth / 2;
                            float currDeltaY = 0;

                            if (twoColumnsLabels != null && twoColumnsLabels.PreventIntersection) currDeltaY += deltaY[labelIndex];

                            if (labels.Step == 0 || (index % labels.Step == 0))
                            {
                                RectangleF measureRect;
                                var seriesLabelsGeom =
                                    ((StiPieSeriesLabelsCoreXF)labels.Core).RenderLabel(pieSeries, context, pointCenter, labelRadius, 0, currAngle, index, Math.Abs(value.GetValueOrDefault()), value,
                                        GetArgumentText(ser, index),
                                        pieSeries.Core.GetTag(index), false,
                                        colorIndex, colorCount, percentPerValue, out measureRect, false, currDeltaY);

                                if (seriesLabelsGeom != null)
                                {
                                    resault.Add(seriesLabelsGeom);
                                }

                                #region ShowValue
                                if (labels is IStiOutsidePieLabels && ((IStiOutsidePieLabels)labels).ShowValue)
                                {
                                    seriesLabelsGeom =
                                        ((StiOutsidePieLabelsCoreXF)labels.Core).RenderLabel(pieSeries, context, pointCenter, labelRadius, 0, currAngle, index, Math.Abs(value.GetValueOrDefault()), value,
                                            GetArgumentText(ser, index),
                                            pieSeries.Core.GetTag(index), false,
                                            colorIndex, colorCount, percentPerValue, out measureRect, true, 0);

                                    if (seriesLabelsGeom != null)
                                    {
                                        resault.Add(seriesLabelsGeom);
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    colorIndex++;

                    angle += arcWidth;
                    index++;

                    if (value != 0 || (seriesLabels != null && seriesLabels.ShowZeros))
                        labelIndex++;
                }
            }

            return resault;
        }

        public IStiPieSeriesLabels GetPieSeriesLabels()
        {
            if (this.Series.ShowSeriesLabels == StiShowSeriesLabels.FromChart)
                return this.Series.Chart.SeriesLabels as IStiPieSeriesLabels;

            if (this.Series.ShowSeriesLabels == StiShowSeriesLabels.FromSeries)
                return this.Series.SeriesLabels as IStiPieSeriesLabels;

            return null;
        }

        private List<StiCellGeom> GetPieElementGeoms(StiContext context, StiAreaGeom geom, float radius, RectangleF rect, IStiSeries[] seriesArray, bool isForValueFrom = false)
        {
            var resaultList = new List<StiCellGeom>();

            var currentSeries = seriesArray[0] as IStiPieSeries;

            float gradPerValue = GetGradPerValue(seriesArray, isForValueFrom);
            float percentPerValue = GetPercentPerValue(seriesArray, isForValueFrom);

            var pointCenter = GetPointCenter(rect);
            var angle = currentSeries.StartAngle;
            var colorCount = GetColorCount(seriesArray, isForValueFrom);

            #region Check for zeros
            int nonZeroValuesCount = 0;
            int firstNonZeroValueIndex = -1;
            IStiSeries firstNonZeroSeries = null;
            double? firstNonZeroValue = CheckNonZerovalue(seriesArray, out nonZeroValuesCount, out firstNonZeroValueIndex, out firstNonZeroSeries, isForValueFrom);
            #endregion

            int colorIndex = 0;
            foreach (IStiPieSeries ser in seriesArray)
            {
                #region nonZeroValuesCount == 0
                if (nonZeroValuesCount == 0)
                {
                    break;
                }
                #endregion

                #region nonZeroValuesCount == 1
                /*else if (nonZeroValuesCount == 1)
                {
                    var pieSeries = firstNonZeroSeries as IStiPieSeries;
                    var seriesBrush = pieSeries.Brush;
                    if (pieSeries.AllowApplyBrush)
                    {
                        seriesBrush = pieSeries.Core.GetSeriesBrush(firstNonZeroValueIndex, colorCount);
                        seriesBrush = pieSeries.ProcessSeriesBrushes(firstNonZeroValueIndex, seriesBrush);
                    }

                    var rect2 = new RectangleF(pointCenter.X - radius, pointCenter.Y - radius, radius * 2, radius * 2);
                    var borderColor = pieSeries.BorderColor;

                    if (pieSeries.AllowApplyBorderColor)
                    {
                        borderColor = (Color)pieSeries.Core.GetSeriesBorderColor(firstNonZeroValueIndex, colorCount);
                    }

                    var pieGeom = new StiPieSeriesFullElementGeom(geom, firstNonZeroValue.GetValueOrDefault(), firstNonZeroValueIndex, pieSeries, rect2, seriesBrush, borderColor);
                    resaultList.Add(pieGeom);

                    break;
                }*/
                #endregion

                else
                {
                    var values = isForValueFrom ? ser.ValuesStart : ser.Values;

                    for (var index = 0; index < values.Length; index++)
                    {
                        double? value = values[index];

                        if (value != 0)
                        {
                            float arcWidth = (float)(gradPerValue * Math.Abs(value.GetValueOrDefault()));

                            var seriesBrush = ser.Brush;
                            if (ser.AllowApplyBrush)
                            {
                                seriesBrush = ser.Core.GetSeriesBrush(colorIndex, colorCount);
                                seriesBrush = ser.ProcessSeriesBrushes(index, seriesBrush);
                            }

                            var borderColor = ser.BorderColor;
                            if (ser.AllowApplyBorderColor)
                                borderColor = (Color)ser.Core.GetSeriesBorderColor(colorIndex, colorCount);

                            var ticks = ser.Values.Length == 0 ? 0 : StiChartHelper.GlobalBeginTimeElement.Ticks / ser.Values.Length * index;
                            var beginTime = new TimeSpan(ticks);

                            var pieElementGeom = RenderPieElement(context, pointCenter, radius, borderColor, seriesBrush,
                               angle, arcWidth, Math.Abs(value.GetValueOrDefault()), index, ser, GetPieDistance(ser, index), geom, beginTime);

                            if (pieElementGeom != null)
                            {
                                resaultList.Add(pieElementGeom);
                            }
                            angle += arcWidth;
                        }

                        colorIndex++;
                    }
                }
            }

            return resaultList;
        }

        protected void CheckIntersectionOutLabels(StiAreaGeom geom)
        {
            var childGeoms = geom.ChildGeoms;
            var labelGeoms = new List<StiOutsidePieLabelsGeom>();
            if (childGeoms != null)
            {
                foreach (StiCellGeom cellGeom in childGeoms)
                {
                    if (cellGeom is StiOutsidePieLabelsGeom)
                        labelGeoms.Add((StiOutsidePieLabelsGeom)cellGeom);
                }
            }

            int count = labelGeoms.Count;

            bool intersection = true;
            int indexCheck = 0;

            while (intersection && indexCheck < 29)
            {
                indexCheck++;

                for (int index1 = 0; index1 < count; index1++)
                {
                    for (int index2 = 0; index2 < count; index2++)
                    {
                        if (index2 == index1) continue;

                        if (labelGeoms[index1].LabelRect.IntersectsWith(labelGeoms[index2].LabelRect))
                        {
                            var rect1 = labelGeoms[index1].LabelRect;
                            var rect2 = labelGeoms[index2].LabelRect;

                            if (rect1.IntersectsWith(rect2))
                            {
                                float overlay = rect1.Height - Math.Abs(labelGeoms[index2].LabelRect.Y - labelGeoms[index1].LabelRect.Y) + 2;
                                if (rect1.Y > rect2.Y)
                                {
                                    rect1.Y += overlay / 2;
                                    rect2.Y -= overlay / 2;
                                }
                                else
                                {
                                    rect1.Y -= overlay / 2;
                                    rect2.Y += overlay / 2;
                                }

                                if (rect1.Y < 0)
                                    rect1.Y = 0;

                                if (rect2.Y < 0)
                                    rect2.Y = 0;

                                if ((rect1.Y + rect1.Height) > geom.ClientRectangle.Height)
                                    rect1.Y = geom.ClientRectangle.Height - rect1.Height;

                                if ((rect2.Y + overlay / 2 + rect2.Height) > geom.ClientRectangle.Height)
                                    rect2.Y = geom.ClientRectangle.Height - rect2.Height;

                                labelGeoms[index1].LabelRect = rect1;
                                labelGeoms[index2].LabelRect = rect2;
                            }
                        }
                    }
                }
            }
        }

        private void CheckIntersectionTwoColumnsLabels(StiAreaGeom geom, RectangleF rect)
        {
            var childGeoms = geom.ChildGeoms;
            if (childGeoms == null) return;

            var centerPoint = GetPointCenter(rect);

            var labelLeftGeoms = new List<StiTwoColumnsPieLabelsGeom>();
            var labelRightGeoms = new List<StiTwoColumnsPieLabelsGeom>();

            foreach (StiCellGeom cellGeom in childGeoms)
            {
                if (cellGeom is StiTwoColumnsPieLabelsGeom)
                {
                    if (cellGeom.ClientRectangle.X < centerPoint.X)
                    {
                        labelLeftGeoms.Add((StiTwoColumnsPieLabelsGeom)cellGeom);
                    }
                    else
                    {
                        labelRightGeoms.Add((StiTwoColumnsPieLabelsGeom)cellGeom);
                    }
                }
            }

            if (IsIntersectionLabels(labelLeftGeoms.Select(x => x.ClientRectangle).ToArray()))
                CheckLabelPosition(labelLeftGeoms, rect);
            if (IsIntersectionLabels(labelRightGeoms.Select(x => x.ClientRectangle).ToArray()))
                CheckLabelPosition(labelRightGeoms, rect);
        }

        protected override void CheckIntersectionLabels(StiAreaGeom geom)
        {
            var childGeoms = geom.ChildGeoms;
            var labelGeoms = new List<StiCenterPieLabelsGeom>();
            if (childGeoms != null)
            {
                foreach (StiCellGeom cellGeom in childGeoms)
                {
                    if (cellGeom is StiCenterPieLabelsGeom)
                        labelGeoms.Add((StiCenterPieLabelsGeom)cellGeom);
                }
            }

            int count = labelGeoms.Count;

            bool intersection = true;
            int indexCheck = 0;

            while (intersection && indexCheck < 29)
            {
                indexCheck++;

                for (int index1 = 0; index1 < count; index1++)
                {
                    for (int index2 = 0; index2 < count; index2++)
                    {
                        if (index2 == index1) continue;

                        if (labelGeoms[index1].ClientRectangle.IntersectsWith(labelGeoms[index2].ClientRectangle))
                        {
                            var rect1 = labelGeoms[index1].LabelRect;
                            var rect2 = labelGeoms[index2].LabelRect;

                            if (rect1.IntersectsWith(rect2))
                            {
                                float overlay = rect1.Height - Math.Abs(labelGeoms[index2].ClientRectangle.Y - labelGeoms[index1].ClientRectangle.Y) + 2;
                                if (rect1.Y > rect2.Y)
                                {
                                    rect1.Y += overlay / 2;
                                    rect2.Y -= overlay / 2;
                                }
                                else
                                {
                                    rect1.Y -= overlay / 2;
                                    rect2.Y += overlay / 2;
                                }

                                if (rect1.Y < 0)
                                    rect1.Y = 0;

                                if (rect2.Y < 0)
                                    rect2.Y = 0;

                                if ((rect1.Y + rect1.Height) > geom.ClientRectangle.Height)
                                    rect1.Y = geom.ClientRectangle.Height - rect1.Height;

                                if ((rect2.Y + overlay / 2 + rect2.Height) > geom.ClientRectangle.Height)
                                    rect2.Y = geom.ClientRectangle.Height - rect2.Height;

                                labelGeoms[index1].LabelRect = rect1;
                                labelGeoms[index2].LabelRect = rect2;
                            }
                        }
                    }
                }
            }
        }

        private void CheckLabelPosition(List<StiTwoColumnsPieLabelsGeom> labels, RectangleF rect)
        {
            var freeHeight = rect.Height;

            labels = labels.OrderBy(x => x.ClientRectangle.Y).ToList();

            foreach (StiTwoColumnsPieLabelsGeom label in labels)
            {
                freeHeight -= label.ClientRectangle.Height;
            }

            var stepFreeHeight = freeHeight / (labels.Count + 1);

            var y0 = rect.Y;

            foreach (StiTwoColumnsPieLabelsGeom label in labels)
            {
                label.ClientRectangle = new RectangleF(label.ClientRectangle.X, y0 + stepFreeHeight, label.ClientRectangle.Width, label.ClientRectangle.Height);

                label.EndPoint = new PointF(label.EndPoint.X, label.ClientRectangle.Y + label.ClientRectangle.Height / 2);

                y0 = label.ClientRectangle.Y + label.ClientRectangle.Height;
            }
        }

        protected float GetGradPerValue(IStiSeries[] series, bool isForValueFrom = false)
        {
            double totals = 0;
            int count = 0;
            foreach (IStiSeries sr in series)
            {
                var values = isForValueFrom ? sr.ValuesStart : sr.Values;

                foreach (double? value in values)
                {
                    totals += Math.Abs(value.GetValueOrDefault());
                    count++;
                }
            }
            if (count == 0) count = 1;
            if (totals > 0)
                return (float)(360 / totals);
            else
                return 360f / count;
        }

        internal float GetPercentPerValue(IStiSeries[] series, bool isForValueFrom = false)
        {
            double totals = 0;
            foreach (IStiSeries sr in series)
            {
                var values = isForValueFrom ? sr.ValuesStart : sr.Values;

                foreach (double? value in values)
                {
                    totals += Math.Abs(value.GetValueOrDefault());
                }
            }
            return (float)(1 / totals * 100);
        }

        protected PointF GetPointCenter(RectangleF rect)
        {
            return new PointF(rect.Width / 2, rect.Height / 2);
        }

        protected float GetRadius(StiContext context, RectangleF rect)
        {
            IStiPieSeries pieSeries = this.Series as IStiPieSeries;
            if (pieSeries.Diameter > 0) return pieSeries.Diameter / 2 * context.Options.Zoom;
            return Math.Min(rect.Width / 2, rect.Height / 2) * 0.95f;
        }

        protected PointF GetPoint(PointF centerPie, float radius, float angle)
        {
            float angleRad = (float)(Math.PI * angle / 180);
            return new PointF(
                centerPie.X + (float)Math.Cos(angleRad) * radius,
                centerPie.Y + (float)Math.Sin(angleRad) * radius);
        }

        protected virtual string GetArgumentText(IStiSeries series, int index)
        {
            if (series.Arguments.Length > index && series.Arguments[index] != null)
            {
                return series.Arguments[index].ToString();
            }
            return string.Empty;
        }

        public float GetPieDistance(int pieIndex)
        {
            return GetPieDistance(this.Series as IStiPieSeries, pieIndex);
        }

        public float GetPieDistance(IStiPieSeries series, int pieIndex)
        {
            if (series.Distance == 0) return 0;
            if (series.CutPieListValues.Length == 0) return series.Distance;

            foreach (double index in series.CutPieListValues)
            {
                if (index == pieIndex + 1) return series.Distance;
            }

            return 0;
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            StiBrush brush = base.GetSeriesBrush(colorIndex, colorCount);
            if (brush == null) return ((IStiPieSeries)this.Series).Brush;
            return brush;
        }

        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            object color = base.GetSeriesBorderColor(colorIndex, colorCount);
            if (color == null) return ((IStiPieSeries)this.Series).BorderColor;
            return color;
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
                return StiLocalization.Get("Chart", "Pie");
            }
        }
        #endregion

        public StiPieSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
