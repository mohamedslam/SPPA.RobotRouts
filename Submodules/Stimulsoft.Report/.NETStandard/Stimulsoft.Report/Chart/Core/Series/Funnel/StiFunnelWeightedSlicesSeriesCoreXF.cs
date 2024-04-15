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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Chart.Geoms.Series.Funnel;

namespace Stimulsoft.Report.Chart
{
    public class StiFunnelWeightedSlicesSeriesCoreXF : StiSeriesCoreXF
    {
        #region Fields
        private IStiFunnelSeriesLabels labels;
        #endregion

        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            var funnelSeries = this.Series as IStiFunnelWeightedSlicesSeries;

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

            var listLabelsGeom = new List<StiSeriesLabelsGeom>();

            #region Render FunnelElements
            double[] allTrueValues = GetAllTrueValues(funnelSeriesArray);
            double[] allValues = GetValues(funnelSeriesArray);
            float singleValueHeight = rect.Height * 0.9f / GetSumValues(allTrueValues);

            int colorIndex = 0;
            int globalIndex = 0;

            float centerAxis;
            if (this.labels is StiOutsideLeftFunnelLabels)
                centerAxis = rect.Width / 2 + rect.X;
            else
                centerAxis = rect.Width / 2;

            this.labels = this.Series.Chart.SeriesLabels as IStiFunnelSeriesLabels;


            foreach (IStiSeries ser in seriesArray)
            {
                var funnelSeries = ser as IStiFunnelWeightedSlicesSeries;

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
                        borderColor = (Color)funnelSeries.Core.GetSeriesBorderColor(globalIndex, colorCount);
                    }
                    var funnelElementGeom = RenderFunnelElement(borderColor, seriesBrush, funnelSeries.Values[firstNonZeroValueIndex].GetValueOrDefault(), allValues, globalIndex, firstNonZeroValueIndex, ser, geom, rect, singleValueHeight, new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks));

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
                    for (int index = 0; index < ser.Values.Length; index++)
                    {
                        double value = ser.Values[index].GetValueOrDefault();

                        if (!funnelSeries.ShowZeros && value == 0)
                        {
                            globalIndex++;
                            continue;
                        }

                        var seriesBrush = funnelSeries.Brush;
                        if (funnelSeries.AllowApplyBrush)
                        {
                            seriesBrush = funnelSeries.Core.GetSeriesBrush(globalIndex, colorCount);
                            seriesBrush = funnelSeries.ProcessSeriesBrushes(globalIndex, seriesBrush);
                        }

                        var borderColor = funnelSeries.BorderColor;
                        if (funnelSeries.AllowApplyBorderColor)
                        {
                            borderColor = (Color)funnelSeries.Core.GetSeriesBorderColor(globalIndex, colorCount);
                        }

                        var funnelElementGeom = RenderFunnelElement(borderColor, seriesBrush, value, allValues, colorIndex, index, ser, geom, rect, singleValueHeight, new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks / ser.Values.Length * index));

                        if (funnelElementGeom != null)
                        {
                            if (ser.Core.Interaction != null)
                                funnelElementGeom.Interaction = new StiSeriesInteractionData(geom.Area, ser, index);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(funnelElementGeom);
                        }

                        #region Render Series Labels
                        if (labels != null && labels.Visible)
                        {
                            double valueNext = (index == allTrueValues.Length - 1) ? value : allTrueValues[index + 1];

                            if (labels.Step == 0 || (index % labels.Step == 0))
                            {
                                var seriesLabelsGeom =
                                    ((StiFunnelSeriesLabelsCoreXF)labels.Core).RenderLabel(ser, context, globalIndex, value, valueNext,
                                    GetArgumentText(ser, index), ser.Core.GetTag(index), globalIndex, colorCount, rect, singleValueHeight, 0, centerAxis, out rect);

                                if (seriesLabelsGeom != null)
                                    listLabelsGeom.Add(seriesLabelsGeom);
                            }
                        }
                        #endregion

