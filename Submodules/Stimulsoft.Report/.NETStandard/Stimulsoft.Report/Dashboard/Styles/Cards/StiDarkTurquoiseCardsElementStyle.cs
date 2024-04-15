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
    public class StiDarkTurquoiseCardsElementStyle : StiCardsElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDarkTurquoiseDashboardTableStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "DarkTurquoise");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.DarkTurquoise;

        public override Color CellForeColor { get; set; } = StiColor.Get("fefdff");

        public override Color CellBackColor { get; set; } = StiColor.Get("235e6d");

        public override Color BackColor { get; set; } = StiColor.Get("235e6d");

        public override Color LineColor { get; set; } = StiColor.Get("5da0b7");

        public override Color CellDataBarsOverlapped { get; set; } = StiColor.Get("db7b46");

        public override Color CellDataBarsPositive { get; set; } = StiColor.Get("5daac1");

        public override Color CellDataBarsNegative { get; set; } = StiColor.Get("dd5555");

        public override Color CellWinLossPositive { get; set; } = StiColor.Get("db7b46");

        public override Color CellSparkline { get; set; } = StiColor.Get("db7b46");

        public override Color CellIndicatorPositive { get; set; } = Color.LimeGreen;

        public override Color CellIndicatorNegative { get; set; } = StiColor.Get("dd5555");

        public override Color[] SeriesColors { get; set; } =
        {
            StiColor.Get("99a8d7e2"),
            StiColor.Get("995ea8bf"),
            StiColor.Get("992b7f9e")
        };

        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(150, 30, 74, 97));

        public override StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.White);

        public override StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder(StiBorderSides.All, Color.White, 1, StiPenStyle.Solid);
        #endregion
    }
}