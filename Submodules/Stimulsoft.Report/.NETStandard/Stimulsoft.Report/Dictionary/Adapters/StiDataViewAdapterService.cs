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
    /// Describes the adapter for access to DataView.
    /// </summary>
    public class StiDataViewAdapterService : StiDataStoreAdapterService
	{
		#region StiService override
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => StiLocalization.Get("Adapters", "AdapterDataViews");
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
            if (table != null)
            {
                if (data.Name != null && data.Data is DataTable)
                {
                    var index = dataName.LastIndexOf(".", StringComparison.InvariantCulture);
                    if (index != -1)
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
                if (StiOptions.Dictionary.ShowOnlyAliasForData)
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
			var view = data.Data as DataView;

		    if (view.Table == null)
		        return dataColumns;

		    foreach (DataColumn column in view.Table.Columns)
		    {
		        dataColumns.Add(new StiDataColumn(column.ColumnName, column.Caption, column.DataType));
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
		/// <param name="dataSource">Data Source in which names will be filled.</param>
		public override void SetDataSourceNames(StiData data, StiDataSource dataSource)
		{
			base.SetDataSourceNames(data, dataSource);
			var view = data.Data as DataView;

			dataSource.Name = "view" + view.Table.TableName;
			dataSource.Alias = "view" + view.Table.TableName;
		}

		/// <summary>
		/// Returns the type of the Data Source.
		/// </summary>
		/// <returns>The type of Data Source.</returns>
		public override Type GetDataSourceType()
		{
			return typeof(StiDataViewSource);
		}

	    /// <summary>
	    /// Returns the array of data types to which the Data Source may refer.
	    /// </summary>
	    /// <returns>Array of data types.</returns>
	    public override Type[] GetDataTypes()
	    {
	        return new[] { typeof (DataView) };
	    }

	    public override void ConnectDataSourceToData(StiDictionary dictionary, StiDataSource dataSource, bool loadData)
		{
            StiDataLeader.Disconnect(dataSource);

            var dataStoreSource = dataSource as StiDataStoreSource;
			DataView selectedView = null;
			var nameInSource = dataStoreSource.NameInSource.ToLower(CultureInfo.InvariantCulture);

			#region Search data in datastore by NameInSource with full name equaling
			foreach (StiData data in dataSource.Dictionary.DataStore)
			{
				if (data.Name.ToLower(CultureInfo.InvariantCulture) == nameInSource && data.Data is DataView)
				{
					selectedView = data.Data as DataView;
					break;
				}
			}
			#endregion

			#region Search data in datastore by NameInSource with partial name equaling
			if (selectedView == null)
			{
				string nameInSource2 = null;
                if (nameInSource.StartsWith("view", StringComparison.InvariantCulture))
                    nameInSource2 = nameInSource.Substring(4);

				foreach (StiData data in dataSource.Dictionary.DataStore)
				{
                    if (!(data.Data is DataView)) continue;

					var dataName = data.Name.ToLower(CultureInfo.InvariantCulture);
					string dataName2 = null;

                    if (dataName.StartsWith("view", StringComparison.InvariantCulture))
                        dataName2 = dataName.Substring(4);

					if (dataName == nameInSource || dataName == nameInSource2 ||
						dataName2 == nameInSource || dataName2 == nameInSource2)
					{
						selectedView = data.Data as DataView;
						break;
					}
				}
			}
			#endregion

			if (selectedView != null)
				dataStoreSource.DataTable = GetDataTableFromData(selectedView, loadData);
		}

		private DataTable GetDataTableFromData(DataView view, bool loadData)
		{
			var newTable = view.Table.Clone();
			newTable.Rows.Clear();

		    if (!loadData)
		        return newTable;

		    for (var index = 0; index < view.Count; index++)
		    {
		        var row = newTable.NewRow();
		        foreach (DataColumn column in view.Table.Columns)
		        {
		            try
		            {
		                row[column.ColumnName] = view[index][column.ColumnName];
		            }
		            catch (Exception e)
		            {
		                StiLogService.Write(this.GetType(), e);
		            }
		        }
		        newTable.Rows.Add(row);
		    }
		    return newTable;
		}
		#endregion
	}
}
