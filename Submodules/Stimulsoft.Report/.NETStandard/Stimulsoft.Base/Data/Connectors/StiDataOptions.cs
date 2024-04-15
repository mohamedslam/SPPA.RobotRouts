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
using System.ComponentModel;

namespace Stimulsoft.Base
{
    public class StiDataOptions
    {
        #region class SampleConnectionString
        public class SampleConnectionString
        {
            public static string MongoDb { get; set; } = @"mongodb://<user>:<password>@localhost/test";

            public static string AzureSql { get; set; } =
                "Server =<server address>; User ID =<username>; Password =<password>; Initial Catalog =<database name>;";

            public static string CosmosMongoDb { get; set; } =
                @"mongodb://<user>:<password>@<user>.documents.azure.com:<port>/<collection>/?ssl=true&replicaSet=globaldb";

            public static string CosmosSql { get; set; } =
                @"Database=<myDataBase>;AccountEndpoint=<EndpointUrl>;AccountKey=<PrimaryKey>;";

            public static string AzureBlobStorage { get; set; } =
                $"AccountKey=<key>;AccountName=<account>;ContainerName=<conainer>;BlobName=<blob>;" + Environment.NewLine +
                    $"BlobContentType=CSV;CodePage=<page>;CsvSeparator=<separator>; OR" + Environment.NewLine +
                    $"BlobContentType=Excel;FirstRowIsHeader=<{bool.TrueString}>; OR" + Environment.NewLine +
                    $"BlobContentType=JSON; OR" + Environment.NewLine +
                    $"BlobContentType=XML;";

            public static string AzureTableStorage { get; set; } =
                @"DefaultEndpointsProtocol=https;AccountName=<AccountName>;AccountKey=<AccountKey>;EndpointSuffix=core.windows.net";

            public static string BigQuery { get; set; } =
                @"Base64EncodedAuthSecret=<secret>;ProjectId=<projectID>;DatasetId=<datasetID>";

            public static string Firebase { get; set; } = @"AuthSecret=<secret>;BasePath=<path>";

            public static string GoogleAnalytics { get; set; } =
                @"Base64EncodedAuthSecret=<secret>;AccountId=<accountId>;PropertyId=<propertyId>;ViewId=<viewId>;Metrics=<metrics>;Dimensions=<dimensions>";

            public static string DataWorld { get; set; } = @"Owner=<owner>;Database=<id>;Token=<token>";

            public static string OData { get; set; } = "https://services.odata.org/V4/Northwind/Northwind.svc";

            public static string QuickBooks { get; set; } = "";

            public static string Db2 { get; set; } =
                @"Server=myAddress:myPortNumber;Database=myDataBase;UID=myUsername;PWD=myPassword;" + Environment.NewLine +
                @"Max Pool Size=100;Min Pool Size=10;";

            public static string Firebird { get; set; } =
                @"User=SYSDBA; Password=masterkey; Database=SampleDatabase.fdb;" + Environment.NewLine +
                @"DataSource=myServerAddress; Port=3050; Dialect=3; Charset=NONE;" + Environment.NewLine +
                @"Role=; Connection lifetime=15; Pooling=true; MinPoolSize=0;" + Environment.NewLine +
                @"MaxPoolSize=50; Packet Size=8192; ServerType=0;";

            public static string Informix { get; set; } =
                @"Database=myDataBase;Host=192.168.10.10;Server=db_engine_tcp;Service=1492;" + Environment.NewLine +
                @"Protocol=onsoctcp;UID=myUsername;Password=myPassword;";

            public static string MariaDb { get; set; } =
                @"Server=localhost; Port=3306; Database=myDataBase;" + Environment.NewLine +
                @"UserId=myUsername; Pwd=myPassword;";

            public static string MsAccess { get; set; } =
                @"Provider=Microsoft.Jet.OLEDB.4.0;User ID=Admin;Password=pass;" + Environment.NewLine +
                @"Data Source=C:\\myAccessFile.accdb;";

            public static string MsSql { get; set; } =
                @"Integrated Security=False; Data Source=myServerAddress;" + Environment.NewLine +
                @"Initial Catalog=myDataBase; User ID=myUsername; Password=myPassword;";

