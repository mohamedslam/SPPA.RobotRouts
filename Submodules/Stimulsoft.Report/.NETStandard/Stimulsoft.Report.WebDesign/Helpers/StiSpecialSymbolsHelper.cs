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
using System.Collections.Generic;

#if !NETSTANDARD
using System.Windows.Media;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiSpecialSymbolsHelper
    {
        public static void GetSpecialSymbols(Hashtable param, Hashtable callbackResult)
        {
            var chars = new List<string>();

#if !NETSTANDARD
            var fontFamily = new FontFamily(param["fontName"] as string);
            var recentStrings = new List<string>();
            GlyphTypeface glyph;
            IDictionary<int, ushort> characterMap;

            var typefaces = fontFamily.GetTypefaces();
            foreach (var typeface in typefaces)
            {
                typeface.TryGetGlyphTypeface(out glyph);
                if (glyph != null)
                {
                    characterMap = glyph.CharacterToGlyphMap;

                    foreach (var key in characterMap.Keys)
                    {
                        try
                        {
                            char c = Convert.ToChar(key);
                            if (c == '\0' || c == '\r' || c == ' ') continue;

                            string text = c.ToString();
                            chars.Add(text);
                        }
                        catch { }
                    }

                    if (chars.Count > 0)
                        break;
                }
            }
#endif

            callbackResult["symbols"] = chars;
        }
    }
}