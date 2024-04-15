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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Maps.Helpers;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Maps
{
    [StiToolbox(true)]
    [StiServiceBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiMap.png")]
    [StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.catInfographics.png")]
    [StiGdiPainter(typeof(StiMapGdiPainter))]
    [StiDesigner("Stimulsoft.Report.Map.Design.StiMapDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiMapWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfMapDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiV2Builder(typeof(StiMapV2Builder))]
    [StiV1Builder(typeof(StiMapV1Builder))]
    [StiV2Builder(typeof(StiMapV2Builder))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiContextTool(typeof(IStiDataSource))]
    public class StiMap : 
        StiComponent,
        IStiExportImageExtended,
        IStiBorder
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // NonSerialized
            jObject.RemoveProperty(nameof(UseParentStyles));
            jObject.RemoveProperty(nameof(CanGrow));
            jObject.RemoveProperty(nameof(CanShrink));
            jObject.RemoveProperty(nameof(GrowToHeight));

            // StiGauge            
            jObject.AddPropertyBorder(nameof(Border), Border);
            jObject.AddPropertyBrush(nameof(Brush), Brush);

            jObject.AddPropertyEnum(nameof(MapStyle), MapStyle, StiMapStyleIdent.Style25);
            jObject.AddPropertyEnum(nameof(DataFrom), DataFrom, StiMapSource.Manual);

            jObject.AddPropertyBool(nameof(ColorEach), ColorEach);
            jObject.AddPropertyBool(nameof(Stretch), Stretch, true);
            jObject.AddPropertyBool(nameof(ShowValue), ShowValue, true);
            jObject.AddPropertyBool(nameof(ShortValue), ShortValue, true);
            jObject.AddPropertyBool(nameof(ShowZeros), ShowZeros, true);
            jObject.AddPropertyEnum(nameof(DisplayNameType), DisplayNameType, StiDisplayNameType.Full);
            jObject.AddPropertyStringNullOrEmpty(nameof(MapIdent), MapIdent);
            jObject.AddPropertyStringNullOrEmpty(nameof(Language), Language);
            jObject.AddPropertyEnum(nameof(MapMode), MapMode, StiMapMode.Choropleth);
            jObject.AddPropertyEnum(nameof(MapType), MapType, StiMapType.Individual);
            jObject.AddPropertyStringNullOrEmpty(nameof(MapData), MapData);
            jObject.AddPropertyStringNullOrEmpty(nameof(KeyDataColumn), KeyDataColumn);
            jObject.AddPropertyStringNullOrEmpty(nameof(NameDataColumn), NameDataColumn);
            jObject.AddPropertyStringNullOrEmpty(nameof(ValueDataColumn), ValueDataColumn);
            jObject.AddPropertyStringNullOrEmpty(nameof(GroupDataColumn), GroupDataColumn);
            jObject.AddPropertyStringNullOrEmpty(nameof(ColorDataColumn), ColorDataColumn);
            jObject.AddPropertyStringNullOrEmpty(nameof(Latitude), Latitude);
            jObject.AddPropertyStringNullOrEmpty(nameof(Longitude), Longitude);

            if (mode == StiJsonSaveMode.Document)
            {
                jObject.AddPropertyStringNullOrEmpty(nameof(MapImage), MapImage);
                jObject.AddPropertyStringNullOrEmpty(nameof(PushPins), PushPins);
            }

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Border):
                        this.Border = property.DeserializeBorder();
                        break;

                    case nameof(Brush):
                        this.Brush = property.DeserializeBrush();
                        break;

                    case nameof(MapStyle):
                        this.MapStyle = property.DeserializeEnum<StiMapStyleIdent>();
                        break;

                    case nameof(DataFrom):
                        this.DataFrom = property.DeserializeEnum<StiMapSource>();
                        break;

                    case nameof(Stretch):
                        this.Stretch = property.DeserializeBool();
                        break;

                    case nameof(ShowValue):
                        this.ShowValue = property.DeserializeBool();
                        break;

                    case nameof(ColorEach):
                        this.ColorEach = property.DeserializeBool();
                        break;

                    case nameof(ShortValue):
                        this.ShortValue = property.DeserializeBool();
                        break;

                    case nameof(ShowZeros):
                        this.ShowZeros = property.DeserializeBool();
                        break;

                    case nameof(DisplayNameType):
                        this.DisplayNameType = property.DeserializeEnum<StiDisplayNameType>();
                        break;

                    case "MapID":
                    case nameof(MapIdent):
                        this.MapIdent = property.DeserializeString();
                        break;

                    case nameof(Language):
                        this.Language = property.DeserializeString();
                        break;

                    case nameof(MapMode):
                        this.MapMode = property.DeserializeEnum<StiMapMode>();
                        break;

                    case nameof(MapType):
                        this.MapType = property.DeserializeEnum<StiMapType>();
                        break;

                    case nameof(MapData):
                        this.mapData = property.DeserializeString();
                        break;

                    case "DataColumnKey":
                    case nameof(KeyDataColumn):
                        this.KeyDataColumn = property.DeserializeString();
                        break;

                    case "DataColumnName":
                    case nameof(NameDataColumn):
                        this.NameDataColumn = property.DeserializeString();
                        break;

                    case "DataColumnValue":
                    case nameof(ValueDataColumn):
                        this.ValueDataColumn = property.DeserializeString();
                        break;

                    case "DataColumnGroup":
                    case nameof(GroupDataColumn):
                        this.GroupDataColumn = property.DeserializeString();
                        break;

                    case "DataColumnColor":
                    case nameof(ColorDataColumn):
                        this.ColorDataColumn = property.DeserializeString();
                        break;

                    case nameof(Latitude):
                        this.Latitude = property.DeserializeString();
                        break;

                    case nameof(Longitude):
                        this.Longitude = property.DeserializeString();
                        break;

                    case nameof(MapImage):
                        this.MapImage = property.DeserializeString();
                        break;

                    case nameof(PushPins):
                        this.PushPins = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiMap;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();
            StiPropertyObject[] list;

            // DataCategory
            list = new[]
            {
                propHelper.MapEditor()
            };
            objHelper.Add(StiPropertyCategories.ComponentEditor, list);

            // MapCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.ShowValue(),
                    propHelper.Stretch(),
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.MapType(),
                    propHelper.ShowValue(),
                    propHelper.Stretch(),
                };
            }
            objHelper.Add(StiPropertyCategories.Map, list);

            // DataCategory
            list = new[]
            {
                propHelper.MapKeyDataColumn(),
                propHelper.MapNameDataColumn(),
                propHelper.MapValueDataColumn(),
                propHelper.MapGroupDataColumn(),
                propHelper.MapColorDataColumn(),
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            // PositionCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                };
            }
            objHelper.Add(StiPropertyCategories.Position, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Border(),
                propHelper.Conditions()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // BehaviorCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.Enabled()
                };
            }
            else if (level == StiLevel.Standard)
            {
                list = new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.Printable(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                };
            }
            objHelper.Add(StiPropertyCategories.Behavior, list);

            // DesignCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.Name()
                };
            }
            else if (level == StiLevel.Standard)
            {
                list = new[]
                {
                    propHelper.Name(),
                    propHelper.Alias()
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.Name(),
                    propHelper.Alias(),
                    propHelper.Restrictions(),
                    propHelper.Locked(),
                    propHelper.Linked()
                };
            }
            objHelper.Add(StiPropertyCategories.Design, list);

            return objHelper;
        }

        public override StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }

        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties)
        {
            var clone = (StiMap)base.Clone(cloneProperties);
            if (this.hashData != null)
            {
                clone.hashData = new List<StiMapData>();
                foreach (var data in this.hashData)
                {
                    clone.hashData.Add(data.Clone());
                }
            }

            return clone;
        }
        #endregion		

        #region IStiExportImageExtended
        public virtual Image GetImage(ref float zoom)
        {
            return GetImage(ref zoom, StiExportFormat.None);
        }

        public virtual Image GetImage(ref float zoom, StiExportFormat format)
        {
            var painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi);
            if (painter is StiMapGdiPainter)
                ((StiMapGdiPainter)painter).UseBackground = false;
            return painter.GetImage(this, ref zoom, format);
        }

        public override bool IsExportAsImage(StiExportFormat format)
        {
            return MapMode == StiMapMode.Online || format != StiExportFormat.Pdf;
        }
        #endregion

        #region IStiBorder
        /// <summary>
        /// The appearance and behavior of the component border.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiSerializable]
        [Description("The appearance and behavior of the component border.")]
        public StiBorder Border { get; set; } = new StiBorder();
        #endregion

        #region StiComponent override
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.Map;

        /// <summary>
        /// Gets a localized name of the component category.
        /// </summary>
        public override string LocalizedCategory => StiLocalization.Get("Components", "StiMap");

        public override string LocalizedName => StiLocalization.Get("Components", "StiMap");

        public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 240, 240);

        [Browsable(false)]
        public override string ComponentStyle
        {
            get
            {
                return base.ComponentStyle;
            }
            set
            {
                base.ComponentStyle = value;
            }
        }

        public override RectangleD ClientRectangle
        {
            get
            {
                return base.ClientRectangle;
            }
            set
            {
                base.ClientRectangle = value;
                StiMapDrawingCache.RemoveImage(this);
            }
        }
        #endregion

        #region Fields
        private bool isMapDataChanged;
        private List<StiMapData> hashData;
        #endregion

        #region Properties.Obsolete
        [Browsable(false)]
        [Obsolete("Please use property KeyDataColumn")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DataColumnKey
        {
            get
            {
                return null;
            }
            set
            {
                this.KeyDataColumn = value;
            }
        }

        [Browsable(false)]
        [Obsolete("Please use property NameDataColumn")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DataColumnName
        {
            get
            {
                return null;
            }
            set
            {
                this.NameDataColumn = value;
            }
        }

        [Browsable(false)]
        [Obsolete("Please use property ValueDataColumn")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DataColumnValue
        {
            get
            {
                return null;
            }
            set
            {
                this.ValueDataColumn = value;
            }
        }

        [Browsable(false)]
        [Obsolete("Please use property GroupDataColumn")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DataColumnGroup
        {
            get
            {
                return null;
            }
            set
            {
                this.GroupDataColumn = value;
            }
        }

        [Browsable(false)]
        [Obsolete("Please use property ColorDataColumn")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DataColumnColor
        {
            get
            {
                return null;
            }
            set
            {
                this.ColorDataColumn = value;
            }
        }
        #endregion

        #region Properties
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Map")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public bool ShortValue { get; set; } = true;

        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Map")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public bool ShowZeros { get; set; } = true;

        /// <summary>
        /// The brush, which is used to draw background.
        /// </summary>
        [Browsable(false)]
        [StiSerializable]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.Transparent);

        [StiSerializable]
        [DefaultValue(StiMapStyleIdent.Style25)]
        [StiCategory("Map")]
        [Browsable(false)]
        [TypeConverter(typeof(StiEnumConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiMapStyleIdent MapStyle { get; set; } = StiMapStyleIdent.Style25;

        [StiSerializable]
        [DefaultValue(StiMapSource.Manual)]
        [StiCategory("Map")]
        [Browsable(false)]
        [TypeConverter(typeof(StiEnumConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiMapSource DataFrom { get; set; } = StiMapSource.Manual;

        [StiSerializable]
        [DefaultValue(true)]
        [StiCategory("Map")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool Stretch { get; set; } = true;

        [StiSerializable]
        [DefaultValue(false)]
        [StiCategory("Map")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool ColorEach { get; set; }
        

        [StiSerializable]
        [DefaultValue(true)]
        [StiBrowsable(true)]
        [StiCategory("Map")]
        [Browsable(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool ShowValue { get; set; } = true;
                
        [Browsable(false)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Obsolete("MapID property is obsolete. Please use MapIdent property.")]
        public StiMapID MapID
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                MapIdent = value.ToString();
            }
        }

        [StiSerializable]        
        [Browsable(false)]
        [StiPropertyLevel(StiLevel.Basic)]
        public string MapIdent { get; set; } = "USA";

        private string language;
        [Browsable(false)]
        [StiSerializable]
        public string Language 
        { 
            get
            {
                return language;
            }
            set
            {
                language = value;
                this.hashData = null;
            }
        }

        [DefaultValue(false)]
        [Browsable(false)]
        public bool ShowLegend { get; set; }

        [StiSerializable]
        [DefaultValue(StiDisplayNameType.Full)]
        [StiCategory("Map")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiDisplayNameType DisplayNameType { get; set; } = StiDisplayNameType.Full;

        [StiSerializable]
        [Browsable(false)]
        [DefaultValue(StiMapType.Individual)]
        [TypeConverter(typeof(StiEnumConverter))]
        public StiMapType MapType 
        {
            get; 
            set; 
        } = StiMapType.Individual;

        private string mapData;
        [StiSerializable]
        [Browsable(false)]
        [StiPropertyLevel(StiLevel.Basic)]
        public string MapData
        {
            get
            {
                return mapData;
            }
            set
            {
                this.mapData = value;
                this.IsHashDataEmpty = true;
                this.isMapDataChanged = true;
                this.hashData = null;
            }
        }

        [StiSerializable]
        [Browsable(false)]
        [DefaultValue("")]
        public string KeyDataColumn { get; set; } = string.Empty;

        [StiSerializable]
        [Browsable(false)]
        [DefaultValue("")]
        public string NameDataColumn { get; set; } = string.Empty;

        [StiSerializable]
        [Browsable(false)]
        [DefaultValue("")]
        public string ValueDataColumn { get; set; } = string.Empty;

        [StiSerializable]
        [Browsable(false)]
        [DefaultValue("")]
        public string GroupDataColumn { get; set; } = string.Empty;

        [StiSerializable]
        [Browsable(false)]
        [DefaultValue("")]
        public string ColorDataColumn { get; set; } = string.Empty;

        [StiSerializable]
        [Browsable(false)]
        [DefaultValue("")]
        public string Latitude { get; set; } = string.Empty;

        [StiSerializable]
        [Browsable(false)]
        [DefaultValue("")]
        public string Longitude { get; set; } = string.Empty;

        [StiNonSerialized]
        [Browsable(false)]
        public bool IsHashDataEmpty { get; private set; } = true;

        [StiSerializable]
        [Browsable(false)]
        [DefaultValue(StiMapMode.Choropleth)]
        [TypeConverter(typeof(StiEnumConverter))]
        public StiMapMode MapMode { get; set; } = StiMapMode.Choropleth;

        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        [Browsable(false)]
        public string MapImage { get; set; }

        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        [Browsable(false)]
        public string PushPins { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        [Description("Internal use only.")]
        internal StiDataTable DataTable { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        [Description("Internal use only.")]
        internal bool ShowBubble { get; set; }
        #endregion

        #region Properties Browsable(false)
        [StiNonSerialized]
        [Browsable(false)]
        public override bool UseParentStyles
        {
            get
            {
                return base.UseParentStyles;
            }
            set
            {
                
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool CanGrow
        {
            get
            {
                return base.CanGrow;
            }
            set
            {
                
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool CanShrink
        {
            get
            {
                return base.CanShrink;
            }
            set
            {

            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool GrowToHeight
        {
            get
            {
                return base.GrowToHeight;
            }
            set
            {

            }
        }
        #endregion

        #region Methods override
        public override StiComponent CreateNew()
        {
            return new StiMap();
        }
        #endregion

        #region Methods
        public static List<StiMapData> GetMapDataInternal(StiReport report, string mapData, string mapID, string lang)
        {
            return GetMapDataInternal(report, mapData, mapID, lang, out var res);
        }

        public static List<StiMapData> GetDefaultMapData(StiReport report, string mapID, string lang)
        {
            var result = new List<StiMapData>();

            byte offset = 10;
            bool isUp = true;
            int index = 0;
            var baseColors = StiMapHelper.GetColors();
            var colors = new List<Color>();

            foreach (var color in baseColors) colors.Add(color);

            var svgContainer = StiMapLoader.LoadResource(report, mapID, lang);
            if (svgContainer == null) return null;

            foreach (string key in svgContainer.HashPaths.Keys)
            {
                var data = new StiMapData(key)
                {
                    Color = ColorTranslator.ToHtml(colors[index])
                };

                var pt = svgContainer.HashPaths[key];
                if (pt != null)
                {
                    data.Name = pt.EnglishName;
                }

                result.Add(data);

                index++;
                if (index >= colors.Count)
                {
                    index = 0;
                    if (isUp)
                    {
                        foreach (var color in baseColors)
                            colors.Add(StiColorUtils.Dark(color, offset));
                    }
                    else
                    {
                        foreach (var color in baseColors)
                            colors.Add(StiColorUtils.Light(color, offset));

                        offset += 10;
                    }

                    isUp = !isUp;
                }
            }

            return result;
        }

        internal static List<StiMapData> GetMapDataInternal(StiReport report, string mapData, string mapID, string lang, out bool isHashDataEmpty)
        {
            isHashDataEmpty = true;
            var result = new List<StiMapData>();

            if (!string.IsNullOrEmpty(mapData))
                StiJsonHelper.LoadFromJsonString(mapData, result);

            var svgContainer = StiMapLoader.LoadResource(report, mapID, lang);
            if (svgContainer != null)
            {
                if (result.Count > 0)
                {
                    #region Сначало проверяем, чтобы в данных, небыло лишних данных
                    int index = 0;
                    while (index < result.Count)
                    {
                        var data = result[index];
                        if (string.IsNullOrEmpty(data.Key) || !svgContainer.HashPaths.ContainsKey(data.Key))
                        {
                            result.RemoveAt(index);
                            continue;
                        }

                        index++;
                    }
                    #endregion

                    #region Проверяем возможно какихто данных не хватает - добавляем их
                    if (svgContainer.HashPaths.Count != result.Count)
                    {
                        foreach (string key in svgContainer.HashPaths.Keys)
                        {
                            if (result.FirstOrDefault(x => x.Key == key) == null)
                            {
                                var data = new StiMapData(key);
                                result.Add(data);
                            }
                        }
                    }
                    #endregion

                    #region Loc Names
                    foreach (var res in result)
                    {
                        if (string.IsNullOrEmpty(res.Name) && svgContainer.HashPaths.TryGetValue(res.Key, out var value))
                        {
                            res.Name = value.EnglishName;
                        }
                    }
                    #endregion

                    #region Проверяем, являются ли данные пустыми
                    // Если хотябы одно значение, влияющее на отрисовку изменено - то фиксируем это
                    foreach (var data in result)
                    {
                        if (!string.IsNullOrEmpty(data.Color) ||
                            !string.IsNullOrEmpty(data.Group) ||
                            !string.IsNullOrEmpty(data.Value))
                        {
                            isHashDataEmpty = false;
                            break;
                        }
                    }
                    #endregion
                }
                else
                {
                    byte offset = 10;
                    bool isUp = true;
                    int index = 0;
                    var baseColors = StiMapHelper.GetColors();
                    var colors = new List<Color>();

                    colors.AddRange(baseColors);

                    foreach (string key in svgContainer.HashPaths.Keys)
                    {
                        var data = new StiMapData(key)
                        {
                            Color = ColorTranslator.ToHtml(colors[index])
                        };

                        var pt = svgContainer.HashPaths[key];
                        if (pt != null)
                        {
                            data.Name = pt.EnglishName;
                        }

                        result.Add(data);

                        index++;
                        if (index >= colors.Count)
                        {
                            index = 0;
                            if (isUp)
                            {
                                foreach (var color in baseColors)
                                    colors.Add(StiColorUtils.Dark(color, offset));
                            }
                            else
                            {
                                foreach (var color in baseColors)
                                    colors.Add(StiColorUtils.Light(color, offset));

                                offset += 10;
                            }

                            isUp = !isUp;
                        }
                    }
                }
            }

            return result;
        }

        public List<StiMapData> GetMapData()
        {
            if (this.hashData != null && this.hashData.Count > 0 && this.isMapDataChanged)
                return hashData;

            this.IsHashDataEmpty = true;

            bool isHashDataEmpty;
            var result = GetMapDataInternal(this.Report, this.mapData, this.MapIdent, this.Language, out isHashDataEmpty);
            if (!isHashDataEmpty)
                this.IsHashDataEmpty = false;

            this.isMapDataChanged = true;
            hashData = result;
            return result;
        }

        public Color[] GetCurrentStyleColors()
        {
            if (!string.IsNullOrEmpty(this.ComponentStyle))
            {
                var style = this.Report.Styles[this.ComponentStyle] as StiMapStyle;
                if (style != null)
                    return style.Colors;
            }

            return GetStyleColors(this.MapStyle);
        }

        public static Color[] GetStyleColors(StiMapStyleIdent style)
        {
            return GetMapStyle(style).Colors;
        }

        internal StiBrush GetStyleBackground()
        {
            if (!string.IsNullOrEmpty(this.ComponentStyle))
            {
                var style = this.Report.Styles[this.ComponentStyle] as StiMapStyle;
                if (style != null)
                    return new StiSolidBrush(style.BackColor);
            }

            return new StiSolidBrush(GetMapStyle(this.MapStyle).BackColor);
        }

        public static StiMapStyle GetMapStyle(StiMap map)
        {
            StiMapStyle mapStyle = null;
            if (!string.IsNullOrEmpty(map.ComponentStyle))
                mapStyle = map.Report.Styles[map.ComponentStyle] as StiMapStyle;

            if (mapStyle == null)
                mapStyle = GetMapStyle(map.MapStyle);

            return mapStyle;
        }

        public static StiMapStyle GetMapStyle(StiMapStyleIdent style)
        {
            switch (style)
            {
                case StiMapStyleIdent.Style21:
                    return new StiMap21StyleFX();

                case StiMapStyleIdent.Style24:
                    return new StiMap24StyleFX();

                case StiMapStyleIdent.Style25:
                    return new StiMap25StyleFX();

                case StiMapStyleIdent.Style26:
                    return new StiMap26StyleFX();

                case StiMapStyleIdent.Style27:
                    return new StiMap27StyleFX();

                case StiMapStyleIdent.Style28:
                    return new StiMap28StyleFX();

                case StiMapStyleIdent.Style29:
                    return new StiMap29StyleFX();

                case StiMapStyleIdent.Style30:
                    return new StiMap30StyleFX();

                case StiMapStyleIdent.Style31:
                    return new StiMap31StyleFX();

                case StiMapStyleIdent.Style32:
                    return new StiMap32StyleFX();

                case StiMapStyleIdent.Style33:
                    return new StiMap33StyleFX();

                case StiMapStyleIdent.Style34:
                    return new StiMap34StyleFX();

                case StiMapStyleIdent.Style35:
                    return new StiMap35StyleFX();

                default:
                    throw new Exception("Style is not supported!");
            }
        }
        #endregion

        /// <summary>
        /// Creates a new StiGauge.
        /// </summary>
        public StiMap() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiGauge.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiMap(RectangleD rect) : base(rect)
		{
            PlaceOnToolbox = true;
		}
    }
}