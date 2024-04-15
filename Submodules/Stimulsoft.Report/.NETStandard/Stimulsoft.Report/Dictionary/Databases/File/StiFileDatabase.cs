#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dictionary.Design;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiCsvDatabaseConverter))]
    public abstract class StiFileDatabase : StiDatabase
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiFileDatabase;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Name(),
                propHelper.Alias(),

                propHelper.PathData()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiJsonDatabase
            jObject.AddPropertyStringNullOrEmpty("PathData", PathData);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "PathData":
                        this.PathData = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        internal StiDataSchema DataSchema { get; set; } = null;

        /// <summary>
        /// Gets or sets a path to the json data.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets a path to the xml data.")]
        [StiOrder((int)Order.PathData)]
		public string PathData { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns full database information.
        /// </summary>
        public override StiDatabaseInformation GetDatabaseInformation(StiReport report)
        {
            DataSet dataSet = null;

            try
            {
                if (DataSchema == null)
                    DataSchema = StiDataLeader.RetrieveSchema(CreateFileConnector(), GetConnectorOptions(report, true));

                if (DataSchema != null) 
                    dataSet = DataSchema.GetDataSet();
            }
            catch
            {
                if (AllowException) throw;
            }

            return dataSet == null ? null : new StiDatabaseInformation(dataSet.Tables) { Relations = dataSet.Relations.Cast<DataRelation>().ToList() };
        }

        public async Task CreateDataSourcesAsync(StiDictionary dictionary)
        {
            await Task.Run(() => { CreateDataSources(dictionary); });
        }

        public void CreateDataSources(StiDictionary dictionary)
        {
            var information = GetDatabaseInformation(dictionary.Report);
            var tables = information.Tables.Select(t => CreateDataTableSource(dictionary, t)).ToList();
                
            tables.ForEach(dictionary.DataSources.Add);
        }

        private StiDataTableSource CreateDataTableSource(StiDictionary dictionary, DataTable table)
        {
            var tableName = StiNameCreation.CreateDataSourcesName(dictionary.Report, table.TableName);
            return new StiDataTableSource
            {
                Dictionary = dictionary,
                NameInSource = $"{Name}.{table.TableName}",
                Name = tableName,
                Alias = tableName,
                Columns = new StiDataColumnsCollection(table.Columns.Cast<DataColumn>().ToList())
            };
        }

        protected string ParsePath(string path, StiReport report)
        {
            if (string.IsNullOrEmpty(path) || !path.Contains("{") || !path.Contains("}"))
                return path;

            try
            {
                var page = new Components.StiPage(report);
                var textComp = new Components.StiText();
                page.Components.Add(textComp);

                return Engine.StiParser.ParseTextValue(path, textComp) as string;
            }
            catch
            {
            }

            return path;
        }

        protected virtual void RegDataSetInDataStore(StiDictionary dictionary, DataSet dataSet)
        {
            if (dataSet == null) return;

            dataSet.DataSetName = Name;
            foreach (DataTable table in dataSet.Tables)
            {
                var data = new StiData($"{Name}.{table.TableName}", table)
                {
                    IsReportData = true 
                };
                dictionary.DataStore[data.Name] = data;
            }
        }

        /// <summary>
        /// Returns new file connector for this database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public StiFileDataConnector CreateFileConnector()
        {
            return CreateConnector() as StiFileDataConnector;
        }

        /// <summary>
        /// Adds tables, views and stored procedures to report dictionary from database information.
        /// </summary>
        public override void ApplyDatabaseInformation(StiDatabaseInformation information, StiReport report, StiDatabaseInformation informationAll)
        {
            if (information == null) return;

            var tableNames = new Hashtable();
            foreach (var dataTable in information.Tables)
            {
                var source = new StiDataTableSource($"{Name}.{dataTable.TableName}",
                    StiNameCreation.CreateName(report, dataTable.TableName, false, false, true));
                dataTable.TableName = source.Name;

                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    source.Columns.Add(new StiDataColumn(dataColumn.ColumnName, dataColumn.DataType));
                }
                tableNames[dataTable.TableName] = source.Name;
                report.Dictionary.DataSources.Add(source);
            }

            foreach (var dataRelation in information.Relations)
            {
                var parentColumns = dataRelation.ParentColumns.Select(r => r.ColumnName);
                var childColumns = dataRelation.ChildColumns.Select(r => r.ColumnName);

                var parentTableName = tableNames[dataRelation.ParentTable.TableName] as string;
                if (parentTableName == null) continue;

                var childTableName = tableNames[dataRelation.ChildTable.TableName] as string;
                if (childTableName == null) continue;

                var parentSource = !string.IsNullOrEmpty(parentTableName) && report.Dictionary.DataSources.Contains(parentTableName)
                    ? report.Dictionary.DataSources[parentTableName]
                    : null;
                if (parentSource == null) continue;

                var childSource = !string.IsNullOrEmpty(childTableName) && report.Dictionary.DataSources.Contains(childTableName)
                    ? report.Dictionary.DataSources[childTableName]
                    : null;
                if (childSource == null) continue;

                if (!parentColumns.All(c => parentSource.Columns[c] != null)) continue;
                if (!childColumns.All(c => childSource.Columns[c] != null)) continue;

                var relation = new StiDataRelation(
                    dataRelation.RelationName,
                    parentSource,
                    childSource,
                    parentColumns.ToArray(),
                    childColumns.ToArray());

                report.Dictionary.Relations.Add(relation);

                StiDataRelationSetName.SetName(relation, report, dataRelation.DataSet, dataRelation.RelationName);
            }
        }

        public override void ApplyDatabaseInformation(StiDatabaseInformation information, StiReport report)
        {
            ApplyDatabaseInformation(information, report, null);
        }

        /// <summary>
        /// Registers the database in dictionary.
        /// </summary>
        /// <param name="dictionary">Dictionary in which is registered database.</param>
        /// <param name="loadData">Load the data or no.</param>
        public override void RegData(StiDictionary dictionary, bool loadData)
        {
            DataSet dataSet = null;

            try
            {
                if (loadData || DataSchema == null)
                {
                    var options = GetConnectorOptions(dictionary.Report, !loadData);
                    if (options != null)
                    {
                        if (loadData)
                        {
                            if (this is StiCsvDatabase || this is StiDBaseDatabase)
                            {
                                dataSet = new DataSet { EnforceConstraints = false };

                                var loadDataSet = StiDataLeader.GetDataSet(CreateFileConnector(), options);
                                var tables = new DataTable[loadDataSet.Tables.Count];
                                loadDataSet.Tables.CopyTo(tables, 0);
                                foreach (DataTable table in tables)
                                {
                                    loadDataSet.Tables.Remove(table);
                                    dataSet.Tables.Add(StiDataAdapterHelper.Fill(dictionary, table));
                                }
                            }
                            else
                            {
                                dataSet = StiDataAdapterHelper.Fill(dictionary, StiDataLeader.GetDataSet(CreateFileConnector(), options), this);
                            }
                        }

                        DataSchema = StiDataLeader.RetrieveSchema(CreateFileConnector(), options);
                        if (dataSet == null) dataSet = options.DataSet;
                    }
                }
                else
                {
                    dataSet = DataSchema.GetDataSet();
                }

            }
            catch (Exception e)
            {
                if (!StiRenderingMessagesHelper.WriteConnectionException(dictionary, Name, e)) throw;
            }

            RegDataSetInDataStore(dictionary, dataSet);
        }

        protected virtual StiFileDataOptions GetConnectorOptions(StiReport report, bool isShema)
        {
            return null;
        }
        #endregion

        public StiFileDatabase()
            : this(string.Empty, string.Empty, string.Empty)
		{
		}

		public StiFileDatabase(string name, string pathData) : base(name)
		{
			this.PathData = pathData;
		}

        public StiFileDatabase(string name, string pathData, string key)
            : base(name, name, key)
        {
            this.PathData = pathData;
        }
	}
}
