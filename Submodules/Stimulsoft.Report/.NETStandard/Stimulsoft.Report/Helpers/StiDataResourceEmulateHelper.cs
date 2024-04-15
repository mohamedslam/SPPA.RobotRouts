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
using Stimulsoft.Data.Extensions;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Helpers
{
    public class StiDataResourceEmulateHelper
    {
        #region Constants
        private const int rowsCount = 5;
        #endregion

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
            #region Remove data
            var listDatabases = new List<StiDatabase>();
            foreach(StiDatabase database in dictionary.Databases)
            {
                if (DatabaseFromRecource(database)) continue;
                listDatabases.Add(database);
            }

            var listDataSources = new List<StiDataSource>();
            foreach (StiDataSource dataSource in dictionary.DataSources)
            {
                //var categoryName = dataSource.GetCategoryName();
                //if (listDatabases.Any(x => x.Name == categoryName))
                if (!DatabaseFromRecource(dataSource.GetConnection() as StiDatabase))
                {
                    listDataSources.Add(dataSource);
                }
            }

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

            #region Copy calculated columns
            foreach (var dataSource in listDataSources)
            {
                var ds = dictionary.DataSources[dataSource.Name];
                if (ds != null)
                {
                    foreach (var dc in dataSource.Columns)
                    {
                        var cdc = dc as StiCalcDataColumn;
                        if (cdc != null)
                        {
                            ds.Columns.Add(new StiCalcDataColumn(cdc.Name, cdc.Alias, cdc.Type, cdc.Value));
                        }
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

            foreach (StiDataSource dataSource in dictionary.DataSources)
            {
                if (DatabaseFromRecource(dataSource.GetConnection() as StiDatabase)) continue;

                string category = dataSource.GetCategoryName();
                var dataSet = hashCategories[category.ToLowerInvariant()] as DataSet;
                if (dataSet == null)
                {
                    dataSet = new DataSet(category);
                    hashCategories[category.ToLowerInvariant()] = dataSet;
                }

                var dataTable = CreateDataTableFromDataSource(dataSource);
                if (dataTable != null)
                    dataSet.Tables.Add(dataTable);
            }

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

        private static DataTable CreateDataTableFromDataSource(StiDataSource dataSource)
        {
            DataTable dt = new DataTable(dataSource.Name);

            foreach (StiDataColumn dataColumn in dataSource.Columns)
            {
                if (!(dataColumn is StiCalcDataColumn) && !string.IsNullOrWhiteSpace(dataColumn.Name) && !dt.Columns.Contains(dataColumn.Name))
                {
                    Type dataType = dataColumn.Type;

                    //check for external assemblies
                    try
                    {
                        var a = dataColumn.Type.Assembly;
                    }
                    catch
                    {
                        dataType = typeof(object);
                    }

                    if (dataType == typeof(Image)) dataType = typeof(byte[]);

                    if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        dataType = Nullable.GetUnderlyingType(dataType);
                    }

                    DataColumn dc = new DataColumn(dataColumn.Name, dataType);
                    dt.Columns.Add(dc);
                }
            }

            for (int indexRow = 0; indexRow < rowsCount; indexRow++)
            {
                DataRow dr = dt.NewRow();

                foreach (DataColumn dc in dt.Columns)
                {
                    try
                    {
                        if (dc.IsIntegerType())
                        {
                            dr[dc] = indexRow;
                        }
                        else if (dc.IsNumericType())
                        {
                            dr[dc] = 1100 * indexRow;
                        }
                        else if (dc.IsDateType())
                        {
                            dr[dc] = new DateTime(2021 + indexRow, indexRow + 1, indexRow + 1, indexRow + 1, indexRow + 1, indexRow + 1);
                        }
                        else if (dc.DataType == typeof(string))
                        {
                            dr[dc] = $"String {indexRow} with long text {indexRow}{indexRow}{indexRow}{indexRow}{indexRow}";
                        }
                        else if (dc.DataType == typeof(char))
                        {
                            dr[dc] = (char)('A' + indexRow);
                        }
                        else if (dc.DataType == typeof(bool))
                        {
                            dr[dc] = indexRow % 2 == 1;
                        }
                        else if (dc.DataType == typeof(Guid))
                        {
                            //dr[dc] = Guid.NewGuid().ToString().Substring(0, 32) + indexRow.ToString("D4");
                            dr[dc] = new string((char)(48 + indexRow), 32).Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-").Substring(0, 32) + indexRow.ToString("D4");
                        }
                        else if (dc.DataType == typeof(byte[]))
                        {
                            dr[dc] = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAIAAAAC64paAAAASklEQVR42mI0yj7KQC5gYqAAjGqmheazU6yAiBzNEG3GOceI0oxsDx6dQMCINYUhOxKXTpzOhmvAoxMIWHBJ4Nc2msKGmGaAAAMAeJgWiHx8Cd8AAAAASUVORK5CYII=");
                        }
                        else if (dc.DataType == typeof(string[]))
                        {
                            dr[dc] = new string[] { "s1", "s2", "s3", "s4", "s5" };
                        }
                        else if (dc.DataType == typeof(DateTime[]))
                        {
                            dr[dc] = new DateTime[] { DateTime.Today, DateTime.Today.AddDays(1), DateTime.Today.AddDays(2) };
                        }
                        else if (dc.DataType == typeof(Int64[]))
                        {
                            dr[dc] = new Int64[] { 1, 2, 3, 4, 5 };
                        }
                        else if (dc.DataType == typeof(Single[]))
                        {
                            dr[dc] = new Single[] { 1.1f, 2.2f, 3.3f, 4.4f, 5.5f };
                        }
                        else if (dc.DataType == typeof(double[]))
                        {
                            dr[dc] = new double[] { 1.1, 2.2, 3.3, 4.4, 5.5 };
                        }
                        else if (dc.DataType == typeof(decimal[]))
                        {
                            dr[dc] = new decimal[] { 1.1m, 2.2m, 3.3m, 4.4m, 5.5m };
                        }
                        else if (dc.DataType == typeof(object))
                        {
                            dr[dc] = $"Object {indexRow}";
                        }
                        else
                        {
                            //nothing
                        }
                    }
                    catch
                    {
                        //
                    }
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }
        #endregion
    }
}
