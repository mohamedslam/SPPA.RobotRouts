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
using Stimulsoft.Report.Painters;
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    public class StiStackedColumnSeriesElementGeom : StiSeriesElementGeom
    {
        #region Properties
        public Color SeriesBorderColor { get; }

        public RectangleF ColumnRectStart { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var columnRect = this.ClientRectangle;
            var pen = new StiPenGeom(SeriesBorderColor, GetSeriesBorderThickness(context.Options.Zoom));
            var chart = this.Series.Chart as StiChart;
            var cornerRadius = this.GetCornerRadius(context.Options.Zoom);

            var fontIconSeries = Series as IStiFontIconsSeries;
            if (fontIconSeries != null && fontIconSeries.Icon != null)
            {
                var roundValuesArea = chart.Area as IStiRoundValuesArea;
                var roundValues = roundValuesArea?.RoundValues;

                StiFontIconsExHelper.DrawDirectionIcons(context, this.SeriesBrush, columnRect, new SizeF(columnRect.Width, columnRect.Width), fontIconSeries.Icon.GetValueOrDefault(), this.GetToolTip(), true, roundValues.GetValueOrDefault());
                return;
            }
            
            if (cornerRadius != null && !cornerRadius.IsEmpty)
                context.PushSmoothingModeToAntiAlias();

            var animation = GetAnimation();

            if (animation != null)
            {
                if (Series.ShowShadow)
                {
                    var axisArea = AreaGeom.Area as IStiAxisArea;
                    var beginTime = new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks / ((StiAxisAreaCoreXF)axisArea.Core).ValuesCount * Index);
                    var animationOpacity = new StiOpacityAnimation(StiChartHelper.GlobalDurationElement, new TimeSpan(beginTime.Ticks + StiChartHelper.GlobalDurationElement.Ticks));
                    context.DrawShadowRect(columnRect, 5, cornerRadius, animationOpacity);
                }

                var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

                context.DrawAnimationColumn(this.SeriesBrush, brush, pen, columnRect, cornerRadius, Value, this.GetToolTip(), this, animation, GetInteractionData());
            }
            else
            {
                var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

                Series.Chart.Style.Core.FillColumn(context, columnRect, cornerRadius, brush,  GetInteractionData());

                if (IsMouseOver || Series.Core.IsMouseOver)
                {
                    context.FillCicledRectangle(StiMouseOverHelper.GetMouseOverColor(), columnRect, cornerRadius);
                }

                var pathBorder = GetBorderPath(columnRect, cornerRadius);
                context.DrawPath(pen, pathBorder, columnRect);
            }

            if (cornerRadius != null && !cornerRadius.IsEmpty)
                context.PopSmoothingMode();
        }

        private List<StiSegmentGeom> GetBorderPath(RectangleF columnRect, StiCornerRadius cornerRadius)
        {
            if (cornerRadius != null && !cornerRadius.IsEmpty)
            {
                return Value < 0
                    ? StiContextRoundedRectangleCreator.CreateWithoutTopSide(columnRect, cornerRadius, 1)
                    : StiContextRoundedRectangleCreator.CreateWithoutBottomSide(columnRect, cornerRadius, 1);
            }

            var list = new List<StiSegmentGeom>();

            if (Value > 0)
            {
                list.Add(new StiLineSegmentGeom(columnRect.X, columnRect.Bottom, columnRect.X, columnRect.Y));
                list.Add(new StiLineSegmentGeom(columnRect.X, columnRect.Y, columnRect.Right, columnRect.Y));
                list.Add(new StiLineSegmentGeom(columnRect.Right, columnRect.Y, columnRect.Right, columnRect.Bottom));
            }
            else
            {
                list.Add(new StiLineSegmentGeom(columnRect.X, columnRect.Y, columnRect.X, columnRect.Bottom));
                list.Add(new StiLineSegmentGeom(columnRect.X, columnRect.Bottom, columnRect.Right, columnRect.Bottom));
                list.Add(new StiLineSegmentGeom(columnRect.Right, columnRect.Bottom, columnRect.Right, columnRect.Y));
            }


            return list;
        }

        protected StiAnimation GetAnimation()
        {
            if (!((StiChart)Series.Chart).IsAnimation) return null;

            var axisArea = AreaGeom.Area as IStiAxisArea;

            var beginTime = new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks / ((StiAxisAreaCoreXF)axisArea.Core).ValuesCount * Index);

            var columnAnimation = new StiColumnAnimation(ColumnRectStart, ClientRectangle, StiChartHelper.GlobalDurationElement, beginTime);
            var getStartFromZero = axisArea.XAxis.Core.GetStartFromZero();
            var argId = getStartFromZero ?
                            axisArea.XAxis.Info.StripLines[Index + 1].ValueObject :
                            axisArea.XAxis.Info.StripLines[Index].ValueObject;
            argId = argId == null ? "" : argId.ToString();

            columnAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_{argId}";

            if (StiColumnAnimation.IsAnimationChangingValues(this.Series, columnAnimation.Id))
            {
                columnAnimation.ApplyPreviousAnimation(this.Series.Chart.PreviousAnimations);
            }

            return columnAnimation;
        }
        #endregion

        public StiStackedColumnSeriesElementGeom(StiAreaGeom areaGeom, double value, int index,
            StiBrush seriesBrush, Color seriesBorderColor, IStiSeries series, RectangleF clientRectangle, RectangleF columnRectStart)
            : base(areaGeom, value, index, series, clientRectangle, seriesBrush)
        {
            SeriesBorderColor = seriesBorderColor;

            this.ColumnRectStart = columnRectStart;
        }
    }
}
