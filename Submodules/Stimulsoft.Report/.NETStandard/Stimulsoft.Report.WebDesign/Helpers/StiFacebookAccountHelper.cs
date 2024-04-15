#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System.Collections;
using System.Security.Cryptography;

namespace Stimulsoft.Report.Web
{
    internal class StiFacebookAccountHelper
    {
        #region Fields
        private const string clientID = "1629645760688442";
        private const string authorizationEndpoint = "https://www.facebook.com/v14.0/dialog/oauth";
        private static string redirectUrl = "https://designer.stimulsoft.com/";
        #endregion

        internal static void GetAuthorizationUrl(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            callbackResult["url"] = StiEncodingHelper.Encode($"{authorizationEndpoint}?client_id={clientID}&redirect_uri={redirectUrl}");
        }

        internal static void FacebookAuthorizationProcess(Hashtable parameters)
        {
            var code = parameters["facebookAuthCode"] as string;
            var error = parameters["facebookAuthError"] as string;

            if (!string.IsNullOrEmpty(code))
                StiCookiesHelper.SetCookie("sti_FacebookAuthCode", code);
            else
                StiCookiesHelper.SetCookie("sti_FacebookAuthError", !string.IsNullOrEmpty(error) ? error : "facebook OAuth authorization error!");
        }
    }
}