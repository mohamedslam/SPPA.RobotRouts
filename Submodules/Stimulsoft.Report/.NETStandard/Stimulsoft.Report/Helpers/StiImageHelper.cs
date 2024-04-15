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
using System.IO;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Import;
using Stimulsoft.Report.Components;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
#endif

namespace Stimulsoft.Report.Helpers
{
    public static class StiImageHelper
    {
        #region Fields
        private static long usedMemoryCounter = 0;
        #endregion

        #region Methods
        public static Image TryFromFile(string fileName, int width = 200, int height = 200)
        {
            try
            {
                return FromFile(fileName, width, height);
            }
            catch
            {
                return null;
            }
        }

        public static byte[] TryBytesFromFile(string fileName)
        {
            try
            {
                return BytesFromFile(fileName);
            }
            catch
            {
                return null;
            }
        }

        public static Image FromFile(string fileName, int width = 100, int height = 100)
        {
            if (!File.Exists(fileName))
                return null;

            var bytes = File.ReadAllBytes(fileName);
            return FromBytes(bytes, width, height);
        }

        public static byte[] BytesFromFile(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            return File.ReadAllBytes(fileName);
        }

        public static Image TryFromBytes(byte[] bytes, int width, int height)
        {
            try
            {
                return FromBytes(bytes, width, height);
            }
            catch
            {
                return null;
            }
        }

        public static Image FromBytes(byte[] bytes, int width, int height, bool stretch = true, bool aspectRatio = false)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            return StiImageConverter.BytesToImage(bytes, width, height, stretch, aspectRatio);
        }

        public static Image GetImageFromObject(object imageObject, int width = 200, int height = 200,
            bool stretch = true, bool aspectRatio = false)
        {
            return StiImageConverter.BytesToImage(GetImageBytesFromObject(imageObject), width, height, stretch, aspectRatio);
        }

        public static byte[] GetImageBytesFromObject(object imageObject)
        {
            #region Get image from Image
            if (imageObject is Image)
                return StiImageConverter.ImageToBytes(imageObject as Image);
            #endregion

            #region Get image from string
            if (imageObject is string && ((string)imageObject).Length > 0)
            {
                try
                {
                    string st = imageObject as string;
                    int pos = st.IndexOfInvariant("<svg");
                    if (pos >= 0 && pos < 1000)
                    {
                        return Encoding.UTF8.GetBytes(st);
                    }
                    return StiImageConverter.StringToByteArray(st);
                }
                catch
                {
                }
            }
            #endregion

            #region Get image from bytes
            var bytes = imageObject as byte[];

            ProcessGarbageCollector();

            if (bytes != null && bytes.Length > 0)
            {
                if (OleUnit.IsOleHeader(bytes))
                {
                    //remove ole-link from array
                    var objHeader = new OleUnit.ObjectHeader(bytes);
                    var tempData = new byte[bytes.Length - objHeader.HeaderLen];
                    Array.Copy(bytes, objHeader.HeaderLen, tempData, 0, bytes.Length - objHeader.HeaderLen);
                    usedMemoryCounter += bytes.Length;
                    bytes = tempData;
                }

                if (OleUnit.IsOleContainer(bytes))
                {
                    #region Get OlePres object
                    var ole = new OleUnit.OleContainer(bytes);
                    var position = 0;
                    for (var index = 1; index < ole.Dir.Length; index++)
                    {
                        if (ole.Dir[index].Name.IndexOf("OlePres", StringComparison.InvariantCulture) != -1)
                        {
                            position = index;
                            break;
                        }
                    }
                    usedMemoryCounter += bytes.Length;
                    bytes = ole.GetStreamData(position);
                    ole.Clear();

                    if (bytes == null || bytes.Length == 0)
                        return null;
                    #endregion

                    const int DibStretchBltHeaderLength = 6 + 20;
                    const int StretchDIBitsHeaderLength = 6 + 22;

                    #region find DIB
                    int offset = 0;
                    bool haveImage = false;
                    int imageLength = bytes.Length;
                    if (OleUnit.CheckForOlePres(bytes))	//OlePres header
                    {
                        offset = OleUnit.OlePresHeaderLength;
                        if ((BitConverter.ToUInt16(bytes, offset + 0) == 0x0001) &&		//WMF Type
                            (BitConverter.ToUInt16(bytes, offset + 2) == 0x0009))		//WMF Header length
                        {
                            offset += 9 * 2;		//WMF Header length
                            int com = 0;
                            int len = 0;
                            do
                            {
                                len = BitConverter.ToInt32(bytes, offset);
                                com = BitConverter.ToInt16(bytes, offset + 4);
                                if (com == 0x0B41)		//DibStretchBlt
                                {
                                    offset += DibStretchBltHeaderLength;
                                    haveImage = true;
                                    imageLength = len * 2 - DibStretchBltHeaderLength;
                                    break;
                                }
                                if (com == 0x0F43)		//StretchDIBits
                                {
                                    offset += StretchDIBitsHeaderLength;
                                    haveImage = true;
                                    imageLength = len * 2 - StretchDIBitsHeaderLength;
                                    break;
                                }
                                if (len < 1) break;	//bad data
                                offset += len * 2;
                            }
                            while ((!((com == 0x0000) && (len == 0x0003))) && (offset < bytes.Length - 6));
                        }

                        if (!haveImage)
                        {
                            offset = OleUnit.OlePresHeaderLength;
                            imageLength -= OleUnit.OlePresHeaderLength;
                        }
                    }
                    #endregion

                    #region create BMP header
                    int bitsPerPixel = BitConverter.ToUInt16(bytes, offset + 0x0E);
                    int paletteLength = 0;

                    if (bitsPerPixel == 0x08)
                        paletteLength = 0x0400;

                    if (bitsPerPixel == 0x04)
                        paletteLength = 0x0040;

                    var bmpHeader = new byte[14] { 0x42, 0x4D, 0x01, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00 };
                    BitConverter.GetBytes(imageLength + 14).CopyTo(bmpHeader, 2);
                    BitConverter.GetBytes(0x0036 + paletteLength).CopyTo(bmpHeader, 10);
                    #endregion

                    var ms = new MemoryStream();
                    ms.Write(bmpHeader, 0, 14);
                    ms.Write(bytes, offset, imageLength);
                    ms.Position = 0;

                    usedMemoryCounter += bytes.Length;

                    if (StiOptions.Engine.ConvertBmpDataToPng && ms.Length > 200000)
                    {
                        var image = Image.FromStream(ms, true);
                        var ms2 = new MemoryStream();
                        image.Save(ms2, ImageFormat.Png);
                        ms2.Seek(0, SeekOrigin.Begin);
                        ms = ms2;

                        usedMemoryCounter += ms.Length;
                    }

                    return ms.ToArray();
                }

                return bytes;
            }
            #endregion

            return null;
        }

