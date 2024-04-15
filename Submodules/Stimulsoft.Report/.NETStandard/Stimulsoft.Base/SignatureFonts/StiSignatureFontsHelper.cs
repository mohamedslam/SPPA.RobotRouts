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

using Stimulsoft.Base;
using Stimulsoft.Base.SignatureFonts;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Text;

#if STIDRAWING
using PrivateFontCollection = Stimulsoft.Drawing.Text.PrivateFontCollection;
using FontFamily = Stimulsoft.Drawing.FontFamily;
#endif

namespace Stimulsoft.Report.SignatureFonts
{
    public static class StiSignatureFontsHelper
    {
        #region Fields
        private static FontFamily fontFamilyStyle1;
        private static FontFamily fontFamilyStyle2;
        private static FontFamily fontFamilyStyle3;
        private static Hashtable familyToFilename = new Hashtable();
        private static Hashtable pfcCache = new Hashtable();
        #endregion

        #region Methods
        public static FontFamily GetFont(StiSignatureStyle style)
        {
            switch (style)
            {
                case StiSignatureStyle.Style1:
                    {
                        if (fontFamilyStyle1 == null)
                            fontFamilyStyle1 = GetFontByName("TeddyBear.ttf");

                        return fontFamilyStyle1;
                    }

                case StiSignatureStyle.Style2:
                    {
                        if (fontFamilyStyle2 == null)
                            fontFamilyStyle2 = GetFontByName("MADELikesScript.otf");

                        return fontFamilyStyle2;
                    }

                case StiSignatureStyle.Style3:
                    {
                        if (fontFamilyStyle3 == null)
                            fontFamilyStyle3 = GetFontByName("Denistina.ttf");

                        return fontFamilyStyle3;
                    }
            }

            return null;
        }


        public static bool IsSignatureFont(string name)
        {
            return familyToFilename.ContainsKey(name);
        }

        public static byte[] GetFontDataByFamilyName(string name)
        {
            string fileName = familyToFilename[name] as string;
            if (fileName == null) return null;

            using (var fontStream = typeof(StiSignatureFontsHelper).Assembly.GetManifestResourceStream($"Stimulsoft.Base.SignatureFonts.{fileName}"))
            {
                if (fontStream == null)
                    return null;

                using (MemoryStream ms = new MemoryStream())
                {
                    fontStream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        private static FontFamily GetFontByName(string name)
        {
            var pfc = pfcCache[name] as PrivateFontCollection;
            if (pfc != null) return pfc.Families[0];

            using (var fontStream = typeof(StiSignatureFontsHelper).Assembly.GetManifestResourceStream($"Stimulsoft.Base.SignatureFonts.{name}"))
            {
                if (null == fontStream)
                    return null;

                pfc = new PrivateFontCollection();
                pfcCache[name] = pfc;
                pfc.AddFontStream(fontStream);
                var family = pfc.Families[0];

                familyToFilename[family.Name] = name;

                return family;
            }
        }
        #endregion
    }
}