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
    public class StiStyleCoreXF15 : StiStyleCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
        /// </summary>
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}15";
        #endregion

        #region Properties
        public override Color[] StyleColors { get; } = 
        {
            StiColor.Get("#0f6fc6"),
            StiColor.Get("#009dd9"),
            StiColor.Get("#0bd0d9"),
            StiColor.Get("#10cf9b"),
            StiColor.Get("#7cca62"),
            StiColor.Get("#a5c649")
        };

        public override Color BasicStyleColor => StiColor.Get("#dbf5f9");

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle15;
        #endregion
    }
}