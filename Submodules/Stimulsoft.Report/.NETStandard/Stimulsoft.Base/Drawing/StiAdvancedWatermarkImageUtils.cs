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

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using ImageAttributes = Stimulsoft.Drawing.Imaging.ImageAttributes;
#endif

namespace Stimulsoft.Base.Drawing
{
    internal static class StiAdvancedWatermarkImageUtils
    {
        private struct StiGdiImageKey
        {
            #region Properties
            public string Key { get; }

            public Size Size { get; } 
            #endregion

            public StiGdiImageKey(string key, Size size)
            {
                Key = key;
                Size = size;
            }
        }


        #region Fields
        private static Dictionary<RectangleF, Image> imageHash = new Dictionary<RectangleF, Image>();
        private static Dictionary<StiGdiImageKey, Image> gdiImageHash = new Dictionary<StiGdiImageKey, Image>();
        #endregion

        #region Methods
        internal static void DrawImage(Graphics g, Image image, Rectangle rect)
        {
            var cachedImage = GetImage(rect);

            if (cachedImage == null)
            {
                var destRect = new Rectangle(0, 0, rect.Width, rect.Height);
                cachedImage = new Bitmap(rect.Width, rect.Height);
                using (var graphics = Graphics.FromImage(cachedImage))

                using (var wrapMode = new ImageAttributes())
                {
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }

                imageHash[rect] = cachedImage;
            }

            g.DrawImageUnscaled(cachedImage, rect);
        }

        internal static Image GetImage(RectangleF rect)
        {
            if (imageHash.ContainsKey(rect))
                return imageHash[rect];

            return null;
        }

        internal static void DisposeCachedImage()
        {
            foreach (var image in imageHash.Values)
                image.Dispose();

            foreach (var image in gdiImageHash.Values)
                image.Dispose();

            imageHash.Clear();
            gdiImageHash.Clear();
        }

        internal static Image GetGdiImage(byte[] bytes, int width = 200, int height = 200)
        {
            var md5Key = GetMD5Key(bytes);
            var size = new Size(width, height);

            var key = new StiGdiImageKey(md5Key, size);

            if (gdiImageHash.ContainsKey(key))
                return gdiImageHash[key];

            var gdiImage = StiImageConverter.BytesToImage(bytes, width, height);

            gdiImageHash[key] = gdiImage;

            return gdiImage;
        }

        private static string GetMD5Key(byte[] message)
        {
            byte[] hashValue = GetMD5(message);

            string hashString = string.Empty;
            foreach (byte x in hashValue)
            {
                hashString += string.Format("{0:x2}", x);
            }

            return hashString;
        }

        private static byte[] GetMD5(byte[] message)
        {
            var hashString = new MD5CryptoServiceProvider();
            return hashString.ComputeHash(message);
        }
        #endregion
    }
}
