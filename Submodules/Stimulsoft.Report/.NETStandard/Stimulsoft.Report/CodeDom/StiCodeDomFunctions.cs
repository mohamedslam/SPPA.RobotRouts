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
using System.CodeDom;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;
using System.Collections.Generic;
 

namespace Stimulsoft.Report.CodeDom
{
	internal class StiCodeDomFunctions
	{
		/// <summary>
		///  Checks a text for the purpose of aggregate functions. If such are located in then generates a code for their processing.
		/// </summary>
		/// <param name="text">Text.</param>
		internal static string ParseFunctions(StiCodeDomSerializator serializator, string text)
		{
			return ParseFunctions(serializator, null, text);
		}

		/// <summary>
		///  Checks a text for the purpose of aggregate functions. If such are located in then generates a code for their processing.
		/// </summary>
		/// <param name="text">Text.</param>
		internal static string ParseFunctions(StiCodeDomSerializator serializator, string propertyName, string text)
		{
			string text2 = text;
			ParseFunctions(serializator, propertyName, null, ref text2, null, true);

			return text2;
		}

		/// <summary>
		///  Checks a text for the purpose of aggregate functions. If such are located in then generates a code for their processing.
		/// </summary>
		/// <param name="comp">The component in which this text is located.</param>
		/// <param name="text">Text.</param>
		/// <returns>true if it is required to delay a report rendering.</returns>
		internal static bool ParseFunctions(StiCodeDomSerializator serializator,  string propertyName,
			StiComponent comp, ref string text, StiExpression expression)
		{
			return ParseFunctions(serializator, propertyName, comp, ref text, expression, false);
		}

		/// <summary>
		///  Checks a text for the purpose of aggregate functions. If such are located in then generates a code for their processing.
		/// </summary>
		/// <param name="comp">The component in which this text is located.</param>
		/// <param name="text">Text.</param>
		/// <returns>true if it is required to delay a report rendering.</returns>
		internal static bool ParseFunctions(StiCodeDomSerializator serializator,  string propertyName,
			StiComponent comp, ref string text, StiExpression expression, bool isScript)
		{
            string prev = text;
            int functionIndex = 1;
            bool? assignedResult = null;
            do
            {
                prev = text;
                bool result = ParseFunctionsInternal(ref functionIndex, serializator, propertyName, comp, ref text, expression, isScript);
                if (assignedResult == null || result == false)
                {
                    assignedResult = result;
                }
            }
            while (prev != text);

            return assignedResult.GetValueOrDefault();
		}

