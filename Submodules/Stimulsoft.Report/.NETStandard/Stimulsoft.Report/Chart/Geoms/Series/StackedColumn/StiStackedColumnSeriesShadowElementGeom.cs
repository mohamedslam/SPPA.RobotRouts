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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using System;

namespace Stimulsoft.Report.Chart
{
    public class StiStackedColumnSeriesShadowElementGeom : StiCellGeom
    {
        #region Properties
        /// <summary>
        /// Gets value which indicates that this geom object is inivisible 
        /// </summary>
        public override bool Invisible
        {
            get
            {
                return true;
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

        private bool isTopShadow;
        public bool IsTopShadow
        {
            get
            {
                return isTopShadow;
            }
        }

        private bool isBottomShadow;
        public bool IsBottomShadow
        {
            get
            {
                return isBottomShadow;
            }
        }

        public int Index { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            RectangleF shadowRect = this.ClientRectangle;

            #region Full Stacked
            if (this.Series is IStiFullStackedColumnSeries)
            {
                context.DrawCachedShadow(shadowRect, StiShadowSides.Right, context.Options.IsPrinting);
            }
            #endregion

            else
            {
                if (IsTopShadow)
                {
                    context.DrawCachedShadow(shadowRect,
                        StiShadowSides.Top |
                        StiShadowSides.Right,
                        context.Options.IsPrinting);
                }

                if (IsBottomShadow)
                {
                    context.DrawCachedShadow(shadowRect,
                        StiShadowSides.Right |
                        StiShadowSides.Edge |
                        StiShadowSides.Bottom |
                        StiShadowSides.Left,
                        context.Options.IsPrinting);

                }
            }
        }
        #endregion

        public StiStackedColumnSeriesShadowElementGeom(IStiSeries series, RectangleF clientRectangle, bool isTopShadow, bool isBottomShadow)
            : base(clientRectangle)
        {
            this.series = series;
            this.isTopShadow = isTopShadow;
            this.isBottomShadow = isBottomShadow;
        }
    }
}
