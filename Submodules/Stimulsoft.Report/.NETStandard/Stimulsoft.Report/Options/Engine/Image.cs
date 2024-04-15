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
        /// A class which controls of settings of the report engine. 
        /// </summary>
		public sealed partial class Engine
        {
            public static class Image
            {
                /// <summary>
                /// Gets or sets absolute path which will be used in combination with path from File property of image component.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets absolute path which will be used in combination with path from File property of image component.")]
                [StiSerializable]
                public static string AbsolutePathOfImages { get; set; }

                /// <summary>
                /// Gets or sets a value forcing image cloning.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value forcing image cloning.")]
                [StiSerializable]
                [Category("Engine")]
                public static bool UseImageCloning { get; set; } = true;

                /// <summary>
                /// Gets or sets a value forcing rotate image by Exif orientation data.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value forcing rotate image by Exif orientation data.")]
                [StiSerializable]
                [Category("Engine")]
                public static bool RotateImageByExifOrientationData { get; set; } = true;

                /// <summary>
                /// Gets or sets a value forcing converting SVG-images to the bitmap for output.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value forcing converting SVG-images to the bitmap for output.")]
                [StiSerializable]
                [Category("Engine")]
                public static bool ConvertSvgToBitmap { get; set; } = true;

            }
        }
    }
}