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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Report.Chart.Geoms.Series;
using Stimulsoft.Report.Helpers;

namespace Stimulsoft.Report.Chart
{
    public class StiBubbleSeriesElementGeom : StiSeriesElementGeom
    {
        #region Properties
        private Color seriesBorderColor;
        public Color SeriesBorderColor
        {
            get
            {
                return seriesBorderColor;
            }
        }
        
        private TimeSpan beginTime;
        public TimeSpan BeginTime
        {
            get
            {
                return beginTime;
            }
        }
        #endregion

        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;
            PointF center = new PointF(this.ClientRectangle.X + this.ClientRectangle.Width / 2, this.ClientRectangle.Y + this.ClientRectangle.Height / 2);
            
            float dx = Math.Abs(center.X - x);
            float dy = Math.Abs(center.Y - y);
            float radius = (float)Math.Sqrt(dx * dx + dy * dy);

            return radius <= this.ClientRectangle.Width / 2;
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var rect = this.ClientRectangle;
            var pen = new StiPenGeom(this.SeriesBorderColor, GetSeriesBorderThickness(context.Options.Zoom));
            var chart = this.Series.Chart as StiChart;

            var fontIconSeries = Series as IStiFontIconsSeries;
            if (fontIconSeries != null && fontIconSeries.Icon != null)
            {
                StiFontIconsExHelper.DrawDirectionIcons(context, this.SeriesBrush, rect, new SizeF(rect.Height, rect.Height), fontIconSeries.Icon.GetValueOrDefault(), this.GetToolTip(), false);
                return;
            }
            #region Draw Bubble
            context.PushSmoothingModeToAntiAlias();

            if (chart.IsAnimation)
            {
                var animation = new StiScaleAnimation(StiChartHelper.GlobalDurationElement, beginTime);

                var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

                context.FillDrawAnimationEllipse(this.SeriesBrush, brush, pen, rect.X, rect.Y, rect.Width, rect.Height, this.GetToolTip(), this, animation, GetInteractionData());
            }

            else
            {
                if (Series.ShowShadow)
                {
                    var shadowBrush = new StiSolidBrush(Color.FromArgb(100, Color.Black));

                    var shadowContext = context.CreateShadowGraphics();

                    var rectShadow = rect;
                    rectShadow.X = 0;
                    rectShadow.Y = 0;
                    rectShadow.X += 4 * context.Options.Zoom;
                    rectShadow.Y += 4 * context.Options.Zoom;
                    shadowContext.FillEllipse(shadowBrush, rectShadow, null);

                    context.DrawShadow(shadowContext, rect, 0);
                }

                var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

                context.FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height, GetToolTip(), GetInteractionData());

                if (IsMouseOver || Series.Core.IsMouseOver)
                    context.FillEllipse(StiMouseOverHelper.GetMouseOverColor(), rect.X, rect.Y, rect.Width, rect.Height, GetToolTip(), GetInteractionData());

                context.DrawEllipse(pen, rect);                
            }
            context.PopSmoothingMode();
            #endregion
        }
        #endregion

        public StiBubbleSeriesElementGeom(StiAreaGeom areaGeom, double value, int index,
            StiBrush seriesBrush, Color seriesBorderColor, IStiSeries series, RectangleF clientRectangle, TimeSpan beginTime)
            : base(areaGeom, value, index, series, clientRectangle, seriesBrush)
        {
            this.seriesBorderColor = seriesBorderColor;

            this.beginTime = beginTime;
        }
    }
}
