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
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiXAxisGeom : 
        StiCellGeom        
    {
        #region Properties
        private IStiXAxis axis;
        public IStiXAxis Axis
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

        private StiXAxisViewGeom view;
        public StiXAxisViewGeom View
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
            var penLine = new StiPenGeom(Axis.LineColor, Axis.LineWidth);
            var penArrow = new StiPenGeom(Axis.LineColor);
            var brushLine = new StiSolidBrush(Axis.LineColor);

            float posY = 0;
            if (((StiXAxisCoreXF)Axis.Core).IsBottomSide) posY = rect.Y;
            if (((StiXAxisCoreXF)Axis.Core).IsTopSide) posY = rect.Bottom;

            PointF arrowStart = PointF.Empty;
            if (Axis.Area.ReverseHor) arrowStart = new PointF(rect.X, posY);
            else arrowStart = new PointF(rect.Right, posY);

            #region Draw Arrow
            switch (Axis.ArrowStyle)
            {
                #region StiArrowStyle.Triangle
                case StiArrowStyle.Triangle:
                    context.DrawLine(penLine, arrowStart.X, arrowStart.Y, arrowStart.X + Axis.Core.ArrowHeight * zoom, arrowStart.Y);

                    context.PushSmoothingModeToAntiAlias();

                    List<StiSegmentGeom> path = new List<StiSegmentGeom>();
                    path.Add(new StiLineSegmentGeom(arrowStart.X + Axis.Core.ArrowHeight * zoom, arrowStart.Y - Axis.Core.ArrowWidth * zoom, arrowStart.X + Axis.Core.ArrowHeight * 4 * zoom, arrowStart.Y));
                    path.Add(new StiLineSegmentGeom(arrowStart.X + Axis.Core.ArrowHeight * 4 * zoom, arrowStart.Y, arrowStart.X + Axis.Core.ArrowHeight * zoom, arrowStart.Y + Axis.Core.ArrowWidth * zoom));

                    context.FillPath(brushLine, path, rect, null);

                    context.PopSmoothingMode();
                    break;
                #endregion

                #region StiArrowStyle.Lines
                case StiArrowStyle.Lines:

                    context.DrawLine(penLine, arrowStart.X, arrowStart.Y, arrowStart.X + Axis.Core.ArrowHeight * 4 * zoom, arrowStart.Y);

                    context.PushSmoothingModeToAntiAlias();

                    context.DrawLine(penArrow, arrowStart.X + Axis.Core.ArrowHeight * zoom, arrowStart.Y - Axis.Core.ArrowWidth * zoom, arrowStart.X + Axis.Core.ArrowHeight * 4 * zoom, arrowStart.Y);
                    context.DrawLine(penArrow, arrowStart.X + Axis.Core.ArrowHeight * 4 * zoom, arrowStart.Y, arrowStart.X + Axis.Core.ArrowHeight * zoom, arrowStart.Y + Axis.Core.ArrowWidth * zoom);

                    context.PopSmoothingMode();
                    break;
                #endregion

                #region StiArrowStyle.Circle
                case StiArrowStyle.Circle:
                    float armCircle = Axis.Core.ArrowHeight * zoom;

                    context.DrawLine(
                        penLine, arrowStart.X, arrowStart.Y, arrowStart.X + armCircle * 2, arrowStart.Y);
                    
                    PointF pointCircle = new PointF(arrowStart.X + armCircle * 2, arrowStart.Y - armCircle / 2);

                    context.PushSmoothingModeToAntiAlias();
                    RectangleF rectCircle = new RectangleF(pointCircle.X, pointCircle.Y, armCircle, armCircle);
                    context.DrawEllipse(penLine, rectCircle);
                    context.PopSmoothingMode();
                    break;
                #endregion

                #region StiArrowStyle.Arc and ArcAndCircle
                case StiArrowStyle.Arc:
                case StiArrowStyle.ArcAndCircle:

                    float armArc = Axis.Core.ArrowHeight * zoom;

                    context.DrawLine(
                        penLine, arrowStart.X, arrowStart.Y, arrowStart.X + armArc * 2, arrowStart.Y);                    
                    
                    context.PushSmoothingModeToAntiAlias();

                    #region Arc
                    List<StiSegmentGeom> pathArc = new List<StiSegmentGeom>();

                    if (Axis.Area.ReverseHor)
                    {
                        PointF pointArc = new PointF(arrowStart.X + armArc * 3, arrowStart.Y + armArc / 2);
                        RectangleF rectArc = new RectangleF(pointArc.X, pointArc.Y, Math.Abs(armArc), Math.Abs(armArc));
                        if (Axis.ArrowStyle == StiArrowStyle.ArcAndCircle)
                        {
                            rectArc.X -= zoom;
                            rectArc.Inflate(1, 1);
                        }
                        pathArc.Add(new StiArcSegmentGeom(rectArc, 270, 180));
                    }
                    else
                    {
                        PointF pointArc = new PointF(arrowStart.X + armArc * 2, arrowStart.Y - armArc / 2);
                        RectangleF rectArc = new RectangleF(pointArc.X, pointArc.Y, Math.Abs(armArc), Math.Abs(armArc));
                        if (Axis.ArrowStyle == StiArrowStyle.ArcAndCircle)
                        {
                            rectArc.X += zoom;
                            rectArc.Inflate(1, 1);
                        }
                        pathArc.Add(new StiArcSegmentGeom(rectArc, 90, 180));
                    }

                    context.DrawPath(penLine, pathArc, StiPathGeom.GetBoundsState);
                    #endregion

                    #region Circle
                    if (Axis.ArrowStyle == StiArrowStyle.ArcAndCircle)
                    {
                        float armAC = Axis.Core.ArrowHeight * zoom;

                        PointF pointAC = new PointF(arrowStart.X + armAC * 2 + zoom, arrowStart.Y - armAC / 2);
                        RectangleF rectAC = new RectangleF(pointAC.X, pointAC.Y, armAC, armAC);

                        if (Axis.Area.ReverseHor)
                        {
                            rectAC.Inflate(0.5f * zoom, 0.5f * zoom);
                            rectAC.X -= zoom * 3;
                        }
                        else
                        {
                            rectAC.Inflate(-0.5f * zoom, -0.5f * zoom);
                            rectAC.X += zoom;
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
            float posY = 0;

            if (((StiXAxisCoreXF)Axis.Core).IsTopSide) posY = rect.Bottom;
            if (((StiXAxisCoreXF)Axis.Core).IsBottomSide) posY = rect.Top;

            var penLine = new StiPenGeom(Axis.LineColor, Axis.LineWidth);
            penLine.PenStyle = Axis.LineStyle;

            if (IsCenterAxis && (Axis.ShowXAxis == StiShowXAxis.Both || Axis.ShowXAxis == StiShowXAxis.Center))
            {
                if (Axis.Area.ReverseVert)
                    context.DrawLine(penLine, rect.X, posY, rect.Right, posY);
                else
                    context.DrawLine(penLine, rect.X, posY + rect.Height, rect.Right, posY + rect.Height);
            }
            else if (!IsCenterAxis && (Axis.ShowXAxis == StiShowXAxis.Both || Axis.ShowXAxis == StiShowXAxis.Bottom))
                context.DrawLine(penLine, rect.X, posY, rect.Right, posY);
        }        

        private void DrawMinorTicks(StiContext context, StiPenGeom pen, float posX1, float posX2, float posY, IStiAxisTicks ticks)
        {
            float step = posX2 - posX1;
            float minorStep = step / (ticks.MinorCount + 1);

            float minorLength = ticks.MinorLength * context.Options.Zoom;
            for (int minorIndex = 1; minorIndex <= ticks.MinorCount; minorIndex++)
            {
                float posX = posX1 + minorStep * minorIndex;
                float posY2 = ((StiXAxisCoreXF)Axis.Core).IsTopSide || IsCenterAxis ? posY - minorLength : posY + minorLength;
                if (Axis.Area.ReverseVert && IsCenterAxis)
                    posY2 = posY + minorLength;

                context.DrawLine(pen, posX, posY, posX, posY2);
            }
        }

        private void DrawTicks(StiContext context, RectangleF rect, IStiAxisTicks ticks, StiPenGeom penLine)
        {
            if (!ticks.Visible) return;

            var ticksLength = ticks.Length * context.Options.Zoom;

            var posY1 = ((StiXAxisCoreXF)Axis.Core).IsTopSide || IsCenterAxis ? rect.Bottom : rect.Top;
            var posY2 = ((StiXAxisCoreXF)Axis.Core).IsTopSide || IsCenterAxis ? posY1 - ticksLength : posY1 + ticksLength;
            if (IsCenterAxis && Axis.Area.ReverseVert)
                posY2 = posY1 + ticksLength;

            ticksLength = ticks.LengthUnderLabels * context.Options.Zoom;

            var posY1LengthUnderLabels = ((StiXAxisCoreXF)Axis.Core).IsTopSide || IsCenterAxis ? rect.Bottom : rect.Top;
            var posY2LengthUnderLabels = ((StiXAxisCoreXF)Axis.Core).IsTopSide || IsCenterAxis ? posY1LengthUnderLabels - ticksLength : posY1LengthUnderLabels + ticksLength;
            if (IsCenterAxis && Axis.Area.ReverseVert)
                posY2LengthUnderLabels = posY1LengthUnderLabels + ticksLength;

            var pointXTickUnderLabels = this.Axis.Info.LabelInfoCollection?.ToArray().Select(l => l.TextPoint.X);

            var infos = Axis.Info.TicksCollection;
            if (IsArgumentDateTime(infos) && ((StiXBottomAxis)this.Axis.Area.XAxis).DateTimeStep.Step != StiTimeDateStep.None)
            {
                #region Ticks for DateTime
                var date = new DateTime();
                var dateNext = new DateTime();

                bool startTick = true;

                string text = string.Empty;

                float startX = 0;
                for (int index = 0; index < infos.Count; index++)
                {
                    if (startTick)
                    {
                        startX = infos[index].Position;
                        startTick = false;
                    }

                    if (!(infos[index].StripLine.ValueObject is DateTime))
                        continue;

                    date = (DateTime)infos[index].StripLine.ValueObject;                    

                    if (index < infos.Count - 2)
                    {
                        dateNext = (DateTime)infos[index + 1].StripLine.ValueObject;
                    }
                    else
                    {
                        dateNext = date;
                    }
                    

                    switch (((StiXBottomAxis)this.Axis.Area.XAxis).DateTimeStep.Step)
                    {
                        case StiTimeDateStep.Day:
                            if (date.ToString("yyyyMMMMdd") != dateNext.ToString("yyyyMMMMdd") || index == infos.Count - 2)
                                text = date.ToString("dd");
                            break;

                        case StiTimeDateStep.Hour:
                            if (date.ToString("yyyyMMMMddHH") != dateNext.ToString("yyyyMMMMddHH") || index == infos.Count - 2)
                                text = date.ToString("HH");
                            break;

                        case StiTimeDateStep.Minute:
                            if (date.ToString("yyyyMMMMddhhmm") != dateNext.ToString("yyyyMMMMddhhmm") || index == infos.Count - 2)
                                text = date.ToString("mm");
                            break;

                        case StiTimeDateStep.Month:
                            if (date.ToString("yyyyMMMM") != dateNext.ToString("yyyyMMMM") || index == infos.Count - 2)
                                text = date.ToString("MMMM");
                            break;

                        case StiTimeDateStep.Second:
                            if (date.ToString("yyyyMMMMddHHmmss") != dateNext.ToString("yyyyMMMMddHHmmss") || index == infos.Count - 2)
                                text = date.ToString("ss");
                            break;

                        case StiTimeDateStep.Year:
                            if (date.ToString("yyyy") != dateNext.ToString("yyyy") || index == infos.Count - 2)
                                text = date.ToString("yyyy");
                            break;
                    }

                    if (text != string.Empty)
                    {
                        float endX = 0;
                        if (index == infos.Count - 2)
                        {
                            endX = infos[index + 1].Position;
                        }
                        else
                        {
                            endX = infos[index].Position + (infos[index + 1].Position - infos[index].Position) / 2;
                        }

                        context.DrawRectangle(penLine, startX, posY1, endX - startX, posY2);

                        text = string.Empty;
                        startX = endX;
                    }

                }
                #endregion
            }
            else
            {
                int index = 0;
                foreach (StiStripPositionXF strip in Axis.Info.TicksCollection)
                {
                    float posX = strip.Position;

                    if (pointXTickUnderLabels != null && pointXTickUnderLabels.Contains(posX))
                        context.DrawLine(penLine, posX, posY1LengthUnderLabels, posX, posY2LengthUnderLabels);
                    else
                        context.DrawLine(penLine, posX, posY1, posX, posY2);

                    if (ticks.MinorVisible && index != Axis.Info.TicksCollection.Count - 1)
                    {
                        float posX2 = Axis.Info.TicksCollection[index + 1].Position;
                        DrawMinorTicks(context, penLine, posX, posX2, posY1, ticks);
                    }
                    index++;
                }
            }
        }

        private bool IsArgumentDateTime(List<StiStripPositionXF> infos)
        {
            foreach (StiStripPositionXF info in infos)
            {
                if (info.StripLine.ValueObject is DateTime)
                    return true;
            }
            return false;
        }

        private void DrawAxis(StiContext context, RectangleF rect)
        {
            var penLine = new StiPenGeom(Axis.LineColor, Axis.LineWidth);
            penLine.PenStyle = Axis.LineStyle;

            if (IsCenterAxis && (Axis.ShowXAxis == StiShowXAxis.Both || Axis.ShowXAxis == StiShowXAxis.Center))
            {
                DrawTicks(context, rect, Axis.Ticks, penLine);
            }
            else if (!IsCenterAxis)
            {
                if (((StiXAxisCoreXF)Axis.Core).IsTopSide)
                    DrawTicks(context, rect, Axis.Ticks, penLine);
                if (((StiXAxisCoreXF)Axis.Core).IsBottomSide && (Axis.ShowXAxis == StiShowXAxis.Both || Axis.ShowXAxis == StiShowXAxis.Bottom))
                    DrawTicks(context, rect, Axis.Ticks, penLine);
            }
                                    
            DrawAxisLine(context, rect);             
        }        
       
        private RectangleF GetViewClipRect()
        {
            var clipRect = this.View.ClientRectangle;
            clipRect.X = (float)((StiAxisAreaCoreXF)Axis.Area.Core).ScrollDistanceX;
            clipRect.Y = 0;
            clipRect.Inflate(1, 1);
            
            if (this.IsCenterAxis)
                clipRect.Inflate(0, 2);

            return clipRect;
        }

        protected override bool AllowChildDrawing(StiCellGeom cellGeom)
        {
            StiAxisLabelGeom geom = cellGeom as StiAxisLabelGeom;
            if (geom == null)
                return true;

            if (this.View == null)
                return true;


            var clipRect = GetViewClipRect();
            var geomRect = geom.ClientRectangle;

            return !(geomRect.Right < clipRect.X || geomRect.X > clipRect.Right);
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
                RectangleF clipRect = GetViewClipRect();
                context.PushClip(clipRect);
            }
            #endregion

            DrawAxis(context, rect);

            if (this.View != null)
                context.PopClip();
        }
        #endregion

        public StiXAxisGeom(IStiXAxis axis, RectangleF clientRectangle, bool isCenterAxis)
            : base(clientRectangle)
        {
            this.axis = axis;
            this.isCenterAxis = isCenterAxis;
        }
    }
}
