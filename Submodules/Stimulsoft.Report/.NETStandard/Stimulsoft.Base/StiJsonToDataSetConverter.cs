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
using System.Data;
using System.Linq;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using System.Text;
using Stimulsoft.Base.Helpers;

namespace Stimulsoft.Base
{
    public static class StiJsonToDataSetConverter
    {
        #region Fields
        private static DataTable mainDataTable;
        private static Hashtable hashDublicateTableName;
        #endregion

        #region Methods
        public static DataSet GetDataSet(JToken token)
        {
            var dataSet = new DataSet();

            ParseJToken(dataSet, null, token);

            CheckColumnType(dataSet);
            RemoveEmptyTable(dataSet);

            hashDublicateTableName = null;

            return dataSet;
        }

        public static DataSet GetDataSet(List<JToken> tokens, bool useOneTable = false)
        {
            var dataSet = new DataSet();

            if (useOneTable)
            {
                mainDataTable = new DataTable();
                dataSet.Tables.Add(mainDataTable);
            }

            JArray array = new JArray();
            foreach (JToken token in tokens)
            {
                array.Add(token);
            }

            ParseJToken(dataSet, "", array);

            CheckColumnType(dataSet);
            RemoveEmptyTable(dataSet);

            hashDublicateTableName = null;

            return dataSet;
        }
        
        public static DataSet GetDataSet(byte[] content)
        {
            var str = StiBytesToStringConverter.ConvertBytesToString(content);
            return GetDataSet(str);
        }

        public static DataSet GetDataSet(string text)
        {
            if (text == null)
                return null;

            var jToken = JsonConvert.DeserializeObject(text) as JToken;

            return GetDataSet(jToken);
        }

        public static DataSet GetDataSet(XElement element)
        {
            var text = JsonConvert.SerializeXNode(element, Formatting.Indented);

            var jToken = JsonConvert.DeserializeObject(text) as JToken;
            return GetDataSet(jToken);
        }

        public static DataSet GetDataSetFromFile(string path)
        {
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                var jToken = JsonConvert.DeserializeObject(text) as JToken;
                return GetDataSet(jToken);
            }

            if (!string.IsNullOrEmpty(path) && (path.ToLower().StartsWith("http:") || path.ToLower().StartsWith("https:")))
            {
                byte[] buffer = StiBytesFromURL.Load(path);
                string text = Encoding.UTF8.GetString(buffer);
                var jToken = JsonConvert.DeserializeObject(text) as JToken;
                return GetDataSet(jToken);
            }

            return null;
        }

        public static DataSet GetDataSetFromXmlFile(string path)
        {
            if (File.Exists(path))
            {
                var element = XElement.Load(path);
                var text = JsonConvert.SerializeXNode(element, Formatting.Indented);

                var jToken = JsonConvert.DeserializeObject(text) as JToken;
                return GetDataSet(jToken);
            }

            return null;
        }

        public static DataSet GetDataSetFromXml(byte[] array)
        {
            if (array != null)
            {
                string text;
                using (var stream = new MemoryStream(array))
                {
                    var element = XElement.Load(stream);
                    text = JsonConvert.SerializeXNode(element, Formatting.Indented);
                }

                var jToken = JsonConvert.DeserializeObject(text) as JToken;
                return GetDataSet(jToken);
            }

            return null;
        }
        
        internal static void CheckColumnType(DataSet dataSet)
        {
            if (Stimulsoft.Base.StiBaseOptions.JsonParseTypeAsString) return;

            for (int indexTable = 0; indexTable < dataSet.Tables.Count; indexTable++)
            {
                for (int indexColumn = 0; indexColumn < dataSet.Tables[indexTable].Columns.Count; indexColumn++)
                {
                    Type type = null;
                    for (int indexRow = 0; indexRow < dataSet.Tables[indexTable].Rows.Count; indexRow++)
                    {
                        var value = dataSet.Tables[indexTable].Rows[indexRow][indexColumn];
                        var valueType = ParseString(value.ToString());

                        if (type == null)
                        {
                            type = valueType;
                        }
                        else
                        {
                            if (!type.Equals(valueType))
                            {
                                type = null;
                                break;
                            }
                        }
                    }

                    if (type != null && !type.Equals(typeof(string)))
                    {
                        ChangeColumnDataType(dataSet.Tables[indexTable], dataSet.Tables[indexTable].Columns[indexColumn], type);
                    }
                }
            }
        }

