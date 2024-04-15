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
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiBubbleSeriesCoreXF : StiScatterSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            if (Series.AllowApplyStyle && Series is IStiBubbleSeries bubbleSeries)
            {
                bubbleSeries.Brush = style.Core.GetColumnBrush(color);
                bubbleSeries.BorderColor = style.Core.GetColumnBorder(color);
                bubbleSeries.BorderThickness = style.Core.SeriesBorderThickness;
            }
        }
        #endregion

        #region Methods
        protected override void RenderLines(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo)
        {
            
        }

        protected void RenderBubbles(StiContext context, StiAreaGeom geom, IStiBubbleSeries series, PointF?[] points, double []weights)
        {
            double max = 0;

            #region Search Max Weight
            int index = 0;
            foreach (IStiSeries serie in series.Chart.Series)
            {
                var bubbleSeries = serie as IStiBubbleSeries;
                if (bubbleSeries == null) continue;

                foreach (double weight in bubbleSeries.Weights)
                {
                    if (index == 0)
                        max = weight;
                    else
                        max = Math.Max(weight, max);

                    index++;
                }
            }
            #endregion

            var axisArea = geom.Area as IStiAxisArea;
            var step = Math.Min(axisArea.XAxis.Info.Step, axisArea.YAxis.Info.Step) * 0.9;

            var dpi = step / max;
            
            var time = StiChartHelper.GlobalBeginTimeElement;

            index = 0;
            foreach (double weight in weights)
            {
                var point = points[index];

                if (point != null && weight > 0)
                {
                    float size = (float)(weight * dpi);

                    var rect = new RectangleF(point.Value.X - size / 2, point.Value.Y - size / 2, size, size);

                    var seriesBrush = GetSeriesBrush(index, points.Length);
                    var seriesBorderColor = (Color)series.Core.GetSeriesBorderColor(index, points.Length);

                    var seriesColumnGeom =
                        new StiBubbleSeriesElementGeom(geom, weight, index, seriesBrush, seriesBorderColor, series, rect, new TimeSpan(time.Ticks / ((StiAxisAreaCoreXF)axisArea.Core).ValuesCount * index));

                    if (series.Core.Interaction != null)
                        seriesColumnGeom.Interaction = new StiSeriesInteractionData(axisArea, series, index);

                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(seriesColumnGeom);
                }

                index++;
            }
        }

        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesArray)
        {
            if (seriesArray == null || seriesArray.Length == 0 || this.Series.Chart == null) return;

            var dotArea = geom.Area as IStiScatterArea;

            double posY = 0;

            var pointLists = new List<List<PointF?>>();
            var weightLists = new List<List<double>>();

            for (int seriesIndex = 0; seriesIndex < seriesArray.Length; seriesIndex++)
            {
                var currentSeries = seriesArray[seriesIndex] as IStiBubbleSeries;

                int pointsCount = Math.Min(currentSeries.Values.Length, currentSeries.Arguments.Length);

                var points = new List<PointF?>();
                var weights = new List<double>();

                for (int pointIndex = 0; pointIndex < pointsCount; pointIndex++)
                {
                    double? value = currentSeries.Values[pointIndex];

                    if (value == null && currentSeries.ShowNulls) value = 0d;

                    if (dotArea.ReverseVert && value != null) value = -value;

                    double srY = 0;
                    if (currentSeries.YAxis == StiSeriesYAxis.LeftYAxis)
                    {
                        srY = -value.GetValueOrDefault() * (float)dotArea.YAxis.Info.Dpi + dotArea.AxisCore.GetDividerY();
                    }
                    else
                    {
                        srY = -value.GetValueOrDefault() * (float)dotArea.YRightAxis.Info.Dpi + dotArea.AxisCore.GetDividerRightY();
                    }

                    #region Parse Argument
                    double? argument = null;

                    // If arguments is DateTime, calculate value from OADate
                    if (currentSeries.Arguments[pointIndex] is DateTime)
                    {
                        var tempDateTime = (DateTime)currentSeries.Arguments[pointIndex];
                        argument = (float)Convert.ToDouble(tempDateTime.ToOADate());
                    }
                    else
                    {
                        try
                        {
                            if (currentSeries.Arguments[pointIndex] != null && currentSeries.Arguments[pointIndex] != DBNull.Value)
                            {
                                object argValue = currentSeries.Arguments[pointIndex];
                                if (argValue is string)
                                {
                                    double res = 0d;
                                    if (double.TryParse((string)argValue, out res))
                                        argument = res;
                                    else
                                        argument = null;
                                }
                                else
                                {
                                    argument = (double)StiConvert.ChangeType(argValue, typeof(double));
                                }
                            }
                        }
                        catch
                        {
                            argument = null;
                        }
                    }
                    #endregion

                    if (argument == null && currentSeries.ShowNulls) argument = 0d;

                    if (value != null && argument != null)
                    {
                        double posX = (argument.Value - dotArea.XAxis.Info.Minimum) * dotArea.XAxis.Info.Dpi;
                        if (dotArea.ReverseHor)
                            posX = rect.Width - posX;

                        points.Add(new PointF((float)posX, (float)srY));
                    }
                    else
                    {
                        points.Add(null);
                    }
                    weights.Add(pointIndex < currentSeries.Weights.Length ? currentSeries.Weights[pointIndex] : 0);
                }
                pointLists.Add(points);
                weightLists.Add(weights);
            }

            int index = 0;
            foreach (var pointList in pointLists)
            {
                var weightList = weightLists[index];
                var currentSeries = seriesArray[index] as IStiBubbleSeries;

                ((StiBubbleSeriesCoreXF)currentSeries.Core).RenderBubbles(context, geom, currentSeries, pointList.ToArray(), weightList.ToArray());

                #region Render Trend Lines
                var trendLines = ((StiSeries)currentSeries).TrendLines;

                var points = new PointF?[pointList.Count];
                pointList.CopyTo(points);

                foreach(StiTrendLine line in trendLines)
                    line.Core.RenderTrendLine(geom, points, dotArea.AxisCore.GetDividerY());
                #endregion

                #region Draw Series Labels
                var labels = currentSeries.Core.GetSeriesLabels();

                if (labels != null && labels.Visible)
                {
                    for (int pointIndex = 0; pointIndex < pointList.Count; pointIndex++)
                    {
                        double? value = currentSeries.Values[pointIndex];

                        double? seriesValue = value;
                        if (dotArea.ReverseVert && seriesValue != null) seriesValue = -seriesValue;

                        if (currentSeries.YAxis == StiSeriesYAxis.LeftYAxis) posY = dotArea.AxisCore.GetDividerY();
                        else posY = dotArea.AxisCore.GetDividerRightY();

                        var endPoint = pointList[pointIndex];
                        if (endPoint != null)
                        {
                            var startPoint = new PointF(endPoint.Value.X, (float)posY);

                            if (rect.Contains(startPoint) || startPoint.Y.Equals(rect.Bottom) || rect.Contains(endPoint.Value))
                            {
                                if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                                {

                                    var weight = currentSeries.Weights.Length > pointIndex ? currentSeries.Weights[pointIndex]: 0;

                                    var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                        CorrectPoint(endPoint.Value, rect, currentSeries.LabelsOffset * context.Options.Zoom),
                                        CorrectPoint(startPoint, rect, currentSeries.LabelsOffset * context.Options.Zoom),
                                        pointIndex, seriesValue, value,
                                        currentSeries.Arguments[pointIndex].ToString(),
                                        currentSeries.Core.GetTag(pointIndex), weight,
                                        pointIndex, pointList.Count, rect, null);

                                    if (seriesLabelsGeom != null)
                                    {
                                        geom.CreateChildGeoms();
                                        geom.ChildGeoms.Add(seriesLabelsGeom);
                                        seriesLabelsGeom.ClientRectangle = CheckLabelsRect(labels, geom, seriesLabelsGeom.ClientRectangle);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                index++;
            }
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
                return StiLocalization.Get("Chart", "Bubble");
            }
        }
        #endregion        

        public StiBubbleSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
