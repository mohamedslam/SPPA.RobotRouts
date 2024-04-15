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
using Stimulsoft.Base.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Base.Helpers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using LinearGradientBrush = Stimulsoft.Drawing.Drawing2D.LinearGradientBrush;
#endif

namespace Stimulsoft.Base.Drawing
{
    [JsonObject]
    public sealed class StiShadow
    {
        #region Fields
        private static StiCycledCache cache = new StiCycledCache(100);
        #endregion

        #region Methods
        public static void DrawCachedShadow(Graphics g, RectangleF rect, StiShadowSides sides, StiCornerRadius cornerRadius, bool isSimple)
        {
            if (isSimple)
            {
                using (var path = new GraphicsPath())
                using (var brush = new SolidBrush(Color.FromArgb(50, Color.Black)))
                {
                    if ((sides & StiShadowSides.Top) > 0)
                        path.AddRectangle(new RectangleF(rect.Right, rect.Y + 4, 4, 4));

                    if ((sides & StiShadowSides.Right) > 0)
                        path.AddRectangle(new RectangleF(rect.Right, rect.Y + 8, 4, rect.Height - 8));

                    if ((sides & StiShadowSides.Edge) > 0)
                        path.AddRectangle(new RectangleF(rect.Right, rect.Bottom, 4, 4));

                    if ((sides & StiShadowSides.Bottom) > 0)
                        path.AddRectangle(new RectangleF(rect.X + 8, rect.Bottom, rect.Width - 8, 4));

                    if ((sides & StiShadowSides.Left) > 0)
                        path.AddRectangle(new RectangleF(rect.X + 4, rect.Bottom, 4, 4));

                    g.FillPath(brush, path);
                }
            }
            else
            {
                DrawCachedShadow(g, ConvertRect(rect), cornerRadius, sides);
            }
        }

        private static string GetKey(Rectangle rect, StiCornerRadius cornerRadius)
        {
            var hashCode = 0;

            hashCode = (hashCode * 397) ^ rect.X.GetHashCode();
            hashCode = (hashCode * 397) ^ rect.Y.GetHashCode();
            hashCode = (hashCode * 397) ^ rect.Width.GetHashCode();
            hashCode = (hashCode * 397) ^ rect.Height.GetHashCode();

            if (cornerRadius != null)
                hashCode = (hashCode * 397) ^ cornerRadius.GetUniqueCode();

            return hashCode.ToString();
        }

        public static void DrawCachedShadow(Graphics g, Rectangle rect, StiCornerRadius cornerRadius, StiShadowSides sides)
        {
            var cachedRect = rect;

            var key = GetKey(rect, cornerRadius);

            var bmpCache = cache.Get(key) as Bitmap;

            if (bmpCache != null)
            {
                g.DrawImageUnscaled(bmpCache, rect.X, rect.Y);
            }

            else
            {
                if (rect.Width <= 0) rect.Width = 1;
                if (rect.Height <= 0) rect.Height = 1;

                if (cornerRadius != null && !cornerRadius.IsEmpty)
                {
                    var bmp = new Bitmap(rect.Width + 6, rect.Height + 6, PixelFormat.Format32bppArgb);
                    var gg = Graphics.FromImage(bmp);

                    var shadow = new StiSimpleShadow(Color.FromArgb(150, 0, 0, 0), new Point(2, 2), 2, true);
                    StiSimpleShadowPainter.PaintShadow(gg, new RectangleD(0, 0, rect.Width - 1, rect.Height - 1), shadow, cornerRadius, false, 1);

                    g.DrawImageUnscaled(bmp, rect.X, rect.Y);

                    cache.Add(key, bmp);
                }

                else
                {
                    var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
                    var gg = Graphics.FromImage(bmp);

                    cachedRect.X = -4;
                    cachedRect.Y = -4;

                    if ((sides & StiShadowSides.Top) > 0) DrawTopShadow(gg, cachedRect);
                    if ((sides & StiShadowSides.Right) > 0) DrawRightShadow(gg, cachedRect);
                    if ((sides & StiShadowSides.Edge) > 0) DrawEdgeShadow(gg, cachedRect);
                    if ((sides & StiShadowSides.Bottom) > 0) DrawBottomShadow(gg, cachedRect);
                    if ((sides & StiShadowSides.Left) > 0) DrawLeftShadow(gg, cachedRect);

                    g.DrawImageUnscaled(bmp, rect.X + 4, rect.Y + 4);

                    gg.Dispose();
                    bmp.Dispose();
                }
            }
        }

