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
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Helpers;
using System.Collections;

namespace Stimulsoft.Data.Engine
{
    public class StiDataRowJoiner
    {
        #region Methods
        public IEnumerable<DataRow> Join(StiDataJoinType type, StiDataLink link, IEnumerable<IStiMeter> meters)
        {
            switch (type)
            {
                case StiDataJoinType.Inner:
                    return InnerJoinRows(link);

                case StiDataJoinType.Left:
                    return LeftJoinRows(link, meters);

                case StiDataJoinType.Cross:
                    return CrossJoinRows();

                case StiDataJoinType.Full:
                    return FullJoinRows(link);

                default:
                    throw new NotSupportedException();
            }
        }

        private IEnumerable<DataRow> LeftJoinRows(StiDataLink link, IEnumerable<IStiMeter> meters)
        {
            switch (StiDataJoiner.JoinEngine)
            {
                case StiDataJoiner.StiDataJoinEngine.V1:
                case StiDataJoiner.StiDataJoinEngine.V4:
                    return LeftJoinRowsV1V4(link);

                case StiDataJoiner.StiDataJoinEngine.V5:
                    return LeftJoinRowsV5(link);

                default:
                    return LeftJoinRowsV2V3(link, meters);
            }            
        }

        private IEnumerable<DataRow> InnerJoinRows(StiDataLink link)
        {
            var fieldIndex1 = GetFieldIndex(table1, link);
            var fieldIndex2 = GetFieldIndex(table2, link);
            var rows1 = table1.AsEnumerable().ToList();
            var rows2 = table2.AsEnumerable().ToList();

            return rows1.Join(rows2,
                    r1 => GetHashCode(r1, fieldIndex1),
                    r2 => GetHashCode(r2, fieldIndex2),
                    SplitRows);
        }

        private IEnumerable<DataRow> LeftJoinRowsV1V4(StiDataLink link)
        {
            var fieldIndex1 = GetFieldIndex(table1, link);
            var fieldIndex2 = GetFieldIndex(table2, link);
            var rows1 = table1.AsEnumerable().ToList();
            var rows2 = table2.AsEnumerable().ToList();

            return rows1.GroupJoin(rows2,
                r1 => GetHashCode(r1, fieldIndex1),
                r2 => GetHashCode(r2, fieldIndex2),
                (r1, r2) => new { key = r1, rows = r2 })
                .SelectMany(a => a.rows.DefaultIfEmpty(), (r1, r2) => SplitRows(r1.key, r2));
        }

        private IEnumerable<DataRow> LeftJoinRowsV5(StiDataLink link)
        {
            var fieldIndexes1 = GetFieldIndexes(table1, link);
            var fieldIndexes2 = GetFieldIndexes(table2, link);
            var rows1 = table1.AsEnumerable().ToList();
            var rows2 = table2.AsEnumerable().ToList();

            return rows1.GroupJoin(rows2,
                r1 => GetHashCode(r1, fieldIndexes1),
                r2 => GetHashCode(r2, fieldIndexes2),
                (r1, r2) => new { key = r1, rows = r2 })
                .SelectMany(a => a.rows.DefaultIfEmpty(), (r1, r2) => SplitRows(r1.key, r2));
        }

        private IEnumerable<DataRow> LeftJoinRowsV2V3(StiDataLink link, IEnumerable<IStiMeter> meters)
        {
            var fieldIndex1 = GetFieldIndex(table1, link);
            var fieldIndex2 = GetFieldIndex(table2, link);
            var rows1 = table1.AsEnumerable().ToList();
            var rows2 = table2.AsEnumerable().ToList();

            var groups = rows1.GroupJoin(rows2,
                r1 => GetHashCode(r1, fieldIndex1),
                r2 => GetHashCode(r2, fieldIndex2),
                (r1, r2) => new { key = r1, rows = r2 });

            var measureIndexes = CalculateIndexes(meters);

            var rows = new List<DataRow>();
            foreach (var group in groups)
            {
                var first = true;
                foreach (var row in group.rows.DefaultIfEmpty())
                {
                    if (first)
                    {
                        //First row copied in any case
                        rows.Add(SplitRows(group.key, row));
                    }
                    else
                    {
                        var oldArray = group.key.ItemArray;
                        var newArray = new object[oldArray.Length];

                        for (var index = 0; index < oldArray.Length; index++)
                        {
                            if (StiDataJoiner.JoinEngine == StiDataJoiner.StiDataJoinEngine.V2)
                            {
                                //All numeric fields should be skipped for correct totals calculation
                                if (!IsNumericType(oldArray[index]))
                                    newArray[index] = oldArray[index];
                            }
                            else if (StiDataJoiner.JoinEngine == StiDataJoiner.StiDataJoinEngine.V3)
                            {
                                if (!measureIndexes.ContainsKey(index) || !IsNumericType(oldArray[index]))
                                    newArray[index] = oldArray[index];
                            }
                        }

                        var newRow = resultTable.LoadDataRow(newArray, false);
                        rows.Add(SplitRows(newRow, row));
                    }

                    first = false;
                }
            }

            return rows;
        }

