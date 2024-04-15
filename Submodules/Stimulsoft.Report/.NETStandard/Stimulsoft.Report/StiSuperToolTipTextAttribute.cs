#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
	/// Class describes an attribute that is used for assinging SuperToolTip text to custom component.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public sealed class StiSuperToolTipTextAttribute : Attribute
	{
	    /// <summary>
        /// Gets or sets a path to the SuperToolTip bitmap in resources.
		/// </summary>
		public string Text { get; set; }

	    /// <summary>
        /// Creates a new attribute of the type StiSuperToolTipTextAttribute.
        /// </summary>
        /// <param name="text">SuperToolTip text.</param>
        public StiSuperToolTipTextAttribute(string text)
		{
            this.Text = text;
		}
	}
}
