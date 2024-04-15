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
    public class StiRadarAreaGeom : StiAreaGeom
    {
        #region Properties
        private int valuesCount;
        public int ValuesCount
        {
            get
            {
                return valuesCount;
            }
        }
        #endregion

        #region Methods
        private void DrawHor(StiContext context, bool fill, bool draw)
        {

            RectangleF rect = this.ClientRectangle;
            IStiRadarArea radarArea = this.Area as IStiRadarArea;
            if (radarArea.YAxis.Info.StripPositions == null) return;
            StiRadarAreaCoreXF radarAreaCore = radarArea.Core as StiRadarAreaCoreXF;

            int index = 0;

            foreach (float radius in radarArea.YAxis.Info.StripPositions)
            {
                if (radius != 0)
                {
                    RectangleF arcRect = new RectangleF(
                        radarAreaCore.CenterPoint.X - radius,
                        radarAreaCore.CenterPoint.Y - radius,
                        radius * 2, radius * 2);

                    #region Circle
                    if (radarArea.RadarStyle == StiRadarStyle.Circle)
                    {
                        if (fill && radarArea.InterlacingHor.Visible)
                        {
                            if (index != radarArea.YAxis.Info.StripPositions.Length - 1 && ((index & 1) == 0))
                            {
                                List<StiSegmentGeom> path = new List<StiSegmentGeom>();

                                float radius2 = radarArea.YAxis.Info.StripPositions[index + 1];

                                RectangleF arcRect2 = new RectangleF(
                                    radarAreaCore.CenterPoint.X - radius2,
                                    radarAreaCore.CenterPoint.Y - radius2,
                                    radius2 * 2, radius2 * 2);

                                path.Add(new StiArcSegmentGeom(arcRect, 0, 360));
                                path.Add(new StiArcSegmentGeom(arcRect2, 0, 360));

                                context.FillPath(radarArea.InterlacingHor.InterlacedBrush, path, rect, null);
                            }
                        }

                        if (draw && radarArea.GridLinesHor.Visible)
                        {
                            StiPenGeom pen;

                            if (index == radarArea.YAxis.Info.StripPositions.Length - 1)
                            {
                                pen = new StiPenGeom(radarArea.BorderColor, Area.BorderThickness);
                            }
                            else
                            {
                                pen = new StiPenGeom(radarArea.GridLinesHor.Color);
                                pen.PenStyle = radarArea.GridLinesHor.Style;
                            }
                            context.DrawEllipse(pen, arcRect);
                        }
                    }
                    #endregion

                    #region Polygon
                    else
                    {
                        #region InterlacingHor
                        if (fill && radarArea.InterlacingHor.Visible)
                        {
                            if (index != radarArea.YAxis.Info.StripPositions.Length - 1 && ((index & 1) == 1))
                            {
                                List<StiSegmentGeom> path = new List<StiSegmentGeom>();

                                #region Inside arcs
                                int pointIndex2 = 0;
                                foreach (PointF curPoint in radarAreaCore.Points)
                                {
                                    PointF nextPoint;

                                    if (pointIndex2 == radarAreaCore.Points.Count - 1)
                                        nextPoint = radarAreaCore.Points[0];
                                    else
                                        nextPoint = radarAreaCore.Points[pointIndex2 + 1];

                                    PointF point1 = new PointF(
                                        radarAreaCore.CenterPoint.X + radius * curPoint.X,
                                        radarAreaCore.CenterPoint.Y + radius * curPoint.Y);

                                    PointF point2 = new PointF(
                                        radarAreaCore.CenterPoint.X + radius * nextPoint.X,
                                        radarAreaCore.CenterPoint.Y + radius * nextPoint.Y);

                                    path.Add(new StiLineSegmentGeom(point1, point2));

                                    pointIndex2++;

                                }
                                #endregion

                                #region Outside arcs
                                float radius2 = radarArea.YAxis.Info.StripPositions[index + 1];

                                pointIndex2 = 0;
                                foreach (PointF curPoint in radarAreaCore.Points)
                                {
                                    PointF nextPoint;

                                    if (pointIndex2 == radarAreaCore.Points.Count - 1)
                                        nextPoint = radarAreaCore.Points[0];
                                    else
                                        nextPoint = radarAreaCore.Points[pointIndex2 + 1];

                                    PointF point1 = new PointF(
                                        radarAreaCore.CenterPoint.X + radius2 * curPoint.X,
                                        radarAreaCore.CenterPoint.Y + radius2 * curPoint.Y);

                                    PointF point2 = new PointF(
                                        radarAreaCore.CenterPoint.X + radius2 * nextPoint.X,
                                        radarAreaCore.CenterPoint.Y + radius2 * nextPoint.Y);

                                    path.Add(new StiLineSegmentGeom(point1, point2));

                                    pointIndex2++;
                                }
                                #endregion

                                context.FillPath(radarArea.InterlacingHor.InterlacedBrush, path, rect, null);
                            }
                        }
                        #endregion

                        #region GridLinesHor
                        if (draw && radarArea.GridLinesHor.Visible)
                        {
                            int pointIndex = 0;
                            foreach (PointF curPoint in radarAreaCore.Points)
                            {
                                PointF nextPoint;

                                if (pointIndex == radarAreaCore.Points.Count - 1)
                                    nextPoint = radarAreaCore.Points[0];
                                else
                                    nextPoint = radarAreaCore.Points[pointIndex + 1];

                                StiPenGeom pen;

                                if (index == radarArea.YAxis.Info.StripPositions.Length - 1)
                                {
                                    pen = new StiPenGeom(radarArea.BorderColor, Area.BorderThickness);
                                }
                                else
                                {
                                    pen = new StiPenGeom(radarArea.GridLinesHor.Color);
                                    pen.PenStyle = radarArea.GridLinesHor.Style;
                                }

                                context.DrawLine(pen,
                                    radarAreaCore.CenterPoint.X + radius * curPoint.X,
                                    radarAreaCore.CenterPoint.Y + radius * curPoint.Y,
                                    radarAreaCore.CenterPoint.X + radius * nextPoint.X,
                                    radarAreaCore.CenterPoint.Y + radius * nextPoint.Y);

                                pointIndex++;
                            }
                        }
                        #endregion
                    }
                    #endregion
                }

                index++;
            }
        }

        private void DrawVert(StiContext context, bool fill, bool draw)
        {
            RectangleF rect = this.ClientRectangle;
            IStiRadarArea radarArea = this.Area as IStiRadarArea;
            StiRadarAreaCoreXF radarAreaCore = radarArea.Core as StiRadarAreaCoreXF;

            int index = 0;
            float sweepAngle = 360f / radarAreaCore.Points.Count;
            float startAngle = 0;
            foreach (PointF endPoint in radarAreaCore.Points)
            {
                PointF point1 = radarAreaCore.CenterPoint;
                PointF point2 = new PointF(
                    point1.X + rect.Width / 2 * endPoint.X, point1.Y + rect.Width / 2 * endPoint.Y);

                #region InterlacingVert
                if (fill && radarArea.InterlacingVert.Visible)
                {
                    PointF point3 = index < radarAreaCore.Points.Count - 1 ?
                        new PointF(
                            point1.X + rect.Width / 2 * radarAreaCore.Points[index + 1].X,
                            point1.Y + rect.Height / 2 * radarAreaCore.Points[index + 1].Y) :
                        new PointF(
                            point1.X + rect.Width / 2 * radarAreaCore.Points[0].X,
                            point1.Y + rect.Height / 2 * radarAreaCore.Points[0].Y);

                    if ((index & 1) == 1)
                    {
                        //PointF[] points = new PointF[] { point1, point2, point3 };

                        List<StiSegmentGeom> path = new List<StiSegmentGeom>();

                        #region Circle
                        if (radarArea.RadarStyle == StiRadarStyle.Circle)
                        {
                            path.Add(new StiLineSegmentGeom(point1, point2));
                            path.Add(new StiArcSegmentGeom(rect, startAngle - 90, sweepAngle));
                            path.Add(new StiLineSegmentGeom(point3, point1));
                        }
                        #endregion

                        #region Polygon
                        else
                        {
                            path.Add(new StiLineSegmentGeom(point1, point2));
                            path.Add(new StiLineSegmentGeom(point2, point3));
                            path.Add(new StiLineSegmentGeom(point3, point1));
                        }
                        #endregion

                        context.FillPath(radarArea.InterlacingVert.InterlacedBrush, path, rect, null);
                    }
                }
                #endregion

                #region GridLinesVert
                if (draw && radarArea.GridLinesVert.Visible)
                {
                    StiPenGeom pen = new StiPenGeom(radarArea.GridLinesVert.Color);
                    pen.PenStyle = radarArea.GridLinesVert.Style;

                    context.DrawLine(pen, point1.X, point1.Y, point2.X, point2.Y);
                }
                #endregion

                startAngle += sweepAngle;
                index++;
            }
        }

        private void DrawBackground(StiContext context)
        {
            RectangleF rect = this.ClientRectangle;
            IStiRadarArea radarArea = this.Area as IStiRadarArea;
            if (radarArea.YAxis.Info.StripPositions == null) return;
            StiRadarAreaCoreXF radarAreaCore = radarArea.Core as StiRadarAreaCoreXF;

            var shadowSize = 3;
            var shadowBrush = new StiSolidBrush(Color.FromArgb(50, 0, 0, 0));

            if (radarArea.RadarStyle == StiRadarStyle.Circle)
            {
                if (Area.ShowShadow && !StiBrush.IsTransparent(radarArea.Brush))
                {
                    context.PushTranslateTransform(shadowSize, shadowSize);
                    context.FillEllipse(shadowBrush, rect, null);
                    context.PopTransform();
                }

                context.FillEllipse(radarArea.Brush, rect, null);
            }
            else
            {
                float radius = radarArea.YAxis.Info.StripPositions[radarArea.YAxis.Info.StripPositions.Length - 1];
                List<StiSegmentGeom> path = new List<StiSegmentGeom>();
                int pointIndex = 0;
                foreach (PointF curPoint in radarAreaCore.Points)
                {
                    PointF nextPoint = pointIndex == radarAreaCore.Points.Count - 1 ?
                        radarAreaCore.Points[0] :
                        radarAreaCore.Points[pointIndex + 1];
                    path.Add(new StiLineSegmentGeom(
                        radarAreaCore.CenterPoint.X + radius * curPoint.X,
                        radarAreaCore.CenterPoint.Y + radius * curPoint.Y,
                        radarAreaCore.CenterPoint.X + radius * nextPoint.X,
                        radarAreaCore.CenterPoint.Y + radius * nextPoint.Y
                    ));
                    pointIndex++;
                }

                if (Area.ShowShadow && !StiBrush.IsTransparent(radarArea.Brush))
                {
                    context.PushTranslateTransform(shadowSize, shadowSize);
                    context.FillPath(shadowBrush, path, rect, null);
                    context.PopTransform();
                }

                context.FillPath(radarArea.Brush, path, rect, null);
            }
        }


        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            RectangleF rect = this.ClientRectangle;
            IStiRadarArea radarArea = this.Area as IStiRadarArea;
            StiRadarAreaCoreXF radarAreaCore = radarArea.Core as StiRadarAreaCoreXF;

            if (rect.Width > 0 && rect.Height > 0)
            {
                context.PushSmoothingModeToAntiAlias();

                DrawBackground(context);

                DrawVert(context, true, false);
                DrawHor(context, true, false);
                DrawVert(context, false, true);
                DrawHor(context, false, true);

                context.PopSmoothingMode();

                //Red line
                //StiPenGeom pen2 = new StiPenGeom(Color.Red);
                //context.DrawRectangle(pen2, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }
        #endregion

        public StiRadarAreaGeom(IStiArea area, RectangleF clientRectangle, int valuesCount)
            : base(area, clientRectangle)
        {
            this.valuesCount = valuesCount;
        }
    }
}
