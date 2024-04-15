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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiRadarPointSeriesElementGeom : StiSeriesElementGeom
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

            int valueIndex = GetValueIndex();

            options.InteractionToolTip = GetToolTip(valueIndex);
            options.InteractionHyperlink = GetHyperlink(valueIndex);
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

        public override void InvokeClick(StiInteractionOptions options)
        {
            int valueIndex = GetValueIndex();

            if (Series.Hyperlinks != null && valueIndex < Series.Hyperlinks.Length)
            {
                options.InteractionHyperlink = Series.Hyperlinks[valueIndex];
            }

            if (Series.Interaction.DrillDownEnabled)
            {
                options.SeriesInteractionData = this.Interaction;

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
                return Series.ToolTips[valueIndex];
            else
                return null;
        }
        #endregion

        #region Properties
        private PointF point;
        public PointF Point
        {
            get
            {
                return point;
            }
        }
        #endregion

        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;
            return GetMouseOverRect().Contains(x, y);
        }

        internal RectangleF GetMouseOverRect()
        {
            RectangleF rect = this.ClientRectangle;
            rect.Inflate(rect.Width / 2, rect.Height / 2);
            return rect;
        }

        public override StiInteractionToolTipPointOptions GetToolTipPoint()
        {
            var rect = GetMouseOverRect();
            var point = new Point((int)(rect.X + rect.Width / 2), (int)(rect.Y));

            var options = new StiInteractionToolTipPointOptions()
            {
                ToolTipPoint = point,
                ToolTipAlignment = StiToolTipAlignment.Top
            };

            return options;
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var radarSeries = Series as IStiRadarSeries;

            var isTooltipMarkerMode = !radarSeries.Marker.Visible && radarSeries.ToolTips.Length > 0;

            if (radarSeries.Marker != null && (radarSeries.Marker.Visible || isTooltipMarkerMode))
            {
                context.PushSmoothingModeToAntiAlias();

                float chartZoom = context.Options.Zoom;

                #region IsMouseOver
                if (IsMouseOver)
                    context.FillEllipse(StiMouseOverHelper.GetLineMouseOverColor(radarSeries.Marker.Brush), GetMouseOverRect(), null);
                #endregion

                var chart = this.Series.Chart as StiChart;
                var interaction = chart.IsAnimation ? new StiInteractionDataGeom()
                {
                    ComponentName = chart.Name,
                    ComponentIndex = chart.Page != null ? chart.Page.Components.IndexOf(chart).ToString() : "",
                    PageGuid = ((StiSeries)this.Series).DrillDownEnabled ? ((StiSeries)this.Series).DrillDownPageGuid : "",
                    PageIndex = chart.Page != null ? chart.Page.Report.RenderedPages.IndexOf(chart.Page).ToString() : "",
                    ElementIndex = this.ElementIndex.ToString(),
                    InteractionData = this.Interaction
                } : null;

                radarSeries.Marker.Core.Draw(context, radarSeries.Marker, this.Point, chartZoom, radarSeries.ShowShadow, IsMouseOver, isTooltipMarkerMode, chart.IsAnimation, this.GetToolTip(), this, interaction);

                context.PopSmoothingMode();
            }
        }
        #endregion

        public StiRadarPointSeriesElementGeom(StiAreaGeom areaGeom, double value, int index, StiBrush brush,
            IStiRadarSeries series, PointF point, float zoom)
            :
            base(areaGeom, value, index, series, StiMarkerCoreXF.GetMarkerRect(point, series.Marker.Size, zoom), brush)
        {
            this.point = point;
        }
    }
}
