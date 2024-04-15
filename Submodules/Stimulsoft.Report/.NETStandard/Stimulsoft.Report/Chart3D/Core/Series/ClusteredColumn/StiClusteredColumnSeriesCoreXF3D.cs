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
    public class StiClusteredColumnSeriesCoreXF3D : StiSeriesCoreXF3D
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            if (Series.AllowApplyStyle)
            {
                ((IStiClusteredColumnSeries3D)Series).Color = color;
                ((IStiClusteredColumnSeries3D)Series).BorderColor = style.Core.GetColumnBorder(color);
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
                return $"3D {StiLocalization.Get("Chart", "ClusteredColumn")}";
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries3D(StiRender3D render3D, StiContext context, StiRectangle3D rect, StiAreaGeom geom, int seriesIndex, IStiSeries[] series)
        {
            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            Render(render3D, context, rect, geom, seriesIndex, series);
        }

        private void Render(StiRender3D render3D, StiContext context, StiRectangle3D rect, StiAreaGeom geom, int seriesIndex, IStiSeries[] series)
        {
            var area = geom.Area;
            var axisArea = area as IStiAxisArea3D;

            int colorIndex = 0;
            int colorCount = axisArea.AxisCore.ValuesCount * series.Length;

            var currentSeries = series[seriesIndex] as StiClusteredColumnSeries3D;

            if (currentSeries == null) return;

            var maxValue = 0f;
            var minValue = 0f;
            if (currentSeries.ColumnShape == StiColumnShape3D.PartialPyramid)
            {
                foreach (var ser in series)
                {
                    maxValue = Math.Max(maxValue, (float)ser.Values.Max().GetValueOrDefault());
                    minValue = Math.Min(minValue, (float)ser.Values.Min().GetValueOrDefault());
                }
            }

            var seriesLength = axisArea.XAxis.Info.Step * currentSeries.Width;
            var seriesWidth = axisArea.ZAxis.Info.Step * currentSeries.Length;

            var seriesZPos = MeasureZPosition(axisArea, series.Length - seriesIndex - 1) + axisArea.ZAxis.Info.Step * (1 - currentSeries.Length) / 2;

            for (int pointIndex = 0; pointIndex < axisArea.AxisCore.ValuesCount; pointIndex++)
            {
                var seriesXPos = MeasureXPosition(axisArea, pointIndex, seriesLength);

                if (pointIndex < currentSeries.Values.Length)
                {
                    #region Draw Column
                    var value = currentSeries.Values[pointIndex];

                    var seriesColor = GetSeriesColor(colorIndex, colorCount, currentSeries.Color);
                    seriesColor = currentSeries.ProcessSeriesColors(colorIndex, seriesColor);

                    var seriesBorderColor = (Color)currentSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);

                    var columnRect = GetColumnRect3D(currentSeries, value, seriesXPos, seriesLength, seriesZPos, seriesWidth);
                    if (((StiChart)currentSeries.Chart).IsAnimation && context.ContextPainter is StiGdiContextPainter animationContextPainter)
                    {
                        var animation = GetAnimation(axisArea.AxisCore.ValuesCount, pointIndex);
                        columnRect = StiAnimationEngine.GetAnimationRectangle3D(animationContextPainter, columnRect, animation);
                    }

                    StiCellGeom cellGeom = null;
                    if (currentSeries.ColumnShape == StiColumnShape3D.Box)
                    {
                        cellGeom = new StiBoxSeriesElementGeom3D(columnRect, value.GetValueOrDefault(), pointIndex, currentSeries, seriesColor, seriesBorderColor, render3D);
                    }

                    else if (currentSeries.ColumnShape == StiColumnShape3D.PartialPyramid)
                    {
                        var fullValue = value > 0 ? maxValue : minValue;

                        if (fullValue != 0)
                        {
                            var fullColumnRect = GetColumnRect3D(currentSeries, fullValue, seriesXPos, seriesLength, seriesZPos, seriesWidth);
                            cellGeom = new StiPartialPyramidSeriesElementGeom3D(fullColumnRect, columnRect, value.GetValueOrDefault(), pointIndex, currentSeries, seriesColor, seriesBorderColor, render3D);
                        }
                    }

                    else
                    {
                        cellGeom = new StiPyramidSeriesElementGeom3D(columnRect, value.GetValueOrDefault(), pointIndex, currentSeries, seriesColor, seriesBorderColor, render3D);
                    }

                    if (cellGeom != null)
                    {
                        if (currentSeries.Core.Interaction != null)
                            ((StiSeriesElementGeom3D)cellGeom).Interaction = new StiSeriesInteractionData(axisArea, currentSeries, pointIndex);

                        geom.CreateChildGeoms();
                        geom.ChildGeoms.Add(cellGeom);
                    }
                    #endregion

                    #region Render Series Labels
                    var labels = currentSeries.Core.GetSeriesLabels();

                    if (labels != null && labels.Visible)
                    {
                        var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF3D)labels.Core).RenderLabel3D(render3D, Series, context, columnRect,
                            pointIndex, value, value, "", currentSeries.Core.GetTag(pointIndex), 0,
                            colorIndex, colorCount);

                        if (seriesLabelsGeom != null)
                        {
                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(seriesLabelsGeom);
                        }
                    }
                    #endregion
                }

                colorIndex++;
            }
        }

        private StiScaleAnimation GetAnimation(int valuesCount, int index)
        {
            var beginTime = new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks / valuesCount * index);
            return new StiScaleAnimation(1, 1, 0, 1, double.NaN, double.NaN, StiChartHelper.GlobalDurationElement, beginTime);
        }

        internal void RenderSideBySide(StiRender3D render3D, StiContext context, StiRectangle3D rect, StiAreaGeom geom, IStiSeries[] series)
        {
            var seriesLabelsList = new List<StiCellGeom>();

            var area = geom.Area;
            var axisArea = area as IStiAxisArea3D;

            int colorIndex = 0;
            int colorCount = axisArea.AxisCore.ValuesCount * series.Length;
            
            #region Measure Series Length
            float sumSeriesLength = 0;
            foreach (IStiSeries ser in series)
            {
                sumSeriesLength += axisArea.XAxis.Info.Step / series.Length * ((IStiClusteredColumnSeries3D)ser).Width;
            }
            #endregion

            for (int pointIndex = 0; pointIndex < axisArea.AxisCore.ValuesCount; pointIndex++)
            {
                for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
                {
                    var currentSeries = series[seriesIndex] as StiClusteredColumnSeries3D;

                    #region Calculate Parametrs for PartialPyramid
                    var maxValue = 0f;
                    var minValue = 0f;

                    if (currentSeries.ColumnShape == StiColumnShape3D.PartialPyramid)
                    {
                        foreach (var ser in series)
                        {
                            maxValue = Math.Max(maxValue, (float)ser.Values.Max().GetValueOrDefault());
                            minValue = Math.Min(minValue, (float)ser.Values.Min().GetValueOrDefault());
                        }
                    } 
                    #endregion

                    var seriesWidth = axisArea.ZAxis.Info.Step * currentSeries.Length;
                    var seriesLength = axisArea.XAxis.Info.Step / series.Length * currentSeries.Width;

                    var seriesZPos = axisArea.ZAxis.Info.Step * (1 - currentSeries.Length) / 2;
                    float seriesXPos = MeasureLeftPosition((IStiAxisArea3D)area, pointIndex, sumSeriesLength);

                    for (var index = 0; index < seriesIndex; index++)
                    {
                        var ofsetSeriesLength = ((IStiClusteredColumnSeries3D)series[index]).Width;
                        seriesXPos += axisArea.XAxis.Info.Step / series.Length * ofsetSeriesLength;
                    }

                    if (pointIndex < currentSeries.Values.Length)
                    {
                        #region Draw Column
                        var value = currentSeries.Values[pointIndex];

                        var seriesColor = GetSeriesColor(colorIndex, colorCount, currentSeries.Color);
                        seriesColor = currentSeries.ProcessSeriesColors(colorIndex, seriesColor);

                        var seriesBorderColor = (Color)currentSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);

                        var columnRect = GetColumnRect3D(currentSeries, value, seriesXPos, seriesLength, seriesZPos, seriesWidth);

                        if (((StiChart)currentSeries.Chart).IsAnimation && context.ContextPainter is StiGdiContextPainter animationContextPainter)
                        {
                            var animation = GetAnimation(axisArea.AxisCore.ValuesCount, pointIndex);
                            columnRect = StiAnimationEngine.GetAnimationRectangle3D(animationContextPainter, columnRect, animation);
                        }

                        StiCellGeom cellGeom;
                        if (currentSeries.ColumnShape == StiColumnShape3D.Box)
                        {
                            cellGeom = new StiBoxSeriesElementGeom3D(columnRect, value.GetValueOrDefault(), pointIndex, currentSeries, seriesColor, seriesBorderColor, render3D);
                        }

                        else if (currentSeries.ColumnShape == StiColumnShape3D.PartialPyramid)
                        {
                            var fullValue = value > 0 ? maxValue : minValue;
                            var fullColumnRect = GetColumnRect3D(currentSeries, fullValue, seriesXPos, seriesLength, seriesZPos, seriesWidth);
                            cellGeom = new StiPartialPyramidSeriesElementGeom3D(fullColumnRect, columnRect, value.GetValueOrDefault(), pointIndex, currentSeries, seriesColor, seriesBorderColor, render3D);
                        }

                        else
                        {
                            cellGeom = new StiPyramidSeriesElementGeom3D(columnRect, value.GetValueOrDefault(), pointIndex, currentSeries, seriesColor, seriesBorderColor, render3D);
                        }

                        geom.CreateChildGeoms();
                        geom.ChildGeoms.Add(cellGeom);
                        #endregion

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
                            {
                                seriesLabelsList.Add(seriesLabelsGeom);
                            }
                        }
                        #endregion
                    }

                    colorIndex++;
                }
            }

            #region Draw Series Labels
            foreach (StiCellGeom seriesLabelsGeom in seriesLabelsList)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesLabelsGeom);
            }
            #endregion
        }

        private float MeasureLeftPosition(IStiAxisArea3D axisArea, int pointIndex, float sumSeriesWidth)
        {
            var posX = axisArea.XAxis.Info.StripPositions[pointIndex + 1];

            posX -= axisArea.XAxis.Info.Step / 2;

            float seriesLeftPos = posX + (axisArea.XAxis.Info.Step - sumSeriesWidth) / 2;

            return seriesLeftPos;
        }

        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            object color = base.GetSeriesBorderColor(colorIndex, colorCount);
            if (color == null) return ((IStiClusteredColumnSeries3D)this.Series).BorderColor;
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

        internal static double MeasureXPosition(IStiAxisArea3D axisArea, int pointIndex, double length)
        {
            var posX = axisArea.XAxis.Info.StripPositions[pointIndex + 1];

            posX -= /*axisArea.XAxis.Info.Step*/(float)length / 2;

            return posX;
        }

        private double MeasureZPosition(IStiAxisArea3D axisArea, int seriesIndex)
        {
            return axisArea.ZAxis.Info.StripPositions[seriesIndex];
        }

        protected StiRectangle3D GetColumnRect3D(StiClusteredColumnSeries3D currentSeries, double? value, double seriesXPos, float seriesWidth, double seriesZPos, float seriesLength)
        {
            var axisArea = currentSeries.Chart.Area as IStiAxisArea3D;

            var posY = axisArea.AxisCore.GetDividerY();

            var seriesHeight = (float)(value.GetValueOrDefault() * axisArea.YAxis.Info.Dpi);

            return new StiRectangle3D()
            {
                X = seriesXPos,
                Y = posY,
                Z = seriesZPos,
                Width = seriesLength,
                Height = seriesHeight,
                Length = seriesWidth
            };
        }
        #endregion

        public StiClusteredColumnSeriesCoreXF3D(IStiSeries series)
        : base(series)
        {
        }
    }
}
