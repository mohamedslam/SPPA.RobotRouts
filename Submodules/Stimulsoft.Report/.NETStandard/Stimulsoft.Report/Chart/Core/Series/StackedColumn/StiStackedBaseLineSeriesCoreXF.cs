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
using System.Collections;
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiStackedBaseLineSeriesCoreXF : StiSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            IStiStackedBaseLineSeries lineSeries = this.Series as IStiStackedBaseLineSeries;

            if (lineSeries.AllowApplyStyle)
            {
                lineSeries.LineColor = color;
                lineSeries.Lighting = style.Core.SeriesLighting;
                lineSeries.Marker.Visible = style.Core.MarkerVisible;

                if (lineSeries.Marker != null)
                {
                    lineSeries.Marker.Brush = new StiSolidBrush(StiColorUtils.Light(color, 100));
                    lineSeries.Marker.BorderColor = StiColorUtils.Dark(color, 100);
                }

                if (lineSeries.LineMarker != null)
                {
                    lineSeries.LineMarker.Brush = new StiSolidBrush(StiColorUtils.Light(color, 50));
                    lineSeries.LineMarker.BorderColor = StiColorUtils.Dark(color, 150);
                }
            }
        }
        #endregion

        #region Methods
        protected void ClipLinePoints(StiContext context, StiAreaGeom geom, PointF?[] startPoints, PointF?[] endPoints,
            out PointF?[] newStartPoints, out PointF?[] newEndPoints, out int startIndex, out int endIndex)
        {
            #region Auto Range
            if (((IStiAxisArea)this.Series.Chart.Area).XAxis.Range.Auto)
            {
                startIndex = 0;
                endIndex = startPoints.Length;
                newStartPoints = startPoints;
                newEndPoints = endPoints;
                return;
            }
            #endregion

            startIndex = -1;
            endIndex = -1;

            #region Create Clip Rect
            RectangleF clipRect = ((StiAxisAreaGeom)geom).View.ClientRectangle;
            clipRect.X = 0;
            clipRect.Y = 0;
            #endregion

            int pointIndex = 0;
            foreach (PointF point in startPoints)
            {
                #region Create Value Point
                PointF valuePoint = point;
                valuePoint.X += geom.ClientRectangle.X;
                valuePoint.Y += geom.ClientRectangle.Y;
                #endregion

                if (clipRect.X <= valuePoint.X && valuePoint.X < clipRect.Right && startIndex == -1)
                    startIndex = pointIndex;

                if ((!(clipRect.X <= valuePoint.X && valuePoint.X < clipRect.Right)) && startIndex != -1)
                {
                    endIndex = pointIndex;
                    break;
                }

                pointIndex++;
            }
            if (endIndex == -1)
                endIndex = startPoints.Length - 1;

            startIndex--;
            endIndex++;
            if (startIndex < 0) startIndex = 0;
            if (endIndex >= startPoints.Length) endIndex = startPoints.Length - 1;

            int newCount = endIndex - startIndex + 1;
            if (newCount == startPoints.Length)
            {
                newStartPoints = startPoints;
                newEndPoints = endPoints;
                return;
            }

            newStartPoints = new PointF?[newCount];
            Array.Copy(startPoints, startIndex, newStartPoints, 0, newCount);

            if (endPoints != null)
            {
                newEndPoints = new PointF?[newCount];
                Array.Copy(endPoints, startIndex, newEndPoints, 0, newCount);
            }
            else newEndPoints = null;

        }

        protected virtual void RenderMarkers(StiContext context, StiAreaGeom geom, PointF?[] points)
        {
            IStiAxisArea axisArea = geom.Area as IStiAxisArea;

            IStiStackedBaseLineSeries lineSeries = this.Series as IStiStackedBaseLineSeries;

            if (points.Length == 0) return;

            var isTooltipMarkerMode = !lineSeries.Marker.Visible && lineSeries.ToolTips.Length > 0;

            if (lineSeries.Marker != null && (lineSeries.Marker.Visible || isTooltipMarkerMode))
            {
                StiSolidBrush shadowBrush = new StiSolidBrush(Color.FromArgb(55, Color.Black));
                StiPenGeom pen = new StiPenGeom(lineSeries.Marker.BorderColor);

                int index = 0;
                foreach (PointF? point in points)
                {
                    if (point != null)
                    {
                        double? value = axisArea.ReverseHor ?
                                this.Series.Values[this.Series.Values.Length - index - 1] :
                                this.Series.Values[index];

                        if (value == null && lineSeries.ShowNulls)
                            value = 0d;

                        #region Create Clip Rect
                        RectangleF clipRect = ((StiAxisAreaGeom)geom).View.ClientRectangle;
                        clipRect.X = 0;
                        clipRect.Y = 0;
                        clipRect.Inflate(10, 10);
                        #endregion

                        #region Create Value Point
                        PointF valuePoint = point.Value;
                        valuePoint.X += geom.ClientRectangle.X;
                        valuePoint.Y += geom.ClientRectangle.Y;
                        #endregion

                        if (clipRect.Contains(valuePoint))
                        {
                            var markerGeom = new StiMarkerGeom(this.Series, index, value.GetValueOrDefault(), point.Value, lineSeries.Marker, this.Series.ShowShadow, context.Options.Zoom, isTooltipMarkerMode);

                            if (markerGeom != null)
                            {
                                if (lineSeries.Core.Interaction != null)
                                    markerGeom.Interaction = new StiSeriesInteractionData(axisArea, lineSeries, index);

                                geom.CreateChildGeoms();
                                geom.ChildGeoms.Add(markerGeom);
                            }
                        }
                    }
                    index++;
                }
            }
        }

        protected virtual void RenderLines(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo)
        {
        }

        protected virtual void RenderAreas(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo)
        {
        }

        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            var lineSeries = this.Series as IStiStackedBaseLineSeries;

            var labelList = new List<StiSeriesLabelsGeom>();

            var area = geom.Area as IStiArea;

            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            var axisArea = area as IStiAxisArea;
            float posY = this.Series.YAxis == StiSeriesYAxis.LeftYAxis
                ? axisArea.AxisCore.GetDividerY()
                : axisArea.AxisCore.GetDividerRightY();

            bool isAnimationChangingValues = ((StiChart)this.Series.Chart).IsAnimationChangingValues;

            bool getStartFromZero = axisArea.XAxis.Core.GetStartFromZero();

            var pointMaxStack = new Hashtable();
            var pointMinStack = new Hashtable();

            var pointLists = new List<PointF?[]>();
            var pointFromLists = new List<PointF?[]>();
            var pointIdsLists = new List<string[]>();

            float dpiY = (float)axisArea.YAxis.Info.Dpi;
            if (this.Series.YAxis == StiSeriesYAxis.RightYAxis)
                dpiY = (float)axisArea.YRightAxis.Info.Dpi;

            for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
            {
                var currentSeries = series[seriesIndex] as IStiStackedBaseLineSeries;

                int pointsCount = currentSeries.Values.Length;

                var endPoints = new PointF?[pointsCount];
                var startPoints = new PointF?[pointsCount];
                var pointsIds = new string[pointsCount];

                for (int pointIndex = 0; pointIndex < pointsCount; pointIndex++)
                {
                    float posX = getStartFromZero ?
                        axisArea.XAxis.Info.StripPositions[pointIndex + 1] :
                        axisArea.XAxis.Info.StripPositions[pointIndex];

                    double? value = axisArea.ReverseHor ?
                            currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] :
                            currentSeries.Values[pointIndex];

                    if (value == null && !currentSeries.ShowNulls)
                    {
                        startPoints[pointIndex] = null;
                        endPoints[pointIndex] = null;
                        pointsIds[pointIndex] = null;
                    }
                    else
                    {
                        if (value == null && currentSeries.ShowNulls)
                            value = 0d;

                        #region FullStacked
                        double totalPositiveHeight = 0;
                        double totalNegativeHeight = 0;

                        CalculateTotalHeight(series, pointIndex, out totalPositiveHeight, out totalNegativeHeight);

                        if (((StiStackedBaseLineSeriesCoreXF)lineSeries.Core).IsFullStacked)
                        {
                            if (!(totalPositiveHeight == 0 && totalNegativeHeight == 0) && value != null)
                            {
                                if (value >= 0)
                                    dpiY = (float)posY / ((float)totalPositiveHeight);
                                else
                                    dpiY = (float)(rect.Height - posY) / ((float)totalNegativeHeight);
                            }
                        }
                        #endregion

                        if (axisArea.ReverseVert && value != null) value = -value;

                        float srY = -(float)value.GetValueOrDefault() * dpiY;

                        float srYStack = 0;

                        if (value >= 0)
                        {
                            if (pointMaxStack[pointIndex] != null) srYStack = (float)pointMaxStack[pointIndex];
                        }
                        else
                        {
                            if (pointMinStack[pointIndex] != null) srYStack = (float)pointMinStack[pointIndex];
                        }

                        srY += srYStack;

                        if (value >= 0) pointMaxStack[pointIndex] = srY;
                        else pointMinStack[pointIndex] = srY;

                        startPoints[pointIndex] = new PointF(posX, srYStack + posY);
                        endPoints[pointIndex] = new PointF(posX, srY + posY);

                        var argId = getStartFromZero ?
                            axisArea.XAxis.Info.StripLines[pointIndex + 1].ValueObject :
                            axisArea.XAxis.Info.StripLines[pointIndex].ValueObject;
                        pointsIds[pointIndex] = argId == null ? "" : argId.ToString();
                    }
                }

                if (endPoints.Length > 0 && startPoints.Length > 0)
                {
                    int startIndex;
                    int endIndex;
                    PointF?[] newStartPoints = null;
                    PointF?[] newEndPoints = null;

                    ClipLinePoints(context, geom, startPoints, endPoints, out newStartPoints, out newEndPoints, out startIndex, out endIndex);
                    var pointsInfo = new StiSeriesPointsInfo()
                    {
                        PointsStart = newStartPoints,
                        PointsEnd = newEndPoints,
                        PointsIds = pointsIds
                    };

                    ((StiStackedBaseLineSeriesCoreXF)currentSeries.Core).RenderAreas(context, geom, pointsInfo);
                }

                pointLists.Add(endPoints);
                pointFromLists.Add(startPoints);
                pointIdsLists.Add(pointsIds);
            }


            int index = 0;
            float[] prevMaxHeights = new float[axisArea.AxisCore.ValuesCount];
            float[] prevMinHeights = new float[axisArea.AxisCore.ValuesCount];

            for (int pointIndex = 0; pointIndex < axisArea.AxisCore.ValuesCount; pointIndex++)
            {
                prevMaxHeights[pointIndex] = posY;
                prevMinHeights[pointIndex] = posY;
            }

            foreach (var pointList in pointLists)
            {
                int startIndex;
                int endIndex;

                PointF?[] newPoints = null;
                PointF?[] newEndPoints = null;
                ClipLinePoints(context, geom, pointList, null, out newPoints, out newEndPoints, out startIndex, out endIndex);

                var currentSeries = series[index] as IStiStackedBaseLineSeries;
                var pointsInfo = new StiSeriesPointsInfo()
                {
                    Points = newPoints,
                    PointsFrom = pointFromLists[index],
                    PointsIds = pointIdsLists[index]
                };

                ((StiStackedBaseLineSeriesCoreXF)currentSeries.Core).RenderLines(context, geom, pointsInfo);

                #region Render Series Labels
                IStiAxisSeriesLabels labels = currentSeries.Core.GetSeriesLabels();

                if (labels != null && labels.Visible)
                {
                    for (int pointIndex = startIndex; pointIndex <= endIndex; pointIndex++)
                    {
                        if (pointIndex < currentSeries.Values.Length)
                        {
                            double? value = axisArea.ReverseHor ?
                                currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] :
                                currentSeries.Values[pointIndex];

                            if (value == null && lineSeries.ShowNulls)
                                value = 0d;

                            float y = posY;

                            if (value > 0)
                            {
                                y = prevMaxHeights[pointIndex];
                                if (pointList[pointIndex] != null)
                                    prevMaxHeights[pointIndex] = pointList[pointIndex].Value.Y;
                            }
                            else
                            {
                                y = prevMinHeights[pointIndex];
                                if (pointList[pointIndex] != null)
                                    prevMinHeights[pointIndex] = pointList[pointIndex].Value.Y;
                            }

                            double? seriesValue = value;
                            if (axisArea.ReverseVert && value != null) seriesValue = -seriesValue;

                            PointF? endPoint = pointList[pointIndex];

                            if (endPoint != null)
                            {
                                PointF startPoint = new PointF(endPoint.Value.X, y);

                                if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                                {
                                    int argumentIndex = axisArea.XAxis.StartFromZero ? pointIndex + 1 : pointIndex;
                                    StiSeriesLabelsGeom seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                        endPoint.Value, startPoint,
                                        pointIndex, seriesValue, value,
                                        axisArea.AxisCore.GetArgumentLabel(axisArea.XAxis.Info.StripLines[argumentIndex], currentSeries),
                                        currentSeries.Core.GetTag(pointIndex),
                                        0, 1, rect);

                                    if (seriesLabelsGeom != null)
                                    {
                                        labelList.Add(seriesLabelsGeom);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                index++;
            }

            #region Render Series labels over other geoms
            foreach (StiSeriesLabelsGeom seriesLabelsGeom in labelList)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesLabelsGeom);
                seriesLabelsGeom.ClientRectangle = CheckLabelsRect(seriesLabelsGeom.SeriesLabels, geom, seriesLabelsGeom.ClientRectangle);
            }
            #endregion
        }

        private void CalculateTotalHeight(IStiSeries[] series, int pointIndex, out double totalPositiveHeight, out double totalNegativeHeight)
        {
            IStiStackedBaseLineSeries lineSeries = this.Series as IStiStackedBaseLineSeries;

            totalPositiveHeight = 0;
            totalNegativeHeight = 0;

            if (((StiStackedBaseLineSeriesCoreXF)lineSeries.Core).IsFullStacked)
            {
                foreach (IStiSeries currentSeries in series)
                {
                    if (pointIndex < currentSeries.Values.Length)
                    {
                        double? value = ((IStiAxisArea)Series.Chart.Area).ReverseHor ?
                            currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] :
                            currentSeries.Values[pointIndex];

                        if (value == null && lineSeries.ShowNulls)
                            value = 0d;

                        if (value > 0)
                            totalPositiveHeight += value.GetValueOrDefault();
                        else
                            totalNegativeHeight += Math.Abs(value.GetValueOrDefault());
                    }
                }
            }
        }

        private PointF CorrectPoint(PointF point, RectangleF rect)
        {
            if (point.Y < 0) return new PointF(point.X, 0);
            if (point.Y > rect.Height) return new PointF(point.X, rect.Height);
            return point;
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            IStiStackedBaseLineSeries lineSeries = this.Series as IStiStackedBaseLineSeries;

            StiBrush brush = base.GetSeriesBrush(colorIndex, colorCount);
            if (brush == null) return new StiSolidBrush(StiColorUtils.Dark(lineSeries.LineColor, 20));
            return brush;
        }

        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            IStiStackedBaseLineSeries lineSeries = this.Series as IStiStackedBaseLineSeries;

            object color = base.GetSeriesBorderColor(colorIndex, colorCount);
            if (color == null) return lineSeries.LineColor;
            return color;
        }
        #endregion

        #region Properties
        public bool IsFullStacked
        {
            get
            {
                return
                    this is StiFullStackedAreaSeriesCoreXF ||
                    this is StiFullStackedLineSeriesCoreXF ||
                    this is StiFullStackedSplineSeriesCoreXF ||
                    this is StiFullStackedSplineAreaSeriesCoreXF;
            }
        }
        #endregion

        public StiStackedBaseLineSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
