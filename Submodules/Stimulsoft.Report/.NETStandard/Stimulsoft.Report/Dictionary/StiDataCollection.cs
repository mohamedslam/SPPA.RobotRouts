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
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;

#if STIDRAWING
using Stimulsoft.Data;
#endif

#if NETSTANDARD || NETCOREAPP
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes a collection of data.
    /// </summary>
    [Serializable]
	public class StiDataCollection : CollectionBase, ICloneable
	{
		#region Collection
        public List<StiData> ToList()
        {
            return this.Cast<StiData>().ToList();
        }

		public void Add(StiData data)
		{
			List.Add(data);
		}

		public void AddRange(StiData[] data)
		{
			base.InnerList.AddRange(data);
		}

		public bool Contains(StiData data)
		{
			return List.Contains(data);
		}

        public bool Contains(string name)
        {
            return ToList().FirstOrDefault(x => x.Name == name) != null;
        }

        public int IndexOf(StiData data)
		{
			return List.IndexOf(data);
		}

		public void Insert(int index, StiData data)
		{
			List.Insert(index, data);
		}

		public void Remove(StiData data)
		{
			List.Remove(data);
		}		
		
		public StiData this[int index]
		{
			get
			{
				return (StiData)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

		public StiData this[string name]
		{
			get
			{
				name = name.ToLowerInvariant();

			    foreach (StiData data in List)
			    {
			        if (data?.Name?.ToLowerInvariant() == name)
			            return data;
			    }

			    return null;
			}
			set
			{
				name = name.ToLowerInvariant();
				for (var index = 0; index < List.Count; index++)				
				{
					var data = List[index] as StiData;
					
					if (data?.Name?.ToLowerInvariant() == name)
					{
						List[index] = value;
						return;
					}
				}
				Add(value);
			}
		}

		public StiData[] Items => (StiData[])InnerList.ToArray(typeof(StiData));
	    #endregion

		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion

		#region Methods
		public void RegData(string name, object data)
		{
			RegData(name, name, data);
		}

		public void RegData(string name, DataTable dataTable)
		{
			RegData(name, name, dataTable);
		}

		public void RegData(string name, DataSet dataSet)
		{
			RegData(name, name, dataSet);
		}

		public void RegData(string name, DataView dataView)
		{
			RegData(name, name, dataView);
		}

		public void RegData(string name, SqlConnection connection)
		{
			RegData(name, name, connection);
		}

        public void RegData(string name, OleDbConnection connection)
		{
			RegData(name, name, connection);
		}

		public void RegData(string name, OdbcConnection connection)
		{
			RegData(name, name, connection);
		}

		public void RegData(string name, string alias, object data)
		{
		    if (data == null) return;

		    #region DataView
		    if (data is DataView)
		    {
		        RegData(name, alias, data as DataView);
		        return;
		    }
		    #endregion

		    #region DataTable
		    if (data is DataTable)
		    {
		        RegData(name, alias, data as DataTable);
		        return;
		    }
		    #endregion

		    #region DataSet
		    if (data is DataSet)
		    {
		        RegData(name, alias, data as DataSet);
		        return;
		    }
		    #endregion

		    #region IDbConnection
		    if (data is IDbConnection)
		    {
		        this.Add(new StiData(name, data));
		        this[Count - 1].OriginalConnectionState = ((IDbConnection)data).State;
		        this[Count - 1].Alias = alias;
		        return;
		    }
		    #endregion

		    #region StiUserData
		    if (data is StiUserData)
		    {
		        this.Add(new StiData(name, data));
		        this[Count - 1].Alias = alias;
		        return;
		    }
		    #endregion

		    #region BindingSource
		    if (data is System.Windows.Forms.BindingSource)
		    {
		        var bindingSource = data as System.Windows.Forms.BindingSource;

		        this.RegData(name, bindingSource.List);
		        this[Count - 1].Name = name;
		        return;
		    }
		    #endregion

		    #region Reg business object
		    if (data is IDataReader)
		        throw new Exception("IDataReader is not supported!");

		    if (data is IDataAdapter)
		        throw new Exception("IDataAdapter is not supported!");

		    var converter = new StiBusinessObjectToDataSet();
		    var dataSet = converter.ConvertBusinessObjectToDataSet(name, data);
					
		    if (dataSet != null)
		    {
		        foreach (DataTable table in dataSet.Tables)
		        {
		            RegData(table.TableName, alias, table);
		            this[Count - 1].Data = data;
		            this[Count - 1].IsBusinessObjectData = true;
		        }
		    }
		    else 
		    {
		        this.Add(new StiData(name, data));
		        this[Count - 1].Alias = alias;
		        this[Count - 1].IsBusinessObjectData = true;
		    }
		    #endregion
		}


		public void RegData(string name, string alias, DataTable dataTable)
		{
		    if (dataTable == null) return;

		    if (this[name] == null)
		    {
		        this.Add(new StiData(name, dataTable, dataTable));
		        this[Count - 1].Alias = alias;
		    }
		    else 
		    {
		        this[name].Data = dataTable;
		        this[name].ViewData = dataTable;
		        this[name].Alias = alias;
		    }
		}

		public void RegData(string name, string alias, DataSet dataSet)
		{
		    if (dataSet == null) return;

		    foreach (DataTable table in dataSet.Tables)
		    {
		        RegData($"{name}.{table.TableName}", alias, table);
		    }
		}

		public void RegData(string name, string alias, DataView dataView)
		{
		    if (dataView == null) return;

		    if (this[name] == null)
		    {
		        this.Add(new StiData(name, dataView, dataView));
		        this[Count - 1].Alias = alias;
		    }
		    else 
		    {
		        this[name].Data = dataView;
		        this[name].Alias = alias;
		    }
		}

		public void RegData(string name, string alias, SqlConnection connection)
		{
			RegData(name, alias, (object)connection);
		}

		public void RegData(string name, string alias, OleDbConnection connection)
		{
			RegData(name, alias, (object)connection);
		}

		public void RegData(string name, string alias, OdbcConnection connection)
		{
			RegData(name, alias, (object)connection);
		}

        public void RegData(DataTable dataTable)
		{
			var name = dataTable.TableName;
			if (string.IsNullOrEmpty(name))
			    name = "Table";

			RegData(name, dataTable);
		}

		public void RegData(DataSet dataSet)
		{
			RegData(dataSet.DataSetName, dataSet);
		}

		public void RegData(DataView dataView)
		{
			RegData($"view{dataView.Table.TableName}", dataView);
		}

		public void RegData(StiDataCollection datas)
		{
			if (datas == this)return;

			foreach (StiData data in datas)
			{
				this.Add(data);
			}
		}

		public void ClearReportDatabase()
		{
			var index = 0;
			while (index < Count)
			{
				if (this[index].IsReportData)
				{
					if (this[index].ViewData is IDisposable)
					{
						try
						{
							((IDisposable)this[index].ViewData).Dispose();
						}
						catch
						{
						}
					}
					RemoveAt(index);
				}
				else index ++;
			}
		}

		public StiDataCollection GetData(Type typeData)
		{
			var datas = new StiDataCollection();
		    foreach (StiData data in List)
		    {
		        if (data.ViewData.GetType() == typeData)
		            datas.Add(data);
		    }

		    return datas;
		}
        #endregion
	}
}
