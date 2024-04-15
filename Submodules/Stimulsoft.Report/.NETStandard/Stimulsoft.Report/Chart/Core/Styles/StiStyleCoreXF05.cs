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
    public class StiStyleCoreXF05 : StiStyleCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
		/// </summary>
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}05";
        #endregion

        #region Properties
        public override Color[] StyleColors { get; } = 
        {
            StiColor.Get("E0A93D"),
            StiColor.Get("B4B500"),
            StiColor.Get("0E980D"),
            StiColor.Get("AA4824"),
            StiColor.Get("85DD4D"),
            StiColor.Get("E6C745"),
            StiColor.Get("DB6434"),
            StiColor.Get("83BE2F")
        };

        public override Color BasicStyleColor => Color.LemonChiffon;

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle05;
        #endregion
	}
}