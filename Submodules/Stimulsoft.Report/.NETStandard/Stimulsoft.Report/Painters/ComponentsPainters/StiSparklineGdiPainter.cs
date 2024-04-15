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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Data.Exceptions;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using System;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiSparklineGdiPainter : StiComponentGdiPainter
    {
        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var sparkline = (StiSparkline)component;

            if (format != StiExportFormat.HtmlSpan &&
                format != StiExportFormat.HtmlDiv &&
                format != StiExportFormat.HtmlTable) 
                zoom *= 2;

            var resZoom = sparkline.Report.Info.Zoom;
            sparkline.Report.Info.Zoom = zoom;
            var rect = sparkline.GetPaintRectangle();
            rect.X = 0;
            rect.Y = 0;
            rect.Width /= StiScale.Factor;
            rect.Height /= StiScale.Factor;
            sparkline.Report.Info.Zoom = resZoom;

            var imageWidth = (int)rect.Width + 2;
            var imageHeight = (int)rect.Height + 2;

            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;

                bool isTransparentBrush = (sparkline.Brush != null) && ((sparkline.Brush is StiSolidBrush) && ((sparkline.Brush as StiSolidBrush).Color.A == 0) || (sparkline.Brush is StiEmptyBrush));
                if ((format == StiExportFormat.Pdf) || (format == StiExportFormat.ImagePng))
                {
                    if (isTransparentBrush)
                    {
                        g.Clear(Color.FromArgb(1, 255, 255, 255));
                    }
                }
                else
                {
                    g.Clear(Color.White);
                }
                StiDrawing.FillRectangle(g, sparkline.Brush, rect);

                DrawSparkline(g, rect, sparkline, zoom);
            }
            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var sparkline = (StiSparkline)component;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!sparkline.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                sparkline.InvokePainting(sparkline, e);

            if (!e.Cancel)
            {
                var g = e.Graphics;
                var rect = sparkline.GetPaintRectangle();
                var zoom = (float)(sparkline.Page.Zoom * StiScale.Factor);

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    #region Fill rectangle
                    if (e.DrawBorderFormatting)
                        PaintBackground(sparkline, g, rect);
                    #endregion
                                        
                    DrawSparkline(g, rect, sparkline, zoom);

                    #region Markers
                    if (e.DrawBorderFormatting)
                        PaintMarkers(sparkline, g, rect);
                    #endregion

                    #region Border
                    if (sparkline.HighlightState == StiHighlightState.Hide)
                        PaintBorder(sparkline, g, rect, zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!sparkline.Border.Topmost));
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        PaintEvents(sparkline, e.Graphics, rect);
                        PaintConditions(sparkline, e.Graphics, rect);
                    }
                }
            }

            e.Cancel = false;

            if (e.DrawBorderFormatting)
                sparkline.InvokePainted(sparkline, e);
        }

        public virtual void PaintBackground(StiSparkline sparkline, Graphics g, RectangleD rect)
        {
            if (sparkline.Brush is StiSolidBrush &&
                ((StiSolidBrush)sparkline.Brush).Color.A == 0 &&
                sparkline.Report.Info.FillComponent &&
                sparkline.IsDesigning)
            {
                var color = Color.FromArgb(150, Color.GhostWhite);

                StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
            }
            else
                StiDrawing.FillRectangle(g, sparkline.Brush, rect);
        }

        internal static void DrawSparkline(Graphics g, RectangleD rect, StiSparkline sparkline, float zoom)
        {            
            var painter = new StiGdiContextPainter(g);
            var context = new StiContext(painter, true, false, sparkline.IsPrinting, zoom);

            RenderSparkline(context, rect, sparkline, zoom);
        }

        internal static void RenderSparkline(StiContext context, RectangleD rect, StiSparkline sparkline, float zoom)
        {
            var values = sparkline.FetchValues();

            var positiveColor = sparkline.PositiveColor;
            var negativeColor = sparkline.NegativeColor;

            if (!string.IsNullOrEmpty(sparkline.ComponentStyle))
            {
                var style = sparkline.Report.Styles.ToList().FirstOrDefault(x => x.Name == sparkline.ComponentStyle) as StiIndicatorStyle;
                if (style != null)
                {
                    positiveColor = style.PositiveColor;
                    negativeColor = style.NegativeColor;
                }
            }

            switch (sparkline.Type)
            {
                case StiSparklineType.Column:
                    StiColumnSparklinesCellPainter.Draw(context, rect, values, positiveColor, negativeColor);
                    break;

                case StiSparklineType.Line:
                    StiLineSparklinesCellPainter.Draw(context, rect, values, zoom, positiveColor,
                        false, sparkline.ShowFirstLastPoints, sparkline.ShowHighLowPoints);
                    break;

                case StiSparklineType.Area:
                    StiLineSparklinesCellPainter.Draw(context, rect, values, zoom, positiveColor,
                        true, sparkline.ShowFirstLastPoints, sparkline.ShowHighLowPoints);
                    break;

                case StiSparklineType.WinLoss:
                    StiWinLossSparklinesCellPainter.Draw(context, rect, values, positiveColor, negativeColor);
                    break;

                default:
                    throw new StiTypeNotRecognizedException(sparkline.Type);
            }

            context.Render(rect.ToRectangleF());
        }
        #endregion
    }
}
