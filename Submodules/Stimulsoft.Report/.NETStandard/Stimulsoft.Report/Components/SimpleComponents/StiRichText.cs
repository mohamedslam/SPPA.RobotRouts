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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.QuickButtons;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Controls;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Helpers;
using System.Collections;
using Stimulsoft.Report.Export;
using System.Linq;
using Stimulsoft.Report.Export.Tools;
using Stimulsoft.Report.Engine;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// The class describes a component that allows to outtype RichText.
    /// </summary>
    [StiServiceBitmap(typeof(StiRichText), "Stimulsoft.Report.Images.Components.StiRichText.png")]
    [StiToolbox(true)]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiShift))]
    [StiContextTool(typeof(IStiEditable))]
    [StiContextTool(typeof(IStiGrowToHeight))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiContextTool(typeof(IStiBreakable))]
    [StiContextTool(typeof(IStiOnlyText))]
    [StiContextTool(typeof(IStiTextOptions))]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiRichTextDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfRichTextDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiRichTextGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiRichTextWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiRichTextQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfRichTextQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiRichText :
        StiSimpleText,
        IStiExportImageExtended,
        IStiBreakable,
        IStiBorder,
        IStiGlobalizationProvider,
        IStiBackColor
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("GlobalizedName");
            jObject.RemoveProperty("LinesOfUnderline");
            jObject.RemoveProperty("HideZeros");
            jObject.RemoveProperty("ProcessingDuplicates");
            jObject.RemoveProperty("MaxNumberOfLines");

            // StiRichText
            jObject.AddPropertyBool("CanBreak", CanBreak);
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyJObject("GetDataUrlEvent", GetDataUrlEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Margins", Margins.SaveToJsonObject(0, 0, 0, 0));
            jObject.AddPropertyBool("WordWrap", WordWrap, true);
            jObject.AddPropertyBool("DetectUrls", DetectUrls, true);
            jObject.AddPropertyColor("BackColor", BackColor, Color.White);
            jObject.AddPropertyStringNullOrEmpty("DataColumn", DataColumn);
            jObject.AddPropertyJObject("DataUrl", DataUrl.SaveToJsonObject(mode));
            jObject.AddPropertyBool("FullConvertExpression", FullConvertExpression);
            jObject.AddPropertyBool("Wysiwyg", Wysiwyg);
            jObject.AddPropertyBool("RightToLeft", RightToLeft);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "CanBreak":
                        this.CanBreak = property.DeserializeBool();
                        break;

                    case "Border":
                        this.Border = property.DeserializeBorder();
                        break;

                    case "GetDataUrlEvent":
                        {
                            var _event = new StiGetDataUrlEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetDataUrlEvent = _event;
                        }
                        break;

                    case "Margins":
                        this.Margins.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "WordWrap":
                        this.WordWrap = property.DeserializeBool();
                        break;

                    case "DetectUrls":
                        this.DetectUrls = property.DeserializeBool();
                        break;

                    case "BackColor":
                        this.BackColor = property.DeserializeColor();
                        break;

                    case "DataColumn":
                        this.dataColumn = property.DeserializeString();
                        break;

                    case "DataUrl":
                        {
                            var _expression = new StiDataUrlExpression();
                            _expression.LoadFromJsonObject((JObject)property.Value);
                            this.DataUrl = _expression;
                        }
                        break;

                    case "FullConvertExpression":
                        this.FullConvertExpression = property.DeserializeBool();
                        break;

                    case "Wysiwyg":
                        this.Wysiwyg = property.DeserializeBool();
                        break;

                    case "RightToLeft":
                        this.RightToLeft = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiRichText;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.RichTextDesign()
            });

            if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.DetectUrls(),
                    propHelper.Editable(),
                    propHelper.Margins(),
                    propHelper.OnlyText(),
                    propHelper.RightToLeft(),
                    propHelper.WordWrap(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.DetectUrls(),
                    propHelper.Editable(),
                    propHelper.FullConvertExpression(),
                    propHelper.Margins(),
                    propHelper.OnlyText(),
                    propHelper.ProcessAt(),
                    propHelper.RightToLeft(),
                    propHelper.WordWrap(),
                    propHelper.Wysiwyg(),
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
            
            if (level != StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
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
                    propHelper.UseParentStyles(),
                });
            }

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
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
                        StiPropertyEventId.GetDataUrlEvent,
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                        StiPropertyEventId.GetValueEvent
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/report_internals_rich_text_output.htm";
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

#if NETSTANDARD
            return true;
