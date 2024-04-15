#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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
using System.Collections.Generic;
using System.Collections;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Chart;

namespace Stimulsoft.Report.Engine
{
	public sealed class StiRenderProviderV1
	{
		#region Methods.Main
		/// <summary>
		/// Render the report.
		/// </summary>
		/// <param name="report">Report for rendering.</param>
		public static void Render(StiReport report, StiRenderState state)
		{
			InitReport(report);

            if (!CheckDialogsInPreview(report))
            {
                /*Before report rendering is stopped
                connected to data and to fill variables were not empty*/
                ConnectToData(report);
                StiVariableHelper.FillItemsOfVariables(report.CompiledReport != null ? report.CompiledReport : report);
                return;
            }

			StiNumberOfPass numberOfPass = GetNumberOfPass(report);

			bool dialogsOnStartExist = IsDialogsOnStartExist(report);
			if (!dialogsOnStartExist)RenderFirstPass(report, numberOfPass);
						
			StiReport masterReport = report.SubReportsMasterReport;

			try
			{
				Hashtable guidToPage = PrepareSubReportsAndDrillDownPages(report);
				
				if (masterReport == null)report.RenderedPages.Clear();
				
				report.InvokeBeginRender();

				ClearTotals(report);

				ConnectToData(report);
                StiVariableHelper.FillItemsOfVariables(report.CompiledReport != null ? report.CompiledReport : report);

				if (!RenderFormsOnStart(report))return;
				if (dialogsOnStartExist)
				{
					DisconnectFromData(report);
					RenderFirstPass(report, numberOfPass);
					ConnectToData(report);
				}
							
				if (masterReport == null)
				{
					report.PageNumber = 1;
					report.EngineV1.RealPageNumber = 1;
				}
				else 
				{
					report.PageNumber = masterReport.PageNumber;
					report.EngineV1.RealPageNumber = masterReport.EngineV1.RealPageNumber;
				}
			
				#region Prepare Bookmarks
				report.Bookmark.Bookmarks.Clear();
				report.Bookmark.Text = report.ReportAlias;
				#endregion

				if (report.Progress != null)report.Progress.ShowProgressBar();

				InitCacheMode(report);
				RenderReport(report, masterReport, state);//Main cycle
				RenderFormsOnEnd(report);

				report.IsRendered = true;
			}

			#region Processing Errors
			catch (Exception e)
			{
				report.IsStopped = true;
				StiLogService.Write(typeof(StiRenderProviderV1), e);
                if (!StiOptions.Engine.HideExceptions) throw;
			}
			#endregion

			finally
			{   
				foreach (StiPage page in report.Pages)
				{
					page.ProcessPageAfterRender();
				}
                								
				report.CurrentPage = 0;
				report.CurrentPrintPage = 0;
				
				if (masterReport == null && report.ReportPass != StiReportPass.First)
				{
					report.PageNumber = 1;
					report.EngineV1.RealPageNumber = 1;
					if (!report.RenderedPages.CacheMode)
					{
						StiPostProcessProviderV1.PostProcessPages(report.RenderedPages);
					}
					else 
					{
                        if (report.RenderedPages.Count > 0)
                        {
                            StiPage page = report.RenderedPages[report.RenderedPages.Count - 1];
                            StiPostProcessProviderV1.PostProcessPage(page, page.PageInfoV1.PageNumber == 1, true);
                        }
					}
				}

				FinishAllPagesInNotCachedPagesArray(report);
				report.RenderedPages.ClearAllStates();
				RemoveAllPagesLessThenFromPageAndGreaterThenToPage(report, state);

				MadeCollate(report);

				#region Prepare bookmarks
				if (report.ManualBookmark.Bookmarks.Count > 0)
				{
					report.bookmarkValue = report.ManualBookmark;
					report.ManualBookmark = null;
				}

				StiBookmarksV1Helper.PrepareBookmark(report.Bookmark);
				StiBookmarksV1Helper.Pack(report.Bookmark);
				#endregion

                if (StiOptions.Engine.DisconnectFromDataBeforeEndRender) DisconnectFromData(report);

                report.EngineV1.ProgressHelper.AllowCachedPagesCache = report.ReportPass != StiReportPass.First;

				//If report empty, adds one page to report
				if (masterReport == null)
				{
					if (report.RenderedPages.Count == 0)report.RenderedPages.Add(StiActivator.CreateObject("Stimulsoft.Report.Components.StiPage", new object[] { report }) as StiPage);
					
					report.EngineV1.ProcessResetPageNumber(report);
					
					#region Correct Page Count
					if (report.ReportPass != StiReportPass.First && 
						report.EngineV1.ReportTotalPageCounts != null &&
						report.EngineV1.ReportTotalPageCounts.Count > 0 &&
						(!report.RenderedPages.CacheMode))
					{
						int index = 0;
						do
						{
							if (index >= report.EngineV1.ReportTotalPageCounts.Count)break;

							int total1 = (int)report.EngineV1.ReportTotalPageCounts[0];
							int total2 = (int)report.EngineV1.ReportTotalPageCounts[index];

							if (total1 != total2)break;	
							index++;
						}
						while (1 == 1);

						if (index == report.EngineV1.ReportTotalPageCounts.Count)
						{
							int realPageCount = report.GetTotalRenderedPageCount();
							int countOfPages = (int)report.EngineV1.ReportTotalPageCounts[0];

							if (countOfPages != realPageCount)
							{
								for (index = 0; index < report.EngineV1.ReportTotalPageCounts.Count; index ++)
								{
									report.EngineV1.ReportTotalPageCounts[index] = report.RenderedPages.Count;
								}
							}
						}
					}
					#endregion

					report.InvokeEndRender();
					report.EngineV1.PageInProgress = null;
				}

                if (!StiOptions.Engine.DisconnectFromDataBeforeEndRender) DisconnectFromData(report);
				
				#region Finish First & Second pass
				if (report.ReportPass == StiReportPass.Second)
					report.ReportPass = StiReportPass.None;

                FinishProgressForm(report);

				if (masterReport == null)
				{
					ClearTotals(report);

					if (report.ReportPass != StiReportPass.First)
						report.ParentReport = null;   

					report.EngineV1.ReportPageNumbers = null;
					report.EngineV1.ReportTotalPageCounts = null;
				}
				#endregion
			}		
		}
		#endregion

