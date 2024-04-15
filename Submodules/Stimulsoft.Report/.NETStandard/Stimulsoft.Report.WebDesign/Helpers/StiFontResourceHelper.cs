#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports         										}
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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Web
{
    public static class StiFontResourceHelper
    {
        #region Methods
        public static void AddFontToReport(StiReport report, StiResource resource, Hashtable resourceItem)
        {
            if (resource.Content != null)
            {
                try
                {
                    resourceItem["contentForCss"] = StiReportResourceHelper.GetBase64DataFromFontResourceContent(resource.Type, resource.Content);
                    resourceItem["originalFontFamily"] = StiFontCollection.GetFontFamily(report.GetResourceFontName(resource.Name)).Name;
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
        }
        #endregion
    }
}
