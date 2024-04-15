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

using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Data.Extensions
{
    public static class IEnumerableTableExt
    {
        public static DataTable ToNetTable(this IEnumerable<object[]> source, IEnumerable<IStiMeter> meters, bool onlyColumns = false)
        {
            var table = new DataTable();

            var rows = source.ToList();
            var meterIndex = 0;
            meters.ToList().ForEach(f =>
            {
                var dataType = FindType(rows, meterIndex);

                table.Columns.Add(new DataColumn
                {
                    ColumnName = table.GetUniqueName(f, StiLabelHelper.GetLabel(f)),
                    DataType = dataType
                });
                meterIndex++;
            });

            if (!onlyColumns)
            {
                table.BeginLoadData();
                source.ToList().ForEach(a => LoadDataRow(table, a));
                table.EndLoadData();
            }

            return table;
        }

        private static void LoadDataRow(DataTable table, object[] a)
        {
            try
            {
                table.LoadDataRow(a, true);
            }
            catch
            {
                var row = table.NewRow();
                for (var index = 0; index < a.Length; index++)
                {
                    try
                    {
                        row[index] = a[index];
                    }
                    catch
                    {
                    }
                }

                table.Rows.Add(row);
            }
        }

        private static Type FindType(List<object[]> rows, int meterIndex)
        {
            var dataType = FindTypeInRows(rows, meterIndex);
            if (dataType != null && dataType != typeof(DBNull))
                return dataType;

            return typeof(object);
        }

        private static Type FindTypeInRows(List<object[]> rows, int columnIndex)
        {
            var types = rows
                .Select(r => r[columnIndex])
                .Where(v => v != null && v != DBNull.Value)
                .Select(v => v.GetType())
                .Where(t => t != null && t != typeof(object))
                .Distinct();

            if (types == null || !types.Any() || types.Count() > 1)
                return typeof(object);

            return types.FirstOrDefault();
        }
    }
}