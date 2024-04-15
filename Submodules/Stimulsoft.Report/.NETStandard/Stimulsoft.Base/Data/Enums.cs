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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;

namespace Stimulsoft.Base
{
    #region StiDataFormatType
    public enum StiDataFormatType
    {
        Xml,
        Json
    }
    #endregion

    #region StiRetrieveColumnsMode
    public enum StiRetrieveColumnsMode
    {
        KeyInfo,
        SchemaOnly,
        FillSchema
    }
    #endregion

    #region StiWizardStoredProcRetrieveMode
    public enum StiWizardStoredProcRetriveMode
    {
        All = 1,
        ParametersOnly = 2
    }

    public enum StiWizardStoredProcRetrieveMode
    {
        All = 1,
        ParametersOnly = 2
    }
    #endregion

    #region StiConnectionIdent
    /// <summary>
    /// An enumeration which contains a types of the data sources.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiConnectionIdent
    {
        Db2DataSource = 1,
        InformixDataSource,
        MsAccessDataSource,
        MsSqlDataSource,
        MySqlDataSource,
        OdbcDataSource,
        OleDbDataSource,
        GraphQLDataSource,
        FirebirdDataSource,
        PostgreSqlDataSource,
        OracleDataSource,
        SqlCeDataSource,
        SqLiteDataSource,
        SybaseDataSource,
        SybaseAdsDataSource,
        TeradataDataSource,
        VistaDbDataSource,
        UniversalDevartDataSource,

        ODataDataSource,

        CsvDataSource,
        DBaseDataSource,
        DynamicsNavDataSource,
        ExcelDataSource,
        JsonDataSource,
        GisDataSource,
        XmlDataSource,

        MongoDbDataSource,

        DropboxCloudStorage,
        GoogleDriveCloudStorage,
        OneDriveCloudStorage,
        SharePointCloudStorage,

        AzureTableStorage,
        AzureBlobStorage,
        AzureSqlDataSource,
        CosmosDbDataSource,
        GoogleSheetsStorage,

        DataWorldDataSource,
        QuickBooksDataSource,
        FirebaseDataSource,
        BigQueryDataSource,
        PdoDataSource,
        GoogleAnalyticsDataSource,
        MariaDbDataSource,

        Unspecified
    }
    #endregion

    #region StiConnectionOrder
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiConnectionOrder
    {
        MsSqlDataSource = 10,
        MySqlDataSource = 20,
        OracleDataSource = 30,
        PostgreSqlDataSource = 40,
        JsonDataSource = 50,
        GisDataSource = 55,
        XmlDataSource = 60,

        OdbcDataSource = 70,
        OleDbDataSource = 80,

        MsAccessDataSource = 90,
        FirebirdDataSource = 100,
        SqlCeDataSource = 110,
        SqLiteDataSource = 120,

        Db2DataSource = 130,
        InformixDataSource = 140,
        SybaseDataSource = 150,
        SybaseAdsDataSource = 160,
        TeradataDataSource = 170,
        VistaDbDataSource = 180,
        UniversalDevartDataSource = 190,

        ODataDataSource = 200,

        ExcelDataSource = 210,
        CsvDataSource = 220,
        DBaseDataSource = 230,

        MongoDbDataSource = 240,
        AzureTableStorage = 250,
        AzureSqlDataSource = 256,
        AzureBlobStorage = 255,
        CosmosDbDataSource = 260,
        GoogleSheetsStorage = 270,

        DynamicsNavDataSource = 280,        

        DropboxCloudStorage = 290,
        GoogleDriveCloudStorage = 300,
        OneDriveCloudStorage = 310,
        SharePointCloudStorage = 320,

        DataWorldDataSource = 330,
        QuickBooksDataSource = 340,
        PdoDataSource = 350,

        BigQueryDataSource = 360,
        GoogleAnalyticsDataSource = 370,
        GraphQLDataSource = 380,

        MariaDbDataSource = 390,

        Unspecified = 0
    }
    #endregion

    #region StiDatabaseType
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiDatabaseType
    {
        MsSql,
        MySql,
        SqlCe
    }
    #endregion

    #region StiODataVersion
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiODataVersion
    {
        V3,
        V4
    }
    #endregion

    #region StiCosmosDbApi
    public enum StiCosmosDbApi
    {
        SQL,
        MongoDB
    }
    #endregion

    #region StiFileType
    /// <summary>
    /// Enum contains list of file types which can be present in FileImem.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiFileType
    {
        Unknown = 1,
        ReportSnapshot,
        Pdf,
        Xps,
        PowerPoint,
        Html,
        Text,
        RichText,
        Word,
        OpenDocumentWriter,
        Excel,
        OpenDocumentCalc,
        Data,
        Image,
        Xml,
        Xsd,
        Csv,
        Dbf,
        Sylk,
        Dif,
        Json,
        Gis
    }
    #endregion
}
