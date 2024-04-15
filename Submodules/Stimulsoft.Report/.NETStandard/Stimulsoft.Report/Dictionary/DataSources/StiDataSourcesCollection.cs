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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using System.Globalization;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
	/// Collection of Data Sources.
	/// </summary>
	public class StiDataSourcesCollection : 
		CollectionBase, 
		ICloneable,
        IComparer, 
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiDataSource dataSource in List)
            {
                jObject.AddPropertyJObject(index.ToString(), dataSource.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            var dataSources = StiOptions.Services.DataSource;

            foreach (var property in jObject.Properties())
            {
                var propJObject = (JObject)property.Value;
                var ident = propJObject.Properties().FirstOrDefault(x => x.Name == "Ident");

                var identName = ident.Value.ToObject<string>();
                var dataSource = dataSources.FirstOrDefault(x => x.GetType().Name == identName)?.CreateNew();
                if (dataSource == null)
                    dataSource = new StiUndefinedDataSource();

                dataSource.Dictionary = dictionary;
                dataSource.LoadFromJsonObject(propJObject);
                List.Add(dataSource);
            }
        }
        #endregion

        #region IComparer
        private int directionFactor = 1;

		int IComparer.Compare(object x, object y)
		{
			var source1 = x as StiDataSource;
			var source2 = y as StiDataSource;

			if (StiOptions.Designer.SortDictionaryByAliases)
				return string.Compare(source1.Alias, source2.Alias, StringComparison.Ordinal) * directionFactor;
			else 
				return string.Compare(source1.Name, source2.Name, StringComparison.Ordinal) * directionFactor;
        }
        #endregion
        
		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
			var dataSources = new StiDataSourcesCollection(this.dictionary);

		    foreach (StiDataSource data in List)
				dataSources.Add((StiDataSource)data.Clone());

			return dataSources;
		}
		#endregion

		#region Methods.Sort
		public void Sort()
		{
			Sort(StiSortOrder.Asc);
		}

		public void Sort(StiSortOrder order)
		{
			Sort(order, true);
		}

		public void Sort(StiSortOrder order, bool sortColumns)
		{
			if (order == StiSortOrder.Asc)directionFactor = 1;
			else directionFactor = -1;

			base.InnerList.Sort(this);

		    if (!sortColumns) return;

		    foreach (StiDataSource source in this)
		    {
		        source.Columns.Sort(order);
		    }
		}
        #endregion

        #region Methods.Collection
        public List<StiDataTransformation> FetchAllDataTransformations()
        {
            return ToList()
                .Where(c => c is StiDataTransformation)
                .Cast<StiDataTransformation>()
                .ToList();
        }

        public List<StiVirtualSource> FetchAllVirtualDataSources()
        {
            return ToList()
                .Where(c => c is StiVirtualSource)
                .Cast<StiVirtualSource>()
                .ToList();
        }

        public List<StiCrossTabDataSource> FetchAllCrossTabDataSources()
        {
            return ToList()
                .Where(c => c is StiCrossTabDataSource)
                .Cast<StiCrossTabDataSource>()
                .ToList();
        }

        public List<StiDataSource> ToList()
        {
            return this.Cast<StiDataSource>().ToList();
        }

        public void Add(StiDataSource dataSource)
        {
            if (dictionary != null) dataSource.Dictionary = dictionary;
            List.Add(dataSource);
        }

        public void AddRange(StiDataSource[] dataSources)
        {
            base.InnerList.AddRange(dataSources);
        }

        public void AddRange(StiDataSourcesCollection dataSources)
        {
            foreach (StiDataSource dataSource in dataSources)
                Add(dataSource);
        }

        public bool Contains(StiDataSource dataSource)
        {
            return List.Contains(dataSource);
        }

        public bool Contains(string name)
        {
            return this[name] != null;
        }

        public int IndexOf(StiDataSource dataSource)
        {
            return List.IndexOf(dataSource);
        }

        public void Insert(int index, StiDataSource dataSource)
        {
            List.Insert(index, dataSource);
        }

        protected override void OnClear()
        {
            base.OnClear();
            if (dictionary != null)
                dictionary.Relations.Clear();

            if (cachedDataSources != null)
            {
                cachedDataSources.Clear();
                cachedDataSources = null;
            }
        }

        public void Remove(StiDataSource dataSource)
        {
            List.Remove(dataSource);

            if (dictionary != null)
            {
                int pos = 0;
                while (pos < dictionary.Relations.Count)
                {
                    if (dictionary.Relations[pos].ParentSource == dataSource ||
                        dictionary.Relations[pos].ChildSource == dataSource)
                    {
                        dictionary.Relations.Remove(dictionary.Relations[pos]);
                    }
                    else pos++;
                }
            }

            var dataSourceName = dataSource.Name.ToLowerInvariant();

            if (CachedDataSources[dataSourceName] != null)
                CachedDataSources.Remove(dataSourceName);
        }

        public StiDataSource this[int index]
        {
            get
            {
                return (StiDataSource)List[index];
            }
            set
            {
                if (dictionary != null) value.Dictionary = dictionary;
                List[index] = value;
            }
        }

        private Hashtable cachedDataSources;
        internal Hashtable CachedDataSources
        {
            get
            {
                return cachedDataSources ?? (cachedDataSources = new Hashtable());
            }
        }

        public StiDataSource this[string name]
        {
            get
            {
                name = name.ToLowerInvariant();

                var searchedDataSource = CachedDataSources[name] as StiDataSource;
                if (searchedDataSource != null)
                    return searchedDataSource;

                foreach (StiDataSource dataSource in List)
                {
                    if (dataSource.Name.ToLowerInvariant() == name)
                    {
                        CachedDataSources[name] = dataSource;
                        return dataSource;
                    }
                    else
                    {
                        if (StiNameValidator.CorrectName(dataSource.Name.ToLowerInvariant(), dictionary?.Report) == StiNameValidator.CorrectName(name, dictionary?.Report))
                        {
                            CachedDataSources[name] = dataSource;
                            return dataSource;
                        }
                    }
                }
                return null;
            }
            set
            {
                name = name.ToLowerInvariant();
                for (int index = 0; index < List.Count; index++)
                {
                    var dataSource = List[index] as StiDataSource;
                    if (dataSource.Name.ToLowerInvariant() == name)
                    {
                        if (dictionary != null) value.Dictionary = dictionary;
                        List[index] = value;
                        return;
                    }
                }
                Add(value);
            }
        }
        #endregion

        #region Methods
        public void ClearParametersExpression()
		{
			foreach (StiDataSource dataSource in this)
			{
				var source = dataSource as StiSqlSource;
			    if (source == null) continue;

			    foreach (StiDataParameter parameter in source.Parameters)
			    {
			        parameter.Expression = string.Empty;
			    }
			}
		}

		public void Connect(bool loadData)
		{
			Connect(null, loadData);
		}

		/// <summary>
		/// Connect Data Source to data.
		/// </summary>
		/// <param name="loadData">Load data or no.</param>
		public void Connect(StiDataCollection datas, bool loadData)
		{
			var dataSources = new StiDataSource[this.Count];

			this.Items.CopyTo(dataSources, 0);
			Array.Sort(dataSources, new StiConnectionOrderComparer());

			foreach (StiDataSource dataSource in dataSources)
			{
				if (dataSource.ConnectionOrder == (int)StiConnectionOrder.None)continue;

                if (dataSource.ConnectOnStart && (loadData || dataSource is StiDataTableSource))
                {
                    StiDataLeader.Connect(dataSource, datas, loadData);
                }
            }
		}
		
		/// <summary>
		/// Disconnect the Data Sources from data.
		/// </summary>
		public void Disconnect()
		{
            foreach (StiDataSource dataSource in InnerList)
            {
                StiDataLeader.Disconnect(dataSource);
            }
        }
        #endregion

        #region Fields
        private StiDictionary dictionary;
        #endregion

        #region Properties
        public StiDataSource[] Items
        {
            get
            {
                return (StiDataSource[])InnerList.ToArray(typeof(StiDataSource));
            }
        }
        #endregion

        /// <summary>
        /// Creates the collection of Data Sources.
        /// </summary>
        /// <param name="dictionary">The dictionary in which the collection is registered.</param>
        public StiDataSourcesCollection(StiDictionary dictionary)
		{
			this.dictionary = dictionary;
		}
    }
}
