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
using Stimulsoft.Data.Expressions.NCalc;
using Stimulsoft.Data.Expressions.NCalc.Domain;
using Stimulsoft.Data.Functions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Stimulsoft.Data.Helpers
{
    public class StiExpressionHelper
    {
        #region class ArgumentExtractionVisitor
        private class ArgumentExtractionVisitor : LogicalExpressionVisitor
        {
            public HashSet<string> Parameters = new HashSet<string>();

            public override void Visit(Identifier function)
            {
                Parameters.Add(function.Name);
            }

            public override void Visit(UnaryExpression expression)
            {
            }

            public override void Visit(BinaryExpression expression)
            {
                expression.LeftExpression.Accept(this);
                expression.RightExpression.Accept(this);
            }

            public override void Visit(TernaryExpression expression)
            {
                expression.LeftExpression.Accept(this);
                expression.RightExpression.Accept(this);
                expression.MiddleExpression.Accept(this);
            }

            public override void Visit(Function function)
            {
                foreach (var expression in function.Expressions)
                {
                    expression.Accept(this);
                }
            }

            public override void Visit(LogicalExpression expression)
            {
                expression.Accept(this);
            }

            public override void Visit(ValueExpression expression)
            {
            }
        }
        #endregion

        #region Fields
        private static Dictionary<string, List<string>> expressionToArguments = new Dictionary<string, List<string>>();
        #endregion

        #region Methods
        public static Expression NewExpression(string expression)
        {
            return new Expression(PrepareExpression(expression), EvaluateOptions.IgnoreCase);
        }

        public static string PrepareExpression(string expression)
        {
            expression = string.IsNullOrWhiteSpace(expression)
                ? expression
                : expression.Replace("\"", "\'");

            return EscapeExpression(expression);
        }

        public static string EscapeExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return expression;

            try
            {
                var tokens = new List<StiToken>();
                var lexer = new StiLexer(expression);
                while (true)
                {
                    var token = lexer.GetToken();
                    if (token == null || token.Type == StiTokenType.EOF) break;
                    tokens.Add(token);
                }

                var sb = new StringBuilder(expression);
                var offset = 0;

                for (var index = 0; index < tokens.Count - 2; index++)
                {
                    if (tokens[index].Type == StiTokenType.Ident &&
                        tokens[index + 1].Type == StiTokenType.Dot &&
                        tokens[index + 2].Type == StiTokenType.Ident)
                    {
                        if (index != 0 && index + 3 < tokens.Count && (tokens[index - 1].Type == StiTokenType.LBracket || tokens[index + 3].Type == StiTokenType.RBracket))
                            continue;

                        var pos1 = tokens[index].Index;
                        var pos2 = tokens[index + 2].Index + tokens[index + 2].Length;

                        sb.Insert(pos1 + offset, "[");
                        offset++;

                        sb.Insert(pos2 + offset, "]");
                        offset++;

                        index++;
                    }
                }

                return sb.ToString();
            }
            catch
            {
            }

            return expression;
        }

        public static string ReplaceFunction(string expression, string newFunction)
        {
            var currentFunction = GetFunction(expression);

            if (currentFunction != null)
                expression = expression.Substring(currentFunction.Length);

            else
            {
                expression = expression.Trim();

                if (!expression.StartsWith("("))
                    expression = $"({expression}";

                if (!expression.EndsWith(")"))
                    expression = $"{expression})";
            }

            return $"{newFunction}{expression}";
        }

        public static string RemoveFunction(string expression)
        {
            if (expression == null)
                return expression;

            var currentFunction = GetFunction(expression);
            if (currentFunction != null)
                expression = expression.Substring(currentFunction.Length);

            expression = expression.Trim();

            if (expression.StartsWith("("))
                expression = expression.Substring(1);

            if (expression.EndsWith(")"))
                expression = expression.Substring(0, expression.Length - 1);

            return expression;
        }

        public static bool IsPercentOfGrandTotal(string expression)
        {
            var function = GetFunction(expression);
            if (string.IsNullOrWhiteSpace(function))
                return false;

            return function.ToLowerInvariant().Trim() == "percentofgrandtotal";
        }

        public static bool IsAggregationFunctionPresent(string expression)
        {
            var function = GetFunction(expression);
            if (string.IsNullOrWhiteSpace(function))
                return false;

            return Funcs.IsAggregationFunction(function);
        }

        public static bool IsFunctionPresent(string expression)
        {
            var function = GetFunction(expression);
            return !string.IsNullOrWhiteSpace(function);
        }

        public static string GetFunction(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression)) return null;

            try
            {
                var parsedExpression = Expression.Compile(PrepareExpression(expression), true);
                var parsedFunction = parsedExpression as Function;
                var functionName = parsedFunction?.Identifier?.Name;

                return functionName != null && expression.Trim().StartsWith(functionName) ? functionName : null;
            }
            catch
            {
                return null;
            }
        }

        public static List<string> GetArguments(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return new List<string>();

            try
            {
                lock (expressionToArguments)
                {
                    if (expressionToArguments.ContainsKey(expression))
                        return expressionToArguments[expression];
                }

                var parsedExpression = Compile(expression);

                var visitor = new ArgumentExtractionVisitor();
                parsedExpression.Accept(visitor);

                var list = visitor.Parameters.ToList();

                lock (expressionToArguments)
                {
                    expressionToArguments[expression] = list;
                }

                return list;
            }
            catch
            {
            }

            return new List<string>();
        }

        public static LogicalExpression Compile(string expression)
        {
            return Expression.Compile(PrepareExpression(expression), true);
        }

        public static string GetFirstArgumentFromExpression(string expression)
        {
            expression = RemoveFunction(expression);
            var args = GetArguments(expression);
            if (args == null) return null;

            var arg = args.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(arg)) return null;

            return arg;
        }

        public static string ParseReportExpression(IStiReport report, string text, bool withBraces, bool allowReturnNull = false)
        {
            if (report != null && !string.IsNullOrEmpty(text))
            {
                if (!withBraces && !text.Contains("{") && !text.Contains("}"))
                    text = "{" + text + "}";

                var currentCulture = Thread.CurrentThread.CurrentCulture;
                var result = string.Empty;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    result = report.FetchPages()?.FirstOrDefault()?.ParseExpression(text, allowReturnNull);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
                
                return result;
            }

            return text;
        }

        public static List<string> FetchBlocksFromExpression(string inputExpression)
        {
            if (string.IsNullOrWhiteSpace(inputExpression))
                return null;

            var list = new List<string>();

            int pos = 0;
            while (pos < inputExpression.Length)
            {
                while (pos < inputExpression.Length && inputExpression[pos] != '{')
                {
                    pos++;
                }

                if (pos < inputExpression.Length && inputExpression[pos] == '{')
                {
                    pos++;
                    int posBegin = pos;
                    while (pos < inputExpression.Length)
                    {
                        if (inputExpression[pos] == '"')
                        {
                            pos++;
                            int pos2 = pos;
                            while (pos2 < inputExpression.Length)
                            {
                                if (inputExpression[pos2] == '"') break;
                                if (inputExpression[pos2] == '\\')
                                    pos2++;
                                
                                pos2++;
                            }
                            pos = pos2 + 1;
                            continue;
                        }

                        if (inputExpression[pos] == '}')
                        {
                            var currentExpression = inputExpression.Substring(posBegin, pos - posBegin);
                            if (!string.IsNullOrWhiteSpace(currentExpression))
                                list.Add(currentExpression.Trim());

                            pos++;
                            break;
                        }
                        pos++;
                    }
                }
            }

            return list;
        }

        public static string ReplaceExpressionBlocksByValues(string inputExpression, List<string> values)
        {
            if (string.IsNullOrWhiteSpace(inputExpression)) 
                return null;

            var result = new StringBuilder();
            var index = 0;
            var pos = 0;

            while (pos < inputExpression.Length)
            {
                var posBegin = pos;
                while (pos < inputExpression.Length && inputExpression[pos] != '{')
                {
                    pos++;
                }
                
                if (pos != posBegin)
                    result.Append(inputExpression.Substring(posBegin, pos - posBegin));

                var flag = false;
                if (pos < inputExpression.Length && inputExpression[pos] == '{')
                {
                    pos++;
                    posBegin = pos;
                    while (pos < inputExpression.Length)
                    {
                        if (inputExpression[pos] == '"')
                        {
                            pos++;
                            int pos2 = pos;
                            while (pos2 < inputExpression.Length)
                            {
                                if (inputExpression[pos2] == '"') break;                                
                                if (inputExpression[pos2] == '\\') 
                                    pos2++;

                                pos2++;
                            }
                            pos = pos2 + 1;
                            continue;
                        }

                        if (inputExpression[pos] == '}')
                        {
                            var currentExpression = inputExpression.Substring(posBegin, pos - posBegin);
                            if (!string.IsNullOrWhiteSpace(currentExpression))
                                result.Append(values[index++]);
                            
                            flag = true;
                            pos++;
                            break;
                        }
                        pos++;
                    }

                    if (!flag)                    
                        result.Append(inputExpression.Substring(posBegin - 1));                    
                }
            }

            return result.ToString();
        }

        public static bool IsTimeExpression(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) 
                return false;

            var blocks = FetchBlocksFromExpression(str);
            if (blocks == null || blocks.Count == 0)
                return false;

            return blocks.All(b => b.ToLowerInvariant().Trim() == "time");
        }
        #endregion
    }
}