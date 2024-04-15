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
    public class StiMarkerGeom : StiCellGeom, IStiSeriesElement
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
                options.InteractionHyperlink = series.Hyperlinks[valueIndex];
            }

            if (Series.Interaction.DrillDownEnabled)
            {
                options.SeriesInteractionData = this.Interaction;

                IsMouseOver = false;
                options.UpdateContext = true;
            }

            if (options.SelectOnClick)
                this.IsSelected = !this.IsSelected;
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

        internal string GetHyperlink()
        {
            return GetHyperlink(GetValueIndex());
        }

        private string GetHyperlink(int valueIndex)
        {
            if (Series.Hyperlinks != null && valueIndex < Series.Hyperlinks.Length)
                return series.Hyperlinks[valueIndex];
            else
                return null;
        }

        internal string GetToolTip()
        {
            return GetToolTip(GetValueIndex());
        }

        private string GetToolTip(int valueIndex)
        {
            string textTooltip = null;

            if (Series.ToolTips != null && valueIndex < Series.ToolTips.Length)
                textTooltip = Series.ToolTips[valueIndex];

            if (!string.IsNullOrEmpty(textTooltip) && textTooltip.Contains("\"StiColor\":\"#ffffff\""))
            {
                var color = StiBrush.ToColor(this.series.ProcessSeriesBrushes(index, this.Marker.Brush));
                textTooltip = textTooltip.Replace("\"StiColor\":\"#ffffff\"", $"\"StiColor\":\"{ColorTranslator.ToHtml(color)}\"");
            }

            return textTooltip;
        }

        public StiInteractionToolTipPointOptions GetToolTipPoint()
        {
            var point = new PointF(this.ClientRectangle.X + this.ClientRectangle.Width / 2, this.ClientRectangle.Y);

            var options = new StiInteractionToolTipPointOptions()
            {
                ToolTipPoint = point,
                ToolTipAlignment = StiToolTipAlignment.Top
            };

            return options;
        }

        public virtual bool AllowMouseOver
        {
            get
            {
                int index = GetValueIndex();
                return 
                    GetHyperlink(GetValueIndex()) != null || 
                    (Series.ToolTips != null && index < Series.ToolTips.Length) ||
                    (this.Series.Interaction.DrillDownEnabled && this.Series.Interaction.AllowSeriesElements);
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

        public virtual bool IsSelected
        {
            get
            {
                if (!((StiChart)this.Series.Chart).IsDashboard)
                    return false;

                var key = GetHashKey();
                if (key == null)
                    return false;

                return ((StiChart)this.Series.Chart).Core.GetIsSelectedSeriesElement(key);
            }
            set
            {
                var key = GetHashKey();
                if (key != null)
                    ((StiChart)this.Series.Chart).Core.SetIsSelectedSeriesElement(key, value);
            }
        }
        #endregion

        #region Properties
        private StiSeriesInteractionData interaction;
        public StiSeriesInteractionData Interaction
        {
            get
            {
                return interaction;
            }
            set
            {
                interaction = value;
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return index;
            }
        }

        private PointF point;
        public PointF Point
        {
            get
            {
                return point;
            }
        }

        private IStiMarker marker;
        public IStiMarker Marker
        {
            get
            {
                return marker;
            }
        }

        private double value;
        public double Value
        {
            get
            {
                return value;
            }
        }

        private bool showShadow;
        public bool ShowShadow
        {
            get
            {
                return showShadow;
            }
        }

        private bool isTooltipMode;
        public bool IsTooltipMode
        {
            get
            {
                return isTooltipMode;
            }
        }

        private IStiSeries series;
        public IStiSeries Series
        {
            get
            {
                return series;
            }
        }

        private string elementIndex;
        public string ElementIndex
        {
            get
            {
                return elementIndex;
            }
            set
            {
                elementIndex = value;
            }
        }
        #endregion

        #region Methods
        public string GetHashKey()
        {
            if (this.Interaction == null)
                return null;

            return $"{this.Interaction.Argument}-{this.Interaction.Value}-{this.Series.CoreTitle}";
        }

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

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            if (this.Marker.Size <= 0) return;

            context.PushSmoothingModeToAntiAlias();

            float chartZoom = context.Options.Zoom;

            if (IsMouseOver)
               context.FillEllipse(StiMouseOverHelper.GetLineMouseOverColor(this.Marker.Brush), GetMouseOverRect(), null);
            
            var chart = this.Series.Chart as StiChart;

            var markerClone = (IStiMarker)this.Marker.Clone();

            var conditionVisible = this.series.ProcessSeriesMarkerVisible(index) || this.Marker.Visible;
            if (!IsTooltipMode || conditionVisible)
            {
                markerClone.Brush = this.series.ProcessSeriesBrushes(index, this.Marker.Brush);
                markerClone.Angle = this.series.ProcessSeriesMarkerAngle(index, this.Marker.Angle);
                markerClone.Type = this.series.ProcessSeriesMarkerType(index, this.Marker.Type);
                markerClone.Visible = conditionVisible;
            }

            if (IsTooltipMode && !conditionVisible)
            {
                markerClone.Brush = null;
                markerClone.BorderColor = Color.Transparent;
            }

            var interaction = chart.IsAnimation ? new StiInteractionDataGeom()
            {
                ComponentName = chart.Name,
                ComponentIndex = chart.Page != null ? chart.Page.Components.IndexOf(chart).ToString() : "",
                PageGuid = ((StiSeries)Series).DrillDownEnabled ? ((StiSeries)Series).DrillDownPageGuid : "",
                PageIndex = chart.Page != null ? chart.Page.Report.RenderedPages.IndexOf(chart.Page).ToString() : "",
                ElementIndex = this.ElementIndex.ToString(),
                InteractionData = this.Interaction
            } : null;

            this.Marker.Core.Draw(context, markerClone, this.Point, chartZoom, this.ShowShadow, this.IsMouseOver || this.Series.Core.IsMouseOver, isTooltipMode, chart.IsAnimation, this.GetToolTip(), this, interaction);
            
            context.PopSmoothingMode();
        }
        #endregion

        public StiMarkerGeom(IStiSeries series, int index, double value, PointF point, IStiMarker marker, bool showShadow, float zoom, bool isTooltipMode)
            : 
            base(StiMarkerCoreXF.GetMarkerRect(point, marker.Size, zoom))
        {
            this.series = series;
            this.index = index;
            this.value = value;
            this.point = point;
            this.marker = marker;
            this.showShadow = showShadow;
            this.isTooltipMode = isTooltipMode;
        }
    }
}
