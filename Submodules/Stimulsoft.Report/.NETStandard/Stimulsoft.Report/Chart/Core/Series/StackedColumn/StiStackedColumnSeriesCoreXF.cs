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
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiStackedColumnSeriesCoreXF : StiSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            var columnSeries = this.Series as IStiStackedColumnSeries;

            if (columnSeries.AllowApplyStyle)
            {
                columnSeries.Brush = style.Core.GetColumnBrush(color);
                columnSeries.BorderColor = style.Core.GetColumnBorder(color);
                columnSeries.BorderThickness = style.Core.SeriesBorderThickness;
                columnSeries.CornerRadius = style.Core.SeriesCornerRadius;
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            var labelList = new List<StiSeriesLabelsGeom>();
            var seriesList = new List<StiStackedColumnSeriesElementGeom>();

            var area = geom.Area;
            var axisArea = area as IStiAxisArea;
            var columnSeries = this.Series as IStiStackedColumnSeries;

            bool getStartFromZero = axisArea.XAxis.Core.GetStartFromZero();

            float posY = axisArea.AxisCore.GetDividerY();
            
            float dpiY = (float)axisArea.YAxis.Info.Dpi;
            if (this.Series.YAxis == StiSeriesYAxis.RightYAxis)
                dpiY = (float)axisArea.YRightAxis.Info.Dpi;

            var colorArea = new int?[axisArea.AxisCore.ValuesCount, series.Length];
            var colorAreaIndex = 0;

            bool isAnimationChangingValues = ((StiChart)this.Series.Chart).IsAnimationChangingValues;

            for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
            {
                for (int valueIndex = 0; valueIndex < series[seriesIndex].Values.Length; valueIndex++)
                {
                    colorArea[valueIndex, seriesIndex] = colorAreaIndex;
                    colorAreaIndex++;
                }
            }

            int colorCount = colorAreaIndex;

            for (int pointIndex = 0; pointIndex < axisArea.AxisCore.ValuesCount; pointIndex++)
            {
                double totalPositiveHeight = 0;
                double totalNegativeHeight = 0;
                
                CalculateTotalHeight(series, pointIndex, out totalPositiveHeight, out totalNegativeHeight);

                float posX = getStartFromZero ?
                    axisArea.XAxis.Info.StripPositions[pointIndex + 1] :
                    axisArea.XAxis.Info.StripPositions[pointIndex];
                posX -= axisArea.XAxis.Info.Step / 2;

                float seriesWidth = axisArea.XAxis.Info.Step - axisArea.XAxis.Info.Step * (1f - columnSeries.Width);
                float seriesLeftPos = posX + (axisArea.XAxis.Info.Step - seriesWidth) / 2;

                float posYMax = 0;
                float posYMin = 0;

                #region DrawShadow
                if (series.Length == 0) return;
                bool showShadow = series[0].ShowShadow;
                // Shadow for animated StackedBar is rendered in StiStackedColumnSeriesElementGeom
                if (((StiChart)series[0].Chart).IsAnimation && !series[0].Chart.IsDesigning) showShadow = false;

                if (showShadow)
                {
                    for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
                    {
                        if (pointIndex < series[seriesIndex].Values.Length)
                        {
                            var currentSeries = series[seriesIndex];

                            double? value = (axisArea.ReverseHor ?
                                currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] :
                                currentSeries.Values[pointIndex]);

                                                       
                            if (value != null)
                            {
                                if (axisArea.ReverseVert) value = -value;
                                if (value > 0) posYMax += (float)value;
                                else posYMin += (float)value;
                            }
                        }
                    }

                    #region Full-Stacked
                    if (this is StiFullStackedColumnSeriesCoreXF)
                    {
                        double totalPositiveHeight2 = axisArea.ReverseVert ? totalNegativeHeight : totalPositiveHeight;
                        double totalNegativeHeight2 = axisArea.ReverseVert ? totalPositiveHeight : totalNegativeHeight;

                        if (totalPositiveHeight2 > 0 && totalNegativeHeight2 > 0)
                        {
                            var shadowRect = new RectangleF(seriesLeftPos, -8, seriesWidth, rect.Height + 8);
                            var shadowGeom = new StiStackedColumnSeriesShadowElementGeom(series[0], shadowRect, false, false);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(shadowGeom);
                        }
                        else if (totalPositiveHeight2 > 0)
                        {
                            var shadowRect = new RectangleF(seriesLeftPos, -8, seriesWidth, axisArea.AxisCore.GetDividerY() + 8);
                            var shadowGeom = new StiStackedColumnSeriesShadowElementGeom(series[0], shadowRect, false, false);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(shadowGeom);
                        }
                        else if (totalNegativeHeight2 > 0)
                        {
                            var shadowRect = new RectangleF(seriesLeftPos, axisArea.AxisCore.GetDividerY() - 8, seriesWidth, 
                                rect.Height - axisArea.AxisCore.GetDividerY() + 8);

                            var shadowGeom = new StiStackedColumnSeriesShadowElementGeom(series[0], shadowRect, false, false);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(shadowGeom);
                        }
                    }
                    #endregion

                    #region Stacked
                    else
                    {
                        #region IsTopShadow
                        if (posYMax > 0)
                        {
                            RectangleF shadowRect = new RectangleF(seriesLeftPos,
                                (float)(-posYMax * dpiY + posY),
                                seriesWidth, (float)(posYMax * dpiY));

                            var shadowGeom = new StiStackedColumnSeriesShadowElementGeom(series[0], shadowRect, true, false);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(shadowGeom);
                        }
                        #endregion

                        #region IsBottomShadow
                        if (posYMin < 0)
                        {
                            var shadowRect = new RectangleF(seriesLeftPos, posY, seriesWidth, (float)(-posYMin * dpiY));

                            shadowRect.Y -= 7;
                            shadowRect.Height += 7;

                            var shadowGeom = new StiStackedColumnSeriesShadowElementGeom(series[0], shadowRect, false, true);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(shadowGeom);
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                posYMax = 0;
                posYMin = 0;

                var time = StiChartHelper.GlobalBeginTimeElement;
                
                for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
                {
                    var currentSeries = series[seriesIndex] as IStiStackedColumnSeries;
                    
                    if (pointIndex < currentSeries.Values.Length)
                    {
                        int colorIndex = colorArea[pointIndex, seriesIndex].GetValueOrDefault();

                        double? value = axisArea.ReverseHor
                            ? currentSeries.Values[currentSeries.Values.Length - pointIndex - 1]
                            : currentSeries.Values[pointIndex];

                        double? totalValue = axisArea.ReverseHor
                            ? series.Cast<StiSeries>().Sum(s => GetValueFromArray(s.Values, currentSeries.Values.Length - pointIndex - 1))
                            : series.Cast<StiSeries>().Sum(s => GetValueFromArray(s.Values, pointIndex));

                        #region Full-Stacked
                        if (this.Series is IStiFullStackedColumnSeries)
                        {
                            if (!(totalPositiveHeight == 0 && totalNegativeHeight == 0) && value != null)
                            {
                                if (value >= 0)
                                    dpiY = (float)axisArea.AxisCore.GetDividerY() / ((float)totalPositiveHeight);
                                else
                                    dpiY = (float)(rect.Height - axisArea.AxisCore.GetDividerY()) / ((float)totalNegativeHeight);
                            }
                        }
                        #endregion

                        if (axisArea.ReverseVert && value != null) value = -value;

                        float seriesHeight = (float)value.GetValueOrDefault() * dpiY;
                        float seriesTop = -seriesHeight + posY;

                        if (value > 0) seriesTop -= posYMax;
                        else
                        {
                            seriesTop = posYMin + posY;
                            seriesHeight = -seriesHeight;
                        }

                        #region ShowZeros
                        if (columnSeries.ShowZeros && (value == 0 || value == null))
                        {
                            seriesHeight = Math.Max(context.Options.Zoom, 1);
                            if (!axisArea.ReverseVert) seriesTop -= seriesHeight;
                        }
                        #endregion

                        var columnRect = new RectangleF(seriesLeftPos, seriesTop, seriesWidth, seriesHeight);
                        columnRect = CorrectRect(columnRect, rect);

                        RectangleF columnRectStart;
                        if (value < 0)
                            columnRectStart = RectangleF.FromLTRB(columnRect.Left, columnRect.Top, columnRect.Right, columnRect.Top);
                        else
                            columnRectStart = RectangleF.FromLTRB(columnRect.Left, columnRect.Bottom, columnRect.Right, columnRect.Bottom);

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
                            var seriesBrush = currentSeries.Core.GetSeriesBrush(colorIndex, colorCount);
                            if (currentSeries.AllowApplyBrushNegative && (value < 0))
                                seriesBrush = currentSeries.BrushNegative;

                            seriesBrush = currentSeries.ProcessSeriesBrushes(pointIndex, seriesBrush);

                            if (columnRect.Width != 0 && Math.Round((decimal)columnRect.Height, 2) > 0 && seriesBrush != null)
                            {
                                var seriesBorderColor = (Color)currentSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);

                                var seriesColumnGeom = new StiStackedColumnSeriesElementGeom(geom, value.GetValueOrDefault(), pointIndex,
                                        seriesBrush, seriesBorderColor, currentSeries, columnRect, columnRectStart);

                                if (currentSeries.Core.Interaction != null)
                                    seriesColumnGeom.Interaction = new StiSeriesInteractionData(axisArea, currentSeries, pointIndex);

                                seriesList.Add(seriesColumnGeom);
                            }
                            #endregion

                            #region Render Series Labels
                            var labels = currentSeries.Core.GetSeriesLabels();

                            if (labels != null && labels.Visible &&
                                (Math.Round((decimal)columnRect.Height, 2) > 0 || labels.ShowNulls))
                            {
                                var endPoint = new PointF(
                                    columnRect.X + columnRect.Width / 2,
                                    columnRect.Y);

                                var startPoint = new PointF(
                                    columnRect.X + columnRect.Width / 2,
                                    columnRect.Bottom);

                                double? seriesValue = axisArea.ReverseHor ?
                                    currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] : currentSeries.Values[pointIndex];

                                if (axisArea.ReverseVert && seriesValue != null)
                                    seriesValue = -value;

                                if (value < 0)
                                {
                                    startPoint.Y = columnRect.Y;
                                    endPoint.Y = columnRect.Bottom;
                                }

                                if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                                {
                                    var labelValue = value;
                                    if ((currentSeries as StiSeries).IsTotalLabel)
                                        labelValue = seriesValue = totalValue;

                                    int argumentIndex = axisArea.XAxis.StartFromZero ? pointIndex + 1 : pointIndex;
                                    var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                        endPoint, startPoint, pointIndex, labelValue, seriesValue,
                                        axisArea.AxisCore.GetArgumentLabel(axisArea.XAxis.Info.StripLines[argumentIndex], currentSeries),
                                        currentSeries.Core.GetTag(pointIndex),
                                        colorIndex, colorCount, rect);

                                    if (seriesLabelsGeom != null)
                                    {
                                        labelList.Add(seriesLabelsGeom);
                                    }
                                }
                            }
                            #endregion
                        }

                        if (value > 0)
                            posYMax += (float)seriesHeight;
                        else
                            posYMin += (float)seriesHeight;
                    }
                }
            }

            #region Render Series over other geoms
            for (int index = seriesList.Count - 1; index >= 0; index--)
            {
                var seriesGeom = seriesList[index];
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesGeom);
            }
            #endregion

            #region Render Series labels over other geoms
            foreach (var seriesLabelsGeom in labelList)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesLabelsGeom);
                seriesLabelsGeom.ClientRectangle = CheckLabelsRect(seriesLabelsGeom.SeriesLabels, geom, seriesLabelsGeom.ClientRectangle);
            }
            #endregion
        }

        internal static double? GetValueFromArray(double?[] values, int pointIndex)
        {
            if (values == null || pointIndex < 0 || pointIndex >= values.Length)
                return 0;

            return values[pointIndex];
        }

        private void CalculateTotalHeight(IStiSeries[] series, int pointIndex, out double totalPositiveHeight, out double totalNegativeHeight)
        {
            totalPositiveHeight = 0;
            totalNegativeHeight = 0;

            if (this.Series is IStiFullStackedColumnSeries)
            {
                foreach (IStiSeries currentSeries in series)
                {
                    if (pointIndex < currentSeries.Values.Length)
                    {
                        double? value = ((IStiAxisArea)this.Series.Chart.Area).ReverseHor ?
                            currentSeries.Values[currentSeries.Values.Length - pointIndex - 1] :
                            currentSeries.Values[pointIndex];
                                                
                        if (value > 0)
                            totalPositiveHeight += value.GetValueOrDefault();
                        else
                            totalNegativeHeight += Math.Abs(value.GetValueOrDefault());
                    }
                }
            }
        }

        private RectangleF CorrectRect(RectangleF columnRect, RectangleF rect)
        {
            if (columnRect.Y > rect.Height || columnRect.Bottom < 0)
            {
                columnRect.Height = 0;
                return columnRect;
            }

            if (columnRect.Top < 0)
            {
                float dist = -columnRect.Top;
                columnRect.Y += dist;
                columnRect.Height -= dist;
            }
            if (columnRect.Bottom > rect.Height)
            {
                float dist = columnRect.Bottom - rect.Height;
                columnRect.Height -= dist;
            }

            return columnRect;
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            IStiStackedColumnSeries columnSeries = this.Series as IStiStackedColumnSeries;
            
            StiBrush brush = base.GetSeriesBrush(colorIndex, colorCount);
            if (brush == null) return columnSeries.Brush;
            return brush;
        }

        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            IStiStackedColumnSeries columnSeries = this.Series as IStiStackedColumnSeries;

            object color = base.GetSeriesBorderColor(colorIndex, colorCount);
            if (color == null) return columnSeries.BorderColor;
            return color;
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
                return StiLocalization.Get("Chart", "StackedColumn");
            }
        }
        #endregion        

        public StiStackedColumnSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
