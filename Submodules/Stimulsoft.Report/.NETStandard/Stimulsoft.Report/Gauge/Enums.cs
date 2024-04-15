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

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using System.ComponentModel;

namespace Stimulsoft.Report.Gauge
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiGaugeRangeMode
    {
        Percentage = 1,
        Value
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiScaleMode
    {
        V1 = 1,
        V2
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiGaugeRangeType
    {
        None = 0,
        Color
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiGaugeCalculationMode
    {
        Auto = 1,
        Custom
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiGaugeType
    {
        FullCircular,
        HalfCircular,
        Linear,
        HorizontalLinear,
        Bullet
    }

    public enum StiPlacement
    {
        Outside,
        Overlay,
        Inside
    }

    public enum StiGaugeElemenType
    {
        LinearElement,
        RadialElement,
        All
    }

    public enum StiBarRangeListType
    {
        LinearBar,
        RadialBar
    }

    public enum StiLinearRangeColorMode
    {
        Default,
        MixedColor,
    }
    
    public enum StiRadialScaleSkin
    {
        Default,
        Empty,
        RadialScaleQuarterCircleNW,
        RadialScaleQuarterCircleNE,
        RadialScaleQuarterCircleSW,
        RadialScaleQuarterCircleSE,
        RadialScaleHalfCircleN,
        RadialScaleHalfCircleS
    }

    public enum StiMarkerSkin
    {
        Diamond,
        Rectangle,
        TriangleTop,
        TriangleBottom,
        PentagonTop,
        PentagonBottom,
        Ellipse,
        RectangularCalloutTop,
        RectangularCalloutBottom,
        TriangleLeft,
        TriangleRight,
        PentagonLeft,
        PentagonRight,
        RectangularCalloutLeft
    }

    public enum StiStateSkin
    {
        Ellipse,
        Rectangle,
        Diamond
    }

    public enum StiLinearBarSkin
    {
        Default,
        HorizontalThermometer,
        VerticalThermometer
    }


    public enum StiNeedleSkin
    {
        DefaultNeedle,
        SpeedometerNeedle,
        SpeedometerNeedle2,
        SimpleNeedle
    }

    public enum StiTickMarkSkin
    {
        Rectangle,
        Ellipse,
        Diamond,
        TriangleTop,
        TriangleRight,
        TriangleLeft,
        TriangleBottom
    }

    public enum StiRadiusMode
    {
        Auto,
        Width,
        Height
    }

    internal enum StiRadialPosition
    {
        TopLeft,
        TopRight,
        BottonLeft,
        BottomRight,
        TopCenter,
        LeftCenter,
        BottomCenter,
        RightCenter
    }

    [Description("Label rotation mode.")]
    public enum StiLabelRotationMode
    {
        // Summary:
        //     No label rotation.
        None = 0,
        //
        // Summary:
        //     Labels are rotated.
        Automatic = 1,
        //
        // Summary:
        //     Labels surround radial scale with character’s base line is directed towards
        //     the center of the scale.
        SurroundIn = 2,
        //
        // Summary:
        //     Labels surround radial scale with character’s base line is directed backwards
        //     the center of the scale.
        SurroundOut = 3,
    }

    #region StiGaugeStyleId
    public enum StiGaugeStyleId
    {
        StiStyle25,
        StiStyle26,
        StiStyle27,
        StiStyle28,
        StiStyle29,
        StiStyle30,
    }
    #endregion
}
