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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Exceptions;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.SaveLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stimulsoft.Report.Dictionary
{

    /// <summary>
    /// Class describes the dictionary data.
    /// </summary>
    [Serializable]
    public class StiDictionary :
        ICloneable,
        IStiStateSaveRestore,
        IStiJsonReportObject,
        IStiAppDictionary
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyJObject("Resources", Resources.SaveToJsonObjectEx(mode));
            jObject.AddPropertyJObject("Variables", Variables.SaveToJsonObjectEx(mode));
            jObject.AddPropertyJObject("DataSources", DataSources.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Databases", Databases.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("BusinessObjects", BusinessObjects.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Relations", Relations.SaveToJsonObject(mode));

            if (jObject.Count == 0)
                return null;

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Resources":
                        this.Resources.LoadFromJsonObjectEx((JObject)property.Value, Report);
                        break;

                    case "Variables":
                        this.Variables.LoadFromJsonObjectEx((JObject)property.Value, Report);
                        break;

                    case "DataSources":
                        this.DataSources.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Databases":
                        this.Databases.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "BusinessObjects":
                        this.BusinessObjects.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Relations":
                        this.Relations.LoadFromJsonObject((JObject)property.Value);
                        break;

                }
            }
        }
        #endregion

        #region IStiStateSaveRestore
        /// <summary>
		/// Saves the current state of an object.
		/// </summary>
		/// <param name="stateName">A name of the state being saved.</param>
		public virtual void SaveState(string stateName)
        {
            foreach (StiDataSource data in this.DataSources)
                data.SaveState(stateName);
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public virtual void RestoreState(string stateName)
        {
            foreach (StiDataSource data in this.DataSources)
                data.RestoreState(stateName);
        }

        /// <summary>
        /// Clears all earlier saved object states.
        /// </summary>
        public virtual void ClearAllStates()
        {
            foreach (StiDataSource data in this.DataSources)
                data.ClearAllStates();
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var dictionary = new StiDictionary();

            using (var stream = new MemoryStream())
            {
                this.Save(stream);

                stream.Seek(0, SeekOrigin.Begin);
                dictionary.Load(stream);
                dictionary.DataStore = new StiDataCollection();
            }

            return dictionary;
        }
        #endregion

        #region IStiAppDictionary
        /// <summary>
        /// Returns an enumeration of the data source from this dictionary.
        /// </summary>
        /// <returns>The enumeration of the data source.</returns>
        IEnumerable<IStiAppDataSource> IStiAppDictionary.FetchDataSources()
        {
            lock (DataSources)
            {
                return DataSources.Cast<IStiAppDataSource>().ToList();
            }
        }

        /// <summary>
        /// Returns an enumeration of the data relations from this dictionary.
        /// </summary>
        /// <returns>The enumeration of the data relations.</returns>
        IEnumerable<IStiAppDataRelation> IStiAppDictionary.FetchDataRelations()
        {
            lock (Relations)
            {
                return Relations.Cast<IStiAppDataRelation>().ToList();
            }
        }

        /// <summary>
        /// Returns an enumeration of the variables from this dictionary.
        /// </summary>
        /// <returns>The enumeration of the variables.</returns>
        IEnumerable<IStiAppVariable> IStiAppDictionary.FetchVariables()
        {
            lock (Variables)
            {
                return Variables.Cast<IStiAppVariable>().ToList();
            }
        }

        /// <summary>
        /// Returns datasource from the data dictionary by its name.
        /// </summary>
        /// <param name="name">A name of the datasource.</param>
        /// <returns>The datasource from the data dictionary. Returns null, if datasource with specified name is not exists.</returns>
        IStiAppDataSource IStiAppDictionary.GetDataSourceByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return DataSources[name];
        }

        /// <summary>
        /// Returns data column from the data dictionary by its name.
        /// </summary>
        /// <param name="name">A name of the data column.</param>
        /// <returns>The data column from the data dictionary. Returns null, if data column with specified name is not exists.</returns>
        IStiAppDataColumn IStiAppDictionary.GetColumnByName(string name)
        {
            return StiDataColumn.GetDataColumnFromColumnName(this, name, true);
        }

        /// <summary>
        /// Returns variable from the data dictionary by its name.
        /// </summary>
        /// <param name="name">A name of the variable.</param>
        /// <returns>The variable from the data dictionary. Returns null, if variable with specified name not exists.</returns>
        IStiAppVariable IStiAppDictionary.GetVariableByName(string name)
        {
            name = StiVariableExpressionHelper.GetSimpleName(name);

            if (string.IsNullOrWhiteSpace(name))
                return null;

            var variable = Variables.Cast<StiVariable>()
                .Where(v => !string.IsNullOrWhiteSpace(v.Name))
                .FirstOrDefault(v => v.Name.Trim().ToLowerInvariant() == name);

            if (variable != null)
                return variable;

            if (name.EndsWith(".from"))
            {
                name = name.Substring(0, name.Length - ".from".Length);

                return Variables.Cast<StiVariable>()
                    .Where(v => !string.IsNullOrWhiteSpace(v.Name))
                    .FirstOrDefault(v => v.Name.Trim().ToLowerInvariant() == name);
            }

            if (name.EndsWith(".to"))
            {
                name = name.Substring(0, name.Length - ".to".Length);

                return Variables.Cast<StiVariable>()
                    .Where(v => !string.IsNullOrWhiteSpace(v.Name))
                    .FirstOrDefault(v => v.Name.Trim().ToLowerInvariant() == name);
            }

            return null;
        }

        /// <summary>
        /// Returns a value from the variable by its name.
        /// </summary>
        /// <param name="name">A name of the variable.</param>
        /// <returns>A value which contains in the variable.</returns>
        object IStiAppDictionary.GetVariableValueByName(string name)
        {
            name = StiVariableExpressionHelper.GetSimpleName(name);

            if (string.IsNullOrWhiteSpace(name))
                return null;

            var variable = (this as IStiAppDictionary).GetVariableByName(name) as StiVariable;
            if (variable != null && Range.IsRangeType(variable.Type))
            {
                if (name.EndsWith(".from"))
                {
                    name = name.Substring(0, name.Length - ".from".Length);

                    var range = GetVariableValueInternal(name) as Range;
                    return range?.FromObject;
                }

                if (name.EndsWith(".to"))
                {
                    name = name.Substring(0, name.Length - ".to".Length);

                    var range = GetVariableValueInternal(name) as Range;
                    return range?.ToObject;
                }
            }

            return GetVariableValueInternal(name);
        }

        private object GetVariableValueInternal(string name)
        {
            if (Report?.CalculationMode == StiCalculationMode.Interpretation || StiOptions.Engine.ForceInterpretationMode)
            {
                var variable = (this as IStiAppDictionary).GetVariableByName(name) as StiVariable;
                if (variable != null && variable.ReadOnly && variable.InitBy == StiVariableInitBy.Expression)
                    return StiReportParser.Parse("{" + variable.Value + "}", Report, false);
            }

            if (Report?.CompiledReport != null && Report?.CompiledReport[name] != null)
                return Report?.CompiledReport[name];

            if (Report != null && Report[name] != null)
                return Report[name];

            return (this as IStiAppDictionary).GetVariableByName(name)?.GetValue();
        }

        /// <summary>
        /// Returns true if a specified name is a name of a system variable.
        /// </summary>
        /// <param name="name">The name of the system variable.</param>
        /// <returns>True, if the specified name is the name of system variable.</returns>
        public bool IsSystemVariable(string name)
        {
            return new[]
            {
                "reportname",
                "reportalias",
                "reportauthor",
                "reportdescription",
                "reportcreated",
                "reportchanged",
                "time",
                "today"
            }.Contains(name.ToLowerInvariant());
        }

        /// <summary>
        /// Returns true if a specified variable is a read-only variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>True, if the specified variable is a read-only variable.</returns>
        public bool IsReadOnlyVariable(string name)
        {
            var variable = (this as IStiAppDictionary).GetVariableByName(name) as StiVariable;
            return variable != null && variable.ReadOnly;
        }

        /// <summary>
        /// Returns value of a specified system variable.
        /// </summary>
        /// <param name="name">A name of the system variable.</param>
        /// <returns>The value of the specified system variable.</returns>
        public object GetSystemVariableValue(string name)
        {
            var lowerName = name.ToLowerInvariant();
            switch (lowerName)
            {
                case "reportname":
                    return Report?.ReportName;

                case "reportalias":
                    return Report?.ReportAlias;

                case "reportauthor":
                    return Report?.ReportAuthor;

                case "reportdescription":
                    return Report?.ReportDescription;

                case "reportcreated":
                    return Report?.ReportCreated;

                case "reportchanged":
                    return Report?.ReportChanged;

                case "time":
                    return DateTime.Now;

                case "today":
                    return DateTime.Today;

                default:
                    throw new StiSystemVariableNotRecognizedException(name);

            }
        }

        /// <summary>
        /// Returns reference to the app which contains this dictionary.
        /// </summary>
        /// <returns>A reference to the app.</returns>
        IStiApp IStiAppDictionary.GetApp()
        {
            return Report;
        }

        /// <summary>
        /// Opens specified connections to the data. Opens all connections if none of them is specified.
        /// </summary>
        IEnumerable<IStiAppConnection> IStiAppDictionary.OpenConnections(IEnumerable<IStiAppConnection> connections)
        {
            if (connections == null)
                connections = this.Databases.ToList();

            connections = connections.Where(c => !StiDataConnections.IsConnectionActive(c)).ToList();

            foreach (StiDatabase connection in connections)
            {
                var dataStoreRes = DataStore.ToList();

                connection?.InvokeConnecting();
                StiDataLeader.RegData(connection, this, true);
                connection?.InvokeConnected();

                var diffData = DataStore.ToList().Except(dataStoreRes).Cast<object>().ToList();

                StiDataConnections.RegisterConnection(connection, diffData);
            }

            return connections;
        }

        /// <summary>
        /// Closes all specified connections. Closes all connections if none of them is specified.
        /// </summary>
        void IStiAppDictionary.CloseConnections(IEnumerable<IStiAppConnection> connections)
        {
            if (connections == null)
                connections = this.Databases.ToList();

            foreach (StiDatabase connection in connections)
            {
                connection?.InvokeDisconnecting();

                var datas = StiDataConnections.UnRegisterConnection(connection)?.Cast<StiData>()?.ToList();
                if (datas == null)continue;

                DisconnectingConnectionInDataStore(datas);
                datas.ForEach(d =>
                {
                    if (DataStore.Contains(d))
                        DataStore.Remove(d);
                });
            };            
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public bool IsEmpty
        {
            get 
            {
                return
                    BusinessObjects.Count == 0 &&
                    Variables.Count == 0 &&
                    Databases.Count == 0 &&
                    DataSources.Count == 0 &&
                    Relations.Count == 0 &&
                    DataStore.Count == 0 &&
                    Resources.Count == 0 &&
                    Restrictions.IsDefault;
            }
        }

        [Browsable(false)]
        public string[] ReconnectListForRequestFromUserVariables { get; set; }

        [Browsable(false)]
        public static Hashtable CacheUserNamesAndPasswords { get; set; }

        /// <summary>
        /// If the property is true then all data sources (including SQL sources) connect only with DataSet data, which were passed using RegData method.  
        /// DataNames of data sources are to be the same.
        /// </summary>
        [Browsable(false)]
        public bool UseInternalData { get; set; }

        /// <summary>
        /// Gets or sets collections of dictionary restrictions. 
        /// </summary>
        [Browsable(false)]
        public StiRestrictions Restrictions { get; set; } = new StiRestrictions();

        /// <summary>
        /// Gets or sets Synchronization mode of the Dictionary.
        /// </summary>
        public static StiAutoSynchronizeMode AutoSynchronize
        {
            get
            {
                return StiOptions.Dictionary.AutoSynchronize;
            }
            set
            {
                StiOptions.Dictionary.AutoSynchronize = value;
            }
        }

        /// <summary>
        /// Gets or sets a DataSet for caching.
        /// </summary>
        [Browsable(false)]
        public DataSet CacheDataSet { get; set; }

        /// <summary>
        /// Gets or sets report in which given dictionary is placed.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Reference, StiSerializeTypes.SerializeToAll)]
        [Browsable(false)]
        public StiReport Report { get; set; }

        /// <summary>
        /// Gets or sets DataStore. 
        /// </summary>
        [Browsable(false)]
        public StiDataCollection DataStore { get; set; } = new StiDataCollection();

        /// <summary>
        /// Gets or sets collection of variables.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiVariablesCollection Variables { get; set; } = new StiVariablesCollection();

        /// <summary>
        /// Gets or sets collection of resources.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiResourcesCollection Resources { get; set; } = new StiResourcesCollection();

        /// <summary>
        /// Gets or sets collection of the data sources.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiDataSourcesCollection DataSources { get; set; }

        /// <summary>
        /// Gets or sets collection of the databases.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiDatabaseCollection Databases { get; set; }

        /// <summary>
        /// Gets or sets collection of the business objects.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiBusinessObjectsCollection BusinessObjects { get; set; }

        /// <summary>
        /// Gets or sets collection of relations.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        public virtual StiDataRelationsCollection Relations { get; set; }

        [Browsable(false)]
        public bool IsRequestFromUserVariablesPresent
        {
            get
            {
                if (Variables == null) return false;

                foreach (StiVariable variable in Variables)
                {
                    if (variable.RequestFromUser)
                        return true;
                }
                return false;
            }
        }
        #endregion

        #region Methods.SortItems
        /// <summary>
        /// Sorts the DataSources collection and columns in each DataSource.
        /// </summary>
        public void SortItems()
        {
            SortItems(StiSortOrder.Asc);
        }

        /// <summary>
        /// Sorts the DataSources collection and columns in each DataSource.
        /// </summary>
        /// <param name="order">Specifies order of sorting items.</param>
        public void SortItems(StiSortOrder order)
        {
            this.DataSources.Sort(order);
            this.Variables.Sort(order);
            this.BusinessObjects.Sort(order);
        }
        #endregion

        #region Methods.File
        /// <summary>
		/// Loads dictionary from the stream through descendants of StiDictionarySLService service.
		/// </summary>
		/// <param name="service">Provider for loading the dictionary.</param>
		/// <param name="stream">Stream to load the dictionary of data.</param>
		public void Load(StiDictionarySLService service, Stream stream)
        {
            service.Load(this, stream);
        }

        /// <summary>
        /// Loads the dictionary from the stream in XML format.
        /// </summary>
        /// <param name="stream">Stream to load the dictionary of data.</param>
        public void Load(Stream stream)
        {
            var service = StiReport.IsJsonFile(stream) ? (StiDictionarySLService)new StiJsonDictionarySLService() : new StiXmlDictionarySLService();
            Load(service, stream);
        }

        /// <summary>
        /// Loads dictionary from the file in XML format.
        /// </summary>
        /// <param name="path">File containing dictionary.</param>
        public void Load(string path)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            Load(stream);
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Loads the dictionary from file through descendants of StiDictionarySLService service.
        /// </summary>
        /// <param name="service">Provider for loading the dictionary.</param>
        /// <param name="path">File for loading the dictionary.</param>
        public void Load(StiDictionarySLService service, string path)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            service.Load(this, stream);
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Merges the dictionary with the stream through provider.
        /// </summary>
        /// <param name="service">Provider for merging with the dictionary.</param>
        /// <param name="stream">Stream to merge with the dictionary of data.</param>
        public void Merge(StiDictionarySLService service, Stream stream)
        {
            service.Merge(this, stream);
        }

        /// <summary>
        /// Merges the dictionary with the stream in XML format.
        /// </summary>
        /// <param name="stream">Stream to merge with dictionary of data.</param>
        public void Merge(Stream stream)
        {
            var service = StiReport.IsJsonFile(stream) ? (StiDictionarySLService)new StiJsonDictionarySLService() : new StiXmlDictionarySLService();
            Merge(service, stream);
        }

        /// <summary>
        /// Merges the dictionary with the file in XML format.
        /// </summary>
        /// <param name="path">File for merging of the dictionary.</param>
        public void Merge(string path)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            Merge(stream);
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Merges the dictionary with the file through provider.
        /// </summary>
        /// <param name="service">Provider for merging with the dictionary.</param>
        /// <param name="path">File for merging with the dictionary.</param>
        public void Merge(StiDictionarySLService service, string path)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            service.Merge(this, stream);
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Merges two dictionaries.
        /// </summary>
        /// <param name="dictionary">Dictionary to merge.</param>
        public void Merge(StiDictionary dictionary)
        {
            #region Merging data sources
            foreach (StiDataSource ds in dictionary.DataSources)
            {
                var dataSource = this.DataSources[ds.Name];
                if (dataSource == null)
                {
                    dataSource = (StiDataSource)ds.Clone();
                    dataSource.Name = StiNameCreation.CreateName(Report, dataSource.Name, false, false, true);
                    this.DataSources.Add(dataSource);
                }
                else
                {
                    foreach (StiDataColumn cl in ds.Columns)
                    {
                        var dataColumn = dataSource.Columns[cl.Name];
                        if (dataColumn == null) dataSource.Columns.Add((StiDataColumn)cl.Clone());
                    }
                }
            }
            #endregion

            #region Merging data relations
            foreach (StiDataRelation rl in dictionary.Relations)
            {
                var relation = this.Relations[rl.NameInSource];
                if (relation != null && relation.Name == rl.Name) continue;

                relation = (StiDataRelation)rl.Clone();
                relation.ParentSource = this.DataSources[relation.ParentSource.Name];
                relation.ChildSource = this.DataSources[relation.ChildSource.Name];
                relation.Name = GetRelationName(relation);

                this.Relations.Add(relation);
            }
            #endregion

            this.Variables.AddRange(dictionary.Variables);
            this.Connect(false);
        }

        /// <summary>
        /// Saves dictionary to the stream through provider.
        /// </summary>
        /// <param name="service">Provider for saving dictionary.</param>
        /// <param name="stream">Stream to save dictionary of data.</param>
        public void Save(StiDictionarySLService service, Stream stream)
        {
            service.Save(this, stream);
        }

        /// <summary>
        /// Saves dictionary to the stream in XML format.
        /// </summary>
        /// <param name="stream">Stream to save dictionary of data.</param>
        public void Save(Stream stream)
        {
            var service = new StiXmlDictionarySLService();
            Save(service, stream);
        }

        /// <summary>
        /// Saves the dictionary in file in XML format.
        /// </summary>
        /// <param name="path">File for saving dictionary.</param>
        public void Save(string path)
        {
            StiFileUtils.ProcessReadOnly(path);
            var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            Save(stream);
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Saves the dictionary in file through service.
        /// </summary>
        /// <param name="service">Provider for saving dictionary.</param>
        /// <param name="path">File for saving dictionary.</param>
        public void Save(StiDictionarySLService service, string path)
        {
            StiFileUtils.ProcessReadOnly(path);
            var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            service.Save(this, stream);
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Imports Data Sources from DataSet.
        /// </summary>
        /// <param name="dataSet">DataSet for importing.</param>
        public void ImportXMLSchema(DataSet dataSet)
        {
            try
            {
                DataSources.Clear();
                Relations.Clear();
                var setNameService = StiOptions.Services.DataTableSetName.FirstOrDefault();
                var tableNames = new Hashtable();

                foreach (DataTable table in dataSet.Tables)
                {
                    var dataSource = new StiDataTableSource();
                    if (setNameService != null)
                        setNameService.SetName(dataSource, Report, dataSet, table.TableName);

                    dataSource.Name = StiNameCreation.CreateName(Report, dataSource.Name, false, false, true);
                    tableNames[table.TableName] = dataSource.Name;
                    this.DataSources.Add(dataSource);

                    foreach (DataColumn column in table.Columns)
                    {
                        dataSource.Columns.Add(new StiDataColumn(column.ColumnName, column.Caption, column.DataType));
                    }
                }

                foreach (DataRelation dataRelation in dataSet.Relations)
                {
                    var parentColumns = new List<string>();
                    foreach (var parentColumn in dataRelation.ParentColumns)
                    {
                        parentColumns.Add(parentColumn.ColumnName);
                    }

                    var childColumns = new List<string>();
                    foreach (var childColumn in dataRelation.ChildColumns)
                    {
                        childColumns.Add(childColumn.ColumnName);
                    }

                    var relation = new StiDataRelation(
                        dataRelation.RelationName,
                        this.DataSources[tableNames[dataRelation.ParentTable.TableName] as string],
                        this.DataSources[tableNames[dataRelation.ChildTable.TableName] as string],
                        parentColumns.ToArray(),
                        childColumns.ToArray());

                    StiDataRelationSetName.SetName(relation, Report, dataSet, dataRelation.RelationName);
                    relation.Name = GetRelationName(relation);
                    this.Relations.Add(relation);
                }
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "ImportXMLSchema...ERROR");
                StiLogService.Write(this.GetType(), e);
            }

            this.Connect(false);
        }

        /// <summary>
        /// Imports Data Sources from DataSet and merges them with this dictionary.
        /// </summary>
        /// <param name="dataSet">DataSet for importing.</param>
        public void MergeXMLSchema(DataSet dataSet)
        {
            var dictionary = new StiDictionary(this.Report);
            dictionary.ImportXMLSchema(dataSet);
            this.Merge(dictionary);
        }

        /// <summary>
        /// Exports Data Sources to DataSet.
        /// </summary>
        /// <param name="dataSetName">Name of the resulting DataSet.</param>
        /// <returns>DataSet with data.</returns>
        public DataSet ExportXMLSchema(string dataSetName)
        {
            var dataSet = new DataSet(dataSetName)
            {
                EnforceConstraints = false,
                Locale = CultureInfo.InvariantCulture
            };

            try
            {
                foreach (StiDataSource dataSource in this.DataSources)
                {
                    var table = new DataTable(dataSource.Name);

                    dataSet.Tables.Add(table);

                    foreach (StiDataColumn column in dataSource.Columns)
                    {
                        if (column.Type.IsGenericType && column.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            table.Columns.Add(new DataColumn(column.Name, Nullable.GetUnderlyingType(column.Type))
                            {
                                AllowDBNull = true,
                                Caption = column.Alias
                            });
                        }
                        else
                        {
                            table.Columns.Add(new DataColumn(column.Name, column.Type)
                            {
                                Caption = column.Alias
                            });
                        }
                    }
                }

                foreach (StiDataRelation relation in this.Relations)
                {
                    var parentColumns = new List<DataColumn>();

                    foreach (var str in relation.ParentColumns)
                    {
                        parentColumns.Add(dataSet.Tables[relation.ParentSource.Name].Columns[str]);
                    }

                    var childColumns = new List<DataColumn>();

                    foreach (var str in relation.ChildColumns)
                    {
                        childColumns.Add(dataSet.Tables[relation.ChildSource.Name].Columns[str]);
                    }

                    dataSet.Relations.Add(new DataRelation(
                        relation.NameInSource,
                        parentColumns.ToArray(),
                        childColumns.ToArray(),
                        false));

                }
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "ExportXMLSchema...ERROR");
                StiLogService.Write(this.GetType(), e);
            }

            return dataSet;
        }
        #endregion

        #region Methods.UsedData
        /// <summary>
		/// Removes from dictionary unused data sources, relations, and rows and etc.
		/// </summary>
		public void RemoveUnusedData()
        {
            var service = new StiDataRetrieval();

            service.Retrieval(Report);

            #region Removes from dictionary unused Relations
            var index = 0;
            while (index < Relations.Count)
            {
                if (service.UsedRelations[Relations[index]] == null)
                    Relations.Remove(Relations[index]);
                else
                    index++;
            }
            #endregion

            #region Removes from dictionary unused columns
            foreach (StiDataSource dataSource in DataSources)
            {
                index = 0;
                while (index < dataSource.Columns.Count)
                {
                    if (service.UsedColumns[dataSource.Columns[index]] == null)
                        dataSource.Columns.Remove(dataSource.Columns[index]);
                    else
                        index++;
                }
            }
            #endregion

            #region Removes from dictionary unused data sources
            index = 0;
            while (index < DataSources.Count)
            {
                if (service.UsedDataSources[DataSources[index]] == null)
                    DataSources.Remove(DataSources[index]);
                else
                    index++;
            }
            #endregion

            service.Dispose();
        }

        /// <summary>
        /// Removes from dictionary unused data sources V2
        /// </summary>
        public void RemoveUnusedDataSourcesV2()
        {
            var usedDataSources = StiDataSourceHelper.GetUsedDataSourcesNames(Report);
            var index = 0;
            while (index < DataSources.Count)
            {
                if (!usedDataSources.ContainsKey(DataSources[index].Name))
                    DataSources.Remove(DataSources[index]);
                else
                    index++;
            }
        }

        /// <summary>
        /// Returns Hashtables with the used relations, data sources and columns.
        /// </summary>
        /// <param name="usedRelations">Used relations.</param>
        /// <param name="usedDataSources">Used data sources.</param>
        /// <param name="usedColumns">Used columns.</param>
        public void RetrievalData(out Hashtable usedRelations, out Hashtable usedDataSources, out Hashtable usedColumns)
        {
            var service = new StiDataRetrieval();
            service.Retrieval(Report);

            usedRelations = service.UsedRelations;
            usedDataSources = service.UsedDataSources;
            usedColumns = service.UsedColumns;
        }

        /// <summary>
        /// Returns Collection of unused relations.
        /// </summary>
        public StiDataRelationsCollection GetUnusedRelationsFromDataStore()
        {
            var categories = new Hashtable();
            return GetUnusedRelationsFromDataStore(ref categories);
        }

        /// <summary>
        /// Returns Collection of unused relations.
        /// </summary>
        public StiDataRelationsCollection GetUnusedRelationsFromDataStore(string tableName)
        {
            var categories = new Hashtable();
            return GetUnusedRelationsFromDataStore(tableName, ref categories);
        }

        /// <summary>
        /// Returns Collection of unused relations.
        /// </summary>
        /// <param name="categories">Collection of unused relations will be added to this Hashtable.</param>
        /// <returns>Returns Collection of unused relations.</returns>
        public StiDataRelationsCollection GetUnusedRelationsFromDataStore(ref Hashtable categories)
        {
            var collection = new StiDataRelationsCollection(this);

            //Create a list of the data sources which contains DataTable
            var tables = DataSources.Cast<StiDataSource>()
                .OfType<StiDataTableSource>()
                .Where(tableSource => tableSource.DataTable != null)
                .ToList();

            foreach (var table in tables)
            {
                var rels = table.DataTable.ParentRelations;

                foreach (DataRelation relation in rels)
                {
                    foreach (var tableSource in tables)
                    {
                        if (tableSource.DataTable != relation.ParentTable) continue;

                        var index = 0;
                        foreach (StiDataRelation rel in Relations)
                        {
                            if (StiStrFix.Del_(rel.NameInSource) == StiStrFix.Del_(relation.RelationName)) break;
                            index++;
                        }

                        if (index == Relations.Count)
                        {
                            var stiRelation = new StiDataRelation(
                                relation.RelationName, tableSource, table,
                                relation.ParentColumns.Select(column => column.ColumnName).ToArray(),
                                relation.ChildColumns.Select(column => column.ColumnName).ToArray());

                            StiDataRelationSetName.SetName(stiRelation, Report, relation.DataSet, relation.RelationName);

                            collection.Add(stiRelation);

                            var category = table.GetCategoryName();

                            categories[stiRelation] = category;
                            break;
                        }
                    }
                }
            }

            return collection;
        }

        /// <summary>
        /// Returns Collection of unused relations.
        /// </summary>
        /// <param name="categories">Collection of unused relations will be added to this Hashtable.</param>
        /// <returns>Returns Collection of unused relations.</returns>
        public StiDataRelationsCollection GetUnusedRelationsFromDataStore(string tableName, ref Hashtable categories)
        {
            var collection = new StiDataRelationsCollection(this);

            //Create a list of the data sources which contains DataTable
            var tables = DataSources.Cast<StiDataSource>()
                .OfType<StiDataTableSource>()
                .Where(tableSource => tableSource.DataTable != null && tableSource.NameInSource != null 
                && tableSource.NameInSource.StartsWithInvariant(tableName + "."))
                .ToList();

            foreach (var table in tables)
            {
                var rels = table.DataTable.ParentRelations;

                foreach (DataRelation relation in rels)
                {
                    foreach (var tableSource in tables)
                    {
                        if (tableSource.DataTable != relation.ParentTable) continue;

                        var index = 0;
                        foreach (StiDataRelation rel in Relations)
                        {
                            if (StiStrFix.Del_(rel.NameInSource) == StiStrFix.Del_(relation.RelationName)) break;
                            index++;
                        }

                        if (index == Relations.Count)
                        {
                            var stiRelation = new StiDataRelation(
                                relation.RelationName, tableSource, table,
                                relation.ParentColumns.Select(column => column.ColumnName).ToArray(),
                                relation.ChildColumns.Select(column => column.ColumnName).ToArray());

                            StiDataRelationSetName.SetName(stiRelation, Report, relation.DataSet, relation.RelationName);

                            collection.Add(stiRelation);

                            var category = table.GetCategoryName();

                            categories[stiRelation] = category;
                            break;
                        }
                    }
                }
            }

            return collection;
        }
        #endregion

        #region Methods.Synchronize
        /// <summary>
        /// Synchronizes DataStore and Dictionary.
        /// <param name="report">Report for Synchronization of the data.</param>
        /// </summary>
        public static async Task DoAutoSynchronizeAsync(StiReport report)
        {
            await Task.Run(() => DoAutoSynchronize(report));
        }

        /// <summary>
        /// Synchronizes DataStore and Dictionary.
        /// <param name="report">Report for Synchronization of the data.</param>
        /// </summary>
        public static void DoAutoSynchronize(StiReport report)
        {
            if (report == null) return;

            if (AutoSynchronize == StiAutoSynchronizeMode.Always ||
                (AutoSynchronize == StiAutoSynchronizeMode.IfDictionaryEmpty &&
                 report.Dictionary.DataSources.Count == 0))
                report.Dictionary.Synchronize();
        }

        private void Synchronize(StiBusinessObjectsCollection businessObjects)
        {
            foreach (StiBusinessObject businessObject in businessObjects)
            {
                var data = businessObject.GetBusinessObjectData();
                if (data != null)
                    SynchronizeColumns(data, businessObject);

                Synchronize(businessObject.BusinessObjects);
            }
        }

        /// <summary>
        /// Synchronizes the content of the DataStore and Dictionary. If DataSources or Columns or Relations from the DataStore does not exists in Dictionary, then new elements will be added to the Dictionary.
        /// </summary>
        public async Task SynchronizeAsync()
        {
            await Task.Run(() => Synchronize());
        }

        /// <summary>
        /// Synchronizes the content of the DataStore and Dictionary. If DataSources or Columns or Relations from the DataStore does not exists in Dictionary, then new elements will be added to the Dictionary.
        /// </summary>
        public void Synchronize()
        {
            lock (this)
            {
                foreach (StiDataSource dataSource in this.DataSources)
                {
                    dataSource.Columns.CachedDataColumns.Clear();
                }

                this.DataSources.CachedDataSources.Clear();

                #region Data Sources
                foreach (var data in DataStore.ToList().Where(d => !(d.ViewData is IDbConnection)))
                {
                    var currentSource = DataSources.ToList()
                        .Where(d => d is StiDataStoreSource && !(d is StiSqlSource || d is StiVirtualSource || d is StiDataTransformation))
                        .Cast<StiDataStoreSource>()
                        .FirstOrDefault(d => d.NameInSource.ToLowerInvariant() == data.Name.ToLowerInvariant());

                    if (currentSource == null)
                    {
                        var adapter = StiDataAdapterService.GetDataAdapter(data);
                        if (adapter == null) continue;

                        var dataSource = adapter.Create(this, false);
                        var columns = StiDataLeader.GetColumnsFromData(adapter, data, dataSource);
                        dataSource.Columns.AddRange(columns);

                        adapter.SetDataSourceNames(data, dataSource);
                        dataSource.Name = StiNameCreation.CreateName(Report, dataSource.Name, false, false, true);
                        this.DataSources.Add(dataSource);
                        SynchronizeColumns(data, dataSource);
                    }
                    else
                        SynchronizeColumns(data, currentSource);
                }
                #endregion

                #region Business Objects from DataStore
                foreach (StiData data in DataStore)
                {
                    var finded = false;
                    foreach (StiDataSource dataSource in DataSources)
                    {
                        var dataStore = dataSource as StiDataStoreSource;
                        if (dataStore != null && dataStore.NameInSource.ToLower(CultureInfo.InvariantCulture) == data.Name.ToLower(CultureInfo.InvariantCulture))
                        {
                            finded = true;
                            break;
                        }
                    }

                    if (!finded && !(data.Data is IDbConnection))
                    {
                        var adapter = new StiBusinessObjectAdapterService();
                        var dataSource = adapter.Create(this, false);
                        var columns = StiDataLeader.GetColumnsFromData(adapter, data, dataSource);
                        dataSource.Columns.AddRange(columns);

                        adapter.SetDataSourceNames(data, dataSource);
                        dataSource.Name = StiNameCreation.CreateName(Report, dataSource.Name, false, false, true);

                        this.DataSources.Add(dataSource);
                    }
                }
                #endregion

                SynchronizeBusinessObjects();
                Connect(false);

                #region Relations
                var unusedRelations = GetUnusedRelationsFromDataStore();

                foreach (StiDataRelation rel in unusedRelations)
                {
                    if (Relations.Cast<StiDataRelation>().Any(relation => StiStrFix.Del_(rel.NameInSource) == StiStrFix.Del_(relation.NameInSource))) continue;

                    rel.Name = StiNameCreation.CreateRelationName(Report, rel, rel.ParentSource.Name);
                    Relations.Add(rel);
                }
                #endregion

                Connect(false);

                StiOptions.Engine.GlobalEvents.InvokeReportSynchronized(this, new EventArgs());
            }
        }

        public void SynchronizeRelations()
        {
            Connect(false);

            #region Relations
            var unusedRelations = GetUnusedRelationsFromDataStore();

            foreach (StiDataRelation rel in unusedRelations)
            {
                if (Relations.Cast<StiDataRelation>().Any(relation => StiStrFix.Del_(rel.NameInSource) == StiStrFix.Del_(relation.NameInSource))) continue;
                rel.Name = GetRelationName(rel);
                Relations.Add(rel);
            }
            #endregion

            Connect(false);
        }

        public async Task SynchronizeRelationsAsync(string tableName)
        {
            await Task.Run(() =>
            {
                SynchronizeRelations(tableName); 
            });
        }

        public void SynchronizeRelations(string tableName)
        {
            Connect(false);

            #region Relations
            var unusedRelations = GetUnusedRelationsFromDataStore(tableName);

            foreach (StiDataRelation rel in unusedRelations)
            {
                if (Relations.Cast<StiDataRelation>().Any(relation => StiStrFix.Del_(rel.NameInSource) == StiStrFix.Del_(relation.NameInSource))) continue;
                rel.Name = GetRelationName(rel);
                Relations.Add(rel);
            }
            #endregion

            Connect(false);
        }

        public void SynchronizeBusinessObjects()
        {
            foreach (var businessObjectData in this.Report.BusinessObjectsStore)
            {
                var finded = false;
                foreach (StiBusinessObject businessObject in this.BusinessObjects)
                {
                    if (businessObject.Name == businessObjectData.Name)
                    {
                        finded = true;
                        businessObject.Alias = businessObjectData.Alias;
                        businessObject.Category = businessObjectData.Category;
                        SynchronizeColumns(businessObjectData, businessObject);

                        Synchronize(businessObject.BusinessObjects);

                        break;
                    }
                }

                if (!finded)
                {
                    var obj = new StiBusinessObject(
                        businessObjectData.Category, businessObjectData.Name, businessObjectData.Alias, null);

                    this.BusinessObjects.Add(obj);
                    SynchronizeColumns(businessObjectData, obj);
                }
            }
        }

        /// <summary>
        /// Synchronizes the business objects with report dictionary.
        /// </summary>
        /// <param name="maxLevel">Maximum level of nested business objects which will be checked.</param>        
        public void SynchronizeBusinessObjects(int maxLevel)
        {
            this.SynchronizeBusinessObjects();
            this.SynchronizeBusinessObjects(this.BusinessObjects, 0, maxLevel);
        }

        /// <summary>
        /// Synchronizes the business objects with report dictionary.
        /// </summary>
        /// <param name="maxLevel">Maximum level of nested business objects which will be checked.</param>        
        private void SynchronizeBusinessObjects(StiBusinessObjectsCollection objects, int curLevel, int maxLevel)
        {
            foreach (StiBusinessObject obj in objects)
            {
                var businessObjectData = obj.GetBusinessObjectData();
                if (businessObjectData != null)
                    SynchronizeColumns(businessObjectData, obj);

                var dataColumns =
                    businessObjectData != null ?
                    StiBusinessObjectHelper.GetColumnsFromData(businessObjectData, true) : null;

                if (dataColumns != null)
                {
                    if (StiOptions.Dictionary.BusinessObjects.ColumnsSynchronizationMode == StiColumnsSynchronizationMode.RemoveAbsentColumns)
                    {
                        var hashColumns = new Hashtable();
                        foreach (StiDataColumn dataColumn in dataColumns)
                        {
                            if (!StiBusinessObjectHelper.IsDataColumn(dataColumn.Type))
                                hashColumns[dataColumn.Name] = null;
                        }

                        var index = 0;
                        while (index < obj.BusinessObjects.Count)
                        {
                            var child = obj.BusinessObjects[index];

                            if (!hashColumns.ContainsKey(child.Name))
                                obj.BusinessObjects.RemoveAt(index);
                            else
                                index++;
                        }
                        hashColumns.Clear();
                    }

                    foreach (StiDataColumn dataColumn in dataColumns)
                    {
                        if (!StiBusinessObjectHelper.IsDataColumn(dataColumn.Type))
                        {
                            #region Try to find child business object
                            var finded = false;
                            foreach (StiBusinessObject child in obj.BusinessObjects)
                            {
                                if (child.Name == dataColumn.Name)
                                    finded = true;
                            }
                            #endregion

                            #region Business object is not finded
                            if (!finded)
                            {
                                obj.BusinessObjects.Add(new StiBusinessObject
                                {
                                    Name = dataColumn.Name,
                                    Alias = dataColumn.Alias
                                });
                            }
                            #endregion
                        }
                    }
                }

                if (curLevel < maxLevel)
                    SynchronizeBusinessObjects(obj.BusinessObjects, curLevel + 1, maxLevel);
            }
        }

        public void SynchronizeColumns(StiData data, StiDataSource dataSource)
        {
            StiDataAdapterService adapter;

            if (data == null && dataSource is StiDBaseSource)
                adapter = new StiDBaseAdapterService();

            else if (data == null && dataSource is StiCsvSource)
                adapter = new StiCsvAdapterService();

            else
                adapter = StiDataAdapterService.GetDataAdapter(data);

            if (adapter == null) return;

            var columns = StiDataLeader.GetColumnsFromData(adapter, data, dataSource);

            foreach (StiDataColumn dataColumn in dataSource.Columns)
            {
                var finded = false;
                foreach (StiDataColumn column in columns)
                {
                    if (dataColumn.NameInSource == column.NameInSource)
                    {
                        column.Name = dataColumn.Name;
                        if (StiOptions.Designer.AutoCorrectDataColumnName)
                            column.Name = StiNameValidator.CorrectName(column.Name, Report);

                        column.Alias = dataColumn.Alias;
                        column.NameInSource = dataColumn.NameInSource;
                        column.Key = dataColumn.Key;

                        if (StiOptions.Dictionary.ColumnTypeSynchronizationMode == StiColumnTypeSynchronizationMode.KeepAsIs)
                        {
                            column.Type = dataColumn.Type;
                        }
                        else
                        {
                            #region Check Nullable types
                            if ((dataColumn.Type == typeof(DateTime) || dataColumn.Type == typeof(DateTime?)) && StiOptions.Dictionary.UseNullableDateTime)
                                column.Type = typeof(DateTime?);

                            else if ((dataColumn.Type == typeof(TimeSpan) || dataColumn.Type == typeof(TimeSpan?)) && StiOptions.Dictionary.UseNullableTimeSpan)
                                column.Type = typeof(TimeSpan?);
                            #endregion
                        }

                        finded = true;
                    }
                }

                if (!finded && StiOptions.Dictionary.ColumnsSynchronizationMode == StiColumnsSynchronizationMode.KeepAbsentColumns || dataColumn is StiCalcDataColumn)
                    columns.Add(dataColumn);
            }

            #region Check columns collection for duplicated names
            //Colection checked from end to start 
            //because old columns placed at end of collection.
            var hashColumns = new Hashtable();
            for (var index = columns.Count - 1; index >= 0; index--)
            {
                var column = columns[index];

                var nameIndex = 1;
                string name = null;
                while (true)
                {
                    if (nameIndex != 1)
                        name = $"{column.Name}{nameIndex}";
                    else
                        name = column.Name;

                    if (hashColumns[name] == null)
                    {
                        hashColumns[name] = name;
                        break;
                    }
                    nameIndex++;
                }
                column.Name = name;
            }
            #endregion

            #region Check Sort Columns
            var columnsCheckOnSort = new StiDataColumnsCollection();

            foreach (StiDataColumn columnSort in dataSource.Columns)
            {
                foreach (StiDataColumn column in columns)
                {
                    if (columnSort.NameInSource == column.NameInSource)
                    {
                        columnsCheckOnSort.Add(column);
                        columns.Remove(column);
                        break;
                    }
                }
            }

            columnsCheckOnSort.AddRange(columns);
            #endregion

            dataSource.Columns.Clear();
            dataSource.Columns.AddRange(columnsCheckOnSort);
        }

        public void SynchronizeColumns(StiBusinessObjectData data, StiBusinessObject source)
        {
            SynchronizeColumns(data.BusinessObjectValue, source);
        }

        public void SynchronizeColumns(object data, StiBusinessObject source)
        {
            var columns = StiBusinessObjectHelper.GetColumnsFromData(data);

            foreach (StiDataColumn dataColumn in source.Columns)
            {
                var finded = false;
                foreach (StiDataColumn column in columns)
                {
                    if (dataColumn.NameInSource == column.NameInSource)
                    {
                        column.Name = dataColumn.Name;
                        if (StiOptions.Designer.AutoCorrectDataColumnName)
                            column.Name = StiNameValidator.CorrectName(column.Name, Report);

                        column.Alias = dataColumn.Alias;
                        column.NameInSource = dataColumn.NameInSource;
                        column.Key = dataColumn.Key;

                        if (StiOptions.Dictionary.ColumnTypeSynchronizationMode == StiColumnTypeSynchronizationMode.KeepAsIs)
                        {
                            column.Type = dataColumn.Type;
                        }
                        else
                        {
                            #region Check Nullable types
                            if ((dataColumn.Type == typeof(DateTime) || dataColumn.Type == typeof(DateTime?)) && StiOptions.Dictionary.UseNullableDateTime)
                                column.Type = typeof(DateTime?);

                            else if (dataColumn.Type == typeof(TimeSpan) || dataColumn.Type == typeof(TimeSpan?) && StiOptions.Dictionary.UseNullableTimeSpan)
                                column.Type = typeof(TimeSpan?);
                            #endregion
                        }

                        finded = true;
                        break;
                    }
                }

                if (!finded && StiOptions.Dictionary.BusinessObjects.ColumnsSynchronizationMode == StiColumnsSynchronizationMode.KeepAbsentColumns)
                    columns.Add(dataColumn);
            }

            #region Check columns collection for duplicated names
            //Colection checked from end to start 
            //because old columns placed at end of collection.
            var hashColumns = new Hashtable();
            for (var index = columns.Count - 1; index >= 0; index--)
            {
                var column = columns[index];

                var nameIndex = 1;
                string name;
                while (true)
                {
                    if (nameIndex != 1)
                        name = $"{column.Name}{nameIndex}";
                    else
                        name = column.Name;

                    if (hashColumns[name] == null)
                    {
                        hashColumns[name] = name;
                        break;
                    }
                    nameIndex++;
                }
                column.Name = name;
            }
            #endregion

            source.Columns.Clear();
            source.Columns.AddRange(columns);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Attempts to cancels the execution of all active SQL queries.
        /// </summary>
        public void CancelAllActiveSQLQueries()
        {
            var report = Report.CompiledReport ?? Report;
            report.DataSources.ToList()
                .Where(d => d is StiSqlSource).Cast<StiSqlSource>().ToList()
                .ForEach(s => s.DataAdapter?.SelectCommand?.Cancel());
        }

        /// <summary>
        /// Clears data dictionary.
        /// </summary>
        public void Clear()
        {
            BusinessObjects.Clear();
            Variables.Clear();
            Databases.Clear();
            DataSources.Clear();
            Relations.Clear();
            DataStore.ClearReportDatabase();
            Resources.Clear();
            Restrictions.Clear();
        }

        private void DisposeCacheDataSet()
        {
            if (CacheDataSet != null)
            {
                foreach (DataTable dataTable in CacheDataSet.Tables)
                {
                    for (var f = dataTable.ChildRelations.Count - 1; f >= 0; f--)
                    {
                        if (dataTable.ChildRelations[f].ChildTable.Constraints.Contains(dataTable.ChildRelations[f].RelationName))
                            dataTable.ChildRelations[f].ChildTable.Constraints.Remove(dataTable.ChildRelations[f].RelationName);

                        dataTable.ChildRelations.RemoveAt(f);
                    }
                    dataTable.ChildRelations.Clear();
                    dataTable.ParentRelations.Clear();
                    dataTable.Constraints.Clear();
                }
                CacheDataSet.Relations.Clear();
                CacheDataSet.Tables.Clear();
                CacheDataSet.Dispose();
                CacheDataSet = null;
            }
        }

        public void RenameDatabase(StiDatabase database, string newName)
        {
            if (database.Name == newName)
                return;

            var lastName = database.Name + ".";
            var name = newName + ".";
            for (var index = 0; index < this.DataSources.Count; index++)
            {
                var dataSource = this.DataSources[index] as StiDataTableSource;

                if (dataSource != null)
                {
                    if (dataSource.NameInSource.StartsWith(lastName))
                        dataSource.NameInSource = dataSource.NameInSource.Replace(lastName, name);

                    else if (dataSource.NameInSource == database.Name)
                        dataSource.NameInSource = newName;
                }
            }

            database.Name = newName;
        }

        /// <summary>
        /// Returns a new DataSet from the dictionary.
        /// </summary>
        /// <param name="dataSetName">Specifies a name of the new DataSet object from the dictionary.</param>
        public DataSet GetDataSet(string dataSetName)
        {
            var dsData = new DataSet(dataSetName);

            this.Connect(true);

            foreach (StiDataSource dataSource in DataSources)
            {
                if (dsData.Tables[dataSource.Name] == null)
                {
                    var table = dataSource.GetDataTable();
                    dsData.Tables.Add(table);
                }
            }
            return dsData;
        }

        /// <summary>
        /// Returns IDbConnection for database with specified name.
        /// </summary>
        public IDbConnection GetConnection(string name)
        {
            foreach (StiData data in this.DataStore)
            {
                var connection = data.Data as IDbConnection;
                if (connection != null && data.Name == name)
                    return connection;
            }
            return null;
        }
        #endregion

        #region Methods.Connect-Disconnect
        public void ConnectToDatabases(bool loadData = true, bool throwException = true)
        {
            ConnectToDatabases(Databases.ToList(), loadData, throwException);
        }

        internal void ConnectToDatabases(List<StiDatabase> databases, bool loadData = true, bool throwException = true)
        {
            DataStore.ClearReportDatabase();
            foreach (var database in databases)
            {
                try
                {
                    database.InvokeConnecting();
                    StiDataLeader.RegData(database, this, loadData);
                    database.InvokeConnected();
                }
                catch (Exception e)
                {
                    if (throwException)
                        throw e;
                }
            }
        }

        internal void TryConnect(bool loadData = true, List<StiDataSource> dataSources = null)
        {
            try
            {
                Connect(loadData, dataSources);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Connects specified databases to the data.
        /// </summary>
        internal void Connect(List<StiDatabase> databases)
        {
            ConnectToDatabases(databases, true, false);
        }

        /// <summary>
        /// Connects all Data Sources to data.
        /// </summary>
        /// <param name="loadData">Specifies whether it is necessary to load data or not.</param>
        public async Task ConnectAsync(bool loadData = true, List<StiDataSource> dataSources = null)
        {
            await Task.Run(() => { Connect(loadData, dataSources); });
        }

        /// <summary>
        /// Connects all Data Sources to data.
        /// </summary>
        /// <param name="loadData">Specifies whether it is necessary to load data or not.</param>
        public void Connect(bool loadData = true, List<StiDataSource> dataSources = null)
        {
            try
            {
                DisposeCacheDataSet();

                CacheDataSet = new DataSet("CacheDataSet");
                ConnectToDatabases(loadData);

                if (dataSources != null)
                    dataSources.ForEach(d => StiDataLeader.Connect(d, loadData));

                else if (Report.RetrieveOnlyUsedData && loadData)
                {
                    var dataSourceNames = StiDataSourceHelper.GetUsedDataSourcesNames(Report);

                    DataSources.ToList()
                        .Where(d => dataSourceNames.ContainsKey(d.Name)).ToList()
                        .ForEach(d => StiDataLeader.Connect(d));
                }
                else
                    DataSources.Connect(loadData);

                BusinessObjects.Connect();

                if (loadData)
                    RegRelations();
            }
            catch
            {
                if (loadData) throw;
            }
        }

        /// <summary>
        /// Connects all Data Sources to data V2
        /// </summary>
        internal void ConnectV2(StiReport baseReport)
        {
            bool loadData = true;

            var report = baseReport.CompiledReport ?? baseReport;

            //prepare list of dataSources, which are used in variables
            List<string> listDS = new List<string>();
            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (variable.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Columns)
                {
                    CheckColumnForDataSourceName(variable.DialogInfo.KeysColumn, listDS, report);
                    CheckColumnForDataSourceName(variable.DialogInfo.ValuesColumn, listDS, report);
                }
            }

            try
            {
                report.Dictionary.DisposeCacheDataSet();

                report.Dictionary.CacheDataSet = new global::System.Data.DataSet("CacheDataSet");
                report.Dictionary.ConnectToDatabases(loadData);

                //make dataSources list and sort it
                List<StiDataSource> dss = report.Dictionary.DataSources.ToList()
                     .Where(d => (d.ConnectionOrder != (int)StiConnectionOrder.None) && d.ConnectOnStart).ToList();
                if (report.RetrieveOnlyUsedData && loadData)
                {
                    var dataSourceNames = StiDataSourceHelper.GetUsedDataSourcesNames(report);
                    dss = dss.Where(d => dataSourceNames.ContainsKey(d.Name)).ToList();
                }

                //connect dataSources which used in variables
                dss.Where(d => listDS.Contains(d.Name)).ToList().ForEach(d => StiDataLeader.Connect(d));

                StiVariableHelper.FillItemsOfVariables(report, false);

                //connect other dataSources
                dss.Where(d => !listDS.Contains(d.Name)).ToList().ForEach(d => StiDataLeader.Connect(d));

                report.Dictionary.BusinessObjects.Connect();

                if (loadData)
                    report.Dictionary.RegRelations();
            }
            catch
            {
                if (loadData) throw;
            }
        }
        private static void CheckColumnForDataSourceName(string column, List<string> dataSources, StiReport report)
        {
            if (!string.IsNullOrWhiteSpace(column))
            {
                var strs = column.Split('.');
                if (report.Dictionary.DataSources.Contains(strs[0]) && !dataSources.Contains(strs[0]))
                {
                    dataSources.Add(strs[0]);
                }
            }
        }

        /// <summary>
        /// Intenal use only.
        /// </summary>
        public void ConnectVirtualDataSources()
        {
            var usedDatasources = new Hashtable();

            var reinit = true;
            while (reinit)
            {
                reinit = false;
                foreach (var dataSource in DataSources.FetchAllVirtualDataSources())
                {
                    var masterSource = Report.Dictionary.DataSources[dataSource.NameInSource];
                    if (masterSource == null || masterSource.Name == dataSource.Name)
                    {
                        usedDatasources[dataSource] = dataSource;
                        continue;
                    }

                    if (masterSource is StiVirtualSource && usedDatasources[masterSource] == null)
                    {
                        reinit = true;
                        continue;
                    }

                    dataSource.ConnectToData();
                    usedDatasources[dataSource] = dataSource;
                }
            }
        }

        /// <summary>
        /// Intenal use only.
        /// </summary>
        public void ConnectDataTransformations()
        {
            DataSources.FetchAllDataTransformations().ForEach(d => d.ConnectToData());
        }

        public void ConnectCrossTabDataSources()
        {
            DataSources.FetchAllCrossTabDataSources().ForEach(ConnectCrossTabDataSources);
        }

        /// <summary>
        /// Intenal use only.
        /// </summary>
        public void ConnectCrossTabDataSources(StiCrossTabDataSource dataSource)
        {
            var crossTab = Report.GetComponents().ToList()
                .FirstOrDefault(c => c is StiCrossTab && c.Name == dataSource.NameInSource) as StiCrossTab;

            if (crossTab == null) return;

            var table = StiCrossTabHelper.CreateCrossForCrossTabDataSource(crossTab);
            dataSource.ConnectToData(table, crossTab);
        }

        internal void TryDisconnect()
        {
            try
            {
                Disconnect();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Disconnects the Data Source from data.
        /// </summary>
        public void Disconnect()
        {            
            DisconnectingDatabases(Databases.ToList());
            DisconnectingConnectionInDataStore();

            DisposeCacheDataSet();

            BusinessObjects.Disconnect();
            DataSources.Disconnect();
            DataStore.ClearReportDatabase();
            
            DisconnectedDatabases(Databases.ToList());

            StiDataConnections.UnRegisterConnections(Databases.Cast<IStiAppConnection>().ToList());
        }

        internal void DisconnectingDatabases(List<StiDatabase> databases)
        {
            databases.ForEach(d => d.InvokeDisconnecting());
        }

        internal void DisconnectedDatabases(List<StiDatabase> databases)
        {
            databases.ForEach(d => d.InvokeDisconnected());
        }

        internal void DisconnectingConnectionInDataStore(List<StiData> datas = null)
        {
            if (datas == null)
                datas = DataStore.ToList().Where(d => d.Data is IDbConnection).ToList();

            foreach (var data in datas)
            {
                var connection = data?.Data as IDbConnection;

                if (data.OriginalConnectionState is ConnectionState)
                {
                    if ((ConnectionState)data.OriginalConnectionState != ConnectionState.Open)
                        connection?.Close();
                }
                else
                {
                    if (connection != null && connection.State != ConnectionState.Closed)
                        connection?.Close();
                }
            }
        }
        #endregion

        #region Methods.Relations
        private bool CheckRelation(DataSet dataSet, string relationName, DataColumn[] parentColumns, DataColumn[] childColumns)
        {
            var relation = dataSet.Relations[relationName] ?? dataSet.Relations[relationName.Trim()];
            if (relation != null)
            {
                if (EqualsRelationColumns(parentColumns, relation.ParentColumns) && EqualsRelationColumns(childColumns, relation.ChildColumns))
                    return true;

                if (relation.ChildTable.Constraints.Contains(relation.RelationName))
                    relation.ChildTable.Constraints.Remove(relation.RelationName);

                dataSet.Relations.Remove(relation);
            }

            var index = 0;
            while (index < dataSet.Relations.Count)
            {
                relation = dataSet.Relations[index];
                if (EqualsRelationColumns(relation.ParentColumns, parentColumns) && EqualsRelationColumns(relation.ChildColumns, childColumns))
                {
                    if (relation.ChildTable.Constraints.Contains(relation.RelationName))
                        relation.ChildTable.Constraints.Remove(relation.RelationName);

                    dataSet.Relations.RemoveAt(index);
                }
                else index++;
            }
            return false;
        }

        private bool EqualsRelationColumns(DataColumn[] columns, DataColumn[] cols)
        {
            return columns.All(column =>
                cols.Any(col => col.ColumnName == column.ColumnName && col.Table == column.Table));
        }

        private string GetRelationName(StiDataRelation relation)
        {
            var baseName = relation.Name;
            var relationName = baseName;

            if (relation.ChildSource.Columns.Cast<StiDataColumn>().Any(column => column.Name == relationName))
                baseName = relationName = $"Parent{relationName}";

            var index = 2;
            var relations = Relations.Cast<StiDataRelation>();
            while (true)
            {
                if (relations.Where(r => r.ChildSource == relation.ChildSource && r != relation).All(r => r.Name != relationName)) break;

                relationName = $"{baseName}_{index++}";
            }

            var dataSources = DataSources.Cast<StiDataSource>();
            while (true)
            {
                if (dataSources.All(dataSource => $"Parent{dataSource.Name}" != relationName)) break;

                relationName = $"{baseName}_{index++}";
            }

            return relationName;
        }

        /// <summary>
        /// Register new relations of the dictionary DataSet.
        /// </summary>
        /// <param name="virtualSources">Specifies wether the datasource is based on another datasource or not.</param>
        public void RegRelations(bool virtualSources = false)
        {
            Relations.ToList().ForEach(r => RegRelation(r, virtualSources));
        }

        /// <summary>
        /// Register new relation of the dictionary DataSet.
        /// </summary>
        public void RegRelation(StiDataRelation relation, bool virtualSources = false)
        {
            try
            {
                var parent = relation.ParentSource;
                var child = relation.ChildSource;

                var isVirtualSourceRelation = parent is StiVirtualSource || child is StiVirtualSource ||
                    parent is StiDataTransformation || child is StiDataTransformation;

                if (!virtualSources && isVirtualSourceRelation) return;
                if (virtualSources && !isVirtualSourceRelation) return;

                #region Check data sources existing
                if (!Report.IsDesigning && parent == null)
                    throw new Exception($"Please, check the relation '{relation.Name}'. The report engine can't find the parent Data Source!");

                if (child == null)
                    throw new Exception($"Please, check the relation '{relation.Name}'. The report engine can't find the child Data Source!");
                #endregion

                #region Check DataTable in DataSource
                if (parent.DataTable == null) return;
                if (child.DataTable == null) return;
                #endregion

                #region Check DataTable in DataSet
                if (!Report.IsDesigning && parent.DataTable.DataSet == null)
                {
                    throw new Exception(
                        $"Parent Data Source '{parent.Name}' is not located in a DataSet and can't be used in the relation '{relation.Name}'! " +
                        "You can use the CacheAllData property of a report to cache this Data Source to one DataSet.");
                }

                if (child.DataTable.DataSet == null)
                {
                    throw new Exception(
                        $"The child Data Source '{child.Name}' is not located in a DataSet and can't be used in the relation '{relation.Name}'!" +
                        "You can use the CacheAllData property of a report to cache this Data Source to one DataSet.");
                }
                #endregion

                #region Check one DataSet
                if (!Report.IsDesigning && parent.DataTable.DataSet != child.DataTable.DataSet)
                {
                    throw new Exception(
                        $"The parent Data Source '{parent.Name}' and the Child Data Source '{child.Name}' is not located in one DataSet " +
                        $"and can't be used in the relation '{relation.Name}'! You can use the CacheAllData property of a report " +
                        "to cache this Data Source to one DataSet.");
                }
                #endregion

                #region Check count of columns in relation
                if (!Report.IsDesigning && relation.ParentColumns.Length == 0)
                    throw new Exception(
                        "Parent columns is specified in the relation '{relation.Name}'! " +
                        "This relation can't be created!");

                if (relation.ChildColumns.Length == 0)
                {
                    throw new Exception(
                        "Child columns is specified in the relation '{relation.Name}'! " +
                        "This relation can't be created!");
                }

                if (!Report.IsDesigning && relation.ParentColumns.Length != relation.ChildColumns.Length)
                {
                    throw new Exception(
                        "The count of parent columns is not equal to the count of child columns in the relation '{relation.Name}'! " +
                        "This relation can't be created!");
                }
                #endregion

                #region Prepare parent columns
                var parentColumns = new DataColumn[relation.ParentColumns.Length];
                var index = 0;
                foreach (var columnName in relation.ParentColumns)
                {
                    if (parent.DataTable.Columns[columnName] == null)
                        throw new Exception(
                            $"The column '{columnName}' does not exist in the Data Source '{parent.Name}' and " +
                            $"the relation '{relation.Name}' can't be created!");

                    parentColumns[index] = parent.DataTable.Columns[columnName];
                    index++;
                }
                #endregion

                #region Prepare child columns
                var childColumns = new DataColumn[relation.ChildColumns.Length];

                index = 0;
                foreach (var columnName in relation.ChildColumns)
                {
                    if (child.DataTable.Columns[columnName] == null)
                        throw new Exception(
                            $"The column '{columnName}' does not exist in the Data Source '{child.Name}' and the relation '{relation.Name}' can't be created!");

                    childColumns[index] = child.DataTable.Columns[columnName];
                    index++;
                }
                #endregion

                var dataSet = parent.DataTable.DataSet;

                if (parentColumns.Length > 0 && childColumns.Length > 0 && !CheckRelation(dataSet, relation.NameInSource, parentColumns, childColumns))
                {
                    dataSet.EnforceConstraints = false;
                    dataSet.Relations.Add(relation.NameInSource, parentColumns, childColumns);
                }
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Creating relation '" + relation.NameInSource + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                Report?.WriteToReportRenderingMessages("Creating relation error! " + e.Message);

                if (!StiOptions.Dictionary.HideRelationExceptions && !Report.IsDesigning)
                    throw e;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiDictionary.
        /// </summary>
        public StiDictionary() : this(null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiDictionary.
        /// </summary>
        /// <param name="report">Specifies Report contaning dictionary.</param>
        public StiDictionary(StiReport report)
        {
            Report = report;
            DataSources = new StiDataSourcesCollection(this);
            Relations = new StiDataRelationsCollection(this);
            Databases = new StiDatabaseCollection();
            BusinessObjects = new StiBusinessObjectsCollection(this, null);
        }
    }
}