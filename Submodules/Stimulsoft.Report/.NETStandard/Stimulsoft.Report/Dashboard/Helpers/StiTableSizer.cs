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

using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Meters;
using Stimulsoft.Report.Dictionary;
using System;
using System.Data;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Dashboard.Helpers
{
    public class StiTableSizer
    {
        public static void MakeAutoSize(DataGridView grid, bool skipLimit = false, bool isDashboardShowing = false)
        {
            try
            {
                if (grid.Disposing || grid.IsDisposed) return;

                var dataTable = grid.DataSource as DataTable;
                if (dataTable == null || dataTable.Rows.Count < 5000 || skipLimit)
                    grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                else
                    grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

                using (var bmp = new Bitmap(1, 1))
                using (var g = Graphics.FromImage(bmp))
                {
                    foreach (DataGridViewColumn column in grid.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;

                        if (!isDashboardShowing)
                            column.Width = Math.Min(StiScale.XXI(300), column.Width);

                        var dataColumn = dataTable.Columns[column.Name];
                        if (dataColumn != null)
                        {
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                            if (dataTable.Columns[column.Name].DataType.IsDateType() || dataTable.Columns[column.Name].DataType == typeof(bool))
                                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                            if (dataColumn?.DataType != null && dataColumn.DataType.IsNumericType())
                                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }

                        if (!isDashboardShowing || GetDefaultColumnWidth(column.Tag as IStiTableColumn) == 0)
                            column.MinimumWidth = (int)g.MeasureString(column.HeaderText, grid.Font).Width + StiScale.XXI(44);
                    }
                }
            }
            catch
            {
            }
        }

        private static int GetDefaultColumnWidth(IStiTableColumn column)
        {
            var columnSize = column as IStiTableColumnSize;
            if (columnSize?.Size == null)
                return 0;

            else
                return columnSize.Size.Width;
        }
    }
}
