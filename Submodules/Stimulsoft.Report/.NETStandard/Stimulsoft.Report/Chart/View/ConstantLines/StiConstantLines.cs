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
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing.Design;
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
    public class StiConstantLines :
        StiService,
        IStiConstantLines,
        IStiPropertyGridObject
    {
        #region enum StiOrientation
        public enum StiOrientation
        {
            Horizontal,
            Vertical,
            HorizontalRight
        }
        #endregion

        #region enum StiTextPosition
        public enum StiTextPosition
        {
            LeftTop,
            LeftBottom,
            CenterTop,
            CenterBottom,
            RightTop,
            RightBottom
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            // StiBand
            jObject.AddPropertyBool("AllowApplyStyle", AllowApplyStyle, true);
            jObject.AddPropertyBool("Antialiasing", Antialiasing, true);
            jObject.AddPropertyEnum("Position", Position, StiTextPosition.LeftTop);
            jObject.AddPropertyFontArial7("Font", Font);
            jObject.AddPropertyStringNullOrEmpty("Text", Text);
            jObject.AddPropertyBool("TitleVisible", TitleVisible, true);
            jObject.AddPropertyEnum("Orientation", Orientation, StiOrientation.Horizontal);
            jObject.AddPropertyFloat("LineWidth", LineWidth, 1f);
            jObject.AddPropertyEnum("LineStyle", LineStyle, StiPenStyle.Solid);
            jObject.AddPropertyColor("LineColor", LineColor, Color.Black);
            jObject.AddPropertyBool("ShowInLegend", ShowInLegend, true);
            jObject.AddPropertyBool("ShowBehind", ShowBehind);
            jObject.AddPropertyString("AxisValue", AxisValue, "1");
            jObject.AddPropertyBool("Visible", Visible, true);

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

                    case "Antialiasing":
                        this.Antialiasing = property.DeserializeBool();
                        break;

                    case "Position":
                        this.Position = property.DeserializeEnum<StiTextPosition>();
                        break;

                    case "Font":
                        this.Font = property.DeserializeFont(this.Font);
                        break;

                    case "Text":
                        this.Text = property.DeserializeString();
                        break;

                    case "TitleVisible":
                        this.TitleVisible = property.DeserializeBool();
                        break;

                    case "Orientation":
                        this.Orientation = property.DeserializeEnum<StiOrientation>(); 
                        break;

                    case "LineWidth":
                        this.LineWidth = property.DeserializeFloat();
                        break;

                    case "LineStyle":
                        this.LineStyle = property.DeserializeEnum<StiPenStyle>(); 
                        break;

                    case "LineColor":
                        this.LineColor = property.DeserializeColor();
                        break;

                    case "ShowInLegend":
                        this.ShowInLegend = property.DeserializeBool();
                        break;

                    case "ShowBehind":
                        this.ShowBehind = property.DeserializeBool();
                        break;

                    case "AxisValue":
                        this.AxisValue = property.DeserializeString();
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
        public StiComponentId ComponentId => StiComponentId.StiConstantLines;

        [Browsable(false)]
        public string PropName => string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();

            var propHelper = propertyGrid.PropertiesHelper;

            // BehaviorCategory
            var list = new[]
            {
                propHelper.AxisValue(),
                propHelper.LineColor(),
                propHelper.LineStyle(),
                propHelper.LineWidth(),
                propHelper.Orientation(),
                propHelper.ShowBehind(),
                propHelper.Visible()
            };
            objHelper.Add(StiPropertyCategories.Behavior, list);

            list = new[]
            {
                propHelper.AllowApplyStyle()
            };
            objHelper.Add(StiPropertyCategories.Misc, list);

            // TitleCategory
            list = new[]
            {
                propHelper.Antialiasing(),
                propHelper.Font(),
                propHelper.Position(),
                propHelper.TextNotEdit(),
                propHelper.TitleVisible()
            };
            objHelper.Add(StiPropertyCategories.Title, list);

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
        public override object Clone()
        {
            var lines = this.MemberwiseClone() as IStiConstantLines;

            if (this.Core != null)
            {
                lines.Core = this.Core.Clone() as StiConstantLinesCoreXF;
                lines.Core.ConstantLines = lines;
            }

            return lines;
        }
        #endregion

        #region StiService override
        /// <summary>
        /// Gets a service category.
        /// </summary>
        [Browsable(false)]
        public sealed override string ServiceCategory => "Chart";

        /// <summary>
        /// Gets a service type.
        /// </summary>
        [Browsable(false)]
        public sealed override Type ServiceType => typeof(StiConstantLines);
        #endregion

        #region Properties
        [Browsable(false)]
        public StiConstantLinesCoreXF Core { get; set; }

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
        [DefaultValue(true)]
        [StiCategory("Appearance")]
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
        /// Gets or sets value which control antialiasing drawing mode.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which control antialiasing drawing mode.")]
        public bool Antialiasing { get; set; } = true;

        /// <summary>
        /// Gets or sets text position at constant line.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(StiTextPosition.LeftTop)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets text position at constant line.")]
        public StiTextPosition Position { get; set; } = StiTextPosition.LeftTop;

        /// <summary>
        /// Gets or sets font which used for drawing constant line text.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets font which used for drawing constant line text.")]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        public virtual Font Font { get; set; } = new Font("Arial", 7);

        /// <summary>
        /// Gets or sets constant line text.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue("")]
        [Description("Gets or sets constant line text.")]
        public string Text { get; set; } = "";

        /// <summary>
        /// Gets or sets visibility of constant lines title.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of constant lines title.")]
        public bool TitleVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets horizontal or vertical orientation of constant line.
        /// </summary>
        [DefaultValue(StiOrientation.Horizontal)]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets horizontal or vertical orientation of constant line.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiOrientation Orientation { get; set; } = StiOrientation.Horizontal;

        /// <summary>
        /// Gets or sets constant line width.
        /// </summary>
        [DefaultValue(1f)]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets constant line width.")]
        public float LineWidth { get; set; } = 1f;

        /// <summary>
        /// Gets or sets constant line style.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiPenStyle.Solid)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets constant line style.")]
        public StiPenStyle LineStyle { get; set; } = StiPenStyle.Solid;

        /// <summary>
        /// Gets or sets color which will be used for drawing constant line.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets color which will be used for drawing constant line.")]
        public virtual Color LineColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets constant lines in chart legend.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets constant lines in chart legend.")]
        public virtual bool ShowInLegend { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that constant lines will be shown behind chart series or in front of chart series.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets value which indicates that constant lines will be shown behind chart series or in front of chart series.")]
        public virtual bool ShowBehind { get; set; }

        /// <summary>
        /// Gets or sets Y axis value through what the constant line is drawn.
        /// </summary>
        [DefaultValue("1")]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets Y axis value through what the constant line is drawn.")]
        public virtual string AxisValue { get; set; } = "1";

        /// <summary>
        /// Gets or sets visibility of constant line.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets visibility of constant line.")]
        public virtual bool Visible { get; set; } = true;

        [StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public IStiChart Chart { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Text) 
                ? $"({Loc.Get("Chart", "ConstantLine")})" 
                : Text;
        }
        #endregion

        public StiConstantLines()
        {
            this.Core = new StiConstantLinesCoreXF(this);
        }
    }
}