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
	public class StiRowCollection : CollectionBase, IComparer
	{
		#region IComparer
		private int directionFactor = 1;

		int IComparer.Compare(object x, object y)
		{
			var row1 = x as StiRow;
			var row2 = y as StiRow;
			
			if (sortType == StiSortType.ByDisplayValue)
			{
                if (row1.DisplayValue != null && row1.DisplayValue != DBNull.Value)
				{
                    if (row2.DisplayValue == DBNull.Value)
                        return -1;

                    if (row2.OthersText != null)
                        return -1;

                    if (row1.OthersText != null)
                        return 1;

                    var comparable = row1.DisplayValue as IComparable;
                    if (comparable == null)
                        return 1;
					try
					{
						return comparable.CompareTo(row2.DisplayValue) * directionFactor;
					}
					catch 
					{
						try
						{
							return comparable.ToString().CompareTo(row2.DisplayValue.ToString()) * directionFactor;
						}
						catch
						{
							return -1;
						}						
					}					
				}
				else
                    return 1;
			}
			else 
			{
                if (row1.Value != null && row1.Value != DBNull.Value)
				{
                    if (row2.Value == DBNull.Value)
                        return -1;

                    if (row2.OthersText != null)
                        return -1;

                    if (row1.OthersText != null)
                        return 1;

                    var comparable = row1.Value as IComparable;
                    if (comparable == null)
                        return 1;

                    return comparable.CompareTo(row2.Value) * directionFactor;
				}
				else
                    return 1;
			}
		}
		#endregion

        #region Fields
        private StiSortType sortType;
        private Hashtable items = new Hashtable();
        #endregion

        #region Methods
        public List<StiRow> ToList()
        {
            return this.Cast<StiRow>().ToList();
        }

        public void Insert(int position, object value)
        {
            List.Insert(position, value);
        }

        public void Add(object value, object displayValue)
		{
			StiRow row = new StiRow(value, displayValue);
			Add(row);
		}

		public void Add(StiRow row)
		{
			List.Add(row);
			if (!row.IsTotal)items.Add(row.Value, row);
		}

		public new void Clear()
		{
			List.Clear();
			items.Clear();
		}

        public void Sort(StiSortDirection direction, StiSortType sortType)
        {
            this.sortType = sortType;

            directionFactor = direction == StiSortDirection.Asc ? 1 : -1;

            base.InnerList.Sort(this);
        }
        #endregion

        #region Properties
        public StiRow this[object name]
		{
			get
			{
				return (StiRow)items[name];
			}
		}

		public StiRow this[int index]
		{
			get
			{
				return (StiRow)List[index];
			}
		}
        #endregion        
		
	}
}
