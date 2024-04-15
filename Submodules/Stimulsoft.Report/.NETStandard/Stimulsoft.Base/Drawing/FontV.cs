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
using System.IO;
using System.Drawing;
using System.Drawing.Text;

#if STIDRAWING
using PrivateFontCollection = Stimulsoft.Drawing.Text.PrivateFontCollection;
using FontFamily = Stimulsoft.Drawing.FontFamily;
using SystemFonts = Stimulsoft.Drawing.SystemFonts;
#endif

namespace Stimulsoft.Base.Drawing
{
    public class FontV : IDisposable
    {
        #region IDisposable
        public void Dispose()
        {
            //if (Font != null)
            //    Font.Dispose();

            if (fontCollection != null)
                fontCollection.Dispose();

            //if (fontData != IntPtr.Zero)
            //    Marshal.FreeCoTaskMem(fontData);

            if (IsCachePath && !string.IsNullOrWhiteSpace(FilePath))
                try
                {
                    File.Delete(FilePath);
                }
                catch
                { }

            fontContent = null;
        }
        #endregion

        #region Fields
        //private IntPtr fontData = IntPtr.Zero;
        private PrivateFontCollection fontCollection;
        private byte[] fontContent = null;
        #endregion

        #region Properties
        //public Font Font { get; }

        public string Name { get; set; }

        public string FilePath { get; } = null;

        public bool IsCachePath { get; } = false;

        public byte[] Content => fontContent;

        internal FontFamily FontFamily => fontCollection.Families.Length > 0 ? fontCollection.Families[0] : SystemFonts.DefaultFont.FontFamily;
        #endregion

        #region Methods
        private static string CreateTempFile(byte[] content, string extension, string dataHash)
        {
            try
            {
                string temp = StiFontCollection.DefaultCachePath;
                if (string.IsNullOrWhiteSpace(temp))
                {
                    temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    temp = Path.Combine(temp, "Stimulsoft", "ReportsResources");
                }
                if (!Directory.Exists(temp)) Directory.CreateDirectory(temp);

                string cache = dataHash + "." + extension.ToLowerInvariant();
                temp = Path.Combine(temp, cache);

                if (!File.Exists(temp))
                {
                    File.WriteAllBytes(temp, content);
                }

                return temp;
            }
            catch
            {
                StiFontCollection.AllowFileCache = false;
            }
            return null;
        }

        internal static string GetHashName(byte[] content)
        {
            var crc = Crypto.Crc32.Calculate(content);
            return $"{crc:X8}-{content.Length}";
        }
        #endregion

        public FontV(string name, byte[] content, string path, string extension, bool allowFileCache, string hash = null, PrivateFontCollection masterFontCollection = null)
        {
            if (content == null) return;

            fontContent = content;
            Name = name;
            FilePath = path;

            if (hash == null)
            {
                hash = GetHashName(content);
            }
            fontCollection = new PrivateFontCollection();

            if (string.IsNullOrEmpty(FilePath) && allowFileCache)
            {
                FilePath = CreateTempFile(content, extension, hash);
                IsCachePath = FilePath != null;
            }

            if (!string.IsNullOrEmpty(FilePath))
            {
                try
                {
                    fontCollection.AddFontFile(FilePath);
                    if (masterFontCollection != null)
                        masterFontCollection.AddFontFile(FilePath);
                }
                catch
                {
                    FilePath = null;
                }
            }
            if (string.IsNullOrEmpty(FilePath))
            {
                try
                {
                    fontCollection.AddFontBytes(content);
                    if (masterFontCollection != null)
                        masterFontCollection.AddFontBytes(content);
                }
                catch(Exception)
                {
                }
            }

            if (fontCollection.Families.Length != 0)
            {
                Name = fontCollection.Families[0].Name;
            }
            else
            {
                Name = SystemFonts.DefaultFont.Name;
            }
        }
    }
}
