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

using System.Collections.Generic;
using Stimulsoft.Report.Components.ShapeTypes;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        public sealed partial class Services
        {
            private static List<StiShapeTypeService> shapes;
            public static List<StiShapeTypeService> Shapes
            {
                get
                {
                    lock (lockObject)
                    {
                        return shapes ?? (shapes = new List<StiShapeTypeService>
                        {
                            new StiArrowShapeType(),
                            new StiDiagonalDownLineShapeType(),
                            new StiDiagonalUpLineShapeType(),
                            new StiHorizontalLineShapeType(),
                            new StiLeftAndRightLineShapeType(),
                            new StiOvalShapeType(),
                            new StiRectangleShapeType(),
                            new StiRoundedRectangleShapeType(),
                            new StiOctagonShapeType(),
                            new StiTopAndBottomLineShapeType(),
                            new StiTriangleShapeType(),
                            new StiVerticalLineShapeType(),
                            new StiComplexArrowShapeType(),
                            new StiBentArrowShapeType(),
                            new StiChevronShapeType(),
                            new StiDivisionShapeType(),
                            new StiEqualShapeType(),
                            new StiFlowchartCardShapeType(),
                            new StiFlowchartCollateShapeType(),
                            new StiFlowchartDecisionShapeType(),
                            new StiFlowchartManualInputShapeType(),
                            new StiFlowchartOffPageConnectorShapeType(),
                            new StiFlowchartPreparationShapeType(),
                            new StiFlowchartSortShapeType(),
                            new StiFrameShapeType(),
                            new StiMinusShapeType(),
                            new StiMultiplyShapeType(),
                            new StiParallelogramShapeType(),
                            new StiPlusShapeType(),
                            new StiRegularPentagonShapeType(),
                            new StiTrapezoidShapeType(),
                            new StiSnipSameSideCornerRectangleShapeType(),
                            new StiSnipDiagonalSideCornerRectangleShapeType()
                        });
                    }
                }
            }
        }
	}
}