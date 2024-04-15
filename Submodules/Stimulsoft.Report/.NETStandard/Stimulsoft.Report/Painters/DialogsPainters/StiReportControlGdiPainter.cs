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
using System.Reflection;
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
using StringFormat = Stimulsoft.Drawing.StringFormat;
using GraphicsState = Stimulsoft.Drawing.Drawing2D.GraphicsState;
using Pens = Stimulsoft.Drawing.Pens;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Brush = Stimulsoft.Drawing.Brush;
using Font = Stimulsoft.Drawing.Font;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiReportControlGdiPainter : StiContainerGdiPainter
    {
        #region Methods
        public void DrawControl(StiReportControl component, Graphics g, Control control, RectangleD rect)
        {
            if (component.Page != null && component.Report != null && control != null)
            {
                Rectangle destRect = rect.ToRectangle();

                if (destRect.Width > 0 && destRect.Height > 0)
                {
                    control.Left = 0;
                    control.Top = 0;
                    control.Width = destRect.Width;
                    control.Height = destRect.Height;
                    control.ForeColor = component.ForeColor;
                    control.BackColor = component.BackColor;
                    control.RightToLeft = component.RightToLeft;
                    control.Enabled = component.Enabled;
                    control.Font = component.Font;

                    var mf = new Bitmap(destRect.Width, destRect.Height);
                    using (Graphics gImage = Graphics.FromImage(mf))
                    {
                        gImage.Clear(SystemColors.Control);

                        if (control is Button)
                        {
                            control.DrawToBitmap(mf, new Rectangle(0, 0, destRect.Width, destRect.Height));
                        }
                        else
                        {
                            var controlType = control.GetType();

                            var getStyleInfo = controlType.GetMethod("GetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
                            var args = new object[1] { ControlStyles.DoubleBuffer };
                            var doubleBuffer = (bool)getStyleInfo.Invoke(control, args);

                            var setStyleInfo = controlType.GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
                            args = new object[2] { ControlStyles.DoubleBuffer, false };
                            setStyleInfo.Invoke(control, args);

                            var ptr = gImage.GetHdc();
                            var message = Message.Create(control.Handle, 15, ptr, IntPtr.Zero);

                            var wndProcInfo = controlType.GetMethod("WndProc", BindingFlags.NonPublic | BindingFlags.Instance);
                            args = new object[1] { message };
                            wndProcInfo.Invoke(control, args);
                            gImage.ReleaseHdc(ptr);

                            args = new object[2] { ControlStyles.DoubleBuffer, doubleBuffer };
                            setStyleInfo.Invoke(control, args);
                        }
                    }

                    if (mf != null)
                    {
                        GraphicsState state = g.Save();
                        g.SetClip(destRect);
                        g.DrawImage(mf, destRect);
                        g.Restore(state);

                        mf.Dispose();
                        mf = null;
                    }
                }
            }
        }
        #endregion

        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiReportControl reportControl = component as StiReportControl;

            Graphics g = e.Graphics;
            RectangleD rect = reportControl.GetPaintRectangle();

            if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
            {
                if (reportControl.IsDesigning)
                {
                    #region Fill container
                    if (reportControl.Report.Info.FillContainer)
                    {
                        Color color = StiColorUtils.Dark(Color.White, 40);
                        color = Color.FromArgb(100, color);

                        StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                    }
                    #endregion

                    StiDrawing.DrawRectangle(g, Pens.Black, rect.Left, rect.Top, rect.Width, rect.Height);

                    #region Container name
                    using (StringFormat stringFormat = new StringFormat())
                    using (Font font = new Font("Arial", (float)(15 * reportControl.Page.Zoom)))
                    using (Brush brush = new SolidBrush(Color.Gray))
                    {
                        stringFormat.LineAlignment = StringAlignment.Center;
                        stringFormat.Alignment = StringAlignment.Center;

                        StiTextDrawing.DrawString(g, reportControl.Name, font, brush,
                            new RectangleD(rect.Left, rect.Top, rect.Width, rect.Height), stringFormat);

                    }
                    #endregion

                    #region Markers
                    PaintMarkers(component, g, rect);
                    #endregion
                }
            }

            PaintComponents(reportControl, e);
        }
        #endregion
    }
}
