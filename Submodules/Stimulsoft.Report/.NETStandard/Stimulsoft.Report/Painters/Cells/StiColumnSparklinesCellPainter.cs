#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using System;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Painters
{
    internal class StiColumnSparklinesCellPainter
    {
        internal static void Draw(StiContext context, RectangleD rect, object[] array, Color positiveColor, Color negativeColor)
        {
            if (array == null || array.Length == 0) return;
            rect.Inflate(-2, -2);

            var values = array.Select(StiValueHelper.TryToDouble).ToArray();
            var min = values.Min(v => v);
            var max = values.Max(v => v);
            var rMin = min;

            if (min > 0)
            {
                values = values.Select(v => v - min).ToArray();
                max -= min;
                min = 0;
            }

            if (max < 0)
            {
                values = values.Select(v => v + -max).ToArray();
                min += -max;
                max = 0;
            }

            var stepWidth = rect.Width / values.Length; // steps;
            var totalHeight = max + Math.Abs(min);
            var maxPart = totalHeight != 0 ? rect.Height * max / totalHeight : 0;

            var x = rect.X;
            var barWidth = stepWidth;
            if (barWidth < 1)
                barWidth = 1f;

            values.ToList().ForEach(value =>
            {
                var color = positiveColor;
                if (value < 0)
                    color = negativeColor;

                var valuePart = totalHeight != 0 ? rect.Height * Math.Abs(value) / totalHeight : 0;
                var barRect = new RectangleD(x, rect.Y + (value > 0 ? maxPart - valuePart : maxPart), barWidth, valuePart);
                if (value == 0)
                {
                    if (rMin > 0)
                        barRect.Y -= 1;

                    barRect.Height = 1;
                }

                if (barRect.Width > 5)
                    barRect.Inflate(-1, 0);

                x += stepWidth;

                context.FillRectangle(color, (float)barRect.X, (float)barRect.Y, (float)barRect.Width, (float)barRect.Height);
            });
        }
    }
}