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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Base;
using Stimulsoft.Report.Images;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Brushes = Stimulsoft.Drawing.Brushes;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Font = Stimulsoft.Drawing.Font;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiTableCellRichTextGdiPainter : StiComponentGdiPainter
    {
        #region Methods
        private int RoundValue(float value)
        {
            int newValue = (int)value;

            if (newValue < value) newValue++;
            return newValue;
        }
        #endregion

        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var richText = (StiRichText)component;

            if (format != StiExportFormat.HtmlSpan &&
                format != StiExportFormat.HtmlDiv &&
                format != StiExportFormat.HtmlTable) zoom *= 2;
            if (richText.Image == null) richText.RenderMetafile();

            double wd = richText.Report.Unit.ConvertToHInches(richText.Width);
            double ht = richText.Report.Unit.ConvertToHInches(richText.Height);
            wd = Math.Max(wd, 1);
            ht = Math.Max(ht, 1);

            var rectRich = new RectangleD(0, 0, wd, ht);
            var rectRichF = richText.ConvertTextMargins(rectRich, false).ToRectangleF();
            rectRichF.Width = Math.Max(rectRichF.Width, 1);
            rectRichF.Height = Math.Max(rectRichF.Height, 1);

            var bmp = new Bitmap(RoundValue((float)wd * zoom), RoundValue((float)ht * zoom));
            using (var imageGraph = Graphics.FromImage(bmp))
            {
                Color backColor = richText.BackColor;
                if (backColor == Color.Transparent) backColor = Color.White;
                imageGraph.Clear(backColor);

                if (StiOptions.Engine.FullTrust && richText.Image != null)
                {
                    imageGraph.DrawImage(richText.Image,
                    new Rectangle(
                        RoundValue(rectRichF.X * zoom),
                        RoundValue(rectRichF.Y * zoom),
                        RoundValue(rectRichF.Width * zoom),
                        RoundValue(rectRichF.Height * zoom)),
                    new Rectangle(
                        RoundValue((float)0),
                        RoundValue((float)0),
                        RoundValue((float)rectRichF.Width + 2),
                        RoundValue((float)rectRichF.Height + 2)), GraphicsUnit.Pixel);
                }
            }
            return bmp;
        }

        public virtual void PaintBackground(StiTableCellRichText text, Graphics g, RectangleD rect)
        {
            if (text.BackColor == Color.Transparent &&
                text.Report.Info.FillComponent &&
                text.IsDesigning)
            {
                Color color = Color.FromArgb(150, Color.White);
                StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
            }
            else StiDrawing.FillRectangle(g, text.BackColor, rect);

            if (text.IsSelected)
            {
                Color selectColor = Color.FromArgb(80, Color.FromArgb(168, 205, 241));
                StiDrawing.FillRectangle(g, selectColor, rect);
            }
        }

        private void DrawCellImage(Graphics g, RectangleD rect)
        {
            if (rect.Width <= StiScale.I9 || rect.Height <= StiScale.I9) return;

            //Don't dispose image - its cached
            var img = StiReportImages.Styles.RichText();            
            g.DrawImageUnscaled(img, new Rectangle((int)rect.X, (int)rect.Y, img.Width, img.Height));            
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var richText = (StiTableCellRichText)component;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!richText.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                richText.InvokePainting(richText, e);

            if (!component.Enabled)
            {
                e.Cancel = false;
                richText.InvokePainted(richText, e);
                return;
            }

            if (!e.Cancel && (!(richText.Enabled == false && richText.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = richText.GetPaintRectangle();
                var rectRich = richText.ConvertTextMargins(rect, true);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    #region Fill rectangle
                    if (e.DrawBorderFormatting)
                        PaintBackground(richText, g, rect);
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        if (!string.IsNullOrEmpty(richText.DataColumn) && richText.IsDesigning)
                        {
                            using (var font = new Font("Arial", (float)(9 * richText.Page.Zoom)))
                            {
                                g.DrawString("Rtf from {" + richText.DataColumn + "}", font, Brushes.Black, rect.ToRectangleF());
                            }
                        }
                        else
                        {
                            richText.RenderMetafile();
                            PaintEvents(richText, g, rect);
                            PaintConditions(richText, g, rect);
                            PaintQuickButtons(richText, g);

                            if (richText.Image != null)
                            {
                                float imageWidth = (float)(rectRich.Width / richText.Page.Zoom) + 1;
                                float imageHeight = (float)(rectRich.Height / richText.Page.Zoom) + 1;


                                if (richText.Image is Metafile)
                                {
                                    var metafile = richText.Image as Metafile;
                                    var unit = GraphicsUnit.Pixel;
                                    var metafileRect = metafile.GetBounds(ref unit);
                                    if (metafileRect.Width > imageWidth && richText.WordWrap)
                                        imageWidth = metafileRect.Width;
                                }

                                try
                                {

                                    var imageRect = rectRich.ToRectangleF();
                                    g.DrawImage(richText.Image, imageRect,
                                        new RectangleF(0, 0, imageWidth, imageHeight), GraphicsUnit.Pixel);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }

                    #region Markers
                    if (e.DrawBorderFormatting)
                        PaintMarkers(richText, g, rect);
                    #endregion

                    #region Border
                    if (richText.HighlightState == StiHighlightState.Hide)
                        PaintBorder(richText, g, rect, richText.Page.Zoom, 
                            e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!richText.Border.Topmost));
                    #endregion

                    #region Image
                    if (e.DrawBorderFormatting)
                    {
                        if (richText.IsDesigning)
                            DrawCellImage(g, rect);
                    }
                    #endregion

                    PaintQuickButtons(richText, e.Graphics);
                }
            }
            e.Cancel = false;

            if (e.DrawBorderFormatting)
                richText.InvokePainted(richText, e);

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
