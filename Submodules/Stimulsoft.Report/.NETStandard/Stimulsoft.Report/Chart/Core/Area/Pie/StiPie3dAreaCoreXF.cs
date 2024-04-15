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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Localization;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiPie3dAreaCoreXF : StiPieAreaCoreXF
    {
        #region Methods
        public override StiCellGeom Render(StiContext context, RectangleF rect)
        {
            PrepareInfo(rect);
            var areaGeom = new StiPieAreaGeom(this.Area, rect);

            #region Draw Series
            var seriesCollection = GetSeries();
            RenderSeries(context, rect, areaGeom, seriesCollection);
            #endregion

            return areaGeom;
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
                return $"{StiLocalization.Get("Chart", "Pie")} 3D";
            }
        }
        #endregion

        #region Properties.Settings
        public override int Position
        {
            get
            {
                return (int)StiChartAreaPosition.Pie3d;
            }
        }
        #endregion

        public StiPie3dAreaCoreXF(IStiArea area)
               : base(area)
        {
        }
    }
}
