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
using Stimulsoft.Report.Events;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// A class which controls of settings of the report engine. 
        /// </summary>
		public sealed partial class Engine
        {
            public sealed partial class GlobalEvents
            {
                #region ReportNameChanged
                public static event EventHandler ReportNameChanged;
                public static void InvokeReportNameChanged(StiReport report, EventArgs e)
                {
                    ReportNameChanged?.Invoke(report, e);
                }
                #endregion

                #region ReportCacheCreated
                /// <summary>
                /// Occurs when new report cache is created.
                /// </summary>
                public static event EventHandler ReportCacheCreated;

                public static void InvokeReportCacheCreated(object sender, EventArgs e)
                {
                    ReportCacheCreated?.Invoke(sender, e);
                }
                #endregion

                #region ReportSynchronized
                public static event EventHandler ReportSynchronized;

                public static void InvokeReportSynchronized(object sender, EventArgs e)
                {
                    ReportSynchronized?.Invoke(sender, e);
                }
                #endregion

				#region ReportExporting
				public static event EventHandler ReportExporting;

				public static void InvokeReportExporting(object sender, StiExportEventArgs e)
				{
                    ReportExporting?.Invoke(sender, e);
                }
				#endregion

				#region ReportExported
				public static event EventHandler ReportExported;

				public static void InvokeReportExported(object sender, StiExportEventArgs e)
				{
                    ReportExported?.Invoke(sender, e);
                }
				#endregion

				#region ReportPrinting
				public static event EventHandler ReportPrinting;

				public static void InvokeReportPrinting(object sender, EventArgs e)
				{
                    ReportPrinting?.Invoke(sender, e);
                }
				#endregion

				#region ReportPrinted
				public static event EventHandler ReportPrinted;

				public static void InvokeReportPrinted(object sender, EventArgs e)
				{
                    ReportPrinted?.Invoke(sender, e);
                }
				#endregion

                #region ReportPrintingPage
                public static event EventHandler ReportPrintingPage;

                public static void InvokeReportPrintingPage(object sender, EventArgs e)
                {
                    ReportPrintingPage?.Invoke(sender, e);
                }
                #endregion

                #region ReportPrintedPage
                public static event EventHandler ReportPrintedPage;

                public static void InvokeReportPrintedPage(object sender, EventArgs e)
                {
                    ReportPrintedPage?.Invoke(sender, e);
                }
                #endregion

                #region ReportRefreshing
                public static event EventHandler ReportRefreshing;

                public static void InvokeReportRefreshing(object sender, EventArgs e)
                {
                    ReportRefreshing?.Invoke(sender, e);
                }
                #endregion

                #region ReportGetSubReport
                public static event StiGetSubReportEventHandler ReportGetSubReport;

				public static void InvokeReportGetSubReport(object sender, StiGetSubReportEventArgs e)
				{
                    ReportGetSubReport?.Invoke(sender, e);
                }
				#endregion

				#region ReportLoading
				public static event EventHandler ReportLoading;

				public static void InvokeReportLoading(object sender, EventArgs e)
				{
                    ReportLoading?.Invoke(sender, e);
                }
				#endregion

				#region ReportLoaded
				public static event EventHandler ReportLoaded;

				public static void InvokeReportLoaded(object sender, EventArgs e)
				{
                    ReportLoaded?.Invoke(sender, e);
                }
				#endregion

				#region ReportSaving
				public static event EventHandler ReportSaving;

				public static void InvokeReportSaving(object sender, EventArgs e)
				{
                    ReportSaving?.Invoke(sender, e);
                }
				#endregion

				#region ReportSaved
				public static event EventHandler ReportSaved;

				public static void InvokeReportSaved(object sender, EventArgs e)
				{
                    ReportSaved?.Invoke(sender, e);
                }
				#endregion
                
				#region ReportCompiling
				public static event EventHandler ReportCompiling;

				public static void InvokeReportCompiling(object sender, EventArgs e)
				{
                    ReportCompiling?.Invoke(sender, e);
                }
				#endregion

				#region ReportCompiled
				public static event EventHandler ReportCompiled;

				public static void InvokeReportCompiled(object sender, EventArgs e)
				{
                    ReportCompiled?.Invoke(sender, e);
                }
				#endregion

				#region ReportBeginRender
				public static event EventHandler ReportBeginRender;

				public static void InvokeReportBeginRender(object sender, EventArgs e)
				{
                    ReportBeginRender?.Invoke(sender, e);
                }
				#endregion

				#region ReportRendering
				public static event EventHandler ReportRendering;

				public static void InvokeReportRendering(object sender, EventArgs e)
				{
                    ReportRendering?.Invoke(sender, e);
                }
				#endregion

				#region ReportEndRender
				public static event EventHandler ReportEndRender;

				public static void InvokeReportEndRender(object sender, EventArgs e)
				{
                    ReportEndRender?.Invoke(sender, e);
                }
                #endregion

                #region ReportDisposed
                public static event EventHandler ReportDisposed;
                public static void InvokeReportDisposed(StiReport report, EventArgs e)
                {
                    ReportDisposed?.Invoke(report, e);
                }
                #endregion
            }
        }
    }
}