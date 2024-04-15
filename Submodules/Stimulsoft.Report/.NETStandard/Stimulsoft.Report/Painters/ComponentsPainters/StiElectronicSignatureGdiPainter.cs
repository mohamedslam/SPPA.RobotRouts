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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.SignatureFonts;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using StiSvgHelper = Stimulsoft.Base.Helpers.StiSvgHelper;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Brushes = Stimulsoft.Drawing.Brushes;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
using Pen = Stimulsoft.Drawing.Pen;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public partial class StiElectronicSignatureGdiPainter : 
        StiComponentGdiPainter
    {
        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var signature = component as StiElectronicSignature;

            double resZoom = signature.Report.Info.Zoom;
            zoom *= 2;
            signature.Report.Info.Zoom = zoom;
            //StiScale.Lock();

            var rect = signature.GetPaintRectangle();

            rect.X = 0;
            rect.Y = 0;

            int imageWidth = (int)rect.Width;
            int imageHeight = (int)rect.Height;

            if (((signature.Brush is StiSolidBrush) && ((signature.Brush as StiSolidBrush).Color.A < 16)) || (signature.Brush is StiEmptyBrush))
                StiExportUtils.DisableFontSmoothing(component.Report);

            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;

                if (((format == StiExportFormat.Pdf) || (format == StiExportFormat.ImagePng)) && (signature.Brush != null) &&
                    (((signature.Brush is StiSolidBrush) && ((signature.Brush as StiSolidBrush).Color.A == 0)) || (signature.Brush is StiEmptyBrush)))
                {
                    g.Clear(Color.FromArgb(1, 255, 255, 255));
                }
                else
                {
                    if (((format == StiExportFormat.Excel) || (format == StiExportFormat.Excel2007) || (format == StiExportFormat.Word2007) || (format == StiExportFormat.Rtf) || (format == StiExportFormat.RtfFrame) || (format == StiExportFormat.RtfTable) || (format == StiExportFormat.RtfWinWord)) &&
                        (signature.Brush != null) && (((signature.Brush is StiSolidBrush) && ((signature.Brush as StiSolidBrush).Color.A < 16)) || (signature.Brush is StiEmptyBrush)))
                    {
                        g.Clear(Color.White);
                    }
                    else
                    {
                        g.Clear(StiBrush.ToColor(signature.Brush));
                    }
                }

                rect.X = 0;
                rect.Y = 0;

                DrawSignature(signature, g, rect, true, true);
            }

            signature.Report.Info.Zoom = resZoom;
            //StiScale.Unlock();

            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var signature = component as StiElectronicSignature;
            signature.InvokePainting(signature, e);

            if (!e.Cancel && (!(signature.Enabled == false && signature.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = signature.GetPaintRectangle();
                if (rect.Width > 0 && rect.Height > 0 &&
                    (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    PaintSignature(signature, g, rect, e.DrawTopmostBorderSides, e.DrawBorderFormatting);

                    if (e.DrawBorderFormatting)
                    {
                        if (signature.IsDesigning)
                            PaintQuickButtons(signature, e.Graphics);

                        PaintEvents(signature, e.Graphics, rect);
                        PaintConditions(signature, e.Graphics, rect);
                    }

                    #region Markers
                    if (signature.HighlightState == StiHighlightState.Hide && signature.Border.Side != StiBorderSides.All) 
                        PaintMarkers(signature, g, rect);
                    #endregion
                }
            }
            e.Cancel = false;
            signature.InvokePainted(signature, e);
        }

        private void PaintSignature(StiElectronicSignature signature, Graphics g, RectangleD rect, bool drawBorder, bool drawFormatting)
        {
            if (signature.IsPrinting)
            {
                float zoom = 1;
                using (var image = GetImage(signature, ref zoom, StiExportFormat.None))
                {
                    g.DrawImage(image, rect.ToRectangleF());
                }
            }
            else
            {
                var progressStatus = StiComponentProgressHelper.Contains(signature);
                if (progressStatus != StiProgressStatus.None && signature.IsDesigning)
                {
                    StiDrawing.FillRectangle(g, Color.FromArgb(0x99, Color.White), rect);
                    PaintProgress(g, rect, progressStatus);
                    return;
                }

                DrawSignature(signature, g, rect, drawBorder, drawFormatting);
            }

            #region Draw Border
            if ((drawBorder && signature.Border.Topmost) || (drawFormatting && (!signature.Border.Topmost)))
            {
                if (signature.Border.Side == StiBorderSides.None && signature.IsDesigning)
                {
                    using (var pen = new Pen(Color.Gray))
                    {
                        pen.DashStyle = DashStyle.Dash;
                        StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                    }
                }
                if (signature.HighlightState == StiHighlightState.Hide)
                {
                    PaintBorder(signature, g, rect, true, true);
                }
            }
            #endregion
        }

        private void DrawSignature(StiElectronicSignature signature, Graphics g, RectangleD rect, bool drawBorder, bool drawFormatting)
        {
            #region Draw Fill
            if (drawFormatting)
            {
                using (var brush = (signature.Brush is StiSolidBrush && ((StiSolidBrush)signature.Brush).Color == Color.Transparent && signature.IsDesigning)
                    ? new SolidBrush(Color.FromArgb(50, 84, 175, 220))
                    : StiBrush.GetBrush(signature.Brush, rect))
                {
                    g.FillRectangle(brush, rect.ToRectangleF());
                }
            }
            #endregion

            g.SetClip(rect.ToRectangle());

            #region Draw Content
            if (drawFormatting)
            {
                switch (signature.Mode)
                {
                    case StiSignatureMode.Type:
                        {
                            var font = new Font(StiSignatureFontsHelper.GetFont(signature.Type.Style), (float)(16 * signature.Page.Zoom));
                            var defaultHint = g.TextRenderingHint;
                            g.TextRenderingHint = TextRenderingHint.AntiAlias;

                            #region Draw Text
                            var text1 = signature.Type.FullName;
                            var text2 = signature.Type.Initials;

                            int x = (int)rect.X + StiScale.XXI(12);

                            if (!string.IsNullOrEmpty(text1))
                            {
                                var size = g.MeasureString(text1, font);

                                using (var brush = new SolidBrush(StiUX.Foreground))
                                    g.DrawString(text1, font, brush, new RectangleF(x, (int)rect.Y + ((int)rect.Height - size.Height) / 2, size.Width, size.Height));

                                x += (int)size.Width + StiScale.XXI(12);
                            }

                            if (!string.IsNullOrEmpty(text2))
                            {
                                var size = g.MeasureString(text2, font);
                                size.Width += 10;

                                using (var brush = new SolidBrush(StiUX.Foreground))
                                    g.DrawString(text2, font, brush, new RectangleF(x, (int)rect.Y + ((int)rect.Height - size.Height) / 2, size.Width, size.Height));
                            }
                            #endregion

                            g.TextRenderingHint = defaultHint;
                        }
                        break;

                    case StiSignatureMode.Draw:
                        {
                            if (signature.Image.Image != null)
                            {
                                var imageBytes = signature.Image.Image;

                                if (StiSvgHelper.IsSvg(imageBytes))
                                {
                                    var svgDoc = StiSvgHelper.OpenSvg(imageBytes);
                                    var svgSize = StiSvgHelper.GetSvgSize(svgDoc);

                                    imageBytes = StiImageConverter.ImageToBytes(StiSvgHelper.ConvertSvgToImage(imageBytes, Convert.ToInt32(svgSize.Width), Convert.ToInt32(svgSize.Height), signature.Image.Stretch, signature.Image.AspectRatio));
                                }

                                using (var image = Bitmap.FromStream(new MemoryStream(imageBytes)))
                                {
                                    if (signature.Image.Stretch && !signature.Image.AspectRatio)
                                    {
                                        g.DrawImage(image, rect.ToRectangleF());
                                    }
                                    else
                                    {
                                        double x = rect.X;
                                        double y = rect.Y;

                                        var zoom = signature.Page.Zoom * StiScale.Factor;
                                        var imageSize = new Size((int)(image.Width * zoom), (int)(image.Height * zoom));

                                        if (signature.Image.Stretch && signature.Image.AspectRatio)
                                        {
                                            double xRatio = rect.Width / image.Width;
                                            double yRatio = rect.Height / image.Height;
                                            double ratio = Math.Min(xRatio, yRatio);
                                            imageSize = new Size((int)(image.Width * ratio), (int)(image.Height * ratio));
                                        }

                                        switch (signature.Image.HorAlignment)
                                        {
                                            case StiHorAlignment.Left:
                                                x = rect.X;
                                                break;

                                            case StiHorAlignment.Center:
                                                x = rect.X + (rect.Width - imageSize.Width) / 2;
                                                break;

                                            case StiHorAlignment.Right:
                                                x = rect.Right - imageSize.Width;
                                                break;
                                        }

                                        switch (signature.Image.VertAlignment)
                                        {
                                            case StiVertAlignment.Top:
                                                y = rect.Y;
                                                break;

                                            case StiVertAlignment.Center:
                                                y = rect.Y + (rect.Height - imageSize.Height) / 2;
                                                break;

                                            case StiVertAlignment.Bottom:
                                                y = rect.Bottom - imageSize.Height;
                                                break;
                                        }

                                        g.DrawImage(image, new RectangleF((float)x, (float)y, imageSize.Width, imageSize.Height));
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(signature.Text.Text))
                            {
                                using (var foreBrush = new SolidBrush(signature.Text.Color))
                                using (var sf = new StringFormat())
                                {
                                    switch (signature.Text.HorAlignment)
                                    {
                                        case StiTextHorAlignment.Left:
                                            sf.Alignment = StringAlignment.Near;
                                            break;

                                        case StiTextHorAlignment.Center:
                                            sf.Alignment = StringAlignment.Center;
                                            break;

                                        case StiTextHorAlignment.Right:
                                            sf.Alignment = StringAlignment.Far;
                                            break;

                                        case StiTextHorAlignment.Width:
                                            sf.Alignment = StringAlignment.Near;
                                            break;
                                    }

                                    switch (signature.Text.VertAlignment)
                                    {
                                        case StiVertAlignment.Top:
                                            sf.LineAlignment = StringAlignment.Near;
                                            break;

                                        case StiVertAlignment.Center:
                                            sf.LineAlignment = StringAlignment.Center;
                                            break;

                                        case StiVertAlignment.Bottom:
                                            sf.LineAlignment = StringAlignment.Far;
                                            break;
                                    }

                                    var font = StiFontUtils.ChangeFontSize(signature.Text.Font, (float)(signature.Text.Font.Size * signature.Page.Zoom));
                                    var defaultHint = g.TextRenderingHint;
                                    g.TextRenderingHint = TextRenderingHint.AntiAlias;

                                    g.DrawString(signature.Text.Text, font, foreBrush, rect.ToRectangleF(), sf);

                                    g.TextRenderingHint = defaultHint;
                                }
                            }

                            if (signature.Draw.Image != null)
                            {
                                using (var bitmap = Bitmap.FromStream(new MemoryStream(signature.Draw.Image)))
                                {
                                    if (signature.Draw.Stretch && !signature.Draw.AspectRatio)
                                    {
                                        g.DrawImage(bitmap, rect.ToRectangleF());
                                    }
                                    else
                                    {
                                        double x = rect.X;
                                        double y = rect.Y;

                                        var zoom = signature.Page.Zoom * StiScale.Factor;
                                        var imageSize = new Size((int)(bitmap.Width * zoom), (int)(bitmap.Height * zoom));
                                        if (signature.Draw.Stretch && signature.Draw.AspectRatio)
                                        {
                                            double xRatio = rect.Width / bitmap.Width;
                                            double yRatio = rect.Height / bitmap.Height;
                                            double ratio = Math.Min(xRatio, yRatio);
                                            imageSize = new Size((int)(bitmap.Width * ratio), (int)(bitmap.Height * ratio));
                                        }

                                        switch (signature.Draw.HorAlignment)
                                        {
                                            case StiHorAlignment.Left:
                                                x = rect.X;
                                                break;

                                            case StiHorAlignment.Center:
                                                x = rect.X + (rect.Width - imageSize.Width) / 2;
                                                break;

                                            case StiHorAlignment.Right:
                                                x = rect.Right - imageSize.Width;
                                                break;
                                        }

                                        switch (signature.Draw.VertAlignment)
                                        {
                                            case StiVertAlignment.Top:
                                                y = rect.Y;
                                                break;

                                            case StiVertAlignment.Center:
                                                y = rect.Y + (rect.Height - imageSize.Height) / 2;
                                                break;

                                            case StiVertAlignment.Bottom:
                                                y = rect.Bottom - imageSize.Height;
                                                break;
                                        }

                                        g.DrawImage(bitmap, new RectangleF((float)x, (float)y, imageSize.Width, imageSize.Height));
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            #endregion

            g.ResetClip();
        }
        #endregion
    }
}