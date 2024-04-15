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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Brushes = Stimulsoft.Drawing.Brushes;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiUndefinedComponentGdiPainter : StiComponentGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            component.InvokePainting(component, e);

            if (!e.Cancel && (!(component.Enabled == false && component.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = component.GetPaintRectangle();
                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    g.FillRectangle(Brushes.Red, rect.ToRectangleF());

                    using (var font = new Font("Arial", (float)(10 * component.Report.Info.Zoom)))
                    using (var stringFormat = new StringFormat())
                    {
                        stringFormat.LineAlignment = StringAlignment.Center;
                        stringFormat.Alignment = StringAlignment.Center;

                        g.DrawString("Undefined Component", font, Brushes.White, rect.ToRectangleF(), stringFormat);
                    }
                }

                PaintQuickButtons(component, e.Graphics);
                PaintEvents(component, e.Graphics, rect);
            }
            e.Cancel = false;
            component.InvokePainted(component, e);

        }
        #endregion
    }
}