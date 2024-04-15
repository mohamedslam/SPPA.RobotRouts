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
    public class StiSiennaCardsElementStyle : StiCardsElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiSiennaDashboardCardsStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "Sienna");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.Sienna;

        public override Color CellBackColor { get; set; } = StiColor.Get("#eee8dc");

        public override Color CellForeColor { get; set; } = StiColor.Get("#270e09");

        public override Color LineColor { get; set; } = StiColor.Get("#c9c2b2");

        public override Color CellDataBarsOverlapped { get; set; } = StiColor.Get("#d8834d");

        public override Color CellDataBarsPositive { get; set; } = StiColor.Get("#8a693c");

        public override Color CellDataBarsNegative { get; set; } = StiColor.Get("#e83437");

        public override Color CellWinLossPositive { get; set; } = StiColor.Get("#8a693c");

        public override Color CellSparkline { get; set; } = StiColor.Get("#906e3e");

        public override Color CellIndicatorPositive { get; set; } = StiColor.Get("#8a693c");

        public override Color CellIndicatorNegative { get; set; } = StiColor.Get("#e83437");

        public override Color[] SeriesColors { get; set; } =
        {
            StiColor.Get("794d26"),
            StiColor.Get("c7986a"),
            StiColor.Get("c4b49a"),
            StiColor.Get("894d29"),
            StiColor.Get("422515"),
            StiColor.Get("564438"),
            StiColor.Get("876c57"),
            StiColor.Get("d8814b"),
            StiColor.Get("532525"),
            StiColor.Get("59413f"),
        };

        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(150, 66, 37, 21));

        public override StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.White);

        public override StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder(StiBorderSides.None, Color.White, 1, StiPenStyle.Solid);
        #endregion
    }
}
