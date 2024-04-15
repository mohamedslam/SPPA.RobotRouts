#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System.Drawing;
using System.ComponentModel;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Base.Drawing
{

	/// <summary>
	/// Stores an ordered pair of decimal numbers, typically the width and height of a rectangle.
	/// </summary>
	[TypeConverter(typeof(SizeMConverter))]
	public struct SizeM : IStiDefault
    {
        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault => Width == 0 && Height == 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the horizontal component of this SizeM.
        /// </summary>
        [StiOrder(1)]
        [RefreshProperties(RefreshProperties.All)]
		public decimal Width { get; set; }

		/// <summary>
		/// Gets or sets the vertical component of this SizeM.
		/// </summary>
        [StiOrder(2)]
        [RefreshProperties(RefreshProperties.All)]
        public decimal Height { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SizeM has zero width and height.
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        public bool IsEmpty => Width == 0 && Height == 0;
        #endregion

        #region Fields.Static
        /// <summary>
        /// Initializes a new instance of the SizeM class.
        /// </summary>
        public static SizeM Empty = new SizeM(0, 0);
        #endregion

        #region Methods.override
        /// <summary>
        /// Tests to see whether the specified object is a SizeM with the same dimensions as this SizeM.
        /// </summary>
        /// <param name="obj">The Object to test.</param>
        /// <returns>This method returns true if obj is a SizeM and has the same width and height as this SizeM; otherwise, false.</returns>
        public override bool Equals(object obj)
		{
			var size = (SizeM)obj;
			return size.Width == Width && size.Height == Height;
		}

		/// <summary>
		/// Returns a hash code for this SizeM structure.
		/// </summary>
		/// <returns>An integer value that specifies a hash value for this SizeM structure.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode(); 
		}

        public override string ToString()
		{
			return $"{{Width={Width}, Height={Height}}}"; 
		}

		public SizeF ToSizeF()
		{
			return new SizeF((float)Width, (float)Height);
		}

        public SizeD ToSizeD()
        {
            return new SizeD((double)Width, (double)Height);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the SizeM class from the specified dimensions.
        /// </summary>
        /// <param name="width">The width component of the new SizeM.</param>
        /// <param name="height">The height component of the new SizeM.</param>
        public SizeM(decimal width, decimal height) : this()
        {
			this.Width = width;
			this.Height = height;
		}
	}
}
