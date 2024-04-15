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

using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Design;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        #region Properties
        /// <summary>
        /// Gets or sets default report culture
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [DefaultValue("")]
        [StiCategory("Globalization")]
        [Category("Globalization")]
        [Description("Gets or sets default report culture.")]
        [StiOrder(StiPropertyOrder.ReportMainCulture)]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiBrowsable(true)]
        [Editor("Stimulsoft.Report.Design.StiCultureEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiCultureConverter))]
        public string Culture { get; set; } = string.Empty;
                
        /// <summary>
        /// Gets a collection which consists of globalization strings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Globalization")]
        [StiCategory("Globalization")]
        [TypeConverter(typeof(StiGlobalizationContainerConverter))]
        [Editor("Stimulsoft.Report.Design.StiGlobalizationContainerCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets a collection which consists of globalization strings.")]
        [Browsable(false)]
        [StiOrder(StiPropertyOrder.ReportMainGlobalizationStrings)]
        [StiPropertyLevel(StiLevel.Professional)]
        public StiGlobalizationContainerCollection GlobalizationStrings { get; }

        private bool ShouldSerializeGlobalizationStrings()
        {
            return GlobalizationStrings.Count > 0;
        }

        /// <summary>
        /// Gets or sets property which allows automatic localization of the report when running starts. 
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [DefaultValue(false)]
        [Category("Globalization")]
        [StiCategory("Globalization")]
        [StiOrder(StiPropertyOrder.ReportMainAutoLocalizeReportOnRun)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets property which allows automatic localization of the report when running starts.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public bool AutoLocalizeReportOnRun { get; set; }

        /// <summary>
        /// Gets or sets a GlobalizationManager property which controls of the report localization.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IStiGlobalizationManager GlobalizationManager { get; set; } = new StiNullGlobalizationManager();

        /// <summary>
        /// Gets a localized report name.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string LocalizedName
        {
            get
            {
                return StiLocalization.CultureName == "en"
                    ? "Report"
                    : StiLocalization.Get("Components", "StiReport");
            }
        }
        #endregion

        #region Methods.Globalization
        /// <summary>
        /// Localize a report to the specified culture name. The culture definition must be placed in GlobalizationStrings.
        /// </summary>
        /// <param name="cultureName"></param>
        public StiReport LocalizeReport(string cultureName)
        {
            GlobalizationStrings.LocalizeReport(cultureName);

            return this;
        }

        /// <summary>
        /// Localize a report to the specified culture. The culture definition must be placed in GlobalizationStrings.
        /// </summary>
        /// <param name="info"></param>
        public StiReport LocalizeReport(CultureInfo info)
        {
            GlobalizationStrings.LocalizeReport(info);

            return this;
        }

        internal string GetParsedCulture()
        {
            if (Culture != null && Culture.StartsWith("{"))
                return StiReportParser.Parse(Culture, this, false, null, false, false);

            return Culture;
        }
        #endregion
    }
}