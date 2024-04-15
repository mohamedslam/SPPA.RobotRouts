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

using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using System;
using Stimulsoft.Report.Chart.Geoms.Series;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiPieSeriesFullElementGeom : StiSeriesElementGeom
    {
        #region Properties
        private StiBrush brush;
        public StiBrush Brush
        {
            get
            {
                return brush;
            }
        }

        private Color borderColor;
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var rectPie = this.ClientRectangle;
            var pen = new StiPenGeom(BorderColor, GetSeriesBorderThickness(context.Options.Zoom));

            context.PushSmoothingModeToAntiAlias();

            var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.Brush)
                    : this.Brush;
            if (((StiChart)this.Series.Chart).IsAnimation)
            {
                var duration = StiChartHelper.GlobalDurationElement;
                var beginTime = StiChartHelper.GlobalBeginTimeElement;
                var animation = new StiOpacityAnimation(duration, beginTime);

                context.FillDrawAnimationEllipse(brush, pen, rectPie.X, rectPie.Y, rectPie.Width, rectPie.Height, this.GetToolTip(), this, animation, GetInteractionData());
            }
            else
            {
                context.FillEllipse(brush, rectPie, null);

                if (IsMouseOver || Series.Core.IsMouseOver)
                    context.FillEllipse(StiMouseOverHelper.GetMouseOverColor(), rectPie, null);

                context.DrawEllipse(pen, rectPie);
            }
            context.PopSmoothingMode();
        }

        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            var center = new PointF(this.ClientRectangle.X + this.ClientRectangle.Width / 2, this.ClientRectangle.Y + this.ClientRectangle.Height / 2);

            float dx = x - center.X;
            float dy = y - center.Y;
            float radius = (float)Math.Sqrt(dx * dx + dy * dy);

            return radius <= this.ClientRectangle.Width / 2;
        }
        #endregion

        public StiPieSeriesFullElementGeom(StiAreaGeom areaGeom, double value, int index, IStiPieSeries series, RectangleF clientRectangle, StiBrush brush, Color borderColor)
            : base(areaGeom, value, index, series, clientRectangle, brush)
        {
            this.brush = brush;
            this.borderColor = borderColor;
        }
    }
}