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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Maps;

namespace Stimulsoft.Report.Engine
{
    public class StiMapV2Builder : StiComponentV2Builder
    {
        #region Methods.Helpers
        public static StiMap RenderMap(StiMap masterMap)
        {
            var mapComp = (StiMap)masterMap.Clone();

            #region DataSource

            if (!string.IsNullOrEmpty(masterMap.KeyDataColumn))
            {
                var keys = StiDataColumn.GetDatasFromDataColumn(mapComp.Report.Dictionary, masterMap.KeyDataColumn, null, true);
                if (keys == null)
                    return mapComp;

                object[] names = null;
                object[] values = null;
                object[] groups = null;
                object[] colors = null;

                if (!string.IsNullOrEmpty(masterMap.NameDataColumn))
                    names = StiDataColumn.GetDatasFromDataColumn(mapComp.Report.Dictionary, masterMap.NameDataColumn, null, true);

                if (!string.IsNullOrEmpty(masterMap.ValueDataColumn))
                    values = StiDataColumn.GetDatasFromDataColumn(mapComp.Report.Dictionary, masterMap.ValueDataColumn, null, true);

                if (!string.IsNullOrEmpty(masterMap.GroupDataColumn))
                    groups = StiDataColumn.GetDatasFromDataColumn(mapComp.Report.Dictionary, masterMap.GroupDataColumn, null, true);

                if (!string.IsNullOrEmpty(masterMap.ColorDataColumn))
                    colors = StiDataColumn.GetDatasFromDataColumn(mapComp.Report.Dictionary, masterMap.ColorDataColumn, null, true);

                if (names == null && values == null && groups == null && colors == null)
                    return mapComp;

                var mapData = mapComp.GetMapData();
                for (var index = 0; index < keys.Length; index++)
                {
                    var key = keys[index] as string;
                    StiMapData data = null;

                    if (!string.IsNullOrEmpty(key))
                    {
                        var idents = new List<string>() { key };
                        var mapIdents = new StiMapKeyHelper().GetMapIdents(key);
                        if (mapIdents != null)
                            idents.AddRange(mapIdents);

                        idents = idents
                            .Where(i => !string.IsNullOrWhiteSpace(i))
                            .Select(i => i?.Replace(" ", "")?.ToLowerInvariant())
                            .Distinct().ToList();

                        data = mapData.FirstOrDefault(x => idents.Any(i => x.Key?.Replace(" ", "")?.ToLowerInvariant() == i));
                    }

                    if (data == null) continue;

                    if (names != null && index >= 0 && index < names.Length)
                    {
                        var name = names[index]?.ToString();
                        data.Name = !string.IsNullOrEmpty(name) ? name : null;
                    }

                    if (values != null && index >= 0 && index < values.Length)
                    {
                        data.Value = StiValueHelper.TryToDecimal(values[index]).ToString();
                    }

                    if (groups != null && index >= 0 && index < groups.Length)
                    {
                        var group = groups[index]?.ToString();

                        if (!string.IsNullOrEmpty(group))
                            data.Group = group;
                        else
                            data.Group = null;
                    }

                    if (colors != null && index >= 0 && index < colors.Length)
                    {
                        var color = colors[index]?.ToString();
                        if (!string.IsNullOrEmpty(color))
                            data.Color = color;

                        else
                            data.Color = null;
                    }
                }

                mapComp.MapData = StiJsonHelper.SaveToJsonString(mapData);
            }
            else if (!string.IsNullOrEmpty(mapComp.Latitude) && !string.IsNullOrEmpty(mapComp.Longitude))
            {
                var pushPins = new List<string>();
                var band = mapComp.Parent as StiDataBand;
                List<object> latitudeValues = null;
                List<object> longitudeValues = null;
                mapComp.PushPins = null;

                if (band != null)
                {
                    var latitude = StiDataColumn.GetDataFromDataColumn(mapComp.Report.Dictionary, mapComp.Latitude);
                    var longitude = StiDataColumn.GetDataFromDataColumn(mapComp.Report.Dictionary, mapComp.Longitude);
                    if (latitude != null && longitude != null)
                    {
                        latitudeValues = new List<object>() { latitude };
                        longitudeValues = new List<object>() { longitude };
                    }                    
                }
                else
                {
                    latitudeValues = StiDataColumn.GetDataListFromDataColumn(mapComp.Report.Dictionary, mapComp.Latitude);
                    longitudeValues = StiDataColumn.GetDataListFromDataColumn(mapComp.Report.Dictionary, mapComp.Longitude);
                }

                if (latitudeValues != null && longitudeValues != null)
                {
                    int count = Math.Min(100, Math.Max(latitudeValues.Count, longitudeValues.Count));
                    for (int index = 0; index < count; index++)
                    {
                        var latValue = latitudeValues[index];
                        var lonValue = longitudeValues[index];

                        if (latValue == null || lonValue == null) continue;

                        var latStr = StiValueHelper.TryToDecimal(latValue).ToString(CultureInfo.InvariantCulture).Replace(",", ".");
                        var lonStr = StiValueHelper.TryToDecimal(lonValue).ToString(CultureInfo.InvariantCulture).Replace(",", ".");

                        pushPins.Add($"pp={latStr},{lonStr};60");
                    }
                    mapComp.PushPins = JsonConvert.SerializeObject(pushPins.ToArray());
                }
            }

            #endregion

            return mapComp;
        }

        #endregion

        #region Methods.Render
        public override void Prepare(StiComponent masterComp)
        {
            base.Prepare(masterComp);

            if (masterComp.Height > masterComp.Page.Height || masterComp.Height > masterComp.Parent.Height)
                masterComp.Height = Math.Min(masterComp.Page.Height, masterComp.Parent.Height);
        }

        public override StiComponent InternalRender(StiComponent masterComp)
        {
            var masterMap = masterComp as StiMap;
            return RenderMap(masterMap);
        }
        #endregion
    }
}
