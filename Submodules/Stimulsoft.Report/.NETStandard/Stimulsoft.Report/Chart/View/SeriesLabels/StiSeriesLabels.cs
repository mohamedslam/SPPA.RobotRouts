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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Report.Components.TextFormats;
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
    [StiServiceBitmap(typeof(StiSeriesLabels), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [StiServiceCategoryBitmap(typeof(StiSeriesLabels), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [TypeConverter(typeof(Design.StiSeriesLabelsConverter))]
    public abstract class StiSeriesLabels :
        StiService,
        IStiSerializeToCodeAsClass,
        IStiSeriesLabels,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyJObject("Conditions", Conditions.SaveToJsonObject(mode));
            jObject.AddPropertyBool("PreventIntersection", PreventIntersection);
            jObject.AddPropertyBool("AllowApplyStyle", AllowApplyStyle, true);
            jObject.AddPropertyBool("ShowZeros", ShowZeros);
            jObject.AddPropertyBool("ShowNulls", ShowNulls, true);
            jObject.AddPropertyBool("MarkerVisible", MarkerVisible);
            jObject.AddPropertySize("MarkerSize", MarkerSize);
            jObject.AddPropertyEnum("MarkerAlignment", MarkerAlignment, StiMarkerAlignment.Left);
            jObject.AddPropertyInt("Step", Step);
            jObject.AddPropertyEnum("ValueType", ValueType, StiSeriesLabelsValueType.Value);
            jObject.AddPropertyStringNullOrEmpty("ValueTypeSeparator", ValueTypeSeparator);
            jObject.AddPropertyEnum("LegendValueType", LegendValueType, StiSeriesLabelsValueType.Value);
            jObject.AddPropertyStringNullOrEmpty("TextBefore", TextBefore);
            jObject.AddPropertyStringNullOrEmpty("TextAfter", TextAfter);
            jObject.AddPropertyFloat("Angle", Angle);
            jObject.AddPropertyStringNullOrEmpty("Format", Format);
            jObject.AddPropertyBool("Antialiasing", Antialiasing, true);
            jObject.AddPropertyBool("Visible", Visible, true);
            jObject.AddPropertyBool("DrawBorder", DrawBorder, true);
            jObject.AddPropertyBool("UseSeriesColor", UseSeriesColor);
            jObject.AddPropertyColor("LabelColor", LabelColor, Color.Black);
            jObject.AddPropertyColor("BorderColor", BorderColor, Color.Black);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyFontArial7("Font", Font);
            jObject.AddPropertyBool("WordWrap", WordWrap);
            jObject.AddPropertyInt("Width", Width);

            if (FormatService != null && !(FormatService is StiGeneralFormatService))
                jObject.AddPropertyJObject("FormatService", FormatService.SaveToJsonObject(mode));

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Conditions":
                        this.Conditions.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "PreventIntersection":
                        this.PreventIntersection = property.DeserializeBool();
                        break;

                    case "AllowApplyStyle":
                        this.AllowApplyStyle = property.DeserializeBool();
                        break;
                        
                    case "ShowZeros":
                        this.ShowZeros = property.DeserializeBool();
                        break;

                    case "ShowNulls":
                        this.ShowNulls = property.DeserializeBool();
                        break;

                    case "MarkerVisible":
                        this.MarkerVisible = property.DeserializeBool();
                        break;

                    case "MarkerSize":
                        this.MarkerSize = property.DeserializeSize();
                        break;

                    case "MarkerAlignment":
                        this.MarkerAlignment = property.DeserializeEnum<StiMarkerAlignment>();
                        break;

                    case "Step":
                        this.Step = property.DeserializeInt();
                        break;

                    case "ValueType":
                        this.ValueType = property.DeserializeEnum<StiSeriesLabelsValueType>(); 
                        break;

                    case "ValueTypeSeparator":
                        this.ValueTypeSeparator = property.DeserializeString();
                        break;

                    case "LegendValueType":
                        this.LegendValueType = property.DeserializeEnum<StiSeriesLabelsValueType>(); 
                        break;

                    case "TextBefore":
                        this.TextBefore = property.DeserializeString();
                        break;

                    case "TextAfter":
                        this.TextAfter = property.DeserializeString();
                        break;

                    case "Angle":
                        this.Angle = property.DeserializeFloat();
                        break;

                    case "Format":
                        this.Format = property.DeserializeString();
                        break;

                    case "Antialiasing":
                        this.Antialiasing = property.DeserializeBool();
                        break;

                    case "Visible":
                        this.Visible = property.DeserializeBool();
                        break;

                    case "DrawBorder":
                        this.DrawBorder = property.DeserializeBool();
                        break;

                    case "UseSeriesColor":
                        this.UseSeriesColor = property.DeserializeBool();
                        break;

                    case "LabelColor":
                        this.LabelColor = property.DeserializeColor();
                        break;

                    case "BorderColor":
                        this.BorderColor = property.DeserializeColor();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "Font":
                        this.Font = property.DeserializeFont(Font);
                        break;

                    case "WordWrap":
                        this.WordWrap = property.DeserializeBool();
                        break;

                    case "Width":
                        this.Width = property.DeserializeInt();
                        break;

                    case "FormatService":
                        this.FormatService = StiFormatService.CreateFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }

        internal static IStiSeriesLabels LoadFromJsonObjectInternal(JObject jObject, StiChart chart)
        {
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();
            var service = StiOptions.Services.ChartSerieLabels.FirstOrDefault(x => x.GetType().Name == ident);

            if (service == null)
                throw new Exception($"Type {ident} is not found!");

            var seriesLabels = service.CreateNew();

            seriesLabels.Chart = chart;
            seriesLabels.LoadFromJsonObject(jObject);

            return seriesLabels;
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public virtual StiComponentId ComponentId => StiComponentId.StiSeriesLabels;

        [Browsable(false)]
        public string PropName => string.Empty;

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            return null;
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
        public override object Clone()
        {
            var labels = base.Clone() as IStiSeriesLabels;

            labels.Brush = this.Brush.Clone() as StiBrush;
            labels.Font = this.Font.Clone() as Font;

            if (this.Core != null)
            {
                labels.Core = this.Core.Clone() as StiSeriesLabelsCoreXF;
                labels.Core.SeriesLabels = labels;
            }

            return labels;
        }
        #endregion

        #region StiService override
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => Core.LocalizedName;

        /// <summary>
        /// Gets a service category.
        /// </summary>
        [Browsable(false)]
        public sealed override string ServiceCategory => "Chart";

        /// <summary>
        /// Gets a service type.
        /// </summary>
        [Browsable(false)]
        public sealed override Type ServiceType => typeof(StiSeriesLabels);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value which indicates that whether it is necessary to avoid intersections between border of series labels and border of series.
        /// </summary>
        [StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.PreventIntersection)]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that whether it is necessary to avoid intersections between border of series labels and border of series.")]
        public bool PreventIntersection { get; set; }

        [Browsable(false)]
        public StiSeriesLabelsCoreXF Core { get; set; }

        [Browsable(false)]
        public StiAxisSeriesLabelsCoreXF AxisCore => Core as StiAxisSeriesLabelsCoreXF;

        [Browsable(false)]
        public StiPieSeriesLabelsCoreXF PieCore => Core as StiPieSeriesLabelsCoreXF;

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.AllowApplyStyle)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
        [DefaultValue(true)]
        public bool AllowApplyStyle
        {
            get
            {
                return allowApplyStyle;
            }
            set
            {
                if (allowApplyStyle != value)
                {
                    allowApplyStyle = value;
                    if (value && Chart != null)
                        this.Core.ApplyStyle(this.Chart.Style);
                }
            }
        }

        /// <summary>
        /// Gets or sets collection of conditions which can be used to change behavior of series labels.
        /// </summary>
        [StiOrder(StiSeriesLabelsPropertyOrder.Conditions)]
        [TypeConverter(typeof(Design.StiChartConditionsCollectionConverter))]
        [Description("Gets or sets collection of conditions which can be used to change behavior of series labels.")]
        [Editor("Stimulsoft.Report.Chart.Design.StiChartConditionsEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public StiChartConditionsCollection Conditions
        {
            get
            {
                if (Chart == null)
                    return null;

                return Chart.SeriesLabelsConditions;
            }
            set
            {
                if (Chart == null)return;

                Chart.SeriesLabelsConditions = value;
            }
        }

        private bool ShouldSerializeConditions()
        {
            return Conditions == null || Conditions.Count > 0;
        }

        [Browsable(false)]
        [Obsolete("ShowOnZeroValues property is obsolete. Please use ShowZeros property instead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowOnZeroValues
        {
            get
            {
                return ShowZeros;
            }
            set
            {
                ShowZeros = value;
            }
        }

        /// <summary>
        /// Gets or sets value which indicates that series labels will be shown or not if value equal to zero.
        /// </summary>
        [DefaultValue(false)]
        [StiOrder(StiSeriesLabelsPropertyOrder.ShowZeros)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that series labels will be shown or not if value equal to zero.")]
        public bool ShowZeros { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that series labels will be shown or not if value equal to null.
        /// </summary>
        [DefaultValue(true)]
        [StiOrder(StiSeriesLabelsPropertyOrder.ShowNulls)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that series labels will be shown or not if value equal to null.")]
        public bool ShowNulls { get; set; } = true;

        /// <summary>
        /// Gets or sets vibility of marker.
        /// </summary>
		[DefaultValue(false)]
        [StiOrder(StiSeriesLabelsPropertyOrder.MarkerVisible)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets vibility of marker.")]
        public bool MarkerVisible { get; set; }

        /// <summary>
        /// Gets or sets marker size.
        /// </summary>
		[StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.MarkerSize)]
        [TypeConverter(typeof(StiSizeConverter))]
        [Description("Gets or sets marker size.")]
        public Size MarkerSize { get; set; } = new Size(8, 6);

        private bool ShouldSerializeMarkerSize()
        {
            return MarkerSize.Width != 8 || MarkerSize.Height != 6;
        }

        /// <summary>
        /// Gets or sets marker alignment related to label text.
        /// </summary>
		[StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.MarkerAlignment)]
        [DefaultValue(StiMarkerAlignment.Left)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets marker alignment related to label text.")]
        public virtual StiMarkerAlignment MarkerAlignment { get; set; } = StiMarkerAlignment.Left;

        /// <summary>
        /// Gets or sets value which indicates with what steps do labels be shown.
        /// </summary>
        [DefaultValue(0)]
        [StiOrder(StiSeriesLabelsPropertyOrder.Step)]
        [StiSerializable]
        [Description("Gets or sets value which indicates with what steps do labels be shown.")]
        public int Step { get; set; }

        /// <summary>
        /// Gets or sets which type of information will be shown in series labels.
        /// </summary>
        [StiOrder(StiSeriesLabelsPropertyOrder.ValueType)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [DefaultValue(StiSeriesLabelsValueType.Value)]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets which type of information will be shown in series labels.")]
        public virtual StiSeriesLabelsValueType ValueType { get; set; } = StiSeriesLabelsValueType.Value;

        /// <summary>
        /// Gets or sets string which contain separator for value information (if applicated).
        /// </summary>
        [StiOrder(StiSeriesLabelsPropertyOrder.ValueTypeSeparator)]
        [StiSerializable]
        [DefaultValue("")]
        [Description("Gets or sets string which contain separator for value information (if applicated).")]
        public virtual string ValueTypeSeparator { get; set; } = "-";

        /// <summary>
        /// Gets or sets which type of information will be shown in legend.
        /// </summary>
        [StiOrder(StiSeriesLabelsPropertyOrder.LegendValueType)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [DefaultValue(StiSeriesLabelsValueType.Value)]
        [Description("Gets or sets which type of information will be shown in legend.")]
        public virtual StiSeriesLabelsValueType LegendValueType { get; set; } = StiSeriesLabelsValueType.Value;

        /// <summary>
        /// Gets or sets text which will be shown before label text.
        /// </summary>
		[DefaultValue("")]
        [StiOrder(StiSeriesLabelsPropertyOrder.TextBefore)]
        [StiSerializable]
        [Description("Gets or sets text which will be shown before label text.")]
        public string TextBefore { get; set; } = "";

        /// <summary>
        /// Gets or sets text which will be shown after label text.
        /// </summary>
		[DefaultValue("")]
        [StiOrder(StiSeriesLabelsPropertyOrder.TextAfter)]
        [StiSerializable]
        [Description("Gets or sets text which will be shown after label text.")]
        public string TextAfter { get; set; } = "";

        /// <summary>
        /// Gets or sets angle of text rotation.
        /// </summary>
		[DefaultValue(0f)]
        [StiOrder(StiSeriesLabelsPropertyOrder.Angle)]
        [StiSerializable]
        [Description("Gets or sets angle of text rotation.")]
        public virtual float Angle { get; set; }

        /// <summary>
        /// Gets or sets format string which used for formating series values (if applicable).
        /// </summary>
		[DefaultValue("")]
        [StiOrder(StiSeriesLabelsPropertyOrder.Format)]
        [Description("Gets or sets format string which used for formating series values (if applicable).")]
        [StiSerializable]
        [Editor("Stimulsoft.Report.Chart.Design.StiFormatEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public string Format { get; set; } = "";

        /// <summary>
        /// Gets or sets value which control antialiasing drawing mode of series labels.
        /// </summary>
		[StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.Antialiasing)]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which control antialiasing drawing mode of series labels.")]
        public virtual bool Antialiasing { get; set; } = true;

        /// <summary>
        /// Gets or sets visiblity of series labels.
        /// </summary>
		[DefaultValue(true)]
        [StiOrder(StiSeriesLabelsPropertyOrder.Visible)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visiblity of series labels.")]
        public virtual bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets value which incates that border will be drawn or not.
        /// </summary>
		[DefaultValue(true)]
        [StiOrder(StiSeriesLabelsPropertyOrder.DrawBorder)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which incates that border will be drawn or not.")]
        public virtual bool DrawBorder { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that series colors must be used.
        /// </summary>
		[DefaultValue(false)]
        [StiOrder(StiSeriesLabelsPropertyOrder.UseSeriesColor)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that series colors must be used.")]
        public virtual bool UseSeriesColor { get; set; }

        /// <summary>
        /// Gets or sets foreground color of series labels.
        /// </summary>
		[StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.LabelColor)]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets foreground color of series labels.")]
        public virtual Color LabelColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets border color of series labels.
        /// </summary>
		[StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.BorderColor)]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets border color of series labels.")]
        public virtual Color BorderColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets brush which will be used to fill area of series labels.
        /// </summary>
        [StiOrder(StiSeriesLabelsPropertyOrder.Brush)]
        [StiSerializable]
        [Description("Gets or sets brush which will be used to fill area of series labels.")]
        public virtual StiBrush Brush { get; set; } = new StiSolidBrush(Color.White);

        /// <summary>
        /// Gets or sets font which will be used to draw series labels.
        /// </summary>
		[StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.Font)]
        [Description("Gets or sets font which will be used to draw series labels.")]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        public virtual Font Font { get; set; } = new Font("Arial", 7);

        [StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public IStiChart Chart { get; set; }

        /// <summary>
        /// Gets or sets word wrap.
        /// </summary>
        [StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.WordWrap)]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets word wrap.")]
        public bool WordWrap { get; set; }

        /// <summary>
        /// Gets or sets fixed width of series labels.
        /// </summary>
        [DefaultValue(0f)]
        [StiOrder(StiSeriesLabelsPropertyOrder.Width)]
        [StiSerializable]
        [Description("Gets or sets fixed width of axis labels.")]
        public int Width { get; set; }
        
        /// <summary>
        /// DBS use only!
        /// </summary>
        [Browsable(false)]
        public StiFormatService FormatService { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return ServiceName;
        }
        #endregion

        #region Methods.virtual
        public virtual StiSeriesLabels CreateNew()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}