        private static bool ParseFunctionsInternal(ref int functionIndex, StiCodeDomSerializator serializator, string propertyName, StiComponent comp, ref string text, StiExpression expression, bool isScript)
        {
            bool needStoreToPrinted = false;

            //check for EngineV2, and cSum must be not in SubReport, 
            if ((serializator.report.EngineVersion == StiEngineVersion.EngineV2) && (comp != null) && (StiSubReport.GetSubReportForPage(comp.Page) == null))
            {
                #region Replace cSum with Totals.cSum
                int textStartIndex = -1;
                while ((textStartIndex = text.IndexOf("cSum(", textStartIndex + 1, StringComparison.InvariantCulture)) > 0)
                {
                    if (textStartIndex > 0 && (Char.IsLetterOrDigit(text[textStartIndex - 1]) || text[textStartIndex - 1] == '_' || text[textStartIndex - 1] == '.')) continue;

                    #region Check for missing data parameter
                    try
                    {
                        bool refStoreToPrint = false;
                        List<StiParser.StiAsmCommand> listAsm = StiParser.ParseTextValue(text, comp, comp, ref refStoreToPrint, false, true) as List<StiParser.StiAsmCommand>;
                        if (listAsm != null)
                        {
                            foreach (var asmCommand in listAsm)
                            {
                                if ((asmCommand.Type == StiParser.StiAsmCommandType.PushComponent) && (asmCommand.Position == textStartIndex) && (asmCommand.Length == -1))
                                {
                                    StiComponent tempComp = asmCommand.Parameter1 as StiComponent;
                                    if ((tempComp != null) && !string.IsNullOrWhiteSpace(tempComp.Name))
                                    {
                                        text = text.Insert(textStartIndex + 5, tempComp.Name + ",");
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                    #endregion

                    text = text.Insert(textStartIndex, "Totals.");
                    textStartIndex += 7 + 5;
                    needStoreToPrinted = true;
                }
                textStartIndex = -1;
                while ((textStartIndex = text.IndexOf("cSumRunning(", textStartIndex + 1, StringComparison.InvariantCulture)) > 0)
                {
                    if (textStartIndex > 0 && (Char.IsLetterOrDigit(text[textStartIndex - 1]) || text[textStartIndex - 1] == '_' || text[textStartIndex - 1] == '.')) continue;

                    #region Check for missing data parameter
                    try
                    {
                        bool refStoreToPrint = false;
                        List<StiParser.StiAsmCommand> listAsm = StiParser.ParseTextValue(text, comp, comp, ref refStoreToPrint, false, true) as List<StiParser.StiAsmCommand>;
                        if (listAsm != null)
                        {
                            foreach (var asmCommand in listAsm)
                            {
                                if ((asmCommand.Type == StiParser.StiAsmCommandType.PushComponent) && (asmCommand.Position == textStartIndex) && (asmCommand.Length == -1))
                                {
                                    StiComponent tempComp = asmCommand.Parameter1 as StiComponent;
                                    if ((tempComp != null) && !string.IsNullOrWhiteSpace(tempComp.Name))
                                    {
                                        text = text.Insert(textStartIndex + 12, tempComp.Name + ",");
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                    #endregion

                    text = text.Insert(textStartIndex, "Totals.");
                    textStartIndex += 7 + 5;
                    needStoreToPrinted = true;
                }
                #endregion
            }

            var al = StiFunctionsParser.Parse(serializator.report, comp, ref text, isScript, expression, isScript);

            #region Processing functions
            if (al.Count > 0)
            {
                int paramIndex = 0;
                var listToAdd = new List<StiFunctionsParser.StiFunctionParam>();
                foreach (StiFunctionsParser.StiFunctionParam param in al)
                {
                    if (param.StartOfArgs < param.EndOfArgs)
                    {
                        string args = text.Substring(param.StartOfArgs, param.EndOfArgs - param.StartOfArgs + 1);
                        string oldArgs = args;
                        var list = StiFunctionsParser.Parse(serializator.report, comp, ref args, true, expression, isScript);
                        if (args != oldArgs)
                        {
                            text = text.Remove(param.StartOfArgs, param.EndOfArgs - param.StartOfArgs + 1);
                            text = text.Insert(param.StartOfArgs, args);

                            int distLength = args.Length - oldArgs.Length;

                            foreach (StiFunctionsParser.StiFunctionParam param2 in list)
                            {
                                param2.Index += distLength;
                            }

                            int startIndex = param.Index;
                            for (int indexCorr = paramIndex + 1; indexCorr < al.Count; indexCorr++)
                            {
                                var param2 = al[indexCorr];
                                if (param2.Index >= startIndex)
                                {
                                    param2.Index += distLength;
                                    param2.StartOfArgs += distLength;
                                    param2.EndOfArgs += distLength;
                                }
                            }
                        }

                        //неясно назначение следующего кода.
                        //когда-нибудь надо проверить, что этот код делает
                        foreach (StiFunctionsParser.StiFunctionParam param2 in list)
                        {
                            param2.Index += param.StartOfArgs;
                        }
                        listToAdd.AddRange(list);
                    }
                    paramIndex++;
                }
                al.AddRange(listToAdd);

                //bool aggregateFunctionsExist = false;
                bool aggregateFunctionsExist = needStoreToPrinted;

                int count = 1;
                int startLength = text.Length;
                paramIndex = 0;
                foreach (StiFunctionsParser.StiFunctionParam param in al)
                {
                    #region Aggregate Functions
                    if (param.AggregateFunction != null)
                    {
                        aggregateFunctionsExist = true;

                        string compName = param.ComponentName;
                        StiComponent aggregateComponent = null;
                        StiComponent groupHeaderBand = null;
                        if (comp != null) groupHeaderBand = comp.GetGroupHeaderBand();

                        #region If aggregateComponent is not specified, search it
                        if (string.IsNullOrEmpty(compName) && comp != null)
                        {
                            aggregateComponent = comp.GetGroupHeaderBand();

                            if (aggregateComponent == null) aggregateComponent = comp.GetDataBand();
                            if (aggregateComponent != null) compName = aggregateComponent.Name;
                        }
                        #endregion

                        string containerName = null;
                        if (param.IsContainerFunc)
                        {
                            if (!string.IsNullOrEmpty(compName) && aggregateComponent == null)
                            {
                                aggregateComponent = serializator.components[compName] as StiDataBand;
                            }
                            if (aggregateComponent != null)
                            {
                                if (serializator.report != null && serializator.report.EngineVersion == Stimulsoft.Report.Engine.StiEngineVersion.EngineV2)
                                {
                                    containerName = aggregateComponent.Page.Name;
                                }
                                else
                                {
                                    containerName = aggregateComponent.GetContainer().Name;
                                }
                            }
                        }
                        string funcName = null;
                        if (comp != null) funcName = comp.Name + '_' + param.FunctionName;
                        else funcName = compName + '_' + param.FunctionName;

                        if ((groupHeaderBand != null) && (groupHeaderBand.Name == param.ComponentName) && param.IsRunningTotal && param.FunctionName.StartsWith("Sum"))
                        {
                            funcName += "Running";
                        }

                        if (al.Count > 1) funcName += functionIndex.ToString();
                        functionIndex++;

                        if (expression is StiExcelValueExpression) funcName += "_ExcelValue";

                        #region Generate code ((double)this.band1.Aggregates[0]) for replace (Sum(123))
                        /*CodeCastExpression cast = new 
							CodeCastExpression(param.Function.GetResultType(),
							new CodeMethodInvokeExpression(
							new CodeFieldReferenceExpression(
							new CodeThisReferenceExpression(), funcName), "GetValue",
							new CodeExpression[0]));
							*/

                        var cast = new CodeCastExpression(param.AggregateFunction.GetResultType(),
                            new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression("StiReport"), "ChangeType",
                            new CodeExpression[]{
													new CodeMethodInvokeExpression(
													new CodeFieldReferenceExpression(
													new CodeThisReferenceExpression(), funcName), "GetValue",
													new CodeExpression[0]),
													new CodeTypeOfExpression(param.AggregateFunction.GetResultType()),
													new CodePrimitiveExpression(true)
												}));

                        string newText = StiCodeDomSerializator.ConvertExpressionToString(serializator.generator, cast);
                        #endregion

                        #region Change call aggregate function
                        int startIndex = param.Index;
                        int dist = text.Length;

                        text = text.Remove(param.Index, param.Length);
                        text = text.Insert(param.Index, newText);

                        dist = text.Length - dist;

                        for (int indexCorr = paramIndex + 1; indexCorr < al.Count; indexCorr++)
                        {
                            var param2 = al[indexCorr];
                            if (param2.Index >= startIndex)
                                param2.Index += dist;
                        }

                        #endregion

                        #region Declare aggregate function
                        serializator.AddDeclare(param.AggregateFunction.GetType(), funcName);

                        if (param.IsRunningTotal)
                        {
                            serializator.Statements.Add(new CodeCommentStatement(funcName));

                            CodeExpression left = new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(), funcName);

                            var assignStatement = new CodeAssignStatement(left, 
                                new CodeObjectCreateExpression(param.AggregateFunction.GetType(), 
                                new CodeExpression[]{ new CodePrimitiveExpression(true)}));

                            serializator.Statements.Add(assignStatement);


                        }
                        else 
                            serializator.AddCreate(count - 1, string.Empty,
                                param.AggregateFunction, funcName, param.AggregateFunction.GetType(), false);

                        #region Running total
                        string runningFooterBandName = null;
                        if (param.IsRunningTotal)
                        {
                            /*
                            CodeAssignStatement assign = new CodeAssignStatement(
                                new CodePropertyReferenceExpression(
                                new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(), funcName), "RunningTotal"),
                                new CodePrimitiveExpression(true));

                            serializator.Statements.Insert(1, assign);*/

                            StiContainer container = comp.Parent;
                            while (container != null && (!(container is StiBand)))
                            {
                                container = container.Parent;
                            }
                            //if (container is StiFooterBand && ((StiFooterBand)container).PrintOnAllPages)
                            if (container is StiFooterBand && ((StiFooterBand)container).PrintOnAllPages &&
                                !(container.Parent != null && container.Parent is StiPage))     //fix 2010.03.22
                                runningFooterBandName = container.Name;
                        }
                        #endregion

                        #endregion

                        #region Adds component to remitted collection
                        //if (!serializator.TestProcessAtEnd(comp, text))
                        serializator.TestProcessAtEnd(comp, text);
                        if (true)
                        {
                            param.Argument = ParseFunctions(serializator, param.Argument);
                            param.Condition = ParseFunctions(serializator, param.Condition);

                            #region Fix for null in TimeSpan field in SumTime function
                            if (StiOptions.Engine.AllowFixSumTimeArgumentConversion && (param.FunctionName == "SumTime") && !string.IsNullOrEmpty(param.Argument))
                            {
                                try
                                {
                                    int posDot = param.Argument.LastIndexOf('.');
                                    if (posDot != -1)
                                    {
                                        bool storeToPrint = false;
                                        object result = StiParser.ParseTextValue("{" + param.Argument + "}", comp, ref storeToPrint, false, true);
                                        var list = result as List<StiParser.StiAsmCommand>;
                                        if ((list != null) && (list.Count == 1) &&
                                            (list[0].Type == StiParser.StiAsmCommandType.PushDataSourceField ||
                                             list[0].Type == StiParser.StiAsmCommandType.PushBusinessObjectField))
                                        {
                                            string[] parts = param.Argument.Split(new char[] { '.' });
                                            bool founded = false;
                                            if (list[0].Type == StiParser.StiAsmCommandType.PushDataSourceField)
                                            {
                                                StiDataSource ds = comp.Report.Dictionary.DataSources[parts[parts.Length - 2]];
                                                if (ds != null)
                                                {
                                                    StiDataColumn dc = ds.Columns[parts[parts.Length - 1]];
                                                    if ((dc != null) && (dc.Type == typeof(TimeSpan)) && !(dc is StiCalcDataColumn))
                                                    {
                                                        founded = true;
                                                    }
                                                }
                                            }
                                            if (list[0].Type == StiParser.StiAsmCommandType.PushBusinessObjectField)
                                            {
                                                StiBusinessObject bo = comp.Report.Dictionary.BusinessObjects[parts[parts.Length - 2]];
                                                if (bo != null)
                                                {
                                                    StiDataColumn dc = bo.Columns[parts[parts.Length - 1]];
                                                    if ((dc != null) && (dc.Type == typeof(TimeSpan)))
                                                    {
                                                        founded = true;
                                                    }
                                                }
                                            }
                                            if (founded)
                                            {
                                                string brackets = 
                                                    serializator.report.ScriptLanguage == StiReportLanguageType.CSharp || 
                                                    serializator.report.ScriptLanguage == StiReportLanguageType.JS 
                                                    ? "[]" : "()";

                                                param.Argument = param.Argument.Substring(0, posDot) + brackets[0] + "\"" + param.Argument.Substring(posDot + 1) + "\"" + brackets[1];
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                            #endregion

                            #region Fix for null in DateTime fields in functions
                            if (StiOptions.Engine.AllowFixDateTimeArgumentConversion && !string.IsNullOrWhiteSpace(param.Argument) &&
                                (param.FunctionName == "AvgDate" || param.FunctionName == "AvgTime" || param.FunctionName == "MaxDate" || param.FunctionName == "MaxTime" || param.FunctionName == "MinDate" || 
                                param.FunctionName == "MinTime" || param.FunctionName == "First" || param.FunctionName == "Last" || param.FunctionName == "SumTime"))
                            {
                                try
                                {
                                    int posDot = param.Argument.LastIndexOf('.');
                                    if (posDot != -1)
                                    {
                                        bool storeToPrint = false;
                                        object result = StiParser.ParseTextValue("{" + param.Argument + "}", comp, ref storeToPrint, false, true);
                                        var list = result as List<StiParser.StiAsmCommand>;
                                        if ((list != null) && (list.Count == 1) &&
                                            (list[0].Type == StiParser.StiAsmCommandType.PushDataSourceField ||
                                             list[0].Type == StiParser.StiAsmCommandType.PushBusinessObjectField))
                                        {
                                            string[] parts = param.Argument.Split(new char[] { '.' });
                                            bool founded = false;
                                            if (list[0].Type == StiParser.StiAsmCommandType.PushDataSourceField)
                                            {
                                                StiDataSource ds = comp.Report.Dictionary.DataSources[parts[parts.Length - 2]];
                                                if (ds != null)
                                                {
                                                    StiDataColumn dc = ds.Columns[parts[parts.Length - 1]];
                                                    if ((dc != null) && !(dc is StiCalcDataColumn) && (dc.Type == typeof(DateTime) || dc.Type == typeof(TimeSpan)))
                                                    {
                                                        founded = true;
                                                    }
                                                }
                                            }
                                            if (list[0].Type == StiParser.StiAsmCommandType.PushBusinessObjectField)
                                            {
                                                StiBusinessObject bo = comp.Report.Dictionary.BusinessObjects[parts[parts.Length - 2]];
                                                if (bo != null)
                                                {
                                                    StiDataColumn dc = bo.Columns[parts[parts.Length - 1]];
                                                    if ((dc != null) && !(dc is StiCalcDataColumn) && (dc.Type == typeof(DateTime) || dc.Type == typeof(TimeSpan)))
                                                    {
                                                        founded = true;
                                                    }
                                                }
                                            }
                                            if (founded)
                                            {
                                                string brackets = 
                                                    serializator.report.ScriptLanguage == StiReportLanguageType.CSharp ||
                                                    serializator.report.ScriptLanguage == StiReportLanguageType.JS 
                                                    ? "[]" : "()";

                                                param.Argument = param.Argument.Substring(0, posDot) + brackets[0] + "\"" + param.Argument.Substring(posDot + 1) + "\"" + brackets[1];
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                            #endregion

                            serializator.AddToRemittedBuild(compName, comp, funcName, param.Argument, text,
                                param.Condition, param.AggregateFunction, param.Expression, param.IsRunningTotal);

                            if (groupHeaderBand != null && param.ComponentName != null)
                            {
                                compName = groupHeaderBand.Name;
                            }

                            if (containerName != null)
                            {
                                var remit = serializator.AddToRemittedSE(containerName, comp, funcName, param.Argument, text,
                                    param.AggregateFunction, true, param.IsColumnFunc, param.IsRunningTotal, param.Expression);

                                remit.RunningFooterBandName = runningFooterBandName;
                            }
                            else
                            {
                                var remit = serializator.AddToRemittedSE(compName, comp, funcName, param.Argument, text,
                                     param.AggregateFunction, false, param.IsColumnFunc, param.IsRunningTotal, param.Expression);
                                remit.RunningFooterBandName = runningFooterBandName;

                                //check if component placed on the masterBand, then use additional remit
                                var dataBand = comp.GetDataBand();
                                if (dataBand != null)
                                {
                                    string name = compName;
                                    if (compName.IndexOf(":", StringComparison.InvariantCulture) != -1)
                                    {
                                        string[] strs = compName.Split(new char[] { ':' });
                                        name = strs[0];
                                    }
                                    if ((dataBand.Name != name) && IsBandHasDetailWithName(dataBand, name))
                                    {
                                        var remit2 = serializator.AddToRemittedSE(dataBand.Name, comp, funcName, param.Argument, text,
                                             param.AggregateFunction, false, param.IsColumnFunc, param.IsRunningTotal, param.Expression);

                                        remit2.ConnectOnlyBegin = true;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region Function
                    else if (param.Function != null && param.Function.UseFullPath)
                    {
                        //int dist = text.Length - startLength;
                        string newText = string.Format("{0}.{1}",
                            param.Function.TypeOfFunction.ToString(), param.Function.FunctionName);

                        int startIndex = param.Index;
                        int dist = text.Length;

                        text = text.Remove(param.Index, param.Length);
                        text = text.Insert(param.Index, newText);

                        dist = text.Length - dist;

                        for (int indexCorr = paramIndex + 1; indexCorr < al.Count; indexCorr++)
                        {
                            var param2 = al[indexCorr];
                            if (param2.Index >= startIndex)
                                param2.Index += dist;
                        }

                    }
                    #endregion

                    paramIndex++;
                    count++;
                }

                if (aggregateFunctionsExist) return false;
                return !serializator.TestProcessAtEnd(comp, text);
            }
            #endregion

            else
            {
                if (propertyName == "Text" || propertyName == "ExcelValue") return !serializator.TestProcessAtEnd(comp, text);
                return false;
            }
        }

        private static bool IsBandHasDetailWithName(StiDataBand dataBand, string compName)
        {
            if (dataBand.Report.EngineVersion != StiEngineVersion.EngineV2) return false;

            var builder = new StiDataBandV2Builder();
            builder.FindDetailDataBands(dataBand);

            foreach (StiDataBand band in dataBand.DataBandInfoV2.DetailDataBands)
            {
                if (band.Name == compName) return true;
                var result = IsBandHasDetailWithName(band, compName);
                if (result) return true;
            }

            return false;
        }

	}
}
