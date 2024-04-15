#region Copyright The MIT License (MIT)
/*
{***********************************************************************************}
{																	                }
{   The MIT License (MIT)                                                           }
{                                                                                   }
{   Copyright(c) 2015 Michal Dymel                                                  }
{                                                                                   }
{   Permission is hereby granted, free of charge, to any person obtaining a copy    }
{   of this software and associated documentation files (the "Software"), to deal   }
{   in the Software without restriction, including without limitation the rights    }
{   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell       }
{   copies of the Software, and to permit persons to whom the Software is           }
{   furnished to do so, subject to the following conditions:                        }
{                                                                                   }
{   The above copyright notice and this permission notice shall be included in all  }
{   copies or substantial portions of the Software.                                 }
{                                                                                   }
{   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR      }
{   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,        }
{   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE     }
{   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER          }
{   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,   }
{   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE   }
{   SOFTWARE.                                                                       }
{                                                                                   }
{   https://github.com/mdymel/superfastblur/blob/master/LICENSE                     }	
{                                                                                   }
{***********************************************************************************}
*/
#endregion Copyright The MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Base.Drawing
{
    public class StiGaussianBlur
    {
        #region Fields
        private readonly int[] alpha;
        private readonly int[] red;
        private readonly int[] green;
        private readonly int[] blue;

        private readonly int width;
        private readonly int height;

        private readonly ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 16 };
        #endregion

        #region Methods
        public Bitmap Process(int radial)
        {
            var newAlpha = new int[width * height];
            var newRed = new int[width * height];
            var newGreen = new int[width * height];
            var newBlue = new int[width * height];
            var dest = new int[width * height];

            Parallel.Invoke(
                () => GaussBlur_4(alpha, newAlpha, radial),
                () => GaussBlur_4(red, newRed, radial),
                () => GaussBlur_4(green, newGreen, radial),
                () => GaussBlur_4(blue, newBlue, radial));

            Parallel.For(0, dest.Length, options, i =>
            {
                if (newAlpha[i] > 255)
                    newAlpha[i] = 255;

                if (newRed[i] > 255)
                    newRed[i] = 255;

                if (newGreen[i] > 255)
                    newGreen[i] = 255;

                if (newBlue[i] > 255)
                    newBlue[i] = 255;

                if (newAlpha[i] < 0)
                    newAlpha[i] = 0;

                if (newRed[i] < 0)
                    newRed[i] = 0;

                if (newGreen[i] < 0)
                    newGreen[i] = 0;

                if (newBlue[i] < 0)
                    newBlue[i] = 0;

                dest[i] = (int)((uint)(newAlpha[i] << 24) | (uint)(newRed[i] << 16) | (uint)(newGreen[i] << 8) | (uint)newBlue[i]);
            });

            var image = new Bitmap(width, height);
            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var bits2 = image.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Marshal.Copy(dest, 0, bits2.Scan0, dest.Length);
            image.UnlockBits(bits2);
            return image;
        }

        private void GaussBlur_4(int[] source, int[] dest, int r)
        {
            var bxs = BoxesForGauss(r, 3);

            BoxBlur_4(source, dest, width, height, (bxs[0] - 1) / 2);
            BoxBlur_4(dest, source, width, height, (bxs[1] - 1) / 2);
            BoxBlur_4(source, dest, width, height, (bxs[2] - 1) / 2);
        }

        private int[] BoxesForGauss(int sigma, int n)
        {
            var wIdeal = Math.Sqrt((12 * sigma * sigma / n) + 1);
            var wl = (int)Math.Floor(wIdeal);
            if (wl % 2 == 0) wl--;
            var wu = wl + 2;

            var mIdeal = (double)(12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
            var m = Math.Round(mIdeal);

            var sizes = new List<int>();

            for (var i = 0; i < n; i++)
                sizes.Add(i < m ? wl : wu);

            return sizes.ToArray();
        }

        private void BoxBlur_4(int[] source, int[] dest, int w, int h, int r)
        {
            for (var i = 0; i < source.Length; i++)
                dest[i] = source[i];

            BoxBlurH_4(dest, source, w, h, r);
            BoxBlurT_4(source, dest, w, h, r);
        }

        private void BoxBlurH_4(int[] source, int[] dest, int w, int h, int r)
        {
            var iar = (double)1 / (r + r + 1);
            Parallel.For(0, h, options, i =>
            {
                var ti = i * w;
                var li = ti;
                var ri = ti + r;
                var fv = source[ti];
                var lv = source[ti + w - 1];
                var val = (r + 1) * fv;

                for (var j = 0; j < r; j++)
                    val += source[ti + j];

                for (var j = 0; j <= r; j++)
                {
                    val += source[ri++] - fv;
                    dest[ti++] = (int)Math.Round(val * iar);
                }

                for (var j = r + 1; j < w - r; j++)
                {
                    val += source[ri++] - dest[li++];
                    dest[ti++] = (int)Math.Round(val * iar);
                }

                for (var j = w - r; j < w; j++)
                {
                    val += lv - source[li++];
                    dest[ti++] = (int)Math.Round(val * iar);
                }
            });
        }

        private void BoxBlurT_4(int[] source, int[] dest, int w, int h, int r)
        {
            var iar = (double)1 / (r + r + 1);
            Parallel.For(0, w, options, i =>
            {
                var ti = i;
                var li = ti;
                var ri = ti + r * w;
                var fv = source[ti];
                var lv = source[ti + w * (h - 1)];
                var val = (r + 1) * fv;

                for (var j = 0; j < r; j++)
                    val += source[ti + j * w];

                for (var j = 0; j <= r; j++)
                {
                    val += source[ri] - fv;
                    dest[ti] = (int)Math.Round(val * iar);
                    ri += w;
                    ti += w;
                }

                for (var j = r + 1; j < h - r; j++)
                {
                    val += source[ri] - source[li];
                    dest[ti] = (int)Math.Round(val * iar);
                    li += w;
                    ri += w;
                    ti += w;
                }

                for (var j = h - r; j < h; j++)
                {
                    val += lv - source[li];
                    dest[ti] = (int)Math.Round(val * iar);
                    li += w;
                    ti += w;
                }
            });
        }
        #endregion

        public StiGaussianBlur(Bitmap image)
        {
            var rct = new Rectangle(0, 0, image.Width, image.Height);
            var source = new int[rct.Width * rct.Height];
            var bits = image.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Marshal.Copy(bits.Scan0, source, 0, source.Length);
            image.UnlockBits(bits);

            width = image.Width;
            height = image.Height;

            alpha = new int[width * height];
            red = new int[width * height];
            green = new int[width * height];
            blue = new int[width * height];

            Parallel.For(0, source.Length, options, i =>
            {
                alpha[i] = (int)((source[i] & 0xff000000) >> 24);
                red[i] = (source[i] & 0xff0000) >> 16;
                green[i] = (source[i] & 0x00ff00) >> 8;
                blue[i] = (source[i] & 0x0000ff);
            });
        }
    }
}
