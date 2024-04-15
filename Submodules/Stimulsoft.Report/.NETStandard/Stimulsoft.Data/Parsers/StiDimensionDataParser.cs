#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Expressions.NCalc;
using Stimulsoft.Data.Functions;
using Stimulsoft.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Data.Parsers
{
    public class StiDimensionDataParser : StiDataParser
    {
        #region Methods
        public object[] Calculate(object[] row, List<IStiMeter> meters)
        {
			this.currentRow = row;           

            return NormalizeDates(Dimensions.Select(CalculateDimension).ToArray(), meters);
        }

        private static object[] NormalizeDates(object[] values, List<IStiMeter> meters)
        {
            if (meters == null)
                return values;

            var meterIndex = 0;
            return values.Select(v => NormalizeDate(v, GetMeter(meters, meterIndex++))).ToArray();
        }

        private static IStiMeter GetMeter(List<IStiMeter> meters, int meterIndex)
        {
            return meterIndex < meters.Count 
                ? meters[meterIndex] 
                : null;
        }

        private static object NormalizeDate(object value, IStiMeter meter)
        {
            var format = meter as IStiDataFormat;
            if (format != null && format.GetDataFormat() == StiDataFormatKind.Time)
                return value;

            return value is DateTime
                ? StiDateTimeCorrector.Correct((DateTime)value)
                : value;
        }

        private object CalculateDimension(IStiDimensionMeter dimension)
        {
            if (string.IsNullOrWhiteSpace(dimension.Expression))
                return null;

            var columnName = GetDimensionGroupColumn(dimension);
            if (columnName == null)
                return CalculateDimensionExpression(dimension);
            else
                return CalculateDimensionGroup(columnName);
        }

        private string GetDimensionGroupColumn(IStiDimensionMeter dimension)
        {
            if (Table == null)
                return null;

            if (string.IsNullOrWhiteSpace(dimension.Expression))
                return null;

            if (expressionToColumn.ContainsKey(dimension.Expression))
                return expressionToColumn[dimension.Expression];

            var expression = Funcs.ToDataName(dimension.Expression);
            var column = Table.Columns
                .Cast<DataColumn>()
                .FirstOrDefault(c => Funcs.ToDataName(c.ColumnName) == expression)?
                .ColumnName;

            expressionToColumn.Add(dimension.Expression, column);

            return column;
        }

        public object CalculateDimensionExpression(IStiDimensionMeter dimension)
        {
            return CalculateDimensionExpression(dimension.Expression);
        }

        public object CalculateDimensionExpression(string expression, bool throwExceptionForColumnName = false)
        {
            return GetExpression(expression, throwExceptionForColumnName)?.Evaluate();
        }

        private object CalculateDimensionGroup(string columnName)
        {
            var columnIndex = GetDataColumnIndex(columnName);
            var value = currentRow?[columnIndex];

            return NormalizeEnum(value, columnIndex);
        }

        private object NormalizeEnum(object value, int columnIndex)
        {
            if (value is int)
            {
                var columnType = Table.Columns[columnIndex].DataType;

                if (columnType.IsEnum)
                    value = Enum.ToObject(columnType, value);
            }

            return value;
        }

        private Expression GetExpression(string expression, bool throwExceptionForColumnName = false)
        {
            if (queryToExpression.ContainsKey(expression))
                return queryToExpression[expression];

            var expObject = StiExpressionHelper.NewExpression(expression);

            expObject.EvaluateFunction += (name, args) => args.Result = RunFunction(name, args);
            expObject.EvaluateParameter += delegate(string name, ParameterArgs args)
            {
                if (IsSystemVariable(name))
                {
                    args.Result = GetSystemVariableValue(name);
                }
                else if (IsVariable(name))
                {
                    args.Result = GetVariableValue(name);
                }
                else
                {
                    var index = GetDataColumnIndex(name);
                    if (index >= 0 && index < currentRow.Length)
                    {
                        args.Result = currentRow[index];
                    }
                    else
                    {
                        if (throwExceptionForColumnName)
                            throw new Exception($"The '{name}' ident is not found.");

                        args.Result = null;
                    }
                }
            };

            queryToExpression[expression] = expObject;

            return expObject;
        }
        #endregion

        #region Properties
        protected IEnumerable<IStiDimensionMeter> Dimensions { get; }
        #endregion

        #region Fields
		private object[] currentRow;
        private Dictionary<string, Expression> queryToExpression = new Dictionary<string, Expression>();
        private Dictionary<string, string> expressionToColumn = new Dictionary<string, string>();
        #endregion

        public StiDimensionDataParser(IStiAppDictionary dictionary, IStiMeter meter)
            : this(dictionary, null, new List<IStiMeter> { meter })
        {
        }

        public StiDimensionDataParser(IStiAppDictionary dictionary, DataTable table, List<IStiMeter> meters) 
            : base(dictionary, table, meters)
        {
            this.Dimensions = Meters.Where(m => m is IStiDimensionMeter).Cast<IStiDimensionMeter>();
    	}
    }
}