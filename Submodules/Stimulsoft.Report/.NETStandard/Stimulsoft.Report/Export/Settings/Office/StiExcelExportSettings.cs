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
using System;
using System.ComponentModel;
using System.Drawing.Imaging;

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// This class describes settings for the export to Excel format.
    /// </summary>
	public class StiExcelExportSettings : 
        StiPageRangeExportSettings
    {
        #region Methods
        public override StiExportFormat GetExportFormat()
        {
            switch (ExcelType)
            {
                case StiExcelType.ExcelXml:
                    return StiExportFormat.ExcelXml;

                case StiExcelType.Excel2007:
                    return StiExportFormat.Excel2007;

                default:
                    return StiExportFormat.Excel;
            }
        }
        #endregion

        #region Properties
        //DEVELOPER! Do not convert to auto property!
        private StiExcelType excelType;
        /// <summary>
        /// Gets or sets type of the exported Excel file.
        /// </summary>
        [DefaultValue(StiExcelType.ExcelBinary)]
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual StiExcelType ExcelType
        {
            get
            {
                return excelType;
            }
            set
            {
                excelType = value;
            }
        }

        /// <summary>
        /// Gets or sets value which indicates that one (first) page header and page footer from report will be used in excel file.
        /// </summary>
        public bool UseOnePageHeaderAndFooter { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that only data information will be created in excel file. 
        /// </summary>
        [Obsolete("Please use property 'DataExportMode'.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ExportDataOnly
        {
            get
            {
                return DataExportMode != StiDataExportMode.AllBands;
            }
            set
            {
                DataExportMode = value ? StiDataExportMode.Data | StiDataExportMode.Headers : StiDataExportMode.AllBands;
            }
        }

        /// <summary>
        /// Gets or sets data export mode.
        /// </summary>
        [DefaultValue(StiDataExportMode.AllBands)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiDataExportMode DataExportMode { get; set; } = StiDataExportMode.AllBands;

        /// <summary>
        /// Gets or sets value which indicates that special page break markers will be created in excel file.
        /// </summary>
        public bool ExportPageBreaks { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that formatting of components will be exported to excel file or not.
        /// </summary>
        [DefaultValue(true)]
        public bool ExportObjectFormatting { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that each page from report will be exported to excel file as separate excel sheet.
        /// </summary>
        public bool ExportEachPageToSheet { get; set; }

        /// <summary>
        /// Gets or sets image quality of images which will be exported to excel file.
        /// </summary>
        [DefaultValue(0.75f)]
        public float ImageQuality { get; set; } = 0.75f;

        /// <summary>
        /// Gets or sets image resolution of images which will be exported to excel file.
        /// </summary>
        [DefaultValue(100f)]
        public float ImageResolution { get; set; } = 100f;

        /// <summary>
        /// Gets or sets image format for exported images. Only for Excel2007+
        /// </summary>
        public ImageFormat ImageFormat { get; set; } = StiOptions.Export.Excel.ImageFormat;

        /// <summary>
        /// Gets or sets information about the creator to be inserted into result Word file. ExcelXml is not supported!
        /// </summary>
        [DefaultValue(null)]
        public string CompanyString { get; set; } = StiExportUtils.GetReportVersion();

        /// <summary>
        /// Gets or sets information about the creator to be inserted into result Word file. ExcelXml is not supported!
        /// </summary>
        [DefaultValue(null)]
        public string LastModifiedString { get; set; } = StiExportUtils.GetReportVersion();

        [DefaultValue(StiExcel2007RestrictEditing.No)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiExcel2007RestrictEditing RestrictEditing { get; set; } = StiExcel2007RestrictEditing.No;
        #endregion
        
        public StiExcelExportSettings()
            : this(StiExcelType.ExcelBinary)
        {
        }

        public StiExcelExportSettings(StiExcelType excelType)
        {
            this.excelType = excelType;
        }
	}
}
