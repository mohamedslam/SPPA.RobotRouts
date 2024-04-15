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
using System.Collections;
using System.Text;
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
using Brush = Stimulsoft.Drawing.Brush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Pen = Stimulsoft.Drawing.Pen;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiLabelControlGdiPainter : StiReportControlGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiLabelControl label = component as StiLabelControl;
            label.InvokePainting(label, e);

            if (!e.Cancel && (!(label.Enabled == false && label.IsDesigning == false)))
            {
                Graphics g = e.Graphics;

                RectangleD rect = label.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    using (SolidBrush brush = new SolidBrush(label.BackColor))
                    {
                        g.FillRectangle(brush, rect.ToRectangleF());
                    }

                    #region Paint text
                    if (label.Text != null || label.Text.Length > 0)
                    {
                        using (StringFormat sf = new StringFormat())
                        {
                            sf.Trimming = StringTrimming.EllipsisCharacter;
                            sf.FormatFlags = StringFormatFlags.NoWrap;
                            sf.Trimming = StringTrimming.EllipsisCharacter;

                            if (label.RightToLeft == RightToLeft.Yes)
                                sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

                            #region TextAlign
                            switch (label.TextAlign)
                            {
                                case ContentAlignment.TopLeft:
                                    sf.Alignment = StringAlignment.Near;
                                    sf.LineAlignment = StringAlignment.Near;
                                    break;

                                case ContentAlignment.TopCenter:
                                    sf.Alignment = StringAlignment.Center;
                                    sf.LineAlignment = StringAlignment.Near;
                                    break;

                                case ContentAlignment.TopRight:
                                    sf.Alignment = StringAlignment.Far;
                                    sf.LineAlignment = StringAlignment.Near;
                                    break;

                                case ContentAlignment.MiddleLeft:
                                    sf.Alignment = StringAlignment.Near;
                                    sf.LineAlignment = StringAlignment.Center;
                                    break;

                                case ContentAlignment.MiddleCenter:
                                    sf.Alignment = StringAlignment.Center;
                                    sf.LineAlignment = StringAlignment.Center;
                                    break;

                                case ContentAlignment.MiddleRight:
                                    sf.Alignment = StringAlignment.Far;
                                    sf.LineAlignment = StringAlignment.Center;
                                    break;

                                case ContentAlignment.BottomLeft:
                                    sf.Alignment = StringAlignment.Near;
                                    sf.LineAlignment = StringAlignment.Far;
                                    break;

                                case ContentAlignment.BottomCenter:
                                    sf.Alignment = StringAlignment.Center;
                                    sf.LineAlignment = StringAlignment.Far;
                                    break;

                                case ContentAlignment.BottomRight:
                                    sf.Alignment = StringAlignment.Far;
                                    sf.LineAlignment = StringAlignment.Far;
                                    break;
                            }
                            #endregion

                            if (label.Enabled)
                            {
                                using (Brush brush = new SolidBrush(label.ForeColor))
                                {
                                    g.DrawString(label.Text, label.Font, brush, rect.ToRectangle(), sf);
                                }
                            }
                            else
                            {
                                ControlPaint.DrawStringDisabled(g, label.Text, label.Font, SystemColors.ControlLight, rect.ToRectangle(), sf);
                            }
                        }

                    }
                    #endregion

                    if (label.IsDesigning)
                    {
                        using (Pen pen = new Pen(SystemColors.ControlDarkDark, 1))
                        {
                            pen.DashStyle = DashStyle.Dash;
                            StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                        }
                    }
                    PaintEvents(label, e.Graphics, rect);
                    PaintConditions(label, e.Graphics, rect);
                }
            }
            e.Cancel = false;
            label.InvokePainted(label, e);
        }
        #endregion
    }
}
