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

using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using Stimulsoft.Base;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Design;
using System.Xml.Linq;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if NETSTANDARD || NETCOREAPP
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

#if STIDRAWING
using Stimulsoft.Data;
#endif

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        /// <summary>
        /// Registers an object in the Data Store.
        /// </summary>
        /// <param name="name">A data name in the Data Store.</param>
        /// <param name="data">The object for registration.</param>
        public StiReport RegData(string name, object data)
        {
            Dictionary.DataStore.RegData(name, data);            
            CompiledReport?.Dictionary?.DataStore?.RegData(name, data);

            return this;
        }

        /// <summary>
        /// Registers an object in the Data Store.
        /// </summary>
        /// <param name="name">DataTable name in the Data Store.</param>
        /// <param name="dataTable">DataTable for registration.</param>
        public StiReport RegData(string name, DataTable dataTable)
        {
            Dictionary.DataStore.RegData(name, dataTable);            
            CompiledReport?.Dictionary?.DataStore?.RegData(name, dataTable);

            return this;
        }

        /// <summary>
        /// Registers the DataTable in the Data Store.
        /// </summary>
        /// <param name="dataTable">The DataTable for registration.</param>
        public StiReport RegData(DataTable dataTable)
        {
            Dictionary.DataStore.RegData(dataTable);                           
            CompiledReport?.Dictionary?.DataStore?.RegData(dataTable);

            return this;
        }

        /// <summary>
        /// Registers all DataTables and DataRelations which are in the specified DataSet in the Data Store.
        /// </summary>
        /// <param name="name">Prefix for object registration.</param>
        /// <param name="dataSet">DataSet for registration.</param>
        public StiReport RegData(string name, DataSet dataSet)
        {
            Dictionary.DataStore.RegData(name, dataSet);
            CompiledReport?.Dictionary?.DataStore?.RegData(name, dataSet);

            return this;
        }

        /// <summary>
        /// Registers all DataTable and DataRelation which are in the specified DataSet in the Data Store.
        /// </summary>
        /// <param name="dataSet">DataSet for registration.</param>
        public StiReport RegData(DataSet dataSet)
        {
            Dictionary.DataStore.RegData(dataSet);
            CompiledReport?.Dictionary?.DataStore?.RegData(dataSet);

            return this;
        }

        /// <summary>
        /// Registers the DataCollection in the DataStore of the report.
        /// </summary>
        /// <param name="element">XElement for registration.</param>
        /// <param name="relationDirection">Specifies direction of the relation processing.</param>
        public StiReport RegData(XElement element, StiRelationDirection relationDirection = StiRelationDirection.ParentToChild)
        {
            var dataSet = StiBaseOptions.DefaultJsonConverterVersion == StiJsonConverterVersion.ConverterV2 ?
                StiJsonToDataSetConverterV2.GetDataSet(element, relationDirection) :
                StiJsonToDataSetConverter.GetDataSet(element);

            RegData(dataSet);

            return this;
        }

        /// <summary>
        /// Registers the DataCollection in the DataStore of the report.
        /// </summary>
        /// <param name="name">Prefix for object registration.</param>
        /// <param name="element">XElement for registration.</param>
        /// <param name="relationDirection">Specifies direction of the relation processing.</param>
        public StiReport RegData(string name, XElement element, StiRelationDirection relationDirection = StiRelationDirection.ParentToChild)
        {
            var dataSet = StiBaseOptions.DefaultJsonConverterVersion == StiJsonConverterVersion.ConverterV2 ?
                StiJsonToDataSetConverterV2.GetDataSet(element, relationDirection) :
                StiJsonToDataSetConverter.GetDataSet(element);

            RegData(name, dataSet);

            return this;
        }

        /// <summary>
        /// Registers DataView in the Data Store.
        /// </summary>
        /// <param name="name">DataView name in the Data Store.</param>
        /// <param name="dataView">DataView for registration.</param>
        public StiReport RegData(string name, DataView dataView)
        {
            Dictionary.DataStore.RegData(name, dataView);
            CompiledReport?.Dictionary?.DataStore?.RegData(name, dataView);

            return this;
        }

        /// <summary>
        /// Registers DataView in the Data Store.
        /// </summary>
        /// <param name="dataView">DataView for registration.</param>
        public StiReport RegData(DataView dataView)
        {
            Dictionary.DataStore.RegData(dataView);            
            CompiledReport?.Dictionary?.DataStore?.RegData(dataView);

            return this;
        }

        /// <summary>
        /// Registers SqlConnection in the Data Store.
        /// </summary>
        /// <param name="name">SqlConnection name in the Data Store.</param>
        /// <param name="connection">SqlConnection for registration.</param>
        public StiReport RegData(string name, SqlConnection connection)
        {
            Dictionary.DataStore.RegData(name, connection);            
            CompiledReport?.Dictionary?.DataStore?.RegData(name, connection);

            return this;
        }

        /// <summary>
        /// Registers OleDbConnection in the Data Store.
        /// </summary>
        /// <param name="name">OleDbConnection name in the Data Store.</param>
        /// <param name="connection">OleDbConnection for registration.</param>
        public StiReport RegData(string name, OleDbConnection connection)
        {
            Dictionary.DataStore.RegData(name, connection);            
            CompiledReport?.Dictionary?.DataStore?.RegData(name, connection);

            return this;
        }

        /// <summary>
        /// Registers OdbcConnection in the Data Store.
        /// </summary>
        /// <param name="name">OdbcConnection name in the Data Store.</param>
        /// <param name="connection">OdbcConnection for registration.</param>
        public StiReport RegData(string name, OdbcConnection connection)
        {
            Dictionary.DataStore.RegData(name, connection);            
            CompiledReport?.Dictionary?.DataStore?.RegData(name, connection);

            return this;
        }

        /// <summary>
        /// Registers an object in the Data Store.
        /// </summary>
        /// <param name="name">A data name in the Data Store.</param>
        /// <param name="alias">A data alias in the Data Store.</param>
        /// <param name="data">The object for registration.</param>
        public StiReport RegData(string name, string alias, object data)
        {
            Dictionary.DataStore.RegData(name, alias, data);            
            CompiledReport?.Dictionary?.DataStore?.RegData(name, alias, data);

            return this;
        }

        /// <summary>
        /// Registers an object in the Data Store.
        /// </summary>
        /// <param name="name">A data name in the Data Store.</param>
        /// <param name="alias">A data alias in the Data Store.</param>
        /// <param name="dataTable">DataTable for registration.</param>
        public StiReport RegData(string name, string alias, DataTable dataTable)
        {
            Dictionary.DataStore.RegData(name, alias, dataTable);            
            CompiledReport?.Dictionary?.DataStore?.RegData(name, alias, dataTable);

            return this;
        }

        /// <summary>
        /// Registers all DataTables and DataRelations which are in the specified DataSet in the Data Store.
        /// </summary>
        /// <param name="name">A data name in the Data Store.</param>
        /// <param name="alias">A data alias in the Data Store.</param>
        /// <param name="dataSet">DataSet for registration.</param>
        public StiReport RegData(string name, string alias, DataSet dataSet)
        {
            Dictionary.DataStore.RegData(name, alias, dataSet);            
            CompiledReport?.Dictionary?.DataStore?.RegData(name, alias, dataSet);

            return this;
        }

        /// <summary>
        /// Registers DataView in the Data Store.
        /// </summary>
        /// <param name="name">A data name in the Data Store.</param>
        /// <param name="alias">A data alias in the Data Store.</param>
        /// <param name="dataView">DataView for registration.</param>
        public StiReport RegData(string name, string alias, DataView dataView)
        {
            Dictionary.DataStore.RegData(name, alias, dataView);                        
            CompiledReport?.Dictionary?.DataStore?.RegData(name, alias, dataView);

            return this;
        }

        /// <summary>
        /// Registers SqlConnection in the Data Store.
        /// </summary>
        /// <param name="name">A data name in the Data Store.</param>
        /// <param name="alias">A data alias in the Data Store.</param>
        /// <param name="connection">SqlConnection for registration.</param>
        public StiReport RegData(string name, string alias, SqlConnection connection)
        {
            Dictionary.DataStore.RegData(name, alias, connection);            
            CompiledReport?.Dictionary?.DataStore?.RegData(name, alias, connection);

            return this;
        }

        /// <summary>
        /// Registers OleDbConnection in the Data Store.
        /// </summary>
        /// <param name="name">A data name in the Data Store.</param>
        /// <param name="alias">A data alias in the Data Store.</param>
        /// <param name="connection">OleDbConnection for registration.</param>
        public StiReport RegData(string name, string alias, OleDbConnection connection)
        {
            Dictionary.DataStore.RegData(name, alias, connection);
            CompiledReport?.Dictionary?.DataStore?.RegData(name, alias, connection);

            return this;
        }

        /// <summary>
        /// Registers OdbcConnection in the Data Store.
        /// </summary>
        /// <param name="name">A data name in the Data Store.</param>
        /// <param name="alias">A data alias in the Data Store.</param>
        /// <param name="connection">OdbcConnection for registration.</param>
        public StiReport RegData(string name, string alias, OdbcConnection connection)
        {
            Dictionary.DataStore.RegData(name, alias, connection);
            CompiledReport?.Dictionary?.DataStore?.RegData(name, alias, connection);

            return this;
        }

        /// <summary>
        /// Registers the DataCollection in the DataStore of the report.
        /// </summary>
        /// <param name="datas">DataCollection for registration.</param>
        public StiReport RegData(StiDataCollection datas)
        {
            Dictionary.DataStore.RegData(datas);            
            CompiledReport?.Dictionary?.DataStore?.RegData(datas);

            return this;
        }

        /// <summary>
        /// Registers the business object in the report.
        /// </summary>
        /// <param name="name">Name of the registered business object.</param>
        /// <param name="value">Business object.</param>
        public StiReport RegBusinessObject(string name, object value)
        {
            RegBusinessObject(string.Empty, name, name, value);

            return this;
        }

        /// <summary>
        /// Registers the business object in the report.
        /// </summary>
        /// <param name="category">Name of the category in which registered business object will be placed in report dictionary.</param>
        /// <param name="name">Name of the registered business object.</param>
        /// <param name="value">Business object.</param>
        public StiReport RegBusinessObject(string category, string name, object value)
        {
            RegBusinessObject(category, name, name, value);

            return this;
        }

        /// <summary>
        /// Registers the business object in the report.
        /// </summary>
        /// <param name="category">Name of the category in which registered business object will be placed in report dictionary.</param>
        /// <param name="name">Name of the registered business object.</param>
        /// <param name="alias">Alias of the registered business object.</param>
        /// <param name="value">Business object.</param>
        public StiReport RegBusinessObject(string category, string name, string alias, object value)
        {
            var businessObject = new StiBusinessObjectData(category, name, alias, value);
            StoreBusinessObjectWithCheckExistingData(businessObject);
            
            CompiledReport?.StoreBusinessObjectWithCheckExistingData(businessObject);

            return this;
        }

        /// <summary>
        /// Registers list of business objects in business objects store of specified report.
        /// </summary>
        /// <param name="businessObjects">List of business objects.</param>
        public StiReport RegBusinessObject(List<StiBusinessObjectData> businessObjects)
        {
            foreach (var businessObject in businessObjects)
            {
                StoreBusinessObjectWithCheckExistingData(businessObject);
            }

            return this;
        }

        private void StoreBusinessObjectWithCheckExistingData(StiBusinessObjectData businessObject)
        {
            if (StiOptions.Dictionary.ReplaceExistingDataAtRegistrationOfNewData)
            {
                for (var index = 0; index < this.BusinessObjectsStore.Count; index++)
                {
                    var businessObject2 = this.BusinessObjectsStore[index];
                    if (businessObject2.Category == businessObject.Category && (businessObject2.Name == businessObject.Name))
                    {
                        this.BusinessObjectsStore[index] = businessObject;
                        return;
                    }
                }
            }
            this.BusinessObjectsStore.Add(businessObject);
        }

        /// <summary>
        /// Internal use only. Registers datasources from ReportDataSources property in report dictionary.
        /// </summary>
        public StiReport RegReportDataSources()
        {
            foreach (StiReportDataSource obj in ReportDataSources)
            {
                if (this.Dictionary.DataStore[obj.Name] == null)
                {
                    var dataSet = obj.Item as DataSet;

                    if (obj.Item is Control)
                        RegBusinessObject(obj.Name, obj.Item);

                    else if (dataSet != null)
                        RegData(dataSet);

                    else
                        RegData(obj.Name, obj.Item);
                }
                else
                {
                    var objItem = obj.Item;
                    if (objItem is BindingSource)
                        objItem = (objItem as BindingSource).List;

                    if (this.Dictionary.DataStore[obj.Name].Data != objItem)
                        this.Dictionary.DataStore[obj.Name].Data = objItem;
                }
            }

            return this;
        }
    }
}