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
	/// Represents the method that handles the GetValue event.
	/// </summary>
	public delegate void StiGetValueEventHandler(object sender, StiGetValueEventArgs e);

	/// <summary>
	/// Describes an argument for the event GetValue.
	/// </summary>
	public class StiGetValueEventArgs :
		EventArgs,
		IStiBlocklyValueEventArgs
	{
	    /// <summary>
		/// Gets or sets the total value.
		/// </summary>
		public virtual string Value { get; set; }

	    /// <summary>
		/// Gets or sets value indicates  - whether the component is to be saved in the collection of printed components or not.
		/// </summary>
		public virtual bool StoreToPrinted { get; set; }
	}
}
