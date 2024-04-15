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
using System.Collections.Generic;

namespace Stimulsoft.Report.Events
{
	/// <summary>
    /// Represents the method that is invoked for fill subreport parameters.
	/// </summary>
    public delegate void StiFillParametersEventHandler(object sender, StiFillParametersEventArgs e);

	/// <summary>
	/// Describes an argument for the event FillParameters.
	/// </summary>
	public class StiFillParametersEventArgs : 
		EventArgs,
		IStiBlocklyValueEventArgs
	{
	    /// <summary>
		/// Gets or sets parameters collection.
		/// </summary>
		public virtual Dictionary<string, object> Value { get; set; } = new Dictionary<string,object>();

	    /// <summary>
        /// Creates a new object of the type StiFillParametersEventArgs.
		/// </summary>
		public StiFillParametersEventArgs() : this(new Dictionary<string, object>())
		{
		}

		/// <summary>
        /// Creates a new object of the type StiFillParametersEventArgs with specified arguments.
		/// </summary>
		/// <param name="value">Value</param>
        public StiFillParametersEventArgs(Dictionary<string, object> value)
		{
			this.Value = value;
		}
	}
}
