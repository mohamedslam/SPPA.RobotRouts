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
using System.Linq;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the condition collection.
    /// </summary>
    public class StiConditionsCollection : 
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
            foreach(StiBaseCondition cond in List)
            {
                jObject.AddPropertyJObject(index.ToString(), cond.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                StiBaseCondition condition = null;
                var propJObject = (JObject)property.Value;
                var ident = propJObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

                switch (ident)
                {
                    case "StiBaseCondition":
                        condition = new StiBaseCondition();
                        break;

                    case "StiCondition":
                        condition = new StiCondition();
                        break;

                    case "StiIconSetCondition":
                        condition = new StiIconSetCondition();
                        break;

                    case "StiColorScaleCondition":
                        condition = new StiColorScaleCondition();
                        break;

                    case "StiDataBarCondition":
                        condition = new StiDataBarCondition();
                        break;

                    case "StiMultiCondition":
                        condition = new StiMultiCondition();
                        break;
                }

                condition.LoadFromJsonObject(propJObject);
                List.Add(condition);
            }
        }
        #endregion

        #region ICloneable.override
        public object Clone()
		{
			var conditions = new StiConditionsCollection();
            foreach (StiBaseCondition condition in this)
			{
                conditions.Add(condition.Clone() as StiBaseCondition);
			}
			return conditions;
		}
        #endregion

        #region Collection
        public void Add(StiBaseCondition condition)
		{
			List.Add(condition);
		}

		public void AddRange(StiConditionsCollection conditions)
		{
            foreach (StiBaseCondition condition in conditions)
			{
				Add(condition);
			}
		}

		public void AddRange(StiConditionsCollection conditions, bool addOnlyNotEqual)
		{
            foreach (StiBaseCondition condition in conditions)
			{
				if (this.Count == 0)Add(condition);
				else
				{
                    foreach (StiBaseCondition cond in this)
					{
						if (!cond.Equals(condition))
						{
							Add(condition);
							break;
						}
					}
				}
			}
		}

        public void AddRange(StiBaseCondition[] conditions)
		{
            foreach (StiBaseCondition condition in conditions) Add(condition);
		}

        public bool Contains(StiBaseCondition condition)
		{
			return List.Contains(condition);
		}

        public int IndexOf(StiBaseCondition condition)
		{
			return List.IndexOf(condition);
		}

        public void Insert(int index, StiBaseCondition condition)
		{
			List.Insert(index, condition);
		}

        public void Remove(StiBaseCondition condition)
		{
			List.Remove(condition);
		}
        
        public StiBaseCondition this[int index]
		{
			get
			{
                return (StiBaseCondition)List[index];
			}
			set
			{
				List[index] = value;
			}
		}	
		#endregion
	}
}