        private static void ProcessGarbageCollector()
        {
            if (StiOptions.Engine.UseGarbageCollectorForImageCache)
            {
                if (usedMemoryCounter > 100000000)
                {
                    usedMemoryCounter = 0;
                    GC.Collect();

                    if (StiOptions.Engine.AllowWaitForPendingFinalizers)
                        GC.WaitForPendingFinalizers();

                    GC.Collect();
                }
            }
            else
            {
                usedMemoryCounter = 0;
            }
        }

        internal static string GetImageName(byte[] data)
        {
            if (IsTiff(data))
                return "Tiff";

            if (IsPng(data))
                return "Png";

            if (IsGif(data))
                return "Gif";

            if (IsJpeg(data))
                return "Jpeg";

            if (IsBmp(data))
                return "Bmp";

            if (IsEmf(data))
                return "Emf";

            if (IsWmf(data))
                return "Wmf";

            if (IsIcon(data))
                return "Icon";

            if (IsSvg(data))
                return "Svg";

            if (IsXml(data)) //xml
            {
                for (var index = 5; index < data.Length - 4; index++)
                {
                    if (data[index] != 0x3c) continue;

                    if (data[index + 1] == 0x73 && data[index + 2] == 0x76 && data[index + 3] == 0x67)
                        return "Svg";

                    break;
                }
            }

            return null;
        }

        public static bool IsEqualImages(byte[] image1, byte[] image2, int bytesToCompare = 0)
        {
            if (image1 != null && image2 == null)
                return false;

            if (image1 == null && image2 != null)
                return false;

            if (image1 == null && image2 == null)
                return true;

            if (image1.Length != image2.Length)
                return false;

            var length = bytesToCompare == 0 ? image1.Length : bytesToCompare;
            var tailIdx = length - length % sizeof(long);

            //check in 8 byte chunks
            for (var i = 0; i < tailIdx; i += sizeof(long))
            {
                if (BitConverter.ToInt64(image1, i) != BitConverter.ToInt64(image2, i))
                    return false;
            }

            //check the remainder of the array, always shorter than 8 bytes
            for (var i = tailIdx; i < length; i++)
            {
                if (image1[i] != image2[i])
                    return false;
            }

            return true;
        }

