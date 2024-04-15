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
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        #region Functions
        [Description("Returns a label of the specified variable.")]
        public string GetLabel(string variableName)
        {
            return StiVariableHelper.GetVariableLabel(this, variableName);
        }

        [Description("Returns a value of the specified variable or parameter.")]
        public object GetParam(string paramName)
        {
            return (Dictionary as IStiAppDictionary)?.GetVariableValueByName(paramName);
        }

        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result equal to 0.
        /// </summary>
        [Description("Returns result of dividing value1 and value2. If value2 is zero, then result equal to 0.")]
        public static long Div(long value1, long value2)
        {
            return Div(value1, value2, 0);
        }

        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).
        /// </summary>
        [Description("Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).")]
        public static long Div(long value1, long value2, long zeroResult)
        {
            if (value2 == 0) 
                return zeroResult;

            return value1 / value2;
        }

        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result equal to 0.
        /// </summary>
        [Description("Returns result of dividing value1 and value2. If value2 is zero, then result equal to 0.")]
        public static double Div(double value1, double value2)
        {
            return Div(value1, value2, 0);
        }

        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).
        /// </summary>
        [Description("Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).")]
        public static double Div(double value1, double value2, double zeroResult)
        {
            if (value2 == 0) 
                return zeroResult;

            return value1 / value2;
        }

        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result equal to 0.
        /// </summary>
        [Description("Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).")]
        public static decimal Div(decimal value1, decimal value2)
        {
            return Div(value1, value2, 0);
        }

        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).
        /// </summary>
        [Description("Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).")]
        public static decimal Div(decimal value1, decimal value2, decimal zeroResult)
        {
            if (value2 == 0) 
                return zeroResult;

            return value1 / value2;
        }

        /// <summary>
        /// Returns value1 if condition is true and value2 if condition is false.
        /// </summary>
        [Description("Returns value1 if condition is true and value2 if condition is false. Base type is object.")]
        protected object IIF(bool condition, object value1, object value2)
        {
            return condition ? value1 : value2;
        }

        /// <summary>
        /// Returns true if current row of dataColumn from dataSource contain null or DbNull.Value.
        /// </summary>
        /// <param name="dataSource">Name of Data Source or reference to Data Source.</param>
        /// <param name="dataColumn">Name of Data Column.</param>
        /// <returns>Result of checking.</returns>
        [Description("Returns true if current row of dataColumn from dataSource contain null or DbNull.Value.")]
        protected bool IsNull(object dataSource, string dataColumn)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException(
                    "Function IsNull: First argument can't be equal to null." +
                    " Please provide name of Data Source.");
            }

            if (dataColumn == null)
            {
                throw new ArgumentNullException(
                    "Function IsNull: Second argument can't be equal to null." +
                    " Please provide name of Data Column.");
            }

            #region Data Source
            var dataSourceObject = dataSource as StiDataSource;
            var businessObject = dataSource as StiBusinessObject;
            if (dataSourceObject == null && businessObject == null)
            {
                if (dataSource is string)
                {
                    dataSourceObject = this.Dictionary.DataSources[dataSource as string];
                    if (dataSourceObject == null)
                        throw new ArgumentException($"Function IsNull: Data Source '{dataSource}' is not exist.");
                }
                else
                    throw new ArgumentException($"Function IsNull: Data Source {dataSource} is not exist.");
            }
            #endregion

            if (businessObject != null)
            {
                var value = businessObject[dataColumn];
                return value == null || value == DBNull.Value;
            }
            else
            {
                var value = dataSourceObject[dataColumn];
                return value == null || value == DBNull.Value;
            }
        }

        /// <summary>
        /// Returns true if the next field value is equal to null or DBNull.Value.
        /// </summary>
        /// <param name="dataSource">Name of Data Source or reference to Data Source.</param>
        /// <param name="dataColumn">Name of Data Column.</param>
        /// <returns>Result of checking.</returns>
        [Description("Returns true if the next field value is equal to null or DBNull.Value.")]
        protected bool NextIsNull(object dataSource, string dataColumn)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException(
                    "Function IsNull: First argument can't be equal to null." +
                    " Please provide name of Data Source.");
            }

            if (dataColumn == null)
            {
                throw new ArgumentNullException(
                    "Function IsNull: Second argument can't be equal to null." +
                    " Please provide name of Data Column.");
            }

            #region Data Source
            var dataSourceObject = dataSource as StiDataSource;
            var businessObject = dataSource as StiBusinessObject;
            if (dataSourceObject == null && businessObject == null)
            {
                if (dataSource is string)
                {
                    dataSourceObject = this.Dictionary.DataSources[dataSource as string];
                    if (dataSourceObject == null)
                        throw new ArgumentException($"Function IsNull: Data Source '{dataSource}' is not exist.");
                }
                else
                    throw new ArgumentException($"Function IsNull: Data Source {dataSource} is not exist.");
            }
            #endregion

            var guid = Guid.NewGuid().ToString();

            dataSourceObject?.SaveState(guid);
            businessObject?.SaveState(guid);

            object value = null;

            try
            {
                if (dataSourceObject != null)
                {
                    dataSourceObject.Next();
                    if (dataSourceObject.IsEof) return false;
                    value = dataSourceObject[dataColumn];
                }

                if (businessObject != null)
                {
                    businessObject.Next();
                    if (businessObject.IsEof) 
                        return false;

                    value = businessObject[dataColumn];
                }
            }
            finally
            {
                dataSourceObject?.RestoreState(guid);
                businessObject?.RestoreState(guid);
            }

            return value == null || value == DBNull.Value;
        }

        /// <summary>
		/// Returns true if the previous field value is equal to null or DBNull.Value.
		/// </summary>
		/// <param name="dataSource">Name of Data Source or reference to Data Source.</param>
		/// <param name="dataColumn">Name of Data Column.</param>
		/// <returns>Result of checking.</returns>
        [Description("Returns true if the previous field value is equal to null or DBNull.Value.")]
        protected object LastOnPage(object dataSource, string dataColumn)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException(
                    "Function IsNull: First argument can't be equal to null." +
                    " Please provide name of Data Source.");
            }

            if (dataColumn == null)
            {
                throw new ArgumentNullException(
                    "Function IsNull: Second argument can't be equal to null." +
                    " Please provide name of Data Column.");
            }

            #region Data Source
            var dataSourceObject = dataSource as StiDataSource;
            var businessObject = dataSource as StiBusinessObject;
            if (dataSourceObject == null && businessObject == null)
            {
                if (dataSource is string)
                {
                    dataSourceObject = this.Dictionary.DataSources[dataSource as string];
                    if (dataSourceObject == null)
                        throw new ArgumentException($"Function IsNull: Data Source '{dataSource}' is not exist.");
                }
                else
                {
                    throw new ArgumentException($"Function IsNull: Data Source {dataSource} is not exist.");
                }
            }
            #endregion

            if (dataSourceObject != null)
            {
                return dataSourceObject.IsEof 
                    ? dataSourceObject[dataColumn] 
                    : StiFunctionsPrintState.Previous(dataSource, dataColumn);
            }

            return businessObject.IsEof 
                ? businessObject[dataColumn] 
                : StiFunctionsPrintState.Previous(dataSource, dataColumn);
        }

        /// <summary>
        /// Returns true if the previous field value is equal to null or DBNull.Value.
        /// </summary>
        /// <param name="dataSource">Name of Data Source or reference to Data Source.</param>
        /// <param name="dataColumn">Name of Data Column.</param>
        /// <returns>Result of checking.</returns>
        [Description("Returns true if the previous field value is equal to null or DBNull.Value.")]
        protected bool PreviousIsNull(object dataSource, string dataColumn)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException(
                    "Function IsNull: First argument can't be equal to null." +
                    " Please provide name of Data Source.");
            }

            if (dataColumn == null)
            {
                throw new ArgumentNullException(
                    "Function IsNull: Second argument can't be equal to null." +
                    " Please provide name of Data Column.");
            }

            #region Data Source
            var dataSourceObject = dataSource as StiDataSource;
            var businessObject = dataSource as StiBusinessObject;
            if (dataSourceObject == null && businessObject == null)
            {
                if (dataSource is string)
                {
                    dataSourceObject = this.Dictionary.DataSources[dataSource as string];
                    if (dataSourceObject == null)
                        throw new ArgumentException($"Function IsNull: Data Source '{dataSource}' is not exist.");
                }
                else
                {
                    throw new ArgumentException($"Function IsNull: Data Source {dataSource} is not exist.");
                }
            }
            #endregion

            string guid = Guid.NewGuid().ToString();
            dataSourceObject?.SaveState(guid);
            businessObject?.SaveState(guid);

            object value = null;

            try
            {
                if (dataSourceObject != null)
                {
                    dataSourceObject.Prior();
                    if (dataSourceObject.IsBof) 
                        return false;

                    value = dataSourceObject[dataColumn];
                }
                if (businessObject != null)
                {
                    businessObject.Prior();
                    if (businessObject.IsBof) 
                        return false;

                    value = businessObject[dataColumn];
                }

            }
            finally
            {
                dataSourceObject?.RestoreState(guid);
                businessObject?.RestoreState(guid);
            }

            return value == null || value == DBNull.Value;
        }

        /// <summary>
        /// Returns formated string of the specified arguments.
        /// </summary>
        [Description("Returns formated string of the specified arguments.")]
        protected string Format(string format, object arg)
        {
            return string.Format(format, arg);
        }
        #endregion
    }
}