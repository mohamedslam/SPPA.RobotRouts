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
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiLegendGeom : StiCellGeom
    {
        #region Properties
        private IStiLegend legend;
        public IStiLegend Legend
        {
            get
            {
                return legend;
            }
        }

        private List<StiLegendItemCoreXF> seriesItems;
        public List<StiLegendItemCoreXF> SeriesItems
        {
            get
            {
                return seriesItems;
            }
        }

        private StiLegendTitleGeom legendTitleGeom;
        public StiLegendTitleGeom LegendTitleGeom
        {
            get
            {
                return legendTitleGeom;
            }
            set
            {
                legendTitleGeom = value;
            }
        }
        #endregion

        #region Methods

        public override void Dispose()
        {
            base.Dispose();
            
            seriesItems.Clear();
            seriesItems = null;
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            if ((!legend.Visible) || seriesItems.Count == 0) return;

            RectangleF rect = ClientRectangle;
            if (this.LegendTitleGeom != null)
            {
                rect.Y += this.LegendTitleGeom.ClientRectangle.Height;
                rect.Height -= this.LegendTitleGeom.ClientRectangle.Height;
            }

            if (rect.Width > 0 && rect.Height > 0)
            {
                #region Draw Shadow
                if (legend.ShowShadow)
                {
                    if (((StiChart)this.Legend.Chart).IsAnimation)
                    {
                        var animation = new StiOpacityAnimation(TimeSpan.Zero, TimeSpan.Zero);
                        context.DrawShadowRect(rect, 6, null, animation);
                    }
                    else
                        context.DrawCachedShadow(rect, StiShadowSides.All, context.Options.IsPrinting);
                }
                #endregion

                #region Fill rectangle
                context.FillRectangle(legend.Brush, rect.X, rect.Y, rect.Width, rect.Height, null);
                #endregion

                #region Draw Border
                StiPenGeom pen = new StiPenGeom(legend.BorderColor);
                context.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                #endregion
            }
        }
        #endregion

        public StiLegendGeom(IStiLegend legend, RectangleF clientRectangle, List<StiLegendItemCoreXF> seriesItems)
            : base(clientRectangle)
        {
            this.legend = legend;
            this.seriesItems = seriesItems;
        }
    }
}
