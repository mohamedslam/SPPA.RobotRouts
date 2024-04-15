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

using Stimulsoft.Report.Maps;

namespace Stimulsoft.Report.Helpers
{
    public class StiMapResourceHelper
    {
        public static StiMapSvg GetSvgBlockFromIsoAlpha2(string alpha2, string mapId, string lang, StiReport report = null)
        {
            if (string.IsNullOrWhiteSpace(alpha2))
                return null;

            var container = GetResource(mapId, lang, report);
            if (container == null)
                return null;

            var simpliedAlpha2 = StiMapKeyHelper.Simplify(alpha2);

            foreach (var path in container.HashPaths)
            {
                if (StiMapKeyHelper.Simplify(path.Value.ISOCode) == simpliedAlpha2)
                    return path.Value;
            }
            return null;
        }

        public static StiMapSvg GetSvgBlockFromName(string name, string mapId, string lang, StiReport report = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var container = GetResource(mapId, lang, report);
            if (container == null)
                return null;

            var simpliedName = StiMapKeyHelper.Simplify(name);
            var decodedAlpha = StiMapKeyHelper.Simplify(DecodeAlpha(name));

            foreach (var path in container.HashPaths)
            {
                if (StiMapKeyHelper.Simplify(path.Value.Key) == simpliedName ||
                    StiMapKeyHelper.Simplify(path.Value.EnglishName) == simpliedName ||
                    StiMapKeyHelper.Simplify(path.Value.ISOCode) == simpliedName ||
                    StiMapKeyHelper.Simplify(path.Value.ISOCode) == decodedAlpha)
                    return path.Value;
            }
            return null;
        }

        public static string GetIsoAlpha2FromName(string name, string mapId, string lang, StiReport report = null)
        {
            var block = GetSvgBlockFromName(name, mapId, lang, report);
            return block != null ? block.ISOCode : null;
        }

        public static string GetIsoAlpha3FromName(string name, string mapId, string lang, StiReport report = null)
        {
            var alpha2 = GetIsoAlpha2FromName(name, mapId, lang, report);
            if (alpha2 == null)
                return null;

            var country = StiIsoElementHelper.GetCountryFromAlpha2(alpha2, mapId);
            return country != null ? country.Alpha3 : null;
        }

        private static StiMapSvgContainer GetResource(string mapId, string lang, StiReport report = null)
        {
            return (string.IsNullOrWhiteSpace(mapId))
                ? StiMapLoader.LoadResource(report, StiMapID.World.ToString(), lang)
                : StiMapLoader.LoadResource(report, mapId, lang);
        }

        private static string DecodeAlpha(string alpha2)
        {
            if (alpha2 == null)
                return alpha2;

            if (!(alpha2.Length == 5 
                  && char.IsLetter(alpha2[0]) 
                  && char.IsLetter(alpha2[1])
                  && alpha2[2] == '-'
                  && char.IsLetter(alpha2[3])
                  && char.IsLetter(alpha2[4])))
                return alpha2;

            var strs = alpha2.Split('-');
            if (strs.Length < 2)
                return null;

            return strs[1];
        }
    }
}