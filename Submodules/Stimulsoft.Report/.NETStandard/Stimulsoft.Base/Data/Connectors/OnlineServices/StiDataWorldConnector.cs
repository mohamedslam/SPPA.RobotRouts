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

using Stimulsoft.Base.Data.Connectors.OnlineServices.Helpers;
using System.Collections.Generic;
using System.Data;

namespace Stimulsoft.Base.Data.Connectors.OnlineServices
{
    public class StiDataWorldConnector : StiDbNoSqlDataConnector
    {
        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.DataWorldDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.DataWorldDataSource;

        public override string Name => "Data.World";

        public override bool IsAvailable => true;
        #endregion

        #region Methods
        public override List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            return new StiDataWorldHelper(this.ConnectionString).GetColumns(collectionName);
        }

        public override DataTable GetDataTable(string collectionName, string query, int? index = null, int? count = null)
        {
            return new StiDataWorldHelper(this.ConnectionString).GetDataTable(collectionName, query);
        }

        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.DataWorld;
        }

        public override StiDataSchema RetrieveSchema(bool allowException = false)
        {
            return new StiDataWorldHelper(this.ConnectionString).RetrieveSchema(allowException);
        }

        public override StiTestConnectionResult TestConnection()
        {
            return new StiDataWorldHelper(this.ConnectionString).TestConnection();
        }
        #endregion

        #region Methods.Static
        public static StiDataWorldConnector Get(string connectionString = null)
        {
            return new StiDataWorldConnector(connectionString);
        }
        #endregion

        public StiDataWorldConnector(string connectionString = null)
            : base(connectionString)
        {
            this.NameAssembly = "Stimulsoft.Data.DataWorld.dll";
        }
    }
}
