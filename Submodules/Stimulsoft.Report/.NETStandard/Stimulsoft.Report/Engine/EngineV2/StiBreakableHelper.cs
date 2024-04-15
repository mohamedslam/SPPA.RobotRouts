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
using Stimulsoft.Report.Components;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class that helps to work with the IStiBreakable interface.
    /// </summary>
    internal class StiBreakableHelper
    {
        #region Properties
        internal StiEngine Engine { get; }
        #endregion

        #region Methods
        /// <summary>
        /// If a container can be broken into two parts, then returns true for the. 
        /// True is returned, if the container realizes IStiBreakable interface and CanBreak is true,
        /// or ParentBand realizes IStiBreakable interface and CanBreak is true.
        /// </summary>
        internal bool IsCanBreak(StiContainer container)
        {
            var breakable = container.ContainerInfoV2.ParentBand as IStiBreakable;
            if (breakable != null && breakable.CanBreak) 
                return true;

            breakable = container;

            return breakable.CanBreak;
        }

        /// <summary>
        /// Returns true, if a container is to be broken and can be broken. A container may need 
        /// in breaking, if it is higher than free space available for output. 
        /// The ability of break is checked with the IsCanBreak method.
        /// </summary>
        internal bool IsNeedBreak(StiContainer container)
        {
            var currentPage = Engine.ContainerForRender as StiPage;
            if (currentPage != null && currentPage.UnlimitedHeight && !currentPage.UnlimitedBreakable)
            {
                //Skip using UnlimitedBreakable property if engine should break band cotnainer with placed crosstab
                if (!(container.ContainerInfoV2?.ParentBand != null &&
                      container.ContainerInfoV2.ParentBand.Components.ToList().Exists(c => c is IStiCrossTab)))
                {
                    return false;
                }
            }

            return container.Height > Engine.FreeSpace && IsCanBreak(container);
        }

        /// <summary>
        /// Breaks the specified container in two parts which are "Continued" and "Breaked", where the "Continued" is a container, 
        /// which should be output on the next page and "Breaked" is a container which will be output on the current page. 
        /// A container passed as the originalContainer method parameter contains the "Breaked" container. This method returns the "Continued" container.
        /// </summary>
        internal StiContainer Break(StiContainer originalContainer)
        {
            var info = originalContainer.ContainerInfoV2;

            //вычисляем, есть ли зазор между компонентами и верхом контейнера.
            //если зазора нет, и в первом разорванном контейнере нет компонентов - то не надо его выводить вообще,
            //чтобы заголовок не выводился над пустым местом внизу страницы
            var minTop = originalContainer.Height;
            foreach (StiComponent comp in originalContainer.Components)
            {
                minTop = Math.Min(minTop, comp.Top);
            }
            var compsCountBeforeBreak = originalContainer.Components.Count;

            var continuedContainer = StiComponentDivider.BreakContainer(Engine.FreeSpace, originalContainer);
            /* Флаг устанавливается для того, чтобы свойство ResetPageNumber имело эффект только 
             * для первого контейнера в серии разрываемых контейнеров бэнда. */
            continuedContainer.ContainerInfoV2.IgnoreResetPageNumber = true;

            var suppressFirstContainer = minTop == 0 && compsCountBeforeBreak > 0 && originalContainer.Components.Count == 0;

            continuedContainer.Name = suppressFirstContainer && info != null && (info.DataSourceRow != null || info.BusinessObjectCurrent != null)
                ? $"Continued_{originalContainer.Name}"
                : "Continued";


            /* Decrease the height of the "Continued" container on the height of the "Breaked" container, 
             * because the height of the "Breaked" container is the same as for the container that can be placed on the current page */
            continuedContainer.Height -= originalContainer.Height;

            originalContainer.Name = !suppressFirstContainer && info != null && (info.DataSourceRow != null || info.BusinessObjectCurrent != null)
                ? $"Breaked_{originalContainer.Name}"
                : "Breaked";

            //We need store original name of a containter which created from the TOC
            if (info.ParentBand is StiTableOfContents)
                originalContainer.Name = info.ParentBand.Name;

            /* Height of the "Breaked" container is the same as height of free space. It is necessary for the "Breaked"
             * container to be placed to the very bottom of a page */
            originalContainer.Height = Engine.FreeSpace;

            if (!suppressFirstContainer)
            {
                /* Increases free space on a page to output the "Breaked" container on the current page without carring out on the next page */
                Engine.FreeSpace += originalContainer.Height;

                /* Output "Breaked" containers on the current page */
                Engine.RenderContainer(originalContainer);

                /* Restore free space on a page */
                Engine.FreeSpace -= originalContainer.Height;
            }

            /* Form a new page to output the "Continued" container */
            Engine.NewDestination(!suppressFirstContainer);

            /* Set the CanGrow to true to increase the size of a part of a container, that is carried over on the next page.
             * It is necessary because the height of all parts of the container is much more than the initial height of the container (true only for text).
             * */
            continuedContainer.CanGrow = true;

            /* Check the container size */
            StiContainerHelper.CheckSize(continuedContainer);

            return continuedContainer;
        }

        /// <summary>
        /// Sets the CanBreak property for all components.
        /// Do not set CanBreak to true for CrossFields and SubReports.
        /// </summary>
        /// <param name="container"></param>
        internal void SetCanBreak(StiContainer container)
        {
            container.CanBreak = true;
            var index = 0;
            while (index < container.Components.Count)
            {
                var comp = container.Components[index];

                var breakable = comp as IStiBreakable;
                if (breakable != null)
                {
                    //fix
                    if (!breakable.CanBreak && Engine?.Report != null)
                    {
                        var tempComp = Engine.Report[comp.Name] as StiComponent;
                        
                        if (tempComp == null && Engine.Report.CalculationMode == StiCalculationMode.Interpretation)                        
                            tempComp = Engine.Report.GetComponentByName(comp.Name);
                        
                        if (tempComp != null && tempComp.Properties == comp.Properties && comp.Properties != null)
                            comp.Properties = comp.Properties.Clone() as StiRepositoryItems;
                    }

                    breakable.CanBreak = true;
                }

                var cont = comp as StiContainer;
                if (cont != null)
                    SetCanBreak(cont);

                index++;
            }
        }

        /// <summary>
        /// Checks the specified container on necessity to use the Breakable interface.
        /// Until a container cannot be placed completely on a page (and it can be broken) it is broken and
        /// output on pages.
        /// </summary>
        internal StiContainer ProcessBreakable(StiContainer container)
        {
            //The CanBreak doesnot work for CrossBands.
            if (Engine.IsCrossBandsMode) 
                return container;

            var isAlreadySetCanBreak = false;
            while (IsNeedBreak(container))
            {
                var heightBeforeBreak = container.Height;

                container = Break(container);

                /* If height of a container before brake is the same as after brake then, 
                 * it is probably cycling occurs. In this case force brake of
                 * components. */
                if (heightBeforeBreak == container.Height)
                {
                    // Second check for cycling. 
                    // If height of a container before brake is the same as after brake on the current page,
                    // then it is definitely cycling. Force brake of components.
                    var continuedContainer = StiComponentDivider.BreakContainer(Engine.FreeSpace, container.Clone() as StiContainer);
                    StiContainerHelper.CheckSize(continuedContainer);

                    if (container.Height == continuedContainer.Height)
                    {
                        if (isAlreadySetCanBreak) break;

                        SetCanBreak(container);
                        isAlreadySetCanBreak = true;
                    }
                }
            }
            return container;
        }
        #endregion

        internal StiBreakableHelper(StiEngine engine)
        {
            this.Engine = engine;
        }
    }
}
