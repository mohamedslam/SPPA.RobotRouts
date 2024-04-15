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

using Stimulsoft.Report.Components;
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class helps to output Footers, which should be output on all pages on the bottom of a page.
    /// </summary>
    internal class StiFootersOnAllPages
    {
        #region Fields
        /// <summary>
        /// A collection of containers which be output on all pages on the bottom of a page.
        /// </summary>
        private Hashtable bands = new Hashtable();
        #endregion

        #region Properties
        private StiEngine Engine { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a container into the collection of containers which will be output on all pages on the bottom of a page.
        /// </summary>
        /// <param name="container">Контейнер для добавления в коллекцию.</param>
        public void Add(StiContainer container)
        {
            bands[container] = container;
        }

        /// <summary>
        /// Returns true, if the specified band is the Footer. The Footer should be output 
        /// on all pages on the bottom of a page.
        /// </summary>
        /// <param name="band">A band for cheking.</param>
        /// <returns></returns>
        public bool CanProcess(StiBand band)
        {
            var footer = band as StiFooterBand;
            return footer != null && footer.PrintOnAllPages;
        }

        /// <summary>
        /// Производит поиск и вывод внизу страницы, всех Footers из указанного контейнера вывода.
        /// Выводятся только те элементы, которые будут найдены до контейнера - маркера.
        /// Остальные элементы будут перенесены позднее на следующию страницу. 
        /// Все выводимые элементы должны быть найдены в составленном ранее списке выводимых на всех 
        /// страницах Footers. При обработке, контейнерам устанавливается новая позиция по вертикали 
        /// внизу страницы.
        /// </summary>
        /// <param name="outContainer">A container in what Footers should be output.</param>
        /// <param name="startIndex">Индекс, начиная с которого, необходимо произвести поиск 
        /// контейнера-маркера. Индекс указывается для того, чтобы не производить повторную обработку 
        /// выведенных ранее колонок на странице.</param>
        /// <param name="markerContainer">Контейнер - маркер, после которого все контейнеры 
        /// будут перенесены на следующию страницу.</param>
        public void Render(StiContainer outContainer, int startIndex, ref StiContainer markerContainer)
        {
            if (outContainer == null || bands.Count == 0) return;

            var containers = new Hashtable();
            var listOfAllContainers = new List<StiContainer>();

            StiDataBand lastDataBand = null;
            var flag = true;
            for (var index = startIndex; index < outContainer.Components.Count; index++)
            {
                var container = outContainer.Components[index] as StiContainer;
                if (container == null) continue;
                if (container.ContainerInfoV2.ParentBand is StiDataBand)
                    lastDataBand = container.ContainerInfoV2.ParentBand as StiDataBand;

                //If the container-marker is found then stop making a list
                if (container == markerContainer)
                    flag = false;

                #region Is Footer On All Pages
                if (flag && bands[container] != null)
                {
                    if (container.ContainerInfoV2.ParentBand is StiFooterBand)
                        ((StiFooterBand) container.ContainerInfoV2.ParentBand).InvokeMoveFooterToBottom();

                    var containerList = containers[container.Name] as ArrayList;
                    if (containerList == null)
                    {
                        containerList = new ArrayList();
                        containers[container.Name] = containerList;
                    }

                    containerList.Add(container);
                    listOfAllContainers.Add(container);
                }
                #endregion

                #region StiFooterMarkerContainer
                if (container is StiFooterMarkerContainer)
                {
                    var containerList = containers[container.Name] as ArrayList;
                    if (containerList != null && containerList.Count > 0)
                    {
                        var footerContainer = containerList[containerList.Count - 1] as StiContainer;
                        var indexOfFooter = outContainer.Components.IndexOf(footerContainer);
                        containerList.RemoveAt(containerList.Count - 1);
                        outContainer.Components.RemoveAt(index);
                        outContainer.Components.Insert(index, footerContainer);
                        outContainer.Components.RemoveAt(indexOfFooter);
                        index--;
                        listOfAllContainers.Remove(footerContainer);

                        var heightOfCorrection = footerContainer.Height;
                        footerContainer.Top = container.Top;
                        Engine.PositionY += footerContainer.Height;

                        /* Бежим от начала созданного списка контейнеров PrintAtBottom до конца обрабатываемого
                         * списка вывода. Коррекция необходима поскольку все PrintAtBottom контейнеры во время
                         * рендеринга не смещали вертикальную позицию вывода. */
                        for (var index2 = index + 1; index2 < outContainer.Components.Count; index2++)
                        {
                            var container2 = outContainer.Components[index2] as StiContainer;
                            if (container2 == null) continue;

                            //If the container-marker is found then stop
                            if (container2 == markerContainer) break;

                            //Check the vertical position of a container
                            container2.Top += heightOfCorrection;
                        }

                        footerContainer.ContainerInfoV2.IsAutoRendered = true;

                        //если markerContainer это StiFooterMarkerContainer, то его надо
                        //заменить на сам footer, чтобы дальнейший алгоритм работы не сломался
                        if (container == markerContainer)
                            markerContainer = footerContainer;
                    }
                }
                #endregion
            }

            /* Output the list of FooterBands which were not output in the above code. 
             * Это бенды DataBand которых еще не завершил печать, поэтому FooterMarker 
             * еще не добавлен в контейнер вывода. */
            lock (((ICollection) listOfAllContainers).SyncRoot)
            {
                foreach (var container in listOfAllContainers)
                {
                    //fix: если с предыдущей страницы перенесены футеры, то они вставляются до первого футера на текущей странице или до первого ReportSummary;
                    //если на текущей странице нет футеров и ReportSummary - они добавляются в конце списка компонентов
                    StiComponent markFooter = null;
                    if (container.ContainerInfoV2.ParentBand is StiFooterBand)
                    {
                        foreach (StiComponent comp in outContainer.Components)
                        {
                            if (comp == container || !(comp is StiContainer) || containers.ContainsKey(comp.Name)) continue;

                            var cont2 = comp as StiContainer;
                            if (cont2.ContainerInfoV2 != null && (cont2.ContainerInfoV2.ParentBand is StiFooterBand || cont2.ContainerInfoV2.ParentBand is StiReportSummaryBand))
                            {
                                if (cont2.ContainerInfoV2.ParentBand is StiReportSummaryBand)
                                {
                                    markFooter = comp;
                                    break;
                                }

                                //fix: вставляем футеры перед футерами последнего датабэнда
                                //учитываем исходный порядок футеров
                                if (lastDataBand != null)
                                {
                                    foreach (StiFooterBand footer in lastDataBand.DataBandInfoV2.FootersOnAllPages)
                                    {
                                        if (footer.Name != comp.Name) continue;

                                        markFooter = comp;
                                        break;
                                    }

                                    foreach (StiFooterBand footer in lastDataBand.DataBandInfoV2.FootersOnLastPage)
                                    {
                                        if (footer.Name != comp.Name) continue;

                                        markFooter = comp;

                                        #region Check footers order
                                        var index1 = container.ContainerInfoV2.ParentBand.Parent.Components.IndexOf(container.ContainerInfoV2.ParentBand);
                                        var index2 = footer.Parent.Components.IndexOf(footer);
                                        if (index1 != -1 && index2 != -1 && index1 > index2) //must be after this footer
                                        {
                                            var index3 = outContainer.Components.IndexOf(comp);
                                            if (index3 != -1)
                                            {
                                                if (index3 == outContainer.Components.Count - 1)
                                                {
                                                    markFooter = null;
                                                    break;
                                                }

                                                markFooter = outContainer.Components[index3 + 1];
                                                continue;
                                            }
                                        }
                                        #endregion

                                        break;
                                    }
                                }
                                else
                                {
                                    markFooter = comp;
                                    break;
                                }
                            }
                        }
                    }

                    if (markFooter != null)
                    {
                        outContainer.Components.Remove(container);
                        var footerIndex = outContainer.Components.IndexOf(markFooter);
                        outContainer.Components.Insert(footerIndex, container);

                        container.Top = markFooter.Top;
                        for (var indexComp = footerIndex + 1; indexComp < outContainer.Components.Count; indexComp++)
                        {
                            outContainer.Components[indexComp].Top += container.Height;
                        }

                        Engine.PositionY += container.Height;
                    }
                    else
                    {
                        outContainer.Components.Remove(container);
                        outContainer.Components.Add(container);

                        container.Top = Engine.PositionY;
                        Engine.PositionY += container.Height;
                    }

                    container.ContainerInfoV2.IsAutoRendered = true; //fix 2009.06.22
                }
            }

            bands.Clear();
        }
        #endregion

        internal StiFootersOnAllPages(StiEngine engine)
        {
            this.Engine = engine;
        }
    }
}
