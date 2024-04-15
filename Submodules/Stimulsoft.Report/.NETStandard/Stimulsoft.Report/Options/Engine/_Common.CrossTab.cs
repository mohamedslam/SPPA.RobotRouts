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
using Stimulsoft.Report.Chart;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// A class which controls of settings of the report engine. 
        /// </summary>
		public sealed partial class Engine
        {
            /// <summary>
            /// If true, the report engine can forcibly set the CanBreak property to true for DataBand, if the CrossTab component with the enabled PrintTitlesOnAllPages property is put on this DataBand.
            /// </summary>
            [DefaultValue(true)]
            [Description("If true, the report engine can forcibly set the CanBreak property to true for DataBand, if the CrossTab component with the enabled PrintTitlesOnAllPages property is put on this DataBand.")]
            [StiSerializable]
            [Category("CrossTab")]
            [Obsolete("This property is not work anymore.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static bool AllowForceCanBreakForCrossTabPrintOnAllPages { get; set; } = true;
        }
    }
}
