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
    public class StiStackedBarSeriesShadowElementGeom : StiCellGeom
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

        private bool isLeftShadow;
        public bool IsLeftShadow
        {
            get
            {
                return isLeftShadow;
            }
        }

        private bool isRightShadow;
        public bool IsRightShadow
        {
            get
            {
                return isRightShadow;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            RectangleF shadowRect = this.ClientRectangle;

            #region Full Stacked
            if (this.Series is IStiFullStackedBarSeries)
            {
                context.DrawCachedShadow(shadowRect, StiShadowSides.Bottom, context.Options.IsPrinting);
            }
            #endregion

            else
            {

                if (IsLeftShadow)
                {
                    context.DrawCachedShadow(shadowRect,
                                    StiShadowSides.Bottom |
                                    StiShadowSides.Left,
                                    context.Options.IsPrinting);
                }

                if (IsRightShadow)
                {
                    context.DrawCachedShadow(shadowRect,
                                    StiShadowSides.Top |
                                    StiShadowSides.Right |
                                    StiShadowSides.Edge |
                                    StiShadowSides.Bottom,
                                    context.Options.IsPrinting);
                }
            }
        }
        #endregion

        public StiStackedBarSeriesShadowElementGeom(IStiSeries series, RectangleF clientRectangle, bool isLeftShadow, bool isRightShadow)
            : base(clientRectangle)
        {
            this.series = series;
            this.isLeftShadow = isLeftShadow;
            this.isRightShadow = isRightShadow;
        }
    }
}
