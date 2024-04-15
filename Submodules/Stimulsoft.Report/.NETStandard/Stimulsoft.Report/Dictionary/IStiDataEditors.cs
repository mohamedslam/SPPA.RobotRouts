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

using Stimulsoft.Report.Dictionary.Databases.Google;
using System.Threading.Tasks;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
	public interface IStiDataEditors
	{
        DialogResult CsvDatabaseEdit(StiCsvDatabase database, StiDictionary dictionary, bool newDatabase);

        DialogResult DBaseDatabaseEdit(StiDBaseDatabase database, StiDictionary dictionary, bool newDatabase);

        DialogResult ExcelDatabaseEdit(StiExcelDatabase database, StiDictionary dictionary, bool newDatabase);

        DialogResult JsonDatabaseEdit(StiJsonDatabase database, StiDictionary dictionary, bool newDatabase);

        DialogResult GisDatabaseEdit(StiGisDatabase database, StiDictionary dictionary, bool newDatabase);

        DialogResult SqlDatabaseEdit(StiSqlDatabase database, StiDictionary dictionary, bool newDatabase);

        Task<DialogResult> SqlDatabaseEditAsync(StiSqlDatabase database, StiDictionary dictionary, bool newDatabase);

        DialogResult NoSqlDatabaseEdit(StiNoSqlDatabase database, StiDictionary dictionary, bool newDatabase);

        Task<DialogResult> NoSqlDatabaseEditAsync(StiNoSqlDatabase database, StiDictionary dictionary, bool newDatabase);

        DialogResult GoogleSheetsDatabaseEdit(StiGoogleSheetsDatabase database, StiDictionary dictionary, bool newDatabase);
        
        Task<DialogResult> GoogleSheetsDatabaseEditAsync(StiGoogleSheetsDatabase database, StiDictionary dictionary, bool newDatabase);
        
        DialogResult DataWorldDatabaseEdit(StiDataWorldDatabase dataWorldDatabase, StiDictionary dictionary, bool newDatabase);        
        
        Task<DialogResult> DataWorldDatabaseEditAsync(StiDataWorldDatabase dataWorldDatabase, StiDictionary dictionary, bool newDatabase);

        DialogResult FirebaseDatabaseEdit(StiFirebaseDatabase firebaseDatabase, StiDictionary dictionary, bool newDatabase);

        Task<DialogResult> FirebaseDatabaseEditAsync(StiFirebaseDatabase firebaseDatabase, StiDictionary dictionary, bool newDatabase);

        DialogResult GraphQLDatabaseEdit(StiGraphQLDatabase database, StiDictionary dictionary, bool newDatabase);

        Task<DialogResult> GraphQLDatabaseEditAsync(StiGraphQLDatabase database, StiDictionary dictionary, bool newDatabase);

        DialogResult BigQueryDatabaseEdit(StiBigQueryDatabase database, StiDictionary dictionary, bool newDatabase);

        Task<DialogResult> BigQueryDatabaseEditAsync(StiBigQueryDatabase database, StiDictionary dictionary, bool newDatabase);

        DialogResult AzureBlobStorageDatabaseEdit(StiAzureBlobStorageDatabase database, StiDictionary dictionary, bool newDatabase);

        DialogResult GoogleAnalyticsDatabaseEdit(StiGoogleAnalyticsDatabase stiGoogleAnalyticsDatabase, StiDictionary dictionary, bool isNewDatabase);

        Task<DialogResult> GoogleAnalyticsDatabaseEditAsync(StiGoogleAnalyticsDatabase stiGoogleAnalyticsDatabase, StiDictionary dictionary, bool isNewDatabase);

        Task<DialogResult> AzureBlobStorageDatabaseEditAsync(StiAzureBlobStorageDatabase database, StiDictionary dictionary, bool newDatabase);

        DialogResult QuickBooksDatabaseEdit(StiQuickBooksDatabase quickBooksDatabase, StiDictionary dictionary, bool newDatabase);

        Task<DialogResult> QuickBooksDatabaseEditAsync(StiQuickBooksDatabase quickBooksDatabase, StiDictionary dictionary, bool newDatabase);

        DialogResult XmlDatabaseEdit(StiXmlDatabase database, StiDictionary dictionary, bool newDatabase);
		
		StiUserNameAndPassword PromptUserNameAndPassword();
		
		bool DataStoreAdapterEdit(StiDataAdapterService adapter, StiDictionary dictionary, StiDataSource dataSource);
		
		bool DataStoreAdapterNew(StiDataAdapterService adapter, StiDictionary dictionary, StiDataSource dataSource);
		
		bool VirtualAdapterEdit(StiVirtualAdapterService adapter, StiDictionary dictionary, StiDataSource dataSource);

		bool VirtualAdapterNew(StiVirtualAdapterService adapter, StiDictionary dictionary, StiDataSource dataSource);

        bool CrossTabAdapterNew(StiCrossTabAdapterService adapter, StiDictionary dictionary, StiDataSource dataSource);

        bool CrossTabAdapterEdit(StiCrossTabAdapterService adapter, StiDictionary dictionary, StiDataSource dataSource);
	}
}
