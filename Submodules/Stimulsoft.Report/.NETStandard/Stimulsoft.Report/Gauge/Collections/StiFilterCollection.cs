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
using Stimulsoft.Report.Components.Gauge;
using System;
using System.Collections;

namespace Stimulsoft.Report.Gauge.Collections
{
    public class StiFilterCollection :
        CollectionBase,
        ICloneable
    {
        #region IStiJsonReportObject
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiStateIndicatorFilter indicatorFilter in List)
            {
                jObject.AddPropertyJObject(index.ToString(), indicatorFilter.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var indicator = new StiStateIndicatorFilter();

                Add(indicator);
                indicator.LoadFromJsonObject((JObject)property.Value);
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
            var clone = new StiFilterCollection();

            lock (((ICollection)this).SyncRoot)
                foreach (StiStateIndicatorFilter element in this) clone.Add(element.Clone() as StiStateIndicatorFilter);

            return clone;
        }
        #endregion

        #region Properties
        public bool IsReadOnly => false;

        public virtual StiStateIndicatorFilter this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.InnerList.Count))
                {
                    throw new ArgumentOutOfRangeException("index", "InvalidArgument");
                }
                return this.InnerList[index] as StiStateIndicatorFilter;
            }
            set
            {
                this.SetItemInternal(index, value);
            }
        }
        #endregion

        #region Methods
        public void Add(StiStateIndicatorFilter element)
        {
            this.InnerList.Add(element);
        }

        public void AddRange(StiStateIndicatorFilter[] elements)
        {
            foreach (StiStateIndicatorFilter element in elements)
            {
                this.InnerList.Add(element);
            }
        }

        public void Insert(int index, StiStateIndicatorFilter element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("item");
            }
            if ((index < 0) || (index > this.InnerList.Count))
            {
                throw new ArgumentOutOfRangeException("index", "InvalidArgument");
            }

            this.InnerList.Insert(index, element);
        }

        public bool Remove(StiStateIndicatorFilter element)
        {
            int index = this.InnerList.IndexOf(element);
            if (index != -1)
            {
                this.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool Contains(StiStateIndicatorFilter element)
        {
            return (this.IndexOf(element) != -1);
        }

        public void CopyTo(StiStateIndicatorFilter[] elements, int arrayIndex)
        {
            this.InnerList.CopyTo(elements, arrayIndex);
        }

        public int IndexOf(StiStateIndicatorFilter element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return this.InnerList.IndexOf(element);
        }

        internal void SetItemInternal(int index, StiStateIndicatorFilter element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("value");
            }
            if ((index < 0) || (index >= this.InnerList.Count))
            {
                throw new ArgumentOutOfRangeException("index", "InvalidArgument");
            }

            this.InnerList[index] = element;
        }

        public bool MoveUp(StiStateIndicatorFilter element)
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

        public bool MoveDown(StiStateIndicatorFilter element)
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

        public StiFilterCollection()
        {
            
        }
    }
}