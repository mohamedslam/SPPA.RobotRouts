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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using Stimulsoft.Base.Design;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{

    [TypeConverter(typeof(StiUniversalConverter))]
    public partial class StiChartTitle :
        IStiChartTitle,
        IStiPropertyGridObject,
        IStiFont
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool("AllowApplyStyle", allowApplyStyle, true);
            jObject.AddPropertyFontTahoma12Bold("Font", Font);
            jObject.AddPropertyStringNullOrEmpty("Text", Text);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyBool("Antialiasing", Antialiasing, true);
            jObject.AddPropertyEnum("Alignment", Alignment, StringAlignment.Center);
            jObject.AddPropertyEnum("Dock", Dock, StiChartTitleDock.Top);
            jObject.AddPropertyInt("Spacing", Spacing, 2);
            jObject.AddPropertyBool("Visible", Visible);

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

                    case "Font":
                        this.Font = property.DeserializeFont(this.Font);
                        break;

                    case "Text":
                        this.Text = property.DeserializeString();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "Antialiasing":
                        this.Antialiasing = property.DeserializeBool();
                        break;

                    case "Alignment":
                        this.Alignment = property.DeserializeEnum<StringAlignment>();
                        break;

                    case "Dock":
                        this.Dock = property.DeserializeEnum<StiChartTitleDock>(); 
                        break;

                    case "Spacing":
                        this.Spacing = property.DeserializeInt();
                        break;

                    case "Visible":
                        this.Visible = property.DeserializeBool();
                        break;

                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiChartTitle;

        [Browsable(false)]
        public string PropName => string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            var list = new[]
            {
                propHelper.ChartTitle()
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
            var title = this.MemberwiseClone() as IStiChartTitle;

            title.Alignment = this.Alignment;
            title.Font = this.Font.Clone() as Font;
            title.Brush = this.Brush.Clone() as StiBrush;

            if (this.Core != null)
            {
                title.Core = this.Core.Clone() as StiChartTitleCoreXF;
                title.Core.ChartTitle = title;
            }

            return title;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                //We especially don't check Antialiasing, Font and Brush properties because 
                //its default state controls by AllowApplySyle property
                return
                    AllowApplyStyle
                    && (Text != null && Text.Length == 0)
                    && Alignment == StringAlignment.Center
                    && Dock == StiChartTitleDock.Top
                    && Spacing == 2
                    && !Visible;
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiChartTitleCoreXF Core { get; set; }

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
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

                    if (value && this.Chart != null)
                        this.Core.ApplyStyle(this.Chart.Style);
                }
            }
        }

        /// <summary>
        /// Gets or sets font of the chart title.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets font of the chart title.")]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        public Font Font { get; set; } = new Font("Tahoma", 12, FontStyle.Bold);

        protected bool ShouldSerializeFont()
        {
            return !(Font != null && Font.Name == "Tahoma" && Font.SizeInPoints == 12 && Font.Style == FontStyle.Bold);
        }

        /// <summary>
        /// Gets or sets text of the chart title.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [Description("Gets or sets text of the chart title.")]
        public string Text { get; set; } = "";

        /// <summary>
        /// Gets or sets text brush of the chart title.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets text brush of the chart title.")]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.SaddleBrown);

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush && ((StiSolidBrush)Brush).Color == Color.SaddleBrown);
        }

        /// <summary>
        /// Gets or sets value which control antialiasing drawing mode of chart title.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which control antialiasing drawing mode of chart title.")]
        public bool Antialiasing { get; set; } = true;

        /// <summary>
        /// Gets os sets alignment of chart title.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StringAlignment.Center)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets os sets alignment of chart title.")]
        public StringAlignment Alignment { get; set; } = StringAlignment.Center;

        /// <summary>
        /// Gets or sets docking ot chart title.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiChartTitleDock.Top)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets docking ot chart title.")]
        public StiChartTitleDock Dock { get; set; } = StiChartTitleDock.Top;

        /// <summary>
        /// Gets or sets spacing between chart title and chart area.
        /// </summary>
        [StiSerializable]
        [DefaultValue(2)]
        [Description("Gets or sets spacing between chart title and chart area.")]
        public int Spacing { get; set; } = 2;

        /// <summary>
        /// Gets or sets visibility of chart title.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of chart title.")]
        public bool Visible { get; set; } = false;
        
        [Browsable(false)]
        public IStiChart Chart { get; set; } = null;
        #endregion

        public StiChartTitle()
        {
            this.Core = new StiChartTitleCoreXF(this);
        }

        [StiUniversalConstructor("Title")]
        public StiChartTitle(
            Font font,
            string text,
            StiBrush brush,
            bool antialiasing,
            StringAlignment alignment,
            StiChartTitleDock dock,
            int spacing,
            bool visible,
            bool allowApplyStyle
            )
        {
            this.Core = new StiChartTitleCoreXF(this);

            this.Font = font;
            this.Text = text;
            this.Brush = brush;
            this.Antialiasing = antialiasing;
            this.Alignment = alignment;
            this.Dock = dock;
            this.Spacing = spacing;
            this.Visible = visible;
            this.allowApplyStyle = allowApplyStyle;
        }
    }
}