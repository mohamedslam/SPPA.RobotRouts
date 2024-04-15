#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Dashboard.Helpers;

namespace Stimulsoft.Report.Dashboard
{
    public static class StiCacheCleaner
    {
        public static void Clean(IStiElement element)
        {
            Clean(element?.GetApp()?.GetKey());
        }

        public static void Clean(IStiAppDictionary dictionary)
        {
            Clean(dictionary?.GetApp()?.GetKey());
        }

        public static void Clean(IStiApp app)
        {
            if (app == null) return;

            Clean(app?.GetKey());
        }

        public static void Clean(string reportKey = null)
        {
            StiElementDataCache.CleanCache(reportKey);
            StiDataPicker.CleanCache(reportKey);
            StiDataJoiner.CleanCache(reportKey);
            StiDataFiltrator.CleanCache(reportKey);
            StiDataSorter.CleanCache(reportKey);
            StiDataActionOperator.CleanCache(reportKey);
            StiReportParser.CleanCache(reportKey);
            StiOnlineMapRepaintHelper.Clean(reportKey);
            StiPivotToContainerCache.Clean(reportKey);
            StiPivotTableToCrossTabCache.Clean(reportKey);
            StiPivotToConvertedStateCache.Clean(reportKey);
            StiDashboardImageHyperlinkCache.Clean(reportKey);
            StiSimpleShadowCache.Clean(reportKey);
        }
    }
}