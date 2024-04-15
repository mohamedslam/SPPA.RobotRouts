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
using System.Text;
using Stimulsoft.Report.Dictionary;

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace Stimulsoft.Report.Engine
{
#if NETSTANDARD
    using System = global::System;
#endif

    public partial class StiParser
    {
        #region Exception provider

        private static string[] errorsList =
        {
            "Syntax error", //0
            "Integral constant is too large", //1        значение целочисленной константы слишком велико
            "The expression is empty", //2
            "Division by zero", //3
            "Unexpected end of expression", //4
            "The name '{0}' does not exist in the current context", //5
            "Syntax error - unprocessed lexemes remain", //6
            "( expected", //7
            ") expected", //8
            "Field, method, or property is not found: '{0}'", //9
            "Operator '{0}' cannot be applied to operands of type '{1}' and type '{2}'", //10
            "The function is not found: '{0}'", //11
            "No overload for method '{0}' takes '{1}' arguments", //12
            "The '{0}' function has invalid argument '{1}': cannot convert from '{2}' to '{3}'", //13
            "The '{0}' function is not yet implemented", //14
            "The '{0}' method has invalid argument '{1}': cannot convert from '{2}' to '{3}'", //15
            "'{0}' does not contain a definition for '{1}'", //16
            "There is no matching overloaded method for '{0}({1})'", //17
            "The type or namespace name '{0}' does not exist in the namespace '{1}'"};  //18


        private enum ParserErrorCode
        {
            SyntaxError = 0,
            IntegralConstantIsTooLarge = 1,
            ExpressionIsEmpty = 2,
            DivisionByZero = 3,
            UnexpectedEndOfExpression = 4,
            NameDoesNotExistInCurrentContext = 5,
            UnprocessedLexemesRemain = 6,
            LeftParenthesisExpected = 7,
            RightParenthesisExpected = 8,
            FieldMethodOrPropertyNotFound = 9,
            OperatorCannotBeAppliedToOperands = 10,
            FunctionNotFound = 11,
            NoOverloadForMethodTakesNArguments = 12,
            FunctionHasInvalidArgument = 13,
            FunctionNotYetImplemented = 14,
            MethodHasInvalidArgument = 15,
            ItemDoesNotContainDefinition = 16,
            NoMatchingOverloadedMethod = 17,
            TheTypeOrNamespaceNotExistInTheNamespace = 18
        }


        // Отображение сообщения о синтаксической ошибке
        private void ThrowError(ParserErrorCode code)
        {
            ThrowError(code, null, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        private void ThrowError(ParserErrorCode code, string message1)
        {
            ThrowError(code, null, message1, string.Empty, string.Empty, string.Empty);
        }

        private void ThrowError(ParserErrorCode code, string message1, string message2)
        {
            ThrowError(code, null, message1, message2, string.Empty, string.Empty);
        }

        private void ThrowError(ParserErrorCode code, string message1, string message2, string message3)
        {
            ThrowError(code, null, message1, message2, message3, string.Empty);
        }

        private void ThrowError(ParserErrorCode code, string message1, string message2, string message3, string message4)
        {
            ThrowError(code, null, message1, message2, message3, message4);
        }


        private void ThrowError(ParserErrorCode code, StiToken token)
        {
            ThrowError(code, token, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        private void ThrowError(ParserErrorCode code, StiToken token, string message1)
        {
            ThrowError(code, token, message1, string.Empty, string.Empty, string.Empty);
        }

        private void ThrowError(ParserErrorCode code, StiToken token, string message1, string message2)
        {
            ThrowError(code, token, message1, message2, string.Empty, string.Empty);
        }

        private void ThrowError(ParserErrorCode code, StiToken token, string message1, string message2, string message3)
        {
            ThrowError(code, token, message1, message2, message3, string.Empty);
        }

        private void ThrowError(ParserErrorCode code, StiToken token, string message1, string message2, string message3, string message4)
        {
            string errorMessage = "Unknown error";
            int errorCode = (int)code;
            if (errorCode < errorsList.Length)
            {
                errorMessage = string.Format(errorsList[errorCode], message1, message2, message3, message4);
            }
            string fullMessage = "Parser error: " + errorMessage;
            StiParserException ex = new StiParserException(fullMessage);
            ex.BaseMessage = errorMessage;
            if (token != null)
            {
                ex.Position = expressionPosition + token.Position;
                ex.Length = token.Length;
            }
            throw ex;
        }

        public class StiParserException : Exception
        {
            public string BaseMessage = null;
            public int Position = -1;
            public int Length = -1;

            public StiParserException(string message)
                : base(message)
            {
            }

            public StiParserException()
                : base()
            {
            }
        }

        #endregion

        #region CheckTypes
        private void CheckTypes(List<StiAsmCommand> asmList)
        {
            if (asmList == null || asmList.Count == 0) return;
            Stack<StiCheckTypeData> stack = new Stack<StiCheckTypeData>();
            List<StiCheckTypeData> argsList = null;
            Type[] types = null;
            StiCheckTypeData par1;
            StiCheckTypeData par2;

            foreach (StiAsmCommand asmCommand in asmList)
            {
                Type type = typeof(object);

                switch (asmCommand.Type)
                {
                    case StiAsmCommandType.PushValue:
                        stack.Push(new StiCheckTypeData(asmCommand.Parameter1 == null ? typeof(object) : asmCommand.Parameter1.GetType(), asmCommand.Position, asmCommand.Length));
                        break;

                    case StiAsmCommandType.PushVariable:
                    case StiAsmCommandType.PushRefVariable:
                        #region push variable type
                        string varName = (string)asmCommand.Parameter1;
                        StiVariable var = report.Dictionary.Variables[varName];
                        if (var != null)
                        {
                            type = var.Type;
                        }
                        else
                        {
                            if (report.Variables != null && report.Variables.ContainsKey(varName))
                            {
                                object varValue = report.Variables[varName];
                                if (varValue != null) type = varValue.GetType();
                            }
                        }
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        #endregion
                        break;

                    case StiAsmCommandType.PushSystemVariable:
                        object systemVariableValue = get_systemVariable(asmCommand.Parameter1);
                        if (systemVariableValue != null) type = systemVariableValue.GetType();
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        break;

                    case StiAsmCommandType.PushComponent:
                        stack.Push(new StiCheckTypeData((asmCommand.Parameter1 == null ? typeof(object) : asmCommand.Parameter1.GetType()), asmCommand.Position, asmCommand.Length));
                        break;

                    case StiAsmCommandType.CopyToVariable:
                        stack.Peek();
                        break;

                    case StiAsmCommandType.PushFunction:
                        #region Push function value
                        argsList = new List<StiCheckTypeData>();
                        for (int index = 0; index < asmCommand.ArgsCount; index++)
                        {
                            argsList.Add(stack.Pop());
                        }
                        argsList.Reverse();
                        types = new Type[argsList.Count];
                        for (int index = 0; index < argsList.Count; index++)
                        {
                            types[index] = argsList[index].TypeCode;
                        }
                        StiParserMethodInfo methodInfo = GetParserMethodInfo((StiFunctionType)asmCommand.Parameter1, types);
                        type = methodInfo != null ? methodInfo.ReturnType : typeof(object);
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        #endregion
                        break;

                    case StiAsmCommandType.PushMethod:
                        #region Push method value
                        argsList = new List<StiCheckTypeData>();
                        for (int index = 0; index < asmCommand.ArgsCount; index++)
                        {
                            argsList.Add(stack.Pop() as StiCheckTypeData);
                        }
                        argsList.Reverse();
                        types = new Type[argsList.Count];
                        for (int index = 0; index < argsList.Count; index++)
                        {
                            types[index] = argsList[index].TypeCode;
                        }
                        type = GetMethodResultType((StiMethodType)asmCommand.Parameter1, types);
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        #endregion
                        break;

                    case StiAsmCommandType.PushProperty:
                        type = GetPropertyType(asmCommand.Parameter1.ToString(), stack.Pop().TypeCode);
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        break;

                    case StiAsmCommandType.PushDataSourceField:
                        #region Push DataSource field
                        List<string> parts = new List<string>(((string)asmCommand.Parameter1).Split(new char[] { '.' }));
                        StiDataSource ds = report.Dictionary.DataSources[parts[0]];
                        if ((ds is StiVirtualSource) && (parts.Count > 2) && !ds.Columns.Contains(parts[1]))
                        {
                            //recombine the fields names with comma, especially for StiVirtualSource
                            string columnName = parts[1] + "." + parts[2];
                            if (ds.Columns.Contains(columnName))
                            {
                                parts[1] = columnName;
                                parts.RemoveAt(2);
                            }
                            else
                            {
                                if (parts.Count > 3)
                                {
                                    columnName += "." + parts[3];
                                    if (ds.Columns.Contains(columnName))
                                    {
                                        parts[1] = columnName;
                                        parts.RemoveAt(2);
                                        parts.RemoveAt(2);
                                    }
                                }
                            }
                        }
                        if (parts.Count > 1)
                        {
                            if (parts.Count == 2)
                            {
                                StiDataColumn column = ds.Columns[parts[1]];
                                if (column != null)
                                {
                                    type = column.Type;
                                }
                            }
                            else
                            {
                                string nameInSource = parts[1];
                                ds = ds.GetParentDataSource(nameInSource);
                                int indexPart = 2;
                                while (indexPart < parts.Count - 1)
                                {
                                    nameInSource = parts[indexPart];
                                    ds = ds.GetParentDataSource(nameInSource);
                                    indexPart++;
                                }
                                StiDataColumn column = ds.Columns[parts[indexPart]];
                                if (column != null)
                                {
                                    type = column.Type;
                                }
                            }
                        }
                        else
                        {
                            type = ds.GetType();
                        }
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        #endregion
                        break;

                    case StiAsmCommandType.PushBusinessObjectField:
                        #region Push BusinessObject field
                        string[] parts2 = ((string)asmCommand.Parameter1).Split(new char[] { '.' });
                        StiBusinessObject bos = report.Dictionary.BusinessObjects[parts2[0]];
                        if (parts2.Length > 1)
                        {
                            string nextName = null;
                            int indexPart = 1;
                            while (indexPart < parts2.Length - 1)
                            {
                                nextName = parts2[indexPart];
                                if (bos.Columns.Contains(nextName))
                                {
                                    break;
                                }
                                bos = bos.BusinessObjects[nextName];
                                indexPart++;
                            }
                            if (bos.Columns.Contains(parts2[indexPart]))
                            {
                                type = bos.Columns[parts2[indexPart]].Type;
                            }
                            else
                            {
                                type = bos.BusinessObjects[parts2[indexPart]].GetType();    // ???
                            }
                        }
                        else
                        {
                            type = bos.GetType();   // ???
                        }
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        #endregion
                        break;

                    case StiAsmCommandType.PushArrayElement:
                        #region Push array value
                        argsList = new List<StiCheckTypeData>();
                        for (int index = 0; index < (int)asmCommand.Parameter1; index++)
                        {
                            argsList.Add(stack.Pop());
                        }
                        argsList.Reverse();
                        types = new Type[argsList.Count];
                        for (int index = 0; index < argsList.Count; index++)
                        {
                            types[index] = argsList[index].TypeCode;
                        }
                        type = GetArrayElementType(types);
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        #endregion
                        break;

                    case StiAsmCommandType.Add:
                    case StiAsmCommandType.Sub:
                    case StiAsmCommandType.Mult:
                    case StiAsmCommandType.Div:
                    case StiAsmCommandType.Mod:
                    case StiAsmCommandType.Shl:
                    case StiAsmCommandType.Shr:
                    case StiAsmCommandType.And:
                    case StiAsmCommandType.Or:
                    case StiAsmCommandType.Xor:
                    case StiAsmCommandType.And2:
                    case StiAsmCommandType.Or2:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        types = new Type[2] { par1.TypeCode, par2.TypeCode };
                        StiParserMethodInfo methodInfo2 = GetParserMethodInfo((StiFunctionType)asmCommand.Type, types);
                        type = methodInfo2 != null ? methodInfo2.ReturnType : typeof(object);
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        break;

                    case StiAsmCommandType.Neg:
                    case StiAsmCommandType.Not: 
                        par1 = stack.Pop();
                        types = new Type[1] { par1.TypeCode };
                        StiParserMethodInfo methodInfo3 = GetParserMethodInfo((StiFunctionType)asmCommand.Type, types);
                        type = methodInfo3 != null ? methodInfo3.ReturnType : typeof(object);
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        break;

                    //case StiAsmCommandType.Power:

                    case StiAsmCommandType.CompareLeft:
                    case StiAsmCommandType.CompareLeftEqual:
                    case StiAsmCommandType.CompareRight:
                    case StiAsmCommandType.CompareRightEqual:
                    case StiAsmCommandType.CompareEqual:
                    case StiAsmCommandType.CompareNotEqual:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        //
                        // need to check type equality; todo
                        //
                        type = typeof(bool);
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        break;

                    case StiAsmCommandType.Cast:
                        par1 = stack.Pop();
                        type = Type.GetType("System." + asmCommand.Parameter1);
                        stack.Push(new StiCheckTypeData(type, asmCommand.Position, asmCommand.Length));
                        break;

                }
            }
        }


        private Type GetMethodResultType(StiMethodType type, Type[] args)
        {
            Type baseType = args[0];
            int count = args.Length - 1;
            string methodName = type.ToString();
            try
            {
                MethodInfo[] methods = baseType.GetMethods();
                bool flag0 = false;
                bool flag1 = false;
                foreach (var mi in methods)
                {
                    if (mi.Name != methodName) continue;
                    flag0 = true;
                    ParameterInfo[] pi = mi.GetParameters();
                    if (pi.Length != count)
                    {
                        if ((count == 0) && (pi.Length == 1))
                        {
                            //проверка сделана для TrimStart/TrimEnd, в этих функциях параметр может быть char[] или null, пока не нашёл лучшего способа проверки.
                            var cc = pi[0].GetCustomAttributes(typeof(System.ParamArrayAttribute), false);
                            if (cc.Length == 1) return mi.ReturnType;
                        }
                        continue;
                    }
                    flag1 = true;
                    bool flag2 = true;
                    for (int index = 0; index < count; index++)
                    {
                        if (IsImplicitlyCastableTo(args[index + 1], pi[index].ParameterType, null)) continue;
                        flag2 = false;
                        break;
                    }
                    if (flag2) return mi.ReturnType;
                }

                if (!flag0)
                {
                    ThrowError(ParserErrorCode.FieldMethodOrPropertyNotFound, Enum.GetName(typeof(StiMethodType), type));
                }

                if (!flag1)
                {
                    ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, Enum.GetName(typeof(StiMethodType), type), count.ToString());
                }
            }
            catch
            {
                return typeof(object);
            }

            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < count; index++)
            {
                sb.Append(args[index + 1].Namespace == "System" ? args[index + 1].Name : args[index + 1].ToString());
                if (index < count - 1) sb.Append(",");
            }

            ThrowError(ParserErrorCode.NoMatchingOverloadedMethod, Enum.GetName(typeof(StiMethodType), type), sb.ToString());

            return typeof(object);
        }

        private Type GetPropertyType(string propertyName, Type baseType)
        {
            PropertyInfo[] properties = baseType.GetProperties();
            foreach (var pi in properties)
            {
                if (pi.Name == propertyName) return pi.PropertyType;
            }

            FieldInfo[] fields = baseType.GetFields();
            foreach (var fi in fields)
            {
                if (fi.IsPublic && (fi.Name == propertyName)) return fi.FieldType;
            }

            ThrowError(ParserErrorCode.FieldMethodOrPropertyNotFound, propertyName);

            return typeof(object);
        }

        private Type GetArrayElementType(Type[] argsList)
        {
            Type baseType = argsList[0];

            if (argsList.Length < 2) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "get_ArrayElement", (argsList.Length - 1).ToString());

            if (baseType == typeof(string))
            {
                if (argsList.Length != 2) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "string.get_Item", (argsList.Length - 1).ToString());
                if (!IsImplicitlyCastableTo(argsList[1].GetType(), typeof(int), null)) ThrowError(ParserErrorCode.NoMatchingOverloadedMethod, "string.get_Item", argsList[1].GetType().ToString()); ;
                return typeof(char);
            }

            PropertyInfo pi = baseType.GetProperty("Item");
            if (pi != null)
            {
                ParameterInfo[] pis = pi.GetGetMethod().GetParameters();
                if (pis.Length > 0)
                {
                    if (argsList.Length < 2) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "object.get_Item", (argsList.Length - 1).ToString());
                    // !!! need check parameters types
                    return pi.PropertyType;
                }
            }
            else if (baseType.IsAssignableFrom(typeof(Array)))
            {
                // !!! need test
                // !!! need check parameters types
                return baseType.UnderlyingSystemType;
            }
            else if (baseType.IsAssignableFrom(typeof(IList)))
            {
                // !!! need test
                // !!! need check parameters types
                return baseType.UnderlyingSystemType;
            }

            return typeof(object);
        }



        private class StiCheckTypeData
        {
            public Type TypeCode;
            public int Position = -1;
            public int Length = -1;

            public StiCheckTypeData(Type typeCode, int position, int length)
            {
                this.TypeCode = typeCode;
                this.Position = position;
                this.Length = length;
            }

            public override string ToString()
            {
                return string.Format("{0}", TypeCode);
            }
        }
         

        #endregion
        
        #region IsImplicitlyCastableTo
        private static readonly Dictionary<KeyValuePair<Type, Type>, bool> ImplicitCastCache = new Dictionary<KeyValuePair<Type, Type>, bool>() {
            { new KeyValuePair<Type, Type>(typeof(System.String), typeof(System.Int32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.String), typeof(System.UInt32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.String), typeof(System.Int64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.String), typeof(System.UInt64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.String), typeof(System.Single)), false },
            { new KeyValuePair<Type, Type>(typeof(System.String), typeof(System.Double)), false },
            { new KeyValuePair<Type, Type>(typeof(System.String), typeof(System.Decimal)), false },
            { new KeyValuePair<Type, Type>(typeof(System.String), typeof(System.Char)), false },

            { new KeyValuePair<Type, Type>(typeof(System.Decimal), typeof(System.Int32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Decimal), typeof(System.UInt32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Decimal), typeof(System.Int64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Decimal), typeof(System.UInt64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Decimal), typeof(System.Double)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Decimal), typeof(System.Single)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Decimal), typeof(System.String)), false },

            { new KeyValuePair<Type, Type>(typeof(System.Double), typeof(System.Int32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Double), typeof(System.UInt32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Double), typeof(System.Int64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Double), typeof(System.UInt64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Double), typeof(System.Single)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Double), typeof(System.Decimal)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Double), typeof(System.String)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Double), typeof(System.DateTime)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Double), typeof(System.TimeSpan)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Double), typeof(System.Char)), false },

            { new KeyValuePair<Type, Type>(typeof(System.Boolean), typeof(System.Int32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Boolean), typeof(System.UInt32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Boolean), typeof(System.Int64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Boolean), typeof(System.UInt64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Boolean), typeof(System.Decimal)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Boolean), typeof(System.Double)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Boolean), typeof(System.Single)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Boolean), typeof(System.Char)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Boolean), typeof(System.String)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Boolean), typeof(System.DateTime)), false },

            { new KeyValuePair<Type, Type>(typeof(System.Int16), typeof(System.UInt64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Int16), typeof(System.UInt32)), false },

            { new KeyValuePair<Type, Type>(typeof(System.Int32), typeof(System.UInt32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Int32), typeof(System.UInt64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Int32), typeof(System.String)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Int32), typeof(System.Char)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Int32), typeof(System.DateTime)), false },

            { new KeyValuePair<Type, Type>(typeof(System.Int64), typeof(System.Int32)), false },

            { new KeyValuePair<Type, Type>(typeof(System.UInt64), typeof(System.UInt32)), false },

            { new KeyValuePair<Type, Type>(typeof(System.DBNull), typeof(System.Double)), false },
            { new KeyValuePair<Type, Type>(typeof(System.DBNull), typeof(System.Decimal)), false },
            { new KeyValuePair<Type, Type>(typeof(System.DBNull), typeof(System.UInt64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.DBNull), typeof(System.Int64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.DBNull), typeof(System.UInt32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.DBNull), typeof(System.Int32)), false },

            { new KeyValuePair<Type, Type>(typeof(System.DateTimeOffset), typeof(System.DateTime)), false },
            { new KeyValuePair<Type, Type>(typeof(System.DateTimeOffset), typeof(System.DateTime?)), false },
            
            { new KeyValuePair<Type, Type>(typeof(System.Object), typeof(System.Single)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Object), typeof(System.Double)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Object), typeof(System.Decimal)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Object), typeof(System.UInt64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Object), typeof(System.Int64)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Object), typeof(System.UInt32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Object), typeof(System.Int32)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Object), typeof(System.String)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Object), typeof(System.DateTime)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Object), typeof(System.TimeSpan)), false },
            { new KeyValuePair<Type, Type>(typeof(System.Object), typeof(System.Char)), false },

            { new KeyValuePair<Type, Type>(typeof(StiBusinessObject), typeof(StiDataSource)), false },
            { new KeyValuePair<Type, Type>(typeof(StiDataSource), typeof(StiBusinessObject)), false }
        };

        public static bool IsImplicitlyCastableTo(Type from, Type to, object valueFrom)
        {
            if (from == to) return true;
            if (from == null || to == null) return false;

            // not strictly necessary, but speeds things up and avoids polluting the cache
            if (to.IsAssignableFrom(from))
            {
                return true;
            }

            //special check for null-to-string
            if (from == typeof(object) && to == typeof(string) && valueFrom == null) return true;

            var key = new KeyValuePair<Type, Type>(from, to);
            bool cachedValue;
            //if (ImplicitCastCache.TryGetCachedValue(key, out cachedValue))
            if (ImplicitCastCache.TryGetValue(key, out cachedValue))
            {
                return cachedValue;
            }

            //speed up IStiList
            if (typeof(IStiList).IsAssignableFrom(to))
            {
                if (typeof(IStiList).IsAssignableFrom(from))
                {
                    return from.FullName == to.FullName;
                }
                return false;
            }
            if (typeof(IStiList).IsAssignableFrom(from)) return false;

            bool result = false;
            try
            {
                GetMethod(() => AttemptImplicitCast<object, object>())
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(from, to)
                    .Invoke(null, new object[0]);
                result = true;
            }
            //catch (TargetInvocationException ex)
            //{
            //    result = !((ex.InnerException is RuntimeBinderException) && ex.InnerException.Message.Contains("System.Collections.Generic.List<"));
            //}
            catch
            {
                //for our purposes it is sufficient simply to catch any error
            }

            ImplicitCastCache[key] = result;
            return result;
        }

        private static void AttemptImplicitCast<TFrom, TTo>()
        {
            // based on the IL produced by:
            // dynamic list = new List<TTo>();
            // list.Add(default(TFrom));
            // We can't use the above code because it will mimic a cast in a generic method
            // which doesn't have the same semantics as a cast in a non-generic method

            var list = new List<TTo>(capacity: 1);
            var binder = Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(
                flags: CSharpBinderFlags.ResultDiscarded,
                name: "Add",
                typeArguments: null,
                context: typeof(StiParser),
                argumentInfo: new[] 
                { 
                    CSharpArgumentInfo.Create(flags: CSharpArgumentInfoFlags.None, name: null), 
                    CSharpArgumentInfo.Create(
                        flags: CSharpArgumentInfoFlags.UseCompileTimeType, 
                        name: null
                    ),
                }
            );
            var callSite = CallSite<Action<CallSite, object, TFrom>>.Create(binder);
            callSite.Target.Invoke(callSite, list, default(TFrom));
        }

        /// <summary>
        /// Gets a method info of a void method.
        /// Example: GetMethod(() => Console.WriteLine("")); will return the
        /// MethodInfo of WriteLine that receives a single argument.
        /// </summary>
        private static MethodInfo GetMethod(Expression<Action> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var body = expression.Body;
            if (body.NodeType != ExpressionType.Call)
                throw new ArgumentException("expression.Body must be a Call expression.", "expression");

            MethodCallExpression callExpression = (MethodCallExpression)body;
            return callExpression.Method;
        }
        #endregion
        
        #region GetTypeName
        private string GetTypeName(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else
            {
                return value.GetType().ToString();
            }
        }
        #endregion
    }
}
