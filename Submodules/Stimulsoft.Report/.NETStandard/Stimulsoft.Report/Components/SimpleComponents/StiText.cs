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
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Xml;
using System.Linq;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Report.Helpers;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the class that realizes the component - Text.
    /// </summary>
    [StiServiceBitmap(typeof(StiText), "Stimulsoft.Report.Images.Components.StiText.png")]
    [StiToolbox(true)]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiTextDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfTextDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiTextGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiTextWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiAutoWidth))]
    [StiContextTool(typeof(IStiGrowToHeight))]
    [StiContextTool(typeof(IStiText))]
    [StiContextTool(typeof(IStiOnlyText))]
    [StiContextTool(typeof(IStiEditable))]
    [StiContextTool(typeof(IStiShift))]
    [StiContextTool(typeof(IStiTextOptions))]
    [StiContextTool(typeof(IStiTextFormat))]
    [StiContextTool(typeof(IStiBreakable))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiTextQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfTextQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiText :
        StiSimpleText,
        IStiTextOptions,
        IStiAutoWidth,
        IStiTextHorAlignment,
        IStiVertAlignment,
        IStiBorder,
        IStiFont,
        IStiBrush,
        IStiTextBrush,
        IStiTextFormat,
        IStiRenderTo,
        IStiSerializable,
        IStiBreakable,
        IStiGlobalizationProvider,
        IStiExportImageExtended,
        IStiIndicator
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiText
            if (Indicator != null)
                jObject.AddPropertyJObject("Indicator", Indicator.SaveToJsonObject(mode));

            jObject.AddPropertyBool("CanBreak", CanBreak);
            jObject.AddPropertyBool("AutoWidth", AutoWidth);
            jObject.AddPropertyStringNullOrEmpty("RenderTo", RenderTo);
            jObject.AddPropertyEnum("HorAlignment", HorAlignment, StiTextHorAlignment.Left);
            jObject.AddPropertyEnum("VertAlignment", VertAlignment, StiVertAlignment.Top);
            jObject.AddPropertyFontArial8("Font", Font);
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyBrush("TextBrush", TextBrush);

            jObject.AddPropertyJObject("TextOptions", TextOptions.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetExcelValueEvent", GetExcelValueEvent.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("NullValue", NullValue);
            jObject.AddPropertyBool("ExportAsImage", ExportAsImage);
            jObject.AddPropertyEnum("TextQuality", TextQuality, StiTextQuality.Standard);
            jObject.AddPropertyBool("AllowHtmlTags", AllowHtmlTags);
            jObject.AddPropertyJObject("Margins", Margins.SaveToJsonObject(0, 0, 0, 0));
            jObject.AddPropertyEnum("ExceedMargins", ExceedMargins, StiExceedMargins.None);
            jObject.AddPropertyBool("ShrinkFontToFit", ShrinkFontToFit);
            jObject.AddPropertyFloat("ShrinkFontToFitMinimumSize", ShrinkFontToFitMinimumSize, 1f);
            jObject.AddPropertyDouble("LineSpacing", LineSpacing, 1d);

            if (mode == StiJsonSaveMode.Report)
            {
                if (!(TextFormat is StiGeneralFormatService))
                    jObject.AddPropertyJObject("TextFormat", TextFormat.SaveToJsonObject(mode));

                jObject.AddPropertyEnum("Type", Type, StiSystemTextType.None);
                jObject.AddPropertyJObject("ExcelValue", ExcelValue.SaveToJsonObject(mode));
            }
            else
            {
                jObject.AddPropertyStringNullOrEmpty("Format", Format);
                if (ExcelDataValue != null)
                    jObject.AddPropertyString("ExcelDataValue", ExcelDataValue);
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
                    case "Indicator":
                        Indicator = StiIndicator.CreateFromJsonObject((JObject)property.Value);
                        break;

                    case "CanBreak":
                        this.CanBreak = property.DeserializeBool();
                        break;

                    case "AutoWidth":
                        this.AutoWidth = property.DeserializeBool();
                        break;

                    case "RenderTo":
                        this.RenderTo = property.DeserializeString();
                        break;

                    case "HorAlignment":
                        this.HorAlignment = property.DeserializeEnum<StiTextHorAlignment>();
                        break;

                    case "VertAlignment":
                        this.VertAlignment = property.DeserializeEnum<StiVertAlignment>();
                        break;

                    case "Font":
                        this.font = property.DeserializeFont(this.font);
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

                    case "TextFormat":
                        this.TextFormat = StiFormatService.CreateFromJsonObject((JObject)property.Value);
                        break;

                    case "Format":
                        this.format = property.DeserializeString();
                        break;

                    case "TextOptions":
                        this.TextOptions.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ExcelValue":
                        {
                            var expression = new StiExcelValueExpression();
                            expression.LoadFromJsonObject((JObject)property.Value);
                            this.ExcelValue = expression;
                        }
                        break;

                    case "ExcelDataValue":
                        this.ExcelDataValue = property.DeserializeString();
                        break;

                    case "GetExcelValueEvent":
                        {
                            var localEvent = new StiGetExcelValueEvent();
                            localEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetExcelValueEvent = localEvent;
                        }
                        break;

                    case "NullValue":
                        this.NullValue = property.DeserializeString();
                        break;

                    case "Type":
                        this.Type = property.DeserializeEnum<StiSystemTextType>();
                        break;

                    case "ExportAsImage":
                        this.ExportAsImage = property.DeserializeBool();
                        break;

                    case "TextQuality":
                        this.TextQuality = property.DeserializeEnum<StiTextQuality>();
                        break;

                    case "LineSpacing":
                        this.LineSpacing = property.DeserializeFloat();
                        break;

                    case "AllowHtmlTags":
                        this.AllowHtmlTags = property.DeserializeBool();
                        break;

                    case "Margins":
                        {
                            var margins = new StiMargins();
                            margins.LoadFromJsonObject((JObject)property.Value);

                            this.Margins = margins;
                        }
                        break;

                    case "ExceedMargins":
                        this.ExceedMargins = property.DeserializeEnum<StiExceedMargins>();
                        break;

                    case "ShrinkFontToFit":
                        this.ShrinkFontToFit = property.DeserializeBool();
                        break;

                    case "ShrinkFontToFitMinimumSize":
                        this.ShrinkFontToFitMinimumSize = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiText;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var collection = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.TextEditor()
            });

            collection.Add(StiPropertyCategories.Text, new[]
            {
                propHelper.Text(),
                propHelper.HorAlignment(),
                propHelper.VertAlignment(),
                propHelper.TextFormat()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.TextAngle(),
                    propHelper.HideZeros(),
                    propHelper.LineSpacing(),
                    propHelper.WordWrap(),
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.AllowHtmlTags(),
                    propHelper.TextAngle(),
                    propHelper.Editable(),
                    propHelper.HideZeros(),
                    propHelper.LinesOfUnderline(),
                    propHelper.LineSpacing(),
                    propHelper.Margins(),
                    propHelper.MaxNumberOfLines(),
                    propHelper.OnlyText(),
                    propHelper.ProcessingDuplicates(),
                    propHelper.TextQuality(),
                    propHelper.WordWrap(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.AllowHtmlTags(),
                    propHelper.TextAngle(),
                    propHelper.Editable(),
                    propHelper.HideZeros(),
                    propHelper.LinesOfUnderline(),
                    propHelper.LineSpacing(),
                    propHelper.Margins(),
                    propHelper.MaxNumberOfLines(),
                    propHelper.OnlyText(),
                    propHelper.ProcessAt(),
                    propHelper.ProcessingDuplicates(),
                    propHelper.RenderTo(),
                    propHelper.ShrinkFontToFit(),
                    propHelper.ShrinkFontToFitMinimumSize(),
                    propHelper.TextQuality(),
                    propHelper.TextOptions(),
                    propHelper.WordWrap(),
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
                    propHelper.Font(),
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
                    propHelper.Font(),
                    propHelper.TextBrush(),
                    propHelper.UseParentStyles()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.AutoWidth(),
                    propHelper.CanBreak(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.AutoWidth(),
                    propHelper.CanBreak(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
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
                    propHelper.AutoWidth(),
                    propHelper.CanBreak(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
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
                    propHelper.GlobalizedName(),
                    propHelper.Restrictions(),
                    propHelper.Locked(),
                    propHelper.Linked()
                });
            }
            
            if (level == StiLevel.Professional)
            {
                collection.Add(StiPropertyCategories.Export, new[]
                {
                    propHelper.ExcelValue(),
                    propHelper.ExportAsImage()
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
                        StiPropertyEventId.GetPointerEvent
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
                        StiPropertyEventId.GetExcelValueEvent,
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                        StiPropertyEventId.GetValueEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "User-Manual/report_internals_output_text_parameters.htm";
        #endregion

        #region IStiIndicator
        /// <summary>
        /// Gets or sets special indicator for text component.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets special indicator for text component.")]
        [Browsable(false)]
        public virtual StiIndicator Indicator { get; set; }
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
            if ((this.Indicator != null) && (format != StiExportFormat.Pdf))
                return true;

            var result = base.IsExportAsImage(format);
            if (result)
                return result;

            if (CheckAllowHtmlTags())
            {
                var nativeExport = format == StiExportFormat.Html ||
                    format == StiExportFormat.HtmlDiv ||
                    format == StiExportFormat.HtmlSpan ||
                    format == StiExportFormat.HtmlTable ||
                    format == StiExportFormat.Rtf ||
                    format == StiExportFormat.RtfTabbedText ||
                    format == StiExportFormat.RtfFrame ||
                    format == StiExportFormat.RtfWinWord ||
                    format == StiExportFormat.RtfTable ||
                    format == StiExportFormat.Text ||
                    format == StiExportFormat.Pdf ||
                    format == StiExportFormat.ImageSvg ||
                    format == StiExportFormat.ImageSvgz ||
                    format == StiExportFormat.Word2007 && !StiOptions.Export.Word.RenderHtmlTagsAsImage ||
                    format == StiExportFormat.Excel2007 && !StiOptions.Export.Excel.RenderHtmlTagsAsImage;

                if (!nativeExport)
                    return true;
            }

            if (LinesOfUnderlining)
            {
                var nativeExport = format == StiExportFormat.Dif ||
                    format == StiExportFormat.Pdf ||
                    format == StiExportFormat.Sylk ||
                    format == StiExportFormat.Text;

                if (!nativeExport)
                    return true;
            }

            return ExportAsImage;
        }
        #endregion

        #region IStiGlobalizationProvider
        /// <summary>
        /// Sets localized string to specified property name.
        /// </summary>
        void IStiGlobalizationProvider.SetString(string propertyName, string value)
        {
            if (propertyName == "Text")
                SetTextInternal(value);

            else if (propertyName == "ToolTip")
                this.ToolTip.Value = value;

            else if (propertyName == "Tag")
                this.Tag.Value = value;

            else if (propertyName == "Hyperlink")
                this.Hyperlink.Value = value;

            else
                throw new ArgumentException($"Globalization for {this.Name} - Property with name {propertyName}");
        }

        /// <summary>
        /// Gets localized string from specified property name.
        /// </summary>
        string IStiGlobalizationProvider.GetString(string propertyName)
        {
            if (propertyName == "Text")
                return GetTextInternal();

            if (propertyName == "Tag")
                return this.Tag.Value;

            if (propertyName == "ToolTip")
                return this.ToolTip.Value;

            if (propertyName == "Hyperlink")
                return this.Hyperlink.Value;

            throw new ArgumentException($"Property with name {propertyName}");
        }

        /// <summary>
        /// Returns array of the property names which can be localized.
        /// </summary>
        string[] IStiGlobalizationProvider.GetAllStrings()
        {
            var strs = new List<string>();

            if (StiOptions.Engine.Globalization.AllowUseText)
                strs.Add("Text");

            if (StiOptions.Engine.Globalization.AllowUseTag)
                strs.Add("Tag");

            if (StiOptions.Engine.Globalization.AllowUseToolTip)
                strs.Add("ToolTip");

            if (StiOptions.Engine.Globalization.AllowUseHyperlink)
                strs.Add("Hyperlink");

            return strs.ToArray();
        }
        #endregion

        #region IStiBreakable
        protected static object PropertyCanBreak = new object();
        /// <summary>
        /// Gets or sets value which indicates whether the component can or cannot break its contents on several pages.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorCanBreak)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether the component can or cannot break its contents on several pages.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool CanBreak
        {
            get
            {
                return Properties.GetBool(PropertyCanBreak, false);
            }
            set
            {
                Properties.SetBool(PropertyCanBreak, value, false);
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

            if (GetTextInternal() != null && GetTextInternal().Length > 0)
            {
                var breakValue = GetTextInternal();
                var rect = Report.Unit.ConvertToHInches(this.ClientRectangle);

                rect = ConvertTextMargins(rect, false);
                rect = ConvertTextBorders(rect, false);

                var str = string.Empty;
                if (rect.Height >= 0)
                {
                    str = StiComponentDivider.BreakText(GetMeasureGraphics(), rect, ref breakValue, Font,
                        TextOptions, TextQuality, CheckAllowHtmlTags(), this);
                }

                this.SetTextInternal(str);

                if (!this.GrowToHeight && (GetTextInternal() == null || GetTextInternal().Length == 0))
                    result = false;

                ((StiText)dividedComponent).SetTextInternal(breakValue);
                if (StiOptions.Engine.MarkBreakedText && (dividedComponent.TagValue == null)) dividedComponent.TagValue = "#breaked#";
            }
            return result;
        }
        #endregion

        #region IStiAutoWidth
        protected static object PropertyAutoWidth = new object();
        /// <summary>
        /// Gets or sets value indicates that this object can change width automatically.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorAutoWidth)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that this object can change width automatically.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool AutoWidth
        {
            get
            {
                return Properties.GetBool(PropertyAutoWidth, false);
            }
            set
            {
                Properties.SetBool(PropertyAutoWidth, value, false);
            }
        }
        #endregion

        #region IStiSerializable
        protected virtual StiTextHorAlignment DefaultHorAlignment => StiTextHorAlignment.Left;

        protected void Write(StiObjectStringConverter converter, string name, object obj, XmlTextWriter tw)
        {
            var s = converter.ObjectToString(obj);
            if (s != null)
                tw.WriteAttributeString(name, s);
        }

        /// <summary>
        /// Serializes object into XmlTextWriter.
        /// </summary>
        /// <param name="converter">The converter to convert objects into strings.</param>
        /// <param name="tw">XmlTextWriter for serialization.</param>
        public void Serialize(StiObjectStringConverter converter, XmlTextWriter tw)
        {
            tw.WriteAttributeString("name", this.Name);
            tw.WriteAttributeString("text", this.Text);

            if (this.HorAlignment != DefaultHorAlignment)
                tw.WriteAttributeString("ha", this.HorAlignment.ToString());

            if (this is StiCrossField)
            {
                if (this.VertAlignment != StiVertAlignment.Center)
                    tw.WriteAttributeString("va", this.VertAlignment.ToString());
            }
            else
            {
                if (this.VertAlignment != StiVertAlignment.Top)
                    tw.WriteAttributeString("va", this.VertAlignment.ToString());
            }

            if (this.LineSpacing != 1)
                tw.WriteAttributeString("ls", this.LineSpacing.ToString(CultureInfo.InvariantCulture));

            if (this.TextQuality != StiTextQuality.Standard)
                tw.WriteAttributeString("tq", this.TextQuality.ToString());

            if (this.Editable)
                tw.WriteAttributeString("ed", this.Editable ? "true" : "false");

            if (!this.Enabled)
                tw.WriteAttributeString("enabled", this.Enabled ? "true" : "false");

            if (!this.Printable)
                tw.WriteAttributeString("pr", this.Printable ? "true" : "false");

            if (this.AllowHtmlTags)
                tw.WriteAttributeString("html", this.AllowHtmlTags ? "true" : "false");

            if (this.LinesOfUnderline != StiPenStyle.None)
                tw.WriteAttributeString("lu", this.LinesOfUnderline.ToString());

            if (this.Guid != null)
                tw.WriteAttributeString("guid", this.Guid);

            if (this.TotalValueHelp != null)
                tw.WriteAttributeString("tvh", this.TotalValueHelp);

            if (this.PointerValue != null)
                tw.WriteAttributeString("pointer", this.PointerValue.ToString());

            if (this.BookmarkValue != null)
                tw.WriteAttributeString("bookmark", this.BookmarkValue.ToString());

            if (this.HyperlinkValue != null)
                tw.WriteAttributeString("hyperlink", this.HyperlinkValue.ToString());

            if (this.TagValue != null)
                tw.WriteAttributeString("tag", this.TagValue.ToString());

            if (this.ToolTipValue != null)
                tw.WriteAttributeString("toolTip", this.ToolTipValue.ToString());

            if (this.ExcelDataValue != null)
                tw.WriteAttributeString("excelvalue", this.ExcelDataValue);

            if (this.Format != null && format.Length > 0)
                tw.WriteAttributeString("format", this.Format);

            if (!string.IsNullOrEmpty(this.ComponentPlacement))
                tw.WriteAttributeString("pl", this.ComponentPlacement);

            if (!string.IsNullOrEmpty(this.ComponentStyle))
                tw.WriteAttributeString("style", this.ComponentStyle);

            Write(converter, "rc", this.ClientRectangle, tw);
            Write(converter, "fn", this.Font, tw);
            Write(converter, "tb", this.TextBrush, tw);
            Write(converter, "bh", this.Brush, tw);
            Write(converter, "br", this.Border, tw);
            Write(converter, "to", this.TextOptions, tw);

            if (Margins.Left != 0 || Margins.Right != 0 || Margins.Top != 0 || Margins.Bottom != 0)
                Write(converter, "mr", Margins, tw);

            if (Indicator != null)
            {
                var indicatorConverter = new StiIndicatorConverter();
                var str = indicatorConverter.ConvertToInvariantString(Indicator);
                tw.WriteAttributeString("indicator", str);
            }

            if (this.ExceedMargins != StiExceedMargins.None)
                tw.WriteAttributeString("exceedmargins", this.ExceedMargins.ToString());

            if (Page.Document != null && Page.Document.Report.SaveInteractionParametersToDocument)
            {
                if (Interaction != null)
                {
                    var jsonString = Interaction.SaveToJsonObject(StiJsonSaveMode.Document).ToString();
                    tw.WriteAttributeString("interaction", jsonString.Substring(1, jsonString.Length - 2));
                }

                if (DrillDownParameters != null && DrillDownParameters.Count > 0)
                {
                    var jsonString = JsonConvert.SerializeObject(DrillDownParameters);
                    tw.WriteAttributeString("ddparams", jsonString.Substring(1, jsonString.Length - 2));
                }
            }
        }

        /// <summary>
        /// Deserilizes object from XmlTextReader.
        /// </summary>
        /// <param name="converter">The converter to convert strings into objects.</param>
        /// <param name="tr">XmlTextWriter for deserialization.</param>
        public void Deserialize(StiObjectStringConverter converter, XmlTextReader tr)
        {
            var value = tr.GetAttribute("name");
            if (value != null) this.Name = value;

            value = tr.GetAttribute("text");
            if (value != null) this.Text = value;

            value = tr.GetAttribute("ha");
            if (value != null) this.HorAlignment = (StiTextHorAlignment)Enum.Parse(typeof(StiTextHorAlignment), value);

            value = tr.GetAttribute("va");
            if (value != null) this.VertAlignment = (StiVertAlignment)Enum.Parse(typeof(StiVertAlignment), value);

            value = tr.GetAttribute("ls");
            if (value != null) this.LineSpacing = float.Parse(value);

            value = tr.GetAttribute("tq");
            if (value != null) this.TextQuality = (StiTextQuality)Enum.Parse(typeof(StiTextQuality), value);

            value = tr.GetAttribute("ed");
            if (value != null) this.Editable = value.ToLowerInvariant() == "true";

            value = tr.GetAttribute("html");
            if (value != null) this.AllowHtmlTags = value.ToLowerInvariant() == "true";

            value = tr.GetAttribute("enabled");
            if (value != null) this.Enabled = value.ToLowerInvariant() == "true";

            value = tr.GetAttribute("ws");
            if (value != null) this.WYSIWYG = value.ToLowerInvariant() == "true";

            value = tr.GetAttribute("pr");
            if (value != null) this.Printable = value.ToLowerInvariant() != "false";

            value = tr.GetAttribute("lou");
            if (value != null) this.LinesOfUnderlining = value.ToLowerInvariant() == "true";

            value = tr.GetAttribute("lu");
            if (value != null)
                this.LinesOfUnderline = (StiPenStyle)Enum.Parse(typeof(StiPenStyle), value);

            this.Guid = tr.GetAttribute("guid") as string;
            this.TotalValueHelp = tr.GetAttribute("tvh") as string;
            this.PointerValue = tr.GetAttribute("pointer");
            this.BookmarkValue = tr.GetAttribute("bookmark");
            this.HyperlinkValue = tr.GetAttribute("hyperlink");
            this.TagValue = tr.GetAttribute("tag");
            this.ToolTipValue = tr.GetAttribute("toolTip");
            this.ExcelDataValue = tr.GetAttribute("excelvalue");

            this.format = tr.GetAttribute("format");

            this.ComponentPlacement = tr.GetAttribute("pl");
            this.ComponentStyle = tr.GetAttribute("style");

            value = tr.GetAttribute("rc");
            if (value != null)
                this.ClientRectangle = (RectangleD)converter.StringToObject(value, typeof(RectangleD));

            value = tr.GetAttribute("fn");
            if (value != null)
                this.Font = converter.StringToObject(value, typeof(Font)) as Font;

            value = tr.GetAttribute("tb");
            if (value != null)
                this.TextBrush = converter.StringToObject(value, typeof(StiBrush)) as StiBrush;

            value = tr.GetAttribute("bh");
            if (value != null)
                this.Brush = converter.StringToObject(value, typeof(StiBrush)) as StiBrush;

            value = tr.GetAttribute("to");
            if (value != null)
                this.TextOptions = converter.StringToObject(value, typeof(StiTextOptions)) as StiTextOptions;

            value = tr.GetAttribute("br");
            if (value != null)
                this.Border = converter.StringToObject(value, typeof(StiBorder)) as StiBorder;

            value = tr.GetAttribute("mr");
            if (value != null)
                this.Margins = (StiMargins)converter.StringToObject(value, typeof(StiMargins));

            value = tr.GetAttribute("indicator");
            if (value != null)
            {
                var indicatorConverter = new StiIndicatorConverter();
                this.Indicator = indicatorConverter.ConvertFromInvariantString(value) as StiIndicator;
            }

            value = tr.GetAttribute("interaction");
            if (value != null)
            {
                Interaction = new StiInteraction();
                var jsonObject = (JObject)JsonConvert.DeserializeObject($"{{{value}}}");
                Interaction.LoadFromJsonObject(jsonObject);
            }

            value = tr.GetAttribute("exceedmargins");
            if (value != null) this.ExceedMargins = (StiExceedMargins)Enum.Parse(typeof(StiExceedMargins), value);

            value = tr.GetAttribute("ddparams");
            if (value != null)
            {
                DrillDownParameters = new Dictionary<string, object>();
                JsonConvert.PopulateObject($"{{{value}}}", DrillDownParameters);
            }
        }
        #endregion

        #region IStiRenderTo
        protected static object PropertyRenderTo = new object();
        /// <summary>
        /// In the property specify the Text component in what the text 
        /// that is out of the current Text component bound	will be continued to be output.
        /// </summary>
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextRenderTo)]
        [StiSerializable]
        [DefaultValue("")]
        [Description("In the property specify the Text component in what the text that is out of the current Text component bound will be continued to be output.")]
        [TypeConverter(typeof(StiRenderToConverter))]
        [Editor("Stimulsoft.Report.Components.Design.StiRenderToEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual string RenderTo
        {
            get
            {
                return Properties.Get(PropertyRenderTo, string.Empty) as string;
            }
            set
            {
                Properties.Set(PropertyRenderTo, string.Intern(value), string.Empty);
            }
        }

        public override void InvokeRenderTo(StiSimpleText textFrom)
        {
            if (!textFrom.IsPropertyPresent(PropertyRenderTo))
                return;

            var renderTo = ((StiText)textFrom).RenderTo;
            if (renderTo.Length <= 0) return;

            renderTo = renderTo.Trim();
            if (renderTo.Length <= 0) return;

            var textTo = textFrom.Parent.Components[renderTo] as StiSimpleText;
            if (textTo == null) return;

            var textStr = textFrom.GetTextInternal();
            if (textStr == null)
                textStr = string.Empty;

            var visibleText = GetVisibleTextForRenderTo(this.GetMeasureGraphics(), textFrom.GetPaintRectangle(true, false, true),
                ref textStr, textFrom as StiText);

            if (string.IsNullOrEmpty(visibleText))
                visibleText = " ";

            textFrom.TextValue = visibleText;
            textFrom.SetTextInternal(visibleText);
            textTo.TextValue = textStr;
            textTo.SetTextInternal(textStr);

            InvokeRenderTo(textTo);
        }

        private string GetVisibleTextForRenderTo(Graphics g, RectangleD rect, ref string text, StiText checkedText)
        {
            rect = ConvertTextMargins(rect, false);
            rect = ConvertTextBorders(rect, false);

            return StiComponentDivider.BreakText(g, rect, ref text, checkedText.Font,
                checkedText.TextOptions, checkedText.TextQuality, CheckAllowHtmlTags(), this);
        }
        #endregion

        #region IStiWYSIWYG OFF
        /// <summary>
        /// Gets or sets value that indicates that this component is to be output as WYSIWYG concept.
        /// </summary>
        [StiNonSerialized]
        [Browsable(false)]
        [DefaultValue(false)]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value that indicates that this component is to be output as WYSIWYG concept.")]
        public virtual bool WYSIWYG
        {
            get
            {
                return TextQuality == StiTextQuality.Wysiwyg;
            }
            set
            {
                TextQuality = value ? StiTextQuality.Typographic : StiTextQuality.Standard;
            }
        }
        #endregion

        #region IStiTextHorAlignment
        /// <summary>
        /// Gets or sets the text horizontal alignment.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiTextHorAlignment.Left)]
        [StiCategory("Text")]
        [StiOrder(StiPropertyOrder.TextHorAlignment)]
        [Description("Gets or sets the text horizontal alignment.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiExpressionEnumConverter))]
        [Editor(StiEditors.ExpressionEnum, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public virtual StiTextHorAlignment HorAlignment { get; set; } = StiTextHorAlignment.Left;
        #endregion

        #region IStiVertAlignment
        /// <summary>
        /// Gets or sets the vertical alignment of an object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiVertAlignment.Top)]
        [StiCategory("Text")]
        [StiOrder(StiPropertyOrder.TextVertAlignment)]
        [Description("Gets or sets the vertical alignment of an object.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiExpressionEnumConverter))]
        [Editor(StiEditors.ExpressionEnum, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public virtual StiVertAlignment VertAlignment { get; set; } = StiVertAlignment.Top;
        #endregion

        #region IStiFont
        private Font font = new Font("Arial", 8);
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

        #region IStiBorder
        /// <summary>
        /// The appearance and behavior of the component border.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiSerializable]
        [Description("The appearance and behavior of the component border.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual StiBorder Border { get; set; } = new StiBorder();

        private bool ShouldSerializeBorder()
        {
            return Border == null || !Border.IsDefault;
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
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.Transparent);

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush && ((StiSolidBrush)Brush).Color == Color.Transparent);
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

        #region IStiTextFormat
        private StiFormatService textFormat;
        /// <summary>
        /// Gets or sets the format of the text.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [DefaultValue("")]
        [Editor("Stimulsoft.Report.Components.TextFormats.Design.StiTextFormatEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Text")]
        [StiOrder(StiPropertyOrder.TextTextFormat)]
        [Description("Gets or sets the format of the text.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual StiFormatService TextFormat
        {
            get
            {
                if (textFormat == null)
                    return StiGeneralFormatService.Default;

                return textFormat;
            }
            set
            {
                if (value is StiGeneralFormatService)
                    value = null;

                if (textFormat != value)
                {
                    textFormat = value;

                    #region StiGeneralFormatService
                    if (value == null)
                        format = "G";
                    #endregion

                    #region StiPercentageFormatService
                    else if (value is StiPercentageFormatService)
                    {
                        var percentageFormat = value as StiPercentageFormatService;
                        if (percentageFormat.UseLocalSetting && percentageFormat.State == StiTextFormatState.None)
                        {
                            format = "P";
                        }
                        else
                        {
                            format = "P" + percentageFormat.DecimalDigits;
                            if (percentageFormat.UseGroupSeparator)
                                format += "G";

                            if (!percentageFormat.UseLocalSetting && percentageFormat.DecimalDigits > 0 &&
                                (percentageFormat.DecimalSeparator[0] == '.' ||
                                 percentageFormat.DecimalSeparator[0] == ','))
                                format += percentageFormat.DecimalSeparator[0];
                        }
                    }
                    #endregion

                    #region StiCurrencyFormatService
                    else if (value is StiCurrencyFormatService)
                    {
                        var currencyFormat = value as StiCurrencyFormatService;
                        if (currencyFormat.UseLocalSetting && currencyFormat.State == StiTextFormatState.None)
                        {
                            format = "C";
                        }
                        else
                        {
                            format = "C" + currencyFormat.DecimalDigits;

                            if (currencyFormat.UseGroupSeparator)
                                format += "G";

                            if (currencyFormat.NegativePattern == 0 || currencyFormat.NegativePattern == 4 ||
                                currencyFormat.NegativePattern == 14 || currencyFormat.NegativePattern == 15)
                                format += "(";

                            if (currencyFormat.DecimalDigits > 0 &&
                                (currencyFormat.DecimalSeparator[0] == '.' ||
                                 currencyFormat.DecimalSeparator[0] == ','))
                                format += currencyFormat.DecimalSeparator[0];

                            if (currencyFormat.PositivePattern == 0 || currencyFormat.PositivePattern == 2)
                                format += "+";
                            else
                                format += "-";

                            format += currencyFormat.Symbol;

                            if (!currencyFormat.UseLocalSetting || ((currencyFormat.State & StiTextFormatState.PositivePattern) > 0) || ((currencyFormat.State & StiTextFormatState.NegativePattern) > 0))
                            {
                                format += "|" + (char)((int)'A' + currencyFormat.PositivePattern) + (char)((int)'A' + currencyFormat.NegativePattern);
                            }
                        }
                    }
                    #endregion

                    #region StiDateFormatService
                    else if (value is StiDateFormatService)
                    {
                        var dateFormat = value as StiDateFormatService;
                        format = $"D{dateFormat.StringFormat}";
                    }
                    #endregion

                    #region StiNumberFormatService
                    else if (value is StiNumberFormatService)
                    {
                        var numberFormat = value as StiNumberFormatService;
                        if (numberFormat.UseLocalSetting && numberFormat.State == StiTextFormatState.None)
                        {
                            format = "N";
                        }
                        else
                        {
                            format = $"N{numberFormat.DecimalDigits}";

                            if (numberFormat.UseGroupSeparator)
                                format += "G";

                            if (numberFormat.NegativePattern == 0)
                                format += "(";

                            if (numberFormat.DecimalDigits > 0 && !string.IsNullOrWhiteSpace(numberFormat.DecimalSeparator) && 
                                (numberFormat.DecimalSeparator[0] == '.' || numberFormat.DecimalSeparator[0] == ','))
                                format += numberFormat.DecimalSeparator[0];
                        }
                    }
                    #endregion

                    #region StiTimeFormatService
                    else if (value is StiTimeFormatService)
                    {
                        var timeFormat = value as StiTimeFormatService;
                        format = $"T{timeFormat.StringFormat}";
                    }
                    #endregion

                    #region StiCustomFormatService
                    else if (value is StiCustomFormatService)
                    {
                        format = "U";
                    }
                    #endregion

                    else
                        format = string.Empty;
                }
            }
        }


        private string format = string.Empty;
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        [DefaultValue("")]
        [Browsable(false)]
        public virtual string Format
        {
            get
            {
                return format;
            }
            set
            {
                if (format != value)
                {
                    format = string.Intern(value);
                }
            }
        }
        #endregion

        #region IStiTextOptions
        /// <summary>
        /// Gets or sets options to control of the text showing.
        /// </summary>
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextTextOptions)]
        [StiSerializable]
        [DefaultValue(null)]
        [Description("Gets or sets options to control of the text showing.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual StiTextOptions TextOptions { get; set; } = new StiTextOptions();
        #endregion

        #region ICloneable override
        public override object Clone(bool cloneProperties)
        {
            var textBox = (StiText)base.Clone(cloneProperties);

            if (this.Border != null)
                textBox.Border = (StiBorder)this.Border.Clone();
            else
                textBox.Border = null;

            if (this.Brush != null)
                textBox.Brush = (StiBrush)this.Brush.Clone();
            else
                textBox.Brush = null;

            if (this.textFormat != null)
                textBox.textFormat = (StiFormatService)this.textFormat.Clone();
            else
                textBox.textFormat = null;

            if (this.TextBrush != null)
                textBox.TextBrush = (StiBrush)this.TextBrush.Clone();
            else
                textBox.TextBrush = null;

            if (this.TextOptions != null)
                textBox.TextOptions = (StiTextOptions)this.TextOptions.Clone();
            else
                textBox.TextOptions = null;

            textBox.HorAlignment = this.HorAlignment;
            textBox.VertAlignment = this.VertAlignment;

            return textBox;
        }
        #endregion

        #region IStiGetFonts
        public override List<StiFont> GetFonts()
        {
            var result = base.GetFonts();
            result.Add(new StiFont(Font));
            return result.Distinct().ToList();
        }
        #endregion

        #region Paint
        public RectangleD ConvertTextMargins(RectangleD rect, bool convert)
        {
            var margins = Margins;//speed optimization
            if (margins.IsEmpty)
                return rect;

            var zoom = Page.Zoom * StiScale.Factor;

            var marginsLeft = margins.Left;
            var marginsRight = margins.Right;
            var marginsTop = margins.Top;
            var marginsBottom = margins.Bottom;

            if (!convert)
                zoom = 1;

            if (marginsLeft != 0)
            {
                rect.X += marginsLeft * zoom;
                rect.Width -= marginsLeft * zoom;
            }

            if (marginsTop != 0)
            {
                rect.Y += marginsTop * zoom;
                rect.Height -= marginsTop * zoom;
            }

            if (marginsRight != 0)
                rect.Width -= marginsRight * zoom;

            if (marginsBottom != 0)
                rect.Height -= marginsBottom * zoom;

            return rect;
        }

        public RectangleD ConvertTextBorders(RectangleD rect, bool convert)
        {
            double borderSizeLeft = 0;
            double borderSizeRight = 0;
            double borderSizeTop = 0;
            double borderSizeBottom = 0;

            if (this.Border.Style != StiPenStyle.None)
            {
                var tempSizeOffset = this.Border.GetSizeOffset();

                if (this.Border.IsLeftBorderSidePresent)
                    borderSizeLeft = tempSizeOffset;

                if (this.Border.IsRightBorderSidePresent)
                    borderSizeRight = tempSizeOffset;

                if (this.Border.IsTopBorderSidePresent)
                    borderSizeTop = tempSizeOffset;

                if (this.Border.IsBottomBorderSidePresent)
                    borderSizeBottom = tempSizeOffset;
            }

            var advBorder = this.Border as StiAdvancedBorder;
            if (advBorder != null)
            {
                borderSizeLeft = advBorder.LeftSide.GetSizeOffset();
                borderSizeRight = advBorder.RightSide.GetSizeOffset();
                borderSizeTop = advBorder.TopSide.GetSizeOffset();
                borderSizeBottom = advBorder.BottomSide.GetSizeOffset();
            }

            if (convert)
            {
                var zoom = (float)Page.Zoom * StiScale.Factor;
                borderSizeLeft *= zoom;
                borderSizeRight *= zoom;
                borderSizeTop *= zoom;
                borderSizeBottom *= zoom;
            }

            rect.X += borderSizeLeft;
            rect.Y += borderSizeTop;
            rect.Width -= borderSizeLeft + borderSizeRight;
            rect.Height -= borderSizeTop + borderSizeBottom;

            if (Report != null && Report.IsWpf)
                rect.Inflate(-1, -1);

            return rect;
        }

        public virtual string GetTextForPaint()
        {
            if (this.Report != null && this.Report.Designer != null && this.IsDesigning)
            {
                if (StiOptions.Designer.UseGlobalizationManager && (this.Report.GlobalizationManager != null) &&
                    !(this.Report.GlobalizationManager is StiNullGlobalizationManager) &&
                    !string.IsNullOrWhiteSpace(GlobalizedName))
                {
                    try
                    {
                        var text = this.Report.GlobalizationManager.GetString(GlobalizedName);
                        if (text != null)
                            return text;
                    }
                    catch
                    {
                    }
                }

                if (this.Report.Designer.UseAliases)
                {
                    if (this.Report.Designer.TextAliasesHash != null)
                    {

                        var text = this.Report.Designer.TextAliasesHash[this] as string;
                        if (text != null)
                            return text;

                        text = Stimulsoft.Report.Design.StiExpressionPacker.PackExpression(GetTextInternal(), this.Report.Designer, true);

                        this.Report.Designer.TextAliasesHash[this] = text;

                        return text;
                    }
                    else
                    {
                        return Stimulsoft.Report.Design.StiExpressionPacker.PackExpression(GetTextInternal(), this.Report.Designer, true);
                    }
                }
            }
            return GetTextInternal();
        }

        public bool GetMarkerFieldResult()
        {
            if (!StiOptions.Designer.MarkComponentsWithErrors)
                return false;

            if (!IsDesigning || OnlyText || Text == null || GetTextInternal() == null || Report == null)
                return false;

            var text = this.GetTextInternal();

            var index = text.IndexOfInvariant("{");
            if (index == -1)
                return false;

            var lastIndex = text.IndexOfInvariant("}");
            if (lastIndex == -1)
                return true;

            if (text.IndexOfInvariant("?") != -1)
                return false;

            var field = text.Substring(index + 1, lastIndex - index - 1);
            if (field.IndexOfInvariant(".") == -1)
                return false;

            var strs = field.Split('.');
            if (strs.Length > 2)
                return false;

            foreach (StiDataSource dataSource in Report.Dictionary.DataSources)
            {
                var dataSourceName = dataSource.Name;
                if (!(StiOptions.Configuration.IsWeb && (!StiOptions.Engine.FullTrust)))
                    dataSourceName = StiNameValidator.CorrectName(dataSourceName, Report);

                if (dataSourceName != strs[0]) continue;

                var columns = dataSource.Columns;

                var fieldIndex = 1;
                while (fieldIndex < strs.Length)
                {
                    var fieldStr = strs[fieldIndex];
                    foreach (StiDataColumn column in columns)
                    {
                        var columnName = column.Name;
                        if (!(StiOptions.Configuration.IsWeb && (!StiOptions.Engine.FullTrust)))
                            columnName = StiNameValidator.CorrectName(columnName, Report);

                        if (columnName == fieldStr)
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region IStiGetActualSize
        public override SizeD GetActualSize()
        {
            var autoWidth = this.AutoWidth;
            var angle = this.Angle;

            if ((CanGrow || CanShrink || autoWidth) && Page != null)
            {
                Hashtable hashCheckSize = null;

                if (StiOptions.Engine.AllowCacheForGetActualSize)
                {
                    var report = this.Report;
                    if (report != null && report.Engine != null)
                    {
                        if (report.Engine.HashCheckSize == null)
                            report.Engine.HashCheckSize = new Hashtable();

                        var obj = report.Engine.HashCheckSize[this];
                        if (obj != null)
                            return (SizeD)obj;

                        hashCheckSize = report.Engine.HashCheckSize;
                    }
                }

                var unit = this.Page.Unit;
                var rect = unit.ConvertToHInches(this.ClientRectangle);
                var newSize = new SizeD(rect.Width, rect.Height);

                rect = ConvertTextMargins(rect, false);
                rect = ConvertTextBorders(rect, false);

                SizeD actualSize;
                var textQuality = TextQuality;

                #region IsWpf
                if (this.Report.IsWpf && (!CheckAllowHtmlTags()))
                {
                    var resAngle = angle;
                    this.Angle = 0;

                    if (resAngle == 90 || resAngle == 270)
                    {
                        actualSize = StiWpfTextRender.MeasureString(rect.Height, this);
                        var temp = actualSize.Width;
                        actualSize.Width = actualSize.Height;
                        actualSize.Height = temp;
                    }
                    else
                        actualSize = StiWpfTextRender.MeasureString(rect.Width, this);

                    this.Angle = resAngle;
                }
                #endregion

                #region New Html engine
                else if (StiOptions.Engine.UseNewHtmlEngine && AllowHtmlTags)
                {
                    var resAngle = angle;
                    if (resAngle == 90 || resAngle == 270)
                    {
                        actualSize = StiHtmlTextRender.MeasureString(this);
                        var temp = actualSize.Width;
                        actualSize.Width = actualSize.Height;
                        actualSize.Height = temp;
                    }
                    else
                        actualSize = StiHtmlTextRender.MeasureString(this);

                    this.Angle = resAngle;
                }
                #endregion

                #region Wysiwyg
                else if (textQuality == StiTextQuality.Wysiwyg || CheckAllowHtmlTags())
                {
                    if (((angle > 45) && (angle < 135)) || ((angle > 225) && (angle < 315)))
                        actualSize = StiWysiwygTextRender.MeasureString(rect.Height, this.Font, this);
                    else
                        actualSize = StiWysiwygTextRender.MeasureString(rect.Width, this.Font, this);
                }
                #endregion

                #region Typographic
                else if (textQuality == StiTextQuality.Typographic)
                {
                    actualSize = angle == 90 || angle == 270
                        ? StiTypographicTextRender.MeasureString(rect.Height, this.Font, this)
                        : StiTypographicTextRender.MeasureString(rect.Width, this.Font, this);
                }
                #endregion

                else
                {
                    actualSize = angle == 90 || angle == 270
                        ? StiStandardTextRenderer.MeasureString(rect.Height, this.Font, this)
                        : StiStandardTextRenderer.MeasureString(rect.Width, this.Font, this);
                }

                var margins = Margins;   //speed optimization
                actualSize.Width += margins.Left + margins.Right;
                actualSize.Height += margins.Top + margins.Bottom;

                #region BorderSize correction
                double borderSizeLeft = 0;
                double borderSizeRight = 0;
                double borderSizeTop = 0;
                double borderSizeBottom = 0;

                if (this.Border.Style != StiPenStyle.None)
                {
                    var tempSizeOffset = this.Border.GetSizeOffset();
                    if (this.Border.IsLeftBorderSidePresent)
                        borderSizeLeft = tempSizeOffset;

                    if (this.Border.IsRightBorderSidePresent)
                        borderSizeRight = tempSizeOffset;

                    if (this.Border.IsTopBorderSidePresent)
                        borderSizeTop = tempSizeOffset;

                    if (this.Border.IsBottomBorderSidePresent)
                        borderSizeBottom = tempSizeOffset;
                }

                var advBorder = this.Border as StiAdvancedBorder;
                if (advBorder != null)
                {
                    borderSizeLeft = advBorder.LeftSide.GetSizeOffset();
                    borderSizeRight = advBorder.RightSide.GetSizeOffset();
                    borderSizeTop = advBorder.TopSide.GetSizeOffset();
                    borderSizeBottom = advBorder.BottomSide.GetSizeOffset();
                }

                actualSize.Width += borderSizeLeft + borderSizeRight;
                actualSize.Height += borderSizeTop + borderSizeBottom;
                #endregion

                if (Report != null && Report.IsWpf)
                {
                    actualSize.Width += 2;
                    actualSize.Height += 2;
                }

                #region AutoWidth
                if (autoWidth)
                {
                    if (angle == 90 || angle == 270)
                        newSize.Height = actualSize.Height;
                    else
                        newSize.Width = actualSize.Width;
                }
                #endregion

                #region CanGrow
                if (this.CanGrow)
                {
                    #region this.Angle == 90 || this.Angle == 270
                    if (angle == 90 || angle == 270)
                    {
                        if (actualSize.Width > newSize.Width)
                        {
                            newSize.Width = actualSize.Width;
                            if (this.MaxNumberOfLines > 0)
                            {
                                double fontHeight = this.Font.GetHeight() * StiScale.System;
                                var line = (int)(newSize.Width / fontHeight);
                                if (line > this.MaxNumberOfLines)
                                {
                                    newSize.Width = fontHeight * this.MaxNumberOfLines + borderSizeLeft + borderSizeRight;

                                    if (this.Report.IsWpf && (!CheckAllowHtmlTags()))
                                        newSize.Width *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorWpf;

                                    else if (textQuality == StiTextQuality.Standard)
                                        newSize.Width *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorStandard;

                                    else if (textQuality == StiTextQuality.Typographic || (this.Report.IsWpf && (!CheckAllowHtmlTags())))
                                        newSize.Width *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorTypographic;

                                    else if (textQuality == StiTextQuality.Wysiwyg || CheckAllowHtmlTags())
                                        newSize.Width *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorWysiwyg;
                                }
                            }
                        }
                    }
                    #endregion

                    #region Other Angles
                    else
                    {
                        if (actualSize.Height > newSize.Height)
                        {
                            newSize.Height = actualSize.Height;
                            if (this.MaxNumberOfLines > 0)
                            {
                                double fontHeight = this.Font.GetHeight() * StiScale.System;
                                var line = (int)(newSize.Height / fontHeight);
                                if (line > this.MaxNumberOfLines)
                                {
                                    newSize.Height = fontHeight * this.MaxNumberOfLines + borderSizeTop + borderSizeBottom;

                                    if (this.Report.IsWpf && (!CheckAllowHtmlTags()))
                                        newSize.Height *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorWpf;

                                    else if (textQuality == StiTextQuality.Standard)
                                        newSize.Height *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorStandard;

                                    else if (textQuality == StiTextQuality.Typographic)
                                        newSize.Height *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorTypographic;

                                    else if (textQuality == StiTextQuality.Wysiwyg || CheckAllowHtmlTags())
                                        newSize.Height *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorWysiwyg;
                                }
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region CanShrink
                if (this.CanShrink)
                {
                    #region this.Angle == 90 || this.Angle == 270
                    if (angle == 90 || angle == 270)
                    {
                        if (this.GetTextInternal() == null || this.GetTextInternal().Trim().Length == 0)
                            newSize.Width = 0;
                        else
                            newSize.Width = Math.Min(actualSize.Width, newSize.Width);
                    }
                    #endregion

                    #region Other Angles
                    else
                    {
                        if (this.GetTextInternal() == null || this.GetTextInternal().Trim().Length == 0)
                            newSize.Height = 0;
                        else
                            newSize.Height = Math.Min(actualSize.Height, newSize.Height);
                    }
                    #endregion
                }
                #endregion

                var size = unit.ConvertFromHInches(newSize);

                #region Size correction for preventing rounding error
                if (angle == 90 || angle == 270)
                {
                    if (autoWidth && (unit.ConvertToHInches(Math.Round(size.Height, 2)) < newSize.Height)) size.Height += 0.01;
                    if ((CanGrow || CanShrink) && (unit.ConvertToHInches(Math.Round(size.Width, 2)) < newSize.Width)) size.Width += 0.01;
                }
                else
                {
                    if ((CanGrow || CanShrink) && (unit.ConvertToHInches(Math.Round(size.Height, 2)) < newSize.Height)) size.Height += 0.01;
                    if (autoWidth && (unit.ConvertToHInches(Math.Round(size.Width, 2)) < newSize.Width)) size.Width += 0.01;
                }
                #endregion

                if (hashCheckSize != null)
                    hashCheckSize[this] = size;

                return size;
            }
            else
                return new SizeD(this.Width, this.Height);
        }
        #endregion

        #region StiService override
        public override void PackService()
        {
            base.PackService();

            textFormat = null;
        }
        #endregion

        #region StiComponent override
        /// <summary>
		/// Returns events collection of this component.
		/// </summary>
		public override StiEventsCollection GetEvents()
        {
            var events = base.GetEvents();

            if (GetExcelValueEvent != null)
                events.Add(GetExcelValueEvent);

            return events;
        }

        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.Text;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory => StiLocalization.Get("Report", "Components");

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiText");
        #endregion

        #region Render.Main
        /// <summary>
        /// Prepares the component for the rendering process.
        /// </summary>
        public override void Prepare()
        {
            base.Prepare();

            if (Conditions == null) return;

            foreach (StiBaseCondition condition in Conditions)
            {
                var indicatorCondition = condition as IStiIndicatorCondition;
                indicatorCondition?.Reset();
            }
        }
        #endregion

        #region Expression
        #region ExcelValue
        /// <summary>
		/// Gets or sets excel data value.
		/// </summary>
		[Browsable(false)]
        [Description("Gets or sets excel data value.")]
        public string ExcelDataValue { get; set; }

        /// <summary>
		/// Gets or sets an expression used for export data to Excel. Only for numeric values.
		/// </summary>
		[StiCategory("Export")]
        [StiOrder(StiPropertyOrder.ExportExcelValue)]
        [StiSerializable]
        [Description("Gets or sets an expression used for export data to Excel. Only for numeric values.")]
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

        #region Events
        /// <summary>
        /// Invokes all events for this components.
        /// </summary>
        public override void InvokeEvents()
        {
            base.InvokeEvents();

            try
            {
                #region GetExcelValue
                if (this.Events[EventGetExcelValue] != null)
                {
                    var e = new StiGetExcelValueEventArgs();
                    e.Value = this.ExcelDataValue;
                    InvokeGetExcelValue(this, e);
                    this.ExcelDataValue = e.Value;
                }

                if (Report?.CalculationMode == StiCalculationMode.Interpretation)
                {
                    if (this.ExcelValue.Value.Length > 0)
                    {
                        var parserResult = StiParser.ParseTextValue(this.ExcelValue.Value, this);
                        this.ExcelDataValue = parserResult != null ? parserResult.ToString() : null;
                    }
                }

                if (!string.IsNullOrEmpty(this.ExcelDataValue))
                {
                    if (HideZeros)
                    {
                        decimal value;
                        if (decimal.TryParse(this.ExcelDataValue, out value))
                        {
                            if (value == 0)
                                this.ExcelDataValue = null;
                        }
                        else
                        {
                            Report?.WriteToReportRenderingMessages($"{Name} FormatException");
                        }
                    }

                    if (((TextFormat as StiNumberFormatService)?.State & StiTextFormatState.Abbreviation) > 0) this.ExcelDataValue = null;
                }
                #endregion
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "DoEvents...ERROR");
                StiLogService.Write(this.GetType(), e);

                Report?.WriteToReportRenderingMessages($"{Name} {e.Message}");
            }
        }

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
                handler?.Invoke(sender, e);

                StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetExcelValueEvent, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in ExcelValue property of '{Name}' can't be evaluated!";
                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);
                Report?.WriteToReportRenderingMessages(str);
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

        #region Properties
        protected static object PropertyNullValue = new object();
        /// <summary>
        /// Gets or sets a value which shows instead null values.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [DefaultValue("")]
        [Description("Gets or sets a value which shows instead null values.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public string NullValue
        {
            get
            {
                return Properties.Get(PropertyNullValue, string.Empty) as string;
            }
            set
            {
                Properties.Set(PropertyNullValue, value, string.Empty);
            }
        }

        protected static object PropertyType = new object();
        [StiSerializable]
        [DefaultValue(StiSystemTextType.None)]
        [Browsable(false)]
        public StiSystemTextType Type
        {
            get
            {
                return (StiSystemTextType)Properties.Get(PropertyType, StiSystemTextType.None);
            }
            set
            {
                Properties.Set(PropertyType, value, StiSystemTextType.None);
            }
        }

        /// <summary>
        /// Gets or sets word wrap.
        /// </summary>
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets word wrap.")]
        [StiOrder(StiPropertyOrder.TextWordWrap)]
        [StiCategory("TextAdditional")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool WordWrap
        {
            get
            {
                return TextOptions.WordWrap;
            }
            set
            {
                TextOptions.WordWrap = value;
            }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets horizontal output direction.")]
        [StiOrder(StiPropertyOrder.TextRightToLeft)]
        [StiCategory("TextAdditional")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool RightToLeft
        {
            get
            {
                return TextOptions.RightToLeft;
            }
            set
            {
                TextOptions.RightToLeft = value;
            }
        }

        [Browsable(false)]
        [DefaultValue(StringTrimming.None)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets type to trim the end of a line.")]
        [StiOrder(StiPropertyOrder.TextTrimming)]
        [StiCategory("TextAdditional")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual StringTrimming Trimming
        {
            get
            {
                return TextOptions.Trimming;
            }
            set
            {
                TextOptions.Trimming = value;
            }
        }

        /// <summary>
        /// Gets or sets angle of a text rotation.
        /// </summary>
        [DefaultValue(0f)]
        [StiOrder(StiPropertyOrder.TextAngle)]
        [Description("Gets or sets angle of a text rotation.")]
        [StiCategory("TextAdditional")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual float Angle
        {
            get
            {
                return TextOptions.Angle;
            }
            set
            {
                TextOptions.Angle = value;
            }
        }

        protected static object PropertyLineSpacing = new object();
        /// <summary>
        /// Gets or sets line spacing of a text
        /// </summary>
        [DefaultValue(1f)]
        [StiSerializable]
        [StiOrder(StiPropertyOrder.TextLineSpacing)]
        [Description("Gets or sets line spacing of a text.")]
        [StiCategory("TextAdditional")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual float LineSpacing
        {
            get
            {
                return Properties.GetFloat(PropertyLineSpacing, 1f);
            }
            set
            {
                if (value > 0)
                    Properties.SetFloat(PropertyLineSpacing, value, 1f);
            }
        }

        protected static object PropertyExportAsImage = new object();
        /// <summary>
        /// Gets or sets value which indicates how content of text will be exported as image or as text.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Export")]
        [StiOrder(StiPropertyOrder.ExportExportAsImage)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates how content of text will be exported as image or as text.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool ExportAsImage
        {
            get
            {
                return Properties.GetBool(PropertyExportAsImage, false);
            }
            set
            {
                Properties.SetBool(PropertyExportAsImage, value, false);
            }
        }

        protected static object PropertyTextQuality = new object();
        /// <summary>
        /// Gets or sets value that indicates quality of text.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiTextQuality.Standard)]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextTextQuality)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets value that indicates quality of text.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiTextQuality TextQuality
        {
            get
            {
                if (!StiOptions.Engine.FullTrust)
                    return StiTextQuality.Typographic;

                return (StiTextQuality)Properties.Get(PropertyTextQuality, StiTextQuality.Standard);
            }
            set
            {
                Properties.Set(PropertyTextQuality, value, StiTextQuality.Standard);
            }
        }

        protected static object PropertyAllowHtmlTags = new object();
        /// <summary>
        /// Gets or sets value that indicates that this component allow Html tags in text.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextAllowHtmlText)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value that indicates that this component allow Html tags in text.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool AllowHtmlTags
        {
            get
            {
                return Properties.GetBool(PropertyAllowHtmlTags, false);
            }
            set
            {
                Properties.SetBool(PropertyAllowHtmlTags, value, false);

                if (value)
                    this.TextQuality = StiTextQuality.Wysiwyg;
            }
        }

        /// <summary>
        /// Gets or sets text margins.
        /// </summary>
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextMargins)]
        [Description("Gets or sets text margins.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiMargins Margins { get; set; } = new StiMargins();

        private bool ShouldSerializeMargins()
        {
            return !Margins.IsDefault;
        }

        /// <summary>
        /// Gets or sets a value to exceed margins of the component background.
        /// </summary>
        [DefaultValue(StiExceedMargins.None)]
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextExceedMargins)]
        [TypeConverter(typeof(StiExceedMarginsConverter))]
        [Description("Gets or sets a value to exceed margins of the component background.")]
        [Editor("Stimulsoft.Report.Components.Design.StiExceedMarginsEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiExceedMargins ExceedMargins { get; set; } = StiExceedMargins.None;

        /// <summary>
		/// Gets a value which indicates whether it is necessary to draw again the whole page when moving the component or
		/// changing its sizes in the designer.
		/// </summary>
		[Browsable(false)]
        public override bool ForceRedrawAll => ExceedMargins != StiExceedMargins.None;

        protected static object PropertyShrinkFontToFit = new object();
        /// <summary>
        /// Gets or sets value that indicates that this component is descrease size of font to fit content of component.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextShrinkFontToFit)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value that indicates that this component is descrease size of font to fit content of component.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool ShrinkFontToFit
        {
            get
            {
                return Properties.GetBool(PropertyShrinkFontToFit, false);
            }
            set
            {
                Properties.SetBool(PropertyShrinkFontToFit, value, false);
            }
        }

        protected static object PropertyShrinkFontToFitMinimumSize = new object();
        /// <summary>
        /// Gets or sets value that indicates minimum font size for ShrinkFontToFit operation.
        /// </summary>
        [StiSerializable]
        [DefaultValue(1f)]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextShrinkFontToFitMinimumSize)]
        [Description("Gets or sets value that indicates minimum font size for ShrinkFontToFit operation.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual float ShrinkFontToFitMinimumSize
        {
            get
            {
                return Properties.GetFloat(PropertyShrinkFontToFitMinimumSize, 1f);
            }
            set
            {
                if (value >= 1f)
                    Properties.SetFloat(PropertyShrinkFontToFitMinimumSize, value, 1f);
            }
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiText();
        }
        #endregion

        #region Methods
        internal bool CheckAllowHtmlTags()
        {
            if (!StiOptions.Engine.UseNewHtmlEngine && !StiOptions.Engine.FullTrust && Report != null && !Report.IsSerializing)
                return false;

            return AllowHtmlTags;
        }

        public Font GetActualFont(string text)
        {
            return GetActualFont(text, 1);
        }

        public Font GetActualFont(string text, float minFontSize)
        {
            var rect = Page.Unit.ConvertToHInches(this.ClientRectangle);

            rect = ConvertTextMargins(rect, false);
            rect = ConvertTextBorders(rect, false);
            rect.Width -= this.Border.Size;

            var textWidth = (this.Angle == 90 || this.Angle == 270) ? rect.Height : rect.Width;

            var thisWidth = this.Page.Unit.ConvertToHInches(this.Width) - this.Margins.Left - this.Margins.Right;
            var thisHeight = this.Page.Unit.ConvertToHInches(this.Height) - this.Margins.Top - this.Margins.Bottom;

            var actualFont = this.font;
            var fnt = this.font;
            var resAngle = this.Angle;

            while (true)
            {
                SizeD size;

                if (this.Report.IsWpf && !CheckAllowHtmlTags())
                {
                    this.Angle = 0;
                    size = StiWpfTextRender.MeasureString(textWidth, this);

                    if (resAngle == 90 || resAngle == 270)
                    {
                        var temp = size.Width;
                        size.Width = size.Height;
                        size.Height = temp;
                    }
                    this.Angle = resAngle;
                }
                else if (TextQuality == StiTextQuality.Wysiwyg || CheckAllowHtmlTags())
                {
                    size = StiWysiwygTextRender.MeasureString(textWidth, fnt, this);
                }
                else if (TextQuality == StiTextQuality.Typographic)
                {
                    size = StiTypographicTextRender.MeasureString(textWidth, fnt, this);
                }
                else size = StiStandardTextRenderer.MeasureString(textWidth, fnt, this);

                if ((size.Width > thisWidth) || (size.Height > thisHeight))
                {
                    var fontSize = fnt.Size;
                    if (fontSize <= minFontSize) break;

                    if (actualFont != fnt)
                        fnt.Dispose();

                    fnt = StiFontUtils.ChangeFontSize(this.font, fontSize - 0.5f);
                    this.Font = fnt;
                }
                else break;
            }

            this.font = actualFont;
            return fnt;
        }
        #endregion

        /// <summary>
        /// Creates a new StiText.
        /// </summary>
        public StiText() : this(RectangleD.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Creates a new StiText.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiText(RectangleD rect) : this(rect, string.Empty)
        {
        }

        /// <summary>
        /// Creates a new StiText.
        /// </summary>
        /// <param name="rect">The rectangle describes sizes and position of the component.</param>
        /// <param name="text">Text expression.</param>
        public StiText(RectangleD rect, string text) : base(rect)
        {
            SetTextInternal(text);
            PlaceOnToolbox = true;

            TextQuality = StiOptions.Engine.DefaultTextQualityMode;
        }
    }
}
