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

using System.ComponentModel;
using System.Text;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base.Json;
using System.Drawing.Imaging;

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Export
{    
    /// <summary>
    /// Class describes settings for export to HTML format.
    /// </summary>
	public class StiHtmlExportSettings : 
        StiPageRangeExportSettings
    {
        #region Methods
        public override StiExportFormat GetExportFormat()
        {
            switch (HtmlType)
            {
                case StiHtmlType.Html5:
                    return StiExportFormat.Html5;

                case StiHtmlType.Mht:
                    return StiExportFormat.Mht;

                default:
                    return StiExportFormat.Html;
            }
        }
        #endregion

        #region Properties
        //DEVELOPER! Do not convert to auto property!
        private StiHtmlType htmlType;
        /// <summary>
        /// Gets or sets type of the exported html file.
        /// </summary>
        [DefaultValue(StiHtmlType.Html)]
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual StiHtmlType HtmlType
        {
            get
            {
                return htmlType;
            }
            set
            {
                htmlType = value;
            }
        }

        /// <summary>
        /// Gets or sets image quality of images which will be exported to result file.
        /// </summary>
        [DefaultValue(0.75f)]
        public float ImageQuality { get; set; } = 0.75f;

        /// <summary>
        /// Gets or sets image resolution of images which will be exported to result file.
        /// </summary>
        [DefaultValue(100f)]
        public float ImageResolution { get; set; } = 100f;

        /// <summary>
        /// Gets or sets image format for exported images.
        /// </summary>
        [JsonConverter(typeof(StiImageFormatJsonConverter))]
        public ImageFormat ImageFormat { get; set; }

        /// <summary>
        /// Gets or sets encoding of html file.
        /// </summary>
        [JsonConverter(typeof(StiEncodingJsonConverter))]
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets zoom factor of exported file. HTML5 export mode is not supported.
        /// </summary>
        [DefaultValue(1d)]
        public double Zoom { get; set; } = 1d;

        /// <summary>
        /// Gets or sets mode of html file creation. HTML5 export mode is not supported.
        /// </summary>
        [DefaultValue(StiHtmlExportMode.Table)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiHtmlExportMode ExportMode { get; set; } = StiHtmlExportMode.Table;

        /// <summary>
        /// Gets or sets quality of html file. HTML5 export mode is not supported.
        /// </summary>
        [DefaultValue(StiHtmlExportQuality.High)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiHtmlExportQuality ExportQuality { get; set; } = StiHtmlExportQuality.High;

        /// <summary>
        /// Gets or sets value which indicates that special page breaks marker will be added to result html file. HTML5 export mode is not supported.
        /// </summary>
        [DefaultValue(true)]
        public bool AddPageBreaks { get; set; } = true;

        /// <summary>
        /// Gets or sets defaullt width of bookmarks tree. HTML5 export mode is not supported.
        /// </summary>
        [DefaultValue(150)]
        public int BookmarksTreeWidth { get; set; } = 150;

        /// <summary>
        /// Gets or sets export mode of bookmarks tree. HTML5 export mode is not supported.
        /// </summary>
        [DefaultValue(StiHtmlExportBookmarksMode.All)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiHtmlExportBookmarksMode ExportBookmarksMode { get; set; } = StiHtmlExportBookmarksMode.All;

        /// <summary>
        /// Gets or sets value which indicates that table styles will be used in result html file. HTML5 and MHT export mode is not supported.
        /// </summary>
        [DefaultValue(true)]
        public bool UseStylesTable { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether it is necessary to remove empty space at the bottom of the page. HTML5 and MHT export mode is not supported.
        /// </summary>
        [DefaultValue(true)]
        public bool RemoveEmptySpaceAtBottom { get; set; } = StiOptions.Export.Html.RemoveEmptySpaceAtBottom;

        /// <summary>
        /// Gets or sets the horizontal alignment of pages. HTML5 and MHT export mode is not supported.
        /// </summary>
        [DefaultValue(StiHorAlignment.Left)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiHorAlignment PageHorAlignment { get; set; } = StiHorAlignment.Left;

        /// <summary>
        /// Gets or sets a value indicating whether it is necessary to save output file as zip-file. HTML5 and MHT export mode is not supported.
        /// </summary>
        [DefaultValue(false)]
        public bool CompressToArchive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is necessary to save images as embedded data in html file. HTML5 and MHT export mode is not supported.
        /// </summary>
        [DefaultValue(false)]
        public bool UseEmbeddedImages { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that all report pages will be shown as vertical ribbon. HTML and MHT export mode is not supported.
        /// </summary>
        [DefaultValue(true)]
        public bool ContinuousPages { get; set; } = true;

        /// <summary>
        /// Gets or sets a target to open links from the exported report.
        /// </summary>
        [DefaultValue(null)]
        public string OpenLinksTarget { get; set; } = StiOptions.Export.Html.OpenLinksTarget;

        /// <summary>
        /// Gets or sets type of the chart in the exported html file. HTML5 export mode is not supported.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual StiHtmlChartType ChartType { get; set; } = StiOptions.Export.Html.ChartAsBitmap 
            ? StiHtmlChartType.Image 
            : (StiOptions.Viewer.InteractionChartOnFirstLook ? StiHtmlChartType.AnimatedVector : StiHtmlChartType.Vector);

        [DefaultValue(false)]
        public bool UseWatermarkMargins { get; set; }
        #endregion

        public StiHtmlExportSettings() : 
            this(StiHtmlType.Html)
        {
        }

        public StiHtmlExportSettings(StiHtmlType htmlType)
        {
            this.htmlType = htmlType;
            this.Encoding = Encoding.UTF8;
            this.ImageFormat = ImageFormat.Png;
        }
	}
}
