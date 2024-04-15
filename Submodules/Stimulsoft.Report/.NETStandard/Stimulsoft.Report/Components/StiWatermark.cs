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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Helpers;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Drawing;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// The class describes the watermark of the page.
	/// </summary>
	[TypeConverter(typeof(StiWatermarkConverter))]
	[RefreshProperties(RefreshProperties.All)]
	public class StiWatermark : 
		ICloneable,
		IStiSerializeToCodeAsClass,
        IStiJsonReportObject,
	    IStiDefault
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyFont("Font", Font, "Arial", 100);
            jObject.AddPropertyBrush("TextBrush", TextBrush);
            jObject.AddPropertyStringNullOrEmpty("Text", Text);
            jObject.AddPropertyStringNullOrEmpty("ImageHyperlink", ImageHyperlink);
            jObject.AddPropertyFloat("Angle", Angle, 45f);
            jObject.AddPropertyBool("Enabled", Enabled, true);
            jObject.AddPropertyBool("ShowImageBehind", ShowImageBehind, true);
            jObject.AddPropertyBool("ShowBehind", ShowBehind);
            jObject.AddPropertyBool("RightToLeft", RightToLeft);
            jObject.AddPropertyDouble("ImageMultipleFactor", ImageMultipleFactor, 1d);
            jObject.AddPropertyInt("ImageTransparency", ImageTransparency);
            jObject.AddPropertyEnum("ImageAlignment", ImageAlignment, ContentAlignment.MiddleCenter);
            jObject.AddPropertyBool("ImageTiling", ImageTiling);
            jObject.AddPropertyBool("ImageStretch", ImageStretch);
            jObject.AddPropertyBool("AspectRatio", AspectRatio);
            jObject.AddPropertyStringNullOrEmpty("EnabledExpression", EnabledExpression);
       
            if (ExistImage())
                jObject.Add("ImageBytes", global::System.Convert.ToBase64String(TakeImage()));

            if (jObject.Count == 0)
                return null;

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Font":
						this.Font = property.DeserializeFont(this.Font);
                        break;

                    case "TextBrush":
                        this.TextBrush = property.DeserializeBrush();
                        break;

                    case "Text":
                        this.Text = property.DeserializeString();
                        break;

                    case "Angle":
                        this.Angle = property.DeserializeFloat();
                        break;

                    case "Enabled":
                        this.Enabled = property.DeserializeBool();
                        break;

                    case "ShowImageBehind":
                        this.ShowImageBehind = property.DeserializeBool();
                        break;

                    case "ShowBehind":
                        this.ShowBehind = property.DeserializeBool();
                        break;

                    case "RightToLeft":
                        this.RightToLeft = property.DeserializeBool();
                        break;

                    case "ImageMultipleFactor":
                        this.ImageMultipleFactor = property.DeserializeDouble();
                        break;

                    case "ImageTransparency":
                        this.imageTransparency = property.DeserializeInt();
                        break;

                    case "Image":
						this.PutImage(property.DeserializeImage());
                        break;

                    case "ImageBytes":
                        this.PutImage(property.DeserializeImage());
                        break;

                    case "ImageHyperlink":
                        this.ImageHyperlink = property.DeserializeString();
                        break;

                    case "ImageAlignment":
                        this.ImageAlignment = property.DeserializeEnum<ContentAlignment>();
                        break;

                    case "ImageTiling":
                        this.ImageTiling = property.DeserializeBool();
                        break;

                    case "ImageStretch":
                        this.ImageStretch = property.DeserializeBool();
                        break;

                    case "AspectRatio":
                        this.AspectRatio = property.DeserializeBool();
                        break;

                    case "EnabledExpression":
                        this.EnabledExpression = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

		#region ICloneable override
		public object Clone()
		{
		    var watermark = (StiWatermark)base.MemberwiseClone();

			watermark.Font = this.Font.Clone() as Font;
			watermark.TextBrush = this.TextBrush.Clone() as StiBrush;

			return watermark;
		}
        #endregion

	    #region IStiDefault
	    [Browsable(false)]
	    public virtual bool IsDefault
	    {
	        get
	        {
	            return !ShouldSerializeFont() &&
	                   !ShouldSerializeTextBrush() &&
	                   Text != null && Text.Length == 0 &&
	                   Angle == 45f &&
	                   Enabled &&
	                   ShowImageBehind &&
	                   !ShowBehind &&
	                   !RightToLeft &&
	                   ImageMultipleFactor == 1d &&
	                   ImageTransparency == 0 &&
	                   ImageBytes == null &&
	                   ImageHyperlink != null && ImageHyperlink.Length == 0 &&
	                   ImageAlignment == ContentAlignment.MiddleCenter &&
	                   !ImageTiling &&
	                   !ImageStretch &&
	                   !AspectRatio &&
	                   EnabledExpression != null && EnabledExpression.Length == 0;
	        }
	    }
	    #endregion

        #region Fields
        private byte[] cachedImage;
        #endregion

        #region Properties
	    /// <summary>
		/// Gets or sets a font of the watermark.
		/// </summary>
		[StiSerializable]
		[Description("Gets or sets font of watermark.")]
		[StiOrder(StiPropertyOrder.WatermarkFont)]
        [StiPropertyLevel(StiLevel.Basic)]
		public Font Font { get; set; } = new Font("Arial", 100);

	    [EditorBrowsable(EditorBrowsableState.Never)]
		internal bool ShouldSerializeFont()
		{
			return !(Font.Size == 100 & Font.Style == FontStyle.Regular & Font.Name == "Arial");
		}

		/// <summary>
		/// The brush of the watermark, which is used to display text.
		/// </summary>
		[StiSerializable]
		[Description("The brush of the watermark, which is used to display text.")]
		[StiOrder(StiPropertyOrder.WatermarkTextBrush)]
        [StiPropertyLevel(StiLevel.Basic)]
		public StiBrush TextBrush { get; set; } = new StiSolidBrush(Color.FromArgb(50, 0, 0, 0));
        
	    [EditorBrowsable(EditorBrowsableState.Never)]
		internal bool ShouldSerializeTextBrush()
		{
			return !(TextBrush is StiSolidBrush && ((StiSolidBrush)TextBrush).Color == Color.FromArgb(50, 0, 0, 0));
		}

	    /// <summary>
		/// Gets or sets a text of the watermark.
		/// </summary>
		[StiSerializable]
		[DefaultValue("")]
		[Description("Gets or sets text of Watermark.")]
		[StiOrder(StiPropertyOrder.WatermarkText)]
		[Editor("Stimulsoft.Report.Components.Design.StiSimpleTextEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual string Text { get; set; } = string.Empty;

	    /// <summary>
		/// Gets or sets an angle of the watermark.
		/// </summary>
		[StiSerializable]
		[DefaultValue(45f)]
		[Description("Gets or sets angle of Watermark.")]
		[StiOrder(StiPropertyOrder.WatermarkAngle)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual float Angle { get; set; } = 45f;

	    /// <summary>
		/// Gets or sets a value which indicates that watermark should be drawn or not.
		/// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value which indicates where Watermark should be drawing or not.")]
		[StiOrder(StiPropertyOrder.WatermarkEnabled)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual bool Enabled { get; set; } = true;

	    /// <summary>
		/// Gets or sets a value which indicates that the watermark's image should be drawn behind or in a front of a page.
		/// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value which indicates where Watermark image should be drawing behind or in front of page.")]
		[StiOrder(StiPropertyOrder.WatermarkShowImageBehind)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool ShowImageBehind { get; set; } = true;

	    /// <summary>
		/// Gets or sets a value which indicates that the watermark should be drawn behind or in a front of a page.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value which indicates where Watermark should be drawing behind or in front of page.")]
		[StiOrder(StiPropertyOrder.WatermarkShowBehind)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool ShowBehind { get; set; }

	    /// <summary>
        /// Gets or sets the watermark's output direction.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets watermark's output direction.")]
        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.All)]
        [StiPropertyLevel(StiLevel.Basic)]
        public bool RightToLeft { get; set; }

	    /// <summary>
		/// Gets or sets a value to multiply by its an image size.
		/// </summary>
		[StiSerializable]
		[DefaultValue(1d)]
		[Description("Gets or sets value to multiply by it an image size.")]
		[StiOrder(StiPropertyOrder.WatermarkImageMultipleFactor)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual double ImageMultipleFactor { get; set; } = 1d;

	    private int imageTransparency;
		/// <summary>
		/// Gets or sets a transparency of the watermark's image.
		/// </summary>
		[StiSerializable]
		[Description("Gets or sets the transparency of the watermark's image.")]
		[DefaultValue(0)]
		[StiOrder(StiPropertyOrder.WatermarkImageTransparency)]
        [StiPropertyLevel(StiLevel.Basic)]
		public int ImageTransparency
		{
			get
			{
				return imageTransparency;
			}
			set
			{
				
				value = Math.Max(0, Math.Min(value, 0xff));
				if (value != imageTransparency)
				{
					imageTransparency = value;
					DisposeCachedImage();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value of the watermark's image.
		/// </summary>
		[StiNonSerialized]
		[DefaultValue(null)]
		[Description("Gets or sets value watermark's image.")]
		[Editor("Stimulsoft.Report.Components.Design.StiSimpleImageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiSimpeImageConverter))]
		[StiOrder(StiPropertyOrder.WatermarkImage)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual Image Image
		{
		    get
		    {
		        return TakeGdiImage();
		    }
		    set
		    {
		        PutImage(value);
		    }
		}

	    private byte[] imageBytes;
        /// <summary>
        /// Gets or sets an image.
        /// </summary>
        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(null)]
        [Description("Gets or sets value watermark's image.")]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleImageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiSimpeImageConverter))]
        [StiOrder(StiPropertyOrder.WatermarkImage)]
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
	                DisposeCachedImage();
                    PutImage(value);
	            }
	        }
	    }

        private string imageHyperlink = "";
	    /// <summary>
	    /// Gets or sets a value of the watermark's image hyperlink.
	    /// </summary>
	    [StiSerializable]
	    [DefaultValue("")]
	    [Description("Gets or sets value watermark's image hyperlink.")]
	    [Browsable(false)]
	    public string ImageHyperlink
	    {
	        get
	        {
	            return imageHyperlink; 
	        }
	        set
	        {
	            if (imageHyperlink != value)
	            {
	                imageHyperlink = value;
                    DisposeCachedImage();
	            }
	        }
	    }

	    /// <summary>
		/// Gets or sets the watermark's image alignment.
		/// </summary>
		[StiSerializable]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets the watermark's image alignment.")]
		[StiOrder(StiPropertyOrder.WatermarkImageAlignment)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual ContentAlignment ImageAlignment { get; set; } = ContentAlignment.MiddleCenter;

	    /// <summary>
		/// Gets or sets the watermark's image should be tiled.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets the watermark's image should be tiled.")]
		[StiOrder(StiPropertyOrder.WatermarkImageTiling)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool ImageTiling { get; set; }

	    /// <summary>
		/// Gets or sets a value of this watermark's image will be stretched on the page.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value, indicates that this watermark's image will stretch on the page.")]
		[StiOrder(StiPropertyOrder.WatermarkImageStretch)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool ImageStretch { get; set; }

	    /// <summary>
        /// Gets or sets a value that indicates that this watermark's image will save its aspect ratio.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageAspectRatio)]
        [TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value, indicates this watermark's image will save its aspect ratio.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool AspectRatio { get; set; }

	    /// <summary>
        /// Gets or sets an expression of the Enabled property.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [DefaultValue("")]
        [Description("Gets or sets text of EnabledExpression.")]
        [StiOrder(StiPropertyOrder.WatermarkText)]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleTextEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual string EnabledExpression { get; set; } = string.Empty;
	    #endregion

        #region Methods
	    /// <summary>
	    /// Internal use only.
	    /// </summary>
	    public byte[] GetImage(StiReport report)
	    {
	        var image = string.IsNullOrWhiteSpace(ImageHyperlink) 
                ? TakeImage() 
                : StiHyperlinkProcessor.GetBytes(report, ImageHyperlink);

	        if (ImageTransparency == 0) 
				return image;

            if (image == null) 
				return null;

            if (cachedImage == null)
	        {
	            var gdiImage = StiImageConverter.BytesToImage(image);
	            var gdiTransparentedImage = StiImageTransparenceHelper.GetTransparentedImage(gdiImage, 1f - this.ImageTransparency / 255f);
                cachedImage = StiImageConverter.ImageToBytes(gdiTransparentedImage);
	        }

	        return cachedImage;
	    }

        private void DisposeCachedImage()
		{
			this.cachedImage = null;
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
	        return ExistImage()
	            ? StiImageConverter.BytesToImage(ImageBytes)
	            : null;
	    }

	    internal Image TryTakeGdiImage()
	    {
	        return ExistImage()
	            ? StiImageConverter.TryBytesToImage(ImageBytes)
	            : null;
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
        #endregion

        public StiWatermark()
		{
		    ImageHyperlink = string.Empty;
		}

		public StiWatermark(StiBrush textBrush, string text, float angle, Font font, bool showBehind) : 
			this(textBrush, text, angle, font, showBehind, true, false)
		{
		}

		public StiWatermark(StiBrush textBrush, string text, float angle, 
			Font font, bool showBehind, bool enabled, bool aspectRatio) : 
            this(textBrush, text, angle, font, showBehind, enabled, aspectRatio, false)
		{
		}

	    public StiWatermark(StiBrush textBrush, string text, float angle,
	        Font font, bool showBehind, bool enabled, bool aspectRatio, bool rightToLeft) : 
            this(textBrush, text, angle, font, showBehind, enabled, aspectRatio, rightToLeft, string.Empty)
	    {
	        
	    }

        public StiWatermark(StiBrush textBrush, string text, float angle,
            Font font, bool showBehind, bool enabled, bool aspectRatio, bool rightToLeft, string imageHyperlink)
        {
            this.ImageHyperlink = imageHyperlink;
            this.TextBrush = textBrush;
            this.Text = text;
            this.Angle = angle;
            this.Font = font;
            this.ShowBehind = showBehind;
            this.Enabled = enabled;
            this.AspectRatio = aspectRatio;
            this.RightToLeft = rightToLeft;
        }
	}
}
