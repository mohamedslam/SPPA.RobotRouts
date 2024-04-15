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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Maps.Helpers
{
    public static class StiCustomMapFinder
    {
        static StiCustomMapFinder()
        {
            var ids = Enum.GetValues(typeof(StiMapID));
            foreach (StiMapID id in ids)
            {
                defaultMaps.Add(id.ToString().ToLowerInvariant());
            }

            defaultMaps.Add("Germany_DE".ToLowerInvariant());
            defaultMaps.Add("France_FR".ToLowerInvariant());
            defaultMaps.Add("Italy_IT".ToLowerInvariant());
            defaultMaps.Add("Russia_RU".ToLowerInvariant());
        }

        #region Fields
        private static StiReport lastReport;
        private static List<string> defaultMaps = new List<string>();
        private static Dictionary<string, StiMapSvgContainer> customMaps = new Dictionary<string, StiMapSvgContainer>();
        #endregion

        #region Properties
        public static int Version { get; private set; } = 1;
        #endregion

        #region Methods
        internal static Dictionary<string, StiMapSvgContainer> GetMaps() => customMaps;

        internal static void Clear()
        {
            lastReport = null;
            Version++;

            customMaps.Clear();
        }

        internal static void Init(StiReport report)
        {
            bool isChanged = (lastReport != report);

            if (report == null)
            {
                if (isChanged) Version++;

                lastReport = null;
                customMaps.Clear();
                return;
            }

            lastReport = report;
            var cache = new Dictionary<string, StiMapSvgContainer>(customMaps);

            var resources = report.Dictionary.Resources.Cast<StiResource>().Where(x => x.Type == StiResourceType.Map && x.Content != null).ToList();
            foreach (var resource in resources)
            {
                // if default map - skip
                if (defaultMaps.Contains(resource.Name.ToLowerInvariant()))
                    continue;

                // already exists
                if (cache.ContainsKey(resource.Name))
                {
                    cache.Remove(resource.Name);
                    continue;
                }

                try
                {
                    string json = StiGZipHelper.ConvertByteArrayToString(resource.Content);

                    var container = new StiMapSvgContainer();
                    JsonConvert.PopulateObject(json, container, StiJsonHelper.DefaultSerializerSettings);

                    if (container.Paths == null)
                        container.Paths = new List<StiMapSvg>();
                    if (container.Paths.Count == 0)
                        container.IsNotCorrect = true;

                    container.Prepare();
                    container.IsCustom = true;
                    customMaps.Add(resource.Name, container);
                }
                catch
                {
                    var container = new StiMapSvgContainer();
                    container.Prepare();
                    container.IsNotCorrect = true;
                    container.IsCustom = true;
                    customMaps.Add(resource.Name, container);
                }

                isChanged = true;
            }

            if (cache.Count > 0)
            {
                isChanged = true;
                foreach (var pair in cache)
                {
                    customMaps.Remove(pair.Key);
                }
            }

            if (isChanged) Version++;
        }

        internal static byte[] GetDefaultIcon()
        {
            return StiImageUtils.GetByteArray(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiMap64.png");
        }

        internal static string GetIcon(StiReport report, string mapIdent)
        {
            Init(report);

            StiMapSvgContainer info;
            customMaps.TryGetValue(mapIdent, out info);

            return info?.Icon;
        }

        public static bool IsCustom(string mapIdent)
        {
            return !defaultMaps.Contains(mapIdent.ToLowerInvariant());
        }
        #endregion

        #region Events
        internal static bool IsCorrentMap(StiReport report, string mapIdent)
        {
            Init(report);

            StiMapSvgContainer info;
            customMaps.TryGetValue(mapIdent, out info);

            return (info != null)
                ? !info.IsNotCorrect
                : false;
        }

        internal static StiMapSvgContainer GetContainer(StiReport report, string mapIdent)
        {
            Init(report);

            StiMapSvgContainer info;
            customMaps.TryGetValue(mapIdent, out info);

            return info;
        }
        #endregion
    }
}