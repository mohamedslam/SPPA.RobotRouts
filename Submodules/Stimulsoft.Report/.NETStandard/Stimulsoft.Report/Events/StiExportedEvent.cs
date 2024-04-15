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
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Events
{
    /// <summary>
    /// Describes the class for realization of the event Exported.
    /// </summary>
    [TypeConverter(typeof(Stimulsoft.Report.Events.Design.StiExportedEventConverter))]
    public class StiExportedEvent : StiEvent
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
                new StiParameterInfo(typeof(StiExportEventArgs), "e")
            };
        }

        /// <summary>
        /// Return the type of the event.
        /// </summary>
        /// <returns>Event type.</returns>
        public override Type GetEventType()
        {
            return typeof(StiExportEventHandler);
        }

        /// <summary>
        /// Returns the string representation of the event.
        /// </summary>
        public override string ToString()
        {
            return "Exported";
        }

        /// <summary>
        /// Creates a new object of the type StiExportedEvent.
        /// </summary>
        public StiExportedEvent() : this("")
        {
        }

        /// <summary>
        /// Creates a new object of the type StiExportedEvent with specified arguments.
        /// </summary>
        /// <param name="script">Script of the event.</param>
        public StiExportedEvent(string script) : base(script)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiEvent with specified arguments.
        /// </summary>
        /// <param name="parent">Component which contain this event.</param>
        public StiExportedEvent(StiComponent parent) : base(parent)
        {
        }
    }
}