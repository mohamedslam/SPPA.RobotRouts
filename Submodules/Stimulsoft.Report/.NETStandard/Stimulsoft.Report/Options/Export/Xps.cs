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
using Stimulsoft.Base.Serializing;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class for adjustment of the report Export.
        /// </summary>
        public sealed partial class Export
		{	
			/// <summary>
			/// Class for adjustment of the Pdf exporting.
			/// </summary>
            public sealed class Xps
			{
			    /// <summary>
			    /// Gets or sets a value indicating whether it is necessary to use image comparer.
			    /// </summary>
			    [DefaultValue(true)]
			    [Description("Gets or sets a value indicating whether it is necessary to use image comparer.")]
			    [StiSerializable]
			    public static bool AllowImageComparer { get; set; } = true;

				/// <summary>
				/// Gets or sets a value indicating whether it is necessary to use image transparency.
				/// </summary>
				[DefaultValue(true)]
			    [Description("Gets or sets a value indicating whether it is necessary to use image transparency.")]
			    [StiSerializable]
#if !BLAZOR
			    public static bool AllowImageTransparency { get; set; } = true;
#else
				public static bool AllowImageTransparency { get; set; } = false;
#endif

				/// <summary>
				/// Gets or sets a value indicating whether it is necessary to reduce font file size.
				/// </summary>
				[DefaultValue(true)]
			    [Description("Gets or sets a value indicating whether it is necessary to reduce font file size.")]
			    [StiSerializable]
			    public static bool ReduceFontFileSize { get; set; } = true;
			}
		}
    }
}