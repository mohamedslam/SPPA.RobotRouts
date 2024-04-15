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

using Stimulsoft.Report.App;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Dashboard.Helpers
{
    public class StiElementScale
    {
        public static double Factor(IStiElement element)
        {
            return element?.Page != null ? element.Page.Zoom : 1d;
        }

        public static double Factor(IStiComponentUI comp)
        {
            return comp?.Page != null ? comp.Page.Zoom : 1d;
        }

        public static double Factor(StiComponent component)
        {
            return component?.Page != null ? component.Page.Zoom : 1d;
        }
    }
}