        public static void DrawEdgeShadow(Graphics g, Rectangle rect)
        {
            lock (edgeShadow)
            {
                g.DrawImageUnscaled(edgeShadow, rect.Right, rect.Bottom, rect.Width * 3, rect.Height * 3);
            }
        }

        public static void DrawLeftShadow(Graphics g, Rectangle rect)
        {
            lock (leftShadow)
            {
                g.DrawImageUnscaled(leftShadow, rect.Left + ShadowSize, rect.Bottom);
            }
        }

        public static void DrawTopShadow(Graphics g, Rectangle rect)
        {
            lock (topShadow)
            {
                g.DrawImageUnscaled(topShadow, rect.Right, rect.Top + ShadowSize);
            }
        }

        public static void DrawRightShadow(Graphics g, Rectangle rect)
        {
            var startColor = Color.FromArgb(ShadowPower, 0, 0, 0);
            var endColor = Color.FromArgb(0, 0, 0, 0);


            var rectSideRight = new Rectangle(
                rect.Right - 1,
                rect.Y + ShadowSize * 2,
                ShadowSize + 1,
                rect.Height - ShadowSize * 2);

            if (rectSideRight.Width == 0 || rectSideRight.Height == 0) return;

            using (var brush = new LinearGradientBrush(rectSideRight, startColor, endColor, 0f))
            {
                g.FillRectangle(brush, rectSideRight);
            }
        }

        public static void DrawBottomShadow(Graphics g, Rectangle rect)
        {
            var startColor = Color.FromArgb(ShadowPower, 0, 0, 0);
            var endColor = Color.FromArgb(0, 0, 0, 0);

            var rectSideBottom = new Rectangle(
                rect.X + ShadowSize * 2,
                rect.Bottom,
                rect.Width - ShadowSize * 2,
                ShadowSize);

            if (rectSideBottom.Width == 0 || rectSideBottom.Height == 0) return;

            using (var brush = new LinearGradientBrush(rectSideBottom, startColor, endColor, 90f))
            {
                g.FillRectangle(brush, rectSideBottom);
            }
        }

        public static void DrawShadow(Graphics g, Rectangle rect)
        {
            DrawEdgeShadow(g, rect);
            DrawLeftShadow(g, rect);
            DrawTopShadow(g, rect);
            DrawRightShadow(g, rect);
            DrawBottomShadow(g, rect);
        }

        private static Rectangle ConvertRect(RectangleF rect)
        {
            return new Rectangle(
                (int)Math.Round(rect.X),
                (int)Math.Round(rect.Y),
                (int)Math.Round(rect.Width),
                (int)Math.Round(rect.Height));
        }

        private static Bitmap GetEdgeShadow()
        {
            var bmp = new Bitmap(ShadowSize, ShadowSize, PixelFormat.Format32bppArgb);
            for (var indexY = 0; indexY < ShadowSize; indexY++)
            {
                var alphay = ShadowPower * (ShadowSize + 1 - indexY) / (ShadowSize + 1);

                for (var indexX = 0; indexX < ShadowSize; indexX++)
                {
                    bmp.SetPixel(indexX, indexY, Color.FromArgb(alphay * (ShadowSize - indexX) / (ShadowSize + 1), 0, 0, 0));
                }
            }

            return bmp;
        }
        #endregion

        #region Consts
        internal const int ShadowSize = 4;
        internal const int ShadowPower = 99;
        #endregion

        #region Fields
        private static Bitmap edgeShadow;
        private static Bitmap leftShadow;
        private static Bitmap topShadow;
        #endregion

        static StiShadow()
        {
            edgeShadow = GetEdgeShadow();
            leftShadow = edgeShadow.Clone() as Bitmap;
            topShadow = edgeShadow.Clone() as Bitmap;

            leftShadow.RotateFlip(RotateFlipType.Rotate180FlipY);
            topShadow.RotateFlip(RotateFlipType.Rotate180FlipX);
        }
    }
}
