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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the collection of the resources which embedded to the report file.
    /// </summary>
    public class StiResourcesCollection : 
		CollectionBase, 
        ICloneable,
		IComparer
    {
        #region ICloneable
        public object Clone()
        {
            var collection = new StiResourcesCollection();

            ToList().Select(c => c.Clone() as StiResource).ToList()
                .ForEach(collection.Add);

            return collection;
        }

        public List<StiResource> CloneAsList()
        {
            return (Clone() as StiResourcesCollection).ToList();
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObjectEx(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiResource resource in List)
            {
                if (mode == StiJsonSaveMode.Report || mode == StiJsonSaveMode.Document && 
                       (resource.AvailableInTheViewer ||
                        resource.Type == StiResourceType.FontEot ||
                        resource.Type == StiResourceType.FontOtf ||
                        resource.Type == StiResourceType.FontTtc ||
                        resource.Type == StiResourceType.FontTtf ||
                        resource.Type == StiResourceType.FontWoff ||
                        resource.Type == StiResourceType.Map))
                {
                    jObject.AddPropertyJObject(index.ToString(), resource.SaveToJsonObject(StiJsonSaveMode.Report));
                    index++;
                }
            }

            return jObject;
        }

        public void LoadFromJsonObjectEx(JObject jObject, StiReport report)
        {
            foreach (var property in jObject.Properties())
            {
                var resource = new StiResource();
                resource.LoadFromJsonObject((JObject)property.Value);

                List.Add(resource);
            }
        }
        #endregion

        #region IComparer
        private int directionFactor = 1;

        int IComparer.Compare(object x, object y)
        {
            var var1 = x as StiResource;
            var var2 = y as StiResource;

            if (StiOptions.Designer.SortDictionaryByAliases)
                return String.Compare(var1.Alias, var2.Alias, StringComparison.Ordinal) * directionFactor;
            else
                return String.Compare(var1.Name, var2.Name, StringComparison.Ordinal) * directionFactor;
        }
        #endregion

		#region Sort
		public void Sort()
		{
			Sort(StiSortOrder.Asc);
		}

		public void Sort(StiSortOrder order)
		{
			if (order == StiSortOrder.Asc)directionFactor = 1;
			else directionFactor = -1;

			base.InnerList.Sort(this);
		}
        #endregion

        #region Collection
        public List<StiResource> ToList()
        {
            return this.Cast<StiResource>().ToList();
        }

		public void Add(string name, StiResourceType type, byte[] content)
		{
            this.List.Add(new StiResource(name, type, content));
		}

        public void Add(StiResource resource)
        {
            this.List.Add(resource);
        }

        public void AddRange(StiResource[] resources)
		{
            foreach (var resource in resources)
            {
                this.List.Add(resource);
            }
		}

        public void AddRange(StiResourcesCollection resources)
		{
            foreach (StiResource resource in resources)
			{
                Add(resource);
			}
		}

        public bool Contains(StiResource resource)
		{
            return List.Contains(resource);
		}

		public bool Contains(string name)
		{
			name = name.ToLower(CultureInfo.InvariantCulture);
            foreach (StiResource resource in this)
			{
                if (resource.Name.ToLower(CultureInfo.InvariantCulture) == name) return true;
			}
			return false;
		}

        public int IndexOf(StiResource resource)
		{
            return List.IndexOf(resource);
		}

		public int IndexOf(string name)
		{
			name = name.ToLower(CultureInfo.InvariantCulture);
			int index = 0;
            foreach (StiResource resource in this)
			{
                if (resource.Name.ToLower(CultureInfo.InvariantCulture) == name)
                    return index;

				index++;
			}
			return -1;
		}

        public void Insert(int index, StiResource resource)
		{
            List.Insert(index, resource);
		}

        public void Remove(StiResource resource)
		{
            List.Remove(resource);
		}
		
		public void Remove(string name)
		{
			name = name.ToLower(CultureInfo.InvariantCulture);
			int index = 0;
			while (index < List.Count)
			{
                var resource = List[index] as StiResource;

			    if (resource.Name.ToLower(CultureInfo.InvariantCulture) == name)
			        List.RemoveAt(index);

			    else
			        index++;
			}			
		}

        public StiResource this[int index]
		{
			get
			{
                return (StiResource)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

        public StiResource this[string name]
		{
			get
			{
                name = name.ToLowerInvariant();
			    foreach (StiResource resource in List)
			    {
			        if (resource.Name.ToLowerInvariant() == name)
			            return resource;
			    }

			    return null;
			}
			set
			{
                name = name.ToLowerInvariant();
				for (var index = 0; index < List.Count; index++)				
				{
                    var resource = List[index] as StiResource;

                    if (resource.Name.ToLowerInvariant() == name)
					{
						List[index] = value;
						return;
					}
				}
				Add(value);
			}
		}
		#endregion
    }
}