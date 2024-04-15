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
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using System.Drawing.Imaging;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Class describes the component - Check Box.
    /// </summary>
    [StiServiceBitmap(typeof(StiCheckBox), "Stimulsoft.Report.Images.Components.StiCheckBox.png")]
    [StiGdiPainter(typeof(StiCheckBoxGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiCheckBoxWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	[StiToolbox(true)]
	[StiContextTool(typeof(IStiShift))]
	[StiContextTool(typeof(IStiCanGrow))]
	[StiContextTool(typeof(IStiGrowToHeight))]
	[StiContextTool(typeof(IStiCanShrink))]
	[StiContextTool(typeof(IStiEditable))]
	public class StiCheckBox: 
		StiComponent,
		IStiBorder,
		IStiTextBrush,
		IStiEditable,
		IStiBrush,
        IStiBreakable,
		IStiExportImageExtended
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");

            // StiCheckBox
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyBrush("TextBrush", TextBrush);
            jObject.AddPropertyBool("Editable", Editable);
            jObject.AddPropertyJObject("GetCheckedEvent", GetCheckedEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetExcelValueEvent", GetExcelValueEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Checked", Checked.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("ExcelDataValue", ExcelDataValue);
            jObject.AddPropertyJObject("ExcelValue", ExcelValue.SaveToJsonObject(mode));
            jObject.AddPropertyColor("ContourColor", ContourColor, Color.Black);
            jObject.AddPropertyDouble("Size", Size, 1d);
            jObject.AddPropertyString("Values", Values, "true/false");
            jObject.AddPropertyEnum("CheckStyleForTrue", CheckStyleForTrue, StiCheckStyle.Check);
            jObject.AddPropertyEnum("CheckStyleForFalse", CheckStyleForFalse, StiCheckStyle.None);
            if (mode == StiJsonSaveMode.Document && CheckedValue != null)
                jObject.AddPropertyString("CheckedValue", CheckedValue.ToString());

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

                    case "TextBrush":
                        this.TextBrush = property.DeserializeBrush();
                        break;

                    case "Editable":
                        this.Editable = property.DeserializeBool();
                        break;

                    case "GetCheckedEvent":
                        {
                            var _event = new StiGetCheckedEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetCheckedEvent = _event;
                        }
                        break;

                    case "GetExcelValueEvent":
                        {
                            var _event = new StiGetExcelValueEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetExcelValueEvent = _event;
                        }
                        break;

                    case "Checked":
                        {
                            var _expression = new StiCheckedExpression();
                            _expression.LoadFromJsonObject((JObject)property.Value);
                            this.Checked = _expression;
                        }
                        break;

                    case "ExcelDataValue":
                        this.ExcelDataValue = property.DeserializeString();
                        break;

                    case "ExcelValue":
                        {
                            var _expression = new StiExcelValueExpression();
                            _expression.LoadFromJsonObject((JObject)property.Value);
                            this.ExcelValue = _expression;
                        }
                        break;

                    case "ContourColor":
                        this.ContourColor = property.DeserializeColor();
                        break;

                    case "Size":
                        this.Size = property.DeserializeDouble();
                        break;

                    case "Values":
                        this.Values = property.DeserializeString();
                        break;

                    case "CheckStyleForTrue":
                        this.CheckStyleForTrue = property.DeserializeEnum<StiCheckStyle>();
                        break;

                    case "CheckStyleForFalse":
                        this.CheckStyleForFalse = property.DeserializeEnum<StiCheckStyle>();
                        break;

                    case "CheckedValue":
                        this.CheckedValue = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCheckBox;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            // CheckCategory
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Check, new[]
                {
                    propHelper.Checked(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Check, new[]
                {
                    propHelper.Checked(),
                    propHelper.CheckStyleForTrue(),
                    propHelper.CheckStyleForFalse(),
                    propHelper.Editable(),
                    propHelper.Size(),
                    propHelper.CheckBoxValues(),
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
                    propHelper.TextBrush(),
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
                    propHelper.ContourColor(),
                    propHelper.TextBrush(),
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
                    propHelper.ShiftMode()
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
            
            if (level == StiLevel.Professional)
            {
                collection.Add(StiPropertyCategories.Export, new[]
                {
                    propHelper.ExcelValue()
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
                        StiPropertyEventId.GetCheckedEvent,
                        StiPropertyEventId.GetExcelValueEvent,
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "User-Manual/report_internals_creating_lists_check_box.htm";
        #endregion

		#region IStiExportImageExtended
		public virtual Image GetImage(ref float zoom)
		{
			return GetImage(ref zoom, StiExportFormat.None);
		}

		public virtual Image GetImage(ref float zoom, StiExportFormat format)
		{
            StiPainter painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi);
            return painter.GetImage(this, ref zoom, format);
		}

		[Browsable(false)]
		public override bool IsExportAsImage(StiExportFormat format)
		{
			if (format == StiExportFormat.Text) return false;
            if ((format == StiExportFormat.Excel ||
                format == StiExportFormat.ExcelXml ||
                format == StiExportFormat.Excel2007 ||
                format == StiExportFormat.Ods) &&
                !string.IsNullOrEmpty(ExcelDataValue)) return false;
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
        public StiBrush TextBrush { get; set; } = new StiSolidBrush(Color.Black);

        private bool ShouldSerializeTextBrush()
        {
            return !(TextBrush is StiSolidBrush && ((StiSolidBrush)TextBrush).Color == Color.Black);
        }
        #endregion

        #region IStiEditable
        /// <summary>
        /// Gets or sets value indicates that a component can be edited in the window of viewer.
        /// </summary>
        [StiSerializable]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Check")]
		[StiOrder(StiPropertyOrder.CheckEditable)]
		[Description("Gets or sets value indicates that a component can be edited in the window of viewer.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool Editable { get; set; }

        /// <summary>
		/// Saves state of editable value.
		/// </summary>
		string IStiEditable.SaveState()
		{
			return (this.CheckedValue is bool && (bool)this.CheckedValue) ? "true" : "false";
		}

		/// <summary>
		/// Restores state of editable value.
		/// </summary>
		void IStiEditable.RestoreState(string value)
		{
			this.CheckedValue = value == "true";
		}
		#endregion

		#region ICloneable override
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone(bool cloneProperties)
		{
			var checkBox = (StiCheckBox)base.Clone(cloneProperties);
			
			if (this.TextBrush != null)
			    checkBox.TextBrush = (StiBrush)this.TextBrush.Clone();
			else
			    checkBox.TextBrush = null;
		
			return checkBox;
		}
		#endregion

        #region IStiBreakable
        private bool canBreak;
        [Browsable(false)]
        [StiNonSerialized]
        public virtual bool CanBreak
        {
            get
            {
                return canBreak || (this.GrowToHeight && (this.Page != null) && (this.Height / this.Page.PageHeight > 0.5));
            }
            set
            {
                canBreak = value;
            }
        }


        /// <summary>
        /// Divides content of components in two parts. Returns result of dividing. If true, then component is successful divided.
        /// </summary>
        /// <param name="dividedComponent">Component for store part of content.</param>
        /// <returns>If true, then component is successful divided.</returns>
        public bool Break(StiComponent dividedComponent, double devideFactor, ref double divideLine)
        {
            divideLine = 0;
            var result = true;

            var first = (devideFactor > 0.5f) || (Page != null && (this.Height / this.Page.PageHeight > 0.5));
            if (first)
                ((StiCheckBox) dividedComponent).CheckedValue = null;
            else
                this.CheckedValue = null;

            return result;
        }
        #endregion

		#region StiComponent override
		/// <summary>
		/// Return events collection of this component.
		/// </summary>
		public override StiEventsCollection GetEvents()
		{
			var events = base.GetEvents();

		    if (GetCheckedEvent != null)
			    events.Add(GetCheckedEvent);

			return events;
		}

		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition => (int)StiComponentToolboxPosition.CheckBox;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory => StiLocalization.Get("Report", "Components");

        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiCheckBox");
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
				#region GetChecked
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    if (this.Events[EventGetChecked] != null && CheckedValue == null)
                    {
                        var e = new StiValueEventArgs();
                        InvokeGetChecked(this, e);
                        this.CheckedValue = e.Value;
                    }
                }
                else
                {
                    if (((this.Events[EventGetChecked] != null) || !string.IsNullOrEmpty(this.Checked.Value)) && CheckedValue == null)
                    {
                        var e = new StiValueEventArgs();
                        InvokeGetChecked(this, e);
                        this.CheckedValue = e.Value;
                    }
                }
				#endregion

                #region GetExcelValue
                if (this.Events[EventGetExcelValue] != null && this.ExcelDataValue == null)
                {
                    var e = new StiGetExcelValueEventArgs();
                    InvokeGetExcelValue(this, e);
                    if (e.Value != null)
                        this.ExcelDataValue = e.Value;
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

		#region GetChecked
        private static readonly object EventGetChecked = new object();

		/// <summary>
		/// Occurs when state is being checked.
		/// </summary>
		public event StiValueEventHandler GetChecked
		{
			add
			{
                Events.AddHandler(EventGetChecked, value);
			}
			remove
			{
                Events.RemoveHandler(EventGetChecked, value);
			}
		}

		/// <summary>
		/// Raises the GetChecked event.
		/// </summary>
		protected virtual void OnGetChecked(StiValueEventArgs e)
		{
		}

		/// <summary>
		/// Raises the GetChecked event.
		/// </summary>
		public void InvokeGetChecked(StiComponent sender, StiValueEventArgs e)
		{
			try
			{
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    OnGetChecked(e);
                    StiValueEventHandler handler = Events[EventGetChecked] as StiValueEventHandler;
                    if (handler != null) handler(sender, e);
                }
                else
                {
                    OnGetChecked(e);

                    if (!string.IsNullOrEmpty(this.Checked.Value))
                    {
                        object parserResult = StiParser.ParseTextValue(this.Checked.Value, this, sender);
                        if (parserResult != null) e.Value = parserResult;
                    }

                    StiValueEventHandler handler = Events[EventGetChecked] as StiValueEventHandler;
                    if (handler != null) handler(sender, e);

                    StiBlocklyHelper.InvokeBlockly(this.Report, this, GetCheckedEvent, e);
                }
			}
			catch (Exception ex)
			{
				string str = $"Expression in Checked property of '{Name}' can't be evaluated!";
				StiLogService.Write(this.GetType(), str);
				StiLogService.Write(this.GetType(), ex.Message);
				Report.WriteToReportRenderingMessages(str);
			}
		}

		/// <summary>
		/// Occurs when state is being checked.
		/// </summary>
		[StiSerializable]
		[StiCategory("ValueEvents")]
		[Browsable(false)]
		[Description("Occurs when state is being checked.")]
		public StiGetCheckedEvent GetCheckedEvent
		{
			get
			{				
				return new StiGetCheckedEvent(this);
			}
			set
			{
				if (value != null)
				    value.Set(this, value.Script);
			}
		}
		#endregion

        #region GetExcelValue
        private static readonly object EventGetExcelValue = new object();

        /// <summary>
        /// Occurs when the ExcelValue is calculated.
        /// </summary>
        public event StiGetExcelValueEventHandler GetExcelValue
        {
            add
            {
                Events.AddHandler(EventGetExcelValue, value);
            }
            remove
            {
                Events.RemoveHandler(EventGetExcelValue, value);
            }
        }

        /// <summary>
        /// Raises the GetExcelValue event.
        /// </summary>
        protected virtual void OnGetExcelValue(StiGetExcelValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetExcelValue event.
        /// </summary>
        public virtual void InvokeGetExcelValue(StiComponent sender, StiGetExcelValueEventArgs e)
        {
            try
            {
                OnGetExcelValue(e);

                var handler = Events[EventGetExcelValue] as StiGetExcelValueEventHandler;
                if (handler != null)
                    handler(sender, e);

                StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetExcelValueEvent, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in ExcelValue property of '{Name}' can't be evaluated!";
                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);
                Report.WriteToReportRenderingMessages(str);
            }
        }


        /// <summary>
        /// Occurs when the ExcelValue is calculated.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when the ExcelValue is calculated.")]
        public StiGetExcelValueEvent GetExcelValueEvent
        {
            get
            {
                return new StiGetExcelValueEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion
        #endregion

		#region Expression
		#region Checked
        /// <summary>
		/// Gets or sets checked value.
		/// </summary>
		[Browsable(false)]
		[Description("Gets or sets checked value.")]
		[StiSerializable(StiSerializeTypes.SerializeToDocument)]
		public object CheckedValue { get; set; }

        /// <summary>
        /// Gets or sets an expression which used to calculate check state.
		/// </summary>
		[StiCategory("Check")]
		[StiOrder(StiPropertyOrder.CheckChecked)]
		[StiSerializable]
		[Description("Gets or sets an expression which used to calculate check state.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public StiCheckedExpression Checked
		{
			get
			{
				return new StiCheckedExpression(this, "Checked");
			}
			set
			{
				if (value != null)
				    value.Set(this, "Checked", value.Value);
			}
		}
		#endregion

        #region ExcelValue
        /// <summary>
        /// Gets or sets excel data value.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets excel data value.")]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]        
        public string ExcelDataValue { get; set; }

        /// <summary>
        /// Gets or sets an expression used for export data to Excel.
        /// </summary>
        [StiCategory("Export")]
        [StiOrder(StiPropertyOrder.ExportExcelValue)]
        [StiSerializable]
        [Description("Gets or sets an expression used for export data to Excel.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual StiExcelValueExpression ExcelValue
        {
            get
            {
                return new StiExcelValueExpression(this, "ExcelValue");
            }
            set
            {
                if (value != null)
                    value.Set(this, "ExcelValue", value.Value);
            }
        }
        #endregion
		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiCheckBox();
        }
        #endregion

		#region Methods
		public bool IsChecked()
		{
			string []strs = null;

            if (Values.IndexOf('/') != -1)
                strs = Values.Split('/');

            else if (Values.IndexOf(';') != -1)
                strs = Values.Split(';');

            else if (Values.IndexOf(',') != -1)
                strs = Values.Split(',');

			if (strs.Length == 0)return false;
			
			var value = strs[0].ToLowerInvariant().Trim();

		    if (value.Length == 0)
			    return false;

			if (CheckedValue == null)
			    return false;

			var checkedValueStr = CheckedValue.ToString().ToLowerInvariant().Trim();
			return checkedValueStr == value;
		}
        
		public bool IsUnchecked()
		{
			string []strs = null;

            if (Values.IndexOf('/') != -1)
                strs = Values.Split('/');

            else if (Values.IndexOf(';') != -1)
                strs = Values.Split(';');

            else if (Values.IndexOf(',') != -1)
                strs = Values.Split(',');

			if (strs.Length < 2)
			    return false;
			
			var value = strs[1].ToLowerInvariant().Trim();

			if (value.Length == 0)
			    return false;

			if (CheckedValue == null)
			    return false;

			var checkedValueStr = CheckedValue.ToString().ToLowerInvariant().Trim();
			return checkedValueStr == value;
		}
        
        public Metafile GetMetafile()
        {
            Metafile mf = null;
            try
            {
                mf = GetMetafile1();
            }
            catch
            {
                //fix - trying to get a picture again, often it helps
                mf = GetMetafile1();
            }
            return mf;
        }

		private Metafile GetMetafile1()
		{
			using (var bmp = new Bitmap(1, 1))
			using (var graph = Graphics.FromImage(bmp))
			{
				var ptrGraph = graph.GetHdc();
				var newImage = new Metafile(ptrGraph, EmfType.EmfOnly);

				graph.ReleaseHdc(ptrGraph);

				using (var imageGraph = Graphics.FromImage(newImage))
				{
					var rect = GetPaintRectangle(true, false);
					rect.X = 0;
					rect.Y = 0;

					StiDrawing.FillRectangle(imageGraph, Brush, rect);
                    StiCheckBoxGdiPainter.PaintCheck(this, imageGraph, rect, 1);
				}
				return newImage;
			}
		}
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a contour color.
        /// </summary>
        [StiCategory("Appearance")]
		[StiOrder(StiPropertyOrder.AppearanceContourColor)]
		[StiSerializable()]
        [TypeConverter(typeof(StiExpressionColorConverter))]
        [Editor(StiEditors.ExpressionColor, typeof(UITypeEditor))]
        [Description("Gets or sets a contour color.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiExpressionAllowed]
        public Color ContourColor { get; set; } = Color.Black;

        private bool ShouldSerializeContourColor()
        {
            return ContourColor != Color.Black;
        }

        /// <summary>
		/// Gets or sets a contour size.
		/// </summary>
		[StiCategory("Check")]
		[StiOrder(StiPropertyOrder.CheckSize)]
		[StiSerializable()]
		[DefaultValue(1d)]
		[Description("Gets or sets a contour size.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public double Size { get; set; } = 1d;

        private StiCheckStyle checkStyle = StiCheckStyle.Check;
		/// <summary>
		/// This property is obsoleted. Please use properties CheckStyleForTrue and CheckStyleForFalse.
		/// </summary>
		[StiNonSerialized]
		[Browsable(false)]
		[Editor("Stimulsoft.Report.Components.Design.StiCheckStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Obsolete("This property is obsoleted. Please use properties CheckStyleForTrue and CheckStyleForFalse.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [StiPropertyLevel(StiLevel.Standard)]
		public StiCheckStyle CheckStyle
		{
			get 
			{
				return CheckStyleForTrue;
			}
			set 
			{
				if (checkStyle == value)
				{
					CheckStyleForTrue = value;
					CheckStyleForFalse = StiCheckStyle.None;
				}
			}
		}

        /// <summary>
		/// Gets or sets string which describes true and false values.
		/// </summary>
		[StiCategory("Check")]
		[StiOrder(StiPropertyOrder.CheckValues)]
		[StiSerializable]
		[DefaultValue("true/false")]
		[Description("Gets or sets string which describes true and false values.")]
		[Editor("Stimulsoft.Report.Components.Design.StiCheckValuesEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
		public string Values { get; set; } = "true/false";

        /// <summary>
		/// Gets or sets check style for true value.
		/// </summary>
		[StiCategory("Check")]
		[StiOrder(StiPropertyOrder.CheckCheckStyleForTrue)]
		[StiSerializable]
		[DefaultValue(StiCheckStyle.Check)]
		[Editor("Stimulsoft.Report.Components.Design.StiCheckStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[TypeConverter(typeof(StiEnumConverter))]
		[Description("Gets or sets check style for true value.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public StiCheckStyle CheckStyleForTrue { get; set; } = StiCheckStyle.Check;

        /// <summary>
		/// Gets or sets check style for false value.
		/// </summary>
		[StiCategory("Check")]
		[StiOrder(StiPropertyOrder.CheckCheckStyleForFalse)]
		[StiSerializable]
		[DefaultValue(StiCheckStyle.None)]
		[Editor("Stimulsoft.Report.Components.Design.StiCheckStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[TypeConverter(typeof(StiEnumConverter))]
		[Description("Gets or sets check style for false value.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public StiCheckStyle CheckStyleForFalse { get; set; } = StiCheckStyle.None;
        #endregion

        /// <summary>
		/// Creates a new component of the type StiCheckBox.
		/// </summary>
		public StiCheckBox() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new component of the type StiCheckBox.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiCheckBox(RectangleD rect) : base(rect)
		{			
			PlaceOnToolbox = false;
		}
	}
}