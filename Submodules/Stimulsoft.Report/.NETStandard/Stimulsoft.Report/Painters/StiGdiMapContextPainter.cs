#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Maps.Geoms;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Maps;
using Stimulsoft.Report.Maps.Helpers;
using Stimulsoft.Report.Painters.Context.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

#if STIDRAWING
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using Graphics = Stimulsoft.Drawing.Graphics;
using HatchBrush = Stimulsoft.Drawing.Drawing2D.HatchBrush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using TextureBrush = Stimulsoft.Drawing.TextureBrush;
using Brush = Stimulsoft.Drawing.Brush;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Pen = Stimulsoft.Drawing.Pen;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiGdiMapContextPainter : 
        IStiAnimationContextPainter<StiGeom>
    {
        #region Fields
        private StiMap map;
        private HeatmapInfo heatmapInfo;
        private HeatmapWithGroupInfo heatmapWithGroupInfo;
        private NoneInfo noneInfo;
        private Dictionary<string, StiMapGroup> hashGroup = new Dictionary<string, StiMapGroup>();
        private StiStyleColorsContainer colorsContainer = new StiStyleColorsContainer();
        #endregion

        #region Properties
        internal float IndividualStep { get; set; } = 0;

        internal SolidBrush DefaultBrush { get; set; }

        internal StiSolidBrush DefaultBrush1 { get; set; }

        internal List<StiMapData> MapData { get; private set; }

        internal bool IsBorderEmpty { get; set; }

        internal StiMapStyle MapStyle { get; set; }

        public StiDataTable DataTable { get; set; }

        public List<string> CheckedMapKeys { get; set; }

        public string MouseOverKey { get; set; }

        public StiAnimationEngine AnimationEngine { get; set; }

        public List<StiGeom> Geoms { get; } = new List<StiGeom>();
        #endregion

        #region Additional Classes

        #region class StiMapGroup
        private class StiMapGroup
        {
            public double? MinValue;
            public double? MaxValue;
            public Brush Fill;
            public StiSolidBrush Fill1;
        }
        #endregion

        #region class StiStyleColorsContainer
        private class StiStyleColorsContainer
        {
            #region Fields
            private List<Color> stackColors = new List<Color>();
            private StiGdiMapContextPainter painter;
            #endregion

            #region Methods
            public Brush GetColor(int index, int count)
            {
                if (stackColors.Count == 0)
                    return painter.DefaultBrush;

                var color = GetColors(count)[index];
                return new SolidBrush(color);
            }

            public StiSolidBrush GetColor1(int index, int count)
            {
                if (stackColors.Count == 0)
                    return painter.DefaultBrush1;

                var color = GetColors(count)[index];
                return new StiSolidBrush(color);
            }

            private Color[] GetColors(int seriesCount)
            {
                var colors = new Color[seriesCount];

                int styleColorIndex = 0;
                int dist = 0;

                for (int colorIndex = 0; colorIndex < seriesCount; colorIndex++)
                {
                    if (this.stackColors.Count == 0 || this.stackColors.Count < styleColorIndex)
                    {
                        colors[colorIndex] = Color.FromArgb(255, 112, 173, 71);
                    }
                    else
                    {
                        if (dist != 0)
                        {
                            var color = this.stackColors[styleColorIndex];

                            int a = Math.Min(color.A + dist, 255);
                            int r = Math.Min(color.R + dist, 255);
                            int g = Math.Min(color.G + dist, 255);
                            int b = Math.Min(color.B + dist, 255);

                            colors[colorIndex] = Color.FromArgb(a, r, g, b);
                        }
                        else
                        {
                            colors[colorIndex] = this.stackColors[styleColorIndex];
                        }
                    }

                    styleColorIndex++;
                    if (styleColorIndex == this.stackColors.Count)
                    {
                        styleColorIndex = 0;
                        dist = 50;
                    }
                }
                return colors;
            }

            public void Init(StiMap map, StiGdiMapContextPainter painter)
            {
                this.painter = painter;

                this.stackColors.Clear();
                var colors = map.GetCurrentStyleColors();

                this.stackColors.AddRange(colors);
            }
            #endregion
        }
        #endregion

        #region class HeatmapInfo
        private class HeatmapInfo
        {
            #region Fields
            private StiGdiMapContextPainter painter;
            private double min;
            private double max;
            private Color color;
            private Color zeroColor;
            private StiHeatmapFillMode mode;
            #endregion

            #region Methods
            public Brush GetBrush(StiMapData data)
            {
                if (data == null || data.Value == null || !double.TryParse(data.Value, out var value))
                    return this.painter.DefaultBrush;

                if (value == 0)
                {
                    if (zeroColor == Color.Transparent)
                        return this.painter.DefaultBrush;

                    return new SolidBrush(zeroColor);
                }
                if (min == max)
                    return new SolidBrush(color);

                var factor = (float)(0.85 * (max - value) / (max - min));
                return (mode == StiHeatmapFillMode.Lightness)
                    ? new SolidBrush(StiColorUtils.ChangeLightness(color, factor))
                    : new SolidBrush(StiColorUtils.ChangeDarkness(color, factor));
            }

            public StiBrush GetBrush1(StiMapData data)
            {
                if (data == null || data.Value == null || !double.TryParse(data.Value, out var value))
                    return this.painter.DefaultBrush1;

                if (value == 0)
                {
                    if (zeroColor == Color.Transparent)
                        return this.painter.DefaultBrush1;

                    return new StiSolidBrush(zeroColor);
                }
                if (min == max)
                    return new StiSolidBrush(color);

                var factor = (float)(0.85 * (max - value) / (max - min));
                return (mode == StiHeatmapFillMode.Lightness)
                    ? new StiSolidBrush(StiColorUtils.ChangeLightness(color, factor))
                    : new StiSolidBrush(StiColorUtils.ChangeDarkness(color, factor));
            }
            #endregion

            public HeatmapInfo(StiGdiMapContextPainter painter, StiMap map, List<StiMapData> mapData)
            {
                this.painter = painter;
                bool isFirst = true;
                foreach (var data in mapData)
                {
                    if (data.Value == null) continue;

                    if (data.Value == null || !double.TryParse(data.Value, out var value)) continue;

                    if (isFirst)
                    {
                        isFirst = false;

                        min = value;
                        max = value;
                    }
                    else
                    {
                        if (value < min)
                            min = value;
                        else if (value > max)
                            max = value;
                    }
                }

                var mapStyle = StiMap.GetMapStyle(map);

                this.color = mapStyle.Heatmap.Color;
                this.zeroColor = mapStyle.Heatmap.ZeroColor;
                this.mode = mapStyle.Heatmap.Mode;
            }
        }
        #endregion

        #region class HeatmapWithGroupInfo
        private class HeatmapWithGroupInfo
        {
            #region Fields
            private StiGdiMapContextPainter painter;
            private Dictionary<string, double[]> hash = new Dictionary<string, double[]>();
            private Dictionary<string, Color> hashColors = new Dictionary<string, Color>();
            private Color zeroColor;
            private StiHeatmapFillMode mode;
            #endregion

            #region Methods
            public Brush GetBrush(StiMapData data)
            {
                if (data.Group == null)
                    return null;

                if (!hash.ContainsKey(data.Group))
                    return this.painter.DefaultBrush;

                if (data.Value == null || !double.TryParse(data.Value, out var value))
                    return this.painter.DefaultBrush;
                if (value == 0)
                    return new SolidBrush(zeroColor);

                var color = hashColors[data.Group];
                var values = hash[data.Group];
                if (values[0] == values[1])
                    return new SolidBrush(color);

                return (mode == StiHeatmapFillMode.Lightness)
                    ? new SolidBrush(StiColorUtils.ChangeLightness(color, (float)(0.85f * (values[1] - value) / (values[1] - values[0]))))
                    : new SolidBrush(StiColorUtils.ChangeDarkness(color, (float)(0.85f * (values[1] - value) / (values[1] - values[0]))));
            }

            public StiBrush GetBrush1(StiMapData data)
            {
                if (data.Group == null) return null;

                if (!hash.ContainsKey(data.Group))
                    return this.painter.DefaultBrush1;

                if (data.Value == null || !double.TryParse(data.Value, out var value))
                    return this.painter.DefaultBrush1;
                if (value == 0)
                    return new StiSolidBrush(zeroColor);

                var color = hashColors[data.Group];
                var values = hash[data.Group];
                if (values[0] == values[1])
                    return new StiSolidBrush(color);

                return (mode == StiHeatmapFillMode.Lightness)
                    ? new StiSolidBrush(StiColorUtils.ChangeLightness(color, (float)(0.85f * (values[1] - value) / (values[1] - values[0]))))
                    : new StiSolidBrush(StiColorUtils.ChangeDarkness(color, (float)(0.85f * (values[1] - value) / (values[1] - values[0]))));
            }
            #endregion

            public HeatmapWithGroupInfo(StiGdiMapContextPainter painter, StiMap map, List<StiMapData> mapData)
            {
                this.painter = painter;

                var style = StiMap.GetMapStyle(map);
                this.zeroColor = style.HeatmapWithGroup.ZeroColor;
                this.mode = style.HeatmapWithGroup.Mode;

                int index = 0;
                var colors = style.HeatmapWithGroup.Colors;
                foreach (var data in mapData)
                {
                    var key = data.Group;
                    if (key == null) continue;
                    if (data.Value == null) continue;

                    if (data.Value == null || !double.TryParse(data.Value, out var value)) continue;

                    if (!hash.ContainsKey(key))
                    {
                        var values = new double[2] { value, value };
                        hash.Add(key, values);
                    }
                    else
                    {
                        var values = hash[key];
                        if (value < values[0])
                            values[0] = value;
                        else if (value > values[1])
                            values[1] = value;
                    }

                    if (!hashColors.ContainsKey(key))
                    {
                        var color = (colors.Length == 0 || colors.Length < index)
                            ? painter.DefaultBrush.Color
                            : colors[index];

                        index++;
                        if (index >= colors.Length)
                            index = 0;

                        hashColors.Add(key, color);
                    }
                }
            }
        }
        #endregion

        #region class NoneInfo
        private class NoneInfo
        {
            public NoneInfo()
            {
                this.Colors = StiMapHelper.GetColors();
            }

            #region Fields

            private Color[] Colors;
            private int index = 0;

            #endregion

            #region Methods

            public StiSolidBrush GetBrush()
            {
                var color = this.Colors[index];

                index++;
                if (index >= this.Colors.Length)
                    index = 0;

                return new StiSolidBrush(color);
            }

            #endregion
        }
        #endregion

        #endregion

        #region Methods.Render
        public void Render(Graphics graphics, bool useZoom)
        {
            Render(graphics, useZoom, map.GetPaintRectangle(true, true));
        }

        public Brush GetGeomBrush(StiMapData data)
        {
            if (map.MapType == StiMapType.Individual)
            {
                if (this.map.ColorEach)
                {
                    if (data == null)
                        return new SolidBrush(this.MapStyle.DefaultColor);

                    var brush = ParseHexColor(data.Color);
                    if (brush != null) return brush;

                    return new SolidBrush(this.noneInfo.GetBrush().Color);
                }
                else
                {
                    return new SolidBrush(StiColorUtils.ChangeLightness(this.MapStyle.IndividualColor, this.IndividualStep));
                }
            }

            switch (map.MapType)
            {
                case StiMapType.Individual:
                    return DefaultBrush;

                case StiMapType.Heatmap:
                    {
                        return heatmapInfo.GetBrush(data);
                    }

                case StiMapType.Group:
                    {
                        return (data != null && data.Group != null && hashGroup.ContainsKey(data.Group))
                            ? hashGroup[data.Group].Fill
                            : new SolidBrush(this.MapStyle.DefaultColor);
                    }

                case StiMapType.HeatmapWithGroup:
                    {
                        if (data == null || data.Group == null)
                            return new SolidBrush(this.MapStyle.DefaultColor);
                        else
                            return heatmapWithGroupInfo.GetBrush(data);
                    }
            }

            return DefaultBrush;
        }

        public StiBrush GetGeomBrush1(StiMapData data)
        {
            if (map.MapType == StiMapType.Individual)
            {
                if (this.map.ColorEach)
                {
                    if (data == null)
                        return new StiSolidBrush(this.MapStyle.DefaultColor);

                    var brush = ParseHexColor1(data.Color);
                    if (brush != null) return brush;

                    return new StiSolidBrush(this.noneInfo.GetBrush().Color);
                }
                else
                {
                    return new StiSolidBrush(StiColorUtils.ChangeLightness(this.MapStyle.IndividualColor, this.IndividualStep));
                }
            }

            switch (map.MapType)
            {
                case StiMapType.Individual:
                    return DefaultBrush1;

                case StiMapType.Heatmap:
                    {
                        if (data.Value == null)
                            return DefaultBrush1;
                        else
                            return heatmapInfo.GetBrush1(data);
                    }

                case StiMapType.Group:
                    {
                        return (data.Group != null && hashGroup.ContainsKey(data.Group))
                            ? hashGroup[data.Group].Fill1
                            : new StiSolidBrush(this.MapStyle.DefaultColor);
                    }

                case StiMapType.HeatmapWithGroup:
                    {
                        if (data.Group == null)
                            return new StiSolidBrush(this.MapStyle.DefaultColor);
                        else
                            return heatmapWithGroupInfo.GetBrush1(data);
                    }
            }

            return DefaultBrush1;
        }

        protected List<object> GetValues(IStiMeter meter)
        {
            if (DataTable == null || meter == null)
                return null;

            var index = DataTable.Meters.IndexOf(meter);
            if (index == -1)
                return null;

            return DataTable.Rows.GetArrayItem(index).ToList();
        }

        protected static List<object> GetValues(StiDataTable table, IStiMeter meter)
        {
            if (table == null || meter == null)
                return null;

            var index = table.Meters.IndexOf(meter);
            if (index == -1)
                return null;

            return table.Rows.GetArrayItem(index).ToList();
        }

        internal void PrepareDataColumns()
        {
            if (map.DataFrom == StiMapSource.Manual)
            {
                this.MapData = map.GetMapData();
                return;
            }

            this.MapData = StiMap.GetDefaultMapData(map.Report, map.MapIdent, map.Language);

            List<object> keyValues = null;
            List<object> nameValues = null;
            List<object> valueValues = null;
            List<object> groupValues = null;
            List<object> colorValues = null;

            if (this.DataTable == null && map.DataTable != null)
                this.DataTable = map.DataTable;

            if (this.DataTable != null)
            {
                keyValues = GetValues(DataTable?.Meters?.FirstOrDefault(f => f is IStiKeyMapMeter));
                nameValues = GetValues(DataTable?.Meters?.FirstOrDefault(f => f is IStiNameMapMeter));
                valueValues = GetValues(DataTable?.Meters?.FirstOrDefault(f => f is IStiValueMapMeter));
                groupValues = GetValues(DataTable?.Meters?.FirstOrDefault(f => f is IStiGroupMapMeter));
                colorValues = GetValues(DataTable?.Meters?.FirstOrDefault(f => f is IStiColorMapMeter));
            }
            else
            {
                try
                {
                    map.Report.Dictionary.Connect();
                    map.Report.Dictionary.ConnectDataTransformations();

                    keyValues = !string.IsNullOrEmpty(map.KeyDataColumn) ? StiDataColumn.GetDataListFromDataColumn(map.Report.Dictionary, map.KeyDataColumn) : null;
                    nameValues = !string.IsNullOrEmpty(map.NameDataColumn) ? StiDataColumn.GetDataListFromDataColumn(map.Report.Dictionary, map.NameDataColumn) : null;
                    valueValues = !string.IsNullOrEmpty(map.ValueDataColumn) ? StiDataColumn.GetDataListFromDataColumn(map.Report.Dictionary, map.ValueDataColumn) : null;
                    groupValues = !string.IsNullOrEmpty(map.GroupDataColumn) ? StiDataColumn.GetDataListFromDataColumn(map.Report.Dictionary, map.GroupDataColumn) : null;
                    colorValues = !string.IsNullOrEmpty(map.ColorDataColumn) ? StiDataColumn.GetDataListFromDataColumn(map.Report.Dictionary, map.ColorDataColumn) : null;

                    map.Report.Dictionary.Disconnect();
                }
                catch { }
            }

            bool allowGss = StiGssMapHelper.AllowGss(this.map.MapIdent);

            if (keyValues != null)
            {
                keyValues = new StiMapKeyHelper().GetMapKeysFromNames(keyValues, map.MapIdent, map.Language, map.Report)?.Cast<object>().ToList();
            }

            if (keyValues == null) return;

            try
            {
                #region find min
                int min = 0;
                if (keyValues != null)
                {
                    if (min == 0)
                    {
                        min = keyValues.Count;
                    }
                    else
                    {
                        if (keyValues.Count < min)
                            min = keyValues.Count;
                    }
                }
                if (nameValues != null)
                {
                    if (min == 0)
                    {
                        min = nameValues.Count;
                    }
                    else
                    {
                        if (nameValues.Count < min)
                            min = nameValues.Count;
                    }
                }
                if (valueValues != null)
                {
                    if (min == 0)
                    {
                        min = valueValues.Count;
                    }
                    else
                    {
                        if (valueValues.Count < min)
                            min = valueValues.Count;
                    }
                }
                if (groupValues != null)
                {
                    if (min == 0)
                    {
                        min = groupValues.Count;
                    }
                    else
                    {
                        if (groupValues.Count < min)
                            min = groupValues.Count;
                    }
                }
                if (colorValues != null)
                {
                    if (min == 0)
                    {
                        min = colorValues.Count;
                    }
                    else
                    {
                        if (colorValues.Count < min)
                            min = colorValues.Count;
                    }
                }
                #endregion

                #region Key
                if (keyValues != null && this.MapData != null)
                {
                    foreach (var data in this.MapData)
                    {
                        data.Group = null;
                        data.Value = null;
                        if (colorValues != null)
                            data.Color = null;
                    }

                    var gssData = allowGss
                        ? StiGssMapHelper.Get(this.map.MapIdent)
                        : null;

                    for (int index = 0; index < min; index++)
                    {
                        string key = null;
                        if (keyValues.Count <= index || keyValues[index] == null) continue;

                        key = keyValues[index].ToString();
                        if (string.IsNullOrEmpty(key)) continue;
                        key = key.ToLowerInvariant();

                        if (allowGss && StiGssMapHelper.IsGssValue(key))
                        {
                            if (gssData.TryGetValue(key, out var state))
                            {
                                key = state.ToLowerInvariant();
                            }
                            else
                            {
                                key = string.Empty;
                            }
                        }

                        var data = this.MapData.FirstOrDefault(x => x.Key.ToLowerInvariant() == key);
                        if (data == null) continue;

                        if (nameValues != null && nameValues[index] != null)
                        {
                            var name = nameValues[index].ToString();
                            data.Name = name;
                        }

                        if (valueValues != null && valueValues[index] != null)
                        {
                            var value = valueValues[index].ToString();
                            data.Value = value;
                        }

                        if (groupValues != null && groupValues[index] != null)
                        {
                            var group = groupValues[index].ToString();
                            data.Group = group;
                        }

                        if (colorValues != null && colorValues[index] != null)
                        {
                            var color = colorValues[index].ToString();
                            data.Color = color;
                        }
                    }
                }
                #endregion
            }
            catch { }
        }

        internal static List<StiMapData> GetDataColumns(StiMap map, StiDataTable dataTable)
        {
            if (map == null)
                return null;

            if (map.DataFrom == StiMapSource.Manual)
                return map.GetMapData();

            var mapData = StiMap.GetDefaultMapData(map.Report, map.MapIdent, map.Language);

            List<object> keyValues = null;
            List<object> nameValues = null;
            List<object> valueValues = null;
            List<object> groupValues = null;
            List<object> colorValues = null;

            if (mapData == null && map.DataTable != null)
                dataTable = map.DataTable;

            if (dataTable != null)
            {
                keyValues = GetValues(dataTable, dataTable?.Meters?.FirstOrDefault(f => f is IStiKeyMapMeter));
                nameValues = GetValues(dataTable, dataTable?.Meters?.FirstOrDefault(f => f is IStiNameMapMeter));
                valueValues = GetValues(dataTable, dataTable?.Meters?.FirstOrDefault(f => f is IStiValueMapMeter));
                groupValues = GetValues(dataTable, dataTable?.Meters?.FirstOrDefault(f => f is IStiGroupMapMeter));
                colorValues = GetValues(dataTable, dataTable?.Meters?.FirstOrDefault(f => f is IStiColorMapMeter));
            }
            else
            {
                try
                {
                    map.Report.Dictionary.Connect();
                    map.Report.Dictionary.ConnectDataTransformations();

                    keyValues = !string.IsNullOrEmpty(map.KeyDataColumn) ? StiDataColumn.GetDataListFromDataColumn(map.Report.Dictionary, map.KeyDataColumn) : null;
                    nameValues = !string.IsNullOrEmpty(map.NameDataColumn) ? StiDataColumn.GetDataListFromDataColumn(map.Report.Dictionary, map.NameDataColumn) : null;
                    valueValues = !string.IsNullOrEmpty(map.ValueDataColumn) ? StiDataColumn.GetDataListFromDataColumn(map.Report.Dictionary, map.ValueDataColumn) : null;
                    groupValues = !string.IsNullOrEmpty(map.GroupDataColumn) ? StiDataColumn.GetDataListFromDataColumn(map.Report.Dictionary, map.GroupDataColumn) : null;
                    colorValues = !string.IsNullOrEmpty(map.ColorDataColumn) ? StiDataColumn.GetDataListFromDataColumn(map.Report.Dictionary, map.ColorDataColumn) : null;

                    map.Report.Dictionary.Disconnect();
                }
                catch { }
            }

            bool allowGss = StiGssMapHelper.AllowGss(map.MapIdent);

            if (keyValues != null)
            {
                keyValues = new StiMapKeyHelper().GetMapKeysFromNames(keyValues, map.MapIdent, map.Language, map.Report)?.Cast<object>().ToList();
            }

            if (keyValues == null) return null;

            try
            {
                #region find min
                int min = 0;
                if (keyValues != null)
                {
                    if (min == 0)
                    {
                        min = keyValues.Count;
                    }
                    else
                    {
                        if (keyValues.Count < min)
                            min = keyValues.Count;
                    }
                }
                if (nameValues != null)
                {
                    if (min == 0)
                    {
                        min = nameValues.Count;
                    }
                    else
                    {
                        if (nameValues.Count < min)
                            min = nameValues.Count;
                    }
                }
                if (valueValues != null)
                {
                    if (min == 0)
                    {
                        min = valueValues.Count;
                    }
                    else
                    {
                        if (valueValues.Count < min)
                            min = valueValues.Count;
                    }
                }
                if (groupValues != null)
                {
                    if (min == 0)
                    {
                        min = groupValues.Count;
                    }
                    else
                    {
                        if (groupValues.Count < min)
                            min = groupValues.Count;
                    }
                }
                if (colorValues != null)
                {
                    if (min == 0)
                    {
                        min = colorValues.Count;
                    }
                    else
                    {
                        if (colorValues.Count < min)
                            min = colorValues.Count;
                    }
                }
                #endregion

                #region Key
                if (keyValues != null && mapData != null)
                {
                    foreach (var data in mapData)
                    {
                        data.Group = null;
                        data.Value = null;
                        if (colorValues != null)
                            data.Color = null;
                    }

                    var gssData = allowGss
                        ? StiGssMapHelper.Get(map.MapIdent)
                        : null;

                    for (int index = 0; index < min; index++)
                    {
                        string key = null;
                        if (keyValues.Count <= index || keyValues[index] == null) continue;

                        key = keyValues[index].ToString();
                        if (string.IsNullOrEmpty(key)) continue;
                        key = key.ToLowerInvariant();

                        if (allowGss && StiGssMapHelper.IsGssValue(key))
                        {
                            if (gssData.TryGetValue(key, out var state))
                            {
                                key = state.ToLowerInvariant();
                            }
                            else
                            {
                                key = string.Empty;
                            }
                        }

                        var data = mapData.FirstOrDefault(x => x.Key.ToLowerInvariant() == key);
                        if (data == null) continue;

                        if (nameValues != null && nameValues[index] != null)
                        {
                            var name = nameValues[index].ToString();
                            data.Name = name;
                        }

                        if (valueValues != null && valueValues[index] != null)
                        {
                            var value = valueValues[index].ToString();
                            data.Value = value;
                        }

                        if (groupValues != null && groupValues[index] != null)
                        {
                            var group = groupValues[index].ToString();
                            data.Group = group;
                        }

                        if (colorValues != null && colorValues[index] != null)
                        {
                            var color = colorValues[index].ToString();
                            data.Color = color;
                        }
                    }
                }
                #endregion
            }
            catch { }

            return mapData;
        }

        public static StiMapInteractionContainer RenderInteractionContainer(StiReport report, StiMap map, 
            bool showValue, bool shortValue, bool showZeros,
            string mapID, string lang, RectangleD rect)
        {
            if (mapID == StiMapID.ChinaWithHongKongAndMacau.ToString() ||
                mapID == StiMapID.ChinaWithHongKongMacauAndTaiwan.ToString())
                mapID = StiMapID.China.ToString();

            var geomContainer = StiMapLoader.GetGeomsObject(report, mapID, lang);
            var svgContainer = StiMapLoader.LoadResource(report, mapID, lang);

            if (svgContainer == null)
                return null;

            var container = new StiMapInteractionContainer(svgContainer.HashPaths, geomContainer.Width, geomContainer.Height);

            var mapData = GetDataColumns(map, null);

            #region Calculate zoom
            float zoom = 1f;
            if (rect.Width != 0 && rect.Height != 0)
            {
                zoom = Math.Min(
                    (float)rect.Width / (float)geomContainer.Width,
                    (float)rect.Height / (float)geomContainer.Height);

                if (zoom == 0)
                    zoom = (float)StiScale.X;

                container.Zoom = zoom;
            }
            #endregion

            foreach (var geoms in geomContainer.Geoms.OrderBy(x => x.Key))
            {
                var listPaths = new List<GraphicsPath>();

                var path = new GraphicsPath();
                var lastPoint = PointF.Empty;

                foreach (var geom in geoms.Geoms)
                {
                    switch (geom.GeomType)
                    {
                        case StiMapGeomType.MoveTo:
                            {
                                if (path.PointCount > 0)
                                {
                                    path.CloseFigure();

                                    listPaths.Add(path);
                                    path = new GraphicsPath();
                                }

                                var moveTo = (StiMoveToMapGeom)geom;
                                lastPoint = new PointF((float)moveTo.X * zoom, (float)moveTo.Y * zoom);
                            }
                            break;

                        case StiMapGeomType.Line:
                            {
                                var line = (StiLineMapGeom)geom;

                                var point = new PointF((float)line.X * zoom, (float)line.Y * zoom);
                                path.AddLine(lastPoint, point);
                                lastPoint = point;
                            }
                            break;

                        case StiMapGeomType.EllipticalArc:
                            {
                                var arc = (StiEllipticalArcMapGeom)geom;

                                arc.AddToPath(path);
                                lastPoint = new PointF((float)arc.EndX * zoom, (float)arc.EndY * zoom);
                            }
                            break;

                        case StiMapGeomType.QuadraticCurve:
                            {
                                var arc = (StiQuadraticCurveMapGeom)geom;

                                arc.AddToPath(path);
                                lastPoint = new PointF((float)arc.EndX * zoom, (float)arc.EndY * zoom);
                            }
                            break;

                        case StiMapGeomType.Bezier:
                            {
                                var bezier = (StiBezierMapGeom)geom;

                                var point1 = new PointF((float)bezier.X1 * zoom, (float)bezier.Y1 * zoom);
                                var point2 = new PointF((float)bezier.X2 * zoom, (float)bezier.Y2 * zoom);
                                var point3 = new PointF((float)bezier.X3 * zoom, (float)bezier.Y3 * zoom);
                                path.AddBezier(lastPoint, point1, point2, point3);

                                lastPoint = point3;
                            }
                            break;

                        case StiMapGeomType.Beziers:
                            {
                                var beziers = (StiBeziersMapGeom)geom;

                                var list = new List<PointF>()
                                {
                                    lastPoint
                                };
                                var p = PointF.Empty;
                                for (var index = 0; index < beziers.Array.Length; index += 2)
                                {
                                    p = new PointF((float)beziers.Array[index] * zoom, (float)beziers.Array[index + 1] * zoom);
                                    list.Add(p);
                                }

                                list.Add(p);
                                lastPoint = p;

                                if (list.Count == 9)
                                {
                                    list.RemoveAt(8);
                                    list.RemoveAt(7);
                                }

                                path.AddBeziers(list.ToArray());
                            }
                            break;

                        case StiMapGeomType.Close:
                            {
                                path.CloseFigure();
                                listPaths.Add(path);
                                path = new GraphicsPath();
                            }
                            break;
                    }
                }

                if (path.PointCount > 0)
                {
                    path.CloseFigure();
                    listPaths.Add(path);
                }

                container.Add(geoms.Key, listPaths);

                if (showValue && mapData != null)
                {
                    var info = mapData.FirstOrDefault(x => x.Key == geoms.Key);
                    if (info != null && info.Value != null)
                    {
                        string valueStr = null;
                        if (shortValue)
                        {
                            if (double.TryParse(info.Value, out var resValue))
                            {
                                if (resValue == 0d && !showZeros)
                                {
                                    continue;
                                }
                                else
                                {
                                    valueStr = StiAbbreviationNumberFormatHelper.Format(resValue);
                                }
                            }
                        }

                        if (valueStr == null)
                        {
                            if (double.TryParse(info.Value, out var resValue) && resValue == 0d && !showZeros)
                                continue;

                            valueStr = info.Value;
                        }

                        container.SetValue(geoms.Key, valueStr);
                    }
                }
            }

            return container;
        }

        private void DrawState(Graphics g, GraphicsPath path, string key)
        {
            if (this.CheckedMapKeys != null && this.CheckedMapKeys.Contains(key))
            {
                using (var fill = new HatchBrush(HatchStyle.WideUpwardDiagonal, Color.FromArgb(100, Color.White), Color.Transparent))
                using (var bmp = new Bitmap(8, 8))
                using (var G = Graphics.FromImage(bmp))
                {
                    G.FillRectangle(fill, 0, 0, 8, 8);
                    var tb = new TextureBrush(bmp);
                    tb.ScaleTransform((float)StiScale.X, (float)StiScale.Y);
                    g.FillPath(tb, path);
                }
            }

            if (MouseOverKey == key)
            {
                using (var fill = new SolidBrush(Color.FromArgb(100, Color.White)))
                {
                    g.FillPath(fill, path);
                }
            }
        }

        public void Render(Graphics g, bool useZoom, RectangleD rect, bool center = true)
        {
            if (map.MapIdent == StiMapID.ChinaWithHongKongAndMacau.ToString() ||
                map.MapIdent == StiMapID.ChinaWithHongKongMacauAndTaiwan.ToString())
                map.MapIdent = StiMapID.China.ToString();

            MouseOverKey = StiMapInteractionHelper.GetKey(map);

            this.MapStyle = StiMap.GetMapStyle(map);
            string name = !string.IsNullOrEmpty(this.MapStyle.DashboardName) ? this.MapStyle.DashboardName : this.MapStyle.Name;

            this.DefaultBrush = new SolidBrush(this.MapStyle.DefaultColor);
            this.DefaultBrush1 = new StiSolidBrush(this.MapStyle.DefaultColor);

            PrepareDataColumns();
            UpdateGroupedData();
            UpdateHeatmapWithGroup();

            float zoom = 1f;
            float translateX = 0;
            float translateY = 0;
            var geomContainer = StiMapLoader.GetGeomsObject(map.Report, map.MapIdent, map.Language);
            bool skipLabels = false;
            if (useZoom && map.Stretch)
            {
                zoom = Math.Min(
                    (float)rect.Width / (float)geomContainer.Width,
                    (float)rect.Height / (float)geomContainer.Height);

                if (zoom == 0)
                    zoom = (float)StiScale.X;

                if (!map.Report.IsExporting)
                {
                    if (zoom > (float)StiScale.X)
                        zoom = (float)StiScale.X;
                }

                if (zoom < (float)(0.2 * StiScale.X))
                    skipLabels = true;

                g.ScaleTransform(zoom, zoom);

                if (center)
                {
                    var destWidth = rect.Width / zoom;
                    var destHeight = rect.Height / zoom;

                    translateX = (float)((destWidth - geomContainer.Width) / 2);
                    translateY = (float)((destHeight - geomContainer.Height) / 2);
                    g.TranslateTransform(
                        (float)((destWidth - geomContainer.Width) / 2),
                        (float)((destHeight - geomContainer.Height) / 2));
                }
            }

            var hashLabels = new List<string>();

            var allTime = TimeSpan.Zero;
            float individualStepValue = 0.5f / geomContainer.Geoms.Count;
            this.IndividualStep = individualStepValue;
            foreach (var geoms in geomContainer.Geoms.OrderBy(x => x.Key))
            {
                var path = new GraphicsPath();
                var lastPoint = PointF.Empty;

                var data = this.MapData.FirstOrDefault(x => x.Key == geoms.Key);
                var fill = GetGeomBrush(data);

                geoms.Animation.BeginTimeCorrect = TimeSpan.Zero;
                fill = (Brush)StiAnimationEngine.GetAnimationOpacity(this, fill, geoms.Animation);
                var borderColor = (Color)StiAnimationEngine.GetAnimationOpacity(this, MapStyle.BorderColor, geoms.Animation);
                if (allTime < geoms.Animation.BeginTime + geoms.Animation.BeginTime) allTime = geoms.Animation.BeginTime + geoms.Animation.BeginTime;

                this.IndividualStep += individualStepValue;

                foreach (var geom in geoms.Geoms)
                {
                    switch (geom.GeomType)
                    {
                        case StiMapGeomType.MoveTo:
                            {
                                if (path.PointCount > 0)
                                {
                                    path.CloseFigure();
                                    g.FillPath(fill, path);

                                    DrawState(g, path, geoms.Key);

                                    if (!IsBorderEmpty)
                                    {
                                        using (var pen = new Pen(borderColor, (float)MapStyle.BorderSize))
                                        {
                                            g.DrawPath(pen, path);
                                        }
                                    }

                                    path.Dispose();
                                    path = new GraphicsPath();
                                }

                                var moveTo = (StiMoveToMapGeom)geom;
                                lastPoint = new PointF((float)moveTo.X, (float)moveTo.Y);
                            }
                            break;

                        case StiMapGeomType.Line:
                            {
                                var line = (StiLineMapGeom)geom;

                                var point = new PointF((float)line.X, (float)line.Y);
                                path.AddLine(lastPoint, point);
                                lastPoint = point;
                            }
                            break;

                        case StiMapGeomType.Bezier:
                            {
                                var bezier = (StiBezierMapGeom)geom;

                                var point1 = new PointF((float)bezier.X1, (float)bezier.Y1);
                                var point2 = new PointF((float)bezier.X2, (float)bezier.Y2);
                                var point3 = new PointF((float)bezier.X3, (float)bezier.Y3);
                                path.AddBezier(lastPoint, point1, point2, point3);

                                lastPoint = point3;
                            }
                            break;

                        case StiMapGeomType.Beziers:
                            {
                                var beziers = (StiBeziersMapGeom)geom;

                                var list = new List<PointF>()
                                {
                                    lastPoint
                                };
                                var p = PointF.Empty;
                                for (var index = 0; index < beziers.Array.Length; index += 2)
                                {
                                    p = new PointF((float)beziers.Array[index], (float)beziers.Array[index + 1]);
                                    list.Add(p);
                                }

                                list.Add(p);
                                lastPoint = p;

                                if ((list.Count - 1) % 3 == 2) list.RemoveAt(list.Count - 1);
                                if ((list.Count - 1) % 3 == 1) list.RemoveAt(list.Count - 1);

                                path.AddBeziers(list.ToArray());
                            }
                            break;

                        case StiMapGeomType.Close:
                            {
                                path.CloseFigure();
                                g.FillPath(fill, path);

                                DrawState(g, path, geoms.Key);

                                if (!IsBorderEmpty)
                                {
                                    using (var pen = new Pen(borderColor, (float)MapStyle.BorderSize))
                                    {
                                        g.DrawPath(pen, path);
                                    }
                                }

                                path.Dispose();
                                path = new GraphicsPath();
                            }
                            break;
                    }
                }

                if (path.PointCount > 0)
                {
                    path.CloseFigure();
                    g.FillPath(fill, path);

                    DrawState(g, path, geoms.Key);

                    if (!IsBorderEmpty)
                    {
                        using (var pen = new Pen(borderColor, (float)MapStyle.BorderSize))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }

                path.Dispose();

                if (map.ShowValue || map.DisplayNameType != StiDisplayNameType.None)
                    hashLabels.Add(geoms.Key);
            }

            if (hashLabels.Count > 0)
            {
                var svgContainer = StiMapLoader.LoadResource(map.Report, map.MapIdent, map.Language);

                float scaceSize = 1f;
                float fontSize = (StiScale.Factor == 1.0d) ? 18 : (float)(10 * StiScale.X);
                if (svgContainer.TextScale != null)
                {
                    scaceSize = (float)svgContainer.TextScale.GetValueOrDefault();
                    fontSize *= scaceSize;
                    skipLabels = false;
                }

                if (!skipLabels)
                {
                    var smoothingMode = g.SmoothingMode;
                    var textRenderingHint = g.TextRenderingHint;
                    var interpolationMode = g.InterpolationMode;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    var typeface = new Font("Calibri", fontSize);
                    var foregroundDark = new SolidBrush(MapStyle.LabelForeground);
                    var foregroundLight = new SolidBrush(Color.FromArgb(200, MapStyle.LabelShadowForeground));

                    foreach (var key in hashLabels)
                    {
                        var animation = svgContainer.Geoms.Find(geom => geom.Key == key).Animation;
                        animation.BeginTimeCorrect = allTime;
                        var path = svgContainer.HashPaths[key];
                        var skipText = false;
                        if (map.DisplayNameType == StiDisplayNameType.Short)
                            skipText = path.SkipTextIso == null ? path.SkipText : path.SkipTextIso.GetValueOrDefault();
                        else
                            skipText = path.SkipText;
                        if (skipText) continue;

                        string text = GetPathText(path, key);
                        if (!string.IsNullOrEmpty(text))
                        {
                            Rectangle bounds = GetPathRect(path);
                            var textSize = (map.DisplayNameType == StiDisplayNameType.Full && path.SetMaxWidth)
                                ? g.MeasureString(text, typeface, (int)(bounds.Width * StiScale.X))
                                : g.MeasureString(text, typeface);

                            textSize.Width /= (float)StiScale.Step;
                            textSize.Height /= (float)StiScale.Step;

                            #region Calc position

                            float x = 0;
                            float y = 0;

                            switch (GetPathHorAlignment(path))
                            {
                                case StiTextHorAlignment.Left:
                                case StiTextHorAlignment.Width:
                                    x = bounds.X;
                                    break;

                                case StiTextHorAlignment.Right:
                                    x = bounds.Right - textSize.Width;
                                    break;

                                case StiTextHorAlignment.Center:
                                    x = bounds.X + (bounds.Width - textSize.Width) / 2;
                                    break;
                            }

                            switch (GetPathVertAlignment(path))
                            {
                                case StiVertAlignment.Top:
                                    y = bounds.Y;
                                    break;

                                case StiVertAlignment.Bottom:
                                    y = bounds.Bottom - textSize.Height;
                                    break;

                                case StiVertAlignment.Center:
                                    y = bounds.Y + (bounds.Height - textSize.Height) / 2;
                                    break;
                            }
                            #endregion

                            using (var graphicsPath = new GraphicsPath())
                            using (var sf = new StringFormat())
                            {
                                sf.Alignment = StringAlignment.Center;
                                sf.LineAlignment = StringAlignment.Center;

                                if (map.DisplayNameType == StiDisplayNameType.Full)
                                {
                                    var width = path.SetMaxWidth ? textSize.Width + 20 : textSize.Width + 10;
                                    var rectText = new RectangleF(x, y, width, textSize.Height);

                                    graphicsPath.AddString(text, typeface.FontFamily, (int)typeface.Style, typeface.Size, rectText, sf);
                                }
                                else
                                {
                                    var rectText = new RectangleF(x, y, textSize.Width, textSize.Height);

                                    graphicsPath.AddString(text, typeface.FontFamily, (int)typeface.Style, typeface.Size, rectText, sf);
                                }

                                using (var pen = new Pen((SolidBrush)StiAnimationEngine.GetAnimationOpacity(this, foregroundLight, animation), 4 * scaceSize))
                                {
                                    pen.LineJoin = LineJoin.Round;
                                    g.DrawPath(pen, graphicsPath);
                                }

                                g.FillPath((SolidBrush)StiAnimationEngine.GetAnimationOpacity(this, foregroundDark, animation), graphicsPath);
                            }
                        }
                    }

                    g.SmoothingMode = smoothingMode;
                    g.TextRenderingHint = textRenderingHint;
                    g.InterpolationMode = interpolationMode;

                    typeface.Dispose();
                    foregroundDark.Dispose();
                    foregroundLight.Dispose();
                }
            }
        }

        private string GetPathText(StiMapSvg path, string key)
        {
            string text = null;
            var info = this.MapData.FirstOrDefault(x => x.Key == key);
            switch (map.DisplayNameType)
            {
                case StiDisplayNameType.Full:
                    {
                        text = (info != null)
                            ? info.Name
                            : path.EnglishName;
                    }
                    break;

                case StiDisplayNameType.Short:
                    {
                        text = StiMapHelper.PrepareIsoCode(path.ISOCode);
                    }
                    break;
            }

            if (map.ShowValue)
            {
                if (info != null && info.Value != null)
                {
                    string valueStr = null;
                    if (map.ShortValue)
                    {
                        if (double.TryParse(info.Value, out var resValue))
                        {
                            if (resValue == 0d && !this.map.ShowZeros)
                                return text;

                            valueStr = StiAbbreviationNumberFormatHelper.Format(resValue);
                        }
                    }

                    if (valueStr == null)
                    {
                        if (double.TryParse(info.Value, out var resValue) && resValue == 0d && !this.map.ShowZeros)
                            return text;

                        valueStr = info.Value;
                    }

                    if (text == null)
                    {
                        text = valueStr;
                    }
                    else
                    {
                        text += Environment.NewLine;
                        text += valueStr;
                    }
                }
            }

            return text;
        }

        private Rectangle GetPathRect(StiMapSvg path)
        {
            Rectangle rect;
            if (map.DisplayNameType == StiDisplayNameType.Short)
                rect = path.RectIso.IsEmpty ? path.Rect : path.RectIso;
            else
                rect = path.Rect;

            return rect;
        }

        private StiTextHorAlignment GetPathHorAlignment(StiMapSvg path)
        {
            if (map.DisplayNameType == StiDisplayNameType.Short)
                return path.HorAlignmentIso != null ? path.HorAlignmentIso.GetValueOrDefault() : path.HorAlignment;

            return path.HorAlignment;
        }

        private StiVertAlignment GetPathVertAlignment(StiMapSvg path)
        {
            if (map.DisplayNameType == StiDisplayNameType.Short)
                return path.VertAlignmentIso != null ? path.VertAlignmentIso.GetValueOrDefault() : path.VertAlignment;

            return path.VertAlignment;
        }
        #endregion

        #region Methods
        private void FillGroupColors()
        {
            colorsContainer.Init(this.map, this);

            int index = 0;
            foreach (string key in hashGroup.Keys.OrderBy(x => x))
            {
                var value = hashGroup[key];
                value.Fill1 = colorsContainer.GetColor1(index, hashGroup.Count);
                value.Fill = new SolidBrush(value.Fill1.Color);
                index++;
            }
        }

        public void UpdateHeatmapWithGroup()
        {
            heatmapInfo = new HeatmapInfo(this, map, MapData);
            noneInfo = new NoneInfo();

            if (map.MapType == StiMapType.HeatmapWithGroup)
                this.heatmapWithGroupInfo = new HeatmapWithGroupInfo(this, map, MapData);
        }

        public void UpdateGroupedData()
        {
            if (this.MapData == null) return;

            foreach (var data in this.MapData)
            {
                if (string.IsNullOrEmpty(data.Group)) continue;

                double? value = null;
                if (data.Value != null && double.TryParse(data.Value, out var result))
                    value = result;

                StiMapGroup group;
                if (!hashGroup.ContainsKey(data.Group))
                {
                    group = new StiMapGroup
                    {
                        MinValue = value,
                        MaxValue = value
                    };

                    hashGroup.Add(data.Group, group);
                }
                else
                {
                    group = hashGroup[data.Group];

                    if (value != null)
                    {
                        if (group.MinValue == null || group.MaxValue == null)
                        {
                            group.MinValue = 0d;
                            group.MaxValue = 0d;
                        }

                        if (group.MinValue > value)
                            group.MinValue = value;
                        else if (group.MaxValue < value)
                            group.MaxValue = value;
                    }
                }
            }

            FillGroupColors();
        }

        private Brush ParseHexColor(string color)
        {
            try
            {
                if (!string.IsNullOrEmpty(color))
                {
                    if (color.StartsWithInvariant("#"))
                        return new SolidBrush(ColorTranslator.FromHtml(color));
                    else
                        return new SolidBrush(Color.FromName(color));
                }
            }
            catch { }

            return null;
        }

        private StiBrush ParseHexColor1(string color)
        {
            try
            {
                if (!string.IsNullOrEmpty(color) && color.StartsWithInvariant("#"))
                    return new StiSolidBrush(ColorTranslator.FromHtml(color));
            }
            catch { }

            return null;
        }
        #endregion

        public StiGdiMapContextPainter(StiMap map)
        {
            this.map = map;
        }
    }
}