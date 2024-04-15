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

using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// The class describes the base class of condition.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
    [TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiConditionConverter))]
	public class StiBaseCondition : StiFilter
    {
        #region Properties
        [StiNonSerialized]
        public object Tag { get; set; }
        #endregion

        /// <summary>
		/// Creates a new object of the type StiBaseCondition.
		/// </summary>
		public StiBaseCondition()
		{
		}

		/// <summary>
		/// Creates a new object of the type StiBaseCondition.
		/// </summary>
		public StiBaseCondition(string expression) : base(expression)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiBaseCondition.
		/// </summary>
		public StiBaseCondition(string column, StiFilterCondition condition, DateTime date1) :
			base(column, condition, date1)
		{			
		}

		/// <summary>
		/// Creates a new object of the type StiBaseCondition.
		/// </summary>
		public StiBaseCondition(string column, StiFilterCondition condition, DateTime date1, DateTime date2) :
			base(column, condition, date1, date2)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiBaseCondition.
		/// </summary>
		public StiBaseCondition(string column, StiFilterCondition condition, string value, StiFilterDataType dataType) :
			base(column, condition, value, dataType)
		{
			
		}

		/// <summary>
		/// Creates a new object of the type StiBaseCondition.
		/// </summary>
		public StiBaseCondition(string column, StiFilterCondition condition, string value1, string value2, StiFilterDataType dataType) :
			base(column, condition, value1, value2, dataType)
		{
		}

		/// <summary>
        /// Creates a new object of the type StiBaseCondition.
		/// </summary>
        public StiBaseCondition(StiFilterItem item, string column, StiFilterCondition condition, 
			string value1, string value2, StiFilterDataType dataType, string expression) : 
            base(item, column, condition, value1, value2, dataType, expression)
		{
		}
    }
}
