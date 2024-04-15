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

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiListBoxControlGdiPainter : StiReportControlGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiListBoxControl listBox = component as StiListBoxControl;
            listBox.InvokePainting(listBox, e);

            if (!e.Cancel && (!(listBox.Enabled == false && listBox.IsDesigning == false)))
            {
                Graphics g = e.Graphics;

                RectangleD rect = listBox.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    RectangleD rectEvent = rect;

                    using (SolidBrush brush = new SolidBrush(listBox.BackColor))
                    {
                        g.FillRectangle(brush, rect.ToRectangle());
                    }

                    Rectangle borderRect = rect.ToRectangle();
                    ControlPaint.DrawBorder3D(g, borderRect, Border3DStyle.Sunken);

                    rect.X += 4;
                    rect.Y += 4;
                    rect.Width -= 8;
                    rect.Height -= 8;

                    using (SolidBrush brush = new SolidBrush(listBox.ForeColor))
                    using (StringFormat sf = new StringFormat())
                    {
                        sf.FormatFlags = StringFormatFlags.NoWrap;

                        foreach (object item in listBox.Items)
                        {
                            string str = item.ToString();
                            g.DrawString(str, listBox.Font, brush, rect.ToRectangleF(), sf);
                            rect.Y += listBox.ItemHeight;
                            rect.Height -= listBox.ItemHeight;
                            if (rect.Height < 0) break;
                        }
                    }
                    PaintEvents(listBox, e.Graphics, rectEvent);
                    PaintConditions(listBox, e.Graphics, rect);
                }
            }
            e.Cancel = false;
            listBox.InvokePainted(listBox, e);
        }
        #endregion
    }
}
