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
    public class StiDarkGreenIndicatorElementStyle : StiIndicatorElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDarkGreenDashboardIndicatorStyle;
        #endregion

        #region Properties
        public override string LocalizedName => Loc.Get("PropertyColor", "DarkGreen");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.DarkGreen;

        public override Color GlyphColor { get; set; } = StiColor.Get("e1bb41");

        public override Color BackColor { get; set; } = StiColor.Get("3f745e");

        public override Color PositiveColor { get; set; } = StiColor.Get("e2ba41");

        public override Color NegativeColor { get; set; } = StiColor.Get("c76858");

        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(150, 20, 75, 69));

        public override StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.White);

        public override StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder(StiBorderSides.None, Color.White, 1, StiPenStyle.Solid);
        #endregion
    }
}