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

using System.ComponentModel;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events.Design;

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Describes the class for realization of the event Refreshing.
	/// </summary>
	[TypeConverter(typeof(StiRefreshingEventConverter))]
	public class StiRefreshingEvent : StiEvent
	{
		/// <summary>
		/// Returns the string representation of the event.
		/// </summary>
		public override string ToString()
		{
			return "Refreshing";
		}

		/// <summary>
		/// Creates a new object of the type StiRefreshingEvent.
		/// </summary>
		public StiRefreshingEvent() : this("")
		{
		}

		/// <summary>
		/// Creates a new object of the type StiRefreshingEvent with specified arguments.
		/// </summary>
		/// <param name="script">Script of the event.</param>
		public StiRefreshingEvent(string script) : base(script)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiRefreshingEvent with specified arguments.
		/// </summary>
		/// <param name="parent">Component which contain this event.</param>
		public StiRefreshingEvent(StiComponent parent) : base(parent)
		{
		}
	}
}
