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

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Represents the method that is invoked for data filtration.
	/// </summary>
	public delegate void StiFilterEventHandler(object sender, StiFilterEventArgs e);

	/// <summary>
	/// Describes an argument for the event Filter.
	/// </summary>
	public class StiFilterEventArgs : EventArgs
	{
	    /// <summary>
		/// Gets or sets value indicates is an element filtered of not.
		/// </summary>
		public virtual bool Value { get; set; }

	    /// <summary>
		/// Creates a new object of the type StiFilterEventArgs.
		/// </summary>
		public StiFilterEventArgs() : this(false)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiFilterEventArgs with specified arguments.
		/// </summary>
		/// <param name="value">Value indicates is an element filtered of not.</param>
		public StiFilterEventArgs(bool value)
		{
			this.Value = value;
		}
	}
}
