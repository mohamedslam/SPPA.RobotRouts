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
using System.Collections.Generic;
using Stimulsoft.Drawing.Imaging;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using System.ComponentModel;

using Size = System.Drawing.Size;
using SizeF = System.Drawing.SizeF;
using DrawingOptions = SixLabors.ImageSharp.Drawing.Processing.DrawingOptions;
using ClippablePath = SixLabors.ImageSharp.Drawing.PolygonClipper.ClippablePath;

namespace Stimulsoft.Drawing
{
    [TypeConverter(typeof(ImageConverter))]
    public abstract class Image : IDisposable, ICloneable
    {
        private readonly List<Action<IImageProcessingContext>> drawingOperations = new List<Action<IImageProcessingContext>>();
        private SixLabors.ImageSharp.Rectangle? safeClipRectangle;

        internal SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> sixImage;
        internal System.Drawing.Image netImage;
        internal StiImageFormat stiImage;

        private ClippablePath? clipPath;
        internal ClippablePath? ClipPath
        {
            get
            {
                return clipPath;
            }
            set
            {
                RenderDrawingOperations();

                clipPath = value;

                safeClipRectangle = null;
                if (value.HasValue)
                {
                    TrySetSafeClipRectangle(value.Value);
                }
            }
        }

        private void ReloadSixImage()
        {
            if (stiImage != null)
            {
                sixImage = Create(SixLabors.ImageSharp.Image.Load(new MemoryStream(stiImage.DataBytes)));
                stiImage = null;
            }
        }

        private void TrySetSafeClipRectangle(ClippablePath value)
        {
            if (value.Path is SixLabors.ImageSharp.Drawing.RectangularPolygon rectPoly)
            {
                var location = new SixLabors.ImageSharp.Point((int)rectPoly.X, (int)rectPoly.Y);
                var size = new SixLabors.ImageSharp.Size((int)rectPoly.Width, (int)rectPoly.Height);
                safeClipRectangle = new SixLabors.ImageSharp.Rectangle(location, size);
            }
            else if (value.Path is SixLabors.ImageSharp.Drawing.Polygon poly && poly.Points.Length == 4)
            {
                var points = poly.Points.ToArray();
                var x0 = points[0].X;
                var y0 = points[0].Y;
                var x1 = points[1].X;
                var y1 = points[1].Y;
                var x2 = points[2].X;
                var y2 = points[2].Y;
                var x3 = points[3].X;
                var y3 = points[3].Y;

                if (y0 == y1 && x1 == x2 && y2 == y3 && x3 == x0)
                {
                    var minX = x0;
                    var maxX = x1;
                    if (x0 > x1)
                    {
                        minX = x1;
                        maxX = x0;
                    }

                    var minY = y0;
                    var maxY = y2;
                    if (y0 > y2)
                    {
                        minY = y2;
                        maxY = y0;
                    }

                    var width = maxX - minX;
                    var height = maxY - minY;
                    if (width < 1) width = 1;
                    if (height < 1) height = 1;

                    safeClipRectangle = new SixLabors.ImageSharp.Rectangle((int)minX, (int)minY, (int)width, (int)height);
                }
            }
        }

        private void ApplyDrawingOperations(IImageProcessingContext ctx)
        {
            foreach (var operation in drawingOperations)
            {
                operation(ctx);
            }

            drawingOperations.Clear();
        }

        internal void RenderDrawingOperations()
        {
            ReloadSixImage();

            if (drawingOperations.Count > 0)
            {
                if (!clipPath.HasValue)
                    sixImage.Mutate(ApplyDrawingOperations);
                else
                {
                    if (!safeClipRectangle.HasValue)
                    {
                        sixImage.Mutate(x => x.Clip(clipPath.Value.Path, ApplyDrawingOperations));
                    }
                    else
                    {
                        var clone = sixImage.Clone(x =>
                        {
                            ApplyDrawingOperations(x);
                            x.Crop(safeClipRectangle.Value);
                        });
                        sixImage.Mutate(x => x.DrawImage(clone, safeClipRectangle.Value.Location, 1));
                    }
                }
            }
        }

        internal void AddDrawingOperation(DrawingOptions DrawingOptions, Action<IImageProcessingContext, DrawingOptions> operation)
        {
            drawingOperations.Add(x => operation(x, DrawingOptions));
        }

