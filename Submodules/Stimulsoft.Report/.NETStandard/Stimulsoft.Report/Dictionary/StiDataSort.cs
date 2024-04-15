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
using System.Reflection;
using System.Data;
using System.Collections;
using Stimulsoft.Report.Components;
using System.Collections.Generic;

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Class is used for sorting the array of data for the data source.
	/// </summary>
	internal class StiDataSort : 
        IComparer<DataRow>
    {
		#region Fields
		/// <summary>
		/// Sorting rules
		/// </summary>
		private string[] sortColumns;

		/// <summary>
		/// Hashtable for converting row to condition index
		/// </summary>
		private Hashtable rowToConditions;

		/// <summary>
		/// Group conditions rules
		/// </summary>
		private object [,,]conditions;

		/// <summary>
		/// Array of hash-tables, is used IComparer
		/// </summary>
		private Hashtable[] hashValues;

		/// <summary>
		/// Sorted data source.
		/// </summary>
		private StiDataSource dataSource;

        /// <summary>
        /// Components for parses in interpretation mode
        /// </summary>
	    private StiText textComp;
        private Engine.StiParserParameters parserParameters = new Engine.StiParserParameters();
        #endregion

        #region Fields.Static
        private static readonly object nullObject = new object();
        #endregion

        #region IComparer
        /// <summary>
        /// Compares two rows with data.
        /// </summary>
        /// <param name="x">First (DataRow).</param>
        /// <param name="y">Second (DataRow).</param>
        /// <returns>Compare result.</returns>
        public int Compare(DataRow x, DataRow y)
		{
            return CompareRows(x, y);
		}

        private int CompareRows(object x, object y)
        {
            try
            {
                if (x == y) return 0;

                var row1 = x as DataRow;
                var row2 = y as DataRow;

                #region Processing sorting by group conditions
                if (rowToConditions != null && conditions != null)
                {
                    var indexCond1 = (int)rowToConditions[row1];
                    var indexCond2 = (int)rowToConditions[row2];

                    var groupCount = conditions.GetLength(1);
                    for (var groupIndex = 0; groupIndex < groupCount; groupIndex++)
                    {
                        var value1 = conditions[indexCond1, groupIndex, 0];
                        var value2 = conditions[indexCond2, groupIndex, 0];

                        var direction =
                            (StiGroupSortDirection)conditions[indexCond1, groupIndex, 1];

                        var groupResult = CompareValues(value1, value2, direction == StiGroupSortDirection.Ascending);
                        if (groupResult == 0) continue;

                        return groupResult;
                    }
                }
                #endregion

                if (sortColumns == null || sortColumns.Length == 0) return 0;

                #region Processing sorting by sorting rules
                var sortIndex = 0;
                var index = 0;

                while (index < sortColumns.Length)
                {
                    #region Sorting by asc or desc
                    var ascendary = sortColumns[index++] == "ASC";
                    var startIndex = index;
                    #endregion

                    #region Define sorting for row
                    while (index < sortColumns.Length && sortColumns[index] != "ASC" && sortColumns[index] != "DESC")
                        index++;
                    #endregion

                    #region Get values from cache
                    var value1 = hashValues[sortIndex][x];
                    var value2 = hashValues[sortIndex][y];
                    #endregion

                    #region If at least one of the values is not found, that gets not found values
                    if (value1 == null || value2 == null)
                    {
                        var dataSource = this.dataSource;

                        var sortRow1 = x as DataRow;
                        var sortRow2 = y as DataRow;

						var str = sortColumns[startIndex];
                        if (str.StartsWith("{", StringComparison.InvariantCulture) && str.EndsWith("}", StringComparison.InvariantCulture))
						{
							var method = str.Substring(1, str.Length - 2);
							var methodInfo = dataSource.Dictionary.Report.GetType().GetMethod(method);

						    var isCompilation = dataSource.Dictionary.Report.CalculationMode == StiCalculationMode.Compilation;

							var positionOfRow1 = (int)rowToConditions[sortRow1];
							var positionOfRow2 = (int)rowToConditions[sortRow2];

							var oldPosition = dataSource.Position;
							dataSource.Position = positionOfRow1;
                            value1 = isCompilation ? methodInfo.Invoke(dataSource.Dictionary.Report, new object[0]) : Engine.StiParser.ParseTextValue(str, textComp, parserParameters);
						    dataSource.Position = positionOfRow2;
                            value2 = isCompilation ? methodInfo.Invoke(dataSource.Dictionary.Report, new object[0]) : Engine.StiParser.ParseTextValue(str, textComp, parserParameters);

							dataSource.Position = oldPosition;
						}
						else
						{
							#region If sorting contains relations, that processes these relations
							if (index - startIndex > 1)
							{
								var pos = startIndex;
								while (pos < index - 1)
								{
									dataSource = dataSource.GetParentDataSource(sortColumns[pos]);
									if (dataSource == null) return 0;
                                    if (value1 == null)
                                    {
                                        var tempRows = sortRow1 != null ? sortRow1.GetParentRows(sortColumns[pos]) : null;
                                        if (tempRows != null && tempRows.Length > 0)
                                            sortRow1 = tempRows[0];
                                        else
                                            sortRow1 = null;
                                    }
                                    if (value2 == null)
                                    {

                                        var tempRows = sortRow2 != null ? sortRow2.GetParentRows(sortColumns[pos]) : null;
                                        if (tempRows != null && tempRows.Length > 0)
                                            sortRow2 = tempRows[0];
                                        else
                                            sortRow2 = null;
                                    }
									pos++;
								}
							}
							#endregion

							#region Get objects of the comparison
							var columnIndex = dataSource.GetColumnIndex(sortColumns[index - 1]);
                            if (columnIndex != -1)
                            {
                                #region Column is DataTable column
                                if (value1 == null)
                                {
                                    if (sortRow1 != null)
                                        value1 = sortRow1.ItemArray[columnIndex];
                                    else
                                        value1 = nullObject;

                                    if (hashValues[sortIndex][x] == null)
                                        hashValues[sortIndex].Add(x, value1);
                                }

                                if (value2 == null)
                                {
                                    if (sortRow2 != null)
                                        value2 = sortRow2.ItemArray[columnIndex];
                                    else
                                        value2 = nullObject;

                                    if (hashValues[sortIndex][y] == null)
                                        hashValues[sortIndex].Add(y, value2);
                                }
                                #endregion
                            }
                            else
                            {
                                #region Column is Calculated column
                                var column = dataSource.Columns[sortColumns[index - 1]];
                                var calcColumn = column as StiCalcDataColumn;
                                if (calcColumn != null && rowToConditions != null)
                                {
                                    var method = $"Get{dataSource.Name}_{calcColumn.Name}";
                                    var methodInfo = dataSource.Dictionary.Report.GetType().GetMethod(method);
                                    var isCompilation = dataSource.Dictionary.Report.CalculationMode == StiCalculationMode.Compilation;

                                    if (value1 == null)
                                    {
                                        if (sortRow1 != null)
                                        {
                                            var indexCond1 = (int)rowToConditions[sortRow1];
                                            var storedPos = dataSource.Position;
                                            dataSource.Position = indexCond1;
                                            value1 = isCompilation ? methodInfo.Invoke(dataSource.Dictionary.Report, new object[0]) : Engine.StiParser.ParseTextValue("{" + calcColumn.Expression + "}", textComp, parserParameters);
                                            dataSource.Position =storedPos;
                                        }
                                        else
                                            value1 = nullObject;

                                        if (!hashValues[sortIndex].ContainsKey(x))
                                            hashValues[sortIndex].Add(x, value1);
                                    }

                                    if (value2 == null)
                                    {
                                        if (sortRow2 != null)
                                        {
                                            var indexCond2 = (int)rowToConditions[sortRow2];
                                            var storedPos = dataSource.Position;
                                            dataSource.Position = indexCond2;
                                            value2 = isCompilation ? methodInfo.Invoke(dataSource.Dictionary.Report, new object[0]) : Engine.StiParser.ParseTextValue("{" + calcColumn.Expression + "}", textComp, parserParameters);
                                            dataSource.Position = storedPos;
                                        }
                                        else
                                            value2 = nullObject;

                                        if (!hashValues[sortIndex].ContainsKey(y))
                                            hashValues[sortIndex].Add(y, value2);
                                    }
                                }
                                #endregion
                            }
							#endregion
						}
                    }
                    #endregion

                    sortIndex++;

                    #region Compare two values, if its equals goes to next row, if not equal are stop
                    var dataResult = CompareValues(value1, value2, ascendary);
                    if (dataResult == 0) continue;
                    return dataResult;
                    #endregion
                }
                #endregion

                return 0;

            }
            catch
            {
                return 0;
            }
        }
		#endregion

		#region Methods
		internal static int CompareValues(object value1, object value2)
		{
			return CompareValues(value1, value2, true);
		}

		internal static int CompareValues(object value1, object value2, bool ascendary)
		{
			var direction = 1;
			if (!ascendary)direction = -1;

			if (value1 == DBNull.Value && value2 == DBNull.Value)
			    return 0;

			if (value1 == DBNull.Value)
			    return -1 * direction;

			if (value2 == DBNull.Value)
			    return 1 * direction;

			var comparable1 = value1 as IComparable;
			var comparable2 = value2 as IComparable;

			if (comparable1 == null && comparable2 == null)
			    return 0;

			if (comparable1 == null)
			    return -1 * direction;

			if (comparable2 == null)
			    return 1 * direction;
					
			if (comparable1 != comparable2)
			{
				int result;
                if (StiOptions.Engine.OrdinalStringComparison && (value1 is string) && (value2 is string))
                {
                    result = StringComparer.Ordinal.Compare(value1, value2);
                }
                else
                {
                    result = comparable1.CompareTo(comparable2);
                }

                if (!ascendary)
				    result = -result;

				if (result != 0)
				    return result;
			}

			return 0;
		}

		internal void Clear()
		{
			sortColumns = null;
			if (rowToConditions != null)
			{
				rowToConditions.Clear();
				rowToConditions = null;
			}
			conditions = null;
			hashValues = null;

			dataSource = null;

		    textComp = null;
		}
		#endregion
		
        /// <summary>
        /// Creates a new object of the type StiDataSort.
        /// </summary>
        /// <param name="rowToConditions">Internal hashtable.</param>
        /// <param name="conditions">Internal array.</param>
        /// <param name="sortColumns">Rows on which necessary to produce sorting.</param>
        /// <param name="dataSource">Data source, data which are sorted.</param>
        public StiDataSort(
            Hashtable rowToConditions, object[,,] conditions, 
            string[] sortColumns, StiDataSource dataSource)
		{
			this.dataSource = dataSource;
			this.sortColumns = sortColumns;
			this.conditions = conditions;
			this.rowToConditions = rowToConditions;

			#region Prepare sort columns
			if (sortColumns != null)
			{
                this.sortColumns = sortColumns.Clone() as string[];

                var index = 0;
				var count = 0;
				while (index < sortColumns.Length)
				{
                    string str = sortColumns[index];

                    if (str == "ASC")
					    count++;

					if (str == "DESC")
					    count++;

                    if (str.StartsWith("{", StringComparison.InvariantCulture) && str.EndsWith("}", StringComparison.InvariantCulture))
                    {
                        var st = str.Substring(1, str.Length - 2).Trim();
                        if (dataSource.Columns.Contains(st))
                        {
                            this.sortColumns[index] = "{" + StiNameValidator.CorrectName(dataSource.Name) + "." + st + "}";
                        }
                    }

					index++;
				}
				hashValues = new Hashtable[count];
				for (var pos = 0; pos < count; pos++)hashValues[pos] = new Hashtable();
			}
			#endregion

            if (dataSource.Dictionary != null)
            {
                bool hasCalc = false;
                foreach (var column in dataSource.Columns)
                    if (column is StiCalcDataColumn)
                    {
                        hasCalc = true;
                        break;
                    }
                var report = dataSource.Dictionary.Report;
                if (report != null && (report.CalculationMode == StiCalculationMode.Interpretation || hasCalc))
                {
                    textComp = new StiText
                    {
                        Name = "**StiDataSort**",
                        Page = report.Pages[0]
                    };
                }
            }
		}

	}
}
