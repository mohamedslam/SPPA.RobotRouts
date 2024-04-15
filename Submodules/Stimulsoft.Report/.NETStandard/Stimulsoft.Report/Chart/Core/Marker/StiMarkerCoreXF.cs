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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Report.Helpers;

namespace Stimulsoft.Report.Chart
{
    public class StiMarkerCoreXF : ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region Methods
        public void DrawMarkers(StiContext context, PointF[] points, bool showShadow)
        {
            if (points.Length == 0) return;

            if (marker != null && marker.Visible && marker.Size > 0)
            {
                context.PushSmoothingModeToAntiAlias();

                float chartZoom = context.Options.Zoom;
                
                foreach (var point in points)
                {
                    marker.Core.Draw(context, marker, point, chartZoom, showShadow, false, false, false, string.Empty, null, null);
                }

                context.PopSmoothingMode();
            }
        }

        public static RectangleF GetMarkerRect(PointF position, float markerSize, float zoom)
        {
            float dx = (float)markerSize * zoom / 2;
            return new RectangleF(
                position.X - dx,
                position.Y - dx,
                dx * 2, dx * 2);
        }
        
        public void Draw(StiContext context, IStiMarker marker, PointF position, float zoom, bool showShadow, bool isMouseOver, bool isTooltipMode, bool isAnimation, string toolTip, object tag, StiInteractionDataGeom interaction)
        {
            var shadowBrush = new StiSolidBrush(Color.FromArgb(55, Color.Black));

            if (marker.Size < 0) return;

            var rect = GetMarkerRect(position, marker.Size, zoom);

            #region Draw Shadow
            if (showShadow && !isTooltipMode)
            {
                rect.X += 2 * zoom;
                rect.Y += 2 * zoom;

                DrawPoint(context, rect.X + rect.Width / 2, rect.Y + rect.Height / 2, zoom, shadowBrush, null, marker.Type, marker.Icon, marker.Size, marker.Angle, false, isAnimation, toolTip, null, null);

                rect.X -= 2 * zoom;
                rect.Y -= 2 * zoom;
            }
            #endregion

            var brush = marker.Brush;
            if (isAnimation && isTooltipMode && !marker.Visible)
            {
                var color = marker.Brush == null ? Color.LightGray : StiBrush.ToColor(marker.Brush);
                brush = new StiSolidBrush(Color.FromArgb(0, color.R, color.G, color.B));
                isAnimation = false;
            }
            var pen = new StiPenGeom(marker.BorderColor);

            if (marker.Visible || isTooltipMode)
                DrawPoint(context, rect.X + rect.Width / 2, rect.Y + rect.Height / 2, zoom, brush, pen, marker.Type, marker.Icon, marker.Size, marker.Angle, isMouseOver, isAnimation, toolTip, tag, interaction);
        }

        public void DrawLine(StiContext context, float x1, float y1, float x2, float y2, float scale,
            StiBrush brushMarker, StiPenGeom penMarker, StiMarkerType markerType, float markerStep, float markerSize, float angle)
        {            
            DrawLines(context, new PointF?[] { new PointF(x1, y1), new PointF(x2, y2) }, scale, brushMarker, penMarker, markerType, markerStep, markerSize, angle);
        }


        public void DrawLines(StiContext context, PointF?[] points, float scale,
            object brushMarker, StiPenGeom penMarker, StiMarkerType markerType, float markerStep, float markerSize, float angle)
        {
            context.PushSmoothingModeToAntiAlias();

