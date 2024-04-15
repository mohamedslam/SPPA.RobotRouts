#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Text;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiAxisAreaGeom : StiAreaGeom
    {
        #region class StiLineF
        private class StiLineF
        {
            public float X1 { get; set; }

            public float Y1 { get; set; }

            public float X2 { get; set; }

            public float Y2 { get; set; }

            public StiLineF(float x1, float y1, float x2, float y2)
            {
                this.X1 = x1;
                this.Y1 = y1;
                this.X2 = x2;
                this.Y2 = y2;
            }
        }
        #endregion

        #region Properties
        private StiAxisAreaViewGeom view;
        public StiAxisAreaViewGeom View
        {
            get
            {
                return view;
            }
        }
        #endregion

        #region Fields
        private float minWidth = 6;
        #endregion

        #region Methods.Draw.Interlacing
        private void DrawInterlacingHor(StiContext context, RectangleF rect)
        {
            rect.X = 0;
            rect.Width = view.ClientRectangle.Width;

            IStiAxisArea axisArea = Area as IStiAxisArea;

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
                float step = firstPosition - pos;

                if (step < minWidth && indexTemp != positions.Length)
                {
                    continue;
                }
                firstPosition = pos;
                positionsTemp.Add(pos);
            }

            float[] positionsNew = new float[positionsTemp.Count];

            positionsTemp.CopyTo(positionsNew);

            int index = 0;
            bool draw = true;

            foreach (float posY in positionsNew)
            {
                float posY2 = positionsNew[index+1];

                if (draw)
                {
                    RectangleF stripRect = new RectangleF(rect.X, rect.Y + posY2, rect.Width, posY - posY2);
                    if (stripRect.Bottom >= 0 && stripRect.Y < view.ClientRectangle.Height)
                        context.FillRectangle(axisArea.InterlacingHor.InterlacedBrush, stripRect.X, stripRect.Y, stripRect.Width, stripRect.Height, null);
                }
                draw = !draw;

                index++;
                if (index == positionsNew.Length - 1)
                    break;
            }
        }

        private void DrawInterlacingVer(StiContext context, RectangleF rect)
        {
            rect.Y = 0;
            rect.Height = view.ClientRectangle.Height;

            IStiAxisArea axisArea = Area as IStiAxisArea;

            if (!axisArea.InterlacingVert.Visible)
                return;

            int index = 0;
            int areaIndex = 0;
            if (axisArea.ReverseHor) areaIndex = 1;

            List<float> positionsTemp = new List<float>();
            float[] positions = axisArea.XAxis.Info.StripPositions;

            float firstPosition = 0;

            bool first = true;
            int indexTemp = 0;

            foreach(float pos in positions)
            {
                indexTemp++;
                if (first)
                {
                    firstPosition = pos;
                    positionsTemp.Add(pos);
                    first = false;
                    continue;
                }
                float step = pos - firstPosition;

                if (step < minWidth && indexTemp != positions.Length)
                {
                    continue;
                }
                firstPosition = pos;
                positionsTemp.Add(pos);
            }

            float[] positionsNew = new float[positionsTemp.Count];

            positionsTemp.CopyTo(positionsNew);

            foreach (float posX in positionsNew)
            {
                float posX2 = positionsNew[index + 1];

                if ((areaIndex & 1) > 0)
                {
                    RectangleF stripRect = new RectangleF(posX + rect.X, rect.Y, posX2 - posX, rect.Height);
                    if (stripRect.Right >= 0 && stripRect.X < view.ClientRectangle.Width)
                        context.FillRectangle(axisArea.InterlacingVert.InterlacedBrush,
                            stripRect.X, stripRect.Y, stripRect.Width, stripRect.Height, null);
                }

                areaIndex++;
                index++;
                if (index == positionsNew.Length - 1)
                    break;
            }
        }
        #endregion

        #region Methods.Draw.GridLines
        private List<StiLineF> GetGridLinesHorMajor(RectangleF rect, IStiGridLinesHor gridLinesHor, bool isLeftAxis)
        {
            var lines = new List<StiLineF>();

            rect.X = 0;
            rect.Width = view.ClientRectangle.Width;

            var axisArea = Area as IStiAxisArea;

            var positions = isLeftAxis ? axisArea.YAxis.Info.StripPositions : axisArea.YRightAxis.Info.StripPositions;

            #region Reorder Values
            int count = positions.Length;
            float[] positionsTemp = new float[count];

            for (int indexTemp = 0; indexTemp < count; indexTemp++)
            {
                positionsTemp[count - indexTemp - 1] = positions[indexTemp];
            }

            positions = positionsTemp;
            #endregion

            if (positions.Length > 0)
            {
                float firstPosition = positions[0];

                int index = 0;
                foreach (float posY in positions)
                {
                    #region Pass Line if small Step
                    float step = firstPosition - posY;

                    if (step < minWidth && step > 0)
                    {
                        continue;
                    }

                    firstPosition = posY;
                    #endregion

                    if (gridLinesHor.Visible && gridLinesHor.Style != StiPenStyle.None)
                    {
                        float pointY = posY + rect.Y;
                        if (pointY >= 0 && pointY <= this.ClientRectangle.Height)
                            lines.Add(new StiLineF(rect.X, pointY, rect.Right, pointY));
                    }

                    index++;
                }
            }

            return lines;
        }

        private List<StiLineF> GetGridLinesHorMinor(RectangleF rect, IStiGridLinesHor gridLinesHor, bool isLeftAxis)
        {
            var lines = new List<StiLineF>();

            rect.X = 0;
            rect.Width = view.ClientRectangle.Width;

            var axisArea = Area as IStiAxisArea;

            var positions = isLeftAxis ? axisArea.YAxis.Info.StripPositions : axisArea.YRightAxis.Info.StripPositions;

            #region Reorder Values
            int count = positions.Length;
            float[] positionsTemp = new float[count];

            for (int indexTemp = 0; indexTemp < count; indexTemp++)
            {
                positionsTemp[count - indexTemp - 1] = positions[indexTemp];
            }

            positions = positionsTemp;
            #endregion

            if (positions.Length > 0)
            {
                float firstPosition = positions[0];

                int index = 0;
                foreach (float posY in positions)
                {
                    #region Pass Line if small Step
                    float step = firstPosition - posY;

                    if (step < minWidth && step > 0)
                    {
                        continue;
                    }

                    firstPosition = posY;
                    #endregion

                    if (gridLinesHor.MinorVisible && gridLinesHor.MinorStyle != StiPenStyle.None && index != positions.Length - 1)
                    {
                        float posY2 = positions[index + 1];

                        if ((posY - posY2) < minWidth) continue;

                        int minorCount = gridLinesHor.MinorCount == 0 ? axisArea.YAxis.Ticks.MinorCount : gridLinesHor.MinorCount;

                        float posMinorY = posY;
                        float minorStep = (posY2 - posY) / (minorCount + 1);

                        for (int minorIndex = 1; minorIndex <= minorCount; minorIndex++)
                        {
                            posMinorY = posY + minorStep * minorIndex;

                            float pointMinorY = posMinorY + rect.Y;
                            if (pointMinorY >= 0 && pointMinorY <= this.ClientRectangle.Height)
                                lines.Add(new StiLineF(rect.X, pointMinorY, rect.Right, pointMinorY));
                        }
                    }
                    index++;
                }
            }

            return lines;
        }

        private List<StiLineF> GetGridLinesVerMajor(RectangleF rect, IStiGridLinesVert gridLinesVert, bool isBottomAxis)
        {
            var lines = new List<StiLineF>();

            rect.Y = 0;
            rect.Height = view.ClientRectangle.Height;

            var axisArea = Area as IStiAxisArea;

            float[] positions = isBottomAxis ? axisArea.XAxis.Info.StripPositions : axisArea.XTopAxis.Info.StripPositions;

            float firstPosition = 0;

            int index = 0;
            foreach (float posX in positions)
            {
                #region Pass Line if small Step
                float step = posX - firstPosition;

                if (step < minWidth && step > 0)
                {
                    continue;
                }

                firstPosition = posX;
                #endregion

                float scaledLineWidth = 1f;
                if (gridLinesVert.Visible && gridLinesVert.Style != StiPenStyle.None)
                {
                    StiPenGeom penGridLine = new StiPenGeom(gridLinesVert.Color, scaledLineWidth);
                    penGridLine.PenStyle = gridLinesVert.Style;

                    float pointX = posX + rect.X;
                    if (pointX >= 0 && pointX <= this.ClientRectangle.Width)
                        lines.Add(new StiLineF(pointX, rect.Y, pointX, rect.Bottom));

                }

                index++;
            }

            return lines;
        }

        private List<StiLineF> GetGridLinesVerMinor(RectangleF rect, IStiGridLinesVert gridLinesVert, bool isBottomAxis)
        {
            var lines = new List<StiLineF>();

            rect.Y = 0;
            rect.Height = view.ClientRectangle.Height;

            IStiAxisArea axisArea = Area as IStiAxisArea;

            float[] positions = isBottomAxis ? axisArea.XAxis.Info.StripPositions : axisArea.XTopAxis.Info.StripPositions;

            float firstPosition = 0;

            int index = 0;
            foreach (float posX in positions)
            {
                #region Pass Line if small Step
                float step = posX - firstPosition;

                if (step < minWidth && step > 0)
                {
                    continue;
                }

                firstPosition = posX;
                #endregion


                if (gridLinesVert.MinorVisible && gridLinesVert.MinorStyle != StiPenStyle.None && index != positions.Length - 1)
                {
                    float posX2 = positions[index + 1];

                    if ((posX2 - posX) < minWidth) continue;

                    int minorCount = gridLinesVert.MinorCount == 0 ? axisArea.YAxis.Ticks.MinorCount : gridLinesVert.MinorCount;

                    float posMinorX = posX;
                    float minorStep = (posX2 - posX) / ((float)minorCount + 1);

                    for (int minorIndex = 1; minorIndex <= minorCount; minorIndex++)
                    {
                        posMinorX = posX + minorStep * minorIndex;
                        float pointMinorX = posMinorX + rect.X;
                        if (pointMinorX >= 0 && pointMinorX <= this.ClientRectangle.Width)
                            lines.Add(new StiLineF(pointMinorX, rect.Y, pointMinorX, rect.Bottom));
                    }
                }
                index++;
            }

            return lines;
        }
        #endregion

        #region Methods
        protected override bool AllowChildDrawing(StiCellGeom cellGeom)
        {
            return cellGeom != null && IsChildVisibleInView(cellGeom);
        }

        /// <summary>
        /// Returns true if specified child visible in area.
        /// </summary>
        /// <param name="cellGeom"></param>
        /// <returns></returns>
        public bool IsChildVisibleInView(StiCellGeom cellGeom)
        {
            RectangleF clipRect = View.ClientRectangle;
            clipRect.X = 0;
            clipRect.Y = 0;

            clipRect.Inflate(1, 1);

            RectangleF cellRect = cellGeom.ClientRectangle;
            cellRect.X -= (float)((StiAxisAreaCoreXF)Area.Core).ScrollDistanceX;
            cellRect.Y -= (float)((StiAxisAreaCoreXF)Area.Core).ScrollDistanceY;

            return cellGeom != null && clipRect.IntersectsWith(cellRect);
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            base.Draw(context);

            IStiAxisArea axisArea = this.Area as IStiAxisArea;

            RectangleF rect = this.ClientRectangle;
            if (rect.IsEmpty) return;

            List<IStiSeries> seriesCollection = axisArea.AxisCore.GetSeries();

            #region Draw Interlacing
            if (seriesCollection.Count > 0)
            {
                DrawInterlacingHor(context, rect);
                DrawInterlacingVer(context, rect);
            }
            #endregion

            #region Draw GridLines
            if (seriesCollection.Count > 0)
            {
                var linesHorMajor = GetGridLinesHorMajor(rect, axisArea.GridLinesHor, true);
                var linesHorMinor = GetGridLinesHorMinor(rect, axisArea.GridLinesHor, true);

                var linesHorMajorRight = GetGridLinesHorMajor(rect, axisArea.GridLinesHorRight, true);
                var linesHorMinorRight = GetGridLinesHorMinor(rect, axisArea.GridLinesHorRight, true);
                
                var linesVerMajor = GetGridLinesVerMajor(rect, axisArea.GridLinesVert, true);
                var linesVerMinor = GetGridLinesVerMinor(rect, axisArea.GridLinesVert, true);

                #region Draw Minors
                var penGridLine = new StiPenGeom(axisArea.GridLinesHor.MinorColor, 1);
                penGridLine.PenStyle = axisArea.GridLinesHor.MinorStyle;
                DrawLines(context, linesHorMinor, penGridLine);

                penGridLine = new StiPenGeom(axisArea.GridLinesHorRight.MinorColor, 1);
                penGridLine.PenStyle = axisArea.GridLinesHorRight.MinorStyle;
                DrawLines(context, linesHorMinorRight, penGridLine);

                penGridLine = new StiPenGeom(axisArea.GridLinesVert.MinorColor, 1);
                penGridLine.PenStyle = axisArea.GridLinesVert.MinorStyle;
                DrawLines(context, linesVerMinor, penGridLine);
                #endregion

                #region Major
                penGridLine = new StiPenGeom(axisArea.GridLinesHor.Color, 1);
                penGridLine.PenStyle = axisArea.GridLinesHor.Style;
                DrawLines(context, linesHorMajor, penGridLine);

                penGridLine = new StiPenGeom(axisArea.GridLinesHorRight.Color, 1);
                penGridLine.PenStyle = axisArea.GridLinesHorRight.Style;
                DrawLines(context, linesHorMajorRight, penGridLine);

                penGridLine = new StiPenGeom(axisArea.GridLinesVert.Color, 1);
                penGridLine.PenStyle = axisArea.GridLinesVert.Style;
                DrawLines(context, linesVerMajor, penGridLine);
                #endregion
            }
            #endregion            
        }

        private void DrawLines(StiContext context, List<StiLineF> lines, StiPenGeom pen)
        {
            foreach( var line in lines)
                context.DrawLine(pen, line.X1, line.Y1, line.X2, line.Y2);
        }
        #endregion

        public StiAxisAreaGeom(StiAxisAreaViewGeom view, IStiArea area, RectangleF clientRectangle)
            : base(area, clientRectangle)
        {
            this.view = view;
        }
    }
}
