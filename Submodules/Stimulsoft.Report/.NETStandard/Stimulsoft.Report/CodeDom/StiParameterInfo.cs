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

namespace Stimulsoft.Report.CodeDom
{
	/// <summary>
	/// Class describes parameters of user's report functions.
	/// </summary>
	public class StiParameterInfo
	{
	    /// <summary>
		/// Gets or sets the type of a parameter.
		/// </summary>
		public Type Type { get; set; }

	    /// <summary>
		/// Gets or sets parameter name.
		/// </summary>
		public string Name { get; set; }

	    /// <summary>
		/// Creates a new instance of the StiParameterInfo class.
		/// </summary>
		/// <param name="type">Parameter type.</param>
		/// <param name="name">Parameter name.</param>
		public StiParameterInfo(Type type, string name)
		{
			this.Type = type;
			this.Name = name;
		}
	}
}
