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
using System.Globalization;

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Describes a collection parameter.
	/// </summary>
	public class StiParametersCollection : 
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
            foreach (StiParameter parameter in List)
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
                var parameter = new StiParameter();
                parameter.LoadFromJsonObject((JObject)property.Value);

                List.Add(parameter);
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
			var al = new StiParametersCollection();

            lock (((ICollection)this).SyncRoot)
                foreach (StiParameter par in this) al.Add(new StiParameter() { Name = par.Name, Expression = par.Expression.Clone() as StiExpression });

			return al;
		}
		#endregion

		#region Collection
		private void AddCore(StiParameter parameter)
		{
			List.Add(parameter);
		}

		public void Add(StiParameter parameter)
		{
			AddCore(parameter);
		}
        
		public void AddRange(StiParametersCollection parameters)
		{
            lock (((ICollection)parameters).SyncRoot)
                foreach (StiParameter parameter in parameters) Add(parameter);
		}


		public void AddRange(StiParameter[] parameters)
		{
            lock (((ICollection)parameters).SyncRoot)
                foreach (StiParameter parameter in parameters) Add(parameter);
		}
        
		public bool Contains(StiParameter parameter)
		{
			return List.Contains(parameter);
		}
		

		public int IndexOf(StiParameter parameter)
		{
			return List.IndexOf(parameter);
		}

        public int IndexOf(string name)
        {
            name = name.ToLower(CultureInfo.InvariantCulture);
            int index = 0;
            lock (((ICollection)List).SyncRoot)
            foreach (StiParameter parameter in List)
            {
                if (parameter.Name.ToLower(CultureInfo.InvariantCulture) == name) return index;
                index++;
            }
            return -1;
        }

		public void InsertRange(int index, StiParametersCollection parameters)
		{
            lock (((ICollection)parameters).SyncRoot)
                foreach (StiParameter parameter in parameters) Insert(index, parameter);
		}
        
		public void Insert(int index, StiParameter parameter)
		{
			List.Insert(index, parameter);
		}

		public void Remove(StiParametersCollection parameters)
		{
            lock (((ICollection)parameters).SyncRoot)
			foreach (StiParameter parameter in parameters)
				Remove(parameter);
		}

		public void Remove(StiParameter parameter)
		{
			if (List.Contains(parameter)) List.Remove(parameter);
		}
		
		public StiParameter this[int index]
		{
			get
			{
                return List[index] as StiParameter;
			}
			set
			{
				List[index] = value;
			}
		}
        
		public StiParameter this[string name]
		{
			get
			{
				name = name.ToLower(CultureInfo.InvariantCulture);

			    lock (List.SyncRoot)
				    foreach (StiParameter parameter in List)
					    if (parameter.Name.ToLowerInvariant() == name) return parameter;

				return null;
			}
			set
			{
				name = name.ToLower(CultureInfo.InvariantCulture);
				for (int index = 0; index < List.Count; index++)				
				{
                    StiParameter parameter = List[index] as StiParameter;

                    if (parameter.Name.ToLowerInvariant() == name)
					{
						List[index] = value;
						return;
					}
				}
				AddCore(value);
			}
		}


		public void CopyTo(Array array, int index)
		{
			List.CopyTo(array, index);
		}
		#endregion
    }
}
