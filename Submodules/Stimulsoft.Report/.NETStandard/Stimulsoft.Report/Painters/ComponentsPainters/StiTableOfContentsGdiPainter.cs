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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Brush = Stimulsoft.Drawing.Brush;
using Brushes = Stimulsoft.Drawing.Brushes;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiTableOfContentsGdiPainter : StiBandGdiPainter
    {
        #region Methods
        public override void PaintBandContent(StiBand band, Graphics g, RectangleD rect)
        {
            var table = band as StiTableOfContents;
            var isFirstInReport = table.IsFirstInReport || (!table.Enabled);

            if (isFirstInReport)
                PaintTableStyles(g, table, rect);

            else
                PaintContainerName(g, table, rect);
        }

        private void PaintContainerName(Graphics g, StiTableOfContents table, RectangleD rect)
        {
            if (rect.Width < 100 || rect.Height < 50 || table.Report.Info.IsComponentsMoving) return;

            var fontSize = (float)(8 * table.Page.Zoom);

            using (var stringFormat = new StringFormat())
            using (var font = new Font("Segoe UI", fontSize))
            using (var brush = new SolidBrush(table.Enabled ? Color.FromArgb(102, 99, 99) : StiColor.Get("bfbfbf")))
            {
                stringFormat.LineAlignment = StringAlignment.Center;
                stringFormat.Alignment = StringAlignment.Center;

                if (table.RightToLeft)
                    stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

                var str = Loc.Get("Errors", "OneTableOfContentsAllowed");

                StiTextDrawing.DrawString(g, str, font, brush, new RectangleD(rect.Left, rect.Top, rect.Width, rect.Height), stringFormat);
            }
        }

        private void PaintTableStyles(Graphics g, StiTableOfContents table, RectangleD rect)
        {
            var state = g.Save();
            g.SetClip(rect.ToRectangleF());

            rect = ApplyMargins(table, rect);

            try
            {
                var posX = rect.X;
                var posY = rect.Y;
                var width = rect.Width;
                var index = 1;

                var styles = table.GetStylesList();
                if (!styles.Any())
                    styles.Add(new StiStyle($"{Loc.GetMain("Heading")}1"));

                foreach (var style in styles)
                {
                    var isLastStyle = styles.IndexOf(style) == styles.Count - 1;
                    var styleCount = isLastStyle ? 3 : 1;
                    for (var styleIndex = 0; styleIndex < styleCount; styleIndex++)
                    {
                        var height = style.Font.GetHeight() * table.Page.Zoom * 1.3 * style.LineSpacing;

                        var name = $"{Loc.GetMain("Heading")}{index}{DotsString}";
                        var styleRect = new RectangleD(posX, posY, width, height);
                        PaintStyleItem(g, table, styleRect, style, name, index, (float)table.Page.Zoom);

                        if (!isLastStyle)
                        {
                            if (!table.RightToLeft)
                                posX += Indent(table);

                            width -= Indent(table);
                        }

                        posY += height;
                        index++;

                        if (posY > rect.Bottom) break;
                    }
                }
            }
            finally
            {
                g.Restore(state);
            }
        }

        private void PaintStyleItem(Graphics g, StiTableOfContents table, RectangleD rect, StiStyle style, string name, int index, float scale)
        {
            var borderRect = rect.ToRectangleF();
            if (style.AllowUseBrush)
            {
                using (Brush brush = StiBrush.GetBrush(style.Brush, rect))
                {
                    StiDrawing.FillRectangle(g, brush, rect);
                }
            }
            else
            {
                rect.Width++;
                rect.Height++;
                StiDrawing.FillRectangle(g, Brushes.White, rect);
                rect.Width--;
                rect.Height--;
            }


            #region Draw Name of Style
            using (var sf = new StringFormat())
            using (var textBrush = StiBrush.GetBrush(style.TextBrush, rect))
            {
                sf.Trimming = StringTrimming.EllipsisCharacter;                
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.None;

                if (table.RightToLeft)
                    sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

                var br = textBrush;
                if (!style.AllowUseTextBrush)
                    br = Brushes.Black;

                var oldMode = g.SmoothingMode;
                try
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    if (style.AllowUseFont)
                    {
                        using (var font = style.AllowUseFont
                            ? new Font(style.Font.Name, (float)(style.Font.Size * scale), style.Font.Style)
                            : new Font("Arial", (float)(8 * scale)))
                        {
                            var indexSize = g.MeasureString(index.ToString(), font);
                            var indexRect = table.RightToLeft
                                ? new RectangleD(rect.X, rect.Top, indexSize.Width, rect.Height)
                                : new RectangleD(rect.Right - indexSize.Width, rect.Top, indexSize.Width, rect.Height);

                            sf.Alignment = StringAlignment.Far;
                            g.DrawString(index.ToString(), font, br, indexRect.ToRectangleF(), sf);

                            var nameRect = table.RightToLeft
                                ? new RectangleD(rect.Left + indexSize.Width, rect.Top, rect.Width - indexSize.Width, rect.Height)
                                : new RectangleD(rect.Left, rect.Top, rect.Width - indexSize.Width, rect.Height);

                            sf.Alignment = StringAlignment.Near;
                            g.DrawString(name, font, br, nameRect.ToRectangleF(), sf);
                        }
                    }
                }
                finally
                {
                    g.SmoothingMode = oldMode;
                }
            }
            #endregion

            if (style.Border != null && style.AllowUseBorderFormatting || style.AllowUseBorderSides)
                style.Border.Draw(g, borderRect, 1, Color.White, style.AllowUseBorderFormatting, style.AllowUseBorderSides);
        }
        
        private double Indent(StiTableOfContents table)
        {
            return table.Report.Unit.ConvertFromHInches((double)table.Indent) * table.Page.Zoom * StiScale.Factor;
        }

        private static RectangleD ApplyMargins(StiTableOfContents table, RectangleD rect)
        {
            var margins = table.Margins;

            rect.X += margins.Left * table.Page.Zoom;
            rect.Width -= margins.Left * table.Page.Zoom;

            rect.Y += margins.Top * table.Page.Zoom;
            rect.Height -= margins.Top * table.Page.Zoom;

            rect.Width -= margins.Right * table.Page.Zoom;
            rect.Height -= margins.Bottom * table.Page.Zoom;
            return rect;
        }
        #endregion

        #region Properties
        private string dotsString;
        private string DotsString
        {
            get
            {
                if (dotsString == null)
                    dotsString = new string('.', 300);

                return dotsString;
            }
        }
        #endregion
    }
}