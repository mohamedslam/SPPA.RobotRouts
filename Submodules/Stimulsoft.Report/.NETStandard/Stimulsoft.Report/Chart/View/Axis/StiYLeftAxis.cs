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
using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiYLeftAxis : 
        StiYAxis,
        IStiYLeftAxis
    {
        public StiYLeftAxis()
		{
            this.Core = new StiYLeftAxisCoreXF(this);
		}

        public StiYLeftAxis(
            IStiAxisLabels labels,
            IStiAxisRange range,
            IStiAxisTitle title,
            IStiAxisTicks ticks,
            StiArrowStyle arrowStyle,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool startFromZero,
            StiShowYAxis showYAxis,
            bool allowApplyStyle
            )
            :
            base(
            labels,
            range,
            title,
            ticks,
            arrowStyle,
            lineStyle,
            lineColor,
            lineWidth,
            visible,
            startFromZero,
            showYAxis,
            allowApplyStyle)
        {
            this.Core = new StiYLeftAxisCoreXF(this);
        }

        public StiYLeftAxis(
            IStiAxisLabels labels,
            IStiAxisRange range,
            IStiAxisTitle title,
            IStiAxisTicks ticks,
            IStiAxisInteraction interaction,
            StiArrowStyle arrowStyle,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool startFromZero,
            StiShowYAxis showYAxis,
            bool allowApplyStyle
            )
            :
            base(
            labels,
            range,
            title,
            ticks,
            interaction,
            arrowStyle,
            lineStyle,
            lineColor,
            lineWidth,
            visible,
            startFromZero,
            showYAxis,
            allowApplyStyle)
        {
            this.Core = new StiYLeftAxisCoreXF(this);
        }

        [StiUniversalConstructor("YAxis")]
        public StiYLeftAxis(
            IStiAxisLabels labels,
            IStiAxisRange range,
            IStiAxisTitle title,
            IStiAxisTicks ticks,
            IStiAxisInteraction interaction,
            StiArrowStyle arrowStyle,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool startFromZero,
            StiShowYAxis showYAxis,
            bool allowApplyStyle,
            bool logarithmicScale
            )
            :
            base(
            labels,
            range,
            title,
            ticks,
            interaction,
            arrowStyle,
            lineStyle,
            lineColor,
            lineWidth,
            visible,
            startFromZero,
            showYAxis,
            allowApplyStyle,
            logarithmicScale)
        {
            this.Core = new StiYLeftAxisCoreXF(this);
        }
	}
}
