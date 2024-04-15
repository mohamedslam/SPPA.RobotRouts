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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Drawing.Text;

namespace Stimulsoft.Drawing
{
    public class FontFamily : IDisposable
    {
        private static Dictionary<SixLabors.Fonts.FontFamily, Dictionary<string, SixLabors.Fonts.Font>> fontFamilys = new Dictionary<SixLabors.Fonts.FontFamily, Dictionary<string, SixLabors.Fonts.Font>>();

        public static FontFamily[] Families
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return System.Drawing.FontFamily.Families.Select(netFontFamily => new FontFamily(netFontFamily)).ToArray();
                else
                    return SixLabors.Fonts.SystemFonts.Collection.Families.Select(sixFontFamily => new FontFamily(sixFontFamily)).ToArray();
            }
        }

        private static SixLabors.Fonts.FontCollection baseFontCollection = new SixLabors.Fonts.FontCollection();

        private static SixLabors.Fonts.FontFamily fontFamilyRoboto;
        internal static SixLabors.Fonts.FontFamily FontFamilyRoboto
        {
            get
            {
                if (fontFamilyRoboto.Name == null)
                {
                    if (!baseFontCollection.TryGet("Roboto", out fontFamilyRoboto))
                    {
                        var assembly = typeof(FontFamily).Assembly;
                        var stream = assembly.GetManifestResourceStream("Stimulsoft.Drawing.Drawing.Roboto-Light.ttf");
                        fontFamilyRoboto = baseFontCollection.Add(stream);
                    }
                }

                return fontFamilyRoboto;
            }
        }

        public static FontFamily GenericRoboto
        {
            get
            {
                return new FontFamily(FontFamilyRoboto);
            }
        }

        internal SixLabors.Fonts.FontFamily sixFontFamily;
        internal System.Drawing.FontFamily netFontFamily;

        public bool IsStyleAvailable(System.Drawing.FontStyle style)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netFontFamily.IsStyleAvailable(style);
            else
                return sixFontFamily.GetAvailableStyles().Any(sixStyle => (int)sixStyle == (int)style || ((int)sixStyle & (int)style) != 0);
        }

        public void Dispose()
        {
        }

        private string name;
        public string Name
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFontFamily.Name ?? sixFontFamily.Name;
                else
                {
                    if (name == null)
                        name = sixFontFamily.Name;

                    return name;
                }
            }
        }

        public string GetName(int language)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netFontFamily.GetName(language);
            else
                return name;
        }

        //public static FontFamily GenericSansSerif
        //{
        //    get { return new FontFamily(GenericFontFamilies.SansSerif); }
        //}

        //public override string ToString()
        //{
        //    return String.Concat("[FontFamily: Name=", Name, "]");
        //}

        internal SixLabors.Fonts.Font CreateSixFont(float sizeInPoints, System.Drawing.FontStyle style)
        {
            //return sixFontFamily.CreateFont(sizeInPoints, (SixLabors.Fonts.FontStyle)style);
            if (fontFamilys.TryGetValue(sixFontFamily, out var sixFonts))
            {
                var key = sizeInPoints.ToString() + style.ToString();
                if (!sixFonts.TryGetValue(key, out var sixFont))
                {
                    sixFont = sixFontFamily.CreateFont(sizeInPoints, (SixLabors.Fonts.FontStyle)style);
                    sixFonts.Add(key, sixFont);
                }
                return sixFont;
            }
            else
            {
                fontFamilys.Add(sixFontFamily, new Dictionary<string, SixLabors.Fonts.Font>());
                return CreateSixFont(sizeInPoints, style);
            }
        }

        internal FontFamily(SixLabors.Fonts.FontFamily sixFontFamily)
        {
            this.sixFontFamily = sixFontFamily;
        }

        internal FontFamily(System.Drawing.FontFamily netFontFamily)
        {
            this.netFontFamily = netFontFamily;
        }

        public FontFamily(string name) : this(name, null)
        {
        }

        public FontFamily(string name, FontCollection fontCollection)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
            {
                if (fontCollection == null)
                {
                    try
                    {
                        netFontFamily = new System.Drawing.FontFamily(name);
                    }
                    catch
                    {
                        netFontFamily = (new System.Drawing.Font(name, 8)).FontFamily;
                    }
                }
                else
                {
                    netFontFamily = new System.Drawing.FontFamily(name, fontCollection.netFontCollectionBase);
                }
            }
            else
            {
                this.name = name;
                var sixFontCollection = baseFontCollection;
                if (fontCollection != null)
                {
                    sixFontCollection = (SixLabors.Fonts.FontCollection)fontCollection.sixFontCollectionBase;
                }

                SixLabors.Fonts.FontFamily fontFamily;
                if (!sixFontCollection.TryGet(name, out fontFamily))
                {
                    if (!SixLabors.Fonts.SystemFonts.TryGet(name, out fontFamily))
                    {
                        fontFamily = FontFamilyRoboto;
                    }
                }

                sixFontFamily = fontFamily;

                if (!fontFamilys.ContainsKey(fontFamily))
                    fontFamilys.Add(fontFamily, new Dictionary<string, SixLabors.Fonts.Font>());
            }
        }
    }
}

