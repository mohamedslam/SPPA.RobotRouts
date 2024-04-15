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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.MathFormula;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helpers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using Font = Stimulsoft.Drawing.Font;
using Pen = Stimulsoft.Drawing.Pen;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Brushes = Stimulsoft.Drawing.Brushes;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public partial class StiMathFormulaGdiPainter : StiComponentGdiPainter
    {
        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var mathFormula = (StiMathFormula)component;

            double resZoom = mathFormula.Report.Info.Zoom;
            mathFormula.Report.Info.Zoom = zoom;

            //var rect = mathFormula.GetPaintRectangle();//fix, GetPaintRectangle now apply StiScale.Factor, so use ClientRectangle
            var rect = component.ComponentToPage(component.ClientRectangle).Normalize();
            rect = mathFormula.Report.Unit.ConvertToHInches(rect).Multiply(zoom);

            rect.X = 0;
            rect.Y = 0;

            int imageWidth = (int)rect.Width + 2;
            int imageHeight = (int)rect.Height + 2;

            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                if (format == StiExportFormat.ImagePng)
                {
                    g.Clear(Color.Transparent);
                }
                else
                {
                    g.Clear(Color.White);
                }
                Draw(mathFormula, g, rect.ToRectangleF());
            }

            mathFormula.Report.Info.Zoom = resZoom;

            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides) return;

            var mathFormula = (StiMathFormula)component;
            
            if (e.DrawBorderFormatting)
                mathFormula.InvokePainting(mathFormula, e);

            if (!e.Cancel && (!(mathFormula.Enabled == false && mathFormula.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = mathFormula.GetPaintRectangle();
                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    #region Fill rectangle
                    if (e.DrawBorderFormatting)
                        PaintBackground(mathFormula, g, rect);
                    #endregion

                    if (e.DrawBorderFormatting)
                        Draw(mathFormula, g, rect.ToRectangleF());

                    #region Markers
                    if (e.DrawBorderFormatting)
                        PaintMarkers(mathFormula, g, rect);
                    #endregion

                    #region Border
                    PaintBorder(mathFormula, g, rect, (float)mathFormula.Page.Zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!mathFormula.Border.Topmost));
                    #endregion

                    if (e.DrawBorderFormatting)
                        PaintEvents(mathFormula, e.Graphics, rect);
                }
            }
            
            e.Cancel = false;

            if (e.DrawBorderFormatting)
                mathFormula.InvokePainted(mathFormula, e);
        }

        public virtual void PaintBackground(StiMathFormula mathFormul, Graphics g, RectangleD rect)
        {
            if (mathFormul.Brush is StiSolidBrush &&
                ((StiSolidBrush)mathFormul.Brush).Color.A == 0 &&
                mathFormul.Report.Info.FillComponent &&
                mathFormul.IsDesigning)
            {
                var color = Color.FromArgb(150, Color.White);

                StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
            }
            else
                StiDrawing.FillRectangle(g, mathFormul.Brush, rect);
        }

        private void Draw(StiMathFormula mathFormula, Graphics g, RectangleF rect)
        {
            if (StiMathHelper.MathAssembly == null)
            {
                #region Draw "IsNotFound"
                using (var font = new Font("Arial", 8, FontStyle.Bold))
                using (var sf = StringFormat.GenericDefault.Clone() as StringFormat)
                using (var pen = new Pen(Color.Gray))
                {
                    pen.DashStyle = DashStyle.Dash;

                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    var state = g.Save();
                    g.SetClip(rect, CombineMode.Intersect);
                    g.TranslateTransform((int)rect.X, (int)rect.Y);
                    g.Clear(Color.White);

                    g.DrawString(string.Format(StiLocalization.Get("Notices", "IsNotFound"), StiMathHelper.NameAssembly), font, Brushes.Gray, new RectangleF(0, 0, rect.Width, rect.Height), sf);

                    g.Restore(state);
                }
                #endregion
            }
            else
            {
                var value = mathFormula.Report != null && mathFormula.Report.IsDesigning ? mathFormula.LaTexExpression : mathFormula.Value;

                if (string.IsNullOrEmpty(value))
                    PaintNoDefinedStatus(g, rect, mathFormula, "StiMathFormula", Loc.GetMain("NoElements"));

                else
                    DrawMath(mathFormula, g, rect);
            }
        }

        private static void DrawMath(StiMathFormula mathFormula, Graphics g, RectangleF rect)
        {
            var image = StiMathFormulaCache.GetImage(mathFormula);
            bool needRedrawUnscaled = mathFormula.IsExporting && (image == null || (int)rect.Width != image.Width);
            if (image != null && !needRedrawUnscaled)
            {
                g.DrawImage(image, rect.X, rect.Y);
            }
            else
            {
                try
                {
                    var formula = mathFormula.Report != null && mathFormula.Report.IsDesigning ? mathFormula.LaTexExpression : mathFormula.Value;

                    if (string.IsNullOrEmpty(formula))
                        return;

                    var zoom = mathFormula.Report.Info.Zoom;

                    //fix for different versions of Svg.dll
                    float scale = 1;
                    if (string.IsNullOrEmpty(Stimulsoft.Base.Helpers.StiSvgHelper.SvgAssemblyPrefix))
                    {
                        // Svg.dll
                        scale = needRedrawUnscaled ? (float)StiDpiHelper.GraphicsScale : 1;
                    }
                    else
                    {
                        // Stimulsoft.Svg.dll
                        scale = needRedrawUnscaled ? 1 : 1 / (float)StiDpiHelper.GraphicsScale;
                    }

                    var fontSize = mathFormula.Font.Size * (float)zoom * scale;
                    var colorHex = StiColorFXHelper.HexConverter(StiBrush.ToColor(mathFormula.TextBrush));

                    var svgMath = StiMathFormulaHelper.GetSvgText(formula, fontSize, colorHex);
                    var svgRect = StiMathFormulaHelper.GetSvgRect(mathFormula, svgMath, rect);

                    StiSvgHelper.DrawWithSvg(svgMath, rect, svgRect, mathFormula.Report.Info.Zoom, g);
                    if (!needRedrawUnscaled)
                    {
                        StiMathFormulaCache.SetImage(mathFormula, svgMath, rect);
                    }
                }
                catch { }
            }
        }
        #endregion
    }
}
