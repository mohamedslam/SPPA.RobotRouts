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

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Helpers;

namespace Stimulsoft.Data.Extensions
{
    public static class DataTableExt
    {
        #region Properties.Static
        public static DataTable NullTable = new DataTable();
        #endregion

        #region Methods
        public static string GetUniqueName(this DataTable table, IStiMeter meter)
        {
            if (!table.Columns.Contains(meter.Label))
                return meter.Label;

            var func = StiExpressionHelper.GetFunction(meter.Expression);
            if (!string.IsNullOrWhiteSpace(func))
            {
                var funcName = $"{meter.Label}-{func}";
                if (!table.Columns.Contains(funcName))
                    return funcName;
            }

            return table.GetUniqueName(meter.Label);
        }

        public static string GetUniqueName(this DataTable table, IStiMeter meter, string baseName)
        {
            if (!table.Columns.Contains(baseName))
                return baseName;

            var func = StiExpressionHelper.GetFunction(meter.Expression);
            if (!string.IsNullOrWhiteSpace(func))
            {
                var funcName = $"{baseName}-{func}";
                if (!table.Columns.Contains(funcName))
                    return funcName;
            }

            return table.GetUniqueName(meter.Label);
        }

        public static string GetUniqueName(this DataTable table, string baseName)
        {
            if (baseName == null)
                return null;

            var name = baseName;
            var index = 2;

            while (table.Columns.Contains(name))
            {
                name = baseName + index++;
            }
            return name;
        }

        public static IEnumerable<DataRelation> ParentRelationList(this DataTable table)
        {
            return table.ParentRelations.Cast<DataRelation>().ToList();
        }

        public static IEnumerable<DataRelation> ChildRelationList(this DataTable table)
        {
            return table.ChildRelations.Cast<DataRelation>().ToList();
        }

        public static IEnumerable<object[]> AsEnumerableArray(this DataTable joinedTable)
        {
            var rows = joinedTable.AsEnumerable().Select(r => r.ItemArray).ToList();

            var columnIndex = 0;
            foreach (DataColumn column in joinedTable.Columns)
            {
                var type = column.DataType;
                if (column.DataType.IsEnum)
                {
                    foreach (var row in rows)
                    {
                        var value = row[columnIndex];
                        if (value != null && value != DBNull.Value)
                            row[columnIndex] = Enum.ToObject(type, value);
                    }
                }

                columnIndex++;
            }

            return rows;
        }
        #endregion
    }
}