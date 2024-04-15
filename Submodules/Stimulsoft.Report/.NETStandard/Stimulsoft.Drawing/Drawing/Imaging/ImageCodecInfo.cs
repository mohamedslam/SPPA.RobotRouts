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

using System.Runtime.InteropServices;
using System;

namespace Stimulsoft.Drawing.Imaging
{
    public sealed class ImageCodecInfo
    {
        internal const string BmpGuid = "557cf400-1a04-11d3-9a73-0000f81ef32e";
        //internal const string EmfGuid = "b96b3cac-0728-11d3-9d7b-0000f81ef32e";
        //internal const string ExifGuid = "b96b3cb2-0728-11d3-9d7b-0000f81ef32e";
        internal const string GifGuid = "557cf402-1a04-11d3-9a73-0000f81ef32e";
        internal const string TiffGuid = "557cf405-1a04-11d3-9a73-0000f81ef32e";
        internal const string PngGuid = "557cf406-1a04-11d3-9a73-0000f81ef32e";
        //internal const string MemoryBmpGuid = "b96b3caa-0728-11d3-9d7b-0000f81ef32e";
        //internal const string IconGuid = "b96b3cb5-0728-11d3-9d7b-0000f81ef32e";
        internal const string JpegGuid = "557cf401-1a04-11d3-9a73-0000f81ef32e";
        //internal const string WmfGuid = "b96b3cad-0728-11d3-9d7b-0000f81ef32e";

        public static ImageCodecInfo[] GetImageEncoders()
        {
            return new ImageCodecInfo[]
            {
                new ImageCodecInfo()
                {
                    MimeType = SixLabors.ImageSharp.Formats.Bmp.BmpFormat.Instance.DefaultMimeType,
                    Clsid = ImageCodecInfo.BmpGuid
                },
                new ImageCodecInfo()
                {
                    MimeType = SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance.DefaultMimeType,
                    Clsid = ImageCodecInfo.GifGuid
                },
                new ImageCodecInfo()
                {
                    MimeType = SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance.DefaultMimeType,
                    Clsid = ImageCodecInfo.JpegGuid
                },
                //new ImageCodecInfo()
                //{
                //    MimeType = SixLabors.ImageSharp.Formats.Pbm.PbmFormat.Instance.DefaultMimeType,
                //    Clsid = ImageCodecInfo.PbmGuid
                //},
                new ImageCodecInfo()
                {
                    MimeType = SixLabors.ImageSharp.Formats.Png.PngFormat.Instance.DefaultMimeType,
                    Clsid = ImageCodecInfo.PngGuid
                },
                //new ImageCodecInfo()
                //{
                //    MimeType = SixLabors.ImageSharp.Formats.Tga.TgaFormat.Instance.DefaultMimeType,
                //    Clsid = ImageCodecInfo.TgaGuid
                //},
                new ImageCodecInfo()
                {
                    MimeType = SixLabors.ImageSharp.Formats.Tiff.TiffFormat.Instance.DefaultMimeType,
                    Clsid = ImageCodecInfo.TiffGuid
                }/*,
                //new ImageCodecInfo()
                //{
                //    MimeType = SixLabors.ImageSharp.Formats.Webp.WebpFormat.Instance.DefaultMimeType,
                //    Clsid = ImageCodecInfo.WebpGuid
                //}*/
            };
        }

        public string MimeType { get; set; }
        public string Clsid { get; set; }
        public Guid FormatID { get; set; }

        public static implicit operator System.Drawing.Imaging.ImageCodecInfo(ImageCodecInfo codecInfo)
        {
            foreach(var encoder in System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders())
            {
                if (encoder.Clsid.ToString() == codecInfo.Clsid) return encoder;
            }

            return null;
        }
    }
}
