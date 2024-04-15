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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Stimulsoft.Report;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Import;
using System.Security;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using ImageCodecInfo = Stimulsoft.Drawing.Imaging.ImageCodecInfo;
using ImageEncoder = Stimulsoft.Drawing.Imaging.Encoder;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using EncoderParameter = Stimulsoft.Drawing.Imaging.EncoderParameter;
using EncoderParameters = Stimulsoft.Drawing.Imaging.EncoderParameters;
#else
using ImageEncoder = System.Drawing.Imaging.Encoder;
#endif

namespace Stimulsoft.Report.Export
{
    [SuppressUnmanagedCodeSecurity]
	public sealed class StiExportUtils
    {
        #region ConvertDigitsToArabic
        public static string ConvertDigitsToArabic(string outputString, StiArabicDigitsType digitsType)
		{
			StringBuilder sb = new StringBuilder(outputString);
			sb = ConvertDigitsToArabic(sb, digitsType);
			return sb.ToString();
		}

		public static StringBuilder ConvertDigitsToArabic(StringBuilder outputString, StiArabicDigitsType digitsType)
		{
			for (int index = 0; index < outputString.Length; index++)
			{
				int num = (int)outputString[index];
				if ((num >= 0x0030) && (num <= 0x0039))
				{
					num += 0x0660 - 0x0030;
					if (digitsType == StiArabicDigitsType.Eastern)
					{
						num += 0x06f0 - 0x0660;
					}
					outputString[index] = (char)num;
				}
			}
			return outputString;
		}
        #endregion

        #region TrimEndWhiteSpace
        public static string TrimEndWhiteSpace(string inputString)
        {
            return TrimEndWhiteSpace(inputString, true);
        }

        public static string TrimEndWhiteSpace(string inputString, bool removeControl)
		{
            if (StiOptions.Engine.MeasureTrailingSpaces || !StiOptions.Export.Text.TrimTrailingSpaces)
			{
				return inputString;
			}
			else
			{
				string outputString = string.Empty;
				int index = inputString.Length;
                while ((index > 0) && (char.IsWhiteSpace(inputString[index - 1])) && (removeControl || ((int)inputString[index - 1] >= 32)))
				{
					index--;
				}
				if (index == inputString.Length)
				{
					outputString = inputString;
				}
				else
				{
					if (index > 0)
					{
						outputString = inputString.Substring(0, index);
					}
				}
				return outputString;
			}
		}
        #endregion

        #region SplitString
        public static List<string> SplitString(string inputString, bool removeControl)
		{
			var stringList = new List<string>();
            if (inputString == null) inputString = string.Empty;

            var st = new StringBuilder();
            foreach (char ch in inputString)			// !!! foreach or for ???
            {
                if (ch == '\n')
                {
                    stringList.Add(TrimEndWhiteSpace(st.ToString(), removeControl));
                    st.Length = 0;
                }
                else
                {
                    if (!(removeControl && (char.IsControl(ch)) && (ch != '\t')))
                    {
                        st.Append(ch);
                    }
                }
            }
            if (st.Length > 0) stringList.Add(TrimEndWhiteSpace(st.ToString(), removeControl));
            if (stringList.Count == 0) stringList.Add(string.Empty);

			return stringList;
		}

        public static string[] SplitString(string inputString, char delimiter)
        {
            var stringList = new List<string>();
            var st = new StringBuilder();
            bool flag = false;
            char quote = ' ';
            foreach (char ch in inputString)
            {
                if (flag)
                {
                    if (ch == quote)
                    {
                        flag = false;
                    }
                    st.Append(ch);
                }
                else
                {
                    if (ch == delimiter)
                    {
                        stringList.Add(st.ToString());
                        st = new StringBuilder();
                    }
                    else
                    {
                        if (ch == '\"' || ch == '\'')
                        {
                            quote = ch;
                            flag = true;
                        }
                        st.Append(ch);
                    }
                }
            }
            if (st.Length > 0) stringList.Add(st.ToString());
            //if (stringList.Count == 0) stringList.Add(string.Empty);
            return stringList.ToArray();
        }
        #endregion

        #region StringToUrl
        public static string StringToUrl(string input)
		{
			UTF8Encoding enc = new UTF8Encoding();
			byte[] buf = enc.GetBytes(input);
			StringBuilder output = new StringBuilder();
			foreach (byte byt in buf)
			{
                if ((byt < 0x20) || (byt > 0x7f) || (wrongUrlSymbols.IndexOf((char)byt) != -1))
				{
					output.Append(string.Format("%{0:x2}", byt));
				}
				else
				{
					output.Append((char)byt);
				}
			}
			return output.ToString();
		}
		//                                         space "   #   %   &   '   *   ,   :   ;   <   >   ?   [   ^   `   {   |   }   
		//private static string wrongUrlSymbols = "\x20\x22\x23\x25\x26\x27\x2a\x2c\x3a\x3b\x3c\x3e\x3f\x5b\x5e\x60\x7b\x7c\x7d";
		private static string wrongUrlSymbols = "\x20\x22\x27\x2a\x2c\x3b\x3c\x3e\x5b\x5e\x60\x7b\x7c\x7d";
        #endregion

        public static MemoryStream MakePdfDeflateStream(byte[] data)
        {
            MemoryStream outputStream = new MemoryStream();

            //outputStream.WriteByte(0x78);
            //outputStream.WriteByte(0x9c);
            //DeflateStream deflateStream = new DeflateStream(outputStream, CompressionMode.Compress, true);
            DeflaterOutputStream deflateStream = new DeflaterOutputStream(outputStream);
            deflateStream.IsStreamOwner = false;

            deflateStream.Write(data, 0, data.Length);
            deflateStream.Close();
            return outputStream;
        }

        #region GetReportVersion
        private static string reportVersion = null;

        public static string GetReportVersion(StiReport report = null)
        {
            bool isDashboards = false;

            if ((report != null) && (report.RenderedPages?.Count > 0))
            {
                var obj = report.RenderedPages.GetPageWithoutCache(0).TagValue;
                isDashboards = (obj as string) == "*Dashboards*" || obj is Base.Drawing.StiAdvancedWatermark;
            }

            if (reportVersion == null)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(StiReport));
                Version version = assembly.GetName().Version;

                reportVersion = string.Format("{0}.{1}.{2} from {3:D}, {4}",
                    version.Major.ToString(),
                    version.Minor.ToString(),
                    version.Build.ToString(),
                    StiVersion.CreationDate,
                    GetFrameworkVersion());
            }

