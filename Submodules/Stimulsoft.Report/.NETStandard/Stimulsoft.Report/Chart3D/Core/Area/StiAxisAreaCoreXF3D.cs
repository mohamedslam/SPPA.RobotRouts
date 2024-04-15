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
using Stimulsoft.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiAxisAreaCoreXF3D : StiAreaCoreXF
    {
        #region IStiApplyStyle
        /// <summary>
        /// Applying specified style to this area.
        /// </summary>
        /// <param name="style"></param>
        public override void ApplyStyle(IStiChartStyle style)
        {
            base.ApplyStyle(style);

            var axisArea = this.Area as IStiAxisArea3D;

            if (axisArea.AllowApplyStyle)
            {
                if (axisArea.InterlacingHor != null) axisArea.InterlacingHor.Core.ApplyStyle(style);
                if (axisArea.InterlacingVert != null) axisArea.InterlacingVert.Core.ApplyStyle(style);

                if (axisArea.GridLinesHor != null) axisArea.GridLinesHor.Core.ApplyStyle(style);
                if (axisArea.GridLinesVert != null) axisArea.GridLinesVert.Core.ApplyStyle(style);

                if (axisArea.XAxis != null) axisArea.XAxis.Core.ApplyStyle(style);
                if (axisArea.YAxis != null) axisArea.YAxis.Core.ApplyStyle(style);
            }
        }
        #endregion 

        #region Fields
        internal int ValuesCount = 0;
        private double contextScale;
        private PointF contextTranslate;
        #endregion

        public override StiCellGeom Render(StiContext context, RectangleF rect)
        {
            var viewRect = rect;
            var axisArea = this.Area as StiAxisArea3D;

            PrepareInfo(rect);

            var sizeYAxisMax = ((StiYAxisCoreXF3D)axisArea.YAxis.Core).GetAxisRect(context, rect, true, true, false, true);
            rect = new RectangleF(sizeYAxisMax.Width, 0, rect.Width - sizeYAxisMax.Width, rect.Height);

            var sizeXAxisMax = ((StiXAxisCoreXF3D)axisArea.XAxis.Core).GetAxisRect(context, rect, true, true, false, true);
            rect = new RectangleF(sizeYAxisMax.Width, 0, rect.Width - sizeYAxisMax.Width, rect.Height - sizeXAxisMax.Height);

            PrepareInfo(rect);

            var seriesCollection = axisArea.Core.GetSeries();
            var render3d = GetRedner3D(rect);
            var viewGeom = new StiAxisAreaViewGeom3D(axisArea, viewRect, render3d);
            var factor = GetFactorWidthHeight(rect);

            var x = factor;
            var y = 1d;
            var z = GetPositionZ(axisArea);

            var color = StiBrush.ToColor(Area.Brush);

            var xyAreaGeom = new StiXYAreaGeom3D(x, y, render3d)
            {
                Color = color
            };
            var xzAreaGeom = new StiXZAreaGeom3D(x, z, render3d)
            {
                Color = color
            };
            var yzAreaGeom = new StiZYAreaGeom3D(z, y, render3d)
            {
                Color = color
            };

            viewGeom.CreateChildGeoms();
            viewGeom.ChildGeoms.Add(yzAreaGeom);
            viewGeom.ChildGeoms.Add(xzAreaGeom);
            viewGeom.ChildGeoms.Add(xyAreaGeom);

            var rect3D = new StiRectangle3D()
            {
                Length = factor,
                Height = 1,
                Width = -GetPositionZ(axisArea)
            };

            RenderInterlacingHor(render3d, rect3D, viewGeom);
            RenderInterlacingVer(render3d, rect3D, viewGeom);

            RenderGridLinesHorZY(render3d, rect3D, axisArea.GridLinesHor, viewGeom);
            RenderGridLinesHorXY(render3d, rect3D, axisArea.GridLinesHor, viewGeom);

            RenderGridLinesVertXY(render3d, rect3D, axisArea.GridLinesVert, viewGeom);
            RenderGridLinesVertZX(render3d, rect3D, axisArea.GridLinesVert, viewGeom);

            RenderSeries(render3d, context, rect3D, viewGeom, seriesCollection);

            if (seriesCollection.Count > 0)
            {
                var axisYLeftGeom = axisArea.YAxis.Core.Render3D(context, rect3D, render3d);

                if (axisYLeftGeom != null)
                {
                    viewGeom.CreateChildGeoms();
                    viewGeom.ChildGeoms.Add(axisYLeftGeom);
                }

                var axisXBottomGeom = axisArea.XAxis.Core.Render3D(context, rect3D, render3d);

                if (axisXBottomGeom != null)
                {
                    viewGeom.CreateChildGeoms();
                    viewGeom.ChildGeoms.Add(axisXBottomGeom);
                }
            }

            return viewGeom;
        }

        private void RenderInterlacingVer(StiRender3D render3d, StiRectangle3D rect, StiAxisAreaViewGeom3D geom)
        {
            var axisArea = Area as IStiAxisArea3D;

            if (!axisArea.InterlacingVert.Visible)
                return;

            int index = 0;

            List<float> positionsTemp = new List<float>();
            float[] positions = axisArea.XAxis.Info.StripPositions;

            float firstPosition = 0;

            bool first = true;
            int indexTemp = 0;

            foreach (float pos in positions)
            {
                indexTemp++;
                if (first)
                {
                    firstPosition = pos;
                    positionsTemp.Add(pos);
                    first = false;
                    continue;
                }
                //float step = pos - firstPosition;

                //if (step < minWidth && indexTemp != positions.Length)
                //{
                //    continue;
                //}
                firstPosition = pos;
                positionsTemp.Add(pos);
            }

            float[] positionsNew = new float[positionsTemp.Count];

            positionsTemp.CopyTo(positionsNew);
            bool draw = true;

            foreach (float posX in positionsNew)
            {
                float posX2 = positionsNew[index + 1];

                if (draw)
                {
                    #region XY
                    var leftBottom = new StiPoint3D(posX, rect.Y, rect.Z);
                    var leftTop = new StiPoint3D(posX, rect.Top, rect.Z);
                    var rightBottom = new StiPoint3D(posX2, rect.Y, rect.Z);
                    var rightTop = new StiPoint3D(posX2, rect.Top, rect.Z);

                    var color = StiBrush.ToColor(axisArea.InterlacingVert.InterlacedBrush);

                    var plane = new StiPlaneGeom3D(leftBottom, leftTop, rightTop, rightBottom, color, render3d);
                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(plane);
                    #endregion

                    #region ZX
                    leftTop = leftBottom;
                    leftBottom = new StiPoint3D(posX, rect.Y, rect.Front);
                    rightTop = rightBottom;
                    rightBottom = new StiPoint3D(posX2, rect.Y, rect.Front);

                    plane = new StiPlaneGeom3D(leftBottom, leftTop, rightTop, rightBottom, color, render3d);
                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(plane); 
                    #endregion
                }

                draw = !draw;
                index++;
                if (index == positionsNew.Length - 1)
                    break;
            }
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public void SwitchOff()
        {
            var axisArea = this.Area as IStiAxisArea3D;

            axisArea.XAxis.Visible = false;
            axisArea.YAxis.Visible = false;
            axisArea.GridLinesHor.MinorVisible = false;
            axisArea.GridLinesVert.MinorVisible = false;
            axisArea.GridLinesHor.Visible = false;
            axisArea.GridLinesVert.Visible = false;
            axisArea.InterlacingHor.Visible = false;
            axisArea.InterlacingVert.Visible = false;
        }

        private void RenderInterlacingHor(StiRender3D render3d, StiRectangle3D rect, StiAxisAreaViewGeom3D geom)
        {
            var axisArea = Area as IStiAxisArea3D;

            if (!axisArea.InterlacingHor.Visible)
                return;

            List<float> positionsTemp = new List<float>();
            int count = axisArea.YAxis.Info.StripPositions.Length;
            float[] positions = new float[count];
            for (int indexReorder = 0; indexReorder < count; indexReorder++)
            {
                positions[count - 1 - indexReorder] = axisArea.YAxis.Info.StripPositions[indexReorder];
            }

            float firstPosition = 0;

            bool first = true;
            int indexTemp = 0;

            foreach (float pos in positions)
            {
                indexTemp++;
                if (first)
                {
                    firstPosition = pos;
                    positionsTemp.Add(pos);
                    first = false;
                    continue;
                }
                //float step = firstPosition - pos;

                //if (step < minWidth && indexTemp != positions.Length)
                //{
                //    continue;
                //}
                firstPosition = pos;
                positionsTemp.Add(pos);
            }

            float[] positionsNew = new float[positionsTemp.Count];

            positionsTemp.CopyTo(positionsNew);

            int index = 0;
            bool draw = true;

            foreach (float posY in positionsNew)
            {
                float posY2 = positionsNew[index + 1];

                if (draw)
                {
                    #region XY
                    var leftBottom = new StiPoint3D(rect.X, posY, rect.Z);
                    var leftTop = new StiPoint3D(rect.X, posY2, rect.Z);
                    var rightBottom = new StiPoint3D(rect.Right, posY, rect.Z);
                    var rightTop = new StiPoint3D(rect.Right, posY2, rect.Z);

                    var color = StiBrush.ToColor(axisArea.InterlacingHor.InterlacedBrush);

                    var plane = new StiPlaneGeom3D(leftBottom, leftTop, rightTop, rightBottom, color, render3d);
                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(plane);
                    #endregion

                    #region ZY
                    rightBottom = leftBottom;
                    rightTop = leftTop;
                    leftBottom = new StiPoint3D(rect.X, posY, rect.Front);
                    leftTop = new StiPoint3D(rect.X, posY2, rect.Front);                   

                    plane = new StiPlaneGeom3D(leftBottom, leftTop, rightTop, rightBottom, color, render3d);
                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(plane);
                    #endregion
                }
                draw = !draw;

                index++;
                if (index == positionsNew.Length - 1)
                    break;
            }
        }

        protected override void PrepareInfo(RectangleF rect)
        {
            var factor = GetFactorWidthHeight(rect);

            var axisArea = this.Area as IStiAxisArea3D;

            var seriesCollection = GetSeries();

            if (seriesCollection.Count > 0)
            {
                axisArea.XAxis.Info.StripLines = new StiStripLinesXF();
                axisArea.YAxis.Info.StripLines = new StiStripLinesXF();
                axisArea.ZAxis.Info.StripLines = new StiStripLinesXF();

                #region ValuesCount
                ValuesCount = 0;

                for (int index = 0; index < seriesCollection.Count; index++)
                {
                    double?[] values = seriesCollection[index].Values;
                    if (values != null)
                        ValuesCount = Math.Max(values.Length, ValuesCount);
                }
                #endregion

                PrepareRange(axisArea.XAxis, axisArea.YAxis, axisArea.ZAxis);

                CheckStartFromZeroYAxis(axisArea.YAxis);

                CalculateMinimumAndMaximumYAxis(axisArea.YAxis);
                CalculateMinimumAndMaximumXAxis(axisArea.XAxis);

                CreateStripLinesXAxis(axisArea.XAxis);
                CreateStripLinesYAxis(axisArea.YAxis);
                CreateStripLinesZAxis(axisArea.ZAxis);

                CheckStripLinesAndMaximumMinimumXAxis(axisArea.XAxis);
                CheckStripLinesAndMaximumMinimumYAxis(axisArea.YAxis);
                CheckStripLinesAndMaximumMinimumZAxis(axisArea.ZAxis);

                CheckShowEdgeValues(axisArea.XAxis);

                axisArea.XAxis.Info.Dpi = factor / axisArea.XAxis.Info.Range;
                axisArea.YAxis.Info.Dpi = 1 / axisArea.YAxis.Info.Range;
                axisArea.ZAxis.Info.Dpi = axisArea.XAxis.Info.Dpi;

                CalculateStepX(axisArea.XAxis, 0, factor);
                CalculateStepY(axisArea.YAxis, 1, 0);
                CalculateStepZ(axisArea.ZAxis, 0, (float)GetPositionZ(axisArea));

                CalculatePositions(axisArea.XAxis, ref axisArea.XAxis.Info.LabelsCollection, 1);//axisArea.XAxis.Labels.Step > 0 ? (int)axisArea.XAxis.Labels.Step : 1);
                CalculatePositions(axisArea.YAxis, ref axisArea.YAxis.Info.LabelsCollection, 1);// axisArea.YAxis.Labels.Step > 1 ? (int)axisArea.YAxis.Labels.Step : 1);
            }

            var render3d = GetRedner3D(rect);

            var x = factor;
            var y = 1d;
            var z = GetPositionZ(axisArea);

            var xyAreaGeom = new StiXYAreaGeom3D(x, y, render3d);
            var xzAreaGeom = new StiXZAreaGeom3D(x, z, render3d);
            var yzAreaGeom = new StiZYAreaGeom3D(z, y, render3d);

            var xyRect = xyAreaGeom.MeasureCientRect();
            var xzRect = xzAreaGeom.MeasureCientRect();
            var yzRect = yzAreaGeom.MeasureCientRect();

            var rectUnion = RectangleF.Union(xyRect, xzRect);
            rectUnion = RectangleF.Union(rectUnion, yzRect);

            this.contextScale = Math.Min(rect.Height / rectUnion.Height, rect.Width / rectUnion.Width);

            var delatX = rect.X + (rect.Width - rectUnion.Width * this.contextScale) / 2;
            var deltaY = rect.Y + (rect.Height - rectUnion.Height * this.contextScale) / 2;

            this.contextTranslate = new PointF(rectUnion.X * (float)this.contextScale - (float)delatX, rectUnion.Y * (float)this.contextScale - (float)deltaY);
        }

        /// <summary>
        /// Calculate minimum and maximum values of YAxis.
        /// </summary>
        internal void CalculateMinimumAndMaximumYAxis(IStiAxis3D axis)
        {
            //#region For Gantt Series
            //if (this.GetSeries().Count > 0 && this.GetSeries()[0] is StiGanttSeries)
            //{
            //    if (!axis.Range.Auto)
            //    {
            //        axis.Info.Maximum = axis.Range.Maximum;
            //        axis.Info.Minimum = axis.Range.Minimum;
            //    }

            //    return;
            //}
            //#endregion

            //if (!axis.StartFromZero && axis.Range.Auto && (axis is IStiYAxis))
            //{
            //    double range = (axis.Info.Maximum - axis.Info.Minimum) * 0.05;

            //    axis.Info.Minimum -= range;
            //    axis.Info.Maximum += range;

            //    if (axis.Info.Minimum < 0 && axis.Info.Minimum + range >= 0) axis.Info.Minimum = 0;
            //}
            //else
            //{
                double minDelts = 0.1;
                if (this.GetSeries().Count > 0 && this.GetSeries()[0].SeriesLabels is StiOutsideEndAxisLabels)
                    minDelts = 0.15;

                //Increase calculated minimum and maximum values per special factor
                double delta = Math.Min(minDelts, Math.Abs(4 / ((axis.Info.Maximum + axis.Info.Minimum) / 2)));
                axis.Info.Maximum *= 1 + delta * Math.Sign(axis.Info.Maximum);
                axis.Info.Minimum *= 1 - delta * Math.Sign(axis.Info.Minimum);
            //}

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

        private void CheckStartFromZeroYAxis(IStiAxis3D axis)
        {
            //if (axis.Core.GetStartFromZero())
            {
                if (axis.Info.Maximum < 0) axis.Info.Maximum = 0;
                if (axis.Info.Minimum > 0) axis.Info.Minimum = 0;
            }
        }

        private void CalculateStepX(IStiAxis3D axis, double left, double right)
        {
            if (axis.Info.StripLines.Count >= 2)
            {
                axis.Info.Step = Math.Abs((float)((axis.Info.StripLines[0].Value - axis.Info.StripLines[1].Value) * axis.Info.Dpi));
                axis.Core.CalculateStripPositions(left, right);
            }
            else
            {
                axis.Info.Step = 1;
                axis.Info.StripPositions = new float[0];
            }
        }

        private void CalculateStepY(IStiAxis3D axis, float topPosition, float bottomPosition)
        {
            if (axis.Info.StripLines.Count >= 2)
            {
                axis.Info.Step = Math.Abs((float)((axis.Info.StripLines[1].Value - axis.Info.StripLines[0].Value) * axis.Info.Dpi));
                axis.Core.CalculateStripPositions(bottomPosition, topPosition);
            }
            else
            {
                axis.Info.Step = 1;
                axis.Info.StripPositions = new float[0];
            }
        }

        private void CalculateStepZ(IStiAxis3D axis, float topPosition, float bottomPosition)
        {
            if (axis.Info.StripLines.Count >= 2)
            {
                axis.Info.Step = -Math.Abs((float)((axis.Info.StripLines[1].Value - axis.Info.StripLines[0].Value) * axis.Info.Dpi));
                axis.Core.CalculateStripPositions(topPosition, bottomPosition);
            }
            else
            {
                axis.Info.Step = -1;
                axis.Info.StripPositions = new float[0];
            }
        }

        internal void CalculatePositions(IStiAxis3D axis, ref List<StiStripPositionXF> collection, int step, bool calculationForTicks = false)
        {
            collection = new List<StiStripPositionXF>();

            var axisArea = Area as IStiAxisArea3D;

            int stepIndex = 0;
            int length = axis.Info.StripPositions.Length;

            for (int index = 0; index < length; index++)
            {
                if (stepIndex == 0)
                {
                    var label = new StiStripPositionXF();

                    int stripIndex =  index;

                    label.StripLine = axis.Info.StripLines[stripIndex];
                    label.Position = axis.Info.StripPositions[stripIndex];

                        collection.Add(label);
                }

                /*if (Area.Core is StiScatterAreaCoreXF && axis is IStiXAxis && !axis.LogarithmicScale)
                    continue;

                //Skip step property  for not ClusteredBarArea and YAxis
                if (!(Area.Core is StiClusteredBarAreaCoreXF) && !(Area.Core is StiScatterAreaCoreXF) && axis is IStiYAxis && !calculationForTicks)
                    continue;

                if (Area.Core is StiClusteredBarAreaCoreXF && axis is IStiXAxis && !calculationForTicks)
                    continue;*/

                #region Add End Value
                if (index == (length - 1) && stepIndex != 0)
                {
                    var xAxis = axis as IStiXAxis3D;
                    if (xAxis != null && xAxis.ShowEdgeValues)
                    {
                        var label = new StiStripPositionXF();
                        int stripIndex = length - 1;

                        label.StripLine = axis.Info.StripLines[stripIndex];
                        label.Position = axis.Info.StripPositions[stripIndex];

                        collection.Add(label);
                    }
                }
                #endregion

                stepIndex++;

                if (stepIndex == step) stepIndex = 0;
            }
        }

        private double GetPositionZ(IStiAxisArea3D axisArea3D)
        {
            if (axisArea3D is StiClusteredColumnArea3D columnArea3D && !columnArea3D.SideBySide)
                return axisArea3D.XAxis.Info.Dpi * GetSeries().Count;

            return axisArea3D.XAxis.Info.Dpi;
        }

        private void RenderSeries(StiRender3D render3d, StiContext context, StiRectangle3D rect, StiAreaGeom geom, List<IStiSeries> seriesCollection)
        {
            var area = geom.Area;
            var axisArea = area as StiClusteredColumnArea3D;

            if (area is StiClusteredColumnArea3D clusteredColumnArea3D && clusteredColumnArea3D.SideBySide)
            {
                ((StiClusteredColumnSeriesCoreXF3D)seriesCollection[0].Core).RenderSideBySide(render3d, context, rect, geom, seriesCollection.ToArray());
            }
            else
            {
                for (int seriesIndex = seriesCollection.Count - 1; seriesIndex >= 0; seriesIndex--)
                {
                    var series = seriesCollection[seriesIndex];
                    ((StiSeriesCoreXF3D)series.Core).RenderSeries3D(render3d, context, rect, geom, seriesIndex, seriesCollection.ToArray());
                }
            }            
        }

        private void RenderGridLinesHorZY(StiRender3D render3d, StiRectangle3D rect, IStiGridLinesHor gridLinesHor, StiAreaGeom geom)
        {
            var axisArea = Area as IStiAxisArea3D;

            var positions = axisArea.YAxis.Info.StripPositions;

            if (positions.Length > 0)
            {
                int index = 0;
                foreach (float posY in positions)
                {
                    if (gridLinesHor.Visible && gridLinesHor.Style != StiPenStyle.None)
                    {
                        var pointY = posY + rect.Y;
                        if (pointY >= 0 && pointY <= rect.Height)
                        {
                            var startPoint = new StiPoint3D(rect.X, pointY, rect.Z);
                            var endPoint = new StiPoint3D(rect.X, pointY, rect.Front);

                            var line = new StiLineGeom3D(startPoint, endPoint, gridLinesHor.Color, gridLinesHor.Style, render3d);
                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(line);
                        }
                    }

                    index++;
                }
            }
        }

        private void RenderGridLinesHorXY(StiRender3D render3d, StiRectangle3D rect, IStiGridLinesHor gridLinesHor, StiAreaGeom geom)
        {
            var axisArea = Area as IStiAxisArea3D;

            var positions = axisArea.YAxis.Info.StripPositions;

            if (positions.Length > 0)
            {
                int index = 0;
                foreach (float posY in positions)
                {
                    if (gridLinesHor.Visible && gridLinesHor.Style != StiPenStyle.None)
                    {
                        var pointY = posY + rect.Y;
                        if (pointY >= 0 && pointY <= rect.Height)
                        {
                            var startPoint = new StiPoint3D(rect.X, pointY, rect.Z);
                            var endPoint = new StiPoint3D(rect.Right, pointY, rect.Z);

                            var line = new StiLineGeom3D(startPoint, endPoint, gridLinesHor.Color, gridLinesHor.Style, render3d);
                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(line);
                        }
                    }

                    index++;
                }
            }
        }

        private void RenderGridLinesVertZX(StiRender3D render3d, StiRectangle3D rect, IStiGridLinesVert gridLinesVert, StiAxisAreaViewGeom3D geom)
        {
            var axisArea = Area as IStiAxisArea3D;

            var positions = axisArea.XAxis.Info.StripPositions;

            if (positions.Length > 0)
            {
                int index = 0;
                foreach (float posX in positions)
                {
                    if (gridLinesVert.Visible && gridLinesVert.Style != StiPenStyle.None)
                    {
                        var pointX = posX + rect.X;
                        if (pointX >= 0 && pointX <= rect.Right)
                        {
                            var startPoint = new StiPoint3D(posX, rect.Y, rect.Z);
                            var endPoint = new StiPoint3D(posX, rect.Y, rect.Front);

                            var line = new StiLineGeom3D(startPoint, endPoint, gridLinesVert.Color, gridLinesVert.Style, render3d);
                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(line);
                        }
                    }

                    index++;
                }
            }
        }

        private void RenderGridLinesVertXY(StiRender3D render3d, StiRectangle3D rect, IStiGridLinesVert gridLinesVert, StiAxisAreaViewGeom3D geom)
        {
            var axisArea = Area as IStiAxisArea3D;

            var positions = axisArea.XAxis.Info.StripPositions;

            if (positions.Length > 0)
            {
                int index = 0;
                foreach (float posX in positions)
                {
                    if (gridLinesVert.Visible && gridLinesVert.Style != StiPenStyle.None)
                    {
                        var pointX = posX + rect.X;
                        if (pointX >= 0 && pointX <= rect.Right)
                        {
                            var startPoint = new StiPoint3D(posX, rect.Y, rect.Z);
                            var endPoint = new StiPoint3D(posX, rect.Top, rect.Z);

                            var line = new StiLineGeom3D(startPoint, endPoint, gridLinesVert.Color, gridLinesVert.Style, render3d);
                            geom.CreateChildGeoms();
                            geom.ChildGeoms.Add(line);
                        }
                    }

                    index++;
                }
            }
        }

        protected virtual void CheckShowEdgeValues(IStiXAxis3D axis)
        {
            if (axis.Info.StripLines.Count > 0)
            {
                if (!axis.ShowEdgeValues)
                {
                    axis.Info.StripLines[0].ValueObject = null;
                    axis.Info.StripLines[axis.Info.StripLines.Count - 1].ValueObject = null;
                }
            }
        }

        private void CalculateMinimumAndMaximumXAxis(IStiAxis3D xAxis)
        {
            
        }

        /// <summary>
        /// Creates Strip lines collection for YAxis.
        /// </summary>
        private void CreateStripLinesYAxis(IStiAxis3D yAxis)
        {
            if (Area.GetDefaultSeriesType() == typeof(StiFullStackedColumnSeries3D))
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

                var step = StiStripLineCalculatorXF.GetInterval(minimum, maximum, 6);
                yAxis.Info.StripLines = StiStripLineCalculatorXF.GetStripLines(minimum, maximum, step, false);

                foreach (StiStripLineXF stripLine in yAxis.Info.StripLines)
                {
                    stripLine.ValueObject = string.Format("{0}%", stripLine.ValueObject);
                }
            }
            else
            {
                var step = StiStripLineCalculatorXF.GetInterval(yAxis.Info.Minimum, yAxis.Info.Maximum, 6);

                #region DBS
                if (((StiChart)this.Area.Chart).IsDashboard)
                    yAxis.Labels.CalculatedStep = (float)step;
                #endregion

                yAxis.Info.StripLines = StiStripLineCalculatorXF.GetStripLines(yAxis.Info.Minimum, yAxis.Info.Maximum, step, false);
            }

            yAxis.Info.StripLines.Reverse();
        }

        /// <summary>
        /// Creates Strip lines collection for XAxis.
        /// </summary>
        protected virtual void CreateStripLinesXAxis(IStiAxis3D axis)
        {
            for (int index = 0; index <= ValuesCount + 1; index++)
                axis.Info.StripLines.Add(index, index);

            var seriesCollection = GetSeries();
            foreach (var series in seriesCollection)
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
                        int finalIndex = 1 + index;
                        if (finalIndex < axis.Info.StripLines.Count)
                            axis.Info.StripLines[finalIndex].ValueObject = valueObject;
                    }
                }
            }
        }

        /// <summary>
        /// Creates Strip lines collection for ZAxis.
        /// </summary>
        protected virtual void CreateStripLinesZAxis(IStiAxis3D axis)
        {
            var seriesCollection = GetSeries();
            for (int index = 0; index <= seriesCollection.Count; index++)
                axis.Info.StripLines.Add(index, index);
        }

        /// <summary>
        /// Gets minimum and maximum values if specified XAxis from calculated StripLines collection.
        /// </summary>
        protected virtual void CheckStripLinesAndMaximumMinimumXAxis(IStiAxis3D axis)
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
        /// Gets minimum and maximum values of specified YAxis from calculated StripLines collection.
        /// </summary>
        protected virtual void CheckStripLinesAndMaximumMinimumYAxis(IStiAxis3D axis)
        {
            if (axis.Info.StripLines.Count > 0)
            {
                axis.Info.Minimum = axis.Info.StripLines[0].Value;
                axis.Info.Maximum = axis.Info.StripLines[axis.Info.StripLines.Count - 1].Value;
            }
            else
            {
                axis.Info.Maximum = 100;
                axis.Info.Minimum = 0;
            }
        }

        /// <summary>
        /// Gets minimum and maximum values if specified ZAxis from calculated StripLines collection.
        /// </summary>
        protected virtual void CheckStripLinesAndMaximumMinimumZAxis(IStiAxis3D axis)
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

        protected virtual void PrepareRange(IStiAxis3D xAxis, IStiAxis3D yAxis, IStiAxis3D zAxis)
        {

        }

        /// <summary>
        /// Returns position on y of zero line for y axis.
        /// </summary>
        /// <returns></returns>
        public double GetDividerY()
        {
            var axisArea = this.Area as IStiAxisArea3D;

            //if (this is StiClusteredBarAreaCoreXF && (!(this is StiGanttAreaCoreXF)))
            //    return 0;
            //else
            //    return (float)CalculateDivider(axisArea.YAxis);

            return CalculateDivider(axisArea.YAxis);
        }

        /// <summary>
        /// Calculate divider for specified axis.
        /// </summary>
        private double CalculateDivider(IStiAxis3D axis)
        {
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

            return 0;
        }

        protected StiRender3D GetRedner3D(RectangleF rect)
        {
            var axisArea = this.Area as StiAxisArea3D;

            var render3d = new StiRender3D(1000, 1000) ; // (rect.Width, rect.Height);
            render3d.GlobalRotationX = axisArea.RotationX;
            render3d.GlobalRotationY = axisArea.RotationY;
            render3d.GlobalScale = axisArea.Scale;
            render3d.Camera.Position.X = axisArea.CameraX;
            render3d.Camera.Position.Y = axisArea.CameraY;
            render3d.Camera.Position.Z = axisArea.CameraZ;
            render3d.Camera.NearPlane = axisArea.NearPlane;
            render3d.ContextScale = this.contextScale;
            render3d.ContextTranslate = this.contextTranslate;

            return render3d;
        }

        protected double GetFactorWidthHeight(RectangleF rect)
        {
            var factor = rect.Width / rect.Height;

            if (factor > 2) factor = 2f;
            if (factor < 1) factor =1;

            return factor;
        }

        protected StiAxisAreaCoreXF3D(IStiArea area) : base(area)
        {
        }
    }
}
