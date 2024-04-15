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

using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Dashboard.Styles
{
    public class StiCustomControlElementStyle : StiControlElementStyle
    {
        #region Fields
        private string styleName;
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCustomDashboardControlStyle;
        #endregion

        #region Properties
        public override string LocalizedName => this.styleName;

        public override StiElementStyleIdent Ident => StiElementStyleIdent.Custom;
        #endregion

        public StiCustomControlElementStyle(StiDialogStyle style)
        {
            this.Font = style.Font;
            this.styleName = style.Name;
            this.BackColor = style.BackColor;
            this.ForeColor = style.ForeColor;
            this.GlyphColor = style.GlyphColor;
            this.HotBackColor = style.HotBackColor;
            this.HotForeColor = style.HotForeColor;
            this.HotGlyphColor = style.HotGlyphColor;
            this.HotSelectedBackColor = style.HotSelectedBackColor;
            this.HotSelectedForeColor = style.HotSelectedForeColor;
            this.HotSelectedGlyphColor = style.HotSelectedGlyphColor;
            this.SelectedBackColor = style.SelectedBackColor;
            this.SelectedForeColor = style.SelectedForeColor;
            this.SelectedGlyphColor = style.SelectedGlyphColor;
            this.SeparatorColor = style.SeparatorColor;
        }
    }
}