            return string.Format("Stimulsoft {0} {1}", isDashboards ? "Dashboards" : "Reports", reportVersion);
        }

        private static string GetFrameworkVersion()
        {
            var frameworkName = Assembly.GetEntryAssembly()?.GetCustomAttribute<global::System.Runtime.Versioning.TargetFrameworkAttribute>()?.FrameworkName;
            if (!string.IsNullOrEmpty(frameworkName) && frameworkName.IndexOf("=v") > 0)
            {
                var version = frameworkName.Substring(frameworkName.IndexOf("=v") + 2);
                return $".NET{(frameworkName.StartsWith(".NETCoreApp") && (version.StartsWith("2") || version.StartsWith("3")) ? " Core" : "")} {version}";
            }

            return ".NET";
        }
        #endregion

        #region Correct images in RichText for Riched20
        public static void CorrectRichTextForRiched20(StiRichText rich)
        {
            //string st = CorrectRichTextImagesOrNull(rich.RtfText);
            //if (st != null)
            //{
            //    rich.RtfText = st;
            //}

            string baseText = rich.RtfText;

            string st = Export.StiExportUtils.CorrectRichTextImagesOrNull(baseText);
            if (st != null)
            {
                string st2 = Export.StiExportUtils.CorrectEncoding(st);
                if (st2 != null)
                {
                    rich.RtfText = st2;
                }
                else
                {
                    rich.RtfText = st;
                }
            }
            else
            {
                string st2 = Export.StiExportUtils.CorrectEncoding(baseText);
                if (st2 != null)
                {
                    rich.RtfText = st2;
                }
            }
        }

        public static string CorrectRichTextForRiched20(string baseText)
        {
            string st = Export.StiExportUtils.CorrectRichTextImagesOrNull(baseText);
            if (st != null)
            {
                string st2 = Export.StiExportUtils.CorrectEncoding(st);
                if (st2 != null) return st2;
                return st;
            }
            else
            {
                string st2 = Export.StiExportUtils.CorrectEncoding(baseText);
                if (st2 != null) return st2;
                return baseText;
            }
        }

        private static readonly Regex RegexRemoveWhitespace = new Regex(@"\s+");

        /// <summary>
        /// Correct images in RichText for Riched20
        /// </summary>
        /// <param name="baseText">Base RTF string</param>
        /// <returns>The modified string, or null if no changes</returns>
        private static string CorrectRichTextImagesOrNull(string baseText)
        {
            string st = baseText;
            var sbNewImage = new StringBuilder();
            bool modified = false;
            try
            {
                int pos = 0;
                int newPos = -1;
                int oldPos = pos;
                while ((newPos = st.IndexOf("{\\pict", pos, StringComparison.InvariantCulture)) != -1)     //"{\\*\\shppict{\\pict"
                {
                    pos = newPos + 6;       //17 for shppict

                    #region Parse rtf
                    var rtf = new StringReader(st.Substring(pos, 1024));
                    var lex = new StiRtfCorrectionHelper(rtf);
                    int level = 0;

                    int picwgoal = -1;
                    int pichgoal = -1;
                    int picscalex = -1;
                    int picscaley = -1;
                    string blip = null;
                    int bliptag = -1;
                    int newPos2 = 0;

                    StiRtfCorrectionHelper.StiRtfToken tok = lex.NextToken();
                    while (tok.Type != StiRtfCorrectionHelper.StiRtfTokenType.Eof)
                    {
                        if (tok.Type == StiRtfCorrectionHelper.StiRtfTokenType.GroupStart) level++;
                        if (tok.Type == StiRtfCorrectionHelper.StiRtfTokenType.GroupEnd) level--;
                        if (level == 0)
                        {
                            switch (tok.Type)
                            {
                                case StiRtfCorrectionHelper.StiRtfTokenType.Keyword:
                                    if (tok.Key == "picwgoal") picwgoal = tok.Parameter;
                                    if (tok.Key == "pichgoal") pichgoal = tok.Parameter;
                                    if (tok.Key == "picscalex") picscalex = tok.Parameter;
                                    if (tok.Key == "picscaley") picscaley = tok.Parameter;
                                    if (tok.Key == "bliptag") bliptag = tok.Parameter;
                                    if (tok.Key.EndsWith("blip")) blip = tok.Key;
                                    break;

                                case StiRtfCorrectionHelper.StiRtfTokenType.Control:
                                    //
                                    break;

                                case StiRtfCorrectionHelper.StiRtfTokenType.Text:
                                    newPos2 = pos + tok.Position;
                                    break;
                            }
                            if (tok.Type == StiRtfCorrectionHelper.StiRtfTokenType.Text) break;
                        }
                        tok = lex.NextToken(level > 0);
                    }
                    #endregion

                    if ((bliptag == -1) && (blip == "jpegblip" || blip == "pngblip") && (picwgoal != -1) && (pichgoal != -1))
                    {
                        #region read image data
                        MemoryStream ms = new MemoryStream();
                        while (st[newPos2] != '}')
                        {
                            int num = (int)st[newPos2];
                            if ((num >= (int)'0' && num <= (int)'9') || (num >= (int)'a' && num <= (int)'f'))
                            {
                                ms.WriteByte((byte)ParseHexTwoCharToInt(st[newPos2], st[newPos2 + 1]));
                                newPos2 += 2;
                            }
                            else
                            {
                                newPos2 += 1;
                            }
                        }
                        pos = newPos2 + 1;  //2 for shppict
                        ms.Seek(0, SeekOrigin.Begin);
                        Bitmap bmp = new Bitmap(ms);
                        #endregion

                        #region make new image
                        Metafile mf = null;
                        using (Bitmap bmpTemp = new Bitmap(1, 1))
                        using (Graphics grfx = Graphics.FromImage(bmpTemp))
                        {
                            IntPtr ipHdc = grfx.GetHdc();
                            mf = new Metafile(ipHdc, EmfType.EmfOnly);
                            grfx.ReleaseHdc(ipHdc);
                        }
                        using (Graphics grfx = Graphics.FromImage(mf))
                        {
                            grfx.DrawImageUnscaledAndClipped(bmp, new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1));
                        }
                        bmp.Dispose();

                        IntPtr _hEmf = mf.GetHenhmetafile();
                        uint _bufferSize = GdipEmfToWmfBits(_hEmf, 0, null, MM_ANISOTROPIC, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
                        byte[] _buffer = new byte[_bufferSize];
                        GdipEmfToWmfBits(_hEmf, _bufferSize, _buffer, MM_ANISOTROPIC, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
                        DeleteEnhMetaFile(_hEmf);
                        mf.Dispose();
                        #endregion

                        if (newPos > 50)
                        {
                            int posShppict = st.IndexOf("shppict", newPos - 50, 50);
                            if (posShppict > 0)
                            {
                                int posShppict2 = st.LastIndexOf("{", posShppict, 20);
                                if (posShppict2 > 0)
                                {
                                    string stShp = st.Substring(posShppict2, newPos - posShppict2);
                                    string stShp2 = RegexRemoveWhitespace.Replace(stShp, "");
                                    if (stShp2 == "{\\*\\shppict")
                                    {
                                        newPos = posShppict2 + 1;
                                    }
                                }
                            }
                        }

                        sbNewImage.Append(st.Substring(oldPos, newPos - oldPos));

                        #region write new image
                        sbNewImage.Append("{\\pict");
                        if (picwgoal > 0) sbNewImage.Append("\\picwgoal" + picwgoal.ToString());
                        if (pichgoal > 0) sbNewImage.Append("\\pichgoal" + pichgoal.ToString());
                        if (picscalex > 0) sbNewImage.Append("\\picscalex" + picscalex.ToString());
                        if (picscaley > 0) sbNewImage.Append("\\picscaley" + picscaley.ToString());
                        sbNewImage.Append("\\wmetafile8\r\n");
                        
                        sbNewImage.Append(ByteArrayToHex(_buffer));

                        sbNewImage.Append("}\r\n");
                        #endregion

                        oldPos = pos;

                        modified = true;
                    }
                }
                if (modified) sbNewImage.Append(st.Substring(oldPos));
            }
            catch
            {
                modified = false;
            }
            if (modified)
            {
                return sbNewImage.ToString();
            }
            return null;
        }
        
        public static string CorrectEncoding(string st)
        {
            if (string.IsNullOrEmpty(st)) return null;

            Encoding enc = Encoding.Default;
            try
            {
                //check for encoding
                int posCpg = st.IndexOf("\\ansicpg", StringComparison.InvariantCulture);
                if (posCpg > 0)
                {
                    posCpg += 8;
                    int posEnd = posCpg;
                    while ((posEnd < st.Length) && (char.IsDigit(st, posEnd))) posEnd++;
                    string codeSt = st.Substring(posCpg, posEnd - posCpg);
                    int codePage = 0;
                    int.TryParse(codeSt, out codePage);
                    if (codePage != 0)
                    {
                        enc = Encoding.GetEncoding(codePage);
                    }
                }
            }
            catch
            {
            }
            byte[] buf = enc.GetBytes(st);

            bool flag = false;
            foreach (byte ch in buf)
            {
                if (ch > 127)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag) return null;

            StringBuilder sb = new StringBuilder();
            foreach (byte ch in buf)
            {
                if (ch > 127)
                {
                    sb.AppendFormat("\\'{0:x2}", ch);
                }
                else
                {
                    sb.Append((char)ch);
                }
            }
            return sb.ToString();
        }

        private static string[] hexStrings = null;
        internal static string GetHexString(byte bb)
        {
            if (hexStrings == null)
            {
                hexStrings = new string[256];
                for (int index = 0; index < 256; index++)
                {
                    hexStrings[index] = index.ToString("x2");
                }
            }
            return hexStrings[bb];
        }

        #region Win32 constants and methods
        /// <summary>
        /// Mapping Modes
        /// </summary>
        private const int MM_TEXT = 1;
        private const int MM_LOMETRIC = 2;
        private const int MM_HIMETRIC = 3;
        private const int MM_LOENGLISH = 4;
        private const int MM_HIENGLISH = 5;
        private const int MM_TWIPS = 6;
        private const int MM_ISOTROPIC = 7;
        private const int MM_ANISOTROPIC = 8;

        /// <summary>
        /// Specifies the flags/options for the unmanaged call to the GDI+ method Metafile.EmfToWmfBits().
        /// </summary>
        private enum EmfToWmfBitsFlags
        {
            // Use the default conversion
            EmfToWmfBitsFlagsDefault = 0x00000000,

            // Embedded the source of the EMF metafiel within the resulting WMF metafile
            EmfToWmfBitsFlagsEmbedEmf = 0x00000001,

            // Place a 22-byte header in the resulting WMF file.  The header is required for the metafile to be considered placeable.
            EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,

            // Don't simulate clipping by using the XOR operator.
            EmfToWmfBitsFlagsNoXORClip = 0x00000004
        };

        [DllImport("gdiplus.dll")]
        private static extern uint GdipEmfToWmfBits(IntPtr _hEmf, uint _bufferSize, byte[] _buffer, int _mappingMode, EmfToWmfBitsFlags _flags);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteEnhMetaFile(IntPtr hEmf);
        #endregion

        #endregion

        #region ByteArrayToHex
        private static readonly uint[] _lookup32 = CreateLookup32();

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
            return result;
        }

        public static string ByteArrayToHex(byte[] bytes)
        {
            int newLen = bytes.Length * 2 + (bytes.Length / 120) * 2;
            var result = new char[newLen];
            int pos = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                var val = _lookup32[bytes[i]];
                result[pos++] = (char)val;
                result[pos++] = (char)(val >> 16);
                if (i % 120 == 0 && i > 0)
                {
                    result[pos++] = (char)0x0D;
                    result[pos++] = (char)0x0A;
                }
            }
            return new string(result);
        }
        #endregion

        private static int ParseHexTwoCharToInt(char c1, char c2)
        {
            return ParseHexCharToInt(c1) * 16 + ParseHexCharToInt(c2);
        }
        private static int ParseHexCharToInt(char ch)
        {
            if (ch <= '9') return (int)ch - (int)'0';
            if (ch == 'a') return 10;
            if (ch == 'b') return 11;
            if (ch == 'c') return 12;
            if (ch == 'd') return 13;
            if (ch == 'e') return 14;
            if (ch == 'f') return 15;
            return 0;
        }

        #region ChecksumAdler32
        public sealed class ChecksumAdler32
        {
            private const uint MOD_ADLER = 65521;
            private uint checksum;

            public long Value
            {
                get
                {
                    return checksum;
                }
            }

            public ChecksumAdler32()
            {
                checksum = 1;
            }

            public void Update(byte[] buf, int off, int len)
            {
                uint s1 = checksum & 0xFFFF;
                uint s2 = checksum >> 16;
                while (len > 0)
                {
                    int n = len > 5550 ? 5550 : len;
                    len -= n;
                    while (n > 0)
                    {
                        s1 += buf[off++];
                        s2 += s1;
                        n--;
                    }
                    s1 %= MOD_ADLER;
                    s2 %= MOD_ADLER;
                }
                checksum = (s2 << 16) | s1;
            }
        }
        #endregion

        internal static uint GetAdler32Checksum(byte[] buff)
        {
            Adler32 adler32 = new Adler32();
            adler32.Update(buff);
            return (uint)adler32.Value;
        }

        #region ClearTypeFix
        public static void DisableFontSmoothing(StiReport report)
        {
            if (!StiOptions.Export.DisableClearTypeDuringExport || !StiOptions.Engine.FullTrust) return;
            if (report == null || report.FlagFontSmoothing) return;
            report.FlagFontSmoothing = true;
            DisableFontSmoothingInternal();
        }
        private static void DisableFontSmoothingInternal()
        {
            lock (lockClearType)
            {
                if (processCounter == 0)
                {
                    try
                    {
                        if (!GetFontSmoothing()) return;
                        processCounter++;
                        int pv = 0;
                        /* Call to systemparametersinfo to set the font smoothing value. */
                        //bool iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 0, ref pv, SPI_UPDATEINIFILE);
                        bool iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 0, ref pv, SPI_UPDATEINIFILE | SPI_SENDCHANGE);   //fix 2013.08.14
                    }
                    catch
                    {
                        if (processCounter == 0) StiOptions.Export.DisableClearTypeDuringExport = false;
                    }
                }
                else
                {
                    processCounter++;
                }
            }
        }

        public static void EnableFontSmoothing(StiReport report)
        {
            if (!StiOptions.Export.DisableClearTypeDuringExport || !StiOptions.Engine.FullTrust || (report == null)) return;
            if (!(report.FlagFontSmoothing || (report.CompiledReport != null && report.CompiledReport.FlagFontSmoothing))) return;
            report.FlagFontSmoothing = false;
            EnableFontSmoothingInternal();
        }
        public static void EnableFontSmoothingInternal()
        {
            lock (lockClearType)
            {
                if (processCounter == 0) return;
                processCounter--;
                if (processCounter == 0)
                {
                    try
                    {
                        int pv = 0;
                        /* Call to systemparametersinfo to set the font smoothing value. */
                        bool iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 1, ref pv, SPI_UPDATEINIFILE | SPI_SENDCHANGE);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static object lockClearType = new object();
        private static int processCounter = 0;

        private static Boolean GetFontSmoothing()
        {
#if !STIDRAWING
            bool iResult;
            int pv = 0;
            /* Call to systemparametersinfo to get the font smoothing value. */
            iResult = SystemParametersInfo(SPI_GETFONTSMOOTHING, 0, ref pv, 0);

            int pv2 = 0;
            /* Call to systemparametersinfo to get the font smoothing type. */
            iResult = SystemParametersInfo(SPI_GETFONTSMOOTHINGTYPE, 0, ref pv2, 0);
            if ((pv > 0) && (pv2 == FE_FONTSMOOTHINGCLEARTYPE))
            {
                return true;
            }
            else
#endif
            {
                return false;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);

        /* Constants used for User32 calls. */
        const uint SPI_GETFONTSMOOTHING = 74;
        const uint SPI_SETFONTSMOOTHING = 75;
        const uint SPI_UPDATEINIFILE = 0x1;
        const uint SPI_SENDCHANGE = 0x2;
        const uint SPI_GETFONTSMOOTHINGTYPE = 0x200A;
        const uint FE_FONTSMOOTHINGSTANDARD = 1;
        const uint FE_FONTSMOOTHINGCLEARTYPE = 2;

#endregion

#region SaveComponentToString
        public static string SaveComponentToString(StiComponent component)
        {
            return SaveComponentToString(component, ImageFormat.Png, 0.75f, 100);
        }

        public static string SaveComponentToString(StiComponent component, ImageFormat imageFormat, float imageQuality, float imageResolution)
        {
            Image image = null;
            float zoom = imageResolution / 100f;

            //DisableFontSmoothing();

            if (component is StiTextInCells)
            {
                image = (component as StiTextInCells).GetImage(ref zoom, StiExportFormat.ImagePng);
            }
            else if (component is StiRichText)
            {
                StiRichText richText = (component as StiRichText).Clone() as StiRichText;
                richText.Border = new Stimulsoft.Base.Drawing.StiBorder();
                image = richText.GetImage(ref zoom, StiExportFormat.ImagePng);
            }
            else if (component is StiText)
            {
                StiText text = (component as StiText).Clone() as StiText;
                text.Border = new Stimulsoft.Base.Drawing.StiBorder();
                image = text.GetImage(ref zoom, StiExportFormat.Pdf);
            }
            else if (component is StiImage)
            {
                image = (component as StiImage).GetImage(ref zoom, StiExportFormat.ImagePng);
            }

            else if (component is StiBarCode)
            {
                StiBarCode barcode = (component as StiBarCode).Clone() as StiBarCode;
                barcode.Border = new Stimulsoft.Base.Drawing.StiBorder();
                image = barcode.GetImage(ref zoom, StiExportFormat.ImagePng);
            }

            else if (component is StiShape)
            {
                image = (component as StiShape).GetImage(ref zoom, StiExportFormat.ImagePng);
            }
            else if (component is StiCheckBox)
            {
                StiCheckBox checkBox = (component as StiCheckBox).Clone() as StiCheckBox;
                checkBox.Border = new Stimulsoft.Base.Drawing.StiBorder();
                image = checkBox.GetImage(ref zoom, StiExportFormat.ImagePng);
            }
            else if (component is StiZipCode)
            {
                StiZipCode zipCode = (component as StiZipCode).Clone() as StiZipCode;
                zipCode.Border = new Stimulsoft.Base.Drawing.StiBorder();
                image = zipCode.GetImage(ref zoom, StiExportFormat.ImagePng);
            }
            else if (component is StiChart)
            {
                StiChart chart = (component as StiChart).Clone() as StiChart;
                chart.Border = new Stimulsoft.Base.Drawing.StiBorder();
                image = chart.GetImage(ref zoom, StiExportFormat.ImagePng);
            }
            EnableFontSmoothing(component.Report);

            if (image == null) return string.Empty;

            MemoryStream mem = new MemoryStream();
            if (imageFormat != ImageFormat.Jpeg)
            {
                image.Save(mem, imageFormat);
            }
            else
            {
                ImageCodecInfo imageCodec = Stimulsoft.Base.Drawing.StiImageCodecInfo.GetImageCodec("image/jpeg");
                if (imageCodec == null)
                {
                    image.Save(mem, imageFormat);
                }
                else
                {
                    EncoderParameters imageEncoderParameters = new EncoderParameters(1);
                    imageEncoderParameters.Param[0] = new EncoderParameter(ImageEncoder.Quality, (long)(imageQuality * 100));

                    image.Save(mem, imageCodec, imageEncoderParameters);
                }
            }

            mem.Seek(0, SeekOrigin.Begin);

            return Convert.ToBase64String(mem.ToArray());
        }
#endregion

#region Additional retail data
        internal static string AdditionalData = 
            "iVBORw0KGgoAAAANSUhEUgAAAd8AAAGNCAYAAABQRnukAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAhdEVYdENyZWF0aW9uIFRpbWUAMjAxNzowMjowNSAwMjoyODo0OXhS4H0AABHRSURBVHhe7d0LktvIsYbRkZcy+1/TbEVuWA1Pq0USIFC" +
            "PzKxzIhzWDYct4hH/10Xp2j9+fvgLABjmP5//DAAMIr4AMJj4AsBg4gsAg4kvAAwmvgAwmPgCwGDiCwCDiS8ADCa+ADCY+ALAYOILAIOJLwAMJr4AMJj4AsBg4gsAg4kvAAwmvgAwmPgCfPPPP//87x/Qi/gCfPE1ugJML+IL8OlRbAWYHsQX4MOryAowrYkvsLwzcRVgWhJfYGnvRFWAaUV8gW" +
            "VdiakA04L4Aku6E1EB5i7xBZbTIp4CzB3iCyylZTQFmKvEF1hGj1gKMFeIL7CEnpEUYN4lvkB5I+IowLxDfIHSRkZRgDlLfIGyZsRQgDlDfIGSZkZQgDkivkA5EeInwLwivkApkaInwDwjvkAZEWMnwDwivgCdCTDfiS9Qxt9///35q3gEmK/EFyhFgMlAfIFyBJjoxBcoSYCJTHyBsgSYqMQXK" +
            "E2AiUh8gfIEmGjEF1iCABOJ+ALLEGCiEF9gKQJMBOILLEeAmU18gSUJMDOJL7AsAWYW8QWWJsDMIL7A8gSY0cQX4IMAM5L4AnwSYEYRX4AvBJgRxBfgGwGmN/EFeECA6Ul8AZ4QYHoRX4AXBJgexBfggADTmvgCnCDAtCS+ACcJMK2IL8AbBJgWxBfgTQLMXeILcIEAc4f4AlwkwFwlvgA3CDBX" +
            "iC/ATQLMu8QXoAEB5h3iC9CIAHOW+AI0JMCcIb4AjQkwR8QXoAMB5hXxBehEgHlGfAE6EmAeEV+AzgSY78QXYAAB5ivxBRhEgNmJL8BAAsxGfAEGE2DEF2ACAV6b+AJMIsDrEl+AiQR4TeILMJkAr0d8AQIQ4LWIL0AQArwO8QUIRIDXIL4AwQhwfeILEJAA1ya+AEEJcF3iCxCYANckvgDBCXA" +
            "94guQgADXIr4ASQhwHeILkIgA1yC+AMkIcH4/fn74/DVAKY9CEDlc74ocukr3uQfxBUq4GqLskRDgnMQXSKt1eLLGQoDzEV8glRGhyRgMAc5FfIEURsdFgNsS4N+JLxDezKhki4YA5yC+QFhRQiLA7QjwL/7/fIGQIgUkcsweiRy4bPeyFydfIBSntnbcy7jEFwgjy6koUzgEOCZfOwMhZPo6Mt" +
            "Nn9RV0TE6+wFSZB9gJuI0VT8BOvsA02U8+TsBtrHgCFl9giiqDK8BtrBZg8QWGqza0AtzGSgEWX2CoqgMrwG2sEmDxBYapPqwC3MYKARZfYIhVTjQC3Eb190V8ge5WCe9OgNuo/N6IL9DVauHdCXAbVd8f8QW6WTW8OwFuo+J7JL5AF6uHdyfAbVR7n8QXaE54fyfAfCe+QFPC+5gA31PthwLxB" +
            "ZoR3tcE+JqKp3HxBZoQ3nME+D0Vw7sRX4DBBPicquHd+N/zBW6bGZNXAx09cpniMvpeVg7vRnyBW2YF7p1xjhxhAf5T9fBuxBe4bEbU7gxz1AgL8L9WCO/Gn/kCadwd5qjDHvlk/l3Pe7hKeDfiC1wyOhithlmA7+txD1cK70Z8gbdlDe9OgO9reQ9XC+9GfIGwtlHuNcwCfF+Le7hieDfiC7xl" +
            "VBxWHeXNKgFe+RmLLxDOyqO8qx7g1Z+x+AKnZQrCkQzXUjXAfrgSXyAYw/y7agH2fH8RX+CUEREYNczZTvBVAiy8/xJfIATD/Fr2AHu+vxNf4FC2k+Irma8la4CF90/iC0xnnM/LFmDP9jHxhTdVOgWupsqz8w7mJ77whn30jF87TkbXeAdzE1846fvYrTJ+Va6z4vMS4LzEF054NnLGL4fKz8k" +
            "7mJP4woGjcTN+zOYdzEd84YWzo2b84mr9bKL+GbV3MBfxhSfeHTPjF0+v8EYOsPcwB/GFB64OmOFbR+S/pe09jE984Zu7w2X4Ymj9HB7FVoC5Snzhi1aDZfjO63GvRoR3J8BcIb7wqfVQGb51RA+wdzEe8YUPvcbJ6I3X+p6fDWvkAG9EOBbxZXm9B8ngjTP7XkcP8Mb7GIP4srRRQ2TwXot6f6" +
            "7ENEuAvZNziS/LGj0+xq6v1vf3TkQzBHizR9i7OZ74sqRZY2PkHrsbK/f1PhEe68fPD5+/hiVEGJgsJ6Ndz3vW4l60/nytnk/2mGV7TzNx8mUpUcYw+yi3EjG8LWWOl/D2Jb4sQ/Di2IY96ri3/lwZIya8/YkvS4gW3mzj1vLztvzPav1cRcc9GEV8WUKkQVl13Lbrbnntmb7JyPLMV303ZxBfl" +
            "hFhWFYdtwzXvXp4hHcs8WUpMwdmxXHbrrnHdfu6ua3Vr38G8WU5M4amwri9ew29rtlfnGtLeOcQX5Y0cnBWHLdM1zzqs0a8Jyu+m1GIL8saMTzVxu3oerZ/vec1O/W2U+3dzEZ8WVrPAVpt3DJe76oBEt75xJfl9RiiyuP2/dq2/3vE9fpLVm2set3RiC98aDlIrcct8lethjwXzysO8YVPLYap" +
            "V3ijBXi7zpFD7s967xPeWMQXvrgzUL3CuxOgdmaEaObzE954xBe+uTJUvcO7WzHAfuj49X5dfceENybxhQfeGaxR4d2J0T2ZY7R99pnvJu2ILzxxZrhaj9vZsK4SYD9oPHYmwsIbm/jCC68GbFZ4d8KUR69n9SzCwhuf+MKBEeN2dZwFmM3XCAtvDuILJ3wdtCjh3VUNcJXrunsd77xvwpuH+MJ" +
            "J27BFC++uaoChKvGFSVoHU4Dj8Ux4Rnxhgl6jXGXse16HIBKB+MJgvcdfXGLwHHhFfGGgUYNs+Ody/zkivjDI6EEWgOey3Bt/e7ku8YUBZo29AD/X696455whvtDZ7DEWg+da3pvtP8u95izxhY6ijLEoPNfi3vS4v75yrk18oZNowRPg57Z7c+X+XP33wY+fHz5/DTQSeZAznKgi3L9X92nE53" +
            "PyrU18oRMBvm7106Tw1udrZ+gk8oCuHjeYTXyhIwG+ZuWTn1PvGsQXOhNg4DvxhQEEmDOcetchvjCIAAM78YWBBPi81U6BTr1rEV8YTIAB8YUJBPicVU6DTr3rEV+YRIDZCO+axBcmEuBjleMkvOsSX5hMgGE9/rud4aTvIWodzcihi/ADQrUfBJx61ya+8MKrwe8xngL8WpUACy++doYHtpE/G" +
            "voeIYg8ylXCN5vwshFf+OJMdHsT4Oeyh0t42YkvfLoSll4xEuDnsgZMePlKfFneFpPZQXlEgJ/LFjLh5Tt/4YqltYpIz3GN+IPBbnZUIt+bjejyjJMvy4o+3LvIA+4E/Jzw8or4sqQs4d0J8HPbvYl2f4SXI752Zjm9YjFicCP/0BAhOE7iZCG+LKXnOI8aXgE+NvoeiS7v8rUzy+g9yKMGP/LQ" +
            "R/nBYOQ9El6ucPJlGSPCMHKIo4TukWhBan2vBJe7xJcljAyVAP8SNVBX75ng0pL4Ut7oQI0eaQGGfPyZLzRWPfbviPyDAcwkvpS2yvgLMOQivtDBjOAIMOQhvlCIAEMO4gudzIqNAEN84ktZKw+9AENs4gsdzQyNAENc4gsdzQ6gAENM/ks2KGvWuEcMXuTQRf4BAXoR3wXsw7vayI0MToZ7K8A" +
            "Qh6+di/s6uJHHN7Ms4Yj8Ob2brEZ8C3s0aEaunS1m2U5sAgwxiG9Rr4aswsjNvoZs0f1KgGE+8S3ozIBlHrn9sx9dQ6/IZA7vToBhLvEt5p3hyjhy3z/z6GuoEN6dAMM84lvIlcHKNHLPPuuoa6gU3p0AwxziW8Sdocowckefsfc1VAzvToBhPPEtoMVARR65s5/NUF8nwDCW+CbXcpgijty7n6" +
            "nHNVQ+9X4lwDCO+CbWY5AijdzVz2KorxNgGEN8k+o5RBFG7u5n2P/9d2Oyyqn3KwGG/sQ3oREDNHPkWv3ehvo6AYa+xDeZkcMzY+Ra/56G+joBhn7EN5EKMXzFoMYjwNCH+CYxc2hG/N6GNC4BhvbEN4EIA9PzMxjQ+AQY2hLf4KIMS6/xNZx5CDC0I76BCe98Rv13UQMc+QcDeER8gxJeoooWO" +
            "uElI/ENSHiJLkrwhJesxDeY6uHNyA8Lj81+R7yjZCa+gawSXqNZx6xn6R0iO/ENYrUTr/GsY/Sz9O5QgfgGsOpXzZlG1FfPr416lsJLFeI72arh3RnTOno/S+8KlYjvRKuHd5dlVJ1+j/V6lsJLNeI7ifD+ToDraP0shZeKxHcC4X3MyNbR6ll6J6hKfAcT3tcyjK3T7zl3n6XwUpn4DiS85whw" +
            "HVefpfBSnfgOIrzvEeA63n2WwssKxHcA4b3GCNdx9ll65qxCfDsT3nuif26n3/OOnqXwshLx7Uh42xDgOp49S+FlNeLbifC2JcB1fH+WwsuKxLcD4e1DgOvYn6XwsqofPz98/poGhLe/6JETFOCIk29DwjtG9OtzAgaOiG8jwjuW0yWQmfg2ILxzRL5ep1/gFfG9SXjnEmAgI/G9QXhjEGAgG/G" +
            "9SHhjEWAgE/G9QHhjEmAgC/F9k/DGJsBABuL7BuHNQYCB6MT3pEijacCPCTAQmfieEHEsDfgxAQaiEt8DkUfSgB8TYCAi8X0hwzga8GMCDEQjvk9kGkUDfkyAgUjE94GMY2jAjwkwEIX4fpN5BA34MQEGIhDfLyqMnwE/JsDAbOL7qdLoGfBjAgzMJL4fKo6dAT8mwMAsy8e38sgZ8GMCDMywdH" +
            "xXGDcDfkyAgdGWje9Ko2bAjwkwMNKS8V1xzAz4MQEGRlkuviuPmAE/JsDACEvF13i5B2cIMNDbMvE1Wv9yL44JMNDTEvE1Vn9yT44JMNBL+fgaqefcm2MCDPRQOr7G6Zh7dEyAgdbKxjfKKEUe7p0BPybAQEsl4xstvAJcgwADrZSLb7Tw7gS4BgEGWigV36jh3QlwDQIM3FUmvtHDuxPgGgQYu" +
            "KNEfLOEdyfANQgwcFX6+GYL706AaxBg4IrU8c0a3p0A1yDAwLvSxjd7eHcCXEOG5wjEkTK+VcK7E+AaIj5HPxRATOniWy28OwGuIdJzFF6IK1V8q4Z3J8A1RHiOwguxpYlv9fDuBLiGmc9ReCG+tH/haoZRoybANcx4jsILOaSJ7+xRGf37C3ANI5+j8EIeqU6+s8Zltd/3HQJ8bMRzFF7IJd3X" +
            "zqNHZvaoCXANPZ+j8EI+Kf/Md9TYRBk1Aa6hx3MUXsgp7V+46j060UZNgGto+RyFF/JKG99Nr/GJOmoCXEOL5yi8kFvq+G5aj1D0URPgGu48R+GF/NLHd9NqjLKMmgDXcOU5Ci/UUCK+m7ujlG3UBLiGd56j8EIdZeK7uTpOWUdNgGs48xyFF2opFd/NuyOVfdQEuIZXz1F4oZ5y8d2cHasqoyb" +
            "ANTx6jsILNZWM7+ZotKqNmgDX8PU5Ci/U9ePnh89fl/Ro8CuPWobAiQqwurIn3933oa8+/BmuzwkYWF35+G72IK1y4hJggNjKf+28Ml9BA8S0xMl3VU7AADGJb3ECDBCP+C5AgAFiEd9FCDBAHOK7EAEGiEF8FyPAAPOJ74IEGGAu8V2UAAPMI74LE2CAOcR3cQIMMJ74IsAAg4kv/yPAAOOIL/" +
            "8nwABjiC+/EWCA/sSXPwgwQF/iy0MCDNCP+PKUAAP0Ib68JMAA7YkvhwQYoC3x5RQBBmhHfDlNgAHaEF/eIsAA94kvbxNggHvEl0sEGOA68eUyAQa4Rny5RYAB3ie+3CbAAO8RX5oQYIDzxJdmBBjgHPGlKQEGOCa+NCfAAK+JL10IMMBz4ks3AgzwmPjSVYYAA4wmvnQXOcB+OABmEF+GiBg54" +
            "QVmEV+GiRQ74QVmEl+GihA94QVmE1+Gmxk/4QUiEF+mmBFB4QWiEF+mGRlD4QUiEV+mGhFF4QWiEV+m6xlH4QUiEl9C6BFJ4QWiEl/CaBlL4QUiE19CaRFN4QWiE1/CuRNP4QUyEF9CuhJR4QWyEF/CeiemwgtkIr6EdiaqwgtkI76E9yquwgtkJL6k8CiywgtkJb6k8TW2wgtk9uPnh89fAwAD" +
            "OPkCwGDiCwCDiS8ADCa+ADCY+ALAYOILAIOJLwAMJr4AMJj4AsBg4gsAg4kvAAwmvgAwmPgCwGDiCwCDiS8ADCa+ADCY+ALAYOILAIOJLwAMJr4AMJj4AsBQf/31X1H6k/ZbMBjlAAAAAElFTkSuQmCC";

        //"Demo"
        //"iVBORw0KGgoAAAANSUhEUgAAAecAAAHECAYAAAADNeLMAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAOF0lEQVR42u3dQW4bSRBFQd//jjyLZz2AMRhLpPplZgTAvfirUc8UZOnXbwAg5ZcJAECcAQBxBgBxBgDEGQDEGQAQZwAQZwBAnAFAnAEAcQYAxBkAxBkAEGcAEGcAQJwBQJwBAHEGAHEGAMQZ" +
        //"gLd7vV5veyHOADwcY7EWZwDiMRZrcQZgSJRFWpwBBHnQC3EGEGWRFmcARFmkxRlAmAVanAEQZZEWZwBRFmnEGUCYBVqcARBmgRZnAFEWaXEGQJgFWpwBhFmgxRkAYRZocQYQZpEWZwCEWaDFGUCUvQRanAGEWaDFGUCcvQRanAGEWaDFGUCYvQRanAGE+a/jJdDiDCDM8VgJtDgDCHM4TAItzgDC" +
        //"HA2SOIszgDhHQyTQ4gwgzNH4CLQ4AwizLc4GWpwBYjGyiUCLM0AoRHYRZ3EGEOZVkRZnAHEWFYEWZwBhtpc4AwizkAi0OANciYztbCvOAMIs0OIMICyX/+yhQIszgDALtDgDiIkwC7Q4A/jUbFtxBhAQYRZocQYQZxuLs0cFQJi3B1qcAQRDnH16FmeAcjDw6VmcAXxqtvvwsxBnQCQE4UygxRlA" +
        //"IMRZnMUZoBYIWoEWZwBxMK5Pz+IMUAsDPj2LM4A4OwtxBhAEcRZncQYQZ+ex/FzEGRADcRZncQYQZ8RZnAHE2ZmIM4AQiLM4izOAODsTcQYQAnF2LuIMIALORZwBRECcnYs4AyyKgECLszgDiLNzEWcAERBncRZngGFxFmhxFmcAn56diTgDCIE4i7M4AwyLs0CLszgDiLMzEWcAIRBocRZnAHF2" +
        //"JuIMIAYCPf88JhBnAHEWZ3EGuB1ngX72LMQZ4A+XrkALtDiLMxC8cMVZoJ84hynEGXjsshVncRZncQaCF61AC/RP7i/OgIt24IX5RJwFWpzFGUhesgIt0PYVZyB4yV6P8/VA21acgegFez3OVwNtU3EG4hesQL88O+IM0LtcxdmzI84AwctVoD07V3cUZyB9uV4P9OZI20+cAZfr2DhvDbTdxBkY" +
        //"Hi2Bfnl+xBmgF6vrcd4SaFuJM7AsVAL98vwc+EeMOAPjAiXQL8+POAO0wiTQ8yJkE3EGlgfpyUvYDu29thBnYGSYBdrfxhZnQJgFesw2V5+bTcQZGB1mge5s4rsG4gwI8+kY1XbxLX1xBgRHoB/ex7fxxRkQGoEObOWsxRkQGIH2OvsnNcUZWBsXGwqzOAPi7BIXTmEWZ0BYBFqYxRkQaJe6QAuz" +
        //"OAOiItBe14gzINC2dGbiDIizQHsJszgDgiLQzkicAXF2+Yu0MIszICQC7UzEGRDo71/CAu0lyuIMxCIi0F7CLM5A8NPz5UCLtDCLM5D99CzQoow4AwKdjIYwI85ALs4CvTvSiDMg0AItyuIMCPQ7L3BRmR9pxBlY9ulZoOdGGnEGBPpcbERZnAEejbNAt0ONOAMCLUQPhxpxBsRZoB88K8QZEGiB" +
        //"BnEGyoF+8h8GAo04A+Is0CDOgEALNOIMiPOH4ifQiDOAQAs04gyIc+FrEGjEGRBogUacAQRaoBFn4F8xEef3RU+gEWfgrTETaIFGnIFoxMT5+7sINOIMfCQaAi3QiDMQDJc4CzTiDESjJdBf30KgEWfgo3EQaIFGnIFopMS59zUJNOIMPj16/wKNOAPVCIizQCPOQPTyF2iBRpwBl75ACzTiDMI8" +
        //"8dIvv1dnhTiDMJ+9+AVaoBFnEObY5V9/f84IcQZhPhkAgRZoxBmEORaACe9NoBFnEOZzEZjwngQacQZhPhcCgRZoxBmEORaDKe9DoBFnEGaBFmiBFmdAmJ8LwqSvX6ARZxDmM5EWaIFGnEGYY2GY9nULNOIMwizQwa9ZoBFnEOYTkfb1CjTiDOJ87Ceitz4fiDMItNfoX/qx7flAnEGgvVb8Tust" +
        //"zwfiDPzufAtze6SnRk2YEWcQ6LWRnhw3YUac4XigN0faL1URZnEGRgd6SqQFWpgRZzgX6G2R9mtJhVmcAYEORtof9hBmcQbWBHpLpK//5S3EGVgY6CmR3vzp+avvAXEGBPr0dwBqzwfiDBwJ9ORIb/m/w8KMOINAr4r0hUCDOINAjwr0pt++JcyIMwj0mkhv+tWYwow4g0CvifS2QIM4cy6KAv33" +
        //"O4gziDN8PIYCLdACjThDMIIC3fzjEwIN4szRMAv0jL+xLM4gzhwLs0ALtEAjzhCOnUB/bwdxBnGGj4RCoL+/g0CDOMPb4yDQAi3QiDMEoybQ79lBnEGcEeZ1F++WAAk0iDPCLNDRHcQZxBlhFmiBFmjEGbZGS6Dfv4M4gzgjzAId3UGgQZwR5vGXr0ALNOIMwhy8dLeGSJxBnBHm0Rfu5hgJNIgz" +
        //"wjz2ohVocUacQZht8+NbCTOIM+Iz8pK9ECZhRpxBmG0V3U6YEWcQZpsJtDAjziDMAj3lvYI4I8wuWYH2zCDOIMy2FGhhRpwRZmEW6Ph7BnFGmF2yAh163yDOiIcwC3TofYM4IxrCLNCh9w7ijFgIs0CH3j+IMyIhzAIdev8gzoiDMAt0aAMQZ0RBmAU6tAOIM2IgzAId2gLEGREQZoF2ViDOCDMC" +
        //"DeKMS98l76ycHYgzwlzdwpkJNIgzwhzcwtkJNIgzwhzcwhkKNIgzwhzcwlkKNIgzwhzcwpkKNIgzwhzcwtkKNIgzwhzcwhkLNIgzwhzcwlkLNIgzwhzcwpkLNIgzwhzcwtkLNIgzwhzcwjMg0CDOCHNwC8+CQIM4I8zBLTwTAg3ijDAHt/BsCDSIM8JsC4EGccalK8wbgiPMIM6IkS3sJMyIM8Is" +
        //"zLao7QXijDALs+CEdgNxRpiFWXBC+4E4I8zCLMyhHUGcEWZhFubQniDOCLMwC3NoVxBnhFmYhTm0LyDOLkthFubQzoA4uyQPRskG3b0BcXY5HoyTMHd3B8TZpXgwUsLc3R8QZ5fiwUALc/dZBMTZpXgw0MLcfRYBcXYpHgy0MHefRUCcORhoYQbEGYEOxUyYfYoFcUagQ1ET5q9tBYgzAv2RMAjz" +
        //"97YCxBmBfmsYhPk9WwHijEC/JQzC/N6tAHFGoL8VBmH+zFaAOCPQXwqDMH/2fAFxRqD/KgzC/DPnCogzAv2/wiDMP3uegDgj0P8ZBmF+5hwBcUag/xgGYX72/ABxRqDP/w3q4rkB4oxAC3PwvABxRqCFOXhOgDgj0KITOx9AnBFowRFmEGe4FmjnIcwgzgi0MAszIM4ItDALM4gzAi3MwgyIMwIt" +
        //"zHYCcYaBgbazMIM4I9DCLMyAOCPQwizMIM4QD7Q9hRnEGYEWZmEGxBmB3hQdG4E4g0ALMyDOCLRACzMgzgh0PEa2AMQZgRZmQJz56gUt0LvjJMyAOC+7iAV69nbCDIjzgQvYHnP2EmZAnA8GyD7dnTwLgDgfjY4fgmru4+wBcT4e5fpFfW0bYQbEWZT9tHJoF2EGxFmYR13g2zcRZkCcRdl/Kwrt" +
        //"IcyAOAuzQIf2EGZAnIV5xQW/ZQthBsRZlAU6tIUwA+IszH6DVmgHYQbEWZgFOrSDMAPiLMwCHdpBmAFxFmaBDu0gzIA4C/PJSFc3EGZAnEVZoEMbCDMgzsIs0IfPCxBnYfYSaDsD4uyCFw7nJ8zA6TiLqkDbFRBnYRYS5ynMgDgLs0DbERBnYR7/6yAFWpgBcT51cV8KkEALMyDO6Qv7aoQEWpgB" +
        //"cc5d1EIk0MIMiPOxME+JkUALMyDOj17Ml/9BItDCDIizMA8Kkj2EGTgcZxetQE/YA+BMnF20ojRhC4AzcXbRCvSELQDOxNlFOyvQnheA5XF2yQr0hC0AzsRZmAV6whYAZ+IszPMDfWELgDNxFmaBnrAFwJk4C/O+SG/cAeBMnIV5Z6C37QBwJs7CLNATdgA4E2dhFucJWwCcibMwC/SELQDOxFmY" +
        //"bwV66hYAZ+IsDgI9YQuAM3EWhbuBnrQFwJk4C4JAT9gC4EychUCgy2fimQHOxVkEBNq5AITi7AeQxNm5AITi7Hc7C7QzAQjF2d8WFmjnASDOgiDQAOIs0OLsLACGxVmgBdo5AATjLNDi7AwAgnEWaIF2BgDBOAu0QNsfwB++EAhxBhBngRZouwMMjbNAi7PNAXEWaLEQZwBxFmiBtjXA0DgLtE/P" +
        //"AOIs0KIhzgDiLNACbWOAoXEWaHEGEGeBFhBxBhBngXYWNgXEWRTERJwBxFmgxdmWAIvjLNDiDCDOAi0sP7Q7gDgLhbiIM4A4C7S9xRngQJwFesfWAOIsGgLtUzOAOAu0fcUZ4GCcBVqYAcRZoM/Exz4A4izQh7YEEGeBFmifmgHEWaDnxsgeAOIs0MIMIM4CLdBP7QXA8TgLtDADiLNAj42UOAOI" +
        //"s0ALM4A4I9BPbgKAOAu0MAOIs0DPipcwA4izQIci5qfTAcRZoEMhE2YAcRboUNCEGUCcBToUNv+XG0CcBToUN2EGEGeBDkVOmAHEWaBDsRNmAHEW6FD0hBlAnAU6FD9hBhBngQ5FUJgBxFmgQzEUZgBxFuhQoIUZQJwFOhRoYQYQZ4EOBVqYAcRZoGOBFmYAceZooAEQZ4EWZgBxRqCFGUCcBVqY" +
        //"AcQZgRZmAHEWaGEGQJwFWpgBxJklgQZAnAVamAHEGYEWZgBxFmhhBkCcBVqYAcSZgYEGQJwFWpgBEGeBFmYAcSYeaADEmVCgARBnQoEGQJwJBRoAcSYUaADEmVCgARBnQoEGQJwJBRoAcSYUaADEmVCgARBnQoEGQJwJBRoAcSYUaADEmVCgARBnQoEGQJwJBRoAcQYAxBkAxBkAEGcAEGcAQJwB" +
        //"AHEGAHEGAMQZAMQZABBnABBnAECcAUCcAQBxBgDEGQDq/gFkt351M0YCNAAAAABJRU5ErkJggg==";

        internal static string AdditionalData2 =
             "iVBORw0KGgoAAAANSUhEUgAAAMgAAAA0CAYAAADPCHf8AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAACYpJREFUeNrsXU1sG0UUHq9Tp06ctE6cmKQ1TVPaBiqRFqlFFE5VewMhKsQFIXJGnHqigAQcKnHixJ2iqOKAxI/gVhGBUFNoJUSKCi1" +
            "t04S0dpzEceps7GTt9bITzarj9czurL3rXUvvSaO09np/5r3vve+9eWOHEEjL5a2L9yX9z4g+wj7eRmlyYv8D0Ia1SDAFvkjSZ3BgyYIaACBBjB5R/U/M59vI6dGjDNoAgASRWiV9vo0tHRyroA0ASBClXx87gFoBQEDY1Gp3AKjVFmgDAALUql7KQK0AIEGV3UCtAC" +
            "Ag7OjRSXIPP2VNjx4l0IZzCcEUeA6QJ/U/nbYHqpUwUlX310bUMkKZ2aJ+7qrA0enJC2eh/EtJB0yBp+DoEwKHpoVQpewNBVtJqzo4dgocuQ7gAIoVTGpVVrwBx0ZBQ5sbmsCRVchRACCtlgExCqRKSKu6r4eqbvP5rCp49DJEDwBIK6MHrlpFBagVQhUl4slNrGaqq" +
            "CqEj6IODij/AkBaBg5Ml/qEDvYKHCVZQ8V1kaQcH5MGrQFAWilinbqYWlU9ola5DFArl4RVxYrrY5c+uilFy/rY1Ece+ycb42h0xRife0HguBF9zHHew12yo4zXCxafMcuYPlie/YbdB3sG98Ufzkx17xk/Zem9Jyf219CvZ19+B42/8m5dMr14+xq6/NlETSn+zLmL" +
            "2hOHT/BPXlixpVbz/0xL925MSQ/v/jGk/3evPhSiX5nMldKgDnuJDnaaolSJnNfJWswoct71jO9/1kWbLNEAwWDYx7mpGBkJMnn4YVeamMg6fyoY6nvJaERxEYH7jXPAISLh9aX5YUycfHN3WyUNFVa54Mxl7oamf/g8vLb8X4jSOSLPHCHzNEwMOe8ALHFihBGL+U+" +
            "Sc6XJedtBHkjURIkiNkKAMobc29ewQEBiaYD6SDVJfewk0cT5U0LUqqp6Q2u3qVWaC471/GLo8qWPOyhwWEmUAGXMxmGESURPCTqWCHV80CVLR5A4o+qiEqT3chRfICGNlg1UX0+PkPOb6ZRiuo47Bmjt5bIWHjGGRCpPzUS27aqVRwuCmFpVytw1j5lfvpLKW0Xzyw" +
            "a16rbQsVUE4TEOlaJTUca54w5YA32veYFjkIBNshymTI6lAYJogJgf8Bbl1eNkxKj3FzgcUGYYHgsgsusGKA4Slgw2Sq2EPaJa7theNW8xtcJy/+avEsOY7hBdhsn8JignodjkhHEOONKEftPzM8ywgYRDmq6gxhYzZY6tJUWA1EGhvCY5MVGePBkGvZIFKJFb0iy1Y" +
            "ilF5eRYHlKragipqjetPflstZFPUfOgUjo2HFreRsdJDlXOM5ztAscRJwULM76JxAlNUY7S/UiyhpF7X3AQZigJcV5zN7JVPGonKeSqSNnUGvhknDOvMjFa2SZHiTDomBUFSjPsrBcFXCQqvJgNabQJTu6mNGtYsk0izsqRbMM+3gR14KXXxcqHlW1q5X5yXt7S0Nqy" +
            "UPSI7U4qjOceRY1V7ViGvWJHMBnHhB1cP0xFetbwxFY7KHTvNF0E//sg8QyrqH1KcyzF0RExQlEIFlUwavZ2xt8f6d5lT5m2qVXFG2qVy4hSq1V5LVsggAibdDxG5YR5Fx0R4iTMrMqWSB4SRew1LkPmkLN1FkcRBKN7lnMB7C1G9HHEhUqSH8LqVE1YRI+MQPQQ31/" +
            "uWdVKmFpViZMoER2rHLqVIjpu5Du7RAsuXm3aynvlwCVTCLxD+Kdiwd/H2oE7MqKIYvJGrOqaLKhswU7dStiTTl28CcqmakVz/8kLZ1XKQG8Rh6FydJwkzCHaJro18mJPpIODxjyZoDgjmTMWhxaaDMmtliyqrYYNMozA9qs4A7EJKreoCnbq4k1Q64xcIEucRi/HUR" +
            "j5yayg1xetADazsFziRHcFeVhRlWxuKE15HC+rS60Q89pLzHT/tjV5NzZBybmHzVIrJ5ugMjZJs9H/dpNBUYzWI57XFknczdLdTNykIjw9FC+NRhK8sSwDJOEmHlhtITDoay3ZPKOd1CTuQ8+crKM5mb+nJStqlb19ramH0YoF0ZIuTa1E5miOAZIIh2qxkm27Nh1ei" +
            "V1GARa6Fytl4wU2OJUFxz7Qw2SNFTVKJmUUODmK6pRaJQ4cqwPI/O8/dijrq1xqde/qd3Ur6clDJzSXn7vIoFZRomMrna1yDJsVQUQ6JsyMI8LQD2oHgCTJw42QJJzVmdntQlWC16LSyuQtzThOJHrUVa06Y3E0dOTFGmAV17Kh6S/Oh5RiPQ6vXDwfyv57vXZS+4eR" +
            "Zfu6c+FtghoiOj5IBsuY+xxchxWNU6i+CmY0KMY5eWGgpQM9bmOnH8jooTe8fTcnwdpweD2Rrl235AHnWkbTW9yhkpj51vhr5yqZm1dq3luYmULfvn8mtPfoKRTr37Odd2BgbOTq7fb4G+85ih73ZqZCi3N/cXO/xJ5DSz9//WmZQX9ijGgyTHnxXoZTVC0okEwib4J" +
            "BQxOU84xZ2ILSDgDZa/G+VStF1qGxF1DrFhtXbLhtmjxXqdkwP/DUc9Xjb36kXL/0SY1xKaV1NHv1e8vPjr7wKkodPe3oeoymQ3NEzzHyiKQF6BM2Dg3ZzCMr/whbAMPo4m2LCigGyCyZwF7BqhRdJgwqtcoK3M9KAxGQKWNnJtRoT5/625cfhjEw7CQS7cG7CLWnT7" +
            "/t2kPv6OxC5a3iPGc+5tHjkr0TfYk4NKM3bxDZl3HzyHrLQSABYrQ1Gy3PxpbJqGnCsDE9IpPhlCYFgVp5x3+rqrTv2Blp6PDz2sKfPyF9bOcaNFgwKOKpMZQaP6UdOPkainS5u9Ya6ezK6gBRLOiQTJ7ZYAXmhlRjD4fRdOhEX8b5I9S5jULFJnVeFbWZwFeP8qtWM" +
            "ZLYWgveBFXe6vRkn8faUlVwxXxr8sLZWdCad1UskFpwSEh0A5WPm6AYuQAIAKQlItawh/eXe7EJymZ/ubkgoUePTVAZAKSV1EqsZ8in/eWUlHVwLIPWACCtpFYJQXB4Q63wJiigVgCQgIrYj2wGZBOUHj2KoDJvBX4f5HH0wMDAvVb27TMbj3bp0UNz/SZK65rgJii8" +
            "Ur4CWvNeoMzrFEgffDOAmvuCOTdkHqIHUKwggmNHAMDxCMABAAmqDPt8ffglKABIYKMHbgXv8vk2nGyCAgGAtJRaDfh8G6z95SAAkMBQKz/nym5/OQgAxLfo0RMAapUFagUACSI4wgFIzPH+8jXQBgAkiDIUAGoF7SQ+yv8CDAAQoXsgf0PrrgAAAABJRU5ErkJggg==";

        //    "iVBORw0KGgoAAAANSUhEUgAAAMgAAAA0CAIAAABAauCrAAAACXBIWXMAAAsTAAALEwEAmpwYAAAKT2lDQ1BQaG90b3Nob3AgSUNDIHByb2ZpbGUAAHjanVNnVFPpFj333vRCS4iAlEtvUhUIIFJCi4AUkSYqIQkQSoghodkVUcERRUUEG8igiAOOjoCMFVEsDIoK2AfkIaKOg6OIisr74Xuja9a89+bN/rXXPues852zzwfACAyWSDNRNYAMqUIe" +
        //    "EeCDx8TG4eQuQIEKJHAAEAizZCFz/SMBAPh+PDwrIsAHvgABeNMLCADATZvAMByH/w/qQplcAYCEAcB0kThLCIAUAEB6jkKmAEBGAYCdmCZTAKAEAGDLY2LjAFAtAGAnf+bTAICd+Jl7AQBblCEVAaCRACATZYhEAGg7AKzPVopFAFgwABRmS8Q5ANgtADBJV2ZIALC3AMDOEAuyAAgMADBRiIUpAAR7AGDIIyN4AISZABRG8lc88SuuEOcqAAB4" +
        //    "mbI8uSQ5RYFbCC1xB1dXLh4ozkkXKxQ2YQJhmkAuwnmZGTKBNA/g88wAAKCRFRHgg/P9eM4Ors7ONo62Dl8t6r8G/yJiYuP+5c+rcEAAAOF0ftH+LC+zGoA7BoBt/qIl7gRoXgugdfeLZrIPQLUAoOnaV/Nw+H48PEWhkLnZ2eXk5NhKxEJbYcpXff5nwl/AV/1s+X48/Pf14L7iJIEyXYFHBPjgwsz0TKUcz5IJhGLc5o9H/LcL//wd0yLESWK5" +
        //    "WCoU41EScY5EmozzMqUiiUKSKcUl0v9k4t8s+wM+3zUAsGo+AXuRLahdYwP2SycQWHTA4vcAAPK7b8HUKAgDgGiD4c93/+8//UegJQCAZkmScQAAXkQkLlTKsz/HCAAARKCBKrBBG/TBGCzABhzBBdzBC/xgNoRCJMTCQhBCCmSAHHJgKayCQiiGzbAdKmAv1EAdNMBRaIaTcA4uwlW4Dj1wD/phCJ7BKLyBCQRByAgTYSHaiAFiilgjjggXmYX4" +
        //    "IcFIBBKLJCDJiBRRIkuRNUgxUopUIFVIHfI9cgI5h1xGupE7yAAygvyGvEcxlIGyUT3UDLVDuag3GoRGogvQZHQxmo8WoJvQcrQaPYw2oefQq2gP2o8+Q8cwwOgYBzPEbDAuxsNCsTgsCZNjy7EirAyrxhqwVqwDu4n1Y8+xdwQSgUXACTYEd0IgYR5BSFhMWE7YSKggHCQ0EdoJNwkDhFHCJyKTqEu0JroR+cQYYjIxh1hILCPWEo8TLxB7iEPE" +
        //    "NyQSiUMyJ7mQAkmxpFTSEtJG0m5SI+ksqZs0SBojk8naZGuyBzmULCAryIXkneTD5DPkG+Qh8lsKnWJAcaT4U+IoUspqShnlEOU05QZlmDJBVaOaUt2ooVQRNY9aQq2htlKvUYeoEzR1mjnNgxZJS6WtopXTGmgXaPdpr+h0uhHdlR5Ol9BX0svpR+iX6AP0dwwNhhWDx4hnKBmbGAcYZxl3GK+YTKYZ04sZx1QwNzHrmOeZD5lvVVgqtip8FZHK" +
        //    "CpVKlSaVGyovVKmqpqreqgtV81XLVI+pXlN9rkZVM1PjqQnUlqtVqp1Q61MbU2epO6iHqmeob1Q/pH5Z/YkGWcNMw09DpFGgsV/jvMYgC2MZs3gsIWsNq4Z1gTXEJrHN2Xx2KruY/R27iz2qqaE5QzNKM1ezUvOUZj8H45hx+Jx0TgnnKKeX836K3hTvKeIpG6Y0TLkxZVxrqpaXllirSKtRq0frvTau7aedpr1Fu1n7gQ5Bx0onXCdHZ4/OBZ3n" +
        //    "U9lT3acKpxZNPTr1ri6qa6UbobtEd79up+6Ynr5egJ5Mb6feeb3n+hx9L/1U/W36p/VHDFgGswwkBtsMzhg8xTVxbzwdL8fb8VFDXcNAQ6VhlWGX4YSRudE8o9VGjUYPjGnGXOMk423GbcajJgYmISZLTepN7ppSTbmmKaY7TDtMx83MzaLN1pk1mz0x1zLnm+eb15vft2BaeFostqi2uGVJsuRaplnutrxuhVo5WaVYVVpds0atna0l1rutu6cR" +
        //    "p7lOk06rntZnw7Dxtsm2qbcZsOXYBtuutm22fWFnYhdnt8Wuw+6TvZN9un2N/T0HDYfZDqsdWh1+c7RyFDpWOt6azpzuP33F9JbpL2dYzxDP2DPjthPLKcRpnVOb00dnF2e5c4PziIuJS4LLLpc+Lpsbxt3IveRKdPVxXeF60vWdm7Obwu2o26/uNu5p7ofcn8w0nymeWTNz0MPIQ+BR5dE/C5+VMGvfrH5PQ0+BZ7XnIy9jL5FXrdewt6V3qvdh" +
        //    "7xc+9j5yn+M+4zw33jLeWV/MN8C3yLfLT8Nvnl+F30N/I/9k/3r/0QCngCUBZwOJgUGBWwL7+Hp8Ib+OPzrbZfay2e1BjKC5QRVBj4KtguXBrSFoyOyQrSH355jOkc5pDoVQfujW0Adh5mGLw34MJ4WHhVeGP45wiFga0TGXNXfR3ENz30T6RJZE3ptnMU85ry1KNSo+qi5qPNo3ujS6P8YuZlnM1VidWElsSxw5LiquNm5svt/87fOH4p3iC+N7" +
        //    "F5gvyF1weaHOwvSFpxapLhIsOpZATIhOOJTwQRAqqBaMJfITdyWOCnnCHcJnIi/RNtGI2ENcKh5O8kgqTXqS7JG8NXkkxTOlLOW5hCepkLxMDUzdmzqeFpp2IG0yPTq9MYOSkZBxQqohTZO2Z+pn5mZ2y6xlhbL+xW6Lty8elQfJa7OQrAVZLQq2QqboVFoo1yoHsmdlV2a/zYnKOZarnivN7cyzytuQN5zvn//tEsIS4ZK2pYZLVy0dWOa9rGo5" +
        //    "sjxxedsK4xUFK4ZWBqw8uIq2Km3VT6vtV5eufr0mek1rgV7ByoLBtQFr6wtVCuWFfevc1+1dT1gvWd+1YfqGnRs+FYmKrhTbF5cVf9go3HjlG4dvyr+Z3JS0qavEuWTPZtJm6ebeLZ5bDpaql+aXDm4N2dq0Dd9WtO319kXbL5fNKNu7g7ZDuaO/PLi8ZafJzs07P1SkVPRU+lQ27tLdtWHX+G7R7ht7vPY07NXbW7z3/T7JvttVAVVN1WbVZftJ" +
        //    "+7P3P66Jqun4lvttXa1ObXHtxwPSA/0HIw6217nU1R3SPVRSj9Yr60cOxx++/p3vdy0NNg1VjZzG4iNwRHnk6fcJ3/ceDTradox7rOEH0x92HWcdL2pCmvKaRptTmvtbYlu6T8w+0dbq3nr8R9sfD5w0PFl5SvNUyWna6YLTk2fyz4ydlZ19fi753GDborZ752PO32oPb++6EHTh0kX/i+c7vDvOXPK4dPKy2+UTV7hXmq86X23qdOo8/pPTT8e7" +
        //    "nLuarrlca7nuer21e2b36RueN87d9L158Rb/1tWeOT3dvfN6b/fF9/XfFt1+cif9zsu72Xcn7q28T7xf9EDtQdlD3YfVP1v+3Njv3H9qwHeg89HcR/cGhYPP/pH1jw9DBY+Zj8uGDYbrnjg+OTniP3L96fynQ89kzyaeF/6i/suuFxYvfvjV69fO0ZjRoZfyl5O/bXyl/erA6xmv28bCxh6+yXgzMV70VvvtwXfcdx3vo98PT+R8IH8o/2j5sfVT" +
        //    "0Kf7kxmTk/8EA5jz/GMzLdsAAAAgY0hSTQAAeiUAAICDAAD5/wAAgOkAAHUwAADqYAAAOpgAABdvkl/FRgAAFP5JREFUeNrsXXlYFEfar6runhmGmwGBAEYOwQQj4oEX6npEDGCM8Yi6JjH6rZoYPx+PNfF4NG4eISYmsIkmmiyeiZpdNa5ovBIXYhIVjaKoeHCtigwzMMPcMz3dXd8fo03TM4yzE5JvH5z34Y/u6urqmupfvcev3mogxhj4xJV8" +
        //    "dV5z6a45VE50eMsIwbta++gegS+mhXTW0SN9AHIpF++aT900xIRQNqaDJx6CQGNgQv3QuJ7BnXgAkQ9DzmK1469/0QbJEIQd3zjDYTPNzchQUAT0Aevxkr9f1DQZ7SFyssPdBIRgg84+LCngqShZ5x5DH7DEcr3B8kOVMTqYYrkOhhWEoMXMRARQU/qEdfph9AGrjXAYfHVBI5cgEnW8ncIYtJjZyX1DZRT0Aevxkm/KtQ06e5ic6GhtBQgIG3T2" +
        //    "wYkBfePkj8NI+oDVKrXN9NHr+uggqsNRBSEw2Fi5BE3tG/qYDKaPbmg1gl+WNctISBFQBCyMAUFAkkBe+/IsxzGIXDI2KkgGfMB6vOToNV2V2hqvkIpRBQBFIhvNNreYCeSdgsdWTAYhuvbK7UpO7F1BAGiGs9JMztBkPynpA1ankgad/chVXXSwxNkIIggBhGqtSWe0UuR/DiwMOIQgIkFz7ZZzzTQkAGiDLQLBmnvaF0c/TY3oVG6JD1gAALCz" +
        //    "TMNhLCOhM7AoCjVrLSYLLZdRXtpBUio3qOSMWRopdrAQgiqNaUh617dnDYGwU4WKPucdnLppuKG0RLry2UkSWa2MRmcmCC+NIEdQFG32M6o5wgUu7XbWYmPmTurbyVDlAxZoNrH7y7URgSTALkI5CECT1sywnHe0FoYEAEBuaISYw0hsHAgE7yj144YlP5UQ4XPeO5vsPt/MsFhOEVzbkI+UyKQSUquzmBkok0lZhoYQ8VEiRIigpK3GjqE5lnXW" +
        //    "OhxB+ZmaKKuBJaWgLXIhBM06S7eYsNdeSPdFhd6LwWAIDAz8b/vxl5SgrFafECFvgyqMAYQURVntXLOBpigKcgzArT43BgACCAnB0DF2IKzxwAhKSLvVz6Dm2uoqjDHAWOIfopBHr3ytd6dEFcMwUJiPVVZWdvDgwTNnzuh0utDQ0IyMjIyMjDFjxvj7+zvfvG7duv3793tkbpqbt2/fPmLEiPYqVFZWxsfHy2RtSJ6ysrJ58+YJSxYtWvTyyy+7" +
        //    "edD8+fPPnDnDn8pksqNHjwYHu85Oqam+/fbXlcn9RxK0XogriUxup61/f3dmU8NdhuUIhHqMmNZv4hJdY52jgl+Qoqnu2qlN8/lbMqatSBz4vLGpXqiRWIIK1N6VmnVCdcWxjEweFBIRU3PjkvL2TwHsnXv3lcHBIf3793cMNUV5GiJotdqTJ09WVlZWVVUBAFJSUrp37z548OC4uDj3NxYVFW3atOmR7ffs2XPnzp38qUqlysnJYVn2kTfSNJ2Y" +
        //    "mNg6md58803R806dOgUAiIiIyM3NnTRpUmZmZkBAAHrI5Vy8ePHSpUueDEFiYuLQoUPbu3ro0KG//OUvZWVlonKVSiVqf/78+W6AdevWrU8//dTJO7a3V3/iixPBM1OG5TyvuqtvY6QIgiBQ9cUSO21zlOiUtZTcr9U3omSMzdx8p5IvseiaKJlw7mGWkMpMGqm5pQ2qONY/SEFSkuN73i/7/iub1dzKoh09CgCIjY3NysqaOHHiiBEjpFJpex59" +
        //    "Q0NDXl7erl27dDqdU7RBTpo0aeHChQMHDmzvh9+4ccOTF6dWq4WnFovlwoULHoJ+xYoVD1Ayfvz49lCsVqu3bduWk5OTmppqMpn4cqvV6uFjzp07R5Lt2txp06bV19cjJ+7Rz8/P2Z5+88037bWzbt06UYlcLkftUJobP/mk/EpFeu/ezsBDALAslodGteoweSAnSPfDHIsIUuSQcSzT6lohimBsfkYVRkRb5kIqkwf847Olp498IUQVL/fu3Ssq" +
        //    "KsrOzg4PD6+vr3fZ84MHD6akpGzcuNEZVQ4btHfv3kGDBn3wwQftDVRISIgnb034rh9YcM9k3rx506dPJwEAu3fvPnToEH9hxIgRAwYMKC4uvnbtmvCGP/3pT0I/af78+QMHDnS8OQhhdXX11q1bH9Iz6M9//nNQUBDLskOHDlUoFO11Yv369Wazefjw4R52euXKlRMmTHAuV6lUu3bt8jQS1GgW/O//AgBc2h2EkEZnsdGMtw4GxIiQ6+4TDM2S" +
        //    "MoHPjkMj4s6c2HHrcilftU+fPiNHjjx9+vS5c+eETfzhD3+IjIx0bvrEiROin5+cnDxo0CCSJM+ePSt8X8uWLVMoFLNmzXLfV39//2XLlpEkKcINxjgmJkZYolAo8vLyOI57aOrhoUOH+G6npKS88sorHMcRBPH2228/cN6FFiQ3N7e4uBgAkJ+fX1xcvG3bNoeSGDRo0OrVq4VPys7Ozs7O5k/r6uqEwHrvvfce+Qbq6uocnUAeL5VUVlaWlpY6" +
        //    "AzE/P9/zKcW/G5HHgDGQSgiDydZisJKkl8QVS0qllhapuYUlJMJIECHSbrdWnDvKlwwePPinn35yHH/33XdFRUX79u1jGEahUBw+fNi5abPZLBxwAMDHH3+8YMEC/nTr1q2zZ8/mT2fPnp2VlSXCh0gCAwNFr9VNzeXLl4uMIw+sPn36rFixQsxj3bp1iz9/7bXX+ONx48YdOHCgurr6nXfe+fLLL90/uLq6Woj39jS5UHJych46H5znr+6dd95x" +
        //    "dqQ2b97s4e3btm07/cMPLi8RBGRYrqnFDCHwjrHkEIlYu9zQiBECbVsgSIlR16TXKIUmgz8ePXr0nj17amtrV69e3V5ItHbtWuFM2LNnjxBVAIBZs2YdOXJEWFJQUPDI8M1zl8Y5JuOP9Xq9C4JUGPQ51JVQEhIS1qxZk5CQ0LERaV5e3vXr1z2sLLRZJSUlFRUVwqsffPCBcHTc6D/lvTo31oEikabFYrHZSS95doARKTeqCbuNc6JDEcQsJvSm" +
        //    "Vp9ux44dojqxsbFr165tzzHYvn07f/zss89OnTrVuU52dvarr77Knz4ybEcIiYLxjhIEAJg8ebKw9/n5+b8D1XH27FnPK4eHh0+ZMoU/FWnvDRs2OA6Cg4NnzJjhpp0vis+5BgQGEoowmu0avUXyK4ygxKaXmjQcIRG7XRDojaaY2LhXX23t3vfffz937lxPAngAQFVVlUql4k/XrFnTXs1ly5YJnY2bN2+611g1NTX1baWurq6ysrIDgLV27dqk" +
        //    "pCRhrJienr5582atVvvbAcuNR+8yMn3vvfeGDBnCR0Z37txxHO/cuZPv56pVqxYsWNCeYW0BoKTG3p4R5Dis1poAwN5tzcGQRBwj16sAABgiJ2DB+436gT2Ctn/+1x49evDln3/+effu3fPz82tra923L8RHeHg4PxTO8vTTT0dHR/On7s2CRqNJTk5+sq3Ex8eLggkvgeXn53fhwgWhY1heXv76669369Zt2rRpJ0+e/G9gcp988smioiJhOMlP" +
        //    "A75w6dKlLS0trr0fDPZcB34UascIEs06i9lCUyThXTIfR5B+RjVFmziCEq3eIASVTcanEiMG9wwFAPz888+jRo3ir9bW1q5YsSIhISErK0sYm4tEaOu7du3qvjOpqaluvB+RsE4yevTomTNndgCwHEbkyJEju3btGjBggLBPe/fuHTNmzMyZMz3U2L+dXLt2LSUlpWfPnvxcdxTyUcLSpUsBALdv33Z5+5GruiolUASIXR9HHp/JYtfqzBTp5aZn" +
        //    "lpBIrEaZScO6MoJWG8Ow3OuT+5MkBQAIDQ11xID9+vUTUQnjx4+fNGmSwWBw+fo9cSIfKmDCC7rRIUlJSd9++23H+Fi8zJgx4+zZs+fOnVu8eLFwWuzYsSM9PV3EmP3O4hhu3oFlGKagoOD999/ng2FnghQA4FgEvKOlD19tiQ4GXNvMGIiQI2uhSWtiOUw4dpBiDCGCgpeHMfco5oqTGxoB5hzpDGIjqDa8lNWzW0yIKII7f/78jz/+uGzZMmFg" +
        //    "tH///vT0dGdsSaVSIY/6SFJG2AE3NUNDQ8vLyysFcuXKFc+XlTwFlkMyMjI+/PDD6urqvLw8vrCiomL69OmezmCWdbOW4p3QNA0A6Nu379ixYx0lixcv5hezNmzYIJFIRLMZYxAgIwEAu89rEIRSCmBRcgyEEgnQ6K0mi01CEQ4iDAOASJJjGEGtR2gIP2MTRZu4tsQVeJjHl9Q1bNKzT7u8cciQIevXr6+qqnr33XeFxM0LL7zgxroplcry8vL2" +
        //    "OlNbW8s7oACALl26uA+309LSegjEecGjw4DlEJIkly9fLoy/Dh06VFdX50mjBEF4uG7gufDTeuPGjaJLMTExc+bMcaFIIJD5ExUacKPRGhlIMjTwC2jTK7NWCSjQpDHweXwYY1lgmKn5vlnbSjjJAkLds68kbcGuFANtZ200M3div0coPAhXrVolxNapU6dEcVlSUtITTzzBn7pUzzxrKjSablZpHQyi1zyWR8AqLi52aelE+QUNDQ2eNLpp06aO" +
        //    "BVZhYSFvmhMTE0UwEjr1bUiKAPLGv80HK+gugSTGwGIwRyX0lAhWi88f2a7SAEoegh7YQI4gqZCowIpjfxOu/UUkptutRrf0lTiTHQBAIHivUZ87LCW5W5v49/jx40aji9ZWrVolVLfO0dwbb7zBH+/bt8+RIiCS8+fPFxYW8qcTJkz4j6LvDgZWfn7+888/n5CQsGjRItH6oMiPi4qKemSLQ4cOnTt3bgd2cfDgwQsXLhThzGH4AAB9+vTJyspy" +
        //    "eaPJxu39RWOzs3KKwABYjNonkmP75bTGO7cvlBx8f1F4bFyXhNSQ6MTwbqkRid3L/l509fi2Vi8kpntc2ghzi8pNDwOCw8Oj40MjYoV/djI0Pjbsjzm9hDU3btw4duzYhISExYsXixim3bt3C4mS2NhY0VPeeustIZU9atSoAwcOCCscO3ZMRK66UWy/tZAVFRWOiF2lUhUWFhYWFo4ZM+aVV14JCwv717/+9eGHH/JVn3nmmfj4eDccruPg66+/" +
        //    "7pCe8aHNvn37xD6Nn9+aNWtWrlzp4LHaJWnMTJOJieuCTNYHhsygsY6eteLn/Z/yztDlfxY2V/+SMCBXIg9kGXv9tR9v/fCPNvNk9nqCkrB2mxtPq/ZGGUlJDC1qQbCJb1Xd21a4VLixp6amxjFD1Gp1QUFBQUFBbm7u9OnTZTJZSUmJ0ISFhIT079/f2Tk5duyY0LRNnDhx+PDho0ePRgiVlJSIiKEtW7akpKT8vwHrpZdeEhWdOHHixIkTzlXd" +
        //    "pw84ZtuGDRuE7NyvEYf7/8knn7hscPny5StXrkxNTRV6teLfhqAf1brLFEKoVzdFJcTO/qi4aHFua5B19fS9q6ddtpAxdXn3zAnN/65077+fOb7jzHHx+kzf3qkZaR8LIgk8depUEXl7+PBhl+vNu3fvdskpZGZmFhcXT5kyxWKxOEpKS0tLS0uda3722Wcu/c7fzxRu3rx5yZIlYWHuvn/Sq1evkpKStLQ0N3UuX74cGRm5ZMmS/4g+cIjL1KLL" +
        //    "ly9369btzTffbM/h3bp1q3NmHz/iAAC7zYIxFmICIdhYV5+Rk/NawcmQ2KfcdE8WGDbyjY8zZ65ruV8johsghJwHmRTF354QZn5DCD/66KMlS5aEh4e7uSsqKmr//v3PPfdcexVyc3Nv3rw5Z84cl6QAhHDGjBlXrlwROcdtQhazWci8e/1JR6Gn6EzDksOGDRs2bNiqVasOHjxYWlp69epVpVKp0+koioqMjOzdu/e4ceOmTZv2yMf06NFDZPLd" +
        //    "S2RkZGBgIEKI4zhhvNM64/v2dV4RF4owEUNoRKIjwqwMMNoYmZ8cIsRxrHDgCQju3aoPf3rkxLzjNWe+uVN+SnPnutWgYWgrIimpf0hwVHxsz6FJmS92SUzT3r3FMjRsm6/HYYwIUiJ1GZZDAAFtpb/42xfR0U8465vMzExH/sLJkyevXLmiVCqtVqtUKlUoFMnJydnZ2S+//PIjNwfExcVt2bJl9erV3333XUVFRW1tLcY4NjY2LS1t5MiRbtwV" +
        //    "foj4RygUCq93noWHh/PtOGePQWfAchxnMplIkuwoSsOl2Gw2mqYhhBhjiqI6ao2dY5lmvaXglMps5/woRElkACEg+I0SimhoMmq0hsCQ0ABFLMvQZm0jbdazDI0IkpIFyEMiJPIgc4vK3KJCiBBlv2AAMCmVa++iFqXzVkECwbtK3cBeccvnjPKktxaLxWazSSQSufz3+wQNTdM2m43XcAEBAd61Y7VaebbSGS2w833ctvg2+P6GPiqYwhhbjTqO" +
        //    "4xyT8kEen5mub9SRBAGhYyMXQUikiKAgIgDmOJZhaSvHMc70gQNXLCmTmDXBugYqIFSEOQihwWRjWPzXt0ZKH/uNwJ1tX2Gl0vrPssaoIMqsb33fD8NMyLC4SWsGACIIMAAAQow5xmYBwCK2aC7VIaIQa/c3NnEYWMwuVvTu1GsX/nGgD1Wgk+2EZjmwq6zZX4JcplSRBGpuMVusdopE3mlpjAi5UU3YLRzhYltzvcqQ8UzsqAEJPlR1NmAdKNcq" +
        //    "9fZQp+/xYQAkFGGy0Fq9WUL9ijw+q15m0rCE1CkWA0aLXUoR/zOhjw9Snc0U/lxj2lXWHBVINRoYZ3USFECpjQxHSDDlzffTOEQQrN1f34gBABCJ98sD2KA2vPFS/6jwAB+kOhWwOAzURnt2anCYv4tvaIcEys5fqzfcbwwOkAGvsi44REqtOsRYOULqnMLQoDb2So58LrO7D0+dDVgQgvG9Qtq7ardai7++GsJwctbbTCOMAYTOiTEQAouNYTlu" +
        //    "3uT+PjB1RmC5vVqw54JSa+saHfwrPt2OXT4EQtigNszI6RUXFeQDU6d13l1K6S91P166ExMZ9Ov+IYALVCEEG5tNSV3DJo9J9SHp8QKWwURv/2d5eKj/b/HJPJpmaZqZO6mfD0aPHbC2HbykN9pCAqQdvsBAEPBuoz53eErykwofjB4vYF2sbPi+rCamS2CH/1ccBGFTi6VrdPD07Gd8GHq8gGWx2T/fdyE0SObtx9ndCcNxOoN1zsS+EorwYejx" +
        //    "AtZXRyqUTcawID+uo40gQvC+yvBcZlKv5EgfgDo53SCSS5UNJedrk7oqEIId/gstVntSXNjM59N96HEj/zcAEbRGVi2tjbwAAAAASUVORK5CYII=";


        private static byte[] cachedAdditionalData2 = null;
        internal static byte[] GetAdditionalData2(byte opacity)
        {
            if (cachedAdditionalData2 == null)
            {
                cachedAdditionalData2 = Convert.FromBase64String(AdditionalData2);
//            var gdiImage = Base.Drawing.StiImageConverter.BytesToImage(Convert.FromBase64String(AdditionalData2));
//#if BLAZOR
//                cachedAdditionalData2 = Base.Drawing.StiImageConverter.ImageToBytes(gdiImage);
//#else
//                var gdiTransparentedImage = Helpers.StiImageTransparenceHelper.GetTransparentedImage(gdiImage, 1f - opacity / 255f);
//                cachedAdditionalData2 = Base.Drawing.StiImageConverter.ImageToBytes(gdiTransparentedImage);
//#endif
            }
            return cachedAdditionalData2;
        }
#endregion

#region Get TextFormat Pattern String
        private static string[] positivePatterns = new string[] {
            "$n",
            "n$",
            "$ n",
            "n $" };
        private static string[] negativePatterns = new string[] {
            "($n)",
            "-$n",
            "$-n",
            "$n-",
            "(n$)",
            "-n$",
            "n-$",
            "n$-",
            "-n $",
            "-$ n",
            "n $-",
            "$ n-",
            "$ -n",
            "n- $",
            "($ n)",
            "(n $)" };

        public static string GetPositivePattern(int patternIndex)
        {
            return positivePatterns[patternIndex];
        }
        public static string GetNegativePattern(int patternIndex)
        {
            return negativePatterns[patternIndex];
        }
#endregion

    }
}