                        colorIndex++;
                        globalIndex++;
                    }
                }
            }
            #endregion

            #region Add Labels on Area
            foreach (var label in listLabelsGeom)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(label);

                label.ClientRectangle = CheckLabelsRect(labels, geom, label.ClientRectangle);
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

            StiStyleCoreXF styleCore = this.Series.Chart.Style != null ? this.Series.Chart.Style.Core : new StiStyleCoreXF29();

            Color color = styleCore.GetColorByIndex(colorIndex, colorCount, SeriesColors);

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

        private double[] GetValues(IStiFunnelSeries[] funnelSeries)
        {
            var values = new List<double>();

            foreach (var series in funnelSeries)
            {
                foreach (double? value in series.Values)
                {
                    if (!series.ShowZeros && value.GetValueOrDefault() == 0)
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
            float singleValueHeight = rect.Height * 0.9f / GetSumValues(values);

            for (int index = 0; index < values.Length; index++)
            {
                double value = values[index];

                var path = MeasureFunnelElementCore(index, rect, singleValueHeight, values);

                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(new StiFunnelEmptySeriesElementGeom(rect, path));
            }            
        }

        private List<StiSegmentGeom> GetPathFunnelEmpty(RectangleF rect)
        {
            var path = new List<StiSegmentGeom>();

            float indent = rect.Height * 0.05f;

            float center;
            if (this.labels is StiOutsideLeftFunnelLabels)
                center = rect.Width / 2 + rect.X;
            else
                center = rect.Width / 2;
            
            var pointLeftTop = new PointF(center - rect.Width * 0.9f / 2, indent);
            var pointRightTop = new PointF(center + rect.Width * 0.9f / 2, indent);
            var pointLeftInflection = new PointF(center - rect.Width * 0.1f / 2, rect.Height + indent - rect.Height / 5);
            var pointRightInflection = new PointF(center + rect.Width * 0.1f / 2, rect.Height + indent - rect.Height / 5);
            var pointRightBottom = new PointF(center + rect.Width * 0.1f / 2, rect.Height + indent);
            var pointLeftBottom = new PointF(center - rect.Width * 0.1f / 2, rect.Height + indent);

            var points = new PointF[] { pointLeftTop, pointRightTop, pointRightInflection, pointRightBottom, pointLeftBottom, pointLeftInflection, pointLeftTop };

            path.Add(new StiLinesSegmentGeom(points));
            path.Add(new StiCloseFigureSegmentGeom());
            return path;
        }

        private StiFunnelSeriesElementGeom RenderFunnelElement(Color borderColor, StiBrush brush, double value, double[] values, int globalIndex, int index,
            IStiSeries currentSeries, StiAreaGeom geom, RectangleF rect, float singleValueHeight, TimeSpan beginTime)
        {
            var path = MeasureFunnelElementCore(globalIndex, rect, singleValueHeight, values);

            return new StiFunnelSeriesElementGeom(geom, value, index, currentSeries, rect, brush, borderColor, path, beginTime);
        }

        private float GetSumValues(double[] values)
        {
            double sumValues = 0;
            foreach (double value in values)
            {
                sumValues += Math.Abs(value);
            }
            return (float)sumValues;
        }

        private float GetSumLastValues(int indexCurrent, double[] values)
        {
            float sumLastValues = 0;
            for (int index = 0; index < indexCurrent; index++)
            {
                if (index >= values.Length) break;
                sumLastValues += (float)Math.Abs(values[index]);
            }
            return sumLastValues;
        }

        private List<StiSegmentGeom> MeasureFunnelElementCore(int index, RectangleF rect, float singleValueHeight, double[] values)
        {
            var path = new List<StiSegmentGeom>();
            float indent = rect.Height * 0.05f;

            float center;
            if (this.labels is StiOutsideLeftFunnelLabels)
                center = rect.Width / 2 + rect.X;
            else
                center = rect.Width / 2;

            PointF pointLeftTop;
            PointF pointRightTop;
            PointF pointLeftInflection = new PointF();
            PointF pointRightInflection = new PointF();
            PointF pointRightBottom;
            PointF pointLeftBottom;

            bool isInflection = false;

            float sumLastValues = GetSumLastValues(index, values);

            bool isInflectionValue = false;

            float valueHeight = rect.Height - singleValueHeight * sumLastValues;
            if (valueHeight < rect.Height / 5)
            {
                valueHeight = rect.Height / 5;
                isInflectionValue = true;
            }

            float valueWidth = rect.Width * 0.9f * valueHeight / rect.Height;

            pointLeftTop = new PointF(center - valueWidth / 2, singleValueHeight * sumLastValues + indent);
            pointRightTop = new PointF(center + valueWidth / 2, singleValueHeight * sumLastValues + indent);

            sumLastValues = GetSumLastValues(index + 1, values);

            valueHeight = rect.Height - singleValueHeight * sumLastValues;

            if (valueHeight < rect.Height / 5)
            {
                valueHeight = rect.Height / 5;
                if (!isInflectionValue)
                {
                    valueWidth = rect.Width * 0.9f * valueHeight / rect.Height;
                    pointLeftInflection = new PointF(center - valueWidth / 2, rect.Height + indent - valueHeight);
                    pointRightInflection = new PointF(center + valueWidth / 2, rect.Height + indent - valueHeight);
                    isInflection = true;
                }
            }

            valueWidth = rect.Width * 0.9f * valueHeight / rect.Height;

            pointRightBottom = new PointF(center + valueWidth / 2, singleValueHeight * sumLastValues + indent);
            pointLeftBottom = new PointF(center - valueWidth / 2, singleValueHeight * sumLastValues + indent);


            PointF[] points;
            if (isInflection)
                points = new PointF[] { pointLeftTop, pointRightTop, pointRightInflection, pointRightBottom, pointLeftBottom, pointLeftInflection, pointLeftTop };
            else
                points = new PointF[] { pointLeftTop, pointRightTop, pointRightBottom, pointLeftBottom, pointLeftTop };

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
                return StiLocalization.Get("Chart", "FunnelWeightedSlices");
            }
        }
        #endregion

        public StiFunnelWeightedSlicesSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
