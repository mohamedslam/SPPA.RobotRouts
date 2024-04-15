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

using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using System;
using System.Data;
using System.Linq;

namespace Stimulsoft.Base
{
    public static class StiGisToDataSetConverter
    {
        #region Methods
        public static DataSet GetDataSetFromWkt(byte[] content, string separator)
        {
            var str = StiBytesToStringConverter.ConvertBytesToString(content);
            return GetDataSetFromWkt(str, separator);
        }

        public static DataSet GetDataSetFromWkt(string text, string separator)
        {
            if (text == null)
                return null;

            var lines = text.Split(Environment.NewLine.ToCharArray());
            if (lines.Length == 0) return new DataSet();

            var table = new DataTable();

            var separatorCharArray = separator.ToCharArray();

            #region Read file
            for (int index = 0; index < lines.Length; index++)
            {
                var line = lines[index];

                #region Read header line
                if (index == 0)
                {
                    var headers = line.Split(separatorCharArray);
                    foreach (var header in headers)
                    {
                        var columnValue = new DataColumn(header, typeof(string));
                        table.Columns.Add(columnValue);
                    }
                }
                #endregion
                else
                #region Read data line
                {
                    if (string.IsNullOrEmpty(line)) continue;

                    var datas = line.Split(separatorCharArray);
                    int index1 = 0;

                    var row = table.NewRow();
                    foreach (var data in datas)
                    {
                        row[index1] = data;
                        index1++;
                    }

                    table.Rows.Add(row);
                }
                #endregion
            }
            #endregion

            var dataSet = new DataSet();
            dataSet.Tables.Add(table);
            return dataSet;
        }

        public static DataSet GetDataSetFromGeoJson(byte[] content)
        {
            var str = StiBytesToStringConverter.ConvertBytesToString(content);

            var jObject = JsonConvert.DeserializeObject(str) as JObject;
            return GetDataSetFromGeoJson(jObject);
        }

        public static DataSet GetDataSetFromGeoJson(JObject jObject)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add(new DataColumn("geometry", typeof(string)));

            var rootItems = jObject.Children().ToList();
            var typeToken = rootItems.FirstOrDefault(x => x is JProperty && ((JProperty)x).Name == "type");
            var featuresToken = rootItems.FirstOrDefault(x => x is JProperty && ((JProperty)x).Name == "features");

            if (typeToken == null || featuresToken == null)
            {
                throw new Exception("Data not correct");
            }

            // get items in "features"
            var dataItems = featuresToken.Children().ToArray();
            var dataTockens = dataItems[0].Children();

            foreach (JToken tocken in dataTockens)
            {
                var row = table.NewRow();

                bool skipRow = false;
                var items = tocken.Children().ToArray();
                for (int index = 0; index < items.Length; index++)
                {
                    var item = items[index] as JProperty;

                    #region type
                    if (item.Name == "type")
                    {
                        if (item.HasValues && item.Value.ToString() != "Feature")
                        {
                            skipRow = true;
                            break;
                        }

                        continue;
                    }
                    #endregion

                    #region geometry
                    if (item.Name == "geometry")
                    {
                        if (item.HasValues)
                        {
                            var geomStr = item.Value.ToString().Replace(Environment.NewLine, "").Replace(" ", "").Trim();
                            row["geometry"] = geomStr;
                        }

                        continue;
                    }
                    #endregion

                    #region properties
                    if (item.Name == "properties")
                    {
                        foreach (JObject propObject in item.Children())
                        {
                            if (propObject != null)
                            {
                                foreach (JProperty prop in propObject.Children())
                                {
                                    if (prop != null)
                                    {
                                        if (prop.HasValues)
                                        {
                                            if (!table.Columns.Contains(prop.Name))
                                                table.Columns.Add(new DataColumn(prop.Name, typeof(string)));

                                            row[prop.Name] = prop.Value.ToString();
                                        }
                                    }
                                }
                            }
                        }

                        continue;
                    }
                    #endregion
                }

                if (!skipRow)
                    table.Rows.Add(row);
            }

            dataSet.Tables.Add(table);
            return dataSet;
        }

        private static bool IsCorrectJToken(JToken token, string name)
        {
            var jProperty = token as JProperty;
            if (jProperty == null) return false;
            return jProperty.Name == name;
        }
        #endregion
    }
}