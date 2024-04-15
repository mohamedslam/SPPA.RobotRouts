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
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// The class describes the component of printing for image printing - Image.
    /// </summary>
    [StiServiceBitmap(typeof(StiImage), "Stimulsoft.Report.Images.Components.StiImage.png")]
    [StiToolbox(true)]
    [StiContextTool(typeof(IStiShift))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiGrowToHeight))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiImageDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfImageDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiImageGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiImageWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiImageQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfImageQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiV1Builder(typeof(StiImageV1Builder))]
    [StiV2Builder(typeof(StiImageV2Builder))]
    [StiContextTool(typeof(IStiBreakable))]
    public class StiImage :
        StiView,
        IStiBreakable,
        IStiGlobalizedName
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiImage
            jObject.AddPropertyStringNullOrEmpty("GlobalizedName", GlobalizedName);
            jObject.AddPropertyBool("CanBreak", CanBreak);
            jObject.AddPropertyJObject("GetImageURLEvent", GetImageURLEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetImageDataEvent", GetImageDataEvent.SaveToJsonObject(mode));
            jObject.AddPropertyEnum("ProcessingDuplicates", ProcessingDuplicates, StiImageProcessingDuplicatesType.None);
            jObject.AddPropertyEnum("ImageRotation", ImageRotation, StiImageRotation.None);
            jObject.AddPropertyStringNullOrEmpty("File", File);
            jObject.AddPropertyStringNullOrEmpty("DataColumn", DataColumn);
            jObject.AddPropertyJObject("Margins", Margins.SaveToJsonObject(0, 0, 0, 0));

            if (Icon != null)
                jObject.AddPropertyEnum("Icon", Icon);

            jObject.AddPropertyColor("IconColor", IconColor, Color.FromArgb(68, 114, 196));

            if (mode == StiJsonSaveMode.Document)
            {
                jObject.AddPropertyStringNullOrEmpty("ImageURLValue", ImageURLValue as string);
            }
            else
            {
                jObject.AddPropertyJObject("ImageURL", ImageURL.SaveToJsonObject(mode));
                jObject.AddPropertyJObject("ImageData", ImageData.SaveToJsonObject(mode));

                if (ExistImage())
                    jObject.Add("ImageBytes", global::System.Convert.ToBase64String(TakeImage()));
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
                    case "GlobalizedName":
                        this.GlobalizedName = property.DeserializeString();
                        break;

                    case "CanBreak":
                        this.CanBreak = property.DeserializeBool();
                        break;

                    case "ImageURLValue":
                        this.imageURLValue = property.DeserializeString();
                        break;

                    case "ImageURL":
                        {
                            var imageExpression = new StiImageURLExpression();
                            imageExpression.LoadFromJsonObject((JObject)property.Value);
                            this.ImageURL = imageExpression;
                        }
                        break;

                    case "ImageData":
                        {
                            var imageDataExpression = new StiImageDataExpression();
                            imageDataExpression.LoadFromJsonObject((JObject)property.Value);
                            this.ImageData = imageDataExpression;
                        }
                        break;

                    case "GetImageURLEvent":
                        {
                            var imageEvent = new StiGetImageURLEvent();
                            imageEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetImageURLEvent = imageEvent;
                        }
                        break;

                    case "GetImageDataEvent":
                        {
                            var imageDataEvent = new StiGetImageDataEvent();
                            imageDataEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetImageDataEvent = imageDataEvent;
                        }
                        break;

                    case "ProcessingDuplicates":
                        this.ProcessingDuplicates = property.DeserializeEnum<StiImageProcessingDuplicatesType>();
                        break;

                    case "ImageRotation":
                        this.ImageRotation = property.DeserializeEnum<StiImageRotation>();
                        break;

                    case "Image":
                        this.PutImage(property.DeserializeImage());
                        break;

                    case "ImageBytes":
                        this.PutImage(property.DeserializeByteArray());
                        break;

                    case "File":
                        this.file = property.DeserializeString();
                        break;

                    case "DataColumn":
                        this.dataColumn = property.DeserializeString();
                        break;

                    case "Icon":
                        this.Icon = property.DeserializeEnum<StiFontIcons>();
                        break;

                    case "IconColor":
                        this.IconColor = property.DeserializeColor();
                        break;

                    case "Margins":
                        var margins = new StiMargins();
                        margins.LoadFromJsonObject((JObject)property.Value);
                        this.Margins = margins;
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiImage;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            objHelper.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.ImageEditor()
            });

            if (level == StiLevel.Basic)
            {
                objHelper.Add(StiPropertyCategories.ImageAdditional, new[]
                {
                    propHelper.AspectRatio(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment(),
                    propHelper.Stretch()
                });
            }
            else if (level == StiLevel.Standard)
            {
                objHelper.Add(StiPropertyCategories.ImageAdditional, new[]
                {
                    propHelper.AspectRatio(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment(),
                    propHelper.ImageRotation(),
                    propHelper.Margins(),
                    propHelper.MultipleFactor(),
                    propHelper.ProcessingDuplicates(),
                    propHelper.Stretch()
                });
            }
            else
            {
                objHelper.Add(StiPropertyCategories.ImageAdditional, new[]
                {
                    propHelper.AspectRatio(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment(),
                    propHelper.ImageRotation(),
                    propHelper.Margins(),
                    propHelper.MultipleFactor(),
                    propHelper.ProcessingDuplicates(),
                    propHelper.Smoothing(),
                    propHelper.Stretch()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                objHelper.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height()
                });
            }
            else
            {
                objHelper.Add(StiPropertyCategories.Position, new[]
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
                objHelper.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.IconColor(),
                });
            }
            else
            {
                objHelper.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.IconColor(),
                    propHelper.UseParentStyles()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                objHelper.Add(StiPropertyCategories.Behavior, new[]
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
                objHelper.Add(StiPropertyCategories.Behavior, new[]
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
                objHelper.Add(StiPropertyCategories.Behavior, new[]
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
                objHelper.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name()
                });
            }
            else if (level == StiLevel.Standard)
            {
                objHelper.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias()
                });
            }
            else
            {
                objHelper.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias(),
                    propHelper.GlobalizedName(),
                    propHelper.Restrictions(),
                    propHelper.Locked(),
                    propHelper.Linked()
                });
            }

            return objHelper;
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
                        StiPropertyEventId.GetImageDataEvent,
                        StiPropertyEventId.GetImageURLEvent,
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/report_internals_graphic_information_output.htm";
        #endregion

        #region IStiExportImageExtended
        public override Image GetImage(ref float zoom, StiExportFormat format)
        {
            if (ExistImageToDraw() && format == StiExportFormat.HtmlTable)
            {
                zoom = 1;
                
                var image = StiImageConverter.BytesToImage(this.ImageBytesToDraw);
                if (image != null && (
                    image.RawFormat.Equals(ImageFormat.Jpeg) ||
                    image.RawFormat.Equals(ImageFormat.Gif) ||
                    image.RawFormat.Equals(ImageFormat.Png) ||
                    image.RawFormat.Equals(ImageFormat.Bmp)))
                    return image;
            }

            return base.GetImage(ref zoom, format);
        }
        #endregion

        #region IStiGlobalizedName
        /// <summary>
        /// Gets or sets special identificator which will be used for report globalization.
        /// </summary>
        [StiCategory("Design")]
        [StiOrder(StiPropertyOrder.DesignGlobalizedName)]
        [StiSerializable]
        [ParenthesizePropertyName(true)]
        [Description("Gets or sets special identificator which will be used for report globalization.")]
        [DefaultValue("")]
        [Editor("Stimulsoft.Report.Design.StiGlobabalizationManagerEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual string GlobalizedName { get; set; } = "";
        #endregion

        #region IStiView override
        /// <summary>
        /// Returns the image from the specified path.
        /// </summary>
        protected virtual byte[] GetImageFromFile()
        {
            if (string.IsNullOrEmpty(file))
                return null;

            var tempFile = StiExpressionHelper.ParseText(Page, File);
            if (!string.IsNullOrEmpty(StiOptions.Engine.Image.AbsolutePathOfImages))
                tempFile = Path.Combine(StiOptions.Engine.Image.AbsolutePathOfImages, tempFile);  // 2013.01.25  fix: arguments are reversed

            if (!global::System.IO.File.Exists(tempFile))
                return null;

            try
            {
                return global::System.IO.File.Exists(tempFile)
                    ? global::System.IO.File.ReadAllBytes(tempFile)
                    : null;
            }
            catch (Exception ex)
            {

                if (!IsDesigning)
                {
                    var str = $"Image can't be loaded from file '{file}' in image component {Name}!";

                    StiLogService.Write(this.GetType(), str);
                    StiLogService.Write(this.GetType(), ex.Message);

                    Report.WriteToReportRenderingMessages(str);
                }
                else
                {
                    return new byte[0];//We should return empty image in design-time
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the image from specified url.
        /// </summary>
        protected virtual byte[] GetImageFromUrl()
        {
            var url = IsDesigning ? ImageURL.Value : ImageURLValue as string;

            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    if (StiHyperlinkProcessor.IsServerHyperlink(url))
                        return StiStimulsoftServerResource.GetImage(this, StiHyperlinkProcessor.GetServerNameFromHyperlink(url));

                    var resourceName = StiHyperlinkProcessor.GetResourceNameFromHyperlink(url);
                    if (resourceName != null)
                        return StiHyperlinkProcessor.GetBytes(Report, url);

                    var variableName = StiHyperlinkProcessor.GetVariableNameFromHyperlink(url);
                    if (variableName != null)
                        return StiHyperlinkProcessor.GetBytes(Report, url);

                    if (IsDesigning && !StiOptions.Configuration.IsWPF && StiImageHyperlinkLoader.AllowAsyncLoading)
                    {
                        StiImageHyperlinkLoader.Load(this, url);
                        return null;
                    }
                    else
                    {
                        return StiDownloadCache.Get(url, Report?.CookieContainer, Report?.HttpHeadersContainer);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!IsDesigning)
                {
                    var str = $"Image can't be loaded from URL '{url}' in image component {Name}!";

                    StiLogService.Write(this.GetType(), str);
                    StiLogService.Write(this.GetType(), ex.Message);

                    Report.WriteToReportRenderingMessages(str);
                }
                else
                {
                    return new byte[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the image from specified data column.
        /// </summary>
        protected virtual byte[] GetImageFromDataColumn()
        {
            try
            {
                var imageObject = StiDataColumn.GetDataFromDataColumn(this.Report.Dictionary, this.DataColumn);

                return StiImageHelper.GetImageBytesFromObject(imageObject);
            }
            catch (Exception ex)
            {
                if (!IsDesigning)
                {
                    var str = $"Image can't be loaded from data column '{DataColumn}' in image component {Name}!";

                    StiLogService.Write(this.GetType(), str);
                    StiLogService.Write(this.GetType(), ex.Message);

                    Report.WriteToReportRenderingMessages(str);
                }
                else
                {
                    return new byte[0];//We should return empty image in design-time
                }
            }
            return null;
        }

        /// <summary>
		/// Returns the image from specified data column.
		/// </summary>
		protected virtual byte[] GetImageFromIcon()
        {
            try
            {
                if (this.Icon == null)
                    return null;

                var factor = StiOptions.Configuration.IsWPF ? 1 : StiScale.Factor;

                var rect = GetPaintRectangle(true, false);
                var imageObject = StiFontIconsHelper.ConvertFontIconToImage(
                    Icon.GetValueOrDefault(), IconColor, (int)(rect.Width * factor), (int)(rect.Height * factor));

                return StiImageHelper.GetImageBytesFromObject(imageObject);
            }
            catch (Exception ex)
            {
                if (!IsDesigning)
                {
                    var str = $"Image can't be loaded from data column '{DataColumn}' in image component {Name}!";

                    StiLogService.Write(this.GetType(), str);
                    StiLogService.Write(this.GetType(), ex.Message);

                    Report.WriteToReportRenderingMessages(str);
                }
                else
                {
                    return new byte[0];//We should return empty image in design-time
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the image being get as a result of rendering.
        /// </summary>
        public override byte[] GetImageFromSource()
        {
            var image = GetImageFromFile();
            if (image != null)
                return image;

            image = GetImageFromUrl();
            if (image != null)
                return image;

            image = GetImageFromIcon();
            if (image != null)
                return image;

#if BLAZOR
            image = GetImageFromDataColumn();
            if (image != null)
                return image;

            return imageBytes;
#else
            return GetImageFromDataColumn();
#endif
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
        public bool Break(StiComponent dividedComponent, double divideFactor, ref double divideLine)
        {
            divideLine = 0;
            var result = true;

            if (this.ExistImageToDraw())
            {
                var originalImageBytes = TakeImageToDraw();
                this.PutImageToDraw(StiComponentDivider.BreakImage(dividedComponent as StiImage, ref originalImageBytes, divideFactor));

                ((StiImage)dividedComponent).PutImageToDraw(originalImageBytes);
            }
            return result;
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.Image;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory => StiLocalization.Get("Report", "Components");

        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiImage");

        /// <summary>
		/// Gets or sets the default client area of a component.
		/// </summary>
		[Browsable(false)]
        public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 100, 100);

        public override void OnResizeComponent(SizeD oldSize, SizeD newSize)
        {
            base.OnResizeComponent(oldSize, newSize);

            var url = IsDesigning ? ImageURL.Value : ImageURLValue as string;
            if (string.IsNullOrEmpty(file) && string.IsNullOrEmpty(url) && Icon != null)
                ResetImageToDraw();
        }
        #endregion

        #region Expressions
        #region ImageURL
        private object imageURLValue;
        /// <summary>
        /// Gets or sets image URL.
        /// </summary>
        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        [Description("Gets or sets image URL.")]
        public object ImageURLValue
        {
            get
            {
                return imageURLValue;
            }
            set
            {
                if (imageURLValue != value)
                {
                    imageURLValue = value;

                    if (!ExistImage() && (IsDesigning || (Report != null && Report.IsRendering)))
                    {
                        if (value != null)
                            this.PutImageToDraw(GetImageFromUrl());
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the expression to fill a component image URL.
        /// </summary>
        [StiCategory("Image")]
        [StiOrder(StiPropertyOrder.ImageImageURL)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a component image URL.")]
        [Editor("Stimulsoft.Report.Components.Design.StiImageExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual StiImageURLExpression ImageURL
        {
            get
            {
                return new StiImageURLExpression(this, "ImageURL");
            }
            set
            {
                if (value != null)
                    value.Set(this, "ImageURL", value.Value);

                ResetImageToDraw();
            }
        }
        #endregion

        #region ImageData
        /// <summary>
        /// Gets or sets the expression to fill a component image property.
        /// </summary>
        [StiCategory("Image")]
        [StiOrder(StiPropertyOrder.ImageImageData)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a component image property.")]
        [Editor("Stimulsoft.Report.Components.Design.StiImageExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual StiImageDataExpression ImageData
        {
            get
            {
                return new StiImageDataExpression(this, "ImageData");
            }
            set
            {
                if (value != null)
                    value.Set(this, "ImageData", value.Value);

                ResetImageToDraw();
            }
        }
        #endregion
        #endregion

        #region Events
        /// <summary>
        /// Return events collection of this component.
        /// </summary>
        public override StiEventsCollection GetEvents()
        {
            var events = base.GetEvents();

            if (GetImageURLEvent != null)
                events.Add(GetImageURLEvent);

            if (GetImageDataEvent != null)
                events.Add(GetImageDataEvent);

            return events;
        }

        /// <summary>
        /// Invokes all events for this components.
        /// </summary>
        public override void InvokeEvents()
        {
            try
            {
                base.InvokeEvents();

                #region GetImageURL
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    if (this.Events[EventGetImageURL] != null)
                    {
                        if (this.ImageURLValue == null || (this.ImageURLValue is string && ((string)this.ImageURLValue).Length == 0))
                        {
                            var e = new StiValueEventArgs();
                            InvokeGetImageURL(this, e);
                            this.ImageURLValue = e.Value;
                        }
                    }
                }
                else
                {
                    if (this.ImageURLValue == null || (this.ImageURLValue is string && ((string)this.ImageURLValue).Length == 0))
                    {
                        var e = new StiValueEventArgs();
                        InvokeGetImageURL(this, e);
                        this.ImageURLValue = e.Value;
                    }
                }
                #endregion

                #region GetImageData
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    if (this.Events[EventGetImageData] != null)
                    {
                        var ee = new StiGetImageDataEventArgs();
                        InvokeGetImageData(this, ee);
                        ProcessGetImageDataValue(ee);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.ImageData.Value))
                    {
                        var ee = new StiGetImageDataEventArgs();
                        InvokeGetImageData(this, ee);
                        ProcessGetImageDataValue(ee);
                    }
                }
                #endregion
            }
            catch (Exception er)
            {
                StiLogService.Write(this.GetType(), "DoEvents...ERROR");
                StiLogService.Write(this.GetType(), er);

                Report?.WriteToReportRenderingMessages($"{Name} {er.Message}");
            }
        }

        private void ProcessGetImageDataValue(StiGetImageDataEventArgs ee)
        {
            if (ee.Value != null)
            {
                object obj = ee.Value;
                if (obj is string)
                {
                    string st = (string)obj;
                    if (st.IsBase64String())
                    {
                        try
                        {
                            obj = global::System.Convert.FromBase64String(st);
                        }
                        catch { }
                    }
                    int pos = st.IndexOfInvariant("<svg");
                    if (pos >= 0 && pos < 1000)
                    {
                        obj = Encoding.UTF8.GetBytes(st);
                    }
                }
                if (obj is Image)
                    PutImage(obj as Image);
                if (obj is byte[])
                    PutImage(obj as byte[]);
            }
#pragma warning disable CS0612 // Type or member is obsolete
            if (ee.ValueBytes != null)
                PutImage(ee.ValueBytes);
#pragma warning restore CS0612 // Type or member is obsolete
        }

        #region GetImageURL
        private static readonly object EventGetImageURL = new object();
        /// <summary>
        /// Occurs when getting image url for the component.
        /// </summary>
        public event StiValueEventHandler GetImageURL
        {
            add
            {
                Events.AddHandler(EventGetImageURL, value);
            }
            remove
            {
                Events.RemoveHandler(EventGetImageURL, value);
            }
        }

        /// <summary>
        /// Raises the GetImageURL event.
        /// </summary>
        protected virtual void OnGetImageURL(StiValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetImageURL event.
        /// </summary>
        public void InvokeGetImageURL(object sender, StiValueEventArgs e)
        {
            try
            {
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    OnGetImageURL(e);

                    var handler = Events[EventGetImageURL] as StiValueEventHandler;
                    if (handler != null)
                        handler(sender, e);
                }
                else
                {
                    OnGetImageURL(e);

                    object parserResult = StiParser.ParseTextValue(this.ImageURL.Value, this, sender);
                    if (parserResult != null) e.Value = parserResult.ToString();

                    var handler = Events[EventGetImageURL] as StiValueEventHandler;
                    if (handler != null)
                        handler(sender, e);
                }

                StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetImageURLEvent, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in ImageURL property of '{Name}' can't be evaluated!";

                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);

                Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting image url for the component.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting image url for the component.")]
        public virtual StiGetImageURLEvent GetImageURLEvent
        {
            get
            {
                return new StiGetImageURLEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion

        #region GetImageData
        private static readonly object EventGetImageData = new object();

        /// <summary>
        /// Occurs when getting image for the component.
        /// </summary>
        public event StiGetImageDataEventHandler GetImageData
        {
            add
            {
                Events.AddHandler(EventGetImageData, value);
            }
            remove
            {
                Events.RemoveHandler(EventGetImageData, value);
            }
        }

        /// <summary>
        /// Raises the GetImageData event.
        /// </summary>
        protected virtual void OnGetImageData(StiGetImageDataEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetImageData event.
        /// </summary>
        public void InvokeGetImageData(object sender, StiGetImageDataEventArgs e)
        {
            try
            {
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    OnGetImageData(e);

                    var handler = Events[EventGetImageData] as StiGetImageDataEventHandler;
                    if (handler != null)
                        handler(sender, e);
                }
                else
                {
                    OnGetImageData(e);

                    e.Value = StiParser.ParseTextValue(this.ImageData.Value, this, sender);

                    var handler = Events[EventGetImageData] as StiGetImageDataEventHandler;
                    if (handler != null)
                        handler(sender, e);
                }

                StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetImageDataEvent, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in ImageData property of '{Name}' can't be evaluated!";

                StiLogService.Write(GetType(), str);
                StiLogService.Write(GetType(), ex.Message);

                Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting image for the component.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting image for the component.")]
        public virtual StiGetImageDataEvent GetImageDataEvent
        {
            get
            {
                return new StiGetImageDataEvent(this);
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
        protected static object PropertyProcessingDuplicates = new object();
        /// <summary>
        /// Gets or sets value which indicates how report engine processes duplicated images.
        /// </summary>
        [DefaultValue(StiImageProcessingDuplicatesType.None)]
        [StiSerializable]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageProcessingDuplicates)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates how report engine processes duplicated images.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiImageProcessingDuplicatesType ProcessingDuplicates
        {
            get
            {
                return (StiImageProcessingDuplicatesType)Properties.Get(PropertyProcessingDuplicates, StiImageProcessingDuplicatesType.None);
            }
            set
            {
                Properties.Set(PropertyProcessingDuplicates, value, StiImageProcessingDuplicatesType.None);
            }
        }

        /// <summary>
		/// Gets or sets value which indicates how to rotate an image before output.
		/// </summary>
		[Browsable(true)]
        [DefaultValue(StiImageRotation.None)]
        [StiSerializable]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageImageRotation)]
        [Description("Gets or sets value which indicates how to rotate an image before output.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiImageRotation ImageRotation { get; set; } = StiImageRotation.None;

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        [Browsable(true)]
        [StiCategory("Image")]
        [StiOrder(StiPropertyOrder.ImageImage)]
        [Description("Gets or sets the image.")]
        [Editor("Stimulsoft.Report.Components.Design.StiImageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiSimpeImageConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        public Image Image
        {
            get
            {
                return TakeGdiImage();
            }
            set
            {
                if (value == StiNullImage.Null) return;

                PutImage(value);
            }
        }

        private bool ShouldSerializeImage()
        {
            return Image != null;
        }

        private byte[] imageBytes;
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
        [StiCategory("Image")]
        [StiOrder(StiPropertyOrder.ImageImage)]
        [Description("Gets or sets the image.")]
        [Editor("Stimulsoft.Report.Components.Design.StiImageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiSimpeImageConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        public byte[] ImageBytes
        {
            get
            {
                return imageBytes;
            }
            set
            {
                if (imageBytes != value)
                {
                    imageBytes = value;
                    PutImageToDraw(value);
                }
            }
        }

        protected static object PropertyMargins = new object();
        /// <summary>
        /// Gets or sets image margins.
        /// </summary>
        [StiSerializable]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageMargins)]
        [Description("Gets or sets image margins.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiMargins Margins { get; set; } = new StiMargins();

        private bool ShouldSerializeMargins()
        {
            return !Margins.IsDefault;
        }

        private string file = string.Empty;
        /// <summary>
        /// Gets or sets the path to the file that contains the image.
        /// </summary>
        [StiSerializable]
        [Editor("Stimulsoft.Report.Components.Design.StiImageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Image")]
        [StiOrder(StiPropertyOrder.ImageFile)]
        [DefaultValue("")]
        [Description("Gets or sets the path to the file that contains the image.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string File
        {
            get
            {
                return file;
            }
            set
            {
                if (file != value)
                {
                    file = value;

                    if (!ExistImage() && (IsDesigning || (Report != null && !Report.IsSerializing)))
                        this.PutImageToDraw(GetImageFromFile());
                }
                else
                    ResetImageToDraw();
            }
        }

        private string dataColumn = string.Empty;
        /// <summary>
        /// Gets or sets a name of the column that contains the image.
        /// </summary>
        [StiSerializable]
        [Editor("Stimulsoft.Report.Components.Design.StiImageDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Image")]
        [StiOrder(StiPropertyOrder.ImageDataColumn)]
        [DefaultValue("")]
        [Description("Gets or sets a name of the column that contains the image.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string DataColumn
        {
            get
            {
                return dataColumn;
            }
            set
            {
                dataColumn = value;
                ResetImageToDraw();
            }
        }

        private StiFontIcons? icon = null;
        /// <summary>
		/// Gets or sets icon.
		/// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [StiCategory("Image")]
        [Editor("Stimulsoft.Report.Design.StiFontIconEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Report.Design.StiFontIconConverter))]
        [Description("Gets or sets icon.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder(StiPropertyOrder.ImageIcon)]
        public StiFontIcons? Icon
        {
            get
            {
                return icon;
            }
            set
            {
                icon = value;
                ResetImageToDraw();
            }
        }

        private Color iconColor = StiColor.Get("4472C4");
        /// <summary>
		/// Gets or sets icon color.
		/// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets icon color.")]
        [StiOrder(StiPropertyOrder.AppearanceIconColor)]
        [TypeConverter(typeof(StiExpressionColorConverter))]
        [Editor(StiEditors.ExpressionColor, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public Color IconColor
        {
            get
            {
                return iconColor;
            }
            set
            {
                iconColor = value;
                ResetImageToDraw();
            }
        }

        private bool ShouldSerializeIconColor()
        {
            return IconColor != StiColor.Get("4472C4");
        }
        #endregion

        #region Methods
        public void UpdateImageToDrawInDesigner()
        {
            if (ExistImageToDraw()) return;

            if (Report == null || Report.IsPageDesigner) return;

            if (Report != null && Report.Dictionary != null && IsDesigning && !Report.Info.IsComponentsMoving)
            {
                try
                {
                    if (ImageData != null)
                        this.PutImageToDraw(StiVariableImageProcessor.GetImage(this.Report, ImageData.Value));

                    if (!this.ExistImageToDraw())
                        this.PutImageToDraw(GetImageFromSource());
                }
                catch (Exception e)
                {
                    ResetImageToDraw();

                    Report?.WriteToReportRenderingMessages($"{Name} {e.Message}");
                }
            }

            if (StiOptions.Designer.UseGlobalizationManager
                && ExistImage()
                && (this.Report.GlobalizationManager != null)
                && !(this.Report.GlobalizationManager is StiNullGlobalizationManager)
                && !string.IsNullOrWhiteSpace(GlobalizedName))
            {
                try
                {
                    this.PutImageToDraw(Report.GlobalizationManager.GetObject(GlobalizedName) as Image);
                }
                catch
                {
                }
            }
        }

        public RectangleD ConvertImageMargins(RectangleD rect, bool convert)
        {
            StiMargins margins = Margins;   //speed optimization
            if (margins.IsEmpty)
                return rect;

            var zoom = convert ? (Page.Zoom * StiScale.Factor) : 1;
            
            var marginsLeft = margins.Left;
            var marginsRight = margins.Right;
            var marginsTop = margins.Top;
            var marginsBottom = margins.Bottom;

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

        public override StiComponent CreateNew()
        {
            return new StiImage();
        }

        internal bool ExistImage()
        {
            return ImageBytes != null;
        }

        internal byte[] TakeImage()
        {
            return ImageBytes;
        }

        internal Image TakeGdiImage()
        {
            if (!ExistImage())
                return null;

            if (ImageIsSvg())
            {
                var rect = GetPaintRectangle(true, false);
                return StiImageConverter.BytesToImage(ImageBytes, (int)rect.Width, (int)rect.Height, Stretch, AspectRatio);
            }

            return StiImageConverter.BytesToImage(ImageBytes);
        }

        internal void PutImage(Image image)
        {
            ImageBytes = StiImageConverter.ImageToBytes(image, true);
        }

        internal void PutImage(byte[] image)
        {
            ImageBytes = image;
        }

        internal void ResetImage()
        {
            ImageBytes = null;
        }

        internal bool ImageIsMetafile()
        {
            return StiImageHelper.IsMetafile(ImageBytes);
        }

        internal bool ImageIsSvg()
        {
            return StiSvgHelper.IsSvg(ImageBytes);
        }

        internal void ResetAllImageProperties()
        {
            ResetImage();
            DataColumn = string.Empty;
            ImageURL.Value = string.Empty;
            ImageData.Value = string.Empty;
            File = string.Empty;
        }

        internal void CopyAllImageProperties(StiImage image)
        {
            if (image == null) return;

            ImageBytes = image.ImageBytes;
            DataColumn = image.DataColumn;
            ImageURL.Value = image.ImageURL.Value;
            ImageData.Value = image.ImageData.Value;
            File = image.File;
        }
        #endregion

        /// <summary>
		/// Creates a new component of the type StiImage.
		/// </summary>
		public StiImage() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiImage with specified location.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiImage(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = true;
        }
    }
}