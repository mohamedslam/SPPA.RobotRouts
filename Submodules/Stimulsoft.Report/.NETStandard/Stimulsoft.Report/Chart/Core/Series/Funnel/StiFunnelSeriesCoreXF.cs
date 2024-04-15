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
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Report.Chart.Geoms.Series.Funnel;

namespace Stimulsoft.Report.Chart
{
    public class StiFunnelSeriesCoreXF : StiSeriesCoreXF
    {
        #region Fields
        private IStiFunnelSeriesLabels labels;
        #endregion

        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            var funnelSeries = this.Series as IStiFunnelSeries;

            if (funnelSeries.AllowApplyStyle)
            {
                funnelSeries.Brush = style.Core.GetColumnBrush(color);
                funnelSeries.BorderColor = style.Core.GetColumnBorder(color);
                funnelSeries.BorderThickness = style.Core.SeriesBorderThickness;
            }
        }
        #endregion

        #region Methods        
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesArray)
        {
            if (seriesArray == null || seriesArray.Length == 0 || this.Series.Chart == null)
            {
                RenderFunnelEmpty(geom, rect);
                return;
            }

            var funnelSeriesArray = seriesArray.Cast<IStiFunnelSeries>().ToArray();

            double[] allValues = GetAllValues(funnelSeriesArray);
            double[] allTrueValues = GetAllTrueValues(funnelSeriesArray);

            #region Calculate Colors
            int colorCount = 0;
            foreach (IStiFunnelSeries ser in funnelSeriesArray)
            {
                if (ser.Values != null)
                {
                    colorCount += ser.Values.Length;
                }
            }
            if (colorCount == 0)
            {
                RenderFunnelEmpty(geom, rect);
                return;
            }
            #endregion

            #region Render FunnelElements

            float singleValueHeight = GetSingleValueHeight(colorCount, rect);
            float singleValueWidth = GetSingleValueWidth(allValues, rect);

            #region Render Series Labels - First Pass
            this.labels = this.Series.Chart.SeriesLabels as IStiFunnelSeriesLabels;

            RectangleF measureRect = rect;
            RectangleF measureRectTemp = rect;

            foreach (var funnelSeries in funnelSeriesArray)
            {
                if (labels != null && labels.Visible)
                {
                    for (int pointIndex = 0; pointIndex < funnelSeries.Values.Length; pointIndex++)
                    {
                        if (funnelSeries.Values.Length > pointIndex)
                        {
                            double value = funnelSeries.Values[pointIndex].GetValueOrDefault();
                            double valueNext = (pointIndex == funnelSeries.Values.Length - 1) ? value : funnelSeries.Values[pointIndex + 1].GetValueOrDefault();

                            if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                            {
                                var seriesLabelsGeom =
                                    ((StiFunnelSeriesLabelsCoreXF)labels.Core).RenderLabel(funnelSeries, context, pointIndex, value, valueNext,
                                    GetArgumentText(funnelSeries, pointIndex), funnelSeries.Core.GetTag(pointIndex), 0, colorCount, rect, singleValueHeight, singleValueWidth, 1, out measureRect);
                                measureRectTemp = measureRect.Width < measureRectTemp.Width ? measureRect : measureRectTemp;
                            }
                        }
                    }
                    measureRect = measureRectTemp;
                }
            }
            #endregion

            singleValueHeight = GetSingleValueHeight(colorCount, measureRect);
            singleValueWidth = GetSingleValueWidth(allValues, measureRect);

            int globalIndex = 0;
            int colorIndex = 0;
            float centerAxis;

            if (this.labels is StiOutsideLeftFunnelLabels)
                centerAxis = measureRect.Width / 2 + measureRect.X;
            else
                centerAxis = measureRect.Width / 2;

            var mainMeasureRect = measureRect;

            foreach (var funnelSeries in funnelSeriesArray)
            {
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
                    var seriesBrush = funnelSeries.Brush;
                    if (funnelSeries.AllowApplyBrush)
                    {
                        seriesBrush = funnelSeries.Core.GetSeriesBrush(firstNonZeroValueIndex, funnelSeries.Values.Length);
                        seriesBrush = funnelSeries.ProcessSeriesBrushes(firstNonZeroValueIndex, seriesBrush);
                    }

                    var borderColor = funnelSeries.BorderColor;
                    if (funnelSeries.AllowApplyBorderColor)
                    {
                        borderColor = (Color)funnelSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);
                    }
                    var funnelElementGeom = RenderFunnelElement(borderColor, seriesBrush, funnelSeries.Values[firstNonZeroValueIndex].GetValueOrDefault(), 0, colorIndex, firstNonZeroValueIndex, funnelSeries, geom, mainMeasureRect, singleValueHeight, singleValueWidth, new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks));

                    if (funnelElementGeom != null)
                    {
                        if (funnelSeries.Core.Interaction != null)
                            funnelElementGeom.Interaction = new StiSeriesInteractionData(geom.Area, funnelSeries, firstNonZeroValueIndex);

                        geom.CreateChildGeoms();
                        geom.ChildGeoms.Add(funnelElementGeom);
                    }
                }
                #endregion

                else
                {
                    for (int index = 0; index < funnelSeries.Values.Length; index++)
                    {
                        double? value = GetCurrentValue(funnelSeries, globalIndex, allTrueValues);
                        if (value == 0 && !funnelSeries.ShowZeros)
                        {
                            globalIndex++;
                            continue;
                        }
                        double? valueNext = GetNextCurrentValue(funnelSeries, globalIndex, allTrueValues);

                        var seriesBrush = funnelSeries.Brush;
                        if (funnelSeries.AllowApplyBrush)
                        {
                            seriesBrush = funnelSeries.Core.GetSeriesBrush(globalIndex, colorCount);
                            seriesBrush = funnelSeries.ProcessSeriesBrushes(globalIndex, seriesBrush);
                        }

                        var borderColor = funnelSeries.BorderColor;
                        if (funnelSeries.AllowApplyBorderColor)
                        {
                            borderColor = (Color)funnelSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);
                        }
                        var funnelElementGeom = RenderFunnelElement(borderColor, seriesBrush, value.GetValueOrDefault(), valueNext.GetValueOrDefault(), colorIndex, index, funnelSeries, geom, mainMeasureRect, singleValueHeight, singleValueWidth, new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks / allValues.Length * index));

                        if (funnelElementGeom != null)
                        {
                            if (funnelSeries.Core.Interaction != null)
                                funnelElementGeom.Interaction = new StiSeriesInteractionData(geom.Area, funnelSeries, index);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(funnelElementGeom);
                        }

                        if (labels != null && labels.Visible && (labels.Step == 0 || (index % labels.Step == 0)))
                        {
                            var seriesLabelsGeom =
                                ((StiFunnelSeriesLabelsCoreXF)labels.Core).RenderLabel(funnelSeries, context, colorIndex, value.GetValueOrDefault(), valueNext.GetValueOrDefault(),
                                GetArgumentText(funnelSeries, index), funnelSeries.Core.GetTag(index), colorIndex, colorCount, rect, singleValueHeight, singleValueWidth, centerAxis, out measureRect);

                            if (seriesLabelsGeom != null)
                            {
                                geom.CreateChildGeoms();
                                geom.ChildGeoms.Add(seriesLabelsGeom);

                                seriesLabelsGeom.ClientRectangle = CheckLabelsRect(labels, geom, seriesLabelsGeom.ClientRectangle);
                            }
                        }

                        colorIndex++;
                        globalIndex++;
                    }
                }
            }
            #endregion
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

        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            if (this.Series.Chart == null || this.Series.Chart.Area == null) return null;

            var styleCore = this.Series.Chart.Style != null ? this.Series.Chart.Style.Core : new StiStyleCoreXF29();
            var color = styleCore.GetColorByIndex(colorIndex, colorCount, SeriesColors);

            return styleCore.GetColumnBorder(color);
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            if (this.Series.Chart == null || this.Series.Chart.Area == null) return null;

            var styleCore = this.Series.Chart.Style != null ? this.Series.Chart.Style.Core : new StiStyleCoreXF29();
            var color = styleCore.GetColorByIndex(colorIndex, colorCount, SeriesColors);
            var seriesBrush = styleCore.GetColumnBrush(color);

            return seriesBrush;
        }

        private double GetCurrentValue(IStiFunnelSeries funnelSeries, int index, double[] values)
        {
            return values[index];
        }

        private double? GetNextCurrentValue(IStiFunnelSeries funnelSeries, int index, double[] values)
        {
            if (index != values.Length - 1)
            {
                var value = values[index + 1];
                if (value == 0 && !funnelSeries.ShowZeros)
                {
                    while (value == 0 && index < values.Length - 2)
                    {
                        index++;
                        value = values[index + 1];
                    }
                }
                return value;
            }

            return values[index];
        }

        private double[] GetAllValues(IStiFunnelSeries[] funnelSeries)
        {
            var values = new List<double>();

            foreach (var series in funnelSeries)
            {
                foreach (double? value in series.Values)
                {
                    if (value.GetValueOrDefault() == 0 && !series.ShowZeros)
                        continue;
                    values.Add(value.GetValueOrDefault());
                }
            }

            return values.ToArray();
        }

        private double[] GetAllTrueValues(IStiFunnelSeries[] funnelSeries)
        {
            var values = new List<double>();

            foreach (var series in funnelSeries)
            {
                foreach (double? value in series.Values)
                {
                    values.Add(value.GetValueOrDefault());
                }
            }

            return values.ToArray();
        }

        private string GetArgumentText(IStiSeries series, int index)
        {
            if (series.Arguments.Length > index && series.Arguments[index] != null)
            {
                return series.Arguments[index].ToString();
            }
            return string.Empty;
        }

        private void RenderFunnelEmpty(StiAreaGeom geom, RectangleF rect)
        {
            var values = new double[] { 3, 2, 1 };
            float singleValueHeight = rect.Height * 0.9f / values.Length;
            var singleValueWidth = rect.Width * 0.9f / values.Length;

            for (int index = 0; index < values.Length; index++)
            {
                double value = values[index];
                double? valueNext = index != values.Length - 1 ? values[index + 1]: values[index];

                var path = MeasureFunnelElementCore(value, valueNext.GetValueOrDefault(), index, rect, singleValueHeight, singleValueWidth);

                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(new StiFunnelEmptySeriesElementGeom(rect, path));
            }
        }

        private StiFunnelSeriesElementGeom RenderFunnelElement(Color borderColor, StiBrush brush, double value, double valueNext, int globalIndex, int index,
            IStiSeries currentSeries, StiAreaGeom geom, RectangleF rect, float singleValueHeight, float singleValueWidth, TimeSpan beginTime)
        {
            var path = MeasureFunnelElementCore(value, valueNext, globalIndex, rect, singleValueHeight, singleValueWidth);

            return new StiFunnelSeriesElementGeom(geom, value, index, currentSeries, rect, brush, borderColor, path, beginTime);
        }

        private float GetSingleValueHeight(int count, RectangleF rect)
        {
            return rect.Height * 0.9f / count;
        }

        private float GetSingleValueWidth(double[] values, RectangleF rect)
        {
            float maxValue = float.MinValue;
            foreach (double? value in values)
            {
                maxValue = maxValue > value.GetValueOrDefault() ? maxValue : (float)value.GetValueOrDefault();
            }            

            return rect.Width * 0.9f / maxValue;
        }

        private List<StiSegmentGeom> MeasureFunnelElementCore(double value, double valueNext, int index,
            RectangleF rect, float singleValueHeight, float singleValueWidth)
        {
            var path = new List<StiSegmentGeom>();
            float indent = rect.Height * 0.05f;

            float center;
            if (this.labels is StiOutsideLeftFunnelLabels)
                center = rect.Width / 2 + rect.X;
            else
                center = rect.Width / 2;

            var pointLeftTop = new PointF(center - (float)value / 2 * singleValueWidth, singleValueHeight * index + indent);
            var pointRightTop = new PointF(center + (float)value / 2 * singleValueWidth, singleValueHeight * index + indent);

            var pointRightBottom = new PointF(center + (float)valueNext / 2 * singleValueWidth, singleValueHeight * (index + 1) + indent);
            var pointLeftBottom = new PointF(center - (float)valueNext / 2 * singleValueWidth, singleValueHeight * (index + 1) + indent);

            var points = new PointF[] { pointLeftTop, pointRightTop, pointRightBottom, pointLeftBottom, pointLeftTop };

            path.Add(new StiLinesSegmentGeom(points));
            path.Add(new StiCloseFigureSegmentGeom());

            return path;
        }         
        #endregion

        #region Properties.Localization
         //<summary>
         //Gets a service name.
         //</summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "Funnel");
            }
        }
        #endregion 

        public StiFunnelSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
