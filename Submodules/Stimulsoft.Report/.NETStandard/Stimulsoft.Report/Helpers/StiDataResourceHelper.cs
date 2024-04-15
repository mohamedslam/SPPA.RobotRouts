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
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stimulsoft.Report.Helpers
{
    public class StiDataResourceHelper
    {
        #region Methods
        public static async Task SaveSnapshotAsync(IStiApp app)
        {
            await Task.Run(() => SaveSnapshot(app));
        }

        public static void SaveSnapshot(IStiApp app)
        {
            try
            {
                var dictionary = app.GetDictionary() as StiDictionary;
                if (dictionary == null) return;

                var listResourceSnapshot = GetListResourceSnapshot(app, dictionary);
                listResourceSnapshot.ForEach(dictionary.Resources.Add);

                var hashRelation = GetRelationInformation(dictionary);

                ReplaceDatabase(dictionary, listResourceSnapshot);
                SaveRelationSnapshot(dictionary, hashRelation);
            }
            catch (Exception ee)
            {
                StiExceptionProvider.Show(ee);
            }
        }

        private static Hashtable GetRelationInformation(StiDictionary dictionary)
        {
            var hash = new Hashtable();

            foreach (StiDataRelation relation in dictionary.Relations)
            {
                if (relation.ParentSource == null || relation.ChildSource == null ||
                    DatabaseFromRecource(relation.ParentSource.GetConnection() as StiDatabase) ||
                    DatabaseFromRecource(relation.ChildSource.GetConnection() as StiDatabase))
                    continue;

                var key = new Tuple<string, string>(relation.ParentSource.Name, relation.ChildSource.Name);
                var valueRelation = relation.Clone() as StiDataRelation;
                valueRelation.ParentSource = null;
                valueRelation.ChildSource = null;

                hash[key] = valueRelation;
            }

            return hash;
        }

        private static void SaveRelationSnapshot(StiDictionary dictionary, Hashtable hashRelation)
        {
            foreach (Tuple<string, string> key in hashRelation.Keys)
            {
                var parentSourceName = key.Item1;
                var childSourceName = key.Item2;

                var relation = hashRelation[key] as StiDataRelation;

                relation.ParentSource = dictionary.DataSources.Items.FirstOrDefault(x => x.Name == parentSourceName);
                relation.ChildSource = dictionary.DataSources.Items.FirstOrDefault(x => x.Name == childSourceName);

                dictionary.Relations.Add(relation);
            }
        }

        private static void ReplaceDatabase(StiDictionary dictionary, List<StiResource> listResource)
        {
            #region Prepare Data
            var calcColumnHash = new Hashtable();

            var listDatabases = new List<StiDatabase>();
            foreach(StiDatabase database in dictionary.Databases)
            {
                if (DatabaseFromRecource(database)) continue;
                listDatabases.Add(database);
            }

            var listDataSources = new List<StiDataSource>();
            foreach (StiDataSource dataSource in dictionary.DataSources)
            {
                var categoryName = dataSource.GetCategoryName();
                if (listDatabases.Any(x => x.Name == categoryName))
                {
                    var calcColumns = dataSource.FetchColumns().Where(c => c is IStiAppCalcDataColumn);
                    if (calcColumns.Count() > 0)
                        calcColumnHash.Add(dataSource.Name, calcColumns);

                    listDataSources.Add(dataSource);
                }
            }
            #endregion

            #region Remove data
            foreach (var dataBase in listDatabases)
                dictionary.Databases.Remove(dataBase);

            foreach (var dataSource in listDataSources)
                dictionary.DataSources.Remove(dataSource);
            #endregion

            #region Add Data
            foreach (StiResource resource in listResource)
            {
                var dataBase = resource.CreateFileDatabase();
                dataBase.Name = resource.Alias;
                dictionary.Databases.Add(dataBase);

                var information = dataBase.GetDatabaseInformation(dictionary.Report);

                dataBase.ApplyDatabaseInformation(information, dictionary.Report);
            }
            #endregion

            #region Add Calc Column
            foreach (StiDataSource dataSource in dictionary.DataSources)
            {
                if (calcColumnHash.ContainsKey(dataSource.Name))
                {
                    var calColumnList = calcColumnHash[dataSource.Name] as IEnumerable;
                    foreach (var calColumn in calColumnList)
                    {
                        dataSource.Columns.Add(calColumn as StiCalcDataColumn);
                    }                    
                }                
            }
            #endregion
        }

        private static bool DatabaseFromRecource(StiDatabase database)
        {
            var fileDatabase = database as StiFileDatabase;
            if (fileDatabase != null)
            {
                return fileDatabase.PathData.StartsWith("resource://");
            }
            return false;
        }

        private static List<StiResource> GetListResourceSnapshot(IStiApp app, StiDictionary dictionary)
        {
            var hashCategories = new Hashtable();

            dictionary.Connect(true);

            foreach (StiDataSource dataSource in dictionary.DataSources)
            {
                if (DatabaseFromRecource(dataSource.GetConnection() as StiDatabase) || dataSource is StiDataTransformation)
                    continue;

                string category = dataSource.GetCategoryName();
                var dataSet = hashCategories[category.ToLowerInvariant()] as DataSet;
                if (dataSet == null)
                {
                    dataSet = new DataSet(category);
                    hashCategories[category.ToLowerInvariant()] = dataSet;
                }

                
                var dataTable = StiDataPicker.GetDataTable(StiDataRequestOption.All, dataSource);
                if (dataTable != null)
                {
                    #region Remove Calc Column
                    var calcColumns = dataSource.FetchColumns().Where(c => c is IStiAppCalcDataColumn).ToList();                    

                    foreach (var column in calcColumns)
                        dataTable.Columns.Remove(((StiCalcDataColumn)column).Name);
                    #endregion

                    dataSet.Tables.Add(dataTable);
                }
            }

            dictionary.Disconnect();

            var listResource = new List<StiResource>();
            foreach (DataSet dataSet in hashCategories.Values)
            {
                listResource.Add(DataSetToResourceXml(app, dataSet));
            }

            return listResource;
        }

        private static StiResource DataSetToResourceXml(IStiApp app, DataSet dataSet)
        {
            var dict = app.GetDictionary() as StiDictionary;

            StiResource resource;
            using (var stream = new MemoryStream())
            {
                dataSet.WriteXml(stream, XmlWriteMode.WriteSchema);

                var bytes = stream.ToArray();

                resource = new StiResource(GetNewResourceName(dataSet.DataSetName, dict.Report), dataSet.DataSetName, StiResourceType.Xml, bytes);
            }
            return resource;
        }

        private static string GetNewResourceName(string name, StiReport report)
        {
            if (StiNameCreation.IsResourceNameExists(report, name))
            {
                name = StiNameCreation.CreateResourceName(report, name);
                return GetNewResourceName(name, report);
            }

            return name;
        }
        #endregion
    }
}
