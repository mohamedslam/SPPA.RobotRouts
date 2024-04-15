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

using System;
using Stimulsoft.Base;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Comparers;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Parsers;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Stimulsoft.Data.Extensions;

namespace Stimulsoft.Data.Engine
{
    public static class StiDataGrouper
    {
        #region Methods
        public static StiDataTable Group(IStiAppDictionary dictionary, DataTable joinedTable, 
            List<IStiMeter> meters)
        {
            var dimensionParser = new StiDimensionDataParser(dictionary, joinedTable, meters);

            //Check skip normalize data for gantt chart
            var meterList = meters.ToList();
            if (meters.Any(m => m is IStiSkipNormalizeDate))
                meterList = null;

            var rows = joinedTable
                .AsEnumerableArray()
                .GroupBy(row => dimensionParser.Calculate(row, meterList), new StiArrayEqualityComparer())
                .OrderBy(groupRow => groupRow.Key, new StiArrayComparer());

            var meterRows = new StiMeasureDataParser(dictionary, joinedTable, meters, rows)
                .Calculate()
                .ToList();//We need calculate all data rows for correct cache work

            return new StiDataTable(meters, meterRows);
        }
        #endregion
    }
}