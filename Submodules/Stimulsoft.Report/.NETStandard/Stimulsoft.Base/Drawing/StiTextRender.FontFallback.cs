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
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Base.Drawing
{
    public partial class StiTextRenderer
    {
        #region Class StiMeasureRichTextBox
        /// <summary>
        /// Represents a Windows rich text box control, with some impovements.
        /// </summary>
        [ToolboxItem(false)]
        [SuppressUnmanagedCodeSecurity]
        internal class StiMeasureRichTextBox : RichTextBox
        {
            #region LoadLibrary
            private static IntPtr moduleHandle;
            private static bool failLoadModule = false;
            private static string lastCheckedClassName = null;

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            static extern IntPtr LoadLibrary(string lpFileName);

            private static bool TryLoadLibrary()
            {
                if (!failLoadModule && moduleHandle == IntPtr.Zero)
                {
                    try
                    {
                        moduleHandle = LoadLibrary("msftedit.dll");
                    }
                    catch
                    {
                        failLoadModule = true;
                    }
                }
                return moduleHandle != IntPtr.Zero;
            }

            private void CheckClassName()
            {
                if (lastCheckedClassName != StiBaseOptions.ExtendedRichTextLibraryClassName)
                {
                    try
                    {
                        lastCheckedClassName = StiBaseOptions.ExtendedRichTextLibraryClassName;
                        this.Rtf = string.Empty;
                        this.Rtf = null;
                    }
                    catch (Win32Exception)
                    {
                        if (StiBaseOptions.ExtendedRichTextLibraryClassName.ToLowerInvariant() == "richedit20w")
                        {
                            StiBaseOptions.ForceLoadExtendedRichTextLibrary = false;
                        }
                        else
                        {
                            if (StiBaseOptions.ExtendedRichTextLibraryClassName.ToLowerInvariant() == "richedit50w")
                            {
                                StiBaseOptions.ExtendedRichTextLibraryClassName = "RichEdit20W";
                            }
                            else
                            {
                                StiBaseOptions.ExtendedRichTextLibraryClassName = "RichEdit50W";
                            }
                        }
                    }
                }
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams prams = base.CreateParams;
                    if (StiBaseOptions.ForceLoadExtendedRichTextLibrary.HasValue && StiBaseOptions.ForceLoadExtendedRichTextLibrary.Value == false) return prams;
                    //if (StiBaseOptions.ForceLoadExtendedRichTextLibrary.GetValueOrDefault(false))
                    {
                        if (TryLoadLibrary())
                        {
                            prams.ClassName = StiBaseOptions.ExtendedRichTextLibraryClassName;
                        }
                    }
                    return prams;
                }
            }
            #endregion

            public StiMeasureRichTextBox()
            {
                CheckClassName();
            }
        }
        #endregion

        #region Fields
        private static Hashtable CacheFontNameToNum = new Hashtable();
        private static Hashtable CacheNumToFontName = new Hashtable();
        private static Hashtable CacheFontSymbolsFallback = new Hashtable();
        #endregion

        #region Methods
        internal static List<StiHtmlState> CheckMissingGlyphs(StiHtmlState state)
        {
            if (StiFontCollection.IsCustomFont(state.TS.FontName)) return null;

            string txt = state.Text.ToString();
            if (string.IsNullOrWhiteSpace(txt)) return null;

            Font baseFont = new Font(state.TS.FontName, 8);

            ushort[] glyphs = (ushort[])GetFontGlyphs(baseFont);

            //get fontNumber
            int fontNum = GetFontNumber(baseFont.Name);

            //get fallbackTable
            byte[] fallbackTable = null;
            var objFallbackTable = CacheFontSymbolsFallback[fontNum];
            if (objFallbackTable != null)
            {
                fallbackTable = (byte[])objFallbackTable;
            }
            else
            {
                fallbackTable = new byte[65536];
                lock (CacheFontSymbolsFallback)
                {
                    CacheFontSymbolsFallback[fontNum] = fallbackTable;
                }
            }

            List<StiHtmlState> states = new List<StiHtmlState>();

            int lastFontNum = fontNum;
            StringBuilder sb = new StringBuilder();
            StiMeasureRichTextBox measureRichTextBox = null;
            for (int indexChar = 0; indexChar < txt.Length; indexChar++)
            {
                int ch = txt[indexChar];
                string st;

                //support of Surrogates and Supplementary Characters; D800-DBFF, DC00-DFFF. 
                if (((ch & 0xD800) == 0xD800) && (indexChar + 1 < txt.Length) && ((txt[indexChar + 1] & 0xDC00) == 0xDC00))
                {
                    st = txt.Substring(indexChar, 2);
                    ch = ((ch & 0x03FF) << 10) | (txt[indexChar + 1] & 0x03FF) + 0x10000;
                    indexChar++;
                    if (fallbackTable.Length == 0x10000)
                    {
                        byte[] newTable = new byte[0x10000 * 17];
                        Array.Copy(fallbackTable, newTable, 0x10000);
                        fallbackTable = newTable;
                        lock (CacheFontSymbolsFallback)
                        {
                            CacheFontSymbolsFallback[fontNum] = fallbackTable;
                        }
                    }
                }
                else
                {
                    st = txt.Substring(indexChar, 1);
                }

                int fallbackFont = fallbackTable[ch];
                if (fallbackFont == 0)
                {
                    bool processed = false;
                    if ((glyphs != null) && (ch < glyphs.Length))
                    {
                        int glyph = glyphs[ch];
                        if (glyph != 0xFFFF)
                        {
                            fallbackFont = fontNum;
                            processed = true;
                        }
                    }

                    if (!processed)
                    {
                        if (measureRichTextBox == null) measureRichTextBox = new StiMeasureRichTextBox();
                        string testFontName = GetFontNameOfSymbol(st, baseFont, measureRichTextBox);
                        if (testFontName == null)
                        {
                            fallbackFont = fontNum;
                        }
                        else
                        {
                            fallbackFont = GetFontNumber(testFontName);
                        }
                    }

                    fallbackTable[ch] = (byte)fallbackFont;
                }

                if ((lastFontNum != fallbackFont) && (ch == 0x20 || ch == 0xA0))    //ignore spaces
                {
                    fallbackFont = lastFontNum;
                }

                if (lastFontNum != fallbackFont)
                {
                    if (sb.Length > 0)
                    {
                        StiHtmlState newState = new StiHtmlState(state);
                        newState.TS.FontName = (string)CacheNumToFontName[lastFontNum];
                        newState.Text = new StringBuilder(sb.ToString());
                        states.Add(newState);
                        sb.Clear();
                    }
                    lastFontNum = fallbackFont;
                }
                sb.Append(st);
            }
            if (measureRichTextBox != null) measureRichTextBox.Dispose();

            if ((states.Count > 0) || (lastFontNum != fontNum))
            {
                if (sb.Length > 0)
                {
                    StiHtmlState newState = new StiHtmlState(state);
                    newState.TS.FontName = (string)CacheNumToFontName[lastFontNum];
                    newState.Text = sb;
                    states.Add(newState);
                }
                return states;
            }
            return null;
        }

        private static int GetFontNumber(string fontName)
        {
            var objFontNum = CacheFontNameToNum[fontName];
            if (objFontNum != null)
            {
                return (int)objFontNum;
            }
            else
            {
                lock (CacheFontNameToNum)
                {
                    int fontNum = CacheFontNameToNum.Count + 1;
                    CacheFontNameToNum[fontName] = fontNum;
                    CacheNumToFontName[fontNum] = fontName;
                    return fontNum;
                }
            }
        }

        private static string GetFontNameOfSymbol(string st, Font baseFont, StiMeasureRichTextBox measureRichTextBox)
        {
            measureRichTextBox.Text = st;
            measureRichTextBox.SelectAll();
            measureRichTextBox.SelectionFont = baseFont;
            measureRichTextBox.Select(0, 1);
            Font font = measureRichTextBox.SelectionFont;
            if (font.Name != baseFont.Name) return font.Name;
            return null;
        }

        private static string ConvertSymbolsToTags(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText)) return inputText;

            inputText = inputText.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

            var sb = new StringBuilder();

            if ((inputText.IndexOf('\r') == -1 && inputText.IndexOf('\n') == -1))
            {
                sb.Append(inputText);
            }
            else
            {
                for (var index = 0; index < inputText.Length; index++)
                {
                    var ch = inputText[index];
                    if (ch == '\r' || ch == '\n')
                    {
                        if (index + 1 < inputText.Length)
                        {
                            var ch2 = inputText[index + 1];
                            if ((ch2 == '\r' || ch2 == '\n') && (ch2 != ch))
                                index++;
                        }
                        sb.Append("<br>");
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }
            }

            return sb.ToString();
        }
        #endregion
    }
}
