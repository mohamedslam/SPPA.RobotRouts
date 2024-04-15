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
using System.Collections;
using System.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    public class StiScatterSeriesCoreXF : StiBaseLineSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            IStiBaseLineSeries series = this.Series as IStiBaseLineSeries;

            if (series.AllowApplyStyle)
            {
                if (!style.Core.MarkerVisible)
                {
                    series.Marker.Visible = true;
                }
            }
        }
        #endregion

        #region Methods
        protected override void RenderLines(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo)
        {
            RenderMarkers(context, geom, pointsInfo.Points);
        }

        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesArray)
        {
            if (seriesArray == null || seriesArray.Length == 0 || this.Series.Chart == null) return;

            var dotArea = geom.Area as IStiScatterArea;
            bool isAnimationChangingValues = ((StiChart)this.Series.Chart).IsAnimationChangingValues;
            double posY = 0;

            #region Create array of list of points
            var pointLists = new List<PointF?[]>();
            var pointsIdsList = new List<string[]>();

            for (int seriesIndex = 0; seriesIndex < seriesArray.Length; seriesIndex++)
            {
                var currentSeries = seriesArray[seriesIndex] as IStiScatterSeries;

                int pointsCount = Math.Min(currentSeries.Values.Length, currentSeries.Arguments.Length);

                #region Create list of points
                var points = new PointF?[pointsCount];
                var pointsIds = new string[pointsCount];

                for (int pointIndex = 0; pointIndex < pointsCount; pointIndex++)
                {
                    double? value = currentSeries.Values[pointIndex];

                    if (value == null && ((IStiScatterSeries)this.Series).ShowNulls)
                        value = 0d;

                    if (value == 0 && !((IStiScatterSeries)this.Series).ShowZeros)
                    {
                        points[pointIndex] = null;
                        pointsIds[pointIndex] = "_zero_";
                        continue;
                    }

                    if (value != null)
                    {
                        if (dotArea.ReverseVert && value != null && !(dotArea.YAxis.LogarithmicScale)) value = -value;

                        double srY = 0;
                        if (dotArea.YAxis.LogarithmicScale)
                        {
                            int countStrip = dotArea.YAxis.Info.StripLines.Count;

                            int startPoint = 0;
                            int endPoint = countStrip - 1;

                            double startValue = dotArea.YAxis.Info.StripLines[startPoint].Value;
                            double endValue = dotArea.YAxis.Info.StripLines[endPoint].Value;

                            double decadeY = Math.Abs(rect.Height / (Math.Log10(endValue) - Math.Log10(startValue)));

                            srY = Math.Abs(Math.Log10(dotArea.YAxis.Info.StripLines[startPoint].Value) * decadeY - Math.Log10(value.GetValueOrDefault()) * decadeY);
                        }
                        else
                        {
                            if (currentSeries.YAxis == StiSeriesYAxis.LeftYAxis)
                            {
                                srY = -value.GetValueOrDefault() * dotArea.YAxis.Info.Dpi + dotArea.AxisCore.GetDividerY();
                            }
                            else
                            {
                                srY = -value.GetValueOrDefault() * dotArea.YRightAxis.Info.Dpi + dotArea.AxisCore.GetDividerRightY();
                            }
                        }

                        double? argument = null;

                        #region Parse argument value
                        // If arguments is DateTime, calculate value from OADate
                        if (currentSeries.Arguments[pointIndex] is DateTime)
                        {
                            var tempDateTime = (DateTime)currentSeries.Arguments[pointIndex];
                            argument = Convert.ToDouble(tempDateTime.ToOADate());
                        }
                        else
                        {
                            try
                            {
                                if (currentSeries.Arguments[pointIndex] == null)
                                {
                                    argument = null;
                                }
                                else
                                {
                                    object argValue = currentSeries.Arguments[pointIndex];
                                    if (argValue is string)
                                    {
                                        double res = 0d;
                                        if (StiSeries.TryParseValue((string)argValue, ((StiChart)currentSeries.Chart).Report.GetParsedCulture(), out res))
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

                        if (argument == null)
                        {
                            points[pointIndex] = null;
                            pointsIds[pointIndex] = "_zero_";
                        }
                        else
                        {
                            double posX = 0;
                            if (dotArea.XAxis.LogarithmicScale && dotArea.XAxis.Info.StripLines.Count > 0)
                            {
                                int countStrip = dotArea.XAxis.Info.StripLines.Count;

                                int startPoint = dotArea.ReverseHor ? countStrip - 1 : 0;
                                int endPoint = dotArea.ReverseHor ? 0 : countStrip - 1;

                                double startValue = dotArea.XAxis.Info.StripLines[startPoint].Value;
                                double endValue = dotArea.XAxis.Info.StripLines[endPoint].Value;

                                double decadeX = Math.Abs(rect.Width / (Math.Log10(endValue) - Math.Log10(startValue)));

                                posX = Math.Log10(argument.Value) * decadeX - Math.Log10(dotArea.XAxis.Info.StripLines[startPoint].Value) * decadeX;
                            }
                            else
                            {
                                posX = (argument.Value - dotArea.XAxis.Info.Minimum) * dotArea.XAxis.Info.Dpi;
                            }

                            if (dotArea.ReverseHor)
                                posX = rect.Width - posX;

                            points[pointIndex] = new PointF((float)posX, (float)srY);
                            pointsIds[pointIndex] = argument.Value.ToString();
                        }
                    }
                    else
                    {
                        points[pointIndex] = null;
                        pointsIds[pointIndex] = "_zero_";
                    }
                }
                #endregion

                pointLists.Add(points);
                pointsIdsList.Add(pointsIds);
            }
            #endregion

            int index = 0;
            foreach (var pointList in pointLists)
            {
                var currentSeries = seriesArray[index] as IStiScatterSeries;

                var pointsInfo = new StiSeriesPointsInfo()
                {
                    Points = pointList,
                    PointsIds = pointsIdsList[index]
                };

                ((StiScatterSeriesCoreXF)currentSeries.Core).RenderLines(context, geom, pointsInfo);

                #region Render Trend Line
                var trendLines = ((StiSeries)currentSeries).TrendLines;

                foreach (StiTrendLine line in trendLines)
                    line.Core.RenderTrendLine(geom, pointList, dotArea.AxisCore.GetDividerY());
                #endregion

                #region Draw Series Labels
                var labels = currentSeries.Core.GetSeriesLabels();

                if (labels != null && labels.Visible)
                {
                    for (int pointIndex = 0; pointIndex < pointList.Length; pointIndex++)
                    {
                        double? value = currentSeries.Values[pointIndex];

                        double? seriesValue = value;
                        if (dotArea.ReverseVert && value != null) seriesValue = -seriesValue;

                        if (currentSeries.YAxis == StiSeriesYAxis.LeftYAxis) posY = dotArea.AxisCore.GetDividerY();
                        else posY = dotArea.AxisCore.GetDividerRightY();

                        var endPoint = pointList[pointIndex];

                        if (endPoint != null)
                        {
                            var startPoint = new PointF(endPoint.Value.X, (float)posY);

                            #region Create Clip Rect
                            var clipRect = ((StiAxisAreaGeom)geom).View.ClientRectangle;
                            clipRect.X = 0;
                            clipRect.Y = 0;
                            clipRect.Inflate(10, 10);
                            #endregion

                            if (rect.Contains(startPoint) || startPoint.Y.Equals(rect.Bottom) || rect.Contains(endPoint.Value) || clipRect.Contains(endPoint.Value))
                            {
                                if ((labels.Step == 0 || (pointIndex % labels.Step == 0)))
                                {
                                    StiSeriesLabelsGeom seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                        CorrectPoint(endPoint.Value, rect, currentSeries.LabelsOffset * context.Options.Zoom),
                                        CorrectPoint(startPoint, rect, currentSeries.LabelsOffset * context.Options.Zoom),
                                        pointIndex, seriesValue, value,
                                        /*dotArea.AxisCore.GetArgumentLabel(dotArea.XAxis.Info.StripLines[pointIndex], currentSeries),*/ currentSeries.Arguments[pointIndex].ToString(),
                                        currentSeries.Core.GetTag(pointIndex),
                                        0, 1, rect);

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

            if (geom.Area.Chart.SeriesLabels.PreventIntersection)
            {
                CheckIntersectionLabels(geom);
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
                return StiLocalization.Get("Chart", "Scatter");
            }
        }
        #endregion        

        public StiScatterSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
