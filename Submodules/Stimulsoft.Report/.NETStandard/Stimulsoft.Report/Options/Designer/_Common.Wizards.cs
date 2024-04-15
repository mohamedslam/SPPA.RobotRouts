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

using System.Collections;
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
            #region Wizards
            /// <summary>
            /// A Class for adjusting Wizards which controls a creation of the new reports.
            /// </summary>
            public sealed class Wizards
            {
                /// <summary>
                /// Gets a collection for the Label Type property of the Label Report Wizard.
                /// </summary>
                public static ArrayList Labels { get; set; } = new ArrayList();

                /// <summary>
                /// Gets a collection for the Size of the Page property in the Label Report Wizard. 
                /// </summary>
                public static ArrayList LabelPageSizes { get; set; } = new ArrayList();

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowNewDataSourceButton { get; set; } = true;

            }
            #endregion

            #region Properties
            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowUseHighlightWizard { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowUsePlacementWizard { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to run the Wizard after loading the designer of the report.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value indicating whether it is necessary to run the Wizard after loading the designer of the report.")]
            [StiSerializable]
            public static bool RunWizardAfterLoad { get; set; }

            /// <summary>
            /// A string representation of the wizard type which should be run after designer starts.
            /// </summary>
            [DefaultValue(false)]
            [Description("A string representation of the wizard type which should be run after designer starts.")]
            [StiSerializable]
            public static string RunSpecificWizardAfterLoad { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowRunDataBandPlacementWizard { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool UseComponentPlacementOptimization { get; set; } = true;
            #endregion
        }
    }
}