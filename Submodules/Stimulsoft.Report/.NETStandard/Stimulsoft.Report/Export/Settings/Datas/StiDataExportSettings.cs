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
using System.Globalization;
using System.Text;

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// Class describes settings for export to Data format.
    /// </summary>
	public class StiDataExportSettings : 
        StiPageRangeExportSettings
    {
        #region Methods
        public override StiExportFormat GetExportFormat()
        {
            switch (DataType)
            {
                case StiDataType.Dbf:
                    return StiExportFormat.Dbf;

                case StiDataType.Dif:
                    return StiExportFormat.Dif;

                case StiDataType.Sylk:
                    return StiExportFormat.Sylk;

                case StiDataType.Xml:
                    return StiExportFormat.Xml;

                case StiDataType.Json:
                    return StiExportFormat.Json;

                default:
                    return StiExportFormat.Csv;
            }
        }
        #endregion

        #region Properties
        //DEVELOPER! Do not convert to auto property!
        private StiDataType dataType;
        /// <summary>
        /// Gets or sets type of the exported data file.
        /// </summary>
        [DefaultValue(StiDataType.Csv)]
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual StiDataType DataType
        {
            get
            {
                return dataType;
            }
            set
            {
                dataType = value;
            }
        }

        /// <summary>
        /// Gets or sets data export mode. SYLK and DIF formats does not support this property.
        /// </summary>
        [DefaultValue(StiDataExportMode.Data)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiDataExportMode DataExportMode { get; set; } = StiDataExportMode.Data;

        /// <summary>
        /// Gets or sets enconding of DIF file format. XML, JSON and DBF formats does not support this property.
        /// </summary>
        [JsonConverter(typeof(StiEncodingJsonConverter))]
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that all formatting of exported report will be removed. XML, JSON, DBF and CSV formats does not support this property.
        /// </summary>
        [DefaultValue(false)]
        public bool ExportDataOnly { get; set; }

        /// <summary>
        /// Gets or sets code page of DBF file format. DBF format only!
        /// </summary>
        [DefaultValue(StiDbfCodePages.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiDbfCodePages CodePage { get; set; } = StiDbfCodePages.Default;

        /// <summary>
        /// Gets or sets string which represents column separator in CSV file. CSV format only!
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Gets or sets name of the table. XML and JSON formats only!
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets value wich indicates that export engine will be write column headers as column headers in table or as simple column values. CSV format only!
        /// </summary>
        [DefaultValue(false)]
        public bool SkipColumnHeaders { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that default system encoding will be used for DIF and SYLK formats. DIF and SYLK format only!
        /// </summary>
        [DefaultValue(true)]
        public bool UseDefaultSystemEncoding { get; set; } = true;
        #endregion

        public StiDataExportSettings() : 
            this(StiDataType.Csv)
        {
        }

        public StiDataExportSettings(StiDataType dataType)
        {
            this.dataType = dataType;
            this.Encoding = Encoding.UTF8;
            this.Separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        }
	}
}
