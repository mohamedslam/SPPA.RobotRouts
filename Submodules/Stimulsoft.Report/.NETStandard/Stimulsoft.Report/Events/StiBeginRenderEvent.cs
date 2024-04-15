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

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Describes the class for realization of the event BeginRender.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.Events.Design.StiBeginRenderEventConverter))]
	public class StiBeginRenderEvent: StiEvent
	{
		/// <summary>
		/// Returns the string representation of the event.
		/// </summary>
		public override string ToString()
		{
			return "BeginRender";
		}

		/// <summary>
		/// Creates a new object of the type StiBeginRenderEvent.
		/// </summary>
		public StiBeginRenderEvent() : this("")
		{
		}

		/// <summary>
		/// Creates a new object of the type StiBeginRenderEvent with specified arguments.
		/// </summary>
		/// <param name="script">Script of the event.</param>
		public StiBeginRenderEvent(string script) : base(script)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiEvent with specified arguments.
		/// </summary>
		/// <param name="parent">Component which contain this event.</param>
		public StiBeginRenderEvent(StiComponent parent) : base(parent)
		{
		}
	}
}
