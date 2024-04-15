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
using System.Collections.Generic;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiSeriesGeom : StiCellGeom
    {
        #region Properties
        private IStiSeries series;
        public IStiSeries Series
        {
            get
            {
                return series;
            }
        }

        private List<StiSeriesInteractionData> interactions;
        public List<StiSeriesInteractionData> Interactions
        {
            get
            {
                return interactions;
            }
            set
            {
                interactions = value;
            }
        }

        private StiAreaGeom areaGeom;
        public StiAreaGeom AreaGeom
        {
            get
            {
                return areaGeom;
            }
            set
            {
                areaGeom = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            RectangleF rect = this.ClientRectangle;
            //Red line
            //context.DrawRectangle(new StiPenGeom(Color.Blue), rect.X, rect.Y, rect.Width, rect.Height);
        }
        #endregion

        public StiSeriesGeom(StiAreaGeom areaGeom, IStiSeries series, RectangleF clientRectangle)
            : base(clientRectangle)
        {
            this.areaGeom = areaGeom;
            this.series = series;
        }
    }
}
