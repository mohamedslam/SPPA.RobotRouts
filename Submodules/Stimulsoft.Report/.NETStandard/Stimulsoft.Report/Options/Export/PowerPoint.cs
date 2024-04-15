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

using System;
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
            /// This class is used for adjustment of the PowerPoint export of a report.
            /// </summary>
            public class PowerPoint
            {
                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use image comparer.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use image comparer.")]
                [StiSerializable]
                public static bool AllowImageComparer { get; set; } = true;


                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to store images in PNG format instead of JPG.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to store images in PNG format instead of JPG.")]
                [StiSerializable]
                public static bool StoreImagesAsPng { get; set; } = true;


                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to replace transparent page background with white color.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to replace transparent page background with white color.")]
                [StiSerializable]
                public static bool ReplaceTransparentPageBackground { get; set; } = true;
            }

            #region Obsoleted
            /// <summary>
            /// Class for adjustment of the PowerPoint export of a report.
            /// </summary>
            [Obsolete("The class StiOptions.Ppt2007 is obsolete! Please use class StiOptions.PowerPoint instead it.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public sealed class Ppt2007 : PowerPoint
            {
                
            }
            #endregion
        }
    }
}