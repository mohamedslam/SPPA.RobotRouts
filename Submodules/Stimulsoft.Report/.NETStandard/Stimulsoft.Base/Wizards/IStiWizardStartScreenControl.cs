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

namespace Stimulsoft.Base.Wizards
{
    public interface IStiWizardStartScreenControl
    {
        #region Properties
        IStiReport Report { get; }

        IntPtr IntPtrParentControl { get; set; }

        IntPtr IntPtrParentForm { get; set; }

        object RecentFile { get; set; }
        #endregion

        #region Events
        event EventHandler CloudReportOpened;

        event EventHandler RefreshRecentFiles;

        event EventHandler RecentFileChanged;

        event EventHandler ReportChanged;

        event EventHandler ReportPathChanged;

        event EventHandler ButtonMinimizeClick;

        event EventHandler ButtonMaximizeClick;

        event EventHandler ButtonCloseClick;
        #endregion
    }
}
