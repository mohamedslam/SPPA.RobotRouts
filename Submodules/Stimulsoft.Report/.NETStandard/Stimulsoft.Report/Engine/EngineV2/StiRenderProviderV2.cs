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

using Stimulsoft.Base;
using Stimulsoft.Base.Exceptions;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Drawing.Printing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Engine
{
    public sealed class StiRenderProviderV2
    {
        #region Constants
        private const string nullGuid = "nullGuid";
        #endregion

        #region Methods.Main
        /// <summary>
		/// Render the report.
		/// </summary>
		/// <param name="report">Report for rendering.</param>
        /// <param name="state">State for the rendering.</param>
        public static void Render(StiReport report, StiRenderState state)
		{
            #region Check for Process EndOfPage
            var comps2 = report.GetComponents();
            foreach (StiComponent comp in comps2)
            {
                var text = comp as StiText;
                if (text != null && text.ProcessAt == StiProcessAt.EndOfPage)
                {
                    report.Engine.AllowEndOfPageProcessing = true;
                    break;
                }
            }
            #endregion

			InitReport(report);

            report.Engine.RenderingStartTicks = Environment.TickCount;

            if (!CheckDialogsInPreview(report))
            {
                /*Before report rendering is stopped
                connected to data and to fill variables were not empty*/

                StiVariableHelper.FillItemsOfVariables(report.CompiledReport ?? report, true);

                #region ConnectToData, only used datasources
                var dataSources = StiDataSourceHelper.GetDataSourcesUsedInRequestFromUsersVariables(report);
                var states = new Hashtable();
                foreach (StiDataSource ds in report.Dictionary.DataSources)
                {
                    if (!dataSources.ContainsKey(ds.Name))
                    {
                        states[ds.Name] = ds.ConnectOnStart;
                        ds.ConnectOnStart = false;
                    }
                }

                ConnectToData(report);

                foreach (StiDataSource ds in report.Dictionary.DataSources)
                {
                    if (states.ContainsKey(ds.Name))
                        ds.ConnectOnStart = (bool) states[ds.Name];
                }
                #endregion

                StiVariableHelper.FillItemsOfVariables(report.CompiledReport ?? report, false);

                if (report.Pages.Count > 0 && report.RenderedPages.Count == 1)
                {
                    var pageFrom = report.Pages[0];
                    var pageTo = report.RenderedPages[0];
                    pageTo.PaperSize = pageFrom.PaperSize;
                    pageTo.Orientation = pageFrom.Orientation;

                    if (pageFrom.PaperSize == PaperKind.Custom)
                    {
                        pageTo.Width = pageFrom.Width;
                        pageTo.Height = pageFrom.Height;
                    }
                }

                return;
            }

            if (report.SubReportsMasterReport == null)
                report.RenderedPages.Clear();

			var numberOfPass = GetNumberOfPass(report);

			var dialogsOnStartExist = IsDialogsOnStartExist(report);
			if (!dialogsOnStartExist)
			    RenderFirstPass(report, numberOfPass);
						
			var masterReport = report.SubReportsMasterReport;
            var newTableComponents = new Hashtable();
            Hashtable eventsHandlers = null;

			try
			{
				PrepareSubReportsAndDrillDownPages(report);
				
				if (masterReport == null)
				    report.RenderedPages.Clear();
				
				report.InvokeBeginRender();

				ClearTotals(report);

                #region Prepare CrossTab for StiCrossTabDataSource
                foreach (StiDataSource ds in report.Dictionary.DataSources)
                {
                    var cds = ds as StiCrossTabDataSource;
                    if ((cds != null) && !string.IsNullOrEmpty(cds.NameInSource))
                    {
                        var comp = report.GetComponentByName(cds.NameInSource);
                        if (comp != null)
                            comp.Prepare();
                    }
                }
                #endregion

                bool isConnectToDataV2 = report.MetaTags?["*ConnectToDataV2*"] != null;

                StiVariableHelper.SetDefaultValueForRequestFromUserVariablesIfUserItems(report.CompiledReport ?? report);
                StiVariableHelper.FillItemsOfVariables(report.CompiledReport ?? report, true);
                ConnectToData(report, isConnectToDataV2);
                bool haveModifiedVars = StiVariableHelper.FillItemsOfVariables(report.CompiledReport ?? report, false);
                StiVariableHelper.SetDefaultValueForRequestFromUserVariables(report.CompiledReport ?? report, haveModifiedVars, false, isConnectToDataV2);

                if (!RenderFormsOnStart(report))return;
				if (dialogsOnStartExist)
				{
					DisconnectFromData(report);
					RenderFirstPass(report, numberOfPass);
					ConnectToData(report);
				}
			
				#region Prepare Bookmarks
				report.Bookmark.Bookmarks.Clear();
				report.Bookmark.Text = report.ReportAlias;
                                
                if (report.IsSecondPass)
                {
                    report.Engine.FirstPassPointer = report.Pointer;

                    report.Pointer = new StiBookmark();
                    report.Pointer.Text = report.ReportAlias;
                }
                #endregion

                if (report.Progress != null) 
                    report.Progress.ShowProgressBar();

			    if (report.ReportPass == StiReportPass.First || report.ReportPass == StiReportPass.None)
			        InitCacheMode(report);

			    #region Prepare components in report

                #region Prepare StiTable 
                var comps = report.GetComponents();
                var listTable = comps.Cast<StiComponent>().Where(c => c is StiTable && c.Enabled).ToList();
                report.ContainsTables = listTable.Count != 0;

                if (listTable.Count > 0)
                {
                    var tableMasterComponent = new Hashtable();
                    var tableAutoWidth = false;
                    foreach (StiTable table in listTable)
                    {
                        if (table.AutoWidth != StiTableAutoWidth.None)
                            tableAutoWidth = true;

                        var newDataBand = table.StartRenderTableBand(ref newTableComponents);
                        if (newDataBand != null)
                        {
                            tableMasterComponent.Add(newDataBand.Name, newDataBand);
                            report.Engine.KeepFirstDetailTogetherTablesList[newDataBand] = table;
                        }
                    }

                    if (tableMasterComponent.Count != 0)
                    {
                        foreach (StiDataBand band in tableMasterComponent.Values)
                        {
                            if (band.MasterComponent != null && band.MasterComponent is StiTable)
                                band.MasterComponent = tableMasterComponent[band.MasterComponent.Name + "_DB"] as StiDataBand;
                        }
                    }

                    comps.Clear();
                    comps = report.GetComponents();
                    report.ContainsTables = tableAutoWidth;
                }
                #endregion

                report.Engine.ParserConversionStore = new Hashtable();
                report.Engine.AnchorsArguments = new Hashtable();
                report.Engine.HashDataSourceReferencesCounter = new Hashtable();
                eventsHandlers = new Hashtable();
                var isCompilationMode = report.CalculationMode == StiCalculationMode.Compilation;

                foreach (StiComponent comp in comps)
                {
                    comp.Prepare();

                    if (!isCompilationMode)
                    {
                        #region Prepare conditions
                        if (!isCompilationMode && comp.Conditions.Count > 0)
                        {
                            var conditionList = new ArrayList();
                            foreach (StiBaseCondition cond in comp.Conditions)
                            {
                                var condition = cond as StiCondition;
                                if (cond is StiMultiCondition)
                                {
                                    var multiCondition = cond as StiMultiCondition;
                                    if (multiCondition.FilterOn && multiCondition.Filters.Count > 0)
                                    {
                                        var conditionExpression = new StringBuilder("{");
                                        for (var index = 0; index < multiCondition.Filters.Count; index++)
                                        {
                                            var filter = multiCondition.Filters[index];
                                            conditionExpression.Append("(");
                                            conditionExpression.Append(StiDataHelper.GetFilterExpression(filter, filter.Column, report));
                                            conditionExpression.Append(")");

                                            if (index < multiCondition.Filters.Count - 1)
                                                conditionExpression.Append(multiCondition.FilterMode == StiFilterMode.And ? " && " : " || ");
                                        }
                                        conditionExpression.Append("}");
                                        var de = new DictionaryEntry(multiCondition, conditionExpression.ToString());
                                        conditionList.Add(de);
                                    }
                                }
                                else if (condition != null)
                                {
                                    var expression = "{" + StiDataHelper.GetFilterExpression(condition, condition.Column, report) + "}";
                                    var de = new DictionaryEntry(condition, expression);
                                    conditionList.Add(de);
                                }
                            }

                            if (conditionList.Count > 0)
                                report.Engine.ParserConversionStore["*StiConditionExpression*" + comp.Name] = conditionList;
                        }
                        #endregion

                        #region Prepare DataBandsUsedInPageTotals
                        var stiText = comp as StiText;
                        if (stiText != null)
                            StiParser.CheckForDataBandsUsedInPageTotals(stiText, report);
                        #endregion

                        #region Prepare RichText expression
                        if (comp is StiRichText && StiOptions.Engine.FullTrust)
                        {
                            var richText = comp as StiRichText; 
                            var script = Helpers.StiXmlDecodeFastHelper.XmlDecodeFast(richText.Text.Value).Replace((char)0, ' ');

                            try
                            {
                                var textToParse = GetRichTextToParse(script, richText).Replace('\0', ' ');
                                if (richText.FullConvertExpression)
                                    textToParse = $"ConvertRtf({textToParse})";

                                if (textToParse.Length > 0)
                                    report.Engine.ParserConversionStore[$"*StiRichTextExpression*{comp.Name}"] = textToParse;
                            }
                            catch (Exception ex)
                            {
                                var str = $"Expression in Text property of '{comp.Name}' can't be evaluated! {ex.Message}";
                                StiLogService.Write(comp.GetType(), str);
                                StiLogService.Write(comp.GetType(), ex.Message);
                                report.WriteToReportRenderingMessages(str);
                            }
                        }
                        #endregion

                        #region Prepare AddAnchor events for Interpretation mode
                        if (comp is StiDataBand && (comp as StiDataBand).RenderingEvent != null && !string.IsNullOrEmpty((comp as StiDataBand).RenderingEvent.Script))
                        {
                            var eventText = (comp as StiDataBand).RenderingEvent.Script;
                            var pos = eventText.IndexOf("AddAnchor(", StringComparison.InvariantCulture);
                            if (pos != -1)
                            {
                                var endPos = eventText.IndexOf(")", pos, StringComparison.InvariantCulture);
                                if (endPos != -1)
                                {
                                    pos += 10;
                                    var args = eventText.Substring(pos, endPos - pos);

                                    report.Engine.AnchorsArguments[comp.Name] = args;
                                    var eh = new EventHandler(StiRenderProviderV2_AddAnchor_Rendering);
                                    (comp as StiDataBand).Rendering += eh;
                                    eventsHandlers[comp] = eh;
                                }
                            }
                        }
                        if (comp is StiPage && (comp as StiPage).RenderingEvent != null && !string.IsNullOrEmpty((comp as StiPage).RenderingEvent.Script))
                        {
                            var eventText = (comp as StiPage).RenderingEvent.Script;
                            var pos = eventText.IndexOf("AddAnchor(", StringComparison.InvariantCulture);
                            if (pos != -1)
                            {
                                var endPos = eventText.IndexOf(")", pos, StringComparison.InvariantCulture);
                                if (endPos != -1)
                                {
                                    pos += 10;
                                    var args = eventText.Substring(pos, endPos - pos);

                                    report.Engine.AnchorsArguments[comp.Name] = args;
                                    var eh = new EventHandler(StiRenderProviderV2_AddAnchor_Rendering);
                                    (comp as StiPage).Rendering += eh;
                                    eventsHandlers[comp] = eh;
                                }
                            }
                        }
                        #endregion
                    }

                    #region Count DataSources and BusinessObjects references
                    var dataBand = comp as StiDataBand;
                    if (dataBand != null)
                    {
                        if (!dataBand.IsDataSourceEmpty)
                        {
                            var obj = report.Engine.HashDataSourceReferencesCounter[dataBand.DataSourceName];
                            var counter = obj == null ? 1 : (int)obj + 1;
                            report.Engine.HashDataSourceReferencesCounter[dataBand.DataSourceName] = counter;
                        }

                        if (!dataBand.IsBusinessObjectEmpty)
                        {
                            var obj = report.Engine.HashDataSourceReferencesCounter[dataBand.BusinessObject.Name];
                            var counter = obj == null ? 1 : (int)obj + 1;
                            report.Engine.HashDataSourceReferencesCounter[dataBand.BusinessObject.Name] = counter;
                        }
                    }
                    #endregion
                }
                #endregion

				RenderReport(report, masterReport, state);//Main cycle
                
                /*If report rendering is not stopped then we call forms 
                 * on end and we will set flag IsRenderedValue to true*/
                if (!report.IsStopped)
                {
                    RenderFormsOnEnd(report);
                    report.IsRendered = true;
                }

                #region Create table components
                if (newTableComponents.Count > 0)
                {
                    foreach (StiPage pageKey in newTableComponents.Keys)
                    {
                        var list = newTableComponents[pageKey] as ArrayList;
                        foreach (StiComponent comp in list)
                        {
                            if (comp.Parent != null)
                                comp.Parent.Components.Remove(comp);
                        }
                    }
                }
                #endregion

                #region Restore base table state
                foreach (var table in listTable)
                {
                    table.Enabled = true;
                }
                #endregion

            }

			#region Processing Errors
			catch (Exception e)
			{
				report.IsStopped = true;
				StiLogService.Write(typeof(StiRenderProviderV2), e);
                if (!StiOptions.Engine.HideExceptions) throw;
			}
			#endregion

			finally
			{   								
				report.CurrentPage = 0;
                if (masterReport == null)
    				report.CurrentPrintPage = 0;
				
				if (masterReport == null && report.ReportPass != StiReportPass.First)
				{
				    if (!report.RenderedPages.CacheMode)
				        StiPostProcessProviderV2.PostProcessPages(report.RenderedPages);

				    else
				    {
				        report.RenderedPages.Flush();

				        if (report.RenderedPages.NotCachedPages != null)
				        {
				            lock (((ICollection) report.RenderedPages.NotCachedPages).SyncRoot)
				            {
				                foreach (var page in report.RenderedPages.NotCachedPages)
				                {
				                    StiPostProcessProviderV2.PostProcessPage(page, IsFirstPage(report, page), IsLastPage(report, page));
				                    StiPostProcessProviderV2.PostProcessPrimitives(page);
				                }
				            }
				        }
				    }
				}

				FinishAllPagesInNotCachedPagesArray(report);

				RemoveAllPagesLessThenFromPageAndGreaterThenToPage(report, state);

				MadeCollate(report);
                MadeMirrorMargins(report);

                StiBookmarksV2Helper.PrepareBookmark(report.Bookmark);
                StiBookmarksV2Helper.Pack(report.Bookmark);

                StiBookmarksV2Helper.PrepareBookmark(report.Pointer);
                StiBookmarksV2Helper.Pack(report.Pointer);

                if (StiOptions.Engine.DisconnectFromDataBeforeEndRender)
				    DisconnectFromData(report);

				//If report empty, adds one page to report
                if (masterReport == null)
                {
                    if (report.RenderedPages.Count == 0)
                        report.RenderedPages.Add(StiActivator.CreateObject("Stimulsoft.Report.Components.StiPage", new object[] { report }) as StiPage);

                    report.Engine.ProgressHelper.AllowCachedPagesCache = report.ReportPass != StiReportPass.First;
                    report.Engine.PageNumbers.ProcessPageNumbers();
                    report.InvokeEndRender();
                    report.Engine.PageNumbers.Clear();

                    #region Clear StiContainerInfoV2.DataSourceRow for all rendered pages
                    if (!report.RenderedPages.CacheMode)
                    {
                        foreach (StiPage page in report.RenderedPages)
                        {
                            var components = page.GetComponents();
                            foreach (StiComponent component in components)
                            {
                                var cont = component as StiContainer;
                                if (cont != null && cont.ContainerInfoV2 != null)
                                {
                                    cont.ContainerInfoV2.DataSourceRow = null;
                                    cont.ContainerInfoV2.BusinessObjectCurrent = null;
                                }
                            }
                        }
                    }
                    #endregion
                }

                if (!StiOptions.Engine.DisconnectFromDataBeforeEndRender)
                    DisconnectFromData(report);

                FinishProgressForm(report);

                #region Finish First & Second passes
                if (report.ReportPass == StiReportPass.Second)
					report.ReportPass = StiReportPass.None;

				if (masterReport == null)
				{
					ClearTotals(report);

					if (report.ReportPass != StiReportPass.First)
						report.ParentReport = null;   
				}

                if (report.Engine.ParserConversionStore != null)
                {
                    report.Engine.ParserConversionStore.Clear();
                    report.Engine.ParserConversionStore = null;
                }
				#endregion

				#region UnPrepare components in report
				var comps = report.GetComponents();
				foreach (StiComponent comp in comps)
				{
					comp.UnPrepare();
				}

                if (eventsHandlers != null)
                {
                    foreach (DictionaryEntry de in eventsHandlers)
                    {
                        if (de.Key is StiDataBand) (de.Key as StiDataBand).Rendering -= de.Value as EventHandler;
                        if (de.Key is StiPage) (de.Key as StiPage).Rendering -= de.Value as EventHandler;
                    }
                }

                report.Engine.KeepFirstDetailTogetherTablesList.Clear();
                #endregion

                StiTableOfContentsV2Builder.PostProcessTableOfContents(report);
            }
		}

        static void StiRenderProviderV2_AddAnchor_Rendering(object sender, EventArgs e)
        {
            var comp = sender as StiContainer;
            if (comp == null || comp.Report == null || comp.Report.Engine == null || 
                !comp.Report.Engine.AnchorsArguments.Contains(comp.Name)) return;

            var arg = (string)comp.Report.Engine.AnchorsArguments[comp.Name];
            if (string.IsNullOrEmpty(arg)) return;

            var tempText = new StiText
            {
                Name = "**AddAnchor**",
                Page = comp.Report.Pages[0]
            };
            var result = StiParser.ParseTextValue("{" + arg + "}", tempText);
            comp.Report.AddAnchor(result);
        }

        internal static string GetRichTextToParse(string script, StiRichText richText)
        {
            var first = true;

#if NETSTANDARD
            var al = StiCodeDomExpressionHelper.GetLexemSimple(null, script, richText);
#else
            var al = richText != null && richText.FullConvertExpression 
                ? StiCodeDomExpressionHelper.GetLexemFullRtf(null, script, richText) 
                : StiCodeDomExpressionHelper.GetLexemSimple(null, script, richText);
#endif

            var res = new StringBuilder();
            if (al == null || al.Count == 0)
                res.Append(script);

            foreach (var str in al)
            {
                if (!first)
                {
                    if (richText != null && richText.FullConvertExpression)
                        res = res.Append(" , ");
                    else
                        res = res.Append(" + ");
                }

                if (str[0] == '{' && str[str.Length - 1] == '}')
                    res = res.Append("ToString(" + str.Substring(1, str.Length - 2).Replace("\\", "\\\\") + ")");

                else
                    res = res.AppendFormat("@\"{0}\"", str.Replace("\"", "\"\""));

                first = false;
            }
            return res.ToString();
        }
		#endregion

		#region Methods.Data
		internal static void ConnectToData(StiReport report, bool isConnectToDataV2 = false)
		{
            report.StatusString = StiLocalization.CultureName == "en"
                ? "Connecting to Data"
                : StiLocalization.Get("Report", "ConnectingToData");

            if (isConnectToDataV2)
                report.Dictionary.ConnectV2(report);
            else
                report.Dictionary.Connect();

			foreach (StiDataSource dataSource in report.Dictionary.DataSources)
			{
				dataSource.ResetData();
                dataSource.ResetDetailsRows();

                if (StiOptions.Dictionary.EnableConnectOnStartOnSecondPass)
                {
                    var sqlSource = dataSource as StiSqlSource;
                    if (sqlSource != null)
                        sqlSource.ConnectOnStart = true;
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
				var formControl = page as StiForm;
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
			if (report.IsInteractionRendering) return true;
		    if (StiOptions.Configuration.IsWeb || report.ReportPass == StiReportPass.First) return true;

		    report.StatusString = string.Empty;

		    var provider = StiDialogsProvider.GetProvider(report);
		    if (provider == null)return true;

		    var result = provider.Render(report, StiFormStartMode.OnStart);
		    if (!result)
		    {
		        StiLogService.Write(typeof(StiRenderProviderV2), "Report stopped in Dialog Forms");
		        report.IsStopped = true;
		        return false;
		    }

		    if (report.Progress != null)
		    {					
		        if (!report.Progress.IsVisible)report.Progress.Show();
		        report.Progress.SetAllowClose(true);
		    }

		    return true;
		}

		private static void RenderFormsOnEnd(StiReport report)
		{
			if (report.IsInteractionRendering) return;
		    if (StiOptions.Configuration.IsWeb || report.ReportPass == StiReportPass.First) return;

		    report.StatusString = string.Empty;

		    var provider = StiDialogsProvider.GetProvider(report);
		    if (provider == null) return;

		    var result = provider.Render(report, StiFormStartMode.OnEnd);
		    if (!result)
		    {
		        StiLogService.Write(typeof(StiRenderProviderV2), "Report stopped in Dialog Forms");
		        report.IsStopped = true;
		        return;
		    }

		    if (report.Progress != null)
		    {
		        if (!report.Progress.IsVisible)report.Progress.Show();
		        report.Progress.SetAllowClose(true);
		    }
		}

		private static bool CheckDialogsInPreview(StiReport report)
        {
            if (report.IsInteractionRendering) return true;

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (variable.RequestFromUser && !report.IsReportRenderingAfterSubmit && report.RequestParameters)
                {
                    report.IsStopped = false;
                    report.IsRendered = true;

                    if (report.ParentReport != null)
                        report.ParentReport.IsRendered = true;

                    return false;
                }
            }

            foreach (StiPage page in report.Pages)
			{
				var formControl = page as StiForm;
			    if (formControl == null || formControl.StartMode != StiFormStartMode.OnPreview || report.IsReportRenderingAfterSubmit) continue;

			    report.IsStopped = false;
			    report.IsRendered = true;

			    if (report.ParentReport != null)
			        report.ParentReport.IsRendered = true;

			    return false;
			}
			return true;
		}
		#endregion

		#region Methods.CacheMode
		internal static void ClearPagesWhichLessThenFromPageAndGreaterThenToPage(StiReport report, StiRenderState state)
		{
		    if (report.RenderedPages.Count <= 1 || report.ReportPass == StiReportPass.First) return;

		    var indexLastPage = report.RenderedPages.Count - 2;
		    if (state.FromPage >= 0 && state.FromPage > indexLastPage || state.ToPage >= 0 && state.ToPage < indexLastPage)
		        report.RenderedPages[indexLastPage].Components.Clear();

		    if (state.ToPage > 0 && state.ToPage < indexLastPage && state.RenderOnlyPagesFromRange)
		        report.IsStopped = true;
		}


		private static void InitCacheMode(StiReport report)
		{
		    if (report.ReportCacheMode == StiReportCacheMode.Off) return;

		    if (report.ReportCacheMode == StiReportCacheMode.On)
		    {
		        report.RenderedPages.CacheMode = true;
		        report.RenderedPages.CheckCacheL2();
		    }

		    report.RenderedPages.CanUseCacheMode = true;
		}


		private static void RemoveAllPagesLessThenFromPageAndGreaterThenToPage(StiReport report, StiRenderState state)
		{
		    if (!state.DestroyPagesWhichNotInRange || report.ReportPass == StiReportPass.First) return;

		    if (state.ToPage >= 0)
		    {
		        while (state.ToPage < report.RenderedPages.Count - 1)
		        {
		            report.RenderedPages.RemoveAt(report.RenderedPages.Count - 1);
		        }
		    }

		    var fromPage = state.FromPage;
		    while (fromPage > 0 && report.RenderedPages.Count > 0)
		    {
		        report.RenderedPages.RemoveAt(0);
		        fromPage--;
		    }
		}

		private static void FinishAllPagesInNotCachedPagesArray(StiReport report)
		{
		    if (!report.RenderedPages.CacheMode || report.RenderedPages.NotCachedPages == null || 
		        report.ReportPass == StiReportPass.First) return;

		    report.RenderedPages.NotCachedPages.Clear();
		    report.RenderedPages.NotCachedPages = null;
		}


        internal static void ProcessPageToCache(StiReport report, StiPage page, bool final)
        {
            if (!report.RenderedPages.CacheMode || report.ReportPass == StiReportPass.First) return;

            if (!final)
            {
                if (report.RenderedPages.NotCachedPages == null)
                    report.RenderedPages.NotCachedPages = new List<StiPage>();

                if (report.RenderedPages.NotCachedPages.IndexOf(page) == -1)
                    report.RenderedPages.NotCachedPages.Add(page);
            }
            else
            {
                if (report.Engine != null && report.Engine.PageNumbers != null && report.Engine.PageNumbers.PageNumbers.Count > 0)
                {
                    StiPostProcessProviderV2.PostProcessPage(page, IsFirstPage(report, page), IsLastPage(report, page), true);
                    StiPostProcessProviderV2.PostProcessPrimitives(page);
                }
            }
        }

        internal static bool IsFirstPage(StiReport report, StiPage page)
        {
            if (report.Engine.PageNumbers == null)
                return report.RenderedPages.IndexOf(page) < 1;

            var pageIndex = report.RenderedPages.IndexOf(page);
            pageIndex = Math.Max(pageIndex, 0);
            pageIndex = Math.Min(pageIndex, report.Engine.PageNumbers.PageNumbers.Count - 1);
            var pageNumber = report.Engine.PageNumbers.PageNumbers[pageIndex];

            return pageNumber.TotalPageCount == -1 || pageNumber.PageNumber == -1
                ? pageIndex == 0 || pageNumber.ResetPageNumber
                : pageNumber.PageNumber == 1;
        }

        internal static bool IsLastPage(StiReport report, StiPage page)
        {
            if (report.Engine.PageNumbers != null)
            {
                var pageIndex = report.RenderedPages.IndexOf(page);
                pageIndex = Math.Max(pageIndex, 0);
                pageIndex = Math.Min(pageIndex, report.Engine.PageNumbers.PageNumbers.Count - 1);
                var pageNumber = report.Engine.PageNumbers.PageNumbers[pageIndex];

                if (pageNumber.TotalPageCount == -1 || pageNumber.PageNumber == -1)
                {
                    return pageIndex == report.RenderedPages.Count - 1 ||
                           pageIndex + 1 < report.Engine.PageNumbers.PageNumbers.Count &&
                           report.Engine.PageNumbers.PageNumbers[pageIndex + 1].ResetPageNumber;
                }
                
                return pageNumber.PageNumber == pageNumber.TotalPageCount;
            }
            else
            {
                var pageNumber = report.RenderedPages.IndexOf(page);
                return pageNumber == report.RenderedPages.Count - 1;
            }
        }
		#endregion

		#region Methods.Pass
		private static void RenderFirstPass(StiReport report, StiNumberOfPass numberOfPass)
		{
		    if (numberOfPass == StiNumberOfPass.DoublePass && report.ReportPass == StiReportPass.None)
		    {
		        report.ReportPass = StiReportPass.First;
		        var storedEngine = report.Engine;
		        report.Engine = null;

		        var pagesCounter = report.RenderedPages.Count;
		        StiPagesCollection pagesStore = null;

		        if (pagesCounter > 0)
		        {
		            pagesStore = new StiPagesCollection(report);
		            pagesStore.AddRange(report.RenderedPages);
		            var tempPage = StiActivator.CreateObject("Stimulsoft.Report.Components.StiPage", new object[] { report }) as StiPage;

		            for (var index = 0; index < report.RenderedPages.Count; index++)
		            {
		                report.RenderedPages[index] = tempPage;
		            }
		        }

		        var storedCurrentPrintPage = report.CurrentPrintPage;

		        var tempEngine = new StiEngine(report);
		        tempEngine.PageNumbers.ClearPageNumbersOnFinish = false;
		        tempEngine.PageNumbers.PageNumbers = storedEngine.PageNumbers.PageNumbers;
		        tempEngine.ParserConversionStore = (Hashtable) storedEngine.ParserConversionStore.Clone();
                tempEngine.RenderingStartTicks = storedEngine.RenderingStartTicks;

                if (storedEngine.HashDataSourceReferencesCounter != null)
		            tempEngine.HashDataSourceReferencesCounter = (Hashtable) storedEngine.HashDataSourceReferencesCounter.Clone();

		        report.Engine = tempEngine;

		        try
		        {
		            report.Render();
		        }
		        catch
		        {
		            report.ReportPass = StiReportPass.None;
		            throw;
		        }

		        storedEngine.LatestProgressValue = report.Engine.LatestProgressValue;
		        report.Engine = storedEngine;
		        tempEngine.ParserConversionStore = null;
		        report.Engine.PageNumbers.ProcessPageNumbers();
		        report.ReportPass = StiReportPass.Second;
		        report.IsRendering = true;
		        report.IsRendered = false;

		        report.RenderedPages.Clear();
		        if (pagesCounter > 0)
		        {
		            report.RenderedPages.AddRange(pagesStore);
		            pagesStore.Clear();
		        }

		        report.CurrentPrintPage = storedCurrentPrintPage;
		    }

		    report.ResetAggregateFunctions();
		}

		internal static void ClearPagesForFirstPass(StiReport report)
		{
		    if (report.ReportPass == StiReportPass.First && report.RenderedPages.Count >= 2)
		        report.RenderedPages[report.RenderedPages.Count - 2].Components.Clear();
		}
		#endregion

		#region Methods
		private static StiNumberOfPass GetNumberOfPass(StiReport report)
		{
			var numberOfPass = report.NumberOfPass;

		    if (report.ReportPass == StiReportPass.None && numberOfPass == StiNumberOfPass.SinglePass)
			{
                if (!StiOptions.Engine.UseAdvancedPrintOnEngineV2)
                {
                    if (report.GetComponents().Cast<StiComponent>().Any(c => c is StiTableOfContents))
                        return StiNumberOfPass.DoublePass;

                    return StiNumberOfPass.SinglePass;
                }

				var comps = report.GetComponents();
				foreach (StiComponent comp in comps)
				{
					if (comp.PrintOn != StiPrintOnType.AllPages && 
						comp.PrintOn != StiPrintOnType.OnlyFirstPage &&
						comp.PrintOn != StiPrintOnType.ExceptFirstPage)
					{
						return StiNumberOfPass.DoublePass;
					}

                    if (comp is StiTableOfContents)
                        return StiNumberOfPass.DoublePass;
				}
			}

			return numberOfPass;
		}

		private static void MadeCollate(StiReport report)
		{
			if (report.RenderedPages.Count < 3 || report.Collate < 2)return;

			var list = new ArrayList();
            if (StiOptions.Engine.UseCollateOldMode)
            {
                for (var index = 0; index < report.Collate; index++)
                {
                    for (var pageIndex = index; pageIndex < report.RenderedPages.Count; pageIndex += report.Collate)
                    {
                        list.Add(report.RenderedPages.GetPageWithoutCache(pageIndex));
                    }
                }
            }
            else
            {
                var range = report.RenderedPages.Count / report.Collate;
                if (range * report.Collate < report.RenderedPages.Count) range++;
                for (var index = 0; index < range; index++)
                {
                    for (var pageIndex = index; pageIndex < report.RenderedPages.Count; pageIndex += range)
                    {
                        list.Add(report.RenderedPages.GetPageWithoutCache(pageIndex));
                    }
                }
            }

            for (var pageIndex = 0; pageIndex < report.RenderedPages.Count; pageIndex++)
            {
                report.RenderedPages.SetPageWithoutCache(pageIndex, list[pageIndex] as StiPage);
            }
        }

        private static void MadeMirrorMargins(StiReport report)
        {
            if (report.RenderedPages.Count < 2) return;

            for (var index = 1; index < report.RenderedPages.Count; index += 2)
            {
                var page = report.RenderedPages.GetPageWithoutCache(index);
                if (page.MirrorMargins)
                {
                    page.Margins = new StiMargins(page.Margins.Right, page.Margins.Left, page.Margins.Top, page.Margins.Bottom);
                }
            }
        }

		private static void InitReport(StiReport report)
		{
			report.CurrentPage = 0;

            if (report.SubReportsMasterReport == null) 
                report.CurrentPrintPage = 0;

            report.IsStopped = false;
            report.ResetAggregateFunctions();
		}

		private static void ClearTotals(StiReport report)
		{
			report.Totals.Clear();
			if (report.ParentReport != null)
			    report.ParentReport.Totals.Clear();
		}

		internal static void PrepareSubReportsAndDrillDownPages(StiReport report)
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

				if (comp.Interaction != null && comp.Interaction.DrillDownEnabled && comp.Interaction.DrillDownPageGuid != null)
				{
					var pg = guidToPage[comp.Interaction.DrillDownPageGuid] as StiPage;
					if (pg != null && !pg.DrillDownActivated)
					    pg.Skip = true;
				}

                if (comp is StiChart chart)
                {
                    foreach (IStiSeries series in chart.Series)
                    {
                        var interaction = series?.Interaction as StiSeriesInteraction;
                        if (interaction?.DrillDownPageGuid != null && interaction.DrillDownEnabled)
                        {
                            var pg = guidToPage[interaction.DrillDownPageGuid] as StiPage;
                            if (pg != null)
                                pg.Skip = true;

                            break;
                        }
                    }
                }
			}
		}

        internal static bool IsUsedAsDrillDownPage(StiPage page)
        {
            var comps = page.Report.GetComponents();

            if (comps.SelectAllSubReports()
                .Any(s => s.SubReportPageGuid == page.Guid))
            {
                return true;
            }

            if (comps.ToList().Any(c =>
                c.Interaction != null &&
                c.Interaction.DrillDownEnabled &&
                c.Interaction.DrillDownPageGuid == page.Guid))
            {
                return true;
            }

            if (comps.SelectAllCharts()
                .SelectMany(c => c.Series.ToList())
                .Any(c =>
                    c.Interaction is StiSeriesInteraction seriesInteraction &&
                    c.Interaction.DrillDownEnabled &&
                    seriesInteraction.DrillDownPageGuid == page.Guid))
            {
                return true;
            }

            return false;
        }

 		private static void RenderReport(StiReport report, StiReport masterReport, StiRenderState state)
		{
            try
            {
                var pageIndex = 0;
                foreach (StiPage page in report.Pages)
                {
                    var storeEnabled = page.Enabled;
                    try
                    {
                        report.Engine.TemplatePage = page;
                        report.Engine.TemplateContainer = page;

                        report.CurrentPrintPage++;

                        if (!page.Skip)
                        {
                            page.InvokeBeforePrint(page, EventArgs.Empty);
                            report.Engine.SkipFirstPageBeforePrintEvent = true;
                        }
                        report.CurrentPrintPage--;

                        if (!page.Enabled || page.Skip)
                        {
                            if (!page.Skip)
                            {
                                report.Engine.SkipFirstPageBeforePrintEvent = false;
                                page.InvokeAfterPrint(page, EventArgs.Empty);
                            }
                            continue;
                        }

                        if (report.Engine.MasterEngine != null)
                        {
                            report.Engine.MasterEngine.TemplatePage = page;
                            report.Engine.MasterEngine.TemplateContainer = page;
                        }

                        for (var indexPage = 0; indexPage < page.NumberOfCopies; indexPage++)
                        {
                            report.PageCopyNumber = indexPage + 1;

                            #region Render Page
                            if (indexPage > 0)
                            {
                                page.PageInfoV2.RenderedCount = 0;

                                #region Clean PageBreakSkipList
                                foreach (StiComponent comp in page.GetComponents())
                                {
                                    var ibreak = comp as IStiPageBreak;
                                    if (ibreak != null && ibreak.NewPageBefore && ibreak.SkipFirst)
                                        report.Engine.RemoveBandFromPageBreakSkipList(ibreak);
                                }
                                #endregion
                            }

                            page.PageInfoV2.IndexOfStartRenderedPages = report.RenderedPages.Count;

                            //fix 08.07.2013
                            //StiPageHelper.PrepareBookmark(page);
                            page.ParentBookmark = page.Report.Bookmark;
                            page.CurrentBookmark = page.ParentBookmark;

                            page.ParentPointer = page.Report.Pointer;
                            page.CurrentPointer = page.ParentPointer;

                            /*If it is a first page or the PrintOnPreviousPage property is false
                             * then generate a new page.*/
                            if (report.RenderedPages.Count == 0 || (!page.PrintOnPreviousPage))
                            {
                                report.Engine.FirstCallNewPage = true;
                                report.Engine.NewPage();
                            }
                            /* If a new page is not added, this means that it is necessary to print on the pervious page. */
                            else
                            {
                                #region PrintOnPreviousPage
                                if (page.PrintOnPreviousPage && pageIndex != 0)
                                {
                                    //Get the last rendered page 
                                    var mainPage = report.RenderedPages[report.RenderedPages.Count - 1];

                                    #region Find the TAG containers which contain pointers to page's parts
                                    StiContainer cont = null;
                                    var str = $"TAG##{report.Engine.ColumnsOnPanel.CurrentColumn}";
                                    foreach (StiComponent comp in mainPage.Components)
                                    {
                                        if (comp.Name == str)
                                            cont = comp as StiContainer;
                                    }
                                    #endregion

                                    #region If the TAG container is found then correct its height
                                    if (cont != null)
                                        cont.Height = report.Engine.PositionY - cont.Top;
                                    else
                                    {
                                        #region Create a container
                                        var tagMainContainer = new StiContainer
                                        {
                                            Name = $"TAG##{report.Engine.ColumnsOnPanel.CurrentColumn}",
                                            Top = 0,
                                            Height = report.Engine.PositionY,
                                            Left = (report.Engine.ColumnsOnPanel.CurrentColumn - 1) * mainPage.GetColumnWidth(),
                                            Width = mainPage.Columns > 0 ? mainPage.GetColumnWidth() : mainPage.Width,
                                            TagValue = mainPage.TagValue,
                                            PointerValue = mainPage.PointerValue,
                                            BookmarkValue = mainPage.BookmarkValue,
                                            HyperlinkValue = mainPage.HyperlinkValue,
                                            ToolTipValue = mainPage.ToolTipValue,
                                            Guid = mainPage.Guid
                                        };
                                        #endregion

                                        #region Remove values on the previous page
                                        mainPage.TagValue = null;
                                        mainPage.PointerValue = null;
                                        mainPage.BookmarkValue = null;
                                        mainPage.HyperlinkValue = null;
                                        mainPage.ToolTipValue = null;
                                        mainPage.Guid = null;
                                        #endregion

                                        report.Engine.AddContainerToDestination(tagMainContainer);
                                    }
                                    #endregion

                                    #region Create a container for the next page
                                    var tagContainer = new StiContainer
                                    {
                                        Name = $"TAG##{report.Engine.ColumnsOnPanel.CurrentColumn}",
                                        Top = report.Engine.PositionY,
                                        Height = report.Engine.PositionBottomY - report.Engine.PositionY,
                                        Left = (report.Engine.ColumnsOnPanel.CurrentColumn - 1) * page.GetColumnWidth(),
                                        Width = page.Columns > 0 ? page.GetColumnWidth() : page.Width,
                                        TagValue = page.TagValue,
                                        PointerValue = page.PointerValue,
                                        BookmarkValue = page.BookmarkValue,
                                        HyperlinkValue = page.HyperlinkValue,
                                        ToolTipValue = page.ToolTipValue,
                                        Guid = page.Guid
                                    };
                                    #endregion

                                    #region Remove values on the previous page
                                    page.TagValue = null;
                                    page.PointerValue = null;
                                    page.BookmarkValue = null;
                                    page.HyperlinkValue = null;
                                    page.ToolTipValue = null;
                                    //In report with two pages, second page PrintOnPreviousPage = true and DoublePass
                                    //this line of code generate error, becuase report engine can't find gui
                                    //page.Guid = null;
                                    #endregion

                                    report.Engine.AddContainerToDestination(tagContainer);

                                    #region Render CrossPrimitives
                                    foreach (StiComponent comp in page.Components)
                                    {
                                        var line = comp as StiCrossLinePrimitive;
                                        if (line != null && line.Guid != null && line.Guid != nullGuid)
                                        {
                                            var newLineComp = line.Render();
                                            mainPage.Components.Add(newLineComp);
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }

                            StiPageHelper.RenderPage(page);
                            #endregion
                        }
                    }
                    catch (StiStopBeforePrintException)
                    {
                    }
                    finally
                    {
                        page.Enabled = storeEnabled;
                    }

                    report.Engine.ProcessLastPageAfterRendering();

                    StiContainer tempCont = null;
                    report.Engine.RenderFootersOnAllPages(null, 0, ref tempCont);
                    report.Engine.RenderPrintAtBottom(null, 0, null);
                    report.Engine.RenderEmptyBands(report.Engine.ContainerForRender, null);
                    report.Engine.EmptyBands.Clear();

                    pageIndex++;
                }

                //for last page
                report.Engine.InvokePageAfterPrint();
                RenderTable(report);
                report.Engine.FinalClear();

                report.ProgressOfRendering = 100;
            }
            catch (StiReportRenderingStopException)
            {
            }
            #if CLOUD
            catch (StiMaxDataRowsException)
            {
            }
            #endif
            catch (StiStopBeforePageException)
            {
                report.Engine.PageNumbers.PageNumbers.RemoveAt(report.Engine.PageNumbers.PageNumbers.Count - 1);
            }
        }

        private static void RenderTable(StiReport report)
        {
            if (!report.ContainsTables) return;

            var ReportPageCollection = new Hashtable();
            var ReportHashWidths = new Hashtable();
            var ReportPageListForTalbes = new Hashtable();
            var ReportTableList = new ArrayList();
            Graphics g = null;
            var GridSize = 0.2;

            // (AutoWidth == (StiTableAutoWidth.Page || StiTableAutoWidth.None))
            foreach (StiPage page in report.RenderedPages)
            {
                GridSize = page.GridSize;
                var PageCollection = new Hashtable();
                var HashWidths = new Hashtable();
                var TableList = new ArrayList();

#region Заполняем компонентами
                g = page.GetMeasureGraphics();
                var coll = page.GetComponents();
                for (var indexEl = 0; indexEl < coll.Count; indexEl++)
                {
                    var cell = coll[indexEl] as IStiTableCell;
                    if (cell != null)
                    {
#region Разбиваем по таблицам
                        Hashtable tableCollection = null;
                        var table = cell.TableTag as StiTable;
                        if (table.AutoWidth == StiTableAutoWidth.None) continue;

                        if (table.AutoWidth == StiTableAutoWidth.Table)
                        {
                            if (!ReportPageCollection.ContainsKey(table))
                            {
                                tableCollection = new Hashtable();
                                ReportPageCollection.Add(table, tableCollection);
                                ReportTableList.Add(table);
                            }
                            else
                            {
                                tableCollection = ReportPageCollection[table] as Hashtable;
                            }

#region Add pages
                            if (!ReportPageListForTalbes.ContainsKey(table.Name))
                            {
                                var pagesForTableCollection = new ArrayList
                                {
                                    page
                                };
                                ReportPageListForTalbes.Add(table.Name, pagesForTableCollection);
                            }
                            else
                            {
                                if (!((ArrayList) ReportPageListForTalbes[table.Name]).Contains(page))
                                    ((ArrayList) ReportPageListForTalbes[table.Name]).Add(page);
                            }
#endregion
                        }
                        else
                        {
                            if (!PageCollection.ContainsKey(table))
                            {
                                tableCollection = new Hashtable();
                                PageCollection.Add(table, tableCollection);
                                TableList.Add(table);
                            }
                            else
                            {
                                tableCollection = PageCollection[table] as Hashtable;
                            }
                        }
#endregion

#region разбиваем по столбцам
                        if (!tableCollection.ContainsKey(cell.Column))
                        {
                            var array = new ArrayList();
                            tableCollection.Add(cell.Column, array);
                        }

                        ((ArrayList) tableCollection[cell.Column]).Add(cell);
#endregion
                    }
                }
#endregion

#region Проверяем все Width и выбираем максимальный
                foreach (Hashtable hashTable in PageCollection.Values)
                {
                    var widths = new StiColumnSize(hashTable.Count);
                    var keys = new object[hashTable.Count];
                    hashTable.Keys.CopyTo(keys, 0);
                    Array.Sort(keys);
                    for (var indexCol = 0; indexCol < hashTable.Count; indexCol++)
                    {
                        var list = hashTable[keys[indexCol]] as ArrayList;
                        if (list == null) continue;

                        double maxWidth = 0;
                        foreach (StiComponent comp in list)
                        {
#region если FixedWidth=true
                            if (((IStiTableCell) comp).FixedWidth)
                            {
                                widths.SetFixedColumn(indexCol, comp.Width);
                                maxWidth = -1;
                                break;
                            }
#endregion

                            switch (((IStiTableCell) comp).CellType)
                            {
                                case StiTablceCellType.CheckBox:
                                case StiTablceCellType.RichText:
                                    if (comp.Width > maxWidth) maxWidth = comp.Width;
                                    break;

                                case StiTablceCellType.Image:
                                    var image = comp as StiTableCellImage;
                                    var canShrink = image.CanShrink;
                                    image.CanShrink = true;
                                    var imageWidth = image.GetRealSize().Width;
                                    image.CanShrink = canShrink;
                                    if (imageWidth > maxWidth)
                                        maxWidth = imageWidth;
                                    break;

                                case StiTablceCellType.Text:
                                    var cellText = comp as StiTableCell;
                                    if (cellText.WordWrap)
                                    {
                                        if (cellText.Width > maxWidth) maxWidth = cellText.Width;
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(cellText.Text.Value))
                                        {
                                            var size = g.MeasureString(cellText.Text.Value, cellText.Font);
                                            var lengthMargin = cellText.Margins.Left + cellText.Margins.Right;
                                            var cellTextWidth = report.Unit.ConvertFromHInches(size.Width + lengthMargin);
                                            if (cellTextWidth > maxWidth) maxWidth = cellTextWidth;
                                        }
                                    }

                                    break;
                            }
                        }

                        if (maxWidth != -1)
                            widths.SetWidth(indexCol, maxWidth);
                    }

                    var arrayList = hashTable[0] as ArrayList;
                    HashWidths.Add(((IStiTableCell) arrayList[0]).TableTag, widths);
                }
#endregion

#region Подгоняем размеры под размер страницы
                for (var indexKey = 0; indexKey < TableList.Count; indexKey++)
                {
                    var table = TableList[indexKey] as StiTable;
                    if (table == null) continue;

                    var tableWidths = (StiColumnSize) HashWidths[table];
                    double sumWidth = 0;
                    for (var indexWidth = 0; indexWidth < tableWidths.Length; indexWidth++)
                        sumWidth += tableWidths.GetWidth(indexWidth);

                    switch (table.AutoWidthType)
                    {
                        case StiTableAutoWidthType.None:
                            if (page.Width < sumWidth)
                            {
                                var segmentCount = (int) (sumWidth / page.Width) + 1;
                                page.SegmentPerWidth = segmentCount;
                            }

                            break;

                        case StiTableAutoWidthType.FullTable:

#region Если есть лишнее место
                            if (sumWidth < table.Width)
                            {
                                var rest2 = table.Width - sumWidth;
                                rest2 /= tableWidths.GetCountNotFixedColumn();
                                for (var index = 0; index < tableWidths.Length; index++)
                                    tableWidths.Add(index, rest2);
                            }
#endregion

#region Если места не хватает
                            if (sumWidth > table.Width)
                            {
                                var rest3 = sumWidth - table.Width;
                                rest3 /= tableWidths.GetCountNotFixedColumn();
                                for (var index = 0; index < tableWidths.Length; index++)
                                    tableWidths.Subtract(index, rest3);
                            }
#endregion

                            break;

                        case StiTableAutoWidthType.LastColumns:

                            var rest = Math.Abs(table.Width - sumWidth);
                            var numberColumn = tableWidths.Length - 1;
                            var finish = false;
                            var minSizeCell = GridSize * 3;

                            if (sumWidth > table.Width)
                            {
                                while (!finish)
                                {
#region FixedWidth=true
                                    if (tableWidths.GetFixed(numberColumn))
                                    {
                                        if (numberColumn == 0)
                                            finish = true;
                                        else
                                            numberColumn--;
                                    }
#endregion

                                    else

#region FixedWidth=false
                                    {
                                        if (tableWidths.GetWidth(numberColumn) - minSizeCell < rest)
                                        {
                                            var widthREST = tableWidths.GetWidth(numberColumn) - minSizeCell;
                                            tableWidths.Subtract(numberColumn, widthREST);
                                            rest -= widthREST;
                                            if (numberColumn == 0)
                                                finish = true;
                                            else
                                                numberColumn--;
                                        }
                                        else
                                        {
                                            tableWidths.Subtract(numberColumn, rest);
                                            finish = true;
                                        }
                                    }
#endregion
                                }
                            }
                            else
                                tableWidths.AddLastNotFixed(rest);

                            break;
                    }
                }
#endregion

#region Задаем новые размеры и положение компонентам
                foreach (var key in PageCollection.Keys)
                {
                    var hashTable = PageCollection[key] as Hashtable;
                    var widths = (StiColumnSize) HashWidths[key];
                    widths.Normalize();
                    var keys = new object[hashTable.Count];
                    hashTable.Keys.CopyTo(keys, 0);
                    Array.Sort(keys);
                    double posX = 0;
                    for (var indexCol = 0; indexCol < hashTable.Count; indexCol++)
                    {
                        var list = hashTable[keys[indexCol]] as ArrayList;
                        if (list == null) return;

                        foreach (StiComponent comp in list)
                        {
                            comp.Width = widths.GetWidth(indexCol);
                            comp.Left = posX;
                        }

                        posX += widths.GetWidth(indexCol);
                    }
                }
#endregion
            }

            if (ReportTableList.Count <= 0) return;

#region Проверяем все Width и выбираем максимальный
            foreach (Hashtable hashTable in ReportPageCollection.Values)
            {
                var widths = new StiColumnSize(hashTable.Count);
                var keys = new object[hashTable.Count];
                hashTable.Keys.CopyTo(keys, 0);
                Array.Sort(keys);
                for (var indexCol = 0; indexCol < hashTable.Count; indexCol++)
                {
                    var list = hashTable[keys[indexCol]] as ArrayList;
                    if (list == null) continue;

                    double maxWidth = 0;
                    foreach (StiComponent comp in list)
                    {
#region если FixedWidth=true
                        if (((IStiTableCell) comp).FixedWidth)
                        {
                            widths.SetFixedColumn(indexCol, comp.Width);
                            maxWidth = -1;
                            break;
                        }
#endregion

                        switch (((IStiTableCell) comp).CellType)
                        {
                            case StiTablceCellType.CheckBox:
                            case StiTablceCellType.RichText:
                                if (comp.Width > maxWidth)
                                    maxWidth = comp.Width;
                                break;

                            case StiTablceCellType.Image:
                                var image = comp as StiTableCellImage;
                                var canShrink = image.CanShrink;
                                image.CanShrink = true;
                                var imageWidth = image.GetRealSize().Width;
                                image.CanShrink = canShrink;

                                if (imageWidth > maxWidth)
                                    maxWidth = imageWidth;
                                break;

                            case StiTablceCellType.Text:
                                var cellText = comp as StiTableCell;
                                if (cellText.WordWrap)
                                {
                                    if (cellText.Width > maxWidth)
                                        maxWidth = cellText.Width;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(cellText.Text.Value))
                                    {
                                        var size = g.MeasureString(cellText.Text.Value, cellText.Font);
                                        var lengthMargin = cellText.Margins.Left + cellText.Margins.Right;

                                        var cellTextWidth = report.Unit.ConvertFromHInches(size.Width + lengthMargin);
                                        if (cellTextWidth > maxWidth)
                                            maxWidth = cellTextWidth;
                                    }
                                }

                                break;
                        }
                    }

                    if (maxWidth != -1)
                        widths.SetWidth(indexCol, maxWidth);
                }

                var arrayList = hashTable[0] as ArrayList;
                ReportHashWidths.Add(((IStiTableCell) arrayList[0]).TableTag, widths);
            }
#endregion

#region Подгоняем размеры под размер страницы
            foreach (StiTable table in ReportTableList)
            {
                if (table == null) continue;

                var tableWidths = (StiColumnSize) ReportHashWidths[table];
                double sumWidth = 0;
                for (var indexWidth = 0; indexWidth < tableWidths.Length; indexWidth++)
                    sumWidth += tableWidths.GetWidth(indexWidth);

                switch (table.AutoWidthType)
                {
                    case StiTableAutoWidthType.None:

                        var tablePages = ReportPageListForTalbes[table.Name] as ArrayList;
                        if (tablePages != null)
                        {
                            if (((StiPage) tablePages[0]).Width < sumWidth)
                            {
                                var segmentCount = (int) (sumWidth / ((StiPage) tablePages[0]).Width) + 1;
                                foreach (StiPage tablePage in tablePages)
                                {
                                    tablePage.SegmentPerWidth = segmentCount;
                                }
                            }
                        }

                        break;

                    case StiTableAutoWidthType.FullTable:

#region Если есть лишнее место
                        if (sumWidth < table.Width)
                        {
                            var rest2 = table.Width - sumWidth;
                            rest2 /= tableWidths.GetCountNotFixedColumn();
                            for (var index = 0; index < tableWidths.Length; index++)
                                tableWidths.Add(index, rest2);
                        }
#endregion

#region Если места не хватает
                        if (sumWidth > table.Width)
                        {
                            var rest3 = sumWidth - table.Width;
                            rest3 /= tableWidths.GetCountNotFixedColumn();
                            for (var index = 0; index < tableWidths.Length; index++)
                                tableWidths.Subtract(index, rest3);
                        }
#endregion

                        break;

                    case StiTableAutoWidthType.LastColumns:

                        var rest = Math.Abs(table.Width - sumWidth);
                        var numberColumn = tableWidths.Length - 1;
                        var finish = false;
                        var minSizeCell = GridSize * 3;

                        if (sumWidth > table.Width)
                        {
                            while (!finish)
                            {
#region FixedWidth=true
                                if (tableWidths.GetFixed(numberColumn))
                                {
                                    if (numberColumn == 0)
                                        finish = true;
                                    else
                                        numberColumn--;
                                }
#endregion

                                else

#region FixedWidth=false
                                {
                                    if (tableWidths.GetWidth(numberColumn) - minSizeCell < rest)
                                    {
                                        var widthRest = tableWidths.GetWidth(numberColumn) - minSizeCell;
                                        tableWidths.Subtract(numberColumn, widthRest);
                                        rest -= widthRest;
                                        if (numberColumn == 0)
                                            finish = true;
                                        else
                                            numberColumn--;
                                    }
                                    else
                                    {
                                        tableWidths.Subtract(numberColumn, rest);
                                        finish = true;
                                    }
                                }
#endregion
                            }
                        }
                        else
                            tableWidths.AddLastNotFixed(rest);

                        break;
                }
            }
#endregion

#region Задаем новые размеры и положение компонентам
            foreach (var key in ReportPageCollection.Keys)
            {
                var hashTable = ReportPageCollection[key] as Hashtable;
                var keys = new object[hashTable.Count];
                hashTable.Keys.CopyTo(keys, 0);
                Array.Sort(keys);
                var widths = (StiColumnSize) ReportHashWidths[key];
                widths.Normalize();
                double posX = 0;
                for (var indexCol = 0; indexCol < hashTable.Count; indexCol++)
                {
                    var list = hashTable[keys[indexCol]] as ArrayList;
                    if (list == null) return;

                    foreach (StiComponent comp in list)
                    {
                        comp.Width = widths.GetWidth(indexCol);
                        comp.Left = posX;
                    }

                    posX += widths.GetWidth(indexCol);
                }
            }
#endregion
        }

        private static void FinishProgressForm(StiReport report)
		{
		    if (report.ReportPass == StiReportPass.First) return;

		    report.StatusString = StiLocalization.CultureName == "en" 
		        ? "Finishing Report" 
		        : StiLocalization.Get("Report", "FinishingReport");

		    if (report.Progress != null)
		    {
		        report.Progress.HideProgressBar();
		        report.Progress.SetAllowClose(false);
		    }
		}
#endregion
	}
}
