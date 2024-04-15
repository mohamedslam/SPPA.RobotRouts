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
using Stimulsoft.Report.Dictionary.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Stimulsoft.Base.Meters;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Exceptions;
using Stimulsoft.Data.Helpers;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the object which helps in data transformation.
    /// </summary>
    [TypeConverter(typeof(StiDataTransformationConverter))]
	public class StiDataTransformation : 
        StiDataStoreSource,
        IStiQueryObject
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            jObject.AddPropertyJObject("Sorts", StiJsonReportObjectHelper.Serialize.JObjectCollection(Sorts, mode));
            jObject.AddPropertyJObject("Filters", StiJsonReportObjectHelper.Serialize.JObjectCollection(Filters, mode));
            jObject.AddPropertyJObject("Actions", StiJsonReportObjectHelper.Serialize.JObjectCollection(Actions, mode));
            
            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Sorts":
                        Sorts.AddRange(((JObject)property.Value).Properties().Select(p => StiDataSortRule.LoadFromJson((JObject)p.Value)));
                        break;

                    case "Filters":
                        Filters.AddRange(((JObject)property.Value).Properties().Select(p => StiDataFilterRule.LoadFromJson((JObject)p.Value)));
                        break;

                    case "Actions":
                        Actions.AddRange(((JObject)property.Value).Properties().Select(p => StiDataActionRule.LoadFromJson((JObject)p.Value)));
                        break;
                }
            }
        }
        #endregion

	    #region IStiRetrieval
        public List<string> RetrieveUsedDataNames(string group)
        {
            return StiUsedDataHelper.GetMany(GetMeters());
        }
        #endregion

        #region IStiQueryObject
        public IEnumerable<IStiAppDataSource> GetDataSources(IEnumerable<string> dataNames)
        {
            var dict = GetDictionary();
            if (dict == null) return null;

            var dataSources = dict.FetchDataSources()
                .Where(d => !(d is StiDataTransformation) && (d.GetKey() != GetKey() || GetKey() == null));

            return StiDataSourcePicker.Fetch(this, null, dataNames, dataSources);
        }

        public string GetKey()
        {
            return Key;
        }
        #endregion

        #region IStiAppDataSource
        /// <summary>
        /// Returns a DataTable with data from this datasource.
        /// </summary>
        /// <returns>The DataTable with data.</returns>
        public override DataTable GetDataTable(bool allowConnectToData)
        {
            return RetrieveDataTable(allowConnectToData ? StiDataRequestOption.AllowOpenConnections : StiDataRequestOption.None);
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var clonedTransformation = (StiDataTransformation)base.Clone();

            clonedTransformation.Sorts = new List<StiDataSortRule>();
            Sorts.ForEach(i => clonedTransformation.Sorts.Add((StiDataSortRule)i.Clone()));

            clonedTransformation.Filters = new List<StiDataFilterRule>();
            Filters.ForEach(i => clonedTransformation.Filters.Add((StiDataFilterRule)i.Clone()));

            clonedTransformation.Actions = new List<StiDataActionRule>();
            Actions.ForEach(i => clonedTransformation.Actions.Add((StiDataActionRule)i.Clone()));

            Columns.ToList().ForEach(c =>
            {
                clonedTransformation.Sorts.Where(i => i.Key == c.Key).ToList().ForEach(i => i.Key = clonedTransformation.Columns[c.Name].Key);
                clonedTransformation.Filters.Where(i => i.Key == c.Key).ToList().ForEach(i => i.Key = clonedTransformation.Columns[c.Name].Key);
                clonedTransformation.Actions.Where(i => i.Key == c.Key).ToList().ForEach(i => i.Key = clonedTransformation.Columns[c.Name].Key);
            });

            return clonedTransformation;
        }
        #endregion

        #region DataAdapter
        protected override Type GetDataAdapterType()
        {
            return typeof(StiDataTransformationAdapterService);
        }
        #endregion

        #region Properties
        [StiSerializable(StiSerializationVisibility.List)]
        [Browsable(false)]
        public List<StiDataSortRule> Sorts { get; set; } = new List<StiDataSortRule>();

        [StiSerializable(StiSerializationVisibility.List)]
        [Browsable(false)]
        public List<StiDataFilterRule> Filters { get; set; } = new List<StiDataFilterRule>();

        [StiSerializable(StiSerializationVisibility.List)]
        [Browsable(false)]
        public List<StiDataActionRule> Actions { get; set; } = new List<StiDataActionRule>();
        #endregion

        #region Methods
        public DataTable RetrieveDataTable(StiDataRequestOption option)
        {
            if (Columns.Count == 0)
                return new DataTable(Name);

            var dataTable = StiDataAnalyzer.Analyze(this, null, GetMeters(), option, Sorts, Filters, null, Actions);
            var types = Columns.ToList().Select(c => c.Type).ToArray();
            var table = StiDataTableConverter.ToNetTable(dataTable, types);
            table.TableName = Name;
            return table;
        }

        internal void ConnectToData()
        {
            DataTable = RetrieveDataTable(StiDataRequestOption.None);
        }

        public List<IStiMeter> GetMeters(string group = null)
	    {
	        return Columns
	            .ToList()
	            .Where(c => c is StiDataTransformationColumn)
                .Cast<StiDataTransformationColumn>()
	            .Select(GetMeter).ToList();
	    }

        internal IStiMeter GetMeter(StiDataTransformationColumn column)
	    {
            switch (column.Mode)
            {
                case StiDataTransformationMode.Dimension:
                    return new StiDimensionTransformationMeter(column.Expression, column.Name, column.Key);

	            case StiDataTransformationMode.Measure:
	                return new StiMeasureTransformationMeter(column.Expression, column.Name, column.Key);

                default:
                    throw new StiTypeNotRecognizedException(column.Mode);
            }
	    }
        #endregion

        #region Methods.Override
        public override StiComponentId ComponentId => StiComponentId.StiDataTransformation;

	    public override StiDataSource CreateNew()
        {
            return new StiDataTransformation();
        }
        #endregion

        /// <summary>
		/// Creates a new object of the type StiDataTransformation.
		/// </summary>
		public StiDataTransformation()
            : this(null, null)
		{
		}

        /// <summary>
        /// Creates a new object of the type StiDataTransformation.
        /// </summary>
        public StiDataTransformation(string nameInSource, string name) 
            : this(nameInSource, name, StiKeyHelper.GenerateKey())
		{
			this.ConnectionOrder = (int)StiConnectionOrder.None;
		}

        /// <summary>
        /// Creates a new object of the type StiDataTransformation.
        /// </summary>
        public StiDataTransformation(string nameInSource, string name, string key) : base(nameInSource, name, name, key)
        {
            ConnectionOrder = (int)StiConnectionOrder.None;
            Key = StiKeyHelper.GetOrGeneratedKey(key);
        }
	}
}