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
using Stimulsoft.Report.Components.ShapeTypes;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.Units;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
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
    /// <summary>
    /// Class describes the component that serves to show shapes.
    /// </summary>
    [StiServiceBitmap(typeof(StiShape), "Stimulsoft.Report.Images.Components.StiShape.png")]
    [StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiShape.png")]
    [StiGdiPainter(typeof(StiShapeGdiPainter))]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiShapeDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiShapeWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfShapeDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
	[StiToolbox(true)]
	[StiContextTool(typeof(IStiShift))]
	[StiContextTool(typeof(IStiGrowToHeight))]
	public class StiShape : 
		StiComponent,
		IStiBrush,
		IStiBorderColor,
        IStiForeColor,
        IStiFont,
        IStiTextHorAlignment,
        IStiVertAlignment,
        IStiExportImageExtended,
        IStiShape
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");

            // StiShape
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyColor("BorderColor", BorderColor, Color.Black);
            jObject.AddPropertyEnum("Style", Style, StiPenStyle.Solid);
            jObject.AddPropertyFloat("Size", Size, 1f);
            jObject.AddPropertyJObject("ShapeType", ShapeType.SaveToJsonObject(mode));

            jObject.AddPropertyString("Text", Text);
            jObject.AddPropertyColor("ForeColor", ForeColor, Color.Black);
            jObject.AddPropertyFontArial8("Font", Font);
            jObject.AddPropertyEnum("HorAlignment", HorAlignment, StiTextHorAlignment.Center);
            jObject.AddPropertyEnum("VertAlignment", VertAlignment, StiVertAlignment.Center);
            jObject.AddPropertyJObject("Margins", Margins.SaveToJsonObject(0, 0, 0, 0));
            jObject.AddPropertyColor("BackgroundColor", BackgroundColor, Color.Transparent);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Text":
                        this.Text = property.DeserializeString();
                        break;

                    case "ForeColor":
                        this.ForeColor = property.DeserializeColor();
                        break;

                    case "BackgroundColor":
                        this.BackgroundColor = property.DeserializeColor();
                        break;

                    case "Font":
                        this.font = property.DeserializeFont(this.font);
                        break;

                    case "HorAlignment":
                        this.HorAlignment = property.DeserializeEnum<StiTextHorAlignment>();
                        break;

                    case "VertAlignment":
                        this.VertAlignment = property.DeserializeEnum<StiVertAlignment>();
                        break;

                    case "Margins":
                        {
                            var margins = new StiMargins();
                            margins.LoadFromJsonObject((JObject)property.Value);

                            this.Margins = margins;
                        }
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "BorderColor":
                        this.BorderColor = property.DeserializeColor();
                        break;

                    case "Style":
                        this.Style = property.DeserializeEnum<StiPenStyle>();
                        break;

                    case "Size":
                        this.size = property.DeserializeFloat();
                        break;

                    case "ShapeType":
                        this.ShapeType = StiShapeTypeService.CreateFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiShape;

	    public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.ShapeEditor()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Text, new[]
                {
                    propHelper.TextStr(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Text, new[]
                {
                    propHelper.TextStr(),
                    propHelper.Font(),
                    propHelper.Margins(),
                    propHelper.ForeColor(),
                    propHelper.BackgroundColor(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment(),
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.UseParentStyles()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.GrowToHeight(),
                    propHelper.Enabled()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.GrowToHeight(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.GrowToHeight(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.Printable(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias(),
                    propHelper.Restrictions(),
                    propHelper.Locked(),
                    propHelper.Linked()
                });
            }

            return collection;
        }

        public override StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return new StiEventCollection
            {
                {
                    StiPropertyCategories.MouseEvents,
                    new[]
                    {
                        StiPropertyEventId.ClickEvent,
                        StiPropertyEventId.DoubleClickEvent,
                        StiPropertyEventId.MouseEnterEvent,
                        StiPropertyEventId.MouseLeaveEvent
                    }
                },
                {
                    StiPropertyCategories.NavigationEvents,
                    new[]
                    {
                        StiPropertyEventId.GetBookmarkEvent,
                        StiPropertyEventId.GetDrillDownReportEvent,
                        StiPropertyEventId.GetHyperlinkEvent,
                        StiPropertyEventId.GetPointerEvent,
                    }
                },
                {
                    StiPropertyCategories.PrintEvents,
                    new[]
                    {
                        StiPropertyEventId.AfterPrintEvent,
                        StiPropertyEventId.BeforePrintEvent,
                    }
                },
                {
                    StiPropertyCategories.ValueEvents,
                    new[]
                    {
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties

        public override string HelpUrl => "user-manual/report_internals_primitives.htm?zoom_highlightsub=Shape";
	    #endregion

		#region IStiCanShrink override
		[Browsable(false)]
		[StiNonSerialized]
		public override bool CanShrink
		{
			get
			{
				return base.CanShrink;
			}
			set
			{
			}
		}
		#endregion

		#region IStiCanGrow override
		[Browsable(false)]
		[StiNonSerialized]
		public override bool CanGrow
		{
			get
			{
				return base.CanGrow;
			}
			set
			{
			}
		}
		#endregion

		#region ICloneable override
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone(bool cloneProperties)
		{
			var shape =	(StiShape)base.Clone(cloneProperties);
			
			if (this.ShapeType != null)
			    shape.ShapeType = (StiShapeTypeService)this.ShapeType.Clone();
			else
			    shape.ShapeType = null;

            if (this.Brush != null)
                shape.Brush = (StiBrush)this.Brush.Clone();
            else
                shape.Brush = null;

            return shape;
		}
		#endregion

        #region IStiUnitConvert
        /// <summary>
        /// Converts a component out of one unit into another.
        /// </summary>
        /// <param name="oldUnit">Old units.</param>
        /// <param name="newUnit">New units.</param>
        public override void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
        {
            base.Convert(oldUnit, newUnit, isReportSnapshot);

            if (ShapeType is StiOctagonShapeType)
                (ShapeType as StiOctagonShapeType).Bevel = (float) newUnit.ConvertFromHInches(oldUnit.ConvertToHInches((ShapeType as StiOctagonShapeType).Bevel));
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
        public virtual StiBrush Brush { get; set; } = new StiSolidBrush();
	    #endregion

		#region IStiBorderColor
	    /// <summary>
		/// Gets or sets border color.
		/// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color BorderColor { get; set; } = Color.Black;
        #endregion

        #region Text
        /// <summary>
        /// Gets or sets text.
        /// </summary>
        [StiCategory("Text")]
        [StiOrder(StiPropertyOrder.ShapeText)]
        [DefaultValue(null)]
        [StiSerializable]
        [Description("Gets or sets text.")]
        [Editor("Stimulsoft.Report.Components.Design.StiExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual string Text { get; set; }
        #endregion

        #region IStiFont
        private Font font = new Font("Arial", 8);
        /// <summary>
        /// Gets or sets font of component.
        /// </summary>
        [StiCategory("Text")]
        [Editor("Stimulsoft.Report.Design.Components.StiFontEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.ShapeFont)]
        [StiSerializable]
        [Description("Gets or sets font of component.")]
        [StiPropertyLevel(StiLevel.Standard)]
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

        #region IStiTextHorAlignment
        /// <summary>
        /// Gets or sets the text horizontal alignment.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiTextHorAlignment.Center)]
        [StiCategory("Text")]
        [StiOrder(StiPropertyOrder.ShapeHorAlignment)]
        [Description("Gets or sets the text horizontal alignment.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [TypeConverter(typeof(StiExpressionEnumConverter))]
        [Editor(StiEditors.ExpressionEnum, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public virtual StiTextHorAlignment HorAlignment { get; set; } = StiTextHorAlignment.Center;
        #endregion

        #region IStiVertAlignment
        /// <summary>
        /// Gets or sets the vertical alignment of an object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiVertAlignment.Center)]
        [StiCategory("Text")]
        [StiOrder(StiPropertyOrder.ShapeVertAlignment)]
        [Description("Gets or sets the vertical alignment of an object.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [TypeConverter(typeof(StiExpressionEnumConverter))]
        [Editor(StiEditors.ExpressionEnum, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public virtual StiVertAlignment VertAlignment { get; set; } = StiVertAlignment.Center;
        #endregion

        #region IStiForeColor
        /// <summary>
        /// Gets or sets a brush to draw text.
        /// </summary>
        [StiCategory("Text")]
        [StiOrder(StiPropertyOrder.ShapeBrush)]
        [StiSerializable]
        [Description("Gets or sets a color to draw text.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [TypeConverter(typeof(StiExpressionColorConverter))]
        [Editor(StiEditors.ExpressionColor, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public virtual Color ForeColor { get; set; } = Color.Black;

        private bool ShouldSerializeForeColor()
        {
            return ForeColor != Color.Black;
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.Shape;

	    public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Shapes;

	    /// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory => StiLocalization.Get("Report", "Shapes");

	    /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiShape");
        #endregion

        #region Methods
        internal string GetParsedText()
        {
            if (string.IsNullOrEmpty(Text) || !Text.Contains("{")) return this.Text;

            return StiReportParser.Parse(this.Text, this);
        } 
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiShape();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text background.
        /// </summary>
        [StiCategory("Text")]
        [StiOrder(StiPropertyOrder.ShapeBackgroundColor)]
        [StiSerializable]
        [Description("Gets or sets the text background.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [TypeConverter(typeof(StiExpressionColorConverter))]
        [Editor(StiEditors.ExpressionColor, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public virtual Color BackgroundColor { get; set; } = Color.Transparent;

        private bool ShouldSerializeBackgroundColor()
        {
            return BackgroundColor != Color.Transparent;
        }

        /// <summary>
        /// Gets or sets the default client area of a component.
        /// </summary>
        [Browsable(false)]
		public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 64, 64);

        /// <summary>
        /// Gets or sets text margins.
        /// </summary>
        [StiSerializable]
        [StiCategory("Text")]
        [StiOrder(StiPropertyOrder.ShapeMargins)]
        [Description("Gets or sets text margins.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiMargins Margins { get; set; } = new StiMargins();

        private bool ShouldSerializeMargins()
        {
            return !Margins.IsDefault;
        }

        /// <summary>
        /// Gets or sets a pen style.
        /// </summary>
        [Editor("Stimulsoft.Base.Drawing.Design.StiPenStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[DefaultValue(StiPenStyle.Solid)]
		[StiCategory("Shape")]
		[StiOrder(StiPropertyOrder.ShapeStyle)]
		[TypeConverter(typeof(StiEnumConverter))]
		[Description("Gets or sets a pen style.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public StiPenStyle Style { get; set; } = StiPenStyle.Solid;


	    private float size = 1f;
		/// <summary>
		/// Gets or sets size of the border.
		/// </summary>
		[StiCategory("Shape")]
		[StiOrder(StiPropertyOrder.ShapeSize)]
		[StiSerializable]
		[DefaultValue(1f)]
        [Description("Gets or sets size of the border.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public float Size
		{
			get 
			{
				return size;
			}
			set 
			{
                if (value != size)
                {
                    if (value < 1)
                        size = 1;
                    else
                        size = value;
                }
			}
		}

        /// <summary>
        /// Gets or sets type of the shape.
        /// </summary>		
        [StiCategory("Shape")]
        [StiOrder(StiPropertyOrder.ShapeShapeType)]
        [StiSerializable(StiSerializationVisibility.Class)]
        [Editor("Stimulsoft.Report.Components.ShapeTypes.Design.StiShapeTypeServiceEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets type of the shape.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [RefreshProperties(RefreshProperties.All)]
        public StiShapeTypeService ShapeType { get; set; } = new StiRectangleShapeType();
        #endregion

        /// <summary>
        /// Creates a new component of the type StiShape.
        /// </summary>
        public StiShape() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new component of the type StiShape.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiShape(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = true;
		}
	}
}