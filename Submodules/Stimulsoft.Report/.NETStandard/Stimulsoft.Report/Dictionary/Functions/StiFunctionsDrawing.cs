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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Stimulsoft.Report.Dictionary
{
	public class StiFunctionsDrawing
	{
		/// <summary>
		/// Creates a Color from the four ARGB component (alpha, red, green, and blue) values.
		/// </summary>
		/// <param name="alpha">An alpha component, which ranges from 0 (fully transparent) to 255 (fully opaque).</param>
		/// <param name="red">A red component, which ranges from 0 (no saturation) to 255 (full saturation).</param>
		/// <param name="green">A green component, which ranges from 0 (no saturation) to 255 (full saturation).</param>
		/// <param name="blue">A blue component, which ranges from 0 (no saturation) to 255 (full saturation).</param>
		/// <returns>A color value.</returns>
		public static Color ARGB(int alpha, int red, int green, int blue)
		{
			return Color.FromArgb(alpha, red, green, blue);
		}

		/// <summary>
		/// Creates a Color from the three RGB component (red, green, and blue) values.
		/// </summary>
		/// <param name="red">A red component, which ranges from 0 (no saturation) to 255 (full saturation).</param>
		/// <param name="green">A green component, which ranges from 0 (no saturation) to 255 (full saturation).</param>
		/// <param name="blue">A blue component, which ranges from 0 (no saturation) to 255 (full saturation).</param>
		/// <returns>A color value.</returns>
		public static Color RGB(int red, int green, int blue)
		{
			return Color.FromArgb(255, red, green, blue);
		}

		/// <summary>
		/// Translates a string representation to a color. For example: ColorValue("Red"), ColorValue("#FF0000"), ColorValue("#55009900").
		/// </summary>
		/// <param name="value">A string representation of a color to translate.</param>
		/// <returns>A color value.</returns>
		public static Color ColorValue(string value)
		{
			return StiColor.Get(value);
		}

		/// <summary>
		/// Makes a color darker or lighter on a specified fade value. For example: ColorFade(Color.Red, -0.5), ColorFade(Color.Green, 0.3).
		/// </summary>
		/// <param name="color">A color value.</param>
		/// <param name="fadeAmount">A number between -1 and 1. -1 fully darkens a color to black, 0 doesn't affect the color, and 1 fully brightens a color to white.</param>
		/// <returns>A color value.</returns>
		public static Color ColorFade(object color, double fadeAmount)
		{
			if (fadeAmount == 0)
				return GetColor(color);

			else if (fadeAmount > 0)
				return StiColorUtils.Light(GetColor(color), (byte)(fadeAmount * 255));

			else
				return StiColorUtils.Dark(GetColor(color), (byte)(fadeAmount * 255));
		}

		/// <summary>
		/// Makes a solid brush. For example: SolidBrushValue("red"), SolidBrushValue(Color.Yellow).
		/// </summary>
		/// <param name="color">A color value for the solid brush.</param>
		/// <returns>A solid brush.</returns>
		public static StiSolidBrush SolidBrushValue(object color)
		{
			return new StiSolidBrush(GetColor(color));
		}

		/// <summary>
		/// Makes a gradient brush. For example: GradientBrushValue("red", "green", 45), GradientBrushValue(Color.Red, Color.Green, 45).
		/// </summary>
		/// <param name="startColor">A starting color for the gradient.</param>
		/// <param name="endColor">An ending color for the gradient.</param>
		/// <param name="angle">An angle, measured in degrees clockwise from the x-axis, of the gradient's orientation line.</param>
		/// <returns>A gradient brush.</returns>
		public static StiGradientBrush GradientBrushValue(object startColor, object endColor, double angle)
		{
			return new StiGradientBrush(GetColor(startColor), GetColor(endColor), angle);
		}

		/// <summary>
		/// Makes a glare brush. For example: GlareBrushValue("red", "green", 45), GlareBrushValue(Color.Red, Color.Green, 45).
		/// </summary>
		/// <param name="startColor">A string representation of a starting color for the gradient.</param>
		/// <param name="endColor">A string representation of a ending color for the gradient.</param>
		/// <param name="angle">An angle, measured in degrees clockwise from the x-axis, of the gradient's orientation line.</param>
		/// <param name="focus">A value from 0 through 1 that specifies the center of the gradient (the point where the gradient is composed of only the ending color).</param>
		/// <param name="scale">A value from 0 through 1 that specifies how fast the colors falloff from the focus.</param>
		/// <returns>A glare brush.</returns>
		public static StiGlareBrush GlareBrushValue(object startColor, object endColor, double angle, double focus, double scale)
		{
			return new StiGlareBrush(GetColor(startColor), GetColor(endColor), angle, (float)focus, (float)scale);
		}

		/// <summary>
		/// Makes a glass brush. For example: GlassBrushValue("#ff0000", 0.2), GlassBrushValue(Color.Red, 0.2).
		/// </summary>
		/// <param name="color">A color for the glass brush.</param>
		/// <param name="drawHatch">Draw hatch at background or not.</param>
		/// <param name="blendFactor">A blend factor. The value must be in range between 0 and 1.</param>
		/// <returns>A glass brush.</returns>
		public static StiGlassBrush GlassBrushValue(object color, bool drawHatch, double blendFactor)
		{
			return new StiGlassBrush(GetColor(color), drawHatch, (float)blendFactor);
		}

		/// <summary>
		/// Makes a hatch brush. For example: HatchBrushValue(HatchStyle.Cross, "gray", "white").
		/// </summary>
		/// <param name="style">A hatch style of the brush.</param>
		/// <param name="foreColor">A foreground color for the hatch brush.</param>
		/// <param name="backColor">A background color for the hatch brush.</param>
		/// <returns>A hatch brush.</returns>
		public static StiHatchBrush HatchBrushValue(HatchStyle style, object foreColor, object backColor)
		{
			return new StiHatchBrush(style, GetColor(foreColor), GetColor(backColor));
		}

		private static Color GetColor(object color)
        {
			if (color == null)
				return Color.Transparent;

			if (color is Color gdiColor)
				return gdiColor;

			if (color is StiBrush brush)
				return StiBrush.ToColor(brush);

			return StiColor.Get(color.ToString());
        }
	}
}
