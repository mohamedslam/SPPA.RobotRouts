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
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiYAxisGeom : StiCellGeom
    {
        #region Properties
        private IStiYAxis axis;
        public IStiYAxis Axis
        {
            get
            {
                return axis;
            }
        }

        private bool isCenterAxis;
        public bool IsCenterAxis
        {
            get
            {
                return isCenterAxis;
            }
        }

        private StiYAxisViewGeom view;
        public StiYAxisViewGeom View
        {
            get
            {
                return view;
            }
            set
            {
                view = value;
            }
        }
        #endregion

        #region Methods
        protected void DrawArrow(StiContext context, RectangleF rect)
        {
            float zoom = context.Options.Zoom;
            var penLine = new StiPenGeom(Axis.LineColor);
            var brushLine = new StiSolidBrush(Axis.LineColor);
            var penArrow = new StiPenGeom(Axis.LineColor);

            if (!Axis.Visible) DrawAxisLine(context, rect);

            var arrowStart = PointF.Empty;
            if (!Axis.Area.ReverseVert) arrowStart = new PointF(rect.Right, rect.Y);
            else arrowStart = new PointF(rect.Right, rect.Bottom);

            if (((StiYAxisCoreXF)Axis.Core).IsRightSide) arrowStart.X = rect.X;

            #region Draw Arrow
            switch (Axis.ArrowStyle)
            {
                #region StiArrowStyle.Triangle
                case StiArrowStyle.Triangle:
                    context.DrawLine(
                        penLine, arrowStart.X, arrowStart.Y, arrowStart.X, arrowStart.Y - Axis.Core.ArrowHeight * zoom);

                    context.PushSmoothingModeToAntiAlias();

                    List<StiSegmentGeom> path = new List<StiSegmentGeom>();
                    path.Add(new StiLineSegmentGeom(arrowStart.X - Axis.Core.ArrowWidth * zoom, arrowStart.Y - Axis.Core.ArrowHeight * zoom, arrowStart.X + Axis.Core.ArrowWidth * zoom, arrowStart.Y - Axis.Core.ArrowHeight * zoom));
                    path.Add(new StiLineSegmentGeom(arrowStart.X + Axis.Core.ArrowWidth * zoom, arrowStart.Y - Axis.Core.ArrowHeight * zoom, arrowStart.X, arrowStart.Y - Axis.Core.ArrowHeight * 4 * zoom));

                    context.FillPath(brushLine, path, StiPathGeom.GetBoundsState, null);

                    context.PopSmoothingMode();
                    break;
                #endregion

                #region StiArrowStyle.Lines
                case StiArrowStyle.Lines:                    
                    context.DrawLine(
                        penLine, arrowStart.X, arrowStart.Y, arrowStart.X, arrowStart.Y - Axis.Core.ArrowHeight * 4 * zoom);

                    context.PushSmoothingModeToAntiAlias();

                    context.DrawLine(
                        penArrow, arrowStart.X - Axis.Core.ArrowWidth * zoom, arrowStart.Y - Axis.Core.ArrowHeight * zoom, arrowStart.X, arrowStart.Y - Axis.Core.ArrowHeight * 4 * zoom);
                    context.DrawLine(
                        penArrow, arrowStart.X + Axis.Core.ArrowWidth * zoom, arrowStart.Y - Axis.Core.ArrowHeight * zoom, arrowStart.X, arrowStart.Y - Axis.Core.ArrowHeight * 4 * zoom);

                    context.PopSmoothingMode();
                    break;
                #endregion

                #region StiArrowStyle.Circle
                case StiArrowStyle.Circle:
                    context.DrawLine(
                        penLine, arrowStart.X, arrowStart.Y, arrowStart.X, arrowStart.Y - Axis.Core.ArrowHeight * zoom * 2);

                    float armCircle = Axis.Core.ArrowHeight * zoom;
                    PointF pointCircle = new PointF(arrowStart.X - armCircle / 2, arrowStart.Y - armCircle * 3);
                    
                    context.PushSmoothingModeToAntiAlias();
                    RectangleF rectCircle = new RectangleF(pointCircle.X, pointCircle.Y, Axis.Core.ArrowHeight * zoom, Axis.Core.ArrowHeight * zoom);
                    context.DrawEllipse(penLine, rectCircle);
                    context.PopSmoothingMode();
                    break;
                #endregion

                #region StiArrowStyle.Arc and ArcAndCircle
                case StiArrowStyle.Arc:
                case StiArrowStyle.ArcAndCircle:

                    float armArc = Axis.Core.ArrowHeight * zoom;

                    context.DrawLine(
                        penLine, arrowStart.X, arrowStart.Y, arrowStart.X, arrowStart.Y - armArc * 2);
                                        
                    context.PushSmoothingModeToAntiAlias();

                    #region Arc
                    List<StiSegmentGeom> pathArc = new List<StiSegmentGeom>();

                    if (Axis.Area.ReverseVert)
                    {
                        PointF pointArc = new PointF(arrowStart.X + armArc / 2, arrowStart.Y - armArc * 2);
                        RectangleF rectArc = new RectangleF(pointArc.X, pointArc.Y, Math.Abs(armArc), Math.Abs(armArc));
                        if (Axis.ArrowStyle == StiArrowStyle.ArcAndCircle)
                        {
                            rectArc.Y += zoom;
                            rectArc.Inflate(1, 1);
                        }
                        pathArc.Add(new StiArcSegmentGeom(rectArc, 180, 180));
                    }
                    else
                    {
                        PointF pointArc = new PointF(arrowStart.X - armArc / 2, arrowStart.Y - armArc * 3);
                        RectangleF rectArc = new RectangleF(pointArc.X, pointArc.Y, armArc, armArc);
                        if (Axis.ArrowStyle == StiArrowStyle.ArcAndCircle)
                        {
                            rectArc.Y -= zoom;
                            rectArc.Inflate(1, 1);
                        }
                        pathArc.Add(new StiArcSegmentGeom(rectArc, 0, 180));
                    }
                    
                    context.DrawPath(penLine, pathArc, StiPathGeom.GetBoundsState);
                    #endregion

                    #region Circle
                    if (Axis.ArrowStyle == StiArrowStyle.ArcAndCircle)
                    {
                        float armAC = Axis.Core.ArrowHeight * zoom;

                        PointF pointAC = new PointF(arrowStart.X - armAC / 2, arrowStart.Y - armAC * 3 - zoom);
                        RectangleF rectAC = new RectangleF(pointAC.X, pointAC.Y, armAC, armAC);

                        if (Axis.Area.ReverseVert)
                        {
                            rectAC.Inflate(0.5f * zoom, 0.5f * zoom);
                            rectAC.Y += zoom * 3;
                        }
                        else
                        {
                            rectAC.Inflate(-0.5f * zoom, -0.5f * zoom);
                            rectAC.Y -= zoom;
                        }

                        context.FillEllipse(brushLine, rectAC, null);
                    }
                    #endregion

                    context.PopSmoothingMode();
                    break;
                #endregion
            }
            #endregion

        }

        private void DrawAxisLine(StiContext context, RectangleF rect)
        {
            float posX = rect.Right;
            if (((StiYAxisCoreXF)Axis.Core).IsRightSide) posX = rect.Left;
            if (IsCenterAxis && Axis.Area.ReverseHor)
                posX = rect.Right;

            var penLine = new StiPenGeom(Axis.LineColor, Axis.LineWidth);
            penLine.PenStyle = Axis.LineStyle;

            if ((IsCenterAxis && (Axis.ShowYAxis == StiShowYAxis.Both || Axis.ShowYAxis == StiShowYAxis.Center)) ||
                (!IsCenterAxis && (Axis.ShowYAxis == StiShowYAxis.Both || Axis.ShowYAxis == StiShowYAxis.Left)))
            {
                context.DrawLine(penLine, posX, rect.Y, posX, rect.Bottom);
            }
        }

        private void DrawMinorTicks(StiContext context, StiPenGeom pen, float posX, float posY1, float posY2, IStiAxisTicks ticks)
        {
            float step = posY2 - posY1;
            float minorStep = step / (ticks.MinorCount + 1);

            float minorLength = ticks.MinorLength * context.Options.Zoom;
            for (int minorIndex = 1; minorIndex <= ticks.MinorCount; minorIndex++)
            {
                float posY = posY1 + minorStep * minorIndex;
                float posX2 = ((StiYAxisCoreXF)Axis.Core).IsLeftSide || IsCenterAxis ? posX - minorLength : posX + minorLength;
                if (Axis.Area.ReverseHor && IsCenterAxis)
                    posX2 = posX + minorLength;

                context.DrawLine(pen, posX, posY, posX2, posY);
            }
        }

        private void DrawTicks(StiContext context, RectangleF rect, IStiAxisTicks ticks, StiPenGeom penLine)
        {
            if (!ticks.Visible) return;

            float ticksLength = ticks.Length * context.Options.Zoom;
            float posX1 = ((StiYAxisCoreXF)Axis.Core).IsLeftSide ? rect.Right : rect.Left;
            float posX2 = ((StiYAxisCoreXF)Axis.Core).IsLeftSide ? posX1 - ticksLength : posX1 + ticksLength;

            if (IsCenterAxis && Axis.Area.ReverseHor)
            {
                posX1 = rect.Right;
                posX2 = rect.Right + ticksLength;
            }


            int index = 0;
            foreach (StiStripPositionXF strip in Axis.Info.TicksCollection)
            {
                float posY = strip.Position;

                context.DrawLine(penLine, posX1, posY, posX2, posY);

                if (ticks.MinorVisible && index != Axis.Info.TicksCollection.Count - 1)
                {
                    float posY2 = Axis.Info.TicksCollection[index + 1].Position;
                    DrawMinorTicks(context, penLine, posX1, posY, posY2, ticks);
                }
                index++;
            }
        }

        private void DrawAxis(StiContext context, RectangleF rect)
        {
            var penLine = new StiPenGeom(Axis.LineColor, Axis.LineWidth);
            penLine.PenStyle = Axis.LineStyle;

            if (IsCenterAxis && (Axis.ShowYAxis == StiShowYAxis.Both || Axis.ShowYAxis == StiShowYAxis.Center))
            {
                DrawTicks(context, rect, Axis.Ticks, penLine);
            }
            else if (!IsCenterAxis)
            {
                if (((StiYAxisCoreXF)Axis.Core).IsLeftSide && (Axis.ShowYAxis == StiShowYAxis.Both || Axis.ShowYAxis == StiShowYAxis.Left))
                    DrawTicks(context, rect, Axis.Area.YAxis.Ticks, penLine);

                if (((StiYAxisCoreXF)Axis.Core).IsRightSide)
                    DrawTicks(context, rect, Axis.Area.YRightAxis.Ticks, penLine);
            }

            DrawAxisLine(context, rect);
        }
        
        private RectangleF GetViewClipRect()
        {
            var clipRect = this.View.ClientRectangle;
            clipRect.X = 0;
            clipRect.Y = (float)((StiAxisAreaCoreXF)Axis.Area.Core).ScrollDistanceY;
            clipRect.Inflate(1, 1);
            if (this.IsCenterAxis)
                clipRect.Inflate(2, 0);
            return clipRect;
        }

        protected override bool AllowChildDrawing(StiCellGeom cellGeom)
        {
            var geom = cellGeom as StiAxisLabelGeom;
            if (geom == null)
                return true;

            if (this.View == null)
                return true;

            var clipRect = GetViewClipRect();
            var geomRect = geom.ClientRectangle;

            return !(geomRect.Bottom < clipRect.Y || geomRect.Y > clipRect.Bottom);
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var rect = this.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0)
                return;
            
            #region Set View Geom Clip
            if (this.View != null)
            {
                var clipRect = GetViewClipRect();
                context.PushClip(clipRect);
            }
            #endregion

            DrawAxis(context, rect);

            if (this.View != null)
                context.PopClip();
        }
        #endregion

        public StiYAxisGeom(IStiYAxis axis, RectangleF clientRectangle, bool isCenterAxis)
            : base(clientRectangle)
        {
            this.axis = axis;
            this.isCenterAxis = isCenterAxis;
        }
    }
}