        private static void RemoveEmptyTable(DataSet dataSet)
        {
            DataTable table1 = null;
            if (dataSet.Tables.Count > 0)
                table1 = dataSet.Tables[0];

            if (table1 != null && table1.TableName == "Table1" && (table1.Rows.Count == 0 || table1.Columns.Count == 0))
                dataSet.Tables.Remove(table1);
        }

        private static Type ParseString(string str)
        {
            bool boolValue;
            //byte byteValue;
            //Int16 int16Value;
            //Int32 int32Value;
            //Int64 int64Value;
            double doubleValue;
            DateTime dateValue;
            decimal decimalValue;
            Guid guidValue;
            TimeSpan timeSpan;
            char charValue;

            if (bool.TryParse(str, out boolValue))
                return typeof(bool);
            //else if (byte.TryParse(str, out byteValue))
            //    return typeof(byte);            
            //else if (Int16.TryParse(str, out int16Value))
            //    return typeof(Int16);
            //else if (Int32.TryParse(str, out int32Value))
            //    return typeof(Int32);
            //else if (Int64.TryParse(str, out int64Value))
            //    return typeof(Int64);
            else if (double.TryParse(str, out doubleValue))
                return typeof(double);
            else if (DateTime.TryParse(str, out dateValue))
                return typeof(DateTime);
            else if (decimal.TryParse(str, out decimalValue))
                return typeof(decimal);
            else if (Guid.TryParse(str, out guidValue))
                return typeof(Guid);
            else if (TimeSpan.TryParse(str, out timeSpan))
                return typeof(TimeSpan);
            else if (char.TryParse(str, out charValue))
                return typeof(char);

            else return typeof(string);
        }

        private static void ChangeColumnDataType(DataTable table, DataColumn column, Type newtype)
        {
            if (!table.Columns.CanRemove(column))
                return;

            var newcolumn = new DataColumn("temp", newtype);
            table.Columns.Add(newcolumn);

            try
            {                
                foreach (DataRow row in table.Rows)
                {
                    row["temp"] = StiConvert.ChangeType(row[column], newtype);
                }

                var indexColumn = table.Columns.IndexOf(column);
                table.Columns.Remove(column);
                
                newcolumn.ColumnName = column.ColumnName;
                table.Columns[newcolumn.ColumnName].SetOrdinal(indexColumn);
            }
            catch (Exception)
            {
                if (table.Columns.Contains("temp") && table.Columns.Contains(newcolumn.ColumnName))
                {
                    table.Columns.Remove(newcolumn);
                }
            }
        }

        private static void ParseJToken(DataSet dataSet, string tokenName, JToken token, DataTable tableProperties = null, DataRow row = null)
        {
            if (token is JContainer)
            {
                ParseJContainer(dataSet, tokenName, token as JContainer, tableProperties, row);
            }
            else if (token is JValue)
            {
                ParseJValue(dataSet, token as JValue);
            }
        }

        private static void ParseJValue(DataSet dataSet, JValue jValue)
        {
            if (mainDataTable != null) return;

            var columnValue = new DataColumn("Value", typeof(string));

            var table = new DataTable();

            table.Columns.Add(columnValue);

            var row = table.NewRow();
            row["Value"] = jValue.Value;
            table.Rows.Add(row);

            table.PrimaryKey = new DataColumn[] { table.Columns[0] };

            dataSet.Tables.Add(table);
        }

        private static void ParseJContainer(DataSet dataSet, string tokenName, JContainer jContainer, DataTable tableProperties = null, DataRow rowProperties = null)
        {
            if (jContainer is JArray)
            {
                ParseJArray(dataSet, tokenName, jContainer as JArray);
            }
            else if (jContainer is JConstructor)
            {

            }
            else if (jContainer is JObject)
            {
                ParseJObject(dataSet, tokenName, jContainer as JObject);
            }
            else if (jContainer is JProperty)
            {
                ParseJProperty(dataSet, jContainer as JProperty, tableProperties, rowProperties);
            }
        }