#else
            if (string.IsNullOrEmpty(this.GetTextInternal()))
                return true;

            if (IsReportWpf)
            {
                var type = Type.GetType("Stimulsoft.Report.Wpf.StiWpfBreakTextHelper, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo);
                if (type == null)
                    throw new Exception("Assembly 'Stimulsoft.Report.Wpf' is not found");

                var helper = StiActivator.CreateObject(type, new object[0]) as IStiWpfBreakTextHelper;

                AppDomain domain = null;
                if (StiOptions.Engine.RenderRichTextInOtherDomain)
                {
                    if ((dividedComponent.Report != null) && (dividedComponent.Report.WpfRichTextDomain != null))
                        domain = dividedComponent.Report.WpfRichTextDomain;

                    if (domain == null)
                    {
#if NETCOREAPP
                        domain = AppDomain.CreateDomain(StiGuidUtils.NewGuid());
#else
                        var appDomainSetup = new AppDomainSetup();
                        appDomainSetup.ShadowCopyFiles = "false";
                        appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

                        domain = AppDomain.CreateDomain(StiGuidUtils.NewGuid(), AppDomain.CurrentDomain.Evidence, appDomainSetup);
#endif
                    }

                    if (dividedComponent.Report != null)
                        dividedComponent.Report.WpfRichTextDomain = domain;
                }

                return helper.BreakRichText(this, dividedComponent, devideFactor, ref divideLine, domain);
            }

            if (StiOptions.Engine.ForceLockRichTextThread)
            {
                lock (lockRichTextThread)
                {
                    return BreakWin(dividedComponent);
                }
            }
            else
            {
                return BreakWin(dividedComponent);
            }
#endif
        }

        private bool BreakWin(StiComponent dividedComponent)
        {
            var rect = Report.Unit.ConvertToHInches(this.ClientRectangle);
            if (rect.Width < 0)
                rect.Width = Math.Abs(rect.Width);  //fix infinity loops in some cases

            rect = ConvertTextMargins(rect, false);

            int charEnd = 0;

            #region	Calculate count of char measured in rectangle
            var fail = true;
            var failCount = 0;

            var getRtfText = this.RtfText;

            while (fail && failCount < 100)
            {
                fail = false;

                try
                {
                    using (StiRichTextBox richTextBox = new StiRichTextBox(false))
                    {
                        richTextBox.WordWrap = this.WordWrap;
                        richTextBox.DetectUrls = this.DetectUrls;

                        if (this.RightToLeft)
                            richTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

                        GetPreparedText(richTextBox);

                        if (richTextBox.TextLength > 0)
                        {
                            int rectWidth = (int)rect.Width;
                            if (!WordWrap)
                                rectWidth = 100000;
                            int heightRtf = 0;

                            if (Wysiwyg || StiDpiHelper.NeedGraphicsRichTextScale)
                            {
                                lock (StiReport.GlobalRichTextMeasureGraphics)
                                {
                                    heightRtf = StiRichTextHelper.FormatRange(StiRtfFormatType.MeasureRtf, richTextBox,
                                        new Rectangle(0, 0, rectWidth, (int)rect.Height),
                                        StiReport.GlobalRichTextMeasureGraphics, 0, -1, out charEnd);
                                }
                            }
                            else
                            {
                                using (Graphics measureGraph = Graphics.FromHwnd(IntPtr.Zero))
                                {
                                    heightRtf = StiRichTextHelper.FormatRange(StiRtfFormatType.MeasureRtf, richTextBox,
                                        new Rectangle(0, 0, rectWidth, (int)rect.Height),
                                        measureGraph, 0, -1, out charEnd);
                                }
                            }

                            if ((heightRtf > (int)rect.Height) && (charEnd > 0) && !StiOptions.Engine.ForceRtfBreak)
                            {
                                //if assembly manifest doesn't explicitly state that your exe assembly is compatible with Windows 8.1 and Windows 10.0, 
                                //System.Environment.OSVersion will return Windows 8 version, which is 6.2, instead of 6.3 and 10.0!
                                //var winVersion = Environment.OSVersion.Version;
                                //if ((winVersion.Major < 6) || (winVersion.Major == 6 && winVersion.Minor < 2))
                                //{
                                    #region Check resulting height, fix for Win7 and earlier
                                    richTextBox.SelectionStart = 0;
                                    richTextBox.SelectionLength = charEnd;
                                    richTextBox.Rtf = richTextBox.SelectedRtf;

                                    try
                                    {
                                        if (Wysiwyg || StiDpiHelper.NeedGraphicsRichTextScale)
                                        {
                                            lock (StiReport.GlobalRichTextMeasureGraphics)
                                            {
                                                int tempCharEnd = 0;
                                                heightRtf = StiRichTextHelper.FormatRange(StiRtfFormatType.MeasureRtf, richTextBox,
                                                    new Rectangle(0, 0, rectWidth, (int)rect.Height),
                                                    StiReport.GlobalRichTextMeasureGraphics, 0, -1, out tempCharEnd);
                                            }
                                        }
                                        else
                                        {
                                            using (Graphics measureGraph = Graphics.FromHwnd(IntPtr.Zero))
                                            {
                                                int tempCharEnd = 0;
                                                heightRtf = StiRichTextHelper.FormatRange(StiRtfFormatType.MeasureRtf, richTextBox,
                                                    new Rectangle(0, 0, rectWidth, (int)rect.Height),
                                                    measureGraph, 0, -1, out tempCharEnd);
                                            }
                                        }

                                        if (heightRtf > (int)rect.Height)
                                        {
                                            int pos1 = 0;
                                            int pos2 = (int)rect.Height;
                                            int pos = (int)(pos2 * (2 - heightRtf / (double)pos2));
                                            if (pos < pos2 / 2) pos = pos2 / 2;

                                            int lastHeightRtf = 0;
                                            int lastCharEnd = 0;
                                            int tempCharEnd = 0;

                                            while ((pos2 - pos1) > 5)
                                            {
                                                if (Wysiwyg || StiDpiHelper.NeedGraphicsRichTextScale)
                                                {
                                                    lock (StiReport.GlobalRichTextMeasureGraphics)
                                                    {
                                                        heightRtf = StiRichTextHelper.FormatRange(StiRtfFormatType.MeasureRtf, richTextBox,
                                                            new Rectangle(0, 0, rectWidth, pos),
                                                            StiReport.GlobalRichTextMeasureGraphics, 0, -1, out tempCharEnd);
                                                    }
                                                }
                                                else
                                                {
                                                    using (Graphics measureGraph = Graphics.FromHwnd(IntPtr.Zero))
                                                    {
                                                        heightRtf = StiRichTextHelper.FormatRange(StiRtfFormatType.MeasureRtf, richTextBox,
                                                            new Rectangle(0, 0, rectWidth, pos),
                                                            measureGraph, 0, -1, out tempCharEnd);
                                                    }
                                                }
                                                if (heightRtf > rect.Height)
                                                {
                                                    pos2 = pos;
                                                }
                                                else
                                                {
                                                    pos1 = pos;
                                                    lastCharEnd = tempCharEnd;
                                                }
                                                pos = (pos1 + pos2) / 2;
                                                if (lastHeightRtf == heightRtf) break;
                                                lastHeightRtf = heightRtf;
                                            }
                                            charEnd = lastCharEnd;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Report?.WriteToReportRenderingMessages($"{Name} {e.Message}");
                                    }
                                    #endregion
                                //}
                                //else
                                //{
                                //    //usually, this happens only if even the first line does not fit
                                //    //therefore nothing to fit
                                //    charEnd = 0;
                                //}
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    fail = true;
                    failCount++;

                    Report?.WriteToReportRenderingMessages($"{Name} {e.Message}");
                }
            }

            if (charEnd == -1)
                charEnd = 0;
            #endregion

            #region Break richtext
            using (var rich = new StiRichTextBox(false))
            {
                if (getRtfText != null && getRtfText.TrimStart().ToLowerInvariant().IndexOf("rtf", StringComparison.InvariantCulture) != -1)
                {
                    rich.Rtf = StiExportUtils.CorrectRichTextForRiched20(getRtfText);
                }
                else
                {
                    rich.Text = this.GetTextInternal();

                    if (StiOptions.Engine.DefaultRtfFont != null)
                    {
                        rich.SelectAll();
                        rich.SelectionFont = StiOptions.Engine.DefaultRtfFont;
                    }
                    else if (DefaultFont != null)
                    {
                        rich.SelectAll();
                        rich.SelectionFont = DefaultFont;
                    }

                    if (!DefaultColor.IsEmpty)
                    {
                        rich.SelectAll();
                        rich.SelectionColor = DefaultColor;
                    }
                }

                rich.SelectionStart = 0;
                rich.SelectionLength = charEnd;

                if (StiOptions.Engine.RtfCache.Enabled)
                    this.NewGuid();

                this.RtfText = rich.SelectedRtf;

                //если в конце текста есть пустые строки, то размер последней части текста вычисляется меньше чем надо,
                //метод думает, что весь текст помещается в существующий контейнер,
                //срабатывает это условие, и при существующей реализации метода BreakContainer получается бесконечное зацикливание,
                //поэтому эти строки пока закомментированы
                //
                //if (!this.GrowToHeight)
                //{
                //    if (rich.TextLength - charEnd == 0) return false;
                //}

                rich.SelectionStart = charEnd;
                rich.SelectionLength = Math.Max(rich.TextLength - charEnd, 0);

                if (StiOptions.Engine.RtfCache.Enabled)
                    dividedComponent.NewGuid();

                string rtf = rich.SelectedRtf;

                #region Check bug of list breaking - font size missing in begin of first line
                Regex regex1 = new Regex(@"\\f\d{1,2}\s");
                Regex regex2 = new Regex(@"\\fs\d{1,3}");

                var m1 = regex1.Match(rtf);
                if (m1 != null && m1.Success)
                {
                    var m2 = regex2.Match(rtf);
                    if (m2 != null && m2.Success && (m2.Index - m1.Index > 25))
                    {
                        rtf = rtf.Insert(m1.Index, m2.Value);
                    }
                }

                //check last line - missing \par
                int pos1 = rtf.LastIndexOf(@"\par");
                int pos2 = rtf.LastIndexOf(@"}");
                if (pos1 < rtf.Length - 15)
                {
                    rtf = rtf.Insert(pos2, @"\par");
                }
                #endregion

                ((StiRichText)dividedComponent).RtfText = rtf;
                //((StiRichText)dividedComponent).RtfText = rtf;
            }
            #endregion

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
            var text = (StiRichText)base.Clone(cloneProperties);

            if (this.Border != null)
                text.Border = (StiBorder)this.Border.Clone();
            else
                text.Border = null;

            return text;
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
                ToolTip.Value = value;

            else if (propertyName == "Tag")
                Tag.Value = value;

            else if (propertyName == "Hyperlink")
                Hyperlink.Value = value;

            else
                throw new ArgumentException($"Property with name {propertyName}");
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

        #region IStiBorder
        /// <summary>
        /// The appearance and behavior of the component border.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiSerializable]
        [Description("The appearance and behavior of the component border.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBorder Border { get; set; } = new StiBorder();

        private bool ShouldSerializeBorder()
        {
            return Border == null || !Border.IsDefault;
        }
        #endregion

        #region IStiBackColor
        /// <summary>
        /// The background color of the component.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiExpressionColorConverter))]
        [Editor(StiEditors.ExpressionColor, typeof(UITypeEditor))]
        [Description("The background color of the component.")]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBackColor)]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiExpressionAllowed]
        public Color BackColor { get; set; } = Color.White;

        private bool ShouldSerializeBackColor()
        {
            return BackColor != Color.White;
        }
        #endregion

        #region IStiExportImageExtended
        public virtual Image GetImage(ref float zoom)
        {
            return GetImage(ref zoom, StiExportFormat.None);
        }

        public virtual Image GetImage(ref float zoom, StiExportFormat format)
        {
            bool flag = Image == null;

            StiGuiMode guiMode;
            if (Report != null)
            {
                guiMode = IsReportWpf || Report.RenderedWith == StiRenderedWith.Wpf ? StiGuiMode.Wpf : StiGuiMode.Gdi;
            }
            else
            {
                guiMode = (StiOptions.Configuration.IsWPF ? StiGuiMode.Wpf : StiGuiMode.Gdi);
            }

            var painter = StiPainter.GetPainter(this.GetType(), guiMode);
            var tempImage = painter.GetImage(this, ref zoom, format);

            if (flag)
            {
                Image?.Dispose();

                ResetImage();
            }

            return tempImage;
        }

        [Browsable(false)]
        public override bool IsExportAsImage(StiExportFormat format)
        {
            if ((format == StiExportFormat.Text) ||
                (format == StiExportFormat.Rtf) ||
                (format == StiExportFormat.RtfFrame) ||
                (format == StiExportFormat.RtfTable) ||
                (format == StiExportFormat.RtfWinWord))
                return false;

            return true;
        }
        #endregion

        #region IStiText browsable(false)
        [Browsable(false)]
        [StiNonSerialized]
        public sealed override string GlobalizedName
        {
            get
            {
                return base.GlobalizedName;
            }
            set
            {
                base.GlobalizedName = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override StiPenStyle LinesOfUnderline
        {
            get
            {
                return base.LinesOfUnderline;
            }
            set
            {
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool HideZeros
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override StiProcessingDuplicatesType ProcessingDuplicates
        {
            get
            {
                return StiProcessingDuplicatesType.None;
            }
            set
            {
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override int MaxNumberOfLines
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        [Editor("Stimulsoft.Report.Components.Design.StiRichTextExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiRichTextExpressionConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        public override StiExpression Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
        #endregion

        #region IStiGetFonts
        public override List<StiFont> GetFonts()
        {
            var result = base.GetFonts();
            try
            {
                var service = new StiPdfExportService
                {
                    report = this.Report
                };
                service.InitPdfFonts();

                var mfRender = new StiPdfMetafileRender(service, true);
                mfRender.AssembleRtf(this);

                var pdfFonts = mfRender.GetPdfFonts();
                foreach (var font in pdfFonts.fontList)
                {
                    var infoFont = (PdfFonts.pfontInfo)font;
                    result.Add(new StiFont(infoFont.Font));
                }

            }
            catch
            {
            }
            return result.Distinct().ToList();
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.RichText;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiRichText");

        /// <summary>
        /// Gets a localized name of the component category.
        /// </summary>
        public override string LocalizedCategory => StiLocalization.Get("Report", "Components");
        #endregion

        #region Paint
        public RectangleD ConvertTextMargins(RectangleD rect, bool convert)
        {
            if (Margins.IsEmpty)
                return rect;

            var zoom = Page.Zoom;

            var marginsLeft = Margins.Left;
            var marginsRight = Margins.Right;
            var marginsTop = Margins.Top;
            var marginsBottom = Margins.Bottom;

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

        [Browsable(false)]
        public Image Image { get; set; }

        public void RenderMetafile()
        {
            bool fail = RenderMetafile(true);

            //check for problems with graphics on some systems
            if (fail && Image == null && Wysiwyg)
            {
                lock (StiReport.GlobalRichTextMeasureGraphics)
                {
                    var bmp = new Bitmap(1, 1);
                    bmp.SetResolution((float)(96 * StiDpiHelper.GraphicsRichTextScale), (float)(96 * StiDpiHelper.GraphicsRichTextScale));

                    var gr = Graphics.FromImage(bmp);
                    gr.PageUnit = GraphicsUnit.Display;
                    gr.PageScale = 1f;

                    StiReport.GlobalRichTextMeasureGraphics = gr;
                }
                RenderMetafile(true);
            }
        }

        public bool RenderMetafile(bool useTransparent)
        {
            if (StiOptions.Engine.ForceLockRichTextThread)
            {
                lock (lockRichTextThread)
                    return RenderMetafile2(useTransparent);
            }
            else
            {
                return RenderMetafile2(useTransparent);
            }
        }

        private bool RenderMetafile2(bool useTransparent)
        {
            if (Page == null || Report == null)
                return false;

            bool fail = true;
            int failCount = 0;

            while (fail && failCount < 10)
            {
                fail = false;
                try
                {
                    using (StiRichTextBox richTextBox = new StiRichTextBox(useTransparent && ((this.BackColor.A == 0) || Wysiwyg || StiDpiHelper.NeedGraphicsRichTextScale)))
                    {
                        var text = GetPreparedText(richTextBox);
                        if (string.IsNullOrWhiteSpace(text))
                            return false;

                        var rect = GetPaintRectangle(true, false);
                        rect = ConvertTextMargins(rect, false);

                        if (BackColor.A > 0) //BackColor != Color.Transparent
                            richTextBox.BackColor = BackColor;

                        richTextBox.WordWrap = this.WordWrap;
                        richTextBox.DetectUrls = this.DetectUrls;

                        if (this.RightToLeft)
                        {
                            var backup = StiClipboardHelper.GetClipboardData();

                            richTextBox.SelectAll();
                            richTextBox.Cut();
                            richTextBox.Text = "Temp";
                            richTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                            richTextBox.SelectAll();
                            richTextBox.Paste();

                            StiClipboardHelper.SetClipboardData(backup);
                        }

                        if (Wysiwyg || StiDpiHelper.NeedGraphicsRichTextScale || (StiOptions.Configuration.IsWPF && StiOptions.Engine.RenderRichTextInWpfAlwaysWysiwyg))
                        {
                            lock (StiReport.GlobalRichTextMeasureGraphics)
                            {
                                CreateMetafile(StiReport.GlobalRichTextMeasureGraphics, rect, richTextBox, text);
                            }
                        }
                        else
                        {
                            using (Graphics graph = Graphics.FromHwnd(richTextBox.Handle))
                            {
                                CreateMetafile(graph, rect, richTextBox, text);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    fail = true;
                    failCount++;

                    Report?.WriteToReportRenderingMessages($"{Name} {e.Message}");
                }
            }

            if (fail)
                return true;

            return false;
        }

        private void CreateMetafile(Graphics graph, RectangleD rect, StiRichTextBox richTextBox, string text)
        {
            if (rect.Width < 0)
                rect.Width = 1;

            if (rect.Height < 0)
                rect.Height = 1;

            var rectWidth = (int)rect.Width + 1;
            var rectHeight = (int)rect.Height + 1;

            if (!this.WordWrap)
                rectWidth = 100000;

            var dpiX = graph.DpiX;
            var dpiY = graph.DpiY;

            var ptrGraph = graph.GetHdc();
            Metafile newImage = null;

            try
            {
                if (Wysiwyg || StiDpiHelper.NeedGraphicsRichTextScale || (StiOptions.Configuration.IsWPF && StiOptions.Engine.RenderRichTextInWpfAlwaysWysiwyg))
                {
                    var rw = (float)(rect.Width * dpiX / 95.7f);
                    var rh = (float)(rect.Height * dpiY / 95.7f);

                    if (rw == 0f)
                        rw = 1f;

                    if (rh == 0f)
                        rh = 1f;

                    newImage = new Metafile(
                        ptrGraph,
                        new RectangleF(0, 0, rw, rh),
                        MetafileFrameUnit.Pixel,
                        StiOptions.Engine.RichTextDrawingMetafileType);
                }
                else
                {
                    newImage = new Metafile(ptrGraph, StiOptions.Engine.RichTextDrawingMetafileType);
                }
            }
            finally
            {
                graph.ReleaseHdc(ptrGraph);
            }

            if (!string.IsNullOrEmpty(text))
            {
                using (var imageGraph = Graphics.FromImage(newImage))
                {
                    int endChar = 0;
                    imageGraph.PageUnit = GraphicsUnit.Pixel;
                    StiRichTextHelper.FormatRange(StiRtfFormatType.DrawRtf, richTextBox,
                        new Rectangle(0, 0, rectWidth, rectHeight), imageGraph, 0, text.Length, out endChar);
                }
            }

            Image?.Dispose();
            Image = newImage;

            //RichText images cache
            if (Report != null)
            {
                if (Report.RichTextImageCache == null)
                    Report.RichTextImageCache = new List<StiRichText>();

                int indexInCache = Report.RichTextImageCache.IndexOf(this);
                if (indexInCache == -1)
                    Report.RichTextImageCache.Add(this);

                var richTextImageCacheSize = Report.ReportCacheMode == StiReportCacheMode.On ? 50 : StiOptions.Engine.RichTextImageCacheSize;
                if (Report.RichTextImageCache.Count > richTextImageCacheSize)
                {
                    var comp = Report.RichTextImageCache[0];
                    Report.RichTextImageCache.RemoveAt(0);

                    comp?.ResetImage(true);
                }
            }
        }
        #endregion

        #region IStiGetActualSize
        public override SizeD GetActualSize()
        {
            var tSize = new SizeD(this.Width, this.Height);

            if (this.CanGrow || this.CanShrink)
            {
                Hashtable hashCheckSize = null;
                if (StiOptions.Engine.AllowCacheForGetActualSize)
                {
                    var report = this.Report;
                    if (report?.Engine != null)
                    {
                        if (report.Engine.HashCheckSize == null)
                            report.Engine.HashCheckSize = new Hashtable();

                        var obj = report.Engine.HashCheckSize[this];
                        if (obj != null)
                            return (SizeD)obj;

                        hashCheckSize = report.Engine.HashCheckSize;
                    }
                }

                if (IsReportWpf)
                {
                    var unit = this.Page.Unit;
                    var rect = unit.ConvertToHInches(this.ClientRectangle);

                    var size = StiWpfTextRender.MeasureRtfString(rect.Width, this);
                    tSize = new SizeD(this.Width, unit.ConvertFromHInches(size.Height));
                }
                else
                {
                    if (StiOptions.Engine.FullTrust)
                    {
                        if (StiOptions.Engine.ForceLockRichTextThread)
                        {
                            lock (lockRichTextThread)
                                tSize = GetActualSizeWin();
                        }
                        else
                        {
                            tSize = GetActualSizeWin();
                        }
                    }
                }

                if (hashCheckSize != null)
                    hashCheckSize[this] = tSize;
            }
            return tSize;
        }

        private static object lockRichTextThread = new object();

        private SizeD GetActualSizeWin()
        {
            bool fail = true;
            int failCount = 0;

            while (fail && failCount < 100)
            {
                fail = false;

                try
                {
                    using (var richTextBox = new StiRichTextBox(false))
                    {
                        richTextBox.WordWrap = this.WordWrap;
                        richTextBox.DetectUrls = this.DetectUrls;
                        if (this.RightToLeft)
                            richTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

                        GetPreparedText(richTextBox);

                        if (richTextBox.TextLength == 0)
                            return new SizeD(0, 0);

                        var rect = GetPaintRectangle(true, false);
                        rect = ConvertTextMargins(rect, false);

                        if (richTextBox.TextLength > 0)
                        {
                            int rectWidth = (int)rect.Width;
                            if (!this.WordWrap)
                                rectWidth = 100000;

                            int rtfHeight = 2;
                            if (Wysiwyg || StiDpiHelper.NeedGraphicsRichTextScale)
                            {
                                lock (StiReport.GlobalRichTextMeasureGraphics)
                                {
                                    int index = 0;
                                    while (index < richTextBox.TextLength)
                                    {
                                        int index2 = 0;
                                        int height = 0;
                                        for (int indexTry = 0; indexTry < 10; indexTry++)
                                        {
                                            height = StiRichTextHelper.FormatRange(StiRtfFormatType.TotalRtfHeight, richTextBox,
                                                new Rectangle(0, 0, rectWidth, (int)rect.Height),
                                                StiReport.GlobalRichTextMeasureGraphics, index, -1, out index2);

                                            if (height != 1000000) break;
                                            else height = (int)rect.Height;
                                        }
                                        index = index2;
                                        rtfHeight += height;
                                    }
                                }
                            }
                            else
                            {
                                using (Graphics measureGraph = Graphics.FromHwnd(IntPtr.Zero))
                                {
                                    int index = 0;
                                    while (index < richTextBox.TextLength)
                                    {
                                        int index2 = 0;
                                        int height = 0;
                                        for (int indexTry = 0; indexTry < 10; indexTry++)
                                        {
                                            height = StiRichTextHelper.FormatRange(StiRtfFormatType.TotalRtfHeight, richTextBox,
                                                new Rectangle(0, 0, rectWidth, (int)rect.Height),
                                                measureGraph, index, -1, out index2);

                                            if (height != 1000000) break;
                                            else height = (int)rect.Height;
                                        }
                                        index = index2;
                                        rtfHeight += height;
                                    }
                                }
                            }
                            rtfHeight += (int)(Margins.Top + Margins.Bottom);
                            var newHeight = Report.Unit.ConvertFromHInches((double)rtfHeight);

                            if (Report.Unit.ConvertToHInches(Math.Round(newHeight, 2)) < rtfHeight)
                                newHeight += 0.01d;

                            if (richTextBox.Rtf.Contains("{\\pict"))
                                newHeight *= 1.015;

                            return new SizeD(this.Width, newHeight);
                        }
                    }
                }
                catch (Exception e)
                {
                    fail = true;
                    failCount++;

                    Report?.WriteToReportRenderingMessages($"{Name} {e.Message}");
                }
            }
            return new SizeD(this.Width, this.Height);
        }
        #endregion

        #region Render override
        internal string GetPreparedText(RichTextBox richTextBox)
        {
            if (IsDesigning)
            {
                var text2 = this.GetRtfFromSource(DataUrl != null ? DataUrl.Value : null, DataColumn);
                if (text2 != null)
                {
                    if (StiRtfHelper.IsRtfText(text2))
                        richTextBox.Rtf = StiExportUtils.CorrectRichTextForRiched20(text2);

                    else
                        richTextBox.Text = text2;

                    return text2;
                }
            }

            string textInternal = this.GetTextInternal();
            if (string.IsNullOrEmpty(textInternal))
            {
                if (IsDesigning)
                    return null;

                richTextBox.Text = string.Empty;
            }
            else
            {
                int rtfPos = textInternal.TrimStart().ToLowerInvariant().IndexOf("rtf");
                if (rtfPos != -1 && rtfPos < 30)
                {
                    var tempSt = UnpackRtf(StiRtfHelper.XmlDecodeFast(textInternal));    //fix: replace XmlConvert.DecodeName with StiRtfHelper.XmlDecodeFast
                    richTextBox.Rtf = StiExportUtils.CorrectRichTextForRiched20(tempSt);
                }
                else
                {
                    richTextBox.Text = textInternal;
                    if (StiOptions.Engine.DefaultRtfFont != null)
                    {
                        richTextBox.SelectAll();
                        richTextBox.SelectionFont = StiOptions.Engine.DefaultRtfFont;
                    }
                    else if (DefaultFont != null)
                    {
                        richTextBox.SelectAll();
                        richTextBox.SelectionFont = DefaultFont;
                    }

                    if (!DefaultColor.IsEmpty)
                    {
                        richTextBox.SelectAll();
                        richTextBox.SelectionColor = DefaultColor;
                    }
                }
            }

            richTextBox.WordWrap = true;

            var text = richTextBox.Rtf;

            if (richTextBox.Text == string.Empty)
                richTextBox.Text = " ";

            return text;
        }
        #endregion

        #region Event override
        public override void InvokeGetValue(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                var isInterpretationMode = (Report != null) && (Report.CalculationMode == StiCalculationMode.Interpretation);

                #region GetDataUrl
                string dataUrlValue = null;
                if (isInterpretationMode)
                {
                    if (this.DataUrl.Value.Length > 0)
                    {
                        var parserResult = StiParser.ParseTextValue(this.DataUrl.Value, this);
                        if (parserResult != null)
                            dataUrlValue = Report.ToString(parserResult);
                    }
                }

                if (this.Events[EventGetDataUrl] != null && dataUrlValue == null)
                {
                    var ee = new StiGetDataUrlEventArgs();
                    InvokeGetDataUrl(this, ee);

                    if (ee.Value != null)
                        dataUrlValue = ee.Value.ToString();
                }
                #endregion

                var rtfData = string.Empty;

                try
                {
                    var rtf = GetRtfFromSource(dataUrlValue, DataColumn);
                    if (!string.IsNullOrEmpty(rtf))
                    {
                        if (StiRtfHelper.IsRtfText(rtf))
                            rtfData = PackRtf(rtf);

                        else
                            rtfData = rtf;
                    }
                }
                catch
                {
                    if (!StiOptions.Engine.HideExceptions) throw;
                }

                if (rtfData != null && rtfData.Length > 0)
                    e.Value = rtfData;

                else
                    base.InvokeGetValue(sender, e);

            }
            catch (Exception ex)
            {
                var str = $"Expression in Text property of '{Name}' can't be evaluated!";

                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);

                Report?.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Returns the rtf being get as a result of rendering.
        /// </summary>
        public string GetRtfFromSource(string dataUrlValue, string dataColumn)
        {
            string rtf = GetRtfFromUrl(dataUrlValue);
            if (!string.IsNullOrEmpty(rtf))
                return rtf;

            rtf = GetRtfFromFile(dataUrlValue);
            if (!string.IsNullOrEmpty(rtf))
                return rtf;

            rtf = GetRtfFromDataColumn(dataColumn);
            if (!string.IsNullOrEmpty(rtf))
                return rtf;

            return null;
        }

        /// <summary>
        /// Returns the rtf from specified url.
        /// </summary>
        protected string GetRtfFromUrl(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    if (StiHyperlinkProcessor.IsServerHyperlink(url))
                        return StiStimulsoftServerResource.GetRichText(this, StiHyperlinkProcessor.GetServerNameFromHyperlink(url));

                    var resourceName = StiHyperlinkProcessor.GetResourceNameFromHyperlink(url);
                    if (resourceName != null)
                        return StiHyperlinkProcessor.GetString(Report, url);

                    var variableName = StiHyperlinkProcessor.GetVariableNameFromHyperlink(url);
                    if (variableName != null)
                        return StiHyperlinkProcessor.GetString(Report, url);

                    var cookieContainer = Report?.CookieContainer;
                    return StiRichTextFromURL.LoadRichText(url, cookieContainer);
                }
            }
            catch (Exception ex)
            {
                var str = $"The RTF file can't be loaded from URL '{url}' in richtext component {Name}!";

                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);

                Report?.WriteToReportRenderingMessages(str);
            }
            return null;
        }

        /// <summary>
        /// Returns the rtf from specified path.
        /// </summary>
        protected string GetRtfFromFile(string hyperlink)
        {
            var file = StiHyperlinkProcessor.GetFileNameFromHyperlink(hyperlink);
            if (file == null)
                return null;

            if (File.Exists(file))
            {
                try
                {
                    return File.ReadAllText(file, Encoding.Default);
                }
                catch (Exception ex)
                {
                    var str = $"The RTF can't be loaded from file '{file}' in richtext component {Name}!";

                    StiLogService.Write(this.GetType(), str);
                    StiLogService.Write(this.GetType(), ex.Message);

                    Report.WriteToReportRenderingMessages(str);
                }
            }
            else
            {
                var str = $"The file '{file}' does not exist in richtext component {Name}!";

                StiLogService.Write(this.GetType(), str);
                Report?.WriteToReportRenderingMessages(str);
            }
            return null;
        }

        /// <summary>
        /// Returns the rtf from specified data column.
        /// </summary>
        protected string GetRtfFromDataColumn(string dataColumn)
        {
            try
            {
                var data = StiDataColumn.GetDataFromDataColumn(Report.Dictionary, dataColumn);
                return data is byte[]? Encoding.UTF8.GetString(data as byte[]) : data as string;
            }
            catch (Exception ex)
            {
                var str = $"The RTF can't be loaded from data column '{dataColumn}' in richtext component {Name}!";

                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);

                Report?.WriteToReportRenderingMessages(str);
            }
            return null;
        }
        #endregion

        #region GetDataUrl
        private static readonly object EventGetDataUrl = new object();

        /// <summary>
        /// Occurs when the DataUrl is calculated.
        /// </summary>
        public event StiGetDataUrlEventHandler GetDataUrl
        {
            add
            {
                base.Events.AddHandler(EventGetDataUrl, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventGetDataUrl, value);
            }
        }

        /// <summary>
        /// Raises the GetDataUrl event.
        /// </summary>
        protected virtual void OnGetDataUrl(StiGetDataUrlEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetDataUrl event.
        /// </summary>
        public virtual void InvokeGetDataUrl(StiComponent sender, StiGetDataUrlEventArgs e)
        {
            try
            {
                OnGetDataUrl(e);

                var handler = base.Events[EventGetDataUrl] as StiGetDataUrlEventHandler;
                if (handler != null)
                    handler(sender, e);

                StiBlocklyHelper.InvokeBlockly(this.Report, this, GetDataUrlEvent, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in DataUrl property of '{Name}' can't be evaluated!";

                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);

                Report?.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when the DataUrl is calculated.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when the DataUrl is calculated.")]
        public virtual StiGetDataUrlEvent GetDataUrlEvent
        {
            get
            {
                return new StiGetDataUrlEvent(this);
            }
            set
            {
                value?.Set(this, value.Script);
            }
        }
        #endregion

        #region Methods
        public override StiComponent CreateNew()
        {
            return new StiRichText();
        }

        public override void OnResizeComponent(SizeD oldSize, SizeD newSize)
        {
            ResetImage();
        }
        #endregion

        #region this
        /// <summary>
        /// Gets or sets text margins.
        /// </summary>
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextMargins)]
        [Description("Gets or sets text margins.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiMargins Margins { get; set; } = new StiMargins(0, 0, 0, 0);

        private bool ShouldSerializeMargins()
        {
            return Margins == null || !Margins.IsDefault;
        }

        /// <summary>
        /// Gets or sets default font.
        /// </summary>
        [Browsable(false)]
        public Font DefaultFont { get; set; } = null;

        /// <summary>
        /// Gets or sets default Color.
        /// </summary>
        [Browsable(false)]
        public Color DefaultColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets word wrap.
        /// </summary>
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets word wrap.")]
        [StiOrder(StiPropertyOrder.TextWordWrap)]
        [StiCategory("TextAdditional")]
        [StiShowInContextMenu]
        [StiSerializable]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool WordWrap { get; set; } = true;

        /// <summary>
        /// Gets or sets detection of urls.
        /// </summary>
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets detection of urls.")]
        [StiOrder(StiPropertyOrder.TextDetectUrls)]
        [StiCategory("TextAdditional")]
        [StiShowInContextMenu]
        [StiSerializable]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool DetectUrls { get; set; } = true;

        private string dataColumn = string.Empty;
        /// <summary>
        /// Gets or sets a name of the column that contains the RTF text.
        /// </summary>
        [StiSerializable]
        [Editor("Stimulsoft.Report.Components.Design.StiRichTextExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Text")]
        [StiOrder(StiPropertyOrder.TextDataColumn)]
        [Description("Gets or sets a name of the column that contains the RTF text.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public string DataColumn
        {
            get
            {
                return dataColumn;
            }
            set
            {
                if (dataColumn != value)
                {
                    dataColumn = value;
                    ResetImage();
                }
            }
        }

        /// <summary>
        /// Gets or sets a URL that contains the RTF text.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets a URL that contains the RTF text.")]
        [DefaultValue(null)]
        [StiCategory("Text")]
        [StiOrder(StiPropertyOrder.TextDataUrl)]
        [StiPropertyLevel(StiLevel.Standard)]
        [Editor("Stimulsoft.Report.Components.Design.StiRichTextExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public StiDataUrlExpression DataUrl
        {
            get
            {
                return new StiDataUrlExpression(this, "DataUrl");
            }
            set
            {
                if (value != null)
                {
                    value.Set(this, "DataUrl", value.Value);
                    ResetImage();
                }
            }
        }

        /// <summary>
        /// Gets or sets value which indicates that it is necessary to fully convert the expression to Rtf format. Full convertion of expressions slows down the report rendering.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextFullConvertExpression)]
        [DefaultValue(false)]
        [Description("Gets or sets value which indicates that it is necessary to fully convert the expression to Rtf format. Full convertion of expressions slows down the report rendering.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public bool FullConvertExpression { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that it is necessary to use the Wysiwyg mode of the rendering.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextWysiwyg)]
        [DefaultValue(false)]
        [Description("Gets or sets value which indicates that it is necessary to use the Wysiwyg mode of the rendering.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public bool Wysiwyg { get; set; }

        /// <summary>
        /// Gets or sets horizontal output direction.
        /// </summary>
        //[Browsable(false)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextRightToLeft)]
        [DefaultValue(false)]
        [Description("Gets or sets horizontal output direction.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Pack RTF text for save and compilation.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string PackRtf(string str)
        {
            var sb = new StringBuilder(str);
            var ns = new StringBuilder();

            for (int index = 0; index < sb.Length; index++)
            {
                var c = sb[index];
                if (c == '{')
                {
                    if (index == 0 || sb[index - 1] != '\\')
                    {
                        ns = ns.Append("__LP__");
                        continue;
                    }
                }

                if (c == '}')
                {
                    if (index == 0 || sb[index - 1] != '\\')
                    {
                        ns = ns.Append("__RP__");
                        continue;
                    }
                }
                if (c != 0)
                    ns = ns.Append(c);
            }

            ns = ns.Replace("\\{", "{");
            ns = ns.Replace("\\}", "}");

            return ns.ToString();
        }

        /// <summary>
        /// Unpack RTF text.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnpackRtf(string str)
        {
            //StringBuilder sb = new StringBuilder(str);
            //sb = sb.Replace("{", "\\{");
            //sb = sb.Replace("}", "\\}");
            //sb.Replace("__LP__", "{");
            //sb.Replace("__RP__", "}");
            //return = sb.ToString();

            if (string.IsNullOrEmpty(str))
                return string.Empty;

            StringBuilder sbb = new StringBuilder();
            int index = 0;
            int start = 0;
            while (index < str.Length)
            {
                var ch = str[index];
                if (ch == '{')
                {
                    if (index > start)
                        sbb.Append(str, start, index - start);

                    sbb.Append("\\{");
                    start = index + 1;
                }
                else if (ch == '}')
                {
                    if (index > start)
                        sbb.Append(str, start, index - start);

                    sbb.Append("\\}");
                    start = index + 1;
                }
                else if ((ch == '_') && (index + 5 < str.Length) && (str[index + 1] == '_') && (str[index + 2] == 'L' || str[index + 2] == 'R') && (str[index + 3] == 'P') && (str[index + 4] == '_') && (str[index + 5] == '_'))
                {
                    if (index > start)
                        sbb.Append(str, start, index - start);

                    sbb.Append(str[index + 2] == 'L' ? "{" : "}");

                    index += 5;
                    start = index + 1;
                }
                index++;
            }

            if (index > start)
                sbb.Append(str, start, index - start);

            return sbb.ToString();
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public override string GetTextInternal()
        {
            if (StiOptions.Engine.RtfCache.Enabled && this.Report != null && (!IsDesigning) && this.Guid != null)
            {
                string path = StiRtfCache.GetRtfCacheName(Report.RtfCachePath, this.Guid);

                if (File.Exists(path))
                    return RemovePageBreakTag(StiRtfCache.LoadRtf(path));
            }

            return RemovePageBreakTag(base.GetTextInternal());
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public override void SetTextInternal(string value)
        {
            if (StiOptions.Engine.RtfCache.Enabled && this.Report != null && (!IsDesigning))
            {
                if (string.IsNullOrEmpty(Report.RtfCachePath))
                    Report.RtfCachePath = StiRtfCache.CreateNewCache();

                if (this.Guid == null)
                    this.NewGuid();

                var path = StiRtfCache.GetRtfCacheName(Report.RtfCachePath, this.Guid);

                StiRtfCache.SaveRtf(value, path);
            }
            else
                base.SetTextInternal(value);

        }

        private static string RemovePageBreakTag(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return inputString;

            //check packed rtf
            inputString = RemoveToken(inputString, "_x005C_pagebb");
            inputString = RemoveToken(inputString, "_x005C_page");

            //check unpacked rtf
            inputString = RemoveToken(inputString, "\\pagebb");
            inputString = RemoveToken(inputString, "\\page");

            return inputString;
        }

        private static string RemoveToken(string inputString, string token)
        {
            var offset = 0;

            do
            {
                offset = inputString.IndexOf(token, offset, StringComparison.InvariantCulture);
                if (offset == -1)
                    return inputString;

                int offset2 = offset + token.Length;

                if (!char.IsLetterOrDigit(inputString[offset2]))
                    inputString = inputString.Remove(offset, offset2 - offset);

                else
                    offset = offset2;
            }
            while (true);
        }

        public void SetFont(Font font)
        {
            using (var richTextBox = new StiRichTextBox(false))
            {
                richTextBox.Rtf = this.RtfText;

                richTextBox.SelectAll();
                richTextBox.SelectionFont = font;

                this.RtfText = richTextBox.Rtf;
            }
        }

        public void SetColor(Color color)
        {
            using (var richTextBox = new StiRichTextBox(false))
            {
                richTextBox.Rtf = this.RtfText;

                richTextBox.SelectAll();
                richTextBox.SelectionColor = color;

                this.RtfText = richTextBox.Rtf;
            }
        }

        public void ResetImage(bool force = false)
        {
            if (!IsDesigning && !force)
                return;

            try
            {
                Image?.Dispose();

                Image = null;
            }
            catch
            {
            }
        }

        internal void ResetSourceProperties()
        {
            ResetImage();

            DataUrl.Value = string.Empty;
            Text.Value = string.Empty;
            DataColumn = string.Empty;
        }

        /// <summary>
        /// Gets or sets text in rtf format.
        /// </summary>
        [Browsable(false)]
        public string RtfText
        {
            get
            {
                return StiRichText.UnpackRtf(StiRtfHelper.XmlDecodeFast(GetTextInternal()));    //fix: replace XmlConvert.DecodeName with StiRtfHelper.XmlDecodeFast
            }
            set
            {
                SetTextInternal(XmlConvert.EncodeName(StiRichText.PackRtf(value)));
            }
        }

        private bool IsReportWpf
        {
            get
            {
#if CLOUD
                return false;
#else
                return Report != null && Report.IsWpf;
#endif
            }
        }

        /// <summary>
        /// Creates a new object of the type StiRichText.
        /// </summary>
        public StiRichText() : this(RectangleD.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiRichText.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiRichText(RectangleD rect) : this(rect, string.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiRichText.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        /// <param name="text">Text expression</param>
        public StiRichText(RectangleD rect, string text) : base(rect, text)
        {
            PlaceOnToolbox = true;
        }
        #endregion
    }
}