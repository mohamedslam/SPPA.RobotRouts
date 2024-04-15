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

namespace Stimulsoft.Report.Chart
{
    public class StiSeriesLabelsGeom : StiCellGeom
    {
        #region IStiGeomInteraction override
        public override void InvokeMouseEnter(StiInteractionOptions options)
        {
            if (!AllowMouseOver) return;

            if (!IsMouseOver)
            {
                IsMouseOver = true;
                options.UpdateContext = true;
            }
        }

        public override void InvokeMouseLeave(StiInteractionOptions options)
        {
            if (!AllowMouseOver) return;

            if (IsMouseOver)
            {
                IsMouseOver = false;
                options.UpdateContext = true;
            }
        }

        private int GetValueIndex()
        {
            int valueIndex = this.Index;

            if (this.Series is IStiClusteredBarSeries ||
                this.Series is IStiStackedBarSeries ||
                this.Series is IStiFullStackedBarSeries)
            {
                if (this.Series.Chart.Area is IStiAxisArea && !((IStiAxisArea)this.Series.Chart.Area).ReverseVert)
                    valueIndex = Series.Values.Length - valueIndex - 1;
            }
            else
            {
                if (this.Series.Chart.Area is IStiAxisArea && ((IStiAxisArea)this.Series.Chart.Area).ReverseHor)
                    valueIndex = Series.Values.Length - valueIndex - 1;
            }
            return valueIndex;
        }

        private string GetHyperlink(int valueIndex)
        {
            if (Series.Hyperlinks != null && valueIndex < Series.Hyperlinks.Length)
                return Series.Hyperlinks[valueIndex];
            else
                return null;
        }

        private string GetToolTip(int valueIndex)
        {
            if (Series.ToolTips != null && valueIndex < Series.ToolTips.Length)
                return Series.Hyperlinks[valueIndex];
            else
                return null;
        }

        public virtual bool AllowMouseOver
        {
            get
            {
                return GetHyperlink(GetValueIndex()) != null || this.Series.Interaction.DrillDownEnabled;
            }
        }

        public virtual bool IsMouseOver
        {
            get
            {
                if (this.Series == null)
                    return false;

                return this.Series.Core.GetIsMouseOverSeriesElement(this.Index);
            }
            set
            {
                if (this.Series != null)
                    this.Series.Core.SetIsMouseOverSeriesElement(this.Index, value);
            }
        }
        #endregion

        #region Properties
        public double Value { get; }

        public int Index { get; }

        public IStiSeries Series { get; }

        public IStiSeriesLabels SeriesLabels { get; }

        public TimeSpan BeginTime { get; set; } = TimeSpan.Zero;

        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        #endregion

        #region Methods
        protected virtual void DrawMarker(StiContext context, Rectangle itemRect, object markerColor, StiBrush markerBrush)
        {
            if (!SeriesLabels.MarkerVisible) return;

            var chart = this.Series.Chart as StiChart;
            var markerRect = Rectangle.Empty;

            switch (SeriesLabels.MarkerAlignment)
            {
                case StiMarkerAlignment.Right:
                    markerRect.X = (int)(itemRect.Right + 2 * context.Options.Zoom);
                    break;

                case StiMarkerAlignment.Left:
                    markerRect.X = (int)(itemRect.Left - (2 + SeriesLabels.MarkerSize.Width) * context.Options.Zoom);
                    break;

                case StiMarkerAlignment.Center:
                    markerRect.X = (int)(itemRect.Left + itemRect.Width / 2 - (2 + SeriesLabels.MarkerSize.Width) / 2 * context.Options.Zoom);
                    break;

            }
            
            markerRect.Y = (int)(itemRect.Y + (itemRect.Height - SeriesLabels.MarkerSize.Height * context.Options.Zoom) / 2);
            markerRect.Width = (int)(SeriesLabels.MarkerSize.Width * context.Options.Zoom);
            markerRect.Height = (int)(SeriesLabels.MarkerSize.Height * context.Options.Zoom);

            var color = markerColor as Color? ?? Color.Black;
            var pen = new StiPenGeom(color, 1);
            if (chart.IsAnimation)
            {
                var animation = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, StiChartHelper.GlobalBeginTimeElement);
                context.DrawAnimationRectangle(markerBrush, pen, markerRect, null, animation, null, GetToolTip(GetValueIndex()));
            }
            else
            {
                context.FillRectangle(markerBrush, markerRect.X, markerRect.Y, markerRect.Width, markerRect.Height, null);
                context.DrawRectangle(pen, markerRect.X, markerRect.Y, markerRect.Width, markerRect.Height);
            }
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
        }
        #endregion

        public StiSeriesLabelsGeom(IStiSeriesLabels seriesLabels, IStiSeries series, int index, double value, RectangleF clientRectangle)
            : base(Rectangle.Round(clientRectangle))
        {
            this.SeriesLabels = seriesLabels;
            this.Series = series;
            this.Index = index;
            this.Value = value;
        }
    }
}
