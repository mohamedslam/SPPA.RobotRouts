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

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
    {
        #region Preview
        /// <summary>
        /// Class for adjustment of the preview of a report.
        /// </summary>
        [Obsolete("Please use property 'StiOptions.Viewer' instead 'StiOptions.Preview' property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed class Preview : Viewer
		{
			[Obsolete("Please use property 'StiOptions.Viewer.DotMatrix' instead 'StiOptions.Preview.DotMatrixWindow' property.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public sealed class DotMatrixWindow : DotMatrix
			{
			}

			[Obsolete("Please use property 'StiOptions.Viewer.Windows' instead 'StiOptions.Preview.Window' property.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public sealed class Window : Windows
			{
			}
		}
		#endregion
    }
}