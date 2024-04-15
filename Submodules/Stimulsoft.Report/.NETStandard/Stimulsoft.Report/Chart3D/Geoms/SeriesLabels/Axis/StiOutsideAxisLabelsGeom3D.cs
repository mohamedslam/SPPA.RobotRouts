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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiOutsideAxisLabelsGeom3D : StiCenterAxisLabelsGeom3D
    {
        #region Methods
        public override void DrawElements(StiContext context, StiMatrix vertices)
        {
            var size = GetLabelRect(context);

            var x = (float)vertices.Grid[0, 0];
            var y = (float)vertices.Grid[0, 1];

            var point = GetPoint(x, y);

            var height = Value > 0 ? -size.Height - StiScale.I4 : size.Height + StiScale.I4;

            var rect = new RectangleF(point.X - size.Width / 2, point.Y + height, size.Width, size.Height);

            DrawLabelArea(context, rect);
            DrawLabelText(context, rect);
        } 
        #endregion

        public StiOutsideAxisLabelsGeom3D(IStiSeriesLabels seriesLabels, IStiSeries series, int index, double value, string labelText, Color labelColor, Color labelBorderColor, StiBrush seriesBrush, StiBrush seriesLabelsBrush, Color seriesBorderColor, StiFontGeom font, StiPoint3D point3D, StiRender3D render3D) : base(seriesLabels, series, index, value, labelText, labelColor, labelBorderColor, seriesBrush, seriesLabelsBrush, seriesBorderColor, font, point3D, render3D)
        {
        }
    }
}
