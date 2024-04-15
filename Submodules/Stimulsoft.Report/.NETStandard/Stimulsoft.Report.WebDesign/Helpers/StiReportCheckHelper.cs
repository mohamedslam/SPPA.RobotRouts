#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using Stimulsoft.Report.Check;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiReportCheckHelper
    {
        #region Items
        private static Hashtable CheckItem(StiCheck check)
        {
            Hashtable checkObject = new Hashtable();
            checkObject["actions"] = GetActions(check);
            checkObject["defaultStateEnabled"] = check.DefaultStateEnabled;
            checkObject["element"] = check.Element != null ? check.Element.GetType().Name : null;
            checkObject["elementName"] = check.ElementName;
            checkObject["enabled"] = check.Enabled;
            checkObject["longMessage"] = check.LongMessage;
            checkObject["objectType"] = check.ObjectType;
            checkObject["previewVisible"] = check.PreviewVisible;
            checkObject["shortMessage"] = check.ShortMessage;
            checkObject["status"] = check.Status;

            return checkObject;
        }

        private static Hashtable CheckActionItem(Check.StiAction action)
        {
            Hashtable actionObject = new Hashtable();
            actionObject["name"] = action.Name;
            actionObject["description"] = action.Description;

            return actionObject;
        }
        #endregion

        #region Helper Methods
        private static ArrayList GetActions(StiCheck check)
        {
            ArrayList actions = new ArrayList();
            foreach (Check.StiAction action in check.Actions)
            {
                actions.Add(CheckActionItem(action));
            }

            return actions;
        }

        public static ArrayList GetChecksJSCollection(List<StiCheck> checks)
        {
            ArrayList checkItems = new ArrayList();
            if (checks != null)
            {
                foreach (var check in checks)
                {
                    checkItems.Add(CheckItem(check));
                }
            }

            return checkItems;
        }

        private static void UpdateCurrentReport(StiReport report, Hashtable parameters, Hashtable callbackResult)
        {
            callbackResult["selectedObjectName"] = parameters["selectedObjectName"];
            report.Info.Zoom = StiReportEdit.StrToDouble((string)parameters["zoom"]);
            if (parameters["reportFile"] != null) report.ReportFile = (string)parameters["reportFile"];
            callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(report);
            callbackResult["reportGuid"] = parameters["reportGuid"];
        }

        private static void CreateImage(StiCheck check, out Bitmap pixelElement, out Bitmap pixelHighlightedElement)
        {
            pixelElement = null;
            pixelHighlightedElement = null;

            if (!check.PreviewVisible) return;

            Image image;
            Image highlightedImage;

            check.CreatePreviewImage(out image, out highlightedImage);
            if (image != null)
                pixelElement = new Bitmap(image);

            if (highlightedImage != null)
                pixelHighlightedElement = new Bitmap(highlightedImage);
        }

        private static int GetErrorsCount(List<StiCheck> checks)
        {
            int errorsCount = 0;
            foreach (StiCheck check in checks)
            {
                if (check.Status == StiCheckStatus.Error) errorsCount++;
            }

            return errorsCount;
        }

        private static void BuildReportRenderingMessages(StiReport report, List<StiCheck> checks)
        {
            report = report.CompiledReport != null ? report.CompiledReport : report;
            if (report.ReportRenderingMessages != null && report.ReportRenderingMessages.Count > 0)
            {
                foreach (string message in report.ReportRenderingMessages)
                {
                    StiReportRenderingMessageCheck check = new StiReportRenderingMessageCheck();
                    check.SetMessage(message);

                    checks.Add(check);
                }
            }
        }
        #endregion

        #region Callback Methods
        public static List<StiCheck> CheckReport(StiRequestParams requestParams, StiReport report, bool createReportCopy)
        {            
            bool compileReport = true;

            #region Compile report
            string targetInvocationException = null;
            if (compileReport && (report.CalculationMode == StiCalculationMode.Compilation) && !StiOptions.Engine.ForceInterpretationMode)
            {
                try
                {
                    report.Compile();
                }
                catch (Exception ex)
                {
                    var targetEx = ex as TargetInvocationException;
                    if (targetEx != null)
                    {
                        string targetSite = string.Empty;
                        if (targetEx.InnerException.TargetSite != null)
                            targetSite = "TargetSite: " + targetEx.InnerException.TargetSite.Name;
                        targetInvocationException = string.Format("{0} {1} ({2})", targetEx.Message, targetEx.InnerException.Message, targetSite);
                    }
                }
            }
            #endregion

            var engine = new StiCheckEngine();
            var checks = engine.CheckReport(report);

            #region Check Compilation Errors
            if (targetInvocationException != null)
            {
                var check = new StiCompilationErrorCheck
                {
                    Element = report,
                    Error = new CompilerError("", 0, 0, "", string.Format("TargetInvocation exception: '{0}'", targetInvocationException))
                };
                checks.Add(check);
            }
            #endregion

            #region Add Report Rendering Messages
            if (GetErrorsCount(checks) == 0)
            {
                #region Compiling
                if (report.CompiledReport != null)
                {
                    if (!report.ContainsOnlyDashboard)
                        try
                        {
                            report.Render(false);
                        }
                        catch { }
                    BuildReportRenderingMessages(report, checks);
                }
                #endregion

                #region Interpretation
                else if (report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    var reportForViewer = report;

                    #region Create copy of report
                    if (createReportCopy)
                    {
                        var ms = new MemoryStream();
                        report.Save(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        reportForViewer = new StiReport();
                        reportForViewer.Load(ms);
                        reportForViewer.RegData(report.DataStore);
                        reportForViewer.RegBusinessObject(report.BusinessObjectsStore);
                        reportForViewer.RenderedPages.Add(new Stimulsoft.Report.Components.StiPage(reportForViewer));
                    }
                    #endregion

                    if (!report.ContainsOnlyDashboard)
                        try
                        {
                            reportForViewer.Render(false);
                        }
                        catch { }
                    BuildReportRenderingMessages(reportForViewer, checks);
                }
                #endregion
            }
            #endregion

            return checks;
        }

        public static void GetCheckPreview(StiRequestParams requestParams, StiReport report, Hashtable parameters, Hashtable callbackResult)
        {
            List<StiCheck> checks = CheckReport(requestParams, report, true) as List<StiCheck>;
            int checkIndex = Convert.ToInt32(parameters["checkIndex"]);

            if (checks != null && checkIndex < checks.Count)
            {
                StiCheck check = checks[checkIndex];
                 
                Bitmap previewImage;
                Bitmap highlightedPreviewImage;
                CreateImage(check, out previewImage, out highlightedPreviewImage);

                if (highlightedPreviewImage != null || previewImage != null)
                {
                    callbackResult["previewImage"] = StiReportEdit.ImageToBase64(highlightedPreviewImage != null ? highlightedPreviewImage : previewImage);
                }
            }
        }

        public static void ActionCheck(StiRequestParams requestParams, StiReport report, Hashtable parameters, Hashtable callbackResult)
        {
            var checks = CheckReport(requestParams, report, true);
            int checkIndex = Convert.ToInt32(parameters["checkIndex"]);
            int actionIndex = Convert.ToInt32(parameters["actionIndex"]);

            if (checks != null && checkIndex < checks.Count)
            {
                StiCheck check = checks[checkIndex];

                if (check.Actions != null && actionIndex < check.Actions.Count)
                {
                    Check.StiAction action = check.Actions[actionIndex];
                    var needUpdateReport = false;

                    if (action is StiGoToCodeAction && check is StiCompilationErrorCheck)
                    {

                    }
                    else if (action is StiEditPropertyAction && check is StiCompilationErrorCheck)
                    {
                        action.Invoke(report, (check as StiCompilationErrorCheck).ComponentName, (check as StiCompilationErrorCheck).PropertyName);
                        needUpdateReport = true;
                    }
                    else
                    {
                        action.Invoke(report, check.Element, check.ElementName);
                        needUpdateReport = true;
                    }

                    if (needUpdateReport) {
                        checks = CheckReport(requestParams, report, true);
                        callbackResult["checkItems"] = GetChecksJSCollection(checks);
                        UpdateCurrentReport(report, parameters, callbackResult);
                    }
                }
            }
        }

        public static void CheckExpression(StiReport report, Hashtable parameters, Hashtable callbackResult)
        {
            var pex = Engine.StiParser.CheckExpression(
                StiEncodingHelper.DecodeString(parameters["expressionText"] as string),
                report.Pages.GetComponentByName(parameters["componentName"] as string));
            
            callbackResult["checkResult"] = StiEncodingHelper.Encode(pex != null ? pex.BaseMessage : "OK");
        }
        #endregion;
    }
}