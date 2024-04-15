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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;

#endif

namespace Stimulsoft.Report.Export
{
    public class StiSvgGeomWriter : IStiExportGeomWriter
    {
        #region Fields
        private XmlTextWriter writer;
        #endregion

        #region IStiExportGeomWriter
        public void BeginPath()
        {
        }

        public void CloseFigure()
        {
        }

        public void DrawBezier(PointF p1, PointF p2, PointF p3, PointF p4, object pen)
        {
        }

        public void DrawBezierTo(PointF p2, PointF p3, PointF p4, object pen)
        {
        }

        public void DrawImage(Image img, RectangleF rect)
        {
            #region Prepare data
            StiImage image = new StiImage();
            image.ClientRectangle = RectangleD.CreateFromRectangle(rect);
            image.PutImageToDraw(img);
            //image.HorAlignment = StiHorAlignment.Center;
            //image.VertAlignment = StiVertAlignment.Center;
            image.Smoothing = true;
            image.Stretch = true;
            image.Page = /* report.RenderedPages[0] ?? */ new StiPage();


            var pp = new StiSvgData();
            pp.X = rect.X;
            pp.Y = rect.Y;
            pp.Width = rect.Width;
            pp.Height = rect.Height;
            pp.Component = image;
            #endregion

            var imageCodec = StiImageCodecInfo.GetImageCodec("image/jpeg");
            StiSvgHelper.WriteImage2(writer, pp, img, 1, img.RawFormat, 0.75f, imageCodec);
        }

        public void DrawLine(PointF pointFrom, PointF pointTo, object pen)
        {
        }

        public void DrawLineTo(PointF pointTo, object pen)
        {
        }

        public void DrawPolygon(PointF[] points, object pen)
        {
        }

        public void DrawPolyline(PointF[] points, object pen)
        {
        }

        public void DrawPolylineTo(PointF[] points, object pen)
        {
        }

        public void DrawRectangle(RectangleF rect, object pen, StiCornerRadius corners = null)
        {
        }

        public void DrawString(string st, Font font, StiBrush brush, RectangleF rect, StringFormat sf, bool allowHtmlTags = false)
        {
            writer.WriteStartElement("text");

            var point = new PointF();

            switch (sf.Alignment)
            {
                case StringAlignment.Near:
                    point.X = rect.X;
                    break;

                case StringAlignment.Center:
                    point.X = rect.X + rect.Width / 2;
                    break;

                case StringAlignment.Far:
                    point.X = rect.X + rect.Width;
                    break;
            }

            switch (sf.LineAlignment)
            {
                case StringAlignment.Near:
                    point.Y = rect.Y;
                    break;

                case StringAlignment.Center:
                    point.Y = rect.Y + rect.Height / 2;
                    break;

                case StringAlignment.Far:
                    point.Y = rect.Y + rect.Height;
                    break;
            }

            writer.WriteAttributeString("transform", $"translate({point.X.ToString().Replace(",", ".")}, {point.Y.ToString().Replace(",", ".")})");

            writer.WriteAttributeString("width", rect.Width.ToString().Replace(",", "."));
            writer.WriteAttributeString("height", rect.Height.ToString().Replace(",", "."));

            var style = new StringBuilder();
            style.Append($"font-size:{font.SizeInPoints}pt;");
            style.Append($"font-family:'{font.Name}';");

            if (font.Bold)
                style.Append("font-weight:bold;");

            if (font.Italic)
                style.Append("font-style:italic;");

            if (font.Underline || font.Strikeout)
            {
                var decoration = font.Underline ? (font.Strikeout ? "underline line-through" : "underline") : (font.Strikeout ? "line-through" : null);
                style.Append($"text-decoration:{decoration};");
            }
            style.Append(StiContextSvgHelper.WriteFillBrush(writer, brush, rect));

            switch (sf.Alignment)
            {
                case StringAlignment.Near:
                    style.AppendFormat("text-anchor:{0};", "start");
                    break;

                case StringAlignment.Center:
                    style.AppendFormat("text-anchor:{0};", "middle");
                    break;

                case StringAlignment.Far:
                    style.AppendFormat("text-anchor:{0};", "end");
                    break;
            }

            switch (sf.LineAlignment)
            {
                case StringAlignment.Near:
                    writer.WriteAttributeString("dy", "1em");
                    break;

                case StringAlignment.Center:
                    writer.WriteAttributeString("dy", "0.5em");
                    break;

                case StringAlignment.Far:
                    break;
            }

            writer.WriteAttributeString("style", style.ToString());

            writer.WriteValue(st);
            writer.WriteEndElement();
        }

