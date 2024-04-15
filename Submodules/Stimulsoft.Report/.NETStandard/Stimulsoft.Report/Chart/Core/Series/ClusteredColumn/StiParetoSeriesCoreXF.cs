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
    public class StiParetoSeriesCoreXF : StiClusteredColumnSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            var paretoSeries = this.Series as IStiParetoSeries;

            if (Series.AllowApplyStyle)
            {
                paretoSeries.Brush = style.Core.GetColumnBrush(color);
                paretoSeries.BorderColor = style.Core.GetColumnBorder(color);

                paretoSeries.Lighting = style.Core.SeriesLighting;
                paretoSeries.Marker.Visible = style.Core.MarkerVisible;

                paretoSeries.BorderThickness = style.Core.SeriesBorderThickness;
                paretoSeries.CornerRadius = style.Core.SeriesCornerRadius;

                if (paretoSeries.Marker != null)
                {
                    paretoSeries.Marker.Brush = new StiSolidBrush(StiColorUtils.Light(color, 100));
                    paretoSeries.Marker.BorderColor = StiColorUtils.Dark(color, 100);
                }

                if (paretoSeries.LineMarker != null)
                {
                    paretoSeries.LineMarker.Brush = new StiSolidBrush(StiColorUtils.Light(color, 50));
                    paretoSeries.LineMarker.BorderColor = StiColorUtils.Dark(color, 150);
                }
            }
            if (!paretoSeries.AllowApplyLineColor)
            {
                paretoSeries.LineColor = StiColorUtils.Light(color, 50);
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            base.RenderSeries(context, rect, geom, series);
            this.RenderLinePareto(context, rect, geom, series);
        }
        
        private void RenderLinePareto(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            var area = geom.Area;

            bool isAnimationChangingValues = ((StiChart)this.Series.Chart).IsAnimationChangingValues;

            var axisArea = area as IStiAxisArea;

            bool getStartFromZero = axisArea.XAxis.Core.GetStartFromZero();

            rect.Width += 0.001f;

            #region Create List of Points
            var pointLists = new List<PointF?[]>();
            var pointFromLists = new List<PointF?[]>();
            var pointIdsLists = new List<string[]>();

            for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
            {
                var currentSeries = series[seriesIndex] as IStiBaseLineSeries;

                var paretoValues = GetParetoValues(currentSeries.Values);

                int pointsCount = paretoValues.Count;

                var points = new PointF?[pointsCount];
                var pointsFrom = new PointF?[pointsCount];
                var pointsIds = new string[pointsCount];

                for (int pointIndex = 0; pointIndex < pointsCount; pointIndex++)
                {

                    var stripPoint = getStartFromZero ? pointIndex + 1 : pointIndex;

                    if (stripPoint >= axisArea.XAxis.Info.StripPositions.Length) break;

                    float posX = axisArea.XAxis.Info.StripPositions[stripPoint];

                    double? value = axisArea.ReverseHor ?
                            paretoValues[paretoValues.Count - pointIndex - 1] :
                            paretoValues[pointIndex];

                    points[pointIndex] = GetPointValue(value, currentSeries, axisArea, posX);

                    #region Calculation Points From
                    var paretoValuesStart = GetParetoValues(currentSeries.ValuesStart);

                    double? valueFrom = 0;
                    if (paretoValuesStart.Count > pointIndex)
                    {
                        valueFrom = axisArea.ReverseHor ?
                            paretoValuesStart[paretoValuesStart.Count - pointIndex - 1] :
                            paretoValuesStart[pointIndex];
                    }

                    pointsFrom[pointIndex] = GetPointValue(valueFrom, currentSeries, axisArea, posX);
                    #endregion

                    var argId = axisArea.XAxis.Info.StripLines[stripPoint].ValueObject;
                    pointsIds[pointIndex] = argId == null ? "" : argId.ToString();
                }

                if (points.Length > 0)
                {
                    points = StiPointHelper.OptimizePoints(points);

                    var pointsInfo = new StiSeriesPointsInfo()
                    {
                        PointsFrom = pointsFrom,
                        Points = points,
                        PointsIds = pointsIds
                    };

                    RenderLines(context, geom, pointsInfo, currentSeries);
                }

                pointLists.Add(points);
                if (isAnimationChangingValues) pointFromLists.Add(pointsFrom);
            }
            #endregion
        }

        private void RenderLines(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo, IStiBaseLineSeries series)
        {
            if (pointsInfo.Points != null && pointsInfo.Points.Length > 1)
            {
                var seriesGeom = new StiLineSeriesGeom(geom, pointsInfo, series);

                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesGeom);
            }
        }

        private List<double> GetParetoValues(double?[] values)
        {
            var paretoTempValues = new List<double>();
            var paretoValues = new List<double>();

            double sum = 0;

            foreach (var value in values)
            {
                sum += value.GetValueOrDefault();
                paretoTempValues.Add(sum);
            }

            foreach (var value in paretoTempValues)
            {
                var percentValue = value / sum * 100;
                paretoValues.Add(percentValue);
            }

            return paretoValues;
        }

        private PointF? GetPointValue(double? value, IStiBaseLineSeries currentSeries, IStiAxisArea axisArea, float posX)
        {
            if (axisArea.ReverseVert && value != null) value = -value;

            double srY = -value.GetValueOrDefault() * (float)axisArea.YRightAxis.Info.Dpi + axisArea.AxisCore.GetDividerRightY();

            return new PointF(posX, (float)srY);
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
                return StiLocalization.Get("Chart", "Pareto");
            }
        }
        #endregion        

        public StiParetoSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
