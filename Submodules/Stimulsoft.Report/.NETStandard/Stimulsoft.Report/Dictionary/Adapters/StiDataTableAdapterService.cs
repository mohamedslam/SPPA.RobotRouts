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

using Stimulsoft.Base.Localization;
using System;
using System.Data;
using System.Globalization;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the adapter for access to DataTable.
    /// </summary>
    public class StiDataTableAdapterService : StiDataStoreAdapterService
	{
		#region StiService override
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => StiLocalization.Get("Adapters", "AdapterDataTables");
	    #endregion

        #region Properties
        public override bool IsObjectAdapter => true;
	    #endregion
		
		#region StiDataAdapterService override
		/// <summary>
		/// Returns name of category for data.
		/// </summary>
		public override string GetDataCategoryName(StiData data)
		{
			var dataName = data.Name;

			var table = data.ViewData as DataTable;
            if (table == null && data.ViewData is DataView) table = (data.ViewData as DataView).Table;

			if (table != null)
			{
				if (data.Name != null && data.Data is DataTable)
				{
                    var index = dataName.LastIndexOf(".", StringComparison.InvariantCulture);
                    var indexForTable = table.TableName.IndexOf(".");
                    if (indexForTable != -1)
                    {
                        if (dataName.Length > table.TableName.Length)
                            dataName = dataName.Substring(0, dataName.Length - table.TableName.Length - 1);
                    }
                    else if (index != -1 && dataName.IndexOf(table.TableName) != -1)
				        dataName = dataName.Substring(0, index);
				}
				else
				{
				    if (table.DataSet != null)
				        return table.DataSet.DataSetName;
				}
			}
			
			if (data.Alias != dataName && !string.IsNullOrEmpty(data.Alias))
			{
				if (StiOptions.Dictionary.ShowOnlyAliasForData && !string.IsNullOrWhiteSpace(data.Alias))
                    return data.Alias;

				dataName = $"{dataName} [{data.Alias}]";
			}
			return dataName;
		}

        /// <summary>
        /// Returns a collection of columns of data.
        /// </summary>
        /// <param name="data">Data to find column.</param>
        /// <returns>Collection of columns found.</returns>
        public override StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource)
        {
            return GetColumnsFromData(data, dataSource, CommandBehavior.SchemaOnly);
        }

        /// <summary>
        /// Returns a collection of columns of data.
        /// </summary>
        /// <param name="data">Data to find column.</param>
        /// <returns>Collection of columns found.</returns>
        public override StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource, CommandBehavior retrieveMode)
		{
			var dataColumns = new StiDataColumnsCollection();
			var table = data.ViewData as DataTable;
            if (table == null && data.ViewData is DataView) table = (data.ViewData as DataView).Table;

            if (table == null) return null;

			foreach (DataColumn column in table.Columns)
			{
                Type columnType = column.DataType;

                //many bugs with adapters and providers, so only through options
                if (column.AllowDBNull)
                {
                    if (StiOptions.Dictionary.UseAllowDBNullProperty)
                    {
                        try
                        {
                            if (columnType.IsValueType && !(columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                            {
                                columnType = typeof(Nullable<>).MakeGenericType(columnType);
                            }
                        }
                        catch
                        {
                        }
                    }

                    if (columnType == typeof(DateTime) && StiOptions.Dictionary.UseNullableDateTime)
                        columnType = typeof(DateTime?);

                    if (columnType == typeof(TimeSpan) && StiOptions.Dictionary.UseNullableTimeSpan)
                        columnType = typeof(TimeSpan?);
                }

                dataColumns.Add(new StiDataColumn(column.ColumnName, column.Caption, columnType));
			}
			foreach (StiDataColumn column in dataColumns)
			{
				var name = column.Name;
				var alias = column.Alias;
				var nameInSource = column.Name;

				if (StiOptions.Designer.AutoCorrectDataColumnName)
					column.Name = StiNameValidator.CorrectName(column.Name, dataSource?.Dictionary?.Report);

				column.NameInSource = nameInSource;
				column.Alias = nameInSource;

				if (alias != name)
				    column.Alias = alias;
			}
			return dataColumns;
		}

        /// <summary>
        /// Returns a collection of parameters of data.
        /// </summary>
        /// <param name="data">Data to find parameters.</param>
        /// <returns>Collection of parameters found.</returns>
        public override StiDataParametersCollection GetParametersFromData(StiData data, StiDataSource dataSource)
        {
            return new StiDataParametersCollection();
        }

		/// <summary>
		/// Fills a name and alias of the Data Source relying on data.
		/// </summary>
		/// <param name="data">Data relying on which names will be filled.</param>
		/// <param name="dataSource">Data source in which names will be filled.</param>
		public override void SetDataSourceNames(StiData data, StiDataSource dataSource)
		{
			base.SetDataSourceNames(data, dataSource);
			var table = data.ViewData as DataTable;

		    if (table == null && data.ViewData is DataView)
                table = (data.ViewData as DataView).Table;

            if (table != null)
            {
                dataSource.Name = table.TableName;
                dataSource.Alias = table.TableName;

                var newAlias = table.DisplayExpression;
                if (!string.IsNullOrEmpty(newAlias))
                {
                    newAlias = newAlias.Trim();

                    if (newAlias.StartsWith("'") && newAlias.EndsWith("'"))
                        dataSource.Alias = newAlias.Substring(1, newAlias.Length - 2);
                }
            }
		}

		/// <summary>
		/// Returns the type of the Data Source.
		/// </summary>
		/// <returns>The type of Data Source.</returns>
		public override Type GetDataSourceType()
		{
			return typeof(StiDataTableSource);
		}

	    /// <summary>
	    /// Returns the array of data types to which the Data Source may refer.
	    /// </summary>
	    /// <returns>Array of data types.</returns>
	    public override Type[] GetDataTypes()
	    {
	        return new[] { typeof (DataTable) };
	    }

        public StiData GetDataFromDataSource(StiDictionary dictionary, StiDataSource dataSource)
        {
            if (dataSource.Dictionary == null)
                return null;

            var dataStoreSource = dataSource as StiDataStoreSource;
            var nameInSource = dataStoreSource.NameInSource.ToLowerInvariant();

            #region Search data in datastore by NameInSource with full name equaling
            foreach (StiData data in dataSource.Dictionary.DataStore)
            {
                if (data != null && data.Name.ToLowerInvariant() == nameInSource && (data.ViewData is DataTable || data.ViewData is DataView))
                    return data;
            }
            #endregion

            if (!StiOptions.Dictionary.UseAdvancedDataSearch)
                return null;

            if (dataSource.Dictionary != null && dataSource.Dictionary.Report != null && dataSource.Dictionary.Report.IsDesigning)
                return null;

            #region Search data in datastore by NameInSource with partial name equaling
            string nameInSource2 = null;
            if (nameInSource.IndexOf(".", StringComparison.InvariantCulture) != -1)
                nameInSource2 = nameInSource.Substring(nameInSource.IndexOf(".", StringComparison.InvariantCulture) + 1);

            foreach (StiData data in dataSource.Dictionary.DataStore)
            {
                if (!(data.ViewData is DataTable || data.ViewData is DataView)) continue;
                var dataName = data.Name.ToLowerInvariant();

                if (dataName == nameInSource || dataName == nameInSource2)
                    return data;

                if (dataName.IndexOf(".", StringComparison.InvariantCulture) != -1)
                    dataName = dataName.Substring(dataName.IndexOf(".", StringComparison.InvariantCulture) + 1);

                if (dataName == nameInSource || dataName == nameInSource2)
                    return data;
            }
            #endregion

            if (!StiOptions.Dictionary.AllowConnectToFirstTableForEmptyDataSource)
                return null;

            #region Search data in datastore, get first DataTable when table count = 1 and only one StiDataTable dataSource 
            var tableCount = 0;
            StiData selectedData = null;

            foreach (StiData data in dataSource.Dictionary.DataStore)
			{				
				if (data.ViewData is DataTable || data.ViewData is DataView)
				{
					tableCount++;

                    if (tableCount > 1)
                        return null;

    			    selectedData = data;
				}				
			}

            if (tableCount == 1)
            {
                if (dataSource.Dictionary.DataSources.Count > 1)
                {
                    int countAdapters = 0;
                    foreach (StiDataSource ds in dataSource.Dictionary.DataSources)
                    {
                        if (ds.GetDataAdapter() is StiDataTableAdapterService)
                            countAdapters++;
                    }

                    if (countAdapters > 1)
                        return null;
                }
            }

            return selectedData;
            #endregion
		}

		public override void ConnectDataSourceToData(StiDictionary dictionary, StiDataSource dataSource, bool loadData)
		{
            StiDataLeader.Disconnect(dataSource);

            var data = GetDataFromDataSource(dictionary, dataSource);
			if (data == null)return;

            var selectedTable = data.ViewData as DataTable;
            if (selectedTable == null && data.ViewData is DataView)
                selectedTable = (data.ViewData as DataView).Table;

			if (selectedTable == null)return;

		    if (dataSource.Dictionary.Report.CacheAllData && loadData)
		        dataSource.DataTable = dataSource.GetDataTable(selectedTable);

		    else
		    {
		        if (dataSource.Dictionary.Report.CacheAllData)
		        {
		            dataSource.Dictionary.Report.CacheAllData = false;
		            dataSource.DataTable = selectedTable;
		            dataSource.Dictionary.Report.CacheAllData = true;
		        }
		        else
		            dataSource.DataTable = selectedTable;
		    }
		}

		#endregion
	}
}
