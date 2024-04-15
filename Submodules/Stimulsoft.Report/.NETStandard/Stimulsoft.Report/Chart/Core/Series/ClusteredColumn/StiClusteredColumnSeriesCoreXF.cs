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
    public class StiClusteredColumnSeriesCoreXF : StiSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            if (Series.AllowApplyStyle)
            {
                ((IStiClusteredColumnSeries)Series).Brush = style.Core.GetColumnBrush(color);
                ((IStiClusteredColumnSeries)Series).BorderColor = style.Core.GetColumnBorder(color);
                ((IStiClusteredColumnSeries)Series).BorderThickness = style.Core.SeriesBorderThickness;
                ((IStiClusteredColumnSeries)Series).CornerRadius = style.Core.SeriesCornerRadius;
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

            bool getStartFromZero = axisArea.XAxis.Core.GetStartFromZero();

            float posY = axisArea.AxisCore.GetDividerY();
            float posRightY = axisArea.AxisCore.GetDividerRightY();

            int colorIndex = 0;
            int colorCount = axisArea.AxisCore.ValuesCount * series.Length;

            #region Lists for Trend Lines
            var pointsLists = new List<List<PointF?>>();
            for (int index = 0; index < series.Length; index++)
            {
                pointsLists.Add(new List<PointF?>());
            }
            #endregion

            #region Measure Series Width
            float sumSeriesWidth = 0;
            foreach (IStiSeries ser in series)
            {
                sumSeriesWidth += axisArea.XAxis.Info.Step / series.Length * ((IStiClusteredColumnSeries)ser).Width;
            }
            #endregion

            for (int pointIndex = 0; pointIndex < axisArea.AxisCore.ValuesCount; pointIndex++)
            {
                float seriesLeftPos = MeasureLeftPosition(axisArea, pointIndex, sumSeriesWidth, getStartFromZero);

                int seriesIndex = 0;
                if (axisArea.ReverseHor) seriesIndex = series.Length - 1;

                while ((seriesIndex < series.Length && (!axisArea.ReverseHor)) || (seriesIndex >= 0 && axisArea.ReverseHor))
                {
                    var currentSeries = series[seriesIndex] as IStiClusteredColumnSeries;

                    float seriesWidth = axisArea.XAxis.Info.Step / series.Length * currentSeries.Width;

                    var valuesStartApproximation = ((StiSeries)series[seriesIndex]).GetApproximationValuesStart();

                    if (pointIndex < currentSeries.Values.Length)
                    {
                        double? value = GetSeriesCurrentValue(axisArea, currentSeries, pointIndex);
                        var columnRect = GetColumnRect(context, currentSeries, value, seriesLeftPos, seriesWidth);

                        double? valueStart = GetSeriesValueStart(axisArea, currentSeries.ValuesStart, pointIndex);
                        double? valueStartApproximation = GetSeriesValueStart(axisArea, valuesStartApproximation, pointIndex);
                        var columnRectStart = GetColumnRect(context, currentSeries, valueStartApproximation ?? 0, seriesLeftPos, seriesWidth);

                        #region Add Point Trend Line
                        if (!((StiSeries)currentSeries).TrendLines.ToList().All(l => l is IStiTrendLineNone))
                        {
                            pointsLists[seriesIndex].Add(new PointF(seriesLeftPos + seriesWidth / 2, columnRect.Y));
                        }
                        #endregion

                        #region Create Clip Rect
                        var clipRect = ((StiAxisAreaGeom)geom).View.ClientRectangle;
                        clipRect.X = 0;
                        clipRect.Y = 0;
                        #endregion

                        #region Create Value Point
                        var columnRectCheck = columnRect;
                        columnRectCheck.X += geom.ClientRectangle.X;
                        columnRectCheck.Y += geom.ClientRectangle.Y;
                        #endregion

                        if ((columnRectCheck.Right > clipRect.X && columnRectCheck.X < clipRect.Right) || ((IStiAxisArea)this.Series.Chart.Area).XAxis.Range.Auto)
                        {
                            #region Draw Column
                            var seriesBrush = GetSeriesBrush(currentSeries, value, colorIndex, colorCount, pointIndex);
                            var seriesBorderColor = (Color)currentSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);
                            
                            if (this.Series.Chart != null && this.Series.Chart.Style != null && columnRect.Height > 0)
                            {
                                var seriesColumnGeom = new StiClusteredColumnSeriesElementGeom(geom, value.GetValueOrDefault(), pointIndex,
                                    seriesBrush, seriesBorderColor, currentSeries, columnRect, columnRectStart);

                                if (currentSeries.Core.Interaction != null)
                                    seriesColumnGeom.Interaction = new StiSeriesInteractionData(axisArea, currentSeries, pointIndex);

                                geom.CreateChildGeoms();
                                geom.ChildGeoms.Add(seriesColumnGeom);
                            }
                            #endregion

                            #region Render Series Labels
                            var labels = currentSeries.Core.GetSeriesLabels();

                            if (labels != null && labels.Visible)
                            {
                                var endPoint = GetPointEnd(currentSeries, value, seriesLeftPos, seriesWidth);

                                double? seriesValue = 0;

                                if (value != null)
                                    seriesValue = axisArea.ReverseVert ? -value : value;

                                StiAnimation animation = null;

                                if (isAnimationChangingValues)
                                {
                                    if (labels.ValueType == StiSeriesLabelsValueType.Value && string.IsNullOrEmpty(labels.Format) && labels.FormatService is StiGeneralFormatService)
                                    {
                                        var endPointFrom = GetPointEnd(currentSeries, valueStart, seriesLeftPos, seriesWidth);
                                        animation = new StiLabelAnimation(valueStart, value, new PointF(endPointFrom.X, endPointFrom.Y), new PointF(endPoint.X, endPoint.Y), StiChartHelper.GlobalBeginTimeElement, TimeSpan.Zero);
                                    }
                                    else
                                    {
                                        animation = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, TimeSpan.Zero);
                                    }
                                }

                                if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                                {
                                    int argumentIndex = axisArea.XAxis.StartFromZero ? pointIndex + 1 : pointIndex;
                                    var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                        CorrectPoint(endPoint, rect),
                                        CorrectPoint(new PointF(endPoint.X, posY), rect),
                                        pointIndex, value, seriesValue,
                                        axisArea.AxisCore.GetArgumentLabel(axisArea.XAxis.Info.StripLines[argumentIndex], currentSeries),
                                        currentSeries.Core.GetTag(pointIndex),
                                        colorIndex, colorCount, rect, animation);

                                    if (seriesLabelsGeom != null)
                                    {
                                        seriesLabelsList.Add(seriesLabelsGeom);
                                        seriesLabelsGeom.ClientRectangle = CheckLabelsRect(labels, geom, seriesLabelsGeom.ClientRectangle);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    seriesLeftPos += seriesWidth;

                    colorIndex++;

                    if (axisArea.ReverseHor) seriesIndex--;
                    else seriesIndex++;
                }
            }

            #region Draw Trend Lines
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

            if (geom.Area.Chart.SeriesLabels.PreventIntersection)
            {
                CheckIntersectionLabels(geom);
            }
            #endregion
        }

        protected PointF GetPointEnd(IStiClusteredColumnSeries currentSeries, double? value, float seriesLeftPos, float seriesWidth)
        {
            PointF endPoint;

            var axisArea = currentSeries.Chart.Area as IStiAxisArea;

            float posY = axisArea.AxisCore.GetDividerY();
            float posRightY = axisArea.AxisCore.GetDividerRightY();

            if (currentSeries.YAxis == StiSeriesYAxis.LeftYAxis)
            {
                endPoint = new PointF(seriesLeftPos + seriesWidth / 2,
                                      -(float)(value.GetValueOrDefault() * axisArea.YAxis.Info.Dpi) + posY);
            }
            else
            {
                endPoint = new PointF(seriesLeftPos + seriesWidth / 2,
                                      -(float)(value.GetValueOrDefault() * axisArea.YRightAxis.Info.Dpi) + posRightY);
            }

            return endPoint;
        }

        protected RectangleF GetColumnRect(StiContext context, IStiClusteredColumnSeries currentSeries, double? value, float seriesLeftPos, float seriesWidth)
        {
            var axisArea = currentSeries.Chart.Area as IStiAxisArea;

            float seriesHeight = 0;
            float seriesTopPos = 0;

            float posY = axisArea.AxisCore.GetDividerY();
            float posRightY = axisArea.AxisCore.GetDividerRightY();

            #region LeftYAxis
            if (currentSeries.YAxis == StiSeriesYAxis.LeftYAxis)
            {
                if (!axisArea.ReverseVert)
                {
                    seriesHeight = (float)(value.GetValueOrDefault() * axisArea.YAxis.Info.Dpi);
                    seriesTopPos = (float)(-seriesHeight + posY);
                }
                else
                {
                    seriesHeight = (float)(value.GetValueOrDefault() * axisArea.YAxis.Info.Dpi);
                    seriesTopPos = (float)(-seriesHeight + posY);
                }
            }
            #endregion

            #region LeftYRightAxis
            else
            {
                seriesHeight = (float)(value.GetValueOrDefault() * axisArea.YRightAxis.Info.Dpi);
                seriesTopPos = -seriesHeight + posRightY;
            }
            #endregion

            #region ShowZeros
            if (currentSeries.ShowZeros && (value == 0 || value == null))
            {
                seriesHeight = Math.Max(context.Options.Zoom, 2);
                if (!axisArea.ReverseVert) seriesTopPos -= seriesHeight;
            }
            #endregion

            #region value < 0
            if (value < 0)
            {
                if (currentSeries.YAxis == StiSeriesYAxis.LeftYAxis) seriesTopPos = posY;
                else seriesTopPos = posRightY;

                seriesHeight = -seriesHeight;
            }
            #endregion

            return new RectangleF(seriesLeftPos, seriesTopPos, seriesWidth, seriesHeight);
        }

        protected PointF CorrectPoint(PointF point, RectangleF rect)
        {
            if (point.Y < 0) return new PointF(point.X, 0);
            if (point.Y > rect.Height) return new PointF(point.X, rect.Height);
            return point;
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            var brush = base.GetSeriesBrush(colorIndex, colorCount);
            if (brush == null) return ((IStiClusteredColumnSeries)this.Series).Brush;
            return brush;
        }

        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            object color = base.GetSeriesBorderColor(colorIndex, colorCount);
            if (color == null) return ((IStiClusteredColumnSeries)this.Series).BorderColor;
            return color;
        }

        private float MeasureLeftPosition(IStiAxisArea axisArea, int pointIndex, float sumSeriesWidth, bool startFromZero)
        {
            var posX = startFromZero ?
                    axisArea.XAxis.Info.StripPositions[pointIndex + 1] :
                    axisArea.XAxis.Info.StripPositions[pointIndex];

            posX -= axisArea.XAxis.Info.Step / 2;

            float seriesLeftPos = posX + (axisArea.XAxis.Info.Step - sumSeriesWidth) / 2;

            return seriesLeftPos;
        }

        private double? GetSeriesCurrentValue(IStiAxisArea axisArea, IStiClusteredColumnSeries currentSeries, int pointIndex)
        {
            double? value = axisArea.ReverseHor ?
                            currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] :
                            currentSeries.Values[pointIndex];

            if (axisArea.ReverseVert && value != null) value = -value;

            return value;
        }

        private double? GetSeriesValueStart(IStiAxisArea axisArea, double?[] valuesStart, int pointIndex)
        {
            double? valueStart = null;

            if (((StiChart)this.Series.Chart).IsAnimationChangingValues)
            {
                if (valuesStart != null && valuesStart.Length > pointIndex)
                {
                    valueStart = axisArea.ReverseHor ?
                        valuesStart[valuesStart.Length - pointIndex - 1] :
                        valuesStart[pointIndex];
                }

                if (axisArea.ReverseVert && valueStart != null) valueStart = -valueStart;
            }

            return valueStart;
        }

        private StiBrush GetSeriesBrush(IStiClusteredColumnSeries currentSeries, double? value, int colorIndex, int colorCount, int pointIndex)
        {
            var seriesBrush = currentSeries.Core.GetSeriesBrush(colorIndex, colorCount);
            if (currentSeries.AllowApplyBrushNegative && (value < 0))
                seriesBrush = currentSeries.BrushNegative;
            seriesBrush = currentSeries.ProcessSeriesBrushes(pointIndex, seriesBrush);

            return seriesBrush;
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
                return StiLocalization.Get("Chart", "ClusteredColumn");
            }
        }
        #endregion        

        public StiClusteredColumnSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
