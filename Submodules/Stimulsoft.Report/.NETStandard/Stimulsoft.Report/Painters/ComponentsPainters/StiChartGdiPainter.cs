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

using Stimulsoft.Base;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Dashboard;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Events;
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
    public class StiChartGdiPainter : StiComponentGdiPainter
    {
        #region Methods
        public void DrawChart(StiChart chart, Graphics g, RectangleF rect, bool useMargins, bool useBackground, double? zoom = null)
        {
            #region Fill rectangle
            if (useBackground)
            {
                if (chart.Brush is StiSolidBrush &&
                    ((StiSolidBrush)chart.Brush).Color == Color.Transparent &&
                    chart.Report != null &&
                    chart.Report.Info.FillComponent &&
                    chart.IsDesigning)
                {
                    var color = Color.FromArgb(150, Color.White);
                    StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                }
                else
                {
                    StiDrawing.FillRectangle(g, chart.Brush, rect);
                }
            }
            #endregion

            if (zoom == null && chart?.Report?.Info != null)
                zoom = chart.Report.Info.Zoom * StiScale.Factor;

            var state = g.Save();

            float angle = 0;
            float deltaX = 0;
            float deltaY = 0;
            float scaleX = 1;
            float scaleY = 1;

            switch (chart.Rotation)
            {
                case StiImageRotation.Rotate90CCW:
                    angle = -90;
                    deltaY = rect.Height;
                    break;
                case StiImageRotation.Rotate90CW:
                    angle = 90;
                    deltaX = rect.Width;
                    break;
                case StiImageRotation.Rotate180:
                    angle = -180;
                    deltaX = rect.Width;
                    deltaY = rect.Height;
                    break;
                case StiImageRotation.FlipVertical:
                    scaleY = -1;
                    deltaY = rect.Height;
                    break;
                case StiImageRotation.FlipHorizontal:
                    scaleX = -1;
                    deltaX = rect.Width;
                    break;
            }

            g.SetClip(rect, CombineMode.Intersect);

            g.TranslateTransform((int)rect.X, (int)rect.Y);

            switch (chart.Rotation)
            {
                case StiImageRotation.Rotate90CCW:
                case StiImageRotation.Rotate90CW:
                    var rectTemp = rect;
                    rect.Width = rectTemp.Height;
                    rect.Height = rectTemp.Width;
                    break;
            }

            rect.X = 0;
            rect.Y = 0;

            var painter = new StiGdiContextPainter(g)
            {
                AnimationEngine = AnimationEngine
            };
            var context = new StiContext(painter, true, false, chart.IsPrinting, (float)zoom.GetValueOrDefault(1d));
            painter.Context = context;

            if (!useMargins)
            {
                rect.Width--;
                rect.Height--;
            }

            chart.IsAnimation = AnimationEngine != null && StiOptions.Viewer.Windows.AnimationPlaybackType != StiAnimationPlaybackType.None;

            if (chart.Area is StiAxisArea3D)
            {
                if (AnimationEngine != null)
                    AnimationEngine.RegisterContextPainter(chart, painter);

                var chartGeom = chart.Core.Render(context, new RectangleF(0, 0, rect.Width, rect.Height), useMargins);

                chartGeom.DrawGeom(context);
                chart.PreviousAnimations = context.Animations;
            }

            else
            {
                if (AnimationEngine == null || !AnimationEngine.IsRunning(chart))
                {
                    var chartGeom = chart.Core.Render(context, new RectangleF(0, 0, rect.Width, rect.Height), useMargins);

                    chartGeom.DrawGeom(context);
                    chart.PreviousAnimations = context.Animations;
                }

                if (AnimationEngine != null)
                    AnimationEngine.RegisterContextPainter(chart, painter);
            }

            chart.IsAnimation = false;

            g.TranslateTransform(deltaX, deltaY);
            g.ScaleTransform(scaleX, scaleY);
            g.RotateTransform(angle);

            context.Render(rect);

            g.Restore(state);
        }

        public void PaintChart(StiChart chart, Graphics g, RectangleF rect, bool drawBorder, bool drawFormatting)
        {
            #region Draw Formatting
            if (drawFormatting)
            {
                if (StiOptions.Print.ChartAsBitmap && chart.IsPrinting)
                {
                    float zoom = 1;
                    using (var image = GetImage(chart, ref zoom, StiExportFormat.None))
                    {
                        g.DrawImage(image, rect);
                    }
                }
                else
                {
                    DrawChart(chart, g, rect, true, true);
                }
            }
            #endregion

            #region Draw Border
            if ((drawBorder && chart.Border.Topmost) || (drawFormatting && (!chart.Border.Topmost)))
            {
                if (chart.Border.Side == StiBorderSides.None && chart.IsDesigning && !(chart.Page is IStiDashboard))
                {
                    using (var pen = new Pen(Color.Gray))
                    {
                        pen.DashStyle = DashStyle.Dash;
                        StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                    }
                }
                if (chart.HighlightState == StiHighlightState.Hide)
                {
                    PaintBorder(chart, g, RectangleD.CreateFromRectangle(rect), true, true);
                }
            }
            #endregion
        }
        #endregion

        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var chart = component as StiChart;

            var resZoom = chart.Report.Info.Zoom;
            if (format != StiExportFormat.HtmlSpan &&
                format != StiExportFormat.HtmlDiv &&
                format != StiExportFormat.HtmlTable) zoom *= 2;
            chart.Report.Info.Zoom = zoom;

            var rect = chart.GetPaintRectangle();
            rect.X = 0;
            rect.Y = 0;

            int imageWidth = (int)rect.Width + 2;
            int imageHeight = (int)rect.Height + 2;

            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;

                if (((format == StiExportFormat.Pdf) || (format == StiExportFormat.ImagePng)) && (chart.Brush != null) &&
                    (((chart.Brush is StiSolidBrush) && ((chart.Brush as StiSolidBrush).Color.A == 0)) || (chart.Brush is StiEmptyBrush)))
                {
                    g.Clear(Color.FromArgb(1, 255, 255, 255));
                }
                else
                {
                    if (((format == StiExportFormat.Excel) || (format == StiExportFormat.Excel2007) || (format == StiExportFormat.Word2007) || (format == StiExportFormat.Rtf) ||
                        (format == StiExportFormat.RtfFrame) || (format == StiExportFormat.RtfTable) || (format == StiExportFormat.RtfWinWord)) &&
                        (chart.Brush != null) && (((chart.Brush is StiSolidBrush) && ((chart.Brush as StiSolidBrush).Color.A < 16)) || (chart.Brush is StiEmptyBrush)))
                    {
                        g.Clear(Color.White);
                    }
                    else
                    {
                        g.Clear(StiBrush.ToColor(chart.Brush));
                    }
                }

                rect.X = 0;
                rect.Y = 0;

                DrawChart(chart, g, rect.ToRectangleF(), true, true);
            }
            chart.Report.Info.Zoom = resZoom;

            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var chart = component as StiChart;
            chart.InvokePainting(chart, e);

            if (!e.Cancel && (!(chart.Enabled == false && chart.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = chart.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 &&
                    (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    PaintChart(chart, g, rect.ToRectangleF(), e.DrawTopmostBorderSides, e.DrawBorderFormatting);

                    if (chart.Series.Count == 0 && chart.IsDesigning && e.DrawBorderFormatting)
                    {
                        PaintNoDefinedStatus(g, rect.ToRectangleF(), component, "StiChart", Loc.GetMain("NoElements"));
                    }

                    #region Markers
                    if (chart.HighlightState == StiHighlightState.Hide && chart.Border.Side != StiBorderSides.All)
                        PaintMarkers(chart, g, rect);
                    #endregion
                }

                PaintQuickButtons(chart, e.Graphics);
                PaintEvents(chart, e.Graphics, rect);
            }
            e.Cancel = false;
            chart.InvokePainted(chart, e);

        }
        #endregion
    }
}