#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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

using System.ComponentModel;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components.Design;

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Describes the class that contains the expression for calculation of Title.
	/// </summary>
	[TypeConverter(typeof(StiTitleExpressionConverter))]
	public class StiTitleExpression : StiExpression
	{
		[Browsable(false)]
		public override bool ApplyFormat => false;

	    /// <summary>
		/// Returns the event for processing of the expession when generation the report script.
		/// </summary>
		/// <returns>The event for processing of the expession.</returns>
		public override StiEvent GetDefaultEvent()
		{
			return new StiGetTitleEvent();
		}

		/// <summary>
		/// Gets value, indicates that this value is be a string.
		/// </summary>
		public override bool FullConvert => true;

	    /// <summary>
		/// Creates a new expression.
		/// </summary>
		public StiTitleExpression() : this(string.Empty)
		{
		}

		/// <summary>
		/// Creates a new expression.
		/// </summary>
        /// <param name="value">Expression value.</param>
		public StiTitleExpression(string value) : base(value)
		{
		}
	}
}