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
using Stimulsoft.Base.Design;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{    
    public interface IStiLegend :
        ICloneable,
        IStiJsonReportObject,
		IStiDefault
	{       		
        #region Properties
        StiLegendCoreXF Core { get; set; }

        bool AllowApplyStyle { get; set;}

        IStiChart Chart { get; set; }

        bool HideSeriesWithEmptyTitle { get; set; }

		bool ShowShadow { get; set; }

		Color BorderColor { get; set; }

		StiBrush Brush { get; set; }

		Color TitleColor { get; set; }

		Color LabelsColor { get; set; }
	
		StiLegendDirection Direction { get; set; }	

		StiLegendHorAlignment HorAlignment { get;  set; }

		StiLegendVertAlignment VertAlignment { get; set; }

		Font TitleFont { get; set; }

		Font Font { get; set; }

		bool Visible { get; set; }

		bool MarkerVisible { get; set; }

        bool MarkerBorder { get; set; }

		Size MarkerSize { get; set; }

		StiMarkerAlignment MarkerAlignment { get;  set; }

		int Columns { get; set; }

		int HorSpacing { get; set; }

		int VertSpacing { get; set; }

		SizeD Size { get; set; }

		string Title { get; set; }

		int ColumnWidth { get; set; }

		bool WordWrap { get; set; }
	    #endregion
	}
}