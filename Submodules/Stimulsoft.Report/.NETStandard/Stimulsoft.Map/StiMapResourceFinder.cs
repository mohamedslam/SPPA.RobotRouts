#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports.Net											}
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
using Stimulsoft.Base.Map;
using System.IO;

namespace Stimulsoft.Map
{
    public class StiMapResourceFinder : IStiMapResourceFinder
    {
        public string Get(string resourceName)
        {
            string name = (resourceName == "ChinaWithHongKongAndMacau" || resourceName == "ChinaWithHongKongMacauAndTaiwan")
                ? "China"
                : resourceName;

            using (var sr = new StreamReader(typeof(StiMapResourceFinder).Assembly.GetManifestResourceStream($"Stimulsoft.Map.Resources.{name}.map")))
            {
                var res = StiGZipHelper.Unpack(sr.ReadToEnd());
                return res;
            }
        }
    }
}