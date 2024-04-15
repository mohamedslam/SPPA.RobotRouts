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
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Report.Engine
{
    public class StiDataBandV2Builder : StiBandV2Builder
    {
        #region Methods.Helper.Find Components
        public void FindHeaders(StiDataBand masterDataBand)
        {
            if (masterDataBand == null) return;
            if (masterDataBand.Parent == null) return;
            if (masterDataBand.DataBandInfoV2 == null) return;

            masterDataBand.DataBandInfoV2.Headers = new StiComponentsCollection();
            var index = masterDataBand.Parent.Components.IndexOf(masterDataBand) - 1;

            while (index >= 0)
            {
                var comp = masterDataBand.Parent.Components[index];
                if (!(comp is StiGroupHeaderBand))
                {
                    if (comp is StiChildBand) { }
                    else if (comp is StiEmptyBand) { }
                    else if (comp is StiHeaderBand)
                        masterDataBand.DataBandInfoV2.Headers.Insert(0, comp);
                    else
                        break;
                }
                index--;
            }
        }

        public void FindHierarchicalHeaders(StiDataBand masterDataBand)
        {
            if (masterDataBand == null) return;
            if (masterDataBand.Parent == null) return;
            if (masterDataBand.DataBandInfoV2 == null) return;

            var band = masterDataBand as StiHierarchicalBand;
            if (band == null || band.Headers.Trim().Length <= 0) return;

            masterDataBand.DataBandInfoV2.HierarchicalHeaders = new StiComponentsCollection();

            var headers = band.Headers.Split(';');
            var headersHash = new Hashtable();
            lock (headers.SyncRoot)
            {
                foreach (var header in headers)
                {
                    var str = header.Trim();
                    headersHash[str] = str;
                }
            }

            lock (((ICollection)masterDataBand.DataBandInfoV2.Headers).SyncRoot)
            {
                foreach (StiHeaderBand headerBand in masterDataBand.DataBandInfoV2.Headers)
                {
                    if (headersHash[headerBand.Name] != null)
                        masterDataBand.DataBandInfoV2.HierarchicalHeaders.Add(headerBand);
                }
            }

            lock (((ICollection)masterDataBand.DataBandInfoV2.HierarchicalHeaders).SyncRoot)
            {
                foreach (StiHeaderBand headerBand in masterDataBand.DataBandInfoV2.HierarchicalHeaders)
                {
                    if (masterDataBand.DataBandInfoV2.Headers.Contains(headerBand))
                        masterDataBand.DataBandInfoV2.Headers.Remove(headerBand, false);
                }
            }
        }

        public void FindFooters(StiDataBand masterDataBand)
        {
            if (masterDataBand == null) return;
            if (masterDataBand.Parent == null) return;
            if (masterDataBand.DataBandInfoV2 == null) return;

            masterDataBand.DataBandInfoV2.FootersOnAllPages = new StiComponentsCollection();
            masterDataBand.DataBandInfoV2.FootersOnLastPage = new StiComponentsCollection();

            var index = masterDataBand.Parent.Components.IndexOf(masterDataBand) + 1;

            while (index < masterDataBand.Parent.Components.Count)
            {
                var comp = masterDataBand.Parent.Components[index];
                if (!(comp is StiGroupFooterBand))
                {
                    if (comp is StiChildBand) { }
                    else if (comp is StiEmptyBand) { }
                    else if (comp is StiTable && !(comp as StiTable).IsConverted) { }
                    else if (comp is StiFooterBand)
                    {
                        if (((StiFooterBand)comp).PrintOnAllPages)
                            masterDataBand.DataBandInfoV2.FootersOnAllPages.Add(comp);
                        else
                            masterDataBand.DataBandInfoV2.FootersOnLastPage.Add(comp);
                    }
                    else break;
                }
                index++;
            }
        }

        public void FindHierarchicalFooters(StiDataBand masterDataBand)
        {
            if (masterDataBand == null) return;
            if (masterDataBand.Parent == null) return;
            if (masterDataBand.DataBandInfoV2 == null) return;

            var band = masterDataBand as StiHierarchicalBand;
            if (band == null || band.Footers.Trim().Length <= 0) return;

            masterDataBand.DataBandInfoV2.HierarchicalFooters = new StiComponentsCollection();

            var footers = band.Footers.Split(';');
            var footersHash = new Hashtable();
            lock (footers.SyncRoot)
            {
                foreach (var footer in footers)
                {
                    var str = footer.Trim();
                    footersHash[str] = str;
                }
            }

            lock (((ICollection)masterDataBand.DataBandInfoV2.FootersOnAllPages).SyncRoot)
            {
                foreach (StiFooterBand footerBand in masterDataBand.DataBandInfoV2.FootersOnAllPages)
                {
                    if (footersHash[footerBand.Name] != null)
                        masterDataBand.DataBandInfoV2.HierarchicalFooters.Add(footerBand);
                }
            }

            lock (((ICollection)masterDataBand.DataBandInfoV2.FootersOnLastPage).SyncRoot)
            {
                foreach (StiFooterBand footerBand in masterDataBand.DataBandInfoV2.FootersOnLastPage)
                {
                    if (footersHash[footerBand.Name] != null)
                        masterDataBand.DataBandInfoV2.HierarchicalFooters.Add(footerBand);
                }
            }

            lock (((ICollection)masterDataBand.DataBandInfoV2.HierarchicalFooters).SyncRoot)
            {
                foreach (StiFooterBand footerBand in masterDataBand.DataBandInfoV2.HierarchicalFooters)
                {
                    if (masterDataBand.DataBandInfoV2.FootersOnAllPages.Contains(footerBand))
                        masterDataBand.DataBandInfoV2.FootersOnAllPages.Remove(footerBand, false);

                    if (masterDataBand.DataBandInfoV2.FootersOnLastPage.Contains(footerBand))
                        masterDataBand.DataBandInfoV2.FootersOnLastPage.Remove(footerBand, false);
                }
            }
        }

        public void FindEmptyBands(StiDataBand masterDataBand)
        {
            if (masterDataBand == null) return;
            if (masterDataBand.Parent == null) return;
            if (masterDataBand.DataBandInfoV2 == null) return;

            masterDataBand.DataBandInfoV2.EmptyBands = new StiComponentsCollection();

            var index = masterDataBand.Parent.Components.IndexOf(masterDataBand) + 1;

            while (index < masterDataBand.Parent.Components.Count)
            {
                var comp = masterDataBand.Parent.Components[index];
                if (comp is StiEmptyBand && comp.Enabled)
                    masterDataBand.DataBandInfoV2.EmptyBands.Add(comp);

                if (comp is StiTable && !(comp as StiTable).IsConverted) { }
                else if (comp is StiFooterBand ||
                    comp is StiGroupFooterBand ||
                    comp is StiHeaderBand ||
                    comp is StiGroupHeaderBand ||
                    comp is StiDataBand) break;

                index++;
            }
        }

        public void FindGroupHeaders(StiDataBand masterDataBand)
        {
            if (masterDataBand == null) return;
            if (masterDataBand.Parent == null) return;
            if (masterDataBand.DataBandInfoV2 == null) return;

            masterDataBand.DataBandInfoV2.GroupHeaders = new StiComponentsCollection();
            var index = masterDataBand.Parent.Components.IndexOf(masterDataBand) - 1;

            while (index >= 0)
            {
                var comp = masterDataBand.Parent.Components[index];
                if (!(comp is StiHeaderBand))
                {
                    if (comp is StiChildBand) { }
                    else if (comp is StiEmptyBand) { }
                    else if (comp is StiGroupHeaderBand)
                    {
                        masterDataBand.DataBandInfoV2.GroupHeaders.Insert(0, comp);
                    }
                    else break;
                }
                index--;
            }
        }

        public void FindGroupFooters(StiDataBand masterDataBand)
        {
            if (masterDataBand == null) return;
            if (masterDataBand.Parent == null) return;
            if (masterDataBand.DataBandInfoV2 == null) return;

            masterDataBand.DataBandInfoV2.GroupFooters = new StiComponentsCollection();
            var index = masterDataBand.Parent.Components.IndexOf(masterDataBand) + 1;

            while (index < masterDataBand.Parent.Components.Count)
            {
                var comp = masterDataBand.Parent.Components[index];
                if (!(comp is StiFooterBand))
                {
                    if (comp is StiChildBand) { }
                    else if (comp is StiEmptyBand) { }
                    else if (comp is StiTable && !(comp as StiTable).IsConverted) { }
                    else if (comp is StiGroupFooterBand)
                        masterDataBand.DataBandInfoV2.GroupFooters.Add(comp);

                    else
                        break;
                }
                index++;
            }
        }

        public void FindDetailDataBands(StiDataBand masterDataBand, List<StiDataBand> loops = null)
        {
            if (masterDataBand == null) return;
            if (masterDataBand.Parent == null) return;
            if (masterDataBand.DataBandInfoV2 == null) return;

            masterDataBand.DataBandInfoV2.DetailDataBands = new StiComponentsCollection();
            var comps = masterDataBand.Page.GetComponents();
            var masterDataBand2 = masterDataBand;

            #region Process DataSource
            if (masterDataBand.IsBusinessObjectEmpty)
            {
                lock (((ICollection)comps).SyncRoot)
                {
                    foreach (StiComponent comp in comps)
                    {
                        if (comp is StiEmptyBand) continue;
                        if (comp == masterDataBand) continue;

                        if (comp is StiTable && !(comp as StiTable).IsConverted && masterDataBand.Name == comp.Name + "_DB")
                            masterDataBand2 = comp as StiDataBand;

                        var masterComponent = comp as IStiMasterComponent;
                        if (masterComponent != null &&
                            masterComponent is StiDataBand &&
                            (masterComponent.MasterComponent == masterDataBand || masterComponent.MasterComponent == masterDataBand2) &&
                            (((StiDataBand)masterComponent).DataSource != masterDataBand2.DataSource ||
                             ((StiDataBand)masterComponent).DataSource == null && masterDataBand2.DataSource == null))
                        {
                            /*Add only those detail bands which are not nested in the master*/
                            StiComponent parent = comp.Parent;
                            while (parent != null && !(parent is StiPage))
                            {
                                if (parent == masterDataBand) break;

                                var ds = parent as IStiDataSource;
                                if ((ds != null) && (ds.DataSource == ((StiDataBand)masterComponent).DataSource))   //looping
                                {
                                    if (loops != null && parent is StiDataBand)
                                    {
                                        loops.Add(parent as StiDataBand);
                                        loops.Add(masterComponent as StiDataBand);
                                    }
                                    parent = masterDataBand;    //fast exit
                                    break;
                                }

                                parent = parent.Parent;
                            }

                            if (parent != masterDataBand)
                                masterDataBand.DataBandInfoV2.DetailDataBands.Add(comp);
                        }

                        var subReport = comp as StiSubReport;
                        if (subReport != null && subReport.SubReportPage != null)
                        {
                            foreach (StiComponent comp3 in subReport.SubReportPage.GetComponents())
                            {
                                var masterComponent3 = comp3 as IStiMasterComponent;
                                if (masterComponent3 != null && masterComponent3 is StiDataBand &&
                                    (masterComponent3.MasterComponent == masterDataBand || masterComponent3.MasterComponent == masterDataBand2) &&
                                    (((StiDataBand)masterComponent3).DataSource != masterDataBand2.DataSource || ((StiDataBand)masterComponent3).DataSource == null &&
                                     masterDataBand2.DataSource == null))
                                {
                                    if (masterDataBand.DataBandInfoV2.DetailDataBandsFromSubReports == null)
                                        masterDataBand.DataBandInfoV2.DetailDataBandsFromSubReports = new Hashtable();

                                    masterDataBand.DataBandInfoV2.DetailDataBandsFromSubReports[comp3] = null;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Process BusinessObject
            else
            {
                //add components from subreports
                var comps2 = new StiComponentsCollection();
                var comps3 = new StiComponentsCollection();
                foreach (StiComponent comp in comps)
                {
                    var subReport = comp as StiSubReport;
                    if (subReport != null && subReport.SubReportPage != null)
                    {
                        comps2.AddRange(subReport.SubReportPage.GetComponents());
                        comps3.AddRange(subReport.SubReportPage.GetComponents());
                    }
                    else
                        comps2.Add(comp);
                }
                comps = comps2;

                var flag1 = false;
                var details = new Hashtable();
                lock (((ICollection)comps).SyncRoot)
                {
                    foreach (StiComponent comp in comps)
                    {
                        if (comp is StiEmptyBand) continue;
                        if (comp == masterDataBand) continue;
                        if (details.ContainsKey(comp)) continue;

                        var detailBand = comp as StiDataBand;
                        if (detailBand != null && (!detailBand.IsBusinessObjectEmpty))
                        {
                            if (masterDataBand.BusinessObject == detailBand.BusinessObject.ParentBusinessObject)
                            {
                                /*Add only those detail bands which are not nested in the master*/
                                StiComponent parent = comp.Parent;
                                while (parent != null && !(parent is StiPage))
                                {
                                    if (parent == masterDataBand) break;

                                    parent = parent.Parent;
                                }

                                if (parent != masterDataBand)
                                {
                                    masterDataBand.DataBandInfoV2.DetailDataBands.Add(comp);
                                    if (comps3.IndexOf(comp) != -1)
                                    {
                                        if (masterDataBand.DataBandInfoV2.DetailDataBandsFromSubReports == null)
                                            masterDataBand.DataBandInfoV2.DetailDataBandsFromSubReports = new Hashtable();

                                        masterDataBand.DataBandInfoV2.DetailDataBandsFromSubReports[comp] = true;
                                    }
                                }

                                flag1 = true;
                            }

                            if (flag1 && IsParentOrCurrentBO(detailBand.BusinessObject.ParentBusinessObject, masterDataBand.BusinessObject)) break;
                            if (detailBand.DataBandInfoV2.DetailDataBands != null)
                            {
                                foreach (var detBand in detailBand.DataBandInfoV2.DetailDataBands)
                                {
                                    details[detBand] = null;
                                }
                            }
                        }
                        if (detailBand != null && detailBand.IsBusinessObjectEmpty && detailBand.CountData != 0 && detailBand.MasterComponent == masterDataBand)
                        {
                            masterDataBand.DataBandInfoV2.DetailDataBands.Add(comp);
                        }
                    }
                }
            }
            #endregion
        }

        private bool IsParentOrCurrentBO(StiBusinessObject currentBO, StiBusinessObject parentBO)
        {
            while (currentBO != null)
            {
                if (currentBO == parentBO) return false;
                currentBO = currentBO.ParentBusinessObject;
            }
            return true;
        }

        public void FindSubReports(StiDataBand masterDataBand)
        {
            if (masterDataBand == null) return;
            if (masterDataBand.Parent == null) return;
            if (masterDataBand.DataBandInfoV2 == null) return;

            masterDataBand.DataBandInfoV2.SubReports = new StiComponentsCollection();
            var comps = masterDataBand.GetComponents();

            lock (((ICollection)comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    if (comp is StiSubReport) masterDataBand.DataBandInfoV2.SubReports.Add(comp);
                }
            }

            var pos = masterDataBand.Parent.Components.IndexOf(masterDataBand) + 1;
            while (pos < masterDataBand.Parent.Components.Count &&
                   masterDataBand.Parent.Components[pos] is StiChildBand)
            {
                var childBand = masterDataBand.Parent.Components[pos] as StiChildBand;
                var comps2 = childBand.GetComponents();

                lock (((ICollection)comps2).SyncRoot)
                {
                    foreach (StiComponent comp in comps2)
                    {
                        if (comp is StiSubReport)
                            masterDataBand.DataBandInfoV2.SubReports.Add(comp);
                    }
                }

                pos++;
            }
        }

        public void FindDetails(StiDataBand masterDataBand, List<StiDataBand> loops = null)
        {
            if (masterDataBand == null) return;
            if (masterDataBand.Parent == null) return;
            if (masterDataBand.DataBandInfoV2 == null) return;

            masterDataBand.DataBandInfoV2.Details = new StiComponentsCollection();
            var comps = masterDataBand.GetComponents();

            lock (((ICollection)comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    var masterComponent = comp as IStiMasterComponent;
                    if (masterComponent != null && !(comp is StiSubReport))
                    {
                        var needAdd = !(masterComponent is StiDataBand && ((StiDataBand)masterComponent).DataSource == masterDataBand.DataSource &&
                                        masterDataBand.DataSource != null);

                        if (!needAdd)
                        {
                            if (loops != null)
                            {
                                loops.Add(masterDataBand);
                                loops.Add(masterComponent as StiDataBand);
                            }
                            continue;
                        }

                        masterDataBand.DataBandInfoV2.Details.Add(comp);
                    }
                }
            }
        }
        #endregion

        #region Methods.Helper.Reset Components
        public void ResetHeaders(StiDataBand masterDataBand)
        {
            masterDataBand.DataBandInfoV2.Headers = null;
        }

        public void ResetHierarchicalHeaders(StiDataBand masterDataBand)
        {
            masterDataBand.DataBandInfoV2.HierarchicalHeaders = null;
        }

        public void ResetFooters(StiDataBand masterDataBand)
        {
            masterDataBand.DataBandInfoV2.FootersOnAllPages = null;
            masterDataBand.DataBandInfoV2.FootersOnLastPage = null;
        }

        public void ResetHierarchicalFooters(StiDataBand masterDataBand)
        {
            masterDataBand.DataBandInfoV2.HierarchicalFooters = null;
        }

        public void ResetEmptyBands(StiDataBand masterDataBand)
        {
            masterDataBand.DataBandInfoV2.EmptyBands = null;
        }

        public void ResetGroupHeaders(StiDataBand masterDataBand)
        {
            masterDataBand.DataBandInfoV2.GroupHeaders = null;
        }

        public void ResetGroupFooters(StiDataBand masterDataBand)
        {
            masterDataBand.DataBandInfoV2.GroupFooters = null;
        }

        public void ResetDetailDataBands(StiDataBand masterDataBand)
        {
            masterDataBand.DataBandInfoV2.DetailDataBands = null;
        }

        public void ResetDetails(StiDataBand masterDataBand)
        {
            masterDataBand.DataBandInfoV2.Details = null;
        }
        #endregion

        #region Methods.Helper.KeepTogether
        public void AddKeepLevelAtLatestDataBand(StiDataBand masterDataBand)
        {
            masterDataBand.Report.Engine.Threads.SelectThreadFromContainer(masterDataBand);

            if (masterDataBand.Report.Engine.ContainerForRender != null)
                masterDataBand.Report.Engine.AddKeepLevelAtLatestDataBand();
        }

        public void AddKeepLevel(StiDataBand masterDataBand)
        {
            masterDataBand.Report.Engine.Threads.SelectThreadFromContainer(masterDataBand);

            if (masterDataBand.Report.Engine.ContainerForRender != null)
                masterDataBand.Report.Engine.AddLevel();
        }

        public void RemoveKeepLevel(StiDataBand masterDataBand)
        {
            masterDataBand.Report.Engine.Threads.SelectThreadFromContainer(masterDataBand);

            if (masterDataBand.Report.Engine.ContainerForRender != null)
                masterDataBand.Report.Engine.RemoveLevel();
        }

        public void RemoveKeepGroupHeaders(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2 != null && masterDataBand.DataBandInfoV2.GroupHeaders != null)
            {
                var index = 0;
                lock (((ICollection)masterDataBand.DataBandInfoV2.GroupHeaders).SyncRoot)
                {
                    foreach (StiGroupHeaderBand groupHeader in masterDataBand.DataBandInfoV2.GroupHeaders)
                    {
                        if (masterDataBand.DataBandInfoV2.GroupHeaderResults[index] && groupHeader.KeepGroupHeaderTogether)
                            RemoveKeepLevel(masterDataBand);

                        index++;
                    }
                }
            }
        }

        public void RemoveKeepHeaders(StiDataBand masterDataBand, bool[] keepHeaders)
        {
            if (masterDataBand.DataBandInfoV2 != null && masterDataBand.DataBandInfoV2.Headers != null)
            {
                lock (((ICollection)masterDataBand.DataBandInfoV2.Headers).SyncRoot)
                {
                    for (var index = 0; index < masterDataBand.DataBandInfoV2.Headers.Count; index++)
                    {
                        var header = masterDataBand.DataBandInfoV2.Headers[index] as StiHeaderBand;
                        if (header.KeepHeaderTogether && (masterDataBand.Position == 0 || keepHeaders[index]))
                        {
                            RemoveKeepLevel(masterDataBand);
                            keepHeaders[index] = false;
                        }
                    }
                }
            }
        }

        public bool AllowKeepDetails(StiDataBand masterDataBand)
        {
            if (!AllowDetailDataBands(masterDataBand)) return false;
            if (masterDataBand.KeepDetails == StiKeepDetails.None) return false;

            lock (((ICollection)masterDataBand.DataBandInfoV2.DetailDataBands).SyncRoot)
            {
                foreach (StiComponent detailDataBand in masterDataBand.DataBandInfoV2.DetailDataBands)
                {
                    if (detailDataBand.Parent.Name != masterDataBand.Parent.Name)
                        return false;
                }
            }

            return true;
        }

        public void AddKeepDetails(StiDataBand masterDataBand)
        {
            if (!AllowKeepDetails(masterDataBand)) return;

            if (masterDataBand.KeepDetails == StiKeepDetails.KeepFirstDetailTogether || masterDataBand.KeepDetails == StiKeepDetails.KeepFirstRowTogether)
            {
                var band = masterDataBand.Report.Engine.KeepFirstDetailTogetherTablesList[masterDataBand] as StiDataBand;
                if (band == null)
                    band = masterDataBand;

                masterDataBand.Report.Engine.KeepFirstDetailTogetherList[band] = masterDataBand;
            }
            AddKeepLevel(masterDataBand);
        }

        public void RemoveKeepDetails(StiDataBand masterDataBand)
        {
            if (AllowKeepDetails(masterDataBand) && masterDataBand.KeepDetails == StiKeepDetails.KeepDetailsTogether)
                RemoveKeepLevel(masterDataBand);

            if (masterDataBand.MasterComponent != null && masterDataBand.Report.Engine.KeepFirstDetailTogetherList[masterDataBand.MasterComponent] != null)
            {
                RemoveKeepLevel(masterDataBand);
                masterDataBand.Report.Engine.KeepFirstDetailTogetherList[masterDataBand.MasterComponent] = null;
            }

            //additional check: remove keep if detailRecords.count==0
            if (masterDataBand.Report.Engine.KeepFirstDetailTogetherList[masterDataBand] != null)
            {
                RemoveKeepLevel(masterDataBand);
                masterDataBand.Report.Engine.KeepFirstDetailTogetherList[masterDataBand] = null;
            }
        }

        public void RemoveKeepDetailsRow(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.DetailDataBands == null || 
                masterDataBand.DataBandInfoV2.DetailDataBands.Count == 0)
                return;

            var band = masterDataBand;
            while (band != null && band.MasterComponent != null)
            {
                band = band.MasterComponent as StiDataBand;

                if (band != null && band.KeepDetails == StiKeepDetails.KeepFirstRowTogether && masterDataBand.Report.Engine.KeepFirstDetailTogetherList[band] != null)
                {
                    RemoveKeepLevel(band);
                    masterDataBand.Report.Engine.KeepFirstDetailTogetherList[band] = null;
                }
            }
        }
        #endregion

        #region Methods.Helper.PrintOnAllPages
        /// <summary>
        /// Starts monitoring of specified collection of bands OnAllPages.
        /// </summary>
        public void StartBands(StiDataBand masterDataBand, StiComponentsCollection bands)
        {
            lock (((ICollection)bands).SyncRoot)
            {
                foreach (StiBand band in bands)
                {
                    StartBand(masterDataBand, band);
                }
            }
        }

        /// <summary>
        /// Starts monitoring of specified band OnAllPages.
        /// </summary>
        public void StartBand(StiDataBand masterDataBand, StiBand band)
        {
            var print = band as IStiPrintOnAllPages;
            if (print != null && print.PrintOnAllPages)
                masterDataBand.Report.Engine.BandsOnAllPages.Add(masterDataBand, band);
        }

        /// <summary>
        /// Ends monitoring of band OnAllPages.
        /// </summary>
        public void EndBands(StiDataBand masterDataBand)
        {
            masterDataBand.Report.Engine.BandsOnAllPages.Remove(masterDataBand);
        }
        #endregion

        #region Methods.Helper.Groups
        public bool GetGroupHeaderResult(StiDataBand masterDataBand, StiGroupHeaderBand groupHeaderBand)
        {
            var index = 0;
            lock (((ICollection)masterDataBand.DataBandInfoV2.GroupHeaders).SyncRoot)
            {
                foreach (StiGroupHeaderBand header in masterDataBand.DataBandInfoV2.GroupHeaders)
                {
                    if (header == groupHeaderBand)
                        return masterDataBand.DataBandInfoV2.GroupHeaderResults[index];

                    index++;
                }
            }

            return false;
        }

        public bool GetGroupFooterResult(StiDataBand masterDataBand, StiGroupHeaderBand groupHeaderBand)
        {
            var index = 0;
            lock (((ICollection)masterDataBand.DataBandInfoV2.GroupHeaders).SyncRoot)
            {
                foreach (StiGroupHeaderBand header in masterDataBand.DataBandInfoV2.GroupHeaders)
                {
                    if (header == groupHeaderBand)
                        return masterDataBand.DataBandInfoV2.GroupFooterResults[index];

                    index++;
                }
            }

            return false;
        }

        public void LinkGroupHeadersAndGroupFooters(StiDataBand masterDataBand)
        {
            lock (((ICollection)masterDataBand.DataBandInfoV2.GroupHeaders).SyncRoot)
            {
                foreach (StiGroupHeaderBand band in masterDataBand.DataBandInfoV2.GroupHeaders)
                    band.GroupHeaderBandInfoV2.GroupFooter = null;
            }

            for (var footerIndex = 0; footerIndex < masterDataBand.DataBandInfoV2.GroupFooters.Count; footerIndex++)
            {
                var headerIndex = masterDataBand.DataBandInfoV2.GroupHeaders.Count - footerIndex - 1;
                if (headerIndex >= 0)
                {
                    ((StiGroupFooterBand)masterDataBand.DataBandInfoV2.GroupFooters[footerIndex]).GroupFooterBandInfoV2.GroupHeader =
                        (StiGroupHeaderBand)masterDataBand.DataBandInfoV2.GroupHeaders[headerIndex];

                    ((StiGroupHeaderBand)masterDataBand.DataBandInfoV2.GroupHeaders[headerIndex]).GroupHeaderBandInfoV2.GroupFooter =
                        (StiGroupFooterBand)masterDataBand.DataBandInfoV2.GroupFooters[footerIndex];
                }
                else
                    ((StiGroupFooterBand)masterDataBand.DataBandInfoV2.GroupFooters[footerIndex]).GroupFooterBandInfoV2.GroupHeader = null;
            }
        }

        public void ResetLinkGroupHeadersAndGroupFooters(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.GroupHeaders != null)
            {
                lock (((ICollection)masterDataBand.DataBandInfoV2.GroupHeaders).SyncRoot)
                {
                    foreach (StiGroupHeaderBand band in masterDataBand.DataBandInfoV2.GroupHeaders)
                        band.GroupHeaderBandInfoV2.GroupFooter = null;
                }
            }

            if (masterDataBand.DataBandInfoV2.GroupFooters != null)
            {
                lock (((ICollection)masterDataBand.DataBandInfoV2.GroupFooters).SyncRoot)
                {
                    foreach (StiGroupFooterBand band in masterDataBand.DataBandInfoV2.GroupFooters)
                        band.GroupFooterBandInfoV2.GroupHeader = null;
                }
            }
        }

        /// <summary>
        /// Prepares a groups result for each group for the current line.
        /// </summary>
        public static void PrepareGroupResults(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.GroupHeaders == null || masterDataBand.DataBandInfoV2.GroupHeaders.Count == 0) return;

            var index = 0;
            //if (masterDataBand.DataBandInfoV2.GroupHeaders != null)
            //{
                int position = masterDataBand.Position;
                int dataCount = masterDataBand.Count;
                if ((masterDataBand.DataBandInfoV2.GroupHeaderCachedResults == null) || (masterDataBand.DataBandInfoV2.GroupHeaderCachedResults.Length != dataCount))
                {
                    masterDataBand.DataBandInfoV2.GroupHeaderCachedResults = new bool[dataCount][];
                    masterDataBand.DataBandInfoV2.GroupFooterCachedResults = new bool[dataCount][];
                }
                bool needCalc = false;
                if (position < masterDataBand.DataBandInfoV2.GroupHeaderCachedResults.Length &&
                    masterDataBand.DataBandInfoV2.GroupHeaderCachedResults[position] == null)
                {
                    masterDataBand.DataBandInfoV2.GroupHeaderCachedResults[position] = new bool[masterDataBand.DataBandInfoV2.GroupHeaders.Count];
                    masterDataBand.DataBandInfoV2.GroupFooterCachedResults[position] = new bool[masterDataBand.DataBandInfoV2.GroupHeaders.Count];
                    needCalc = true;
                }

                lock (((ICollection)masterDataBand.DataBandInfoV2.GroupHeaders).SyncRoot)
                {
                    foreach (StiGroupHeaderBand header in masterDataBand.DataBandInfoV2.GroupHeaders)
                    {
                        #region Check Group Header result
                        if (position > 0)
                        {
                            if (needCalc)
                            {
                                if (!masterDataBand.IsBusinessObjectEmpty)
                                {
                                    var businessObject = masterDataBand.BusinessObject;
                                    businessObject.SetPrevValue();
                                    var prevValue = StiGroupHeaderBandV2Builder.GetCurrentConditionValue(header);
                                    businessObject.RestoreCurrentValue();
                                    var currentValue = StiGroupHeaderBandV2Builder.GetCurrentConditionValue(header);

                                    masterDataBand.DataBandInfoV2.GroupHeaderResults[index] = !object.Equals(prevValue, currentValue);
                                }
                                else
                                {
                                    masterDataBand.Position = position - 1;
                                    var prevValue = StiGroupHeaderBandV2Builder.GetCurrentConditionValue(header);
                                    masterDataBand.Position = position;
                                    var currentValue = StiGroupHeaderBandV2Builder.GetCurrentConditionValue(header);

                                    masterDataBand.DataBandInfoV2.GroupHeaderResults[index] = !object.Equals(prevValue, currentValue);
                                }

                                masterDataBand.DataBandInfoV2.GroupHeaderCachedResults[position][index] = masterDataBand.DataBandInfoV2.GroupHeaderResults[index];
                            }
                            else
                            {
                                masterDataBand.DataBandInfoV2.GroupHeaderResults[index] = masterDataBand.DataBandInfoV2.GroupHeaderCachedResults[position][index];
                            }
                        }
                        else masterDataBand.DataBandInfoV2.GroupHeaderResults[index] = true;
                        #endregion

                        #region Check Group Footer result
                        if (position < (dataCount - 1))
                        {
                            if (needCalc)
                            {
                                if (!masterDataBand.IsBusinessObjectEmpty)
                                {
                                    var businessObject = masterDataBand.BusinessObject;
                                    businessObject.SetNextValue();
                                    var nextValue = StiGroupHeaderBandV2Builder.GetCurrentConditionValue(header);
                                    businessObject.RestoreCurrentValue();
                                    var currentValue = StiGroupHeaderBandV2Builder.GetCurrentConditionValue(header);
                                    masterDataBand.DataBandInfoV2.GroupFooterResults[index] = !object.Equals(nextValue, currentValue);
                                }
                                else
                                {
                                    masterDataBand.Position = position + 1;
                                    var nextValue = StiGroupHeaderBandV2Builder.GetCurrentConditionValue(header);
                                    masterDataBand.Position = position;
                                    var currentValue = StiGroupHeaderBandV2Builder.GetCurrentConditionValue(header);

                                    masterDataBand.DataBandInfoV2.GroupFooterResults[index] = !object.Equals(nextValue, currentValue);
                                }

                                masterDataBand.DataBandInfoV2.GroupFooterCachedResults[position][index] = masterDataBand.DataBandInfoV2.GroupFooterResults[index];
                            }
                            else
                            {
                                masterDataBand.DataBandInfoV2.GroupFooterResults[index] = masterDataBand.DataBandInfoV2.GroupFooterCachedResults[position][index];
                            }
                        }
                        else masterDataBand.DataBandInfoV2.GroupFooterResults[index] = true;
                        #endregion

                        index++;
                    }
                }
            //}

            #region Set header parent result
            for (var indexHeader = 0; indexHeader < masterDataBand.DataBandInfoV2.GroupHeaderResults.Length; indexHeader++)
            {
                if (masterDataBand.DataBandInfoV2.GroupHeaderResults[indexHeader])
                {
                    for (var index2 = indexHeader + 1; index2 < masterDataBand.DataBandInfoV2.GroupHeaderResults.Length; index2++)
                    {
                        masterDataBand.DataBandInfoV2.GroupHeaderResults[index2] = true;
                    }
                    break;
                }
            }
            #endregion

            #region Set footer parent result
            for (var indexFooter = 0; indexFooter < masterDataBand.DataBandInfoV2.GroupFooterResults.Length; indexFooter++)
            {
                if (masterDataBand.DataBandInfoV2.GroupFooterResults[indexFooter])
                {
                    for (var index2 = indexFooter + 1; index2 < masterDataBand.DataBandInfoV2.GroupFooterResults.Length; index2++)
                    {
                        masterDataBand.DataBandInfoV2.GroupFooterResults[index2] = true;
                    }
                    break;
                }
            }
            #endregion
        }

        /// <summary>
        /// Renders all group headers of this databand.
        /// </summary>
        public void RenderGroupHeaders(StiDataBand masterDataBand)
        {
            masterDataBand.ParentBookmark = masterDataBand.DataBandInfoV2.StoredParentBookmark;
            masterDataBand.ParentPointer = masterDataBand.DataBandInfoV2.StoredParentPointer;

            var isGroupRendered = false;
            var index = 0;
            lock (((ICollection)masterDataBand.DataBandInfoV2.GroupHeaders).SyncRoot)
            {
                foreach (StiGroupHeaderBand groupHeader in masterDataBand.DataBandInfoV2.GroupHeaders)
                {
                    groupHeader.ParentBookmark = masterDataBand.ParentBookmark;
                    bool isNewGuidCreated = groupHeader.DoBookmark();

                    if (groupHeader.ParentBookmark != groupHeader.CurrentBookmark)
                        masterDataBand.ParentBookmark = groupHeader.CurrentBookmark;

                    groupHeader.ParentPointer = masterDataBand.ParentPointer;
                    groupHeader.DoPointer(!isNewGuidCreated);

                    if (groupHeader.ParentPointer != groupHeader.CurrentPointer)
                        masterDataBand.ParentPointer = groupHeader.CurrentPointer;

                    if (masterDataBand.DataBandInfoV2.GroupHeaderResults[index])
                    {
                        #region Reset SkipFirst property flag for first line in child group
                        for (var index2 = index + 1; index2 < masterDataBand.DataBandInfoV2.GroupHeaders.Count; index2++)
                        {
                            masterDataBand.Report.Engine.RemoveBandFromPageBreakSkipList(
                                masterDataBand.DataBandInfoV2.GroupHeaders[index2] as IStiPageBreak);
                        }
                        #endregion

                        if (masterDataBand.Report != null)
                        {
                            var report = masterDataBand.Report;
                            if (report.CacheTotals && report.CachedTotals != null)
                                report.CachedTotals[groupHeader] = null;
                        }

                        groupHeader.GroupHeaderBandInfoV2.SkipKeepGroups = masterDataBand.Report.Engine.IsFirstDataBandOnPage &&
                                                                           masterDataBand.ComponentType == StiComponentType.Master &&
                                                                           groupHeader.Line == 1;

                        if (!groupHeader.GroupHeaderBandInfoV2.SkipKeepGroups && groupHeader.KeepGroupTogether)
                            AddKeepLevel(masterDataBand);

                        if (groupHeader.KeepGroupHeaderTogether)
                            AddKeepLevel(masterDataBand);

                        if (masterDataBand.Line > 1)
                        {
                            masterDataBand.Report.Engine.HashDataBandLastLine[masterDataBand.Name] = masterDataBand.Line - 1;
                        }

                        masterDataBand.Line = 1;
                        groupHeader.InvokeBeginRender();
                        groupHeader.CollapsingIndex = groupHeader.Line;
                        groupHeader.Report.GroupLine = groupHeader.Line;
                        InvokeCollapsedEvent(groupHeader);
                        RenderBand(masterDataBand, groupHeader);
                        isGroupRendered = true;

                        if (IsCollapsed(groupHeader, true))
                        {
                            groupHeader.GroupHeaderBandInfoV2.OldSilentMode = masterDataBand.Report.Engine.SilentMode;
                            masterDataBand.Report.Engine.SilentMode = true;
                            groupHeader.GroupHeaderBandInfoV2.SilentModeEnabled = true;
                        }
                    }

                    index++;
                }
            }

            if (isGroupRendered)
                RenderColumns(masterDataBand);
        }

        /// <summary>
        /// Renders all group footers of this databand.
        /// </summary>
        public void RenderGroupFooters(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.GroupHeaders == null) return;

            for (var index = masterDataBand.DataBandInfoV2.GroupHeaders.Count - 1; index >= 0; index--)
            {
                if (!masterDataBand.DataBandInfoV2.GroupFooterResults[index]) continue;

                var groupHeader = masterDataBand.DataBandInfoV2.GroupHeaders[index] as StiGroupHeaderBand;
                var groupFooter = groupHeader.GroupHeaderBandInfoV2.GroupFooter;

                if (groupFooter != null)
                {
                    var needKeepLevel = groupFooter.KeepGroupFooterTogether;

                    if (needKeepLevel) AddKeepLevelAtLatestDataBand(masterDataBand);

                    #region Interactive
                    if (groupHeader.GroupHeaderBandInfoV2.SilentModeEnabled &&
                        groupHeader.Interaction is StiBandInteraction &&
                        !((StiBandInteraction)groupHeader.Interaction).CollapseGroupFooter)
                    {
                        masterDataBand.Report.Engine.SilentMode = groupHeader.GroupHeaderBandInfoV2.OldSilentMode;
                        groupHeader.GroupHeaderBandInfoV2.SilentModeEnabled = false;
                    }
                    #endregion

                    RenderBand(masterDataBand, groupFooter);

                    #region Interactive
                    if (groupHeader.GroupHeaderBandInfoV2.SilentModeEnabled)
                    {
                        masterDataBand.Report.Engine.SilentMode = groupHeader.GroupHeaderBandInfoV2.OldSilentMode;
                        groupHeader.GroupHeaderBandInfoV2.SilentModeEnabled = false;
                    }
                    #endregion

                    if (needKeepLevel)
                        RemoveKeepLevel(masterDataBand);
                }

                groupHeader.InvokeEndRender();

                if (!groupHeader.GroupHeaderBandInfoV2.SkipKeepGroups && groupHeader.KeepGroupTogether)
                    RemoveKeepLevel(masterDataBand);

                masterDataBand.Report.Engine.PrintOnAllPagesIgnoreList.Remove(groupHeader);

                for (var index2 = index + 1; index2 < masterDataBand.DataBandInfoV2.GroupHeaders.Count; index2++)
                {
                    var groupHeader2 = masterDataBand.DataBandInfoV2.GroupHeaders[index2] as StiGroupHeaderBand;
                    masterDataBand.Report.Engine.PrintOnAllPagesIgnoreList.Remove(groupHeader2);
                }

                groupHeader.Line++;
            }
        }
        #endregion

        #region Methods.Helper.Details
        /// <summary>
        /// Sets detail.
        /// </summary>
        public static void SetDetails(StiDataBand masterDataBand)
        {
            if (!masterDataBand.IsDataSourceEmpty || !masterDataBand.IsBusinessObjectEmpty)
            {
                #region DetailDataBands
                var processed = new Hashtable();
                if (masterDataBand.DataBandInfoV2.DetailDataBands != null)
                {
                    lock (((ICollection)masterDataBand.DataBandInfoV2.DetailDataBands).SyncRoot)
                    {
                        foreach (StiComponent comp in masterDataBand.DataBandInfoV2.DetailDataBands)
                        {
                            masterDataBand.Report.Engine.RemoveBandFromPageBreakSkipList(comp as IStiPageBreak);
                            StiDataHelper.SetData(comp, false);

                            if (comp is StiDataBand && (!((StiDataBand)comp).IsBusinessObjectEmpty))
                            {
                                var businessObject = ((StiDataBand)comp).BusinessObject;
                                processed[businessObject] = businessObject;
                            }
                        }
                    }
                }

                if (!masterDataBand.IsBusinessObjectEmpty)
                {
                    var businessObject = masterDataBand.BusinessObject;
                    foreach (StiBusinessObject child in businessObject.BusinessObjects)
                    {
                        if (processed[child] == null)
                            child.SetDetails();
                    }
                }
                #endregion

                #region Set SubReport
                if (masterDataBand.DataBandInfoV2.SubReports != null)
                {
                    lock (((ICollection)masterDataBand.DataBandInfoV2.SubReports).SyncRoot)
                    {
                        foreach (StiSubReport subReport in masterDataBand.DataBandInfoV2.SubReports)
                        {
                            if (subReport.SubReportPage == null) continue;

                            var comps = subReport.SubReportPage.GetComponents();
                            lock (((ICollection)comps).SyncRoot)
                            {
                                foreach (StiComponent comp in comps)
                                {
                                    var dataBand = comp as StiDataBand;
                                    var theSameDataSource = dataBand != null && dataBand.DataSource == masterDataBand.DataSource;
                                    if (theSameDataSource) continue;

                                    StiDataHelper.SetData(comp, false);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Set Details
                if (masterDataBand.DataBandInfoV2.Details != null)
                {
                    lock (((ICollection)masterDataBand.DataBandInfoV2.Details).SyncRoot)
                    {
                        foreach (StiComponent detail in masterDataBand.DataBandInfoV2.Details)
                        {
                            StiDataHelper.SetData(detail, false);
                        }
                    }
                }
                #endregion
            }

            PrepareGroupResults(masterDataBand);
        }

        public void RenderDetailDataBands(StiDataBand masterDataBand)
        {
            if (AllowDetailDataBands(masterDataBand))
            {
                lock (((ICollection)masterDataBand.DataBandInfoV2.DetailDataBands).SyncRoot)
                {
                    foreach (StiComponent component in masterDataBand.DataBandInfoV2.DetailDataBands)
                    {
                        if (component.Enabled && IsAllow(masterDataBand, component as StiDataBand))
                        {
                            component.ParentBookmark = masterDataBand.CurrentBookmark;
                            component.ParentPointer = masterDataBand.CurrentPointer;

                            var renderMaster = component as IStiRenderMaster;
                            if (renderMaster != null)
                                renderMaster.RenderMaster();
                            else
                                component.Render();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns databand on which specified container in splaced.
        /// </summary>
        /// <param name="cont"></param>
        /// <returns></returns>
        private StiDataBand GetParentDataBand(StiContainer cont)
        {
            var parent = cont.Parent;
            while (parent != null && (!(parent is StiPage)))
            {
                if (parent is StiDataBand)
                    return parent as StiDataBand;

                if (parent is StiChildBand)
                {
                    var band = (parent as StiChildBand).GetMaster();
                    if (band is StiDataBand)
                        return band as StiDataBand;
                }
                parent = parent.Parent;
            }
            return null;
        }

        /// <summary>
        /// Returns true if specified detail DataBand can be printed.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        private bool IsAllow(StiDataBand master, StiDataBand detail)
        {
            if (master.DataBandInfoV2.DetailDataBandsFromSubReports != null && master.DataBandInfoV2.DetailDataBandsFromSubReports[detail] != null) return false;

            if (master.Parent == detail.Parent) return true;

            var masterDataBand = GetParentDataBand(master);
            var detailDataBand = GetParentDataBand(detail);

            return masterDataBand != detailDataBand || masterDataBand == null;
        }

        public bool AllowDetailDataBands(StiDataBand masterDataBand)
        {
            return
                masterDataBand.DataBandInfoV2.DetailDataBands != null && 
                masterDataBand.DataBandInfoV2.DetailDataBands.Count > 0 &&
                masterDataBand.Columns < 2;
        }

        public bool IsDenyDetailsOnFirstPage(StiDataBand masterDataBand)
        {
            return
                masterDataBand.Report.Engine.IsFirstDataBandOnPage &&
                masterDataBand.ComponentType == StiComponentType.Master &&
                masterDataBand.Line == 1;
        }

        /// <summary>
        /// Gets value indicates that all detail components are empty.
        /// </summary>
        public static bool IsDetailDataSourcesEmpty(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.DetailDataBands == null || masterDataBand.DataBandInfoV2.DetailDataBands.Count == 0) return false;

            lock (((ICollection)masterDataBand.DataBandInfoV2.DetailDataBands).SyncRoot)
            {
                foreach (StiComponent comp in masterDataBand.DataBandInfoV2.DetailDataBands)
                {
                    if (comp is IStiDataSource && !((IStiDataSource)comp).IsEmpty) return false;

                    if (comp is IStiBusinessObject && !((IStiBusinessObject)comp).IsEmpty) return false;
                }
            }

            return true;
        }

        public static bool IsPrintIfDetailEmpty(StiDataBand masterDataBand)
        {
            if (masterDataBand.PrintIfDetailEmpty) return true;
            if (IsDetailDataSourcesEmpty(masterDataBand)) return false;

            var isNotEmpty = false;
            var flag = true;
            if (masterDataBand.DataBandInfoV2.DetailDataBands != null)
            {
                foreach (StiDataBand dataBand in masterDataBand.DataBandInfoV2.DetailDataBands)
                {
                    flag = false;
                    if (StiOptions.Engine.PrintIfDetailEmptyNesting)
                    {
                        dataBand.SaveState("CheckPrintIfDetailEmpty");
                        dataBand.First();
                        while (!dataBand.IsEof)
                        {
                            if (IsPrintIfDetailEmpty(dataBand))
                            {
                                isNotEmpty = true;
                                break;
                            }
                            dataBand.Next();
                        }
                        dataBand.RestoreState("CheckPrintIfDetailEmpty");
                    }
                    else
                    {
                        if (IsPrintIfDetailEmpty(dataBand)) isNotEmpty = true;
                    }
                    if (isNotEmpty) break;
                }
            }

            return flag || isNotEmpty;
        }
        #endregion

        #region Methods.Helper.Headers
        public void RenderHeaders(StiDataBand masterDataBand, bool[] keepHeaders)
        {
            lock (((ICollection)masterDataBand.DataBandInfoV2.Headers).SyncRoot)
            {
                for (var index = 0; index < masterDataBand.DataBandInfoV2.Headers.Count; index++)
                {
                    var header = masterDataBand.DataBandInfoV2.Headers[index] as StiHeaderBand;
                    if (!masterDataBand.IsEmpty || header.PrintIfEmpty)
                    {
                        if (!masterDataBand.IsEmpty && header.KeepHeaderTogether)
                        {
                            AddKeepLevel(masterDataBand);
                            keepHeaders[index] = true;
                        }

                        RenderBand(masterDataBand, header);
                    }
                }
            }
        }

        public void RenderHierarchicalHeaders(StiDataBand masterDataBand, bool allowIndent, int level)
        {
            if (masterDataBand.DataBandInfoV2.HierarchicalHeaders == null) return;
            lock (((ICollection)masterDataBand.DataBandInfoV2.HierarchicalHeaders).SyncRoot)
            {
                foreach (StiHeaderBand header in masterDataBand.DataBandInfoV2.HierarchicalHeaders)
                {
                    if (!masterDataBand.IsEmpty || header.PrintIfEmpty)
                    {
                        if (!masterDataBand.IsEmpty && header.KeepHeaderTogether)
                            AddKeepLevel(masterDataBand);

                        try
                        {
                            if (allowIndent)
                                StiHierarchicalBandV2Builder.CreateIndention(masterDataBand as StiHierarchicalBand, header, level);

                            RenderBand(masterDataBand, header);
                        }
                        finally
                        {
                            if (allowIndent)
                                StiHierarchicalBandV2Builder.CreateIndention(masterDataBand as StiHierarchicalBand, header, -level);
                        }
                    }
                }
            }
        }
        #endregion

        #region Methods.Helper.Footers
        /// <summary>
        /// Adds the Footer Marker to the end of a list, if before the list 
        /// FooterBands were output on all pages.
        /// </summary>
        public void AddFooterMarker(StiDataBand masterDataBand, StiFooterBand footerMaster)
        {
            masterDataBand.Report.Engine.Threads.SelectThreadFromContainer(masterDataBand);
            masterDataBand.Report.Engine.AddFooterMarker(footerMaster);
        }

        public void RenderMarkerFootersOnAllPages(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.FootersOnAllPages != null)
            {
                lock (((ICollection)masterDataBand.DataBandInfoV2.FootersOnAllPages).SyncRoot)
                {
                    foreach (StiFooterBand band in masterDataBand.DataBandInfoV2.FootersOnAllPages)
                    {
                        if (!masterDataBand.IsEmpty || band.PrintIfEmpty)
                            AddFooterMarker(masterDataBand, band);
                    }
                }
            }
        }

        public void RenderFootersOnLastPage(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.FootersOnLastPage == null)
                return;

            try
            {
                if (masterDataBand is StiHierarchicalBand)
                    ((StiHierarchicalBand)masterDataBand).HierarchicalBandInfoV2.FinalFooterCalculation = true;

                lock (((ICollection)masterDataBand.DataBandInfoV2.FootersOnLastPage).SyncRoot)
                {
                    foreach (StiFooterBand footer in masterDataBand.DataBandInfoV2.FootersOnLastPage)
                    {
                        if (masterDataBand.IsEmpty && !footer.PrintIfEmpty) continue;

                        var needKeepLevel = !masterDataBand.IsEmpty && footer.KeepFooterTogether;
                        if (needKeepLevel)
                            AddKeepLevelAtLatestDataBand(masterDataBand);

                        #region FooterMarker part1
                        var tempCont = masterDataBand.Report.Engine.ContainerForRender;
                        StiFooterMarkerContainer lastMarker = null;
                        if (tempCont != null && tempCont.Components.Count > 0)
                            lastMarker = tempCont.Components[tempCont.Components.Count - 1] as StiFooterMarkerContainer;
                        #endregion

                        RenderBand(masterDataBand, footer);

                        #region FooterMarker part2
                        StiComponent lastComp = null;
                        if (lastMarker != null)
                            lastComp = tempCont.Components[tempCont.Components.Count - 1];
                        #endregion

                        if (needKeepLevel)
                            RemoveKeepLevel(masterDataBand);

                        #region FooterMarker part3
                        if (lastMarker != null && lastMarker.ContainerInfoV2.ParentBand != null)
                        {
                            var index1 = lastMarker.ContainerInfoV2.ParentBand.Parent.Components.IndexOf(lastMarker.ContainerInfoV2.ParentBand);
                            var index2 = footer.Parent.Components.IndexOf(footer);
                            if (index1 != -1 && index2 != -1 && index1 > index2)
                            {
                                if (masterDataBand.Report.Engine.ContainerForRender == tempCont)
                                {
                                    tempCont.Components.Remove(lastMarker);
                                    tempCont.Components.Add(lastMarker);
                                    lastMarker.Top = lastComp.Bottom;
                                }
                                else
                                {
                                    var cont1 = tempCont.Components[tempCont.Components.Count - 2] as StiContainer;
                                    var cont2 = tempCont.Components[tempCont.Components.Count - 1] as StiContainer;
                                    if (cont1 != null && cont2 != null && cont2.Name == "Breaked")
                                    {
                                        tempCont.Components.Remove(cont1);
                                        tempCont.Components.Add(cont1);
                                        cont2.Top = cont1.Top;
                                        cont1.Top = cont2.Bottom;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            finally
            {
                if (masterDataBand is StiHierarchicalBand)
                    ((StiHierarchicalBand)masterDataBand).HierarchicalBandInfoV2.FinalFooterCalculation = false;
            }
        }

        public void RenderFootersOnAllPages(StiDataBand masterDataBand)
        {
            lock (((ICollection)masterDataBand.DataBandInfoV2.FootersOnAllPages).SyncRoot)
            {
                foreach (StiFooterBand band in masterDataBand.DataBandInfoV2.FootersOnAllPages)
                {
                    if (!masterDataBand.IsEmpty || band.PrintIfEmpty)
                        RenderBand(masterDataBand, band);
                }
            }
        }

        public void RenderHierarchicalFooters(StiDataBand masterDataBand, bool allowIndent, int level)
        {
            if (masterDataBand.DataBandInfoV2.HierarchicalFooters == null) return;
            lock (((ICollection)masterDataBand.DataBandInfoV2.HierarchicalFooters).SyncRoot)
            {
                foreach (StiFooterBand footer in masterDataBand.DataBandInfoV2.HierarchicalFooters)
                {
                    if (masterDataBand.IsEmpty && !footer.PrintIfEmpty) continue;

                    var needKeepLevel = !masterDataBand.IsEmpty && footer.KeepFooterTogether;
                    if (needKeepLevel)
                        AddKeepLevelAtLatestDataBand(masterDataBand);

                    try
                    {
                        if (allowIndent)
                            StiHierarchicalBandV2Builder.CreateIndention(masterDataBand as StiHierarchicalBand, footer, level);

                        RenderBand(masterDataBand, footer);
                    }
                    finally
                    {
                        if (allowIndent)
                            StiHierarchicalBandV2Builder.CreateIndention(masterDataBand as StiHierarchicalBand, footer, -level);
                    }

                    if (needKeepLevel)
                        RemoveKeepLevel(masterDataBand);
                }
            }
        }
        #endregion

        #region Methods.Helper.ReportTitles
        public void RenderReportTitles(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.ReportTitles == null) return;
            lock (((ICollection)masterDataBand.DataBandInfoV2.ReportTitles).SyncRoot)
            {
                foreach (var title in masterDataBand.DataBandInfoV2.ReportTitles)
                {
                    if (!masterDataBand.IsEmpty || title.PrintIfEmpty)
                        RenderBand(masterDataBand, title);
                }
            }
        }
        #endregion

        #region Methods.Helper.ReportSummaries
        public void RenderReportSummaries(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.ReportSummaries == null) return;
            lock (((ICollection)masterDataBand.DataBandInfoV2.ReportSummaries).SyncRoot)
            {
                foreach (var summary in masterDataBand.DataBandInfoV2.ReportSummaries)
                {
                    if (masterDataBand.IsEmpty && !summary.PrintIfEmpty) continue;

                    var needKeepLevel = !masterDataBand.IsEmpty && summary.KeepReportSummaryTogether;
                    if (needKeepLevel)
                        AddKeepLevelAtLatestDataBand(masterDataBand);

                    RenderBand(masterDataBand, summary);

                    if (needKeepLevel)
                        RemoveKeepLevel(masterDataBand);
                }
            }
        }

        public bool CheckKeepReportSummaryTogether(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.ReportSummaries == null) return false;
            var keepReportSummaryTogether = false;
            lock (((ICollection)masterDataBand.DataBandInfoV2.ReportSummaries).SyncRoot)
            {
                foreach (var summary in masterDataBand.DataBandInfoV2.ReportSummaries)
                {
                    if (!masterDataBand.IsEmpty && summary.KeepReportSummaryTogether)
                        keepReportSummaryTogether = true;
                }
            }

            return keepReportSummaryTogether;
        }
        #endregion

        #region Methods.Helper
        public void Block(StiDataBand masterDataBand)
        {
            lock (((ICollection)masterDataBand.DataBandInfoV2.GroupHeaders).SyncRoot)
                foreach (StiDynamicBand band in masterDataBand.DataBandInfoV2.GroupHeaders)
                    band.Blocked = true;

            lock (((ICollection)masterDataBand.DataBandInfoV2.GroupFooters).SyncRoot)
                foreach (StiDynamicBand band in masterDataBand.DataBandInfoV2.GroupFooters)
                    band.Blocked = true;

            lock (((ICollection)masterDataBand.DataBandInfoV2.Headers).SyncRoot)
                foreach (StiDynamicBand band in masterDataBand.DataBandInfoV2.Headers)
                    band.Blocked = true;

            lock (((ICollection)masterDataBand.DataBandInfoV2.FootersOnAllPages).SyncRoot)
                foreach (StiDynamicBand band in masterDataBand.DataBandInfoV2.FootersOnAllPages)
                    band.Blocked = true;

            lock (((ICollection)masterDataBand.DataBandInfoV2.FootersOnLastPage).SyncRoot)
                foreach (StiDynamicBand band in masterDataBand.DataBandInfoV2.FootersOnLastPage)
                    band.Blocked = true;

            if (masterDataBand.DataBandInfoV2.ReportSummaries != null)
            {
                lock (((ICollection)masterDataBand.DataBandInfoV2.ReportSummaries).SyncRoot)
                    foreach (var band in masterDataBand.DataBandInfoV2.ReportSummaries)
                        band.Blocked = true;
            }

            masterDataBand.Blocked = true;
        }

        public void UnBlock(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.GroupHeaders == null ||
                masterDataBand.DataBandInfoV2.GroupFooters == null ||
                masterDataBand.DataBandInfoV2.Headers == null ||
                masterDataBand.DataBandInfoV2.FootersOnAllPages == null ||
                masterDataBand.DataBandInfoV2.FootersOnLastPage == null)
                return;

            lock (((ICollection)masterDataBand.DataBandInfoV2.GroupHeaders).SyncRoot)
                foreach (StiDynamicBand band in masterDataBand.DataBandInfoV2.GroupHeaders)
                    band.Blocked = false;

            lock (((ICollection)masterDataBand.DataBandInfoV2.GroupFooters).SyncRoot)
                foreach (StiDynamicBand band in masterDataBand.DataBandInfoV2.GroupFooters)
                    band.Blocked = false;

            lock (((ICollection)masterDataBand.DataBandInfoV2.Headers).SyncRoot)
                foreach (StiDynamicBand band in masterDataBand.DataBandInfoV2.Headers)
                    band.Blocked = false;

            lock (((ICollection)masterDataBand.DataBandInfoV2.FootersOnAllPages).SyncRoot)
                foreach (StiDynamicBand band in masterDataBand.DataBandInfoV2.FootersOnAllPages)
                    band.Blocked = false;

            lock (((ICollection)masterDataBand.DataBandInfoV2.FootersOnLastPage).SyncRoot)
                foreach (StiDynamicBand band in masterDataBand.DataBandInfoV2.FootersOnLastPage)
                    band.Blocked = false;

            masterDataBand.Blocked = false;
        }


        public void CheckHierarchicalHeaders(StiDataBand masterDataBand)
        {
            if (!(masterDataBand is StiHierarchicalBand)) return;

            var currentLevel = 0;
            var position = 0;

            if (!masterDataBand.IsDataSourceEmpty)
            {
                currentLevel = masterDataBand.DataSource.GetLevel();
                position = masterDataBand.DataSource.Position;
            }

            if (!masterDataBand.IsBusinessObjectEmpty)
            {
                currentLevel = masterDataBand.BusinessObject.GetLevel();
                position = masterDataBand.BusinessObject.Position;
            }

            if (position == 0)
            {
                for (var index = 0; index <= currentLevel; index++)
                {
                    if (index != currentLevel)
                        ((StiHierarchicalBand)masterDataBand).HierarchicalBandInfoV2.SpecifiedLevel = index;

                    RenderHierarchicalHeaders(masterDataBand, true, index);
                    ((StiHierarchicalBand)masterDataBand).HierarchicalBandInfoV2.SpecifiedLevel = -1;
                }
            }
            else
            {
                var prevLevel = 0;

                if (!masterDataBand.IsDataSourceEmpty)
                {
                    masterDataBand.DataSource.Position--;
                    prevLevel = masterDataBand.DataSource.GetLevel();
                    masterDataBand.DataSource.Position++;
                }

                if (!masterDataBand.IsBusinessObjectEmpty)
                {
                    masterDataBand.BusinessObject.SetPrevValue();
                    prevLevel = masterDataBand.BusinessObject.GetLevel();
                    masterDataBand.BusinessObject.RestoreCurrentValue();
                }

                if (prevLevel < currentLevel)
                {
                    for (var index = currentLevel; index > prevLevel; index--)
                    {
                        if (index != currentLevel)
                            ((StiHierarchicalBand)masterDataBand).HierarchicalBandInfoV2.SpecifiedLevel = index;

                        RenderHierarchicalHeaders(masterDataBand, true, index);
                        ((StiHierarchicalBand)masterDataBand).HierarchicalBandInfoV2.SpecifiedLevel = -1;
                    }
                    ((StiHierarchicalBand)masterDataBand).HierarchicalBandInfoV2.SpecifiedLevel = -1;
                }
            }
        }

        public void CheckHierarchicalFooters(StiDataBand masterDataBand)
        {
            if (!(masterDataBand is StiHierarchicalBand)) return;

            var position = 0;
            var count = 0;
            var currentLevel = 0;

            if (!masterDataBand.IsDataSourceEmpty)
            {
                position = masterDataBand.DataSource.Position;
                count = masterDataBand.DataSource.Count;
                currentLevel = masterDataBand.DataSource.GetLevel();
            }

            if (!masterDataBand.IsBusinessObjectEmpty)
            {
                position = masterDataBand.BusinessObject.Position;
                count = masterDataBand.BusinessObject.Count;
                currentLevel = masterDataBand.BusinessObject.GetLevel();
            }

            if (position == count - 1)
            {
                for (var index = currentLevel; index >= 0; index--)
                {
                    if (index != currentLevel)
                        ((StiHierarchicalBand)masterDataBand).HierarchicalBandInfoV2.SpecifiedLevel = index;

                    RenderHierarchicalFooters(masterDataBand, true, index);
                }
                ((StiHierarchicalBand)masterDataBand).HierarchicalBandInfoV2.SpecifiedLevel = -1;
            }
            else
            {
                var nextLevel = 0;

                if (!masterDataBand.IsDataSourceEmpty)
                {
                    masterDataBand.DataSource.Position++;
                    nextLevel = masterDataBand.DataSource.GetLevel();
                    masterDataBand.DataSource.Position--;
                }

                if (!masterDataBand.IsBusinessObjectEmpty)
                {
                    masterDataBand.BusinessObject.SetNextValue();
                    nextLevel = masterDataBand.BusinessObject.GetLevel();
                    masterDataBand.BusinessObject.RestoreCurrentValue();
                }

                if (nextLevel < currentLevel)
                {
                    for (var index = currentLevel; index > nextLevel; index--)
                    {
                        if (index != currentLevel)
                            ((StiHierarchicalBand)masterDataBand).HierarchicalBandInfoV2.SpecifiedLevel = index;

                        RenderHierarchicalFooters(masterDataBand, true, index);
                    }
                    ((StiHierarchicalBand)masterDataBand).HierarchicalBandInfoV2.SpecifiedLevel = -1;
                }
            }
        }

        public void RenderBand(StiDataBand masterDataBand, StiBand band)
        {
            RenderBand(masterDataBand, band, false, false);
        }

        public void RenderBand(StiDataBand masterDataBand, StiBand band, bool ignorePageBreaks, bool allowRenderingEvents)
        {
            RegisterEmptyBands(masterDataBand);

            if (!(band is StiGroupHeaderBand))
            {
                if (band != masterDataBand)
                {
                    band.ParentBookmark = masterDataBand.ParentBookmark;
                    band.ParentPointer = masterDataBand.ParentPointer;
                }

                bool isNewGuidCreated = false;
                if (band.Enabled || StiOptions.Engine.ShowBookmarkToDisabledBand) isNewGuidCreated = band.DoBookmark();
                band.DoPointer(!isNewGuidCreated);
            }

            masterDataBand.Report.Engine.Threads.SelectThreadFromContainer(band);
            if (masterDataBand.Report.Engine.ContainerForRender != null)
                masterDataBand.Report.Engine.RenderBand(band, ignorePageBreaks, allowRenderingEvents);
        }

        public void RenderColumns(StiDataBand masterDataBand)
        {
            masterDataBand.Report.Engine.Threads.SelectThreadFromContainer(masterDataBand);
            masterDataBand.Report.Engine.ColumnsOnDataBand.RenderColumns(masterDataBand);
        }

        public void RegisterEmptyBands(StiDataBand masterDataBand)
        {
            masterDataBand.Report.Engine.EmptyBands.Register(masterDataBand.DataBandInfoV2.EmptyBands);
        }

        public static bool IsCollapsed(StiContainer masterDataBand, bool isRendering)
        {
            if (StiOptions.Engine.ForceDisableCollapsing) return false;

            if (masterDataBand.Interaction == null ||
                (masterDataBand.Interaction is StiBandInteraction && !((StiBandInteraction)masterDataBand.Interaction).CollapsingEnabled)) return false;

            var isCollapsed = masterDataBand.CollapsedValue is bool && (bool)masterDataBand.CollapsedValue;

            if (masterDataBand.Report.InteractionCollapsingStates == null)
                return isCollapsed;

            Hashtable list = null;
            if (masterDataBand.CollapsingTreePath != null)
                list = masterDataBand.Report.InteractionCollapsingStates[masterDataBand.CollapsingTreePath + masterDataBand.Name] as Hashtable;

            if (list == null)
                list = masterDataBand.Report.InteractionCollapsingStates[masterDataBand.Name] as Hashtable;

            if (list == null)
                return isCollapsed;

            if (!(list[masterDataBand.CollapsingIndex] is bool))
                return isCollapsed;

            return (bool)list[masterDataBand.CollapsingIndex];
        }
        #endregion

        #region Methods.Render
        /// <summary>
        /// Sets system variables which are specific for the specified component.
        /// </summary>
        public override void SetReportVariables(StiComponent masterComp)
        {
            var masterDataBand = masterComp as StiDataBand;

            masterDataBand.Report.Line = masterDataBand.Line;
            masterDataBand.Report.LineThrough = masterDataBand.LineThrough;
        }

        public override void Prepare(StiComponent masterComp)
        {
            base.Prepare(masterComp);

            var masterDataBand = masterComp as StiDataBand;

            #region Init headers, footers, details
            FindHeaders(masterDataBand);
            FindHierarchicalHeaders(masterDataBand);
            FindFooters(masterDataBand);
            FindHierarchicalFooters(masterDataBand);
            FindEmptyBands(masterDataBand);
            FindDetailDataBands(masterDataBand);
            FindDetails(masterDataBand);
            FindSubReports(masterDataBand);
            FindGroupHeaders(masterDataBand);
            FindGroupFooters(masterDataBand);

            masterDataBand.DataBandInfoV2.GroupHeaderResults = new bool[masterDataBand.DataBandInfoV2.GroupHeaders.Count];
            masterDataBand.DataBandInfoV2.GroupFooterResults = new bool[masterDataBand.DataBandInfoV2.GroupHeaders.Count];
            #endregion

            StiFilterHelper.SetFilter(masterComp);

            #region Init OwnerBand property
            if (!masterDataBand.IsBusinessObjectEmpty &&
                (masterDataBand.Sort != null && masterDataBand.Sort.Length > 0 ||
                 masterDataBand.FilterOn && masterDataBand.FilterMethodHandler != null ||
                 masterComp.Report != null && masterComp.Report.CalculationMode == StiCalculationMode.Interpretation && masterDataBand.FilterOn && masterDataBand.Filters.Count > 0 ||
                 masterDataBand is StiHierarchicalBand))
            {
                masterDataBand.BusinessObject.OwnerBand = masterDataBand;
            }
            #endregion

            masterDataBand.LineThrough = 1;
        }

        /// <summary>
        /// Clears a component after rendering.
        /// </summary>
        public override void UnPrepare(StiComponent masterComp)
        {
            base.UnPrepare(masterComp);

            var masterDataBand = masterComp as StiDataBand;

            masterDataBand.DataBandInfoV2.Headers = null;
            masterDataBand.DataBandInfoV2.HierarchicalHeaders = null;
            masterDataBand.DataBandInfoV2.FootersOnAllPages = null;
            masterDataBand.DataBandInfoV2.FootersOnLastPage = null;
            masterDataBand.DataBandInfoV2.HierarchicalFooters = null;
            masterDataBand.DataBandInfoV2.EmptyBands = null;
            masterDataBand.DataBandInfoV2.FootersOnLastPage = null;
            masterDataBand.DataBandInfoV2.DetailDataBands = null;
            masterDataBand.DataBandInfoV2.SubReports = null;
            masterDataBand.DataBandInfoV2.ReportTitles = null;
            masterDataBand.DataBandInfoV2.ReportSummaries = null;
            masterDataBand.DataBandInfoV2.Headers = null;
            masterDataBand.DataBandInfoV2.GroupHeaders = null;
            masterDataBand.DataBandInfoV2.GroupFooters = null;
        }

        private static void InvokeCollapsedEvent(StiContainer masterDataBand)
        {
            var e = new StiValueEventArgs();

            if (masterDataBand is StiGroupHeaderBand)
            {
                ((StiGroupHeaderBand)masterDataBand).InvokeGetCollapsed(e);
                ((StiGroupHeaderBand)masterDataBand).CollapsedValue = e.Value;
            }
            else if (masterDataBand is StiDataBand)
            {
                ((StiDataBand)masterDataBand).InvokeGetCollapsed(e);
                ((StiDataBand)masterDataBand).CollapsedValue = e.Value;
            }
            if (StiOptions.Engine.FixFirstCollapsingBandState && e.Value != null && masterDataBand.Report.InteractionCollapsingStates == null)
            {
                masterDataBand.Report.InteractionCollapsingStates = new Hashtable();
                var list = new Hashtable();
                var collapsingName = masterDataBand.Name;
                var cont = masterDataBand;

                if (cont.CollapsingTreePath != null)
                    collapsingName = cont.CollapsingTreePath + masterDataBand.Name;

                masterDataBand.Report.InteractionCollapsingStates[collapsingName] = list;
                list[masterDataBand.CollapsingIndex] = e.Value;
            }
        }

        public override StiComponent Render(StiComponent masterComp)
        {
            return null;
        }

        public virtual void RenderMaster(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV2.GroupHeaders == null ||
                masterDataBand.DataBandInfoV2.GroupFooters == null ||
                masterDataBand.DataBandInfoV2.Headers == null ||
                masterDataBand.DataBandInfoV2.FootersOnAllPages == null ||
                masterDataBand.DataBandInfoV2.FootersOnLastPage == null)
                return;

            var storedIsCrossBandsMode = masterDataBand.Report.Engine.IsCrossBandsMode;
            var newIsCrossBandsMode = masterDataBand is StiCrossDataBand;

            masterDataBand.Report.Engine.Threads.SelectThreadFromContainer(masterDataBand);
            masterDataBand.Report.Engine.IsCrossBandsMode = newIsCrossBandsMode;
            masterDataBand.DataBandInfoV2.StoredParentBookmark = masterDataBand.ParentBookmark;
            masterDataBand.DataBandInfoV2.StoredParentPointer = masterDataBand.ParentPointer;
            masterDataBand.Report.Engine.IsDynamicBookmarksMode = true;
            masterDataBand.InvokeBeginRender();

            Block(masterDataBand);
            LinkGroupHeadersAndGroupFooters(masterDataBand);
            try
            {
                masterDataBand.Line = 1;
                if (masterDataBand.MasterComponent == null)
                    masterDataBand.LineThrough = 1;

                //set the GroupLine to 1; for case of NumberOfCopies or SubReports
                lock (((ICollection)masterDataBand.DataBandInfoV2.GroupHeaders).SyncRoot)
                {
                    foreach (StiGroupHeaderBand groupHeader in masterDataBand.DataBandInfoV2.GroupHeaders)
                    {
                        groupHeader.Line = 1;
                    }
                }

                #region SetData
                //check for the double call of SetData method
                var needSetData = masterDataBand.MasterComponent == null || !StiOptions.Engine.OptimizeDetailDataFiltering;
                var tempDataBand = masterDataBand.MasterComponent as StiDataBand;
                if (tempDataBand != null && tempDataBand.IsDataSourceEmpty && tempDataBand.IsBusinessObjectEmpty)
                    needSetData = true;

                if (!needSetData)
                {
                    if (masterDataBand.Report.Engine.HashDataSourceReferencesCounter == null)
                        needSetData = true;

                    else
                    {
                        if (!masterDataBand.IsDataSourceEmpty)
                        {
                            var tempCount = masterDataBand.Report.Engine.HashDataSourceReferencesCounter[masterDataBand.DataSourceName];
                            if (tempCount != null && (int)tempCount > 1)
                                needSetData = true;
                        }

                        if (!masterDataBand.IsBusinessObjectEmpty)
                        {
                            var tempCount = masterDataBand.Report.Engine.HashDataSourceReferencesCounter[masterDataBand.BusinessObject.Name];
                            if (tempCount != null && (int)tempCount > 1)
                                needSetData = true;
                        }
                    }
                }

                if (needSetData)
                    StiDataHelper.SetData(masterDataBand, false);
                #endregion

                if (!needSetData || (masterDataBand.BusinessObject != null))
                    masterDataBand.First();

                //fix
                //if (masterDataBand.Columns < 2)
                    masterDataBand.Report.Engine.ColumnsOnDataBand.Enabled = false;

                RenderReportTitles(masterDataBand);

                var keepHeaders = new bool[masterDataBand.DataBandInfoV2.Headers.Count];
                RenderHeaders(masterDataBand, keepHeaders);

                RenderFootersOnAllPages(masterDataBand);

                #region PrintOnAllPages
                StartBands(masterDataBand, masterDataBand.DataBandInfoV2.Headers);
                StartBands(masterDataBand, masterDataBand.DataBandInfoV2.FootersOnAllPages);
                StartBands(masterDataBand, masterDataBand.DataBandInfoV2.GroupHeaders);
                StartBand(masterDataBand, masterDataBand);
                #endregion

                if (masterDataBand.DataBandInfoV2.GroupHeaders.Count == 0) RenderColumns(masterDataBand);
                if (masterDataBand.Interaction is StiBandInteraction && masterDataBand.DataBandInfoV2.DetailDataBands.Count == 0 &&
                    !(masterDataBand is StiHierarchicalBand))
                    ((StiBandInteraction)masterDataBand.Interaction).CollapsingEnabled = false;

                masterDataBand.First();//Сбрасываем второй раз на тот случай если он был промотан в заголовках

                #region Limit rows
                int countOnCurrentContainer = 0;
                object currentContainer = masterDataBand.Report.Engine.ContainerForRender;
                int limitRows = 0;
                try
                {
                    if (!string.IsNullOrWhiteSpace(masterDataBand.LimitRows) && (masterDataBand.Columns < 2))
                    {
                        object limitRowsObj = StiParser.ParseTextValue("{" + masterDataBand.LimitRows + "}", masterDataBand, new StiParserParameters() { ExecuteIfStoreToPrint = true });
                        limitRows = Convert.ToInt32(limitRowsObj);
                    }
                }
                catch (Exception ex)
                {
                    string str = string.Format("Expression in LimitRows property of '{0}' can't be evaluated! {1}", masterDataBand.Name, ex.Message);
                    StiLogService.Write(masterDataBand.GetType(), str);
                    StiLogService.Write(masterDataBand.GetType(), ex.Message);
                    masterDataBand.Report.WriteToReportRenderingMessages(str);
                }
                #endregion

                StiBusinessObject businessObject = masterDataBand.BusinessObject;

                while (!masterDataBand.IsEof)
                {
                    if (limitRows > 0)
                    {
                        if (currentContainer != masterDataBand.Report.Engine.ContainerForRender)
                        {
                            currentContainer = masterDataBand.Report.Engine.ContainerForRender;
                            countOnCurrentContainer = 1;
                        }
                        countOnCurrentContainer++;
                        if (countOnCurrentContainer > limitRows)
                        {
                            masterDataBand.Report.Engine.NewDestination();
                            countOnCurrentContainer = 1;
                        }
                    }

                    //Check for possible looping - part0
                    int oldPosition = masterDataBand.Position;

                    masterDataBand.CollapsingIndex = oldPosition;   //caching
                    if (masterDataBand.Interaction != null && ((StiBandInteraction)masterDataBand.Interaction).CollapsingEnabled)
                    {
                        string collapsingPath = null;
                        var collapsingMaster = masterDataBand;
                        while (collapsingMaster.MasterComponent != null && collapsingMaster.MasterComponent is StiDataBand)
                        {
                            collapsingMaster = collapsingMaster.MasterComponent as StiDataBand;
                            collapsingPath = $"{collapsingMaster.Position}:{collapsingPath}";
                        }

                        if (collapsingPath != null)
                            masterDataBand.CollapsingTreePath = collapsingPath;
                    }

                    //fix
                    if (masterDataBand.Columns < 2)
                        masterDataBand.Report.Engine.ColumnsOnDataBand.Enabled = false;

                    RenderGroupHeaders(masterDataBand);

                    /* Blocks the first group of details holding for the first page should always be filled */
                    var denyDetails = IsDenyDetailsOnFirstPage(masterDataBand);

                    if (!denyDetails)
                        AddKeepDetails(masterDataBand);

                    //Sets report variables. It is necessary for the correct work of Bookmarks.
                    masterDataBand.SetReportVariables();
                    int storedLine = masterDataBand.Report.Line;

                    var flagIsPrintIfDetailEmpty = false;
                    if (IsPrintIfDetailEmpty(masterDataBand))
                    {
                        InvokeCollapsedEvent(masterDataBand);

                        CheckHierarchicalHeaders(masterDataBand);
                        RenderBand(masterDataBand, masterDataBand, false, true);
                        flagIsPrintIfDetailEmpty = true;
                    }
                    if (flagIsPrintIfDetailEmpty)
                        RemoveKeepHeaders(masterDataBand, keepHeaders);

                    var oldSilentMode = masterDataBand.Report.Engine.SilentMode;

                    if (IsCollapsed(masterDataBand, true))
                        masterDataBand.Report.Engine.SilentMode = true;

                    if (!denyDetails)
                        RemoveKeepDetailsRow(masterDataBand);

                    //Check for possible looping - part1
                    //int oldPosition = masterDataBand.Position;

                    RenderDetailDataBands(masterDataBand);

                    masterDataBand.Report.Engine.SilentMode = oldSilentMode;

                    if (flagIsPrintIfDetailEmpty)
                        CheckHierarchicalFooters(masterDataBand);

                    if (!denyDetails)
                        RemoveKeepDetails(masterDataBand);

                    RemoveKeepGroupHeaders(masterDataBand);
                    RenderGroupFooters(masterDataBand);

                    //Check for possible looping - part2
                    if (businessObject != null && !businessObject.isEnumeratorCreated)
                    {
                        businessObject.SetDetails();
                    }
                    int masterPosition = masterDataBand.Position;
                    if (masterPosition < oldPosition)    //possible infinite loop, most often with BusinessObjects
                    {
                        masterDataBand.Position = oldPosition;
                    }

                    if (((masterPosition & 7) == 0) && (masterDataBand.Report.StopBeforeTime > 0) && (Environment.TickCount - masterDataBand.Report.Engine.RenderingStartTicks > masterDataBand.Report.StopBeforeTime * 1000))
                        throw new StiReportRenderingStopException();

                    masterDataBand.Line++;
                    masterDataBand.LineThrough++;
                    masterDataBand.Next();
                }

                if (masterDataBand.Report.Engine.ColumnsOnDataBand.Enabled)
                {
                    StiColumnsContainer columns = masterDataBand.Report.Engine.ColumnsOnDataBand.GetColumns();
                    masterDataBand.Report.Engine.CheckBreakColumnsContainer(columns);
                }

                masterDataBand.Report.Engine.Threads.SelectThreadFromContainer(masterDataBand); //fix

                //Render all Footers which are output on all pages (if there are some pages)
                //It is necessary to do to output ReportSummaryBands after Footers
                //Report.Engine.RenderFootersOnAllPages();

                RenderMarkerFootersOnAllPages(masterDataBand);

                RenderFootersOnLastPage(masterDataBand);

                if (CheckKeepReportSummaryTogether(masterDataBand))
                {
                    RenderReportSummaries(masterDataBand);
                    EndBands(masterDataBand);
                }
                else
                {
                    EndBands(masterDataBand);
                    RenderReportSummaries(masterDataBand);
                }

                if (masterDataBand.Line > 1)
                {
                    masterDataBand.Report.Engine.HashDataBandLastLine[masterDataBand.Name] = masterDataBand.Line - 1;
                }
            }
            finally
            {
                ResetLinkGroupHeadersAndGroupFooters(masterDataBand);
                UnBlock(masterDataBand);
                masterDataBand.Report.Engine.IsCrossBandsMode = storedIsCrossBandsMode;
                masterDataBand.Report.Engine.IsDynamicBookmarksMode = false;
            }
            masterDataBand.InvokeEndRender();
        }
        #endregion
    }
}
