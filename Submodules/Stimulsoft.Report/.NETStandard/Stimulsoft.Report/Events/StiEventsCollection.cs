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

using System.Collections;

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Collection of Events
	/// </summary>
	public class StiEventsCollection : CollectionBase
	{
		public void Add(StiEvent ev)
		{
			List.Add(ev);
		}

		public void AddRange(StiEventsCollection evs)
		{
			foreach (StiEvent ev in evs)Add(ev);
		}

		public void AddRange(StiEvent[] evs)
		{
			foreach (StiEvent ev in evs)Add(ev);
		}

		public bool Contains(StiEvent ev)
		{
			return List.Contains(ev);
		}
		
		public int IndexOf(StiEvent ev)
		{
			return List.IndexOf(ev);
		}

		public void Insert(int index, StiEvent ev)
		{
			List.Insert(index, ev);
		}

		public void Remove(StiEventsCollection evs)
		{
			foreach (StiEvent ev in evs)Remove(ev);
		}

		public void Remove(StiEvent ev)
		{
			List.Remove(ev);
		}
		
		public StiEvent this[int index]
		{
			get
			{
				return (StiEvent)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
	}
}
