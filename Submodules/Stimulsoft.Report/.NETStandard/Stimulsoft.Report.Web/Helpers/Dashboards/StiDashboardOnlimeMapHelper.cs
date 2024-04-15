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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Maps;
using System;
using System.Collections;
using System.Drawing;
using System.Text;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiDashboardOnlimeMapHelper
    {
        #region Methods.Helpers
        public static string GetBingMapScript(IStiElement element, bool showTitle)
        {
            var result = (string)StiInvokeMethodsHelper.InvokeStaticMethod(
                "Stimulsoft.Dashboard", "Helpers.StiOnlineMapHelper", "GetBingMapScript",
                new object[] { element, showTitle },
                new Type[] { typeof(IStiElement), typeof(bool) });

            return result;
        }
        #endregion
    }
}
