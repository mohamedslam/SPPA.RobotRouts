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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the column collection.
    /// </summary>    
    public class StiDataParametersCollection :
        CollectionBase,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            var index = 0;
            foreach (StiDataParameter parameter in List)
            {
                jObject.AddPropertyJObject(index.ToString(), parameter.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var parameter = new StiDataParameter();
                parameter.LoadFromJsonObject((JObject)property.Value);

                List.Add(parameter);
            }
        }
        #endregion

        #region Collection
        public List<StiDataParameter> ToList()
        {
            return this.Cast<StiDataParameter>().ToList();
        }

        protected override void OnInsert(int index, object value)
        {
            if (((StiDataParameter)value).DataSource == null)
                ((StiDataParameter)value).DataSource = dataSource;

            if (((StiDataParameter)value).dataParametersCollection == null)
                ((StiDataParameter)value).dataParametersCollection = this;
        }

        public void Add(StiDataParameter parameter)
        {
            List.Add(parameter);
        }

        public void AddRange(StiDataParametersCollection parameters)
        {
            foreach (StiDataParameter parameter in parameters)
            {
                Add(parameter);
            }
        }

        public void AddRange(StiDataParameter[] parameters)
        {
            foreach (var parameter in parameters)
                Add(parameter);
        }

        public bool Contains(StiDataParameter parameter)
        {
            return List.Contains(parameter);
        }

        public bool Contains(string name)
        {
            return List.Cast<StiDataParameter>().Any(parameter => parameter.Name == name);
        }

        public int IndexOf(StiDataParameter parameter)
        {
            return List.IndexOf(parameter);
        }

        public void Insert(int index, StiDataParameter parameter)
        {
            List.Insert(index, parameter);
        }

        public void Remove(StiDataParameter parameter)
        {
            List.Remove(parameter);

            var parameterName = parameter.Name.ToLower(CultureInfo.InvariantCulture);

            if (CachedDataParameters.Contains(parameterName))
                CachedDataParameters.Remove(parameterName);
        }

        public StiDataParameter this[int index]
        {
            get
            {
                return (StiDataParameter)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        private Hashtable CachedDataParameters = new Hashtable();

        public StiDataParameter this[string name]
        {
            get
            {
                name = name.ToLower(CultureInfo.InvariantCulture);
                var searchedDataParameter = CachedDataParameters[name] as StiDataParameter;
                if (searchedDataParameter != null) return searchedDataParameter;

                foreach (StiDataParameter parameter in List)
                {
                    if (parameter.Name.ToLower(CultureInfo.InvariantCulture) == name)
                    {
                        CachedDataParameters[name] = parameter;
                        return parameter;
                    }
                }
                return null;
            }
            set
            {
                name = name.ToLower(CultureInfo.InvariantCulture);
                for (var index = 0; index < List.Count; index++)
                {
                    var parameter = List[index] as StiDataParameter;

                    if (parameter.Name.ToLower(CultureInfo.InvariantCulture) == name)
                    {
                        List[index] = value;
                        return;
                    }
                }
                Add(value);
            }
        }
        #endregion

        #region Fields
        private StiDataSource dataSource;
        #endregion

        public StiDataParametersCollection() : this(null)
        {
        }

        public StiDataParametersCollection(StiDataSource dataSource)
        {
            this.dataSource = dataSource;
        }
    }
}
