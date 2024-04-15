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

using System;
using System.ComponentModel;

namespace Stimulsoft.Drawing
{
    [Serializable]
    [TypeConverter(typeof(FontConverter))]
    public class Font : IDisposable
    {
        internal SixLabors.Fonts.Font sixFont;
        internal System.Drawing.Font netFont;

        private FontFamily fontFamily;
        public FontFamily FontFamily
        {
            get
            {
                if (fontFamily == null)
                    if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                        fontFamily = new FontFamily(netFont.FontFamily);
                    else
                        fontFamily = new FontFamily(Name);

                return fontFamily;
            }

        }

        private bool bold;
        public bool Bold
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.Bold;
                else 
                    return bold;
            }
        }

        private bool italic;
        public bool Italic
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.Italic;
                else
                    return italic;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.Name;
                else
                    return name;
            }
        }

        private string originalFontName;
        public string OriginalFontName
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.OriginalFontName;
                else
                    return originalFontName;
            }
        }

        private float size;
        public float Size
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.Size;
                else
                    return size;
            }
        }

        private float sizeInPoints;
        public float SizeInPoints
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.SizeInPoints;
                else
                    return sizeInPoints;
            }
        }

        private bool strikeout;
        public bool Strikeout
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.Strikeout;
                else
                    return strikeout;
            }
        }

        private System.Drawing.FontStyle style;
        public System.Drawing.FontStyle Style
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.Style;
                else
                    return style;
            }
        }

        private bool underline;
        public bool Underline
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.Underline;
                else
                    return underline;
            }
        }

        private System.Drawing.GraphicsUnit unit;
        public System.Drawing.GraphicsUnit Unit
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.Unit;
                else
                    return unit;
            }
        }

        private bool gdiVerticalFont = false;
        public bool GdiVerticalFont
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.GdiVerticalFont;
                else
                    return gdiVerticalFont;
            }
        }

        private byte gdiCharSet = 0;
        public byte GdiCharSet
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.GdiCharSet;
                else
                    return gdiCharSet;
            }
        }

        public float Height
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFont.GetHeight();
                else
                {
                    var point = ((sixFont.FontMetrics.LineHeight - sixFont.FontMetrics.LineGap) * SizeInPoints / sixFont.FontMetrics.UnitsPerEm);
                    return UnitConversion(System.Drawing.GraphicsUnit.Point, System.Drawing.GraphicsUnit.Pixel, point);
                }
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Dispose()
        {
        }

        public override String ToString()
        {
            return String.Format("[Font: Name={0}, Size={1}, Units={2}, GdiCharSet={3}, GdiVerticalFont={4}]", Name, Size, (int)Unit, GdiCharSet, GdiVerticalFont);
        }

        internal float UnitConversion(System.Drawing.GraphicsUnit fromUnit, System.Drawing.GraphicsUnit toUnit, float nSrc)
        {
            float inchs = 0;
            float nTrg = 0;

            switch (fromUnit)
            {
                case System.Drawing.GraphicsUnit.Display:
                    inchs = nSrc / 75f;
                    break;
                case System.Drawing.GraphicsUnit.Document:
                    inchs = nSrc / 300f;
                    break;
                case System.Drawing.GraphicsUnit.Inch:
                    inchs = nSrc;
                    break;
                case System.Drawing.GraphicsUnit.Millimeter:
                    inchs = nSrc / 25.4f;
                    break;
                case System.Drawing.GraphicsUnit.Pixel:
                case System.Drawing.GraphicsUnit.World:
                    inchs = nSrc / 96f;
                    break;
                case System.Drawing.GraphicsUnit.Point:
                    inchs = nSrc / 72f;
                    break;
                default:
                    throw new ArgumentException("Invalid GraphicsUnit");
            }

            switch (toUnit)
            {
                case System.Drawing.GraphicsUnit.Display:
                    nTrg = inchs * 75;
                    break;
                case System.Drawing.GraphicsUnit.Document:
                    nTrg = inchs * 300;
                    break;
                case System.Drawing.GraphicsUnit.Inch:
                    nTrg = inchs;
                    break;
                case System.Drawing.GraphicsUnit.Millimeter:
                    nTrg = inchs * 25.4f;
                    break;
                case System.Drawing.GraphicsUnit.Pixel:
                case System.Drawing.GraphicsUnit.World:
                    nTrg = inchs * 96f;
                    break;
                case System.Drawing.GraphicsUnit.Point:
                    nTrg = inchs * 72;
                    break;
                default:
                    throw new ArgumentException("Invalid GraphicsUnit");
            }

            return nTrg;
        }

        public float GetHeight()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netFont.GetHeight();
            else 
                return Height;
        }

        public IntPtr ToHfont()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netFont.ToHfont();
            else
                throw new NotImplementedException();
        }

        public static Font FromLogFont(object elfLogFont)
        {
            throw new NotImplementedException();
        }

        public Font(Font prototype, float size)
            : this(prototype.FontFamily, size, prototype.style, prototype.Unit)
        {
        }

        public Font(Font prototype, System.Drawing.FontStyle newStyle)
            : this(prototype.FontFamily, prototype.Size, newStyle, prototype.Unit)
        {
        }

        public Font(FontFamily family, float emSize, System.Drawing.GraphicsUnit unit)
            : this(family, emSize, System.Drawing.FontStyle.Regular, unit, 1, false)
        {
        }

        public Font(string familyName, float emSize, System.Drawing.GraphicsUnit unit)
            : this(new FontFamily(familyName), emSize, System.Drawing.FontStyle.Regular, unit, 1, false)
        {
        }

        public Font(FontFamily family, float emSize)
            : this(family, emSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 1, false)
        {
        }

        public Font(FontFamily family, float emSize, System.Drawing.FontStyle style)
            : this(family, emSize, style, System.Drawing.GraphicsUnit.Point, 1, false)
        {
        }

        public Font(FontFamily family, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit)
            : this(family, emSize, style, unit, 1, false)
        {
        }

        public Font(FontFamily family, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit, byte gdiCharSet)
            : this(family, emSize, style, unit, gdiCharSet, false)
        {
        }

        public Font(string familyName, float emSize)
            : this(familyName, emSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 1, false)
        {
        }

        public Font(string familyName, float emSize, System.Drawing.FontStyle style)
            : this(familyName, emSize, style, System.Drawing.GraphicsUnit.Point, 1, false)
        {
        }

        public Font(string familyName, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit)
            : this(familyName, emSize, style, unit, 1, false)
        {
        }

        public Font(string familyName, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit, byte gdiCharSet)
            : this(familyName, emSize, style, unit, gdiCharSet, false)
        {
        }

        public Font(string familyName, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
            : this(new FontFamily(familyName), emSize, style, unit, gdiCharSet, gdiVerticalFont)
        {
            if (Graphics.GraphicsEngine != GraphicsEngine.Gdi)
                this.name = familyName;
        }

        public Font(FontFamily family, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                this.netFont = new System.Drawing.Font(family.netFontFamily, emSize, (System.Drawing.FontStyle)style, (System.Drawing.GraphicsUnit)unit, gdiCharSet, gdiVerticalFont);
            else
            {
                this.name = family.Name;
                this.originalFontName = family.Name;
                this.fontFamily = family;
                this.size = emSize;

                this.unit = unit;
                this.style = style;

                this.sizeInPoints = UnitConversion(unit, System.Drawing.GraphicsUnit.Point, this.size);

                this.bold = this.italic = this.strikeout = this.underline = false;

                if ((style & System.Drawing.FontStyle.Bold) == System.Drawing.FontStyle.Bold)
                    this.bold = true;

                if ((style & System.Drawing.FontStyle.Italic) == System.Drawing.FontStyle.Italic)
                    this.italic = true;

                if ((style & System.Drawing.FontStyle.Strikeout) == System.Drawing.FontStyle.Strikeout)
                    this.strikeout = true;

                if ((style & System.Drawing.FontStyle.Underline) == System.Drawing.FontStyle.Underline)
                    this.underline = true;

                this.sixFont = this.fontFamily.CreateSixFont(sizeInPoints, style);
            }
        }

        public static implicit operator System.Drawing.Font(Font font)
        {
            return font.netFont;
        }

        public static implicit operator Font(System.Drawing.Font netFont)
        {
            var font = new Font(netFont.FontFamily.Name, netFont.Size, netFont.Style, netFont.Unit, netFont.GdiCharSet);
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi) {
                font.netFont = netFont;
                font.sixFont = null;
            }
            
            return font;
        }
    }
}