        public void DrawText(PointF basePoint, string text, int[] charsOffset, Font font, Color textColor, float angle, EmfTextAlignmentMode textAlign)
        {
        }

        public void EndPath()
        {
        }

        public void FillPath(object brush)
        {
        }

        public void FillPolygon(PointF[] points, object brush)
        {
            var style = new StringBuilder();
            style.Append(StiContextSvgHelper.WriteFillBrush(writer, brush, new RectangleF()));

            var pointsText = "";
            foreach (var point in points)
            {
                pointsText += $"{point.X.ToString().Replace(",", ".")},{point.Y.ToString().Replace(",", ".")} ";
            }

            writer.WriteStartElement("polygon");
            writer.WriteAttributeString("points", pointsText);
            writer.WriteAttributeString("style", style.ToString());
            writer.WriteEndElement();
        }

        public void FillPolygons(List<List<PointF>> points, object brush)
        {
            if (points.Count == 0) return;

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
            var rectf = new RectangleF(minX, minY, maxX - minX, maxY - minY);

            var style = new StringBuilder();
            style.Append(StiContextSvgHelper.WriteFillBrush(writer, brush, rectf));

            foreach (var list in points)
            {
                var pointsText = "";
                foreach (var point in list)
                {
                    pointsText += $"{Math.Round(point.X, 2).ToString().Replace(",", ".")},{Math.Round(point.Y, 2).ToString().Replace(",", ".")} ";
                }

                writer.WriteStartElement("polygon");
                writer.WriteAttributeString("points", pointsText);
                writer.WriteAttributeString("style", style.ToString());
                writer.WriteEndElement();
            }
        }

        public void FillRectangle(RectangleF rect, object brush, StiCornerRadius corners = null)
        {
            var style = new StringBuilder();

            style.Append(StiContextSvgHelper.WriteFillBrush(writer, brush, rect));

            writer.WriteStartElement("rect");
            writer.WriteAttributeString("x", rect.X.ToString().Replace(",", "."));
            writer.WriteAttributeString("y", rect.Y.ToString().Replace(",", "."));
            writer.WriteAttributeString("width", rect.Width.ToString().Replace(",", "."));
            writer.WriteAttributeString("height", rect.Height.ToString().Replace(",", "."));
            writer.WriteAttributeString("fill", "red");
            writer.WriteAttributeString("style", style.ToString());
            writer.WriteAttributeString("shape-rendering", "crispEdges");

            writer.WriteEndElement();
        }

        public void FillRectangle(RectangleF rect, Color color, StiCornerRadius corners = null)
        {
            this.FillRectangle(rect, (object)color);
        }

        public void DrawEllipse(RectangleF rect, object pen)
        {
        }

        public void FillEllipse(RectangleF rect, object brush)
        {
            var style = new StringBuilder();

            style.Append(StiContextSvgHelper.WriteFillBrush(writer, brush, rect));

            writer.WriteStartElement("ellipse");
            writer.WriteAttributeString("cx", (rect.X + rect.Width / 2).ToString().Replace(",", "."));
            writer.WriteAttributeString("cy", (rect.Y + rect.Height / 2).ToString().Replace(",", "."));
            writer.WriteAttributeString("rx", (rect.Width / 2).ToString().Replace(",", "."));
            writer.WriteAttributeString("ry", (rect.Height / 2).ToString().Replace(",", "."));

            writer.WriteAttributeString("style", style.ToString());

            writer.WriteEndElement();
        }

        public SizeF MeasureString(string st, Font font)
        {
            return new SizeF();
        }

        public void MoveTo(PointF point)
        {
        }

        public void RestoreState()
        {
        }

        public void RotateTransform(float angle)
        {
            writer.WriteStartElement("g");

            writer.WriteAttributeString("transform", $"rotate({angle.ToString().Replace(",", ".")})");
        }

        public void SaveState()
        {
        }

        public void SetPixel(PointF point, Color color)
        {
        }

        public void StrokePath(object pen)
        {
        }

        public void TranslateTransform(float x, float y)
        {
            writer.WriteStartElement("g");

            writer.WriteAttributeString("transform", $"translate({x.ToString().Replace(",", ".")},{y.ToString().Replace(",", ".")})");
        }

        public void EndTransform()
        {
            writer.WriteEndElement();
        }
        #endregion        

        public StiSvgGeomWriter(XmlTextWriter writer)
        {
            this.writer = writer;
        }
    }
}
