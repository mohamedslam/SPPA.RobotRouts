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

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
    /// Describes attribute that is used to sets alias for business properties.
	/// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class StiAliasAttribute : Attribute
	{
	    /// <summary>
		/// Gets or sets the alias of the business object property.
		/// </summary>
        public string Alias { get; set; }

	    /// <summary>
        /// Initializes a new instance of the StiAliasAttribute.
		/// </summary>
        /// <param name="alias">Gets or sets the alias of the business object property.</param>
        public StiAliasAttribute(string alias)
		{
            this.Alias = alias;
		}
	}
}