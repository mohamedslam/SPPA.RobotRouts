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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Base;
using Stimulsoft.Report.Images;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Graphics = Stimulsoft.Drawing.Graphics;
using Brushes = Stimulsoft.Drawing.Brushes;
using Font = Stimulsoft.Drawing.Font;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiTableCellImageGdiPainter : StiViewGdiPainter
    {
        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var view = (StiView)component;

            if (!view.ExistImageToDraw()) return null;

            double resZoom = view.Page.Zoom;
            view.Report.Info.Zoom = zoom;

            try
            {
                var rect = view.GetPaintRectangle();
                rect.X = 0;
                rect.Y = 0;
                view.Report.Info.Zoom = zoom;

                int imageWidth = (int)rect.Width + 1;
                int imageHeight = (int)rect.Height + 1;

                var bmp = new Bitmap(imageWidth, imageHeight);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.PageUnit = GraphicsUnit.Pixel;
                    if (view.Brush is StiSolidBrush)
                    {
                        Color color = StiBrush.ToColor(view.Brush);
                        if (color == Color.Transparent)
                        {
                            if ((format == StiExportFormat.Pdf) || (format == StiExportFormat.Xps) || (format == StiExportFormat.ImagePng))
                            {
                                color = Color.FromArgb(1, 255, 255, 255);
                            }
                            else
                            {
                                color = Color.White;
                            }
                        }
                        g.Clear(color);
                    }
                    else if (view.Brush is StiEmptyBrush)
                    {
                        g.Clear(Color.White);
                    }
                    else StiDrawing.FillRectangle(g, view.Brush, new RectangleD(0, 0, imageWidth, imageHeight));

                    PaintImage(view, g, new RectangleD(0, 0, imageWidth, imageHeight));
                }
                return bmp;
            }
            catch
            {
                return null;
            }
            finally
            {
                view.Report.Info.Zoom = resZoom;
            }
        }

        private void DrawText(Graphics g, StiTableCellImage image, RectangleD rect)
        {
            string text = string.Empty;
            if (!image.ExistImageToDraw() && image.IsDesigning)
            {
                if (!string.IsNullOrEmpty(image.DataColumn))
                {
                    text = string.Format("{0}: {1}", StiLocalization.Get("PropertyMain", "DataColumn"), image.DataColumn);
                }
                else if (image.ImageURL != null && image.ImageURL.Value != null && image.ImageURL.Value.Length > 0)
                {
                    text = string.Format("{0}: {1}", StiLocalization.Get("PropertyMain", "ImageURL"),
                        StiExpressionPacker.PackExpression(image.ImageURL.Value, image.Report.Designer, true));
                }
                else if (!string.IsNullOrEmpty(image.File))
                {
                    text = string.Format("{0}: {1}", StiLocalization.Get("PropertyMain", "File"), image.File);
                }
                else if (image.ImageData != null && image.ImageData.Value != null && image.ImageData.Value.Length > 0)
                {
                    text = string.Format("{0}: {1}", StiLocalization.Get("PropertyMain", "ImageData"),
                        StiExpressionPacker.PackExpression(image.ImageData.Value, image.Report.Designer, true));
                }

                if (text.Length > 0)
                {
                    using (var font = new Font("Arial", 9 * (float)image.Report.Info.Zoom))
                    {
                        g.DrawString(text, font, Brushes.Black, rect.ToRectangleF());
                    }
                }
            }
        }

        private void DrawCellImage(Graphics g, RectangleD rect)
        {
            if (rect.Width <= StiScale.I9 || rect.Height <= StiScale.I9) return;

            //Don't dispose image - its cached
            var img = StiReportImages.Styles.Image();            
            g.DrawImageUnscaled(img, new Rectangle((int)rect.X, (int)rect.Y, img.Width, img.Height));            
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var cellImage = (StiTableCellImage)component;
            var view = (StiView)component;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!view.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
            {
                if (!cellImage.Enabled)
                {
                    e.Cancel = false;
                    view.InvokePainted(view, e);
                    return;
                }

                view.InvokePainting(view, e);
            }

            if (!e.Cancel && (!(view.Enabled == false && view.IsDesigning == false)))
            {
                var g = e.Graphics;
                var rect = view.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    #region Fill rectangle
                    if (e.DrawBorderFormatting)
                    {
                        if (cellImage.Brush is StiSolidBrush &&
                            ((StiSolidBrush)cellImage.Brush).Color == Color.Transparent &&
                            cellImage.Report.Info.FillComponent &&
                            cellImage.IsDesigning)
                        {
                            Color color = Color.FromArgb(150, Color.White);

                            StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                        }
                        else StiDrawing.FillRectangle(g, cellImage.Brush, rect);

                        if (cellImage.IsSelected)
                        {
                            Color selectColor = Color.FromArgb(80, Color.FromArgb(168, 205, 241));
                            StiDrawing.FillRectangle(g, selectColor, rect);
                        }
                    }
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        if (view.ExistImageToDraw())
                        {
                            PaintImage(view, g, rect);
                        }
                    }

                    #region Markers
                    if (e.DrawBorderFormatting)
                        PaintMarkers(view, g, rect);
                    #endregion

                    #region Border
                    if (view.HighlightState == StiHighlightState.Hide)
                    {
                        PaintBorder(view, g, rect, view.Page.Zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!view.Border.Topmost));
                    }
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        if (cellImage.IsDesigning)
                        {
                            DrawText(g, cellImage, rect);
                            DrawCellImage(g, rect);
                        }

                        if (view.IsDesigning) PaintQuickButtons(view, e.Graphics);

                        PaintEvents(view, e.Graphics, rect);
                        PaintConditions(view, e.Graphics, rect);
                    }
                }
            }
            e.Cancel = false;
            if (e.DrawBorderFormatting)
                view.InvokePainted(view, e);
        }

        public override void PaintSelection(StiComponent component, StiPaintEventArgs e)
        {
            var g = e.Graphics;
            if (component.IsDesigning && component.IsSelected && !component.Report.Info.IsComponentsMoving)
            {
                var rect = component.GetPaintRectangle();

                var size = StiScale.I2;

                using (var brush = new SolidBrush(Color.DimGray))
                {
                    StiDrawing.DrawSelectedRectangle(g, size, brush, rect);
                }
            }
        }
        #endregion
    }
}
