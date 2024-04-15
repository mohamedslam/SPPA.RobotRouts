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

using Stimulsoft.Base.Blocks;
using Stimulsoft.Report.Events;
using System;

namespace Stimulsoft.Report.Gauge.Events
{
    /// <summary>
    /// Represents the method that handles the GetText event.
    /// </summary>
    public delegate void StiGetTextEventHandler(object sender, StiGetTextEventArgs e);

    /// <summary>
    /// Describes an argument for the event GetText.
    /// </summary>
    public class StiGetTextEventArgs :
        EventArgs,
        IStiBlocklyValueEventArgs
    {
        /// <summary>
        /// Gets or sets a text.
        /// </summary>
        public string Value { get; set; }

        public StiGetTextEventArgs()
        {
            
        }
    }
}