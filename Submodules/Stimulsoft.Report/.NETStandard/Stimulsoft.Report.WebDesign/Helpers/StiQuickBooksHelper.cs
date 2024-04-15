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


using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Web;

namespace Stimulsoft.Report.Web
{
    internal class StiQuickBooksHelper
    {
        #region Fields
        private static string stimulsoftClientId = "ABCOlUoW6pnAVfhh9Jo8M3o2ESseTWW6thnktdWY88d4jTLC0e";
        private static string stimulsoftClientSecret = "JNZebKvLaGwNxPW1wrUj9yJqoMfaNddW5fI8hT5V";
        private static string oauth2Url = "https://appcenter.intuit.com/connect/oauth2";
        private static string bearerUrl = "https://oauth.platform.intuit.com/oauth2/v1/tokens/bearer";
        private static string responseType = "code";
        private static string scope = "com.intuit.quickbooks.accounting";

#if CLOUD
        private static string stimulsoftRedirectUrl = "https://designer.stimulsoft.com/";
#else
        private static string stimulsoftRedirectUrl = "http://localhost:51190/Designer.aspx";
#endif
        #endregion

        internal static void GetAuthorizationUrl(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var state = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            StiCookiesHelper.SetCookie("sti_QuickBooksAuthState", state);

            var clientId = param.Contains("clientId") ? StiEncodingHelper.DecodeString(param["clientId"] as string) : stimulsoftClientId;
            var redirectUrl = param.Contains("redirectUrl") ? StiEncodingHelper.DecodeString(param["redirectUrl"] as string) : stimulsoftRedirectUrl;
            var url = $"{oauth2Url}?client_id={clientId}&response_type={responseType}&scope={scope}&redirect_uri={redirectUrl}&state={state}";

            if (!string.IsNullOrWhiteSpace(param["realmId"] as string))
                url += $"&realm_id={StiEncodingHelper.DecodeString(param["realmId"] as string)}";

            callbackResult["url"] = StiEncodingHelper.Encode(url);
        }

        internal static void GetTokens(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var clientId = param.Contains("clientId") ? StiEncodingHelper.DecodeString(param["clientId"] as string) : stimulsoftClientId;
            var clientSecret = param.Contains("clientSecret") ? StiEncodingHelper.DecodeString(param["clientSecret"] as string) : stimulsoftClientSecret;
            var redirectUrl = param.Contains("redirectUrl") ? StiEncodingHelper.DecodeString(param["redirectUrl"] as string) : stimulsoftRedirectUrl;
            var authorizationCode = param.Contains("authorizationCode") ? StiEncodingHelper.DecodeString(param["authorizationCode"] as string) : "";

            var client = GetDefaultWebClient();
            client.Headers["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))}";
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            var responseJson = client.UploadString(bearerUrl, "POST", $"code={authorizationCode}&redirect_uri={redirectUrl}&grant_type=authorization_code");
            var response = JObject.Parse(responseJson);
            if (response["access_token"] != null && response["access_token"] != null)
            {
                callbackResult["accessToken"] = StiEncodingHelper.Encode(response["access_token"].ToString());
                callbackResult["refreshToken"] = StiEncodingHelper.Encode(response["refresh_token"].ToString());
            }
        }

        internal static void RefreshTokens(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var clientId = param.Contains("clientId") ? StiEncodingHelper.DecodeString(param["clientId"] as string) : stimulsoftClientId;
            var clientSecret = param.Contains("clientSecret") ? StiEncodingHelper.DecodeString(param["clientSecret"] as string) : stimulsoftClientSecret;
            var refreshToken = param.Contains("refreshToken") ? StiEncodingHelper.DecodeString(param["refreshToken"] as string) : "";

            var client = GetDefaultWebClient();
            client.Headers["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))}";
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            var responseJson = client.UploadString(bearerUrl, "POST", $"refresh_token={refreshToken}&grant_type=refresh_token");
            var response = JObject.Parse(responseJson);
            if (response["access_token"] != null && response["access_token"] != null)
            {
                callbackResult["accessToken"] = StiEncodingHelper.Encode(response["access_token"].ToString());
                callbackResult["refreshToken"] = StiEncodingHelper.Encode(response["refresh_token"].ToString());
            }
        }

        internal static void QuickBooksAuthorizationProcess(Hashtable parameters)
        {
            var code = parameters["oAuthCode"] as string;
            var error = parameters["oAuthError"] as string;
            var realmId = parameters["oAuthRealmId"] as string;

            if (!string.IsNullOrEmpty(code))
            {
                StiCookiesHelper.SetCookie("sti_QuickBooksAuthCode", code);

                if (!string.IsNullOrEmpty(realmId))
                    StiCookiesHelper.SetCookie("sti_QuickBooksAuthRealmId", realmId);
            }
            else
                StiCookiesHelper.SetCookie("sti_QuickBooksAuthError", !string.IsNullOrEmpty(error) ? error : "QuickBooks OAuth authorization error!");
        }

        private static WebClient GetDefaultWebClient()
        {
            var client = new WebClient();
            client.Proxy = StiProxy.GetProxy();
            client.Encoding = StiBaseOptions.WebClientEncoding;
            client.Headers["Accept"] = "application/json";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return client;
        }
    }
}