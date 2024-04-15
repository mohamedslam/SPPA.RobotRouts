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

using Stimulsoft.Base.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
using ImageAttributes = Stimulsoft.Drawing.Imaging.ImageAttributes;
#endif

namespace Stimulsoft.Base.Context
{
    public class StiTableImageGeom : StiTableGeom
    {
        #region Properties
        public Image Image { get; set; }

        public RectangleF Rect { get; set; }
        #endregion

        #region Properties.Override
        public override StiTableGeomType Type => StiTableGeomType.Image;
        #endregion

        #region Methods
        public static void CheckImage(ref Image image, ref RectangleF rect)
        {
            rect = GetRect(image, rect);
            image = ResizeImage(image, (int)rect.Width, (int)rect.Height);
        }

        private static RectangleF GetRect(Image image, RectangleF rect)
        {
            var resualtRect = new RectangleF();

            var zoomVert = image.Width / rect.Width;

            var tempHeight = image.Height / zoomVert;

            if (tempHeight <= rect.Height)
            {
                resualtRect.Width = rect.Width;
                resualtRect.Height = tempHeight;

                resualtRect.X = rect.X + (rect.Width - resualtRect.Width) / 2;
                resualtRect.Y = rect.Y;
            }
            else
            {
                var zoomHor = image.Height / rect.Height;

                resualtRect.Width = image.Width / zoomHor;
                resualtRect.Height = rect.Height;

                resualtRect.X = rect.X + (rect.Width - resualtRect.Width) / 2;
                resualtRect.Y = rect.Y;
            }

            return resualtRect;
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        #endregion

        public StiTableImageGeom(Image image, RectangleF rect)
        {
            this.Rect = rect;
            this.Image = image;
        }
    }
}
