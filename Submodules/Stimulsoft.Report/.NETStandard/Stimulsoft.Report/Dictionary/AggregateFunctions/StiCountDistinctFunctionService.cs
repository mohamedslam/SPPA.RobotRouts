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

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the class for work with CountDistinct aggregate function.
    /// To calculate the type Int64 is used.
    /// </summary>
    public class StiCountDistinctFunctionService : StiAggregateFunctionService
    {
        #region StiService override
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => "CountDistinct";
        #endregion

        #region IStiStateSaveRestore
        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public override void SaveState(string stateName)
        {
            States.PushInt64(stateName, this, "counter", counter);
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
                counter = States.PopInt64(stateName, this, "counter");
                values = States.Pop(stateName, this, "values") as Hashtable;
            }
        }
        #endregion

        #region Fields
        private long counter;
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
                counter = 0;
                values = new Hashtable();
            }
        }

        /// <summary>
        /// A value calculation.
        /// </summary>
        /// <param name="value">Value.</param>
        public override void CalcItem(object value)
        {
            if (values == null) values = new Hashtable();
            if (values[value] == null)
            {
                values.Add(value, value);
                counter++;
            }
        }

        /// <summary>
        /// Returns the calculation result.
        /// </summary>
        /// <returns>Calculation result.</returns>
        public override object GetValue()
        {
            return counter;
        }

        /// <summary>
        /// Sets the calculation result.
        /// </summary>
        public override void SetValue(object value)
        {
            this.counter = (long)value;
        }

        /// <summary>
        /// Returns the type of the result.
        /// </summary>
        public override Type GetResultType()
        {
            return typeof(long);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets value, indicates that this function require a parameter.
        /// </summary>
        public override bool RecureParam => true;
        #endregion

        public StiCountDistinctFunctionService(bool runningTotal) : base(runningTotal)
        {
        }

        public StiCountDistinctFunctionService()
        {
        }
    }
}
