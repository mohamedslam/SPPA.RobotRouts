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
using System.Linq;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Components.Gauge.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Report.Gauge.Collections
{
    public class StiGaugeElementCollection :
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
            foreach (StiGaugeElement component in List)
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

                var element = StiOptions.Services.GaugeElements.FirstOrDefault(x => x.GetType().Name == ident);
                if (element == null)
                    throw new Exception(string.Format("Type {0} is not found!", ident));

                var elementClone = element.CreateNew();
                elementClone.Scale = element.Scale;

                Add(elementClone);
                elementClone.LoadFromJsonObject((JObject)property.Value);
            }
        }
        #endregion

        #region Fields
        private readonly StiScaleBase scale;
        private readonly StiGaugeElemenType scaleType;
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var clone = new StiGaugeElementCollection(this.scale);

            lock (((ICollection)this).SyncRoot)
                foreach (StiGaugeElement element in this) clone.Add(element.Clone() as StiGaugeElement);

            return clone;
        }
        #endregion

        #region Properties
        public bool IsReadOnly => false;

        public StiGaugeElement this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.InnerList.Count))
                {
                    throw new ArgumentOutOfRangeException("index", "InvalidArgument");
                }
                return this.InnerList[index] as StiGaugeElement;
            }
            set
            {

                value.Scale = this.scale;
                this.SetItemInternal(index, value);
            }
        }        
        #endregion

        #region Methods
        public List<StiGaugeElement> ToArray()
        {
            var list = new List<StiGaugeElement>();
            foreach(StiGaugeElement element in this.InnerList)
            {
                list.Add(element);
            }

            return list;
        }

        private void AddCore(StiGaugeElement element)
        {
            if (element.ElementType == this.scaleType || element.ElementType == StiGaugeElemenType.All)
            {
                element.Scale = this.scale;
                this.InnerList.Add(element);
            }
        }

        public void Add(StiGaugeElement element)
        {
            AddCore(element);
        }

        public void AddRange(StiGaugeElement[] elements)
        {
            foreach (StiGaugeElement element in elements)
            {
                AddCore(element);
            }
        }

        public void Insert(int index, StiGaugeElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("item");
            }
            if ((index < 0) || (index > this.InnerList.Count))
            {
                throw new ArgumentOutOfRangeException("index", "InvalidArgument");
            }

            AddCore(element);
        }

        public bool Remove(StiGaugeElement element)
        {
            int index = this.InnerList.IndexOf(element);
            if (index != -1)
            {
                element.Scale = null;
                this.RemoveAt(index);

                return true;
            }

            return false;
        }

        public bool Contains(StiGaugeElement element)
        {
            return (this.IndexOf(element) != -1);
        }

        public void CopyTo(StiGaugeElement[] elements, int arrayIndex)
        {
            this.InnerList.CopyTo(elements, arrayIndex);
        }

        public int IndexOf(StiGaugeElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return this.InnerList.IndexOf(element);
        }

        internal void SetItemInternal(int index, StiGaugeElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("value");
            }
            if ((index < 0) || (index >= this.InnerList.Count))
            {
                throw new ArgumentOutOfRangeException("index", "InvalidArgument");
            }

            if (element.ElementType == this.scaleType || element.ElementType == StiGaugeElemenType.All)
            {
                element.Scale = this.scale;
                this.InnerList[index] = element;
            }
        }

        public bool MoveUp(StiGaugeElement element)
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

        public bool MoveDown(StiGaugeElement element)
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

        public StiGaugeElementCollection(StiScaleBase scale)
        {
            this.scaleType = scale.ScaleType;
            this.scale = scale;
        }
    }
}