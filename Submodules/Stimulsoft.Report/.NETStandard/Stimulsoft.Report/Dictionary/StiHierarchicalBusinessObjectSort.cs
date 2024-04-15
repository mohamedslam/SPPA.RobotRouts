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
	internal class StiHierarchicalBusinessObjectSort : IComparer
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

                object key1 = StiBusinessObjectHelper.GetValueFromObject(x, keyColumn);
                object key2 = StiBusinessObjectHelper.GetValueFromObject(y, keyColumn);

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
            if (value is Type) value = null;
            if (value == null) return string.Empty;
			if (value == DBNull.Value)return string.Empty;
			string str = value.ToString();
			if (str.Trim().Length == 0)return string.Empty;
			return str;
		}

        internal void Process()
		{
            //if (dataSource.detailRows == null) return;

            var values = new Hashtable();
            CreateTree(values);

			businessObject.RowToLevel = new Hashtable();
            SetLevelAndSort(values, this.parentValue, 0);
            
            var rows = new ArrayList();
            CreateRowList(values, rows, this.parentValue);

            //dataSource.detailRows = new DataRow[rows.Count];
            //rows.CopyTo(dataSource.detailRows);
            this.businessObject.enumerator = rows.GetEnumerator();
        }

        /// <summary>
        /// Create tree from hierarchical items.
        /// </summary>
        private void CreateTree(Hashtable values)
        {
            try { this.businessObject.enumerator.Reset(); }
            catch { }

            while (this.businessObject.enumerator.MoveNext())
            {
                object masterKey = StiBusinessObjectHelper.GetValueFromObject(this.businessObject.enumerator.Current, masterKeyColumn);

                masterKey = GetParentValue(masterKey);
                ArrayList rowItems = values[masterKey] as ArrayList;
                if (rowItems == null)
                {
                    rowItems = new ArrayList();
                    values[masterKey] = rowItems;
                }
                rowItems.Add(this.businessObject.enumerator.Current);
            }
        }

        /// <summary>
        /// Set level and sort items.
        /// </summary>
        private void SetLevelAndSort(Hashtable values, object masterKey, int level)
        {
            ArrayList list = values[GetParentValue(masterKey)] as ArrayList;
            if (list != null)
            {
                foreach (object row in list)
                {
                    businessObject.RowToLevel[row] = level;
                    object key = StiBusinessObjectHelper.GetValueFromObject(row, keyColumn);
                    SetLevelAndSort(values, key, level + 1);
                }
               
                StiBusinessObjectSort dataSort = new StiBusinessObjectSort(businessObject.OwnerBand.Sort, null, null);
                list.Sort(dataSort);

                dataSort.Clear();
            }
        }

        private void CreateRowList(Hashtable values, ArrayList rows, object masterKey)
        {
            ArrayList list = values[GetParentValue(masterKey)] as ArrayList;
            if (list != null)
            {
                foreach (object row in list)
                {
                    rows.Add(row);
                    object key = StiBusinessObjectHelper.GetValueFromObject(row, keyColumn);
                    CreateRowList(values, rows, key);
                }
            }
        }       
		#endregion

		#region Fields
        private StiBusinessObject businessObject = null;
		
		private string keyColumn = null;
		private string masterKeyColumn = null;
		private string parentValue = null;
        private string[] sortColumns = null;
		#endregion

        public StiHierarchicalBusinessObjectSort(StiBusinessObject businessObject, StiHierarchicalBand band, string[] sortColumns)
		{
            this.businessObject = businessObject;

			this.keyColumn = band.KeyDataColumn;
			this.masterKeyColumn = band.MasterKeyDataColumn;
			this.parentValue = band.ParentValue;
            this.sortColumns = sortColumns;

			if (keyColumn == null || keyColumn.Trim().Length == 0)
				throw new ArgumentException("Property 'KeyDataColumn' of '" + band.Name + "'is not filled!");

			if (masterKeyColumn == null || masterKeyColumn.Trim().Length == 0)
				throw new ArgumentException("Property 'MasterKeyDataColumn' of '" + band.Name + "'is not filled!");

			if (!businessObject.Columns.Contains(keyColumn))
                throw new ArgumentException("Column '" + keyColumn + "' does not present in '" + businessObject.Name + "'");

            if (!businessObject.Columns.Contains(masterKeyColumn))
                throw new ArgumentException("Column '" + masterKeyColumn + "' does not present in '" + businessObject.Name + "'");

			if (this.parentValue == null)parentValue = string.Empty;
		}
	}
}
