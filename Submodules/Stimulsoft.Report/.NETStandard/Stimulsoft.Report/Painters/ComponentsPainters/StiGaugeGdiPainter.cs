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
using Stimulsoft.Gauge.Painters;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.Helpers;
using System;
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
    public class StiGaugeGdiPainter : StiComponentGdiPainter
    {
        #region Properties
        internal bool IsDashboardMode { get; set; }
        #endregion

        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var gauge = component as StiGauge;
            gauge.InvokePainting(gauge, e);

            if (!e.Cancel && (!(component.Enabled == false && component.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = component.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    #region Draw Formatting
                    if (e.DrawBorderFormatting)
                        DrawGauge(g, gauge, rect.ToRectangleF());
                    #endregion

                    #region Draw Border
                    if ((e.DrawTopmostBorderSides && gauge.Border.Topmost) || (e.DrawBorderFormatting && (!gauge.Border.Topmost)))
                    {
                        if (gauge.Border.Side == StiBorderSides.None && gauge.IsDesigning && !(gauge.Page is IStiDashboard))
                        {
                            using (var pen = new Pen(Color.Gray))
                            {
                                pen.DashStyle = DashStyle.Dash;
                                StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                            }
                        }
                        if (gauge.HighlightState == StiHighlightState.Hide)
                        {
                            PaintBorder(gauge, g, rect, true, true);
                        }
                    }
                    #endregion
                }
            }
            e.Cancel = false;
            gauge.InvokePainted(gauge, e);
        }

        public void DrawGauge(Graphics g, StiGauge gauge, RectangleF rect)
        {
            this.DrawGauge(g, gauge, rect, gauge.Page != null ? (float)gauge.Page.Zoom : 1f);
        }

        public void DrawGauge(Graphics g, StiGauge gauge, RectangleF rect, float zoom)
        {
            this.DrawGauge(g, gauge, rect, gauge.Page != null ? (float)gauge.Page.Zoom : 1f, true);
        }

        public void DrawGauge(Graphics g, StiGauge gauge, RectangleF rect, float zoom, bool isClip, bool drawBorder = true)
        {
            //gauge = (StiGauge)gauge.Clone();
            gauge.ApplyStyle(gauge.GetGaugeStyle());

            #region Fill rectangle
            if (!IsDashboardMode)
            {
                var fillComponent = gauge.Report?.Info?.FillComponent ?? false;
                if (gauge.Brush is StiSolidBrush && ((StiSolidBrush)gauge.Brush).Color == Color.Transparent && fillComponent && gauge.IsDesigning)
                {
                    var color = Color.FromArgb(150, Color.White);
                    StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                }
                else
                {
                    StiDrawing.FillRectangle(g, gauge.Brush, rect);
                }
            }
            #endregion

            if (isClip)
                g.SetClip(rect);

            if (gauge.IsDesigning)
            {
                StiGaugeV2InitHelper.Prepare(gauge);
            }

            var context = new StiGdiGaugeContextPainter(g, gauge, rect, zoom);
            context.AnimationEngine = AnimationEngine;

            if (AnimationEngine == null || !AnimationEngine.IsRunning(gauge))
            {
                gauge.DrawGauge(context);
                gauge.PreviousAnimations = context.Animations;
            }

            if (AnimationEngine != null)
                AnimationEngine.RegisterContextPainter(gauge, context);

            context.Render();

            if (isClip)
                g.ResetClip();

            #region Draw Border
            if (gauge.Border.Side == StiBorderSides.None && gauge.IsDesigning && drawBorder)
            {
                using (var pen = new Pen(Color.Gray))
                {
                    pen.DashStyle = DashStyle.Dash;
                    StiDrawing.DrawRectangle(g, pen, RectangleD.CreateFromRectangle(rect));
                }
            }
            if (gauge.HighlightState == StiHighlightState.Hide)
            {
                PaintBorder(gauge, g, RectangleD.CreateFromRectangle(rect), true, true);
            }
            #endregion
        }

        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var gauge = component as StiGauge;

            double resZoom = gauge.Report.Info.Zoom;
            if (format != StiExportFormat.HtmlSpan &&
                format != StiExportFormat.HtmlDiv &&
                format != StiExportFormat.HtmlTable) zoom *= 2;
            gauge.Report.Info.Zoom = zoom;

            var rect = gauge.GetPaintRectangle();
            rect.X = 0;
            rect.Y = 0;

            int imageWidth = (int)rect.Width + 2;
            int imageHeight = (int)rect.Height + 2;

            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;

                if (((format == StiExportFormat.Pdf) || (format == StiExportFormat.ImagePng)) && (gauge.Brush != null) &&
                    (((gauge.Brush is StiSolidBrush) && ((gauge.Brush as StiSolidBrush).Color.A == 0)) || (gauge.Brush is StiEmptyBrush)))
                {
                    g.Clear(Color.FromArgb(1, 255, 255, 255));
                }
                else
                {
                    if (((format == StiExportFormat.Excel) || (format == StiExportFormat.Excel2007) || (format == StiExportFormat.Word2007) || (format == StiExportFormat.Rtf) ||
                        (format == StiExportFormat.RtfFrame) || (format == StiExportFormat.RtfTable) || (format == StiExportFormat.RtfWinWord)) &&
                        (gauge.Brush != null) && (((gauge.Brush is StiSolidBrush) && ((gauge.Brush as StiSolidBrush).Color.A < 16)) || (gauge.Brush is StiEmptyBrush)))
                    {
                        g.Clear(Color.White);
                    }
                    else
                    {
                        g.Clear(StiBrush.ToColor(gauge.Brush));
                    }
                }

                rect.X = 0;
                rect.Y = 0;

                var rectF = rect.ToRectangleF();
                var context = new StiGdiGaugeContextPainter(g, gauge, rectF, zoom);
                gauge.DrawGauge(context);
                gauge.PreviousAnimations = context.Animations;
                context.Render();
            }
            gauge.Report.Info.Zoom = resZoom;

            return bmp;
        }
        #endregion
    }
}