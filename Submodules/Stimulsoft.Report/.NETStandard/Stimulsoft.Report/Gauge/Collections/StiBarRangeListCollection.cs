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
using Stimulsoft.Report.Components.Gauge;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using System.Linq;

namespace Stimulsoft.Report.Gauge.Collections
{
    public class StiBarRangeListCollection : 
        CollectionBase,
        ICloneable,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiIndicatorRangeInfo indicator in List)
            {
                jObject.AddPropertyJObject(index.ToString(), indicator.SaveToJsonObject(mode));
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

                var element = StiOptions.Services.IndicatorRanges.FirstOrDefault(x => x.GetType().Name == ident);
                if (element == null)
                    throw new Exception(string.Format("Type {0} is not found!", ident));

                var elementClone = element.CreateNew();
                
                Add(elementClone);
                elementClone.LoadFromJsonObject((JObject)property.Value);
            }
        }
        #endregion

        #region Fields
        private StiBarRangeListType barType;
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var clone = new StiBarRangeListCollection(this.barType);

            lock (((ICollection)this).SyncRoot)
                foreach (StiIndicatorRangeInfo element in this) clone.Add(element.Clone() as StiIndicatorRangeInfo);

            return clone;
        }
        #endregion

        #region Properties
        public bool IsReadOnly => false;

        public virtual StiIndicatorRangeInfo this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.InnerList.Count))
                {
                    throw new ArgumentOutOfRangeException("index", "InvalidArgument");
                }
                return this.InnerList[index] as StiIndicatorRangeInfo;
            }
            set
            {
                this.SetItemInternal(index, value);
            }
        }
        #endregion

        #region Methods
        public void Add(StiIndicatorRangeInfo element)
        {
            AddCore(element);
        }

        public void AddRange(StiIndicatorRangeInfo[] elements)
        {
            foreach (StiIndicatorRangeInfo element in elements)
            {
                AddCore(element);
            }
        }

        private void AddCore(StiIndicatorRangeInfo element)
        {
            if (element.RangeListType == this.barType)
                this.InnerList.Add(element);
        }

        public void Insert(int index, StiIndicatorRangeInfo element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("item");
            }
            if ((index < 0) || (index > this.InnerList.Count))
            {
                throw new ArgumentOutOfRangeException("index", "InvalidArgument");
            }

            if (element.RangeListType == this.barType)
                this.InnerList.Insert(index, element);
        }

        public bool Remove(StiIndicatorRangeInfo element)
        {
            int index = this.InnerList.IndexOf(element);
            if (index != -1)
            {
                this.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool Contains(StiIndicatorRangeInfo element)
        {
            return (this.IndexOf(element) != -1);
        }

        public void CopyTo(StiIndicatorRangeInfo[] elements, int arrayIndex)
        {
            this.InnerList.CopyTo(elements, arrayIndex);
        }

        public int IndexOf(StiIndicatorRangeInfo element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return this.InnerList.IndexOf(element);
        }

        internal void SetItemInternal(int index, StiIndicatorRangeInfo element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("value");
            }
            if ((index < 0) || (index >= this.InnerList.Count))
            {
                throw new ArgumentOutOfRangeException("index", "InvalidArgument");
            }

            if (element.RangeListType == this.barType)
                this.InnerList[index] = element;
        }

        public bool MoveUp(StiIndicatorRangeInfo element)
        {
            int index = this.InnerList.IndexOf(element);
            if (index > 0)
            {
                this.InnerList.RemoveAt(index);
                index--;
                this.InnerList.Insert(index, element);
                return true;
            }

            return false;
        }

        public bool MoveDown(StiIndicatorRangeInfo element)
        {
            int index = this.InnerList.IndexOf(element);
            if (index != -1 && this.InnerList.Count > 1 && index < this.InnerList.Count - 1)
            {
                this.InnerList.RemoveAt(index);
                index++;
                this.InnerList.Insert(index, element);
                return true;
            }

            return false;
        }
        #endregion

        public StiBarRangeListCollection(StiBarRangeListType barType)
        {
            this.barType = barType;
        }
    }
}