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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiContourTextGdiPainter : StiTextGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var contourText = (StiContourText)component;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!contourText.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                contourText.InvokePainting(contourText, e);

            if (!e.Cancel && (!(contourText.Enabled == false && contourText.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = contourText.GetPaintRectangle();
                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    #region Fill rectangle
                    if (e.DrawBorderFormatting)
                    {
                        if (contourText.Brush is StiSolidBrush &&
                            ((StiSolidBrush) contourText.Brush).Color == Color.Transparent &&
                            contourText.Report.Info.FillComponent &&
                            contourText.IsDesigning)
                        {
                            var color = Color.FromArgb(150, Color.White);
                            StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                        }
                        else
                        {
                            StiDrawing.FillRectangle(g, contourText.Brush, rect);
                        }
                    }
                    #endregion

                    #region Markers
                    if (e.DrawBorderFormatting)
                        PaintMarkers(contourText, g, rect);
                    #endregion

                    #region Border
                    if (contourText.HighlightState == StiHighlightState.Hide)
                    {
                        PaintBorder(contourText, g, rect, contourText.Page.Zoom,
                            e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!contourText.Border.Topmost));
                    }
                    #endregion

                    #region Text
                    if (e.DrawBorderFormatting)
                    {
                        string text = contourText.GetTextInternal();
                        if (contourText.IsDesigning && (!contourText.Report.Info.QuickInfoOverlay) && contourText.Report.Info.QuickInfoType != StiQuickInfoType.None)
                            text = contourText.GetQuickInfo();

                        if (!string.IsNullOrEmpty(text))
                        {
                            var gs = g.Save();
                            double borderSize = contourText.Border.Size / 2;
                            var textRect = rect;
                            textRect.X += borderSize;
                            textRect.Y += borderSize;
                            textRect.Width -= borderSize;
                            textRect.Height -= borderSize;

                            g.SetClip(textRect.ToRectangleF(), CombineMode.Intersect);

                            g.TranslateTransform((float)(textRect.X + textRect.Width / 2),
                                (float)(textRect.Y + textRect.Height / 2));
                            textRect.X = -textRect.Width / 2;
                            textRect.Y = -textRect.Height / 2;

                            if ((contourText.TextOptions.Angle > 45 && contourText.TextOptions.Angle < 135) ||
                                (contourText.TextOptions.Angle > 225 && contourText.TextOptions.Angle < 315))
                                textRect = new RectangleD(textRect.Y, textRect.X, textRect.Height, textRect.Width);

                            using (var gp = new GraphicsPath())
                            using (var sf = StiTextDrawing.GetStringFormat(contourText.TextOptions,
                                contourText.HorAlignment, contourText.VertAlignment, (float)contourText.Page.Zoom))
                            {

                                gp.AddString(
                                    text,
                                    contourText.Font.FontFamily,
                                    (int)contourText.Font.Style,
                                    (float)(contourText.Font.Size * contourText.Page.Zoom * 1.3 * StiScale.Factor),
                                    textRect.ToRectangleF(),
                                    sf);

                                g.RotateTransform((float)contourText.TextOptions.Angle);

                                using (var brush = StiBrush.GetBrush(contourText.TextBrush, textRect.ToRectangleF()))
                                using (var pen = new Pen(contourText.ContourColor))
                                {
                                    g.FillPath(brush, gp);

                                    pen.Width = (float)(contourText.Size * contourText.Page.Zoom * StiScale.Factor);
                                    g.DrawPath(pen, gp);
                                }
                            }
                            g.Restore(gs);

                        }
                    }
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        PaintEvents(contourText, e.Graphics, rect);
                        PaintConditions(contourText, e.Graphics, rect);
                    }
                }
            }
            e.Cancel = false;
            
            if (e.DrawBorderFormatting)
                contourText.InvokePainted(contourText, e);
        }
        #endregion
    }
}
