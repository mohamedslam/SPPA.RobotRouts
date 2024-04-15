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

using Stimulsoft.Base;
using Stimulsoft.Report.Components;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Stimulsoft.Blockly
{
    public static class StiObjConverter
    {
        #region Methods
        public static double ToDouble(object obj)
        {
            return (double)StiConvert.ChangeType(GetValue(obj), typeof(double));
        }

        public static object GetValue(object obj)
        {
            if (obj is StiExpression expresion)
                return expresion.Value;

            return obj;
        }

        public static Color ToColor(string colorName)
        {
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

            if (colorName.StartsWith("#"))
            {
                return ColorTranslator.FromHtml(colorName);
            }

            return Color.Empty;
        }
        #endregion
    }
}
