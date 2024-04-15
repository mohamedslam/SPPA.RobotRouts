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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace Stimulsoft.Drawing.Drawing2D
{
    public sealed class GraphicsPath : ICloneable, IDisposable
    {
        private Matrix transformMatrix;

        internal SixLabors.ImageSharp.Drawing.PathBuilder sixPathBuilder;
        internal System.Drawing.Drawing2D.GraphicsPath netPath;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Dispose()
        {
        }

        public PointF[] PathPoints
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netPath.PathPoints;
                else
                {
                    var path = sixPathBuilder.Build();
                    if (transformMatrix != null) path = path.Transform(transformMatrix.matrix);

                    return path.Flatten().SelectMany(x => x.Points.ToArray().Select(point => new PointF(point.X, point.Y)).ToArray()).ToArray();
                }
            }
        }

        public int PointCount
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netPath.PointCount;
                else
                    return PathPoints.Length;
            }
        }

        public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddArc(rect, startAngle, sweepAngle);
            else
                AddArc(rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddArc(x, y, width, height, startAngle, sweepAngle);
            else
            {
                var radiusWidth = width / 2;
                var radiusHeight = height / 2;
                var centerX = x + radiusWidth;
                var centerY = y + radiusHeight;

                sixPathBuilder.AddArc(centerX, centerY, radiusWidth, radiusHeight, 0, startAngle, sweepAngle);
            }
        }

        public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddBezier(pt1, pt2, pt3, pt4);
            else
                AddBezier(pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddBezier(x1, y1, x2, y2, x3, y3, x4, y4);
            else
            {
                var sixPt1 = new SixLabors.ImageSharp.PointF(x1, y1);
                var sixPt2 = new SixLabors.ImageSharp.PointF(x2, y2);
                var sixPt3 = new SixLabors.ImageSharp.PointF(x3, y3);
                var sixPt4 = new SixLabors.ImageSharp.PointF(x4, y4);

                sixPathBuilder.AddCubicBezier(sixPt1, sixPt2, sixPt3, sixPt4);
            }
        }

        public void AddBeziers(PointF[] points)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddBeziers(points);
            else
                sixPathBuilder.AddSegment(
                    new SixLabors.ImageSharp.Drawing.CubicBezierLineSegment(points.Select(point => PointExt.ToSixPoint(point)).ToArray()));
        }

        public void AddEllipse(RectangleF rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddEllipse(rect);
            else
            {
                var kappa = 0.5522848f;

                var sizeVector = new SizeF(rect.Size.Width / 2, rect.Size.Height / 2);
                var rootLocation = rect.Location;

                var pointO = new SizeF(sizeVector.Width * kappa, sizeVector.Height * kappa);
                var pointE = rect.Location + rect.Size;
                var pointM = rect.Location + sizeVector;
                var pointMminusO = pointM - pointO;
                var pointMplusO = pointM + pointO;

                var points = new PointF[] {
                new PointF(rootLocation.X, pointM.Y),

                new PointF(rootLocation.X, pointMminusO.Y),
                new PointF(pointMminusO.X, rootLocation.Y),
                new PointF(pointM.X, rootLocation.Y),

                new PointF(pointMplusO.X, rootLocation.Y),
                new PointF(pointE.X, pointMminusO.Y),
                new PointF(pointE.X, pointM.Y),

                new PointF(pointE.X, pointMplusO.Y),
                new PointF(pointMplusO.X, pointE.Y),
                new PointF(pointM.X, pointE.Y),

                new PointF(pointMminusO.X, pointE.Y),
                new PointF(rootLocation.X, pointMplusO.Y),
                new PointF(rootLocation.X, pointM.Y),
             };

                AddBeziers(points);
            }
        }

        public void AddLine(PointF pt1, PointF pt2)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddLine(pt1, pt2);
            else
                AddLine(pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        public void AddLine(int x1, int y1, int x2, int y2)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddLine(x1, y1, x2, y2);
            else
                AddLine((float)x1, (float)y1, (float)x2, (float)y2);
        }

        public void AddLine(float x1, float y1, float x2, float y2)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddLine(x1, y1, x2, y2);
            else
                sixPathBuilder.AddLine(x1, y1, x2, y2);
        }

        public void AddLines(Point[] points)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddLines(points);
            else
                AddLines(points.Cast<PointF>().ToArray());
        }

        public void AddLines(PointF[] points)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddLines(points);
            else
                sixPathBuilder.AddLines(points.Select(point => PointExt.ToSixPoint(point)));
        }

        public void AddPie(Rectangle rect, float startAngle, float sweepAngle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddPie(rect, startAngle, sweepAngle);
            else
                AddPie(rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void AddPie(RectangleF rect, float startAngle, float sweepAngle)
        {
            AddPie(rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddPie(x, y, width, height, startAngle, sweepAngle);
            else
            {
                var radiusWidth = width / 2;
                var radiusHeight = height / 2;
                var centerX = x + radiusWidth;
                var centerY = y + radiusHeight;

                var pathBuilder = new SixLabors.ImageSharp.Drawing.PathBuilder();
                pathBuilder.AddArc(centerX, centerY, radiusWidth, radiusHeight, 0, startAngle, sweepAngle);
                var points = pathBuilder.Build().Flatten().First().Points.ToArray();
                var firstPoint = points.First();
                var lastPoint = points.Last();

                sixPathBuilder.AddLine(centerX, centerY, firstPoint.X, firstPoint.Y);
                sixPathBuilder.AddArc(centerX, centerY, radiusWidth, radiusHeight, 0, startAngle, sweepAngle);
                sixPathBuilder.AddLine(lastPoint.X, lastPoint.Y, centerX, centerY);
            }
        }

        public void AddPolygon(Point[] points)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddPolygon(points);
            else
                AddPolygon(points.Select(point => new PointF(point.X, point.Y)).ToArray());
        }

        public void AddPolygon(PointF[] points)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddPolygon(points);
            else
            {
                var sixPoints = points.Select(point => new SixLabors.ImageSharp.PointF(point.X, point.Y)).ToArray();
                var polygon = new SixLabors.ImageSharp.Drawing.LinearLineSegment(sixPoints);
                sixPathBuilder.AddSegment(polygon);
                sixPathBuilder.CloseFigure();
            }
        }

        public void AddRectangle(RectangleF rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddRectangle(rect);
            else
            {
                var points = new PointF[] {
                    new PointF(rect.Left, rect.Top),
                    new PointF(rect.Right, rect.Top),
                    new PointF(rect.Right, rect.Bottom),
                    new PointF(rect.Left, rect.Bottom)
                };

                AddPolygon(points);
            }
        }

        public void AddCurve(PointF[] points, float tension)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddClosedCurve(points, tension);
            else
            {
                tension *= 0.3f;
                var lastIndex = points.Length - 2;
                var prevIndex = 0;
                for (var i = 0; i <= lastIndex; i++)
                {
                    float cpX1 = points[i].X + tension * (points[i + 1].X - points[prevIndex].X);
                    float cpY1 = points[i].Y + tension * (points[i + 1].Y - points[prevIndex].Y);
                    float cpX2 = points[i + 1].X;
                    float cpY2 = points[i + 1].Y;

                    if (i == lastIndex)
                    {
                        cpX2 += tension * (points[i].X - points[i + 1].X);
                        cpY2 += tension * (points[i].Y - points[i + 1].Y);
                    }
                    else
                    {
                        cpX2 -= tension * (points[i + 2].X - points[i].X);
                        cpY2 -= tension * (points[i + 2].Y - points[i].Y);
                    }

                    AddBezier(points[i].X, points[i].Y, cpX1, cpY1, cpX2, cpY2, points[i + 1].X, points[i + 1].Y);
                    prevIndex = i;
                }
            }
        }

        public void AddString(string s, FontFamily family, int style, float emSize, Rectangle layoutRectangle, StringFormat format)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddString(s, family.netFontFamily, style, emSize, layoutRectangle, format.netFormat);
            else
                AddString(s, family, style, emSize, new RectangleF(layoutRectangle.Location, layoutRectangle.Size), format);
        }

        public void AddString(string s, FontFamily family, int style, float emSize, RectangleF layoutRectangle, StringFormat format)
        {

            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.AddString(s, family.netFontFamily, style, emSize, layoutRectangle, format.netFormat);
            else
            {
                var font = new Font(family, emSize, (System.Drawing.FontStyle)style, System.Drawing.GraphicsUnit.Pixel);
                var location = new SixLabors.ImageSharp.PointF(layoutRectangle.Location.X, layoutRectangle.Location.Y);

                var textOptions = new SixLabors.Fonts.TextOptions(font.sixFont);
                textOptions.Dpi = 96;
                textOptions.Origin = location;
                textOptions.WrappingLength = layoutRectangle.Width;

                var shapes = SixLabors.ImageSharp.Drawing.TextBuilder.GenerateGlyphs(s, textOptions);
                foreach (var shape in shapes)
                {
                    var complexPolygon = shape as SixLabors.ImageSharp.Drawing.ComplexPolygon;
                    if (complexPolygon == null) complexPolygon = new SixLabors.ImageSharp.Drawing.ComplexPolygon(shape);

                    foreach(SixLabors.ImageSharp.Drawing.Polygon polygon in complexPolygon.Paths)
                    {
                        sixPathBuilder.StartFigure();

                        foreach (var segment in polygon.LineSegments)
                        {
                            sixPathBuilder.AddSegment(segment);
                        }
                        sixPathBuilder.CloseFigure();
                    }
                }
            }
        }

        public void Transform(Matrix matrix)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.Transform(matrix.netMatrix);
            else
            {
                transformMatrix = matrix;
                sixPathBuilder.SetTransform(matrix.matrix);
            }
        }

        public RectangleF GetBounds()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netPath.GetBounds();
            else
            {
                var path = sixPathBuilder.Build();
                if (transformMatrix != null) path = path.Transform(transformMatrix.matrix);

                var bounds = path.Bounds;
                return new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }
        }

        public bool IsVisible(float x, float y)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netPath.IsVisible(x, y);
            else
                return false;
        }

        public void StartFigure()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.StartFigure();
            else
                sixPathBuilder.StartFigure();
        }

        public void CloseAllFigures()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.CloseAllFigures();
            else
                sixPathBuilder.CloseAllFigures();
        }

        public void CloseFigure()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath.CloseFigure();
            else
                sixPathBuilder.CloseFigure();
        }

        public GraphicsPath()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPath = new System.Drawing.Drawing2D.GraphicsPath();
            else
                sixPathBuilder = new SixLabors.ImageSharp.Drawing.PathBuilder();
        }
    }
}
