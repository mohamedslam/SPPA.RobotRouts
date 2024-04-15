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

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Describes the class that contains the expression for tool tip calculation.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiToolTipExpressionConverter))]
	public class StiToolTipExpression : StiUnifiedExpression
	{
	    #region Properties
        [Browsable(false)]
		public override bool ApplyFormat => false;
        #endregion

        #region Methods
        /// <summary>
        /// Returns the event for processing of the expression while script report generation.
        /// </summary>
        /// <returns>The event to process the expression.</returns>
        public override StiEvent GetDefaultEvent()
		{
			return new StiGetToolTipEvent();
		}
        #endregion

        /// <summary>
        /// Creates a new expression.
        /// </summary>
        public StiToolTipExpression() : this(string.Empty)
		{
		}

		/// <summary>
		/// Creates a new expression.
		/// </summary>
		/// <param name="value">Expression value.</param>
		public StiToolTipExpression(string value) : base(value)
		{
		}

		/// <summary>
		/// Creates a new expression.
		/// </summary>
		public StiToolTipExpression(StiComponent parent, string propertyName) : base(parent, propertyName)
		{
		}
	}
}