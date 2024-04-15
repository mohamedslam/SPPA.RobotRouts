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

namespace Stimulsoft.Report.CrossTab.Core
{
	public class StiSummaryContainer
	{
		#region Fields
		private int level;
		private Hashtable dataCol = new Hashtable();
		#endregion

		#region Methods
		internal StiSummary GetSummary(StiColumn col, StiRow row)
		{
			return GetSummary(col, row, true); 
		}

		public StiSummary GetSummary(StiColumn col, StiRow row, bool create)
		{
			Hashtable dataRow = dataCol[col] as Hashtable;
			if (dataRow == null)
			{
				if (!create)return null;
				dataCol[col] = new Hashtable();
				dataRow = dataCol[col] as Hashtable;
			}

			StiSummary summary = dataRow[row] as StiSummary;
			if (summary == null)
			{
				if (!create)return null;
				summary = new StiSummary(level);
				dataRow[row] = summary;
			}
			return summary;
		}

		public Hashtable GetArguments(Hashtable argValues)
        {
			Hashtable argSums = new Hashtable();
			foreach (string key in argValues.Keys)
            {
				var value = argValues[key];
				var array = new ArrayList();
				array.Add(value);
				argSums[key] = array;
			}			
			return argSums;
		}

        public Hashtable GetDataCol()
        {
            return dataCol;
        }
        #endregion

        public StiSummaryContainer(int level)
		{
			this.level = level;
		}
	}
}
