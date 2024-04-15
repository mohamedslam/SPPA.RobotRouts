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

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// Class describes settings for export to XPS format.
    /// </summary>
	public class StiXpsExportSettings : 
        StiPageRangeExportSettings
    {
        #region Methods
        public override StiExportFormat GetExportFormat()
        {
            return StiExportFormat.Xps;
        }
        #endregion

        #region Properties
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
        /// Gets or sets value which indicates that rtf text will be exported as bitmap images or as vector images.
        /// </summary>
        [DefaultValue(false)]
		public bool ExportRtfTextAsImage { get; set; }
        #endregion
    }
}
