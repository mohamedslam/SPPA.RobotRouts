#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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

using Stimulsoft.Report;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Web
{
    public static class StiBlocklyDataSourceHelper
    {
        public static List<string> LoadDataSource(StiReport report)
        {
            return report.Dictionary.DataSources.ToList().Select(v => v.Name).ToList();
        }

        public static List<string> GetDataSourceProperties()
        {
            return new List<string>()
            {
                "Count",
                "IsBof",
                "IsConnected",
                "IsEmpty",
                "IsEof",
                "Position"
            };
        }

        public static List<string> GetDataSourceMethods()
        {
            return new List<string>()
            {
                "Connect",
                "Disconnect",
                "First",
                "Last",
                "Next",
                "Prior"
            };
        }
    }
}