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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiRadarAreaCoreXF : StiAreaCoreXF
	{
        #region IStiApplyStyle
        /// <summary>
        /// Applying specified style to this area.
        /// </summary>
        /// <param name="style"></param>
        public override void ApplyStyle(IStiChartStyle style)
        {
            base.ApplyStyle(style);

            IStiRadarArea radarArea = this.Area as IStiRadarArea;

            if (radarArea.AllowApplyStyle)
            {
                if (radarArea.InterlacingHor != null) radarArea.InterlacingHor.Core.ApplyStyle(style);
                if (radarArea.InterlacingVert != null) radarArea.InterlacingVert.Core.ApplyStyle(style);

                if (radarArea.GridLinesHor != null) radarArea.GridLinesHor.Core.ApplyStyle(style);
                if (radarArea.GridLinesVert != null) radarArea.GridLinesVert.Core.ApplyStyle(style);

                if (radarArea.XAxis != null) radarArea.XAxis.Core.ApplyStyle(style);
                if (radarArea.YAxis != null) radarArea.YAxis.Core.ApplyStyle(style);

                radarArea.Brush = new StiEmptyBrush();
            }
        }
        #endregion       

        #region Fields
        /// <summary>
        /// Count of values.
        /// </summary>
        internal int ValuesCount = 0;
        internal List<PointF> Points = new List<PointF>();
        internal List<object> Arguments = new List<object>();
        internal PointF CenterPoint = PointF.Empty;
        #endregion 

        #region Methods
        public override StiCellGeom Render(StiContext context, RectangleF areaRect)
        {
            IStiRadarArea radarArea = this.Area as IStiRadarArea;

            RectangleF rect = areaRect;

            rect = CenterArea(rect);
            
            PrepareInfo(rect);

            rect = MeasureLabels(context, rect);
            rect = CenterArea(rect);
            rect = new RectangleF(
                areaRect.X + (areaRect.Width - rect.Width) / 2,
                areaRect.Y + (areaRect.Height - rect.Height) / 2,
                rect.Width, rect.Height);
            
            PrepareInfo(rect);

            StiRadarAreaGeom areaGeom = new StiRadarAreaGeom(this.Area, rect, this.ValuesCount);

            #region Render Axis Geoms
            StiRadarAxisGeom axisGeom = radarArea.YAxis.YCore.Render(context, rect) as StiRadarAxisGeom;

            if (axisGeom != null)
            {
                areaGeom.CreateChildGeoms();
                areaGeom.ChildGeoms.Add(axisGeom);
            }
            #endregion

            if (this.Area.Chart.Series.Count > 0)
                RenderArguments(context, areaGeom, this.Area.Chart.Series[0] as IStiSeries);

            #region Render Series
            List<IStiSeries> seriesCollection = GetSeries();
            RenderSeries(context, rect, areaGeom, seriesCollection);
            #endregion

            return areaGeom;
        }

        private static RectangleF CenterArea(RectangleF rect)
        {
            if (rect.Width > rect.Height)
                rect.Width = rect.Height;
            else
                rect.Height = rect.Width;

            return rect;
        }

        public RectangleF MeasureLabels(StiContext context, RectangleF rect)
        {
            if (ValuesCount == 0) return rect;
            var radarArea = this.Area as IStiRadarArea;

            if (radarArea.XAxis != null && radarArea.XAxis.Visible && radarArea.YAxis.Info.StripPositions != null)
            {                
                float radius = radarArea.YAxis.Info.StripPositions[radarArea.YAxis.Info.StripPositions.Length - 1] + 4 * context.Options.Zoom;
                int pointIndex = 0;

                float arc = 360f / ValuesCount;
                float angle = 0;

                RectangleF areaRect = rect;

                foreach (PointF curPoint in this.Points)
                {
                    var argument = pointIndex < this.Arguments.Count ? this.Arguments[pointIndex] : null;

                    var point = new PointF(
                        this.CenterPoint.X + radius * curPoint.X,
                        this.CenterPoint.Y + radius * curPoint.Y);

                    var argumentText = ((StiXRadarAxisCoreXF)radarArea.XAxis.Core).GetLabelText(argument);


                    var labelRect = radarArea.XAxis.XCore.GetLabelRect(context, point, argumentText, angle);
                    if (!labelRect.IsEmpty)
                        areaRect = RectangleF.Union(areaRect, labelRect);

                    //Red line
                    //context.DrawRectangle(new StiPenGeom(Color.Red), labelRect.X, labelRect.Y, labelRect.Width, labelRect.Height);
                    
                    angle += arc;
                    pointIndex++;
                }

                float distLeft = rect.Left - areaRect.Left;
                float distRight = areaRect.Right - rect.Right;
                float distTop = rect.Top - areaRect.Top;
                float distBottom = areaRect.Bottom - rect.Bottom;

                if (distLeft > 0)rect.Width -= distLeft;
                if (distRight > 0)rect.Width -= distRight;
                if (distTop > 0) rect.Height -= distTop;
                if (distBottom > 0) rect.Height -= distBottom;
            }
            return rect;
        }

        public void RenderArguments(StiContext context, StiRadarAreaGeom geom, IStiSeries series)
        {
            if (ValuesCount == 0) return;
            var radarArea = this.Area as IStiRadarArea;

            if (radarArea.XAxis != null && radarArea.XAxis.Visible && radarArea.YAxis.Info.StripPositions != null)
            {
                float radius = radarArea.YAxis.Info.StripPositions[radarArea.YAxis.Info.StripPositions.Length - 1] + 4 * context.Options.Zoom;
                int pointIndex = 0;

                float arc = 360f / ValuesCount;
                float angle = 0;
                
                foreach (PointF curPoint in this.Points)
                {
                    object argument = pointIndex < this.Arguments.Count ? this.Arguments[pointIndex] : null;
                    
                    var point = new PointF(
                        this.CenterPoint.X + radius * curPoint.X,
                        this.CenterPoint.Y + radius * curPoint.Y);
                    
                        point.X -= geom.ClientRectangle.X;
                        point.Y -= geom.ClientRectangle.Y;

                    var labelGeom = radarArea.XAxis.XCore.RenderLabel(context, series, point, argument, angle, pointIndex, ValuesCount);

                    if (labelGeom != null)
                    {
                        geom.CreateChildGeoms();
                        geom.ChildGeoms.Add(labelGeom);
                    }
                    angle += arc;
                    pointIndex++;
                }
            }

            if (!radarArea.XAxis.Labels.RotationLabels && radarArea.XAxis.Labels.PreventIntersection)
                CheckIntersectionLabels(geom);
        }


        protected virtual void CheckIntersectionLabels(StiAreaGeom geom)
        {
            var childGeoms = geom.ChildGeoms;

            var centerPoint = new PointF(geom.ClientRectangle.Width / 2, geom.ClientRectangle.Height / 2);

            var labelTopGeoms = new List<StiXRadarAxisLabelGeom>();
            var labelBottomGeoms = new List<StiXRadarAxisLabelGeom>();

            foreach (var cellGeom in childGeoms)
            {
                var axisLabelsGeom = cellGeom as StiXRadarAxisLabelGeom;
                if (axisLabelsGeom != null)
                {                    
                    if (axisLabelsGeom.GetDrawRectangle().Y < centerPoint.Y)
                    {
                        labelTopGeoms.Add(axisLabelsGeom);
                    }
                    else
                    {
                        labelBottomGeoms.Add(axisLabelsGeom);
                    }
                }
            }

            CheckTopIntersectionLabels(labelTopGeoms.OrderBy(x => x.GetDrawRectangle().Y).ToList());
            CheckBottomIntersectionLabels(labelBottomGeoms.OrderBy(x => x.GetDrawRectangle().Y).ToList());
        }

        private void CheckTopIntersectionLabels(List<StiXRadarAxisLabelGeom> labelGeoms)
        {
            int count = labelGeoms.Count;

            bool intersection = true;
            int indexCheck = 0;

            while (intersection && indexCheck < 20)
            {
                indexCheck++;

                for (int index1 = 0; index1 < count; index1++)
                {
                    for (int index2 = 0; index2 < count; index2++)
                    {
                        if (index2 == index1) continue;

                        var rect1 = labelGeoms[index1].GetDrawRectangle();
                        var rect2 = labelGeoms[index2].GetDrawRectangle();

                        if (rect1.IntersectsWith(rect2))
                        {

                            if (rect1.IntersectsWith(rect2))
                            {
                                if (rect1.Y < rect2.Y)
                                {
                                    labelGeoms[index1].Point = new PointF(labelGeoms[index1].Point.X, labelGeoms[index1].Point.Y -1);
                                }
                                else
                                {
                                    labelGeoms[index2].Point = new PointF(labelGeoms[index2].Point.X, labelGeoms[index2].Point.Y - 1);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CheckBottomIntersectionLabels(List<StiXRadarAxisLabelGeom> labelGeoms)
        {
            int count = labelGeoms.Count;

            bool intersection = true;
            int indexCheck = 0;

            while (intersection && indexCheck < 20)
            {
                indexCheck++;

                for (int index1 = 0; index1 < count; index1++)
                {
                    for (int index2 = 0; index2 < count; index2++)
                    {
                        if (index2 == index1) continue;

                        var rect1 = labelGeoms[index1].GetDrawRectangle();
                        var rect2 = labelGeoms[index2].GetDrawRectangle();

                        if (rect1.IntersectsWith(rect2))
                        {

                            if (rect1.IntersectsWith(rect2))
                            {
                                if (rect1.Y > rect2.Y)
                                {
                                    labelGeoms[index1].Point = new PointF(labelGeoms[index1].Point.X, labelGeoms[index1].Point.Y + 1);
                                }
                                else
                                {
                                    labelGeoms[index2].Point = new PointF(labelGeoms[index2].Point.X, labelGeoms[index2].Point.Y + 1);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, List<IStiSeries> seriesCollection)
        {
            var seriesTypes = new List<List<IStiSeries>>();
            var seriesTypesHash = new Hashtable();

            foreach (var ser in seriesCollection)
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

            foreach (var seriesType in seriesTypes)
            {
                var seriesArray = seriesType.ToArray();
                seriesArray[0].Core.RenderSeries(context, rect, geom, seriesArray);
            }
        }        

        protected override void PrepareInfo(RectangleF rect)
        {            
            var radarArea = this.Area as IStiRadarArea;
                        
            #region Calculate ValuesCount
            ValuesCount = 0;

            List<IStiSeries> seriesCollection = GetSeries();
            if (seriesCollection.Count > 0)
            {
                for (int index = 0; index < seriesCollection.Count; index++)
                {
                    double?[] values = ((IStiSeries)seriesCollection[index]).Values;
                    if (values != null)
                    {
                        ValuesCount = Math.Max(values.Length, ValuesCount);
                    }
                }
            }
            #endregion

            #region Calculate Arguments
            this.Arguments = new List<object>();
            if (seriesCollection.Count > 0)
            {
                IStiSeries curSeries = seriesCollection[0];
                foreach (IStiSeries series in seriesCollection)
                {
                    if (series.Arguments.Length == ValuesCount)
                    {
                        curSeries = series;
                        break;
                    }
                }

                for (int index = 0; index < ValuesCount; index++)
                {
                    if (index < curSeries.Arguments.Length)
                    {
                        var arg = curSeries.Arguments[index] == null ? string.Empty : curSeries.Arguments[index].ToString();
                        Arguments.Add(arg);
                    }
                }
            }
            else
            {
                Arguments.Add("1");
                Arguments.Add("2");
                Arguments.Add("3");
                Arguments.Add("4");
                Arguments.Add("5");
            }
            #endregion

            this.CenterPoint = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            #region Calculate radial points
            decimal arc;

            if (ValuesCount > 0) arc = 360m / ValuesCount;
            else arc = 360m / 5;

            this.Points = new List<PointF>();

            decimal angle = 0;
            //decimal radius = (decimal)rect.Width / 2;
            while (angle < 360)
            {
                float radAngle = (float)(((float)angle - 90) * Math.PI / 180);                

                PointF endPoint = new PointF(
                    (float)(Math.Cos(radAngle)),
                    (float)(Math.Sin(radAngle)));

                this.Points.Add(endPoint);
                angle += arc;
                if (this.Points.Count == ValuesCount)
                    break;
            }
            #endregion

            #region Calculate Minimum & Maximum
            double minimum = 0;
            double maximum = 0;
            if (radarArea.YAxis.Range.Auto)
            {

                bool firstValue = true;

                for (int index = 0; index < seriesCollection.Count; index++)
                {
                    double?[] values = ((IStiSeries)seriesCollection[index]).Values;
                    if (values != null)
                    {
                        foreach (double? value in values)
                        {
                            if (value == null) continue;
                            if (firstValue)
                            {
                                minimum = value.Value;
                                maximum = value.Value;
                                firstValue = false;
                            }
                            else
                            {
                                minimum = Math.Min(minimum, value.Value);
                                maximum = Math.Max(maximum, value.Value);
                            }
                        }
                    }
                }
                if (seriesCollection.Count == 0)
                    maximum = 1;

                if (Math.Abs(minimum) > maximum)
                    maximum = Math.Abs(minimum);

                if (maximum == 0 && minimum == 0)
                    maximum = 1;

                minimum = 0;
            }
            else
            {
                minimum = radarArea.YAxis.Range.Minimum;
                maximum = radarArea.YAxis.Range.Maximum;
            }
            #endregion

            CreateStripLinesAxis(radarArea.YAxis, (float)minimum, (float)maximum);

            if (radarArea.YAxis.Info.StripLines != null && radarArea.YAxis.Info.StripLines.Count > 0)
            {
                radarArea.YAxis.Info.Minimum = radarArea.YAxis.Info.StripLines[radarArea.YAxis.Info.StripLines.Count - 1].Value;
                radarArea.YAxis.Info.Maximum = radarArea.YAxis.Info.StripLines[0].Value;
            }

            radarArea.YAxis.Info.Dpi = (double)rect.Height * 0.5 / radarArea.YAxis.Info.Range;

            CalculateStep(radarArea.YAxis, 0, rect.Height / 2);

            CalculatePositions(radarArea.YAxis, ref radarArea.YAxis.Info.LabelsCollection, radarArea.YAxis.Labels.Step > 0 ? (int)radarArea.YAxis.Labels.Step : 1);
            CalculatePositions(radarArea.YAxis, ref radarArea.YAxis.Info.TicksCollection, radarArea.YAxis.Ticks.Step > 0 ? (int)radarArea.YAxis.Ticks.Step : 1, true);
        }

        protected virtual void CreateStripLinesAxis(IStiYRadarAxis axis, float minimum, float maximum)
        {
            IStiRadarArea radarArea = this.Area as IStiRadarArea;

            double step = radarArea.YAxis.Labels.Step;
            if (step == 0) step = StiStripLineCalculatorXF.GetInterval(minimum, maximum, 6);
            radarArea.YAxis.Info.StripLines = StiStripLineCalculatorXF.GetStripLines(minimum, maximum, step, false);

            foreach (StiStripLineXF stripLine in radarArea.YAxis.Info.StripLines)
            {
                stripLine.ValueObject = string.Format("{0}", stripLine.ValueObject);
            }
        }

        private void CalculateStep(IStiYRadarAxis axis, float topPosition, float bottomPosition)
        {
            if (axis.Info.StripLines.Count >= 2)
            {
                axis.Info.Step = Math.Abs((float)((axis.Info.StripLines[1].Value - axis.Info.StripLines[0].Value) * axis.Info.Dpi));
                axis.YCore.CalculateStripPositions(topPosition, bottomPosition);
            }
            else
                axis.Info.Step = 1;
        }

        /// <summary>
        /// Fill specified collection with values from axis.Info.StripPositions and with taken in consideration step argument.
        /// </summary>
        internal void CalculatePositions(IStiYRadarAxis axis, ref List<StiStripPositionXF> collection, int step, bool calculationForTicks = false)
        {
            collection = new List<StiStripPositionXF>();

            int stepIndex = 0;
            if (axis.Info.StripPositions != null)
            {
                int length = axis.Info.StripPositions.Length;

                for (int index = 0; index < length; index++)
                {
                    if (stepIndex == 0)
                    {
                        var label = new StiStripPositionXF();

                        int stripIndex = index;

                        if (axis.Info.StripLines.Count > 0 && axis.Info.StripPositions.Length > 0)
                        {
                            label.StripLine = axis.Info.StripLines[stripIndex];
                            label.Position = axis.Info.StripPositions[stripIndex];
                        }

                        collection.Add(label);
                    }

                    stepIndex++;

                    if (stepIndex == step || !calculationForTicks) stepIndex = 0;
                }
            }
        }
        #endregion        

        public StiRadarAreaCoreXF(IStiArea area)
            : base(area)
        {            
        }
	}
}
