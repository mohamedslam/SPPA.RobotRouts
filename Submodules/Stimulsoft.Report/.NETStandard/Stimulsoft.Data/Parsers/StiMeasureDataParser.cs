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

using Stimulsoft.Data.Expressions.NCalc;
using Stimulsoft.Data.Functions;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Stimulsoft.Base;

namespace Stimulsoft.Data.Parsers
{
    public class StiMeasureDataParser : StiDataParser
    {
        #region Methods
        public IEnumerable<object[]> Calculate()
        {
            return grandRows.Select(rows => Meters
                .Select(meter => CalculateMeter(meter, rows.Key, rows))
                .ToArray());
        }

        public object CalculateMeter(IStiMeter meter, object[] keys = null, IEnumerable<object[]> rows = null)
        {
            var dimension = meter as IStiDimensionMeter;
            if (dimension != null)
                return CalculateDimension(dimension, keys);

            var measure = meter as IStiMeasureMeter;
            if (measure != null)
            {
                var pair = GetMeasureColumn(measure);

                if (pair == null)
                    return CalculateMeasureExpression(measure, rows);
                else
                    return CalculateMeasureFunction(pair.Function, pair.ColumnName, rows);
            }

            throw new NotSupportedException();
        }

        private object CalculateDimension(IStiDimensionMeter dimension, object[] keys)
        {
            var dimensionIndex = GetDimensionIndex(dimension);
            if (dimensionIndex == -1)
                return null;

            var key = keys?[dimensionIndex];

            if (key is SimpleValue simpleValue)
                return simpleValue.Value;

            if (key is DateTimeValue dateTimeValue)
                return dateTimeValue.Value;

            if (key is StiFiscalMonth fiscalMonth)
                return fiscalMonth.Month;

            return key;
        }

        private object CalculateMeasureFunction(string function, string columnName, IEnumerable<object[]> rows)
        {
            var columnIndex = GetDataColumnIndex(columnName);
            if (columnIndex == -1)
                return null;

            var values = rows.Select(row => row[columnIndex]);
            return Funcs.Calculate(function, values);
        }

        private object CalculateMeasureExpression(IStiMeasureMeter measure, IEnumerable<object[]> rows)
        {
            if (string.IsNullOrWhiteSpace(measure.Expression))
                return null;

            var e = GetExpression(measure.Expression);

            this.currentRows = rows;

            return e.Evaluate();
        }

        private StiFunctionColumnPair GetMeasureColumn(IStiMeasureMeter measure)
        {
            if (Table == null)
                return null;

            if (string.IsNullOrWhiteSpace(measure.Expression))
                return null;

            if (expressionToPair.ContainsKey(measure.Expression))
                return expressionToPair[measure.Expression];

            var expression = measure.Expression.Trim().ToLowerInvariant();

            foreach (var function in Funcs.GetMeasureFunctions())
            {
                var lowerFunction = Funcs.ToLowerCase(function);
                
                var column = Table.Columns
                    .Cast<DataColumn>()
                    .FirstOrDefault(c =>
                    {
                        var lowerColumnName = Funcs.ToLowerCase(c.ColumnName).Trim();

                        return $"{lowerFunction}({lowerColumnName})" == expression || 
                               $"{lowerFunction}([{lowerColumnName}])" == expression;
                    })?
                    .ColumnName;
                
                if (column != null)
                {
                    var pair = new StiFunctionColumnPair { Function = function, ColumnName = column };
                    expressionToPair.Add(measure.Expression, pair);
                    return pair;
                }
            }

            return null;
        }

        private Expression GetExpression(string expression)
        {
            if (queryToExpression.ContainsKey(expression))
                return queryToExpression[expression];

            var expObject = StiExpressionHelper.NewExpression(expression);

            expObject.EvaluateFunction += (name, args) => args.Result = RunFunction(name, args);
            expObject.EvaluateParameter += delegate(string name, ParameterArgs args)
            {
                if (IsSystemVariable(name))
                    args.Result = GetSystemVariableValue(name);

                else if (IsVariable(name))
                    args.Result = GetVariableValue(name);

                else
                    args.Result = IsGrandTotal
                    ? grandRows?.SelectMany(rows => rows.Select(row => GetDataRowValue(name, row)))
                    : currentRows?.Select(row => GetDataRowValue(name, row));
            };

            queryToExpression[expression] = expObject;

            return expObject;
        }

        private object GetDataRowValue(string name, object[] row)
        {
            var columnIndex = GetDataColumnIndex(name);
            return columnIndex == -1 ? null : row[GetDataColumnIndex(name)];
        }
        #endregion

        #region Fields
        private IEnumerable<IGrouping<object[], object[]>> grandRows;
        private IEnumerable<object[]> currentRows;//Current set of row objects
        private Dictionary<string, Expression> queryToExpression = new Dictionary<string, Expression>();
        private Dictionary<string, StiFunctionColumnPair> expressionToPair = new Dictionary<string, StiFunctionColumnPair>();
        #endregion

        public StiMeasureDataParser(IStiAppDictionary dictionary, IStiMeter meter)
            : this(dictionary, null, new List<IStiMeter> { meter }, null)
        {
        }

        public StiMeasureDataParser(IStiAppDictionary dictionary, DataTable table, List<IStiMeter> meters, 
            IEnumerable<IGrouping<object[], object[]>> grandRows)
            : base(dictionary, table, meters)
        {
            this.grandRows = grandRows;
        }
    }
}