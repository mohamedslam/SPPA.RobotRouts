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

using System.Drawing;

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Dashboard.Styles
{
    public class StiSlateGrayCardsElementStyle : StiCardsElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiSlateGrayDashboardCardsStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "SlateGray");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.SlateGray;

        public override Color CellForeColor { get; set; } = StiColor.Get("ddd");

        public override Color CellBackColor { get; set; } = StiColor.Get("475b6f");

        public override Color BackColor { get; set; } = StiColor.Get("33475b");

        public override Color LineColor { get; set; } = StiColor.Get("c0c0c0");

        public override Color[] SeriesColors { get; set; } =
        {
            StiColor.Get("0bac45"),
            StiColor.Get("ec334d"),
            StiColor.Get("a1ae94"),
            StiColor.Get("ed7d31"),
            StiColor.Get("5ab0ee"),
            StiColor.Get("3dde77")
        };

        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(180, 50, 50, 50));

        public override StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.White);

        public override StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder(StiBorderSides.All, Color.White, 1, StiPenStyle.Solid);
        #endregion
    }
}