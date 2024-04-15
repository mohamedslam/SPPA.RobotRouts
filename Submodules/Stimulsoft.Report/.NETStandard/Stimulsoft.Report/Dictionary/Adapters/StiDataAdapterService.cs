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
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the base class that is used for relations with Data Sources of a report in the DataStore.
    /// </summary>
    [StiServiceBitmap(typeof(StiDataAdapterService), "Stimulsoft.Report.Bmp.DataAdapter.bmp")]
	[StiServiceCategoryBitmap(typeof(StiDataAdapterService), "Stimulsoft.Report.Bmp.DataAdapter.bmp")]
	public abstract class StiDataAdapterService : StiService
	{
		#region StiService override
		/// <summary>
		/// Gets a service category.
		/// </summary>
		public sealed override string ServiceCategory => StiLocalization.Get("Services", "categoryDictionary");

	    /// <summary>
		/// Gets a service type.
		/// </summary>
		public sealed override Type ServiceType => typeof(StiDataAdapterService);
	    #endregion

        #region Methods.GetQueryBuilderProviders
        public virtual object GetSyntaxProvider()
        {
            return null;
        }
        
        public virtual object GetMetadataProvider(IDbConnection connection)
        {
            return null;
        }
        #endregion

        #region Properties
	    public virtual bool IsObjectAdapter => false;
	    #endregion

        #region Methods
        /// <summary>
		/// Returns name of category for data.
		/// </summary>
		public abstract string GetDataCategoryName(StiData data);

		/// <summary>
		/// Returns adapter for Data Source.
		/// </summary>
		/// <param name="dataSource">Data Source for which retrieval of adapter will be done.</param>
		/// <returns>Adapter of data.</returns>
		public static StiDataAdapterService GetDataAdapter(StiDataSource dataSource)
		{
		    var adapters = StiOptions.Services.DataAdapters.Where(s => s.ServiceEnabled);
            var adapter = dataSource is StiBusinessObjectSource ? adapters.FirstOrDefault(a => a is StiBusinessObjectAdapterService) : null;
            if (adapter != null)
                return adapter;

            return adapters.FirstOrDefault(a => a.GetDataSourceType() == dataSource.GetType());
		}
	
		/// <summary>
		/// Returns adapter for Data in DataStore.
		/// </summary>
		/// <param name="data">Data for which retrieval of adapter will be done.</param>
		/// <returns>Adapter of data.</returns>
		public static StiDataAdapterService GetDataAdapter(StiData data)
		{
            if (data == null || data.Data == null) return null;

            var dataType = data.Data.GetType();

		    if (dataType.BaseType != null && dataType.BaseType.FullName.StartsWith("System.Data.TypedTableBase"))
		        dataType = typeof(DataTable);

		    foreach (var adapter in StiOptions.Services.DataAdapters.Where(s => s.ServiceEnabled))
			{
				if (adapter is StiBusinessObjectAdapterService)
				{
					if (data.Data is DataSet || 
						data.Data is DataTable || 
						data.Data is DataView ||
						data.Data is IDbConnection ||
						data.Data is StiUserData) continue;
                    return adapter;
				}

			    try
			    {
			        if (adapter.IsAdapterDataType(dataType)) return adapter;
			    }
			    catch
			    {
			    }
			}
			return null;
		}

		
		/// <summary>
		/// Returns name of category for DataAdapter.
		/// </summary>
		/// <param name="dataAdapter">Adapter of data.</param>
		/// <param name="dataStore">Collection of data.</param>
		/// <returns>Name of category.</returns>
		public static TreeNode GetDataAdapterCategories(StiDataAdapterService dataAdapter, 
			StiDataCollection dataStore)
		{
            var list = new SortedList<string, List<StiData>>();
			var adapters = new Hashtable();
			var parentNode = new TreeNode();

			foreach (StiData data in dataStore)
			{					
				if (dataAdapter is StiBusinessObjectAdapterService)
				{
					if (data.Data is DataSet || 
						data.Data is DataTable || 
						data.Data is DataView ||
						data.Data is IDbConnection ||
                        data.Data is StiUserData) continue;
				}

				var types = dataAdapter.GetDataTypes();
				if (types != null)
				{
					foreach (var type in types)
					{
						if ((StiTypeFinder.FindInterface(data.Data.GetType(), type)) ||
							StiTypeFinder.FindType(data.Data.GetType(), type))
						{
							var category = dataAdapter.GetDataCategoryName(data);
                            if (!list.ContainsKey(category))
                                list[category] = new List<StiData>();

                            var datas = list[category];
							if (!datas.Contains(data))
                                datas.Add(data);

							adapters[category] = dataAdapter;						
						}
					}
				}
			}

			foreach (string category in list.Keys)
			{
                var datas = list[category];
                        
				if (datas.Count > 0)
				{
				    var categoryNode = new TreeNode(category, 0, 0) { Tag = adapters[category] };
				    parentNode.Nodes.Add(categoryNode);

					foreach (StiData data in datas)
					{
					    var node = new TreeNode(data.Name, 1, 1) { Tag = data };
					    categoryNode.Nodes.Add(node);
						
					}
				}
			}

			return parentNode;
		}

		/// <summary>
		/// Creates a new Data Source and adds it to the dictionary.
		/// </summary>
		/// <param name="dictionary">Dictionary to add Data Source.</param>
		/// <returns>Created Data Source.</returns>
		public abstract StiDataSource Create(StiDictionary dictionary, bool addToDictionary);

		public StiDataSource Create(StiDictionary dictionary)
		{
			return Create(dictionary, true);
		}

		/// <summary>
		/// Calls the form for a new data source edition.
		/// </summary>
		/// <param name="dictionary">Dictionary in which Data Source is located.</param>
        /// <param name="dataSource">Data Source.</param>
		/// <returns>Result of gialog form.</returns>
		public abstract bool New(StiDictionary dictionary, StiDataSource dataSource);

		/// <summary>
		/// Calls the form for data source edition.
		/// </summary>
		/// <param name="dictionary">Dictionary in which data source is located.</param>
        /// <param name="dataSource">Data Source.</param>
		/// <returns>Result of gialog form.</returns>
		public abstract bool Edit(StiDictionary dictionary, StiDataSource dataSource);

		/// <summary>
		/// Returns the type of the data source.
		/// </summary>
		/// <returns>The type of Data Source.</returns>
		public abstract Type GetDataSourceType();

		/// <summary>
		/// Returns the array of data types to which the Data Source may refer.
		/// </summary>
		/// <returns>Array of data types.</returns>
		public abstract Type[] GetDataTypes();

        /// <summary>
        /// Returns true if the specified type is supported by this data adapters.
        /// </summary>
        public virtual bool IsAdapterDataType(Type type)
        {
            if (type == null) return false;
            
            var types = GetDataTypes();
            if (types == null) return false;

            return types.Any(t => t == type);
        }

		/// <summary>
		/// Returns a collection of columns of data.
		/// </summary>
		/// <param name="data">Data to find column.</param>
		/// <returns>Collection of columns found.</returns>
		public abstract StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource);

        /// <summary>
        /// Returns a collection of columns of data.
        /// </summary>
        /// <param name="data">Data to find column.</param>
        /// <returns>Collection of columns found.</returns>
        public abstract StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource, CommandBehavior retrieveMode);

        /// <summary>
        /// Returns a collection of parameters of data.
        /// </summary>
        /// <param name="data">Data to find parameters.</param>
        /// <returns>Collection of parameters found.</returns>
        public abstract StiDataParametersCollection GetParametersFromData(StiData data, StiDataSource dataSource);

		/// <summary>
		/// Fills a name and alias of the data source relying on data.
		/// </summary>
		/// <param name="data">Data relying on which names will be filled.</param>
        /// <param name="dataSource">Data source in which names will be filled.</param>
		public abstract void SetDataSourceNames(StiData data, StiDataSource dataSource);

		public abstract void ConnectDataSourceToData(StiDictionary dictionary, StiDataSource dataSource, bool loadData);
		
		public override string ToString()
		{
			return ServiceName;
		}
		#endregion
	}
}
