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
using System.Drawing;

namespace Stimulsoft.Report.Dashboard.Styles
{
    public class StiCustomIndicatorElementStyle : StiIndicatorElementStyle
    {
        #region Fields
        private string name;
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCustomDashboardIndicatorStyle;
        #endregion

        #region Properties
        public override string LocalizedName => this.name;

        public override StiElementStyleIdent Ident => StiElementStyleIdent.Custom;

        public override Color GlyphColor { get; set; }

        public override Color ForeColor { get; set; }
        #endregion

        public StiCustomIndicatorElementStyle(StiIndicatorStyle style)
        {
            this.name = style.Name;
            this.GlyphColor = style.GlyphColor;
            this.ForeColor = style.ForeColor;
            this.BackColor = style.BackColor;
            this.HotBackColor = style.HotBackColor;
            this.PositiveColor = style.PositiveColor;
            this.NegativeColor = style.NegativeColor;
            this.ToolTipBrush = style.ToolTipBrush;
            this.ToolTipBorder = style.ToolTipBorder;
            this.ToolTipCornerRadius = style.ToolTipCornerRadius;
            this.ToolTipTextBrush = style.ToolTipTextBrush;
        }
    }
}