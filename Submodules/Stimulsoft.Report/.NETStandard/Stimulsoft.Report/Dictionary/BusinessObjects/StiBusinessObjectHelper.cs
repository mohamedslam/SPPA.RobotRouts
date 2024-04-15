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
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Design;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Chart;
using System.Drawing;
using System.Drawing.Imaging;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Class which helps work with business objects.
    /// </summary>
    public static class StiBusinessObjectHelper
    {
        #region Properties.Static
        public static StiPropertiesProcessingType PropertiesProcessingType
        {
            get
            {
                return StiOptions.Dictionary.BusinessObjects.PropertiesProcessingType;
            }
            set
            {
                StiOptions.Dictionary.BusinessObjects.PropertiesProcessingType = value;
            }
        }


        public static StiFieldsProcessingType FieldsProcessingType
        {
            get
            {
                return StiOptions.Dictionary.BusinessObjects.FieldsProcessingType;
            }
            set
            {
                StiOptions.Dictionary.BusinessObjects.FieldsProcessingType = value;
            }
        }
        #endregion

        #region Methods.GetElement
        public static object GetElementFromEnumerable(IEnumerable enumerable)
        {
            try
            {
                IEnumerator enumerator = enumerable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null) return enumerator.Current;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static Type GetElementType(Type arrayType)
        {
            if (arrayType != null)
            {
                //GenericArguments нужен для List<>, чтобы получить тип элемента, но не для шаблонов (for example, class<class2>)
                //к сожалению, List<> можно определить пока только по названию
                //то же и для "WrappedEntityCollection"
                if (arrayType.IsGenericType &&
                    (arrayType.FullName.StartsWith("System.Collections.Generic.List") ||
                     arrayType.FullName.StartsWith("System.Collections.Generic.IList") ||
                     arrayType.FullName.StartsWith("System.Collections.Generic.ICollection") ||
                     arrayType.FullName.IndexOf("WrappedEntityCollection`1") != -1))
                {
                    try
                    {
                        Type[] types = arrayType.GetGenericArguments();
                        if (types.Length > 1)
                        {
                            return typeof(object);
                        }
                        else if (types.Length == 1)
                        {
                            return types[0];
                        }
                    }
                    catch
                    {
                    }
                }

                if (arrayType.GetElementType() != null) return arrayType.GetElementType();

                //MethodInfo[] methods = arrayType.GetMethods();
                //foreach (MethodInfo methodInfo in methods)
                //{
                //    if (methodInfo.Name == "get_Item")
                //    {
                //        return methodInfo.ReturnType;
                //    }
                //}

                string typeName = arrayType.FullName;
                if (typeName.Length > 2 && typeName[typeName.Length - 2] == '`')
                    return Stimulsoft.Base.StiTypeFinder.GetType(typeName.Substring(0, typeName.Length - 2));
                else
                    return Stimulsoft.Base.StiTypeFinder.GetType(typeName);
            }
            return null;
        }

        private static object GetElement(object value)
        {
            if (value is IEnumerable)
            {
                object item = GetElementFromEnumerable(value as IEnumerable);
                if (item != null)
                    return item;
            }
            if (value == null)
                return null;
            if (value is Type) return GetElementType(value as Type);
            Type type = value.GetType();
            return GetElementType(type);
        }
        #endregion

        #region Methods.IsAllow
        internal static string GetAlias(PropertyInfo valueProp)
        {
            object[] attrs = valueProp.GetCustomAttributes(typeof(StiAliasAttribute), true);
            if (attrs != null && attrs.Length > 0) return ((StiAliasAttribute) attrs[0]).Alias;

            attrs = valueProp.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (attrs != null && attrs.Length > 0) return ((DisplayNameAttribute) attrs[0]).DisplayName;

            return null;
        }

        internal static string GetAlias(FieldInfo valueProp)
        {
            object[] attrs = valueProp.GetCustomAttributes(typeof(StiAliasAttribute), true);
            if (attrs != null && attrs.Length > 0) return ((StiAliasAttribute) attrs[0]).Alias;

            return null;
        }

        internal static string GetAlias(PropertyDescriptor valueProp)
        {
            Attribute attr = valueProp.Attributes[typeof(StiAliasAttribute)];
            if (attr != null) return ((StiAliasAttribute) attr).Alias;

            return null;
        }

        internal static bool IsAllowUseProperty(PropertyDescriptor valueProp)
        {
            Attribute attr = valueProp.Attributes[typeof(StiBrowsableAttribute)];
            if (attr != null) return ((StiBrowsableAttribute) attr).Browsable;

            attr = valueProp.Attributes[typeof(BrowsableAttribute)];
            if (attr != null && (!((BrowsableAttribute) attr).Browsable) &&
                PropertiesProcessingType == StiPropertiesProcessingType.Browsable) return false;

            return true;
        }

        internal static bool IsAllowUseProperty(PropertyInfo valueProp)
        {
            object[] attrs = valueProp.GetCustomAttributes(typeof(StiBrowsableAttribute), true);
            if (attrs != null && attrs.Length > 0) return ((StiBrowsableAttribute) attrs[0]).Browsable;

            attrs = valueProp.GetCustomAttributes(typeof(BrowsableAttribute), true);
            if (attrs != null && attrs.Length > 0 && (!((BrowsableAttribute) attrs[0]).Browsable) &&
                PropertiesProcessingType == StiPropertiesProcessingType.Browsable) return false;

            return true;
        }

        internal static bool IsAllowUseField(FieldInfo valueField)
        {
            object[] attrs = valueField.GetCustomAttributes(typeof(StiBrowsableAttribute), true);
            if (attrs != null && attrs.Length > 0) return ((StiBrowsableAttribute) attrs[0]).Browsable;

            attrs = valueField.FieldType.GetCustomAttributes(typeof(BrowsableAttribute), true);
            if (attrs != null && attrs.Length > 0 && (!((BrowsableAttribute) attrs[0]).Browsable) &&
                PropertiesProcessingType == StiPropertiesProcessingType.Browsable) return false;

            return true;
        }
        #endregion

        #region Methods.GetType
        private static Type GetType(Type type)
        {
            return type;
        }
        #endregion

        #region Methods.GetDataColumn
        private static StiDataColumn GetDataColumn(string name, Type type)
        {
            return GetDataColumn(name, name, type);
        }

        private static StiDataColumn GetDataColumn(string name, string alias, Type type)
        {
            if (alias == null) alias = name;
            return new StiDataColumn(name, alias, GetType(type));
        }

        private static StiDataColumn GetDataColumn(DataColumn dataColumn)
        {
            return new StiDataColumn(dataColumn.ColumnName, dataColumn.Caption, dataColumn.DataType);
        }
        #endregion

        #region Methods.GetColumns
        private static StiDataColumnsCollection GetColumnsFromObject(object value)
        {
            if (value is System.Windows.Forms.BindingSource)
                value = ((System.Windows.Forms.BindingSource) value).DataSource;

            if (value is DataSet)
                return GetColumnsFromDataSet(value as DataSet);

            if (value is DataTable)
                return GetColumnsFromDataTable(value as DataTable);

            if (value is ITypedList)
                return GetColumnsFromITypedList(value as ITypedList);

            if (value is ICustomTypeDescriptor)
                return GetColumnsFromICustomTypeDescriptor(value as ICustomTypeDescriptor);

            StiDataColumnsCollection columns = new StiDataColumnsCollection();
            if (value == null)
                return columns;

            object item = GetElement(value);
            if (item == null)
                item = value;

            if (item is Type)
            {
                Type type = item as Type;
                if (type == typeof(string))
                    return GetColumnsFromString();

                if (type == typeof(Image))
                    return GetColumnsFromImage();

                if (type == typeof(DictionaryEntry))
                    return GetColumnsFromDictionaryEntry();

                if ((item as Type).IsValueType)
                    return GetColumnsFromValueType(item);

                return GetColumnsFromClass(item);
            }
            else
            {
                if (item is ITypedList)
                    return GetColumnsFromITypedList(item as ITypedList);

                if (item is ICustomTypeDescriptor)
                    return GetColumnsFromICustomTypeDescriptor(item as ICustomTypeDescriptor);

                if (item is string)
                    return GetColumnsFromString();

                if (item is Image)
                    return GetColumnsFromImage();

                if (item is DictionaryEntry)
                    return GetColumnsFromDictionaryEntry();

                Type itemType = item.GetType();

                if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                    return GetColumnsFromKeyValuePair(item);

                if (itemType.IsValueType)
                    return GetColumnsFromValueType(item);

                return GetColumnsFromClass(item);
            }
        }

        private static StiDataColumnsCollection GetColumnsFromITypedList(ITypedList typedList)
        {
            StiDataColumnsCollection columns = new StiDataColumnsCollection();

            PropertyDescriptorCollection props = typedList.GetItemProperties(null);
            foreach (PropertyDescriptor prop in props)
            {
                if (!IsAllowUseProperty(prop)) continue;

                StiDataColumn column = GetDataColumn(prop.Name, GetAlias(prop), prop.PropertyType);
                columns.Add(column);
            }
            return columns;
        }

        private static StiDataColumnsCollection GetColumnsFromICustomTypeDescriptor(
            ICustomTypeDescriptor customTypeDescriptor)
        {
            StiDataColumnsCollection columns = new StiDataColumnsCollection();

            PropertyDescriptorCollection props = customTypeDescriptor.GetProperties();
            foreach (PropertyDescriptor prop in props)
            {
                if (!IsAllowUseProperty(prop)) continue;

                StiDataColumn column = GetDataColumn(prop.Name, GetAlias(prop), prop.PropertyType);
                columns.Add(column);
            }
            return columns;
        }

        private static StiDataColumnsCollection GetColumnsFromString()
        {
            StiDataColumnsCollection columns = new StiDataColumnsCollection();

            StiDataColumn column = GetDataColumn("Data", "Data", typeof(string));
            columns.Add(column);

            return columns;
        }

        private static StiDataColumnsCollection GetColumnsFromImage()
        {
            StiDataColumnsCollection columns = new StiDataColumnsCollection();

            StiDataColumn column = GetDataColumn("Image", "Image", typeof(Image));
            columns.Add(column);

            return columns;
        }

        private static StiDataColumnsCollection GetColumnsFromDictionaryEntry()
        {
            StiDataColumnsCollection columns = new StiDataColumnsCollection();

            StiDataColumn column = GetDataColumn("Key", "Key", typeof(object));
            columns.Add(column);

            column = GetDataColumn("Value", "Value", typeof(object));
            columns.Add(column);

            return columns;
        }

        private static StiDataColumnsCollection GetColumnsFromKeyValuePair(object item)
        {
            Type[] types = item.GetType().GetGenericArguments();

            StiDataColumnsCollection columns = new StiDataColumnsCollection();

            StiDataColumn column = GetDataColumn("Key", "Key", types[0]);
            columns.Add(column);

            column = GetDataColumn("Value", "Value", types[1]);
            columns.Add(column);

            return columns;
        }

        private static StiDataColumnsCollection GetColumnsFromValueType(object value)
        {
            StiDataColumnsCollection columns = new StiDataColumnsCollection();

            if (value is Type)
            {
                StiDataColumn column = GetDataColumn("Data", "Data", (Type) value);
                columns.Add(column);
            }
            else
            {
                StiDataColumn column = GetDataColumn("Data", "Data", value.GetType());
                columns.Add(column);
            }

            return columns;
        }

        private static StiDataColumnsCollection GetColumnsFromClass(object value)
        {
            StiDataColumnsCollection columns = new StiDataColumnsCollection();

            #region Properties
            if (StiOptions.Dictionary.BusinessObjects.AllowUseProperties)
            {
                PropertyInfo[] valueProps =
                    value is Type ? ((Type) value).GetProperties() : value.GetType().GetProperties();
                foreach (PropertyInfo valueProp in valueProps)
                {
                    if (!IsAllowUseProperty(valueProp)) continue;

                    if (valueProp.PropertyType.FullName == "System.Windows.Threading.Dispatcher" ||
                        valueProp.PropertyType.FullName == "System.Windows.DependencyObjectType") continue;

                    StiDataColumn dataColumn = GetDataColumn(valueProp.Name, GetAlias(valueProp),
                        GetType(valueProp.PropertyType));
                    columns.Add(dataColumn);
                }
            }
            #endregion

            #region Fields
            if (StiOptions.Dictionary.BusinessObjects.AllowUseFields)
            {
                FieldInfo[] valueFields = value is Type ? ((Type) value).GetFields() : value.GetType().GetFields();
                foreach (FieldInfo valueField in valueFields)
                {
                    if (!IsAllowUseField(valueField)) continue;

                    StiDataColumn dataColumn = GetDataColumn(valueField.Name, GetAlias(valueField),
                        GetType(valueField.FieldType));
                    columns.Add(dataColumn);
                }
            }
            #endregion

            return columns;
        }

        private static StiDataColumnsCollection GetColumnsFromDataTable(DataTable table)
        {
            StiDataColumnsCollection columns = new StiDataColumnsCollection();

            foreach (DataRelation relation in table.ChildRelations)
            {
                columns.Add(new StiDataColumn(relation.RelationName, typeof(DataTable)));
            }

            foreach (DataColumn column in table.Columns)
            {
                columns.Add(GetDataColumn(column));
            }

            return columns;
        }

        private static StiDataColumnsCollection GetColumnsFromDataSet(DataSet dataSet)
        {
            StiDataColumnsCollection columns = new StiDataColumnsCollection();

            foreach (DataTable table in dataSet.Tables)
            {
                columns.Add(new StiDataColumn(table.TableName, table.TableName, typeof(object)));
            }

            return columns;
        }

        public static StiDataColumnsCollection GetColumnsFromBusinessObjectData(StiBusinessObjectData data)
        {
            return GetColumnsFromBusinessObjectData(data, false);
        }

        public static StiDataColumnsCollection GetColumnsFromBusinessObjectData(StiBusinessObjectData data,
            bool includeChildDataSources)
        {
            return GetColumnsFromData(data.BusinessObjectValue, includeChildDataSources);
        }

        public static StiDataColumnsCollection GetColumnsFromData(object data)
        {
            return GetColumnsFromData(data, false);
        }

        public static StiDataColumnsCollection GetColumnsFromData(object data, bool includeChildDataSources)
        {
            StiDataColumnsCollection columns = GetColumnsFromObject(data);
            if (includeChildDataSources)
                return columns;

            StiDataColumnsCollection allowedColumns = new StiDataColumnsCollection();
            foreach (StiDataColumn dataColumn in columns)
            {
                if (!IsDataColumn(dataColumn.Type)) continue;
                allowedColumns.Add(dataColumn);
            }

            return allowedColumns;
        }

        public static StiDataColumnsCollection GetColumnsFromBusinessObject(StiBusinessObject data)
        {
            return GetColumnsFromBusinessObject(data, false);
        }

        public static StiDataColumnsCollection GetColumnsFromBusinessObject(StiBusinessObject data,
            bool includeChildDataSources)
        {
            StiDataColumnsCollection columns = GetColumnsFromObject(data.BusinessObjectValue);

            StiDataColumnsCollection allowedColumns = new StiDataColumnsCollection();
            foreach (StiDataColumn dataColumn in columns)
            {
                if (!IsDataColumn(dataColumn.Type)) continue;
                allowedColumns.Add(dataColumn);
            }

            return allowedColumns;
        }
        #endregion

        #region Methods.GetRows
        public static IEnumerator GetEnumeratorFromObject(object value)
        {
            if (value is System.Windows.Forms.BindingSource)
                value = ((System.Windows.Forms.BindingSource) value).DataSource;

            if (value == null)
                return null;

            if (value is DataSet)
                return GetEnumeratorFromDataSet(value as DataSet);

            if (value is DataTable)
                return GetEnumeratorFromDataTable(value as DataTable);

            if (value is IEnumerable)
                return GetEnumeratorFromIEnumerable(value as IEnumerable);

            if (value is IEnumerator)
                return GetEnumeratorFromIEnumerator(value as IEnumerator);

            return GetEnumeratorFromClass(value);
        }

        public static IEnumerator GetEnumeratorFromDataSet(DataSet dataSet)
        {
            return GetEnumeratorFromIEnumerable(new object[]
            {
                dataSet
            });
        }

        public static IEnumerator GetEnumeratorFromDataTable(DataTable dataTable)
        {
            return GetEnumeratorFromIEnumerable(dataTable.Rows);
        }

        public static IEnumerator GetEnumeratorFromIEnumerable(IEnumerable enumerable)
        {
            try
            {
                return GetEnumeratorFromIEnumerator(enumerable.GetEnumerator());
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerator GetEnumeratorFromIEnumerator(IEnumerator enumerator)
        {
            return enumerator;
        }

        public static IEnumerator GetEnumeratorFromClass(object value)
        {
            return GetEnumeratorFromIEnumerable(new object[]
            {
                value
            });
        }
        #endregion

        #region Methods.GetValueFrom
        public static object GetElementFromObject(object value, string name)
        {
            return GetElementFromObject(value, name, false);
        }

        public static object GetElementFromObject(object value, string name, bool isColumnsRetrieve)
        {
            if (value is System.Windows.Forms.BindingSource)
                value = ((System.Windows.Forms.BindingSource) value).DataSource;

            if (value == null)
                return null;

            if (value is DataSet)
                return GetValueFromDataSet(value as DataSet, name);

            if (value is DataTable)
                return GetValueFromDataTable(value as DataTable, name);

            if (value is ITypedList)
                return GetValueFromITypedList(value as ITypedList, name);

            if (value is ICustomTypeDescriptor)
                return GetValueFromICustomTypeDescriptor(value as ICustomTypeDescriptor, name);

            if (value is string)
                return value as string;

            if (value is Image)
                return value as Image;

            if (value is DictionaryEntry)
                return GetValueFromDictionaryEntry((DictionaryEntry) value, name);

            if (IsValueExistInClass(value, name))
                return GetValueFromClass(value, name, isColumnsRetrieve);

            object item = GetElement(value);
            if (item == null) item = value;

            if (item is DataSet)
                return GetValueFromDataSet(item as DataSet, name);

            if (item is DataTable)
                return GetValueFromDataTable(item as DataTable, name);

            if (item is ITypedList)
                return GetValueFromITypedList(item as ITypedList, name);

            if (item is ICustomTypeDescriptor)
                return GetValueFromICustomTypeDescriptor(item as ICustomTypeDescriptor, name);

            if (item is string)
                return item as string;

            if (item is Image)
                return value as Image;

            if (item is DictionaryEntry)
                return GetValueFromDictionaryEntry((DictionaryEntry) item, name);

            if (item == null) item = value;

            if (item.GetType().IsValueType)
                return item;

            return GetValueFromClass(item, name, isColumnsRetrieve);
        }

        public static object GetValueFromObject(object value, string name)
        {
            return GetValueFromObject(value, name, false);
        }

        public static object GetValueFromObject(object value, string name, bool isColumnsRetrieve)
        {
            //fix
            if ((value is DataRow[]) && (name != "Length"))
            {
                DataRow[] dr = value as DataRow[];
                if ((dr != null) && (dr.Length > 0)) value = dr[0];
            }

            //fix 2014.04.10, then 2014.04.23
            if ((value is IEnumerable) && !(value is string))
            {
                object item = GetElementFromEnumerable(value as IEnumerable);
                if (item != null) value = item;
            }

            if (value == null)
                return null;

            if (value is DataSet)
                return GetValueFromDataSet(value as DataSet, name);

            if (value is DataRow)
                return GetValueFromDataRow(value as DataRow, name);

            if (value is ITypedList)
                return GetValueFromITypedList(value as ITypedList, name);

            if (value is ICustomTypeDescriptor)
                return GetValueFromICustomTypeDescriptor(value as ICustomTypeDescriptor, name);

            if (value is string)
                return value as string;

            if (value is Image)
                return value as Image;

            if (value is DictionaryEntry)
                return GetValueFromDictionaryEntry((DictionaryEntry) value, name);

            Type valueType = value.GetType();

            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                return GetValueFromKeyValuePair(value, name);

            if (valueType.IsValueType)
                return value;

            return GetValueFromClass(value, name, isColumnsRetrieve);
        }

        public static object GetValueFromDataSet(DataSet dataSet, string name)
        {
            if (dataSet.Tables.Contains(name))
                return dataSet.Tables[name];
            return null;
        }

        public static object GetValueFromDataTable(DataTable dataTable, string name)
        {
            if (dataTable.ChildRelations.Contains(name))
                return dataTable.ChildRelations[name].ChildTable;

            if (dataTable.Columns.Contains(name))
                return dataTable.Columns[name];

            return null;
        }

        public static object GetValueFromDataRow(DataRow dataRow, string name)
        {
            if (dataRow.Table.Columns.Contains(name))
                return dataRow[name];
            if (dataRow.Table.ChildRelations.Contains(name))
                return dataRow.GetChildRows(name);

            return null;
        }

        public static object GetValueFromITypedList(ITypedList typedList, string name)
        {
            PropertyDescriptorCollection props = typedList.GetItemProperties(null);
            PropertyDescriptor prop = props[name];
            return prop.GetValue(typedList);
        }

        public static object GetValueFromITypedList(ITypedList typedList, string name, object value)
        {
            PropertyDescriptorCollection props = typedList.GetItemProperties(null);
            PropertyDescriptor prop = props[name];
            if (prop == null) return null;
            return prop.GetValue(value);
        }

        public static object GetValueFromICustomTypeDescriptor(ICustomTypeDescriptor customTypeDescriptor, string name)
        {
            PropertyDescriptorCollection props = customTypeDescriptor.GetProperties();
            PropertyDescriptor prop = props[name];
            return prop.GetValue(customTypeDescriptor);
        }

        public static object GetValueFromICustomTypeDescriptor(ICustomTypeDescriptor customTypeDescriptor, string name,
            object value)
        {
            PropertyDescriptorCollection props = customTypeDescriptor.GetProperties();
            PropertyDescriptor prop = props[name];
            if (prop == null) return null;
            return prop.GetValue(value);
        }

        public static object GetValueFromDictionaryEntry(DictionaryEntry entry, string name)
        {
            if (name == "Key")
                return entry.Key;
            if (name == "Value")
                return entry.Value;
            return null;
        }

        public static object GetValueFromKeyValuePair(object item, string name)
        {
            if (name == "Key" || name == "Value")
            {
                try
                {
                    return item.GetType().GetProperty(name).GetValue(item, null);
                }
                catch
                {
                }
            }
            return null;
        }

        public static object GetValueFromClass(object value, string name)
        {
            return GetValueFromClass(value, name, false);
        }

        public static object GetValueFromClass(object value, string name, bool isColumnsRetrieve)
        {
            if (value == null)
                return null;

            #region Properties
            if (StiOptions.Dictionary.BusinessObjects.AllowUseProperties)
            {
                PropertyInfo[] props = value is Type ? ((Type) value).GetProperties() : value.GetType().GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    if (prop.Name == name)
                    {
                        if (value is Type)
                            return prop.PropertyType;
                        else
                        {
                            try
                            {
                                object valueProp = prop.GetValue(value, null);
                                if (valueProp != null)
                                    return valueProp;
                                else if (IsTypeImplementICollection(prop.PropertyType) ||
                                         StiBusinessObject.FieldsIgnoreList.ContainsKey(prop.ReflectedType.Name + "." + prop.Name))
                                {
                                    return null;
                                }
                                else
                                {
                                    return prop.PropertyType;
                                }
                            }
                            catch
                            {
                                return prop.PropertyType;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Fields
            if (StiOptions.Dictionary.BusinessObjects.AllowUseFields)
            {
                FieldInfo[] fields = value is Type ? ((Type) value).GetFields() : value.GetType().GetFields();
                foreach (FieldInfo field in fields)
                {
                    if (field.Name == name)
                    {
                        if (value is Type)
                            return field.FieldType;
                        else
                        {
                            object valueField = field.GetValue(value);
                            if (valueField != null)
                                return valueField;
                            else
                                return field.FieldType;
                        }
                    }
                }
            }
            #endregion

            return null;
        }

        public static bool IsTypeImplementICollection(Type type)
        {
            try
            {
                foreach (Type impType in type.GetInterfaces())
                {
                    if (impType == typeof(ICollection)) return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public static bool IsValueExistInClass(object value, string name)
        {
            if (value == null)
                return false;

            if (name == "Item")
            {
                object item = GetElement(value);
                if (item != null) return false;    //IEnumerable and Lists contain internal Item property
            }

            #region Properties
            if (StiOptions.Dictionary.BusinessObjects.AllowUseProperties)
            {
                PropertyInfo[] props = value is Type ? ((Type) value).GetProperties() : value.GetType().GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    if (prop.Name == name)
                        return true;
                }
            }
            #endregion

            #region Fields
            if (StiOptions.Dictionary.BusinessObjects.AllowUseFields)
            {
                FieldInfo[] fields = value is Type ? ((Type) value).GetFields() : value.GetType().GetFields();
                foreach (FieldInfo field in fields)
                {
                    if (field.Name == name)
                        return true;
                }
            }
            #endregion

            return false;
        }
        #endregion

        #region Methods
        public static bool IsDataColumn(Type type)
        {
            if (type.IsValueType) return true;
            if (type == typeof(string)) return true;
            if (type == typeof(byte[])) return true;
            if (type == typeof(Image)) return true;
            return false;
        }

        public static string GetBusinessObjectFullName(StiBusinessObject businessObject)
        {
            string name = businessObject.Name;
            while (businessObject.ParentBusinessObject != null)
            {
                businessObject = businessObject.ParentBusinessObject;
                name = businessObject.Name + "." + name;
            }
            return name;
        }

        public static string GetBusinessObjectFullAlias(StiBusinessObject businessObject)
        {
            string alias = businessObject.Alias;
            while (businessObject.ParentBusinessObject != null)
            {
                businessObject = businessObject.ParentBusinessObject;
                alias = businessObject.Alias + "." + alias;
            }
            return alias;
        }

        public static StiBusinessObject GetBusinessObjectFromGuid(StiReport report, string guid)
        {
            List<StiBusinessObject> list = StiBusinessObjectHelper.GetBusinessObjectsFromReport(report);
            if (list == null) return null;
            foreach (StiBusinessObject obj in list)
            {
                if (obj.Guid == guid)
                {
                    return obj;
                }
            }

            return null;
        }

        public static List<StiBusinessObject> GetBusinessObjectsFromReport(StiBusinessObjectsCollection businessObjects)
        {
            List<StiBusinessObject> list = null;
            foreach (StiBusinessObject businessObject in businessObjects)
            {
                List<StiBusinessObject> list2 = GetBusinessObjectsFromReport(businessObject.BusinessObjects);
                if (list2 != null)
                {
                    if (list == null)
                        list = new List<StiBusinessObject>();
                    foreach (StiBusinessObject obj in list2)
                    {
                        list.Add(obj);
                    }
                }
                if (list == null)
                    list = new List<StiBusinessObject>();
                list.Add(businessObject);
            }
            return list;
        }

        public static List<StiBusinessObject> GetBusinessObjectsFromReport(StiReport report)
        {
            return GetBusinessObjectsFromReport(report.Dictionary.BusinessObjects);
        }


        public static List<string> GetUsedBusinessObjectsNamesList(StiReport report)
        {
            return GetUsedBusinessObjectsNamesList(report, false);
        }

        public static List<string> GetUsedBusinessObjectsNamesList(StiReport report, bool addColumns)
        {
            Hashtable businessobjectsNames = GetUsedBusinessObjectsNames(report, addColumns);
            List<string> names = new List<string>();
            foreach (DictionaryEntry de in businessobjectsNames)
            {
                names.Add((string) de.Value);
            }
            names.Sort();
            return names;
        }

        public static Hashtable GetUsedBusinessObjectsNames(StiReport report)
        {
            return GetUsedBusinessObjectsNames(report, false);
        }

        public static Hashtable GetUsedBusinessObjectsNames(StiReport report, bool addColumns)
        {
            Hashtable businessobjectsNames = new Hashtable();

            foreach (StiComponent component in report.GetComponents())
            {
                try
                {
                    StiDataBand dataBand = component as StiDataBand;
                    if (dataBand != null && dataBand.BusinessObject.Name != null &&
                        dataBand.BusinessObject.Name.Length > 0)
                    {
                        string fullName = StiBusinessObjectHelper.GetBusinessObjectFullName(dataBand.BusinessObject);
                        businessobjectsNames[fullName] = fullName;
                    }

                    StiCrossTab crosstab = component as StiCrossTab;
                    if (crosstab != null && crosstab.BusinessObject.Name != null &&
                        crosstab.BusinessObject.Name.Length > 0)
                    {
                        string fullName = StiBusinessObjectHelper.GetBusinessObjectFullName(crosstab.BusinessObject);
                        businessobjectsNames[fullName] = fullName;
                    }

                    StiGroupHeaderBand groupHeaderBand = component as StiGroupHeaderBand;
                    if (groupHeaderBand != null)
                    {
                        CheckExpression(groupHeaderBand.Condition.Value, component, businessobjectsNames, addColumns);
                    }

                    StiSimpleText stiSimpleText = component as StiSimpleText;
                    if (stiSimpleText != null)
                    {
                        CheckExpression(stiSimpleText.Text.Value, component, businessobjectsNames, addColumns);
                    }

                    StiText stiText = component as StiText;
                    if (stiText != null)
                    {
                        CheckExpression(stiText.ExcelValue.Value, component, businessobjectsNames, addColumns);
                    }

                    StiRichText richText = component as StiRichText;
                    if (richText != null)
                    {
                        using (RichTextBox richTextBox = new Controls.StiRichTextBox(false))
                        {
                            richTextBox.Rtf = richText.RtfText;
                            CheckExpression(richTextBox.Text, component, businessobjectsNames, addColumns);
                        }
                    }

                    StiImage stiImage = component as StiImage;
                    if (stiImage != null)
                    {
                        CheckExpression(stiImage.ImageData.Value, component, businessobjectsNames, addColumns);
                        CheckExpression("{" + stiImage.DataColumn + "}", component, businessobjectsNames, addColumns);
                    }

                    StiBarCode barcode = component as StiBarCode;
                    if (barcode != null)
                    {
                        CheckExpression(barcode.Code.Value, component, businessobjectsNames, addColumns);
                    }

                    StiZipCode zipcode = component as StiZipCode;
                    if (zipcode != null)
                    {
                        CheckExpression(zipcode.Code.Value, component, businessobjectsNames, addColumns);
                    }

                    StiCheckBox checkbox = component as StiCheckBox;
                    if (checkbox != null)
                    {
                        CheckExpression(checkbox.Checked.Value, component, businessobjectsNames, addColumns);
                    }

                    var chart = component as StiChart;
                    if (chart != null)
                    {
                        string fullName = StiBusinessObjectHelper.GetBusinessObjectFullName(chart.BusinessObject);
                        if (!string.IsNullOrEmpty(fullName))
                        {
                            businessobjectsNames[fullName] = fullName;
                        }
                        foreach (StiSeries series in chart.Series)
                        {
                            CheckExpression(series.Argument.Value, component, businessobjectsNames, addColumns);
                            CheckExpression("{" + series.ArgumentDataColumn + "}", component, businessobjectsNames,
                                addColumns);
                        }
                    }

                    var page = component as StiPage;
                    if (page != null)
                    {
                        CheckExpression(page.ExcelSheet.Value, component, businessobjectsNames, addColumns);
                    }

                    var icondition = component as IStiConditions;
                    if (icondition != null && icondition.Conditions.Count > 0)
                    {
                        var conditionList = new List<DictionaryEntry>();

                        #region Prepare conditions
                        foreach (StiBaseCondition cond in icondition.Conditions)
                        {
                            var condition = cond as StiCondition;
                            string expression = null;
                            if (cond is StiMultiCondition)
                            {
                                var multiCondition = cond as StiMultiCondition;
                                if (multiCondition.FilterOn && multiCondition.Filters.Count > 0)
                                {
                                    var conditionExpression = new StringBuilder("{");
                                    for (int index = 0; index < multiCondition.Filters.Count; index++)
                                    {
                                        var filter = multiCondition.Filters[index];
                                        conditionExpression.Append("(");
                                        conditionExpression.Append(
                                            StiDataHelper.GetFilterExpression(filter, filter.Column, report));
                                        conditionExpression.Append(")");
                                        if (index < multiCondition.Filters.Count - 1)
                                        {
                                            conditionExpression.Append(multiCondition.FilterMode == StiFilterMode.And
                                                ? " && "
                                                : " || ");
                                        }
                                    }
                                    conditionExpression.Append("}");
                                    var de = new DictionaryEntry(multiCondition, conditionExpression.ToString());
                                    conditionList.Add(de);
                                }
                            }
                            else if (condition != null)
                            {
                                expression =
                                    "{" + StiDataHelper.GetFilterExpression(condition, condition.Column, report) + "}";
                                var de = new DictionaryEntry(condition, expression);
                                conditionList.Add(de);
                            }
                        }
                        #endregion

                        if (conditionList.Count > 0)
                        {
                            foreach (var de in conditionList)
                            {
                                CheckExpression((string) de.Value, component, businessobjectsNames, addColumns);
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return businessobjectsNames;
        }

        private static void CheckExpression(string expression, StiComponent component, Hashtable businessobjectsNames,
            bool addColumns)
        {
            try
            {
                bool storeToPrint = false;
                object result =
                    Stimulsoft.Report.Engine.StiParser.ParseTextValue(expression, component, ref storeToPrint, false,
                        true);
                if (result != null && result is List<Stimulsoft.Report.Engine.StiParser.StiAsmCommand>)
                {
                    foreach (Stimulsoft.Report.Engine.StiParser.StiAsmCommand asmCommand in
                        result as List<Stimulsoft.Report.Engine.StiParser.StiAsmCommand>)
                    {
                        if (asmCommand.Type ==
                            Stimulsoft.Report.Engine.StiParser.StiAsmCommandType.PushBusinessObjectField)
                        {
                            string fieldPath = (string) asmCommand.Parameter1;
                            string[] parts = fieldPath.Split(new char[]
                            {
                                '.'
                            });

                            try
                            {
                                StiBusinessObject bo = component.Report.Dictionary.BusinessObjects[parts[0]];
                                string fullName = StiBusinessObjectHelper.GetBusinessObjectFullName(bo);
                                businessobjectsNames[fullName] = fullName;

                                string nameInSource = parts[1];
                                bo = bo.BusinessObjects[parts[1]];
                                if (parts.Length > 2)
                                {
                                    fullName = StiBusinessObjectHelper.GetBusinessObjectFullName(bo);
                                    businessobjectsNames[fullName] = fullName;
                                    int indexPart = 2;
                                    while (indexPart < parts.Length - 1)
                                    {
                                        nameInSource = parts[indexPart];
                                        bo = bo.BusinessObjects[parts[indexPart]];
                                        fullName = StiBusinessObjectHelper.GetBusinessObjectFullName(bo);
                                        businessobjectsNames[fullName] = fullName;
                                        indexPart++;
                                    }
                                }
                                else
                                {
                                    if (bo != null)
                                    {
                                        fullName = StiBusinessObjectHelper.GetBusinessObjectFullName(bo);
                                        businessobjectsNames[fullName] = fullName;
                                    }
                                }
                                if (addColumns)
                                {
                                    businessobjectsNames[fieldPath] = fieldPath;
                                }
                            }
                            catch
                            {
                                businessobjectsNames[fieldPath] = fieldPath;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        #endregion
    }
}