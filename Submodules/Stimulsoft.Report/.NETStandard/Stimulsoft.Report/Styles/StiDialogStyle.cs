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

using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using Stimulsoft.Base;
using Stimulsoft.Base.Dashboard;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
using Brush = Stimulsoft.Drawing.Brush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Describes the class that contains a style for dialog controls.
    /// </summary>	
    public class StiDialogStyle : StiBaseStyle
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyColor(nameof(ForeColor), ForeColor, Color.Black);
            jObject.AddPropertyColor(nameof(BackColor), BackColor, Color.White);
            jObject.AddPropertyColor(nameof(GlyphColor), GlyphColor, Color.DimGray);
            jObject.AddPropertyColor(nameof(SeparatorColor), SeparatorColor, Color.LightGray);
            jObject.AddPropertyColor(nameof(SelectedBackColor), SelectedBackColor, "#3498db");
            jObject.AddPropertyColor(nameof(SelectedForeColor), SelectedForeColor, Color.White);
            jObject.AddPropertyColor(nameof(SelectedGlyphColor), SelectedGlyphColor, Color.White);
            jObject.AddPropertyColor(nameof(HotBackColor), HotBackColor, StiColorUtils.Light(Color.LightGray, 15));
            jObject.AddPropertyColor(nameof(HotForeColor), HotForeColor, StiElementConsts.ForegroundColor);
            jObject.AddPropertyColor(nameof(HotGlyphColor), HotGlyphColor, StiElementConsts.ForegroundColor);
            jObject.AddPropertyColor(nameof(HotSelectedBackColor), HotSelectedBackColor, StiColorUtils.Light(ColorTranslator.FromHtml("#3498db"), 30));
            jObject.AddPropertyColor(nameof(HotSelectedForeColor), HotSelectedForeColor, Color.White);
            jObject.AddPropertyColor(nameof(HotSelectedGlyphColor), HotSelectedGlyphColor, Color.White);
            jObject.AddPropertyBool(nameof(AllowUseFont), AllowUseFont, true);
            jObject.AddPropertyBool(nameof(AllowUseBackColor), AllowUseBackColor, true);
            jObject.AddPropertyBool(nameof(AllowUseForeColor), AllowUseForeColor, true);
            jObject.AddPropertyFontArial8(nameof(Font), Font);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(ForeColor):
                        this.ForeColor = property.DeserializeColor();
                        break;

                    case nameof(BackColor):
                        this.BackColor = property.DeserializeColor();
                        break;

                    case nameof(GlyphColor):
                        this.GlyphColor = property.DeserializeColor();
                        break;

                    case nameof(SeparatorColor):
                        this.SeparatorColor = property.DeserializeColor();
                        break;

                    case nameof(SelectedBackColor):
                        this.SelectedBackColor = property.DeserializeColor();
                        break;

                    case nameof(SelectedForeColor):
                        this.SelectedForeColor = property.DeserializeColor();
                        break;

                    case nameof(SelectedGlyphColor):
                        this.SelectedGlyphColor = property.DeserializeColor();
                        break;

                    case nameof(HotBackColor):
                        this.HotBackColor = property.DeserializeColor();
                        break;

                    case nameof(HotForeColor):
                        this.HotForeColor = property.DeserializeColor();
                        break;

                    case nameof(HotGlyphColor):
                        this.HotGlyphColor = property.DeserializeColor();
                        break;

                    case nameof(HotSelectedBackColor):
                        this.HotSelectedBackColor = property.DeserializeColor();
                        break;

                    case nameof(HotSelectedForeColor):
                        this.HotSelectedForeColor = property.DeserializeColor();
                        break;

                    case nameof(HotSelectedGlyphColor):
                        this.HotSelectedGlyphColor = property.DeserializeColor();
                        break;

                    case nameof(AllowUseFont):
                        this.AllowUseFont = property.DeserializeBool();
                        break;

                    case nameof(AllowUseBackColor):
                        this.AllowUseBackColor = property.DeserializeBool();
                        break;

                    case nameof(AllowUseForeColor):
                        this.AllowUseForeColor = property.DeserializeBool();
                        break;

                    case nameof(Font):
                        this.Font = property.DeserializeFont(this.Font);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDialogStyle;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // MainCategory
            var list = new[]
            {
                propHelper.StyleName(),
                propHelper.Description(),
                propHelper.StylesCollectionName(),
                propHelper.StyleConditions()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Font()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // ParametersCategory
            list = new[]
            {
                propHelper.BackColor(),
                propHelper.ForeColor(),
                propHelper.AllowUseBackColor(),
                propHelper.AllowUseFont(),
                propHelper.AllowUseForeColor()
            };
            objHelper.Add(StiPropertyCategories.Parameters, list);

            return objHelper;
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var style = base.Clone() as StiDialogStyle;
            style.Font = this.Font.Clone() as Font;
            return style;
        }
        #endregion

        #region IStiFont
        private Font font = new Font("Arial", 8);
        /// <summary>
        /// Gets or sets the font for drawing this style.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("Gets or sets the font for drawing this style.")]
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

        #region Properties
        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color ForeColor { get; set; } = Color.Black;

        private bool ShouldSerializeForeColor()
        {
            return ForeColor != Color.Black;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color BackColor { get; set; } = Color.White;

        private bool ShouldSerializeBackColor()
        {
            return BackColor != Color.White;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color GlyphColor { get; set; } = Color.DimGray;

        private bool ShouldSerializeGlyphColor()
        {
            return GlyphColor != Color.DimGray;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color SeparatorColor { get; set; } = Color.LightGray;

        private bool ShouldSerializeSeparatorColor()
        {
            return SeparatorColor != Color.LightGray;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color SelectedBackColor { get; set; } = ColorTranslator.FromHtml("#3498db");

        private bool ShouldSerializeSelectedBackColor()
        {
            return SelectedBackColor != ColorTranslator.FromHtml("#3498db");
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color SelectedForeColor { get; set; } = Color.White;

        private bool ShouldSerializeSelectedForeColor()
        {
            return SelectedForeColor != Color.White;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color SelectedGlyphColor { get; set; } = Color.White;

        private bool ShouldSerializeSelectedGlyphColor()
        {
            return SelectedGlyphColor != Color.White;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color HotBackColor { get; set; } = StiColorUtils.Light(Color.LightGray, 15);

        private bool ShouldSerializeHotBackColor()
        {
            return HotBackColor != StiColorUtils.Light(Color.LightGray, 15);
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color HotForeColor { get; set; } = StiElementConsts.ForegroundColor;

        private bool ShouldSerializeHotForeColor()
        {
            return HotForeColor != StiElementConsts.ForegroundColor;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color HotGlyphColor { get; set; } = StiElementConsts.ForegroundColor;

        private bool ShouldSerializeHotGlyphColor()
        {
            return HotGlyphColor != StiElementConsts.ForegroundColor;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color HotSelectedBackColor { get; set; } = StiColorUtils.Light(ColorTranslator.FromHtml("#3498db"), 30);

        private bool ShouldSerializeHotSelectedBackColor()
        {
            return HotSelectedBackColor != StiColorUtils.Light(ColorTranslator.FromHtml("#3498db"), 30);
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color HotSelectedForeColor { get; set; } = Color.White;

        private bool ShouldSerializeHotSelectedForeColor()
        {
            return HotSelectedForeColor != Color.White;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color HotSelectedGlyphColor { get; set; } = Color.White;

        private bool ShouldSerializeHotSelectedGlyphColor()
        {
            return HotSelectedGlyphColor != Color.White;
        }
        #endregion

        #region Properties.Allow
        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use Font for dialog controls. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use Font for dialog controls.")]
        public bool AllowUseFont { get; set; } = true;

        private bool ShouldSerializeAllowUseFont()
        {
            return AllowUseFont != true;
        }

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use BackColor for dialog controls. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use BackColor for dialog controls.")]
        public bool AllowUseBackColor { get; set; } = true;

        private bool ShouldSerializeAllowUseBackColor()
        {
            return AllowUseBackColor != true;
        }


        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use ForeColor for dialog controls.
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use ForeColor for dialog controls.")]
        public bool AllowUseForeColor { get; set; } = true;

        private bool ShouldSerializeAllowUseForeColor()
        {
            return AllowUseForeColor != true;
        }

        #endregion

        #region Methods.Style
        public override void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            var imageRect = GetStyleImageRect(rect);

            var dist = (int)(imageRect.Height * 0.13f);
            imageRect = new Rectangle(imageRect.X, imageRect.Y + dist, imageRect.Width, imageRect.Height - 2 * dist);
            imageRect.Inflate(-StiScaleUI.I4, -StiScaleUI.I4);

            if (this.AllowUseBackColor && this.SelectedBackColor != Color.Transparent)
                StiDrawing.FillRectangle(g, SelectedBackColor, imageRect);

            g.DrawRectangle(Pens.LightGray, imageRect);

            using (var br = new SolidBrush(this.ForeColor))
            using (var font = new Font("Arial", 7))
            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                g.DrawString("OK", font, br, imageRect, sf);
            }

            DrawStyleName(g, rect);
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
            if (!(component is StiReportControl || component is StiForm)) return;

            var compStyle = componentStyle as StiDialogStyle;
            if (compStyle != null) return;

            var form = component as StiForm;
            var control = component as StiReportControl;

            #region IStiFont
            if ((styleElements & StiStyleElements.Font) > 0)
            {
                if (form != null)
                    this.Font = form.Font.Clone() as Font;
                else
                    this.Font = control.Font.Clone() as Font;

                this.AllowUseFont = true;
            }
            #endregion

            #region IStiBackColor
            if ((styleElements & StiStyleElements.Brush) > 0)
            {
                this.BackColor = form != null ? form.BackColor : control.BackColor;
                this.AllowUseBackColor = true;
            }
            #endregion

            #region IStiForeColor
            if ((styleElements & StiStyleElements.TextBrush) > 0)
                this.AllowUseForeColor = true;
            #endregion

        }

        /// <summary>
        /// Sets style to a component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void SetStyleToComponent(StiComponent component)
        {
            var control = component as StiReportControl;
            if (control == null) return;

            if (!StiStyleConditionHelper.IsAllowStyle(component, this))
                return;

            #region IStiFont
            if (component is IStiFont && this.AllowUseFont)
                control.Font = this.Font.Clone() as Font;
            #endregion

            #region IStiBackColor
            if (this.AllowUseBackColor)
                control.BackColor = this.BackColor;
            #endregion

            #region IStiForeColor
            if (this.AllowUseForeColor)
                control.ForeColor = this.ForeColor;
            #endregion
        }
        #endregion

        /// <summary>
		/// Creates a new object of the type StiStyle.
		/// </summary>
		/// <param name="name">Style name.</param>
		/// <param name="description">Style description.</param>
		internal StiDialogStyle(string name, string description, StiReport report) : base(name, description, report)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        public StiDialogStyle(string name, string description) : this(name, description, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        public StiDialogStyle(string name) : this(name, "")
        {
        }

        /// <summary>
        /// Creates a new object of the type StiStyle.
        /// </summary>
        public StiDialogStyle()
            : this("")
        {
        }
    }
}
