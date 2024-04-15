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
    public class StiDarkGrayPivotElementStyle : StiPivotElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDarkGrayDashboardPivotStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "DarkGray");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.DarkGray;

        public override Color CellBackColor { get; set; } = StiColor.Get("595b65");

        public override Color CellForeColor { get; set; } = StiColor.Get("ddd");

        public override Color AlternatingCellBackColor { get; set; } = StiColor.Get("6b6e75");

        public override Color AlternatingCellForeColor { get; set; } = StiColor.Get("ddd");

        public override Color SelectedCellBackColor { get; set; } = StiColor.Get("db7b46");

        public override Color SelectedCellForeColor { get; set; } = StiColor.Get("eee");

        public override Color ColumnHeaderBackColor { get; set; } = StiColor.Get("6b6e75");

        public override Color ColumnHeaderForeColor { get; set; } = StiColor.Get("ddd");

        public override Color RowHeaderBackColor { get; set; } = StiColor.Get("6b6e75");

        public override Color RowHeaderForeColor { get; set; } = StiColor.Get("ddd");

        public override Color HotColumnHeaderBackColor { get; set; } = StiColorUtils.Light("db7b46", 30);

        public override Color HotRowHeaderBackColor { get; set; } = StiColorUtils.Light("db7b46", 30);

        public override Color LineColor { get; set; } = StiColor.Get("66d3d3d5");

        public override Color BackColor { get; set; } = StiColor.Get("595b65");
        #endregion
    }
}