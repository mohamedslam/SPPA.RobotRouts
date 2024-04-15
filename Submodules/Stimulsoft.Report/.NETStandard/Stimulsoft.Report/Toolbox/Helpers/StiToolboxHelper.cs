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

using Stimulsoft.Base.Localization;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.ShapeTypes;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Toolbox;
using System.Collections.Generic;

namespace Stimulsoft.Report.Maps
{
    public static class StiToolboxHelper
    {
        #region Methods
        public static List<StiMapToolboxInfo> GetMapToolboxItems()
        {
            var mapItemsCache = new List<StiMapToolboxInfo>()
            {
                new StiMapToolboxInfo(StiMapCategory.PopularMaps, "Popular maps", "Популярные карты")
                {
                    Infos = new List<StiMapToolboxInfo>()
                    {
                        new StiMapToolboxInfo(StiMapID.World),
                        new StiMapToolboxInfo(StiMapID.USA),
                        new StiMapToolboxInfo(StiMapID.Europe),
                        new StiMapToolboxInfo(StiMapID.Asia),
                        new StiMapToolboxInfo(StiMapID.China),
                    }
                },
                new StiMapToolboxInfo(StiMapCategory.Europe, "Europe", "Европа")
                {
                    Infos = new List<StiMapToolboxInfo>()
                    {
                        new StiMapToolboxInfo(StiMapID.UK),
                        new StiMapToolboxInfo(StiMapID.UKCountries),
                        new StiMapToolboxInfo(StiMapID.Europe),
                        new StiMapToolboxInfo(StiMapID.EuropeWithRussia),
                        new StiMapToolboxInfo(StiMapID.Austria),
                        new StiMapToolboxInfo(StiMapID.Belgium),
                        new StiMapToolboxInfo(StiMapID.Benelux),
                        new StiMapToolboxInfo(StiMapID.France, new string[] { "EN", "FR" }),
                        new StiMapToolboxInfo(StiMapID.FranceDepartments),
                        new StiMapToolboxInfo(StiMapID.France18Regions),
                        new StiMapToolboxInfo(StiMapID.Germany, new string[] { "EN", "DE" }),
                        new StiMapToolboxInfo(StiMapID.Ireland),
                        new StiMapToolboxInfo(StiMapID.Liechtenstein),
                        new StiMapToolboxInfo(StiMapID.Luxembourg),
                        new StiMapToolboxInfo(StiMapID.Monaco),
                        new StiMapToolboxInfo(StiMapID.Netherlands),
                        new StiMapToolboxInfo(StiMapID.Switzerland),
                        new StiMapToolboxInfo(StiMapID.Belarus),
                        new StiMapToolboxInfo(StiMapID.Bulgaria),
                        new StiMapToolboxInfo(StiMapID.CzechRepublic),
                        new StiMapToolboxInfo(StiMapID.Poland),
                        new StiMapToolboxInfo(StiMapID.Romania),
                        new StiMapToolboxInfo(StiMapID.Russia, new string[] { "EN", "RU" }),
                        new StiMapToolboxInfo(StiMapID.Slovakia),
                        new StiMapToolboxInfo(StiMapID.Ukraine),
                        new StiMapToolboxInfo(StiMapID.Denmark),
                        new StiMapToolboxInfo(StiMapID.Estonia),
                        new StiMapToolboxInfo(StiMapID.Finland),
                        new StiMapToolboxInfo(StiMapID.Hungary),
                        new StiMapToolboxInfo(StiMapID.Iceland),
                        new StiMapToolboxInfo(StiMapID.Latvia),
                        new StiMapToolboxInfo(StiMapID.Lithuania),
                        new StiMapToolboxInfo(StiMapID.Norway),
                        new StiMapToolboxInfo(StiMapID.Scandinavia),
                        new StiMapToolboxInfo(StiMapID.Sweden),
                        new StiMapToolboxInfo(StiMapID.Albania),
                        new StiMapToolboxInfo(StiMapID.Andorra),
                        new StiMapToolboxInfo(StiMapID.BosniaAndHerzegovina),
                        new StiMapToolboxInfo(StiMapID.Croatia),
                        new StiMapToolboxInfo(StiMapID.Cyprus),
                        new StiMapToolboxInfo(StiMapID.Greece),
                        new StiMapToolboxInfo(StiMapID.Italy, new string[] { "EN", "IT" }),
                        new StiMapToolboxInfo(StiMapID.Macedonia),
                        new StiMapToolboxInfo(StiMapID.Malta),
                        new StiMapToolboxInfo(StiMapID.Montenegro),
                        new StiMapToolboxInfo(StiMapID.Portugal),
                        new StiMapToolboxInfo(StiMapID.SanMarino),
                        new StiMapToolboxInfo(StiMapID.Serbia),
                        new StiMapToolboxInfo(StiMapID.Slovenia),
                        new StiMapToolboxInfo(StiMapID.Spain),
                        new StiMapToolboxInfo(StiMapID.Vatican),
                        new StiMapToolboxInfo(StiMapID.Armenia),
                        new StiMapToolboxInfo(StiMapID.Azerbaijan),
                        new StiMapToolboxInfo(StiMapID.EU),
                        new StiMapToolboxInfo(StiMapID.EUWithUnitedKingdom),
                        new StiMapToolboxInfo(StiMapID.Georgia),
                        new StiMapToolboxInfo(StiMapID.Kazakhstan),
                        new StiMapToolboxInfo(StiMapID.Moldova),
                        new StiMapToolboxInfo(StiMapID.Turkey),
                    }
                },
                new StiMapToolboxInfo(StiMapCategory.NorthAmerica, "North America", "Северная Америка")
                {
                    Infos = new List<StiMapToolboxInfo>()
                    {
                        new StiMapToolboxInfo(StiMapID.NorthAmerica),
                        new StiMapToolboxInfo(StiMapID.USA),
                        new StiMapToolboxInfo(StiMapID.Canada),
                        new StiMapToolboxInfo(StiMapID.USAAndCanada),
                        new StiMapToolboxInfo(StiMapID.Mexico),
                    }
                },
                new StiMapToolboxInfo(StiMapCategory.SouthAmerica, "South America", "Южная Америка")
                {
                    Infos = new List<StiMapToolboxInfo>()
                    {
                        new StiMapToolboxInfo(StiMapID.SouthAmerica),
                        new StiMapToolboxInfo(StiMapID.Argentina),
                        new StiMapToolboxInfo(StiMapID.ArgentinaFD),
                        new StiMapToolboxInfo(StiMapID.Bolivia),
                        new StiMapToolboxInfo(StiMapID.Brazil),
                        new StiMapToolboxInfo(StiMapID.Chile),
                        new StiMapToolboxInfo(StiMapID.Colombia),
                        new StiMapToolboxInfo(StiMapID.Ecuador),
                        new StiMapToolboxInfo(StiMapID.FalklandIslands),
                        new StiMapToolboxInfo(StiMapID.Guyana),
                        new StiMapToolboxInfo(StiMapID.Paraguay),
                        new StiMapToolboxInfo(StiMapID.Peru),
                        new StiMapToolboxInfo(StiMapID.Suriname),
                        new StiMapToolboxInfo(StiMapID.Uruguay),
                        new StiMapToolboxInfo(StiMapID.Venezuela),
                    }
                },
                new StiMapToolboxInfo(StiMapCategory.Asia, "Asia", "Азия")
                {
                    Infos = new List<StiMapToolboxInfo>()
                    {
                        new StiMapToolboxInfo(StiMapID.Afghanistan),
                        new StiMapToolboxInfo(StiMapID.Asia),
                        new StiMapToolboxInfo(StiMapID.SoutheastAsia),
                        new StiMapToolboxInfo(StiMapID.Armenia),
                        new StiMapToolboxInfo(StiMapID.Azerbaijan),
                        new StiMapToolboxInfo(StiMapID.China),
                        new StiMapToolboxInfo(StiMapID.Georgia),
                        new StiMapToolboxInfo(StiMapID.Taiwan),
                        new StiMapToolboxInfo(StiMapID.India),
                        new StiMapToolboxInfo(StiMapID.Israel),
                        new StiMapToolboxInfo(StiMapID.Japan),
                        new StiMapToolboxInfo(StiMapID.Kazakhstan),
                        new StiMapToolboxInfo(StiMapID.Malaysia),
                        new StiMapToolboxInfo(StiMapID.MiddleEast),
                        new StiMapToolboxInfo(StiMapID.Oman),
                        new StiMapToolboxInfo(StiMapID.Philippines),
                        new StiMapToolboxInfo(StiMapID.Qatar),
                        new StiMapToolboxInfo(StiMapID.SaudiArabia),
                        new StiMapToolboxInfo(StiMapID.SouthKorea),
                        new StiMapToolboxInfo(StiMapID.Turkey),
                        new StiMapToolboxInfo(StiMapID.Thailand),
                        new StiMapToolboxInfo(StiMapID.Vietnam),
                    }
                },
                new StiMapToolboxInfo(StiMapCategory.Oceania, "Oceania", "Океания")
                {
                    Infos = new List<StiMapToolboxInfo>()
                    {
                        new StiMapToolboxInfo(StiMapID.Australia),
                        new StiMapToolboxInfo(StiMapID.Indonesia),
                        new StiMapToolboxInfo(StiMapID.NewZealand),
                        new StiMapToolboxInfo(StiMapID.Oceania),
                    }
                },
                new StiMapToolboxInfo(StiMapCategory.Africa, "Africa", "Африка")
                {
                    Infos = new List<StiMapToolboxInfo>()
                    {
                        new StiMapToolboxInfo(StiMapID.SouthAfrica),
                        new StiMapToolboxInfo(StiMapID.CentralAfricanRepublic),
                    }
                }
            };

            return mapItemsCache;
        }

