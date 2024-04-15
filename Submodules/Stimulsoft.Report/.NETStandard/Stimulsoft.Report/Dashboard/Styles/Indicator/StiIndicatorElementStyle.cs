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
using Stimulsoft.Report.Helpers;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Dashboard.Styles
{
    public abstract class StiIndicatorElementStyle : StiElementStyle
    {
        #region Properties
        public abstract string LocalizedName { get; }

        public abstract Color GlyphColor { get; set; }

        [Browsable(false)]
        public virtual Color BackColor { get; set; } = Color.White;

        [Browsable(false)]
        public virtual Color ForeColor { get; set; } = Color.White;

        [Browsable(false)]
        public virtual Color HotBackColor { get; set; } = Color.White;

        [Browsable(false)]
        public virtual Color PositiveColor { get; set; } = Color.Green;

        [Browsable(false)]
        public virtual Color NegativeColor { get; set; } = Color.Red;

        [Browsable(false)]
        public virtual StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(180, 50, 50, 50));

        [Browsable(false)]
        public virtual StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.White);

        [Browsable(false)]
        public virtual StiCornerRadius ToolTipCornerRadius { get; set; } = new StiCornerRadius(8);

        [Browsable(false)]
        public virtual StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder();
        #endregion

        #region Methods
        public void DrawStyleForGallery(Graphics g, Rectangle rect, StiFontIcons indicatorFontIcons, bool isDashboard, bool isFormUI)
        {
            var state = g.Save();

            try
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;

                if (isDashboard)
                {
                    var backColor = GetBackColor();
                    StiDrawing.FillRectangle(g, backColor, rect);

                    var icon = StiFontIconsHelper.GetContent(indicatorFontIcons);

                    RectangleF rectIcon;

                    float fontSize = (StiScale.Factor > 1.0d) ? 15f : 20f;
                    using (var font = new Font(StiFontIconsHelper.GetFontFamilyIcons(), (float)(fontSize * StiScale.Factor)))
                    using (var iconBrush = new SolidBrush(GlyphColor))
                    {
                        var iconSize = g.MeasureString(icon, font);
                        rectIcon = new RectangleF(rect.Right - iconSize.Width - StiScale.XXI(8), rect.Top + (rect.Height - iconSize.Height) / 2, iconSize.Width, iconSize.Height);
                        g.DrawString(icon, font, iconBrush, rectIcon);
                    }

                    var textRect = new RectangleF(rect.Left, rect.Top, rect.Width - rectIcon.Width, rect.Height);
                    var foreColor = Ident == StiElementStyleIdent.Custom ? this.ForeColor : StiDashboardStyleHelper.GetForeColor(Ident);
                    using (var textFont = new Font("Arial", (float)(20f * StiScale.Factor), FontStyle.Bold, GraphicsUnit.Pixel))
                    using (var textBrush = new SolidBrush(foreColor))
                    using (var sf = new StringFormat())
                    {
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;

                        g.DrawString("1000", textFont, textBrush, textRect, sf);
                    }
                }
                else
                {
                    var backColor = GetBackColor();
                    StiDrawing.FillRectangle(g, backColor, rect);

                    float fontSize = (StiScale.Factor > 1.0d) ? 13f : 17f;
                    using (var font = new Font(StiFontIconsHelper.GetFontFamilyIcons(), (float)(fontSize * StiScale.Factor)))
                    using (var iconBrush = new SolidBrush(PositiveColor))
                    {
                        var icon = StiFontIconsHelper.GetContent(indicatorFontIcons);

                        var iconSize = g.MeasureString(icon, font);
                        var rectIcon = new RectangleF(rect.Left + StiScale.XXI(8), rect.Top + (rect.Height - iconSize.Height) / 2, iconSize.Width, iconSize.Height);
                        g.DrawString(icon, font, iconBrush, rectIcon);
                    }
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