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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Stimulsoft.Data.Engine
{
    public static class StiDataAnalyzer
    {
        #region Methods
        public static StiDataTable Analyze(IStiQueryObject query, string group, IEnumerable<IStiMeter> meters, StiDataRequestOption option = StiDataRequestOption.All,
            IEnumerable<StiDataSortRule> userSorts = null, IEnumerable<StiDataFilterRule> userFilters = null,
            IEnumerable<StiDataFilterRule> dataFilters = null, IEnumerable<StiDataActionRule> dataActions = null,
            IEnumerable<StiDataSortRule> transformSorts = null, IEnumerable<StiDataFilterRule> transformFilters = null, IEnumerable<StiDataActionRule> transformActions = null,
            IEnumerable<StiDataFilterRule> drillDownFilters = null)
        {
            if (meters == null || !meters.Any())
                return StiDataTable.NullTable;

            var dict = query.GetDictionary();
            var report = dict?.GetApp() as IStiReport;
            var hash = 0;
            DataTable table;

            var names = UnionNames(null, userFilters?.Select(f => f.Path), userFilters?.Select(f => f.Path2));
            names = UnionNames(names, dataFilters?.Select(f => f.Path), dataFilters?.Select(f => f.Path2));
            names = UnionNames(names, dataActions?.Select(f => f.Path));
            names = UnionNames(names, transformFilters?.Select(f => f.Path), transformFilters?.Select(f => f.Path2));
            names = UnionNames(names, transformActions?.Select(f => f.Path));
            names = UnionNames(names, drillDownFilters?.Select(f => f.Path), drillDownFilters?.Select(f => f.Path2));

            //Do not remove! It's the first part of code to run data analyze via SQL queries
            /*if (StiBIDataCacheOptions.Enabled && StiDataPicker.IsAllBICached(query, group, option))
            {
                table = StiSqlDataAnalyzer.FetchAndJoin(query, group, names, dataFilters, report);
                if (table == null)
                    return StiDataTable.NullTable;
            }
            else*/

            var links = StiDataLinkHelper.GetLinks(dict);

            var tables = StiDataPicker.Fetch(query, group, option, names, links);
            if (tables == null || !tables.Any())
                return StiDataCreator.Create(dict, meters);
                        
            table = StiDataJoiner.Join(tables, links, meters, report);
            if (table == null)
                return StiDataTable.NullTable;

            //Filters
            var filters = UnionFilters(dataFilters, userFilters, drillDownFilters);
            hash = group?.GetHashCode() ?? 0;
            hash = GetUniqueCode(report, filters, hash);
            table = StiDataFiltrator.Filter(table, filters, report, hash);
            if (table == null)
                return StiDataTable.NullTable;

            //Data: Actions before grouping
            hash = GetUniqueCode(report, dataActions?.Where(a => a.Priority == StiDataActionPriority.BeforeTransformation), hash);
            table = StiDataActionOperator.Apply(table, dataActions, report, hash);
            if (table == null)
                return StiDataTable.NullTable;

            //Groups
            hash = GetUniqueCode(meters, hash);
            var dataTable = StiDataGrouper.Group(dict, table, meters.ToList());
            if (dataTable == null)
                return StiDataTable.NullTable;

            //Data: Actions after grouping
            hash = GetUniqueCode(report, dataActions?.Where(a => a.Priority == StiDataActionPriority.AfterGroupingData), hash);
            dataTable = StiDataActionOperator.ApplyAfterTransformation(dataTable, dataActions, StiDataActionPriority.AfterGroupingData, report, hash);
            if (dataTable == null)
                return StiDataTable.NullTable;

            //Transform: Filters
            hash = GetUniqueCode(report, transformFilters, hash);
            dataTable = StiDataFiltrator.Filter(dataTable, transformFilters, report, hash);
            if (dataTable == null)
                return StiDataTable.NullTable;

            //Transform: Actions after grouping
            hash = GetUniqueCode(report, transformActions?.Where(a => a.Priority == StiDataActionPriority.AfterGroupingData), hash);
            dataTable = StiDataActionOperator.ApplyAfterTransformation(dataTable, transformActions, StiDataActionPriority.AfterGroupingData, report, hash);
            if (dataTable == null)
                return StiDataTable.NullTable;

            //Transform: Sorts
            hash = GetUniqueCode(report, transformSorts, hash);
            dataTable = StiDataSorter.Sort(dataTable, transformSorts, report, hash);
            if (dataTable == null)
                return StiDataTable.NullTable;

            //Transform: Actions after sorts
            hash = GetUniqueCode(report, transformActions?.Where(a => a.Priority == StiDataActionPriority.AfterSortingData), hash);
            dataTable = StiDataActionOperator.ApplyAfterTransformation(dataTable, transformActions, StiDataActionPriority.AfterSortingData, report, hash);
            if (dataTable == null)
                return StiDataTable.NullTable;

            //User: Sorts
            hash = GetUniqueCode(report, userSorts, hash);
            dataTable = StiDataSorter.Sort(dataTable, userSorts, report, hash);
            if (dataTable == null)
                return StiDataTable.NullTable;

            //User: Actions after sorts
            hash = GetUniqueCode(report, dataActions?.Where(a => a.Priority == StiDataActionPriority.AfterSortingData), hash);
            dataTable = StiDataActionOperator.ApplyAfterTransformation(dataTable, dataActions, StiDataActionPriority.AfterSortingData, report, hash);
            if (dataTable == null)
                return StiDataTable.NullTable;

            return dataTable;
        }

        private static List<StiDataFilterRule> UnionFilters(IEnumerable<StiDataFilterRule> dataFilters,
            IEnumerable<StiDataFilterRule> userFilters, IEnumerable<StiDataFilterRule> drillDownFilters)
        {
            if (dataFilters == null && userFilters == null && drillDownFilters == null)
                return null;

            var filters = new List<StiDataFilterRule>();
            if (dataFilters != null)
                filters = filters.Union(dataFilters).ToList();

            if (userFilters != null)
                filters = filters.Union(userFilters).ToList();

            if (drillDownFilters != null)
                filters = filters.Union(drillDownFilters).ToList();

            return filters;
        }

        private static IEnumerable<string> UnionNames(IEnumerable<string> namesTo, IEnumerable<string> namesFrom1,
            IEnumerable<string> namesFrom2 = null)
        {
            if (namesTo == null && namesFrom1 == null && namesFrom2 == null)
                return null;

            if (namesTo != null && namesFrom1 != null && namesFrom2 == null)
                return namesTo.Union(namesFrom1);

            if (namesTo != null && namesFrom1 != null && namesFrom2 != null)
                return namesTo.Union(namesFrom1).Union(namesFrom2);

            if (namesTo == null && namesFrom1 != null && namesFrom2 != null)
                return namesFrom1.Union(namesFrom2);

            return namesTo ?? namesFrom1;
        }

        private static int GetUniqueCode(IStiApp app, IEnumerable<StiDataRule> rules, int? initialHash = null)
        {
            if (rules == null || !rules.Any())
                return initialHash.GetValueOrDefault(0);

            var hash = rules
                .Select(r => r.GetUniqueCode())
                .Aggregate(0, (r1, r2) => unchecked(r1 + r2));

            //Gets additional hash for expression values
            if (rules.All(r => r is StiDataFilterRule))
            {
                var expressionsHash = StiDataFilterRuleHelper.GetFilterRulesHash(app, rules.Cast<StiDataFilterRule>());
                hash = unchecked(hash + expressionsHash);
            }

            if (initialHash == null)
                return hash;
            else
                return unchecked(hash + initialHash.Value);
        }

        private static int GetUniqueCode(IEnumerable<IStiMeter> meters, int? initialHash = null)
        {
            if (meters == null || !meters.Any())
                return initialHash.GetValueOrDefault(0);

            var hash = meters
                .Select(c => c.GetUniqueCode())
                .Aggregate(0, (c1, c2) => unchecked(c1 + c2));

            if (initialHash == null)
                return hash;
            else
                return unchecked(hash + initialHash.Value);
        }
        #endregion
    }
}