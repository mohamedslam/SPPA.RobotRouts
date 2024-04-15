#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft   							}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.Collections.Generic;
using System.Text;
using Stimulsoft.Base.Drawing;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Report.Export
{
    public class StiExcel2007GeomWriter : IStiExportGeomWriter
    {
        #region Variables
        private List<Stimulsoft.Report.Export.StiExcel2007ExportService.BarCodeLineData> barCodesList = null;
        private int row = 0;
        private int column = 0;

        private double offsetX = 0;
        private double offsetY = 0;
        #endregion

        #region Commands
        public void BeginPath()
        {
        }

        public void CloseFigure()
        {
        }

        public void EndPath()
        {
        }

        public void FillPath(object brush)
        {
        }

        public void StrokePath(object pen)
        {
        }

        public void MoveTo(PointF point)
        {
        }

        public void DrawLine(PointF pointFrom, PointF pointTo, object pen)
        {
        }

        public void DrawLineTo(PointF pointTo, object pen)
        {
        }

        public void DrawRectangle(RectangleF rect, object pen, StiCornerRadius corners = null)
        {
        }


        public void FillRectangle(RectangleF rect, Color color, StiCornerRadius corners = null)
        {
            var barCodeLine = new Stimulsoft.Report.Export.StiExcel2007ExportService.BarCodeLineData();
            barCodeLine.Row = row;
            barCodeLine.Column = column;
            barCodeLine.Position = new PointD(offsetX + rect.X, offsetY + rect.Top);
            barCodeLine.Size = new SizeD(rect.Width, rect.Height);
            barCodeLine.Color = color;

            barCodesList.Add(barCodeLine);
        }
        public void FillRectangle(RectangleF rect, object brush, StiCornerRadius corners = null)
        {
            StiBrush stiBrush = brush as StiBrush;
            if (stiBrush != null)
            {
                FillRectangle(rect, StiBrush.ToColor(stiBrush));
            }
        }


        public void DrawPolyline(PointF[] points, object pen)
        {
        }

        public void DrawPolygon(PointF[] points, object pen)
        {
        }

        public void DrawPolyline(PointF[] points, object pen, bool close, bool drawTo = false)
        {
        }

        public void DrawPolylineTo(PointF[] points, object pen)
        {
        }

        public void FillPolygon(PointF[] points, object brush)
        {
        }

        public void FillPolygons(List<List<PointF>> points, object brush)
        {
        }

        public void DrawBezier(PointF p1, PointF p2, PointF p3, PointF p4, object pen)
        {
        }

        public void DrawBezierTo(PointF p2, PointF p3, PointF p4, object pen)
        {
        }

        public void DrawSpline(PointF[] points, float tension, object pen)
        {
        }

        //public void DrawArc(RectangleF rect, float startAngle, float sweepAngle)
        //{
        //}

        public void DrawEllipse(RectangleF rect, object pen)
        {
        }

        public void FillEllipse(RectangleF rect, object brush)
        {
        }

        public void DrawPie(RectangleF rect, float startAngle, float sweepAngle)
        {
        }


        public void DrawImage(Image img, RectangleF rect)
        {
            //StiImage image = new StiImage();
            //image.ClientRectangle = RectangleD.CreateFromRectangle(rect);
            //image.ImageToDraw = img;
            ////image.HorAlignment = StiHorAlignment.Center;
            ////image.VertAlignment = StiVertAlignment.Center;
            //image.Smoothing = true;
            //image.Stretch = true;

            //if (assembleData)
            //{
            //    pdfService.StoreImageDataForGeom(image);
            //    return;
            //}

            //Stimulsoft.Report.Export.StiPdfExportService.StiPdfData pp = new StiPdfExportService.StiPdfData();
            //pp.Component = image;
            //pp.Width = rect.Width;
            //pp.Height = rect.Height;

            //pageStream.WriteLine("q");
            //pdfService.PushColorToStack();
            //pageStream.WriteLine("1 0 0 1 {0} {1} cm", ConvertToString(rect.X), ConvertToString(rect.Y + rect.Height));
            //pageStream.WriteLine("1 0 0 -1 0 0 cm");
            ////pageStream.WriteLine("1.39 0 0 1.39 0 0 cm");

            //pdfService.RenderImage(pp, 100f);

            //pageStream.WriteLine("Q");
            //pdfService.PopColorFromStack();
        }

        public void DrawString(string st, Font font, StiBrush brush, RectangleF rect, StringFormat sf, bool allowHtmlTags = false)
        {
            //StiText txt = new StiText();
            //txt.ClientRectangle = RectangleD.CreateFromRectangle(rect);
            //txt.Text = st;
            //txt.Font = font;
            //txt.TextBrush = brush;
            //if (sf.Alignment == StringAlignment.Center) txt.HorAlignment = StiTextHorAlignment.Center;
            //if (sf.Alignment == StringAlignment.Far) txt.HorAlignment = StiTextHorAlignment.Right;
            //if (sf.LineAlignment == StringAlignment.Center) txt.VertAlignment = StiVertAlignment.Center;
            //if (sf.LineAlignment == StringAlignment.Far) txt.VertAlignment = StiVertAlignment.Bottom;

            //Stimulsoft.Report.Export.StiPdfExportService.StiPdfData pp = new StiPdfExportService.StiPdfData();
            //pp.Component = txt;
            //pp.Width = rect.Width * hiToTwips;
            //pp.Height = rect.Height * hiToTwips;

            //pageStream.WriteLine("q");
            //pdfService.PushColorToStack();
            //pageStream.WriteLine("1 0 0 1 {0} {1} cm", ConvertToString(rect.X), ConvertToString(rect.Y + rect.Height));
            //pageStream.WriteLine("1 0 0 -1 0 0 cm");
            //pageStream.WriteLine("1.39 0 0 1.39 0 0 cm");

            //pdfService.RenderTextFont(pp);
            //pdfService.RenderText(pp);

            //pageStream.WriteLine("Q");
            //pdfService.PopColorFromStack();
        }

        public void SaveState()
        {
        }

        public void RestoreState()
        {
        }

        public void TranslateTransform(float x, float y)
        {
            offsetX += x;
            offsetY += y;
        }

        public void RotateTransform(float angle)
        {
        }

        public void SetClip(RectangleF rect)
        {
        }
        #endregion

        #region GDI specific commands
        public void DrawArc(RectangleF rect, PointF p1, PointF p2, Pen pen)
        {
        }

        public void DrawText(PointF basePoint, string text, int[] charsOffset, Font font, Color textColor, float angle, EmfTextAlignmentMode textAlign)
        {
        }

        public void SetPixel(PointF point, Color color)
        {
        }

        public SizeF MeasureString(string st, Font font)
        {
            using (Bitmap bmp = new Bitmap(1, 1))
            {
                Graphics g = Graphics.FromImage(bmp);
                var size = g.MeasureString(st, font);
                g.Dispose();
                return size;
            }
        }
        #endregion

        public StiExcel2007GeomWriter()
        {
        }

        internal StiExcel2007GeomWriter(List<Stimulsoft.Report.Export.StiExcel2007ExportService.BarCodeLineData> barCodeLinesList, int row, int column)
        {
            this.barCodesList = barCodeLinesList;
            this.row = row;
            this.column = column;
        }

    }

}
