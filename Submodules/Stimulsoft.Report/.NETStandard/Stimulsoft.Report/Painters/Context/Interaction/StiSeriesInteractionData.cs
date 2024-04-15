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

using Stimulsoft.Report.Chart;
using System.Drawing;

namespace Stimulsoft.Base.Context
{
    public class StiSeriesInteractionData : StiInteractionData
    {
        #region Properties
        public bool IsElements { get; set; } = true;

        public object Tag { get; private set; }

        public string Tooltip { get; private set; }

        public string Hyperlink { get; private set; }

        public object Argument { get; private set; }

        public object OriginalArgument { get; private set; }

        public double? EndValue { get; private set; }

        public IStiSeries Series { get; set; }

        public int PointIndex { get; private set; }

        public PointF? Point { get; set; }
        #endregion

        #region Methods
        public void Fill(IStiArea area, IStiSeries series, int pointIndex)
        {
            if (area is IStiAxisArea && ((IStiAxisArea)area).ReverseHor)
            {
                this.PointIndex = series.Values.Length - pointIndex - 1;
                this.Series = series;

                if (pointIndex >= 0 && pointIndex < series.Arguments.Length)
                    Argument = series.Arguments[series.Arguments.Length - pointIndex - 1];

                if (series.OriginalArguments != null && pointIndex >= 0 && pointIndex < series.OriginalArguments.Length)
                    OriginalArgument = series.OriginalArguments[series.OriginalArguments.Length - pointIndex - 1];

                if (pointIndex >= 0 && pointIndex < series.Values.Length)
                    Value = series.Values[series.Values.Length - pointIndex - 1];

                if (pointIndex >= 0 && pointIndex < series.Tags.Length)
                    Tag = series.Tags[series.Tags.Length - pointIndex - 1];

                if (pointIndex >= 0 && pointIndex < series.Hyperlinks.Length)
                    Hyperlink = series.Hyperlinks[series.Hyperlinks.Length - pointIndex - 1];

                if (pointIndex >= 0 && pointIndex < series.ToolTips.Length)
                    Tooltip = series.ToolTips[series.ToolTips.Length - pointIndex - 1];

                var rangeSeries = series as IStiRangeSeries;

                if (rangeSeries != null && pointIndex >= 0 && pointIndex < rangeSeries.ValuesEnd.Length)
                    EndValue = rangeSeries.ValuesEnd[rangeSeries.ValuesEnd.Length - pointIndex - 1];
            }
            else
            {
                this.PointIndex = pointIndex;
                this.Series = series;

                if (pointIndex >= 0 && pointIndex < series.Arguments.Length)
                    Argument = series.Arguments[pointIndex];

                if (series.OriginalArguments != null && pointIndex >= 0 && pointIndex < series.OriginalArguments.Length)
                    OriginalArgument = series.OriginalArguments[pointIndex];

                if (pointIndex >= 0 && pointIndex < series.Values.Length)
                    Value = series.Values[pointIndex];

                if (pointIndex >= 0 && pointIndex < series.Tags.Length)
                    Tag = series.Tags[pointIndex];

                if (pointIndex >= 0 && pointIndex < series.Hyperlinks.Length)
                    Hyperlink = series.Hyperlinks[pointIndex];

                if (pointIndex >= 0 && pointIndex < series.ToolTips.Length)
                    Tooltip = series.ToolTips[pointIndex];

                var rangeSeries = series as IStiRangeSeries;

                if (rangeSeries != null && pointIndex >= 0 && pointIndex < rangeSeries.ValuesEnd.Length)
                    EndValue = rangeSeries.ValuesEnd[pointIndex];
            }
        }
        #endregion

        public StiSeriesInteractionData() : this(null, null, 0)
        {
        }

        public StiSeriesInteractionData(IStiArea area, IStiSeries series, int pointIndex)
        {
            if (area != null || series != null)
                Fill(area, series, pointIndex);
        }
    }
}
