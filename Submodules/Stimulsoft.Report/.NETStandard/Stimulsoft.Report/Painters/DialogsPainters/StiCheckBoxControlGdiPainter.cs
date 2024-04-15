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
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiCheckBoxControlGdiPainter : StiReportControlGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiCheckBoxControl checkBox = component as StiCheckBoxControl;
            checkBox.InvokePainting(checkBox, e);

            if (!e.Cancel && (!(checkBox.Enabled == false && checkBox.IsDesigning == false)))
            {
                Graphics g = e.Graphics;

                RectangleD rect = checkBox.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    using (CheckBox control = new CheckBox())
                    {
                        control.Text = checkBox.Text;
                        control.Checked = checkBox.Checked;

                        DrawControl(component as StiReportControl, e.Graphics, control, rect);
                    }

                    PaintEvents(checkBox, e.Graphics, rect);
                    PaintConditions(checkBox, e.Graphics, rect);
                }
            }
            e.Cancel = false;
            checkBox.InvokePainted(checkBox, e);
        }
        #endregion
    }
}
