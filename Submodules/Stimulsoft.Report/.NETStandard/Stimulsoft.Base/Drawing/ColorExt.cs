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
    /// <summary>
    /// This class contains some methods for an extended functionality of Color type.
    /// </summary>
    public static class ColorExt
    {
        /// <summary>
        /// Removes alpha channel from a color.
        /// </summary>
        /// <param name="color">A color in which Alpha channel should be removed.</param>
        /// <returns>A color with removed Alpha channel.</returns>
        public static Color RemoveAlpha(this Color color)
        {
            return Color.FromArgb(255, color.R, color.G, color.B);
        }

        /// <summary>
        /// Changes alpha channel from a color.
        /// </summary>
        /// <param name="color">A color in which Alpha channel should be changed.</param>
        /// <returns>A color with changed Alpha channel.</returns>
        public static Color ChangeAlpha(this Color color, int alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        /// <summary>
        /// Removes alpha channel from a color.
        /// </summary>
        /// <param name="color">A color in which Alpha channel should be removed.</param>
        /// <returns>A color with removed Alpha channel.</returns>
        public static string ToHex(this Color color)
        {
            var str = string.Empty;

            try
            {
                return color.A != 255 
                    ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}" 
                    : $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            catch
            {
            }

            return str;
        }
    }
}
