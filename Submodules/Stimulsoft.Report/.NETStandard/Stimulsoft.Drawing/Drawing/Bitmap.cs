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
using System.Drawing;
using System.IO;
using SixLabors.ImageSharp.Processing;
using Stimulsoft.Drawing.Imaging;

namespace Stimulsoft.Drawing
{
    public sealed class Bitmap : Image
    {
        internal System.Drawing.Bitmap NetBitmap => (System.Drawing.Bitmap)netImage;

        public void SetPixel(int x, int y, Color color)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                NetBitmap.SetPixel(x, y, color);
            else
            {
                RenderDrawingOperations();

                var sixColor = SixLabors.ImageSharp.Color.FromRgba(color.R, color.G, color.B, color.A);
                sixImage[x, y] = sixColor;
            }
        }

        public Color GetPixel(int x, int y)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return NetBitmap.GetPixel(x, y);
            else
            {
                RenderDrawingOperations();

                var color = sixImage[x, y];
                return Color.FromArgb(color.A, color.R, color.G, color.B);
            }
        }

        public void MakeTransparent()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                NetBitmap.MakeTransparent();
            else
            {
                RenderDrawingOperations();
                //throw new NotImplementedException();
            }
        }

        public void MakeTransparent(Color transparentColor)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                NetBitmap.MakeTransparent(transparentColor);
            else
            {
                RenderDrawingOperations();
                throw new NotImplementedException();
            }
        }

        public Bitmap GetThumbnailImage(int width, int height, object callback, IntPtr callbackData)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return new Bitmap()
                {
                    netImage = NetBitmap.GetThumbnailImage(width, height, null, callbackData)
                };
            else
            {
                RenderDrawingOperations();
                return new Bitmap()
                {
                    sixImage = sixImage.Clone(x => x.Resize(width, height))
                };
            }
                
        }

        public BitmapData LockBits(Rectangle rect, System.Drawing.Imaging.ImageLockMode readWrite, System.Drawing.Imaging.PixelFormat format32bppArgb)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
               return NetBitmap.LockBits(rect, readWrite, format32bppArgb);
            else
                throw new NotImplementedException();
        }

        public void UnlockBits(BitmapData bits)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
               NetBitmap.UnlockBits(bits);
            else
                throw new NotImplementedException();
        }

        public void SetResolution(float horizontalResolution, float verticalResolution)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                NetBitmap.SetResolution(horizontalResolution, verticalResolution);
            else
            {
                RenderDrawingOperations();
                //throw new NotImplementedException();
            }                
        }

        public Bitmap(int width, int height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImage = new System.Drawing.Bitmap(width, height);
            else
                sixImage = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(width, height);
        }

        public Bitmap(int width, int height, System.Drawing.Imaging.PixelFormat format)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImage = new System.Drawing.Bitmap(width, height, (System.Drawing.Imaging.PixelFormat)format);
            else
                sixImage = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(width, height);
        }
        
        public Bitmap(Stream stream)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImage = Image.FromStream(stream).netImage;
            else
            {
                var image = Image.FromStream(stream);
                stiImage = image.stiImage;
                sixImage = image.sixImage;
            }
        }

        public Bitmap(int width, int height, Graphics g) : this(width, height)
        {
        }

        public Bitmap(Bitmap bitmap) : this(bitmap as Image)
        {
        }

        public Bitmap(Image image)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImage = image.netImage.Clone() as System.Drawing.Image;
            else
            {
                image.RenderDrawingOperations();
                sixImage = Image.Create(image.sixImage);
            }
        }

        internal Bitmap()
        {

        }

        public static implicit operator System.Drawing.Bitmap(Bitmap image)
        {
            return image.NetBitmap;
        }

        public static implicit operator Bitmap(System.Drawing.Bitmap netImage)
        {
            return new Bitmap()
            {
                netImage = netImage
            };
        }
    }
}