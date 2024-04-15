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
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiTrendLineCoreXF :
        ICloneable,
        IStiApplyStyle
    {
        #region IStiApplyStyle
        public virtual void ApplyStyle(IStiChartStyle style)
        {
            if (this.TrendLine.AllowApplyStyle)
            {
                this.TrendLine.LineColor = style.Core.TrendLineColor;
                this.TrendLine.ShowShadow = style.Core.TrendLineShowShadow;
            }
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public abstract string LocalizedName
        {
            get;
        }
        #endregion

        #region Properties
        private IStiTrendLine trendLine;
        public IStiTrendLine TrendLine
        {
            get
            {
                return trendLine;
            }
            set
            {
                trendLine = value;
            }
        }
        #endregion

        #region Methods
        public virtual void RenderTrendLine(StiAreaGeom geom, PointF?[] points, float posY)
        {
        }

        protected float Sum(float[] values)
        {
            float sum = 0;
            foreach (float value in values)
            {
                sum += value;
            }
            return sum;
        }

        protected float SumSqr(float[] values)
        {
            float sum = 0;
            foreach (float value in values)
            {
                sum += value * value;
            }
            return sum;

        }

        protected float SumProductions(float[] valuesX, float[] valuesY)
        {
            float sum = 0;
            for (int index = 0; index < valuesX.Length; index++)
            {
                sum += valuesX[index] * valuesY[index];
            }
            return sum;
        }

        protected float SumProductionsXLogY(float[] valuesX, float[] valuesY)
        {
            float sum = 0;
            for (int index = 0; index < valuesX.Length; index++)
            {
                sum += valuesX[index] * (float)Math.Log(valuesY[index]);
            }
            return sum;

        }

        protected float SumLn(float[] values)
        {
            float sum = 0;
            foreach (float value in values)
            {
                sum += (float)Math.Log(value);
            }
            return sum;
        }
        #endregion

        public StiTrendLineCoreXF(IStiTrendLine trendLine)
        {
            this.trendLine = trendLine;
        }
    }
}
