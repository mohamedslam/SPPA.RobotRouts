#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Collections.Specialized;
using System.Net;

namespace Stimulsoft.Base.Drawing
{
    public sealed class StiBytesFromURL
    {
        #region Fields.Static
        private static Hashtable failedAttempts = new Hashtable();
        #endregion

        #region Properties
        public static bool AllowCacheFails { get; set; } = true;
        #endregion

        #region Methods
        /// <summary>
        /// Loads bytes from the specified hyperlink.
        /// </summary>
        public static byte[] Load(string url, CookieContainer cookieContainer = null, NameValueCollection headers = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            //Cache for the previous cases of exception on trying to get an object from the specified path
            if (AllowCacheFails)
            {
                var tickCountObject = failedAttempts[url];
                if (tickCountObject is int && ((int)tickCountObject - Environment.TickCount) < 2000)
                    return null;
            }

            try
            {
                using (var client = new StiWebClientEx(cookieContainer))
                {
                    client.Credentials = CredentialCache.DefaultCredentials;
                    if (headers != null)
                        client.Headers.Add(headers);
                    return client.DownloadData(url);
                }
            }
            catch (Exception)
            {
                if (AllowCacheFails)
                    failedAttempts[url] = Environment.TickCount;

                throw;
            }
        }

        /// <summary>
        /// Tries to load bytes from the specified hyperlink.
        /// </summary>
        public static byte[] TryLoad(string url, CookieContainer cookieContainer = null, NameValueCollection headers = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            //Cache for the previous cases of exception on trying to get an object from the specified path
            var tickCountObject = failedAttempts[url];
            if (tickCountObject is int && ((int)tickCountObject - Environment.TickCount) < 2000)
                return null;

            try
            {
                using (var client = new StiWebClientEx(cookieContainer))
                {
                    client.Credentials = CredentialCache.DefaultCredentials;

                    if (headers != null)
                        client.Headers.Add(headers);

                    return client.DownloadData(url);
                }
            }
            catch
            {
                failedAttempts[url] = Environment.TickCount;
                return null;
            }
        }
        #endregion
    }
}
