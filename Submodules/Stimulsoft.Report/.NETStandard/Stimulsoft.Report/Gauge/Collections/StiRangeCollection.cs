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
using Stimulsoft.Report.Components.Gauge.Primitives;
using System;
using System.Collections;
using System.Linq;

namespace Stimulsoft.Report.Gauge.Collections
{
    public class StiRangeCollection :
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
            foreach (StiRangeBase range in List)
            {
                jObject.AddPropertyJObject(index.ToString(), range.SaveToJsonObject(mode));
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

                var range = StiOptions.Services.Ranges.FirstOrDefault(x => x.GetType().Name == ident);
                if (range == null)
                    throw new Exception(string.Format("Type {0} is not found!", ident));

                var rangeClone = range.CreateNew();
                
                Add(rangeClone);
                rangeClone.LoadFromJsonObject((JObject)property.Value);
            }
        }
        #endregion

        #region Fields
        private StiScaleRangeList parent = null;
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var clone = new StiRangeCollection(this.parent);

            lock (((ICollection)this).SyncRoot)
                foreach (StiRangeBase element in this) clone.Add(element.Clone() as StiRangeBase);

            return clone;
        }
        #endregion

        #region Properties
        public bool IsReadOnly => false;

        public virtual StiRangeBase this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.InnerList.Count))
                {
                    throw new ArgumentOutOfRangeException("index", "InvalidArgument");
                }
                return this.InnerList[index] as StiRangeBase;
            }
            set
            {
                this.SetItemInternal(index, value);
            }
        }
        #endregion

        #region Methods
        protected virtual void SetParent(StiRangeBase element)
        {
            element.rangeList = this.parent;
        }

        protected virtual void ClearParent(StiRangeBase element)
        {
            element.rangeList = null;
        }

        public void Add(StiRangeBase element)
        {
            this.AddInternal(element);
        }

        public void AddRange(StiRangeBase[] elements)
        {
            foreach (StiRangeBase element in elements)
            {
                this.AddInternal(element);
            }
        }

        public void Insert(int index, StiRangeBase element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("item");
            }
            if ((index < 0) || (index > this.InnerList.Count))
            {
                throw new ArgumentOutOfRangeException("index", "InvalidArgument");
            }

            SetParent(element);
            this.InnerList.Insert(index, element);
        }

        public bool Remove(StiRangeBase element)
        {
            int index = this.InnerList.IndexOf(element);
            if (index != -1)
            {
                ClearParent(element);
                this.RemoveAt(index);

                return true;
            }

            return false;
        }

        public bool Contains(StiRangeBase element)
        {
            return (this.IndexOf(element) != -1);
        }

        public void CopyTo(StiRangeBase[] elements, int arrayIndex)
        {
            this.InnerList.CopyTo(elements, arrayIndex);
        }

        public int IndexOf(StiRangeBase element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return this.InnerList.IndexOf(element);
        }

        internal void SetItemInternal(int index, StiRangeBase element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("value");
            }
            if ((index < 0) || (index >= this.InnerList.Count))
            {
                throw new ArgumentOutOfRangeException("index", "InvalidArgument");
            }

            SetParent(element);
            this.InnerList[index] = element;
        }

        private void AddInternal(StiRangeBase element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            SetParent(element);
            this.InnerList.Add(element);
        }

        public bool MoveUp(StiRangeBase element)
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

        public bool MoveDown(StiRangeBase element)
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

        public StiRangeCollection(StiScaleRangeList parent)
        {
            this.parent = parent;
        }
    }
}