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
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Components
{
    [StiToolbox(true)]
    [StiServiceBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiMathFormula.png")]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiMathFormulaDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfMathFormulaDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiMathFormulaGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiMathFormulaWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiEngine(StiEngineVersion.EngineV2)]
    [StiV2Builder(typeof(StiMathFormulаV2Builder))]
    public class StiMathFormula :
        StiComponent,
        IStiExportImageExtended,
        IStiFont,
        IStiBorder,
        IStiBrush,
        IStiTextBrush,
        IStiHorAlignment,
        IStiVertAlignment
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyStringNullOrEmpty("LaTexExpression", LaTexExpression);

            jObject.AddPropertyEnum("HorAlignment", HorAlignment, StiHorAlignment.Left);
            jObject.AddPropertyEnum("VertAlignment", VertAlignment, StiVertAlignment.Top);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyFontArial8("Font", Font);
            jObject.AddPropertyBrush("TextBrush", TextBrush);

            if (mode == StiJsonSaveMode.Document)
            {
                jObject.AddPropertyStringNullOrEmpty("Value", Value);
            }

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
                        this.HorAlignment = property.DeserializeEnum<StiHorAlignment>();
                        break;

                    case "VertAlignment":
                        this.VertAlignment = property.DeserializeEnum<StiVertAlignment>();
                        break;

                    case "LaTexExpression":
                        this.LaTexExpression = property.DeserializeString();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "Border":
                        this.Border = property.DeserializeBorder();
                        break;

                    case "Font":
                        this.font = property.DeserializeFont(this.font);
                        break;

                    case "TextBrush":
                        this.TextBrush = property.DeserializeBrush();
                        break;

                    case "Value":
                        this.Value = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiMathFormula;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var helper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            // AppearanceCategory
            var list = new[]
            {
                propHelper.HorAlignment(),
                propHelper.VertAlignment(),
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.Font(),
                    propHelper.TextBrush(),
                };
            helper.Add(StiPropertyCategories.Appearance, list);

            // PositionCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height()
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                };
            }
            helper.Add(StiPropertyCategories.Position, list);
            
            // BehaviorCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                };
            }
            else if (level == StiLevel.Standard)
            {
                list = new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                    propHelper.InteractionEditor(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                    propHelper.InteractionEditor(),
                    propHelper.Printable(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                };
            }
            helper.Add(StiPropertyCategories.Behavior, list);

            // DesignCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.Name()
                };
            }
            else if (level == StiLevel.Standard)
            {
                list = new[]
                {
                    propHelper.Name(),
                    propHelper.Alias()
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.Name(),
                    propHelper.Alias(),
                    propHelper.Restrictions(),
                    propHelper.Locked(),
                    propHelper.Linked()
                };
            }
            helper.Add(StiPropertyCategories.Design, list);

            list = new[]
            {
                propHelper.LaTexExpression()
            };
            helper.Add(StiPropertyCategories.Main, list);

            return helper;
        }
        #endregion

        #region IStiExportImageExtended
        public virtual Image GetImage(ref float zoom)
        {
            return GetImage(ref zoom, StiExportFormat.None);
        }

        public virtual Image GetImage(ref float zoom, StiExportFormat format)
        {
            var painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi);
            return painter.GetImage(this, ref zoom, format);
        }

        [Browsable(false)]
        public override bool IsExportAsImage(StiExportFormat format)
        {
            return true;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties)
        {
            var mathFormula = (StiMathFormula)base.Clone(cloneProperties);

            mathFormula.Border = this.Border?.Clone() as StiBorder;
            mathFormula.Brush = this.Brush?.Clone() as StiBrush;
            mathFormula.TextBrush = this.TextBrush?.Clone() as StiBrush;
            mathFormula.HorAlignment = this.HorAlignment;
            mathFormula.VertAlignment = this.VertAlignment;

            return mathFormula;
        }
        #endregion

        #region IStiHorAlignment
        /// <summary>
        /// Gets or sets the horizontal alignment of an object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiHorAlignment.Left)]
        [StiCategory("Appearance")]
        [Description("Gets or sets the horizontal alignment of an object.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiExpressionEnumConverter))]
        [Editor(StiEditors.ExpressionEnum, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public StiHorAlignment HorAlignment { get; set; } = StiHorAlignment.Left;
        #endregion

        #region IStiVertAlignment
        /// <summary>
        /// Gets or sets the vertical alignment of an object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiVertAlignment.Top)]
        [StiCategory("Appearance")]
        [Description("Gets or sets the vertical alignment of an object.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiExpressionEnumConverter))]
        [Editor(StiEditors.ExpressionEnum, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public StiVertAlignment VertAlignment { get; set; } = StiVertAlignment.Top;
        #endregion

        #region IStiBrush
        /// <summary>
        /// The brush, which is used to draw background.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBrush)]
        [StiSerializable]
        [Description("The brush, which is used to draw background.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiExpressionBrushConverter))]
        [Editor(StiEditors.ExpressionBrush, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public StiBrush Brush { get; set; } = new StiSolidBrush();

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush && ((StiSolidBrush)Brush).Color == Color.Transparent);
        }
        #endregion

        #region IStiBorder
        /// <summary>
        /// Gets or sets border of the component.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiSerializable]
        [Description("Gets or sets border of the component.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBorder Border { get; set; } = new StiBorder();

        private bool ShouldSerializeBorder()
        {
            return Border == null || !Border.IsDefault;
        }
        #endregion

        #region IStiTextBrush
        /// <summary>
        /// The brush of the component, which is used to display text.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceTextBrush)]
        [StiSerializable]
        [Description("The brush of the component, which is used to display text.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiExpressionBrushConverter))]
        [Editor(StiEditors.ExpressionBrush, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public virtual StiBrush TextBrush { get; set; } = new StiSolidBrush(Color.Black);

        private bool ShouldSerializeTextBrush()
        {
            return !(TextBrush is StiSolidBrush && ((StiSolidBrush)TextBrush).Color == Color.Black);
        }
        #endregion

        #region IStiFont
        private Font font = new Font("Arial", 12);
        /// <summary>
        /// Gets or sets font of component.
        /// </summary>
        [StiCategory("Appearance")]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.AppearanceFont)]
        [StiSerializable]
        [Description("Gets or sets font of component.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual Font Font
        {
            get
            {
                return font;
            }
            set
            {
                if (value != null || !IsDesigning)
                    font = value;
            }
        }

        private bool ShouldSerializeFont()
        {
            return !(Font != null && Font.Name == "Arial" && font.Style == FontStyle.Regular && font.Size == 8);
        }
        #endregion

        #region Properties
        private string laTexExpression;
        /// <summary>
        /// Gets or sets size of the border.
        /// </summary>
        [StiCategory("MathFormula")]
        [StiOrder(StiPropertyOrder.MathFormulaLaTexExpression)]
        [StiSerializable(StiSerializeTypes.SerializeToAllExceptDocument)]
        [DefaultValue("")]
        [Description("Gets or sets mathematical expression.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string LaTexExpression
        {
            get
            {
                return laTexExpression;
            }
            set
            {
                if (value != laTexExpression)
                {
                    laTexExpression = value.Trim();
                }
            }
        }

        private string value;
        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.Component;

        public override string LocalizedCategory => StiLocalization.Get("Report", "Components");

        /// <summary>
        /// Gets or sets the default client area of a component.
        /// </summary>
        [Browsable(false)]
        public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 100, 30);

        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType => StiComponentType.Simple;

        public override int ToolboxPosition => (int)StiComponentToolboxPosition.MathFormula;
        
        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiMathFormula");
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/index.html?report_internals_math_formula.htm";
        #endregion

        #region Methods
        public override StiComponent CreateNew()
        {
            return new StiMathFormula();
        }

        public string GetFormulaString()
        {
            try
            {
                if (this.Report == null || this.Report.IsDesigning)
                    return this.LaTexExpression;

                var formula = ReplaceStimulsoftLatexBracket(LaTexExpression);

                formula = formula.Replace("{", "__LP__");
                formula = formula.Replace("}", "__RP__");

                formula = formula.Replace("__LPE__", "{");
                formula = formula.Replace("__RPE__", "}");

                var result = global::System.Convert.ToString(StiParser.ParseTextValue(formula, this));

                if (!string.IsNullOrWhiteSpace(result))
                {
                    formula = result.Replace("__LP__", "{");
                    formula = formula.Replace("__RP__", "}");

                    return formula;
                }
                    
            }
            catch { }

            return LaTexExpression;
        }

        private static string ReplaceStimulsoftLatexBracket(string formula)
        {
            var regex = new Regex(@"(?<beginExpression>\\sti{)(?<expression>.[^{}]*)(?<endExpression>})");
            formula = regex.Replace(formula, m => ProcessReplaceStimulsoftLatexBracket(m.Groups["beginExpression"].Value, m.Groups["expression"].Value, m.Groups["endExpression"].Value));

            return formula;
        }

        private static string ProcessReplaceStimulsoftLatexBracket(string beginExpression, string expression, string endExpression)
        {
            return $"__LPE__{expression}__RPE__";
        }
        #endregion

        /// <summary>
        /// Creates a new StiMathFormula.
        /// </summary>
        public StiMathFormula() : base(RectangleD.Empty)
        {

        }

        /// <summary>
		/// Creates a new StiMathFormula.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiMathFormula(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = true;
        }
    }
}

