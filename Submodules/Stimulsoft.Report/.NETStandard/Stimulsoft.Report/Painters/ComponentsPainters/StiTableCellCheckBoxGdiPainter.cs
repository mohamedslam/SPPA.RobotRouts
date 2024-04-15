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
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Base;
using Stimulsoft.Report.Images;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Brushes = Stimulsoft.Drawing.Brushes;
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiTableCellCheckBoxGdiPainter : StiComponentGdiPainter
    {
        #region Methods
        public static void PaintCheck(StiTableCellCheckBox checkBox, Graphics g, RectangleD rect, float zoom)
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
                StiCheckStyle checkStyleSelected =
                    isCheckedValue ? checkBox.CheckStyleForTrue : checkBox.CheckStyleForFalse;

                StiComponentPainter.DrawCheckStyle(g, rect, checkStyleSelected, checkBox, zoom);
            }
        }
        #endregion

        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var checkBox = (StiTableCellCheckBox)component;

            double resZoom = checkBox.Report.Info.Zoom;
            checkBox.Report.Info.Zoom = zoom;
            var rect = checkBox.GetPaintRectangle();
            if (rect.Width < 40)
            {
                zoom *= 2;
                checkBox.Report.Info.Zoom = zoom;
                rect = checkBox.GetPaintRectangle();
            }
            rect.X = 0;
            rect.Y = 0;
            checkBox.Report.Info.Zoom = resZoom;

            int imageWidth = (int)rect.Width + 2;
            int imageHeight = (int)rect.Height + 2;

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

        private void DrawCellImage(Graphics g, RectangleD rect)
        {
            if (rect.Width <= StiScale.I9 || rect.Height <= StiScale.I9) return;

            //Don't dispose image - its cached
            var img = StiReportImages.Styles.CheckBox();            
            g.DrawImageUnscaled(img, new Rectangle((int)rect.X, (int)rect.Y, img.Width, img.Height));
            
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var checkBox = (StiTableCellCheckBox)component;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!checkBox.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
            {
                checkBox.InvokePainting(checkBox, e);

                if (!checkBox.Enabled)
                {
                    e.Cancel = false;
                    checkBox.InvokePainted(checkBox, e);
                    return;
                }
            }

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
                        if (checkBox.Brush is StiSolidBrush &&
                            ((StiSolidBrush)checkBox.Brush).Color == Color.Transparent &&
                            checkBox.Report.Info.FillComponent &&
                            checkBox.IsDesigning)
                        {
                            Color color = Color.FromArgb(150, Color.White);
                            StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                        }
                        else StiDrawing.FillRectangle(g, checkBox.Brush, rect);

                        if (component.IsSelected)
                        {
                            Color selectColor = Color.FromArgb(80, Color.FromArgb(168, 205, 241));
                            StiDrawing.FillRectangle(g, selectColor, rect);
                        }
                    }
                    #endregion

                    #region Check
                    if (e.DrawBorderFormatting)
                    {
                        if (checkBox.IsPrinting)
                        {
                            float zm = (float)zoom;
                            using (Image bmp = checkBox.GetImage(ref zm, StiExportFormat.ImagePng))
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

                    #region Image
                    if (e.DrawBorderFormatting)
                    {
                        if (checkBox.IsDesigning)
                            DrawCellImage(g, rect);
                    }
                    #endregion

                    #region Border
                    if (checkBox.HighlightState == StiHighlightState.Hide)
                    {
                        PaintBorder(checkBox, g, rect, zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!checkBox.Border.Topmost));
                    }
                    #endregion

                    if (e.DrawBorderFormatting)
                    {
                        PaintEvents(checkBox, e.Graphics, rect);
                        PaintQuickButtons(checkBox, e.Graphics);
                        PaintConditions(checkBox, e.Graphics, rect);
                    }
                }
            }

            e.Cancel = false;
            if (e.DrawBorderFormatting)
                checkBox.InvokePainted(checkBox, e);
        }

        public override void PaintSelection(StiComponent component, StiPaintEventArgs e)
        {
            var g = e.Graphics;
            if (component.IsDesigning && component.IsSelected && !component.Report.Info.IsComponentsMoving)
            {
                var rect = component.GetPaintRectangle();

                var size = StiScale.I2;

                using (var brush = new SolidBrush(Color.DimGray))
                {
                    StiDrawing.DrawSelectedRectangle(g, size, brush, rect);
                }
            }
        }
        #endregion
    }
}
