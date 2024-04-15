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
    /// Represents the method that handles the Pasted event.
    /// </summary>
    public delegate void StiPastedEventHandler(object sender, StiPastedEventArgs e);

    /// <summary>
    /// Describes an argument for the event Pasted.
    /// </summary>
    public class StiPastedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets collection of selected components.
        /// </summary>
        public StiComponentsCollection Components { get; }

        public StiPastedEventArgs(StiComponent comp)
        {
            this.Components = new StiComponentsCollection { comp };
        }

        public StiPastedEventArgs(StiComponentsCollection comps)
        {
            this.Components = comps;
        }
    }
}
