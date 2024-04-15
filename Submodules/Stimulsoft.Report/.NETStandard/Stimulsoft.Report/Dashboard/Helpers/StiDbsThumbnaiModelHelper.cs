#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System;
using System.Collections.Generic;

namespace Stimulsoft.Report.Dashboard.Helpers
{
    public static class StiDbsThumbnaiModelHelper
    {
        #region Fields
        private static object lockPage = new object();
        private static List<StiPage> cache = new List<StiPage>();
        #endregion

        #region Methods
        public static void Add(StiPage page)
        {
            if (page == null) return;

            lock (lockPage)
            {
                if (!cache.Contains(page))
                    cache.Add(page);
            }
        }

        public static void Remove(StiPage page)
        {
            if (page == null) return;

            lock (lockPage)
            {
                if (cache.Contains(page))
                    cache.Remove(page);
            }
        }

        public static bool AllowPage(StiPage page)
        {
            if (page == null) return false;

            lock (lockPage)
            {
                return cache.Contains(page);
            }
        }
        #endregion
    }
}
