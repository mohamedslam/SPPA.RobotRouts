#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft BI												    }
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

using System.Data;

namespace Stimulsoft.Base
{
    public interface IStiBIDataCache
    {
        bool Exists(IStiAppDataSource dataSource);

        bool Exists(string tableKey);

        void Remove(string tableKey);

        void Clean(string appKey);

        void CleanAll();

        long GetTableCount();

        long GetRowCount(string tableKey);

        DataTable GetSchema(string tableKey);

        DataTable GetData(string tableKey);

        DataTable RunQuery(string query);

        void SaveData(string appKey, string tableKey, DataTable dataTable);

        //string GetTableName(string appKey, string tableKey);
    }
}
