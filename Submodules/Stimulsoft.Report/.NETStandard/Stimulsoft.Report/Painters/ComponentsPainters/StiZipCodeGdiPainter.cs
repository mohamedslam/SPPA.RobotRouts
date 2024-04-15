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
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiZipCodeGdiPainter : StiComponentGdiPainter
    {
        #region Methods
        #region DrawLines
        /// <summary>
        /// Draw Up Horizontal Line
        /// </summary>
        private void DrawHorizLineUp(Graphics g, RectangleD drawRect, Pen pen)
        {
            float z = pen.Width;
            if (pen.DashStyle == DashStyle.Solid) z = pen.Width / 2;
            if (!StiOptions.Engine.UseOldZipCodePaintMode) z = 0;
            float x1 = (float)(drawRect.X + z);
            float y1 = (float)(drawRect.Y + z);
            float x2 = (float)(drawRect.Right - z);
            float y2 = (float)(drawRect.Y + z);
            g.DrawLine(pen, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draw Center Horizontal Line
        /// </summary>
        private void DrawHorizLineCenter(Graphics g, RectangleD drawRect, Pen pen)
        {
            float z = pen.Width;
            if (pen.DashStyle == DashStyle.Solid) z = pen.Width / 2;
            if (!StiOptions.Engine.UseOldZipCodePaintMode) z = 0;
            float x1 = (float)(drawRect.X + z);
            float y1 = (float)(drawRect.Y + drawRect.Height / 2);
            float x2 = (float)(drawRect.Right - z);
            float y2 = (float)(drawRect.Y + drawRect.Height / 2);
            g.DrawLine(pen, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draw Down Horizontal Line
        /// </summary>
        private void DrawHorizLineDown(Graphics g, RectangleD drawRect, Pen pen)
        {
            float z = pen.Width;
            if (pen.DashStyle == DashStyle.Solid) z = pen.Width / 2;
            if (!StiOptions.Engine.UseOldZipCodePaintMode) z = 0;
            float x1 = (float)(drawRect.X + z);
            float y1 = (float)(drawRect.Bottom - z);
            float x2 = (float)(drawRect.Right - z);
            float y2 = (float)(drawRect.Bottom - z);
            g.DrawLine(pen, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draw Up Right Vertical Line
        /// </summary>
        private void DrawVerticalLineUpRight(Graphics g, RectangleD drawRect, Pen pen)
        {
            float z = pen.Width;
            if (pen.DashStyle == DashStyle.Solid) z = pen.Width / 2;
            if (!StiOptions.Engine.UseOldZipCodePaintMode) z = 0;
            float x1 = (float)(drawRect.Right - z);
            float y1 = (float)(drawRect.Y + z);
            float x2 = (float)(drawRect.Right - z);
            float y2 = (float)(drawRect.Y + drawRect.Height / 2);
            g.DrawLine(pen, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draw Up Left Vertical Line
        /// </summary>
        private void DrawVerticalLineUpLeft(Graphics g, RectangleD drawRect, Pen pen)
        {
            float z = pen.Width;
            if (pen.DashStyle == DashStyle.Solid) z = pen.Width / 2;
            if (!StiOptions.Engine.UseOldZipCodePaintMode) z = 0;
            float x1 = (float)(drawRect.X + z);
            float y1 = (float)(drawRect.Y + z);
            float x2 = (float)(drawRect.X + z);
            float y2 = (float)(drawRect.Y + drawRect.Height / 2);
            g.DrawLine(pen, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draw Down Right Vertical Line
        /// </summary>
        private void DrawVerticalLineDownRight(Graphics g, RectangleD drawRect, Pen pen)
        {
            float z = pen.Width;
            if (pen.DashStyle == DashStyle.Solid) z = pen.Width / 2;
            if (!StiOptions.Engine.UseOldZipCodePaintMode) z = 0;
            float x1 = (float)(drawRect.Right - z);
            float y1 = (float)(drawRect.Y + drawRect.Height / 2);
            float x2 = (float)(drawRect.Right - z);
            float y2 = (float)(drawRect.Bottom - z);
            g.DrawLine(pen, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draw Down Left Vertical Line
        /// </summary>
        private void DrawVerticalLineDownLeft(Graphics g, RectangleD drawRect, Pen pen)
        {
            float z = pen.Width;
            if (pen.DashStyle == DashStyle.Solid) z = pen.Width / 2;
            if (!StiOptions.Engine.UseOldZipCodePaintMode) z = 0;
            float x1 = (float)(drawRect.X + z);
            float y1 = (float)(drawRect.Y + drawRect.Height / 2);
            float x2 = (float)(drawRect.X + z);
            float y2 = (float)(drawRect.Bottom - z);
            g.DrawLine(pen, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draw Up Dioganal Line
        /// </summary>
        private void DrawDioganalLineUp(Graphics g, RectangleD drawRect, Pen pen)
        {
            float z = pen.Width;
            if (pen.DashStyle == DashStyle.Solid) z = pen.Width / 2;
            if (!StiOptions.Engine.UseOldZipCodePaintMode) z = 0;
            float x1 = (float)(drawRect.Right - z);
            float y1 = (float)(drawRect.Y + z);
            float x2 = (float)(drawRect.X + z);
            float y2 = (float)(drawRect.Y + drawRect.Height / 2);
            g.DrawLine(pen, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draw Down Dioganal Line
        /// </summary>
        private void DrawDioganalLineDown(Graphics g, RectangleD drawRect, Pen pen)
        {
            float z = pen.Width;
            if (pen.DashStyle == DashStyle.Solid) z = pen.Width / 2;
            if (!StiOptions.Engine.UseOldZipCodePaintMode) z = 0;
            float x1 = (float)(drawRect.Right - z);
            float y1 = (float)(drawRect.Y + drawRect.Height / 2);
            float x2 = (float)(drawRect.X + z);
            float y2 = (float)(drawRect.Bottom - z);
            g.DrawLine(pen, x1, y1, x2, y2);
        }
        #endregion

        #region DrawIndex
        /// <summary>
        /// Draw Index 1
        /// </summary>
        private void DrawIndex1(Graphics g, RectangleD drawRect, Pen penSolid, Pen penDash)
        {
            DrawHorizLineUp(g, drawRect, penDash);
            DrawHorizLineCenter(g, drawRect, penDash);
            DrawHorizLineDown(g, drawRect, penDash);
            DrawVerticalLineUpLeft(g, drawRect, penDash);
            DrawVerticalLineDownLeft(g, drawRect, penDash);
            DrawVerticalLineUpRight(g, drawRect, penSolid);
            DrawVerticalLineDownRight(g, drawRect, penSolid);
            DrawDioganalLineUp(g, drawRect, penSolid);
            DrawDioganalLineDown(g, drawRect, penDash);
        }

        /// <summary>
        /// Draw Index 2
        /// </summary>
        private void DrawIndex2(Graphics g, RectangleD drawRect, Pen penSolid, Pen penDash, int count, int index)
        {
            DrawHorizLineUp(g, drawRect, penSolid);
            DrawHorizLineCenter(g, drawRect, penDash);
            DrawHorizLineDown(g, drawRect, penSolid);
            DrawVerticalLineUpLeft(g, drawRect, penDash);
            DrawVerticalLineDownLeft(g, drawRect, penDash);
            DrawVerticalLineUpRight(g, drawRect, penSolid);
            DrawVerticalLineDownRight(g, drawRect, penDash);
            DrawDioganalLineUp(g, drawRect, penDash);
            DrawDioganalLineDown(g, drawRect, penSolid);
        }

        /// <summary>
        /// Draw Index 3
        /// </summary>
        private void DrawIndex3(Graphics g, RectangleD drawRect, Pen penSolid, Pen penDash, int count, int index)
        {
            DrawHorizLineUp(g, drawRect, penSolid);
            DrawHorizLineCenter(g, drawRect, penSolid);
            DrawHorizLineDown(g, drawRect, penDash);
            DrawVerticalLineUpLeft(g, drawRect, penDash);
            DrawVerticalLineDownLeft(g, drawRect, penDash);
            DrawVerticalLineUpRight(g, drawRect, penDash);
            DrawVerticalLineDownRight(g, drawRect, penDash);
            DrawDioganalLineUp(g, drawRect, penSolid);
            DrawDioganalLineDown(g, drawRect, penSolid);
        }

        /// <summary>
        /// Draw Index 4
        /// </summary>
        private void DrawIndex4(Graphics g, RectangleD drawRect, Pen penSolid, Pen penDash, int count, int index)
        {
            DrawHorizLineUp(g, drawRect, penDash);
            DrawHorizLineCenter(g, drawRect, penSolid);
            DrawHorizLineDown(g, drawRect, penDash);
            DrawVerticalLineUpLeft(g, drawRect, penSolid);
            DrawVerticalLineDownLeft(g, drawRect, penDash);
            DrawVerticalLineUpRight(g, drawRect, penSolid);
            DrawVerticalLineDownRight(g, drawRect, penSolid);
            DrawDioganalLineUp(g, drawRect, penDash);
            DrawDioganalLineDown(g, drawRect, penDash);
        }

        /// <summary>
        /// Draw Index 5
        /// </summary>
        private void DrawIndex5(Graphics g, RectangleD drawRect, Pen penSolid, Pen penDash, int count, int index)
        {
            DrawHorizLineUp(g, drawRect, penSolid);
            DrawHorizLineCenter(g, drawRect, penSolid);
            DrawHorizLineDown(g, drawRect, penSolid);
            DrawVerticalLineUpLeft(g, drawRect, penSolid);
            DrawVerticalLineDownLeft(g, drawRect, penDash);
            DrawVerticalLineUpRight(g, drawRect, penDash);
            DrawVerticalLineDownRight(g, drawRect, penSolid);
            DrawDioganalLineUp(g, drawRect, penDash);
            DrawDioganalLineDown(g, drawRect, penDash);
        }

        /// <summary>
        /// Draw Index 6
        /// </summary>
        private void DrawIndex6(Graphics g, RectangleD drawRect, Pen penSolid, Pen penDash, int count, int index)
        {
            DrawHorizLineUp(g, drawRect, penDash);
            DrawHorizLineCenter(g, drawRect, penSolid);
            DrawHorizLineDown(g, drawRect, penSolid);
            DrawVerticalLineUpLeft(g, drawRect, penDash);
            DrawVerticalLineDownLeft(g, drawRect, penSolid);
            DrawVerticalLineUpRight(g, drawRect, penDash);
            DrawVerticalLineDownRight(g, drawRect, penSolid);
            DrawDioganalLineUp(g, drawRect, penSolid);
            DrawDioganalLineDown(g, drawRect, penDash);
        }

        /// <summary>
        /// Draw Index 7
        /// </summary>
        private void DrawIndex7(Graphics g, RectangleD drawRect, Pen penSolid, Pen penDash, int count, int index)
        {
            DrawHorizLineUp(g, drawRect, penSolid);
            DrawHorizLineCenter(g, drawRect, penDash);
            DrawHorizLineDown(g, drawRect, penDash);
            DrawVerticalLineUpLeft(g, drawRect, penDash);
            DrawVerticalLineDownLeft(g, drawRect, penSolid);
            DrawVerticalLineUpRight(g, drawRect, penDash);
            DrawVerticalLineDownRight(g, drawRect, penDash);
            DrawDioganalLineUp(g, drawRect, penSolid);
            DrawDioganalLineDown(g, drawRect, penDash);
        }

        /// <summary>
        /// Draw Index 8
        /// </summary>
        private void DrawIndex8(Graphics g, RectangleD drawRect, Pen penSolid, Pen penDash, int count, int index)
        {
            DrawHorizLineUp(g, drawRect, penSolid);
            DrawHorizLineCenter(g, drawRect, penSolid);
            DrawHorizLineDown(g, drawRect, penSolid);
            DrawVerticalLineUpLeft(g, drawRect, penSolid);
            DrawVerticalLineDownLeft(g, drawRect, penSolid);
            DrawVerticalLineUpRight(g, drawRect, penSolid);
            DrawVerticalLineDownRight(g, drawRect, penSolid);
            DrawDioganalLineUp(g, drawRect, penDash);
            DrawDioganalLineDown(g, drawRect, penDash);
        }

        /// <summary>
        /// Draw Index 9
        /// </summary>
        private void DrawIndex9(Graphics g, RectangleD drawRect, Pen penSolid, Pen penDash, int count, int index)
        {
            DrawHorizLineUp(g, drawRect, penSolid);
            DrawHorizLineCenter(g, drawRect, penSolid);
            DrawHorizLineDown(g, drawRect, penDash);
            DrawVerticalLineUpLeft(g, drawRect, penSolid);
            DrawVerticalLineDownLeft(g, drawRect, penDash);
            DrawVerticalLineUpRight(g, drawRect, penSolid);
            DrawVerticalLineDownRight(g, drawRect, penDash);
            DrawDioganalLineUp(g, drawRect, penDash);
            DrawDioganalLineDown(g, drawRect, penSolid);
        }

        /// <summary>
        /// Draw Index 0
        /// </summary>
        private void DrawIndex0(Graphics g, RectangleD drawRect, Pen penSolid, Pen penDash, int count, int index)
        {
            DrawHorizLineUp(g, drawRect, penSolid);
            DrawHorizLineCenter(g, drawRect, penDash);
            DrawHorizLineDown(g, drawRect, penSolid);
            DrawVerticalLineUpLeft(g, drawRect, penSolid);
            DrawVerticalLineDownLeft(g, drawRect, penSolid);
            DrawVerticalLineUpRight(g, drawRect, penSolid);
            DrawVerticalLineDownRight(g, drawRect, penSolid);
            DrawDioganalLineUp(g, drawRect, penDash);
            DrawDioganalLineDown(g, drawRect, penDash);
        }

        /// <summary>
        /// Draw Index Space
        /// </summary>
        private void DrawIndexSpace(Graphics g, RectangleD drawRect, Pen penSolid, Pen penDash, int count, int index)
        {
            float z = penSolid.Width / 2.5f;
            StiDrawing.DrawLine(g, penSolid, drawRect.X - z, drawRect.Y - z, drawRect.X + drawRect.Width + z, drawRect.Y - z);
        }
        #endregion		

        #region DrawSpace
        /// <summary>
        /// Draw space.
        /// </summary>
        private void DrawSpace(Graphics g, RectangleD rect, Pen penDash)
        {
            g.DrawRectangle(penDash, rect.ToRectangle());

            g.DrawLine(penDash, (float)rect.X,
                                (float)(rect.Y + (rect.Height / 2)),
                                (float)(rect.X + rect.Width),
                                (float)rect.Y);

            g.DrawLine(penDash, (float)rect.X,
                                (float)(rect.Y + (rect.Height / 2)),
                                (float)(rect.X + rect.Width),
                                (float)(rect.Y + (rect.Height / 2)));

            g.DrawLine(penDash, (float)(rect.X + rect.Width),
                                (float)(rect.Y + (rect.Height / 2)),
                                (float)rect.X,
                                (float)(rect.Y + rect.Height));
        }
        #endregion

        public void DrawZipCode(StiZipCode zipCode, Graphics g, RectangleD rect, double zoom)
        {
            #region Fill rectangle
            if (zipCode.Brush is StiSolidBrush &&
                ((StiSolidBrush)zipCode.Brush).Color == Color.Transparent &&
                zipCode.Report.Info.FillComponent &&
                zipCode.IsDesigning)
            {
                var color = Color.FromArgb(150, Color.White);

                StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
            }
            else StiDrawing.FillRectangle(g, zipCode.Brush, rect);
            #endregion

            var state = g.Save();
            g.SetClip(rect.ToRectangleF(), CombineMode.Intersect);
            using (var penDash = new Pen(zipCode.ForeColor))
            using (var penSolid = new Pen(zipCode.ForeColor))
            {
                penDash.DashStyle = DashStyle.Dot;
                penDash.Width = (float)(1);
                penDash.DashPattern = new float[] { 1, 1 };

                penSolid.DashStyle = DashStyle.Solid;
                penSolid.Width = (float)(zipCode.Size * 2 * zoom);

                penSolid.StartCap = LineCap.Triangle;
                penSolid.EndCap = LineCap.Triangle;

                #region Calculate Code
                string code;
                if (zipCode.CodeValue != null) code = zipCode.CodeValue;
                else
                {
                    var sb = new StringBuilder(zipCode.Code.Value);
                    for (int index = 0; index < sb.Length; index++)
                    {
                        if (!Char.IsDigit(sb[index])) sb[index] = ' ';
                    }
                    sb.Replace(" ", "");
                    code = sb.ToString();
                }
                #endregion

                for (int index = 0; index < code.Length; index++)
                {
                    RectangleD markRect;
                    RectangleD drawRect = zipCode.CalculateRect(rect, code.Length, index, out markRect);

                    switch (code[index])
                    {
                        case '1':
                            DrawIndex1(g, drawRect, penSolid, penDash);
                            break;
                        case '2':
                            DrawIndex2(g, drawRect, penSolid, penDash, code.Length, index);
                            break;
                        case '3':
                            DrawIndex3(g, drawRect, penSolid, penDash, code.Length, index);
                            break;
                        case '4':
                            DrawIndex4(g, drawRect, penSolid, penDash, code.Length, index);
                            break;
                        case '5':
                            DrawIndex5(g, drawRect, penSolid, penDash, code.Length, index);
                            break;
                        case '6':
                            DrawIndex6(g, drawRect, penSolid, penDash, code.Length, index);
                            break;
                        case '7':
                            DrawIndex7(g, drawRect, penSolid, penDash, code.Length, index);
                            break;
                        case '8':
                            DrawIndex8(g, drawRect, penSolid, penDash, code.Length, index);
                            break;
                        case '9':
                            DrawIndex9(g, drawRect, penSolid, penDash, code.Length, index);
                            break;
                        case '0':
                            DrawIndex0(g, drawRect, penSolid, penDash, code.Length, index);
                            break;
                        case ' ':
                            if (!StiOptions.Engine.UseOldZipCodePaintMode)
                                DrawIndexSpace(g, drawRect, penSolid, penDash, code.Length, index);
                            break;
                    }

                    if (zipCode.UpperMarks && !StiOptions.Engine.UseOldZipCodePaintMode)
                    {
                        StiDrawing.FillRectangle(g, penSolid.Color, markRect);
                    }
                }
            }
            g.Restore(state);
        }
        #endregion

        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var zipCode = (StiZipCode)component;

            if (format != StiExportFormat.HtmlSpan &&
                format != StiExportFormat.HtmlDiv &&
                format != StiExportFormat.HtmlTable) zoom *= 2;
            double resZoom = zipCode.Report.Info.Zoom;
            zipCode.Report.Info.Zoom = zoom;

            var rect = zipCode.ComponentToPage(zipCode.ClientRectangle).Normalize();
            rect = zipCode.Report.Unit.ConvertToHInches(rect).Multiply(zoom);

            rect.X = 0;
            rect.Y = 0;
            zipCode.Report.Info.Zoom = resZoom;

            int imageWidth = (int)rect.Width + 2;
            int imageHeight = (int)rect.Height + 2;

            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                if (format == StiExportFormat.ImagePng)
                {
                    g.Clear(Color.Transparent);
                }
                else
                {
                    g.Clear(Color.White);
                }
                DrawZipCode(zipCode, g, rect, zoom);
            }
            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var zipCode = (StiZipCode)component;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!zipCode.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                zipCode.InvokePainting(zipCode, e);

            if (!e.Cancel)
            {
                var g = e.Graphics;
                var rect = zipCode.GetPaintRectangle();
                double zoom = zipCode.Page.Zoom * StiScale.Factor;

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    #region ZipCode
                    if (e.DrawBorderFormatting)
                        DrawZipCode(zipCode, g, rect, zoom);
                    #endregion

                    #region Markers
                    if (e.DrawBorderFormatting)
                        PaintMarkers(zipCode, g, rect);
                    #endregion

                    #region Border
                    if (zipCode.HighlightState == StiHighlightState.Hide)
                        PaintBorder(zipCode, g, rect, zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!zipCode.Border.Topmost));
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        PaintEvents(zipCode, e.Graphics, rect);
                        PaintConditions(zipCode, e.Graphics, rect);
                    }
                }
            }

            e.Cancel = false;

            if (e.DrawBorderFormatting)
                zipCode.InvokePainted(zipCode, e);
        }
        #endregion
    }
}
