#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Helpers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

#if STIDRAWING
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// This class helps in a drawing a watermark.
    /// </summary>
    public static class StiAdvancedWatermarkGdiPainter
    {
        #region Methods
        /// <summary>
        /// Paints a watermark on specified context with specified arguments.
        /// </summary>
        public static void PaintWatermark(Graphics g, StiAdvancedWatermark watermark, StiCornerRadius cornerRadius, RectangleD rect, double scale)
        {
            var state = g.Save();
            GraphicsPath clipPath = null;

            try
            {
                if (cornerRadius == null || cornerRadius.IsEmpty)
                {
                    g.SetClip(rect.ToRectangleF());
                }
                else
                {
                    clipPath = StiRoundedRectangleCreator.Create(rect.ToRectangleF(), cornerRadius, (float)scale);
                    g.SetClip(clipPath);                    
                }

                PaintWatermarkImage(g, watermark, rect, scale);
                PaintWatermarkText(g, watermark, rect, scale);
                PaintWatermarkWeave(g, watermark, rect, scale);
            }
            finally
            {
                clipPath?.Dispose();
                g.Restore(state);
            }
        }

        private static void PaintWatermarkText(Graphics g, StiAdvancedWatermark watermark, RectangleD rect, double scale)
        {
            if (string.IsNullOrEmpty(watermark.Text) || !watermark.TextEnabled) return;

            var fontSize = (float)(watermark.TextFont.Size * scale / StiScale.Factor);  //fix, StiScale.Factor is already in scale

            using (var brush = new SolidBrush(watermark.TextColor))
            using (var font = new Font(watermark.TextFont.FontFamily, fontSize,
                watermark.TextFont.Style, watermark.TextFont.Unit,
                watermark.TextFont.GdiCharSet, watermark.TextFont.GdiVerticalFont))
            {
                var defaultHint = g.TextRenderingHint;

                try
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;

                    var textOptions = new StiTextOptions
                    {
                        Angle = watermark.TextAngle
                    };

                    StiTextDrawing.DrawString(g, watermark.Text, font, brush, rect,
                        textOptions, StiTextHorAlignment.Center, StiVertAlignment.Center, true, watermark.TextAngle);
                }
                finally
                {
                    g.TextRenderingHint = defaultHint;
                }
            }
        }

        private static void PaintWatermarkImage(Graphics g, StiAdvancedWatermark watermark, RectangleD rect, double scale)
        {
            if (!watermark.ImageEnabled) return;

            var imageBuffer = GetImage(watermark);
            if (imageBuffer == null) return;


            if (watermark.ImageStretch)
            {
                var destRect = rect.ToRectangleF();

                var gdiImage = StiAdvancedWatermarkImageUtils.GetGdiImage(imageBuffer, (int)destRect.Width, (int)destRect.Height);
                #region ImageAspectRatio
                if (watermark.ImageAspectRatio)
                {
                    var xRatio = destRect.Width / gdiImage.Width;
                    var yRatio = destRect.Height / gdiImage.Height;

                    if (xRatio > yRatio)
                    {
                        destRect.X = (destRect.Width - gdiImage.Width * yRatio) / 2;
                        destRect.Width = gdiImage.Width * yRatio;
                    }
                    else
                    {
                        destRect.Y = (destRect.Height - gdiImage.Height * xRatio) / 2;
                        destRect.Height = gdiImage.Height * xRatio;
                    }

                    var minRatio = Math.Max(xRatio, yRatio);
                    var imageSize = new SizeD(gdiImage.Size.Width * minRatio, gdiImage.Height * minRatio);
                    destRect = StiRectangleUtils.AlignSizeInRect(rect, imageSize, watermark.ImageAlignment).ToRectangleF();
                }
                #endregion

                StiAdvancedWatermarkImageUtils.DrawImage(g, gdiImage, Rectangle.Round(destRect));
            }
            else

            {
                var gdiImage = StiAdvancedWatermarkImageUtils.GetGdiImage(imageBuffer);
                scale *= watermark.ImageMultipleFactor;
                var imageSize = new SizeD(gdiImage.Size.Width * scale, gdiImage.Height * scale);
                if (watermark.ImageTiling)
                {
                    PaintWatermarkTileImage(gdiImage, g, rect, imageSize);
                }
                else
                {
                    var imageRect = StiRectangleUtils.AlignSizeInRect(rect, imageSize, watermark.ImageAlignment);
                    StiAdvancedWatermarkImageUtils.DrawImage(g, gdiImage, imageRect.ToRectangle());
                }
            }
        }

        private static void PaintWatermarkWeave(Graphics g, StiAdvancedWatermark watermark, RectangleD rect, double scale)
        {
            if (!watermark.WeaveEnabled || (watermark.WeaveMajorIcon == null && watermark.WeaveMinorIcon == null)) return;

            var majorSize = (int)Math.Ceiling(5 * (float)watermark.WeaveMajorSize * scale);
            var minorSize = (int)Math.Ceiling(5 * (float)watermark.WeaveMinorSize * scale);

            var dist = (int)Math.Ceiling(watermark.WeaveDistance * scale);

            var imageMajor = watermark.WeaveMajorIcon != null
                ? StiFontIconsHelper.ConvertFontIconToImage(watermark.WeaveMajorIcon.Value, watermark.WeaveMajorColor, majorSize, majorSize)
                : null;

            var imageMinor = watermark.WeaveMinorIcon != null
                ? StiFontIconsHelper.ConvertFontIconToImage(watermark.WeaveMinorIcon.Value, watermark.WeaveMinorColor, minorSize, minorSize)
                : null;

            var centralX = rect.X + rect.Width / 2;
            var centralY = rect.Y + rect.Height / 2;

            var posX = centralX;
            var posY = centralY;

            for (var step = 0; step < 30; step++)
            {
                var forwardRad = (watermark.WeaveAngle + 90) * (Math.PI / 180);
                var x = posX + (float)(dist * step * Math.Cos(forwardRad));
                var y = posY + (float)(dist * step * Math.Sin(forwardRad));

                if (!DrawWeaveLine(g, watermark, rect.ToRectangleF(), dist, imageMajor, imageMinor, (float)x, (float)y, step)) break;
            }

            posX = centralX;
            posY = centralY;

            for (var step = 1; step < 30; step++)
            {
                var backwardRad = (watermark.WeaveAngle - 90) * (Math.PI / 180);
                var x = posX + (float)(dist * step * Math.Cos(backwardRad));
                var y = posY + (float)(dist * step * Math.Sin(backwardRad));

                if (!DrawWeaveLine(g, watermark, rect.ToRectangleF(), dist, imageMajor, imageMinor, (float)x, (float)y, -step)) break;
            }

            imageMajor?.Dispose();
            imageMinor?.Dispose();
        }
                
        private static bool DrawWeaveLine(Graphics g, StiAdvancedWatermark watermark, RectangleF rect, 
            int dist, Image imageMajor, Image imageMinor, float posX, float posY, int shift)
        {
            var isAny = false;
            var isOnce = false;
            var rad = watermark.WeaveAngle * (Math.PI / 180);

            for (var step = 0; step < 30; step++)
            {
                var x = posX + (float)(dist * step * Math.Cos(rad));
                var y = posY + (float)(dist * step * Math.Sin(rad));
                var image = ((step + shift) & 1) == 0 ? imageMajor : imageMinor;

                if (image == null)continue;
                
                if (ContainsWeaveImage(g, watermark, image, rect, x, y))
                {
                    DrawWeaveImage(g, watermark, image, x, y);

                    isAny = true;
                    isOnce = true;
                }
                else
                {
                    if (isOnce) break;
                }
            }

            for (var step = 1; step < 30; step++)
            {
                var x = posX - (float)(dist * step * Math.Cos(rad));
                var y = posY - (float)(dist * step * Math.Sin(rad));
                var image = ((-step + shift) & 1) == 0 ? imageMajor : imageMinor;

                if (image == null)continue;
                
                if (ContainsWeaveImage(g, watermark, image, rect, x, y))
                {
                    DrawWeaveImage(g, watermark, image, x, y);

                    isAny = true;
                    isOnce = true;
                }
                else
                {
                    if (isOnce) break;
                }
            }

            return isAny;
        }

        private static bool ContainsWeaveImage(Graphics g, StiAdvancedWatermark watermark, Image image, RectangleF rect, float x, float y)
        {
            if (image == null)
                return false;

            var width = image.Width / 2;
            var height = image.Height / 2;
            var rad = watermark.WeaveAngle * (Math.PI / 180);

            var p1 = new PointF(
                (-width) * (float)Math.Cos(rad) + (-height) * (float)Math.Sin(rad) + x,
                (-width) * (float)Math.Sin(rad) - (-height) * (float)Math.Cos(rad) + y);

            var p2 = new PointF(
                width * (float)Math.Cos(rad) + (-height) * (float)Math.Sin(rad) + x,
                width * (float)Math.Sin(rad) - (-height) * (float)Math.Cos(rad) + y);

            var p3 = new PointF(
                (-width) * (float)Math.Cos(rad) + height * (float)Math.Sin(rad) + x,
                (-width) * (float)Math.Sin(rad) - height * (float)Math.Cos(rad) + y);

            var p4 = new PointF(
                width * (float)Math.Cos(rad) + height * (float)Math.Sin(rad) + x,
                width * (float)Math.Sin(rad) - height * (float)Math.Cos(rad) + y);

            //Red line
            //g.FillRectangle(Brushes.Red, p1.X + x, p1.Y + y, 10, 10);
            //g.FillRectangle(Brushes.Blue, p2.X + x, p2.Y + y, 10, 10);
            //g.FillRectangle(Brushes.Green, p3.X + x, p3.Y + y, 10, 10);
            //g.FillRectangle(Brushes.Yellow, p4.X + x, p4.Y + y, 10, 10);

            return rect.Contains(p1) || rect.Contains(p2) || rect.Contains(p3) || rect.Contains(p4);
        }

        private static void DrawWeaveImage(Graphics g, StiAdvancedWatermark watermark, Image image, float posX, float posY)
        {
            var state = g.Save();

            try
            {
                g.TranslateTransform(posX, posY);
                g.RotateTransform(watermark.WeaveAngle);

                posX = -image.Width / 2;
                posY = -image.Height / 2;

                var rect = new RectangleF(posX, posY, image.Width, image.Height);
                g.DrawImage(image, rect);

                //Red line
                //g.DrawRectangle(Pens.Red, rect.X, rect.Y, rect.Width, rect.Height);
            }
            finally
            {
                g.Restore(state);
            }
        }

        public static void PaintWatermarkTileImage(Image image, Graphics g, RectangleD rect, SizeD imageSize)
        {
            var y = (float)rect.Y;

            while (y < rect.Bottom)
            {
                var x = (float)rect.X;
                while (x < rect.Right)
                {
                    g.DrawImage(image, new RectangleF(x, y, (float)imageSize.Width, (float)imageSize.Height));
                    x += (float)imageSize.Width;
                }
                y += (float)imageSize.Height;
            }
        }

        public static byte[] GetImage(StiAdvancedWatermark watermark)
        {
            var image = watermark.TakeImage();

            if (watermark.ImageTransparency == 0)
                return image;

            if (image == null)
                return null;

            if (watermark.GetCachedImage() == null)
            {
                var gdiImage = StiImageConverter.BytesToImage(image);
                var gdiTransparentedImage = StiImageTransparenceHelper
                    .GetTransparentedImage(gdiImage, 1f - watermark.ImageTransparency / 255f);

                watermark.PutCachedImage(StiImageConverter.ImageToBytes(gdiTransparentedImage));
            }

            return watermark.GetCachedImage();
        }
        #endregion
    }
}