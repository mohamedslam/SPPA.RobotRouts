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

using Stimulsoft.Base;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Chart.Geoms.Series;
using Stimulsoft.Report.Helpers;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiTreemapSeriesElementGeom : StiSeriesElementGeom
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

        private StiAnimation animation;
        public StiAnimation Animation
        {
            get
            {
                return animation;
            }
        }
        #endregion

        #region Methods
        public override void Draw(StiContext context)
        {
            var chart = this.Series.Chart as StiChart;
            var rect = this.ClientRectangle;
            var cornerRadius = this.GetCornerRadius(context.Options.Zoom);

            var fontIconSeries = Series as IStiFontIconsSeries;
            if (fontIconSeries != null && fontIconSeries.Icon != null)
            {
                var sizeIcon = (float)(30 * StiScale.Factor);
                context.PushClip(rect);
                StiFontIconsExHelper.DrawFillIcons(context, this.SeriesBrush, rect, new SizeF(sizeIcon, sizeIcon), fontIconSeries.Icon.GetValueOrDefault(), this.GetToolTip());
                context.PopClip();
                return;
            }

            if (cornerRadius != null && !cornerRadius.IsEmpty)
                context.PushSmoothingModeToAntiAlias();

            var pen = new StiPenGeom(SeriesBorderColor, GetSeriesBorderThickness(context.Options.Zoom));

            var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

            if (chart.IsAnimation)
            {
                context.DrawAnimationCicledRectangle(this.SeriesBrush, brush, pen, rect, cornerRadius, this, animation, GetInteractionData(), GetToolTip());
            }

            else
            {
                base.Draw(context);

                #region Draw Box        
                Series.Chart.Style.Core.FillColumn(context, rect, cornerRadius, brush, GetInteractionData());

                if (IsMouseOver || Series.Core.IsMouseOver)
                {
                    context.FillCicledRectangle(StiMouseOverHelper.GetMouseOverColor(), rect, cornerRadius);
                }
                #endregion

                #region Draw Column Border                
                context.DrawCicledRectangle(pen, rect, cornerRadius);
                #endregion
            }

            if (cornerRadius != null && !cornerRadius.IsEmpty)
                context.PopSmoothingMode();
        }
        #endregion

        public StiTreemapSeriesElementGeom(StiAreaGeom areaGeom, double value, int index,
            StiBrush seriesBrush, Color seriesBorderColor, IStiSeries series, RectangleF clientRectangle, StiAnimation animation)
            : base(areaGeom, value, index, series, clientRectangle, seriesBrush)
        {
            this.seriesBorderColor = seriesBorderColor;

            this.animation = animation;
        }
    }
}
