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
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Exceptions;
using Stimulsoft.Data.Expressions.NCalc;
using Stimulsoft.Data.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Data.Parsers
{
    public abstract class StiDataParser
    {
        #region Methods
        protected object RunFunction(string funcName, FunctionArgs args)
        {
            var lowerName = Funcs.ToLowerCase(funcName);

            switch (lowerName)
            {
                case "all":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.All(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                #region Avg
                case "avg":
                    return Funcs.Avg(GetDataColumnFromArg0(funcName, args));

                case "avgnulls":
                    return Funcs.AvgNulls(GetDataColumnFromArg0(funcName, args));

                case "avgd":
                    return Funcs.AvgD(GetDataColumnFromArg0(funcName, args));

                case "avgi":
                    return Funcs.AvgI(GetDataColumnFromArg0(funcName, args));

                case "avgdate":
                    return Funcs.AvgDate(GetDataColumnFromArg0(funcName, args));

                case "avgtime":
                    return Funcs.AvgTime(GetDataColumnFromArg0(funcName, args));
                #endregion

                case "count":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.Count(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }

                case "countif":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.CountIf(GetDataColumnFromArg0(funcName, args), GetObjectFromArg1("condition", funcName, args));
                        else
                            return 0;
                    }
                    
                case "distinct":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.Distinct(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }

                case "distinctcount":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.DistinctCount(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }

                case "distinctcountif":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.DistinctCountIf(GetDataColumnFromArg0(funcName, args), GetObjectFromArg1("condition", funcName, args));
                        else
                            return 0;
                    }

                case "grandcount":
                    {
                        if (args.Parameters.Length > 0)
                        {
                            IsGrandTotal = true;
                            var result = Funcs.Count(GetDataColumnFromArg0(funcName, args));
                            IsGrandTotal = false;
                            return result;
                        }
                        else
                            return 0;
                    }

                case "grandtotal":
                {
                    if (args.Parameters.Length > 0)
                    {
                        IsGrandTotal = true;
                        var result = Funcs.Sum(GetDataColumnFromArg0(funcName, args));
                        IsGrandTotal = false;
                        return result;
                    }
                    else
                        return 0;
                }

                case "getparam":
                    {
                        if (args.Parameters.Length > 0)                        
                            return Dictionary.GetVariableValueByName(GetDataColumnFromArg0(funcName, args)?.ToString());                        
                        else
                            return null;
                    }

                case "percentofgrandtotal":
                    {
                        if (args.Parameters.Length > 0)
                        {
                            var value = Funcs.Sum(GetDataColumnFromArg0(funcName, args));
                            IsGrandTotal = true;
                            var grand = Funcs.Sum(GetDataColumnFromArg0(funcName, args));
                            IsGrandTotal = false;

                            if (grand == 0)
                                return 0;

                            return value / grand;
                        }
                        else
                        {
                            return 0;
                        }
                    }

                case "first":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.First(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                case "last":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.Last(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                case "median":
                    return Funcs.Median(GetDataColumnFromArg0(funcName, args));

                #region Max
                case "max":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.Max(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }

                case "maxnulls":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MaxNulls(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                case "maxd":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MaxD(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }

                case "maxi":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MaxI(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }

                case "maxdate":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MaxDate(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                case "maxtime":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MaxTime(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                case "maxstr":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MaxStr(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }
                #endregion

                #region Min
                case "min":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.Min(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }

                case "minnulls":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MinNulls(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                case "mind":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MinD(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }

                case "mini":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MinI(GetDataColumnFromArg0(funcName, args));

                        else
                            return 0;
                    }

                case "mindate":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MinDate(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                case "minmaxdatestring":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MinMaxDateString(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                case "mintime":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MinTime(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                case "minstr":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.MinStr(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }
                #endregion

                #region Sum
                case "sum":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.Sum(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }

                case "sumnulls":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.SumNulls(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }

                case "sumd":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.SumD(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }

                case "sumi":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.SumI(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }                

                case "sumdistinct":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.SumDistinct(GetDataColumnFromArg0(funcName, args));
                        else
                            return 0;
                    }                

                case "sumtime":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.SumTime(GetDataColumnFromArg0(funcName, args));
                        else
                            return new TimeSpan();
                    }
                #endregion

                #region SumIf
                case "sumif":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.SumIf(GetDataColumnFromArg0(funcName, args), GetObjectFromArg1("condition", funcName, args));
                        else
                            return 0;
                    }

                case "sumdif":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.SumDIf(GetDataColumnFromArg0(funcName, args), GetObjectFromArg1("condition", funcName, args));
                        else
                            return 0;
                    }

                case "sumiif":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.SumIIf(GetDataColumnFromArg0(funcName, args), GetObjectFromArg1("condition", funcName, args));
                        else
                            return 0;
                    }

                case "sumdistinctif":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.SumDistinctIf(GetDataColumnFromArg0(funcName, args), GetObjectFromArg1("condition", funcName, args));
                        else
                            return 0;
                    }

                case "sumtimeif":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.SumTimeIf(GetDataColumnFromArg0(funcName, args), GetObjectFromArg1("condition", funcName, args));
                        else
                            return 0;
                    }
                #endregion

                #region Date
                case "addmonths":
                    {
                        var arg0 = GetObjectFromArg(0, "date", funcName, args);
                        var arg1 = StiValueHelper.TryToInt(GetObjectFromArg(1, "months", funcName, args));

                        return Funcs.AddMonthsObject(arg0, arg1);
                    }

                case "addyear":
                    {
                        var arg0 = GetObjectFromArg(0, "date", funcName, args);
                        var arg1 = StiValueHelper.TryToInt(GetObjectFromArg(1, "years", funcName, args));

                        return Funcs.AddYearsObject(arg0, arg1);
                    }

                case "datediff":
                    return Funcs.DateDiffObject(GetObjectFromArg0("date1", funcName, args), GetObjectFromArg1("date2", funcName, args));

                case "datetime":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.DateTime(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                case "day":
                    return Funcs.DayObject(GetObjectFromArg0("date", funcName, args));

                case "daysinmonth":
                    {
                        if (args.Parameters.Length == 1)
                            return Funcs.DaysInMonthObject(GetObjectFromArg0("date", funcName, args));
                        else
                            return Funcs.DaysInMonthObject(GetObjectFromArg0("year", funcName, args), GetObjectFromArg1("month", funcName, args));
                    }

                case "daysinyear":
                    {
                        var value = GetObjectFromArg0("value", funcName, args);
                        if (value is DateTime)
                            return Funcs.DaysInYearObject((DateTime)value);
                        else
                            return Funcs.DaysInYearObject(StiValueHelper.TryToInt(value));
                    }

                case "dayofweek":
                    {
                        var arg0 = GetObjectFromArg0("date", funcName, args);

                        if (args.Parameters.Length == 1)
                            return Funcs.DayOfWeekObject(arg0);

                        else if (args.Parameters.Length == 2)
                        {
                            var arg1 = GetObjectFromArg1("localized", funcName, args);

                            if (arg1 is bool)
                                return Funcs.DayOfWeekObject(arg0, (bool)arg1);

                            else
                                return Funcs.DayOfWeekObject(arg0, arg1.ToString());
                        }
                        else if (args.Parameters.Length == 3)
                        {
                            var arg1 = GetObjectFromArg1("culture", funcName, args);
                            var arg2 = GetObjectFromArg2("upperCase", funcName, args);

                            return Funcs.DayOfWeekObject(arg0, arg1.ToString(), arg2 is bool && (bool)arg2);
                        }
                        else throw new StiArgumentCountException(funcName);
                    }

                case "dayofweekident":
                    return Funcs.DayOfWeekIdentObject(GetObjectFromArg0("date", funcName, args));

                case "dayofweekindex":
                    return Funcs.DayOfWeekIndexObject(GetObjectFromArg0("date", funcName, args));

                case "dayofyear":
                    return Funcs.DayOfYearObject(GetObjectFromArg0("date", funcName, args));

                case "financialquarter":
                    return Funcs.FinancialQuarterObject(GetObjectFromArg0("date", funcName, args));

                case "financialquarterindex":
                    return Funcs.FinancialQuarterIndexObject(GetObjectFromArg0("date", funcName, args));

                case "hour":
                    return Funcs.HourObject(GetObjectFromArg0("date", funcName, args));

                case "makedate":
                case "dateserial":
                    {
                        var arg0 = GetObjectFromArg(0, "year", funcName, args);
                        var arg1 = GetObjectFromArg(1, "months", funcName, args);
                        var arg2 = GetObjectFromArg(2, "day", funcName, args);

                        return Funcs.MakeDateObject(arg0, arg1, arg2);
                    }

                case "makedatetime":
                    {
                        var arg0 = GetObjectFromArg(0, "year", funcName, args);
                        var arg1 = GetObjectFromArg(1, "months", funcName, args);
                        var arg2 = GetObjectFromArg(2, "day", funcName, args);
                        var arg3 = GetObjectFromArg(3, "hour", funcName, args);
                        var arg4 = GetObjectFromArg(4, "minute", funcName, args);
                        var arg5 = GetObjectFromArg(5, "second", funcName, args);

                        return Funcs.MakeDateTimeObject(arg0, arg1, arg2, arg3, arg4, arg5);
                    }

                case "maketime":
                case "timeserial":
                    {
                        var arg0 = GetObjectFromArg(0, "hour", funcName, args);
                        var arg1 = GetObjectFromArg(1, "minute", funcName, args);
                        var arg2 = GetObjectFromArg(2, "second", funcName, args);

                        return Funcs.MakeTimeObject(arg0, arg1, arg2);
                    }

                case "minute":
                    return Funcs.MinuteObject(GetObjectFromArg0("date", funcName, args));

                case "month":
                    return Funcs.MonthObject(GetObjectFromArg0("date", funcName, args));

                case "monthident":
                    return Funcs.MonthIdentObject(GetObjectFromArg0("date", funcName, args));

                case "fiscalmonthident":
                    {
                        var arg0 = GetObjectFromArg(0, "date", funcName, args);
                        var arg1 = GetObjectFromArg(1, "startMonth", funcName, args);

                        return Funcs.FiscalMonthIdentObject(arg0, arg1);
                    }

                case "monthname":
                    {
                        var arg0 = GetObjectFromArg0("date", funcName, args);

                        if (args.Parameters.Length == 1)
                            return Funcs.MonthNameObject(arg0);

                        else if (args.Parameters.Length == 2)
                        {
                            var arg1 = GetObjectFromArg1("localized", funcName, args);

                            if (arg1 is bool)
                                return Funcs.MonthNameObject(arg0, (bool)arg1);

                            else
                                return Funcs.MonthNameObject(arg0, arg1.ToString());
                        }
                        else if (args.Parameters.Length == 3)
                        {
                            var arg1 = GetObjectFromArg1("culture", funcName, args);
                            var arg2 = GetObjectFromArg2("upperCase", funcName, args);

                            return Funcs.MonthNameObject(arg0, arg1.ToString(), arg2 is bool && (bool)arg2);
                        }
                        else
                            throw new StiArgumentCountException(funcName);
                    }

                case "now":
                    return Funcs.Now();

                case "fromoadate":
                    {
                        var arg0 = GetObjectFromArg(0, "value", funcName, args);
                        return Funcs.FromOADateObject(arg0);
                    }

                case "tooadate":
                    {
                        var arg0 = GetObjectFromArg(0, "value", funcName, args);
                        return Funcs.ToOADateObject(arg0);
                    }

                case "quarter":
                    return Funcs.QuarterObject(GetObjectFromArg0("date", funcName, args));

                case "quarterindex":
                    return Funcs.QuarterIndexObject(GetObjectFromArg0("date", funcName, args));

                case "quartername":
                {
                    var arg0 = GetObjectFromArg0("date", funcName, args);

                    if (args.Parameters.Length == 1)
                        return Funcs.QuarterNameObject(arg0, false);

                    else if (args.Parameters.Length == 2)
                    {
                        var arg1 = GetObjectFromArg1("localized", funcName, args);

                        if (arg1 is bool)
                            return Funcs.QuarterNameObject(arg0, (bool)arg1);

                        else
                            return Funcs.QuarterNameObject(arg0);
                    }
                    else
                        throw new StiArgumentCountException(funcName);
                }

                case "second":
                    return Funcs.SecondObject(GetObjectFromArg0("date", funcName, args));

                case "time":
                    {
                        if (args.Parameters.Length > 0)
                            return Funcs.Time(GetDataColumnFromArg0(funcName, args));
                        else
                            return null;
                    }

                case "year":
                    return Funcs.YearObject(GetObjectFromArg0("date", funcName, args));

                case "yearmonth":
                    return Funcs.YearMonthObject(GetObjectFromArg0("date", funcName, args));
                #endregion

                #region String
                case "format":
                    {
                        var arg0 = StiValueHelper.TryToString(GetObjectFromArg0("format", funcName, args));
                        var arg1 = GetObjectFromArg(1, "value", funcName, args);

                        return Funcs.Format(arg0, arg1);
                    }

                case "insert":
                    {
                        var arg0 = GetObjectFromArg0("str", funcName, args);
                        var arg1 = StiValueHelper.TryToInt(GetObjectFromArg(1, "startIndex", funcName, args));
                        var arg2 = StiValueHelper.TryToString(GetObjectFromArg(2, "value", funcName, args));
                        return Funcs.InsertObject(arg0, arg1, arg2);
                    }

                case "iso2":
                    {
                        var arg0 = GetObjectFromArg0("name", funcName, args);
                        var arg1 = args.Parameters.Length > 1 ? StiValueHelper.TryToString(GetObjectFromArg(1, "mapId", funcName, args)) : null;
                        return Funcs.Iso2Object(arg0, arg1, null);
                    }

                case "iso2toname":
                    {
                        var arg0 = GetObjectFromArg0("alpha2", funcName, args);
                        var arg1 = args.Parameters.Length > 1 ? StiValueHelper.TryToString(GetObjectFromArg(1, "mapId", funcName, args)) : null;
                        return Funcs.Iso2ToNameObject(arg0, arg1, null);
                    }

                case "iso3":
                    {
                        var arg0 = GetObjectFromArg0("name", funcName, args);
                        var arg1 = args.Parameters.Length > 1 ? StiValueHelper.TryToString(GetObjectFromArg(1, "mapId", funcName, args)) : null;
                        return Funcs.Iso3Object(arg0, arg1, null);
                    }

                case "iso3toname":
                    {
                        var arg0 = GetObjectFromArg0("alpha2", funcName, args);
                        var arg1 = args.Parameters.Length > 1 ? StiValueHelper.TryToString(GetObjectFromArg(1, "mapId", funcName, args)) : null;
                        return Funcs.Iso3ToNameObject(arg0, arg1, null);
                    }

                case "left":
                    {
                        var arg0 = GetObjectFromArg0("str", funcName, args);
                        var arg1 = StiValueHelper.TryToInt(GetObjectFromArg(1, "length", funcName, args));
                        return Funcs.LeftObject(arg0, arg1);
                    }

                case "length":
                    return Funcs.LengthObject(GetObjectFromArg0("str", funcName, args));

                case "normalizename":
                    {
                        var arg0 = GetObjectFromArg0("name", funcName, args);
                        var arg1 = args.Parameters.Length > 1 ? StiValueHelper.TryToString(GetObjectFromArg(1, "mapId", funcName, args)) : null;
                        return Funcs.NormalizeNameObject(arg0, arg1, null);
                    }

                case "remove":
                    {
                        var arg0 = GetObjectFromArg0("str", funcName, args);
                        var arg1 = StiValueHelper.TryToInt(GetObjectFromArg(1, "startIndex", funcName, args));
                        var arg2 = StiValueHelper.TryToInt(GetObjectFromArg(2, "count", funcName, args));
                        return Funcs.RemoveObject(arg0, arg1, arg2);
                    }

                case "replace":
                    {
                        var arg0 = GetObjectFromArg0("str", funcName, args);
                        var arg1 = StiValueHelper.TryToString(GetObjectFromArg(1, "oldValue", funcName, args));
                        var arg2 = StiValueHelper.TryToString(GetObjectFromArg(2, "newValue", funcName, args));
                        return Funcs.ReplaceObject(arg0, arg1, arg2);
                    }

                case "right":
                    {
                        var arg0 = GetObjectFromArg0("str", funcName, args);
                        var arg1 = StiValueHelper.TryToInt(GetObjectFromArg(1, "length", funcName, args));
                        return Funcs.RightObject(arg0, arg1);
                    }

                case "topropercase":
                    return Funcs.ToProperCaseObject(GetObjectFromArg0("str", funcName, args));

                case "tolowercase":
                    return Funcs.ToLowerCaseObject(GetObjectFromArg0("str", funcName, args));

                case "tostring":
                    return Funcs.ToStringObject(GetObjectFromArg0("value", funcName, args));

                case "touppercase":
                    return Funcs.ToUpperCaseObject(GetObjectFromArg0("str", funcName, args));

                case "substring":
                    {
                        var arg0 = GetObjectFromArg0("str", funcName, args);
                        var arg1 = StiValueHelper.TryToInt(GetObjectFromArg(1, "index", funcName, args));
                        var arg2 = args.Parameters.Length > 2 ? StiValueHelper.TryToInt(GetObjectFromArg(2, "length", funcName, args)) : -1;

                        return Funcs.SubstringObject(arg0, arg1, arg2);
                    }

                case "trim":
                    return Funcs.TrimObject(GetObjectFromArg0("str", funcName, args));

                case "trimstart":
                    return Funcs.TrimStartObject(GetObjectFromArg0("str", funcName, args));

                case "trimend":
                    return Funcs.TrimEndObject(GetObjectFromArg0("str", funcName, args));
                #endregion

                #region Image
                case "image":
                    var widthObject = args.Parameters.Length > 1 ? GetObjectFromArg1("width", funcName, args) : null;
                    var heightObject = args.Parameters.Length > 2 ? GetObjectFromArg2("height", funcName, args) : null;

                    var width = StiValueHelper.TryToNullableInt(widthObject).GetValueOrDefault(200);
                    var height = StiValueHelper.TryToNullableInt(heightObject).GetValueOrDefault(200);

                    if (args.Parameters.Length > 0)
                        return Funcs.Image(GetDataColumnFromArg0(funcName, args), width, height);
                    else
                        return null;
                #endregion

                #region Logic
                case "array":
                    return args.Parameters.Select(p => p.Evaluate()).ToArray();

                case "list":
                    return args.Parameters.Select(p => p.Evaluate()).ToList();

                case "iif":
                    {
                        var condition = GetObjectFromArg0("condition", funcName, args);

                        if (StiValueHelper.TryToBool(condition))
                            return GetObjectFromArg1("truePart", funcName, args);
                        else
                            return GetObjectFromArg2("falsePart", funcName, args);
                    }

                case "choose":
                    {
                        var indexArg = GetObjectFromArg0("index", funcName, args);
                        var index = StiValueHelper.TryToInt(indexArg);
                        var parameters = args.Parameters.Skip(1).Select(p => p.Evaluate()).ToList();
                        
                        if (parameters.Count == 0)
                            return null;

                        if (index < 1 || index > parameters.Count)
                            return null;

                        return parameters[index - 1];
                    }

                case "switch":
                    {
                        var parameters = args.Parameters.Select(p => p.Evaluate()).ToList();
                        for (int index = 0; index < parameters.Count; index += 2)
                        {
                            var condition = parameters[index];
                            var value = parameters[index + 1];

                            if (condition is bool && ((bool)condition) == true)
                                return value;

                            if (condition is bool? && ((bool?)condition) == true)
                                return value;
                        }
                        return null;
                    }
                #endregion

                #region Math
                case "abs":
                    return Funcs.AbsObject(GetObjectFromArg0("value", funcName, args));

                case "acos":
                    return Funcs.AcosObject(GetObjectFromArg0("value", funcName, args));

                case "asin":
                    return Funcs.AsinObject(GetObjectFromArg0("value", funcName, args));

                case "atan":
                    return Funcs.AtanObject(GetObjectFromArg0("value", funcName, args));

                case "ceiling":
                    return Funcs.CeilingObject(GetObjectFromArg0("value", funcName, args));

                case "cos":
                    return Funcs.CosObject(GetObjectFromArg0("value", funcName, args));

                case "div":
                    {
                        object arg3 = null;
                        if (args.Parameters.Length > 2) arg3 = GetObjectFromArg(2, "zeroResult", funcName, args);

                        return Funcs.DivObject(
                            GetObjectFromArg(0, "value1", funcName, args),
                            GetObjectFromArg(1, "value2", funcName, args),
                            arg3);
                    }

                case "exp":
                    return Funcs.ExpObject(GetObjectFromArg0("value", funcName, args));

                case "floor":
                    return Funcs.FloorObject(GetObjectFromArg0("value", funcName, args));

                case "log":
                    return Funcs.LogObject(GetObjectFromArg0("value", funcName, args));

                case "round":
                    {
                        var arg0 = GetObjectFromArg0("value", funcName, args);
                        var arg1 = 0;

                        if (args.Parameters.Length > 1)
                            arg1 = StiValueHelper.TryToInt(GetObjectFromArg(1, "decimals", funcName, args));

                        return Funcs.RoundObject(arg0, arg1);
                    }

                case "sign":
                    return Funcs.SignObject(GetObjectFromArg0("value", funcName, args));

                case "sin":
                    return Funcs.SinObject(GetObjectFromArg0("value", funcName, args));

                case "sqrt":
                    return Funcs.SqrtObject(GetObjectFromArg0("value", funcName, args));

                case "tan":
                    return Funcs.TanObject(GetObjectFromArg0("value", funcName, args));

                case "truncate":
                    return Funcs.TruncateObject(GetObjectFromArg0("value", funcName, args));
                #endregion

                default:
                    if (Funcs.ExistsCustomFunction(funcName))
                        return Funcs.InvokeCustomFunction(funcName, EvaluateArgs(args));

                    throw new StiFunctionNotFoundException(funcName);
            }
        }

        protected object GetVariableValue(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var isReadOnly = Dictionary != null && Dictionary.IsReadOnlyVariable(name);
            if (!isReadOnly && nameToValue.ContainsKey(name))
                return nameToValue[name];

            var value = Dictionary?.GetVariableValueByName(name);

            if (!isReadOnly)
                nameToValue[name] = value;

            return value;
        }

        protected bool IsVariable(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (nameToVariable.ContainsKey(name))
                return true;

            var value = Dictionary?.GetVariableValueByName(name);

            if (value != null)
            {
                nameToValue[name] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool IsSystemVariable(string name)
        {
            var lowerName = name?.ToLowerInvariant();

            if (lowerName == "line")
                return true;

            if (lowerName == "lineabc")
                return true;

            if (lowerName == "lineroman")
                return true;

            return Dictionary.IsSystemVariable(name);
        }

        protected object GetSystemVariableValue(string name)
        {
            var lowerName = name?.ToLowerInvariant();

            if (lowerName == "line")
                return StiSystemVariableObject.Line;

            if (lowerName == "lineabc")
                return StiSystemVariableObject.LineABC;

            if (lowerName == "lineroman")
                return StiSystemVariableObject.LineRoman;

            return Dictionary.GetSystemVariableValue(name);
        }

        private static object GetObjectFromArg(int argIndex, string argName, string funcName, FunctionArgs args)
        {
            if (argIndex >= args.Parameters.Length)
                throw new StiArgumentNotFoundException(funcName, argName);

            return args.Parameters[argIndex].Evaluate();
        }

        private static IEnumerable<object> EvaluateArgs(FunctionArgs args)
        {
            return args.Parameters.Select(p => p.Evaluate());
        }

        private static object GetObjectFromArg0(string argName, string funcName, FunctionArgs args)
        {
            return GetObjectFromArg(0, argName, funcName, args);
        }

        private static object GetObjectFromArg1(string argName, string funcName, FunctionArgs args)
        {
            return GetObjectFromArg(1, argName, funcName, args);
        }

        private static object GetObjectFromArg2(string argName, string funcName, FunctionArgs args)
        {
            return GetObjectFromArg(2, argName, funcName, args);
        }

        private static object GetDataColumnFromArg0(string funcName, FunctionArgs args)
        {
            return GetObjectFromArg0("dataColumn", funcName, args);
        }

        protected int GetDataColumnIndex(string columnName)
        {
            if (Table == null)
                return -1;

            if (nameToIndex.ContainsKey(columnName))
                return nameToIndex[columnName];

            var dataName = Funcs.ToDataName(columnName);

            var dataColumn = Table.Columns.Cast<DataColumn>()
                .FirstOrDefault(c => DataEqual(c, dataName));

            if (dataColumn == null)
                return -1;

            var dataColumnIndex = Table.Columns.IndexOf(dataColumn);
            if (dataColumnIndex == -1)
                return -1;

            nameToIndex.Add(columnName, dataColumnIndex);

            return dataColumnIndex;
        }

        private bool DataEqual(DataColumn dataColumn, string searchColumnName)
        {
            var dataColumnName = Funcs.ToDataName(dataColumn.ColumnName);
            if (!searchColumnName.Contains("."))
                return false;

            return dataColumnName == searchColumnName;
        }

        protected int GetDimensionIndex(IStiDimensionMeter dimension)
        {
            return Meters
                .Where(m => m is IStiDimensionMeter)
                .ToList()
                .IndexOf(dimension);
        }
        #endregion

        #region Properties
        protected IStiAppDictionary Dictionary { get; }

        protected DataTable Table { get; }

        protected List<IStiMeter> Meters { get; }

        protected bool IsGrandTotal { get; set; }//It is grand measure function now is processed
        #endregion

        #region Fields
        private Dictionary<string, int> nameToIndex = new Dictionary<string, int>();
        private Dictionary<string, object> nameToValue = new Dictionary<string, object>();
        private Dictionary<string, IStiAppVariable> nameToVariable = new Dictionary<string, IStiAppVariable>();
        #endregion

        protected StiDataParser(IStiAppDictionary dictionary, DataTable table, List<IStiMeter> meters)
        {
            this.Dictionary = dictionary;
            this.Table = table;
            this.Meters = meters;
        }
    }
}