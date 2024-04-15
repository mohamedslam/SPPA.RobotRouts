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

using Stimulsoft.Base.Data.Connectors.Rest.Helpers;
using Stimulsoft.Base.Design;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Net;

namespace Stimulsoft.Base
{
    public class StiODataConnector : StiRestDataConnector
    {
        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.ODataDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.ODataDataSource;

        public override string Name => "OData";

        /// <summary>
        /// Get a value which indicates that this data connector can be used now.
        /// </summary>
        public override bool IsAvailable => AssemblyHelper != null && AssemblyHelper.IsAllowed;

        /// <summary>
        /// Gets the package identificator for this connector.
        /// </summary>
        public override string[] NuGetPackages => new string[] { "Stimulsoft.Data.OData" };

        /// <summary>
        /// Gets the package version for this connector.
        /// </summary>
        public override string NuGetVersion => "2022.1.1";

        /// <summary>
        /// Get or sets value, which indicates Odata version.
        /// </summary>
        public StiODataVersion Version { get; set; } = StiODataVersion.V4;

        [Browsable(false)]
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// List of headers used for http requests to load data.
        /// </summary>
        public NameValueCollection Headers { get; protected set; }

        /// <summary>
        /// Allows exceptions from the retrieving schema methods and data.
        /// </summary>
        [Browsable(false)]
        public bool AllowException { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns schema object which contains information about structure of the database. Schema returned start at specified root element (if it applicable).
        /// </summary>
        public override StiDataSchema RetrieveSchema(bool allowException = false)
        {
            if (this.Version == StiODataVersion.V4)
                return new StiODataHelperV2(this.ConnectionString, this.NuGetVersion, this.CookieContainer, this.Headers) { AllowException = this.AllowException }.RetrieveSchema();

            return new StiODataHelper(this.ConnectionString, this.Headers) { AllowException = this.AllowException }.RetrieveSchema();
        }

        public override void FillDataTable(DataTable table, string query)
        {
            if (this.Version == StiODataVersion.V4)
                new StiODataHelperV2(this.ConnectionString, this.NuGetVersion, this.CookieContainer, this.Headers) { AllowException = this.AllowException }.FillDataTable(table, query);
            else
                new StiODataHelper(this.ConnectionString, this.Headers) { AllowException = this.AllowException }.FillDataTable(table, query);
        }

        public List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            if (this.Version == StiODataVersion.V4)
                return new StiODataHelperV2(this.ConnectionString, this.NuGetVersion, this.CookieContainer, this.Headers) { AllowException = this.AllowException }.GetColumns(collectionName);
                
            return new StiODataHelper(this.ConnectionString, this.Headers) { AllowException = this.AllowException }.GetColumns(collectionName);

        }

        /// <summary>
        /// Returns StiTestConnectionResult that is the information of whether the connection string specified in this class is correct.
        /// </summary>
        /// <returns>The result of testing the connection string.</returns>
        public override StiTestConnectionResult TestConnection()
        {
            return new StiODataHelper(this.ConnectionString, this.Headers).TestConnection();
        }

        /// <summary>
        /// Returns sample of the connection string to this connector.
        /// </summary>
        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.OData;
        }
        #endregion

        #region Methods.Static
        public static StiODataConnector Get(string connectionString = null, NameValueCollection headers = null)
        {
            return new StiODataConnector(connectionString, headers);
        }
        #endregion

        public StiODataConnector(string connectionString = null, NameValueCollection headers = null)
            : base(connectionString)
        {
            this.FolderAssembly = $"Stimulsoft.Data.OData-{this.NuGetVersion}";
            this.NameAssembly = "Stimulsoft.Data.OData.dll";
            this.Headers = headers ?? new NameValueCollection();
        }
    }
}