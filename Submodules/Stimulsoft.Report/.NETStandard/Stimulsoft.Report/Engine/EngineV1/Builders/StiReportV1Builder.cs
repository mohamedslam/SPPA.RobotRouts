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
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiReportV1Builder
	{
		public static void RenderSingleReport(StiReport masterReport, StiRenderState renderState)
		{			
			try
			{
				masterReport.IsStopped = false;
				masterReport.IndexName = 1;

				StiLogService.Write(masterReport.GetType(), "Rendering report");

				masterReport.IsRendered = false;

                #region NeedsCompiling
				if (masterReport.CheckNeedsCompiling())
				{
					if (masterReport.Progress != null)
					{
						masterReport.StatusString = StiLocalization.Get("Report", "CompilingReport");
					}
					if (masterReport.CompiledReport == null) masterReport.Compile();

					if (masterReport.CompiledReport != null)
					{

						masterReport.CompiledReport.Tag = masterReport.Tag;
						masterReport.CompiledReport.Progress = masterReport.Progress;
						masterReport.CompiledReport.ParentReport = masterReport;
						masterReport.IsRendering = true;
						masterReport.CompiledReport.IsRendering = true;

						StiRenderProviderV1.Render(masterReport.CompiledReport, renderState);

						masterReport.DisposeCachedRichText();
						masterReport.DisposeCachedRichTextFormat();

						if (masterReport.CompiledReport != null)
						{
							masterReport.CompiledReport.DisposeCachedRichText();
							masterReport.CompiledReport.DisposeCachedRichTextFormat();

							masterReport.IsRendered = masterReport.CompiledReport.IsRendered;
							if (StiOptions.Engine.NotClearParentReport) masterReport.CompiledReport.ParentReport = null;
						}
						masterReport.IsRendering = true;
						masterReport.InvokeRefreshViewer();
					}
				}
                #endregion

                #region Already Compiled
				else
				{
					masterReport.IsRendering = true;
					StiRenderProviderV1.Render(masterReport, renderState);					
					masterReport.InvokeRefreshViewer();
				}
                #endregion
			}
			catch (Exception e)
			{
				StiLogService.Write(masterReport.GetType(), "Rendering report...ERROR");
				StiLogService.Write(masterReport.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
			}
			finally
			{
				masterReport.IsRendering = false;
				if (masterReport.CompiledReport != null)
				{
					masterReport.CompiledReport.IsRendering = false;
				}		
			}
		}

		public static void RenderSubReports(StiReport ownerReport, StiRenderState renderState)
		{
			if (ownerReport.CheckNeedsCompiling() && ownerReport.CompiledReport == null)
				ownerReport.Compile();

			ownerReport.RenderedPages.Clear();			

			#region Compile
			foreach (StiReport report in ownerReport.SubReports)
			{
				if (report.CheckNeedsCompiling() && report.CompiledReport == null) report.Compile();
			}
			#endregion

			#region Prepare RenderedPages
			foreach (StiReport report in ownerReport.SubReports)
			{
				report.RenderedPages = ownerReport.RenderedPages;
			}		
			#endregion

			#region Rendering
			Hashtable copiesOfReportsTotals = new Hashtable();
			ownerReport.IsStopped = false;
			ownerReport.InvokeBeginRender();
			ownerReport.PageNumber = 1;			
			ownerReport.EngineV1.RealPageNumber = 1;

			#region Render Master if not empty
			StiReport masterReport = null;
			if (ownerReport.Pages[0].GetComponents().Count > 0)
			{

				ownerReport.SubReportsMasterReport = ownerReport;
				masterReport = ownerReport;

				if (ownerReport.CompiledReport != null)
				{
					masterReport = ownerReport.CompiledReport;

					ownerReport.CompiledReport.SubReportsMasterReport = ownerReport;
					ownerReport.CompiledReport.RenderedPages.Clear();
				}

				StiReportsCollection savedSubReports = ownerReport.SubReports;

				ownerReport.SubReports = null;
				//StiRenderState renderState = new StiRenderState(showProgress);
				renderState.IsSubReportMode = true;

				ownerReport.Render(renderState);
				ownerReport.SubReports = savedSubReports;
				ownerReport.SubReportsMasterReport = null;
			}
			#endregion

			int reportIndex = 0;
			StiReport prevReport = null;
			foreach (StiReport report in ownerReport.SubReports)
			{
				#region Reset page number if need
				if (report.SubReportsResetPageNumber && prevReport != null)
				{
					prevReport.totalPageCountValue = ownerReport.totalPageCountValue;
					prevReport.EngineV1.ProcessResetPageNumber(prevReport);
					ownerReport.PageNumber = 1;
					prevReport.EngineV1.PageInProgress = null;
				}
				#endregion

				try
				{
					#region Master Report
					report.SubReportsMasterReport = ownerReport;
					if (report.CompiledReport != null)report.CompiledReport.SubReportsMasterReport = ownerReport;
					#endregion

					#region Print On Previous Page
					if (prevReport != null)
					{
						report.Pages[0].PrintOnPreviousPage = report.SubReportsPrintOnPreviousPage;

						if (report.CompiledReport != null)
						{
							report.CompiledReport.Pages[0].PrintOnPreviousPage = report.SubReportsPrintOnPreviousPage;
						}
					}
					#endregion

					int renderedPagesCount = ownerReport.RenderedPages.Count;

					report.Totals = new Hashtable();
					report.Render(renderState.ShowProgress);

                    #region Create a copy of totals of the current report
					Hashtable copyOfTotals = new Hashtable();

					StiReport rep = report;
					if (rep.CompiledReport != null) rep = report.CompiledReport;

					object[] keys = new object[rep.Totals.Keys.Count];
					rep.Totals.Keys.CopyTo(keys, 0);
					foreach (object key in keys)
					{
						ArrayList values = rep.Totals[key] as ArrayList;
						if (values != null)
						{
							ArrayList newValues = new ArrayList();
							copyOfTotals[key] = newValues;

							foreach (StiRuntimeVariables value in values)
							{
								newValues.Add(value.Clone());
							}
						}
					}
					copiesOfReportsTotals[reportIndex++] = copyOfTotals;
                    #endregion

                    #region Convert units
					if (report.ReportUnit != ownerReport.ReportUnit)
					{
						for (int index = renderedPagesCount; index < ownerReport.RenderedPages.Count; index++)
						{
							StiPage page = ownerReport.RenderedPages[index];
							page.Convert(report.Unit, ownerReport.Unit);
						}
					}
					#endregion
				}
				finally
				{
					report.SubReportsMasterReport = null;
					if (report.CompiledReport != null)report.CompiledReport.SubReportsMasterReport = null;
				}

				#region Stop report
				if (report.IsStopped)
				{
					ownerReport.IsStopped = report.IsStopped;
					break;
				}
				#endregion

				#region Bookmarks
				ownerReport.Bookmark.Bookmarks.AddRange(report.Bookmark.Bookmarks);
				#endregion

				prevReport = report;
			}
			#endregion

			#region Finalizing reports
            #region First pass
			reportIndex = 0;
			foreach (StiReport report in ownerReport.SubReports)
			{
				StiReport rep = report;
				if (rep.CompiledReport != null)rep = rep.CompiledReport;

				rep.Totals = copiesOfReportsTotals[reportIndex++] as Hashtable;                

				rep.TotalPageCount = ownerReport.PageNumber - 1;

				rep.EngineV1.ProcessResetPageNumber(rep);
				rep.InvokeEndRender();
			}
            #endregion

            #region Second pass, clear report states
			foreach (StiReport report in ownerReport.SubReports)
			{
				report.EngineV1.PageInProgress = null;
				report.Totals = null;
				report.EngineV1.ReportPageNumbers = null;
				report.EngineV1.ReportTotalPageCounts = null;
			}
            #endregion

			if (masterReport != null)
			{
				masterReport.TotalPageCount = ownerReport.PageNumber - 1;

				masterReport.EngineV1.ProcessResetPageNumber(masterReport);
				masterReport.InvokeEndRender();
				masterReport.EngineV1.PageInProgress = null;
			}
			if (masterReport == null || masterReport != ownerReport)ownerReport.InvokeEndRender();

			foreach (StiPage page in ownerReport.RenderedPages)
			{
				page.Report = ownerReport.CompiledReport != null ? ownerReport.CompiledReport : ownerReport;
			}
			#endregion

			ownerReport.IsRendered = true;
			ownerReport.NeedsCompiling = false;
            if (ownerReport.CompiledReport != null)
            {
                ownerReport.CompiledReport.IsRendered = true;
                ownerReport.CompiledReport.NeedsCompiling = false;
            }
            if (ownerReport.ParentReport != null)
            {
                ownerReport.ParentReport.IsRendered = true;
                ownerReport.ParentReport.NeedsCompiling = false;
            }

            StiPostProcessProviderV1.PostProcessPages(ownerReport.RenderedPages);
			ownerReport.InvokeRefreshViewer();

		}
	}
}
