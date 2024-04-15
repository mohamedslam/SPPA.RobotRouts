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

namespace Stimulsoft.Base.Server
{
    public interface IStiAccountCreator
    {
        #region Methods
        IStiWpfDashboardViewerControl GetWpfDashboardViewerControl(IStiReport report, int startPage);

        IStiAccountControl GetAccountControl(IntPtr intPtr);

        IStiNotificationsControl GetNotificationsControl(IntPtr intPtr);

        IStiNotificationMenuControl GetNotificationMenuControl(IntPtr intPtr);

        IStiLoginMenuControl GetLoginMenuContol();

        IStiAccountLoginMenuControl GetAccountLoginMenuControl();

        IStiShareMenuControl GetShareMenuControl(object viewer);

        IStiDialogWindow GetSubscriptionsWindow();

        IStiDialogWindow GetChangePasswordWindow();

        IStiUpdateWindow GetUpdateWindow();

        IStiDialogWindow GetProfileWindow();

        IStiPublishWindow GetPublishWindow(IStiReport report);

        IStiGetStartedWindow GetStartedWindow();

        IStiChooseSkillLevelWindow GetChooseSkillLevelWindow(StiDesignerSpecification designerSpecification);

        IStiTrialDaysLeftWindow GetTrialDaysLeftWindow();

        IStiCloudShareWindow GetCloudShareWindow(IStiReport report, string reportItemKey);

        IStiAuthorizationWindow GetAuthorizationWindow();

        IStiCloudSaveWindow GetCloudSaveWindow(IStiReport report, string password, string fileName);

        IStiCloudOpenControl GetCloudOpenControl(IntPtr intPtr);

        IStiCloudOpenWindow GetCloudOpenWindow();

        IStiCustomMapEditorWindow GetCustomMapEditorWindow(string fileData);

        IStiDialogWindow GetNotificationCollectionWindow(StiAccountNotificationCollectionEventArgs accountMessage);

        IStiDialogWindow GetMessageWindow(StiAccountMessageEventArgs accountMessage);

        IStiNuGetWindow GetNuGetWindow(StiDataConnector connector);

        IStiNuGetDownloader GetNuGetDownloader();

        IStiReportCommandsService GetReportCommandsService();

        IStiUnsaveReportWindow GetUnsaveReportWindow();
        #endregion
    }
}
