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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
 

namespace Stimulsoft.Report.CodeDom
{
	/// <summary>
	/// Help class for storage of information about aggregate function in the script.
	/// </summary>
	public class StiRemit
	{

		#region Fields
		internal StiComponent Component;

		internal StiComponent BuildComponent;

		internal string Name;

        internal string RunningFooterBandName = null;

		/// <summary>
		/// Gets or sets script for calculation of an aggregate function element.
		/// </summary>
		internal string ScriptCalcItem;	

		/// <summary>
		/// Gets or sets script for assignment a final value to a component.
		/// </summary>
		internal string ScriptGetValue;

		/// <summary>
		/// Gets or sets script of condition.
		/// </summary>
		internal string ScriptCondition;

		/// <summary>
		/// Gets or sets an aggregate function class.
		/// </summary>
		internal StiAggregateFunctionService Function;

		/// <summary>
		/// Gets or sets value indicates that it is necessary to use events of printing.
		/// </summary>
		internal bool UsePrintEvent;

		/// <summary>
		/// Gets or sets value indicates that it is necessary to use events of column.
		/// </summary>
		internal bool UseColumn;

		internal StiExpression Expression;

        /// <summary>
        /// Expression contain running total.
        /// </summary>
        internal bool isRunningTotal;

        /// <summary>
        /// Expression contain page total.
        /// </summary>
        internal bool isPageTotal;

        internal bool ConnectOnlyBegin;

		#endregion


		/// <summary>
		/// Creates an object of the type StiRemit.
		/// </summary>
		public StiRemit(
			StiComponent Component, 
			string name, 
			string scriptCalcItem, 
			string scriptGetValue, 
			string scriptCondition,
			StiAggregateFunctionService function, 
			bool usePrintEvent,
			bool useColumn,
			StiExpression expression,
            bool isRunningTotal)
		{
			this.Component = Component;
			this.BuildComponent = Component;
			this.Name = name;
			this.ScriptCalcItem = scriptCalcItem;
			this.ScriptGetValue = scriptGetValue;
			this.ScriptCondition  = scriptCondition;
			this.Function = function;
			this.UsePrintEvent = usePrintEvent;
			this.UseColumn = useColumn;
			this.Expression = expression;
            this.isRunningTotal = isRunningTotal;
		}
	}
}