		#region Methods.Data
		private static void ConnectToData(StiReport report)
		{
			report.StatusString = StiLocalization.Get("Report", "ConnectingToData");
           
			report.Dictionary.Connect();
			foreach (StiDataSource dataSource in report.Dictionary.DataSources)
			{
				dataSource.ResetData();
                dataSource.ResetDetailsRows();

                if (StiOptions.Dictionary.EnableConnectOnStartOnSecondPass)
                {
                    StiSqlSource sqlSource = dataSource as StiSqlSource;
                    if (sqlSource != null) sqlSource.ConnectOnStart = true;
                }
			}

			report.Dictionary.ConnectVirtualDataSources();
            report.Dictionary.ConnectCrossTabDataSources();
		    report.Dictionary.ConnectDataTransformations();
            report.Dictionary.RegRelations(true);
		}


		private static void DisconnectFromData(StiReport report)
		{
			report.Dictionary.Disconnect();
		}
		#endregion

		#region Methods.Dialogs
		private static bool IsDialogsOnStartExist(StiReport report)
		{
			foreach (StiPage page in report.Pages)
			{
				IStiForm formControl = page as IStiForm;
				if (formControl != null && 
					formControl.StartMode == StiFormStartMode.OnStart && 
					formControl.Enabled)
				{
					return true;
				}
			}
			return false;
		}


		private static bool RenderFormsOnStart(StiReport report)
		{
            if ((!StiOptions.Configuration.IsWeb) && report.ReportPass != StiReportPass.First)
			{
				report.StatusString = string.Empty;

				StiDialogsProvider provider = StiDialogsProvider.GetProvider(report);
				bool result = provider.Render(report, StiFormStartMode.OnStart);
				if (!result)
				{
					StiLogService.Write(typeof(StiRenderProviderV1), "Report stopped in Dialog Forms");
					report.IsStopped = true;
					return false;
				}
				if (report.Progress != null)
				{					
					if (!report.Progress.IsVisible)report.Progress.Show();
					report.Progress.SetAllowClose(true);
				}
			}

			return true;
		}


