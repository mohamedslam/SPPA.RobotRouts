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

using System;
using System.Drawing;

namespace Stimulsoft.Base
{
	/// <summary>
	/// Helps to aligning values.
	/// </summary>
	public sealed class StiAlignValue
	{
		/// <summary>
		/// Aligning value on grid to greater.
		/// </summary>
		/// <param name="value">Value to align.</param>
		/// <param name="gridSize">Grid size.</param>
		/// <param name="aligningToGrid">Align or no.</param>
		/// <returns>Aligned value.</returns>
		public static double AlignToMaxGrid(double value, double gridSize, bool aligningToGrid)
		{
		    if (!aligningToGrid) return value;

		    var alignedValue = Math.Round((value / gridSize)) * gridSize;
		    if (value > alignedValue)alignedValue += gridSize;

		    return alignedValue;
		}

        /// <summary>
        /// Aligning value on grid to greater.
        /// </summary>
        /// <param name="value">Value to align.</param>
        /// <param name="gridSize">Grid size.</param>
        /// <param name="aligningToGrid">Align or no.</param>
        /// <returns>Aligned value.</returns>
        public static decimal AlignToMaxGrid(decimal value, decimal gridSize, bool aligningToGrid)
        {
            if (!aligningToGrid) return value;

            var alignedValue = Math.Round(value / gridSize) * gridSize;
            if (value > alignedValue) alignedValue += gridSize;

            return alignedValue;
        }

	
		/// <summary>
		/// Aligning value on grid to less.
		/// </summary>
		/// <param name="value">Value to align.</param>
		/// <param name="gridSize">Grid size.</param>
		/// <param name="aligningToGrid">Align or no.</param>
		/// <returns>Aligned value.</returns>
		public static double AlignToMinGrid(double value, double gridSize, bool aligningToGrid)
		{
		    if (!aligningToGrid) return value;

		    var alignedValue = Math.Round(value / gridSize) * gridSize;
		    if (value < alignedValue)alignedValue -= gridSize;

		    return alignedValue;
		}

		
		/// <summary>
		/// Aligning value on grid.
		/// </summary>
		/// <param name="value">Value to align.</param>
		/// <param name="gridSize">Grid size.</param>
		/// <param name="aligningToGrid">Align or no.</param>
		/// <returns>Aligned value.</returns>
		public static double AlignToGrid(double value, double gridSize, bool aligningToGrid)
		{
		    return aligningToGrid ? Math.Round((value / gridSize)) * gridSize : value;
		}		
	}
}
