#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

#if CLOUD
using Stimulsoft.Base.Plans;
#endif

namespace Stimulsoft.Report.Dashboard
{
    public static class StiElementDataCache
    {
        #region Fields.Static
        private static object lockObject = new object();
        private static object lockCacheObject = new object();
        private static BackgroundWorker worker;
        private static List<IStiElement> elements = new List<IStiElement>();
        private static Dictionary<string, StiDataTable> cache = new Dictionary<string, StiDataTable>();
        private static IStiPivotTableCreator pivotCreator;
        #endregion

        #region Events
        public static event EventHandler DataInit;
        #endregion

        #region Methods
        public static StiDataTable TryToGetOrCreate(IStiElement element, StiDataRequestOption option = StiDataRequestOption.All)
        {
            try
            {
                return GetOrCreate(element, option);
            }
            catch
            {
                return null;
            }
        }

        public static StiDataTable GetOrCreate(IStiElement element, StiDataRequestOption option = StiDataRequestOption.All)
        {
            lock (lockObject)
            {
                var dataTable = Get(element);
                if (dataTable != null)
                    return dataTable;

                #region Clear data rows counters for Cloud plans
#if CLOUD
                if (element?.Report != null)
                {
                    var reportGuid = element.Report.ReportGuid;
                    if (string.IsNullOrEmpty(reportGuid))
                    {
                        reportGuid = StiGuidUtils.NewGuid();
                        element.Report.ReportGuid = reportGuid;
                    }

                    StiCloudReportResults.ClearLimits(reportGuid);
                    StiCloudReport.ResetDataRows(reportGuid);
                }
#endif
                #endregion

                dataTable = Create(element, option);

                Add(element, dataTable);

                return dataTable;
            }
        }

        public static IStiPivotGridContainer GetOrCreatePivot(IStiPivotTableElement element, IStiPivotTableCreator creator,
            StiDataRequestOption option = StiDataRequestOption.All)
        {
            lock (lockObject)
            {
                if (StiPivotToContainerCache.Contains(element) && Get(element) != null)
                    return StiPivotToContainerCache.Get(element);

                var dataTable = Get(element);
                if (dataTable == null)
                {
                    dataTable = Create(element, option);
                    Add(element, dataTable);

                    StiPivotToContainerCache.Remove(element);
                }

                creator.Create(element, dataTable);

                return StiPivotToContainerCache.Get(element);
            }
        }

        public static StiDataTable GetOrCreateWithProgress(IStiElement element,
            StiDataRequestOption option = StiDataRequestOption.All)
        {
            lock (lockObject)
            {
                var dataTable = Get(element);
                if (dataTable != null)
                    return dataTable;

                if (elements.Contains(element))
                    return null;

                StiComponentProgressHelper.Add(element);
                elements.Add(element);

                InitWorker(option);

                return null;
            }
        }

        public static IStiPivotGridContainer GetOrCreatePivotWithProgress(IStiPivotTableElement element, IStiPivotTableCreator creator,
            StiDataRequestOption option = StiDataRequestOption.All)
        {
            lock (lockObject)
            {
                if (StiPivotToContainerCache.Contains(element) && Get(element) != null)
                    return StiPivotToContainerCache.Get(element);

                if (elements.Contains(element))
                    return null;

                pivotCreator = creator;

                StiComponentProgressHelper.Add(element);
                elements.Add(element);

                InitWorker(option);

                return null;
            }
        }

        public static StiDataTable Get(IStiElement element)
        {
            lock (lockCacheObject)
            {
                var key = GetKey(element);
                return cache.ContainsKey(key) ? cache[key] : null;
            }
        }

        public static StiDataTable Create(IStiElement element, StiDataRequestOption option)
        {
            var skipTransform = (option & StiDataRequestOption.DisallowTransform) > 0;
            var dashboard = element.Page as IStiDashboard;

            var meters = element is IStiPivotTableElement pivotTable ? pivotTable.GetUsedMeters() : element.GetMeters();
            if (meters == null || !meters.Any())
                return StiDataTable.NullTable;

            var group = StiGroupElementHelper.GetGroup(element);
            var userFilters = GetUserFilters(element, dashboard);
            var userSorts = GetUserSorts(element, option);

            var dataFilters = GetDataFilters(element);

            var transformActions = skipTransform ? null : GetTransformActions(element);
            var transformFilters = skipTransform ? null : GetTransformFilters(element);
            var transformSorts = skipTransform ? null : GetTransformSorts(element);

            var drillDownFilters = GetDrillDownFilters(element);

            var query = StiDataJoiner.JoinEngine == StiDataJoiner.StiDataJoinEngine.V4 || StiDataJoiner.JoinEngine == StiDataJoiner.StiDataJoinEngine.V5 ? element : dashboard;

            var crossFiltering = element as IStiCrossFiltering;
            if (crossFiltering != null && !crossFiltering.CrossFiltering)
            {
                group = string.Empty;
                userFilters = null;
            }

            return StiDataAnalyzer.Analyze(query, group, meters, option,
                userSorts, userFilters, dataFilters, null,
                transformSorts, transformFilters, transformActions, drillDownFilters);
        }

