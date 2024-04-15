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
using Stimulsoft.Base.Json.Linq;
using System.Linq;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Gauge.Collections
{
    public class StiCustomValuesCollection: 
        CollectionBase,
        ICloneable
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiCustomValueBase value in List)
            {
                jObject.AddPropertyJObject(index.ToString(), value.SaveToJsonObject(mode));
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

                var value = StiOptions.Services.CustomValues.FirstOrDefault(x => x.GetType().Name == ident);
                if (value == null)
                    throw new Exception(string.Format("Type {0} is not found!", ident));

                var valueClone = value.CreateNew();

                Add(valueClone);
                valueClone.LoadFromJsonObject((JObject)property.Value);
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
            var clone = new StiCustomValuesCollection();

            lock (((ICollection)this).SyncRoot)
                foreach (StiCustomValueBase value in this) clone.Add(value.Clone() as StiCustomValueBase);

            return clone;
        }
        #endregion

        #region Properties
        public bool IsReadOnly => false;

        public StiCustomValueBase this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.List.Count))
                {
                    throw new ArgumentOutOfRangeException("index", "InvalidArgument");
                }
                return this.InnerList[index] as StiCustomValueBase;
            }
            set
            {
                this.SetItemInternal(index, value);
            }
        }
        #endregion

        #region Methods
        public void Add(StiCustomValueBase element)
        {
            this.InnerList.Add(element);
        }

        public void AddRange(StiCustomValueBase[] elements)
        {
            foreach (StiCustomValueBase element in elements)
            {
                this.InnerList.Add(element);
            }
        }

        public void Insert(int index, StiCustomValueBase element)
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

        public bool Remove(StiCustomValueBase element)
        {
            int index = this.InnerList.IndexOf(element);
            if (index != -1)
            {
                this.RemoveAt(index);

                return true;
            }

            return false;
        }

        public bool Contains(StiCustomValueBase element)
        {
            return (this.IndexOf(element) != -1);
        }

        public void CopyTo(StiCustomValueBase[] elements, int arrayIndex)
        {
            this.InnerList.CopyTo(elements, arrayIndex);
        }

        public int IndexOf(StiCustomValueBase element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return this.InnerList.IndexOf(element);
        }

        internal void SetItemInternal(int index, StiCustomValueBase element)
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

        public bool MoveUp(StiCustomValueBase element)
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

        public bool MoveDown(StiCustomValueBase element)
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
    }
}