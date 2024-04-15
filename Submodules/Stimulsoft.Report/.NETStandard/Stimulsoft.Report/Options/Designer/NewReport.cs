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

using System.ComponentModel;
using Stimulsoft.Base.Serializing;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class which allows adjustment of the Designer of the report.
        /// </summary>
        public sealed partial class Designer
        {
			public class NewReport
			{
                [DefaultValue(StiNewReportDictionaryBehavior.NewDictionary)]
                [StiSerializable]
                public static StiNewReportDictionaryBehavior DictionaryBehavior
                {
                    get
                    {
                        StiSettings.Load();
                        return (StiNewReportDictionaryBehavior)StiSettings.Get("StiDesigner", "NewReportDictionaryBehavior", StiNewReportDictionaryBehavior.NewDictionary);
                    }
                    set
                    {
                        StiSettings.Load();
                        StiSettings.Set("StiDesigner", "NewReportDictionaryBehavior", value);
                        StiSettings.Save();
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowRegisterDataStoreFromOldReportInNewReport { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowRegisterDatabasesFromOldReportInNewReport { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowRegisterDataSourcesFromOldReportInNewReport { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowRegisterRelationsFromOldReportInNewReport { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowRegisterVariablesFromOldReportInNewReport { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowRegisterResourcesFromOldReportInNewReport { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowRegisterRestrictionsFromOldReportInNewReport { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowRegisterBusinessObjectsFromOldReportInNewReport { get; set; } = true;
            }
		}
    }
}