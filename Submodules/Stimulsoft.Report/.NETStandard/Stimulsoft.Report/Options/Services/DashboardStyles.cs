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

using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Dashboard.Styles.Cards;
using System.Collections.Generic;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        public sealed partial class Services
        {
            public static class Dashboards
            {
                private static List<StiDashboardStyle> dashboardStyles;
                public static List<StiDashboardStyle> DashboardStyles
                {
                    get
                    {
                        lock (lockObject)
                        {
                            return dashboardStyles ?? (dashboardStyles = new List<StiDashboardStyle>
                            {
                                new StiBlueDashboardStyle(),
                                new StiOrangeDashboardStyle(),
                                new StiGreenDashboardStyle(),
                                new StiTurquoiseDashboardStyle(),
                                new StiSlateGrayDashboardStyle(),
                                new StiDarkBlueDashboardStyle(),
                                new StiDarkGrayDashboardStyle(),
                                new StiDarkTurquoiseDashboardStyle(),
                                new StiSilverDashboardStyle(),
                                new StiAliceBlueDashboardStyle(),
                                new StiDarkGreenDashboardStyle(),
                                new StiSiennaDashboardStyle()
                            });
                        }
                    }
                }

                private static List<StiControlElementStyle> controlStyles;
                public static List<StiControlElementStyle> ControlStyles
                {
                    get
                    {
                        lock (lockObject)
                        {
                            return controlStyles ?? (controlStyles = new List<StiControlElementStyle>
                            {
                                new StiBlueControlElementStyle(),
                                new StiOrangeControlElementStyle(),
                                new StiGreenControlElementStyle(),
                                new StiTurquoiseControlElementStyle(),
                                new StiSlateGrayControlElementStyle(),
                                new StiDarkBlueControlElementStyle(),
                                new StiDarkGrayControlElementStyle(),
                                new StiDarkTurquoiseControlElementStyle(),
                                new StiSilverControlElementStyle(),
                                new StiAliceBlueControlElementStyle(),
                                new StiDarkGreenControlElementStyle(),
                                new StiSiennaControlElementStyle(),
                            });
                        }
                    }
                }

                private static List<StiIndicatorElementStyle> indicatorStyles;
                public static List<StiIndicatorElementStyle> IndicatorStyles
                {
                    get
                    {
                        lock (lockObject)
                        {
                            return indicatorStyles ?? (indicatorStyles = new List<StiIndicatorElementStyle>
                            {
                                new StiBlueIndicatorElementStyle(),
                                new StiOrangeIndicatorElementStyle(),
                                new StiGreenIndicatorElementStyle(),
                                new StiTurquoiseIndicatorElementStyle(),
                                new StiSlateGrayIndicatorElementStyle(),
                                new StiDarkBlueIndicatorElementStyle(),
                                new StiDarkGrayIndicatorElementStyle(),
                                new StiDarkTurquoiseIndicatorElementStyle(),
                                new StiSilverIndicatorElementStyle(),
                                new StiAliceBlueIndicatorElementStyle(),
                                new StiDarkGreenIndicatorElementStyle(),
                                new StiSiennaIndicatorElementStyle(),
                            });
                        }
                    }
                }

                private static List<StiPivotElementStyle> pivotStyles;
                public static List<StiPivotElementStyle> PivotStyles
                {
                    get
                    {
                        lock (lockObject)
                        {
                            return pivotStyles ?? (pivotStyles = new List<StiPivotElementStyle>
                            {
                                new StiBluePivotElementStyle(),
                                new StiOrangePivotElementStyle(),
                                new StiGreenPivotElementStyle(),
                                new StiTurquoisePivotElementStyle(),
                                new StiSlateGrayPivotElementStyle(),
                                new StiDarkBluePivotElementStyle(),
                                new StiDarkGrayPivotElementStyle(),
                                new StiDarkTurquoisePivotElementStyle(),
                                new StiSilverPivotElementStyle(),
                                new StiAliceBluePivotElementStyle(),
                                new StiDarkGreenPivotElementStyle(),
                                new StiSiennaPivotElementStyle(),
                            });
                        }
                    }
                }

                private static List<StiProgressElementStyle> progressStyles;
                public static List<StiProgressElementStyle> ProgressStyles
                {
                    get
                    {
                        lock (lockObject)
                        {
                            return progressStyles ?? (progressStyles = new List<StiProgressElementStyle>
                            {
                                new StiBlueProgressElementStyle(),
                                new StiOrangeProgressElementStyle(),
                                new StiGreenProgressElementStyle(),
                                new StiTurquoiseProgressElementStyle(),
                                new StiSlateGrayProgressElementStyle(),
                                new StiDarkBlueProgressElementStyle(),
                                new StiDarkGrayProgressElementStyle(),
                                new StiDarkTurquoiseProgressElementStyle(),
                                new StiSilverProgressElementStyle(),
                                new StiAliceBlueProgressElementStyle(),
                                new StiDarkGreenProgressElementStyle(),
                                new StiSiennaProgressElementStyle(),
                            });
                        }
                    }
                }

                private static List<StiTableElementStyle> tableStyles;
                public static List<StiTableElementStyle> TableStyles
                {
                    get
                    {
                        lock (lockObject)
                        {
                            return tableStyles ?? (tableStyles = new List<StiTableElementStyle>
                            {
                                new StiBlueTableElementStyle(),
                                new StiOrangeTableElementStyle(),
                                new StiGreenTableElementStyle(),
                                new StiTurquoiseTableElementStyle(),
                                new StiSlateGrayTableElementStyle(),
                                new StiDarkBlueTableElementStyle(),
                                new StiDarkGrayTableElementStyle(),
                                new StiDarkTurquoiseTableElementStyle(),
                                new StiSilverTableElementStyle(),
                                new StiAliceBlueTableElementStyle(),
                                new StiDarkGreenTableElementStyle(),
                                new StiSiennaTableElementStyle()
                            });
                        }
                    }
                }

                private static List<StiCardsElementStyle> cardsStyles;
                public static List<StiCardsElementStyle> CardsStyles
                {
                    get
                    {
                        lock (lockObject)
                        {
                            return cardsStyles ?? (cardsStyles = new List<StiCardsElementStyle>
                            {
                                new StiBlueCardsElementStyle(),
                                new StiOrangeCardsElementStyle(),
                                new StiGreenCardsElementStyle(),
                                new StiTurquoiseCardsElementStyle(),
                                new StiSlateGrayCardsElementStyle(),
                                new StiDarkBlueCardsElementStyle(),
                                new StiDarkGrayCardsElementStyle(),
                                new StiDarkTurquoiseCardsElementStyle(),
                                new StiSilverCardsElementStyle(),
                                new StiAliceBlueCardsElementStyle(),
                                new StiDarkGreenCardsElementStyle(),
                                new StiSiennaCardsElementStyle()
                            });
                        }
                    }
                }
            }
        }
	}
}