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

using System.Collections.Generic;

namespace Stimulsoft.Report.CrossTab.Core
{
	public class StiRow
	{
	    #region Properties
        public object HyperlinkValue { get; set; }

	    public object TagValue { get; set; }

	    public object ToolTipValue { get; set; }

	    public Dictionary<string, object> DrillDownParameters { get; set; } = null;

	    public bool IsTotal { get; set; }

	    public int Level { get; set; } = -1;

	    public StiRowCollection Rows { get; } = new StiRowCollection();

        public object Value { get; set; }

	    public object DisplayValue { get; set; }

        public string OthersText { get; set; }
        #endregion

        public StiRow(object value, object displayValue)
		{
			this.Value = value;
			this.DisplayValue = displayValue;
		}
	}
}
