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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary.Design;
using Stimulsoft.Report.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the datasource which based on other datasource.
    /// </summary>
    [TypeConverter(typeof(StiVirtualSourceConverter))]
	public class StiVirtualSource : 
		StiDataStoreSource, 
		IStiFilter
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiVirtualSource
            jObject.AddPropertyEnum("FilterMode", FilterMode, StiFilterMode.And);
            jObject.AddPropertyJObject("Filters", Filters.SaveToJsonObject(mode));
            jObject.AddPropertyStringArray("GroupColumns", GroupColumns);
            jObject.AddPropertyStringArray("Results", Results);
            jObject.AddPropertyStringArray("Sort", Sort);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "FilterMode":
                        this.FilterMode = property.DeserializeEnum<StiFilterMode>();
                        break;

                    case "Filters":
                        this.Filters.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "GroupColumns":
                        this.GroupColumns = property.DeserializeStringArray();
                        break;

                    case "Results":
                        this.Results = property.DeserializeStringArray();
                        break;

                    case "Sort":
                        this.Sort = property.DeserializeStringArray();
                        break;
                }
            }
        }
        #endregion

        #region IStiFilter
        [Browsable(false)]
        public StiFilterEventHandler FilterMethodHandler { get; set; }

        [Browsable(false)]
        public virtual bool FilterOn { get; set; } = true;

        /// <summary>
        /// Gets or sets filter mode.
        /// </summary>
        [DefaultValue(StiFilterMode.And)]
        [StiSerializable]
        [Browsable(false)]
        public StiFilterMode FilterMode { get; set; } = StiFilterMode.And;

		/// <summary>
		/// Gets or sets the collection of data filters.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.List)]
		[Browsable(false)]
		public virtual StiFiltersCollection Filters { get; set; } = new StiFiltersCollection();
		#endregion

		#region DataAdapter
        protected override Type GetDataAdapterType()
        {
            return typeof(StiVirtualAdapterService);
        }
        #endregion

        #region Properties
        public override StiComponentId ComponentId => StiComponentId.StiVirtualSource;

        [StiSerializable(StiSerializationVisibility.List)]
        [Browsable(false)]
        public string[] GroupColumns { get; set; } = new string[0];

        [StiSerializable(StiSerializationVisibility.List)]
        [Browsable(false)]
        public string[] Results { get; set; } = new string[0];
        
        [StiSerializable(StiSerializationVisibility.List)]
        [Browsable(false)]
        public virtual string[] Sort { get; set; } = new string[0];
        #endregion

        #region Methods
        internal void ConnectToData(bool allowConnect = false)
        {
            if (allowConnect)
            {
                var masterSource = Dictionary.DataSources[this.NameInSource];
                if (masterSource.DataTable == null)
                {
                    if (StiDataPicker.ExistsInCache(masterSource))
                        masterSource.DataTable = StiDataPicker.GetFromCache(masterSource)?.Copy();
                    else
                        masterSource.DataTable = StiDataPicker.GetDataTable(Dictionary.Report, masterSource)?.Copy();
                }
            }

            ConnectToDataInternal();
        }

        private void ConnectToDataInternal()
		{
			object filterMethodHandler = null;

			#region Get Filter Method
			string correctedDataName = StiNameValidator.CorrectName(this.Name, Dictionary.Report);

			var type = Dictionary.Report.GetType();
			var method = type.GetMethod($"{correctedDataName}__GetFilter", 
				new Type[] { typeof(object), typeof(StiFilterEventArgs) });
				
			if (method != null)
			{
                filterMethodHandler = Delegate.CreateDelegate(
                    typeof(StiFilterEventHandler), this.Dictionary.Report, $"{correctedDataName}__GetFilter")
                    as StiFilterEventHandler;
			}

            if ((filterMethodHandler == null && Dictionary.Report.CalculationMode == StiCalculationMode.Interpretation) || 
                Dictionary.Report.IsDesigning)
            {
                var tempBand = new StiDataBand
                {
                    Name = "VirtualSourceBand",
                    Page = this.Dictionary.Report.Pages[0],
                    DataSourceName = this.NameInSource,
                    Filters = this.Filters,
                    FilterMode = this.FilterMode,
                    FilterOn = this.FilterOn
                };
                filterMethodHandler = StiDataHelper.GetFilterEventHandler(tempBand, tempBand);
            }
			#endregion

			var masterSource = Dictionary.DataSources[this.NameInSource];
			if (masterSource == null)
				throw new ArgumentException($"'{Name}' filter. Datasource '{NameInSource}' is not found.");

			#region Create Sort
			var listSort = new List<string>();
			foreach (string group in GroupColumns)
			{
				string groupField = group;
                if (groupField.StartsWithInvariant("DESC"))
				{
                    if (masterSource.Columns[groupField] != null && masterSource.Columns[groupField.Substring(4)] == null)
                    {
                        listSort.Add("ASC");
                    }
                    else
                    {
                        listSort.Add("DESC");
                        groupField = groupField.Substring(4);
                    }
				}

                else if (groupField.StartsWithInvariant("NONE"))
                    continue;

				else
                    listSort.Add("ASC");

                var strs = groupField.Split(new char[] { '.' });
				int index = 0;
				foreach (string str in strs)
				{
					string col = str;
					if ((index == 0 && strs.Length > 1) || index < strs.Length - 1)
						col = StiDataColumn.GetRelationName(Dictionary, masterSource, str);

                    listSort.Add(col);
				}
			}

			foreach (string strSort in Sort)
			{
				listSort.Add(strSort);
			}
			var sort = listSort.ToArray();

            if (sort.Length == 0) 
                sort = null;
			#endregion

            masterSource.ResetDetailsRows();
            masterSource.First();
			masterSource.SetSort(null, sort, null, null, null);
			masterSource.SetFilter(filterMethodHandler);

			#region Create Table
			var table = new DataTable();

			foreach (StiDataColumn column in Columns)
			{
				var dataColumn = new DataColumn(column.Name, column.Type);
				table.Columns.Add(dataColumn);
			}
			#endregion

			#region Prepare Totals
			bool totalsExist = false;

			var totalsHash = new Hashtable();
            var totalsList = new List<StiAggregateFunctionService>();

			foreach (string group in GroupColumns)
			{
				string groupField = group;
                if (groupField.StartsWithInvariant("DESC"))
                {
                    if (!(masterSource.Columns[groupField] != null && masterSource.Columns[groupField.Substring(4)] == null))
                        groupField = groupField.Substring(4);
                }

                else if (groupField.StartsWithInvariant("NONE"))
                    groupField = groupField.Substring(4);

				var totalGroup = new StiFirstFunctionService();
				totalsHash[groupField] = totalGroup;
				totalsList.Add(totalGroup);
			}

			int totalIndex = 0;
			while (totalIndex < this.Results.Length)
			{
				var column = Results[totalIndex++];//Do not remove!
				var function = Results[totalIndex++];
				var name = Results[totalIndex++];

				switch (function)
				{
					case "Sum":
						StiAggregateFunctionService total1 = new StiSumDecimalFunctionService();
						totalsHash[name] = total1;
						totalsList.Add(total1);
						totalsExist = true;
						break;

                    case "SumDistinct":
                        StiAggregateFunctionService total10 = new StiSumDistinctDecimalFunctionService();
                        totalsHash[name] = total10;
                        totalsList.Add(total10);
                        totalsExist = true;
                        break;

					case "Count":
						StiAggregateFunctionService total7 = new StiCountFunctionService();
						totalsHash[name] = total7;
						totalsList.Add(total7);
						totalsExist = true;
						break;

					case "CountDistinct":
						StiAggregateFunctionService total8 = new StiCountDistinctFunctionService();
						totalsHash[name] = total8;
						totalsList.Add(total8);
						totalsExist = true;
						break;

					case "Min":
						StiAggregateFunctionService total2 = new StiMinDecimalFunctionService();
						totalsHash[name] = total2;
						totalsList.Add(total2);
						totalsExist = true;
						break;

					case "Max":
						StiAggregateFunctionService total3 = new StiMaxDecimalFunctionService();
						totalsHash[name] = total3;
						totalsList.Add(total3);
						totalsExist = true;
						break;

					case "Avg":
						StiAggregateFunctionService total4 = new StiAvgDecimalFunctionService();
						totalsHash[name] = total4;
						totalsList.Add(total4);
						totalsExist = true;
						break;
					
					case "First":
						StiAggregateFunctionService total5 = new StiFirstFunctionService();
						totalsHash[name] = total5;
						totalsList.Add(total5);
						totalsExist = true;
						break;

					case "Last":
						StiAggregateFunctionService total6 = new StiLastFunctionService();
						totalsHash[name] = total6;
						totalsList.Add(total6);
						totalsExist = true;
						break;

					case "MinDate":
						StiAggregateFunctionService total11 = new StiMinDateFunctionService();
						totalsHash[name] = total11;
						totalsList.Add(total11);
						totalsExist = true;
						break;

					case "MaxDate":
						StiAggregateFunctionService total12 = new StiMaxDateFunctionService();
						totalsHash[name] = total12;
						totalsList.Add(total12);
						totalsExist = true;
						break;

                    case "MinTime":
                        StiAggregateFunctionService total1_1 = new StiMinTimeFunctionService();
                        totalsHash[name] = total1_1;
                        totalsList.Add(total1_1);
                        totalsExist = true;
                        break;

                    case "MaxTime":
                        StiAggregateFunctionService total1_2 = new StiMaxTimeFunctionService();
                        totalsHash[name] = total1_2;
                        totalsList.Add(total1_2);
                        totalsExist = true;
                        break;

                    case "MinStr":
                        StiAggregateFunctionService total1_3 = new StiMinStrFunctionService();
                        totalsHash[name] = total1_3;
                        totalsList.Add(total1_3);
                        totalsExist = true;
                        break;

                    case "MaxStr":
                        StiAggregateFunctionService total1_4 = new StiMaxStrFunctionService();
                        totalsHash[name] = total1_4;
                        totalsList.Add(total1_4);
                        totalsExist = true;
                        break;

                    case "Mode":
                        StiAggregateFunctionService total1_5 = new StiModeDecimalFunctionService();
                        totalsHash[name] = total1_5;
                        totalsList.Add(total1_5);
                        totalsExist = true;
                        break;

                    case "Median":
                        StiAggregateFunctionService total1_6 = new StiMedianDecimalFunctionService();
                        totalsHash[name] = total1_6;
                        totalsList.Add(total1_6);
                        totalsExist = true;
                        break;

					default:
						StiAggregateFunctionService total0 = new StiFirstFunctionService();
						totalsHash[name] = total0;
						totalsList.Add(total0);
						break;
				}
			}
			#endregion

			#region Fill Data
			var groupExist = GroupColumns.Length > 0;

			//Collection of previous values
			var prevValues = new object[this.GroupColumns.Length];

			//Is first row
            bool firstRow = true;
			int rowCount = 0;

			if ((!groupExist) && totalsExist)
                InitTotals(totalsList);

			bool needAddRow = (!totalsExist) && (!groupExist);

            #region Check columns for relations, only current level 
            string[] results2 = new string[Results.Length];
            Results.CopyTo(results2, 0);
            int resultIndex2 = 0;
            while (resultIndex2 < results2.Length)
            {
                string column = results2[resultIndex2];
                results2[resultIndex2] = StiNameValidator.CorrectName(column);

                string[] parts = column.Split(new char[] { '.' });
                if (parts.Length > 1)
                {
                    string relation = parts[0];
                    int partIndex = 0;
                    while (partIndex < parts.Length - 1)
                    {
                        if (masterSource.GetParentRelations().Contains(relation))
                        {
                            var stColumn = column.Substring(relation.Length + 1);
                            results2[resultIndex2] = StiNameValidator.CorrectName(relation) + "." + StiNameValidator.CorrectName(stColumn);
                            break;
                        }
                        partIndex++;
                        relation += "." + parts[partIndex];
                    };
                }
                resultIndex2 += 3;
            }
            #endregion

            masterSource.First();
			Dictionary.Report.Line = 1;
			Dictionary.Report.LineThrough = 1;
			while (!masterSource.IsEof)
			{
				#region Group Exist
				if (groupExist)
				{
					#region Prepare Current groups values
					var currentValues = new object[this.GroupColumns.Length];
					int groupIndex = 0;
					foreach (string group in this.GroupColumns)
					{
						string groupField = group;
                        if (groupField.StartsWithInvariant("DESC"))
                        {
                            if (!(masterSource.Columns[groupField] != null && masterSource.Columns[groupField.Substring(4)] == null))
                                groupField = groupField.Substring(4);
                        }
                        else if (groupField.StartsWithInvariant("NONE"))
                            groupField = groupField.Substring(4);

						currentValues[groupIndex++] = StiDataColumn.GetDataFromDataColumn(Dictionary, StiNameValidator.CorrectName(masterSource.Name, Dictionary.Report) + "." + groupField, false);
					}
					#endregion

					bool newGroup = false;
				
					if (firstRow)
                        InitTotals(totalsList);

					else
                        newGroup = Compare(prevValues, currentValues);
				
					if (newGroup)
					{					
						AddRow(table, totalsHash);
						InitTotals(totalsList);
						rowCount = 0;
					}
				
					prevValues = currentValues;
					firstRow = false;
				}
				#endregion
			
				DataRow row = null;
				//Create new row if result table does not contain totals or groups
				if (needAddRow)
                    row = table.NewRow();

				#region Fill columns from group
				try
				{
					foreach (string group in GroupColumns)
					{
						string groupField = group;
                        if (groupField.StartsWithInvariant("DESC"))
                        {
                            if (!(masterSource.Columns[groupField] != null && masterSource.Columns[groupField.Substring(4)] == null))
                                groupField = groupField.Substring(4);
                        }

                        else if (groupField.StartsWithInvariant("NONE"))
                            groupField = groupField.Substring(4);

						var rowObject = StiDataColumn.GetDataFromDataColumn(Dictionary, StiNameValidator.CorrectName(masterSource.Name, Dictionary.Report) + "." + groupField, false);
						if (rowObject == null)
                            rowObject = DBNull.Value;

                        if (needAddRow)
                        {
                            row[group] = rowObject;
                        }
                        else
                        {
                            var total = totalsHash[groupField] as StiAggregateFunctionService;
                            total?.CalcItem(rowObject);
                        }
					}
				}
				catch
				{
				}
				#endregion
				
				#region Fill columns from result
				int resultIndex = 0;
				while (resultIndex < results2.Length)
				{
					try
					{
						string column = results2[resultIndex++];
						string function = results2[resultIndex++];
						string name = results2[resultIndex++];

						object rowObject = DBNull.Value;
						if (!string.IsNullOrEmpty(column))
						{
							rowObject = StiDataColumn.GetDataFromDataColumn(Dictionary, StiNameValidator.CorrectName(masterSource.Name, Dictionary.Report) + "." + column, false);
							if (rowObject == null)
                                rowObject = DBNull.Value;
						}

                        if (needAddRow)
                        {
                            row[name] = rowObject;
                        }
                        else
                        {
                            var total = totalsHash[name] as StiAggregateFunctionService;
                            total?.CalcItem(rowObject);
                        }
					}
					catch
					{
					}
				}
				#endregion

				rowCount ++;				

				//Add created row to result table does not contain totals or groups
				if (needAddRow)
                    table.Rows.Add(row);

				//Move on next line
                masterSource.Next();

				Dictionary.Report.Line++;
				Dictionary.Report.LineThrough++;
			}

			if (rowCount > 0 && (totalsExist || groupExist))
				AddRow(table, totalsHash);
			#endregion

			base.DataTable = table;

            masterSource.ResetDetailsRows();
            masterSource.ResetData();
			masterSource.First();
		}


		private bool Compare(object[] values1, object[] values2)
		{
			int index = 0;
			foreach (object obj in values1)
			{
                if (obj == null)
                {
                    if (values2[index] == null)
                    {
                        index++;
                        continue;
                    }
                    else
                    {
                        return true;
                    }
                }

                if (!obj.Equals(values2[index++]))
                    return true;
			}
			return false;
		}

        private void InitTotals(List<StiAggregateFunctionService> totalsList)
		{
			foreach (var total in totalsList)
			{
				total.Init();
			}
		}


		private void AddRow(DataTable table, Hashtable totalsHash)
		{
			var row = table.NewRow();
			foreach (StiDataColumn column in this.Columns)
			{
				var total = totalsHash[column.Name] as StiAggregateFunctionService;
                if (total != null)
                {
                    var value = total.GetValue();
                    if (value == null)
                        value = DBNull.Value;

                    row[column.Name] = value;
                }
			}
			table.Rows.Add(row);
		}
        #endregion

        #region Methods.override
        public override StiDataSource CreateNew()
        {
            return new StiVirtualSource();
        }

        public override object Clone()
        {
            var dataSource = base.Clone() as StiVirtualSource;

            dataSource.Filters = Filters.Clone() as StiFiltersCollection;
            dataSource.GroupColumns = GroupColumns.Clone() as string[];
            dataSource.Results = Results.Clone() as string[];
            dataSource.Sort = Sort.Clone() as string[];

            return dataSource;
        }
        #endregion

        /// <summary>
		/// Creates a new object of the type StiVirtualSource.
		/// </summary>
		public StiVirtualSource()
		{
		}

		/// <summary>
		/// Creates a new object of the type StiVirtualSource.
		/// </summary>
		public StiVirtualSource(string nameInSource, string name) : base(nameInSource, name)
		{
			this.ConnectionOrder = (int)StiConnectionOrder.None;
		}

        /// <summary>
        /// Creates a new object of the type StiVirtualSource.
        /// </summary>
        public StiVirtualSource(string nameInSource, string name, string key) : base(nameInSource, name, name, key)
        {
            this.ConnectionOrder = (int)StiConnectionOrder.None;
        }
	}
}