            if (points.Length < 2) return;
            float pos = 0;
            for (int index = 0; index < points.Length - 1; index++)
            {
                PointF? point = points[index];
                PointF? nextPoint = points[index + 1];

                if (point == null || nextPoint == null) continue;

                float x1 = point.Value.X;
                float y1 = point.Value.Y;
                float x2 = nextPoint.Value.X;
                float y2 = nextPoint.Value.Y;

                float dx = x2 - x1;
                float dy = y2 - y1;
                float length = (float)Math.Sqrt(dx * dx + dy * dy);

                float step = markerStep;

                while (pos < length)
                {
                    float x = x1 + dx * pos / length;
                    float y = y1 + dy * pos / length;

                    DrawPoint(context, x, y, scale, brushMarker, penMarker, markerType, marker.Icon, markerSize, angle, false, false, string.Empty, null, null);

                    pos += step * scale;
                }
                pos -= length;
            }
            context.PopSmoothingMode();
        }

        public void DrawPoint(StiContext context, float x, float y, float scale, object brush, StiPenGeom pen, StiMarkerType markerType, StiFontIcons? icon, float markerSize, float angle, 
            bool isMouseOver, bool isAnimation, string toolTip, object tag, StiInteractionDataGeom interaction)
        {
            float step = markerSize * scale / 2;
            float x1 = x - step;
            float x2 = x + step;
            float y1 = y - step;
            float y2 = y + step;

            if (icon != null)
            {
                var rect = new RectangleF(x1, y1, markerSize * scale, markerSize * scale);

                if (rect.Height > 0 && rect.Width > 0)
                    StiFontIconsExHelper.DrawDirectionIcons(context, brush, rect, new SizeF(rect.Height, rect.Height), icon.GetValueOrDefault(), toolTip, false);

                return;
            }

            switch (markerType)
            {
                case StiMarkerType.Star5:
                    DrawPolygon(context, brush, pen, x, y, step, 10, angle, true, isMouseOver, isAnimation, toolTip, tag, interaction);
                    break;

                case StiMarkerType.Star6:
                    DrawPolygon(context, brush, pen, x, y, step, 12, angle, true, isMouseOver, isAnimation, toolTip, tag, interaction);
                    break;

                case StiMarkerType.Star7:
                    DrawPolygon(context, brush, pen, x, y, step, 14, angle, true, isMouseOver, isAnimation, toolTip, tag, interaction);
                    break;

                case StiMarkerType.Star8:
                    DrawPolygon(context, brush, pen, x, y, step, 16, angle, true, isMouseOver, isAnimation, toolTip, tag, interaction);
                    break;

                case StiMarkerType.Hexagon:
                    DrawPolygon(context, brush, pen, x, y, step, 6, 30 + angle, false, isMouseOver, isAnimation, toolTip, tag, interaction);
                    break;

                case StiMarkerType.Rectangle:
                    DrawPolygon(context, brush, pen, x, y, step, 4, 45 + angle, false, isMouseOver, isAnimation, toolTip, tag, interaction);
                    break;

                case StiMarkerType.Circle:
                    if (isAnimation)
                    {
                        var animation = new StiScaleAnimation(StiChartHelper.GlobalDurationElement, TimeSpan.Zero);
                        context.FillDrawAnimationEllipse(brush, pen, x1, y1, x2 - x1, y2 - y1, toolTip, tag, animation, interaction);
                    }
                    else
                    {
                        if (brush != null)
                            context.FillEllipse(brush, x1, y1, x2 - x1, y2 - y1, toolTip, interaction);

                        if (isMouseOver)
                            context.FillEllipse(StiMouseOverHelper.GetMouseOverColor(), x1, y1, x2 - x1, y2 - y1, toolTip, null);

                        if (pen != null)
                            context.DrawEllipse(pen, x1, y1, x2 - x1, y2 - y1);
                    }
                    break;

                case StiMarkerType.HalfCircle:
                    var rect = new RectangleF(x1, y1, x2 - x1, y2 - y1);
                    var rectEmpty = new RectangleF(0, 0, x2 - x1, y2 - y1);
                    var path = new List<StiSegmentGeom>();
                    path.Add(new StiArcSegmentGeom(new RectangleF(rectEmpty.X, rectEmpty.Top + rectEmpty.Height / 4, rectEmpty.Width, rectEmpty.Height), 360, -180));
                    path.Add(new StiLineSegmentGeom(new PointF(rectEmpty.X, rectEmpty.Bottom - rectEmpty.Height / 4), new PointF(rectEmpty.Right, rectEmpty.Bottom - rectEmpty.Height / 4)));
                    context.PushTranslateTransform(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                    context.PushRotateTransform(angle);
                    context.PushTranslateTransform(-rect.Width / 2, -rect.Height / 2);
                    if (isAnimation)
                    {
                        var animation = new StiScaleAnimation(StiChartHelper.GlobalDurationElement, TimeSpan.Zero);
                        context.FillDrawAnimationPath(brush, pen, path, rect, tag, animation, interaction);
                    }
                    else
                    {
                        
                        if (brush != null)
                            context.FillPath(brush, path, rectEmpty);

                        if (isMouseOver)
                            context.FillPath(StiMouseOverHelper.GetMouseOverColor(), path, rect, null);

                        if (pen != null)
                            context.DrawPath(pen, path, rectEmpty);
                        
                    }
                    context.PopTransform();
                    context.PopTransform();
                    context.PopTransform();
                    break;

                case StiMarkerType.Triangle:
                    DrawPolygon(context, brush, pen, x, y, step, 3, angle, false, isMouseOver, isAnimation, toolTip, tag, interaction);
                    break;
            }
        }

        private void DrawPolygon(StiContext context, object fillBrush, StiPenGeom borderPen, float centerX, float centerY, float radius, int count, float startAngle, 
            bool isStar, bool isMouseOver, bool isAnimation, string toolTip, object tag, StiInteractionDataGeom interaction)
        {
            startAngle = -startAngle;
            var path = new List<StiSegmentGeom>();

            float angle = -90 + startAngle;

            var points = new PointF[count + 1];
            if (isStar)
            {
                for (int index = 0; index < count; index += 2)
                {
                    points[index].X = centerX + radius * (float)Math.Cos(angle * Math.PI / 180);
                    points[index].Y = centerY + radius * (float)Math.Sin(angle * Math.PI / 180);

                    angle += 360f / count;

                    points[index + 1].X = centerX + radius / 2 * (float)Math.Cos(angle * Math.PI / 180);
                    points[index + 1].Y = centerY + radius / 2 * (float)Math.Sin(angle * Math.PI / 180);

                    angle += 360f / count;
                }
            }
            else
            {
                for (int index = 0; index < count; index++)
                {
                    points[index].X = centerX + radius * (float)Math.Cos(angle * Math.PI / 180);
                    points[index].Y = centerY + radius * (float)Math.Sin(angle * Math.PI / 180);

                    angle += 360f / count;
                }
            }

            points[count] = points[0];
            path.Add(new StiLinesSegmentGeom(points));
            if (isAnimation)
            {
                var animation = new StiScaleAnimation(StiChartHelper.GlobalDurationElement, TimeSpan.Zero);
                context.FillDrawAnimationPath(fillBrush, borderPen, path, StiPathGeom.GetBoundsState, tag, animation, interaction);
            }
            else
            {
                if (fillBrush != null)
                    context.FillPath(fillBrush, path, StiPathGeom.GetBoundsState, null, -1, toolTip);

                if (isMouseOver)
                    context.FillPath(StiMouseOverHelper.GetMouseOverColor(), path, StiPathGeom.GetBoundsState, null, -1, toolTip);

                if (borderPen != null)
                    context.DrawPath(borderPen, path, null);
            }

        }
        #endregion

        #region Properties
        private IStiMarker marker;
        public IStiMarker Marker
        {
            get
            {
                return marker;
            }
            set
            {
                marker = value;
            }
        }
        #endregion

        public StiMarkerCoreXF(IStiMarker marker)
        {
            this.marker = marker;
        }
    }
}