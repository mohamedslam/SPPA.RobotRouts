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

namespace Stimulsoft.Report.Chart
{
    public class StiMarkerLegendFactory
    {
        public static IStiLegendMarker CreateMarker(IStiSeries series)
        {
            if (series is IStiFontIconsSeries && ((IStiFontIconsSeries)series).Icon != null)
                return new StiLegendFontIconMarker();

            else if (series is IStiDoughnutSeries)
                return new StiLegendDoughnutMarker();

            else if (series is IStiPieSeries)
                return new StiLegendPieMarker();

            else if (series is StiAreaSeries)
                return new StiLegendAreaMarker();

            else if (series is StiStackedAreaSeries)
                return new StiLegendStackedAreaMarker();

            else if (series is StiSplineAreaSeries)
                return new StiLegendSplineAreaMarker();

            else if (series is StiStackedSplineAreaSeries)
                return new StiLegendStackedSplineAreaMarker();

            else if (series is StiSteppedAreaSeries)
                return new StiLegendSteppedAreaMarker();

            else if (series is StiRangeSeries)
                return new StiLegendRangeMarker();

            else if (series is StiSplineRangeSeries)
                return new StiLegendSplineRangeMarker();

            else if (series is StiSteppedRangeSeries)
                return new StiLegendSteppedRangeMarker();

            else if (series is IStiBaseLineSeries || series is IStiStackedBaseLineSeries || series is IStiRadarSeries)
                return new StiLegendLineMarker();

            else if (series is IStiFunnelSeries)
                return new StiLegendFunnelMarker();

            else if (series is IStiStockSeries)
                return new StiLegendStockMarker();

            else if (series is IStiCandlestickSeries)
                return new StiLegendCandelstickMarker();

            else if (series is IStiPictorialSeries)
                return new StiLegendPictorialMarker();

            //Series 3D
            else if (series is IStiClusteredColumnSeries3D)
                return new StiLegendColumnMarker3D();

            else if (series is IStiLineSeries3D)
                return new StiLegendLineMarker3D();

            return new StiLegendColumnMarker();
        }
    }
}
