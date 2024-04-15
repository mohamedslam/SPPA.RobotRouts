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
using System.Data;

namespace Stimulsoft.Report
{
	/// <summary>
	/// Describes collection of meta tags.
	/// </summary>
	public class StiMetaTagCollection : 
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
            foreach (StiMetaTag metaTag in List)
            {
                jObject.AddPropertyJObject(index.ToString(), metaTag.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var metaTag = new StiMetaTag(null, null);
                metaTag.LoadFromJsonObject((JObject)property.Value);

                List.Add(metaTag);
            }
        }
        #endregion

		#region Collection
        public void Add(string name, string tag)
        {
            List.Add(new StiMetaTag(name, tag));
        }

		public void Add(StiMetaTag metaTag)
		{
			List.Add(metaTag);
		}

		public void AddRange(StiMetaTag[] metaTags)
		{
			foreach (StiMetaTag metaTag in metaTags)
				Add(metaTag);
		}

        public void AddRange(StiMetaTagCollection metaTags)
		{
            foreach (StiMetaTag metaTag in metaTags)
                Add(metaTag);
		}


        public bool Contains(StiMetaTag metaTag)
		{
            return List.Contains(metaTag);
		}

        public int IndexOf(StiMetaTag metaTag)
		{
            return List.IndexOf(metaTag);
		}

        public void Insert(int index, StiMetaTag metaTag)
		{
            List.Insert(index, metaTag);
		}

        public void Remove(StiMetaTag metaTag)
		{
            List.Remove(metaTag);
		}

        public StiMetaTag this[int index]
		{
			get
			{
                return (StiMetaTag)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

        public StiMetaTag this[string name]
        {
            get
            {
                foreach (StiMetaTag tag in List)
                {
                    if (tag.Name == name)                        
                        return tag;
                }
                return null;
            }
            set
            {
                for (int index = 0; index < List.Count; index++)
                {
                    StiMetaTag tag = List[index] as StiMetaTag;

                    if (tag.Name == name)
                    {
                        List[index] = value;
                        return;
                    }
                }
                List.Add(value);
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
            StiMetaTagCollection metaTags = new StiMetaTagCollection();
            foreach (StiMetaTag metaTag in List) metaTags.Add((StiMetaTag)metaTag.Clone());
            return metaTags;
		}
		#endregion
	}
}
