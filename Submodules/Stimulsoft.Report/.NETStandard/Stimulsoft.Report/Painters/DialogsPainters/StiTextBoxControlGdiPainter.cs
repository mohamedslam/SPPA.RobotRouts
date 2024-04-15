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
    public class StiTextBoxControlGdiPainter : StiReportControlGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiTextBoxControl textBox = component as StiTextBoxControl;
            textBox.InvokePainting(textBox, e);

            if (!e.Cancel && (!(textBox.Enabled == false && textBox.IsDesigning == false)))
            {
                Graphics g = e.Graphics;

                RectangleD rect = textBox.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    using (SolidBrush brush = new SolidBrush(textBox.BackColor))
                    {
                        g.FillRectangle(brush, rect.ToRectangle());
                    }

                    Rectangle borderRect = rect.ToRectangle();
                    ControlPaint.DrawBorder3D(g, borderRect, Border3DStyle.Sunken);

                    rect.X += 4;
                    rect.Y += 4;
                    rect.Width -= 8;
                    rect.Height -= 8;

                    using (SolidBrush brush = new SolidBrush(textBox.ForeColor))
                    using (StringFormat sf = new StringFormat())
                    {
                        sf.FormatFlags = StringFormatFlags.NoWrap;
                        sf.Trimming = StringTrimming.EllipsisCharacter;
                        sf.FormatFlags = StringFormatFlags.NoWrap;
                        sf.Trimming = StringTrimming.EllipsisCharacter;

                        if (textBox.RightToLeft == RightToLeft.Yes)
                            sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

                        g.DrawString(textBox.Text, textBox.Font, brush, rect.ToRectangleF(), sf);
                    }

                    PaintEvents(textBox, g, rect);
                    PaintConditions(textBox, g, rect);
                }
            }
            e.Cancel = false;
            textBox.InvokePainted(textBox, e);
        }
        #endregion
    }
}
