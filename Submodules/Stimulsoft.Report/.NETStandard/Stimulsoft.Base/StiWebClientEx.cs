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
using System.ComponentModel;
using System.Net;

namespace Stimulsoft.Base
{
    [ToolboxItem(false)]
    public class StiWebClientEx : WebClient
    {
        #region Properties
        public CookieContainer CookieContainer { get; set; }
        #endregion

        #region WebClient override
        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);

            var request = webRequest as HttpWebRequest;
            if (request != null)
            {
                request.UserAgent = "Foo";
                request.Accept = "*/*";

                if (CookieContainer != null)
                    request.CookieContainer = CookieContainer;
            }

            return webRequest;
        }
        #endregion

        public StiWebClientEx(CookieContainer container = null)
        {
            this.CookieContainer = container;
            this.Encoding = StiBaseOptions.WebClientEncoding;

            if (StiBaseOptions.WebClientCheckTlsProtocols)
            {
                #region Check for Tls11 & Tls12 and enable it
                var platformSupportsTls11 = false;
                var platformSupportsTls12 = false;
                foreach (SecurityProtocolType protocol in Enum.GetValues(typeof(SecurityProtocolType)))
                {
                    if (protocol.GetHashCode() == 768) 
                        platformSupportsTls11 = true;

                    if (protocol.GetHashCode() == 3072) 
                        platformSupportsTls12 = true;
                }

                if (!ServicePointManager.SecurityProtocol.HasFlag((SecurityProtocolType)768) && platformSupportsTls11) 
                    ServicePointManager.SecurityProtocol |= (SecurityProtocolType)768;

                if (!ServicePointManager.SecurityProtocol.HasFlag((SecurityProtocolType)3072) && platformSupportsTls12) 
                    ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
                #endregion
            }
        }
    }
}
