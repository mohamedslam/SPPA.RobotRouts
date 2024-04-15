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
using Stimulsoft.Report.Styles;
using System.Drawing;

namespace Stimulsoft.Report.Dashboard.Styles
{
    public class StiCustomCardsElementStyle : StiCardsElementStyle
    {
        #region Fields
        private string name;
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCustomDashboardCardsStyle;
        #endregion

        #region Properties
        public override string LocalizedName => this.name;

        public override StiElementStyleIdent Ident => StiElementStyleIdent.Blue;

        public override Color[] SeriesColors { get; set; }

        public Color ForeColor { get; set; }
        #endregion

        public StiCustomCardsElementStyle(StiCardsStyle style)
        {
            this.name = style.Name;
            this.BackColor = style.BackColor;
            this.LineColor = style.LineColor;
            this.SeriesColors = style.SeriesColors;
            this.ForeColor = style.ForeColor;
            this.ToolTipBrush = style.ToolTipBrush;
            this.ToolTipBorder = style.ToolTipBorder;
            this.ToolTipCornerRadius = style.ToolTipCornerRadius;
            this.ToolTipTextBrush = style.ToolTipTextBrush;
        }
    }
}
