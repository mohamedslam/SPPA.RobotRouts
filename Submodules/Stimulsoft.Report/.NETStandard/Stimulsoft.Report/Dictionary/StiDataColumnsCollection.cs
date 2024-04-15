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
using System.Data;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the column collection.
    /// </summary>
    public class StiDataColumnsCollection :
        CollectionBase,
        IComparer,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            var index = 0;
            foreach (StiDataColumn column in List)
            {
                jObject.AddPropertyJObject(index.ToString(), column.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                StiDataColumn column = null;

                var propJObject = (JObject)property.Value;
                var ident = propJObject.Properties().FirstOrDefault(x => x.Name == "Ident");

                if (ident != null && ident.Value.ToObject<string>() == "Calc")
                    column = new StiCalcDataColumn();

                else if (ident != null && ident.Value.ToObject<string>() == "Transform")
                    column = new StiDataTransformationColumn();

                else
                    column = new StiDataColumn();

                column.LoadFromJsonObject((JObject)property.Value);

                List.Add(column);
            }
        }
        #endregion

        #region IComparer
        private int directionFactor = 1;

        int IComparer.Compare(object x, object y)
        {
            var col1 = x as StiDataColumn;
            var col2 = y as StiDataColumn;

            if (StiOptions.Designer.SortDictionaryByAliases)
                return col1.Alias.CompareTo(col2.Alias) * directionFactor;
            else
                return col1.Name.CompareTo(col2.Name) * directionFactor;
        }
        #endregion

        #region Properties
        internal Hashtable CachedDataColumns { get; set; } = new Hashtable();

        internal StiBusinessObject BusinessObject { get; set; }

        internal StiDataSource DataSource { get; set; }
        #endregion

        #region Methods
        public List<StiDataColumn> ToList()
        {
            return this.Cast<StiDataColumn>().ToList();
        }

        protected override void OnInsert(int index, object value)
        {
            var dataColumn = value as StiDataColumn;
            if (dataColumn == null) return;

            if (dataColumn.DataSource == null)
                dataColumn.DataSource = DataSource;

            if (dataColumn.BusinessObject == null)
                dataColumn.BusinessObject = BusinessObject;

            if (dataColumn.DataColumnsCollection == null)
                dataColumn.DataColumnsCollection = this;
        }

        public void Add(string name, Type type)
        {
            Add(name, name, type);
        }

        public void Add(string name, string alias, Type type)
        {
            Add(new StiDataColumn(name, alias, type));
        }

        public void Add(StiDataColumn column)
        {
            List.Add(column);
        }

        public void AddRange(StiDataColumnsCollection columns)
        {
            foreach (StiDataColumn column in columns)
            {
                Add(column);
            }
        }

        public void AddRange(StiDataColumn[] columns)
        {
            foreach (var column in columns) Add(column);
        }

        public bool Contains(StiDataColumn column)
        {
            return List.Contains(column);
        }

        public bool Contains(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;

            name = StiNameValidator.CorrectName(name.ToLowerInvariant(), DataSource?.Dictionary?.Report);

            foreach (StiDataColumn column in List)
            {
                if (StiNameValidator.CorrectName(column.Name.ToLowerInvariant(), DataSource?.Dictionary?.Report) == name) return true;
            }
            return false;
        }

        public int IndexOf(StiDataColumn column)
        {
            return List.IndexOf(column);
        }

        public void Insert(int index, StiDataColumn column)
        {
            List.Insert(index, column);
        }

        public void Remove(StiDataColumn column)
        {
            List.Remove(column);

            var columnName = column.Name.ToLowerInvariant();

            if (CachedDataColumns.Contains(columnName))
                CachedDataColumns.Remove(columnName);
        }

        public StiDataColumn this[int index]
        {
            get
            {
                return (StiDataColumn)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public StiDataColumn this[string name]
        {
            get
            {
                name = StiNameValidator.CorrectName(name.ToLowerInvariant(), DataSource?.Dictionary?.Report);

                var searchedDataColumn = CachedDataColumns[name] as StiDataColumn;
                if (searchedDataColumn != null) return searchedDataColumn;

                foreach (StiDataColumn column in List)
                {
                    if (StiNameValidator.CorrectName(column.Name.ToLowerInvariant(), DataSource?.Dictionary?.Report) ==
                        name)
                    {
                        CachedDataColumns[name] = column;
                        return column;
                    }
                }
                return null;
            }
            set
            {
                name = StiNameValidator.CorrectName(name.ToLowerInvariant(), DataSource?.Dictionary?.Report);

                for (var index = 0; index < List.Count; index++)
                {
                    var column = List[index] as StiDataColumn;

                    if (StiNameValidator.CorrectName(column.Name.ToLowerInvariant(), DataSource?.Dictionary?.Report) ==
                        name)
                    {
                        List[index] = value;
                        return;
                    }
                }
                Add(value);
            }
        }
        #endregion

        #region Methods.Sort
        public void Sort(StiSortOrder order)
        {
            if (order == StiSortOrder.Asc) directionFactor = 1;
            else directionFactor = -1;

            base.InnerList.Sort(this);
        }
        #endregion

        public StiDataColumnsCollection()
        {
        }

        public StiDataColumnsCollection(List<StiDataColumn> columns)
        {
            columns.ForEach(this.Add);
        }

        public StiDataColumnsCollection(List<DataColumn> columns)
        {
            columns.Select(c => new StiDataColumn(c.ColumnName, c.DataType))
                .ToList()
                .ForEach(this.Add);
        }

        public StiDataColumnsCollection(StiDataSource dataSource)
        {
            this.DataSource = dataSource;
        }

        public StiDataColumnsCollection(StiBusinessObject source)
        {
            this.BusinessObject = source;
        }
    }
}
