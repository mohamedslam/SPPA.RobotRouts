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

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// This class describes settings for export to Excel2007 format.
    /// </summary>
    public class StiExcel2007ExportSettings : 
        StiExcelExportSettings
	{
        #region StiExcelExportSettings override
        /// <summary>
        /// Gets or sets image type for exported images.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public override StiExcelType ExcelType
        {
            get
            {
                return StiExcelType.Excel2007;
            }
            set
            {
                //throw new ArgumentException("An ExcelType property of the StiExcel2007ExportSettings class can't be changed!");
            }
        }
        #endregion

        public StiExcel2007ExportSettings()
            : base(StiExcelType.Excel2007)
        {
        }
	}
}
