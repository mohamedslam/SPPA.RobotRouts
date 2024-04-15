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

using Stimulsoft.Report.App;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Maps;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report
{
    /// <summary>
    /// This class for adjustment all aspects of Stimulsoft products.
    /// </summary>
    public sealed partial class StiOptions
	{
        public sealed partial class Services
        {
            private static List<StiComponent> components;
            public static List<StiComponent> Components
            {
                get
                {
                    lock (lockObject)
                    {
                        if (components == null)
                        {
                            components = new List<StiComponent>
                            {
                                new StiReportTitleBand(),
                                new StiReportSummaryBand(),
                                new StiPageHeaderBand(),
                                new StiPageFooterBand(),
                                new StiGroupHeaderBand(),
                                new StiGroupFooterBand(),
                                new StiHeaderBand(),
                                new StiFooterBand(),
                                new StiColumnHeaderBand(),
                                new StiColumnFooterBand(),
                                new StiHierarchicalBand(),
                                new StiEmptyBand(),
                                new StiOverlayBand(),
                                new StiDataBand(),

                                new StiTable(),
                                new StiTableCell(),
                                new StiTableCellCheckBox(),
                                new StiTableCellImage(),
                                new StiTableCellRichText(),

                                new StiPage(),
                                new StiPanel(),
                                new StiContainer(),
                                new StiCheckBox(),
                                new StiChildBand(),
                                new StiClone(),
                                new StiText(),
                                new StiSystemText(),
                                new StiTextInCells(),
                                new StiImage(),
                                new StiRichText(),
                                new StiShape(),
                                new StiSubReport(),
                                new StiWinControl(),
                                new StiZipCode(),
                                new StiSparkline(),
                                new StiTableOfContents(),

                                new StiElectronicSignature(),
                                new StiPdfDigitalSignature(),

                                new StiHorizontalLinePrimitive(),
                                new StiVerticalLinePrimitive(),
                                new StiRectanglePrimitive(),
                                new StiRoundedRectanglePrimitive(),
                                new StiStartPointPrimitive(),
                                new StiEndPointPrimitive(),

                                new StiCrossDataBand(),
                                new StiCrossFooterBand(),
                                new StiCrossGroupFooterBand(),
                                new StiCrossGroupHeaderBand(),
                                new StiCrossHeaderBand(),

                                new StiBarCode(),

                                new StiCrossTab(),
                                new StiCrossColumn(),
                                new StiCrossTotal(),
                                new StiCrossRowTotal(),
                                new StiCrossColumnTotal(),
                                new StiCrossRow(),
                                new StiCrossSummary(),
                                new StiCrossTitle(),
                                new StiCrossSummaryHeader(),

                                new StiChart(),
#if !NETSTANDARD
                                new StiButtonControl(),
                                new StiCheckBoxControl(),
                                new StiCheckedListBoxControl(),
                                new StiComboBoxControl(),
                                new StiDateTimePickerControl(),
                                new StiForm(),
                                new StiGridControl(),
                                new StiGroupBoxControl(),
                                new StiLabelControl(),
                                new StiListBoxControl(),
                                new StiLookUpBoxControl(),
                                new StiNumericUpDownControl(),
                                new StiPanelControl(),
                                new StiPictureBoxControl(),
                                new StiRadioButtonControl(),
                                new StiTextBoxControl(),
                                new StiRichTextBoxControl(),
                                new StiTreeViewControl(),
                                new StiListViewControl(),
#endif
                                new StiGauge(),
                                new StiMap(),
                                new StiMathFormula()
                            };

                            var elements = StiDashboardAssembly.LoadDashboardElements();
                            if (elements != null)
                                components.AddRange(elements);

                            var componentsUI = StiAppAssembly.LoadAppComponents();
                            if (componentsUI != null)
                                components.AddRange(componentsUI);

                            components = components.OrderBy(f => f.ToolboxPosition).ToList();
                        }
                        return components;
                    }
                }
            }
        }
	}
}