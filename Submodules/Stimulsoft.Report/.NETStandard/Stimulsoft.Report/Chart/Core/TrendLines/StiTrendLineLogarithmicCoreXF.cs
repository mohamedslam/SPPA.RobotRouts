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
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Stimulsoft.Base.Localization;


namespace Stimulsoft.Report.Chart
{
    public class StiTrendLineLogarithmicCoreXF : StiTrendLineCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("PropertyMain", "Logarithmic");
            }
        }
        #endregion

        #region Methods
        public override void RenderTrendLine(StiAreaGeom geom, PointF?[] points, float posY)
        {
            if (points.Length > 0)
            {
                float[] valuesX = new float[points.Length];
                float[] valuesY = new float[points.Length];

                float maxValue = points[0].Value.X;
                float minValue = points[0].Value.X;

                for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
                {
                    valuesX[pointIndex] = (float)Math.Log(points[pointIndex].Value.X);
                    valuesY[pointIndex] = points[pointIndex].Value.Y;

                    if (maxValue < points[pointIndex].Value.X) maxValue = points[pointIndex].Value.X;
                    if (minValue > points[pointIndex].Value.X) minValue = points[pointIndex].Value.X;
                }

                int countPoints = points.Length;

                float sumX = Sum(valuesX);
                float sumY = Sum(valuesY);
                float sumX2 = SumSqr(valuesX);
                float sumProductionsXY = SumProductions(valuesX, valuesY);

                float c = (sumX * sumX - sumX2 * countPoints);

                float a = (sumY * sumX - sumProductionsXY * countPoints) / c;
                float b = (sumX * sumProductionsXY - sumX2 * sumY) / c;

                PointF?[] pointTemp = new PointF?[31];

                for (int index = 0; index < 31; index++)
                {
                    float x = minValue + (maxValue - minValue) / 30 * index;
                    float y = a * (float)Math.Log(x) + b;
                    pointTemp[index] = new PointF(x, y);
                }

                StiTrendCurveGeom curve = new StiTrendCurveGeom(pointTemp, this.TrendLine);
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(curve);
            }
        }
        #endregion

        public StiTrendLineLogarithmicCoreXF(IStiTrendLine trendLine)
            : base(trendLine)
        {            
        }
    }
}
