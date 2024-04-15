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
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using Stimulsoft.Report.Chart;
using System.Runtime.CompilerServices;

#if STIDRAWING
using Matrix = Stimulsoft.Drawing.Drawing2D.Matrix;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using LinearGradientBrush = Stimulsoft.Drawing.Drawing2D.LinearGradientBrush;
#endif

namespace Stimulsoft.Report.Painters.Context.Animation
{
    public class StiAnimationEngine : IDisposable
    {
        #region class ContextStore
        private class ContextStore
        {
            public bool AnimationExist { get; set; } = false;
        }
        #endregion

        #region class ContextStore<T>
        private class ContextStore<T> : ContextStore
        {
            public IStiAnimationContextPainter<T> contextPainter;
            public List<T> geoms;
        }
        #endregion

        #region Fields
        private Hashtable contexts = new Hashtable();
        private int animationStart;
        private double animationTime;
        private bool isStart;
        private bool isFirstStart = true;
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Stop();
        }
        #endregion

        #region Event
        public delegate void AnimationHandler(StiComponent component);
        public event AnimationHandler Animation;

        public delegate void ReverseEndHandler();
        public event ReverseEndHandler ReverseEnd;
        #endregion

        #region Properties
        public bool IsReverse { get; set; }

        public object Tag { get; set; }

        public double Speed { get; set; } = 1;

        private bool AnimationExist
        {
            get
            {
                return contexts.Values.Cast<ContextStore>().Any(contextStore => contextStore.AnimationExist);
            }
        }
        #endregion

        #region Methods
        public void StartReverse()
        {
            StartReverse(Speed);
        }

        public void StartReverse(double speed)
        {
            Start(speed);
            if (!StiOptions.Viewer.Windows.AllowAnimation) return;
            IsReverse = true;
        }

        public void Start()
        {
            Start(Speed);
        }

        public void Start(bool isFirstStart)
        {
            this.isFirstStart = isFirstStart;
            Start(Speed);
        }

        public void Start(double speed)
        {
            if (!StiOptions.Viewer.Windows.AllowAnimation) return;

            if (isFirstStart)
            {
                isFirstStart = false;
                if (!StiOptions.Viewer.Windows.AnimationPlaybackType.HasFlag(StiAnimationPlaybackType.OnStart))
                    return;
            }
            else if (!StiOptions.Viewer.Windows.AnimationPlaybackType.HasFlag(StiAnimationPlaybackType.OnPreview))
            {
                isStart = true;
                return;
            }

            this.Speed = speed;

            IsReverse = false;

            isStart = true;
            isFirstStart = false;

            contexts.Clear();

            animationStart = 0;
            animationTime = 0;
        }

        public void Continue()
        {
            if (isStart && AnimationExist)
            {
                if (animationStart == 0) animationStart = Environment.TickCount;

                Task.Delay(15).ContinueWith(o => { OnTimer(); });
            }
            else if (isStart && IsReverse)
            {
                animationStart = 0;
                IsReverse = false;
                contexts.Keys.Cast<StiComponent>().ToList().ForEach(component => ReverseEnd?.Invoke());
            }
            else
            {
                contexts.Clear();
                animationStart = 0;
                isStart = false;
                IsReverse = false;
            }
        }

        public void Stop()
        {
            isStart = false;
            Continue();
        }

        public void Finish()
        {
            animationStart = -int.MaxValue;
            Continue();
        }

        public void RegisterContextPainter<T>(StiComponent component, IStiAnimationContextPainter<T> contextPainter)
        {
            if (!isStart) return;

            if (!contexts.ContainsKey(component))
            {
                contexts.Add(component, new ContextStore<T>() { geoms = contextPainter.Geoms });
            }

            else
            {
                var contextStore = (ContextStore<T>)contexts[component];

                if (IsRunning(component))
                    contextPainter.Geoms.AddRange(contextStore.geoms);
                else
                    contextStore.geoms = contextPainter.Geoms;

                contextStore.AnimationExist = false;
            }

            (contexts[component] as ContextStore<T>).contextPainter = contextPainter;
        }

