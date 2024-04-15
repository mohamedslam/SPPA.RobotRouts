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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base;
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
	public class StiStrips : 
        StiService,
        IStiStrips,
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

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool("AllowApplyStyle", AllowApplyStyle, true);
            jObject.AddPropertyBool("ShowBehind", ShowBehind, true);
            jObject.AddPropertyBrush("StripBrush", StripBrush);
            jObject.AddPropertyBool("Antialiasing", Antialiasing, true);
            jObject.AddPropertyFontArial7("Font", Font);
            jObject.AddPropertyStringNullOrEmpty("Text", Text);
            jObject.AddPropertyBool("TitleVisible", TitleVisible, true);
            jObject.AddPropertyColor("TitleColor", TitleColor, Color.Green);
            jObject.AddPropertyEnum("Orientation", Orientation, StiOrientation.Horizontal);
            jObject.AddPropertyBool("ShowInLegend", ShowInLegend, true);
            jObject.AddPropertyString("MaxValue", MaxValue, "1");
            jObject.AddPropertyString("MinValue", MinValue, "0");
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

                    case "ShowBehind":
                        this.ShowBehind = property.DeserializeBool();
                        break;

                    case "StripBrush":
                        this.StripBrush = property.DeserializeBrush();
                        break;

                    case "Antialiasing":
                        this.Antialiasing = property.DeserializeBool();
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

                    case "TitleColor":
                        this.TitleColor = property.DeserializeColor();
                        break;

                    case "Orientation":
                        this.Orientation = property.DeserializeEnum<StiOrientation>();
                        break;

                    case "ShowInLegend":
                        this.ShowInLegend = property.DeserializeBool();
                        break;

                    case "MaxValue":
                        this.MaxValue = property.DeserializeString();
                        break;

                    case "MinValue":
                        this.MinValue = property.DeserializeString();
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
        public StiComponentId ComponentId => StiComponentId.StiStrips;

        [Browsable(false)]
        public string PropName => string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, Base.StiLevel level)
        {
            var objHelper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            // ChartCategory
            var list = new[] 
            { 
                propHelper.AllowApplyStyle()
            };
            objHelper.Add(StiPropertyCategories.Chart, list);

            // BehaviorCategory
            list = new[] 
            { 
                propHelper.MaxValue(),
                propHelper.MinValue(),
                propHelper.Orientation(),
                propHelper.ShowBehind(),
                propHelper.StripBrush(),
                propHelper.Visible()
            };
            objHelper.Add(StiPropertyCategories.Behavior, list);

            // DataCategory
            list = new[] 
            { 
                propHelper.Antialiasing(),
                propHelper.Font(),
                propHelper.TextNotEdit(),
                propHelper.TitleColor(),
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
            var strips = this.MemberwiseClone() as IStiStrips;

            if (this.Core != null)
            {
                strips.Core = this.Core.Clone() as StiStripsCoreXF;
                strips.Core.Strips = strips;
            }

            return strips;
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
        public sealed override Type ServiceType => typeof(StiStrips);
        #endregion

        #region Properties
        [Browsable(false)]
        public StiStripsCoreXF Core { get; set; }

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
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
        /// Gets or sets value which indicates that strips will be shown behind chart series or in front of chart series.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets value which indicates that strips will be shown behind chart series or in front of chart series.")]
        public virtual bool ShowBehind { get; set; } = true;

        /// <summary>
        /// Gets or sets brush which will be used for drawing strips.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets brush which will be used for drawing strips.")]
        public StiBrush StripBrush { get; set; } = new StiSolidBrush(Color.Transparent);

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
        /// Gets or sets font of title text.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets font of title text.")]
        public virtual Font Font { get; set; } = new Font("Arial", 7);

        /// <summary>
        /// Gets or sets title text.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue("")]
        [Description("Gets or sets title text.")]
        public string Text { get; set; } = "";

        /// <summary>
        /// Gets or sets visibility of title.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of title.")]
        public bool TitleVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets foreground color of title.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets foreground color of title.")]
        public virtual Color TitleColor { get; set; } = Color.Green;

        /// <summary>
        /// Gets or sets horizontal or vertical orientation of strips.
        /// </summary>
        [DefaultValue(StiOrientation.Horizontal)]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets horizontal or vertical orientation of strips.")]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiOrientation Orientation { get; set; } = StiOrientation.Horizontal;

        /// <summary>
        /// Gets or sets value which indicates that strips will be shown in legend of chart.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets value which indicates that strips will be shown in legend of chart.")]
        public virtual bool ShowInLegend { get; set; } = true;

        /// <summary>
        /// Gets or sets maximal range of strips.
        /// </summary>
        [DefaultValue("1")]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets maximal value of strips.")]
        public virtual string MaxValue { get; set; } = "1";

        /// <summary>
        /// Gets or sets minimal range of strips.
        /// </summary>
        [DefaultValue("0")]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets minimal range of strips.")]
        public virtual string MinValue { get; set; } = "0";

        /// <summary>
        /// Gets or sets visibility of strips.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets visibility of strips.")]
        public virtual bool Visible { get; set; } = true;

        [StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public IStiChart Chart { get; set; }
        #endregion

        public StiStrips()
        {
            this.Core = new StiStripsCoreXF(this);
        }
	}
}
