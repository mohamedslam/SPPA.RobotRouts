#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System.Collections.Generic;
using Stimulsoft.Report.Dictionary;
using System.Data;

namespace Stimulsoft.Report.Web
{
    internal class StiViewDataHelper
    {
        public DataTable ResultDataTable;
        private StiDataSource dataSource;
        private StiBusinessObject businessObject;        

        public StiViewDataHelper(StiBusinessObject businessObject)
            : this(null, businessObject)
        {
        }

        public StiViewDataHelper(StiDataSource dataSource)
            : this(dataSource, null)
        {
        }

        public StiViewDataHelper(StiDataSource dataSource, StiBusinessObject businessObject)
        {
            this.dataSource = dataSource;
            this.businessObject = businessObject;

            if (dataSource != null && !dataSource.IsConnected)
                dataSource.Dictionary.Connect(false);

            Build();
        }
        
        #region Methods
        private void FillLevel(StiBusinessObject businessObject, DataTable tempTable)
        {
            businessObject.Connect();
            businessObject.First();
            while (!businessObject.IsEof)
            {
                DataRow row = tempTable.NewRow();
                foreach (StiDataColumn column in businessObject.Columns)
                {
                    try
                    {
                        row[column.Name] = businessObject[column.Name];
                    }
                    catch
                    {
                    }
                }
                tempTable.Rows.Add(row);
                businessObject.Next();
            }
        }

        private void FillLevel(int index, List<StiBusinessObject> list, StiBusinessObject businessObject, DataTable tempTable)
        {
            if (index >= list.Count)
            {
                FillLevel(businessObject, tempTable);
            }
            else
            {
                StiBusinessObject parentBusinessObject = list[index];

                parentBusinessObject.Connect();
                parentBusinessObject.First();
                while (!parentBusinessObject.IsEof)
                {
                    FillLevel(index + 1, list, businessObject, tempTable);
                    parentBusinessObject.Next();
                }
            }
        }

        private void Build()
        {
            #region Process BusinessObject
            if (businessObject != null)
            {
                var tempTable = new DataTable();
                tempTable.TableName = businessObject.Name;
                foreach (StiDataColumn column in businessObject.Columns)
                {
                    DataColumn dataColumn;
                    if (Nullable.GetUnderlyingType(column.Type) != null)
                    {
                        dataColumn = new DataColumn(column.Name, Nullable.GetUnderlyingType(column.Type));
                    }
                    else
                    {
                        dataColumn = new DataColumn(column.Name, column.Type);
                    }
                    tempTable.Columns.Add(dataColumn);
                }

                var parent = businessObject.ParentBusinessObject;
                var list = new List<StiBusinessObject>();
                while (parent != null)
                {
                    list.Add(parent);
                    parent = parent.ParentBusinessObject;
                }

                FillLevel(0, list, businessObject, tempTable);

                ResultDataTable = tempTable;
            }
            #endregion

            #region Process DataSource
            else
            {
                #region Load data from XML and CSV database
                if (dataSource is StiDataTableSource)
                {
                    foreach (StiDatabase database in dataSource.Dictionary.Databases)
                    {
                        if ((database is StiXmlDatabase || database is StiCsvDatabase) && ((StiDataTableSource)dataSource).NameInSource != null)
                        {
                            var nameInSource = ((StiDataTableSource)dataSource).NameInSource;
                            var dotIndex = nameInSource.IndexOf(".");
                            if (dotIndex == -1) dotIndex = nameInSource.Length;
                            var databaseName = nameInSource.Substring(0, dotIndex);

                            if (databaseName == database.Name)
                            {
                                database.RegData(dataSource.Dictionary, true);
                                dataSource.Connect(true);
                            }
                        }
                    }
                }
                #endregion

                StiDataTableSource table = dataSource as StiDataTableSource;

                if (table != null && (!(table is StiSqlSource)))
                {
                    if (dataSource.IsConnected)
                    {
                        ResultDataTable = table.DataTable;
                    }
                }
                else
                {
                    if (dataSource is StiVirtualSource)
                        dataSource.Dictionary.ConnectVirtualDataSources();
                    else if (dataSource is StiDataTransformation)
                        dataSource.Dictionary.ConnectDataTransformations();
                    else
                        dataSource.Connect(true);
                    
                    ResultDataTable = dataSource.GetDataTable();
                    dataSource.Connect(false);
                }
            }
            #endregion
        }
        #endregion
    }
}