        public bool IsRunning(StiComponent component = null)
        {
            if (component == null || contexts[component] == null) return AnimationExist;
            return ((ContextStore)contexts[component]).AnimationExist;
        }
        /// <summary>
        /// </summary>
        /// <returns>0 - Animation is not started; 1 - Animation ended</returns>
        private float GetMultiple<T>(IStiAnimationContextPainter<T> contextPainter, StiAnimation animation)
        {
            if (!isStart) return 1;
            var contextStore = contexts.Values.OfType<ContextStore<T>>().First(cs => cs.contextPainter == contextPainter);

            return GetMultiple(contextStore, animation);
        }

        private float GetMultiple<T>(ContextStore<T> contextStore, StiAnimation animation)
        {
            var beginTimeMilliseconds = animation.BeginTime.TotalMilliseconds;
            if (IsReverse)
                beginTimeMilliseconds = animation.BeginTimeCorrect.TotalMilliseconds - beginTimeMilliseconds;
            else
                beginTimeMilliseconds += animation.BeginTimeCorrect.TotalMilliseconds;

            var durationMilliseconds = animation.Duration.TotalMilliseconds;

            durationMilliseconds *= Speed;
            beginTimeMilliseconds *= Speed;

            var multiple = 1d;

            if (durationMilliseconds == 0)
                return beginTimeMilliseconds >= animationTime ? 0 : 1;
            else
                multiple = (animationTime - beginTimeMilliseconds) / durationMilliseconds;

            if (multiple < 0) multiple = 0;
            if (multiple > 1) multiple = 1;

            if (multiple < 1)
                contextStore.AnimationExist = true;

            if (IsReverse) return (float)(1 - multiple);
            return (float)multiple;
        }
        #endregion

        #region Handlers
        private void OnTimer()
        {
            animationTime = (double)Environment.TickCount - animationStart;
            if (AnimationExist)
            {
                contexts.Keys.Cast<StiComponent>().ToList().ForEach(component => Animation?.Invoke(component));
            }
            else
            {
                Stop();
            }
        }
        #endregion

        #region Methods.Draw
        private static PointF GetPoint(PointF centerPie, float radius, float angle)
        {
            float angleRad = (float)(Math.PI * angle / 180);
            return new PointF(
                centerPie.X + (float)Math.Cos(angleRad) * radius,
                centerPie.Y + (float)Math.Sin(angleRad) * radius);
        }

        public static void CorrectAnimationTimes<T>(IStiAnimationContextPainter<T> contextPainter, List<StiGeom> geoms)
        {
            if (contextPainter.AnimationEngine == null) return;

            var animationGeoms = geoms
                .Where(geom => geom is StiAnimationGeom)
                .Cast<StiAnimationGeom>()
                .Where(geom => geom.Animation != null);

            var allTime = animationGeoms
                .Where(geom => geom.Type != StiGeomType.AnimationLabel)
                .Where(geom => geom.Type != StiGeomType.AnimationEllipse)
                .Select(geom => geom.Animation)
                .Aggregate(TimeSpan.Zero, (acc, animation) => animation.Duration + animation.BeginTime);

            animationGeoms
                .Where(geom => geom.Type == StiGeomType.AnimationLabel || geom.Type == StiGeomType.AnimationEllipse)
                .Select(geom => geom.Animation)
                .ToList()
                .ForEach(animation =>
                {
                    animation.BeginTime = allTime;
                    animation.Duration = TimeSpan.FromMilliseconds(300);
                });


            if (contextPainter.AnimationEngine.IsReverse)
            {
                allTime = animationGeoms
                   .Select(geom => geom.Animation)
                   .Aggregate(TimeSpan.Zero, (acc, animation) => animation.Duration + animation.BeginTime);

                animationGeoms
                    .Select(geom => geom.Animation)
                    .ToList()
                    .ForEach(animation => animation.BeginTimeCorrect = allTime - animation.Duration);
            }
        }

        public static PointF GetAnimationPoint<T>(IStiAnimationContextPainter<T> contextPainter, PointF from, PointF to, StiAnimation animation)
        {
            if (contextPainter.AnimationEngine == null) return to;

            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return to;
            if (multiple == 0) return from;

            return GetAnimationPoint(from, to, multiple);
        }

