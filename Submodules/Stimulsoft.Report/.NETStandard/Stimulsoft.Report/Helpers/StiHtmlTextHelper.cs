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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Dashboard;
using System;
using System.Text;
using static Stimulsoft.Base.Drawing.StiTextRenderer;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Controls
{
    internal class StiHtmlTextHelper : IStiHtmlTextHelper
    {
        #region Properties.Static
        public static string DefaultFontName = "Arial";
        public static float DefaultFontSize = 12f;
        #endregion

        #region Methods
        public string SetFont(string text, Font font, Color defaultColor)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor);

                richTextBox.SelectAll();
                richTextBox.SelectionFont = font;

                return GetHtmlText(richTextBox, defaultColor);
            }
        }

        public string SetFontName(string text, string fontName, Color defaultColor)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor);

                richTextBox.SelectAll();
                richTextBox.SetSelectionFont(fontName);

                return GetHtmlText(richTextBox, defaultColor);
            }
        }

        public string SetFontSize(string text, float fontSize, Color defaultColor)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor);

                richTextBox.SelectAll();
                richTextBox.SetSelectionSize((int)Math.Round(fontSize));

                return GetHtmlText(richTextBox, defaultColor);
            }
        }

        public string GrowFontSize(string text, Color defaultColor)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor);

                richTextBox.SelectAll();
                richTextBox.SetSelectionSize((int)Math.Round(GetSelectionOrFirstFont(richTextBox).Size + 1));

                return GetHtmlText(richTextBox, defaultColor);
            }
        }

        public string ShrinkFontSize(string text, Color defaultColor)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor);

                richTextBox.SelectAll();

                if (GetSelectionOrFirstFont(richTextBox).Size - 1 > 0)
                    richTextBox.SetSelectionSize((int)Math.Round(GetSelectionOrFirstFont(richTextBox).Size - 1));

                return GetHtmlText(richTextBox, defaultColor);
            }
        }

        public string SetFontBoldStyle(string text, bool isBold, Color defaultColor)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor);

                richTextBox.SelectAll();
                richTextBox.SetSelectionBold(isBold);

                return GetHtmlText(richTextBox, defaultColor);
            }
        }

        public string SetFontItalicStyle(string text, bool isItalic, Color defaultColor)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor);

                richTextBox.SelectAll();
                richTextBox.SetSelectionItalic(isItalic);

                return GetHtmlText(richTextBox, defaultColor);
            }
        }

        public string SetFontUnderlineStyle(string text, bool isUnderline, Color defaultColor)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor);

                richTextBox.SelectAll();
                richTextBox.SetSelectionUnderlined(isUnderline);

                return GetHtmlText(richTextBox, defaultColor);
            }
        }

        public string SetColor(string text, Color color)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, color);

                richTextBox.SelectAll();
                richTextBox.SelectionColor = color;

                return GetHtmlText(richTextBox, color);
            }        
        }

        public string SetHorAlignment(string text, StiTextHorAlignment alignment, Color defaultColor)
        {
            return SetHorAlignment(text, alignment, defaultColor, DefaultFontName, DefaultFontSize);
        }

        public string SetHorAlignment(string text, StiTextHorAlignment alignment, 
            Color defaultColor, string defaultFontName, float defaultFontSize)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor, defaultFontName, defaultFontSize);

                richTextBox.SelectAll();
                richTextBox.SelectionAlignment = GetAlignment(alignment);

                return GetHtmlText(richTextBox, defaultColor, defaultFontName, defaultFontSize);
            }
        }

        public Font GetFont(string text, Color defaultColor)
        {
            return GetFont(text, defaultColor, DefaultFontName, DefaultFontSize);
        }

        public Font GetFont(string text, Color defaultColor, string defaultFontName, float defaultFontSize)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor, defaultFontName, defaultFontSize);
                richTextBox.SelectAll();
                return GetSelectionOrFirstFont(richTextBox);
            }
        }

        private Font GetSelectionOrFirstFont(StiRichBoxControl richTextBox)
        {
            var font = richTextBox.SelectionFont;
            if (font != null)
                return font;

            richTextBox.Select(0, 1);
            return richTextBox.SelectionFont;
        }

        public Color GetColor(string text, Color defaultColor)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor);

                richTextBox.SelectAll();
                return richTextBox.SelectionColor;
            }
        }

        public StiTextHorAlignment GetHorAlign(string text, Color defaultColor)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(text, richTextBox, defaultColor);

                richTextBox.SelectAll();
                return GetAlignment(richTextBox.SelectionAlignment);
            }
        }

        public void SetHtmlText(string htmlText, StiRichBoxControl richTextBox, Color defaultColor)
        {
            SetHtmlText(htmlText, richTextBox, defaultColor, DefaultFontName, DefaultFontSize);
        }

        public void SetHtmlText(string htmlText, StiRichBoxControl richTextBox, Color defaultColor, string defaultFontName, float defaultFontSize)
        {
            richTextBox.SelectAll();

            richTextBox.SetSelectionFont(defaultFontName);
            richTextBox.SetSelectionSize((int)defaultFontSize);

            var baseTagsState = new StiHtmlTagsState(false, false, false, false, defaultFontSize, defaultFontName,
                defaultColor, Color.White, false, false, 0, 0, 0, StiTextHorAlignment.Center);

            Font font = null;

            var baseState = new StiHtmlState(baseTagsState, 0);
            var states = ParseHtmlToStates(htmlText, baseState, false);

            //fix for fontSize
            float fontSizeFix = defaultFontSize;
            if (states.Count > 1)
            {
                fontSizeFix = states[0].TS.FontSize;
                foreach (var state in states)
                {
                    if (state.TS.FontSize != fontSizeFix)
                    {
                        font = richTextBox.Font = new Font(state.TS.FontName, state.TS.FontSize);
                        fontSizeFix = state.TS.FontSize;
                        break;
                    }
                }
            }

            //fix for align
            if (states.Count > 2)
            {
                var state = states[states.Count - 1];
                state.TS.TextAlign = states[states.Count - 2].TS.TextAlign;
                states[states.Count - 1] = state;
            }

            foreach (var state in states)
            {
                var pos = richTextBox.Text.Length;
                richTextBox.Select(pos, 0);

                string selectedText = null;

                if (!string.IsNullOrEmpty(state.TS.Href))
                {
                    selectedText = $"<a href=\"{state.TS.Href}\">{state.Text}</a>";
                }
                else
                {
                    selectedText = state.Text.ToString();
                }

                richTextBox.SelectedText = selectedText;
                richTextBox.Select(pos, selectedText.Length);

                var style = FontStyle.Regular;
                if (state.TS.Bold)
                    style |= FontStyle.Bold;

                if (state.TS.Italic)
                    style |= FontStyle.Italic;

                if (state.TS.Underline)
                    style |= FontStyle.Underline;

                var fontName = state.TS.FontName;
                var fontSize = state.TS.FontSize;

                if (font == null)
                    font = richTextBox.Font = new Font(fontName, fontSize);

                richTextBox.SelectionFont = StiFontUtils.ChangeFontStyle(fontName, fontSizeFix, style);
                richTextBox.SelectionColor = state.TS.FontColor;
                richTextBox.SelectionAlignment = GetAlignment(state.TS.TextAlign);
            }

            richTextBox.Select(0, 0);
        }

        public string GetHtmlText(StiRichBoxControl richTextBox, Color defaultColor)
        {
            return GetHtmlText(richTextBox, defaultColor, DefaultFontName, DefaultFontSize);
        }

        public string GetHtmlText(StiRichBoxControl richTextBox, Color defaultColor, string defaultFontName, float defaultFontSize)
        {
            var isBold = false;
            var isItalic = false;
            var isUnderline = false;
            var color = defaultColor;

            var sb = new StringBuilder();
            richTextBox.SelectAll();
            var selectionFont = GetSelectionOrFirstFont(richTextBox);
            var fontName = selectionFont != null ? selectionFont.Name : defaultFontName;
            var fontSize = selectionFont != null ? selectionFont.SizeInPoints : defaultFontSize;
            var horAlignment = richTextBox.SelectionAlignment;

            sb = sb.Append($"<font face=\"{fontName}\" size=\"{fontSize}\">");
            sb = sb.Append($"<text-align=\"{horAlignment}\">");
            var isExpression = false;

            for (var index = 0; index < richTextBox.Text.Length; index++)
            {
                richTextBox.Select(index, 1);

                if (!isExpression)
                {
                    var font = richTextBox.SelectionFont;
                    var isCurrentBold = font != null && font.Style.HasFlag(FontStyle.Bold);
                    var isCurrentItalic = font != null && font.Style.HasFlag(FontStyle.Italic);
                    var isCurrentUnderline = font != null && font.Style.HasFlag(FontStyle.Underline);
                    var currentColor = richTextBox.SelectionColor;
                    var isLineBreak = richTextBox.Text[index] == '\n';

                    if (isLineBreak)
                        sb = sb.Append("<br />");

                    #region Bold
                    if (isCurrentBold && !isBold)
                    {
                        isBold = true;
                        sb = sb.Append("<b>");
                    }

                    if (!isCurrentBold && isBold)
                    {
                        isBold = false;
                        sb = sb.Append("</b>");
                    }
                    #endregion

                    #region Italic
                    if (isCurrentItalic && !isItalic)
                    {
                        isItalic = true;
                        sb = sb.Append("<i>");
                    }

                    if (!isCurrentItalic && isItalic)
                    {
                        isItalic = false;
                        sb = sb.Append("</i>");
                    }
                    #endregion

                    #region Underline
                    if (isCurrentUnderline && !isUnderline)
                    {
                        isUnderline = true;
                        sb = sb.Append("<u>");
                    }

                    if (!isCurrentUnderline && isUnderline)
                    {
                        isUnderline = false;
                        sb = sb.Append("</u>");
                    }
                    #endregion

                    #region Color
                    if (color != currentColor)
                    {
                        if (color == defaultColor)
                        {
                            sb = sb.Append($"<font-color=\"{ColorTranslator.ToHtml(currentColor)}\">");
                            color = currentColor;
                        }
                        else if (currentColor == defaultColor)
                        {
                            sb = sb.Append("</font-color>");
                            color = currentColor;
                        }
                        else
                        {
                            sb = sb.Append("</font-color>");
                            sb = sb.Append($"<font-color=\"{ColorTranslator.ToHtml(currentColor)}\">");
                            color = currentColor;
                        }
                    }
                    #endregion
                }

                if (richTextBox.Text[index] == '{')
                    isExpression = true;

                if (richTextBox.Text[index] == '}')
                    isExpression = false;

                sb = sb.Append(richTextBox.Text[index]);
            }

            if (isExpression)
                sb = sb.Append("}");

            if (color != defaultColor)
                sb = sb.Append("</font-color>");

            if (isUnderline)
                sb = sb.Append("</u>");

            if (isItalic)
                sb = sb.Append("</i>");

            if (isBold)
                sb = sb.Append("</b>");

            sb = sb.Append("</text-align>");
            sb = sb.Append("</font>");

            return sb.ToString();
        }

        private static StiRichTextAlignment GetAlignment(StiTextHorAlignment align)
        {
            switch (align)
            {
                case StiTextHorAlignment.Left:
                    return StiRichTextAlignment.Left;

                case StiTextHorAlignment.Center:
                    return StiRichTextAlignment.Center;

                case StiTextHorAlignment.Right:
                    return StiRichTextAlignment.Right;

                case StiTextHorAlignment.Width:
                    return StiRichTextAlignment.Justify;

                default:
                    throw new NotSupportedException();
            }
        }

        private static StiTextHorAlignment GetAlignment(StiRichTextAlignment align)
        {
            switch (align)
            {
                case StiRichTextAlignment.Left:
                    return StiTextHorAlignment.Left;

                case StiRichTextAlignment.Center:
                    return StiTextHorAlignment.Center;

                case StiRichTextAlignment.Right:
                    return StiTextHorAlignment.Right;

                case StiRichTextAlignment.Justify:
                    return StiTextHorAlignment.Width;

                default:
                    throw new NotSupportedException();
            }
        }

        public string GetSimpleText(string htmlText, Color defaultColor)
        {
            using (var richTextBox = new StiRichBoxControl())
            {
                SetHtmlText(htmlText, richTextBox, defaultColor);

                return richTextBox.Text;
            }
        }
        #endregion
    }
}
