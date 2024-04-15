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

using System.Data;

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// The class wrapper for DataRow.
	/// </summary>
	public class StiDataRow
	{
        #region Fields
        private StiDataSource dataSource;
        #endregion

        #region Properties
        /// <summary>
        /// DataRow that is used for data extraction.
        /// </summary>
        public DataRow Row { get; set; }
        
	    protected StiDictionary Dictionary => dataSource.Dictionary;

	    /// <summary>
		/// Indexer that returns data by the column name.
		/// </summary>
		public object this[string columnName]
		{
			get
			{
                if (Row == null) return null;
                var columnIndex = dataSource.GetColumnIndex(columnName);
                if (columnIndex < 0) return null;
                return Row.ItemArray[columnIndex];
			}
		}
        #endregion

        #region Methods
        /// <summary>
        /// Returns StiDataRow by name of the parent relation.
        /// </summary>
        /// <param name="relation">Parent relation name.</param>
        /// <returns>Parent StiDataRow.</returns>
        public virtual StiDataRow GetParentData(string relation)
		{
			if (Row == null)return null;

		    var parentRows = Row.GetParentRows(relation);
			if (parentRows == null || parentRows.Length == 0)return null;

			return new StiDataRow(dataSource.GetParentDataSource(relation), parentRows[0]);
		}
        #endregion

        /// <summary>
        /// Creates a new object of the type StiDataRow.
        /// </summary>
        /// <param name="dataRow">DataRow that is used for data extraction.</param>
        public StiDataRow(StiDataRow dataRow) : this(dataRow != null ? dataRow.dataSource : null, dataRow != null ? dataRow.Row : null)
		{
		}
		
		/// <summary>
		/// Creates a new object of the type StiDataRow.
		/// </summary>
		/// <param name="dataSource">Data Source in which DataRow is located.</param>
		/// <param name="dataRow">DataRow that is used for data extraction.</param>
		public StiDataRow(StiDataSource dataSource, DataRow dataRow)
		{
			this.dataSource = dataSource;
			this.Row = dataRow;
		}
	}
}
