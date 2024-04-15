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
using Stimulsoft.Report.Chart.Design;
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
    [StiServiceBitmap(typeof(StiTrendLine), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [StiServiceCategoryBitmap(typeof(StiTrendLine), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [TypeConverter(typeof(StiTrendLineConverter))]
    public abstract class StiTrendLine :
        StiService,
        IStiTrendLine,
        IStiPropertyGridObject
    {
        #region enum StiTextPosition
        public enum StiTextPosition
        {
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyColor("LineColor", LineColor, Color.Black);
            jObject.AddPropertyFloat("LineWidth", LineWidth, 1f);
            jObject.AddPropertyEnum("LineStyle", LineStyle, StiPenStyle.Solid);
            jObject.AddPropertyBool("ShowShadow", ShowShadow);
            jObject.AddPropertyEnum("Position", Position, StiTextPosition.LeftBottom);
            jObject.AddPropertyFontArial7("Font", Font);
            jObject.AddPropertyStringNullOrEmpty("Text", Text);
            jObject.AddPropertyBool("TitleVisible", TitleVisible, true);
            jObject.AddPropertyBool("AllowApplyStyle", AllowApplyStyle, true);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "LineColor":
                        this.LineColor = property.DeserializeColor();
                        break;

                    case "LineWidth":
                        this.LineWidth = property.DeserializeFloat();
                        break;

                    case "LineStyle":
                        this.LineStyle = property.DeserializeEnum<StiPenStyle>();
                        break;

                    case "ShowShadow":
                        this.ShowShadow = property.DeserializeBool();
                        break;

                    case "Font":
                        this.Font = property.DeserializeFont(this.Font);
                        break;

                    case "Position":
                        this.Position = property.DeserializeEnum<StiTextPosition>(); 
                        break;

                    case "Text":
                        this.Text = property.DeserializeString();
                        break;

                    case "TitleVisible":
                        this.TitleVisible = property.DeserializeBool();
                        break;

                    case "AllowApplyStyle":
                        this.AllowApplyStyle = property.DeserializeBool();
                        break;
                }
            }
        }

        internal static IStiTrendLine CreateFromJsonObject(JObject jObject)
        {
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();
            var service = StiOptions.Services.ChartTrendLines.FirstOrDefault(x => x.GetType().Name == ident);

            if (service == null)
                throw new Exception($"Type {ident} is not found!");

            var trendLine = service.CreateNew();
            trendLine.LoadFromJsonObject(jObject);

            return trendLine;
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public virtual StiComponentId ComponentId => StiComponentId.StiTrendLine;

        [Browsable(false)]
        public string PropName => string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            var list = new[]
            {
                propHelper.ChartTrendLine()
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
        public override object Clone()
        {
            var line = base.Clone() as IStiTrendLine;

            if (this.Core != null)
            {
                line.Core = this.Core.Clone() as StiTrendLineCoreXF;
                line.Core.TrendLine = line;
            }

            return line;
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
        public sealed override Type ServiceType => typeof(StiTrendLine);
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                //LineColor and ShowShadow are controlled by AllowApplyStyle
                return
                    AllowApplyStyle
                    && LineWidth == 1f
                    && LineStyle == StiPenStyle.Solid                    
                    && Position == StiTextPosition.LeftBottom
                    && (Text != null && Text.Length == 0)
                    && TitleVisible;
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiTrendLineCoreXF Core { get; set; }

        /// <summary>
        /// Gets or sets color which will be used for drawing trend line.
        /// </summary>
        [StiSerializable]
        [StiOrder(StiTrendLinePropertyOrder.LineColor)]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets color which will be used for drawing trend line.")]
        public virtual Color LineColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets trend line width.
        /// </summary>
        [DefaultValue(1f)]
        [StiSerializable]
        [StiCategory("Common")]
        [StiOrder(StiTrendLinePropertyOrder.LineWidth)]
        [Description("Gets or sets trend line width.")]
        public float LineWidth { get; set; } = 1f;

        /// <summary>
        /// Gets or sets trend line style.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiPenStyle.Solid)]
        [StiCategory("Common")]
        [StiOrder(StiTrendLinePropertyOrder.LineStyle)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets trend line style.")]
        public StiPenStyle LineStyle { get; set; } = StiPenStyle.Solid;

        /// <summary>
        /// Gets or sets value which indicates draw shadow or no.
        /// </summary>
        [StiOrder(StiTrendLinePropertyOrder.ShowShadow)]
        [DefaultValue(true)]
        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates draw shadow or no.")]
        public bool ShowShadow { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
        [DefaultValue(true)]
        public bool AllowApplyStyle { get; set; } = true;
        
        /// <summary>
        /// Gets or sets font which used for drawing constant line text.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets font which used for drawing constant line text.")]
        public virtual Font Font { get; set; } = new Font("Arial", 7);

        private bool ShouldSerializeFont()
        {
            return !(Font != null && Font.Name == "Arial" && Font.SizeInPoints == 7 && Font.Style == FontStyle.Regular);
        }

        /// <summary>
        /// Gets or sets text position at constant line.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(StiTextPosition.LeftBottom)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets text position at trend line.")]
        public StiTextPosition Position { get; set; } = StiTextPosition.LeftBottom;

        /// <summary>
        /// Gets or sets trend line text.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue("")]
        [Description("Gets or sets trend line text.")]
        public string Text { get; set; } = "";

        /// <summary>
        /// Gets or sets visibility of trend lines title.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of trend line title.")]
        public bool TitleVisible { get; set; } = true;
        #endregion

        #region Methods
        public abstract StiTrendLine CreateNew();

        public override string ToString()
        {
            return ServiceName;
        }
        #endregion

        public StiTrendLine()
        {
        }
    }
}
