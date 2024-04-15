#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System.Data;

namespace Stimulsoft.Base
{
    public class StiCsvConnector : StiFileDataConnector
    {
        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.CsvDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.CsvDataSource;

        public override string Name => "CSV";

        /// <summary>
        /// Get a value which indicates that this data connector can be used now.
        /// </summary>
        public override bool IsAvailable => true;

        /// <summary>
        /// A type of the file which can be processed with this connection helper.
        /// </summary>
        public override StiFileType FileType => StiFileType.Csv;
        #endregion

        #region Methods
        /// <summary>
        /// Returns DataSet based on specified options.
        /// </summary>
        public override DataSet GetDataSet(StiFileDataOptions options)
        {
            var csvOptions = options as StiCsvOptions;
            if (csvOptions == null)
                throw new NotSupportedException("Only StiCsvOptions accepted!");

            if (!string.IsNullOrWhiteSpace(csvOptions.Path) && csvOptions.Content == null)
                options.DataSet = GetDataSetFromPath(csvOptions);

            else if (csvOptions.Content == null || csvOptions.Content.Length == 0)
                options.DataSet = null;

            else 
                options.DataSet = StiCsvHelper.GetDataSet(csvOptions.Content, csvOptions.TableName, csvOptions.CodePage, csvOptions.Separator, csvOptions.MaxDataRows);

            return options.DataSet;
        }

        private DataSet GetDataSetFromPath(StiCsvOptions options)
        {
            DataSet dataSet = null;

            var datas = StiDataLoaderHelper.LoadMultiple(options.Path, "*.csv");
            if (datas != null)
            {
                foreach (var data in datas)
                {
                    try
                    {
                        var dataTable = StiCsvHelper.GetTable(data.Array, options.CodePage, options.Separator, options.MaxDataRows);
                        if (dataTable == null) continue;

                        if (dataSet == null)
                            dataSet = new DataSet { EnforceConstraints = false };

                        dataTable.TableName = data.Name;
                        dataSet.Tables.Add(dataTable);
                    }
                    catch
                    {
                    }
                }
            }
            return dataSet;
        }
        #endregion

        #region Methods.Static
        public static StiCsvConnector Get()
        {
            return new StiCsvConnector();
        }
        #endregion

        private StiCsvConnector()
        {
        }
    }
}