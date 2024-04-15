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
using Stimulsoft.Report.Helpers;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Chart
{
    public class StiFunnelSeriesElementGeom : StiSeriesElementGeom
    {
        #region Properties
        public List<StiSegmentGeom> Path { get; }

        public Color BorderColor { get; }

        public StiBrush Brush { get; }

        public TimeSpan BeginTime { get; }
        #endregion

        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            var linesSegment = Path.OfType<StiLinesSegmentGeom>().FirstOrDefault();

            if (linesSegment == null) return false;

            return IsPointInPolygon(new PointF(x, y), linesSegment.Points);
        }

        private bool IsPointInPolygon(PointF p, PointF[] polygon)
        {
            double minX = polygon[0].X;
            double maxX = polygon[0].X;
            double minY = polygon[0].Y;
            double maxY = polygon[0].Y;
            for (int i = 1; i < polygon.Length; i++)
            {
                PointF q = polygon[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].Y > p.Y) != (polygon[j].Y > p.Y) &&
                     p.X < (polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public override void Draw(StiContext context)
        {
            var rect = this.ClientRectangle;

            var pen = new StiPenGeom(StiColorUtils.Dark(BorderColor, 10), GetSeriesBorderThickness(context.Options.Zoom));
            pen.Alignment = StiPenAlignment.Inset;

            var chart = this.Series.Chart as StiChart;

            var fontIconSeries = Series as IStiFontIconsSeries;
            if (fontIconSeries != null && fontIconSeries.Icon != null)
            {
                var sizeIcon = (float)(30 * StiScale.Factor);
                context.PushClipPath(Path);
                StiFontIconsExHelper.DrawFillIcons(context, this.SeriesBrush, rect, new SizeF(sizeIcon, sizeIcon), fontIconSeries.Icon.GetValueOrDefault(), this.GetToolTip());
                context.PopClip();
                return;
            }

            if (chart.IsAnimation)
            {
                var animationOpacity = new StiOpacityAnimation(TimeSpan.FromSeconds(1), BeginTime);

                #region Draw Funnel Element Shadow
                if (Series.ShowShadow)
                {
                    context.PushTranslateTransform(4, 4);
                    context.DrawAnimationPathElement(null, new StiSolidBrush(Color.FromArgb(50, 100, 100, 100)), null, Path, rect, null, this, animationOpacity);
                    context.PopTransform();
                }
                #endregion

                #region Draw Funnel Element
                var brush1 = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(Brush)
                    : Brush;

                context.DrawAnimationPathElement(Brush, brush1, null, Path, rect, this.GetToolTip(), this, animationOpacity, GetInteractionData());
                #endregion

                #region Draw Funnel Element Border
                if (!Color.Transparent.Equals(BorderColor))
                    context.DrawAnimationPathElement(null, null, pen, Path, rect, null, this, animationOpacity);
                #endregion
            }
            else
            {
                context.PushSmoothingModeToAntiAlias();

                #region Draw Funnel Element Shadow
                if (Series.ShowShadow)
                {
                    context.PushTranslateTransform(4, 4);
                    context.FillPath(Color.FromArgb(50, 100, 100, 100), Path, rect, null);
                    context.PopTransform();
                }
                #endregion

                #region Draw Funnel Element

                var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(Brush)
                    : Brush;

                context.FillPath(brush, Path, rect, GetInteractionData());

                if (IsMouseOver || Series.Core.IsMouseOver)
                    context.FillPath(StiMouseOverHelper.GetMouseOverColor(), Path, rect, null);
                #endregion

                #region Draw Funnel Element Border
                if (!Color.Transparent.Equals(BorderColor))
                    context.DrawPath(pen, Path, null);
                #endregion

                context.PopSmoothingMode();
            }

        }
        #endregion

        public StiFunnelSeriesElementGeom(StiAreaGeom areaGeom, double value, int index,
            IStiSeries series, RectangleF clientRectangle, StiBrush brush, Color borderColor, List<StiSegmentGeom> path, TimeSpan beginTime)
            : base(areaGeom, value, index, series, clientRectangle, brush)
        {
            this.Path = path;
            this.BorderColor = borderColor;
            this.Brush = brush;

            this.BeginTime = beginTime;
        }
    }
}
