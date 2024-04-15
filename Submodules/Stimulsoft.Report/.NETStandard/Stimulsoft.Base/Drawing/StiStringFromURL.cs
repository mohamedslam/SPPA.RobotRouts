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

using System.Net;
using System.IO;
using System.Text;

namespace Stimulsoft.Base.Drawing
{
    public sealed class StiStringFromURL
    {
        public static string BaseAddress { get; set; }

        /// <summary>
        /// Loads string from URL.
        /// </summary>
        public static string LoadString(string url, CookieContainer cookieContainer = null)
        {
            using (var cl = new StiWebClientEx(cookieContainer))
            {
                cl.BaseAddress = string.IsNullOrEmpty(BaseAddress) ? cl.BaseAddress : BaseAddress;
                cl.Credentials = CredentialCache.DefaultCredentials;

                var bytes = cl.DownloadData(url);
                
                using (var stream = new MemoryStream(bytes))
                using (var sr = new StreamReader(stream, Encoding.Default))
                {
                    return sr.ReadToEnd();
                }
            }
        }

    }
}
