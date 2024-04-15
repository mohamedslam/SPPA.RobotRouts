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

using System.Collections;
using System.Data;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using Stimulsoft.Base.Helpers;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Reflection;

namespace Stimulsoft.Base
{
    public static class StiJsonToDataSetConverterV2
    {
        private class StiJsonMetaData
        {
            #region Properties
            public string CollectionName { get; set; }
            public string Address { get; set; }
            public string Cast { get; set; }
            public JToken Object { get; set; }
            #endregion
        }

        private class StiJsonRelationData
        {
            #region Properties
            public DataColumn ChildColumn { get; set; }
            public DataTable ChildTable { get; set; }
            public string ParentTableName { get; set; }
            #endregion
        }

        #region Methods
        public static DataSet GetDataSet(JToken json)
        {
            return GetDataSet(json, StiRelationDirection.ParentToChild);
        }

        public static DataSet GetDataSet(JToken json, StiRelationDirection relationDirection)
        {
            json = CorrectJson(json);

            var collections = new List<StiJsonMetaData>();

            FillCollection(collections, "", "", "", json, true);

            var addresses = new Hashtable();
            var tables = new Hashtable();
            var relations = new Hashtable();
            var lastIndexesForAddress = new Hashtable();
            var allRelationsColumn = new List<DataColumn>();

            foreach (StiJsonMetaData collection in collections)
            {
                if (collection == null) continue;
                var table = tables[collection.Cast] as List<StiJsonMetaData>;

                if (table == null) table = new List<StiJsonMetaData>();
                lastIndexesForAddress[table] = 0;
                addresses[collection.Address] = table;
                table.Add(collection);
                tables[collection.Cast] = table;
                var relationId = collection.Object["relationId"]?.ToString();
                if (relationId != null && relationId.IndexOf("#relation#") == 0)
                {
                    var relationParentCast = relationId.Replace("#relation#", "");
                    if (tables.ContainsKey(relationParentCast)) collection.Object["relationId"] = ((List<StiJsonMetaData>)tables[relationParentCast]).Count.ToString();
                    else collection.Object["relationId"] = "0";
                }
            }

            collections = null;

            var dataSet = new DataSet();

            foreach (List<StiJsonMetaData> table in tables.Values)
            {
                var dataTableRowsCount = 0;
                foreach (StiJsonMetaData item in table)
                {
                    var tableName = item.CollectionName;
                    var dataTable = dataSet.Tables[tableName];
                    if (dataTable == null)
                    {
                        dataTable = new DataTable(tableName);
                        dataSet.Tables.Add(dataTable);
                    }

                    var itemDataHelp = item.Object["relationId"]?.ToString();

                    var dataRow = dataTable.NewRow();
                    dataTableRowsCount++;
                    var isEmptyData = dataTable.Rows.Count == 0;
                    foreach (JProperty columnName in (JContainer)item.Object)
                    {
                        try
                        {
                            var dataColumn = dataTable.Columns[columnName.Name];
                            if (dataColumn == null)
                            {
                                var dataColumnType = typeof(string);
                                if (columnName.Name != "relationId")
                                {
                                    if (columnName.Value.Type == JTokenType.Integer) dataColumnType = typeof(decimal);
                                    else if (columnName.Value.Type == JTokenType.Float) dataColumnType = typeof(decimal);
                                    else if (columnName.Value.Type == JTokenType.Boolean) dataColumnType = typeof(bool);
                                    else if (columnName.Value.Type == JTokenType.Date) dataColumnType = typeof(DateTime);
                                }
                                dataColumn = new DataColumn(columnName.Name, dataColumnType);
                                dataTable.Columns.Add(dataColumn);
                            }

                            var columnValue = ((JValue)columnName.Value).Value;
                            if (columnName.Name == "relationId" && itemDataHelp != "")
                            {
                                var dataColumnRalationId = dataTable.Columns["relationId"];
                                if (dataColumnRalationId == null)
                                {
                                    dataColumnRalationId = new DataColumn("relationId");
                                    dataTable.Columns.Add(dataColumnRalationId);
                                    allRelationsColumn.Add(dataColumnRalationId);
                                }

                                if (itemDataHelp == "-1")
                                {
                                    itemDataHelp = "0";

                                    var table1 = addresses[item.Address] as List<StiJsonMetaData>;
                                    for (int index = (int)lastIndexesForAddress[table1]; index < table1.Count; index++)
                                    {
                                        var item1 = table1[index];

                                        if (item1.Address == item.Address)
                                        {
                                            itemDataHelp = index.ToString();
                                            lastIndexesForAddress[table1] = index;
                                            break;
                                        }
                                    }
                                }
                                item.Object["relationId"] = itemDataHelp;
                                dataRow[dataColumnRalationId] = itemDataHelp;
                            }
                            else if (dataColumn.DataType == typeof(string) && columnName.Value.Type == JTokenType.String && columnValue != null && ((string)columnValue).IndexOf("#relation#") == 0)
                            {
                                isEmptyData = false;
                                dataRow[dataColumn] = (dataTableRowsCount - 1).ToString();
                                var address = ((string)columnValue).Replace("#relation#", "");
                                if (addresses.ContainsKey(address))
                                {
                                    var list = addresses[address] as List<StiJsonMetaData>;
                                    if (list.Count > 0)
                                    {
                                        var dataHelpRelation = new StiJsonRelationData()
                                        {
                                            ChildColumn = dataColumn,
                                            ChildTable = dataTable,
                                            ParentTableName = list[0].CollectionName
                                        };

                                        relations[$"{dataTable.TableName}.{dataColumn.ColumnName}"] = dataHelpRelation;
                                    }
                                }
                            }
                            else
                            {
                                isEmptyData = false;
                                if (columnValue == null)
                                {
                                    dataRow[dataColumn] = DBNull.Value;
                                }
                                else
                                {
                                    if (dataColumn.DataType == typeof(object))
                                        dataRow[dataColumn] = columnValue;
                                    else
                                    {
                                        try
                                        {
                                            var valueType = columnValue.GetType();
                                            if (valueType == dataColumn.DataType || valueType.IsNumericType() && dataColumn.DataType.IsNumericType())
                                            {
                                                dataRow[dataColumn] = columnValue;
                                            }
                                            else if (valueType == typeof(DateTime) && dataColumn.DataType == typeof(string))
                                            {
                                                dataRow[dataColumn] = ((DateTime)columnValue).ToString("o", CultureInfo.InvariantCulture);
                                            }
                                            else
                                            {
                                                dataRow[dataColumn] = StiConvert.ChangeType(columnValue, dataColumn.DataType);
                                            }
                                        }
                                        catch
                                        {
                                            dataRow[dataColumn] = DBNull.Value;
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (!isEmptyData)
                        dataTable.Rows.Add(dataRow);
                }
            }

            tables = null;

            foreach (StiJsonRelationData relation in relations.Values)
            {
                var childColumn = relation.ChildColumn;
                var parentTable = dataSet.Tables[relation.ParentTableName];
                var parentColumn = parentTable.Columns["relationId"];

                if (parentColumn != null && childColumn != null)
                {
                    try
                    {
                        if (relationDirection == StiRelationDirection.ChildToParent)
                            dataSet.Relations.Add(parentTable.TableName, parentColumn, childColumn);
                        else
                            dataSet.Relations.Add(parentTable.TableName, childColumn, parentColumn);
                    }
                    catch
                    {
                    }

                    for (var index = 0; index < allRelationsColumn.Count; index++)
                    {
                        if (allRelationsColumn[index] == parentColumn)
                        {
                            allRelationsColumn.RemoveAt(index);
                            break;
                        }
                    }
                }
            }

            foreach (DataColumn deleteColumn in allRelationsColumn)
            {
                try
                {
                    if (deleteColumn.Table.Columns.Contains(deleteColumn.ColumnName) && deleteColumn.Table.Columns.CanRemove(deleteColumn))
                        deleteColumn.Table.Columns.Remove(deleteColumn);
                }
                catch
                {
                }
            }

            //StiJsonToDataSetConverter.CheckColumnType(dataSet);

            return dataSet;
        }

        public static DataSet GetDataSet(byte[] content)
        {
            return GetDataSet(content, StiRelationDirection.ParentToChild);
        }

        public static DataSet GetDataSet(byte[] content, StiRelationDirection relationDirection)
        {
            var str = StiBytesToStringConverter.ConvertBytesToString(content);
            return GetDataSet(str, relationDirection);
        }

        public static DataSet GetDataSet(string text)
        {
            return GetDataSet(text, StiRelationDirection.ParentToChild);
        }

        public static DataSet GetDataSet(string text, StiRelationDirection relationDirection)
        {
            if (text == null)
                return null;

            JsonSerializerSettings serializerSettings = null;
            if (!Stimulsoft.Base.StiBaseOptions.TryParseDateTime)
                serializerSettings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };

            var jToken = JsonConvert.DeserializeObject(text, serializerSettings) as JToken;

            return GetDataSet(jToken, relationDirection);
        }

        public static DataSet GetDataSet(XElement element)
        {
            return GetDataSet(element, StiRelationDirection.ParentToChild);
        }

        public static DataSet GetDataSet(XElement element, StiRelationDirection relationDirection)
        {
            var text = JsonConvert.SerializeXNode(element, Formatting.Indented);

            JsonSerializerSettings serializerSettings = null;
            if (!Stimulsoft.Base.StiBaseOptions.TryParseDateTime)
                serializerSettings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };

            var jToken = JsonConvert.DeserializeObject(text, serializerSettings) as JToken;

            return GetDataSet(jToken, relationDirection);
        }

        public static DataSet GetDataSetFromXml(byte[] array)
        {
            return GetDataSetFromXml(array, StiRelationDirection.ParentToChild);
        }

        public static DataSet GetDataSetFromXml(byte[] array, StiRelationDirection relationDirection)
        {
            if (array != null)
            {
                string text;
                using (var stream = new MemoryStream(array))
                {
                    var element = XElement.Load(stream);
                    text = JsonConvert.SerializeXNode(element, Formatting.Indented);
                }

                JsonSerializerSettings serializerSettings = null;
                if (!Stimulsoft.Base.StiBaseOptions.TryParseDateTime)
                    serializerSettings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };

                var jToken = JsonConvert.DeserializeObject(text, serializerSettings) as JToken;
                return GetDataSet(jToken, relationDirection);
            }

            return null;
        }

        public static DataSet GetDataSetFromFile(string path)
        {
            return GetDataSetFromFile(path, StiRelationDirection.ParentToChild);
        }

        public static DataSet GetDataSetFromFile(string path, StiRelationDirection relationDirection)
        {
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                JsonSerializerSettings serializerSettings = null;
                if (!Stimulsoft.Base.StiBaseOptions.TryParseDateTime)
                    serializerSettings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };

                var jToken = JsonConvert.DeserializeObject(text, serializerSettings) as JToken;
                return GetDataSet(jToken, relationDirection);
            }

            return null;
        }

        private static void CorrectArray(JToken json)
        {
            var changedItems = new Dictionary<JToken, JToken>();
            if (json is JArray jsonArray) {
                for (var index = 0; index < jsonArray.Count; index++) 
                //foreach (var item in jsonArray)
                {
                    var item = jsonArray[index];
                    if (item is JValue itemValue)
                    {
                        jsonArray[index] = new JObject(new JProperty("value", itemValue.Value));
                        //changedItems.Add(itemValue,  new JObject(new JProperty("value", itemValue.Value)));
                    }
                    else CorrectArray(item);
                }
            }
            else if (json is JContainer)
            {
                foreach (var item in json)
                {
                    if (item is JProperty property)
                    {
                        if (property.Value.HasValues)
                        {
                            CorrectArray(property.Value);

                            if (property.Value.Type != JTokenType.Array)
                            {
                                changedItems.Add(property, new JProperty(property.Name, new JArray { property.Value }));
                            }
                        }
                        else if (property.Value.Type == JTokenType.Object)
                        {
                            changedItems.Add(property, new JProperty(property.Name, new JArray()));
                        }
                    }
                }
            }

            foreach (var item in changedItems)
            {
                item.Key.Replace(item.Value);
            }
        }

        private static JToken CorrectJson(JToken jToken)
        {
            CorrectArray(jToken);

            var jContainer = jToken as JContainer;

            if (jContainer is JArray)
            {
                var root = new JProperty("root");
                root.Value = jToken;

                return root;
            }

            foreach (var item in jContainer)
            {
                var itemJArray = item as JArray;
                var itemJObject = item as JObject;
                var itemJProperty = item as JProperty;

                if (itemJProperty != null && !(itemJProperty.Value is JContainer))
                {
                    var root = new JProperty("root");
                    root.Value = jToken;

                    return root;
                }
            }

            return jToken;
        }

        private static string CorrectJsonString(string json)
        {
            string newJson = json;

            int pos = 0;
            while (pos < newJson.Length)
            {
                if (newJson[pos] == '{') return newJson;
                else if (newJson[pos] == '[') return "{\"root\": " + newJson + "}";
                pos++;
            }

            return newJson;
        }

        private static void FillCollection(List<StiJsonMetaData> collections, string collectionName, string parentName, string address, JToken json, bool first = false)
        {
            var cast = $"{parentName}_{collectionName}";
            if (string.IsNullOrEmpty(parentName)) cast = collectionName;
            var relationCast = $"#relation#{cast}";

            var property = json as JProperty;
            var array = json as JArray;
            var itemName = property?.Name;
            var name = 0;
            var replaceItems = new Dictionary<JToken, JToken>();
            foreach (var item in json)
            {
                JToken itemValue = null;
                var itemJProperty = item as JProperty;
                if (itemJProperty == null)
                {
                    if (property == null) itemName = name.ToString();
                    itemValue = item;
                }
                else
                {
                    if (property == null) itemName = itemJProperty.Name;
                    itemValue = itemJProperty.Value;
                }

                var itemJArray = item as JArray;
                var itemJObject = item as JObject;
                var itemValueJArray = itemValue as JArray;

                if (itemJArray != null || itemJObject != null || itemJProperty != null && (itemJProperty.Value.Type == JTokenType.Array || itemJProperty.Value.Type == JTokenType.Object))
                {
                    var isSetRelation = false;
                    var addressItemName = $"{address}.{itemName}";
                    if (array != null)
                    {
                        FillCollection(collections, collectionName, parentName, $"{address}.#array#{name}", item);
                    }
                    else
                    {
                        if (!first && itemValueJArray != null)
                        {
                            if (itemValueJArray.Count == 0)
                                itemValueJArray.Add(new JObject());

                            foreach (var itemItem in itemValueJArray)
                            {
                                if (itemItem is JObject itemItemObject)
                                    itemItemObject.Add("relationId", relationCast);
                            }
                        }

                        FillCollection(collections, itemName, cast, addressItemName, itemValue);
                        if (itemValueJArray == null)
                        {
                            itemJObject.Add("relationId", "-1");
                        }
                        else
                        {
                            if (property == null)
                                replaceItems.Add(item, new JProperty(itemName, $"#relation#{addressItemName}.#array#0"));
                            else
                                replaceItems.Add(item, new JValue($"#relation#{addressItemName}.#array#0"));

                            isSetRelation = true;
                        }
                    }
                    if (!isSetRelation && itemValueJArray == null)
                    {
                        replaceItems.Add(item, new JValue($"#relation#{address}.{itemName}"));
                    }
                }

                name++;
            }

            foreach (var item in replaceItems)
            {
                item.Key.Replace(item.Value);
            }

            if (array != null)
            {
                if (array.Count == 0) collections.Add(new StiJsonMetaData() { CollectionName = collectionName, Address = address + ".#array#0", Cast = cast, Object = new JObject() });
                return;
            }
            if (!string.IsNullOrEmpty(address))
            {
                if (!string.IsNullOrEmpty(parentName)) collectionName = $"{parentName}_{collectionName}";
                collections.Add(new StiJsonMetaData() { CollectionName = collectionName, Address = address, Cast = cast, Object = json });
            }
        }
        #endregion
    }
}
