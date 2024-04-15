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

using Stimulsoft.Base.Blocks;
using System;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Events
{
    /// <summary>
    /// Represents the method that handles the events which work with image.
    /// </summary>
    public delegate void StiGetImageDataEventHandler(object sender, StiGetImageDataEventArgs e);

    /// <summary>
    /// Describes an argument for the event GetImageData.
    /// </summary>
    public class StiGetImageDataEventArgs :
        EventArgs,
        IStiBlocklyValueEventArgs
    {
        /// <summary>
        /// Gets or sets the image for the event.
        /// </summary>
        public virtual object Value { get; set; }

        /// <summary>
        /// Gets or sets the image bytes for the event.
        /// </summary>
        [Obsolete("'ValueBytes' property is obsolete. Please use 'Value' property instead it.")]
        public virtual byte[] ValueBytes { get; set; }

        /// <summary>
        /// Creates a new object of the type StiGetImageDataEventArgs.
        /// </summary>
        public StiGetImageDataEventArgs() : this(null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiGetImageDataEventArgs.
        /// </summary>
        /// <param name="image">Image for event.</param>
        public StiGetImageDataEventArgs(object image)
        {
            this.Value = image;
        }
    }
}