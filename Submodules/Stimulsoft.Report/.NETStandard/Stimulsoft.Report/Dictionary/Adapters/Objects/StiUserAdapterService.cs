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
    /// Describes the adapter of data that provides access to data which are specified by the user.
    /// </summary>
    public class StiUserAdapterService : StiDataStoreAdapterService
	{
		#region StiService override
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => StiLocalization.Get("Adapters", "AdapterUserSources");
	    #endregion

        #region Properties
        public override bool IsObjectAdapter => true;
	    #endregion

		#region StiDataAdapterService override
		/// <summary>
		/// Fills a name and alias of the Data Source relying on data.
		/// </summary>
		/// <param name="data">Data relying on which names will be filled.</param>
		/// <param name="dataSource">Data Source in which names will be filled.</param>
		public override void SetDataSourceNames(StiData data, StiDataSource dataSource)
		{
			base.SetDataSourceNames(data, dataSource);

			var userData = data.ViewData as StiUserData;
			
			if (userData != null)
			{
				dataSource.Name = userData.Name;
				dataSource.Alias = userData.Alias;
			}
		}

		/// <summary>
		/// Returns name of category for data.
		/// </summary>
		public override string GetDataCategoryName(StiData data)
		{
			return ServiceName;
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

			var userData = data.ViewData as StiUserData;

		    if (userData != null)
		        dataColumns.AddRange(userData.Columns);

		    foreach (StiDataColumn column in dataColumns)
			{
				column.Name = StiNameValidator.CorrectName(column.Name, dataSource?.Dictionary?.Report);
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
		/// Returns the type of the Data Source.
		/// </summary>
		/// <returns>The type of Data Source.</returns>
		public override Type GetDataSourceType()
		{
			return typeof(StiUserSource);
		}

		/// <summary>
		/// Returns the array of data types to which the Data Source may refer.
		/// </summary>
		/// <returns>Array of data types.</returns>
		public override Type[] GetDataTypes()
		{
			return new[] { typeof(StiUserData) };
		}

		public override void ConnectDataSourceToData(StiDictionary dictionary, StiDataSource dataSource, bool loadData)
		{
            StiDataLeader.Disconnect(dataSource);

            StiUserData selectedUserSource = null;
			var dataStoreSource = dataSource as StiDataStoreSource;
			var nameInSource = dataStoreSource.NameInSource.ToLower(CultureInfo.InvariantCulture);

			#region Search data in datastore by NameInSource with full name equaling
			foreach (StiData data in dataSource.Dictionary.DataStore)
			{
				if (data.Name.ToLower(CultureInfo.InvariantCulture) == nameInSource && data.Data is StiUserData)
				{
					selectedUserSource = data.Data as StiUserData;					
					break;
				}
			}
			#endregion

			#region Search data in datastore by NameInSource with partial name equaling
			if (selectedUserSource == null)
			{
				string nameInSource2 = null;
                if (nameInSource.StartsWith("view", StringComparison.InvariantCulture))
                    nameInSource2 = nameInSource.Substring(4);

				foreach (StiData data in dataSource.Dictionary.DataStore)
				{
					if (!(data.Data is StiUserData))continue;

					var dataName = data.Name.ToLower(CultureInfo.InvariantCulture);
					string dataName2 = null;

                    if (dataName.StartsWith("view", StringComparison.InvariantCulture)) dataName2 = dataName.Substring(4);

					if (dataName == nameInSource || dataName == nameInSource2 ||
						dataName2 == nameInSource || dataName2 == nameInSource2)
					{
						selectedUserSource = data.Data as StiUserData;					
						break;
					}
				}
			}
			#endregion

			if (selectedUserSource != null)
				dataStoreSource.DataTable = GetDataTableFromData(dataSource, selectedUserSource);
		}

        private bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

		private DataTable GetDataTableFromData(StiDataSource dataSource, StiUserData userData)
		{
			userData.InvokeConnect(EventArgs.Empty);

			var newTable = new DataTable(dataSource.Name);

			foreach (StiDataColumn column in userData.Columns)
			{
                if (IsNullableType(column.Type))
                    newTable.Columns.Add(column.Name, column.Type.GetGenericArguments()[0]);
                else
				    newTable.Columns.Add(column.Name, column.Type);
			}	
		
			if (userData.Columns.Count == 0)
			{
				foreach (StiDataColumn column in dataSource.Columns)
				{
					newTable.Columns.Add(column.Name, column.Type);
				}
			}

			for (int index = 0; index < userData.Count; index ++)
			{
				var row = newTable.NewRow();
				
				foreach (DataColumn column in newTable.Columns)
				{
					try
					{
						var e = new StiUserGetDataEventArgs(dataSource as StiUserSource, index, column.ColumnName);
						userData.InvokeGetData(e);
						row[column.ColumnName] = e.Data;
					}
					catch (Exception e)
					{
						StiLogService.Write(this.GetType(), e);
						if (!StiOptions.Engine.HideExceptions)throw;
					}
				}

				newTable.Rows.Add(row);				
			}

			userData.InvokeDisconnent(EventArgs.Empty);
			
			return newTable;
		}
		#endregion
	}
}
