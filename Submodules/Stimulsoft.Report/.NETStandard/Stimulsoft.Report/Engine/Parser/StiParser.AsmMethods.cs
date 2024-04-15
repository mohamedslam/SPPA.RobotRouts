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
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report.Engine
{
    public partial class StiParser
    {
        #region Evaluate the value of the method.
        private object call_method(object name, ArrayList argsList, StiAsmCommand asmCommand)
        {
            int category;
            object baseValue = argsList[0];

            int overload = CheckParserMethodInfo2((StiFunctionType)name, argsList, asmCommand);

            #region Global method ToString
            switch ((StiMethodType)name)
            {
                case StiMethodType.ToString:
                    category = get_category(argsList[0]);
                    if (category == 1)
                    {
                        return Convert.ToString(argsList[0]);
                    }
                    else if (category == 2 || category == 3)
                    {
                        decimal resDecimal = Convert.ToDecimal(argsList[0]);
                        if (argsList.Count == 1) return resDecimal.ToString();
                        else return resDecimal.ToString(Convert.ToString(argsList[1]));
                    }
                    else if (category == 4 || category == 6)
                    {
                        ulong resUlong = Convert.ToUInt64(argsList[0]);
                        if (argsList.Count == 1) return resUlong.ToString();
                        else return resUlong.ToString(Convert.ToString(argsList[1]));
                    }
                    else if (category == 5 || category == 7)
                    {
                        long resLong = Convert.ToInt64(argsList[0]);
                        if (argsList.Count == 1) return resLong.ToString();
                        else return resLong.ToString(Convert.ToString(argsList[1]));
                    }
                    else if (category == 8)
                    {
                        DateTime resDate = Convert.ToDateTime(argsList[0]);
                        if (argsList.Count == 1) return resDate.ToString();
                        else return resDate.ToString(Convert.ToString(argsList[1]));
                    }
                    else if (category == 9)
                    {
                        return Convert.ToBoolean(argsList[0]).ToString();
                    }
                    else if (baseValue is TimeSpan)
                    {
                        TimeSpan resTime = (TimeSpan)argsList[0];
                        if (argsList.Count == 1) return resTime.ToString();
                        else return resTime.ToString(Convert.ToString(argsList[1]));
                    }
                    else if (baseValue == null)
                        return string.Empty;
                    else
                        return argsList[0].ToString();
            }
            #endregion

            #region type String
            if (baseValue is string)
            {
                switch ((StiMethodType)name)
                {
                    case StiMethodType.Substring:
                        if (overload == 1) return ((string)argsList[0]).Substring((int)argsList[1]);
                        if (overload == 2) return ((string)argsList[0]).Substring((int)argsList[1], (int)argsList[2]);
                        break;

                    case StiMethodType.ToLower:
                        if (overload == 1) return ((string)argsList[0]).ToLowerInvariant();
                        break;

                    case StiMethodType.ToUpper:
                        if (overload == 1) return ((string)argsList[0]).ToUpperInvariant();
                        break;

                    case StiMethodType.IndexOf:
                        if (overload == 1) return ((string)argsList[0]).IndexOf((string)argsList[1]);
                        break;

                    case StiMethodType.StartsWith:
                        if (overload == 1) return ((string)argsList[0]).StartsWith((string)argsList[1]);
                        break;

                    case StiMethodType.EndsWith:
                        if (overload == 1) return ((string)argsList[0]).EndsWith((string)argsList[1]);
                        break;

                    case StiMethodType.Contains:
                        if (overload == 1) return ((string)argsList[0]).Contains(Convert.ToString(argsList[1]));
                        break;

                    case StiMethodType.Replace:
                        if (overload == 1) return ((string)argsList[0]).Replace((char)argsList[1], (char)argsList[2]);
                        if (overload == 2) return ((string)argsList[0]).Replace((string)argsList[1], (string)argsList[2]);
                        break;

                    case StiMethodType.PadLeft:
                        if (overload == 1) return ((string)argsList[0]).PadLeft((int)argsList[1]);
                        if (overload == 2) return ((string)argsList[0]).PadLeft((int)argsList[1], (char)argsList[2]);
                        break;
                    case StiMethodType.PadRight:
                        if (overload == 1) return ((string)argsList[0]).PadRight((int)argsList[1]);
                        if (overload == 2) return ((string)argsList[0]).PadRight((int)argsList[1], (char)argsList[2]);
                        break;

                    case StiMethodType.TrimStart:
                        if (overload == 1) return ((string)argsList[0]).TrimStart();
                        break;
                    case StiMethodType.TrimEnd:
                        if (overload == 1) return ((string)argsList[0]).TrimEnd();
                        break;
                }

            }
            #endregion

            #region type List
            if (baseValue is List<bool> ||
                baseValue is List<char> ||
                baseValue is List<DateTime> ||
                baseValue is List<TimeSpan> ||
                baseValue is List<decimal> ||
                baseValue is List<float> ||
                baseValue is List<double> ||
                baseValue is List<byte> ||
                baseValue is List<short> ||
                baseValue is List<int> ||
                baseValue is List<long> ||
                baseValue is List<Guid> ||
                baseValue is List<string>)
            {
                switch ((StiMethodType)name)
                {
                    case StiMethodType.Contains:
                        if (overload == 210) return ((List<bool>)argsList[0]).Contains(Convert.ToBoolean(argsList[1]));
                        if (overload == 220) return ((List<char>)argsList[0]).Contains(Convert.ToChar(argsList[1]));
                        if (overload == 230) return ((List<DateTime>)argsList[0]).Contains(Convert.ToDateTime(argsList[1]));
                        if (overload == 231) return ((DateTimeList)argsList[0]).Contains(Convert.ToString(argsList[1]));
                        if (overload == 240) return ((List<TimeSpan>)argsList[0]).Contains((TimeSpan)argsList[1]);
                        if (overload == 241) return ((TimeSpanList)argsList[0]).Contains(Convert.ToString(argsList[1]));
                        if (overload == 250) return ((List<decimal>)argsList[0]).Contains(Convert.ToDecimal(argsList[1]));
                        if (overload == 251) return ((DecimalList)argsList[0]).Contains(Convert.ToInt64(argsList[1]));
                        if (overload == 252) return ((DecimalList)argsList[0]).Contains(Convert.ToString(argsList[1]));
                        if (overload == 260) return ((List<float>)argsList[0]).Contains(Convert.ToSingle(argsList[1]));
                        if (overload == 261) return ((FloatList)argsList[0]).Contains(Convert.ToInt64(argsList[1]));
                        if (overload == 262) return ((FloatList)argsList[0]).Contains(Convert.ToString(argsList[1]));
                        if (overload == 270) return ((List<double>)argsList[0]).Contains(Convert.ToDouble(argsList[1]));
                        if (overload == 271) return ((DoubleList)argsList[0]).Contains(Convert.ToInt64(argsList[1]));
                        if (overload == 272) return ((DoubleList)argsList[0]).Contains(Convert.ToString(argsList[1]));
                        if (overload == 280) return ((List<byte>)argsList[0]).Contains(Convert.ToByte(argsList[1]));
                        if (overload == 282) return ((ByteList)argsList[0]).Contains(Convert.ToDecimal(argsList[1]));
                        if (overload == 283) return ((ByteList)argsList[0]).Contains(Convert.ToString(argsList[1]));
                        if (overload == 290) return ((List<short>)argsList[0]).Contains(Convert.ToInt16(argsList[1]));
                        if (overload == 292) return ((ShortList)argsList[0]).Contains(Convert.ToDecimal(argsList[1]));
                        if (overload == 293) return ((ShortList)argsList[0]).Contains(Convert.ToString(argsList[1]));
                        if (overload == 300) return ((List<int>)argsList[0]).Contains(Convert.ToInt32(argsList[1]));
                        if (overload == 302) return ((IntList)argsList[0]).Contains(Convert.ToDecimal(argsList[1]));
                        if (overload == 303) return ((IntList)argsList[0]).Contains(Convert.ToString(argsList[1]));
                        if (overload == 310) return ((List<long>)argsList[0]).Contains(Convert.ToInt64(argsList[1]));
                        if (overload == 312) return ((LongList)argsList[0]).Contains(Convert.ToDecimal(argsList[1]));
                        if (overload == 313) return ((LongList)argsList[0]).Contains(Convert.ToString(argsList[1]));
                        if (overload == 320) return ((List<Guid>)argsList[0]).Contains((Guid)argsList[1]);
                        if (overload == 330) return ((List<string>)argsList[0]).Contains(Convert.ToString(argsList[1]));
                        if (overload == 331) return ((StringList)argsList[0]).Contains(Convert.ToInt64(argsList[1]));
                        if (overload == 332) return ((StringList)argsList[0]).Contains(Convert.ToDecimal(argsList[1]));
                        if (overload == 333) return ((StringList)argsList[0]).Contains(Convert.ToDouble(argsList[1]));
                        break;

                    case StiMethodType.ToQueryString:
                        if (argsList.Count == 1)
                        {
                            if (baseValue is List<bool>) return ((BoolList)argsList[0]).ToQueryString();
                            if (baseValue is List<char>) return ((CharList)argsList[0]).ToQueryString();
                            if (baseValue is List<DateTime>) return ((DateTimeList)argsList[0]).ToQueryString();
                            if (baseValue is List<TimeSpan>) return ((TimeSpanList)argsList[0]).ToQueryString();
                            if (baseValue is List<decimal>) return ((DecimalList)argsList[0]).ToQueryString();
                            if (baseValue is List<float>) return ((FloatList)argsList[0]).ToQueryString();
                            if (baseValue is List<double>) return ((DoubleList)argsList[0]).ToQueryString();
                            if (baseValue is List<byte>) return ((ByteList)argsList[0]).ToQueryString();
                            if (baseValue is List<short>) return ((ShortList)argsList[0]).ToQueryString();
                            if (baseValue is List<int>) return ((IntList)argsList[0]).ToQueryString();
                            if (baseValue is List<long>) return ((LongList)argsList[0]).ToQueryString();
                            if (baseValue is List<Guid>) return ((GuidList)argsList[0]).ToQueryString();
                            if (baseValue is List<string>) return ((StringList)argsList[0]).ToQueryString();
                        }
                        else if (argsList.Count == 2)
                        {
                            if (baseValue is List<bool>) return ((BoolList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<char>) return ((CharList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<DateTime>) return ((DateTimeList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<TimeSpan>) return ((TimeSpanList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<decimal>) return ((DecimalList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<float>) return ((FloatList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<double>) return ((DoubleList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<byte>) return ((ByteList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<short>) return ((ShortList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<int>) return ((IntList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<long>) return ((LongList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<Guid>) return ((GuidList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                            if (baseValue is List<string>) return ((StringList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]));
                        }
                        else if (argsList.Count == 3)
                        {
                            if (baseValue is List<DateTime>) return ((DateTimeList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]), Convert.ToString(argsList[2]));
                            if (baseValue is List<TimeSpan>) return ((TimeSpanList)argsList[0]).ToQueryString(Convert.ToString(argsList[1]), Convert.ToString(argsList[2]));
                        }
                        else ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "ToQueryString", (argsList.Count - 1).ToString());
                        break;
                }
            }
            #endregion

            #region type DataSource
            if (baseValue is StiDataSource)
            {
                switch ((StiMethodType)name)
                {
                    case StiMethodType.GetData:
                        category = get_category(argsList[1]);
                        if (category != 1) ThrowError(ParserErrorCode.MethodHasInvalidArgument, "GetData", "1", GetTypeName(argsList[0]), "string");
                        if (argsList.Count == 3)
                        {
                            category = get_category(argsList[2]);
                            if (category < 4 || category > 7) ThrowError(ParserErrorCode.MethodHasInvalidArgument, "GetData", "2", GetTypeName(argsList[0]), "int");
                            return ((StiDataSource)argsList[0]).GetData(Convert.ToString(argsList[1]), Convert.ToInt32(argsList[2]));
                        }
                        else if (argsList.Count == 2)
                        {
                            return ((StiDataSource)argsList[0]).GetData(Convert.ToString(argsList[1]));
                        }
                        else ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "GetData", (argsList.Count - 1).ToString());
                        break;
                }
            }
            #endregion

            #region type DateTime
            if (baseValue is DateTime)
            {
                switch ((StiMethodType)name)
                {
                    case StiMethodType.AddDays:
                        category = get_category(argsList[1]);
                        if (category < 2 || category > 7) ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "AddDays", "1", GetTypeName(argsList[1]), "double");
                        if (argsList.Count == 2) return ((DateTime)argsList[0]).AddDays(Convert.ToDouble(argsList[1]));
                        ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "AddDays", (argsList.Count - 1).ToString());
                        break;

                    case StiMethodType.AddHours:
                        category = get_category(argsList[1]);
                        if (category < 2 || category > 7) ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "AddHours", "1", GetTypeName(argsList[1]), "double");
                        if (argsList.Count == 2) return ((DateTime)argsList[0]).AddHours(Convert.ToDouble(argsList[1]));
                        ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "AddHours", (argsList.Count - 1).ToString());
                        break;

                    case StiMethodType.AddMilliseconds:
                        category = get_category(argsList[1]);
                        if (category < 2 || category > 7) ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "AddMilliseconds", "1", GetTypeName(argsList[1]), "double");
                        if (argsList.Count == 2) return ((DateTime)argsList[0]).AddMilliseconds(Convert.ToDouble(argsList[1]));
                        ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "AddMilliseconds", (argsList.Count - 1).ToString());
                        break;

                    case StiMethodType.AddMinutes:
                        category = get_category(argsList[1]);
                        if (category < 2 || category > 7) ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "AddMinutes", "1", GetTypeName(argsList[1]), "double");
                        if (argsList.Count == 2) return ((DateTime)argsList[0]).AddMinutes(Convert.ToDouble(argsList[1]));
                        ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "AddMinutes", (argsList.Count - 1).ToString());
                        break;

                    case StiMethodType.AddMonths:
                        category = get_category(argsList[1]);
                        if (category < 4 || category > 7) ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "AddMonths", "1", GetTypeName(argsList[1]), "int");
                        if (argsList.Count == 2) return ((DateTime)argsList[0]).AddMonths(Convert.ToInt32(argsList[1]));
                        ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "AddMonths", (argsList.Count - 1).ToString());
                        break;

                    case StiMethodType.AddSeconds:
                        category = get_category(argsList[1]);
                        if (category < 2 || category > 7) ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "AddSeconds", "1", GetTypeName(argsList[1]), "double");
                        if (argsList.Count == 2) return ((DateTime)argsList[0]).AddSeconds(Convert.ToDouble(argsList[1]));
                        ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "AddSeconds", (argsList.Count - 1).ToString());
                        break;

                    case StiMethodType.AddYears:
                        category = get_category(argsList[1]);
                        if (category < 4 || category > 7) ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "AddYears", "1", GetTypeName(argsList[1]), "int");
                        if (argsList.Count == 2) return ((DateTime)argsList[0]).AddYears(Convert.ToInt32(argsList[1]));
                        ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "AddYears", (argsList.Count - 1).ToString());
                        break;

                    case StiMethodType.ToShortDateString:
                        if (overload == 1) return ((DateTime)argsList[0]).ToShortDateString();
                        break;
                    case StiMethodType.ToShortTimeString:
                        if (overload == 1) return ((DateTime)argsList[0]).ToShortTimeString();
                        break;
                    case StiMethodType.ToLongDateString:
                        if (overload == 1) return ((DateTime)argsList[0]).ToLongDateString();
                        break;
                    case StiMethodType.ToLongTimeString:
                        if (overload == 1) return ((DateTime)argsList[0]).ToLongTimeString();
                        break;
                }
            }
            #endregion

            #region type TimeSpan
            if (baseValue is TimeSpan)
            {
                switch ((StiMethodType)name)
                {
                    case StiMethodType.Add:
                        if (!(argsList[1] is TimeSpan)) ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Add", "1", GetTypeName(argsList[1]), "TimeSpan");
                        if (argsList.Count == 2) return ((TimeSpan)argsList[0]).Add((TimeSpan)argsList[1]);
                        ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "Add", (argsList.Count - 1).ToString());
                        break;

                    case StiMethodType.Subtract:
                        if (!(argsList[1] is TimeSpan)) ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Subtract", "1", GetTypeName(argsList[1]), "TimeSpan");
                        if (argsList.Count == 2) return ((TimeSpan)argsList[0]).Subtract((TimeSpan)argsList[1]);
                        ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "Subtract", (argsList.Count - 1).ToString());
                        break;
                }
            }
            #endregion

            #region type GroupHeaderBand
            if (baseValue is StiGroupHeaderBand)
            {
                switch ((StiMethodType)name)
                {
                    case StiMethodType.GetCurrentConditionValue:
                        return ((StiGroupHeaderBand)argsList[0]).GetCurrentConditionValue();
                }
            }
            #endregion

            string message1 = (baseValue == null) ? "null" : GetTypeName(argsList[0]);
            ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, message1, Enum.GetName(typeof(StiMethodType), name));

            return null;
        }
        #endregion    
    }
}
