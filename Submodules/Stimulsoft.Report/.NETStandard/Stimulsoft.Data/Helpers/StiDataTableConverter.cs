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

using Stimulsoft.Base;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Extensions;
using System;
using System.Data;
using System.Linq;

namespace Stimulsoft.Data.Helpers
{
    public static class StiDataTableConverter
    {
        public static DataTable ToNetTable(StiDataTable dataTable, Type[] types = null)
        {
            if (dataTable == null) return null;

            var table = new DataTable();

            var row = dataTable.Rows.FirstOrDefault();
            var meterIndex = 0;
            dataTable.Meters.ToList().ForEach(f =>
            {
                table.Columns.Add(new DataColumn
                {
                    ColumnName = table.GetUniqueName(f),
                    DataType = GetDataType(row, meterIndex, types)
                });
                meterIndex++;
            });

            table.BeginLoadData();
            dataTable.Rows.ToList().ForEach(a =>
            {
                var newRow = table.NewRow();

                try
                {
                    for (var index = 0; index < a.Length; index++)
                    {
                        if (index >= types.Length) continue;

                        try
                        {
                            if (a[index] is DBNull || a[index] == null)
                                newRow[index] = DBNull.Value;
                            else
                                newRow[index] = StiConvert.ChangeType(a[index], table.Columns[index].DataType);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }

                table.Rows.Add(newRow);
            });
            table.EndLoadData();

            return table;
        }

        private static Type GetDataType(object[] row, int meterIndex, Type[] types = null)
        {
            if (types != null && meterIndex < types.Length)
            {
                var type = types[meterIndex];

                if (StiTypeWrapper.SimpleNullableTypes.Contains(type))
                    type = Nullable.GetUnderlyingType(type);

                return type;
            }
            else
            {
                var type = row != null ? row[meterIndex].GetType() : typeof(object);
                return type == typeof(DBNull) ? typeof(object) : type;
            }
        }
    }
}