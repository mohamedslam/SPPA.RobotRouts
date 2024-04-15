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
using System.Text;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Engine
{
    public partial class StiParser
    {
        #region class StiParserData
        public class StiParserData
        {
            public object Data = null;
            public List<StiAsmCommand> AsmList = null;
            public List<StiAsmCommand> AsmList2 = null;
            public List<StiAsmCommand> ConditionAsmList = null;
            public StiParser Parser = null;

            public StiParserData(object data, List<StiAsmCommand> asmList, StiParser parser)
            {
                this.Data = data;
                this.AsmList = asmList;
                this.Parser = parser;
                this.ConditionAsmList = null;
            }

            public StiParserData(object data, List<StiAsmCommand> asmList, StiParser parser, List<StiAsmCommand> conditionAsmList)
            {
                this.Data = data;
                this.AsmList = asmList;
                this.Parser = parser;
                this.ConditionAsmList = conditionAsmList;
            }
        }
        #endregion

        #region class StiFilterParserData
        public class StiFilterParserData
        {
            public StiComponent Component;
            public string Expression;

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                StiFilterParserData filter = obj as StiFilterParserData;
                if (filter == null) return false;
                return (filter.Component == Component) && (filter.Expression == Expression);
            }

            public StiFilterParserData(StiComponent component, string expression)
            {
                this.Component = component;
                this.Expression = expression;
            }
        }
        #endregion

        #region class StiToken
        public class StiToken
        {
            public StiTokenType Type = StiTokenType.Empty;
            public string Value;
            public object ValueObject;
            public int Position = -1;
            public int Length = -1;

            public StiToken(StiTokenType type, int position, int length)
            {
                this.Type = type;
                this.Position = position;
                this.Length = length;
            }
            public StiToken(StiTokenType type, int position)
            {
                this.Type = type;
                this.Position = position;
            }
            public StiToken(StiTokenType type)
            {
                this.Type = type;
            }
            public StiToken()
            {
                this.Type = StiTokenType.Empty;
            }

            public override string ToString()
            {
                return string.Format("TokenType={0}{1}", Type.ToString(), Value != null ? string.Format(", value=\"{0}\"", Value) : "");
            }
        }
        #endregion

        #region class StiAsmCommand
        public class StiAsmCommand
        {
            public StiAsmCommandType Type;
            public int ArgsCount;
            public object Parameter1;
            public object Parameter2;
            public int Position = -1;
            public int Length = -1;

            public StiAsmCommand(StiAsmCommandType type)
                : this(type, null, 0)
            {
            }

            public StiAsmCommand(StiAsmCommandType type, object parameter)
                : this(type, parameter, 0)
            {
            }

            public StiAsmCommand(StiAsmCommandType type, object parameter1, int argsCount, object parameter2 = null)
            {
                this.Type = type;
                this.Parameter1 = parameter1;
                this.ArgsCount = argsCount;
                this.Parameter2 = parameter2;
            }

            public override string ToString()
            {
                return string.Format("{0}({1},{2}{3})",
                    Type.ToString(),
                    Parameter1 != null ? Parameter1.ToString() : "null",
                    ArgsCount,
                    Parameter2 != null ? ",*" : "");
            }
        }
        #endregion

        #region class StiParserMethodInfo
        public class StiParserMethodInfo
        {
            public StiFunctionType Name;
            public int Number;
            public Type[] Arguments;
            public Type ReturnType;

            public StiParserMethodInfo(StiFunctionType name, int number, Type[] arguments)
            {
                this.Name = name;
                this.Number = number;
                this.Arguments = arguments;
                this.ReturnType = typeof(string);
            }

            public StiParserMethodInfo(StiFunctionType name, int number, Type[] arguments, Type returnType)
            {
                this.Name = name;
                this.Number = number;
                this.Arguments = arguments;
                this.ReturnType = returnType;
            }
        }
        #endregion

        #region class StiRefVariableObject
        internal class StiRefVariableObject
        {
            public string Name;
            public object Value;

            public StiRefVariableObject(string name, object value)
            {
                this.Name = name;
                this.Value = value;
            }
        }
        #endregion

        #region class StiParserDataSourceFieldInfo
        internal class StiParserDataSourceFieldInfo
        {
            public List<string> Path;
            public List<object> Objects;

            public StiParserDataSourceFieldInfo()
            {
                Path = new List<string>();
                Objects = new List<object>();
            }
        }
        #endregion

        #region class StiParserCheckMethodInfo
        internal class StiParserCheckMethodInfo
        {
            public int LastOverload = -1;
            public Type[] LastArgsTypes;

            public StiParserCheckMethodInfo(int lastOverload, Type[] lastArgsTypes)
            {
                LastOverload = lastOverload;
                LastArgsTypes = lastArgsTypes;
            }
        }
        #endregion

        #region Fields
        private StiReport report = null;
        private string inputExpression = string.Empty;
        private StiComponent component = null;
        private object sender = null;
        private bool syntaxCaseSensitive = true;
        private bool checkSyntaxMode = false;
        private StiParserGetDataFieldValueDelegate getDataFieldValue = null;
        private bool useAliases = false;
        private StiParserParameters parameters;

        private int position = 0;
        private List<StiToken> tokensList = null;
        private StiToken currentToken = null;
        private int tokenPos = 0;
        private List<StiAsmCommand> asmList = null;
        private Hashtable hashAliases = null;

        private Hashtable runtimeConstants = null;
        private Hashtable runtimeConstantsHash = null;

        private int expressionPosition = 0;

        private bool IsVB
        {
            get
            {
                return report != null && report.ScriptLanguage == StiReportLanguageType.VB;
            }
        }
        #endregion

        #region Properties
        public static bool ProcessIifAsTernary { get; set; } = false;
        #endregion

        #region Delegates
        public class StiParserGetDataFieldValueEventArgs : EventArgs
        {
            public virtual string DataSourceName { get; set; }

            public virtual string DataColumnName { get; set; }

            public virtual bool Processed { get; set; }

            public virtual object Value { get; set; }

            public virtual List<string> Path { get; set; }

            public StiParserGetDataFieldValueEventArgs(string dataSourceName, string dataColumnName, List<string> path):this(dataSourceName, dataColumnName)
            {                
                this.Path = path;
            }

            public StiParserGetDataFieldValueEventArgs(string dataSourceName, string dataColumnName)
            {
                this.DataSourceName = dataSourceName;
                this.DataColumnName = dataColumnName;
            }
        }

        public delegate void StiParserGetDataFieldValueDelegate(object sender, StiParserGetDataFieldValueEventArgs e);
        #endregion

        #region ExecuteAsm
        public object ExecuteAsm(object objectAsmList)
        {
            List<StiAsmCommand> asmList = objectAsmList as List<StiAsmCommand>;
            if (asmList == null || asmList.Count == 0) return null;
            Stack stack = new Stack();
            ArrayList argsList = null;
            object par1 = 0;
            object par2 = 0;
            for (int indexAsm = 0; indexAsm < asmList.Count; indexAsm++)
            {
                var asmCommand = asmList[indexAsm];

                switch (asmCommand.Type)
                {
                    case StiAsmCommandType.PushValue:
                        stack.Push(asmCommand.Parameter1);
                        break;
                    case StiAsmCommandType.PushVariable:
                        stack.Push(getVariableValue((string)asmCommand.Parameter1));
                        break;
                    case StiAsmCommandType.PushRefVariable:
                        stack.Push(new StiRefVariableObject((string)asmCommand.Parameter1, getVariableValue((string)asmCommand.Parameter1)));
                        break;
                    case StiAsmCommandType.PushSystemVariable:
                        stack.Push(get_systemVariable(asmCommand.Parameter1));
                        break;
                    case StiAsmCommandType.PushComponent:
                        stack.Push(asmCommand.Parameter1);
                        break;

                    case StiAsmCommandType.CopyToVariable:
                        report[(string)asmCommand.Parameter1] = stack.Peek();
                        break;

                    case StiAsmCommandType.PushFunction:
                        #region Push function value
                        argsList = new ArrayList();
                        for (int index = 0; index < asmCommand.ArgsCount; index++)
                        {
                            argsList.Add(stack.Pop());
                        }
                        if (asmCommand.ArgsCount > 1) argsList.Reverse();
                        stack.Push(call_func(asmCommand.Parameter1, argsList, asmCommand));
                        #endregion
                        break;

                    case StiAsmCommandType.PushMethod:
                        #region Push method value
                        argsList = new ArrayList();
                        for (int index = 0; index < asmCommand.ArgsCount; index++)
                        {
                            argsList.Add(stack.Pop());
                        }
                        if (asmCommand.ArgsCount > 1) argsList.Reverse();
                        stack.Push(call_method(asmCommand.Parameter1, argsList, asmCommand));
                        #endregion
                        break;

                    case StiAsmCommandType.PushProperty:
                        argsList = new ArrayList();
                        argsList.Add(stack.Pop());
                        stack.Push(call_property(asmCommand.Parameter1, argsList));
                        break;

                    case StiAsmCommandType.PushDataSourceField:
                        stack.Push(getDataSourceField(asmCommand));
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

                            StiDataColumn column = bos.Columns[parts2[indexPart]];
                            if (column != null)
                            {
                                if (column is StiCalcDataColumn)
                                {
                                    stack.Push(StiParser.ParseTextValue("{" + (column as StiCalcDataColumn).Expression + "}", component));
                                }
                                else
                                {
                                    stack.Push(bos[parts2[indexPart]]);
                                }
                            }
                            else
                            {
                                stack.Push(bos.BusinessObjects[parts2[indexPart]]);
                            }
                        }
                        else
                        {
                            stack.Push(bos);
                        }
                        #endregion
                        break;

                    case StiAsmCommandType.PushArrayElement:
                        #region Push array value
                        argsList = new ArrayList();
                        for (int index = 0; index < (int)asmCommand.Parameter1; index++)
                        {
                            argsList.Add(stack.Pop());
                        }
                        argsList.Reverse();
                        stack.Push(call_arrayElement(argsList));
                        #endregion
                        break;

                    case StiAsmCommandType.Jump:
                        indexAsm += Convert.ToInt32(asmCommand.Parameter1);
                        break;

                    case StiAsmCommandType.JumpTrue:
                        if (Convert.ToBoolean(stack.Pop()))
                        {
                            indexAsm += Convert.ToInt32(asmCommand.Parameter1);
                        }
                        break;

                    case StiAsmCommandType.JumpFalse:
                        if (!Convert.ToBoolean(stack.Pop()))
                        {
                            indexAsm += Convert.ToInt32(asmCommand.Parameter1);
                        }
                        break;
                        
                    case StiAsmCommandType.Add:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_Add(par1, par2));
                        break;
                    case StiAsmCommandType.Sub:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_Sub(par1, par2));
                        break;

                    case StiAsmCommandType.Mult:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_Mult(par1, par2));
                        break;
                    case StiAsmCommandType.Div:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_Div(par1, par2));
                        break;
                    case StiAsmCommandType.Mod:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_Mod(par1, par2));
                        break;

                    case StiAsmCommandType.Power:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_Pow(par1, par2));
                        break;

                    case StiAsmCommandType.Neg:
                        par1 = stack.Pop();
                        stack.Push(op_Neg(par1));
                        break;

                    case StiAsmCommandType.Cast:
                        par1 = stack.Pop();
                        par2 = asmCommand.Parameter1;
                        stack.Push(op_Cast(par1, par2));
                        break;

                    case StiAsmCommandType.Not:
                        par1 = stack.Pop();
                        stack.Push(op_Not(par1));
                        break;

                    case StiAsmCommandType.CompareLeft:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_CompareLeft(par1, par2));
                        break;
                    case StiAsmCommandType.CompareLeftEqual:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_CompareLeftEqual(par1, par2));
                        break;
                    case StiAsmCommandType.CompareRight:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_CompareRight(par1, par2));
                        break;
                    case StiAsmCommandType.CompareRightEqual:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_CompareRightEqual(par1, par2));
                        break;

                    case StiAsmCommandType.CompareEqual:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_CompareEqual(par1, par2));
                        break;
                    case StiAsmCommandType.CompareNotEqual:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_CompareNotEqual(par1, par2));
                        break;

                    case StiAsmCommandType.Shl:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_Shl(par1, par2));
                        break;
                    case StiAsmCommandType.Shr:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_Shr(par1, par2));
                        break;

                    case StiAsmCommandType.And:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_And(par1, par2));
                        break;
                    case StiAsmCommandType.Or:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_Or(par1, par2));
                        break;
                    case StiAsmCommandType.Xor:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_Xor(par1, par2));
                        break;

                    case StiAsmCommandType.And2:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_And2(par1, par2));
                        break;
                    case StiAsmCommandType.Or2:
                        par2 = stack.Pop();
                        par1 = stack.Pop();
                        stack.Push(op_Or2(par1, par2));
                        break;
                }
            }
            return stack.Pop();
        }

        private object getDataSourceField(StiAsmCommand asmCommand)
        {
            var fieldInfo = asmCommand.Parameter2 as StiParserDataSourceFieldInfo;
            if (fieldInfo != null)
            {
                #region Process FieldInfo
                if (getDataFieldValue != null)
                {
                    var args = new StiParserGetDataFieldValueEventArgs(fieldInfo.Path[0], fieldInfo.Path[1], fieldInfo.Path);
                    getDataFieldValue(report, args);
                    if (args.Processed)
                    {
                        return args.Value;
                    }
                }

                StiDataSource ds = fieldInfo.Objects[0] as StiDataSource;
                if (asmCommand.ArgsCount == 1)
                {
                    return ds;
                }
                if (asmCommand.ArgsCount == 2)
                {
                    if (fieldInfo.Objects[1] is StiDataRelation)
                    {
                        return (fieldInfo.Objects[1] as StiDataRelation).ParentSource;
                    }
                    StiDataColumn column = fieldInfo.Objects[1] as StiDataColumn;
                    if (column != null && column is StiCalcDataColumn)
                    {
                        return StiParser.ParseTextValue("{" + (column as StiCalcDataColumn).Expression + "}", component);
                    }
                    else
                    {
                        return StiReport.ChangeType(ds.GetData(fieldInfo.Path[1]), column.Type, report.ConvertNulls);
                    }
                }
                else
                {
                    string nameInSource = fieldInfo.Path[1];
                    StiDataRow row = ds.GetParentData(nameInSource);
                    //ds = (fieldInfo.Objects[1] as StiDataRelation).ParentSource;
                    int indexPart = 2;
                    while (indexPart < asmCommand.ArgsCount - 1)
                    {
                        nameInSource = fieldInfo.Path[indexPart];
                        row = row.GetParentData(nameInSource);
                        //ds = (fieldInfo.Objects[indexPart] as StiDataRelation).ParentSource;
                        indexPart++;
                    }

                    if (fieldInfo.Objects[indexPart] is StiDataRelation)
                    {
                        return (fieldInfo.Objects[indexPart] as StiDataRelation).ParentSource;
                    }
                    StiDataColumn column = fieldInfo.Objects[indexPart] as StiDataColumn;
                    if (column != null && column is StiCalcDataColumn)
                    {
                        return StiParser.ParseTextValue("{" + (column as StiCalcDataColumn).Expression + "}", component);
                    }
                    else
                    {
                        object columnValue = null;
                        if (row != null)
                        {
                            columnValue = row[fieldInfo.Path[indexPart]];
                        }
                        return StiReport.ChangeType(columnValue, column.Type, report.ConvertNulls);
                    }
                }
                #endregion
            }
            else
            {
                #region Process path parts
                //this part for back compatibility
                List<string> parts = asmCommand.Parameter2 as List<string>;
                if (parts == null)
                {
                    parts = new List<string>(((string)asmCommand.Parameter1).Split(new char[] { '.' }));
                }

                if (getDataFieldValue != null)
                {
                    var args = new StiParserGetDataFieldValueEventArgs(parts[0], parts[1], parts);
                    getDataFieldValue(report, args);
                    if (args.Processed)
                    {
                        return args.Value;
                    }
                }

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
                if (parts.Count == 1)
                {
                    return ds;
                }
                if (parts.Count == 2)
                {
                    StiDataColumn column = ds.Columns[parts[1]];
                    if (column == null)
                    {
                        var ds2 = ds.GetParentDataSource(parts[1]);
                        if (ds2 != null)
                        {
                            return ds2;
                        }
                    }
                    if (column != null && column is StiCalcDataColumn)
                    {
                        return StiParser.ParseTextValue("{" + (column as StiCalcDataColumn).Expression + "}", component);
                    }
                    else
                    {
                        return StiReport.ChangeType(ds.GetData(parts[1]), column.Type, report.ConvertNulls);
                    }
                }
                else
                {
                    string nameInSource = parts[1];
                    StiDataRow row = ds.GetParentData(nameInSource);
                    ds = ds.GetParentDataSource(nameInSource);
                    int indexPart = 2;
                    while (indexPart < parts.Count - 1)
                    {
                        nameInSource = parts[indexPart];
                        row = row.GetParentData(nameInSource);
                        ds = ds.GetParentDataSource(nameInSource);
                        indexPart++;
                    }
                    StiDataColumn column = ds.Columns[parts[indexPart]];
                    if (column == null)
                    {
                        var ds2 = ds.GetParentDataSource(parts[indexPart]);
                        if (ds2 != null)
                        {
                            return ds2;
                        }
                    }
                    if (column != null && column is StiCalcDataColumn)
                    {
                        return StiParser.ParseTextValue("{" + (column as StiCalcDataColumn).Expression + "}", component);
                    }
                    else
                    {
                        object columnValue = null;
                        if (row != null)
                        {
                            columnValue = row[parts[indexPart]];
                        }
                        return StiReport.ChangeType(columnValue, column.Type, report.ConvertNulls);
                    }
                }
                #endregion
            }
        }

        private object getVariableValue(string name)
        {
            if (runtimeConstants != null && runtimeConstants.Count > 0)
            {
                if (runtimeConstantsHash.ContainsKey(name)) return runtimeConstantsHash[name];
            }

            StiReport tempReport = report.CompiledReport == null ? report : report.CompiledReport;
            FieldInfo field = tempReport.GetType().GetField(name);
            if (field != null)
            {
                return field.GetValue(tempReport);
            }
            else
            {
                StiVariable var = tempReport.Dictionary.Variables[name];
                if (var != null && var.ReadOnly && var.InitBy == StiVariableInitBy.Expression)
                {
                    if (CheckVariableRecursion(name)) return null;
                    StiText tempText = new StiText();
                    tempText.Name = "**ReportVariables**";
                    tempText.Page = report.Pages[0];
                    var result = op_Cast(StiParser.ParseTextValue("{" + var.Value + "}", tempText, parameters), var.Type);
                    RemoveRecursionCheck(name);
                    return result;
                }
                if (tempReport.Variables != null && tempReport.Variables.ContainsKey(name))
                {
                    return tempReport.Variables[name];
                }
                if (var != null)
                {
                    if (var.InitBy == StiVariableInitBy.Expression)
                    {
                        if (CheckVariableRecursion(name)) return null;
                        var result = op_Cast(PrepareVariableValue(var, tempReport, null, false, parameters), var.Type);
                        RemoveRecursionCheck(name);
                        return result;
                    }
                    return var.ValueObject;
                }
                return null;
            }
        }

        private bool CheckVariableRecursion(string varName)
        {
            if (parameters.VariablesRecursionCheck == null)
            {
                parameters.VariablesRecursionCheck = new Hashtable();
            }
            if (parameters.VariablesRecursionCheck.ContainsKey(varName)) return true;
            parameters.VariablesRecursionCheck[varName] = null;
            return false;
        }
        private void RemoveRecursionCheck(string varName)
        {
            parameters.VariablesRecursionCheck.Remove(varName);
        }
        #endregion

        #region Calls

        private int get_category(object par)
        {
            if (par == null) return -1;
            Type type = par.GetType();
            if (type == typeof(string) || type == typeof(char)) return 1;
            if (type == typeof(decimal)) return 2;
            if (type == typeof(double) || type == typeof(float)) return 3;
            if (type == typeof(ulong)) return 4;
            if (type == typeof(long)) return 5;
            if (type == typeof(uint) || type == typeof(ushort) || type == typeof(byte)) return 6;
            if (type == typeof(int) || type == typeof(short) || type == typeof(sbyte)) return 7;
            if (type == typeof(DateTime)) return 8;
            if (type == typeof(bool)) return 9;
            if (type == typeof(DateTimeOffset)) return 10;
            if (type == typeof(object)) return 0;

            if (IsImplicitlyCastableTo(type, typeof(decimal), par)) return 2;
            if (IsImplicitlyCastableTo(type, typeof(double), par)) return 3;
            if (IsImplicitlyCastableTo(type, typeof(ulong), par)) return 4;
            if (IsImplicitlyCastableTo(type, typeof(long), par)) return 5;
            if (IsImplicitlyCastableTo(type, typeof(uint), par)) return 6;
            if (IsImplicitlyCastableTo(type, typeof(int), par)) return 7;

            return 0;
        }

        #region CheckParserMethodInfo
        private int CheckParserMethodInfo(StiFunctionType type, ArrayList args)
        {
            int count = args.Count;
            Type[] types = new Type[count];
            object[] values = new object[count];
            for (int index = 0; index < count; index++)
            {
                if (args[index] == null)
                {
                    types[index] = typeof(object);
                }
                else
                {
                    types[index] = args[index].GetType();
                }
                values[index] = args[index];
            }

            StiParserMethodInfo methodInfo = GetParserMethodInfo(type, types, values);

            if (methodInfo != null) return methodInfo.Number;

            return 0;
        }

        private int CheckParserMethodInfo2(StiFunctionType type, ArrayList args, StiAsmCommand asmCommand)
        {
            int count = args.Count;

            //prepare types array
            Type[] types = new Type[count];
            object[] values = new object[count];
            for (int index = 0; index < count; index++)
            {
                if (args[index] == null)
                {
                    types[index] = typeof(object);
                }
                else
                {
                    types[index] = args[index].GetType();
                }
                values[index] = args[index];
            }

            var checkMethodInfo = asmCommand.Parameter2 as StiParserCheckMethodInfo;
            if (checkMethodInfo != null)
            {
                bool flag = checkMethodInfo.LastArgsTypes.Length == count;
                if (flag)
                {
                    for (int index = count - 1; index >= 0; index--)
                    {
                        if (types[index] != checkMethodInfo.LastArgsTypes[index])
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    return checkMethodInfo.LastOverload;
                }
                StiParserMethodInfo methodInfo2 = GetParserMethodInfo(type, types, values);
                checkMethodInfo.LastOverload = (methodInfo2 != null) ? methodInfo2.Number : 0;
                checkMethodInfo.LastArgsTypes = types;
                return checkMethodInfo.LastOverload;
            }
            StiParserMethodInfo methodInfo = GetParserMethodInfo(type, types, values);
            int overload = (methodInfo != null) ? methodInfo.Number : 0;
            asmCommand.Parameter2 = new StiParserCheckMethodInfo(overload, types);
            return overload;
        }

        public StiParserMethodInfo GetParserMethodInfo(StiFunctionType type, Type[] args, object[] argValues = null)
        {
            object obj = MethodsHash[type];
            if (obj == null) return null;
            int count = args.Length;
            if (argValues == null) argValues = new object[args.Length];

            List<StiParserMethodInfo> methods = (List<StiParserMethodInfo>)obj;
            bool flag1 = false;
            foreach (StiParserMethodInfo methodInfo in methods)
            {
                if (methodInfo.Arguments.Length != count) continue;
                flag1 = true;
                bool flag2 = true;
                for (int index = 0; index < count; index++)
                {
                    if (IsImplicitlyCastableTo(args[index], methodInfo.Arguments[index], argValues[index])) continue;
                    flag2 = false;
                    break;
                }
                if (flag2) return methodInfo;
            }

            if (!flag1)
            {
                ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, Enum.GetName(typeof(StiFunctionType), type), count.ToString());
            }

            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < count; index++)
            {
                sb.Append((!(args[index] is Stimulsoft.Base.StiUndefinedType) && args[index].Namespace == "System") ? args[index].Name : args[index].ToString());
                if (index < count - 1) sb.Append(",");
            }

            ThrowError(ParserErrorCode.NoMatchingOverloadedMethod, Enum.GetName(typeof(StiFunctionType), type), sb.ToString());
            return null;
        }
        #endregion
 
        //----------------------------------------
        // Получение элемента массива
        //----------------------------------------
        private object call_arrayElement(ArrayList argsList)
        {
            object baseValue = argsList[0];

            if (argsList.Count < 2) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "get_ArrayElement", (argsList.Count - 1).ToString());

            if (baseValue is String)
            {
                if (argsList.Count != 2) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "string.get_Item", (argsList.Count - 1).ToString());
                int index = Convert.ToInt32(argsList[1]);
                return (baseValue as string)[index];
            }

            PropertyInfo pi = baseValue.GetType().GetProperty("Item");
            if (pi != null)
            {
                if (pi.GetGetMethod().GetParameters().Length > 0)
                {
                    if (argsList.Count < 2) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "object.get_Item", (argsList.Count - 1).ToString());
                    object[] args = new object[argsList.Count - 1];
                    for (int index = 0; index < argsList.Count - 1; index++)
                    {
                        args[index] = argsList[index + 1];
                    }
                    return pi.GetValue(baseValue, args);
                }
            }
            else if (baseValue is Array)
            {
                int[] args = new int[argsList.Count - 1];
                for (int index = 0; index < argsList.Count - 1; index++)
                {
                    args[index] = Convert.ToInt32(argsList[index + 1]);
                }
                return (baseValue as Array).GetValue(args);
            }
            else if (baseValue is IList)
            {
                return (baseValue as IList)[Convert.ToInt32(argsList[1])];     //only firts dimension, temporarily
            }

            return null;
        }

        //----------------------------------------
        // Получение системной переменной
        //----------------------------------------
        private object get_systemVariable(object name)
        {
            switch ((StiSystemVariableType)name)
            {
                case StiSystemVariableType.Column:              return report.Column;
                case StiSystemVariableType.Line:                return report.Line;
                case StiSystemVariableType.LineThrough:         return report.LineThrough;
                case StiSystemVariableType.LineABC:             return report.LineABC;
                case StiSystemVariableType.LineRoman:           return report.LineRoman;
                case StiSystemVariableType.GroupLine:           return report.GroupLine;
                case StiSystemVariableType.PageNumber:          return report.PageNumber;
                case StiSystemVariableType.PageNumberThrough:   return report.PageNumberThrough;
                case StiSystemVariableType.PageNofM:            return report.PageNofM;
                case StiSystemVariableType.PageNofMThrough:     return report.PageNofMThrough;
                case StiSystemVariableType.TotalPageCount:      return report.TotalPageCount;
                case StiSystemVariableType.TotalPageCountThrough: return report.TotalPageCountThrough;
                case StiSystemVariableType.IsFirstPage:         return report.IsFirstPage;
                case StiSystemVariableType.IsFirstPageThrough:  return report.IsFirstPageThrough;
                case StiSystemVariableType.IsLastPage:          return report.IsLastPage;
                case StiSystemVariableType.IsLastPageThrough:   return report.IsLastPageThrough;
                case StiSystemVariableType.PageCopyNumber:      return report.PageCopyNumber;
                case StiSystemVariableType.ReportAlias:         return report.ReportAlias;
                case StiSystemVariableType.ReportAuthor:        return report.ReportAuthor;
                case StiSystemVariableType.ReportChanged:       return report.ReportChanged;
                case StiSystemVariableType.ReportCreated:       return report.ReportCreated;
                case StiSystemVariableType.ReportDescription:   return report.ReportDescription;
                case StiSystemVariableType.ReportName:          return report.ReportName;
                case StiSystemVariableType.Time:                return report.Time;
                case StiSystemVariableType.Today:               return report.Today;
                case StiSystemVariableType.ConditionValue:      return report.Engine?.LastInvokeTextProcessValueEventArgsValue; 
                case StiSystemVariableType.ConditionValue2:     return report.Engine?.LastInvokeTextProcessValueEventArgsValue;
                case StiSystemVariableType.ConditionTag:        return (component is StiText) ? (component as StiText).TagValue : null;
                case StiSystemVariableType.Index:               return report.Engine?.LastInvokeTextProcessIndexEventArgsValue;
                case StiSystemVariableType.Sender:              return sender;

                case StiSystemVariableType.DateTimeNow:         return DateTime.Now;
                case StiSystemVariableType.DateTimeToday:       return DateTime.Today;
                case StiSystemVariableType.DateTimeUtcNow:      return DateTime.UtcNow;
            }
            return null;
        }

        #endregion

        #region ParseTextValue
        public static object ParseTextValue(string inputExpression, StiComponent component)
        {
            return ParseTextValue(inputExpression, component, component, new StiParserParameters());
        }
        public static object ParseTextValue(string inputExpression, StiComponent component, ref bool storeToPrint, bool executeIfStoreToPrint)
        {
            var parameters = new StiParserParameters() { StoreToPrint = storeToPrint, ExecuteIfStoreToPrint = executeIfStoreToPrint };
            object result = ParseTextValue(inputExpression, component, component, parameters);
            storeToPrint = parameters.StoreToPrint;
            return result;
        }
        public static object ParseTextValue(string inputExpression, StiComponent component, ref bool storeToPrint, bool executeIfStoreToPrint, bool returnAsmList)
        {
            var parameters = new StiParserParameters() { StoreToPrint = storeToPrint, ExecuteIfStoreToPrint = executeIfStoreToPrint, ReturnAsmList = returnAsmList };
            object result = ParseTextValue(inputExpression, component, component, parameters);
            storeToPrint = parameters.StoreToPrint;
            return result;
        }

        public static object ParseTextValue(string inputExpression, StiComponent component, object sender)
        {
            return ParseTextValue(inputExpression, component, sender, new StiParserParameters());
        }
        public static object ParseTextValue(string inputExpression, StiComponent component, object sender, ref bool storeToPrint, bool executeIfStoreToPrint)
        {
            var parameters = new StiParserParameters() { StoreToPrint = storeToPrint, ExecuteIfStoreToPrint = executeIfStoreToPrint };
            object result = ParseTextValue(inputExpression, component, sender, parameters);
            storeToPrint = parameters.StoreToPrint;
            return result;
        }
        public static object ParseTextValue(string inputExpression, StiComponent component, object sender, ref bool storeToPrint, bool executeIfStoreToPrint, bool returnAsmList)
        {
            var parameters = new StiParserParameters() { StoreToPrint = storeToPrint, ExecuteIfStoreToPrint = executeIfStoreToPrint, ReturnAsmList = returnAsmList };
            object result = ParseTextValue(inputExpression, component, sender, parameters);
            storeToPrint = parameters.StoreToPrint;
            return result;
        }

        public static object ParseTextValue(string inputExpression, StiComponent component, StiParserParameters parameters)
        {
            if (parameters == null) parameters = new StiParserParameters();
            return ParseTextValue(inputExpression, component, component, parameters);
        }

        public static object ParseTextValue(string inputExpression, StiComponent component, object sender, StiParserParameters parameters)
        {
            if (string.IsNullOrEmpty(inputExpression)) return null;

            if (parameters.Parser == null)
            {
                parameters.Parser = new StiParser();
            }
            if (component.Report != null) parameters.Parser.report = component.Report;
            parameters.Parser.component = component;
            parameters.Parser.sender = sender;

            parameters.Parser.syntaxCaseSensitive = parameters.SyntaxCaseSensitive ?? 
                parameters.Parser.report.ScriptLanguage == StiReportLanguageType.CSharp || parameters.Parser.report.ScriptLanguage == StiReportLanguageType.JS;

            parameters.Parser.checkSyntaxMode = parameters.CheckSyntaxMode;
            parameters.Parser.getDataFieldValue = parameters.GetDataFieldValue;
            parameters.Parser.useAliases = parameters.UseAliases;
            parameters.Parser.runtimeConstants = parameters.Constants;
            if (parameters.Constants != null) parameters.Parser.CreateRuntimeConstantsHash();

            parameters.Parser.parameters = parameters;  //loop, but simple :) 

            List<StiAsmCommand> list = null;
            string expressionId = inputExpression + component.Name + parameters.GlobalizedNameExt;

            Hashtable conversionStore = parameters.ConversionStore;
            if (conversionStore == null)
            {
                StiEngine engine = parameters.Parser.report?.Engine;
                if (engine != null)
                {
                    if (engine.ParserConversionStore == null)
                        engine.ParserConversionStore = new Hashtable();

                    conversionStore = engine.ParserConversionStore;
                }
                else
                {
                    conversionStore = new Hashtable();
                }
            }

            if (conversionStore.Contains(expressionId))
            {
                list = (List<StiAsmCommand>)conversionStore[expressionId];
            }
            if (list == null)
            {
                #region Check GlobalizedName
                try
                {
                    if (!parameters.IgnoreGlobalizedName && parameters.Parser.report?.GlobalizationManager != null)
                    {
                        var iGlobalizedName = component as IStiGlobalizedName;
                        if ((iGlobalizedName != null) && (!string.IsNullOrWhiteSpace(iGlobalizedName.GlobalizedName)))
                        {
                            string text = component.Report.GlobalizationManager.GetString(iGlobalizedName.GlobalizedName + parameters.GlobalizedNameExt);
                            if (text != null)
                            {
                                inputExpression = text;
                            }
                        }
                    }
                }
                catch
                {
                }
                #endregion

                if ((component is StiText) && (component as StiText).OnlyText)
                {
                    list = new List<StiAsmCommand>();
                    list.Add(new StiAsmCommand(StiAsmCommandType.PushValue, inputExpression));
                }
                else try
                {
                    list = new List<StiAsmCommand>();
                    int counter = 0;
                    int pos = 0;
                    while (pos < inputExpression.Length)
                    {
                        #region Plain text
                        int posBegin = pos;
                        while (pos < inputExpression.Length && inputExpression[pos] != '{') pos++;
                        if (pos != posBegin)
                        {
                            if (counter == 1)
                            {
                                list.Add(new StiAsmCommand(StiAsmCommandType.Cast, TypeCode.String));
                            }
                            list.Add(new StiAsmCommand(StiAsmCommandType.PushValue, inputExpression.Substring(posBegin, pos - posBegin)));
                            counter++;
                            if (counter > 1) list.Add(new StiAsmCommand(StiAsmCommandType.Add));
                        }
                        #endregion

                        #region Expression
                        if (pos < inputExpression.Length && inputExpression[pos] == '{')
                        {
                            pos++;
                            posBegin = pos;
                            bool flag = false;
                            while (pos < inputExpression.Length)
                            {
                                if (inputExpression[pos] == '"')
                                {
                                    pos++;
                                    int pos2 = pos;
                                    while (pos2 < inputExpression.Length)
                                    {
                                        if (inputExpression[pos2] == '"') break;
                                        if (inputExpression[pos2] == '\\') pos2++;
                                        pos2++;
                                    }
                                    pos = pos2 + 1;
                                    continue;
                                }
                                if (inputExpression[pos] == '}')
                                {
                                    string currentExpression = inputExpression.Substring(posBegin, pos - posBegin);
                                    if (currentExpression != null && currentExpression.Length > 0)
                                    {
                                        parameters.Parser.expressionPosition = posBegin;
                                        list.AddRange(parameters.Parser.ParseToAsm(currentExpression));
                                        counter++;
                                        if (counter > 1)
                                        {
                                            list.Add(new StiAsmCommand(StiAsmCommandType.Cast, TypeCode.String));
                                            list.Add(new StiAsmCommand(StiAsmCommandType.Add));
                                        }
                                    }
                                    flag = true;
                                    pos++;
                                    break;
                                }
                                pos++;
                            }
                            if (!flag)
                            {
                                parameters.Parser.expressionPosition = posBegin;
                                list.Add(new StiAsmCommand(StiAsmCommandType.PushValue, inputExpression.Substring(posBegin - 1)));
                                counter++;
                                if (counter > 1) list.Add(new StiAsmCommand(StiAsmCommandType.Add));
                            }
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    conversionStore[expressionId] = new List<StiAsmCommand>();
                    throw ex;
                }
                conversionStore[expressionId] = list;
            }

            if (parameters.ReturnAsmList) return list;
            if (list.Count > 0)
            {
                parameters.StoreToPrint = CheckForStoreToPrint(list, component);
                if (parameters.StoreToPrint && !parameters.ExecuteIfStoreToPrint) return inputExpression;
                return parameters.Parser.ExecuteAsm(list);
            }
            return null;
        }

        private List<StiAsmCommand> ParseToAsm(string inputExpression)
        {
            this.inputExpression = inputExpression;
            MakeTokensList();
            asmList = new List<StiAsmCommand>();
            eval_exp();
            return asmList;
        }

        private static bool CheckForStoreToPrint(object objAsmList, StiComponent component)
        {
            var stiText = component as StiText;
            if (stiText != null)
            {
                if (stiText.ProcessAt == StiProcessAt.EndOfReport) return true;
            }

            bool result = false;
            List<StiAsmCommand> asmList = objAsmList as List<StiAsmCommand>;
            if (asmList != null)
            {
                foreach (StiAsmCommand command in asmList)
                {
                    if (command.Type == StiAsmCommandType.PushSystemVariable)
                    {
                        StiSystemVariableType type = (StiSystemVariableType)command.Parameter1;
                        if (type == StiSystemVariableType.PageNumber ||
                            type == StiSystemVariableType.PageNumberThrough ||
                            type == StiSystemVariableType.TotalPageCount ||
                            type == StiSystemVariableType.TotalPageCountThrough ||
                            type == StiSystemVariableType.PageNofM ||
                            type == StiSystemVariableType.PageNofMThrough ||
                            type == StiSystemVariableType.IsFirstPage ||
                            type == StiSystemVariableType.IsFirstPageThrough ||
                            type == StiSystemVariableType.IsLastPage ||
                            type == StiSystemVariableType.IsLastPageThrough)
                        {
                            result = true;
                            break;
                        }
                    }
                    if (command.Type == StiAsmCommandType.PushFunction)
                    {
                        StiFunctionType type = (StiFunctionType)command.Parameter1;
                        if (type >= StiFunctionType.pCount && type <= StiFunctionType.pLast ||
                            type >= StiFunctionType.prCount && type <= StiFunctionType.prLast ||
                            type >= StiFunctionType.piCount && type <= StiFunctionType.piLast ||
                            type >= StiFunctionType.priCount && type <= StiFunctionType.priLast ||
                            type == StiFunctionType.GetAnchorPageNumber || type == StiFunctionType.GetAnchorPageNumberThrough)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        #region CheckExpression
        public static StiParserException CheckExpression(string inputExpression, StiComponent component, bool useAliases = false)
        {
            try
            {
                var parameters = new StiParserParameters() { ExecuteIfStoreToPrint = false, ReturnAsmList = true, CheckSyntaxMode = true, UseAliases = useAliases };
                object result = ParseTextValue(inputExpression, component, parameters);
                List<StiAsmCommand> list = result as List<StiAsmCommand>;
                if (list != null) parameters.Parser.CheckTypes(list);
            }
            catch (StiParserException pex)
            {
                return pex;
            }
            return null;
        }
        #endregion

        #region Check for DataBandsUsedInPageTotals
        internal static void CheckForDataBandsUsedInPageTotals(StiText stiText, StiReport report = null)
        {
            try
            {
                var parameters = new StiParserParameters() { ReturnAsmList = true, Parser = new StiParser() };
                if (parameters.Parser.report == null) parameters.Parser.report = report;
                object result = ParseTextValue(stiText.Text.Value, stiText, stiText, parameters);
            }
            catch (Exception ex)
            {
                string str = string.Format("Expression in Text property of '{0}' can't be evaluated! {1}", stiText.Name, ex.Message);
                StiLogService.Write(stiText.GetType(), str);
                StiLogService.Write(stiText.GetType(), ex.Message);
                stiText.Report.WriteToReportRenderingMessages(str);
            }
        }
        #endregion

        #region Prepare variable value
        public static void PrepareReportVariables(StiReport report)
        {
            report.Variables = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

            var tempText = new StiText
            {
                Name = "**ReportVariables**",
                Page = report.Pages[0]
            };
            foreach (StiVariable var in report.Dictionary.Variables)
            {
                //Ignore all parser error for report variables
                try
                {
                    PrepareVariableValue(var, report, tempText, false, null, false);
                }
                catch
                {
                }
            }
            report.ModifiedVariables.Clear();
        }

        public static object PrepareVariableValue(StiVariable var, StiReport report, StiText textBox = null, bool fillItems = false, StiParserParameters parameters = null, bool processReadOnly = true)
        {
            if (!processReadOnly && var.ReadOnly && var.InitBy == StiVariableInitBy.Expression) return null;

            if (textBox == null)
            {
                textBox = new StiText
                {
                    Name = "**ReportVariables**",
                    Page = report.Pages[0]
                };
            }

            //fix, in compilation mode this is not serialized automaticaly
            if (var.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Columns)
            {
                var.DialogInfo.Keys = new string[0];
                var.DialogInfo.Values = new string[0];
                var.DialogInfo.CheckedStates = new bool[0];
                var.DialogInfo.ValuesBindingList = new List<object>[0];
            }

            Hashtable hashDataSources = null;
            if (fillItems)
            {
                hashDataSources = new Hashtable();
            }

            object obj = null;

            if (var.Type == null || var.Type.IsValueType || var.Type == typeof(string))
            {
                if (var.InitBy == StiVariableInitBy.Value)
                {
                    obj = var.ValueObject;
                    if ((var.Type == typeof(DateTime)) && (obj == null)) obj = DateTime.Now;
                    if ((var.Type == typeof(DateTimeOffset)) && (obj == null)) obj = DateTimeOffset.Now;
                }
                else
                {
                    obj = GetExpressionValue("{" + var.Value + "}", textBox, hashDataSources, report, parameters);
                }
            }
            else
            {
                if (var.Type == typeof(Metafile) || var.Type == typeof(Bitmap) || var.Type == typeof(Image) || var.Type == typeof(byte[]))
                {
                    obj = var.ValueObject;
                }
                else
                {
                    obj = Activator.CreateInstance(var.Type);
                }
                if (obj is Range)
                {
                    var range = obj as Range;
                    if (var.InitBy == StiVariableInitBy.Value)
                    {
                        range.FromObject = (var.ValueObject as Range).FromObject;
                        range.ToObject = (var.ValueObject as Range).ToObject;
                    }
                    else
                    {
                        range.FromObject = GetExpressionValue("{" + var.InitByExpressionFrom + "}", textBox, hashDataSources, report, parameters);
                        range.ToObject = GetExpressionValue("{" + var.InitByExpressionTo + "}", textBox, hashDataSources, report, parameters);

                        if (range.FromObject is DateTime)
                            range.FromObject = ((DateTime)range.FromObject).Date;

                        if (range.ToObject is DateTime)
                            range.ToObject = ((DateTime)range.ToObject).Date.AddDays(1).AddTicks(-1);
                    }
                }

                else if (obj is IStiList objList)
                {
                    var items = var.DialogInfo.GetDialogInfoItems(var.Type);
                    items = var.DialogInfo.OrderBy(items);

                    if (items != null && items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            try
                            {
                                if (item.Checked)
                                    objList.AddElement(item.KeyObject);
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                if (fillItems)
                {
                    bool tempBool = false;
                    StiVariableHelper.FillItemsOfVariable(var, report, ref tempBool);
                }
            }

            if (fillItems)
            {
                foreach (DictionaryEntry de in hashDataSources)
                {
                    var dataSource = report.Dictionary.DataSources[(string)de.Key];
                    StiDataLeader.Disconnect(dataSource);
                }
            }

            //store calculated value in variables hash
            report[var.Name] = obj;

            return obj;
        }

        private static object GetExpressionValue(string expr, StiComponent comp, Hashtable hash, StiReport report, StiParserParameters parameters)
        {
            if (hash != null)
            {
                Hashtable hash2 = new Hashtable();
                StiDataSourceHelper.CheckExpression(expr, comp, hash2);
                foreach (DictionaryEntry de in hash2)
                {
                    object obj = hash[de.Key];
                    if (obj == null)
                    {
                        hash[de.Key] = true;

                        var dataSource = report.Dictionary.DataSources[(string)de.Key];
                        StiDataLeader.Connect(dataSource);
                    }
                }
            }
            if (parameters != null) return StiParser.ParseTextValue(expr, comp, parameters);
            return StiParser.ParseTextValue(expr, comp);
        }
        #endregion


    }
}