        public static Image RotateImage(Image image, StiImageRotation rotation, bool disposeOriginal = false)
        {
            if (image == null)
                return null;

            if (StiOptions.Engine.FullTrust && image is Metafile)
                return RotateMetafile(image, rotation, disposeOriginal);

            switch (rotation)
            {
                case StiImageRotation.Rotate180:
                    image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;

                case StiImageRotation.Rotate90CW:
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;

                case StiImageRotation.Rotate90CCW:
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;

                case StiImageRotation.FlipHorizontal:
                    image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;

                case StiImageRotation.FlipVertical:
                    image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;
            }

            return image;
        }

        private static Image RotateMetafile(Image image, StiImageRotation rotation, bool disposeOriginal = false)
        {
            if (image == null)
                return null;

            var metafile = image as Metafile;
            var unit = GraphicsUnit.Pixel;
            var rectm = metafile.GetBounds(ref unit);
            var center = new PointF(rectm.Width / 2, rectm.Height / 2);

            MemoryStream ms;
            Metafile mf;
            switch (rotation)
            {
                case StiImageRotation.Rotate90CCW:
                    ms = new MemoryStream();
                    using (var bmp = new Bitmap(1, 1))
                    using (var grfx = Graphics.FromImage(bmp))
                    {
                        var ipHdc = grfx.GetHdc();
                        mf = new Metafile(ms, ipHdc, new RectangleF(rectm.Y, rectm.X, rectm.Height, rectm.Width), MetafileFrameUnit.Pixel);
                        grfx.ReleaseHdc(ipHdc);
                    }
                    using (var grfx = Graphics.FromImage(mf))
                    {
                        grfx.TranslateTransform(center.Y, center.X);
                        grfx.RotateTransform(-90);
                        grfx.TranslateTransform(-center.X, -center.Y);
                        grfx.DrawImage(
                            metafile,
                            metafile.GetMetafileHeader().Bounds.Left,
                            metafile.GetMetafileHeader().Bounds.Top,
                            metafile.Width,
                            metafile.Height);
                    }

                    if (disposeOriginal)
                        metafile.Dispose();

                    return mf;

                case StiImageRotation.Rotate90CW:
                    ms = new MemoryStream();
                    using (var bmp = new Bitmap(1, 1))
                    using (var grfx = Graphics.FromImage(bmp))
                    {
                        var ipHdc = grfx.GetHdc();
                        //mf = new Metafile(ms, ipHdc, rectm, MetafileFrameUnit.Pixel);
                        mf = new Metafile(ms, ipHdc, new RectangleF(rectm.Y, rectm.X, rectm.Height, rectm.Width), MetafileFrameUnit.Pixel);
                        grfx.ReleaseHdc(ipHdc);
                    }
                    using (var grfx = Graphics.FromImage(mf))
                    {
                        grfx.TranslateTransform(center.Y, center.X);
                        grfx.RotateTransform(90);
                        grfx.TranslateTransform(-center.X, -center.Y);
                        grfx.DrawImage(
                            metafile,
                            metafile.GetMetafileHeader().Bounds.Left,
                            metafile.GetMetafileHeader().Bounds.Top,
                            metafile.Width,
                            metafile.Height);
                    }

                    if (disposeOriginal)
                        metafile.Dispose();

                    return mf;

                case StiImageRotation.Rotate180:
                    ms = new MemoryStream();
                    using (var bmp = new Bitmap(1, 1))
                    using (var grfx = Graphics.FromImage(bmp))
                    {
                        var ipHdc = grfx.GetHdc();
                        //mf = new Metafile(ms, ipHdc, rectm, MetafileFrameUnit.Pixel);
                        mf = new Metafile(ms, ipHdc, rectm, MetafileFrameUnit.Pixel);
                        grfx.ReleaseHdc(ipHdc);
                    }
                    using (var grfx = Graphics.FromImage(mf))
                    {
                        grfx.TranslateTransform(center.X, center.Y);
                        grfx.RotateTransform(180);
                        grfx.TranslateTransform(-center.X, -center.Y);
                        grfx.DrawImage(
                            metafile,
                            metafile.GetMetafileHeader().Bounds.Left,
                            metafile.GetMetafileHeader().Bounds.Top,
                            metafile.Width,
                            metafile.Height);
                    }

                    if (disposeOriginal)
                        metafile.Dispose();

                    return mf;

                case StiImageRotation.FlipHorizontal:
                    var bmp2 = new Bitmap((int)rectm.Width, (int)rectm.Height);
                    using (var grfx = Graphics.FromImage(bmp2))
                    {
                        grfx.DrawImage(
                            metafile,
                            -metafile.GetMetafileHeader().Bounds.Left,
                            -metafile.GetMetafileHeader().Bounds.Top,
                            metafile.Width,
                            metafile.Height);
                    }
                    bmp2.RotateFlip(RotateFlipType.RotateNoneFlipX);

                    if (disposeOriginal)
                        metafile.Dispose();

                    return bmp2;

                case StiImageRotation.FlipVertical:
                    var bmp3 = new Bitmap((int)rectm.Width, (int)rectm.Height);
                    using (var grfx = Graphics.FromImage(bmp3))
                    {
                        grfx.DrawImage(
                            metafile,
                            -metafile.GetMetafileHeader().Bounds.Left,
                            -metafile.GetMetafileHeader().Bounds.Top,
                            metafile.Width,
                            metafile.Height);
                    }
                    bmp3.RotateFlip(RotateFlipType.RotateNoneFlipY);

                    if (disposeOriginal)
                        metafile.Dispose();

                    return bmp3;
            }

            return metafile;
        }
        #endregion

