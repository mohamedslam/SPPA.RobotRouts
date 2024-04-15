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

namespace Stimulsoft.Base.Drawing
{
    public struct StiAlignHelper
	{
        /// <summary>
        /// Align a value to grid.
        /// </summary>
        /// <param name="gridSize">Grid size.</param>
        /// <param name="aligningToGrid">Align or no.</param>
        /// <returns>Aligned rectangle.</returns>
        public static double AlignToGrid(double value, double gridSize, bool aligningToGrid)
        {
            if (aligningToGrid)            
                return (int)Math.Round(value / gridSize) * gridSize;
            
            else
                return value;
        }

        /// <summary>
        /// Align a rectangle to grid.
        /// </summary>
        /// <param name="gridSize">Grid size.</param>
        /// <param name="aligningToGrid">Align or no.</param>
        /// <returns>Aligned rectangle.</returns>
        public static Rectangle AlignToGrid(Rectangle rect, int gridSize, bool aligningToGrid)
		{
		    if (aligningToGrid)
            {
                return new Rectangle(
                    (int)Math.Round(((float)rect.X / gridSize)) * gridSize,
                    (int)Math.Round(((float)rect.Y / gridSize)) * gridSize,
                    (int)Math.Round(((float)rect.Width / gridSize)) * gridSize,
                    (int)Math.Round(((float)rect.Height / gridSize)) * gridSize);
            }

		    return rect;
		}

        /// <summary>
        /// Align the rectangle to grid.
        /// </summary>
        /// <param name="gridSize">Grid size.</param>
        /// <param name="aligningToGrid">Align or no.</param>
        /// <returns>Aligned rectangle.</returns>
        public static RectangleD AlignToGrid(RectangleD rect, double gridSize, bool aligningToGrid)
        {
            if (aligningToGrid)
            {
                return new RectangleD(
                    (int)Math.Round(rect.X / gridSize) * gridSize,
                    (int)Math.Round(rect.Y / gridSize) * gridSize,
                    (int)Math.Round(rect.Width / gridSize) * gridSize,
                    (int)Math.Round(rect.Height / gridSize) * gridSize);
            }

            return rect;
        }
	}
}
