#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using System.Threading.Tasks;

namespace Stimulsoft.Report.Helpers
{
    public class StiDictionaryInitializer
    {
        public static void Initialize(StiReport oldReport, StiReport newReport)
        {
            if (newReport == null) return;

            newReport.Dictionary.Clear();

            if (oldReport != null)
            {
                if (StiOptions.Designer.NewReport.DictionaryBehavior == StiNewReportDictionaryBehavior.MergeDictionary)
                {
                    if (StiOptions.Designer.NewReport.AllowRegisterDataStoreFromOldReportInNewReport)
                        newReport.Dictionary.DataStore.RegData(oldReport.Dictionary.DataStore);

                    if (StiOptions.Designer.NewReport.AllowRegisterDatabasesFromOldReportInNewReport)
                        newReport.Dictionary.Databases.AddRange(oldReport.Dictionary.Databases);

                    if (StiOptions.Designer.NewReport.AllowRegisterDataSourcesFromOldReportInNewReport)
                        newReport.Dictionary.DataSources.AddRange(oldReport.Dictionary.DataSources);

                    if (StiOptions.Designer.NewReport.AllowRegisterRelationsFromOldReportInNewReport)
                        newReport.Dictionary.Relations.AddRange(oldReport.Dictionary.Relations);

                    if (StiOptions.Designer.NewReport.AllowRegisterVariablesFromOldReportInNewReport)
                        newReport.Dictionary.Variables.AddRange(oldReport.Dictionary.Variables);

                    if (StiOptions.Designer.NewReport.AllowRegisterResourcesFromOldReportInNewReport)
                        newReport.Dictionary.Resources.AddRange(oldReport.Dictionary.Resources);

                    if (StiOptions.Designer.NewReport.AllowRegisterRestrictionsFromOldReportInNewReport)
                        newReport.Dictionary.Restrictions = oldReport.Dictionary.Restrictions;
                }
            }
        }
    }
}
