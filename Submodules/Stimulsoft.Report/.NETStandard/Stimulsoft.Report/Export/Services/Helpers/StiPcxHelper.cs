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
using BitmapData = Stimulsoft.Drawing.Imaging.BitmapData;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Export
{
	internal class StiPcxHelper
	{
		#region Methods.Data Write
		private static void dwByte(byte[] bfData, int position, byte dataValue)	//1
		{
			bfData[position] = dataValue;
		}

		private static void dwByte(byte[] bfData, int position, byte dv1, byte dv2)	//2
		{
			dwByte(bfData, position, dv1);
			dwByte(bfData, position+1, dv2);
		}

		private static void dwByte(byte[] bfData, int position, byte dv1, byte dv2, byte dv3)	//3
		{
			dwByte(bfData, position, dv1, dv2);
			dwByte(bfData, position+2, dv3);
		}

		private static void dwWord(byte[] bfData, int position, Int16 dataValue)
		{
			bfData[position + 0] = (byte)(dataValue & 0xff);
			bfData[position + 1] = (byte)((dataValue >> 8) & 0xff);
		}

		private static void dwEncodedData(Stream stream, byte byt, int runLen)
		{
		    if (runLen == 1 && (byt & 0xC0) != 0xC0)
		        stream.WriteByte(byt);

		    else
		    {
		        stream.WriteByte((byte) (runLen | 0xC0));
		        stream.WriteByte(byt);
		    }
		}
		#endregion

		#region Methods.SaveToStream
		public static void SaveToStream(Image imageForExport, StiPcxPaletteType paletteType, StiMonochromeDitheringType ditheringType, Stream stream)
		{
			#region Prepare bitmap
			Bitmap bitmap = null;
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
			int stride;
			byte[] pixelsData;
            if (isGdi)
			{
				// Lock the bitmap's bits
				Rectangle rect = new Rectangle(0, 0, imageWidth, imageHeight);
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

            int bytesPerLine = 0;
			byte[] buf = null;

			if (paletteType == StiPcxPaletteType.Monochrome)
			{
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
                bytesPerLine = ((imageWidth - 1) / 16 + 1) * 2;
                buf = new byte[bytesPerLine * imageHeight];
				for (int indexY = 0; indexY < imageHeight; indexY++)
				{
					byte bitPos = 0x80;
					int bytePos = indexY * bytesPerLine;
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
			}
			if (paletteType == StiPcxPaletteType.Color)
			{
				#region Make color image
				bytesPerLine = ((imageWidth - 1) / 2 + 1) * 2;
				buf = new byte[bytesPerLine * 3 * imageHeight];
				for (int indexY = 0; indexY < imageHeight; indexY++)
				{
					int offset1 = indexY * stride;
					int offset2 = bytesPerLine * 3 * indexY; 
					for (int indexX = 0; indexX < imageWidth; indexX++)
					{
						buf[offset2 + bytesPerLine * 0] = pixelsData[offset1 + 2];
						buf[offset2 + bytesPerLine * 1] = pixelsData[offset1 + 1];
						buf[offset2 + bytesPerLine * 2] = pixelsData[offset1 + 0];
						offset1 += 4;
						offset2 += 1;
					}
				}
				#endregion
			}

			#region Prepare header
			byte[] header = new byte[128];
			dwByte(header, 0, 10);	//Constant Flag, 10 = ZSoft .pcx 
			dwByte(header, 1, 5);	//Version information 
			dwByte(header, 2, 1);	//1 = .PCX run length encoding
			if (paletteType == StiPcxPaletteType.Monochrome)
			{
				dwByte(header, 3, 1);	//Number of bits to represent a pixel (per Plane)
			}
			if (paletteType == StiPcxPaletteType.Color)
			{
				dwByte(header, 3, 8);	//Number of bits to represent a pixel (per Plane)
			}
			dwWord(header, 4, 0);	//Xmin
			dwWord(header, 6, 0);	//Ymin
			dwWord(header, 8, (Int16)(imageWidth - 1));	//Xmax
			dwWord(header, 10, (Int16)(imageHeight - 1));	//Ymax
			dwWord(header, 12, 72);	//HDpi
			dwWord(header, 14, 72);	//VDpi
			dwByte(header, 16, 0x00, 0x00, 0x00);	//Colormap, Color0 - Black
			dwByte(header, 19, 0xFF, 0xFF, 0xFF);	//Colormap, Color1 - White
			dwByte(header, 64, 0);	//Reserved
			if (paletteType == StiPcxPaletteType.Monochrome)
			{
				dwByte(header, 65, 1);	//Number of color planes 
			}
			if (paletteType == StiPcxPaletteType.Color)
			{
				dwByte(header, 65, 3);	//Number of color planes 
			}
			dwWord(header, 66, (Int16)bytesPerLine);	//Number of bytes to allocate for a scanline plane
			dwWord(header, 68, 1);	//How to interpret palette - 1 = Color/BW
			#endregion

			stream.Write(header, 0, 128);

			#region Pack data
			int runLen = 1;
			byte lastByte = buf[0];
			for (int indexByte = 1; indexByte < buf.Length; indexByte++)
			{
				byte thisByte = buf[indexByte];
				if (lastByte == thisByte)
				{
					runLen++;
					if ((runLen == 63) || (((indexByte + 1) % bytesPerLine) == 0))
					{
						dwEncodedData(stream, lastByte, runLen);
						runLen = 0;
					}
				}
				else
				{
					if (runLen != 0)
					{
						dwEncodedData(stream, lastByte, runLen);
					}
					lastByte = thisByte;
					runLen = 1;
				}
			}
			if (runLen != 0)
			{
				dwEncodedData(stream, lastByte, runLen);
			}
			#endregion

			stream.Flush();
		}
		#endregion
    }
}