        public static PointF GetAnimationPoint(PointF from, PointF to, float multiple)
        {
            var offsetX = (to.X - from.X) * multiple;
            var offsetY = (to.Y - from.Y) * multiple;

            return new PointF(offsetX + from.X, offsetY + from.Y);
        }

        public static PointF[] GetAnimationPoints<T>(IStiAnimationContextPainter<T> contextPainter, PointF[] to, StiPointsAnimation animation)
        {
            if (contextPainter.AnimationEngine == null) return to;

            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return to;
            var pointsFrom = animation.PointsFrom;
            if (multiple == 0) return pointsFrom;

            return to.Select((point, index) => GetAnimationPoint(pointsFrom[index], point, multiple)).ToArray();
        }

        public static RectangleF GetAnimationRectangle<T>(IStiAnimationContextPainter<T> contextPainter, RectangleF to, RectangleF from, StiColumnAnimation animation)
        {
            if (contextPainter.AnimationEngine == null) return to;

            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return to;
            if (multiple == 0) return from;

            var offsetLeft = (to.Left - from.Left) * multiple;
            var offsetTop = (to.Top - from.Top) * multiple;
            var offsetRight = (to.Right - from.Right) * multiple;
            var offsetBottom = (to.Bottom - from.Bottom) * multiple;

            return RectangleF.FromLTRB(offsetLeft + from.Left, offsetTop + from.Top, offsetRight + from.Right, offsetBottom + from.Bottom);
        }

        public static StiPenGeom GetAnimationOpacity<T>(IStiAnimationContextPainter<T> contextPainter, StiPenGeom to, StiAnimation animation)
        {
            if (contextPainter.AnimationEngine == null || to == null) return to;
            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return to;

            var penBrush = GetAnimationOpacity(contextPainter, to.Brush, animation);
            return new StiPenGeom(penBrush)
            {
                Thickness = to.Thickness,
                PenStyle = to.PenStyle,
                Alignment = to.Alignment,
                StartCap = to.StartCap,
                EndCap = to.EndCap
            };
        }

        public static object GetAnimationOpacity<T>(IStiAnimationContextPainter<T> contextPainter, object to, StiAnimation animation)
        {
            if (contextPainter.AnimationEngine == null || to == null) return to;
            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return to;

            if (to is Color color)
                return Color.FromArgb((int)(color.A * multiple), color.R, color.G, color.B);

            if (to is SolidBrush solidBrush)
            {
                return new SolidBrush(Color.FromArgb((int)(solidBrush.Color.A * multiple), solidBrush.Color.R, solidBrush.Color.G, solidBrush.Color.B));
            }
            if (to is StiSolidBrush stiSolidBrush)
            {
                return new StiSolidBrush(Color.FromArgb((int)(stiSolidBrush.Color.A * multiple), stiSolidBrush.Color.R, stiSolidBrush.Color.G, stiSolidBrush.Color.B));
            }
            if (to is StiGradientBrush gradientBrush)
            {
                var startColor = Color.FromArgb((int)(gradientBrush.StartColor.A * multiple), gradientBrush.StartColor.R, gradientBrush.StartColor.G, gradientBrush.StartColor.B);
                var endColor = Color.FromArgb((int)(gradientBrush.EndColor.A * multiple), gradientBrush.EndColor.R, gradientBrush.EndColor.G, gradientBrush.EndColor.B);
                return new StiGradientBrush(startColor, endColor, gradientBrush.Angle);
            }
            if (to is StiHatchBrush hatchBrush)
            {
                var foreColor = Color.FromArgb((int)(hatchBrush.ForeColor.A * multiple), hatchBrush.ForeColor.R, hatchBrush.ForeColor.G, hatchBrush.ForeColor.B);
                var backColor = Color.FromArgb((int)(hatchBrush.BackColor.A * multiple), hatchBrush.BackColor.R, hatchBrush.BackColor.G, hatchBrush.BackColor.B);
                return new StiHatchBrush(hatchBrush.Style, foreColor, backColor);
            }
            if (to is StiGlassBrush glassBrush)
            {
                return new StiGlassBrush(Color.FromArgb((int)(glassBrush.Color.A * multiple), glassBrush.Color.R, glassBrush.Color.G, glassBrush.Color.B), glassBrush.DrawHatch, glassBrush.Blend);
            }
            if (to is StiGlareBrush glareBrush)
            {
                var startColor = Color.FromArgb((int)(glareBrush.StartColor.A * multiple), glareBrush.StartColor.R, glareBrush.StartColor.G, glareBrush.StartColor.B);
                var endColor = Color.FromArgb((int)(glareBrush.EndColor.A * multiple), glareBrush.EndColor.R, glareBrush.EndColor.G, glareBrush.EndColor.B);
                return new StiGlareBrush(startColor, endColor, glareBrush.Angle, glareBrush.Focus, glareBrush.Scale);
            }

