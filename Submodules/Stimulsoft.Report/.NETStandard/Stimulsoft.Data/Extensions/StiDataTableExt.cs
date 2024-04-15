#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Data.Engine;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Data.Extensions
{
    public static class StiDataTableExt
    {
        #region Methods
        public static DataTable ToNetTable(this StiDataTable table, bool onlyColumns = false)
        {
            return table.Rows.ToNetTable(table.Meters, onlyColumns);
        }

        public static DataTable Remove(this DataTable table, IEnumerable<DataColumn> columns)
        {
            if (columns == null) 
                return table;

            columns.ToList().ForEach(column => table.Columns.Remove(column));

            return table;
        }

        public static DataTable Remove(this DataTable table, IEnumerable<string> columns)
        {
            if (columns == null)
                return table;

            columns.ToList().ForEach(column => table.Columns.Remove(column));

            return table;
        }

        public static List<DataColumn> ColumnsList(this DataTable table)
        {
            return table.Columns.Cast<DataColumn>().ToList();
        }

        public static DataColumn AddColumn(this DataTable table, DataColumn column, int order = -1)
        {
            table.Columns.Add(column);
            
            if (order != -1)
                column.SetOrdinal(order);
            
            return column;

        }        
        #endregion
    }
}