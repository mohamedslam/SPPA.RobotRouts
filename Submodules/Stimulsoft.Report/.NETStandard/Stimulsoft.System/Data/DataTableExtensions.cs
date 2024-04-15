using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Data;

namespace Stimulsoft.System.Data
{
    public static class DataTableExtensions
    {
        private static DataTable LoadTableFromEnumerable<T>(IEnumerable<T> source, DataTable table, LoadOption? options, FillErrorEventHandler errorHandler)
            where T : DataRow
        {
            if (options.HasValue) {
                switch(options.Value) {
                    case LoadOption.OverwriteChanges:
                    case LoadOption.PreserveChanges:
                    case LoadOption.Upsert:
                        break;
                    default:
                        throw DataSetUtil.InvalidLoadOption(options.Value);
                }
            }


            using (IEnumerator<T> rows = source.GetEnumerator())
            {
                // need to get first row to create table
                if (!rows.MoveNext())
                {
                    if (table == null)
                    {
                        throw DataSetUtil.InvalidOperation(Strings.DataSetLinq_EmptyDataRowSource);
                    }
                    else
                    {
                        return table;
                    }
                }

                DataRow current;
                if (table == null)
                {
                    current = rows.Current;
                    if (current == null)
                    {
                        throw DataSetUtil.InvalidOperation(Strings.DataSetLinq_NullDataRow);
                    }

                    table = new DataTable();
                    table.Locale = CultureInfo.CurrentCulture;
                    // We do not copy the same properties that DataView.ToTable does.
                    // If user needs that functionality, use other CopyToDataTable overloads.
                    // The reasoning being, the IEnumerator<DataRow> can be sourced from
                    // different DataTable, so we just use the "Default" instead of resolving the difference.

                    foreach (DataColumn column in current.Table.Columns)
                    {
                        table.Columns.Add(column.ColumnName, column.DataType);
                    }
                }

                table.BeginLoadData();
                try
                {
                    do
                    {
                        current = rows.Current;
                        if (current == null)
                        {
                            continue;
                        }

                        object[] values = null;
                        try
                        {   // 'recoverable' error block
                            switch(current.RowState)
                            {
                                case DataRowState.Detached:
                                    if (!current.HasVersion(DataRowVersion.Proposed))
                                    {
                                        throw DataSetUtil.InvalidOperation(Strings.DataSetLinq_CannotLoadDetachedRow);
                                    }
                                    goto case DataRowState.Added;
                                case DataRowState.Unchanged:
                                case DataRowState.Added:
                                case DataRowState.Modified:
                                    values = current.ItemArray;
                                    if (options.HasValue)
                                    {
                                        table.LoadDataRow(values, options.Value);
                                    }
                                    else
                                    {
                                        table.LoadDataRow(values, true);
                                    }
                                    break;
                                case DataRowState.Deleted:
                                    throw DataSetUtil.InvalidOperation(Strings.DataSetLinq_CannotLoadDeletedRow);
                                default:
                                    throw DataSetUtil.InvalidDataRowState(current.RowState);
                            }
                        }
                        catch (Exception e)
                        {
                            if (!DataSetUtil.IsCatchableExceptionType(e))
                            {
                                throw;
                            }

                            FillErrorEventArgs fillError = null;
                            if (null != errorHandler)
                            {
                                fillError = new FillErrorEventArgs(table, values);
                                fillError.Errors = e;
                                errorHandler.Invoke(rows, fillError);
                            }
                            if (null == fillError) {
                                throw;
                            }
                            else if (!fillError.Continue)
                            {
                                if (Object.ReferenceEquals(fillError.Errors ?? e, e))
                                {   // if user didn't change exception to throw (or set it to null)
                                    throw;
                                }
                                else
                                {   // user may have changed exception to throw in handler
                                    throw fillError.Errors;
                                }
                            }
                        }
                    } while (rows.MoveNext());
                }
                finally
                {
                    table.EndLoadData();
                }
            }
            Debug.Assert(null != table, "null DataTable");
            return table;
        }
    }
}
