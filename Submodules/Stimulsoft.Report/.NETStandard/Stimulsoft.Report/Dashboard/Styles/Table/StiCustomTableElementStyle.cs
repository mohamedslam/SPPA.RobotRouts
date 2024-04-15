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

namespace Stimulsoft.Report.Dashboard.Styles
{
    public class StiCustomTableElementStyle : StiTableElementStyle
    {
        #region Fields
        private string name;
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCustomDashboardTableStyle;
        #endregion

        #region Properties
        public override string LocalizedName => this.name;

        public override StiElementStyleIdent Ident => StiElementStyleIdent.Blue;
        #endregion

        public StiCustomTableElementStyle(StiTableStyle style)
        {
            this.name = style.Name;
            this.BackColor = style.BackColor;
            this.AlternatingCellBackColor = style.AlternatingDataColor;
            this.AlternatingCellForeColor = style.AlternatingDataForeground;
            this.CellBackColor = style.DataColor;
            this.CellForeColor = style.DataForeground;
            this.LineColor = style.GridColor;
            this.HeaderBackColor = style.HeaderColor;
            this.HeaderForeColor = style.HeaderForeground;
            this.HotHeaderBackColor = style.HotHeaderColor;
            this.SelectedCellBackColor = style.SelectedDataColor;
            this.SelectedCellForeColor = style.SelectedDataForeground;
            this.FooterBackColor = style.FooterColor;
            this.FooterForeColor = style.FooterForeground;
        }
    }
}