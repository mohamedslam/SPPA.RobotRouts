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
    internal class StiBmpHelper
    {
        #region Methods.Data Write
        private static void dwByte(byte[] bfData, int position, byte dataValue)	//1
        {
            bfData[position] = dataValue;
        }

        private static void dwByte(byte[] bfData, int position, byte dv1, byte dv2)	//2
        {
            dwByte(bfData, position, dv1);
            dwByte(bfData, position + 1, dv2);
        }

        private static void dwWord(byte[] bfData, int position, Int16 dataValue)
        {
            bfData[position + 0] = (byte)(dataValue & 0xff);
            bfData[position + 1] = (byte)((dataValue >> 8) & 0xff);
        }

        private static void dwDWord(byte[] bfData, int position, Int32 dataValue)
        {
            bfData[position + 0] = (byte)(dataValue & 0xff);
            bfData[position + 1] = (byte)((dataValue >> 8) & 0xff);
            bfData[position + 2] = (byte)((dataValue >> 16) & 0xff);
            bfData[position + 3] = (byte)((dataValue >> 24) & 0xff);
        }
        #endregion

        #region Method.SaveToStreamMonochrome
        public static void SaveToStreamMonochrome(Image imageForExport, StiMonochromeDitheringType ditheringType, Stream stream)
        {
            #region Prepare bitmap
            Bitmap bitmap;
            if (imageForExport is Bitmap)
            {
                bitmap = imageForExport as Bitmap;
            }
            else
            {
                bitmap = new Bitmap(imageForExport.Width, imageForExport.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(imageForExport, 0, 0, imageForExport.Width, imageForExport.Height);
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

            #region Make monochrome image
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
                    int oldPixel = (int)(pixelsData[offset + 2] * 0.3f + pixelsData[offset + 1] * 0.59f + pixelsData[offset + 0] * 0.11f);
                    offset += 4;
                    byte newPixel = 0;

                    if (ditheringType == StiMonochromeDitheringType.None)
                    {
                        #region Without dithering
                        newPixel = (byte)(oldPixel >= 128 ? 255 : 0);
                        #endregion
                    }
                    if (ditheringType == StiMonochromeDitheringType.FloydSteinberg)
                    {
                        #region Floyd-Steinberg dithering
                        oldPixel += buf2[indexY, indexX] - 128;
                        newPixel = (byte)(oldPixel >= 128 ? 255 : 0);
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
                        newPixel = (byte)(oldPixel >= 255 / 17f * bayerMatrix[indexX & 0x03, indexY & 0x03] ? 255 : 0);
                        #endregion
                    }

                    buf2[indexY, indexX] = newPixel;
                }
            }
            #endregion

            #region Prepare bits data
            int bytesPerLine = ((imageWidth - 1) / 32 + 1) * 4;
            int bitmapSize = bytesPerLine * imageHeight;
            byte[] buf = new byte[bitmapSize];
            for (int indexY = 0; indexY < imageHeight; indexY++)
            {
                byte bitPos = 0x80;
                int bytePos = (imageHeight - 1 - indexY) * bytesPerLine;
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

            #region Prepare header
            int headerSize = 0x3E;
            int fileSize = headerSize + bitmapSize;
            byte[] header = new byte[headerSize];

            //BITMAPFILEHEADER
            dwByte(header, 0x00, 0x42, 0x4D);	//Identifier ?BM? - Windows
            dwDWord(header, 0x02, fileSize);	//Complete file size in bytes
            dwDWord(header, 0x06, 0);	        //Reserved for later use
            dwDWord(header, 0x0A, 0x003E);      //Offset from beginning of file to the beginning of the bitmap data

            //BITMAPINFOHEADER
            dwDWord(header, 0x0E, 0x0028);	    //Length of the Bitmap Info Header
            dwDWord(header, 0x12, imageWidth);  //Horizontal width of bitmap in pixels
            dwDWord(header, 0x16, imageHeight);	//Vertical height of bitmap in pixels
            dwWord(header, 0x1A, 1);	        //Number of planes in this bitmap
            dwWord(header, 0x1C, 1);	        //Bits per pixel; 1 - Monochrome bitmap
            dwDWord(header, 0x1E, 0);	        //Compression specifications; 0 - none
            dwDWord(header, 0x22, bitmapSize);  //Size of the bitmap data in bytes
            dwDWord(header, 0x26, 0x0F61);      //Horizontal resolution expressed in pixel per meter
            dwDWord(header, 0x2A, 0x0F61);      //Vertical resolution expressed in pixels per meter
            dwDWord(header, 0x2E, 0);           //Number of colors used by this bitmap; 0 - all
            dwDWord(header, 0x32, 0);           //Number of important colors; 0 - all

            //Palette
            dwDWord(header, 0x36, 0x000000);    //Black
            dwDWord(header, 0x3A, 0xFFFFFF);    //White
            #endregion

            stream.Write(header, 0, headerSize);
            stream.Write(buf, 0, bitmapSize);

            stream.Flush();
        }
        #endregion
    }
}
