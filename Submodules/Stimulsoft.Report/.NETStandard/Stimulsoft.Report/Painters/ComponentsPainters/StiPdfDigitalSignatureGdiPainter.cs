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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Export;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

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
    public partial class StiPdfDigitalSignatureGdiPainter : 
        StiComponentGdiPainter
    {
        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var signature = component as StiPdfDigitalSignature;

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

                DrawSignature(signature, g, rect, true);
            }

            signature.Report.Info.Zoom = resZoom;
            //StiScale.Unlock();

            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var signature = component as StiPdfDigitalSignature;
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

        private void PaintSignature(StiPdfDigitalSignature signature, Graphics g, RectangleD rect, bool drawBorder, bool drawFormatting)
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
                DrawSignature(signature, g, rect, drawFormatting);
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

        private void DrawSignature(StiPdfDigitalSignature signature, Graphics g, RectangleD rect, bool drawFormatting)
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

            #region Draw Content
            if (drawFormatting)
            {
                var digitalRect = rect;

                using (var font = new Font("Segoe UI", 10f * (float)signature.Report.Info.Zoom))
                using (var sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    var text = signature.Placeholder;
                    if (string.IsNullOrEmpty(text))
                        text = StiLocalization.Get("Signature", "DigitalSignatureEmptyWatermark");

                    g.DrawString(text, font, Brushes.Red, digitalRect.ToRectangleF(), sf);
                }
            }
            #endregion
        }
        #endregion
    }
}