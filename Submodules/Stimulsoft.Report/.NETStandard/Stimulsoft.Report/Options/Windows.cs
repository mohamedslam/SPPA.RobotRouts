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

using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
    {
		public sealed class Windows
        {
            #region Properties.Obsolete
            /// <summary>
            /// Please use StiOptions.Windows.GlobalGuiStyle instead Style property of StiViewerControl!
            /// </summary>
            [Obsolete("Please use StiOptions.Windows.GlobalGuiStyle instead Style property of StiViewerControl!")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static StiGuiStyle GuiStyle
			{
				get
				{
				    if (GlobalGuiStyle == StiGlobalGuiStyle.Office2007Black)
                        return StiGuiStyle.Office2007Black;

				    if (GlobalGuiStyle == StiGlobalGuiStyle.Office2007Blue)
				        return StiGuiStyle.Office2007Blue;

				    if (GlobalGuiStyle == StiGlobalGuiStyle.Office2007Silver)
				        return StiGuiStyle.Office2007Silver;

				    return StiGuiStyle.Default;
				}
                set
				{
                    if (value == StiGuiStyle.Office2007Black)
                        GlobalGuiStyle = StiGlobalGuiStyle.Office2007Black;

                    else if (value == StiGuiStyle.Office2007Blue)
                        GlobalGuiStyle = StiGlobalGuiStyle.Office2007Blue;

                    else if (value == StiGuiStyle.Office2007Silver)
                        GlobalGuiStyle = StiGlobalGuiStyle.Office2007Silver;
				}
			}
            #endregion
            
            #region Properties
            private static StiGlobalGuiStyle globalGuiStyle = StiGlobalGuiStyle.Office2013;
            /// <summary>
            /// Gets or sets current theme of Stimulsoft Reports application.
            /// </summary>
            [DefaultValue(StiGlobalGuiStyle.Office2013)]
            [Description("Gets or sets current theme of Stimulsoft Reports application.")]
            [StiSerializable]
            [Browsable(false)]
            [Obsolete("Property 'GlobalGuiStyle' is obsoleted.")]
            public static StiGlobalGuiStyle GlobalGuiStyle
            {
                get
                {
                    if (GlobalGuiStyleLoaded)
                        return globalGuiStyle;

                    StiSettings.Load();
                    GlobalGuiStyleLoaded = true;
                    return globalGuiStyle = (StiGlobalGuiStyle)StiSettings.Get("Windows", "GlobalGuiStyle", StiGlobalGuiStyle.Office2013);
                }
                set
                {
                    if (GlobalGuiStyleLoaded && globalGuiStyle == value) return;
                    StiSettings.Load();
                    StiSettings.Set("Windows", "GlobalGuiStyle", value);

                    var oldValue = globalGuiStyle;
                    globalGuiStyle = value;
                    GlobalGuiStyleLoaded = true;
                    StiSettings.Save();

                    if (globalGuiStyle != oldValue)
                        Engine.GlobalEvents.InvokeGlobalGuiStyleChanged(null, EventArgs.Empty);
                }
            }

            /// <summary>
            /// Internal use only.
            /// </summary>
            [Obsolete("Property 'IsGuiTypeSelected' is obsoleted.")]
            public static bool IsGuiTypeSelected
            {
                get
                {
                    StiSettings.Load();
                    return StiSettings.GetBool("GuiHelper", "IsGuiTypeSelected", false);
                }
                set
                {
                    StiSettings.Load();
                    StiSettings.Set("GuiHelper", "IsGuiTypeSelected", value);
                    StiSettings.Save();
                }
            }
            #endregion

            #region Fields
            /// <summary>
            /// Internal use only.
            /// </summary>
            public static bool GlobalGuiStyleLoaded { get; set; }

            /// <summary>
            /// Internal use only.
            /// </summary>
            public static StiGlobalGuiStyle? LastGlobalGuiStyle { get; set; }

            [DefaultValue(false)]
		    [StiSerializable]
		    public static bool UseSkinableReportRenderingWindow { get; set; }

            /// <summary>
            /// Enables Windows 7 task bar support for report designer and report viewer applications.
            /// </summary>
            [DefaultValue(false)]
		    [StiSerializable]
		    [Description("Enables Windows 7 task bar support for report designer and report viewer applications.")]
		    public static bool Windows7TaskBarSupport { get; set; }
            #endregion

            #region Methods.States
            /// <summary>
            /// Returns value which indicates that Office 2010 or Office 2013 or Office 2016 gui is enabled.
            /// </summary>
            [Obsolete("Property 'IsOffice2010or2013Enabled' is obsoleted.")]
            public static bool IsOffice2010or2013Enabled()
            {
                return IsOffice2010Enabled() || IsOffice2013Enabled();
            }

            /// <summary>
            /// Returns value which indicates that Office 2013 gui is enabled.
            /// </summary>
            [Obsolete("Property 'IsOffice2013Enabled' is obsoleted.")]
            public static bool IsOffice2013Enabled()
            {
                return GlobalGuiStyle == StiGlobalGuiStyle.Office2013;
            }

            /// <summary>
            /// Returns value which indicates that Office 2013 gui is enabled.
            /// </summary>
            [Obsolete("Property 'IsOffice2013Enabled' is obsoleted.")]
            public static bool IsOffice2013Enabled(StiGlobalGuiStyle style)
            {
                return style == StiGlobalGuiStyle.Office2013;
            }

            /// <summary>
            /// Returns value which indicates that Office 2010 gui is enabled.
            /// </summary>
            [Obsolete("Property 'IsOffice2013Enabled' is obsoleted.")]
            public static bool IsOffice2010Enabled()
            {
                return IsOffice2010Enabled(GlobalGuiStyle);
            }

            /// <summary>
            /// Returns value which indicates that Office 2010 gui is enabled.
            /// </summary>
            [Obsolete("Property 'IsOffice2010Enabled' is obsoleted.")]
            public static bool IsOffice2010Enabled(StiGlobalGuiStyle style)
            {
                return
                    style == StiGlobalGuiStyle.Office2010Black ||
                    style == StiGlobalGuiStyle.Office2010Blue ||
                    style == StiGlobalGuiStyle.Office2010Silver ||
                    style == StiGlobalGuiStyle.Windows7;
            }

            /// <summary>
            /// Returns value which indicates that Office 2007 gui is enabled.
            /// </summary>
            [Obsolete("Property 'IsOffice2007Enabled' is obsoleted.")]
            public static bool IsOffice2007Enabled()
            {
                return IsOffice2007Enabled(GlobalGuiStyle);
            }

            /// <summary>
            /// Returns value which indicates that Office 2007 gui is enabled.
            /// </summary>
            [Obsolete("Property 'IsOffice2007Enabled' is obsoleted.")]
            public static bool IsOffice2007Enabled(StiGlobalGuiStyle style)
            {
                return
                    style == StiGlobalGuiStyle.Office2007Black ||
                    style == StiGlobalGuiStyle.Office2007Blue ||
                    style == StiGlobalGuiStyle.Office2007Silver ||
                    style == StiGlobalGuiStyle.Vista;
            }

            /// <summary>
            /// Returns value which indicates that Office 2003 gui is enabled.
            /// </summary>
            [Obsolete("Property 'IsOffice2003Enabled' is obsoleted.")]
            public static bool IsOffice2003Enabled()
            {
                return IsOffice2003Enabled(GlobalGuiStyle);
            }

            /// <summary>
            /// Returns value which indicates that Office 2003 gui is enabled.
            /// </summary>
            [Obsolete("Property 'IsOffice2003Enabled' is obsoleted.")]
            public static bool IsOffice2003Enabled(StiGlobalGuiStyle style)
            {
                return
                    style == StiGlobalGuiStyle.Office2003Black ||
                    style == StiGlobalGuiStyle.Office2003Blue ||
                    style == StiGlobalGuiStyle.Office2003Silver;
            }

            /// <summary>
            /// Returns value which indicates that Ribbon gui is enabled.
            /// </summary>
            [Obsolete("Property 'IsRibbonGuiEnabled' is obsoleted.")]
            public static bool IsRibbonGuiEnabled()
            {
                return IsRibbonGuiEnabled(GlobalGuiStyle);
            }

            /// <summary>
            /// Returns value which indicates that Ribbon gui is enabled.
            /// </summary>
            [Obsolete("Property 'IsRibbonGuiEnabled' is obsoleted.")]
            public static bool IsRibbonGuiEnabled(StiGlobalGuiStyle style)
            {
                return IsOffice2007Enabled(style) || IsOffice2010Enabled(style) || IsOffice2013Enabled(style);
            }
            #endregion
		}
    }
}