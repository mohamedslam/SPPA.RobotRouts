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
using System.IO;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Globalization;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using HatchBrush = Stimulsoft.Drawing.Drawing2D.HatchBrush;
using Matrix = Stimulsoft.Drawing.Drawing2D.Matrix;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Report.Export
{
    public class StiBarCodeExportPainter : Stimulsoft.Report.Painters.IStiBarCodePainter
    {
        #region Fields
        private IStiExportGeomWriter geomWriter = null;
        #endregion

        #region Constants
        private float pdfGeomWriterInflate = -0.09f;
        #endregion

        #region Properties
        public bool OnlyAssembleData => (geomWriter is StiPdfGeomWriter) && (geomWriter as StiPdfGeomWriter).assembleData;
        #endregion

        #region IStiBarCodePainter
        public void BaseTransform(object context, float x, float y, float angle, float dx, float dy)
        {
            //geomWriter.SaveState();

            geomWriter.TranslateTransform(x, y);
            geomWriter.RotateTransform(angle);
            geomWriter.TranslateTransform(dx, dy);
        }
        
        public void BaseRollbackTransform(object context)
        {
            if (geomWriter is StiSvgGeomWriter)
            {
                var gw = geomWriter as StiSvgGeomWriter;

                gw.EndTransform();
                gw.EndTransform();
                gw.EndTransform();
            }

            //geomWriter.RestoreState();
        }

        public void BaseFillRectangle(object context, StiBrush brush, float x, float y, float width, float height)
        {
            RectangleF rectF = new RectangleF(x, y, width, height);
            if (geomWriter is StiPdfGeomWriter) rectF.Inflate(pdfGeomWriterInflate, pdfGeomWriterInflate);
            geomWriter.FillRectangle(rectF, brush);
        }

        public void BaseFillRectangle2D(object context, StiBrush brush, float x, float y, float width, float height)
        {
            RectangleF rectF = new RectangleF(x, y, width, height);
            if (geomWriter is StiPdfGeomWriter) rectF.Inflate(pdfGeomWriterInflate, pdfGeomWriterInflate);
            geomWriter.FillRectangle(rectF, brush);
        }

        public void BaseFillPolygon(object context, StiBrush brush, PointF[] points)
        {
            geomWriter.FillPolygon(points, brush);
        }

        public void BaseFillPolygons(object context, StiBrush brush, List<List<PointF>> points, RectangleF? rectf = null)
        {
            geomWriter.FillPolygons(points, brush);
        }

        public void BaseFillEllipse(object context, StiBrush brush, float x, float y, float width, float height)
        {
            geomWriter.FillEllipse(new RectangleF(x, y, width, height), brush);
        }

        public void BaseDrawRectangle(object context, Color penColor, float penSize, float x, float y, float width, float height)
        {
            RectangleF rectF = new RectangleF(x, y, width, height);
            geomWriter.DrawRectangle(rectF, new Pen(penColor, penSize));
        }

        public void BaseDrawImage(object context, Image image, StiReport report, float x, float y, float width, float height)
        {
            if (image != null)
            {
                RectangleF rectF = new RectangleF(x, y, width, height);
                geomWriter.DrawImage(image, rectF);
            }
        }

        public void BaseDrawString(object context, string st, Font font, StiBrush brush, RectangleF rect, StringFormat sf)
        {
            using (var gdiBrush = StiBrush.GetBrush(brush, rect))
            {
                geomWriter.DrawString(st, font, brush, rect, sf ?? new StringFormat());
            }
        }

        public SizeF BaseMeasureString(object context, string st, Font font)
        {
            return geomWriter.MeasureString(st, font);
        }
        #endregion

        public StiBarCodeExportPainter(StiSvgGeomWriter geomWriter)
        {
            this.geomWriter = geomWriter;
        }

        public StiBarCodeExportPainter(StiPdfGeomWriter geomWriter)
        {
            this.geomWriter = geomWriter;
        }

        public StiBarCodeExportPainter(StiExcel2007GeomWriter geomWriter)
        {
            this.geomWriter = geomWriter;
        }
    }

    public interface IStiExportGeomWriter
    {
        void BeginPath();
        void CloseFigure();
        void EndPath();
        void FillPath(object brush);
        void StrokePath(object pen);
        void MoveTo(PointF point);
        void DrawLine(PointF pointFrom, PointF pointTo, object pen);
        void DrawLineTo(PointF pointTo, object pen);
        void DrawRectangle(RectangleF rect, object pen, StiCornerRadius corners = null);
        void FillRectangle(RectangleF rect, Color color, StiCornerRadius corners = null);
        void FillRectangle(RectangleF rect, object brush, StiCornerRadius corners = null);
        void DrawPolyline(PointF[] points, object pen);
        void DrawPolylineTo(PointF[] points, object pen);
        void DrawPolygon(PointF[] points, object pen);
        void FillPolygon(PointF[] points, object brush);
        void FillPolygons(List<List<PointF>> points, object brush);
        void DrawBezier(PointF p1, PointF p2, PointF p3, PointF p4, object pen);
        void DrawBezierTo(PointF p2, PointF p3, PointF p4, object pen);
        //void DrawArc(RectangleF rect, PointF p1, PointF p2, Pen pen);
        void DrawEllipse(RectangleF rect, object pen);
        void FillEllipse(RectangleF rect, object brush);
        void SetPixel(PointF point, Color color);
        void DrawImage(Image img, RectangleF rect);
        void DrawText(PointF basePoint, string text, int[] charsOffset, Font font, Color textColor, float angle, EmfTextAlignmentMode textAlign);
        void DrawString(string st, Font font, StiBrush brush, RectangleF rect, StringFormat sf, bool allowHtmlTags = false);
        void SaveState();
        void RestoreState();
        void TranslateTransform(float x, float y);
        void RotateTransform(float angle);
        SizeF MeasureString(string st, Font font);
    }

    public class StiPdfGeomWriter : IStiExportGeomWriter
    {
        #region Constants
        private const double penWidthDefault = 0.1;
        private const int precision_digits = 3;
        private const double hiToTwips = 0.72;
        private const float pdfCKT = 0.55228f; 
        #endregion

        #region Variables
        private PointF lastPoint;
        private bool makePath = false;
        private bool pathClosed = false;
        internal StringBuilder path = null;
        private NumberFormatInfo currentNumberFormat = null;

        internal StreamWriter pageStream = null;
        private StiPdfExportService pdfService = null;
        internal bool assembleData = false;
        internal int pageNumber = 0;
        internal Stack<Matrix> matrixCache = null;
        internal bool allowThinLines = false;
        internal PointD basePoint = new PointD();
        internal float componentAngle = 0;
        internal bool forceNewPoint = false;

        private float? xmin = null;
        private float? xmax = null;
        private float? ymin = null;
        private float? ymax = null;
        #endregion

        #region Utils
        private void CalculateMinMax(PointF pt)
        {
            if (xmin == null)
            {
                xmin = pt.X;
            }
            else
            {
                xmin = Math.Min(xmin.Value, pt.X);
            }
            if (xmax == null)
            {
                xmax = pt.X;
            }
            else
            {
                xmax = Math.Max(xmax.Value, pt.X);
            }
            if (ymin == null)
            {
                ymin = pt.Y;
            }
            else
            {
                ymin = Math.Min(ymin.Value, pt.Y);
            }
            if (ymax == null)
            {
                ymax = pt.Y;
            }
            else
            {
                ymax = Math.Max(ymax.Value, pt.Y);
            }
        }

        private string ConvertToString(double Value)
        {
            decimal numValue = Math.Round((decimal)Value, precision_digits);
            return numValue.ToString("G", currentNumberFormat);
        }

        private bool IsPenEmpty(object objPen)
        {
            if (objPen == null) return true;
            if (objPen is Pen)
            {
                return (objPen as Pen).Color.A == 0;
            }
            if (objPen is Stimulsoft.Base.Context.StiPenGeom)
            {
                var pen = objPen as Stimulsoft.Base.Context.StiPenGeom;

                Color color = Color.Transparent;
                if (pen.Brush is Color)
                {
                    color = (Color)pen.Brush;
                }
                if (pen.Brush is StiBrush)
                {
                    color = StiBrush.ToColor(pen.Brush as StiBrush);
                }

                return (color.A == 0) || (pen.PenStyle == StiPenStyle.None);
            }
            return false;
        }

        public bool SetPen(object objPen, bool saveState = false, bool forceThin = false)
        {
            bool needSaveState = false;
            if (objPen is Pen)
            {
                var pen = objPen as Pen;
                pdfService.SetStrokeColor(pen.Color);

                double penWidth = (pen.Width > hiToTwips) ? pen.Width : pen.Width * hiToTwips;
                if (penWidth == 0) penWidth = penWidthDefault;
                double penWidthForDash = pen.Width;
                if (allowThinLines)
                {
                    penWidth = ((pen.Width < 0.5) || (forceThin && pen.Width == 1)) ? penWidthDefault : pen.Width;
                    penWidthForDash = (penWidth > 0.1) ? penWidth : penWidthDefault * 4;
                }
                pageStream.WriteLine("{0} w", ConvertToString(penWidth));

                needSaveState = saveState && (pen.DashStyle != DashStyle.Solid);
                if (needSaveState)
                {
                    pageStream.WriteLine("q");
                }
                string dash = GetPenStyleDashString(pen.DashStyle, penWidthForDash * 0.035);
                if (dash != null)
                {
                    pageStream.WriteLine(dash);
                }
            }
            if (objPen is Stimulsoft.Base.Context.StiPenGeom)
            {
                var pen = objPen as Stimulsoft.Base.Context.StiPenGeom;

                Color color = Color.Transparent;
                if (pen.Brush is Color)
                {
                    color = (Color)pen.Brush;
                }
                if (pen.Brush is StiBrush)
                {
                    color = StiBrush.ToColor(pen.Brush as StiBrush);
                }

                pdfService.SetStrokeColor(color);

                double penWidth = (pen.Thickness > hiToTwips) ? pen.Thickness : pen.Thickness * hiToTwips;
                if (penWidth == 0) penWidth = penWidthDefault;
                double penWidthForDash = pen.Thickness;
                if (allowThinLines)
                {
                    penWidth = ((pen.Thickness < 0.5) || (forceThin && pen.Thickness == 1)) ? penWidthDefault : pen.Thickness;
                    penWidthForDash = (penWidth > 0.1) ? penWidth : penWidthDefault * 4;
                }
                pageStream.WriteLine("{0} w", ConvertToString(penWidth));

                needSaveState = saveState && (pen.PenStyle != StiPenStyle.Solid);
                if (needSaveState)
                {
                    pageStream.WriteLine("q");
                }
                string dash = GetPenStyleDashString(StiPenUtils.GetPenStyle(pen.PenStyle), penWidthForDash * 0.035);
                if (dash != null)
                {
                    pageStream.WriteLine(dash);
                }
            }
            return needSaveState;
        }

        public bool SetBrush(object brush, RectangleF rect, out bool isTransparent, bool saveState = false)
        {
            isTransparent = false;
            if (assembleData)
            {
                if ((brush is StiGradientBrush) || (brush is StiGlareBrush) || (brush is StiHatchBrush) || (brush is StiGlassBrush))
                {
                    pdfService.StoreShadingData1(brush as StiBrush, pageNumber);
                    pdfService.StoreHatchData(brush as StiBrush);
                }
                return false;
            }

            bool needSaveState = false;
            Color tempColor = Color.Transparent;
            if (brush is Color)
            {
                tempColor = (Color)brush;
            }
            if (brush is StiBrush)
            {
                tempColor = StiBrush.ToColor(brush as StiBrush);
            }
            pdfService.SetNonStrokeColor(tempColor);
            if (tempColor.A == 0) isTransparent = true;
            if ((brush is StiGradientBrush) || (brush is StiGlareBrush) || (brush is StiHatchBrush) || (brush is StiGlassBrush))
            {
                Matrix mm = matrixCache.Peek();
                PointF p1 = new PointF(rect.X, rect.Bottom);
                PointF p2 = new PointF(rect.Right, rect.Y);
                PointF[] points = new PointF[] { p1, p2 };
                mm.TransformPoints(points);
                RectangleF rect2 = new RectangleF(points[0].X, points[0].Y, points[1].X - points[0].X, points[1].Y - points[0].Y);

                int shadingCurrent = pdfService.StoreShadingData2(
                    Math.Min(rect2.X, rect2.Right),
                    Math.Min(rect2.Y, rect2.Bottom),
                    Math.Abs(rect2.Width),
                    Math.Abs(rect2.Height),
                    brush as StiBrush,
                    componentAngle);
                if (brush is StiGradientBrush || brush is StiGlareBrush)
                {
                    pageStream.WriteLine("/Pattern cs /P{0} scn", 1 + shadingCurrent);
                }
                if (brush is StiHatchBrush)
                {
                    StiHatchBrush hBrush = brush as StiHatchBrush;
                    pageStream.WriteLine("/Cs1 cs /PH{0} scn", pdfService.GetHatchNumber(hBrush) + 1);
                }
                if (brush is StiGlassBrush)
                {
                    StiGlassBrush glass = brush as StiGlassBrush;
                    //it is difficult to implement the desired functionality here, so far only a lower part of glass will be work
                    if (glass.DrawHatch)
                    {
                        pageStream.WriteLine("/Cs1 cs /PH{0} scn", pdfService.GetHatchNumber(glass.GetTopBrush() as HatchBrush) + 1);
                    }
                    else
                    {
                        pdfService.SetNonStrokeColor(glass.GetTopColor());
                    }
                    if (glass.DrawHatch)
                    {
                        pageStream.WriteLine("/Cs1 cs /PH{0} scn", pdfService.GetHatchNumber(glass.GetBottomBrush() as HatchBrush) + 1);
                    }
                    else
                    {
                        pdfService.SetNonStrokeColor(glass.GetBottomColor());
                    }
                }
                needSaveState = true;
            }
            needSaveState = needSaveState && saveState;
            if (needSaveState)
            {
                pageStream.WriteLine("q");
            }
            return needSaveState;
        }

        //private void SetNonStrokeColor(Brush brush)
        //{
        //    pdfService.SetNonStrokeColor((brush as SolidBrush).Color);
        //}

        private void OutputLineString(string st, object pen, bool forceThin = false)
        {
            if (!IsPenEmpty(pen))
            {
                bool needRestoreState = SetPen(pen, true, forceThin);
                pageStream.WriteLine(st);
                if (needRestoreState)
                {
                    pageStream.WriteLine("Q");
                }
            }
        }

        private PointF[] ConvertArcToBezierPoints(RectangleF rect, float startAngle, float sweepAngle)
        {
            float cx = rect.X + rect.Width / 2;
            float cy = rect.Y + rect.Height / 2;
            float rx = rect.Width / 2;
            float ry = rect.Height / 2;
            startAngle *= (float)(Math.PI / 180);
            sweepAngle *= (float)(Math.PI / 180);

            int nCurves = (int)Math.Ceiling(Math.Abs(sweepAngle) / (Math.PI / 2));
            PointF[] pts = new PointF[3 * nCurves + 1];
            float aSweep = sweepAngle / nCurves;

            // calculates control points for Bezier approx. of arc with radius=1,
            // circle center at (0,0), middle of arc at (1,0)
            double y0 = Math.Sin(aSweep / 2);
            double x0 = Math.Cos(aSweep / 2);
            double tx = (1 - x0) * 4 / 3;
            double ty = y0 - tx * x0 / (y0 + 0.0001);
            double[] px = new double[4];
            double[] py = new double[4];
            px[0] = x0;
            py[0] = -y0;
            px[1] = x0 + tx;
            py[1] = -ty;
            px[2] = x0 + tx;
            py[2] = ty;
            px[3] = x0;
            py[3] = y0;

            // rotation and translation of control points
            double sn = Math.Sin(startAngle + aSweep / 2);
            double cs = Math.Cos(startAngle + aSweep / 2);
            pts[0].X = (float)(cx + rx * (px[0] * cs - py[0] * sn));
            pts[0].Y = (float)(cy + ry * (px[0] * sn + py[0] * cs));

            for (int iCurve = 0; iCurve < nCurves; iCurve++)
            {
                float aStart = startAngle + aSweep * iCurve;
                sn = Math.Sin(aStart + aSweep / 2);
                cs = Math.Cos(aStart + aSweep / 2);
                for (int index = 1; index <= 3; index++)
                {
                    pts[index + iCurve * 3].X = (float)(cx + rx * (px[index] * cs - py[index] * sn));
                    pts[index + iCurve * 3].Y = (float)(cy + ry * (px[index] * sn + py[index] * cs));
                }
            }

            return pts;
        }


        public PointF[] ConvertSplineToCubicBezier(PointF[] points, float tension)
		{
			var count = points.Length;
			int len_pt = count * 3 - 2;
			PointF[] pt = new PointF[len_pt];
		
			tension = tension * 0.3f;
			
			pt[0] = points[0];
			pt[1] = CalculateCurveBezierEndPoints(points[0], points[1], tension);
			
			for(int index = 0; index < count - 2; index++)
			{
                PointF[] temp = CalculateCurveBezier(points, index, tension);
				
				pt[3 * index + 2] = temp[0];
				pt[3 * index + 3] = points[index+1];
				pt[3 * index + 4] = temp[1];
			}
			
			pt[len_pt - 2] = CalculateCurveBezierEndPoints(points[count - 1], points[count - 2], tension);
			pt[len_pt - 1] = points[count - 1];
			
			return pt;
		}
		
        private PointF[] CalculateCurveBezier(PointF[] points, int index, float tension)
		{
			float xDiff = points[index + 2].X - points[index + 0].X;
			float yDiff = points[index + 2].Y - points[index + 0].Y;

			return new PointF[] {
                new PointF(points[index + 1].X - tension * xDiff, points[index + 1].Y - tension * yDiff),
                new PointF(points[index + 1].X + tension * xDiff, points[index + 1].Y + tension * yDiff) };
		}

        private PointF CalculateCurveBezierEndPoints(PointF end, PointF adj, float tension)
        {
            return new PointF(tension * (adj.X - end.X) + end.X, tension * (adj.Y - end.Y) + end.Y);
        }
        #endregion

        #region GetStrings
        internal string GetPointString(PointF point)
        {
            CalculateMinMax(point);
            return string.Format("{0} {1} {2} ",
                ConvertToString(point.X),
                ConvertToString(point.Y),
                (makePath && (path.Length > 0)) && !forceNewPoint ? "l" : "m");
        }
        internal string GetLineToString(PointF pointTo)
        {
            CalculateMinMax(pointTo);
            return string.Format("{0} {1} l ",
                ConvertToString(pointTo.X),
                ConvertToString(pointTo.Y));
        }
        internal string GetLineString(PointF pointFrom, PointF pointTo)
        {
            CalculateMinMax(pointFrom);
            CalculateMinMax(pointTo);
            return string.Format("{0} {1} m {2} {3} l ",
                ConvertToString(pointFrom.X),
                ConvertToString(pointFrom.Y),
                ConvertToString(pointTo.X),
                ConvertToString(pointTo.Y));
        }

        internal string GetRectString(double x, double y, double width, double height)
        {
            return GetRectString(new RectangleF((float)x, (float)y, (float)width, (float)height));
        }
        internal string GetRectString(RectangleF rect)
        {
            return string.Format("{0} {1} {2} {3} re ",
                ConvertToString(rect.X),
                ConvertToString(rect.Top),
                ConvertToString(rect.Right - rect.X),
                ConvertToString(rect.Bottom - rect.Y));
        }

        internal string GetRectWithCornersString(RectangleF rect, StiCornerRadius corners)
        {
            if (corners == null) return GetRectString(rect);

            BeginPath();

            float cf = corners.TopLeft * 2 * (float)hiToTwips;
            if (corners.TopLeft == 0)
                MoveTo(new PointF(rect.X, rect.Top));
            else
                DrawArc(new RectangleF(rect.X, rect.Top, cf, cf), 180, 90);
            cf = corners.TopRight * 2 * (float)hiToTwips;
            if (corners.TopRight == 0)
                MoveTo(new PointF(rect.Right, rect.Top));
            else
                DrawArc(new RectangleF(rect.Right - cf, rect.Top, cf, cf), 270, 90);
            cf = corners.BottomRight * 2 * (float)hiToTwips;
            if (corners.BottomRight == 0)
                MoveTo(new PointF(rect.Right, rect.Bottom));
            else
                DrawArc(new RectangleF(rect.Right - cf, rect.Bottom - cf, cf, cf), 0, 90);
            cf = corners.BottomLeft * 2 * (float)hiToTwips;
            if (corners.BottomLeft == 0)
                MoveTo(new PointF(rect.X, rect.Bottom));
            else
                DrawArc(new RectangleF(rect.X, rect.Bottom - cf, cf, cf), 90, 90);

            CloseFigure();
            EndPath();

            return path.ToString();
        }

        internal string GetBezierString(PointF p1, PointF p2, PointF p3)
        {
            CalculateMinMax(p1);
            CalculateMinMax(p2);
            CalculateMinMax(p3);
            return string.Format("{0} {1} {2} {3} {4} {5} c ",
                ConvertToString(p1.X),
                ConvertToString(p1.Y),
                ConvertToString(p2.X),
                ConvertToString(p2.Y),
                ConvertToString(p3.X),
                ConvertToString(p3.Y));
        }
        internal string GetBezierString(float p1x, float p1y, float p2x, float p2y, float p3x, float p3y)
        {
            CalculateMinMax(new PointF(p1x, p1y));
            CalculateMinMax(new PointF(p2x, p2y));
            CalculateMinMax(new PointF(p3x, p3y));
            return string.Format("{0} {1} {2} {3} {4} {5} c ",
                ConvertToString(p1x),
                ConvertToString(p1y),
                ConvertToString(p2x),
                ConvertToString(p2y),
                ConvertToString(p3x),
                ConvertToString(p3y));
        }

        internal string GetPolylineString(PointF[] points, bool close, bool drawTo)
        {
            StringBuilder sb = new StringBuilder();
            if (!drawTo)
            {
                sb.Append(GetPointString(points[0]));
            }
            for (int index = (drawTo ? 0 : 1); index < points.Length; index++)
            {
                sb.Append(GetLineToString(points[index]));
            }
            if (close)
            {
                sb.Append("h ");
            }
            return sb.ToString();
        }

        internal string GetEllipseString(double x, double y, double width, double height)
        {
            return GetEllipseString(new RectangleF((float)x, (float)y, (float)width, (float)height));
        }
        internal string GetEllipseString(RectangleF rect)
        {
            float tmpX = rect.Width / 2 * (1 - pdfCKT);
            float tmpY = rect.Height / 2 * (1 - pdfCKT);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} {1} m ", ConvertToString(rect.X), ConvertToString(rect.Y + rect.Height / 2));
            sb.Append(GetBezierString(
                rect.X, rect.Y + rect.Height - tmpY,
                rect.X + tmpX, rect.Y + rect.Height,
                rect.X + rect.Width / 2, rect.Y + rect.Height));
            sb.Append(GetBezierString(
                rect.X + rect.Width - tmpX, rect.Y + rect.Height,
                rect.X + rect.Width, rect.Y + rect.Height - tmpY,
                rect.X + rect.Width, rect.Y + rect.Height / 2));
            sb.Append(GetBezierString(
                rect.X + rect.Width, rect.Y + tmpY,
                rect.X + rect.Width - tmpX, rect.Y,
                rect.X + rect.Width / 2, rect.Y));
            sb.Append(GetBezierString(
                rect.X + tmpX, rect.Y,
                rect.X, rect.Y + tmpY,
                rect.X, rect.Y + rect.Height / 2));

            return sb.ToString();
        }

        private string GetPenStyleDashString(DashStyle style, double step)
        {
            double pow = 1 / Math.Pow(10, precision_digits);
            if (step < pow) step = pow;
            switch (style)
            {
                case DashStyle.Dot:
                    return string.Format("[{0} {1}] 0 d", ConvertToString(step), ConvertToString(step * 58));

                case DashStyle.Dash:
                    return string.Format("[{0} {1}] 0 d", ConvertToString(step * 49.5), ConvertToString(step * 62));

                case DashStyle.DashDot:
                    return string.Format("[{0} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));

                case DashStyle.DashDotDot:
                    return string.Format("[{0} {1} {2} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));
            }
            return null;
        }
        #endregion

        #region Commands
        public void BeginPath()
        {
            if (assembleData) return;
            path = new StringBuilder();
            makePath = true;
            pathClosed = false;
            xmin = null;
            xmax = null;
            ymin = null;
            ymax = null;
            forceNewPoint = false;
        }

        public void CloseFigure()
        {
            if (assembleData) return;
            path.Append("h ");
            pathClosed = true;
        }

        public void EndPath()
        {
            if (assembleData) return;
            //path.CloseAllFigures(); // ??? пока так
            makePath = false;
            //pathClosed = true;
        }

        public void FillPath(object brush)
        {
            RectangleF rect = assembleData ? new RectangleF() : new RectangleF(xmin.Value, ymin.Value, xmax.Value - xmin.Value, ymax.Value - ymin.Value);
            bool isTransparent;
            bool needRestore = SetBrush(brush, rect, out isTransparent, true);
            if (assembleData) return;
            if (!pathClosed)
            {
                EndPath();
            }
            if (!isTransparent)
            {
                pageStream.Write(path.ToString());
                pageStream.WriteLine("f");
            }
            if (needRestore) pageStream.WriteLine("Q");
        }

        public void StrokePath(object pen)
        {
            if (assembleData) return;
            if (!IsPenEmpty(pen))
            {
                SetPen(pen);
                pageStream.Write(path.ToString());
                pageStream.WriteLine("S");
            }
        }


        public void MoveTo(PointF point)
        {
            if (assembleData) return;
            lastPoint = point;
            string stPoint = GetPointString(point);
            if (makePath)
            {
                path.AppendLine(stPoint);
            }
            else
            {
                pageStream.WriteLine(stPoint);
            }
        }


        public void DrawLine(PointF pointFrom, PointF pointTo, object pen)
        {
            if (assembleData) return;
            string stLine = GetPointString(pointFrom) + GetLineToString(pointTo);
            if (makePath)
            {
                path.AppendLine(stLine);
            }
            else
            {
                bool forceThin = pointFrom.X == pointTo.X || pointFrom.Y == pointTo.Y;
                OutputLineString(stLine + "S", pen, forceThin);
            }
            lastPoint = pointTo;
        }

        public void DrawLineTo(PointF pointTo, object pen)
        {
            if (assembleData) return;
            string stLine = GetLineToString(pointTo);
            if (makePath)
            {
                path.AppendLine(stLine);
            }
            else
            {
                OutputLineString(stLine + "S", pen);
            }
            lastPoint = pointTo;
        }


        public void DrawRectangle(RectangleF rect, object pen, StiCornerRadius corners = null)
        {
            if (assembleData) return;
            if (makePath) throw new Exception();    // !!!
            OutputLineString(GetRectWithCornersString(rect, corners) + "S", pen);
        }


        public void FillRectangle(RectangleF rect, Color color, StiCornerRadius corners = null)
        {
            if (color.A != 0)
                FillRectangle(rect, new StiSolidBrush(color), corners);
        }
        public void FillRectangle(RectangleF rect, object brush, StiCornerRadius corners = null)
        {
            bool isTransparent;
            bool needRestore = SetBrush(brush, rect, out isTransparent, true);
            if (assembleData) return;
            if (makePath) throw new Exception();    // !!!
            if (brush != null && !isTransparent)
            {
                StiGlassBrush glass = brush as StiGlassBrush;
                if (glass != null)
                {
                    if (glass.DrawHatch)
                    {
                        pageStream.WriteLine("/Cs1 cs /PH{0} scn", pdfService.GetHatchNumber(glass.GetTopBrush() as HatchBrush) + 1);
                    }
                    else
                    {
                        pdfService.SetNonStrokeColor(glass.GetTopColor());
                    }
                    if (corners == null)
                        pageStream.WriteLine(GetRectString(rect.X, rect.Y, rect.Width, rect.Height / 2) + "f");
                    else
                        pageStream.WriteLine(GetRectWithCornersString(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height / 2), new StiCornerRadius(corners.TopLeft, corners.TopRight, 0, 0)) + "f");

                    if (glass.DrawHatch)
                    {
                        pageStream.WriteLine("/Cs1 cs /PH{0} scn", pdfService.GetHatchNumber(glass.GetBottomBrush() as HatchBrush) + 1);
                    }
                    else
                    {
                        pdfService.SetNonStrokeColor(glass.GetBottomColor());
                    }
                    if (corners == null)
                        pageStream.WriteLine(GetRectString(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height / 2) + "f");
                    else
                        pageStream.WriteLine(GetRectWithCornersString(new RectangleF(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height / 2), new StiCornerRadius(0, 0, corners.BottomRight, corners.BottomLeft)) + "f");
                }
                else
                {
                    pageStream.WriteLine(GetRectWithCornersString(rect, corners) + "f");
                }
            }
            if (needRestore) pageStream.WriteLine("Q");
        }


        public void DrawPolyline(PointF[] points, object pen)
        {
            DrawPolyline(points, pen, false);
        }

        public void DrawPolygon(PointF[] points, object pen)
        {
            if (makePath) throw new Exception();    // !!!
            DrawPolyline(points, pen, true);
        }

        public void DrawPolyline(PointF[] points, object pen, bool close, bool drawTo = false)
        {
            if (assembleData) return;
            string stPolyline = GetPolylineString(points, close, drawTo);

            if (makePath)
            {
                path.AppendLine(stPolyline);
            }
            else
            {
                OutputLineString(stPolyline + "S", pen);
            }
            lastPoint = points[points.Length - 1];
        }

        public void DrawPolylineTo(PointF[] points, object pen)
        {
            DrawPolyline(points, pen, false, true);
        }


        public void FillPolygon(PointF[] points, object brush)
        {
            foreach (PointF pt in points) CalculateMinMax(pt);
            RectangleF rect = new RectangleF(xmin.Value, ymin.Value, xmax.Value - xmin.Value, ymax.Value - ymin.Value);
            bool isTransparent;
            bool needRestore = SetBrush(brush, rect, out isTransparent, true);
            if (assembleData) return;
            if (makePath) throw new Exception();    // !!!
            if (brush != null && !isTransparent)
            {
                string stPolygon = GetPolylineString(points, true, false);
                pageStream.WriteLine(stPolygon + "f");
            }
            if (needRestore) pageStream.WriteLine("Q");
        }

        public void FillPolygons(List<List<PointF>> points, object brush)
        {
            foreach (List<PointF> list in points)
            {
                foreach (PointF pt in list) CalculateMinMax(pt);
            }
            RectangleF rect = new RectangleF(xmin.Value, ymin.Value, xmax.Value - xmin.Value, ymax.Value - ymin.Value);
            bool isTransparent;
            bool needRestore = SetBrush(brush, rect, out isTransparent, true);
            if (assembleData) return;
            if (makePath) throw new Exception();    // !!!
            if (brush != null && !isTransparent)
            {
                foreach (List<PointF> list in points)
                {
                    string stPolygon = GetPolylineString(list.ToArray(), true, false);
                    pageStream.WriteLine(stPolygon + "f");
                }
            }
            if (needRestore) pageStream.WriteLine("Q");
        }

        public void DrawBezier(PointF p1, PointF p2, PointF p3, PointF p4, object pen)
        {
            if (assembleData) return;
            string stBezier = GetPointString(p1) + GetBezierString(p2, p3, p4);
            if (makePath)
            {
                path.AppendLine(stBezier);
            }
            else
            {
                OutputLineString(stBezier + "S", pen);
            }
            lastPoint = p4;
        }

        public void DrawBezierTo(PointF p2, PointF p3, PointF p4, object pen)
        {
            if (assembleData) return;
            string stBezier = GetBezierString(p2, p3, p4);
            if (makePath)
            {
                path.AppendLine(stBezier);
            }
            else
            {
                OutputLineString(stBezier + "S", pen);
            }
            lastPoint = p4;
        }

        public void DrawSpline(PointF[] points, float tension, object pen)
        {
            if (assembleData) return;

            var pts = ConvertSplineToCubicBezier(points, tension);
            int nCurves = (pts.Length - 1) / 3;

            StringBuilder stSpline = new StringBuilder();
            stSpline.Append(GetPointString(pts[0]) + GetBezierString(pts[1], pts[2], pts[3]));
            for (int index = 1; index < nCurves; index++)
            {
                stSpline.Append(GetBezierString(pts[index * 3 + 1], pts[index * 3 + 2], pts[index * 3 + 3]));
            }

            if (makePath)
            {
                path.AppendLine(stSpline.ToString());
            }
            else
            {
                OutputLineString(stSpline.ToString() + "S", pen);
            }
            lastPoint = pts[pts.Length - 1];
        }


        public void DrawArc(RectangleF rect, float startAngle, float sweepAngle)
        {
            if (assembleData) return;

            var pts = ConvertArcToBezierPoints(rect, startAngle, sweepAngle);
            int nCurves = (pts.Length - 1) / 3; 

            DrawBezier(pts[0], pts[1], pts[2], pts[3], null);
            for (int index = 1; index < nCurves; index++)
            {
                DrawBezierTo(pts[index * 3 + 1], pts[index * 3 + 2], pts[index * 3 + 3], null);
            }
        }

        public void DrawEllipse(RectangleF rect, object pen)
        {
            if (assembleData) return;
            if (makePath) throw new Exception();    // !!!
            OutputLineString(GetEllipseString(rect) + "S", pen);
        }

        public void FillEllipse(RectangleF rect, object brush)
        {
            bool isTransparent;
            bool needRestore = SetBrush(brush, rect, out isTransparent, true);
            if (assembleData) return;
            if (makePath) throw new Exception();    // !!!
            if (brush != null && !isTransparent)
            {
                pageStream.WriteLine(GetEllipseString(rect) + "f");
            }
            if (needRestore) pageStream.WriteLine("Q");
        }

        public void DrawPie(RectangleF rect, float startAngle, float sweepAngle)
        {
            if (assembleData) return;

            var pts = ConvertArcToBezierPoints(rect, startAngle, sweepAngle);
            int nCurves = (pts.Length - 1) / 3;

            float cx = rect.X + rect.Width / 2;
            float cy = rect.Y + rect.Height / 2;

            DrawBezier(pts[0], pts[1], pts[2], pts[3], null);
            for (int index = 1; index < nCurves; index++)
            {
                DrawBezierTo(pts[index * 3 + 1], pts[index * 3 + 2], pts[index * 3 + 3], null);
            }
            DrawLineTo(new PointF(cx, cy), null);
            DrawLineTo(pts[0], null);
        }


        public void DrawImage(Image img, RectangleF rect)
        {
            StiImage image = new StiImage();
            image.ClientRectangle = RectangleD.CreateFromRectangle(rect);
            image.PutImageToDraw(img);
            //image.HorAlignment = StiHorAlignment.Center;
            //image.VertAlignment = StiVertAlignment.Center;
            image.Smoothing = true;
            image.Stretch = true;
            image.Page = pdfService.report.RenderedPages[0] ?? new StiPage(pdfService.report);

            if (assembleData)
            {
                pdfService.StoreImageDataForGeom(image);
                return;
            }

            Stimulsoft.Report.Export.StiPdfExportService.StiPdfData pp = new StiPdfExportService.StiPdfData();
            pp.Component = image;
            pp.Width = rect.Width;
            pp.Height = rect.Height;

            pageStream.WriteLine("q");
            pdfService.PushColorToStack();
            pageStream.WriteLine("1 0 0 1 {0} {1} cm", ConvertToString(rect.X), ConvertToString(rect.Y + rect.Height));
            pageStream.WriteLine("1 0 0 -1 0 0 cm");
            //pageStream.WriteLine("1.39 0 0 1.39 0 0 cm");

            pdfService.RenderImage(pp, 100f, true);

            pageStream.WriteLine("Q");
            pdfService.PopColorFromStack();
        }

        public void DrawString(string st, Font font, StiBrush brush, RectangleF rect, StringFormat sf, bool allowHtmlTags = false)
        {
            #region Create StiText component
            StiText txt = new StiText();
            txt.ClientRectangle = pdfService.report.Unit.ConvertFromHInches(RectangleD.CreateFromRectangle(rect));
            txt.Text = st;
            txt.Font = font;
            txt.TextBrush = brush;
            if (sf.Alignment == StringAlignment.Center) txt.HorAlignment = StiTextHorAlignment.Center;
            if (sf.Alignment == StringAlignment.Far) txt.HorAlignment = StiTextHorAlignment.Right;
            if (sf.LineAlignment == StringAlignment.Center) txt.VertAlignment = StiVertAlignment.Center;
            if (sf.LineAlignment == StringAlignment.Far) txt.VertAlignment = StiVertAlignment.Bottom;
            if ((sf.FormatFlags & StringFormatFlags.NoWrap) == 0) txt.WordWrap = true;
            txt.TextQuality = StiTextQuality.Standard;
            if (allowHtmlTags)
            {
                txt.TextQuality = StiTextQuality.Wysiwyg;
                txt.AllowHtmlTags = true;
            }
            txt.Page = pdfService.report.RenderedPages[0] ?? new StiPage(pdfService.report);
            #endregion

            if (assembleData)
            {
                if ((brush is StiGradientBrush) || (brush is StiGlareBrush) || (brush is StiHatchBrush) || (brush is StiGlassBrush))
                {
                    pdfService.StoreShadingData1(brush, pageNumber);
                    pdfService.StoreHatchData(brush);
                }

                if (allowHtmlTags)
                {
                    pdfService.StoreWysiwygSymbols(txt);
                    return;
                }

                if (font != null)
                {
                    int fnt = pdfService.pdfFont.GetFontNumber(font);
                }
                bool useRightToLeft = (sf.FormatFlags & StringFormatFlags.DirectionRightToLeft) > 0;
                //StringBuilder sb = new StringBuilder(st);
                //sb = pdfService.bidi.Convert(sb, useRightToLeft);
                var sb = new StringBuilder(StiBidirectionalConvert2.ConvertString(st, useRightToLeft));
                pdfService.pdfFont.StoreUnicodeSymbolsInMap(sb);
                return;                
            }

            Stimulsoft.Report.Export.StiPdfExportService.StiPdfData pp = new StiPdfExportService.StiPdfData();
            pp.Component = txt;
            pp.Width = rect.Width * hiToTwips;
            pp.Height = rect.Height * hiToTwips;

            pageStream.WriteLine("q");
            pdfService.PushColorToStack(); 
            pageStream.WriteLine("1 0 0 1 {0} {1} cm", ConvertToString(rect.X), ConvertToString(rect.Y + rect.Height));
            //pageStream.WriteLine("1 0 0 -1 0 0 cm");
            //pageStream.WriteLine("1.39 0 0 1.39 0 0 cm");
            pageStream.WriteLine("1.39 0 0 -1.39 0 0 cm");

            if (allowHtmlTags)
            {
                pdfService.RenderText2(pp);
            }
            else
            {
                pdfService.RenderTextFont(pp);
                pdfService.RenderText(pp, basePoint);
            }

            pageStream.WriteLine("Q");
            pdfService.PopColorFromStack();
        }

        public void SaveState()
        {
            if (assembleData) return;
            pageStream.WriteLine("q");
            pdfService.PushColorToStack();
            if (matrixCache.Count > 0) matrixCache.Push(matrixCache.Peek().Clone());
        }

        public void RestoreState()
        {
            if (assembleData) return;
            pageStream.WriteLine("Q");
            pdfService.PopColorFromStack();
            if (matrixCache.Count > 0) matrixCache.Pop();
        }

        public void TranslateTransform(float x, float y)
        {
            if (assembleData) return;
            pageStream.WriteLine("1 0 0 1 {0} {1} cm",
                ConvertToString(x),
                ConvertToString(y));
            if (matrixCache.Count > 0) matrixCache.Peek().Translate(x, y);
        }

        public void RotateTransform(float angle)
        {
            if (assembleData) return;
            double AngleInRadians = angle * Math.PI / 180f;
            pageStream.WriteLine("{0} {1} {2} {3} 0 0 cm",
                ConvertToString(Math.Cos(AngleInRadians)),
                ConvertToString(Math.Sin(AngleInRadians)),
                ConvertToString(-Math.Sin(AngleInRadians)),
                ConvertToString(Math.Cos(AngleInRadians)));
            if (matrixCache.Count > 0) matrixCache.Peek().Rotate(angle);
        }

        public void RotateTransform(float angle, float x, float y)
        {
            if (assembleData) return;
            double AngleInRadians = angle * Math.PI / 180f;
            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} cm",
                ConvertToString(Math.Cos(AngleInRadians)),
                ConvertToString(Math.Sin(AngleInRadians)),
                ConvertToString(-Math.Sin(AngleInRadians)),
                ConvertToString(Math.Cos(AngleInRadians)),
                ConvertToString(x),
                ConvertToString(y));
            if (matrixCache.Count > 0) matrixCache.Peek().RotateAt(angle, new PointF(x, y));
        }

        public void ScaleTransform(float scaleX, float scaleY)
        {
            if (assembleData) return;
            pageStream.WriteLine("{0} 0 0 {1} 0 0 cm",
                ConvertToString(scaleX),
                ConvertToString(scaleY));
            if (matrixCache.Count > 0) matrixCache.Peek().Scale(scaleX, scaleY);
        }

        public void SetClipPath()
        {
            if (assembleData) return;
            pageStream.Write(path.ToString());
            pageStream.WriteLine("W n");
        }
        public void SetClip(RectangleF rect)
        {
            if (assembleData) return;
            pageStream.WriteLine(GetRectString(rect) + "W n");
        }
        #endregion

        #region GDI specific commands
        public void DrawArc(RectangleF rect, PointF p1, PointF p2, Pen pen)
        {
            if (assembleData) return;
            //PointF center = new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            //float angle1 = (float)(Math.Atan2((p1.Y - center.Y), p1.X - center.X) * 180 / Math.PI);
            //float angle2 = (float)(Math.Atan2((p2.Y - center.Y), p2.X - center.X) * 180 / Math.PI);
            //while (angle2 > angle1) angle1 += 360;
            //float startAngle = angle2;
            //float sweepAngle = angle1 - angle2;

            //if (makePath)
            //{
            //    path.AddArc(rect, startAngle, sweepAngle);
            //}
            //else
            //{
            //    if (pen != null)
            //    {
            //        using (Graphics gr = Graphics.FromImage(Bitmap))
            //        {

            //            gr.DrawArc(pen, rect, startAngle, sweepAngle);
            //        }
            //    }
            //}
            lastPoint = p2;
        }

        public void DrawText(PointF basePoint, string text, int[] charsOffset, Font font, Color textColor, float angle, EmfTextAlignmentMode textAlign)
        {
            if (assembleData)
            {

                return;
            }

            if (makePath) throw new Exception();    // !!!
            //using (Graphics gr = Graphics.FromImage(Bitmap))
            //{

            //    // charsOffset  !!!

            //    //textAlign
            //    if (textAlign == EmfTextAlignmentMode.TA_BASELINE)
            //    {
            //        basePoint.Y -= (float)(font.SizeInPoints * 1.2);
            //    }
            //    if (textAlign == EmfTextAlignmentMode.TA_BOTTOM)
            //    {
            //        basePoint.Y -= font.SizeInPoints;   // ???
            //    }

            //    basePoint.X -= (float)(font.SizeInPoints * 0.15);

            //    PointF point = basePoint;
            //    if (angle != 0)
            //    {
            //        point.X = 0;
            //        point.Y = 0;
            //        gr.TranslateTransform(basePoint.X, basePoint.Y);
            //        gr.RotateTransform(-angle);
            //    }
            //    gr.DrawString(text, font, new SolidBrush(textColor), point);
            //    if (angle != 0)
            //    {
            //        gr.RotateTransform(angle);
            //        gr.TranslateTransform(-basePoint.X, -basePoint.Y);
            //    }
            //}
        }

        public void SetPixel(PointF point, Color color)
        {
            if (assembleData) return;
            //if (makePath) throw new Exception();    // !!!
            //using (Graphics gr = Graphics.FromImage(Bitmap))
            //{                
            //    gr.FillRectangle(new SolidBrush(color), point.X, point.Y, 1, 1);
            //}
        }

        public SizeF MeasureString(string st, Font font)
        {
            if (assembleData) return new SizeF();
            using (Bitmap bmp = new Bitmap(1, 1))
            {
                Graphics g = Graphics.FromImage(bmp);
                var size = g.MeasureString(st, font);
                g.Dispose();
                return size;
            }
        }
        #endregion

        public StiPdfGeomWriter()
        {
        }

        public StiPdfGeomWriter(StreamWriter stream, StiPdfExportService service, bool assembleData = false, bool allowThinLines = false)
        {
            pageStream = stream;
            //this.baseX = x;
            //this.baseY = y;
            this.pdfService = service;
            this.assembleData = assembleData;
            this.allowThinLines = allowThinLines;

            matrixCache = new Stack<Matrix>();

            currentNumberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            currentNumberFormat.NumberDecimalSeparator = ".";
        }

    }
}
