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
using Stimulsoft.Base.Exceptions;
using Stimulsoft.Base.Plans;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Stimulsoft.Report.Dictionary
{
    internal class StiDataAdapterHelper
	{
	    public static void Fill(StiDictionary dictionary, DbDataAdapter dataAdapter, DataTable dataTable, bool schemaOnly)
	    {
            if (schemaOnly)
	        {
                StiDataMonitor.FillSchema(dataAdapter);
                dataAdapter.FillSchema(dataTable, SchemaType.Source);
	        }
	        else
	        {
#if CLOUD
	            var reportGuid = dictionary?.Report?.ReportGuid;
	            var maxRows = StiCloudReport.GetMaxDataRows(reportGuid);
	            var fetchedRows = dataAdapter.Fill(0, maxRows, dataTable);
	            var resultRows = StiCloudReport.AddDataRows(reportGuid, fetchedRows);

				if (resultRows >= maxRows)
				{
					StiCloudReportResults.InitMaxDataRows(reportGuid, maxRows);
					throw new StiMaxDataRowsException();
				}
#else
                StiDataMonitor.FillAdapter(dataAdapter);
                dataAdapter.Fill(dataTable);
#endif
	        }
	    }

	    public static DataTable Fill(StiDictionary dictionary, DataTable dataTable)
	    {
#if CLOUD
            var reportGuid = dictionary?.Report?.ReportGuid;
	        var maxRows = StiCloudReport.GetMaxDataRows(reportGuid);
	        var resultRows = StiCloudReport.AddDataRows(reportGuid, dataTable.Rows.Count);

            if (resultRows >= maxRows)
            {
                StiCloudReportResults.InitMaxDataRows(reportGuid, maxRows);
                throw new StiMaxDataRowsException();
            }
#endif
            return dataTable;
        }

	    public static DataSet Fill(StiDictionary dictionary, DataSet dataSet, StiDatabase database)
	    {
#if CLOUD
            var dataSources = StiDataSourceHelper.GetDataSourcesFromDatabase(dictionary.Report, database);

            var tables = new List<DataTable>();
            foreach (StiDataStoreSource dataSource in dataSources)
            {
                foreach (DataTable table in dataSet.Tables)
                {
                    var name = $"{database.Name}.{table.TableName}";
                    if (name == dataSource.NameInSource)
                    {
                        Fill(dictionary, table);
                        tables.Add(table);
                        break;
                    }
                }                
            }

            foreach (DataTable table in dataSet.Tables)
            {
                if (!tables.Contains(table))
                {
                    try
                    {
                        table.Rows.Clear();
                    }
                    catch
                    {
                    }
                }
            }
#endif
            return dataSet;
	    }
	}
}
