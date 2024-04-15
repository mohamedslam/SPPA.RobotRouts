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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using System.Linq;
using System.Collections.Generic;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiBoxAndWhiskerSeriesCoreXF :
        StiSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);
            
            if (Series.AllowApplyStyle && Series is IStiBoxAndWhiskerSeries boxAndWhiskerSeries)
            {
                boxAndWhiskerSeries.Brush = style.Core.GetColumnBrush(color);
                boxAndWhiskerSeries.BorderThickness = style.Core.SeriesBorderThickness;
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            var boxAndWhiskerArea = geom.Area as IStiBoxAndWhiskerArea;
            float singleX = (float)boxAndWhiskerArea.XAxis.Info.Dpi;
            float singleY = (float)boxAndWhiskerArea.YAxis.Info.Dpi;
            int posXAdjustment = boxAndWhiskerArea.XAxis.StartFromZero ? 0 : -1;

            float posY = boxAndWhiskerArea.AxisCore.GetDividerY();

            var indexSeries = 1;
            foreach (IStiBoxAndWhiskerSeries boxAnsWhiskerSeries in series)
            {
                var pointLabelList = new List<Tuple<double, PointF>>();
                
                var seriesBrush = boxAnsWhiskerSeries.Brush;
                if (boxAnsWhiskerSeries.AllowApplyBrush)
                {
                    seriesBrush = boxAnsWhiskerSeries.Core.GetSeriesBrush(indexSeries - 1, series.Length);
                    seriesBrush = boxAnsWhiskerSeries.ProcessSeriesBrushes(indexSeries - 1, seriesBrush);
                }

                if (boxAnsWhiskerSeries.Values.Length > 0)
                {
                    var minValue = boxAnsWhiskerSeries.Values.Min().GetValueOrDefault();
                    var maxValue = boxAnsWhiskerSeries.Values.Max().GetValueOrDefault();
                    var median = GetMedian(boxAnsWhiskerSeries.Values);
                    var firstQuartile = GetFirstQuartile(boxAnsWhiskerSeries.Values);
                    var thirdQuartile = GetThirdQuartile(boxAnsWhiskerSeries.Values);

                    var positionX = (indexSeries + posXAdjustment) * singleX;
                    var maxValuePosition = Math.Abs(posY - (float)maxValue * singleY);
                    var minValuePosition = Math.Abs(posY - (float)minValue * singleY);
                    var medianPosition = Math.Abs(posY - (float)median * singleY);
                    var firstQuartilePosition = Math.Abs(posY - (float)firstQuartile * singleY);
                    var thirdQuartilePosition = Math.Abs(posY - (float)thirdQuartile * singleY);

                    var clientRect = new RectangleF(positionX - singleX / 4, minValuePosition, singleX / 2, maxValuePosition - minValuePosition);
                    var borderColor = StiBrush.ToColor(StiBrush.Dark(seriesBrush, 30));

                    pointLabelList.Add(Tuple.Create(minValue, new PointF(positionX, minValuePosition)));
                    pointLabelList.Add(Tuple.Create(maxValue, new PointF(positionX, maxValuePosition)));
                    pointLabelList.Add(Tuple.Create(median, new PointF(positionX, medianPosition)));
                    pointLabelList.Add(Tuple.Create(firstQuartile, new PointF(positionX, firstQuartilePosition)));
                    pointLabelList.Add(Tuple.Create(thirdQuartile, new PointF(positionX, thirdQuartilePosition)));

                    #region Calculate ShowInnerPoints
                    var valueList = new List<double>();
                    if (boxAnsWhiskerSeries.ShowInnerPoints)
                    {
                        foreach (var value in boxAnsWhiskerSeries.Values)
                        {
                            var currentValue = value.GetValueOrDefault();
                            if (currentValue != minValue && currentValue != maxValue && !valueList.Contains(currentValue))
                            {
                                var point = Math.Abs(posY - currentValue * singleY);
                                valueList.Add(point);

                                pointLabelList.Add(Tuple.Create(currentValue, new PointF(positionX, (float)point)));
                            }
                        }
                    }
                    #endregion

                    #region Calculate ShowMeanMarkers
                    double? meanValuePoint = null;
                    if (boxAnsWhiskerSeries.ShowMeanMarkers)
                    {
                        var sum = boxAnsWhiskerSeries.Values.Sum();
                        var meanValue = sum / boxAnsWhiskerSeries.Values.Length;
                        meanValuePoint = Math.Abs(posY - meanValue.GetValueOrDefault() * singleY);

                        pointLabelList.Add(Tuple.Create(meanValue.GetValueOrDefault(), new PointF(positionX, (float)meanValuePoint.GetValueOrDefault())));
                    }
                    #endregion

                    var boxAndWhiskerGeom = new StiBoxAndWhiskerSeriesElementGeom(geom, boxAnsWhiskerSeries, positionX,
                        minValuePosition, maxValuePosition, firstQuartilePosition, thirdQuartilePosition, medianPosition, valueList.ToArray(), meanValuePoint, clientRect,
                        seriesBrush, borderColor, new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks / series.Length * indexSeries));
                                        

                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(boxAndWhiskerGeom);

                    #region Render Series Labels
                    var labels = boxAnsWhiskerSeries.Core.GetSeriesLabels();

                    if (labels != null && labels.Visible)
                    {
                        foreach (var pointLabel in pointLabelList)
                        {
                            var animation = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, TimeSpan.Zero);
                            var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(boxAnsWhiskerSeries, context,
                                                pointLabel.Item2, pointLabel.Item2,
                                                indexSeries, pointLabel.Item1, pointLabel.Item1, pointLabel.Item1.ToString(),
                                                null, 0, 0, 1, rect, animation);

                            if (seriesLabelsGeom != null)
                            {
                                geom.CreateChildGeoms();
                                geom.ChildGeoms.Add(seriesLabelsGeom);
                            }
                        }
                    }
                    #endregion
                }

                indexSeries++;
            }
        }

        private double?[] GetFirstValues(double?[] values)
        {
            var medianIndices = GetMedianIndices(values);

            var firstValues = medianIndices.Length == 1
                    ? new double?[medianIndices[0] + 1]
                    : new double?[medianIndices[1]];

            Array.Copy(values, 0, firstValues, 0, firstValues.Length);

            return firstValues;
        }

        private double?[] GetSecondValues(double?[] values)
        {
            var medianIndices = GetMedianIndices(values);

            var secondValues = medianIndices.Length == 1
                    ? new double?[medianIndices[0] + 1]
                    : new double?[medianIndices[1]];

            Array.Copy(values, secondValues.Length -1, secondValues, 0, secondValues.Length);

            return secondValues;
        }

        private double GetThirdQuartile(double?[] values)
        {
            Array.Sort(values);
            var secondValues = GetSecondValues(values);

            return GetMedian(secondValues);
        }

        private double GetFirstQuartile(double?[] values)
        {
            Array.Sort(values);
            var firstValues = GetFirstValues(values);

            return GetMedian(firstValues);
        }

        private double GetMedian(double?[] values)
        {
            Array.Sort(values);

            var indices = GetMedianIndices(values);
            var median = 0d;
            foreach (var index in indices)
            {
                median += values[index].GetValueOrDefault();
            }

            return median / indices.Length;
        }

        private int[] GetMedianIndices(double?[] values)
        {
            var array = (values.Length % 2) == 0 
                ? new int[2]
                : new int[1];

            if (array.Length == 1)
            {
                var indexMedian = (int)Math.Ceiling((double)values.Length / 2) - 1;
                array[0] = indexMedian;
            }
            else
            {
                var indexMedianLeft = values.Length / 2 - 1;
                array[0] = indexMedianLeft;
                array[1] = indexMedianLeft + 1;
            }

            return array;
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
                return StiLocalization.Get("Chart", "BoxAndWhisker");
            }
        }
        #endregion

        public StiBoxAndWhiskerSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
