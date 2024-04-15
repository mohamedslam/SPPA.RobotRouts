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
using Stimulsoft.Base.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Dictionary
{

    public sealed class StiBusinessObjectToDataSet
	{
	    #region class StiRelation
	    private class StiRelation
	    {
	        public string Name;
	        public string ParentTableName;
	        public string ChildTableName;
	        public string ParentColumnName;
	        public string ChildColumnName;

	        public StiRelation(string name, string parentTableName, string childTableName, string parentColumnName, string childColumnName)
	        {
	            this.Name = name;
	            this.ParentTableName = parentTableName;
	            this.ChildTableName = childTableName;
	            this.ParentColumnName = parentColumnName;
	            this.ChildColumnName = childColumnName;
	        }
	    }
	    #endregion

        #region Properties
        [Obsolete("StiBusinessObjectToDataSet.PropertiesProcessingType property is obsolete. Please use StiOptions.Dictionary.BusinessObjects.PropertiesProcessingType property insead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
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

        [Obsolete("StiBusinessObjectToDataSet.FieldsProcessingType property is obsolete. Please use StiOptions.Dictionary.BusinessObjects.FieldsProcessingType property insead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
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

        [Obsolete("StiBusinessObjectToDataSet.Delimeter property is obsolete. Please use StiOptions.Dictionary.BusinessObjects.Delimeter property insead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static char Delimeter
		{
			get
			{
				return StiOptions.Dictionary.BusinessObjects.Delimeter;
			}
			set
			{
				StiOptions.Dictionary.BusinessObjects.Delimeter = value;
			}
		}

        [Obsolete("StiBusinessObjectToDataSet.MaxLevel property is obsolete. Please use StiOptions.Dictionary.BusinessObjects.MaxLevel property insead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int MaxLevel
		{
			get
			{
				return StiOptions.Dictionary.BusinessObjects.MaxLevel;
			}
			set
			{
				StiOptions.Dictionary.BusinessObjects.MaxLevel = value;
			}
		}

        [Obsolete("StiBusinessObjectToDataSet.CheckTableDuplication property is obsolete. Please use StiOptions.Dictionary.BusinessObjects.CheckTableDuplication property insead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool CheckTableDuplication
		{
			get
			{
				return StiOptions.Dictionary.BusinessObjects.CheckTableDuplication;
			}
			set
			{
				StiOptions.Dictionary.BusinessObjects.CheckTableDuplication = value;
			}
		}
		#endregion

		#region Fields
		private DataSet dataSet;
		private Hashtable relations;
		private Hashtable uniques;
		private int level;
		#endregion
        
		#region Methods
        private static Type GetType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return Nullable.GetUnderlyingType(type);

            return type;
        }

        internal static bool IsAllowUseProperty(PropertyDescriptor valueProp)
		{
			var attr = valueProp.Attributes[typeof(StiBrowsableAttribute)];
			if (attr != null)
			    return ((StiBrowsableAttribute)attr).Browsable;
			
			attr = valueProp.Attributes[typeof(BrowsableAttribute)];
			if (attr != null && (!((BrowsableAttribute)attr).Browsable) &&
                StiOptions.Dictionary.BusinessObjects.PropertiesProcessingType == StiPropertiesProcessingType.Browsable) return false;

			return true;
		}

        internal static string GetAlias(PropertyInfo valueProp)
        {
            var attrs = valueProp.GetCustomAttributes(typeof(StiAliasAttribute), true);
            return attrs.Length > 0 ? ((StiAliasAttribute)attrs[0]).Alias : null;
        }

        internal static string GetAlias(FieldInfo valueProp)
        {
            var attrs = valueProp.GetCustomAttributes(typeof(StiAliasAttribute), true);
            return attrs.Length > 0 ? ((StiAliasAttribute)attrs[0]).Alias : null;
        }

        internal static string GetAlias(PropertyDescriptor valueProp)
        {
            var attr = valueProp.Attributes[typeof(StiAliasAttribute)];
            return attr != null ? ((StiAliasAttribute)attr).Alias : null;
        }

        internal static bool IsAllowUseProperty(PropertyInfo valueProp, Hashtable hashIsAllowUseProperty)
        {
            bool result;

            var res = hashIsAllowUseProperty[valueProp];
            if (res == null)
            {
                result = IsAllowUseProperty(valueProp);
                hashIsAllowUseProperty[valueProp] = result;
            }
            else
                result = (bool) res;
            
            return result;
        }

        internal static bool IsAllowUseProperty(PropertyInfo valueProp)
		{
			var attrs = valueProp.GetCustomAttributes(typeof(StiBrowsableAttribute), true);
			if (attrs.Length > 0)
			    return ((StiBrowsableAttribute)attrs[0]).Browsable;

			attrs = valueProp.GetCustomAttributes(typeof(BrowsableAttribute), true);
			if (attrs.Length > 0 && !((BrowsableAttribute)attrs[0]).Browsable &&
                StiOptions.Dictionary.BusinessObjects.PropertiesProcessingType == StiPropertiesProcessingType.Browsable) return false;

			return true;
		}

        internal static bool IsAllowUseField(FieldInfo valueField)
		{
			var attrs = valueField.GetCustomAttributes(typeof(StiBrowsableAttribute), true);
			if (attrs.Length > 0)
			    return ((StiBrowsableAttribute)attrs[0]).Browsable;

			attrs = valueField.FieldType.GetCustomAttributes(typeof(BrowsableAttribute), true);
			if (attrs.Length > 0 && !((BrowsableAttribute)attrs[0]).Browsable &&
                StiOptions.Dictionary.BusinessObjects.PropertiesProcessingType == StiPropertiesProcessingType.Browsable) return false;

			return true;
		}		

		private bool NeedProcess(object obj)
		{
		    if (obj == null) return false;

		    if (obj.GetType().IsValueType) return false;
		    if (obj is string)return false;
		    if (obj is Image)return false;

		    if (obj.GetType().ToString() == "System.Windows.Media.ImageSource") return false;
		    if (obj.GetType().ToString() == "System.Windows.Threading.Dispatcher") return false;
				
		    if (obj is Point)return false;
		    if (obj is Size)return false;
		    if (obj is Rectangle)return false;

		    if (obj is PointF)return false;
		    if (obj is SizeF)return false;
		    if (obj is RectangleF)return false;
				
		    if (obj is IEnumerable || obj.GetType().IsClass)return true;

		    return false;
		}

		private bool NeedProcess(Type type)
		{
		    if (type == null) return false;

		    if (type.IsValueType) return false;
		    if (type == typeof(string)) return false;
		    if (StiTypeFinder.FindType(type, typeof(Image))) return false;

		    if (type.ToString() == "System.Windows.Media.ImageSource") return false;
		    if (type.ToString() == "System.Windows.Threading.Dispatcher") return false;

		    if (type == typeof(Point)) return false;
		    if (type == typeof(Size)) return false;
		    if (type == typeof(Rectangle)) return false;

		    if (type == typeof(PointF)) return false;
		    if (type == typeof(SizeF)) return false;
		    if (type == typeof(RectangleF)) return false;

		    if (StiTypeFinder.FindInterface(type, typeof(IEnumerable)) || type.IsClass) return true;

		    return false;
		}

        internal static DataColumn GetDataColumn(string name, Type type)
        {
            return GetDataColumn(name, name, type);
        }

		internal static DataColumn GetDataColumn(string name, string alias, Type type)
		{
		    return new DataColumn(name, GetType(type))
		    {
		        Caption = string.IsNullOrEmpty(alias) ? name : alias,
		        AllowDBNull = !StiOptions.Dictionary.UseAllowDBNullProperty || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) || !type.IsValueType || type == typeof(string))
		    };
		}

		private bool CheckTable(ArrayList tableList, ArrayList tableNames, string checkName, object checkObject)
		{
            if (!StiOptions.Dictionary.BusinessObjects.CheckTableDuplication) return true;

			for (var index = 0; index < tableNames.Count; index++)
			{
				var name = tableNames[index] as string;
				var obj = tableList[index];

                if (name == checkName && obj.GetType() == checkObject.GetType())
                {
                    if (obj is IEnumerable && checkObject is IEnumerable)
                    {
                        var objValue = GetValueFromEnumerable(obj as IEnumerable);
                        var checkObjectValue = GetValueFromEnumerable(checkObject as IEnumerable);

                        if (objValue != null && checkObjectValue != null && objValue.GetType() == checkObjectValue.GetType())
                            return false;
                    }
                    else return false;
                }
			}
			return true;
		}

		private string GetTableName(string parentTableName, string columnName)
		{
            return $"{parentTableName}{StiOptions.Dictionary.BusinessObjects.Delimeter}{columnName}";
		}

		private string GetNameFromObject(object obj, string baseName)
		{
            #region Check for Name from ITypedList
			if (obj is ITypedList)
			{
				var name = ((ITypedList)obj).GetListName(null);
				if (name != null && name.Trim().Length > 0)
				    return name;
			}
            #endregion

            #region Check for Name from ICustomTypeDescriptor
			else if (obj is ICustomTypeDescriptor)
			{
				var name = ((ICustomTypeDescriptor)obj).GetComponentName();
				if (name != null && name.Trim().Length > 0)
				    return name;
			}
            #endregion

            return baseName;
		}

		private void ProcessItem(DataTable table, object obj, Type typeOfData, DataRow row, string tableName, ArrayList tableList, 
			ArrayList tableNames, string uniqueIndex, string columnName)
		{
			if (NeedProcess(obj) ||  NeedProcess(typeOfData))
			{
				if (row != null)
				    row[uniqueIndex] = table.Rows.Count;

                if (level < StiOptions.Dictionary.BusinessObjects.MaxLevel)
				{
					var childUniqueIndex = "parent";
					var childTableName = GetTableName(tableName, columnName);

					if (obj != null)
					    childTableName = GetNameFromObject(obj, childTableName);

					if (obj == null || CheckTable(tableList, tableNames, columnName, obj))
					{
                        DataTable childTable;
                        if (obj != null)
                        {
                            tableList.Add(obj);
                            tableNames.Add(columnName);

                            childTable = ConvertBusinessObjectToDataTable(
                                childTableName, tableList, tableNames, obj, typeOfData, ref childUniqueIndex, table.Rows.Count);

                            tableList.RemoveAt(tableList.Count - 1);
                            tableNames.RemoveAt(tableNames.Count - 1);
                            
                        }
                        else
                        {
                            var childUniqueIndex2 = "parent";

                            childTable = ConvertBusinessObjectToDataTable(childTableName, tableList, tableNames, null, typeOfData, ref childUniqueIndex2, table.Rows.Count);
                        }

                        if (childTable != null)
                        {
                            var relationName = childTableName + "Relation";
                            if (relations[relationName] == null)
                            {
                                var relation = new StiRelation(relationName,
                                    tableName, childTableName,
                                    uniqueIndex, GetUniqueName(childUniqueIndex, childTable));

                                relations[relationName] = relation;
                            }
                        }
					}
				}
			}

            var columnIndex = table.Columns.IndexOf(columnName);
            if (columnIndex != -1 && row != null)
            {
                if (obj == null && StiOptions.Dictionary.ConvertNulls.UseDefaultDateTimeForNullValues && table.Columns[columnIndex].DataType == typeof(DateTime))
                    obj = StiOptions.Dictionary.ConvertNulls.DefaultDateTimeForNullValues;
                row[columnIndex] = obj ?? DBNull.Value;
            }
		}        				

		private string GetUniqueName(string uniqueName, DataTable table)
		{
			if (uniqueName == null)return null;
			if (uniques[table] != null)
			{
			    var tbl = uniques[table] as Hashtable;
			    if (tbl != null && tbl[uniqueName] != null)
			        return tbl[uniqueName] as string;
			}

			var index = -1;
			while (true)
			{
				var str = "_" + uniqueName;

			    if (index == -1)
				    str += "ID";

				else if (index >= 0)
				    str += $"ID{index}";

				var test = true;
				foreach (DataColumn col in table.Columns)
				{
					if (col.ColumnName == str)
					{
						test = false;
						break;
					}
				}

				if (test)
				{
					var tbl = uniques[table] as Hashtable;
					if (tbl == null)
					{
						tbl = new Hashtable();
						uniques[table] = tbl;
					}

					tbl[uniqueName] = str;
					return str;
				}
				index++;
			}
		}

		private static Type GetTypeOfArrayElement(Type arrayType)
		{
		    if (arrayType == null) return null;

		    if (arrayType.IsGenericType)
		    {
		        try					
		        {
		            var types = arrayType.GetGenericArguments();
		            if (types.Length > 1)
		                return typeof(object);

		            if (types.Length == 1)
		                return types[0];
		        }
		        catch
		        {
		        }
		    }

		    if (arrayType.GetElementType() != null)
		        return arrayType.GetElementType();

		    var methods = arrayType.GetMethods();

		    foreach (var methodInfo in methods)
		    {
		        if (methodInfo.Name == "get_Item")
		            return methodInfo.ReturnType;
		    }

		    var typeName = arrayType.FullName;
		    return StiTypeFinder.GetType(typeName.Substring(0, typeName.Length - 2));
		}

		private DataTable GetTable(string tableName, object obj, Type typeOfData, string nameID, object valueID)
		{
			var enumerable = obj as IEnumerable;
			var table = dataSet.Tables[tableName];
			if (table != null)return table;

			table = new DataTable(tableName);

			#region Create Table
			#region Create Columns
			if (enumerable is ITypedList)
			{
				#region ITypedList
				var typedList = enumerable as ITypedList;
				var props = typedList.GetItemProperties(null);
				foreach (PropertyDescriptor prop in props)
				{
					if (!IsAllowUseProperty(prop))continue;

					var column = GetDataColumn(prop.Name, GetAlias(prop), prop.PropertyType);

					if (!table.Columns.Contains(column.ColumnName))
					    table.Columns.Add(column);
				}
				#endregion
			}
			else if (enumerable is ICustomTypeDescriptor)
			{
				#region ICustomTypeDescriptor
				var customTypeDescriptor = enumerable as ICustomTypeDescriptor;
				var props = customTypeDescriptor.GetProperties();
				foreach (PropertyDescriptor prop in props)
				{
					if (!IsAllowUseProperty(prop)) continue;

                    var column = GetDataColumn(prop.Name, GetAlias(prop), prop.PropertyType);

					if (!table.Columns.Contains(column.ColumnName))
					    table.Columns.Add(column);					
				}
				#endregion
			}
			else
			{
				#region Collection
				Type type = null;
				Type enumType = null;
                
				#region Collection
				if (enumerable != null) enumType = enumerable.GetType();

				#region Check generic types
				if (enumType != null && enumType.IsGenericType)
				{
					var types = enumType.GetGenericArguments();

				    if (types.Length != 1)
				        type = typeof(object);

				    else if (types.Length == 1)
				        type = types[0];
				}
				#endregion

				if (typeOfData != null) type = typeOfData;
			
				object value = null;
				if (type == null)
				{
				    value = enumerable != null ? GetValueFromEnumerable(enumerable) : obj;

				    if (value != null)type = value.GetType();
				}

				if (value == null && type == null && enumType != null)
				{
					type = enumType;
					type = GetTypeOfArrayElement(type);
				}
                
				if (value != null || type != null)
				{
					if ((!type.IsClass ||
						value is string ||
						value is Image ||
						value is IEnumerable) && 
						!(value is DictionaryEntry) &&
                        type.ToString().IndexOf("KeyValuePair", StringComparison.InvariantCulture) == -1 &&
						value != null)
					{
                        if (StiOptions.Dictionary.BusinessObjects.AllowUseDataColumn)
                        {
                            if (!table.Columns.Contains("Data"))
                            {
                                var column = GetDataColumn("Data", value.GetType());
                                table.Columns.Add(column);
                            }
                        }
					}
					else
					{
						if (value is ICustomTypeDescriptor)
						{
							var typeDescriptor = value as ICustomTypeDescriptor;
							var props = typeDescriptor.GetProperties();

							foreach (PropertyDescriptor prop in props)
							{
								if (!IsAllowUseProperty(prop)) continue;

								var column = GetDataColumn(prop.Name, GetAlias(prop), prop.PropertyType);
								if (!table.Columns.Contains(column.ColumnName))
								    table.Columns.Add(column);							
							}
						}
						else
						{
                            #region Properties
							if (StiOptions.Dictionary.BusinessObjects.AllowUseProperties)
							{
								PropertyInfo[] props = null;
								if (value == null)
								{
									if (type != typeof(string))
									{
									    if (StiTypeFinder.FindInterface(type, typeof(IEnumerable)))
									        type = GetTypeOfArrayElement(type);
									}
									if (type != null)
										props = type.GetProperties();
								}
								else props = value.GetType().GetProperties();

								if (props != null)
								{
									foreach (var prop in props)
									{
										if (!IsAllowUseProperty(prop)) continue;
                                        if (prop.DeclaringType == typeof(string)) continue;
                                        if (prop.DeclaringType == typeof(Array)) continue;

                                        var column = GetDataColumn(prop.Name, GetAlias(prop), prop.PropertyType);

                                        if (!table.Columns.Contains(column.ColumnName))
                                            table.Columns.Add(column);
									}
								}
							}
                            #endregion

                            #region Fields
							if (StiOptions.Dictionary.BusinessObjects.AllowUseFields)
							{
								FieldInfo[] fields = null;
								if (value == null)
								{
									if (type != typeof(string))
									{
									    if (StiTypeFinder.FindInterface(type, typeof(IEnumerable)))
									        type = GetTypeOfArrayElement(type);
									}

									if (type != null)
										fields = type.GetFields();
								}
								else
								    fields = value.GetType().GetFields();

								if (fields != null)
								{
									foreach (var field in fields)
									{
										if (!IsAllowUseField(field)) continue;

                                        var column = GetDataColumn(field.Name, GetAlias(field), field.FieldType);


										if (!table.Columns.Contains(column.ColumnName))
										    table.Columns.Add(column);
									}
								}
							}
                            #endregion
						}
					}
				}
				#endregion

				#endregion
			}
			#endregion

			#region Create ID
			var uniqueIndex = GetUniqueName(string.Empty, table);
			var uniqueColumn = new DataColumn(uniqueIndex, typeof(int));
			
			table.Columns.Add(uniqueColumn);
			#endregion					
			#endregion

			#region Create nameID for data relation
			if (nameID != null)
			{
				nameID = GetUniqueName(nameID, table);

				table.Columns.Add(GetDataColumn(nameID, valueID.GetType()));
			}
			#endregion

			#region Current
			var currColumn = new DataColumn("_Current", typeof(object));
			table.Columns.Add(currColumn);
			#endregion

			dataSet.Tables.Add(table);
			return table;
		}

		private object GetValueFromEnumerable(IEnumerable enumerable)
		{
			var enumerator = enumerable.GetEnumerator();
            if (StiOptions.Engine.AllowUseResetMethodInBusinessObject)
            {
                try
                {
                    enumerator.Reset();
                }
                catch
                {
                }
            }

			while (enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				    return enumerator.Current;
			}

			return null;
		}

		private void ProcessUniqueIndex(string nameID, object valueID, DataTable table, DataRow row)
		{
		    if (nameID == null) return;

		    var columnName = GetUniqueName(nameID, table);

		    if (table.Columns[columnName] == null)
		        table.Columns.Add(columnName, typeof(int));

		    row[columnName] = valueID;
		}

		private void FillDataTableFromType(DataTable table, string tableName, ArrayList tableList, ArrayList tableNames, string nameID, object valueID)
		{
			foreach (DataColumn column in table.Columns)
			{
				if (column.ColumnName == "_Current" ||
					column.ColumnName == "Data" ||                    
					column.ColumnName == "_parentID" ||
					column.ColumnName == "_ID") continue;

			    var uniqueIndex = GetUniqueName(string.Empty, table);
				ProcessItem(table, null, column.DataType, null, tableName, tableList, tableNames, uniqueIndex, column.ColumnName);
			}
		}

		private void FillDataTable(DataTable table, string tableName, ArrayList tableList, ArrayList tableNames, object obj, string nameID, object valueID)
		{
			if (obj == null)
			{
				FillDataTableFromType(table, tableName, tableList, tableNames, nameID, valueID);
				return;
			}

			var enumerable = obj as IEnumerable;
		    var value = enumerable != null ? GetValueFromEnumerable(enumerable) : obj;
			var uniqueIndex = GetUniqueName(string.Empty, table);

            #region Fill Data
            #region ITypedList || ICustomTypeDescriptor
			if (enumerable is ITypedList || enumerable is ICustomTypeDescriptor)
			{
				var typedList = enumerable as ITypedList;
				var customTypeDescriptor = enumerable as ICustomTypeDescriptor;

			    var valueProps = typedList != null 
			        ? typedList.GetItemProperties(null) 
			        : customTypeDescriptor.GetProperties();

				var enumerator = enumerable.GetEnumerator();

                if (StiOptions.Engine.AllowUseResetMethodInBusinessObject)
                {
                    try
                    {
                        enumerator.Reset();
                    }
                    catch
                    {
                    }
                }
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null)
					{
                        #region Add column to table from current row
						foreach (PropertyDescriptor valueProp in valueProps)
						{
							if (table.Columns[valueProp.Name] == null)
							{
                                var dataColumn = GetDataColumn(valueProp.Name, GetAlias(valueProp), valueProp.PropertyType);
                                table.Columns.Add(dataColumn);
							}
						}
                        #endregion

                        #region Add New Row
						var row = table.NewRow();
						row["_Current"] = enumerator.Current;

						foreach (PropertyDescriptor valueProp in valueProps)
						{
							try
							{
								var obj2 = valueProp.GetValue(enumerator.Current);
                                if (obj2 != null || StiOptions.Dictionary.BusinessObjects.AllowProcessNullItemsInEnumerables)
								    ProcessItem(table, obj2, valueProp.PropertyType, row, tableName, tableList, tableNames, uniqueIndex, valueProp.Name);
							}
							catch
							{
							}
						}
						ProcessUniqueIndex(nameID, valueID, table, row);
						table.Rows.Add(row);
                        #endregion
					}
				}
			}
            #endregion

            #region Collection
			else
			{
                var hashIsAllowUseProperty = new Hashtable();

                if (enumerable == null)
                    FillOneItem(table, value, tableName, tableList, tableNames, nameID, valueID, uniqueIndex, hashIsAllowUseProperty);

				else
				{
					var enumerator = enumerable.GetEnumerator();
                    if (StiOptions.Engine.AllowUseResetMethodInBusinessObject)
                    {
                        try
                        {
                            enumerator.Reset();
                        }
                        catch
                        {
                        }
                    }
					var processed = false;

					var listOfObject = new ArrayList();
					while (enumerator.MoveNext())
					{
						listOfObject.Add(enumerator.Current);
					}

					foreach (var objValue in listOfObject)
					{
                        FillOneItem(table, objValue, tableName, tableList, tableNames, nameID, valueID, uniqueIndex, hashIsAllowUseProperty);
						processed = true;
					}

				    if (!processed)
				        FillDataTableFromType(table, tableName, tableList, tableNames, nameID, valueID);
				}
                hashIsAllowUseProperty.Clear();
			}
            #endregion

            #endregion
		}

		private void FillOneItem(DataTable table, object obj, string tableName, ArrayList tableList, ArrayList tableNames, string nameID, object valueID,
			string uniqueIndex, Hashtable hashIsAllowUseProperty)
		{
			DataRow row = null;
		    if (obj == null) return;

		    if ((!obj.GetType().IsClass ||
		         obj is string ||
		         obj is Image ||
		         obj is IEnumerable) && 
		        !(obj is DictionaryEntry) &&
		        obj.GetType().ToString().IndexOf("KeyValuePair", StringComparison.InvariantCulture) == -1)
		    {
		        row = table.NewRow();

		        if (StiOptions.Dictionary.BusinessObjects.AllowUseDataColumn)
		        {
		            if (!table.Columns.Contains("Data"))
		            {
		                var column = GetDataColumn("Data", obj.GetType());
		                table.Columns.Add(column);
		            }
		        }

		        row["_Current"] = obj;

		        if (StiOptions.Dictionary.BusinessObjects.AllowUseDataColumn)
		            row["Data"] = obj;

		        ProcessItem(table, obj, null, row, tableName, tableList, tableNames, uniqueIndex, level.ToString());
		    }
		    else
		    {
		        #region ICustomTypeDescriptor
		        if (obj is ICustomTypeDescriptor)
		        {
		            var typeDescriptor = obj as ICustomTypeDescriptor;
		            var valueProps = typeDescriptor.GetProperties();

		            foreach (PropertyDescriptor valueProp in valueProps)
		            {
		                if (!IsAllowUseProperty(valueProp))continue;

		                if (table.Columns[valueProp.Name] == null)
		                {
		                    var dataColumn = GetDataColumn(valueProp.Name, GetAlias(valueProp), valueProp.PropertyType);
		                    table.Columns.Add(dataColumn);
		                }
		            }

		            row = table.NewRow();
		            row["_Current"] = obj;
						
		            foreach (PropertyDescriptor valueProp in valueProps)
		            {
		                if (!IsAllowUseProperty(valueProp))continue;

		                try
		                {
		                    var obj2 = valueProp.GetValue(obj);
		                    ProcessItem(table, obj2, null, row, tableName, tableList, tableNames, uniqueIndex, valueProp.Name);
		                }
		                catch
		                {
		                }
		            }
		        }
		        #endregion

		        else
		        {
		            #region Properties
		            if (StiOptions.Dictionary.BusinessObjects.AllowUseProperties)
		            {
		                var valueProps = obj.GetType().GetProperties();
		                foreach (var valueProp in valueProps)
		                {
		                    if (!IsAllowUseProperty(valueProp, hashIsAllowUseProperty)) continue;

		                    if (table.Columns[valueProp.Name] == null)
		                    {
		                        var dataColumn = GetDataColumn(valueProp.Name, GetAlias(valueProp), valueProp.PropertyType);
		                        table.Columns.Add(dataColumn);
		                    }
		                }

		                row = table.NewRow();
		                row["_Current"] = obj;

		                foreach (var valueProp in valueProps)
		                {
		                    if (!IsAllowUseProperty(valueProp, hashIsAllowUseProperty)) continue;

		                    try
		                    {
		                        if (valueProp.CanRead)
		                        {
		                            var obj2 = valueProp.GetValue(obj, null);
		                            ProcessItem(table, obj2, valueProp.PropertyType, row, tableName, tableList, tableNames, uniqueIndex, valueProp.Name);
		                        }
		                    }
		                    catch
		                    {
		                    }
		                }
		            }
		            #endregion

		            #region Fields
		            if (StiOptions.Dictionary.BusinessObjects.AllowUseFields)
		            {
		                var valueFields = obj.GetType().GetFields();
		                foreach (var valueField in valueFields)
		                {
		                    if (!IsAllowUseField(valueField))continue;

		                    if (table.Columns[valueField.Name] == null)
		                    {
		                        var dataColumn = GetDataColumn(valueField.Name, GetAlias(valueField), valueField.FieldType);
		                        table.Columns.Add(dataColumn);
		                    }
		                }

		                if (row == null)
		                    row = table.NewRow();

		                row["_Current"] = obj;

		                foreach (var valueField in valueFields)
		                {
		                    if (!IsAllowUseField(valueField))continue;

		                    try
		                    {
		                        var obj2 = valueField.GetValue(obj);
		                        ProcessItem(table, obj2, null, row, tableName, tableList, tableNames, uniqueIndex, valueField.Name);
		                    }
		                    catch
		                    {
		                    }
		                }
		            }
		            #endregion
		        }
		    }
		    if (row == null) row = table.NewRow();
		    ProcessUniqueIndex(nameID, valueID, table, row);

            for (int index = 0; index < row.ItemArray.Length; index++)
            {
                if (row.ItemArray[index] == DBNull.Value && !table.Columns[index].AllowDBNull)
                {
                    table.Columns[index].AllowDBNull = true;
                }
            }

		    table.Rows.Add(row);
		}

		private DataTable ConvertBusinessObjectToDataTable(string tableName, ArrayList tableList, 
			ArrayList tableNames, object obj, Type typeOfData, ref string nameID, object valueID)
		{
			level ++;

			if (obj != null)tableName = GetNameFromObject(obj, tableName);

			var table = GetTable(tableName, obj, typeOfData, nameID, valueID);
			if (table != null)
				FillDataTable(table, tableName, tableList, tableNames, obj, nameID, valueID);

			level --;

			return table;
		}

		public DataSet ConvertBusinessObjectToDataSet(string name, object obj)
		{
			dataSet = new DataSet(name);
			relations = new Hashtable();
			uniques = new Hashtable();
			var tableList = new ArrayList();
			var tableNames = new ArrayList();

			level = 0;

			string nameID = null;
			ConvertBusinessObjectToDataTable(name, tableList, tableNames, obj, null, ref nameID, null);

			var rels = new StiRelation[relations.Count];
			relations.Values.CopyTo(rels, 0);

			foreach (var rel in rels)
			{
				try
				{
					var parentColumns = new DataColumn[1];
					var parentTableIndex = dataSet.Tables.IndexOf(rel.ParentTableName);
					parentColumns[0] = dataSet.Tables[parentTableIndex].Columns[rel.ParentColumnName];

					var childColumns = new DataColumn[1];
					var childTableIndex = dataSet.Tables.IndexOf(rel.ChildTableName);
					childColumns[0] = dataSet.Tables[childTableIndex].Columns[rel.ChildColumnName];

					dataSet.Relations.Add(rel.Name, parentColumns, childColumns);
				}
				catch
				{
				}
			}

			relations.Clear();
			relations = null;

			uniques.Clear();
			uniques = null;

			return dataSet;
		}
		#endregion
	}
}
