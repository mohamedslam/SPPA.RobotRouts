#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.ShapeTypes;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters.Context.Animation;
using System;
using System.Collections;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public abstract class StiPainter
    {
        #region Fields
        private static Hashtable typeToGdiPainter = new Hashtable();
        private static Hashtable typeToWpfPainter = new Hashtable();
        private static object lockObject = new object();
        #endregion

        #region Delegates
        public delegate StiPainter GetWpfComponentPainterDelegate(Type type);
        public delegate StiShapeTypePainter GetWpfShapeTypePainterDelegate(Type type);
        public delegate StiIndicatorTypePainter GetWpfIndicatorTypePainterDelegate(Type type);
        #endregion

        #region Properties
        internal StiAnimationEngine AnimationEngine { get; set; }
        #endregion  

        #region Methods
        public static StiPainter GetPainter(Type type, StiGuiMode guiMode)
        {
            lock (lockObject)
            {
                var typeToPainter = guiMode == StiGuiMode.Gdi ? typeToGdiPainter : typeToWpfPainter;
                var painter = typeToPainter[type] as StiPainter;
                if (painter != null) return painter;

                switch (guiMode)
                {
                    case StiGuiMode.Wpf:
                        painter = GetWpfComponentTypePainter(type);
                        break;

                    case StiGuiMode.Gdi:
                        painter = GetGdiComponentTypePainter(type);
                        break;
                }

                if (painter == null)
                    throw new Exception($"StiPainter for '{type}' is not found!");

                typeToPainter[type] = painter;
                return painter;
            }
        }

        private static StiPainter GetWpfComponentTypePainter(Type type)
        {
            var painter = StiWpfPainterDelegates.ComponentPainter != null ? StiWpfPainterDelegates.ComponentPainter(type) : null;
            if (painter != null)
                return painter;

            var painterAttr = type.GetCustomAttributes(typeof(StiWpfPainterAttribute), true).FirstOrDefault() as StiWpfPainterAttribute;
            var typeName = painterAttr?.PainterTypeName;

            return StiActivator.CreateObject(typeName, new object[0]) as StiPainter;
        }

        private static StiComponentGdiPainter GetGdiComponentTypePainter(Type type)
        {
            if (type == typeof(StiChart))
                return new StiChartGdiPainter();

            if (type == typeof(StiClone))
                return new StiCloneGdiPainter();

            if (type == typeof(StiCheckBox))
                return new StiCheckBoxGdiPainter();

            if (type == typeof(StiTableCellCheckBox))
                return new StiTableCellCheckBoxGdiPainter();

            if (type == typeof(StiContourText))
                return new StiContourTextGdiPainter();

            if (type == typeof(StiCrossTab))
                return new StiCrossTabGdiPainter();

            if (type == typeof(StiHorizontalLinePrimitive))
                return new StiHorizontalLinePrimitiveGdiPainter();

            if (type == typeof(StiImage))
                return new StiImageGdiPainter();

            if (type == typeof(StiTableCellImage))
                return new StiTableCellImageGdiPainter();

            if (type == typeof(StiPage))
                return new StiPageGdiPainter();

            if (type == typeof(StiPointPrimitive) ||
                type == typeof(StiEndPointPrimitive) ||
                type == typeof(StiStartPointPrimitive))
                return new StiPointPrimitiveGdiPainter();

            if (type == typeof(StiRoundedRectanglePrimitive))
                return new StiRoundedRectanglePrimitiveGdiPainter();

            if (type == typeof(StiRectanglePrimitive))
                return new StiRectanglePrimitiveGdiPainter();

            if (type == typeof(StiRichText))
                return new StiRichTextGdiPainter();

            if (type == typeof(StiTableCellRichText))
                return new StiTableCellRichTextGdiPainter();

            if (type == typeof(StiShape))
                return new StiShapeGdiPainter();

            if (type == typeof(StiSubReport))
                return new StiSubReportGdiPainter();

            if (type == typeof(StiTextInCells))
                return new StiTextInCellsGdiPainter();

            if (type == typeof(StiTableCell))
                return new StiTableCellTextGdiPainter();

            if (type == typeof(StiUndefinedComponent))
                return new StiUndefinedComponentGdiPainter();

            if (type == typeof(StiVerticalLinePrimitive))
                return new StiVerticalLinePrimitiveGdiPainter();

            if (type == typeof(StiBand) ||
                type == typeof(StiOverlayBand) ||
                type == typeof(StiColumnFooterBand) ||
                type == typeof(StiColumnHeaderBand) ||
                type == typeof(StiPageFooterBand) ||
                type == typeof(StiPageHeaderBand) ||
                type == typeof(StiReportSummaryBand) ||
                type == typeof(StiReportTitleBand) ||
                type == typeof(StiChildBand) ||
                type == typeof(StiDataBand) ||
                type == typeof(StiTable) ||
                type == typeof(StiFooterBand) ||
                type == typeof(StiGroupFooterBand) ||
                type == typeof(StiGroupHeaderBand) ||
                type == typeof(StiHeaderBand) ||
                type == typeof(StiEmptyBand))
                return new StiBandGdiPainter();

            if (type == typeof(StiContainer) ||
                type == typeof(StiPanel) ||
                type == typeof(StiFooterMarkerContainer))
                return new StiContainerGdiPainter();

            if (type == typeof(StiZipCode))
                return new StiZipCodeGdiPainter();

            if (type == typeof(StiView))
                return new StiViewGdiPainter();

            if (type == typeof(StiBarCode))
                return new StiBarCodeGdiPainter();

            if (type == typeof(StiComponent))
                return new StiComponentGdiPainter();

            if (type == typeof(StiText) ||
                type == typeof(StiSystemText))
                return new StiTextGdiPainter();

            if (type == typeof(StiCrossRowTotal) ||
                type == typeof(StiCrossColumnTotal) ||
                type == typeof(StiCrossSummary) ||
                type == typeof(StiCrossSummaryHeader) ||
                type == typeof(StiCrossTitle) ||
                type == typeof(StiCrossTotal))
                return new StiTextGdiPainter();

            if (type == typeof(StiCrossColumn) ||
                type == typeof(StiCrossRow))
                return new StiCrossHeaderGdiPainter();

#if !NETSTANDARD
            if (type == typeof(StiForm))
                return new StiFormGdiPainter();

            if (type == typeof(StiButtonControl))
                return new StiButtonControlGdiPainter();

            if (type == typeof(StiCheckBoxControl))
                return new StiCheckBoxControlGdiPainter();

            if (type == typeof(StiCheckedListBoxControl))
                return new StiCheckedListBoxControlGdiPainter();

            if (type == typeof(StiComboBoxControl) || type == typeof(StiLookUpBoxControl))
                return new StiComboBoxControlGdiPainter();

            if (type == typeof(StiCustomControl))
                return new StiCustomControlGdiPainter();

            if (type == typeof(StiDateTimePickerControl))
                return new StiDateTimePickerControlGdiPainter();

            if (type == typeof(StiGridControl))
                return new StiGridControlGdiPainter();

            if (type == typeof(StiGroupBoxControl))
                return new StiGroupBoxControlGdiPainter();

            if (type == typeof(StiLabelControl))
                return new StiLabelControlGdiPainter();

            if (type == typeof(StiListBoxControl))
                return new StiListBoxControlGdiPainter();

            if (type == typeof(StiListViewControl))
                return new StiListViewControlGdiPainter();

            if (type == typeof(StiNumericUpDownControl))
                return new StiNumericUpDownControlGdiPainter();

            if (type == typeof(StiPanelControl))
                return new StiPanelControlGdiPainter();

            if (type == typeof(StiPictureBoxControl))
                return new StiPictureBoxControlGdiPainter();

            if (type == typeof(StiRadioButtonControl))
                return new StiRadioButtonControlGdiPainter();

            if (type == typeof(StiRichTextBoxControl))
                return new StiRichTextBoxControlGdiPainter();

            if (type == typeof(StiTextBoxControl))
                return new StiTextBoxControlGdiPainter();

            if (type == typeof(StiTreeViewControl))
                return new StiTreeViewControlGdiPainter();
#endif

            var painterAttr = type.GetCustomAttributes(typeof(StiGdiPainterAttribute), true).FirstOrDefault() as StiGdiPainterAttribute;
            return painterAttr != null ? StiActivator.CreateObject(painterAttr.PainterTypeName, new object[0]) as StiComponentGdiPainter : null;
        }

        public static StiShapeTypePainter GetShapePainter(Type type, StiGuiMode guiMode)
        {
            lock (lockObject)
            {
                var typeToPainter = guiMode == StiGuiMode.Gdi ? typeToGdiPainter : typeToWpfPainter;
                var painter = typeToPainter[type] as StiShapeTypePainter;
                if (painter != null)
                    return painter;

                switch (guiMode)
                {
                    case StiGuiMode.Wpf:
                        painter = GetWpfShapeTypePainter(type);
                        break;

                    case StiGuiMode.Gdi:
                        painter = GetGdiShapeTypePainter(type);
                        break;
                }

                if (painter == null)
                    throw new Exception($"StiPainter for '{type}' is not found!");

                typeToPainter[type] = painter;
                return painter;
            }
        }

        private static StiShapeTypePainter GetWpfShapeTypePainter(Type type)
        {
            var painter = StiWpfPainterDelegates.ShapePainter != null ? StiWpfPainterDelegates.ShapePainter(type) : null;
            if (painter != null)
                return painter;

            var painterAttr = type.GetCustomAttributes(typeof(StiWpfShapeTypePainterAttribute), true)
                .FirstOrDefault() as StiWpfShapeTypePainterAttribute;

            var typeName = painterAttr?.PainterTypeName;

            return StiActivator.CreateObject(typeName, new object[0]) as StiShapeTypePainter;
        }

        private static StiShapeTypePainter GetGdiShapeTypePainter(Type type)
        {
            if (type == typeof(StiArrowShapeType))
                return new StiArrowGdiShapeTypePainter();

            if (type == typeof(StiDivisionShapeType))
                return new StiDivisionGdiShapeTypePainter();

            if (type == typeof(StiChevronShapeType))
                return new StiChevronGdiShapeTypePainter();

            if (type == typeof(StiComplexArrowShapeType))
                return new StiComplexArrowGdiShapeTypePainter();

            if (type == typeof(StiDiagonalDownLineShapeType))
                return new StiDiagonalDownLineGdiShapeTypePainter();

            if (type == typeof(StiDiagonalUpLineShapeType))
                return new StiDiagonalUpLineGdiShapeTypePainter();

            if (type == typeof(StiEqualShapeType))
                return new StiEqualGdiShapeTypePainter();

            if (type == typeof(StiFlowchartCardShapeType))
                return new StiFlowchartCardGdiShapeTypePainter();

            if (type == typeof(StiFlowchartCollateShapeType))
                return new StiFlowchartCollateGdiShapeTypePainter();

            if (type == typeof(StiFlowchartDecisionShapeType))
                return new StiFlowchartDecisionGdiShapeTypePainter();

            if (type == typeof(StiFlowchartManualInputShapeType))
                return new StiFlowchartManualInputGdiShapeTypePainter();

            if (type == typeof(StiFlowchartOffPageConnectorShapeType))
                return new StiFlowchartOffPageConnectorGdiShapeTypePainter();

            if (type == typeof(StiFlowchartPreparationShapeType))
                return new StiFlowchartPreparationGdiShapeTypePainter();

            if (type == typeof(StiFlowchartSortShapeType))
                return new StiFlowchartSortGdiShapeTypePainter();

            if (type == typeof(StiFrameShapeType))
                return new StiFrameGdiShapeTypePainter();

            if (type == typeof(StiHorizontalLineShapeType))
                return new StiHorizontalLineGdiShapeTypePainter();

            if (type == typeof(StiLeftAndRightLineShapeType))
                return new StiLeftAndRightLineGdiShapeTypePainter();

            if (type == typeof(StiMinusShapeType))
                return new StiMinusGdiShapeTypePainter();

            if (type == typeof(StiMultiplyShapeType))
                return new StiMultiplyGdiShapeTypePainter();

            if (type == typeof(StiOctagonShapeType))
                return new StiOctagonGdiShapeTypePainter();

            if (type == typeof(StiOvalShapeType))
                return new StiOvalGdiShapeTypePainter();

            if (type == typeof(StiParallelogramShapeType))
                return new StiParallelogramGdiShapeTypePainter();

            if (type == typeof(StiPlusShapeType))
                return new StiPlusGdiShapeTypePainter();

            if (type == typeof(StiRectangleShapeType))
                return new StiRectangleGdiShapeTypePainter();

            if (type == typeof(StiRegularPentagonShapeType))
                return new StiRegularPentagonGdiShapeTypePainter();

            if (type == typeof(StiRoundedRectangleShapeType))
                return new StiRoundedRectangleGdiShapeTypePainter();

            if (type == typeof(StiSnipDiagonalSideCornerRectangleShapeType))
                return new StiSnipDiagonalSideCornerRectangleGdiShapeTypePainter();

            if (type == typeof(StiSnipSameSideCornerRectangleShapeType))
                return new StiSnipSameSideCornerRectangleGdiShapeTypePainter();

            if (type == typeof(StiTopAndBottomLineShapeType))
                return new StiTopAndBottomLineGdiShapeTypePainter();

            if (type == typeof(StiTrapezoidShapeType))
                return new StiTrapezoidGdiShapeTypePainter();

            if (type == typeof(StiTriangleShapeType))
                return new StiTriangleGdiShapeTypePainter();

            if (type == typeof(StiVerticalLineShapeType))
                return new StiVerticalLineGdiShapeTypePainter();

            if (type == typeof(StiBentArrowShapeType))
                return new StiBentArrowGdiShapeTypePainter();

            var painterAttr = type
                .GetCustomAttributes(typeof(StiGdiShapeTypePainterAttribute), true)
                .FirstOrDefault() as StiGdiShapeTypePainterAttribute;

            return painterAttr != null
                ? StiActivator.CreateObject(painterAttr.PainterTypeName, new object[0]) as StiShapeTypePainter
                : null;
        }

        public static StiIndicatorTypePainter GetIndicatorPainter(Type type, StiGuiMode guiMode)
        {
            lock (lockObject)
            {
                var typeToPainter = guiMode == StiGuiMode.Gdi ? typeToGdiPainter : typeToWpfPainter;
                var painter = typeToPainter[type] as StiIndicatorTypePainter;
                if (painter != null)
                    return painter;

                switch (guiMode)
                {
                    case StiGuiMode.Wpf:
                        painter = GetWpfIndicatorTypePainter(type);
                        break;

                    case StiGuiMode.Gdi:
                        painter = GetGdiIndicatorTypePainter(type);
                        break;
                }

                if (painter == null)
                    throw new Exception($"StiPainter for '{type}' is not found!");

                typeToPainter[type] = painter;
                return painter;
            }
        }

        private static StiIndicatorTypePainter GetWpfIndicatorTypePainter(Type type)
        {
            var painter = StiWpfPainterDelegates.IndicatorPainter != null ? StiWpfPainterDelegates.IndicatorPainter(type) : null;
            if (painter != null)
                return painter;

            var painterAttr = type
                .GetCustomAttributes(typeof(StiWpfIndicatorTypePainterAttribute), true)
                .FirstOrDefault() as StiWpfIndicatorTypePainterAttribute;

            var typeName = painterAttr?.PainterTypeName;

            return StiActivator.CreateObject(typeName, new object[0]) as StiIndicatorTypePainter;
        }

        private static StiIndicatorTypePainter GetGdiIndicatorTypePainter(Type type)
        {
            if (type == typeof(StiIconSetIndicator))
                return new StiIconSetGdiIndicatorTypePainter();

            if (type == typeof(StiDataBarIndicator))
                return new StiDataBarGdiIndicatorTypePainter();

            var painterAttr = type
                .GetCustomAttributes(typeof(StiGdiIndicatorTypePainterAttribute), true)
                .FirstOrDefault() as StiGdiIndicatorTypePainterAttribute;

            return painterAttr != null
                ? StiActivator.CreateObject(painterAttr.PainterTypeName, new object[0]) as StiIndicatorTypePainter
                : null;
        }

        public abstract Image GetImage(StiComponent component, ref float zoom, StiExportFormat format);

        public abstract void Paint(StiComponent component, StiPaintEventArgs e);

        public abstract void PaintSelection(StiComponent component, StiPaintEventArgs e);

        public abstract void PaintHighlight(StiComponent component, StiPaintEventArgs e);

        /// <summary>
        /// Gets a thumbnail image of the component.
        /// </summary>
        /// <param name="width">Width of the thumbnail image.</param>
        /// <param name="height">Height of the thumbnail image.</param>
        /// <returns>A thumbnail image of the specified size.</returns>
        public abstract Bitmap GetThumbnail(StiComponent component, int width, int height, bool isDesignTime);
#endregion
    }
}