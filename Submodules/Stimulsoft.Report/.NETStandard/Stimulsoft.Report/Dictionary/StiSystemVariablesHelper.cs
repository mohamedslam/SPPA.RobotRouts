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

using System.Collections.Generic;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Dictionary
{
	public class StiSystemVariablesHelper
	{
		#region Methods
		public static string GetSystemVariableInfo(string variable)
		{
            return StiLocalization.Get("SystemVariables", variable);
		}

		public static List<string> GetSystemVariables(StiReport report)
		{
		    return report != null && report.EngineVersion == StiEngineVersion.EngineV2 ? 
                GetSystemVariablesV2() : 
                GetSystemVariablesV1();
		}

	    protected static List<string> systemVariablesV1;
		public static List<string> GetSystemVariablesV1()
		{
		    return systemVariablesV1 ?? (systemVariablesV1 = new List<string>
		    {
		        "Column",
		        "Line",
		        "LineThrough",
		        "LineABC",
		        "LineRoman",
		        "PageNumber",
		        "PageNofM",
		        "TotalPageCount",
		        "IsFirstPage",
		        "IsLastPage",
		        "PageCopyNumber",
		        "ReportAlias",
		        "ReportAuthor",
		        "ReportChanged",
		        "ReportCreated",
		        "ReportDescription",
		        "ReportName",
		        "Time",
		        "Today"
		    });
		}

        protected static List<string> systemVariablesV2;
		public static List<string> GetSystemVariablesV2()
		{
		    return systemVariablesV2 ?? (systemVariablesV2 = new List<string>
		    {
		        "Column",
		        "Line",
		        "LineThrough",
		        "LineABC",
		        "LineRoman",
		        "GroupLine",
		        "PageNumber",
		        "PageNumberThrough",
		        "PageNofM",
		        "PageNofMThrough",
		        "TotalPageCount",
		        "TotalPageCountThrough",
		        "IsFirstPage",
		        "IsFirstPageThrough",
		        "IsLastPage",
		        "IsLastPageThrough",
		        "PageCopyNumber",
		        "ReportAlias",
		        "ReportAuthor",
		        "ReportChanged",
		        "ReportCreated",
		        "ReportDescription",
		        "ReportName",
		        "Time",
		        "Today"
		    });
		}
	    #endregion
	}
}
