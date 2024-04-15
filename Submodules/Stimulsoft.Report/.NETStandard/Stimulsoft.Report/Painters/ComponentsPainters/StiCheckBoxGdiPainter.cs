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
using Graphics = Stimulsoft.Drawing.Graphics;
using Brushes = Stimulsoft.Drawing.Brushes;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiCheckBoxGdiPainter : StiComponentGdiPainter
    {
        #region Methods
        public static void PaintCheck(StiCheckBox checkBox, Graphics g, RectangleD rect, float zoom)
        {
            bool isCheckedValue = true;
            bool isUncheckedValue = false;
            if (!(checkBox.IsDesigning && (!checkBox.Report.IsPageDesigner)))
            {
                isCheckedValue = checkBox.IsChecked();
                isUncheckedValue = checkBox.IsUnchecked();
            }

            if (isCheckedValue || isUncheckedValue)
            {
                var checkStyleSelected =
                    isCheckedValue ? checkBox.CheckStyleForTrue : checkBox.CheckStyleForFalse;

                StiComponentPainter.DrawCheckStyle(g, rect, checkStyleSelected, checkBox, zoom);
            }
        }
        #endregion

        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            //Null checking
            if (component.Report == null) return null;

            var checkBox = component as StiCheckBox;

            if (checkBox.Report.Unit.ConvertToHInches(component.Width) * zoom < 40)
            {
                zoom *= 2;
            }

            double resZoom = checkBox.Report.Info.Zoom;
            checkBox.Report.Info.Zoom = zoom;

            //var rect = view.GetPaintRectangle();  //fix, GetPaintRectangle now apply StiScale.Factor, so use ClientRectangle
            var rect = component.ComponentToPage(component.ClientRectangle).Normalize();
            rect = checkBox.Report.Unit.ConvertToHInches(rect).Multiply(zoom);

            rect.X = 0;
            rect.Y = 0;
            checkBox.Report.Info.Zoom = resZoom;

            int imageWidth = (int)rect.Width + 1;
            int imageHeight = (int)rect.Height + 1;

            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                if (format != StiExportFormat.ImagePng)
                {
                    g.FillRectangle(Brushes.White, new Rectangle(0, 0, imageWidth, imageHeight));
                }
                StiDrawing.FillRectangle(g, checkBox.Brush, rect);

                PaintCheck(checkBox, g, rect, zoom);
            }
            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var checkBox = component as StiCheckBox;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!checkBox.Border.Topmost)) return;

            checkBox.InvokePainting(checkBox, e);

            if (!e.Cancel && (!(checkBox.Enabled == false && checkBox.IsDesigning == false)))
            {
                var g = e.Graphics;
                var rect = checkBox.GetPaintRectangle();
                double zoom = checkBox.Page.Zoom;

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    #region Fill rectangle
                    if (e.DrawBorderFormatting)
                    {
                        if (checkBox.Brush is StiSolidBrush && ((StiSolidBrush)checkBox.Brush).Color == Color.Transparent &&
                            checkBox.Report.Info.FillComponent && checkBox.IsDesigning)
                        {
                            Color color = Color.FromArgb(150, Color.White);

                            StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                        }
                        else StiDrawing.FillRectangle(g, checkBox.Brush, rect);
                    }
                    #endregion

                    #region Check
                    if (e.DrawBorderFormatting)
                    {
                        if (checkBox.IsPrinting)
                        {
                            float zm = (float)zoom;
                            using (var bmp = checkBox.GetImage(ref zm, StiExportFormat.ImagePng))
                            {
                                g.DrawImage(bmp, rect.ToRectangleF());
                            }
                        }
                        else PaintCheck(checkBox, g, rect, (float)zoom);
                    }
                    #endregion

                    #region Markers
                    if (e.DrawBorderFormatting)
                        PaintMarkers(checkBox, g, rect);
                    #endregion

                    #region Border
                    if (checkBox.HighlightState == StiHighlightState.Hide)
                        PaintBorder(checkBox, g, rect, zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!checkBox.Border.Topmost));
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        PaintEvents(checkBox, e.Graphics, rect);
                        PaintConditions(checkBox, e.Graphics, rect);
                    }
                }
            }

            e.Cancel = false;
            checkBox.InvokePainted(checkBox, e);
        }
        #endregion
    }
}