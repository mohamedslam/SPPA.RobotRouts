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

using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Painters;
using System.Drawing;

namespace Stimulsoft.Report.Gauge.Primitives
{
    public interface IStiScaleBarGeometry
    {
        #region Properties
        SizeF Size { get; }

        RectangleF RectGeometry { get; }

        PointF Center { get; }

        float Radius { get; }

        float Diameter { get; }
        #endregion

        #region Methods
        void CheckRectGeometry(RectangleF rect);
        void DrawScaleGeometry(StiGaugeContextPainter context);
        float GetRestToLenght();
        StiGraphicsPathLinesGaugeGeom DrawGeometry(StiGaugeContextPainter context, float StartValue, float EndValue, float StartWidth, float EndWidth, float Offset, StiPlacement Placement, ref RectangleF rect, bool returnOnlyRect);
        #endregion
    }
}