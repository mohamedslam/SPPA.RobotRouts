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
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Report.Dashboard.Styles
{
    public abstract class StiPivotElementStyle : StiElementStyle
    {
        #region Properties
        public abstract string LocalizedName { get; }

        public virtual Color CellBackColor { get; set; }

        public virtual Color CellForeColor { get; set; } = StiColor.Get("222");

        public virtual Color SelectedCellBackColor { get; set; }

        public virtual Color SelectedCellForeColor { get; set; } = StiColor.Get("222");

        public virtual Color AlternatingCellBackColor { get; set; }

        public virtual Color AlternatingCellForeColor { get; set; } = StiColor.Get("222");

        public virtual Color ColumnHeaderBackColor { get; set; }

        public virtual Color ColumnHeaderForeColor { get; set; } = Color.White;

        public virtual Color RowHeaderBackColor { get; set; }

        public virtual Color RowHeaderForeColor { get; set; } = Color.White;

        public virtual Color HotColumnHeaderBackColor { get; set; }

        public virtual Color HotRowHeaderBackColor { get; set; }

        public virtual Color LineColor { get; set; } = Color.Gainsboro;

        [Browsable(false)]
        public virtual Color BackColor { get; set; } = Color.White;
        #endregion

        #region Methods
        public void DrawStyleForGallery(Graphics g, Rectangle rect)
        {
            var rect1 = new Rectangle(rect.X + StiScale.XXI(15), rect.Y + StiScale.YYI(5), rect.Width - StiScale.XXI(30), rect.Height - StiScale.YYI(10));
            int cellWidth = rect1.Width / 3;
            int cellHeight = rect1.Height / 5;

            using (var colBrush = new SolidBrush(ColumnHeaderBackColor))
            using (var rowBrush = new SolidBrush(RowHeaderBackColor))
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

                        if (row == 0 || column == 0)
                        {
                            g.FillRectangle(column == 0 ? rowBrush : colBrush, rect2);
                            g.FillRectangle(lineLightBrush, new Rectangle(rect2.Right - StiScale.XXI(1), rect2.Top, StiScale.XXI(1), rect2.Height));

                            if (column == 0 && row > 0)
                                g.FillRectangle(lineLightBrush, new Rectangle(rect2.Left, rect2.Top, rect2.Width, StiScale.YYI(1)));
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

                using (var pen = new Pen(lineLightBrush, (float)(1 * StiScale.Factor)))
                {
                    g.DrawRectangle(pen, new Rectangle(rect1.X, rect1.Y, cellWidth * 3 - StiScale.XXI(1), cellHeight * 5));
                }
            }
        }
        #endregion
    }
}