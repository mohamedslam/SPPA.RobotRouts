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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Dictionary.Design;
using Stimulsoft.Report.Dashboard;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes a column with data.
    /// </summary>
    [TypeConverter(typeof(StiDataColumnConverter))]
    [StiSerializable]
    public class StiDataColumn :
        ICloneable,
        IStiName,
        IStiAppDataColumn,
        IStiAppAlias,
        IStiInherited,
        IStiPropertyGridObject,
        IStiJsonReportObject
    {
        #region enum Order
        public enum Order
        {
            NameInSource = 100,
            Name = 200,
            Alias = 300,
            Expression = 400,
            Type = 500
        }
        #endregion

        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            // StiDataColumn
            jObject.AddPropertyStringNullOrEmpty("Name", Name);
            jObject.AddPropertyInt("Index", Index);
            jObject.AddPropertyStringNullOrEmpty("NameInSource", NameInSource);
            jObject.AddPropertyStringNullOrEmpty("Alias", Alias);
            jObject.AddPropertyStringNullOrEmpty("Type", StiDataColumnConverter.ConvertTypeToString(Type));
            jObject.AddPropertyStringNullOrEmpty("Key", Key);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Name":
                        this.name = property.DeserializeString();
                        break;

                    case "Index":
                        this.Index = property.DeserializeInt();
                        break;

                    case "NameInSource":
                        this.NameInSource = property.DeserializeString();
                        break;

                    case "Alias":
                        this.Alias = property.DeserializeString();
                        break;

                    case "Type":
                        {
                            Type type = null;
                            var typeStr = property.DeserializeString();
                            try
                            {
                                type = StiTypeFinder.GetType(typeStr);
                            }
                            catch
                            {
                            }
                            this.Type = Stimulsoft.Report.Dictionary.Design.StiDataColumnConverter.CheckType(typeStr, type);
                        }
                        break;

                    case "Key":
                        this.Key = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public virtual StiComponentId ComponentId => StiComponentId.StiDataColumn;

        [Browsable(false)]
        public string PropName => this.name;

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Alias(),
                propHelper.Name(),
                propHelper.NameInSource(),
                propHelper.Type()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
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

        #region IStiAppDataColumn
        string IStiAppDataColumn.GetNameInSource()
        {
            return NameInSource;
        }

        /// <summary>
        /// Returns a type of the data column.
        /// </summary>
        /// <returns>The name of the data column.</returns>
        Type IStiAppDataColumn.GetDataType()
        {
            return Type;
        }
        #endregion

        #region IStiAppDataCell
        /// <summary>
        /// Returns a name of the data column.
        /// </summary>
        /// <returns>The name of the data column.</returns>
        string IStiAppDataCell.GetName()
        {
            return Name;
        }
        #endregion

        #region IStiAppAlias
        /// <summary>
        /// Returns a name of the data column.
        /// </summary>
        /// <returns>The name of the data column.</returns>
        string IStiAppAlias.GetAlias()
        {
            return Alias;
        }
        #endregion

        #region IStiAppCell
        string IStiAppCell.GetKey()
        {
            Key = StiKeyHelper.GetOrGeneratedKey(Key);

            return Key;
        }

        void IStiAppCell.SetKey(string key)
        {
            Key = key;
        }
        #endregion

        #region IStiName
        private string name;
        /// <summary>
        /// Gets or sets a column name.
        /// </summary>
        [DefaultValue("Column")]
        [RefreshProperties(RefreshProperties.All)]
        [StiCategory("Data")]
        [Description("Gets or sets a column name.")]
        [ParenthesizePropertyName(true)]
        [StiOrder((int)Order.Name)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (DataSource?.Dictionary?.Report != null &&
                    DataSource.Dictionary.Report.IsDesigning)
                {
                    if (StiOptions.Designer.AutoCorrectDataColumnName)
                        value = StiNameValidator.CorrectName(value, DataSource.Dictionary.Report);
                }

                if (name == value) return;

                if (name == NameInSource) 
                    NameInSource = value;

                if (name == Alias) 
                    Alias = value;

                name = value;
            }
        }
        #endregion

        #region IStiInherited
        [Browsable(false)]
        [DefaultValue(false)]
        public bool Inherited
        {
            get
            {
                return DataSource != null && DataSource.Inherited;
            }
            set
            {
            }
        }
        #endregion

        #region Properties
        internal StiDataColumnsCollection DataColumnsCollection { get; set; }

        /// <summary>
		/// Gets or sets the Data Source in what the column is described.
		/// </summary>
		[Browsable(false)]
        public StiDataSource DataSource { get; set; }

        /// <summary>
        /// Gets or sets the Business Object in what the column is described.
        /// </summary>
        [Browsable(false)]
        public StiBusinessObject BusinessObject { get; set; }

        /// <summary>
		/// Gets or sets the index of a column in the collection of columns.
		/// </summary>
		[Browsable(false)]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets a original name.
        /// </summary>
        [DefaultValue("Column")]
        [RefreshProperties(RefreshProperties.All)]
        [StiCategory("Data")]
        [Description("Gets or sets a column original name.")]
        [ParenthesizePropertyName(true)]
        [StiOrder((int)Order.NameInSource)]
        public virtual string NameInSource { get; set; } = "Column";

        /// <summary>
		/// Gets or sets an alias of column data.
		/// </summary>
		[DefaultValue("Column")]
        [StiCategory("Data")]
        [Description("Gets or sets an alias of column data.")]
        [ParenthesizePropertyName(true)]
        [StiOrder((int)Order.Alias)]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the type of column data.
        /// </summary>
        [DefaultValue(typeof(object))]
        [TypeConverter(typeof(Stimulsoft.Report.Dictionary.Design.StiTypeConverter))]
        [Editor("Stimulsoft.Base.Design.StiTypeEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [Description("Gets or sets the type of column data.")]
        [StiOrder((int)Order.Type)]
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the key of the dictionary object.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [Browsable(false)]
        public string Key { get; set; }
        #endregion

        #region Methods
        public string GetColumnPath()
        {
            return $"{DataSource.Name}.{Name}";
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool onlyAlias)
        {
            if (onlyAlias && !string.IsNullOrWhiteSpace(Alias)) 
                return Alias;

            if (Name == Alias || string.IsNullOrWhiteSpace(Alias)) 
                return Name;

            return $"{Name} [{Alias}]";
        }

        /// <summary>
        /// Returns StiDataColumn from full name of column.
        /// </summary>
        /// <param name="dictionary">Dictionary which contain Data Source.</param>
        /// <param name="column">Full name of column.</param>
        /// <returns>Returned object.</returns>
        public static StiDataColumn GetDataColumnFromColumnName(StiDictionary dictionary, string column, bool allowRelationName = false)
        {
            try
            {
                if (column == null) 
                    return null;

                column = column.Trim();
                
                if (column.Length == 0) 
                    return null;

                var strs = column.Split('.');
                var dataSource = dictionary.DataSources[strs[0]];

                if (dataSource == null)
                    return null;

                var qnt = strs.Length - 1;
                var index = 1;

                while (qnt > 0)
                {
                    if (qnt >= 2)
                    {
                        dataSource = dataSource.GetParentDataSource(strs[index], allowRelationName);
                        
                        if (dataSource == null) 
                            return null;

                        index++;
                        qnt--;
                    }
                    else
                    {                        
                        if (dataSource.Columns.Contains(strs[index]))
                            return dataSource.Columns[strs[index]];
                        else
                            return null;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        public static object GetDataFromDataColumn(StiDictionary dictionary, string column)
        {
            return GetDataFromDataColumn(dictionary, column, true);
        }

        public static string GetRelationName(StiDictionary dictionary, StiDataSource dataSource, string relationName)
        {
            foreach (StiDataRelation relation in dictionary.Relations)
            {
                if (relation.ChildSource == dataSource && relation.Name == relationName)
                    return relation.NameInSource;
            }
            return relationName;
        }

        public static object GetDataFromBusinessObject(StiDictionary dictionary, string column)
        {
            var strs = column.Split('.');

            var businessObject = dictionary.BusinessObjects[strs[0]];
            if (businessObject == null) 
                return null;

            var index = 1;
            while (index < strs.Length)
            {
                if (index == strs.Length - 1)
                    return businessObject[strs[index]];

                businessObject = businessObject.BusinessObjects[strs[index]];
                index++;
            }
            return null;
        }

        public static StiBusinessObject GetBusinessObjectFromDataColumn(StiDictionary dictionary, string column)
        {
            var strs = column.Split('.');

            var businessObject = dictionary.BusinessObjects[strs[0]];
            if (businessObject == null) 
                return null;

            var index = 1;
            while (index < strs.Length)
            {
                if (index == strs.Length - 1)
                    return businessObject;

                businessObject = businessObject.BusinessObjects[strs[index]];
                index++;
            }
            return null;
        }

        /// <summary>
        /// Returns object from the Data Source with full name of column.
        /// </summary>
        /// <param name="dictionary">Dictionary which contain Data Source.</param>
        /// <param name="column">Full name of column.</param>
        /// <returns>Returned object.</returns>
        public static object GetDataFromDataColumn(StiDictionary dictionary, string column, bool useRelationName)
        {
            try
            {
                if (column == null) 
                    return null;

                var strs = column.Split('.');
                var dataSource = dictionary.DataSources[strs[0]];

                if (dataSource == null)
                    return GetDataFromBusinessObject(dictionary, column);

                StiDataRow row = null;

                var qnt = strs.Length - 1;
                var index = 1;

                while (qnt > 0)
                {
                    if (qnt >= 2)
                    {
                        var relationName = strs[index];
                        if (!useRelationName)
                            relationName = GetRelationName(dictionary, dataSource, relationName);

                        row = row == null
                            ? dataSource.GetParentData(relationName)
                            : row.GetParentData(relationName);

                        dataSource = dataSource.GetParentDataSource(relationName);

                        index++;
                        qnt--;
                    }
                    else
                    {
                        if (dataSource != null)
                        {
                            var calcDataColumn = dataSource.Columns[strs[index]] as StiCalcDataColumn;
                            if (calcDataColumn != null)
                            {
                                if (dictionary.Report != null && dictionary.Report.IsDesigning)
                                    return null;

                                var isInterpretaMode = dictionary.Report != null 
                                    && dictionary.Report.CalculationMode == StiCalculationMode.Interpretation;

                                if (isInterpretaMode)
                                {
                                    var tempText = new StiText
                                    {
                                        Name = "**StiCalcDataColumn**",
                                        Page = dictionary.Report.Pages[0]
                                    };
                                    return Engine.StiParser.ParseTextValue("{" + calcDataColumn.Expression + "}", tempText);
                                }
                                else
                                {
                                    var name = string.Format("Get{0}_{1}",
                                        StiNameValidator.CorrectName(calcDataColumn.DataSource.Name, dictionary.Report),
                                        StiNameValidator.CorrectName(calcDataColumn.name, dictionary.Report));

                                    var method = dictionary.Report.GetType().GetMethod(name, new Type[0]);
                                    if (method != null)
                                        return method.Invoke(dictionary.Report, new object[0]);
                                }
                            }
                        }

                        if (row != null)
                            return row[strs[index]];

                        return dataSource == null ? null : dataSource[strs[index]];
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Returns Data Source from the full name of column.
        /// </summary>
        /// <param name="dictionary">Dictionary which contain Data Source.</param>
        /// <param name="column">Full name of column.</param>
        /// <returns>Data Source.</returns>
        public static StiDataSource GetDataSourceFromDataColumn(StiDictionary dictionary, string column)
        {
            try
            {
                var strs = column.Split('.');
                return dictionary.DataSources[strs[0]];
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Returns column name from the full name of column.
        /// </summary>
        /// <param name="dictionary">Dictionary which contain Data Source.</param>
        /// <param name="column">Full name of column.</param>
        /// <returns>Column name.</returns>
        public static string GetColumnNameFromDataColumn(StiDictionary dictionary, string column)
        {
            try
            {
                var strs = column.Split('.');
                return column.Substring(strs[0].Length + 1);
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Returns list of object from the Data Source with specified DataColumn.
        /// </summary>
        /// <param name="dictionary">Dictionary which contain Data Source.</param>
        /// <param name="column">Full name of column.</param>
        /// <returns>Returned list of object.</returns>
        public static List<object> GetDataListFromDataColumn(StiDictionary dictionary, string column, int? maxRows = null, bool firstPositionInDataSource = false)
        {
            GetDataSourceFromDataColumn(dictionary, column)?.First();
            return GetDatasFromDataColumn(dictionary, column, maxRows, firstPositionInDataSource)?.ToList();
        }

        /// <summary>
        /// Returns array of object from the Data Source with specified DataColumn.
        /// </summary>
        /// <param name="dictionary">Dictionary which contain Data Source.</param>
        /// <param name="column">Full name of column.</param>
        /// <returns>Returned array of object.</returns>
        public static object[] GetDatasFromDataColumn(StiDictionary dictionary, string column, int? maxRows = null, bool firstPositionInDataSource = false, 
            bool useRelationName = true)
        {
            try
            {
                var items = new List<object>();

                #region DataSource
                var dataSource = GetDataSourceFromDataColumn(dictionary, column);
                if (dataSource != null)
                {
                    if (firstPositionInDataSource)
                        dataSource.First();

                    var rowIndex = 0;
                    while (!dataSource.IsEof)
                    {
                        var rowObject = GetDataFromDataColumn(dictionary, column, useRelationName);

                        if (rowObject != null) 
                            items.Add(rowObject);

                        dataSource.Next();

                        rowIndex++;
                        if (maxRows != null && maxRows == rowIndex) break;
                    }
                    dataSource.First();
                }
                #endregion

                #region BusinessObject
                var businessObject = GetBusinessObjectFromDataColumn(dictionary, column);
                if (businessObject != null)
                {
                    businessObject.SaveState("Totals");
                    businessObject.CreateEnumerator();

                    var rowIndex = 0;
                    while (!businessObject.IsEof)
                    {
                        var rowObject = GetDataFromDataColumn(dictionary, column);
                        if (rowObject != null) 
                            items.Add(rowObject);

                        businessObject.Next();

                        rowIndex++;
                        if (maxRows != null && maxRows == rowIndex) break;
                    }
                    businessObject.RestoreState("Totals");
                }
                #endregion

                return items.ToArray();
            }
            catch
            {
            }
            return new object[0];
        }

        /// <summary>
        /// Returns array of objects from the Data Source or Business Object with specified expression.
        /// </summary>
        public static object[] GetDatasFromDataSourceWithExpression(object data, string expression, 
            int? maxRows = null, bool firstPositionInDataSource = false)
        {
            try
            {
                var items = new List<object>();

                #region DataSource
                var dataSource = data as StiDataSource;
                if (dataSource != null)
                {
                    dataSource.SaveState("ExpressionCalculation");
                    if (firstPositionInDataSource)
                        dataSource.First();

                    var rowIndex = 0;
                    while (!dataSource.IsEof)
                    {
                        var rowObject = StiReportParser.Parse(expression, dataSource.Dictionary.Report, false, null, false, true);
                        if (rowObject != null)
                            items.Add(rowObject);

                        dataSource.Next();

                        rowIndex++;
                        if (maxRows != null && maxRows == rowIndex) break;
                    }
                    dataSource.First();
                    dataSource.RestoreState("ExpressionCalculation");
                }
                #endregion

                #region BusinessObject
                var businessObject = data as StiBusinessObject;
                if (businessObject != null)
                {
                    businessObject.SaveState("ExpressionCalculation");
                    businessObject.CreateEnumerator();

                    var rowIndex = 0;
                    while (!businessObject.IsEof)
                    {
                        var rowObject = StiReportParser.Parse(expression, dataSource.Dictionary.Report as IStiReportComponent, false);
                        if (rowObject != null)
                            items.Add(rowObject);

                        businessObject.Next();

                        rowIndex++;
                        if (maxRows != null && maxRows == rowIndex) break;
                    }
                    businessObject.RestoreState("ExpressionCalculation");
                }
                #endregion

                return items.ToArray();
            }
            catch
            {
            }
            return new object[0];
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiDataColumn.
        /// </summary>
        public StiDataColumn() : this("Column")
        {
        }

        /// <summary>
        /// Creates a new object of the type StiDataColumn.
        /// </summary>
        /// <param name="name">Name of column.</param>
        public StiDataColumn(string name) : this(name, name, typeof(string))
        {
        }

        /// <summary>
        /// Creates a new object of the type StiDataColumn.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <param name="type">Type of data of column.</param>
        public StiDataColumn(string name, Type type) : this(name, name, type)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiDataColumn.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <param name="alias">Alias of column.</param>
        /// <param name="type">Type of data of column.</param>
        public StiDataColumn(string name, string alias, Type type) :
            this(name, name, alias, type)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiDataColumn.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <param name="alias">Alias of column.</param>
        /// <param name="type">Type of data of column.</param>
        public StiDataColumn(string nameInSource, string name, string alias, Type type)
        {
            this.NameInSource = nameInSource;
            this.name = name;
            this.Alias = alias;
            this.Type = type;
            this.Index = -1;
        }

        /// <summary>
        /// Creates a new object of the type StiDataColumn.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <param name="alias">Alias of column.</param>
        /// <param name="type">Type of data of column.</param>
        /// <param name="key">Key string.</param>
        public StiDataColumn(string nameInSource, string name, string alias, Type type, string key)
        {
            this.NameInSource = nameInSource;
            this.name = name;
            this.Alias = alias;
            this.Type = type;
            this.Index = -1;
            this.Key = key;
        }
    }
}