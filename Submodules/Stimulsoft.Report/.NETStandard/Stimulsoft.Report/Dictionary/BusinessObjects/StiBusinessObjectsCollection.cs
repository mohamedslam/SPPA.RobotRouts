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
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using System.Globalization;

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
    /// Collection of business objects.
	/// </summary>
	public class StiBusinessObjectsCollection : 
		CollectionBase, 
		ICloneable,
		IComparer, 
        IStiJsonReportObject
	{
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach(StiBusinessObject businessObject in List)
            {
                jObject.AddPropertyJObject(index.ToString(), businessObject.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var businessObject = new StiBusinessObject();
                businessObject.Dictionary = dictionary;

                businessObject.LoadFromJsonObject((JObject)property.Value);
                List.Add(businessObject);
            }
        }
        #endregion

		#region IComparer
		private int directionFactor = 1;
		int IComparer.Compare(object x, object y)
		{
            StiBusinessObject source1 = x as StiBusinessObject;
            StiBusinessObject source2 = y as StiBusinessObject;

			if (StiOptions.Designer.SortDictionaryByAliases)
				return source1.Alias.CompareTo(source2.Alias) * directionFactor;
			else 
				return source1.Name.CompareTo(source2.Name) * directionFactor;
		}
		#endregion

		#region Collection
        public List<StiBusinessObject> ToList()
        {
            return this.Cast<StiBusinessObject>().ToList();
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
            base.OnSet(index, oldValue, newValue);

            StiBusinessObject businessObject = newValue as StiBusinessObject;

            if (dictionary != null) businessObject.Dictionary = dictionary;
            if (parentBusinessObject != null) businessObject.ParentBusinessObject = parentBusinessObject;
        }

        protected override void OnInsert(int index, object value)
        {
            base.OnInsert(index, value);

            StiBusinessObject businessObject = value as StiBusinessObject;

            if (dictionary != null) businessObject.Dictionary = dictionary;
            if (parentBusinessObject != null) businessObject.ParentBusinessObject = parentBusinessObject;

        }

        public void Add(StiBusinessObject source)
		{			
			List.Add(source);
		}

        public void AddRange(StiBusinessObject[] sources)
		{
			base.InnerList.AddRange(sources);
		}

        public void AddRange(StiBusinessObjectsCollection sources)
		{
            foreach (StiBusinessObject source in sources)
				Add(source);
		}

        public bool Contains(StiBusinessObject source)
		{
			return List.Contains(source);
		}

        public bool Contains(string name)
        {
            name = name.ToLowerInvariant();
            foreach (StiBusinessObject businessObject in this)
            {
                if (businessObject.Name.ToLowerInvariant() == name) 
                    return true;
            }
            return false;
        }

        public int IndexOf(StiBusinessObject source)
		{
			return List.IndexOf(source);
		}

        public void Insert(int index, StiBusinessObject source)
		{
			List.Insert(index, source);
		}

		protected override void OnClear()
		{
			base.OnClear();
            if (cachedBusinessObjects != null)
            {
                cachedBusinessObjects.Clear();
                cachedBusinessObjects = null;
            }
		}

        public void Remove(StiBusinessObject source)
		{
			List.Remove(source);
            string businessObjectName = source.Name.ToLower(CultureInfo.InvariantCulture);
            if (CachedBusinessObjects[businessObjectName] != null) CachedBusinessObjects.Remove(businessObjectName);
		}

        public StiBusinessObject this[int index]
		{
			get
			{
				return (StiBusinessObject)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

        internal Hashtable cachedBusinessObjects;
        internal Hashtable CachedBusinessObjects
        {
            get
            {
                if (cachedBusinessObjects == null)
                    cachedBusinessObjects = new Hashtable();
                return cachedBusinessObjects;
            }
        }

        public StiBusinessObject this[string name]
		{
			get
			{
				name = name.ToLower(CultureInfo.InvariantCulture);
                StiBusinessObject searchedSource = CachedBusinessObjects[name] as StiBusinessObject;
				if (searchedSource != null)return searchedSource;

                foreach (StiBusinessObject source in List)
				{
                    if (source.Name.ToLower(CultureInfo.InvariantCulture) == name)
                    {
                        CachedBusinessObjects[name] = source;
                        return source;
                    }
                    else
                    {
                        if (StiNameValidator.CorrectName(source.Name.ToLower(CultureInfo.InvariantCulture), dictionary?.Report) == StiNameValidator.CorrectName(name, dictionary?.Report))
                        {
                            cachedBusinessObjects[name] = source;
                            return source;
                        }
                    }
				}
				return null;
			}
			set
			{
				name = name.ToLower(CultureInfo.InvariantCulture);
				for (int index = 0; index < List.Count; index++)				
				{
                    StiBusinessObject source = List[index] as StiBusinessObject;

					if (source.Name.ToLower(CultureInfo.InvariantCulture) == name)
					{
						List[index] = value;
						return;
					}
				}
				Add(value);
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
            StiBusinessObjectsCollection sources = new StiBusinessObjectsCollection(this.dictionary, this.parentBusinessObject);
            foreach (StiBusinessObject data in List)
                sources.Add((StiBusinessObject)data.Clone());
			return sources;
		}
		#endregion

		#region Sort
		public void Sort()
		{
			Sort(StiSortOrder.Asc);
		}

		public void Sort(StiSortOrder order)
		{
			Sort(order, true);
		}

		public void Sort(StiSortOrder order, bool sortColumns)
		{
			if (order == StiSortOrder.Asc)directionFactor = 1;
			else directionFactor = -1;

			base.InnerList.Sort(this);

			if (sortColumns)
			{
                foreach (StiBusinessObject source in this)
				{
					source.Columns.Sort(order);
                    StiBusinessObjectsCollection businessObject = source.BusinessObjects;

                    if (businessObject != null && businessObject.Count > 0)
                        businessObject.Sort(order);
				}
			}
		}
		#endregion

        #region Methods
        public void Connect()
        {
            foreach (StiBusinessObject businessObject in this.List)
            {
                businessObject.Connect();
            }
        }

        public void Disconnect()
        {
            foreach (StiBusinessObject businessObject in this.List)
            {
                businessObject.Disconnect();
            }
        }
        #endregion

        #region Fields
        internal StiDictionary dictionary;
        internal StiBusinessObject parentBusinessObject;
        #endregion

        /// <summary>
		/// Creates the collection of Data Sources.
		/// </summary>
		/// <param name="dictionary">The dictionary in which the collection is registered.</param>
        public StiBusinessObjectsCollection(StiDictionary dictionary, StiBusinessObject parentBusinessObject)
		{
			this.dictionary = dictionary;
            this.parentBusinessObject = parentBusinessObject;
		}
	}
}
