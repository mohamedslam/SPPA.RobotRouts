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

using System;
using System.Collections;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the class for work with SumDistinct aggregate function.
    /// To calculate the type Decimal is used.
    /// </summary>
    public class StiSumDistinctDecimalFunctionService : StiAggregateFunctionService
    {
        #region StiService override
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => "SumDistinct";
        #endregion

        #region IStiStateSaveRestore
        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public override void SaveState(string stateName)
        {
            States.PushDecimal(stateName, this, "summary", summary);
            if (values != null)
                States.Push(stateName, this, "values", values.Clone());
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public override void RestoreState(string stateName)
        {
            if (States.IsExist(stateName, this))
            {
                summary = States.PopDecimal(stateName, this, "summary");
                values = States.Pop(stateName, this, "values") as Hashtable;
            }
        }
        #endregion

        #region Fields
        private decimal summary;
        private Hashtable values;
        #endregion

        #region Methods
        /// <summary>
        /// First initialization.
        /// </summary>
        public override void Init()
        {
            if (!this.RunningTotal || this.IsFirstInit)
            {
                summary = 0;
                values = new Hashtable();
            }
        }

        /// <summary>
        /// A value calculation.
        /// </summary>
        /// <param name="value">Value.</param>
        public override void CalcItem(object value)
        {
            if (values == null)
                values = new Hashtable();

            if (value == null || value == DBNull.Value) return;

            if (values[value] == null)
            {
                values.Add(value, value);
                summary += StiObjectConverter.ConvertToDecimal(value);
            }
        }

        public void CalcItem(object value, object valueToSum)
        {
            if (values == null)
                values = new Hashtable();

            if (value == null || value == DBNull.Value) return;

            if (values[value] == null)
            {
                values.Add(value, value);
                summary += StiObjectConverter.ConvertToDecimal(valueToSum);
            }
        }

        /// <summary>
        /// Returns the calculation result.
        /// </summary>
        /// <returns>Calculation result.</returns>
        public override object GetValue()
        {
            return summary;
        }

        /// <summary>
        /// Sets the calculation result.
        /// </summary>
        public override void SetValue(object value)
        {
            this.summary = (decimal)value;
        }

        /// <summary>
        /// Returns the type of the result.
        /// </summary>
        public override Type GetResultType()
        {
            return typeof(decimal);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets value, indicates that this function require a parameter.
        /// </summary>
        public override bool RecureParam => true;
        #endregion

        public StiSumDistinctDecimalFunctionService(bool runningTotal)
            : base(runningTotal)
        {
        }

        public StiSumDistinctDecimalFunctionService()
        {
        }
    }
}