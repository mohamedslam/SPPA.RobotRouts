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

using Stimulsoft.Base.Drawing;
using System;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Base
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public sealed class StiDesignerIconAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Gets or sets a path to the bitmap in resources.
        /// </summary>
        public string BitmapName { get; set; }

        /// <summary>
        /// Gets or sets service type to which a bitmap is assign to.
        /// </summary>
        public Type Type { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a path to image according to its type.
        /// </summary>
        /// <param name="type">Service type.</param>
        /// <returns>Path to Image.</returns>
        public static string GetImagePath(Type type)
        {
            var attributes = (StiDesignerIconAttribute[])type.GetCustomAttributes(typeof(StiDesignerIconAttribute), true);

            return attributes.Length > 0 ? attributes[0].BitmapName : null;
        }

		/// <summary>
		/// Returns a service image according to its type.
		/// </summary>
		/// <param name="type">Service type.</param>
		/// <returns>Image.</returns>
		public static Bitmap GetImage(Type type)
		{
		    var attributes = (StiDesignerIconAttribute[])type.GetCustomAttributes(typeof(StiDesignerIconAttribute), true);

		    return attributes.Length > 0 ? attributes[0].GetImage() : null;
		}

		/// <summary>
		/// Returns an image of this type.
		/// </summary>
		/// <returns>Image.</returns>
		public Bitmap GetImage()
		{
			return StiImageUtils.GetImage(Type, BitmapName);
		}
        #endregion

        /// <summary>
        /// Creates a new attribute of the type StiDataAdapterIconAttribute.
        /// </summary>
        /// <param name="type">Service type to which a bitmap is compared to.</param>
        /// <param name="bitmapName">A path to the bitmap in resources.</param>
        public StiDesignerIconAttribute(Type type, string bitmapName)
		{
			this.Type = type;
			this.BitmapName = bitmapName;
		}
    }
}