        public static StiInfographicsToolboxInfo[] GetBarCodesToolboxItems()
        {
            return new[]
            {
                new StiInfographicsToolboxInfo(
                    new List<StiBarCodeTypeService>
                    {
                            new StiQRCodeBarCodeType(),
                            new StiDataMatrixBarCodeType(),
                            new StiMaxicodeBarCodeType(),
                            new StiPdf417BarCodeType(),
                            new StiAztecBarCodeType(),
                    }
                    , StiLocalization.Get("BarCode", "TwoDimensional"), "StiQRCodeBarCodeType"),
                new StiInfographicsToolboxInfo(
                    new List<StiBarCodeTypeService>
                    {
                            new StiEAN128aBarCodeType(),
                            new StiEAN128bBarCodeType(),
                            new StiEAN128cBarCodeType(),
                            new StiEAN128AutoBarCodeType(),
                            new StiEAN13BarCodeType(),
                            new StiEAN8BarCodeType(),
                            new StiUpcABarCodeType(),
                            new StiUpcEBarCodeType(),
                            new StiUpcSup2BarCodeType(),
                            new StiUpcSup5BarCodeType(),
                            new StiJan13BarCodeType(),
                            new StiJan8BarCodeType(),
                    }
                    , "EAN\\UPC", "StiPharmacodeBarCodeType"),
                new StiInfographicsToolboxInfo(
                    new List<StiBarCodeTypeService>
                    {
                            new StiGS1_128BarCodeType(),
                            new StiGS1DataMatrixBarCodeType(),
                            new StiGS1QRCodeBarCodeType(),
                            new StiSSCC18BarCodeType(),
                            new StiITF14BarCodeType(),
                    }
                    , "GS1", "StiPharmacodeBarCodeType"),

                new StiInfographicsToolboxInfo(
                    new List<StiBarCodeTypeService>
                    {
                            new StiAustraliaPost4StateBarCodeType(),
                            new StiIntelligentMail4StateBarCodeType(),
                            new StiPostnetBarCodeType(),
                            new StiDutchKIXBarCodeType(),
                            new StiRoyalMail4StateBarCodeType(),
                            new StiFIMBarCodeType()
                    }
                    , StiLocalization.Get("BarCode", "Post"), "StiAustraliaPost4StateBarCodeType"),
                new StiInfographicsToolboxInfo(
                    new List<StiBarCodeTypeService>
                    {
                            new StiPharmacodeBarCodeType(),
                            new StiCode11BarCodeType(),
                            new StiCode128aBarCodeType(),
                            new StiCode128bBarCodeType(),
                            new StiCode128cBarCodeType(),
                            new StiCode128AutoBarCodeType(),
                            new StiCode39BarCodeType(),
                            new StiCode39ExtBarCodeType(),
                            new StiCode93BarCodeType(),
                            new StiCode93ExtBarCodeType(),
                            new StiCodabarBarCodeType(),
                            new StiIsbn10BarCodeType(),
                            new StiIsbn13BarCodeType(),
                            new StiMsiBarCodeType(),
                            new StiPlesseyBarCodeType(),
                            new StiInterleaved2of5BarCodeType(),
                            new StiStandard2of5BarCodeType()
                    }
                    , StiLocalization.Get("FormDesigner", "Others"), "StiGS1_128BarCodeType"),
            };
        }

