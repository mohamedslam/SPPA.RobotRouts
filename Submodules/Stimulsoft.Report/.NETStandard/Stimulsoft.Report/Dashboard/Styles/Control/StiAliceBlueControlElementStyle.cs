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
    public class StiAliceBlueControlElementStyle : StiControlElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiAliceBlueDashboardControlStyle;
        #endregion

        #region Properties
        public override string LocalizedName => Loc.Get("PropertyColor", "AliceBlue");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.AliceBlue;

        public override Color BackColor { get; set; } = StiColor.Get("f2f5fc");

        public override Color ForeColor { get; set; } = StiColor.Get("1e1e1e");

        public override Color GlyphColor { get; set; } = StiColor.Get("1e1e1e");

        public override Color SeparatorColor { get; set; } = StiColor.Get("c0cae9");

        public override Color SelectedBackColor { get; set; } = StiColor.Get("40568d");

        public override Color SelectedForeColor { get; set; } = StiColor.Get("e4ffff");

        public override Color SelectedGlyphColor { get; set; } = StiColor.Get("e4ffff");

        public override Color HotBackColor { get; set; } = StiColor.Get("f5cc84");

        public override Color HotForeColor { get; set; } = StiColor.Get("1e1e1e");

        public override Color HotGlyphColor { get; set; } = StiColor.Get("1e1e1e");

        public override Color HotSelectedBackColor { get; set; } = StiColor.Get("5d6b99");

        public override Color HotSelectedForeColor { get; set; } = StiColor.Get("e4ffff");

        public override Color HotSelectedGlyphColor { get; set; } = StiColor.Get("e4ffff");
        #endregion
    }
}