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

using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    internal static class StiMouseOverHelper
    {
        #region Methods
        public static Color GetMouseOverColor()
        {
            return Color.FromArgb(100, Color.White);
        }

        public static Color GetLineMouseOverColor(Color color)
        {
            return Color.FromArgb(110, StiColorUtils.Light(color, 10));
        }

        public static Color GetLineMouseOverColor(StiBrush brush)
        {
            var color = StiBrush.ToColor(brush);
            return Color.FromArgb(110, StiColorUtils.Light(color, 10));
        }
        #endregion

        #region Fields
        public static float MouseOverLineDistance = 8f;
        public static float MouseOverSplineDistance = 15f;
        #endregion
    }
}
