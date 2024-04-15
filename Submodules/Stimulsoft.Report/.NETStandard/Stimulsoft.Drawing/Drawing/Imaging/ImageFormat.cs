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

namespace Stimulsoft.Drawing.Imaging
{
    public class ImageFormat
    {
        private string guid;
        private string name;

        internal const string BmpGuid = "b96b3cab-0728-11d3-9d7b-0000f81ef32e";
        internal const string EmfGuid = "b96b3cac-0728-11d3-9d7b-0000f81ef32e";
        //internal const string ExifGuid = "b96b3cb2-0728-11d3-9d7b-0000f81ef32e";
        internal const string GifGuid = "b96b3cb0-0728-11d3-9d7b-0000f81ef32e";
        internal const string TiffGuid = "b96b3cb1-0728-11d3-9d7b-0000f81ef32e";
        internal const string PngGuid = "b96b3caf-0728-11d3-9d7b-0000f81ef32e";
        internal const string MemoryBmpGuid = "b96b3caa-0728-11d3-9d7b-0000f81ef32e";
        //internal const string IconGuid = "b96b3cb5-0728-11d3-9d7b-0000f81ef32e";
        internal const string JpegGuid = "b96b3cae-0728-11d3-9d7b-0000f81ef32e";
        //internal const string WmfGuid = "b96b3cad-0728-11d3-9d7b-0000f81ef32e";

        //private static object locker = new object();

        private static ImageFormat BmpImageFormat;
        private static ImageFormat EmfImageFormat;
        //private static ImageFormat ExifImageFormat;
        private static ImageFormat GifImageFormat;
        private static ImageFormat TiffImageFormat;
        private static ImageFormat PngImageFormat;
        private static ImageFormat MemoryBmpImageFormat;
        //private static ImageFormat IconImageFormat;
        private static ImageFormat JpegImageFormat;
        //private static ImageFormat WmfImageFormat;

        private SixLabors.ImageSharp.Formats.IImageEncoder sixImageEncoder;
        internal SixLabors.ImageSharp.Formats.IImageEncoder SixImageEncoder
        {
            get
            {
                if (sixImageEncoder == null)
                {
                    if (guid == BmpGuid || guid == MemoryBmpGuid)
                        sixImageEncoder = new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder();
                    if (guid == GifGuid)
                        sixImageEncoder = new SixLabors.ImageSharp.Formats.Gif.GifEncoder();
                    if (guid == JpegGuid)
                        sixImageEncoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder();
                    //if (guid == PbmGuid)
                    //    sixImageEncoder = new SixLabors.ImageSharp.Formats.Pbm.PbmEncoder();
                    if (guid == PngGuid)
                        sixImageEncoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder();
                    //if (guid == TgaGuid)
                    //    sixImageEncoder = new SixLabors.ImageSharp.Formats.Tga.TgaEncoder();
                    if (guid == TiffGuid)
                    {
                        sixImageEncoder = new SixLabors.ImageSharp.Formats.Tiff.TiffEncoder();
                        (sixImageEncoder as SixLabors.ImageSharp.Formats.Tiff.TiffEncoder).Compression = SixLabors.ImageSharp.Formats.Tiff.Constants.TiffCompression.Lzw;
                    }
                    //if (guid == WebpGuid)
                    //    sixImageEncoder = new SixLabors.ImageSharp.Formats.Webp.WebpEncoder();
                }

                return sixImageEncoder;
            }
        }


        public Guid Guid
        {
            get
            {
                return new Guid(guid);
            }
        }

        public static ImageFormat Bmp
        {
            get
            {
                if (BmpImageFormat == null)
                    BmpImageFormat = new ImageFormat("Bmp", BmpGuid);
                return BmpImageFormat;
            }
        }

        public static ImageFormat Emf
        {
            get
            {
                if (EmfImageFormat == null)
                    EmfImageFormat = new ImageFormat("Emf", EmfGuid);
                return EmfImageFormat;
            }
        }

        //public static ImageFormat Exif
        //{
        //    get
        //    {
        //        lock (locker)
        //        {
        //            if (ExifImageFormat == null)
        //                ExifImageFormat = new ImageFormat("Exif", ExifGuid);
        //            return ExifImageFormat;
        //        }
        //    }
        //}

