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

using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base;


namespace Stimulsoft.Report.Chart
{
    public class StiXBottomAxis :
        StiXAxis,
        IStiXBottomAxis
    {
        public StiXBottomAxis()
        {
            this.Core = new StiXBottomAxisCoreXF(this);
        }

        public StiXBottomAxis(
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
            StiShowXAxis showXAxis,
            bool allowApplyStyle
            ) : this(labels, range, title, ticks, arrowStyle,
                    lineStyle, lineColor, lineWidth, visible,
                    startFromZero, showXAxis, false, allowApplyStyle)
        {
        }

        public StiXBottomAxis(
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
            StiShowXAxis showXAxis,
            bool showEdgeValues,
            bool allowApplyStyle)
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
            showXAxis,
            showEdgeValues,
            allowApplyStyle)
        {
            this.Core = new StiXBottomAxisCoreXF(this);
        }
        public StiXBottomAxis(
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
            StiShowXAxis showXAxis,
            bool showEdgeValues,
            bool allowApplyStyle)
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
            showXAxis,
            showEdgeValues,
            allowApplyStyle)
        {
            this.Core = new StiXBottomAxisCoreXF(this);
        }

        public StiXBottomAxis(
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
            StiShowXAxis showXAxis,
            bool showEdgeValues,
            bool allowApplyStyle,
            IStiAxisDateTimeStep dateTimeStep)
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
            showXAxis,
            showEdgeValues,
            allowApplyStyle,
            dateTimeStep)
        {
            this.DateTimeStep = dateTimeStep;
            this.Core = new StiXBottomAxisCoreXF(this);
        }

        [StiUniversalConstructor("XAxis")]
        public StiXBottomAxis(
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
            StiShowXAxis showXAxis,
            bool showEdgeValues,
            bool allowApplyStyle,
            IStiAxisDateTimeStep dateTimeStep,
            bool logarithmicScale)
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
            showXAxis,
            showEdgeValues,
            allowApplyStyle,
            dateTimeStep,
            logarithmicScale)
        {
            this.DateTimeStep = dateTimeStep;
            this.Core = new StiXBottomAxisCoreXF(this);
        }
    }
}
