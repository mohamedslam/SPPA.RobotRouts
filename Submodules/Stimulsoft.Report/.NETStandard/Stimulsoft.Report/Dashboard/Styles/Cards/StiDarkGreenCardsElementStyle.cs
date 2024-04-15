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

namespace Stimulsoft.Report.Dashboard.Styles
{
    public class StiDarkGreenCardsElementStyle : StiCardsElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDarkGreenDashboardCardsStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "DarkGreen");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.DarkGreen;

        public override Color CellForeColor { get; set; } = StiColor.Get("f6f3f5");

        public override Color CellBackColor { get; set; } = StiColor.Get("3f745e");

        public override Color BackColor { get; set; } = StiColor.Get("595b65");

        public override Color LineColor { get; set; } = StiColor.Get("66d3d3d5");

        public override Color CellDataBarsOverlapped { get; set; } = StiColor.Get("db7b46");

        public override Color CellDataBarsPositive { get; set; } = StiColor.Get("db7b46");

        public override Color CellDataBarsNegative { get; set; } = StiColor.Get("dd5555");

        public override Color CellWinLossPositive { get; set; } = StiColor.Get("db7b46");

        public override Color CellSparkline { get; set; } = StiColor.Get("db7b46");

        public override Color CellIndicatorPositive { get; set; } = Color.LimeGreen;

        public override Color CellIndicatorNegative { get; set; } = StiColor.Get("dd5555");

        public override Color[] SeriesColors { get; set; } =
        {
            StiColor.Get("ecb92f"),
            StiColor.Get("d66153"),
            StiColor.Get("319491"),
            StiColor.Get("f7f7f7"),
            StiColor.Get("dd7c21"),
        };
        #endregion
    }
}