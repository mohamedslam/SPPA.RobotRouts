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
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Stimulsoft.Base.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Stimulsoft.Base.Events;
using System.Drawing.Text;
using System.Drawing;

#if STIDRAWING
using PrivateFontCollection = Stimulsoft.Drawing.Text.PrivateFontCollection;
using FontFamily = Stimulsoft.Drawing.FontFamily;
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Base
{
    public static class StiFontCollection
    {
        #region Fields.Static
        private static object lockObject = new object();
        private static Hashtable fontFamilyHash = new Hashtable();  //name to family
        private static Hashtable fontContentToName = new Hashtable();       //content hash to name
        private static Hashtable customFontCountHash = new Hashtable();
        private static Dictionary<string, FontVStyles> fontsVHash = new Dictionary<string, FontVStyles>();  //name to FontVStyles
        #endregion

        #region Properties.Static
        private static PrivateFontCollection instance = new PrivateFontCollection();
        public static PrivateFontCollection Instance
        {
            get
            {
                return instance ?? (instance = new PrivateFontCollection());
            }
        }

        private class FontVStyles
        {
            public FontV Regular;
            public FontV Bold;
            public FontV Italic;
            public FontV BoldItalic;
        }

        public static string DefaultCachePath { get; set; }

#if NETSTANDARD
        public static bool AllowFileCache { get; set; } = false;
#else
        public static bool AllowFileCache { get; set; } = true;
#endif  

        #endregion

        #region Methods.Static
        private static string AddContentToCache(byte[] content, string filePath, string extension, string alias = null, FontStyle? fontStyle = null, bool isResource = false)
        {
            string hash = FontV.GetHashName(content);
            string name = fontContentToName[hash] as string;
            if (name != null) return name;

            FontV fontV = new FontV(alias, content, filePath, extension, AllowFileCache, hash, isResource ? null : instance);

            //get font info
            name = fontV.Name;
            FontStyle fntStyle = FontStyle.Regular;
            var ttfFont = StiFontReader.ScanFontFile(content, name);
            if (ttfFont != null)
            {
                var name2 = ttfFont.GetFamilyName();
                if (!string.IsNullOrWhiteSpace(name2)) name = name2;
                fntStyle = ttfFont.GetStyle();
            }
            if (fontStyle != null)
            {
                fntStyle = fontStyle.Value & (FontStyle.Bold | FontStyle.Italic);
            }

            //get FontVStyles
            FontVStyles fstyles = null;
            if (fontsVHash.ContainsKey(name))
            {
                fstyles = fontsVHash[name];
            }
            else
            {
                fstyles = new FontVStyles();
                fontsVHash.Add(name, fstyles);
            }

            //store fontV
            if (fntStyle == FontStyle.Regular)
            {
                fstyles.Regular = fontV;
            }
            else if (fntStyle == FontStyle.Bold)
            {
                fstyles.Bold = fontV;
            }
            else if (fntStyle == FontStyle.Italic)
            {
                fstyles.Italic = fontV;
            }
            else if (fntStyle == (FontStyle.Bold | FontStyle.Italic))
            {
                fstyles.BoldItalic = fontV;
            }

            fontContentToName[hash] = name;

            return name;
        }

        private static void CopyTo(string name, string alias)
        {
            if (!string.IsNullOrWhiteSpace(alias) && (name != alias) && fontsVHash.ContainsKey(name) && !fontsVHash.ContainsKey(alias))
            {
                FontVStyles fstyles = fontsVHash[name];
                fontsVHash[alias] = fstyles;
            }
        }

        public static void AddFontFile(string fileName, string alias = null, FontStyle? fontStyle = null)
        {
            byte[] content = null;
            try
            {
                content = File.ReadAllBytes(fileName);
            }
            catch
            {
            }
            if (content == null || content.Length == 0) return;

            lock (lockObject)
            {
                string name = AddContentToCache(content, fileName, Path.GetExtension(fileName).Substring(1), alias, fontStyle);
                CopyTo(name, alias);
                //Instance.AddFontFile(fileName);
            }
        }

        public static void AddFontBytes(byte[] content, string alias = null, FontStyle? fontStyle = null)
        {
            if (content == null || content.Length == 0) return;

            lock (lockObject)
            {
                string name = AddContentToCache(content, null, "ttf", alias, fontStyle);
                CopyTo(name, alias);
            }
        }

        public static void AddFontBase64(string fontBase64, string alias = null, FontStyle? fontStyle = null)
        {
            byte[] content = global::System.Convert.FromBase64String(fontBase64);
            AddFontBytes(content, alias, fontStyle);
        }

        public static void AddMemoryFont(IntPtr memory, int length)
        {
            lock (lockObject)
            {
                //Instance.AddMemoryFont(memory, length);

                byte[] content = new byte[length];
                Marshal.Copy(memory, content, 0, length);

                string name = AddContentToCache(content, null, "ttf");
            }
        }

        public static List<FontFamily> GetFontFamilies()
        {
            lock (lockObject)
            {
                var fonts = FontFamily.Families.ToList();

                //fonts.AddRange(instance.Families);
                foreach (var fs in fontsVHash.Values)
                {
                    FontV fv = fs.Regular ?? fs.Bold ?? fs.Italic ?? fs.BoldItalic;
                    if (fv != null)
                        fonts.Add(fv.FontFamily);
                }

                var listResult = fonts.OrderBy(f => f.Name).ToList();

                if (GenerateFonts != null)
                {
                    var e = new StiGenerateFontsEventArgs(listResult);
                    GenerateFonts.Invoke(null, e);

                    return e.Fonts;
                }

                return listResult;
            }
        }

        public static List<FontFamily> GetFontFamiliesFirstCustom()
        {
            lock (lockObject)
            {
                var listResult = new List<FontFamily>();
                var listResult2 = new List<FontFamily>();

                listResult.AddRange(FontFamily.Families.ToList());

                //listResult.AddRange(instance.Families.Where(r => !customFontCountHash.ContainsKey(r.Name)).ToList());
                foreach (var fs in fontsVHash.Values)
                {
                    FontV fv = fs.Regular ?? fs.Bold ?? fs.Italic ?? fs.BoldItalic;
                    if (!customFontCountHash.ContainsKey(fv.FontFamily.Name))
                        listResult.Add(fv.FontFamily);
                    else
                        listResult2.Add(fv.FontFamily);
                }

                listResult = listResult.OrderBy(x => x.Name).ToList();

                listResult.InsertRange(0, listResult2.OrderBy(x => x.Name).ToList());

                if (GenerateFonts != null)
                {
                    var e = new StiGenerateFontsEventArgs(listResult);
                    GenerateFonts.Invoke(null, e);

                    return e.Fonts;
                }

                return listResult;
            }
        }

        public static FontFamily GetFontFamily(string fontName, FontStyle baseFontStyle = FontStyle.Regular, bool allowNullResult = false)
        {
            lock (lockObject)
            {
                if (fontsVHash.ContainsKey(fontName))
                {
                    FontVStyles fstyles = fontsVHash[fontName];
                    FontStyle fontStyle = baseFontStyle & (FontStyle.Bold | FontStyle.Italic);

                    if (fontStyle == FontStyle.Regular)
                    {
                        FontV fontV = fstyles.Regular ?? fstyles.Bold ?? fstyles.Italic ?? fstyles.BoldItalic;
                        return fontV.FontFamily;
                    }
                    if (fontStyle == FontStyle.Bold)
                    {
                        FontV fontV = fstyles.Bold ?? fstyles.Regular ?? fstyles.Italic ?? fstyles.BoldItalic;
                        return fontV.FontFamily;
                    }
                    if (fontStyle == FontStyle.Italic)
                    {
                        FontV fontV = fstyles.Italic ?? fstyles.Regular ?? fstyles.BoldItalic ?? fstyles.Bold;
                        return fontV.FontFamily;
                    }
                    if (fontStyle == (FontStyle.Bold | FontStyle.Italic))
                    {
                        FontV fontV = fstyles.BoldItalic ?? fstyles.Bold ?? fstyles.Italic ?? fstyles.Regular;
                        return fontV.FontFamily;
                    }
                }

                var fontFamily = fontFamilyHash[fontName] as FontFamily;
                if (fontFamily != null) return fontFamily;

                using (var font = new Font(fontName, 1f, baseFontStyle))
                {
                    if ((font.Name != fontName) && allowNullResult)
                        return null;

                    fontFamily = font.FontFamily;
                }

                fontFamilyHash[fontName] = fontFamily;
                return fontFamily;
            }
        }

        public static bool IsCustomFont(string fontName)
        {
            //if (Instance.Families.Any(f => f.Name == fontName)) return true;
            if (fontName == null) return false;
            if (fontsVHash.ContainsKey(fontName)) return true;
            return false;
        }

        public static bool IsStyleAvailable(string fontName, FontStyle style)
        {
            if (!fontsVHash.ContainsKey(fontName)) return false;

            style = style & (FontStyle.Bold | FontStyle.Italic);

            FontVStyles fs = fontsVHash[fontName];
            if (style == FontStyle.Regular)
            {
                return fs.Regular != null;
            }
            if (style == FontStyle.Bold)
            {
                return fs.Bold != null;
            }
            if (style == FontStyle.Italic)
            {
                return fs.Italic != null;
            }
            if (style == (FontStyle.Bold | FontStyle.Italic))
            {
                return fs.BoldItalic != null;
            }

            return false;
        }

        public static string GetCustomFontPath(string fontName, FontStyle baseFontStyle = FontStyle.Regular)
        {
            var fstyles = fontsVHash[fontName];
            if (fstyles == null) return null;

            string path = null;
            FontStyle fontStyle = baseFontStyle & (FontStyle.Bold | FontStyle.Italic);

            if (fontStyle == FontStyle.Regular)
            {
                path = (fstyles.Regular ?? fstyles.Bold ?? fstyles.Italic ?? fstyles.BoldItalic).FilePath;
            }
            if (fontStyle == FontStyle.Bold)
            {
                path = (fstyles.Bold ?? fstyles.Regular ?? fstyles.Italic ?? fstyles.BoldItalic).FilePath;
            }
            if (fontStyle == FontStyle.Italic)
            {
                path = (fstyles.Italic ?? fstyles.Regular ?? fstyles.BoldItalic ?? fstyles.Bold).FilePath;
            }
            if (fontStyle == (FontStyle.Bold | FontStyle.Italic))
            {
                path = (fstyles.BoldItalic ?? fstyles.Bold ?? fstyles.Italic ?? fstyles.Regular).FilePath;
            }

            if (!path.StartsWith("http://") && !path.StartsWith("https://") && !path.StartsWith("ftp:") && !path.StartsWith("file:///")) path = "file:///" + path;

            return path;
        }

        public static byte[] GetCustomFontData(string fontName, FontStyle baseFontStyle = FontStyle.Regular)
        {
            if (!fontsVHash.ContainsKey(fontName)) return null;

            var fstyles = fontsVHash[fontName];
            FontStyle fontStyle = baseFontStyle & (FontStyle.Bold | FontStyle.Italic);

            if (fontStyle == FontStyle.Regular)
            {
                return (fstyles.Regular ?? fstyles.Bold ?? fstyles.Italic ?? fstyles.BoldItalic).Content;
            }
            if (fontStyle == FontStyle.Bold)
            {
                return (fstyles.Bold ?? fstyles.Regular ?? fstyles.Italic ?? fstyles.BoldItalic).Content;
            }
            if (fontStyle == FontStyle.Italic)
            {
                return (fstyles.Italic ?? fstyles.Regular ?? fstyles.BoldItalic ?? fstyles.Bold).Content;
            }
            if (fontStyle == (FontStyle.Bold | FontStyle.Italic))
            {
                return (fstyles.BoldItalic ?? fstyles.Bold ?? fstyles.Italic ?? fstyles.Regular).Content;
            }
            return null;
        }

        public static Font CreateFont(string fontName, float fontSize, FontStyle fontStyle)
        {
            if (fontSize <= 0)
                fontSize = 0.1f;

            return new Font(GetFontFamily(fontName, fontStyle), fontSize, fontStyle);
        }
        #endregion

        #region Methods.ResourceFont
        public static void AddResourceFont(string name, byte[] content, string extension, string alias)
        {
            if (content == null || content.Length == 0) return;

            string hash = FontV.GetHashName(content);
            string name2 = fontContentToName[hash] as string;
            if (name2 == null)
            {
                lock (lockObject)
                {
                    name2 = AddContentToCache(content, null, extension, alias, null, true);
                }
            }
            IncreaseCount(name2);

            CopyTo(name2, name);    //resource name
            CopyTo(name2, alias);   //resource alias
        }

        public static FontFamily GetFontFamilyByContent(string name, byte[] content, string extension, string alias)
        {
            if (content == null || content.Length == 0) return null;

            string hash = FontV.GetHashName(content);
            string name2 = fontContentToName[hash] as string;

            if (name2 == null)
            {
                lock (lockObject)
                {
                    name2 = AddContentToCache(content, null, extension, alias, null, true);
                    IncreaseCount(name2);
                }
            }

            FontStyle fontStyle = FontStyle.Regular;
            if (fontsVHash.ContainsKey(name2))
            {
                FontVStyles fstyles = fontsVHash[name2];
                if (fstyles.Bold != null && EqualBytes(fstyles.Bold.Content, content))
                {
                    fontStyle = FontStyle.Bold;
                }
                else if (fstyles.Italic != null && EqualBytes(fstyles.Italic.Content, content))
                {
                    fontStyle = FontStyle.Italic;
                }
                else if (fstyles.BoldItalic != null && EqualBytes(fstyles.BoldItalic.Content, content))
                {
                    fontStyle = FontStyle.Bold | FontStyle.Italic;
                }
            }

            //return fontFamilyHash[name2] as FontFamily;
            return GetFontFamily(name2, fontStyle);
        }
        private static bool EqualBytes(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }

        private static void IncreaseCount(string name)
        {
            lock (lockObject)
            {
                int count = 1;
                object objCount = customFontCountHash[name];
                if (objCount != null)
                {
                    count = Convert.ToInt32(objCount);
                    count++;
                }
                customFontCountHash[name] = count;
            }
        }

        public static void RemoveResourceFont(string name, byte[] content)
        {
            if (content == null || content.Length == 0) return;

            string hash = FontV.GetHashName(content);
            string name2 = fontContentToName[hash] as string;
            if (name2 == null) return;

            lock (lockObject)
            {
                object objCount = customFontCountHash[name2];
                if (objCount == null) return;
                int count = Convert.ToInt32(objCount);
                if (count > 0) count--;
                customFontCountHash[name2] = count;
            }
        }

        //public static void RemoveUnusedResourceFonts()
        //{
        //    lock (lockObject)
        //    {
        //        ArrayList fonts1 = new ArrayList();
        //        ArrayList fonts2 = new ArrayList();
        //        foreach (DictionaryEntry de in fontVCountHash)
        //        {
        //            int count = Convert.ToInt32(de.Value);
        //            if (count == 0)
        //            {
        //                fonts2.Add(de.Key);
        //                var fontV = de.Key as FontV;
        //                if (fontV != null)
        //                {
        //                    foreach (DictionaryEntry de2 in fontVHash)
        //                    {
        //                        if (de2.Value == fontV) fonts1.Add(de2.Key);
        //                    }
        //                }
        //            }
        //        }

        //        foreach (object obj in fonts1)
        //        {
        //            fontVHash.Remove(obj);
        //        }
        //        foreach (object obj in fonts2)
        //        {
        //            fontVCountHash.Remove(obj);
        //        }
        //    }
        //}

        //public static void RemoveAllResourceFonts()
        //{
        //    lock (lockObject)
        //    {
        //        fontVCountHash.Clear();
        //        foreach (DictionaryEntry de in fontVHash)
        //        {
        //            var fontV = de.Value as FontV;
        //            if (fontV != null) fontV.Dispose();
        //        }
        //        fontVHash.Clear();
        //    }
        //}
        #endregion

        #region Events
        public static event StiGenerateFontsEventHandler GenerateFonts;
        #endregion

        public static void AddFontBase64(this PrivateFontCollection collection, string fontBase64)
        {
            collection.AddFontBytes(Convert.FromBase64String(fontBase64));
        }

        public static void AddFontBytes(this PrivateFontCollection collection, byte[] fontData)
        {
#if STIDRAWING
            var isGdi = true;
            isGdi = Graphics.GraphicsEngine == Stimulsoft.Drawing.GraphicsEngine.Gdi;
            if (isGdi)
            {
#endif
                var data = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, data, fontData.Length);
                collection.AddMemoryFont(data, fontData.Length);
                Marshal.FreeCoTaskMem(data);
#if STIDRAWING
            }
            else
            {
                collection.AddFontBytesInternal(fontData);
            }
#endif
        }

        public static void AddFontStream(this PrivateFontCollection collection, Stream fontStream)
        {
            int fontStreamLength = (int)fontStream.Length;
            var fontData = new byte[fontStreamLength];
            fontStream.Read(fontData, 0, fontStreamLength);
            collection.AddFontBytes(fontData);
        }
    }
}
