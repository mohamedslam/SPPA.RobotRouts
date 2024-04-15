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
            /// A Class for managing of the report resource files.
            /// </summary>
            public sealed class ReportResources
            {
                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("Property 'AllowUseResourcesInWinForms' is obsolete. Now report engine automatically decide how to store resources.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool AllowUseResourcesInWinForms { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("Property 'AllowUseResourcesInWPF' is obsolete. Now report engine automatically decide how to store resources.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool AllowUseResourcesInWPF { get; set; } = true;

                [DefaultValue(false)]
                [StiSerializable]
                [Obsolete("Property 'AllowUseResourcesInWeb' is obsolete. Now report engine automatically decide how to store resources.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool AllowUseResourcesInWeb { get; set; }

                /// <summary>
                /// Specifies path for the report resource files.
                /// </summary>
                [DefaultValue(null)]
                [StiSerializable]
                public static string ResourcesPath { get; set; }

                [DefaultValue(5000000)]
                [StiSerializable]
                public static int MaximumSize { get; set; } = 5000000;
            }
		}
    }
}