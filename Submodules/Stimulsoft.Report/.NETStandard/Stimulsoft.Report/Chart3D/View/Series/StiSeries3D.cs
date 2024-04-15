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

using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiSeries3D : StiSeries
    {
        #region Properties
        [Browsable(false)]
        public override bool ShowShadow
        {
            get
            {
                return base.ShowShadow;
            }
            set
            {
                base.ShowShadow = value;
            }
        }

        public override IStiSeriesInteraction Interaction
        {
            get
            {
                return base.Interaction;
            }
            set
            {
                base.Interaction = value;
            }
        }

        [Browsable(false)]
        public override StiTrendLinesCollection TrendLines
        {
            get
            {
                return base.TrendLines;
            }
            set
            {
                base.TrendLines = value;
            }
        }

        [Browsable(false)]
        public override StiSeriesYAxis YAxis
        {
            get
            {
                return base.YAxis;
            }
            set
            {
                base.YAxis = value;
            }
        } 
        #endregion
    }
}
