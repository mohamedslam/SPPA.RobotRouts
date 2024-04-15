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
using System.ComponentModel;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
#endif

namespace Stimulsoft.Report.Dashboard.Styles
{
    public abstract class StiTableElementStyle :
        StiElementStyle,
        IStiCellIndicatorStyle
    {
        #region Properties
        public abstract string LocalizedName { get; }

        public virtual Color CellBackColor { get; set; }

        public virtual Color CellForeColor { get; set; } = StiColor.Get("222");

        public virtual Color SelectedCellBackColor { get; set; } = StiColor.Get("3498db");

        public virtual Color SelectedCellForeColor { get; set; } = Color.White;

        public virtual Color AlternatingCellBackColor { get; set; }

        public virtual Color AlternatingCellForeColor { get; set; } = StiColor.Get("222");

        public virtual Color HeaderBackColor { get; set; }

        public virtual Color HeaderForeColor { get; set; } = Color.White;

        public virtual Color HotHeaderBackColor { get; set; }

        public virtual Color LineColor { get; set; } = Color.Gainsboro;

        [Browsable(false)]
        public virtual Color FooterBackColor { get; set; } = Color.White;

        [Browsable(false)]
        public virtual Color FooterForeColor { get; set; } = Color.Black;

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
        #endregion

        #region Methods
        public void DrawStyleForGallery(Graphics g, Rectangle rect)
        {
            var rect1 = new Rectangle(rect.X + StiScale.XXI(15), rect.Y + StiScale.YYI(5), rect.Width - StiScale.XXI(30), rect.Height - StiScale.YYI(10));
            int cellWidth = rect1.Width / 3;
            int cellHeight = rect1.Height / 5;

            using (var headerBrush = new SolidBrush(HeaderBackColor))
            using (var row1Brush = new SolidBrush(CellBackColor))
            using (var row2Brush = new SolidBrush(AlternatingCellBackColor))
            using (var lineLightBrush = new SolidBrush(LineColor))
            {

                int x = rect1.X;
                int y = rect1.Y;
                for (int row = 0; row < 5; row++)
                {
                    for (int column = 0; column < 3; column++)
                    {
                        var rect2 = new Rectangle(x, y, cellWidth, cellHeight);

                        if (row == 0)
                        {
                            g.FillRectangle(headerBrush, rect2);
                            g.FillRectangle(lineLightBrush, new Rectangle(rect2.Right - StiScale.XXI(1), rect2.Top, StiScale.XXI(1), rect2.Height));
                        }
                        else
                        {
                            var brush = (row % 2 == 0) ? row2Brush : row1Brush;
                            g.FillRectangle(brush, rect2);
                            g.FillRectangle(lineLightBrush, new Rectangle(rect2.Right - StiScale.XXI(1), rect2.Top, StiScale.XXI(1), rect2.Height));
                        }

                        x += cellWidth;
                    }

                    x = rect1.X;
                    y += cellHeight;
                }

                using (var pen = new Pen(lineLightBrush, (float)(1f * StiScale.Factor)))
                {
                    g.DrawRectangle(pen, new Rectangle(rect1.X, rect1.Y, cellWidth * 3 - StiScale.XXI(1), cellHeight * 5));
                }
            }
        }
        #endregion
    }
}