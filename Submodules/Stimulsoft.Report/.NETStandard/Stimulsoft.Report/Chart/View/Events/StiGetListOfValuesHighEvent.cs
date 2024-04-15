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
using System.ComponentModel;
using System.Reflection;
using System.Drawing.Design;

using Stimulsoft.Base;
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Describes the class for realization of the event GetListOfValuesHighEvent.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.Events.Design.StiGetListOfValuesHighEventConverter))]
    public class StiGetListOfValuesHighEvent : StiEvent
	{
		/// <summary>
		/// Returns an array of event parameters.
		/// </summary>
		/// <returns>Array of event parameters.</returns>
		public override StiParameterInfo[] GetParameters()
		{
			return new StiParameterInfo[]{
											 new StiParameterInfo(typeof(object), "sender"),
											 new StiParameterInfo(typeof(StiGetValueEventArgs), "e")};
		}

		/// <summary>
		/// Return the type of the event.
		/// </summary>
		/// <returns>Event type.</returns>
		public override Type GetEventType()
		{
			return typeof(StiGetValueEventHandler);
		}

		/// <summary>
		/// Returns the string representation of the event.
		/// </summary>
		public override string ToString()
		{
			return "GetListOfValuesHigh";
		}

		/// <summary>
        /// Creates a new object of the type StiGetListOfValuesHighEvent.
		/// </summary>
		public StiGetListOfValuesHighEvent() : this("")
		{
		}

		/// <summary>
        /// Creates a new object of the type StiGetListOfValuesHighEvent with specified arguments.
		/// </summary>
		/// <param name="script">Script of the event.</param>
		public StiGetListOfValuesHighEvent(string script):base(script)
		{
		}

		/// <summary>
        /// Creates a new object of the type StiGetListOfValuesHighEvent with specified arguments.
		/// </summary>
		/// <param name="parent">Component which contain this event.</param>
		public StiGetListOfValuesHighEvent(StiComponent parent) : base(parent)
		{
		}
	}
}