        public static StiInfographicsToolboxInfo[] GetGaugeToolboxItems(bool allowGaugeV2 = false)
        {
            return GetAllGaugeToolboxItems(allowGaugeV2 && !StiGaugeV2InitHelper.AllowOldEditor);
        }

        public static StiInfographicsToolboxInfo[] GetAllGaugeToolboxItems(bool isV2)
        {
            if (isV2)
            {
                return new[]
                {
                    new StiInfographicsToolboxInfo(StiInfographicsItemType.Gauge, StiGaugeType.FullCircular, StiLocalization.Get("PropertyEnum", "StiGaugeTypeFullCircular"), "RadialBar"),
                    new StiInfographicsToolboxInfo(StiInfographicsItemType.Gauge, StiGaugeType.HalfCircular, StiLocalization.Get("PropertyEnum", "StiGaugeTypeHalfCircular"), "HalfDonuts"),
                    new StiInfographicsToolboxInfo(StiInfographicsItemType.Gauge, StiGaugeType.Linear, StiLocalization.Get("PropertyEnum", "StiGaugeTypeLinear"), "VerticalLinear"),
                    new StiInfographicsToolboxInfo(StiInfographicsItemType.Gauge, StiGaugeType.HorizontalLinear, StiLocalization.Get("PropertyEnum", "StiGaugeTypeHorizontalLinear"), "HorizontalLinear"),
                    new StiInfographicsToolboxInfo(StiInfographicsItemType.Gauge, StiGaugeType.Bullet, StiLocalization.Get("PropertyEnum", "StiGaugeTypeBullet"), "Bullet"),
                };
            }
            else
            {
                return new[]
                {
                    new StiInfographicsToolboxInfo(StiInfographicsItemType.Gauge, StiGaugeElemenType.RadialElement, StiLocalization.Get("Gauge", "RadialScale"), "RadialScale"),
                    new StiInfographicsToolboxInfo(StiInfographicsItemType.Gauge, StiGaugeElemenType.LinearElement, StiLocalization.Get("Gauge", "LinearScale"), "LinearScale"),
                };
            }
        }

