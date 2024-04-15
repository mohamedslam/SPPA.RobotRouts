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

namespace Stimulsoft.Report.Chart
{
    public class StiTreemapEmptySeriesElementGeom : StiCellGeom
    {
        public override void Draw(StiContext context)
        {
            var rect = Rectangle.Round(this.ClientRectangle);
            var pen = new StiPenGeom(Color.LightGray);

            context.PushSmoothingModeToAntiAlias();

            context.FillRectangle(Color.FromArgb(50, Color.LightGray), rect, null);

            context.DrawRectangle(pen, rect.X, rect.Y, rect.Width / 3, rect.Height);
            context.DrawRectangle(pen, rect.X + rect.Width / 3, rect.Y, rect.Width * 2 / 3, rect.Height);

            context.PopSmoothingMode();
        }

        public StiTreemapEmptySeriesElementGeom(RectangleF clientRectangle) : base(clientRectangle)
        {
        }
    }
}
