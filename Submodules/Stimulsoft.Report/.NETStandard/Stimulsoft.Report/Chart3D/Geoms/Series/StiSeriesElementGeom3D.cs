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

using Stimulsoft.Base.Context;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiSeriesElementGeom3D : 
        StiGeom3D,
        IStiSeriesElement
    {
        #region IStiGeomInteraction override
        public override void InvokeMouseEnter(StiInteractionOptions options)
        {
            if (!AllowMouseOver) return;

            if (!IsMouseOver)
            {
                IsMouseOver = true;
                options.UpdateContext = Series.Interaction.DrillDownEnabled;
            }

            var valueIndex = GetValueIndex();

            options.SeriesInteractionData = Interaction;
            options.InteractionToolTip = GetToolTip(valueIndex);
            options.InteractionHyperlink = GetHyperlink(valueIndex);
            options.InteractionToolTipPoint = GetToolTipPoint();
        }

        public override void InvokeMouseLeave(StiInteractionOptions options)
        {
            if (!AllowMouseOver) return;
            if (!IsMouseOver) return;

            IsMouseOver = false;
            options.UpdateContext = Series.Interaction.DrillDownEnabled;
            options.SeriesInteractionData = Interaction;
        }

        public override void InvokeClick(StiInteractionOptions options)
        {
            int valueIndex = GetValueIndex();

            if (Series.Hyperlinks != null && valueIndex < Series.Hyperlinks.Length)
                options.InteractionHyperlink = Series.Hyperlinks[valueIndex];

            if (Series.Interaction.DrillDownEnabled)
            {
                options.SeriesInteractionData = Interaction;

                IsMouseOver = false;
                options.UpdateContext = Series.Interaction.DrillDownEnabled;
            }

            if (options.SelectOnClick)
                this.IsSelected = !this.IsSelected;
        }

        private int GetValueIndex()
        {
            int valueIndex = this.Index;

            //if (this.Series is IStiClusteredBarSeries && !(this.Series is IStiGanttSeries) ||
            //    this.Series is IStiStackedBarSeries ||
            //    this.Series is IStiFullStackedBarSeries)
            //{
            //    if (this.Series.Chart.Area is IStiAxisArea && !((IStiAxisArea)this.Series.Chart.Area).ReverseVert)
            //        valueIndex = Series.Values.Length - valueIndex - 1;
            //}
            //else
            //{
            //    if (this.Series.Chart.Area is IStiAxisArea && ((IStiAxisArea)this.Series.Chart.Area).ReverseHor)
            //        valueIndex = Series.Values.Length - valueIndex - 1;
            //}

            return valueIndex;
        }

        internal string GetHyperlink()
        {
            return GetHyperlink(GetValueIndex());
        }

        private string GetHyperlink(int valueIndex)
        {
            if (Series.Hyperlinks != null && valueIndex < Series.Hyperlinks.Length)
                return Series.Hyperlinks[valueIndex];
            else
                return null;
        }
        internal override string GetToolTip()
        {
            var valueIndex = GetValueIndex();

            return GetToolTip(valueIndex);
        }

        protected string GetToolTip(int valueIndex)
        {
            string textTooltip = null;

            if (Series.ToolTips != null && valueIndex < Series.ToolTips.Length)
                textTooltip = Series.ToolTips[valueIndex];

            if (!string.IsNullOrEmpty(textTooltip) && textTooltip.Contains("\"StiColor\":\"#ffffff\""))
            {
                textTooltip = textTooltip.Replace("\"StiColor\":\"#ffffff\"", $"\"StiColor\":\"{ColorTranslator.ToHtml(this.SeriesColor)}\"");
            }

            return textTooltip;
        }
        public virtual StiInteractionToolTipPointOptions GetToolTipPoint()
        {
            return null;
        }

        public virtual bool AllowMouseOver
        {
            get
            {
                var index = GetValueIndex();
                return
                    (Series.Hyperlinks != null && index < Series.Hyperlinks.Length) ||
                    (Series.ToolTips != null && index < Series.ToolTips.Length) ||
                    (this.Series.Interaction.DrillDownEnabled && this.Series.Interaction.AllowSeriesElements);
            }
        }

        public virtual bool IsMouseOver
        {
            get
            {
                return this.Series.Core.GetIsMouseOverSeriesElement(this.Index);
            }
            set
            {
                this.Series.Core.SetIsMouseOverSeriesElement(this.Index, value);
            }
        }

        public bool IsSelected
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
        public Color SeriesColor { get; set; }

        public double Value { get; }

        public int Index { get; }

        public IStiSeries Series { get; }

        public virtual StiSeriesInteractionData Interaction { get; set; }

        public string ElementIndex { get; set; }    
        #endregion

        #region Methods
        public override bool Contains(float x, float y)
        {
            var points = GetPoints();
            var wrappingPoints = StiComputationalGeometryHelper.ConvexHull(points);

            return StiComputationalGeometryHelper.IsPointInPolygon(wrappingPoints, new PointF(x, y));
        }

        public string GetHashKey()
        {
            if (this.Interaction == null)
                return null;

            return $"{this.Interaction.Argument}-{this.Interaction.Value.GetValueOrDefault()}-{this.Series.CoreTitle}";
        }
        #endregion

        public StiSeriesElementGeom3D(StiRender3D render, double value, int index, IStiSeries series, Color color)
            : base(render)
        {
            this.Series = series;
            this.Value = value;
            this.Index = index;
            this.SeriesColor = color;
        }
    }
}
