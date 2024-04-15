#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
{	                         										}
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

namespace Stimulsoft.Report.Design
{
    public interface IStiHtmlTextEditorControl
    {
        string HtmlText { get; set; }

        bool ShowFontNameBox { get; set; }

        bool ShowFontSizeBox { get; set; }

        bool ShowFontBold { get; set; }

        bool ShowFontItalic { get; set; }

        bool ShowFontUnderline { get; set; }

        bool ShowFontColor { get; set; }

        bool ShowAlignLeft { get; set; }
        
        bool ShowAlignCenter { get; set; }

        bool ShowAlignRight { get; set; }

        bool ShowAlignJustify { get; set; }

        bool ShowClearAllFormatting { get; set; }
    }
}
