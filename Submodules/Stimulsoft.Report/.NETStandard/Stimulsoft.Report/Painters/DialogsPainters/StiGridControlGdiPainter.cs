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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Events;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiGridControlGdiPainter : StiReportControlGdiPainter
    {
#if !NETSTANDARD
        private void DrawColumn(StiGridControl grid, Graphics g, Rectangle rect, string text)
        {
            g.DrawLine(SystemPens.ControlDark, rect.X, rect.Y + 2, rect.X, rect.Bottom - 4);
            g.DrawLine(SystemPens.ControlLightLight, rect.X + 1, rect.Y + 2, rect.X + 1, rect.Bottom - 4);

            using (SolidBrush brush = new SolidBrush(grid.HeaderForeColor))
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.Character;

                g.DrawString(text, grid.HeaderFont, brush, rect, sf);
            }
        }

        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            StiGridControl grid = component as StiGridControl;
            grid.InvokePainting(grid, e);

            if (!e.Cancel && (!(grid.Enabled == false && grid.IsDesigning == false)))
            {
                Graphics g = e.Graphics;

                Rectangle rect = grid.GetPaintRectangle().ToRectangle();

                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle.ToRectangle())))
                {
                    Rectangle rectEvent = rect;

                    using (SolidBrush brush = new SolidBrush(grid.BackgroundColor))
                    {
                        g.FillRectangle(brush, rect);
                    }

        #region Draw Border
                    Rectangle borderRect = rect;
                    ControlPaint.DrawBorder3D(g, borderRect, Border3DStyle.Sunken);
        #endregion

                    rect.X += 2;
                    rect.Y += 2;
                    rect.Width -= 4;
                    rect.Height -= 4;

                    g.SetClip(rect);

        #region Draw Columns
                    if (grid.ColumnHeadersVisible && grid.Columns.Count > 0)
                    {
                        int posX = grid.RowHeadersVisible ? grid.RowHeaderWidth + rect.X : rect.X;
                        int posY = (int)rect.Y;

                        Rectangle columnsRect = new Rectangle(rect.X, rect.Y, rect.Width, 20);
                        using (SolidBrush brush = new SolidBrush(grid.HeaderBackColor))
                            g.FillRectangle(brush, columnsRect);

                        g.DrawLine(SystemPens.ControlLightLight,
                            columnsRect.X, columnsRect.Y, columnsRect.Right, columnsRect.Y);
                        g.DrawLine(SystemPens.ControlLightLight,
                            columnsRect.X, columnsRect.Y, columnsRect.X, columnsRect.Bottom);
                        g.DrawLine(SystemPens.ControlDarkDark,
                            columnsRect.X, columnsRect.Bottom, columnsRect.Right, columnsRect.Bottom);
                        g.DrawLine(SystemPens.ControlDarkDark,
                            columnsRect.Right, columnsRect.Y, columnsRect.Right, columnsRect.Bottom);
                        //posX += RowHeaderWidth;

                        foreach (StiGridColumn gridColumn in grid.Columns)
                        {
                            int columnWidth = gridColumn.Width > 0 ? gridColumn.Width : grid.PreferredColumnWidth;
                            Rectangle columnRect = new Rectangle(posX, posY, columnWidth, 20);
                            DrawColumn(grid, g, columnRect,
                                !string.IsNullOrEmpty(gridColumn.HeaderText) ? gridColumn.HeaderText : gridColumn.DataTextField);
                            posX += columnWidth;
                        }
                    }
        #endregion

                    g.ResetClip();

                    PaintEvents(grid, e.Graphics, RectangleD.CreateFromRectangle(rect));
                    PaintConditions(grid, e.Graphics, RectangleD.CreateFromRectangle(rect));
                }
            }
            e.Cancel = false;
            grid.InvokePainted(grid, e);

            PaintComponents(grid, e);
        }
        #endregion
#endif
    }
}
