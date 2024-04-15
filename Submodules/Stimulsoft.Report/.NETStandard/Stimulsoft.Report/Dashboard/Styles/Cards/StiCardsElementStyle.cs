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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Painters;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Dashboard.Styles
{
    public abstract class StiCardsElementStyle :
        StiElementStyle,
        IStiCellIndicatorStyle
    {
        #region Properties
        public abstract string LocalizedName { get; }

        public abstract Color[] SeriesColors { get; set; }

        public virtual Color CellBackColor { get; set; }
        
        public virtual Color LineColor { get; set; } = Color.Gainsboro;

        public virtual Color CellForeColor { get; set; } = StiColor.Get("222");
        
        [Browsable(false)]
        public virtual Color BackColor { get; set; } = Color.White;

        [Browsable(false)]
        public virtual Color CellDataBarsOverlapped { get; set; } = Color.FromArgb(0xff, 0x33, 0x5e, 0x96);

        [Browsable(false)]
        public virtual Color CellDataBarsPositive { get; set; } = Color.FromArgb(0xff, 0x63, 0x8e, 0xc6);

        [Browsable(false)]
        public virtual Color CellDataBarsNegative { get; set; } = Color.FromArgb(0xff, 0xff, 0, 0);

        [Browsable(false)]
        public virtual Color CellWinLossPositive { get; set; } = Color.FromArgb(0xff, 0x63, 0x8e, 0xc6);

        [Browsable(false)]
        public virtual Color CellWinLossNegative { get; set; } = Color.FromArgb(0xff, 0xff, 0, 0);

        [Browsable(false)]
        public virtual Color CellSparkline { get; set; } = Color.FromArgb(0xff, 0x53, 0x7e, 0xb6);

        [Browsable(false)]
        public virtual Color CellIndicatorPositive { get; set; } = Color.Green;

        [Browsable(false)]
        public virtual Color CellIndicatorNegative { get; set; } = Color.Red;

        [Browsable(false)]
        public virtual Color CellIndicatorNeutral { get; set; } = Color.LightGray;

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
        public void DrawStyleForGallery(Graphics g, Rectangle rect)
        {
            var state = g.Save();

            try
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                var rectMain = new Rectangle(rect.X + StiScale.XXI(15), rect.Y + StiScale.YYI(5), rect.Width - StiScale.XXI(30), rect.Height - StiScale.YYI(10));
                var textHeight = rectMain.Height / 3;
                var sparklineHeight = rectMain.Height * 2 / 3;

                rect.Inflate(-1, -1);

                if (this.ComponentId != StiComponentId.StiCustomDashboardCardsStyle)
                {
                    var backColor = StiDashboardStyleHelper.GetBackColor(this.Ident);
                    StiDrawing.FillRectangle(g, backColor, rect);
                }

                else
                {
                    StiDrawing.FillRectangle(g, this.BackColor, rect);
                }

                StiDrawing.FillRectangle(g, this.CellBackColor, rectMain);
                rect.Inflate(-4, -4);
                StiDrawing.DrawRectangle(g, this.LineColor, rect);

                var textRect = new RectangleF(rectMain.Left, rectMain.Top, rectMain.Width, textHeight);

                using (var textFont = new Font("Arial", (float)(17f * StiScale.Factor), FontStyle.Bold, GraphicsUnit.Pixel))
                using (var textBrush = new SolidBrush(CellForeColor))
                using (var sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString("1000", textFont, textBrush, textRect, sf);
                }

                var painter = new StiGdiContextPainter(g);
                var context = new StiContext(painter, true, false, false, 1);

                var array = new object[] { 2, 3, 1, 3, 7, 6, 2, 3 };

                var sparklineRect = new RectangleD(rectMain.Left, textRect.Bottom, rectMain.Width, sparklineHeight);

                StiLineSparklinesCellPainter.Draw(context, sparklineRect, array, 1, CellSparkline, true, true, true);

                context.Render(sparklineRect.ToRectangleF());
            }
            finally
            {
                g.Restore(state);
            }
        }
        #endregion
    }
}