        private static void ParseJArray(DataSet dataSet, string tokenName, JArray jArray)
        {
            if (jArray.Count == 0) return;

            var table = new DataTable();
            var hashChildTables = new Hashtable();
            
            if (TryParseArrayToTable(jArray, out table, out hashChildTables))
            {
                if (mainDataTable != null)
                {
                    ParseArrayToTable(jArray, mainDataTable);
                }
                else
                {   
                    table.TableName = GetTableName(dataSet, tokenName);
                    dataSet.Tables.Add(table);

                    if (hashChildTables.Count > 0)
                    {
                        foreach (string key in hashChildTables.Keys)
                        {
                            var jArrayChild = hashChildTables[key] as JArray;

                            var childTable = new DataTable();
                            childTable.TableName = GetTableName(dataSet, key);
                            ParseArrayToTable(jArrayChild, childTable);

                            var relationName = string.Format("relation{0}_{1}", table.TableName, childTable.TableName);
                            dataSet.Tables.Add(childTable);
                            var childColumn = childTable.Columns["relationId"];
                            if (childColumn != null)
                            {
                                dataSet.Relations.Add(relationName, table.Columns[key], childColumn);
                            }
                        }
                    }
                }
            }
            else if (CheckArrayForSimpleCreateTable(jArray))//Example JSON: [[a],[b]]
            {
                if (mainDataTable == null)
                    dataSet.Tables.Add(table);

                foreach (JToken jToken in jArray)
                {
                    var childArray = jToken as JArray;

                    while (table.Columns.Count < childArray.Count)
                    {
                        table.Columns.Add(new DataColumn());
                    }

                    var row = table.NewRow();

                    for (int indexRowValue = 0; indexRowValue < childArray.Count; indexRowValue++)
                    {
                        row[indexRowValue] = childArray[indexRowValue];
                    }
                    table.Rows.Add(row);
                }
            }
        }

