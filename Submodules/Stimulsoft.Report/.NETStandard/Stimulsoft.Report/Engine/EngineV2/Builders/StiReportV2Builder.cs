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
	public class StiReportV2Builder
	{
		public static void RenderSingleReport(StiReport masterReport, StiRenderState renderState)
		{		
			var engineOnStart = masterReport.Engine;
			StiReport reportOfEngineOnStart = null;

			if (engineOnStart != null)
			    reportOfEngineOnStart = masterReport.Engine.Report;
            
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
				        masterReport.StatusString = StiLocalization.Get("Report", "CompilingReport");

				    if (masterReport.CompiledReport == null)
				        masterReport.Compile();

					if (masterReport.CompiledReport != null)
					{
						if (engineOnStart != null)
						{
							masterReport.CompiledReport.Engine = engineOnStart;
							masterReport.CompiledReport.Engine.Report = masterReport.CompiledReport;
						}
						else
						    masterReport.CompiledReport.Engine = new StiEngine(masterReport.CompiledReport);

						masterReport.CompiledReport.Tag = masterReport.Tag;
						masterReport.CompiledReport.Progress = masterReport.Progress;
						masterReport.CompiledReport.ParentReport = masterReport;
						masterReport.IsRendering = true;
						masterReport.CompiledReport.IsRendering = true;
                        masterReport.CompiledReport.RenderedWith = masterReport.RenderedWith;
                        masterReport.CompiledReport.CacheTotals = masterReport.CacheTotals;
                        masterReport.CompiledReport.CachedTotals = masterReport.CachedTotals;
                        masterReport.CompiledReport.bookmarkValue.Engine = masterReport.bookmarkValue.Engine;

						StiRenderProviderV2.Render(masterReport.CompiledReport, renderState);

						masterReport.DisposeCachedRichText();
						masterReport.DisposeCachedRichTextFormat();

						if (masterReport.CompiledReport != null)
						{
							masterReport.CompiledReport.DisposeCachedRichText();
							masterReport.CompiledReport.DisposeCachedRichTextFormat();

							masterReport.IsRendered = masterReport.CompiledReport.IsRendered;

							if (StiOptions.Engine.NotClearParentReport)
								masterReport.CompiledReport.ParentReport = null;
						}
						
						masterReport.IsRendering = true;
						masterReport.InvokeRefreshViewer();
					}
				}
                #endregion

                #region Already Compiled
				else
				{
					if (engineOnStart != null)
					{
						masterReport.Engine = engineOnStart;
						masterReport.Engine.Report = masterReport;
					}
					else
					    masterReport.Engine = new StiEngine(masterReport);

					masterReport.IsRendering = true;
					StiRenderProviderV2.Render(masterReport, renderState);					
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

					if (engineOnStart == null)
					    masterReport.CompiledReport.Engine = null; 
				}

				if (engineOnStart == null)
				    masterReport.Engine = null;
				else
				    engineOnStart.Report = reportOfEngineOnStart;
			}			
		}

		public static void RenderSubReports(StiReport ownerReport, StiRenderState renderState)
		{
			if (ownerReport.CheckNeedsCompiling() && ownerReport.CompiledReport == null)
				ownerReport.Compile();

			ownerReport.RenderedPages.Clear();

            lock (((ICollection)ownerReport.SubReports).SyncRoot)
			foreach (StiReport report in ownerReport.SubReports)
			{
				if (report.EngineVersion != ownerReport.EngineVersion)
					throw new Exception("A Value of the EngineVersion property of one of subreport " +
					                    "is not equal to the value of the EngineVersion property of a master report.");
			}

            #region Compile subreports
            var oldModes = new StiCalculationMode[ownerReport.SubReports.Count];

            lock (((ICollection) ownerReport.SubReports).SyncRoot)
		    {
		        for (int index = 0; index < ownerReport.SubReports.Count; index++)
		        {
                    StiReport report = ownerReport.SubReports[index];

                    oldModes[index] = report.CalculationMode;
                    if (StiOptions.Engine.ForceInterpretationMode)
                        report.CalculationMode = StiCalculationMode.Interpretation;

                    if (report.CheckNeedsCompiling() && report.CompiledReport == null)
		                report.Compile();
		        }
		    }
		    #endregion

			#region Prepare RenderedPages
		    lock (((ICollection) ownerReport.SubReports).SyncRoot)
		    {
		        foreach (StiReport report in ownerReport.SubReports)
		        {
		            report.RenderedPages = ownerReport.RenderedPages;
		        }
		    }
		    #endregion

			#region Rendering
			var copiesOfReportsTotals = new Hashtable();
			ownerReport.IsStopped = false;
			ownerReport.InvokeBeginRender();

			StiEngine engine;

			#region Render Master if not empty
			StiReport masterReport = null;
            if (ownerReport.Pages[0].GetComponents().Count > 0)
			{
                if (ownerReport.CheckNeedsCompiling())
				{
                    engine = new StiEngine(ownerReport.CompiledReport);
					ownerReport.Engine = engine;
					ownerReport.CompiledReport.Engine = engine;
                    ownerReport.CompiledReport.CurrentPrintPage = 0;
				}
				else
				{
                    engine = new StiEngine(ownerReport);
					ownerReport.Engine = engine;
                    ownerReport.CurrentPrintPage = 0;
				}

                ownerReport.SubReportsMasterReport = ownerReport;
				masterReport = ownerReport;

                if (ownerReport.CompiledReport != null)
				{
                    masterReport = ownerReport.CompiledReport;

                    ownerReport.CompiledReport.SubReportsMasterReport = ownerReport;
                    ownerReport.CompiledReport.RenderedPages.Clear();
				}

                var savedSubReports = ownerReport.SubReports;

                ownerReport.SubReports = null;
				renderState.IsSubReportMode = true;

				ownerReport.Render(renderState);
                ownerReport.SubReports = savedSubReports;

                ownerReport.SubReportsMasterReport = null;
			}
			else
			{
                engine = new StiEngine(ownerReport);
                ownerReport.CurrentPrintPage = 0;
				ownerReport.Engine = engine;

				if (ownerReport.CompiledReport != null)
					ownerReport.CompiledReport.Engine = engine;
			}

            if (ownerReport.CompiledReport != null)
                ownerReport.CurrentPrintPage = ownerReport.CompiledReport.CurrentPrintPage;
			#endregion

			var reportIndex = 0;
			StiReport prevReport = null;
            lock (((ICollection)ownerReport.SubReports).SyncRoot)
			foreach (StiReport report in ownerReport.SubReports)
			{
				try
				{
					#region Master Report
					report.SubReportsMasterReport = ownerReport;

					if (report.CompiledReport != null)
					{
						report.CompiledReport.SubReportsMasterReport = ownerReport;
						report.Engine = engine;
						report.CompiledReport.Engine = engine;
						report.CompiledReport.CurrentPrintPage = ownerReport.CurrentPrintPage;
					}
					else
					{
						report.Engine = engine;
						report.CurrentPrintPage = ownerReport.CurrentPrintPage;
					}
					#endregion

					#region Print On Previous Page
					if (prevReport != null)
					{
						report.Pages[0].PrintOnPreviousPage = report.SubReportsPrintOnPreviousPage;

					    if (report.CompiledReport != null)
					        report.CompiledReport.Pages[0].PrintOnPreviousPage = report.SubReportsPrintOnPreviousPage;
					}
					#endregion

                    #region Reset Page Number
					if (prevReport != null)
					{
						report.Pages[0].ResetPageNumber = report.SubReportsResetPageNumber;

					    if (report.CompiledReport != null)
					        report.CompiledReport.Pages[0].ResetPageNumber = report.SubReportsResetPageNumber;
					}
                    #endregion

					var renderedPagesCount = ownerReport.RenderedPages.Count;

				    if (engine.PageNumbers.PageNumbers.Count > 0)
				        engine.PageNumbers.PageNumbers[engine.PageNumbers.PageNumbers.Count - 1].FixedPosition = true;

				    report.Totals = new Hashtable();
				    report.Render(new StiRenderState(renderState.ShowProgress)
				    {
				        IsSubReportMode = renderState.IsSubReportMode
				    });

					ownerReport.CurrentPrintPage = report.CompiledReport != null
					    ? report.CompiledReport.CurrentPrintPage
					    : report.CurrentPrintPage;

                    #region Create a copy of totals of current report
					var copyOfTotals = new Hashtable();

					var rep = report;
					if (rep.CompiledReport != null)
					    rep = report.CompiledReport;

					var keys = new object[rep.Totals.Keys.Count];
					rep.Totals.Keys.CopyTo(keys, 0);
				    lock (keys.SyncRoot)
				    {
                        foreach (var key in keys)
                        {
                            var values = rep.Totals[key] as ArrayList;
                            if (values != null)
                            {
                                var newValues = new ArrayList();
                                copyOfTotals[key] = newValues;

                                lock (((ICollection)values).SyncRoot)
                                {
                                    foreach (StiRuntimeVariables value in values)
                                    {
                                        newValues.Add(value.Clone());
                                    }
                                }
                            }

                            string keySt = key as string;
                            if (keySt != null && keySt.StartsWith("#%#"))
                            {
                                copyOfTotals[key] = rep.Totals[key];
                            }
                        }
				    }

				    copiesOfReportsTotals[reportIndex++] = copyOfTotals;
                    #endregion

                    #region Store anchors from current report
                    foreach (DictionaryEntry de in rep.Anchors)
                    {
                        if (!masterReport.Anchors.ContainsKey(de.Key))
                        {
                            masterReport.Anchors[de.Key] = de.Value;
                        }
                    }
                    #endregion

                    #region Convert units
                    if (report.ReportUnit != ownerReport.ReportUnit)
					{
						for (var index = renderedPagesCount; index < ownerReport.RenderedPages.Count; index++)
						{
							var page = ownerReport.RenderedPages[index];
							page.Convert(report.Unit, ownerReport.Unit);
						}
					}
					#endregion
				}
				finally
				{
					report.SubReportsMasterReport = null;
					if (report.CompiledReport != null)
					    report.CompiledReport.SubReportsMasterReport = null;
				}

				#region Stop report
				if (report.IsStopped)
				{
					ownerReport.IsStopped = report.IsStopped;
					break;
				}
				#endregion

				ownerReport.Bookmark.Bookmarks.AddRange(report.Bookmark.Bookmarks);
				ownerReport.Pointer.Bookmarks.AddRange(report.Pointer.Bookmarks);

				prevReport = report;
			}
			#endregion

			engine.PageNumbers.ProcessPageNumbers();

			#region Finalizing reports
            #region First pass
			reportIndex = 0;
		    lock (((ICollection) ownerReport.SubReports).SyncRoot)
		    {
		        foreach (StiReport report in ownerReport.SubReports)
		        {
		            var rep = report;
		            if (rep.CompiledReport != null)
		                rep = rep.CompiledReport;

		            rep.Totals = copiesOfReportsTotals[reportIndex++] as Hashtable;
		            rep.InvokeEndRender();
		        }
		    }
		    #endregion

            #region Second pass, clear report states
		    lock (((ICollection) ownerReport.SubReports).SyncRoot)
		    {
                for (int index = 0; index < ownerReport.SubReports.Count; index++)
                {
                    StiReport report = ownerReport.SubReports[index];

                    report.CalculationMode = oldModes[index];

		            report.Totals = null;
		        }
		    }
		    #endregion

			if (masterReport != null)
			{
				masterReport.Engine = engine;
				masterReport.InvokeEndRender();
				masterReport.Engine = null;
			}

			if (masterReport == null || masterReport != ownerReport)
			    ownerReport.InvokeEndRender();

		    lock (((ICollection) ownerReport.RenderedPages).SyncRoot)
		    {
		        foreach (StiPage page in ownerReport.RenderedPages)
		        {
		            page.Report = ownerReport.CompiledReport ?? ownerReport;
		        }
		    }
		    #endregion

            ownerReport.Engine = null;
            ownerReport.IsRendered = true;
            //ownerReport.NeedsCompiling = false;

            if (ownerReport.CompiledReport != null)
            {
                ownerReport.CompiledReport.Engine = null;
                ownerReport.CompiledReport.IsRendered = true;
                //ownerReport.CompiledReport.NeedsCompiling = false;
            }

            if (ownerReport.ParentReport != null)
            {
                ownerReport.ParentReport.Engine = null;
                ownerReport.ParentReport.IsRendered = true;
                //ownerReport.ParentReport.NeedsCompiling = false;
            }
            			
            StiPostProcessProviderV2.PostProcessPages(ownerReport.RenderedPages);            

			if (ownerReport != null && ownerReport.RenderedPages != null)
			{
				foreach (StiPage page in ownerReport.RenderedPages)
				{
					page.Report = ownerReport;
				}
			}

			renderState.IsSubReportMode = false;
			ownerReport.InvokeRefreshViewer();
		}
	}
}
