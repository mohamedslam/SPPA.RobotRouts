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
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Data.Engine
{
    public static class StiSqlDataAnalyzer
    {
        #region Methods
        public static DataTable FetchAndJoin(IStiQueryObject query, string group, IEnumerable<string> filterNames, 
            IEnumerable < StiDataFilterRule> dataFilters, IStiApp app)
        {
            var sql = BuildSqlQuery(query, group, filterNames, dataFilters, app);
            return StiBIDataCacheHelper.RunQuery(sql);
        }

        public static string BuildSqlQuery(IStiQueryObject query, string group, IEnumerable<string> filterNames, 
            IEnumerable<StiDataFilterRule> dataFilters, IStiApp app)
        {
            var appKey = StiAppKey.GetOrGeneratedKey(app);
            var dataSources = StiDataPicker.RetrieveUsedDataSources(query, group,filterNames);
            var dataNames = dataSources.Select(d => $"{d.GetKey()}").ToList();
            var realDataNames = dataSources.Select(StiBIDataCacheHelper.GetTableName).ToList();

            var links = StiDataLinkHelper.GetLinks(app.GetDictionary());
            var selectQuery = BuildSelectQuery(dataSources, app);

            var fromQuery = dataNames.FirstOrDefault();
            var path = StiDataJoiner.FindPath(realDataNames, links);
            var joinQuery = BuildJoinQuery(path, realDataNames, dataNames);

            if (!string.IsNullOrWhiteSpace(joinQuery))
            {
                var fromIndex = realDataNames.IndexOf(path.FirstOrDefault().ParentTable);
                fromQuery = dataNames[fromIndex];
            }

            //var filter = StiDataFilterRuleHelper.GetDataTableFilterQuery(dataFilters.ToList(), columnNames, columnTypes, report);
            return $"SELECT {selectQuery} FROM [{fromQuery}] {joinQuery}";
        }

        public static string BuildJoinQuery(List<StiDataLink> path, List<string> realDataNames, List<string> dataNames)
        {
            if (path == null || !path.Any())
                return "";

            var parts = path.Select(r =>
            {
                var parentIndex = realDataNames.IndexOf(r.ParentTable);
                var childIndex = realDataNames.IndexOf(r.ChildTable);
                var parentTable = dataNames[parentIndex];
                var childTable = dataNames[childIndex];

                var parentField = $"[{parentTable}].[{r.ParentColumns.FirstOrDefault()}]";
                var childField = $"[{childTable}].[{r.ChildColumns.FirstOrDefault()}]";

                return $"LEFT JOIN [{childTable}] ON {parentField} = {childField}";
            });

            return string.Join(" ", parts);
        }

        public static string BuildSelectQuery(IEnumerable<IStiAppDataSource> dataSources, IStiApp app)
        {
            var appKey = StiAppKey.GetOrGeneratedKey(app);
            var tableNames = dataSources.Select(StiBIDataCacheHelper.GetTableName).ToList();
            var tableIndex = 0;
            var parts = dataSources.Select(d =>
            {
                var tableName = tableNames[tableIndex++];
                var columns = d.FetchColumns().Select(c =>
                {
                    var columnName = c.GetNameInSource();
                    return $"[{d.GetKey()}].[{columnName}] as [{tableName}.{columnName}]";
                });
                return string.Join(", ", columns);
            });

            return string.Join(", ", parts);
        }
        #endregion
    }
}