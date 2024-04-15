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

using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report.Web;
using System;
using System.Collections;
using System.Globalization;

#if NETSTANDARD
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
#endif

namespace Stimulsoft.Report.Angular
{
    public class StiAngularDesigner :
#if NETSTANDARD
    StiNetCoreDesigner
#else
    StiMvcDesigner
#endif
    {
        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer for Angular version.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="options">A set of options that will be used for the designer.</param>
        public static ActionResult DesignerDataResult(StiRequestParams requestParams, StiAngularDesignerOptions options)
        {
            options.Server.UseRelativeUrls = false;
            return GetScriptsResult(requestParams, options, true);
        }

        public StiAngularDesigner(
#if NETSTANDARD
            IHtmlHelper htmlHelper,
#else
            HtmlHelper htmlHelper,
#endif
            string viewerId, StiAngularDesignerOptions options) : base(htmlHelper, viewerId, options)
        {
        }
    }
}