		private static bool RenderFormsOnEnd(StiReport report)
		{
            if ((!StiOptions.Configuration.IsWeb) && report.ReportPass != StiReportPass.First)
			{
				report.StatusString = string.Empty;

				StiDialogsProvider provider = StiDialogsProvider.GetProvider(report);

				bool result = provider.Render(report, StiFormStartMode.OnEnd);
				if (!result)
				{
					StiLogService.Write(typeof(StiRenderProviderV1), "Report stopped in Dialog Forms");
					report.IsStopped = true;
					return false;
				}
				if (report.Progress != null)
				{
					if (!report.Progress.IsVisible)report.Progress.Show();
					report.Progress.SetAllowClose(true);
				}
			}
			return true;
		}


        private static bool CheckDialogsInPreview(StiReport report)
        {
            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (variable.RequestFromUser && !report.IsReportRenderingAfterSubmit)
                {
                    if (report.RequestParameters)
                    {
                        report.IsStopped = false;
                        report.IsRendered = true;
                        if (report.ParentReport != null) report.ParentReport.IsRendered = true;
                        return false;
                    }
                }
            }

            foreach (StiPage page in report.Pages)
            {
                StiForm formControl = page as StiForm;
                if (formControl != null &&
                    formControl.StartMode == StiFormStartMode.OnPreview &&
                    (!report.IsReportRenderingAfterSubmit))
                {
                    report.IsStopped = false;
                    report.IsRendered = true;
                    if (report.ParentReport != null) report.ParentReport.IsRendered = true;
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Methods.CacheMode
        private static void ClearPagesWhichLessThenFromPageAndGreaterThenToPage(StiReport report, StiRenderState state)
		{
			if (report.RenderedPages.Count > 1 && report.ReportPass != StiReportPass.First)
			{
				int indexLastPage = report.RenderedPages.Count - 2;
				if ((state.FromPage > 0 && state.FromPage > indexLastPage) || 
					(state.ToPage > 0 && state.ToPage < indexLastPage))
					report.RenderedPages[indexLastPage].Components.Clear();

				if (state.ToPage > 0 && 
					state.ToPage < indexLastPage &&
					state.RenderOnlyPagesFromRange)
					report.IsStopped = true;
			}
		}


		private static void InitCacheMode(StiReport report)
		{
			if (report.ReportCacheMode != StiReportCacheMode.Off)
			{
				report.RenderedPages.CacheMode = report.ReportCacheMode == StiReportCacheMode.On;
				report.RenderedPages.CanUseCacheMode = true;

                //report.ReportCachePath = StiReportCache.CreateNewCache();
                //StiOptions.Engine.GlobalEvents.InvokeReportCacheCreated(report, EventArgs.Empty);
			}
		}


		private static void RemoveAllPagesLessThenFromPageAndGreaterThenToPage(StiReport report, StiRenderState state)
		{
			if (state.DestroyPagesWhichNotInRange && report.ReportPass != StiReportPass.First)
			{
				if (state.ToPage > 0)
				{
					while (state.ToPage < report.RenderedPages.Count - 1)
					{
						report.RenderedPages.RemoveAt(report.RenderedPages.Count - 1);
					}
				}

				int fromPage = state.FromPage;
				while (fromPage > 0 && report.RenderedPages.Count > 0)
				{
					report.RenderedPages.RemoveAt(0);
					fromPage--;
				}
			}
		}


		private static void FinishAllPagesInNotCachedPagesArray(StiReport report)
		{
			if (report.RenderedPages.CacheMode && 
				report.RenderedPages.NotCachedPages != null && 
				report.ReportPass != StiReportPass.First)
			{
				report.RenderedPages.NotCachedPages.Clear();
				report.RenderedPages.NotCachedPages = null;
			}
		}


		private static void ProcessPageToCache(StiReport report, StiPage page)
		{
			if (report.RenderedPages.CacheMode && report.ReportPass != StiReportPass.First)
			{
				if (report.RenderedPages.NotCachedPages == null)
                    report.RenderedPages.NotCachedPages = new List<StiPage>();

				if (report.RenderedPages.NotCachedPages.IndexOf(page) == -1)
					report.RenderedPages.NotCachedPages.Add(page);

				StiPostProcessProviderV1.PostProcessPage(page, page.PageInfoV1.PageNumber == 1, false);
				StiPostProcessProviderV1.PostProcessPrimitives(page);
			}
		}		
		#endregion

		#region Methods.Pass
		private static void RenderFirstPass(StiReport report, StiNumberOfPass numberOfPass)
		{
			if (numberOfPass == StiNumberOfPass.DoublePass)
			{					
				if (report.ReportPass == StiReportPass.None)
				{
					report.ReportPass = StiReportPass.First;
					report.Render();
					report.ReportPass = StiReportPass.Second;
					report.IsRendering = true;
					report.IsRendered = false;

					report.EngineV1.ReportPageNumbers = new Hashtable();
					report.EngineV1.ReportTotalPageCounts = new Hashtable();

					int index = 0;
					foreach (StiPage page2 in report.RenderedPages)
					{
						report.EngineV1.ReportPageNumbers[index] = page2.PageInfoV1.PageNumber;
						report.EngineV1.ReportTotalPageCounts[index] = page2.PageInfoV1.TotalPageCount;
						index++;
					}

					report.RenderedPages.Clear();

					ResetRenderStatus(report);
				}
			}
		}


		private static void ResetRenderStatus(StiReport report)
		{
			StiComponentsCollection comps = report.GetComponents();
			foreach (StiComponent comp in comps)
			{
				comp.PointerValue = null;
				comp.BookmarkValue = null;
				comp.TagValue = null;
				comp.ToolTipValue = null;

				StiText text = comp as StiText;
				if (text != null)text.TextValue = null;
			}
		}


		private static StiNumberOfPass GetNumberOfPass(StiReport report)
		{
			StiNumberOfPass numberOfPass = report.NumberOfPass;
			if (report.ReportPass == StiReportPass.None && numberOfPass == StiNumberOfPass.SinglePass)
			{
				if (!StiOptions.Engine.UseAdvancedPrintOnEngineV1)return StiNumberOfPass.SinglePass;

                StiComponentsCollection comps = report.GetComponents();
				foreach (StiComponent comp in comps)
				{
					if (comp.PrintOn != StiPrintOnType.AllPages)
					{
						return StiNumberOfPass.DoublePass;
					}
				}
			}

			return numberOfPass;
		}


		private static void ClearPagesForFirstPass(StiReport report)
		{
			if (report.ReportPass == StiReportPass.First && report.RenderedPages.Count >= 2)
			{
				report.RenderedPages[report.RenderedPages.Count - 2].Components.Clear();
			}
		}

		#endregion

		#region Methods
		private static void MadeCollate(StiReport report)
		{
			if (report.RenderedPages.Count < 3 || report.Collate < 2)return;

            var list = new List<StiPage>();
            if (StiOptions.Engine.UseCollateOldMode)
            {
                for (int index = 0; index < report.Collate; index++)
                {
                    for (int pageIndex = index; pageIndex < report.RenderedPages.Count; pageIndex += report.Collate)
                    {
                        list.Add(report.RenderedPages[pageIndex]);
                    }
                }
            }
            else
            {
                int range = report.RenderedPages.Count / report.Collate;
                if (range * report.Collate < report.RenderedPages.Count) range++;
                for (int index = 0; index < range; index++)
                {
                    for (int pageIndex = index; pageIndex < report.RenderedPages.Count; pageIndex += range)
                    {
                        list.Add(report.RenderedPages[pageIndex]);
                    }
                }
            }

			report.RenderedPages.Clear();
			foreach (var page in list)
			{
				report.RenderedPages.AddV2Internal(page);
			}
		}


		private static void InitReport(StiReport report)
		{
			report.CurrentPage = 0;
			report.CurrentPrintPage = 0;
			report.EngineV1.RequreResetPageNumber = false;
            report.IsStopped = false;
		}


		private static void ClearTotals(StiReport report)
		{
			report.Totals.Clear();
			if (report.ParentReport != null) report.ParentReport.Totals.Clear();
		}


		private static Hashtable PrepareSubReportsAndDrillDownPages(StiReport report)
		{
			var guidToPage = new Hashtable();
			var comps = report.GetComponents();
			foreach (StiPage page in report.Pages)
			{
				page.Skip = false;
				guidToPage[page.Guid] = page;
			}

			foreach (StiComponent comp in comps)
			{
				var subReport = comp as StiSubReport;
				if (subReport != null && subReport.SubReportPageGuid != null)
				{
					var pg = guidToPage[subReport.SubReportPageGuid] as StiPage;
					if (pg != null)
                        pg.Skip = true;
				}

				if (comp.Interaction != null && comp.Interaction.DrillDownPageGuid != null)
				{
					var pg = guidToPage[comp.Interaction.DrillDownPageGuid] as StiPage;
					if (pg != null)
                        pg.Skip = true;
				}

                if (comp is StiChart)
                {
                    foreach (IStiSeries series in ((StiChart)comp).Series)
                    {
                        if (series.Interaction != null && comp.Interaction != null && comp.Interaction.DrillDownPageGuid != null)
                        {
                            var pg = guidToPage[comp.Interaction.DrillDownPageGuid] as StiPage;
                            if (pg != null)
                                pg.Skip = true;
                            break;
                        }
                    }
                }
			}
			return guidToPage;
		}


		private static void UpdateSystemVariables(StiReport report, StiReport masterReport, StiPage page)
		{
			report.PageNumber += page.SegmentPerWidth * page.SegmentPerHeight;
			report.EngineV1.RealPageNumber += page.SegmentPerWidth * page.SegmentPerHeight;
			report.TotalPageCount = report.PageNumber - 1;

			if (masterReport != null)
			{
				masterReport.PageNumber = report.PageNumber;
				masterReport.EngineV1.RealPageNumber = report.EngineV1.RealPageNumber;
				masterReport.TotalPageCount = report.TotalPageCount;
			}
		}


		private static void RenderReport(StiReport report, StiReport masterReport, StiRenderState state)
		{
			try
			{
                report.PageCopyNumber = 1;
				StiRender render = new StiRender(report);
				do
				{
					//Invoke event Rendering
					report.InvokeRendering();
					//Invoke event Rendering for master report
					if (masterReport != null) masterReport.InvokeRendering();

                    if (report.EngineV1 != null)
                        report.EngineV1.ProgressHelper.Process();

					//Get next report page
					StiPage page = render.GetNextPage();

					//If report stopped then stop report rendering
					if (page == null || report.IsStopped) break;

					if (page != null)
					{
						ProcessPageToCache(report, page);

						report.RenderedPages.Add(page);

						ClearPagesWhichLessThenFromPageAndGreaterThenToPage(report, state);
						ClearPagesForFirstPass(report);
						UpdateSystemVariables(report, masterReport, page);

                        if (report.Progress != null && report.Progress.IsBreaked)
                        {
                            report.IsStopped = true;
                            break;
                        }

						//If report stopped then break report rendering
						if (report.IsStopped) break;

						//Stop page
						if (report.StopBeforePage != 0 && report.StopBeforePage <= report.CurrentPrintPage) break;
					}
				}
				while (1 == 1);
			}
            catch (StiReportRenderingStopException)
            {
            }
            finally
			{
				foreach (StiPage page in report.RenderedPages)
				{
					page.UnPrepare();
				}
			}
		}

		
		private static void FinishProgressForm(StiReport report)
		{			
			if (report.ReportPass != StiReportPass.First)
			{
				report.StatusString = StiLocalization.Get("Report", "FinishingReport");

				if (report.Progress != null)report.Progress.HideProgressBar();

				if (report.Progress != null)
				{
					report.Progress.SetAllowClose(false);
					report.Progress.Update(report.StatusString, 90);
				}
			}
		}
		#endregion
	}
}