            return to;
        }

        public static StiRectangle3D GetAnimationRectangle3D<T>(IStiAnimationContextPainter<T> contextPainter, StiRectangle3D toRect, StiScaleAnimation animation)
        {
            if (contextPainter.AnimationEngine == null) return toRect;
            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return toRect;

            return new StiRectangle3D()
            {
                X = toRect.X,
                Y = toRect.Y,
                Z = toRect.Z,
                Width = toRect.Width,
                Height = toRect.Height * multiple,
                Length = toRect.Length
            };
        }

        public static PointF[] GetAnimationScale1<T>(IStiAnimationContextPainter<T> contextPainter, PointF[] to, StiScaleAnimation animation)
        {
            if (contextPainter.AnimationEngine == null) return to;

            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return to;

            var points = to.Select(point => new PointF(point.X, point.Y)).ToArray();
            var scaleX = (animation.EndScaleX - animation.StartScaleX) * multiple;
            var scaleY = (animation.EndScaleY - animation.StartScaleY) * multiple;

            var matrix = new Matrix();
            matrix.Translate((float)-animation.CenterX, (float)-animation.CenterY);
            matrix.TransformPoints(points);
            matrix.Reset();
            matrix.Scale((float)(animation.StartScaleX - scaleX), (float)(animation.StartScaleY + scaleY));
            matrix.TransformPoints(points);

            return points;
        }

        public static RectangleF GetAnimationScale<T>(IStiAnimationContextPainter<T> contextPainter, RectangleF to, StiScaleAnimation animation)
        {
            if (contextPainter.AnimationEngine == null) return to;

            var points = GetAnimationScale(contextPainter, new PointF[] { new PointF(to.Left, to.Top), new PointF(to.Right, to.Bottom) }, animation);

            return RectangleF.FromLTRB(points[0].X, points[0].Y, points[1].X, points[1].Y);
        }

        public static List<StiSegmentGeom> GetAnimationScale<T>(IStiAnimationContextPainter<T> contextPainter, List<StiSegmentGeom> to, StiScaleAnimation animation)
        {
            if (contextPainter.AnimationEngine == null) return to;

            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return to;

            return to.Select(segmentGeom =>
            {
                switch (segmentGeom)
                {
                    case StiArcSegmentGeom geom:
                        return (StiSegmentGeom)new StiArcSegmentGeom(GetAnimationScale(contextPainter, geom.Rect, animation), geom.StartAngle, geom.SweepAngle);
                    case StiCloseFigureSegmentGeom geom:
                        return (StiSegmentGeom)new StiCloseFigureSegmentGeom();
                    case StiCurveSegmentGeom geom:
                        return (StiSegmentGeom)new StiCurveSegmentGeom(GetAnimationScale(contextPainter, geom.Points, animation), geom.Tension);
                    case StiLineSegmentGeom geom:
                        var points = GetAnimationScale(contextPainter, new PointF[] { new PointF(geom.X1, geom.Y1), new PointF(geom.X2, geom.Y2) }, animation);
                        return (StiSegmentGeom)new StiLineSegmentGeom(points[0], points[1]);
                    case StiLinesSegmentGeom geom:
                        return (StiSegmentGeom)new StiLinesSegmentGeom(GetAnimationScale(contextPainter, geom.Points, animation));
                    case StiPieSegmentGeom geom:
                        return (StiSegmentGeom)new StiPieSegmentGeom(GetAnimationScale(contextPainter, geom.Rect, animation), geom.StartAngle, geom.SweepAngle, animation);
                }

                return segmentGeom;
            }).ToList();
        }

