#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

#if STIDRAWING
using FontFamily = Stimulsoft.Drawing.FontFamily;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Base.Drawing
{
	public sealed class StiFontUtils
	{
        #region Methods
        public static FontStyle CorrectStyle(string fontName, FontStyle style)
        {
            var isCustomFont = StiFontCollection.IsCustomFont(fontName);

            if (isCustomFont && StiFontCollection.IsStyleAvailable(fontName, style)) return style;

            FontFamily family = null;
            try
            {
                family = isCustomFont ? StiFontCollection.GetFontFamily(fontName) : new FontFamily(fontName);
            }
            catch
            {
                return FontStyle.Regular;
            }

            try
            {
                if (family.IsStyleAvailable(style))
                    return style;
                else
                {
                    if (!family.IsStyleAvailable(FontStyle.Bold) && (style & FontStyle.Bold) > 0)
                        style -= FontStyle.Bold;

                    if (!family.IsStyleAvailable(FontStyle.Italic) && (style & FontStyle.Italic) > 0)
                        style -= FontStyle.Italic;

                    if (!family.IsStyleAvailable(FontStyle.Strikeout) && (style & FontStyle.Strikeout) > 0)
                        style -= FontStyle.Strikeout;

                    if (!family.IsStyleAvailable(FontStyle.Underline) && (style & FontStyle.Underline) > 0)
                        style -= FontStyle.Underline;

                    if (!family.IsStyleAvailable(style))
                    {
                        if (family.IsStyleAvailable(FontStyle.Bold))
                            return FontStyle.Bold;

                        if (family.IsStyleAvailable(FontStyle.Italic))
                            return FontStyle.Italic;

                        else if (family.IsStyleAvailable(FontStyle.Underline))
                            return FontStyle.Underline;

                        else if (family.IsStyleAvailable(FontStyle.Strikeout))
                            return FontStyle.Strikeout;
                    }

                    return style;
                }
            }
            finally
            {
                if (!isCustomFont)
                    family.Dispose();
            }
        }

		public static Font ChangeFontName(Font font, string newFontName)
		{
			if (string.IsNullOrEmpty(newFontName)) return font;
			
			return new Font(
				StiFontCollection.GetFontFamily(newFontName),
				font.Size,
				font.Style,
				font.Unit,
				font.GdiCharSet,
				font.GdiVerticalFont);
		}

		public static Font ChangeFontFamily(Font font, FontFamily fontFamily)
		{
			if (fontFamily == null) 
				return font;

			return new Font(
				fontFamily,
				font.Size,
				font.Style,
				font.Unit,
				font.GdiCharSet,
				font.GdiVerticalFont);
		}

		public static Font ChangeFontSize(Font font, float newFontSize)
		{
			if (newFontSize < 1)
			    newFontSize = 1;

			return new Font(
				font.FontFamily, 
				newFontSize,
				font.Style, 
				font.Unit, 
				font.GdiCharSet, 
				font.GdiVerticalFont);
		}

        public static Font ChangeFontStyle(Font font, FontStyle style)
        {
            return new Font(
                font.FontFamily,
                font.Size,
                style,
                font.Unit,
                font.GdiCharSet,
                font.GdiVerticalFont);
        }

	    public static Font ChangeFontStyle(string fontName, float fontSize, FontStyle style)
	    {
	        return new Font(fontName, fontSize, style);
	    }

        public static Font ChangeFontStyleBold(Font font, bool bold)
		{
			var style = FontStyle.Regular;

			if (bold) style |= FontStyle.Bold;
			if (font.Italic) style |= FontStyle.Italic;
			if (font.Underline) style |= FontStyle.Underline;

			return new Font(
				font.FontFamily,
				font.Size,
				style,
				font.Unit,
				font.GdiCharSet,
				font.GdiVerticalFont);
		}

		public static Font ChangeFontStyleItalic(Font font, bool italic)
		{
			var style = FontStyle.Regular;

			if (font.Bold) style |= FontStyle.Bold;
			if (italic) style |= FontStyle.Italic;
			if (font.Underline) style |= FontStyle.Underline;

			return new Font(
				font.FontFamily,
				font.Size,
				style,
				font.Unit,
				font.GdiCharSet,
				font.GdiVerticalFont);
		}

		public static Font ChangeFontStyleUnderline(Font font, bool underline)
		{
			var style = FontStyle.Regular;

			if (font.Bold) style |= FontStyle.Bold;
			if (font.Italic) style |= FontStyle.Italic;
			if (underline) style |= FontStyle.Underline;

			return new Font(
				font.FontFamily,
				font.Size,
				style,
				font.Unit,
				font.GdiCharSet,
				font.GdiVerticalFont);
		}

        public static Font ChangeFontStyleStrikeout(Font font, bool strikeout)
        {
            var style = FontStyle.Regular;

            if (font.Bold) style |= FontStyle.Bold;
            if (font.Italic) style |= FontStyle.Italic;
			if (font.Underline) style |= FontStyle.Underline;
            if (strikeout) style |= FontStyle.Strikeout;

            return new Font(
                font.FontFamily,
                font.Size,
                style,
                font.Unit,
                font.GdiCharSet,
                font.GdiVerticalFont);
        }
        #endregion
    }
}
