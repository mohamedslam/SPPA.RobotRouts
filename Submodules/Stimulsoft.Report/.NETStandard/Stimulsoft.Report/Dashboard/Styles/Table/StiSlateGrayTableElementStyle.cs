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
    public class StiSlateGrayTableElementStyle : StiTableElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiSlateGrayDashboardTableStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "SlateGray");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.SlateGray;

        public override Color CellBackColor { get; set; } = StiColor.Get("475b6f");

        public override Color CellForeColor { get; set; } = StiColor.Get("ddd");

        public override Color AlternatingCellBackColor { get; set; } = StiColor.Get("33475b");

        public override Color AlternatingCellForeColor { get; set; } = StiColor.Get("ddd");

        public override Color HeaderBackColor { get; set; } = StiColor.Get("33475b");

        public override Color HeaderForeColor { get; set; } = StiColor.Get("ddd");

        public override Color FooterBackColor { get; set; } = StiColor.Get("33475b");

        public override Color FooterForeColor { get; set; } = StiColor.Get("ddd");

        public override Color SelectedCellBackColor { get; set; } = StiColorUtils.Light("33475b", 50);

        public override Color SelectedCellForeColor { get; set; } = StiColor.Get("eee");

        public override Color HotHeaderBackColor { get; set; } = StiColorUtils.Light("33475b", 30);

        public override Color LineColor { get; set; } = StiColor.Get("c0c0c0");

        public override Color BackColor { get; set; } = StiColor.Get("33475b");
        #endregion
    }
}