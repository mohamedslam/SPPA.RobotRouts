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
    public class StiDarkBlueTableElementStyle : StiTableElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDarkBlueDashboardTableStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "DarkBlue");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.DarkBlue;

        public override Color CellBackColor { get; set; } = StiColor.Get("1f4265");

        public override Color CellForeColor { get; set; } = StiColor.Get("ddd");

        public override Color AlternatingCellBackColor { get; set; } = StiColor.Get("0a325a");

        public override Color AlternatingCellForeColor { get; set; } = StiColor.Get("ddd");

        public override Color HeaderBackColor { get; set; } = StiColor.Get("0a325a");

        public override Color HeaderForeColor { get; set; } = StiColor.Get("ddd");

        public override Color FooterBackColor { get; set; } = StiColor.Get("0a325a");

        public override Color FooterForeColor { get; set; } = StiColor.Get("ddd");

        public override Color SelectedCellBackColor { get; set; } = StiColorUtils.Light("0a325a", 50);

        public override Color SelectedCellForeColor { get; set; } = StiColor.Get("eee");

        public override Color HotHeaderBackColor { get; set; } = StiColorUtils.Light("0a325a", 30);

        public override Color LineColor => StiColor.Get("758696");

        public override Color BackColor { get; set; } = StiColor.Get("0a325a");
        #endregion
    }
}