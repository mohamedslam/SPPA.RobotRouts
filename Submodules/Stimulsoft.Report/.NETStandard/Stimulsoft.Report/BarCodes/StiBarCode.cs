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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.BarCodes
{
    [StiServiceBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiBarCode.png")]
    [StiGdiPainter(typeof(StiBarCodeGdiPainter))]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiBarCodeDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiBarCodeWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfBarCodeDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiToolbox(true)]
    [StiContextTool(typeof(IStiShift))]
    [StiContextTool(typeof(IStiGrowToHeight))]
    public class StiBarCode :
        StiComponent,
        IStiBarCode,
        IStiExportImageExtended,
        IStiVertAlignment,
        IStiHorAlignment,
        IStiEnumAngle,
        IStiBorder
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // NonSerialized
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");

            // StiBarCode
            jObject.AddPropertyEnum("Angle", Angle, StiAngle.Angle0);
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyColor("ForeColor", ForeColor, Color.Black);
            jObject.AddPropertyColor("BackColor", BackColor, Color.White);
            jObject.AddPropertyBool("AutoScale", AutoScale);
            jObject.AddPropertyBool("ShowLabelText", ShowLabelText, true);
            jObject.AddPropertyBool("ShowQuietZones", ShowQuietZones, true);
            jObject.AddPropertyFontArial8BoldPixel("Font", Font);

            jObject.AddPropertyEnum("HorAlignment", HorAlignment, StiHorAlignment.Left);
            jObject.AddPropertyEnum("VertAlignment", VertAlignment, StiVertAlignment.Top);
            jObject.AddPropertyJObject("GetBarCodeEvent", GetBarCodeEvent.SaveToJsonObject(mode));

            jObject.AddPropertyJObject("BarCodeType", BarCodeType.SaveToJsonObject(mode));

            if (mode == StiJsonSaveMode.Document)
            {
                jObject.AddPropertyStringNullOrEmpty("CodeValue", CodeValue);
            }
            else
            {
                jObject.AddPropertyJObject("Code", Code.SaveToJsonObject(mode));
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
                    case "Angle":
                        this.Angle = property.DeserializeEnum<StiAngle>();
                        break;

                    case "Border":
                        this.Border = property.DeserializeBorder();
                        break;

                    case "ForeColor":
                        this.ForeColor = property.DeserializeColor();
                        break;

                    case "BackColor":
                        this.BackColor = property.DeserializeColor();
                        break;

                    case "AutoScale":
                        this.AutoScale = property.DeserializeBool();
                        break;

                    case "ShowLabelText":
                        this.ShowLabelText = property.DeserializeBool();
                        break;

                    case "ShowQuietZones":
                        this.ShowQuietZones = property.DeserializeBool();
                        break;

                    case "Font":
                        this.font = property.DeserializeFont(this.Font);
                        break;

                    case "HorAlignment":
                        this.HorAlignment = property.DeserializeEnum<StiHorAlignment>();
                        break;

                    case "VertAlignment":
                        this.VertAlignment = property.DeserializeEnum<StiVertAlignment>();
                        break;

                    case "CodeValue":
                        this.CodeValue = property.DeserializeString();
                        break;

                    case "Code":
                        {
                            var exp = new StiBarCodeExpression();
                            exp.LoadFromJsonObject((JObject)property.Value);
                            this.Code = exp;
                        }
                        break;

                    case "GetBarCodeEvent":
                        {
                            var _event = new StiGetBarCodeEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetBarCodeEvent = _event;
                        }
                        break;

                    case "BarCodeType":
                        {
                            this.BarCodeType = StiBarCodeTypeService.CreateFromJsonObject((JObject)property.Value);
                        }
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiBarCode;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var collection = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.BarCodeEditor(),
            });

            switch (level)
            {
                case StiLevel.Basic:
                    collection.Add(StiPropertyCategories.BarCodeAdditional, new[]
                    {
                        propHelper.BarCodeAngle()
                    });
                    break;

                case StiLevel.Standard:
                    collection.Add(StiPropertyCategories.BarCodeAdditional, new[]
                    {
                        propHelper.BarCodeAngle(),
                        propHelper.AutoScale(),
                        propHelper.HorAlignment(),
                        propHelper.VertAlignment(),
                        propHelper.ShowLabelText(),
                    });
                    break;

                case StiLevel.Professional:
                    collection.Add(StiPropertyCategories.BarCodeAdditional, new[]
                    {
                        propHelper.BarCodeAngle(),
                        propHelper.AutoScale(),
                        propHelper.HorAlignment(),
                        propHelper.VertAlignment(),
                        propHelper.ShowLabelText(),
                        propHelper.ShowQuickButtons()
                    });
                    break;
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
                    propHelper.BackColor(),
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
                    propHelper.BackColor(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.Font(),
                    propHelper.ForeColor(),
                    propHelper.UseParentStyles(),
                });
            }

            switch (level)
            {
                case StiLevel.Basic:
                    collection.Add(StiPropertyCategories.Behavior, new[]
                    {
                        propHelper.Enabled(),
                        propHelper.GrowToHeight(),
                    });
                    break;

                case StiLevel.Standard:
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
                    break;

                case StiLevel.Professional:
                    collection.Add(StiPropertyCategories.Behavior, new[]
                    {
                        propHelper.AnchorMode(),
                        propHelper.DockStyle(),
                        propHelper.Enabled(),
                        propHelper.GrowToHeight(),
                        propHelper.InteractionEditor(),
                        propHelper.Printable(),
                        propHelper.PrintOn(),
                        propHelper.ShiftMode(),
                    });
                    break;
            }

            switch (level)
            {
                case StiLevel.Basic:
                    collection.Add(StiPropertyCategories.Design, new[] 
                    { 
                        propHelper.Name() 
                    });
                    break;

                case StiLevel.Standard:
                    collection.Add(StiPropertyCategories.Design, new[]
                    {
                        propHelper.Name(),
                        propHelper.Alias()
                    });
                    break;

                case StiLevel.Professional:
                    collection.Add(StiPropertyCategories.Design, new[]
                    {
                        propHelper.Name(),
                        propHelper.Alias(),
                        propHelper.Restrictions(),
                        propHelper.Locked(),
                        propHelper.Linked()
                    });
                    break;
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
                        StiPropertyEventId.MouseLeaveEvent,
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
                        StiPropertyEventId.GetBarCodeEvent,
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/report_internals_barcodes.htm";
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties)
        {
            var barCode = (StiBarCode)base.Clone(cloneProperties);

            if (this.BarCodeType != null)
                barCode.BarCodeType = (StiBarCodeTypeService)this.BarCodeType.Clone();

            return barCode;
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

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.BarCode;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory => StiLocalization.Get("Components", "StiBarCode");

        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiBarCode");
        #endregion

        #region IStiExportImageExtended
        [Browsable(false)]
        public override bool IsExportAsImage(StiExportFormat format)
        {
            if (format == StiExportFormat.ImageSvg)
                return false;

            if (format == StiExportFormat.Pdf && !StiOptions.Export.Pdf.RenderBarCodeAsImage)
                return false;

            return true;
        }

        public virtual Image GetImage(ref float zoom)
        {
            return GetImage(ref zoom, StiExportFormat.None);
        }

        public virtual Image GetImage(ref float zoom, StiExportFormat format)
        {
            var painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi);
            return painter.GetImage(this, ref zoom, format);
        }
        #endregion

        #region IStiEnumAngle
        /// <summary>
        /// Gets or sets angle of a bar code rotation.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiAngle.Angle0)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("BarCodeAdditional")]
        [StiOrder(StiPropertyOrder.BarCodeAngle)]
        [Description("Gets or sets angle of a bar code rotation.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiAngle Angle { get; set; } = StiAngle.Angle0;

        /// <summary>
        /// The appearance and behavior of the component border.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiSerializable]
        [Description("The appearance and behavior of the component border.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBorder Border { get; set; } = new StiBorder();
        #endregion

        #region IStiBarCode
        /// <summary>
		/// Gets or sets bar code color.
		/// </summary>
		[StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceForeColor)]
        [StiSerializable]
        [TypeConverter(typeof(StiExpressionColorConverter))]
        [Editor(StiEditors.ExpressionColor, typeof(UITypeEditor))]
        [Description("Gets or sets bar code color.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiExpressionAllowed]
        public Color ForeColor { get; set; } = Color.Black;

        private bool ShouldSerializeForeColor()
        {
            return ForeColor != Color.Black;
        }

        /// <summary>
		/// The background color of the component.
		/// </summary>
		[StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBackColor)]
        [StiSerializable()]
        [TypeConverter(typeof(StiExpressionColorConverter))]
        [Editor(StiEditors.ExpressionColor, typeof(UITypeEditor))]
        [Description("The background color of the component.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiExpressionAllowed]
        public Color BackColor { get; set; } = Color.White;

        private bool ShouldSerializeBackColor()
        {
            return BackColor != Color.White;
        }

        /// <summary>
		/// Gets or sets value which indicates how bar code will scale its size.
		/// </summary>
		[DefaultValue(false)]
        [StiSerializable]
        [StiCategory("BarCodeAdditional")]
        [StiOrder(StiPropertyOrder.BarCodeAutoScale)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates how bar code will scale its size.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool AutoScale { get; set; }

        /// <summary>
        /// Gets or sets value which indicates will this bar code show label text or no. Only for linear barcodes.
        /// </summary>
		[DefaultValue(true)]
        [StiSerializable]
        [StiCategory("BarCodeAdditional")]
        [StiOrder(StiPropertyOrder.BarCodeShowLabelText)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates will this bar code show label text or no. Only for linear barcodes.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool ShowLabelText { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates will this bar code show quiet zones or no.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("BarCodeAdditional")]
        [StiOrder(StiPropertyOrder.BarCodeShowQuietZones)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates will this bar code show quiet zones or no.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool ShowQuietZones { get; set; } = true;

        /// <summary>
        /// Gets or sets type of the bar code.
        /// </summary>
        private StiBarCodeTypeService barCodeType = new StiEAN13BarCodeType();
        [StiCategory("BarCode")]
        [StiOrder(StiPropertyOrder.BarCodeBarCodeType)]
        [StiSerializable(StiSerializationVisibility.Class)]
        [Editor("Stimulsoft.Report.BarCodes.Design.StiBarCodeTypeServiceEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets type of the bar code.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBarCodeTypeService BarCodeType
        {
            get
            {
                return barCodeType;
            }
            set
            {
                if (value != null && Code != null)
                {
                    if ((barCodeType == null) || (Code.Value == barCodeType.DefaultCodeValue)) Code.Value = value.DefaultCodeValue;
                }
                barCodeType = value;
            }
        }

        public string GetBarCodeString()
        {
            if (CodeValue != null)
                return CodeValue;

            if (StiOptions.Designer.CalculateBarcodeValueInDesignMode)
            {
                try
                {
#if NETSTANDARD
                    var result = global::System.Convert.ToString(Engine.StiParser.ParseTextValue(Code.Value, this));
#else
                    var result = System.Convert.ToString(Engine.StiParser.ParseTextValue(Code.Value, this));
#endif
                    if (!string.IsNullOrWhiteSpace(result))
                        return result;
                }
                catch
                {
                }
            }
            return Code.Value;
        }
        #endregion

        #region IStiHorAlignment
        /// <summary>
        /// Gets or sets the horizontal alignment of an barcode.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiHorAlignment.Left)]
        [StiCategory("BarCodeAdditional")]
        [StiOrder(StiPropertyOrder.BarCodeHorAlignment)]
        [Description("Gets or sets the horizontal alignment of an object.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [TypeConverter(typeof(StiExpressionEnumConverter))]
        [Editor(StiEditors.ExpressionEnum, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public StiHorAlignment HorAlignment { get; set; } = StiHorAlignment.Left;
        #endregion

        #region IStiVertAlignment
        /// <summary>
        /// Gets or sets the vertical alignment of an barcode.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiVertAlignment.Top)]
        [StiCategory("BarCodeAdditional")]
        [StiOrder(StiPropertyOrder.BarCodeVertAlignment)]
        [Description("Gets or sets the vertical alignment of an object.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [TypeConverter(typeof(StiExpressionEnumConverter))]
        [Editor(StiEditors.ExpressionEnum, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public StiVertAlignment VertAlignment { get; set; } = StiVertAlignment.Top;
        #endregion

        #region IStiGetFonts
        public override List<StiFont> GetFonts()
        {
            var result = base.GetFonts();
            result.Add(new StiFont(Font));
            return result.Distinct().ToList();
        }
        #endregion

        #region Expressions

        #region BarCode expression
        /// <summary>
        /// Gets or sets the component bar code.
        /// </summary>
        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        [Description("Gets or sets the component bar code.")]
        public string CodeValue { get; set; }

        // TODO
        //
        //public string CodeValue
        //{
        //    get
        //    {
        //        return Unpack(CodeValueInternal);
        //    }
        //    set
        //    {
        //        CodeValueInternal = Pack(value);
        //    }
        //}

        //private string Pack(string input)
        //{
        //    if (string.IsNullOrWhiteSpace(input)) return input;
        //    StringBuilder sb = new StringBuilder("~");
        //    bool flag = false;
        //    foreach (char ch in input)
        //    {
        //        if (ch == '~')
        //        {
        //            sb.Append("~~");
        //            flag = true;
        //        }
        //        else if (ch < 0x20)
        //        {
        //            sb.Append(string.Format("~{0:X2}", (int)ch));
        //            flag = true;
        //        }
        //        else
        //        {
        //            sb.Append((char)ch);
        //        }
        //    }
        //    if (flag) return sb.ToString();
        //    return input;
        //}

        //private string Unpack(string input)
        //{
        //    if (string.IsNullOrWhiteSpace(input) || !input.StartsWith("~")) return input;
        //    StringBuilder sb = new StringBuilder();
        //    for (int index = 1; index < input.Length; index++)
        //    {
        //        char ch = input[index];
        //        if (ch == '~')
        //        {
        //            if (input[index + 1] == '~')
        //            {
        //                sb.Append("~");
        //                index++;
        //            }
        //            else
        //            {
        //                sb.Append((char)int.Parse(input.Substring(index + 1, 2), System.Globalization.NumberStyles.HexNumber));
        //                index += 2;
        //            }
        //        }
        //        else
        //        {
        //            sb.Append((char)ch);
        //        }
        //    }
        //    return sb.ToString();
        //}

        /// <summary>
		/// Gets or sets the expression to fill a code of bar code.
		/// </summary>
		[StiCategory("BarCode")]
        [StiOrder(StiPropertyOrder.BarCodeCode)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a code of bar code.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual StiBarCodeExpression Code
        {
            get
            {
                if (BarCodeType != null)
                {
                    var combinedCode = BarCodeType.GetCombinedCode();
                    if (combinedCode != null)
                        return new StiBarCodeExpression(combinedCode);
                }
                return new StiBarCodeExpression(this, "Code");
            }
            set
            {
                if (value != null)
                    value.Set(this, "Code", value.Value);
            }
        }
        #endregion
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
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    if (this.Events[EventGetBarCode] != null && CodeValue == null)
                    {
                        var e = new StiValueEventArgs();
                        InvokeGetBarCode(this, e);
                        if (e.Value != null)
                            CodeValue = e.Value.ToString();
                    }
                }
                else
                {
                    if (CodeValue == null)
                    {
                        var e = new StiValueEventArgs();
                        InvokeGetBarCode(this, e);
                        if (e.Value != null)
                            CodeValue = e.Value.ToString();
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "DoEvents...ERROR");
                StiLogService.Write(this.GetType(), e);
            }
        }

        #region GetBarCode
        private static readonly object EventGetBarCode = new object();

        /// <summary>
        /// Occurs when getting the code of barcode.
        /// </summary>
        public event StiValueEventHandler GetBarCode
        {
            add
            {
                Events.AddHandler(EventGetBarCode, value);
            }
            remove
            {
                Events.RemoveHandler(EventGetBarCode, value);
            }
        }

        /// <summary>
        /// Raises the BarCode event.
        /// </summary>
        protected virtual void OnGetBarCode(StiValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetBarCode event.
        /// </summary>
        public void InvokeGetBarCode(StiComponent sender, StiValueEventArgs e)
        {
            try
            {
                OnGetBarCode(e);
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    (Events[EventGetBarCode] as StiValueEventHandler)?.Invoke(sender, e);
                }
                else
                {
                    object parserResult = Engine.StiParser.ParseTextValue(this.Code.Value, this, sender);
                    if (parserResult != null) e.Value = parserResult;
                    (Events[EventGetBarCode] as StiValueEventHandler)?.Invoke(sender, e);
                }
            }
            catch (Exception ex)
            {
                var str = $"Expression in BarCode property of '{this.Name}' can't be evaluated!";
                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);
                Report.WriteToReportRenderingMessages(str);

            }
        }

        /// <summary>
        /// Occurs when getting the code of bar code.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the code of bar code.")]
        public StiGetBarCodeEvent GetBarCodeEvent
        {
            get
            {
                return new StiGetBarCodeEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiBarCode();
        }
        #endregion

        #region Properties
        private Font font;
        /// <summary>
        /// Gets or sets font of bar code.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceFont)]
        [Description("Gets or sets font of bar code.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        public virtual Font Font
        {
            get
            {
                return font ?? (font = new Font("Arial", 8, FontStyle.Bold, GraphicsUnit.Pixel));
            }
            set
            {
                font = value;
            }
        }

        /// <summary>
        /// Gets or sets the default client area of a component.
        /// </summary>
        [Browsable(false)]
        public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 240, 110);
        #endregion

        /// <summary>
        /// Creates a new component of the type StiBarCode.
        /// </summary>
        public StiBarCode() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiBarCode.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiBarCode(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
            Code.Value = this.BarCodeType.DefaultCodeValue;
        }
    }
}