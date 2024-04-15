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

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using System.ComponentModel;

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// Base class which describes export settings for all types of exports to image formats.
    /// </summary>
	public class StiImageExportSettings : StiPageRangeExportSettings
    {
        #region Methods
        public override StiExportFormat GetExportFormat()
        {
            switch (ImageType)
            {
                case StiImageType.Bmp:
                    return StiExportFormat.ImageBmp;

                case StiImageType.Emf:
                    return StiExportFormat.ImageEmf;

                case StiImageType.Gif:
                    return StiExportFormat.ImageGif;

                case StiImageType.Jpeg:
                    return StiExportFormat.ImageJpeg;

                case StiImageType.Pcx:
                    return StiExportFormat.ImagePcx;

                case StiImageType.Png:
                    return StiExportFormat.ImagePng;

                case StiImageType.Svg:
                    return StiExportFormat.ImageSvg;

                case StiImageType.Svgz:
                    return StiExportFormat.ImageSvgz;

                default:
                    return StiExportFormat.ImageTiff;
            }
        }
        #endregion

        #region Properties
        //DEVELOPER! Do not convert to auto property!
        private StiImageType imageType;
        /// <summary>
        /// Gets or sets image type for exported images.
        /// </summary>
        [DefaultValue(StiImageType.Jpeg)]
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual StiImageType ImageType
        {
            get
            {
                return imageType;
            }
            set
            {
                imageType = value;
            }
        }

        /// <summary>
        /// Gets or sets image zoom factor for exported images. This property can't be used with EMF, SVG, SVGZ formats.
        /// </summary>
        [DefaultValue(1d)]
        public double ImageZoom { get; set; } = 1d;

        /// <summary>
        /// Gets or sets image resolution for exported images. This property can't be used with EMF, SVG, SVGZ formats.
        /// </summary>
        [DefaultValue(100)]
        public int ImageResolution { get; set; } = 100;

        /// <summary>
        /// Gets or sets value which indicates that page margins will be cut or not. This property can't be used with EMF, SVG, SVGZ formats.
        /// </summary>
        [DefaultValue(false)]
        public bool CutEdges { get; set; }

        /// <summary>
        /// Gets or sets image format for exported images. This property can't be used with EMF, SVG, SVGZ formats.
        /// </summary>
        [DefaultValue(StiImageFormat.Color)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiImageFormat ImageFormat { get; set; } = StiImageFormat.Color;

        /// <summary>
        /// Gets or sets value which indicates that export engine will be create one solid file or multiple files (one file per page).
        /// This property can't be used with EMF, SVG, SVGZ formats.
        /// </summary>
        [DefaultValue(false)]
        public virtual bool MultipleFiles { get; set; }

        /// <summary>
        /// Gets or sets type of dithering. This property can't be used with EMF, SVG, SVGZ formats.
        /// </summary>
        [DefaultValue(StiMonochromeDitheringType.FloydSteinberg)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiMonochromeDitheringType DitheringType { get; set; } = StiMonochromeDitheringType.FloydSteinberg;

        /// <summary>
        /// Gets or sets compression scheme of TIFF format. This property can't be used with EMF, SVG, SVGZ formats.
        /// </summary>
        [DefaultValue(StiTiffCompressionScheme.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiTiffCompressionScheme TiffCompressionScheme { get; set; } = StiTiffCompressionScheme.Default;

        /// <summary>
        /// Gets or sets a value indicating whether it is necessary to save output files as zip-file.
        /// </summary>
        [DefaultValue(false)]
        public bool CompressToArchive { get; set; } = false;
        #endregion

        public StiImageExportSettings() :
            this(StiImageType.Jpeg)
        {
        }

        public StiImageExportSettings(StiImageType imageType)
        {
            this.imageType = imageType;
        }
    }
}
