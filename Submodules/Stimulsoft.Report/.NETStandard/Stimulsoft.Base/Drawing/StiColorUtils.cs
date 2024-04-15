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

namespace Stimulsoft.Base.Drawing
{
    public static class StiColorUtils
    {
        #region Methods
        /// <summary>
        /// Retrieves the current color of the specified display element.
        /// </summary>
        /// <param name="colorType">Specifies the display element whose color is to be retrieved.</param>
        /// <returns>Color value of the given element.</returns>
        public static Color GetSysColor(Win32.ColorType colorType)
		{
			return ColorTranslator.FromWin32(Win32.GetSysColor((int)colorType));
		}

        public static Color ChangeLightness(Color color, byte correctionFactor)
        {
            float value = correctionFactor / 255f;
            return ChangeLightness(color, value);
        }

        public static Color ChangeLightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

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

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }

        public static Color ChangeDarkness(Color color, byte correctionFactor)
        {
            float value = correctionFactor / 255f;
            return ChangeDarkness(color, value);
        }

        public static Color ChangeDarkness(Color color, float percDarker)
        {
            return ChangeLightness(color, percDarker * -1);
        }

        public static Color Light(string baseColor, byte value)
        {
            return Light(StiColor.Get(baseColor), value);
        }

        public static Color Light(Color baseColor, byte value)
        {
            var R = baseColor.R;
            var G = baseColor.G;
            var B = baseColor.B;

            if (R + value > 255) R = 255;
            else R += value;

            if (G + value > 255) G = 255;
            else G += value;

            if (B + value > 255) B = 255;
            else B += value;

            return Color.FromArgb(R, G, B);
        }

        public static bool IsItTooLight(Color color)
        {
            return color.R > 200 && color.G > 200 && color.B > 200;
        }

        public static bool IsItTooDark(Color color)
        {
            return color.R < 50 && color.G < 50 && color.B < 50;
        }

        public static Color MixingColors(Color color1, Color color2, int alpha = 255)
        {
            var r = color2.R * alpha / 255 + color1.R * (255 - alpha) / 255;
            var g = color2.G * alpha / 255 + color1.G * (255 - alpha) / 255;
            var b = color2.B * alpha / 255 + color1.B * (255 - alpha) / 255;

            return Color.FromArgb(255, r, g, b);
        }

        public static Color Dark(string baseColor, byte value)
        {
            return Dark(StiColor.Get(baseColor), value);
        }

        public static Color Dark(Color baseColor, byte value)
        {
            var R = baseColor.R;
            var G = baseColor.G;
            var B = baseColor.B;

            if (R - value < 0) R = 0;
            else R -= value;

            if (G - value < 0) G = 0;
            else G -= value;

            if (B - value < 0) B = 0;
            else B -= value;

            return Color.FromArgb(R, G, B);
        }
        		
		public static int GetColorRop(Color color, int darkROP, int lightROP)
		{
		    return color.GetBrightness() < 0.5f ? darkROP : lightROP;
		}
        #endregion
    }
}
