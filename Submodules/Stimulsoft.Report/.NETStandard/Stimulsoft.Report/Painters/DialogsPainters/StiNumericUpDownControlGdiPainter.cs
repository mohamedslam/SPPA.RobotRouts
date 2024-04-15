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
    public class StiNumericUpDownControlGdiPainter : StiReportControlGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiNumericUpDownControl numericControl = component as StiNumericUpDownControl;
            numericControl.InvokePainting(numericControl, e);

            if (!e.Cancel && (!(numericControl.Enabled == false && numericControl.IsDesigning == false)))
            {
                Graphics g = e.Graphics;

                RectangleD rect = numericControl.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    using (SolidBrush brush = new SolidBrush(numericControl.BackColor))
                    {
                        g.FillRectangle(brush, rect.ToRectangle());
                    }

                    Rectangle borderRect = StiControlPaint.GetButtonRect(rect.ToRectangle(), false,
                        numericControl.RightToLeft);

                    using (SolidBrush brush = new SolidBrush(numericControl.ForeColor))
                    using (StringFormat sf = new StringFormat())
                    {
                        sf.FormatFlags = StringFormatFlags.NoWrap;
                        sf.LineAlignment = StringAlignment.Center;
                        g.DrawString(numericControl.Value.ToString(), numericControl.Font, brush,
                            StiControlPaint.GetContentRect(rect.ToRectangle(), false,
                            numericControl.RightToLeft), sf);
                    }

                    StiControlPaint.DrawBorder(g, rect.ToRectangle(), false, false, false);
                    
                    Rectangle upButtonRect = new Rectangle(borderRect.X, borderRect.Y, borderRect.Width, borderRect.Height / 2);
                    Rectangle downButtonRect = new Rectangle(borderRect.X, borderRect.Y + upButtonRect.Height, borderRect.Width, borderRect.Height - upButtonRect.Height);

                    StiControlPaint.DrawButton(g, upButtonRect, StiReportImages.Engine.SpinUp(), false, false, false, numericControl.Enabled, false);
                    StiControlPaint.DrawButton(g, downButtonRect, StiReportImages.Engine.SpinDown(), false, false, false, numericControl.Enabled, false);

                    PaintEvents(numericControl, e.Graphics, rect);
                    PaintConditions(numericControl, e.Graphics, rect);
                }
            }
            e.Cancel = false;
            numericControl.InvokePainted(numericControl, e);
        }
        #endregion
    }
}
