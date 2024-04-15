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

using Stimulsoft.Report.Components;

namespace Stimulsoft.Report
{
    public class StiStimulsoftServerResource
    {
        #region Delegates
        public delegate byte[] StiGetImageDelegate(StiImage image, string key);
        public delegate string StiGetRichTextDelegate(StiRichText richText, string key);
        public delegate StiReport StiGetSubReportDelegate(StiSubReport SubReport, string key);
        public delegate StiReport StiGetDrillDownReportDelegate(StiComponent component, string key);
        #endregion

        #region Fields.Static
        public static StiGetImageDelegate GetImageDelegate;
        public static StiGetRichTextDelegate GetRichTextDelegate;
        public static StiGetSubReportDelegate GetSubReportDelegate;
        public static StiGetDrillDownReportDelegate GetDrillDownReportDelegate;
        #endregion

        #region Methods.Static
        /// <summary>
        /// Gets the image with specified key from the Stimulsoft Server.
        /// </summary>
        public static byte[] GetImage(StiImage component, string key)
        {
            return GetImageDelegate != null ? GetImageDelegate(component, key) : null;
        }

        /// <summary>
        /// Gets the rich-text with specified key from the Stimulsoft Server.
        /// </summary>
        public static string GetRichText(StiRichText component, string key)
        {
            return GetRichTextDelegate != null ? GetRichTextDelegate(component, key) : null;
        }

        /// <summary>
        /// Gets the sub-report with specified key from the Stimulsoft Server.
        /// </summary>
        public static StiReport GetSubReport(StiSubReport component, string key)
        {
            return GetSubReportDelegate != null ? GetSubReportDelegate(component, key) : null;
        }

        /// <summary>
        /// Gets the drill-down report with specified key from the Stimulsoft Server.
        /// </summary>
        public static StiReport GetDrillDownReport(StiComponent component, string key)
        {
            return GetDrillDownReportDelegate != null ? GetDrillDownReportDelegate(component, key) : null;
        }
        #endregion
    }
}
