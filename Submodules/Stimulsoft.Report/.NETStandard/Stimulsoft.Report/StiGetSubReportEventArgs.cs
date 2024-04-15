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

namespace Stimulsoft.Report
{
	/// <summary>
	/// Represents the method that handles the GetSubReport event.
	/// </summary>
	public delegate void StiGetSubReportEventHandler(object sender, StiGetSubReportEventArgs e);

	/// <summary>
	/// Describes the class that contains data for the event GetSubReport.
	/// </summary>
	public class StiGetSubReportEventArgs : EventArgs
	{
	    public StiReport Report { get; set; }
        
	    public string SubReportName { get; }

	    /// <summary>
		/// Creates a new object of the type StiGetSubReportEventArgs.
		/// </summary>
        /// <param name="subReportName">The component on which it is necessary to go on.</param>
		public StiGetSubReportEventArgs(string subReportName)
		{
			this.SubReportName = subReportName;
		}
	}
}