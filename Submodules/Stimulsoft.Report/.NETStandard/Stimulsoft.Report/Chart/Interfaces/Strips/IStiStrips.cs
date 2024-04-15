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
using Stimulsoft.Base.Drawing;
using System;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    public interface IStiStrips :
        ICloneable,
        IStiJsonReportObject    
    {
        #region Properties
        StiStripsCoreXF Core { get; set; }
        
        bool AllowApplyStyle { get; set; }
        
        bool ShowBehind { get; set; }

        StiBrush StripBrush { get; set; }

        bool Antialiasing { get; set; }
        
        Font Font { get; set; }

        string Text { get; set; }
        
        bool TitleVisible { get; set; }

        Color TitleColor { get; set; }
        
        StiStrips.StiOrientation Orientation { get; set; }
        
        bool ShowInLegend { get; set; } 
        
        string MaxValue { get; set; }
        
        string MinValue { get; set; }
        
        bool Visible { get; set; }

        IStiChart Chart { get; set; }
        #endregion
	}
}
