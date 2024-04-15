#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using System.Drawing;

namespace Stimulsoft.Report.Dashboard.Styles.Cards
{
    public class StiAliceBlueCardsElementStyle : StiCardsElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiAliceBlueDashboardCardsStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "AliceBlue");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.AliceBlue;

        public override Color CellForeColor { get; set; } = StiColor.Get("1e1e1e");

        public override Color CellBackColor { get; set; } = StiColor.Get("f2f5fc");

        public override Color LineColor { get; set; } = StiColor.Get("c0cae9");

        public override Color CellDataBarsOverlapped { get; set; } = StiColor.Get("40568d");

        public override Color CellDataBarsPositive { get; set; } = StiColor.Get("40568d");

        public override Color CellDataBarsNegative { get; set; } = StiColor.Get("d43642");

        public override Color CellWinLossPositive { get; set; } = StiColor.Get("40568d");

        public override Color CellSparkline { get; set; } = StiColor.Get("273946");

        public override Color CellIndicatorPositive { get; set; } = Color.LimeGreen;

        public override Color CellIndicatorNegative { get; set; } = StiColor.Get("dd5555");

        public override Color[] SeriesColors { get; set; } =
        {
            StiColor.Get("4569bb"),
            StiColor.Get("e47334"),
            StiColor.Get("9d9c9c"),
            StiColor.Get("f8b92d"),
            StiColor.Get("5e93cc"),
            StiColor.Get("6ea548")
        };
        #endregion
    }
}
