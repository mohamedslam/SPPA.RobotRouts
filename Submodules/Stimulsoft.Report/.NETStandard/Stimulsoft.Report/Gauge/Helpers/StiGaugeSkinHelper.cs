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

using Stimulsoft.Report.Gauge.Skins;

namespace Stimulsoft.Report.Gauge.Helpers
{
    internal static class StiGaugeSkinHelper
    {
        public static StiGaugeElementSkin GetMarkerSkin(StiMarkerSkin skin)
        {
            switch (skin)
            {
                case StiMarkerSkin.Diamond:
                    return new StiMarker1Skin();
                case StiMarkerSkin.Rectangle:
                    return new StiMarker2Skin();
                case StiMarkerSkin.TriangleTop:
                    return new StiMarker3Skin();
                case StiMarkerSkin.TriangleBottom:
                    return new StiMarker4Skin();
                case StiMarkerSkin.PentagonTop:
                    return new StiMarker5Skin();
                case StiMarkerSkin.PentagonBottom:
                    return new StiMarker6Skin();
                case StiMarkerSkin.Ellipse:
                    return new StiMarker7Skin();
                case StiMarkerSkin.RectangularCalloutTop:
                    return new StiMarker8Skin();
                case StiMarkerSkin.RectangularCalloutBottom:
                    return new StiMarker9Skin();
                case StiMarkerSkin.TriangleLeft:
                    return new StiMarker10Skin();
                case StiMarkerSkin.TriangleRight:
                    return new StiMarker11Skin();
                case StiMarkerSkin.PentagonLeft:
                    return new StiMarker12Skin();
                case StiMarkerSkin.PentagonRight:
                    return new StiMarker13Skin();
                case StiMarkerSkin.RectangularCalloutLeft:
                    return new StiMarker14Skin();
                default:
                    return null;
            }
        }

        public static StiGaugeElementSkin GetTickMarkSkin(StiTickMarkSkin skin)
        {
            switch (skin)
            {
                case StiTickMarkSkin.Rectangle:
                    return new StiMark1Skin();
                case StiTickMarkSkin.Ellipse:
                    return new StiMark2Skin();
                case StiTickMarkSkin.Diamond:
                    return new StiMark3Skin();
                case StiTickMarkSkin.TriangleTop:
                    return new StiMark4Skin();
                case StiTickMarkSkin.TriangleRight:
                    return new StiMark5Skin();
                case StiTickMarkSkin.TriangleLeft:
                    return new StiMark6Skin();
                case StiTickMarkSkin.TriangleBottom:
                    return new StiMark7Skin();
                default:
                    return null;
            }
        }

        public static StiGaugeElementSkin GetStateIndicatorSkin(StiStateSkin skin)
        {
            switch (skin)
            {
                case StiStateSkin.Ellipse:
                    return new StiState1Skin();
                case StiStateSkin.Rectangle:
                    return new StiState2Skin();
                case StiStateSkin.Diamond:
                    return new StiState3Skin();
                default:
                    return null;
            }
        }

        public static StiGaugeElementSkin GetNeedleIndicatorSkin(StiNeedleSkin skin)
        {
            switch (skin)
            {
                case StiNeedleSkin.DefaultNeedle:
                    return new StiNeedleIndicator1Skin();
                case StiNeedleSkin.SpeedometerNeedle:
                    return new StiNeedleIndicator2Skin();
                case StiNeedleSkin.SpeedometerNeedle2:
                    return new StiNeedleIndicator3Skin();
                case StiNeedleSkin.SimpleNeedle:
                    return new StiNeedleIndicator4Skin();
                default:
                    return null;
            }
        }
    }
}