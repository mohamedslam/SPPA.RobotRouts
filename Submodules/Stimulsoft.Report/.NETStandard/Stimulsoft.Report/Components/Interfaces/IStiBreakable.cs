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
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Describes interface of breakable feature.
	/// </summary>
	public interface IStiBreakable
	{
		/// <summary>
		/// Gets or sets value which indicates whether the component can or cannot break its contents on several pages.
		/// </summary>
		bool CanBreak
		{
			get;
			set;			
		}

		/// <summary>
		/// Divides content of components in two parts. Returns result of dividing. If true, then component is successful divided.
		/// </summary>
		/// <param name="dividedComponent">Component for store part of content.</param>
		/// <returns>If true, then component is successful divided.</returns>
        bool Break(StiComponent dividedComponent, double devideFactor, ref double divideLine);
	}
}