        public static PointF[] GetAnimationScale<T>(IStiAnimationContextPainter<T> contextPainter, PointF[] to, StiScaleAnimation animation)
        {
            if (contextPainter.AnimationEngine == null || animation == null) return to;
            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return to;

            var points = to.Select(point => new PointF(point.X, point.Y)).ToArray();
            var scaleX = (animation.EndScaleX - animation.StartScaleX) * multiple;
            var scaleY = (animation.EndScaleY - animation.StartScaleY) * multiple;

            var centerX = (float)animation.CenterX;
            var centerY = (float)animation.CenterY;

            if (Double.IsNaN(centerX))
            {
                var left = points.Min(point => point.X);
                var right = points.Max(point => point.X);
                centerX = left + (right - left) / 2;
            }

            if (Double.IsNaN(centerY))
            {
                var top = points.Min(point => point.Y);
                var bottom = points.Max(point => point.Y);
                centerY = top + (bottom - top) / 2;
            }

            var matrix = new Matrix();
            matrix.Translate(-centerX, -centerY);
            matrix.TransformPoints(points);
            matrix.Reset();
            matrix.Scale((float)(animation.StartScaleX + scaleX), (float)(animation.StartScaleY + scaleY));
            matrix.TransformPoints(points);
            matrix.Reset();
            matrix.Translate(centerX, centerY);
            matrix.TransformPoints(points);

            return points;
        }

        public static PointF[] GetAnimationRotation<T>(IStiAnimationContextPainter<T> contextPainter, PointF[] to, StiRotationAnimation animation)
        {
            if (contextPainter.AnimationEngine == null || animation == null) return to;
            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return to;
            multiple = 1 - multiple;
            var points = to.Select(point => new PointF(point.X, point.Y)).ToArray();
            var angle = (animation.EndAngle - animation.StartAngle) * multiple;
            if (angle == 0) return to;
            var matrix = new Matrix();
            matrix.Translate((float)-animation.CenterPoint.X, (float)-animation.CenterPoint.Y);
            matrix.TransformPoints(points);
            matrix.Reset();
            matrix.Rotate((float)-angle);
            matrix.TransformPoints(points);
            matrix.Reset();
            matrix.Translate((float)animation.CenterPoint.X, (float)animation.CenterPoint.Y);
            matrix.TransformPoints(points);

            return points;
        }

        public static StiPenGeom GetAnimationTranslation<T>(IStiAnimationContextPainter<T> contextPainter, PointF[] points, StiPenGeom pen, StiTranslationAnimation animation)
        {
            if (contextPainter.AnimationEngine == null) return pen;
            if (points.First().Equals(points.Last())) return GetAnimationOpacity(contextPainter, pen, animation);

            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return pen;
            var brush = pen.Brush;

            if (multiple > 0)
                brush = new LinearGradientBrush(points.First(), points.Last(), (Color)brush, Color.Transparent)
                {
                    Blend = new Blend()
                    {
                        Factors = new float[] { 0f, 0f, 1f, 1f },
                        Positions = new float[] { 0f, multiple, multiple, 1f }
                    }
                };

            return new StiPenGeom(brush)
            {
                Alignment = pen.Alignment,
                EndCap = pen.EndCap,
                PenStyle = multiple == 0 ? StiPenStyle.None : pen.PenStyle,
                StartCap = pen.StartCap,
                Thickness = pen.Thickness
            };
        }

        public static PointF[] GetAnimationTranslation<T>(IStiAnimationContextPainter<T> contextPainter, PointF[] to, StiTranslationAnimation animation)
        {
            if (contextPainter.AnimationEngine == null || animation == null) return to;
            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return to;
            multiple = 1 - multiple;

            var points = to.Select(point => new PointF(point.X, point.Y)).ToArray();
            var offsetX = (animation.StartPoint.X - animation.EndPoint.X) * multiple;
            var offsetY = (animation.StartPoint.Y - animation.EndPoint.Y) * multiple;

            var matrix = new Matrix();
            matrix.Translate(offsetX, offsetY);
            matrix.TransformPoints(points);

            return points;
        }

