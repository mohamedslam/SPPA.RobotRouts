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

using System.ComponentModel;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.CrossTab
{	
	/// <summary>
	/// Describes the class that contains an expression for calculation of display value.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.CrossTab.Design.StiDisplayCrossValueExpressionConverter))]
	public class StiDisplayCrossValueExpression : StiExpression
	{
		/// <summary>
		/// Returns the event for processing of the expression while script report generation.
		/// </summary>
		/// <returns>The event to process the expression.</returns>
		public override StiEvent GetDefaultEvent()
		{
			return new StiGetDisplayCrossValueEvent();
		}

        /// <summary>
        /// Gets value, indicates that the value is to be a string.
        /// </summary>
        public override bool FullConvert => false;

        /// <summary>
        /// Creates a new expression.
        /// </summary>
        public StiDisplayCrossValueExpression() : this(string.Empty)
		{
		}

		/// <summary>
		/// Creates a new expression.
		/// </summary>
        /// <param name="value">Expression value.</param>
		public StiDisplayCrossValueExpression(string value) : base(value)
		{
			
		}
	}
}