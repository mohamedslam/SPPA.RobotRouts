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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Report.Helpers;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Class describes the component - ZipCode.
	/// </summary>
    [StiServiceBitmap(typeof(StiZipCode), "Stimulsoft.Report.Images.Components.StiZipCode.png")]
    [StiGdiPainter(typeof(StiZipCodeGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiZipCodeWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	[StiToolbox(true)]
	[StiContextTool(typeof(IStiShift))]
	[StiContextTool(typeof(IStiCanGrow))]
	[StiContextTool(typeof(IStiGrowToHeight))]
	[StiContextTool(typeof(IStiCanShrink))]
	public class StiZipCode : 
		StiComponent,
		IStiBorder,
		IStiExportImageExtended,
		IStiBrush
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");

            // StiZipCode
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyStringNullOrEmpty("CodeValue", CodeValue);
            jObject.AddPropertyJObject("Code", Code.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetZipCodeEvent", GetZipCodeEvent.SaveToJsonObject(mode));
            jObject.AddPropertyBool("Ratio", Ratio, true);
            jObject.AddPropertyColor("ForeColor", ForeColor, Color.Black);
            jObject.AddPropertyDouble("SpaceRatio", SpaceRatio, 4d);
            jObject.AddPropertyBool("UpperMarks", UpperMarks);
            jObject.AddPropertyDouble("Size", Size, 1d);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "Border":
                        this.Border = property.DeserializeBorder();
                        break;

                    case "CodeValue":
                        this.CodeValue = property.DeserializeString();
                        break;

                    case "Code":
                        {
                            var _expression = new StiZipCodeExpression();
                            _expression.LoadFromJsonObject((JObject)property.Value);
                            this.Code = _expression;
                        }
                        break;

                    case "GetZipCodeEvent":
                        {
                            var _event = new StiGetZipCodeEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetZipCodeEvent = _event;
                        }
                        break;

                    case "Ratio":
                        this.Ratio = property.DeserializeBool();
                        break;

                    case "ForeColor":
                        this.ForeColor = property.DeserializeColor();
                        break;

                    case "SpaceRatio":
                        this.SpaceRatio = property.DeserializeDouble();
                        break;

                    case "UpperMarks":
                        this.UpperMarks = property.DeserializeBool();
                        break;

                    case "Size":
                        this.Size = property.DeserializeDouble();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiZipCode;

	    public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.ZipCode, new StiPropertyObject[]
                {
                    propHelper.Code(),
                    propHelper.Ratio(),
                    propHelper.Size(),
                    propHelper.UpperMarks(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.ZipCode, new StiPropertyObject[]
                {
                    propHelper.Code(),
                    propHelper.Ratio(),
                    propHelper.Size(),
                    propHelper.SpaceRatio(),
                    propHelper.UpperMarks(),
                });
            }
            
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
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.ForeColor(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.ForeColor(),
                    propHelper.UseParentStyles()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                    propHelper.InteractionEditor(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                    propHelper.InteractionEditor(),
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
                        StiPropertyEventId.GetZipCodeEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "User-Manual/report_internals_output_text_parameters_zip_code.htm";
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
			var zipCode = (StiZipCode) base.Clone(cloneProperties);
			
            zipCode.Border = this.Border?.Clone() as StiBorder;
            zipCode.Brush = this.Brush?.Clone() as StiBrush;

            return zipCode;
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

		#region Methods.CalculateRect
		/// <summary>
		/// Calculate Rectangle
		/// </summary>
		public RectangleD CalculateRect(RectangleD rect, int count, int index, out RectangleD markRect)
		{
            double module = 0;
			double width = 0;
			double border = 0;
			double height = 0;
			double x = 0;
			double y = 0;

            if (StiOptions.Engine.UseOldZipCodePaintMode)
            {
                if (!Ratio)
                {
                    width = (5 * rect.Width) / (6 * count - 1);
                    border = width / 5;
                    height = rect.Height;

                    x = rect.X + border * index + width * index;
                    y = rect.Y;
                }
                else
                {
                    border = rect.Height / 10;
                    height = rect.Height;
                    width = height / 2;

                    x = rect.X + border * index + width * index;
                    y = rect.Y;
                }
                markRect = new RectangleD(0, 0, 0, 0);
            }
            else
            {
                if (!Ratio)
                {
                    module = rect.Height / (12 + (UpperMarks ? 3 : 0));
                    double moduleX = rect.Width / ((5 + SpaceRatio) * count - SpaceRatio + 2);
                    width = moduleX * 5;
                    border = moduleX * SpaceRatio;
                    height = module * 10;

                    x = rect.X + moduleX + (width + border) * index;
                    y = rect.Y + module * (UpperMarks ? 4 : 1);

                    markRect = new RectangleD(x - moduleX, y - module * 4, moduleX * 7, module * 2);
                }
                else
                {
                    module = rect.Height / (12 + (UpperMarks ? 3 : 0));
                    height = module * 10;
                    width = module * 5;
                    border = module * SpaceRatio;

                    x = rect.X + module + (width + border) * index;
                    y = rect.Y + module * (UpperMarks ? 4 : 1);

                    markRect = new RectangleD(x - module, y - module * 4, module * 7, module * 2);
                }
            }

			RectangleD rectSpace = new RectangleD(x, y, width, height);
			return rectSpace;
		}
		#endregion		

		#region StiComponent override
		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition => (int)StiComponentToolboxPosition.ZipCode;

	    public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;
        
	    /// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory => StiLocalization.Get("Report", "Components");
        
	    /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiZipCode");
        #endregion

        #region Expressions.ZipCode
	    /// <summary>
		/// Gets or sets the component zip code.
		/// </summary>
		[Browsable(false)]
		[StiSerializable(StiSerializeTypes.SerializeToDocument)]
		[Description("Gets or sets the component zip code.")]
		public string CodeValue { get; set; }

	    /// <summary>
		/// Gets or sets the expression to fill a code of zip code.
		/// </summary>
		[StiCategory("ZipCode")]
		[StiOrder(StiPropertyOrder.ZipCodeCode)]
		[StiSerializable(
			 StiSerializeTypes.SerializeToCode |
			 StiSerializeTypes.SerializeToDesigner |
			 StiSerializeTypes.SerializeToSaveLoad)]
		[Description("Gets or sets the expression to fill a code of zip code.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual StiZipCodeExpression Code
		{
			get
			{
				return new StiZipCodeExpression(this, "Code");
			}
			set
			{
				if (value != null)
				    value.Set(this, "Code", value.Value);
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Invokes all events for this components.
		/// </summary>
		public override void InvokeEvents()
		{
			base.InvokeEvents();
			try
			{
				#region Code
                if (this.Events[EventGetZipCode] != null)
                {
                    var e = new StiValueEventArgs();
                    InvokeGetZipCode(this, e);
                    if (e.Value != null)
                        this.CodeValue = e.Value.ToString();
                }
				#endregion
			}
			catch (Exception e)
			{
				StiLogService.Write(this.GetType(), "DoEvents...ERROR");
				StiLogService.Write(this.GetType(), e);

                if (Report != null)
                    Report.WriteToReportRenderingMessages(this.Name + " " + e.Message);
			}
		}


		#region GetZipCode
		private static readonly object EventGetZipCode = new object();

		/// <summary>
		/// Occurs when getting the code of zipcode.
		/// </summary>
		public event StiValueEventHandler GetZipCode
		{
			add
			{
				base.Events.AddHandler(EventGetZipCode, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventGetZipCode, value);
			}
		}

		/// <summary>
		/// Raises the ZipCode event.
		/// </summary>
		protected virtual void OnGetZipCode(StiValueEventArgs e)
		{
		}

		/// <summary>
		/// Raises the GetZipCode event.
		/// </summary>
		public void InvokeGetZipCode(StiComponent sender, StiValueEventArgs e)
		{
			try
			{
				OnGetZipCode(e);

			    var handler = base.Events[EventGetZipCode] as StiValueEventHandler;
				handler?.Invoke(sender, e);


                StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetZipCodeEvent, e);
            }
			catch (Exception ex)
			{
				var str = $"Expression in ZipCode property of '{Name}' can't be evaluated!";

				StiLogService.Write(GetType(), str);
				StiLogService.Write(GetType(), ex.Message);
				Report.WriteToReportRenderingMessages(str);
			}
		}

		/// <summary>
		/// Occurs when getting the code of zip code.
		/// </summary>
		[StiSerializable]
		[StiCategory("ValueEvents")]
		[Browsable(false)]
		[Description("Occurs when getting the code of zip code.")]
		public StiGetZipCodeEvent GetZipCodeEvent
		{
			get
			{				
				return new StiGetZipCodeEvent(this);
			}
			set
			{
				if (value != null)
				    value.Set(this, value.Script);
			}
		}
		#endregion
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the default client area of a component.
		/// </summary>
		[Browsable(false)]
		public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 200, 32);

	    /// <summary>
		/// Get or sets value, which indicates width and height ratio.
		/// </summary>
		[StiCategory("ZipCode")]
		[StiOrder(StiPropertyOrder.ZipCodeRatio)]
		[StiSerializable()]
		[DefaultValue(true)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Get or sets value, which indicates width and height ratio.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public bool Ratio { get; set; } = true;

	    /// <summary>
		/// Gets or sets a fore color.
		/// </summary>
		[StiCategory("Appearance")]
		[StiOrder(StiPropertyOrder.AppearanceForeColor)]
		[StiSerializable()]
        [TypeConverter(typeof(StiExpressionColorConverter))]
        [Editor(StiEditors.ExpressionColor, typeof(UITypeEditor))]
        [Description("Gets or sets a fore color.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiExpressionAllowed]
        public Color ForeColor { get; set; } = Color.Black;

        private bool ShouldSerializeForeColor()
        {
            return ForeColor != Color.Black;
        }

        /// <summary>
        /// Gets or sets a space ratio.
        /// </summary>
        [StiCategory("ZipCode")]
		[StiOrder(StiPropertyOrder.ZipCodeSpaceRatio)]
		[StiSerializable]
		[DefaultValue(4d)]
        [Description("Gets or sets a space ratio.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public double SpaceRatio { get; set; } = 4d;

	    /// <summary>
        /// Gets or sets a value which indicates whether it is necessary to draw the upper marks.
        /// </summary>
        [StiCategory("ZipCode")]
        [StiOrder(StiPropertyOrder.ZipCodeUpperMarks)]
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether it is necessary to draw the upper marks.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public bool UpperMarks { get; set; }

	    /// <summary>
        /// Gets or sets a contour size.
        /// </summary>
        [StiCategory("ZipCode")]
        [StiOrder(StiPropertyOrder.ZipCodeSize)]
        [StiSerializable()]
        [DefaultValue(1d)]
        [Description("Gets or sets a contour size.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public double Size { get; set; } = 1d;
	    #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiZipCode();
        }
        #endregion

        /// <summary>
		/// Creates a new component of the type StiZipCode.
		/// </summary>
		public StiZipCode() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new component of the type StiZipCode.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiZipCode(RectangleD rect) : base(rect)
		{			
			PlaceOnToolbox = false;
			Code.Value = "1234567890";

            if (StiOptions.Engine.UseOldZipCodePaintMode)
                SpaceRatio = 1;
		}
	}
}