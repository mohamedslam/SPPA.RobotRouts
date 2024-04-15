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

using Stimulsoft.Base;
using Stimulsoft.Base.Map;
using Stimulsoft.Report.Maps.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stimulsoft.Report.Helpers
{
    public class StiMapKeyHelper : IStiMapKeyHelper
    {
        #region Methods
        public List<string> GetMapIdents(string key)
        {
            key = Simplify(key);
            var list = new List<string>();
            
            var idents = GetMapIdents(key, StiIsoElementHelper.Countries);
            if (idents != null)
                list.AddRange(idents);

            var iso2 = GetIsoAlpha2FromName(key, null, null);
            if (!string.IsNullOrWhiteSpace(iso2))
                list.Add(iso2);

            var iso3 = GetIsoAlpha3FromName(key, null, null);
            if (!string.IsNullOrWhiteSpace(iso3))
                list.Add(iso3);

            if (list.Count == 0)
            {
                idents = GetMapIdents(key, StiIsoElementHelper.UsStates);
                if (idents != null)
                    list.AddRange(idents);
            }

            if (list.Count == 0)
            {
                idents = GetMapIdents(key, StiIsoElementHelper.CanadaProvinces);
                if (idents != null)
                    list.AddRange(idents);
            }

            if (list.Count == 0)
            {
                idents = GetMapIdents(key, StiIsoElementHelper.BrazilProvinces);
                if (idents != null)
                    list.AddRange(idents);
            }
            
            list.Add(key);
            
            return list.Distinct().ToList();
        }

        private static List<string> GetMapIdents(string key, Collection<StiIsoCountry> countries)
        {
            foreach (var country in countries)
            {
                if (Simplify(country.Alpha2) == key
                    || Simplify(country.Alpha3) == key
                    || (country.Names != null && country.Names.Any(n => Simplify(n) == key))
                    || (country.FrNames != null && country.FrNames.Any(n => Simplify(n) == key))
                    || (country.RuNames != null && country.RuNames.Any(n => Simplify(n) == key)))
                {
                    var list = new List<string>();

                    if (country.Names != null)
                        list.AddRange(country.Names);

                    if (country.RuNames != null)
                        list.AddRange(country.RuNames);

                    if (country.FrNames != null)
                        list.AddRange(country.FrNames);

                    return list;
                }
            }
            return null;
        }

        public string GetNameFromIsoAlpha2(string alpha2, string mapId, string lang, IStiReport report = null)
        {
            var block = StiMapResourceHelper.GetSvgBlockFromName(alpha2, mapId, lang, report as StiReport);
            if (block != null)
                return block.EnglishName;

            var country = StiIsoElementHelper.GetCountryFromAlpha2(alpha2, mapId);
            if (country != null)
                return country.Names.FirstOrDefault();

            var name = StiRegionInfoHelper.GetNameFromIsoAlpha2(alpha2);
            if (name != null)
                return name;

            return null;
        }

        public string GetNameFromIsoAlpha3(string alpha3, string mapId, string lang, IStiReport report = null)
        {
            var block = StiMapResourceHelper.GetSvgBlockFromName(alpha3, mapId, lang, report as StiReport);
            if (block != null)
                return block.EnglishName;

            var country = StiIsoElementHelper.GetCountryFromAlpha3(alpha3, mapId);
            if (country != null)
                return country.Names.FirstOrDefault();

            var name = StiRegionInfoHelper.GetNameFromIsoAlpha3(alpha3);
            if (name != null)
                return name;

            return null;
        }

        public string NormalizeName(string name, string mapId, string lang, IStiReport report = null)
        {
            var alpha2 = GetIsoAlpha2FromName(name, mapId, lang, report);
            var normalizedName = GetNameFromIsoAlpha2(alpha2, mapId, lang, report);
            
            return string.IsNullOrWhiteSpace(normalizedName) ? name : normalizedName;
        }

        public string GetIsoAlpha2FromName(string name, string mapId, string lang, IStiReport report = null)
        {
            if (StiGssMapHelper.IsGssValue(name))
                return name;

            var alpha2 = StiMapResourceHelper.GetIsoAlpha2FromName(name, mapId, lang, report as StiReport);
            if (alpha2 != null)
                return alpha2;

            alpha2 = StiIsoElementHelper.GetIsoAlpha2FromName(name, mapId);
            if (alpha2 != null)
                return alpha2;

            alpha2 = StiRegionInfoHelper.GetIsoAlpha2FromName(name);
            if (alpha2 != null)
                return alpha2;

            return null;
        }

        public string GetIsoAlpha3FromName(string name, string mapId, string lang, IStiReport report = null)
        {
            var alpha3 = StiMapResourceHelper.GetIsoAlpha3FromName(name, mapId, lang, report as StiReport);
            if (alpha3 != null)
                return alpha3;

            alpha3 = StiIsoElementHelper.GetIsoAlpha3FromName(name, mapId);
            if (alpha3 != null)
                return alpha3;

            alpha3 = StiRegionInfoHelper.GetIsoAlpha3FromName(name);
            if (alpha3 != null)
                return alpha3;

            return null;
        }

        public List<string> ConvertMapKeysToIsoAlpha2(List<string> mapKeys, string mapId, string lang, IStiReport report = null)
        {
            if (mapKeys == null || mapKeys.Count == 0)
                return null;

            return mapKeys.Select(k => StiMapResourceHelper.GetIsoAlpha2FromName(k, mapId, lang, report as StiReport))
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Distinct()
                .ToList();
        }

        public List<string> GetMapKeysFromNames(List<object> values, string mapId, string lang, IStiReport report = null)
        {
            if (values == null)
                return null;

            return values
                .Select(v => v?.ToString())
                .Select(n => GetMapKeyFromName(n, mapId, lang, report))
                .ToList();
        }

        private string GetMapKeyFromName(string name, string mapId, string lang, IStiReport report = null)
        {
            if (StiGssMapHelper.IsGssValue(name))
                return name;

            var isoAlpha2 = new StiMapKeyHelper().GetIsoAlpha2FromName(name, mapId, lang, report);
            if (isoAlpha2 == null) 
                return name;

            var block = StiMapResourceHelper.GetSvgBlockFromIsoAlpha2(isoAlpha2, mapId, lang, report as StiReport);
            return block?.Key;
        }
        #endregion

        #region Methods.Static
        public static string Simplify(string key)
        {
            return key?.ToLowerInvariant()?.Replace(" ", "")?.Replace("-", "");
        }
        #endregion
    }
}