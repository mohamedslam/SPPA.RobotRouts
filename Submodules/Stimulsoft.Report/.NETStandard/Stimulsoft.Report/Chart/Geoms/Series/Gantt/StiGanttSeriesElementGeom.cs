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
    public class StiGanttSeriesElementGeom : StiSeriesElementGeom
    {
        #region Properties
        public RectangleF RectFrom { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var currSeries = Series as IStiGanttSeries;
            var columnRect = this.ClientRectangle;
            var chart = this.Series.Chart as StiChart;
            var cornerRadius = StiCornerRadiusHelper.Rotation90(this.GetCornerRadius(context.Options.Zoom));
            cornerRadius = StiCornerRadiusHelper.FlipHorizontal(cornerRadius);

            var fontIconSeries = Series as IStiFontIconsSeries;
            if (fontIconSeries != null && fontIconSeries.Icon != null)
            {
                var roundValuesArea = chart.Area as IStiRoundValuesArea;
                var roundValues = roundValuesArea?.RoundValues;

                StiFontIconsExHelper.DrawDirectionIcons(context, this.SeriesBrush, columnRect, new SizeF(columnRect.Height, columnRect.Height), fontIconSeries.Icon.GetValueOrDefault(), this.GetToolTip(), false, roundValues.GetValueOrDefault());
                return;
            }

            if (cornerRadius != null && !cornerRadius.IsEmpty)
                context.PushSmoothingModeToAntiAlias();

            var pen = new StiPenGeom(currSeries.BorderColor, GetSeriesBorderThickness(context.Options.Zoom));

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

                var brush1 = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

                context.DrawAnimationBar(this.SeriesBrush, brush1, pen, columnRect, cornerRadius, Value, this.GetToolTip(), this, animation, GetInteractionData());
            }
            else
            {
                base.Draw(context);

                #region Draw Shadow
                if (currSeries.ShowShadow && columnRect.Width > 4 && columnRect.Height > 4)
                {
                    context.DrawCachedShadow(columnRect, StiShadowSides.All, context.Options.IsPrinting);
                }
                #endregion

                #region Draw Series
                var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

                context.FillCicledRectangle(brush, columnRect, cornerRadius, GetInteractionData());

                if (IsMouseOver || Series.Core.IsMouseOver)
                {
                    context.FillCicledRectangle(StiMouseOverHelper.GetMouseOverColor(), columnRect, cornerRadius);
                }

                context.DrawCicledRectangle(pen, columnRect, cornerRadius);
                #endregion

                if (cornerRadius != null && !cornerRadius.IsEmpty)
                    context.PopSmoothingMode();
            }
        }

        protected StiAnimation GetAnimation()
        {
            if (!((StiChart)Series.Chart).IsAnimation) return null;

            var axisArea = AreaGeom.Area as IStiAxisArea;

            var beginTime = new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks / ((StiAxisAreaCoreXF)axisArea.Core).ValuesCount * Index);

            var columnAnimation = new StiColumnAnimation(RectFrom, ClientRectangle, StiChartHelper.GlobalDurationElement, beginTime);
            var getStartFromZero = axisArea.YAxis.Core.GetStartFromZero();

            object argId = null;

            if (getStartFromZero && axisArea.YAxis.Info.StripLines.Count > Index + 1)
                argId = axisArea.YAxis.Info.StripLines[Index + 1].ValueObject;

            else if (axisArea.YAxis.Info.StripLines.Count > Index)
                argId = axisArea.YAxis.Info.StripLines[Index].ValueObject;

            argId = argId == null ? "" : argId.ToString();

            columnAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_{argId}";

            if (StiColumnAnimation.IsAnimationChangingValues(this.Series, columnAnimation.Id))
            {
                columnAnimation.ApplyPreviousAnimation(this.Series.Chart.PreviousAnimations);
            }

            return columnAnimation;
        }
        #endregion

        public StiGanttSeriesElementGeom(StiAreaGeom areaGeom, double value, int index,
            IStiSeries series, RectangleF clientRectangle, RectangleF rectFrom, StiBrush brush)
            : base(areaGeom, value, index, series, clientRectangle, brush)
        {
            RectFrom = rectFrom;
        }
    }
}
