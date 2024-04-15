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
    internal class StiWinLossSparklinesCellPainter
    {
        internal static void Draw(StiContext context, RectangleD rect, object[] array, Color positiveColor, Color negativeColor)
        {
            if (array == null || array.Length == 0) return;
            rect.Inflate(-2, -2);

            var values = array.Select(StiValueHelper.TryToDouble).ToArray();
            values = values.Select(v =>
            {
                if (v == 0d) return 0d;
                return v > 0 ? 1d : -1d;
            }).ToArray();


            var stepWidth = rect.Width / values.Length;//steps
            var maxPart = rect.Height / 2;

            #region Calculate Rects
            var x = rect.X;
            var rects = values.Select(value =>
            {
                var valuePart = rect.Height * Math.Abs(value) / 2;
                var barRect = new RectangleD(x, rect.Y + (value > 0 ? maxPart - valuePart : maxPart), stepWidth, valuePart);
                
                if (barRect.Width > 4) 
                    barRect.Inflate(-1, 0);

                x += stepWidth;

                return barRect;
            }).ToList();
            #endregion

            var valueIndex = 0;
            rects.ForEach(r =>
            {
                var color = values[valueIndex] > 0 ? positiveColor : negativeColor;

                context.FillRectangle(color, (float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
                valueIndex++;
            });
        }
    }
}
