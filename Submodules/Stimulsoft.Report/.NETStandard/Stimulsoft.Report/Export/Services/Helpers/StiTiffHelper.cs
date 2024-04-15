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
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
using BitmapData = Stimulsoft.Drawing.Imaging.BitmapData;
#endif

namespace Stimulsoft.Report.Export
{
    public class StiTiffHelper
    {
        #region Methods
        /// <summary>
        /// Make monochrome image (1 bit per plane) from normal 32bit image
        /// </summary>
        /// <param name="inputImage">Input image 32bit</param>
        /// <param name="ditheringType">Dithering type (None, FloydSteinberg or Ordered)</param>
        /// <param name="diffusionPower">Power of diffusion, from 1 to 255; default value is 128</param>
        /// <returns>Monochrome image</returns>
        public static Image MakeMonochromeImage(Image inputImage, StiMonochromeDitheringType ditheringType, int diffusionPower)
        {
            #region Prepare bitmap
            Bitmap bitmap = null;
            if (inputImage is Bitmap)
            {
                bitmap = inputImage as Bitmap;
            }
            else
            {
                bitmap = new Bitmap(inputImage.Width, inputImage.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(inputImage, 0, 0, inputImage.Width, inputImage.Height);
                }
            }
            int imageWidth = bitmap.Width;
            int imageHeight = bitmap.Height;
            #endregion

            #region Prepare pixels data 
            var isGdi = true;
#if STIDRAWING
            isGdi = Graphics.GraphicsEngine == Stimulsoft.Drawing.GraphicsEngine.Gdi;
#endif

            Rectangle rect = new Rectangle(0, 0, imageWidth, imageHeight);
            int stride;
            byte[] pixelsData;
            if (isGdi)
            {
                // Lock the bitmap's bits
                BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                stride = Math.Abs(bitmapData.Stride);

                // Declare an array to hold the bytes of the bitmap.
                int pixelsDataCount = stride * bitmapData.Height;
                pixelsData = new byte[pixelsDataCount];

                // Copy the bitmap pixel values into the array.
                Marshal.Copy(bitmapData.Scan0, pixelsData, 0, pixelsDataCount);

                // Unlock the bits.
                bitmap.UnlockBits(bitmapData);
            }
            else
            {
                stride = imageWidth * 4;
                pixelsData = new byte[stride * imageHeight];
                int offset = 0;
                for (int y = 0; y < imageHeight; y++)
                {
                    for (int x = 0; x < imageWidth; x++)
                    {
                        var color = bitmap.GetPixel(x, y);
                        pixelsData[offset++] = color.B;
                        pixelsData[offset++] = color.G;
                        pixelsData[offset++] = color.R;
                        pixelsData[offset++] = color.A;
                    }
                }
            }
            #endregion

            #region Make monochrome data
            byte[,] buf2 = new byte[imageHeight + 1, imageWidth + 1];
            for (int indexY = 0; indexY < imageHeight; indexY++)
            {
                for (int indexX = 0; indexX < imageWidth; indexX++)
                {
                    buf2[indexY, indexX] = 128;
                }
            }

            int[,] bayerMatrix = new int[4, 4]
                {
                    { 1,  9,  3, 11},
                    {13,  5, 15,  7},
                    { 4, 12,  2, 10},
                    {16,  8, 14,  6}
                };

            for (int indexY = 0; indexY < imageHeight; indexY++)
            {
                int offset = indexY * stride;
                for (int indexX = 0; indexX < imageWidth; indexX++)
                {
                    //need optimize - from float to int operations
                    int oldPixel = (int)(pixelsData[offset + 2] * 0.3f + pixelsData[offset + 1] * 0.59f + pixelsData[offset + 0] * 0.11f);
                    offset += 4;
                    byte newPixel = 0;

                    if (ditheringType == StiMonochromeDitheringType.None)
                    {
                        #region Without dithering
                        newPixel = (byte)(oldPixel >= diffusionPower ? 255 : 0);
                        #endregion
                    }
                    if (ditheringType == StiMonochromeDitheringType.FloydSteinberg)
                    {
                        #region Floyd-Steinberg dithering
                        oldPixel += buf2[indexY, indexX] - 128;
                        newPixel = (byte)(oldPixel >= diffusionPower ? 255 : 0);
                        int quant_error = oldPixel - newPixel;
                        buf2[indexY, indexX + 1] += (byte)(7f / 16f * quant_error);
                        if (indexX != 0)
                        {
                            buf2[indexY + 1, indexX - 1] += (byte)(3f / 16f * quant_error);
                        }
                        buf2[indexY + 1, indexX + 0] += (byte)(5f / 16f * quant_error);
                        buf2[indexY + 1, indexX + 1] += (byte)(1f / 16f * quant_error);
                        #endregion
                    }
                    if (ditheringType == StiMonochromeDitheringType.Ordered)
                    {
                        #region Ordered dithering (Bayer matrix)
                        newPixel = (byte)(oldPixel >= diffusionPower * 2 / 17f * bayerMatrix[indexX & 0x03, indexY & 0x03] ? 255 : 0);
                        #endregion
                    }

                    buf2[indexY, indexX] = newPixel;
                }
            }
            #endregion

            if (!isGdi)
            {
                #region Prepare new bitmap - StiDrawing
                //ImageSharp does not support 1bpp images, so use standard Rgba32
                Bitmap newBitmap2 = new Bitmap(imageWidth, imageHeight);

                for (int indexY = 0; indexY < imageHeight; indexY++)
                {
                    for (int indexX = 0; indexX < imageWidth; indexX++)
                    {
                        var color = buf2[indexY, indexX] != 0 ? Color.White : Color.Black;
                        newBitmap2.SetPixel(indexX, indexY, color);
                    }
                }
                #endregion

                return newBitmap2;

            }

            #region Prepare new bitmap
            Bitmap newBitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format1bppIndexed);

            // Lock the bitmap's bits
            BitmapData bitmapData2 = newBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            int stride2 = Math.Abs(bitmapData2.Stride);
            #endregion

            #region Prepare bits data
            byte[] buf = new byte[stride2 * imageHeight];
            for (int indexY = 0; indexY < imageHeight; indexY++)
            {
                byte bitPos = 0x80;
                int bytePos = indexY * stride2;
                byte val = 0;
                for (int indexX = 0; indexX < imageWidth; indexX++)
                {
                    if (buf2[indexY, indexX] != 0)
                    {
                        val |= bitPos;
                    }
                    bitPos >>= 1;
                    if (bitPos == 0)
                    {
                        buf[bytePos] = val;
                        val = 0;
                        bitPos = 0x80;
                        bytePos++;
                    }
                }
                if (bitPos != 0x80)
                {
                    buf[bytePos] = val;
                }
            }
            #endregion

            #region Store data to image
            // Copy the bitmap pixel values into the array.
            Marshal.Copy(buf, 0, bitmapData2.Scan0, buf.Length);

            // Unlock the bits.
            newBitmap.UnlockBits(bitmapData2);
            #endregion

            return newBitmap;
        }
        #endregion
    }
}
