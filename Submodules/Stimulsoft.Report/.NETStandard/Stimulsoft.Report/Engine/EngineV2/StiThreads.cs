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

using System.Collections;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
    internal class StiThreads
    {
        #region Properties
        internal bool IsActive { get; set; }

        internal int CurrentPage { get; set; } = -1;

        internal int CurrentColumn { get; set; } = -1;

        internal string DestinationName { get; set; }

        internal StiEngine Engine { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Prepares a new page to output a container. A new page is not always a page
        /// of a report. This be a new container.
        /// </summary>
        internal void NewPage()
        {
            //Zero a container of destination.
            Engine.ContainerForRender = null;

            /* At first, try to find a container with the following number of a column */
            this.CurrentColumn++;
            Engine.ContainerForRender = GetDestinationContainer();

            //If a container is not found then add a new page to a report.
            if (Engine.ContainerForRender == null)
            {
                this.CurrentColumn = 1;//Resets a number of column to 1 because there is a new page
                this.CurrentPage++;//Increase a number of a page by 1.

                /* If there is no pages for printing then generate a new page. */
                if (CurrentPage >= Engine.Report.RenderedPages.Count)
                {
                    /* Use the basic Engine of a report for new page generation. */
                    try
                    {
                        Engine.Report.Engine = Engine.MasterEngine;
                        Engine.Report.Engine.NewPage();
                    }
                    finally
                    {
                        Engine.Report.Engine = this.Engine;
                    }
                }

                Engine.ContainerForRender = GetDestinationContainer();
            }
        }

        /// <summary>
        /// Sets the stream to output the specified component. If the stream does not exist then it is created.
        /// </summary>
        internal void SelectThreadFromContainer(StiContainer container)
        {
            if (Engine.DenyChangeThread) return;

            string name = null;
            StiContainer parent = null;

            parent = container.ContainerInfoV2.ParentBand != null 
                ? container.ContainerInfoV2.ParentBand.Parent 
                : container.Parent;

            name = parent.Name;
            
            if (parent is StiPage)
            {
                if (Engine.MasterEngine != null)
                    Engine.Report.Engine = Engine.MasterEngine;
                return;
            }

            StiEngine masterEngine = Engine.MasterEngine == null ? Engine : Engine.MasterEngine;

            StiEngine containerEngine = masterEngine.SlaveEngines[name] as StiEngine;
            if (containerEngine == null)
            {
                containerEngine =
                    masterEngine.Threads.CreateContainerEngine(
                    name, Engine.Report, masterEngine, masterEngine.Page != null ? masterEngine.Page.PageInfoV2.IndexOfStartRenderedPages : 0);
                masterEngine.SlaveEngines[name] = containerEngine;

                containerEngine.PrintOnAllPagesIgnoreList = masterEngine.PrintOnAllPagesIgnoreList;
            }

            Engine.Report.Engine = containerEngine;
            //containerEngine.ContainerForRender = containerEngine.Threads.GetDestinationContainer();
            
        }

        /// <summary>
        /// Creates a new engine sample to output in the specified container. 
        /// </summary>
        /// <param name="destinationName">A name of a container that will process output.</param>
        /// <param name="report"></param>
        /// <param name="masterEngine">An engine that outputs bands on a page.</param>
        /// <param name="indexOfStartRenderedPages">An index of a page. Starting with it output of the current page began.</param>
        /// <returns></returns>
        internal StiEngine CreateContainerEngine(string destinationName, StiReport report, StiEngine masterEngine, int indexOfStartRenderedPages)
        {
            var containerEngine = new StiEngine(report);

            containerEngine.MasterEngine = masterEngine;
            containerEngine.Threads.IsActive = true;
            containerEngine.Threads.CurrentPage = indexOfStartRenderedPages;
            containerEngine.Threads.CurrentColumn = 1;
            containerEngine.Threads.DestinationName = destinationName;
            containerEngine.TemplatePage = report.Engine.TemplatePage;
            containerEngine.TemplateContainer = containerEngine.Threads.GetTemplateContainer();
            containerEngine.ParserConversionStore = (Hashtable)report.Engine.ParserConversionStore.Clone();

            if (indexOfStartRenderedPages != -1)
            {
                //get container from rendered page
                containerEngine.ContainerForRender = containerEngine.Threads.GetDestinationContainer();
            }
            else
            {
                //get container from subreport
                containerEngine.ContainerForRender = containerEngine.Threads.GetDestinationContainer(
                    masterEngine.ContainerForRender,
                    containerEngine.Threads.DestinationName,
                    containerEngine.Threads.CurrentColumn);
            }

            containerEngine.NewList();

            if (indexOfStartRenderedPages == -1)
                containerEngine.FreeSpace = 100000000000;

            return containerEngine;
        }

        public StiContainer GetTemplateContainer()
        {
            return GetTemplateContainer(Engine.TemplatePage, DestinationName);
        }

        private StiContainer GetTemplateContainer(StiContainer template, string name)
        {
            lock (((ICollection)template.Components).SyncRoot)
            foreach (StiComponent comp in template.Components)
            {
                StiContainer cont = comp as StiContainer;
                if (cont == null) continue;
                if (cont.Name == name) return cont;
                StiContainer findedCont = GetTemplateContainer(cont, name);
                if (findedCont != null) return findedCont;                
            }
            return null;
        }

        public StiContainer GetDestinationContainer()
        {
            StiPage page = null;

            //May be use PrintOnPreviousPage property, so as result we don't have required pages. 
            //Try to search required container on latest rendered page.
            page = Engine.Report.RenderedPages.Count <= CurrentPage 
                ? Engine.Report.RenderedPages[Engine.Report.RenderedPages.Count - 1] 
                : Engine.Report.RenderedPages[CurrentPage];

            return GetDestinationContainer(page, DestinationName, this.CurrentColumn);
        }

        private StiContainer GetDestinationContainer(StiContainer container, string name, int columnNumber)
        {
            lock (((ICollection)container.Components).SyncRoot)
            foreach (StiComponent comp in container.Components)
            {
                StiContainer cont = comp as StiContainer;
                if (cont == null) continue;
                if (cont.Name == name && cont.ContainerInfoV2.RenderStep == columnNumber) return cont;
                
                var findedCont = GetDestinationContainer(cont, name, columnNumber);
                if (findedCont != null) return findedCont;
            }
            return null;
        }
        #endregion

        public StiThreads(StiEngine engine)
        {
            this.Engine = engine;
        }
    }
}
