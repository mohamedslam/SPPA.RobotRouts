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

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes collection of report resource.
    /// </summary>
    public class StiReportResourceCollection : 
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
            foreach (StiReportResource resource in List)
            {
                jObject.AddPropertyJObject(index.ToString(), resource.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var resource = new StiReportResource(null, null, null);
                resource.LoadFromJsonObject((JObject)property.Value);

                List.Add(resource);
            }
        }
        #endregion

		#region Collection
        public void Add(StiReportResource resource)
		{
            List.Add(resource);
		}

        public void AddRange(StiReportResource[] resources)
		{
            foreach (StiReportResource resource in resources)
                Add(resource);
		}

        public void AddRange(StiReportResourceCollection resources)
		{
            foreach (StiReportResource resource in resources)
                Add(resource);
		}

        public bool Contains(StiReportResource resource)
		{
            return List.Contains(resource);
		}

        public int IndexOf(StiReportResource resource)
		{
            return List.IndexOf(resource);
		}

        public void Insert(int index, StiReportResource resource)
		{
            List.Insert(index, resource);
		}

        public void Remove(StiReportResource resource)
		{
            List.Remove(resource);
		}

        public StiReportResource this[int index]
		{
			get
			{
                return (StiReportResource)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
		#endregion

		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
            var resources = new StiReportResourceCollection();

            foreach (StiReportResource resource in List) 
                resources.Add((StiReportResource)resource.Clone());

            return resources;
		}
		#endregion
	}
}