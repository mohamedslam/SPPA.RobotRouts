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
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Check
{
    public class StiCheckEngine
    {
        #region class CheckComparer
        private class CheckComparer : IComparer<StiCheck>
        {
            public int Compare(StiCheck check1, StiCheck check2)
            {
                if (check1.Status == StiCheckStatus.Error)
                {
                    if (check2.Status == StiCheckStatus.Error)
                        return 0;

                    else
                        return -1;
                }
                else if (check1.Status == StiCheckStatus.Warning)
                {
                    if (check2.Status == StiCheckStatus.Error)
                        return 1;

                    else if (check2.Status == StiCheckStatus.Information)
                        return -1;

                    else return 0;
                }
                else
                {
                    if (check2.Status == StiCheckStatus.Error || check2.Status == StiCheckStatus.Warning)
                        return 1;

                    else 
                        return 0;
                }
            }
        }
        #endregion

        #region Events
        #region FinishCheckingReport
        public event EventHandler FinishCheckingReport;

        private void InvokeFinishCheckingReport()
        {
            FinishCheckingReport?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region StartCheckingPages
        public event EventHandler StartCheckingPages;

        private void InvokeStartCheckingPages()
        {
            StartCheckingPages?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region CheckingPages
        public event EventHandler CheckingPages;

        private void InvokeCheckingPages()
        {
            CheckingPages?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region FinishCheckingPages
        public event EventHandler FinishCheckingPages;

        private void InvokeFinishCheckingPages()
        {
            FinishCheckingPages?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region StartCheckingComponents
        public event EventHandler StartCheckingComponents;

        private void InvokeStartCheckingComponents()
        {
            StartCheckingComponents?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region CheckingComponents
        public event EventHandler CheckingComponents;

        private void InvokeCheckingComponents()
        {
            CheckingComponents?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region FinishCheckingComponents
        public event EventHandler FinishCheckingComponents;

        private void InvokeFinishCheckingComponents()
        {
            FinishCheckingComponents?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region StartCheckingDatabases
        public event EventHandler StartCheckingDatabases;

        private void InvokeStartCheckingDatabases()
        {
            StartCheckingDatabases?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region CheckingDatabases
        public event EventHandler CheckingDatabases;

        private void InvokeCheckingDatabases()
        {
            CheckingDatabases?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region FinishCheckingDatabases
        public event EventHandler FinishCheckingDatabases;

        private void InvokeFinishCheckingDatabases()
        {
            FinishCheckingDatabases?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region StartCheckingDataSource
        public event EventHandler StartCheckingDataSource;

        private void InvokeStartCheckingDataSource()
        {
            StartCheckingDataSource?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region CheckingDataSource
        public event EventHandler CheckingDataSource;

        private void InvokeCheckingDataSource()
        {
            CheckingDataSource?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region FinishCheckingDataSource
        public event EventHandler FinishCheckingDataSource;

        private void InvokeFinishCheckingDataSource()
        {
            FinishCheckingDataSource?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region StartCheckingRelations
        public event EventHandler StartCheckingRelations;

        private void InvokeStartCheckingRelations()
        {
            StartCheckingRelations?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region CheckingRelations
        public event EventHandler CheckingRelations;

        private void InvokeCheckingRelations()
        {
            CheckingRelations?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region FinishCheckingRelations
        public event EventHandler FinishCheckingRelations;

        private void InvokeFinishCheckingRelations()
        {
            FinishCheckingRelations?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region StartCheckingVariables
        public event EventHandler StartCheckingVariables;

        private void InvokeStartCheckingVariables()
        {
            StartCheckingVariables?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region CheckingVariables
        public event EventHandler CheckingVariables;

        private void InvokeCheckingVariables()
        {
            CheckingVariables?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region FinishCheckingVariables
        public event EventHandler FinishCheckingVariables;

        private void InvokeFinishCheckingVariables()
        {
            FinishCheckingVariables?.Invoke(this, EventArgs.Empty);
        }
        #endregion
        #endregion

        #region Properties.Static
        private static List <StiCheck> checks;
        public static List <StiCheck> Checks
        {
            get
            {
                if (checks == null)
                    CreateChecks();

                return checks;
            }
        }
        #endregion

        #region Properties
        public double ProgressValue { get; private set; }

        public double ProgressMaximum { get; private set; }

        public string ProgressInformation { get; private set; } = string.Empty;
        #endregion

        #region Methods
        private static void CreateChecks()
        {
            checks = new List<StiCheck>();

            #region Errors
            checks.Add(new StiDuplicatedNameCheck());
            checks.Add(new StiDuplicatedReportNameCheck());
            checks.Add(new StiNoNamePageCheck());
            checks.Add(new StiFunctionsOnlyForEngineV2Check());
            checks.Add(new StiComponentStyleIsNotFoundAtPageCheck());
            checks.Add(new StiNoNameComponentCheck());
            checks.Add(new StiUndefinedComponentCheck());
            checks.Add(new StiUndefinedConnectionCheck());
            checks.Add(new StiDifferentAmountOfKeysInDataRelationCheck());
            checks.Add(new StiKeysInAbsentDataRelationCheck());
            checks.Add(new StiKeysNotFoundRelationCheck());
            checks.Add(new StiKeysTypesMismatchDataRelationCheck());
            checks.Add(new StiNoNameDataRelationCheck());
            checks.Add(new StiNoNameInSourceDataRelationCheck());
            checks.Add(new StiNoNameDataSourceCheck());
            checks.Add(new StiNoNameInSourceDataSourceCheck());
            checks.Add(new StiUndefinedDataSourceCheck());
            checks.Add(new StiCompilationErrorCheck());
            checks.Add(new StiCalculatedColumnRecursionCheck());
            checks.Add(new StiVariableRecursionCheck());
            checks.Add(new StiVariableInitializationCheck());
            checks.Add(new StiVariableTypeCheck());
            checks.Add(new StiGroupHeaderSummaryExpressionCheck());
            #endregion

            #region Warnings
#if NETSTANDARD || NETCOREAPP
            checks.Add(new StiNetCoreCompilationModeCheck());
#endif
#if CLOUD
            checks.Add(new StiCloudCompilationModeCheck());
#endif
            checks.Add(new StiDuplicatedNameInSourceCheck());
            checks.Add(new StiExpressionElementCheck());
            checks.Add(new StiFilterCircularDependencyElementCheck());
            checks.Add(new StiLostPointsOnPageCheck());
            checks.Add(new StiOrientationPageCheck());
            checks.Add(new StiColumnsWidthGreaterPageWidthCheck());
            checks.Add(new StiAllowHtmlTagsInTextCheck());
            checks.Add(new StiCanBreakComponentInContainerCheck());
            checks.Add(new StiCanGrowComponentInContainerCheck());
            checks.Add(new StiGrowToHeightOverlappingCheck());
            checks.Add(new StiComponentStyleIsNotFoundAtComponentCheck());
            checks.Add(new StiCorruptedCrossLinePrimitiveCheck());
            checks.Add(new StiPrintOnDoublePassCheck());
            checks.Add(new StiLocationOutsidePageCheck());
            //checks.Add(new StiImageHyperlinkCheck());
            checks.Add(new StiNegativeSizesOfComponentsCheck());
            checks.Add(new StiPanelInEngineV1Check());
            checks.Add(new StiWordWrapCanGrowTextDoesNotFitCheck());
            checks.Add(new StiDuplicatedNameInSourceInDataRelationReportCheck());
            checks.Add(new StiSourcesInAbsentDataRelationCheck());
            checks.Add(new StiIsFirstPageIsLastPageDoublePassCheck());
            checks.Add(new StiComponentBoundsAreOutOfBand());
            checks.Add(new StiComponentDataColumnCheck());
            checks.Add(new StiComponentExpressionCheck());
            checks.Add(new StiComponentResourceCheck());
            checks.Add(new StiTextTextFormatCheck());
            checks.Add(new StiFilterValueCheck());
            checks.Add(new StiComponentEventsAtInterpretationCheck());
            checks.Add(new StiPageEventsAtInterpretationCheck());
            checks.Add(new StiConnectionEventsAtInterpretationCheck());
            checks.Add(new StiReportEventsAtInterpretationCheck());
            checks.Add(new StiChartSeriesValueCheck());
            checks.Add(new StiDataSourceLoopingCheck());
            checks.Add(new StiCategoryRequestFromUserCheck());
            #endregion

            #region Informations
            checks.Add(new StiLicenseTrialCheck());
            checks.Add(new StiTotalPageCountDoublePassCheck());
            checks.Add(new StiIsFirstPassIsSecondPassCheck());
            checks.Add(new StiPrintHeadersAndFootersFromPreviousPageCheck());
            checks.Add(new StiLargeHeightAtPageCheck());
            checks.Add(new StiPrintOnPreviousPageCheck());
            checks.Add(new StiPrintOnPreviousPageCheck2());
            checks.Add(new StiResetPageNumberCheck());
            checks.Add(new StiVerySmallSizesOfComponentsCheck());
            checks.Add(new StiShowInsteadNullValuesCheck());
            checks.Add(new StiPropertiesOnlyEngineV1Check());
            checks.Add(new StiPropertiesOnlyEngineV2Check());
            checks.Add(new StiMinRowsInColumnsCheck());
            checks.Add(new StiDataSourcesForImageCheck());
            checks.Add(new StiCanGrowGrowToHeightComponentInContainerCheck());
            checks.Add(new StiCanGrowWordWrapTextAndWysiwygCheck());
            checks.Add(new StiColumnsWidthGreaterContainerWidthCheck());
            checks.Add(new StiGroupHeaderNotEqualToGroupFooterOnPageCheck());
            checks.Add(new StiCrossGroupHeaderNotEqualToCrossGroupFooterOnPageCheck());
            checks.Add(new StiGroupHeaderNotEqualToGroupFooterOnContainerCheck());
            checks.Add(new StiCrossGroupHeaderNotEqualToCrossGroupFooterOnContainerCheck());
            checks.Add(new StiContainerInEngineV2Check());
            checks.Add(new StiSystemTextObsoleteCheck());
            checks.Add(new StiContourTextObsoleteCheck());
            checks.Add(new StiCountDataDataSourceAtDataBandCheck());
            checks.Add(new StiNoConditionAtGroupCheck());
            checks.Add(new StiWidthHeightZeroComponentCheck());
            checks.Add(new StiTextColorEqualToBackColorCheck());
            checks.Add(new StiFontMissingCheck());
            checks.Add(new StiInsufficientTextHeightForOneLineCheck());
            #endregion
        }

        public List<StiCheck> CheckReport(StiReport report)
        {
            var results = new List<StiCheck>();

            #region Create List of Checks
            var checksReport = new List<StiCheck>();
            var checksReportCompilation = new List<StiCheck>();
            var checksPage = new List<StiCheck>();
            var checksComponent = new List<StiCheck>();
            var checksDatabase = new List<StiCheck>();
            var checksDataSource = new List<StiCheck>();
            var checksDataRelation = new List<StiCheck>();
            var checksDataColumn = new List<StiCheck>();
            var checksVariable = new List<StiCheck>();
            
            foreach (var check in Checks)
            {
                if (check is StiLicenseTrialCheck)
                {
                    check.Enabled = true;
                }

                if (!check.Enabled) continue;

                switch (check.ObjectType)
                {
                    case StiCheckObjectType.Report:
                        if (check is StiCompilationErrorCheck)
                            checksReportCompilation.Add(check);

                        else
                            checksReport.Add(check);
                        break;

                    case StiCheckObjectType.Page:
                        checksPage.Add(check);
                        break;

                    case StiCheckObjectType.Component:
                        checksComponent.Add(check);
                        break;

                    case StiCheckObjectType.Database:
                        checksDatabase.Add(check);
                        break;

                    case StiCheckObjectType.DataSource:
                        checksDataSource.Add(check);
                        break;

                    case StiCheckObjectType.DataRelation:
                        checksDataRelation.Add(check);
                        break;

                    case StiCheckObjectType.DataColumn:
                        checksDataColumn.Add(check);
                        break;

                    case StiCheckObjectType.Variable:
                        checksVariable.Add(check);
                        break;
                }
            }
            #endregion

            #region Process Report
            CheckObject(report, report, results, checksReport);

            var hasErrors = results.Any(r => r is StiDuplicatedNameCheck || r is StiDuplicatedReportNameCheck);
            if (!hasErrors)
                CheckObject(report, report, results, checksReportCompilation);

            InvokeFinishCheckingReport();
            #endregion

            #region Process Pages
            ProgressMaximum = report.Pages.Count;
            ProgressValue = 0;
            InvokeStartCheckingPages();

            foreach (StiPage page in report.Pages)
            {
                #region Process Page
                ProgressInformation = page.Name;
                ProgressValue++;
                InvokeCheckingPages();
                CheckObject(report, page, results, checksPage);
                #endregion
            }
            InvokeFinishCheckingPages();
            #endregion

            #region Process Components
            ProgressMaximum = report.GetComponentsCount();
            ProgressValue = 0;
            InvokeStartCheckingComponents();

            foreach (StiPage page in report.Pages)
            {
                var comps = page.GetComponents();
                foreach (StiComponent comp in comps)
                {
                    ProgressInformation = comp.Name;
                    ProgressValue++;
                    InvokeCheckingComponents();
                    CheckObject(report, comp, results, checksComponent);
                }
            }
            InvokeFinishCheckingComponents();
            #endregion

            #region Process Dictionary
            #region Process Databases
            ProgressMaximum = report.Dictionary.Databases.Count;
            ProgressValue = 0;
            InvokeStartCheckingDatabases();

            foreach (StiDatabase database in report.Dictionary.Databases)
            {
                ProgressInformation = $"{StiLocalization.Get("QueryBuilder", "Database")} - '{database.Name}'";
                ProgressValue++;
                InvokeCheckingDatabases();

                CheckObject(report, database, results, checksDatabase);                
            }
            InvokeFinishCheckingDatabases();
            #endregion

            #region Process DataSource
            ProgressMaximum = report.Dictionary.DataSources.Count;
            ProgressValue = 0;
            InvokeStartCheckingDataSource();

            foreach (StiDataSource dataSource in report.Dictionary.DataSources)
            {
                ProgressInformation = $"{StiLocalization.Get("PropertyMain", "DataSource")} - '{dataSource.Name}'";
                ProgressValue++;
                InvokeCheckingDataSource();

                CheckObject(report, dataSource, results, checksDataSource);

                foreach (StiDataColumn dataColumn in dataSource.Columns)
                {
                    CheckObject(report, dataColumn, results, checksDataColumn);
                }                
            }
            InvokeFinishCheckingDataSource();
            #endregion

            #region Process Relations
            ProgressMaximum = report.Dictionary.Relations.Count;
            ProgressValue = 0;
            InvokeStartCheckingRelations();

            foreach (StiDataRelation dataRelation in report.Dictionary.Relations)
            {
                ProgressInformation = $"{StiLocalization.Get("PropertyMain", "DataRelation")} - '{dataRelation.Name}'";
                ProgressValue++;
                InvokeCheckingRelations();

                CheckObject(report, dataRelation, results, checksDataRelation);                
            }
            InvokeFinishCheckingRelations();
            #endregion

            #region Process Variables
            ProgressMaximum = report.Dictionary.Variables.Count;
            ProgressValue = 0;
            InvokeStartCheckingVariables();

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                ProgressInformation = $"{StiLocalization.Get("PropertyMain", "Variable")} - '{variable.Name}'";
                ProgressValue++;
                InvokeCheckingVariables();

                CheckObject(report, variable, results, checksVariable);
                
            }
            InvokeFinishCheckingVariables();
            #endregion
            #endregion

            results.Sort(new CheckComparer());
            return results;
        }

        private static void CheckObject(StiReport report, object obj, List<StiCheck> results, List<StiCheck> checksObject)
        {
            foreach (var check in checksObject)
            {
                var checkObject = check.ProcessCheck(report, obj);
                if (checkObject != null)
                {
                    var createdCheck = checkObject as StiCheck;
                    if (createdCheck != null)
                        results.Add(createdCheck);

                    var createdChecks = checkObject as List<StiCheck>;
                    if (createdChecks != null)
                    {
                        foreach (var createdCheck2 in createdChecks)
                        {
                            results.Add(createdCheck2);
                        }
                    }
                }
            }
        }
        #endregion

        public StiCheckEngine()
        {
            if (Stimulsoft.Report.Helper.StiLocalizationExt.Localization != StiLocalization.Localization)
                Stimulsoft.Report.Helper.StiLocalizationExt.Load();
        }
    }
}