        public static List<StiSegmentGeom> GetAnimationAngle<T>(IStiAnimationContextPainter<T> contextPainter, List<StiSegmentGeom> geoms, StiPieSegmentAnimation animation)
        {
            if (contextPainter.AnimationEngine == null || animation == null) return geoms;
            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return geoms;
            var geom = geoms[0] as StiArcSegmentGeom;
            var geomDt = geoms[2] as StiArcSegmentGeom;

            var startAngleDelta = geom.StartAngle - animation.StartAngleFrom;
            var endAngleDelta = (geom.StartAngle + geom.SweepAngle) - animation.EndAngleFrom;
            var startAngle = animation.StartAngleFrom + (startAngleDelta * multiple);
            var sweepAngle = (animation.EndAngleFrom + (endAngleDelta * multiple)) - startAngle;

            var offsetLeft = (geom.Rect.Left - animation.RectFrom.Left) * multiple;
            var offsetTop = (geom.Rect.Top - animation.RectFrom.Top) * multiple;
            var offsetRight = (geom.Rect.Right - animation.RectFrom.Right) * multiple;
            var offsetBottom = (geom.Rect.Bottom - animation.RectFrom.Bottom) * multiple;

            var rect = RectangleF.FromLTRB(
                offsetLeft + animation.RectFrom.Left,
                offsetTop + animation.RectFrom.Top,
                offsetRight + animation.RectFrom.Right,
                offsetBottom + animation.RectFrom.Bottom);

            var offsetDtLeft = (geomDt.Rect.Left - animation.RectDtFrom.Left) * multiple;
            var offsetDtTop = (geomDt.Rect.Top - animation.RectDtFrom.Top) * multiple;
            var offsetDtRight = (geomDt.Rect.Right - animation.RectDtFrom.Right) * multiple;
            var offsetDtBottom = (geomDt.Rect.Bottom - animation.RectDtFrom.Bottom) * multiple;

            var rectDt = RectangleF.FromLTRB(
                offsetDtLeft + animation.RectDtFrom.Left,
                offsetDtTop + animation.RectDtFrom.Top,
                offsetDtRight + animation.RectDtFrom.Right,
                offsetDtBottom + animation.RectDtFrom.Bottom);

            var radius = rect.Width / 2;
            var radiusDt = rectDt.Width / 2;
            var center = new PointF(rect.X + radius, rect.Y + radius);

            return new List<StiSegmentGeom>() {
                new StiArcSegmentGeom(rect, startAngle, sweepAngle),
                new StiLineSegmentGeom(GetPoint(center, radius, startAngle + sweepAngle), GetPoint(center, radiusDt, startAngle + sweepAngle)),
                new StiArcSegmentGeom(rectDt, startAngle + sweepAngle, -sweepAngle),
                new StiLineSegmentGeom(GetPoint(center, radiusDt, startAngle), GetPoint(center, radius, startAngle))};
        }

        public static StiPieSegmentGeom GetAnimationAngle<T>(IStiAnimationContextPainter<T> contextPainter, StiPieSegmentGeom geom, StiPieSegmentAnimation animation)
        {
            if (contextPainter.AnimationEngine == null || animation == null) return geom;
            var multiple = contextPainter.AnimationEngine.GetMultiple(contextPainter, animation);

            if (multiple == 1) return geom;
            var startAngleDelta = geom.StartAngle - animation.StartAngleFrom;
            var endAngleDelta = (geom.StartAngle + geom.SweepAngle) - animation.EndAngleFrom;
            var startAngle = animation.StartAngleFrom + (startAngleDelta * multiple);
            var sweepAngle = (animation.EndAngleFrom + (endAngleDelta * multiple)) - startAngle;

            var offsetLeft = (geom.Rect.Left - animation.RectFrom.Left) * multiple;
            var offsetTop = (geom.Rect.Top - animation.RectFrom.Top) * multiple;
            var offsetRight = (geom.Rect.Right - animation.RectFrom.Right) * multiple;
            var offsetBottom = (geom.Rect.Bottom - animation.RectFrom.Bottom) * multiple;

            var rect = RectangleF.FromLTRB(offsetLeft + animation.RectFrom.Left, offsetTop + animation.RectFrom.Top, offsetRight + animation.RectFrom.Right, offsetBottom + animation.RectFrom.Bottom);

            return new StiPieSegmentGeom(rect, startAngle, sweepAngle, animation);
        }
        #endregion
    }
}