        internal static void Add(IStiElement element, StiDataTable dataTable)
        {
            lock (lockCacheObject)
            {
                var key = GetKey(element);
                cache[key] = dataTable;
            }
        }

        private static void InitWorker(StiDataRequestOption option = StiDataRequestOption.All)
        {
            if (worker != null) return;

            worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                while (true)
                {
                    if (elements.Count == 0)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    IStiElement element;

                    lock (lockObject)
                    {
                        element = elements.FirstOrDefault();
                        elements.Remove(element);
                    }

                    try
                    {
                        var dataTable = Get(element);
                        if (dataTable == null)
                        {
                            dataTable = Create(element, option);
                            Add(element, dataTable);

                            if (element is IStiPivotTableElement) //Clear cache for this element
                                StiPivotToContainerCache.Remove(element as IStiPivotTableElement);
                        }

                        if (element is IStiPivotTableElement)
                            pivotCreator.Create(element as IStiPivotTableElement, dataTable);
                    }
                    catch (Exception)
                    {
                        Add(element, StiDataTable.NullTable);
                    }
                    finally
                    {
                        StiComponentProgressHelper.Remove(element);
                        DataInit?.Invoke(element, EventArgs.Empty);
                    }
                }
            };
            worker.RunWorkerAsync();
        }

        public static string GetKey(IStiElement element)
        {
            var appKey = StiAppKey.GetOrGeneratedKey(element) ?? string.Empty;

            var meters = element?.GetMeters();
            if (meters == null || !meters.Any())
                return $"{appKey}.0";

            var dashboard = element.Page as IStiDashboard;
            var index = 0;
            var keys = meters.Select(m => index++.ToString() + m.GetUniqueCode().ToString()).ToList();
            var userFilters = dashboard?.GetUserFilters(element);
            var userFilterKeys = userFilters?.Select(s => index++.ToString() + s.GetUniqueCode().ToString());

            if (userFilterKeys != null)
                keys.AddRange(userFilterKeys);

            var dataFilters = GetDataFilters(element);
            var dataFilterKeys = dataFilters?.Select(s => index++.ToString() + s.GetUniqueCode().ToString());

            if (dataFilterKeys != null)
                keys.AddRange(dataFilterKeys);

            var drillDownFilters = GetDrillDownFilters(element);
            var drillDownFilterKeys = drillDownFilters?.Select(s => index++.ToString() + s.GetUniqueCode().ToString());

            if (drillDownFilterKeys != null)
                keys.AddRange(drillDownFilterKeys);

            var sortKeys = (element as IStiUserSorts)?.UserSorts.Select(s => index++.ToString() + s.GetUniqueCode().ToString());
            if (sortKeys != null)
                keys.AddRange(sortKeys);

            if (element is IStiPivotTableElement)
            {
                var title = (element as IStiTitleElement)?.Title?.Text ?? string.Empty;
                keys.Add(title.GetHashCode().ToString());
                keys.AddRange((element as IStiPivotTableElement).PivotTableConditions.Select(c => c.GetUniqueCode().ToString()).ToList());
                keys.Add((element as IStiPivotTableElement).SummaryDirection.ToString());
            }

            var hashKeys = 0L;
            foreach (var key in keys)
            {
                hashKeys = unchecked(hashKeys + key.GetHashCode());
            }

            var expressionsHash = StiDataFilterRuleHelper.GetFilterRulesHash(dashboard?.GetApp(), userFilters);
            hashKeys = unchecked(hashKeys + expressionsHash);

            return $"{appKey}.{hashKeys}";
        }

        public static void CleanCache(string reportKey)
        {
            lock (lockObject)
            {
                if (reportKey == null)
                {
                    cache.Clear();
                }
                else
                {
                    cache.Keys
                        .Where(k => k.StartsWith(reportKey))
                        .ToList()
                        .ForEach(k => cache.Remove(k));
                    
                }
            }
        }

        public static void Remove(string key)
        {
            lock (lockObject)
            {
                if (cache.ContainsKey(key))
                    cache.Remove(key);
            }
        }
        #endregion

        #region Methods.Helpers
        private static List<StiDataFilterRule> GetUserFilters(IStiElement element, IStiDashboard dashboard)
        {
            return dashboard?.GetUserFilters(element);
        }

        private static List<StiDataSortRule> GetUserSorts(IStiElement element, StiDataRequestOption option)
        {
            return (option & StiDataRequestOption.AllowDataSort) > 0 ? (element as IStiUserSorts)?.UserSorts.ToList() : null;
        }

        private static List<StiDataFilterRule> GetDataFilters(IStiElement element)
        {
            return (element as IStiDataFilters)?.DataFilters;
        }

        private static List<StiDataActionRule> GetTransformActions(IStiElement element)
        {
            return (element as IStiTransformActions)?.TransformActions;
        }

        private static List<StiDataFilterRule> GetTransformFilters(IStiElement element)
        {
            return (element as IStiTransformFilters)?.TransformFilters;
        }

        private static List<StiDataSortRule> GetTransformSorts(IStiElement element)
        {
            return (element as IStiTransformSorts)?.TransformSorts;
        }

        private static List<StiDataFilterRule> GetDrillDownFilters(IStiElement element)
        {
            return (element as IStiDrillDownElement)?.DrillDownFilters;
        }
        #endregion
    }
}