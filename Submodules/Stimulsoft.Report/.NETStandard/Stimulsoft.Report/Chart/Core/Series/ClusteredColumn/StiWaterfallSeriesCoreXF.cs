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
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiWaterfallSeriesCoreXF : StiClusteredColumnSeriesCoreXF
    {
        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            this.RenderColumns(context, rect, geom, series);
        }

        private float GetSumSeriesWidth(IStiAxisArea axisArea, IStiSeries[] series)
        {
            float sumSeriesWidth = 0;
            foreach (IStiSeries ser in series)
            {
                sumSeriesWidth += axisArea.XAxis.Info.Step / series.Length * ((IStiWaterfallSeries)ser).Width;
            }
            return sumSeriesWidth;
        }

        private float[] GetDividerYSeries(IStiAxisArea axisArea, IStiSeries[] series)
        {
            float posY = axisArea.AxisCore.GetDividerY();
            var array = new float[series.Length];

            for (var index = 0; index < array.Length; index++)
                array[index] = posY;

            return array;
        }

        private void RenderColumns(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            var area = geom.Area;

            var seriesLabelsList = new List<StiSeriesLabelsGeom>();
            var axisArea = area as IStiAxisArea;

            var posYSeries = GetDividerYSeries(axisArea, series);

            int colorIndex = 0;
            int colorCount = axisArea.AxisCore.ValuesCount * series.Length;

            float sumSeriesWidth = GetSumSeriesWidth(axisArea, series);

            for (int pointIndex = 0; pointIndex < axisArea.AxisCore.ValuesCount; pointIndex++)
            {
                float posX = axisArea.XAxis.Core.GetStartFromZero() ?
                    axisArea.XAxis.Info.StripPositions[pointIndex + 1] :
                    axisArea.XAxis.Info.StripPositions[pointIndex];

                posX -= axisArea.XAxis.Info.Step / 2;

                float seriesLeftPos = (posX + (axisArea.XAxis.Info.Step - sumSeriesWidth) / 2);

                int seriesIndex = axisArea.ReverseHor ? series.Length - 1 : 0;

                while ((seriesIndex < series.Length && (!axisArea.ReverseHor)) || (seriesIndex >= 0 && axisArea.ReverseHor))
                {
                    var currentSeries = series[seriesIndex] as IStiWaterfallSeries;

                    float seriesWidth = axisArea.XAxis.Info.Step / series.Length * currentSeries.Width;

                    if (pointIndex < currentSeries.Values.Length)
                    {
                        double? value = axisArea.ReverseHor ?
                            currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] :
                            currentSeries.Values[pointIndex];

                        if (value == null)
                            seriesWidth = 0;

                        if (axisArea.ReverseVert && value != null) value = -value;

                        var isLastValue = axisArea.ReverseHor ? pointIndex == 0 : pointIndex == currentSeries.Values.Length - 1;

                        var posY = isLastValue && currentSeries.Total.Visible ? axisArea.AxisCore.GetDividerY() : posYSeries[seriesIndex];

                        var columnRect = isLastValue && currentSeries.Total.Visible ?
                            GetColumnRect(context, currentSeries, value, seriesLeftPos, seriesWidth, ref posY) :
                            GetColumnRect(context, currentSeries, value, seriesLeftPos, seriesWidth, ref posYSeries[seriesIndex]);

                        RectangleF rectFrom;
                        if (value < 0)
                            rectFrom = RectangleF.FromLTRB(columnRect.Left, columnRect.Top, columnRect.Right, columnRect.Top);
                        else
                            rectFrom = RectangleF.FromLTRB(columnRect.Left, columnRect.Bottom, columnRect.Right, columnRect.Bottom);


                        #region Create Clip Rect
                        RectangleF clipRect = ((StiAxisAreaGeom)geom).View.ClientRectangle;
                        clipRect.X = 0;
                        clipRect.Y = 0;
                        #endregion

                        #region Create Value Point
                        RectangleF columnRectCheck = columnRect;
                        columnRectCheck.X += geom.ClientRectangle.X;
                        columnRectCheck.Y += geom.ClientRectangle.Y;
                        #endregion

                        if ((columnRectCheck.Right > clipRect.X && columnRectCheck.X < clipRect.Right) || ((IStiAxisArea)this.Series.Chart.Area).XAxis.Range.Auto)
                        {
                            #region Draw Column
                            StiBrush seriesBrush;
                            Color seriesBorderColor;
                            if (area.ColorEach)
                            {
                                seriesBrush = currentSeries.Core.GetSeriesBrush(colorIndex, colorCount);
                                if (currentSeries.AllowApplyBrushNegative && (value < 0))
                                    seriesBrush = currentSeries.BrushNegative;
                                seriesBrush = currentSeries.ProcessSeriesBrushes(pointIndex, seriesBrush);

                                seriesBorderColor = (Color)currentSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);
                            }
                            else
                            {
                                seriesBrush = currentSeries.ConnectorLine.Visible && !(isLastValue && currentSeries.Total.Visible)
                                    ? currentSeries.Core.GetSeriesBrush(0, 3)
                                    : currentSeries.Core.GetSeriesBrush(2, 3);

                                if (value < 0)
                                    seriesBrush = currentSeries.AllowApplyBrushNegative ? currentSeries.BrushNegative : currentSeries.Core.GetSeriesBrush(1, 3);

                                seriesBrush = currentSeries.ProcessSeriesBrushes(pointIndex, seriesBrush);
                                seriesBrush = StiBrush.Light(seriesBrush, (byte)(seriesIndex * 10));

                                seriesBorderColor = (Color)currentSeries.Core.GetSeriesBorderColor(0, 3);
                            }

                            if (this.Series.Chart != null && this.Series.Chart.Style != null)
                            {
                                StiClusteredColumnSeriesElementGeom seriesColumnGeom = null;
                                if (columnRect.Height > 0)
                                {
                                    seriesColumnGeom = new StiClusteredColumnSeriesElementGeom(geom, value.GetValueOrDefault(), pointIndex,
                                            seriesBrush, seriesBorderColor, currentSeries, columnRect, rectFrom);

                                    if (currentSeries.Core.Interaction != null)
                                        seriesColumnGeom.Interaction = new StiSeriesInteractionData(axisArea, currentSeries, pointIndex);
                                }

                                #region Draw Line
                                if (currentSeries.ConnectorLine.Visible && !isLastValue)
                                {
                                    var pen = new StiPenGeom(currentSeries.ConnectorLine.LineColor, currentSeries.ConnectorLine.LineWidth);
                                    pen.PenStyle = currentSeries.ConnectorLine.LineStyle;

                                    var y = value > 0 ? columnRect.Top : columnRect.Bottom;
                                    var width = columnRect.Right + axisArea.XAxis.Info.Step - seriesWidth;
                                    var lineGeom = new StiWaterfallLineGeom(new PointF(columnRect.Right, y), new PointF(width, y), pen, new RectangleF(columnRect.Right, y, width - columnRect.Right, y), ((StiChart)currentSeries.Chart).IsAnimation);

                                    geom.CreateChildGeoms();
                                    geom.ChildGeoms.Add(lineGeom);
                                }
                                #endregion

                                if (seriesColumnGeom != null)
                                {
                                    geom.CreateChildGeoms();
                                    geom.ChildGeoms.Add(seriesColumnGeom);
                                }
                            }
                            #endregion

                            #region Render Series Labels
                            var labels = currentSeries.Core.GetSeriesLabels();

                            if (labels != null && labels.Visible && value != null)
                            {
                                var posYLabel = isLastValue && currentSeries.Total.Visible ? axisArea.AxisCore.GetDividerY() : posY;
                                var endPoint = GetPointEnd(currentSeries, value, seriesLeftPos, seriesWidth, posYLabel);

                                double? seriesValue = axisArea.ReverseVert ? -value : value;

                                StiAnimation animation = null;

                                if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                                {
                                    int argumentIndex = axisArea.XAxis.StartFromZero ? pointIndex + 1 : pointIndex;
                                    var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                        CorrectPoint(endPoint, rect),
                                        CorrectPoint(new PointF(endPoint.X, posYLabel), rect),
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

        protected PointF GetPointEnd(IStiClusteredColumnSeries currentSeries, double? value, float seriesLeftPos, float seriesWidth, float posY)
        {
            PointF endPoint;

            var axisArea = currentSeries.Chart.Area as IStiAxisArea;

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

        protected RectangleF GetColumnRect(StiContext context, IStiClusteredColumnSeries currentSeries, double? value, float seriesLeftPos, float seriesWidth, ref float posY)
        {
            var axisArea = currentSeries.Chart.Area as IStiAxisArea;

            float seriesHeight = 0;
            float seriesTopPos = 0;

            float posRightY = axisArea.AxisCore.GetDividerRightY();

            #region LeftYAxis
            if (currentSeries.YAxis == StiSeriesYAxis.LeftYAxis)
            {
                if (!axisArea.ReverseVert)
                {
                    seriesHeight = (float)(value.GetValueOrDefault() * axisArea.YAxis.Info.Dpi);
                    seriesTopPos = -seriesHeight + posY;
                }
                else
                {
                    seriesHeight = (float)(value.GetValueOrDefault() * axisArea.YAxis.Info.Dpi);
                    seriesTopPos = -seriesHeight + posY;
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

            posY = value > 0 ? seriesTopPos : seriesTopPos + seriesHeight;

            return new RectangleF(seriesLeftPos, seriesTopPos, seriesWidth, seriesHeight);
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
                return StiLocalization.Get("Chart", "Waterfall");
            }
        }
        #endregion        

        public StiWaterfallSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
