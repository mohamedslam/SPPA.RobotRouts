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
using System.Collections;
using System.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class hepls to output bands which should be output on all pages.
    /// </summary>
    internal class StiBandsOnAllPages
    {
        #region class BandItem
        private class BandItem
        {
            #region Properties
            /// <summary>
            /// A DataBand to what the band belongs to and what it is nesessary to be output on all pages.
            /// </summary>
            public StiDataBand DataBand { get; }

            /// <summary>
            /// A Band that should be output on all pages.
            /// </summary>
            public StiBand Band { get; }
            #endregion

            public BandItem(StiDataBand dataBand, StiBand band)
            {
                this.DataBand = dataBand;
                this.Band = band;
            }
        }
        #endregion

        #region Fields
        /// <summary>
        /// A collection of containers which should be output on all paged.
        /// </summary>
        private List<BandItem> bands = new List<BandItem>();
        #endregion

        #region Properties
        internal StiEngine Engine { get; }

        /// <summary>
        /// If the property is true then bands rendering on all pages is blocked. The property is used to 
        /// output headers, with the height higher than one page, on all pages.
        /// </summary>
        public bool DenyRendering { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Starts monitoring of this band for rendering OnAllPages.
        /// </summary>
        /// <param name="dataBand">A DataBand to what the band belongs to and what it is nesessary to be output on all pages.</param>
        /// <param name="band">A band what it is nesessary to be output on all pages.</param>
        internal void Add(StiDataBand dataBand, StiBand band)
        {
            bands.Add(new BandItem(dataBand, band));
        }

        /// <summary>
        /// Ends monitoring of this band for rendering OnAllPages. All bands which are dependent on the specified DataBand are removed.
        /// </summary>
        /// <param name="dataBand">A DataBand for removing dependent bands.</param>
        internal void Remove(StiDataBand dataBand)
        {
            var index = 0;
            while (index < bands.Count)
            {
                var item = bands[index];
                if (item.DataBand == dataBand)
                    bands.RemoveAt(index);
                else
                    index++;
            }
        }

        private bool AllowRenderBand(StiBand band)
        {
            if (this.Engine.BandsInProgress.Count == 0) return true;

            var currentBand = this.Engine.BandsInProgress[this.Engine.BandsInProgress.Count - 1];

            /*If a band that, on the current momment, is output on a new page is the same one that the last output band
                     * (which provoked to output of a new page), then skip it.*/
            if (band == currentBand && !(band is StiHierarchicalBand))
                return false;

            /*Если текущий бэнд это GroupHeaderBand и выводимый бэнд является подчиненным бэндом текущего, то 
             не выводим бэнд*/
            if (!(currentBand is StiGroupHeaderBand) || !(band is StiGroupHeaderBand)) return true;

            if (((StiGroupHeaderBand)currentBand).GetDataBand() !=
                ((StiGroupHeaderBand)band).GetDataBand()) return true;

            var bandIndex = band.Parent.Components.IndexOf(band);
            var currentBandIndex = band.Parent.Components.IndexOf(currentBand);

            return bandIndex <= currentBandIndex;
        }

        /// <summary>
        /// Outputs bands, which were previously added to the collection of bands, which are output on all pages, 
        /// on a new page.
        /// </summary>
        internal void Render()
        {
            if (DenyRendering) return;
         
            var storedDenyRendering = this.DenyRendering;
            try
            {
                this.DenyRendering = true;

                Engine.DenyClearPrintOnAllPagesIgnoreList = true;

                lock (((ICollection)bands).SyncRoot)
                foreach (var bandItem in bands)
                {
                    var band = bandItem.Band;
                    
                    if (!AllowRenderBand(band)) continue;

                    var conts = RenderBand(band, true, false);

                    #region If this band is rendered successfully then procces the collection
                    if (conts != null)
                    {
                        lock (((ICollection)conts).SyncRoot)
                        foreach (StiContainer cont in conts)
                        {
                            //Skip all containers which parentband is null (Breaked containers).
                            if (cont.ContainerInfoV2.ParentBand != null)
                            {
                                //Mark a container as automatically rendered
                                cont.ContainerInfoV2.IsAutoRendered = true;

                                //Put this band into the list of ignoring. The list of ignoring
                                //is required to prevent duplication of headers on a new page
                                var needIgnore = !(band is StiHierarchicalBand) || !((StiHierarchicalBand)band).PrintOnAllPages;

                                if (band is StiDataBand)
                                    needIgnore = false;

                                if (needIgnore)
                                    Engine.PrintOnAllPagesIgnoreList[cont.ContainerInfoV2.ParentBand] = cont.ContainerInfoV2.ParentBand;
                            }
                        }
                    }
                    #endregion
                }
            }
            finally
            {
                Engine.DenyClearPrintOnAllPagesIgnoreList = false;
                this.DenyRendering = storedDenyRendering;
            }
        }

        private StiComponentsCollection RenderBand(StiBand band, bool ignorePageBreaks, bool ignoreRenderingEvents)
        {
            if (!(band is StiHierarchicalBand))
                return Engine.RenderBand(band, ignorePageBreaks, ignoreRenderingEvents);

            var comps = new StiComponentsCollection();
            var treeBand = band as StiHierarchicalBand;
            treeBand.SaveState("TreeBandPrintOnAllPages");

            var resPrintOnAllPages = treeBand.PrintOnAllPages;
            treeBand.PrintOnAllPages = false;

            try
            {
                var positions = new List<int>();

                treeBand.Brush = new StiSolidBrush(Color.Green);
                var level = treeBand.DataSource.GetLevel();
                treeBand.Prior();
                while (level >= 0 && (!treeBand.IsBof))
                {
                    var currentLevel = treeBand.DataSource.GetLevel();
                    if (level > currentLevel)
                    {
                        positions.Add(treeBand.Position);
                        level = currentLevel;
                    }

                    treeBand.Prior();
                }

                for (var index = positions.Count - 1; index >= 0; index--)
                {
                    treeBand.Position = positions[index];

                    var comps2 = Engine.RenderBand(treeBand, ignorePageBreaks, ignoreRenderingEvents);
                    lock (((ICollection) comps2).SyncRoot)
                    {
                        foreach (StiComponent comp2 in comps2)
                        {
                            comps.Add(comp2);
                        }
                    }
                }

                treeBand.Brush = new StiSolidBrush(Color.Transparent);
                treeBand.RestoreState("TreeBandPrintOnAllPages");

                return comps;
            }
            finally
            {
                treeBand.PrintOnAllPages = resPrintOnAllPages;
            }
        }

        internal bool IsBandInBandsList(StiBand band)
        {
            if (band == null) return false;

            foreach (var bandItem in bands)
            {
                if (bandItem.Band == band) return true;
            }
            return false;
        }

        internal List<StiBand> GetBandsList()
        {
            var list = new List<StiBand>();
            foreach (var info in bands)
            {
                list.Add(info.Band);
            }
            return list;
        }
        #endregion

        internal StiBandsOnAllPages(StiEngine engine)
        {
            this.Engine = engine;
        }
    }
}
