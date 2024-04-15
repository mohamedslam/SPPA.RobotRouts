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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    public interface IStiConstantLines :
        ICloneable,
        IStiJsonReportObject
    {       
        #region Properties
        StiConstantLinesCoreXF Core { get; set; }

        bool AllowApplyStyle { get; set; }

        bool Antialiasing { get; set; }

        StiConstantLines.StiTextPosition Position { get;  set; }
        
        Font Font { get; set; }
        
        string Text { get; set; }
        
        bool TitleVisible { get; set; }
        
        StiConstantLines.StiOrientation Orientation { get; set; }

        float LineWidth { get; set; } 

        StiPenStyle LineStyle { get; set; }

        Color LineColor { get; set; }
        
        bool ShowInLegend { get; set; }

        bool ShowBehind { get; set; }
        
        string AxisValue { get; set; }
        
        bool Visible { get; set; } 

        IStiChart Chart { get; set; }
        #endregion
    }
}
