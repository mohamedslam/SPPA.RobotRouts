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
    public static class StiDataSourceChainFinder
    {
        public static IEnumerable<IStiAppDataSource> Find(IEnumerable<IStiAppDataSource> dataSources)
        {
            dataSources = dataSources.Where(d => d != null);
            return dataSources.SelectMany(primaryTable =>
            {
                return dataSources
                    .Where(d => d != primaryTable)
                    .Select(t => Find(primaryTable, t))
                    .Where(t => t != null)
                    .SelectMany(t => t)
                    .Where(t => !dataSources.Contains(t))
                    .Distinct();

            }).Union(dataSources).Distinct();
        }

        public static IEnumerable<IStiAppDataSource> Find(IEnumerable<IStiAppDataSource> dataSources, string table1, string table2)
        {
            var t1 = dataSources.FirstOrDefault(d => d.GetName() == table1);
            var t2 = dataSources.FirstOrDefault(d => d.GetName() == table2);

            return Find(new List<IStiAppDataSource> { t1, t2 });
        }

        private static IEnumerable<IStiAppDataSource> Find(IStiAppDataSource dataSource1, IStiAppDataSource dataSource2)
        {
            var parentPath = FindInParent(dataSource1, dataSource2);
            var childPath = FindInChild(dataSource1, dataSource2);

            if (parentPath == null && childPath == null)
                return FindInBoth(dataSource1, dataSource2);

            if (parentPath != null && childPath == null)
                return parentPath;

            if (parentPath == null && childPath != null)
                return childPath;

            return parentPath.Count() >= childPath.Count() ? childPath : parentPath;
        }

        public static IEnumerable<IStiAppDataSource> FindInParent(IStiAppDataSource dataSource1, IStiAppDataSource dataSource2,
            List<string> dataPath = null)
        {
            #region Loop guard
            if (dataPath == null)
                dataPath = new List<string>();

            var dataPoint = GetDataPoint(dataSource1, dataSource2);
            if (dataPath.Contains(dataPoint))
                return null;

            dataPath.Add(dataPoint);
            #endregion

            var parentRelations = dataSource1.FetchParentRelations(true);
            if (!parentRelations.Any())
                return null;

            var directRelations = parentRelations.Where(r => r.GetParentDataSource() == dataSource2);
            if (directRelations.Any())
                return new List<IStiAppDataSource> { dataSource1, dataSource2 };

            foreach (var relation in GetActiveRelations(parentRelations))
            {
                var path = FindInParent(relation.GetParentDataSource(), dataSource2)?.ToList();
                if (path == null) continue;

                path.Insert(0, relation.GetChildDataSource());
                return path;
            }

            return null;
        }

        public static IEnumerable<IStiAppDataSource> FindInChild(IStiAppDataSource dataSource1, IStiAppDataSource dataSource2, 
            List<string> dataPath = null)
        {
            #region Loop guard
            if (dataPath == null)
                dataPath = new List<string>();

            var dataPoint = GetDataPoint(dataSource1, dataSource2);
            if (dataPath.Contains(dataPoint))
                return null;

            dataPath.Add(dataPoint);
            #endregion

            var childRelations = dataSource1.FetchChildRelations(true);
            if (!childRelations.Any())
                return FindInParent(dataSource1, dataSource2);//Try to find in parent (only for Child!)
            
            var onceRelation = childRelations.FirstOrDefault(r => r.GetChildDataSource() == dataSource2);
            if (onceRelation != null)
                return new List<IStiAppDataSource> { dataSource1, dataSource2 };

            foreach (var relation in GetActiveRelations(childRelations))
            {
                var path = FindInChild(relation.GetChildDataSource(), dataSource2, dataPath)?.ToList();
                if (path == null) continue;

                path.Insert(0, relation.GetParentDataSource());
                return path;
            }

            return null;
        }

        public static IEnumerable<IStiAppDataSource> FindInBoth(IStiAppDataSource dataSource1, IStiAppDataSource dataSource2)
        {
            //At this moment we check only one level
            var relations1 = dataSource1.FetchParentRelations(true);
            var relations2 = dataSource2.FetchParentRelations(true);

            foreach (var rel1 in relations1)
            {
                var dataSource = relations2.FirstOrDefault(rel2 => rel1.GetParentDataSource() == rel2.GetParentDataSource());
                if (dataSource != null)
                    return new List<IStiAppDataSource> { rel1?.GetParentDataSource() };                
            }

            return null;
        }

        private static string GetDataPoint(IStiAppDataSource dataSource1, IStiAppDataSource dataSource2)
        {
            return dataSource1.GetName() + dataSource2.GetName();
        }

        private static IEnumerable<IStiAppDataRelation> GetActiveRelations(IEnumerable<IStiAppDataRelation> childRelations)
        {
            if (childRelations.Any(r => r.GetActiveState()))
                return childRelations.Where(r => r.GetActiveState());
            else
                return childRelations;
        }
    }
}