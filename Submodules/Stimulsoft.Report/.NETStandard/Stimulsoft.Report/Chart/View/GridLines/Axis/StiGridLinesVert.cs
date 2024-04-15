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

using System.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Chart
{
	public class StiGridLinesVert : 
        StiGridLines,
        IStiGridLinesVert
	{
		public StiGridLinesVert()
		{
		}

		[StiUniversalConstructor("GridLinesVert")]
		public StiGridLinesVert(
			Color color,
			StiPenStyle style,
			bool visible,
			Color minorColor,			
			StiPenStyle minorStyle,			
			bool minorVisible,
			int minorCount,
            bool allowApplyStyle
			) : 
			base (
			color,
			style,
			visible,
			minorColor,			
			minorStyle,			
			minorVisible,
			minorCount,
            allowApplyStyle)
		{
		}
	}
}
