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
    internal class StiGitHubAccountHelper
    {
        #region Fields
        private const string clientID = "567aecccd3db10ec21d7";
        private const string authorizationEndpoint = "https://gitHub.com/login/oauth/authorize";
        private static string redirectUrl = "https://designer.stimulsoft.com/";
        #endregion

        internal static void GetAuthorizationUrl(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            // Generates state and PKCE values.
            string state = RandomDataBase64url(32);

            // Creates the OAuth 2.0 authorization request.
            callbackResult["url"] = StiEncodingHelper.Encode($"{authorizationEndpoint}?client_id={clientID}&redirect_uri={redirectUrl}&state={state}");
        }

        internal static void GitHubAuthorizationProcess(Hashtable parameters)
        {
            var code = parameters["gitHubAuthCode"] as string;
            var error = parameters["gitHubAuthError"] as string;

            if (!string.IsNullOrEmpty(code))
                StiCookiesHelper.SetCookie("sti_GitHubAuthCode", code);
            else
                StiCookiesHelper.SetCookie("sti_GitHubAuthError", !string.IsNullOrEmpty(error) ? error : "GitHub OAuth authorization error!");
        }

        private static string RandomDataBase64url(uint length)
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64urlencodeNoPadding(bytes);
        }

        private static string Base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
    }
}