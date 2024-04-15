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
using Stimulsoft.Report.Dialogs;
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
using Font = Stimulsoft.Drawing.Font;
using Brush = Stimulsoft.Drawing.Brush;
using Pen = Stimulsoft.Drawing.Pen;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using TextureBrush = Stimulsoft.Drawing.TextureBrush;
using LinearGradientBrush = Stimulsoft.Drawing.Drawing2D.LinearGradientBrush;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiFormGdiPainter : StiPageGdiPainter
    {
        #region Methods
        private Rectangle GetOffsetRect(StiForm form, Rectangle rect, int minWidth, int minHeight)
        {
            if (form.IsSelected)
            {
                rect.Width = Math.Max(rect.Width + (int)(form.OffsetRectangle.Width * StiScale.Factor), minWidth);
                rect.Height = Math.Max(rect.Height + (int)(form.OffsetRectangle.Height * StiScale.Factor), minHeight);
            }
            else
            {
                rect.Width = Math.Max(rect.Width, minWidth);
                rect.Height = Math.Max(rect.Height, minHeight);
            }
            return rect;
        }
        #endregion

        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var form = component as StiForm;
            var g = e.Graphics;

            var mgLeft = (int)(form.Margins.Left * StiScale.Factor);
            var mgTop = (int)(form.Margins.Top * StiScale.Factor);

            var pgWidth = StiScale.XXI(form.DisplayRectangle.Width);
            var pgHeight = StiScale.YYI(form.DisplayRectangle.Height);

            if (!e.ClipRectangle.IsEmpty)
            {
                g.ResetClip();
                g.SetClip(e.ClipRectangle.ToRectangleF());
            }

            var rect = GetOffsetRect(form, new Rectangle(0, 0, pgWidth, pgHeight), StiScale.XXI(112), StiScale.YYI(32));

            ControlPaint.DrawBorder3D(g, rect, Border3DStyle.Raised, Border3DSide.All);

            #region Draw Title
            var titleHeight = StiScale.YYI(20);
            var titleRect = new Rectangle(StiScale.I2, StiScale.I2, pgWidth - StiScale.I4, (int)titleHeight - StiScale.I2);

            if (form.IsSelected)
                titleRect.Width = Math.Max(titleRect.Width + (int)(form.OffsetRectangle.Width * StiScale.Factor), StiScale.YYI(108));

            g.FillRectangle(
                new LinearGradientBrush(titleRect, StiColors.ActiveCaptionEnd, StiColors.ActiveCaptionStart, 0f),
                titleRect);

            titleRect.X += StiScale.I2;
            using (var font = new Font("Arial", 8, FontStyle.Bold))
            using (var brush = new SolidBrush(SystemColors.ActiveCaptionText))
            using (var sf = new StringFormat())
            {
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.EllipsisCharacter;

                if (form.RightToLeft == RightToLeft.Yes)
                    sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

                g.DrawString(form.Text, font, brush, titleRect, sf);
            }
            #endregion

            var clipRect = GetOffsetRect(form, StiScale.I(form.ClientRectangle.ToRectangle()), StiScale.I(104), StiScale.I8);
            clipRect.Width++;
            clipRect.Height++;
            clipRect.Y += StiScale.YYI(20);
            clipRect.X += StiScale.I4;
            g.SetClip(clipRect);

            #region Fill Content
            if (form.Report.Info.ShowGrid)
            {
                var grid = StiScale.I(form.GridSize);
                var bmp = new Bitmap(grid, grid);
                using (var gg = Graphics.FromImage(bmp))
                {
                    using (Brush brush = new SolidBrush(form.BackColor))
                    {
                        gg.FillRectangle(brush, 0, 0, grid, grid);
                    }
                }
                bmp.SetPixel(0, 0, SystemColors.ControlDarkDark);

                var textureRect = clipRect;
                g.TranslateTransform(textureRect.X, textureRect.Y);

                textureRect.X = 0;
                textureRect.Y = 0;

                var textureBrush = new TextureBrush(bmp);
                g.FillRectangle(textureBrush, textureRect);
                g.ResetTransform();
            }
            #endregion

            g.TranslateTransform(mgLeft, mgTop);

            PaintTableLines(form, g);

            var buffer = new Bitmap(clipRect.Width, clipRect.Height);
            using (var gBuffer = Graphics.FromImage(buffer))
            {
                PaintComponents(form, new StiPaintEventArgs(gBuffer, e.ClipRectangle));
                gBuffer.ResetClip();

                if (form.Report.Info.ShowOrder && form.IsDesigning)
                    PaintOrderAndQuickInfo(form, gBuffer, "");
            }

            g.DrawImage(buffer, 0, 0, clipRect.Width, clipRect.Height);
            g.ResetClip();

            #region Draw Dimension Lines
            if (form.Report.Info.ShowDimensionLines && form.IsDesigning && form.Report.Info.CurrentAction != StiAction.None)
                StiDimensionLinesHelper.DrawDimensionLines(g, form);
            #endregion

            #region Draw Selected Rectangle
            if (!form.SelectedRectangle.IsEmpty)
            {
                using (var brush = new SolidBrush(Color.FromArgb(75, Color.Blue)))
                using (var pen = new Pen(Color.DimGray))
                {
                    pen.DashStyle = DashStyle.Dash;

                    var rectSelect = form.SelectedRectangle.ToRectangleF();
                    g.FillRectangle(brush, rectSelect);
                    g.DrawRectangle(pen, rectSelect.X, rectSelect.Y, rectSelect.Width, rectSelect.Height);
                }
            }
            #endregion

            PaintSelection(form, e);
        }
        #endregion
    }
}
