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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Images;
using Stimulsoft.Report.Components.Table;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Brushes = Stimulsoft.Drawing.Brushes;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Brush = Stimulsoft.Drawing.Brush;
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Describes the class that contains a style for components.
    /// </summary>	
    public class StiStyle :
        StiBaseStyle,
        IStiTextFormat,
        IStiFont
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyEnum("HorAlignment", HorAlignment, StiTextHorAlignment.Left);
            jObject.AddPropertyEnum("VertAlignment", VertAlignment, StiVertAlignment.Top);
            jObject.AddPropertyFontArial8("Font", Font);
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyBrush("TextBrush", TextBrush);
            jObject.AddPropertyBrush("NegativeTextBrush", NegativeTextBrush);
            jObject.AddPropertyFloat("LineSpacing", LineSpacing, 1f);

            if (!(TextFormat is StiGeneralFormatService))
                jObject.AddPropertyJObject("TextFormat", TextFormat.SaveToJsonObject(mode));

            jObject.AddPropertyBool("AllowUseHorAlignment", AllowUseHorAlignment);
            jObject.AddPropertyBool("AllowUseVertAlignment", AllowUseVertAlignment);
            jObject.AddPropertyBool("AllowUseImage", AllowUseImage);
            jObject.AddPropertyBool("AllowUseFont", AllowUseFont, true);
            jObject.AddPropertyBool("AllowUseBorderFormatting", AllowUseBorderFormatting, true);
            jObject.AddPropertyBool("AllowUseBorderSides", AllowUseBorderSides, true);
            jObject.AddPropertyBool("AllowUseBorderSidesFromLocation", AllowUseBorderSidesFromLocation);
            jObject.AddPropertyBool("AllowUseBrush", AllowUseBrush, true);
            jObject.AddPropertyBool("AllowUseTextBrush", AllowUseTextBrush, true);
            jObject.AddPropertyBool("AllowUseNegativeTextBrush", AllowUseNegativeTextBrush);
            jObject.AddPropertyBool("AllowUseTextFormat", AllowUseTextFormat);

            jObject.AddPropertyImage("Image", Image);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "HorAlignment":
                        this.HorAlignment = property.DeserializeEnum<StiTextHorAlignment>();
                        break;

                    case "VertAlignment":
                        this.VertAlignment = property.DeserializeEnum<StiVertAlignment>(); 
                        break;

                    case "Font":
                        this.Font = property.DeserializeFont(this.Font);
                        break;

                    case "Border":
                        this.Border = property.DeserializeBorder();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "TextBrush":
                        this.TextBrush = property.DeserializeBrush();
                        break;

                    case "NegativeTextBrush":
                        this.NegativeTextBrush = property.DeserializeBrush();
                        break;

                    case "LineSpacing":
                        this.LineSpacing = property.DeserializeFloat();
                        break;

                    case "TextFormat":
                        this.TextFormat = StiFormatService.CreateFromJsonObject((JObject)property.Value);
                        break;

                    case "AllowUseHorAlignment":
                        this.AllowUseHorAlignment = property.DeserializeBool();
                        break;

                    case "AllowUseVertAlignment":
                        this.AllowUseVertAlignment = property.DeserializeBool();
                        break;

                    case "AllowUseImage":
                        this.AllowUseImage = property.DeserializeBool();
                        break;

                    case "AllowUseFont":
                        this.AllowUseFont = property.DeserializeBool();
                        break;

                    case "AllowUseBorderFormatting":
                        this.AllowUseBorderFormatting = property.DeserializeBool();
                        break;

                    case "AllowUseBorderSides":
                        this.AllowUseBorderSides = property.DeserializeBool();
                        break;

                    case "AllowUseBorderSidesFromLocation":
                        this.AllowUseBorderSidesFromLocation = property.DeserializeBool();
                        break;

                    case "AllowUseBrush":
                        this.AllowUseBrush = property.DeserializeBool();
                        break;

                    case "AllowUseTextBrush":
                        this.AllowUseTextBrush = property.DeserializeBool();
                        break;

                    case "AllowUseNegativeTextBrush":
                        this.AllowUseNegativeTextBrush = property.DeserializeBool();
                        break;

                    case "AllowUseTextFormat":
                        this.AllowUseTextFormat = property.DeserializeBool();
                        break;

                    case "Image":
                        this.Image = property.DeserializeImage();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiStyle;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // MainCategory
            var list = new[] {
                propHelper.StyleName(),
                propHelper.Description(),
                propHelper.StylesCollectionName(),
                propHelper.StyleConditions()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            // AppearanceCategory
            list = new[] {
                propHelper.Border(),
                propHelper.Brush(),
                propHelper.Font(),
                propHelper.HorAlignment(),
                propHelper.SimpleImage(),
                propHelper.LineSpacing(),
                propHelper.NegativeTextBrush(),
                propHelper.TextBrush(),
                propHelper.TextFormat(),
                propHelper.VertAlignment(),
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // ParametersCategory
            list = new[] {
                propHelper.AllowUseBorderFormatting(),
                propHelper.AllowUseBorderSides(),
                propHelper.AllowUseBorderSidesFromLocation(),
                propHelper.AllowUseBrush(),
                propHelper.AllowUseFont(),
                propHelper.AllowUseImage(),
                propHelper.AllowUseNegativeTextBrush(),
                propHelper.AllowUseTextBrush(),
                propHelper.AllowUseTextFormat(),
                propHelper.AllowUseTextOptions(),
                propHelper.AllowUseHorAlignment(),
                propHelper.AllowUseVertAlignment()
            };
            objHelper.Add(StiPropertyCategories.Parameters, list);

            return objHelper;
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var style = base.Clone() as StiStyle;
            style.Font = this.Font.Clone() as Font;
            style.Border = this.Border.Clone() as StiBorder;
            style.Brush = this.Brush.Clone() as StiBrush;
            style.TextBrush = this.TextBrush.Clone() as StiBrush;
            style.TextFormat = this.TextFormat.Clone() as StiFormatService;
            style.NegativeTextBrush = this.NegativeTextBrush.Clone() as StiBrush;

            return style;
        }
        #endregion

        #region IStiTextHorAlignment
        /// <summary>
        /// Gets or sets a horizontal alignment of the style.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiTextHorAlignment.Left)]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets a horizontal alignment of the style.")]
        public virtual StiTextHorAlignment HorAlignment { get; set; } = StiTextHorAlignment.Left;

        private bool ShouldSerializeHorAlignment()
        {
            return HorAlignment != StiTextHorAlignment.Left;
        }

        #endregion

        #region IStiVertAlignment
        /// <summary>
        /// Gets or sets a vertical alignment of the style.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiVertAlignment.Top)]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets a vertical alignment of the style.")]
        public virtual StiVertAlignment VertAlignment { get; set; } = StiVertAlignment.Top;

        private bool ShouldSerializeVertAlignment()
        {
            return VertAlignment != StiVertAlignment.Top;
        }

        #endregion

        #region IStiFont
        private Font font = new Font("Arial", 8);
        /// <summary>
        /// Gets or sets a font for drawing this style.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("Gets or sets a font for drawing this style.")]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                if (font != value)
                {
                    if (value == null)
                        value = new Font("Arial", 8);

                    font = value;
                }
            }
        }

        private bool ShouldSerializeFont()
        {
            return !(
                Font != null &&
                Font.Name == "Arial" &&
                Font.Size == 8 &&
                Font.Style == FontStyle.Regular);
        }
        #endregion

        #region IStiBorder
        /// <summary>
        /// The appearance and behavior of the component border.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("The appearance and behavior of the component border.")]
        public StiBorder Border { get; set; } = new StiBorder();

        private bool ShouldSerializeBorder()
        {
            return Border == null || !Border.IsDefault;
        }
        #endregion

        #region IStiBrush
        /// <summary>
        /// Gets or sets a brush to fill the style.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("Gets or sets a brush to fill the style.")]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.Transparent);

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush && ((StiSolidBrush)Brush).Color == Color.Transparent);
        }
        #endregion

        #region IStiTextBrush
        /// <summary>
        /// The brush of the style, which is used to display text.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("The brush of the style, which is used to display text.")]
        public StiBrush TextBrush { get; set; } = new StiSolidBrush(Color.Black);

        private bool ShouldSerializeTextBrush()
        {
            return !(TextBrush is StiSolidBrush && ((StiSolidBrush)TextBrush).Color == Color.Black);
        }
        #endregion

        #region IStiTextFormat
        /// <summary>
        /// Gets or sets the format of the component.
        /// </summary>
        [Editor("Stimulsoft.Report.Components.TextFormats.Design.StiTextFormatEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Appearance")]
        [Description("Gets or sets the format of the component.")]
        public StiFormatService TextFormat { get; set; } = new StiGeneralFormatService();

        private bool ShouldSerializeTextFormat()
        {
            return !(TextFormat is StiGeneralFormatService);
        }
        #endregion

        #region Properties.Allow
        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use HorAlignment formatting or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use HorAlignment formatting or not.")]
        public bool AllowUseHorAlignment { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use VertAlignment formatting or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use VertAlignment formatting or not.")]
        public bool AllowUseVertAlignment { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use Image formatting or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use Image formatting or not.")]
        public bool AllowUseImage { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use Font formatting or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use Font formatting or not.")]
        public bool AllowUseFont { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use Border formatting or not. 
        /// </summary>
        [StiNonSerialized]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use Border formatting or not.")]
        [Browsable(false)]
        [Obsolete("AllowUserBorder property is obsolete. Please use AllowUseBorderFormatting and AllowUseBorderSides properties instead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool AllowUseBorder
        {
            get
            {
                return AllowUseBorderFormatting & AllowUseBorderSides;
            }
            set
            {
                this.AllowUseBorderFormatting = value;
                this.AllowUseBorderSides = value;
            }
        }

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use Border formatting or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use Border formatting or not.")]
        public bool AllowUseBorderFormatting { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use Border Sides or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use Border Sides or not.")]
        public bool AllowUseBorderSides { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can set border sides of a component depending on the component location.
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can set border sides of a component depending on the component location.")]
        public bool AllowUseBorderSidesFromLocation { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use Brush formatting or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use Brush formatting or not.")]
        public bool AllowUseBrush { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use TextBrush formatting or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use TextBrush formatting or not.")]
        public bool AllowUseTextBrush { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use NegativeTextBrush formatting or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use NegativeTextBrush formatting or not.")]
        public bool AllowUseNegativeTextBrush { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use Text Format. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use Text Format.")]
        public bool AllowUseTextFormat { get; set; }
        #endregion

        #region Methods.Style
        public override void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            var borderRect = rect;

            if (this.AllowUseBrush)
            {
                using (Brush brush = StiBrush.GetBrush(this.Brush, rect))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else
            {
                rect.Width++;
                rect.Height++;
                StiDrawing.FillRectangle(g, StiUX.Background, rect);
                rect.Width--;
                rect.Height--;
            }

            if (!paintValue)
            {
                #region Draw Image
                if (paintImage)
                {
                    DrawStyleImage(g, rect);
                }
                #endregion

                #region Draw Name of Style
                var nameBrush = this.TextBrush;
                var nameColor = StiBrush.ToColor(Brush);

                if (StiUX.IsDark && (nameColor == Color.Transparent || nameColor == Color.Empty))
                    nameBrush = new StiSolidBrush(StiUX.ItemForeground);

                using (var sf = new StringFormat())
                using (var textBrush = StiBrush.GetBrush(nameBrush, rect))
                {
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.FormatFlags = StringFormatFlags.NoWrap;
                    sf.Trimming = StringTrimming.None;

                    #region AllowUseHorAlignment
                    if (this.AllowUseHorAlignment)
                    {
                        switch (this.HorAlignment)
                        {
                            case StiTextHorAlignment.Left:
                                sf.Alignment = StringAlignment.Near;
                                break;

                            case StiTextHorAlignment.Right:
                                sf.Alignment = StringAlignment.Far;
                                break;

                            default:
                                sf.Alignment = StringAlignment.Center;
                                break;
                        }
                    }
                    #endregion

                    #region AllowUseVertAlignment
                    if (this.AllowUseVertAlignment)
                    {
                        switch (this.VertAlignment)
                        {
                            case StiVertAlignment.Top:
                                sf.LineAlignment = StringAlignment.Near;
                                break;

                            case StiVertAlignment.Bottom:
                                sf.LineAlignment = StringAlignment.Far;
                                break;

                            default:
                                sf.LineAlignment = StringAlignment.Center;
                                break;
                        }
                    }
                    #endregion

                    var br = textBrush;
                    if (!this.AllowUseTextBrush)
                        br = Brushes.Black;

                    var str = this.Name;
                    if ((!string.IsNullOrEmpty(this.Description)) && this.Description != this.Name)
                        str = this.Description;

                    var textRect = GetStyleNameRect(borderRect, paintImage);

                    if (this.AllowUseFont)
                    {
                        using (var font = new Font(Font.Name, Font.Size, Font.Style))
                        {
                            g.DrawString(str, font, br, textRect, sf);
                        }
                    }
                    else
                    {
                        using (Font font = new Font("Arial", 8))
                        {
                            g.DrawString(str, font, br, textRect, sf);
                        }
                    }
                }
                #endregion
            }

            if (this.Border != null && this.AllowUseBorderFormatting || this.AllowUseBorderSides)
                this.Border.Draw(g, borderRect, 1, StiUX.Background, this.AllowUseBorderFormatting, this.AllowUseBorderSides);
        }

        public override void DrawStyleImage(Graphics g, Rectangle rect)
        {
            var rectButton = new Rectangle(rect.X + StiScaleUI.I4, rect.Y + rect.Height / 4, (int)(rect.Height * 1), rect.Height / 2);
            var scale = rectButton.Height < StiScale.I(16) ? 0.5f : 1f;

            using (var brA = new SolidBrush(StiColor.Get("4d82b8")))
            //using (var brB = new SolidBrush(StiUX.IsDark ? StiColor.Get("808080") : StiColor.Get("505050")))
            using (var brB = StiBrush.GetBrush(TextBrush, rectButton))
            using (var font = new Font("Arial", 16f * scale, FontStyle.Bold))
            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                rectButton.X -= StiScale.I(6 * scale);
                rectButton.Y -= StiScale.I(4 * scale);
                g.DrawString("A", font, brA, rectButton, sf);

                rectButton.X += StiScale.I(12 * scale);
                rectButton.Y += StiScale.I(8 * scale);
                g.DrawString("A", font, brB, rectButton, sf);
            }
        }

        public override void DrawBox(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            rect.X += StiScale.I1;
            rect.Y += StiScale.I1;
            rect.Width -= StiScale.I2 + 1;
            rect.Height -= StiScale.I2 + 1;

            this.DrawStyle(g, rect, paintValue, paintImage);

            if (this.Border != null && this.Border.Side == StiBorderSides.None && (!paintValue))
            {
                using (Pen pen = new Pen(Color.FromArgb(150, Color.Gray)))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(pen, rect);
                }
            }

            if (rect.Width < StiScale.I20)
            {
                rect.X += StiScale.I4;
                rect.Y += StiScale.I4;
                rect.Width -= StiScale.I8;
                rect.Height -= StiScale.I8;

                using (Brush brush = StiBrush.GetBrush(this.TextBrush, rect))
                {
                    g.FillRectangle(brush, rect);
                }
            }
        }

        /// <summary>
        /// Gets a style from the component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void GetStyleFromComponent(StiComponent component, StiStyleElements styleElements)
        {
            GetStyleFromComponent(component, styleElements, null);
        }

        /// <summary>
        /// Gets a style from the component.
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="styleElements">Elements of style</param>
        /// <param name="componentStyle">Odd/Even/Component style of component, if present</param>
        internal void GetStyleFromComponent(StiComponent component, StiStyleElements styleElements, StiBaseStyle componentStyle)
        {
            var compStyle = componentStyle as StiStyle;

            this.AllowUseBorderFormatting = false;
            this.AllowUseBorderSides = false;
            this.AllowUseBorderSidesFromLocation = false;
            this.AllowUseBrush = false;
            this.AllowUseTextBrush = false;
            this.AllowUseFont = false;
            this.AllowUseHorAlignment = false;
            this.AllowUseVertAlignment = false;
            this.AllowUseTextFormat = false;

            var useComponentStyleProperty = !StiOptions.Engine.UseParentStylesOldMode;
            if (compStyle == null)
                useComponentStyleProperty = false;

            #region LineSpacing
            if (component is StiText)
            {
                var cmp = component as StiText;
                this.LineSpacing = cmp.LineSpacing;
            }
            else if (useComponentStyleProperty)
            {
                this.LineSpacing = compStyle.LineSpacing;
            }
            #endregion

            #region IStiFont
            if ((styleElements & StiStyleElements.Font) > 0)
            {
                if (component is IStiFont)
                {
                    var cmp = component as IStiFont;
                    this.Font = cmp.Font.Clone() as Font;

                    this.AllowUseFont = true;
                }
                else if (useComponentStyleProperty && compStyle.AllowUseFont)
                {
                    this.Font = compStyle.Font.Clone() as Font;
                    this.AllowUseFont = true;
                }
            }
            #endregion

            #region IStiBorder
            if ((styleElements & StiStyleElements.Border) > 0)
            {
                if (component is IStiBorder)
                {
                    var cmp = component as IStiBorder;
                    this.Border = cmp.Border.Clone() as StiBorder;

                    this.AllowUseBorderFormatting = true;
                    this.AllowUseBorderSides = true;
                }
                else if (useComponentStyleProperty && (compStyle.AllowUseBorderSides || compStyle.AllowUseBorderFormatting))
                {
                    this.Border = compStyle.Border.Clone() as StiBorder;
                    this.AllowUseBorderFormatting = true;
                    this.AllowUseBorderSides = true;
                }
            }
            #endregion

            #region IStiBrush
            if ((styleElements & StiStyleElements.Brush) > 0)
            {
                if (component is IStiBrush)
                {
                    var cmp = component as IStiBrush;
                    this.Brush = cmp.Brush.Clone() as StiBrush;

                    this.AllowUseBrush = true;
                }
                else if (useComponentStyleProperty && compStyle.AllowUseBrush)
                {
                    this.Brush = compStyle.Brush.Clone() as StiBrush;
                    this.AllowUseBrush = true;
                }
            }
            #endregion

            #region IStiTextBrush
            if ((styleElements & StiStyleElements.TextBrush) > 0)
            {
                if (component is IStiTextBrush)
                {
                    var cmp = component as IStiTextBrush;
                    this.TextBrush = cmp.TextBrush.Clone() as StiBrush;

                    this.AllowUseTextBrush = true;
                }
                else if (useComponentStyleProperty && compStyle.AllowUseTextBrush)
                {
                    this.TextBrush = compStyle.TextBrush.Clone() as StiBrush;
                    this.AllowUseTextBrush = true;
                }
            }
            #endregion

            #region IStiBackColor
            if (component is IStiBackColor && ((styleElements & StiStyleElements.Brush) > 0))
            {
                var cmp = component as IStiBackColor;
                this.Brush = new StiSolidBrush(cmp.BackColor);

                this.AllowUseBrush = true;
            }
            #endregion

            #region IStiForeColor
            if (component is IStiForeColor && ((styleElements & StiStyleElements.TextBrush) > 0))
            {
                var cmp = component as IStiForeColor;
                this.TextBrush = new StiSolidBrush(cmp.ForeColor);

                this.AllowUseTextBrush = true;
            }
            #endregion

            #region IStiTextHorAlignment
            if ((styleElements & StiStyleElements.HorAlignment) > 0)
            {
                if (component is IStiTextHorAlignment)
                {
                    var cmp = component as IStiTextHorAlignment;
                    this.HorAlignment = cmp.HorAlignment;

                    this.AllowUseHorAlignment = true;
                }
                else if (useComponentStyleProperty && compStyle.AllowUseHorAlignment)
                {
                    this.HorAlignment = compStyle.HorAlignment;
                    this.AllowUseHorAlignment = true;
                }
            }
            #endregion

            #region IStiHorAlignment
            if (component is IStiHorAlignment && ((styleElements & StiStyleElements.HorAlignment) > 0))
            {
                var cmp = component as IStiHorAlignment;
                switch (cmp.HorAlignment)
                {
                    case StiHorAlignment.Center:
                        this.HorAlignment = StiTextHorAlignment.Center;
                        break;

                    case StiHorAlignment.Left:
                        this.HorAlignment = StiTextHorAlignment.Left;
                        break;

                    case StiHorAlignment.Right:
                        this.HorAlignment = StiTextHorAlignment.Right;
                        break;
                }

                this.AllowUseHorAlignment = true;
            }
            #endregion

            #region IStiVertAlignment
            if ((styleElements & StiStyleElements.VertAlignment) > 0)
            {
                if (component is IStiVertAlignment)
                {
                    var cmp = component as IStiVertAlignment;
                    this.VertAlignment = cmp.VertAlignment;
                    this.AllowUseVertAlignment = true;
                }
                else if (useComponentStyleProperty && compStyle.AllowUseVertAlignment)
                {
                    this.VertAlignment = compStyle.VertAlignment;
                    this.AllowUseVertAlignment = true;
                }
            }
            #endregion

            #region Primitives
            var primitive = component as StiLinePrimitive;
            if (primitive != null)
            {
                this.Border = new StiBorder(this.Border.Side, primitive.Color, primitive.Size, primitive.Style);

                this.AllowUseBorderFormatting = true;
                this.AllowUseBorderSides = true;
            }
            #endregion
        }

        /// <summary>
        /// Sets style to a component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void SetStyleToComponent(StiComponent component)
        {
            if (component is Stimulsoft.Report.Chart.StiChart) return;
            if (component is Stimulsoft.Report.CrossTab.StiCrossTab) return;

            var primitive = component as StiLinePrimitive;
            var shape = component as StiShape;

            #region LineSpacing
            if (component is StiText)
            {
                var cmp = component as StiText;
                if (cmp.LineSpacing != this.LineSpacing)
                {
                    cmp.Properties = cmp.Properties.Clone() as StiRepositoryItems;
                    cmp.LineSpacing = this.LineSpacing;
                }
            }
            #endregion

            #region IStiFont
            if (component is IStiFont && this.AllowUseFont)
            {
                var cmp = component as IStiFont;
                cmp.Font = this.Font.Clone() as Font;
            }
            #endregion

            #region IStiBorder
            if (component is IStiBorder && (this.AllowUseBorderFormatting || this.AllowUseBorderSides) && (primitive == null))
            {
                var cmp = component as IStiBorder;

                var sides = cmp.Border.Side;

                if (this.AllowUseBorderFormatting)
                {
                    cmp.Border = this.Border.Clone() as StiBorder;
                    cmp.Border.Side = sides;
                }

                if (this.AllowUseBorderSides)
                {
                    if (this.AllowUseBorderSidesFromLocation && component.Parent != null)
                        cmp.Border.Side = StiStylesHelper.GetBorderSidesFromLocation(component);
                    else
                        cmp.Border.Side = this.Border.Side;
                }
            }
            #endregion

            #region IStiBrush
            if (component is IStiBrush && this.AllowUseBrush)
            {
                var cmp = component as IStiBrush;
                cmp.Brush = this.Brush.Clone() as StiBrush;
            }
            #endregion

            #region Table - Brush
            if (this.AllowUseBrush && component is StiDataBand && component.Name.EndsWith("_DB"))
            {
                var comps = ((StiContainer)component).GetComponents();
                foreach (var comp in comps)
                {
                    if (comp is IStiBrush brush)
                    {
                        brush.Brush = this.Brush.Clone() as StiBrush;
                    }
                }
            }
            #endregion

            #region IStiTextBrush
            if (component is IStiTextBrush && this.AllowUseTextBrush)
            {
                var cmp = component as IStiTextBrush;
                cmp.TextBrush = this.TextBrush.Clone() as StiBrush;
            }
            #endregion

            #region Table - TextBrush
            if (this.AllowUseTextBrush && component is StiDataBand && component.Name.EndsWith("_DB"))
            {
                var comps = ((StiContainer)component).GetComponents();
                foreach (var comp in comps)
                {
                    if (comp is IStiTextBrush textBrush)
                    {
                        textBrush.TextBrush = this.TextBrush.Clone() as StiBrush;
                    }
                }
            }
            #endregion

            #region IStiBackColor
            if (component is IStiBackColor && this.AllowUseBrush)
            {
                var cmp = component as IStiBackColor;
                cmp.BackColor = StiBrush.ToColor(this.Brush);
            }
            #endregion

            #region IStiForeColor
            if (component is IStiForeColor && this.AllowUseTextBrush)
            {
                var cmp = component as IStiForeColor;
                cmp.ForeColor = StiBrush.ToColor(this.TextBrush);
            }
            #endregion

            #region IStiTextHorAlignment
            if (component is IStiTextHorAlignment && this.AllowUseHorAlignment)
            {
                var cmp = component as IStiTextHorAlignment;
                cmp.HorAlignment = this.HorAlignment;
            }
            #endregion

            #region IStiHorAlignment
            if (component is IStiHorAlignment && this.AllowUseHorAlignment)
            {
                var cmp = component as IStiHorAlignment;
                switch (this.HorAlignment)
                {
                    case StiTextHorAlignment.Center:
                        cmp.HorAlignment = StiHorAlignment.Center;
                        break;

                    case StiTextHorAlignment.Left:
                        cmp.HorAlignment = StiHorAlignment.Left;
                        break;

                    case StiTextHorAlignment.Right:
                        cmp.HorAlignment = StiHorAlignment.Right;
                        break;
                }
            }
            #endregion

            #region IStiVertAlignment
            if (component is IStiVertAlignment && this.AllowUseVertAlignment)
            {
                var cmp = component as IStiVertAlignment;
                cmp.VertAlignment = this.VertAlignment;
            }
            #endregion

            #region IStiTextFormat
            if (component is IStiTextFormat && this.AllowUseTextFormat)
            {
                var cmp = component as IStiTextFormat;
                cmp.TextFormat = this.TextFormat.Clone() as StiFormatService;
            }
            #endregion

            #region Image
            if (component is StiImage && this.AllowUseImage)
            {
                var cmp = component as StiImage;
                cmp.PutImage(this.Image);
            }
            #endregion

            #region Primitives
            if (primitive != null && this.AllowUseBorderFormatting)
            {
                primitive.Color = this.Border.Color;
                primitive.Size = (float)this.Border.Size;
                primitive.Style = this.Border.Style;
            }
            #endregion

            #region Shape
            if (shape != null && this.AllowUseBorderFormatting)
            {
                shape.BorderColor = this.Border.Color;
                shape.Size = (float)this.Border.Size;
                shape.Style = this.Border.Style;
            }
            #endregion
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use TextOptions formatting or not. 
        /// </summary>
        [Obsolete("Please do not use this property")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [StiNonSerialized]
        public bool AllowUseTextOptions
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        /// <summary>
        /// Gets or sets a value which indicates on which band in a report this style can be apply when applying style automatically. StyleCode property is obsolete. Do not use it!
        /// </summary>
        [DefaultValue(StiStyleCode.None)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates on which band in a report this style can be apply when applying style automatically. StyleCode property is obsolete. Do not use it!")]
        [Browsable(false)]
        [Obsolete("StyleCode property is obsolete. Do not use it!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiStyleCode StyleCode { get; set; } = StiStyleCode.None;

        /// <summary>
        /// Gets or sets an image to fill the Image property of the Image component.
        /// </summary>
        [Browsable(true)]
        [StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
        [StiCategory("Appearance")]
        [Description("Gets or sets an image to fill the Image property of the Image component.")]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleImageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiSimpeImageConverter))]
        public Image Image { get; set; }

        private bool ShouldSerializeImage()
        {
            return Image != null;
        }

        /// <summary>
        /// Gets or sets a brush to draw the negative values.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("Gets or sets a brush to draw the negative values.")]
        public StiBrush NegativeTextBrush { get; set; } = new StiSolidBrush(Color.Red);

        private bool ShouldSerializeNegativeTextBrush()
        {
            return !(NegativeTextBrush is StiSolidBrush && ((StiSolidBrush)NegativeTextBrush).Color == Color.Red);
        }

        private float lineSpacing = 1f;
        [StiCategory("Appearance")]
        [StiSerializable]
        [DefaultValue(1f)]
        [Description("Gets or sets line spacing of a text.")]
        public virtual float LineSpacing
        {
            get
            {
                return lineSpacing;
            }
            set
            {
                lineSpacing = Math.Max(0.2f, Math.Min(10f, value));
            }
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        internal StiStyle(string name, string description, StiReport report) : base(name, description, report)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        public StiStyle(string name, string description) : this(name, description, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        public StiStyle(string name) : this(name, "")
        {
        }

        /// <summary>
        /// Creates a new object of the type StiStyle.
        /// </summary>
        public StiStyle() : this("")
        {
        }
    }
}