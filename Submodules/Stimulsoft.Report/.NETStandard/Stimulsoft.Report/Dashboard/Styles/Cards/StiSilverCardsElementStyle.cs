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
    public class StiSilverCardsElementStyle : StiCardsElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiSilverDashboardTableStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "Silver");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.Silver;

        public override Color CellForeColor { get; set; } = StiColor.Get("e9f5fc");

        public override Color CellBackColor { get; set; } = StiColor.Get("6d7e8b");

        public override Color LineColor { get; set; } = StiColor.Get("66e9f5fc");

        public override Color CellDataBarsOverlapped { get; set; } = StiColor.Get("273946");

        public override Color CellDataBarsPositive { get; set; } = StiColor.Get("273946");

        public override Color CellDataBarsNegative { get; set; } = StiColor.Get("d43642");

        public override Color CellWinLossPositive { get; set; } = StiColor.Get("273946");

        public override Color CellSparkline { get; set; } = StiColor.Get("273946");

        public override Color CellIndicatorPositive { get; set; } = Color.LimeGreen;

        public override Color CellIndicatorNegative { get; set; } = StiColor.Get("dd5555");

        public override Color[] SeriesColors { get; set; } =
        {
            StiColor.Get("73829a"),
            StiColor.Get("343c49"),
            StiColor.Get("4f737a"),
            StiColor.Get("cfd3dd")
        };

        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(150, 67, 84, 94));
        #endregion
    }
}