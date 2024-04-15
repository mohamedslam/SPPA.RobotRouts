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
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiStackedBarSeriesCoreXF : StiSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            var barSeries = this.Series as IStiStackedBarSeries;

            if (barSeries.AllowApplyStyle)
            {
                barSeries.Brush = style.Core.GetColumnBrush(color);

                if (barSeries.Brush is StiGradientBrush gradientBrush)
                    gradientBrush.Angle += 90;

                if (barSeries.Brush is StiGlareBrush glareBrush)
                    glareBrush.Angle += 90;

                barSeries.BorderColor = style.Core.GetColumnBorder(color);
                barSeries.BorderThickness = style.Core.SeriesBorderThickness;
                barSeries.CornerRadius = style.Core.SeriesCornerRadius;
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            List<StiSeriesLabelsGeom> labelList = new List<StiSeriesLabelsGeom>();
            List<StiStackedBarSeriesElementGeom> seriesList = new List<StiStackedBarSeriesElementGeom>();

            IStiArea area = geom.Area;
            IStiAxisArea axisArea = area as IStiAxisArea;
            IStiStackedBarSeries barSeries = this.Series as IStiStackedBarSeries;

            float posX = axisArea.AxisCore.GetDividerX();

            float dpiX = (float)axisArea.XAxis.Info.Dpi;
            int pointCount = axisArea.AxisCore.ValuesCount;

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

            for (int pointIndex = 0; pointIndex < pointCount; pointIndex++)
            {
                double totalPositiveWidth = 0;
                double totalNegativeWidth = 0;

                CalculateTotalWidth(series, pointIndex, out totalPositiveWidth, out totalNegativeWidth);

                float posY = axisArea.YAxis.Info.StripPositions[axisArea.YAxis.StartFromZero ?  pointIndex + 1: pointIndex] - axisArea.YAxis.Info.Step / 2;

                float seriesHeight = axisArea.YAxis.Info.Step - axisArea.YAxis.Info.Step * (1f - barSeries.Width);
                float seriesTopPos = posY + (axisArea.YAxis.Info.Step - seriesHeight) / 2;

                float posXMax = 0;
                float posXMin = 0;

                #region DrawShadow
                if (series.Length == 0) return;
                bool showShadow = ((IStiStackedBarSeries)series[0]).ShowShadow;
                // Shadow for animated StackedBar is rendered in StiStackedBarSeriesElementGeom
                if (((StiChart)series[0].Chart).IsAnimation && !series[0].Chart.IsDesigning) showShadow = false;
                
                if (showShadow)
                {
                    for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
                    {
                        if (pointIndex < series[seriesIndex].Values.Length)
                        {
                            IStiSeries currentSeries = series[seriesIndex];

                            double? value = (axisArea.ReverseVert ?
                                currentSeries.Values[pointIndex] :
                                currentSeries.Values[currentSeries.Values.Length - pointIndex - 1]);

                            if (axisArea.ReverseHor && value != null) value = -value;
                            if (value > 0) posXMax += (float)value.GetValueOrDefault();
                            else posXMin += (float)value.GetValueOrDefault();
                        }
                    }

                    #region Full-Stacked
                    if (this.Series is IStiFullStackedBarSeries)
                    {
                        double totalPositiveWidth2 = axisArea.ReverseHor ? totalNegativeWidth : totalPositiveWidth;
                        double totalNegativeWidth2 = axisArea.ReverseHor ? totalPositiveWidth : totalNegativeWidth;

                        if (totalPositiveWidth2 > 0 && totalNegativeWidth2 > 0)
                        {
                            RectangleF shadowRect = new RectangleF(-8, seriesTopPos, rect.Width + 8, seriesHeight);

                            StiStackedBarSeriesShadowElementGeom shadowGeom =
                                new StiStackedBarSeriesShadowElementGeom(series[0], shadowRect, false, false);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(shadowGeom);
                        }
                        else if (totalPositiveWidth2 > 0)
                        {
                            RectangleF shadowRect = new RectangleF(axisArea.AxisCore.GetDividerX() - 8, seriesTopPos,
                                rect.Width - axisArea.AxisCore.GetDividerX() + 8, seriesHeight);

                            StiStackedBarSeriesShadowElementGeom shadowGeom =
                                new StiStackedBarSeriesShadowElementGeom(series[0], shadowRect, false, false);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(shadowGeom);
                        }
                        else if (totalNegativeWidth2 > 0)
                        {
                            RectangleF shadowRect = new RectangleF(-8, seriesTopPos, axisArea.AxisCore.GetDividerX() + 8, seriesHeight);

                            StiStackedBarSeriesShadowElementGeom shadowGeom =
                                new StiStackedBarSeriesShadowElementGeom(series[0], shadowRect, false, false);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(shadowGeom);

                        }
                    }
                    #endregion

                    #region Stacked
                    else
                    {
                        #region IsRightShadow
                        if (posXMax > 0)
                        {
                            RectangleF shadowRect = new RectangleF(
                                (float)(posX), seriesTopPos,
                                (float)(posXMax * dpiX), seriesHeight);

                            shadowRect.X -= 7;
                            shadowRect.Width += 7;

                            StiStackedBarSeriesShadowElementGeom shadowGeom =
                                new StiStackedBarSeriesShadowElementGeom(series[0], shadowRect, false, true);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(shadowGeom);
                        }
                        #endregion

                        #region IsLeftShadow
                        if (posXMin < 0)
                        {
                            RectangleF shadowRect = new RectangleF(posX + posXMin * dpiX, seriesTopPos, (float)(-posXMin * dpiX), seriesHeight);

                            StiStackedBarSeriesShadowElementGeom shadowGeom =
                                new StiStackedBarSeriesShadowElementGeom(series[0], shadowRect, true, false);

                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(shadowGeom);
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                posXMax = 0;
                posXMin = 0;

                bool isAnimationChangingValues = ((StiChart)this.Series.Chart).IsAnimationChangingValues;

                for (int seriesIndex = 0; seriesIndex < series.Length; seriesIndex++)
                {
                    var currentSeries = series[seriesIndex] as IStiStackedBarSeries;
                    if (pointIndex < currentSeries.Values.Length)
                    {
                        int colorIndex = colorArea[pointIndex, seriesIndex].GetValueOrDefault();

                        double? originalValue = (axisArea.ReverseVert ?
                            currentSeries.Values[pointIndex] :
                            currentSeries.Values[currentSeries.Values.Length - pointIndex - 1]);

                        double? value = originalValue != null ? -originalValue : originalValue;

                        double? totalValue = 0;
                        var indexTotalValue = axisArea.ReverseHor ? pointIndex : currentSeries.Values.Length - pointIndex - 1;
                        foreach (var curSeries in series)
                        {
                            if (curSeries.Values.Length > indexTotalValue)
                                totalValue += curSeries.Values[indexTotalValue];
                        }

                        #region Full-Stacked
                        if (this.Series is IStiFullStackedBarSeries)
                        {
                            if (!(totalPositiveWidth == 0 && totalNegativeWidth == 0) && value != null)
                            {
                                if (value > 0)
                                    dpiX = (float)axisArea.AxisCore.GetDividerX() / ((float)totalNegativeWidth);
                                else
                                    dpiX = (float)(rect.Width - axisArea.AxisCore.GetDividerX()) / ((float)totalPositiveWidth);
                            }
                        }
                        #endregion

                        if (axisArea.ReverseHor && value != null) value = -value;

                        float seriesWidth = (float)(value.GetValueOrDefault() * dpiX);
                        float seriesLeftPos = -seriesWidth + posX;

                        if (value > 0) seriesLeftPos -= posXMax;
                        else
                        {
                            seriesLeftPos = posXMin + posX;
                            seriesWidth = -seriesWidth;
                        }

                        #region ShowZeros
                        if (currentSeries.ShowZeros && (value == 0 || value == null))
                        {
                            seriesWidth = Math.Max(context.Options.Zoom, 1);
                            if (!axisArea.ReverseHor) seriesLeftPos -= seriesWidth;
                        }
                        #endregion

                        RectangleF columnRect = new RectangleF(
                            (float)seriesLeftPos, (float)seriesTopPos,
                            (float)seriesWidth, (float)seriesHeight);

                        columnRect = CorrectRect(columnRect, rect);

                        RectangleF columnRectStart;
                        if (value < 0)
                            columnRectStart = RectangleF.FromLTRB(columnRect.Left, columnRect.Top, columnRect.Left, columnRect.Bottom);
                        else
                            columnRectStart = RectangleF.FromLTRB(columnRect.Right, columnRect.Top, columnRect.Right, columnRect.Bottom);

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

                        if ((columnRectCheck.Bottom > clipRect.Y && columnRectCheck.Y < clipRect.Bottom) || ((IStiAxisArea)this.Series.Chart.Area).YAxis.Range.Auto)
                        {
                            #region Draw Column
                            StiBrush seriesBrush = currentSeries.Core.GetSeriesBrush(colorIndex, colorCount);
                            if (currentSeries.AllowApplyBrushNegative && (value > 0))
                                seriesBrush = currentSeries.BrushNegative;
                            seriesBrush = currentSeries.ProcessSeriesBrushes((pointCount - 1) - pointIndex, seriesBrush);

                            if (columnRect.Height != 0 && Math.Round((decimal)columnRect.Width, 2) > 0 && seriesBrush != null)
                            {
                                var seriesBorderColor = (Color)currentSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);

                                var seriesColumnGeom = new StiStackedBarSeriesElementGeom(geom, value.GetValueOrDefault(), pointIndex,
                                        seriesBrush, seriesBorderColor, currentSeries, columnRect, columnRectStart);

                                if (currentSeries.Core.Interaction != null)
                                    seriesColumnGeom.Interaction = new StiSeriesInteractionData(axisArea, currentSeries, (pointCount - 1) - pointIndex);

                                seriesList.Add(seriesColumnGeom);
                            }
                            #endregion

                            #region Render Series Labels
                            IStiAxisSeriesLabels labels = currentSeries.Core.GetSeriesLabels();

                            if (labels != null &&
                                labels.Visible &&
                                Math.Round((decimal)columnRect.Width, 2) > 0)
                            {
                                PointF endPoint = new PointF(
                                    columnRect.X,
                                    columnRect.Y + columnRect.Height / 2);

                                PointF startPoint = new PointF(
                                    columnRect.Right,
                                    columnRect.Y + columnRect.Height / 2);

                                if (value < 0)
                                {
                                    startPoint.X = columnRect.X;
                                    endPoint.X = columnRect.Right;
                                }

                                if (value < 0)
                                {
                                    startPoint.X = columnRect.X;
                                    endPoint.X = columnRect.Right;
                                }

                                if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                                {


                                    var labelValue = value;
                                    if ((currentSeries as StiSeries).IsTotalLabel)
                                        labelValue = originalValue = totalValue;

                                    var argumentIndex = axisArea.YAxis.StartFromZero ? pointIndex + 1 : pointIndex;
                                    var stripLine = axisArea.YAxis.Info.StripLines.Count > argumentIndex ?
                                        axisArea.YAxis.Info.StripLines[argumentIndex] : null;

                                    var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currentSeries, context,
                                        endPoint, startPoint,
                                        pointIndex, labelValue, originalValue,
                                        axisArea.AxisCore.GetArgumentLabel(stripLine, currentSeries),
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

                        if (value > 0) posXMax += (float)seriesWidth;
                        else posXMin += (float)seriesWidth;
                    }
                }
            }

            #region Render labels over other geoms
            for (int index = seriesList.Count - 1; index >= 0; index--)
            {
                StiStackedBarSeriesElementGeom seriesGeom = seriesList[index];
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesGeom);
            }
            #endregion

            #region Render Series labels over other geoms
            foreach (StiSeriesLabelsGeom seriesLabelsGeom in labelList)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesLabelsGeom);
                seriesLabelsGeom.ClientRectangle = CheckLabelsRect(seriesLabelsGeom.SeriesLabels, geom, seriesLabelsGeom.ClientRectangle);
            }
            #endregion
        }

        private void CalculateTotalWidth(IStiSeries[] series, int pointIndex, out double totalPositiveWidth, out double totalNegativeWidth)
        {
            totalPositiveWidth = 0;
            totalNegativeWidth = 0;

            if (this.Series is IStiFullStackedBarSeries)
            {
                foreach (IStiSeries currentSeries in series)
                {
                    if (pointIndex < currentSeries.Values.Length)
                    {
                        double? value = ((IStiAxisArea)this.Series.Chart.Area).ReverseVert ?
                            currentSeries.Values[pointIndex] :
                            currentSeries.Values[currentSeries.Values.Length - pointIndex - 1];

                        if (value > 0)
                            totalPositiveWidth += value.GetValueOrDefault();
                        else
                            totalNegativeWidth += Math.Abs(value.GetValueOrDefault());
                    }
                }
            }
        }

        private RectangleF CorrectRect(RectangleF columnRect, RectangleF rect)
        {
            if (columnRect.X > rect.Width || columnRect.Right < 0)
            {
                columnRect.Width = 0;
                return columnRect;
            }

            if (columnRect.X < 0)
            {
                float dist = -columnRect.X;
                columnRect.X += dist;
                columnRect.Width -= dist;
            }
            if (columnRect.Right > rect.Width)
            {
                float dist = columnRect.Right - rect.Width;
                columnRect.Width -= dist;
            }

            return columnRect;
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            IStiStackedBarSeries barSeries = this.Series as IStiStackedBarSeries;

            StiBrush brush = base.GetSeriesBrush(colorIndex, colorCount);
            if (brush == null) return barSeries.Brush;
            return brush;
        }

        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            IStiStackedBarSeries barSeries = this.Series as IStiStackedBarSeries;

            object color = base.GetSeriesBorderColor(colorIndex, colorCount);
            if (color == null) return barSeries.BorderColor;
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
                return StiLocalization.Get("Chart", "StackedBar");
            }
        }
        #endregion        

        public StiStackedBarSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
