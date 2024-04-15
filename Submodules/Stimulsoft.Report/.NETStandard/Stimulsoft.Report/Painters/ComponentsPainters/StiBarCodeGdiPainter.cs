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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiBarCodeGdiPainter : StiComponentGdiPainter, IStiBarCodePainter
    {
        #region IStiBarCodePainter
        public void BaseTransform(object context, float x, float y, float angle, float dx, float dy)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = context as Graphics;

            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            g.TranslateTransform(dx, dy);
        }
        
        public void BaseRollbackTransform(object context)
        {
        }

        public void BaseFillRectangle(object context, StiBrush brush, float x, float y, float width, float height)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = context as Graphics;

            using (var gdiBrush = StiBrush.GetBrush(brush, new RectangleF(x, y, width, height)))
            {
                g.FillRectangle(gdiBrush, x, y, width, height);
            }
        }

        public void BaseFillRectangle2D(object context, StiBrush brush, float x, float y, float width, float height)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = context as Graphics;

            using (var gdiBrush = StiBrush.GetBrush(brush, new RectangleF(x, y, width, height)))
            {
                g.FillRectangle(gdiBrush, x, y, width, height);
            }
        }

        public void BaseFillPolygon(object context, StiBrush brush, PointF[] points)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = context as Graphics;

            //find bounds
            float minX = points[0].X;
            float maxX = points[0].X;
            float minY = points[0].Y;
            float maxY = points[0].Y;
            foreach (var point in points)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.Y > maxY) maxY = point.Y;
            }

            using (var gdiBrush = StiBrush.GetBrush(brush, new RectangleF(minX, minY, maxX - minX, maxY - minY)))
            {
                g.FillPolygon(gdiBrush, points);
            }
        }

        public void BaseFillPolygons(object context, StiBrush brush, List<List<PointF>> points, RectangleF? rectf = null)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (points.Count == 0) return;

            var g = context as Graphics;

            if (rectf == null)
            {
                //find bounds
                float minX = points[0][0].X;
                float maxX = points[0][0].X;
                float minY = points[0][0].Y;
                float maxY = points[0][0].Y;
                foreach (List<PointF> list in points)
                {
                    foreach (var point in list)
                    {
                        if (point.X < minX) minX = point.X;
                        if (point.X > maxX) maxX = point.X;
                        if (point.Y < minY) minY = point.Y;
                        if (point.Y > maxY) maxY = point.Y;
                    }
                }
                rectf = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            }

            using (var gdiBrush = StiBrush.GetBrush(brush, rectf.Value))
            {
                foreach (List<PointF> list in points)
                {
                    g.FillPolygon(gdiBrush, list.ToArray());
                }
            }
        }

        public void BaseFillEllipse(object context, StiBrush brush, float x, float y, float width, float height)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = context as Graphics;

            using (var gdiBrush = StiBrush.GetBrush(brush, new RectangleF(x, y, width, height)))
            {
                g.FillEllipse(gdiBrush, x, y, width, height);
            }
        }

        public void BaseDrawRectangle(object context, Color penColor, float penSize, float x, float y, float width, float height)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = context as Graphics;

            using (var pen = new Pen(penColor, penSize))
            {
                g.DrawRectangle(pen, x, y, width, height);
            }
        }

        public void BaseDrawImage(object context, Image image, StiReport report, float x, float y, float width, float height)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (image != null)
            {
                var g = context as Graphics;
                g.DrawImage(image, x, y, width, height);
            }
        }

        public void BaseDrawString(object context, string st, Font font, StiBrush brush, RectangleF rect, StringFormat sf)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = context as Graphics;

            using (var gdiBrush = StiBrush.GetBrush(brush, rect))
            {
                var defaultHint = g.TextRenderingHint;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                g.DrawString(st, font, gdiBrush, rect, sf);

                g.TextRenderingHint = defaultHint;
            }
        }

        public SizeF BaseMeasureString(object context, string st, Font font)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = context as Graphics;

            return g.MeasureString(st, font);
        }
        #endregion

        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var barCode = component as StiBarCode;

            zoom *= StiOptions.Engine.BarcodeDpiMultiplierFactor;
            double resZoom = barCode.Report.Info.Zoom;
            barCode.Report.Info.Zoom = zoom;

            //var rect = barCode.GetPaintRectangle();  //fix, GetPaintRectangle now apply StiScale.Factor, so use ClientRectangle
            var rect = component.ComponentToPage(component.ClientRectangle).Normalize();
            rect = barCode.Report.Unit.ConvertToHInches(rect).Multiply(zoom);

            rect.X = 0;
            rect.Y = 0;
            barCode.Report.Info.Zoom = resZoom;

            int imageWidth = (int)rect.Width;
            int imageHeight = (int)rect.Height;

            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                if ((format != StiExportFormat.Pdf) && (format != StiExportFormat.ImagePng))
                {
                    g.Clear(Color.White);
                }
                else
                {
                    //g.FillRectangle(new SolidBrush(Color.FromArgb(1, 255, 255, 255)), new Rectangle(0, 0, imageWidth, imageHeight));
                    g.Clear(Color.FromArgb(1, 255, 255, 255));
                }
                if (!string.IsNullOrEmpty(barCode.CodeValue))
                {
                    barCode.BarCodeType.Draw(g, barCode, rect.ToRectangleF(), (float)zoom);
                }
            }
            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var barCode = component as StiBarCode;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!barCode.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                barCode.InvokePainting(barCode, e);

            if (!e.Cancel && (!(component.Enabled == false && component.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = barCode.GetPaintRectangle();
                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    if (e.DrawBorderFormatting)
                    {
                        var state = g.Save();
                        if (!(barCode.IsPrinting || barCode.IsExporting || StiOptions.Engine.DisableAntialiasingInPainters))
                        {
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                        }

                        g.SetClip(rect.ToRectangleF(), CombineMode.Intersect);

                        if (barCode.Page != null)
                        {
                            if (barCode.IsDesigning || (!string.IsNullOrEmpty(barCode.CodeValue)))
                            {
                                float zoom = (float)(barCode.Page.Zoom * StiScale.Factor);
                                if (barCode.IsPrinting && StiOptions.Print.BarcodeAsBitmap)
                                {
                                    var image = GetImage(barCode, ref zoom, StiExportFormat.ImagePng);
                                    g.DrawImage(image, rect.ToRectangleF());
                                }
                                else
                                {
                                    barCode.BarCodeType.Draw(g, barCode, rect.ToRectangleF(), zoom);
                                }
                            }
                        }

                        g.Restore(state);
                    }

                    #region Markers
                    if (e.DrawBorderFormatting)
                        PaintMarkers(barCode, g, rect);
                    #endregion

                    #region Border
                    PaintBorder(barCode, g, rect, (float)barCode.Page.Zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!barCode.Border.Topmost));
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        PaintEvents(barCode, e.Graphics, rect);
                        PaintConditions(barCode, e.Graphics, rect);
                    }
                }
            }
            e.Cancel = false;

            if (e.DrawBorderFormatting)
                barCode.InvokePainted(barCode, e);

        }
        #endregion
    }
}