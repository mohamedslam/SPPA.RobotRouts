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

namespace Stimulsoft.Report.Dashboard.Styles
{
    public class StiCustomPivotElementStyle : StiPivotElementStyle
    {
        #region Fields
        private string name;
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCustomDashboardPivotStyle;
        #endregion

        #region Properties
        public override string LocalizedName => this.name;

        public override StiElementStyleIdent Ident => StiElementStyleIdent.Custom;

        public override Color CellBackColor { get; set; }

        public override Color AlternatingCellBackColor { get; set; }

        public override Color SelectedCellBackColor { get; set; }

        public override Color SelectedCellForeColor { get; set; }

        public override Color ColumnHeaderBackColor { get; set; }

        public override Color ColumnHeaderForeColor { get; set; }

        public override Color RowHeaderBackColor { get; set; }

        public override Color RowHeaderForeColor { get; set; }

        public override Color HotColumnHeaderBackColor { get; set; }

        public override Color HotRowHeaderBackColor { get; set; }

        public override Color CellForeColor { get; set; }

        public override Color LineColor { get; set; }
        #endregion

        public StiCustomPivotElementStyle(StiCrossTabStyle style)
        {
            this.name = style.Name;
            this.BackColor = style.BackColor;
            this.AlternatingCellForeColor = style.AlternatingCellForeColor;
            this.AlternatingCellBackColor = style.AlternatingCellBackColor;
            this.CellBackColor = style.CellBackColor;
            this.CellForeColor = style.CellForeColor;
            this.ColumnHeaderBackColor = style.ColumnHeaderBackColor;
            this.ColumnHeaderForeColor = style.ColumnHeaderForeColor;
            this.HotColumnHeaderBackColor = style.HotColumnHeaderBackColor;
            this.HotRowHeaderBackColor = style.HotRowHeaderBackColor;
            this.RowHeaderBackColor = style.RowHeaderBackColor;
            this.RowHeaderForeColor = style.RowHeaderForeColor;
            this.SelectedCellBackColor = style.SelectedCellBackColor;
            this.SelectedCellForeColor = style.SelectedCellForeColor;
            this.LineColor = style.LineColor;
        }
    }
}