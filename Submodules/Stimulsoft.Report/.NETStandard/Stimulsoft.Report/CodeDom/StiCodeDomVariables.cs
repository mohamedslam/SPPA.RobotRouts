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

using Stimulsoft.Base;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using System;
using System.CodeDom;
using System.Globalization;
using System.Threading;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.CodeDom
{
    internal class StiCodeDomVariables
    {
        internal static void Serialize(StiCodeDomSerializator serializator, StiReport report)
        {
            if (report.Dictionary.Variables.Count <= 0) return;

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                #region this.Dictionary.Variables.Add(variable)
                var expr = new CodeObjectCreateExpression(variable.GetType(),
                    serializator.GetArguments(variable.GetType(), variable));

                serializator.memberMethod.Statements.Add(new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        new CodePropertyReferenceExpression(
                            new CodeThisReferenceExpression(), "Dictionary"), "Variables"),
                    "Add", expr
                ));
                #endregion
            }

            SerializeVariablesToCode(serializator, report);
        }

        private static string ConvertArgumentFromString(StiCodeDomSerializator serializator, Type argumentType, string argumentStr)
        {
            #region string
            if (argumentType == typeof(string))
            {
                argumentStr = serializator.generator.QuoteSnippetString(argumentStr);

                if ((!argumentStr.StartsWith("\"", StringComparison.InvariantCulture) && (!argumentStr.StartsWith("@\"", StringComparison.InvariantCulture))))
                    argumentStr = "\"" + argumentStr;

                if (!argumentStr.EndsWith("\"", StringComparison.InvariantCulture))
                    argumentStr += "\"";

                return argumentStr;
            }
            #endregion

            #region Guid
            if (argumentType == typeof(Guid) || argumentType == typeof(Guid?))
            {
                return $"new Guid(\"{argumentStr}\")";
            }
            #endregion

            #region double
            if (argumentType == typeof(double) || argumentType == typeof(double?))
            {
                argumentStr = argumentStr.Replace(",", ".");
                if (!(argumentStr.EndsWith("d", StringComparison.InvariantCulture) || argumentStr.EndsWith("D", StringComparison.InvariantCulture)))
                    argumentStr += "d";

                return argumentStr;
            }
            #endregion

            #region float
            if (argumentType == typeof(float) || argumentType == typeof(float?))
            {
                argumentStr = argumentStr.Replace(",", ".");
                if (!(argumentStr.EndsWith("f", StringComparison.InvariantCulture) || argumentStr.EndsWith("F", StringComparison.InvariantCulture)))
                    argumentStr += "f";

                return argumentStr;
            }
            #endregion

            #region decimal
            if (argumentType == typeof(decimal) || argumentType == typeof(decimal?))
            {
                argumentStr = argumentStr.Replace(",", ".");
                if (serializator.report.ScriptLanguage == StiReportLanguageType.CSharp || serializator.report.ScriptLanguage == StiReportLanguageType.JS)
                {
                    if (!(argumentStr.EndsWith("m", StringComparison.InvariantCulture) || argumentStr.EndsWith("M", StringComparison.InvariantCulture)))
                        argumentStr += "m";
                }
                return argumentStr;
            }
            #endregion

            #region char
            if (argumentType == typeof(char) || argumentType == typeof(char?))
            {
                if (string.IsNullOrEmpty(argumentStr)) 
                    argumentStr = " ";

                if (serializator.report.ScriptLanguage == StiReportLanguageType.CSharp || serializator.report.ScriptLanguage == StiReportLanguageType.JS)
                {
                    if (!(argumentStr.StartsWith("'", StringComparison.InvariantCulture))) 
                        argumentStr = "'" + argumentStr;

                    if (!(argumentStr.EndsWith("'", StringComparison.InvariantCulture))) 
                        argumentStr += "'";

                    if (argumentStr.Length > 3)
                        argumentStr = argumentStr.Substring(0, 2) + "'";
                }
                else
                {
                    argumentStr = argumentStr.Length > 0
                        ? $"Microsoft.VisualBasic.ChrW({(int)argumentStr[0]})"
                        : "Microsoft.VisualBasic.ChrW(32)";
                }
                return argumentStr;
            }
            #endregion

            else
                return argumentStr;
        }

        private static void SerializeVariablesToCode(StiCodeDomSerializator serializator, StiReport report)
        {
            if (report.Dictionary.Variables.Count <= 0) return;

            var saveStatements = new CodeStatementCollection();
            var restoreStatements = new CodeStatementCollection();

            serializator.Statements.Add(new CodeCommentStatement(""));
            serializator.Statements.Add(new CodeCommentStatement("Variables init"));
            serializator.Statements.Add(new CodeCommentStatement(""));

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (string.IsNullOrEmpty(variable.Name)) continue;

                var variableValue = variable.Value != null ? variable.Value.Trim() : "";

                //fix - if variable has List type, but Value field is not empty
                if (typeof(IStiList).IsAssignableFrom(variable.Type)) 
                    variableValue = string.Empty;

                #region Process empty value
                if (variableValue.Length == 0)
                {
                    if (variable.Type == typeof(int) ||
                        variable.Type == typeof(uint) ||
                        variable.Type == typeof(long) ||
                        variable.Type == typeof(ulong) ||
                        variable.Type == typeof(byte) ||
                        variable.Type == typeof(sbyte) ||
                        variable.Type == typeof(short) ||
                        variable.Type == typeof(ushort))
                    {
                        variableValue = "0";
                    }
                    else if (variable.Type == typeof(string))
                    {
                        variableValue = "\"\"";
                    }
                    else if (variable.Type == typeof(bool))
                    {
                        variableValue = "false";
                    }
                    else if (variable.Type == typeof(char))
                    {
                        variableValue = 
                            report.ScriptLanguage == StiReportLanguageType.CSharp ||
                            report.ScriptLanguage == StiReportLanguageType.JS
                            ? "' '" : "Microsoft.VisualBasic.ChrW(32)";
                    }
                    else if (variable.Type == typeof(double))
                    {
                        variableValue = "0d";
                    }
                    else if (variable.Type == typeof(float))
                    {
                        variableValue = "0f";
                    }
                    else if (variable.Type == typeof(decimal))
                    {
                        variableValue = 
                            report.ScriptLanguage == StiReportLanguageType.CSharp ||
                            report.ScriptLanguage == StiReportLanguageType.JS
                            ? "0m" : "0";
                    }
                    else if (variable.Type == typeof(DateTime))
                    {
                        variableValue = "DateTime.Now";
                    }
                    else if (variable.Type == typeof(DateTimeOffset))
                    {
                        variableValue = "DateTimeOffset.Now";
                    }
                    else if (variable.Type == typeof(int?) ||
                             variable.Type == typeof(uint?) ||
                             variable.Type == typeof(long?) ||
                             variable.Type == typeof(ulong?) ||
                             variable.Type == typeof(byte?) ||
                             variable.Type == typeof(sbyte?) ||
                             variable.Type == typeof(short?) ||
                             variable.Type == typeof(ushort?))
                    {
                        variableValue = "0";
                    }
                    else if (variable.Type == typeof(bool?))
                    {
                        variableValue = "false";
                    }
                    else if (variable.Type == typeof(char?))
                    {
                        variableValue = 
                            report.ScriptLanguage == StiReportLanguageType.CSharp ||
                            report.ScriptLanguage == StiReportLanguageType.JS
                            ? "' '" : "Microsoft.VisualBasic.ChrW(32)";
                    }
                    else if (variable.Type == typeof(double?))
                    {
                        variableValue = "0d";
                    }
                    else if (variable.Type == typeof(float?))
                    {
                        variableValue = "0f";
                    }
                    else if (variable.Type == typeof(decimal?))
                    {
                        variableValue = 
                            report.ScriptLanguage == StiReportLanguageType.CSharp ||
                            report.ScriptLanguage == StiReportLanguageType.JS
                            ? "0m" : "0";
                    }
                    else if (variable.Type == typeof(DateTime?) || variable.Type == typeof(DateTimeOffset?))
                    {
                        variableValue = 
                            report.ScriptLanguage == StiReportLanguageType.CSharp ||
                            report.ScriptLanguage == StiReportLanguageType.JS
                            ? "null" : "Nothing";
                    }
                    else if (variable.Type == typeof(TimeSpan?))
                    {
                        variableValue = 
                            report.ScriptLanguage == StiReportLanguageType.CSharp ||
                            report.ScriptLanguage == StiReportLanguageType.JS
                            ? "null" : "Nothing";
                    }
                    else if (variable.Type == typeof(Guid) || variable.Type == typeof(Guid?))
                    {
                        variableValue = "new Guid()";
                    }
                    else if (StiTypeFinder.FindType(variable.Type, typeof(Range)))
                    {
                        var range = StiActivator.CreateObject(variable.Type) as Range;
                        variableValue = $"new {range.RangeName}()";
                    }
                    else
                    {
                        if (report.ScriptLanguage == StiReportLanguageType.CSharp || report.ScriptLanguage == StiReportLanguageType.JS)
                        {
                            if (StiTypeFinder.FindType(variable.Type, typeof(Array)))
                                variableValue = $"new {serializator.GetTypeString(variable.Type.GetElementType())}[0]";
                            else
                                variableValue = $"({serializator.GetTypeString(variable.Type)})Activator.CreateInstance(typeof({variable.Type}))";
                        }
                        else
                        {
                            if (StiTypeFinder.FindType(variable.Type, typeof(Array)))
                                variableValue = $"New {serializator.GetTypeString(variable.Type.GetElementType())}(0-0){{}}";
                            else
                                variableValue = $"CType(Activator.CreateInstance(GetType({serializator.GetTypeString(variable.Type)})), {variable.Type})";
                        }
                    }
                }
                #endregion

                #region Process variables with init values
                else
                {
                    #region InitBy == StiVariableInitBy.Value
                    if (variable.InitBy == StiVariableInitBy.Value)
                    {
                        var isDateTime = variable.Type == typeof(DateTime) || variable.Type == typeof(DateTime?);
                        var isDateTimeOffset = variable.Type == typeof(DateTimeOffset) || variable.Type == typeof(DateTimeOffset?);
                        var isTimeSpan = variable.Type == typeof(TimeSpan) || variable.Type == typeof(TimeSpan?);

                        #region string
                        if (variable.Type == typeof(string))
                        {
                            variableValue = serializator.generator.QuoteSnippetString(variableValue);

                            if ((!variableValue.StartsWith("\"", StringComparison.InvariantCulture) && (!variableValue.StartsWith("@\"", StringComparison.InvariantCulture))))
                                variableValue = "\"" + variableValue;

                            if (!variableValue.EndsWith("\"", StringComparison.InvariantCulture))
                                variableValue += "\"";

                        }
                        #endregion

                        #region Image
                        else if (StiTypeFinder.FindType(variable.Type, typeof(Image)))
                        {
                        }
                        #endregion

                        #region Guid
                        else if (variable.Type == typeof(Guid) || variable.Type == typeof(Guid?))
                        {
                            variableValue = $"new Guid(\"{variable.Value}\")";
                        }
                        #endregion

                        #region Range
                        else if (StiTypeFinder.FindType(variable.Type, typeof(Range)))
                        {
                            var range = variable.ValueObject as Range;
                            var dateTimeRange = range as DateTimeRange;
                            var timeSpanRange = range as TimeSpanRange;

                            #region DateTimeRange
                            if (dateTimeRange != null)
                            {
                                var currentCulture = Thread.CurrentThread.CurrentCulture;
                                try
                                {
                                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                                    string argFrom;
                                    string argTo;

                                    #region From
                                    if (dateTimeRange.From == null)
                                    {
                                        argFrom =
                                            serializator.report.ScriptLanguage == StiReportLanguageType.CSharp ||
                                            serializator.report.ScriptLanguage == StiReportLanguageType.JS
                                            ? "null" : "Nothing";
                                    }
                                    else
                                    {
                                        argFrom = $"ParseDateTime(\"{dateTimeRange.FromObject}\")";
                                    }
                                    #endregion

                                    #region To
                                    if (dateTimeRange.To == null)
                                    {
                                        argTo = 
                                            serializator.report.ScriptLanguage == StiReportLanguageType.CSharp ||
                                            serializator.report.ScriptLanguage == StiReportLanguageType.JS
                                            ? "null" : "Nothing";
                                    }
                                    else
                                    {
                                        argTo = $"ParseDateTime(\"{dateTimeRange.ToObject}\")";
                                    }
                                    #endregion

                                    variableValue = $"new {dateTimeRange.RangeName}({argFrom}, {argTo})";
                                }
                                finally
                                {
                                    Thread.CurrentThread.CurrentCulture = currentCulture;
                                }

                            }
                            #endregion

                            #region TimeSpanRange
                            else if (timeSpanRange != null)
                            {
                                var currentCulture = Thread.CurrentThread.CurrentCulture;
                                try
                                {
                                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                                    string argFrom;
                                    string argTo;

                                    #region From
                                    if (timeSpanRange.From == null)
                                    {
                                        argFrom = 
                                            serializator.report.ScriptLanguage == StiReportLanguageType.CSharp ||
                                            serializator.report.ScriptLanguage == StiReportLanguageType.JS
                                            ? "null" : "Nothing";
                                    }
                                    else
                                    {
                                        argFrom = $"ParseDateTime(\"{timeSpanRange.FromObject}\").TimeOfDay";
                                    }
                                    #endregion

                                    #region To
                                    if (timeSpanRange.To == null)
                                    {
                                        argTo = 
                                            serializator.report.ScriptLanguage == StiReportLanguageType.CSharp ||
                                            serializator.report.ScriptLanguage == StiReportLanguageType.JS
                                            ? "null" : "Nothing";
                                    }
                                    else
                                    {
                                        argTo = $"ParseDateTime(\"{timeSpanRange.ToObject}\").TimeOfDay";
                                    }
                                    #endregion

                                    variableValue = $"new {timeSpanRange.RangeName}({argFrom}, {argTo})";

                                }
                                finally
                                {
                                    Thread.CurrentThread.CurrentCulture = currentCulture;
                                }

                            }
                            #endregion

                            else
                            {
                                variableValue = string.Format("new {0}({1}, {2})", range.RangeName,
                                    ConvertArgumentFromString(serializator, range.FromObject.GetType(), range.FromObject.ToString()),
                                    ConvertArgumentFromString(serializator, range.ToObject.GetType(), range.ToObject.ToString()));
                            }
                        }
                        #endregion

                        #region isDateTime
                        else if (isDateTime)
                        {
                            if (!variableValue.StartsWithInvariant("\"") && char.IsDigit(variableValue[0]))
                                variableValue = $"ParseDateTime(\"{variable.GetNativeValue()}\")";
                        }
                        #endregion

                        #region isDateTimeOffset
                        else if (isDateTimeOffset)
                        {
                            if (!variableValue.StartsWithInvariant("\"") && char.IsDigit(variableValue[0]))
                                variableValue = $"ParseDateTimeOffset(\"{variable.GetNativeValue()}\")";
                        }
                        #endregion

                        #region isTimeSpan
                        else if (isTimeSpan)
                        {
                            if (!variableValue.StartsWithInvariant("\"") && char.IsDigit(variableValue[0]))
                                variableValue = $"ParseTimeSpan(\"{variable.GetNativeValue()}\")";
                        }
                        #endregion

                        #region double
                        else if (variable.Type == typeof(double) || variable.Type == typeof(double?))
                        {
                            variableValue = variableValue.Replace(",", ".");
                            if (!(variableValue.EndsWithInvariant("d") || variableValue.EndsWithInvariant("D")))
                                variableValue += "d";
                        }
                        #endregion

                        #region float
                        else if (variable.Type == typeof(float) || variable.Type == typeof(float?))
                        {
                            variableValue = variableValue.Replace(",", ".");
                            if (!(variableValue.EndsWithInvariant("f") || variableValue.EndsWithInvariant("F")))
                                variableValue += "f";
                        }
                        #endregion

                        #region decimal
                        else if (variable.Type == typeof(decimal) || variable.Type == typeof(decimal?))
                        {
                            variableValue = variableValue.Replace(",", ".");
                            if (report.ScriptLanguage == StiReportLanguageType.CSharp || report.ScriptLanguage == StiReportLanguageType.JS)
                            {
                                if (!(variableValue.EndsWithInvariant("m") || variableValue.EndsWithInvariant("M")))
                                    variableValue += "m";
                            }
                        }
                        #endregion

                        #region char
                        else if (variable.Type == typeof(char) || variable.Type == typeof(char?))
                        {
                            if (string.IsNullOrEmpty(variableValue)) 
                                variableValue = " ";

                            if (report.ScriptLanguage == StiReportLanguageType.CSharp || report.ScriptLanguage == StiReportLanguageType.JS)
                            {
                                if (!variableValue.StartsWithInvariant("'"))
                                    variableValue = "'" + variableValue;

                                if (!variableValue.EndsWithInvariant("'"))
                                    variableValue += "'";

                                if (variableValue.Length > 3)
                                    variableValue = variableValue.Substring(0, 2) + "'";
                            }
                            else
                            {
                                variableValue = variableValue.Length > 0 
                                    ? $"Microsoft.VisualBasic.ChrW({(int) variableValue[0]})" 
                                    : "Microsoft.VisualBasic.ChrW(32)";
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                if (!(variable.Type == typeof(Image) && variable.InitBy == StiVariableInitBy.Value))
                {
                    variableValue = StiCodeDomFunctions.ParseFunctions(serializator, variableValue);
                    variableValue = StiCodeDomTotalsFunctionsParser.ProcessTotals(serializator, variableValue, variable.Name);
                }

                CodeExpression expr;

                if (StiTypeFinder.FindType(variable.Type, typeof(Range)) && variable.InitBy == StiVariableInitBy.Expression)
                {
                    var variableValueFrom = StiCodeDomFunctions.ParseFunctions(serializator, variable.InitByExpressionFrom);
                    var variableValueTo = StiCodeDomFunctions.ParseFunctions(serializator, variable.InitByExpressionTo);

                    variableValueFrom = StiCodeDomTotalsFunctionsParser.ProcessTotals(serializator, variableValueFrom, variable.Name);
                    variableValueTo = StiCodeDomTotalsFunctionsParser.ProcessTotals(serializator, variableValueTo, variable.Name);

                    if (variable.Type == typeof(DateTimeRange) && variable.DialogInfo != null &&
                        variable.DialogInfo.DateTimeType == StiDateTimeType.Date && variable.InitBy == StiVariableInitBy.Expression)
                    {
                        variableValueFrom += ".Date";
                        variableValueTo += ".Date.AddDays(1).AddTicks(-1)";
                    }

                    var range = Activator.CreateInstance(variable.Type) as Range;
                    expr = new CodeSnippetExpression($"new {range.RangeName}({variableValueFrom}, {variableValueTo})");
                }
                else
                {
                    expr = new CodeSnippetExpression(variableValue);
                }

                if (StiTypeFinder.FindType(variable.Type, typeof(Image)))
                {
                    if (variable.ValueObject == null)
                        expr = new CodePrimitiveExpression(null);
                    else
                    {
                        expr =
                            new CodeCastExpression(serializator.GetTypeString(variable.Type),
                                new CodeMethodInvokeExpression(
                                    new CodeTypeReferenceExpression("StiVariableLoader"), "GetImage",
                                    new CodeThisReferenceExpression(),
                                    new CodePrimitiveExpression(variable.Name)));
                    }
                }

                #region Declare property
                if (variable.ReadOnly && !StiTypeFinder.FindType(variable.Type, typeof(Image)))
                {
                    var prop = new CodeMemberProperty
                    {
                        Attributes = MemberAttributes.Public,
                        Name = StiNameValidator.CorrectName(variable.Name, report),
                        HasGet = true,
                        Type = new CodeTypeReference(serializator.GetTypeString(variable.Type))
                    };
                    prop.GetStatements.Add(new CodeCommentStatement(StiCodeDomSerializator.GetCheckerInfoString(variable.Name, "Value")));
                    prop.GetStatements.Add(new CodeMethodReturnStatement(expr));
                    serializator.Members.Add(prop);
                }
                #endregion

                #region Declare field
                else
                {
                    #region field
                    var field = new CodeMemberField(serializator.GetTypeString(variable.Type), StiNameValidator.CorrectName(variable.Name, report))
                    {
                        Attributes = MemberAttributes.Public
                    };
                    if (Array.IndexOf(StiCodeDomSerializator.ReservedWords, variable.Name) != -1)
                        field.Attributes |= MemberAttributes.New;

                    serializator.Members.Add(field);
                    serializator.Statements.Add(new CodeCommentStatement(StiCodeDomSerializator.GetCheckerInfoString(variable.Name, "Value")));
                    serializator.Statements.Add(
                        new CodeAssignStatement(
                            new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(), StiNameValidator.CorrectName(variable.Name, report)), expr));
                    #endregion

                    if (!StiTypeFinder.FindType(variable.Type, typeof(Image)))
                    {
                        string pushMethodName = "";
                        if (variable.Type == typeof(int))
                            pushMethodName = "Int";

                        else if (variable.Type == typeof(long))
                            pushMethodName = "Int64";

                        else if (variable.Type == typeof(bool))
                            pushMethodName = "Bool";

                        else if (variable.Type == typeof(float))
                            pushMethodName = "Float";

                        else if (variable.Type == typeof(double))
                            pushMethodName = "Double";

                        else if (variable.Type == typeof(decimal))
                            pushMethodName = "Decimal";

                        #region saveStatements
                        var save = new CodeMethodInvokeExpression(
                            new CodePropertyReferenceExpression(
                                new CodeBaseReferenceExpression(), "States"), "Push" + pushMethodName, new CodeArgumentReferenceExpression("stateName"), new CodeThisReferenceExpression(), new CodePrimitiveExpression(variable.Name), new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), variable.Name));

                        saveStatements.Add(save);
                        #endregion

                        #region restoreStatements
                        CodeExpression popExpr = new CodeMethodInvokeExpression(
                            new CodeFieldReferenceExpression(
                                new CodeBaseReferenceExpression(), "States"), "Pop" + pushMethodName, new CodeArgumentReferenceExpression("stateName"), new CodeThisReferenceExpression(), new CodePrimitiveExpression(variable.Name));
                                
                        if (pushMethodName.Length == 0)
                            popExpr = new CodeCastExpression(serializator.GetTypeString(variable.Type), popExpr);

                        restoreStatements.Add(
                            new CodeAssignStatement(
                                new CodeFieldReferenceExpression(
                                    new CodeThisReferenceExpression(), variable.Name),
                                popExpr));
                        #endregion
                    }

                }
                #endregion
            }

            #region StateSave method
            CodeMemberMethod method;
            if (saveStatements.Count > 0 && report.EngineVersion == StiEngineVersion.EngineV1)
            {
                method = new CodeMemberMethod
                {
                    Attributes = MemberAttributes.Override | MemberAttributes.Public,
                    Name = "SaveState"
                };
                method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "stateName"));
                method.Statements.Add(new CodeMethodInvokeExpression(
                    new CodeBaseReferenceExpression(), "SaveState", new CodeArgumentReferenceExpression("stateName")));

                method.Statements.AddRange(saveStatements);
                serializator.Members.Add(method);
            }
            #endregion

            #region StateRestore method
            if (restoreStatements.Count > 0 && report.EngineVersion == Stimulsoft.Report.Engine.StiEngineVersion.EngineV1)
            {
                method = new CodeMemberMethod
                {
                    Attributes = MemberAttributes.Override | MemberAttributes.Public,
                    Name = "RestoreState"
                };
                method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "stateName"));
                method.Statements.Add(new CodeMethodInvokeExpression(
                    new CodeBaseReferenceExpression(), "RestoreState", new CodeArgumentReferenceExpression("stateName")));

                method.Statements.AddRange(restoreStatements);
                serializator.Members.Add(method);
            }
            #endregion
        }
    }
}
