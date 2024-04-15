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
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Data.Parsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Data.Engine
{
    public static class StiDataCreator
    {
        public static StiDataTable Create(IStiAppDictionary dict, IEnumerable<IStiMeter> meters)
        {
            var array = meters?
                .Select(m => GetData(dict, m, meters))
                .ToArray();

            var list = array.Where(ListExt.IsList);
            var rowsCount = list.Any () ? list.Max(o => ListExt.ToList(o).Count()) : 1;
            rowsCount = Math.Max(1, rowsCount);

            var rows = array.Select(a => Convert(a, rowsCount)).ToList();
            var metersCount = meters.Count();

            var newRows = new List<object[]>();
            for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                var newRow = new object[metersCount];
                for (var itemIndex = 0; itemIndex < metersCount; itemIndex++)
                {
                    newRow[itemIndex] = rows[itemIndex][rowIndex];
                }

                newRows.Add(newRow);
            }

            return new StiDataTable
            {
                Meters = meters.ToList(),
                Rows = newRows
            };
        }

        private static object[] Convert(object obj, int rowsCount)
        {
            if (ListExt.IsList(obj))
            {
                var list = ListExt.ToList(obj);
                var array = Enumerable.Repeat(list.LastOrDefault(), rowsCount).ToArray();
                var index = 0;

                list.ToList().ForEach(o => array[index++] = o);

                return array;
            }
            else
            {
                return Enumerable.Repeat(obj, rowsCount).ToArray();
            }
        }

        private static object GetData(IStiAppDictionary dict, IStiMeter meter, IEnumerable<IStiMeter> meters)
        {
            if (meter is IStiDimensionMeter)
            {
                //Check skip normalize data for gantt chart
                if (meters.Any(m => m is IStiSkipNormalizeDate))
                    meters = null;

                return new StiDimensionDataParser(dict, meter)
                    .Calculate(new object[1], meters.ToList()).FirstOrDefault();
            }
            else if (meter is IStiMeasureMeter)
            {
                return new StiMeasureDataParser(dict, meter).CalculateMeter(meter);
            }

            return null;
        }
    }
}