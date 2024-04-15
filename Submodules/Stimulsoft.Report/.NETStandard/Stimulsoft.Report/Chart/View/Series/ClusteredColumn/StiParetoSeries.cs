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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiParetoSeries : StiSeries, IStiParetoSeries
    {
        #region IStiJsonReportObject.override
        [Browsable(false)]
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiParetoSeries;
            }
        }

        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyFloat("Width", width, 0.9f);
            jObject.AddPropertyColor("BorderColor", borderColor, Color.Gray);
            jObject.AddPropertyBrush("Brush", brush);

            jObject.RemoveProperty("Conditions");

            jObject.AddPropertyJObject("Marker", Marker.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("LineMarker", LineMarker.SaveToJsonObject(mode));
            jObject.AddPropertyColor("LineColor", LineColor, Color.Black);
            jObject.AddPropertyEnum("LineStyle", LineStyle, StiPenStyle.Solid);
            jObject.AddPropertyBool("Lighting", Lighting, true);
            jObject.AddPropertyFloat("LineWidth", LineWidth, 2f);
            jObject.AddPropertyCornerRadius(nameof(CornerRadius), CornerRadius);

            jObject.AddPropertyBool("AllowApplyLineColor", AllowApplyLineColor);
            if (this.Icon != null)
                jObject.AddPropertyEnum("Icon", this.Icon);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {

                    case "Width":
                        this.width = property.DeserializeFloat();
                        break;

                    case "BorderColor":
                        this.borderColor = property.DeserializeColor();
                        break;

                    case "Brush":
                        this.brush = property.DeserializeBrush();
                        break;

                    case "Marker":
                        this.Marker.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "LineMarker":
                        this.LineMarker.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "LineColor":
                        this.LineColor = property.DeserializeColor();
                        break;

                    case "LineStyle":
                        this.LineStyle = property.DeserializeEnum<StiPenStyle>();
                        break;

                    case "Lighting":
                        this.Lighting = property.DeserializeBool();
                        break;

                    case "LineWidth":
                        this.LineWidth = property.DeserializeFloat();
                        break;

                    case "AllowApplyLineColor":
                        this.AllowApplyLineColor = property.DeserializeBool();
                        break;

                    case "Icon":
                        this.Icon = property.DeserializeEnum<StiFontIcons>(); 
                        break;

                    case nameof(CornerRadius):
                        CornerRadius = property.DeserializeCornerRadius();
                        break;
                }
            }
        }

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // Value
            var list = new[]
            {
                propHelper.ValueDataColumn(),
                propHelper.Value(),
                propHelper.ListOfValues()
            };
            objHelper.Add(StiPropertyCategories.Value, list);

            // Argument
            list = new[]
            {
                propHelper.ArgumentDataColumn(),
                propHelper.Argument(),
                propHelper.ListOfArguments()
            };
            objHelper.Add(StiPropertyCategories.Argument, list);

            // Data
            list = new[]
            {
                propHelper.Format(),
                propHelper.AutoSeriesKeyDataColumn(),
                propHelper.AutoSeriesColorDataColumn(),
                propHelper.AutoSeriesTitleDataColumn()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            // Appearance
            list = new[]
            {
                propHelper.BorderColor(),
                propHelper.Brush(),
                propHelper.ShowShadow(),
                propHelper.LabelsOffset(),
                propHelper.Lighting(),
                propHelper.LineColor(),
                propHelper.LineStyle(),
                propHelper.LineWidth()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // Behavior
            list = new[]
            {
                propHelper.AllowApplyStyle(),
                propHelper.ShowInLegend(),
                propHelper.ShowSeriesLabels(),
                propHelper.Title(),
                propHelper.YAxis(),
                propHelper.fWidth()
            };
            objHelper.Add(StiPropertyCategories.Behavior, list);

            return objHelper;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var series = base.Clone() as IStiParetoSeries;

            series.Brush = this.Brush.Clone() as StiBrush;            
            series.Marker = this.Marker.Clone() as IStiMarker;
            series.LineStyle = this.LineStyle;

            return series;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiParetoArea);
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiParetoSeries();
        }
        #endregion

        #region Properties IStiClusteredColumnSeries
        [StiNonSerialized]
        [Browsable(false)]
        public bool ShowZeros { get; set; }

        [StiNonSerialized]
        [Browsable(false)]
        public virtual StiBrush BrushNegative { get; set; }

        [StiNonSerialized]
        [Browsable(false)]
        public virtual bool AllowApplyBrushNegative { get; set; }

        private float width = 0.9f;
        /// <summary>
        /// Gets or sets the width factor of one bar series. Value 1 is equal to 100%.
        /// </summary>
		[StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(0.9f)]
        [Description("Gets or sets the width factor of one bar series. Value 1 is equal to 100%.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value >= 0.01f && value <= 1f) width = value;
            }
        }

        private Color borderColor = Color.Gray;
        /// <summary>
        /// Gets or sets border color of series bar.
        /// </summary>
		[StiSerializable]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets border color of series bar.")]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets border thickness of series bar.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets border thickness of series bar.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public int BorderThickness { get; set; }

        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Represents the value to which the corners are rounded.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiCornerRadius CornerRadius { get; set; } = new StiCornerRadius();

        private StiBrush brush = new StiSolidBrush(Color.Gainsboro);
        /// <summary>
        /// Gets or sets brush which will used to fill bar area.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets brush which will used to fill bar area.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiBrush Brush
        {
            get
            {
                return brush;
            }
            set
            {
                brush = value;
            }
        }

        #endregion

        #region Properties IStiBaseLineSeries
        [StiNonSerialized]
        [Browsable(false)]
        public virtual bool ShowNulls { get; set; }

        [StiNonSerialized]
        [Browsable(false)]
        public virtual Color LineColorNegative { get; set; }

        [StiNonSerialized]
        [Browsable(false)]
        public virtual bool AllowApplyColorNegative { get; set; }

        [StiNonSerialized]
        [Browsable(false)]
        public int LabelsOffset { get; set; }

        private IStiMarker marker = new StiMarker();
        /// <summary>
        /// Gets or sets marker settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Appearance")]
        [Description("Gets or sets marker settings.")]
        [Browsable(false)]
        [TypeConverter(typeof(Stimulsoft.Report.Chart.Design.StiMarkerConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual IStiMarker Marker
        {
            get
            {
                return marker;
            }
            set
            {
                marker = value;
            }
        }


        private IStiLineMarker lineMarker = new StiLineMarker();
        /// <summary>
        /// Gets or sets line marker settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Appearance")]
        [Description("Gets or sets line marker settings.")]
        [Browsable(false)]
        [TypeConverter(typeof(Stimulsoft.Report.Chart.Design.StiLineMarkerConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual IStiLineMarker LineMarker
        {
            get
            {
                return lineMarker;
            }
            set
            {
                lineMarker = value;
            }
        }


        private Color lineColor = Color.Black;
        /// <summary>
        /// Gets or sets line color of series.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets line color of series.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual Color LineColor
        {
            get
            {
                return lineColor;
            }
            set
            {
                lineColor = value;
            }
        }


        private StiPenStyle lineStyle = StiPenStyle.Solid;
        /// <summary>
        /// Gets or sets a line style of series.
        /// </summary>
        [Editor(StiEditors.PenStyle, typeof(UITypeEditor))]
        [DefaultValue(StiPenStyle.Solid)]
        [Description("Gets or sets a line style of series.")]
        [StiSerializable]
        [StiCategory("Common")]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiPenStyle LineStyle
        {
            get
            {
                return lineStyle;
            }
            set
            {
                lineStyle = value;
            }
        }


        private bool lighting = true;
        /// <summary>
        /// Gets or sets value which indicates that light effect will be shown.
        /// </summary>
		[DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that light effect will be shown.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool Lighting
        {
            get
            {
                return lighting;
            }
            set
            {
                lighting = value;
            }
        }


        private float lineWidth = 2f;
        /// <summary>
        /// Gets or sets line width of series.
        /// </summary>
		[DefaultValue(2f)]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets line width of series.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual float LineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                if (value > 0)
                {
                    lineWidth = value;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value which indicates that pareto line color will be used.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that pareto line color will be used.")]
        [DefaultValue(false)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool AllowApplyLineColor { get; set; }

        private StiSeriesSortType sortBy = StiSeriesSortType.Value;
        /// <summary>
        /// Gets or sets mode of series values sorting.
        /// </summary>
        [DefaultValue(StiSeriesSortType.Value)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets mode of series values sorting.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public override StiSeriesSortType SortBy
        {
            get
            {
                return sortBy;
            }
            set
            {
                sortBy = value;
            }
        }


        private StiSeriesSortDirection sortDirection = StiSeriesSortDirection.Descending;
        /// <summary>
        /// Gets or sets sort direction.
        /// </summary>
        [DefaultValue(StiSeriesSortDirection.Descending)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets sort direction.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public override StiSeriesSortDirection SortDirection
        {
            get
            {
                return sortDirection;
            }
            set
            {
                sortDirection = value;
            }
        }

        /// <summary>
        /// Internal use only. Special for DBS.
        /// </summary>
        [StiNonSerialized]
        [Browsable(false)]
        public StiShowEmptyCellsAs ShowNullsAs { get; set; } = StiShowEmptyCellsAs.Gap;

        /// <summary>
        /// Internal use only. Special for DBS.
        /// </summary>
        [StiNonSerialized]
        [Browsable(false)]
        public StiShowEmptyCellsAs ShowZerosAs { get; set; } = StiShowEmptyCellsAs.Gap;

        [DefaultValue(null)]
        [StiSerializable]
        [StiCategory("Common")]
        [Editor("Stimulsoft.Report.Design.StiFontIconEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Report.Design.StiFontIconConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiFontIcons? Icon { get; set; }
        #endregion

        public StiParetoSeries()
        {
            this.Core = new StiParetoSeriesCoreXF(this);
        }
    }
}
