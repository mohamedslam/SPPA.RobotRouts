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

namespace Stimulsoft.Report.Engine
{
	internal class StiPageNumberCollection : CollectionBase 
	{
		#region Collection
		public void Add(StiPageNumber pageNumber)
		{
			List.Add(pageNumber);
		}

		public void AddRange(StiPageNumber[] pageNumbers)
		{
			base.InnerList.AddRange(pageNumbers);
		}

		public bool Contains(StiPageNumber pageNumber)
		{
			return List.Contains(pageNumber);
		}
		
		public int IndexOf(StiPageNumber pageNumber)
		{
			return List.IndexOf(pageNumber);
		}

		public void Insert(int index, StiPageNumber pageNumber)
		{
			List.Insert(index, pageNumber);
		}

		public void Remove(StiPageNumber pageNumber)
		{
			List.Remove(pageNumber);
		}

		public StiPageNumber this[int index]
		{
			get
			{
				return (StiPageNumber)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
		#endregion
	}
}
