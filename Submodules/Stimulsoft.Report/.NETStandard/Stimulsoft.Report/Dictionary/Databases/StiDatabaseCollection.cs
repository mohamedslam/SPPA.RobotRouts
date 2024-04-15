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
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Collection of Databases.
    /// </summary>
    public class StiDatabaseCollection : 
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
            foreach (StiDatabase database in this)
            {
                jObject.AddPropertyJObject(index.ToString(), database.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var propJObject = (JObject)property.Value;

                var ident = propJObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();
                var db = StiOptions.Services.Databases.FirstOrDefault(x => x.GetType().Name == ident);

                if (db != null)
                {
                    var database = db.CreateNew();
                    database.LoadFromJsonObject((JObject)property.Value);
                    List.Add(database);
                }
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
            return this.MemberwiseClone();
        }
        #endregion

        #region Collection
        public List<StiDatabase> ToList()
        {
            return this.Cast<StiDatabase>().ToList();
        }

		public void Add(StiDatabase data)
		{
			List.Add(data);
		}

		public void AddRange(StiDatabase[] datas)
		{
			base.InnerList.AddRange(datas);
		}

		public void AddRange(StiDatabaseCollection datas)
		{
			foreach (StiDatabase database in datas)
			{
				Add(database);
			}
		}

		public bool Contains(StiDatabase data)
		{
			return List.Contains(data);
		}

        public bool Contains(string name)
        {
            return ToList().FirstOrDefault(x => x.Name == name) != null;
        }

        public int IndexOf(StiDatabase data)
		{
			return List.IndexOf(data);
		}

		public int IndexOf(string name)
		{
			int index = 0;
			foreach (StiDatabase database in this)
			{
				if (name == database.Name)
				    return index;

				index++;
			}
			return -1;
		}

		public void Insert(int index, StiDatabase data)
		{
			List.Insert(index, data);
		}

		public void Remove(StiDatabase data)
		{
			List.Remove(data);
		}

		public void Remove(string name)
		{
            int index = this.IndexOf(name);
			if (index != -1)
				List.Remove(index);
			else
				throw new Exception($"Database \'{name}\' is not found");
		}
		
		public StiDatabase this[int index]
		{
			get
			{
				return (StiDatabase)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

		public StiDatabase this[string name]
		{
			get
			{
				name = name.ToLowerInvariant();
			    foreach (StiDatabase data in List)
			    {
			        if (data.Name.ToLowerInvariant() == name)
			            return data;
			    }

			    return null;
			}
			set
			{
				name = name.ToLowerInvariant();
				for (int index = 0; index < List.Count; index++)				
				{
					var dataBase = List[index] as StiDatabase;
					if (dataBase.Name.ToLowerInvariant() == name)
					{
						List[index] = value;
						return;
					}
				}
				Add(value);
			}
		}

		public StiDatabase[] Items => (StiDatabase[])InnerList.ToArray(typeof(StiDatabase));
        #endregion
	}
}
