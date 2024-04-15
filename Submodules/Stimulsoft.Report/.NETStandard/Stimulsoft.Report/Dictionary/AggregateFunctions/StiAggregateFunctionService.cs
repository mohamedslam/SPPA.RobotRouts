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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.CodeDom;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the base class for work with aggregate function.
    /// </summary>
    [StiServiceBitmap(typeof(StiAggregateFunctionService), "Stimulsoft.Report.Bmp.AggregateFunction.bmp")]
    public abstract class StiAggregateFunctionService :
        StiService,
        IStiStateSaveRestore
    {
        #region StiService override
        /// <summary>
        /// Gets a service category.
        /// </summary>
        public sealed override string ServiceCategory => StiLocalization.Get("Services", "categoryDictionary");

        /// <summary>
        /// Gets a service type.
        /// </summary>
        public sealed override Type ServiceType => typeof(StiAggregateFunctionService);
        #endregion

        #region IStiStateSaveRestore
        private StiStatesManager states;
        /// <summary>
        /// Gets the component states manager.
        /// </summary>
        protected StiStatesManager States
        {
            get
            {
                return states ?? (states = new StiStatesManager());
            }
        }

        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public abstract void SaveState(string stateName);

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public abstract void RestoreState(string stateName);

        /// <summary>
        /// Clear all earlier saved object states.
        /// </summary>
        public virtual void ClearAllStates()
        {
            states = null;
        }

        public override void PackService()
        {
            base.PackService();
            states = null;
        }
        #endregion

        #region Methods
        /// <summary>
        /// First initialization.
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Reset result of total calculation.
        /// </summary>
        public virtual void Reset()
        {
            var resRunningTotal = this.RunningTotal;
            this.RunningTotal = false;
            this.Init();
            this.RunningTotal = resRunningTotal;
        }

        /// <summary>
        /// A value calculation.
        /// </summary>
        /// <param name="value">Value.</param>
        public abstract void CalcItem(object value);

        /// <summary>
        /// Returns the calculation result.
        /// </summary>
        /// <returns>Calculation result.</returns>
        public abstract object GetValue();

        /// <summary>
        /// Sets the calculation result.
        /// </summary>
        public abstract void SetValue(object value);

        /// <summary>
        /// Returns the type of the result.
        /// </summary>
        public abstract Type GetResultType();

        private string ConvertTypeToString(Type type, StiReportLanguageType language)
        {
            var typeStr = StiCodeDomSerializator.ConvertTypeToString(type, language);

            return string.IsNullOrEmpty(typeStr) 
                ? type.ToString() 
                : typeStr;
        }

        public string GetLongFunctionString(StiReportLanguageType language)
        {
            if (language == StiReportLanguageType.CSharp || language == StiReportLanguageType.JS)
            {
                var sb = new StringBuilder();
                sb.Append(ConvertTypeToString(this.GetResultType(), language));
                sb.Append("  ");
                sb.Append(this.ServiceName);
                sb.Append(" (");

                if (RecureParam)
                {
                    var argumentType = typeof(object);
                    var argumentName = "value";

                    sb.Append(ConvertTypeToString(argumentType, language));
                    sb.Append(" ");
                    sb.Append(argumentName);
                }

                sb.Append(")");
                return sb.ToString();
            }
            else
            {
                var sb = new StringBuilder();

                sb.Append(this.ServiceName);
                sb.Append("(");

                var argumentType = typeof(object);
                var argumentName = "Value";
                sb.Append(argumentName);
                sb.Append(" As ");
                sb.Append(ConvertTypeToString(argumentType, language));

                sb.Append(")");
                if (this.GetResultType() != typeof(void))
                    sb.Append(" As " + ConvertTypeToString(this.GetResultType(), language));

                return sb.ToString();
            }
        }
        #endregion

        #region Methods.Static
        /// <summary>
        /// Returns the aggregate function from the service container by its name.
        /// </summary>
        /// <param name="services">The container of services that contains a fuction.</param>
        /// <param name="name">Function name.</param>
        /// <returns>Function.</returns>
        public static StiAggregateFunctionService GetFunction(List<StiAggregateFunctionService> services, string name)
        {
            return services.FirstOrDefault(s => s.ServiceName == name);
        }
        #endregion

        #region Properties
        public bool IsFirstInit { get; set; }

        /// <summary>
		/// Gets value, indicates that this function require a parameter.
		/// </summary>
        [Browsable(false)]
        public abstract bool RecureParam { get; }

        [Browsable(false)]
        public bool RunningTotal { get; set; }
        #endregion

        public StiAggregateFunctionService(bool runningTotal)
        {
            this.RunningTotal = runningTotal;
        }

        public StiAggregateFunctionService()
        {
        }
    }
}
