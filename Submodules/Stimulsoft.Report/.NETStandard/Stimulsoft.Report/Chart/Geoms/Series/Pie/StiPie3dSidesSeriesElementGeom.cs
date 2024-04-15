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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Chart.Geoms.Series.Pie
{
    public class StiPie3dSidesSeriesElementGeom : StiSeriesElementGeom
    {
        #region Feilds
        private static float s_shadowAngle = 20F;
        private StiPenGeom pen;
        private float startnAngle;
        private float sweepAngle;
        private RectangleF m_boundingRectangle;
        private float m_sliceHeight;
        private float m_startAngle;
        private float m_sweepAngle;
        private PointF m_center;
        private PointF m_centerBelow;
        private PointF m_pointStart;
        private PointF m_pointStartBelow;
        private PointF m_pointEnd;
        private PointF m_pointEndBelow;
        private StiBrush m_brushStartSide;
        private StiBrush m_brushEndSide;
        private StiBrush m_brushPeripherySurface;
        private StiBrush m_brushSurface;
        private StiPie3dQuadrilateral m_startSide;
        private StiPie3dQuadrilateral m_endSide;
        private bool startSideExists;
        private bool endSideExists;
        private int count;
        #endregion

        #region Properties
        public float StartAngle
        {
            get { return m_startAngle; }
        }

        public float EndAngle
        {
            get { return (m_startAngle + m_sweepAngle) % 360; }
        }

        public override StiSeriesInteractionData Interaction
        {
            get
            {
                return this.Pie3DSlice.Interaction;
            }
        }

        public StiPie3dSlice Pie3DSlice { get; private set; }
        #endregion

        #region Methods
        public override bool Contains(float px, float py)
        {
            var point = new PointF(px, py);

            if (PeripheryContainsPoint(point))
                return true;

            if (m_startSide.Contains(point))
                return true;

            if (m_endSide.Contains(point))
                return true;

            return false;
        }

        internal bool PeripheryContainsPoint(PointF point)
        {
            var peripherySurfaceBounds = GetVisiblePeripherySurfaceBounds();
            foreach (var surfaceBounds in peripherySurfaceBounds)
            {
                if (CylinderSurfaceSectionContainsPoint(point, surfaceBounds.StartAngle, surfaceBounds.EndAngle, surfaceBounds.StartPoint, surfaceBounds.EndPoint))
                    return true;
            }
            return false;
        }

        private bool CylinderSurfaceSectionContainsPoint(PointF point, float startAngle, float endAngle, PointF point1, PointF point2)
        {
            if (m_sliceHeight > 0)
            {
                return StiPie3dQuadrilateral.Contains(point, new PointF[] { point1, new PointF(point1.X, point1.Y + m_sliceHeight), new PointF(point2.X, point2.Y + m_sliceHeight), point2 });
            }
            return false;
        }

        public override void Draw(StiContext context)
        {
            context.PushSmoothingModeToAntiAlias();

            DrawHiddenPeriphery(context);
            
            // draw wegde sides
            if (StartAngle > 90 && StartAngle < 270)
            {
                DrawEndSide(context);
                DrawStartSide(context);
            }
            else
            {
                DrawStartSide(context);
                DrawEndSide(context);
            }

            DrawVisiblePeriphery(context);

            context.PopSmoothingMode();
        }

        private void DrawStartSide(StiContext context)
        {
            if (m_startSide != null)
            {
                // checks if the side is visible 
                if (this.StartAngle > 90 && this.StartAngle < 270)
                    m_startSide.Draw(context, pen, m_brushStartSide, this.IsMouseOver);

                else
                    m_startSide.Draw(context, pen, m_brushSurface, this.IsMouseOver);
            }
        }

        private void DrawEndSide(StiContext context)
        {
            if (m_endSide != null)
            {
                // checks if the side is visible 
                if (EndAngle > 90 && EndAngle < 270)
                    m_endSide.Draw(context, pen, m_brushSurface, this.IsMouseOver);

                else
                    m_endSide.Draw(context, pen, m_brushEndSide, this.IsMouseOver);
            }
        }

        private void DrawVisiblePeriphery(StiContext context)
        {
            var peripherySurfaceBounds = GetVisiblePeripherySurfaceBounds();

            foreach (StiPie3dPeripherySurfaceBounds surfaceBounds in peripherySurfaceBounds)
            {
                DrawCylinderSurfaceSection(context, pen, m_brushPeripherySurface, surfaceBounds);
            }
        }

        private void DrawHiddenPeriphery(StiContext context)
        {
            var peripherySurfaceBounds = GetHiddenPeripherySurfaceBounds();

            foreach (StiPie3dPeripherySurfaceBounds surfaceBounds in peripherySurfaceBounds)
            {
                DrawCylinderSurfaceSection(context, pen, m_brushSurface, surfaceBounds);
            }
        }

        private List<StiPie3dPeripherySurfaceBounds> GetHiddenPeripherySurfaceBounds()
        {
            var peripherySurfaceBounds = new List<StiPie3dPeripherySurfaceBounds>();

            // outer periphery side is not visible when startAngle or endAngle 
            // is between 180 and 360 degrees
            if (!(m_sweepAngle == 0 || (m_startAngle >= 0 && m_startAngle + m_sweepAngle <= 180)))
            {
                // draws the periphery from start angle to the end angle or right
                // edge, whichever comes first
                if (m_startAngle + m_sweepAngle > 180)
                {
                    var fStartAngle = m_startAngle;
                    var fStartPoint = new PointF(m_pointStart.X, m_pointStart.Y);

                    var fEndAngle = m_startAngle + m_sweepAngle;
                    var fEndPoint = new PointF(m_pointEnd.X, m_pointEnd.Y);

                    var rStartAngle = this.startnAngle;
                    var rEndAngle = this.startnAngle + this.sweepAngle;

                    if (fStartAngle < 180)
                    {
                        fStartAngle = rStartAngle = 180;
                        fStartPoint.X = m_boundingRectangle.Left;
                        fStartPoint.Y = m_center.Y;
                    }

                    if (fEndAngle > 360)
                    {
                        fEndAngle = rEndAngle = 360;
                        fEndPoint.X = m_boundingRectangle.Right;
                        fEndPoint.Y = m_center.Y;
                    }

                    peripherySurfaceBounds.Add(new StiPie3dPeripherySurfaceBounds(fStartAngle, fEndAngle, fStartPoint, fEndPoint, rStartAngle, rEndAngle));
                    // if pie is crossing 360 & 180 deg. boundary, we have to 
                    // invisible peripheries
                    if (m_startAngle < 360 && m_startAngle + m_sweepAngle > 540)
                    {
                        fStartAngle = 180;
                        fStartPoint = new PointF(m_boundingRectangle.Left, m_center.Y);
                        fEndAngle = EndAngle;
                        fEndPoint = new PointF(m_pointEnd.X, m_pointEnd.Y);
                        peripherySurfaceBounds.Add(new StiPie3dPeripherySurfaceBounds(fStartAngle, fEndAngle, fStartPoint, fEndPoint, rStartAngle, rEndAngle));
                    }
                }
            }
            return peripherySurfaceBounds;
        }

        private List<StiPie3dPeripherySurfaceBounds> GetVisiblePeripherySurfaceBounds()
        {
            var peripherySurfaceBounds = new List<StiPie3dPeripherySurfaceBounds>();

            // outer periphery side is visible only when startAngle or endAngle 
            // is between 0 and 180 degrees
            if (!(m_sweepAngle == 0 || (m_startAngle >= 180 && m_startAngle + m_sweepAngle <= 360)))
            {
                // draws the periphery from start angle to the end angle or left
                // edge, whichever comes first
                if (StartAngle < 180)
                {
                    var fStartAngle = m_startAngle;
                    var fStartPoint = new PointF(m_pointStart.X, m_pointStart.Y);

                    var fEndAngle = EndAngle;
                    var fEndPoint = new PointF(m_pointEnd.X, m_pointEnd.Y);

                    var rStartAngle = this.startnAngle;
                    var rEndAngle = this.startnAngle + this.sweepAngle;

                    if (m_startAngle + m_sweepAngle > 180)
                    {
                        fEndAngle = rEndAngle = 180;
                        fEndPoint.X = m_boundingRectangle.X;
                        fEndPoint.Y = m_center.Y;
                    }
                    peripherySurfaceBounds.Add(new StiPie3dPeripherySurfaceBounds(fStartAngle, fEndAngle, fStartPoint, fEndPoint, rStartAngle, rEndAngle));
                }

                // if lateral surface is visible from the right edge 
                if (m_startAngle + m_sweepAngle > 360)
                {
                    var fStartAngle = 0;
                    var fStartPoint = new PointF(m_boundingRectangle.Right, m_center.Y);
                    var fEndAngle = EndAngle;
                    var fEndPoint = new PointF(m_pointEnd.X, m_pointEnd.Y);

                    float rStartAngle = this.startnAngle;
                    float rEndAngle = this.startnAngle + this.sweepAngle;

                    if (fEndAngle > 180)
                    {
                        fEndAngle = rEndAngle = 180;
                        fEndPoint.X = m_boundingRectangle.Left;
                        fEndPoint.Y = m_center.Y;
                    }
                    peripherySurfaceBounds.Add(new StiPie3dPeripherySurfaceBounds(fStartAngle, fEndAngle, fStartPoint, fEndPoint, rStartAngle, rEndAngle));
                }
            }
            return peripherySurfaceBounds;
        }

        protected void DrawCylinderSurfaceSection(StiContext context, StiPenGeom pen, StiBrush brush, StiPie3dPeripherySurfaceBounds peripherySurfaceBounds)
        {
            brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(brush)
                    : brush;

            var path = CreatePathForCylinderSurfaceSection(peripherySurfaceBounds);

            var rect = context.GetPathBounds(path);
            var animation = GetAnimation();
            if (animation != null)
            {
                context.DrawAnimationPathElement(brush, brush, pen, path, rect, this.GetToolTip(), this, animation, GetInteractionData());
            }
            else
            {
                context.FillPath(brush, path, rect);
                context.DrawPath(pen, path, rect);
            }

            if (this.IsMouseOver)
                context.FillPath(StiMouseOverHelper.GetMouseOverColor(), path, this.ClientRectangle);
        }

        public PointF GetTextPosition()
        {
            if (sweepAngle >= 180)
                return PeripheralPoint(m_center.X, m_center.Y, m_boundingRectangle.Width / 3, m_boundingRectangle.Height / 3, GetActualAngle(StartAngle) + sweepAngle / 2);

            float x = (m_pointStart.X + m_pointEnd.X) / 2;
            float y = (m_pointStart.Y + m_pointEnd.Y) / 2;
            float angle = (float)(Math.Atan2(y - m_center.Y, x - m_center.X) * 180 / Math.PI);

            return PeripheralPoint(m_center.X, m_center.Y, m_boundingRectangle.Width / 3, m_boundingRectangle.Height / 3, GetActualAngle(angle));
        }

        protected float GetActualAngle(float transformedAngle)
        {
            double x = m_boundingRectangle.Height * Math.Cos(transformedAngle * Math.PI / 180);
            double y = m_boundingRectangle.Width * Math.Sin(transformedAngle * Math.PI / 180);

            float result = (float)(Math.Atan2(y, x) * 180 / Math.PI);

            if (result < 0)
                return result + 360;

            return result;
        }

        private List<StiSegmentGeom> CreatePathForCylinderSurfaceSection(StiPie3dPeripherySurfaceBounds peripherySurfaceBounds)
        {
            var startAngle = peripherySurfaceBounds.StartAngle;
            var endAngle = peripherySurfaceBounds.EndAngle;
            var pointStart = peripherySurfaceBounds.StartPoint;
            var pointEnd = peripherySurfaceBounds.EndPoint;
            var realStartAngle = peripherySurfaceBounds.RealStartAngle;
            var realEndAngle = peripherySurfaceBounds.RealEndAngle;
            var realSweetAngle = peripherySurfaceBounds.RealEndAngle - peripherySurfaceBounds.RealStartAngle;

            var path = new List<StiSegmentGeom>();
            path.Add(new StiArcSegmentGeom(m_boundingRectangle, startAngle, endAngle - startAngle, realStartAngle, realSweetAngle));
            path.Add(new StiLineSegmentGeom(pointEnd.X, pointEnd.Y, pointEnd.X, pointEnd.Y + m_sliceHeight));            
            var rect = new RectangleF(m_boundingRectangle.X, m_boundingRectangle.Y + m_sliceHeight, m_boundingRectangle.Width, m_boundingRectangle.Height);
            path.Add(new StiArcSegmentGeom(rect, endAngle, startAngle - endAngle, realEndAngle, realStartAngle - realEndAngle));
            path.Add(new StiLineSegmentGeom(pointStart.X, pointStart.Y + m_sliceHeight, pointStart.X, pointStart.Y));

            return path;
        }

        private void InitializePieSlice(float xBoundingRect, float yBoundingRect, float widthBoundingRect, float heightBoundingRect, float sliceHeight, StiPie3dLightingStyle shadowStyle)
        {
            // stores bounding rectangle and pie slice height
            m_boundingRectangle = new RectangleF(xBoundingRect, yBoundingRect, widthBoundingRect, heightBoundingRect);
            m_sliceHeight = sliceHeight;
            var m_actualStartAngle = startnAngle;
            // recalculates start and sweep angle used for rendering
            m_startAngle = TransformAngle(startnAngle);
            m_sweepAngle = sweepAngle;
            if (m_sweepAngle % 180 != 0F)
                m_sweepAngle = TransformAngle(m_actualStartAngle + sweepAngle) - m_startAngle;
            if (m_sweepAngle < 0)
                m_sweepAngle += 360;
            // creates brushes
            CreateSurfaceBrushes(shadowStyle);
            // calculates center and end points on periphery
            float xCenter = xBoundingRect + widthBoundingRect / 2;
            float yCenter = yBoundingRect + heightBoundingRect / 2;
            m_center = new PointF(xCenter, yCenter);
            m_centerBelow = new PointF(xCenter, yCenter + sliceHeight);
            m_pointStart = PeripheralPoint(xCenter, yCenter, widthBoundingRect / 2, heightBoundingRect / 2, m_actualStartAngle);
            m_pointStartBelow = new PointF(m_pointStart.X, m_pointStart.Y + sliceHeight);
            m_pointEnd = PeripheralPoint(xCenter, yCenter, widthBoundingRect / 2, heightBoundingRect / 2, m_actualStartAngle + sweepAngle);
            m_pointEndBelow = new PointF(m_pointEnd.X, m_pointEnd.Y + sliceHeight);

            InitializeSides(this.startSideExists, this.endSideExists);
        }

        private void InitializeSides(bool startSideExists, bool endSideExists)
        {
            var animation = GetAnimation();

            if (startSideExists)
                m_startSide = new StiPie3dQuadrilateral(m_boundingRectangle, m_center, m_pointStart, m_pointStartBelow, m_centerBelow, m_sweepAngle != 180, animation);

            else
                m_startSide = StiPie3dQuadrilateral.Empty;

            if (endSideExists)
                m_endSide = new StiPie3dQuadrilateral(m_boundingRectangle, m_center, m_pointEnd, m_pointEndBelow, m_centerBelow, m_sweepAngle != 180, animation);

            else
                m_endSide = StiPie3dQuadrilateral.Empty;
        }

        protected virtual void CreateSurfaceBrushes(StiPie3dLightingStyle shadowStyle)
        {
            var surfaceColor = StiBrush.ToColor(this.SeriesBrush);

            if (this.IsSelected)
            {
                var brush = new StiSolidBrush(surfaceColor);

                m_brushSurface = brush;
                m_brushStartSide = m_brushEndSide = m_brushPeripherySurface = StiBrush.Light(brush, 30); ;
                return;
            }

            m_brushSurface = new StiSolidBrush(surfaceColor);

            switch (shadowStyle)
            {
                case StiPie3dLightingStyle.No:
                    m_brushStartSide = m_brushEndSide = m_brushPeripherySurface = new StiSolidBrush(surfaceColor);
                    break;
                
                case StiPie3dLightingStyle.Solid:
                    m_brushStartSide = m_brushEndSide = m_brushPeripherySurface = StiBrush.Light(SeriesBrush, 30);
                    break;

                case StiPie3dLightingStyle.Gradient:
                    double angle = m_startAngle - 180 - s_shadowAngle;
                    if (angle < 0)
                        angle += 360;
                    m_brushStartSide = CreateBrushForSide(surfaceColor, angle);
                    angle = m_startAngle + m_sweepAngle - s_shadowAngle;
                    if (angle < 0)
                        angle += 360;
                    m_brushEndSide = CreateBrushForSide(surfaceColor, angle);
                    m_brushPeripherySurface = CreateBrushForPeriphery(surfaceColor);
                    break;
            }
        }

        protected virtual StiBrush CreateBrushForSide(Color color, double angle)
        {
            return new StiSolidBrush(StiPie3dHelper.CreateColorWithCorrectedLightness(color, -(float)(StiPie3dHelper.BrightnessEnhancementFactor1 * (1 - 0.8 * Math.Cos(angle * Math.PI / 180)))));
        }

        protected virtual StiBrush CreateBrushForPeriphery(Color color)
        {
            return new StiGlareBrush(color, CreateColorWithCorrectedLightness(color, -StiPie3dHelper.BrightnessEnhancementFactor1), 90, 0.1f, 1); ;
        }

        public static Color CreateColorWithCorrectedLightness(Color color, float correctionFactor)
        {
            if (correctionFactor == 0)
                return color;
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;
            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }
            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        protected PointF PeripheralPoint(float xCenter, float yCenter, float semiMajor, float semiMinor, float angleDegrees)
        {
            double angleRadians = angleDegrees * Math.PI / 180;
            return new PointF(xCenter + (float)(semiMajor * Math.Cos(angleRadians)), yCenter + (float)(semiMinor * Math.Sin(angleRadians)));
        }

        protected float TransformAngle(float angle)
        {
            double x = m_boundingRectangle.Width * Math.Cos(angle * Math.PI / 180);
            double y = m_boundingRectangle.Height * Math.Sin(angle * Math.PI / 180);
            float result = (float)(Math.Atan2(y, x) * 180 / Math.PI);
            if (result < 0)
                return result + 360;
            return result;
        }

        private StiAnimation GetAnimation()
        {
            if (!((StiChart)Series.Chart).IsAnimation) return null;

            var duration = StiChartHelper.GlobalDurationElement;
            var beginTime = StiChartHelper.GlobalBeginTimeElement;

            StiAnimation animation = null;
            StiPieSegmentAnimation pieAnimation = null;

            if (animation == null)
            {
                var beginTimeAnimationOpacity = new TimeSpan(beginTime.Ticks / count * Index);
                if (this.Series.Chart.PreviousAnimations.Count > 0) beginTimeAnimationOpacity = TimeSpan.FromMilliseconds(300);
                var opacityAnimation = new StiOpacityAnimation(duration, beginTimeAnimationOpacity);

                opacityAnimation.AnotherAnimation = pieAnimation;
                opacityAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_{Index}";

                animation = opacityAnimation;
            }

            return animation;
        }
        #endregion

        public StiPie3dSidesSeriesElementGeom(StiPie3dSlice pie3DSlice, StiAreaGeom areaGeom, double value, int index, int count, IStiPie3dSeries series, RectangleF clientRectangle,
            Color borderColor, StiBrush brush, float startAngle, float sweepAngle, float pieHeight, bool startSideExists, bool endSideExists)
            : base(areaGeom, value, index, series, clientRectangle, brush)
        {
            this.Pie3DSlice = pie3DSlice;
            this.count = count;
            this.startnAngle = startAngle;
            this.pen = new StiPenGeom(borderColor);
            this.sweepAngle = sweepAngle;
            this.startSideExists = startSideExists;
            this.endSideExists = endSideExists;

            InitializePieSlice(clientRectangle.X, clientRectangle.Y, clientRectangle.Width, clientRectangle.Height, pieHeight, series.Options3D.Lighting);
        }
    }
}
