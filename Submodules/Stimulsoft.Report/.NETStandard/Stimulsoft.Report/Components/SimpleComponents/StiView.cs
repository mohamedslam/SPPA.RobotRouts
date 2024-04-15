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
using System.IO;
using System.ComponentModel;
using System.Linq;
using Stimulsoft.Report.Units;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Base.Drawing.Design;
using System.Drawing.Design;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// The class describes the base component that renders into an image.
    /// </summary>
    [StiToolbox(true)]
    [StiGdiPainter(typeof(StiViewGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiViewWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiV1Builder(typeof(StiViewV1Builder))]
    [StiV2Builder(typeof(StiViewV2Builder))]
    public abstract class StiView :
        StiComponent,
        IStiHorAlignment,
        IStiVertAlignment,
        IStiBorder,
        IStiExportImageExtended,
        IStiBrush
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiView
            jObject.AddPropertyEnum("HorAlignment", HorAlignment, StiHorAlignment.Left);
            jObject.AddPropertyEnum("VertAlignment", VertAlignment, StiVertAlignment.Top);
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyBool("Smoothing", Smoothing, true);
            jObject.AddPropertyBool("Stretch", Stretch);
            jObject.AddPropertyDouble("MultipleFactor", MultipleFactor, 1d);
            jObject.AddPropertyBool("AspectRatio", AspectRatio);

            if (mode == StiJsonSaveMode.Document && ExistImageToDraw())
                jObject.AddPropertyByteArray("ImageBytesToDraw", TakeImageToDraw());

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

                    case "Border":
                        this.Border = property.DeserializeBorder();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "Smoothing":
                        this.Smoothing = property.DeserializeBool();
                        break;

                    case "Stretch":
                        this.Stretch = property.DeserializeBool();
                        break;

                    case "MultipleFactor":
                        this.MultipleFactor = property.DeserializeDouble();
                        break;

                    case "AspectRatio":
                        this.AspectRatio = property.DeserializeBool();
                        break;

                    case "ImageToDraw":
                        this.PutImageToDraw(property.DeserializeImage());
                        break;

                    case "ImageBytesToDraw":
                        this.PutImageToDraw(property.DeserializeImage());
                        break;
                }
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
            var view = (StiView)base.Clone(cloneProperties);

            if (this.Border != null)
                view.Border = (StiBorder)this.Border.Clone();
            else
                view.Border = null;

            if (this.Brush != null)
                view.Brush = (StiBrush)this.Brush.Clone();
            else
                view.Brush = null;

            view.HorAlignment = this.HorAlignment;
            view.VertAlignment = this.VertAlignment;

            return view;
        }
        #endregion

        #region IStiHorAlignment
        /// <summary>
        /// Gets or sets the horizontal alignment of an object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiHorAlignment.Left)]
        [StiOrder(StiPropertyOrder.ImageHorAlignment)]
        [StiCategory("ImageAdditional")]
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
        [StiOrder(StiPropertyOrder.ImageVertAlignment)]
        [StiCategory("ImageAdditional")]
        [Description("Gets or sets the vertical alignment of an object.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiExpressionEnumConverter))]
        [Editor(StiEditors.ExpressionEnum, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public StiVertAlignment VertAlignment { get; set; } = StiVertAlignment.Top;
        #endregion

        #region IStiExportImageExtended
        public virtual Image GetImage(ref float zoom)
        {
            return GetImage(ref zoom, StiExportFormat.None);
        }

        public virtual Image GetImage(ref float zoom, StiExportFormat format)
        {
            if (this.ObjectToDraw != null)
            {
                var painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Wpf);
                return painter.GetImage(this, ref zoom, format);
            }
            else
            {
                var painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi);
                return painter.GetImage(this, ref zoom, format);
            }
        }

        [Browsable(false)]
        public override bool IsExportAsImage(StiExportFormat format)
        {
            return true;
        }
        #endregion

        #region IStiBorder
        /// <summary>
        /// Gets or sets border of the component.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("Gets or sets border of the component.")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBorder Border { get; set; } = new StiBorder();

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
        [StiSerializable]
        [Description("The brush, which is used to draw background.")]
        [StiOrder(StiPropertyOrder.AppearanceBrush)]
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

        #region IStiGetActualSize
        public override SizeD GetActualSize()
        {
            var newSize = new SizeD(Width, Height);

            if (!CanGrow && !CanShrink)
                return newSize;

            if (!ExistImageToDraw())
                return SizeD.Empty;

            var unit = Page.Unit;
            var compWidth = unit.ConvertToHInches(this.Width);
            var compHeight = unit.ConvertToHInches(this.Height);

            using (var gdiImage = TryTakeGdiImageToDraw())
            {
                if (gdiImage != null)
                {
                    if (CanGrow)
                    {
                        if (gdiImage.Width * MultipleFactor > compWidth)
                            newSize.Width = unit.ConvertFromHInches(gdiImage.Width * MultipleFactor);

                        if (gdiImage.Height * MultipleFactor > compHeight)
                            newSize.Height = unit.ConvertFromHInches(gdiImage.Height * MultipleFactor);
                    }

                    if (CanShrink)
                    {
                        if (gdiImage.Width * MultipleFactor < compWidth)
                            newSize.Width = unit.ConvertFromHInches(gdiImage.Width * MultipleFactor);

                        if (gdiImage.Height * MultipleFactor < compHeight)
                            newSize.Height = unit.ConvertFromHInches(gdiImage.Height * MultipleFactor);
                    }

                    if (AspectRatio && CanShrink && (!MinSize.IsEmpty || !MaxSize.IsEmpty))
                    {
                        double newWidth = newSize.Width;
                        if (MinSize.Width != 0) newWidth = Math.Max(newWidth, MinSize.Width);
                        if (MaxSize.Width != 0) newWidth = Math.Min(newWidth, MaxSize.Width);
                        double newHeight = newSize.Height;
                        if (MinSize.Height != 0) newHeight = Math.Max(newHeight, MinSize.Height);
                        if (MaxSize.Height != 0) newHeight = Math.Min(newHeight, MaxSize.Height);

                        try
                        {
                            double aspect1 = (double)gdiImage.Width / gdiImage.Height;
                            double aspect2 = newWidth / newHeight;
                            if (aspect1 > aspect2)
                            {
                                newSize.Height = newWidth / aspect1;
                            }
                        }
                        catch { }
                    }
                }
            }

            return newSize;
        }

        public SizeD GetRealSize()
        {
            var newSize = new SizeD(this.Width, this.Height);

            if (!CanGrow && !CanShrink)
                return newSize;

            if (!ExistImageToDraw())
                return SizeD.Empty;
            
            using (var gdiImage = TryTakeGdiImageToDraw())
            {
                if (gdiImage != null)
                {
                    var unit = Page.Unit;
                    var compWidth = unit.ConvertToHInches(this.Width);
                    var compHeight = unit.ConvertToHInches(this.Height);

                    if (CanGrow)
                    {
                        if (gdiImage.Width * MultipleFactor > compWidth)
                            newSize.Width = unit.ConvertFromHInches(gdiImage.Width * MultipleFactor);

                        if (gdiImage.Height * MultipleFactor > compHeight)
                            newSize.Height = unit.ConvertFromHInches(gdiImage.Height * MultipleFactor);
                    }

                    #region CanShrink
                    if (gdiImage.Width * MultipleFactor < compWidth)
                        newSize.Width = unit.ConvertFromHInches(gdiImage.Width * MultipleFactor);

                    if (gdiImage.Height * MultipleFactor < compHeight)
                        newSize.Height = unit.ConvertFromHInches(gdiImage.Height * MultipleFactor);
                    #endregion
                }
            }

            return newSize;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets smoothing mode for drawing image.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageSmoothing)]
        [DefaultValue(true)]
        [Description("Gets or sets smoothing mode for drawing image.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool Smoothing { get; set; } = true;
        
        [Browsable(false)]
        public bool IsCachedImage { get; set; }

        /// <summary>
        /// Gets or sets the WPF visual object that appeared as a result of the component rendering. This property accepts objects of two types: DrawingVisual and ImageSource.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the WPF visual object that appeared as a result of the component rendering. This property accepts objects of two types: DrawingVisual and ImageSource.")]
        public object ObjectToDraw { get; set; }
        
        /// <summary>
        /// Gets or sets the image that appeared as a result of the component rendering.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the image that appeared as a result of the component rendering.")]
        public virtual Image ImageToDraw
        {
            get
            {
                return TakeGdiImageToDraw();
            }
            set
            {
                PutImageToDraw(value);
            }
        }

        internal byte[] imageBytesToDraw;
        /// <summary>
        /// Gets or sets the image bytes that appeared as a result of the component rendering.
        /// </summary>
        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        [Description("Gets or sets the image bytes that appeared as a result of the component rendering.")]
        public virtual byte[] ImageBytesToDraw
        {
            get
            {
                if (IsDesigning || imageBytesToDraw != null)
                    return imageBytesToDraw;

                return StiFileImageCacheV2.LoadImage(Report, Guid);
            }
            set
            {
                IsCachedImage = false;

                if (Report != null && !IsDesigning && StiOptions.Engine.ImageCache.Enabled && value != null)
                {
                    if (this.Guid == null)
                        this.NewGuid();

                    StiFileImageCacheV2.SaveImage(Report, Guid, value);

                    IsCachedImage = true;
                }
                else
                    imageBytesToDraw = value;
            }
        }

        /// <summary>
        /// Gets or sets value, indicates that this component will stretch the image till the image will get size equal in its size on the page.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageStretch)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value, indicates that this component will stretch the image till the image will get size equal in its size on the page.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public bool Stretch { get; set; }

        /// <summary>
        /// Gets or sets value to multiply by it an image size.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(1d)]
        [StiSerializable]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageMultipleFactor)]
        [Description("Gets or sets value to multiply by it an image size.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public double MultipleFactor { get; set; } = 1d;

        /// <summary>
        /// Gets or sets value, indicates that the image will save its aspect ratio.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageAspectRatio)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value, indicates that the image will save its aspect ratio.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public bool AspectRatio { get; set; }
        #endregion

        #region Methods
        public abstract byte[] GetImageFromSource();

        internal bool ExistImageToDraw()
        {
            return ImageBytesToDraw != null;
        }

        internal byte[] TakeImageToDraw()
        {
            return ImageBytesToDraw;
        }

        internal Image TakeGdiImageToDraw(float svgScale = 1)
        {
            if (!ExistImageToDraw()) return null;

            if (ImageToDrawIsSvg())
            {
                var rect = GetPaintRectangle(true, false);
                try
                {
                    return StiSvgHelper.ConvertSvgToImage(ImageBytesToDraw, (int)(rect.Width * svgScale), (int)(rect.Height * svgScale), Stretch, AspectRatio, true);
                }
                catch (OutOfMemoryException)
                {
                    if (svgScale > 1)
                    {
                        #region Try to reduce svg resolution
                        svgScale = (svgScale + 1) / 2;  //half scale
                        try
                        {
                            return StiSvgHelper.ConvertSvgToImage(ImageBytesToDraw, (int)(rect.Width * svgScale), (int)(rect.Height * svgScale), Stretch, AspectRatio, true);
                        }
                        catch (OutOfMemoryException)
                        {
                            //without scaling
                            return StiSvgHelper.ConvertSvgToImage(ImageBytesToDraw, (int)rect.Width, (int)rect.Height, Stretch, AspectRatio);
                        }
                        #endregion
                    }
                    return new Bitmap(1, 1);
                }
            }

            var image = StiImageConverter.BytesToImage(ImageBytesToDraw);

            if (StiOptions.Engine.Image.RotateImageByExifOrientationData && image != null)
                RotateImageByExifOrientationData(image);

            return image;
        }

        public static void RotateImageByExifOrientationData(Image img, bool updateExifData = true)
        {
            try
            {
#if !STIDRAWING
                var orientationId = 0x0112;
                if (!img.PropertyIdList.Contains(orientationId)) return;

                var pItem = img.GetPropertyItem(orientationId);
                var fType = RotateFlipType.RotateNoneFlipNone;

                switch (pItem.Value[0])
                {
                    case 2:
                        fType = RotateFlipType.RotateNoneFlipX;
                        break;

                    case 3:
                        fType = RotateFlipType.Rotate180FlipNone;
                        break;

                    case 4:
                        fType = RotateFlipType.Rotate180FlipX;
                        break;

                    case 5:
                        fType = RotateFlipType.Rotate90FlipX;
                        break;

                    case 6:
                        fType = RotateFlipType.Rotate90FlipNone;
                        break;

                    case 7:
                        fType = RotateFlipType.Rotate270FlipX;
                        break;

                    case 8:
                        fType = RotateFlipType.Rotate270FlipNone;
                        break;
                }

                if (fType != RotateFlipType.RotateNoneFlipNone)
                {
                    img.RotateFlip(fType);

                    // Remove Exif orientation tag (if requested)
                    if (updateExifData)
                        img.RemovePropertyItem(orientationId);
                }
#endif
            }
            catch
            {
            }
        }

        internal Image TryTakeGdiImageToDraw()
        {
            return ExistImageToDraw()
                ? StiImageConverter.TryBytesToImage(ImageBytesToDraw)
                : null;
        }

        internal void PutImageToDraw(Image image)
        {
            ImageBytesToDraw = StiImageConverter.ImageToBytes(image, true);
        }

        internal void PutImageToDraw(byte[] image)
        {
            ImageBytesToDraw = image;
        }

        internal void ResetImageToDraw()
        {
            ImageBytesToDraw = null;
        }

        internal bool ImageToDrawIsMetafile()
        {
            return StiImageHelper.IsMetafile(ImageBytesToDraw);
        }

        internal bool ImageToDrawIsSvg()
        {
            return StiSvgHelper.IsSvg(ImageBytesToDraw);
        }
        #endregion

        /// <summary>
        /// Creates a new component of the type StiView.
        /// </summary>
        public StiView() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiView.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiView(RectangleD rect) : base(rect)
        {
        }
    }
}