        #region Methods.Status
        public static bool IsXml(byte[] data)
        {
            return data != null && data.Length > 4 && data[0] == 0x3c && data[1] == 0x3f && data[2] == 0x78 && data[3] == 0x6d && data[4] == 0x6c;
        }

        public static bool IsSvg(byte[] data)
        {
            return Stimulsoft.Base.Helpers.StiSvgHelper.IsSvg(data);
        }

        public static bool IsIcon(byte[] data)
        {
            return data != null && data.Length > 4 && data[0] == 0x00 && data[1] == 0x00 && (data[2] == 0x01 || data[2] == 0x02) && data[3] == 0x00 && data[4] != 0x00;
        }

        public static bool IsWmf(byte[] data)
        {
            return data != null && data.Length > 3
                && ((data[0] == 0xd7 && data[1] == 0xcd && data[2] == 0xc6 && data[3] == 0x9a)
                || (data[0] == 0x01 && data[1] == 0x00 && data[2] == 0x09 && data[3] == 0x00));
        }

        public static bool IsEmf(byte[] data)
        {
            return data != null && data.Length > 45 &&
                data[40] == 0x20 && data[41] == 0x45 && data[42] == 0x4d && data[43] == 0x46;
        }

        public static bool IsBmp(byte[] data)
        {
            return data != null && data.Length > 2 && data[0] == 0x42 && data[1] == 0x4d;
        }

        public static bool IsJpeg(byte[] data)
        {
            return data != null && data.Length > 2 && data[0] == 0xff && data[1] == 0xd8;
        }

        public static bool IsGif(byte[] data)
        {
            return data != null && data.Length > 3 && data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46;
        }

        public static bool IsPng(byte[] data)
        {
            return data != null && data.Length > 3 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4e && data[3] == 0x47;
        }

        public static bool IsTiff(byte[] data)
        {
            return data != null && data.Length > 3
                && ((data[0] == 0x49 && data[1] == 0x49 && data[2] == 0x2a && data[3] == 0x00)
                || (data[0] == 0x4d && data[1] == 0x4d && data[2] == 0x00 && data[3] == 0x2a));
        }

        public static bool IsMetafile(byte[] data)
        {
            return IsEmf(data) || IsWmf(data);
        }

        public static bool IsImage(object data)
        {
            if (data is byte[])
                return IsImage(data as byte[]);

            if (data is string)
                return IsImage(data as string);

            return data is Image;
        }

        public static bool IsImage(byte[] data)
        {
            if (data != null && data.Length < 128)
                return false;

            if (OleUnit.IsOleHeader(data))
            {
                //remove ole-link from array
                var objHeader = new OleUnit.ObjectHeader(data);
                var tempData = new byte[data.Length - objHeader.HeaderLen];
                Array.Copy(data, objHeader.HeaderLen, tempData, 0, data.Length - objHeader.HeaderLen);
                data = tempData;
            }

            return GetImageName(data) != null;
        }

        public static bool IsImage(string str)
        {
            if (str != null && str.Length < 128)
                return false;

            try
            {
                if (!str.IsBase64String())
                    return false;

                var data = Convert.FromBase64String(str.Substring(0, 256));

                return GetImageName(data) != null;
            }
            catch
            {
            }
            return false;
        }
        #endregion
    }
}