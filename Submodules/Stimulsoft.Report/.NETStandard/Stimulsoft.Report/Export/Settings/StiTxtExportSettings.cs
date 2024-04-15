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
using System.Text;

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// Class describes settings for export to TXT format.
    /// </summary>
	public class StiTxtExportSettings : 
        StiPageRangeExportSettings
    {
        #region Methods
        public override StiExportFormat GetExportFormat()
        {
            return StiExportFormat.Text;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets encoding of result text file.
        /// </summary>
		[JsonConverter(typeof(StiEncodingJsonConverter))]
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets or sets value which indicates that borders will be drawn or not.
        /// </summary>
        [DefaultValue(true)]
		public bool DrawBorder { get; set; } = true;

        /// <summary>
        /// Gets or sets type of drawing border.
        /// </summary>
        [DefaultValue(StiTxtBorderType.UnicodeSingle)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiTxtBorderType BorderType { get; set; } = StiTxtBorderType.UnicodeSingle;

        /// <summary>
        /// Gets or sets value which indicates that empty space lines will be removed.
        /// </summary>
        [DefaultValue(true)]
		public bool KillSpaceLines { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that empty graph space lines will be removed.
        /// </summary>
        [DefaultValue(true)]
		public bool KillSpaceGraphLines { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that special FeedPageCode marker will be placed in result file.
        /// </summary>
        [DefaultValue(true)]
		public bool PutFeedPageCode { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that long text lines will be cut.
        /// </summary>
        [DefaultValue(true)]
		public bool CutLongLines { get; set; } = true;

        /// <summary>
        /// Gets or sets horizontal zoom factor by X axis. By default a value is 1.0f what is equal 100% in export settings window
        /// </summary>
        [DefaultValue(1f)]
		public float ZoomX { get; set; } = 1f;

        /// <summary>
        /// Gets or sets vertical zoom factor by Y axis. By default a value is 1.0f what is equal 100% in export settings window
        /// </summary>
        [DefaultValue(1f)]
		public float ZoomY { get; set; } = 1f;

        /// <summary>
        /// Gets or sets value which indicates that Escape codes will be used.
        /// </summary>
        [DefaultValue(false)]
		public bool UseEscapeCodes { get; set; }

        /// <summary>
        /// Gets or sets value which indicates a EscapeCodesCollection name.
        /// </summary>
        [DefaultValue(null)]
		public string EscapeCodesCollectionName { get; set; }
        #endregion
    }
}
