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
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiSunburstSeriesElementGeom : StiSeriesElementGeom
    {
        private int index2 = 0;
        private int index3 = 0;

        #region Properties
        public RectangleF ClientRectangleDt { get; }

        public float StartAngle { get; }

        public float EndAngle { get; }

        public float RadiusFrom { get; }

        public List<StiSegmentGeom> Path { get; }

        public Color BorderColor { get; }

        public StiBrush Brush { get; }

        public float RadiusTo { get; }

        public TimeSpan BeginTime { get; }
        #endregion

        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            PointF center = new PointF(this.ClientRectangle.X + this.ClientRectangle.Width / 2, this.ClientRectangle.Y + this.ClientRectangle.Height / 2);

            float dx = x - center.X;
            float dy = y - center.Y;
            float radius = (float)Math.Sqrt(dx * dx + dy * dy);

            if (radius < this.RadiusTo && radius > this.RadiusFrom)
            {
                float alpha = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
                if (alpha < 0) alpha += 360;

                return alpha >= StartAngle && alpha <= EndAngle;
            }

            return false;
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var rectChart = this.ClientRectangle;

            var pen = new StiPenGeom(BorderColor, GetSeriesBorderThickness(context.Options.Zoom));
            pen.Alignment = StiPenAlignment.Inset;

            context.PushSmoothingModeToAntiAlias();

            var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.Brush)
                    : this.Brush;

            var animation = GetAnimation();
            if (animation != null)
            {
                context.DrawAnimationPathElement(brush, pen, this.Path, rectChart, this.GetToolTip(), null, animation, GetInteractionData());
            }
            else
            {

                if (Path != null)
                {
                    context.FillPath(brush, Path, rectChart, GetInteractionData());


                    if (IsMouseOver || Series.Core.IsMouseOver)
                        context.FillPath(StiMouseOverHelper.GetMouseOverColor(), Path, rectChart, null);
                }


                context.DrawPath(pen, this.Path, StiPathGeom.GetBoundsState);
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
                pieAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_{Index}_{index2}_{index3}";

                if (StiPieSegmentAnimation.IsAnimationChangingValues(this.Series, pieAnimation.Id))
                {
                    pieAnimation.ApplyPreviousAnimation(this.Series.Chart.PreviousAnimations);
                    animation = pieAnimation;
                }
            }

            if (animation == null)
            {
                var beginTimeAnimationOpacity = BeginTime;
                if (this.Series.Chart.PreviousAnimations.Count > 0) beginTimeAnimationOpacity = TimeSpan.FromMilliseconds(300);
                var opacityAnimation = new StiOpacityAnimation(duration, beginTimeAnimationOpacity);

                opacityAnimation.AnotherAnimation = pieAnimation;
                opacityAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_{Index}_{index2}_{index3}";

                animation = opacityAnimation;
            }

            return animation;
        }
        #endregion

        public StiSunburstSeriesElementGeom(StiAreaGeom areaGeom, double value, int index1, int index2, int index3,
            IStiSeries series, RectangleF clientRectangle, RectangleF clientRectangleDt,
            List<StiSegmentGeom> path,
            Color borderColor, StiBrush brush,
            float startAngle, float endAngle, float radiusFrom, float radiusTo, TimeSpan beginTime)
            : base(areaGeom, value, index2, series, clientRectangle, brush)
        {
            this.index2 = index2;
            this.index3 = index3;
            this.ClientRectangleDt = clientRectangleDt;
            this.Path = path;
            this.BorderColor = borderColor;
            this.Brush = brush;
            this.StartAngle = startAngle;
            this.EndAngle = endAngle;
            this.RadiusFrom = radiusFrom;
            this.RadiusTo = radiusTo;

            this.BeginTime = beginTime;
        }
    }
}
