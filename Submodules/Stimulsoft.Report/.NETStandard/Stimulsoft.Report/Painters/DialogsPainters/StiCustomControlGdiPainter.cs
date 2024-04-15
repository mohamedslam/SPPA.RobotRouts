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
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Controls;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Pen = Stimulsoft.Drawing.Pen;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiCustomControlGdiPainter : StiReportControlGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiCustomControl panel = component as StiCustomControl;
            panel.InvokePainting(panel, e);

            if (!e.Cancel && (!(panel.Enabled == false && panel.IsDesigning == false)))
            {
                Graphics g = e.Graphics;

                RectangleD rect = panel.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    using (SolidBrush brush = new SolidBrush(panel.BackColor))
                    {
                        g.FillRectangle(brush, rect.ToRectangleF());
                    }

                    if (panel.BorderStyle != StiBorderStyle.None)
                    {
                        Rectangle borderRect = rect.ToRectangle();
                        ControlPaint.DrawBorder3D(g, borderRect, (Border3DStyle)panel.BorderStyle);
                    }

                    if (panel.IsDesigning)
                    {
                        if (panel.BorderStyle == StiBorderStyle.None)
                        {
                            using (Pen pen = new Pen(SystemColors.ControlDarkDark, 1))
                            {
                                pen.DashStyle = DashStyle.Dash;
                                StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                            }
                        }

                        if (panel.Control != null)
                        {
                            Bitmap bitmapPanel = new Bitmap((int)panel.Width, (int)panel.Height);
                            ((Control)panel.Control).DrawToBitmap(bitmapPanel, ((Control)panel.Control).ClientRectangle);
                            g.DrawImage(bitmapPanel, new Rectangle((int)panel.Left, (int)panel.Top, (int)panel.Width, (int)panel.Height));
                        }
                        //#region Container name
                        //using (StringFormat stringFormat = new StringFormat())
                        //using (Font font = new Font("Arial", (float)(15 * panel.Page.Zoom)))
                        //using (Brush brush = new SolidBrush(Color.Gray))
                        //{
                        //    stringFormat.LineAlignment = StringAlignment.Center;
                        //    stringFormat.Alignment = StringAlignment.Center;

                        //    StiTextDrawing.DrawString(g, panel.Name, font, brush,
                        //        new RectangleD(rect.Left, rect.Top, rect.Width, rect.Height), stringFormat);

                        //}
                        //#endregion
                    }
                    PaintEvents(panel, e.Graphics, rect);
                    PaintConditions(panel, e.Graphics, rect);
                }
            }
            e.Cancel = false;
            panel.InvokePainted(panel, e);

            PaintComponents(panel, e);
        }
        #endregion
    }
}