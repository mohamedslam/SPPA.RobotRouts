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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Chart
{
    public class StiStyleCoreXF20 : StiStyleCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
        /// </summary>
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}20";
        #endregion

        #region Properties
        public override Color[] StyleColors { get; } = 
        {
            StiColor.Get("#d9bb94"),
            StiColor.Get("#f5be77"),
            StiColor.Get("#e6bd7f"),
            StiColor.Get("#dcd9d4"),
            StiColor.Get("#ede5d8"),
            StiColor.Get("#92e9d4")
        };

        public override Color BasicStyleColor => Color.Bisque;

        public override Color AxisLineColor => StiColorUtils.Dark(BasicStyleColor, 50);

        public override Color ChartAreaBorderColor => StiColorUtils.Dark(BasicStyleColor, 50);

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle20;
        #endregion

        #region Methods
        public override StiBrush GetColumnBrush(Color color)
        {
            return new StiSolidBrush(color);
        }
        #endregion
    }
}