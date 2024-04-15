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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.CrossTab.Core
{
    public class StiColumnCollection : CollectionBase, IComparer
	{
		#region IComparer
		private int directionFactor = 1;

		int IComparer.Compare(object x, object y)
		{
			var col1 = x as StiColumn;
			var col2 = y as StiColumn;

			if (sortType == StiSortType.ByDisplayValue)
			{
			    if (col1.DisplayValue != null && col2.DisplayValue != DBNull.Value)
				{
					var comparable = col1.DisplayValue as IComparable;
                    if (comparable == null) return 1;
                    if (col2.OthersText != null) return -1;
                    if (col1.OthersText != null) return 1;
					return comparable.CompareTo(col2.DisplayValue) * directionFactor;
				}

			    return 1;
			}
			else
			{
			    if (col1.Value != null)
				{
                    if (col2.OthersText != null) return -1;
                    if (col1.OthersText != null) return 1;
                    var comparable = col1.Value as IComparable;
					return comparable.CompareTo(col2.Value) * directionFactor;
				}

			    return 1;
			}
		}
		#endregion

        #region Fields
        private StiSortType sortType;
        private Hashtable items = new Hashtable();
        #endregion

        #region Methods
        public List<StiColumn> ToList()
        {
            return this.Cast<StiColumn>().ToList();
        }

        public void Insert(int position, object value)
        {
            List.Insert(position, value);
        }

		public void Add(object value, object displayValue)
		{
			Add(new StiColumn(value, displayValue));
		}

		public void Add(StiColumn col)
		{
			List.Add(col);
			if (!col.IsTotal)
			    items.Add(col.Value, col);
		}

		public void Sort(StiSortDirection direction, StiSortType sortType)
		{
			this.sortType = sortType;

		    directionFactor = direction == StiSortDirection.Asc ? 1 : -1;

		    base.InnerList.Sort(this);
		}

		public new void Clear()
		{
			List.Clear();
			items.Clear();
		}
        #endregion

        #region Properties
        public StiColumn this[object name]
		{
			get
			{
				return (StiColumn)items[name];
			}
		}

		public StiColumn this[int index]
		{
			get
			{
				return (StiColumn)List[index];
			}
        }
        #endregion
    }
}
