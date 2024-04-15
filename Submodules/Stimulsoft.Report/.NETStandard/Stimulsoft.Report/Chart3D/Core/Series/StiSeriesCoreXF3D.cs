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
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiSeriesCoreXF3D : StiSeriesCoreXF
    {
        #region Methods
        public abstract void RenderSeries3D(StiRender3D render3D, StiContext context, StiRectangle3D rect, StiAreaGeom geom, int seriesIndex, IStiSeries[] seriesArray);

        public abstract Color GetSeriesColor(int colorIndex, int colorCount, Color color);
        #endregion

        protected StiSeriesCoreXF3D(IStiSeries series) : base(series)
        {
        }
    }
}
