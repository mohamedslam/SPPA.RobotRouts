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
    public class StiTrendLineLinearCoreXF : StiTrendLineCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("PropertyMain", "Linear");
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
                    valuesX[pointIndex] = points[pointIndex].Value.X;
                    valuesY[pointIndex] = points[pointIndex].Value.Y;

                    if (maxValue < valuesX[pointIndex]) maxValue = valuesX[pointIndex];
                    if (minValue > valuesX[pointIndex]) minValue = valuesX[pointIndex];
                }

                int countPoints = points.Length;

                float sumX = Sum(valuesX);
                float sumY = Sum(valuesY);
                float sumX2 = SumSqr(valuesX);
                float sumProductionsXY = SumProductions(valuesX, valuesY);

                float c = (sumX * sumX - sumX2 * countPoints);

                float a = (sumY * sumX - sumProductionsXY * countPoints) / c;
                float b = (sumX * sumProductionsXY - sumX2 * sumY) / c;

                StiTrendLineGeom line = new StiTrendLineGeom(new PointF(minValue, a * minValue + b),
                    new PointF(maxValue, a * maxValue + b),
                    this.TrendLine);
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(line);
            }
        }
        #endregion

        public StiTrendLineLinearCoreXF(IStiTrendLine trendLine)
            : base(trendLine)
        {            
        }
    }
}
