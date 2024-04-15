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
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiRichTextBoxControlGdiPainter : StiReportControlGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiRichTextBoxControl richText = component as StiRichTextBoxControl;
            richText.InvokePainting(richText, e);

            if (!e.Cancel && (!(richText.Enabled == false && richText.IsDesigning == false)))
            {
                Graphics g = e.Graphics;

                RectangleD rect = richText.GetPaintRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    using (SolidBrush brush = new SolidBrush(richText.BackColor))
                    {
                        g.FillRectangle(brush, rect.ToRectangle());
                    }

                    Rectangle borderRect = rect.ToRectangle();
                    ControlPaint.DrawBorder3D(g, borderRect, Border3DStyle.Sunken);

                    rect.X += 4;
                    rect.Y += 4;
                    rect.Width -= 8;
                    rect.Height -= 8;

                    richText.RenderMetafile();

                    if (richText.Image != null)
                    {
                        float imageWidth = (float)(rect.Width / richText.Page.Zoom);
                        float imageHeight = (float)(rect.Height / richText.Page.Zoom);

                        try
                        {
                            g.DrawImage(richText.Image, rect.ToRectangleF(),
                                new RectangleF(0, 0, imageWidth, imageHeight), GraphicsUnit.Pixel);
                        }
                        catch
                        {
                        }
                    }

                    PaintEvents(richText, g, rect);
                    PaintConditions(richText, g, rect);
                }
            }
            e.Cancel = false;
            richText.InvokePainted(richText, e);
        }
        #endregion
    }
}
