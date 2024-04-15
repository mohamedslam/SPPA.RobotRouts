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
using System.Reflection;
using System.Web;
using System.IO;

#if NETSTANDARD
using Stimulsoft.System.Web;
#endif

namespace Stimulsoft.Report.Web
{
    internal static class StiWebHelper
    {
        public static void InitWeb()
        {
            if (StiOptions.Configuration.IsWeb) return;

            StiOptions.Configuration.IsWeb = true;
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Server != null)
                {
                    StiOptions.Configuration.ApplicationDirectory = HttpContext.Current.Server.MapPath(string.Empty);
                    return;
                }
            }
            catch
            {
                InitWeb2();
            }

            InitWeb2();
        }

        private static void InitWeb2()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    StiOptions.Configuration.ApplicationDirectory = HttpRuntime.AppDomainAppPath;
                    return;
                }
            }
            catch
            {
                InitWeb3();
            }

            InitWeb3();
        }

        private static void InitWeb3()
        {
            Assembly a = Assembly.GetEntryAssembly();
            if (a != null) StiOptions.Configuration.ApplicationDirectory = Path.GetDirectoryName(a.Location);
            else StiOptions.Configuration.ApplicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
