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
	internal class StiHierarchicalDataSort : IComparer
	{
        #region IComparer
        /// <summary>
        /// Compares two rows with data.
        /// </summary>
        /// <param name="x">First (DataRow).</param>
        /// <param name="y">Second (DataRow).</param>
        /// <returns>Compare result.</returns>
        public int Compare(object x, object y)
        {
            try
            {
                if (x == y) return 0;

                var row1 = x as DataRow;
                var row2 = y as DataRow;

                var key1 = row1[keyColumn];
                var key2 = row2[keyColumn];

                return StiDataSort.CompareValues(key1, key2);
            }
            catch
            {
                return 0;
            }
        }
        #endregion

		#region Methods
		private string GetParentValue(object value)
		{
			if (value == null)return string.Empty;
			if (value == DBNull.Value)return string.Empty;
			var str = value.ToString();
			if (str.Trim().Length == 0)return string.Empty;
			return str;
		}

        internal void Process(Hashtable rowToConditions)
		{
            if (dataSource.DetailRows == null) return;

            var values = new Hashtable();
            CreateTree(values);

			dataSource.RowToLevel = new Hashtable();
            SetLevelAndSort(rowToConditions, values, this.parentValue, 0);
            
            var rows = new ArrayList();
            CreateRowList(values, rows, this.parentValue);

            dataSource.DetailRows = new DataRow[rows.Count];
            rows.CopyTo(dataSource.DetailRows);
        }

        /// <summary>
        /// Create tree from hierarchical items.
        /// </summary>
        private void CreateTree(Hashtable values)
        {            
            foreach (var row in dataSource.DetailRows)
            {
                var masterKey = row[masterKeyColumn];

                masterKey = GetParentValue(masterKey);
                var rowItems = values[masterKey] as ArrayList;
                if (rowItems == null)
                {
                    rowItems = new ArrayList();
                    values[masterKey] = rowItems;
                }
                rowItems.Add(row);
            }
        }

        /// <summary>
        /// Set level and sort items.
        /// </summary>
        private void SetLevelAndSort(Hashtable rowToConditions, Hashtable values, object masterKey, int level)
        {
            var list = values[GetParentValue(masterKey)] as ArrayList;
            if (list == null) return;

            foreach (DataRow row in list)
            {
                dataSource.RowToLevel[row] = level;
                var key = row[keyColumn];
                SetLevelAndSort(rowToConditions.Clone() as Hashtable, values, key, level + 1);
            }

            var dataSort = new StiDataSort(rowToConditions, null, sortColumns, dataSource);

            var listOfDetailRows = new List<DataRow>();

            foreach (DataRow row in list)
            {
                listOfDetailRows.Add(row);
            }
            listOfDetailRows.Sort(dataSort);
            list.Clear();
            foreach (var row in listOfDetailRows)
            {
                list.Add(row);
            }

            dataSort.Clear();
        }

        private void CreateRowList(Hashtable values, ArrayList rows, object masterKey)
        {
            var list = values[GetParentValue(masterKey)] as ArrayList;
            if (list == null) return;

            foreach (DataRow row in list)
            {
                rows.Add(row);
                var key = row[keyColumn];
                CreateRowList(values, rows, key);
            }
        }       
		#endregion

		#region Fields
		private StiDataSource dataSource;
		private string keyColumn;
		private string masterKeyColumn;
		private string parentValue;
        private string[] sortColumns;
		#endregion

		public StiHierarchicalDataSort(StiDataSource dataSource, StiHierarchicalBand band, string []sortColumns)
		{
			this.dataSource = dataSource;

			this.keyColumn = band.KeyDataColumn;
			this.masterKeyColumn = band.MasterKeyDataColumn;
			this.parentValue = band.ParentValue;
            this.sortColumns = sortColumns;

			if (keyColumn == null || keyColumn.Trim().Length == 0)
				throw new ArgumentException($"Property 'KeyDataColumn' of '{band.Name}'is not filled!");

			if (masterKeyColumn == null || masterKeyColumn.Trim().Length == 0)
				throw new ArgumentException($"Property 'MasterKeyDataColumn' of '{band.Name}' is not filled!");

			if (!dataSource.Columns.Contains(keyColumn))
				throw new ArgumentException($"Column '{keyColumn}' does not present in '{dataSource.Name}'!");

			if (!dataSource.Columns.Contains(masterKeyColumn))
				throw new ArgumentException($"Column '{masterKeyColumn}' does not present in '{dataSource.Name}'!");

			if (this.parentValue == null)parentValue = string.Empty;
		}
	}
}
