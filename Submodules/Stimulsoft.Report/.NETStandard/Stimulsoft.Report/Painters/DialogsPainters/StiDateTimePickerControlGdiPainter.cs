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
using Stimulsoft.Report.Images;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiDateTimePickerControlGdiPainter : StiReportControlGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiDateTimePickerControl dateTimePicker = component as StiDateTimePickerControl;
            dateTimePicker.InvokePainting(dateTimePicker, e);

            if (!e.Cancel && (!(dateTimePicker.Enabled == false && dateTimePicker.IsDesigning == false)))
            {
                Graphics g = e.Graphics;

                RectangleD rect = dateTimePicker.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    using (SolidBrush brush = new SolidBrush(dateTimePicker.BackColor))
                    {
                        g.FillRectangle(brush, rect.ToRectangle());
                    }

                    Rectangle borderRect = StiControlPaint.GetButtonRect(rect.ToRectangle(), false,
                        dateTimePicker.RightToLeft);

                    using (SolidBrush brush = new SolidBrush(dateTimePicker.ForeColor))
                    using (StringFormat sf = new StringFormat())
                    {
                        sf.FormatFlags = StringFormatFlags.NoWrap;
                        sf.LineAlignment = StringAlignment.Center;
                        g.DrawString(dateTimePicker.Value.ToString("G"), dateTimePicker.Font, brush,
                            StiControlPaint.GetContentRect(rect.ToRectangle(), false,
                            dateTimePicker.RightToLeft), sf);
                    }

                    StiControlPaint.DrawBorder(g, rect.ToRectangle(), false, false, false);
                    StiControlPaint.DrawButton(g, borderRect, StiReportImages.Engine.DropDown(), false, false, false, dateTimePicker.Enabled, false);

                    PaintEvents(dateTimePicker, e.Graphics, rect);
                    PaintConditions(dateTimePicker, e.Graphics, rect);
                }
            }
            e.Cancel = false;
            dateTimePicker.InvokePainted(dateTimePicker, e);
        }
        #endregion
    }
}
