using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report
{
	public class StiGlobalizationItemCollection : CollectionBase, IComparer
	{
		#region IComparer
		int IComparer.Compare(object x, object y)
		{
			var item1 = x as StiGlobalizationItem;
			var item2 = y as StiGlobalizationItem;

			return string.Compare(item1.PropertyName, item2.PropertyName, StringComparison.Ordinal);
		}
		#endregion

		#region Collection
		public void Add(StiGlobalizationItem data)
		{
			List.Add(data);
		}

		public void AddRange(StiGlobalizationItem[] data)
		{
			base.InnerList.AddRange(data);
		}

		public bool Contains(StiGlobalizationItem data)
		{
			return List.Contains(data);
		}
		
		public int IndexOf(StiGlobalizationItem data)
		{
			return List.IndexOf(data);
		}

		public void Insert(int index, StiGlobalizationItem data)
		{
			lock (this)List.Insert(index, data);
		}

		public void Remove(StiGlobalizationItem data)
		{
			lock (this)List.Remove(data);
		}

        public List<StiGlobalizationItem> ToList()
        {
            return InnerList.ToArray()
                .Cast<StiGlobalizationItem>()
                .ToList();
        }		
		
		public StiGlobalizationItem this[int index]
		{
			get
			{
				return (StiGlobalizationItem)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
		#endregion

		public void Sort()
		{
			this.InnerList.Sort(this);
		}
	}
}