        public int Height
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netImage.Height;
                else if (stiImage != null)
                    return stiImage.GetHeight();
                else
                    return sixImage.Height;
            }
        }

        public float HorizontalResolution
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netImage.HorizontalResolution;
                else if (stiImage != null)
                    return stiImage.GetHorizontalResolution();
                else
                    return (float)sixImage.Metadata.HorizontalResolution;
            }
        }

        public Size Size
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netImage.Size;
                else if (stiImage != null)
                    return new Size(stiImage.GetWidth(), stiImage.GetHeight());
                else
                    return new Size(Width, Height);
            }
        }

        public float VerticalResolution
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netImage.VerticalResolution;
                else if (stiImage != null)
                    return stiImage.GetVerticalResolution();
                else
                    return (float)sixImage.Metadata.VerticalResolution;
            }
        }

        public int Width
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netImage.Width;
                else if (stiImage != null)
                    return stiImage.GetWidth();
                else
                    return sixImage.Width;
            }
        }

        private SixLabors.ImageSharp.Formats.IImageFormat sixImageFormat;
        public ImageFormat RawFormat
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netImage.RawFormat;
                else if (stiImage != null)
                {
                    if (stiImage.Guid == "bmp") return ImageFormat.Bmp;
                    if (stiImage.Guid == "gif") return ImageFormat.Gif;
                    if (stiImage.Guid == "tiff") return ImageFormat.Tiff;
                    if (stiImage.Guid == "png") return ImageFormat.Png;
                    if (stiImage.Guid == "jpeg") return ImageFormat.Jpeg;
                }
                else
                {
                    if (sixImageFormat == SixLabors.ImageSharp.Formats.Bmp.BmpFormat.Instance) return ImageFormat.Bmp;
                    if (sixImageFormat == SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance) return ImageFormat.Gif;
                    if (sixImageFormat == SixLabors.ImageSharp.Formats.Tiff.TiffFormat.Instance) return ImageFormat.Tiff;
                    if (sixImageFormat == SixLabors.ImageSharp.Formats.Png.PngFormat.Instance) return ImageFormat.Png;
                    if (sixImageFormat == SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance) return ImageFormat.Jpeg;
                }

                return ImageFormat.Png;
            }
        }

        private System.Drawing.Imaging.PixelFormat pixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
        public System.Drawing.Imaging.PixelFormat PixelFormat
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netImage.PixelFormat;
                else
                    return pixelFormat;
            }
        }

        public SizeF PhysicalDimension
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netImage.PhysicalDimension;
                else
                    return Size;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Dispose()
        {
        }

        internal static SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> Create(SixLabors.ImageSharp.Image image)
        {
            var sixImage = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(image.Width, image.Height);
            sixImage.Mutate(i => i.DrawImage(image, 1));
            return sixImage;
        }

        public static Image FromFile(string filename)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return new Bitmap() { netImage = System.Drawing.Image.FromFile(filename) };
            else
            {
                return FromStream(new MemoryStream(File.ReadAllBytes(filename)));
            }
        }

        public static Image FromStream(Stream stream)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return new Bitmap() { netImage = System.Drawing.Image.FromStream(stream) };
            else
            {
                try
                {
                    byte[] imageBytes;
                    using (var reader = new BinaryReader(stream))
                    {
                        imageBytes = reader.ReadBytes((int)stream.Length);
                    }

                    var stiImage = StiImageFormat.GetImageFormat(imageBytes);
                    if (stiImage != null)
                        return new Bitmap() { stiImage = stiImage };
                }
                catch { }

                var bitmap = new Bitmap();
                bitmap.sixImage = Create(SixLabors.ImageSharp.Image.Load(stream, out bitmap.sixImageFormat));
                return bitmap;
            }
        }

        public static Image FromStream(Stream stream, bool useEmbeddedColorManagement)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return new Bitmap() { netImage = System.Drawing.Image.FromStream(stream, useEmbeddedColorManagement) };
            else
                return FromStream(stream);
        }

        public void Save(Stream stream, ImageCodecInfo encoder, EncoderParameters encoderParams)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImage.Save(stream, encoder, encoderParams);
            else
            {
                ImageFormat format = null;
                if (encoder.Clsid == ImageCodecInfo.BmpGuid)
                    format = ImageFormat.Bmp;
                if (encoder.Clsid == ImageCodecInfo.GifGuid)
                    format = ImageFormat.Gif;
                if (encoder.Clsid == ImageCodecInfo.JpegGuid)
                    format = ImageFormat.Jpeg;
                if (encoder.Clsid == ImageCodecInfo.PngGuid)
                    format = ImageFormat.Png;
                if (encoder.Clsid == ImageCodecInfo.TiffGuid)
                    format = ImageFormat.Tiff;

                Save(stream, format);
            }
        }

        public void Save(Stream stream, ImageFormat format)
        {
            if ((Graphics.GraphicsEngine == GraphicsEngine.Gdi) || (netImage != null && stiImage == null && sixImage == null))
                netImage.Save(stream, format);
            else
            {
                if (stiImage != null)
                {
                    if (RawFormat == format)
                        stream.Write(stiImage.DataBytes, 0, stiImage.DataBytes.Length);
                    else
                    {
                        ReloadSixImage();
                        Save(stream, format);
                    }
                }
                else
                {
                    RenderDrawingOperations();

                    if (format == ImageFormat.Jpeg)
                        sixImage.Mutate(x => x.BackgroundColor(SixLabors.ImageSharp.Color.White));

                    sixImage.Save(stream, format.SixImageEncoder);
                }
            }
        }

        public void SaveAdd(EncoderParameters encoderParams)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImage.SaveAdd(encoderParams);
            else
                throw new NotImplementedException();
        }

        public void SaveAdd(System.Drawing.Image image, EncoderParameters encoderParams)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImage.SaveAdd(image, encoderParams);
            else
                throw new NotImplementedException();
        }

        public void RotateFlip(System.Drawing.RotateFlipType type)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImage.RotateFlip(type);
            else
            {
                RenderDrawingOperations();

                var flag = (int)type;
                var rotateMode = RotateMode.None;
                var flipMode = FlipMode.None;
                if (flag >= 4)
                {
                    flag -= 4;
                    flipMode = FlipMode.Horizontal;
                }

                if (flag == 1) rotateMode = RotateMode.Rotate90;
                if (flag == 2) rotateMode = RotateMode.Rotate180;
                if (flag == 3) rotateMode = RotateMode.Rotate270;

                sixImage.Mutate(x => x.RotateFlip(rotateMode, flipMode));
            }
        }

        public static implicit operator System.Drawing.Image(Image image)
        {
            return image.netImage;
        }

        public static implicit operator Image(System.Drawing.Image netImage)
        {
            return new Bitmap()
            {
                netImage = netImage
            };
        }
    }
}