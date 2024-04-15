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
using Stimulsoft.Report.Components.TextFormats;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiClusteredBarSeriesCoreXF :
        StiClusteredColumnSeriesCoreXF,
        IStiApplyStyleSeries
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            var barSeries = this.Series as IStiClusteredBarSeries;

            if (barSeries.AllowApplyStyle)
            {
                barSeries.Brush = style.Core.GetColumnBrush(color);

                if (barSeries.Brush is StiGradientBrush)
                    ((StiGradientBrush)barSeries.Brush).Angle += 90;

                if (barSeries.Brush is StiGlareBrush)
                    ((StiGlareBrush)barSeries.Brush).Angle += 90;

                barSeries.BorderColor = style.Core.GetColumnBorder(color);
                barSeries.BorderThickness = style.Core.SeriesBorderThickness;
                barSeries.CornerRadius = style.Core.SeriesCornerRadius;
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            var seriesLabelsList = new List<StiSeriesLabelsGeom>();

            var area = geom.Area;

            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            bool isAnimationChangingValues = ((StiChart)this.Series.Chart).IsAnimationChangingValues;

            var axisArea = area as IStiAxisArea;

            bool getStartFromZero = axisArea.YAxis.Core.GetStartFromZero();

            float posX = axisArea.AxisCore.GetDividerX();
            float posTopX = axisArea.AxisCore.GetDividerTopX();

            int colorCount = axisArea.AxisCore.ValuesCount * series.Length;
            int colorIndex = axisArea.ReverseVert ? colorCount - 1 : 0;

            int pointCount = axisArea.AxisCore.ValuesCount;

            #region Lists for Trend Lines
            var pointsLists = new List<List<PointF?>>();
            for (int index = 0; index < series.Length; index++)
            {
                pointsLists.Add(new List<PointF?>());
            }
            #endregion

            for (int pointIndex = 0; pointIndex < pointCount; pointIndex++)
            {
                float posY = getStartFromZero ?
                    axisArea.YAxis.Info.StripPositions[pointIndex + 1] - axisArea.YAxis.Info.Step / 2 :
                    axisArea.YAxis.Info.StripPositions[pointIndex] - axisArea.YAxis.Info.Step / 2;

                float sumSeriesHeight = 0;

                foreach (IStiSeries ser in series)
                {
                    sumSeriesHeight += axisArea.YAxis.Info.Step / series.Length * ((IStiClusteredColumnSeries)ser).Width;
                }

                float seriesTopPos = posY + (axisArea.YAxis.Info.Step - sumSeriesHeight) / 2;

                int seriesIndex = 0;
                if (axisArea.ReverseVert) seriesIndex = series.Length - 1;

                var time = StiChartHelper.GlobalBeginTimeElement;

                while ((seriesIndex < series.Length && (!axisArea.ReverseVert)) || (seriesIndex >= 0 && axisArea.ReverseVert))
                {
                    var currentSeries = series[seriesIndex] as IStiClusteredBarSeries;

                    float seriesHeight = axisArea.YAxis.Info.Step / series.Length * currentSeries.Width;

                    if (pointIndex < currentSeries.Values.Length)
                    {
                        double? value = -(!axisArea.ReverseVert ?
                            currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] :
                            currentSeries.Values[pointIndex]);

                        double seriesWidth = value.GetValueOrDefault() * axisArea.XAxis.Info.Dpi;

                        var columnRect = GetBarRect(context, currentSeries, value, seriesTopPos, seriesHeight);

                        var rectFrom = RectangleF.Empty;
                        double? valueFrom = null;

                        #region Calculation Rect From
                        if (isAnimationChangingValues)
                        {
                            if (currentSeries.ValuesStart.Length > pointIndex)
                            {
                                valueFrom = -(!axisArea.ReverseVert ?
                                    currentSeries.ValuesStart[currentSeries.ValuesStart.Length - pointIndex - 1] :
                                    currentSeries.ValuesStart[pointIndex]);
                            }

                            if (axisArea.ReverseHor)
                            {
                                if (value < 0)
                                    rectFrom = RectangleF.FromLTRB(columnRect.Right, columnRect.Top, columnRect.Right, columnRect.Bottom);
                                else
                                    rectFrom = RectangleF.FromLTRB(columnRect.Left, columnRect.Top, columnRect.Left, columnRect.Bottom);
                            }
                            else
                            {
                                if (value >= 0)
                                    rectFrom = RectangleF.FromLTRB(columnRect.Right, columnRect.Top, columnRect.Right, columnRect.Bottom);
                                else
                                    rectFrom = RectangleF.FromLTRB(columnRect.Left, columnRect.Top, columnRect.Left, columnRect.Bottom);
                            }
                        }
                        #endregion

                        #region Add Point Trend Lines
                        if (!((StiSeries)currentSeries).TrendLines.ToList().All(l => l is IStiTrendLineNone))
                        {
                            pointsLists[seriesIndex].Add(new PointF((float)seriesWidth, seriesTopPos + seriesHeight / 2));
                        }
                        #endregion

                        #region Create Clip Rect
                        var clipRect = ((StiAxisAreaGeom)geom).View.ClientRectangle;
                        clipRect.X = 0;
                        clipRect.Y = 0;
                        #endregion

                        #region Create Value Point
                        RectangleF columnRectCheck = columnRect;
                        columnRectCheck.X += geom.ClientRectangle.X;
                        columnRectCheck.Y += geom.ClientRectangle.Y;
                        #endregion

                        if ((columnRectCheck.Bottom > clipRect.Y && columnRectCheck.Y < clipRect.Bottom) || ((IStiAxisArea)this.Series.Chart.Area).YAxis.Range.Auto)
                        {
                            #region Draw Bar
                            if (this.Series.Chart != null && this.Series.Chart.Style != null && columnRect.Width > 0)
                            {
                                var seriesBrush = currentSeries.Core.GetSeriesBrush(colorIndex, colorCount);

                                if (currentSeries.AllowApplyBrushNegative && (value > 0))
                                    seriesBrush = currentSeries.BrushNegative;

                                seriesBrush = currentSeries.ProcessSeriesBrushes((pointCount - 1) - pointIndex, seriesBrush);

                                var seriesBorderColor = (Color)currentSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);

                                var seriesColumnGeom = new StiClusteredBarSeriesElementGeom(geom, valueFrom.GetValueOrDefault(), axisArea.ReverseHor ? -value.GetValueOrDefault() : value.GetValueOrDefault(), pointIndex,
                                    seriesBrush, seriesBorderColor, currentSeries, rectFrom, columnRect);

                                if (currentSeries.Core.Interaction != null)
                                    seriesColumnGeom.Interaction = new StiSeriesInteractionData(axisArea, currentSeries, (pointCount - 1) - pointIndex);

                                geom.CreateChildGeoms();
                                geom.ChildGeoms.Add(seriesColumnGeom);
                            }
                            #endregion

                            #region Render Series Labels
                            var labels = currentSeries.Core.GetSeriesLabels();

                            if (labels != null && labels.Visible)
                            {
                                double? seriesValue = value;

                                if (value != null)
                                    seriesValue = axisArea.ReverseHor ? value : -value;

                                var currentPosX = currentSeries.YAxis == StiSeriesYAxis.RightYAxis ? posTopX : posX;
                                var currentDpi = currentSeries.YAxis == StiSeriesYAxis.RightYAxis ? axisArea.XTopAxis.Info.Dpi : axisArea.XAxis.Info.Dpi;
                                var endPoint = new PointF((float)(seriesValue.GetValueOrDefault() * currentDpi) + currentPosX,
                                    seriesTopPos + seriesHeight / 2);


                                StiAnimation animation = null;

                                if (isAnimationChangingValues)
                                {
                                    if (labels.ValueType == StiSeriesLabelsValueType.Value && string.IsNullOrEmpty(labels.Format) && labels.FormatService is StiGeneralFormatService)
                                    {
                                        var endPointFrom = new PointF(-(float)(valueFrom.GetValueOrDefault() * currentDpi) + currentPosX, seriesTopPos + seriesHeight / 2);

                                        double? animationValueFrom = null;
                                        double? animationValue = null;

                                        if (valueFrom != null)
                                            animationValueFrom = axisArea.ReverseHor ? valueFrom : -valueFrom;

                                        if (value != null)
                                            animationValue = axisArea.ReverseHor ? value : -value;

                                        animation = new StiLabelAnimation(animationValueFrom, animationValue, new PointF(endPointFrom.X, endPointFrom.Y), new PointF(endPoint.X, endPoint.Y), time, TimeSpan.Zero);
                                    }
                                    else
                                    {
                                        animation = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, TimeSpan.Zero);
                                    }
                                }

                                if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                                {

                                    int argumentIndex = axisArea.YAxis.StartFromZero ? pointIndex + 1 : pointIndex;

                                    var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                        CorrectPoint(endPoint, rect),
                                        CorrectPoint(new PointF(currentPosX, endPoint.Y), rect),
                                        pointIndex, value, axisArea.ReverseHor ? -seriesValue : seriesValue,
                                        axisArea.AxisCore.GetArgumentLabel(axisArea.YAxis.Info.StripLines[argumentIndex], currentSeries),
                                        currentSeries.Core.GetTag(pointIndex),
                                        colorIndex, colorCount, rect, animation);

                                    if (seriesLabelsGeom != null)
                                    {
                                        seriesLabelsList.Add(seriesLabelsGeom);
                                        seriesLabelsGeom.ClientRectangle = CheckLabelsRect(labels, clipRect, seriesLabelsGeom.ClientRectangle);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    seriesTopPos += seriesHeight;
                    colorIndex = axisArea.ReverseVert ? colorIndex - 1 : colorIndex + 1;

                    if (axisArea.ReverseVert) seriesIndex--;
                    else seriesIndex++;
                }
            }

            #region Draw Trend Liness
            for (int indexSeries = 0; indexSeries < series.Length; indexSeries++)
            {
                var trendLines = ((StiSeries)series[indexSeries]).TrendLines;
                var pointsList = pointsLists[indexSeries];

                foreach (StiTrendLine line in trendLines)
                    line.Core.RenderTrendLine(geom, pointsList.ToArray(), axisArea.AxisCore.GetDividerY());
            }
            #endregion

            #region Draw Series Labels in second path
            foreach (StiSeriesLabelsGeom seriesLabelsGeom in seriesLabelsList)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesLabelsGeom);
            }
            #endregion
        }

        private RectangleF GetBarRect(StiContext context, IStiClusteredBarSeries currentSeries, double? value, float seriesTopPos, float seriesHeight)
        {
            var axisArea = currentSeries.Chart.Area as IStiAxisArea;

            float posX = axisArea.AxisCore.GetDividerX();
            float posTopX = axisArea.AxisCore.GetDividerTopX();
            var posCurrentX = currentSeries.YAxis == StiSeriesYAxis.RightYAxis ? posTopX : posX;
            var currentDpi = currentSeries.YAxis == StiSeriesYAxis.RightYAxis ? axisArea.XTopAxis.Info.Dpi : axisArea.XAxis.Info.Dpi;

            if (axisArea.ReverseHor && value != null) value = -value;

            double seriesWidth = value.GetValueOrDefault() * currentDpi;
            double seriesLeftPos = -seriesWidth + posCurrentX;

            if (((IStiClusteredBarSeries)this.Series).ShowZeros && (value == 0 || value == null))
            {
                seriesWidth = Math.Max(context.Options.Zoom, 2);
                if (axisArea.ReverseHor) seriesLeftPos -= seriesWidth;
            }

            if (value < 0)
            {
                seriesLeftPos = posCurrentX;
                seriesWidth = -seriesWidth;
            }

            return new RectangleF((float)seriesLeftPos, seriesTopPos, (float)seriesWidth, seriesHeight);
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
                return StiLocalization.Get("Chart", "ClusteredBar");
            }
        }
        #endregion        

        public StiClusteredBarSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
