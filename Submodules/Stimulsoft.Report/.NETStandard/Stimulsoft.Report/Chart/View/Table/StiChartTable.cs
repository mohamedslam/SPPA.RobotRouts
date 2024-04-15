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
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Drawing;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiChartTable : 
        IStiChartTable, 
        IStiSerializeToCodeAsClass,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();
            
            jObject.AddPropertyBool("Visible", Visible);
            jObject.AddPropertyBool("AllowApplyStyle", AllowApplyStyle, true);
            jObject.AddPropertyBool("MarkerVisible", MarkerVisible, true);
            jObject.AddPropertyColor("GridLineColor", GridLineColor, Color.Gray);            
            jObject.AddPropertyBool("GridLinesHor", GridLinesHor, true);
            jObject.AddPropertyBool("GridLinesVert", GridLinesVert, true);
            jObject.AddPropertyBool("GridOutline", GridOutline, true);
            jObject.AddPropertyStringNullOrEmpty("Format", Format);
            jObject.AddPropertyJObject("Header", Header.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("DataCells", DataCells.SaveToJsonObject(mode));

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Font":
                        this.DataCells.Font = property.DeserializeFont(DataCells.Font);
                        break;

                    case "Visible":
                        this.Visible = property.DeserializeBool();
                        break;

                    case "AllowApplyStyle":
                        this.AllowApplyStyle = property.DeserializeBool();
                        break;

                    case "MarkerVisible":
                        this.MarkerVisible = property.DeserializeBool();
                        break;

                    case "GridLineColor":
                        this.GridLineColor = property.DeserializeColor();
                        break;

                    case "TextColor":
                        this.DataCells.TextColor = property.DeserializeColor();
                        break;

                    case "GridLinesHor":
                        this.GridLinesHor = property.DeserializeBool();
                        break;

                    case "GridLinesVert":
                        this.GridLinesVert = property.DeserializeBool();
                        break;

                    case "GridOutline":
                        this.GridOutline = property.DeserializeBool();
                        break;

                    case "Format":
                        this.Format = property.DeserializeString();
                        break;

                    case "Header":
                        this.Header.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "DataCells":
                        this.DataCells.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiChartTable;

        [Browsable(false)]
        public string PropName => string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            var list = new[] 
            { 
                propHelper.ChartTable()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var table = this.MemberwiseClone() as IStiChartTable;
            
            table.Header = this.Header.Clone() as IStiChartTableHeader;
            table.DataCells = this.DataCells.Clone() as IStiChartTableDataCells;
            
            if (this.Core != null)
            {
                table.Core = this.Core.Clone() as StiChartTableCore;
                table.Core.ChartTable = table;
            }

            return table;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                //We especially don't check GridLineColor properties because 
                //its default state controls by AllowApplySyle property
                return
                    AllowApplyStyle
                    && MarkerVisible
                    && GridLinesHor
                    && GridLinesVert
                    && GridOutline
                    && (Format != null && Format.Length == 0)
                    && !ShouldSerializeDataCells()
                    && !ShouldSerializeHeader()
                    && !Visible;
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        [StiNonSerialized]
        [Obsolete("Font property is obsolete. Please use DataCells.Font property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Font Font
        {
            get
            {
                return DataCells.Font;
            }
            set
            {
                DataCells.Font = value;
            }
        }        

        /// <summary>
        /// Gets or sets visibility of table.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of table.")]
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
        [DefaultValue(true)]
        public bool AllowApplyStyle { get; set; } = true;

        /// <summary>
        /// Gets or sets visibility of markers.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of markers.")]
        public bool MarkerVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets grid lines color.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets grid lines color.")]
        public Color GridLineColor { get; set; } = Color.Gray;

        private bool ShouldSerializeGridLineColor()
        {
            return GridLineColor != Color.Gray;
        }

        [Browsable(false)]
        [StiNonSerialized]
        [Obsolete("TextColor property is obsolete. Please use DataCells.TextColor property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color TextColor
        {
            get
            {
                return DataCells.TextColor;
            }
            set
            {
                DataCells.TextColor = value;
            }
        }

        /// <summary>
        /// Gets or sets visibility of grid lines horizontal.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of grid lines horizontal.")]
        public bool GridLinesHor { get; set; } = true;

        /// <summary>
        /// Gets or sets visibility of grid lines vertical.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of grid lines vertical.")]
        public bool GridLinesVert { get; set; } = true;

        /// <summary>
        /// Gets or sets visibility of grid outline.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of grid outline.")]
        public bool GridOutline { get; set; } = true;

        /// <summary>
        /// Gets or sets format string which used for formating of the chart table.
        /// </summary>
        [DefaultValue("")]
        [Description("Gets or sets format string which used for formating of the chart table.")]
        [StiSerializable]
        [Editor("Stimulsoft.Report.Chart.Design.StiFormatEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public string Format { get; set; } = "";

        /// <summary>
        /// Gets or sets header settings.
        /// </summary>
        [Description("Gets or sets header settings.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiChartTableHeader Header { get; set; }

        private bool ShouldSerializeHeader()
        {
            return Header == null || !Header.IsDefault;
        }

        /// <summary>
        /// Gets or sets Data Cells settings.
        /// </summary>
        [Description("Gets or sets data cells settings.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiChartTableDataCells DataCells { get; set; }

        private bool ShouldSerializeDataCells()
        {
            return DataCells == null || !(AllowApplyStyle ? DataCells.IsDefaultExceptTextColor : DataCells.IsDefault);
        }

        [Browsable(false)]
        public StiChartTableCore Core { get; set; }

        [Browsable(false)]
        public IStiChart Chart { get; set; } = null;
        #endregion

        [StiUniversalConstructor("Table")]
        public StiChartTable()
        {
            Header = new StiChartTableHeader();
            DataCells = new StiChartTableDataCells();

            Core = new StiChartTableCore(this);
        }
    }
}
