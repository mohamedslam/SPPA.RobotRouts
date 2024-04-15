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
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiTreeViewControlGdiPainter : StiReportControlGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiTreeViewControl treeView = component as StiTreeViewControl;
            treeView.InvokePainting(treeView, e);

            if (!e.Cancel && (!(treeView.Enabled == false && treeView.IsDesigning == false)))
            {
                Graphics g = e.Graphics;

                RectangleD rect = treeView.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    using (SolidBrush brush = new SolidBrush(treeView.BackColor))
                    {
                        g.FillRectangle(brush, rect.ToRectangleF());
                    }

                    Rectangle borderRect = rect.ToRectangle();

                    ControlPaint.DrawBorder3D(g, borderRect, Border3DStyle.Sunken);

                    borderRect.X += (int)SystemInformation.Border3DSize.Width;
                    borderRect.Y += (int)SystemInformation.Border3DSize.Height;
                    borderRect.Width -= (int)SystemInformation.Border3DSize.Width * 2;
                    borderRect.Height -= (int)SystemInformation.Border3DSize.Height * 2;

                    using (SolidBrush brush = new SolidBrush(treeView.ForeColor))
                    using (StringFormat sf = new StringFormat())
                    {
                        g.DrawString(treeView.Name, treeView.Font, brush, borderRect, sf);
                    }

                    PaintEvents(treeView, g, rect);
                    PaintConditions(treeView, g, rect);
                }
            }
            e.Cancel = false;
            treeView.InvokePainted(treeView, e);
        }
        #endregion
    }
}
