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
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Base.Drawing
{
	/// <summary>
	/// Stores an ordered pair of floating-point numbers, typically the width and height of a rectangle.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.SizeDConverter))]
	public struct SizeD : IStiDefault
    {
        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault => Width == 0 && Height == 0;
        #endregion

        #region Properties
        /// <summary>
		/// Gets or sets the horizontal component of this SizeD.
		/// </summary>
		[StiOrder(1)]
        [RefreshProperties(RefreshProperties.All)]
		public double Width { get; set; }

        /// <summary>
		/// Gets or sets the vertical component of this SizeD.
		/// </summary>
        [StiOrder(2)]
        [RefreshProperties(RefreshProperties.All)]
		public double Height { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SizeD has zero width and height.
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        public bool IsEmpty => Width == 0 && Height == 0;
        #endregion

        #region Fields.Static
        /// <summary>
        /// Initializes a new instance of the SizeD class.
        /// </summary>
        public static SizeD Empty = new SizeD(0, 0);
        #endregion

        #region Methods.override
        /// <summary>
        /// Tests to see whether the specified object is a SizeD with the same dimensions as this SizeD.
        /// </summary>
        /// <param name="obj">The Object to test.</param>
        /// <returns>This method returns true if obj is a SizeD and has the same width and height as this SizeD; otherwise, false.</returns>
        public override bool Equals(object obj)
		{
			var size = (SizeD)obj;
			return size.Width == Width && size.Height == Height;
		}

		/// <summary>
		/// Returns a hash code for this SizeD structure.
		/// </summary>
		/// <returns>An integer value that specifies a hash value for this SizeD structure.</returns>
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
        #endregion

        #region Methods
        /// <summary>
        /// Converts the specified SizeD to a Size.
        /// </summary>
        public Size ToSize()
        {
            return new Size((int)Width, (int)Height);
        }

        /// <summary>
        /// Multiplies the size on number.
        /// </summary>
        /// <param name="multipleFactor">Number.</param>
        /// <returns>Multiplied size.</returns>
        public SizeD Multiply(double multipleFactor)
        {
            return new SizeD(
                Width * multipleFactor,
                Height * multipleFactor);
        }

        /// <summary>
        /// Creates a new SizeD object based on the specified SizeF object.
        /// </summary>
        public static SizeD CreateFromSize(SizeF size)
        {
            return new SizeD(size.Width, size.Height);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the SizeD class from the specified dimensions.
        /// </summary>
        /// <param name="width">The width component of the new SizeD.</param>
        /// <param name="height">The height component of the new SizeD.</param>
        public SizeD(double width, double height)
		{
			this.Width = width;
			this.Height = height;
		}
	}
}
