#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using System.IO;
using System.Reflection;

namespace Stimulsoft.Report.Web
{
    internal static class StiReportCopier
    {
        public static StiReport CloneReport(StiReport report, bool withResources = false)
        {
            if (report == null) return null;
            StiReport reportCopy;
            StiResourcesCollection tempResources = null;
            var reportFile = report.ReportFile;

            if (!withResources)
            {
                //Save resources to temp collection
                tempResources = CloneResource(report);

                //Clear resources before clone report
                report.Dictionary.Resources.Clear();
            }

            if (!StiOptions.Engine.FullTrust || report.CalculationMode == StiCalculationMode.Interpretation || report.NeedsCompiling)
            {
                reportCopy = new StiReport();
                MemoryStream stream = new MemoryStream();
                report.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                reportCopy.Load(stream);
                stream.Close();
            }
            else
            {
                reportCopy = StiActivator.CreateObject(report.GetType()) as StiReport;
            }

            // Copy report variables
            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                var reportDialogInfo = reportCopy.Dictionary.Variables[variable.Name].DialogInfo;
                reportDialogInfo.Keys = variable.DialogInfo.Keys?.Clone() as string[];
                reportDialogInfo.Values = variable.DialogInfo.Values?.Clone() as string[];
                reportDialogInfo.CheckedStates = variable.DialogInfo.CheckedStates?.Clone() as bool[];
                
                var field = report.GetType().GetField(variable.Name);
                if (field != null) 
                    reportCopy[variable.Name] = field.GetValue(report);
            }

            if (report.Variables != null && report.Variables.Count > 0)
            {
                foreach (string key in report.Variables.Keys)
                {
                    reportCopy[key] = report[key];
                }
            }

            reportCopy.RegData(report.DataStore);
            reportCopy.RegBusinessObject(report.BusinessObjectsStore);

            if (tempResources != null)
            {
                report.Dictionary.Resources.Clear();

                //Restore resources from temp collection
                StiReportResourceHelper.LoadResourcesToReport(report, tempResources);

                //Load resources from temp collection
                StiReportResourceHelper.LoadResourcesToReport(reportCopy, tempResources);
            }

            if (report.GlobalizationManager != null)
            {
                reportCopy.GlobalizationManager = report.GlobalizationManager;
            }

            report.ReportFile = reportFile;
            reportCopy.ReportFile = reportFile;

            reportCopy.CookieContainer = report.CookieContainer;
            reportCopy.InteractionCollapsingStates = report.InteractionCollapsingStates;

            CopyFilterElementsUserFilters(report, reportCopy);

            return reportCopy;
        }

        private static StiResourcesCollection CloneResource(StiReport report)
        {
            StiResourcesCollection resources = new StiResourcesCollection();

            foreach (StiResource resource in report.Dictionary.Resources)
            {
                resources.Add(resource);
            }

            return resources;
        }

        public static void CopyReportDictionary(StiReport reportFrom, StiReport reportTo)
        {
            if (reportTo == null) return;

            reportTo.Dictionary.Clear();

            if (reportFrom != null)
            {
                if (StiOptions.Designer.NewReport.AllowRegisterDataStoreFromOldReportInNewReport)
                    reportTo.Dictionary.DataStore.RegData(reportFrom.Dictionary.DataStore);

                if (StiOptions.Designer.NewReport.AllowRegisterDatabasesFromOldReportInNewReport)
                    reportTo.Dictionary.Databases.AddRange(reportFrom.Dictionary.Databases);

                if (StiOptions.Designer.NewReport.AllowRegisterDataSourcesFromOldReportInNewReport)
                    reportTo.Dictionary.DataSources.AddRange(reportFrom.Dictionary.DataSources);

                if (StiOptions.Designer.NewReport.AllowRegisterRelationsFromOldReportInNewReport)
                    reportTo.Dictionary.Relations.AddRange(reportFrom.Dictionary.Relations);

                if (StiOptions.Designer.NewReport.AllowRegisterVariablesFromOldReportInNewReport)
                    reportTo.Dictionary.Variables.AddRange(reportFrom.Dictionary.Variables);

                if (StiOptions.Designer.NewReport.AllowRegisterResourcesFromOldReportInNewReport)
                    reportTo.Dictionary.Resources.AddRange(reportFrom.Dictionary.Resources);

                if (StiOptions.Designer.NewReport.AllowRegisterRestrictionsFromOldReportInNewReport)
                    reportTo.Dictionary.Restrictions = reportFrom.Dictionary.Restrictions;

                reportTo.Dictionary.BusinessObjects.AddRange(reportFrom.Dictionary.BusinessObjects);
                reportTo.Dictionary.SynchronizeBusinessObjects();

                reportTo.Tag = reportFrom.Tag;
                reportTo.Dictionary.Synchronize();
            }
        }

        public static void CopyElementsDrillDown(StiReport reportFrom, StiReport reportTo)
        {
            reportTo.GetComponents().ToList().ForEach(c =>
            {
                if (c is IStiDrillDownElement)
                {
                    var drillDownComp = reportFrom.Pages.GetComponentByName(c.Name) as IStiDrillDownElement;
                    if (drillDownComp != null)
                    {
                        var cloneDrillDownComp = c as IStiDrillDownElement;
                        cloneDrillDownComp.DrillDownCurrentLevel = drillDownComp.DrillDownCurrentLevel;
                        cloneDrillDownComp.DrillDownFilters = drillDownComp.DrillDownFilters;
                        cloneDrillDownComp.DrillDownFiltersList = drillDownComp.DrillDownFiltersList;
                    }
                }
            });
        }

        public static void CopyFilterElementsUserFilters(StiReport reportFrom, StiReport reportTo)
        {
            reportFrom.GetComponents().ToList().ForEach(comp =>
            {
                if (comp is IStiFilterElement)
                {
                    var userFilter = comp as IStiUserFilters;
                    if (userFilter != null && userFilter.UserFilters?.Count > 0)
                    {
                        var copyComp = reportTo.GetComponentByName(comp.Name);
                        if (copyComp is IStiUserFilters)
                            ((IStiUserFilters)copyComp).UserFilters = ((IStiUserFilters)comp).UserFilters;
                    }
                }
            });
        }
    }
}
