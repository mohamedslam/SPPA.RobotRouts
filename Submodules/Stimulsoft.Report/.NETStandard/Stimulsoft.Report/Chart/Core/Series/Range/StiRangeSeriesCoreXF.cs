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

using System.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using System;
using System.Reflection;

namespace Stimulsoft.Report.Chart
{
    public class StiRangeSeriesCoreXF : StiLineSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            IStiLineRangeSeries areaSeries = this.Series as IStiLineRangeSeries;

            if (areaSeries.AllowApplyStyle)
            {
                areaSeries.Brush = style.Core.GetAreaBrush(color);
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            var rangeArea = geom.Area as IStiRangeArea;
            var axisArea = geom.Area as IStiAxisArea;
            var getStartFromZero = axisArea.XAxis.Core.GetStartFromZero();

            int colorIndex = 0;

            foreach (IStiLineRangeSeries currentSeries in series)
            {
                int valuesCount = Math.Min(currentSeries.Values.Length, currentSeries.ValuesEnd.Length);

                var points = new PointF?[valuesCount];
                var pointsEnd = new PointF?[valuesCount];
                var pointsIds = new string[valuesCount];

                for (int pointIndex = 0; pointIndex < valuesCount; pointIndex++)
                {
                    double? value = rangeArea.ReverseHor ?
                            currentSeries.Values[valuesCount - pointIndex - 1] :
                            currentSeries.Values[pointIndex];

                    double? valueEnd = rangeArea.ReverseHor ?
                            currentSeries.ValuesEnd[valuesCount - pointIndex - 1] :
                            currentSeries.ValuesEnd[pointIndex];

                    if (value == valueEnd)
                    {
                        points[pointIndex] = pointsEnd[pointIndex] = GetYPoint(value, currentSeries, rangeArea, pointIndex);
                    }
                    else
                    {
                        points[pointIndex] = GetYPoint(value, currentSeries, rangeArea, pointIndex);
                        pointsEnd[pointIndex] = GetYPoint(valueEnd, currentSeries, rangeArea, pointIndex);
                    }

                    var argId = getStartFromZero ?
                            axisArea.XAxis.Info.StripLines[pointIndex + 1].ValueObject :
                            axisArea.XAxis.Info.StripLines[pointIndex].ValueObject;

                    pointsIds[pointIndex] = argId == null ? "" : argId.ToString();
                }

                var pointsInfo = new StiSeriesPointsInfo()
                {
                    Points = points,
                    PointsEnd = pointsEnd,
                    PointsIds = pointsIds
                };

                var pointsInfoEnd = new StiSeriesPointsInfo()
                {
                    Points = pointsEnd,
                    PointsIds = pointsIds,
                    AdditionalSeriesId = "_e"
                };

                RenderAreas(geom, pointsInfo, currentSeries);
                RenderLines(context, geom, pointsInfo, currentSeries.Values, currentSeries);
                RenderLines(context, geom, pointsInfoEnd, currentSeries.ValuesEnd, currentSeries);

                #region Render Series Labels
                float posY = 0;

                var labels = currentSeries.Core.GetSeriesLabels();

                if (labels != null && labels.Visible)
                {
                    for (int pointIndex = 0; pointIndex < valuesCount; pointIndex++)
                    {
                        double? value = axisArea.ReverseHor ?
                                currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] :
                                currentSeries.Values[pointIndex];

                        double? valueEnd = axisArea.ReverseHor ?
                                currentSeries.ValuesEnd[currentSeries.ValuesEnd.Length - pointIndex - 1] :
                                currentSeries.ValuesEnd[pointIndex];

                        if (value == null && currentSeries.ShowNulls)
                            value = 0d;

                        if (valueEnd == null && currentSeries.ShowNulls)
                            value = 0d;

                        double? seriesValue = value;
                        if (rangeArea.ReverseVert && value != null)
                            seriesValue = -seriesValue;

                        double? seriesValueEnd = valueEnd;
                        if (rangeArea.ReverseVert && valueEnd != null)
                            seriesValueEnd = -seriesValueEnd;

                        if (currentSeries.YAxis == StiSeriesYAxis.LeftYAxis) posY = rangeArea.AxisCore.GetDividerY();
                        else posY = rangeArea.AxisCore.GetDividerRightY();

                        PointF? endPoint = points[pointIndex];
                        PointF? endPointEnd = pointsEnd[pointIndex];

                        if (endPoint != null)
                        {
                            var startPoint = new PointF(endPoint.Value.X, posY);

                            if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                            {
                                var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                    CorrectPoint(endPoint.Value, rect, currentSeries.LabelsOffset * context.Options.Zoom),
                                    CorrectPoint(startPoint, rect, currentSeries.LabelsOffset * context.Options.Zoom),
                                    pointIndex, seriesValue, value,
                                    rangeArea.AxisCore.GetArgumentLabel(rangeArea.XAxis.Info.StripLines[pointIndex], currentSeries),
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

                        if (endPointEnd != null)
                        {
                            var startPoint = new PointF(endPointEnd.Value.X, posY);

                            if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                            {
                                var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                    CorrectPoint(endPointEnd.Value, rect, currentSeries.LabelsOffset * context.Options.Zoom),
                                    CorrectPoint(startPoint, rect, currentSeries.LabelsOffset * context.Options.Zoom),
                                    pointIndex, seriesValueEnd, valueEnd,
                                    rangeArea.AxisCore.GetArgumentLabel(rangeArea.XAxis.Info.StripLines[pointIndex], currentSeries),
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
                #endregion

                colorIndex++;
            }
        }

        private void RenderLines(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo, double?[] values, IStiLineRangeSeries series)
        {
            if (pointsInfo.Points != null && pointsInfo.Points.Length > 1)
            {
                var seriesGeom = new StiLineSeriesGeom(geom, pointsInfo, series);

                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesGeom);

                #region Interaction
                if (this.Interaction != null)
                {
                    seriesGeom.Interactions = GetInteractions(context, geom, pointsInfo.Points);
                }
                #endregion
            }
            RenderMarkers(context, geom, pointsInfo.Points, values, series);
        }

        private void RenderMarkers(StiContext context, StiAreaGeom geom, PointF?[] points, double?[] values, IStiLineRangeSeries series)
        {
            var axisArea = geom.Area as IStiAxisArea;
            var lineSeries = series;

            if (points.Length == 0) return;

            var isTooltipMarkerMode = !lineSeries.Marker.Visible && lineSeries.ToolTips.Length > 0;

            if (lineSeries.Marker != null && (lineSeries.Marker.Visible || isTooltipMarkerMode))
            {
                int index = 0;
                int valuesCount = Math.Min(lineSeries.Values.Length, lineSeries.ValuesEnd.Length);

                foreach (PointF? point in points)
                {
                    if (point != null)
                    {
                        double? value = axisArea.ReverseHor ?
                                values[valuesCount - index - 1] :
                                values[index];

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
                            var markerGeom = new StiMarkerGeom(series, index, value.GetValueOrDefault(), point.Value, lineSeries.Marker, lineSeries.ShowShadow, context.Options.Zoom, isTooltipMarkerMode);

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

        private PointF? GetYPoint(double? value, IStiLineRangeSeries currentSeries, IStiRangeArea axisArea, int index)
        {
            PointF? point;

            float posX = axisArea.XAxis.Core.GetStartFromZero() ?
                        axisArea.XAxis.Info.StripPositions[index + 1] :
                        axisArea.XAxis.Info.StripPositions[index];

            if (value == null && !currentSeries.ShowNulls)
            {
                point = null;
            }
            else
            {
                if (value == null && currentSeries.ShowNulls)
                    value = 0d;

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

                point = new PointF(posX, (float)srY);
            }

            return point;
        }

        private void RenderAreas(StiAreaGeom geom, StiSeriesPointsInfo pointsInfo, IStiLineRangeSeries series)
        {
            if (pointsInfo.Points == null || pointsInfo.Points.Length <= 1) return;

            var seriesGeom = new StiRangeSeriesGeom(geom, pointsInfo, series);

            if (seriesGeom != null)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesGeom);
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
                return StiLocalization.Get("Chart", "Range");
            }
        }
        #endregion

        public StiRangeSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
