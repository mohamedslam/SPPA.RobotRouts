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
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base;
using Stimulsoft.Base.Meters;
using Stimulsoft.Report.Components;
using System.Collections.Generic;
using Stimulsoft.Data.Engine;

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiElement : 
        IStiReportComponent,
        IStiQueryObject
    {
        List<IStiMeter> FetchAllMeters();

        List<IStiMeter> GetMeters();

        List<StiPage> GetNestedPages();

        string Key { get; set; }

        StiReport Report { get; set; }

	    StiPage Page { get; set; }

        string Name { get; set; }

        double Zoom { get; }

        bool IsDefined { get; }

        bool IsDesigning { get; }

        bool IsEnabled { get; }

        bool IsQuerable { get; }
    }
}
