#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports       										}
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
using System;
using System.Data;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    public class StiODataAdapterService : StiSqlAdapterService
    {
        #region Methods
        public override Type GetDataSourceType()
        {
            return typeof(StiODataSource);
        }

        /// <summary>
        /// Returns new data connector for this type of the database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiSqlDataConnector CreateConnector(string connectionString = null)
        {
            return StiODataConnector.Get(connectionString);
        }

        public override void ConnectDataSourceToData(StiDictionary dictionary, StiDataSource dataSource, bool loadData)
        {
            if (!loadData) return;

            StiDataLeader.Disconnect(dataSource);

            var oDataSource = dataSource as StiODataSource;
            var connection = dictionary.Databases.ToList().FirstOrDefault(d => d.Name == oDataSource.NameInSource) as StiODataDatabase;
            if (connection == null) return;

            var connector = CreateConnector(connection.ConnectionString) as StiODataConnector;
            connector.AllowException = connection.AllowException;
            connector.Headers.Add(connection.Headers);
            connector.Version = connection.Version;
            connector.CookieContainer = dictionary.Report.CookieContainer;

            var table = new DataTable(oDataSource.Name);
            oDataSource.Columns.ToList().ForEach(c => table.Columns.Add(c.Name, c.Type));


            connector.FillDataTable(table, oDataSource.SqlCommand);
            oDataSource.DataTable = table;

        }
        #endregion
    }
}
