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

using System.Collections.Generic;
using System.Linq;

using Stimulsoft.Data.Exceptions;
using Stimulsoft.Data.Helpers;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        public static bool IsMeasureFunction(string expression)
        {
            var func = StiExpressionHelper.GetFunction(expression);
            if (func == null) return false;

            func = ToLowerCase(func);

            if (func == "percentofgrandtotal")
                return true;

            return GetMeasureFunctions()
                .Any(f => ToLowerCase(f.ToString()) == func);
        }

        public static IEnumerable<string> GetMeasureFunctions()
        {
            return GetAggregateMeasureFunctions().Concat(GetCommonMeasureFunctions()).Distinct();
        }

        public static IEnumerable<string> GetAggregateMeasureFunctions()
        {
            return new []
            {
                "Sum",
                "Avg",
                "Min",
                "Max",
                "DistinctCount",
                "Count",
                "Median"
            };
        }

        public static IEnumerable<string> GetCommonMeasureFunctions()
        {
            return new[]
            {
                "First",
                "Last",
                "Count",
                "DistinctCount"
            };
        }

        public static object Calculate(string function, IEnumerable<object> values)
        {
            switch (ToLowerCase(function))
            {
                case "all":
                    return All(values);

                #region Avg
                case "avg":
                    return Avg(values);

                case "avgd":
                    return AvgD(values);

                case "avgi":
                    return AvgI(values);

                case "avgdate":
                    return AvgDate(values);

                case "avgtime":
                    return AvgTime(values);
                #endregion

                case "count":
                    return Count(values);

                case "distinctcount":
                    return DistinctCount(values);

                case "first":
                    return First(values);

                case "last":
                    return Last(values);

                #region Max
                case "max":
                    return Max(values);

                case "maxd":
                    return MaxD(values);

                case "maxi":
                    return MaxI(values);

                case "maxdate":
                    return MaxDate(values);

                case "maxtime":
                    return MaxTime(values);

                case "maxstr":
                    return MaxStr(values);
                #endregion

                #region Median
                case "median":
                    return Median(values);

                case "mediand":
                    return MedianD(values);

                case "mediani":
                    return MedianI(values);
                #endregion

                #region Min
                case "min":
                    return Min(values);

                case "mind":
                    return MinD(values);

                case "mini":
                    return MinI(values);

                case "mindate":
                    return MinDate(values);

                case "mintime":
                    return MinTime(values);

                case "minstr":
                    return MinStr(values);
                #endregion

                #region Mode
                case "mode":
                    return Mode(values);

                case "moded":
                    return ModeD(values);

                case "modei":
                    return ModeI(values);
                #endregion

                #region Sum
                case "sum":
                    return Sum(values);

                case "sumd":
                    return SumD(values);

                case "sumi":
                    return SumI(values);

                case "sumtime":
                    return SumTime(values);

                case "sumdistinct":
                    return SumDistinct(values);
                #endregion

                default:
                    throw new StiFunctionNotFoundException(function);
            }
        }
 }
}