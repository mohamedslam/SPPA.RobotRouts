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

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class helps to output bands which should be output on the bottom of a page.
    /// </summary>
    internal class StiPrintAtBottom
    {
        #region Fields
        /// <summary>
        /// A collection of containers which should be output on the bottom of a page.
        /// </summary>
        private Hashtable bands = new Hashtable();
        #endregion

        #region Properties
        internal StiEngine Engine { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns true if the specified band is a band that should be output 
        /// on the bottom of a page.
        /// </summary>
        /// <param name="band">A band for cheking.</param>
        /// <returns></returns>
        public bool CanProcess(StiBand band)
        {
            var printAtBottom = band as IStiPrintAtBottom;
            return printAtBottom != null && printAtBottom.PrintAtBottom;
        }

        /// <summary>
        /// Adds a collection of containers which will be output on the bottom of a page.
        /// </summary>
        /// <param name="container">A container for adding into the collection.</param>
        public void Add(StiContainer container)
        {
            bands[container] = container;
        }
        
        /// <summary>
        /// Finds all elements from the specified container of output and outputs them on a new page.
        /// Outputs elements which are found before the container-marker.
        /// The rest of elements will be moved on the next page. 
        /// All elements should be found in a previously compiled list of bands which are output on the bottom of a page. 
        /// When processing, a new vertical position on the bottom of a page is set.
        /// </summary>
        /// <param name="outContainer">A container in what bands should be output.</param>
        /// <param name="startIndex">The Index starting with what it is necessary to find 
        /// a container-marker. The Index is specified because there is no need to process again 
        /// previously output clumns on a page.</param>
        /// <param name="markerContainer">A container-marker after what all containers 
        /// will be moved on the next page.</param>
        public void Render(StiContainer outContainer, int startIndex, StiContainer markerContainer)
        {
            if (outContainer == null || bands.Count == 0) return;

            #region Create a list of containers which should be output. Start search from the startIndex.
            /* A collection contains a list of containers which will be output on the bottom of page. If,
             * when collection formation is in process, a container, which PrintAtBottom is not true, will be met 
             * then the collection will be cleared. */
            var listOfContainers = new ArrayList();
            var listStartFrom = -1;
            var markerContainerExist = false;

            for (var index = startIndex; index < outContainer.Components.Count; index++)
            {
                var container = outContainer.Components[index] as StiContainer;

                if (container == null) continue;
                if (container is StiLevelContainer)continue;

                //If the container-marker is found then stop list formation
                if (container == markerContainer)
                    markerContainerExist = true;

                if (!(container.ContainerInfoV2.ParentBand is StiFooterBand && ((StiFooterBand)container.ContainerInfoV2.ParentBand).PrintOnAllPages) && 
                    markerContainerExist) continue;

                //If the container should be output on the bottom of a page, 
                //then add the specified container into the list
                if (bands[container] != null)
                {
                    listOfContainers.Insert(0, container);
                    
                    if (listStartFrom == -1)
                        listStartFrom = index;
                }

                // If a specified container is not PrintAtBottom, then clear lists of output
                else
                {
                    if (listStartFrom != -1 && !(container is StiFooterMarkerContainer))
                    {
                        listOfContainers.Clear();

                        double heightOfCorrection = 0;

                        #region Correct the vertical position of containers
                        for (var index2 = listStartFrom; index2 < outContainer.Components.Count; index2++)
                        {
                            var container2 = outContainer.Components[index2] as StiContainer;
                            
                            //If a container-marker is found then stop
                            if (container2 == markerContainer) break;

                            container2.Top += heightOfCorrection;
                            
                            #region if the PrintAtBottom property is true, then increase height
                            if (bands[container2] != null && index2 < index)
                            {
                                if (!(container2.ContainerInfoV2.ParentBand is StiFooterBand &&
                                    ((StiFooterBand)container2.ContainerInfoV2.ParentBand).PrintOnAllPages))
                                {
                                    heightOfCorrection += container2.Height;
                                    Engine.PositionY += container2.Height;
                                }
                            }
                            #endregion
                        }
                        #endregion

                        listStartFrom = -1;
                    }
                }
            }
            #endregion

            #region Output the ready list on the bottom of a current page
            //listOfFooters.AddRange(listOfContainers);
            foreach (StiContainer container in listOfContainers)
            {
                //StiContainer container = outContainer.Components[index] as StiContainer;
                container.Top = Engine.PositionBottomY - container.Height;
                if (Engine.Page != null && Engine.Page.SegmentPerHeight > 1)
                    container.Top += (Engine.Page.PageHeight - Engine.Page.Margins.Top - Engine.Page.Margins.Bottom) * (Engine.Page.SegmentPerHeight - 1);

                Engine.PositionBottomY -= container.Height;
                if (container.ContainerInfoV2.ParentBand is StiFooterBand &&
                    ((StiFooterBand)container.ContainerInfoV2.ParentBand).PrintOnAllPages)
                    Engine.PositionY -= container.Height;
            }
            #endregion

            bands.Clear();
        }
        #endregion

        internal StiPrintAtBottom(StiEngine engine)
        {
            this.Engine = engine;
        }
    }
}
