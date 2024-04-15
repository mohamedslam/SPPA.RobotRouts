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
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using System.Linq;

namespace Stimulsoft.Report.Gauge.Collections
{
    public class StiScaleCollection : 
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
            foreach (StiScaleBase component in List)
            {
                jObject.AddPropertyJObject(index.ToString(), component.SaveToJsonObject(mode));
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

                var scale = StiOptions.Services.GaugeScales.FirstOrDefault(x => x.GetType().Name == ident);
                if (scale == null)
                    throw new Exception(string.Format("Type {0} is not found!", ident));

                var scaleClone = scale.CreateNew();
                scaleClone.Gauge = scale.Gauge;

                Add(scaleClone);
                scaleClone.LoadFromJsonObject((JObject)property.Value);
            }
        }
        #endregion

        #region Fields
        private StiGauge parent = null;
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var clone = new StiScaleCollection(this.parent);

            lock (((ICollection)this).SyncRoot)
                foreach (StiScaleBase scale in this) clone.Add(scale.Clone() as StiScaleBase);

            return clone;
        }
        #endregion

        #region Properties
        public bool IsReadOnly => false;

        public StiScaleBase this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.List.Count))
                {
                    throw new ArgumentOutOfRangeException("index", "InvalidArgument");
                }
                return this.InnerList[index] as StiScaleBase;
            }
            set
            {
                this.SetItemInternal(index, value);
            }
        }
        #endregion

        #region Methods
        private void SetParent(StiScaleBase element)
        {
            element.Gauge = this.parent;
        }

        private void ClearParent(StiScaleBase element)
        {
            element.Gauge = null;
        }
        
        public void Add(StiScaleBase element)
        {
            this.AddInternal(element);
            this.InvokeElementsChanged();
        }

        public void AddRange(StiScaleBase[] elements)
        {
            try
            {
                this.AddRangeInternal(elements);
            }
            finally
            {
                this.InvokeElementsChanged();
            }
        }

        private void AddRangeInternal(IList elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("items");
            }
            foreach (StiScaleBase element in elements)
            {
                this.AddInternal(element);
            }
        }

        public void Insert(int index, StiScaleBase element)
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
            this.InvokeElementsChanged();
        }

        public bool Remove(StiScaleBase element)
        {
            int index = this.InnerList.IndexOf(element);
            if (index != -1)
            {
                ClearParent(element);
                this.RemoveAt(index);
                this.InvokeElementsChanged();

                return true;
            }

            return false;
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            base.OnRemoveComplete(index, value);
            this.InvokeElementsChanged();
        }

        protected override void OnClearComplete()
        {
            base.OnClearComplete();
            this.InvokeElementsChanged();
        }

        public bool Contains(StiScaleBase element)
        {
            return (this.IndexOf(element) != -1);
        }

        public void CopyTo(StiScaleBase[] elements, int arrayIndex)
        {
            this.InnerList.CopyTo(elements, arrayIndex);
        }

        public int IndexOf(StiScaleBase element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return this.InnerList.IndexOf(element);
        }

        internal void SetItemInternal(int index, StiScaleBase element)
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

        private void AddInternal(StiScaleBase element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            SetParent(element);
            this.InnerList.Add(element);
        }

        public bool MoveUp(StiScaleBase element)
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

        public bool MoveDown(StiScaleBase element)
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

        #region Events
        public event EventHandler ElementsChanged;
        private void InvokeElementsChanged()
        {
            this.ElementsChanged?.Invoke(this, new EventArgs());
        }
        #endregion

        public StiScaleCollection(StiGauge parent)
        {
            this.parent = parent;
        }
    }
}