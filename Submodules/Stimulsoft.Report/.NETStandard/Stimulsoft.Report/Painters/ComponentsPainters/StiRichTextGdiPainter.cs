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
using Stimulsoft.Report.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Brushes = Stimulsoft.Drawing.Brushes;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Font = Stimulsoft.Drawing.Font;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiRichTextGdiPainter : StiComponentGdiPainter
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
            if (richText.Image == null && StiOptions.Engine.FullTrust) richText.RenderMetafile(richText.BackColor.A > 0);

            //когда-то проблема была только с прозрачным фоном. сейчас воспроизводится на любом, поэтому проверку выключаю.
            //if (richText.BackColor.A == 0)
            //{
                Stimulsoft.Report.Export.StiExportUtils.DisableFontSmoothing(component.Report);
            //}
            
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
                if (backColor.A == 0)
                {
                    if (format == StiExportFormat.ImagePng || format == StiExportFormat.Pdf)
                    {
                        backColor = Color.FromArgb(1, 255, 255, 255);
                    }
                    else
                    {
                        backColor = Color.White;
                    }
                }
                imageGraph.Clear(backColor);

                var metafileRect = new RectangleF();
                if (richText.Image is Metafile)
                {
                    var metafile = richText.Image as Metafile;
                    var unit = GraphicsUnit.Pixel;

                    try
                    {
                        metafileRect = metafile.GetBounds(ref unit);
                    }
                    catch
                    {
                        metafile.Dispose();
                        richText.RenderMetafile();
                        metafile = richText.Image as Metafile;
                        metafileRect = metafile.GetBounds(ref unit);
                    }
                }
                var sourceRect = new RectangleF(0, 0, rectRichF.Width + 2, rectRichF.Height + 2);
                if ((richText.Wysiwyg || StiDpiHelper.NeedGraphicsRichTextScale) && metafileRect.Width > 0)
                {
                    sourceRect = metafileRect;
                }
                
                if (StiOptions.Engine.FullTrust && richText.Image != null)
                {
                    imageGraph.DrawImage(richText.Image,
                        new Rectangle(
                            RoundValue(rectRichF.X * zoom),
                            RoundValue(rectRichF.Y * zoom),
                            RoundValue(rectRichF.Width * zoom),
                            RoundValue(rectRichF.Height * zoom)),
                        sourceRect,
                        GraphicsUnit.Pixel);
                }

#if NETSTANDARD
                imageGraph.FillRectangle(
                    Brushes.Red,
                    new Rectangle(
                        RoundValue(rectRichF.X * zoom),
                        RoundValue(rectRichF.Y * zoom),
                        RoundValue(rectRichF.Width * zoom),
                        RoundValue(rectRichF.Height * zoom))
                );

                imageGraph.FillRectangle(
                    Brushes.White,
                    new Rectangle(
                        RoundValue((rectRichF.X + 3) * zoom),
                        RoundValue((rectRichF.Y + 3) * zoom),
                        RoundValue((rectRichF.Width - 6) * zoom),
                        RoundValue((rectRichF.Height - 6) * zoom))
                );

                imageGraph.DrawString(
                    "The Rich Text component is not available in the .NET Core framework.", 
                    new Font("Verdana,Arial,Helvetica", 12 * zoom),
                    Brushes.Black,
                    new RectangleF(rectRichF.X * zoom, rectRichF.Y * zoom, rectRichF.Width * zoom, rectRichF.Height * zoom),
                    new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    }
                );
#endif
            }
            return bmp;
        }

        public virtual void PaintBackground(StiRichText text, Graphics g, RectangleD rect)
        {
            if (text.BackColor.A == 0 &&
                text.Report.Info.FillComponent &&
                text.IsDesigning)
            {
                Color color = Color.FromArgb(150, Color.White);

                StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
            }
            else StiDrawing.FillRectangle(g, text.BackColor, rect);
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var richText = (StiRichText)component;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!richText.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                richText.InvokePainting(richText, e);

            if (!e.Cancel && !(richText.Enabled == false && richText.IsDesigning == false))
            {
                var g = e.Graphics;

                var rect = richText.GetPaintRectangle();
				var rectRich = richText.ConvertTextMargins(rect, true);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    if (component.Report.IsExporting && richText.BackColor.A == 0)
                    {
                        Stimulsoft.Report.Export.StiExportUtils.DisableFontSmoothing(component.Report);
                    }

                    #region Fill rectangle
                    if (e.DrawBorderFormatting)
                        PaintBackground(richText, g, rect);
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        if (richText.Image == null)
                            richText.RenderMetafile();

                        if (richText.Image == null)
                        {
                            if (!string.IsNullOrEmpty(richText.DataColumn) || !string.IsNullOrEmpty(richText.DataUrl.Value))
                            {
                                using (var font = new Font("Segoe UI", (float) (10*richText.Page.Zoom)))
                                {
                                    string str = null;
                                    var hyperlink = richText.DataUrl != null ? richText.DataUrl.Value : null;
                                    if (!string.IsNullOrEmpty(hyperlink))
                                        str = StiHyperlinkProcessor.HyperlinkToString(hyperlink);
                                    else if (!string.IsNullOrEmpty(richText.DataColumn))
                                        str = string.Format("{0} {1}", StiLocalization.Get("Report", "LabelDataColumn"), richText.DataColumn);

                                    g.DrawString(str, font, Brushes.Black, rect.ToRectangleF());
                                }
                            }
                        }
                        else
                        {
                            PaintEvents(richText, g, rect);
                            PaintConditions(richText, e.Graphics, rect);

                            if (richText.Image != null)
                            {
                                float imageWidth = (float) (rectRich.Width/richText.Page.Zoom * (StiOptions.Engine.RichTextScale / 96f)) + 1;
                                float imageHeight = (float) (rectRich.Height/richText.Page.Zoom * (StiOptions.Engine.RichTextScale / 96f)) + 1;

                                var metafileRect = new RectangleF();
                                if (richText.Image is Metafile)
                                {
                                    var metafile = richText.Image as Metafile;
                                    var unit = GraphicsUnit.Pixel;

                                    try
                                    {
                                        metafileRect = metafile.GetBounds(ref unit);
                                    }
                                    catch
                                    {
                                        metafile.Dispose();
                                        richText.RenderMetafile();
                                        metafile = richText.Image as Metafile;
                                        metafileRect = metafile.GetBounds(ref unit);
                                    }

                                    if (metafileRect.Width > imageWidth && richText.WordWrap)
                                        imageWidth = metafileRect.Width;

                                    metafileRect.Width *= 96 / StiOptions.Engine.RichTextScale;
                                    metafileRect.Height *= 96 / StiOptions.Engine.RichTextScale;
                                }

                                try
                                {
                                    var imageRect = rectRich.ToRectangleF();
                                    var sourceRect = new RectangleF(0, 0, imageWidth, imageHeight);
                                    if (richText.Wysiwyg || StiDpiHelper.NeedGraphicsRichTextScale)
                                    {
                                        sourceRect = metafileRect;
                                    }
                                    g.DrawImage(richText.Image, imageRect, sourceRect, GraphicsUnit.Pixel);
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
                    {
                        PaintBorder(richText, g, rect, richText.Page.Zoom, 
                            e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!richText.Border.Topmost));
                    }
                    #endregion

                }
                if (e.DrawBorderFormatting)
                    PaintQuickButtons(richText, e.Graphics);
            }
            e.Cancel = false;

            if (e.DrawBorderFormatting)
                richText.InvokePainted(richText, e);
        }
        #endregion
    }
}