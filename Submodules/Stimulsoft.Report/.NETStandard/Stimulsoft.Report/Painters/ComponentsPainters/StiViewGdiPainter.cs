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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helpers;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#if STIDRAWING
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiViewGdiPainter : StiComponentGdiPainter
    {
        #region Consts
        private const int maxImageMemorySize = 45000000;    //45mb
        internal static readonly string StiGetRawFormatTag = "GetRawFormat";
        #endregion

        #region Methods
        public virtual void PaintImage(StiView view, Graphics g, RectangleD rect, float svgScale = 1)
        {
            if (!view.ExistImageToDraw()) return;

            var gdiImage = view.TakeGdiImageToDraw(svgScale);
            if (gdiImage == null) return;

            if (view.TagValue as string == StiGetRawFormatTag)
                view.TagValue = gdiImage.RawFormat;

            gdiImage = RotateImage(gdiImage, view);
            var isMetafile = gdiImage is Metafile;

            var stiImage = view as StiImage;
            if (stiImage != null)
            {
                bool convert = !stiImage.Report.IsExporting && !stiImage.Report.IsPrinting;
                rect = stiImage.ConvertImageMargins(rect, convert);
            }
                

            var destRect = new RectangleF((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height);
            var srcRect = RectangleF.Empty;
            var clipRect = rect.ToRectangleF();

            bool isStretch = view.Stretch || ((view is StiImage) && ((view as StiImage).Icon != null));

            #region !Stretch
            if (!isStretch)
            {
                double scaleFactor = view.IsExporting ? 1 : StiScale.Factor;
                srcRect = new RectangleF(0, 0,
                    (float) (rect.Width / (view.Report.Info.Zoom *  view.MultipleFactor * scaleFactor)),
                    (float) (rect.Height / (view.Report.Info.Zoom * view.MultipleFactor * scaleFactor)));

                var mult = (float) (view.Page.Zoom * view.MultipleFactor * scaleFactor);

                #region HorAlignment
                switch (view.HorAlignment)
                {
                    case StiHorAlignment.Left:
                        break;

                    case StiHorAlignment.Center:
                        if (destRect.Width > gdiImage.Width * mult)
                        {
                            destRect.X = destRect.X + (destRect.Width - gdiImage.Width * mult) / 2;
                        }
                        else
                        {
                            srcRect.X = (gdiImage.Width - destRect.Width / mult) / 2;
                            srcRect.Width = destRect.Width / mult;
                        }
                        break;

                    case StiHorAlignment.Right:
                        if (destRect.Width > gdiImage.Width * mult)
                        {
                            destRect.X = destRect.Right - gdiImage.Width * mult;
                        }
                        else
                        {
                            srcRect.X = gdiImage.Width - destRect.Width / mult;
                            srcRect.Width = destRect.Width / mult;
                        }
                        break;
                }
                #endregion

                #region VertAlignment
                switch (view.VertAlignment)
                {
                    case StiVertAlignment.Top:
                        break;

                    case StiVertAlignment.Center:
                        if (destRect.Height > gdiImage.Height * mult)
                        {
                            destRect.Y = destRect.Y + (destRect.Height - gdiImage.Height * mult) / 2;
                        }
                        else
                        {
                            srcRect.Y = (gdiImage.Height - destRect.Height / mult) / 2;
                            srcRect.Height = destRect.Height / mult;
                        }
                        break;

                    case StiVertAlignment.Bottom:
                        if (destRect.Height > gdiImage.Height * mult)
                        {
                            destRect.Y = destRect.Bottom - gdiImage.Height * mult;
                        }
                        else
                        {
                            srcRect.Y = gdiImage.Height - destRect.Height / mult;
                            srcRect.Height = destRect.Height / mult;
                        }
                        break;
                }
                #endregion

                if (destRect.Width > gdiImage.Width * mult)
                {
                    destRect.Width = gdiImage.Width * mult;
                    srcRect.Width = gdiImage.Width;
                }
                if (destRect.Height > gdiImage.Height * mult)
                {
                    destRect.Height = gdiImage.Height * mult;
                    srcRect.Height = gdiImage.Height;
                }
            }
            #endregion

            #region Stretch
            else
            {
                srcRect = new RectangleF(0, 0, gdiImage.Width, gdiImage.Height);

                #region AspectRatio
                if (view.AspectRatio)
                {
                    var xRatio = destRect.Width / srcRect.Width;
                    var yRatio = destRect.Height / srcRect.Height;

                    if (xRatio > yRatio) destRect.Width = srcRect.Width * yRatio;
                    else destRect.Height = srcRect.Height * xRatio;

                    #region VertAlignment
                    switch (view.VertAlignment)
                    {
                        case StiVertAlignment.Top:
                            break;

                        case StiVertAlignment.Center:
                            destRect.Y = (float) (destRect.Y + (rect.Height - destRect.Height) / 2);
                            break;

                        case StiVertAlignment.Bottom:
                            destRect.Y = (float) (rect.Bottom - destRect.Height);
                            break;
                    }
                    #endregion

                    #region HorAlignment
                    switch (view.HorAlignment)
                    {
                        case StiHorAlignment.Left:
                            break;

                        case StiHorAlignment.Center:
                            destRect.X = (float) (destRect.X + (rect.Width - destRect.Width) / 2);
                            break;

                        case StiHorAlignment.Right:
                            destRect.X = (float) (rect.Right - destRect.Width);
                            break;
                    }
                    #endregion
                }
                #endregion

            }
            #endregion

            var gs = g.Save();
            g.SetClip(clipRect, CombineMode.Intersect);

            if (!StiOptions.Engine.DisableAntialiasingInPainters)
            {
                if (!view.Smoothing && view.IsPrinting)
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.SmoothingMode = SmoothingMode.None;
                }
                else
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            }

            if (view.IsPrinting && gdiImage is Metafile && StiOptions.Print.MetafileAsBitmap)
            {
                var scale = g.DpiX / 100f;
                var newSize = destRect.Width * scale * (destRect.Height * scale) * 4;
                if (newSize > maxImageMemorySize)
                    scale = scale / (float) Math.Sqrt(newSize / maxImageMemorySize);

                var newWidth = (int) (destRect.Width * scale);
                var newHeight = (int) (destRect.Height * scale);
                if (newWidth > 1 && newHeight > 1)
                {
                    using (var newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb))
                    using (var grNew = Graphics.FromImage(newImage))
                    {
                        grNew.DrawImage(gdiImage, new RectangleF(0, 0, newWidth, newHeight));
                        g.DrawImage(newImage, destRect);
                    }
                }
            }
            else
            {
                if (isMetafile)
                {
                    if (destRect.Width > 0 && destRect.Height > 0)
                        g.DrawImage(gdiImage, destRect);
                }

                else if (!StiOptions.Engine.Image.ConvertSvgToBitmap && StiSvgHelper.IsSvg(view.ImageBytesToDraw))
                    StiSvgHelper.DrawSvg(view.ImageBytesToDraw, rect.ToRectangleF(), view.Stretch, view.AspectRatio, view.Page.Zoom, g);

                else
                    g.DrawImage(gdiImage, Rectangle.Round(destRect), Rectangle.Round(srcRect), GraphicsUnit.Pixel);
            }

            g.Restore(gs);
            gdiImage.Dispose();
        }

        protected Image RotateImage(Image gdiImage, StiView viewComp)
        {
            if (gdiImage == null) return null;

            var imageComp = viewComp as StiImage;
            if (imageComp != null && imageComp.ImageRotation != StiImageRotation.None)
                return StiImageHelper.RotateImage(gdiImage, imageComp.ImageRotation, true);

            return gdiImage;
        }
        #endregion

        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var view = (StiView)component;

            if (!view.ExistImageToDraw() || view.Report == null) return null;

            var resZoom = view.Page.Zoom;
            view.Report.Info.Zoom = zoom;

            try
            {               
                //var rect = view.GetPaintRectangle();  //fix, GetPaintRectangle now apply StiScale.Factor, so use ClientRectangle
                var rect = component.ComponentToPage(component.ClientRectangle).Normalize();
                rect = view.Report.Unit.ConvertToHInches(rect).Multiply(zoom);

                rect.X = 0;
                rect.Y = 0;
                view.Report.Info.Zoom = zoom;

                var imageWidth = (int)rect.Width + 1;
                var imageHeight = (int)rect.Height + 1;

                var bmp = new Bitmap(imageWidth, imageHeight);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.PageUnit = GraphicsUnit.Pixel;
                    if (view.Brush is StiSolidBrush)
                    {
                        var color = StiBrush.ToColor(view.Brush);
                        if (color.A == 0)
                        {
                            if (format == StiExportFormat.Pdf || format == StiExportFormat.Xps || format == StiExportFormat.ImagePng)
                                color = Color.FromArgb(1, 255, 255, 255);

                            else
                            {
                                color = Color.White;
                                if (view.Page != null) color = StiBrush.ToColor(view.Page.Brush);
                                if (color.Equals(Color.Transparent) || color.Equals(Color.Empty)) color = Color.White;
                            }
                        }
                        g.Clear(color);
                    }
                    else if (view.Brush is StiEmptyBrush)
                    {
                        if (format == StiExportFormat.Pdf || format == StiExportFormat.Xps || format == StiExportFormat.ImagePng)
                            g.Clear(Color.FromArgb(1, 255, 255, 255));

                        else
                        {
                            var color = StiBrush.ToColor(view.Page?.Brush);
                            if (color.Equals(Color.Transparent) || color.Equals(Color.Empty)) color = Color.White;
                            g.Clear(Color.White);
                        }
                    }
                    else
                        StiDrawing.FillRectangle(g, view.Brush, new RectangleD(0, 0, imageWidth, imageHeight));

                    PaintImage(view, g, new RectangleD(0, 0, imageWidth, imageHeight), zoom);
                }
                return bmp;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (view.Report != null)
                    view.Report.Info.Zoom = resZoom;
            }
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var view = (StiView)component;

            if (!e.DrawBorderFormatting && e.DrawTopmostBorderSides && !view.Border.Topmost)
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                view.InvokePainting(view, e);

            if (!e.Cancel && !(view.Enabled == false && view.IsDesigning == false))
            {
                var g = e.Graphics;
                var rect = view.GetPaintRectangle();
                
                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    #region Fill rectangle
                    if (e.DrawBorderFormatting)
                    {
                        if (view.Brush is StiSolidBrush &&
                            ((StiSolidBrush) view.Brush).Color == Color.Transparent &&
                            view.Report.Info.FillComponent &&
                            view.IsDesigning && !view.ExistImageToDraw())
                        {
                            var color = Color.FromArgb(50, Color.Green);
                            StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                        }
                        else
                        {
                            StiDrawing.FillRectangle(g, view.Brush, rect);
                        }
                    }
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        if (view.ObjectToDraw != null && (StiOptions.Configuration.IsWPF))
                        {
                            var zoom = g.DpiX / 96;
                            var image = view.GetImage(ref zoom, StiExportFormat.ImagePng);
                            view.PutImageToDraw(image);
                            image.Dispose();
                        }

                        if (view.ExistImageToDraw())
                        {
                            float scale = (float)Math.Min(3d, Math.Max(1d, view.Page.Zoom * StiScale.Factor));
                            PaintImage(view, g, rect, scale);
                        }
                    }

                    #region Markers
                    if (e.DrawBorderFormatting)
                    {
                        PaintMarkers(view, g, rect);
                    }
                    #endregion

                    #region Border
                    if (view.HighlightState == StiHighlightState.Hide)
                        PaintBorder(view, g, rect, view.Page.Zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!view.Border.Topmost));
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        if (view.IsDesigning)
                            PaintQuickButtons(view, e.Graphics);

                        PaintEvents(view, e.Graphics, rect);
                        PaintConditions(view, e.Graphics, rect);
                    }
                }
            }

            e.Cancel = false;
            if (e.DrawBorderFormatting)
                view.InvokePainted(view, e);
        }
        #endregion
    }
}
