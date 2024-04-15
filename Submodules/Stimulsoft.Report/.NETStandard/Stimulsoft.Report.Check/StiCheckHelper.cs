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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace Stimulsoft.Report.Check
{
    public sealed class StiCheckHelper
    {
        #region Properties
        public int ErrorsCount { get; private set; }

        public int WarningsCount { get; private set; }

        public int InformationMessagesCount { get; private set; }

        public int ReportRenderingMessagesCount { get; private set; }

        public List<StiCheck> Checks { get; private set; }

        public List<StiCheck> ReportRenderingMessagesChecks { get; private set; }

        public bool IsMessagesPresent
        {
            get
            {
                return
                    ErrorsCount > 0 ||
                    InformationMessagesCount > 0 ||
                    ReportRenderingMessagesCount > 0 ||
                    WarningsCount > 0;
            }
        }
        #endregion

        #region Methods
        public void BuildChecks(StiReport report)
        {
            string targetInvocationException = null;
            string otherException = null;

            #region Compile report if required
#if !NETSTANDARD && !NETCOREAPP
            if (report.CompiledReport == null && report.CalculationMode == StiCalculationMode.Compilation)
#else
            if (report.CompiledReport == null && report.CheckNeedsCompiling())
#endif
            {
                if (report.CompilerResults == null || report.CompilerResults.Errors.Count == 0)
                {
                    try
                    {
                        report.Compile();
                    }
                    catch(Exception ex)
                    {
                        var targetEx = ex as TargetInvocationException;
                        if (targetEx != null)
                        {
                            var targetSite = string.Empty;
                            if (targetEx.InnerException.TargetSite != null)
                                targetSite = "TargetSite: " + targetEx.InnerException.TargetSite.Name;

                            targetInvocationException = $"{targetEx.Message} {targetEx.InnerException.Message} ({targetSite})";
                        }
                        else
                        {
                            otherException = ex.Message;
                        }
                    }
                }
            }
            #endregion

            var engine = new StiCheckEngine();
            Checks = engine.CheckReport(report);

            if (targetInvocationException != null)
            {
                Checks.Add(new StiCompilationErrorCheck
                {
                    Element = report,
                    Error = new CompilerError("", 0, 0, "", $"TargetInvocationException: '{targetInvocationException}'")
                });
            }
            else if (otherException != null)
            {
                Checks.Add(new StiCompilationErrorCheck
                {
                    Element = report,
                    Error = new CompilerError("", 0, 0, "", otherException)
                });
            }

            ErrorsCount = 0;
            WarningsCount = 0;
            InformationMessagesCount = 0;

            foreach (StiCheck check in Checks)
            {
                switch (check.Status)
                {
                    case StiCheckStatus.Error:
                        ErrorsCount++;
                        break;

                    case StiCheckStatus.Warning:
                        WarningsCount++;
                        break;

                    case StiCheckStatus.Information:
                        InformationMessagesCount++;
                        break;
                }
            }
        }

        public void BuildReportRenderingMessages(StiReport report)
        {
            ReportRenderingMessagesCount = 0;
            ReportRenderingMessagesChecks = new List<StiCheck>();

            report = report.CompiledReport != null ? report.CompiledReport : report;
            if (report.ReportRenderingMessages != null && report.ReportRenderingMessages.Count > 0)
            {
                foreach (string message in report.ReportRenderingMessages)
                {
                    var check = new StiReportRenderingMessageCheck();
                    check.SetMessage(message);

                    Checks.Add(check);
                    ReportRenderingMessagesChecks.Add(check);
                }
                ReportRenderingMessagesCount = report.ReportRenderingMessages.Count;
            }
        }
        #endregion
    }
}