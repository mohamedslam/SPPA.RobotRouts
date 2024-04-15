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
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Report.CrossTab.Core
{
	public class StiSummary
	{
	    #region Properties
        public ArrayList[] Sums { get; }

		public Hashtable[] Arguments { get; set; }

		public object[] HyperlinkValues { get; set; }

	    public object[] TagValues { get; set; }

	    public object[] ToolTipValues { get; set; }

	    public Dictionary<string, object>[] DrillDownParameters { get; set; }
        #endregion

        public StiSummary(int level)
		{
			Sums = new ArrayList[level];
            HyperlinkValues = new object[level];
            TagValues = new object[level];
            ToolTipValues = new object[level];
			Arguments = new Hashtable[level];

			for (int index = 0; index < level; index ++)
			{
				Sums[index] = new ArrayList();
				Arguments[index] = new Hashtable();
			}
		}
	}
}
