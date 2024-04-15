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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiSplineRangeSeriesCoreXF : StiSplineSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            IStiSplineRangeSeries areaSeries = this.Series as IStiSplineRangeSeries;

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

            IStiSplineRangeArea rangeArea = geom.Area as IStiSplineRangeArea;
            var axisArea = geom.Area as IStiAxisArea;
            var getStartFromZero = axisArea.XAxis.Core.GetStartFromZero();

            int colorIndex = 0;

            foreach (IStiSplineRangeSeries currentSeries in series)
            {
                int valuesCount = currentSeries.Values.Length;

                if (currentSeries.ValuesEnd.Length < valuesCount) valuesCount = currentSeries.ValuesEnd.Length;

                var points = new PointF?[valuesCount];
                var pointsEnd = new PointF?[valuesCount];
                var pointsIds = new string[valuesCount];

                for (int pointIndex = 0; pointIndex < valuesCount; pointIndex++)
                {
                    double? value = rangeArea.ReverseHor ?
                            currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] :
                            currentSeries.Values[pointIndex];

                    double? valueEnd = rangeArea.ReverseHor ?
                            currentSeries.ValuesEnd[currentSeries.Values.Length - pointIndex - 1] :
                            currentSeries.ValuesEnd[pointIndex];


                    points[pointIndex] = GetYPoint(value, currentSeries, rangeArea, pointIndex);
                    pointsEnd[pointIndex] = GetYPoint(valueEnd, currentSeries, rangeArea, pointIndex);

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

                RenderAreas(context, geom, pointsInfo, currentSeries);
                RenderLines(context, geom, pointsInfo);
                RenderLines(context, geom, pointsInfoEnd);

                #region Render Series Labels
                float posY = 0;

                IStiAxisSeriesLabels labels = currentSeries.Core.GetSeriesLabels();

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
                            PointF startPoint = new PointF(endPoint.Value.X, posY);

                            if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                            {
                                StiSeriesLabelsGeom seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                    CorrectPoint(endPoint.Value, rect, ((IStiBaseLineSeries)currentSeries).LabelsOffset * context.Options.Zoom),
                                    CorrectPoint(startPoint, rect, ((IStiBaseLineSeries)currentSeries).LabelsOffset * context.Options.Zoom),
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
                            PointF startPoint = new PointF(endPointEnd.Value.X, posY);

                            if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                            {
                                StiSeriesLabelsGeom seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                    CorrectPoint(endPointEnd.Value, rect, ((IStiBaseLineSeries)currentSeries).LabelsOffset * context.Options.Zoom),
                                    CorrectPoint(startPoint, rect, ((IStiBaseLineSeries)currentSeries).LabelsOffset * context.Options.Zoom),
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

        private void RenderLines(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsinfo, double?[] values, IStiSplineRangeSeries series)
        {
            if (pointsinfo.Points != null || pointsinfo.Points.Length > 1)
            {
                StiLineSeriesGeom seriesGeom = new StiLineSeriesGeom(geom, pointsinfo, series);
                if (seriesGeom != null)
                {
                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(seriesGeom);
                }

                #region Interaction
                if (this.Interaction != null)
                {
                    seriesGeom.Interactions = GetInteractions(context, geom, pointsinfo.Points);
                }
                #endregion
            }
            RenderMarkers(context, geom, pointsinfo.Points, values, series);
        }

        private void RenderMarkers(StiContext context, StiAreaGeom geom, PointF?[] points, double?[] values, IStiSplineRangeSeries series)
        {
            IStiAxisArea axisArea = geom.Area as IStiAxisArea;
            IStiBaseLineSeries lineSeries = series as IStiBaseLineSeries;

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
                                values[lineSeries.Values.Length - index - 1] :
                                values[index];

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

        private PointF? GetYPoint(double? value, IStiSplineRangeSeries currentSeries, IStiSplineRangeArea axisArea, int index)
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

        private void RenderAreas(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo, IStiSplineRangeSeries series)
        {
            if (pointsInfo.Points == null || pointsInfo.Points.Length <= 1) return;

            StiSplineRangeSeriesGeom seriesGeom = new StiSplineRangeSeriesGeom(geom, pointsInfo, series);

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
                return StiLocalization.Get("Chart", "SplineRange");
            }
        }
        #endregion

        public StiSplineRangeSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
