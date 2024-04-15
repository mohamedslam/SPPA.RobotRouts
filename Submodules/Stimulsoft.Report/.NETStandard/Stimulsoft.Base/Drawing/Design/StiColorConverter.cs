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
using System.ComponentModel;
using System.Globalization;
using Stimulsoft.Base.Localization;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Stimulsoft.Base.Drawing.Design
{
	/// <summary>
	/// Converts colors from one data type to another.
	/// </summary>
	public class StiColorConverter : ColorConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			    return true; 

			return base.CanConvertFrom(context, sourceType); 
		} 

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var colorName = value as string;
		    if (this is StiStyleColorConverter && colorName == Loc.Get("FormStyleDesigner", "FromStyle"))
		        return Color.Transparent;

			if (colorName != null & colorName.IndexOfInvariant(",") != -1)
			{
				colorName = colorName.Trim();

				var strs = colorName.Split(',');
				if (strs.Length == 4)
					return Color.FromArgb(
						int.Parse(strs[0].Trim()),
						int.Parse(strs[1].Trim()),
						int.Parse(strs[2].Trim()),
						int.Parse(strs[3].Trim()));

				return Color.FromArgb(
					int.Parse(strs[0].Trim()),
					int.Parse(strs[1].Trim()),
					int.Parse(strs[2].Trim()));
			}

			if ((colorName.Length == 6 || colorName.Length == 3) && !colorName.StartsWith("#"))
            {
                try
                {
					var regex = "([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
					var match = Regex.Match(colorName, regex, RegexOptions.IgnoreCase);

					if (match.Success)
					{
						return ColorTranslator.FromHtml($"#{colorName}");
					}
				}
                catch { }
            }

			return base.ConvertFrom(context, culture, value);
		}

		private Color TryGetDefaultColor(Color color)
        {
			if (color.IsKnownColor)
				return color;

			var namedColor = typeof(Color)
				.GetProperties(BindingFlags.Public | BindingFlags.Static)
				.Select(f => (Color)f.GetValue(null, null))
				.Where(c => (c.IsNamedColor || c.IsSystemColor) && c.ToArgb()
				.Equals(color.ToArgb()));

			if (namedColor.Count() > 0)
				return namedColor.First();

			return color;
        }

		public override bool CanConvertTo(ITypeDescriptorContext context, 
			Type destinationType)
		{
			if (destinationType == typeof(string))
			    return true;

			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			if (destinationType == typeof(string))
			{
				var expr = StiAppExpressionHelper.GetExpressionFromInstance(context);
				if (!string.IsNullOrWhiteSpace(expr))
					return expr;

				if (value == null)
					return "null";

				var color = TryGetDefaultColor((Color)value);

			    if (color == Color.Transparent && this is StiStyleColorConverter)
			        return Loc.Get("FormStyleDesigner", "FromStyle");

				string colorName;
                if (color.IsSystemColor)
				{
                    colorName = StiLocalization.Get("PropertySystemColors", color.Name, false);
                    if (colorName == null) return color.Name;
				}
				else if (color.IsKnownColor)
				{
					colorName = StiLocalization.Get("PropertyColor", color.Name);
				}
                else
                {
					if (color.A == 255)
						return $"{color.R},{color.G},{color.B}";

					else 
						return $"{color.A},{color.R},{color.G},{color.B}";
				}

				return colorName;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
