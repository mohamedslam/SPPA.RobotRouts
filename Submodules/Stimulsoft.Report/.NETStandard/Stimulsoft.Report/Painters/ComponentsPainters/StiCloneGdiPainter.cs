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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
using Pen = Stimulsoft.Drawing.Pen;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiCloneGdiPainter : StiContainerGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var clone = component as StiClone;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!clone.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                clone.InvokePainting(clone, e);

            if (!e.Cancel)
            {
                var g = e.Graphics;

                if (clone.IsDesigning)
                {
                    var rect = clone.GetPaintRectangle();

                    #region Fill rectangle
                    if (e.DrawBorderFormatting)
                    {
                        if (clone.Brush is StiSolidBrush && ((StiSolidBrush)clone.Brush).Color == Color.Transparent &&
                            clone.Report.Info.FillComponent && clone.IsDesigning)
                        {
                            Color color = Color.FromArgb(245, 245, 245);
                            StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                        }
                        else StiDrawing.FillRectangle(g, clone.Brush, rect);
                    }
                    #endregion

                    #region Clone name
                    if (e.DrawBorderFormatting)
                    {
                        using (var stringFormat = new StringFormat())
                        using (var font = new Font("Segoe UI", (float)(15 * clone.Page.Zoom)))
                        using (var brush = new SolidBrush(Color.Gray))
                        {
                            stringFormat.LineAlignment = StringAlignment.Center;
                            stringFormat.Alignment = StringAlignment.Center;

                            StiTextDrawing.DrawString(g, clone.Name, font, brush, new RectangleD(rect.Left, rect.Top, rect.Width, rect.Height), stringFormat);

                        }
                    }
                    #endregion

                    #region Markers
                    if (e.DrawBorderFormatting)
                    {
                        if (clone.HighlightState == StiHighlightState.Hide && clone.Border.Side != StiBorderSides.All)
                            PaintMarkers(clone, g, rect);
                    }
                    #endregion

                    #region Border
                    if (clone.Border.Side == StiBorderSides.None && clone.IsDesigning && e.DrawBorderFormatting)
                    {
                        using (var pen = new Pen(Color.FromArgb(128, 128, 128)))
                        {
                            pen.DashStyle = DashStyle.Dash;
                            StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                        }
                    }
                    if (clone.HighlightState == StiHighlightState.Hide)
                    {
                        PaintBorder(clone, g, rect, clone.Page.Zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!clone.Border.Topmost));
                    }
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        PaintEvents(clone, g, rect);
                        PaintConditions(clone, g, rect);
                    }
                }
            }

            e.Cancel = false;
            if (e.DrawBorderFormatting)
                clone.InvokePainted(clone, e);
        }
        #endregion
    }
}