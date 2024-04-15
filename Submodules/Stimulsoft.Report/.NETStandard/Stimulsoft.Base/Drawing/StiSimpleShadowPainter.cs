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

using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Base.Drawing
{
    public static class StiSimpleShadowPainter
    {
        public static void PaintShadow(Graphics g, RectangleD rect, StiSimpleShadow shadow, StiCornerRadius cornerRadius,
            bool isCircle, float scale)
        {
            var pos = GetShadowLocation(rect, shadow, isCircle, scale);

            using (var gaussBitmap = GetShadowBitmap(rect, shadow, cornerRadius, isCircle, scale))
            {
                g.DrawImage(gaussBitmap, pos);
            }
        }

        public static Point GetShadowLocation(RectangleD rect, StiSimpleShadow shadow, bool isCircle, float scale)
        {
            if (isCircle)
                rect = FitRectangleInCircle(rect);

            var size = StiScale.I15 * scale;
            rect.Inflate(size, size);

            return new Point(
                (int)rect.X + StiScale.I(shadow.Location.X),
                (int)rect.Y + StiScale.I(shadow.Location.Y));
        }

        public static Bitmap GetShadowBitmap(RectangleD rect, StiSimpleShadow shadow, StiCornerRadius cornerRadius,
            bool isCircle, float scale)
        {
            if (shadow == null || !shadow.Visible) 
                return null;

            if (isCircle)
                rect = FitRectangleInCircle(rect);

            var shadowRect = rect;
            var size = StiScale.I15 * scale;
            shadowRect.Inflate(size, size);

            using (var shadowBitmap = new Bitmap((int)shadowRect.Width, (int)shadowRect.Height))
            {
                using (var shadowGraphics = Graphics.FromImage(shadowBitmap))
                {
                    shadowGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                    shadowGraphics.Clear(Color.Transparent);

                    using (var shadowBrush = new SolidBrush(shadow.Color))
                    {
                        var geomRect = new Rectangle((int)size, (int)size, (int)rect.Width, (int)rect.Height);

                        if (isCircle)
                        {
                            shadowGraphics.FillEllipse(shadowBrush, geomRect);
                        }
                        else if (cornerRadius == null || cornerRadius.IsEmpty)
                        {
                            shadowGraphics.FillRectangle(shadowBrush, geomRect);
                        }
                        else
                        {
                            using (var path = StiRoundedRectangleCreator.Create(geomRect, cornerRadius, scale))
                            {
                                shadowGraphics.FillPath(shadowBrush, path);
                            }
                        }
                    }
                }

                var gauss = new StiGaussianBlur(shadowBitmap);
                return gauss.Process((int)(StiScale.Factor * (float)shadow.Size / 2 * scale));
            }
        }

        private static RectangleD FitRectangleInCircle(RectangleD rect)
        {
            if (rect.Width > rect.Height)
                return new RectangleD(rect.X + (rect.Width - rect.Height) / 2, rect.Y, rect.Height, rect.Height);
            else
                return new RectangleD(rect.X, rect.Y + (rect.Height - rect.Width) / 2, rect.Width, rect.Width);
        }
    }
}
