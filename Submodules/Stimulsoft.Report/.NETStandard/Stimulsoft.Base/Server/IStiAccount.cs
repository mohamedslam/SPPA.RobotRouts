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
    internal interface IStiAccount
    {
        #region Methods
        void CheckLicense();

        void CheckRememberMeLogout();

        void Logout();

        void InvokeUserInfoChanged();

        void InvokeNotificationsRequest();

        void InvokeNotificationsLoaded();

        void InvokeNotificationsChanged();
        
        void InvokeShowNewNotifications(EventArgs arg);

        void InvokeShowNewMessage(EventArgs arg);

        void InvokeDesignerSpecificationChanged();
        #endregion

        #region Properties
        IStiAccountUser User { get; set; }

        IStiAccountActions Actions { get; set; }

        IStiAccountChecker AccountChecker { get; set; }

        IStiAccountCreator AccountCreater { get; set; }
        #endregion

        #region Events
        event EventHandler UserInfoChanged;
        
        event EventHandler<EventArgs> ShowNewNotifications;

        event EventHandler<EventArgs> ShowNewMessage;

        event EventHandler NotificationsLoaded;

        event EventHandler NotificationsChanged;

        event EventHandler NotificationsRequest;

        event EventHandler DesignerSpecificationChanged;
        #endregion
    }
}