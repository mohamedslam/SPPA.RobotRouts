#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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
using Stimulsoft.Base.Dashboard;
using Stimulsoft.Base.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
using Font = Stimulsoft.Drawing.Font;
using Brushes = Stimulsoft.Drawing.Brushes;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Dashboard.Styles
{
    public abstract class StiControlElementStyle : StiElementStyle
    {
        #region Properties
        public abstract string LocalizedName { get; }

        public virtual Color BackColor { get; set; } = StiElementConsts.BackgroundColor;

        public virtual Color ForeColor { get; set; } = StiElementConsts.ForegroundColor;

        public virtual Color GlyphColor { get; set; } = Color.DimGray;

        public virtual Color SeparatorColor { get; set; } = Color.LightGray;
        
        public virtual Color SelectedBackColor { get; set; } = StiColor.Get("3498db");

        public virtual Color SelectedForeColor { get; set; } = Color.White;

        public virtual Color SelectedGlyphColor { get; set; } = Color.White;

        public virtual Color HotBackColor { get; set; } = StiColorUtils.Light(Color.LightGray, 15);

        public virtual Color HotForeColor { get; set; } = StiElementConsts.ForegroundColor;

        public virtual Color HotGlyphColor { get; set; } = StiElementConsts.ForegroundColor;

        public virtual Color HotSelectedBackColor { get; set; } = StiColorUtils.Light("3498db", 30);

        public virtual Color HotSelectedForeColor { get; set; } = Color.White;

        public virtual Color HotSelectedGlyphColor { get; set; } = Color.White;

        public Font Font { get; set; } = new Font("Arial", 8);
        #endregion

        #region Methods
        public void DrawStyleForGallery(Graphics g, Rectangle rect)
        {
            g.SetClip(rect);
            g.FillRectangle(Brushes.White, rect);

            rect.Inflate((int) (-10 * StiScale.Factor), (int) (-10 * StiScale.Factor));

            var size = StiScale.XXI(30);
            rect = new Rectangle(rect.X, rect.Y + (rect.Height - size) / 2, rect.Width, size);

            StiDrawing.FillRectangle(g, BackColor, rect);

            var selectedRect = new RectangleF(rect.Right - rect.Height, rect.Y, rect.Height, rect.Height);
            selectedRect.Inflate((int)(-3 * StiScale.Factor), (int)(-3 * StiScale.Factor));

            StiDrawing.FillRectangle(g, SelectedBackColor, selectedRect);

            g.DrawRectangle(Pens.LightGray, rect);
            PaintButton(g, selectedRect);

            using (var font = new Font("Arial", 8f))
            using (var sf = new StringFormat())
            {
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;

                var textRect = new Rectangle(StiScale.I6 + rect.X, rect.Y, rect.Width - StiScale.XXI(26), rect.Height);

                StiDrawing.DrawString(g, LocalizedName, font, ForeColor, textRect, sf);

                g.ResetClip();
            }
        }

        private void PaintButton(Graphics g, RectangleF rect)
        {
            var size = rect.Height;
            var buttonRect = new RectangleF(rect.Right - size, rect.Y, size, rect.Height);

            var posX = buttonRect.X + buttonRect.Width / 2;
            var posY = buttonRect.Y + buttonRect.Height / 2;

            using (var path = new GraphicsPath())
            {
                var i1 = StiScale.I1;
                var i2 = StiScale.I2;
                var i3 = StiScale.I3;

                path.AddLine(posX - i3, posY - i2, posX - i3, posY - i1);
                path.AddLine(posX - i3, posY - i1, posX, posY + i2);
                path.AddLine(posX, posY + i2, posX + i3, posY - i1);
                path.AddLine(posX + i3, posY - i1, posX + i3, posY - i2);

                using (var pen = new Pen(GlyphColor, 2))
                {
                    pen.StartCap = LineCap.Flat;
                    pen.EndCap = LineCap.Flat;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.SetClip(new RectangleF(posX - i3, posY - i2, StiScale.I7, StiScale.I6));
                    g.DrawPath(pen, path);
                    g.ResetClip();
                }
            }
        }
        #endregion
    }
}