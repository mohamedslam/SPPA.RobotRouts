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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Stimulsoft.Base;
using Stimulsoft.Base.Json;

#if CLOUD
using System.Web.UI;
using Stimulsoft.Server.Connect;
using Stimulsoft.Server.Objects;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiGoogleAccountHelper
    {
        #region Const
        private const string TokenRequestURI = "https://accounts.google.com/o/oauth2/token";
        private const string EncryptingKey = "8pTP5X15uKADcSw7";
        private const string ClientID = "87797872002-0ohi2vph6jt8cqtmt9qmacpu30cb5aac.apps.googleusercontent.com";
        private const string ClientSecret = "8ebByltuRlx-UNJhbW-cokRJ";
        private const string CloudServerAdress = "https://cloud.stimulsoft.com/";
        #endregion

        //for localhost 
        //private const string ClientID = "87797872002-t22qcq8r4dvpke8sqss0l2koe1tgvb7s.apps.googleusercontent.com";
        //private const string ClientSecret = "Q3rNH7KFtuL3pHgC9ItVzb0w";


        internal static void GoogleAuthorizationProcess(Hashtable parameters)
        {
#if CLOUD
            var code = parameters["oAuthCode"] as string;
            var error = parameters["oAuthError"] as string;
            var redirectUrl = parameters["googleAuthRedirectUrl"] as string;

            if (!string.IsNullOrEmpty(code))
            {
                var token = ExchangeCodeAtToken(code, redirectUrl);
                if (!string.IsNullOrEmpty(token))
                {
                    var googleAuthAction = StiCookiesHelper.GetCookie("sti_GoogleAuthAction");
                    var currentSessionKey = StiCookiesHelper.GetCookie("sti_GoogleAuthCurrentSessionKey");

                    if (googleAuthAction == "AddLoginWithGoogle" && currentSessionKey != null)
                    {
                        UserAddLoginWithGoogle(token, currentSessionKey);
                        return;
                    }

                    var loginResult = LoginWithGoogle(token, parameters);
                    if (loginResult != null && !string.IsNullOrEmpty(loginResult.SessionKey) && !string.IsNullOrEmpty(loginResult.UserKey))
                    {
                        StiCookiesHelper.SetCookie("sti_GoogleAuthSessionKey", loginResult.SessionKey);
                        StiCookiesHelper.SetCookie("sti_GoogleAuthUserKey", loginResult.UserKey);
                        return;
                    }
                }
            }
            else
            {
                StiCookiesHelper.SetCookie("sti_GoogleAuthError", !string.IsNullOrEmpty(error) ? error : "OAuth authorization error!");
            }
#endif
        }

        private static string ExchangeCodeAtToken(string code, string redirectUrl)
        {
            #region Builds the request
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&client_secret={3}&grant_type=authorization_code",
                code,
                redirectUrl,
                ClientID,
                ClientSecret);
            #endregion

            #region Sends the request
            var tokenRequest = (HttpWebRequest)WebRequest.Create(TokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            byte[] byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = byteVersion.Length;

            var stream = tokenRequest.GetRequestStream();
            stream.Write(byteVersion, 0, byteVersion.Length);
            stream.Close();
            #endregion

            try
            {
            #region Gets the response
                var tokenResponse = tokenRequest.GetResponse();
                using (var reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    var responseText = reader.ReadToEnd();
                    var tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    return tokenEndpointDecoded["access_token"];
                }
            #endregion
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
            #region Reads response body
                            string responseText = reader.ReadToEnd();
                            var errorDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                            var message = "OAuth authorization error!";
                            if (!string.IsNullOrEmpty(errorDecoded["error"])) message += $" {errorDecoded["error"]}.";
                            if (!string.IsNullOrEmpty(errorDecoded["error_description"])) message += $" {errorDecoded["error_description"]}.";

                            StiCookiesHelper.SetCookie("sti_GoogleAuthError", message);
            #endregion
                        }
                    }
                }
            }

            return null;
        }

        private static Dictionary<string, string> GetUserInfoList(string token)
        {
            string url = $"https://www.googleapis.com/oauth2/v2/userinfo?access_token={token}";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            var response = request.GetResponse();
            using (var responseReader = new StreamReader(response.GetResponseStream()))
            {
                var text = responseReader.ReadToEnd();
                var userListInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

                return userListInfo;
            }
        }

#if CLOUD
        private static StiLoginResult LoginWithGoogle(string token, Hashtable parameters)
        {
            try
            {
                var email = GetUserInfoList(token)?["email"];
                if (!string.IsNullOrEmpty(email))
                {
                    StiCookiesHelper.SetCookie("sti_GoogleAuthEmail", email);

                    var connection = new StiServerConnection(CloudServerAdress);
                    return connection.Accounts.Users.Login(StiServerAuthType.Google, email, token, email);
                }
            }
            catch (Exception ee)
            {
                var serverException = ee as StiServerException;
                if (serverException != null && serverException.Notice?.Ident == StiNoticeIdent.AuthUserNameNotAssociatedWithYourAccount)
                {
                    StiCookiesHelper.SetCookie("sti_GoogleAuthException", "NotAssociatedWithYourAccount");
                    StiCookiesHelper.SetCookie("sti_GoogleAuthEncryptingToken", StiEncryption.Encrypt(token, EncryptingKey));
                }
                else
                    StiCookiesHelper.SetCookie("sti_GoogleAuthError", ee.Message);
            }
            return null;
        }

        public static void AssociatedGoogleAuthWithYourAccount(Hashtable param, Hashtable callbackResult)
        {
            var encryptingToken = param["encryptingToken"] as string;
            var email = param["email"] as string;
            var password = param["password"] as string;

            if (!string.IsNullOrEmpty(encryptingToken) && !string.IsNullOrEmpty(email))
            {
                try
                {
                    var token = StiEncryption.Decrypt(encryptingToken, EncryptingKey);
                    var connection = new StiServerConnection(CloudServerAdress);
                    connection.Accounts.Users.AddLoginWithGoogle(token, email, email, password);
                    var loginResult = connection.Accounts.Users.Login(StiServerAuthType.Google, email, token, email);
                    if (loginResult != null && !string.IsNullOrEmpty(loginResult.SessionKey) && !string.IsNullOrEmpty(loginResult.UserKey))
                    {
                        callbackResult["sessionKey"] = loginResult.SessionKey;
                        callbackResult["userKey"] = loginResult.UserKey;
                    }
                }
                catch (Exception ee)
                {
                    var serverException = ee as StiServerException;
                    callbackResult["enterPasswordError"] = serverException != null && serverException.Notice != null ? StiNoticeConverter.Convert(serverException.Notice) : ee.Message;
                }
            }
        }

        private static void UserAddLoginWithGoogle(string token, string sessionKey)
        {
            var email = GetUserInfoList(token)?["email"];
            var connection = new StiServerConnection(CloudServerAdress);
            connection.Login(sessionKey);
            connection.Accounts.Users.AddLoginWithGoogle(token, email);
            StiCookiesHelper.SetCookie("sti_GoogleAuthEmail", email);
        }
#endif
    }
}