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
using System.ComponentModel;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;

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
            #region Properties
            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to show or to hide Tips On Start Up Dialog window when opening the report designer.
            /// </summary>
            [Obsolete("Form 'Tip of the Day' is not used anymore.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static bool ShowTipsOnStartup { get; set; }

            /// <summary>
            /// Please use StiOptions.Windows.GlobalGuiStyle instead StiOptions.Designer.IsRibbonGuiEnabled property!
            /// </summary>
            [Obsolete("Please use StiOptions.Windows.GlobalGuiStyle instead StiOptions.Designer.IsRibbonGuiEnabled property!")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static bool IsRibbonGuiEnabled
            {
                get
                {
                    return Windows.IsRibbonGuiEnabled();
                }
                set
                {
                    Windows.GlobalGuiStyle =
                        value ? StiGlobalGuiStyle.Office2007Blue : StiGlobalGuiStyle.Office2003Blue;
                }
            }

            /// <summary>
            /// Please use StiOptions.Designer.ShowWatermarkInPageSetup instead StiOptions.Designer.DisableWatermarkInPageSetup property!
            /// </summary>
            [Obsolete("Please use StiOptions.Designer.ShowWatermarkInPageSetup instead StiOptions.Designer.DisableWatermarkInPageSetup property!")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static bool DisableWatermarkInPageSetup
            {
                get
                {
                    return ShowWatermarkInPageSetup;
                }
                set
                {
                    ShowWatermarkInPageSetup = value;
                }
            }

            [Obsolete("Sorry, you can't use more this property. Now only ReportChecker available for report messages.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static bool IsCheckMessagesEnabled { get; set; }

            [Obsolete("Please use StiOptions.Designer.IgnoreOptionReportNeverSaved. Spelling error.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static bool IgnoreOptionReportNewerSaved
            {
                get
                {
                    return IgnoreOptionReportNeverSaved;
                }
                set
                {
                    IgnoreOptionReportNeverSaved = value;
                }
            }
            #endregion

            #region GlobalEvents
            public sealed class GlobalEvents
            {
                #region PageSetup
                [Obsolete("Please dont use event StiOptions.Designer.GlobalEvents.PageSetup! You need use event StiOptions.Engine.GlobalEvents.RunPageSetupInDesigner instead it.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static event EventHandler PageSetup
                {
                    add
                    {
                        Engine.GlobalEvents.RunPageSetupInDesigner += value;
                    }
                    remove
                    {
                        Engine.GlobalEvents.RunPageSetupInDesigner -= value;
                    }
                }

                [Obsolete("Please dont use method StiOptions.Designer.GlobalEvents.InvokePageSetup! You need use method StiOptions.Engine.GlobalEvents.InvokeRunPageSetupInDesigner instead it.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool InvokePageSetup(StiPage page)
                {
                    return Engine.GlobalEvents.InvokeRunPageSetupInDesigner(page);
                }
                #endregion
            }

            #region CheckDataBandNameCreatedInWizard
            [Obsolete("Please dont use event StiOptions.Designer.CheckDataBandNameCreatedInWizard! You need use event StiOptions.Engine.GlobalEvents.CheckDataBandNameCreatedInWizard instead it.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static event StiCheckDataBandNameCreatedInWizardHandler CheckDataBandNameCreatedInWizard
            {
                add
                {
                    Engine.GlobalEvents.CheckDataBandNameCreatedInWizard += value;
                }
                remove
                {
                    Engine.GlobalEvents.CheckDataBandNameCreatedInWizard -= value;
                }
            }

            [Obsolete("Please dont use method StiOptions.Designer.InvokeCheckDataBandNameCreatedInWizard! You need use method StiOptions.Engine.GlobalEvents.InvokeCheckDataBandNameCreatedInWizard instead it.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static void InvokeCheckDataBandNameCreatedInWizard(StiCheckDataBandNameCreatedInWizardArgs checkArgs)
            {
                Engine.GlobalEvents.InvokeCheckDataBandNameCreatedInWizard(checkArgs);
            }
            #endregion
            #endregion
        }
    }
}