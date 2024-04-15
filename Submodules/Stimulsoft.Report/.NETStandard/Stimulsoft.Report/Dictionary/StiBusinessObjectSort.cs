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
using System.Data;
using System.Collections;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Class is used for sorting the array of data for the data source.
    /// </summary>
    internal class StiBusinessObjectSort :
        IComparer
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
        private object[,,] conditions;
        #endregion

        #region IComparer
        int IComparer.Compare(object x, object y)
        {
            try
            {
                if (x == y)
                    return 0;

                #region Processing sorting by group conditions
                if (rowToConditions != null && conditions != null)
                {
                    var indexCond1 = (int)rowToConditions[x];
                    var indexCond2 = (int)rowToConditions[y];

                    var groupCount = conditions.GetLength(1);
                    for (var groupIndex = 0; groupIndex < groupCount; groupIndex++)
                    {
                        var value1 = conditions[indexCond1, groupIndex, 0];
                        var value2 = conditions[indexCond2, groupIndex, 0];

                        var direction = (StiGroupSortDirection)conditions[indexCond1, groupIndex, 1];

                        var groupResult = CompareValues(value1, value2, direction == StiGroupSortDirection.Ascending);
                        if (groupResult == 0) continue;
                        return groupResult;
                    }
                }
                #endregion

                if (sortColumns == null || sortColumns.Length == 0) return 0;

                #region Processing sorting by sorting rules
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

                    var objX = x;
                    var objY = y;

                    if (index - startIndex > 1)
                    {
                        var pos = startIndex;
                        while (pos < index - 1)
                        {
                            objX = StiBusinessObjectHelper.GetValueFromObject(objX, sortColumns[pos]);

                            var drX = objX as DataRow[];
                            if (drX != null && drX.Length > 0) objX = drX[0];

                            objY = StiBusinessObjectHelper.GetValueFromObject(objY, sortColumns[pos]);

                            var drY = objY as DataRow[];
                            if (drY != null && drY.Length > 0) objY = drY[0];
                            pos++;
                        }
                    }

                    var columnName = sortColumns[index - 1];
                    var result1 = StiBusinessObjectHelper.GetValueFromObject(objX, columnName);
                    var result2 = StiBusinessObjectHelper.GetValueFromObject(objY, columnName);

                    var dataResult = CompareValues(result1, result2, ascendary);
                    
                    if (dataResult == 0) continue;
                    return dataResult;
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
            if (!ascendary) direction = -1;

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
                var result = comparable1.CompareTo(comparable2);

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
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiDataSort.
        /// </summary>
        /// <param name="sortColumns">Rows on which necessary to produce sorting.</param>
        public StiBusinessObjectSort(string[] sortColumns, Hashtable rowToConditions, object[,,] conditions)
        {
            this.sortColumns = sortColumns;
            this.conditions = conditions;
            this.rowToConditions = rowToConditions;
        }
    }
}
