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

using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Data.Types;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        public static object First(object value)
        {
            if (ListExt.IsList(value))
                return SkipNulls(ListExt.ToList(value)).FirstOrDefault();
            else
                return value;
        }

        public static object Last(object value)
        {
            if (ListExt.IsList(value))
                return SkipNulls(ListExt.ToList(value)).LastOrDefault();
            else
                return value;
        }

        public static object All(object value)
        {
            if (ListExt.IsList(value))
                return SkipNulls(ListExt.ToList(value)).Select(v => new SimpleValue(v));
            else
                return new SimpleValue(value);
        }
        
        public static IEnumerable<object> RunningTotal(IEnumerable<object> values)
        {
            var items = values.Select(v => v is IEnumerable<object>
                ? Sum(v as IEnumerable<object>)
                : StiValueHelper.TryToDecimal(v));

            var start = 0m;
            foreach (var item in items)
            {
                yield return start + item;
                start += item;
            }
        }

        public static bool IsAggregationFunction(string function)
        {
            switch (ToLowerCase(function))
            {
                case "avg":
                case "avgd":
                case "avgi":
                case "avgdate":
                case "avgtime":

                case "max":
                case "maxd":
                case "maxi":
                case "maxdate":
                case "maxtime":
                case "maxstr":

                case "median":
                case "mediand":
                case "mediani":

                case "min":
                case "mind":
                case "mini":
                case "mindate":
                case "mintime":
                case "minstr":
                case "minmaxdatestring":                   
                
                case "mode":
                case "moded":
                case "modei":

                case "sum":
                case "sumd":
                case "sumi":
                case "sumtime":
                case "sumdistinct":

                case "sumif":
                case "sumdif":
                case "sumiif":
                case "sumtimeif":
                case "sumdistinctif":

                case "count":
                case "countdistinct":

                case "countif":
                case "countdistinctif":
                    return true;

                default:
                    return false;
            }
        }
    }
}