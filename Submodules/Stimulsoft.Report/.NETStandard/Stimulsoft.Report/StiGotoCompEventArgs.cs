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
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report
{
	/// <summary>
	/// Represents the method that handles the GotoComp event.
	/// </summary>
	public delegate void StiGotoCompEventHandler(object sender, StiGotoCompEventArgs e);

	/// <summary>
	/// Describes the class that contains data for the event GotoComp.
	/// </summary>
	public class StiGotoCompEventArgs : EventArgs
	{
	    /// <summary>
		/// Gets or sets the component on which it is necessary to go on.
		/// </summary>
		public StiComponent Component { get; set; }

	    /// <summary>
		/// Creates a new object of the type StiGotoCompArgs.
		/// </summary>
		/// <param name="component">The component on which it is necessary to go on.</param>
		public StiGotoCompEventArgs(StiComponent component)
		{
			this.Component = component;
		}
	}
}
