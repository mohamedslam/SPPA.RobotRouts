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

using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base.Drawing
{
    public sealed class StiImageFromURL
    {
        /// <summary>
        /// Loads Bitmap from URL.
        /// </summary>
        public static Image LoadBitmap(string url, CookieContainer cookieContainer = null, NameValueCollection headers = null)
        {
            var bytes = StiDownloadCache.Get(url, cookieContainer, headers);
            var stream = new MemoryStream(bytes);
            return new Bitmap(stream);
        }

        /// <summary>
        /// Loads Metafile from URL.
        /// </summary>
        public static Image LoadMetafile(string url, CookieContainer cookieContainer = null, NameValueCollection headers = null)
        {
            var bytes = StiDownloadCache.Get(url, cookieContainer, headers);
            return StiMetafileConverter.BytesToMetafile(bytes);
        }

        /// <summary>
        /// Loads image from URL.
        /// </summary>
        public static Image LoadImage(string url, CookieContainer cookieContainer = null, NameValueCollection headers = null)
        {
            var bytes = StiDownloadCache.Get(url, cookieContainer, headers);
            var stream = new MemoryStream(bytes);
            return Image.FromStream(stream);
        }

        /// <summary>
        /// Tries to load image from URL.
        /// </summary>
        public static Image TryLoadImage(string url, CookieContainer cookieContainer = null, NameValueCollection headers = null)
        {
            try
            {
                var bytes = StiDownloadCache.Get(url, cookieContainer, headers);
                var stream = new MemoryStream(bytes);
                return Image.FromStream(stream);
            }
            catch
            {
                return null;
            }
        }
    }
}
