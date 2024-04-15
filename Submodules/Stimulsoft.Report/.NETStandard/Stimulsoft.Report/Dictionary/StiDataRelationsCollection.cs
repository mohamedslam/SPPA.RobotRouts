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
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes collection of relations.
    /// </summary>
    public class StiDataRelationsCollection : 
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
            foreach(StiDataRelation relation in List)
            {
                jObject.AddPropertyJObject(index.ToString(), relation.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var relation = new StiDataRelation { Dictionary = dictionary };

                relation.LoadFromJsonObject((JObject)property.Value);

                List.Add(relation);
            }
        }
        #endregion

		#region Collection
        public List<StiDataRelation> ToList()
        {
            return this.Cast<StiDataRelation>().ToList();
        }

		public void Add(StiDataRelation relation)
		{
			if (dictionary != null)relation.Dictionary = this.dictionary;
			List.Add(relation);
		}

		public void AddRange(StiDataRelation[] relations)
		{
			foreach (StiDataRelation relation in relations)
				Add(relation);
		}

		public void AddRange(StiDataRelationsCollection relations)
		{
			foreach (StiDataRelation relation in relations)
				Add(relation);
		}

		public bool Contains(StiDataRelation relation)
		{
			return List.Contains(relation);
		}

        public bool Contains(string name)
        {
            return ToList().FirstOrDefault(x => x.Name == name) != null;
        }

        public int IndexOf(StiDataRelation relation)
		{
			return List.IndexOf(relation);
		}

		public void Insert(int index, StiDataRelation relation)
		{
			List.Insert(index, relation);
		}

		public void Remove(StiDataRelation relation)
		{
			List.Remove(relation);
            cachedDataRelations.Clear();
		}

		public StiDataRelation this[int index]
		{
			get
			{
				return (StiDataRelation)List[index];
			}
			set
			{
				List[index] = value;
                cachedDataRelations.Clear();
            }
		}

		public StiDataRelation this[string relationName]
		{
			get
			{
				StiDataRelation searchedDataRelation = cachedDataRelations[relationName] as StiDataRelation;
				if (searchedDataRelation != null)return searchedDataRelation;

				foreach (StiDataRelation relation in Items)
				{
					if (relation.NameInSource == relationName)// && (!relation.Inherited))
					{
						cachedDataRelations[relationName] = relation;
						return relation;
					}
                    if (StiNameValidator.CorrectName(relation.NameInSource, dictionary?.Report) == relationName)
                    {
                        cachedDataRelations[relationName] = relation;
                        return relation;
                    }
                }

				return null;
			}
			set
			{
				for (int index = 0; index < List.Count; index++)				
				{
					StiDataRelation relation = List[index] as StiDataRelation;
					
					if (relation.Name == relationName)// && (!relation.Inherited))
					{
						List[index] = value;
                        cachedDataRelations.Clear();
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
			var relations = new StiDataRelationsCollection(dictionary);

		    foreach (StiDataRelation relation in List)
			    relations.Add((StiDataRelation)relation.Clone());

			return relations;
		}
        #endregion

        #region Properties
        public StiDataRelation[] Items => (StiDataRelation[])InnerList.ToArray(typeof(StiDataRelation));
        #endregion

        #region Fields
        private Hashtable cachedDataRelations = new Hashtable();
        private StiDictionary dictionary;
        #endregion

        #region Methods
        protected override void OnClear()
        {
            base.OnClear();

			cachedDataRelations?.Clear();
		}
		#endregion

		public StiDataRelationsCollection(StiDictionary dictionary)
		{
			this.dictionary = dictionary;
		}
	}
}
