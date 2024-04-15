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

using Stimulsoft.Base.Server;
using Stimulsoft.Base.Wpf.RecentFiles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stimulsoft.Base.Wpf.SaveLoad
{
    public interface IStiRibbonMenuDesignerControl
    {
        List<StiFilterTag> GetRibbonSaveMenuFilters(ref int jsonFilterIndex);

        Task<StiRecentCreateReportTemplateResult> SaveToCloudAsync(string fileName, StiRecentFolder folder, string changes, bool isJsonFilter);

        Task<string> UpdateInCloudAsync(string fileName, string itemKey, string changes, bool isJsonFilter);

        string GetReportName();
    }
}