        public static StiInfographicsToolboxInfo[] GetSignatureToolboxItems()
        {
            return new[]
            {
                new StiInfographicsToolboxInfo(StiInfographicsItemType.Signature, typeof(StiElectronicSignature), StiLocalization.Get("Components", "StiElectronicSignature"), "StiElectronicSignature"),
                new StiInfographicsToolboxInfo(StiInfographicsItemType.Signature, typeof(StiPdfDigitalSignature), StiLocalization.Get("Components", "StiPdfDigitalSignature"), "StiPdfDigitalSignature"),
            };
        }

        public static StiInfographicsToolboxInfo[] GetChartToolboxItems()
        {
            return new[]
            {
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "ClusteredColumn"), "ClusteredColumn")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiClusteredColumnSeries), StiLocalization.Get("Chart", "ClusteredColumn"), "ClusteredColumn"),
                        new StiInfographicsToolboxInfo(typeof(StiStackedColumnSeries), StiLocalization.Get("Chart", "StackedColumn"), "StackedColumn"),
                        new StiInfographicsToolboxInfo(typeof(StiFullStackedColumnSeries), StiLocalization.Get("Chart", "FullStackedColumn"), "FullStackedColumn"),
                        new StiInfographicsToolboxInfo(typeof(StiClusteredColumnSeries3D), $"3D {StiLocalization.Get("Chart", "ClusteredColumn")}", "ClusteredColumn3D"),
                        new StiInfographicsToolboxInfo(typeof(StiStackedColumnSeries3D), $"3D {StiLocalization.Get("Chart", "StackedColumn")}", "StackedColumn3D"),
                        new StiInfographicsToolboxInfo(typeof(StiFullStackedColumnSeries3D), $"3D {StiLocalization.Get("Chart", "FullStackedColumn")}", "FullStackedColumn3D"),
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Line"), "Line")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiLineSeries), StiLocalization.Get("Chart", "Line"), "Line"),
                        new StiInfographicsToolboxInfo(typeof(StiStackedLineSeries), StiLocalization.Get("Chart", "StackedLine"), "StackedLine"),
                        new StiInfographicsToolboxInfo(typeof(StiFullStackedLineSeries), StiLocalization.Get("Chart", "FullStackedLine"), "FullStackedLine"),

                        new StiInfographicsToolboxInfo(typeof(StiSplineSeries), StiLocalization.Get("Chart", "Spline"), "Spline", true),
                        new StiInfographicsToolboxInfo(typeof(StiStackedSplineSeries), StiLocalization.Get("Chart", "StackedSpline"), "StackedSpline"),
                        new StiInfographicsToolboxInfo(typeof(StiFullStackedSplineSeries), StiLocalization.Get("Chart", "FullStackedSpline"), "FullStackedSpline"),

                        new StiInfographicsToolboxInfo(typeof(StiSteppedLineSeries), StiLocalization.Get("Chart", "SteppedLine"), "SteppedLine", true),

                        //new StiInfographicsToolboxInfo(typeof(StiLineSeries3D), $"3D {StiLocalization.Get("Chart", "Line")}", "Line"),
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Pie"), "Pie")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiPieSeries), StiLocalization.Get("Chart", "Pie"), "Pie"),
                        new StiInfographicsToolboxInfo(typeof(StiPie3dSeries), $"3D {StiLocalization.Get("Chart", "Pie")}", "Pie3d"),
                        new StiInfographicsToolboxInfo(typeof(StiDoughnutSeries), StiLocalization.Get("Chart", "Doughnut"), "Doughnut"),
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "ClusteredBar"), "ClusteredBar")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiClusteredBarSeries), StiLocalization.Get("Chart", "ClusteredBar"), "ClusteredBar"),
                        new StiInfographicsToolboxInfo(typeof(StiStackedBarSeries), StiLocalization.Get("Chart", "StackedBar"), "StackedBar"),
                        new StiInfographicsToolboxInfo(typeof(StiFullStackedBarSeries), StiLocalization.Get("Chart", "FullStackedBar"), "FullStackedBar"),
                        new StiInfographicsToolboxInfo(typeof(StiGanttSeries), StiLocalization.Get("Chart", "Gantt"), "Gantt"),
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Area"), "Area")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiAreaSeries), StiLocalization.Get("Chart", "Area"), "Area"),
                        new StiInfographicsToolboxInfo(typeof(StiStackedAreaSeries), StiLocalization.Get("Chart", "StackedArea"), "StackedArea"),
                        new StiInfographicsToolboxInfo(typeof(StiFullStackedAreaSeries), StiLocalization.Get("Chart", "FullStackedArea"), "FullStackedArea"),

                        new StiInfographicsToolboxInfo(typeof(StiSplineAreaSeries), StiLocalization.Get("Chart", "SplineArea"), "SplineArea", true),
                        new StiInfographicsToolboxInfo(typeof(StiStackedSplineAreaSeries), StiLocalization.Get("Chart", "StackedSplineArea"), "StackedSplineArea"),
                        new StiInfographicsToolboxInfo(typeof(StiFullStackedSplineAreaSeries), StiLocalization.Get("Chart", "FullStackedSplineArea"), "FullStackedSplineArea"),

                        new StiInfographicsToolboxInfo(typeof(StiSteppedAreaSeries), StiLocalization.Get("Chart", "SteppedArea"), "SteppedArea", true)
                    }
                },
                new StiInfographicsToolboxInfo(typeof(StiRangeSeries), StiLocalization.Get("Chart", "Range"), "Range")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiRangeSeries), StiLocalization.Get("Chart", "Range"), "Range"),
                        new StiInfographicsToolboxInfo(typeof(StiSplineRangeSeries), StiLocalization.Get("Chart", "SplineRange"), "SplineRange"),
                        new StiInfographicsToolboxInfo(typeof(StiSteppedRangeSeries), StiLocalization.Get("Chart", "SteppedRange"), "SteppedRange"),
                        new StiInfographicsToolboxInfo(typeof(StiRangeBarSeries), StiLocalization.Get("Chart", "RangeBar"), "RangeBar"),
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Scatter"), "Scatter")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiScatterSeries), StiLocalization.Get("Chart", "Scatter"), "Scatter"),
                        new StiInfographicsToolboxInfo(typeof(StiScatterLineSeries), StiLocalization.Get("Chart", "ScatterLine"), "ScatterLine"),
                        new StiInfographicsToolboxInfo(typeof(StiScatterSplineSeries), StiLocalization.Get("Chart", "ScatterSpline"), "ScatterSpline"),
                        new StiInfographicsToolboxInfo(typeof(StiBubbleSeries), StiLocalization.Get("Chart", "Bubble"), "Bubble"),
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Radar"), "RadarArea")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiRadarPointSeries), StiLocalization.Get("Chart", "RadarPoint"), "RadarPoint"),
                        new StiInfographicsToolboxInfo(typeof(StiRadarLineSeries), StiLocalization.Get("Chart", "RadarLine"), "RadarLine"),
                        new StiInfographicsToolboxInfo(typeof(StiRadarAreaSeries), StiLocalization.Get("Chart", "RadarArea"), "RadarArea")
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Funnel"), "Funnel")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiFunnelSeries), StiLocalization.Get("Chart", "Funnel"), "Funnel"),
                        new StiInfographicsToolboxInfo(typeof(StiFunnelWeightedSlicesSeries), StiLocalization.Get("Chart", "FunnelWeightedSlices"), "FunnelWeightedSlices")
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Financial"), "Candlestick")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiCandlestickSeries), StiLocalization.Get("Chart", "Candlestick"), "Candlestick"),
                        new StiInfographicsToolboxInfo(typeof(StiStockSeries), StiLocalization.Get("Chart", "Stock"), "Stock"),

                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Treemap"), "Treemap")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiTreemapSeries), StiLocalization.Get("Chart", "Treemap"), "Treemap")
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Sunburst"), "Sunburst")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiSunburstSeries), StiLocalization.Get("Chart", "Sunburst"), "Sunburst"),
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Histogram"), "Histogram")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiHistogramSeries), StiLocalization.Get("Chart", "Histogram"), "Histogram"),
                        new StiInfographicsToolboxInfo(typeof(StiParetoSeries), StiLocalization.Get("Chart", "Pareto"), "Pareto"),
                        new StiInfographicsToolboxInfo(typeof(StiRibbonSeries), StiLocalization.Get("Chart", "Ribbon"), "Ribbon"),
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "BoxAndWhisker"), "BoxAndWhisker")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiBoxAndWhiskerSeries), StiLocalization.Get("Chart", "BoxAndWhisker"), "BoxAndWhisker")
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Waterfall"), "Waterfall")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiWaterfallSeries), StiLocalization.Get("Chart", "Waterfall"), "Waterfall"),
                    }
                },
                new StiInfographicsToolboxInfo(null, StiLocalization.Get("Chart", "Pictorial"), "Pictorial")
                {
                    Infos = new List<StiInfographicsToolboxInfo>()
                    {
                        new StiInfographicsToolboxInfo(typeof(StiPictorialSeries), StiLocalization.Get("Chart", "Pictorial"), "Pictorial"),
                        new StiInfographicsToolboxInfo(typeof(StiPictorialStackedSeries), StiLocalization.Get("Chart", "PictorialStacked"), "PictorialStacked")
                    }
                }
            };
        }

        public static Dictionary<string, List<StiComponent>> GetShapesToolboxItems()
        {
            return new Dictionary<string, List<StiComponent>>()
            {
                {"BasicShapes", new List<StiComponent>()
                    {
                        new StiHorizontalLinePrimitive(),
                        new StiVerticalLinePrimitive(),
                        new StiRectanglePrimitive(),
                        new StiRoundedRectanglePrimitive()
                    } 
                },
                {"EquationShapes", new List<StiComponent>()
                    {
                        new StiShape { ShapeType = new StiPlusShapeType() },
                        new StiShape { ShapeType = new StiMinusShapeType() },
                        new StiShape { ShapeType = new StiMultiplyShapeType() },
                        new StiShape { ShapeType = new StiDivisionShapeType() },
                        new StiShape { ShapeType = new StiEqualShapeType() },
                    } 
                },
                {"BlockArrows", new List<StiComponent>()
                    {
                        new StiShape { ShapeType = new StiArrowShapeType(){ Direction = StiShapeDirection.Right } },
                        new StiShape { ShapeType = new StiArrowShapeType(){ Direction = StiShapeDirection.Left } },
                        new StiShape { ShapeType = new StiArrowShapeType(){ Direction = StiShapeDirection.Up } },
                        new StiShape { ShapeType = new StiArrowShapeType(){ Direction = StiShapeDirection.Down } },
                        new StiShape { ShapeType = new StiComplexArrowShapeType() },
                        new StiShape { ShapeType = new StiFlowchartSortShapeType() },
                        new StiShape { ShapeType = new StiBentArrowShapeType() },
                        new StiShape { ShapeType = new StiChevronShapeType() },
                    } 
                },
                {"Lines", new List<StiComponent>()
                    {
                        new StiShape { ShapeType = new StiDiagonalDownLineShapeType() },
                        new StiShape { ShapeType = new StiDiagonalUpLineShapeType() },
                        new StiShape { ShapeType = new StiHorizontalLineShapeType() },
                        new StiShape { ShapeType = new StiLeftAndRightLineShapeType() },
                        new StiShape { ShapeType = new StiTopAndBottomLineShapeType() },
                        new StiShape { ShapeType = new StiVerticalLineShapeType() }
                    } 
                },
                {"Flowchart", new List<StiComponent>()
                    {
                        new StiShape { ShapeType = new StiOvalShapeType() },
                        new StiShape { ShapeType = new StiRectangleShapeType() },
                        new StiShape { ShapeType = new StiRoundedRectangleShapeType() },
                        new StiShape { ShapeType = new StiTriangleShapeType() },
                        new StiShape { ShapeType = new StiFlowchartCardShapeType() },
                        new StiShape { ShapeType = new StiFlowchartCollateShapeType() },
                        new StiShape { ShapeType = new StiFlowchartDecisionShapeType() },
                        new StiShape { ShapeType = new StiFlowchartManualInputShapeType() },
                        new StiShape { ShapeType = new StiFlowchartOffPageConnectorShapeType() },
                        new StiShape { ShapeType = new StiFlowchartPreparationShapeType() },
                        new StiShape { ShapeType = new StiFrameShapeType() },
                        new StiShape { ShapeType = new StiParallelogramShapeType() },
                        new StiShape { ShapeType = new StiRegularPentagonShapeType() },
                        new StiShape { ShapeType = new StiTrapezoidShapeType() },
                        new StiShape { ShapeType = new StiOctagonShapeType() },
                        new StiShape { ShapeType = new StiSnipSameSideCornerRectangleShapeType() },
                        new StiShape { ShapeType = new StiSnipDiagonalSideCornerRectangleShapeType() }
                    } 
                }
            };
        }
        #endregion
    }
}