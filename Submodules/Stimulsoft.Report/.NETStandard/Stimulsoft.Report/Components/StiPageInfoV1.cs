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

using System.Collections;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Components
{	
	public class StiPageInfoV1 : StiComponentInfo
	{
		internal StiComponentsCollection Overlays;
	
		internal bool Processed;

		/// <summary>
		/// Gets a collection of delimiters. Used in cross-reports rendering.
		/// </summary>
		public Hashtable DelimiterComponentsLeft = new Hashtable();

		public bool IsFirstDataBandOnPage = true;
		
		public int PageNumber = 1;
		
		public int TotalPageCount = -1;		
		
		public double TopPageContentPosition;
		
		public double BottomPageContentPosition;
		
		public bool IsPrevPage;
		
		public bool RestoreCurrentDetailComponent = true;	
		
		public int NumberOfCopiesToPrint = 1;
	}
}
