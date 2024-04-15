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

namespace Stimulsoft.Report.Events
{
    /// <summary>
    /// Represents the method that handles the AllowClipboardOperation event.
    /// </summary>
    public delegate void StiAllowClipboardOperationEventHandler(object sender, StiAllowClipboardOperationEventArgs e);

    /// <summary>
    /// Describes an argument for the event AllowClipboardOperation.
    /// </summary>
    public class StiAllowClipboardOperationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets collection of selected components.
        /// </summary>
        public StiComponentsCollection Components { get; }

        /// <summary>
        /// Gets or sets a value which indicates that clipboard operation is allowed or not.
        /// </summary>
        public virtual bool Allow { get; set; } = true;

        public StiAllowClipboardOperationEventArgs(StiComponentsCollection comps)
        {
            this.Components = comps;
        }
    }
}
