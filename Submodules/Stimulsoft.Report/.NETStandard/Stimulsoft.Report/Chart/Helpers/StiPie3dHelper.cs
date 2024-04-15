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

namespace Stimulsoft.Report.Chart
{
    internal static class StiPie3dHelper
    {
        #region Const
        public static readonly float BrightnessEnhancementFactor1 = 0.3F;
        #endregion

        #region Methods
        public static Color CreateColorWithCorrectedLightness(Color color, float correctionFactor)
        {
            if (correctionFactor == 0)
                return color;

            var red = (float)color.R;
            var green = (float)color.G;
            var blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }

            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        /// <summary>
        ///   Gets the actual angle from the rendering angle.
        /// </summary>
        /// <param name="transformedAngle">
        ///   Transformed angle for which actual angle has to be evaluated.
        /// </param>
        /// <returns>
        ///   Actual angle.
        /// </returns>
        public static float GetActualAngle(RectangleF rect, float transformedAngle)
        {
            double x = rect.Height * Math.Cos(transformedAngle * Math.PI / 180);
            double y = rect.Width * Math.Sin(transformedAngle * Math.PI / 180);

            float result = (float)(Math.Atan2(y, x) * 180 / Math.PI);

            if (result < 0)
                return result + 360;

            return result;
        }

        public static float TransformAngle(RectangleF rect, float angle)
        {
            double x = rect.Width * Math.Cos(angle * Math.PI / 180);
            double y = rect.Height * Math.Sin(angle * Math.PI / 180);
            float result = (float)(Math.Atan2(y, x) * 180 / Math.PI);
            if (result < 0)
                return result + 360;
            return result;
        }
        #endregion
    }
}
