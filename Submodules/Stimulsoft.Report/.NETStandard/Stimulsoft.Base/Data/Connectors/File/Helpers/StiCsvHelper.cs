#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
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
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using Stimulsoft.Base.Exceptions;

namespace Stimulsoft.Base
{
    public class StiCsvHelper
    {
        #region Constants CodePageCodes
        public static int[] CodePageCodes = 
		{
			0,
			1,
			65000,
			65001,
			1200,
			1250,
			1251,
			1252,
			1253,
			1254,
			1255,
			1256
		};

        public static string[] CodePageNames =
		{
			"Default",
			"System",
			"UTF7",
			"UTF8",
			"Unicode",
			"1250",
			"1251",
			"1252",
			"1253",
			"1254",
			"1255",
			"1256"
		};
        #endregion

        #region Methods
		/// <summary>
        /// Convert table from CSV file.
		/// </summary>
        public static DataTable GetTable(string path, int maxDataRows = 0)
		{
            return GetTable(path, 0, null, maxDataRows);
		}

        /// <summary>
        /// Convert table from CSV file.
        /// </summary>
        public static DataTable GetTable(string path, int codePage, string separator, int maxDataRows = 0)
        {
            return GetTable(File.ReadAllBytes(path), codePage, separator, maxDataRows);
        }

        /// <summary>
        /// Convert table from CSV file.
        /// </summary>
        public static DataTable GetTable(byte[] data, int maxDataRows = 0)
        {
            return GetTable(data, 0, null, maxDataRows);
        }

        /// <summary>
        /// Convert dataset from bytes stored in CSV format.
        /// </summary>
        public static DataSet GetDataSet(byte[] data, string tableName, int codePage, string separator, int maxDataRows = 0)
        {
            var dataTable = GetTable(data, codePage, separator, maxDataRows);
            if (dataTable == null) 
                return null;

            dataTable.TableName = string.IsNullOrWhiteSpace(tableName) ? StiFileItemTable.DefaultCsvTableName : tableName;

            var dataSet = new DataSet { EnforceConstraints = false };
            dataSet.Tables.Add(dataTable);
            return dataSet;
        }

        /// <summary>
        /// Convert table from bytes stored in CSV format.
        /// </summary>
        public static DataTable GetTable(byte[] data, int codePage, string separator, int maxDataRows = 0)
        {
            return GetTable(data, codePage, separator, true, maxDataRows);
        }

        /// <summary>
        /// Convert table from bytes stored in CSV format.
        /// </summary>
        public static DataTable GetTable(byte[] data, int codePage, string separator, bool loadData, int maxDataRows = 0)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                var listSeparator = !string.IsNullOrEmpty(separator) ? separator : CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                var delimiter = listSeparator[0];

                #region Prepare encoding
                Encoding enc = null;
                if (codePage != 0)
                {
                    enc = codePage == 1 ? Encoding.Default : Encoding.GetEncoding(codePage);
                }
                #endregion

                using (var sr = enc == null ? new StreamReader(memoryStream) : new StreamReader(memoryStream, enc))
                {

                    #region Read columns headers
                    string[] columnsNames = null;
                    var line = sr.ReadLine();
                    if (line != null) //not end of file
                    {
                        if (line.Length > 0)
                        {
                            //get header info
                            columnsNames = SplitToColumns(ref line, sr, delimiter);

                            #region Try to use other separator if we have only one column
                            if (columnsNames.Length == 1)
                            {
                                columnsNames = SplitToColumns(ref line, sr, '\t');
                                if (columnsNames.Length > 1)
                                {
                                    delimiter = '\t';
                                }
                                else
                                {
                                    columnsNames = SplitToColumns(ref line, sr, ';');
                                    if (columnsNames.Length > 1)
                                    {
                                        delimiter = ';';
                                    }
                                    else
                                    {
                                        columnsNames = SplitToColumns(ref line, sr, ',');
                                        if (columnsNames.Length > 1)
                                        {
                                            delimiter = ',';
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region Check columns names
                    if (columnsNames != null)
                    {
                        var hs = new Hashtable();
                        for (var index = 0; index < columnsNames.Length; index++)
                        {
                            var st = columnsNames[index].Trim();
                            if (st.Length == 0) st = string.Format("Column{0}", index + 1);
                            st = StiDataNameValidator.Correct(st);
                            var stNum = string.Empty;
                            var num = 0;
                            while (true)
                            {
                                if (!hs.Contains(st + stNum)) break;
                                num++;
                                stNum = num.ToString();
                            }
                            st += stNum;
                            columnsNames[index] = st;
                            hs.Add(st, st);
                        }
                    }
                    #endregion

                    #region Prepare DataTable
                    var dataTable = new DataTable(StiFileItemTable.DefaultCsvTableName);
                    if (columnsNames != null)
                    {
                        foreach (var columnName in columnsNames)
                        {
                            var columnType = typeof (string);
                            var column = new DataColumn(columnName, columnType);
                            dataTable.Columns.Add(column);
                        }
                    }
                    #endregion

                    #region Read data
                    dataTable.BeginLoadData();
                    if (loadData && columnsNames != null)
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Length == 0) line = new string(delimiter, columnsNames.Length - 1);

                            var dataRow = dataTable.NewRow();
                            var columnsData = SplitToColumns(ref line, sr, delimiter, dataRow.Table.Columns.Count);
                            dataTable.LoadDataRow(columnsData, LoadOption.OverwriteChanges);

                            if (maxDataRows != 0 && maxDataRows < dataTable.Rows.Count) break;
                        }
                    }
                    dataTable.EndLoadData();
                    #endregion

                    return dataTable;
                }
            }
        }

        private static string[] SplitToColumns(ref string inputString, StreamReader sr, char delimiter, int count = 0)
        {
            var strings = new List<string>();
            var sb = new StringBuilder();
            var stPos = 0;
            var posInString = false;

            while (stPos < inputString.Length)
            {
                char sym = inputString[stPos];
                stPos++;

                if (sym == '\"')
                {
                    posInString = !posInString;
                }

                if ((sym == delimiter) && (!posInString))
                {
                    strings.Add(sb.ToString());
                    sb = new StringBuilder();
                    continue;
                }

                sb.Append(sym);
                if ((stPos == inputString.Length) && posInString)
                {
                    string newLine = sr.ReadLine();
                    if (newLine != null)
                    {
                        inputString += '\n' + newLine;
                    }
                }
            }
            strings.Add(sb.ToString());

            if (count == 0) count = strings.Count;
            var output = new string[count];
            for (var index = 0; index < Math.Min(count, strings.Count); index++)
            {
                var st = strings[index];
                if ((st.Length > 1) && (st.StartsWith("\"", StringComparison.InvariantCulture)) && (st.EndsWith("\"", StringComparison.InvariantCulture)))
                {
                    output[index] = st.Substring(1, st.Length - 2).Replace("\"\"", "\"");
                }
                else
                {
                    output[index] = st;
                }
            }

            return output;
        }
        #endregion
    }
}
