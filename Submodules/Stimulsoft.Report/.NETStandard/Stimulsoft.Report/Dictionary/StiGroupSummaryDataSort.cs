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
using System.Collections.Generic;

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Class is used for sorting the array of data for the data source.
	/// </summary>
	internal class StiGroupSummaryDataSort : 
        IComparer<DataRow>
    {
		#region Fields
        private Hashtable groupSummaries;

        private Hashtable groupLines;

        private Hashtable baseRowOrder;

        private StiComponentsCollection groupHeaders;
        #endregion

		#region IComparer
		/// <summary>
		/// Compares two rows with data.
		/// </summary>
        /// <param name="row1">First (DataRow).</param>
        /// <param name="row2">Second (DataRow).</param>
		/// <returns>Compare result.</returns>
		public int Compare(DataRow row1, DataRow row2)
		{
            try
            {
                if (row1 == row2) return 0;

                foreach (StiGroupHeaderBand group in groupHeaders)
                {
                    if (group.SummarySortDirection == StiGroupSortDirection.None) continue;

                    var sortFactor = group.SummarySortDirection == StiGroupSortDirection.Ascending ? 1 : -1;

                    var groupSummariesHash = groupSummaries[group] as Hashtable;
                    var groupLinesHash = groupLines[group] as Hashtable;

                    var value1 = groupSummariesHash[row1];
                    var value2 = groupSummariesHash[row2];

                    var result = CompareValues(value1, value2);
                    if (result != 0)
                        return result * sortFactor;

                    var line1 = (int)groupLinesHash[row1];
                    var line2 = (int)groupLinesHash[row2];
                    result = line1.CompareTo(line2);
                    if (result != 0)
                        return result * sortFactor;
                }

                var baseline1 = (int)baseRowOrder[row1];
                var baseline2 = (int)baseRowOrder[row2];
                return baseline1.CompareTo(baseline2);
            }
            catch
            {
                return 0;
            }
        }

        private int CompareValues(object value1, object value2)
        {
            if (value1 is Int64 || value2 is Int64)
                return ((Int64) value1).CompareTo((Int64) value2);

            if (value1 is decimal || value2 is decimal)
                return ((decimal) value1).CompareTo((decimal) value2);

            if (value1 is DateTime || value2 is DateTime)
                return ((DateTime) value1).CompareTo((DateTime) value2);

            if (value1 is TimeSpan || value2 is TimeSpan)
                return ((TimeSpan) value1).CompareTo((TimeSpan) value2);

            return 0;
        }
		#endregion

        #region Methods
        public void Clear()
        {
            groupSummaries.Clear();
            groupSummaries = null;

            groupLines.Clear();
            groupLines = null;

            groupHeaders = null;

            baseRowOrder.Clear();
            baseRowOrder = null;
        }
        #endregion

        public StiGroupSummaryDataSort(
            Hashtable groupSummaries, Hashtable groupLines,
            StiComponentsCollection groupHeaders, Hashtable baseRowOrder)
		{
            this.groupSummaries = groupSummaries;
            this.groupLines = groupLines;
            this.groupHeaders = groupHeaders;
            this.baseRowOrder = baseRowOrder;
		}
	}
}
