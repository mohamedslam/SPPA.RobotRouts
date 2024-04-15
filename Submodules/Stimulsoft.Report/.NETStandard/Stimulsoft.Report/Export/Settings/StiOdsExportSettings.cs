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
using System.Drawing.Imaging;

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// Class describes settings for export to ODS format.
    /// </summary>
	public class StiOdsExportSettings : 
        StiPageRangeExportSettings
    {
        #region Methods
        public override StiExportFormat GetExportFormat()
        {
            return StiExportFormat.Ods;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets image quality of images which will be exported to ODS file.
        /// </summary>
        [DefaultValue(0.75f)]
		public float ImageQuality { get; set; } = 0.75f;

        /// <summary>
        /// Gets or sets image resolution of images which will be exported to ODS file.
        /// </summary>
        [DefaultValue(100f)]
		public float ImageResolution { get; set; } = 100f;

        /// <summary>
        /// Gets or sets image format for exported images.
        /// </summary>
        public ImageFormat ImageFormat { get; set; } = StiOptions.Export.OpenDocumentCalc.ImageFormat;
        #endregion
    }
}
