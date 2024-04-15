#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Dashboard
{
    internal interface IStiHtmlTextHelper
    {
        #region Methods
        string SetFontName(string text, string fontName, Color defaultColor);

        string SetFontSize(string text, float fontSize, Color defaultColor);

        string GrowFontSize(string text, Color defaultColor);

        string ShrinkFontSize(string text, Color defaultColor);

        string SetFontBoldStyle(string text, bool isBold, Color defaultColor);

        string SetFontItalicStyle(string text, bool isItalic, Color defaultColor);

        string SetFontUnderlineStyle(string text, bool isUnderline, Color defaultColor);

        string SetColor(string text, Color color);

        string SetHorAlignment(string text, StiTextHorAlignment alignment, Color defaultColor);

        Font GetFont(string text, Color defaultColor);

        Color GetColor(string text, Color defaultColor);

        StiTextHorAlignment GetHorAlign(string text, Color defaultColor);

        string GetSimpleText(string htmlText, Color defaultColor);
        #endregion
    }
}
