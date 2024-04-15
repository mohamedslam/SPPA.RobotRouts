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
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Components;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Chart
{
	[TypeConverter(typeof(StiUniversalConverter))]
    public class StiLegend : 
        IStiLegend,
		IStiSerializeToCodeAsClass,
		ICloneable,
        IStiPropertyGridObject,
        IStiFont
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool("AllowApplyStyle", allowApplyStyle, true);
            jObject.AddPropertyBool("HideSeriesWithEmptyTitle", HideSeriesWithEmptyTitle);
            jObject.AddPropertyBool("ShowShadow", ShowShadow, true);
            jObject.AddPropertyColor("BorderColor", BorderColor, Color.Gray);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyColor("TitleColor", TitleColor, Color.Gray);
            jObject.AddPropertyColor("LabelsColor", LabelsColor, Color.Gray);
            jObject.AddPropertyEnum("Direction", Direction, StiLegendDirection.TopToBottom);
            jObject.AddPropertyEnum("HorAlignment", HorAlignment, StiLegendHorAlignment.Left);
            jObject.AddPropertyEnum("VertAlignment", VertAlignment, StiLegendVertAlignment.Top);
            jObject.AddPropertyFontArial14Bold("TitleFont", TitleFont);
            jObject.AddPropertyFontArial8("Font", Font);
            jObject.AddPropertyBool("Visible", Visible, true);
            jObject.AddPropertyBool("MarkerVisible", MarkerVisible, true);
            jObject.AddPropertyBool("MarkerBorder", MarkerBorder, true);
            jObject.AddPropertySize("MarkerSize", MarkerSize);
            jObject.AddPropertyEnum("MarkerAlignment", MarkerAlignment, StiMarkerAlignment.Left);
            jObject.AddPropertyInt("Columns", columns, 0);
            jObject.AddPropertyInt("HorSpacing", HorSpacing, 4);
            jObject.AddPropertyInt("VertSpacing", VertSpacing, 2);
            jObject.AddPropertySizeD("Size", Size);
            jObject.AddPropertyStringNullOrEmpty("Title", Title);
            jObject.AddPropertyInt(nameof(ColumnWidth), ColumnWidth);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "AllowApplyStyle":
                        this.AllowApplyStyle = property.DeserializeBool();
                        break;

                    case "HideSeriesWithEmptyTitle":
                        this.HideSeriesWithEmptyTitle = property.DeserializeBool();
                        break;

                    case "ShowShadow":
                        this.ShowShadow = property.DeserializeBool();
                        break;

                    case "BorderColor":
                        this.BorderColor = property.DeserializeColor();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "TitleColor":
                        this.TitleColor = property.DeserializeColor();
                        break;

                    case "LabelsColor":
                        this.LabelsColor = property.DeserializeColor();
                        break;

                    case "Direction":
                        this.Direction = property.DeserializeEnum<StiLegendDirection>();
                        break;

                    case "HorAlignment":
                        this.HorAlignment = property.DeserializeEnum<StiLegendHorAlignment>(); 
                        break;

                    case "VertAlignment":
                        this.VertAlignment = property.DeserializeEnum<StiLegendVertAlignment>();
                        break;

                    case "TitleFont":
                        this.TitleFont = property.DeserializeFont(this.TitleFont);
                        break;

                    case "Font":
                        this.Font = property.DeserializeFont(this.Font);
                        break;

                    case "Visible":
                        this.Visible = property.DeserializeBool();
                        break;

                    case "MarkerVisible":
                        this.MarkerVisible = property.DeserializeBool();
                        break;

                    case "MarkerBorder":
                        this.MarkerBorder = property.DeserializeBool();
                        break;

                    case "MarkerSize":
                        this.MarkerSize = property.DeserializeSize();
                        break;

                    case "MarkerAlignment":
                        this.MarkerAlignment = property.DeserializeEnum<StiMarkerAlignment>(); 
                        break;

                    case "Columns":
                        this.columns = property.DeserializeInt();
                        break;

                    case "HorSpacing":
                        this.HorSpacing = property.DeserializeInt();
                        break;

                    case "VertSpacing":
                        this.VertSpacing = property.DeserializeInt();
                        break;

                    case "Size":
                        this.Size = property.DeserializeSizeD();
                        break;

                    case "Title":
                        this.Title = property.DeserializeString();
                        break;

                    case nameof(ColumnWidth):
                        this.ColumnWidth = property.DeserializeInt();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiLegend;
            }
        }

        [Browsable(false)]
        public string PropName
        {
            get
            {
                return string.Empty;
            }
        }

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[] 
            {
                propHelper.Legend()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion
        
        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            IStiLegend legend = this.MemberwiseClone() as IStiLegend;
            legend.Brush = this.Brush.Clone() as StiBrush;
            legend.Font = this.Font.Clone() as Font;
            legend.TitleFont = this.TitleFont.Clone() as Font;
            legend.Direction = this.Direction;
            legend.HorAlignment = this.HorAlignment;
            legend.VertAlignment = this.VertAlignment;
            legend.MarkerAlignment = this.MarkerAlignment;

            if (this.Core != null)
            {
                legend.Core = this.Core.Clone() as StiLegendCoreXF;
                legend.Core.Legend = legend;
            }

            return legend;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                //We especially don't check ShowShadow, BorderColor, Brush, TitleColor, LabelsColor,
                //TitleFont, Font properties because its default state controls 
                //by AllowApplySyle property
                return
                    AllowApplyStyle
                    && !HideSeriesWithEmptyTitle
                    && Direction == StiLegendDirection.TopToBottom
                    && HorAlignment == StiLegendHorAlignment.Left
                    && VertAlignment == StiLegendVertAlignment.Top
                    && Visible
                    && MarkerVisible
                    && MarkerBorder
                    && !ShouldSerializeMarkerSize()
                    && MarkerAlignment == StiMarkerAlignment.Left
                    && Columns == 0
                    && HorSpacing == 4
                    && VertSpacing == 2
                    && ColumnWidth == 0
                    && !ShouldSerializeSize()
                    && (Title != null && Title.Length == 0);
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiLegendCoreXF Core { get; set; }

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
        [DefaultValue(true)]
        [StiPropertyLevel(StiLevel.Standard)]
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

        [StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public IStiChart Chart { get; set; } = null;

        /// <summary>
        /// Gets or sets value which shows/hides series with empty title.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which shows/hides series with empty title.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool HideSeriesWithEmptyTitle { get; set; }

        /// <summary>
        /// Gets or sets value which indicates draw shadow or no.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates draw shadow or no.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public bool ShowShadow { get; set; } = true;

        /// <summary>
        /// Gets or sets border color.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets border color.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public Color BorderColor { get; set; } = Color.Gray;

        private bool ShouldSerializeBorderColor()
        {
            return BorderColor != Color.Gray;
        }

        /// <summary>
        /// Gets or sets background brush of legend.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets background brush of legend.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.White);

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush && ((StiSolidBrush)Brush).Color == Color.White);
        }

        /// <summary>
        /// Gets or sets title color of legend.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets title color of legend.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public Color TitleColor { get; set; } = Color.Gray;

        private bool ShouldSerializeTitleColor()
        {
            return TitleColor != Color.Gray;
        }

        /// <summary>
        /// Gets or sets color of the labels.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets color of the labels.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public Color LabelsColor { get; set; } = Color.Gray;

        private bool ShouldSerializeLabelsColor()
        {
            return LabelsColor != Color.Gray;
        }

        /// <summary>
        /// Gets or sets direction which used for series drawing in legend.
        /// </summary>
		[StiSerializable]
		[DefaultValue(StiLegendDirection.TopToBottom)]
		[TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets direction which used for series drawing in legend.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual StiLegendDirection Direction { get; set; } = StiLegendDirection.TopToBottom;

		/// <summary>
		/// Gets or sets horizontal alignment of legend placement.
		/// </summary>
		[StiSerializable]
        [DefaultValue(StiLegendHorAlignment.Left)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets horizontal alignment of legend placement.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual StiLegendHorAlignment HorAlignment { get; set; } = StiLegendHorAlignment.Left;

		/// <summary>
        /// Gets or sets vertical alignment of legend placement.
		/// </summary>
		[StiSerializable]
		[DefaultValue(StiLegendVertAlignment.Top)]
		[TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets vertical alignment of legend placement.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual StiLegendVertAlignment VertAlignment { get; set; } = StiLegendVertAlignment.Top;

        /// <summary>
        /// Gets or sets title font of the chart legend.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets title font of the chart legend.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        public Font TitleFont { get; set; } = new Font("Arial", 14, FontStyle.Bold);

        private bool ShouldSerializeTitleFont()
        {
            return !(TitleFont != null && TitleFont.Name == "Arial" && TitleFont.SizeInPoints == 14 && TitleFont.Style == FontStyle.Bold);
        }

        /// <summary>
        /// Gets or sets font which used for series title drawing in chart legend.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets font which used for series title drawing in chart legend.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        public Font Font { get; set; } = new Font("Arial", 8);

        private bool ShouldSerializeFont()
        {
            return !(Font != null && Font.Name == "Arial" && Font.SizeInPoints == 14 && Font.Style == FontStyle.Bold);
        }

        /// <summary>
        /// Gets or sets visibility of chart legend.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of chart legend.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets visibility of markers.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of markers.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool MarkerVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets show a border marker.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets show a border marker.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public bool MarkerBorder { get; set; } = true;

        /// <summary>
        /// Gets or sets marker size.
        /// </summary>
		[StiSerializable]
        [Description("Gets or sets marker size.")]
        [TypeConverter(typeof(StiSizeConverter))]
        [StiPropertyLevel(StiLevel.Professional)]
        public Size MarkerSize { get; set; } = new Size(10, 10);

        private bool ShouldSerializeMarkerSize()
        {
            return MarkerSize.Width != 10 || MarkerSize.Height != 10;
        }

        /// <summary>
        /// Gets or sets alignment of markers related to series title.
        /// </summary>
		[StiSerializable]
		[DefaultValue(StiMarkerAlignment.Left)]
		[TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets alignment of markers related to series title.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiMarkerAlignment MarkerAlignment { get; set; } = StiMarkerAlignment.Left;

		private int columns = 0;
        /// <summary>
        /// Gets or sets amount of columns.
        /// </summary>
		[StiSerializable]
		[DefaultValue(0)]
        [Description("Gets or sets amount of columns.")]
        [Editor("Stimulsoft.Report.Design.Components.StiLegendColumnsEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public int Columns
		{
			get
			{
				return columns;
			}
			set
			{
				if (value >= 0)
				{
					columns = value;
				}
			}
		}

        private int columnWidth = 0;
        /// <summary>
        /// Gets or sets amount of columns.
        /// </summary>
		[StiSerializable]
        [DefaultValue(0)]
        [StiPropertyLevel(StiLevel.Standard)]
        [Description("Gets or sets width of column.")]
        public int ColumnWidth
        {
            get
            {
                return columnWidth;
            }
            set
            {
                if (value >= 0)
                {
                    columnWidth = value;
                }
            }
        }

        [StiNonSerialized]
        [DefaultValue(false)]
        [Browsable(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets word wrap.")]
        public bool WordWrap { get; set; }//To do property

        /// <summary>
        /// Gets or sets horizontal spacing between items in legend.
        /// </summary>
        [StiSerializable]
        [DefaultValue(4)]
        [Description("Gets or sets horizontal spacing between items in legend.")]
        public int HorSpacing { get; set; } = 4;

        /// <summary>
        /// Gets or sets vertical spacing between items in legend.
        /// </summary>
        [StiSerializable]
        [DefaultValue(2)]
        [Description("Gets or sets vertical spacing between items in legend.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public int VertSpacing { get; set; } = 2;

        /// <summary>
        /// Gets or sets size of legend.
        /// </summary>
		[StiSerializable]
        [Description("Gets or sets size of legend.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public SizeD Size { get; set; }

        private bool ShouldSerializeSize()
        {
            return Size.Width != 0d || Size.Height != 0d;
        }

        /// <summary>
        /// Gets or sets title of the legend.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [Description("Gets or sets title of the legend.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string Title { get; set; } = "";
        #endregion

        [StiUniversalConstructor("Legend")]
		public StiLegend()
		{
            this.Core = new StiLegendCoreXF(this);
		}
	}
}
