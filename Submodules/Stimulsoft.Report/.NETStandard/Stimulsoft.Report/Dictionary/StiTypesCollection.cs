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

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Collection of the registered types.
	/// </summary>
	public class StiTypesCollection : CollectionBase
	{
		#region Collection
        public List<StiType> ToList()
        {
            return this.Cast<StiType>().ToList();
        }

		public void Add(StiType type)
		{
			lock(List)List.Add(type);
		}

		public void AddRange(StiType[] type)
		{
			lock(InnerList)base.InnerList.AddRange(type);
		}

		public void AddRange(StiTypesCollection types)
		{
			foreach (StiType type in types)this.Add(type);
		}

		public bool Contains(StiType type)
		{
			lock(List)return List.Contains(type);
		}
		
		public int IndexOf(StiType type)
		{
			lock(List)return List.IndexOf(type);
		}

		public void Insert(int index, StiType type)
		{
			lock(List)List.Insert(index, type);
		}

		public void Remove(StiType type)
		{
			lock(List)List.Remove(type);
		}
		public StiType this[int index]
		{
			get
			{
				lock(List)return (StiType)List[index];
			}
			set
			{
				lock(List)List[index] = value;
			}
		}

		public StiType this[string name]
		{
			get
			{
				lock(List)
				{
					foreach (StiType type in List)
						if (type.Name == name)return type;
					return null;
				}
			}
			set
			{
				lock(List)
				{
					for (int index = 0; index < List.Count; index++)				
					{
						StiType data = List[index] as StiType;
					
						if (data.Name == name)
						{
							List[index] = value;
							return;
						}
					}
					Add(value);

				}
			}
		}

		public StiType this[Type type]
		{
			get
			{
				lock(List)
				{
					foreach (StiType tp in List)
						if (tp.Type == type)return tp;
					return null;
				}
			}
			set
			{
				lock(List)
				{
					int pos = 0;
					foreach (StiType tp in List)
					{
						if (tp.Type == type)List[pos] = value;
						pos++;
					}
				}
			}
		}

		public StiType[] Items
		{
			get
			{
				lock(InnerList)return (StiType[])InnerList.ToArray(typeof(StiType));
			}
		}
        #endregion

        #region Methods
        /// <summary>
        /// Registers type.
        /// </summary>
        /// <param name="name">Name of type.</param>
        /// <param name="type">Type.</param>
        public void RegType(string name, Type type)
		{
			Add(new StiType(name, type));
		}
        #endregion
    }
}
