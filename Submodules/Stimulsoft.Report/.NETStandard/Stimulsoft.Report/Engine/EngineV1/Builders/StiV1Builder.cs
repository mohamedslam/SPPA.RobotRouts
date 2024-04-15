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

using System;
using System.Collections;
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CrossTab;

namespace Stimulsoft.Report.Engine
{
    public abstract class StiV1Builder : StiBuilder
    {
		#region Fields
		private static Hashtable typeToV1Builder = new Hashtable();
        #endregion

		#region Methods.Helper
		public static StiV1Builder GetBuilder(Type componentType)
		{
			StiV1Builder builder = typeToV1Builder[componentType] as StiV1Builder;
			if (builder == null)
			{
                #region Create Builder in Full Trust
                if (!StiOptions.Engine.FullTrust)
                {
                    if (componentType == typeof(StiChart) || componentType.IsSubclassOf(typeof(StiChart)))
                        return new StiChartV1Builder();
                    else if (componentType == typeof(StiClone) || componentType.IsSubclassOf(typeof(StiClone)))
                        return new StiCloneV1Builder();
                    else if (componentType == typeof(StiCrossDataBand) || componentType.IsSubclassOf(typeof(StiCrossDataBand)))
                        return new StiCrossDataBandV1Builder();
                    else if (componentType == typeof(StiCrossTab) || componentType.IsSubclassOf(typeof(StiCrossTab)))
                        return new StiCrossTabV1Builder();
                    else if (componentType == typeof(StiDataBand) || componentType.IsSubclassOf(typeof(StiDataBand)))
                        return new StiDataBandV1Builder();
                    else if (componentType == typeof(StiFooterBand) || componentType.IsSubclassOf(typeof(StiFooterBand)))
                        return new StiFooterBandV1Builder();
                    else if (componentType == typeof(StiGroupFooterBand) || componentType.IsSubclassOf(typeof(StiGroupFooterBand)))
                        return new StiGroupFooterBandV1Builder();
                    else if (componentType == typeof(StiGroupHeaderBand) || componentType.IsSubclassOf(typeof(StiGroupHeaderBand)))
                        return new StiGroupHeaderBandV1Builder();
                    else if (componentType == typeof(StiHeaderBand) || componentType.IsSubclassOf(typeof(StiHeaderBand)))
                        return new StiHeaderBandV1Builder();
                    else if (componentType == typeof(StiHierarchicalBand) || componentType.IsSubclassOf(typeof(StiHierarchicalBand)))
                        return new StiHierarchicalBandV1Builder();
                    else if (componentType == typeof(StiImage) || componentType.IsSubclassOf(typeof(StiImage)))
                        return new StiImageV1Builder();
                    else if (componentType == typeof(StiPage) || componentType.IsSubclassOf(typeof(StiPage)))
                        return new StiPageV1Builder();
                    else if (componentType == typeof(StiPointPrimitive) || componentType.IsSubclassOf(typeof(StiPointPrimitive)))
                        return new StiPointPrimitiveV1Builder();
                    else if (componentType == typeof(StiReportSummaryBand) || componentType.IsSubclassOf(typeof(StiReportSummaryBand)))
                        return new StiReportSummaryBandV1Builder();
                    else if (componentType == typeof(StiReportTitleBand) || componentType.IsSubclassOf(typeof(StiReportTitleBand)))
                        return new StiReportTitleBandV1Builder();
                    else if (componentType == typeof(StiSubReport) || componentType.IsSubclassOf(typeof(StiSubReport)))
                        return new StiSubReportV1Builder();
                    else if (componentType == typeof(StiTextInCells) || componentType.IsSubclassOf(typeof(StiTextInCells)))
                        return new StiTextInCellsV1Builder();
                    else if (componentType == typeof(StiSimpleText) || componentType.IsSubclassOf(typeof(StiSimpleText)))
                        return new StiSimpleTextV1Builder();
                    else if (componentType == typeof(StiWinControl) || componentType.IsSubclassOf(typeof(StiWinControl)))
                        return new StiWinControlV1Builder();
                    else if (componentType == typeof(StiView) || componentType.IsSubclassOf(typeof(StiView)))
                        return new StiViewV1Builder();
                    else if (componentType == typeof(StiBand) || componentType.IsSubclassOf(typeof(StiBand)))
                        return new StiBandV1Builder();
                    else if (componentType == typeof(StiContainer) || componentType.IsSubclassOf(typeof(StiContainer)))
                        return new StiContainerV1Builder();
                    else if (componentType == typeof(StiComponent) || componentType.IsSubclassOf(typeof(StiComponent)))
                        return new StiComponentV1Builder();
                }
                #endregion

                #region Get builder from attribute
				StiV1BuilderAttribute[] builderAttrs = componentType.GetCustomAttributes(typeof(StiV1BuilderAttribute), true) as
					StiV1BuilderAttribute[];

				if (builderAttrs != null && builderAttrs.Length > 0)
				{
					builder = StiActivator.CreateObject(builderAttrs[0].BuilderTypeName, new object[0]) as StiV1Builder;
				}
				if (builder == null) throw new Exception("StiBuilder for '" + componentType.ToString() + "' is not found!");
				typeToV1Builder[componentType] = builder;
                #endregion
            }
			return builder;
		}
		#endregion

		#region Methods.Render
		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent events into consideration and without taking Conditions into consideration.
		/// The rendered component is returned in the renderedComponent.
		/// </summary>
		/// <param name="renderedComponent">A rendered component.</param>
		/// <param name="outContainer">A panel in what rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public abstract bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer);

		/// <summary>
		/// Renders a component in the specified container with taking generation of events into consideration. The rendered component is returned in the renderedComponent argument.
		/// </summary>
		/// <param name="renderedComponent">A component which is being rendered.</param>
		/// <param name="outContainer">A container in which rendering will be done.</param>
		/// <returns>A value which indicates whether rendering of the component is finished or not.</returns>
		public abstract bool Render(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer);

		/// <summary>
		/// Renders a component in the specified container with taking generation of events into consideration.
		/// </summary>
		/// <param name="outContainer">A Panel in what rendering will be done.</param>
		/// <returns>A value that indicates whether rendering is finished or not.</returns>
		public abstract bool Render(StiComponent masterComp, StiContainer outContainer);

		/// <summary>
		/// Prepares a component for rendering.
		/// </summary>
		public abstract void Prepare(StiComponent masterComp);

		/// <summary>
		/// Clears a component after rendering.
		/// </summary>
		public abstract void UnPrepare(StiComponent masterComp);
		#endregion
    }
}
