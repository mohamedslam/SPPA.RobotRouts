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
    public class StiDarkTurquoiseControlElementStyle : StiControlElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDarkTurquoiseDashboardControlStyle;
        #endregion

        #region Properties
        public override string LocalizedName => Loc.Get("PropertyColor", "DarkTurquoise");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.DarkTurquoise;

        public override Color BackColor { get; set; } = StiColor.Get("235e6d");

        public override Color ForeColor { get; set; } = StiColor.Get("fbffff");

        public override Color SelectedBackColor { get; set; } = StiColor.Get("f0621e");

        public override Color SelectedForeColor { get; set; } = StiColor.Get("eee");

        public override Color GlyphColor { get; set; } = StiColor.Get("fbffff");

        public override Color SeparatorColor { get; set; } = StiColor.Get("aed2e5");

        public override Color SelectedGlyphColor { get; set; } = StiColor.Get("fbffff");

        public override Color HotBackColor { get; set; } = StiColorUtils.Dark("f0621e", 10);

        public override Color HotForeColor { get; set; } = StiColor.Get("fbffff");

        public override Color HotGlyphColor { get; set; } = StiColor.Get("fbffff");

        public override Color HotSelectedBackColor { get; set; } = StiColorUtils.Light("f0621e", 50);

        public override Color HotSelectedForeColor { get; set; } = StiColor.Get("fbffff");

        public override Color HotSelectedGlyphColor { get; set; } = StiColor.Get("fbffff");
        #endregion
    }
}