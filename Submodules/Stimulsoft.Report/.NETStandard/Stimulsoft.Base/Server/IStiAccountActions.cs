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

using Stimulsoft.Base.Design;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Stimulsoft.Base.Server
{
    public interface IStiAccountActions
    {
        #region Properties
        bool SkipRequestChangesSaving { get; set; }
        #endregion

        #region Handler
        event EventHandler SaveReportStart;

        event EventHandler SaveReportCompleted;

        event ProgressChangedEventHandler SaveReportProgressChanged;

        event EventHandler OpenReportStart;

        event EventHandler OpenReportCompleted;

        event ProgressChangedEventHandler OpenReportProgressChanged;
        #endregion

        #region Methods
        StiLoadResult LoadReportByteArray(string itemKey);
        
        Task<StiLoadResult> LoadReportByteArrayAsync(string itemKey, IntPtr? intPtr = null);

        IStiReport OpenReport(string key);

        Task<IStiReport> OpenReportAsync(string key);

        Task SaveReportAsync(string itemKey, object report, IntPtr intPtr);

        Task SaveEncryptedReportAsync(string itemKey, object report, string password, IntPtr intPtr);
        #endregion
    }
}