            public static string MySql { get; set; } =
                @"Server=myServerAddress; Database=myDataBase;" + Environment.NewLine +
                @"UserId=myUsername; Pwd=myPassword;";

            public static string MySqlDevart { get; set; } =
                @"User ID=root;Password=myPassword;Host=localhost;Port=3306;Database=myDataBase;" + Environment.NewLine +
                @"Direct=true;Protocol=TCP;Compress=false;Pooling=true;Min Pool Size=0;" + Environment.NewLine +
                @"Max Pool Size=100;Connection Lifetime=0;";

            public static string Odbc { get; set; } =
                @"Driver={SQL Server}; Server=myServerAddress;" + Environment.NewLine +
                @"Database=myDataBase; Uid=myUsername; Pwd=myPassword;";

            public static string OleDb { get; set; } =
                @"Provider=SQLOLEDB.1; Integrated Security=SSPI;" + Environment.NewLine +
                @"Persist Security Info=False; Initial Catalog=myDataBase;" + Environment.NewLine +
                @"Data Source=myServerAddress";

            public static string Oracle { get; set; } =
                @"Data Source=TORCL;User Id=myUsername;Password=myPassword;";

            public static string PostgreSql { get; set; } =
                @"Server=myServerAddress; Port=5432; Database=myDataBase;" + Environment.NewLine +
                @"User Id=myUsername; Password=myPassword;";

            public static string PostgreSqlDevart { get; set; } =
                @"User ID=root;Password=myPassword;Host=localhost;Port=5432;Database=myDataBase;" + Environment.NewLine +
                @"Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;";

            public static string SqlCe { get; set; } =
                @"Data Source=c:\MyData.sdf; Persist Security Info=False;";

            public static string SqLite { get; set; } =
                @"Data Source=c:\mydb.db; Version=3;";

            public static string SybaseAds { get; set; } =
                @"Data Source=\\myserver\myvolume\mypat\mydd.add;User ID=myUsername;Password=myPassword;ServerType=REMOTE;";

            public static string Sybase { get; set; } =
                @"Data Source=myASEserver;Port=5000;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";

            public static string Teradata { get; set; } =
                @"Data Source=myServerAddress;User ID=myUsername;Password=myPassword;";

            public static string UniversalDevart { get; set; } =
                @"Provider=Oracle;direct=true;data source=192.168.0.1;port=1521;sid=sid;user=user;password=pass";

            public static string VistaDb { get; set; } =
                @"Data Source=D:\folder\myVistaDatabaseFile.vdb4;Open Mode=ExclusiveReadWrite;";
        }
        #endregion

        /// <summary>
        /// StiDataOptions.RetriveColumnsMode was obsoleted. Please use StiDataOptions.RetrieveColumnsMode.
        /// </summary>
        [Obsolete("StiDataOptions.RetriveColumnsMode was obsoleted. Please use StiDataOptions.RetrieveColumnsMode.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static StiRetrieveColumnsMode RetriveColumnsMode
        {
            get
            {
                return RetrieveColumnsMode;
            }
            set
            {
                RetrieveColumnsMode = value;
            }
        }

        /// <summary>
        /// Value which describes mode of retrieving database information.
        /// </summary>
        public static StiRetrieveColumnsMode RetrieveColumnsMode { get; set; } = StiRetrieveColumnsMode.SchemaOnly;

        /// <summary>
        /// Value which describes mode of retrieving information for stored procedures.
        /// </summary>
        [Obsolete("StiDataOptions.WizardStoredProcRetriveMode was obsoleted. Please use StiDataOptions.WizardStoredProcRetrieveMode.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static StiWizardStoredProcRetriveMode WizardStoredProcRetriveMode
        {
            get
            {
                return (StiWizardStoredProcRetriveMode)WizardStoredProcRetrieveMode;
            }
            set
            {
                WizardStoredProcRetrieveMode = (StiWizardStoredProcRetrieveMode)value;
            }
        }

        /// <summary>
        /// Value which describes mode of retrieving information for stored procedures.
        /// </summary>
        public static StiWizardStoredProcRetrieveMode WizardStoredProcRetrieveMode { get; set; } = StiWizardStoredProcRetrieveMode.All;

        /// <summary>
        /// Value which allows to use default Oracle Client connector.
        /// </summary>
        public static bool AllowUseOracleClientConnector { get; set; }        
    }
}
