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
using Stimulsoft.Report.Components.TextFormats;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    public interface IStiSeriesLabels : 
        ICloneable,
        IStiJsonReportObject
	{
        #region Properties
        bool AllowApplyStyle { get; set; }

        bool ShowZeros { get; set; }

        bool ShowNulls { get; set; }
        
		bool MarkerVisible { get; set; }

        int Step { get; set; }

        string ValueTypeSeparator { get; set; }

		string TextBefore { get; set; }

		string TextAfter { get; set; } 

		float Angle { get; set; }
        
		string Format { get; set; }
        
		bool Antialiasing { get; set; }
        
		bool Visible { get; set; }
        
		bool DrawBorder { get; set; }
		
		bool UseSeriesColor { get; set; }
		
        StiMarkerAlignment MarkerAlignment { get; set; }
        
        StiSeriesLabelsValueType ValueType { get; set; }

        StiSeriesLabelsValueType LegendValueType { get; set; }

        Size MarkerSize { get; set; }

        Color LabelColor { get; set; }

        Color BorderColor { get; set; }

        StiBrush Brush { get; set; } 

        Font Font { get; set; }

        IStiChart Chart { get; set; }

        StiSeriesLabelsCoreXF Core { get; set; }

        bool PreventIntersection { get; set; }

        bool WordWrap { get; set; }

        int Width { get; set; }

        StiFormatService FormatService { get; set; }
        #endregion
    }
}
