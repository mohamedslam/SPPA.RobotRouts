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

using Stimulsoft.Report.CodeDom;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events.Design;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Events
{
    /// <summary>
    /// Describes the class for realization of the event StiGetBarCodeEvent.
    /// </summary>
    [TypeConverter(typeof(StiGetBarCodeEventConverter))]
	public class StiGetBarCodeEvent : StiEvent
	{
		/// <summary>
		/// Returns an array of event parameters.
		/// </summary>
		/// <returns>Array of event parameters.</returns>
		public override StiParameterInfo[] GetParameters()
		{
			return new[]
			{
				new StiParameterInfo(typeof(object), "sender"),
				new StiParameterInfo(typeof(StiValueEventArgs), "e")
			};
		}

		/// <summary>
		/// Return the type of the event.
		/// </summary>
		/// <returns>Event type.</returns>
		public override Type GetEventType() => typeof(StiValueEventHandler);

		/// <summary>
		/// Returns the string representation of the event.
		/// </summary>
		public override string ToString() => "GetBarCode";

		/// <summary>
		/// Creates a new object of the type StiBarCodeEvent.
		/// </summary>
		public StiGetBarCodeEvent() : this("")
		{
		}

		/// <summary>
		/// Creates a new object of the type StiGetBarCodeEvent with specified arguments.
		/// </summary>
		/// <param name="script">Script of the event.</param>
		public StiGetBarCodeEvent(string script) : base(script)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiEvent with specified arguments.
		/// </summary>
		/// <param name="parent">Component which contain this event.</param>
		public StiGetBarCodeEvent(StiComponent parent) : base(parent)
		{
		}
	}
}
