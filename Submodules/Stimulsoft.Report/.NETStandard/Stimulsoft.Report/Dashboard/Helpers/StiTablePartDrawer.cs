#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Dashboard;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Dashboard.Styles;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Dashboard.Helpers
{
    public class StiTablePartDrawer
    {
        #region Methods
        public void DrawCell()
        {
            if (CellArgs.RowIndex != -1)
                DrawDataCell();
            else
                DrawHeaderCell();
        }

        private void DrawHeaderCell()
        {
            var g = CellArgs.Graphics;
            CellArgs.Handled = true;

            CellArgs.Paint(CellBounds,
                DataGridViewPaintParts.Background |
                DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.Focus |
                DataGridViewPaintParts.SelectionBackground);

            var pos = Grid.PointToClient(Cursor.Position);
            var isMouseOver = CellBounds.Contains(pos);

            Color backColor;

            if (style != null)
                backColor = isMouseOver || dragColumnIndex == CellArgs.ColumnIndex && CellArgs.RowIndex == -1 ? style.HotHeaderBackColor : style.HeaderBackColor;
            else
                backColor = isMouseOver ? StiUX.GridHeaderBackgroundMouseOver : StiUX.GridHeaderBackground;

            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, CellBounds);

                PaintFrozenArea(g, CellBounds, CellArgs.ColumnIndex);
            }

            DrawBorder();
            DrawHeaderText();

            var elementInteraction = Table as IStiElementInteraction;
            var interaction = elementInteraction?.DashboardInteraction as IStiTableDashboardInteraction;

            if (interaction == null || interaction.AllowUserSorting)
                DrawSortArrow();

            if (interaction == null || interaction.AllowUserFiltering)
                DrawFilterArrow();
        }

        private void DrawDataCell()
        {
            CellArgs.Paint(CellBounds, DataGridViewPaintParts.All);
            DrawBorder();
            CellArgs.Handled = true;
        }

        private void PaintFrozenArea(Graphics g, Rectangle rect, int columnIndex)
        {
            var columns = Grid.Columns.Cast<DataGridViewColumn>();

            var column = columns.LastOrDefault(c => c.Frozen);
            if (column == null) return;

            var frozenColumns = columns.ToList().IndexOf(column) + 1;
            if (frozenColumns == 0) return;

            if (columnIndex == frozenColumns - 1)
            {
                StiDrawing.FillRectangle(g, StiColor.Get("10FFFFFF"),
                    new Rectangle(rect.Right - StiScale.I6, rect.Top, StiScale.I6, rect.Height));
            }
            else if (columnIndex == frozenColumns)
            {
                StiDrawing.FillRectangle(g, StiColor.Get("10000000"),
                    new Rectangle(rect.Left, rect.Top, StiScale.I6, rect.Height));
            }
        }

        private void DrawBorder()
        {
            var g = CellArgs.Graphics;
            var color = style == null ? StiUX.GridSeparator : style.LineColor;
            var rect = CellBounds;
            rect.Width--;
            rect.Height--;

            StiDrawing.DrawLine(g, color, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
            StiDrawing.DrawLine(g, color, rect.Right, rect.Top, rect.Right, rect.Bottom);
        }

        public void DrawHeaderText()
        {
            var rect = CellBounds;

            var headerAlignment = GetHeaderAlignment();
            if (headerAlignment == StringAlignment.Near && IsSortUsed)
            {
                rect.X += StiScale.I19;
                rect.Width -= StiScale.I19;

                var index = GetSortIndex();
                if (index > 0 && index <= 9)
                {
                    rect.X += StiScale.I2;
                    rect.Width -= StiScale.I2;
                }
            }

            if (headerAlignment == StringAlignment.Far && IsFiltersOrActionsUsed)
            {
                rect.Width -= StiScale.I18;
            }

            using (var sf = new StringFormat())
            {
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = headerAlignment;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = WordWrapHeader ? 0 : StringFormatFlags.NoWrap;

                var str = CellArgs.FormattedValue?.ToString();
                if (!string.IsNullOrWhiteSpace(str))
                    StiDrawing.DrawString(Graphics, str, GetHeaderFont(), GetHeaderForeColorColor(), rect, sf);
            }
        }

        private StringAlignment GetHeaderAlignment()
        {
            return CellArgs.ColumnIndex != -1
                ? GetHeaderAlignment(Grid.Columns[CellArgs.ColumnIndex].HeaderCell.Style.Alignment)
                : StringAlignment.Center;
        }

        private StringAlignment GetHeaderAlignment(DataGridViewContentAlignment horAlignment)
        {
            if (horAlignment == DataGridViewContentAlignment.MiddleLeft)
                return StringAlignment.Near;

            if (horAlignment == DataGridViewContentAlignment.MiddleRight)
                return StringAlignment.Far;

            return StringAlignment.Center;
        }

        private Font GetHeaderFont()
        {
            return headerFont ?? Grid.Font;
        }

        private Color GetHeaderForeColorColor()
        {
            if (headerForeColor != null && headerForeColor != Color.Transparent)
                return headerForeColor.GetValueOrDefault();

            return this.style == null ? StiUX.GridForeground : this.style.HeaderForeColor;
        }

        public void DrawFilterArrow()
        {
            if (ColumnsCount <= CellArgs.ColumnIndex) return;

            if (IsFilterPresent || IsActionPresent)
            {
                var image = style == null
                    ? (IsMouseOverCell ? StiReportImages.Actions.DropDownFilterMouseOver() : StiReportImages.Actions.DropDownFilter())
                    : (IsMouseOverCell ? StiReportImages.Actions.DropDownFilterMouseOverWhite() : StiReportImages.Actions.DropDownFilterWhite());

                Graphics.DrawImage(image, GetRightImageRect(image));

                return;
            }

            var headerAlignment = GetHeaderAlignment();
            if (IsMouseOverCell && dragColumnIndex == -1 && headerAlignment != StringAlignment.Far)
            {
                var image = style == null
                    ? StiReportImages.Actions.DropDownArrowMouseOver()
                    : StiReportImages.Actions.DropDownArrowMouseOverWhite();

                Graphics.DrawImage(image, GetRightImageRect(image));
            }
        }

        private Rectangle GetRightImageRect(Image image)
        {
            var imageWidth = image.Width;
            var imageHeight = image.Height;

            return new Rectangle(
                CellBounds.Right - StiScale.I5 - imageWidth, 
                CellBounds.Y + (CellBounds.Height - imageHeight) / 2, 
                imageWidth, imageHeight);
        }

        public void DrawSortArrow()
        {
            if (ColumnsCount <= CellArgs.ColumnIndex) return;

            var headerAlignment = GetHeaderAlignment();

            var direction = StiDataSortRuleHelper.GetSortDirection(Sorts, ColumnKey);
            if (direction != StiDataSortDirection.None || (IsMouseOverCell && headerAlignment != StringAlignment.Near))
            {
                //Don't dispose image - its cached
                var image = GetSortImage(direction);
                var imageRect = GetSortRectangle(image.Height, image.Height);

                Graphics.DrawImage(image, imageRect);

                imageRect.X += imageRect.Width - StiScale.I4;
                imageRect.Width = 100000;

                DrawSortIndex(imageRect);
            }
        }

        public Rectangle GetSortRectangle(int imageWidth, int imageHeight)
        {
            return new Rectangle(CellBounds.X + StiScale.I4, CellBounds.Y + (CellBounds.Height - imageHeight) / 2, imageWidth, imageHeight);
        }

        public static Rectangle GetSortRectangle(Rectangle cellBounds)
        {
            var image = StiReportImages.Actions.SortArrowAscMouseOver();

            return new Rectangle(cellBounds.X + StiScale.I4, cellBounds.Y + (cellBounds.Height - image.Height) / 2, image.Width, image.Height);
        }

        private Image GetSortImage(StiDataSortDirection direction)
        {
            if (style == null)
            {
                if (direction == StiDataSortDirection.None && IsMouseOverCell)
                    return StiReportImages.Actions.SortArrowAscMouseOver();

                else if (direction == StiDataSortDirection.Ascending)
                    return IsMouseOverCell ? StiReportImages.Actions.SortArrowAscMouseOver() : StiReportImages.Actions.SortArrowAsc();

                else
                    return IsMouseOverCell ? StiReportImages.Actions.SortArrowDescMouseOver() : StiReportImages.Actions.SortArrowDesc();
            }
            else
            {
                if (direction == StiDataSortDirection.None && IsMouseOverCell)
                    return StiReportImages.Actions.SortArrowAscMouseOverDisable();

                else if (direction == StiDataSortDirection.Ascending)
                    return IsMouseOverCell ? StiReportImages.Actions.SortArrowAscMouseOverWhite() : StiReportImages.Actions.SortArrowAscWhite();

                else
                    return IsMouseOverCell ? StiReportImages.Actions.SortArrowDescMouseOverWhite() : StiReportImages.Actions.SortArrowDescWhite();
            }
        }

        public void DrawSortIndex(Rectangle textRect)
        {
            if (Sorts == null || Sorts.Count <= 1) return;
            if (ColumnsCount <= CellArgs.ColumnIndex) return;

            var index = GetSortIndex();
            if (index == -1 || index > 9) return;

            using (var font = new Font(Grid.Font.Name, 7))
            using (var sf = new StringFormat())
            {
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Near;

                var color = style == null
                    ? IsMouseOverCell ? ColorTranslator.FromHtml("#7a7a7a") : ColorTranslator.FromHtml("#aaaaaa")
                    : style.HeaderForeColor;

                using (var brush = new SolidBrush(color))
                {
                    Graphics.DrawString(index.ToString(), font, brush, textRect, sf);
                }
            }
        }

        private int GetSortIndex()
        {
            var rule = Sorts.FirstOrDefault(r => string.Equals(r.Key, ColumnKey, StringComparison.InvariantCultureIgnoreCase));
            if (rule == null) 
                return -1;

            return Sorts.IndexOf(rule) + 1;
        }
        #endregion

        #region Fields
        private StiTableElementStyle style;
        private Color? headerForeColor;
        private Font headerFont;
        private int dragColumnIndex = -1;
        #endregion

        #region Properties
        private IStiTableDashboardInteraction TableInteraction
        {
            get
            {
                return (Table as IStiElementInteraction)?.DashboardInteraction as IStiTableDashboardInteraction;
            }
        }

        private bool IsSortUsed
        {
            get
            {
                var direction = StiDataSortRuleHelper.GetSortDirection(Table.UserSorts, ColumnKey) != StiDataSortDirection.None;
                return TableInteraction.AllowUserSorting && direction;
            }
        }

        private bool IsFiltersOrActionsUsed
        {
            get
            {
                return TableInteraction.AllowUserFiltering && (IsFilterPresent || IsActionPresent);
            }
        }

        private bool IsActionPresent => Actions != null && Actions.Any(a => a.Key == ColumnKey);

        private bool IsFilterPresent => Filters != null && Filters.Any(r => r.Key == ColumnKey);

        private bool IsMouseOverCell => CellBounds.Contains(Grid.PointToClient(Cursor.Position));

        private DataGridViewCellPaintingEventArgs CellArgs { get; }

        private Graphics Graphics => CellArgs.Graphics;

        private Rectangle CellBounds => CellArgs.CellBounds;

        public bool WordWrapHeader { get; set; }

        private DataGridView Grid { get; }

        private List<StiDataSortRule> Sorts { get; }

        private List<StiDataFilterRule> Filters { get; }

        private List<StiDataActionRule> Actions { get; }

        private int ColumnsCount { get; }

        private string ColumnKey { get; }

        private IStiTableElement Table { get; }
        #endregion

        public StiTablePartDrawer(DataGridViewCellPaintingEventArgs cellArgs, DataGridView grid, ListBox.ObjectCollection items, 
            List<StiDataSortRule> sorts, List<StiDataFilterRule> filters, List<StiDataActionRule> actions, IStiTableElement table = null)
        {
            this.CellArgs = cellArgs;
            this.Grid = grid;
            this.Sorts = sorts;
            this.Filters = filters;
            this.Actions = actions;
            this.Table = table;

            this.ColumnsCount = items.Count;

            var columnIndex = Math.Min(ColumnsCount - 1, cellArgs.ColumnIndex);
            this.ColumnKey = columnIndex != -1 ? (items[columnIndex] as StiDataColumn)?.Key : null;
        }

        public StiTablePartDrawer(DataGridViewCellPaintingEventArgs cellArgs, DataGridView grid,
            IStiTableElement element, StiTableElementStyle style = null, Color? headerForeColor = null, Font headerFont = null, 
            int dragColumnIndex = -1, bool wordWrapHeader = false)
        {
            this.CellArgs = cellArgs;
            this.Grid = grid;
            this.Sorts = element.UserSorts;
            this.Filters = element.UserFilters;
            this.style = style;
            this.headerForeColor = headerForeColor;
            this.headerFont = headerFont;
            this.Table = element;
            this.WordWrapHeader = wordWrapHeader;

            var meters = element.GetMeters();
            this.ColumnsCount = meters.Count;

            var columnIndex = Math.Min(ColumnsCount - 1, cellArgs.ColumnIndex);
            this.ColumnKey = meters[columnIndex]?.Key;
            this.dragColumnIndex = dragColumnIndex;
        }

        public StiTablePartDrawer(DataGridViewCellPaintingEventArgs cellArgs, DataGridView grid)
        {
            this.CellArgs = cellArgs;
            this.Grid = grid;
        }
    }
}
