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
using System.Text;
using Stimulsoft.Base.Serializing;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class for adjustment of the preview of a report.
        /// </summary>
        public partial class Viewer
		{
            /// <summary>
            /// Class for adjusting a Dot-Matrix window of the report.
            /// </summary>
            public class DotMatrix
			{
                [StiSerializable]
                [DefaultValue(false)]
				public static bool GroupBoxBorderType
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "groupBoxBorderType");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "groupBoxBorderType", value);
						StiSettings.Save();
					}
				}

			    /// <summary>
			    /// Gets or sets a value which controls collapsing of the Group Box Encoding of the Dot-Matrix window of the report. 
			    /// </summary>
                [Description("Gets or sets a value which controls collapsing of the Group Box Encoding of the Dot-Matrix window of the report. ")]
                [StiSerializable]
                [DefaultValue(false)]
                public static bool GroupBoxEncoding
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "groupBoxEncoding");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "groupBoxEncoding", value);
						StiSettings.Save();
					}
				}

			    /// <summary>
			    /// Gets or sets a value which controls collapsing of the Group Box Rrefresh of the Dot-Matrix window of the report.
			    /// </summary>
                [Description("Gets or sets a value which controls collapsing of the Group Box Rrefresh of the Dot-Matrix window of the report.")]
                [StiSerializable]
                [DefaultValue(false)]
                public static bool GroupBoxRefresh
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "groupBoxRefresh");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "groupBoxRefresh", value);
						StiSettings.Save();
					}
				}

			    /// <summary>
			    /// Gets or sets a value which contols a collapsing of the Group Box Settings of the Dot-Matrix window of the report.
			    /// </summary>
                [Description("Gets or sets a value which contols a collapsing of the Group Box Settings of the Dot-Matrix window of the report.")]
                [StiSerializable]
                [DefaultValue(false)]
                public static bool GroupBoxSettings
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "groupBoxSettings");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "groupBoxSettings", value);
						StiSettings.Save();
					}
				}

                /// <summary>
                /// Gets or sets a value which controls a collapsing of the Group Box Zoom.
                /// </summary>
                [Description("Gets or sets a value which controls a collapsing of the Group Box Zoom.")]
                [StiSerializable]
                [DefaultValue(false)]
                public static bool GroupBoxZoom
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "groupBoxZoom");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "groupBoxZoom", value);
						StiSettings.Save();
					}
				}

                /// <summary>
                /// Gets or sets a value which controls a visibillity of the Dot-Matrix Mode button of the report.
                /// </summary>
                [Description("Gets or sets a value which controls a visibillity of the Dot-Matrix Mode button of the report.")]
                [StiSerializable]
                [DefaultValue(false)]
                public static bool DotMatrixMode
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "DotMatrixMode");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "DotMatrixMode", value);
						StiSettings.Save();
					}
				}

                [StiSerializable]
                [DefaultValue(false)]
				public static bool CutLongLines
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "CutLongLines");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "CutLongLines", value);
						StiSettings.Save();
					}
				}

                [StiSerializable]
                [DefaultValue(false)]
				public static bool DrawBorder
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "DrawBorder");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "DrawBorder", value);
						StiSettings.Save();
					}
				}

                [StiSerializable]
                [DefaultValue(false)]
				public static bool KillSpaceLines
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "KillSpaceLines");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "KillSpaceLines", value);
						StiSettings.Save();
					}
				}

                [StiSerializable]
                [DefaultValue(false)]
				public static bool KillSpaceGraphLines
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "KillSpaceGraphLines");						
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "KillSpaceGraphLines", value);
						StiSettings.Save();
					}
				}

                [StiSerializable]
                [DefaultValue(false)]
				public static bool PutFeedPageCode
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "PutFeedPageCode");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "PutFeedPageCode", value);
						StiSettings.Save();
					}
				}

                [StiSerializable]
                [DefaultValue(false)]
				public static bool Simple
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "Simple");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "Simple", value);
						StiSettings.Save();
					}
				}

                [StiSerializable]
                [DefaultValue(false)]
				public static bool UnicodeSingle
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "UnicodeSingle");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "UnicodeSingle", value);
						StiSettings.Save();
					}
				}

                [StiSerializable]
                [DefaultValue(false)]
				public static bool UnicodeDouble
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "UnicodeDouble");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "UnicodeDouble", value);
						StiSettings.Save();
					}
				}
                
                [StiSerializable]
                [DefaultValue(false)]
				public static bool AutoRefresh
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetBool("Viewer", "AutoRefresh");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "AutoRefresh", value);
						StiSettings.Save();
					}
				}

                [StiSerializable]
                [DefaultValue("")]
				public static string ZoomX
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetStr("Viewer", "ZoomX");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "ZoomX", value);
						StiSettings.Save();
					}
				}

                [StiSerializable]
                [DefaultValue("")]
				public static string ZoomY
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetStr("Viewer", "ZoomY");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "ZoomY", value);
						StiSettings.Save();
					}
				}

                [StiSerializable]
                [DefaultValue(-1)]
				public static int Encoding
				{
					get
					{
						StiSettings.Load();
						return StiSettings.GetInt("Viewer", "Encoding");
					}
					set
					{
						StiSettings.Load();
						StiSettings.Set("Viewer", "Encoding", value);
						StiSettings.Save();
					}
                }

                [StiSerializable]
                [DefaultValue(false)]
                public static bool UseEscapeCodes
                {
                    get
                    {
                        StiSettings.Load();
                        return StiSettings.GetBool("Viewer", "UseEscapeCodes");
                    }
                    set
                    {
                        StiSettings.Load();
                        StiSettings.Set("Viewer", "UseEscapeCodes", value);
                        StiSettings.Save();
                    }
                }

                [StiSerializable]
                [DefaultValue("")]
                public static string EscapeCodesCollectionName
                {
                    get
                    {
                        StiSettings.Load();
                        return StiSettings.GetStr("Viewer", "EscapeCodesCollectionName");
                    }
                    set
                    {
                        StiSettings.Load();
                        StiSettings.Set("Viewer", "EscapeCodesCollectionName", value);
                        StiSettings.Save();
                    }
                }

                [DefaultValue(true)]
                [Description("Class for adjusting a Dot-Matrix window of the report.")]
                [StiSerializable]
                public static bool ShowInTaskbar { get; set; } = true;

                /// <summary>
                /// Gets or sets window state of the viewer window.
                /// </summary>
                [DefaultValue(FormWindowState.Maximized)]
                [Description("Gets or sets window state of the viewer window.")]
                [StiSerializable]
                public static FormWindowState ViewerWindowState { get; set; } = FormWindowState.Maximized;

                public static Encoding[] DefaultEncodings { get; set; }

                public static Icon ViewerIcon { get; set; }

                public static object ViewerWpfIcon { get; set; }
            }
		}
    }
}