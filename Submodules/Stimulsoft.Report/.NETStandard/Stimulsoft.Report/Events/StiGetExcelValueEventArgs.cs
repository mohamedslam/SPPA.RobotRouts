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

using Stimulsoft.Base.Blocks;
using System;

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Represents the method that handles the GetExcelValue event.
	/// </summary>
	public delegate void StiGetExcelValueEventHandler(object sender, StiGetExcelValueEventArgs e);

	/// <summary>
	/// Describes an argument for the event GetExcelValue.
	/// </summary>
	public class StiGetExcelValueEventArgs :
		EventArgs,
		IStiBlocklyValueEventArgs
	{
	    /// <summary>
		/// Internal use only.
		/// </summary>
		public virtual bool StoreToPrinted { get; set; }

	    /// <summary>
		/// Gets or sets the value.
		/// </summary>
		public virtual string Value { get; set; }
	}
}
