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
using System.Web;

#if NETSTANDARD
using Stimulsoft.System.Web;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiCookiesHelper
    {
        internal static void SetCookie(string name, string value)
        {
            try
            {
                var cookie = new HttpCookie(name, value);
                cookie.Expires = DateTime.Now.AddHours(1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal static string GetCookie(string name)
        {
            return HttpContext.Current.Request.Cookies.Get(name)?.Value;
        }
    }
}
