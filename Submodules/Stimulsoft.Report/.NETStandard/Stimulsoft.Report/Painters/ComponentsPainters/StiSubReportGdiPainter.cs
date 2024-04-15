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
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Brush = Stimulsoft.Drawing.Brush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using HatchBrush = Stimulsoft.Drawing.Drawing2D.HatchBrush;
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiSubReportGdiPainter : StiContainerGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var subReport = component as StiSubReport;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!subReport.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                subReport.InvokePainting(subReport, e);

            var g = e.Graphics;

            if (!e.Cancel)
            {
                var rect = subReport.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    if (subReport.IsDesigning)
                    {
                        if (e.DrawBorderFormatting)
                        {
                            if (subReport.Brush is StiSolidBrush &&
                                ((StiSolidBrush)subReport.Brush).Color == Color.Transparent &&
                                subReport.Report.Info.FillComponent &&
                                subReport.IsDesigning)
                            {
                                using (Brush brush = new HatchBrush(HatchStyle.BackwardDiagonal,
                                    Color.FromArgb(240, Color.LightBlue),
                                    Color.FromArgb(240, Color.White)))
                                {
                                    StiDrawing.FillRectangle(g, brush, rect.Left, rect.Top, rect.Width, rect.Height);
                                    StiDrawing.DrawRectangle(g, Pens.Black, rect.Left, rect.Top, rect.Width, rect.Height);
                                }
                            }
                            else StiDrawing.FillRectangle(g, subReport.Brush, rect);
                        }

                        if (e.DrawBorderFormatting)
                        {
                            using (var stringFormat = new StringFormat())
                            using (var font = new Font("Segoe UI", (float)(15 * subReport.Page.Zoom)))
                            using (var brush = new SolidBrush(Color.FromArgb(146, 146, 146)))
                            {
                                stringFormat.LineAlignment = StringAlignment.Center;
                                stringFormat.Alignment = StringAlignment.Center;

                                string info = null;
                                if (!string.IsNullOrEmpty(subReport.SubReportUrl))
                                {
                                    info = string.Format("{0}\n{1}",
                                        subReport.Name,
                                        subReport.SubReportUrl);
                                }
                                else
                                {
                                    bool needOneLine = rect.Height < 44 * StiScale.Factor;
                                    info = string.Format("{0}{4}{3}{1} : {2}",
                                        subReport.Name,
                                        needOneLine ? "" : StiLocalization.Get("PropertyMain", "SubReportPage"),
                                        subReport.SubReportPage != null ? subReport.SubReportPage.Name : StiLocalization.Get("Report", "NotAssigned"),
                                        needOneLine ? "" : "\n",
                                        !string.IsNullOrWhiteSpace(subReport.Alias) && (subReport.Alias != subReport.Name) & !needOneLine ? string.Format(" [{0}]", subReport.Alias) : "");
                                }

                                StiTextDrawing.DrawString(g, info, font, brush,
                                    new RectangleD(rect.Left, rect.Top, rect.Width, rect.Height), stringFormat);
                            }
                        }


                        #region Markers
                        if (e.DrawBorderFormatting)
                        {
                            if (subReport.HighlightState == StiHighlightState.Hide && subReport.Border.Side != StiBorderSides.All)
                                PaintMarkers(subReport, g, rect);
                        }
                        #endregion

                        #region Border
                        if (subReport.Border.Side == StiBorderSides.None && subReport.IsDesigning)
                        {
                            if (e.DrawBorderFormatting)
                            {
                                using (var pen = new Pen(Color.FromArgb(128, 128, 128)))
                                {
                                    pen.DashStyle = DashStyle.Dash;
                                    StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                                }
                            }
                        }
                        if (subReport.HighlightState == StiHighlightState.Hide)
                        {
                            PaintBorder(subReport, g, rect, subReport.Page.Zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!subReport.Border.Topmost));
                        }
                        #endregion

                        if (e.DrawBorderFormatting)
                        {
                            PaintEvents(subReport, g, rect);
                            PaintConditions(subReport, g, rect);
                        }
                    }
                }
            }
            e.Cancel = false;

            if (e.DrawBorderFormatting)
            {
                subReport.InvokePainted(subReport, e);
                PaintColumns(subReport, e.Graphics);
            }

            base.PaintComponents(subReport, e);            
        }
        #endregion
    }
}