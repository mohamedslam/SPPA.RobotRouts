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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.TextFormats;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiBaseLineSeriesCoreXF :
        StiSeriesCoreXF,
        IStiApplyStyleSeries
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            IStiBaseLineSeries columnSeries = this.Series as IStiBaseLineSeries;

            if (columnSeries.AllowApplyStyle)
            {
                columnSeries.LineColor = color;

                columnSeries.Lighting = style.Core.SeriesLighting;

                if (columnSeries.Marker != null)
                {
                    var marker = columnSeries.Marker as StiMarker;
                    if (marker != null && marker.ExtendedVisible != StiExtendedStyleBool.FromStyle)
                        columnSeries.Marker.Visible = marker.ExtendedVisible == StiExtendedStyleBool.True;
                    else
                        columnSeries.Marker.Visible = style.Core.MarkerVisible;

                    columnSeries.Marker.Brush = new StiSolidBrush(StiColorUtils.Light(color, 100));
                    columnSeries.Marker.BorderColor = StiColorUtils.Dark(color, 100);
                }

                if (columnSeries.LineMarker != null)
                {
                    columnSeries.LineMarker.Brush = new StiSolidBrush(StiColorUtils.Light(color, 50));
                    columnSeries.LineMarker.BorderColor = StiColorUtils.Dark(color, 150);
                }
            }
        }
        #endregion

        #region Methods
        protected PointF?[] ClipLinePoints(StiContext context, StiAreaGeom geom, PointF?[] points, out int startIndex, out int endIndex)
        {
            #region Auto Range
            if (((IStiAxisArea)this.Series.Chart.Area).XAxis.Range.Auto)
            {
                startIndex = 0;
                endIndex = points.Length;
                return points;
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
            foreach (PointF? point in points)
            {
                if (point != null)
                {
                    #region Create Value Point
                    PointF valuePoint = point.Value;
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
                }

                pointIndex++;
            }
            if (endIndex == -1)
                endIndex = points.Length - 1;

            startIndex--;
            endIndex++;
            if (startIndex < 0) startIndex = 0;
            if (endIndex >= points.Length) endIndex = points.Length - 1;

            int newCount = endIndex - startIndex + 1;
            if (newCount == points.Length)
                return points;

            PointF?[] newPoints = new PointF?[newCount];
            Array.Copy(points, startIndex, newPoints, 0, newCount);
            return newPoints;
        }

        protected virtual void RenderMarkers(StiContext context, StiAreaGeom geom, PointF?[] points)
        {
            var axisArea = geom.Area as IStiAxisArea;
            var lineSeries = this.Series as IStiBaseLineSeries;

            if (points.Length == 0) return;

            var isTooltipMarkerMode = !lineSeries.Marker.Visible && lineSeries.ToolTips.Length > 0 && !((StiComponent)this.Series.Chart).Report.IsDesigning;

            if (lineSeries.Marker != null)
            {
                int index = 0;
                foreach (PointF? point in points)
                {
                    if (point != null)
                    {
                        double? value = axisArea.ReverseHor ?
                                lineSeries.Values[lineSeries.Values.Length - index - 1] :
                                lineSeries.Values[index];

                        if (value == null && lineSeries.ShowNulls)
                            value = 0d;

                        #region Create Clip Rect
                        var clipRect = ((StiAxisAreaGeom)geom).View.ClientRectangle;
                        clipRect.X = 0;
                        clipRect.Y = 0;
                        clipRect.Inflate(10, 10);
                        #endregion

                        #region Create Value Point
                        var valuePoint = point.Value;
                        valuePoint.X += geom.ClientRectangle.X;
                        valuePoint.Y += geom.ClientRectangle.Y;
                        #endregion

                        if (clipRect.Contains(valuePoint))
                        {
                            var markerGeom = new StiMarkerGeom(this.Series, index, value.GetValueOrDefault(), point.Value, lineSeries.Marker, lineSeries.ShowShadow, context.Options.Zoom, isTooltipMarkerMode);

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

        protected List<StiSeriesInteractionData> GetInteractions(StiContext context, StiAreaGeom geom, PointF?[] points)
        {
            var interactions = new List<StiSeriesInteractionData>();
            for (int index = 0; index < points.Length; index++)
            {
                var data = new StiSeriesInteractionData(geom.Area as IStiAxisArea, this.Series, index);
                data.Point = points[index];

                interactions.Add(data);
            }
            return interactions;
        }

        protected virtual void RenderLines(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo)
        {
        }

        protected virtual void RenderAreas(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo)
        {
        }

        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            var area = geom.Area;
            var axisArea = area as IStiAxisArea;

            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            bool isAnimationChangingValues = ((StiChart)this.Series.Chart).IsAnimationChangingValues;

            bool getStartFromZero = axisArea.XAxis.Core.GetStartFromZero();

            rect.Width += 0.001f;

            #region Create List of Points, Render Areas
            float posY = 0;

            var pointLists = new List<PointF?[]>();
            var pointFromLists = new List<PointF?[]>();
            var pointsZeroConnectList = new List<List<PointF?>>();
            var pointsNullConnectList = new List<List<PointF?>>();
            var pointsIdsList = new List<string[]>();

            for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
            {
                var currentSeries = series[seriesIndex] as IStiBaseLineSeries;

                int pointsCount = currentSeries.Values.Length;

                var points = new PointF?[pointsCount];
                var pointsStart = new PointF?[pointsCount];
                var pointsIds = new string[pointsCount];

                var valuesStartApproximation = ((StiSeries)series[seriesIndex]).GetApproximationValuesStart();

                for (int pointIndex = 0; pointIndex < pointsCount; pointIndex++)
                {
                    var stripPoint = getStartFromZero ? pointIndex + 1 : pointIndex;
                    if (stripPoint >= axisArea.XAxis.Info.StripPositions.Length) break;

                    float posX = axisArea.XAxis.Info.StripPositions[stripPoint];

                    points[pointIndex] = GetPointValue(pointIndex, currentSeries, currentSeries.Values, axisArea, posX);

                    pointsStart[pointIndex] = GetPointValue(pointIndex, currentSeries, valuesStartApproximation, axisArea, posX);
                    var argId = axisArea.XAxis.Info.StripLines[stripPoint].ValueObject;
                    pointsIds[pointIndex] = argId == null ? "" : argId.ToString();
                }

                var pointZeroConnectLists = GetPointsZeroConnect(currentSeries, axisArea);
                var pointNullConnectLists = GetPointsNullConnect(currentSeries, axisArea);

                pointsZeroConnectList.Add(pointZeroConnectLists);
                pointsNullConnectList.Add(pointNullConnectLists);
                pointsIdsList.Add(pointsIds);

                if (points.Length > 0)
                {
                    points = StiPointHelper.OptimizePoints(points);
                    pointsStart = StiPointHelper.OptimizePoints(pointsStart);

                    int startIndex;
                    int endIndex;
                    var newPointList = ClipLinePoints(context, geom, points, out startIndex, out endIndex);
                    var newPointFromList = ClipLinePoints(context, geom, pointsStart, out startIndex, out endIndex);
                    var pointsInfo = new StiSeriesPointsInfo()
                    {
                        PointsFrom = newPointFromList,
                        Points = newPointList,
                        PointsZeroConnect = pointZeroConnectLists.ToArray(),
                        PointsNullConnect = pointNullConnectLists.ToArray(),
                        PointsIds = pointsIds
                    };

                    ((StiBaseLineSeriesCoreXF)currentSeries.Core).RenderAreas(context, geom, pointsInfo);

                    if (!IsTopmostLine(currentSeries))
                        ((StiBaseLineSeriesCoreXF)currentSeries.Core).RenderLines(context, geom, pointsInfo);
                }

                pointLists.Add(points);
                pointFromLists.Add(pointsStart);
            }
            #endregion

            int index = 0;
            foreach (var pointList in pointLists)
            {
                int startIndex;
                int endIndex;
                var newPointList = ClipLinePoints(context, geom, pointList, out startIndex, out endIndex);
                var newPointFromList = ClipLinePoints(context, geom, pointFromLists[index], out startIndex, out endIndex);
                var pointsIds = pointsIdsList[index];

                int startIndex2 = startIndex;
                int endIndex2 = endIndex;
                var newPointZeroConnectList = ClipLinePoints(context, geom, pointsZeroConnectList[index].ToArray(), out startIndex2, out endIndex2);
                var newPointNullConnectList = ClipLinePoints(context, geom, pointsNullConnectList[index].ToArray(), out startIndex2, out endIndex2);

                var currentSeries = series[index] as IStiBaseLineSeries;

                if (IsTopmostLine(series[index]))
                {
                    var pointsInfo = new StiSeriesPointsInfo()
                    {
                        PointsFrom = newPointFromList,
                        Points = newPointList,
                        PointsZeroConnect = newPointZeroConnectList.ToArray(),
                        PointsNullConnect = newPointNullConnectList.ToArray(),
                        PointsIds = pointsIds
                    };

                    ((StiBaseLineSeriesCoreXF)currentSeries.Core).RenderLines(context, geom, pointsInfo);
                }

                #region Render Trend Line
                var trendLines = ((StiSeries)currentSeries).TrendLines;

                foreach (StiTrendLine line in trendLines)
                    line.Core.RenderTrendLine(geom, newPointList, axisArea.AxisCore.GetDividerY());
                #endregion

                #region Render Series Labels
                var labels = currentSeries.Core.GetSeriesLabels();

                if (labels != null && labels.Visible)
                {
                    for (int pointIndex = startIndex; pointIndex <= endIndex; pointIndex++)
                    {
                        if (currentSeries.Values.Length > pointIndex)
                        {
                            double? value = axisArea.ReverseHor ?
                                currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] :
                                currentSeries.Values[pointIndex];

                            if (value == null && currentSeries.ShowNulls)
                                value = 0d;

                            double? seriesValue = value;
                            if (axisArea.ReverseVert && value != null)
                                seriesValue = -seriesValue;

                            if (currentSeries.YAxis == StiSeriesYAxis.LeftYAxis) posY = axisArea.AxisCore.GetDividerY();
                            else posY = axisArea.AxisCore.GetDividerRightY();

                            var endPoint = pointList.Length > pointIndex ? pointList[pointIndex] : null;

                            if (endPoint != null)
                            {

                                double? valueFrom = 0;
                                if (currentSeries.ValuesStart != null && currentSeries.ValuesStart.Length > pointIndex)
                                {
                                    valueFrom = axisArea.ReverseHor ?
                                        currentSeries.ValuesStart[currentSeries.Values.Length - pointIndex - 1] :
                                        currentSeries.ValuesStart[pointIndex];
                                }

                                var startPoint = new PointF(endPoint.Value.X, posY);

                                StiAnimation animation = null;

                                if (isAnimationChangingValues && labels.ValueType == StiSeriesLabelsValueType.Value)
                                {
                                    if (labels.ValueType == StiSeriesLabelsValueType.Value && string.IsNullOrEmpty(labels.Format) && labels.FormatService is StiGeneralFormatService)
                                    {
                                        var endPointFrom = pointFromLists[index].Length > pointIndex ? pointFromLists[index][pointIndex] : null;
                                        animation = new StiLabelAnimation(valueFrom, value, endPointFrom.GetValueOrDefault(), endPoint.GetValueOrDefault(), StiChartHelper.GlobalBeginTimeElement, TimeSpan.Zero);
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
                                        CorrectPoint(endPoint.Value, rect, currentSeries.LabelsOffset * context.Options.Zoom),
                                        CorrectPoint(startPoint, rect, currentSeries.LabelsOffset * context.Options.Zoom),
                                        pointIndex, seriesValue, value,
                                        axisArea.AxisCore.GetArgumentLabel(axisArea.XAxis.Info.StripLines[argumentIndex], currentSeries),
                                        currentSeries.Core.GetTag(pointIndex),
                                        0, 1, rect, animation);

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

        private List<PointF?> GetPointsZeroConnect(IStiBaseLineSeries series, IStiAxisArea axisArea)
        {
            var pointZeroConnectLists = new List<PointF?>();
            if (series.ShowZerosAs == StiShowEmptyCellsAs.Gap) return pointZeroConnectLists;

            var pointConnectIndexGroupList = new List<List<int>>();
            var indexGroup = new List<int>();

            for (var index = 0; index < series.Values.Length; index++)
            {
                var value = series.Values[index];
                if (value == 0)
                {
                    indexGroup.Add(index);
                    if (indexGroup.Count == 1)// add new group
                        pointConnectIndexGroupList.Add(indexGroup);
                }
                else
                {
                    indexGroup = new List<int>();
                }
            }

            pointZeroConnectLists = GetPointConnect(series, axisArea, pointConnectIndexGroupList);

            return pointZeroConnectLists;
        }

        private List<PointF?> GetPointsNullConnect(IStiBaseLineSeries series, IStiAxisArea axisArea)
        {
            var pointNullConnectLists = new List<PointF?>();
            if (series.ShowNullsAs == StiShowEmptyCellsAs.Gap) return pointNullConnectLists;

            var pointConnectIndexGroupList = new List<List<int>>();
            var indexGroup = new List<int>();

            for (var index = 0; index < series.Values.Length; index++)
            {
                var value = series.Values[index];
                if (value == null)
                {
                    indexGroup.Add(index);
                    if (indexGroup.Count == 1)// add new group
                        pointConnectIndexGroupList.Add(indexGroup);
                }
                else
                {
                    indexGroup = new List<int>();
                }
            }

            pointNullConnectLists = GetPointConnect(series, axisArea, pointConnectIndexGroupList);

            return pointNullConnectLists;
        }

        private List<PointF?> GetPointConnect(IStiBaseLineSeries series, IStiAxisArea axisArea, List<List<int>> pointConnectIndexGroupList)
        {
            var pointConnectLists = new List<PointF?>();

            foreach (var indexGroupCurrent in pointConnectIndexGroupList)
            {
                int startIndexGroup = indexGroupCurrent[0] - 1;
                int endIndexGroup = indexGroupCurrent[indexGroupCurrent.Count - 1] + 1;

                double startValueInterpolation = 0;
                double endValueInterpolation = 0;

                if (startIndexGroup >= 0 && startIndexGroup < series.Values.Length)
                {
                    startValueInterpolation = series.Values[startIndexGroup].GetValueOrDefault();

                    // Add Start Value
                    var pointStart = GetPointValue(series, axisArea, startValueInterpolation, startIndexGroup);
                    pointConnectLists.Add(pointStart);
                }

                if (endIndexGroup >= 0 && endIndexGroup < series.Values.Length)
                {
                    endValueInterpolation = series.Values[endIndexGroup].GetValueOrDefault();
                }

                var deltaInterpolation = (endValueInterpolation - startValueInterpolation) / (indexGroupCurrent.Count + 1);

                for (var index = 0; index < indexGroupCurrent.Count; index++)
                {
                    var value = startValueInterpolation + deltaInterpolation * (index + 1);

                    var point = GetPointValue(series, axisArea, value, indexGroupCurrent[index]);
                    pointConnectLists.Add(point);
                }

                if (endIndexGroup >= 0 && endIndexGroup < series.Values.Length)
                {

                    // Add End Value
                    var pointEnd = GetPointValue(series, axisArea, endValueInterpolation, endIndexGroup);
                    pointConnectLists.Add(pointEnd);
                }

                pointConnectLists.Add(null);
            }

            return pointConnectLists;
        }

        private static PointF GetPointValue(IStiBaseLineSeries series, IStiAxisArea axisArea, double value, int index)
        {
            index = axisArea.XAxis.Core.GetStartFromZero() ? index + 1 : index;
            if (index >= axisArea.XAxis.Info.StripPositions.Length)
                return PointF.Empty;

            float posX = axisArea.XAxis.Info.StripPositions[index];

            double srY;
            if (series.YAxis == StiSeriesYAxis.LeftYAxis)
            {
                srY = -value * (float)axisArea.YAxis.Info.Dpi + axisArea.AxisCore.GetDividerY();
            }
            else
            {
                srY = -value * (float)axisArea.YRightAxis.Info.Dpi + axisArea.AxisCore.GetDividerRightY();
            }

            return new PointF(posX, (float)srY);
        }

        private PointF? GetPointValue(int pointIndex, IStiBaseLineSeries currentSeries, double?[] values, IStiAxisArea axisArea, float posX)
        {
            double? value = 0;
            if (values.Length > pointIndex)
            {
                value = axisArea.ReverseHor ?
                           values[values.Length - pointIndex - 1] :
                           values[pointIndex];
            }

            return GetPointValue(value, currentSeries, axisArea, posX);
        }

        private PointF? GetPointValue(double? value, IStiBaseLineSeries currentSeries, IStiAxisArea axisArea, float posX)
        {
            var isAreaSeries = currentSeries is StiAreaSeries ||
                currentSeries is StiSplineAreaSeries ||
                currentSeries is StiStackedAreaSeries ||
                currentSeries is StiStackedSplineAreaSeries ||
                currentSeries is StiSteppedAreaSeries;

            if (value == 0 && !currentSeries.ShowZeros && !isAreaSeries) return null;
            if (value == null && !currentSeries.ShowNulls && !isAreaSeries) return null;
            if (value == null && currentSeries.ShowNulls) value = 0d;
            if (axisArea.ReverseVert && value != null) value = -value;

            double srY = 0;
            if (currentSeries.YAxis == StiSeriesYAxis.LeftYAxis)
            {
                srY = -value.GetValueOrDefault() * (float)axisArea.YAxis.Info.Dpi + axisArea.AxisCore.GetDividerY();
            }
            else
            {
                srY = -value.GetValueOrDefault() * (float)axisArea.YRightAxis.Info.Dpi + axisArea.AxisCore.GetDividerRightY();
            }

            return new PointF(posX, (float)srY);
        }

        private bool IsTopmostLine(IStiSeries series)
        {
            if (series is IStiSplineAreaSeries)
                return ((IStiSplineAreaSeries)series).TopmostLine;

            if (series is IStiSteppedAreaSeries)
                return ((IStiSteppedAreaSeries)series).TopmostLine;

            if (series is IStiAreaSeries)
                return ((IStiAreaSeries)series).TopmostLine;

            return true;
        }

        protected PointF CorrectPoint(PointF point, RectangleF rect, float correctY)
        {
            if (point.Y + correctY < 0) return new PointF(point.X, 0);
            if (point.Y + correctY > rect.Height) return new PointF(point.X, rect.Height);
            return new PointF(point.X, point.Y + correctY);
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            IStiBaseLineSeries lineSeries = this.Series as IStiBaseLineSeries;

            StiBrush brush = base.GetSeriesBrush(colorIndex, colorCount);
            if (brush == null)
            {
                if (this.Series is StiBubbleSeries bubbleSeries)
                    return bubbleSeries.Brush;

                return new StiSolidBrush(StiColorUtils.Dark(lineSeries.LineColor, 20));
            }
                
            return brush;
        }

        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            IStiBaseLineSeries lineSeries = this.Series as IStiBaseLineSeries;

            object color = base.GetSeriesBorderColor(colorIndex, colorCount);
            
            if (color == null)
            {
                if (this.Series is StiBubbleSeries bubbleSeries)
                    return bubbleSeries.BorderColor;

                return lineSeries.LineColor;
            }

            return color;
        }
        #endregion

        public StiBaseLineSeriesCoreXF(IStiSeries series)
                    : base(series)
        {
        }
    }
}
