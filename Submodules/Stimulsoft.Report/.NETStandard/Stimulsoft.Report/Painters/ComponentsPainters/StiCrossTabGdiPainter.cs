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
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Pens = Stimulsoft.Drawing.Pens;
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiCrossTabGdiPainter : StiContainerGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var crossTab = (StiCrossTab)component;
            crossTab.InvokePainting(crossTab, e);
            var g = e.Graphics;

            if (!e.Cancel)
            {
                var rect = crossTab.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    if (crossTab.IsDesigning)
                    {
                        #region Fill container
                        if (crossTab.Report.Info.FillContainer)
                        {
                            var color = Color.FromArgb(100, 167, 215, 115);

                            StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                        }
                        #endregion

                        StiDrawing.DrawRectangle(g, Pens.Black, rect.Left, rect.Top, rect.Width, rect.Height);

                        #region Container name
                        using (var stringFormat = new StringFormat())
                        using (var font = new Font("Segoe UI", (float)(15 * crossTab.Page.Zoom)))
                        using (var brush = new SolidBrush(Color.FromArgb(102, 99, 99)))
                        {
                            stringFormat.LineAlignment = StringAlignment.Center;
                            stringFormat.Alignment = StringAlignment.Center;

                            StiTextDrawing.DrawString(g, crossTab.Name, font, brush, new RectangleD(rect.Left, rect.Top, rect.Width, rect.Height), stringFormat);
                        }
                        #endregion

                        #region Markers
                        PaintMarkers(crossTab, g, rect);
                        #endregion
                    }
                    PaintEvents(crossTab, e.Graphics, rect);
                    PaintConditions(crossTab, e.Graphics, rect);
                }

                var state = g.Save();
                rect.X -= 2;
                rect.Y -= 2;
                rect.Width += 4;
                rect.Height += 4;
                g.SetClip(rect.ToRectangleF(), CombineMode.Intersect);

                PaintComponents(crossTab, e);
                g.Restore(state);
            }

            e.Cancel = false;
            crossTab.InvokePainted(crossTab, e);

            PaintQuickButtons(crossTab, e.Graphics);
        }
        #endregion
    }
}