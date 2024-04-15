#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using Stimulsoft.Base;

namespace Stimulsoft.Report.Web
{
    internal class StiFontNames
    {
        public static ArrayList GetItems(bool allowLoadingCustomFonts)
        {
            var families = StiFontCollection.GetFontFamilies().ToArray();
            var items = new ArrayList();

            foreach (var fontFamily in families)
            {
                var fontName = fontFamily.GetName(0);

                if (!string.IsNullOrEmpty(fontName))
                {
                    var item = new Hashtable();
                    item["value"] = fontName;

                    if (allowLoadingCustomFonts && StiFontCollection.IsCustomFont(fontName))
                    {
                        var fontData = StiFontCollection.GetCustomFontData(fontName);

                        if (fontData != null)
                        {
                            item["data"] = $"{GetFontMimeType(fontData)};base64,{Convert.ToBase64String(fontData)}";
                        }
                    }

                    items.Add(item);
                }
            }

            return items;
        }

        public static ArrayList GetOpenTypeFontItems()
        {
            var families = StiFontCollection.GetFontFamilies().ToArray();
            var items = new ArrayList();

            foreach (var fontFamily in families)
            {
                var fontName = fontFamily.GetName(0);

                if (!string.IsNullOrEmpty(fontName) && StiFontCollection.IsCustomFont(fontName) && !items.Contains(fontName))
                {
                    items.Add(fontName);
                }
            }

            return items;
        }

        private static string GetFontMimeType(byte[] data)
        {
            if (data.Length > 4)
            {
                if (data[0] == 0x00 && data[1] == 0x01 && data[2] == 0x00 && data[3] == 0x00)
                {
                    return "data:application/x-font-ttf";
                }
                else if (data[0] == 0x4F && data[1] == 0x54 && data[2] == 0x54 && data[3] == 0x4F)
                {
                    return "data:application/x-font-opentype";
                }
            }

            return "data:application/font";
        }
    }
}