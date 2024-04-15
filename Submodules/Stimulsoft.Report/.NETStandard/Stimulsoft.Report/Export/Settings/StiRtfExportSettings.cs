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
    /// Class describes settings for export to RTF format.
    /// </summary>
	public class StiRtfExportSettings : 
        StiPageRangeExportSettings
    {
        #region Methods
        public override StiExportFormat GetExportFormat()
        {
            return StiExportFormat.Rtf;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets code page of RTF file.
        /// </summary>
        [DefaultValue(0)]
		public int CodePage { get; set; }

        /// <summary>
        /// Gets or sets mode of RTF file creation.
        /// </summary>
        [DefaultValue(StiRtfExportMode.Table)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiRtfExportMode ExportMode { get; set; } = StiRtfExportMode.Table;

        /// <summary>
        /// Gets or sets value which enables special mode of exporting page headers and page footers into result file. In this mode export engine try to insert all page 
        /// headers and footers as RTF headers and footers.
        /// </summary>
        [DefaultValue(false)]
		public bool UsePageHeadersAndFooters { get; set; }

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
        /// Gets or sets a value indicating whether it is necessary to remove empty space at the bottom of the page
        /// </summary>
        [DefaultValue(true)]
        public bool RemoveEmptySpaceAtBottom { get; set; } = StiOptions.Export.RichText.RemoveEmptySpaceAtBottom;

        /// <summary>
        /// Gets or sets a value indicating whether it is necessary to store images in Png format
        /// </summary>
        [DefaultValue(false)]
        public bool StoreImagesAsPng { get; set; }
        #endregion
    }
}
