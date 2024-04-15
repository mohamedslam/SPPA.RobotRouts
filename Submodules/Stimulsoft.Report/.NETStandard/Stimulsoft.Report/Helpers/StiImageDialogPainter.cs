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
using System.Linq;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using System.Drawing;
using System.Drawing.Drawing2D;
using Stimulsoft.Base;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
using Brushes = Stimulsoft.Drawing.Brushes;
using Pen = Stimulsoft.Drawing.Pen;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Helpers
{
    public static class StiImageDialogPainter
    {
        #region Methods
        public static void PaintRoundedImagePlace(Control control, Graphics g, 
            byte[] image, bool drawDragDrop = false, bool drawBackground = true)
        {
            using (var gdiImage = StiImageConverter.TryBytesToImage(image, control.Width, control.Height))
            {                
                PaintRoundedImagePlace(control, g, gdiImage, drawDragDrop, drawBackground);
            }        
        }

        public static void PaintRoundedImagePlace(Control control, Graphics g,
            Image image, bool drawDragDrop = false, bool drawBackground = true)
        {
            var isEmpty = image == null;
            var borderColor = isEmpty ? StiUX.ButtonBorder : StiUX.InputBorder;
            var rect = new Rectangle(0, 0, control.Width, control.Height);
            Paint(rect, g, image, drawDragDrop, true, true,
                drawBackground, true, borderColor, isEmpty);
        }

        public static void PaintRoundedPlace(Control control, Graphics g, 
            bool isEmpty, bool drawDragDrop = false, bool drawBackground = true, Color? backColor = null)
        {
            var borderColor = isEmpty ? StiUX.ButtonBorder : StiUX.InputBorder;
            var rect = new Rectangle(0, 0, control.Width, control.Height);
            Paint(rect, g, null, drawDragDrop, true, true, 
                drawBackground, true, borderColor, isEmpty, backColor);
        }

        public static void Paint(Control control, Graphics g, bool drawDragDrop, bool drawBorder, bool isImage, 
            bool drawBackground = true)
        {
            Paint(control, g, new byte[0], drawDragDrop, drawBorder, isImage, drawBackground);
        }

        public static void Paint(Control control, Graphics g, Image image, bool drawDragDrop, bool drawBorder, bool isImage, 
            bool drawBackground = true, bool rounded = false)
        {
            var rect = new Rectangle(0, 0, control.Width, control.Height);

            Paint(rect, g, image, drawDragDrop, drawBorder, isImage, drawBackground, rounded);
        }

        public static void Paint(Control control, Graphics g, byte[] image, bool drawDragDrop, bool drawBorder, bool isImage, 
            bool drawBackground = true, bool rounded = false)
        {
            using (var gdiImage = StiImageConverter.TryBytesToImage(image, control.Width, control.Height))
            {
                Paint(control, g, gdiImage, drawDragDrop, drawBorder, isImage, drawBackground, rounded);
            }
        }

        public static void Paint(Rectangle rect, Graphics g, Image image, bool drawDragDrop, bool drawBorder, bool isImage, 
            bool drawBackground = true, bool rounded = false, Color? borderColor = null, bool dashed = true, Color? backColor = null)
        {
            try
            {
                if (drawBackground)
                    DrawBackground(g, rect, rounded, backColor);

                if (image == null && drawDragDrop)
                    DrawDragDropLabel(g, rect, isImage);

                if (image != null)
                    DrawImage(g, image, rect);

                if (drawBorder)
                    DrawBorder(g, rect, rounded, borderColor, dashed);
            }
            catch
            {
            }
        }

        private static void DrawDragDropLabel(Graphics g, Rectangle rect, bool isImage)
        {
            var str = isImage
                ? StiLocalization.Get("FormDictionaryDesigner", "TextDropImageHere")
                : StiLocalization.Get("FormDictionaryDesigner", "TextDropFileHere");

            rect.Inflate(-StiScale.XXI(16), -StiScale.XXI(16));

            using (var font = new Font("Arial", 8))
            using (var sf = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center})
            {
                g.DrawString(str, font, Brushes.DimGray, rect, sf);
            }
        }

        public static void DrawImage(Graphics g, Image image, Rectangle rect)
        {
            var rectOrigin = rect;
            rect.Inflate(-2, -2);

            float imageWidth;
            float imageHeight;

            if (image.Width < rect.Width && image.Height < rect.Height)
            {
                imageWidth = image.Width;
                imageHeight = image.Height;
            }
            else
            {
                var scaleX = (float) rect.Width / (float) image.Width;
                var scaleY = (float) rect.Height / (float) image.Height;

                if (scaleX < scaleY)
                {
                    imageWidth = image.Width * scaleX;
                    imageHeight = image.Height * scaleX;
                }
                else
                {
                    imageWidth = image.Width * scaleY;
                    imageHeight = image.Height * scaleY;
                }
            }

            g.DrawImage(image,
                rectOrigin.X + (rectOrigin.Width - imageWidth) / 2,
                rectOrigin.Y + (rectOrigin.Height - imageHeight) / 2,
                imageWidth, imageHeight);
        }

        private static void DrawBackground(Graphics g, Rectangle rect, bool rounded = false, Color? backColor = null)
        {
            if (!rounded || StiUX.DefaultInputBoxCornerRadius <= 0)
            {
                g.Clear(backColor.GetValueOrDefault(StiUX.InputBackground));
                return;
            }

            g.Clear(StiUX.InputBackground);

            rect.Inflate(-1, -1);
            var oldSmoothingMode = g.SmoothingMode;

            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var path = StiRoundedRectangleCreator.Create(rect, StiUX.DefaultInputBoxCornerRadius))
                {
                    StiDrawing.FillPath(g, backColor.GetValueOrDefault(StiUX.InputBackground), path);
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothingMode;
            }
        }

        private static void DrawBorder(Graphics g, Rectangle rect, bool rounded = false, 
            Color? borderColor = null, bool dashed = true)
        {
            if (rounded)
            {
                rect.Inflate(-1, -1);

                var oldSmoothingMode = g.SmoothingMode;                

                try
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    using (var pen = new Pen(borderColor.GetValueOrDefault(StiUX.ButtonBorder)))
                    using (var path = StiRoundedRectangleCreator.Create(rect, StiUX.DefaultInputBoxCornerRadius))
                    {
                        if (dashed)
                            pen.DashStyle = DashStyle.Dash;

                        g.DrawPath(pen, path);
                    }
                }
                finally
                {
                    g.SmoothingMode = oldSmoothingMode;
                }
            }
            else
            { 
                StiDrawing.DrawRectangle(g, StiUX.ButtonBorder, 0, 0, rect.Width - 1, rect.Height - 1);
            }
        }
        #endregion
    }
}
