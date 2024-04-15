﻿#region Copyright (C) 2003-2022 Stimulsoft
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

using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiSplineSeriesCoreXF : StiBaseLineSeriesCoreXF
    {
        #region Methods
        protected override void RenderLines(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo)
        {
            if (pointsInfo.Points == null) return;
            
            var seriesGeom = new StiSplineSeriesGeom(geom, pointsInfo, this.Series);

            if (seriesGeom != null)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesGeom);
            }

            #region Interaction
            if (this.Interaction != null)
            {
                seriesGeom.Interactions = GetInteractions(context, geom, pointsInfo.Points);
            }
            #endregion

            RenderMarkers(context, geom, pointsInfo.Points);
        }
        #endregion        

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "Spline");
            }
        }
        #endregion        

        public StiSplineSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
	}
}