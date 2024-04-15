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

using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using System;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public interface IStiGridLines : 
        ICloneable,
        IStiJsonReportObject,
		IStiDefault
	{
		#region Properties
        StiGridLinesCoreXF Core { get; set; }

        bool AllowApplyStyle { get; set; }

		Color Color { get; set; }
		
        Color MinorColor { get; set; }

		StiPenStyle Style { get; set; }

		StiPenStyle MinorStyle { get; set; }

		bool Visible { get; set; } 

		bool MinorVisible { get; set; }

		int MinorCount { get; set; }

        IStiArea Area { get; set; }
        #endregion
	}
}