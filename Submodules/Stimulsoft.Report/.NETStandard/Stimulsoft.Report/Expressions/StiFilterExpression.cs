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
	/// Describes the class that contains the expression for filter calculation.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiFilterExpressionConverter))]
	public class StiFilterExpression : StiUnifiedExpression
	{
	    #region Properties
        [Browsable(false)]
		public override bool ApplyFormat => false;

	    /// <summary>
	    /// Gets value, indicates that it is necessary to add methods of getting the expression value to the event handler.
	    /// </summary>
	    public override bool GenAddEvent => false;

	    /// <summary>
	    /// Gets value, indicates that the value is to be a string.
	    /// </summary>
	    public override bool FullConvert => false;
        #endregion

        #region Methods
        /// <summary>
        /// Returns the event for generation of the expression when report script generation.
        /// </summary>
        /// <returns>The event for processing of the expression.</returns>
        public override StiEvent GetDefaultEvent()
		{
			return new StiGetFilterEvent();
		}
        #endregion

	    /// <summary>
		/// Creates a new expression.
		/// </summary>
		public StiFilterExpression() : this(string.Empty)
		{
		}

		/// <summary>
		/// Creates a new expression.
		/// </summary>
        /// <param name="value">Expression value.</param>
		public StiFilterExpression(string value) : base(value)
		{
			
		}

		/// <summary>
		/// Creates a new expression.
		/// </summary>
		public StiFilterExpression(StiComponent parent, string propertyName) : base(parent, propertyName)
		{
		}
	}
}
