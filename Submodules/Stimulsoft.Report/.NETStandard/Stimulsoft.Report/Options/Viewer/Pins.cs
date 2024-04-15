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
        /// Class for adjustment of the preview of a report.
        /// </summary>
        public partial class Viewer
		{
            /// <summary>
            /// Class for adjusting right to left orientation of pins.
            /// </summary>
            public class Pins
            {
                /// <summary>
                /// Gets or sets value which indicates orientation of quick buttons pin.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets value which indicates orientation of quick buttons pin.")]
                [StiSerializable]
                public static bool QuickButtonsRightToLeft { get; set; }

                /// <summary>
                /// Gets or sets value which indicates orientation of events pin.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets value which indicates orientation of events pin.")]
                [StiSerializable]
                public static bool EventsRightToLeft { get; set; }

                /// <summary>
                /// Gets or sets value which indicates orientation of conditions pin.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets value which indicates orientation of conditions pin.")]
                [StiSerializable]
                public static bool ConditionsRightToLeft { get; set; }

                /// <summary>
                /// Gets or sets value which indicates orientation of filters pin.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets value which indicates orientation of filters pin.")]
                [StiSerializable]
                public static bool FiltersRightToLeft { get; set; }

                /// <summary>
                /// Gets or sets value which indicates orientation of interaction sort pin.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets value which indicates orientation of interaction sort pin.")]
                [StiSerializable]
                public static bool InteractionSortRightToLeft { get; set; }

                /// <summary>
                /// Gets or sets value which indicates orientation of interaction collapsing pin.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets value which indicates orientation of interaction collapsing pin.")]
                [StiSerializable]
                public static bool InteractionCollapsingRightToLeft { get; set; }

                /// <summary>
                /// Gets or sets value which indicates orientation of inheritance pin.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets value which indicates orientation of inheritance pin.")]
                [StiSerializable]
                public static bool InheritedRightToLeft { get; set; }

                /// <summary>
                /// Gets or sets value which indicates orientation of order and quick info pin.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets value which indicates orientation of order and quick info pin.")]
                [StiSerializable]
                public static bool OrderAndQuickInfoRightToLeft { get; set; }
            }
		}
    }
}