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
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Units;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Font = Stimulsoft.Drawing.Font;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Matrix = Stimulsoft.Drawing.Drawing2D.Matrix;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Summary description for StiWysiwygTextRender.
    /// </summary>
    public static class StiWysiwygTextRender
    {
        public static void DrawString(Graphics g, RectangleD rect, string text, StiText textBox)
        {
            if (!StiDpiHelper.IsWindows)
            {
                StiTypographicTextRender.DrawString(g, rect, textBox.CheckAllowHtmlTags() ? StiTextRenderer.GetPlainTextFromHtmlTags(text) : text, textBox);
                return;
            }
            if (!StiOptions.Engine.UseOldWYSIWYGTextQuality)
            {
                StiDpiHelper.CheckWysiwygScaling();

                double scale = StiDpiHelper.NeedFontScaling && textBox.IsPrinting ? 1 : 1/StiDpiHelper.GraphicsScale;
                if (StiScale.IsNoScaling)
                {
                    bool isDashboards = (textBox.Page?.TagValue != null) && ((textBox.Page.TagValue as string) == "*Dashboards*" || textBox.Page.TagValue is Base.Drawing.StiAdvancedWatermark);
                    if (!textBox.IsPrinting && StiDpiHelper.NeedFontScaling && !isDashboards)
                    {
                        scale = 1;
                    }
                    else
                    {
                        scale = 1 / StiDpiHelper.GraphicsScale;
                    }
                }

                StiTextRenderer.DrawText(g, text, textBox.Font, rect, StiBrush.ToColor(textBox.TextBrush),
                    Color.Transparent, textBox.LineSpacing * StiOptions.Engine.TextLineSpacingScale, textBox.HorAlignment, textBox.VertAlignment, textBox.WordWrap,
                    textBox.TextOptions.RightToLeft, textBox.Page.Zoom, textBox.Angle, textBox.TextOptions.Trimming,
                    textBox.TextOptions.LineLimit, textBox.CheckAllowHtmlTags(), textBox.TextOptions, scale);
            }
            else
            {
                #region Old mode
                if (textBox.Page.Zoom < 0.1f)
                    StiStandardTextRenderer.DrawString(g, rect, text, textBox);

                else
                {
                    try
                    {
                        #region Metafile
                        Metafile metafile;

                        using (var bmp = new Bitmap(1, 1))
                        using (var grfx = Graphics.FromImage(bmp))
                        {
                            var ipHdc = grfx.GetHdc();
                            metafile = new Metafile(ipHdc, EmfType.EmfOnly);
                            grfx.ReleaseHdc(ipHdc);
                        }
                        #endregion

                        var zoom = (float) (textBox.Page.Zoom * StiDpiHelper.GraphicsScale * StiScale.X);

                        #region Create metafile graphics
                        using (var gMetafile = Graphics.FromImage(metafile))
                        {
                            gMetafile.PageUnit = GraphicsUnit.Pixel;
                            var rect2 = new RectangleD(0, 0, rect.Width / zoom, rect.Height / zoom);
                            StiStandardTextRenderer.DrawString(gMetafile, rect2, text, textBox, 1);
                        }
                        #endregion

                        #region Convert StiTextHorAlignment to StiHorAlignment
                        var align = StiHorAlignment.Left;

                        if (textBox.HorAlignment == StiTextHorAlignment.Center)
                            align = StiHorAlignment.Center;

                        if (textBox.HorAlignment == StiTextHorAlignment.Right)
                            align = StiHorAlignment.Right;
                        #endregion

                        #region Prepare rectangle
                        var unit = GraphicsUnit.Pixel;
                        var size = metafile.GetBounds(ref unit).Size;
                        size.Width += 2;
                        size.Height += 2;

                        var sizeMetafile = new SizeD(size.Width * zoom, size.Height * zoom);
                        var rectMetafile = StiRectangleUtils.AlignSizeInRect(rect, sizeMetafile, align, textBox.VertAlignment);
                        #endregion

                        if (rectMetafile.Width > 1 && rectMetafile.Height > 1)
                        {
                            var rc = rectMetafile.ToRectangleF();
                            rc.Height--;
                            rc.Width--;
                            var clipRect = g.ClipBounds;
                            g.SetClip(rect.ToRectangleF());
                            g.DrawImage(metafile, rc);
                            g.SetClip(clipRect);
                        }

                        metafile.Dispose();
                    }
                    catch
                    {
                    }
                }
                #endregion
            }
        }

        public static SizeD MeasureString(double width, Font font, StiText textBox)
        {
            if (StiOptions.Engine.UseNewHtmlEngine && textBox.CheckAllowHtmlTags())
            {
                SizeD actualSize;
                if (textBox.Angle == 90 || textBox.Angle == 270)
                {
                    actualSize = StiHtmlTextRender.MeasureString(textBox);
                    var temp = actualSize.Width;
                    actualSize.Width = actualSize.Height;
                    actualSize.Height = temp;
                }
                else
                    actualSize = StiHtmlTextRender.MeasureString(textBox);
                return actualSize;
            }

            if (!StiDpiHelper.IsWindows)
            {
                if (textBox.CheckAllowHtmlTags())
                {
                    string text = textBox.Text;
                    textBox.Text = StiTextRenderer.GetPlainTextFromHtmlTags(text);
                    var result = StiTypographicTextRender.MeasureString(width, font, textBox);
                    textBox.Text = text;
                    return result;
                }
                return StiTypographicTextRender.MeasureString(width, font, textBox);
            }
            if (!StiOptions.Engine.UseOldWYSIWYGTextQuality)
            {
                bool wordwrap = textBox.WordWrap || (textBox.HorAlignment == StiTextHorAlignment.Width);

                if (textBox.AutoWidth && !wordwrap)
                    width = 10000000;

                float angle = textBox.Angle % 360;
                if (angle < 0) angle = 360 + angle;

                var rect = new RectangleD(0, 0, width, 10000000);

                var size = StiTextRenderer.MeasureText(textBox.GetMeasureGraphics(), textBox.Text, font, rect, textBox.LineSpacing * StiOptions.Engine.TextLineSpacingScale, wordwrap,
                    textBox.TextOptions.RightToLeft, 1, 0, textBox.TextOptions.Trimming,
                    textBox.TextOptions.LineLimit, textBox.CheckAllowHtmlTags(), textBox.TextOptions, 1);

                if (angle != 0) size = RotateSize(size, angle);

                //correction of the width to prevent rounding error
                if (textBox.Report != null && textBox.Report.ReportUnit != StiReportUnitType.HundredthsOfInch)
                {
                    var unit = textBox.Report.Unit;
                    var fullWidth = size.Width + textBox.Margins.Left + textBox.Margins.Right + textBox.Border.Size;

                    if (unit.ConvertToHInches(Math.Round(unit.ConvertFromHInches(fullWidth), 2)) < fullWidth)
                        size.Width += unit.ConvertToHInches(0.01);
                }

                if (angle == 90 || angle == 180)
                    size.Width *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorWysiwyg;

                else
                    size.Height *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorWysiwyg;

                return new SizeD(size.Width, size.Height);
            }
            else
            {
                #region Old mode
                lock (textBox.GetMeasureGraphics())
                {
                    var size = StiTextDrawing.MeasureString(textBox.GetMeasureGraphics(), textBox.Text,
                        font, width, textBox.TextOptions,
                        textBox.HorAlignment, textBox.VertAlignment, false);

                    size.Height *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorStandard;
                    return size;
                }
                #endregion
            }
        }

        private static SizeD RotateSize(SizeD size, float angle)
        {
            float sx = (float)(size.Width / 2);
            float sy = (float)(size.Height / 2);

            var points = new[]
            {
                new PointF(-sx, -sy),
                new PointF(sx, -sy),
                new PointF(sx, sy),
                new PointF(-sx, sy)
            };

            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            matrix.TransformPoints(points);

            double minX = Math.Min(points[3].X, Math.Min(points[2].X, Math.Min(points[1].X, points[0].X)));
            double minY = Math.Min(points[3].Y, Math.Min(points[2].Y, Math.Min(points[1].Y, points[0].Y)));
            double maxX = Math.Max(points[3].X, Math.Max(points[2].X, Math.Max(points[1].X, points[0].X)));
            double maxY = Math.Max(points[3].Y, Math.Max(points[2].Y, Math.Max(points[1].Y, points[0].Y)));

            return new SizeD(maxX - minX, maxY - minY);
        }
    }
}
