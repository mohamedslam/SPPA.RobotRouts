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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Painters.Context.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiStackedColumnSeriesCoreXF3D : StiSeriesCoreXF3D
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            var columnSeries = this.Series as IStiStackedColumnSeries3D;

            if (columnSeries.AllowApplyStyle)
            {
                columnSeries.Color = color;
                columnSeries.BorderColor = style.Core.GetColumnBorder(color);
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
                return $"3D {StiLocalization.Get("Chart", "StackedColumn")}";
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries3D(StiRender3D render3D, StiContext context, StiRectangle3D rect, StiAreaGeom geom, int index, IStiSeries[] series)
        {
            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            var labelList = new List<StiCellGeom>();

            var area = geom.Area;
            var axisArea = area as IStiAxisArea3D;
            var firstSeries = series[0] as StiStackedColumnSeries3D;

            var isPyramidShape = firstSeries.ColumnShape != StiColumnShape3D.Box;

            var seriesWidth = axisArea.ZAxis.Info.Step * firstSeries.Length;
            var seriesLength = axisArea.XAxis.Info.Step * firstSeries.Width;
            var seriesZPos = axisArea.ZAxis.Info.Step * (1 - firstSeries.Length) / 2;

            float dpiY = (float)axisArea.YAxis.Info.Dpi;

            #region Color Area
            var colorArea = new int?[axisArea.AxisCore.ValuesCount, series.Length];
            var colorAreaIndex = 0;

            for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
            {
                for (int valueIndex = 0; valueIndex < series[seriesIndex].Values.Length; valueIndex++)
                {
                    colorArea[valueIndex, seriesIndex] = colorAreaIndex;
                    colorAreaIndex++;
                }
            }

            int colorCount = colorAreaIndex;
            #endregion
            var isMinusY = false;

            for (int pointIndex = 0; pointIndex < axisArea.AxisCore.ValuesCount; pointIndex++)
            {
                for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
                {
                    var currentSeries = series[seriesIndex] as IStiStackedColumnSeries3D;

                    if (pointIndex < currentSeries.Values.Length)
                    {
                        var value = currentSeries.Values[pointIndex];

                        if (value < 0)
                        {
                            isMinusY = true;
                            break;
                        }
                    }
                }

                if (isMinusY)
                    break;
            }

            for (int pointIndex = 0; pointIndex < axisArea.AxisCore.ValuesCount; pointIndex++)
            {
                var seriesXPos = StiClusteredColumnSeriesCoreXF3D.MeasureXPosition(axisArea, pointIndex, seriesLength);

                float posYMax = 0;
                float posYMin = 0;

                var listGeomPlusY = new List<StiCellGeom>();
                var listGeomMinusY = new List<StiCellGeom>();

                var sumPlusValue = 0d;
                var sumMinusValue = 0d;


                #region Calculate Parametrs for PartialPyramid
                //if (isPyramidShape)
                //{
                for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
                    {
                        var currentSeries = series[seriesIndex] as IStiStackedColumnSeries3D;

                        if (pointIndex < currentSeries.Values.Length)
                        {
                            var value = currentSeries.Values[pointIndex];

                            if (value > 0)
                                sumPlusValue += value.GetValueOrDefault();

                            else
                                sumMinusValue += value.GetValueOrDefault();
                        }
                    }
                //}
                #endregion

                for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
                {
                    var posY = (float)axisArea.AxisCore.GetDividerY();
                    var currentSeries = series[seriesIndex] as IStiStackedColumnSeries3D;

                    if (pointIndex < currentSeries.Values.Length)
                    {
                        int colorIndex = colorArea[pointIndex, seriesIndex].GetValueOrDefault();

                        var value = currentSeries.Values[pointIndex];
                        var totalValue =  series.Cast<StiSeries>().Sum(s => StiStackedColumnSeriesCoreXF.GetValueFromArray(s.Values, pointIndex));

                        #region Full-Stacked
                        if (this.Series is IStiFullStackedColumnSeries3D)
                        {
                            if (!(sumPlusValue == 0 && sumMinusValue == 0) && value != null)
                            {
                                var yAxisDelta = isMinusY ? 0.5f : 1;

                                if (value >= 0)
                                    dpiY = yAxisDelta / (float)sumPlusValue;

                                else
                                    dpiY = yAxisDelta / (float)Math.Abs(sumMinusValue);
                            }
                        }
                        #endregion

                        var seriesHeight = (float)(value.GetValueOrDefault() * dpiY);

                        if (value > 0)
                            posY += posYMax;

                        else
                            posY += posYMin;

                        var columnRect = new StiRectangle3D()
                        {
                            X = seriesXPos,
                            Y = posY,
                            Z = seriesZPos,
                            Width = seriesWidth,
                            Height = seriesHeight,
                            Length = seriesLength
                        };

                        var seriesColor = GetSeriesColor(colorIndex, colorCount, currentSeries.Color);
                        seriesColor = currentSeries.ProcessSeriesColors(colorIndex, seriesColor);
                        var seriesBorderColor = (Color)currentSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);

                        StiCellGeom cellGeom;
                        if (((StiChart)currentSeries.Chart).IsAnimation && context.ContextPainter is StiGdiContextPainter animationContextPainter)
                        {
                            var animation = GetAnimation(axisArea.AxisCore.ValuesCount, pointIndex);
                            seriesColor = (Color)StiAnimationEngine.GetAnimationOpacity(animationContextPainter, seriesColor, animation);
                        }

                        if (!isPyramidShape)
                        {
                            cellGeom = new StiBoxSeriesElementGeom3D(columnRect, value.GetValueOrDefault(), pointIndex, currentSeries, seriesColor, seriesBorderColor, render3D);
                        }

                        else
                        {
                            var seriesFullColumnHeight = 
                                value > 0
                                ? sumPlusValue * dpiY
                                : sumMinusValue * dpiY;

                            var fullColumnRect = new StiRectangle3D()
                            {
                                X = seriesXPos,
                                Y = (float)axisArea.AxisCore.GetDividerY(),
                                Z = seriesZPos,
                                Width = seriesWidth,
                                Height = seriesFullColumnHeight,
                                Length = seriesLength
                            };
                            cellGeom = new StiPartialPyramidSeriesElementGeom3D(fullColumnRect, columnRect, value.GetValueOrDefault(), pointIndex, currentSeries, seriesColor, seriesBorderColor, render3D);
                        }

                        if (currentSeries.Core.Interaction != null)
                            ((StiSeriesElementGeom3D)cellGeom).Interaction = new StiSeriesInteractionData(axisArea, currentSeries, pointIndex);

                        #region Render Series Labels
                        var labels = currentSeries.Core.GetSeriesLabels();

                        if (labels != null && labels.Visible)
                        {
                            var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF3D)labels.Core).RenderLabel3D(render3D, Series, context, columnRect,
                                pointIndex, value, value,
                                "",
                                currentSeries.Core.GetTag(pointIndex), 0,
                                colorIndex, colorCount);

                            if (seriesLabelsGeom != null)
                                labelList.Add(seriesLabelsGeom);
                        }
                        #endregion

                        if (value > 0)
                            listGeomPlusY.Add(cellGeom);

                        else
                            listGeomMinusY.Add(cellGeom);

                        if (value > 0)
                            posYMax += (float)seriesHeight;
                        else
                            posYMin += (float)seriesHeight;
                    }                    
                }

                listGeomMinusY.Reverse();

                for (int indexGeom = 0; indexGeom < listGeomMinusY.Count; indexGeom++)
                {
                    if (listGeomMinusY[indexGeom] is IStiDrawSidesGeom3D geom3D)
                    {
                        geom3D.DrawLeft = false;
                        geom3D.DrawBack = false;

                        geom3D.DrawTop = indexGeom == 0;
                        geom3D.DrawBottom = listGeomPlusY.Count == 0 && indexGeom == listGeomMinusY.Count - 1;
                    }
                }

                for (int indexGeom = 0; indexGeom < listGeomPlusY.Count; indexGeom++)
                {
                    if (listGeomPlusY[indexGeom] is IStiDrawSidesGeom3D geom3D)
                    {
                        geom3D.DrawLeft = false;
                        geom3D.DrawBack = false;

                        geom3D.DrawBottom = listGeomMinusY.Count == 0 && indexGeom == 0;
                        geom3D.DrawTop = indexGeom == listGeomPlusY.Count - 1;
                    }
                }

                foreach (var cellGeom in listGeomMinusY)
                {
                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(cellGeom);
                }

                foreach (var cellGeom in listGeomPlusY)
                {
                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(cellGeom);
                }
            }

            #region Render Series labels over other geoms
            foreach (var seriesLabelsGeom in labelList)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesLabelsGeom);
            }
            #endregion
        }

        private StiAnimation GetAnimation(int valuesCount, int index)
        {
            var beginTime = new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks / valuesCount * index);
            return new StiOpacityAnimation(StiChartHelper.GlobalDurationElement, beginTime);
        }

        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            object color = base.GetSeriesBorderColor(colorIndex, colorCount);
            if (color == null) return ((IStiStackedColumnSeries3D)this.Series).BorderColor;
            return color;
        }

        public override Color GetSeriesColor(int colorIndex, int colorCount, Color color)
        {
            if (this.Series.Chart == null || this.Series.Chart.Area == null)
                return color;

            if (this.Series.Chart.Area.ColorEach && string.IsNullOrEmpty(((StiSeries)this.Series).AutoSeriesColorDataColumn))
            {
                var styleCore = Series.Chart.Style != null ? Series.Chart.Style.Core : new StiStyleCoreXF29();

                return styleCore.GetColorByIndex(colorIndex, colorCount, SeriesColors);
            }
            return color;
        }
        #endregion

        public StiStackedColumnSeriesCoreXF3D(IStiSeries series)
            : base(series)
        {
        }
    }
}
