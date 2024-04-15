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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Helpers;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiLegendItemGeom : StiCellGeom
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

        public override void InvokeClick(StiInteractionOptions options)
        {
            if (this.Item.Series.Interaction != null && 
                this.Item.Series.Interaction.DrillDownEnabled &&
                this.Item.Series.Interaction.AllowSeries)
            {
                var data = new StiSeriesInteractionData();
                
                if (IsColorEach)
                {
                    data.Fill(this.Item.Series.Chart.Area, this.Item.Series, this.Item.Index);
                    data.IsElements = true;
                }
                else
                {
                    data.Series = this.Item.Series;
                    data.IsElements = false;
                }
                
                IsMouseOver = false;
                options.UpdateContext = true;
                options.SeriesInteractionData = data;

                if (IsColorEach && options.SelectOnClick)
                    this.IsSelected = !this.IsSelected;
            }
        }

        public virtual bool AllowMouseOver
        {
            get
            {
                return 
                    this.Item != null && 
                    this.Item.Series != null && 
                    this.Item.Series.Interaction != null &&
                    this.Item.Series.Interaction.AllowSeries &&
                    this.Item.Series.Interaction.DrillDownEnabled;
            }
        }

        private bool IsColorEach
        {
            get
            {
                return this.Item.Series.Chart.Area.ColorEach;
            }
        }

        public virtual bool IsMouseOver
        {
            get
            {
                if (IsColorEach)
                {
                    if (this.Item.Index == -1)
                        return false;
                    return this.Item.Series.Core.GetIsMouseOverSeriesElement(this.Item.Index);
                }
                return this.Item.Series.Core.IsMouseOver;                
            }
            set
            {
                if (IsColorEach)
                {
                    if (this.Item.Index != -1)
                        this.Item.Series.Core.SetIsMouseOverSeriesElement(this.Item.Index, value);                    
                }
                else this.Item.Series.Core.IsMouseOver = value;
            }
        }

        internal bool IsSelected
        {
            get
            {
                if (!((StiChart)this.Item.Series.Chart).IsDashboard) return false;

                return ((StiChart)this.Item.Series.Chart).Core.GetIsSelectedSeriesElement($"legend{this.Item.Index}");
            }
            set
            {
                ((StiChart)this.Item.Series.Chart).Core.SetIsSelectedSeriesElement($"legend{this.Item.Index}", value);
            }
        }
        #endregion

        #region Properties
        private IStiLegend legend;
        public IStiLegend Legend
        {
            get
            {
                return legend;
            }
        }

        private StiLegendItemCoreXF item;
        public StiLegendItemCoreXF Item
        {
            get
            {
                return item;
            }
        }

        private int colorIndex;
        public int ColorIndex
        {
            get
            {
                return colorIndex;
            }
        }

        private int legendItemsCount;
        public int LegendItemsCount
        {
            get
            {
                return legendItemsCount;
            }
        }

        private int legendItemIndex;
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {            
            #region Marker
            var textRect = ClientRectangle;
            
            if (legend.MarkerVisible)
            {
                var markerRect = new RectangleF(
                    ClientRectangle.X, 
                    ClientRectangle.Y + (ClientRectangle.Height - legend.MarkerSize.Height * context.Options.Zoom) / 2,
                    legend.MarkerSize.Width * context.Options.Zoom,
                    legend.MarkerSize.Height * context.Options.Zoom);

                if (legend.MarkerAlignment == StiMarkerAlignment.Right)
                    markerRect.X = ClientRectangle.Right - markerRect.Width;

                #region Draw Marker
                var legendMarker = StiMarkerLegendFactory.CreateMarker(Item.Series);

                if (markerRect.Width > 0 && markerRect.Height > 0)
                    legendMarker.Draw(context, Item.Series, markerRect, this.ColorIndex, this.LegendItemsCount, this.legendItemIndex);
                #endregion

                if (legend.MarkerAlignment == StiMarkerAlignment.Left) textRect.X += legend.MarkerSize.Width * context.Options.Zoom;
                textRect.Width -= legend.MarkerSize.Width * context.Options.Zoom;
            }
            #endregion

            #region Draw series name
            textRect.X += 2;
            textRect.Width += 4;

            var brush = new StiSolidBrush(Legend.LabelsColor);
            var newFont = StiFontGeom.ChangeFontSize(Legend.Font, Legend.Font.Size * context.Options.Zoom);

            var sf = context.GetDefaultStringFormat();

            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            sf.FormatFlags = (StringFormatFlags)0;
            if (legend.MarkerAlignment == StiMarkerAlignment.Right)
            {
                sf.FormatFlags = StringFormatFlags.NoWrap;
            }
            context.DrawString(Item.GetText(context, newFont), newFont, brush, textRect, sf, this.legendItemIndex);
            #endregion            
        }
        #endregion

        public StiLegendItemGeom(IStiLegend legend, StiLegendItemCoreXF item, RectangleF clientRectangle, int colorIndex, int legendItemsCount, int legendItemIndex)
            : base(clientRectangle)
        {
            this.legend = legend;
            this.item = item;
            this.colorIndex = colorIndex;
            this.legendItemsCount = legendItemsCount;
            this.legendItemIndex = legendItemIndex;
        }
    }
}
