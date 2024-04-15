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
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Maps;
using System;
using System.Collections;

namespace Stimulsoft.Report.Engine
{
    public abstract class StiV2Builder : StiBuilder
    {
		#region Fields
		private static Hashtable typeToV2Builder = new Hashtable();
        #endregion

		#region Methods.Helper
		public static StiV2Builder GetBuilder(Type componentType)
		{
			var builder = typeToV2Builder[componentType] as StiV2Builder;
            if (builder != null) return builder;

            builder = GetComponentBuilder(componentType);

            if (builder == null)
                throw new Exception($"StiBuilder for '{componentType}' is not found!");

		    typeToV2Builder[componentType] = builder;
			return builder;
		}

        private static StiV2Builder GetComponentBuilder(Type type)
        {
            #region Predefined builders
            if (type == typeof(StiMathFormula)) return new StiMathFormulаV2Builder();

            if (type == typeof(StiChart)) return new StiChartV2Builder();

            if (type == typeof(StiClone)) return new StiCloneV2Builder();

            if (type == typeof(StiCrossTab)) return new StiCrossTabV2Builder();
            if (type == typeof(StiGauge)) return new StiGaugeV2Builder();
            if (type == typeof(StiImage) ||
                type == typeof(StiTableCellImage)) return new StiImageV2Builder();
            if (type == typeof(StiPage)) return new StiPageV2Builder();
            if (type == typeof(StiSubReport)) return new StiSubReportV2Builder();
            if (type == typeof(StiTextInCells)) return new StiTextInCellsV2Builder();
            if (type == typeof(StiWinControl)) return new StiWinControlV2Builder();
            if (type == typeof(StiView)) return new StiViewV2Builder();
            if (type == typeof(StiContainer) ||
                type == typeof(StiPanel) ||
                type == typeof(StiFooterMarkerContainer)) return new StiContainerV2Builder();

            if (type == typeof(StiColumnHeaderBand)) return new StiColumnHeaderBandV2Builder();
            if (type == typeof(StiColumnFooterBand)) return new StiColumnFooterBandV2Builder();
            if (type == typeof(StiHierarchicalBand)) return new StiHierarchicalBandV2Builder();
            if (type == typeof(StiDataBand) ||
                type == typeof(StiCrossDataBand) ||
                type == typeof(StiTable)) return new StiDataBandV2Builder();
            if (type == typeof(StiHeaderBand) ||
                type == typeof(StiCrossHeaderBand)) return new StiHeaderBandV2Builder();
            if (type == typeof(StiFooterBand) ||
                type == typeof(StiCrossFooterBand)) return new StiFooterBandV2Builder();
            if (type == typeof(StiGroupHeaderBand) ||
                type == typeof(StiCrossGroupHeaderBand)) return new StiGroupHeaderBandV2Builder();
            if (type == typeof(StiGroupFooterBand) ||
                type == typeof(StiCrossGroupFooterBand)) return new StiGroupFooterBandV2Builder();
            if (type == typeof(StiBand) ||
                type == typeof(StiOverlayBand) ||
                type == typeof(StiPageFooterBand) ||
                type == typeof(StiPageHeaderBand) ||
                type == typeof(StiReportTitleBand) ||
                type == typeof(StiReportSummaryBand) ||
                type == typeof(StiChildBand) ||
                type == typeof(StiEmptyBand)) return new StiBandV2Builder();

            if (type == typeof(StiText) ||
                type == typeof(StiSimpleText) ||
                type == typeof(StiSystemText) ||
                type == typeof(StiContourText) ||
                type == typeof(StiRichText) ||
                type == typeof(StiTableCellRichText) ||
                type == typeof(StiTableCell)) return new StiSimpleTextV2Builder();

            if (type == typeof(StiCrossRowTotal) ||
                type == typeof(StiCrossColumnTotal) ||
                type == typeof(StiCrossSummary) ||
                type == typeof(StiCrossSummaryHeader) ||
                type == typeof(StiCrossTitle) ||
                type == typeof(StiCrossTotal) ||
                type == typeof(StiCrossColumn) ||
                type == typeof(StiCrossRow)) return new StiSimpleTextV2Builder();

            if (type == typeof(StiPointPrimitive) ||
                type == typeof(StiEndPointPrimitive) ||
                type == typeof(StiStartPointPrimitive)) return new StiPointPrimitiveV2Builder();

            if (type == typeof(StiVerticalLinePrimitive) ||
                type == typeof(StiRoundedRectanglePrimitive) ||
                type == typeof(StiRectanglePrimitive)) return new StiCrossLinePrimitiveV2Builder();

            if (type == typeof(StiMap)) return new StiMapV2Builder();

            if (type == typeof(StiCheckBox) ||
                type == typeof(StiShape) ||
                type == typeof(StiTableCellCheckBox) ||
                type == typeof(StiHorizontalLinePrimitive) ||
                type == typeof(StiZipCode) ||

                type == typeof(StiBarCode) ||
                type == typeof(StiComponent)) return new StiComponentV2Builder();
            #endregion

            #region Create Builder in Full Trust
            if (!StiOptions.Engine.FullTrust)
            {
                if (type == typeof(StiChart) || type.IsSubclassOf(typeof(StiChart)))
                    return new StiChartV2Builder();

                if (type == typeof(StiClone) || type.IsSubclassOf(typeof(StiClone)))
                    return new StiCloneV2Builder();

                if (type == typeof(StiColumnFooterBand) || type.IsSubclassOf(typeof(StiColumnFooterBand)))
                    return new StiColumnFooterBandV2Builder();

                if (type == typeof(StiColumnHeaderBand) || type.IsSubclassOf(typeof(StiColumnHeaderBand)))
                    return new StiColumnHeaderBandV2Builder();

                if (type == typeof(StiCrossTab) || type.IsSubclassOf(typeof(StiCrossTab)))
                    return new StiCrossTabV2Builder();

                if (type == typeof(StiHierarchicalBand) || type.IsSubclassOf(typeof(StiHierarchicalBand)))
                    return new StiHierarchicalBandV2Builder();

                if (type == typeof(StiDataBand) || type.IsSubclassOf(typeof(StiDataBand)))
                    return new StiDataBandV2Builder();

                if (type == typeof(StiFooterBand) || type.IsSubclassOf(typeof(StiFooterBand)))
                    return new StiFooterBandV2Builder();

                if (type == typeof(StiGroupFooterBand) || type.IsSubclassOf(typeof(StiGroupFooterBand)))
                    return new StiGroupFooterBandV2Builder();

                if (type == typeof(StiGroupHeaderBand) || type.IsSubclassOf(typeof(StiGroupHeaderBand)))
                    return new StiGroupHeaderBandV2Builder();

                if (type == typeof(StiHeaderBand) || type.IsSubclassOf(typeof(StiHeaderBand)))
                    return new StiHeaderBandV2Builder();

                if (type == typeof(StiImage) || type.IsSubclassOf(typeof(StiImage)))
                    return new StiImageV2Builder();

                if (type == typeof(StiPage) || type.IsSubclassOf(typeof(StiPage)))
                    return new StiPageV2Builder();

                if (type == typeof(StiPointPrimitive) || type.IsSubclassOf(typeof(StiPointPrimitive)))
                    return new StiPointPrimitiveV2Builder();

                if (type == typeof(StiSubReport) || type.IsSubclassOf(typeof(StiSubReport)))
                    return new StiSubReportV2Builder();

                if (type == typeof(StiTextInCells) || type.IsSubclassOf(typeof(StiTextInCells)))
                    return new StiTextInCellsV2Builder();

                if (type == typeof(StiSimpleText) || type.IsSubclassOf(typeof(StiSimpleText)))
                    return new StiSimpleTextV2Builder();

                if (type == typeof(StiWinControl) || type.IsSubclassOf(typeof(StiWinControl)))
                    return new StiWinControlV2Builder();

                if (type == typeof(StiView) || type.IsSubclassOf(typeof(StiView)))
                    return new StiViewV2Builder();

                if (type == typeof(StiBand) || type.IsSubclassOf(typeof(StiBand)))
                    return new StiBandV2Builder();

                if (type == typeof(StiContainer) || type.IsSubclassOf(typeof(StiContainer)))
                    return new StiContainerV2Builder();

                if (type == typeof(StiCrossLinePrimitive) || type.IsSubclassOf(typeof(StiCrossLinePrimitive)))
                    return new StiCrossLinePrimitiveV2Builder();

                if (type == typeof(StiComponent) || type.IsSubclassOf(typeof(StiComponent)))
                    return new StiComponentV2Builder();
            }
            #endregion			  

            #region Get builder from attribute
            var builderAttrs = type.GetCustomAttributes(typeof(StiV2BuilderAttribute), true) as StiV2BuilderAttribute[];
            if (builderAttrs != null && builderAttrs.Length > 0)
                return StiActivator.CreateObject(builderAttrs[0].BuilderTypeName, new object[0]) as StiV2Builder;
            #endregion

            return null;
        }
		#endregion

		#region Methods.Render
		/// <summary>
		/// Sets system variables which are specific for the specified component.
		/// </summary>
		public abstract void SetReportVariables(StiComponent masterComp);

		/// <summary>
		/// Prepares a component for rendering.
		/// </summary>
		public abstract void Prepare(StiComponent masterComp);

		/// <summary>
		/// Clears a component for rendering.
		/// </summary>
		public abstract void UnPrepare(StiComponent masterComp);

		/// <summary>
		/// Renders a component. Does not call events of a component while rendering.
		/// </summary>
		/// <param name="masterComp"></param>
		/// <returns></returns>
		public abstract StiComponent InternalRender(StiComponent masterComp);
		
		/// <summary>
		/// Renders a component with calling an event. A method is used with InternalRender for the component rendering.
		/// </summary>
		/// <param name="masterComp"></param>
		/// <returns></returns>
		public abstract StiComponent Render(StiComponent masterComp);
		#endregion
    }
}
