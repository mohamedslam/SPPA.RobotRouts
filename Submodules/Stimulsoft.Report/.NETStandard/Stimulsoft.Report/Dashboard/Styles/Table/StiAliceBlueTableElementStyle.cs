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
    public class StiAliceBlueTableElementStyle : StiTableElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiAliceBlueDashboardTableStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "AliceBlue");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.AliceBlue;

        public override Color CellBackColor { get; set; } = StiColor.Get("f2f5fc");

        public override Color CellForeColor { get; set; } = StiColor.Get("1e1e1e");

        public override Color AlternatingCellBackColor { get; set; } = StiColor.Get("ccd5f0");

        public override Color AlternatingCellForeColor { get; set; } = StiColor.Get("1e1e1e");

        public override Color HeaderBackColor { get; set; } = StiColor.Get("5d6b99");

        public override Color HeaderForeColor { get; set; } = StiColor.Get("e4ffff");

        public override Color FooterBackColor { get; set; } = StiColor.Get("5d6b99");

        public override Color FooterForeColor { get; set; } = StiColor.Get("e4ffff");

        public override Color SelectedCellBackColor { get; set; } = StiColor.Get("f5cc84");

        public override Color SelectedCellForeColor { get; set; } = StiColor.Get("1e1e1e");

        public override Color HotHeaderBackColor { get; set; } = StiColorUtils.Light("5d6b99", 30);

        public override Color LineColor { get; set; } = StiColor.Get("c0cae9");

        public override Color BackColor { get; set; } = StiColor.Get("595b65");

        public override Color CellDataBarsOverlapped { get; set; } = StiColor.Get("40568d");

        public override Color CellDataBarsPositive { get; set; } = StiColor.Get("40568d");

        public override Color CellDataBarsNegative { get; set; } = StiColor.Get("d43642");

        public override Color CellWinLossPositive { get; set; } = StiColor.Get("40568d");

        public override Color CellSparkline { get; set; } = StiColor.Get("273946");

        public override Color CellIndicatorPositive { get; set; } = Color.LimeGreen;

        public override Color CellIndicatorNegative { get; set; } = StiColor.Get("dd5555");
        #endregion
    }
}