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
using System;

namespace Stimulsoft.Base.Drawing
{
    public sealed class StiRichTextFromURL
    {
        public static string BaseAddress { get; set; }

        /// <summary>
        /// Load RichText from URL.
        /// </summary>
        public static string LoadRichText(string url, CookieContainer cookieContainer = null)
        {
            if (url?.ToLowerInvariant() == "file://")
                return string.Empty;

            try
            {
                using (var cl = new StiWebClientEx(cookieContainer))
                {
                    if (!string.IsNullOrEmpty(BaseAddress))
                        cl.BaseAddress = BaseAddress;

                    cl.Credentials = CredentialCache.DefaultCredentials;
                    var bytes = cl.DownloadData(url);

                    using (var stream = new MemoryStream(bytes))
                    using (var reader = new StreamReader(stream, Encoding.Default))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
