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

using Stimulsoft.Base.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Base
{
    /// <summary>
    /// Describes collection of property expressions.
    /// </summary>
    public class StiAppExpressionCollection : 
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
            foreach (StiAppExpression prop in List)
            {
                jObject.AddPropertyJObject(index.ToString(), prop.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var prop = new StiAppExpression(null, null);
                prop.LoadFromJsonObject((JObject)property.Value);

                List.Add(prop);
            }
        }
        #endregion

		#region Collection
        public void Add(string name, string expression)
        {
            List.Add(new StiAppExpression(name, expression));
        }

		public void Add(StiAppExpression prop)
		{
			List.Add(prop);
		}

		public void AddRange(StiAppExpression[] props)
		{
			foreach (StiAppExpression prop in props)
				Add(prop);
		}

        public void AddRange(StiAppExpressionCollection props)
		{
            foreach (StiAppExpression prop in props)
                Add(prop);
		}


        public bool Contains(StiAppExpression prop)
		{
            return List.Contains(prop);
		}

        public bool Contains(string name)
        {
            return this[name] != null;
        }

        public int IndexOf(StiAppExpression prop)
		{
            return List.IndexOf(prop);
		}

        public void Insert(int index, StiAppExpression prop)
		{
            List.Insert(index, prop);
		}

        public void Remove(StiAppExpression prop)
		{
            List.Remove(prop);
		}

        public void Remove(string name)
        {
            var prop = this[name];
            if (prop != null)
                Remove(prop);
        }

        public StiAppExpression this[int index]
		{
			get
			{
                return (StiAppExpression)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

        public StiAppExpression this[string name]
        {
            get
            {
                foreach (StiAppExpression expression in List)
                {
                    if (expression.Name == name)                        
                        return expression;
                }
                return null;
            }
            set
            {
                for (int index = 0; index < List.Count; index++)
                {
                    var expression = List[index] as StiAppExpression;

                    if (expression.Name == name)
                    {
                        List[index] = value;
                        return;
                    }
                }
                List.Add(value);
            }
        }

        public IEnumerable<StiAppExpression> ToList()
        {
            return List.Cast<StiAppExpression>();
        }
		#endregion

		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
            var props = new StiAppExpressionCollection();

            foreach (StiAppExpression prop in List)
            {
                props.Add((StiAppExpression)prop.Clone());
            }

            return props;
		}
        #endregion
    }
}
