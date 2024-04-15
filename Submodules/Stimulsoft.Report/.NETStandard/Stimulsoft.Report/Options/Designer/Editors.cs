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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Gauge.Helpers;

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
            public static class Editors
			{
                #region ScriptEditor
                public static class ScriptEditor
                {
                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool BlocksScriptEditorByDefault
                    {
                        get
                        {
                            return StiSettings.GetBool("Designer", "BlocksScriptEditorByDefault", true);
                        }
                        set
                        {
                            StiSettings.Set("Designer", "BlocksScriptEditorByDefault", value);
                        }
                    }
                }
                #endregion

                #region ChartEditor
                public static class ChartEditor
                {
                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool SimpleEditorByDefault
                    {
                        get
                        {
                            return StiSettings.GetBool("Designer", "SimpleChartEditorByDefault", true);
                        }
                        set
                        {
                            StiSettings.Set("Designer", "SimpleChartEditorByDefault", value);
                        }
                    }

                    [DefaultValue(false)]
                    [StiSerializable]
                    public static bool ShowRunChartWizard
                    {
                        get
                        {
                            return StiSettings.GetBool("Designer", "ShowRunChartWizard", false);
                        }
                        set
                        {
                            StiSettings.Set("Designer", "ShowRunChartWizard", value);
                        }
                    }
                }
                #endregion

                #region GaugeEditor
                public static class GaugeEditor
                {
                    public static bool AllowOldEditor
                    {
                        get
                        {
                            return StiGaugeV2InitHelper.AllowOldEditor;
                        }
                        set
                        {
                            StiGaugeV2InitHelper.AllowOldEditor = value;
                        }
                    }
                }
                #endregion

                #region ImageEditor
                public static class ImageEditor
				{
				    [DefaultValue(true)]
                    [StiSerializable]
					public static bool ShowImageTabPage { get; set; } = true;

				    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDataColumnTabPage { get; set; } = true;

				    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowImageDataTabPage { get; set; } = true;

				    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowImageURLTabPage { get; set; } = true;

				    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowFileTabPage { get; set; } = true;
                }
                #endregion

                #region SystemRichTextEditor
                public static class SystemRichTextEditor
                {
                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDataColumnTabPage { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDataTabPage { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowURLTabPage { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowFileTabPage { get; set; } = true;
                }
                #endregion

                #region SystemTextEditor
                public static class SystemTextEditor
                {
                    [DefaultValue(false)]
                    [StiSerializable]
                    public static bool HighlightTextInSystemTextEditor { get; set; }
                }
                #endregion

                #region EditCustomConnection
                public static event StiEditCustomConnectionEventHandler EditCustomConnection;
                public static void InvokeEditCustomConnection(StiEditCustomConnectionEventArgs e)
                {
                    EditCustomConnection?.Invoke(null, e);
                }
                #endregion

                #region EditCustomConnection
                public static event StiEditDataStoreAdapterEventHandler EditDataStoreAdapter;
                public static void InvokeEditDataStoreAdapter(StiEditDataStoreAdapterEventArgs e)
                {
                    EditDataStoreAdapter?.Invoke(null, e);
                }
                #endregion

			    [DefaultValue(false)]
			    [StiSerializable]
			    public static bool AllowConnectToDataInGallery { get; set; }
            }
        }
    }
}