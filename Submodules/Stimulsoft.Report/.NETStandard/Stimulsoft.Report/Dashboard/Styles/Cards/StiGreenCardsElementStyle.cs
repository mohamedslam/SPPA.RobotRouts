﻿#region Copyright (C) 2003-2022 Stimulsoft
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
    public class StiGreenCardsElementStyle : StiCardsElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiGreenDashboardCardsStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "Green");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.Green;

        public override Color CellBackColor { get; set; } = Color.White;

        public override Color CellSparkline { get; set; } = StiColor.Get("70ad47");

        public override Color CellDataBarsOverlapped { get; set; } = StiColor.Get("74c93a");

        public override Color CellDataBarsPositive { get; set; } = StiColor.Get("70ad47");

        public override Color CellDataBarsNegative { get; set; } = StiColor.Get("dd5555");

        public override Color[] SeriesColors { get; set; } =
        {
            StiColor.Get("70ad47"),
            StiColor.Get("4472c4"),
            StiColor.Get("ffc000"),
            StiColor.Get("43682b"),
            StiColor.Get("fd6a37"),
            StiColor.Get("997300")
        };

        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.White);

        public override StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.DimGray);

        public override StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder(StiBorderSides.All, StiColor.Get("#8c8c8c"), 1, StiPenStyle.Solid);
        #endregion
    }
}