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
    public class StiStyleCoreXF14 : StiStyleCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
		/// </summary>
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}14";
        #endregion

        #region Properties
        public override Color[] StyleColors { get; } = 
        {
            StiColor.Get("#f0a22e"),
            StiColor.Get("#a5644e"),
            StiColor.Get("#b58b80"),
            StiColor.Get("#c3986d"),
            StiColor.Get("#a19574"),
            StiColor.Get("#c17529")
        };

        public override Color BasicStyleColor => StiColor.Get("#fbeec9");

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle14;
        #endregion
	}
}