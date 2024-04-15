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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Pen = Stimulsoft.Drawing.Pen;
using Brushes = Stimulsoft.Drawing.Brushes;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiCrossBandGdiPainter : StiBandGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var band = component as StiBand;

            band.InvokePainting(band, e);
            var g = e.Graphics;

            if (!e.Cancel)
            {
                if (band.IsDesigning)
                {
                    var rect = band.GetPaintRectangle();

                    #region Set colors
                    Color headerColorStart = Color.White;

                    if (band is StiCrossDataBand)
                    {
                        headerColorStart = Color.FromArgb(90, 147, 204);
                    }
                    else if (band is StiCrossHeaderBand || band is StiCrossFooterBand)
                    {
                        headerColorStart = Color.FromArgb(178, 197, 223);
                    }
                    else if (band is StiCrossGroupHeaderBand || band is StiCrossGroupFooterBand)
                    {
                        headerColorStart = Color.FromArgb(239, 155, 52);
                    }
                    #endregion

                    var zoom = band.Page.Zoom * StiScale.X;
                    var headerRect = new RectangleD(rect.Left, rect.Bottom, rect.Width, band.HeaderSize * zoom);

                    var fullRect = rect;
                    fullRect.Height += rect.Height;

                    if (fullRect.Width > 0 && fullRect.Height > 0 && (e.ClipRectangle.IsEmpty || fullRect.IntersectsWith(e.ClipRectangle)))
                    {
                        #region Fill bands
                        if (band.Brush is StiSolidBrush && ((StiSolidBrush)band.Brush).Color == Color.Transparent &&
                            band.Report.Info.FillCrossBands && band.IsDesigning)
                        {
                            var color = band.Report.Info.GetFillColor(band.HeaderEndColor);
                            StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                        }
                        else StiDrawing.FillRectangle(g, band.Brush, rect);
                        #endregion

                        #region Headers
                        if (band.Report.Info.ShowHeaders && headerRect.Width != 0 && headerRect.Height != 0)
                        {
                            using (var brush = new SolidBrush(headerColorStart))
                            {
                                g.FillRectangle(brush, headerRect.ToRectangleF());
                            }
                        }
                        #endregion

                        if (band.Report.Info.FillCrossBands)
                        {
                            var color = band.Report.Info.GetFillColor(headerColorStart);
                            StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                        }

                        if (e.DrawBorderFormatting)
                        {
                            if (band.IsDesigning)
                            {
                                var headerColor = Color.FromArgb(220, band.HeaderStartColor);
                                using (var pen = new Pen(headerColor))
                                {
                                    pen.DashStyle = DashStyle.Dash;
                                    StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                                }
                            }
                            //StiDrawing.DrawRectangle(g, Pens.Black, rect.Left, rect.Top, rect.Width, rect.Height);
                            PaintBorder(band, g, rect, zoom, e.DrawBorderFormatting, true);
                        }

                        #region Band name
                        using (var stringFormat = new StringFormat())
                        using (var font = new Font("Segoe UI", (float)(10 * band.Page.Zoom)))
                        {
                            stringFormat.LineAlignment = StringAlignment.Center;
                            stringFormat.Alignment = StringAlignment.Near;

                            if (band.Report.Info.ShowHeaders && headerRect.Width != 0 && headerRect.Height != 0)
                            {
                                StiTextDrawing.DrawString(g, band.Name, font, Brushes.Black, headerRect, stringFormat);
                            }
                        }
                        #endregion

                        PaintEvents(band, g, rect);
                        PaintConditions(band, g, rect);
                    }
                }
            }
            e.Cancel = false;
            band.InvokePainted(band, e);

            PaintComponents(band, e);
        }
        #endregion
    }
}