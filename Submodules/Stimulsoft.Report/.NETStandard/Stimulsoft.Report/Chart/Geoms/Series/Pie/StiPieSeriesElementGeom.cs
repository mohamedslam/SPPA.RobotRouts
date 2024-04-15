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
    public class StiPieSeriesElementGeom : StiSeriesElementGeom
    {
        #region Properties
        public List<StiSegmentGeom> Path { get; }

        public List<StiSegmentGeom> PathLight { get; }

        public Color BorderColor { get; }

        public float StartAngle { get; }

        public float EndAngle { get; }

        public float Radius { get; }

        public int Count { get; set; }
        #endregion

        #region Methods
        public override StiInteractionToolTipPointOptions GetToolTipPoint()
        {
            var center = new PointF(this.ClientRectangle.X + this.ClientRectangle.Width / 2, this.ClientRectangle.Y + this.ClientRectangle.Height / 2);

            var angle = StartAngle + (EndAngle - StartAngle) / 2;
            var radius = this.Radius * 0.8f;

            angle = (float)(angle * Math.PI / 180);

            var x1 = center.X + radius * Math.Cos(angle);
            var y1 = center.Y + radius * Math.Sin(angle);

            var point = new Point((int)x1, (int)y1);

            var options = new StiInteractionToolTipPointOptions()
            {
                ToolTipAlignment = StiToolTipAlignment.Top,
                ToolTipPoint = point
            };

            return options;
        }

        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            var center = new PointF(this.ClientRectangle.X + this.ClientRectangle.Width / 2, this.ClientRectangle.Y + this.ClientRectangle.Height / 2);

            var dx = x - center.X;
            var dy = y - center.Y;
            var radius = (float)Math.Sqrt(dx * dx + dy * dy);

            if (radius >= this.Radius) return false;

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

            var penColor = Color.Transparent.Equals(BorderColor) ? BorderColor : StiColorUtils.Dark(BorderColor, 10);
            var pen = new StiPenGeom(penColor, GetSeriesBorderThickness(context.Options.Zoom))
            {
                Alignment = StiPenAlignment.Inset
            };

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

            var animation = GetAnimation();
            if (animation != null)
            {
                #region Draw Pie Segment
                var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

                context.DrawAnimationPathElement(this.SeriesBrush, brush, pen, Path, rectPie, this.GetToolTip(), this, animation, GetInteractionData());
                #endregion

                #region Render lights
                if (PathLight != null)
                {
                    var brLight = new StiSolidBrush(Color.FromArgb(30, Color.Black));
                    context.DrawAnimationPathElement(brLight, null, PathLight, rectPie, this.GetToolTip(), null, animation, GetInteractionData());
                }
                #endregion
            }

            else
            {
                #region Draw Pie Segment
                var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

                context.FillPath(brush, Path, rectPie, GetInteractionData());

                if (IsMouseOver || Series.Core.IsMouseOver)
                    context.FillPath(StiMouseOverHelper.GetMouseOverColor(), Path, rectPie, null);
                #endregion

                #region Render lights
                if (PathLight != null)
                {
                    var brLight = new StiSolidBrush(Color.FromArgb(30, Color.Black));
                    context.FillPath(brLight, PathLight, rectPie, null);
                }
                #endregion

                #region Draw Pie Segment Border
                if (!Color.Transparent.Equals(BorderColor))
                    context.DrawPath(pen, Path, null);
                #endregion
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
            if (this.Series.Chart.IsAnimationChangingValues)
            {
                var startAngleTo = StartAngle;
                var endAngleTo = EndAngle;
                var rectTo = ClientRectangle;

                if (Path[0] is StiPieSegmentGeom pieGeom)
                {
                    startAngleTo = pieGeom.StartAngle;
                    endAngleTo = pieGeom.StartAngle + pieGeom.SweepAngle;
                    rectTo = pieGeom.Rect;
                }

                pieAnimation = new StiPieSegmentAnimation(ClientRectangle, rectTo, StartAngle, EndAngle, startAngleTo, endAngleTo, duration, TimeSpan.Zero);
                pieAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_{Index}";

                if (StiPieSegmentAnimation.IsAnimationChangingValues(this.Series, pieAnimation.Id))
                {
                    pieAnimation.ApplyPreviousAnimation(this.Series.Chart.PreviousAnimations);
                    animation = pieAnimation;
                }
            }

            if (animation == null)
            {
                var beginTimeAnimationOpacity = new TimeSpan(beginTime.Ticks / Count * Index);
                if (this.Series.Chart.PreviousAnimations.Count > 0) beginTimeAnimationOpacity = TimeSpan.FromMilliseconds(300);
                var opacityAnimation = new StiOpacityAnimation(duration, beginTimeAnimationOpacity);

                opacityAnimation.AnotherAnimation = pieAnimation;
                opacityAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_{Index}";

                animation = opacityAnimation;
            }

            return animation;
        }
        #endregion

        public StiPieSeriesElementGeom(StiAreaGeom areaGeom, double value, int index, IStiPieSeries series, RectangleF clientRectangle,
            List<StiSegmentGeom> path, List<StiSegmentGeom> pathLight, Color borderColor, StiBrush brush, float startAngle, float endAngle,
            float radius)
            : base(areaGeom, value, index, series, clientRectangle, brush)
        {
            this.Path = path;
            this.PathLight = pathLight;
            this.BorderColor = borderColor;
            this.StartAngle = startAngle;
            this.EndAngle = endAngle;
            this.Radius = radius;
        }
    }
}
