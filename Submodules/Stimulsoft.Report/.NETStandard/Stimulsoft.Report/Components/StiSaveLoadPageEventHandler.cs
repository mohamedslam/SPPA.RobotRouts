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
	/// Represents the method that is invoked for the page saving and loading process into the report cache.
	/// </summary>
    public delegate void StiSaveLoadPageEventHandler(object sender, StiSaveLoadPageEventArgs e);

	/// <summary>
	/// Describes an argument for the event SavePageToCache and LoadPageFromCache.
	/// </summary>
    public class StiSaveLoadPageEventArgs : EventArgs
	{
	    /// <summary>
        /// Gets the index of the page in RenderedPages collection.
        /// </summary>
        public int PageIndex { get; }

	    /// <summary>
        /// Gets a path to the file of the page.
        /// </summary>
        public string CachePath { get; }

	    /// <summary>
        /// Gets the page to save or load.
        /// </summary>
        public StiPage Page { get; }

	    public StiSaveLoadPageEventArgs(StiPage page, int pageIndex, string cachePath)
        {
            this.PageIndex = pageIndex;
            this.CachePath = cachePath;
            this.Page = page;
        }
	}
}
