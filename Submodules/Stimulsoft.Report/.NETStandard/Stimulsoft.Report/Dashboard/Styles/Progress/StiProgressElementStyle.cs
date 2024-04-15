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
using Stimulsoft.Base.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
using Pen = Stimulsoft.Drawing.Pen;
using Brushes = Stimulsoft.Drawing.Brushes;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Dashboard.Styles
{
    public abstract class StiProgressElementStyle : StiElementStyle
    {
        #region Properties
        public abstract string LocalizedName { get; }

        public virtual Color ForeColor { get; set; } = Color.Transparent;

        public abstract Color TrackColor { get; set; }

        public abstract Color BandColor { get; set; }

        public abstract Color[] SeriesColors { get; set; }

        [Browsable(false)]
        public virtual Color BackColor { get; set; } = Color.White;
        #endregion

        #region Methods
        public void DrawStyleForGallery(Graphics g, Rectangle rect, StiProgressElementMode mode)
        {
            var state = g.Save();

            try
            {
                
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;

                var backColor = GetBackColor();
                StiDrawing.FillRectangle(g, backColor, rect);

                switch (mode)
                {
                    #region Circle
                    case StiProgressElementMode.Circle:
                        {
                            var size = (float) Math.Min(rect.Width, rect.Height) - StiScale.XXI(10);
                            var rect1 = new RectangleF(rect.X + (rect.Width - size) / 2, rect.Y + (rect.Height - size) / 2, size, size);
                            using (var pen = new Pen(this.TrackColor, (float)(7 * StiScale.Factor)))
                            {
                                g.DrawEllipse(pen, rect1);
                            }

                            if (this.SeriesColors != null && this.SeriesColors.Length > 0)
                            {
                                g.SetClip(new RectangleF(rect1.X + rect1.Width / 2, rect1.Y - StiScale.YYI(5), rect1.Width / 2 + StiScale.XXI(10), rect1.Height + StiScale.YYI(10)));
                                using (var pen = new Pen(this.SeriesColors[0], (float)(7 * StiScale.Factor)))
                                {
                                    g.DrawEllipse(pen, rect1);
                                }

                                g.ResetClip();

                                g.SetClip(new RectangleF(rect1.X - StiScale.XXI(5), rect1.Y + rect1.Height / 2, rect1.Width / 2 + StiScale.XXI(10), (rect1.Height / 2) + StiScale.YYI(10)));
                                using (var pen = new Pen(this.SeriesColors[0], (float)(7 * StiScale.Factor)))
                                {
                                    g.DrawEllipse(pen, rect1);
                                }

                                g.ResetClip();
                            }

                            rect1.Y += 2;

                            
                            var foreColor = Ident == StiElementStyleIdent.Custom ? this.ForeColor: StiDashboardStyleHelper.GetForeColor(Ident);
                            foreColor = Color.FromArgb(255, foreColor);

                            using (var textFont = new Font("Arial", (float)(14f * StiScale.Factor), FontStyle.Bold, GraphicsUnit.Pixel))
                            using (var textBrush = new SolidBrush(foreColor))
                            using (var sf = new StringFormat())
                            {
                                sf.Alignment = StringAlignment.Center;
                                sf.LineAlignment = StringAlignment.Center;

                                g.DrawString("75%", textFont, textBrush, rect1, sf);
                            }
                        }
                        break;
                    #endregion

                    #region Pie
                    case StiProgressElementMode.Pie:
                        {
                            var size = (float) Math.Min(rect.Width, rect.Height) - StiScale.XXI(10);
                            var rect1 = new RectangleF(rect.X + (rect.Width - size) / 2, rect.Y + (rect.Height - size) / 2, size, size);
                            using (var brush = new SolidBrush(this.TrackColor))
                            {
                                g.FillEllipse(brush, rect1);
                            }

                            if (this.SeriesColors != null && this.SeriesColors.Length > 0)
                            {
                                g.SetClip(new RectangleF(rect1.X + rect1.Width / 2, rect1.Y, rect1.Width / 2, rect1.Height));
                                using (var brush = new SolidBrush(this.SeriesColors[0]))
                                {
                                    g.FillEllipse(brush, rect1);
                                }

                                g.ResetClip();

                                g.SetClip(new RectangleF(rect1.X, rect1.Y + rect1.Height / 2, rect1.Width / 2, rect1.Height / 2));
                                using (var brush = new SolidBrush(this.SeriesColors[0]))
                                {
                                    g.FillEllipse(brush, rect1);
                                }

                                g.ResetClip();
                            }
                        }
                        break;
                    #endregion

                    #region DataBars
                    case StiProgressElementMode.DataBars:
                        {
                            var rect1 = new RectangleF(rect.X + StiScale.XXI(5), rect.Y + StiScale.YYI(5), rect.Width - StiScale.XXI(10), rect.Height - StiScale.YYI(10));
                            using (var brush = new SolidBrush(this.TrackColor))
                            {
                                g.FillRectangle(brush, rect1);
                            }

                            var foreColor = StiDashboardStyleHelper.GetNativeForeColor();
                            using (var textFont = new Font("Arial", (float)(25f * StiScale.Factor), FontStyle.Bold, GraphicsUnit.Pixel))
                            using (var textBrush = new SolidBrush(foreColor))
                            using (var sf = new StringFormat())
                            {
                                sf.Alignment = StringAlignment.Center;
                                sf.LineAlignment = StringAlignment.Center;

                                g.DrawString("75%", textFont, textBrush, rect1, sf);
                            }

                            var rect2 = new RectangleF(rect1.X, rect1.Y, rect1.Width * 0.75f, rect1.Height);
                            if (this.SeriesColors != null && this.SeriesColors.Length > 0)
                            {
                                using (var brush = new SolidBrush(this.SeriesColors[0]))
                                {
                                    g.FillRectangle(brush, rect2);
                                }
                            }
                            
                            g.SetClip(rect2);
                            using (var textFont = new Font("Arial", (float)(25f * StiScale.Factor), FontStyle.Bold, GraphicsUnit.Pixel))
                            using (var sf = new StringFormat())
                            {
                                sf.Alignment = StringAlignment.Center;
                                sf.LineAlignment = StringAlignment.Center;

                                g.DrawString("75%", textFont, Brushes.White, rect1, sf);
                            }

                            g.ResetClip();
                        }
                        break;
                    #endregion
                }
            }
            finally
            {
                g.Restore(state);
            }
        }

        private Color GetBackColor()
        {
            if (Ident == StiElementStyleIdent.Custom)
                return this.BackColor;

            return StiDashboardStyleHelper.GetBackColor(Ident);
        }
        #endregion
    }
}