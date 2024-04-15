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
    public class StiStyleCoreXF19 : StiStyleCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
		/// </summary>
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}19";
        #endregion

        #region Properties
        public override Color[] StyleColors { get; } = 
        {
            StiColor.Get("#f2eadd"),
            StiColor.Get("#f0ede8"),
            StiColor.Get("#e8ddcb"),
            StiColor.Get("#e1c9ad"),
            StiColor.Get("#d8b48c")
        };

        public override Color BasicStyleColor => StiColor.Get("#f2eadd");

        public override StiBrush InterlacingHorBrush => new StiSolidBrush("#f0ede8");

        public override StiBrush InterlacingVertBrush => new StiSolidBrush("#f0ede8");

        public override StiBrush ChartAreaBrush => new StiSolidBrush("#f0ede8");

        public override StiBrush ChartBrush => new StiSolidBrush(Color.White);

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle19;
        #endregion

        #region Methods
        public override StiBrush GetColumnBrush(Color color)
        {
            return new StiSolidBrush(color);
        }
        #endregion
	}
}