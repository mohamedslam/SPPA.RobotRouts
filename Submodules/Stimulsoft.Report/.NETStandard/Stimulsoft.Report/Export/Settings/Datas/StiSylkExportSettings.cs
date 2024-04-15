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
using System.Text;

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// Class describes settings for export to SYLK format.
    /// </summary>
    public class StiSylkExportSettings : StiDataExportSettings
	{
        #region StiDataExportSettings override
        /// <summary>
        /// Gets or sets image type for exported images.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public override StiDataType DataType 
        {
            get
            {
                return StiDataType.Sylk;
            }
            set
            {
                //throw new ArgumentException("An DataType property of the StiSylkExportSettings class can't be changed!");
            }
        }
        #endregion

        public StiSylkExportSettings()
            : base(StiDataType.Sylk)
        {
            Encoding = Encoding.ASCII;
        }
	}
}
