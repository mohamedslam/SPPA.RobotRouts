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

using Stimulsoft.Base.Json.Linq;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using FontFamily = Stimulsoft.Drawing.FontFamily;
#endif

namespace Stimulsoft.Base.Context
{
    public class StiFontGeom : StiGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.Add(new JProperty("FontName", FontName));
            jObject.Add(new JProperty("FontSize", FontSize));
            jObject.Add(new JProperty("FontStyle", FontStyle.ToString()));
            jObject.Add(new JProperty("Unit", Unit.ToString()));
            jObject.Add(new JProperty("GdiCharSet", GdiCharSet));
            jObject.Add(new JProperty("GdiVerticalFont", GdiVerticalFont));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Methods
        public static StiFontGeom ChangeFontSize(Font font, float newFontSize)
        {
            if (newFontSize < 1) newFontSize = 1;

            return new StiFontGeom(
                font.FontFamily.Name,
                newFontSize,
                font.Style,
                font.Unit,
                font.GdiCharSet,
                font.GdiVerticalFont);
        }
        #endregion

        #region Properties
        public FontFamily FontFamily { get; set; }

        public string FontName { get; set; }

        public float FontSize { get; set; }

        public FontStyle FontStyle { get; set; }

        public GraphicsUnit Unit { get; set; }

        public byte GdiCharSet { get; set; }

        public bool GdiVerticalFont { get; set; }
        #endregion

        #region Properties.Override
        public override StiGeomType Type => StiGeomType.Font;
        #endregion

        public StiFontGeom(Font font)
            : this(font.FontFamily.Name, font.Size, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont)
        {
        }

        public StiFontGeom(string fontName, float fontSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
            : this(null, fontName, fontSize, style, unit, gdiCharSet, gdiVerticalFont)
        {
        }

        public StiFontGeom(FontFamily fontFamily, string fontName, float fontSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
        {
            this.FontFamily = fontFamily;
            this.FontName = fontName;
            this.FontSize = fontSize;
            this.FontStyle = style;
            this.Unit = unit;
            this.GdiCharSet = gdiCharSet;
            this.GdiVerticalFont = gdiVerticalFont;
        }
    }
}
