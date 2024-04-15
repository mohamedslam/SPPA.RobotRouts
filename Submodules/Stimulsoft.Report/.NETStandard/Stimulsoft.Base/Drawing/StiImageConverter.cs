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
using System.ComponentModel;
using System.IO;
using Stimulsoft.Base.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// Class realize methods for conversion image to string and string to image.
    /// </summary>
    public sealed class StiImageConverter
    {
        /// <summary>
        /// Convert Image to packed String.
        /// </summary>
        /// <param name="image">Image for converting.</param>
        /// <returns>Result string.</returns>
        public static string ImageToPackedString(Image image)
        {
            if (image == null) return string.Empty;

            var bytes = ImageToBytes(image);
            return StiGZipHelper.Pack(Convert.ToBase64String(bytes, StiBaseOptions.AllowInsertLineBreaksWhenSavingByteArray
                ? Base64FormattingOptions.InsertLineBreaks
                : Base64FormattingOptions.None));
        }

        /// <summary>
        /// Convert packed String to Image.
        /// </summary>
        /// <param name="str">String for converting.</param>
        /// <returns>Result Image.</returns>
        public static Image PackedStringToImage(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;

            var bytes = StringToByteArray(StiGZipHelper.Unpack(str));
            return BytesToImage(bytes);
        }

        /// <summary>
        /// Convert Image to String.
        /// </summary>
        /// <param name="image">Image for converting.</param>
        /// <param name="allowNulls">Returns null when image equal to null. Otherwise, return byte[0].</param>
        /// <returns>Result string.</returns>
        public static string ImageToString(Image image, bool allowNulls = false)
        {
            var bytes = ImageToBytes(image, allowNulls);
            return bytes != null ? Convert.ToBase64String(bytes, StiBaseOptions.AllowInsertLineBreaksWhenSavingByteArray ? Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None) : null;
        }

        /// <summary>
        /// Convert Image to Bytes.
        /// </summary>
        /// <param name="image">Image for converting.</param>
        /// <param name="allowNulls">Returns null when image equal to null. Otherwise, return byte[0].</param>
        /// <returns>Result byte array.</returns>
        public static byte[] ImageToBytes(Image image, bool allowNulls = false)
        {
            if (image == null)
                return allowNulls ? null : new byte[0];

            if (image is Metafile)
                return StiMetafileConverter.MetafileToBytes(image as Metafile);

#if NETSTANDARD && !STIDRAWING
            var imageConverter = new System.Drawing.ImageConverter();
#else
            var imageConverter = TypeDescriptor.GetConverter(typeof(Image));
#endif

            try
            {
                return (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
            }
            catch
            {
                try
                {
                    using (var bitmap = new Bitmap(image.Width, image.Height, image.PixelFormat))
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.DrawImage(image, 0, 0, image.Width, image.Height);
                        return (byte[])imageConverter.ConvertTo(bitmap, typeof(byte[]));
                    }
                }
                catch
                {
                }
            }

            return allowNulls ? null : new byte[0];
        }

        public static Image TryBytesToImage(byte[] bytes, int width = 200, int height = 200, bool stretch = true, bool aspectRatio = false)
        {
            try
            {
                return BytesToImage(bytes, width, height, stretch, aspectRatio);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convert Bytes to Image.
        /// </summary>
        public static Image BytesToImage(byte[] bytes, int width = 200, int height = 200, bool stretch = true, bool aspectRatio = false)
        {
            if (bytes == null || bytes.Length == 0) return null;

            if (StiSvgHelper.IsSvg(bytes))
                return StiSvgHelper.ConvertSvgToImage(bytes, width, height, stretch, aspectRatio);

            try
            {
                var stream = new MemoryStream(bytes);
                return Image.FromStream(stream);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convert String to Image.
        /// </summary>
        /// <param name="str">String for converting.</param>
        /// <returns>Result Image.</returns>
        public static Image StringToImage(string str)
        {
            var bytes = StringToByteArray(str);
            return bytes != null ? BytesToImage(bytes) : null;
        }

        public static byte[] StringToByteArray(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;

            if (str.StartsWith("data:image"))
            {
                var pos = str.LastIndexOf(',');
                if (pos > 0)
                    str = str.Substring(pos + 1);
            }

            return str.IsBase64String() ? Convert.FromBase64String(str) : null;
        }
    }
}
