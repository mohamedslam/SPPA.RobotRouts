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
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(Stimulsoft.Report.Chart.Design.StiChartConditionsCollectionConverter))]
    public class StiChartConditionsCollection :
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
            foreach (StiChartCondition condition in List)
            {
                jObject.AddPropertyJObject(index.ToString(), condition.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            List.Clear();

            foreach (var property in jObject.Properties())
            {
                var condition = new StiChartCondition();
                condition.LoadFromJsonObject((JObject)property.Value);

                List.Add(condition);
            }
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            var conditions = new StiChartConditionsCollection();

            foreach (StiChartCondition condition in this)
            {
                conditions.Add(condition.Clone() as StiChartCondition);
            }
            return conditions;
        }
        #endregion

        #region Collection
        protected override void OnInsert(int index, object value)
        {
            if (((StiChartCondition)value).Conditions == null)
                ((StiChartCondition)value).Conditions = this;
        }

        public void Add(StiChartCondition condition)
        {
            List.Add(condition);
        }

        public void AddRange(StiChartConditionsCollection conditions)
        {
            foreach (StiChartCondition condition in conditions)
            {
                Add(condition);
            }
        }

        public void AddRange(StiChartCondition[] conditions)
        {
            foreach (StiChartCondition condition in conditions) Add(condition);
        }

        public bool Contains(StiChartCondition condition)
        {
            return List.Contains(condition);
        }

        public int IndexOf(StiChartCondition condition)
        {
            return List.IndexOf(condition);
        }

        public void Insert(int index, StiChartCondition condition)
        {
            List.Insert(index, condition);
        }

        public void Remove(StiChartCondition condition)
        {
            List.Remove(condition);
        }

        public StiChartCondition this[int index]
        {
            get
            {
                return (StiChartCondition)List[index];
            }
            set
            {
                List[index] = value;
            }
        }
        #endregion
    }
}