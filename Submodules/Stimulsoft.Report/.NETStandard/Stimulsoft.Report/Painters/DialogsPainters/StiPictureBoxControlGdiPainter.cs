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
using Pens = Stimulsoft.Drawing.Pens;
using GraphicsState = Stimulsoft.Drawing.Drawing2D.GraphicsState;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiPictureBoxControlGdiPainter : StiReportControlGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiPictureBoxControl pictureControl = component as StiPictureBoxControl;

            pictureControl.InvokePainting(pictureControl, e);

            if (!e.Cancel && (!(pictureControl.Enabled == false && pictureControl.IsDesigning == false)))
            {
                Graphics g = e.Graphics;

                RectangleD rect = pictureControl.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    using (SolidBrush brush = new SolidBrush(pictureControl.BackColor))
                    {
                        g.FillRectangle(brush, rect.ToRectangleF());
                    }

                    Rectangle borderRect = rect.ToRectangle();

                    if (pictureControl.BorderStyle == BorderStyle.Fixed3D)
                    {
                        ControlPaint.DrawBorder3D(g, borderRect, Border3DStyle.Sunken);
                    }
                    else if (pictureControl.BorderStyle == BorderStyle.FixedSingle)
                    {
                        g.DrawRectangle(Pens.Black, borderRect);
                    }

                    borderRect.X += pictureControl.GetBorderWidth();
                    borderRect.Y += pictureControl.GetBorderHeight();
                    borderRect.Width -= pictureControl.GetBorderWidth() * 2;
                    borderRect.Height -= pictureControl.GetBorderHeight() * 2;

                    if (pictureControl.IsDesigning)
                    {
                        if (pictureControl.BorderStyle == BorderStyle.None)
                        {
                            using (Pen pen = new Pen(SystemColors.ControlDarkDark, 1))
                            {
                                pen.DashStyle = DashStyle.Dash;
                                StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                            }
                        }
                    }

                    if (pictureControl.Image != null)
                    {
                        GraphicsState state = g.Save();
                        g.SetClip(borderRect);

                        int imageWidth = pictureControl.Image.Width;
                        int imageHeight = pictureControl.Image.Height;

                        switch (pictureControl.SizeMode)
                        {
                            case PictureBoxSizeMode.AutoSize:
                            case PictureBoxSizeMode.Normal:
                                g.DrawImage(pictureControl.Image, borderRect.X, borderRect.Y, imageWidth, imageHeight);
                                break;

                            case PictureBoxSizeMode.CenterImage:
                                g.DrawImage(pictureControl.Image,
                                    borderRect.X + (borderRect.Width - imageWidth) / 2,
                                    borderRect.Y + (borderRect.Height - imageHeight) / 2, imageWidth, imageHeight);
                                break;

                            case PictureBoxSizeMode.StretchImage:
                                g.DrawImage(pictureControl.Image, borderRect);
                                break;
                        }

                        g.Restore(state);
                    }
                    PaintEvents(pictureControl, e.Graphics, rect);
                    PaintConditions(pictureControl, e.Graphics, rect);
                }
            }
            e.Cancel = false;
            pictureControl.InvokePainted(pictureControl, e);
        }
        #endregion
    }
}
