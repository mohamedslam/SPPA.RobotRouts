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
using Stimulsoft.Report.Chart.Design;
using System;
using System.Collections;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    /// <summary>
    /// Describes the chart filter collection.
    /// </summary>
    [TypeConverter(typeof(StiChartFiltersCollectionConverter))]
    public class StiChartFiltersCollection :
        CollectionBase,
        ICloneable,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiChartFilter filter in List)
            {
                jObject.AddPropertyJObject(index.ToString(), filter.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var filter = new StiChartFilter();
                Add(filter);

                filter.LoadFromJsonObject((JObject)property.Value);
            }
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            var filters = new StiChartFiltersCollection();
            foreach (StiChartFilter filter in this)
            {
                filters.Add(filter.Clone() as StiChartFilter);
            }
            return filters;
        }
        #endregion

        #region Collection
        protected override void OnInsert(int index, object value)
        {
            if (((StiChartFilter)value).Filters == null)
                ((StiChartFilter)value).Filters = this;
        }

        public void Add(StiChartFilter filter)
        {
            List.Add(filter);
        }

        public void AddRange(StiChartFiltersCollection filters)
        {
            foreach (StiChartFilter filter in filters)
            {
                Add(filter);
            }
        }

        public void AddRange(StiChartFilter[] filters)
        {
            foreach (StiChartFilter filter in filters) Add(filter);
        }

        public bool Contains(StiChartFilter filter)
        {
            return List.Contains(filter);
        }

        public int IndexOf(StiChartFilter filter)
        {
            return List.IndexOf(filter);
        }

        public void Insert(int index, StiChartFilter filter)
        {
            List.Insert(index, filter);
        }

        public void Remove(StiChartFilter filter)
        {
            List.Remove(filter);
        }


        public StiChartFilter this[int index]
        {
            get
            {
                return (StiChartFilter)List[index];
            }
            set
            {
                List[index] = value;
            }
        }
        #endregion
    }
}