        private Hashtable CalculateIndexes(IEnumerable<IStiMeter> meters)
        {
            var measures = meters.Where(m => m is IStiMeasureMeter)
                .SelectMany(m => StiExpressionHelper.GetArguments(m.Expression))
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Select(m => m.Trim().ToLowerInvariant()).ToList();

            var measureIndexes = new Hashtable();
            foreach (string measure in measures)
            {
                var column = table1.Columns.Cast<DataColumn>().ToList().FirstOrDefault(c => c.ColumnName.ToLowerInvariant() == measure);
                if (column == null) continue;

                var index = table1.Columns.IndexOf(column);
                if (index == -1) continue;
                
                measureIndexes[index] = index;
            }

            return measureIndexes;
        }

        private bool IsNumericType(object value)
        {
            return value != null && value.GetType().IsNumericType();
        }

        private IEnumerable<DataRow> CrossJoinRows()
        {
            var rows1 = table1.AsEnumerable().ToList();
            var rows2 = table2.AsEnumerable().ToList();

            return rows1.SelectMany(r1 => rows2.Select(r2 => SplitRows(r1, r2)));
        }

        private IEnumerable<DataRow> FullJoinRows(StiDataLink link)
        {
            var fieldIndex1 = GetFieldIndex(table1, link);
            var fieldIndex2 = GetFieldIndex(table2, link);
            var rows1 = table1.AsEnumerable().ToList();
            var rows2 = table2.AsEnumerable().ToList();

            return rows1
                .FullOuterJoin(rows2,
                    r1 => GetHashCode(r1, fieldIndex1),
                    r2 => GetHashCode(r2, fieldIndex2),
                    SplitRows);
        }

        private int GetHashCode(DataRow row, int fieldIndex)
        {
            return fieldIndex != -1 && row[fieldIndex] != null ? row[fieldIndex].GetHashCode() : 0;
        }

        private int GetHashCode(DataRow row, int[] fieldIndexes)
        {
            int hash = 0;

            if (fieldIndexes == null)
                return hash;

            fieldIndexes.ToList().ForEach(i => hash = GetHashCode(row, i) ^ hash);

            return hash;
        }

        private DataRow SplitRows(DataRow row1, DataRow row2)
        {
            var values = new object[resultTable.Columns.Count];

            foreach (DataColumn column in resultTable.Columns)
            {
                var index = resultColumnIndexes.ContainsKey(column.ColumnName) ? resultColumnIndexes[column.ColumnName] : -1;
                if (index == -1) continue;

                var index1 = column1Indexes.ContainsKey(column.ColumnName) ? column1Indexes[column.ColumnName] : -1;
                var index2 = column2Indexes.ContainsKey(column.ColumnName) ? column2Indexes[column.ColumnName] : -1;

                if (index1 != -1 && row1 != null)
                    values[index] = row1[index1];

                if (index2 != -1 && row2 != null)
                    values[index] = row2[index2];
            }

            return resultTable.LoadDataRow(values.ToArray(), false);
        }

        private int GetFieldIndex(DataTable table, StiDataLink link)
        {
            return GetFieldIndex(table, link.ParentKeys?.FirstOrDefault(), link.ChildKeys?.FirstOrDefault());
        }

        private int GetFieldIndex(DataTable table, string parentKey, string childKey)
        {
            var column = table.Columns
                .Cast<DataColumn>()
                .FirstOrDefault(c => c.ColumnName == parentKey ||
                                     c.ColumnName == childKey ||
                                     $"{c.Table.TableName}.{c.ColumnName}" == parentKey ||
                                     $"{c.Table.TableName}.{c.ColumnName}" == childKey);

            if (column == null) 
                return -1;

            return table.Columns.IndexOf(column);
        }

        private int[] GetFieldIndexes(DataTable table, StiDataLink link)
        {
            if (link == null)
                return null;

            var index = 0;
            return link.ParentKeys
                .Select(parentKey => GetFieldIndex(table, parentKey, GetItem(link.ChildKeys, index++)))
                .ToArray();
        }

        private string GetItem(string[] keys, int index)
        {
            return keys != null && index < keys.Length ? keys[index] : null;
        }
        #endregion

        #region Fields
        private DataTable resultTable;
        private DataTable table1;
        private DataTable table2;
        private Dictionary<string, int> resultColumnIndexes = new Dictionary<string, int>();
        private Dictionary<string, int> column1Indexes = new Dictionary<string, int>();
        private Dictionary<string, int> column2Indexes = new Dictionary<string, int>();
        #endregion

        public StiDataRowJoiner(DataTable resultTable, DataTable table1, DataTable table2)
        {
            this.resultTable = resultTable;
            this.table1 = table1;
            this.table2 = table2;

            foreach (DataColumn column in resultTable.Columns)
            {
                resultColumnIndexes.Add(column.ColumnName, resultTable.Columns.IndexOf(column.ColumnName));
            }

            foreach (DataColumn column in table1.Columns)
            {
                column1Indexes.Add(column.ColumnName, table1.Columns.IndexOf(column.ColumnName));
            }

            foreach (DataColumn column in table2.Columns)
            {
                column2Indexes.Add(column.ColumnName, table2.Columns.IndexOf(column.ColumnName));
            }
        }
    }
}