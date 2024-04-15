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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using BitmapData = Stimulsoft.Drawing.Imaging.BitmapData;
#endif

namespace Stimulsoft.Report.Export
{
    class StiColorQuantization
    {
        #region Methods.Private

        #region CreateColorMap
        private static int[] CreateColorMap(Bitmap bitmap, ref byte[] pixelsData, ref long usedMemoryCounter)
        {
            if (Stimulsoft.Report.StiOptions.Engine.FullTrust) return CreateColorMapFullTrust(bitmap, ref pixelsData, ref usedMemoryCounter);

            var buf1 = new int[64 * 64 * 64];

            //count how many times each color met
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var color = bitmap.GetPixel(x, y);
                    int offset = (color.R >> 2) << 12 | (color.G >> 2) << 6 | (color.B >> 2);
                    if (buf1[offset] < 65535) buf1[offset]++;
                }
            }

            return buf1;
        }

        private static int[] CreateColorMapFullTrust(Bitmap bitmap, ref byte[] pixelsData, ref long usedMemoryCounter)
        {
            int bitmapWidth = bitmap.Width;
            int bitmapHeight = bitmap.Height;

            // Lock the bitmap's bits
            Rectangle rect = new Rectangle(0, 0, bitmapWidth, bitmapHeight);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int stride = Math.Abs(bitmapData.Stride);

            // Declare an array to hold the bytes of the bitmap.
            int pixelsDataCount = stride * bitmapData.Height;
            pixelsData = new byte[pixelsDataCount];

            // Copy the bitmap pixel values into the array.
            Marshal.Copy(bitmapData.Scan0, pixelsData, 0, pixelsDataCount);

            // Unlock the bits.
            bitmap.UnlockBits(bitmapData);

            //create color map
            var map = new int[64 * 64 * 64];
            for (int y = 0; y < bitmapHeight; y++)
            {
                int offs1 = y * stride;
                int x = bitmapWidth;
                while (x > 0)
                {
                    int mapOffset = (pixelsData[offs1 + 2] >> 2) << 12 | (pixelsData[offs1 + 1] >> 2) << 6 | (pixelsData[offs1 + 0] >> 2);
                    if (map[mapOffset] < 65535) map[mapOffset]++;
                    offs1 += 4;
                    x--;
                }
            }

            usedMemoryCounter += pixelsData.Length;

            return map;
        }
        #endregion

        #region Get Palette

        #region GetPaletteV1
        private static Color[] GetPaletteV1(int[] colorMap, int count)
        {
            //count colors
            var buf2 = new int[65536];
            for (int index = 0; index < colorMap.Length; index++)
            {
                buf2[colorMap[index]]++;
            }

            //select maximum values
            int minColorIndex = 65535;
            var counter = 0; 
            for (int index = 65535; index >= 0; index--)
            {
                var cCount = buf2[index];
                if (cCount > 0)
                {
                    minColorIndex = index;
                    counter += cCount;
                    if (counter > count) break;
                }
            }

            //fill palette
            var palette = new Color[counter];
            counter = 0;
            for (int index = 0; index < colorMap.Length; index++)
            {
                if (colorMap[index] >= minColorIndex)
                {
                    palette[counter] = Color.FromArgb((index >> 10) & 0xfc, (index >> 4) & 0xfc, (index << 2) & 0xfc);
                    counter++;
                }
            }

            return palette;
        }
        #endregion

        #region GetPaletteV2
        private static Color[] GetPaletteV2(int[] colorMap, int count)
        {
            var palette = new Color[count];

            int offset = 3 + (int)(96 / count);    //empiric

            var colorCounter = 0;
            while (colorCounter < count)
            {
                //get max value
                int max = 0;
                int indexMax = 0;
                for (int index = 0; index < colorMap.Length; index++)
                {
                    if (colorMap[index] > max)
                    {
                        max = colorMap[index];
                        indexMax = index;
                    }
                }

                //store color to palette
                int r = (indexMax >> 10) & 0xfc;
                int g = (indexMax >> 4) & 0xfc;
                int b = (indexMax << 2) & 0xfc;
                palette[colorCounter] = Color.FromArgb(r, g, b);

                //remove color from map
                r >>= 2;
                g >>= 2;
                b >>= 2;
                for (int iR = Math.Max(r - offset, 0); iR < Math.Min(r + offset, 64); iR++)
                {
                    for (int iG = Math.Max(g - offset, 0); iG < Math.Min(g + offset, 64); iG++)
                    {
                        for (int iB = Math.Max(b - offset, 0); iB < Math.Min(b + offset, 64); iB++)
                        {
                            colorMap[(iR << 12) + (iG << 6) + iB] = 1;  //not zero - mark that cell is used
                        }
                    }
                }

                colorCounter++;
            }

            return palette;
        }
        #endregion

        #region GetPaletteV3
        private static Color[] GetPaletteV3(int[] colorMap, int count)
        {
            //count colors by axes
            long[,] colorsByAxes = new long[3, 64];
            int offs = 0;
            for (int indexR = 0; indexR < 64; indexR++)
            {
                for (int indexG = 0; indexG < 64; indexG++)
                {
                    for (int indexB = 0; indexB < 64; indexB++)
                    {
                        int value = colorMap[offs++];
                        colorsByAxes[0, indexR] += value;
                        colorsByAxes[1, indexG] += value;
                        colorsByAxes[2, indexB] += value;
                    }
                }
            }

            //create main Pip
            Pip pip1 = new Pip() { X2 = 63, Y2 = 63, Z2 = 63 };
            while (colorsByAxes[0, pip1.X1] == 0) pip1.X1++;
            while (colorsByAxes[0, pip1.X2] == 0) pip1.X2--;
            while (colorsByAxes[1, pip1.Y1] == 0) pip1.Y1++;
            while (colorsByAxes[1, pip1.Y2] == 0) pip1.Y2--;
            while (colorsByAxes[2, pip1.Z1] == 0) pip1.Z1++;
            while (colorsByAxes[2, pip1.Z2] == 0) pip1.Z2--;

            List<Pip> pips = new List<Pip>();
            pips.Add(pip1);
            DivPip(pips, count, colorsByAxes);

            var palette = new Color[count];
            for (int index = 0; index < count; index++)
            {
                Pip pip = pips[index];
                int r = GetMax(pip.X1, pip.X2, 0, colorsByAxes) * 4;
                int g = GetMax(pip.Y1, pip.Y2, 1, colorsByAxes) * 4;
                int b = GetMax(pip.Z1, pip.Z2, 2, colorsByAxes) * 4;
                palette[index] = Color.FromArgb(r, g, b);
            }

            return palette;
        }

        private static void DivPip(List<Pip> pips, int count, long[,] colorsByAxes)
        {
            while (true)
            {
                int indexPip = pips.Count - 1;
                while (indexPip >= 0)
                {
                    if (pips.Count >= count) return;

                    Pip pip = pips[indexPip];

                    int limit = 4;
                    if (pip.XW < limit || pip.YW < limit || pip.ZW < limit)
                    {
                        indexPip--;
                        continue;
                    }

                    Pip pip1 = null;
                    Pip pip2 = null;

                    if (pip.XW > pip.YW)
                    {
                        if (pip.XW >= pip.ZW)
                        {
                            int med = GetMedian(pip.X1, pip.X2, 0, colorsByAxes);
                            pip1 = new Pip() { X1 = pip.X1, X2 = med, Y1 = pip.Y1, Y2 = pip.Y2, Z1 = pip.Z1, Z2 = pip.Z2 };
                            pip2 = new Pip() { X1 = med + 1, X2 = pip.X2, Y1 = pip.Y1, Y2 = pip.Y2, Z1 = pip.Z1, Z2 = pip.Z2 };
                        }
                        else
                        {
                            int med = GetMedian(pip.Z1, pip.Z2, 2, colorsByAxes);
                            pip1 = new Pip() { X1 = pip.X1, X2 = pip.X2, Y1 = pip.Y1, Y2 = pip.Y2, Z1 = pip.Z1, Z2 = med };
                            pip2 = new Pip() { X1 = pip.X1, X2 = pip.X2, Y1 = pip.Y1, Y2 = pip.Y2, Z1 = med + 1, Z2 = pip.Z2 };
                        }
                    }
                    else
                    {
                        if (pip.YW >= pip.ZW)
                        {
                            int med = GetMedian(pip.Y1, pip.Y2, 1, colorsByAxes);
                            pip1 = new Pip() { X1 = pip.X1, X2 = pip.X2, Y1 = pip.Y1, Y2 = med, Z1 = pip.Z1, Z2 = pip.Z2 };
                            pip2 = new Pip() { X1 = pip.X1, X2 = pip.X2, Y1 = med + 1, Y2 = pip.Y2, Z1 = pip.Z1, Z2 = pip.Z2 };
                        }
                        else
                        {
                            int med = GetMedian(pip.Z1, pip.Z2, 2, colorsByAxes);
                            pip1 = new Pip() { X1 = pip.X1, X2 = pip.X2, Y1 = pip.Y1, Y2 = pip.Y2, Z1 = pip.Z1, Z2 = med };
                            pip2 = new Pip() { X1 = pip.X1, X2 = pip.X2, Y1 = pip.Y1, Y2 = pip.Y2, Z1 = med + 1, Z2 = pip.Z2 };
                        }
                    }

                    pips.RemoveAt(indexPip);
                    pips.Add(pip1);
                    pips.Add(pip2);

                    indexPip--;
                }
            }
        }

        private static int GetMedian(int a, int b, int axis, long[,] colorsByAxes)
        {
            long sum1 = 0;
            for (int index = a; index <= b; index++)
            {
                sum1 += colorsByAxes[axis, index];
            }
            long sum2 = 0;

            int i = b;
            while ((i > a) && (sum2 < sum1))
            {
                long value = colorsByAxes[axis, i];
                sum1 -= value;
                sum2 += value;
                i--;
            }

            if (i == b) return a;
            return i;
        }

        private static int GetMax(int a, int b, int axis, long[,] colorsByAxes)
        {
            long max = 0;
            int indexMax = a;
            for (int index = a; index <= b; index++)
            {
                long value = colorsByAxes[axis, index];
                if (max < value)
                {
                    max = value;
                    indexMax = index;
                }
            }
            return indexMax;
        }

        private class Pip
        {
            public int X1;
            public int X2;
            public int Y1;
            public int Y2;
            public int Z1;
            public int Z2;

            public int XW => X2 - X1;
            public int YW => Y2 - Y1;
            public int ZW => Z2 - Z1;
        }
        #endregion

        #endregion

        #region Prepare colorMap for fast conversion
        private static void PrepareColorMapForFastConversion(int[] colorMap, Color[] palette, StiMonochromeDitheringType ditheringType)
        {
            for (int indexMap = 0; indexMap < colorMap.Length; indexMap++)
            {
                if (colorMap[indexMap] == 0 && ditheringType == StiMonochromeDitheringType.None) continue;

                int r = (indexMap >> 10) & 0xfc;
                int g = (indexMap >> 4) & 0xfc;
                int b = (indexMap << 2) & 0xfc;

                int minOffs = int.MaxValue;
                int newColorIndex = 0;

                for (int index = 0; index < palette.Length; index++)
                {
                    Color pColor = palette[index];
                    int offs = (r - pColor.R) * (r - pColor.R) + (g - pColor.G) * (g - pColor.G) + (b - pColor.B) * (b - pColor.B);
                    if (offs < minOffs)
                    {
                        minOffs = offs;
                        newColorIndex = index;
                    }
                }

                colorMap[indexMap] = newColorIndex;
            }
        }
        #endregion

        #endregion

        #region Methods.Public

        #region Convert Image to Indexed Image
        public static Image ConvertImage(Bitmap source, int count, StiMonochromeDitheringType ditheringType)
        {
            long usedMemoryCounter = 0;
            byte[] pixelsData = null;
            int[] colorMap = CreateColorMap(source, ref pixelsData, ref usedMemoryCounter);
            var palette = GetPaletteV2(colorMap, count);
            var dest = ConvertToPalette(source, pixelsData, colorMap, palette, ditheringType, ref usedMemoryCounter);
            return dest;
        }

        private static Image ConvertToPalette(Bitmap oldBitmap, byte[] oldPixelsData, int[] colorMap, Color[] palette, StiMonochromeDitheringType ditheringType, ref long usedMemoryCounter)
        {
            if (Stimulsoft.Report.StiOptions.Engine.FullTrust) return ConvertToPaletteFullTrust(oldBitmap, oldPixelsData, colorMap, palette, ditheringType, ref usedMemoryCounter);

            Bitmap dest = new Bitmap(oldBitmap.Width, oldBitmap.Height);

            for (int y = 0; y < oldBitmap.Height; y++)
            {
                for (int x = 0; x < oldBitmap.Width; x++)
                {
                    var color = oldBitmap.GetPixel(x, y);

                    int minOffs = int.MaxValue;
                    int newColorIndex = 0;
                    for (int index = 0; index < palette.Length; index++)
                    {
                        Color pColor = palette[index];
                        int offs = (color.R - pColor.R) * (color.R - pColor.R) + (color.G - pColor.G) * (color.G - pColor.G) + (color.B - pColor.B) * (color.B - pColor.B);
                        if (offs < minOffs)
                        {
                            minOffs = offs;
                            newColorIndex = index;
                        }
                    }

                    dest.SetPixel(x, y, palette[newColorIndex]);
                }
            }

            return dest;
        }

        private static Image ConvertToPaletteFullTrust(Bitmap oldBitmap, byte[] oldPixelsData, int[] colorMap, Color[] palette, StiMonochromeDitheringType ditheringType, ref long usedMemoryCounter)
        {
            PrepareColorMapForFastConversion(colorMap, palette, ditheringType);

            int bitmapWidth = oldBitmap.Width;
            int bitmapHeight = oldBitmap.Height;

            Bitmap dest = new Bitmap(bitmapWidth, bitmapHeight);

            // Lock the bitmap's bits
            Rectangle rect = new Rectangle(0, 0, bitmapWidth, bitmapHeight);
            BitmapData bitmapData = dest.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int stride = Math.Abs(bitmapData.Stride);

            // Declare an array to hold the bytes of the bitmap.
            int pixelsDataCount = stride * bitmapData.Height;
            byte[] pixelsData = new byte[pixelsDataCount + stride + 4]; //"+stride+4" is for FloydSteinberg

            if (ditheringType == StiMonochromeDitheringType.None)
            {
                for (int y = 0; y < bitmapHeight; y++)
                {
                    int offs1 = y * stride;
                    int x = bitmapWidth;
                    while (x > 0)
                    {
                        byte r = oldPixelsData[offs1 + 2];
                        byte g = oldPixelsData[offs1 + 1];
                        byte b = oldPixelsData[offs1 + 0];

                        int newColorIndex = colorMap[(r >> 2) << 12 | (g >> 2) << 6 | (b >> 2)];

                        pixelsData[offs1 + 2] = palette[newColorIndex].R;
                        pixelsData[offs1 + 1] = palette[newColorIndex].G;
                        pixelsData[offs1 + 0] = palette[newColorIndex].B;
                        pixelsData[offs1 + 3] = oldPixelsData[offs1 + 3];

                        offs1 += 4;
                        x--;
                    }
                }
            }

            if (ditheringType == StiMonochromeDitheringType.FloydSteinberg)
            {
                for (int index = 0; index < pixelsData.Length; index++)
                {
                    pixelsData[index] = 128;
                }

                for (int y = 0; y < bitmapHeight; y++)
                {
                    int offs1 = y * stride;
                    int x = bitmapWidth;
                    while (x > 0)
                    {
                        int rr = oldPixelsData[offs1 + 2] + pixelsData[offs1 + 2] - 128;
                        int gg = oldPixelsData[offs1 + 1] + pixelsData[offs1 + 1] - 128;
                        int bb = oldPixelsData[offs1 + 0] + pixelsData[offs1 + 0] - 128;
                        byte r = (byte)Math.Min(255, Math.Max(0, rr));
                        byte g = (byte)Math.Min(255, Math.Max(0, gg));
                        byte b = (byte)Math.Min(255, Math.Max(0, bb));

                        int newColorIndex = colorMap[(r >> 2) << 12 | (g >> 2) << 6 | (b >> 2)];
                        Color newColor = palette[newColorIndex];

                        pixelsData[offs1 + 2] = newColor.R;
                        pixelsData[offs1 + 1] = newColor.G;
                        pixelsData[offs1 + 0] = newColor.B;
                        pixelsData[offs1 + 3] = oldPixelsData[offs1 + 3];

                        int quant_error = b - newColor.B;
                        pixelsData[offs1 + 4] += (byte)(7f / 16f * quant_error);
                        pixelsData[offs1 + stride - 4] += (byte)(3f / 16f * quant_error);
                        pixelsData[offs1 + stride] += (byte)(5f / 16f * quant_error);
                        pixelsData[offs1 + stride + 4] += (byte)(1f / 16f * quant_error);

                        offs1++;
                        quant_error = g - newColor.G;
                        pixelsData[offs1 + 4] += (byte)(7f / 16f * quant_error);
                        pixelsData[offs1 + stride - 4] += (byte)(3f / 16f * quant_error);
                        pixelsData[offs1 + stride] += (byte)(5f / 16f * quant_error);
                        pixelsData[offs1 + stride + 4] += (byte)(1f / 16f * quant_error);

                        offs1++;
                        quant_error = r - newColor.R;
                        pixelsData[offs1 + 4] += (byte)(7f / 16f * quant_error);
                        pixelsData[offs1 + stride - 4] += (byte)(3f / 16f * quant_error);
                        pixelsData[offs1 + stride] += (byte)(5f / 16f * quant_error);
                        pixelsData[offs1 + stride + 4] += (byte)(1f / 16f * quant_error);

                        offs1 += 2;
                        x--;
                    }
                }
            }

            // Copy the bitmap pixel values into the array.
            Marshal.Copy(pixelsData, 0, bitmapData.Scan0, pixelsDataCount);

            // Unlock the bits.
            dest.UnlockBits(bitmapData);

            usedMemoryCounter += pixelsData.Length;

            return dest;
        }
        #endregion

        #region Convert PixelsData to Indexed byte array
        public static byte[] ConvertToIndexed(byte[] pixelsData, int count, out Color[] palette, StiMonochromeDitheringType ditheringType, ref long usedMemoryCounter)
        {
            //create color map
            var colorMap = new int[64 * 64 * 64];
            int counter = pixelsData.Length / 3;
            int offs = 0;
            while (counter > 0)
            {
                int mapOffset = (pixelsData[offs + 0] >> 2) << 12 | (pixelsData[offs + 1] >> 2) << 6 | (pixelsData[offs + 2] >> 2);
                if (colorMap[mapOffset] < 65535) colorMap[mapOffset]++;
                offs += 3;
                counter--;
            }
            usedMemoryCounter += colorMap.Length * 4;

            palette = GetPaletteV2(colorMap, count);
            PrepareColorMapForFastConversion(colorMap, palette, ditheringType);

            byte[] output = new byte[pixelsData.Length / 3];

            //ditheringType not realized in this method, need additional parameter imageWidth

            counter = pixelsData.Length / 3;
            int offs1 = 0;
            int offs2 = 0;
            while (counter > 0)
            {
                byte r = pixelsData[offs1 + 0];
                byte g = pixelsData[offs1 + 1];
                byte b = pixelsData[offs1 + 2];

                int newColorIndex = colorMap[(r >> 2) << 12 | (g >> 2) << 6 | (b >> 2)];

                output[offs2] = (byte)(newColorIndex & 0xff);

                offs1 += 3;
                offs2++;
                counter--;
            }

            usedMemoryCounter += output.Length;

            return output;
        }
        #endregion

        #endregion
    }
}
