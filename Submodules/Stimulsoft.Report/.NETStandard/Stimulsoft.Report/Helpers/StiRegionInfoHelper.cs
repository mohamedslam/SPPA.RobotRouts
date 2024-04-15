#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using System.Globalization;
using System.Linq;

namespace Stimulsoft.Report.Helpers
{
    public class StiRegionInfoHelper
    {
        #region Methods
        public static string GetIsoAlpha2FromName(string name)
        {
            var region = GetRegionInfoFromName(name);

            return region != null ? region.TwoLetterISORegionName : null;
        }

        public static string GetIsoAlpha3FromName(string name)
        {
            var region = GetRegionInfoFromName(name);

            return region != null ? region.ThreeLetterISORegionName : null;
        }

        public static string GetNameFromIsoAlpha2(string alpha2)
        {
            var region = GetRegionInfoFromName(alpha2);

            return region != null ? region.Name : null;
        }

        public static string GetNameFromIsoAlpha3(string alpha3)
        {
            var region = GetRegionInfoFromName(alpha3);

            return region != null ? region.Name : null;
        }

        public static string GetLocalizedNameFromIsoAlpha2(string alpha2)
        {
            var region = GetRegionInfoFromName(alpha2);
            return region != null ? region.NativeName : null;
        }

        public static string GetLocalizedNameFromIsoAlpha3(string alpha3)
        {
            var region = GetRegionInfoFromName(alpha3);
            return region != null ? region.NativeName : null;
        }

        public static RegionInfo GetRegionInfoFromName(string name)
        {
            var simpliedName = StiMapKeyHelper.Simplify(name);
            return GetAllRegions().FirstOrDefault(c => 
                    StiMapKeyHelper.Simplify(c.Name) == simpliedName ||
                    StiMapKeyHelper.Simplify(c.NativeName) == simpliedName ||
                    StiMapKeyHelper.Simplify(c.ThreeLetterISORegionName) == simpliedName ||
                    StiMapKeyHelper.Simplify(c.ThreeLetterWindowsRegionName) == simpliedName ||
                    StiMapKeyHelper.Simplify(c.TwoLetterISORegionName) == simpliedName);
        }

        private static IEnumerable<RegionInfo> GetAllRegions()
        {
#if NETSTANDARD || NETCOREAPP
            return new RegionInfo[0];
#else
            return CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(c => c.LCID != 127)
                .Select(c => new RegionInfo(c.TextInfo.CultureName));
#endif
        }
		#endregion
    }
}