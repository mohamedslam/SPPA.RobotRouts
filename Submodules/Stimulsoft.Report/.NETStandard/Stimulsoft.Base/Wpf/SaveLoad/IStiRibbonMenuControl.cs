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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stimulsoft.Base.Wpf.SaveLoad
{
    public interface IStiRibbonMenuControl
    {
        bool ShowFileRecentFiles { get; }

        bool ShowMainMenuRecentFiles { get; }

        IntPtr GetOwnerWindowHandle();

        object GetOwnerWindow();

        void ShowMessage(string message);
        bool ShowMessageYesNo(string message, string caption);

        List<StiImportTag> GetImportData();

        void ProcessActiveReports();
        void ProcessComponentOne();
        void ProcessCrystalReports();
        void ProcessDevExpress();
        void ProcessFastReport();
        void ProcessMicrosoftReportingServices();
        void ProcessReportSharpShooter();
        void ProcessRichTextRTF();
        void ProcessTelerikReporting();
        void ProcessVisualFoxPro();

        bool GetSettingsBool(string key, string subkey, bool defValue);
        void SetSettingsBool(string key, string subkey, bool value);

        string GetSettingsStr(string key, string subkey, string defValue);
        void SetSettingsStr(string key, string subkey, string value);

        void OpenFile(string path);

        Task<bool> OpenCloudFileAsync(string itemKey);

        void ProcessBrowseReport(string initDirectory);

        string GetDesignerTitle();

        Task<byte[]> SaveReportToByteArray();

        object GetReport();

        void ShowWarningMessage(string message);

        void SaveToLocalFolder(string path);

        void SaveAsBrowse();
    }
}