        private static bool CheckArrayForSimpleCreateTable(JArray jArray)
        {
            foreach (JToken jToken in jArray)
            {
                if (!(jToken is JArray))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryParseArrayToTable(JArray array, out DataTable table, out Hashtable hashChildTables)
        {
            table = new DataTable();
            hashChildTables = new Hashtable();

            if (array.Count > 0 && array[0] is JValue)
            {
                #region Check when all Jvalue
                table.Columns.Add(new DataColumn("Value"));
                
                for (int index = 0; index < array.Count; index++)
                {
                    var jValue = array[index] as JValue;
                    if (jValue != null)
                    {
                        var row = table.NewRow();
                        row[0] = jValue.Value;

                        table.Rows.Add(row);
                    }
                    else
                    {
                        return false;
                    }
                }
                #endregion
            }
            else
            {
                for(var index = 0; index < array.Count; index++)
                { 
                    var jObject = array[index] as JObject;
                    if (jObject != null)
                    {
                        var row = table.NewRow();

                        foreach (JProperty property in jObject.Children())
                        {
                            if (property != null)
                            {
                                if (!table.Columns.Contains(property.Name))
                                {
                                    table.Columns.Add(new DataColumn(property.Name));
                                }

                                var indexColumn = table.Columns.IndexOf(property.Name);

                                var valueJArray = property.Value as JArray;
                                if (valueJArray != null && ValidJArrayStruct(valueJArray))
                                {
                                    row[indexColumn] = index;

                                    FillRelationIdInJArray("relationId", index, valueJArray);

                                    #region Add Child Table
                                    if (hashChildTables.ContainsKey(property.Name))
                                    {
                                        var jArray = hashChildTables[property.Name] as JArray;
                                        hashChildTables[property.Name] = UnitJArray(jArray, valueJArray);
                                    }
                                    else
                                    {
                                        hashChildTables.Add(property.Name, valueJArray);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    row[indexColumn] = property.Value;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }

                        table.Rows.Add(row);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }

        private static JArray UnitJArray(JArray jArray1, JArray jArray2)
        {
            for (var index = 0; index < jArray2.Count; index++)
            {
                var jObject = jArray2[index] as JObject;
                if (jObject != null)
                {
                    jArray1.Add(jObject);
                }
            }

            return jArray1;
        }

        private static void FillRelationIdInJArray(string relationName, int relationId, JArray jArray)
        {
            for (var index = 0; index < jArray.Count; index++)
            {
                var jObject = jArray[index] as JObject;
                if (jObject != null)
                {
                    jObject.Add(new JProperty(relationName, relationId));
                }
            }
        }

        private static bool ValidJArrayStruct(JArray jArray)
        {
            for (var index = 0; index < jArray.Count; index++)
            {
                var jObject = jArray[index] as JObject;
                if (jObject != null)
                {
                    foreach (JProperty property in jObject.Children())
                    {
                        if (property == null)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        
        private static void ParseArrayToTable(JArray array, DataTable table)
        {
            if (array.Count > 0 && array[0] is JValue)
            {
                #region Check when all Jvalue
                table.Columns.Add(new DataColumn("Value"));

                for (int index = 0; index < array.Count; index++)
                {
                    var jValue = array[index] as JValue;
                    if (jValue != null)
                    {
                        var row = table.NewRow();
                        row[0] = jValue.Value;

                        table.Rows.Add(row);
                    }
                }
                #endregion
            }
            else
            {
                foreach (JToken jToken in array)
                {
                    var jObject = jToken as JObject;
                    if (jObject != null)
                    {
                        var row = table.NewRow();

                        foreach (JToken toKen in jObject.Children())
                        {
                            var property = toKen as JProperty;

                            if (property != null)
                            {
                                if (!table.Columns.Contains(property.Name))
                                {
                                    table.Columns.Add(new DataColumn(property.Name));
                                }

                                var indexRow = table.Columns.IndexOf(property.Name);

                                var dateTime = ParseJTokenToDateTime(property.Value);

                                row[indexRow] = dateTime!= null? dateTime.GetValueOrDefault() : property.Value;
                            }
                        }

                        table.Rows.Add(row);
                    }
                }
            }
        }

        private static void ParseJProperty(DataSet dataSet, JProperty jProperty, DataTable tableProperties, DataRow rowProperties = null)
        {
            if (jProperty.Value is JValue)
            {
                if (tableProperties == null)
                {
                    tableProperties = mainDataTable != null ? mainDataTable : new DataTable() { TableName = jProperty.Name };

                    if (mainDataTable == null)
                        dataSet.Tables.Add(tableProperties);

                    if (StiBaseOptions.JsonParseCreatingTableProperties) {
                        if (!tableProperties.Columns.Contains("Name"))
                            tableProperties.Columns.Add("Name");

                        if (!tableProperties.Columns.Contains("Value"))
                            tableProperties.Columns.Add("Value");
                    }                    
                }

                if (StiBaseOptions.JsonParseCreatingTableProperties)
                {
                    var row = tableProperties.NewRow();
                    row[0] = jProperty.Name;
                    row[1] = jProperty.Value;

                    tableProperties.Rows.Add(row);
                }
                else
                {
                    tableProperties.Columns.Add(new DataColumn(jProperty.Name));

                    var indexRow = tableProperties.Columns.IndexOf(jProperty.Name);

                    var dateTime = ParseJTokenToDateTime(jProperty.Value);

                    rowProperties[indexRow] = dateTime != null ? dateTime.GetValueOrDefault() : jProperty.Value;
                }
            }
            else
            {
                ParseJToken(dataSet, jProperty.Name, jProperty.Value, tableProperties);
            }
        }

        private static void ParseJObject(DataSet dataSet, string tokenName, JObject jObject)
        {
            var tableProperties = mainDataTable != null? mainDataTable: new DataTable(){TableName = GetTableName(dataSet, tokenName) };
            DataRow rowProperties = null;
            if (StiBaseOptions.JsonParseCreatingTableProperties)
            {
                if (!tableProperties.Columns.Contains("Name"))
                    tableProperties.Columns.Add("Name");

                if (!tableProperties.Columns.Contains("Value"))
                    tableProperties.Columns.Add("Value");
            }
            else
            {
                rowProperties = tableProperties.NewRow();
            }
            
            if (mainDataTable == null)
                dataSet.Tables.Add(tableProperties);

            for (int index = 0; index < jObject.Count; index++)
            {
                ParseJToken(dataSet, null, jObject.Children().ToArray()[index], tableProperties, rowProperties);
            }

            if (!StiBaseOptions.JsonParseCreatingTableProperties)
                tableProperties.Rows.Add(rowProperties);
        }

        //Specifically parse for DateTime to work with MongoDB. Artem
        private static DateTime? ParseJTokenToDateTime(JToken token)
        {
            var value = token.ToString();

            if (value.StartsWith("{\r\n  \"$date\": ") && value.EndsWith("\r\n}"))
            {
                var valueNew = value.Replace("{\r\n  \"$date\": ", "");
                valueNew = valueNew.Replace("\r\n}", "");

                double valueDouble;

                if (double.TryParse(valueNew, out valueDouble))
                {
                    var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
                    var dateTime = posixTime.AddMilliseconds(valueDouble);

                    return dateTime;                
                }
            }

            return null;
        }

        private static string GetTableName(DataSet dataSet, string tableName)
        {
            if (dataSet.Tables.Contains(tableName))
            {
                if (hashDublicateTableName == null) hashDublicateTableName = new Hashtable();

                if (hashDublicateTableName.ContainsKey(tableName))
                {
                    var valueNew = (int)hashDublicateTableName[tableName] + 1;
                    hashDublicateTableName[tableName] = valueNew;
                }
                else
                {
                    hashDublicateTableName.Add(tableName, 1);
                }

                return string.Format("{0}{1}", tableName, hashDublicateTableName[tableName].ToString());
            }

            return tableName;
        }
        
        #endregion
    }
}
