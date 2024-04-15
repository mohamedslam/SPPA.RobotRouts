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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Report.Chart.Geoms.Series;
using System.Reflection;

namespace Stimulsoft.Report.Chart
{
    public class StiRangeBarElementGeom : StiSeriesElementGeom
    {
        #region Properties
        public RectangleF RectFrom { get; }
        #endregion

        #region Methods
        public override void Draw(StiContext context)
        {
            var currSeries = Series as IStiRangeBarSeries;
            var seriesRect = this.ClientRectangle;
            var cornerRadius = GetCornerRadius(context.Options.Zoom);

            var pen = new StiPenGeom(currSeries.BorderColor, GetSeriesBorderThickness(context.Options.Zoom));

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
                    context.DrawShadowRect(seriesRect, 5, cornerRadius, animationOpacity);
                }

                var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

                context.DrawAnimationColumn(this.SeriesBrush, brush, pen, seriesRect, cornerRadius, Value, this.GetToolTip(), this, animation, GetInteractionData());
            }
            else
            {
                #region Draw Shadow
                if (currSeries.ShowShadow && seriesRect.Width > 4 && seriesRect.Height > 4)
                {
                    context.DrawCachedShadow(seriesRect, StiShadowSides.All, context.Options.IsPrinting);
                }
                #endregion

                #region Draw Series
                var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

                context.FillCicledRectangle(brush, seriesRect, cornerRadius, GetInteractionData());

                if (IsMouseOver || Series.Core.IsMouseOver)
                {
                    context.FillCicledRectangle(StiMouseOverHelper.GetMouseOverColor(), seriesRect, cornerRadius);
                }

                context.DrawCicledRectangle(pen, seriesRect, cornerRadius);
                #endregion
            }

            if (cornerRadius != null && !cornerRadius.IsEmpty)
                context.PopSmoothingMode();
        }

        protected StiAnimation GetAnimation()
        {
            if (!((StiChart)Series.Chart).IsAnimation) return null;

            var currSeries = (IStiRangeBarSeries)Series;

            var axisArea = AreaGeom.Area as IStiAxisArea;
            int valuesCount = Series.Values.Length;
            if (currSeries.ValuesEnd.Length < valuesCount) valuesCount = currSeries.ValuesEnd.Length;

            int argumentsCount = currSeries.Arguments.Length;
            int count = Math.Min(valuesCount, argumentsCount);

            var beginTime = new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks / count * Index);

            int argumentIndex = axisArea.XAxis.Info.StripLines.Count - 1;
            foreach (StiStripLineXF line in axisArea.XAxis.Info.StripLines)
            {
                if (Series.Arguments[Index].ToString() == line.ValueObject?.ToString()) break;
                argumentIndex--;
            }

            argumentIndex = axisArea.XAxis.Info.StripLines.Count - 1 - argumentIndex;

            var columnAnimation = new StiColumnAnimation(RectFrom, ClientRectangle, StiChartHelper.GlobalDurationElement, beginTime);
            var argId = axisArea.XAxis.Info.StripLines[axisArea.ReverseVert ? axisArea.XAxis.Info.StripLines.Count - 1 - argumentIndex : argumentIndex].ValueObject;
            argId = argId == null ? "" : argId.ToString();
            columnAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_{argId}";

            if (StiColumnAnimation.IsAnimationChangingValues(this.Series, columnAnimation.Id))
            {
                columnAnimation.ApplyPreviousAnimation(this.Series.Chart.PreviousAnimations);
            }

            return columnAnimation;
        }
        #endregion

        public StiRangeBarElementGeom(StiAreaGeom areaGeom, double value, int index,
            IStiSeries series, StiBrush brush, RectangleF clientRectangle, RectangleF rectFrom)
            : base(areaGeom, value, index, series, clientRectangle, brush)
        {
            RectFrom = rectFrom;
        }
    }
}
