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
    /// Describes the class for work with Rank aggregate function.
    /// To calculate the type Int is used.
    /// </summary>
    public class StiRankFunctionService : StiAggregateFunctionService
    {
        #region StiService override
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => "Rank";
        #endregion

        #region IStiStateSaveRestore
        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public override void SaveState(string stateName)
        {
            States.Push(stateName, this, "rank", hash);
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public override void RestoreState(string stateName)
        {
            if (States.IsExist(stateName, this))
                hash = (Hashtable) States.Pop(stateName, this, "rank");
        }
        #endregion

        #region Fields
        private Hashtable hash = new Hashtable();
        private StiRankOrder sortOrder = StiRankOrder.Asc;
        private bool dense = true;
        #endregion

        #region Methods
        /// <summary>
        /// First initialization.
        /// </summary>
        public override void Init()
        {
            if (this.IsFirstInit)
                hash.Clear();
        }

        /// <summary>
        /// A value calculation.
        /// </summary>
        /// <param name="value">Value.</param>
        public override void CalcItem(object value)
        {
            if (value == null || value == DBNull.Value) return;

            if (hash.ContainsKey(value))
            {
                var n = (int) hash[value];
                hash[value] = n + 1;
            }
            else
                hash[value] = 1;
        }

        /// <summary>
        /// Returns the calculation result.
        /// </summary>
        /// <returns>Calculation result.</returns>
        public override object GetValue()
        {
            var keys = new object[hash.Keys.Count];
            hash.Keys.CopyTo(keys, 0);
            Array.Sort(keys);

            if (sortOrder == StiRankOrder.Desc)
                Array.Reverse(keys);

            var index = 1;
            foreach (var obj in keys)
            {
                var n = (int)hash[obj];
                hash[obj] = index;
                index += dense ? 1 : n;
            }
            return hash;
        }

        /// <summary>
        /// Sets the calculation result.
        /// </summary>
        public override void SetValue(object value)
        {
            hash = (Hashtable)value;
        }

        /// <summary>
        /// Returns the type of the result.
        /// </summary>
        public override Type GetResultType()
        {
            return typeof(Hashtable);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets value, indicates that this function require a parameter.
        /// </summary>
        public override bool RecureParam => true;
        #endregion

        public StiRankFunctionService(bool runningTotal, bool dense, StiRankOrder sortOrder)
            : base(runningTotal)
        {
            this.dense = dense;
            this.sortOrder = sortOrder;
        }

        public StiRankFunctionService(bool runningTotal)
            : base(runningTotal)
        {
        }

        public StiRankFunctionService()
        {
        }
    }
}
