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

using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Drawing;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// The class describes the watermark of the container.
    /// </summary>
    [TypeConverter(typeof(StiUniversalConverter))]
    [RefreshProperties(RefreshProperties.All)]
    public class StiAdvancedWatermark :
        ICloneable,
        IStiSerializeToCodeAsClass,
        IStiJsonReportObject,
        IStiDefault
    {
        #region Consts
        private static readonly Color DefaultWeaveMajorColor = StiColor.Get("#77777777");
        private static readonly Color DefaultWeaveMinorColor = StiColor.Get("#55777777");
        private static readonly Color DefaultTextColor = Color.Gray;
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool(nameof(TextEnabled), TextEnabled);

            if (Text != "Text")
                jObject.AddPropertyStringNullOrEmpty(nameof(Text), Text);

            jObject.AddPropertyFont(nameof(TextFont), TextFont, "Arial", 36);
            jObject.AddPropertyColor(nameof(TextColor), TextColor, DefaultTextColor);

            jObject.AddPropertyInt(nameof(TextAngle), TextAngle, 45);

            jObject.AddPropertyBool(nameof(ImageEnabled), ImageEnabled);
            if (ExistImage())
                jObject.AddPropertyByteArray(nameof(ImageBytes), TakeImage());

            jObject.AddPropertyDouble(nameof(ImageMultipleFactor), ImageMultipleFactor, 1d);
            jObject.AddPropertyInt(nameof(ImageTransparency), ImageTransparency);
            jObject.AddPropertyEnum(nameof(ImageAlignment), ImageAlignment, ContentAlignment.MiddleCenter);
            jObject.AddPropertyBool(nameof(ImageTiling), ImageTiling);
            jObject.AddPropertyBool(nameof(ImageStretch), ImageStretch);
            jObject.AddPropertyBool(nameof(ImageAspectRatio), ImageAspectRatio);

            jObject.AddPropertyBool(nameof(WeaveEnabled), WeaveEnabled);
            if (WeaveMajorIcon != null)
                jObject.AddPropertyEnum(nameof(WeaveMajorIcon), WeaveMajorIcon);
            jObject.AddPropertyInt(nameof(WeaveMajorSize), WeaveMajorSize, 20);
            jObject.AddPropertyColor(nameof(WeaveMajorColor), WeaveMajorColor, DefaultWeaveMajorColor);

            if (WeaveMinorIcon != null)
                jObject.AddPropertyEnum(nameof(WeaveMinorIcon), WeaveMinorIcon);

            jObject.AddPropertyInt(nameof(WeaveMinorSize), WeaveMinorSize, 10);
            jObject.AddPropertyColor(nameof(WeaveMinorColor), WeaveMinorColor, DefaultWeaveMinorColor);

            jObject.AddPropertyInt(nameof(WeaveAngle), WeaveAngle, 30);
            jObject.AddPropertyInt(nameof(WeaveDistance), WeaveDistance, 100);

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
                    case nameof(TextEnabled):
                        this.TextEnabled = property.DeserializeBool();
                        break;

                    case nameof(Text):
                        this.Text = property.DeserializeString();
                        break;

                    case nameof(TextFont):
                        this.TextFont = property.DeserializeFont(this.TextFont);
                        break;

                    case nameof(TextColor):
                        TextColor = property.DeserializeColor();
                        break;

                    case nameof(TextAngle):
                        this.TextAngle = property.DeserializeInt();
                        break;

                    case nameof(ImageEnabled):
                        this.ImageEnabled = property.DeserializeBool();
                        break;

                    case nameof(ImageBytes):
                        this.PutImage(property.DeserializeImage());
                        break;

                    case nameof(ImageMultipleFactor):
                        this.ImageMultipleFactor = property.DeserializeDouble();
                        break;

                    case nameof(ImageTransparency):
                        this.imageTransparency = property.DeserializeInt();
                        break;

                    case nameof(ImageAlignment):
                        this.ImageAlignment = property.DeserializeEnum<ContentAlignment>();
                        break;

                    case nameof(ImageTiling):
                        this.ImageTiling = property.DeserializeBool();
                        break;

                    case nameof(ImageStretch):
                        this.ImageStretch = property.DeserializeBool();
                        break;

                    case nameof(ImageAspectRatio):
                        this.ImageAspectRatio = property.DeserializeBool();
                        break;

                    case nameof(WeaveEnabled):
                        this.WeaveEnabled = property.DeserializeBool();
                        break;

                    case nameof(WeaveMajorIcon):
                        this.WeaveMajorIcon = property.DeserializeEnum<StiFontIcons>();
                        break;

                    case nameof(WeaveMajorSize):
                        this.WeaveMajorSize = property.DeserializeInt();
                        break;

                    case nameof(WeaveMajorColor):
                        WeaveMajorColor = property.DeserializeColor();
                        break;

                    case nameof(WeaveMinorIcon):
                        this.WeaveMinorIcon = property.DeserializeEnum<StiFontIcons>();
                        break;

                    case nameof(WeaveMinorSize):
                        this.WeaveMinorSize = property.DeserializeInt();
                        break;

                    case nameof(WeaveMinorColor):
                        WeaveMinorColor = property.DeserializeColor();
                        break;

                    case nameof(WeaveAngle):
                        this.WeaveAngle = property.DeserializeInt();
                        break;

                    case nameof(WeaveDistance):
                        this.WeaveDistance = property.DeserializeInt();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable override
        public object Clone()
        {
            var watermark = MemberwiseClone() as StiAdvancedWatermark;

            watermark.TextFont = this.TextFont.Clone() as Font;

            return watermark;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public virtual bool IsDefault
        {
            get
            {
                return
                    !TextEnabled &&
                    Text == "Text" &&
                    !ShouldSerializeTextFont() &&
                    TextColor == DefaultTextColor &&
                    TextAngle == 45 &&

                    !ImageEnabled &&
                    ImageBytes == null &&
                    ImageMultipleFactor == 1d &&
                    ImageTransparency == 0 &&
                    ImageAlignment == ContentAlignment.MiddleCenter &&
                    !ImageTiling &&
                    !ImageStretch &&
                    !ImageAspectRatio &&

                    !WeaveEnabled &&
                    WeaveMajorIcon == StiFontIcons.Star &&
                    WeaveMajorSize == 20 &&
                    WeaveMajorColor == DefaultWeaveMajorColor &&
                    WeaveMinorIcon == null &&
                    WeaveMinorSize == 10 &&
                    WeaveMinorColor == DefaultWeaveMinorColor &&
                    WeaveAngle == 30 &&
                    WeaveDistance == 100;
            }
        }
        #endregion

        #region Fields
        private byte[] cachedImage;
        #endregion

        #region Properties
        [Browsable(false)]
        public bool IsVisible
        {
            get
            {
                return
                    (TextEnabled && !string.IsNullOrEmpty(Text)) ||
                    (ImageEnabled && ImageBytes != null) ||
                    (WeaveEnabled && (WeaveMajorIcon != null || WeaveMajorIcon != null));
            }
        }
        #endregion

        #region Properties.Text
        /// <summary>
        /// Gets or sets a value which indicates that watermark's text is visible or not.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        public virtual bool TextEnabled { get; set; }

        /// <summary>
        /// Gets or sets text of the watermark.
        /// </summary>
        [StiSerializable]
        [DefaultValue("Text")]
        public virtual string Text { get; set; } = "Text";

        /// <summary>
        /// Gets or sets a font of the watermark text.
        /// </summary>
        [StiSerializable]
        public Font TextFont { get; set; } = new Font("Arial", 36);

        internal bool ShouldSerializeTextFont()
        {
            return !(TextFont.Size == 36 & TextFont.Style == FontStyle.Regular & TextFont.Name == "Arial");
        }

        /// <summary>
        /// Gets or sets a color of the watermark text.
        /// </summary>
        [StiSerializable]
        public Color TextColor { get; set; } = DefaultTextColor;

        internal bool ShouldSerializeTextColor()
        {
            return TextColor != DefaultTextColor;
        }

        /// <summary>
        /// Gets or sets an angle of the watermark text.
        /// </summary>
        [StiSerializable]
        [DefaultValue(45)]
        public virtual int TextAngle { get; set; } = 45;
        #endregion

        #region Properties.Image
        /// <summary>
        /// Gets or sets a value which indicates that watermark's image is visible or not.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        public virtual bool ImageEnabled { get; set; }

        /// <summary>
        /// Gets or sets value a watermark's image.
        /// </summary>
        [StiNonSerialized]
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
        /// Gets or sets a bytes representation of the watermark image.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
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

        /// <summary>
        /// Gets or sets a value to multiply by it an watermark's image size.
        /// </summary>
        [StiSerializable]
        [DefaultValue(1d)]
        public virtual double ImageMultipleFactor { get; set; } = 1d;

        private int imageTransparency;
        /// <summary>
        /// Gets or sets a transparency of the watermark's image.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0)]
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
        /// Gets or sets an alignment of the watermark's image.
        /// </summary>
        [StiSerializable]
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public virtual ContentAlignment ImageAlignment { get; set; } = ContentAlignment.MiddleCenter;

        /// <summary>
        /// Gets or sets a value which indicates that watermark's image should be tiled.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        public virtual bool ImageTiling { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates that this watermark's image will be stretched on the page.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        public virtual bool ImageStretch { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that this watermark's image will save its aspect ratio.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        public bool ImageAspectRatio { get; set; }
        #endregion

        #region Properties.Weave
        /// <summary>
        /// Gets or sets a value which indicates that watermark's weave is visible or not.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        public virtual bool WeaveEnabled { get; set; }

        [StiSerializable]
        [DefaultValue(null)]
        public StiFontIcons? WeaveMajorIcon { get; set; }

        private int weaveMajorSize = 20;
        [StiSerializable]
        [DefaultValue(20)]
        public int WeaveMajorSize
        {
            get
            {
                return weaveMajorSize;
            }
            set
            {
                if (weaveMajorSize != value)
                {
                    if (value < 5)
                        value = 5;

                    if (value > 30)
                        value = 30;

                    weaveMajorSize = value;
                }
            }
        }
        [StiSerializable]
        public Color WeaveMajorColor { get; set; } = DefaultWeaveMajorColor;

        internal bool ShouldSerializeWeaveMajorColor()
        {
            return WeaveMajorColor != DefaultWeaveMajorColor;
        }

        [StiSerializable]
        [DefaultValue(null)]
        public StiFontIcons? WeaveMinorIcon { get; set; }

        private int weaveMinorSize = 10;
        [StiSerializable]
        [DefaultValue(10)]
        public int WeaveMinorSize
        {
            get
            {
                return weaveMinorSize;
            }
            set
            {
                if (weaveMinorSize != value)
                {
                    if (value < 5)
                        value = 5;

                    if (value > 30)
                        value = 30;

                    weaveMinorSize = value;
                }
            }
        }
        [StiSerializable]
        public Color WeaveMinorColor { get; set; } = DefaultWeaveMinorColor;

        internal bool ShouldSerializeWeaveMinorColor()
        {
            return WeaveMinorColor != DefaultWeaveMinorColor;
        }

        private int weaveAngle = 30;
        [StiSerializable]
        [DefaultValue(30)]
        public int WeaveAngle
        {
            get 
            { 
                return weaveAngle; 
            }
            set
            {
                if (weaveAngle != value)
                {
                    if (value < 0)
                        value = 0;

                    if (value > 360)
                        value = 360;

                    weaveAngle = value;
                }
            }
        }

        private int weaveDistance = 100;
        [StiSerializable]
        [DefaultValue(100)]
        public int WeaveDistance
        {
            get
            {
                return weaveDistance;
            }
            set
            {
                if (weaveDistance != value)
                {
                    if (value < 50)
                        value = 50;

                    if (value > 200)
                        value = 200;

                    weaveDistance = value;
                }
            }
        }
        #endregion

        #region Methods
        internal byte[] GetCachedImage()
        {
            return this.cachedImage;
        }

        internal void PutCachedImage(byte[] image)
        {
            this.cachedImage = image;
        }

        private void DisposeCachedImage()
        {
            this.cachedImage = null;
            StiAdvancedWatermarkImageUtils.DisposeCachedImage();
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
        #endregion

        public StiAdvancedWatermark()
        {
        }

        [StiUniversalConstructor("Watermark")]
        public StiAdvancedWatermark(
            bool textEnabled, string text, Color textColor, int textAngle, Font textFont,
            bool imageEnabled, byte[] imageBytes, double imageMultipleFactor, int imageTransparency, 
            ContentAlignment imageAlignment, bool imageTiling, bool imageStretch, bool imageAspectRatio,
            bool weaveEnabled, StiFontIcons? weaveMajorIcon, int weaveMajorSize, Color weaveMajorColor,
            StiFontIcons? weaveMinorIcon, int weaveMinorSize, Color weaveMinorColor,
            int weaveAngle, int weaveDistance)
        {
            this.TextEnabled = textEnabled;
            this.Text = text;
            this.TextColor = textColor;
            this.TextAngle = textAngle;
            this.TextFont = textFont;

            this.ImageEnabled = imageEnabled;
            this.ImageBytes = imageBytes;
            this.ImageMultipleFactor = imageMultipleFactor;
            this.ImageTransparency = imageTransparency;
            this.ImageAlignment = imageAlignment;
            this.ImageTiling = imageTiling;
            this.ImageStretch = imageStretch;
            this.ImageAspectRatio = imageAspectRatio;

            this.WeaveEnabled = weaveEnabled;
            this.WeaveMajorIcon = weaveMajorIcon;
            this.WeaveMajorSize = weaveMajorSize;
            this.WeaveMajorColor = weaveMajorColor;
            this.WeaveMinorIcon = weaveMinorIcon;
            this.WeaveMinorSize = weaveMinorSize;
            this.WeaveMinorColor = weaveMinorColor;
            this.WeaveAngle = weaveAngle;
            this.WeaveDistance = weaveDistance;
        }
    }
}
