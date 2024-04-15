#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
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
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiDoughnutAreaCoreXF : StiPieAreaCoreXF
    {
        #region Methods
        public override StiCellGeom Render(StiContext context, RectangleF rect)
        {
            PrepareInfo(rect);
            var doughnutAreaGeom = new StiDoughnutAreaGeom(this.Area, rect);

            #region Draw Series
            var seriesCollection = GetSeries();
            RenderSeries(context, rect, doughnutAreaGeom, seriesCollection);
            #endregion

            return doughnutAreaGeom;
        }
        #endregion        

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "Doughnut");
            }
        }
        #endregion

        #region Properties.Settings
        public override int Position
        {
            get
            {
                return (int)StiChartAreaPosition.Doughnut;
            }
        }
        #endregion

        public StiDoughnutAreaCoreXF(IStiArea area)
            : base(area)
        {            
        }
	}
}
