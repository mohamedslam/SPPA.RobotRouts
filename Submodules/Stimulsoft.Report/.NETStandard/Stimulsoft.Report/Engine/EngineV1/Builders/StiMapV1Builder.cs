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
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Maps;

namespace Stimulsoft.Report.Engine
{
    public class StiMapV1Builder : StiComponentV2Builder
    {
        #region Methods.Helpers

        public static StiMap RenderMap(StiMap masterMap)
        {
            var mapComp = (StiMap)masterMap.Clone();

            #region DataSource

            if (!string.IsNullOrEmpty(masterMap.KeyDataColumn))
            {
                var keys = StiDataColumn.GetDatasFromDataColumn(mapComp.Report.Dictionary, masterMap.KeyDataColumn);
                if (keys == null)
                    return mapComp;

                object[] names = null;
                object[] values = null;
                object[] groups = null;
                object[] colors = null;

                if (!string.IsNullOrEmpty(masterMap.NameDataColumn))
                    names = StiDataColumn.GetDatasFromDataColumn(mapComp.Report.Dictionary, masterMap.NameDataColumn);
                if (!string.IsNullOrEmpty(masterMap.ValueDataColumn))
                    values = StiDataColumn.GetDatasFromDataColumn(mapComp.Report.Dictionary, masterMap.ValueDataColumn);
                if (!string.IsNullOrEmpty(masterMap.GroupDataColumn))
                    groups = StiDataColumn.GetDatasFromDataColumn(mapComp.Report.Dictionary, masterMap.GroupDataColumn);
                if (!string.IsNullOrEmpty(masterMap.ColorDataColumn))
                    colors = StiDataColumn.GetDatasFromDataColumn(mapComp.Report.Dictionary, masterMap.ColorDataColumn);

                if (names == null && values == null && groups == null && colors == null)
                    return mapComp;

                var mapData = mapComp.GetMapData();
                for (int index = 0; index < keys.Length; index++)
                {
                    string key = keys[index] as string;
                    StiMapData data = null;
                    if (!string.IsNullOrEmpty(key))
                        data = mapData.FirstOrDefault(x => x.Key == key);
                    if (data == null)
                        continue;

                    if (names != null)
                    {
                        var name = names[index] as string;
                        if (!string.IsNullOrEmpty(name))
                        {
                            data.Name = name;
                        }
                        else
                        {
                            data.Name = null;
                        }
                    }

                    if (values != null)
                    {
                        data.Value = values[index] as string;
                    }

                    if (groups != null)
                    {
                        var group = groups[index] as string;
                        if (!string.IsNullOrEmpty(group))
                        {
                            data.Group = group;
                        }
                        else
                        {
                            data.Group = null;
                        }
                    }

                    if (colors != null)
                    {
                        var color = colors[index] as string;
                        if (!string.IsNullOrEmpty(color))
                        {
                            data.Color = color;
                        }
                        else
                        {
                            data.Color = null;
                        }
                    }
                }

                mapComp.MapData = StiJsonHelper.SaveToJsonString(mapData);
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
            {
                masterComp.Height = Math.Min(masterComp.Page.Height, masterComp.Parent.Height);
            }
        }

        public override StiComponent InternalRender(StiComponent masterComp)
        {
            var masterMap = masterComp as StiMap;
            return RenderMap(masterMap);
        }
        #endregion
    }
}
