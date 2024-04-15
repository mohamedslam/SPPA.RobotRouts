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

using System.Collections.Generic;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using System;
using System.Linq;
using System.Collections;

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Describes the filter collection.
	/// </summary>
	public class StiFiltersCollection : 
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
            foreach(StiFilter filter in List)
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
                StiFilter filter = null;

                var propJObject = (JObject)property.Value;
                var ident = propJObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

                switch (ident)
                {
                    case "StiFilter":
                        filter = new StiFilter();
                        break;

                    case "StiBaseCondition":
                        filter = new StiBaseCondition();
                        break;

                    case "StiConditionHelper":
                        filter = new StiConditionHelper();
                        break;
                }

                filter.LoadFromJsonObject((JObject)property.Value);
                List.Add(filter);
            }
        }
        #endregion

        #region ICloneable.override
        public object Clone()
		{
			var filters = new StiFiltersCollection();
			foreach (StiFilter filter in this)
			{
				filters.Add(filter.Clone() as StiFilter);
			}
			return filters;
		}
        #endregion

        #region Collection
	    public List<StiFilter> ToList()
	    {
	        return List.Cast<StiFilter>().ToList();
	    }

        public void Add(StiFilter filter)
		{
			List.Add(filter);
		}

		public void AddRange(StiFiltersCollection filters)
		{
			foreach (StiFilter filter in filters)
			{
				Add(filter);
			}
		}

		public void AddRange(StiFilter[] filters)
		{
			foreach (StiFilter filter in filters)Add(filter);
		}

		public bool Contains(StiFilter filter)
		{
			return List.Contains(filter);
		}
		
		public int IndexOf(StiFilter filter)
		{
			return List.IndexOf(filter);
		}

		public void Insert(int index, StiFilter filter)
		{
			List.Insert(index, filter);
		}

		public void Remove(StiFilter filter)
		{
			List.Remove(filter);
		}
		
		
		public StiFilter this[int index]
		{
			get
			{
				return (StiFilter)List[index];
			}
			set
			{
				List[index] = value;
			}
		}	
		#endregion
	}
}
