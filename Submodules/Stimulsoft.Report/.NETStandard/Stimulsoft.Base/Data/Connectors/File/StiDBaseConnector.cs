﻿#region Copyright (C) 2003-2022 Stimulsoft
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
    public class StiDBaseConnector : StiFileDataConnector
    {
        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.DBaseDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.DBaseDataSource;

        public override string Name => "DBase";

        /// <summary>
        /// Get a value which indicates that this data connector can be used now.
        /// </summary>
        public override bool IsAvailable => true;

        /// <summary>
        /// A type of the file which can be processed with this connection helper.
        /// </summary>
        public override StiFileType FileType => StiFileType.Dbf;
        #endregion

        #region Methods
        /// <summary>
        /// Returns DataSet based on specified options.
        /// </summary>
        public override DataSet GetDataSet(StiFileDataOptions options)
        {
            var dBaseOptions = options as StiDBaseOptions;
            if (dBaseOptions == null) 
                throw new NotSupportedException("Only StiDBaseOptions accepted!");

            if (!string.IsNullOrWhiteSpace(dBaseOptions.Path) && dBaseOptions.Content == null)
                options.DataSet = GetDataSetFromPath(dBaseOptions);

            else if (dBaseOptions.Content == null || dBaseOptions.Content.Length == 0)
                options.DataSet = null;

            else
                options.DataSet = StiDBaseHelper.GetDataSet(dBaseOptions.Content, dBaseOptions.TableName, false, dBaseOptions.CodePage);

            return options.DataSet;
        }

        private DataSet GetDataSetFromPath(StiDBaseOptions options)
        {
            DataSet dataSet = null;

            var datas = StiDataLoaderHelper.LoadMultiple(options.Path, "*.dbf");
            if (datas != null)
            {
                foreach (var data in datas)
                {
                    try
                    {
                        var dataTable = StiDBaseHelper.GetTable(data.Array, options.CodePage, options.MaxDataRows);
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
        public static StiDBaseConnector Get()
        {
            return new StiDBaseConnector();
        }
        #endregion

        private StiDBaseConnector()
        {
        }
    }
}