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

using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Gauge.Helpers
{
    public static class StiMixedColorHelper
    {
        public static Color ColorMixed(List<Color> colors)
        {
            if (colors == null || colors.Count == 0)
            {
                return Color.Transparent;
            }
            else if (colors.Count == 1)
            {
                return colors[0];
            }
            else
            {
                Color finishColor = colors[0];
                int index = 0;
                while(++index < colors.Count)
                {
                    finishColor = ColorMixer(finishColor, colors[index]);
                }

                return finishColor;
            }
        }

        private static Color ColorMixer(Color c1, Color c2)
        {
            byte r = (byte)((c1.R + c2.R) / 2);
            byte g = (byte)((c1.G + c2.G) / 2);
            byte b = (byte)((c1.B + c2.B) / 2);
            return Color.FromArgb(r, g, b);
        }
    }
}