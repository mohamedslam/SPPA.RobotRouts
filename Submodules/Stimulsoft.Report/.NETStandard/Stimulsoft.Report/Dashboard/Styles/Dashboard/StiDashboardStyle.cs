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
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Dashboard.Styles
{
    public abstract class StiDashboardStyle : StiElementStyle
    {
        #region Properties
        public abstract string LocalizedName { get; }

        public abstract Color ForeColor { get; set; }

        public virtual Color BackColor { get; set; } = Color.White;

        public abstract Color TitleBackColor { get; set; }

        public abstract Color TitleForeColor { get; set; }

        public virtual Color BorderColor
        {
            get
            {
                return TitleBackColor;
            }
            set
            {

            }
        }
        #endregion

        #region Methods
        public void DrawStyleForGallery(Graphics g, Rectangle rect)
        {
            StiDrawing.FillRectangle(g, Color.White, rect);

            var rectAll = new Rectangle(rect.X + StiScale.I5, rect.Y + StiScale.I5, rect.Width - StiScale.I10, rect.Height - StiScale.I10);
            var rectTop = new Rectangle(rectAll.Location, new Size(rectAll.Width, rectAll.Height - StiScale.YYI(16)));
            var rectBottom = new Rectangle(rectTop.X, rectTop.Bottom, rectTop.Width, StiScale.YYI(16));

            StiDrawing.FillRectangle(g, BackColor, rectAll);
            StiDrawing.DrawRectangle(g, BorderColor, rectTop.X, rectTop.Y, rectTop.Width - 1, rectTop.Height - 1);

            using (var font = new Font("Arial", 15f))
            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                var state = g.Save();

                try
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    StiDrawing.DrawString(g, "Aa", font, ForeColor, rectTop, sf);
                }
                finally
                {
                    g.Restore(state);
                }
            }

            using (var font = new Font("Arial", 8f))
            using (var sf = new StringFormat())
            {
                StiDrawing.FillRectangle(g, TitleBackColor, rectBottom);

                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                var state = g.Save();

                try
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    StiDrawing.DrawString(g, LocalizedName, font, TitleForeColor, rectBottom, sf);
                }
                finally
                {
                    g.Restore(state);
                }
            }
        }
        #endregion
    }
}