#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System;
using System.Collections;
using Stimulsoft.Base.Drawing;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Export
{
    public class StiCellStyle : ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            StiCellStyle style = this.MemberwiseClone() as StiCellStyle;

            if (this.Border != null)
                style.Border = this.Border.Clone() as StiBorderSide;

            if (this.BorderL != null)
                style.BorderL = this.BorderL.Clone() as StiBorderSide;

            if (this.BorderR != null)
                style.BorderR = this.BorderR.Clone() as StiBorderSide;

            if (this.BorderB != null)
                style.BorderB = this.BorderB.Clone() as StiBorderSide;

            if (this.Font != null)
                style.Font = this.Font.Clone() as Font;

            if (this.TextOptions != null)
                style.TextOptions = this.TextOptions.Clone() as StiTextOptions;

            return style;
        }
        #endregion

        #region IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Border != null ? Border.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BorderL != null ? BorderL.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BorderR != null ? BorderR.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BorderB != null ? BorderB.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ (Font != null ? Font.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)HorAlignment;
                hashCode = (hashCode * 397) ^ (int)VertAlignment;
                hashCode = (hashCode * 397) ^ (TextOptions != null ? TextOptions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ TextColor.GetHashCode();
                hashCode = (hashCode * 397) ^ WordWrap.GetHashCode();
                hashCode = (hashCode * 397) ^ (Format != null ? Format.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (internalStyleName != null ? internalStyleName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ LineSpacing.GetHashCode();
                hashCode = (hashCode * 397) ^ AllowHtmlTags.GetHashCode();
                return hashCode;
            }
        }

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}

        public override bool Equals(Object obj)
        {
            StiCellStyle style = obj as StiCellStyle;

            if (!this.Color.Equals(style.Color)) return false;
            if (this.HorAlignment != style.HorAlignment) return false;
            if (this.VertAlignment != style.VertAlignment) return false;
            if (!this.TextColor.Equals(style.TextColor)) return false;

            if (this.WordWrap != style.WordWrap) return false;

            if (this.Font.Bold != style.Font.Bold) return false;
            if (this.Font.Italic != style.Font.Italic) return false;
            if (this.Font.Strikeout != style.Font.Strikeout) return false;
            if (this.Font.Underline != style.Font.Underline) return false;
            if (this.Font.Size != style.Font.Size) return false;
            if (this.Font.Name != style.Font.Name) return false;    //slow operation

            if (this.TextOptions == null)
            {
                if (style.TextOptions != null) return false;
            }
            else
            {
                if (style.TextOptions == null) return false;
                if (this.TextOptions.Angle != style.TextOptions.Angle) return false;
                if (this.TextOptions.WordWrap != style.TextOptions.WordWrap) return false;
                if (this.TextOptions.RightToLeft != style.TextOptions.RightToLeft) return false;
            }

            if (this.Border == null)
            {
                if (style.Border != null) return false;
            }
            else
            {
                if (style.Border == null) return false;
                if (this.Border.Color != style.Border.Color) return false;
                if (this.Border.Size != style.Border.Size) return false;
                if (this.Border.Style != style.Border.Style) return false;
            }


            if (this.BorderL == null)
            {
                if (style.BorderL != null) return false;
            }
            else
            {
                if (style.BorderL == null) return false;
                if (this.BorderL.Color != style.BorderL.Color) return false;
                if (this.BorderL.Size != style.BorderL.Size) return false;
                if (this.BorderL.Style != style.BorderL.Style) return false;
            }

            if (this.BorderR == null)
            {
                if (style.BorderR != null) return false;
            }
            else
            {
                if (style.BorderR == null) return false;
                if (this.BorderR.Color != style.BorderR.Color) return false;
                if (this.BorderR.Size != style.BorderR.Size) return false;
                if (this.BorderR.Style != style.BorderR.Style) return false;
            }

            if (this.BorderB == null)
            {
                if (style.BorderB != null) return false;
            }
            else
            {
                if (style.BorderB == null) return false;
                if (this.BorderB.Color != style.BorderB.Color) return false;
                if (this.BorderB.Size != style.BorderB.Size) return false;
                if (this.BorderB.Style != style.BorderB.Style) return false;
            }

            if (this.Format != style.Format) return false;

            if (this.InternalStyleName != style.InternalStyleName) return false;

            if (this.LineSpacing != style.LineSpacing) return false;

            if (this.AllowHtmlTags != style.AllowHtmlTags) return false;

            return true;
        }
        #endregion

        #region GetStyleHashCode
        public static StiCellStyle GetStyleFromCache(Color color, Color textColor, Font font, StiTextHorAlignment horAlignment, StiVertAlignment vertAlignment,
            StiBorderSide border, StiBorderSide borderL, StiBorderSide borderR, StiBorderSide borderB, StiTextOptions textOptions, bool wordWrap, string format, string internalStyleName, double lineSpacing, bool allowHtmlTags,
            Hashtable hashStyles, ArrayList styles, Hashtable fontsCache, StiCellStyle cellStyle, bool simplyAdd)
        {
            int hashCode = 0;

            unchecked
            {
                if (font != null)
                {
                    object fontHash = fontsCache[font];
                    if (fontHash == null)
                    {
                        hashCode = (font.Bold ? 1231 : 1237);
                        hashCode = (hashCode * 397) ^ (font.Italic ? 1231 : 1237);
                        hashCode = (hashCode * 397) ^ (font.Strikeout ? 1231 : 1237);
                        hashCode = (hashCode * 397) ^ (font.Underline ? 1231 : 1237);
                        hashCode = (hashCode * 397) ^ font.Size.GetHashCode();
                        hashCode = (hashCode * 397) ^ font.Name.GetHashCode();

                        fontsCache[font] = hashCode;
                    }
                    else
                    {
                        hashCode = (int)fontHash;
                    }
                }
                hashCode = (hashCode * 397) ^ (border != null ? border.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (borderL != null ? borderL.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (borderR != null ? borderR.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (borderB != null ? borderB.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ color.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)horAlignment;
                hashCode = (hashCode * 397) ^ (int)vertAlignment;
                hashCode = (hashCode * 397) ^ (textOptions != null ? textOptions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ textColor.GetHashCode();
                hashCode = (hashCode * 397) ^ wordWrap.GetHashCode();
                hashCode = (hashCode * 397) ^ (format != null ? format.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (internalStyleName != null ? internalStyleName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ lineSpacing.GetHashCode();
                hashCode = (hashCode * 397) ^ allowHtmlTags.GetHashCode();
            }

            if (simplyAdd && cellStyle != null)
            {
                hashStyles[hashCode] = cellStyle;
                return cellStyle;
            }

            object obj = hashStyles[hashCode];
            if (obj == null)
            {
                StiCellStyle newStyle = new StiCellStyle(color, textColor, font, horAlignment, vertAlignment, border, borderL, borderR, borderB, textOptions, wordWrap, format, internalStyleName, lineSpacing, allowHtmlTags);
                if (cellStyle != null)
                    newStyle.styleName = cellStyle.styleName;

                hashStyles[hashCode] = newStyle;
                styles.Add(newStyle);
                return newStyle;
            }
            return (StiCellStyle)obj;
        }
        #endregion

        #region Fields
        public StiBorderSide Border;
        public StiBorderSide BorderL;
        public StiBorderSide BorderR;
        public StiBorderSide BorderB;
        public bool AbsolutePosition = false;
        public bool AllowHtmlTags = false;
        public Color Color;
        public Font Font;
        public StiTextHorAlignment HorAlignment;
        public StiVertAlignment VertAlignment;
        public StiTextOptions TextOptions;
        public Color TextColor;
        public bool WordWrap;
        public string Format;
        public double LineSpacing = 1;
        #endregion

        #region Properties
        private string internalStyleName = null;
        public string InternalStyleName
        {
            get
            {
                return internalStyleName;
            }
            set
            {
                if (value == null) return;
                internalStyleName = StiNameValidator.CorrectName(value.Trim());
            }
        }

        private string styleName = null;
        public string StyleName
        {
            get
            {
                if (styleName != null) return styleName;

                styleName = InternalStyleName ?? Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
                return styleName;
            }
            set
            {
                styleName = value;
            }
        }
        #endregion

        //public StiCellStyle(Color color, Color textColor, Font font,
        //    StiTextHorAlignment horAlignment, StiVertAlignment vertAlignment,
        //    StiBorderSide border, StiTextOptions textOptions, bool wordWrap, string format) :
        //    this(color, textColor, font, horAlignment, vertAlignment, border, textOptions, wordWrap, format, null)
        //{
        //}

        //public StiCellStyle(Color color, Color textColor, Font font,
        //    StiTextHorAlignment horAlignment, StiVertAlignment vertAlignment,
        //    StiBorderSide border, StiTextOptions textOptions, bool wordWrap, string format, string styleName)
        //{
        //    this.Color = color;
        //    this.TextColor = textColor;
        //    this.Font = font;
        //    this.HorAlignment = horAlignment;
        //    this.VertAlignment = vertAlignment;
        //    this.Border = border;
        //    this.BorderL = null;
        //    this.BorderR = null;
        //    this.BorderB = null;
        //    this.TextOptions = textOptions;
        //    this.WordWrap = wordWrap;
        //    this.Format = format;
        //    this.InternalStyleName = styleName;
        //}

        //public StiCellStyle(Color color, Color textColor, Font font,
        //    StiTextHorAlignment horAlignment, StiVertAlignment vertAlignment,
        //    StiBorderSide border, StiBorderSide borderL, StiBorderSide borderR, StiBorderSide borderB, StiTextOptions textOptions, bool wordWrap, string format) :
        //    this(color, textColor, font, horAlignment, vertAlignment, border, borderL, borderR, borderB, textOptions, wordWrap, format, null)
        //{
        //}

        public StiCellStyle(Color color, Color textColor, Font font,
            StiTextHorAlignment horAlignment, StiVertAlignment vertAlignment,
            StiBorderSide border, StiBorderSide borderL, StiBorderSide borderR, StiBorderSide borderB, StiTextOptions textOptions,
            bool wordWrap, string format, string styleName, double lineSpacing, bool allowHtmlTags)
        {
            this.Color = color;
            this.TextColor = textColor;
            this.Font = font;
            this.HorAlignment = horAlignment;
            this.VertAlignment = vertAlignment;
            this.Border = border;
            this.BorderL = borderL;
            this.BorderR = borderR;
            this.BorderB = borderB;
            this.TextOptions = textOptions;
            this.WordWrap = wordWrap;
            this.InternalStyleName = styleName;
            this.Format = format;
            this.LineSpacing = lineSpacing;
            this.AllowHtmlTags = allowHtmlTags;
        }
    }
}
