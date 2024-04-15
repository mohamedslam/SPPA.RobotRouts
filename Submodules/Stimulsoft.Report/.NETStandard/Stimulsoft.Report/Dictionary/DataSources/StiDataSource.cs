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
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary.Design;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Extensions;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
	/// The base class that realizes the Data Source.
	/// </summary>
    [Stimulsoft.Base.Json.JsonObject]
    public abstract class StiDataSource :
        ICloneable,
        IStiAppDataSource,
        IStiStateSaveRestore,
        IStiEnumerator,        
        IEnumerator,
        IEnumerable,
        IStiName,
        IStiAlias,
        IStiInherited,
        IStiPropertyGridObject,
        IStiJsonReportObject
    {
        #region enum Order
        public enum Order
        {
            NameInSource = 100,
            Name = 200,
            Alias = 300,
            AllowExpressions = 400,
            Columns = 500,
            CommandTimeout = 600,
            ConnectOnStart = 700,
            Parameters = 800,
            Query = 850,
            ReconnectOnEachRow = 900,
            SqlCommand = 1000,
            Type = 1100
        }
        #endregion

        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            // StiDataSource
            jObject.AddPropertyBool("Inherited", Inherited);
            jObject.AddPropertyStringNullOrEmpty("Name", Name);
            jObject.AddPropertyBool("IsCloud", IsCloud);
            jObject.AddPropertyStringNullOrEmpty("Alias", Alias);
            jObject.AddPropertyStringNullOrEmpty("Key", Key);
            jObject.AddPropertyJObject("Columns", Columns.SaveToJsonObject(mode));

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Inherited":
                        this.Inherited = property.DeserializeBool();
                        break;

                    case "Name":
                        this.name = property.DeserializeString();
                        break;

                    case "IsCloud":
                        this.IsCloud = property.DeserializeBool();
                        break;

                    case "Alias":
                        this.Alias = property.DeserializeString();
                        break;

                    case "Key":
                        this.Key = property.DeserializeString();
                        break;

                    case "Columns":
                        this.columns.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public virtual StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiDataSource;
            }
        }

        [Browsable(false)]
        public string PropName
        {
            get
            {
                return this.name;
            }
        }

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Name(),
                propHelper.Alias(),
                propHelper.ConnectOnStart(),
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region IStiAppDataSource
        public string GetNameInSource()
        {
            var dataStore = this as StiDataStoreSource;
            return string.IsNullOrEmpty(dataStore?.NameInSource) ? "" : dataStore?.NameInSource;
        }

        public string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Returns a DataTable with data from this datasource.
        /// </summary>
        /// <returns>The DataTable with data.</returns>
        public virtual DataTable GetDataTable(bool allowConnectToData)
        {
            var lockObject = Dictionary != null ? (object)Dictionary : this;
            lock (lockObject)
            {
                if (!allowConnectToData)
                    return DataTable;

                try
                {
                    var sqlSource = this as StiSqlSource;
                    var virtualSource = this as StiVirtualSource;

                    if (sqlSource != null)
                        ConnectSqlSource(sqlSource);

                    else if (virtualSource != null)
                        virtualSource.ConnectToData(true);

                    else
                        StiDataLeader.Connect(this);

                    #region Do not change this code!!!
                    var dataTable = this.DataTable;
                    return dataTable;
                    #endregion
                }
                finally
                {
                    DataTable = null;
                }
            }
        }

        private void ConnectSqlSource(StiSqlSource sqlSource)
        {
            var resSql = sqlSource.SqlCommand;

            try
            {
                StiDataSourceParserHelper.ConnectSqlSource(sqlSource);
                StiDataLeader.Connect(this);
            }
            finally
            {
                sqlSource.SqlCommand = resSql;
            }
        }

        /// <summary>
        /// Returns reference to the dictionary which contains this datasource.
        /// </summary>
        /// <returns>Reference to the app.</returns>
        public IStiAppDictionary GetDictionary()
        {
            return Dictionary;
        }

        /// <summary>
        /// Returns an enumeration of the data columns from this dictionary.
        /// </summary>
        /// <returns>The enumeration of the data columns.</returns>
        public IEnumerable<IStiAppDataColumn> FetchColumns()
        {
            return Columns.Cast<IStiAppDataColumn>();
        }

        /// <summary>
        /// Returns a connection to data for this data source.
        /// </summary>
        /// <returns>Reference to the connection.</returns>
        public IStiAppConnection GetConnection()
        {
            return StiDataSourceHelper.GetDatabaseFromDataSource(this);
        }

        /// <summary>
        /// Returns an enumeration of the parent data relations for this data source.
        /// </summary>
        /// <returns>The enumeration of the data relations.</returns>
        public IEnumerable<IStiAppDataRelation> FetchParentRelations(bool activePreferred)
        {
            return this.ParentRelationList(activePreferred);
        }

        /// <summary>
        /// Returns an enumeration of the child data relations for this data source.
        /// </summary>
        /// <returns>The enumeration of the data relations.</returns>
        public IEnumerable<IStiAppDataRelation> FetchChildRelations(bool activePreferred)
        {
            return this.ChildRelationList(activePreferred);
        }

        /// <summary>
        /// Returns an array of values for the specified column in the specified position.
        /// </summary>
        /// <param name="names">An array of names of the data column.</param>
        /// <returns>The enumeration of the data column values.</returns>
        public IEnumerable<object[]> FetchColumnValues(string[] names)
        {
            var values = new List<object[]>();
            if (names == null || names.Length == 0)
                return values;

            var dataColumns = names.Select(name => Columns[name]);
            if (dataColumns.All(c => c == null))
                return values;

            var isNeedConnect = DataTable == null || DataTable.Rows.Count == 0;

            if (isNeedConnect)
                StiDataLeader.Connect(this);

            if (DataTable == null || DataTable.Rows.Count == 0)
                return values;

            First();
            foreach (DataRow row in DataTable.Rows)
            {
                var items = new object[dataColumns.Count()];
                var itemIndex = 0;
                foreach (var dataColumn in dataColumns)
                {
                    if (dataColumn is StiCalcDataColumn)
                    {
                        var calcColumnExpression = "{" + (dataColumn as StiCalcDataColumn).Value + "}";
                        var value = StiReportParser.Parse(calcColumnExpression, Dictionary.Report.Pages[0], false, null, false);

                        try
                        {
                            items[itemIndex] = StiConvert.ChangeType(value, dataColumn.Type);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        var dataColumnIndex = dataTable.Columns.IndexOf(dataColumn.NameInSource);
                        var value = row[dataColumnIndex];
                        items[itemIndex] = value;
                    }

                    itemIndex++;
                }

                values.Add(items);

                Next();
            }

            First();

            if (isNeedConnect)
                StiDataLeader.Disconnect(this);

            return values;
        }
        #endregion

        #region IStiAppCell
        string IStiAppCell.GetKey()
        {
            Key = StiKeyHelper.GetOrGeneratedKey(Key);

            return Key;
        }

        void IStiAppCell.SetKey(string key)
        {
            Key = key;
        }

        [Browsable(false)]
        [DefaultValue(false)]
        [StiSerializable]
        public bool Inherited { get; set; }
        #endregion

        #region IEnumerator
        object IEnumerator.Current => this;

        bool IEnumerator.MoveNext()
        {
            Next();

            return !IsEof;
        }

        void IEnumerator.Reset()
        {
            First();
        }
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
        #endregion

        #region IStiName
        private string name;
        /// <summary>
        /// Gets or sets of the Data Source name.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [Description("Gets or sets of the Data Source name.")]
        [ParenthesizePropertyName(true)]
        [StiOrder((int)Order.Name)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (Dictionary != null &&
                    Dictionary.Report != null &&
                    Dictionary.Report.IsDesigning && StiOptions.Designer.AutoCorrectDataSourceName)
                {
                    value = StiNameValidator.CorrectName(value, Dictionary.Report);
                }

                name = value;
            }
        }

        #endregion

        #region IStiAlias
        /// <summary>
		/// Gets or sets of the Data Source alias.
		/// </summary>
		[StiSerializable]
        [StiCategory("Data")]
        [Description("Gets or sets of the Data Source alias.")]
        [ParenthesizePropertyName(true)]
        [StiOrder((int)Order.Alias)]
        public string Alias { get; set; }
        #endregion

        #region IStiEnumerator
        protected int positionValue = 0;
        /// <summary>
        /// Gets the current position.
        /// </summary>
        [Browsable(false)]
        public virtual int Position
        {
            get
            {
                return positionValue;
            }
            set
            {
                positionValue = value;
            }
        }

        [Browsable(false)]
        public int RealCount
        {
            get
            {
                if (DetailRows != null)
                    return DetailRows.Length;

                return DataTable == null ? 0 : DataTable.Rows.Count;
            }
        }

        /// <summary>
        /// Gets count of elements.
        /// </summary>
        [Browsable(false)]
        public int Count
        {
            get
            {
                if (RealCount == 0 && StiOptions.Engine.EmulateData)
                {
                    if (DataTable == null)
                        return 1;

                    if (DataTable != null && DataTable.Rows.Count == 0)
                        return 1;
                }

                return RealCount;
            }
        }

        protected bool isBofValue;
        /// <summary>
        /// Gets value indicates that this position specifies to the beginning of data.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsBof
        {
            get
            {
                if (IsEmpty)
                    return true;

                return isBofValue;
            }
            set
            {
                isBofValue = value;
            }
        }

        protected bool isEofValue;
        /// <summary>
        /// Gets value indicates that this position specifies to the data end.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsEof
        {
            get
            {
                if (IsEmpty)
                    return true;

                return isEofValue;
            }
            set
            {
                isEofValue = value;
            }
        }

        /// <summary>
        /// Gets value indicates that no data.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsEmpty => Count == 0;

        /// <summary>
		/// Sets a position at the beginning.
		/// </summary>
		public virtual void First()
        {
            positionValue = 0;

            isEofValue = false;
            isBofValue = true;
        }

        /// <summary>
        /// Sets a position on the previous element.
        /// </summary>
        public virtual void Prior()
        {
            isBofValue = false;
            isEofValue = false;

            if (positionValue <= 0)
                isBofValue = true;
            else
                positionValue--;
        }

        /// <summary>
        /// Sets a position on the next element.
        /// </summary>
        public virtual void Next()
        {
            isBofValue = false;
            isEofValue = false;

            if (positionValue >= Count - 1)
                isEofValue = true;
            else
                positionValue++;
        }

        /// <summary>
        /// Sets a position on the last element.
        /// </summary>
        public virtual void Last()
        {
            positionValue = Count - 1;

            isEofValue = true;
            isBofValue = false;
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone()
        {
            var clonedDataSource = (StiDataSource)this.MemberwiseClone();

            clonedDataSource.columns = new StiDataColumnsCollection(clonedDataSource);
            foreach (StiDataColumn column in this.columns)
            {
                var clonedColumn = (StiDataColumn)column.Clone();
                clonedColumn.Key = StiGuidUtils.NewGuid();
                clonedColumn.DataSource = clonedDataSource;
                clonedDataSource.Columns.Add(clonedColumn);
            }

            if (this is StiSqlSource)
            {
                clonedDataSource.Parameters = new StiDataParametersCollection(clonedDataSource);
                foreach (StiDataParameter parameter in this.Parameters)
                {
                    var clonedParameter = (StiDataParameter)parameter.Clone();
                    clonedParameter.Key = StiGuidUtils.NewGuid();
                    clonedParameter.DataSource = clonedDataSource;
                    clonedDataSource.Parameters.Add(clonedParameter);
                };
            }

            return clonedDataSource;
        }
        #endregion

        #region IStiStateSaveRestore
        private StiStatesManager states = null;
        /// <summary>
        /// Gets the component states manager.
        /// </summary>
        protected StiStatesManager States
        {
            get
            {
                return states ?? (states = new StiStatesManager());
            }
        }

        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public virtual void SaveState(string stateName)
        {
            States.PushInt(stateName, this, "positionValue", positionValue);
            States.PushBool(stateName, this, "isEofValue", isEofValue);
            States.PushBool(stateName, this, "isBofValue", isBofValue);

            if (DetailRows != null && this is StiSqlSource && ((this as StiSqlSource).ReconnectOnEachRow))
            {
                var dt = dataTable.Clone();
                for (int index = 0; index < DetailRows.Length; index++)
                {
                    dt.ImportRow(DetailRows[index]);
                    DetailRows[index] = dt.Rows[index];
                }
            }

            if (this is StiSqlSource)
                States.PushBool(stateName, this, "reconnectOnEachRow", (this as StiSqlSource).ReconnectOnEachRow);

            States.Push(stateName, this, "detailRows", DetailRows);
            States.Push(stateName, this, "resFilterMethod", resFilterMethod);
            States.PushBool(stateName, this, "IsInited", IsInited);
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public virtual void RestoreState(string stateName)
        {
            if (!States.IsExist(stateName, this)) return;

            isBofValue = States.PopBool(stateName, this, "isBofValue");
            isEofValue = States.PopBool(stateName, this, "isEofValue");
            positionValue = States.PopInt(stateName, this, "positionValue");
            DetailRows = (DataRow[])States.Pop(stateName, this, "detailRows");
            resFilterMethod = States.Pop(stateName, this, "resFilterMethod");
            IsInited = States.PopBool(stateName, this, "IsInited");

            if (this is StiSqlSource)
                (this as StiSqlSource).ReconnectOnEachRow = States.PopBool(stateName, this, "reconnectOnEachRow");

            States.ClearState(stateName);
        }

        /// <summary>
        /// Clear all earlier saved object states.
        /// </summary>
        public virtual void ClearAllStates()
        {
            states = null;
        }
        #endregion

        #region Prepare data
        private bool IsEqualSort(string[] sortColumns1, string[] sortColumns2)
        {
            if (sortColumns1 == null && sortColumns2 == null)
                return true;

            if (sortColumns1 != null && sortColumns2 == null)
                return false;

            if (sortColumns1 == null && sortColumns2 != null)
                return false;

            if (sortColumns1.Length != sortColumns2.Length)
                return false;

            int index = 0;
            foreach (string str in sortColumns1)
            {
                if (str != sortColumns2[index])
                    return false;

                index++;
            }
            return true;
        }

        public void SetData(StiDataBand dataBand,
            string relationName, object filterMethod, string[] sortColumns, bool reinit,
            StiComponent component)
        {
            var reconnectOnEachRow = false;
            if (this is StiSqlSource)
                reconnectOnEachRow = ((StiSqlSource)this).ReconnectOnEachRow;

            if (!FiltersEqual(resFilterMethod, filterMethod))
                IsInited = false;

            if (!IsEqualSort(sortColumns, resSortColumns))
                IsInited = false;

            if (dataBand != null)
            {
                if (dataBand.Name != NameOfDataBandWhichInitDataSource)
                    IsInited = false;

                NameOfDataBandWhichInitDataSource = dataBand.Name;

                if (dataBand.MultipleInitialization)
                    IsInited = false;
            }

            if ((!reconnectOnEachRow) && IsInited && (!reinit) && (!InitForSubreport) && relationName == relationNameStored) return;

            if (dataBand != null)
            {
                StiReport report = dataBand.Report;
                if ((report == null) && (component != null))
                    report = component.Report;

                if ((report != null) && (report.CacheTotals) && (report.CachedTotals != null) && (!report.CachedTotalsLocked))
                    report.CachedTotals[dataBand] = null;
            }

            InitForSubreport = false;

            resFilterMethod = filterMethod;
            resSortColumns = sortColumns;

            StiComponentsCollection groupHeaderComponents = null;
            if (dataBand != null)
            {
                if (dataBand.Report.EngineVersion == StiEngineVersion.EngineV1)
                    groupHeaderComponents = dataBand.DataBandInfoV1.GroupHeaderComponents;
                else
                {
                    groupHeaderComponents = dataBand.DataBandInfoV2.GroupHeaders;
                    dataBand.DataBandInfoV2.GroupHeaderCachedResults = null;
                    dataBand.DataBandInfoV2.GroupFooterCachedResults = null;
                }
            }

            DetailRows = null;
            SetDetails(relationName);

            if (StiOptions.Engine.FilterDataInDataSourceBeforeSorting) SetFilter(filterMethod);

            var conditions = GetConditions(dataBand);
            SetSort(conditions, sortColumns, component, dataBand, groupHeaderComponents);

            if (!StiOptions.Engine.FilterDataInDataSourceBeforeSorting) SetFilter(filterMethod);

            IsInited = true;
        }

        private bool FiltersEqual(object filter1, object filter2)
        {
            if (filter1 == null && filter2 == null) return true;
            if (filter1 == null && filter2 != null) return false;
            if (filter1 != null && filter2 == null) return false;
            return filter1.Equals(filter2);
        }

        internal object[,,] GetConditions(StiDataBand dataBand)
        {
            object[,,] conditions = null;

            #region Sort by group conditions
            if (dataBand == null)return null;

            StiComponentsCollection groupHeaderComponents;

            if (dataBand.Report.EngineVersion == StiEngineVersion.EngineV1)
                groupHeaderComponents = dataBand.DataBandInfoV1.GroupHeaderComponents;
            else
                groupHeaderComponents = dataBand.DataBandInfoV2.GroupHeaders;

            if (groupHeaderComponents != null && groupHeaderComponents.Count > 0)
            {
                bool resIsEof = dataBand.IsEof;
                bool resIsBof = dataBand.IsBof;

                int groupCount = 0;
                foreach (StiGroupHeaderBand groupHeader in groupHeaderComponents)
                {
                    if (groupHeader.SortDirection != StiGroupSortDirection.None)
                        groupCount++;
                }

                if (groupCount == 0)
                    return null;

                conditions = new object[dataBand.Count, groupCount, 2];

                for (int index = 0; index < dataBand.Count; index++)
                {
                    dataBand.Position = index;
                    int groupIndex = 0;
                    foreach (StiGroupHeaderBand groupHeader in groupHeaderComponents)
                    {
                        if (groupHeader.SortDirection != StiGroupSortDirection.None)
                        {
                            if (dataBand.Report.EngineVersion == StiEngineVersion.EngineV1)
                                conditions[index, groupIndex, 0] = StiGroupHeaderBandV1Builder.GetCurrentConditionValue(groupHeader);
                            else
                                conditions[index, groupIndex, 0] = StiGroupHeaderBandV2Builder.GetCurrentConditionValue(groupHeader);

                            conditions[index, groupIndex, 1] = groupHeader.SortDirection;
                            groupIndex++;
                        }
                    }
                }

                dataBand.IsEof = resIsEof;
                dataBand.IsBof = resIsBof;
            }
            #endregion

            return conditions;
        }

        /// <summary>
        /// Set the details data for Data Source.
        /// </summary>
        /// <param name="relationName">Relation to find Parent of the Data Source.</param>
        public void SetDetails(string relationName)
        {
            #region ReconnectOnEachRow
            var sqlSource = this as StiSqlSource;
            if (sqlSource != null && sqlSource.ReconnectOnEachRow)
                sqlSource.UpdateParameters();
            #endregion

            if (string.IsNullOrEmpty(relationName)) return;

            var relation = Dictionary.Relations[relationName];
            if (relation == null) return;

            var table = relation.ParentSource;
            if (table == null) return;

            var dataTable = table.DataTable;
            if (dataTable == null) return;

            if (table.DetailRows != null)
            {
                if (table.DetailRows.Length != 0)
                {
                    if (table.Position < table.DetailRows.Length)
                        DetailRows = table.DetailRows[table.Position].GetChildRows(relationName);
                    else
                        DetailRows = new DataRow[0];
                }
                else
                    DetailRows = table.DetailRows;
            }
            else
            {
                if (table.Position < dataTable.Rows.Count)
                    DetailRows = dataTable.Rows[table.Position].GetChildRows(relationName);
            }
        }

        /// <summary>
        /// Apply filter to data.
        /// </summary>
        /// <param name="filterMethod"></param>
        public void SetFilter(object filterMethod)
        {
            if (isFilterProcessing || filterMethod == null || RealCount <= 0) return;

            isFilterProcessing = true;
            try
            {
                var resPos = Position;
                var resLine = Dictionary.Report.Line;
                var rows = new List<DataRow>();
                var count = Count;  //speed optimization

                if (filterMethod is StiFilterEventHandler)
                {
                    var filterMethod2 = (StiFilterEventHandler)filterMethod;
                    for (int pos = 0; pos < count; pos++)
                    {
                        Position = pos;
                        Dictionary.Report.Line = pos + 1;

                        try
                        {
                            var args = new StiFilterEventArgs();
                            filterMethod2(this, args);
                            if (args.Value)
                                rows.Add(DetailRows == null ? DataTable.Rows[pos] : DetailRows[pos]);
                        }
                        catch
                        {
                        }
                    }
                }
                else if (filterMethod is StiParser.StiFilterParserData)
                {
                    var data = (StiParser.StiFilterParserData)filterMethod;
                    StiParserParameters parms = new StiParserParameters() { ReturnAsmList = true };
                    var asmList = StiParser.ParseTextValue(data.Expression, data.Component, data.Component, parms);
                    for (int pos = 0; pos < count; pos++)
                    {
                        Position = pos;
                        Dictionary.Report.Line = pos + 1;

                        try
                        {
                            //var result = StiParser.ParseTextValue(data.Expression, data.Component);
                            var result = parms.Parser.ExecuteAsm(asmList);
                            if (result is bool && (bool)result)
                                rows.Add(DetailRows == null ? DataTable.Rows[pos] : DetailRows[pos]);
                        }
                        catch
                        {
                        }
                    }
                }

                DetailRows = rows.ToArray();

                Position = resPos;
                Dictionary.Report.Line = resLine;
            }
            finally
            {
                isFilterProcessing = false;
            }
        }

        /// <summary>
        /// Sorts data. If data source is not able to sort data then this command is ignored.
        /// </summary>
        /// <param name="conditions">Array with group conditions.
        /// Array with group conditions.
        /// 1 dimension - group condition.
        /// 2 dimension - sort direction.</param>
        /// <param name="sortColumns">Parameters of sorting.
        /// Example: new string[] {"ASC", "Company", "DESC", "ParentRelation", "DATE"}</param>
        /// <param name="component"></param>
        /// <param name="databand"></param>
        /// <param name="groupHeaders"></param>
		public void SetSort(object[,,] conditions, string[] sortColumns, StiComponent component,
            StiDataBand databand, StiComponentsCollection groupHeaders)
        {
            try
            {
                #region Sort data rows for hierarchical band
                if (component is StiHierarchicalBand)
                {
                    if (RealCount == 0) return;

                    if (DetailRows == null && DataTable != null)
                    {
                        DetailRows = new DataRow[DataTable.Rows.Count];
                        DataTable.Rows.CopyTo(DetailRows, 0);
                    }

                    #region Preparing group conditions
                    Hashtable rowToConditions = new Hashtable();
                    int index = 0;
                    foreach (DataRow row in DetailRows)
                    {
                        rowToConditions[row] = index++;
                    }
                    #endregion

                    var hierarchicalSort = new StiHierarchicalDataSort(this, component as StiHierarchicalBand, sortColumns);
                    hierarchicalSort.Process(rowToConditions);

                    return;
                }
                #endregion

                #region Sort data rows by groups, columns
                if ((conditions != null || sortColumns != null) && RealCount > 0)
                {
                    List<DataRow> listOfDetailRows = new List<DataRow>();

                    if (DetailRows == null && DataTable != null)
                    {
                        foreach (DataRow row in dataTable.Rows)
                        {
                            listOfDetailRows.Add(row);
                        }
                    }
                    if (DetailRows != null)
                    {
                        foreach (DataRow row in DetailRows)
                        {
                            listOfDetailRows.Add(row);
                        }
                    }

                    #region Preparing group conditions
                    Hashtable rowToConditions = new Hashtable();
                    int index = 0;
                    foreach (DataRow row in listOfDetailRows)
                    {
                        rowToConditions[row] = index++;
                    }
                    #endregion

                    #region Sort data rows array
                    StiDataSort dataSort = new StiDataSort(rowToConditions, conditions, sortColumns, this);
                    listOfDetailRows.Sort(dataSort);
                    #endregion

                    #region Copy sorted array of rows to detailsRows
                    int rowIndex = 0;
                    DetailRows = new DataRow[listOfDetailRows.Count];
                    foreach (DataRow row in listOfDetailRows)
                    {
                        DetailRows[rowIndex++] = row;
                    }
                    #endregion

                    #region Check for group summary sorting
                    bool groupSummarySorting = false;
                    if (groupHeaders != null)
                    {
                        foreach (StiGroupHeaderBand group in groupHeaders)
                        {
                            if (group.SummarySortDirection != StiGroupSortDirection.None)
                            {
                                groupSummarySorting = true;
                                break;
                            }
                        }
                    }
                    #endregion

                    if (groupSummarySorting)
                    {
                        Hashtable groupSummaries = new Hashtable();
                        Hashtable groupLines = new Hashtable();
                        int[] groupLinesArray = new int[groupHeaders.Count];
                        Hashtable baseRowOrder = new Hashtable();

                        int pos = 0;
                        foreach (DataRow row in listOfDetailRows)
                        {
                            databand.Position = pos;
                            baseRowOrder[row] = pos;
                            StiDataBandV2Builder.PrepareGroupResults(databand);
                            int groupIndex = 0;
                            foreach (StiGroupHeaderBand group in groupHeaders)
                            {
                                if (databand.DataBandInfoV2.GroupHeaderResults[groupIndex] && (group.Report != null))
                                {
                                    StiReport tempReport = group.Report;
                                    if ((tempReport.CacheTotals) && (tempReport.CachedTotals != null))
                                    {
                                        tempReport.CachedTotals[group] = null;
                                    }
                                }

                                #region Calculate Group Summary
                                string methodName = null;
                                object bandData = group;
                                if (group.SummaryType != StiGroupSummaryType.Count)
                                {
                                    if (group.Report.CalculationMode == StiCalculationMode.Compilation)
                                    {
                                        methodName = group.Name + "__GetSummaryExpression";
                                        if (Totals.GetMethod(group.Report, methodName) == null) continue;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(group.SummaryExpression.Value)) continue;
                                        var parserParameters = new StiParserParameters() { ExecuteIfStoreToPrint = false, ReturnAsmList = true };
                                        List<StiParser.StiAsmCommand> asmList = (List<StiParser.StiAsmCommand>)StiParser.ParseTextValue(group.SummaryExpression.Value, group, parserParameters);
                                        bandData = new StiParser.StiParserData(group, asmList, parserParameters.Parser);
                                    }
                                }
                                object value = null;

                                #region Process Summary Type
                                switch (group.SummaryType)
                                {
                                    case StiGroupSummaryType.Avg:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.Avg(bandData, group.Report, methodName), typeof(decimal));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.AvgDate:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.AvgDate(bandData, group.Report, methodName), typeof(DateTime));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.AvgTime:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.AvgTime(bandData, group.Report, methodName), typeof(TimeSpan));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.Count:
                                        try
                                        {
                                            value = Totals.Count(group);
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.CountDistinct:
                                        try
                                        {
                                            value = Totals.CountDistinct(group, group.Report, methodName);
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.Max:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.Max(bandData, group.Report, methodName), typeof(decimal));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.MaxDate:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.MaxDate(bandData, group.Report, methodName), typeof(DateTime));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.MaxTime:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.MaxTime(bandData, group.Report, methodName), typeof(TimeSpan));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.Median:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.Median(bandData, group.Report, methodName), typeof(decimal));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.Min:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.Min(bandData, group.Report, methodName), typeof(decimal));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.MinDate:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.MinDate(bandData, group.Report, methodName), typeof(DateTime));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.MinTime:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.MinTime(bandData, group.Report, methodName), typeof(TimeSpan));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.Mode:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.Mode(bandData, group.Report, methodName), typeof(decimal));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.Sum:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.Sum(bandData, group.Report, methodName), typeof(decimal));
                                        }
                                        catch
                                        {
                                        }
                                        break;

                                    case StiGroupSummaryType.SumTime:
                                        try
                                        {
                                            value = Convert.ChangeType(Totals.SumTime(bandData, group.Report, methodName), typeof(TimeSpan));
                                        }
                                        catch
                                        {
                                        }
                                        break;
                                }
                                #endregion
                                #endregion

                                #region Increase group line of we start process new group
                                if (databand.DataBandInfoV2.GroupHeaderResults[groupIndex])
                                {
                                    groupLinesArray[groupIndex]++;
                                }
                                #endregion

                                #region Store summary value to groupSummaries array
                                Hashtable rows = groupSummaries[group] as Hashtable;
                                if (rows == null)
                                {
                                    rows = new Hashtable();
                                    groupSummaries[group] = rows;
                                }
                                rows[row] = value;
                                #endregion

                                #region Store group line index to groupLines array
                                rows = groupLines[group] as Hashtable;
                                if (rows == null)
                                {
                                    rows = new Hashtable();
                                    groupLines[group] = rows;
                                }
                                rows[row] = groupLinesArray[groupIndex];
                                #endregion

                                groupIndex++;
                            }
                            pos++;
                        }

                        if (groupSummaries != null && groupSummaries.Count > 0)
                        {
                            databand.DataBandInfoV2.GroupHeaderCachedResults = null;
                            databand.DataBandInfoV2.GroupFooterCachedResults = null;

                            StiGroupSummaryDataSort groupDataSort = new StiGroupSummaryDataSort(groupSummaries, groupLines, groupHeaders, baseRowOrder);
                            listOfDetailRows.Sort(groupDataSort);

                            #region Copy sorted rows
                            rowIndex = 0;
                            DetailRows = new DataRow[listOfDetailRows.Count];
                            foreach (DataRow row in listOfDetailRows)
                            {
                                DetailRows[rowIndex++] = row;
                            }
                            #endregion


                            groupDataSort.Clear();
                        }
                    }

                    dataSort.Clear();
                }
                #endregion

            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "SetSort...ERROR");
                StiLogService.Write(this.GetType(), e);
                if (!StiOptions.Engine.HideExceptions) throw;
            }
        }


        public void ResetDetailsRows()
        {
            DetailRows = null;
        }


        public void ResetData()
        {
            IsInited = false;
            relationNameStored = null;
            resFilterMethod = null;
            resSortColumns = null;
        }
        #endregion

        #region Relations
        internal DataRow GetDataRow(int index)
        {
            if (DetailRows == null)
                return DataTable != null && index < DataTable.Rows.Count ? DataTable.Rows[index] : null;

            else
                return index < DetailRows.Length ? DetailRows[index] : null;
        }

        /// <summary>
        /// Returns the parent row with data for the indicated relation.
        /// </summary>
        /// <param name="relation">Relation name.</param>
        /// <returns>Relation string.</returns>
        public StiDataRow GetParentData(string relation)
        {
            var dataRow = GetDataRow(Position);
            if (dataRow == null)            
                return new StiDataRow(Dictionary.Relations[relation]?.ParentSource, dataRow);
            
            var rel = this.Dictionary.Relations[relation];
            if (rel != null)
                relation = rel.NameInSource;

            var rows = dataRow.GetParentRows(relation);
            if (rows != null && rows.Length > 0)
                dataRow = rows[0];
            else
                dataRow = null;

            return new StiDataRow(Dictionary.Relations[relation]?.ParentSource, dataRow);
        }

        /// <summary>
        /// Returns the colection of Parent relations.
        /// </summary>
        /// <returns>Collection.</returns>
        public virtual StiDataRelationsCollection GetParentRelations()
        {
            var relations = new StiDataRelationsCollection(Dictionary);
            if (Dictionary == null)
                return relations;

            foreach (StiDataRelation relation in Dictionary.Relations)
            {
                if (relation.ChildSource == this)
                    relations.Add(relation);
            }

            return relations;
        }

        /// <summary>
        /// Returns a collection of Child relations.
        /// </summary>
        /// <returns>Collection.</returns>
        public virtual StiDataRelationsCollection GetChildRelations()
        {
            var relations = new StiDataRelationsCollection(Dictionary);
            foreach (StiDataRelation relation in Dictionary.Relations)
            {
                if (relation.ParentSource == this)
                    relations.Add(relation);
            }
            return relations;
        }

        /// <summary>
        /// Returns the parent Data Source by the relation name.
        /// </summary>
        /// <param name="relationName">Relation name.</param>
        /// <returns>Data Source.</returns>
        public virtual StiDataSource GetParentDataSource(string relationName, bool allowRelationName = false)
        {
            foreach (StiDataRelation relation in Dictionary.Relations)
            {
                if (relation.ChildSource == this && relation.NameInSource == relationName)
                    return relation.ParentSource;

                if (relation.ChildSource == this && allowRelationName && relation.Name == relationName)
                    return relation.ParentSource;
            }

            #region Try to search relation with corrected name
            foreach (StiDataRelation relation in Dictionary.Relations)
            {
                if (relation.ChildSource == this && StiNameValidator.CorrectName(relation.NameInSource, Dictionary?.Report) == relationName)
                    return relation.ParentSource;
            }
            #endregion

            return null;
        }

        /// <summary>
        /// Returns the child Data Source by the relation name.
        /// </summary>
        /// <param name="relationName">Relation name.</param>
        /// <returns>Data Source.</returns>
        public virtual StiDataSource GetChildDataSource(string relationName)
        {
            foreach (StiDataRelation relation in Dictionary.Relations)
            {
                if (relation.ParentSource == this && relation.NameInSource == relationName)
                    return relation.ChildSource;
            }

            return null;
        }
        #endregion

        #region Connect
        public event EventHandler Connecting;

        public event EventHandler Disconnecting;

        protected void InvokeConnecting()
        {
            try
            {
                this.Connecting?.Invoke(this, EventArgs.Empty);

                var isCompilationMode = this.Dictionary?.Report?.CalculationMode != StiCalculationMode.Interpretation;
                if (isCompilationMode) return;

                var tempText = new StiText
                {
                    Name = "**DataSourceParameter**",
                    Page = this.Dictionary.Report.Pages[0]
                };

                foreach (StiDataParameter param in this.Parameters)
                {
                    param.ParameterValue = StiParser.ParseTextValue("{" + param.Value + "}", tempText);
                }

                var dataSource = this as StiSqlSource;
                if (dataSource == null) return;

                object baseSqlCommand = null;
                if (this.Dictionary.Report.Variables != null && this.Dictionary.Report.Variables["**StoredDataSourceSqlCommandForInterpretationMode**" + dataSource.Name] != null)
                    baseSqlCommand = this.Dictionary.Report.Variables["**StoredDataSourceSqlCommandForInterpretationMode**" + dataSource.Name];

                if (baseSqlCommand != null && baseSqlCommand is string)
                {
                    tempText.Name = "**DataSourceSqlCommand**";
                    dataSource.SqlCommand = global::System.Convert.ToString(StiParser.ParseTextValue(baseSqlCommand as string, tempText));
                }
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), $"Datasource \'{Name}\' connecting...ERROR");
                StiLogService.Write(this.GetType(), e);
                if (!StiOptions.Engine.HideExceptions) throw;
            }
        }

        protected void InvokeDisconnecting()
        {
            try
            {
                this.Disconnecting?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Datasource '" + Name + "' disconnecting...ERROR");
                StiLogService.Write(this.GetType(), e);
                if (!StiOptions.Engine.HideExceptions) throw;
            }
        }

        public void Connect()
        {
            StiDataLeader.Connect(this);
        }

        public void Connect(bool loadData, bool invokeEvents = true)
        {
            Connect(null, loadData, invokeEvents);
        }

        public void Connect(StiDataCollection datas)
        {
            Connect(datas, true);
        }

        /// <summary>
        /// Connect Data Source to data. It is necessary to call the parent of the method(base.Connect).
        /// </summary>
        /// <param name="loadData">Load data or no.</param>
        public virtual void Connect(StiDataCollection datas, bool loadData, bool invokeEvents = true)
        {
            if (isConnectProcessing) return;
            isConnectProcessing = true;

            if (this is StiSqlSource && string.IsNullOrWhiteSpace((this as StiSqlSource).SqlCommand))
                invokeEvents = true;

            if (invokeEvents && Dictionary != null && !Dictionary.UseInternalData)
                InvokeConnecting();

            StiDataLeader.ConnectDataSourceToData(GetDataAdapter(), Dictionary, this, loadData);

            #region Emulate data
            if ((DataTable == null || RealCount == 0) && StiOptions.Engine.EmulateData && Dictionary.Report.IsRendering)
            {
                #region Create datatable
                var newTable = DataTable;
                if (newTable == null)
                {
                    newTable = new DataTable();

                    foreach (StiDataColumn column in Columns)
                    {
                        newTable.Columns.Add(new DataColumn(column.NameInSource, column.Type));
                    }
                }
                #endregion

                #region Fill Data
                try
                {
                    var row = newTable.NewRow();
                    foreach (StiDataColumn column in Columns)
                    {
                        var conversionType = column.Type;
                        object value = DBNull.Value;

                        if (conversionType == typeof(string))
                        {
                            if (DataTable != null && DataTable.Columns[column.NameInSource] != null &&
                                DataTable.Columns[column.NameInSource].GetType() == typeof(string))
                                value = "Test";
                            else
                                value = "";
                        }
                        else if (conversionType == typeof(bool))
                            value = true;

                        else if (conversionType == typeof(DateTime))
                            value = DateTime.Now;

                        else if (conversionType == typeof(char))
                            value = ' ';

                        else if (conversionType.IsPrimitive)
                            value = global::System.Convert.ChangeType(0, conversionType);

                        row[column.NameInSource] = value;
                    }

                    newTable.Rows.Add(row);
                }
                catch
                {
                }

                DataTable = null;
                DataTable = newTable;
                #endregion
            }
            #endregion

            isConnectProcessing = false;
        }

        public virtual void Disconnect()
        {
            InvokeDisconnecting();

            if (DataTable != null)
            {
                if (Dictionary != null &&
                    Dictionary.CacheDataSet != null &&
                    Dictionary.CacheDataSet.Tables.IndexOf(DataTable) != -1)
                {
                    int index = 0;
                    while (index < this.Dictionary.CacheDataSet.Relations.Count)
                    {
                        DataRelation relation = this.Dictionary.CacheDataSet.Relations[index];
                        if (relation.ParentTable == DataTable)
                            relation.ChildTable.Constraints.Remove(relation.RelationName);

                        if (relation.ChildTable == DataTable && relation.ParentTable.Constraints.Contains(relation.RelationName))
                            relation.ParentTable.Constraints.Remove(relation.RelationName);

                        if (relation.ParentTable == DataTable || relation.ChildTable == DataTable)
                            this.Dictionary.CacheDataSet.Relations.RemoveAt(index);
                        else
                            index++;
                    }

                    DataTable.ChildRelations.Clear();
                    DataTable.ParentRelations.Clear();
                    DataTable.Constraints.Clear();
                    this.Dictionary.CacheDataSet.Tables.Remove(DataTable);
                }
                DataTable = null;
            }
            RowToLevel = null;
            DetailRows = null;
            isConnectProcessing = false;
        }

        /// <summary>
        /// Gets value indicates which Data Source is connected with data.
        /// </summary>
        [Browsable(false)]
        public bool IsConnected => dataTable != null;
        #endregion

        #region DataAdapter
        protected abstract Type GetDataAdapterType();

        protected virtual string DataAdapterType => GetDataAdapterType().ToString();

        public void FillColumns()
        {
            try
            {
                if (this is StiDBaseSource || this is StiCsvSource)
                {
                    this.Columns.Clear();
                    this.Dictionary.SynchronizeColumns(null, this);
                    return;
                }

                var adapter = StiDataAdapterService.GetDataAdapter(this);
                if (adapter == null) return;

                var storeSource = this as StiDataStoreSource;
                if (storeSource == null || storeSource.NameInSource == null || storeSource.NameInSource.Trim().Length == 0) return;

                var selectedData = this.Dictionary.DataStore.ToList().FirstOrDefault(data => data.Name.ToLowerInvariant() == storeSource.NameInSource.ToLowerInvariant());
                if (selectedData == null) return;

                var sqlSource = this as StiSqlSource;
                this.Columns.Clear();

                if (sqlSource != null)
                {
                    var columns = StiDataLeader.GetColumnsFromData(adapter, selectedData, this);
                    this.columns.AddRange(columns);
                }
                else
                    this.Dictionary.SynchronizeColumns(selectedData, this);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), e);
                if (!StiOptions.Engine.HideExceptions) throw;
            }
        }

        public virtual StiDataAdapterService GetDataAdapter()
        {
            var adapters = StiOptions.Services.DataAdapters.Where(s => s.ServiceEnabled);

            var adapterType = GetDataAdapterType();
            if (adapterType != null)
                return StiActivator.CreateObject(adapterType) as StiDataAdapterService;

            var adapter = Dictionary != null && Dictionary.UseInternalData ? adapters.FirstOrDefault(d => d is StiDataTableAdapterService) : null;
            if (adapter != null)
                return adapter;

            return adapters.FirstOrDefault(d => d.GetType().ToString() == DataAdapterType);
        }
        #endregion

        #region Data
        public DataTable GetDataTable(DataTable table)
        {
            DataTable newTable = table.Clone();

            int[] columnsIndex = new int[newTable.Columns.Count];
            for (int index = 0; index < newTable.Columns.Count; index++)
            {
                DataColumn column = newTable.Columns[index];
                columnsIndex[index] = table.Columns.IndexOf(column.ColumnName);
            }

            for (int index = 0; index < table.Rows.Count; index++)
            {
                DataRow row = newTable.NewRow();
                for (int index2 = 0; index2 < newTable.Columns.Count; index2++)
                {
                    try
                    {
                        row[index2] = table.Rows[index][columnsIndex[index2]];
                    }
                    catch
                    {
                    }
                }
                newTable.Rows.Add(row);
            }

            return newTable;
        }

        public DataTable GetDataTable()
        {
            try
            {
                if (DataTable != null)
                    return GetDataTable(DataTable);

                return new DataTable(Name);
            }
            catch
            {
                if (!StiOptions.Engine.HideExceptions) throw;
            }
            return new DataTable(Name);
        }

        /// <summary>
        /// Indexer to work with the current row of data.
        /// Returns the value by the column name and the current position in the Data Source. 
        /// Indexer calls the method GetData.
        /// ColumnName - name of column.
        /// </summary>
        public virtual object this[string columnName]
        {
            get
            {
                return GetData(columnName);
            }
        }

        /// <summary>
        /// Returns the value by the column name and the current position in the Data Source.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value.</returns>
        public object GetData(string columnName)
        {
            try
            {
                return GetData(columnName, Position);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), e);
                if (!StiOptions.Engine.HideExceptions) throw;
            }
            return null;
        }

        /// <summary>
        /// Returns the value by the column name and index of row in the Data Source.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="index">Index of row in the Data Source.</param>
        /// <returns>Value.</returns>
        public object GetData(string columnName, int index)
        {
            if (IsConnected)
            {
                int columnIndex = GetColumnIndex(columnName);
                if (columnIndex < 0)
                    return null;

                var row = GetDataRow(index);
                if (row == null)
                    return null;

                return row[columnIndex];
            }
            else
            {
                //Try to find data in the dbs cache. If not than load it to cache
                var dataTable = StiDataPicker.GetFromCache(this);
                if (dataTable == null && Dictionary?.Report != null && Dictionary.Report.Pages.ToList().Any(p => p.IsPage))
                {
                    StiDataLeader.Connect(this, true, false);

                    dataTable = GetDataTable();
                    dataTable = dataTable?.Copy();
                    dataTable = StiDataPicker.ProcessCalculatedColumns(dataTable, this);
                    StiDataPicker.AddToCache(this, dataTable);

                    StiDataLeader.Disconnect(this);
                }

                if (dataTable != null)
                {
                    return 
                        dataTable.Columns.Contains(columnName) && index >= 0 && index < dataTable.Rows.Count 
                        ? dataTable.Rows[index][columnName] 
                        : DBNull.Value;
                }
                return DBNull.Value;
            }
        }

        /// <summary>
        /// Returns the index of a column in the data source.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>Index.</returns>
        public int GetColumnIndex(string columnName)
        {
            if (ColumnIndexes != null)
            {
                var index = ColumnIndexes[columnName];

                if ((index == null || (index is int && (int) index == -1)) && index == null)
                    index = ColumnIndexes[columnName.ToLower(CultureInfo.InvariantCulture)];

                if (index == null)
                    return -1;

                if ((int)index == -1)
                {
                    var message = $"Column '{columnName}' from data source '{this.Name}' not found in table '{DataTable.TableName}'";

                    if (StiOptions.Engine.AllowThrowExceptionWhenColumnDoesNotExists)
                        throw new Exception(message);
                    else
                        StiLogService.Write(this.GetType(), message);

                }

                return (int)index;
            }
            else
            {
                int index = 0;
                foreach (StiDataColumn column in this.Columns)
                {
                    if (column.NameInSource == columnName)
                        return index;

                    index++;
                }
                index = 0;

                foreach (StiDataColumn column in this.Columns)
                {
                    if (column.Name == columnName || column.Alias == columnName)
                        return index;

                    index++;
                }
                return -1;
            }
        }

        /// <summary>
        /// Gets or sets rows collection.
        /// </summary>
        [Browsable(false)]
        public StiRowsCollection Rows { get; set; }
        #endregion

        #region Fields
        private string relationNameStored;

        /// <summary>
        /// Protection against looping when using aggregate functions in a filter expression.
        /// </summary>
        private bool isFilterProcessing;
        /// <summary>
        /// Protection against looping when using expressions in a parameter expression.
        /// </summary>
        private bool isConnectProcessing;
        private object resFilterMethod;
        private string[] resSortColumns;        
        #endregion

        #region Methods
        /// <summary>
        /// Synchronize list of columns with columns in datastore.
        /// </summary>
        public void SynchronizeColumns()
        {
            var storeSource = this as StiDataStoreSource;
            if (storeSource == null) return;

            StiData selectedData = null;
            foreach (StiData data in Dictionary.DataStore)
            {
                if (data.Name.ToLowerInvariant() == storeSource.NameInSource.ToLowerInvariant())
                {
                    selectedData = data;
                    break;
                }
                else if (StiOptions.Dictionary.NotIdenticalNameAndAliasAtRegistrationOfNewData && data.Alias.ToLowerInvariant() == storeSource.NameInSource.ToLowerInvariant())
                {
                    data.Name = data.Alias;
                    selectedData = data;
                    break;
                }
            }
            if (selectedData == null) return;

            Dictionary.SynchronizeColumns(selectedData, this);
        }


        /// <summary>
        /// Internal use only.
        /// </summary>
        public void CheckColumnsIndexs()
        {
            if ((ColumnIndexes != null && ColumnIndexes.Keys.Count != 0) || (dataTable == null || DataTable.Columns.Count <= 0)) return;
            ColumnIndexes = new Hashtable();

            Hashtable hashColumns = new Hashtable();
            for (int index = 0; index < DataTable.Columns.Count; index++)
            {
                hashColumns[DataTable.Columns[index].ColumnName.ToLowerInvariant()] = index;
            }

            foreach (StiDataColumn column in Columns)
            {
                if (column is StiCalcDataColumn) continue;

                int index = -1;
                object obj = hashColumns[column.NameInSource.ToLowerInvariant()];

                if (obj != null)
                    index = (int)obj;

                ColumnIndexes[column.NameInSource.ToLowerInvariant()] = index;
                ColumnIndexes[column.NameInSource] = index;
                ColumnIndexes[column.Name] = index;
                ColumnIndexes[column.Alias] = index;
                ColumnIndexes[StiNameValidator.CorrectName(column.Name, Dictionary?.Report)] = index;
            }
        }

        /// <summary>
        /// Returns the text view of the Data Source.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool onlyAlias)
        {
            if (onlyAlias && !string.IsNullOrWhiteSpace(Alias))
                return Alias;

            if (Name == Alias || string.IsNullOrWhiteSpace(Alias))
                return Name;

            return $"{Name} [{Alias}]";
        }

        /// <summary>
        /// Returns level of data in hierarchical band.
        /// </summary>
        /// <returns></returns>
        public int GetLevel()
        {
            if (RowToLevel == null) return 0;
            DataRow row = null;
            if (DetailRows != null)
            {
                if (DetailRows.Length <= Position)
                    return 0;

                row = DetailRows[Position];
            }
            else
            {
                if (DataTable == null || DataTable.Rows.Count <= Position)
                    return 0;

                row = DataTable.Rows[Position];
            }

            object value = RowToLevel[row];
            if (value is int)
                return (int)value;

            return 0;
        }

        /// <summary>
        /// Returns the name to categories of the Data Source.
        /// </summary>
        public virtual string GetCategoryName()
        {
            var dataAdapter = StiDataAdapterService.GetDataAdapter(this);
            if (dataAdapter != null)
                return dataAdapter.ServiceName;

            return string.Empty;
        }

        public virtual StiDataSource CreateNew()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the key of the dictionary object.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [Browsable(false)]
        public string Key { get; set; }

        internal bool IsInited { get; set; }

        internal bool InitForSubreport { get; set; }

        protected Hashtable ColumnIndexes { get; set; }

        protected internal DataRow[] DetailRows { get; set; }

        protected internal Hashtable RowToLevel { get; set; }

        /// <summary>
        /// Gets or sets the parameter collection.
        /// </summary>
        [Browsable(false)]
        [TypeConverter(typeof(StiDataParametersCollectionConverter))]
        [Editor("Stimulsoft.Report.Dictionary.Design.StiDataParametersEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual StiDataParametersCollection Parameters { get; set; }

        private bool ShouldSerializeParameters()
        {
            return Parameters == null || Parameters.Count > 0;
        }

        internal string NameOfDataBandWhichInitDataSource { get; private set; }

        /// <summary>
		/// Gets or sets value which indicates that datasource not connect to the data automatically.
		/// </summary>
		[Browsable(false)]
        [StiCategory("Data")]
        [DefaultValue(true)]
        [Description("Gets or sets value which indicates that datasource not connect to the data automatically.")]
        public virtual bool ConnectOnStart { get; set; } = true;

        /// <summary>
		/// Gets or sets value which indicates in which order that datasource will be connect to the data.
		/// </summary>
		[Browsable(false)]
        [StiCategory("Data")]
        [Description("Gets or sets value which indicates in which order that datasource will be connect to the data.")]
        public virtual int ConnectionOrder { get; set; } = (int)StiConnectionOrder.Standard;

        private DataTable dataTable;
        /// <summary>
        /// Gets DataTable.
        /// </summary>
        [Browsable(false)]
        public DataTable DataTable
        {
            get
            {
                return dataTable;
            }
            set
            {
                dataTable = value;

                DetailRows = null;

                if (value != null)
                {
                    CheckColumnsIndexs();

                    if ((this is StiDataTableSource && Dictionary.Report.CacheAllData) || (dataTable?.DataSet == null))
                    {
                        if (this.Dictionary.CacheDataSet != null && DataTable?.TableName != null)
                        {
                            var index = 0;
                            var name = this.DataTable.TableName;
                            while (this.Dictionary.CacheDataSet.Tables[name] != null)
                            {
                                index++;
                                name = $"{DataTable.TableName}{index}";
                            }

                            if (this.DataTable.TableName != name)
                                this.DataTable.TableName = name;

                            this.Dictionary.CacheDataSet.Tables.Add(this.DataTable);
                        }
                    }
                }
                First();
            }
        }

        /// <summary>
        /// Gets or sets value which indicates that this data source is created dynamically by the Stimulsoft Server.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        [StiSerializable]
        public bool IsCloud { get; set; }

        /// <summary>
		/// Gets or sets the dictionary in which this Data Source is located.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public StiDictionary Dictionary { get; set; }        

        private StiDataColumnsCollection columns;
        /// <summary>
        /// Gets or sets the column collection.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [StiCategory("Data")]
        [Description("Gets or sets the column collection.")]
        [StiOrder((int)Order.Columns)]
        [TypeConverter(typeof(StiDataColumnsCollectionConverter))]
        [Editor("Stimulsoft.Report.Dictionary.Design.StiDataColumnsEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual StiDataColumnsCollection Columns
        {
            get
            {
                return columns;
            }
            set
            {
                if (columns != null)
                {
                    columns = value;
                    if (value != null)
                    {
                        value.DataSource = this;
                        value.ToList().ForEach(c => c.DataSource = this);
                    }
                }
            }
        }

        private bool ShouldSerializeColumns()
        {
            return Columns == null || Columns.Count > 0;
        }
        #endregion

        /// <summary>
        /// Creates a new Data Source.
        /// </summary>
        /// <param name="name">Data Source name.</param>
        /// <param name="alias">Data Source alias.</param>
        public StiDataSource(string name, string alias)
        {
            this.Name = name;
            this.Alias = alias;

            if (string.IsNullOrEmpty(this.Alias))
                this.Alias = name;
            
            Rows = new StiRowsCollection(this);
            columns = new StiDataColumnsCollection(this);
            Parameters = new StiDataParametersCollection(this);
            Key = StiKeyHelper.GenerateKey();
        }

        /// <summary>
        /// Creates a new Data Source.
        /// </summary>
        /// <param name="name">Data Source name.</param>
        /// <param name="alias">Data Source alias.</param>
        /// <param name="key">Key string</param>
        public StiDataSource(string name, string alias, string key) : this(name, alias)
        {
            this.Key = key;
        }
    }
}