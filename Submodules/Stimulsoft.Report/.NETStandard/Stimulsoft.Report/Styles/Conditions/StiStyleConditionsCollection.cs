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
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report
{

	/// <summary>
	/// Describes the style condition collection.
	/// </summary>
    public class StiStyleConditionsCollection : 
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
            foreach(StiStyleCondition cond in List)
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
                var cond = new StiStyleCondition();
                cond.LoadFromJsonObject((JObject)property.Value);

                List.Add(cond);
            }
        }
        #endregion

        #region Methods.Equals
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var conditions2 = obj as StiStyleConditionsCollection;
            if (conditions2 == null || this.Count != conditions2.Count) return false;

            var index = 0;
            foreach (StiStyleCondition condition in this)
            {
                var condition2 = conditions2[index++];
                if (condition.Equals(condition2)) continue;
                return false;
            }
            return true;
        }
        #endregion

        #region ICloneable
        public object Clone()
		{
            var conditions = new StiStyleConditionsCollection();
            foreach (StiStyleCondition condition in this)
			{
                conditions.Add(condition.Clone() as StiStyleCondition);
			}
			return conditions;
        }
        #endregion

        #region Collection
        public void Add(StiStyleCondition condition)
		{
			List.Add(condition);
		}

        /// <summary>
        /// Creates a new object of the type StiStyleCondition.
        /// </summary>
        public void Add(StiStyleConditionElement[] elements)
        {
            var condition = new StiStyleCondition();
            condition.FromElements(elements);
            this.Add(condition);
        }

        public void AddRange(StiStyleConditionsCollection conditions)
		{
            foreach (StiStyleCondition condition in conditions)
			{
				Add(condition);
			}
		}

        public void AddRange(StiStyleCondition[] conditions)
		{
            foreach (StiStyleCondition condition in conditions) Add(condition);
		}

        public bool Contains(StiStyleCondition condition)
		{
			return List.Contains(condition);
		}

        public int IndexOf(StiStyleCondition condition)
		{
			return List.IndexOf(condition);
		}

        public void Insert(int index, StiStyleCondition condition)
		{
			List.Insert(index, condition);
		}

        public void Remove(StiStyleCondition condition)
		{
			List.Remove(condition);
		}

        public StiStyleCondition this[int index]
		{
			get
			{
                return (StiStyleCondition)List[index];
			}
			set
			{
				List[index] = value;
			}
		}	
		#endregion
    }
}
