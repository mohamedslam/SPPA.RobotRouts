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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
    public partial class StiParser
    {
        #region Parser

        //----------------------------------------
        // Точка входа анализатора
        //----------------------------------------
        private void eval_exp()
        {
            tokenPos = 0;
            if (tokensList.Count == 0)
            {
                ThrowError(ParserErrorCode.ExpressionIsEmpty);
                return;
            }
            eval_exp0();
            if (tokenPos <= tokensList.Count)
            {
                ThrowError(ParserErrorCode.UnprocessedLexemesRemain);
            }
        }

        private void eval_exp0()
        {
            get_token();
            eval_exp1();
        }

        //----------------------------------------
        // Conditional Operator  ? : 
        //----------------------------------------
        private void eval_exp1()
        {
            eval_exp01();
            if (currentToken.Type == StiTokenType.Question)
            {
                if (checkSyntaxMode)
                {
                    //emulate conditional operator through IFF function; for correct check of types only
                    get_token();
                    eval_exp01();
                    if (currentToken.Type != StiTokenType.Colon) ThrowError(ParserErrorCode.SyntaxError, currentToken);
                    get_token();
                    eval_exp01();
                    asmList.Add(new StiAsmCommand(StiAsmCommandType.PushFunction, StiFunctionType.IIF, 3));
                }
                else
                {
                    get_token();    //skip question token
                    var jump1 = new StiAsmCommand(StiAsmCommandType.JumpFalse, 0);
                    asmList.Add(jump1);
                    int addr1 = asmList.Count;
                    eval_exp01();   //store first expression
                    if (currentToken.Type != StiTokenType.Colon) ThrowError(ParserErrorCode.SyntaxError, currentToken);
                    var jump2 = new StiAsmCommand(StiAsmCommandType.Jump, 0);
                    asmList.Add(jump2);
                    int addr2 = asmList.Count;
                    get_token();    //skip colon
                    eval_exp01();   //store second expression
                    jump1.Parameter1 = addr2 - addr1;
                    jump2.Parameter1 = asmList.Count - addr2;
                }
            }
        }

        //----------------------------------------
        // Обработка присваивания
        //----------------------------------------
        private void eval_exp01()
        {
            if ((currentToken.Type == StiTokenType.Variable) ||
                (currentToken.Type == StiTokenType.SystemVariable) && (currentToken.Value == "ReportName" || currentToken.Value == "ReportAuthor" || currentToken.Value == "ReportAlias" || currentToken.Value == "ReportDescription"))
            {
                StiToken variableToken = currentToken;
                get_token();
                if (currentToken.Type != StiTokenType.Assign)
                {
                    tokenPos--;
                    currentToken = tokensList[tokenPos - 1];
                }
                else
                {
                    get_token();
                    eval_exp10();
                    asmList.Add(new StiAsmCommand(StiAsmCommandType.CopyToVariable, variableToken.Value));
                    return;
                }
            }
            eval_exp10();
        }

        //----------------------------------------
        // Логическое ИЛИ
        //----------------------------------------
        private void eval_exp10()
        {
            eval_exp11();
            while (currentToken.Type == StiTokenType.DoubleOr)
            {
                get_token();
                eval_exp11();
                asmList.Add(new StiAsmCommand(StiAsmCommandType.Or2));
            }
        }

        //----------------------------------------
        // Логическое И
        //----------------------------------------
        private void eval_exp11()
        {
            eval_exp12();
            while (currentToken.Type == StiTokenType.DoubleAnd)
            {
                get_token();
                eval_exp12();
                asmList.Add(new StiAsmCommand(StiAsmCommandType.And2));
            }
        }

        //----------------------------------------
        // Бинарное ИЛИ
        //----------------------------------------
        private void eval_exp12()
        {
            eval_exp14();
            while (currentToken.Type == StiTokenType.Or)
            {
                get_token();
                eval_exp14();
                asmList.Add(new StiAsmCommand(StiAsmCommandType.Or));
            }
        }

        //----------------------------------------
        // Бинарное исключающее ИЛИ
        //----------------------------------------
        private void eval_exp14()
        {
            eval_exp15();
            if (currentToken.Type == StiTokenType.Xor)
            {
                get_token();
                eval_exp15();
                asmList.Add(new StiAsmCommand(StiAsmCommandType.Xor));
            }
        }

        //----------------------------------------
        // Бинарное И
        //----------------------------------------
        private void eval_exp15()
        {
            eval_exp16();
            while (currentToken.Type == StiTokenType.And)
            {
                get_token();
                eval_exp16();
                asmList.Add(new StiAsmCommand(StiAsmCommandType.And));
            }
        }


        //----------------------------------------
        // Equality (==, !=)
        //----------------------------------------
        private void eval_exp16()
        {
            eval_exp17();
            if (currentToken.Type == StiTokenType.Equal || currentToken.Type == StiTokenType.NotEqual)
            {
                StiAsmCommand command = new StiAsmCommand(StiAsmCommandType.CompareEqual);
                if (currentToken.Type == StiTokenType.NotEqual) command.Type = StiAsmCommandType.CompareNotEqual;
                get_token();
                eval_exp17();
                asmList.Add(command);
            }
        }

        //----------------------------------------
        // Relational and type testing (<, >, <=, >=, is, as)
        //----------------------------------------
        private void eval_exp17()
        {
            eval_exp18();
            if (currentToken.Type == StiTokenType.Left || currentToken.Type == StiTokenType.LeftEqual ||
                currentToken.Type == StiTokenType.Right || currentToken.Type == StiTokenType.RightEqual)
            {
                StiAsmCommand command = null;
                if (currentToken.Type == StiTokenType.Left) command = new StiAsmCommand(StiAsmCommandType.CompareLeft);
                if (currentToken.Type == StiTokenType.LeftEqual) command = new StiAsmCommand(StiAsmCommandType.CompareLeftEqual);
                if (currentToken.Type == StiTokenType.Right) command = new StiAsmCommand(StiAsmCommandType.CompareRight);
                if (currentToken.Type == StiTokenType.RightEqual) command = new StiAsmCommand(StiAsmCommandType.CompareRightEqual);
                get_token();
                eval_exp18();
                asmList.Add(command);
            }
        }


        //----------------------------------------
        // Shift (<<, >>)
        //----------------------------------------
        private void eval_exp18()
        {
            eval_exp2();
            if ((currentToken.Type == StiTokenType.Shl) || (currentToken.Type == StiTokenType.Shr))
            {
                StiAsmCommand command = new StiAsmCommand(StiAsmCommandType.Shl);
                if (currentToken.Type == StiTokenType.Shr) command.Type = StiAsmCommandType.Shr;
                get_token();
                eval_exp2();
                asmList.Add(command);
            }
        }


        //----------------------------------------
        // Сложение или вычитание двух слагаемых
        //----------------------------------------
        private void eval_exp2()
        {
            eval_exp3();
            while ((currentToken.Type == StiTokenType.Plus) || (currentToken.Type == StiTokenType.Minus))
            {
                StiToken operation = currentToken;
                get_token();
                eval_exp3();
                if (operation.Type == StiTokenType.Minus)
                {
                    asmList.Add(new StiAsmCommand(StiAsmCommandType.Sub));
                }
                else if (operation.Type == StiTokenType.Plus)
                {
                    asmList.Add(new StiAsmCommand(StiAsmCommandType.Add));
                }
            }
        }


        //----------------------------------------
        // Умножение или деление двух множителей
        //----------------------------------------
        private void eval_exp3()
        {
            eval_exp4();
            while (currentToken.Type == StiTokenType.Mult || currentToken.Type == StiTokenType.Div || currentToken.Type == StiTokenType.Percent)
            {
                StiToken operation = currentToken;
                get_token();
                eval_exp4();
                if (operation.Type == StiTokenType.Mult)
                {
                    asmList.Add(new StiAsmCommand(StiAsmCommandType.Mult));
                }
                else if (operation.Type == StiTokenType.Div)
                {
                    asmList.Add(new StiAsmCommand(StiAsmCommandType.Div));
                }
                if (operation.Type == StiTokenType.Percent)
                {
                    asmList.Add(new StiAsmCommand(StiAsmCommandType.Mod));
                }
            }
        }


        //----------------------------------------
        // Возведение в степень
        //----------------------------------------
        private void eval_exp4()
        {
            eval_exp5();
            //if (currentToken.Type == StiTokenType.Xor)
            //{
            //    get_token();
            //    eval_exp4();
            //    asmList.Add(new StiAsmCommand(StiAsmCommandType.Power));
            //}
        }


        //----------------------------------------
        // Вычисление унарного + и -
        //----------------------------------------
        private void eval_exp5()
        {
            StiAsmCommand command = null;
            if (currentToken.Type == StiTokenType.Plus || currentToken.Type == StiTokenType.Minus || currentToken.Type == StiTokenType.Not)
            {
                if (currentToken.Type == StiTokenType.Minus) command = new StiAsmCommand(StiAsmCommandType.Neg);
                if (currentToken.Type == StiTokenType.Not) command = new StiAsmCommand(StiAsmCommandType.Not);
                get_token();
            }
            eval_exp6();
            if (command != null)
            {
                asmList.Add(command);
            }
        }


        //----------------------------------------
        // Обработка выражения в скобках
        //----------------------------------------
        private void eval_exp6()
        {
            if (currentToken.Type == StiTokenType.LParenthesis)
            {
                get_token();
                if (currentToken.Type == StiTokenType.Cast)
                {
                    var typeCode = currentToken.ValueObject;
                    get_token();
                    if (currentToken.Type != StiTokenType.RParenthesis)
                    {
                        ThrowError(ParserErrorCode.RightParenthesisExpected);  //несбалансированные скобки
                    }
                    get_token();
                    eval_exp5();
                    asmList.Add(new StiAsmCommand(StiAsmCommandType.Cast, typeCode));
                }
                else
                {
                    eval_exp1();
                    if (currentToken.Type != StiTokenType.RParenthesis)
                    {
                        ThrowError(ParserErrorCode.RightParenthesisExpected);  //несбалансированные скобки
                    }
                    get_token();
                    if (currentToken.Type == StiTokenType.Dot)
                    {
                        get_token();
                        eval_exp7();
                    }
                    if (currentToken.Type == StiTokenType.LBracket)
                    {
                        eval_exp62();
                    }
                }
            }
            else
            {
                eval_exp62();
            }
        }


        //----------------------------------------
        // Обработка индексов
        //----------------------------------------
        private void eval_exp62()
        {
            if (currentToken.Type == StiTokenType.LBracket)
            {
                int argsCount = 0;
                while (argsCount == 0 || currentToken.Type == StiTokenType.Comma)
                {
                    get_token();
                    eval_exp1();
                    argsCount++;
                }
                if (currentToken.Type != StiTokenType.RBracket)
                {
                    ThrowError(ParserErrorCode.SyntaxError, currentToken);  //несбалансированные квадратные скобки  //!!!
                }
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushArrayElement, argsCount + 1));
                get_token();
                if (currentToken.Type == StiTokenType.LBracket)
                {
                    eval_exp62();
                }
                if (currentToken.Type == StiTokenType.Dot)
                {
                    get_token();
                    eval_exp7();
                }
            }
            else
            {
                eval_exp7();
            }
        }


        //----------------------------------------
        // Вычисление методов и свойств
        //----------------------------------------
        private void eval_exp7()
        {
            atom();
            if (currentToken.Type == StiTokenType.Dot)
            {
                get_token();
                eval_exp7();
            }
            if (currentToken.Type == StiTokenType.LBracket)
            {
                eval_exp62();
            }
        }


        //----------------------------------------
        // Получение значения числа или переменной
        //----------------------------------------
        private void atom()
        {
            if (currentToken.Type == StiTokenType.Variable)
            {
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushVariable, currentToken.Value));
                get_token();
                return;
            }
            if (currentToken.Type == StiTokenType.RefVariable)
            {
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushRefVariable, currentToken.Value));
                get_token();
                return;
            }
            else if (currentToken.Type == StiTokenType.SystemVariable)
            {
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushSystemVariable, SystemVariablesList[currentToken.Value]));
                get_token();
                return;
            }
            else if (currentToken.Type == StiTokenType.Function)
            {
                StiToken function = currentToken;
                StiFunctionType functionType;
                object objFunc = FunctionsList[function.Value];
                if (objFunc != null)
                {
                    functionType = (StiFunctionType)objFunc;
                }
                else
                {
                    functionType = (StiFunctionType)UserFunctionsList[function.Value];
                }
                if (functionType == StiFunctionType.IIF && ProcessIifAsTernary)
                {
                    get_args_count(functionType);   //all commands stored inside
                }
                else
                {
                    asmList.Add(new StiAsmCommand(StiAsmCommandType.PushFunction, functionType, get_args_count(functionType)) { Position = expressionPosition + function.Position, Length = function.Length });
                }
                get_token();
                return;
            }
            else if (currentToken.Type == StiTokenType.New)
            {
                StiToken function = currentToken;
                StiFunctionType functionType = StiFunctionType.NewType;
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushValue, Activator.CreateInstance((Type)function.ValueObject)));
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushFunction, functionType, get_args_count(functionType) + 1) { Position = expressionPosition + function.Position, Length = function.Length });
                get_token();
                return;
            }
            else if (currentToken.Type == StiTokenType.Method)
            {
                StiToken method = currentToken;
                StiMethodType methodType = (StiMethodType)MethodsList[method.Value];
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushMethod, methodType, get_args_count(methodType) + 1));
                get_token();
                return;
            }
            else if (currentToken.Type == StiTokenType.Property)
            {
                StiToken property = currentToken;
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushProperty, PropertiesList[property.Value] ?? property.Value));
                get_token();
                return;
            }
            else if (currentToken.Type == StiTokenType.DataSourceField)
            {
                var fieldInfo = currentToken.ValueObject as StiParserDataSourceFieldInfo;
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushDataSourceField, currentToken.Value, fieldInfo.Path.Count, fieldInfo));
                get_token();
                return;
            }
            else if (currentToken.Type == StiTokenType.BusinessObjectField)
            {
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushBusinessObjectField, currentToken.Value));
                get_token();
                return;
            }
            else if (currentToken.Type == StiTokenType.Component)
            {
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushComponent, ComponentsList[currentToken.Value]) { Position = expressionPosition + currentToken.Position, Length = currentToken.Length });
                get_token();
                return;
            }
            else if (currentToken.Type == StiTokenType.Number)
            {
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushValue, currentToken.ValueObject));
                get_token();
                return;
            }
            else if (currentToken.Type == StiTokenType.String)
            {
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushValue, currentToken.ValueObject));
                get_token();
                return;
            }
            else if (currentToken.Type == StiTokenType.Char)
            {
                asmList.Add(new StiAsmCommand(StiAsmCommandType.PushValue, currentToken.ValueObject));
                get_token();
                return;
            }
            else
            {
                if (currentToken.Type == StiTokenType.Empty)
                {
                    ThrowError(ParserErrorCode.UnexpectedEndOfExpression);
                }
                ThrowError(ParserErrorCode.SyntaxError, currentToken);
            }
        }


        //----------------------------------------
        // Получение аргументов функции
        //----------------------------------------
        private int get_args_count(object name)
        {
            int posFunc = -1;
            bool isTotal = false;
            if (currentToken != null)
            {
                posFunc = currentToken.Position;
                isTotal = currentToken.Value.StartsWith("Totals.");
            }

            var args = get_args();

            //If aggregateComponent is not specified, search it
            var func = (StiFunctionType)name;
            if ((func == StiFunctionType.Count || func == StiFunctionType.rCount ||
                 func == StiFunctionType.cCount || func == StiFunctionType.crCount ||
                 func == StiFunctionType.pCount || func == StiFunctionType.prCount) && (args.Count == 0) ||

                (func >= StiFunctionType.CountDistinct && func <= StiFunctionType.Last ||
                 func >= StiFunctionType.rCountDistinct && func <= StiFunctionType.rLast ||
                 func >= StiFunctionType.cCountDistinct && func <= StiFunctionType.cLast ||
                 func >= StiFunctionType.crCountDistinct && func <= StiFunctionType.crLast ||
                 func >= StiFunctionType.pCountDistinct && func <= StiFunctionType.pLast ||
                 func >= StiFunctionType.prCountDistinct && func <= StiFunctionType.prLast) && (args.Count == 1) ||

                (func == StiFunctionType.iCount || func == StiFunctionType.riCount ||
                 func == StiFunctionType.ciCount || func == StiFunctionType.criCount ||
                 func == StiFunctionType.piCount || func == StiFunctionType.priCount) && (args.Count == 1) ||

                (func >= StiFunctionType.iCountDistinct && func <= StiFunctionType.iLast ||
                 func >= StiFunctionType.riCountDistinct && func <= StiFunctionType.riLast ||
                 func >= StiFunctionType.ciCountDistinct && func <= StiFunctionType.ciLast ||
                 func >= StiFunctionType.criCountDistinct && func <= StiFunctionType.criLast ||
                 func >= StiFunctionType.piCountDistinct && func <= StiFunctionType.piLast ||
                 func >= StiFunctionType.priCountDistinct && func <= StiFunctionType.priLast) && (args.Count == 2))
            {
                object aggregateComponent = component.GetGroupHeaderBand();
                if (aggregateComponent == null)
                    aggregateComponent = component.GetDataBand();

                #region Check for dataSource
                if ((aggregateComponent == null) && (args.Count > 0) && (args[0] != null) && (args[0].Count > 0))
                {
                    try
                    {
                        string dsName = null;
                        foreach (StiAsmCommand command in args[0])
                        {
                            if (command.Type == StiAsmCommandType.PushDataSourceField)
                            {
                                var fieldInfo = command.Parameter2 as StiParserDataSourceFieldInfo;
                                string dsName2 = fieldInfo.Path[0];
                                if (dsName == null)
                                {
                                    dsName = dsName2;
                                }
                                else
                                {
                                    if (dsName != dsName2)
                                    {
                                        dsName = null;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(dsName))
                        {
                            aggregateComponent = report.Dictionary.DataSources[dsName];
                        }
                    }
                    catch { }
                }
                #endregion

                var newCommand = new List<StiAsmCommand>();
                newCommand.Add(new StiAsmCommand(StiAsmCommandType.PushComponent, aggregateComponent) { Position = expressionPosition + posFunc });
                args.Insert(0, newCommand);
            }

            //изменяем aggregateComponent если надо
            bool alreadyGroup = (args.Count > 0) && (args[0].Count > 0) && (args[0][0].Type == StiAsmCommandType.PushComponent) && (args[0][0].Parameter1 is StiGroupHeaderBand);
            if ((!alreadyGroup) && !isTotal &&
                ((func >= StiFunctionType.Count && func <= StiFunctionType.Last) ||
                (func >= StiFunctionType.rCount && func <= StiFunctionType.rLast) ||
                (func >= StiFunctionType.iCount && func <= StiFunctionType.iLast) ||
                (func >= StiFunctionType.riCount && func <= StiFunctionType.riLast) ||
                (func >= StiFunctionType.cCount && func <= StiFunctionType.cLast) ||
                (func >= StiFunctionType.crCount && func <= StiFunctionType.crLast) ||
                (func >= StiFunctionType.ciCount && func <= StiFunctionType.ciLast) ||
                (func >= StiFunctionType.criCount && func <= StiFunctionType.criLast) ||
                (func >= StiFunctionType.pCount && func <= StiFunctionType.pLast) ||
                (func >= StiFunctionType.prCount && func <= StiFunctionType.prLast) ||
                (func >= StiFunctionType.piCount && func <= StiFunctionType.piLast) ||
                (func >= StiFunctionType.priCount && func <= StiFunctionType.priLast)))
            {
                StiComponent aggregateComponent = component.GetGroupHeaderBand();
                if (aggregateComponent != null)
                {
                    var newCommand = new List<StiAsmCommand>();
                    newCommand.Add(new StiAsmCommand(StiAsmCommandType.PushComponent, aggregateComponent));
                    args[0] = newCommand;
                }
            }

            //запоминаем имя бэнда, чтобы он не удалился при чистке страницы 
            if ((func >= StiFunctionType.pCount && func <= StiFunctionType.pLast ||
                 func >= StiFunctionType.prCount && func <= StiFunctionType.prLast ||
                 func >= StiFunctionType.piCount && func <= StiFunctionType.piLast ||
                 func >= StiFunctionType.priCount && func <= StiFunctionType.priLast) && (args.Count > 0))
            {
                var arg0 = args[0];
                if ((arg0 != null) && (arg0.Count > 0) &&
                    (arg0[0].Type == StiAsmCommandType.PushComponent) &&
                    (arg0[0].Parameter1 is StiBand))
                {
                    string dataBandName = (arg0[0].Parameter1 as StiBand).Name;

                    #region Add dataBandName
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
            }

            if ((func == StiFunctionType.IIF) && (args.Count == 3) && ProcessIifAsTernary)
            {
                asmList.AddRange(args[0]);
                var jump1 = new StiAsmCommand(StiAsmCommandType.JumpFalse, 0);
                asmList.Add(jump1);
                int addr1 = asmList.Count;
                asmList.AddRange(args[1]);   //store first expression
                var jump2 = new StiAsmCommand(StiAsmCommandType.Jump, 0);
                asmList.Add(jump2);
                int addr2 = asmList.Count;
                asmList.AddRange(args[2]);   //store second expression
                jump1.Parameter1 = addr2 - addr1;
                jump2.Parameter1 = asmList.Count - addr2;
                return args.Count;
            }

            //проверяем какие параметры надо оставить в виде выражений
            int bitsValue = 0;
            if (ParametersList.Contains(name)) bitsValue = (int)ParametersList[name];
            int bitsCounter = 1;
            foreach (var arg in args)
            {
                if ((bitsValue & bitsCounter) > 0)
                {
                    asmList.Add(new StiAsmCommand(StiAsmCommandType.PushValue, arg));
                }
                else
                {
                    asmList.AddRange(arg);
                }
                bitsCounter = bitsCounter << 1;
            }

            return args.Count;
        }

        private List<List<StiAsmCommand>> get_args()
        {
            var args = new List<List<StiAsmCommand>>();

            get_token();
            if (currentToken.Type != StiTokenType.LParenthesis) ThrowError(ParserErrorCode.LeftParenthesisExpected);   //ожидается открывающая скобка
            get_token();
            if (currentToken.Type == StiTokenType.RParenthesis)
            {
                //пустой список
                return args;
            }
            else
            {
                tokenPos--;
                currentToken = tokensList[tokenPos - 1];
            }

            //обработка списка значений
            List<StiAsmCommand> tempAsmList = asmList;
            do
            {
                asmList = new List<StiAsmCommand>();
                eval_exp0();
                args.Add(asmList);
            }
            while (currentToken.Type == StiTokenType.Comma);
            asmList = tempAsmList;

            if (currentToken.Type != StiTokenType.RParenthesis) ThrowError(ParserErrorCode.RightParenthesisExpected);   //ожидается закрывающая скобка
            return args;
        }


        private void get_token()
        {
            if (tokenPos < tokensList.Count)
            {
                currentToken = tokensList[tokenPos];
            }
            else
            {
                currentToken = new StiToken();
            }
            tokenPos++;
        }

        #endregion
    }
}
