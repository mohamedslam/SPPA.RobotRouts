#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using Stimulsoft.Drawing.Text;
using System;
using Stimulsoft.Drawing.Drawing2D;
using System.Drawing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Linq;
using Stimulsoft.Drawing.Imaging;
using System.Numerics;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.Drawing
{
    public class Graphics : IDisposable
    {
        public static GraphicsEngine GraphicsEngine { get; set; } = GraphicsEngine.ImageSharp;

        public delegate bool EnumerateMetafileProc(System.Drawing.Imaging.EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, System.Drawing.Imaging.PlayRecordCallback callbackData);

        internal System.Drawing.Graphics netGraphics;
        private Image canvasImage;
        
        private Region clip;
        public Region Clip
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return new Region(netGraphics.Clip.GetBounds(netGraphics));
                else
                    return clip;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netGraphics.Clip = value.netRegion;
                else
                {
                    clip = value;
                    var path = clip.path.sixPathBuilder.Build();
                    if (Transform != null) path = path.Transform(Transform.matrix);

                    //check for avoid "Crop rectangle should be smaller than the source bounds."
                    var rectf = path.Bounds;
                    if (rectf.Left < 0 || rectf.Right > canvasImage.Width || rectf.Top < 0 || rectf.Bottom > canvasImage.Height)
                    {
                        float x1 = Math.Min(Math.Max(0, rectf.Left), canvasImage.Width - 1);
                        float x2 = Math.Max(Math.Min(rectf.Right, canvasImage.Width), 1);
                        float y1 = Math.Min(Math.Max(0, rectf.Top), canvasImage.Height - 1);
                        float y2 = Math.Max(Math.Min(rectf.Bottom, canvasImage.Height), 1);

                        var newRect = new RectangleF(x1, y1, x2 - x1, y2 - y1);
                        var newRegion = new Region(newRect);
                        path = newRegion.path.sixPathBuilder.Build();
                    }

                    canvasImage.ClipPath = new SixLabors.ImageSharp.Drawing.PolygonClipper.ClippablePath(path, SixLabors.ImageSharp.Drawing.PolygonClipper.ClippingType.Subject);
                }
            }
        }

        private SixLabors.ImageSharp.Drawing.Processing.DrawingOptions drawingOptions;
        private SixLabors.ImageSharp.Drawing.Processing.DrawingOptions DrawingOptions
        {
            get
            {
                if (drawingOptions == null)
                {
                    drawingOptions = new SixLabors.ImageSharp.Drawing.Processing.DrawingOptions();
                    //drawingOptions.GraphicsOptions.Antialias = SmoothingMode == System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    drawingOptions.GraphicsOptions.Antialias = true;
                    drawingOptions.Transform = Transform.matrix;
                }
                return drawingOptions;
            }
        }

        private System.Drawing.Drawing2D.CompositingMode compositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        public System.Drawing.Drawing2D.CompositingMode CompositingMode
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netGraphics.CompositingMode;
                else
                    return compositingMode;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netGraphics.CompositingMode = (System.Drawing.Drawing2D.CompositingMode)value;
                else
                    compositingMode = value;
            }
        }

        private System.Drawing.Drawing2D.CompositingQuality compositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
        public System.Drawing.Drawing2D.CompositingQuality CompositingQuality
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netGraphics.CompositingQuality;
                else
                    return compositingQuality;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netGraphics.CompositingQuality = (System.Drawing.Drawing2D.CompositingQuality)value;
                else
                    compositingQuality = value;
            }
        }

        private System.Drawing.Drawing2D.InterpolationMode interpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
        public System.Drawing.Drawing2D.InterpolationMode InterpolationMode
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netGraphics.InterpolationMode;
                else
                    return interpolationMode;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netGraphics.InterpolationMode = (System.Drawing.Drawing2D.InterpolationMode)value;
                else
                    interpolationMode = value;
            }
        }

        private GraphicsUnit pageUnit = GraphicsUnit.Pixel;
        public GraphicsUnit PageUnit
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netGraphics.PageUnit;
                else
                    return pageUnit;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netGraphics.PageUnit = value;
                else
                    pageUnit = value;
            }
        }

        private System.Drawing.Drawing2D.PixelOffsetMode pixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Default;
        public System.Drawing.Drawing2D.PixelOffsetMode PixelOffsetMode
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netGraphics.PixelOffsetMode;
                else
                    return pixelOffsetMode;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netGraphics.PixelOffsetMode = (System.Drawing.Drawing2D.PixelOffsetMode)value;
                else
                    pixelOffsetMode = value;
            }
        }

        private System.Drawing.Drawing2D.SmoothingMode smoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        public System.Drawing.Drawing2D.SmoothingMode SmoothingMode
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netGraphics.SmoothingMode;
                else
                    return smoothingMode;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netGraphics.SmoothingMode = (System.Drawing.Drawing2D.SmoothingMode)value;
                else
                {
                    smoothingMode = value;
                    ResetDrawingOptions();
                }
            }
        }

        private System.Drawing.Text.TextRenderingHint textRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        public System.Drawing.Text.TextRenderingHint TextRenderingHint
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netGraphics.TextRenderingHint;
                else
                    return textRenderingHint;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netGraphics.TextRenderingHint = (System.Drawing.Text.TextRenderingHint)value;
                else
                    textRenderingHint = value;
            }
        }

        private Matrix transform = new Matrix();
        public Matrix Transform
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                   return netGraphics.Transform;
                else
                    return transform;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netGraphics.Transform = value.netMatrix;
                else
                    transform = value;

                ResetDrawingOptions();
            }
        }

        private float pageScale;
        public float PageScale
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netGraphics.PageScale;
                else
                    return pageScale;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netGraphics.PageScale = value;
                else
                    pageScale = value;
            }
        }

        private float dpiX = 96;
        public float DpiX
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netGraphics.DpiX;
                else
                    return dpiX;
            }
        }

        private float dpiY = 96;
        public float DpiY
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netGraphics.DpiY;
                else
                    return dpiY;
            }
        }

        public RectangleF ClipBounds
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netGraphics.ClipBounds;
                else
                    return clip.GetBounds(this);
            }
        }

        public void Dispose()
        {
        }

        public void Clear(Color color)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.Clear(color);
            else
            {
                var sixColor = SixLabors.ImageSharp.Color.FromRgba(color.R, color.G, color.B, color.A);
                canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.Clear(sixColor));
            }
        }

        private void ResetDrawingOptions()
        {
            drawingOptions = null;
        }

        public void EnumerateMetafile(Metafile metafile, Point m_destPoint, EnumerateMetafileProc m_delegate)
        {
        }

        #region Draw
        public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawArc(pen.netPen, rect, startAngle, sweepAngle);
            else
                DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawArc(pen.netPen, x, y, width, height, startAngle, sweepAngle);
            else
            {
                var arcPath = new GraphicsPath();
                arcPath.AddArc(x, y, width, height, startAngle, sweepAngle);
                DrawPath(pen, arcPath);
            }
        }

        public void DrawCurve(Pen pen, PointF[] points, float tension)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawCurve(pen.netPen, points, tension);
            else
            {
                if (points.Length < 3)
                {
                    DrawLines(pen, points);
                }
                else
                {
                    var curvePath = new GraphicsPath();
                    curvePath.AddCurve(points, tension);
                    DrawPath(pen, curvePath);
                }
            }
        }

        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawEllipse(pen.netPen, rect);
            else
                DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawEllipse(Pen pen, RectangleF rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawEllipse(pen.netPen, rect);
            else
                DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawEllipse(pen.netPen, x, y, width, height);
            else
                DrawEllipse(pen, (float)x, (float)y, (float)width, (float)height);
        }

        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawEllipse(pen.netPen, x, y, width, height);
            else
            {
                var ellipsePolygon = new SixLabors.ImageSharp.Drawing.EllipsePolygon(x + width / 2, y + height / 2, width, height);
                canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.Draw(o, pen.SixPen, ellipsePolygon));
            }
        }

        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawLine(pen.netPen, pt1, pt2);
            else
                DrawLines(pen, new Point[] { pt1, pt2 });
        }

        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawLine(pen.netPen, pt1, pt2);
            else
                DrawLines(pen, new PointF[] { pt1, pt2 });
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawLine(pen.netPen, x1, y1, x2, y2);
            else
                DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawLine(pen.netPen, x1, y1, x2, y2);
            else
                DrawLines(pen, new PointF[] { new PointF(x1, y1), new PointF(x2, y2) });
        }

        public void DrawLines(Pen pen, Point[] points)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawLines(pen.netPen, points);
            else
                DrawLines(pen, points.Select(point => new PointF(point.X, point.Y)).ToArray());
        }

        public void DrawLines(Pen pen, PointF[] points)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawLines(pen.netPen, points);
            else
            {
                var sixPoints = points.Select(point => new SixLabors.ImageSharp.PointF(point.X, point.Y)).ToArray();
                canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.DrawLines(o, pen.SixPen, sixPoints));
            }
        }

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawPath(pen.netPen, path.netPath);
            else
            {
                var sixPath = path.sixPathBuilder.Build();
                canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.Draw(o, pen.SixPen, sixPath));
            }
        }

        public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawPie(pen.netPen, rect, startAngle, sweepAngle);
            else
            {
                var piePath = new GraphicsPath();
                piePath.AddPie(rect, startAngle, sweepAngle);

                DrawPath(pen, piePath);
            }
        }

        public void DrawPolygon(Pen pen, PointF[] points)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawPolygon(pen.netPen, points);
            else
            {
                var sixPoints = points.Select(point => new SixLabors.ImageSharp.PointF(point.X, point.Y)).ToArray();
                canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.DrawPolygon(o, pen.SixPen, sixPoints));
            }
        }

        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawRectangle(pen.netPen, rect);
            else
                DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawRectangle(pen.netPen, x, y, width, height);
            else
                DrawRectangle(pen, (float)x, (float)y, (float)width, (float)height);
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawRectangle(pen.netPen, x, y, width, height);
            else
            {
                var rectangularPolygon = new SixLabors.ImageSharp.Drawing.RectangularPolygon(x, y, width, height);
                canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.Draw(o, pen.SixPen, rectangularPolygon));
            }
        }
        #endregion

        #region Fill
        public void FillEllipse(Brush brush, Rectangle rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillEllipse(brush.NetBrush, rect);
            else
                FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillEllipse(Brush brush, RectangleF rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillEllipse(brush.NetBrush, rect);
            else
                FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillEllipse(Brush brush, int x, int y, int width, int height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillEllipse(brush.NetBrush, x, y, width, height);
            else
                FillEllipse(brush, (float)x, (float)y, (float)width, (float)height);
        }

        public void FillEllipse(Brush brush, float x, float y, float width, float height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillEllipse(brush.NetBrush, x, y, width, height);
            else
            {
                var ellipsePoligon = new SixLabors.ImageSharp.Drawing.EllipsePolygon(x + width / 2, y + height / 2, width, height);
                canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.Fill(o, brush.SixBrush, ellipsePoligon));
            }
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillPath(brush.NetBrush, path.netPath);
            else
            {
                var sixPath = path.sixPathBuilder.Build();
                canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.Fill(o, brush.SixBrush, sixPath));
            }
        }

        public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillPie(brush.NetBrush, x, y, width, height, startAngle, sweepAngle);
            else
            {
                var piePath = new GraphicsPath();
                piePath.AddPie(x, y, width, height, startAngle, sweepAngle);

                FillPath(brush, piePath);
            }
        }

        public void FillPolygon(Brush brush, PointF[] points)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillPolygon(brush.NetBrush, points);
            else
            {
                var sixPoints = points.Select(point => new SixLabors.ImageSharp.PointF(point.X, point.Y)).ToArray();
                canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.FillPolygon(o, brush.SixBrush, sixPoints));
            }
        }

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillRectangle(brush.NetBrush, rect);
            else
                FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void FillRectangle(Brush brush, Rectangle rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillRectangle(brush.NetBrush, rect);
            else
                FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillRectangle(brush.NetBrush, x, y, width, height);
            else
                FillRectangle(brush, (float)x, (float)y, (float)width, (float)height);
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillRectangle(brush.NetBrush, x, y, width, height);
            else
            {
                var rectangularPolygon = new SixLabors.ImageSharp.Drawing.RectangularPolygon(x, y, width, height);
                var dd = DrawingOptions;
                canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.Fill(dd, brush.SixBrush, rectangularPolygon));
            }
        }

        public void FillRegion(Brush brush, Region region)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.FillRegion(brush.NetBrush, region.netRegion);
            else
                FillPath(brush, region.path);
        }
        #endregion

        #region Image
        public void DrawImage(Image image, Rectangle rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImage(image.netImage, rect);
            else
                DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawImage(Image image, RectangleF rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImage(image.netImage, rect);
            else
                DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawImage(Image image, Point point)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImage(image.netImage, point);
            else
                DrawImage(image, point.X, point.Y);
        }

        public void DrawImage(Image image, float x, float y)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImage(image.netImage, x, y);
            else
                DrawImage(image, (int)x, (int)y);
        }

        public void DrawImage(Image image, int x, int y)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImage(image.netImage, x, y);
            else
            {
                var location = new SixLabors.ImageSharp.Point(x, y);
                location.Offset((int)DrawingOptions.Transform.Translation.X, (int)DrawingOptions.Transform.Translation.Y);
                if (location.X > canvasImage.Width || location.Y > canvasImage.Height || location.X + image.Width < 0 || location.Y + image.Height < 0) return;  //"Cannot draw image because source image does not overlap the target image."
                image.RenderDrawingOperations();
                this.canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.DrawImage(image.sixImage, location, 1));
            }
        }

        public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImage(image.netImage, destRect, srcRect, (System.Drawing.GraphicsUnit)srcUnit);
            else
                DrawImage(image, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
        }

        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImage(image.netImage, destRect, srcRect, (System.Drawing.GraphicsUnit)srcUnit);
            else
                DrawImage(image, (int)destRect.X, (int)destRect.Y, (int)destRect.Width, (int)destRect.Height, (int)srcRect.X, (int)srcRect.Y, (int)srcRect.Width, (int)srcRect.Height);
        }

        public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImage(image.netImage, x, y, srcRect, (System.Drawing.GraphicsUnit)srcUnit);
            else
            {
                var location = new SixLabors.ImageSharp.Point(x, y);
                location.Offset((int)DrawingOptions.Transform.Translation.X, (int)DrawingOptions.Transform.Translation.Y);
                if (location.X > canvasImage.Width || location.Y > canvasImage.Height || location.X + srcRect.Width < 0 || location.Y + srcRect.Height < 0) return;  //"Cannot draw image because source image does not overlap the target image."
                image.RenderDrawingOperations();
                var cropRect = new SixLabors.ImageSharp.Rectangle(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);
                var resizeImage = image.sixImage.Clone(i => i.Crop(cropRect));
                this.canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.DrawImage(resizeImage, location, 1));
            }
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImage(image.netImage, destRect, srcX, srcY, srcWidth, srcHeight, (System.Drawing.GraphicsUnit)srcUnit);
            else
                DrawImage(image, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight);
        }

        private void DrawImage(Image image, int destX, int destY, int destWidth, int destHeight, int srcX, int srcY, int srcWidth, int srcHeight)
        {
            var location = new SixLabors.ImageSharp.Point(destX, destY);
            location.Offset((int)DrawingOptions.Transform.Translation.X, (int)DrawingOptions.Transform.Translation.Y);
            if (location.X > canvasImage.Width || location.Y > canvasImage.Height || location.X + destWidth < 0 || location.Y + destHeight < 0) return;  //"Cannot draw image because source image does not overlap the target image."
            image.RenderDrawingOperations();
            var cropRect = new SixLabors.ImageSharp.Rectangle(srcX, srcY, srcWidth, srcHeight);
            var resizeImage = image.sixImage.Clone(x => x.Crop(cropRect).Resize(destWidth, destHeight));
            this.canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.DrawImage(resizeImage, location, 1));
        }

        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImage(image.netImage, x, y, width, height);
            else
                DrawImage(image, (int)x, (int)y, (int)width, (int)height);
        }

        public void DrawImage(Image image, int x, int y, int width, int height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImage(image.netImage, x, y, width, height);
            else
            {
                var location = new SixLabors.ImageSharp.Point(x, y);
                location.Offset((int)DrawingOptions.Transform.Translation.X, (int)DrawingOptions.Transform.Translation.Y);
                if (location.X > canvasImage.Width || location.Y > canvasImage.Height || location.X + width < 0 || location.Y + height < 0) return;  //"Cannot draw image because source image does not overlap the target image."
                image.RenderDrawingOperations();
                var resizeImage = image.sixImage.Clone(i => i.Resize(width, height));
                this.canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.DrawImage(resizeImage, location, 1));
            }
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
               netGraphics.DrawImage(image.netImage, destRect, srcX, srcY, srcWidth, srcHeight, (System.Drawing.GraphicsUnit)srcUnit, imageAttr);
            else
                DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, object callback, IntPtr callbackData)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
               netGraphics.DrawImage(image.netImage, destRect, srcX, srcY, srcWidth, srcHeight, (System.Drawing.GraphicsUnit)srcUnit, imageAttrs, null, callbackData);
            else
                DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit);
        }

        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImageUnscaled(image.netImage, rect);
            else
                DrawImage(image, rect.X, rect.Y);
        }

        public void DrawImageUnscaled(Image image, int x, int y)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImageUnscaled(image.netImage, x, y);
            else
                DrawImage(image, x, y);
        }

        public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImageUnscaled(image.netImage, x, y, width, height);
            else
                DrawImage(image, x, y);
        }

        public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawImageUnscaledAndClipped(image.netImage, rect);
            else
            {
                int width = (image.Width > rect.Width) ? rect.Width : image.Width;
                int height = (image.Height > rect.Height) ? rect.Height : image.Height;

                DrawImageUnscaled(image, rect.X, rect.Y, width, height);
            }
        }
        #endregion

        #region Text
        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawString(s, font.netFont, brush.NetBrush, layoutRectangle);
            else
                DrawString(s, font, brush, layoutRectangle, null);
        }

        public void DrawString(string s, Font font, Brush brush, PointF point)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawString(s, font.netFont, brush.NetBrush, point);
            else
                DrawString(s, font, brush, new RectangleF(point.X, point.Y, float.MaxValue, float.MaxValue), null);
        }

        public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawString(s, font.netFont, brush.NetBrush, point, format.netFormat);
            else
                DrawString(s, font, brush, new RectangleF(point.X, point.Y, float.MaxValue, float.MaxValue), format);
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawString(s, font.netFont, brush.NetBrush, x, y);
            else
                DrawString(s, font, brush, new RectangleF(x, y, float.MaxValue, float.MaxValue), null);
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawString(s, font.netFont, brush.NetBrush, x, y, format.netFormat);
            else
                DrawString(s, font, brush, new RectangleF(x, y, float.MaxValue, float.MaxValue), format);
        }

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.DrawString(s, font.netFont, brush.NetBrush, layoutRectangle, format.netFormat);
            else
            {
                format = format ?? StringFormat.GenericDefault;
                var location = new SixLabors.ImageSharp.PointF(layoutRectangle.Location.X, layoutRectangle.Location.Y);
                if (layoutRectangle.Width < float.MaxValue)
                {
                    if (format.Alignment == StringAlignment.Center)
                    {
                        var size = MeasureString(s, font, layoutRectangle, format);
                        location.X += (layoutRectangle.Width - size.Width) / 2;
                        //textOptions.TextAlignment = SixLabors.Fonts.TextAlignment.Center;
                        //textOptions.HorizontalAlignment = SixLabors.Fonts.HorizontalAlignment.Center;
                    }
                    if (format.Alignment == StringAlignment.Far)
                    {
                        var size = MeasureString(s, font, layoutRectangle, format);
                        location.X += layoutRectangle.Width - size.Width;
                        //textOptions.TextAlignment = SixLabors.Fonts.TextAlignment.End;
                        //textOptions.HorizontalAlignment = SixLabors.Fonts.HorizontalAlignment.Right;
                    }
                }

                var textOptions = new SixLabors.Fonts.TextOptions(font.sixFont);
                textOptions.Dpi = 96;
                textOptions.Origin = location;
                textOptions.WrappingLength = layoutRectangle.Width;

                canvasImage.AddDrawingOperation(DrawingOptions, (x, o) => x.DrawText(o, textOptions, s, brush.SixBrush, null));
            }
        }

        public SizeF MeasureString(string text, Font font)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netGraphics.MeasureString(text, font.netFont);
            else
                return MeasureString(text, font, SizeF.Empty);
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netGraphics.MeasureString(text, font.netFont, layoutArea);
            else
            {
                var rect = new RectangleF(0, 0, layoutArea.Width, layoutArea.Height);
                return MeasureString(text, font, rect, null);
            }
        }

        public SizeF MeasureString(string text, Font font, int width)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netGraphics.MeasureString(text, font.netFont, width);
            else
            {
                var rect = new RectangleF(0, 0, width, Int32.MaxValue);
                return MeasureString(text, font, rect, null);
            }
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netGraphics.MeasureString(text, font.netFont, layoutArea, stringFormat.netFormat);
            else
            {
                var rect = new RectangleF(0, 0, layoutArea.Width, layoutArea.Height);
                return MeasureString(text, font, rect, stringFormat);
            }
        }

        public SizeF MeasureString(string text, Font font, int width, StringFormat stringFormat)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netGraphics.MeasureString(text, font.netFont, width, stringFormat.netFormat);
            else
            {
                var rect = new RectangleF(0, 0, width, Int32.MaxValue);
                return MeasureString(text, font, rect, stringFormat);
            }
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netGraphics.MeasureString(text, font.netFont, layoutArea, stringFormat.netFormat, out charactersFitted, out linesFilled);
            else
            {
                charactersFitted = 0;
                linesFilled = 0;
                return MeasureString(text, font, layoutArea, stringFormat);
            }
        }

        private SizeF MeasureString(string text, Font font, RectangleF layoutRectangle, StringFormat format)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netGraphics.MeasureString(text, font.netFont, layoutRectangle.Location, format.netFormat);
            else
            {
                if (text == null) return SizeF.Empty;

                //library features - leading empty lines are ignored, so we add one space
                if ((text.Length > 2) && (text[0] == 0x0A || (text[0] == 0x0D && text[1] == 0x0A)))
                {
                    text = " " + text;
                }

                var upScale = 1f;
                if (font.Size < 10)
                {
                    upScale = 1000 / font.Size;
                    font = new Font(font, 1000);
                }

                var textOptions = new SixLabors.Fonts.TextOptions(font.sixFont);
                textOptions.Dpi = 96;
                textOptions.WrappingLength = layoutRectangle.Width * upScale;
                var rect = SixLabors.Fonts.TextMeasurer.Measure(text, textOptions);

                var boundingBox = RectangleF.FromLTRB((layoutRectangle.X * upScale) + rect.Left, rect.Top, (layoutRectangle.Left * upScale) + rect.Right, rect.Bottom);
                return new SizeF(boundingBox.Width / upScale, (Math.Max(font.Height, boundingBox.Height/* - font.sixFont.FontMetrics.UnderlinePosition*/)) / upScale);
            }
        }

        public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRectangle, StringFormat format)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
            {
                return
                    netGraphics.MeasureCharacterRanges(text, font.netFont, layoutRectangle, format.netFormat)
                    .Select(region => region.GetBounds(netGraphics))
                    .Select(rect => new Region(rect)).ToArray();
            }
            else
                return MeasureCharacterRanges2(text, font, layoutRectangle, format);
        }

        private Region[] MeasureCharacterRanges2(string text, Font font, RectangleF layoutRectangle, StringFormat format)
        {
            if ((text == null) || (text.Length == 0))
                return new Region[0];

            var regions = new List<Region>();
            var textOptions = new SixLabors.Fonts.TextOptions(font.sixFont);
            textOptions.Dpi = 96;
            if (format.Alignment == StringAlignment.Center) textOptions.TextAlignment = SixLabors.Fonts.TextAlignment.Center;
            if (format.Alignment == StringAlignment.Far) textOptions.TextAlignment = SixLabors.Fonts.TextAlignment.End;

            SixLabors.Fonts.GlyphBounds[] characterBoundsAll = null;
            SixLabors.Fonts.TextMeasurer.TryMeasureCharacterBounds(text.AsSpan(), textOptions, out characterBoundsAll);

            textOptions.WrappingLength = layoutRectangle.Width;

            SixLabors.Fonts.GlyphBounds[] characterBounds;
            SixLabors.Fonts.TextMeasurer.TryMeasureCharacterBounds(text.AsSpan(), textOptions, out characterBounds);
            SixLabors.Fonts.FontRectangle?[] charactersRects = new SixLabors.Fonts.FontRectangle?[text.Length];

            float lineHeight = (float)(textOptions.Font.FontMetrics.LineHeight / (double)textOptions.Font.FontMetrics.UnitsPerEm * textOptions.Font.Size / 0.72 * 0.96);

            var index1 = 0;
            for (var index = 0; index < text.Length; index++)
            {
                if (characterBounds[index1].Codepoint != new SixLabors.Fonts.Unicode.CodePoint(text[index]))
                {
                    if ((format.FormatFlags & StringFormatFlags.MeasureTrailingSpaces) == StringFormatFlags.MeasureTrailingSpaces)
                    {
                        var deltaX = characterBoundsAll[index1].Bounds.X - characterBoundsAll[index - 1].Bounds.X;
                        var deltaY = characterBoundsAll[index1].Bounds.Y - characterBoundsAll[index - 1].Bounds.Y;
                        var heightCorrection1 = characterBoundsAll[index1].Bounds.Y;
                        var bounds1 = characterBounds[index1 - 1].Bounds;

                        charactersRects[index] = new SixLabors.Fonts.FontRectangle(bounds1.X + deltaX, bounds1.Y + deltaY - heightCorrection1, characterBoundsAll[index1].Bounds.Width, characterBoundsAll[index1].Bounds.Height + heightCorrection1);
                    }
                    else
                        charactersRects[index] = SixLabors.Fonts.FontRectangle.Empty;

                    index++;
                }

                var bounds = characterBounds[index1].Bounds;
                float lineY = (float)(Math.Truncate(bounds.Y / lineHeight) * lineHeight);
                
                if (index < charactersRects.Length)
                    charactersRects[index] = new SixLabors.Fonts.FontRectangle(bounds.X, lineY, bounds.Width, lineHeight);
                
                index1++;
            }

            foreach (var characterRange in format.measurableCharacterRanges)
            {
                var left = float.MaxValue;
                var right = float.MinValue;
                var top = float.MaxValue;
                var bottom = float.MinValue;

                for (var indexChar = characterRange.First; indexChar < characterRange.First + characterRange.Length; indexChar++)
                {
                    if (charactersRects[indexChar] != null)
                    {
                        var rect = (SixLabors.Fonts.FontRectangle)charactersRects[indexChar];
                        if (rect.Right < right)
                        {
                            var boundingBox2 = new RectangleF(layoutRectangle.X + left, layoutRectangle.Y + top, right - left, bottom - top);
                            regions.Add(new Region(boundingBox2));

                            left = float.MaxValue;
                            right = float.MinValue;
                            top = float.MaxValue;
                            bottom = float.MinValue;
                        }

                        left = Math.Min(left, rect.Left);
                        top = Math.Min(top, rect.Top);
                        right = Math.Max(right, rect.Right);
                        bottom = Math.Max(bottom, rect.Bottom);
                    }
                }

                var boundingBox = new RectangleF(layoutRectangle.X + left, layoutRectangle.Y + top, right - left, bottom - top);
                regions.Add(new Region(boundingBox));
            }

            return regions.ToArray();
        }
        #endregion

        #region Transform
        public GraphicsState Save()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return new GraphicsState
                {
                    netState = netGraphics.Save()
                };
            else
            {
                var state = new GraphicsState();
                state.transform = this.Transform.Clone();
                state.clipPath = canvasImage.ClipPath;
                state.smoothingMode = this.SmoothingMode;

                return state;
            }
        }

        public void Restore(GraphicsState state)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.Restore(state.netState);
            else
            {
                this.Transform = state.transform;
                canvasImage.ClipPath = state.clipPath;
                this.smoothingMode = state.smoothingMode;

                ResetDrawingOptions();
            }
        }

        public void TranslateTransform(int x, int y)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.TranslateTransform(x, y);
            else
                TranslateTransform(x, y, System.Drawing.Drawing2D.MatrixOrder.Prepend);
        }

        public void TranslateTransform(int x, int y, System.Drawing.Drawing2D.MatrixOrder order)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.TranslateTransform(x, y, (System.Drawing.Drawing2D.MatrixOrder)order);
            else
                TranslateTransform((float)x, (float)y, order);
        }

        public void TranslateTransform(float x, float y)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.TranslateTransform(x, y);
            else
                TranslateTransform(x, y, System.Drawing.Drawing2D.MatrixOrder.Prepend);
        }

        public void TranslateTransform(float x, float y, System.Drawing.Drawing2D.MatrixOrder order)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.TranslateTransform(x, y, (System.Drawing.Drawing2D.MatrixOrder)order);
            else
            {
                Transform.Translate(x, y, order);
                ResetDrawingOptions();
            }
        }

        public void RotateTransform(float angle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.RotateTransform(angle);
            else
                RotateTransform(angle, System.Drawing.Drawing2D.MatrixOrder.Prepend);
        }

        public void RotateTransform(float angle, System.Drawing.Drawing2D.MatrixOrder order)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.RotateTransform(angle, order);
            else
            {
                Transform.Rotate(angle, order);
                ResetDrawingOptions();
            }
        }

        public void ResetTransform()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.ResetTransform();
            else
            {
                Transform = new Matrix();
                ResetDrawingOptions();
            }
        }

        public void MultiplyTransform(Matrix matrix)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.MultiplyTransform(matrix.netMatrix);
            else
            {
                Transform.matrix *= matrix.matrix;
                ResetDrawingOptions();
            }
        }

        public void TransformPoints(System.Drawing.Drawing2D.CoordinateSpace page, System.Drawing.Drawing2D.CoordinateSpace device, PointF[] points)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.TransformPoints(page, device, points);
            else
            {
                Transform.TransformPoints(points);
                ResetDrawingOptions();
            }
                
        }

        public void ScaleTransform(float scaleX, float scaleY)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.ScaleTransform(scaleX, scaleY);
            else
            {
                Transform.Scale(scaleX, scaleY);
                ResetDrawingOptions();
            }                
        }

        public void SetClip(Region region, System.Drawing.Drawing2D.CombineMode intersect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.SetClip(region.netRegion, intersect);
            else
                Clip = region;
        }

        public void SetClip(Rectangle rect, System.Drawing.Drawing2D.CombineMode intersect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.SetClip(rect, intersect);
            SetClip(rect, intersect);
        }

        public void SetClip(RectangleF rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.SetClip(rect);
            else
                SetClip(new Region(rect), System.Drawing.Drawing2D.CombineMode.Replace);
        }

        public void SetClip(RectangleF rect, System.Drawing.Drawing2D.CombineMode intersect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.SetClip(rect, intersect);
            else
                SetClip(rect);
        }

        public void SetClip(GraphicsPath path)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.SetClip(path.netPath);
            else
                SetClip(path);
        }

        public void SetClip(GraphicsPath path, System.Drawing.Drawing2D.CombineMode intersect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.SetClip(path.netPath, intersect);
            else
                SetClip(new Region(path), intersect);
        }

        public void ResetClip()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.ResetClip();
            else
                canvasImage.ClipPath = null;
        }
        #endregion

        public IntPtr GetHdc()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netGraphics.GetHdc();            
            else
                throw new NotImplementedException();
        }

        public void ReleaseHdc()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.ReleaseHdc();
            else
                throw new NotImplementedException();
        }

        public void ReleaseHdc(IntPtr ptrGraphics)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netGraphics.ReleaseHdc(ptrGraphics);
            else
                throw new NotImplementedException();
        }

        public static Graphics FromHwnd(IntPtr handle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return System.Drawing.Graphics.FromHwnd(handle);
            else
                throw new NotImplementedException();
        }

        public static Graphics FromHdc(IntPtr hdc)
        {
            throw new NotImplementedException();
        }

        public static Graphics FromImage(Image image)
        {
            var g = new Graphics();
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                g.netGraphics = System.Drawing.Graphics.FromImage(image.netImage);
            else
                g.canvasImage = image;

            return g;
        }

        public static implicit operator System.Drawing.Graphics(Graphics graphics)
        {
            return graphics.netGraphics;
        }

        public static implicit operator Graphics(System.Drawing.Graphics netGraphics)
        {
            var graphics = new Graphics();
            graphics.netGraphics = netGraphics;

            return graphics;
        }
    }
}
