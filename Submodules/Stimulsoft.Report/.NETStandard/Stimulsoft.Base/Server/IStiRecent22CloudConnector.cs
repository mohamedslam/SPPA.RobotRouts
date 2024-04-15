#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Threading.Tasks;

namespace Stimulsoft.Base.Server
{
    public delegate IStiRecentCloudFileStructureItem CreateRecentCloudFileStructureItem();

    public interface IStiRecent22CloudConnector
    {
        IStiAuthorizationWindow GetAuthorizationWindow();

        Task FillLevelAsync(IStiRecentCloudFileStructureLevel level);

        Task<object> CreateNewFolderAsync(string folderName, string folderKey);

        Task<string> GetFullPathAsync(string itemKey);

        Task<string> DeleteItemAsync(string itemKey);

        Task<bool> ItemExistsAsync(string itemKey);

        Task FillLevelsAsync(IStiRecentCloudFileStructure fileStructure, string itemKey);

        Task<object> GetReportAsync(string itemKey);

        int GetMaxFileSize();

        Task<string> UpdateReportTemplateDataAsync(string itemKey, byte[] reportByteArray, string changes);

        string GetQuotaMaximumFileSizeExceededText();

        Task<StiRecentCreateReportTemplateResult> CreateReportTemplateAsync(object report, string folderKey, string reportName, byte[] reportByteArray, string changes);

        Task<string> CreateReportIfNnotExistsTemplateAsync(object report, string folderKey, string reportName, byte[] reportByteArray, string changes);

        Task<List<IStiRecentCloudFileStructureItem>> GetReportTemplateListByNameAsync(string folderKey, string reportName, 
            CreateRecentCloudFileStructureItem createItem);
    }
}