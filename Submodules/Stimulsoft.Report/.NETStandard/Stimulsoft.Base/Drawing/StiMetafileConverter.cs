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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// Class realize methods for conversion Metafile to string and string to Metafile.
    /// </summary>
    public sealed class StiMetafileConverter
	{
		/// <summary>
		/// Convert Metafile to String.
		/// </summary>
		/// <param name="metafile">Metafile for converting.</param>
		/// <returns>Result string.</returns>
		public static string MetafileToString(Metafile metafile)
		{
			var bytes = MetafileToBytes(metafile);
			return Convert.ToBase64String(bytes, StiBaseOptions.AllowInsertLineBreaksWhenSavingByteArray ? Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None);
		}

		/// <summary>
		/// Convert Metafile to Bytes.
		/// </summary>
		public static byte[] MetafileToBytes(Metafile metafile)
		{
		    using (var ms = new MemoryStream())
		    {
		        StiMetafileSaver.Save(ms, metafile);
		        return ms.ToArray();
		    }
		}

		/// <summary>
		/// Convert Bytes to Metafile.
		/// </summary>
		public static Metafile BytesToMetafile(byte[] bytes)
		{
			var stream = new MemoryStream(bytes);
			return Image.FromStream(stream) as Metafile;
		}
		
		/// <summary>
		/// Convert String to Metafile.
		/// </summary>
		public static Metafile StringToMetafile(string str)
		{
			var bytes = Convert.FromBase64String(str);
			return BytesToMetafile(bytes);
		}

	    /// <summary>
	    /// Converts metafile to bitmap bytes.
	    /// </summary>
	    public static byte[] MetafileToBitmapBytes(byte[] metafileBytes, int bitmapWidth, int bitmapHeight)
	    {
	        if (metafileBytes == null || metafileBytes.Length == 0) return null;

            using (var metafile = BytesToMetafile(metafileBytes))
            using (var bitmap = MetafileToBitmap(metafile, bitmapWidth, bitmapHeight))
            {
                return StiImageConverter.ImageToBytes(bitmap, true);
            }
	    }

        /// <summary>
        /// Converts metafile to bitmap.
        /// </summary>
        public static Bitmap MetafileToBitmap(byte[] metafileBytes, int bitmapWidth, int bitmapHeight)
        {
            if (metafileBytes == null || metafileBytes.Length == 0) return null;

	        using (var metafile = BytesToMetafile(metafileBytes))
	        {
	            return MetafileToBitmap(metafile, bitmapWidth, bitmapHeight);
	        }
	    }

        /// <summary>
        /// Converts metafile to bitmap with specified size.
        /// </summary>
	    public static Bitmap MetafileToBitmap(Metafile metafile, int bitmapWidth, int bitmapHeight)
	    {
	        var bitmap = new Bitmap(bitmapWidth, bitmapHeight);
	        using (var g = Graphics.FromImage(bitmap))
	        {
	            g.Clear(Color.White);
	            g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(metafile, 0, 0, bitmapWidth, bitmapHeight);
	        }

	        return bitmap;
	    }

	    /// <summary>
	    /// Converts metafile to bitmap in png format with specified size.
	    /// </summary>
	    public static byte[] MetafileToPngBytes(byte[] metafileBytes, int bitmapWidth, int bitmapHeight)
	    {
	        if (metafileBytes == null || metafileBytes.Length == 0) return null;
	        
	        using (var gdiMetafile = BytesToMetafile(metafileBytes))
	        {
	            return MetafileToPngBytes(gdiMetafile, bitmapWidth, bitmapHeight);
	        }
	    }

	    /// <summary>
        /// Converts metafile to bitmap in png format with specified size.
        /// </summary>
        public static byte[] MetafileToPngBytes(Metafile metafile, int bitmapWidth, int bitmapHeight)
	    {
            using (var bitmap = MetafileToBitmap(metafile, bitmapWidth, bitmapHeight))
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
	    }
    }
}
