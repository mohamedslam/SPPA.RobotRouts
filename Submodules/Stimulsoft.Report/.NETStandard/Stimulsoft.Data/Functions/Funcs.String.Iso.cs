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

using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        #region Fields.Static
        private static Dictionary<string, List<string>> iso2Cache = new Dictionary<string, List<string>>();
        #endregion

        #region Methods.Funcs
        public static List<string> GetMapIdents(string key)
        {
            var helper = StiReportAssembly.GetMapKeyHelper();
            if (helper == null)
                return null;

            return helper.GetMapIdents(key);
        }

        public static List<string> GetIso2ConvertedValues(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return iso2Cache.ContainsKey(name) ? iso2Cache[name] : new List<string> { name };
        }

        public static string Iso2(string name, string mapId, string lang)
        {
            if (name == null)
                return string.Empty;

            var helper = StiReportAssembly.GetMapKeyHelper();
            if (helper == null)
                return name;

            var alpha = helper.GetIsoAlpha2FromName(name, mapId, lang);
            var result = alpha != null ? alpha : name;

            var values = iso2Cache.ContainsKey(result) ? iso2Cache[result] : null;
            if (values == null)
            {
                values = new List<string> { name };
                iso2Cache[result] = values;
            }
            else if (!values.Contains(name))
                values.Add(name);

            return result;
        }

        public static object Iso2Object(object value, string mapId, string lang)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(v => Iso2(v, mapId, lang));
            else
                return Iso2(StiValueHelper.TryToString(value), mapId, lang);
        }

        public static string Iso2ToName(string alpha2, string mapId, string lang)
        {
            var helper = StiReportAssembly.GetMapKeyHelper();
            if (helper == null)
                return alpha2;

            var name = helper.GetNameFromIsoAlpha2(alpha2, mapId, lang);
            return name != null ? name : alpha2;
        }

        public static object Iso2ToNameObject(object value, string mapId, string lang)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(v => Iso2ToName(v, mapId, lang));
            else
                return Iso2ToName(StiValueHelper.TryToString(value), mapId, lang);
        }

        public static string Iso3(string name, string mapId, string lang)
        {
            if (name == null)
                return string.Empty;

            var helper = StiReportAssembly.GetMapKeyHelper();
            if (helper == null)
                return name;

            var alpha = helper.GetIsoAlpha3FromName(name, mapId, lang);
            return alpha != null ? alpha : name;
        }

        public static object Iso3Object(object value, string mapId, string lang)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(v => Iso3(v, mapId, lang));
            else
                return Iso3(StiValueHelper.TryToString(value), mapId, lang);
        }

        public static string Iso3ToName(string alpha3, string mapId, string lang)
        {
            var helper = StiReportAssembly.GetMapKeyHelper();
            if (helper == null)
                return alpha3;

            var name = helper.GetNameFromIsoAlpha3(alpha3, mapId, lang);
            return name != null ? name : alpha3;
        }

        public static object Iso3ToNameObject(object value, string mapId, string lang)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(v => Iso3ToName(v, mapId, lang));
            else
                return Iso3ToName(StiValueHelper.TryToString(value), mapId, lang);
        }

        public static string NormalizeName(string alpha3, string mapId, string lang)
        {
            var helper = StiReportAssembly.GetMapKeyHelper();
            if (helper == null)
                return alpha3;

            return helper.NormalizeName(alpha3, mapId, lang);
        }

        public static object NormalizeNameObject(object value, string mapId, string lang)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(v => NormalizeName(v, mapId, lang));
            else
                return NormalizeName(StiValueHelper.TryToString(value), mapId, lang);
        }
        #endregion
    }
}