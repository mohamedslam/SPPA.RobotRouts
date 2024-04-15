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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Maps;

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
			public sealed class ComponentsTypes
			{
				#region SimpleComponents
				public sealed class SimpleComponents
				{
                    public static Type Text { get; set; } = typeof(StiText);

                    public static Type RickText { get; set; } = typeof(StiRichText);

                    public static Type Image { get; set; } = typeof(StiImage);

                    public static Type Map { get; set; } = typeof(StiMap);

                    public static Type CheckBox { get; set; } = typeof(StiCheckBox);
				}
				#endregion

				#region ComplexComponents
				public sealed class ComplexComponents
				{
                    public static Type Container { get; set; } = typeof(StiContainer);

                    public static Type Panel { get; set; } = typeof(StiPanel);

                    public static Type SubReport { get; set; } = typeof(StiSubReport);
				}
				#endregion

				#region Bands
				public sealed class Bands
				{
                    public static Type ReportTitleBand { get; set; } = typeof(StiReportTitleBand);

                    public static Type HeaderBand { get; set; } = typeof(StiHeaderBand);

                    public static Type GroupHeaderBand { get; set; } = typeof(StiGroupHeaderBand);

                    public static Type DataBand { get; set; } = typeof(StiDataBand);

                    public static Type GroupFooterBand { get; set; } = typeof(StiGroupFooterBand);

                    public static Type FooterBand { get; set; } = typeof(StiFooterBand);

                    public static Type ChildBand { get; set; } = typeof(StiChildBand);
				}
				#endregion
			}
		}
    }
}