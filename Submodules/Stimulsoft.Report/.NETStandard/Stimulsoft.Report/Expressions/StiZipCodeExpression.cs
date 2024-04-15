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
	/// Describes the class that contains the expression for calculation of zipcode.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiZipCodeExpressionConverter))]
	public class StiZipCodeExpression : StiUnifiedExpression
	{
	    #region Properties
        [Browsable(false)]
		public override bool ApplyFormat => false;

	    /// <summary>
	    /// Gets value, indicates that this value is be a string.
	    /// </summary>
	    public override bool FullConvert => false;
        #endregion

        #region Methods
        /// <summary>
        /// Returns the event for processing of the expession when generation the report script.
        /// </summary>
        /// <returns>The event for processing of the expession.</returns>
        public override StiEvent GetDefaultEvent()
		{
			return new StiGetZipCodeEvent();
		}
        #endregion


	    /// <summary>
		/// Creates a new expression.
		/// </summary>
		public StiZipCodeExpression() : this(string.Empty)
		{
		}

		/// <summary>
		/// Creates a new expression.
		/// </summary>
        /// <param name="value">Expression value.</param>
		public StiZipCodeExpression(string value) : base(value)
		{
		}

		/// <summary>
		/// Creates a new expression.
		/// </summary>
		public StiZipCodeExpression(StiComponent parent, string propertyName) : base(parent, propertyName)
		{
		}
	}
}