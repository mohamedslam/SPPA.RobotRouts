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
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Report.Chart.Geoms.Series;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Chart
{
    public class StiDoughnutSeriesElementGeom : StiSeriesElementGeom
    {
        #region Properties
        public RectangleF ClientRectangleDt { get; }

        public List<StiSegmentGeom> Path { get; }

        public List<StiSegmentGeom> PathLight { get; }

        public List<StiSegmentGeom> PathDark { get; }

        public Color BorderColor { get; }

        public StiBrush Brush { get; }

        public StiBrush BrushLight { get; }

        public StiBrush BrushDark { get; }

        public float StartAngle { get; }

        public float EndAngle { get; }

        public float RadiusFrom { get; }

        public float RadiusTo { get; }

        public TimeSpan BeginTime { get; }
        #endregion

        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            var center = new PointF(this.ClientRectangle.X + this.ClientRectangle.Width / 2, this.ClientRectangle.Y + this.ClientRectangle.Height / 2);

            float dx = x - center.X;
            float dy = y - center.Y;
            float radius = (float)Math.Sqrt(dx * dx + dy * dy);

            if (radius < this.RadiusTo || radius > this.RadiusFrom) return false;

            float alpha = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
            if (alpha < 0) alpha += 360;

            return alpha >= StartAngle && alpha <= EndAngle;
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var rectPie = this.ClientRectangle;

            var pen = new StiPenGeom(BorderColor, GetSeriesBorderThickness(context.Options.Zoom));
            pen.Alignment = StiPenAlignment.Inset;

            var fontIconSeries = Series as IStiFontIconsSeries;
            if (fontIconSeries != null && fontIconSeries.Icon != null)
            {
                var sizeIcon = (float)(30 * StiScale.Factor);
                context.PushClipPath(Path);
                StiFontIconsExHelper.DrawFillIcons(context, this.SeriesBrush, rectPie, new SizeF(sizeIcon, sizeIcon), fontIconSeries.Icon.GetValueOrDefault(), this.GetToolTip());
                context.PopClip();
                return;
            }

            context.PushSmoothingModeToAntiAlias();

            var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(Brush)
                    : Brush;

            var animation = GetAnimation();
            if (animation != null)
            {    
                context.DrawAnimationPathElement(Brush, brush, pen, Path, rectPie, this.GetToolTip(), this, animation, GetInteractionData());

                #region Draw light
                if (PathLight != null)
                    context.DrawAnimationPathElement(BrushLight, null, PathLight, rectPie, this.GetToolTip(), null, animation, GetInteractionData());

                if (PathDark != null)
                    context.DrawAnimationPathElement(BrushDark, null, PathDark, rectPie, this.GetToolTip(), null, animation, GetInteractionData());
                #endregion
            }
            else
            {
                if (Path != null)
                {
                    context.FillPath(brush, Path, rectPie, GetInteractionData());

                    if (IsMouseOver || Series.Core.IsMouseOver)
                        context.FillPath(StiMouseOverHelper.GetMouseOverColor(), Path, rectPie, null);
                }

                #region Draw light
                if (PathLight != null)
                    context.FillPath(BrushLight, PathLight, rectPie, null);

                if (PathDark != null)
                    context.FillPath(BrushDark, PathDark, rectPie, null);
                #endregion

                if (!Color.Transparent.Equals(BorderColor))
                    context.DrawPath(pen, Path, null);
            }
            context.PopSmoothingMode();
        }

        private StiAnimation GetAnimation()
        {
            if (!((StiChart)Series.Chart).IsAnimation) return null;

            var duration = StiChartHelper.GlobalDurationElement;
            var beginTime = StiChartHelper.GlobalBeginTimeElement;

            StiAnimation animation = null;
            StiPieSegmentAnimation pieAnimation = null;
            if (((StiChart)Series.Chart).IsAnimationChangingValues)
            {
                var startAngleTo = StartAngle;
                var endAngleTo = EndAngle;
                var rectTo = ClientRectangle;
                var rectDtTo = ClientRectangle;

                if (Path.Count == 4 && Path[0] is StiArcSegmentGeom arcGeom1 && Path[2] is StiArcSegmentGeom arcGeom2)
                {
                    startAngleTo = arcGeom1.StartAngle;
                    endAngleTo = arcGeom1.StartAngle + arcGeom1.SweepAngle;
                    rectTo = arcGeom1.Rect;
                    rectDtTo = arcGeom2.Rect;
                }

                pieAnimation = new StiPieSegmentAnimation(ClientRectangle, rectTo, ClientRectangleDt, rectDtTo, StartAngle, EndAngle, startAngleTo, endAngleTo, duration, TimeSpan.Zero);
                pieAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_{Index}";

                if (StiPieSegmentAnimation.IsAnimationChangingValues(this.Series, pieAnimation.Id))
                {
                    pieAnimation.ApplyPreviousAnimation(this.Series.Chart.PreviousAnimations);
                    animation = pieAnimation;
                }
            }

            if (animation == null)
            {
                var beginTimeAnimationOpacity = new TimeSpan(beginTime.Ticks / Series.Values.Length * Index);
                if (this.Series.Chart.PreviousAnimations.Count > 0) beginTimeAnimationOpacity = TimeSpan.FromMilliseconds(300);
                var opacityAnimation = new StiOpacityAnimation(duration, beginTimeAnimationOpacity);

                opacityAnimation.AnotherAnimation = pieAnimation;
                opacityAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_{Index}";

                animation = opacityAnimation;
            }

            return animation;
        }
        #endregion

        public StiDoughnutSeriesElementGeom(StiAreaGeom areaGeom, double value, int index,
            IStiDoughnutSeries series, RectangleF clientRectangle, RectangleF clientRectangleDt,
            List<StiSegmentGeom> path, List<StiSegmentGeom> pathLight, List<StiSegmentGeom> pathDark,
            Color borderColor, StiBrush brush, StiBrush brushLight, StiBrush brushDark,
            float startAngle, float endAngle, float radiusFrom, float radiusTo, TimeSpan beginTime)
            : base(areaGeom, value, index, series, clientRectangle, brush)
        {
            this.ClientRectangleDt = clientRectangleDt;
            this.Path = path;
            this.PathLight = pathLight;
            this.PathDark = pathDark;
            this.BorderColor = borderColor;
            this.Brush = brush;
            this.BrushLight = brushLight;
            this.BrushDark = brushDark;
            this.StartAngle = startAngle;
            this.EndAngle = endAngle;
            this.RadiusFrom = radiusFrom;
            this.RadiusTo = radiusTo;

            this.BeginTime = beginTime;
        }
    }
}
