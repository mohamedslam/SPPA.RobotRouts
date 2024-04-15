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
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Images;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Pen = Stimulsoft.Drawing.Pen;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Describes the class that contains a style for Map components.
    /// </summary>	
    public class StiMapStyle : 
        StiBaseStyle
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            if (Heatmap != null)
                jObject.AddPropertyJObject(nameof(Heatmap), Heatmap.SaveToJsonObject(mode));

            if (HeatmapWithGroup != null)
                jObject.AddPropertyJObject(nameof(HeatmapWithGroup), HeatmapWithGroup.SaveToJsonObject(mode));

            jObject.AddPropertyColor(nameof(IndividualColor), IndividualColor, "#70ad47");
            jObject.AddPropertyArrayColor(nameof(Colors), Colors);
            jObject.AddPropertyColor(nameof(DefaultColor), DefaultColor, "#4472c4");
            jObject.AddPropertyColor(nameof(BackColor), BackColor, Color.White);
            jObject.AddPropertyDouble(nameof(BorderSize), BorderSize, 0.7);
            jObject.AddPropertyColor(nameof(BorderColor), BorderColor, Color.White);
            jObject.AddPropertyColor(nameof(LabelShadowForeground), LabelShadowForeground, Color.FromArgb(255, 251, 251, 251));
            jObject.AddPropertyColor(nameof(LabelForeground), LabelForeground, Color.FromArgb(255, 37, 37, 37));
            jObject.AddPropertyColor(nameof(BubbleBackColor), BubbleBackColor, Color.Red);
            jObject.AddPropertyColor(nameof(BubbleBorderColor), BubbleBorderColor, Color.White);
            jObject.AddPropertyBrush(nameof(ToolTipBrush), ToolTipBrush);
            jObject.AddPropertyBrush(nameof(ToolTipTextBrush), ToolTipTextBrush);
            jObject.AddPropertyCornerRadius(nameof(ToolTipCornerRadius), ToolTipCornerRadius);
            jObject.AddPropertyBorder(nameof(ToolTipBorder), ToolTipBorder);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Heatmap):
                        Heatmap.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(HeatmapWithGroup):
                        HeatmapWithGroup.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(IndividualColor):
                        this.IndividualColor = property.DeserializeColor();
                        break;

                    case nameof(Colors):
                        this.Colors = property.DeserializeArrayColor();
                        break;

                    case nameof(DefaultColor):
                        this.DefaultColor = property.DeserializeColor();
                        break;

                    case nameof(BackColor):
                        this.BackColor = property.DeserializeColor();
                        break;

                    case nameof(BorderSize):
                        this.BorderSize = property.DeserializeDouble();
                        break;

                    case nameof(BorderColor):
                        this.BorderColor = property.DeserializeColor();
                        break;

                    case nameof(LabelShadowForeground):
                        this.LabelShadowForeground = property.DeserializeColor();
                        break;

                    case nameof(LabelForeground):
                        this.LabelForeground = property.DeserializeColor();
                        break;

                    case nameof(BubbleBackColor):
                        this.BubbleBackColor = property.DeserializeColor();
                        break;

                    case nameof(BubbleBorderColor):
                        this.BubbleBorderColor = property.DeserializeColor();
                        break;

                    case nameof(ToolTipBrush):
                        this.ToolTipBrush = property.DeserializeBrush();
                        break;

                    case nameof(ToolTipTextBrush):
                        this.ToolTipTextBrush = property.DeserializeBrush();
                        break;

                    case nameof(ToolTipCornerRadius):
                        this.ToolTipCornerRadius = property.DeserializeCornerRadius();
                        break;

                    case nameof(ToolTipBorder):
                        this.ToolTipBorder = property.DeserializeSimpleBorder();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiMapStyle;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.StyleName(),
                propHelper.Description(),
                propHelper.StyleCollectionName(),
                propHelper.StyleConditions()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            list = new[]
            {
                propHelper.BackColor(),
                propHelper.Colors(),
                propHelper.DefaultColor(),
                propHelper.Heatmap(),
                propHelper.HeatmapWithGroup(),
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            list = new[]
            {
                propHelper.BorderColor(),
                propHelper.BorderSize(),
            };
            objHelper.Add(StiPropertyCategories.Map, list);

            return objHelper;
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var style = base.Clone() as StiMapStyle;
            style.Colors = this.Colors?.ToArray();
            style.Heatmap = this.Heatmap.Clone() as StiHeatmapStyleData;
            style.HeatmapWithGroup = this.HeatmapWithGroup.Clone() as StiHeatmapWithGroupStyleData;
            style.ToolTipBrush = (StiBrush)this.ToolTipBrush.Clone();
            style.ToolTipTextBrush = (StiBrush)this.ToolTipTextBrush.Clone();
            style.ToolTipCornerRadius = (StiCornerRadius)this.ToolTipCornerRadius.Clone();
            style.ToolTipBorder = (StiSimpleBorder)this.ToolTipBorder.Clone();

            return style;
        }
        #endregion

        #region Consts
        private Color[] DefaultColors =
        {
            ColorTranslator.FromHtml("#70ad47"),
            ColorTranslator.FromHtml("#4472c4"),
            ColorTranslator.FromHtml("#ffc000"),
            ColorTranslator.FromHtml("#43682b"),
            ColorTranslator.FromHtml("#fd6a37"),
            ColorTranslator.FromHtml("#997300"),
        };
        #endregion

        #region Properties
        [StiNonSerialized]
        [Browsable(false)]
        [Obsolete("This property is obsoleted. Please use Heatmap/HeatmapWithGroup properties.")]
        public virtual Color[] HeatmapColors 
        {
            get
            {
                return null;
            }
            set
            {
                if (value != null && value.Length > 0)
                {
                    Heatmap.Color = value[0];
                    HeatmapWithGroup.Colors = value;
                }
            }
        }

        /// <summary>
        /// The properties set define the settings will be used for heatmap.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiCategory("Appearance")]
        [Description("The properties set define the settings will be used for heatmap.")]
        public StiHeatmapStyleData Heatmap { get; set; } = new StiHeatmapStyleData();

        private bool ShouldSerializeHeatmap()
        {
            return Heatmap?.IsDefault() != true;
        }

        /// <summary>
        /// The properties set define the settings will be used for heatmap with group.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiCategory("Appearance")]
        [Description("The properties set define the settings will be used for heatmap with group.")]
        public StiHeatmapWithGroupStyleData HeatmapWithGroup { get; set; } = new StiHeatmapWithGroupStyleData();

        private bool ShouldSerializeHeatmapWithGroup()
        {
            return HeatmapWithGroup?.IsDefault() != true;
        }


        /// <summary>
        /// The property defines the color will be used to fill the map geographic objects.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Description("The property defines the color will be used to fill the map geographic objects.")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color IndividualColor { get; set; } = ColorTranslator.FromHtml("#70ad47");

        private bool ShouldSerializeIndividualColor()
        {
            return IndividualColor != ColorTranslator.FromHtml("#70ad47");
        }

        /// <summary>
        /// The property allows to create colors set which will be used to fill the map geographic objects when color each mode is used.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiStyleColorsConverter))]
        [StiCategory("Appearance")]
        [Description("The property allows to create colors set which will be used to fill the map geographic objects when color each mode is used.")]
        [Editor("Stimulsoft.Report.Components.Design.StiColorsCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color[] Colors { get; set; }

        private bool ShouldSerializeColors()
        {
            return Colors == null ||
                   Colors.Length != DefaultColors.Length ||
                   !Colors.ToList().SequenceEqual(DefaultColors);
        }


        /// <summary>
        /// The property defines the color by default will be used to fill the map geographic objects when the map type is setted any type except individual.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Description("The property defines the color by default will be used to fill the map geographic objects when the map type is setted any type except individual.")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color DefaultColor { get; set; } = ColorTranslator.FromHtml("#4472c4");

        private bool ShouldSerializeDefaultColor()
        {
            return DefaultColor != ColorTranslator.FromHtml("#4472c4");
        }

        /// <summary>
        /// The property defines the color will be used to fill map background.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Description("The property defines the color will be used to fill map background.")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color BackColor { get; set; } = Color.White;

        private bool ShouldSerializeBackColor()
        {
            return BackColor != Color.White;
        }

        [StiNonSerialized]
        [Browsable(false)]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Obsolete("ForeColor property is obsolete. Please use LabelForeground property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color ForeColor { get; set; } = Color.White;

        /// <summary>
        /// The property defines the border width will be used for the map geographic objects.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("The property defines the border width will be used for the map geographic objects.")]
        [DefaultValue(0.5d)]
        [StiPropertyLevel(StiLevel.Basic)]
        public double BorderSize { get; set; } = 0.7;

        private bool ShouldSerializeBorderSize()
        {
            return BorderSize != 0.7;
        }


        /// <summary>
        /// The property defines the color will be used to draw borders of the map geographic objects.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiColorConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("The property defines the color will be used to draw borders of the map geographic objects.")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color BorderColor { get; set; } = Color.White;

        private bool ShouldSerializeBorderColor()
        {
            return BackColor != Color.White;
        }


        /// <summary>
        /// The property defines the color will be used to draw label shadow of the map geographic objects.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiColorConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("The property defines the color will be used to draw label shadow of the map geographic objects.")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color LabelShadowForeground { get; set; } = Color.FromArgb(255, 251, 251, 251);

        private bool ShouldSerializeLabelShadowForeground()
        {
            return LabelShadowForeground != Color.FromArgb(255, 37, 37, 37);
        }


        /// <summary>
        /// The property defines the color will be used to draw labels of the map geographic objects.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("The property defines the color will be used to draw labels of the map geographic objects.")]
        [TypeConverter(typeof(StiColorConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color LabelForeground { get; set; } = Color.FromArgb(255, 37, 37, 37);

        private bool ShouldSerializeLabelForeground()
        {
            return LabelForeground != Color.FromArgb(180, 251, 251, 251);
        }


        /// <summary>
        /// The property defines the color will be used to fill map bubbles.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiColorConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("The property defines the color will be used to fill map bubbles.")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color BubbleBackColor { get; set; } = Color.Red;

        private bool ShouldSerializeBubbleBackColor()
        {
            return BubbleBackColor != Color.Red;
        }


        /// <summary>
        /// The property defines the color will be used to draw borders of the map bubbles.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiColorConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("The property defines the color will be used to draw borders of the map bubbles.")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color BubbleBorderColor { get; set; } = Color.White;

        private bool ShouldSerializeBubbleBorderColor()
        {
            return BubbleBorderColor != Color.White;
        }
        #endregion

        #region Properties Tooltip
        [StiCategory("ToolTip")]
        [StiSerializable]
        public virtual StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(180, 50, 50, 50));

        private bool ShouldSerializeToolTipBrush()
        {
            return !(ToolTipBrush is StiSolidBrush && ((StiSolidBrush)ToolTipBrush).Color == Color.FromArgb(180, 50, 50, 50));
        }

        [StiCategory("ToolTip")]
        [StiSerializable]
        public virtual StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.White);

        private bool ShouldSerializeToolTipTextBrush()
        {
            return !(ToolTipTextBrush is StiSolidBrush && ((StiSolidBrush)ToolTipTextBrush).Color == Color.White);
        }

        [StiCategory("ToolTip")]
        [StiSerializable]
        public virtual StiCornerRadius ToolTipCornerRadius { get; set; } = new StiCornerRadius();

        private bool ShouldSerializeToolTipCornerRadius()
        {
            return ToolTipCornerRadius == null || !ToolTipCornerRadius.IsDefault;
        }

        [StiCategory("ToolTip")]
        [StiSerializable]
        public virtual StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder();

        private bool ShouldSerializeToolTipBorder()
        {
            return !ToolTipBorder.IsDefault;
        }
        #endregion

        #region Methods.Style
        public override void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            if (paintImage)
                DrawStyleImage(g, rect, StiReportImages.Styles.Map(StiImageSize.Double));

            DrawStyleName(g, rect);
        }

        /// <summary>
        /// Gets a style from the component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void GetStyleFromComponent(StiComponent component, StiStyleElements styleElements)
        {
            if (styleElements != StiStyleElements.All)
                throw new Exception("StiCrossTabStyle support only StiStyleElements.All.");

            var map = component as Maps.StiMap;
            if (map == null) return;

            var style = map.GetComponentStyle() as StiMapStyle;
            if (style == null) return;

            this.Colors = (style.Colors == null)
                ? null
                : style.Colors.Clone() as Color[];
            this.Heatmap = (style.Heatmap == null)
                ? null
                : style.Heatmap.Clone() as StiHeatmapStyleData;
            this.HeatmapWithGroup = (style.HeatmapWithGroup == null)
                ? null
                : style.Heatmap.Clone() as StiHeatmapWithGroupStyleData;

            this.IndividualColor = style.IndividualColor;
            this.DefaultColor = style.DefaultColor;
            this.BackColor = style.BackColor;
            this.BorderSize = style.BorderSize;
            this.BorderColor = style.BorderColor;
            this.LabelShadowForeground = style.LabelShadowForeground;
            this.LabelForeground = style.LabelForeground;
            this.BubbleBackColor = style.BubbleBackColor;
            this.BubbleBorderColor = style.BubbleBorderColor;
        }

        /// <summary>
        /// Sets style to a component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void SetStyleToComponent(StiComponent component)
        {

        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiMapStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        internal StiMapStyle(string name, string description, StiReport report)
            : base(name, description, report)
        {
            Colors = DefaultColors.Clone() as Color[];
        }

        /// <summary>
        /// Creates a new object of the type StiMapStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        public StiMapStyle(string name, string description)
            : this(name, description, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiMapStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        public StiMapStyle(string name)
            : this(name, "")
        {
        }

        /// <summary>
        /// Creates a new object of the type StiMapStyle.
        /// </summary>
        public StiMapStyle()
            : this("")
        {
        }
    }
}
