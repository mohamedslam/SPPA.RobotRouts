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
using System.Linq;

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// This class helps in converting a string to a color.
    /// </summary>
    public static class StiColor
    {
        /// <summary>
        /// Translates a HTML color representation to a GDI+ System.Drawing.Color structure.
        /// </summary>
        public static Color Get(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return Color.Transparent;

            color = color.Trim();

            if (StiColors.NameColors.Contains(CapitalFirstLetter(color)))
                return Color.FromName(color);

            if (color.IndexOfInvariant(",") != -1)
            {
                try
                {
                    var strs = color.Split(',');
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
                catch
                {
                    return Color.Transparent;
                }
            }

            if (!color.StartsWith("#"))
                color = $"#{color}";

            try
            {
                return ColorTranslator.FromHtml(color);
            }
            catch { }

            return Color.Transparent;
        }

        /// <summary>
        /// Translates an array with HTML colors representation to a GDI+ System.Drawing.Color structure.
        /// </summary>
        public static Color[] Get(params string[] colors)
        {
            return colors.ToList().Select(Get).ToArray();
        }

        private static string CapitalFirstLetter(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return $"{str.Substring(0, 1).ToUpper()}{str.Substring(1).ToLower()}";
        }
    }
}
