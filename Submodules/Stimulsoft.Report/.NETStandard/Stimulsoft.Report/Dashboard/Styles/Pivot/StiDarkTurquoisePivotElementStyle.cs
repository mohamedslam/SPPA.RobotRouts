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
    public class StiDarkTurquoisePivotElementStyle : StiPivotElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDarkTurquoiseDashboardPivotStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "DarkTurquoise");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.DarkTurquoise;

        public override Color CellBackColor { get; set; } = StiColor.Get("235e6d");

        public override Color CellForeColor { get; set; } = StiColor.Get("fefdff");

        public override Color AlternatingCellBackColor { get; set; } = StiColor.Get("215667");

        public override Color AlternatingCellForeColor { get; set; } = StiColor.Get("fefdff");

        public override Color SelectedCellBackColor { get; set; } = StiColor.Get("f0621e");

        public override Color SelectedCellForeColor { get; set; } = StiColor.Get("1c445b");

        public override Color ColumnHeaderBackColor { get; set; } = StiColor.Get("2a7f9e");

        public override Color ColumnHeaderForeColor { get; set; } = StiColor.Get("fefdff");

        public override Color RowHeaderBackColor { get; set; } = StiColor.Get("2a7f9e");

        public override Color RowHeaderForeColor { get; set; } = StiColor.Get("fefdff");

        public override Color HotColumnHeaderBackColor { get; set; } = StiColor.Get("f0621e");

        public override Color HotRowHeaderBackColor { get; set; } = StiColor.Get("f0621e");

        public override Color LineColor { get; set; } = StiColor.Get("5da0b7");

        public override Color BackColor { get; set; } = StiColor.Get("235e6d");
        #endregion
    }
}