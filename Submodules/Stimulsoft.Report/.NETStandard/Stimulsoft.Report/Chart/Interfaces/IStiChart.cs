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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    public interface IStiChart
    {
        #region Properties
        StiChartConditionsCollection SeriesLabelsConditions { get; set; }

        StiSeriesCollection Series { get; set; }

        IStiArea Area { get; set; }

        IStiChartStyle Style { get; set; }

        IStiSeriesLabels SeriesLabels { get; set; }

        IStiLegend Legend { get; set; }

        IStiChartTitle Title { get; set; }

        IStiChartTable Table { get; set; }

        StiStripsCollection Strips { get; set; }

        StiConstantLinesCollection ConstantLines { get; set; }

        int HorSpacing { get; set; }

        int VertSpacing { get; set; }

        bool IsDesigning { get; }

        StiBrush Brush { get; set; }

        List<StiAnimation> PreviousAnimations { get; set; }

        bool IsAnimationChangingValues { get; }
        #endregion

        #region Methods
        double ConvertToHInches(double value);
        #endregion

    }
}
