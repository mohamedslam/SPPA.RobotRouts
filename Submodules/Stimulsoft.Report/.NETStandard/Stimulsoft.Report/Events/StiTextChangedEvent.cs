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
	/// Represents the method that handles the events which occurs when text value is changed.
	/// </summary>
	public delegate void StiTextChangedEventHandler(object sender, StiTextChangedEventArgs e);

	/// <summary>
	/// Describes an argument for the event TextChanged. 
	/// </summary>
	public class StiTextChangedEventArgs : EventArgs
	{
	    /// <summary>
		/// Gets or sets the old value of text.
		/// </summary>
		public string OldValue { get; }

	    /// <summary>
		/// Gets or sets the new value of text.
		/// </summary>
		public string NewValue { get; }

	    /// <summary>
		/// Creates a new object of the type StiTextChangedEventArgs.
		/// </summary>
        /// <param name="oldValue">Old value of text.</param>
        /// <param name="newValue">New value of text.</param>
		public StiTextChangedEventArgs(string oldValue, string newValue)
		{
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}
	}
}