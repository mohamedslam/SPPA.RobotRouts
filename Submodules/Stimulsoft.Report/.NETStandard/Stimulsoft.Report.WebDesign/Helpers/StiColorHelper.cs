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

using Stimulsoft.Report.Dashboard;
using System.Collections;
using System.Drawing;

namespace Stimulsoft.Report.Web
{
    internal class StiColorHelper
    {
#if !NETSTANDARD
        public static System.Windows.Media.Color ColorToWpfColor(Color color)
        {
            System.Windows.Media.Color wpfColor = new System.Windows.Media.Color();
            wpfColor.A = color.A;
            wpfColor.R = color.R;
            wpfColor.G = color.G;
            wpfColor.B = color.B;

            return wpfColor;
        }

        public static Color WpfColorToColor(System.Windows.Media.Color wpfColor)
        {
            return Color.FromArgb(wpfColor.A, wpfColor.R, wpfColor.G, wpfColor.B);
        }

        public static System.Windows.Media.Color Light(Color baseColor, byte value)
        {
            return Light(ColorToWpfColor(baseColor), value);
        }

        public static System.Windows.Media.Color Light(System.Windows.Media.Color baseColor, byte value)
        {
            byte R = baseColor.R;
            byte G = baseColor.G;
            byte B = baseColor.B;

            if ((R + value) > 255) R = 255;
            else R += value;

            if ((G + value) > 255) G = 255;
            else G += value;

            if ((B + value) > 255) B = 255;
            else B += value;

            return System.Windows.Media.Color.FromArgb(baseColor.A, R, G, B);
        }

        public static System.Windows.Media.Color Dark(System.Windows.Media.Color baseColor, byte value)
        {
            byte R = baseColor.R;
            byte G = baseColor.G;
            byte B = baseColor.B;

            if ((R - value) < 0) R = 0;
            else R -= value;

            if ((G - value) < 0) G = 0;
            else G -= value;

            if ((B - value) < 0) B = 0;
            else B -= value;

            return System.Windows.Media.Color.FromArgb(baseColor.A, R, G, B);
        }
#endif

        public static Hashtable GetPredefinedColors()
        {
            var predefinedColors = new Hashtable();

            #region Sets
            predefinedColors["sets"] = new ArrayList();
            foreach (var colors in StiPredefinedColors.Sets)
            {
                var jsColors = new ArrayList();
                foreach (var color in colors)
                    jsColors.Add(StiReportEdit.GetStringFromColor(color));

                ((ArrayList)predefinedColors["sets"]).Add(jsColors);
            }
            #endregion;

            #region NegativeSets
            predefinedColors["negativeSets"] = new ArrayList();
            foreach (var colors in StiPredefinedColors.NegativeSets)
            {
                var jsColors = new ArrayList();
                foreach (var color in colors)
                    jsColors.Add(StiReportEdit.GetStringFromColor(color));

                ((ArrayList)predefinedColors["negativeSets"]).Add(jsColors);
            }
            #endregion;

            return predefinedColors;
        }
    }
}
