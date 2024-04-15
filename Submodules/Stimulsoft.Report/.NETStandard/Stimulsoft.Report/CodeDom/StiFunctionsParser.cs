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
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.CodeDom
{
	/// <summary>
	/// The class parses scripts which contain functions.
	/// </summary>
	internal class StiFunctionsParser
	{
        #region class StiFunctionParam
        /// <summary>
        /// Function parameters.
        /// </summary>
        internal class StiFunctionParam
		{
			/// <summary>
			/// Gets or sets a name of the component with which an function works.
			/// </summary>
			public string ComponentName;

			/// <summary>
			///  Gets or sets an function argument.
			/// </summary>
			public string Argument;

			/// <summary>
			/// Gets or sets an function condition.
			/// </summary>
			public string Condition;

			/// <summary>
			/// Gets or sets an function name.
			/// </summary>
			public string FunctionName;

			public StiFunction Function = null;

			/// <summary>
			/// Gets or sets an aggregate function service.
			/// </summary>
			public StiAggregateFunctionService AggregateFunction;

			/// <summary>
			/// Gets or sets the initial position of an function script text.
			/// </summary>
			public int Index;

			/// <summary>
			/// Gets or sets the length of an function script text.
			/// </summary>
			public int Length;

			/// <summary>
			/// Gets or sets the initial position of an function arguments text.
			/// </summary>
			public int StartOfArgs;

			/// <summary>
			/// Gets or sets the finish position of an function function text.
			/// </summary>
			public int EndOfArgs;

			/// <summary>
			/// Gets or sets value indicates that the aggregate function works with the container.
			/// </summary>
			public bool IsContainerFunc;

			/// <summary>
			/// Gets or sets value indicates that the aggregate function works with the column.
			/// </summary>
			public bool IsColumnFunc;

            /// <summary>
            /// Gets or sets value indicates that the aggregate function works in running total mode.
            /// </summary>
            public bool IsRunningTotal;

			public StiExpression Expression = null;

			/// <summary>
			/// Function parameters.
			/// </summary>
			/// <param name="componentName">Name of the component with which an aggregate function works.</param>
			/// <param name="argument">Function argument.</param>
			/// <param name="condition">Function condition.</param>
			/// <param name="functionName">Function name.</param>
			/// <param name="function">Aggregate function service.</param>
			/// <param name="index">Initial position of an function in the text of a script.</param>
			/// <param name="length">Length of an function text.</param>
			/// <param name="isContainerFunc">If true, then the function works with the container.</param>
			/// <param name="isColumnFunc">If true, then the function works with the column.</param>
            /// <param name="isColumnFunc">If true, then the function works in running total mode.</param>
			public StiFunctionParam(
				string componentName, 
				string argument,
				string condition,
				string functionName, 
				StiAggregateFunctionService function,
				int index, int length,
				bool isContainerFunc,
				bool isColumnFunc,
                bool isRunningTotal,
				StiExpression expression)
			{
				this.ComponentName = componentName;
				this.Argument = argument;
				this.Condition = condition;
				this.FunctionName = functionName;
				this.AggregateFunction = function;
				this.Index = index;
				this.Length = length;
                this.IsContainerFunc = isContainerFunc;
				this.IsColumnFunc = isColumnFunc;
                this.IsRunningTotal = isRunningTotal;
				this.Expression = expression;
			}
		}
		#endregion

		#region Methods
		private static string GetParsContent(StiLexer lexer)
		{
			var sb = new StringBuilder("(");
			var token = lexer.GetToken();
            var lastType = StiTokenType.None;
            while (token.Type != StiTokenType.EOF && token.Type != StiTokenType.RPar)
			{
                if (token.Type == StiTokenType.LPar)
                {
                    sb = sb.Append(GetParsContent(lexer));
                }
                else
                {
                    if (CheckForNeedWhiteSpace(lastType, token)) sb.Append(' ');
                    sb = sb.Append(lexer.Text.ToString().Substring(token.Index, token.Length));
                }
                lastType = token.Type;
                token = lexer.GetToken();
			}
			sb = sb.Append(")");
			return sb.ToString();			
		}


		/// <summary>
		/// Parses a string expression on parameters and names of the component.
		/// </summary>
		/// <param name="lexer">Scanner.</param>
		/// <returns>Collection of parameters. The first is the name of the component. The second are parameters.</returns>
        private static List<string> GetArguments(StiLexer lexer, ref int startOfArgs, ref int endOfArgs)
		{			
			var token = lexer.GetToken();
			startOfArgs = token.Index;

			var arguments = new List<string>();
			var sb = new StringBuilder();

            var lastType = StiTokenType.None;
            while (token.Type != StiTokenType.EOF && token.Type != StiTokenType.RPar)
			{
				if (token.Type == StiTokenType.Comma)
				{
					arguments.Add(sb.ToString());
					sb = new StringBuilder();
				}
                else if (token.Type == StiTokenType.LPar)
                {
                    sb = sb.Append(GetParsContent(lexer));
                }
                else
                {
                    if (CheckForNeedWhiteSpace(lastType, token)) sb.Append(' ');
                    sb = sb.Append(lexer.Text.ToString().Substring(token.Index, token.Length));
                }
                lastType = token.Type;
				token = lexer.GetToken();
			}
			if (sb.Length > 0)arguments.Add(sb.ToString());
			endOfArgs = token.Index - 1;

			return arguments;
		}

        private static bool CheckForNeedWhiteSpace(StiTokenType lastType, StiToken token)
        {
            if (token.Type == StiTokenType.Ident)
            {
                if (lastType == StiTokenType.Ident || lastType == StiTokenType.Unknown) return true;
                if (lastType == StiTokenType.Value)
                {
                    string st = token.Data as string;
                    if (!string.IsNullOrWhiteSpace(st)) st = st.ToLowerInvariant();
                    if (st == "f" || st == "d" || st == "m" || st == "u" || st == "l" || st == "ul") return false;
                    return true;
                }
            }
            if (lastType == StiTokenType.Ident && (token.Type == StiTokenType.Value || token.Type == StiTokenType.Unknown)) return true;
            return false;
        }

        public static List<StiFunctionParam> Parse(StiReport report, StiComponent ownerComponent, ref string text, StiExpression expression, bool checkTotals)
		{
            return Parse(report, ownerComponent, ref text, false, expression, checkTotals);
		}


		/// <summary>
		///  Returns a collection of aggregate functions parameters which are present in the script text.
		/// </summary>
        public static List<StiFunctionParam> Parse(StiReport report, StiComponent ownerComponent, ref string text, bool isScript, StiExpression expression, bool checkTotals)
		{
            var par = new List<StiFunctionParam>();

            if ((!isScript) && (text.IndexOf('{') == -1 || text.IndexOf('}') == -1))
                return par;


            var lexer = new StiLexer(text) { IsVB = report.ScriptLanguage == StiReportLanguageType.VB };
			var token = new StiToken(StiTokenType.EOF);
            var index = 0;
			var length = 0;

			try
			{
                if (!isScript && lexer.PositionInText < lexer.Text.Length && lexer.Text[lexer.PositionInText] == '"')
                {
                    token = new StiToken(StiTokenType.Value, lexer.PositionInText, 1);
                    lexer.PositionInText++;
                    token.Data = '"';
                }
                else
                {
                    token = lexer.GetToken();
                }
			}
			catch
			{
			}

			
			bool aggrIdent = isScript;

			while (token.Type != StiTokenType.EOF)
			{
				if (!aggrIdent)
				{
					if (token.Type == StiTokenType.LBrace)
					{
						if (!isScript)aggrIdent = true;
					}
				}				
				else if (token.Type == StiTokenType.RBrace)
				{
					if (!isScript)aggrIdent = false;
				}
				else
				{
					#region Ident
					if (token.Type == StiTokenType.Ident)
					{
						bool isContainerFunc = false;
						bool isConditionFunc = false;
						bool isColumnFunc = false;
                        bool isRunningTotal = false;

						var funcName = (string)token.Data;

                        #region Variable
                        var variable = report.Dictionary.Variables[funcName];
                        if (variable != null)
                        {
                            lexer.SavePosToken();
                            lexer.SavePosToken();
                            var token1 = lexer.GetToken();
                            var token2 = lexer.GetToken();
                            lexer.UngetToken();
                            lexer.UngetToken();

                            if (token1.Type == StiTokenType.Dot && token2.Type == StiTokenType.Ident && (token2.Data as string) == "Label")
                            {
                                var variableName = token.Data as string;
                                var newFuncName = $"GetLabel(\"{variableName}\")";
                                
                                text = text.Remove(token.Index, token.Length + token1.Length + token2.Length);
                                text = text.Insert(token.Index, newFuncName);
                                lexer.PositionInText += newFuncName.Length - token.Length - token1.Length - token2.Length;
                                token.Length = newFuncName.Length;
                                lexer.Text = new StringBuilder(text);
                            }
                        }
                        #endregion

                        #region Check If Condition
                        if (funcName.EndsWith("If", StringComparison.InvariantCulture))
						{
							funcName = funcName.Substring(0, funcName.Length - 2);
							isConditionFunc = true;

                            #region Check Running Total
                            if (funcName.EndsWith("Running", StringComparison.InvariantCulture))
                            {
                                funcName = funcName.Substring(0, funcName.Length - 7);
                                isRunningTotal = true;
                            }
                            #endregion
						}
                        #endregion

                        #region Check Running Total
                        if (funcName.EndsWith("Running", StringComparison.InvariantCulture))
                        {
                            funcName = funcName.Substring(0, funcName.Length - 7);
                            isRunningTotal = true;

                            #region Check If Condition
                            if (funcName.EndsWith("If", StringComparison.InvariantCulture))
                            {
                                funcName = funcName.Substring(0, funcName.Length - 2);
                                isConditionFunc = true;
                            }
                            #endregion
                        }
                        #endregion

                        #region Check Column Mode
                        if (funcName.StartsWith("col", StringComparison.InvariantCulture))
						{
							isColumnFunc = true;
							funcName = funcName.Substring(3);
                        }
                        #endregion

                        #region Check Container Mode
                        else if (funcName.StartsWith("c", StringComparison.InvariantCulture))
						{
							isContainerFunc = true;
							funcName = funcName.Substring(1);
                        }
                        #endregion

                        #region Check total placement
                        /*Check total placement, if total placed on HeaderBand or FooterBand and this bands have PrintOnAllPages property = true
                         * and DataBand of this bands is master DataBand (not slave) and this is running total 
                         * then automatically convert this total to container total. */
                        if ((!isContainerFunc) && ownerComponent != null && isRunningTotal)
                        {
                            StiComponent parent = ownerComponent.Parent;
                            while (!(parent == null || parent is StiPage || parent is StiHeaderBand || parent is StiFooterBand))
                            {
                                parent = parent.Parent;
                            }
                            if (parent is StiHeaderBand || parent is StiFooterBand)
                            {   
                                StiDataBand dataBand = null;

                                #region Check HeaderBand
                                StiHeaderBand headerBand = parent as StiHeaderBand;
                                if (headerBand != null)
                                {
                                    if (report.EngineVersion == Stimulsoft.Report.Engine.StiEngineVersion.EngineV1)
                                        dataBand = StiHeaderBandV1Builder.GetMaster(headerBand) as StiDataBand;
                                    else
                                        dataBand = StiHeaderBandV2Builder.GetMaster(headerBand) as StiDataBand;

                                    #region If header band is not PrintOnAllPages then this total can't be RunningTotal
                                    if (dataBand != null && (!headerBand.PrintOnAllPages))
                                    {
                                        dataBand = null;
                                        isRunningTotal = false;
                                    }
                                    #endregion
                                }
                                #endregion

                                #region Check FooterBand
                                StiFooterBand footerBand = parent as StiFooterBand;
                                if (footerBand != null)
                                {
                                    if (report.EngineVersion == Stimulsoft.Report.Engine.StiEngineVersion.EngineV1)
                                        dataBand = StiFooterBandV1Builder.GetMaster(footerBand) as StiDataBand;
                                    else
                                        dataBand = StiFooterBandV2Builder.GetMaster(footerBand) as StiDataBand;

                                    #region If footer band is not PrintOnAllPages then this total can't be RunningTotal
                                    if (dataBand != null && (!footerBand.PrintOnAllPages))
                                    {
                                        dataBand = null;
                                        isRunningTotal = false;
                                    }
                                    #endregion
                                }
                                #endregion

                                if (dataBand != null && dataBand.MasterComponent == null)
                                {
                                    isContainerFunc = true;
                                }
                            }
                            if (ownerComponent.Parent is StiPageHeaderBand || ownerComponent.Parent is StiPageFooterBand)   //fix 2010.03.22
                            {
                                isContainerFunc = true;
                            }
                        }
                        #endregion

                        #region Parse Functions
                        var func = StiAggregateFunctionService.GetFunction(StiOptions.Services.AggregateFunctions.Where(s => s.ServiceEnabled).ToList(), funcName);					
						try
						{
							#region Aggregate functions
                            if (func != null)
                            {
                                #region Try to find symbol "(" after function name. If we don't find it then this function is not total function
                                int index3 = token.Index + token.Length;
                                bool finded = false;
                                while (index3 < text.Length)
                                {
                                    if (char.IsWhiteSpace(text[index3]))
                                    {
                                        index3++;
                                        continue;
                                    }
                                    if (text[index3] == '(')
                                    {
                                        finded = true;
                                        break;
                                    }
                                    break;
                                }

								//If we find symbol "(" after function name then try to find symbol ")" after symbol ")"
								if (finded && isScript && checkTotals)
								{
									int endIndex = index3 + 1;
									while (endIndex < text.Length && char.IsWhiteSpace(text, endIndex))
									{
										endIndex++;
									}
									//We have out from text length. This is not total function!
									if (endIndex >= text.Length)finded = false;

									//Argument is absent, in script all total functions must have argument! This is not total function! 
									if (text[endIndex] == ')')//Arguments is absent
									{
										finded = false;
									}

									//Try to find "." before function name. If we find it then this is not total function!
									if (finded)
									{
										endIndex = token.Index - 1;
										while (endIndex >= 0 && char.IsWhiteSpace(text, endIndex))
										{
											endIndex--;
										}
										//We find symbol '.' before function name! This is not total function
										if (endIndex >= 0 && text[endIndex] == '.')finded = false;
									}
								}
                                #endregion

                                if (finded)
                                {
                                    //if ((isScript && checkTotals) || (isContainerFunc && token.Index > 0 && lexer.Text[token.Index - 1] != '.'))
                                    if (isScript && checkTotals)    //снова убрал условие, т.к. не работало cSumIf
                                    {
                                        string newFuncName;

                                        if (isContainerFunc)
                                            newFuncName = string.Format("Totals.c{0}", funcName);
                                        else
                                            newFuncName = string.Format("Totals.{0}", funcName);

                                        if (isRunningTotal)
                                            newFuncName += "Running";

                                        text = text.Remove(token.Index, token.Length);
                                        text = text.Insert(token.Index, newFuncName);
                                        lexer.PositionInText += newFuncName.Length - token.Length;
                                        token.Length = newFuncName.Length;
                                        lexer.Text = new StringBuilder(text);

                                        #region Check name of databand for running total
                                        int startOfArgs = 0;
                                        int endOfArgs = 0;

                                        StiLexer lexer2 = new StiLexer(lexer.Text.ToString());
                                        lexer2.PositionInText = lexer.PositionInText;
                                        var arguments = GetArguments(lexer2, ref startOfArgs, ref endOfArgs);
                                        string dataBandName = arguments[0];
                                        if (dataBandName.Contains(",")) dataBandName = dataBandName.Substring(0, dataBandName.IndexOf(','));
                                        dataBandName = dataBandName.Replace("(", "").Replace(")", "").Replace("{", "").Replace("}", "");
                                        if (report.DataBandsUsedInPageTotals == null)
                                        {
                                            report.DataBandsUsedInPageTotals = new string[1];
                                            report.DataBandsUsedInPageTotals[0] = dataBandName;
                                        }
                                        else
                                        {
                                            bool finded3 = false;
                                            foreach (string str in report.DataBandsUsedInPageTotals)
                                            {
                                                if (str == dataBandName)
                                                {
                                                    finded3 = true;
                                                    break;
                                                }
                                            }
                                            if (!finded3)
                                            {
                                                string[] strs = new string[report.DataBandsUsedInPageTotals.Length + 1];
                                                for (int indexStr = 0; indexStr < report.DataBandsUsedInPageTotals.Length; indexStr++)
                                                {
                                                    strs[indexStr] = report.DataBandsUsedInPageTotals[indexStr];
                                                }
                                                strs[report.DataBandsUsedInPageTotals.Length] = dataBandName;
                                                report.DataBandsUsedInPageTotals = strs;
                                            }
                                        }
                                        #endregion 
                                    }
                                    else
                                    {
                                        if (isContainerFunc)
                                        {
                                            #region Check name of databand for running total
                                            int startOfArgs = 0;
                                            int endOfArgs = 0;

                                            StiLexer lexer2 = new StiLexer(lexer.Text.ToString());
                                            lexer2.PositionInText = lexer.PositionInText + 1;   // +1 for '('
                                            var arguments = GetArguments(lexer2, ref startOfArgs, ref endOfArgs);
                                            
                                            var dataBandName = arguments.FirstOrDefault();
                                            
                                            if (dataBandName == null)
                                                dataBandName = string.Empty;

                                            if (dataBandName.Contains(",")) 
                                                dataBandName = dataBandName.Substring(0, dataBandName.IndexOf(','));

                                            dataBandName = dataBandName.Replace("(", "").Replace(")", "").Replace("{", "").Replace("}", "");
                                            if (report.DataBandsUsedInPageTotals == null)
                                            {
                                                report.DataBandsUsedInPageTotals = new string[1];
                                                report.DataBandsUsedInPageTotals[0] = dataBandName;
                                            }
                                            else
                                            {
                                                bool finded3 = false;
                                                foreach (string str in report.DataBandsUsedInPageTotals)
                                                {
                                                    if (str == dataBandName)
                                                    {
                                                        finded3 = true;
                                                        break;
                                                    }
                                                }
                                                if (!finded3)
                                                {
                                                    string[] strs = new string[report.DataBandsUsedInPageTotals.Length + 1];
                                                    for (int indexStr = 0; indexStr < report.DataBandsUsedInPageTotals.Length; indexStr++)
                                                    {
                                                        strs[indexStr] = report.DataBandsUsedInPageTotals[indexStr];
                                                    }
                                                    strs[report.DataBandsUsedInPageTotals.Length] = dataBandName;
                                                    report.DataBandsUsedInPageTotals = strs;
                                                }
                                            }
                                            #endregion
                                        }

                                        #region Standard totals processing
                                        index = token.Index;
                                        if (lexer.WaitLparen2() &&
                                            ((index > 0 && text[index - 1] != '.') ||
                                            index == 0))
                                        {
                                            int startOfArgs = 0;
                                            int endOfArgs = 0;

                                            var arguments = GetArguments(lexer, ref startOfArgs, ref endOfArgs);
                                            length = lexer.PositionInText - index;

                                            string componentName = string.Empty;
                                            string argument = string.Empty;
                                            string condition = string.Empty;

                                            if (arguments.Count > 0)
                                            {
                                                #region isConditionFunc
                                                if (isConditionFunc)
                                                {
                                                    if (func.RecureParam)
                                                    {
                                                        if (arguments.Count == 3)
                                                        {
                                                            componentName = arguments[0];
                                                            argument = arguments[1];
                                                            condition = arguments[2];
                                                        }
                                                        if (arguments.Count == 2)
                                                        {
                                                            argument = arguments[0];
                                                            condition = arguments[1];
                                                        }
                                                        else if (arguments.Count == 1)
                                                        {
                                                            argument = arguments[0];
                                                            condition = "true";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (arguments.Count == 2)
                                                        {
                                                            componentName = arguments[0];
                                                            condition = arguments[1];
                                                        }
                                                        else if (arguments.Count == 1)
                                                        {
                                                            condition = arguments[0];
                                                        }
                                                    }
                                                }
                                                #endregion

                                                #region is not isConditionFunc
                                                else
                                                {
                                                    if (func.RecureParam)
                                                    {
                                                        if (arguments.Count == 2)
                                                        {
                                                            componentName = arguments[0];
                                                            argument = arguments[1];
                                                        }
                                                        else if (arguments.Count == 1)
                                                        {
                                                            argument = arguments[0];
                                                        }
                                                        else if ((arguments.Count == 3) && (funcName == "SumDistinct"))
                                                        {
                                                            componentName = arguments[0];
                                                            argument = arguments[1];
                                                            condition = arguments[2];
                                                        }
                                                    }
                                                    else
                                                    {
                                                        componentName = arguments[0];
                                                    }
                                                }
                                                #endregion
                                            }

                                            par.Add(
                                                new StiFunctionParam(
                                                componentName,
                                                argument,
                                                condition,
                                                func.ServiceName,
                                                func,
                                                index,
                                                length,
                                                isContainerFunc,
                                                isColumnFunc,
                                                isRunningTotal,
                                                expression));
                                        }
                                        #endregion
                                    }
                                }
                            }
							#endregion

							#region Functions
							else 
							{
								var functions = StiFunctions.GetFunctions(report, funcName, true);

								if (functions != null)
								{
									index = token.Index;
									if (lexer.WaitLparen2() && 
										((index > 0 && text[index - 1] != '.') || index == 0))
									{					
										int startOfArgs = 0;
										int endOfArgs = 0;

										var arguments = GetArguments(lexer, ref startOfArgs, ref endOfArgs);
										length = token.Length;

										var param = 
											new StiFunctionParam(
											string.Empty, 
											string.Empty,
											string.Empty,
											funcName,
											func,
											index,
											length,
											false,
											false,
                                            false,
											expression);
										param.StartOfArgs = startOfArgs;
										param.EndOfArgs = endOfArgs;

										foreach (var function in functions)
										{
											if (function.ArgumentTypes == null)
											{
												if (arguments.Count == 0)
												{
													param.Function = function;
													break;
												}
												continue;
											}
											
											if (function.ArgumentTypes.Length == arguments.Count)
											{
												param.Function = function;
											}
										}

										if (param.Function == null && functions.Length > 0)
											param.Function = functions[0];
										
										if (param.Function != null)
											par.Add(param);
									}
								}
							}
							#endregion
						}
						catch (Exception e)
						{
							StiLogService.Write(typeof(StiFunctionsParser), "Parse Aggregate...ERROR");
							StiLogService.Write(typeof(StiFunctionsParser), e.Message);
						}
						#endregion					
					}
					#endregion
				}

				try
				{
                    if (!aggrIdent && lexer.PositionInText < lexer.Text.Length && lexer.Text[lexer.PositionInText] == '"')
                    {
                        token = new StiToken(StiTokenType.Value, lexer.PositionInText, 1);
                        lexer.PositionInText++;
                        token.Data = '"';
                    }
                        
					token = lexer.GetToken(); 
				}
				catch 
				{
					return par;
				}
			}
			return par;
			
		}
		#endregion
	}
}
