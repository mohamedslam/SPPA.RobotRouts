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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.CrossTab.Core;
using Stimulsoft.Report.Dictionary.Design;
using Stimulsoft.Report.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiCrossTabDataSourceConverter))]
    public class StiCrossTabDataSource : StiDataStoreSource, 
		IStiFilter
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiCrossTabDataSource
            jObject.AddPropertyEnum("FilterMode", FilterMode, StiFilterMode.And);
            jObject.AddPropertyJObject("Filters", Filters.SaveToJsonObject(mode));

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
                }
            }
        }
        #endregion

        #region IStiFilter Members
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
        protected override Type GetDataAdapterType() => typeof(StiCrossTabAdapterService);
        #endregion

        #region Fields
        private SortedDictionary<int, List<object>> sum = null;
        private List<string> summaryColumn = null;
        private Hashtable columns = null;
        private Hashtable summaryType = null;
        private StiCrossTab crossTab;
        #endregion

        #region Methods
        public void ConnectToData(DataTable crossDataTable, StiCrossTab crossT)
        {
            if (crossT == null)
                return;

            var table = new DataTable();
            crossTab = crossT;
            columns = new Hashtable();
            summaryColumn = new List<string>();
            summaryType = new Hashtable();

            #region Create Columns for table
            foreach (StiComponent component in crossTab.Components)
            {
                if (component is StiCrossCell || component is StiCrossTitle || component is StiCrossSummaryHeader)
                {
                    var row = component as StiCrossRow;
                    var col = component as StiCrossColumn;
                    var sum = component as StiCrossSummary;

                    if (row != null)
                    {
                        columns.Add(row.Alias, row.Name);
                        var column = new DataColumn(row.Alias);
                        table.Columns.Add(column);
                        continue;
                    }
                    if (col != null)
                    {
                        columns.Add(col.Alias, col.Name);
                        var column = new DataColumn(col.Alias);
                        table.Columns.Add(column);
                        continue;
                    }
                    if (sum != null)
                    {
                        columns.Add(sum.Alias, sum.Name);
                        summaryColumn.Add(sum.Name);
                        summaryType.Add(summaryType.Count, sum.Summary);
                        var stiColumn = crossTab.DataSource.Columns[sum.Alias];
                        DataColumn column;
                        column = new DataColumn(sum.Alias, typeof(decimal));
                        table.Columns.Add(column);
                        continue;
                    }
                }
            }
            #endregion

            #region copy rows in table
            foreach (DataRow row in crossDataTable.Rows)
            {
                DataRow newRow = table.NewRow();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    newRow[table.Columns[i].Caption] = row[(string)(columns[table.Columns[i].Caption])];
                }
                table.Rows.Add(newRow);
            }
            #endregion

            CalculationTable(table);
        }

        public void BuildColumnsForDictionaryPanel(StiCrossTab cT)
        {
            this.Columns.Clear();

            if (cT != null && this.NameInSource != string.Empty)
            {
                foreach (StiComponent component in cT.Components)
                {
                    if (component is StiCrossCell || component is StiCrossTitle || component is StiCrossSummaryHeader)
                    {
                        var row = component as StiCrossRow;
                        var col = component as StiCrossColumn;
                        var sum = component as StiCrossSummary;

                        if (row != null)
                        {
                            var stiColumn = new StiDataColumn(row.Alias, FildType(cT.DataSource, cT.BusinessObject, row.Alias) ?? typeof(object));
                            this.Columns.Add(stiColumn);
                            continue;
                        }
                        if (col != null)
                        {
                            var stiColumn = new StiDataColumn(col.Alias, FildType(cT.DataSource, cT.BusinessObject, col.Alias) ?? typeof(object));
                            this.Columns.Add(stiColumn);
                            continue;
                        }
                        if (sum != null)
                        {
                            var stiColumn = new StiDataColumn(sum.Alias, FildType(cT.DataSource, cT.BusinessObject, sum.Alias) ?? typeof(string));
                            stiColumn.Type = typeof(decimal);
                            this.Columns.Add(stiColumn);
                            continue;
                        }
                    }
                }
            }
        }

        private Type FildType(StiDataSource dataSource, StiBusinessObject businessObject, string colName)
        {
            Type result = null;

            if (dataSource != null)
                result = dataSource.Columns[colName]?.Type;
            if (businessObject != null)
                result = businessObject.Columns[colName]?.Type;

            if (result == null)
            {
                foreach (StiDataRelation rel in dataSource.Dictionary.Relations)
                {
                    if (rel.ChildSource == dataSource && rel?.ParentSource.Columns[colName] != null)
                    {
                        result = rel?.ParentSource.Columns[colName].Type;
                    }
                }
            }
            return result;
        }

        private void CalculationTable(DataTable table)
        {
            var tempTable = table.Clone();
            sum = new SortedDictionary<int, List<object>>();
            int key = 0;

            #region Summation of identical lines
            int count = columns.Count - summaryColumn.Count;
            foreach (DataRow row in table.Rows)
            {
                bool add = false;
                if (summaryColumn.Count > 0)
                    for (int i = 0; i < tempTable.Rows.Count; i++)
                    {
                        bool state = true;
                        DataRow tempRow = tempTable.Rows[i];
                        for (int j = 0; j < count; j++)
                        {
                            if (tempRow[j].ToString() != row[j].ToString())
                            {
                                state = false;
                                break;
                            }
                        }
                        if (state)
                        {
                            CalculationRows(tempRow, row);
                            add = true;
                            break;
                        }
                    }
                if (!add)
                {
                    tempTable.Rows.Add(CopyRow(row, tempTable, count, ref key));
                }
            }
            #endregion

            base.DataTable = CalculationSummaryColunms(tempTable);
        }

        private void CalculationRows(DataRow newRow, DataRow row)
        {
            int count = columns.Count - summaryColumn.Count;
            for (int i = count; i < columns.Count; i++)
            {
                int index = Convert.ToInt32(newRow[i]);
                sum[index].Add(row[i].ToString());
            }
        }
        
        private DataRow CopyRow(DataRow row, DataTable tempTable, int countColunmsNotSummary, ref int key)
        {
            var newRow = tempTable.NewRow();
            for (int i = 0; i < tempTable.Columns.Count; i++)
            {
                if (i >= countColunmsNotSummary)
                {
                    newRow[i] = key;
                    sum.Add(key, new List<object>());
                    sum[key].Add(row[i]);
                    key++;
                }
                else
                newRow[i] = row[i];
            }

            return newRow;
        }

        private DataTable CalculationSummaryColunms(DataTable tempTable)
        {
            int indexColunm = columns.Count - summaryColumn.Count;

            foreach (DataRow row in tempTable.Rows)
            {
                int nomer = 0;
                for (int i = indexColunm; i < columns.Count; i++)
                {
                    switch ((StiSummaryType)summaryType[nomer])
                    {
                        case StiSummaryType.Sum:
                            row[i] = Sum(Convert.ToInt32(row[i]));
                            break;
                        case StiSummaryType.Average:
                            row[i] = Average(Convert.ToInt32(row[i]));
                            break;
                        case StiSummaryType.Min:
                            row[i] = Min(Convert.ToInt32(row[i]));
                            break;
                        case StiSummaryType.Max:
                            row[i] = Max(Convert.ToInt32(row[i]));
                            break;
                        case StiSummaryType.Count:
                            row[i] = SummaryCount(Convert.ToInt32(row[i]));
                            break;
                        case StiSummaryType.CountDistinct:
                            row[i] = CountDistinct(Convert.ToInt32(row[i]));
                            break;
                        case StiSummaryType.None:
                            row[i] = DBNull.Value;
                            break;
                        case StiSummaryType.Image:
                            row[i] = DBNull.Value;
                            break;
                        default: break;
                    } 
                    nomer++;
                }
            }
            return tempTable;
        }

        #region Methods.Calculation
        private decimal Sum(int nomer)
        {
            var list = sum[nomer];
            decimal summ = 0;
            for (int i = 0; i < list.Count; i++)
            {
                summ += Convert.ToDecimal(list[i]);
            }

            return Math.Round(summ, 2);
        }

        private decimal Average(int nomer)
        {
            var list = sum[nomer];
            if (list.Count == 0)
                return 0;

            decimal average = 0;
            for (int i = 0; i < list.Count; i++)
            {
                average += Convert.ToDecimal(list[i]);
            }

            return Math.Round(average / list.Count, 5);
        }

        private decimal Min(int nomer)
        {
            var list = sum[nomer];
            decimal min = 0;
            if (list.Count > 0)
                min = Convert.ToDecimal(list[0]);
            else
                return 0;

            for (int i = 1; i < list.Count; i++)
            {
                if (min > Convert.ToDecimal(list[i]))
                    min = Convert.ToDecimal(list[i]);
            }

            return Math.Round(min, 2);
        }

        private decimal Max(int nomer)
        {
            var list = sum[nomer];
            decimal min = 0;
            if (list.Count > 0)
                min = Convert.ToDecimal(list[0]);
            else
                return 0;

            for (int i = 1; i < list.Count; i++)
            {
                if (min < Convert.ToDecimal(list[i]))
                    min = Convert.ToDecimal(list[i]);
            }

            return Math.Round(min, 2);
        }

        private decimal SummaryCount(int nomer)
        {
            return sum[nomer].Count;
        }

        private decimal CountDistinct(int nomer)
        {
            var list = sum[nomer];
            var newList = new List<object>();
            for (int i = 0; i < list.Count; i++)
            {
                if (!newList.Contains(list[i]))
                    newList.Add(list[i]);
            }
            return newList.Count;
        }
        #endregion

        #endregion

        #region Methods.override
        public override StiComponentId ComponentId => StiComponentId.StiCrossTabDataSource;

        public override StiDataSource CreateNew() => new StiCrossTabDataSource();
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new object of the type SticRossTabDataSource.
        /// </summary>
        public StiCrossTabDataSource()
        {
            this.ConnectionOrder = (int)StiConnectionOrder.None;
        }

        /// <summary>
        /// Creates a new object of the type SticRossTabDataSource.
        /// </summary>
        public StiCrossTabDataSource(string nameInSource, string name)
            : base(nameInSource, name)
        {
            this.ConnectionOrder = (int)StiConnectionOrder.None;
        }

        /// <summary>
        /// Creates a new object of the type SticRossTabDataSource.
        /// </summary>
        public StiCrossTabDataSource(string nameInSource, string name, string key)
            : base(nameInSource, name, name, key)
        {
            this.ConnectionOrder = (int)StiConnectionOrder.None;
        }
        #endregion
    }
}