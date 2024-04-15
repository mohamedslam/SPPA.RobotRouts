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
using Stimulsoft.Report.Events;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiWinControlGdiPainter : StiComponentGdiPainter
    {
        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var winControl = (StiWinControl)component;
            return winControl.GetControlImage();
        }


        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var winControl = (StiWinControl)component;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!winControl.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                winControl.InvokePainting(winControl, e);

            if (!e.Cancel && (!(winControl.Enabled == false && winControl.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = winControl.GetPaintRectangle();
                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    #region Fill rectangle
                    if (e.DrawBorderFormatting)
                    {
                        if (winControl.Control == null && winControl.Report.Info.FillComponent && winControl.IsDesigning)
                        {
                            Color color = Color.FromArgb(150, Color.White);

                            StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                        }
                    }
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        if (winControl.Control != null)
                        {
                            using (var image = winControl.GetControlImage())
                            {
                                if (image != null) 
                                    g.DrawImage(image, rect.ToRectangle());
                            }
                        }
                    }

                    #region Markers
                    if (e.DrawBorderFormatting)
                        PaintMarkers(winControl, g, rect);
                    #endregion

                    #region Border
                    var borderRect = rect.ToRectangle();
                    if (winControl.HighlightState == StiHighlightState.Hide)
                    {
                        PaintBorder(winControl, g, RectangleD.CreateFromRectangle(borderRect), winControl.Page.Zoom, 
                            e.DrawBorderFormatting, e.DrawTopmostBorderSides);
                    }
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        PaintEvents(winControl, e.Graphics, rect);
                        PaintConditions(winControl, e.Graphics, rect);
                    }
                }
            }

            e.Cancel = false;
            if (e.DrawBorderFormatting)
                winControl.InvokePainted(winControl, e);
        }
        #endregion
    }
}