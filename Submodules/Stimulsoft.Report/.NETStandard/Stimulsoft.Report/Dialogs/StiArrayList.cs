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

namespace Stimulsoft.Report.Dialogs
{
	public class StiArrayList : ArrayList
	{	
		public event EventHandler CollectionChanged;

		private void InvokeCollectionChanged()
		{
		    CollectionChanged?.Invoke(this, EventArgs.Empty);
		}

		public override int Add(object value)
		{
			int index = base.Add(value);
			InvokeCollectionChanged();
			return index;
		}

        public int AddCore(object value)
        {
            return base.Add(value);
        }

		public override void AddRange(ICollection c)
		{
			base.AddRange(c);
			InvokeCollectionChanged();
		}

        public void AddRangeCore(ICollection c)
        {
            base.AddRange(c);
        }
 
		public override void Clear()
		{
			base.Clear();
			InvokeCollectionChanged();
		}

        public void ClearCore()
        {
            base.Clear();
        }

		public override void Insert(int index, object value)
		{
			base.Insert(index, value);
			InvokeCollectionChanged();
		}

		public override void InsertRange(int index, ICollection c)
		{
			base.InsertRange(index, c);
			InvokeCollectionChanged();
		}

		public override void Remove(object obj)
		{
			base.Remove(obj);
			InvokeCollectionChanged();
		}
 
		public override void RemoveAt(int index)
		{
			base.RemoveAt(index);
			InvokeCollectionChanged();
		}

		public override void RemoveRange(int index, int count)
		{
			base.RemoveRange(index, count);
			InvokeCollectionChanged();
		}
	}
}
