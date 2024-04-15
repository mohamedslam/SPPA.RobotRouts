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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiLegendStockMarker : IStiLegendMarker
    {
        public void Draw(StiContext context, IStiSeries serie, RectangleF rect, int colorIndex, int colorCount, int index)
        {
            IStiStockSeries series = serie as IStiStockSeries;

            Color color = series.LineColor;
            StiPenGeom pen = new StiPenGeom(color);

            context.PushSmoothingModeToAntiAlias();
            context.DrawLine(pen, rect.Left + rect.Width / 4, rect.Top + rect.Height / 4, rect.Left + rect.Width / 2, rect.Top + rect.Height / 4);
            context.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top, rect.Left + rect.Width / 2, rect.Bottom);
            context.DrawLine(pen, rect.Left + rect.Width / 2, rect.Bottom - rect.Height / 4, rect.Right - rect.Width / 4, rect.Bottom - rect.Height / 4);
            context.PopSmoothingMode();
        }
    }
}