        public static ImageFormat Gif
        {
            get
            {
                if (GifImageFormat == null)
                    GifImageFormat = new ImageFormat("Gif", GifGuid);
                return GifImageFormat;
            }
        }

        //public static ImageFormat Icon
        //{
        //    get
        //    {
        //        lock (locker)
        //        {
        //            if (IconImageFormat == null)
        //                IconImageFormat = new ImageFormat("Icon", IconGuid);
        //            return IconImageFormat;
        //        }
        //    }
        //}

        public static ImageFormat Jpeg
        {
            get
            {
                if (JpegImageFormat == null)
                    JpegImageFormat = new ImageFormat("Jpeg", JpegGuid);
                return JpegImageFormat;
            }
        }

        public static ImageFormat MemoryBmp
        {
            get
            {
                if (MemoryBmpImageFormat == null)
                    MemoryBmpImageFormat = new ImageFormat("MemoryBMP", MemoryBmpGuid);
                return MemoryBmpImageFormat;
            }
        }

        public static ImageFormat Png
        {
            get
            {
                if (PngImageFormat == null)
                    PngImageFormat = new ImageFormat("Png", PngGuid);
                return PngImageFormat;
            }
        }

        public static ImageFormat Tiff
        {
            get
            {
                if (TiffImageFormat == null)
                    TiffImageFormat = new ImageFormat("Tiff", TiffGuid);
                return TiffImageFormat;
            }
        }

        //public static ImageFormat Wmf
        //{
        //    get
        //    {
        //        lock (locker)
        //        {
        //            if (WmfImageFormat == null)
        //                WmfImageFormat = new ImageFormat("Wmf", WmfGuid);
        //            return WmfImageFormat;
        //        }
        //    }
        //}
        //#endregion

        public override string ToString()
        {
            return name;
        }

        private ImageFormat(string name, string guid)
        {
            this.name = name;
            this.guid = guid;
        }

        public ImageFormat(Guid guid)
        {
            this.guid = guid.ToString();
        }

        public static implicit operator System.Drawing.Imaging.ImageFormat(ImageFormat format)
        {
            switch (format.guid)
            {
                case ImageFormat.BmpGuid: return System.Drawing.Imaging.ImageFormat.Bmp;
                case ImageFormat.EmfGuid: return System.Drawing.Imaging.ImageFormat.Emf;
                //case ImageFormat.ExifGuid: return System.Drawing.Imaging.ImageFormat.Exif;
                case ImageFormat.GifGuid: return System.Drawing.Imaging.ImageFormat.Gif;
                case ImageFormat.TiffGuid: return System.Drawing.Imaging.ImageFormat.Tiff;
                case ImageFormat.PngGuid: return System.Drawing.Imaging.ImageFormat.Png;
                case ImageFormat.MemoryBmpGuid: return System.Drawing.Imaging.ImageFormat.MemoryBmp;
                //case ImageFormat.IconGuid: return System.Drawing.Imaging.ImageFormat.Icon;
                case ImageFormat.JpegGuid: return System.Drawing.Imaging.ImageFormat.Jpeg;
                //case ImageFormat.WmfGuid: return System.Drawing.Imaging.ImageFormat.Wmf;
                default: return System.Drawing.Imaging.ImageFormat.Png;
            }
        }

        public static implicit operator ImageFormat(System.Drawing.Imaging.ImageFormat format)
        {
            if (format != null)
            {
                if (format == System.Drawing.Imaging.ImageFormat.Bmp) return ImageFormat.Bmp;
                if (format == System.Drawing.Imaging.ImageFormat.Emf) return ImageFormat.Emf;
                if (format == System.Drawing.Imaging.ImageFormat.Gif) return ImageFormat.Gif;
                if (format == System.Drawing.Imaging.ImageFormat.Tiff) return ImageFormat.Tiff;
                if (format == System.Drawing.Imaging.ImageFormat.Png) return ImageFormat.Png;
                if (format == System.Drawing.Imaging.ImageFormat.MemoryBmp) return ImageFormat.MemoryBmp;
                if (format == System.Drawing.Imaging.ImageFormat.Jpeg) return ImageFormat.Jpeg;
            }

            return ImageFormat.Png;
        }
    }
}
