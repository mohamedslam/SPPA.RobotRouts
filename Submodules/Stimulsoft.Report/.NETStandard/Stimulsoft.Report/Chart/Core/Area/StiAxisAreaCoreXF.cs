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
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiAxisAreaCoreXF : StiAreaCoreXF
	{
        #region IStiApplyStyle
        /// <summary>
        /// Applying specified style to this area.
        /// </summary>
        /// <param name="style"></param>
        public override void ApplyStyle(IStiChartStyle style)
        {
            base.ApplyStyle(style);

            var axisArea = this.Area as IStiAxisArea;

            if (axisArea.AllowApplyStyle)
            {
                if (axisArea.InterlacingHor != null) axisArea.InterlacingHor.Core.ApplyStyle(style);
                if (axisArea.InterlacingVert != null) axisArea.InterlacingVert.Core.ApplyStyle(style);

                if (axisArea.GridLinesHor != null) axisArea.GridLinesHor.Core.ApplyStyle(style);
                if (axisArea.GridLinesVert != null) axisArea.GridLinesVert.Core.ApplyStyle(style);

                if (axisArea.XAxis != null) axisArea.XAxis.Core.ApplyStyle(style);
                if (axisArea.XTopAxis != null) axisArea.XTopAxis.Core.ApplyStyle(style);
                if (axisArea.YAxis != null) axisArea.YAxis.Core.ApplyStyle(style);
                if (axisArea.YRightAxis != null) axisArea.YRightAxis.Core.ApplyStyle(style);
            }
        }
        #endregion       

        #region Methods
        public override StiCellGeom Render(StiContext context, RectangleF rect)
        {
            var axisArea = this.Area as IStiAxisArea;
            axisArea.YAxis.Info.Minimum = 0;
            axisArea.YAxis.Info.Maximum = 0;
            axisArea.YRightAxis.Info.Minimum = 0;
            axisArea.YRightAxis.Info.Maximum = 0;
            axisArea.XAxis.Info.Minimum = 0;
            axisArea.XAxis.Info.Maximum = 0;
            axisArea.XTopAxis.Info.Minimum = 0;
            axisArea.XTopAxis.Info.Maximum = 0;
            
            PrepareInfo(rect);//First path, calculation for using in Axis
                        
            #region Axis View Geom's
            StiYAxisViewGeom axisYLeftViewGeom = null;
            StiYAxisViewGeom axisYCenterViewGeom = null;
            StiYAxisViewGeom axisYRightViewGeom = null;
            StiXAxisViewGeom axisXTopViewGeom = null;
            StiXAxisViewGeom axisXCenterViewGeom = null;
            StiXAxisViewGeom axisXBottomViewGeom = null;
            #endregion

            #region Axis Geom's
            StiYAxisGeom axisYLeftGeom = null;
            StiYAxisGeom axisYCenterGeom = null;
            StiYAxisGeom axisYRightGeom = null;
            StiXAxisGeom axisXTopGeom = null;
            StiXAxisGeom axisXCenterGeom = null;
            StiXAxisGeom axisXBottomGeom = null;
            #endregion

            #region Axis Rect's
            RectangleF rectYLeftAxis = RectangleF.Empty;
            RectangleF rectYCenterAxis = RectangleF.Empty;
            RectangleF rectYRightAxis = RectangleF.Empty;
            RectangleF rectXTopAxis = RectangleF.Empty;
            RectangleF rectXCenterAxis = RectangleF.Empty;
            RectangleF rectXBottomAxis = RectangleF.Empty;
            #endregion

            RectangleF viewRect = RectangleF.Empty;

            #region Render Axis
            var seriesCollection = axisArea.Core.GetSeries();
            if (seriesCollection.Count > 0)
            {
                float distTop = 0;
                float distBottom = 0;
                float distLeft = 0;
                float distRight = 0;

                #region Calculate Axis Locations
                #region YLeftAxis
                rectYLeftAxis = axisArea.ReverseHor?
                    ((StiYRightAxisCoreXF)axisArea.YRightAxis.Core).GetAxisRect(context, rect, true, true, false, true) :
                    ((StiYAxisCoreXF)axisArea.YAxis.Core).GetAxisRect(context, rect, true, true, false, true);

                #region Legend Table Chart
                if (this.Area.Chart.Table.Core.ShowTable())
                {
                    float widhtLegendTableChart = this.Area.Chart.Table.Core.GetWidthCellLegend(context);
                    if (widhtLegendTableChart > rectYLeftAxis.Width)
                    {
                        rectYLeftAxis.Width = widhtLegendTableChart;
                        rectYLeftAxis.X = -widhtLegendTableChart;
                        if (rectYLeftAxis.Height == 0)
                            rectYLeftAxis.Height = 1;
                    }
                }
                #endregion

                if (!rectYLeftAxis.IsEmpty)
                {
                    rectYLeftAxis.X += rect.X;
                    rectYLeftAxis.Y += rect.Y;
                }                
                if (!rectYLeftAxis.IsEmpty)
                {
                    distLeft = rectYLeftAxis.Width;
                    if (rectYLeftAxis.Top < rect.Top)
                        distTop = Math.Abs(rect.Top - rectYLeftAxis.Top);

                    if (rect.Bottom < rectYLeftAxis.Bottom)
                        distBottom = Math.Abs(rectYLeftAxis.Bottom - rect.Bottom);
                }
                #endregion

                #region YCenterAxis
                rectYCenterAxis = ((StiYAxisCoreXF)axisArea.YAxis.Core).GetCenterAxisRect(context, rect, true, true, false);
                if (!rectYCenterAxis.IsEmpty)
                {
                    rectYCenterAxis.X += rect.X;
                    rectYCenterAxis.Y += rect.Y;
                }
                #endregion

                #region YRightAxis
                rectYRightAxis = axisArea.ReverseHor ?
                    ((StiYAxisCoreXF)axisArea.YAxis.Core).GetAxisRect(context, rect, true, true, false, true):
                    ((StiYRightAxisCoreXF)axisArea.YRightAxis.Core).GetAxisRect(context, rect, true, true, false, true);
                if (!rectYRightAxis.IsEmpty)
                {
                    rectYRightAxis.X += rect.X;
                    rectYRightAxis.Y += rect.Y;
                }
                if (!rectYRightAxis.IsEmpty)
                {
                    distRight = rectYRightAxis.Width;
                    if (rectYRightAxis.Top < rect.Top)
                        distTop = Math.Max(distTop, Math.Abs(rect.Top - rectYRightAxis.Top));

                    if (rect.Bottom < rectYRightAxis.Bottom)
                        distBottom = Math.Max(distBottom, Math.Abs(rectYRightAxis.Bottom - rect.Bottom));
                }
                #endregion

                #region XTopAxis
                rectXTopAxis = ((StiXTopAxisCoreXF)axisArea.XTopAxis.Core).GetAxisRect(context, rect, true, true, false, true);
                if (!rectXTopAxis.IsEmpty)
                {
                    rectXTopAxis.X += rect.X;
                    rectXTopAxis.Y += rect.Y;
                }
                if (!rectXTopAxis.IsEmpty)
                {
                    distTop = Math.Max(rectXTopAxis.Height, distTop);
                    if (rectXTopAxis.X < rect.X)
                        distLeft = Math.Max(distLeft, Math.Abs(rect.Left - rectXTopAxis.Left));

                    if (rect.Right < rectXTopAxis.Right)
                        distRight = Math.Max(distRight, Math.Abs(rectXTopAxis.Right - rect.Right));
                }
                #endregion

                #region XCenterAxis
                rectXCenterAxis = ((StiXAxisCoreXF)axisArea.XAxis.Core).GetCenterAxisRect(context, rect, true, true, false);
                if (!rectXCenterAxis.IsEmpty)
                {
                    rectXCenterAxis.X += rect.X;
                    rectXCenterAxis.Y += rect.Y;
                }
                #endregion

                #region XBottomAxis
                #region Prepare Info for Auto Rotation Mode

                if (axisArea.ReverseHor)
                    Swap(ref distLeft, ref distRight);

                var rectWithoutYAxis = new RectangleF(rect.X + distLeft, rect.Y, rect.Width - distLeft - distRight, rect.Height);
                axisArea.XAxis.Info.Dpi = (double)rectWithoutYAxis.Width / axisArea.XAxis.Info.Range;
                CalculateStepX(axisArea.XAxis, rectWithoutYAxis.Left, rectWithoutYAxis.Right);
                CalculatePositions(axisArea.XAxis, ref axisArea.XAxis.Info.LabelsCollection, axisArea.XAxis.Labels.Step > 0 ? (int)axisArea.XAxis.Labels.Step : 1);
                #endregion

                rectXBottomAxis = ((StiXAxisCoreXF)axisArea.XAxis.Core).GetAxisRect(context, rectWithoutYAxis, true, true, false, true);
                if (!rectXBottomAxis.IsEmpty && !this.Area.Chart.Table.Core.ShowTable())
                {
                    rectXBottomAxis.X += rect.X;
                    rectXBottomAxis.Y += rect.Y;
                }
                if (!rectXBottomAxis.IsEmpty && !(this.Area.Chart.Table.Core.ShowTable() && !(this.Area.Chart.Table.Chart.Area is IStiClusteredBarArea)))
                {
                    distBottom = Math.Max(rectXBottomAxis.Height, distBottom);
                    if (rectXBottomAxis.X < rect.X)
                        distLeft = Math.Max(distLeft, Math.Abs(rect.Left - rectXBottomAxis.Left));

                    if (rect.Right < rectXBottomAxis.Right)
                        distRight = Math.Max(distRight, Math.Abs(rectXBottomAxis.Right - rect.Right));
                }
                #endregion
                #endregion

                if (axisArea.ReverseHor)
                    Swap(ref distLeft, ref distRight);
                if (axisArea.ReverseVert)
                    Swap(ref distTop, ref distBottom);

                #region Add Destination
                rect.X += distLeft;
                rect.Width -= distLeft + distRight;
                rect.Y += distTop;
                rect.Height -= distTop + distBottom;
                #endregion

                PrepareInfo(rect);//Second path, calculation for area without axis

                #region Render Axis Geoms
                axisYLeftViewGeom = axisArea.YAxis.Core.RenderView(context, rect) as StiYAxisViewGeom;
                axisYCenterViewGeom = ((StiYAxisCoreXF)axisArea.YAxis.Core).RenderCenterView(context, rect) as StiYAxisViewGeom;
                axisYRightViewGeom = axisArea.YRightAxis.Core.RenderView(context, rect) as StiYAxisViewGeom;

                axisXTopViewGeom = axisArea.XTopAxis.Core.RenderView(context, rect) as StiXAxisViewGeom;
                axisXCenterViewGeom = ((StiXAxisCoreXF)axisArea.XAxis.Core).RenderCenterView(context, rect) as StiXAxisViewGeom;
                axisXBottomViewGeom = axisArea.XAxis.Core.RenderView(context, rect) as StiXAxisViewGeom;
                #endregion
            }
                                
            viewRect = rect;

            double scrollFactorX = CalculateScrollValuesX(rect, axisArea);
            double scrollFactorY = CalculateScrollValuesY(rect, axisArea);            

            rect.X = -(float)ScrollDistanceX;
            rect.Y = -(float)ScrollDistanceY;
            rect.Width *= (float)scrollFactorX;
            rect.Height *= (float)scrollFactorY;
            

            PrepareInfo(rect);//Third path, final calculation

            if (seriesCollection.Count > 0)
            {
                #region Render Axis Geoms
                axisYLeftGeom = axisArea.YAxis.Core.Render(context, rect) as StiYAxisGeom;
                axisYCenterGeom = ((StiYAxisCoreXF)axisArea.YAxis.Core).RenderCenter(context, rect) as StiYAxisGeom;
                axisYRightGeom = axisArea.YRightAxis.Core.Render(context, rect) as StiYAxisGeom;

                axisXTopGeom = axisArea.XTopAxis.Core.Render(context, rect) as StiXAxisGeom;
                axisXCenterGeom = ((StiXAxisCoreXF)axisArea.XAxis.Core).RenderCenter(context, rect) as StiXAxisGeom;
                axisXBottomGeom = axisArea.XAxis.Core.Render(context, rect) as StiXAxisGeom;
                #endregion

                if (axisYLeftGeom != null) axisYLeftGeom.View = axisYLeftViewGeom;
                if (axisYCenterGeom != null) axisYCenterGeom.View = axisYCenterViewGeom;
                if (axisYRightGeom != null) axisYRightGeom.View = axisYRightViewGeom;

                if (axisXTopGeom != null) axisXTopGeom.View = axisXTopViewGeom;
                if (axisXCenterGeom != null) axisXCenterGeom.View = axisXCenterViewGeom;
                if (axisXBottomGeom != null) axisXBottomGeom.View = axisXBottomViewGeom;
            }
            #endregion  
            
            #region Render Scroll Bar's
            if (axisXBottomViewGeom != null)
                ((StiXAxisCoreXF)axisArea.XAxis.Core).RenderScrollBar(context, axisXBottomViewGeom.ClientRectangle, axisXBottomViewGeom);

            if (axisXTopViewGeom != null)
                ((StiXAxisCoreXF)axisArea.XTopAxis.Core).RenderScrollBar(context, axisXTopViewGeom.ClientRectangle, axisXTopViewGeom);

            if (axisYLeftViewGeom != null)
                ((StiYAxisCoreXF)axisArea.YAxis.Core).RenderScrollBar(context, axisYLeftViewGeom.ClientRectangle, axisYLeftViewGeom);

            if (axisYRightViewGeom != null)
                ((StiYAxisCoreXF)axisArea.YRightAxis.Core).RenderScrollBar(context, axisYRightViewGeom.ClientRectangle, axisYRightViewGeom);
            #endregion

            var viewGeom = new StiAxisAreaViewGeom(axisArea, viewRect);
            var geom = new StiAxisAreaGeom(viewGeom, axisArea, rect);
            viewGeom.CreateChildGeoms();
            viewGeom.ChildGeoms.Add(geom);

            #region Render Strips (Behind)
            foreach (IStiStrips strip in this.Area.Chart.Strips)
            {
                if (strip.ShowBehind) strip.Core.Render(context, geom, rect);
            }
            #endregion

            #region Render Constant Lines (Behind)
            foreach (IStiConstantLines line in this.Area.Chart.ConstantLines)
            {
                if (line.ShowBehind) line.Core.Render(context, geom, rect);
            }
            #endregion

            #region Render Series
            RenderSeries(context, rect, geom, seriesCollection);
            #endregion 
            
            #region Render Strips
            foreach (IStiStrips strip in this.Area.Chart.Strips)
            {
                if (!strip.ShowBehind) strip.Core.Render(context, geom, rect);
            }
            #endregion

            #region Render Constant Lines
            foreach (IStiConstantLines line in this.Area.Chart.ConstantLines)
            {
                if (!line.ShowBehind) line.Core.Render(context, geom, rect);
            }
            #endregion

            #region Add Axis Geoms
            if (axisYLeftGeom != null)
            {
                viewGeom.CreateChildGeoms();
                axisYLeftViewGeom.CreateChildGeoms();
                                
                viewGeom.ChildGeoms.Add(axisYLeftViewGeom);
                axisYLeftViewGeom.ChildGeoms.Add(axisYLeftGeom);
            }

            if (axisYCenterGeom != null)
            {
                viewGeom.CreateChildGeoms();
                axisYCenterViewGeom.CreateChildGeoms();

                viewGeom.ChildGeoms.Add(axisYCenterViewGeom);
                axisYCenterViewGeom.ChildGeoms.Add(axisYCenterGeom);
            }

            if (axisYRightGeom != null)
            {
                viewGeom.CreateChildGeoms();
                axisYRightViewGeom.CreateChildGeoms();

                viewGeom.ChildGeoms.Add(axisYRightViewGeom);
                axisYRightViewGeom.ChildGeoms.Add(axisYRightGeom);
            }

            if (axisXTopGeom != null)
            {
                viewGeom.CreateChildGeoms();
                axisXTopViewGeom.CreateChildGeoms();

                viewGeom.ChildGeoms.Add(axisXTopViewGeom);
                axisXTopViewGeom.ChildGeoms.Add(axisXTopGeom);
            }

            if (axisXCenterGeom != null)
            {
                viewGeom.CreateChildGeoms();
                axisXCenterViewGeom.CreateChildGeoms();

                viewGeom.ChildGeoms.Add(axisXCenterViewGeom);
                axisXCenterViewGeom.ChildGeoms.Add(axisXCenterGeom);
            }

            if (axisXBottomGeom != null && !(this.Area.Chart.Table.Core.ShowTable() && !(this.Area.Chart.Table.Chart.Area is IStiClusteredBarArea)))
            {
                viewGeom.CreateChildGeoms();
                axisXBottomViewGeom.CreateChildGeoms();

                viewGeom.ChildGeoms.Add(axisXBottomViewGeom);
                axisXBottomViewGeom.ChildGeoms.Add(axisXBottomGeom);
            }
            #endregion

            return viewGeom;
        }

        private double CalculateScrollValuesX(RectangleF rect, IStiAxisArea axisArea)
        {
            double minimum;
            double maximum;

            double scrollFactorX = 1;

            #region Calculate minimum and maximum values
            if (!IsAutoRangeXAxis(axisArea.XAxis))
            {
                minimum = axisArea.XAxis.Range.Minimum;
                maximum = axisArea.XAxis.Range.Maximum;
                
                if (minimum > maximum)
                {
                    minimum = axisArea.XAxis.Info.Minimum;
                    maximum = axisArea.XAxis.Info.Maximum;
                }
            }
            else
            {
                minimum = axisArea.XAxis.Info.Minimum;
                maximum = axisArea.XAxis.Info.Maximum;
            }
            #endregion

            this.scrollRangeX = axisArea.XAxis.Info.Maximum - axisArea.XAxis.Info.Minimum;
            this.scrollViewX = maximum - minimum;

            if (!axisArea.XAxis.Core.GetStartFromZero() || axisArea.XAxis.LogarithmicScale)//Block in an case for StartFromZero not true
                BlockScrollValueX = true;
            else
                BlockScrollValueX = false;

            if (!BlockScrollValueX && !axisArea.XAxis.Interaction.ShowScrollBar)
            {
                double value = minimum + Math.Abs(axisArea.XAxis.Info.Minimum);

                //Artem: if set range not correct set first value in Scatter Series
                if (axisArea.XAxis.Info.Minimum > 0 && minimum > 0)
                    value = Math.Abs(minimum - axisArea.XAxis.Info.Minimum);

                if (!axisArea.ReverseHor)
                    this.ScrollValueX = value;
                else
                    this.ScrollValueX = scrollRangeX - value - scrollViewX;
            }

            scrollFactorX = ScrollRangeX / ScrollViewX;
            this.scrollDpiX = rect.Width * scrollFactorX / ScrollRangeX;

            return scrollFactorX;
        }


        private double CalculateScrollValuesY(RectangleF rect, IStiAxisArea axisArea)
        {
            double minimum;
            double maximum;

            double scrollFactorY = 1;

            #region Calculate minimum and maximum values
            if (!IsAutoRangeYAxis(axisArea.YAxis))
            {
                minimum = axisArea.YAxis.Range.Minimum;
                maximum = axisArea.YAxis.Range.Maximum;
                
                if (minimum > maximum)
                {
                    minimum = axisArea.YAxis.Info.Minimum;
                    maximum = axisArea.YAxis.Info.Maximum;
                }
            }
            else
            {
                minimum = axisArea.YAxis.Info.Minimum;
                maximum = axisArea.YAxis.Info.Maximum;
            }
            #endregion            

            this.scrollRangeY = axisArea.YAxis.Info.Maximum - axisArea.YAxis.Info.Minimum;
            this.scrollViewY = maximum - minimum;

            if (!axisArea.YAxis.Core.GetStartFromZero() || axisArea.YAxis.LogarithmicScale)//Block in an case for StartFromZero not true
                BlockScrollValueY = true;
            else
                BlockScrollValueY = false;

            if (!BlockScrollValueY)
            {
                double value = minimum + Math.Abs(axisArea.YAxis.Info.Minimum);

                if (axisArea.ReverseVert)
                    this.ScrollValueY = value;
                else
                    this.ScrollValueY = scrollRangeY - value - scrollViewY;
            }
            else
            {
                this.ScrollValueY = 0;
            }

            scrollFactorY = ScrollRangeY / ScrollViewY;
            this.scrollDpiY = rect.Height * scrollFactorY / ScrollRangeY;

            return scrollFactorY;
        }        
        
        protected override void PrepareInfo(RectangleF rect)
        {
            var seriesCollection = GetSeries();
            if (seriesCollection.Count > 0)
            {
                var axisArea = this.Area as IStiAxisArea;

                ValuesCount = 0;

                var specXAxis = (this is StiClusteredBarAreaCoreXF) ? axisArea.YAxis as IStiAxis : axisArea.XAxis as IStiAxis;
                var specXTopAxis = (this is StiClusteredBarAreaCoreXF) ? axisArea.YRightAxis as IStiAxis : axisArea.XTopAxis as IStiAxis;
                var specYAxis = (this is StiClusteredBarAreaCoreXF) ? axisArea.XAxis as IStiAxis : axisArea.YAxis as IStiAxis;
                var specYRightAxis = (this is StiClusteredBarAreaCoreXF) ? axisArea.XTopAxis as IStiAxis : axisArea.YRightAxis as IStiAxis;

                specXAxis.Info.StripLines = new StiStripLinesXF();
                specYAxis.Info.StripLines = new StiStripLinesXF();
                specYRightAxis.Info.StripLines = new StiStripLinesXF();

                #region Check IsDateTimeValues
                bool isDateTimeValues = false;
                for (int index = 0; index < seriesCollection.Count; index++)
                {
                    double?[] values = seriesCollection[index].Values;
                    if (seriesCollection[index] is StiCandlestickSeries)
                        values = ((StiCandlestickSeries)seriesCollection[index]).ValuesOpen;
                    if (values != null)
                    {
                        ValuesCount = Math.Max(values.Length, ValuesCount);
                    }

                    if (seriesCollection[index].Core.IsDateTimeValues) isDateTimeValues = true;
                }
                #endregion

                if (this is StiBoxAndWhiskerAreaCoreXF)
                    ValuesCount = seriesCollection.Count;

                PrepareRange(specXAxis, specXTopAxis, specYAxis, specYRightAxis);

                if (!specYAxis.Range.Auto && specYAxis.Range.Maximum > specYAxis.Info.Maximum)
                    specYAxis.Info.Maximum = specYAxis.Range.Maximum;

                if (!specYAxis.Range.Auto && specYAxis.Range.Minimum < specYAxis.Info.Minimum)
                    specYAxis.Info.Minimum = specYAxis.Range.Minimum;

                if (!specYRightAxis.Range.Auto && (specYRightAxis.Range.Maximum > specYRightAxis.Info.Maximum || axisArea is StiParetoArea))
                    specYRightAxis.Info.Maximum = specYRightAxis.Range.Maximum;

                if (!specYRightAxis.Range.Auto && specYRightAxis.Range.Minimum < specYRightAxis.Info.Minimum)
                    specYRightAxis.Info.Minimum = specYRightAxis.Range.Minimum;

                bool isScatterSeries = this.Area.Chart.Series.Count > 0 && (this.Area.Chart.Series[0] is StiScatterSeries);

                if (!(specYAxis.LogarithmicScale && isScatterSeries))
                {
                    CheckStartFromZeroYAxis(specYAxis);
                    CheckStartFromZeroYAxis(specYRightAxis);

                    CalculateMinimumAndMaximumYAxis(specYAxis);
                    if (specYRightAxis.Range.Auto)
                        CalculateMinimumAndMaximumYAxis(specYRightAxis);
                }
                
                CalculateMinimumAndMaximumXAxis(specXAxis);

                CreateStripLinesXAxis(specXAxis);
                CreateStripLinesYAxis(specYAxis, isDateTimeValues);
                CreateStripLinesYAxis(specYRightAxis, isDateTimeValues);

                CheckStripLinesAndMaximumMinimumXAxis(specXAxis);
                CheckStripLinesAndMaximumMinimumYAxis(specYAxis);
                CheckStripLinesAndMaximumMinimumYAxis(specYRightAxis);

                if (axisArea is IStiClusteredBarArea)
                {
                    RotateStripLines(specXAxis);
                    RotateStripLines(specYAxis);
                    RotateStripLines(specYRightAxis);
                }

                specXTopAxis.Info = specXAxis.Info.Clone() as StiAxisInfoXF;

                CheckShowEdgeValues(specXAxis);
                CheckShowEdgeValues(specXTopAxis);

                if (axisArea.ReverseHor)
                {
                    RotateStripLines(axisArea.XAxis);
                    RotateStripLines(axisArea.XTopAxis);
                }

                if (axisArea.ReverseVert)
                {
                    RotateStripLines(axisArea.YAxis);
                    RotateStripLines(axisArea.YRightAxis);
                }               

                axisArea.XAxis.Info.Dpi = (double)rect.Width / axisArea.XAxis.Info.Range;
                axisArea.XTopAxis.Info.Dpi = (double)rect.Width / axisArea.XTopAxis.Info.Range;
                axisArea.YAxis.Info.Dpi = (double)rect.Height / axisArea.YAxis.Info.Range;
                axisArea.YRightAxis.Info.Dpi = (double)rect.Height / axisArea.YRightAxis.Info.Range;

                CalculateStepX(axisArea.XAxis, rect.Left, rect.Right);
                CalculateStepX(axisArea.XTopAxis, rect.Left, rect.Right);
                CalculateStepY(axisArea.YAxis, rect.Top, rect.Bottom);
                CalculateStepY(axisArea.YRightAxis, rect.Top, rect.Bottom);

                CalculatePositions(axisArea.XAxis, ref axisArea.XAxis.Info.LabelsCollection, axisArea.XAxis.Labels.Step > 0 ? (int)axisArea.XAxis.Labels.Step : 1);
                CalculatePositions(axisArea.XTopAxis, ref axisArea.XTopAxis.Info.LabelsCollection, axisArea.XTopAxis.Labels.Step > 0 ? (int)axisArea.XTopAxis.Labels.Step : 1);
                CalculatePositions(axisArea.YAxis, ref axisArea.YAxis.Info.LabelsCollection, axisArea.YAxis.Labels.Step > 1 ? (int)axisArea.YAxis.Labels.Step : 1);
                CalculatePositions(axisArea.YRightAxis, ref axisArea.YRightAxis.Info.LabelsCollection, axisArea.YRightAxis.Labels.Step > 0 ? (int)axisArea.YRightAxis.Labels.Step : 1);

                CalculatePositions(axisArea.XAxis, ref axisArea.XAxis.Info.TicksCollection, axisArea.XAxis.Ticks.Step > 0 ? axisArea.XAxis.Ticks.Step : 1, true);
                CalculatePositions(axisArea.XTopAxis, ref axisArea.XTopAxis.Info.TicksCollection, axisArea.XTopAxis.Ticks.Step > 0 ? axisArea.XTopAxis.Ticks.Step : 1, true);
                CalculatePositions(axisArea.YAxis, ref axisArea.YAxis.Info.TicksCollection, axisArea.YAxis.Ticks.Step > 0 ? axisArea.YAxis.Ticks.Step : 1, true);
                CalculatePositions(axisArea.YRightAxis, ref axisArea.YRightAxis.Info.TicksCollection, axisArea.YRightAxis.Ticks.Step > 0 ? axisArea.YRightAxis.Ticks.Step : 1, true);
            }
        }
        
        private void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, List<IStiSeries> seriesCollection)
        {
            var seriesTypes = new List<List<IStiSeries>>();
            var seriesTypesHash = new Hashtable();
            foreach (var ser in seriesCollection)
            {
                if (IsAcceptableSeries(ser.GetType()))
                {
                    var list = seriesTypesHash[ser.GetType()] as List<IStiSeries>;
                    if (list == null)
                    {
                        list = new List<IStiSeries>();
                        seriesTypes.Add(list);
                        seriesTypesHash.Add(ser.GetType(), list);
                    }

                    list.Add(ser);
                }
            }

            foreach (var seriesType in seriesTypes)
            {
                var seriesArray = seriesType.ToArray();

                if (seriesArray[0] is StiStackedBarSeries ||
                    seriesArray[0] is StiStackedColumnSeries ||
                    seriesArray[0] is StiStackedBaseLineSeries)
                {
                    #region Stacked Series for Left and Right YAxis
                    var leftSeries = new List<IStiSeries>();
                    var rightSeries = new List<IStiSeries>();

                    foreach (var series in seriesArray)
                    {
                        if (series.YAxis == StiSeriesYAxis.LeftYAxis)
                            leftSeries.Add(series);
                        else
                            rightSeries.Add(series);
                    }

                    if (leftSeries.Count > 0)
                        leftSeries.ToArray()[0].Core.RenderSeries(context, rect, geom, leftSeries.ToArray());

                    if (rightSeries.Count > 0)
                        rightSeries.ToArray()[0].Core.RenderSeries(context, rect, geom, rightSeries.ToArray());
                    #endregion
                }
                else
                    seriesArray[0].Core.RenderSeries(context, rect, geom, seriesArray);
            }
        }

        /// <summary>
        /// Returns true if specified axis need to be used in auto range mode. Method calculate mode for XAxis.
        /// </summary>
        internal bool IsAutoRangeXAxis(IStiAxis axis)
        {
            return
                axis.Range.Auto ||
                axis.Range.Minimum >= axis.Range.Maximum ||
                axis.LogarithmicScale;
        }

        /// <summary>
        /// Returns true if specified axis need to be used in auto range mode. Method calculate mode for YAxis.
        /// </summary>
        internal bool IsAutoRangeYAxis(IStiAxis axis)
        {
            return
                axis.Range.Auto ||
                axis.Range.Maximum == axis.Range.Minimum ||
                Area.IsDefaultSeriesTypeFullStackedColumnSeries ||
                axis.LogarithmicScale;
        }

        /// <summary>
        /// Calculate minimum and maximum values of XAxis.
        /// </summary>
        internal void CalculateMinimumAndMaximumXAxis(IStiAxis axis)
        {
            if (!axis.Range.Auto)//Will use user-defined values of minimum and maximum values
            {
                axis.Info.Maximum = axis.Range.Maximum;
                axis.Info.Minimum = axis.Range.Minimum;
            }
        }

        internal void CalculateMinimumAndMaximumYAxis(ref double min, ref double max)
        {
            double minDelts = 0.1;
            if (this.GetSeries().Count > 0 && this.GetSeries()[0].SeriesLabels is StiOutsideEndAxisLabels)
                minDelts = 0.15;

            //Increase calculated minimum and maximum values per special factor
            double delta = Math.Min(minDelts, Math.Abs(4 / ((max + min) / 2)));
            max *= 1 + delta * Math.Sign(max);
            min *= 1 - delta * Math.Sign(min);

            //If minimum and maximum values is equal
            if (min == max)
            {
                //if maximum and minimum values equal to zero then use for maximum value 100
                if (max == 0) max = 100;
                else//Also increase maximum for 10% of maximum and descrease minimum for 10% of minimum
                {
                    min -= min * 0.1;
                    max += max * 0.1;
                }
            }

            var step = StiStripLineCalculatorXF.GetInterval(min, max, 6);
            var stripLines = StiStripLineCalculatorXF.GetStripLines(min, max, step, false);

            if (stripLines.Count > 0)
            {
                max = stripLines[0].Value;
                min = stripLines[stripLines.Count - 1].Value;
            }
            else
            {
                max = 100;
                min = 0;
            }

        }

        /// <summary>
        /// Calculate minimum and maximum values of YAxis.
        /// </summary>
        internal void CalculateMinimumAndMaximumYAxis(IStiAxis axis)
        {
            #region For Gantt Series
            if (this.GetSeries().Count > 0 && this.GetSeries()[0] is StiGanttSeries)
            {
                if (!axis.Range.Auto)
                {
                    axis.Info.Maximum = axis.Range.Maximum;
                    axis.Info.Minimum = axis.Range.Minimum;
                }

                return;
            }
            #endregion
            
            if (!axis.StartFromZero && axis.Range.Auto && (axis is IStiYAxis))
            {
                double range = (axis.Info.Maximum - axis.Info.Minimum) * 0.05;

                axis.Info.Minimum -= range;
                axis.Info.Maximum += range;

                if (axis.Info.Minimum < 0 && axis.Info.Minimum + range >= 0) axis.Info.Minimum = 0;
            }
            else
            {
                double minDelts = 0.1;
                if (this.GetSeries().Count > 0 && this.GetSeries()[0].SeriesLabels is StiOutsideEndAxisLabels)
                    minDelts = 0.15;

                //Increase calculated minimum and maximum values per special factor
                double delta = Math.Min(minDelts, Math.Abs(4 / ((axis.Info.Maximum + axis.Info.Minimum) / 2)));
                axis.Info.Maximum *= 1 + delta * Math.Sign(axis.Info.Maximum);
                axis.Info.Minimum *= 1 - delta * Math.Sign(axis.Info.Minimum); 
            }

            //If minimum and maximum values is equal
            if (axis.Info.Minimum == axis.Info.Maximum)
            {
                //if maximum and minimum values equal to zero then use for maximum value 100
                if (axis.Info.Maximum == 0) axis.Info.Maximum = 100;
                else//Also increase maximum for 10% of maximum and descrease minimum for 10% of minimum
                {
                    axis.Info.Minimum -= axis.Info.Minimum * 0.1;
                    axis.Info.Maximum += axis.Info.Maximum * 0.1;
                }
            }
        }

        internal string GetArgumentLabel(StiStripLineXF line, IStiSeries series)
        {
            if (line == null)
                return string.Empty;

            IStiAxisArea axisArea = this.Area as IStiAxisArea;

            if (axisArea.Core.SeriesOrientation == StiChartSeriesOrientation.Vertical)
            {
                return ((StiXAxisCoreXF)axisArea.XAxis.Core).GetLabelText(line, series);
            }
            else
            {
                return ((StiYAxisCoreXF)axisArea.YAxis.Core).GetLabelText(line);
            }
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public void SwitchOff()
        {
            IStiAxisArea axisArea = this.Area as IStiAxisArea;

            axisArea.GridLinesHor.Visible = false;
            axisArea.GridLinesVert.Visible = false;
            axisArea.InterlacingHor.Visible = false;
            axisArea.InterlacingVert.Visible = false;
            axisArea.XAxis.Visible = false;
            axisArea.YAxis.Visible = false;
            axisArea.XTopAxis.Visible = false;
            axisArea.YRightAxis.Visible = false;
            axisArea.YAxis.Range.Auto = true;
            axisArea.XAxis.Range.Auto = true;
            axisArea.XAxis.StartFromZero = true;
            axisArea.GridLinesHor.MinorVisible = false;
            axisArea.GridLinesVert.MinorVisible = false;
            axisArea.GridLinesHorRight.Visible = false;
        }

        /// <summary>
        /// Exchange two values.
        /// </summary>
        private void Swap(ref float value1, ref float value2)
        {
            float temp = value1;
            value1 = value2;
            value2 = temp;
        }

        /// <summary>
        /// Exchange two values.
        /// </summary>
        private void Swap(ref double value1, ref double value2)
        {
            double temp = value1;
            value1 = value2;
            value2 = temp;
        }

        protected virtual void PrepareRange(IStiAxis specXAxis, IStiAxis specXTopAxis, IStiAxis specYAxis, IStiAxis specYRightAxis)
        {
        }

        /// <summary>
        /// Creates Strip lines collection for XAxis.
        /// </summary>
        protected virtual void CreateStripLinesXAxis(IStiAxis axis)
        {
            bool getStartFromZero = axis.Core.GetStartFromZero();
            int fromValue = getStartFromZero ? 0 : 1;
            int toValue = getStartFromZero ? ValuesCount + 1 : ValuesCount;

            for (int index = fromValue; index <= toValue; index++)
            {
                axis.Info.StripLines.Add(index, index);
            }

            var seriesCollection = GetSeries();
            foreach (IStiSeries series in seriesCollection)
            {
                for (int index = 0; index < ValuesCount; index++)
                {
                    object valueObject = null;
                    if (series.Arguments != null && index < series.Arguments.Length)
                    {
                        valueObject = series.Arguments[index];
                        if (valueObject == null) continue;
                    }
                    if (valueObject != null)
                    {
                        int finalIndex = getStartFromZero ? 1 + index : index;
                        if (finalIndex < axis.Info.StripLines.Count)
                            axis.Info.StripLines[finalIndex].ValueObject = valueObject;
                    }
                }
            }
        }

        protected virtual void CheckShowEdgeValues(IStiAxis axis)
        {
            if (axis.Info.StripLines.Count > 0)
            {
                var barArea = axis.Area as IStiClusteredBarArea;
                var showEdgeValues = axis is IStiXTopAxis ? axis.Area.XTopAxis.ShowEdgeValues : axis.Area.XAxis.ShowEdgeValues;
                if (!showEdgeValues || barArea != null)
                {
                    axis.Info.StripLines[0].ValueObject = null;
                    axis.Info.StripLines[axis.Info.StripLines.Count - 1].ValueObject = null;
                }
            }
        }

        /// <summary>
        /// Creates Strip lines collection for YAxis.
        /// </summary>
        protected virtual void CreateStripLinesYAxis(IStiAxis axis, bool isDateTimeValues)
        {
            if (Area.IsDefaultSeriesTypeFullStackedColumnSeries ||
                Area.IsDefaultSeriesTypeFullStackedBarSeries)
            {
                #region Check Positive & Negative values
                bool positivePresent = false;
                bool negativePresent = false;

                var seriesCollection = this.GetSeries();
                foreach (IStiSeries series in seriesCollection)
                {
                    foreach (double? value in series.Values)
                    {
                        if (value > 0)
                            positivePresent = true;
                        if (value < 0)
                            negativePresent = true;
                    }
                }
                #endregion                

                double minimum = negativePresent ? -100 : 0;
                double maximum = positivePresent ? 100 : 0;

                if (minimum == 0 && maximum == 0)
                {
                    maximum = 100;
                }

                double step = axis.Labels.Step;
                if (step == 0) step = StiStripLineCalculatorXF.GetInterval(minimum, maximum, 6);
                axis.Info.StripLines = StiStripLineCalculatorXF.GetStripLines(minimum, maximum, step, false);

                foreach (StiStripLineXF stripLine in axis.Info.StripLines)
                {
                    stripLine.ValueObject = string.Format("{0}%", stripLine.ValueObject);
                }
            }
            else
            {
                double step = axis.Labels.Step;

                //Special code for preventing very small step
                if (step > 0 && axis.Info.Range > 0 && (axis.Info.Range / step) > 500)
                    step = 0;

                if (step == 0)
                {
                    step = StiStripLineCalculatorXF.GetInterval(axis.Info.Minimum, axis.Info.Maximum, 6);

                    #region DBS
                    if(((StiChart)this.Area.Chart).IsDashboard)
                        axis.Labels.CalculatedStep = (float)step;
                    #endregion
                }
                axis.Info.StripLines = StiStripLineCalculatorXF.GetStripLines(axis.Info.Minimum, axis.Info.Maximum, step, isDateTimeValues);
            }
        }

        /// <summary>
        /// Gets minimum and maximum values if specified YAxis from calculated StripLines collection.
        /// </summary>
        protected virtual void CheckStripLinesAndMaximumMinimumXAxis(IStiAxis axis)
        {

            if (axis.Info.StripLines.Count > 0)
            {
                axis.Info.Minimum = axis.Info.StripLines[0].Value;
                axis.Info.Maximum = axis.Info.StripLines[axis.Info.StripLines.Count - 1].Value;
            }
            else
            {
                axis.Info.Minimum = 0;
                axis.Info.Maximum = 1;
            }
        }


        /// <summary>
        /// Gets minimum and maximum values of specified XAxis from calculated StripLines collection.
        /// </summary>
        protected virtual void CheckStripLinesAndMaximumMinimumYAxis(IStiAxis axis)
        {
            if (axis.Info.StripLines.Count > 0)
            {
                axis.Info.Maximum = axis.Info.StripLines[0].Value;
                axis.Info.Minimum = axis.Info.StripLines[axis.Info.StripLines.Count - 1].Value;
            }
            else
            {
                axis.Info.Maximum = 100;
                axis.Info.Minimum = 0;
            }
        }


        /// <summary>
        /// Calculate Step factor of specified axis as for XAxis.
        /// </summary>
        private void CalculateStepX(IStiAxis axis, float topPosition, float bottomPosition)
        {
            if (axis.Info.StripLines.Count >= 2)
            {
                axis.Info.Step = Math.Abs((float)((axis.Info.StripLines[0].Value - axis.Info.StripLines[1].Value) * axis.Info.Dpi));
                axis.Core.CalculateStripPositions(topPosition, bottomPosition);
            }
            else
            {
                axis.Info.Step = 1;
                axis.Info.StripPositions = new float[0];
            }
        }

        /// <summary>
        /// Calculate Step factor of specified axis as for XAxis.
        /// </summary>
        private void CalculateStepY(IStiAxis axis, float topPosition, float bottomPosition)
        {
            if (axis.Info.StripLines.Count >= 2)
            {
                axis.Info.Step = Math.Abs((float)((axis.Info.StripLines[1].Value - axis.Info.StripLines[0].Value) * axis.Info.Dpi));
                axis.Core.CalculateStripPositions(topPosition, bottomPosition);
            }
            else
            {
                axis.Info.Step = 1;
                axis.Info.StripPositions = new float[0];
            }
        }

        /// <summary>
        /// Checks Minimum and Maximum values and StartFromZero factor
        /// </summary>
        private void CheckStartFromZeroYAxis(IStiAxis axis)
        {
            if (axis.Core.GetStartFromZero())
            {
                if (axis.Info.Maximum < 0) axis.Info.Maximum = 0;
                if (axis.Info.Minimum > 0) axis.Info.Minimum = 0;
            }
        }

        /// <summary>
        /// Fill specified collection with values from axis.Info.StripPositions and with taken in consideration step argument.
        /// </summary>
        internal void CalculatePositions(IStiAxis axis, ref List<StiStripPositionXF> collection, int step, bool calculationForTicks = false)
        {
            collection = new List<StiStripPositionXF>();

            IStiAxisArea axisArea = Area as IStiAxisArea;

            bool revert = false;
            if (Area is IStiAxisArea && axis is IStiYAxis)
            {
                revert = axisArea.ReverseVert;
            }
            else if (Area is IStiAxisArea && axis is IStiXAxis)
            {
                revert = axisArea.ReverseHor;
            }

            int stepIndex = 0;
            int length = axis.Info.StripPositions.Length;

            for (int index = 0; index < length; index++)
            {
                if (stepIndex == 0)
                {
                    var label = new StiStripPositionXF();

                    int stripIndex = revert ? length - index - 1 : index;

                    label.StripLine = axis.Info.StripLines[stripIndex];
                    label.Position = axis.Info.StripPositions[stripIndex];

                    if (revert)
                        collection.Insert(0, label);
                    else
                        collection.Add(label);
                }

                if (Area.Core is StiScatterAreaCoreXF && axis is IStiXAxis && !axis.LogarithmicScale)
                    continue;
                               
                //Skip step property  for not ClusteredBarArea and YAxis
                if (!(Area.Core is StiClusteredBarAreaCoreXF) && !(Area.Core is StiScatterAreaCoreXF) && axis is IStiYAxis && !calculationForTicks)
                    continue;

                if (Area.Core is StiClusteredBarAreaCoreXF && axis is IStiXAxis && !calculationForTicks)
                    continue;

                #region Add End Value
                if (index == (length - 1) && stepIndex != 0)
                {
                    var xAxis = axis as IStiXAxis;
                    if (xAxis != null && xAxis.ShowEdgeValues)
                    {
                        var label = new StiStripPositionXF();
                        int stripIndex = revert ? 0 : length - 1;

                        label.StripLine = axis.Info.StripLines[stripIndex];
                        label.Position = axis.Info.StripPositions[stripIndex];

                        if (revert)
                            collection.Insert(0, label);
                        else
                            collection.Add(label);
                    }
                }
                #endregion

                stepIndex++;

                if (stepIndex == step) stepIndex = 0;
            }
        }                
        

        /// <summary>
        /// Calculate divider for specified axis.
        /// </summary>
        private double CalculateDivider(IStiAxis axis)
        {
            var axisArea = this.Area as IStiAxisArea;
            
            int stripIndex = 0;
            if (axis.Info.StripLines != null)
            {
                foreach (StiStripLineXF stripLine in axis.Info.StripLines)
                {
                    if (stripLine.Value == 0d)
                    {
                        return axis.Info.StripPositions[stripIndex];
                    }
                    stripIndex++;
                }
            }
            if (axis is IStiYAxis)
            {
                if (axisArea.ReverseVert)
                    return (float)(-axis.Info.Minimum * axis.Info.Dpi);
                else
                    return (axis.Info.StripPositions == null || axis.Info.StripPositions.Length == 0) ? 
                        (float)(axis.Info.Minimum * axis.Info.Dpi) : 
                        (float)(axis.Info.StripPositions[axis.Info.StripPositions.Length - 1] + axis.Info.Minimum * axis.Info.Dpi);
            }
            else if (axis is IStiXAxis)
            {
                if (axisArea.ReverseHor)
                    return (axis.Info.StripPositions == null || axis.Info.StripPositions.Length == 0) ? 
                        (float)(axis.Info.Minimum * axis.Info.Dpi) :
                        (float)(axis.Info.StripPositions[axis.Info.StripPositions.Length - 1] + axis.Info.Minimum * axis.Info.Dpi);
                else
                    return -axis.Info.Minimum * axis.Info.Dpi;
            }
            else
                return 0;
        }

        /// <summary>
        /// Rotate strip lines of specified axis from start to end.
        /// </summary>
        private static void RotateStripLines(IStiAxis axis)
        {
            var lines = new StiStripLinesXF();
            foreach (StiStripLineXF line in axis.Info.StripLines)
            {
                lines.Insert(0, line);
            }
            axis.Info.StripLines = lines;
        }


        /// <summary>
        /// Returns position on x of zero line for x axis.
        /// </summary>
        /// <returns></returns>
        public float GetDividerX()
        {
            var axisArea = this.Area as IStiAxisArea;

            if (this is StiClusteredBarAreaCoreXF || this is StiScatterAreaCoreXF)
            {
                return (float)CalculateDivider(axisArea.XAxis);
            }

            return 0;
        }

        public double GetDividerXD()
        {
            var axisArea = this.Area as IStiAxisArea;

            if (this is StiClusteredBarAreaCoreXF || this is StiScatterAreaCoreXF)
            {
                return CalculateDivider(axisArea.XAxis);
            }

            return 0;
        }

        /// <summary>
        /// Returns position on x top of zero line for x axis.
        /// </summary>
        /// <returns></returns>
        public float GetDividerTopX()
        {
            var axisArea = this.Area as IStiAxisArea;

            if (this is StiClusteredBarAreaCoreXF)
            {
                return (float)CalculateDivider(axisArea.XTopAxis);
            }

            return 0;
        }

        /// <summary>
        /// Returns position on y of zero line for y axis.
        /// </summary>
        /// <returns></returns>
        public float GetDividerY()
        {
            var axisArea = this.Area as IStiAxisArea;

            if (this is StiClusteredBarAreaCoreXF && (!(this is StiGanttAreaCoreXF)))
                return 0;
            else
                return (float)CalculateDivider(axisArea.YAxis);
        }

        /// <summary>
        /// Returns position on right y of zero line for y axis.
        /// </summary>
        /// <returns></returns>
        public float GetDividerRightY()
        {
            var axisArea = this.Area as IStiAxisArea;

            if (this is StiClusteredBarAreaCoreXF && (!(this is StiGanttAreaCoreXF)))
                return 0;
            else
                return (float)CalculateDivider(axisArea.YRightAxis);
        }
        #endregion        

        #region Fields
        /// <summary>
        /// Count of value elements.
        /// </summary>
        internal int ValuesCount = 0;

        /// <summary>
        /// Gets scrollable area distance from left side of view area for Axis X.
        /// </summary>
        public double ScrollDistanceX
        {
            get
            {
                return ScrollDpiX * ScrollValueX;
            }
        }


        /// <summary>
        /// Gets scrollable area distance from left side of view area for Axis Y.
        /// </summary>
        public double ScrollDistanceY
        {
            get
            {
                return ScrollDpiY * ScrollValueY;
            }
        }


        private double scrollRangeX = 0;
        /// <summary>
        /// Gets size of scrollable area for Axis X.
        /// </summary>        
        public double ScrollRangeX
        {
            get
            {
                return scrollRangeX;
            }
        }


        private double scrollRangeY = 0;
        /// <summary>
        /// Gets size of scrollable area for Axis Y.
        /// </summary>        
        public double ScrollRangeY
        {
            get
            {
                return scrollRangeY;
            }
        }


        private double scrollViewX = 0;
        /// <summary>
        /// Gets view area size in scrollable area for Axis X.
        /// </summary>
        public double ScrollViewX
        {
            get
            {
                return scrollViewX;
            }
        }


        private double scrollViewY = 0;
        /// <summary>
        /// Gets view area size in scrollable area for Axis Y.
        /// </summary>
        public double ScrollViewY
        {
            get
            {
                return scrollViewY;
            }
        }

        public bool BlockScrollValueX { get; set; }


        public bool BlockScrollValueY { get; set; }

        /// <summary>
        /// Gets or sets left position of view area in scrollable area for Axis X.
        /// </summary>
        public double ScrollValueX { get; set; } = 0;


        /// <summary>
        /// Gets or sets left position of view area in scrollable area for Axis Y.
        /// </summary>
        public double ScrollValueY { get; set; } = 0;


        private double scrollDpiX = 1;
        /// <summary>
        /// Gets scrollable dpi factor for Axis X.
        /// </summary>
        public double ScrollDpiX
        {
            get
            {
                return scrollDpiX;
            }
        }


        private double scrollDpiY = 1;
        /// <summary>
        /// Gets scrollable dpi factor for Axis Y.
        /// </summary>
        public double ScrollDpiY
        {
            get
            {
                return scrollDpiY;
            }
        }

        public double ScrollDragStartValue { get; set; } = 0;
        #endregion

        public StiAxisAreaCoreXF(IStiArea area)
            : base(area)
        {            
        }
	}
}
